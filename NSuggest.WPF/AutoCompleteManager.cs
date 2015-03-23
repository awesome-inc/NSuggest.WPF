﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Windows.Themes;

namespace NSuggest.WPF
{
    public class AutoCompleteManager
    {
        // ReSharper disable InconsistentNaming
        const int WM_NCLBUTTONDOWN = 0x00A1;
        const int WM_NCRBUTTONDOWN = 0x00A4;
        // ReSharper restore InconsistentNaming
        const int PopupShadowDepth = 5;

        #region Internal States

        double _itemHeight;
        double _downWidth;
        double _downHeight;
        double _downTop;
        Point _ptDown;

        bool _popupOnTop = true;
        bool _manualResized;
        string _textBeforeChangedByCode;
        bool _textChangedByCode;

        TextBox _textBox;
        Popup _popup;
        SystemDropShadowChrome _chrome;
        ListBox _listBox;
        ScrollBar _scrollBar;
        ResizeGrip _resizeGrip;
        ScrollViewer _scrollViewer;
        Thread _asyncThread;

        bool _disabled;
        bool _supressAutoAppend;

        #endregion

        #region Public Interface

        public IProvideSuggestions DataProvider { get; set; }

        public bool Disabled
        {
            get { return _disabled; }
            set
            {
                _disabled = value;
                if (_disabled && _popup != null)
                {
                    _popup.IsOpen = false;
                }
            }
        }

        public bool AutoCompleting
        {
            get { return _popup != null && _popup.IsOpen; }
        }

        public bool Asynchronous { get; set; }

        public bool AutoAppend { get; set; }

        #endregion

        #region Construction

        public AutoCompleteManager()
        {
        }

        public AutoCompleteManager(TextBox textBox)
        {
            AttachTextBox(textBox);
        }

        #endregion

        #region Initialization

        public void AttachTextBox(TextBox textBox)
        {
            if (textBox == null)
                throw new ArgumentNullException("textBox");

            if (Application.Current.Resources.FindName("AcTb_ListBoxStyle") == null)
            {
                var myResourceDictionary = new ResourceDictionary();
                // TODO: hard coded namespace. this can break on namespace changes! avoid that!
                var uri = new Uri("/NSuggest.WPF;component/Resources.xaml", UriKind.Relative);
                myResourceDictionary.Source = uri;
                Application.Current.Resources.MergedDictionaries.Add(myResourceDictionary);
            }

            //
            _textBox = textBox;

            // when not in design mode, hook up initialization to owner window events
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(textBox)) return;
            var ownerWindow = Window.GetWindow(textBox);
            // ReSharper disable PossibleNullReferenceException
            // ReSharper disable InvocationIsSkipped
            Debug.Assert(ownerWindow != null, "ownerWindow != null");
            if (ownerWindow.IsLoaded)
                Initialize();
            else
                ownerWindow.Loaded += OwnerWindowLoaded;
            ownerWindow.LocationChanged += OwnerWindowLocationChanged;
            // ReSharper restore InvocationIsSkipped
            // ReSharper restore PossibleNullReferenceException
        }

        void OwnerWindowLocationChanged(object sender, EventArgs e)
        {
            _popup.IsOpen = false;
        }

        void OwnerWindowLoaded(object sender, RoutedEventArgs e)
        {
            Initialize();
        }

        void Initialize()
        {
            _listBox = new ListBox();
            var tempItem = new ListBoxItem { Content = "TEMP_ITEM_FOR_MEASUREMENT" };
            _listBox.Items.Add(tempItem);
            _listBox.Focusable = false;
            _listBox.Style = (Style) Application.Current.Resources["AcTb_ListBoxStyle"];

            _chrome = new SystemDropShadowChrome
            {
                Margin = new Thickness(0, 0, PopupShadowDepth, PopupShadowDepth), 
                Child = _listBox
            };

            _popup = new Popup
            {
                SnapsToDevicePixels = true,
                AllowsTransparency = true,
                MinHeight = SystemParameters.HorizontalScrollBarHeight + PopupShadowDepth,
                MinWidth = SystemParameters.VerticalScrollBarWidth + PopupShadowDepth,
                VerticalOffset = SystemParameters.PrimaryScreenHeight + 100,
                Child = _chrome,
                IsOpen = true
            };

            _itemHeight = tempItem.ActualHeight;
            _listBox.Items.Clear();

            //
            GetInnerElementReferences();
            UpdateGripVisual();
            SetupEventHandlers();
        }

        void GetInnerElementReferences()
        {
            var border = (Border)_listBox.Template.FindName("Border", _listBox);
            _scrollViewer = (ScrollViewer)border.Child;
            _resizeGrip = _scrollViewer.Template.FindName("ResizeGrip", _scrollViewer) as ResizeGrip;
            _scrollBar = _scrollViewer.Template.FindName("PART_VerticalScrollBar", _scrollViewer) as ScrollBar;
        }

        void UpdateGripVisual()
        {
            var rectSize = SystemParameters.VerticalScrollBarWidth;
            var triangle = (Path)_resizeGrip.Template.FindName("RG_TRIANGLE", _resizeGrip);
            var pg = (PathGeometry)triangle.Data;
            pg = pg.CloneCurrentValue();
            var figure = pg.Figures[0];
            var p = figure.StartPoint;
            p.X = rectSize;
            figure.StartPoint = p;
            var line = (PolyLineSegment)figure.Segments[0];
            p = line.Points[0];
            p.Y = rectSize;
            line.Points[0] = p;
            p = line.Points[1];
            p.X = p.Y = rectSize;
            line.Points[1] = p;
            triangle.Data = pg;
        }

        void SetupEventHandlers()
        {
            var ownerWindow = Window.GetWindow(_textBox);
            // ReSharper disable InvocationIsSkipped
            // ReSharper disable PossibleNullReferenceException
            Debug.Assert(ownerWindow != null, "ownerWindow != null");
            ownerWindow.PreviewMouseDown += OwnerWindowPreviewMouseDown;
            ownerWindow.Deactivated += OwnerWindowDeactivated;
            // ReSharper restore PossibleNullReferenceException
            // ReSharper restore InvocationIsSkipped

            var wih = new WindowInteropHelper(ownerWindow);
            var hwndSource = HwndSource.FromHwnd(wih.Handle);
            // ReSharper disable InvocationIsSkipped
            // ReSharper disable PossibleNullReferenceException
            Debug.Assert(hwndSource != null, "hwndSource != null");
            var hwndSourceHook = new HwndSourceHook(HookHandler);
            hwndSource.AddHook(hwndSourceHook);
            //hwndSource.RemoveHook();?
            // ReSharper restore PossibleNullReferenceException
            // ReSharper restore InvocationIsSkipped

            _textBox.TextChanged += TextBoxTextChanged;
            _textBox.PreviewKeyDown += TextBoxPreviewKeyDown;
            _textBox.LostFocus += TextBoxLostFocus;

            _listBox.PreviewMouseLeftButtonDown += ListBoxPreviewMouseLeftButtonDown;
            _listBox.MouseLeftButtonUp += ListBoxMouseLeftButtonUp;
            _listBox.PreviewMouseMove += ListBoxPreviewMouseMove;

            _resizeGrip.PreviewMouseLeftButtonDown += ResizeGripPreviewMouseLeftButtonDown;
            _resizeGrip.PreviewMouseMove += ResizeGripPreviewMouseMove;
            _resizeGrip.PreviewMouseUp += ResizeGripPreviewMouseUp;
        }

        #endregion

        #region TextBox Event Handling

        void TextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            if (_textChangedByCode || Disabled || DataProvider == null)
                return;

            var text = _textBox.Text;
            if (string.IsNullOrEmpty(text))
            {
                _popup.IsOpen = false;
                return;
            }

            if (Asynchronous)
            {
                if (_asyncThread != null && _asyncThread.IsAlive)
                    _asyncThread.Abort();

                _asyncThread = new Thread(() => 
                {
                    var dispatcher = _textBox.Dispatcher; // Application.Current.Dispatcher;
                    var currentText = (String)dispatcher.Invoke((Func<TextBox,string>)(txtBox => txtBox.Text), _textBox);
                    if (text != currentText)
                        return;
                    var items = GetSuggestions(text);
                    dispatcher.Invoke((Action<IEnumerable<string>>)(PopulatePopupList), items);
                });
                _asyncThread.Start();
            }
            else
            {
                var items = GetSuggestions(text);
                PopulatePopupList(items);
            }
        }

        private IEnumerable<string> GetSuggestions(string text)
        {
            try
            {
                return DataProvider.For(text);
            }
            catch (Exception ex)
            {
                Trace.TraceError("Could not get suggestions for \"{0}\": {1}", text, ex);
                return Enumerable.Empty<string>();
            }
        }

        void TextBoxPreviewKeyDown(object sender, KeyEventArgs e)
        {
            _supressAutoAppend = e.Key == Key.Delete || e.Key == Key.Back;
            if (!_popup.IsOpen)
                return;

            if (e.Key == Key.Enter)
            {
                _popup.IsOpen = false;
                _textBox.SelectAll();
                return;
            }

            if (e.Key == Key.Escape)
            {
                _popup.IsOpen = false;
                e.Handled = true;
                return;
            }

            var index = _listBox.SelectedIndex;
            switch (e.Key)
            {
                case Key.PageUp:
                    switch (index)
                    {
                        case -1:
                            index = _listBox.Items.Count - 1;
                            break;
                        case 0:
                            index = -1;
                            break;
                        default:
                            if (index == (int)_scrollBar.Value)
                            {
                                index -= (int) _scrollBar.ViewportSize;
                                if (index < 0)
                                    index = 0;
                            }
                            else
                            {
                                index = (int) _scrollBar.Value;
                            }
                            break;
                    }
                    break;
                case Key.PageDown:
                    if (index == -1)
                    {
                        index = 0;
                    }
                    else if (index == _listBox.Items.Count - 1)
                    {
                        index = -1;
                    }
                    else if (index == (int)(_scrollBar.Value + _scrollBar.ViewportSize) - 1)
                    {
                        index += (int)_scrollBar.ViewportSize - 1;
                        if (index > _listBox.Items.Count - 1)
                        {
                            index = _listBox.Items.Count - 1;
                        }
                    }
                    else
                    {
                        index = (int) (_scrollBar.Value + _scrollBar.ViewportSize - 1);
                    }
                    break;
                case Key.Up:
                    if (index == -1)
                    {
                        index = _listBox.Items.Count - 1;
                    }
                    else
                    {
                        --index;
                    }
                    break;
                case Key.Down:
                    ++index;
                    break;
            }

            if (index != _listBox.SelectedIndex)
            {
                string text;
                if (index < 0 || index > _listBox.Items.Count - 1)
                {
                    text = _textBeforeChangedByCode;
                    _listBox.SelectedIndex = -1;
                }
                else
                {
                    _listBox.SelectedIndex = index;
                    _listBox.ScrollIntoView(_listBox.SelectedItem);
                    text = _listBox.SelectedItem as string;
                }
                UpdateText(text, false);
                e.Handled = true;
            }
        }

        void TextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            _popup.IsOpen = false;
        }

        #endregion

        #region ListBox event handling

        void ListBoxPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var pos = e.GetPosition(_listBox);
            var hitTestResult = VisualTreeHelper.HitTest(_listBox, pos);
            if (hitTestResult == null)
            {
                return;
            }
            var d = hitTestResult.VisualHit;
            while (d != null)
            {
                if (d is ListBoxItem)
                {
                    e.Handled = true;
                    break;
                }
                d = VisualTreeHelper.GetParent(d);
            }
        }

        void ListBoxPreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (Mouse.Captured != null)
            {
                return;
            }
            var pos = e.GetPosition(_listBox);
            var hitTestResult = VisualTreeHelper.HitTest(_listBox, pos);
            if (hitTestResult == null)
            {
                return;
            }
            var d = hitTestResult.VisualHit;
            while (d != null)
            {
                if (d is ListBoxItem)
                {
                    var item = (d as ListBoxItem);
                    item.IsSelected = true;
//                    _listBox.ScrollIntoView(item);
                    break;
                }
                d = VisualTreeHelper.GetParent(d);
            }
        }

        void ListBoxMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ListBoxItem item = null;
            var d = e.OriginalSource as DependencyObject;
            while (d != null)
            {
                if (d is ListBoxItem)
                {
                    item = d as ListBoxItem;
                    break;
                }
                d = VisualTreeHelper.GetParent(d);
            }
            if (item != null)
            {
                _popup.IsOpen = false;
                UpdateText(item.Content as string, true);
            }
        }

        #endregion

        #region ResizeGrip event handling

        void ResizeGripPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _downWidth = _chrome.ActualWidth + PopupShadowDepth;
            _downHeight = _chrome.ActualHeight + PopupShadowDepth;
            _downTop = _popup.VerticalOffset;

            var p = e.GetPosition(_resizeGrip);
            p = _resizeGrip.PointToScreen(p);
            _ptDown = p;

            _resizeGrip.CaptureMouse();
        }

        private const double Epsilon = 0.5; // half a pixel
        void ResizeGripPreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed)
            {
                return;
            }
            var ptMove = e.GetPosition(_resizeGrip);
            ptMove = _resizeGrip.PointToScreen(ptMove);
            var dx = ptMove.X - _ptDown.X;
            var dy = ptMove.Y - _ptDown.Y;
            var newWidth = _downWidth + dx;

            if (Math.Abs(newWidth - _popup.Width) > Epsilon && newWidth > 0)
            {
                _popup.Width = newWidth;
            }
            if (PopupOnTop)
            {
                var bottom = _downTop + _downHeight;
                var newTop = _downTop + dy;
                if (Math.Abs(newTop - _popup.VerticalOffset) > Epsilon && newTop < bottom - _popup.MinHeight)
                {
                    _popup.VerticalOffset = newTop;
                    _popup.Height = bottom - newTop;
                }
            }
            else
            {
                var newHeight = _downHeight + dy;
                if (Math.Abs(newHeight - _popup.Height) > Epsilon && newHeight > 0)
                {
                    _popup.Height = newHeight;
                }
            }
        }

        void ResizeGripPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            _resizeGrip.ReleaseMouseCapture();
            if (Math.Abs(_popup.Width - _downWidth) > Epsilon || Math.Abs(_popup.Height - _downHeight) > Epsilon)
            {
                _manualResized = true;
            }
        }

        #endregion

        #region Window event handling

        void OwnerWindowDeactivated(object sender, EventArgs e)
        {
            _popup.IsOpen = false;
        }

        void OwnerWindowPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!Equals(e.Source, _textBox))
                _popup.IsOpen = false;
        }

// ReSharper disable RedundantAssignment
        IntPtr HookHandler(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
// ReSharper restore RedundantAssignment
        {
            handled = false;

            switch (msg)
            {
                case WM_NCLBUTTONDOWN: // pass through
                case WM_NCRBUTTONDOWN:
                    _popup.IsOpen = false;
                    break;
            }
            return IntPtr.Zero;
        }

        #endregion

        #region AutoCompleteTextBox States and Behaviours

        void PopulatePopupList(IEnumerable<string> items)
        {
            var text = _textBox.Text;
            
            _listBox.ItemsSource = items;
            if (_listBox.Items.Count == 0)
            {
                _popup.IsOpen = false;
                return;
            }
            var firstSuggestion = (string)_listBox.Items[0];
            if (_listBox.Items.Count == 1 && text.Equals(firstSuggestion, StringComparison.OrdinalIgnoreCase))
            {
                _popup.IsOpen = false;
            }
            else
            {
                _listBox.SelectedIndex = -1;
                _textBeforeChangedByCode = text;
                _scrollViewer.ScrollToHome();
                ShowPopup();

                //
                if (AutoAppend && !_supressAutoAppend && 
                     _textBox.SelectionLength == 0 && 
                     _textBox.SelectionStart == _textBox.Text.Length)
                {
                    _textChangedByCode = true;
                    try
                    {
                        var appendProvider = DataProvider as ISuggestSuffix;
                        var appendText = appendProvider != null 
                            ? appendProvider.For(text, firstSuggestion) 
                            : firstSuggestion.Substring(_textBox.Text.Length);

                        if (!string.IsNullOrEmpty(appendText))
                            _textBox.SelectedText = appendText;
                    }
                    finally
                    {
                        _textChangedByCode = false;
                    }
                }
            }
        }

        bool PopupOnTop
        {
            get { return _popupOnTop; }
            set
            {
                if (_popupOnTop == value)
                {
                    return;
                }
                _popupOnTop = value;
                if (_popupOnTop)
                {
                    _resizeGrip.VerticalAlignment = VerticalAlignment.Top;
                    _scrollBar.Margin = new Thickness(0, SystemParameters.HorizontalScrollBarHeight, 0, 0);
                    _resizeGrip.LayoutTransform = new ScaleTransform(1, -1);
                    _resizeGrip.Cursor = Cursors.SizeNESW;
                }
                else
                {
                    _resizeGrip.VerticalAlignment = VerticalAlignment.Bottom;
                    _scrollBar.Margin = new Thickness(0, 0, 0, SystemParameters.HorizontalScrollBarHeight);
                    _resizeGrip.LayoutTransform = Transform.Identity;
                    _resizeGrip.Cursor = Cursors.SizeNWSE;
                }
            }
        }

        void ShowPopup()
        {
            var popupOnTop = false;

            var p = new Point(0, _textBox.ActualHeight);
            p = _textBox.PointToScreen(p);
            var tbBottom = p.Y;

            p = new Point(0, 0);
            p = _textBox.PointToScreen(p);
            var tbTop = p.Y;

            _popup.HorizontalOffset = p.X;
            var popupTop = tbBottom;

            if (!_manualResized)
            {
                _popup.Width = _textBox.ActualWidth + PopupShadowDepth;
            }

            double popupHeight;
            if (_manualResized)
            {
                popupHeight = _popup.Height;
            }
            else
            {
                var visibleCount = Math.Min(16, _listBox.Items.Count + 1);
                popupHeight = visibleCount*_itemHeight + PopupShadowDepth;
            }
            var screenHeight = SystemParameters.PrimaryScreenHeight;
            if (popupTop + popupHeight > screenHeight)
            {
                if (screenHeight - tbBottom > tbTop)
                {
                    popupHeight = SystemParameters.PrimaryScreenHeight - popupTop;
                }
                else
                {
                    popupOnTop = true;
                    popupTop = tbTop - popupHeight + 4;
                    if (popupTop < 0)
                    {
                        popupTop = 0;
                        popupHeight = tbTop + 4;
                    }
                }
            }
            PopupOnTop = popupOnTop;
            _popup.Height = popupHeight;
            _popup.VerticalOffset = popupTop;

            _popup.IsOpen = true;
        }

        void UpdateText(string text, bool selectAll)
        {
            _textChangedByCode = true;
            _textBox.Text = text;
            if (selectAll)
            {
                _textBox.SelectAll();
            }
            else
            {
                _textBox.SelectionStart = text.Length;
            }
            _textChangedByCode = false;
        }

        #endregion
    }
}
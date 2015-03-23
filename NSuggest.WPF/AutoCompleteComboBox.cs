﻿using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace NSuggest.WPF
{
    public class AutoCompleteComboBox : ComboBox
    {
        readonly AutoCompleteManager _acm = new AutoCompleteManager();
        TextBox _textBox;
        int _oldSelStart;
        int _oldSelLength;
        string _oldText;

        public AutoCompleteManager AutoCompleteManager { get { return _acm; } }

        public AutoCompleteComboBox()
        {
            IsEditable = true;
            IsTextSearchEnabled = false;
            GotMouseCapture += AutoCompleteComboBoxGotMouseCapture;
        }

        void AutoCompleteComboBoxGotMouseCapture(object sender, MouseEventArgs e)
        {
            _oldSelStart = _textBox.SelectionStart;
            _oldSelLength = _textBox.SelectionLength;
            _oldText = _textBox.Text;
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (_acm.AutoCompleting)
            {
                return;
            }
            if (e.Key == Key.Up || e.Key == Key.Down)
            {
                SelectedValue = Text;
            }
            base.OnPreviewKeyDown(e);
        }

        protected override void OnDropDownOpened(EventArgs e)
        {
            _acm.Disabled = true;
            IsTextSearchEnabled = true;
            SelectedValue = Text;

            base.OnDropDownOpened(e);

            if (SelectedValue != null) return;
            Text = _oldText;
            _textBox.SelectionStart = _oldSelStart;
            _textBox.SelectionLength = _oldSelLength;
        }

        protected override void OnDropDownClosed(EventArgs e)
        {
            base.OnDropDownClosed(e);
            _acm.Disabled = false;
            IsTextSearchEnabled = false;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _textBox = GetTemplateChild("PART_EditableTextBox") as TextBox;
            _acm.AttachTextBox(_textBox);
        }
    }
}
using System.Windows;
using NSuggest;
using NSuggest.WPF;
using TestSuggestions.Gisgraphy;

namespace TestSuggestions
{
    public partial class MainWindow
    {
        readonly AutoCompleteManager _acmRegistryPath;
        readonly AutoCompleteManager _acmGeocoding;

        public MainWindow()
        {
            InitializeComponent();
            
            _acmRegistryPath = new AutoCompleteManager(txtRegistryPath)
            {
                DataProvider = new RegistrySuggestions()
            };

            accbStates.ItemsSource = StateData.States;
            accbStates.AutoCompleteManager.DataProvider = new StaticDataSuggestions(StateData.States);

            _acmGeocoding = new AutoCompleteManager(txtGeocoding)
            {
                //DataProvider = new SuggestionsProxy(new GisgraphySuggestions())
                DataProvider = new KeyedValuesProxy<FullTextSearch.Document>(new GisgraphySuggestions())
                {
                    BlackListSize = 3000,
                    CacheSize = 3000,
                    MaxFailures = 3
                },
                Asynchronous = true
            };

            actbFileSysPath.AutoCompleteManager.DataProvider = new FileSystemSuggestions();
        }

        void ChkIncludeFilesClick(object sender, RoutedEventArgs e)
        {
            var fileSysDataProvider = (FileSystemSuggestions)actbFileSysPath.AutoCompleteManager.DataProvider;
            fileSysDataProvider.IncludeFiles = chkIncludeFiles.IsChecked ?? false;
        }

        void ChkAutoAppendClick(object sender, RoutedEventArgs e)
        {
            var autoAppend = chkAutoAppend.IsChecked ?? false;
            _acmRegistryPath.AutoAppend =
                _acmGeocoding.AutoAppend =
                accbStates.AutoCompleteManager.AutoAppend =
                actbFileSysPath.AutoCompleteManager.AutoAppend = autoAppend;
        }
    }
}

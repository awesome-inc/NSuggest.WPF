using System;
using System.Windows;
using NSuggest;
using NSuggest.Rest;
using NSuggest.WPF;
using TestSuggestions.Gisgraphy;

namespace TestSuggestions
{
    public partial class MainWindow
    {
        private readonly AutoCompleteManager _acmRegistryPath;
        private readonly AutoCompleteManager _acmGeocoding;
        private readonly AutoCompleteManager _acmElasticSearch;

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

            // elasticsearch
            _acmElasticSearch = new AutoCompleteManager(txtEsSearch)
            {
                //DataProvider = new SuggestionsProxy(new GisgraphySuggestions())
                DataProvider = new SuggestionsProxy(new ElasticSearchSuggestions(
                    new RestClient() { BaseAddress = new Uri("http://localhost:9200/movies/movie") },
                    new[] { "producer", "director" }))
                {
                    BlackListSize = 3000,
                    CacheSize = 3000,
                    MaxFailures = 3
                },
                Asynchronous = true
            };
        }

        private void ChkIncludeFilesClick(object sender, RoutedEventArgs e)
        {
            var fileSysDataProvider = (FileSystemSuggestions)actbFileSysPath.AutoCompleteManager.DataProvider;
            fileSysDataProvider.IncludeFiles = chkIncludeFiles.IsChecked ?? false;
        }

        private void ChkAutoAppendClick(object sender, RoutedEventArgs e)
        {
            var autoAppend = chkAutoAppend.IsChecked ?? false;
            _acmRegistryPath.AutoAppend =
                _acmGeocoding.AutoAppend =
                accbStates.AutoCompleteManager.AutoAppend =
                actbFileSysPath.AutoCompleteManager.AutoAppend = autoAppend;
        }
    }
}

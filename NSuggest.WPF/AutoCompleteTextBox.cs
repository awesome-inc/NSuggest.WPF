using System.Windows.Controls;

namespace NSuggest.WPF
{
    public class AutoCompleteTextBox : TextBox
    {
        public AutoCompleteManager AutoCompleteManager { get; private set; }

        public AutoCompleteTextBox()
        {
            AutoCompleteManager = new AutoCompleteManager();
            Loaded += (s, e) => AutoCompleteManager.AttachTextBox(this);
        }
    }
}

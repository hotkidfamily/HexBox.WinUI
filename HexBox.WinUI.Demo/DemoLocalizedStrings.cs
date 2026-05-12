using Microsoft.Windows.ApplicationModel.Resources;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HexBox.WinUI.Demo
{
    public class DemoLocalizedStrings : INotifyPropertyChanged
    {
        private const string _resSpace = "HexBox.WinUI.Demo";
        private static readonly ResourceManager _rm = new();

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public DemoLocalizedStrings()
        {
            Reload();
        }

        public void Reload()
        {
            BrowseButton = GetValue("BrowseButton/Content");
            FindBox = GetValue("FindBox/PlaceholderText");
            ThemeTB = GetValue("ThemeTB/Text");
            LangTB = GetValue("LangTB/Text");
            FixedView = GetValue("FixedView/Content");
            Columns = GetValue("Columns/Text");
            FileSize = GetValue("FileSize/Text");
        }

        private static string GetValue(string key)
        {
            var value = _rm.MainResourceMap.TryGetValue($"Resources/{key}");
            value ??= _rm.MainResourceMap.GetValue($"{_resSpace}/Resources/{key}");
            return value?.ValueAsString;
        }

        private string _BrowseButton;
        public string BrowseButton
        {
            get => _BrowseButton;
            set { if (_BrowseButton != value) { _BrowseButton = value; OnPropertyChanged(); } }
        }

        private string _FindBox;
        public string FindBox
        {
            get => _FindBox;
            set { if (_FindBox != value) { _FindBox = value; OnPropertyChanged(); } }
        }

        private string _ThemeTB;
        public string ThemeTB
        {
            get => _ThemeTB;
            set { if (_ThemeTB != value) { _ThemeTB = value; OnPropertyChanged(); } }
        }

        private string _LangTB;
        public string LangTB
        {
            get => _LangTB;
            set { if (_LangTB != value) { _LangTB = value; OnPropertyChanged(); } }
        }

        private string _FixedView;
        public string FixedView
        {
            get => _FixedView;
            set { if (_FixedView != value) { _FixedView = value; OnPropertyChanged(); } }
        }

        private string _Columns;
        public string Columns
        {
            get => _Columns;
            set { if (_Columns != value) { _Columns = value; OnPropertyChanged(); } }
        }

        private string _FileSize;
        public string FileSize
        {
            get => _FileSize;
            set { if (_FileSize != value) { _FileSize = value; OnPropertyChanged(); } }
        }
    }
}

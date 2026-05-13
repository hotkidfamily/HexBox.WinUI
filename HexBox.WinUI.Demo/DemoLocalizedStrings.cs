using Microsoft.Windows.ApplicationModel.Resources;
using System.Collections.Generic;
using System.ComponentModel;

namespace HexBox.WinUI.Demo
{
    public sealed class DemoLocalizedStrings : INotifyPropertyChanged
    {
        private const string _resSpace = "HexBox.WinUI.Demo";
        private static readonly ResourceManager _rm = new();

        private static readonly string[] _keys =
        [
            nameof(BrowseButton), nameof(FindBox),
            nameof(ThemeTB), nameof(LangTB),
            nameof(FixedView), nameof(Columns), nameof(FileSize),
        ];

        private static readonly Dictionary<string, string> _keyMap = new()
        {
            [nameof(BrowseButton)] = "BrowseButton/Content",
            [nameof(FindBox)] = "FindBox/PlaceholderText",
            [nameof(ThemeTB)] = "ThemeTB/Text",
            [nameof(LangTB)] = "LangTB/Text",
            [nameof(FixedView)] = "FixedView/Content",
            [nameof(Columns)] = "Columns/Text",
            [nameof(FileSize)] = "FileSize/Text",
        };

        private readonly Dictionary<string, string> _values = [];

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public DemoLocalizedStrings() => Reload();

        public void Reload()
        {
            _values.Clear();
            foreach (var key in _keys)
                _values[key] = GetValue(_keyMap[key]);
            foreach (var key in _keys)
                OnPropertyChanged(key);
        }

        private static string GetValue(string key)
        {
            var value = _rm.MainResourceMap.TryGetValue($"Resources/{key}");
            value ??= _rm.MainResourceMap.GetValue($"{_resSpace}/Resources/{key}");
            return value?.ValueAsString;
        }

        public string BrowseButton => _values[nameof(BrowseButton)];
        public string FindBox => _values[nameof(FindBox)];
        public string ThemeTB => _values[nameof(ThemeTB)];
        public string LangTB => _values[nameof(LangTB)];
        public string FixedView => _values[nameof(FixedView)];
        public string Columns => _values[nameof(Columns)];
        public string FileSize => _values[nameof(FileSize)];
    }
}

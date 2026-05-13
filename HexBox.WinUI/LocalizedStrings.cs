using Microsoft.Windows.ApplicationModel.Resources;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HexBox.WinUI
{
    public sealed class LocalizedStrings : INotifyPropertyChanged
    {
        private const string _resSpace = "HexBox.WinUI";
        private static readonly ResourceManager _rm = new();

        private static readonly string[] _keys =
        [
            nameof(AddressProperties), nameof(AddressPropertiesNoAddress),
            nameof(Copy), nameof(CopyText), nameof(SelectAll),
            nameof(DataformatDecimal), nameof(DataFormatHex),
            nameof(DataProperties), nameof(DataPropertiesNoData),
            nameof(DataSigned), nameof(DataUnsigned),
            nameof(DataTypeEightByteFloat), nameof(DataTypeEightByteInteger),
            nameof(DataTypeFourByteFloat), nameof(DataTypeFourByteInteger),
            nameof(DataTypeTwoByteInteger), nameof(DataTypeOneByteInteger),
            nameof(EndianBig), nameof(EndianLittle),
            nameof(TextFormatAscii), nameof(TextProperties), nameof(TextPropertiesNoText),
        ];

        private readonly Dictionary<string, string> _values = [];

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public LocalizedStrings() => Reload();

        internal void Reload()
        {
            _values.Clear();
            foreach (var key in _keys)
                _values[key] = GetValue(key);
            foreach (var key in _keys)
                OnPropertyChanged(key);
        }

        private static string GetValue(string name)
        {
            var value = _rm.MainResourceMap.TryGetValue($"{_resSpace}/Resources/{name}/Text");
            value ??= _rm.MainResourceMap.GetValue($"{_resSpace}/{_resSpace}/Resources/{name}");
            return value?.ValueAsString;
        }

        public string AddressProperties => _values[nameof(AddressProperties)];
        public string AddressPropertiesNoAddress => _values[nameof(AddressPropertiesNoAddress)];
        public string Copy => _values[nameof(Copy)];
        public string CopyText => _values[nameof(CopyText)];
        public string SelectAll => _values[nameof(SelectAll)];
        public string DataformatDecimal => _values[nameof(DataformatDecimal)];
        public string DataFormatHex => _values[nameof(DataFormatHex)];
        public string DataProperties => _values[nameof(DataProperties)];
        public string DataPropertiesNoData => _values[nameof(DataPropertiesNoData)];
        public string DataSigned => _values[nameof(DataSigned)];
        public string DataUnsigned => _values[nameof(DataUnsigned)];
        public string DataTypeEightByteFloat => _values[nameof(DataTypeEightByteFloat)];
        public string DataTypeEightByteInteger => _values[nameof(DataTypeEightByteInteger)];
        public string DataTypeFourByteFloat => _values[nameof(DataTypeFourByteFloat)];
        public string DataTypeFourByteInteger => _values[nameof(DataTypeFourByteInteger)];
        public string DataTypeTwoByteInteger => _values[nameof(DataTypeTwoByteInteger)];
        public string DataTypeOneByteInteger => _values[nameof(DataTypeOneByteInteger)];
        public string EndianBig => _values[nameof(EndianBig)];
        public string EndianLittle => _values[nameof(EndianLittle)];
        public string TextFormatAscii => _values[nameof(TextFormatAscii)];
        public string TextProperties => _values[nameof(TextProperties)];
        public string TextPropertiesNoText => _values[nameof(TextPropertiesNoText)];
    }
}

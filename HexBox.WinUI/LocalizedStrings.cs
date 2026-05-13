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

        private static readonly Dictionary<string, string> _keyMap = new()
        {
            [nameof(AddressProperties)] = "AddressProperties/Text",
            [nameof(AddressPropertiesNoAddress)] = "AddressPropertiesNoAddress/Text",
            [nameof(Copy)] = "Copy/Text",
            [nameof(CopyText)] = "CopyText/Text",
            [nameof(SelectAll)] = "SelectAll/Text",
            [nameof(DataformatDecimal)] = "DataformatDecimal/Text",
            [nameof(DataFormatHex)] = "DataFormatHex/Text",
            [nameof(DataProperties)] = "DataProperties/Text",
            [nameof(DataPropertiesNoData)] = "DataPropertiesNoData/Text",
            [nameof(DataSigned)] = "DataSigned/Text",
            [nameof(DataUnsigned)] = "DataUnsigned/Text",
            [nameof(DataTypeEightByteFloat)] = "DataTypeEightByteFloat/Text",
            [nameof(DataTypeEightByteInteger)] = "DataTypeEightByteInteger/Text",
            [nameof(DataTypeFourByteFloat)] = "DataTypeFourByteFloat/Text",
            [nameof(DataTypeFourByteInteger)] = "DataTypeFourByteInteger/Text",
            [nameof(DataTypeTwoByteInteger)] = "DataTypeTwoByteInteger/Text",
            [nameof(DataTypeOneByteInteger)] = "DataTypeOneByteInteger/Text",
            [nameof(EndianBig)] = "EndianBig/Text",
            [nameof(EndianLittle)] = "EndianLittle/Text",
            [nameof(TextFormatAscii)] = "TextFormatAscii/Text",
            [nameof(TextProperties)] = "TextProperties/Text",
            [nameof(TextPropertiesNoText)] = "TextPropertiesNoText/Text",
        };

        private readonly Dictionary<string, string> _values = [];

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public LocalizedStrings() => Reload();

        internal void Reload()
        {
            _values.Clear();
            foreach (var (key, resKey) in _keyMap)
                _values[key] = GetValue(resKey);
            foreach (var key in _keyMap.Keys)
                OnPropertyChanged(key);
        }

        private static string GetValue(string resKey)
        {
            var value = _rm.MainResourceMap.TryGetValue($"{_resSpace}/Resources/{resKey}");
            value ??= _rm.MainResourceMap.GetValue($"{_resSpace}/{_resSpace}/Resources/{resKey}");
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

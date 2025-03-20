using Microsoft.Windows.ApplicationModel.Resources;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HexBox.WinUI
{
    public partial class LocalizedStrings: INotifyPropertyChanged
    {
        private const string _resSpace = "HexBox.WinUI";
        private static readonly ResourceManager _rm = new();

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public LocalizedStrings()
        {
            AddressProperties = GetValue(nameof(AddressProperties));
            Copy = GetValue(nameof(Copy));
            CopyText = GetValue(nameof(CopyText));
            SelectAll = GetValue(nameof(SelectAll));
            AddressPropertiesNoAddress = GetValue(nameof(AddressPropertiesNoAddress));
            DataformatDecimal = GetValue(nameof(DataformatDecimal));
            DataFormatHex = GetValue(nameof(DataFormatHex));
            DataProperties = GetValue(nameof(DataProperties));
            DataPropertiesNoData = GetValue(nameof(DataPropertiesNoData));
            DataSigned = GetValue(nameof(DataSigned));
            DataTypeEightByteFloat = GetValue(nameof(DataTypeEightByteFloat));
            DataTypeEightByteInteger = GetValue(nameof(DataTypeEightByteInteger));
            DataTypeFourByteInteger = GetValue(nameof(DataTypeFourByteInteger));
            DataTypeTwoByteInteger = GetValue(nameof(DataTypeTwoByteInteger));
            DataTypeOneByteInteger = GetValue(nameof(DataTypeOneByteInteger));
            DataTypeFourByteFloat = GetValue(nameof(DataTypeFourByteFloat));
            DataUnsigned = GetValue(nameof(DataUnsigned));
            EndianBig = GetValue(nameof(EndianBig));
            EndianLittle = GetValue(nameof(EndianLittle));
            TextFormatAscii = GetValue(nameof(TextFormatAscii));
            TextProperties = GetValue(nameof(TextProperties));
            TextPropertiesNoText = GetValue(nameof(TextPropertiesNoText));
        }

        private static string GetValue(string name)
        {
            var value = _rm.MainResourceMap.TryGetValue($"{_resSpace}/Resources/{name}/Text");
            value ??= _rm.MainResourceMap.GetValue($"{_resSpace}/{_resSpace}/Resources/{name}");

            return value?.ValueAsString;
        }

        private string _AddressProperties;
        private string _AddressPropertiesNoAddress;
        private string _Copy;
        private string _CopyText;
        private string _DataformatDecimal;
        private string _DataFormatHex;
        private string _DataProperties;
        private string _DataPropertiesNoData;
        private string _DataSigned;
        private string _DataTypeEightByteFloat;
        private string _DataTypeEightByteInteger;
        private string _DataTypeFourByteFloat;
        private string _DataTypeFourByteInteger;
        private string _DataTypeTwoByteInteger;
        private string _DataTypeOneByteInteger;
        private string _DataUnsigned;
        private string _EndianBig;
        private string _EndianLittle;
        private string _SelectAll;
        private string _TextFormatAscii;
        private string _TextProperties;
        private string _TextPropertiesNoText;

        public string AddressProperties
        {
            get => _AddressProperties;
            set
            {
                if (_AddressProperties != value)
                {
                    _AddressProperties = value;
                    OnPropertyChanged();
                }
            }
        }
        public string AddressPropertiesNoAddress
        {
            get => _AddressPropertiesNoAddress;
            set
            {
                if (_AddressPropertiesNoAddress != value)
                {
                    _AddressPropertiesNoAddress = value;
                    OnPropertyChanged();
                }
            }
        }
        public string Copy
        {
            get => _Copy;
            set
            {
                if (_Copy != value)
                {
                    _Copy = value;
                    OnPropertyChanged();
                }
            }
        }
        public string CopyText
        {
            get => _CopyText;
            set
            {
                if (_CopyText != value)
                {
                    _CopyText = value;
                    OnPropertyChanged();
                }
            }
        }
        public string DataformatDecimal
        {
            get => _DataformatDecimal;
            set
            {
                if (_DataformatDecimal != value)
                {
                    _DataformatDecimal = value;
                    OnPropertyChanged();
                }
            }
        }
        public string DataFormatHex
        {
            get => _DataFormatHex;
            set
            {
                if (_DataFormatHex != value)
                {
                    _DataFormatHex = value;
                    OnPropertyChanged();
                }
            }
        }
        public string DataProperties
        {
            get => _DataProperties;
            set
            {
                if (_DataProperties != value)
                {
                    _DataProperties = value;
                    OnPropertyChanged();
                }
            }
        }
        public string DataPropertiesNoData
        {
            get => _DataPropertiesNoData;
            set
            {
                if (_DataPropertiesNoData != value)
                {
                    _DataPropertiesNoData = value;
                    OnPropertyChanged();
                }
            }
        }
        public string DataSigned
        {
            get => _DataSigned;
            set
            {
                if (_DataSigned != value)
                {
                    _DataSigned = value;
                    OnPropertyChanged();
                }
            }
        }
        public string DataTypeEightByteFloat
        {
            get => _DataTypeEightByteFloat;
            set
            {
                if (_DataTypeEightByteFloat != value)
                {
                    _DataTypeEightByteFloat = value;
                    OnPropertyChanged();
                }
            }
        }
        public string DataTypeEightByteInteger
        {
            get => _DataTypeEightByteInteger;
            set
            {
                if (_DataTypeEightByteInteger != value)
                {
                    _DataTypeEightByteInteger = value;
                    OnPropertyChanged();
                }
            }
        }
        public string DataTypeFourByteFloat
        {
            get => _DataTypeFourByteFloat;
            set
            {
                if (_DataTypeFourByteFloat != value)
                {
                    _DataTypeFourByteFloat = value;
                    OnPropertyChanged();
                }
            }
        }
        public string DataTypeFourByteInteger
        {
            get => _DataTypeFourByteInteger;
            set
            {
                if (_DataTypeFourByteInteger != value)
                {
                    _DataTypeFourByteInteger = value;
                    OnPropertyChanged();
                }
            }
        }
        public string DataTypeTwoByteInteger
        {
            get => _DataTypeTwoByteInteger;
            set
            {
                if (_DataTypeTwoByteInteger != value)
                {
                    _DataTypeTwoByteInteger = value;
                    OnPropertyChanged();
                }
            }
        }
        public string DataTypeOneByteInteger
        {
            get => _DataTypeOneByteInteger;
            set
            {
                if (_DataTypeOneByteInteger != value)
                {
                    _DataTypeOneByteInteger = value;
                    OnPropertyChanged();
                }
            }
        }
        public string DataUnsigned
        {
            get => _DataUnsigned;
            set
            {
                if (_DataUnsigned != value)
                {
                    _DataUnsigned = value;
                    OnPropertyChanged();
                }
            }
        }
        public string EndianBig
        {
            get => _EndianBig;
            set
            {
                if (_EndianBig != value)
                {
                    _EndianBig = value;
                    OnPropertyChanged();
                }
            }
        }
        public string EndianLittle
        {
            get => _EndianLittle;
            set
            {
                if (_EndianLittle != value)
                {
                    _EndianLittle = value;
                    OnPropertyChanged();
                }
            }
        }
        public string SelectAll
        {
            get => _SelectAll;
            set
            {
                if (_SelectAll != value)
                {
                    _SelectAll = value;
                    OnPropertyChanged();
                }
            }
        }
        public string TextFormatAscii
        {
            get => _TextFormatAscii;
            set
            {
                if (_TextFormatAscii != value)
                {
                    _TextFormatAscii = value;
                    OnPropertyChanged();
                }
            }
        }
        public string TextProperties
        {
            get => _TextProperties;
            set
            {
                if (_TextProperties != value)
                {
                    _TextProperties = value;
                    OnPropertyChanged();
                }
            }
        }
        public string TextPropertiesNoText
        {
            get => _TextPropertiesNoText;
            set
            {
                if (_TextPropertiesNoText != value)
                {
                    _TextPropertiesNoText = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}

using kissskia.Library.EndianConvert;
using Microsoft.UI;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using SkiaSharp;
using SkiaSharp.Views.Windows;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Core;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace kissskia
{
    [TemplatePart(Name = nameof(PartCanvas), Type = typeof(SKXamlCanvas))]
    [TemplatePart(Name = nameof(PartVerticalScrollBar), Type = typeof(ScrollBar))]
    public sealed class HexBox : Control, INotifyPropertyChanged
    {
        /// <summary>
        /// Defines the address at which the data in the <see cref="DataSourceProperty"/> begins.
        /// </summary>
        public static readonly DependencyProperty AddressProperty =
            DependencyProperty.Register(nameof(Address), typeof(ulong), typeof(HexBox),
                new PropertyMetadata(0UL, OnAddressChanged));

        /// <summary>
        /// Defines the brush used to display the addresses in the address section of the control.
        /// </summary>
        public static readonly DependencyProperty AddressBrushProperty =
            DependencyProperty.Register(nameof(AddressBrush), typeof(Brush), typeof(HexBox),
                new PropertyMetadata(new SolidColorBrush(Colors.CornflowerBlue), OnPropertyChangedInvalidateMeasure));

        /// <summary>
        /// Defines the width of the addresses displayed in the address section of the control.
        /// </summary>
        public static readonly DependencyProperty AddressFormatProperty =
            DependencyProperty.Register(nameof(AddressFormat), typeof(AddressFormat), typeof(HexBox),
                new PropertyMetadata(AddressFormat.Address32, OnPropertyChangedInvalidateMeasure));

        /// <summary>
        ///  Defines the brush used for alternating for text in alternating (odd numbered) columns in the data section of the control.
        /// </summary>
        public static readonly DependencyProperty AlternatingDataColumnTextBrushProperty =
            DependencyProperty.Register(nameof(AlternatingDataColumnTextBrush), typeof(Brush), typeof(HexBox),
                new PropertyMetadata(new SolidColorBrush(Colors.Gray), OnPropertyChangedInvalidateMeasure));

        /// <summary>
        /// Defines the number of columns to display.
        /// </summary>
        public static readonly DependencyProperty ColumnsProperty =
            DependencyProperty.Register(nameof(Columns), typeof(int), typeof(HexBox),
                new PropertyMetadata(16, OnPropertyChangedInvalidateMeasure));

        /// <summary>
        /// Defines the endianness used to interpret the data.
        /// </summary>
        public static readonly DependencyProperty EndiannessProperty =
            DependencyProperty.Register(nameof(Endianness), typeof(Endianness), typeof(HexBox),
                new PropertyMetadata(Endianness.BigEndian, OnPropertyChangedInvalidateMeasure));

        /// <summary>
        /// Defines the format of the data to display.
        /// </summary>
        public static readonly DependencyProperty DataFormatProperty =
            DependencyProperty.Register(nameof(DataFormat), typeof(DataFormat), typeof(HexBox),
                new PropertyMetadata(DataFormat.Hexadecimal, OnPropertyChangedInvalidateMeasure));

        /// <summary>
        /// Defines the signedness of the data to display.
        /// </summary>
        public static readonly DependencyProperty DataSignednessProperty =
            DependencyProperty.Register(nameof(DataSignedness), typeof(DataSignedness), typeof(HexBox),
                new PropertyMetadata(DataSignedness.Unsigned, OnPropertyChangedInvalidateMeasure));

        /// <summary>
        /// Defines the data source which is used to read the data to display within this control.
        /// </summary>
        public static readonly DependencyProperty DataSourceProperty =
            DependencyProperty.Register(nameof(DataSource), typeof(BinaryReader), typeof(HexBox),
                new PropertyMetadata(null, OnDataSourceChanged));

        /// <summary>
        /// Defines the type of the data to display.
        /// </summary>
        public static readonly DependencyProperty DataTypeProperty =
            DependencyProperty.Register(nameof(DataType), typeof(DataType), typeof(HexBox),
                new PropertyMetadata(DataType.Integer, OnDataTypeChanged));

        /// <summary>
        /// Defines the width of the data to display.
        /// </summary>
        public static readonly DependencyProperty DataWidthProperty =
            DependencyProperty.Register(nameof(DataWidth), typeof(int), typeof(HexBox),
                new PropertyMetadata(1, OnDataWidthChanged));

        /// <summary>
        /// Defines the offset from the <see cref="DataSourceProperty"/> of the first visible data element being displayed.
        /// </summary>
        public static readonly DependencyProperty OffsetProperty =
            DependencyProperty.Register(nameof(Offset), typeof(long), typeof(HexBox),
                new PropertyMetadata(0L, OnPropertyChangedInvalidateMeasure));

        /// <summary>
        /// Defines the maximum number of columns, based on the size of the control, which can be displayed.
        /// </summary>
        public static readonly DependencyProperty MaxVisibleColumnsProperty =
            DependencyProperty.Register(nameof(MaxVisibleColumns), typeof(int), typeof(HexBox),
                new PropertyMetadata(int.MaxValue, OnPropertyChangedInvalidateMeasure));


        /// <summary>
        /// Defines the maximum number of rows, based on the size of the control, which can be displayed.
        /// </summary>
        public static readonly DependencyProperty MaxVisibleRowsProperty =
            DependencyProperty.Register(nameof(MaxVisibleRows), typeof(int), typeof(HexBox),
                new PropertyMetadata(int.MaxValue, OnPropertyChangedInvalidateMeasure));

        /// <summary>
        /// Defines the brush used for selection fill.
        /// </summary>
        public static readonly DependencyProperty SelectionBrushProperty =
            DependencyProperty.Register(nameof(SelectionBrush), typeof(Brush), typeof(HexBox),
                new PropertyMetadata(new SolidColorBrush(Colors.LightPink), OnPropertyChangedInvalidateMeasure));

        /// <summary>
        /// Defines the brush used for selected text.
        /// </summary>
        public static readonly DependencyProperty SelectionTextBrushProperty =
            DependencyProperty.Register(nameof(SelectionTextBrush), typeof(Brush), typeof(HexBox),
                new PropertyMetadata(new SolidColorBrush(Colors.Black), OnPropertyChangedInvalidateMeasure));

        /// <summary>
        /// Defines the offset from <see cref="DataSourceProperty"/> of where the user selection has ended.
        /// </summary>
        public static readonly DependencyProperty SelectionEndProperty =
            DependencyProperty.Register(nameof(SelectionEnd), typeof(long), typeof(HexBox),
                new PropertyMetadata(0L, OnSelectionEndChanged));

        /// <summary>
        /// Defines the offset from <see cref="DataSourceProperty"/> of where the user selection has started.
        /// </summary>
        public static readonly DependencyProperty SelectionStartProperty =
            DependencyProperty.Register(nameof(SelectionStart), typeof(long), typeof(HexBox),
                new PropertyMetadata(0L, OnSelectionStartChanged));

        /// <summary>
        /// Determines whether to show the address section of the control.
        /// </summary>
        public static readonly DependencyProperty ShowAddressProperty =
            DependencyProperty.Register(nameof(ShowAddress), typeof(bool), typeof(HexBox),
                new PropertyMetadata(true, OnPropertyChangedInvalidateMeasure));

        /// <summary>
        /// Determines whether to show the data section of the control.
        /// </summary>
        public static readonly DependencyProperty ShowDataProperty =
            DependencyProperty.Register(nameof(ShowData), typeof(bool), typeof(HexBox),
                new PropertyMetadata(true, OnPropertyChangedInvalidateMeasure));

        /// <summary>
        /// Determines whether to show the text section of the control.
        /// </summary>
        public static readonly DependencyProperty ShowTextProperty =
            DependencyProperty.Register(nameof(ShowText), typeof(bool), typeof(HexBox),
                new PropertyMetadata(true, OnPropertyChangedInvalidateMeasure));

        /// <summary>
        /// Defines the format of the text to display in the text section.
        /// </summary>
        public static readonly DependencyProperty TextFormatProperty =
            DependencyProperty.Register(nameof(TextFormat), typeof(TextFormat), typeof(HexBox),
                new PropertyMetadata(TextFormat.Ascii, OnPropertyChangedInvalidateMeasure));

        private const int MaxColumns = 128;
        private const int MaxRows = 128;

        private const int CharsBetweenSections = 2;
        private const int CharsBetweenDataColumns = 1;
        private const int ScrollWheelScrollRows = 3;

        private SKRect _TextMeasure;
        private SKTypeface _TextTypeFace = SKTypeface.FromFamilyName("Monaco", SKFontStyle.Normal);
        private float _LineSize = 1.0f;

        private SKXamlCanvas PartCanvas;

        private SelectionArea highlightBegin = SelectionArea.None;
        private SelectionArea highlightState = SelectionArea.None;

        private double lastVerticalScrollValue = 0;

        private ScrollBar PartVerticalScrollBar;

        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;

        private enum SelectionArea
        {
            None,
            Address,
            Data,
            Text,
        }

        /// <summary>
        /// Gets the <see cref="ApplicationCommands.Copy"/> routed command.
        /// </summary>
        public ICommand CopyCommand { get; set; }

        /// <summary>
        /// Gets or sets the address at which the data in the <see cref="DataSource"/> begins.
        /// </summary>
        public ulong Address
        {
            get => (ulong)GetValue(AddressProperty);

            set => SetValue(AddressProperty, value);
        }

        /// <summary>
        /// Gets or sets the brush used to display the addresses in the address section of the control.
        /// </summary>
        public Brush AddressBrush
        {
            get => (Brush)GetValue(AddressBrushProperty);

            set => SetValue(AddressBrushProperty, value);
        }

        /// <summary>
        /// Gets or sets the brush used for alternating for text in alternating (odd numbered) columns in the data section of the control.
        /// </summary>
        public Brush AlternatingDataColumnTextBrush
        {
            get => (Brush)GetValue(AlternatingDataColumnTextBrushProperty);

            set => SetValue(AlternatingDataColumnTextBrushProperty, value);
        }

        /// <summary>
        /// Gets or sets the number of columns to display.
        /// </summary>
        public int Columns
        {
            get => (int)GetValue(ColumnsProperty);

            set => SetValue(ColumnsProperty, CoerceColumns(this, value));
        }

        /// <summary>
        /// Gets or sets the endianness used to interpret the data.
        /// </summary>
        public Endianness Endianness
        {
            get => (Endianness)GetValue(EndiannessProperty);

            set => SetValue(EndiannessProperty, value);
        }

        /// <summary>
        /// Gets or sets the format of the data to display.
        /// </summary>
        public DataFormat DataFormat
        {
            get => (DataFormat)GetValue(DataFormatProperty);

            set => SetValue(DataFormatProperty, value);
        }

        /// <summary>
        /// Gets or sets the signedness of the data to display.
        /// </summary>
        public DataSignedness DataSignedness
        {
            get => (DataSignedness)GetValue(DataSignednessProperty);

            set => SetValue(DataSignednessProperty, value);
        }

        /// <summary>
        /// Gets or sets the data source which is used to read the data to display within this control.
        /// </summary>
        public BinaryReader DataSource
        {
            get => (BinaryReader)GetValue(DataSourceProperty);

            set => SetValue(DataSourceProperty, value);
        }

        /// <summary>
        /// Gets or sets the type of the data to display.
        /// </summary>
        public DataType DataType
        {
            get => (DataType)GetValue(DataTypeProperty);

            set => SetValue(DataTypeProperty, value);
        }

        /// <summary>
        /// Gets or sets the width of the data to display.
        /// </summary>
        public int DataWidth
        {
            get => (int)GetValue(DataWidthProperty);

            set
            {
                if (ValidateDataWidth(value))
                    SetValue(DataWidthProperty, CoerceDataWidth(this, value));
                else
                    throw new ArgumentOutOfRangeException(nameof(value), "Value must be 1,2,4,8.");
            }
        }

        /// <summary>
        /// Gets a value indicating whether the user has made any selection within the control.
        /// </summary>
        public bool IsSelectionActive => SelectionLength != 0;

        /// <summary>
        /// Gets the maximum number of columns, based on the size of the control, which can be displayed.
        /// </summary>
        public int MaxVisibleColumns
        {
            get => (int)GetValue(MaxVisibleColumnsProperty);

            private set => SetValue(MaxVisibleColumnsProperty, CoerceMaxVisibleColumns(this, value));
        }

        /// <summary>
        /// Gets the maximum number of rows, based on the size of the control, which can be displayed.
        /// </summary>
        public int MaxVisibleRows
        {
            get => (int)GetValue(MaxVisibleRowsProperty);

            private set => SetValue(MaxVisibleRowsProperty, CoerceMaxVisibleRows(this, value));
        }

        /// <summary>
        /// Gets or sets the offset from the <see cref="DataSource"/> of the first visible data element being displayed.
        /// </summary>
        public long Offset
        {
            get => (long)GetValue(OffsetProperty);

            set => SetValue(OffsetProperty, CoerceOffset(this, value));
        }

        /// <summary>
        /// Gets lowest order address currently being selected.
        /// </summary>
        public ulong SelectedAddress => Address + (ulong)SelectedOffset;

        /// <summary>
        /// Gets the offset from <see cref="DataSource"/> of the <see cref="SelectedAddress"/>.
        /// </summary>
        public long SelectedOffset => Math.Min(SelectionStart, SelectionEnd);

        /// <summary>
        /// Gets or sets the brush used for selection fill.
        /// </summary>
        public Brush SelectionBrush
        {
            get => (Brush)GetValue(SelectionBrushProperty);

            set => SetValue(SelectionBrushProperty, value);
        }

        /// <summary>
        /// Gets the offset from <see cref="DataSource"/> of where the user selection has ended.
        /// </summary>
        public long SelectionEnd
        {
            get => (long)GetValue(SelectionEndProperty);

            private set => SetValue(SelectionEndProperty, CoerceSelectionEnd(this, value));
        }

        /// <summary>
        /// Gets the number of bytes selected.
        /// </summary>
        public long SelectionLength
        {
            get
            {
                if (SelectionStart <= SelectionEnd)
                {
                    return SelectionEnd - SelectionStart;
                }
                else
                {
                    return SelectionStart - SelectionEnd + BytesPerColumn;
                }
            }
        }

        /// <summary>
        /// Gets the offset from <see cref="DataSource"/> of where the user selection has started.
        /// </summary>
        public long SelectionStart
        {
            get => (long)GetValue(SelectionStartProperty);

            private set => SetValue(SelectionStartProperty, CoerceSelectionStart(this, value));
        }

        /// <summary>
        /// Gets or sets the brush used for selected text.
        /// </summary>
        public Brush SelectionTextBrush
        {
            get => (Brush)GetValue(SelectionTextBrushProperty);

            set => SetValue(SelectionTextBrushProperty, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show the address section of the control.
        /// </summary>
        public bool ShowAddress
        {
            get => (bool)GetValue(ShowAddressProperty);

            set => SetValue(ShowAddressProperty, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show the data section of the control.
        /// </summary>
        public bool ShowData
        {
            get => (bool)GetValue(ShowDataProperty);

            set => SetValue(ShowDataProperty, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show the text section of the control.
        /// </summary>
        public bool ShowText
        {
            get => (bool)GetValue(ShowTextProperty);

            set => SetValue(ShowTextProperty, value);
        }

        /// <summary>
        /// Gets or sets the width of the addresses displayed in the address section of the control.
        /// </summary>
        public AddressFormat AddressFormat
        {
            get => (AddressFormat)GetValue(AddressFormatProperty);

            set => SetValue(AddressFormatProperty, value);
        }

        /// <summary>
        /// Gets or sets the format of the text to display in the text section.
        /// </summary>
        public TextFormat TextFormat
        {
            get => (TextFormat)GetValue(TextFormatProperty);

            set => SetValue(TextFormatProperty, value);
        }

        private double SelectionBoxDataXPadding => _TextMeasure.Width / 4;

        private double SelectionBoxDataYPadding => 0;

        private double SelectionBoxTextXPadding => 0;

        private double SelectionBoxTextYPadding => 0;

        private int BytesPerColumn => DataWidth;

        private int BytesPerRow => DataWidth * Columns;

        private const int WidthIncludingTrailingWhitespace = 0;

        private int count = 0;

        /// <summary>
        /// Copies the current selection of the control to the <see cref="Clipboard"/>.
        /// </summary>
        public void Copy()
        {
            if (IsSelectionActive)
            {
                StringBuilder builder = new();

                long savedDataSourcePositionBeforeReadingData = DataSource.BaseStream.Position;

                DataSource.BaseStream.Position = Math.Min(SelectionStart, SelectionEnd);

                while (DataSource.BaseStream.Position < Math.Max(SelectionStart, SelectionEnd))
                {
                    var formattedData = ReadFormattedData();

                    builder.Append(formattedData);
                }

                DataSource.BaseStream.Position = savedDataSourcePositionBeforeReadingData;

                var dataPackage = new DataPackage();
                dataPackage.SetText(builder.ToString());
                Clipboard.SetContent(dataPackage);
            }
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            PartCanvas = GetTemplateChild(nameof(PartCanvas)) as SKXamlCanvas;

            if (PartCanvas != null)
            {
                CopyCommand = new RelayCommand(CopyExecuted, CopyCanExecute);
                PartCanvas.PaintSurface += PartCanvas_PaintSurface;
            }
            else
            {
                throw new InvalidOperationException($"Could not find {nameof(PartCanvas)} template child.");
            }

            if (PartVerticalScrollBar != null)
            {
                PartVerticalScrollBar.Scroll -= OnVerticalScrollBarScroll;
            }

            PartVerticalScrollBar = GetTemplateChild(nameof(PartVerticalScrollBar)) as ScrollBar;

            if (PartVerticalScrollBar != null)
            {
                PartVerticalScrollBar.Scroll += OnVerticalScrollBarScroll;
                PartVerticalScrollBar.ValueChanged += OnVerticalScrollBarValueChanged;

                PartVerticalScrollBar.Minimum = 0;
                PartVerticalScrollBar.SmallChange = 1;
                PartVerticalScrollBar.LargeChange = MaxVisibleRows;
            }
            else
            {
                throw new InvalidOperationException($"Could not find {nameof(PartVerticalScrollBar)} template child.");
            }
        }


        private void DrawSelectionGeometry(SKCanvas Canvas,
                                            Brush brush,
                                            SKPaint pen,
                                            Point point0,
                                            Point point1,
                                            SelectionArea relativeTo)
        {
            if ((long)point0.Y > (long)point1.Y)
            {
                throw new ArgumentException($"{nameof(point0)}.Y > {nameof(point1)}.Y", nameof(point0));
            }

            Point lhsVerticalLinePoint0;
            Point rhsVerticalLinePoint0;

            double selectionBoxXPadding;
            double selectionBoxYPadding;

            switch (relativeTo)
            {
                case SelectionArea.Data:
                    {
                        lhsVerticalLinePoint0 = CalculateAddressVerticalLinePoint0();
                        rhsVerticalLinePoint0 = CalculateDataVerticalLinePoint0();

                        selectionBoxXPadding = SelectionBoxDataXPadding;
                        selectionBoxYPadding = SelectionBoxDataYPadding;
                    }

                    break;

                case SelectionArea.Text:
                    {
                        lhsVerticalLinePoint0 = CalculateDataVerticalLinePoint0();
                        rhsVerticalLinePoint0 = CalculateTextVerticalLinePoint0();

                        selectionBoxXPadding = SelectionBoxTextXPadding;
                        selectionBoxYPadding = SelectionBoxTextYPadding;
                    }

                    break;

                default:
                    {
                        throw new ArgumentException($"Invalid relative area {relativeTo}", nameof(relativeTo));
                    }
            }

            point0.X -=  selectionBoxXPadding;
            point1.X +=  selectionBoxXPadding;
            point0.Y -=  selectionBoxYPadding;
            point1.Y +=  selectionBoxYPadding;

            SKPath path = new();
            SKPoint[] points;

            if ((long)point0.X < (long)point1.X)
            {
                if ((long)point0.Y < (long)point1.Y)
                {
                    // +---------------------------+
                    // |                           |
                    // |             0-------------2
                    // |             |             |
                    // 6-------------7     1-------3
                    // |                   |       |
                    // 5-------------------4       |
                    // |                           |
                    // |                           |
                    // |                           |
                    // +---------------------------+
                    SKPoint point2 = new((float)(rhsVerticalLinePoint0.X - CharsBetweenSections * _TextMeasure.Width + selectionBoxXPadding), (float)point0.Y);
                    SKPoint point3 = new((float)(rhsVerticalLinePoint0.X - CharsBetweenSections * _TextMeasure.Width + selectionBoxXPadding), (float)point1.Y);
                    SKPoint point4 = new((float)point1.X, (float)(point1.Y + _TextMeasure.Height));
                    SKPoint point5 = new((float)(lhsVerticalLinePoint0.X + CharsBetweenSections * _TextMeasure.Width - selectionBoxXPadding), (float)(point1.Y + _TextMeasure.Height));
                    SKPoint point6 = new((float)(lhsVerticalLinePoint0.X + CharsBetweenSections * _TextMeasure.Width - selectionBoxXPadding), (float)(point0.Y + _TextMeasure.Height));
                    SKPoint point7 = new((float)point0.X, (float)(point0.Y + _TextMeasure.Height));

                    points = [point0.ToSKPoint(), point2, point3, point1.ToSKPoint(), point4, point5, point6, point7];
                }
                else
                {
                    // +---------------------------+
                    // |                           |
                    // |     0-------------1       |
                    // |     |             |       |
                    // |     3-------------2       |
                    // |                           |
                    // |                           |
                    // |                           |
                    // |                           |
                    // |                           |
                    // +---------------------------+
                    Point point2 = new(point1.X, (float)(point1.Y + _TextMeasure.Height));
                    Point point3 = new(point0.X, (float)(point0.Y + _TextMeasure.Height));

                    points = [point0.ToSKPoint(), point1.ToSKPoint(), point2.ToSKPoint(), point3.ToSKPoint()];
                }
            }
            else
            {
                if ((long)(point0.Y + _TextMeasure.Height) == (long)point1.Y)
                {
                    // +---------------------------+
                    // |                           |
                    // |             0-------------2
                    // |             |             |
                    // 7--------1    4-------------3
                    // |        |                  |
                    // 6--------5                  |
                    // |                           |
                    // |                           |
                    // |                           |
                    // +---------------------------+
                    {
                        Point point2 = new(rhsVerticalLinePoint0.X - CharsBetweenSections * _TextMeasure.Width + selectionBoxXPadding, point0.Y);
                        Point point3 = new(rhsVerticalLinePoint0.X - CharsBetweenSections * _TextMeasure.Width + selectionBoxXPadding, point1.Y);
                        Point point4 = new(point0.X, point1.Y);

                        points = [point0.ToSKPoint(), point2.ToSKPoint(), point3.ToSKPoint(), point4.ToSKPoint()];
                    }

                    {
                        Point point5 = new(point1.X, point1.Y + _TextMeasure.Height);
                        Point point6 = new(lhsVerticalLinePoint0.X + CharsBetweenSections * _TextMeasure.Width - selectionBoxXPadding, point1.Y + _TextMeasure.Height);
                        Point point7 = new(lhsVerticalLinePoint0.X + CharsBetweenSections * _TextMeasure.Width - selectionBoxXPadding, point1.Y);
                        points = [point1.ToSKPoint(), point5.ToSKPoint(), point6.ToSKPoint(), point7.ToSKPoint()];
                    }
                }
                else
                {
                    // +---------------------------+
                    // |                           |
                    // |             0-------------2
                    // |             |             |
                    // 6-------------7             |
                    // |                           |
                    // |        1------------------3
                    // |        |                  |
                    // 5--------4                  |
                    // |                           |
                    // +---------------------------+
                    Point point2 = new(rhsVerticalLinePoint0.X - CharsBetweenSections * _TextMeasure.Width + selectionBoxXPadding, point0.Y);
                    Point point3 = new(rhsVerticalLinePoint0.X - CharsBetweenSections * _TextMeasure.Width + selectionBoxXPadding, point1.Y);
                    Point point4 = new(point1.X, (float)(point1.Y + _TextMeasure.Height));
                    Point point5 = new(lhsVerticalLinePoint0.X + CharsBetweenSections * _TextMeasure.Width - selectionBoxXPadding, (float)(point1.Y + _TextMeasure.Height));
                    Point point6 = new(lhsVerticalLinePoint0.X + CharsBetweenSections * _TextMeasure.Width - selectionBoxXPadding, point0.Y + _TextMeasure.Height);
                    Point point7 = new(point0.X, point0.Y + _TextMeasure.Height);

                    points = [point0.ToSKPoint(), point2.ToSKPoint(), point3.ToSKPoint(), point1.ToSKPoint(), point4.ToSKPoint(), point5.ToSKPoint(), point6.ToSKPoint(), point7.ToSKPoint()];
                }
            }

            path.AddPoly(points);
            if (brush is SolidColorBrush s)
                pen.Color = s.Color.ToSKColor();
            Canvas.DrawPath(path, pen);
        }

        private void PartCanvas_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            var view = sender as SKXamlCanvas;
            var canvas = e.Surface.Canvas;

            UpdateState();

            if (false)
            {
                var pint = new SKPaint()
                {
                    Color=Colors.Red.ToSKColor(),
                    StrokeWidth = 2,
                    Style = SKPaintStyle.Stroke,
                };
                SKRect rect = new(0, 0, (float)view.ActualWidth, (float)view.ActualHeight);
                //canvas.DrawRect(rect, pint);

                //SKPoint p0 = new(0, 0), p1 = new (rect.Width, 0), p2 = new (rect.Width, rect.Height), p3 = new(0, rect.Height);
                //canvas.DrawLine(p0, p1, pint);
                //canvas.DrawLine(p1, p2, pint);
                //canvas.DrawLine(p2, p3, pint);
                //canvas.DrawLine(p3, p0, pint);

                SKPath path = new();
                path.AddRect(rect);
                canvas.DrawPath(path, pint);
            }

            if (DataSource != null)
            {
                canvas.Clear();
                long savedDataSourcePosition = DataSource.BaseStream.Position;

                DataSource.BaseStream.Position = Offset;

                using SKPaint SplitLinePaint = new()
                {
                    Color = SKColors.Black,
                    IsStroke = true,
                    IsAntialias = true,
                    StrokeWidth = 1,
                    TextSize = (float)FontSize,
                    Typeface = _TextTypeFace,
                    TextScaleX = 1f,
                    TextAlign = SKTextAlign.Center,
                    IsDither = true
                };

                using SKPaint DataPaint = new()
                {
                    TextSize = (float)FontSize,
                    Typeface = _TextTypeFace,
                    TextScaleX = 1f,
                    IsAntialias = true,
                    IsDither = true,
                    TextAlign = SKTextAlign.Left,
                };

                if (ShowAddress)
                {
                    var addressVerticalLinePoint0 = CalculateAddressVerticalLinePoint0();
                    var addressVerticalLinePoint1 = CalculateAddressVerticalLinePoint1();

                    var p0 = addressVerticalLinePoint0.ToSKPoint();
                    var p1 = addressVerticalLinePoint1.ToSKPoint();

                    canvas.DrawLine(p0, p1, SplitLinePaint);
                }

                if (ShowData)
                {
                    var dataVerticalLinePoint0 = CalculateDataVerticalLinePoint0();
                    var dataVerticalLinePoint1 = CalculateDataVerticalLinePoint1();

                    var p0 = dataVerticalLinePoint0.ToSKPoint();
                    var p1 = dataVerticalLinePoint1.ToSKPoint();
                    canvas.DrawLine(p0, p1, SplitLinePaint);

                    if (SelectionLength != 0 && MaxVisibleRows > 0 && Columns > 0)
                    {
                        Point selectionPoint0 = ConvertOffsetToPosition(SelectedOffset, SelectionArea.Data);
                        Point selectionPoint1 = ConvertOffsetToPosition(SelectedOffset + SelectionLength, SelectionArea.Data);

                        if ((SelectedOffset + SelectionLength - Offset) / BytesPerColumn % Columns == 0)
                        {
                            // We're selecting the last column so the end point is the data vertical line (effectively)
                            selectionPoint1.X = dataVerticalLinePoint0.X - CharsBetweenSections * _TextMeasure.Width;
                            selectionPoint1.Y -=  _TextMeasure.Height;
                        }
                        else
                        {
                            selectionPoint1.X -= CharsBetweenDataColumns * _TextMeasure.Width;
                        }

                        DrawSelectionGeometry(canvas, SelectionBrush, DataPaint, selectionPoint0, selectionPoint1, SelectionArea.Data);
                    }
                }

                if (ShowText)
                {
                    var textVerticalLinePoint0 = CalculateTextVerticalLinePoint0();
                    var textVerticalLinePoint1 = CalculateTextVerticalLinePoint1();

                    if (SelectionLength != 0 && MaxVisibleRows > 0 && Columns > 0)
                    {
                        Point selectionPoint0 = ConvertOffsetToPosition(SelectedOffset, SelectionArea.Text);
                        Point selectionPoint1 = ConvertOffsetToPosition(SelectedOffset + SelectionLength, SelectionArea.Text);

                        if ((SelectedOffset + SelectionLength - Offset) / BytesPerColumn % Columns == 0)
                        {
                            // We're selecting the last column so the end point is the text vertical line (effectively)
                            selectionPoint1.X = textVerticalLinePoint0.X - CharsBetweenSections * _TextMeasure.Width;
                            selectionPoint1.Y -= _TextMeasure.Height;
                        }

                        DrawSelectionGeometry(canvas, SelectionBrush, DataPaint, selectionPoint0, selectionPoint1, SelectionArea.Text);
                    }
                }

                SKPoint origin = default;
                origin.Y = _TextMeasure.Height; // left bottom to right top

                for (var row = 0; row < MaxVisibleRows; ++row)
                {
                    if (ShowAddress)
                    {
                        if (DataSource.BaseStream.Position + BytesPerColumn <= DataSource.BaseStream.Length)
                        {
                            var textToFormat = GetFormattedAddressText(Address + (ulong)DataSource.BaseStream.Position);

                            if (AddressBrush is SolidColorBrush s)
                            {
                                DataPaint.Color = s.Color.ToSKColor();
                            }
                            canvas.DrawText(textToFormat, origin.X, origin.Y, DataPaint);

                            origin.X += (float)((CalculateAddressColumnCharWidth() + CharsBetweenSections) * _TextMeasure.Width);
                        }
                    }

                    long savedDataSourcePositionBeforeReadingData = DataSource.BaseStream.Position;

                    if (ShowData)
                    {
                        origin.X += (float)(CharsBetweenSections * _TextMeasure.Width);

                        var cachedDataColumnCharWidth = CalculateDataColumnCharWidth();

                        // Needed to track text in alternating columns so we can use a different brush when drawing
                        var evenColumnBuilder = new StringBuilder(Columns * DataWidth);
                        var oddColumnBuilder = new StringBuilder(Columns * DataWidth);

                        var column = 0;

                        // Draw text up until selection start point
                        while (column < Columns)
                        {
                            if (DataSource.BaseStream.Position + BytesPerColumn <= DataSource.BaseStream.Length)
                            {
                                if (DataSource.BaseStream.Position >= SelectedOffset)
                                {
                                    break;
                                }

                                var textToFormat = ReadFormattedData();

                                if (column % 2 == 0)
                                {
                                    evenColumnBuilder.Append(textToFormat);
                                    evenColumnBuilder.Append(' ', CharsBetweenDataColumns);

                                    oddColumnBuilder.Append(' ', textToFormat.Length + CharsBetweenDataColumns);
                                }
                                else
                                {
                                    oddColumnBuilder.Append(textToFormat);
                                    oddColumnBuilder.Append(' ', CharsBetweenDataColumns);

                                    evenColumnBuilder.Append(' ', textToFormat.Length + CharsBetweenDataColumns);
                                }
                            }
                            else
                            {
                                evenColumnBuilder.Append(' ', cachedDataColumnCharWidth + CharsBetweenDataColumns);
                                oddColumnBuilder.Append(' ', cachedDataColumnCharWidth + CharsBetweenDataColumns);
                            }

                            ++column;
                        }

                        {
                            if (Foreground is SolidColorBrush s)
                            {
                                DataPaint.Color = s.Color.ToSKColor();
                            }
                            canvas.DrawText(evenColumnBuilder.ToString(), origin.X, origin.Y, DataPaint);
                        }

                        {
                            if (AlternatingDataColumnTextBrush is SolidColorBrush s)
                            {
                                DataPaint.Color = s.Color.ToSKColor();
                            }
                            canvas.DrawText(oddColumnBuilder.ToString(), origin.X, origin.Y, DataPaint);
                        }
                        origin.X += evenColumnBuilder.Length * _TextMeasure.Width;

                        if (column < Columns)
                        {
                            // We'll reuse this builder for drawing selection text
                            evenColumnBuilder.Clear();

                            // Draw text starting from selection start point
                            while (column < Columns)
                            {
                                if (DataSource.BaseStream.Position + BytesPerColumn <= DataSource.BaseStream.Length)
                                {
                                    if (DataSource.BaseStream.Position >= SelectedOffset + SelectionLength)
                                    {
                                        break;
                                    }

                                    var textToFormat = ReadFormattedData();

                                    evenColumnBuilder.Append(textToFormat);
                                    evenColumnBuilder.Append(' ', CharsBetweenDataColumns);
                                }
                                else
                                {
                                    evenColumnBuilder.Append(' ', cachedDataColumnCharWidth + CharsBetweenDataColumns);
                                }

                                ++column;
                            }

                            {
                                if (SelectionTextBrush is SolidColorBrush s)
                                {
                                    DataPaint.Color = s.Color.ToSKColor();
                                }
                                canvas.DrawText(evenColumnBuilder.ToString(), origin.X, origin.Y, DataPaint);
                            }

                            origin.X += evenColumnBuilder.Length * _TextMeasure.Width;

                            if (column < Columns)
                            {
                                evenColumnBuilder.Clear();
                                oddColumnBuilder.Clear();

                                // Draw text after end of selection
                                while (column < Columns)
                                {
                                    if (DataSource.BaseStream.Position + BytesPerColumn <= DataSource.BaseStream.Length)
                                    {
                                        var textToFormat = ReadFormattedData();

                                        if (column % 2 == 0)
                                        {
                                            evenColumnBuilder.Append(textToFormat);
                                            evenColumnBuilder.Append(' ', CharsBetweenDataColumns);

                                            oddColumnBuilder.Append(' ', textToFormat.Length + CharsBetweenDataColumns);
                                        }
                                        else
                                        {
                                            oddColumnBuilder.Append(textToFormat);
                                            oddColumnBuilder.Append(' ', CharsBetweenDataColumns);

                                            evenColumnBuilder.Append(' ', textToFormat.Length + CharsBetweenDataColumns);
                                        }
                                    }
                                    else
                                    {
                                        evenColumnBuilder.Append(' ', cachedDataColumnCharWidth + CharsBetweenDataColumns);
                                        oddColumnBuilder.Append(' ', cachedDataColumnCharWidth + CharsBetweenDataColumns);
                                    }

                                    ++column;
                                }

                                {
                                    if (Foreground is SolidColorBrush s)
                                    {
                                        DataPaint.Color = s.Color.ToSKColor();
                                    }
                                    canvas.DrawText(evenColumnBuilder.ToString(), origin.X, origin.Y, DataPaint);
                                }

                                {
                                    if (AlternatingDataColumnTextBrush is SolidColorBrush s)
                                    {
                                        DataPaint.Color = s.Color.ToSKColor();
                                    }
                                    canvas.DrawText(oddColumnBuilder.ToString(), origin.X, origin.Y, DataPaint);
                                }

                                origin.X += oddColumnBuilder.Length * _TextMeasure.Width;
                            }
                        }

                        // Compensate for the extra space added at the end of the builder
                        origin.X += (float)((CharsBetweenSections - CharsBetweenDataColumns) * _TextMeasure.Width);
                    }

                    if (ShowText)
                    {
                        origin.X += (float)(CharsBetweenSections * _TextMeasure.Width);

                        if (ShowData)
                        {
                            // Reset the stream to read one byte at a time
                            DataSource.BaseStream.Position = savedDataSourcePositionBeforeReadingData;
                        }

                        var builder = new StringBuilder(Columns * DataWidth);

                        var column = 0;

                        // Draw text up until selection start point
                        while (column < Columns)
                        {
                            if (DataSource.BaseStream.Position + BytesPerColumn <= DataSource.BaseStream.Length)
                            {
                                if (DataSource.BaseStream.Position >= SelectedOffset)
                                {
                                    break;
                                }

                                var textToFormat = ReadFormattedText();
                                builder.Append(textToFormat);
                            }

                            ++column;
                        }

                        {
                            if (Foreground is SolidColorBrush s)
                            {
                                DataPaint.Color = s.Color.ToSKColor();
                            }
                            canvas.DrawText(builder.ToString(), origin.X, origin.Y, DataPaint);
                        }

                        if (column < Columns)
                        {
                            origin.X += builder.Length * _TextMeasure.Width;

                            builder.Clear();

                            // Draw text starting from selection start point
                            while (column < Columns)
                            {
                                if (DataSource.BaseStream.Position + BytesPerColumn <= DataSource.BaseStream.Length)
                                {
                                    if (DataSource.BaseStream.Position >= SelectedOffset + SelectionLength)
                                    {
                                        break;
                                    }

                                    var textToFormat = ReadFormattedText();
                                    builder.Append(textToFormat);
                                }

                                ++column;
                            }

                            {
                                if (SelectionTextBrush is SolidColorBrush s)
                                {
                                    DataPaint.Color = s.Color.ToSKColor();
                                }
                                canvas.DrawText(builder.ToString(), origin.X, origin.Y, DataPaint);
                            }

                            if (column < Columns)
                            {
                                origin.X += builder.Length * _TextMeasure.Width;

                                builder.Clear();

                                // Draw text after end of selection
                                while (column < Columns)
                                {
                                    if (DataSource.BaseStream.Position + BytesPerColumn <= DataSource.BaseStream.Length)
                                    {
                                        var textToFormat = ReadFormattedText();
                                        builder.Append(textToFormat);
                                    }

                                    ++column;
                                }

                                {
                                    if (Foreground is SolidColorBrush s)
                                    {
                                        DataPaint.Color = s.Color.ToSKColor();
                                    }
                                    canvas.DrawText(builder.ToString(), origin.X, origin.Y, DataPaint);
                                }
                            }
                        }
                    }

                    origin.X = 0;
                    origin.Y += _TextMeasure.Height;
                }

                DataSource.BaseStream.Position = savedDataSourcePosition;
            }
        }

        /// <summary>
        /// Scrolls the contents of the control to the specified offset.
        /// </summary>
        ///
        /// <param name="offset">
        /// The offset to scroll to.
        /// </param>
        public void ScrollToOffset(long offset)
        {
            long maxBytesDisplayed = BytesPerRow * MaxVisibleRows;

            if (Offset > offset)
            {
                // We need to scroll up
                Offset -= ((Offset - offset - 1) / BytesPerRow + 1) * BytesPerRow;
            }

            if (Offset + maxBytesDisplayed <= offset)
            {
                // We need to scroll down
                Offset += ((offset - (Offset + maxBytesDisplayed)) / BytesPerRow + 1) * BytesPerRow;
            }
        }

        private static bool IsKeyDown(VirtualKey key) => InputKeyboardSource.GetKeyStateForCurrentThread(key) == CoreVirtualKeyStates.Down;

        /// <inheritdoc/>
        protected override void OnKeyDown(KeyRoutedEventArgs e)
        {
            base.OnKeyDown(e);

            if (Columns > 0 && MaxVisibleRows > 0)
            {
                switch (e.Key)
                {
                    case VirtualKey.A:
                        {
                            if (IsKeyDown(VirtualKey.LeftControl) || IsKeyDown(VirtualKey.RightControl))
                            {
                                SelectionStart = 0;
                                SelectionEnd = DataSource.BaseStream.Length;

                                e.Handled = true;
                            }

                            break;
                        }

                    case VirtualKey.C:
                        {
                            if (IsKeyDown(VirtualKey.LeftControl) || IsKeyDown(VirtualKey.RightControl))
                            {
                                e.Handled = true;
                            }

                            break;
                        }

                    case VirtualKey.Down:
                        {
                            if (IsKeyDown(VirtualKey.LeftShift) || IsKeyDown(VirtualKey.RightShift))
                            {
                                SelectionEnd += BytesPerRow;
                            }
                            else
                            {
                                SelectionStart += BytesPerRow;
                                SelectionEnd = SelectionStart + BytesPerColumn;
                            }

                            ScrollToOffset(SelectionEnd - BytesPerColumn);

                            e.Handled = true;

                            break;
                        }

                    case VirtualKey.End:
                        {
                            if (IsKeyDown(VirtualKey.LeftControl) || IsKeyDown(VirtualKey.RightControl))
                            {
                                SelectionEnd = DataSource.BaseStream.Length;

                                if (!IsKeyDown(VirtualKey.LeftShift) && !IsKeyDown(VirtualKey.RightShift))
                                {
                                    SelectionStart = SelectionEnd - BytesPerColumn;
                                }

                                ScrollToOffset(SelectionEnd - BytesPerColumn);
                            }
                            else
                            {
                                SelectionEnd += (Offset - SelectionEnd).Mod(BytesPerRow);

                                if (!IsKeyDown(VirtualKey.LeftShift) && !IsKeyDown(VirtualKey.RightShift))
                                {
                                    SelectionStart = SelectionEnd - BytesPerColumn;
                                }

                                ScrollToOffset(SelectionEnd - BytesPerColumn);
                            }

                            e.Handled = true;

                            break;
                        }

                    case VirtualKey.Home:
                        {
                            if (IsKeyDown(VirtualKey.LeftControl) || IsKeyDown(VirtualKey.RightControl))
                            {
                                SelectionEnd = 0;

                                if (!IsKeyDown(VirtualKey.LeftShift) && !IsKeyDown(VirtualKey.RightShift))
                                {
                                    SelectionStart = SelectionEnd;
                                    SelectionEnd = SelectionStart + BytesPerColumn;
                                }

                                ScrollToOffset(SelectionEnd - BytesPerColumn);
                            }
                            else
                            {
                                // TODO: Because of the way we represent selection there is no way to distinguish at the
                                // moment whether the selection ends at the start of the current line or the end of the
                                // previous line. As such, when the Shift+End hotkey is used twice consecutively a whole
                                // new line above the current selection will be selected. This is undesirable behavior
                                // that deviates from the canonical semantics of Shift+End.
                                SelectionEnd -= (SelectionEnd - 1 - Offset).Mod(BytesPerRow) + 1;

                                if (!IsKeyDown(VirtualKey.LeftShift) && !IsKeyDown(VirtualKey.RightShift))
                                {
                                    SelectionStart = SelectionEnd;
                                    SelectionEnd = SelectionStart + BytesPerColumn;
                                }

                                ScrollToOffset(SelectionEnd - BytesPerColumn);
                            }

                            e.Handled = true;

                            break;
                        }

                    case VirtualKey.Left:
                        {
                            if (IsKeyDown(VirtualKey.LeftShift) || IsKeyDown(VirtualKey.RightShift))
                            {
                                SelectionEnd -= BytesPerColumn;
                            }
                            else
                            {
                                SelectionStart -= BytesPerColumn;
                                SelectionEnd = SelectionStart + BytesPerColumn;
                            }

                            ScrollToOffset(SelectionEnd - BytesPerColumn);

                            e.Handled = true;

                            break;
                        }

                    case VirtualKey.PageDown:
                        {
                            bool isOffsetVisibleBeforeSelectionChange = IsOffsetVisible(SelectionEnd);

                            SelectionEnd += BytesPerRow * MaxVisibleRows;

                            if (!IsKeyDown(VirtualKey.LeftShift) && !IsKeyDown(VirtualKey.RightShift))
                            {
                                SelectionStart = SelectionEnd - BytesPerColumn;
                            }

                            if (isOffsetVisibleBeforeSelectionChange)
                            {
                                ScrollToOffset(Offset + BytesPerRow * MaxVisibleRows * 2 - BytesPerColumn);
                            }
                            else
                            {
                                ScrollToOffset(SelectionEnd - BytesPerColumn);
                            }

                            e.Handled = true;
                            break;
                        }

                    case VirtualKey.PageUp:
                        {
                            bool isOffsetVisibleBeforeSelectionChange = IsOffsetVisible(SelectionEnd);

                            SelectionEnd -= BytesPerRow * MaxVisibleRows;

                            if (!IsKeyDown(VirtualKey.LeftShift) && !IsKeyDown(VirtualKey.RightShift))
                            {
                                SelectionStart = SelectionEnd - BytesPerColumn;
                                SelectionEnd = SelectionStart + BytesPerColumn;
                            }

                            if (isOffsetVisibleBeforeSelectionChange)
                            {
                                ScrollToOffset(Offset - BytesPerRow * MaxVisibleRows);
                            }
                            else
                            {
                                ScrollToOffset(SelectionEnd - BytesPerColumn);
                            }

                            e.Handled = true;
                            break;
                        }

                    case VirtualKey.Right:
                        {
                            if (IsKeyDown(VirtualKey.LeftShift) || IsKeyDown(VirtualKey.RightShift))
                            {
                                SelectionEnd += BytesPerColumn;
                            }
                            else
                            {
                                SelectionStart += BytesPerColumn;
                                SelectionEnd = SelectionStart + BytesPerColumn;
                            }

                            ScrollToOffset(SelectionEnd - BytesPerColumn);

                            e.Handled = true;
                            break;
                        }

                    case VirtualKey.Up:
                        {
                            if (IsKeyDown(VirtualKey.LeftShift) || IsKeyDown(VirtualKey.RightShift))
                            {
                                SelectionEnd -= BytesPerRow;
                            }
                            else
                            {
                                SelectionStart -= BytesPerRow;
                                SelectionEnd = SelectionStart + BytesPerColumn;
                            }

                            ScrollToOffset(SelectionEnd - BytesPerColumn);

                            e.Handled = true;
                            break;
                        }
                }
            }
        }
        protected override void OnDoubleTapped(DoubleTappedRoutedEventArgs e)
        {
            Debugger.Log(0, "s", $"OnDoubleTapped {count++}\n");

            base.OnDoubleTapped(e);
            if (e.PointerDeviceType == PointerDeviceType.Mouse)
            {
                OnMouseDoubleClick(e.GetPosition(PartCanvas));
            }
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);

            Debugger.Log(0, "s", $"OnGotFocus {count++} {e.OriginalSource.ToString()}, {FocusState}\n");
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);

            Debugger.Log(0, "s", $"OnLostFocus {count++} {e.OriginalSource.ToString()}, {FocusState}\n");
        }

        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            if (FocusState == FocusState.Unfocused)
            {
                var res = Focus(FocusState.Programmatic);
                if (!res)
                {
                    Debugger.Log(0, "s", $"Focus not moved {count++}\n");
                }
            }

            base.OnPointerPressed(e);
            var pps = e.GetCurrentPoint(this).Properties;
            if (pps != null)
            {
                if (pps.PointerUpdateKind ==  PointerUpdateKind.LeftButtonPressed)
                {
                    OnMouseLeftButtonDown(e);
                }
            }
        }
        protected override void OnPointerReleased(PointerRoutedEventArgs e)
        {
            base.OnPointerReleased(e);
            var pps = e.GetCurrentPoint(this).Properties;
            if (pps != null)
            {
                if (pps.PointerUpdateKind ==  PointerUpdateKind.LeftButtonReleased)
                {
                    OnMouseLeftButtonUp(e);
                }
            }
        }
        protected override void OnPointerCanceled(PointerRoutedEventArgs e)
        {
            base.OnPointerCanceled(e);
            var pps = e.GetCurrentPoint(this).Properties;
            if (pps != null)
            {
                if (pps.PointerUpdateKind ==  PointerUpdateKind.LeftButtonReleased)
                {
                    OnMouseLeftButtonUp(e);
                }
            }
        }
        protected override void OnPointerCaptureLost(PointerRoutedEventArgs e)
        {
            base.OnPointerCaptureLost(e);
        }
        protected override void OnPointerExited(PointerRoutedEventArgs e)
        {
            base.OnPointerExited(e);
        }
        protected override void OnPointerEntered(PointerRoutedEventArgs e)
        {
            base.OnPointerEntered(e);
        }
        /// <inheritdoc/>
        protected override void OnPointerMoved(PointerRoutedEventArgs e)
        {
            base.OnPointerMoved(e);

            switch (highlightState)
            {
                case SelectionArea.Data:
                case SelectionArea.Text:
                    {
                        var position = e.GetCurrentPoint(PartCanvas).Position;

                        var currentMouseOverOffset = ConvertPositionToOffset(position);

                        if (currentMouseOverOffset >= SelectionStart)
                        {
                            SelectionEnd = currentMouseOverOffset + BytesPerColumn;
                        }
                        else
                        {
                            SelectionEnd = currentMouseOverOffset;
                        }

                        break;
                    }
            }
        }

        /// <inheritdoc/>
        protected override void OnPointerWheelChanged(PointerRoutedEventArgs e)
        {
            base.OnPointerWheelChanged(e);
            var Delta = e.GetCurrentPoint(this).Properties.MouseWheelDelta;

            var value = PartVerticalScrollBar.Value;
            if (Delta < 0)
            {
                PartVerticalScrollBar.Value += ScrollWheelScrollRows;

                OnVerticalScrollBarScroll(PartVerticalScrollBar, ScrollEventType.SmallIncrement, PartVerticalScrollBar.Value);
            }
            else
            {
                PartVerticalScrollBar.Value -= ScrollWheelScrollRows;

                OnVerticalScrollBarScroll(PartVerticalScrollBar, ScrollEventType.SmallDecrement, PartVerticalScrollBar.Value);
            }
        }

        /// <inheritdoc/>
        private void OnMouseDoubleClick(Point position)
        {
            {
                Point addressVerticalLinePoint0 = CalculateAddressVerticalLinePoint0();

                if (position.X < addressVerticalLinePoint0.X)
                {
                    highlightBegin = SelectionArea.Address;
                    highlightState = SelectionArea.Address;

                    SelectionStart = ConvertPositionToOffset(position);
                    SelectionEnd = SelectionStart + BytesPerRow;
                }
            }
        }

        /// <inheritdoc/>
        private void OnMouseLeftButtonDown(PointerRoutedEventArgs e)
        {
            if (highlightState == SelectionArea.None && CapturePointer(e.Pointer))
            {
                Point position = e.GetCurrentPoint(PartCanvas).Position;

                Point addressVerticalLinePoint0 = CalculateAddressVerticalLinePoint0();
                Point dataVerticalLinePoint0 = CalculateDataVerticalLinePoint0();
                Point textVerticalLinePoint0 = CalculateTextVerticalLinePoint0();

                if (position.X < addressVerticalLinePoint0.X)
                {
                    highlightBegin = SelectionArea.Address;
                    highlightState = SelectionArea.Address;
                }
                else if (position.X < dataVerticalLinePoint0.X)
                {
                    highlightBegin = SelectionArea.Data;
                    highlightState = SelectionArea.Data;
                }
                else if (position.X < textVerticalLinePoint0.X)
                {
                    highlightBegin = SelectionArea.Text;
                    highlightState = SelectionArea.Text;
                }

                if (highlightState != SelectionArea.None)
                {
                    SelectionStart = ConvertPositionToOffset(position);

                    SelectionEnd = SelectionStart + BytesPerColumn;
                }
            }
        }

        /// <inheritdoc/>
        private void OnMouseLeftButtonUp(PointerRoutedEventArgs e)
        {
            highlightState = SelectionArea.None;

            ReleasePointerCapture(e.Pointer);
        }

        private static void OnPropertyChangedInvalidateMeasure(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var HexBox = (HexBox)d;

            HexBox.Reflush();
        }

        private static void OnSelectionEndChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var HexBox = (HexBox)d;

            HexBox.Reflush();

            HexBox.OnPropertyChanged(nameof(SelectionEnd));
            HexBox.OnPropertyChanged(nameof(SelectionLength));
            HexBox.OnPropertyChanged(nameof(SelectedOffset));
            HexBox.OnPropertyChanged(nameof(SelectedAddress));
            HexBox.OnPropertyChanged(nameof(IsSelectionActive));
        }

        private static void OnSelectionStartChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var HexBox = (HexBox)d;

            HexBox.Reflush();

            HexBox.OnPropertyChanged(nameof(SelectionStart));
            HexBox.OnPropertyChanged(nameof(SelectionLength));
            HexBox.OnPropertyChanged(nameof(SelectedOffset));
            HexBox.OnPropertyChanged(nameof(SelectedAddress));
            HexBox.OnPropertyChanged(nameof(IsSelectionActive));
        }

        private static object CoerceColumns(DependencyObject d, object value)
        {
            var HexBox = (HexBox)d;

            if (HexBox.MaxVisibleColumns == 0)
            {
                return (int)value;
            }
            else
            {
                return Math.Min((int)value, HexBox.MaxVisibleColumns);
            }
        }

        private static object CoerceMaxVisibleColumns(DependencyObject d, object value)
        {
            return Math.Min((int)value, MaxColumns);
        }

        private static object CoerceMaxVisibleRows(DependencyObject d, object value)
        {
            return Math.Min((int)value, MaxRows);
        }

        private static object CoerceSelectionStart(DependencyObject d, object value)
        {
            var HexBox = (HexBox)d;

            if (HexBox.DataSource != null)
            {
                long selectionStart = (long)value;

                // Selection offset cannot start in the middle of the data width
                selectionStart -= selectionStart % HexBox.BytesPerColumn;

                // Selection start cannot be at the end of the stream so adjust by data width number of bytes
                value = selectionStart.Clamp(0, HexBox.DataSource.BaseStream.Length / HexBox.BytesPerColumn * HexBox.BytesPerColumn - HexBox.BytesPerColumn);
            }
            else
            {
                value = 0L;
            }

            return value;
        }

        private static object CoerceSelectionEnd(DependencyObject d, object value)
        {
            var HexBox = (HexBox)d;

            if (HexBox.DataSource != null)
            {
                long selectionEnd = (long)value;

                // Selection offset cannot start in the middle of the data width
                selectionEnd -= selectionEnd % HexBox.BytesPerColumn;

                // Unlike selection start the selection end can be at the end of the stream
                value = selectionEnd.Clamp(0, HexBox.DataSource.BaseStream.Length / HexBox.BytesPerColumn * HexBox.BytesPerColumn);
            }
            else
            {
                value = 0L;
            }

            return value;
        }

        private static object CoerceDataWidth(DependencyObject d, object value)
        {
            var HexBox = (HexBox)d;

            if (HexBox.DataType == DataType.FloatingPoint && (int)value < 4)
            {
                value = 4;
            }

            return value;
        }

        private static object CoerceOffset(DependencyObject d, object value)
        {
            var HexBox = (HexBox)d;

            if (HexBox.DataSource != null)
            {
                long offset = (long)value;

                value = offset.Clamp(0, HexBox.DataSource.BaseStream.Length - 5*MaxColumns);
            }
            else
            {
                value = 0L;
            }

            return value;
        }

        private static bool ValidateDataWidth(object value)
        {
            bool result = false;

            switch ((int)value)
            {
                case 1:
                case 2:
                case 4:
                case 8:
                    {
                        result = true;
                        break;
                    }
            }

            return result;
        }

        private static void OnAddressChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var HexBox = (HexBox)d;

            HexBox.SelectionStart = 0;
            HexBox.SelectionEnd = 0;

            HexBox.Reflush();

            HexBox.OnPropertyChanged(nameof(Address));
            HexBox.OnPropertyChanged(nameof(SelectedAddress));
        }

        private static void OnDataTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var HexBox = (HexBox)d;

            HexBox.DataWidth = (int)e.NewValue;

            HexBox.Reflush();
        }

        private static void OnDataSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var HexBox = (HexBox)d;

            HexBox.Offset = 0;
            HexBox.SelectionStart = 0;
            HexBox.SelectionEnd = 0;

            HexBox.Reflush();
        }

        private static void OnDataWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var HexBox = (HexBox)d;

            HexBox.SelectionStart = 0;
            HexBox.SelectionEnd = 0;

            HexBox.Reflush();
        }

        private void Reflush()
        {
            if (PartCanvas != null)
            {
                PartCanvas.Invalidate();
            }
        }

        private void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private string ReadFormattedText()
        {
            StringBuilder builder = new(DataWidth);

            switch (TextFormat)
            {
                case TextFormat.Ascii:
                    {
                        for (var k = 0; k < DataWidth; ++k)
                        {
                            byte value = DataSource.ReadByte();

                            if (value > 31 && value < 127)
                            {
                                builder.Append(Convert.ToChar(value));
                            }
                            else
                            {
                                builder.Append('.');
                            }
                        }

                        break;
                    }

                default:
                    {
                        throw new InvalidOperationException($"Invalid {nameof(TextFormat)} value.");
                    }
            }

            return builder.ToString();
        }

        private string ReadFormattedData()
        {
            string result;

            switch (DataType)
            {
                case DataType.Integer:
                    {
                        switch (DataFormat)
                        {
                            case DataFormat.Decimal:
                                {
                                    switch (DataSignedness)
                                    {
                                        case DataSignedness.Signed:
                                            {
                                                switch (DataWidth)
                                                {
                                                    case 1:
                                                        {
                                                            result = $"{DataSource.ReadSByte():+#;-#;0}".PadLeft(4);
                                                            break;
                                                        }

                                                    case 2:
                                                        {
                                                            result = $"{EndianBitConverter.Convert(DataSource.ReadInt16(), Endianness):+#;-#;0}".PadLeft(6);
                                                            break;
                                                        }

                                                    case 4:
                                                        {
                                                            result = $"{EndianBitConverter.Convert(DataSource.ReadInt32(), Endianness):+#;-#;0}".PadLeft(11);
                                                            break;
                                                        }

                                                    case 8:
                                                        {
                                                            result = $"{EndianBitConverter.Convert(DataSource.ReadInt64(), Endianness):+#;-#;0}".PadLeft(21);
                                                            break;
                                                        }

                                                    default:
                                                        {
                                                            throw new InvalidOperationException($"Invalid {nameof(DataWidth)} value.");
                                                        }
                                                }

                                                break;
                                            }

                                        case DataSignedness.Unsigned:
                                            {
                                                switch (DataWidth)
                                                {
                                                    case 1:
                                                        {
                                                            result = $"{DataSource.ReadByte()}".PadLeft(3);
                                                            break;
                                                        }

                                                    case 2:
                                                        {
                                                            result = $"{EndianBitConverter.Convert(DataSource.ReadUInt16(), Endianness)}".PadLeft(5);
                                                            break;
                                                        }

                                                    case 4:
                                                        {
                                                            result = $"{EndianBitConverter.Convert(DataSource.ReadUInt32(), Endianness)}".PadLeft(10);
                                                            break;
                                                        }

                                                    case 8:
                                                        {
                                                            result = $"{EndianBitConverter.Convert(DataSource.ReadUInt64(), Endianness)}".PadLeft(20);
                                                            break;
                                                        }

                                                    default:
                                                        {
                                                            throw new InvalidOperationException($"Invalid {nameof(DataWidth)} value.");
                                                        }
                                                }

                                                break;
                                            }

                                        default:
                                            {
                                                throw new InvalidOperationException($"Invalid {nameof(DataType)} value.");
                                            }
                                    }

                                    break;
                                }

                            case DataFormat.Hexadecimal:
                                {
                                    switch (DataWidth)
                                    {
                                        case 1:
                                            {
                                                result = $"{DataSource.ReadByte(),0:X2}";
                                                break;
                                            }

                                        case 2:
                                            {
                                                result = $"{EndianBitConverter.Convert(DataSource.ReadUInt16(), Endianness),0:X4}";
                                                break;
                                            }

                                        case 4:
                                            {
                                                result = $"{EndianBitConverter.Convert(DataSource.ReadUInt32(), Endianness),0:X8}";
                                                break;
                                            }

                                        case 8:
                                            {
                                                result = $"{EndianBitConverter.Convert(DataSource.ReadUInt64(), Endianness),0:X16}";
                                                break;
                                            }

                                        default:
                                            {
                                                throw new InvalidOperationException($"Invalid {nameof(DataWidth)} value.");
                                            }
                                    }

                                    break;
                                }

                            default:
                                {
                                    throw new InvalidOperationException($"Invalid {nameof(DataFormat)} value.");
                                }
                        }

                        break;
                    }

                case DataType.FloatingPoint:
                    {
                        switch (DataWidth)
                        {
                            case 4:
                                {
                                    var bytes = BitConverter.GetBytes(EndianBitConverter.Convert(DataSource.ReadUInt32(), Endianness));
                                    var value = BitConverter.ToSingle(bytes, 0);
                                    result = $"{value:E08}".PadLeft(16);
                                    break;
                                }

                            case 8:
                                {
                                    var bytes = BitConverter.GetBytes(EndianBitConverter.Convert(DataSource.ReadUInt64(), Endianness));
                                    var value = BitConverter.ToSingle(bytes, 0);
                                    result = $"{value:E16}".PadLeft(24);
                                    break;
                                }

                            default:
                                {
                                    throw new InvalidOperationException($"Invalid {nameof(DataWidth)} value.");
                                }
                        }

                        break;
                    }

                default:
                    {
                        throw new InvalidOperationException($"Invalid {nameof(DataType)} value.");
                    }
            }

            return result;
        }

        private void CopyExecuted(object sender)
        {
            Debugger.Log(0, "s", $"CopyExecuted {count++}");
            Copy();
        }

        private bool CopyCanExecute(object sender)
        {
            Debugger.Log(0, "s", $"CopyCanExecute {count++}");
            return IsSelectionActive;
        }

        private void OnVerticalScrollBarValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            lastVerticalScrollValue = e.OldValue;
        }

        private void OnVerticalScrollBarScroll(object sender, ScrollEventArgs e)
        {
            long valueDelta = (long)(e.NewValue - lastVerticalScrollValue);

            long newOffset = Offset + valueDelta * BytesPerRow;

            Offset = newOffset;

            Reflush();
        }

        private void OnVerticalScrollBarScroll(object sender, ScrollEventType type, double NewValue)
        {
            long valueDelta = (long)(NewValue - lastVerticalScrollValue);

            long newOffset = Offset + valueDelta * BytesPerRow;

            Offset = newOffset;

            Reflush();
        }

        private string GetFormattedAddressText(ulong address)
        {
            string formattedAddressText;

            switch (AddressFormat)
            {
                case AddressFormat.Address16:
                    {
                        formattedAddressText = $"{address & 0xFFFF,0:X4}";
                        break;
                    }

                case AddressFormat.Address24:
                    {
                        formattedAddressText = $"{address >> 16 & 0xFF,0:X2}:{address & 0xFFFF,0:X4}";
                        break;
                    }

                case AddressFormat.Address32:
                    {
                        formattedAddressText = $"{address >> 16 & 0xFFFF,0:X4}:{address & 0xFFFF,0:X4}";
                        break;
                    }

                case AddressFormat.Address48:
                    {
                        formattedAddressText = $"{address >> 32 & 0xFF,0:X4}:{address & 0xFFFFFFFF,0:X8}";
                        break;
                    }

                case AddressFormat.Address64:
                    {
                        formattedAddressText = $"{address >> 32,0:X8}:{address & 0xFFFFFFFF,0:X8}";
                        break;
                    }

                default:
                    {
                        throw new InvalidOperationException($"Invalid {nameof(AddressFormat)} value.");
                    }
            }

            return formattedAddressText;
        }

        private int CalculateAddressColumnCharWidth()
        {
            int addressColumnCharWidth;

            switch (AddressFormat)
            {
                case AddressFormat.Address16:
                    {
                        addressColumnCharWidth = 4;
                        break;
                    }

                case AddressFormat.Address24:
                    {
                        addressColumnCharWidth = 7;
                        break;
                    }

                case AddressFormat.Address32:
                    {
                        addressColumnCharWidth = 9;
                        break;
                    }

                case AddressFormat.Address48:
                    {
                        addressColumnCharWidth = 13;
                        break;
                    }

                case AddressFormat.Address64:
                    {
                        addressColumnCharWidth = 17;
                        break;
                    }

                default:
                    {
                        throw new InvalidOperationException($"Invalid {nameof(AddressFormat)} value.");
                    }
            }

            return addressColumnCharWidth;
        }

        private int CalculateDataColumnCharWidth()
        {
            int dataColumnCharWidth;

            switch (DataType)
            {
                case DataType.Integer:
                    {
                        switch (DataFormat)
                        {
                            case DataFormat.Decimal:
                                {
                                    switch (DataSignedness)
                                    {
                                        case DataSignedness.Signed:
                                            {
                                                switch (DataWidth)
                                                {
                                                    case 1:
                                                        {
                                                            dataColumnCharWidth = 4;
                                                            break;
                                                        }

                                                    case 2:
                                                        {
                                                            dataColumnCharWidth = 6;
                                                            break;
                                                        }

                                                    case 4:
                                                        {
                                                            dataColumnCharWidth = 11;
                                                            break;
                                                        }

                                                    case 8:
                                                        {
                                                            dataColumnCharWidth = 21;
                                                            break;
                                                        }

                                                    default:
                                                        {
                                                            throw new InvalidOperationException($"Invalid {nameof(DataWidth)} value.");
                                                        }
                                                }
                                            }

                                            break;

                                        case DataSignedness.Unsigned:
                                            {
                                                switch (DataWidth)
                                                {
                                                    case 1:
                                                        {
                                                            dataColumnCharWidth = 3;
                                                            break;
                                                        }

                                                    case 2:
                                                        {
                                                            dataColumnCharWidth = 5;
                                                            break;
                                                        }

                                                    case 4:
                                                        {
                                                            dataColumnCharWidth = 10;
                                                            break;
                                                        }

                                                    case 8:
                                                        {
                                                            dataColumnCharWidth = 20;
                                                            break;
                                                        }

                                                    default:
                                                        {
                                                            throw new InvalidOperationException($"Invalid {nameof(DataWidth)} value.");
                                                        }
                                                }
                                            }

                                            break;

                                        default:
                                            {
                                                throw new InvalidOperationException($"Invalid {nameof(DataType)} value.");
                                            }
                                    }
                                }

                                break;

                            case DataFormat.Hexadecimal:
                                {
                                    switch (DataWidth)
                                    {
                                        case 1:
                                        case 2:
                                        case 4:
                                        case 8:
                                            {
                                                dataColumnCharWidth = 2 * DataWidth;
                                                break;
                                            }

                                        default:
                                            {
                                                throw new InvalidOperationException($"Invalid {nameof(DataWidth)} value.");
                                            }
                                    }

                                    break;
                                }

                            default:
                                {
                                    throw new InvalidOperationException($"Invalid {nameof(DataFormat)} value.");
                                }
                        }
                    }

                    break;

                case DataType.FloatingPoint:
                    {
                        switch (DataWidth)
                        {
                            case 4:
                                {
                                    dataColumnCharWidth = 16;
                                    break;
                                }

                            case 8:
                                {
                                    dataColumnCharWidth = 24;
                                    break;
                                }

                            default:
                                {
                                    throw new InvalidOperationException($"Invalid {nameof(DataWidth)} value.");
                                }
                        }
                    }

                    break;

                default:
                    {
                        throw new InvalidOperationException($"Invalid {nameof(DataType)} value.");
                    }
            }

            return dataColumnCharWidth;
        }

        private Point CalculateAddressVerticalLinePoint0()
        {
            Point point1 = default;

            if (ShowAddress)
            {
                point1.X = (CalculateAddressColumnCharWidth() + CharsBetweenSections) * _TextMeasure.Width;
            }

            return point1;
        }

        private Point CalculateAddressVerticalLinePoint1()
        {
            Point point2 = default;

            if (ShowAddress)
            {
                point2.X = (CalculateAddressColumnCharWidth() + CharsBetweenSections) * _TextMeasure.Width;
            }

            point2.Y = Math.Min((_TextMeasure.Height + 1) * MaxVisibleRows, PartCanvas.ActualHeight);

            return point2;
        }

        private Point CalculateDataVerticalLinePoint0()
        {
            Point point1 = CalculateAddressVerticalLinePoint0();

            if (ShowData)
            {
                point1.X += (CharsBetweenSections + (CalculateDataColumnCharWidth() + CharsBetweenDataColumns) * Columns - CharsBetweenDataColumns + CharsBetweenSections) * _TextMeasure.Width;
            }

            return point1;
        }

        private Point CalculateDataVerticalLinePoint1()
        {
            Point point2 = CalculateAddressVerticalLinePoint1();

            if (ShowData)
            {
                point2.X += (CharsBetweenSections + (CalculateDataColumnCharWidth() + CharsBetweenDataColumns) * Columns - CharsBetweenDataColumns + CharsBetweenSections) * _TextMeasure.Width;
            }

            return point2;
        }

        private int CalculateTextColumnCharWidth()
        {
            return BytesPerColumn;
        }

        private Point CalculateTextVerticalLinePoint0()
        {
            Point point1 = CalculateDataVerticalLinePoint0();

            if (ShowText)
            {
                point1.X += (CharsBetweenSections + CalculateTextColumnCharWidth() * Columns + CharsBetweenSections) * _TextMeasure.Width;
            }

            return point1;
        }

        private Point CalculateTextVerticalLinePoint1()
        {
            Point point2 = CalculateDataVerticalLinePoint1();

            if (ShowText)
            {
                point2.X += (CharsBetweenSections + CalculateTextColumnCharWidth() * Columns + CharsBetweenSections) * _TextMeasure.Width;
            }

            return point2;
        }

        private void UpdateState()
        {
            UpdateMaxVisibleRowsAndColumns();
            UpdateScrollBar();
        }

        private void UpdateMaxVisibleRowsAndColumns()
        {
            int maxVisibleRows = 0;
            int maxVisibleColumns = 0;

            if ((ShowAddress || ShowData || ShowText) && PartCanvas != null)
            {
                using (var paint = new SKPaint()
                {
                    TextSize = (float)FontSize,
                    Typeface = _TextTypeFace,
                    TextScaleX = 1f,
                    IsAntialias = true,
                    IsDither = true,
                    HintingLevel = SKPaintHinting.Normal,
                    TextAlign = SKTextAlign.Center,
                    FakeBoldText = true
                })
                {
                    paint.MeasureText("X", ref _TextMeasure);
                }

                _TextMeasure.Bottom = _TextMeasure.Height * _LineSize;

                maxVisibleRows = Math.Max(0, (int)(PartCanvas.ActualHeight / _TextMeasure.Height));

                if (ShowData || ShowText)
                {
                    int charsPerRow = (int)(PartCanvas.ActualWidth / _TextMeasure.Width);

                    if (ShowAddress)
                    {
                        charsPerRow -= CalculateAddressColumnCharWidth() + 2 * CharsBetweenSections;
                    }

                    if (ShowData && ShowText)
                    {
                        charsPerRow -= 3 * CharsBetweenSections;
                    }

                    int charsPerColumn = 0;

                    if (ShowData)
                    {
                        charsPerColumn += CalculateDataColumnCharWidth() + CharsBetweenDataColumns;
                    }

                    if (ShowText)
                    {
                        charsPerColumn += CalculateTextColumnCharWidth();
                    }

                    if (charsPerColumn != 0)
                    {
                        maxVisibleColumns = Math.Max(0, charsPerRow / charsPerColumn);
                    }
                }
                else
                {
                    maxVisibleColumns = 0;
                }
            }

            MaxVisibleRows = maxVisibleRows;
            MaxVisibleColumns = maxVisibleColumns;

            // Maximum visible rows has now changed and so we must update the maximum amount we should scroll by
            PartVerticalScrollBar.LargeChange = maxVisibleRows;
        }

        private void UpdateScrollBar()
        {
            if ((ShowAddress || ShowData || ShowText) && DataSource != null && Columns > 0 && MaxVisibleRows > 0)
            {
                long q = DataSource.BaseStream.Length / BytesPerRow;
                long r = DataSource.BaseStream.Length % BytesPerRow;

                // Each scroll value represents a single drawn row
                PartVerticalScrollBar.Maximum = q + (r > 0 ? 1 : 0) - MaxVisibleRows;

                // Adjust the scroll value based on the current offset
                PartVerticalScrollBar.Value = Offset / BytesPerRow;

                // Adjust again to compensate for residual bytes if the number of bytes between the start of the stream
                // and the current offset is less than the number of bytes we can display per row
                if (PartVerticalScrollBar.Value == 0 && Offset > 0)
                {
                    ++PartVerticalScrollBar.Value;
                }
            }
            else
            {
                PartVerticalScrollBar.Maximum = 0;
            }
        }

        private long ConvertPositionToOffset(Point position)
        {
            long offset = Offset;

            switch (highlightBegin)
            {
                case SelectionArea.Address:
                    {
                        Point addressVerticalLinePoint0 = CalculateAddressVerticalLinePoint0();
                        Point addressVerticalLinePoint1 = CalculateAddressVerticalLinePoint1();

                        // Clamp the Y coordinate to within the address region
                        position.Y = position.Y.Clamp(addressVerticalLinePoint0.Y, addressVerticalLinePoint1.Y);

                        // Convert the Y coordinate to the row number
                        position.Y /= _TextMeasure.Height;

                        if (position.Y >= MaxVisibleRows)
                        {
                            // Due to floating point rounding we may end up with exactly the maximum number of rows, so adjust to compensate
                            --position.Y;
                        }

                        offset += BytesPerRow * (long)position.Y;
                    }

                    break;

                case SelectionArea.Data:
                    {
                        Point addressVerticalLinePoint0 = CalculateAddressVerticalLinePoint0();

                        Point dataVerticalLinePoint0 = CalculateDataVerticalLinePoint0();
                        Point dataVerticalLinePoint1 = CalculateDataVerticalLinePoint1();

                        // Clamp the X coordinate to within the data region
                        position.X = position.X.Clamp(addressVerticalLinePoint0.X + CharsBetweenSections * _TextMeasure.Width, dataVerticalLinePoint0.X - CharsBetweenSections * _TextMeasure.Width);

                        // Normalize with respect to the data region
                        position.X -= addressVerticalLinePoint0.X + CharsBetweenSections * _TextMeasure.Width;

                        // Convert the X coordinate to the column number
                        position.X /= (CalculateDataColumnCharWidth() + CharsBetweenDataColumns) * _TextMeasure.Width;

                        if (position.X >= Columns)
                        {
                            // Due to floating point rounding we may end up with exactly the maximum number of columns, so adjust to compensate
                            --position.X;
                        }

                        // Clamp the Y coordinate to within the data region
                        position.Y = position.Y.Clamp(dataVerticalLinePoint0.Y, dataVerticalLinePoint1.Y);

                        // Convert the Y coordinate to the row number
                        position.Y /= _TextMeasure.Height;

                        if (position.Y >= MaxVisibleRows)
                        {
                            // Due to floating point rounding we may end up with exactly the maximum number of rows, so adjust to compensate
                            --position.Y;
                        }

                        offset += ((long)position.Y * Columns + (long)position.X) * BytesPerColumn;
                    }

                    break;

                case SelectionArea.Text:
                    {
                        Point dataVerticalLinePoint0 = CalculateDataVerticalLinePoint0();

                        Point textVerticalLinePoint0 = CalculateTextVerticalLinePoint0();
                        Point textVerticalLinePoint1 = CalculateTextVerticalLinePoint1();

                        // Clamp the X coordinate to within the text region
                        position.X = position.X.Clamp((float)(dataVerticalLinePoint0.X + CharsBetweenSections * _TextMeasure.Width), (float)(textVerticalLinePoint0.X - CharsBetweenSections * _TextMeasure.Width));

                        // Normalize with respect to the text region
                        position.X -= (float)(dataVerticalLinePoint0.X + CharsBetweenSections * _TextMeasure.Width);

                        // Convert the X coordinate to the column number
                        position.X /= (float)(CalculateTextColumnCharWidth() * _TextMeasure.Width);

                        if (position.X >= Columns)
                        {
                            // Due to floating point rounding we may end up with exactly the maximum number of columns, so
                            // adjust to compensate
                            --position.X;
                        }

                        // Clamp the Y coordinate to within the text region
                        position.Y = position.Y.Clamp(textVerticalLinePoint0.Y, textVerticalLinePoint1.Y);

                        // Convert the Y coordinate to the row number
                        position.Y /= _TextMeasure.Height;

                        if (position.Y >= MaxVisibleRows)
                        {
                            // Due to floating point rounding we may end up with exactly the maximum number of rows, so adjust to compensate
                            --position.Y;
                        }

                        offset += ((long)position.Y * Columns + (long)position.X) * BytesPerColumn;
                    }

                    break;

                default:
                    {
                        throw new InvalidOperationException($"Invalid highlight state ${highlightState}");
                    }
            }

            return offset;
        }

        private Point ConvertOffsetToPosition(long offset, SelectionArea relativeTo)
        {
            Point position = default;

            switch (relativeTo)
            {
                case SelectionArea.Data:
                    {
                        Point addressVerticalLinePoint0 = CalculateAddressVerticalLinePoint0();

                        position.X = addressVerticalLinePoint0.X + CharsBetweenSections * _TextMeasure.Width;
                        position.Y = addressVerticalLinePoint0.Y;

                        // Normalize requested offset to a zero based column
                        long normalizedColumn = (offset - Offset) / BytesPerColumn;

                        position.X += (normalizedColumn % Columns + Columns) % Columns * (CalculateDataColumnCharWidth() + CharsBetweenDataColumns) * _TextMeasure.Width;

                        if (normalizedColumn < 0)
                        {
                            // Negative normalized offset means the Y position is above the current offset. Because division
                            // rounds toward zero we need to compensate here.
                            position.Y += (float)(((normalizedColumn + 1) / Columns - 1) * _TextMeasure.Height);
                        }
                        else
                        {
                            position.Y += (float)(normalizedColumn / Columns * _TextMeasure.Height);
                        }
                    }

                    break;

                case SelectionArea.Text:
                    {
                        Point dataVerticalLinePoint0 = CalculateDataVerticalLinePoint0();

                        position.X = dataVerticalLinePoint0.X + CharsBetweenSections * _TextMeasure.Width;
                        position.Y = dataVerticalLinePoint0.Y;

                        // Normalize requested offset to a zero based column
                        long normalizedColumn = (offset - Offset) / BytesPerColumn;

                        position.X += (normalizedColumn % Columns + Columns) % Columns * CalculateTextColumnCharWidth() * _TextMeasure.Width;

                        if (normalizedColumn < 0)
                        {
                            // Negative normalized offset means the Y position is above the current offset. Because division
                            // rounds toward zero we need to compensate here.
                            position.Y += ((normalizedColumn + 1) / Columns - 1) * _TextMeasure.Height;
                        }
                        else
                        {
                            position.Y += normalizedColumn / Columns * _TextMeasure.Height;
                        }
                    }

                    break;

                default:
                    {
                        throw new ArgumentException($"Invalid relative area {relativeTo}", nameof(relativeTo));
                    }
            }

            return position;
        }

        private bool IsOffsetVisible(long offset)
        {
            long maxBytesDisplayed = BytesPerRow * MaxVisibleRows;

            return Offset <= offset && Offset + maxBytesDisplayed >= offset;
        }

        /// <summary>
        /// Initializes static members of the <see cref="HexBox"/> class.
        /// </summary>
        public HexBox()
        {
            DefaultStyleKey = typeof(HexBox);
        }
    }
}

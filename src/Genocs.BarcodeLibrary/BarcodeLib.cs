using Genocs.BarcodeLibrary.Symbologies;
using SkiaSharp;
using System.Drawing;
using System.Security;
using System.Text;
using System.Text.Json;
using System.Xml;
using System.Xml.Serialization;

/*
 * ***************************************************
 *                 Barcode Library                   *
 *                                                   *
 *             Written by: Brad Barnhill             *
 *                   Date: 09-21-2007                *
 *                                                   *
 *  This library was designed to give developers an  *
 *  easy class to use when they need to generate     *
 *  barcode images from a string of data.            *
 * ***************************************************
 */
namespace Genocs.BarcodeLibrary;

#region Enums

/// <summary>
/// The enumeration that specifies the type of the barcode.
/// </summary>
public enum BarcodeType
{
    /// <summary>
    /// Unspecified barcode type.
    /// </summary>
    Unspecified,

    /// <summary>
    /// UPC-A barcode type.
    /// </summary>
    UpcA,

    /// <summary>
    /// UPC-E barcode type.
    /// </summary>
    UpcE,

    /// <summary>
    /// UPC Supplemental 2-digit barcode type.
    /// </summary>
    UpcSupplemental2Digit,

    /// <summary>
    /// UPC Supplemental 5-digit barcode type.
    /// </summary>
    UpcSupplemental5Digit,

    /// <summary>
    /// EAN-13 barcode type.
    /// </summary>
    Ean13,

    /// <summary>
    /// EAN-8 barcode type.
    /// </summary>
    Ean8,

    /// <summary>
    /// Interleaved 2 of 5 barcode type.
    /// </summary>
    Interleaved2Of5,

    /// <summary>
    /// Interleaved 2 of 5 with Mod 10 Checksum barcode type.
    /// </summary>
    Interleaved2Of5Mod10,

    /// <summary>
    /// Standard 2 of 5 barcode type.
    /// </summary>
    Standard2Of5,

    /// <summary>
    /// Standard 2 of 5 with Mod 10 Checksum barcode type.
    /// </summary>
    Standard2Of5Mod10,

    /// <summary>
    /// Industrial 2 of 5 barcode type.
    /// </summary>
    Industrial2Of5,

    /// <summary>
    /// Industrial 2 of 5 with Mod 10 Checksum barcode type.
    /// </summary>
    Industrial2Of5Mod10,

    /// <summary>
    /// Code 39 barcode type.
    /// </summary>
    Code39,

    /// <summary>
    /// Code 39 Extended barcode type.
    /// </summary>
    Code39Extended,

    /// <summary>
    /// Code 39 with Mod 43 Checksum barcode type.
    /// </summary>
    Code39Mod43,

    /// <summary>
    /// Codabar barcode type.
    /// </summary>
    Codabar,

    /// <summary>
    /// PostNet barcode type.
    /// </summary>
    PostNet,

    /// <summary>
    /// Bookland barcode type.
    /// </summary>
    Bookland,

    /// <summary>
    /// ISBN barcode type.
    /// </summary>
    Isbn,

    /// <summary>
    /// JAN-13 barcode type.
    /// </summary>
    Jan13,
    MsiMod10,
    Msi2Mod10,
    MsiMod11,
    MsiMod11Mod10,
    ModifiedPlessey,
    Code11,
    Usd8,
    Ucc12,
    Ucc13,
    Logmars,
    Code128,
    Code128A,
    Code128B,
    Code128C,
    Itf14,
    Code93,
    Telepen,
    Fim,
    Pharmacode
}

public enum SaveTypes
{
    Jpg,
    Png,
    Webp,
    Unspecified
}

public enum AlignmentPositions
{
    Center,
    Left,
    Right
}

#endregion

/// <summary>
/// Generates a barcode image of a specified symbology from a string of data.
/// </summary>
[SecuritySafeCritical]
public class Barcode : IDisposable
{

    private IBarcode _iBarcode = new Blank();
    private static readonly XmlSerializer SaveDataXmlSerializer = new XmlSerializer(typeof(SaveData));

    /// <summary>
    /// Default constructor. Does not populate the raw data.  MUST be done via the RawData property before encoding.
    /// </summary>
    public Barcode()
    {
    }

    /// <summary>
    /// Constructor. Populates the raw data. No whitespace will be added before or after the barcode.
    /// </summary>
    /// <param name="data">String to be encoded.</param>
    public Barcode(string data)
    {
        RawData = data;
    }

    public Barcode(string data, BarcodeType iType)
    {
        RawData = data;
        EncodedType = iType;
        GenerateBarcode();
    }

    #region Properties

    /// <summary>
    /// Gets or sets the raw data to encode.
    /// </summary>
    public string RawData { get; set; } = string.Empty;

    /// <summary>
    /// Gets the encoded value.
    /// </summary>
    public string EncodedValue { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the Country that assigned the Manufacturer Code.
    /// </summary>
    public string CountryAssigningManufacturerCode { get; private set; } = "N/A";

    /// <summary>
    /// Gets or sets the Encoded BarcodeType (ex. UPC-A, EAN-13 ... etc).
    /// </summary>
    public BarcodeType EncodedType { set; get; } = BarcodeType.Unspecified;

    /// <summary>
    /// Gets the Image of the generated barcode.
    /// </summary>
    public SKImage EncodedImage { get; private set; }

    /// <summary>
    /// Gets or sets the color of the bars. (Default is black)
    /// </summary>
    public SKColorF ForeColor { get; set; } = SKColors.Black;

    /// <summary>
    /// Gets or sets the background color. (Default is white)
    /// </summary>
    public SKColorF BackColor { get; set; } = SKColors.White;

    /// <summary>
    /// Gets or sets the label font. (Default is Microsoft Sans Serif, 10pt, Bold)
    /// </summary>
    public SKFont LabelFont { get; set; } = new SKFont(SKTypeface.FromFamilyName("Arial", SKFontStyle.Bold), 28);

    /// <summary>
    /// Gets or sets the width of the image to be drawn. (Default is 300 pixels)
    /// </summary>
    public int Width { get; set; } = 300;

    /// <summary>
    /// Gets or sets the height of the image to be drawn. (Default is 150 pixels)
    /// </summary>
    public int Height { get; set; } = 150;

    /// <summary>
    ///   If non-null, sets the width of a bar. <see cref="Width"/> is ignored and calculated automatically.
    /// </summary>
    public int? BarWidth { get; set; }

    /// <summary>
    ///   If non-null, <see cref="Height"/> is ignored and set to <see cref="Width"/> divided by this value rounded down.
    /// </summary>
    /// <remarks><para>
    ///   As longer barcodes may be more difficult to align a scanner gun with,
    ///   growing the height based on the width automatically allows the gun to be rotated the
    ///   same amount regardless of how wide the barcode is. A recommended value is 2.
    ///   </para><para>
    ///   This value is applied to <see cref="Height"/> after after <see cref="Width"/> has been
    ///   calculated. So it is safe to use in conjunction with <see cref="BarWidth"/>.
    /// </para></remarks>
    public double? AspectRatio { get; set; }

    /// <summary>
    /// Gets or sets whether a label should be drawn below the image. (Default is false).
    /// </summary>
    public bool IncludeLabel { get; set; }

    /// <summary>
    /// Alternate label to be displayed.  (IncludeLabel must be set to true as well).
    /// </summary>
    public string AlternateLabel { get; set; }

    /// <summary>
    /// Gets or sets the amount of time in milliseconds that it took to encode and draw the barcode.
    /// </summary>
    public double EncodingTime { get; set; }

    /// <summary>
    /// Gets or sets the image format to use when encoding and returning images. (Jpeg is default).
    /// </summary>
    public SKEncodedImageFormat ImageFormat { get; set; } = SKEncodedImageFormat.Jpeg;

    /// <summary>
    /// Gets the list of errors encountered.
    /// </summary>
    public List<string> Errors => _iBarcode.Errors;

    /// <summary>
    /// Gets or sets the alignment of the barcode inside the image. (Not for Postnet or ITF-14).
    /// </summary>
    public AlignmentPositions Alignment
    {
        get;
        set;
    }

    /// <summary>
    /// Gets a byte array representation of the encoded image. (Used for Crystal Reports).
    /// </summary>
    public byte[]? EncodedImageBytes
    {
        get
        {
            if (EncodedImage == null)
                return null;

            using (var ms = new MemoryStream())
            {
                EncodedImage.Encode(ImageFormat, 100).SaveTo(ms);
                return ms.ToArray();
            }
        }
    }

    /// <summary>
    /// Gets the assembly version information.
    /// </summary>
    public static Version Version => System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

    /// <summary>
    /// Disables EAN13 invalid country code exception.
    /// </summary>
    public bool DisableEan13CountryException { get; set; } = false;
    #endregion

    #region General Encode

    /// <summary>
    /// Encodes the raw data into binary form representing bars and spaces.  Also generates an Image of the barcode.
    /// </summary>
    /// <param name="iType">BarcodeType of encoding to use.</param>
    /// <param name="stringToEncode">Raw data to encode.</param>
    /// <param name="width">Width of the resulting barcode.(pixels).</param>
    /// <param name="height">Height of the resulting barcode.(pixels).</param>
    /// <returns>Image representing the barcode.</returns>
    public SKImage Encode(BarcodeType iType, string stringToEncode, int width, int height)
    {
        Width = width;
        Height = height;
        return Encode(iType, stringToEncode);
    }

    /// <summary>
    /// Encodes the raw data into binary form representing bars and spaces.  Also generates an Image of the barcode.
    /// </summary>
    /// <param name="iType">BarcodeType of encoding to use.</param>
    /// <param name="stringToEncode">Raw data to encode.</param>
    /// <param name="foreColor">Foreground color.</param>
    /// <param name="backColor">Background color.</param>
    /// <param name="width">Width of the resulting barcode.(pixels).</param>
    /// <param name="height">Height of the resulting barcode.(pixels).</param>
    /// <returns>Image representing the barcode.</returns>
    public SKImage Encode(BarcodeType iType, string stringToEncode, SKColorF foreColor, SKColorF backColor, int width, int height)
    {
        Width = width;
        Height = height;
        return Encode(iType, stringToEncode, foreColor, backColor);
    }

    /// <summary>
    /// Encodes the raw data into binary form representing bars and spaces.  Also generates an Image of the barcode.
    /// </summary>
    /// <param name="iType">BarcodeType of encoding to use.</param>
    /// <param name="stringToEncode">Raw data to encode.</param>
    /// <param name="foreColor">Foreground color</param>
    /// <param name="backColor">Background color</param>
    /// <returns>Image representing the barcode.</returns>
    public SKImage Encode(BarcodeType iType, string stringToEncode, SKColorF foreColor, SKColorF backColor)
    {
        BackColor = backColor;
        ForeColor = foreColor;
        return Encode(iType, stringToEncode);
    }

    /// <summary>
    /// Encodes the raw data into binary form representing bars and spaces.  Also generates an Image of the barcode.
    /// </summary>
    /// <param name="iType">BarcodeType of encoding to use.</param>
    /// <param name="stringToEncode">Raw data to encode.</param>
    /// <returns>Image representing the barcode.</returns>
    public SKImage Encode(BarcodeType iType, string stringToEncode)
    {
        RawData = stringToEncode;
        return Encode(iType);
    }

    /// <summary>
    /// Encodes the raw data into binary form representing bars and spaces.  Also generates an Image of the barcode.
    /// </summary>
    /// <param name="iType">BarcodeType of encoding to use.</param>
    internal SKImage Encode(BarcodeType iType)
    {
        EncodedType = iType;
        return Encode();
    }

    public SKImage Encode(string stringToEncode)
    {
        RawData = stringToEncode;
        return Encode();
    }

    /// <summary>
    /// Encodes the raw data into a barcode image.
    /// </summary>
    internal SKImage Encode()
    {
        _iBarcode.Errors.Clear();

        var dtStartTime = DateTime.Now;

        EncodedValue = GenerateBarcode();
        RawData = _iBarcode.RawData;

        EncodedImage = SKImage.FromBitmap(Generate_Image());

        EncodingTime = (DateTime.Now - dtStartTime).TotalMilliseconds;

        return EncodedImage;
    }

    /// <summary>
    /// Encodes the raw data into binary form representing bars and spaces.
    /// </summary>
    /// <returns>
    /// Returns a string containing the binary value of the barcode. 
    /// This also sets the internal values used within the class.
    /// </returns>
    /// <param name="rawData" >Optional raw_data parameter to for quick barcode generation</param>
    public string GenerateBarcode(string rawData = "")
    {
        if (rawData != string.Empty)
        {
            RawData = rawData;
        }

        // Make sure there is something to encode
        if (RawData.Trim() == string.Empty)
            throw new Exception("EENCODE-1: Input data not allowed to be blank.");

        if (EncodedType == BarcodeType.Unspecified)
            throw new Exception("EENCODE-2: Symbology type not allowed to be unspecified.");

        EncodedValue = string.Empty;
        CountryAssigningManufacturerCode = "N/A";

        switch (EncodedType)
        {
            case BarcodeType.Ucc12:
            case BarcodeType.UpcA:
                _iBarcode = new UPCA(RawData);
                break;
            case BarcodeType.Ucc13:
            case BarcodeType.Ean13:
                _iBarcode = new EAN13(RawData, DisableEan13CountryException);
                break;
            case BarcodeType.Interleaved2Of5Mod10:
            case BarcodeType.Interleaved2Of5:
                _iBarcode = new Interleaved2of5(RawData, EncodedType);
                break;
            case BarcodeType.Industrial2Of5Mod10:
            case BarcodeType.Industrial2Of5:
            case BarcodeType.Standard2Of5Mod10:
            case BarcodeType.Standard2Of5:
                _iBarcode = new Standard2of5(RawData, EncodedType);
                break;
            case BarcodeType.Logmars:
            case BarcodeType.Code39:
                _iBarcode = new Code39(RawData);
                break;
            case BarcodeType.Code39Extended:
                _iBarcode = new Code39(RawData, true);
                break;
            case BarcodeType.Code39Mod43:
                _iBarcode = new Code39(RawData, false, true);
                break;
            case BarcodeType.Codabar:
                _iBarcode = new Codabar(RawData);
                break;
            case BarcodeType.PostNet:
                _iBarcode = new Postnet(RawData);
                break;
            case BarcodeType.Isbn:
            case BarcodeType.Bookland:
                _iBarcode = new ISBN(RawData);
                break;
            case BarcodeType.Jan13:
                _iBarcode = new JAN13(RawData);
                break;
            case BarcodeType.UpcSupplemental2Digit:
                _iBarcode = new UPCSupplement2(RawData);
                break;
            case BarcodeType.MsiMod10:
            case BarcodeType.Msi2Mod10:
            case BarcodeType.MsiMod11:
            case BarcodeType.MsiMod11Mod10:
            case BarcodeType.ModifiedPlessey:
                _iBarcode = new MSI(RawData, EncodedType);
                break;
            case BarcodeType.UpcSupplemental5Digit:
                _iBarcode = new UPCSupplement5(RawData);
                break;
            case BarcodeType.UpcE:
                _iBarcode = new UPCE(RawData);
                break;
            case BarcodeType.Ean8:
                _iBarcode = new EAN8(RawData);
                break;
            case BarcodeType.Usd8:
            case BarcodeType.Code11:
                _iBarcode = new Code11(RawData);
                break;
            case BarcodeType.Code128:
                _iBarcode = new Code128(RawData);
                break;
            case BarcodeType.Code128A:
                _iBarcode = new Code128(RawData, Code128.TYPES.A);
                break;
            case BarcodeType.Code128B:
                _iBarcode = new Code128(RawData, Code128.TYPES.B);
                break;
            case BarcodeType.Code128C:
                _iBarcode = new Code128(RawData, Code128.TYPES.C);
                break;
            case BarcodeType.Itf14:
                _iBarcode = new ITF14(RawData);
                break;
            case BarcodeType.Code93:
                _iBarcode = new Code93(RawData);
                break;
            case BarcodeType.Telepen:
                _iBarcode = new Telepen(RawData);
                break;
            case BarcodeType.Fim:
                _iBarcode = new FIM(RawData);
                break;
            case BarcodeType.Pharmacode:
                _iBarcode = new Pharmacode(RawData);
                break;

            default: throw new Exception("EENCODE-2: Unsupported encoding type specified.");
        }

        return _iBarcode.EncodedValue;
    }
    #endregion

    #region Image Functions

    /// <summary>
    /// Gets a bitmap representation of the encoded data.
    /// </summary>
    /// <returns>Bitmap of encoded value.</returns>
    private SKBitmap Generate_Image()
    {
        if (EncodedValue == string.Empty) throw new Exception("EGENERATE_IMAGE-1: Must be encoded first.");
        SKBitmap bitmap;

        var dtStartTime = DateTime.Now;

        switch (EncodedType)
        {
            case BarcodeType.Itf14:
                {
                    // Automatically calculate the Width if applicable. Quite confusing with this
                    // barcode type, and it seems this method overestimates the minimum width. But
                    // at least it�s deterministic and does't produce too small of a value.
                    if (BarWidth.HasValue)
                    {
                        // Width = (BarWidth * EncodedValue.Length) + bearerwidth + iquietzone
                        // Width = (BarWidth * EncodedValue.Length) + 2*Width/12.05 + 2*Width/20
                        // Width - 2*Width/12.05 - 2*Width/20 = BarWidth * EncodedValue.Length
                        // Width = (BarWidth * EncodedValue.Length)/(1 - 2/12.05 - 2/20)
                        // Width = (BarWidth * EncodedValue.Length)/((241 - 40 - 24.1)/241)
                        // Width = BarWidth * EncodedValue.Length / 176.9 * 241
                        // Rounding error? + 1
                        Width = (int)(241 / 176.9 * EncodedValue.Length * BarWidth.Value + 1);
                    }

                    Height = (int?)(Width / AspectRatio) ?? Height;

                    int ilHeight = Height;
                    if (IncludeLabel)
                    {
                        ilHeight -= Utils.GetFontHeight(RawData, LabelFont);
                    }

                    bitmap = new SKBitmap(Width, Height);

                    var bearerwidth = (int)((bitmap.Width) / 12.05);
                    var iquietzone = Convert.ToInt32(bitmap.Width * 0.05);
                    var iBarWidth = (bitmap.Width - (bearerwidth * 2) - (iquietzone * 2)) / EncodedValue.Length;
                    var shiftAdjustment = ((bitmap.Width - (bearerwidth * 2) - (iquietzone * 2)) % EncodedValue.Length) / 2;

                    if (iBarWidth <= 0 || iquietzone <= 0)
                        throw new Exception("EGENERATE_IMAGE-3: Image size specified not large enough to draw image. (Bar size determined to be less than 1 pixel or quiet zone determined to be less than 1 pixel)");

                    // draw image
                    var pos = 0;

                    var canvas = new SKCanvas(bitmap);

                    // fill background
                    canvas.Clear(BackColor);

                    // lines are fBarWidth wide so draw the appropriate color line vertically
                    using (var paint = new SKPaint())
                    {
                        paint.ColorF = ForeColor;
                        paint.StrokeWidth = iBarWidth;

                        // paint.Alignment = PenAlignment.Right;

                        while (pos < EncodedValue.Length)
                        {
                            // draw the appropriate color line vertically
                            if (EncodedValue[pos] == '1')
                                canvas.DrawLine(new SKPoint((pos * iBarWidth) + shiftAdjustment + bearerwidth + iquietzone, 0), new SKPoint((pos * iBarWidth) + shiftAdjustment + bearerwidth + iquietzone, Height), paint);

                            pos++;
                        }

                        // bearer bars
                        paint.StrokeWidth = (float)ilHeight / 8;
                        paint.ColorF = ForeColor;

                        // paint.Alignment = PenAlignment.Center;
                        canvas.DrawLine(new SKPoint(0, 0), new SKPoint(bitmap.Width, 0), paint);
                        canvas.DrawLine(new SKPoint(0, ilHeight), new SKPoint(bitmap.Width, ilHeight), paint);
                        canvas.DrawLine(new SKPoint(0, 0), new SKPoint(0, ilHeight), paint);
                        canvas.DrawLine(new SKPoint(bitmap.Width, 0), new SKPoint(bitmap.Width, ilHeight), paint);
                    }

                    if (IncludeLabel)
                        Labels.Label_ITF14(this, bitmap);

                    break;
                }

            case BarcodeType.UpcA:
                {
                    // Automatically calculate Width if applicable.
                    Width = BarWidth * EncodedValue.Length ?? Width;

                    // Automatically calculate Height if applicable.
                    Height = (int?)(Width / AspectRatio) ?? Height;

                    var ilHeight = Height;
                    var topLabelAdjustment = 0;

                    var iBarWidth = Width / EncodedValue.Length;

                    // set alignment
                    var shiftAdjustment = BarcodeCommon.GetAlignmentShiftAdjustment(this);

                    bitmap = new SKBitmap(Width, Height);
                    if (iBarWidth <= 0)
                        throw new Exception("EGENERATE_IMAGE-2: Image size specified not large enough to draw image. (Bar size determined to be less than 1 pixel)");

                    // draw image
                    var pos = 0;
                    var halfBarWidth = (int)(iBarWidth * 0.5);

                    using (var canvas = new SKCanvas(bitmap))
                    {
                        // clears the image and colors the entire background
                        canvas.Clear(BackColor);

                        var barwidth = iBarWidth;

                        // lines are fBarWidth wide so draw the appropriate color line vertically
                        using (var paintFore = new SKPaint())
                        {
                            paintFore.ColorF = ForeColor;
                            paintFore.StrokeWidth = barwidth;
                            while (pos < EncodedValue.Length)
                            {
                                if (EncodedValue[pos] == '1')
                                {
                                    canvas.DrawLine(new SKPoint(pos * iBarWidth + shiftAdjustment + halfBarWidth, topLabelAdjustment), new SKPoint(pos * iBarWidth + shiftAdjustment + halfBarWidth, ilHeight + topLabelAdjustment), paintFore);
                                }

                                pos++;
                            }
                        }
                    }

                    if (IncludeLabel)
                    {
                        Labels.Label_UPCA(this, bitmap);
                    }

                    break;
                }

            case BarcodeType.Ean13:
                {
                    // Automatically calculate Width if applicable.
                    Width = BarWidth * EncodedValue.Length ?? Width;

                    // Automatically calculate Height if applicable.
                    Height = (int?)(Width / AspectRatio) ?? Height;

                    var ilHeight = Height;
                    var topLabelAdjustment = 0;

                    // set alignment
                    var shiftAdjustment = BarcodeCommon.GetAlignmentShiftAdjustment(this);

                    if (IncludeLabel)
                    {
                        // Shift drawing down if top label.
                        if (AlternateLabel != null)
                        {
                            topLabelAdjustment = Utils.GetFontHeight(RawData, LabelFont);
                            ilHeight -= Utils.GetFontHeight(RawData, LabelFont);
                        }
                    }

                    bitmap = new SKBitmap(Width, Height);
                    var iBarWidth = Width / EncodedValue.Length;
                    if (iBarWidth <= 0)
                        throw new Exception("EGENERATE_IMAGE-2: Image size specified not large enough to draw image. (Bar size determined to be less than 1 pixel)");

                    // draw image
                    var pos = 0;
                    var halfBarWidth = (int)(iBarWidth * 0.5);

                    using (var canvas = new SKCanvas(bitmap))
                    {
                        // clears the image and colors the entire background
                        canvas.Clear((SKColor)BackColor);

                        using (var paint = new SKPaint())
                        {
                            paint.ColorF = ForeColor;
                            paint.StrokeWidth = iBarWidth;
                            while (pos < EncodedValue.Length)
                            {
                                if (EncodedValue[pos] == '1')
                                {
                                    canvas.DrawLine(new SKPoint(pos * iBarWidth + shiftAdjustment + halfBarWidth, topLabelAdjustment), new SKPoint(pos * iBarWidth + shiftAdjustment + halfBarWidth, ilHeight + topLabelAdjustment), paint);
                                }

                                pos++;
                            }
                        }
                    }

                    if (IncludeLabel)
                    {
                        Labels.Label_EAN13(this, bitmap);
                    }

                    break;
                }

            default:
                {
                    // Automatically calculate Width if applicable.
                    Width = BarWidth * EncodedValue.Length ?? Width;

                    // Automatically calculate Height if applicable.
                    Height = (int?)(Width / AspectRatio) ?? Height;

                    var ilHeight = Height;

                    bitmap = new SKBitmap(Width, Height);
                    var iBarWidth = Width / EncodedValue.Length;
                    var iBarWidthModifier = 1;

                    if (EncodedType == BarcodeType.PostNet)
                        iBarWidthModifier = 2;

                    // set alignment
                    var shiftAdjustment = BarcodeCommon.GetAlignmentShiftAdjustment(this);

                    if (iBarWidth <= 0)
                        throw new Exception("EGENERATE_IMAGE-2: Image size specified not large enough to draw image. (Bar size determined to be less than 1 pixel)");

                    // draw image
                    var pos = 0;
                    var halfBarWidth = (int)Math.Round(iBarWidth * 0.5);

                    using (var canvas = new SKCanvas(bitmap))
                    {
                        //clears the image and colors the entire background
                        canvas.Clear((SKColor)BackColor);

                        var barWidth = iBarWidth / iBarWidthModifier;

                        //lines are fBarWidth wide so draw the appropriate color line vertically
                        using (var backPaint = new SKPaint())
                        {
                            backPaint.ColorF = BackColor;
                            backPaint.StrokeWidth = barWidth;
                            using (var forePaint = new SKPaint())
                            {
                                forePaint.ColorF = ForeColor;
                                forePaint.StrokeWidth = barWidth;
                                while (pos < EncodedValue.Length)
                                {
                                    if (EncodedType == BarcodeType.PostNet)
                                    {
                                        // draw half bars in postnet
                                        var y = 0f;
                                        if (EncodedValue[pos] == '0')
                                            y = ilHeight - ilHeight * 0.4f;

                                        canvas.DrawLine(new SKPoint(pos * iBarWidth + shiftAdjustment + halfBarWidth, ilHeight), new SKPoint(pos * iBarWidth + shiftAdjustment + halfBarWidth, y), forePaint);
                                    }
                                    else
                                    {
                                        if (EncodedValue[pos] == '1')
                                            canvas.DrawLine(new SKPoint(pos * iBarWidth + shiftAdjustment + halfBarWidth, 0f), new SKPoint(pos * iBarWidth + shiftAdjustment + halfBarWidth, ilHeight), forePaint);
                                    }

                                    pos++;
                                }
                            }
                        }
                    }

                    if (IncludeLabel)
                    {
                        Labels.Label_Generic(this, bitmap);
                    }

                    break;
                }
        }

        EncodedImage = SKImage.FromBitmap(bitmap);

        EncodingTime += (DateTime.Now - dtStartTime).TotalMilliseconds;

        return bitmap;
    }

    /// <summary>
    /// Gets the bytes that represent the image.
    /// </summary>
    /// <param name="savetype">File type to put the data in before returning the bytes.</param>
    /// <returns>Bytes representing the encoded image.</returns>
    public byte[]? GetImageData(SaveTypes savetype)
    {
        byte[]? imageData = null;

        try
        {
            if (EncodedImage != null)
            {
                // Save the image to a memory stream so that we can get a byte array!
                using var ms = new MemoryStream();
                SaveImage(ms, savetype);
                imageData = ms.ToArray();
                ms.Flush();
                ms.Close();
            }
        }
        catch (Exception ex)
        {
            throw new Exception("EGETIMAGEDATA-1: Could not retrieve image data. " + ex.Message);
        }

        return imageData;
    }

    /// <summary>
    /// Saves an encoded image to a specified file and type.
    /// </summary>
    /// <param name="filename">Filename to save to.</param>
    /// <param name="fileType">Format to use.</param>
    public void SaveImage(string filename, SaveTypes fileType)
    {
        try
        {
            if (EncodedImage == null) return;

            using Stream fs = File.OpenWrite(filename);
            var data = EncodedImage.Encode(GetSaveType(fileType), 100);
            data.SaveTo(fs);

        }
        catch (Exception ex)
        {
            throw new Exception("ESAVEIMAGE-1: Could not save image.\n\n=======================\n\n" + ex.Message);
        }
    }

    /// <summary>
    /// Saves an encoded image to a specified stream.
    /// </summary>
    /// <param name="stream">Stream to write image to.</param>
    /// <param name="fileType">Format to use.</param>
    public void SaveImage(Stream stream, SaveTypes fileType)
    {
        try
        {
            EncodedImage?.Encode(GetSaveType(fileType), 100).SaveTo(stream);
        }
        catch (Exception ex)
        {
            throw new Exception("ESAVEIMAGE-2: Could not save image.\n\n=======================\n\n" + ex.Message);
        }
    }

    private SKEncodedImageFormat GetSaveType(SaveTypes fileType)
    {
        switch (fileType)
        {
            case SaveTypes.Jpg: return SKEncodedImageFormat.Jpeg;
            case SaveTypes.Png: return SKEncodedImageFormat.Png;
            case SaveTypes.Webp: return SKEncodedImageFormat.Webp;
            case SaveTypes.Unspecified:
            default: return ImageFormat;
        }
    }

    #endregion

    #region XML Methods

    private SaveData GetSaveData(bool includeImage = true)
    {
        var saveData = new SaveData();
        saveData.Type = EncodedType.ToString();
        saveData.RawData = RawData;
        saveData.EncodedValue = EncodedValue;
        saveData.EncodingTime = EncodingTime;
        saveData.IncludeLabel = IncludeLabel;
        saveData.Forecolor = ForeColor.ToString();
        saveData.Backcolor = BackColor.ToString();
        saveData.CountryAssigningManufacturingCode = CountryAssigningManufacturerCode;
        saveData.ImageWidth = Width;
        saveData.ImageHeight = Height;
        saveData.LabelFont = LabelFont.ToString();
        saveData.ImageFormat = ImageFormat.ToString();
        saveData.Alignment = (int)Alignment;

        // get image in base 64
        if (!includeImage) return saveData;

        using (var ms = new MemoryStream())
        {
            EncodedImage.Encode(ImageFormat, 100).SaveTo(ms);
            saveData.Image = Convert.ToBase64String(ms.ToArray(), Base64FormattingOptions.None);
        }

        return saveData;
    }

    public string ToJson(bool includeImage = true)
    {
        var bytes = JsonSerializer.SerializeToUtf8Bytes(GetSaveData(includeImage));
        return (new UTF8Encoding(false)).GetString(bytes);
    }

    public string ToXml(bool includeImage = true)
    {
        if (EncodedValue == string.Empty)
        {
            throw new Exception("EGETXML-1: Could not retrieve XML due to the barcode not being encoded first.  Please call Encode first.");
        }
        else
        {
            try
            {
                var xml = GetSaveData(includeImage);

                using (var sw = new Utf8StringWriter())
                {
                    SaveDataXmlSerializer.Serialize(sw, xml);
                    return sw.ToString();
                }

            }
            catch (Exception ex)
            {
                throw new Exception("EGETXML-2: " + ex.Message);
            }
        }
    }

    public static SaveData FromJson(Stream jsonStream)
    {
        using (jsonStream)
        {
            if (jsonStream is MemoryStream)
            {
                return JsonSerializer.Deserialize<SaveData>(((MemoryStream)jsonStream).ToArray());
            }

            using (var memoryStream = new MemoryStream())
            {
                jsonStream.CopyTo(memoryStream);
                return JsonSerializer.Deserialize<SaveData>(memoryStream.ToArray());
            }
        }
    }

    public static SaveData FromXml(Stream xmlStream)
    {
        try
        {
            using (var reader = XmlReader.Create(xmlStream))
            {
                return (SaveData)SaveDataXmlSerializer.Deserialize(reader);
            }
        }
        catch (Exception ex)
        {
            throw new Exception("EGETIMAGEFROMXML-1: " + ex.Message);
        }
    }

    public static SKImage GetImageFromSaveData(SaveData saveData)
    {
        try
        {
            // loading it to memory stream and then to image object
            using (var ms = new MemoryStream(Convert.FromBase64String(saveData.Image)))
            {
                return SKImage.FromBitmap(SKBitmap.Decode(ms));
            }
        }
        catch (Exception ex)
        {
            throw new Exception("EGETIMAGEFROMXML-1: " + ex.Message);
        }
    }

    public class Utf8StringWriter : StringWriter
    {
        public override Encoding Encoding => new UTF8Encoding(false);
    }
    #endregion

    #region Static Encode Methods

    /// <summary>
    /// Encodes the raw data into binary form representing bars and spaces.  Also generates an Image of the barcode.
    /// </summary>
    /// <param name="iType">BarcodeType of encoding to use.</param>
    /// <param name="data">Raw data to encode.</param>
    /// <returns>Image representing the barcode.</returns>
    public static SKImage DoEncode(BarcodeType iType, string data)
    {
        using (var b = new Barcode())
        {
            return b.Encode(iType, data);
        }
    }

    /// <summary>
    /// Encodes the raw data into binary form representing bars and spaces.  Also generates an Image of the barcode.
    /// </summary>
    /// <param name="iType">BarcodeType of encoding to use.</param>
    /// <param name="data">Raw data to encode.</param>
    /// <param name="xml">XML representation of the data and the image of the barcode.</param>
    /// <returns>Image representing the barcode.</returns>
    public static SKImage DoEncode(BarcodeType iType, string data, out string xml)
    {
        using (var b = new Barcode())
        {
            var i = b.Encode(iType, data);
            xml = b.ToXml();
            return i;
        }
    }

    /// <summary>
    /// Encodes the raw data into binary form representing bars and spaces.  Also generates an Image of the barcode.
    /// </summary>
    /// <param name="iType">BarcodeType of encoding to use.</param>
    /// <param name="data">Raw data to encode.</param>
    /// <param name="includeLabel">Include the label at the bottom of the image with data encoded.</param>
    /// <returns>Image representing the barcode.</returns>
    public static SKImage DoEncode(BarcodeType iType, string data, bool includeLabel)
    {
        using (var b = new Barcode())
        {
            b.IncludeLabel = includeLabel;
            return b.Encode(iType, data);
        }
    }

    /// <summary>
    /// Encodes the raw data into binary form representing bars and spaces.  Also generates an Image of the barcode.
    /// </summary>
    /// <param name="iType">BarcodeType of encoding to use.</param>
    /// <param name="data">Raw data to encode.</param>
    /// <param name="includeLabel">Include the label at the bottom of the image with data encoded.</param>
    /// <param name="width">Width of the resulting barcode.(pixels)</param>
    /// <param name="height">Height of the resulting barcode.(pixels)</param>
    /// <returns>Image representing the barcode.</returns>
    public static SKImage DoEncode(BarcodeType iType, string data, bool includeLabel, int width, int height)
    {
        using (var b = new Barcode())
        {
            b.IncludeLabel = includeLabel;
            return b.Encode(iType, data, width, height);
        }
    }

    /// <summary>
    /// Encodes the raw data into binary form representing bars and spaces.  Also generates an Image of the barcode.
    /// </summary>
    /// <param name="iType">BarcodeType of encoding to use.</param>
    /// <param name="data">Raw data to encode.</param>
    /// <param name="includeLabel">Include the label at the bottom of the image with data encoded.</param>
    /// <param name="drawColor">Foreground color</param>
    /// <param name="backColor">Background color</param>
    /// <returns>Image representing the barcode.</returns>
    public static SKImage DoEncode(BarcodeType iType, string data, bool includeLabel, Color drawColor, Color backColor)
    {
        using (var b = new Barcode())
        {
            b.IncludeLabel = includeLabel;
            return b.Encode(iType, data, new SKColor(drawColor.R, drawColor.G, drawColor.B, drawColor.A), new SKColor(backColor.R, backColor.G, backColor.B, backColor.A));
        }
    }

    /// <summary>
    /// Encodes the raw data into binary form representing bars and spaces.  Also generates an Image of the barcode.
    /// </summary>
    /// <param name="iType">BarcodeType of encoding to use.</param>
    /// <param name="data">Raw data to encode.</param>
    /// <param name="includeLabel">Include the label at the bottom of the image with data encoded.</param>
    /// <param name="drawColor">Foreground color</param>
    /// <param name="backColor">Background color</param>
    /// <param name="width">Width of the resulting barcode.(pixels)</param>
    /// <param name="height">Height of the resulting barcode.(pixels)</param>
    /// <returns>Image representing the barcode.</returns>
    public static SKImage DoEncode(BarcodeType iType, string data, bool includeLabel, Color drawColor, Color backColor, int width, int height)
    {
        using (var b = new Barcode())
        {
            b.IncludeLabel = includeLabel;
            return b.Encode(iType, data, new SKColor(drawColor.R, drawColor.G, drawColor.B, drawColor.A), new SKColor(backColor.R, backColor.G, backColor.B, backColor.A), width, height);
        }
    }

    /// <summary>
    /// Encodes the raw data into binary form representing bars and spaces.  Also generates an Image of the barcode.
    /// </summary>
    /// <param name="iType">BarcodeType of encoding to use.</param>
    /// <param name="data">Raw data to encode.</param>
    /// <param name="includeLabel">Include the label at the bottom of the image with data encoded.</param>
    /// <param name="drawColor">Foreground color.</param>
    /// <param name="backColor">Background color.</param>
    /// <param name="width">Width of the resulting barcode.(pixels).</param>
    /// <param name="height">Height of the resulting barcode.(pixels).</param>
    /// <param name="xml">XML representation of the data and the image of the barcode.</param>
    /// <returns>Image representing the barcode.</returns>
    public static SKImage DoEncode(BarcodeType iType, string data, bool includeLabel, Color drawColor, Color backColor, int width, int height, out string xml)
    {
        using (var b = new Barcode())
        {
            b.IncludeLabel = includeLabel;
            var i = b.Encode(iType, data, new SKColor(drawColor.R, drawColor.G, drawColor.B, drawColor.A), new SKColor(backColor.R, backColor.G, backColor.B, backColor.A), width, height);
            xml = b.ToXml();
            return i;
        }
    }

    #region IDisposable Support
    private bool _disposedValue; // To detect redundant calls

    protected virtual void Dispose(bool disposing)
    {
        if (_disposedValue) return;
        if (disposing)
        {
            // TODO: dispose managed state (managed objects).
        }

        // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
        // TODO: set large fields to null.

        _disposedValue = true;
        LabelFont?.Dispose();
        LabelFont = null;

        EncodedImage?.Dispose();
        EncodedImage = null;

        RawData = null;
        EncodedValue = null;
        CountryAssigningManufacturerCode = null;
    }

    ~Barcode()
    {
        // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        Dispose(false);
    }

    // This code added to correctly implement the disposable pattern.
    void IDisposable.Dispose()
    {
        // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        Dispose(true);

        // Call GC.SuppressFinalize(object). This will prevent derived types that introduce a finalizer from needing to re-implement 'IDisposable' to call it.
        GC.SuppressFinalize(this);
    }
    #endregion

    #endregion
}

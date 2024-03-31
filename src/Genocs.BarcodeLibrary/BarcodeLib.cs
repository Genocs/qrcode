using Genocs.BarcodeLibrary.Symbologies;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Processing;
using System.Security;
using System.Text;
using System.Text.Json;
using System.Xml;
using System.Xml.Serialization;

namespace Genocs.BarcodeLibrary;

#region Enums
public enum TYPE : int
{
    UNSPECIFIED,
    UPCA,
    UPCE,
    UPC_SUPPLEMENTAL_2DIGIT,
    UPC_SUPPLEMENTAL_5DIGIT,
    EAN13,
    EAN8,
    Interleaved2of5,
    Interleaved2of5_Mod10,
    Standard2of5,
    Standard2of5_Mod10,
    Industrial2of5,
    Industrial2of5_Mod10,
    CODE39,
    CODE39Extended,
    CODE39_Mod43,
    Codabar,
    PostNet,
    BOOKLAND,
    ISBN,
    JAN13,
    MSI_Mod10,
    MSI_2Mod10,
    MSI_Mod11,
    MSI_Mod11_Mod10,
    Modified_Plessey,
    CODE11,
    USD8,
    UCC12,
    UCC13,
    LOGMARS,
    CODE128,
    CODE128A,
    CODE128B,
    CODE128C,
    ITF14,
    CODE93,
    TELEPEN,
    FIM,
    PHARMACODE
};

public enum SaveTypes : int
{
    JPG,
    BMP,
    PNG,
    GIF,
    UNSPECIFIED
}

public enum AlignmentPositions : int
{
    CENTER,
    LEFT,
    RIGHT
}

public enum LabelPositions : int
{
    TOPLEFT,
    TOPCENTER,
    TOPRIGHT,
    BOTTOMLEFT,
    BOTTOMCENTER,
    BOTTOMRIGHT
}

#endregion

/// <summary>
/// Generates a barcode image of a specified symbology from a string of data.
/// </summary>
[SecuritySafeCritical]
public class Barcode : IDisposable
{

    private IBarcode ibarcode = new Blank();
    private string Encoded_Value = "";
    private string _Country_Assigning_Manufacturer_Code = "N/A";
    private TYPE EncodingType = TYPE.UNSPECIFIED;
    private Image _Encoded_Image = null;

    public Color ForeColor { get; set; } = Color.Black;
    public Color BackColor { get; set; } = Color.White;
    public int Width { get; set; } = 300;
    public int Height { get; set; } = 150;


    /// <summary>
    /// Gets or sets the image format to use when encoding and returning images. (Jpeg is default)
    /// </summary>
    public IImageEncoder ImageFormat { get; set; }


    /// <summary>
    /// Gets or sets the label font. (Default is Microsoft Sans Serif, 10pt, Bold)
    /// </summary>
    public Font LabelFont { get; set; }

    /// <summary>
    /// Gets or sets the location of the label in relation to the barcode. (BOTTOMCENTER is default)
    /// </summary>
    public LabelPositions LabelPosition { get; set; } = LabelPositions.BOTTOMCENTER;

    //        private RotateFlipType _RotateFlipType = RotateFlipType.RotateNoneFlipNone;
    public bool StandardizeLabel { get; set; } = true;


    #region Constructors
    /// <summary>
    /// Default constructor.  
    /// Does not populate the raw data.
    /// MUST be done via the RawData property before encoding.
    /// </summary>
    public Barcode()
    {
        FontCollection collection = new FontCollection();
        if (collection.TryFind("Arial", out FontFamily family))
        {
            LabelFont = family.CreateFont(12, FontStyle.Regular);
        }
    }

    /// <summary>
    /// Constructor. Populates the raw data. No whitespace will be added before or after the barcode.
    /// </summary>
    /// <param name="data">String to be encoded.</param>
    public Barcode(string data) : this()
    {
        this.RawData = data;
    }

    public Barcode(string data, TYPE iType) : this(data)
    {
        this.EncodingType = iType;
        GenerateBarcode(null);
    }
    #endregion


    /// <summary>
    ///   The default resolution of 96 dots per inch.
    /// </summary>
    const float DefaultResolution = 96f;

    /// <summary>
    ///   The number of pixels in one point at 96DPI. Since there are 72 points in an inch, this is
    ///   96/72.
    /// </summary>
    /// <remarks><para>
    ///   Used when calculating default font size in terms of points at 96DPI by manually calculating
    ///   pixels to avoid being affected by the system DPI. See issue #100
    ///   and https://stackoverflow.com/a/10800363.
    /// </para></remarks>
    public const float DotsPerPointAt96Dpi = DefaultResolution / 72;


    #region Properties

    /// <summary>
    /// Gets or sets the raw data to encode.
    /// </summary>
    public string RawData { get; set; } = "";

    /// <summary>
    /// Gets the encoded value.
    /// </summary>
    public string EncodedValue
    {
        get { return Encoded_Value; }
    }

    /// <summary>
    /// Gets the Country that assigned the Manufacturer Code.
    /// </summary>
    public string Country_Assigning_Manufacturer_Code
    {
        get { return _Country_Assigning_Manufacturer_Code; }
    }

    /// <summary>
    /// Gets or sets the Encoded Type (ex. UPC-A, EAN-13 ... etc)
    /// </summary>
    public TYPE EncodedType
    {
        set { EncodingType = value; }
        get { return EncodingType; }
    }

    /// <summary>
    /// Gets the Image of the generated barcode.
    /// </summary>
    public Image EncodedImage
    {
        get
        {
            return _Encoded_Image;
        }
    }


    //public Font LabelFont
    //{
    //    get { return this._LabelFont; }
    //    set { this._LabelFont = value; }
    //}


    /// <summary>
    /// Gets or sets the degree in which to rotate/flip the image.(No action is default)
    /// </summary>
    //public RotateFlipType RotateFlipType
    //{
    //    get { return _RotateFlipType; }
    //    set { _RotateFlipType = value; }
    //}



    /// <summary>
    ///   The number of pixels per horizontal inch. Used when creating the Bitmap.
    /// </summary>
    public float HoritontalResolution { get; set; } = DefaultResolution;

    /// <summary>
    ///   The number of pixels per vertical inch. Used when creating the Bitmap.
    /// </summary>
    public float VerticalResolution { get; set; } = DefaultResolution;

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
    /// Gets or sets whether a label should be drawn below the image. (Default is false)
    /// </summary>
    public bool IncludeLabel { get; set; }

    /// <summary>
    /// Alternate label to be displayed.  (IncludeLabel must be set to true as well)
    /// </summary>
    public String AlternateLabel { get; set; }



    /// <summary>
    /// Gets the list of errors encountered.
    /// </summary>
    public List<string> Errors
    {
        get { return this.ibarcode.Errors; }
    }

    /// <summary>
    /// Gets or sets the alignment of the barcode inside the image. (Not for Postnet or ITF-14)
    /// </summary>
    public AlignmentPositions Alignment { get; set; }


    /// <summary>
    /// Gets the assembly version information.
    /// </summary>
    public static Version Version
    {
        get { return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version; }
    }

    #endregion

    /// <summary>
    /// Represents the size of an image in real world coordinates (millimeters or inches).
    /// </summary>
    public class ImageSize
    {
        public ImageSize(double width, double height, bool metric)
        {
            Width = width;
            Height = height;
            Metric = metric;
        }

        public double Width { get; set; }
        public double Height { get; set; }
        public bool Metric { get; set; }
    }

    #region General Encode

    /// <summary>
    /// Encodes the raw data into binary form representing bars and spaces.  Also generates an Image of the barcode.
    /// </summary>
    /// <param name="iType">Type of encoding to use.</param>
    /// <param name="StringToEncode">Raw data to encode.</param>
    /// <param name="Width">Width of the resulting barcode.(pixels)</param>
    /// <param name="Height">Height of the resulting barcode.(pixels)</param>
    /// <returns>Image representing the barcode.</returns>
    public Image Encode(TYPE iType, string StringToEncode, int Width, int Height)
    {
        this.Width = Width;
        this.Height = Height;
        return Encode(iType, StringToEncode);
    }

    /// <summary>
    /// Encodes the raw data into binary form representing bars and spaces.  Also generates an Image of the barcode.
    /// </summary>
    /// <param name="iType">Type of encoding to use.</param>
    /// <param name="StringToEncode">Raw data to encode.</param>
    /// <param name="DrawColor">Foreground color</param>
    /// <param name="BackColor">Background color</param>
    /// <param name="Width">Width of the resulting barcode.(pixels)</param>
    /// <param name="Height">Height of the resulting barcode.(pixels)</param>
    /// <returns>Image representing the barcode.</returns>
    public Image Encode(TYPE iType, string StringToEncode, Color ForeColor, Color BackColor, int Width, int Height)
    {
        this.Width = Width;
        this.Height = Height;
        return Encode(iType, StringToEncode, ForeColor, BackColor);
    }

    /// <summary>
    /// Encodes the raw data into binary form representing bars and spaces.  Also generates an Image of the barcode.
    /// </summary>
    /// <param name="iType">Type of encoding to use.</param>
    /// <param name="StringToEncode">Raw data to encode.</param>
    /// <param name="DrawColor">Foreground color</param>
    /// <param name="BackColor">Background color</param>
    /// <returns>Image representing the barcode.</returns>
    public Image Encode(TYPE iType, string StringToEncode, Color ForeColor, Color BackColor)
    {
        this.BackColor = BackColor;
        this.ForeColor = ForeColor;
        return Encode(iType, StringToEncode);
    }

    /// <summary>
    /// Encodes the raw data into binary form representing bars and spaces.  Also generates an Image of the barcode.
    /// </summary>
    /// <param name="iType">Type of encoding to use.</param>
    /// <param name="StringToEncode">Raw data to encode.</param>
    /// <returns>Image representing the barcode.</returns>
    public Image Encode(TYPE iType, string StringToEncode)
    {
        RawData = StringToEncode;
        return Encode(iType);
    }

    /// <summary>
    /// Encodes the raw data into binary form representing bars and spaces.  Also generates an Image of the barcode.
    /// </summary>
    /// <param name="iType">Type of encoding to use.</param>
    internal Image Encode(TYPE iType)
    {
        EncodingType = iType;
        return Encode();
    }

    /// <summary>
    /// Encodes the raw data into a barcode image.
    /// </summary>
    internal Image Encode()
    {
        ibarcode.Errors.Clear();

        this.Encoded_Value = GenerateBarcode();
        this.RawData = ibarcode.RawData;

        _Encoded_Image = GenerateImage();

        //            this.EncodedImage.RotateFlip(this.RotateFlipType);

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
    public string GenerateBarcode(string? rawData = null)
    {
        if (!string.IsNullOrWhiteSpace(rawData))
        {
            RawData = rawData.Trim();
        }

        // Make sure there is something to encode
        if (string.IsNullOrWhiteSpace(RawData))
        {
            throw new Exception("EENCODE-1: Input data not allowed to be blank.");
        }

        if (EncodedType == TYPE.UNSPECIFIED)
        {
            throw new Exception("EENCODE-2: Symbology type not allowed to be unspecified.");
        }

        Encoded_Value = "";
        _Country_Assigning_Manufacturer_Code = "N/A";

        switch (EncodingType)
        {
            case TYPE.UCC12:
            case TYPE.UPCA: // Encode_UPCA();
                ibarcode = new UPCA(RawData);
                break;
            case TYPE.UCC13:
            case TYPE.EAN13: // Encode_EAN13();
                ibarcode = new EAN13(RawData);
                break;
            case TYPE.Interleaved2of5_Mod10:
            case TYPE.Interleaved2of5: // Encode_Interleaved2of5();
                ibarcode = new Interleaved2of5(RawData, EncodingType);
                break;
            case TYPE.Industrial2of5_Mod10:
            case TYPE.Industrial2of5:
            case TYPE.Standard2of5_Mod10:
            case TYPE.Standard2of5: // Encode_Standard2of5();
                ibarcode = new Standard2of5(RawData, EncodingType);
                break;
            case TYPE.LOGMARS:
            case TYPE.CODE39: // Encode_Code39();
                ibarcode = new Code39(RawData);
                break;
            case TYPE.CODE39Extended:
                ibarcode = new Code39(RawData, true);
                break;
            case TYPE.CODE39_Mod43:
                ibarcode = new Code39(RawData, false, true);
                break;
            case TYPE.Codabar: // Encode_Codabar();
                ibarcode = new Codabar(RawData);
                break;
            case TYPE.PostNet: //Encode_PostNet();
                ibarcode = new Postnet(RawData);
                break;
            case TYPE.ISBN:
            case TYPE.BOOKLAND: //Encode_ISBN_Bookland();
                ibarcode = new ISBN(RawData);
                break;
            case TYPE.JAN13: //Encode_JAN13();
                ibarcode = new JAN13(RawData);
                break;
            case TYPE.UPC_SUPPLEMENTAL_2DIGIT: //Encode_UPCSupplemental_2();
                ibarcode = new UPCSupplement2(RawData);
                break;
            case TYPE.MSI_Mod10:
            case TYPE.MSI_2Mod10:
            case TYPE.MSI_Mod11:
            case TYPE.MSI_Mod11_Mod10:
            case TYPE.Modified_Plessey: //Encode_MSI();
                ibarcode = new MSI(RawData, EncodingType);
                break;
            case TYPE.UPC_SUPPLEMENTAL_5DIGIT: //Encode_UPCSupplemental_5();
                ibarcode = new UPCSupplement5(RawData);
                break;
            case TYPE.UPCE: //Encode_UPCE();
                ibarcode = new UPCE(RawData);
                break;
            case TYPE.EAN8: //Encode_EAN8();
                ibarcode = new EAN8(RawData);
                break;
            case TYPE.USD8:
            case TYPE.CODE11: //Encode_Code11();
                ibarcode = new Code11(RawData);
                break;
            case TYPE.CODE128: //Encode_Code128();
                ibarcode = new Code128(RawData);
                break;
            case TYPE.CODE128A:
                ibarcode = new Code128(RawData, Code128.TYPES.A);
                break;
            case TYPE.CODE128B:
                ibarcode = new Code128(RawData, Code128.TYPES.B);
                break;
            case TYPE.CODE128C:
                ibarcode = new Code128(RawData, Code128.TYPES.C);
                break;
            case TYPE.ITF14:
                ibarcode = new ITF14(RawData);
                break;
            case TYPE.CODE93:
                ibarcode = new Code93(RawData);
                break;
            case TYPE.TELEPEN:
                ibarcode = new Telepen(RawData);
                break;
            case TYPE.FIM:
                ibarcode = new FIM(RawData);
                break;
            case TYPE.PHARMACODE:
                ibarcode = new Pharmacode(RawData);
                break;

            default: throw new Exception("EENCODE-2: Unsupported encoding type specified.");
        }

        return ibarcode.EncodedValue;
    }

    #endregion

    #region Image Functions
    /// <summary>
    /// Create and preconfigures a Bitmap for use by the library. Ensures it is independent from
    /// system DPI, etc.
    /// </summary>
    internal Image CreateBitmap(int width, int height)
    {
        return new Image<SixLabors.ImageSharp.PixelFormats.Rgba32>(width, height);
    }

    /// <summary>
    /// Gets a bitmap representation of the encoded data.
    /// </summary>
    /// <returns>Bitmap of encoded value.</returns>
    private Image GenerateImage()
    {
        if (Encoded_Value == string.Empty)
            throw new Exception("EGENERATE_IMAGE-1: Must be encoded first.");

        Image bitmap = null;

        DateTime dtStartTime = DateTime.Now;

        switch (EncodingType)
        {
            case TYPE.ITF14:
                {
                    // Automatically calculate the Width if applicable. Quite confusing with this
                    // barcode type, and it seems this method overestimates the minimum width. But
                    // at least it's deterministic and doesn't produce too small of a value.
                    if (BarWidth.HasValue)
                    {
                        // Width = (BarWidth * EncodedValue.Length) + bearerwidth + iquietzone
                        // Width = (BarWidth * EncodedValue.Length) + 2*Width/12.05 + 2*Width/20
                        // Width - 2*Width/12.05 - 2*Width/20 = BarWidth * EncodedValue.Length
                        // Width = (BarWidth * EncodedValue.Length)/(1 - 2/12.05 - 2/20)
                        // Width = (BarWidth * EncodedValue.Length)/((241 - 40 - 24.1)/241)
                        // Width = BarWidth * EncodedValue.Length / 176.9 * 241
                        // Rounding error? + 1
                        Width = (int)(241 / 176.9 * Encoded_Value.Length * BarWidth.Value + 1);
                    }
                    Height = (int?)(Width / AspectRatio) ?? Height;

                    int ILHeight = Height;
                    if (IncludeLabel)
                    {
                        //ILHeight -= this.LabelFont.Height;
                    }

                    bitmap = CreateBitmap(Width, Height);

                    int bearerwidth = (int)((bitmap.Width) / 12.05);
                    int iquietzone = Convert.ToInt32(bitmap.Width * 0.05);
                    int iBarWidth = (bitmap.Width - (bearerwidth * 2) - (iquietzone * 2)) / Encoded_Value.Length;
                    int shiftAdjustment = ((bitmap.Width - (bearerwidth * 2) - (iquietzone * 2)) % Encoded_Value.Length) / 2;

                    if (iBarWidth <= 0 || iquietzone <= 0)
                        throw new Exception("EGENERATE_IMAGE-3: Image size specified not large enough to draw image. (Bar size determined to be less than 1 pixel or quiet zone determined to be less than 1 pixel)");

                    // fill background
                    bitmap.Mutate(x => x.Fill(BackColor));

                    // draw image
                    int pos = 0;

                    // lines are fBarWidth wide so draw the appropriate color line vertically
                    Pen pen = new Pen(ForeColor, iBarWidth);

                    while (pos < Encoded_Value.Length)
                    {
                        //draw the appropriate color line vertically
                        if (Encoded_Value[pos] == '1')
                        {
                            bitmap.Mutate(x => x.DrawLines(pen, new Point((pos * iBarWidth) + shiftAdjustment + bearerwidth + iquietzone, 0), new Point((pos * iBarWidth) + shiftAdjustment + bearerwidth + iquietzone, Height)));
                        }
                        pos++;
                    }

                    // bearer bars
                    bitmap.Mutate(x => x.DrawLines(pen, new Point(0, 0), new Point(bitmap.Width, 0)));//top

                    bitmap.Mutate(x => x.DrawLines(pen, new Point(0, ILHeight), new Point(bitmap.Width, ILHeight)));//bottom
                    bitmap.Mutate(x => x.DrawLines(pen, new Point(0, 0), new Point(0, ILHeight)));//left
                    bitmap.Mutate(x => x.DrawLines(pen, new Point(bitmap.Width, 0), new Point(bitmap.Width, ILHeight)));//right

                    //if (IncludeLabel)
                    //    Labels.Label_ITF14(this, bitmap);

                    break;
                }

            case TYPE.UPCA:
                {
                    // Automatically calculate Width if applicable.
                    Width = BarWidth * Encoded_Value.Length ?? Width;

                    // Automatically calculate Height if applicable.
                    Height = (int?)(Width / AspectRatio) ?? Height;

                    int ILHeight = Height;
                    int topLabelAdjustment = 0;

                    int shiftAdjustment = 0;
                    int iBarWidth = Width / Encoded_Value.Length;

                    //set alignment
                    switch (Alignment)
                    {
                        case AlignmentPositions.LEFT:
                            shiftAdjustment = 0;
                            break;
                        case AlignmentPositions.RIGHT:
                            shiftAdjustment = (Width % Encoded_Value.Length);
                            break;
                        case AlignmentPositions.CENTER:
                        default:
                            shiftAdjustment = (Width % Encoded_Value.Length) / 2;
                            break;
                    }

                    //if  (IncludeLabel)
                    //{
                    //    if ((AlternateLabel == null || RawData.StartsWith(AlternateLabel)) && _StandardizeLabel)
                    //    {
                    //        // UPCA standardized label
                    //        string defTxt = RawData;
                    //        string labTxt = defTxt.Substring(0, 1) + "--" + defTxt.Substring(1, 6) + "--" + defTxt.Substring(7);

                    //        Font labFont = new Font(this.LabelFont != null ? this.LabelFont.FontFamily.Name : "Arial", Labels.getFontsize(this, Width, Height, labTxt) * DotsPerPointAt96Dpi, FontStyle.Regular, GraphicsUnit.Pixel);
                    //        if (this.LabelFont != null)
                    //        {
                    //            this.LabelFont.Dispose();
                    //        }
                    //        LabelFont = labFont;

                    //        ILHeight -= (labFont.Height / 2);

                    //        iBarWidth = (int)Width / Encoded_Value.Length;
                    //    }
                    //    else
                    //    {
                    //        // Shift drawing down if top label.
                    //        if ((LabelPosition & (LabelPositions.TOPCENTER | LabelPositions.TOPLEFT | LabelPositions.TOPRIGHT)) > 0)
                    //            topLabelAdjustment = this.LabelFont.Height;

                    //        ILHeight -= this.LabelFont.Height;
                    //    }
                    //}

                    bitmap = CreateBitmap(Width, Height);
                    int iBarWidthModifier = 1;
                    if (iBarWidth <= 0)
                        throw new Exception("EGENERATE_IMAGE-2: Image size specified not large enough to draw image. (Bar size determined to be less than 1 pixel)");

                    //draw image
                    int pos = 0;
                    int halfBarWidth = (int)(iBarWidth * 0.5);

                    //fill background
                    bitmap.Mutate(x => x.Fill(BackColor));

                    //lines are fBarWidth wide so draw the appropriate color line vertically
                    Pen pen = new Pen(ForeColor, iBarWidth / iBarWidthModifier);

                    while (pos < Encoded_Value.Length)
                    {
                        if (Encoded_Value[pos] == '1')
                        {
                            bitmap.Mutate(x => x.DrawLines(pen, new Point(pos * iBarWidth + shiftAdjustment + halfBarWidth, topLabelAdjustment), new Point(pos * iBarWidth + shiftAdjustment + halfBarWidth, ILHeight + topLabelAdjustment)));
                        }

                        pos++;
                    }

                    //if (IncludeLabel)
                    //{
                    //    if ((AlternateLabel == null || RawData.StartsWith(AlternateLabel)) && _StandardizeLabel)
                    //    {
                    //        Labels.Label_UPCA(this, bitmap);
                    //    }
                    //    else
                    //    {
                    //        Labels.Label_Generic(this, bitmap);
                    //    }
                    //}

                    break;
                }

            case TYPE.EAN13:
                {
                    // Automatically calculate Width if applicable.
                    Width = BarWidth * Encoded_Value.Length ?? Width;

                    // Automatically calculate Height if applicable.
                    Height = (int?)(Width / AspectRatio) ?? Height;

                    int ILHeight = Height;
                    int topLabelAdjustment = 0;

                    int shiftAdjustment = 0;

                    //set alignment
                    switch (Alignment)
                    {
                        case AlignmentPositions.LEFT:
                            shiftAdjustment = 0;
                            break;
                        case AlignmentPositions.RIGHT:
                            shiftAdjustment = (Width % Encoded_Value.Length);
                            break;
                        case AlignmentPositions.CENTER:
                        default:
                            shiftAdjustment = (Width % Encoded_Value.Length) / 2;
                            break;
                    }

                    //if (IncludeLabel)
                    //{
                    //    if (((AlternateLabel == null) || RawData.StartsWith(AlternateLabel)) && _StandardizeLabel)
                    //    {
                    //        // EAN13 standardized label
                    //        string defTxt = RawData;
                    //        string labTxt = defTxt.Substring(0, 1) + "--" + defTxt.Substring(1, 6) + "--" + defTxt.Substring(7);

                    //        Font font = this.LabelFont;
                    //        Font labFont = new Font(font != null ? font.FontFamily.Name : "Arial", Labels.getFontsize(this, Width, Height, labTxt) * DotsPerPointAt96Dpi, FontStyle.Regular, GraphicsUnit.Pixel);

                    //        if (font != null)
                    //        {
                    //            this.LabelFont.Dispose();
                    //        }

                    //        LabelFont = labFont;

                    //        ILHeight -= (labFont.Height / 2);
                    //    }
                    //    else
                    //    {
                    //        // Shift drawing down if top label.
                    //        if ((LabelPosition & (LabelPositions.TOPCENTER | LabelPositions.TOPLEFT | LabelPositions.TOPRIGHT)) > 0)
                    //            topLabelAdjustment = this.LabelFont.Height;

                    //        ILHeight -= this.LabelFont.Height;
                    //    }
                    //}

                    bitmap = CreateBitmap(Width, Height);
                    int iBarWidth = Width / Encoded_Value.Length;
                    int iBarWidthModifier = 1;
                    if (iBarWidth <= 0)
                        throw new Exception("EGENERATE_IMAGE-2: Image size specified not large enough to draw image. (Bar size determined to be less than 1 pixel)");

                    // draw image
                    int pos = 0;
                    int halfBarWidth = (int)(iBarWidth * 0.5);

                    // lines are fBarWidth wide so draw the appropriate color line vertically
                    IBrush brush = new SolidBrush(ForeColor);
                    Pen pen = new Pen(brush, iBarWidth / iBarWidthModifier);

                    while (pos < Encoded_Value.Length)
                    {
                        if (Encoded_Value[pos] == '1')
                        {
                            bitmap.Mutate(x => x.DrawLines(pen, new Point(pos * iBarWidth + shiftAdjustment + halfBarWidth, topLabelAdjustment), new Point(pos * iBarWidth + shiftAdjustment + halfBarWidth, ILHeight + topLabelAdjustment)));
                        }

                        pos++;
                    }

                    //if (IncludeLabel)
                    //{
                    //    if (((AlternateLabel == null) || RawData.StartsWith(AlternateLabel)) && _StandardizeLabel)
                    //    {
                    //        Labels.Label_EAN13(this, bitmap);
                    //    }
                    //    else
                    //    {
                    //        Labels.Label_Generic(this, bitmap);
                    //    }
                    //}

                    break;
                }

            default:
                {
                    // Automatically calculate Width if applicable.
                    Width = BarWidth * Encoded_Value.Length ?? Width;

                    // Automatically calculate Height if applicable.
                    Height = (int?)(Width / AspectRatio) ?? Height;

                    int ILHeight = Height;
                    int topLabelAdjustment = 0;

                    //if (IncludeLabel)
                    //{
                    //    // Shift drawing down if top label.
                    //    if ((LabelPosition & (LabelPositions.TOPCENTER | LabelPositions.TOPLEFT | LabelPositions.TOPRIGHT)) > 0)
                    //        topLabelAdjustment = this.LabelFont.Height;

                    //    ILHeight -= this.LabelFont.Height;
                    //}


                    bitmap = CreateBitmap(Width, Height);
                    int iBarWidth = Width / Encoded_Value.Length;
                    iBarWidth += (iBarWidth % 2 == 0) ? 1 : 0;
                    int shiftAdjustment = 0;
                    int iBarWidthModifier = 1;

                    if (this.EncodingType == TYPE.PostNet)
                        iBarWidthModifier = 2;

                    // set alignment
                    switch (Alignment)
                    {
                        case AlignmentPositions.LEFT:
                            shiftAdjustment = 0;
                            break;
                        case AlignmentPositions.RIGHT:
                            shiftAdjustment = (Width % Encoded_Value.Length);
                            break;
                        case AlignmentPositions.CENTER:
                        default:
                            shiftAdjustment = (Width % Encoded_Value.Length) / 2;
                            break;
                    }

                    if (iBarWidth <= 0)
                        throw new Exception("EGENERATE_IMAGE-2: Image size specified not large enough to draw image. (Bar size determined to be less than 1 pixel)");

                    // draw image
                    int pos = 0;
                    int halfBarWidth = (int)Math.Round(iBarWidth * 0.5);

                    // fill background
                    bitmap.Mutate(x => x.Fill(BackColor));

                    // lines are fBarWidth wide so draw the appropriate color line vertically
                    IBrush brush = new SolidBrush(ForeColor);
                    Pen pen = new Pen(brush, iBarWidth / iBarWidthModifier);

                    while (pos < Encoded_Value.Length)
                    {
                        if (this.EncodingType == TYPE.PostNet)
                        {
                            // draw half bars in postnet
                            if (Encoded_Value[pos] == '0')
                                bitmap.Mutate(x => x.DrawLines(pen, new Point(pos * iBarWidth + shiftAdjustment + halfBarWidth, ILHeight + topLabelAdjustment), new Point(pos * iBarWidth + shiftAdjustment + halfBarWidth, (ILHeight / 2) + topLabelAdjustment)));
                            else
                                bitmap.Mutate(x => x.DrawLines(pen, new Point(pos * iBarWidth + shiftAdjustment + halfBarWidth, ILHeight + topLabelAdjustment), new Point(pos * iBarWidth + shiftAdjustment + halfBarWidth, topLabelAdjustment)));
                        }
                        else
                        {
                            if (Encoded_Value[pos] == '1')
                            {
                                bitmap.Mutate(x => x.DrawLines(pen, new Point(pos * iBarWidth + shiftAdjustment + halfBarWidth, topLabelAdjustment), new Point(pos * iBarWidth + shiftAdjustment + halfBarWidth, ILHeight + topLabelAdjustment)));
                            }
                        }

                        pos++;
                    }
                }

                //if (IncludeLabel)
                //{
                //    Labels.Label_Generic(this, bitmap);
                //}

                break;
        }

        _Encoded_Image = bitmap;
        return bitmap;
    }

    /// <summary>
    /// Gets the bytes that represent the image.
    /// </summary>
    /// <param name="savetype">File type to put the data in before returning the bytes.</param>
    /// <returns>Bytes representing the encoded image.</returns>
    public byte[] GetImageData(SaveTypes savetype)
    {
        byte[] imageData = null;

        try
        {
            if (_Encoded_Image != null)
            {
                //Save the image to a memory stream so that we can get a byte array!      
                using (MemoryStream ms = new MemoryStream())
                {
                    SaveImage(ms, savetype);
                    imageData = ms.ToArray();
                    ms.Flush();
                    ms.Close();
                }
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
    /// <param name="Filename">Filename to save to.</param>
    /// <param name="FileType">Format to use.</param>
    public void SaveImage(string Filename, SaveTypes FileType)
    {
        try
        {
            if (_Encoded_Image != null)
            {
                switch (FileType)
                {
                    case SaveTypes.BMP: _Encoded_Image.SaveAsBmp(Filename); break;
                    case SaveTypes.GIF: _Encoded_Image.SaveAsGif(Filename); break;
                    case SaveTypes.JPG: _Encoded_Image.SaveAsJpeg(Filename); break;
                    case SaveTypes.PNG: _Encoded_Image.SaveAsPng(Filename); break;
                }
            }
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
    /// <param name="FileType">Format to use.</param>
    public void SaveImage(Stream stream, SaveTypes FileType)
    {
        try
        {
            if (_Encoded_Image != null)
            {
                switch (FileType)
                {
                    case SaveTypes.BMP: _Encoded_Image.SaveAsBmp(stream); break;
                    case SaveTypes.GIF: _Encoded_Image.SaveAsGif(stream); break;
                    case SaveTypes.JPG: _Encoded_Image.SaveAsJpeg(stream); break;
                    case SaveTypes.PNG: _Encoded_Image.SaveAsPng(stream); break;
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception("ESAVEIMAGE-2: Could not save image.\n\n=======================\n\n" + ex.Message);
        }
    }

    /// <summary>
    /// Returns the size of the EncodedImage in real world coordinates (millimeters or inches).
    /// </summary>
    /// <param name="Metric">Millimeters if true, otherwise Inches.</param>
    /// <returns></returns>
    public ImageSize GetSizeOfImage(bool Metric)
    {
        double Width = 0;
        double Height = 0;
        if (this.EncodedImage != null && this.EncodedImage.Width > 0 && this.EncodedImage.Height > 0)
        {
            double MillimetersPerInch = 25.4;

            Width = this.EncodedImage.Width;
            Height = this.EncodedImage.Height;

            if (Metric)
            {
                Width *= MillimetersPerInch;
                Height *= MillimetersPerInch;
            }

        }

        return new ImageSize(Width, Height, Metric);
    }

    #endregion

    #region XML Methods

    private SaveData GetSaveData(Boolean includeImage = true)
    {
        SaveData saveData = new SaveData();
        saveData.Type = EncodedType.ToString();
        saveData.RawData = RawData;
        saveData.EncodedValue = EncodedValue;
        saveData.IncludeLabel = IncludeLabel;
        //saveData.Forecolor = ColorTranslator.ToHtml(ForeColor);
        //saveData.Backcolor = ColorTranslator.ToHtml(BackColor);
        saveData.CountryAssigningManufacturingCode = Country_Assigning_Manufacturer_Code;
        saveData.ImageWidth = Width;
        saveData.ImageHeight = Height;
        saveData.LabelPosition = (int)LabelPosition;
        //saveData.LabelFont = LabelFont.ToString();
        //saveData.ImageFormat = ImageFormat.ToString();
        saveData.Alignment = (int)Alignment;

        //get image in base 64
        if (includeImage)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                EncodedImage.SaveAsBmp(ms);
                saveData.Image = Convert.ToBase64String(ms.ToArray(), Base64FormattingOptions.None);
            }
        }

        return saveData;
    }

    public string ToJSON(Boolean includeImage = true)
    {
        byte[] bytes = JsonSerializer.SerializeToUtf8Bytes(GetSaveData(includeImage));
        return (new UTF8Encoding(false)).GetString(bytes);
    }

    public string ToXML(Boolean includeImage = true)
    {
        if (EncodedValue == "")
            throw new Exception("EGETXML-1: Could not retrieve XML due to the barcode not being encoded first.  Please call Encode first.");
        else
        {
            try
            {
                SaveData xml = GetSaveData(includeImage);

                XmlSerializer writer = new XmlSerializer(typeof(SaveData));
                using (Utf8StringWriter sw = new Utf8StringWriter())
                {
                    writer.Serialize(sw, xml);
                    return sw.ToString();
                }

            }
            catch (Exception ex)
            {
                throw new Exception("EGETXML-2: " + ex.Message);
            }
        }
    }

    public static SaveData FromJSON(Stream jsonStream)
    {
        using (jsonStream)
        {
            if (jsonStream is MemoryStream)
            {
                return JsonSerializer.Deserialize<SaveData>(((MemoryStream)jsonStream).ToArray());
            }
            else
            {
                using (var memoryStream = new MemoryStream())
                {
                    jsonStream.CopyTo(memoryStream);
                    return JsonSerializer.Deserialize<SaveData>(memoryStream.ToArray());
                }
            }

        }
    }

    public static SaveData FromXML(Stream xmlStream)
    {
        try
        {
            XmlSerializer serializer = new XmlSerializer(typeof(SaveData));
            using (XmlReader reader = XmlReader.Create(xmlStream))
            {
                return (SaveData)serializer.Deserialize(reader);
            }
        }
        catch (Exception ex)
        {
            throw new Exception("EGETIMAGEFROMXML-1: " + ex.Message);
        }
    }
    public static Image GetImageFromSaveData(SaveData saveData)
    {
        try
        {
            //loading it to memory stream and then to image object
            using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(saveData.Image)))
            {
                return Image.Load(ms);
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
    /// <param name="iType">Type of encoding to use.</param>
    /// <param name="Data">Raw data to encode.</param>
    /// <returns>Image representing the barcode.</returns>
    public static Image DoEncode(TYPE iType, string Data)
    {
        using (Barcode b = new Barcode())
        {
            return b.Encode(iType, Data);
        }
    }

    /// <summary>
    /// Encodes the raw data into binary form representing bars and spaces.  Also generates an Image of the barcode.
    /// </summary>
    /// <param name="iType">Type of encoding to use.</param>
    /// <param name="Data">Raw data to encode.</param>
    /// <param name="XML">XML representation of the data and the image of the barcode.</param>
    /// <returns>Image representing the barcode.</returns>
    public static Image DoEncode(TYPE iType, string Data, ref string XML)
    {
        using (Barcode b = new Barcode())
        {
            Image i = b.Encode(iType, Data);
            XML = b.ToXML();
            return i;
        }
    }

    /// <summary>
    /// Encodes the raw data into binary form representing bars and spaces.  Also generates an Image of the barcode.
    /// </summary>
    /// <param name="iType">Type of encoding to use.</param>
    /// <param name="Data">Raw data to encode.</param>
    /// <param name="IncludeLabel">Include the label at the bottom of the image with data encoded.</param>
    /// <returns>Image representing the barcode.</returns>
    public static Image DoEncode(TYPE iType, string Data, bool IncludeLabel)
    {
        using (Barcode b = new Barcode())
        {
            b.IncludeLabel = IncludeLabel;
            return b.Encode(iType, Data);
        }
    }

    /// <summary>
    /// Encodes the raw data into binary form representing bars and spaces.  Also generates an Image of the barcode.
    /// </summary>
    /// <param name="iType">Type of encoding to use.</param>
    /// <param name="data">Raw data to encode.</param>
    /// <param name="IncludeLabel">Include the label at the bottom of the image with data encoded.</param>
    /// <param name="Width">Width of the resulting barcode.(pixels)</param>
    /// <param name="Height">Height of the resulting barcode.(pixels)</param>
    /// <returns>Image representing the barcode.</returns>
    public static Image DoEncode(TYPE iType, string Data, bool IncludeLabel, int Width, int Height)
    {
        using (Barcode b = new Barcode())
        {
            b.IncludeLabel = IncludeLabel;
            return b.Encode(iType, Data, Width, Height);
        }
    }

    /// <summary>
    /// Encodes the raw data into binary form representing bars and spaces.  Also generates an Image of the barcode.
    /// </summary>
    /// <param name="iType">Type of encoding to use.</param>
    /// <param name="Data">Raw data to encode.</param>
    /// <param name="IncludeLabel">Include the label at the bottom of the image with data encoded.</param>
    /// <param name="DrawColor">Foreground color</param>
    /// <param name="BackColor">Background color</param>
    /// <returns>Image representing the barcode.</returns>
    public static Image DoEncode(TYPE iType, string Data, bool IncludeLabel, Color DrawColor, Color BackColor)
    {
        using (Barcode b = new Barcode())
        {
            b.IncludeLabel = IncludeLabel;
            return b.Encode(iType, Data, DrawColor, BackColor);
        }
    }

    /// <summary>
    /// Encodes the raw data into binary form representing bars and spaces.  Also generates an Image of the barcode.
    /// </summary>
    /// <param name="iType">Type of encoding to use.</param>
    /// <param name="Data">Raw data to encode.</param>
    /// <param name="IncludeLabel">Include the label at the bottom of the image with data encoded.</param>
    /// <param name="DrawColor">Foreground color</param>
    /// <param name="BackColor">Background color</param>
    /// <param name="Width">Width of the resulting barcode.(pixels)</param>
    /// <param name="Height">Height of the resulting barcode.(pixels)</param>
    /// <returns>Image representing the barcode.</returns>
    public static Image DoEncode(TYPE iType, string Data, bool IncludeLabel, Color DrawColor, Color BackColor, int Width, int Height)
    {
        using (Barcode b = new Barcode())
        {
            b.IncludeLabel = IncludeLabel;
            return b.Encode(iType, Data, DrawColor, BackColor, Width, Height);
        }
    }

    /// <summary>
    /// Encodes the raw data into binary form representing bars and spaces.  Also generates an Image of the barcode.
    /// </summary>
    /// <param name="iType">Type of encoding to use.</param>
    /// <param name="Data">Raw data to encode.</param>
    /// <param name="IncludeLabel">Include the label at the bottom of the image with data encoded.</param>
    /// <param name="DrawColor">Foreground color</param>
    /// <param name="BackColor">Background color</param>
    /// <param name="Width">Width of the resulting barcode.(pixels)</param>
    /// <param name="Height">Height of the resulting barcode.(pixels)</param>
    /// <param name="XML">XML representation of the data and the image of the barcode.</param>
    /// <returns>Image representing the barcode.</returns>
    public static Image DoEncode(TYPE iType, string Data, bool IncludeLabel, Color DrawColor, Color BackColor, int Width, int Height, ref string XML)
    {
        using (Barcode b = new Barcode())
        {
            b.IncludeLabel = IncludeLabel;
            Image i = b.Encode(iType, Data, DrawColor, BackColor, Width, Height);
            XML = b.ToXML();
            return i;
        }
    }

    #region IDisposable Support
    private bool disposedValue = false; // To detect redundant calls

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects).
            }

            // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
            // TODO: set large fields to null.

            disposedValue = true;
            //LabelFont?.Dispose();
            //LabelFont = null;

            _Encoded_Image?.Dispose();
            _Encoded_Image = null;

            RawData = null;
            Encoded_Value = null;
            _Country_Assigning_Manufacturer_Code = null;
            //_ImageFormat = null;
        }
    }

    // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
    // ~Barcode() {
    //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
    //   Dispose(false);
    // }

    // This code added to correctly implement the disposable pattern.
    void IDisposable.Dispose()
    {
        // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        Dispose(true);
        // TODO: uncomment the following line if the finalizer is overridden above.
        // GC.SuppressFinalize(this);
    }
    #endregion

    #endregion
}
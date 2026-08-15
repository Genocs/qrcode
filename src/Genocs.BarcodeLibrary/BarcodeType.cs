namespace Genocs.BarcodeLibrary;

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
    /// The Industrial 2 of 5 barcode is a numeric-only barcode symbology
    /// that encodes pairs of digits using a combination of bars and spaces.
    /// It is commonly used in industrial applications for inventory management,
    /// shipping, and tracking purposes.
    /// The barcode consists of a series of vertical bars and spaces that
    /// represent the digits 0-9, with each digit being represented by a
    /// unique pattern of bars and spaces.
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

    /// <summary>
    /// MSI barcode type.
    /// </summary>
    MsiMod10,

    /// <summary>
    /// MSI with 2 Mod 10 Checksum barcode type.
    /// </summary>
    Msi2Mod10,

    /// <summary>
    /// MSI with Mod 11 Checksum barcode type.
    /// </summary>
    MsiMod11,

    /// <summary>
    /// MSI with Mod 11 and Mod 10 Checksum barcode type.
    /// </summary>
    MsiMod11Mod10,

    /// <summary>
    /// Modified Plessey barcode type.
    /// </summary>
    ModifiedPlessey,

    /// <summary>
    /// Code 11 barcode type.
    /// </summary>
    Code11,

    /// <summary>
    /// USD-8 barcode type.
    /// </summary>
    Usd8,

    /// <summary>
    /// UCC-12 barcode type.
    /// </summary>
    Ucc12,

    /// <summary>
    /// UCC-13 barcode type.
    /// The UCC-13 barcode is a 13-digit barcode symbology used
    /// for product identification and tracking in the retail industry.
    /// It is based on the EAN-13 barcode standard and is used to encode
    ///  the Global Trade Item Number (GTIN) assigned to products
    /// by the Uniform Code Council (UCC).
    /// The UCC-13 barcode consists of a series of vertical bars and spaces
    /// that represent the digits 0-9, with each digit being represented
    /// by a unique pattern of bars and spaces.
    /// The UCC-13 barcode is widely used in retail applications
    /// for inventory management, point-of-sale scanning, and supply chain tracking.
    /// </summary>
    Ucc13,

    /// <summary>
    /// Logmars barcode type.
    /// The Logmars barcode is a high-density barcode symbology
    /// that can encode all 128 ASCII characters.
    /// </summary>
    Logmars,

    /// <summary>
    /// Code 128 barcode type.
    /// The Code 128 barcode is a high-density linear barcode symbology
    /// that can encode all 128 ASCII characters.
    /// </summary>
    Code128,

    /// <summary>
    /// Code 128A barcode type.
    /// </summary>
    Code128A,

    /// <summary>
    /// Code 128B barcode type.
    /// </summary>
    Code128B,

    /// <summary>
    /// Code 128C barcode type.
    /// </summary>
    Code128C,

    /// <summary>
    /// ITF-14 barcode type.
    /// The ITF-14 barcode is a 14-digit barcode symbology used for
    /// packaging and labeling of products.
    /// </summary>
    Itf14,

    /// <summary>
    /// Code 93 barcode type.
    /// The Code 93 barcode is a linear barcode symbology that can
    /// encode all 93 ASCII characters.
    /// </summary>
    Code93,

    /// <summary>
    /// Telepen barcode type.
    /// Telepen is a high-density barcode symbology that can encode all 128 ASCII characters.
    /// </summary>
    Telepen,

    /// <summary>
    /// FIM barcode type.
    /// The FIM (Facing Identification Mark) barcode is
    /// a barcode used by the United States Postal Service (USPS)
    /// to identify the type of mail and its processing requirements.
    /// It consists of a series of vertical bars and spaces that encode
    /// information about the mailpiece, such as its class, shape, and destination.
    /// </summary>
    Fim,

    /// <summary>
    /// Pharmacode barcode type.
    /// The Pharmacode is a barcode used in the pharmaceutical industry
    /// for packaging and labeling of medicines.
    /// It is a numeric-only barcode that can encode numbers from 3 to 131070.
    /// </summary>
    Pharmacode
}

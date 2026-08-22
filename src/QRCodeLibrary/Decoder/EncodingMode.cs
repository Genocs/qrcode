namespace Genocs.QRCodeLibrary.Decoder;

/// <summary>
/// QR Code encoding modes.
/// </summary>
public enum EncodingMode
{
    /// <summary>
    /// Terminator.
    /// </summary>
    Terminator,

    /// <summary>
    /// Numeric.
    /// </summary>
    Numeric,

    /// <summary>
    /// Alpha numeric.
    /// </summary>
    AlphaNumeric,

    /// <summary>
    /// Append.
    /// </summary>
    Append,

    /// <summary>
    /// byte encoding.
    /// </summary>
    Byte,

    /// <summary>
    /// FNC1 first.
    /// </summary>
    FNC1First,

    /// <summary>
    /// Unknown encoding constant.
    /// </summary>
    Unknown6,

    /// <summary>
    /// Extended Channel Interpretation (ECI) mode.
    /// </summary>
    ECI,

    /// <summary>
    /// Kanji encoding (not implemented by this software).
    /// </summary>
    Kanji,

    /// <summary>
    /// FNC1 second.
    /// </summary>
    FNC1Second,

    /// <summary>
    /// Unknown encoding constant.
    /// </summary>
    Unknown10,

    /// <summary>
    /// Unknown encoding constant.
    /// </summary>
    Unknown11,

    /// <summary>
    /// Unknown encoding constant.
    /// </summary>
    Unknown12,

    /// <summary>
    /// Unknown encoding constant.
    /// </summary>
    Unknown13,

    /// <summary>
    /// Unknown encoding constant.
    /// </summary>
    Unknown14,

    /// <summary>
    /// Unknown encoding constant.
    /// </summary>
    Unknown15,
}


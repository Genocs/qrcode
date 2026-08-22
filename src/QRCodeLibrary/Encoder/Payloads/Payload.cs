namespace Genocs.QRCodeLibrary.Encoder.Payloads;

/// <summary>
/// Represents a payload for QR code encoding.
/// </summary>
public abstract class Payload
{
    /// <summary>
    /// Gets the version of the QR code. Returns -1 for automatic version selection.
    /// </summary>
    public virtual int Version
    {
        get { return -1; }
    }

    /// <summary>
    /// Gets the error correction level of the QR code. Returns ECCLevel.M by default.
    /// </summary>
    public virtual QRCodeGenerator.ECCLevel EccLevel
    {
        get { return QRCodeGenerator.ECCLevel.M; }
    }

    /// <summary>
    /// Gets the ECI (Extended Channel Interpretation) mode of the QR code. Returns EciMode.Default by default.
    /// </summary>
    public virtual QRCodeGenerator.EciMode EciMode
    {
        get { return QRCodeGenerator.EciMode.Default; }
    }

    public abstract override string ToString();
}

namespace Genocs.QRCodeLibrary.Encoder.Payloads;

/// <summary>
/// Generates a phone call payload for QR code encoding.
/// </summary>
internal class PhoneNumber : Payload
{
    private readonly string _number;

    /// <summary>
    /// Generates a phone call payload.
    /// </summary>
    /// <param name="number">Phone Number of the receiver.</param>
    public PhoneNumber(string number)
    {
        _number = number;
    }

    public override string ToString()
    {
        return $"tel:{_number}";
    }
}

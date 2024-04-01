namespace Genocs.QRCodeGenerator.Exceptions;

public class DataTooLongException : Exception
{
    public DataTooLongException(string eccLevel, string encodingMode, int maxSizeByte)
        : base(
        $"The given payload exceeds the maximum size of the QR code standard. The maximum size allowed for the chosen parameters (ECC level={eccLevel}, EncodingMode={encodingMode}) is {maxSizeByte} byte."
    )
    {
    }
}

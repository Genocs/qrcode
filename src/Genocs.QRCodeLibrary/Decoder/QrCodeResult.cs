namespace Genocs.QRCodeGenerator.Decoder;

public class QrCodeResult
{
    public List<string> Results { get; set; }

    public QrCodeResult()
    {
        Results = new List<string>();
    }

    public QrCodeResult(List<string> items)
    {
        Results = items;
    }
}

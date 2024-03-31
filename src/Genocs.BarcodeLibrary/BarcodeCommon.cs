using System.Text.RegularExpressions;

namespace Genocs.BarcodeLibrary;

internal abstract class BarcodeCommon
{
    protected string _rawData = string.Empty;
    protected List<string> _errors = new();

    public string RawData
    {
        get { return _rawData; }
    }

    public List<string> Errors
    {
        get { return _errors; }
    }

    public void Error(string errorMessage)
    {
        _errors.Add(errorMessage);
        throw new Exception(errorMessage);
    }

    internal static bool CheckNumericOnly(string data)
    {
        return Regex.IsMatch(data, @"^\d+$", RegexOptions.Compiled);
    }

    internal static int GetAlignmentShiftAdjustment(Barcode barcode)
    {
        switch (barcode.Alignment)
        {
            case AlignmentPositions.Left:
                return 0;
            case AlignmentPositions.Right:
                return barcode.Width % barcode.EncodedValue.Length;
            case AlignmentPositions.Center:
            default:
                return (barcode.Width % barcode.EncodedValue.Length) / 2;
        }
    }
}

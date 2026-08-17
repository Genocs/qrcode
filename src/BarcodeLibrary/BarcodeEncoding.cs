using System.Text.RegularExpressions;

namespace Genocs.BarcodeLibrary;

/// <summary>
/// Represents the base class for barcode symbologies, providing common functionality and properties for barcode encoding.
/// </summary>
internal abstract class BarcodeEncoding
{
    /// <summary>
    /// Gets the raw data to be encoded in the barcode.
    /// </summary>
    public string RawData { get; protected set; } = string.Empty;

    /// <summary>
    /// Gets the list of error messages encountered during barcode encoding.
    /// </summary>
    public List<string> Errors { get; } = new List<string>();

    public void Error(string errorMessage)
    {
        Errors.Add(errorMessage);
        throw new Exception(errorMessage);
    }

    /// <summary>
    /// Encodes the raw data into a barcode representation.
    /// This method must be implemented by derived classes to provide specific encoding logic for different barcode symbologies.
    /// </summary>
    /// <returns>The encoded barcode value.</returns>
    protected abstract string Encode();

    public string EncodedValue => Encode();

    /// <summary>
    /// Checks if the provided data contains only numeric characters.
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    internal static bool CheckNumericOnly(string data)
    {
        return Regex.IsMatch(data, @"^\d+$", RegexOptions.Compiled);
    }

    /// <summary>
    /// Calculates the alignment shift adjustment based on the barcode's alignment position and width.
    /// </summary>
    /// <param name="barcode">The barcode for which to calculate the alignment shift adjustment.</param>
    /// <returns>The alignment shift adjustment.</returns>
    internal static int GetAlignmentShiftAdjustment(Barcode barcode)
    {
        return barcode.Alignment switch
        {
            AlignmentPositions.Left => 0,
            AlignmentPositions.Right => barcode.Width % barcode.EncodedValue.Length,
            _ => (barcode.Width % barcode.EncodedValue.Length) / 2,
        };
    }
}

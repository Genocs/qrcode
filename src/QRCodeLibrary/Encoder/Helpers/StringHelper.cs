using System.Text;
using System.Text.RegularExpressions;

namespace Genocs.QRCodeLibrary.Encoder.Helpers;

internal static class StringHelper
{

    /// <summary>
    /// Reverses a string.
    /// </summary>
    /// <param name="input">The string to reverse.</param>
    /// <returns>The reversed string.</returns>
    public static string ReverseString(string input)
    {
        char[] chars = input.ToCharArray();
        char[] result = new char[chars.Length];
        for (int i = 0, j = input.Length - 1; i < input.Length; i++, j--)
        {
            result[i] = chars[j];
        }

        return new string(result);
    }

    /// <summary>
    /// Validates an IBAN (International Bank Account Number) for structural correctness and checksum validity.
    /// </summary>
    /// <param name="iban">The IBAN to validate.</param>
    /// <returns>true if the IBAN is valid; otherwise, false.</returns>
    public static bool IsValidIban(string iban)
    {
        // Clean IBAN
        string ibanCleared = iban.ToUpper().Replace(" ", string.Empty).Replace("-", string.Empty);

        // Check for general structure
        bool structurallyValid = Regex.IsMatch(ibanCleared, @"^[a-zA-Z]{2}[0-9]{2}([a-zA-Z0-9]?){16,30}$");

        // Check IBAN checksum
        string sum = $"{ibanCleared.Substring(4)}{ibanCleared.Substring(0, 4)}".ToCharArray().Aggregate(string.Empty, (current, c) => current + (char.IsLetter(c) ? (c - 55).ToString() : c.ToString()));

        if (!decimal.TryParse(sum, out decimal sumDec))
            return false;

        bool checksumValid = sumDec % 97 == 1;

        return structurallyValid && checksumValid;
    }

    /// <summary>
    /// Validates a QR IBAN (International Bank Account Number) for structural correctness, checksum validity, and specific QR IBAN range.
    /// </summary>
    /// <param name="iban">The IBAN to validate.</param>
    /// <returns>true if the IBAN is valid; otherwise, false.</returns>
    public static bool IsValidQRIban(string iban)
    {
        if (string.IsNullOrWhiteSpace(iban))
            return false;

        bool foundQrIid = false;

        try
        {
            string ibanCleared = iban.ToUpper().Replace(" ", string.Empty).Replace("-", string.Empty);
            int possibleQrIid = Convert.ToInt32(ibanCleared.Substring(4, 5));
            foundQrIid = possibleQrIid >= 30000 && possibleQrIid <= 31999;
        }
        catch { }

        return IsValidIban(iban) && foundQrIid;
    }

    /// <summary>
    /// Validates a BIC (Bank Identifier Code) for structural correctness.
    /// </summary>
    /// <param name="bic">The BIC to validate.</param>
    /// <returns>true if the BIC is valid; otherwise, false.</returns>
    public static bool IsValidBic(string bic)
    {
        return Regex.IsMatch(bic.Replace(" ", string.Empty), @"^([a-zA-Z]{4}[a-zA-Z]{2}[a-zA-Z0-9]{2}([a-zA-Z0-9]{3})?)$");
    }

    public static string ConvertStringToEncoding(string message, string encoding)
    {
        var iso = Encoding.GetEncoding(encoding);
        var utf8 = Encoding.UTF8;
        byte[] utfBytes = utf8.GetBytes(message);
        byte[] isoBytes = Encoding.Convert(utf8, iso, utfBytes);
        return iso.GetString(isoBytes, 0, isoBytes.Length);
    }
}
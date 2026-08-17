using System.Text.RegularExpressions;

namespace Genocs.QRCodeLibrary.Encoder.Helpers;

internal static class PayloadHelper
{
    internal static bool IsHexStyle(string inp)
    {
        return Regex.IsMatch(inp, @"\A\b[0-9a-fA-F]+\b\Z") || Regex.IsMatch(inp, @"\A\b(0[xX])?[0-9a-fA-F]+\b\Z");
    }

    internal static string EscapeInput(string input, bool simple = false)
    {
        char[] forbiddenChars = { '\\', ';', ',', ':' };
        if (simple)
        {
            forbiddenChars = new char[1] { ':' };
        }

        foreach (char c in forbiddenChars)
        {
            input = input.Replace(c.ToString(), "\\" + c);
        }

        return input;
    }
}

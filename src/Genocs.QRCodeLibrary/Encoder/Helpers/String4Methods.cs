namespace Genocs.QRCodeGenerator.Encoder.Helpers;

internal static class String40Methods
{
    public static string ReverseString(string str)
    {
        char[] chars = str.ToCharArray();
        char[] result = new char[chars.Length];
        for (int i = 0, j = str.Length - 1; i < str.Length; i++, j--)
        {
            result[i] = chars[j];
        }
        return new string(result);
    }
}
using System.Text.RegularExpressions;

namespace Genocs.BarcodeLibrary;

abstract class BarcodeCommon
{
    protected string _RawData = "";
    protected List<string> _Errors = new List<string>();

    public string RawData
    {
        get { return _RawData; }
    }

    public List<string> Errors
    {
        get { return this._Errors; }
    }

    public void Error(string errorMessage)
    {
        this._Errors.Add(errorMessage);
        throw new Exception(errorMessage);
    }

    internal static bool CheckNumericOnly(string data)
    {
        return Regex.IsMatch(data, @"^\d+$", RegexOptions.Compiled);
    }
}

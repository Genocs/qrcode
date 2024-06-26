﻿namespace Genocs.BarcodeLibrary.Symbologies;

/// <summary>
///  FIM encoding
///  Written by: Brad Barnhill.
/// </summary>
internal class FIM : BarcodeCommon, IBarcode
{
    private readonly string[] FIM_Codes = { "110010011", "101101101", "110101011", "111010111", "101000101" };
    public enum FIMTypes { FIM_A = 0, FIM_B, FIM_C, FIM_D, FIM_E };

    public FIM(string input)
    {
        input = input.Trim();

        switch (input)
        {
            case "A":
            case "a":
                _rawData = FIM_Codes[(int)FIMTypes.FIM_A];
                break;
            case "B":
            case "b":
                _rawData = FIM_Codes[(int)FIMTypes.FIM_B];
                break;
            case "C":
            case "c":
                _rawData = FIM_Codes[(int)FIMTypes.FIM_C];
                break;
            case "D":
            case "d":
                _rawData = FIM_Codes[(int)FIMTypes.FIM_D];
                break;
            case "E":
            case "e":
                _rawData = FIM_Codes[(int)FIMTypes.FIM_E];
                break;
            default:
                Error("EFIM-1: Could not determine encoding type. (Only pass in A, B, C, D, or E)");
                break;
        }
    }

    public string Encode_FIM()
    {
        string encoded = "";
        foreach (char c in RawData)
        {
            encoded += c + "0";
        }

        encoded = encoded.Substring(0, encoded.Length - 1);

        return encoded;
    }

    public string EncodedValue
        => Encode_FIM();
}

using System.Collections;

namespace Genocs.BarcodeLibrary.Symbologies;

/// <summary>
///  Telepen encoding.
/// </summary>
internal class Telepen : BarcodeEncoding, IBarcode
{
    private static readonly Hashtable TelepenCodes = new();

    private enum StartStopCode : int
    {
        START1,
        STOP1,
        START2,
        STOP2,
        START3,
        STOP3
    }

    private StartStopCode _startCode = StartStopCode.START1;
    private StartStopCode _stopCode = StartStopCode.STOP1;
    private int _switchModeIndex;
    private int _iCheckSum;

    /// <summary>
    /// Encodes data using the Telepen algorithm.
    /// </summary>
    /// <param name="input"></param>
    public Telepen(string input)
    {
        RawData = input;
        Init();
    }

    /// <summary>
    /// Encode the raw data using the Telepen algorithm.
    /// </summary>
    protected override string Encode()
    {
        _iCheckSum = 0;

        SetEncodingSequence();

        // Include the Start sequence pattern
        string result = TelepenCodes[_startCode].ToString();

        switch (_startCode)
        {
            // Numeric --> Ascii
            case StartStopCode.START2:
                EncodeNumeric(RawData.Substring(0, _switchModeIndex), ref result);

                if (_switchModeIndex < RawData.Length)
                {
                    EncodeSwitchMode(ref result);
                    EncodeASCII(RawData.Substring(_switchModeIndex), ref result);
                }

                break;

            // Ascii --> Numeric
            case StartStopCode.START3:
                EncodeASCII(RawData.Substring(0, _switchModeIndex), ref result);
                EncodeSwitchMode(ref result);
                EncodeNumeric(RawData.Substring(_switchModeIndex), ref result);
                break;

            // Full Ascii
            default:
                EncodeASCII(RawData, ref result);
                break;
        }

        // Checksum
        result += TelepenCodes[CalculateChecksum(_iCheckSum)];

        // Stop character
        result += TelepenCodes[_stopCode];

        return result;
    }

    private void EncodeASCII(string input, ref string output)
    {
        try
        {
            foreach (char c in input)
            {
                output += TelepenCodes[c];
                _iCheckSum += Convert.ToInt32(c);
            }
        }
        catch
        {
            Error("ETELEPEN-1: Invalid data when encoding ASCII");
        }
    }

    private void EncodeNumeric(string input, ref string output)
    {
        try
        {
            if ((input.Length % 2) > 0)
            {
                Error("ETELEPEN-3: Numeric encoding attempted on odd number of characters");
            }

            for (int i = 0; i < input.Length; i += 2)
            {
                output += TelepenCodes[Convert.ToChar(int.Parse(input.Substring(i, 2)) + 27)];
                _iCheckSum += int.Parse(input.Substring(i, 2)) + 27;
            }
        }
        catch
        {
            Error("ETELEPEN-2: Numeric encoding failed");
        }
    }

    private void EncodeSwitchMode(ref string output)
    {
        // ASCII code DLE is used to switch modes
        _iCheckSum += 16;
        output += TelepenCodes[Convert.ToChar(16)];
    }

    private static char CalculateChecksum(int iCheckSum)
    {
        return Convert.ToChar(127 - (iCheckSum % 127));
    }

    private void SetEncodingSequence()
    {
        // reset to full ascii
        _startCode = StartStopCode.START1;
        _stopCode = StartStopCode.STOP1;
        _switchModeIndex = RawData.Length;

        // starting number of 'numbers'
        int startNumerics = 0;
        foreach (char c in RawData)
        {
            if (char.IsNumber(c))
            {
                startNumerics++;
            }
            else
            {
                break;
            }
        }

        if (startNumerics == RawData.Length)
        {
            // Numeric only mode due to only numbers being present
            _startCode = StartStopCode.START2;
            _stopCode = StartStopCode.STOP2;

            if ((RawData.Length % 2) > 0)
                _switchModeIndex = RawData.Length - 1;
        }
        else
        {
            // Ending number of numbers
            int endNumerics = 0;
            for (int i = RawData.Length - 1; i >= 0; i--)
            {
                if (char.IsNumber(RawData[i]))
                    endNumerics++;
                else
                    break;
            }

            if (startNumerics >= 4 || endNumerics >= 4)
            {
                // hybrid mode will be used
                if (startNumerics > endNumerics)
                {
                    // start in numeric switching to ascii
                    _startCode = StartStopCode.START2;
                    _stopCode = StartStopCode.STOP2;
                    _switchModeIndex = (startNumerics % 2) == 1 ? startNumerics - 1 : startNumerics;
                }
                else
                {
                    // start in ascii switching to numeric
                    _startCode = StartStopCode.START3;
                    _stopCode = StartStopCode.STOP3;
                    _switchModeIndex = (endNumerics % 2) == 1 ? RawData.Length - endNumerics + 1 : RawData.Length - endNumerics;
                }
            }
        }
    }

    private static void Init()
    {
        if (TelepenCodes.Count > 0)
            return;

        TelepenCodes.Add(Convert.ToChar(0), "1110111011101110");
        TelepenCodes.Add(Convert.ToChar(1), "1011101110111010");
        TelepenCodes.Add(Convert.ToChar(2), "1110001110111010");
        TelepenCodes.Add(Convert.ToChar(3), "1010111011101110");
        TelepenCodes.Add(Convert.ToChar(4), "1110101110111010");
        TelepenCodes.Add(Convert.ToChar(5), "1011100011101110");
        TelepenCodes.Add(Convert.ToChar(6), "1000100011101110");
        TelepenCodes.Add(Convert.ToChar(7), "1010101110111010");
        TelepenCodes.Add(Convert.ToChar(8), "1110111000111010");
        TelepenCodes.Add(Convert.ToChar(9), "1011101011101110");
        TelepenCodes.Add(Convert.ToChar(10), "1110001011101110");
        TelepenCodes.Add(Convert.ToChar(11), "1010111000111010");
        TelepenCodes.Add(Convert.ToChar(12), "1110101011101110");
        TelepenCodes.Add(Convert.ToChar(13), "1010001000111010");
        TelepenCodes.Add(Convert.ToChar(14), "1000101000111010");
        TelepenCodes.Add(Convert.ToChar(15), "1010101011101110");
        TelepenCodes.Add(Convert.ToChar(16), "1110111010111010");
        TelepenCodes.Add(Convert.ToChar(17), "1011101110001110");
        TelepenCodes.Add(Convert.ToChar(18), "1110001110001110");
        TelepenCodes.Add(Convert.ToChar(19), "1010111010111010");
        TelepenCodes.Add(Convert.ToChar(20), "1110101110001110");
        TelepenCodes.Add(Convert.ToChar(21), "1011100010111010");
        TelepenCodes.Add(Convert.ToChar(22), "1000100010111010");
        TelepenCodes.Add(Convert.ToChar(23), "1010101110001110");
        TelepenCodes.Add(Convert.ToChar(24), "1110100010001110");
        TelepenCodes.Add(Convert.ToChar(25), "1011101010111010");
        TelepenCodes.Add(Convert.ToChar(26), "1110001010111010");
        TelepenCodes.Add(Convert.ToChar(27), "1010100010001110");
        TelepenCodes.Add(Convert.ToChar(28), "1110101010111010");
        TelepenCodes.Add(Convert.ToChar(29), "1010001010001110");
        TelepenCodes.Add(Convert.ToChar(30), "1000101010001110");
        TelepenCodes.Add(Convert.ToChar(31), "1010101010111010");
        TelepenCodes.Add(' ', "1110111011100010");
        TelepenCodes.Add('!', "1011101110101110");
        TelepenCodes.Add('"', "1110001110101110");
        TelepenCodes.Add('#', "1010111011100010");
        TelepenCodes.Add('$', "1110101110101110");
        TelepenCodes.Add('%', "1011100011100010");
        TelepenCodes.Add('&', "1000100011100010");
        TelepenCodes.Add('\'', "1010101110101110");
        TelepenCodes.Add('(', "1110111000101110");
        TelepenCodes.Add(')', "1011101011100010");
        TelepenCodes.Add('*', "1110001011100010");
        TelepenCodes.Add('+', "1010111000101110");
        TelepenCodes.Add(',', "1110101011100010");
        TelepenCodes.Add('-', "1010001000101110");
        TelepenCodes.Add('.', "1000101000101110");
        TelepenCodes.Add('/', "1010101011100010");
        TelepenCodes.Add('0', "1110111010101110");
        TelepenCodes.Add('1', "1011101000100010");
        TelepenCodes.Add('2', "1110001000100010");
        TelepenCodes.Add('3', "1010111010101110");
        TelepenCodes.Add('4', "1110101000100010");
        TelepenCodes.Add('5', "1011100010101110");
        TelepenCodes.Add('6', "1000100010101110");
        TelepenCodes.Add('7', "1010101000100010");
        TelepenCodes.Add('8', "1110100010100010");
        TelepenCodes.Add('9', "1011101010101110");
        TelepenCodes.Add(':', "1110001010101110");
        TelepenCodes.Add(';', "1010100010100010");
        TelepenCodes.Add('<', "1110101010101110");
        TelepenCodes.Add('=', "1010001010100010");
        TelepenCodes.Add('>', "1000101010100010");
        TelepenCodes.Add('?', "1010101010101110");
        TelepenCodes.Add('@', "1110111011101010");
        TelepenCodes.Add('A', "1011101110111000");
        TelepenCodes.Add('B', "1110001110111000");
        TelepenCodes.Add('C', "1010111011101010");
        TelepenCodes.Add('D', "1110101110111000");
        TelepenCodes.Add('E', "1011100011101010");
        TelepenCodes.Add('F', "1000100011101010");
        TelepenCodes.Add('G', "1010101110111000");
        TelepenCodes.Add('H', "1110111000111000");
        TelepenCodes.Add('I', "1011101011101010");
        TelepenCodes.Add('J', "1110001011101010");
        TelepenCodes.Add('K', "1010111000111000");
        TelepenCodes.Add('L', "1110101011101010");
        TelepenCodes.Add('M', "1010001000111000");
        TelepenCodes.Add('N', "1000101000111000");
        TelepenCodes.Add('O', "1010101011101010");
        TelepenCodes.Add('P', "1110111010111000");
        TelepenCodes.Add('Q', "1011101110001010");
        TelepenCodes.Add('R', "1110001110001010");
        TelepenCodes.Add('S', "1010111010111000");
        TelepenCodes.Add('T', "1110101110001010");
        TelepenCodes.Add('U', "1011100010111000");
        TelepenCodes.Add('V', "1000100010111000");
        TelepenCodes.Add('W', "1010101110001010");
        TelepenCodes.Add('X', "1110100010001010");
        TelepenCodes.Add('Y', "1011101010111000");
        TelepenCodes.Add('Z', "1110001010111000");
        TelepenCodes.Add('[', "1010100010001010");
        TelepenCodes.Add('\\', "1110101010111000");
        TelepenCodes.Add(']', "1010001010001010");
        TelepenCodes.Add('^', "1000101010001010");
        TelepenCodes.Add('_', "1010101010111000");
        TelepenCodes.Add('`', "1110111010001000");
        TelepenCodes.Add('a', "1011101110101010");
        TelepenCodes.Add('b', "1110001110101010");
        TelepenCodes.Add('c', "1010111010001000");
        TelepenCodes.Add('d', "1110101110101010");
        TelepenCodes.Add('e', "1011100010001000");
        TelepenCodes.Add('f', "1000100010001000");
        TelepenCodes.Add('g', "1010101110101010");
        TelepenCodes.Add('h', "1110111000101010");
        TelepenCodes.Add('i', "1011101010001000");
        TelepenCodes.Add('j', "1110001010001000");
        TelepenCodes.Add('k', "1010111000101010");
        TelepenCodes.Add('l', "1110101010001000");
        TelepenCodes.Add('m', "1010001000101010");
        TelepenCodes.Add('n', "1000101000101010");
        TelepenCodes.Add('o', "1010101010001000");
        TelepenCodes.Add('p', "1110111010101010");
        TelepenCodes.Add('q', "1011101000101000");
        TelepenCodes.Add('r', "1110001000101000");
        TelepenCodes.Add('s', "1010111010101010");
        TelepenCodes.Add('t', "1110101000101000");
        TelepenCodes.Add('u', "1011100010101010");
        TelepenCodes.Add('v', "1000100010101010");
        TelepenCodes.Add('w', "1010101000101000");
        TelepenCodes.Add('x', "1110100010101000");
        TelepenCodes.Add('y', "1011101010101010");
        TelepenCodes.Add('z', "1110001010101010");
        TelepenCodes.Add('{', "1010100010101000");
        TelepenCodes.Add('|', "1110101010101010");
        TelepenCodes.Add('}', "1010001010101000");
        TelepenCodes.Add('~', "1000101010101000");
        TelepenCodes.Add(Convert.ToChar(127), "1010101010101010");
        TelepenCodes.Add(StartStopCode.START1, "1010101010111000");
        TelepenCodes.Add(StartStopCode.STOP1, "1110001010101010");
        TelepenCodes.Add(StartStopCode.START2, "1010101011101000");
        TelepenCodes.Add(StartStopCode.STOP2, "1110100010101010");
        TelepenCodes.Add(StartStopCode.START3, "1010101110101000");
        TelepenCodes.Add(StartStopCode.STOP3, "1110101000101010");
    }
}

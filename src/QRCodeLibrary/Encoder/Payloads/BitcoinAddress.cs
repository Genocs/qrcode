using System.Globalization;

namespace Genocs.QRCodeLibrary.Encoder.Payloads;

internal class BitcoinAddress : Payload
{
    private readonly string _address;
    private readonly string? _label;
    private readonly string? _message;
    private readonly double? _amount;

    /// <summary>
    /// Generates a Bitcoin payment payload. QR Codes with this payload can open a Bitcoin payment app.
    /// </summary>
    /// <param name="address">Bitcoin address of the payment receiver.</param>
    /// <param name="amount">Amount of Bitcoins to transfer.</param>
    /// <param name="label">Reference label.</param>
    /// <param name="message">Referece text aka message.</param>
    public BitcoinAddress(string address, double? amount, string? label = null, string? message = null)
    {
        _address = address;

        if (!string.IsNullOrEmpty(label))
        {
            _label = Uri.EscapeDataString(label);
        }

        if (!string.IsNullOrEmpty(message))
        {
            _message = Uri.EscapeDataString(message);
        }

        _amount = amount;
    }

    public override string ToString()
    {
        string? query = null;

        var queryValues = new KeyValuePair<string, string>[]
        {
              new("label", string.IsNullOrWhiteSpace(_label) ? string.Empty : _label),
              new("message", string.IsNullOrWhiteSpace(_message) ? string.Empty : _message),
              new("amount", _amount.HasValue ? _amount.Value.ToString("#.########", CultureInfo.InvariantCulture) : string.Empty)
        };

        if (queryValues.Any(keyPair => !string.IsNullOrEmpty(keyPair.Value)))
        {
            query = "?" + string.Join("&", queryValues
                .Where(keyPair => !string.IsNullOrEmpty(keyPair.Value))
                .Select(keyPair => $"{keyPair.Key}={keyPair.Value}")
                .ToArray());
        }

        return $"bitcoin:{_address}{query}";
    }
}

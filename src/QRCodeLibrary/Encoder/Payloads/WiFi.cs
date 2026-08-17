using Genocs.QRCodeLibrary.Encoder.Helpers;

namespace Genocs.QRCodeLibrary.Encoder.Payloads;

/// <summary>
/// Generates a WiFi payload for QR code encoding. When scanned by a QR Code scanner app, the device will connect to the specified WiFi network.
/// </summary>
internal class WiFi : Payload
{
    private readonly string _ssid;
    private readonly string _password;
    private readonly string _authenticationMode;

    private readonly bool _isHiddenSsid;

    /// <summary>
    /// Generates a WiFi payload. Scanned by a QR Code scanner app, the device will connect to the WiFi.
    /// </summary>
    /// <param name="ssid">SSID of the WiFi network.</param>
    /// <param name="password">Password of the WiFi network.</param>
    /// <param name="authenticationMode">Authentication mode (WEP, WPA, WPA2).</param>
    /// <param name="isHiddenSSID">Set flag, if the WiFi network hides its SSID.</param>
    public WiFi(string ssid, string password, Authentication authenticationMode, bool isHiddenSSID = false)
    {
        _ssid = PayloadHelper.EscapeInput(ssid);
        _ssid = PayloadHelper.IsHexStyle(_ssid) ? "\"" + _ssid + "\"" : _ssid;
        _password = PayloadHelper.EscapeInput(password);
        _password = PayloadHelper.IsHexStyle(_password) ? "\"" + _password + "\"" : _password;
        _authenticationMode = authenticationMode.ToString();
        _isHiddenSsid = isHiddenSSID;
    }

    public override string ToString()
        => $"WIFI:T:{_authenticationMode};S:{_ssid};P:{_password};{(_isHiddenSsid ? "H:true" : string.Empty)};";

    public enum Authentication
    {
        WEP,
        WPA,
        nopass
    }
}

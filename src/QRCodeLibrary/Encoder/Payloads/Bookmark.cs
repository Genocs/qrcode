using Genocs.QRCodeLibrary.Encoder.Helpers;

namespace Genocs.QRCodeLibrary.Encoder.Payloads;

internal class Bookmark : Payload
{
    private readonly string _url;
    private readonly string _title;

    /// <summary>
    /// Generates a bookmark payload. Scanned by an QR Code reader, this one creates a browser bookmark.
    /// </summary>
    /// <param name="url">Url of the bookmark.</param>
    /// <param name="title">Title of the bookmark.</param>
    public Bookmark(string url, string title)
    {
        _url = PayloadHelper.EscapeInput(url);
        _title = PayloadHelper.EscapeInput(title);
    }

    public override string ToString()
    {
        return $"MEBKM:TITLE:{_title};URL:{_url};;";
    }
}

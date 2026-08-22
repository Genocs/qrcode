using Genocs.QRCodeLibrary.Encoder.Helpers;

namespace Genocs.QRCodeLibrary.Encoder.Payloads;

internal class Mail : Payload
{
    private readonly string _mailReceiver;
    private readonly string _subject;
    private readonly string _message;
    private readonly MailEncoding _encoding;

    /// <summary>
    /// Creates an empty email payload.
    /// </summary>
    /// <param name="mailReceiver">Receiver's email address.</param>
    /// <param name="encoding">Payload encoding type. Choose dependent on your QR Code scanner app.</param>
    public Mail(string mailReceiver, MailEncoding encoding = MailEncoding.MAILTO)
    {
        _mailReceiver = mailReceiver;
        _subject = _message = string.Empty;
        _encoding = encoding;
    }

    /// <summary>
    /// Creates an email payload with subject.
    /// </summary>
    /// <param name="mailReceiver">Receiver's email address.</param>
    /// <param name="subject">Subject line of the email.</param>
    /// <param name="encoding">Payload encoding type. Choose dependent on your QR Code scanner app.</param>
    public Mail(string mailReceiver, string subject, MailEncoding encoding = MailEncoding.MAILTO)
    {
        _mailReceiver = mailReceiver;
        _subject = subject;
        _message = string.Empty;
        _encoding = encoding;
    }

    /// <summary>
    /// Creates an email payload with subject and message/text.
    /// </summary>
    /// <param name="mailReceiver">Receiver's email address.</param>
    /// <param name="subject">Subject line of the email.</param>
    /// <param name="message">Message content of the email.</param>
    /// <param name="encoding">Payload encoding type. Choose dependent on your QR Code scanner app.</param>
    public Mail(string mailReceiver, string subject, string message, MailEncoding encoding = MailEncoding.MAILTO)
    {
        _mailReceiver = mailReceiver;
        _subject = subject;
        _message = message;
        _encoding = encoding;
    }

    public override string ToString()
    {
        return _encoding switch
        {
            MailEncoding.MAILTO => $"mailto:{_mailReceiver}?subject={Uri.EscapeDataString(_subject)}&body={Uri.EscapeDataString(_message)}",
            MailEncoding.MATMSG => $"MATMSG:TO:{_mailReceiver};SUB:{PayloadHelper.EscapeInput(_subject)};BODY:{PayloadHelper.EscapeInput(_message)};;",
            MailEncoding.SMTP => $"SMTP:{_mailReceiver}:{PayloadHelper.EscapeInput(_subject, true)}:{PayloadHelper.EscapeInput(_message, true)}",
            _ => _mailReceiver,
        };
    }

    public enum MailEncoding
    {
        MAILTO,
        MATMSG,
        SMTP
    }
}

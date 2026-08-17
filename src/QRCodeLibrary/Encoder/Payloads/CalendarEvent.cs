using System.Text;

namespace Genocs.QRCodeLibrary.Encoder.Payloads;

/// <summary>
/// Generates a calendar entry/event payload for QR code encoding.
/// </summary>
internal class CalendarEvent : Payload
{
    private readonly string _subject;
    private readonly string _description;
    private readonly string _location;
    private readonly string _start;
    private readonly string _end;
    private readonly EventEncoding _encoding;

    /// <summary>
    /// Generates a calender entry/event payload.
    /// </summary>
    /// <param name="subject">Subject/title of the calender event.</param>
    /// <param name="description">Description of the event.</param>
    /// <param name="location">Location (lat:long or address) of the event.</param>
    /// <param name="start">Start time of the event.</param>
    /// <param name="end">End time of the event.</param>
    /// <param name="allDayEvent">It defines if it is a full day event.</param>
    /// <param name="encoding">Type of encoding (universal or iCal).</param>
    public CalendarEvent(string subject, string description, string location, DateTime start, DateTime end, bool allDayEvent, EventEncoding encoding = EventEncoding.Universal)
    {
        _subject = subject;
        _description = description;
        _location = location;
        _encoding = encoding;
        string dtFormat = allDayEvent ? "yyyyMMdd" : "yyyyMMddTHHmmss";
        _start = start.ToString(dtFormat);
        _end = end.ToString(dtFormat);
    }

    public override string ToString()
    {
        StringBuilder vEvent = new StringBuilder();
        vEvent.AppendLine($"BEGIN:VEVENT");
        vEvent.AppendLine($"SUMMARY:{_subject}");
        if (!string.IsNullOrEmpty(_description)) vEvent.AppendLine($"DESCRIPTION:{_description}");
        if (!string.IsNullOrEmpty(_location)) vEvent.AppendLine($"LOCATION:{_location}");
        vEvent.AppendLine($"DTSTART:{_start}");
        vEvent.AppendLine($"DTEND:{_end}");
        vEvent.Append("END:VEVENT");

        if (_encoding == EventEncoding.iCalComplete)
            return $@"BEGIN:VCALENDAR{Environment.NewLine}VERSION:2.0{Environment.NewLine}{vEvent}{Environment.NewLine}END:VCALENDAR";

        return vEvent.ToString();
    }

    public enum EventEncoding
    {
        iCalComplete,
        Universal
    }
}


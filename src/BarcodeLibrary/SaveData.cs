namespace Genocs.BarcodeLibrary;

public sealed class SaveData
{
    public string? Type { get; init; }
    public string? RawData { get; init; }
    public string? EncodedValue { get; init; }
    public double EncodingTime { get; init; }
    public bool IncludeLabel { get; init; }
    public string? Forecolor { get; init; }
    public string? Backcolor { get; init; }
    public string? CountryAssigningManufacturingCode { get; init; }
    public int ImageWidth { get; init; }
    public int ImageHeight { get; init; }
    public string? Image { get; set; }
    public int LabelPosition { get; init; }
    public int Alignment { get; init; }
    public string? LabelFont { get; init; }
    public string? ImageFormat { get; init; }
}

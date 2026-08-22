namespace Genocs.QRCodeLibrary.Decoder;

/// <summary>
/// QR code finder class.
/// </summary>
internal class Finder
{
    // horizontal scan
    internal int Row { get; private set; }
    internal int Col { get; private set; }
    internal double HModule { get; private set; }
    internal double VModule { get; private set; }

    internal int Col1 { get; private set; }
    internal int Col2 { get; private set; }

    internal double Distance { get; private set; }

    // vertical scan
    private int _row1;
    private int _row2;

    private double _moduleSize;

    /// <summary>
    /// Constructor during horizontal scan.
    /// </summary>
    internal Finder(int row, int col1, int col2, double hModule)
    {
        Row = row;
        Col1 = col1;
        Col2 = col2;
        HModule = hModule;
        Distance = double.MaxValue;
    }

    /// <summary>
    /// Match during vertical scan.
    /// </summary>
    internal void Match(int col, int row1, int row2, double vModule)
    {
        // Test if horizontal and vertical are not related
        if (col < Col1 || col >= Col2 || Row < row1 || Row >= row2) return;

        // Module sizes must be about the same
        if (Math.Min(HModule, vModule) < Math.Max(HModule, vModule) * QRDecoder.MODULE_SIZE_DEVIATION) return;

        // calculate distance
        double deltaX = col - (0.5 * (Col1 + Col2));
        double deltaY = Row - (0.5 * (row1 + row2));
        double delta = Math.Sqrt((deltaX * deltaX) + (deltaY * deltaY));

        // distance between two points must be less than 2 pixels
        if (delta > QRDecoder.HOR_VERT_SCAN_MAX_DISTANCE) return;

        // new result is better than last result
        if (delta < Distance)
        {
            Col = col;
            _row1 = row1;
            _row2 = row2;
            VModule = vModule;
            _moduleSize = 0.5 * (HModule + vModule);
            Distance = delta;
        }

        return;
    }

    /// <summary>
    /// Horizontal and vertical scans overlap.
    /// </summary>
    internal bool Overlap(Finder other)
    {
        return other.Col1 < Col2 && other.Col2 >= Col1 && other._row1 < _row2 && other._row2 >= _row1;
    }

    /// <summary>
    /// Finder to string.
    /// </summary>
    public override string ToString()
    {
        if (Distance == double.MaxValue)
        {
            return $"Finder: Row: {Row}, Col1: {Col1}, Col2: {Col2}, HModule: {HModule:0.00}";
        }

        return $"Finder: Row: {Row}, Col: {Col}, Module: {_moduleSize:0.00}, Distance: {Distance:0.00}";
    }
}

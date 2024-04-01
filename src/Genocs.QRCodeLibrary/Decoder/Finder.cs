namespace Genocs.QRCodeLibrary.Decoder;

/// <summary>
/// QR code finder class.
/// </summary>
internal class Finder
{
    // horizontal scan
    internal int _row;
    internal int _col1;
    internal int _col2;
    internal double _hModule;

    // vertical scan
    internal int _col;
    internal int _row1;
    internal int _row2;
    internal double _vModule;

    internal double _distance;
    internal double _moduleSize;

    /// <summary>
    /// Constructor during horizontal scan.
    /// </summary>
    internal Finder(int row, int col1, int col2, double hModule)
    {
        _row = row;
        _col1 = col1;
        _col2 = col2;
        _hModule = hModule;
        _distance = double.MaxValue;
    }

    /// <summary>
    /// Match during vertical scan.
    /// </summary>
    internal void Match(int col, int row1, int row2, double vModule)
    {
        // test if horizontal and vertical are not related
        if (col < _col1 || col >= _col2 || _row < row1 || _row >= row2) return;

        // Module sizes must be about the same
        if (Math.Min(_hModule, vModule) < Math.Max(_hModule, vModule) * QRDecoder.MODULE_SIZE_DEVIATION) return;

        // calculate distance
        double deltaX = col - 0.5 * (_col1 + _col2);
        double deltaY = _row - 0.5 * (row1 + row2);
        double delta = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);

        // distance between two points must be less than 2 pixels
        if (delta > QRDecoder.HOR_VERT_SCAN_MAX_DISTANCE) return;

        // new result is better than last result
        if (delta < _distance)
        {
            _col = col;
            _row1 = row1;
            _row2 = row2;
            _vModule = vModule;
            _moduleSize = 0.5 * (_hModule + vModule);
            _distance = delta;
        }

        return;
    }

    /// <summary>
    /// Horizontal and vertical scans overlap.
    /// </summary>
    internal bool Overlap(Finder other)
    {
        return other._col1 < _col2 && other._col2 >= _col1 && other._row1 < _row2 && other._row2 >= _row1;
    }

    /// <summary>
    /// Finder to string.
    /// </summary>
    public override string ToString()
    {
        if (_distance == double.MaxValue)
        {
            return string.Format("Finder: Row: {0}, Col1: {1}, Col2: {2}, HModule: {3:0.00}", _row, _col1, _col2, _hModule);
        }

        return string.Format("Finder: Row: {0}, Col: {1}, Module: {2:0.00}, Distance: {3:0.00}", _row, _col, _moduleSize, _distance);
    }
}

namespace Genocs.QRCodeLibrary.Decoder;

/// <summary>
/// Represents a corner of a QR code, defined by three finder patterns: top-left, top-right, and bottom-left.
/// </summary>
internal sealed class Corner
{
    private readonly double _topLineDeltaX;
    private readonly double _topLineDeltaY;

    internal Finder TopLeftFinder { get; private set; }
    internal Finder TopRightFinder { get; private set; }
    internal Finder BottomLeftFinder { get; private set; }
    internal double TopLineLength { get; private set; }
    internal double LeftLineDeltaX { get; private set; }
    internal double LeftLineDeltaY { get; private set; }
    internal double LeftLineLength { get; private set; }

    /// <summary>
    /// QR corner constructor.
    /// </summary>
    /// <param name="topLeftFinder">The top-left finder.</param>
    /// <param name="topRightFinder">The top-right finder.</param>
    /// <param name="bottomLeftFinder">The bottom-left finder.</param>
    private Corner(Finder topLeftFinder, Finder topRightFinder, Finder bottomLeftFinder)
    {
        // save three finders
        TopLeftFinder = topLeftFinder;
        TopRightFinder = topRightFinder;
        BottomLeftFinder = bottomLeftFinder;

        // top line slope
        _topLineDeltaX = topRightFinder.Col - topLeftFinder.Col;
        _topLineDeltaY = topRightFinder.Row - topLeftFinder.Row;

        // top line length
        TopLineLength = Math.Sqrt((_topLineDeltaX * _topLineDeltaX) + (_topLineDeltaY * _topLineDeltaY));

        // left line slope
        LeftLineDeltaX = bottomLeftFinder.Col - topLeftFinder.Col;
        LeftLineDeltaY = bottomLeftFinder.Row - topLeftFinder.Row;

        // left line length
        LeftLineLength = Math.Sqrt((LeftLineDeltaX * LeftLineDeltaX) + (LeftLineDeltaY * LeftLineDeltaY));
        return;
    }

    /// <summary>
    /// Create a corner from three finder patterns.
    /// </summary>
    /// <param name="topLeftFinder">The top-left finder.</param>
    /// <param name="topRightFinder">The top-right finder.</param>
    /// <param name="bottomLeftFinder">The bottom-left finder.</param>
    /// <returns>The created corner, or null if invalid.</returns>
    internal static Corner? CreateCorner(Finder topLeftFinder, Finder topRightFinder, Finder bottomLeftFinder)
    {
        // Try all three possible permutation of three finders
        for (int index = 0; index < 3; index++)
        {
            // TestCorner runs three times to test all possibilities
            // rotate top left, top right and bottom left
            if (index != 0)
            {
                var temp = topLeftFinder;
                topLeftFinder = topRightFinder;
                topRightFinder = bottomLeftFinder;
                bottomLeftFinder = temp;
            }

            // top line slope
            double topLineDeltaX = topRightFinder.Col - topLeftFinder.Col;
            double topLineDeltaY = topRightFinder.Row - topLeftFinder.Row;

            // left line slope
            double leftLineDeltaX = bottomLeftFinder.Col - topLeftFinder.Col;
            double leftLineDeltaY = bottomLeftFinder.Row - topLeftFinder.Row;

            // top line length
            double topLineLength = Math.Sqrt((topLineDeltaX * topLineDeltaX) + (topLineDeltaY * topLineDeltaY));

            // left line length
            double leftLineLength = Math.Sqrt((leftLineDeltaX * leftLineDeltaX) + (leftLineDeltaY * leftLineDeltaY));

            // the short side must be at least 80% of the long side
            if (Math.Min(topLineLength, leftLineLength) < QRDecoder.CORNER_SIDE_LENGTH_DEV * Math.Max(topLineLength, leftLineLength)) continue;

            // top line vector
            double topLineSin = topLineDeltaY / topLineLength;
            double topLineCos = topLineDeltaX / topLineLength;

            // rotate lines such that top line is parallel to x axis
            // left line after rotation
            double newLeftX = (topLineCos * leftLineDeltaX) + (topLineSin * leftLineDeltaY);
            double newLeftY = (-topLineSin * leftLineDeltaX) + (topLineCos * leftLineDeltaY);

            // new left line X should be zero (or between +/- 4 deg)
            if (Math.Abs(newLeftX / leftLineLength) > QRDecoder.CORNER_RIGHT_ANGLE_DEV) continue;

            // swap top line with left line
            if (newLeftY < 0)
            {
                // swap top left with bottom right
                (bottomLeftFinder, topRightFinder) = (topRightFinder, bottomLeftFinder);
            }

            return new Corner(topLeftFinder, topRightFinder, bottomLeftFinder);
        }

        return null;
    }

    /// <summary>
    /// Calculate the initial version number of the QR code based on the corner finders.
    /// </summary>
    /// <returns>The initial version number.</returns>
    /// <exception cref="ApplicationException">Thrown when the corner is not valid.</exception>
    internal int InitialVersionNumber()
    {
        // version number based on top line
        double topModules = 7;

        // top line is mostly horizontal
        if (Math.Abs(_topLineDeltaX) >= Math.Abs(_topLineDeltaY))
        {
            topModules += TopLineLength * TopLineLength /
                (Math.Abs(_topLineDeltaX) * 0.5 * (TopLeftFinder.HModule + TopRightFinder.HModule));
        }
        else
        {
            // top line is mostly vertical
            topModules += TopLineLength * TopLineLength /
                (Math.Abs(_topLineDeltaY) * 0.5 * (TopLeftFinder.VModule + TopRightFinder.VModule));
        }

        // version number based on left line
        double leftModules = 7;

        // Left line is mostly vertical
        if (Math.Abs(LeftLineDeltaY) >= Math.Abs(LeftLineDeltaX))
        {
            leftModules += LeftLineLength * LeftLineLength /
                (Math.Abs(LeftLineDeltaY) * 0.5 * (TopLeftFinder.VModule + BottomLeftFinder.VModule));
        }
        else
        {
            // left line is mostly horizontal
            leftModules += LeftLineLength * LeftLineLength /
                (Math.Abs(LeftLineDeltaX) * 0.5 * (TopLeftFinder.HModule + BottomLeftFinder.HModule));
        }

        // version (there is rounding in the calculation)
        int version = ((int)Math.Round(0.5 * (topModules + leftModules)) - 15) / 4;

        // not a valid corner
        if (version < 1 || version > 40)
        {
            throw new ApplicationException("Corner is not valid (version number must be 1 to 40)");
        }

        // exit with version number
        return version;
    }
}

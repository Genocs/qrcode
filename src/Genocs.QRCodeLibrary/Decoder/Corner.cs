﻿namespace Genocs.QRCodeGenerator.Decoder;

/////////////////////////////////////////////////////////////////////
// QR corner three finders pattern class
/////////////////////////////////////////////////////////////////////

internal class Corner
{
    internal Finder TopLeftFinder;
    internal Finder TopRightFinder;
    internal Finder BottomLeftFinder;

    internal double TopLineDeltaX;
    internal double TopLineDeltaY;
    internal double TopLineLength;
    internal double LeftLineDeltaX;
    internal double LeftLineDeltaY;
    internal double LeftLineLength;

    /// <summary>
    /// QR corner constructor.
    /// </summary>
    /// <param name="topLeftFinder"></param>
    /// <param name="topRightFinder"></param>
    /// <param name="bottomLeftFinder"></param>
    private Corner(
            Finder topLeftFinder,
            Finder topRightFinder,
            Finder bottomLeftFinder)
    {
        // save three finders
        this.TopLeftFinder = topLeftFinder;
        this.TopRightFinder = topRightFinder;
        this.BottomLeftFinder = bottomLeftFinder;

        // top line slope
        TopLineDeltaX = topRightFinder._col - topLeftFinder._col;
        TopLineDeltaY = topRightFinder._row - topLeftFinder._row;

        // top line length
        TopLineLength = Math.Sqrt(TopLineDeltaX * TopLineDeltaX + TopLineDeltaY * TopLineDeltaY);

        // left line slope
        LeftLineDeltaX = bottomLeftFinder._col - topLeftFinder._col;
        LeftLineDeltaY = bottomLeftFinder._row - topLeftFinder._row;

        // left line length
        LeftLineLength = Math.Sqrt(LeftLineDeltaX * LeftLineDeltaX + LeftLineDeltaY * LeftLineDeltaY);
        return;
    }

    /////////////////////////////////////////////////////////////////////
    // Test QR corner for validity
    /////////////////////////////////////////////////////////////////////

    internal static Corner? CreateCorner(
            Finder topLeftFinder,
            Finder topRightFinder,
            Finder bottomLeftFinder)
    {
        // try all three possible permutation of three finders
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
            double topLineDeltaX = topRightFinder._col - topLeftFinder._col;
            double topLineDeltaY = topRightFinder._row - topLeftFinder._row;

            // left line slope
            double leftLineDeltaX = bottomLeftFinder._col - topLeftFinder._col;
            double leftLineDeltaY = bottomLeftFinder._row - topLeftFinder._row;

            // top line length
            double topLineLength = Math.Sqrt((topLineDeltaX * topLineDeltaX) + topLineDeltaY * topLineDeltaY);

            // left line length
            double leftLineLength = Math.Sqrt(leftLineDeltaX * leftLineDeltaX + (leftLineDeltaY * leftLineDeltaY));

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
                var tempFinder = topRightFinder;
                topRightFinder = bottomLeftFinder;
                bottomLeftFinder = tempFinder;
            }

            return new Corner(topLeftFinder, topRightFinder, bottomLeftFinder);
        }

        return null;
    }

    /////////////////////////////////////////////////////////////////////
    // Test QR corner for validity
    /////////////////////////////////////////////////////////////////////

    internal int InitialVersionNumber()
    {
        // version number based on top line
        double topModules = 7;

        // top line is mostly horizontal
        if (Math.Abs(TopLineDeltaX) >= Math.Abs(TopLineDeltaY))
        {
            topModules += TopLineLength * TopLineLength /
                (Math.Abs(TopLineDeltaX) * 0.5 * (TopLeftFinder._hModule + TopRightFinder._hModule));
        }

        // top line is mostly vertical
        else
        {
            topModules += TopLineLength * TopLineLength /
                (Math.Abs(TopLineDeltaY) * 0.5 * (TopLeftFinder._vModule + TopRightFinder._vModule));
        }

        // version number based on left line
        double leftModules = 7;

        // Left line is mostly vertical
        if (Math.Abs(LeftLineDeltaY) >= Math.Abs(LeftLineDeltaX))
        {
            leftModules += LeftLineLength * LeftLineLength /
                (Math.Abs(LeftLineDeltaY) * 0.5 * (TopLeftFinder._vModule + BottomLeftFinder._vModule));
        }

        // left line is mostly horizontal
        else
        {
            leftModules += LeftLineLength * LeftLineLength /
                (Math.Abs(LeftLineDeltaX) * 0.5 * (TopLeftFinder._hModule + BottomLeftFinder._hModule));
        }

        // version (there is rounding in the calculation)
        int version = ((int)Math.Round(0.5 * (topModules + leftModules)) - 15) / 4;

        // not a valid corner
        if (version < 1 || version > 40) throw new ApplicationException("Corner is not valid (version number must be 1 to 40)");

        // exit with version number
        return version;
    }
}

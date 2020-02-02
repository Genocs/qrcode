/////////////////////////////////////////////////////////////////////
//
//	QR Code Library
//
//	QR Code error correction calculations.
//
//	Author: Uzi Granot
//	Original Version: 1.0
//	Date: June 30, 2018
//	Copyright (C) 2018-2019 Uzi Granot. All Rights Reserved
//	For full version history please look at QRDecoder.cs
//
//	QR Code Library C# class library and the attached test/demo
//  applications are free software.
//	Software developed by this author is licensed under CPOL 1.02.
//	Some portions of the QRCodeVideoDecoder are licensed under GNU Lesser
//	General Public License v3.0.
//
//	The solution is made of 3 projects:
//	1. QRCodeDecoderLibrary: QR code decoding.
//	3. QRCodeDecoderDemo: Decode QR code image files.
//	4. QRCodeVideoDecoder: Decode QR code using web camera.
//		This demo program is using some of the source modules of
//		Camera_Net project published at CodeProject.com:
//		https://www.codeproject.com/Articles/671407/Camera_Net-Library
//		and at GitHub: https://github.com/free5lot/Camera_Net.
//		This project is based on DirectShowLib.
//		http://sourceforge.net/projects/directshownet/
//		This project includes a modified subset of the source modules.
//
//	The main points of CPOL 1.02 subject to the terms of the License are:
//
//	Source Code and Executable Files can be used in commercial applications;
//	Source Code and Executable Files can be redistributed; and
//	Source Code can be modified to create derivative works.
//	No claim of suitability, guarantee, or any warranty whatsoever is
//	provided. The software is provided "as-is".
//	The Article accompanying the Work may not be distributed or republished
//	without the Author's consent
//
//	For version history please refer to QRDecoder.cs
/////////////////////////////////////////////////////////////////////

using System;

namespace Genocs.QRCodeLibrary.Decoder
{
    internal class ReedSolomon
    {
        internal static int INCORRECTABLE_ERROR = -1;

        internal static int CorrectData
                (
                byte[] ReceivedData,        // recived data buffer with data and error correction code
                int DataLength,         // length of data in the buffer (note sometimes the array is longer than data) 
                int ErrCorrCodewords    // numer of error correction codewords
                )
        {
            // calculate syndrome vector
            int[] Syndrome = CalculateSyndrome(ReceivedData, DataLength, ErrCorrCodewords);

            // received data has no error
            // note: this should not happen because we call this method only if error was detected
            if (Syndrome == null) return 0;

            // Modified Berlekamp-Massey
            // calculate sigma and omega
            int[] Sigma = new int[ErrCorrCodewords / 2 + 2];
            int[] Omega = new int[ErrCorrCodewords / 2 + 1];
            int ErrorCount = CalculateSigmaMBM(Sigma, Omega, Syndrome, ErrCorrCodewords);

            // data cannot be corrected
            if (ErrorCount <= 0) return INCORRECTABLE_ERROR;

            // look for error position using Chien search
            int[] ErrorPosition = new int[ErrorCount];
            if (!ChienSearch(ErrorPosition, DataLength, ErrorCount, Sigma)) return INCORRECTABLE_ERROR;

            // correct data array based on position array
            ApplyCorrection(ReceivedData, DataLength, ErrorCount, ErrorPosition, Sigma, Omega);

            // return error count before it was corrected
            return ErrorCount;
        }

        // Syndrome vector calculation
        // S0 = R0 + R1 +        R2 + ....        + Rn
        // S1 = R0 + R1 * A**1 + R2 * A**2 + .... + Rn * A**n
        // S2 = R0 + R1 * A**2 + R2 * A**4 + .... + Rn * A**2n
        // ....
        // Sm = R0 + R1 * A**m + R2 * A**2m + .... + Rn * A**mn

        internal static int[] CalculateSyndrome
                (
                byte[] ReceivedData,        // recived data buffer with data and error correction code
                int DataLength,         // length of data in the buffer (note sometimes the array is longer than data) 
                int ErrCorrCodewords    // numer of error correction codewords
                )
        {
            // allocate syndrome vector
            int[] Syndrome = new int[ErrCorrCodewords];

            // reset error indicator
            bool Error = false;

            // syndrome[zero] special case
            // Total = Data[0] + Data[1] + ... Data[n]
            int Total = ReceivedData[0];
            for (int SumIndex = 1; SumIndex < DataLength; SumIndex++) Total = ReceivedData[SumIndex] ^ Total;
            Syndrome[0] = Total;
            if (Total != 0) Error = true;

            // all other synsromes
            for (int Index = 1; Index < ErrCorrCodewords; Index++)
            {
                // Total = Data[0] + Data[1] * Alpha + Data[2] * Alpha ** 2 + ... Data[n] * Alpha ** n
                Total = ReceivedData[0];
                for (int IndexT = 1; IndexT < DataLength; IndexT++) Total = ReceivedData[IndexT] ^ MultiplyIntByExp(Total, Index);
                Syndrome[Index] = Total;
                if (Total != 0) Error = true;
            }

            // if there is an error return syndrome vector otherwise return null
            return Error ? Syndrome : null;
        }

        // Modified Berlekamp-Massey
        internal static int CalculateSigmaMBM
                (
                int[] sigma,
                int[] omega,
                int[] syndrome,
                int errCorrCodewords
                )
        {
            int[] polyC = new int[errCorrCodewords];
            int[] polyB = new int[errCorrCodewords];
            polyC[1] = 1;
            polyB[0] = 1;
            int ErrorControl = 1;
            int ErrorCount = 0;     // L
            int m = -1;

            for (int ErrCorrIndex = 0; ErrCorrIndex < errCorrCodewords; ErrCorrIndex++)
            {
                // Calculate the discrepancy
                int Dis = syndrome[ErrCorrIndex];
                for (int i = 1; i <= ErrorCount; i++) Dis ^= Multiply(polyB[i], syndrome[ErrCorrIndex - i]);

                if (Dis != 0)
                {
                    int DisExp = StaticTables.IntToExp[Dis];
                    int[] WorkPolyB = new int[errCorrCodewords];
                    for (int Index = 0; Index <= ErrCorrIndex; Index++) WorkPolyB[Index] = polyB[Index] ^ MultiplyIntByExp(polyC[Index], DisExp);
                    int js = ErrCorrIndex - m;
                    if (js > ErrorCount)
                    {
                        m = ErrCorrIndex - ErrorCount;
                        ErrorCount = js;
                        if (ErrorCount > errCorrCodewords / 2) return INCORRECTABLE_ERROR;
                        for (int Index = 0; Index <= ErrorControl; Index++) polyC[Index] = DivideIntByExp(polyB[Index], DisExp);
                        ErrorControl = ErrorCount;
                    }
                    polyB = WorkPolyB;
                }

                // shift polynomial right one
                Array.Copy(polyC, 0, polyC, 1, Math.Min(polyC.Length - 1, ErrorControl));
                polyC[0] = 0;
                ErrorControl++;
            }

            PolynomialMultiply(omega, polyB, syndrome);
            Array.Copy(polyB, 0, sigma, 0, Math.Min(polyB.Length, sigma.Length));
            return ErrorCount;
        }

        // Chien search is a fast algorithm for determining roots of polynomials defined over a finite field.
        // The most typical use of the Chien search is in finding the roots of error-locator polynomials
        // encountered in decoding Reed-Solomon codes and BCH codes.
        private static bool ChienSearch
                (
                int[] ErrorPosition,
                int DataLength,
                int ErrorCount,
                int[] Sigma
                )
        {
            // last error
            int LastPosition = Sigma[1];

            // one error
            if (ErrorCount == 1)
            {
                // position is out of range
                if (StaticTables.IntToExp[LastPosition] >= DataLength) return false;

                // save the only error position in position array
                ErrorPosition[0] = LastPosition;
                return true;
            }

            // we start at last error position
            int PosIndex = ErrorCount - 1;
            for (int DataIndex = 0; DataIndex < DataLength; DataIndex++)
            {
                int DataIndexInverse = 255 - DataIndex;
                int Total = 1;
                for (int Index = 1; Index <= ErrorCount; Index++) Total ^= MultiplyIntByExp(Sigma[Index], (DataIndexInverse * Index) % 255);
                if (Total != 0) continue;

                int Position = StaticTables.ExpToInt[DataIndex];
                LastPosition ^= Position;
                ErrorPosition[PosIndex--] = Position;
                if (PosIndex == 0)
                {
                    // position is out of range
                    if (StaticTables.IntToExp[LastPosition] >= DataLength) return false;
                    ErrorPosition[0] = LastPosition;
                    return true;
                }
            }

            // search failed
            return false;
        }

        private static void ApplyCorrection
                (
                byte[] ReceivedData,
                int DataLength,
                int ErrorCount,
                int[] ErrorPosition,
                int[] Sigma,
                int[] Omega
                )
        {
            for (int ErrIndex = 0; ErrIndex < ErrorCount; ErrIndex++)
            {
                int ps = ErrorPosition[ErrIndex];
                int zlog = 255 - StaticTables.IntToExp[ps];
                int OmegaTotal = Omega[0];
                for (int Index = 1; Index < ErrorCount; Index++) OmegaTotal ^= MultiplyIntByExp(Omega[Index], (zlog * Index) % 255);
                int SigmaTotal = Sigma[1];
                for (int j = 2; j < ErrorCount; j += 2) SigmaTotal ^= MultiplyIntByExp(Sigma[j + 1], (zlog * j) % 255);
                ReceivedData[DataLength - 1 - StaticTables.IntToExp[ps]] ^= (byte)MultiplyDivide(ps, OmegaTotal, SigmaTotal);
            }
            return;
        }

        internal static void PolynominalDivision(byte[] Polynomial, int PolyLength, byte[] Generator, int ErrCorrCodewords)
        {
            int DataCodewords = PolyLength - ErrCorrCodewords;

            // error correction polynomial division
            for (int Index = 0; Index < DataCodewords; Index++)
            {
                // current first codeword is zero
                if (Polynomial[Index] == 0) continue;

                // current first codeword is not zero
                int Multiplier = StaticTables.IntToExp[Polynomial[Index]];

                // loop for error correction coofficients
                for (int GeneratorIndex = 0; GeneratorIndex < ErrCorrCodewords; GeneratorIndex++)
                {
                    Polynomial[Index + 1 + GeneratorIndex] = (byte)(Polynomial[Index + 1 + GeneratorIndex] ^ StaticTables.ExpToInt[Generator[GeneratorIndex] + Multiplier]);
                }
            }
            return;
        }

        internal static int Multiply
                (
                int Int1,
                int Int2
                )
        {
            return (Int1 == 0 || Int2 == 0) ? 0 : StaticTables.ExpToInt[StaticTables.IntToExp[Int1] + StaticTables.IntToExp[Int2]];
        }

        internal static int MultiplyIntByExp
                (
                int Int,
                int Exp
                )
        {
            return Int == 0 ? 0 : StaticTables.ExpToInt[StaticTables.IntToExp[Int] + Exp];
        }

        internal static int MultiplyDivide
                (
                int int1,
                int int2,
                int int3
                )
        {
            return (int1 == 0 || int2 == 0) ? 0 : StaticTables.ExpToInt[(StaticTables.IntToExp[int1] + StaticTables.IntToExp[int2] - StaticTables.IntToExp[int3] + 255) % 255];
        }

        internal static int DivideIntByExp
                (
                int Int,
                int Exp
                )
        {
            return Int == 0 ? 0 : StaticTables.ExpToInt[StaticTables.IntToExp[Int] - Exp + 255];
        }

        internal static void PolynomialMultiply(int[] result, int[] poly1, int[] poly2)
        {
            Array.Clear(result, 0, result.Length);
            for (int index1 = 0; index1 < poly1.Length; index1++)
            {
                if (poly1[index1] == 0) continue;
                int loga = StaticTables.IntToExp[poly1[index1]];
                int index2End = Math.Min(poly2.Length, result.Length - index1);
                // = Sum(Poly1[Index1] * Poly2[Index2]) for all Index2
                for (int index2 = 0; index2 < index2End; index2++)
                    if (poly2[index2] != 0) result[index1 + index2] ^= StaticTables.ExpToInt[loga + StaticTables.IntToExp[poly2[index2]]];
            }
            return;
        }
    }
}

﻿# .NET8 Barcode library

This package contains a set of functionalities to build different kind of barcodes.

Thanks to 

The libraries are built using .NET8.


## Description

This package contains a set of functionalities to handling authorization logic as JWT.


## Support

Please check the GitHub repository getting more info.


### DataProvider Settings
Following are the project settings needed to enable monitoring

``` json

```

## Release notes

Release Notes for BarcodeLib.dll
================================
3.0.0.0
- BREAK CHANGE: Update to SkiaSharp drawing lib for cross platform support
- Labels rewrite
2.3.0.0
- Fix EAN13 country code lookup failure
- Update to System.Drawing.Common 5.0.0
2.2.10.0
- Fix Standard 2 of 5 encoding bug
2.2.9.0
- Fix Interleaved 2 of 5 encoding bug
2.2.8.0
- Fix Standard 2 of 5 checksum calculation
2.2.7.0
- Fix Interleaved 2 of 5 checksum calculation
- Add support setting DPI for height and width
- Fix UPC-E encoding
- Fix font resizing issue
- Add .snupkg packaging for source debugging
2.2.6.0
- bug fix
2.2.5.0
- Remove unnecessary calculations to make Pharmacode more efficient
2.2.4.0
- Fix incorrect encoding for single digit Pharmacode
2.2.3.0
- Allow use in partially trusted assemblies for SSRS
2.2.2.0
- Simplified the code to check numeric
- Allow FNC1 for Code 128-C
2.2.1.0
- standard 2 of 5 append check digit encoding
2.2.0.0
- Add Interleaved 2 of 5 Mod 10
- Add Standard 2 of 5 Mod 10
2.1.0.0
- Fix massive memory leak
2.0.0.0
- Conversion to .NET Standard 2.0
- Build nuget package on every build
1.0.0.24
- bug fix with Code 128 START_C char
- add missing EAN-13 country codes
1.0.0.21
- Fixed bug in UPC-E encoding
1.0.0.19
- Fixed bug in Code 39 Mod 43 check digit calculation
1.0.0.18
- cleaned up layout of test application
- added alternate label text property
1.0.0.17
- Fixed a bug in Code 11 where K checksums were being calculated for messages shorter than 10 characters in length.
- Fixed a bug in PostNet that drew incorrectly for all PostNet barcodes (thanks jonney3099)
- Added Code 39 Mod 43 support
- Updated the GetImageSize method to return an ImageSize object containing the real world size of the image generated.
1.0.0.16
- Pharmacode symbology added.
- Removed duplicate IsNumeric check method, moved CheckNumericOnly method to the BarcodeCommon class
1.0.0.15
- Fixed a bug in the Codabar symbology that would not allow valid non-numeric characters from being encoded.
- Fixed a bug that would not encode C128-C codes with FNC1 in the starting characters.
- Fixed a bug in the ITF-14 check digit calculation where it would calculate the wrong check digit most of the time.
1.0.0.14
- Added a byte array representation of the encoded image (Encoded_Image_Bytes) which can be used in Crystal Reports. See (http://www.codeproject.com/Messages/4300245/Re-Problem-in-Reports-RDLC.aspx)
- Updated the XML schema to use integers instead of Enums due to versioning conflicts.
1.0.0.13
- Corrected comments on class summaries
- Eliminated some unnecessary private variables
1.0.0.12
- Fixed a bug in drawing of barcodes that caused barcodes to be cut off on the left and right when aligned to the sides.
- Fixed a bug in the project where the BarcodeXML dataset was corrupt.
- Added GetSizeOfImage function that returns the real world coordinates of the EncodedImage.
- Facing Identification Mark(FIM) symbology added.
1.0.0.11
- Fixed a bug in Code 93 that caused four characters to be encoded incorrectly.
- Fixed a bug where the ITF-14 bearer bars were not drawing evenly.
- Fixed a bug in Codabar that would report an object not set to a reference error if non-numeric is found.
- Added property LabelPosition to position label above or below the barcode, and align the label left, right, or center.
- Added property RotateFlipType to allow rotation/flipping the image before it is returned.
- Added several of the newer properties to the XML output of GetXML().
- Removed Codabar start / stop characters in the label.
- IsNumeric function added to BarcodeCommon so that every symbology has access to it.
1.0.0.10
- Fixed a bug in Code 39 extended that was erasing the start and stop characters if extended was used.
- Fixed a bug that if barcodes were aligned left or right they would cut off a part of the starting or ending bar which was a drawing bug thats been present since 1.0.0.0
- Fixed a bug in Code 128C that checked for numeric data, if it was bigger than Int64 and was numeric it would throw and exception saying it was non-numeric data.
- Fixed a bug in UPC-A that encoded with the same sets as EAN-13 and only CodeA and CodeC should have been used.
- Made the Version property static so it can be read without creating an instance.
- Added a LabelFont property to allow the labels font to be changed.
- Restructured the label drawing functions to take font height and use that to determine the height of the label.
- Created an IsNumeric function in C128-C to better separate that functionality.  Replaced Int64 with Int32 to better allow compatibility with x86 processors.
- EncodingTime now includes the time to draw the image and not just the encoding.
- Alignment property added to allow aligning the barcode in the image given if the image space is wider than the drawn barcode. (Default is centered)
- Postnet drawing is encorporated into the default drawing case now, which shortens the code and gets rid of some redundant code.
- Telepen symbology added.
1.0.0.9
- The UPC-A check digit is now calculated every time whether 11 or 12 digits is passed in.  If 12 is passed in and its got an incorrect check digit then it is replaced with the correct check digit.  This prevents an unscannable barcode from being generated.
- The EAN13 check digit is now calculated every time whether 12 or 13 digits is passed in.  If 13 is passed in and its got an incorrect check digit then it is replaced with the correct check digit.  This prevents an unscannable barcode from being generated.
- All errors can be accessed via the BarcodeLib.Errors properties which is a list of seperate errors encountered.
- All symbologies were moved to the BarcodeLib.Symbologies namespace for better organization.
- FormattedData property was not being used so it was removed.
- Version property was added to BarcodeLib to allow reading the libraries version number.
1.0.0.8
- Changed the use of a Pen object that was not disposed of.  This was not causing a problem just bad technique.
- Fixed an encoding issue with C128-B that had a wrong character in its encoding set at one point. (U instead of Z in codeset B)
1.0.0.7
- Fixed a bug that allowed non-numeric data to be encoded with Code128-C, a check has been put in place to handle this.  It throws an error EC128-6 now if found to contain something that isnt in Code128-C.
- Fixed a bug in GetEncoding() for C128.  This would allow Code128-B to switch and dynamically use Code128-A if it couldnt find a char in its set.
1.0.0.6
- Fixed a bug in Code128-A and Code128-B that would cause it to encode incorrectly due to incorrectly trying to compact the barcode for Code128-C.  This functionality is now bypassed if Code128-A or Code128-B is selected.
- Removed a useless variable bEncoded from BarcodeLib.cs
- Static methods now support generating the data label (required addition of a parameter to 3 of the 5 static methods used to encode).
- Property now available to retrieve the amount of time (EncodingTime) it took to encode and generate the image. (Might be helpful for diagnostics)
- Modified a few error messages to be more descriptive about correcting the problem with data length.
- Barcode class now inherits from IDisposable
- XML export functionality added to BarcodeLib to allow the data, encoded data and other properties to be exported in XML along with the Image in Base64String format.  This includes functionality to GetXML() and GetImageFromXML(BarcodeXML).
- To go along with the XML functionality there is now a dataset included that has the basic layout of the XML data, to make importing and exporting easy.
- ImageFormat is now a property to set to select what type of image you want returned (JPEG is default).  This can help speed of transferring data if using a webservice.
- ITF-14 now draws the label with the proper background color instead of always being white.
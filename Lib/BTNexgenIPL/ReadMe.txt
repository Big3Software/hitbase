Binary Technologies
http://www.binary-technologies.de | http://www.binary-technologies.com
info@binary-technologies.com


    NexgenIPL - Image Processing Library
    ------------------------------------

NexgenIPL is a MS Visual C++ Extension Library for image loading and manipulating.

Currently the following formats are supported. All formats can be read from file and resource.

Reading: BMP, PCX, PNG, PBM, PPM, PGM, TGA, TIFF, JPEG, GIF (animated), IFF, ILBM, RAS, EPS,
         ICO, MNG, JNG, WMF, EMF, AMP, PSP, JP2, JPC, YUV, CUT
Writing: BMP, PCX, PNG, MNG, PBM, PPM, PGM, TGA, TIFF, JPEG, GIF (uncompressed), JP2, YUV,
         CUT, RAS

Embedded color profile support (only for decoders) is available for:

- TIFF, JPEG

EXIF support: +-------+---------+---------+
              | Codec | Decoder | Encoder | 
              +-------+---------+---------+
              | JPEG  |    x    |    x    |
              +-------+---------+---------+

IPTC support: +-------+---------+---------+
              | Codec | Decoder | Encoder | 
              +-------+---------+---------+
              | JPEG  |    x    |    x    |
              | TIFF  |    x    |         |
              +-------+---------+---------+

The following image manipulation functions are supported:

- Add your own input/output streams
- Add your own decoders/encoders
- 1, 4, 8, 16, 24 and 32 bpp support
- Convert to 24 and 32 bits per pixel
- Copy
- Fill
- Paste
- Blend
- Add
- Subtract
- Multiply
- Divide
- Difference
- Lightest
- Darkest
- Average
- Or
- And
- Screen
- Overlay
- AdjustBrightness
- AdjustContrast
- AdjustHighlight
- AdjustMidtone
- AdjustShadow
- AdjustHue
- AdjustSaturation
- AdjustGamma
- AdjustHLS
- AdjustRed
- AdjustGreen
- AdjustBlue
- AdjustRGB
- Grayscale
- Negate
- Flip
- Mirror
- Rotate
- Rotate90
- Quantize
- Resize
- Crop
- Sharpen
- Normalize
- EdgeEnhance
- EdgeEnhanceMore
- FindEdge
- Solarize
- Embose
- Smooth
- Antialias
- DeInterlace
- Posterize
- Threshold
- Median
- Dilate
- Erode
- Detail
- Convolve
- Noise
- OilPaint
- much more...

The following resizing filters are available:

- Box
- Bilinear
- Gaussian
- Hamming
- Blackman
- Triangle
- Bell
- BSpline
- Sinc
- Lanczos3
- Mitchell
- Quadratic
- Hermite
- Hanning
- Catrom
- Cubic
- Hann4

Please notice:

GIF-Files created with NexgenIPL are uncopressed (no LZW usage). So there shouldn't be any
problems with UNISYS's licences.

The author accepts no liability for any damage/loss of business that this product may cause
(also see beginning of each header file).

Usage
-----

To use this library, you must copy the files

	- BTCImageData.h
	- BTCImageObject.h
	- BTNexgenIPL.h
        - BTICodec.h
	- BTIProgressNotification.h
	- BTCIOStream.h
	- BTDefines.h
	- BTCColor.h
	- BTCOptions.h
	- BTCString.h

located in the "\include" directory to the directory where your
other include files are located. When you use Microsoft Visual c++ v5.0
or v6.0 this is for example:

	c:\program files\devstudio\vc\include
	c:\program files\microsoft visual studio\vc98\include

or copy the headers to the directory of your project.

To use the librarys you do not have to edit your project settings. The librarys
lib (*.lib) files will be added automatically. If you do not copy the BTNexgenIPL32.lib
to MS Visual C++ default "lib" directory or your project's root, you have to change
the path to the libs in the BTNexgenIPL.h.

BTNexgenIPL.h
-------------

...

/////////////////////////////////////////////////////////////////////////////
// The following will ensure that when building an application (or another
// DLL) using this DLL, the appropriate .LIB file will automatically be used
// when linking.
/////////////////////////////////////////////////////////////////////////////

#ifndef _BTNEXGENIPL_NOAUTOLIB
   #if defined(UNICODE) || defined(_UNICODE)
      #define _BTNEXGENIPL_LIBPATH "BTNexgenIPL32u.lib" <- Change path for unicode version here
   #else
      #define _BTNEXGENIPL_LIBPATH "BTNexgenIPL32.lib"  <- Change path for default version here
   #endif

   #define _BTNEXGENIPL_MESSAGE "Automatically linking with " _BTNEXGENIPL_LIBPATH

   #pragma comment(lib, _BTNEXGENIPL_LIBPATH)
   #pragma message(_BTNEXGENIPL_MESSAGE)
#endif

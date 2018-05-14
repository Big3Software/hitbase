/////////////////////////////////////////////////////////////////////////////
//
// NexgenIPL - Next Generation Image Processing Library
// Copyright (c) 1999-2003 Binary Technologies
// All Rights Reserved.
//
// This code may be used in compiled form in any way you desire. This
// file may be redistributed unmodified by any means PROVIDING it is 
// not sold for profit without the authors written consent, and 
// providing that this notice and the authors name is included. If 
// the source code in this file is used in any commercial application 
// then a simple email would be nice.
//
// THIS SOFTWARE IS PROVIDED BY BINARY TECHNOLOGIES ``AS IS'' AND ANY
// EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL BINARY TECHNOLOGIES OR
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
// EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
// PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
// PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY
// OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE
// USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH
// DAMAGE.
//
// Binary Technologies
// http://www.binary-technologies.com
// info@binary-technologies.com
//
/////////////////////////////////////////////////////////////////////////////

#ifndef BTCOPTIONS_INCLUDED
#define BTCOPTIONS_INCLUDED

#pragma once

#include "BTDefines.h"
#include "BTNexgenIPL.h"
#include "BTCColor.h"
#include "BTCString.h"
#include "BTCIOStream.h"

#ifndef BTGETOPTION
#define BTGETOPTION(opt,id,val) if(NULL != (opt)) { (void)(opt)->GetOption(id,val); }
#endif
#ifndef BTSETOPTION
#define BTSETOPTION(opt,id,val) if(NULL != (opt)) { (void)(opt)->SetOption(id,val); }
#endif

class BTNEXGENIPL_CLASS_EXPORT BTCOptions
{
public:
	BTCOptions() {}
	virtual ~BTCOptions() {}

	virtual BTRESULT GetOption(const long lId, int &nValue) = 0;
	virtual BTRESULT GetOption(const long lId, long &lValue) = 0;
	virtual BTRESULT GetOption(const long lId, double &dValue) = 0;
	virtual BTRESULT GetOption(const long lId, BTCString &strValue) = 0;
	virtual BTRESULT GetOption(const long lId, bool &bValue) = 0;
	virtual BTRESULT GetOption(const long lId, BTColorRGB &rgbValue) = 0;

	virtual BTRESULT SetOption(const long lId, const int nValue) = 0;
	virtual BTRESULT SetOption(const long lId, const long lValue) = 0;
	virtual BTRESULT SetOption(const long lId, const double dValue) = 0;
	virtual BTRESULT SetOption(const long lId, const BTCString strValue) = 0;
	virtual BTRESULT SetOption(const long lId, const bool bValue) = 0;
	virtual BTRESULT SetOption(const long lId, const BTColorRGB rgbValue) = 0;
};

class BTNEXGENIPL_CLASS_EXPORT BTCDecoderOptions : public BTCOptions
{
public:
	virtual BTRESULT GetOption(const long lId, int &nValue);
	virtual BTRESULT GetOption(const long lId, long &lValue);
	virtual BTRESULT GetOption(const long lId, double &dValue);
	virtual BTRESULT GetOption(const long lId, BTCString &strValue);
	virtual BTRESULT GetOption(const long lId, bool &bValue);
	virtual BTRESULT GetOption(const long lId, BTColorRGB &rgbValue);
	virtual BTRESULT GetOption(const long lId, BTCOStream **ppOStream);

	virtual BTRESULT SetOption(const long lId, const int nValue);
	virtual BTRESULT SetOption(const long lId, const long lValue);
	virtual BTRESULT SetOption(const long lId, const double dValue);
	virtual BTRESULT SetOption(const long lId, const BTCString strValue);
	virtual BTRESULT SetOption(const long lId, const bool bValue);
	virtual BTRESULT SetOption(const long lId, const BTColorRGB rgbValue);
	virtual BTRESULT SetOption(const long lId, BTCOStream *pOStream);
};

class BTNEXGENIPL_CLASS_EXPORT BTCEncoderOptions : public BTCOptions
{
public:
	virtual BTRESULT GetOption(const long lId, int &nValue);
	virtual BTRESULT GetOption(const long lId, long &lValue);
	virtual BTRESULT GetOption(const long lId, double &dValue);
	virtual BTRESULT GetOption(const long lId, BTCString &strValue);
	virtual BTRESULT GetOption(const long lId, bool &bValue);
	virtual BTRESULT GetOption(const long lId, BTColorRGB &rgbValue);
	virtual BTRESULT GetOption(const long lId, BTCIStream **ppIStream);

	virtual BTRESULT SetOption(const long lId, const int nValue);
	virtual BTRESULT SetOption(const long lId, const long lValue);
	virtual BTRESULT SetOption(const long lId, const double dValue);
	virtual BTRESULT SetOption(const long lId, const BTCString strValue);
	virtual BTRESULT SetOption(const long lId, const bool bValue);
	virtual BTRESULT SetOption(const long lId, const BTColorRGB rgbValue);
	virtual BTRESULT SetOption(const long lId, BTCIStream *pIStream);
};

/////////////////////////////////////////////////////////////////////////////
// BMP options
/////////////////////////////////////////////////////////////////////////////

class BTNEXGENIPL_CLASS_EXPORT BTCEncoderOptionsBMP : public BTCEncoderOptions
{
public:
	BTCEncoderOptionsBMP();
	virtual ~BTCEncoderOptionsBMP();

	enum { BTEO_BMP_ENABLERLE = 0 };

	virtual BTRESULT GetOption(const long lId, bool &bValue);
	virtual BTRESULT SetOption(const long lId, const bool bValue);

private:
	bool m_bEnableRLE; // [in] Enable/disable RLE (run length encryption) compression.
};

/////////////////////////////////////////////////////////////////////////////
// TGA options
/////////////////////////////////////////////////////////////////////////////

class BTNEXGENIPL_CLASS_EXPORT BTCEncoderOptionsTGA : public BTCEncoderOptions
{
public:
	BTCEncoderOptionsTGA();
	virtual ~BTCEncoderOptionsTGA();

	enum { BTEO_TGA_ENABLERLE = 0 };

	virtual BTRESULT GetOption(const long lId, bool &bValue);
	virtual BTRESULT SetOption(const long lId, const bool bValue);

private:
	bool m_bEnableRLE; // [in] Enable/disable RLE (run length encryption) compression.
};

/////////////////////////////////////////////////////////////////////////////
// MNG options
/////////////////////////////////////////////////////////////////////////////

class BTNEXGENIPL_CLASS_EXPORT BTCEncoderOptionsMNG : public BTCEncoderOptions
{
public:
	BTCEncoderOptionsMNG();
	virtual ~BTCEncoderOptionsMNG();

	enum {	BTEO_MNG_COMPRESSIONLEVEL = 0,
			BTEO_MNG_TITLE,
			BTEO_MNG_AUTHOR,
			BTEO_MNG_DESCRIPTION,
			BTEO_MNG_COPYRIGHT,
			BTEO_MNG_CREATIONTIME,
			BTEO_MNG_SOFTWARE,
			BTEO_MNG_DISCLAIMER,
			BTEO_MNG_WARNING,
			BTEO_MNG_SOURCE,
			BTEO_MNG_COMMENT };

	virtual BTRESULT GetOption(const long lId, int &nValue);
	virtual BTRESULT GetOption(const long lId, BTCString &strValue);

	virtual BTRESULT SetOption(const long lId, const int nValue);
	virtual BTRESULT SetOption(const long lId, const BTCString strValue);

private:
	int       m_nCompressionLevel; // [in] 1...9 = Speed...Best, 0 = no compression, -1 = default.
	BTCString m_strTitle;          // [in]
	BTCString m_strAuthor;         // [in]
	BTCString m_strDescription;    // [in]
	BTCString m_strCopyright;      // [in]
	BTCString m_strCreationTime;   // [in]
	BTCString m_strSoftware;       // [in]
	BTCString m_strDisclaimer;     // [in]
	BTCString m_strWarning;        // [in]
	BTCString m_strSource;         // [in]
	BTCString m_strComment;        // [in]
};

class BTNEXGENIPL_CLASS_EXPORT BTCDecoderOptionsMNG : public BTCDecoderOptions
{
public:
	BTCDecoderOptionsMNG();
	virtual ~BTCDecoderOptionsMNG();

	enum {	BTDO_MNG_TITLE = 0,
			BTDO_MNG_AUTHOR,
			BTDO_MNG_DESCRIPTION,
			BTDO_MNG_COPYRIGHT,
			BTDO_MNG_CREATIONTIME,
			BTDO_MNG_SOFTWARE,
			BTDO_MNG_DISCLAIMER,
			BTDO_MNG_WARNING,
			BTDO_MNG_SOURCE,
			BTDO_MNG_COMMENT };

	virtual BTRESULT GetOption(const long lId, BTCString &strValue);
	virtual BTRESULT SetOption(const long lId, const BTCString strValue);

private:
	BTCString m_strTitle;        // [out]
	BTCString m_strAuthor;       // [out]
	BTCString m_strDescription;  // [out]
	BTCString m_strCopyright;    // [out]
	BTCString m_strCreationTime; // [out]
	BTCString m_strSoftware;     // [out]
	BTCString m_strDisclaimer;   // [out]
	BTCString m_strWarning;      // [out]
	BTCString m_strSource;       // [out]
	BTCString m_strComment;      // [out]
};

/////////////////////////////////////////////////////////////////////////////
// JPEG options
/////////////////////////////////////////////////////////////////////////////

class BTNEXGENIPL_CLASS_EXPORT BTCEncoderOptionsJPEG : public BTCEncoderOptions
{
public:
	BTCEncoderOptionsJPEG();
	virtual ~BTCEncoderOptionsJPEG();

	enum {	BTEO_JPEG_QUALITY = 0,
			BTEO_JPEG_SMOOTHINGFACTOR,
			BTEO_JPEG_DCTMETHOD,
			BTEO_JPEG_OPTIMIZECODING,
			BTEO_JPEG_EXIFDATA,
			BTEO_JPEG_IPTCDATA };

	virtual BTRESULT GetOption(const long lId, int &nValue);
	virtual BTRESULT GetOption(const long lId, bool &bValue);
	virtual BTRESULT GetOption(const long lId, BTCIStream **ppIStream);
	
	virtual BTRESULT SetOption(const long lId, const int nValue);
	virtual BTRESULT SetOption(const long lId, const bool bValue);
	virtual BTRESULT SetOption(const long lId, BTCIStream *pIStream);

private:
	int  m_nQuality;            // [in] Quality of compression on a scale from 0..100 [low..best].
	int  m_nSmoothingFactor;    // [in] 1..100, or 0 for no input smoothing.
	int  m_nDCTMethod;          // [in] DCT/IDCT algorithm.
	bool m_bOptimizeCoding;     // [in] true = optimize entropy encoding parms.
	BTCIStream *m_pIEXIFStream; // [in] Used to import EXIF data.
	BTCIStream *m_pIIPTCStream; // [in] Used to import IPTC data.
};

class BTNEXGENIPL_CLASS_EXPORT BTCDecoderOptionsJPEG : public BTCDecoderOptions
{
public:
	BTCDecoderOptionsJPEG();
	virtual ~BTCDecoderOptionsJPEG();

	enum {	BTDO_JPEG_DCTMETHOD = 0,
			BTDO_JPEG_USEEMBEDDEDPROFILE,
			BTDO_JPEG_EXIFDATA,
			BTDO_JPEG_IPTCDATA };

	virtual BTRESULT GetOption(const long lId, int &nValue);
	virtual BTRESULT GetOption(const long lId, bool &bValue);
	virtual BTRESULT GetOption(const long lId, BTCOStream **ppOStream);
	
	virtual BTRESULT SetOption(const long lId, const int nValue);
	virtual BTRESULT SetOption(const long lId, const bool bValue);
	virtual BTRESULT SetOption(const long lId, BTCOStream *pOStream);

private:
	int  m_nDCTMethod;          // [in] DCT/IDCT algorithm.
	bool m_bUseEmbeddedProfile; // [in] Use embedded color profile if available.
	BTCOStream *m_pOEXIFStream;	// [out] Used to export EXIF data.
	BTCOStream *m_pOIPTCStream;	// [out] Used to export IPTC data.
};

/////////////////////////////////////////////////////////////////////////////
// PBM/PGM/PPM options
/////////////////////////////////////////////////////////////////////////////

class BTNEXGENIPL_CLASS_EXPORT BTCEncoderOptionsPXM : public BTCEncoderOptions
{
public:
	BTCEncoderOptionsPXM();
	virtual ~BTCEncoderOptionsPXM();

	enum {	BTEO_PXM_ENCODINGTYPE = 0,
			BTEO_PXM_DESCRIPTION };

	virtual BTRESULT GetOption(const long lId, int &nValue);
	virtual BTRESULT GetOption(const long lId, BTCString &strValue);
	
	virtual BTRESULT SetOption(const long lId, const int nValue);
	virtual BTRESULT SetOption(const long lId, const BTCString strValue);

private:
	int       m_nEncodingType;  // [in] The encoding type of the image (Ascii or Binary/Raw).
	BTCString m_strDescription; // [in] Text that will be saved in the file. Each line
                                // must start with a '#':
                                //
                                // BTCEncoderOptionsPXM Param;
                                // Param.SetOption( BTEO_PXM_DESCRIPTION, "# 1. Line\n# 2. Line\n# 3. Line");
};

class BTNEXGENIPL_CLASS_EXPORT BTCDecoderOptionsPXM : public BTCDecoderOptions
{
public:
	BTCDecoderOptionsPXM();
	virtual ~BTCDecoderOptionsPXM();

	enum {	BTDO_PXM_ENCODINGTYPE = 0,
			BTDO_PXM_DESCRIPTION };

	virtual BTRESULT GetOption(const long lId, int &nValue);
	virtual BTRESULT GetOption(const long lId, BTCString &strValue);

	virtual BTRESULT SetOption(const long lId, const int nValue);
	virtual BTRESULT SetOption(const long lId, const BTCString strValue);

private:
	int       m_nEncodingType;  // [out] The encoding type of the image (Ascii or Binary/Raw).
	BTCString m_strDescription; // [out]
};

/////////////////////////////////////////////////////////////////////////////
// TIFF options
/////////////////////////////////////////////////////////////////////////////

class BTNEXGENIPL_CLASS_EXPORT BTCEncoderOptionsTIFF : public BTCEncoderOptions
{
public:
	BTCEncoderOptionsTIFF();
	virtual ~BTCEncoderOptionsTIFF();

	enum {	BTEO_TIFF_TITLE = 0,
			BTEO_TIFF_AUTHOR,
			BTEO_TIFF_DESCRIPTION,
			BTEO_TIFF_SOFTWARE,
			BTEO_TIFF_COPYRIGHT,
			BTEO_TIFF_CODEC };

	enum BTTIFFCodec {	BTTIFFCODEC_NONE          = 1,
						BTTIFFCODEC_CCITTRLE      = 2,
						BTTIFFCODEC_CCITTRLEW     = 32771,
						BTTIFFCODEC_CCITTFAX3     = 3,
						BTTIFFCODEC_CCITT_T4      = 3,
						BTTIFFCODEC_CCITTFAX4     = 4,
						BTTIFFCODEC_CCITT_T6      = 4,
						BTTIFFCODEC_JPEG          = 7,
						BTTIFFCODEC_PACKBITS      = 32773,
						BTTIFFCODEC_DEFLATE       = 32946,
						BTTIFFCODEC_ADOBE_DEFLATE = 8,
						BTTIFFCODEC_SGILOG        = 34676,
						BTTIFFCODEC_SGILOG24      = 34677 };

	virtual BTRESULT GetOption(const long lId, long &lValue);
	virtual BTRESULT GetOption(const long lId, BTCString &strValue);
	
	virtual BTRESULT SetOption(const long lId, const long lValue);
	virtual BTRESULT SetOption(const long lId, const BTCString strValue);

private:
	BTCString m_strTitle;       // [in]
	BTCString m_strAuthor;      // [in]
	BTCString m_strDescription; // [in]
	BTCString m_strSoftware;    // [in]
	BTCString m_strCopyright;   // [in]
	long      m_lCodec;         // [in] Codec used to encode image.
};

class BTNEXGENIPL_CLASS_EXPORT BTCDecoderOptionsTIFF : public BTCDecoderOptions
{
public:
	BTCDecoderOptionsTIFF();
	virtual ~BTCDecoderOptionsTIFF();

	enum {	BTDO_TIFF_USEEMBEDDEDPROFILE = 0,
			BTDO_TIFF_TITLE,
			BTDO_TIFF_AUTHOR,
			BTDO_TIFF_DESCRIPTION,
			BTDO_TIFF_SOFTWARE,
			BTDO_TIFF_COPYRIGHT,
			BTDO_TIFF_IPTCDATA };

	virtual BTRESULT GetOption(const long lId, bool &bValue);
	virtual BTRESULT GetOption(const long lId, BTCString &strValue);
	virtual BTRESULT GetOption(const long lId, BTCOStream **ppOStream);
	
	virtual BTRESULT SetOption(const long lId, const bool bValue);
	virtual BTRESULT SetOption(const long lId, const BTCString strValue);
	virtual BTRESULT SetOption(const long lId, BTCOStream *pOStream);

private:
	bool        m_bUseEmbeddedProfile; // [in] Use embedded color profile if available.
	BTCString   m_strTitle;            // [out]
	BTCString   m_strAuthor;           // [out]
	BTCString   m_strDescription;      // [out]
	BTCString   m_strSoftware;         // [out]
	BTCString   m_strCopyright;        // [out]
	BTCOStream *m_pOIPTCStream;        // [out] Used to export IPTC data.
};

/////////////////////////////////////////////////////////////////////////////
// PNG options
/////////////////////////////////////////////////////////////////////////////

class BTNEXGENIPL_CLASS_EXPORT BTCEncoderOptionsPNG : public BTCEncoderOptions
{
public:
	BTCEncoderOptionsPNG();
	virtual ~BTCEncoderOptionsPNG();

	enum {	BTEO_PNG_TITLE = 0,
			BTEO_PNG_AUTHOR,
			BTEO_PNG_DESCRIPTION,
			BTEO_PNG_SOFTWARE,
			BTEO_PNG_COPYRIGHT,
			BTEO_PNG_DISCLAIMER,
			BTEO_PNG_COMMENT,
			BTEO_PNG_TRANSPARENTCOLOR,
			BTEO_PNG_USETRANSPARENTCOLOR,
			BTEO_PNG_INTERLACE };

	virtual BTRESULT GetOption(const long lId, BTColorRGB &rgbValue);
	virtual BTRESULT GetOption(const long lId, BTCString &strValue);
	virtual BTRESULT GetOption(const long lId, bool &bValue);

	virtual BTRESULT SetOption(const long lId, const BTColorRGB rgbValue);
	virtual BTRESULT SetOption(const long lId, const BTCString strValue);
	virtual BTRESULT SetOption(const long lId, const bool bValue);

private:
	BTCString  m_strTitle;             // [in]
	BTCString  m_strAuthor;            // [in]
	BTCString  m_strDescription;       // [in]
	BTCString  m_strSoftware;          // [in]
	BTCString  m_strCopyright;         // [in]
	BTCString  m_strDisclaimer;        // [in]
	BTCString  m_strComment;           // [in]
	BTColorRGB m_rgbTransparentColor;  // [in] Transparent color for palatted and rgb images.
	bool       m_bUseTransparentColor; // [in] Will be set automatically when 'm_rgbTransparentColor' is set.
	bool       m_bInterlace;           // [in] Enable interlacing.
};

class BTNEXGENIPL_CLASS_EXPORT BTCDecoderOptionsPNG : public BTCDecoderOptions
{
public:
	BTCDecoderOptionsPNG();
	virtual ~BTCDecoderOptionsPNG();

	enum {	BTDO_PNG_TRANSPARENTCOLORAVAILABLE = 0,
			BTDO_PNG_TRANSPARENTCOLORINDEX,
			BTDO_PNG_TRANSPARENTCOLOR,
			BTDO_PNG_BACKGROUNDCOLOR,
			BTDO_PNG_USEBACKGROUNDCOLOR,
			BTDO_PNG_TITLE,
			BTDO_PNG_AUTHOR,
			BTDO_PNG_DESCRIPTION,
			BTDO_PNG_SOFTWARE,
			BTDO_PNG_COPYRIGHT,
			BTDO_PNG_DISCLAIMER,
			BTDO_PNG_COMMENT };

	virtual BTRESULT GetOption(const long lId, int &nValue);
	virtual BTRESULT GetOption(const long lId, bool &bValue);
	virtual BTRESULT GetOption(const long lId, BTColorRGB &rgbValue);
	virtual BTRESULT GetOption(const long lId, BTCString &strValue);

	virtual BTRESULT SetOption(const long lId, const int nValue);
	virtual BTRESULT SetOption(const long lId, const bool bValue);
	virtual BTRESULT SetOption(const long lId, const BTColorRGB rgbValue);
	virtual BTRESULT SetOption(const long lId, const BTCString strValue);

private:
	bool       m_bUseBackgroundColor;    // [in]  Will be set automatically when 'm_rgbBackgroundColor' is set.
	BTColorRGB m_rgbBackgroundColor;     // [in]  Color that should be used as background color (Only necessary for paletted images).
	bool       m_bTransparentColor;      // [out] Is transparent color available ?
	int        m_nTransparentColorIndex; // [out] Index of transparent color (only for indexed images).
	BTColorRGB m_rgbTransparentColor;    // [out] Transparent color (only for RGB images).
	BTCString  m_strTitle;               // [out]
	BTCString  m_strAuthor;              // [out]
	BTCString  m_strDescription;         // [out]
	BTCString  m_strSoftware;            // [out]
	BTCString  m_strCopyright;           // [out]
	BTCString  m_strDisclaimer;          // [out]
	BTCString  m_strComment;             // [out]
};

/////////////////////////////////////////////////////////////////////////////
// GIF options
/////////////////////////////////////////////////////////////////////////////

class BTNEXGENIPL_CLASS_EXPORT BTCEncoderOptionsGIF : public BTCEncoderOptions
{
public:
	BTCEncoderOptionsGIF();
	virtual ~BTCEncoderOptionsGIF();

	enum {	BTEO_GIF_TRANSPARENTCOLOR = 0,
			BTEO_GIF_COMMENT };

	virtual BTRESULT GetOption(const long lId, int &nValue);
	virtual BTRESULT GetOption(const long lId, BTCString &strValue);
	
	virtual BTRESULT SetOption(const long lId, const int nValue);
	virtual BTRESULT SetOption(const long lId, const BTCString strValue);

private:
	int       m_nTransparentColor; // [in] Palette index of transparent color.
	BTCString m_strComment;        // [in] Comment
};

class BTNEXGENIPL_CLASS_EXPORT BTCDecoderOptionsGIF : public BTCDecoderOptions
{
public:
	BTCDecoderOptionsGIF();
	virtual ~BTCDecoderOptionsGIF();

	enum {	BTDO_GIF_IMAGEINDEX = 0,
			BTDO_GIF_IMAGECOUNT,
			BTDO_GIF_TRANSPARENTCOLORAVAILABLE,
			BTDO_GIF_TRANSPARENTCOLOR,
			BTDO_GIF_BACKGROUNDCOLOR,
			BTDO_GIF_DELAY,
			BTDO_GIF_COMMENT };

	virtual BTRESULT GetOption(const long lId, int &nValue);
	virtual BTRESULT GetOption(const long lId, bool &bValue);
	virtual BTRESULT GetOption(const long lId, BTCString &strValue);
	virtual BTRESULT GetOption(const long lId, BTColorRGB &rgbValue);
	
	virtual BTRESULT SetOption(const long lId, const int nValue);
	virtual BTRESULT SetOption(const long lId, const bool bValue);
	virtual BTRESULT SetOption(const long lId, const BTCString strValue);
	virtual BTRESULT SetOption(const long lId, const BTColorRGB rgbValue);

private:
	int        m_nImageIndex;         // [in]  Index of requested image.
	int        m_nImageCount;         // [out] Number of images.
	bool       m_bTransparentColor;   // [out] Is transparent color available ?
	BTColorRGB m_rgbTransparentColor; // [out] Transparent color.
	BTColorRGB m_rgbBackgroundColor;  // [out] Background color
	int        m_nDelay;              // [out] Delay
	BTCString  m_strComment;          // [out] Comment
};

/////////////////////////////////////////////////////////////////////////////
// JP2 (JPEG 2000) options
/////////////////////////////////////////////////////////////////////////////

class BTNEXGENIPL_CLASS_EXPORT BTCEncoderOptionsJP2 : public BTCEncoderOptions
{
public:
	BTCEncoderOptionsJP2();
	virtual ~BTCEncoderOptionsJP2();

	enum {	BTEO_JP2_COMPRESSIONMODE = 0,
			BTEO_JP2_COMPRESSIONRATE,
			BTEO_JP2_CODEC };

	enum BTJP2CompressionMode {	BTJP2COMPRESSIONMODE_INT = 0,
								BTJP2COMPRESSIONMODE_REAL };

	enum BTJP2Codec {	BTJP2CODEC_JP2 = 0, // File Format Syntax
						BTJP2CODEC_JPC };   // Code Stream Syntax

	virtual BTRESULT GetOption(const long lId, int &nValue);
	virtual BTRESULT GetOption(const long lId, double &dValue);
	
	virtual BTRESULT SetOption(const long lId, const int nValue);
	virtual BTRESULT SetOption(const long lId, const double dValue);

private:
	int    m_nCodec;           // [in] Codec used to encode image.
	int    m_nCompressionMode; // [in] Compression mode (Real or Int).
	double m_dCompressionRate; // [in] e.g. 100:1 = 0.01, 200:1 = 0.005.
};

#endif // BTCOPTIONS_INCLUDED

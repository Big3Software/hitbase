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

#ifndef BTCIMAGEDATA_INCLUDED
#define BTCIMAGEDATA_INCLUDED

#pragma once

#include "BTNexgenIPL.h"
#include "BTDefines.h"

class BTCHistogram;

class BTNEXGENIPL_CLASS_EXPORT BTCImageData
{
public:
	enum BTChannel         { Red,
                             Green,
                             Blue,
                             Alpha };

	enum BTDirection       { Left,
                             Right };

	enum BTResizeFilter    { Default,
                             Box,
                             Bilinear,
                             Gaussian,
                             Hamming,
                             Blackman,
                             Triangle,
                             Bell,
                             BSpline,
                             Sinc,
                             Lanczos3,
                             Mitchell,
                             Quadratic,
                             Hermite,
                             Hanning,
                             Catrom,
                             Cubic,
                             Hann4 };

	enum BTDeInterlaceType { Interpolate,
                             Duplicate };

	enum BTCombineFunction { Normal,
                             Add,
                             Subtract,
                             Multiply,
                             Divide,
                             Difference,
                             Lightest,
                             Darkest,
                             Average,
                             Or,
                             And,
                             Screen,
                             Overlay };

	enum BTNoiseType       { UniformNoise = 0,
                             LaplacianNoise,
                             PoissonNoise,
                             GaussianNoise,
                             MultiplicativeGaussianNoise,
                             ImpulseNoise };

	enum { BTCMP_IMAGE     = 0x00000001,   // Images are different.
           BTCMP_NUMCOLORS = 0x00000002,   // Number of colours in palette differ.
           BTCMP_COLOR     = 0x00000004,   // Image colours differ.
           BTCMP_SIZE_X    = 0x00000008,   // Image width differs.
           BTCMP_SIZE_Y    = 0x00000010 }; // Image heights differ.

	static int GetNumColorEntries(int nBitsPerPixel);
	static int GetColorTableSize(int nNumColorEntries);
	static long GetBytesPerLine(int nWidth, int nBitsPerPixel);
	static long GetImageSize(int nWidth, int nHeight, int nBitsPerPixel);


	BTCImageData();
	BTCImageData(const BTCImageData& ImageData);

	virtual ~BTCImageData();

	bool Create(int nWidth, int nHeight, int nBitsPerPixel = 8);

	void DeleteObject();
	bool IsLoaded() const;

	BTCImageData& operator=(BTCImageData &ImageData);

	int GetPlanes() const;
	int GetDepth() const;
	int GetBytesPerPixel() const;
	int GetBitsPerPixel() const;
	long GetBytesPerLine() const;
	long GetImageSize() const;
	long GetWidth() const;
	long GetHeight() const;

	BTResolution GetResolution(const enum BTResolutionMetric& Metric = BTMETRIC_METER) const;

	LPBITMAPINFO GetBitmapInfo();
	LPBITMAPINFOHEADER GetBitmapInfoHeader();

	unsigned char* GetBits();
	unsigned char* GetBits( int x, int y);


	int GetIndexFromColor(BTCOLORREF col);
	int GetIndexFromPixel(int x, int y);
	BTCOLORREF GetColorFromIndex(int nIndex);
	BTCOLORREF GetColorFromPixel(int x, int y);

	bool SetColorForIndex(int nIndex, BTCOLORREF col);
	bool SetColorForPixel(int x, int y, int nIndex);
	bool SetColorForPixel(int x, int y, BTCOLORREF col);
	bool SetColorForColor(BTCOLORREF oldCol, BTCOLORREF newCol);

	bool IsColorInColorTable(BTCOLORREF col);
	int FindNearestPaletteColor(BTCOLORREF col);
	bool SwapPaletteIndex(unsigned char nIndex1, unsigned char nIndex2);

	RGBQUAD* GetColorTable();
	int GetColorTableSize() const;
	int GetNumColorEntries() const;
	bool FillColorTable(const RGBQUAD* pPalette, int iEntries);

	HPALETTE GetPalette() const;
	bool SetPalette(HPALETTE hPal);


	HANDLE GetDIB();     // Must be freed after usage.
	HBITMAP GetBitmap(); // Must be freed after usage.

	bool FromDC(HDC hdc, int nDestWidth, int nDestHeight, int xSrc, int ySrc, int nSrcWidth, int nSrcHeight, unsigned long dwRop = SRCCOPY);
	bool FromHandle(HBITMAP hBitmap);
	bool FromDIB(HANDLE hDib);


	int Compare(BTCImageData* pImageData);

	bool GetHistogram(BTCHistogram* pHistogram);
	bool GetChannel(BTCImageData* pImage, const enum BTChannel& Channel); // Destination image given as parameter will be created automatically.
	bool SetChannel(BTCImageData* pImage, const enum BTChannel& Channel);
	bool SplitChannels(BTCImageData* pRed, BTCImageData* pGreen, BTCImageData* pBlue, BTCImageData* pAlpha);
	bool CombineChannels(BTCImageData* pRed, BTCImageData* pGreen, BTCImageData* pBlue, BTCImageData* pAlpha); // All images must have same size and color depth. Destination image (current object) must be initialized with Create(...) before using his method.


	bool ConvertTo24BPP();
	bool ConvertTo32BPP();

	bool Quantize(int nBitsPerPixel = 8);

	bool Resize(int nWidth, int nHeight, const enum BTResizeFilter& Filter = Bilinear);
	bool Resize(BTCImageData& DestImage, int nWidth, int nHeight, const enum BTResizeFilter& Filter = Bilinear);

	bool Crop(const RECT& rect);
	bool Crop(int x, int y, int nWidth, int nHeight);
	bool Copy(BTCImageData& SrcImage, const RECT& rect);
	bool Copy(BTCImageData& SrcImage, int x, int y, int nWidth, int nHeight);
	bool Fill(const RECT& rect, BTCOLORREF col);
	bool Fill(int x, int y, int w, int h, BTCOLORREF col);
	bool Blend(BTCImageData& ImageData, int x, int y, int a, BTCOLORREF colProtect = 0x01000000);
	bool Combine(BTCImageData *pImage, enum BTCombineFunction cf, int x = 0, int y = 0, int nOpacity = 100, bool bClipValues = true, int nDivisor = 1, int nBias = 0);

	bool Blur(); // Only for 32 bpp images.
	bool Grayscale();
	bool Negate();
	bool Flip();
	bool Mirror();
	bool Rotate(double dAngle, const enum BTDirection& Direction = Right, BTCOLORREF colBackground = 0x00000000);
	bool Rotate90(const enum BTDirection& Direction = Right);
	bool Smooth(int nKernelSize = 3);     // Only for 24 and 32 bpp images.
	bool Normalize();
	bool Posterize(int nLevels = 7);
	bool Threshold(int nThreshold = 166);
	bool Emboss();                        // Only for 24 and 32 bpp images.
	bool Detail();                        // Only for 24 and 32 bpp images.
	bool EdgeEnhance();                   // Only for 24 and 32 bpp images.
	bool EdgeEnhanceMore();               // Only for 24 and 32 bpp images.
	bool Median(int nAperature = 1);      // Only for 24 and 32 bpp images.
	bool Erode(int nAperature = 1);       // Only for 24 and 32 bpp images.
	bool Dilate(int nAperature = 1);      // Only for 24 and 32 bpp images.
	bool FindEdge(double dFactor = 60.0); // Only for 24 and 32 bpp images.
	bool Sharpen();                       // Only for 24 and 32 bpp images.
	bool Solarize(double dFactor = 0.0);
	bool Antialias(double dWeight = 1.0 / 3.0, int nSameThreshold = 25, int nDiffThreshold = 25); // Only for 24 and 32 bpp images.
	bool DeInterlace(bool bRetainOdd = true, const enum BTDeInterlaceType& Type = Interpolate);
	bool Noise(BTNoiseType Type);         // Only for 24 and 32 bpp images.
	bool OilPaint(int nRadius = 2, int nSmoothness = 85);

	bool Convolve(const double* pKernel, int nKernelSize, int nDivisor = 1, int nBias = 0, bool bAbs = false); // Only for 24 and 32 bpp images.

	bool AdjustBrightness(int nPercentage);
	bool AdjustContrast(int nPercentage);
	bool AdjustHighlight(int nPercentage);
	bool AdjustMidtone(int nPercentage);
	bool AdjustShadow(int nPercentage);
	bool AdjustGamma(double dRedValue = 1.0, double dGreenValue = 1.0, double dBlueValue = 1.0);
	bool AdjustHue(int nDegrees);
	bool AdjustLightness(int nPercentage);
	bool AdjustSaturation(int nPercentage);
	bool AdjustHLS(int nHueDegrees = 0, int nLightnessPercentage = 0, int nSaturationPercentage = 0);
	bool AdjustRed(int nPercentage);
	bool AdjustGreen(int nPercentage);
	bool AdjustBlue(int nPercentage);
	bool AdjustRGB(int nRedPercentage = 0, int nGreenPercentage = 0, int nBluePercentage = 0);


	int Stretch(HDC hdc, int xDest, int yDest, int nDestWidth, int nDestHeight, int xSrc, int ySrc, int nSrcWidth, int nSrcHeight, int iMode = COLORONCOLOR, unsigned long dwRop = SRCCOPY);
	int Draw(HDC hdc, int xDest, int yDest, int nDestWidth, int nDestHeight, int xSrc, int ySrc, int iMode = COLORONCOLOR, unsigned long dwRop = SRCCOPY);
	int DrawDithered(HDC hdc, int xDest, int yDest, int nDestWidth, int nDestHeight, int xSrc, int ySrc);

	bool StretchTransparent(HDC hdc, int xDest, int yDest, int nDestWidth, int nDestHeight, int xSrc, int ySrc, int nSrcWidth, int nSrcHeight, BTCOLORREF crColor);
	bool DrawTransparent(HDC hdc, int xDest, int yDest, int nDestWidth, int nDestHeight, int xSrc, int ySrc, BTCOLORREF crColor);
	bool DrawSemiTransparent(HDC hdc, int xDest, int yDest, int nDestWidth, int nDestHeight, int xSrc, int ySrc, BTCOLORREF crColor);

protected:
	BITMAPINFO*    m_pBitmapInfo; // Pointer to bitmap info.
    unsigned char* m_pBits;       // Pointer to bitmap bits.
};

class BTNEXGENIPL_CLASS_EXPORT BTCHistogram
{
public:
	enum BTChannel { Red,
                     Green,
                     Blue };

	BTCHistogram();
	virtual ~BTCHistogram();

	bool Initialize(BTCImageData* pImage);

	long GetNumPixels() const;
	double GetMeanValue(const enum BTChannel& Channel) const;

protected:
	long m_r[256];
	long m_g[256];
	long m_b[256];

	long m_lNumPixel; // Total number of pixels in image.
};

#endif // BTCIMAGEDATA_INCLUDED

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

#ifndef BTCCOLOR_INCLUDED
#define BTCCOLOR_INCLUDED

#pragma once

#include "BTNexgenIPL.h"
#include "BTDefines.h"

typedef struct tagBTColorHSB
{
	double Hue;
	double Saturation;
	double Brightness;
} BTColorHSB;

typedef struct tagBTColorHSL
{
	double Hue;
	double Saturation;
	double Lightness;
} BTColorHSL;

typedef struct tagBTColorRGB
{
	unsigned char Red;
	unsigned char Green;
	unsigned char Blue;
} BTColorRGB;

// +------------+----------------------+-----------------------+-------+
// | ColorSpace | Number of components |  Range of components  | Gamma |
// +------------+----------------------+-----------------------+-------+
// | LinearRGB  |          3           | R/G/B:   0 ... 1      |  1.0  |
// | sRGB       |          3           | R/G/B:   0 ... 1      |  2.2  |
// | CIEXYZ     |          3           | X:       0 ... 0.9642 |  2.2  |
// |            |                      | Y:       0 ... 1.0    |       |
// |            |                      | Z:       0 ... 0.8251 |       |
// | CIELab     |          3           | L*:      0 ... 100    |  2.2  |
// |            |                      | a*:   -128 ... 127    |       |
// |            |                      | b*:   -128 ... 127    |       |
// | CIEYxy     |          3           | Y:       0 ... 100    |  2.2  |
// |            |                      | x:       0 ... 1      |       |
// |            |                      | y:       0 ... 1      |       |
// | CMYK       |          4           | C/M/Y/K: 0 ... 1      |  2.2  |
// | HSB        |          3           | H:       0 ... 360    |  2.2  |
// |            |                      | S:       0 ... 1      |       |
// |            |                      | B:       0 ... 1      |       |
// | HSL        |          3           | H:       0 ... 360    |  2.2  |
// |            |                      | S:       0 ... 1      |       |
// |            |                      | L:       0 ... 1      |       |
// | Gray       |          1           | K:       0 ... 1      |  2.2  |
// +------------+----------------------+-----------------------+-------+

enum BTColorSpace
{
	BTColorSpace_LinearRGB = 0,
	BTColorSpace_sRGB,   // ITU-R BT.709 (D65)
	BTColorSpace_CIEXYZ, // (D50)
	BTColorSpace_CIELab,
	BTColorSpace_CIEYxy,
	BTColorSpace_CMYK,
	BTColorSpace_HSB,
	BTColorSpace_HSL,
	BTColorSpace_Gray,   // According to ITU-R BT.709 (Rec 709)
	BTColorSpace_External
};

class BTIColorSpace
{
public:
	BTIColorSpace() {};
	virtual ~BTIColorSpace() {};

	// Returns a BTIColorSpace representing one of the specific predefined color spaces.
	static BTNEXGENIPL_CLASS_EXPORT BTIColorSpace *GetColorSpace(BTColorSpace cs);

	// Returns the number of components of this color space.
	virtual int GetNumComponents() = 0;

	// Returns the color space type of this color space.
	virtual BTColorSpace GetType() = 0;

	// Transforms a color value assumed to be in the CIEXYZ (D50) conversion color space into this color space.
	virtual bool FromCIEXYZ(double xyz[3], double *col) = 0;

	// Transforms a color value assumed to be in the default sRGB color space into this color space.
	virtual bool FromRGB(double rgb[3], double *col) = 0;

	// Transforms a color value assumed to be in this color space into the CIEXYZ (D50) conversion color space.
	virtual bool ToCIEXYZ(double col[], double *xyz) = 0;

	// Transforms a color value assumed to be in this color space into a value in the default sRGB color space.
	virtual bool ToRGB(double col[], double *rgb) = 0;
};

class BTIColorManagementSystem
{
public:
	BTIColorManagementSystem() {};
	virtual ~BTIColorManagementSystem() {};

	static BTNEXGENIPL_CLASS_EXPORT bool Initialize();
	static BTNEXGENIPL_CLASS_EXPORT bool IsInitialized();
	static BTNEXGENIPL_CLASS_EXPORT void Terminate();

	static BTNEXGENIPL_CLASS_EXPORT BTIColorManagementSystem *GetColorManagementSystem();

	virtual bool LinkColorSpace(BTIColorSpace* pIColorSpace) = 0;
	virtual bool UnlinkColorSpace(const enum BTColorSpace cs) = 0;
};

class BTNEXGENIPL_CLASS_EXPORT BTCColor
{
public:
	BTCColor();
	virtual ~BTCColor();

	static BTColorHSB RGBtoHSB(const BTColorRGB rgb);
	static BTColorHSB RGBtoHSB(const unsigned char r, const unsigned char g, const unsigned char b);

	static BTColorRGB HSBtoRGB(const BTColorHSB hsb);
	static BTColorRGB HSBtoRGB(const double h, const double s, const double b);

	static BTColorHSL RGBtoHSL(const BTColorRGB rgb);
	static BTColorHSL RGBtoHSL(const unsigned char r, const unsigned char g, const unsigned char b);

	static BTColorRGB HSLtoRGB(const BTColorHSL hsl);
	static BTColorRGB HSLtoRGB(const double h, const double s, const double b);

	static double GetHue(BTColorRGB rgb);
	static double GetLightness(BTColorRGB rgb);
	static double GetSaturation(BTColorRGB rgb);
	static double GetBrightness(BTColorRGB rgb);
	static double GetLuminance(BTColorRGB rgb);
	static double GetIntensity(BTColorRGB rgb);
};

#endif // BTCCOLOR_INCLUDED

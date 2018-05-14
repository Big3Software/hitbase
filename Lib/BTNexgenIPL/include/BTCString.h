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

#ifndef BTCSTRING_INCLUDED
#define BTCSTRING_INCLUDED

#pragma once

#include "BTDefines.h"
#include "BTNexgenIPL.h"

class BTNEXGENIPL_CLASS_EXPORT BTCString
{
public:
	BTCString();
	BTCString(const BTCString& str);
	BTCString(const bt_char* str);
	BTCString(const bt_char* str, const unsigned long ulLength);
/*#ifdef _UNICODE
	BTCString(const char* str);
	BTCString(const char* str, const unsigned long ulLength);
#endif*/
	virtual ~BTCString();

	const BTCString& operator  =(const bt_char* str);
	const BTCString& operator  =(const BTCString& str);
	const BTCString& operator +=(const bt_char* str);
	const BTCString& operator +=(const BTCString& str);
	const BTCString& operator +=(const bt_char ch);
/*#ifdef _UNICODE
	const BTCString& operator +=(const char ch);
#endif*/

	virtual bool operator !=(const BTCString& str);
	virtual bool operator ==(const BTCString& str);

	operator bt_char*() const;
	operator const bt_char*() const;

	void Empty();
	unsigned long GetLength() const;
	bool IsEmpty() const;

	int Compare(const BTCString& str) const;
	int CompareNoCase(const BTCString& str) const;

protected:
	unsigned long m_ulLength;
	bt_char* m_pString;
};

/////////////////////////////////////////////////////////////////////////////
// Internal helper class to convert unicode strings into ansi.
/////////////////////////////////////////////////////////////////////////////

class BTCStringA
{
public:
	BTCStringA();
	BTCStringA(const unsigned long ulLength);
	BTCStringA(const char* str);
	BTCStringA(const char* str, const unsigned long ulLength);
	BTCStringA(const BTCString& str);
	BTCStringA(const BTCStringA& str);
	virtual ~BTCStringA();

	const BTCStringA& operator  =(const char* str);
	const BTCStringA& operator  =(const BTCString& str);
	const BTCStringA& operator  =(const BTCStringA& str);
	const BTCStringA& operator +=(const char* str);
	const BTCStringA& operator +=(const BTCStringA& str);

	operator char*() const;
	operator const char*() const;

	BTRESULT Format(const char* strFormat, ...);

	void Empty();
	unsigned long GetLength() const;
	bool IsEmpty() const;

protected:
	unsigned long m_ulLength;
	char* m_pString;
};

#endif // BTCSTRING_INCLUDED

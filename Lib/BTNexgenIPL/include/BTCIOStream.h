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

#ifndef BTCIOSTREAM_INCLUDED
#define BTCIOSTREAM_INCLUDED

#pragma once

#include "BTDefines.h"

class BTCIOStream
{
public:
	enum Position	{	Begin    = 1,
						Current  = 2,
						End      = 3 };

	BTCIOStream() {};
	virtual ~BTCIOStream() {};

	/////////////////////////////////////////////////////////////////////////////
	// Seek returns the offset, in bytes, of the new position from the
	// beginning of the stream.
	// A return value of -1 indicates an error.
	/////////////////////////////////////////////////////////////////////////////

	virtual long Seek(long lOffset, unsigned int nOrigin) = 0;

	/////////////////////////////////////////////////////////////////////////////
	// Check the stream if it is open.
	/////////////////////////////////////////////////////////////////////////////

	virtual bool IsOpen() = 0;

	/////////////////////////////////////////////////////////////////////////////
	// Close the stream. Close returns 0 if the stream was successfully closed.
	// A return value of -1 indicates an error.
	/////////////////////////////////////////////////////////////////////////////

	virtual int Close() = 0;
};

class BTCOStream : public BTCIOStream
{
public:

	/////////////////////////////////////////////////////////////////////////////
	// If successful, Write returns the number of bytes actually written.
	// A return value of -1 indicates an error. Also a return value less than
	// count indicates an error.
	/////////////////////////////////////////////////////////////////////////////

	virtual long Write(const void* pData, long lCount) = 0;
};

class BTCIStream : public BTCIOStream
{
public:

	/////////////////////////////////////////////////////////////////////////////
	// Read returns the number of bytes read, which may be less than count if
	// there are fewer than count bytes left in the stream.
	// A return value of -1 indicates an error.
	/////////////////////////////////////////////////////////////////////////////

	virtual long Read(void* pData, long lCount) = 0;

	/////////////////////////////////////////////////////////////////////////////
	// Retrieves the current stream pointer.
	/////////////////////////////////////////////////////////////////////////////

	virtual long GetPosition() = 0;

	/////////////////////////////////////////////////////////////////////////////
	// Retrieves the current size of the stream.
	/////////////////////////////////////////////////////////////////////////////

	virtual long GetSize() = 0;
};

#endif // BTCIOSTREAM_INCLUDED

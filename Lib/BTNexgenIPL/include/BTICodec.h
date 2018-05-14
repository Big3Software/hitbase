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

#ifndef BTICODEC_INCLUDED
#define BTICODEC_INCLUDED

#pragma once

#include "BTCIOStream.h"
#include "BTCImageData.h"
#include "BTIProgressNotification.h"
#include "BTCOptions.h"

class BTICodec
{
public:
	BTICodec() : m_lId(-1) {};
	virtual ~BTICodec() {};

	void Initialize(long lId) { m_lId = lId; }
	long GetId() const { return m_lId; }

	virtual const bt_char *GetFormat() = 0;
	virtual const bt_char *GetDescription() = 0;
	virtual const bt_char *GetExtension() = 0;

	virtual const BTCodecType GetCodecType() = 0;

protected:
	long m_lId;
};

class BTCDecoder : public BTICodec
{
public:
	virtual BTRESULT IsValidType(BTCIStream* pIStream) = 0;

	virtual bool Load(	BTCIStream*              pIStream,
						BTCImageData*            pImageData,
						BTIProgressNotification* pProgressNotification,
						BTCDecoderOptions*       pOptions) = 0;

	virtual const BTCodecType GetCodecType() { return BTCODECTYPE_DECODER; }
};

class BTCEncoder : public BTICodec
{
public:
	virtual bool Save(	BTCOStream*              pOStream,
						BTCImageData*            pImageData,
						BTIProgressNotification* pProgressNotification,
						BTCEncoderOptions*       pOptions) = 0;

	virtual const BTCodecType GetCodecType() { return BTCODECTYPE_ENCODER; }
};

class BTICodecList
{
public:
	BTICodecList() {};
	virtual ~BTICodecList() {};

	virtual bool First() = 0;
	virtual bool Next() = 0;
	virtual bool IsDone() = 0;

	virtual BTICodec* Current() = 0;

	virtual long Size() = 0;
};

#endif // BTICODEC_INCLUDED

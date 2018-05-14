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

#ifndef BTCIMAGEOBJECT_INCLUDED
#define BTCIMAGEOBJECT_INCLUDED

#pragma once

#include "BTNexgenIPL.h"

#include "BTDefines.h"
#include "BTCImageData.h"
#include "BTCIOStream.h"
#include "BTICodec.h"
#include "BTIProgressNotification.h"

class BTNEXGENIPL_CLASS_EXPORT BTCImageObject  
{
public:
	BTCImageObject();
	BTCImageObject(const BTCImageObject& ImageObject);

	virtual ~BTCImageObject();

	bool Attach(BTCImageData& ImageData);

	BTCImageData GetObjectData() const;
	BTCImageData* GetObjectDataPtr();

	BTRESULT AddCodec(BTICodec* pCodec);
	BTRESULT RemoveCodec(long lId, const enum BTCodecType& CodecType);
	long GetCodecIdFromFormat(const BTCString strFormat, const enum BTCodecType& CodecType);
	long GetCodecIdFromExtension(const BTCString strExtension, const enum BTCodecType& CodecType);
	BTICodecList *GetCodecs(const enum BTCodecType& CodecType);

	virtual long GetFileType(BTCIStream* pIStream);
	virtual long Load(BTCIStream* pIStream, BTCDecoderOptions* pOptions);
	virtual BTRESULT Save(BTCOStream* pOStream, long lCodecId, BTCEncoderOptions* pOptions);

	long GetFileType(unsigned char* pData, long lSize);
	long GetFileType(unsigned int nResource, HANDLE hModule = NULL, const bt_char *lpType = (char*)RT_BITMAP);
	long GetFileType(const bt_char *lpszResource, HANDLE hModule = NULL, const bt_char *lpType = (char*)RT_BITMAP);

	long Load(unsigned char* pData, long lSize, BTCDecoderOptions* pOptions = NULL);
	long Load(unsigned int nResource, BTCDecoderOptions* pOptions = NULL, HANDLE hModule = NULL, const bt_char *lpType = (char*)RT_BITMAP);
	long Load(const bt_char *lpszResource, BTCDecoderOptions* pOptions = NULL, HANDLE hModule = NULL, const bt_char *lpType = (char*)RT_BITMAP);

	BTRESULT Save(const BTCString strFileName, long lCodecId, BTCEncoderOptions* pOptions = NULL);

	void SetProgressNotification(BTIProgressNotification* pProgressNotification);

protected:
	void*                    m_pCodecFactory;

	BTCImageData             m_ImageData;
	BTIProgressNotification* m_pProgressNotification;
};

#endif // BTCIMAGEOBJECT_INCLUDED

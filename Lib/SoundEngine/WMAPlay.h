//
//  Microsoft Windows Media Technologies
//  Copyright (C) 1999 Microsoft Corporation.  All rights reserved.
//
// You have a royalty-free right to use, modify, reproduce and distribute
// the Sample Application Files (including modified versions) in any way 
// you determine to be useful, provided that you agree that Microsoft 
// Corporation provides no warrant or support, and has no obligation or
// liability resulting from the use of any Sample Application Files. 
//
#ifndef SIMPLEPLAYER_H
#define SIMPLEPLAYER_H

//#include "dsound.h"

#define WMAUDIO_UNITS 10000

class CWMAEngine;

///////////////////////////////////////////////////////////////////////////////
class CWMAPlay : public IWMReaderCallback
{
public:
    CWMAPlay(CWMAEngine* pWMAEngine);
    ~CWMAPlay();
    
	virtual HRESULT Open(LPCWSTR pszUrl);
	virtual HRESULT Close();
    virtual HRESULT Play( DWORD dwMSStart, DWORD dwMSEnd, HANDLE hCompletionEvent, HRESULT *phrCompletion );
	virtual HRESULT Stop();
	virtual HRESULT Pause();
	virtual HRESULT Seek(DWORD dwPosition);
	virtual DWORD   GetCurrentPosition();

// General tags
public:
	CString m_sDescription;
	CString m_sRating;
	CString m_sCopyright;
	DWORD   m_dwBitrate;

    // IUnknown
public:
    virtual HRESULT STDMETHODCALLTYPE QueryInterface(REFIID riid, void **ppvObject);

    virtual ULONG STDMETHODCALLTYPE AddRef();
    virtual ULONG STDMETHODCALLTYPE Release();

    // IWMAudioReadCallback
public:
	virtual void SetOutputDevice(int iDevice);
	BOOL ReadInformation();

    virtual HRESULT STDMETHODCALLTYPE OnSample( 
        /* [in] */ DWORD dwOutputNum,
        /* [in] */ QWORD cnsSampleTime,
        /* [in] */ QWORD cnsSampleDuration,
        /* [in] */ DWORD dwFlags,
        /* [in] */ INSSBuffer __RPC_FAR *pSample,
        /* [in] */ void __RPC_FAR *pvContext);
    
    virtual HRESULT STDMETHODCALLTYPE OnStatus( 
        /* [in] */ WMT_STATUS Status,
        /* [in] */ HRESULT hr,
        /* [in] */ WMT_ATTR_DATATYPE dwType,
        /* [in] */ BYTE __RPC_FAR *pValue,
        /* [in] */ void __RPC_FAR *pvContext);

protected:

    void OnWaveOutMsg( UINT uMsg, DWORD dwParam1, DWORD dwParam2 );

    static void CALLBACK WaveProc(HWAVEOUT hwo, UINT uMsg, DWORD dwInstance, DWORD dwParam1, DWORD dwParam2);

    LONG    m_cRef;
    LONG    m_cBuffersOutstanding;
    BOOL    m_fEof;
    HANDLE  m_hCompletionEvent;

    IWMReader *m_pReader;
    HWAVEOUT m_hwo;

	LPDIRECTSOUND m_pDirectSound;
	LPDIRECTSOUNDBUFFER m_pDirectSoundBuffer;

    HRESULT *m_phrCompletion;

    union
    {
        WAVEFORMATEX m_wfx;
        BYTE m_WfxBuf[1024];
    };

    LPWSTR  m_pszUrl;

	DWORD m_dwCurrentPosition;

	int m_iOutputDevice;
	HANDLE m_hOpenEvent;
	HANDLE m_hCloseEvent;
	BOOL m_bFirstTime;
	BOOL m_bPauseActive;

	CWMAEngine* m_pWMAEngine;

private:
	HRESULT GetHeaderAttribute(LPCWSTR pwszName, BYTE** ptrValue, IWMHeaderInfo* pHeaderInfo);
	void WaitForEvent(HANDLE hEvent);
};

#endif // SIMPLEPLAYER_H

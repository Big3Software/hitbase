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

#include "stdafx.h"
#include <shellapi.h>
#include "SoundEngine.h"
#include "WMAEngine.h"
#include "WMAPlay.h"
#include "../PlugInEngine/SoundVisualizer.h"

///////////////////////////////////////////////////////////////////////////////
CWMAPlay::CWMAPlay(CWMAEngine* pWMAEngine)
{
    m_cRef = 1;
    m_cBuffersOutstanding = 0;

    m_pReader = NULL;
    m_hwo = NULL;

    m_fEof = FALSE;
    m_pszUrl = NULL;

    m_hOpenEvent = CreateEvent( NULL, FALSE, FALSE, NULL );
    m_hCloseEvent = CreateEvent( NULL, FALSE, FALSE, NULL );

	m_dwCurrentPosition = 0;
	m_bFirstTime = FALSE;
	m_bPauseActive = FALSE;

	m_pWMAEngine = pWMAEngine;
}


///////////////////////////////////////////////////////////////////////////////
CWMAPlay::~CWMAPlay()
{
//    ASSERT( 0 == m_cBuffersOutstanding );

    if( m_pReader != NULL )
    {
        m_pReader->Release();
        m_pReader = NULL;
    }

    if( m_hwo != NULL )
    {
        waveOutClose( m_hwo );
    }

    delete [] m_pszUrl;

    CloseHandle( m_hOpenEvent );
    CloseHandle( m_hCloseEvent );
}


///////////////////////////////////////////////////////////////////////////////
HRESULT STDMETHODCALLTYPE CWMAPlay::QueryInterface(REFIID riid, void **ppvObject)
{
    return( E_NOINTERFACE );
}


///////////////////////////////////////////////////////////////////////////////
ULONG STDMETHODCALLTYPE CWMAPlay::AddRef()
{
    return( InterlockedIncrement( &m_cRef ) );
}


///////////////////////////////////////////////////////////////////////////////
ULONG STDMETHODCALLTYPE CWMAPlay::Release()
{
    ULONG uRet = InterlockedDecrement( &m_cRef );

    if( 0 == uRet )
    {
        delete this;
    }

    return( uRet );
}


///////////////////////////////////////////////////////////////////////////////
HRESULT STDMETHODCALLTYPE CWMAPlay::OnSample( 
        /* [in] */ DWORD dwOutputNum,
        /* [in] */ QWORD cnsSampleTime,
        /* [in] */ QWORD cnsSampleDuration,
        /* [in] */ DWORD dwFlags,
        /* [in] */ INSSBuffer __RPC_FAR *pSample,
        /* [in] */ void __RPC_FAR *pvContext)
{
    HRESULT hr = S_OK;

	m_dwCurrentPosition = (DWORD)(cnsSampleTime / WMAUDIO_UNITS);    // Nanosekunden in Millisekunden umrechnen!

	// Das Hauptprogramm soll erst weiterlaufen, wenn der Sound wirklich spielt, damit
	// die CurrentPosition stimmt!
/*	if (m_bFirstTime)
	{
		SetEvent( m_hEvent );
		m_bFirstTime = FALSE;
	}*/

	switch (m_iOutputDevice)
	{
	case POD_WAVEOUTPUT:
		{
			HRESULT hr = S_OK;
			BYTE *pData;
			DWORD cbData;

			hr = pSample->GetBufferAndLength( &pData, &cbData );
			if ( FAILED( hr ) )
			{
				return( E_UNEXPECTED );
			}

			LPWAVEHDR pwh = (LPWAVEHDR) new BYTE[ sizeof( WAVEHDR ) + cbData ];

			if( NULL == pwh )
			{
				ErrorBox( L"OnSample OUT OF MEMORY!" );

				*m_phrCompletion = E_OUTOFMEMORY;
				SetEvent( m_hCompletionEvent );
				return( E_UNEXPECTED );
			}

			pwh->lpData = (LPSTR)&pwh[1];
			pwh->dwBufferLength = cbData;
			pwh->dwBytesRecorded = cbData;
			pwh->dwUser = 0;
			pwh->dwLoops = 0;
			pwh->dwFlags = 0;

			CopyMemory( pwh->lpData, pData, cbData );

			MMRESULT mmr;

			mmr = waveOutPrepareHeader( m_hwo, pwh, sizeof(WAVEHDR) );
			mmr = MMSYSERR_NOERROR;

			if( mmr != MMSYSERR_NOERROR )
			{
				delete pwh;

				ErrorBox( L"failed to prepare wave buffer, error=%lu", mmr );
				*m_phrCompletion = E_UNEXPECTED;
				SetEvent( m_hCompletionEvent );
				return( E_UNEXPECTED );
			}

			mmr = waveOutWrite( m_hwo, pwh, sizeof(WAVEHDR) );
			mmr = MMSYSERR_NOERROR;

			if( mmr != MMSYSERR_NOERROR )
			{
				delete pwh;

				ErrorBox( L"failed to write wave sample, error=%lu", mmr );
				*m_phrCompletion = E_UNEXPECTED;
				SetEvent( m_hCompletionEvent );
				return( E_UNEXPECTED );
			}

			TRACE1("Buffer length: %d\n", cbData);

			InterlockedIncrement( &m_cBuffersOutstanding );

			CSoundVisualizer SoundVisualizer;
			unsigned char *p = new unsigned char[cbData/2];
			unsigned short* sp = (unsigned short*)pData;
			for (DWORD i=0;i<cbData/2;i++)
			{
				p[i] = sp[i]/256;
			}
			SoundVisualizer.AccessWaveData((const char* )p, cbData/2);
			delete p;

			return( S_OK );
		}
	case POD_DIRECTSOUND:
		{
/*			DSBUFFERDESC DSBufferDesc;
			LPDIRECTSOUNDBUFFER pDirectSoundBuffer;
			WAVEFORMATEX wfxFormat;

			wfxFormat.cbSize = sizeof(WAVEFORMATEX);
			wfxFormat.nChannels = 2;
			wfxFormat.nSamplesPerSec = 44100;
			wfxFormat.wBitsPerSample = 8;
			wfxFormat.wFormatTag = WAVE_FORMAT_PCM;
			wfxFormat.nAvgBytesPerSec = wfxFormat.nSamplesPerSec * wfxFormat.nChannels * wfxFormat.wBitsPerSample / 8;

			::ZeroMemory(&DSBufferDesc, sizeof(DSBUFFERDESC));
			DSBufferDesc.dwSize = sizeof(DSBUFFERDESC);
			DSBufferDesc.dwBufferBytes = pSample;
//			DSBufferDesc.lpwfxFormat = &wfxFormat;
			DSBufferDesc.lpwfxFormat = NULL;

			HRESULT hr;
			hr = m_pDirectSound->CreateSoundBuffer(&DSBufferDesc, &pDirectSoundBuffer, NULL);

			char* pFirstBuff;
			char* pSecondBuff;
			DWORD dwFirstLength, dwSecondLength;
			pDirectSoundBuffer->Lock(0, 0, (void**)&pFirstBuff, &dwFirstLength, (void**)&pSecondBuff, &dwSecondLength, DSBLOCK_ENTIREBUFFER);

			memcpy(pFirstBuff, pData, cbData);

			pDirectSoundBuffer->Unlock((void**)&pFirstBuff, dwFirstLength, (void**)&pSecondBuff, dwSecondLength);

			pDirectSoundBuffer->Play(0, 0, 0);*/
			break;
		}
	}

    InterlockedIncrement( &m_cBuffersOutstanding );

    return( S_OK );
}

///////////////////////////////////////////////////////////////////////////////
HRESULT CWMAPlay::Play( DWORD dwMSStart, DWORD dwMSEnd, HANDLE hCompletionEvent, HRESULT *phrCompletion )
{
	if (m_bPauseActive)
	{
		if (m_pReader)
			m_pReader->Resume();

		m_bPauseActive = FALSE;

		return S_OK;
	}

	HRESULT hr;

    //
    // Attempt to open the URL
    //
    m_hCompletionEvent = hCompletionEvent;

    m_phrCompletion = phrCompletion;

    //
    // Make sure we're audio only
    //
    DWORD cOutputs;

    hr = m_pReader->GetOutputCount( &cOutputs );
    if ( FAILED( hr ) )
    {
        ErrorBox( L"failed GetOutputCount(), hr=0x%1X", hr );
        return( hr );
    }

    if ( cOutputs != 1 )
    {
        ErrorBox( L"Not audio only (cOutputs = %d).", cOutputs );
        return( E_UNEXPECTED );
    }

    IWMOutputMediaProps *pProps;
    hr = m_pReader->GetOutputProps( 0, &pProps );
    if ( FAILED( hr ) )
    {
        ErrorBox( L"failed GetOutputProps(), hr=0x%1X", hr );
        return( hr );
    }

    BYTE pbBuffer[1024];
    DWORD cbBuffer = 1024;

    WM_MEDIA_TYPE *pMediaType = ( WM_MEDIA_TYPE * )pbBuffer;

    hr = pProps->GetMediaType( pMediaType, &cbBuffer );
    if ( FAILED( hr ) )
    {
        pProps->Release( );
        ErrorBox( L"get media type failed (media type buffer may be too small). hr = 0x%1X.", hr );
        return( hr );
    }
    pProps->Release( );

    if ( pMediaType->majortype != WMMEDIATYPE_Audio )
    {
        ErrorBox( L"Not audio only (major type mismatch)." );
        return( E_UNEXPECTED );
    }
	switch (m_iOutputDevice)
	{
	case POD_WAVEOUTPUT:
		{
			//
			// Set up for audio playback
			//
			WAVEFORMATEX *pwfx = ( WAVEFORMATEX * )pMediaType->pbFormat;
			memcpy( &m_wfx, pwfx, sizeof( WAVEFORMATEX ) + pwfx->cbSize );
    
			MMRESULT mmr;

			mmr = waveOutOpen( &m_hwo,
							   WAVE_MAPPER, 
							   &m_wfx, 
							   (DWORD)WaveProc, 
							   (DWORD)this, 
							   CALLBACK_FUNCTION );
			mmr = MMSYSERR_NOERROR;

			if( mmr != MMSYSERR_NOERROR  )
			{
				CString str;
				str.Format(L"failed to open wav output device, error=%lu\n", mmr );
		        AfxMessageBox(str);
				return( E_UNEXPECTED );
			}

			break;
		}
	case POD_DIRECTSOUND:
		{
/*			HRESULT hr;

			if ((hr = DirectSoundCreate(NULL, &m_pDirectSound, NULL)) != DS_OK)
			{
				CString str;
				str.Format("failed to open DirectSound output device, error=%lu\n", hr);
				AfxMessageBox(str);
				return( E_UNEXPECTED );
			}

			m_pDirectSound->SetCooperativeLevel(*AfxGetMainWnd(), DSSCL_NORMAL);

			DSBUFFERDESC DSBufferDesc;
			::ZeroMemory(&DSBufferDesc, sizeof(DSBufferDesc));
			DSBufferDesc.dwSize = sizeof(DSBUFFERDESC);
			DSBufferDesc.dwFlags = DSBCAPS_PRIMARYBUFFER;

			m_pDirectSound->CreateSoundBuffer(&DSBufferDesc, &m_pDirectSoundBuffer, NULL);*/
		}
	}

    //
    // Start reading the data (and rendering the audio)
    //
	
	m_dwCurrentPosition = 0;
	m_bFirstTime = TRUE;

	QWORD qwLength = 0;
	
	if (dwMSEnd)
		qwLength = (QWORD)(dwMSEnd - dwMSStart) * WMAUDIO_UNITS;
    hr = m_pReader->Start( (QWORD)dwMSStart * WMAUDIO_UNITS, qwLength, 1.0, NULL );

	// Jetzt darauf warten, dass der Sound beginnt!
//    WaitForSingleObject( m_hEvent, INFINITE );
//	ResetEvent(m_hEvent);

    if( FAILED( hr ) )
    {
        CString str;
        str.Format(L"failed Start(), hr=0x%lX\n", hr);
		AfxMessageBox(str);
        return( hr );
    }

    return hr;
}

///////////////////////////////////////////////////////////////////////////////
HRESULT CWMAPlay::Stop()
{
	if (m_pReader)
	{
		m_pReader->Stop();
		m_fEof = TRUE;

		switch (m_iOutputDevice)
		{
		case POD_WAVEOUTPUT:
			waveOutClose(m_hwo);
			break;
		case POD_DIRECTSOUND:
			break;
		}
	}
	
	m_bPauseActive = FALSE;

	return S_OK;
}

///////////////////////////////////////////////////////////////////////////////
HRESULT CWMAPlay::Pause()
{
	if (m_pReader)
	{
		m_pReader->Pause();
		m_bPauseActive = TRUE;
	}
	
	return S_OK;
}

///////////////////////////////////////////////////////////////////////////////
HRESULT CWMAPlay::Seek(DWORD dwPosition)
{
	if (m_pReader)
	{
	    m_pReader->Start( dwPosition * WMAUDIO_UNITS, 0, 1.0, NULL );
	}
	
	return S_OK;
}

///////////////////////////////////////////////////////////////////////////////
DWORD CWMAPlay::GetCurrentPosition()
{
	return m_dwCurrentPosition;
}


///////////////////////////////////////////////////////////////////////////////
HRESULT STDMETHODCALLTYPE CWMAPlay::OnStatus( 
        /* [in] */ WMT_STATUS Status,
        /* [in] */ HRESULT hr,
        /* [in] */ WMT_ATTR_DATATYPE dwType,
        /* [in] */ BYTE __RPC_FAR *pValue,
        /* [in] */ void __RPC_FAR *pvContext)
{
    switch( Status )
    {
    case WMT_OPENED:
        SetEvent( m_hOpenEvent );
        break;

    case WMT_CLOSED:
        SetEvent( m_hCloseEvent );
        break;

    case WMT_ERROR:
        break;

    case WMT_BUFFERING_START:
        TRACE0( "OnStatus( WMT_BUFFERING START)\n" );
        break;

    case WMT_BUFFERING_STOP:
        TRACE0( "OnStatus( WMT_BUFFERING STOP)\n" );

        break;

	case WMT_STARTED:
		break;

    case WMT_EOF:
        TRACE0( "OnStatus( WMT_EOF )\n" );

        //
        // cleanup and exit
        //

        m_fEof = TRUE;

        if( 0 == m_cBuffersOutstanding )
        {
            SetEvent( m_hCompletionEvent );
        }

        break;

    case WMT_LOCATING:
        TRACE0( "OnStatus( WMT_LOCATING )\n" );
        break;

    case WMT_CONNECTING:
        TRACE0( "OnStatus( WMT_CONNECTING )\n" );
        break;

/*    case WMT_NO_RIGHTS:

        LPWSTR pszEscapedURL = NULL;

        hr = MakeEscapedURL( m_pszUrl, &pszEscapedURL );

        if( SUCCEEDED( hr ) )
        {
            WCHAR szURL[ 0x1000 ];

            swprintf( szURL, L"%s&filename=%s&embedded=false", pParam->bstrVal, pszEscapedURL );

            hr = LaunchURL( szURL );

            if( FAILED( hr ) )
            {
                printf( "Unable to launch web browser to retrieve playback license (err = %#X)\n", hr );
            }

            delete [] pszEscapedURL;
        }*/
    };

    return( S_OK );
}


///////////////////////////////////////////////////////////////////////////////
void CWMAPlay::OnWaveOutMsg( UINT uMsg, DWORD dwParam1, DWORD dwParam2 )
{
    if( WOM_DONE == uMsg )
    {
        waveOutUnprepareHeader( m_hwo, (LPWAVEHDR)dwParam1, sizeof(WAVEHDR) );
        
        delete (void*)dwParam1;

		TRACE0("Buffer playing done!\r\n");

        InterlockedDecrement( &m_cBuffersOutstanding );

        if( m_fEof && ( 0 == m_cBuffersOutstanding ) )
        {
            SetEvent( m_hCompletionEvent );
        }
    }
}


///////////////////////////////////////////////////////////////////////////////
void CALLBACK CWMAPlay::WaveProc( 
                                HWAVEOUT hwo, 
                                UINT uMsg, 
                                DWORD dwInstance, 
                                DWORD dwParam1, 
                                DWORD dwParam2 )
{
    CWMAPlay *pThis = (CWMAPlay*)dwInstance;

    pThis->OnWaveOutMsg( uMsg, dwParam1, dwParam2 );
}

HRESULT CWMAPlay::Open(LPCWSTR pszUrl)
{
    HRESULT hr;

    //
    // If the URL is not a UNC path, a full path, or an Internet-style URL then assume it is
    // a relative local file name that needs to be expanded to a full path.
    //
    WCHAR wszFullUrl[ MAX_PATH ];

    if( ( 0 == wcsstr( pszUrl, L"\\\\" ) )
        && ( 0 == wcsstr( pszUrl, L":\\" ) )
        && ( 0 == wcsstr( pszUrl, L"://" ) ) )
    {
        //
        // Make an ANSI version of the Unicode relative file name
        //
        char szRelUrl[ MAX_PATH ];

        if( 0 == WideCharToMultiByte( CP_ACP, 0, pszUrl, -1, szRelUrl, sizeof( szRelUrl ), NULL, NULL ) )
        {
            hr = HRESULT_FROM_WIN32( GetLastError() );
            return( hr );
        }

        //
        // Expand to a full ANSI path name
        //
        char szFullUrl[ MAX_PATH ];

        LPSTR pszCheck = _fullpath( szFullUrl, szRelUrl, MAX_PATH );

        if( NULL != pszCheck )
        {
            //
            // Convert back to Unicode
            //
            if( 0 == MultiByteToWideChar( CP_ACP, 0, szFullUrl, -1, wszFullUrl, MAX_PATH ) )
            {
                ErrorBox( L"internal error %lu", GetLastError() );
                return( E_UNEXPECTED );
            }

            pszUrl = wszFullUrl;
        }
    }

    //
    // Save a copy of the URL
    //
    delete [] m_pszUrl;

    m_pszUrl = new WCHAR[ wcslen( pszUrl ) + 1 ];

    if( NULL == m_pszUrl )
    {
        return( E_OUTOFMEMORY );
    }

    wcscpy( m_pszUrl, pszUrl );

    //
    // Use ourselves as the callback
    //
    hr = WMCreateReader( NULL, WMT_RIGHT_PLAYBACK, &m_pReader);

    if( FAILED( hr ) )
    {
		ErrorBox( L"failed to create audio reader (hr=%#X)", hr );
        return( hr );
    }

    hr = m_pReader->Open(pszUrl, this, NULL);

    if( FAILED( hr ) )
    {
		// JUS 001231: Keine Fehlermeldung
//		ErrorBox( "failed to open audio file %S\n(hr=%#X)", pszUrl, hr );
        return( hr );
    }

//    WaitForSingleObject( m_hEvent, INFINITE );
//	ResetEvent(m_hEvent);

	WaitForSingleObject( m_hOpenEvent, INFINITE ) ;
	return 0;
}

HRESULT CWMAPlay::Close()
{
	HRESULT hr;

	if (!m_pReader)
		return S_OK;

	hr = m_pReader->Close();

    if( SUCCEEDED( hr ) )
    {
        WaitForSingleObject( m_hCloseEvent, INFINITE );
    }

    if( m_pReader != NULL )
    {
        m_pReader->Release();
        m_pReader = NULL;
    }

	return hr;
}

BOOL CWMAPlay::ReadInformation()
{
	HRESULT hr;
    BYTE* pValue = NULL;
	IWMHeaderInfo*	pHeaderInfo = NULL ;

    hr = m_pReader->QueryInterface( IID_IWMHeaderInfo, ( VOID ** )&pHeaderInfo );
    if ( FAILED( hr ) )
    {
		CString str;
        str.Format( L"failed to qi for header interface (hr=%#X)\n", hr );
		AfxMessageBox(str);
        return( hr );
    }

    hr = GetHeaderAttribute( g_wszWMTitle, &pValue, pHeaderInfo) ;
	if (pValue)
	{
		m_pWMAEngine->m_sTitle.Format(L"%S", (WCHAR*)pValue);
		delete[] pValue ;
		pValue = NULL ;
	}

    hr = GetHeaderAttribute( g_wszWMAuthor, &pValue, pHeaderInfo) ;
	if (pValue)
	{
		m_pWMAEngine->m_sArtist.Format(L"%S", (WCHAR*)pValue);
		delete[] pValue ;
		pValue = NULL ;
	}

    hr = GetHeaderAttribute( g_wszWMDuration, &pValue, pHeaderInfo) ;
	if (pValue)
	{
		m_pWMAEngine->m_dwLength = (DWORD)(*((QWORD*)pValue) / WMAUDIO_UNITS);
		delete[] pValue ;
		pValue = NULL ;
	}

    if( pHeaderInfo != NULL )
    {
        pHeaderInfo->Release();
        pHeaderInfo = NULL;
    }

	return TRUE;
}

HRESULT CWMAPlay::GetHeaderAttribute(LPCWSTR pwszName, BYTE** ptrValue, IWMHeaderInfo* pHeaderInfo)
{
	WORD nstreamNum = 0, cbLength = 0;
	WMT_ATTR_DATATYPE type ;
	BYTE *pValue = NULL ;
	HRESULT hr = S_OK ;

	//
	// Get the no of bytes to be allocated for pValue
	//
	hr = pHeaderInfo->GetAttributeByName( &nstreamNum,
										  pwszName,
										  &type,
										  NULL,
										  &cbLength ) ;

	if( FAILED( hr ) && hr != ASF_E_NOTFOUND )
	{
		return hr ;
	}

	if( cbLength == 0 && hr == ASF_E_NOTFOUND )
	{
		return S_OK;
	}

	pValue = new BYTE[ cbLength ];

	if( NULL == pValue )
	{
		return HRESULT_FROM_WIN32( GetLastError() ) ;
	}

	//
	// Get the value
	//
	hr = pHeaderInfo->GetAttributeByName( &nstreamNum,
										  pwszName,
										  &type,
										  pValue,
										  &cbLength );
	*(ptrValue) = pValue ;

	return S_OK ;
}

// Setzt das Ausgabe-Device
// POD_WAVEOUTPUT = Wave-Output
// POD_DIRECTSOUND = DirectSound

void CWMAPlay::SetOutputDevice(int iDevice)
{
	m_iOutputDevice = iDevice;
}

void CWMAPlay::WaitForEvent(HANDLE hEvent)
{
	//
	// Wait for 10 milisecs for the event
	//
    if( WAIT_TIMEOUT != WaitForSingleObject( hEvent, 10 ) )
    {
		ResetEvent(hEvent);
    }

	return ;
}


// WMAEngine.cpp: implementation of the CWMAEngine class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "SoundEngine.h"
#include "SoundEngineIntern.h"
#include "WMAEngine.h"
#include "WMAPlay.h"
#include <io.h>

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

CWMAEngine::CWMAEngine()
{
	m_pWMAPlay = new CWMAPlay(this);
	m_bPlayCompleted = TRUE;
    m_hStartEvent = CreateEvent( NULL, TRUE, FALSE, NULL );
}

CWMAEngine::~CWMAEngine()
{
	delete m_pWMAPlay;
	m_pWMAPlay = NULL;

	CloseHandle(m_hStartEvent);
}

CWMAEngine* CWMAEngine::CreateFromFile(const CString &sFilename)
{
	CWMAEngine* pWMAEngine = new CWMAEngine;

	if (!_waccess(sFilename, 0))
	{
		pWMAEngine->m_sFilename = sFilename;

		if (pWMAEngine->Open())
			return pWMAEngine;
	}

	delete pWMAEngine;

	return NULL;
}

BOOL CWMAEngine::Open()
{
	CString sFilename = m_sFilename;
	
	Close();

//    WCHAR wszFilename[ MAX_PATH ];
//    MultiByteToWideChar( CP_ACP, 0, sFilename, -1, wszFilename, MAX_PATH );

	HRESULT hr;
	hr = m_pWMAPlay->Open(sFilename);

	if (hr == S_OK)
	{
		m_pWMAPlay->ReadInformation();
	}

	return hr == S_OK;
}

BOOL CWMAEngine::Close()
{
	m_pWMAPlay->Close();

	return TRUE;
}

BOOL CWMAEngine::Play(DWORD dwMSStart, DWORD dwMSEnd)
{
	m_pWMAPlay->SetOutputDevice(m_pSoundEngine->m_iOutputDevice);

	m_dwMSStart = dwMSStart;
	m_dwMSEnd = dwMSEnd;

	AfxBeginThread((AFX_THREADPROC)PlayThread, (LPVOID)this, 0, 0, 0, NULL);

	// Warten bis gestartet!!
	WaitForSingleObject(m_hStartEvent, 5000);    // Maximal 5 Sekunden!!

	return TRUE;
}

int CWMAEngine::PlayThread(LPVOID dwData)
{
	CWMAEngine* pWMAEngine = (CWMAEngine*)dwData;
    HANDLE hCompletionEvent = CreateEvent( NULL, TRUE, FALSE, NULL );

    HRESULT hrPlay;
    
//    WCHAR wszFullUrl[ MAX_PATH ];
//    MultiByteToWideChar( CP_ACP, 0, pWMAEngine->m_sFilename, -1, wszFullUrl, MAX_PATH );

	pWMAEngine->m_bPlayCompleted = FALSE;
	hrPlay = pWMAEngine->m_pWMAPlay->Play( pWMAEngine->m_dwMSStart, pWMAEngine->m_dwMSEnd, hCompletionEvent, &hrPlay );

	SetEvent(pWMAEngine->m_hStartEvent);

    if( SUCCEEDED( hrPlay ) )
    {
        WaitForSingleObject( hCompletionEvent, INFINITE );
    }

	pWMAEngine->m_bPlayCompleted = TRUE;

    CloseHandle( hCompletionEvent );

    return( hrPlay );
}

BOOL CWMAEngine::Stop()
{
	m_pWMAPlay->Stop();

	return TRUE;
}

BOOL CWMAEngine::Pause(BOOL bPause /* = TRUE */)
{
	m_pWMAPlay->Pause();

	return TRUE;
}

BOOL CWMAEngine::Seek(DWORD dwPosition)
{ 
	return m_pWMAPlay->Seek(dwPosition) == S_OK;
}

DWORD CWMAEngine::GetPlayPositionMS()
{
	DWORD dwPos = m_pWMAPlay->GetCurrentPosition();

	return dwPos;
}

BOOL CWMAEngine::IsPlaying()
{
	return !m_bPlayCompleted;
}

BOOL CWMAEngine::IsPlayCompleted()
{
	return m_bPlayCompleted;
}

BOOL CWMAEngine::SetSpeed(double dblSpeed)
{
	return TRUE;
}

BOOL CWMAEngine::SetVolume(double dblVolume)
{
	return TRUE;
}

void CWMAEngine::GetAvailableFormatList(CStringArray &saFormat)
{
    HRESULT hr = S_OK;
    DWORD dwIndex = 0;
    DWORD cProfiles = 0;
    WCHAR *pwszName = NULL;
    DWORD cchName = 0;
    IWMProfileManager *pIWMProfileManager = NULL;
    IWMProfileManager2 *pIWMProfileManager2 = NULL;

//	GetCodecNames(saFormat);
//	return;

    do
    {
        hr = WMCreateProfileManager( &pIWMProfileManager );
        if( FAILED( hr ) )
        {
			AfxMessageBox(L"Failed to get profile manager!");
            break;
        }

        hr = pIWMProfileManager->QueryInterface( IID_IWMProfileManager2, 
                                                 ( void ** )&pIWMProfileManager2 );
        if( FAILED( hr ) )
        {
			AfxMessageBox(L"Failed to query interface!");
            break;
        }

        //
        // Set system profile version to 8.0
        //
        hr = pIWMProfileManager2->SetSystemProfileVersion( WMT_VER_8_0 );
        if( FAILED( hr ) )
        {
			AfxMessageBox(L"Failed to set system profile!");
            break;
        }

		IWMProfileManagerLanguage* pProfileMgrLang = NULL;

		// Get the profile manager language interface.
		hr = pIWMProfileManager2->QueryInterface(IID_IWMProfileManagerLanguage,
										(void **) &pProfileMgrLang);
		if(FAILED(hr))
		{
			AfxMessageBox(L"Couldn't get IWMProfileManagerLanguage.\n");
			pProfileMgrLang->Release();
			return ;
		}

		// Retrieve the current language (as a LANGID value)
		WORD wLangID = 0;
		hr = pProfileMgrLang->GetUserLanguageID(&wLangID);
		if(FAILED(hr))
		{
			printf("Could not get the current language.\n");
			pProfileMgrLang->Release();
			return ;
		}

		hr = pProfileMgrLang->SetUserLanguageID(MAKELANGID(LANG_GERMAN, SUBLANG_GERMAN));

        hr = pProfileMgrLang->QueryInterface( IID_IWMProfileManager2, 
                                                 ( void ** )&pIWMProfileManager2 );

		pProfileMgrLang->Release();

        hr = pIWMProfileManager2->GetSystemProfileCount(  &cProfiles );
        if( FAILED( hr ) )
        {
			AfxMessageBox(L"Failed to get Systen Profile Count!");
            break;
        }

        IWMProfile *pIWMProfile = NULL;

        for( dwIndex = 0; ( dwIndex < cProfiles ) && SUCCEEDED( hr ); dwIndex++ )
        {
            hr = pIWMProfileManager2->LoadSystemProfile( dwIndex, &pIWMProfile );

            if( SUCCEEDED( hr ) )
            {
                hr = pIWMProfile->GetName( NULL, &cchName );
            }

            if( SUCCEEDED( hr ) )
            {
                pwszName = new WCHAR[ cchName ];
                if( NULL == pwszName )
                {
                    hr = E_OUTOFMEMORY;
                }
            }

			hr = pIWMProfile->GetName( pwszName, &cchName );
	
			if( SUCCEEDED( hr ) )
			{
				/*CString str;
				str.Format(L"%S", pwszName);*/
				saFormat.Add(pwszName);
			}

            pIWMProfile->Release();
            pIWMProfile = NULL;

            delete[] pwszName;
            pwszName = NULL;
        }
    }
    while( FALSE );

    pIWMProfileManager->Release();
}

HRESULT CWMAEngine::GetCodecNames(CStringArray &saFormat)
{
	IWMCodecInfo3* pCodecInfo;
    HRESULT hr = S_OK;
    DWORD   cCodecs  = 0;
    WCHAR*  pwszCodecName  = NULL;
    DWORD   cchCodecName     = 0;
   IWMProfileManager *pIWMProfileManager = NULL;
    IWMProfileManager2 *pIWMProfileManager2 = NULL;

	do
	{
        hr = WMCreateProfileManager( &pIWMProfileManager );
        if( FAILED( hr ) )
        {
			AfxMessageBox(L"Failed to get profile manager!");
            break;
        }

        hr = pIWMProfileManager->QueryInterface( IID_IWMProfileManager2, 
                                                 ( void ** )&pIWMProfileManager2 );
        if( FAILED( hr ) )
        {
			AfxMessageBox(L"Failed to query interface!");
            break;
        }

		IWMProfileManagerLanguage* pProfileMgrLang = NULL;

		// Get the profile manager language interface.
		hr = pIWMProfileManager->QueryInterface(IID_IWMProfileManagerLanguage,
										(void **) &pProfileMgrLang);
		if(FAILED(hr))
		{
			printf("Couldn't get IWMProfileManagerLanguage.\n");
			pProfileMgrLang->Release();
			return hr;
		}

		// Retrieve the current language (as a LANGID value)
		WORD wLangID = 0;
		hr = pProfileMgrLang->GetUserLanguageID(&wLangID);
		if(FAILED(hr))
		{
			printf("Could not get the current language.\n");
			pProfileMgrLang->Release();
			return hr;
		}

		pProfileMgrLang->SetUserLanguageID(MAKELANGID(LANG_GERMAN, SUBLANG_GERMAN));

		pProfileMgrLang->Release();

        //
        // Set system profile version to 8.0
        //
        hr = pIWMProfileManager2->SetSystemProfileVersion( WMT_VER_8_0 );
        if( FAILED( hr ) )
        {
			AfxMessageBox(L"Failed to set system profile!");
            break;
        }


	hr = pIWMProfileManager->QueryInterface( IID_IWMCodecInfo,
                                                 (void **) &pCodecInfo );
        if( FAILED( hr ) )
        {
            break;
        }

    // Retrieve the number of supported audio codecs on the system.
    hr = pCodecInfo->GetCodecInfoCount(WMMEDIATYPE_Audio, &cCodecs);

    if(SUCCEEDED(hr))
        printf("Number of audio codecs: %d\n\n", cCodecs);
    else
    {
        printf("Could not get the count of audio codecs.\n");
        return hr;
    }

    // Loop through all the audio codecs.
    for(DWORD dwCodecIndex = 0; dwCodecIndex < cCodecs; dwCodecIndex++)
    {
        // Get the codec name:
        // First, get the size of the name.
        hr = pCodecInfo->GetCodecName(WMMEDIATYPE_Audio, 
                                      dwCodecIndex, 
                                      NULL, 
                                      &cchCodecName);
        if(FAILED(hr))
        {
            printf("Could not get the size of the codec name.\n");
            return hr;
        }

        // Allocate a string of the appropriate size.
        pwszCodecName = new WCHAR[cchCodecName];
        if(pwszCodecName == NULL)
        {
            printf("Could not allocate memory.\n");
            return E_OUTOFMEMORY;
        }

        // Retrieve the codec name.
        hr = pCodecInfo->GetCodecName(WMMEDIATYPE_Audio, 
                                      dwCodecIndex, 
                                      pwszCodecName, 
                                      &cchCodecName);
        if(FAILED(hr))
        {
            delete[] pwszCodecName;
            printf("Could not get the codec name.\n");
            return hr;
        }


		DWORD cFormat = 0;
		pCodecInfo->GetCodecFormatCount(WMMEDIATYPE_Audio, dwCodecIndex, &cFormat);
		for (int iFormat=0;iFormat<cFormat;iFormat++)
		{
			IWMStreamConfig* pStreamConfig;

			WCHAR* pwszName = NULL;
			DWORD   cchName = 0;

			hr = pCodecInfo->GetCodecFormatDesc(WMMEDIATYPE_Audio, dwCodecIndex, iFormat, &pStreamConfig, NULL, &cchName);

			if(FAILED(hr))
			{
				printf("Could not get the size of the stream name.\n");
				return hr;
			}

			// Allocate a string of the appropriate size.
			pwszName = new WCHAR[cchName];

			if(pwszName == NULL)
			{
				printf("Could not allocate memory.\n");
				return E_OUTOFMEMORY;
			}

			// Retrieve the codec name.
			hr = pCodecInfo->GetCodecFormatDesc(WMMEDIATYPE_Audio, dwCodecIndex, iFormat, &pStreamConfig, pwszName, &cchName);
			if(FAILED(hr))
			{
				delete[] pwszName;
				printf("Could not get the codec name.\n");
				return hr;
			}

			CString str;
			str.Format(L"%S", pwszName); 
			saFormat.Add(str);

			delete[] pwszName;
		}
        
        // Clean up for the next iteration.
        delete[] pwszCodecName;
        pwszCodecName = NULL;
        cchCodecName  = 0;
    }

	} while (FALSE);
    return S_OK;
}


BOOL CWMAEngine::SaveToFile()
{
	return TRUE;
}


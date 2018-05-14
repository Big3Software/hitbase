// PlugInManager.cpp: implementation of the CPlugInManager class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "../hitmisc/HitbaseWinAppBase.h"
#include "../../App/Hitbase/resource.h"
#include "PlugInManager.h"
#include "SoundVisualizerWnd.h"
#include "PlugInOptionsDlg.h"
#include <math.h>
#include "../hitmisc/fourier.h"

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

CPlugInManager::CPlugInManager()
{

}

CPlugInManager::~CPlugInManager()
{
	SaveAllParameters();

	// Alle noch aktiven Plug-Ins schlieﬂen!
	for (int i=0;i<m_hpiList.GetSize();i++)
	{
		if (m_hpiList[i].m_bActive)
 		    SetActive(i, NULL, FALSE);
	}

	for (int i=0;i<m_hpiList.GetSize();i++)
		::FreeLibrary(m_hpiList[i].m_hpiInstance);
}

// Liest alle Plug-Ins, die im angegebenen Verzeichnis gefunden werden!
BOOL CPlugInManager::ReadPlugIns(const CString & sDirectory)
{
	m_hpiList.RemoveAll();

    CFileFind finder;
	CString sSearch = sDirectory;
	if (sDirectory.Right(1) != '\\')
		sSearch += "\\";
	
	sSearch += "*.hpi";
    BOOL bWorking = finder.FindFile(sSearch);

    while (bWorking)
    {
        bWorking = finder.FindNextFile();
        
		CPlugIn PlugIn;
		if (hpiLoad(finder.GetFilePath(), &PlugIn))
		{
			m_hpiList.Add(PlugIn);
		}
    }

	ReadAllParameters();

	return TRUE;
}

BOOL CPlugInManager::hpiLoad(const CString & shpiFilename, CPlugIn* pPlugIn)
{
	pPlugIn->m_hpiInstance = LoadLibrary(shpiFilename);

	if (!pPlugIn->m_hpiInstance)
		return FALSE;

	pPlugIn->m_fphpiInit = (fphpiInit*)GetProcAddress(pPlugIn->m_hpiInstance, "hpiInit");

	if (pPlugIn->m_fphpiInit)
	{
		memset(&pPlugIn->m_hpiInfo, 0, sizeof(HPI_INFO));
		pPlugIn->m_hpiInfo.nInterfaceVersion = HPI_VERSION_2;
		pPlugIn->m_fphpiInit(&pPlugIn->m_hpiInfo);

		// Noch die Versionsnummer pr¸fen! Wenn sie unterschiedlich sind, dann Warnung ausgeben!
		// JUS 990209: Das hier kann man wahrscheinlich so ‰ndern, daﬂ man auf < HPI_VERSION_1
		//             abfragt! Hitbase sollte eigentlich immer die alten Plug-Ins auch unterst¸tzen!
		//             Die Frage ist nur, ob die ohne eine Warnmeldung geschehen soll! Mach ich sp‰ter!
		if (pPlugIn->m_hpiInfo.nInterfaceVersion != HPI_VERSION_2 && 
			pPlugIn->m_hpiInfo.nInterfaceVersion != HPI_VERSION_1)
		{
			CString str, str1, str2;
			str.LoadString(TEXT_PLUGIN_LOAD_ERROR);
			str2.Format(get_string(TEXT_PLUGIN_VERSION_CONFLICT), pPlugIn->m_hpiInfo.nInterfaceVersion/100, pPlugIn->m_hpiInfo.nInterfaceVersion%100,
				HPI_VERSION_1/100, HPI_VERSION_1%100);

			str1.Format(str, shpiFilename, str2);
			if (AfxMessageBox(str1, MB_YESNO|MB_ICONEXCLAMATION) == IDNO)
			{
				FreeLibrary(pPlugIn->m_hpiInstance);
				return FALSE;
			}
		}
	}

	if (pPlugIn->m_hpiInfo.nInterfaceVersion == HPI_VERSION_2)
		pPlugIn->m_fphpiStart2 = (fphpiStart2*)GetProcAddress(pPlugIn->m_hpiInstance, "hpiStart");
	if (pPlugIn->m_hpiInfo.nInterfaceVersion == HPI_VERSION_1)
		pPlugIn->m_fphpiStart = (fphpiStart*)GetProcAddress(pPlugIn->m_hpiInstance, "hpiStart");
	pPlugIn->m_fphpiEnd = (fphpiEnd*)GetProcAddress(pPlugIn->m_hpiInstance, "hpiEnd");
	pPlugIn->m_fphpiGetParamInfo = (fphpiGetParamInfo*)GetProcAddress(pPlugIn->m_hpiInstance, "hpiGetParamInfo");
	pPlugIn->m_fphpiSetParamInfo = (fphpiSetParamInfo*)GetProcAddress(pPlugIn->m_hpiInstance, "hpiSetParamInfo");
	pPlugIn->m_fphpiGetBitmap = (fphpiGetBitmap*)GetProcAddress(pPlugIn->m_hpiInstance, "hpiGetBitmap");
	pPlugIn->m_fphpiDisplaySoundData = (fphpiDisplaySoundData*)GetProcAddress(pPlugIn->m_hpiInstance, "hpiDisplaySoundData");

	if (!pPlugIn->m_fphpiInit ||
		(!pPlugIn->m_fphpiStart &&
		 !pPlugIn->m_fphpiStart2) ||
		!pPlugIn->m_fphpiEnd ||
		!pPlugIn->m_fphpiGetParamInfo ||
		!pPlugIn->m_fphpiSetParamInfo ||
		!pPlugIn->m_fphpiGetBitmap ||
		!pPlugIn->m_fphpiDisplaySoundData)
	{
		CString str, str1;
		str.LoadString(TEXT_PLUGIN_LOAD_ERROR);
		str1.Format(str, shpiFilename, get_string(TEXT_PLUGIN_MISSING_FUNCTION));
		if (AfxMessageBox(str1, MB_YESNO|MB_ICONEXCLAMATION) == IDNO)
		{
			FreeLibrary(pPlugIn->m_hpiInstance);
			return FALSE;
		}
	}

	int i;
	for (i=0;;i++)
	{
		HPI_PARAM_INFO ParamInfo;

		memset(&ParamInfo, 0, sizeof(ParamInfo));
		if (pPlugIn->m_fphpiGetParamInfo(HPIP_USER+i, &ParamInfo))
			pPlugIn->m_Param.Add(ParamInfo);
		else
			break;
	}

	pPlugIn->m_nNumberOfParameters = i;

	return TRUE;
}

void CPlugInManager::FillLibrariesInComboBox(CComboBox * pComboBox)
{
	for (int i=0;i<m_hpiList.GetSize();i++)
	{
		if (pComboBox->FindString(-1, CString(m_hpiList[i].m_hpiInfo.szLibraryName)) < 0)
		{				// Keine doppelten Eintr‰ge
			int nIndex = pComboBox->AddString(CString(m_hpiList[i].m_hpiInfo.szLibraryName));
			pComboBox->SetItemData(nIndex, i);
		}
	}
}

void CPlugInManager::FillModulesInComboBox(CComboBox * pComboBox, int nBibliothek /* = -1 */)
{
	for (int i=0;i<m_hpiList.GetSize();i++)
	{
		if (nBibliothek == -1 || !_stricmp(m_hpiList[nBibliothek].m_hpiInfo.szLibraryName, m_hpiList[i].m_hpiInfo.szLibraryName))
		{
			int nIndex = pComboBox->AddString(CString(m_hpiList[i].m_hpiInfo.szFullName));
			pComboBox->SetItemData(nIndex, i);
		}
	}
}

HPI_INFO* CPlugInManager::GetPlugInInformation(int nIndex)
{
	ASSERT(nIndex >= 0 && nIndex < m_hpiList.GetSize());

	return &m_hpiList[nIndex].m_hpiInfo;
}

void CPlugInManager::AddPlugInsToMenu(HMENU hMenu)
{
	int i;

	ASSERT(hMenu);
 
	if (!hMenu)
		return;

	for (i=0;;i++)
	{
		UINT nID = GetMenuItemID(hMenu, 0);
		
		if (nID >= ID_ANSICHT_PLUGIN_1 && nID <= ID_ANSICHT_PLUGIN_1+MAX_PLUGINS)
			DeleteMenu(hMenu, 0, MF_BYPOSITION);
		else
			break;
	}

	if (i > 0)       // Jetzt noch den Separator wegschmeiﬂen!
	{
		UINT nID = GetMenuItemID(hMenu, 0);
		if (!nID)
			DeleteMenu(hMenu, 0, MF_BYPOSITION);
	}

	if (m_hpiList.GetSize() > 0)
		InsertMenu(hMenu, 0, MF_BYPOSITION|MF_SEPARATOR, 0, NULL);
	for (i=0;i<m_hpiList.GetSize();i++)
	{
		InsertMenu(hMenu, i, MF_BYPOSITION|MF_STRING, ID_ANSICHT_PLUGIN_1+i, (CString)m_hpiList[i].m_hpiInfo.szShortName);
	}	
/*	int i;

	ASSERT(pMenu);
 
	if (!pMenu)
		return;

	for (i=0;;i++)
	{
		UINT nID = pMenu->GetMenuItemID(0);
		
		if (nID >= ID_ANSICHT_PLUGIN_1 && nID <= ID_ANSICHT_PLUGIN_1+MAX_PLUGINS)
			pMenu->DeleteMenu(0, MF_BYPOSITION);
		else
			break;
	}

	if (i > 0)       // Jetzt noch den Separator wegschmeiﬂen!
	{
		UINT nID = pMenu->GetMenuItemID(0);
		if (!nID)
			pMenu->DeleteMenu(0, MF_BYPOSITION);
	}

	if (m_hpiList.GetSize() > 0)
		pMenu->InsertMenu(0, MF_BYPOSITION|MF_SEPARATOR);
	for (i=0;i<m_hpiList.GetSize();i++)
	{
		pMenu->InsertMenu(i, MF_BYPOSITION|MF_STRING, ID_ANSICHT_PLUGIN_1+i, m_hpiList[i].m_hpiInfo.szShortName);
	}	*/
}

BOOL CPlugInManager::IsActive(int nIndex)
{
	return m_hpiList[nIndex].m_bActive;
}

void CPlugInManager::SetActive(int nIndex, CSoundVisualizerWnd* pSoundVisualizerWnd, BOOL bActive)
{
	if (!bActive && m_hpiList[nIndex].m_pSoundVisualizerWnd)
	{
		m_hpiList[nIndex].m_bActive = FALSE;
		Sleep(500);       // Kurz warten, damit das Plug-In den aktuellen Aufruf noch beenden kann.
		m_hpiList[nIndex].m_pSoundVisualizerWnd->DestroyWindow();
		m_hpiList[nIndex].m_pSoundVisualizerWnd = NULL;
	}

	if (bActive)
	{
		if (m_hpiList[nIndex].m_hpiInfo.nInterfaceVersion == HPI_VERSION_2)
		{
			if (!m_hpiList[nIndex].m_fphpiStart2(*pSoundVisualizerWnd))
			{
				pSoundVisualizerWnd->DestroyWindow();
				return;
			}
		}
		else
		{
			// JUS 990321: Wenn die Start-Funktion fehlschl‰gt, dann abbrechen! (Fr¸hlingsanfang! Toll!)
			if (!m_hpiList[nIndex].m_fphpiStart())
			{
				pSoundVisualizerWnd->DestroyWindow();
				return;
			}
		}
	}
	else
		m_hpiList[nIndex].m_fphpiEnd();

	m_hpiList[nIndex].m_bActive = bActive;
	m_hpiList[nIndex].m_pSoundVisualizerWnd = pSoundVisualizerWnd;
}

// Schlieﬂt alle offenen Plug-Ins und speichert den "Active"-Status.
void CPlugInManager::CloseAllPlugIns()
{
	for (int i=0;i<m_hpiList.GetSize();i++)
	{
		m_hpiList[i].SaveParameters(TRUE);

		if (m_hpiList[i].m_pSoundVisualizerWnd && m_hpiList[i].m_bActive)
		{
			SetActive(i, NULL, FALSE);
		}
	}
}

BOOL CPlugInManager::DisplaySoundData(const char* pWaveData, DWORD dwCount)
{
	// JUS 990513: Damit auch Plug-Ins abgebrochen werden kˆnnen, die das System
	//             lahmlegen.
	MSG msg;

	while ( ::PeekMessage( &msg, NULL, 0, 0, PM_NOREMOVE ) ) 
	{
		if (!AfxGetApp() || !AfxGetApp()->PumpMessage()) 
		{ 
			::PostQuitMessage(0); 
			return 0; 
		} 
	}
	// JUS_

	for (int i=0;i<m_hpiList.GetSize();i++)
	{
		if (m_hpiList[i].m_bActive)
		{
			// JUS 990311
			switch (m_hpiList[i].m_hpiInfo.nType & (HPIT_SIGNAL|HPIT_FREQUENCY))
			{
			case HPIT_SIGNAL:
				{
					m_hpiList[i].m_fphpiDisplaySoundData(*m_hpiList[i].m_pSoundVisualizerWnd, pWaveData, dwCount);
					break;
				}
			case HPIT_FREQUENCY:
				{
					const int nNumberOfSamples = 1024;   // Das ist z.Zt. fest!
					double RealIn[nNumberOfSamples+1];
					double RealOut[nNumberOfSamples+1];
					double ImagOut[nNumberOfSamples+1];
					unsigned char FFTValues[nNumberOfSamples+1];
				    int j, sample1, sample2;

				    memset(&RealIn, 0, sizeof(RealIn));
					for (j=0;i<nNumberOfSamples && j < (int)dwCount/2;j++)
					{
						sample1 = ((int)(unsigned char)pWaveData[j*2])-128;
						sample2 = ((int)(unsigned char)pWaveData[j*2+1])-128;
						RealIn[j] = (sample1 + sample2)/2;
						// Das hier ist das sog. "Hemming-Fenster"
						// Vielleicht gibt es da noch was besseres? (Ist aber schon ganz gut!)
						RealIn[j] *= 0.54 + 0.46*cos((6.2831853*((double)j-((double)nNumberOfSamples/2)))/(double)nNumberOfSamples);
					}
    
					// Hier ist sie (TARRAAA!): Fast Fourier Transformation
					// Die RealOut-Werte scheinen wohl bis 32768 als maximum zu gehen!?
					CFFT::fft_double ( 1024, 0, RealIn, NULL, RealOut, ImagOut);

					// Jetzt die double-Werte in char zur¸ckumwandeln

					for (j=1;j<nNumberOfSamples/4;j++)
					{
						int value = abs(max((int)RealOut[j*2], (int)RealOut[j*2+1]))/5;
						//value = min(255, value/level);   Das hier vielleicht als Parameter?
						FFTValues[j-1] = min(255, value);
					}
					m_hpiList[i].m_fphpiDisplaySoundData(*m_hpiList[i].m_pSoundVisualizerWnd, (const char*)FFTValues, nNumberOfSamples/4-1);
					break;
				}
			default:
				ASSERT(FALSE);
			}
		}
	}

	return TRUE;
}

BOOL CPlugInManager::ChangeOptions(int nPlugInIndex)
{
	CPlugInOptionsDlg PlugInOptionsDlg(&m_hpiList[nPlugInIndex]);

	if (PlugInOptionsDlg.DoModal() == IDOK)
	{
		for (int i=0;i<m_hpiList[nPlugInIndex].m_nNumberOfParameters;i++)
			m_hpiList[nPlugInIndex].m_fphpiSetParamInfo(HPIP_USER+i, &m_hpiList[nPlugInIndex].m_Param[i]);
		if (m_hpiList[nPlugInIndex].m_pSoundVisualizerWnd)
			m_hpiList[nPlugInIndex].m_pSoundVisualizerWnd->Invalidate();
		return TRUE;
	}
	else
		return FALSE;
}

// Schreibt alle Parameter der Plug-Ins in die Registry.
void CPlugInManager::SaveAllParameters()
{
	for (int i=0;i<m_hpiList.GetSize();i++)
	{
		m_hpiList[i].SaveParameters();
	}
}

// Liest alle Parameter der Plug-Ins aus der Registry.
void CPlugInManager::ReadAllParameters()
{
	for (int i=0;i<m_hpiList.GetSize();i++)
	{
		m_hpiList[i].LoadParameters();
	}
}


///////////////////////////////////////////////////////////////////////
// Die Plug-Ins

CPlugIn::CPlugIn()
{
	m_bActive = FALSE; 
	m_pSoundVisualizerWnd = NULL; 
	m_WindowRect.SetRect(100, 100, 400, 400);
}

CPlugIn::CPlugIn(const CPlugIn& theOther)
{
	*this = theOther;
}

CPlugIn& CPlugIn::operator= (const CPlugIn& theOther)
{
	m_hpiInstance = theOther.m_hpiInstance;
	m_fphpiInit = theOther.m_fphpiInit;
	m_fphpiStart = theOther.m_fphpiStart;
	m_fphpiStart2 = theOther.m_fphpiStart2;
	m_fphpiEnd = theOther.m_fphpiEnd;
	m_fphpiGetParamInfo = theOther.m_fphpiGetParamInfo;
	m_fphpiSetParamInfo = theOther.m_fphpiSetParamInfo;
	m_fphpiGetBitmap = theOther.m_fphpiGetBitmap;
	m_fphpiDisplaySoundData = theOther.m_fphpiDisplaySoundData;

	memcpy(&m_hpiInfo, &theOther.m_hpiInfo, sizeof(m_hpiInfo));

	m_bActive = theOther.m_bActive;
	m_pSoundVisualizerWnd = theOther.m_pSoundVisualizerWnd;
	m_WindowRect = theOther.m_WindowRect;

	// Benutzerfelder
	m_nNumberOfParameters = theOther.m_nNumberOfParameters;
	
	for (int i=0;i<theOther.m_Param.GetSize();i++)
		m_Param.Add(theOther.m_Param[i]);

	return *this;
}

HBITMAP CPlugIn::GetBitmap()
{
	if (m_fphpiGetBitmap)
		return m_fphpiGetBitmap();
	else
		return NULL;
}

void CPlugIn::SaveParameters(BOOL bSaveActiveState /* = FALSE */)
{
	HKEY hHitKey;
	DWORD ret;
	CString SaveKey;
	
	SaveKey = (CString)REGISTRY_KEY + L"\\Plug-Ins\\" + (CString)m_hpiInfo.szShortName;
	RegCreateKeyEx(HKEY_CURRENT_USER, SaveKey, 0,
		L"Hitbase", REG_OPTION_NON_VOLATILE, KEY_ALL_ACCESS, NULL,
		&hHitKey, &ret);

	for (int i=0;i<m_Param.GetSize();i++)
	{
		CString str;

		switch (m_Param[i].nType)
		{
		case HPIPT_BOOLEAN:
		case HPIPT_INTEGER:
		case HPIPT_COLOR:
			str.Format(L"%d", m_Param[i].nValue);
			break;
		case HPIPT_STRING:
			str = m_Param[i].szValue;
			break;
		default:
			ASSERT(FALSE);
		}

		CConfig::RegWriteString(hHitKey, (CString)m_Param[i].szName, str);
	}

	// Fenster-Position
	CConfig::RegWriteBinary(hHitKey, L"WindowPos", m_WindowRect, sizeof(m_WindowRect));

	// Aktiv?
	if (bSaveActiveState)
		CConfig::RegWriteInt(hHitKey, L"Active", m_bActive);

	RegCloseKey(hHitKey);
}

void CPlugIn::LoadParameters()
{
	USES_CONVERSION;
	HKEY hHitKey;
	DWORD ret;
	CString SaveKey;
	
	SaveKey = (CString)REGISTRY_KEY + L"\\Plug-Ins\\" + (CString)m_hpiInfo.szShortName;
	RegCreateKeyEx(HKEY_CURRENT_USER, SaveKey, 0,
		L"Hitbase", REG_OPTION_NON_VOLATILE, KEY_ALL_ACCESS, NULL,
		&hHitKey, &ret);

	for (int i=0;i<m_Param.GetSize();i++)
	{
		CString str;

		str = CConfig::RegQueryString(hHitKey, (CString)m_Param[i].szName, L"");

		if (str.IsEmpty())
			continue;

		switch (m_Param[i].nType)
		{
		case HPIPT_BOOLEAN:
		case HPIPT_INTEGER:
		case HPIPT_COLOR:
			m_Param[i].nValue = _wtoi(str);
			break;
		case HPIPT_STRING:
			strcpy(m_Param[i].szValue, W2A(str));
			break;
		default:
			ASSERT(FALSE);
		}

		m_fphpiSetParamInfo(HPIP_USER+i, &m_Param[i]);
	}

	// Fenster-Position
	CConfig::RegQueryBinary(hHitKey, L"WindowPos", &m_WindowRect, sizeof(m_WindowRect));

	// Aktiv?
	CConfig::RegQueryInt(hHitKey, L"Active", m_bActive);

	RegCloseKey(hHitKey);
}

// Gibt True zur¸ck, wenn ein Fullscreen Plug-In aktiv ist.
BOOL CPlugInManager::FullScreenPlugInActive()
{
	for (int i=0;i<m_hpiList.GetSize();i++)
	{
		if (m_hpiList[i].m_bActive &&
			(m_hpiList[i].m_hpiInfo.nType & HPIT_FULLSCREEN))
			return TRUE;
	}

	return FALSE;
}

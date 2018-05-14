// CDAudioEngine.cpp: implementation of the CCDAudio class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "SoundEngineIntern.h"
#include "CDAudio.h"
#include "CDText.h"
#include "CDExtra.h"
#include "resource.h"
#include "../hitmisc/hitcommn.h"
#include "CDAudioAnalogEngine.h"
#include "CDAudioDigitalEngine.h"
#include <io.h>
#include "../../app/hitbase/resource.h"
#include "../hitmisc/HitbaseWinAppBase.h"

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

CCDAudio::CCDAudio()
{
	m_pCDText = new CCDText;
	m_bCDText = FALSE;
	m_pCDExtra = new CCDExtra;
	m_bCDExtra = FALSE;
	m_iDeviceNumber = -1;
}

CCDAudio::~CCDAudio()
{
	delete m_pCDText;
	delete m_pCDExtra;
}

CCDAudio* CCDAudio::CreateObject()
{
	// In Abhängigkeit der Optionen, entweder digital oder analog nehmen
	if (!((CHitbaseWinAppBase*)AfxGetApp())->m_config.m_bCDOutputDigital)
	{
		CCDAudioAnalogEngine* pAnalog = new CCDAudioAnalogEngine;

		return pAnalog;
	}
	else
	{
		CCDAudioDigitalEngine* pDigital = new CCDAudioDigitalEngine;

		return pDigital;
	}
}

BOOL CCDAudio::IsCDExtra()
{
	return m_bCDExtra;
}

BOOL CCDAudio::IsCDText()
{
	return m_bCDText;
}

CCDExtra* CCDAudio::GetCDExtra()
{
	return m_pCDExtra;
}

CCDText* CCDAudio::GetCDText()
{
	return m_pCDText;
}

BOOL CCDAudio::CheckForCDExtra()
{
	CString str, testpath;

   	if (((CHitbaseWinAppBase*)AfxGetApp())->m_config.m_bDisableCDText)    // JUS 20030507: CD-Text und CD-Extra gleich
		return FALSE;

	str = GetDriveFromDeviceID(m_iDeviceNumber);
	testpath.Format(L"%s\\cdplus", str);
	
	if (!_waccess(testpath, 0))          // CD-EXTRA!!
		return TRUE;
	
	return FALSE;
}

BOOL CCDAudio::ReadCDText()
{
	if (((CHitbaseWinAppBase*)AfxGetApp())->m_config.m_bDisableCDText)    // JUS 20021224
		return FALSE;

	if (!m_pCDText->Init())
	{
		BOOL bWarningBoxDisableCDText = CConfig::ReadGlobalRegistryKeyInt(L"WarningBoxDisableCDText", FALSE);
		if (!bWarningBoxDisableCDText)
		{
			CCommonCheckDlg dlgCDTextWarning(IDD_COMMON_CHECKDLG, get_string(IDS_NO_ASPI_INSTALLED));
			dlgCDTextWarning.DoModal();
			CConfig::WriteGlobalRegistryKeyInt(L"WarningBoxDisableCDText", dlgCDTextWarning.m_Check1 ? TRUE : FALSE);
		}
		return FALSE;
	}

	BOOL bRet = m_pCDText->ReadCDText(2);

	m_pCDText->Release();

	return bRet;
}

// Liefert die logische Device-ID des angegebenen CDROM-Laufwerks
// zurück. Wird für CDAudio.Open benötigt.
// drive: Laufwerksbuchstabe z.B. 'G'

CString CCDAudio::GetDriveFromDeviceID(int id)
{
	int i, DeviceID=-1;
	UINT type;
	CString devicename;
	
	for (i=2;i<26;i++)
	{
		devicename.Format(L"%c:\\", 'A'+i);
		type = GetDriveType(devicename);
		if (type == DRIVE_CDROM)
		{
			DeviceID++;
			
			if (DeviceID == id)
				return devicename.Left(2);
		}
	}
	
	return L"";
}

//////////////////////////////////////////////////////////////////////////////////

BOOL CCDAudio::CanAddFiles()
{
	return FALSE;
}

CString CCDAudio::GetDeviceName()
{
	return GetDriveFromDeviceID(m_iDeviceNumber);
}

int CCDAudio::GetDeviceNumber()
{
	return m_iDeviceNumber;
}

BOOL CCDAudio::IsVirtual()
{
	return FALSE;
}

BOOL CCDAudio::IsPureDataCD()
{
	return FALSE;
}

BOOL CCDAudio::IsMediaPresent()
{
	return FALSE;
}

BOOL CCDAudio::CanEjectTray()
{
	return TRUE;
}

BOOL CCDAudio::OpenTray(BOOL bWait)
{
	return TRUE;
}

BOOL CCDAudio::CloseTray(BOOL bWait)
{
	return TRUE;
}

BOOL CCDAudio::StopAll()
{
	return Stop();
}

// Ermittelt die Anzahl der vorhandenen CDROM-Laufwerke
int CCDAudio::GetNumberOfDrives()
{
	int i, iCount = 0;
	UINT type;
	CString sDeviceName;
	
	for (i=2;i<26;i++)
	{
		sDeviceName.Format(L"%c:\\", 'A'+i);
		type = GetDriveType(sDeviceName);
		if (type == DRIVE_CDROM)
		{
			iCount++;
		}
	}
	
	return iCount;
}

BOOL CCDAudio::ReadInformation()
{
	if (!m_bPureDataCD)
		m_bCDExtra = CheckForCDExtra();   // Prüft, ob es sich hierbei um eine CD-Extra handelt!
	else
		m_bCDExtra = FALSE;
	
	if (!m_bPureDataCD)
		m_bCDText = ReadCDText();
	else
		m_bCDText = FALSE;

	if (m_bCDExtra)
	{
		if (!m_pCDExtra->ReadInformation(GetDriveFromDeviceID(m_iDeviceNumber)))
			m_bCDExtra = FALSE;    // Bei fehlerhaften CD-Extras Flag wieder löschen!
	}

	return !m_bPureDataCD;
}

CString CCDAudio::GetMediaIdentity()
{
	return m_sMediaIdentity;
}

CString CCDAudio::GetUPC()
{
	return m_sUPC;
}


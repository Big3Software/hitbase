// SoundEngine1.cpp: implementation of the CSoundEngine class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "SoundEngine.h"
#include "MP3Engine.h"
#include "MP3Player.h"
#include "resource.h"
#include "../../app/hitbase/resource.h"
#include <io.h>
#include "LoadSoundFilesDlg.h"
#include "../hitmisc/md5.h"
#include ".\soundengine.h"
#include <shlwapi.h>

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

bool CSoundEngine::m_bMp3PlayerInitialized = false;
FMOD::DSP* CSoundEngine::m_pFSoundDSPUnit = NULL;
FMOD::System* CSoundEngine::m_pFMODSystem = NULL;

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

CSoundEngine::CSoundEngine()
{
	m_nFileFormat = -1;
	m_iOutputDevice = POD_WAVEOUTPUT;
	m_pSoundEngineItem = NULL;
	m_dwTotalTime = 0;
}

CSoundEngine::~CSoundEngine()
{
	GlobalExit();

	for (int i=0;i<m_Tracks.GetCount();i++)
		delete m_Tracks[i];

	Close();
}

BOOL CSoundEngine::Open(const CString& sDevice)
{
	return FALSE;
}

BOOL CSoundEngine::OpenDevice(int iDeviceNumber)
{
	CString sDevice;
	sDevice.Format(L"DEVICE=%d", iDeviceNumber);
	return Open(sDevice);
}

BOOL CSoundEngine::Close()
{
	return FALSE;
}

BOOL CSoundEngine::Play(DWORD dwMSStart, DWORD dwMSEnd, BOOL bOverlap, CWnd* pWndCallback)
{
	if (m_Tracks.GetSize() <= 0)
		return FALSE;

	int iTrack = GetTrackFromPosition(dwMSStart);

	if (iTrack != m_iCurrentTrack || !m_pSoundEngineItem)
	{
		m_iCurrentTrack = iTrack;
		
		if (!bOverlap || !IsPlaying())
		{
			if (m_pSoundEngineItem)
			{
				m_pSoundEngineItem->Close();
			}
		}

		m_pSoundEngineItem = m_Tracks[iTrack];
		if (!m_pSoundEngineItem->Open())
		{
			CString sError;
			sError.Format(get_string(TEXT_SOUNDFILE_ERROR), m_pSoundEngineItem->m_sFilename);
			AfxMessageBox(sError, MB_OK|MB_ICONINFORMATION);
		}
	}

	if (dwMSEnd)
		dwMSEnd -= m_Tracks[iTrack]->m_dwStartTime;
	BOOL bRet = m_pSoundEngineItem->Play(dwMSStart - m_Tracks[iTrack]->m_dwStartTime, dwMSEnd);

	if (bRet)
		m_bPlaying = TRUE;

	return bRet;
}

BOOL CSoundEngine::Stop()
{
	m_bPlaying = FALSE;

	if (m_pSoundEngineItem)
	{
		m_pSoundEngineItem->Stop();

		m_pSoundEngineItem->Close();
		m_pSoundEngineItem = NULL;
	}

	return TRUE;
}

BOOL CSoundEngine::StopAll()
{
	Stop();

	// Und jetzt noch alle eventuell spielenden Titel (wegen Überblendung) stoppen.
	for (int i=0;i<m_Tracks.GetSize();i++)
	{
		if (m_Tracks[i]->IsPlaying())
			m_Tracks[i]->Stop();
	}

	return TRUE;
}

BOOL CSoundEngine::Pause(BOOL bPause /* = TRUE */)
{
	if (!bPause)
		m_bPlaying = TRUE;
	else
		m_bPlaying = FALSE;

	if (m_pSoundEngineItem)
		return m_pSoundEngineItem->Pause(bPause);
	else
		return FALSE;
}

BOOL CSoundEngine::Seek(DWORD dwPosition)
{ 
	return FALSE;
}

CSoundEngineItem* CSoundEngine::GetTrack(int iTrack)
{
	return m_Tracks[iTrack];
}

DWORD CSoundEngine::GetStartPositionMS()
{
	if (m_Tracks.GetSize() > 0)
		return m_Tracks[0]->m_dwStartTime;
	else
		return 0;
}

DWORD CSoundEngine::GetTrackLengthMS(int iTrack)
{
	if (m_Tracks.GetSize() <= iTrack)
		return 0;
	else
		return m_Tracks[iTrack]->m_dwLength;
}

DWORD CSoundEngine::GetTotalTimeMS()
{
	return m_dwTotalTime;
}

DWORD CSoundEngine::GetTrackStartPositionMS(int iTrack)
{
	if (m_Tracks.GetSize() <= iTrack)
		return GetTotalTimeMS();
	else
		return m_Tracks[iTrack]->m_dwStartTime;
}

DWORD CSoundEngine::GetPlayPositionMS()
{
	// Damit beim Ende eines Liedes automatisch das nächste gespielt wird!
	if (m_bPlaying && m_pSoundEngineItem->IsPlayCompleted() && m_iCurrentTrack < GetNumberOfTracks()-1)
		Play(m_Tracks[m_iCurrentTrack+1]->m_dwStartTime);

	return m_Tracks[m_iCurrentTrack]->m_dwStartTime + m_pSoundEngineItem->GetPlayPositionMS();
}

int CSoundEngine::GetCurrentTrack()
{
	return m_iCurrentTrack;
}

int CSoundEngine::SetOutputDevice(int iOutputDevice)
{
	m_iOutputDevice = iOutputDevice;

	return TRUE;
}

BOOL CSoundEngine::IsPlaying()
{
	if (m_pSoundEngineItem)
		return m_pSoundEngineItem->IsPlaying();
	else
		return FALSE;
}

BOOL CSoundEngine::IsPlayCompleted()
{
	return FALSE;
}

int CSoundEngine::FillSoundDevicesInComboBox(CComboBox& theComboBox)
{
	USES_CONVERSION;
	GlobalInit();

	int iNumberOfDrivers;
	m_pFMODSystem->getNumDrivers(&iNumberOfDrivers);
	int i;
	for (i=0;i<iNumberOfDrivers;i++)
	{
		char sDriverName[200];
		FMOD_GUID guid;
		m_pFMODSystem->getDriverInfo(i, sDriverName, 200, &guid);
		//sDriverName.ReleaseBuffer();
		//sDriverName.Format(L"%s", sDriverName);
		theComboBox.AddString(A2W(sDriverName));
	}

	return i;
}

int CSoundEngine::GetNumberOfTracks()
{
	return m_Tracks.GetCount();
}

int CSoundEngine::GetTrackFromPosition(DWORD dwPos)
{
	int iTrack = -1;

	if (!m_Tracks.GetSize())
		return -1;

	if (dwPos < m_Tracks[0]->m_dwStartTime)
		return -1;

	// Das hier sollte ziemlich schnell gehen, da es ziemlich große Playlisten gibt!!
	// Da die StartTime aufsteigend ist, kann man hier binäre Suche machen!
	int iLeft = 0;
	int iRight = m_Tracks.GetSize();
	int iMiddle = (iLeft + iRight)/2;
	while (iLeft != iRight && iLeft >= 0 && iRight >= 0)
	{
		if (dwPos >= m_Tracks[iMiddle]->m_dwStartTime && dwPos < m_Tracks[iMiddle]->m_dwStartTime + m_Tracks[iMiddle]->m_dwLength)
			break;

		if (m_Tracks[iMiddle]->m_dwStartTime < dwPos)
		{
			iLeft = iMiddle+1;
			iMiddle = (iLeft + iRight)/2;
		}
		else
		{
			iRight = iMiddle-1;
			iMiddle = (iLeft + iRight)/2;
		}
	}

	return iMiddle;
}

// Berechnet die Startzeiten und die Gesamtzeit der virtuellen CD.
void CSoundEngine::RecalcTrackTimes()
{
	m_dwTotalTime = 0;

	for (int i=0;i<m_Tracks.GetSize();i++)
	{
		m_Tracks[i]->m_dwStartTime = m_dwTotalTime;
		m_dwTotalTime += m_Tracks[i]->m_dwLength;
	}
}

// Fügt eine Datei (Lied) der virtuellen CD hinzu. Liefert den Index der neuen Tracks
// zurück, wenn erfolgreich, ansonsten -1.
int CSoundEngine::AddFile(const CString &sFilename, int iInsertAt /* = -1 */)
{
	int iNewIndex;
	
	CSoundEngineItem* pSoundEngineItem = CSoundEngineItem::CreateFromFile(sFilename);

	// Informationen aus der Sounddatei lesen und anzeigen
	if (pSoundEngineItem && pSoundEngineItem->GetTotalTime() > 0)
	{
		// Wenn Titel und Interpret leer sind, hier versuchen, aus dem Dateinamen zu erzeugen.
		if (Big3::Hitbase::Configuration::Settings::Current->DetectTrackInfoFromFilename)
		{
			if (pSoundEngineItem->m_sTitle.IsEmpty() && pSoundEngineItem->m_sArtist.IsEmpty())
			{
				System::String^ artist = "";
				System::String^ title = "";
				Big3::Hitbase::Miscellaneous::Misc::GetAlbumInfoByFilename(gcnew System::String(sFilename), Big3::Hitbase::Configuration::Settings::Current->DetectTrackInfoDelimiter, artist, title);
				pSoundEngineItem->m_sArtist = artist;
				pSoundEngineItem->m_sTitle = title;
			}
		}

		pSoundEngineItem->m_pSoundEngine = this;

		if (iInsertAt >= 0)
		{
			m_Tracks.InsertAt(iInsertAt, pSoundEngineItem);
			iNewIndex = iInsertAt;
		}
		else
			iNewIndex = m_Tracks.Add(pSoundEngineItem);


		RecalcTrackTimes();

		return iNewIndex;
	}
	else
	{
		if (pSoundEngineItem)
			delete pSoundEngineItem;

		return -1;
	}
}

// Löscht eine Datei aus der virtuellen CD.
BOOL CSoundEngine::DeleteFile(int iIndex)
{
	if (iIndex < m_iCurrentTrack)
		m_iCurrentTrack--;
	m_Tracks.RemoveAt(iIndex);

	RecalcTrackTimes();

	return TRUE;
}

// Löscht alle Dateien der virtuellen CD aus der Liste.
BOOL CSoundEngine::DeleteAllFiles()
{
	m_iCurrentTrack = 0;
	for (int i=0;i<m_Tracks.GetCount();i++)
		delete m_Tracks[i];

	m_Tracks.RemoveAll();

	RecalcTrackTimes();

	return TRUE;
}

// Die virtuelle CD nach bestimmten Feldern sortieren
void CSoundEngine::Sort(int iSortField, BOOL bAscending)
{
	CString sComp1;
	CString sComp2;
	int iComp1;
	int iComp2;
	bool bTypeInteger;
	bool bChange;

	if (iSortField == FIELD_TRACK_NUMBER)     // Nach der Nummer kann nicht sortiert werden
		return;

	if (m_Tracks.GetCount() < 1)		// Wo nix ist, kann auch nix sortiert werden
		return;

	// Spielendes Lied markieren
	m_Tracks[m_iCurrentTrack]->m_bPlaying = TRUE;

	// Bubble-Sort..... kann man später noch besser machen.
	for (int i=0;i<m_Tracks.GetCount();i++)
	{
		for (int j=i+1;j<m_Tracks.GetCount();j++)
		{
			bTypeInteger = false;
			bChange = false;

			switch(iSortField)
			{
				case FIELD_TRACK_ARTIST:
					sComp1 = m_Tracks[i]->m_sArtist;
					sComp2 = m_Tracks[j]->m_sArtist;
					break;
				case FIELD_TRACK_TITLE: 
					sComp1 = m_Tracks[i]->m_sTitle;
					sComp2 = m_Tracks[j]->m_sTitle;
					break;
				case FIELD_TRACK_LENGTH:
					iComp1 = m_Tracks[i]->m_dwLength;
					iComp2 = m_Tracks[j]->m_dwLength;
					bTypeInteger = true;
					break;
				case FIELD_TRACK_COMMENT: 
					sComp1 = m_Tracks[i]->m_sComment;
					sComp2 = m_Tracks[j]->m_sComment;
					break;
				case FIELD_TRACK_SOUNDFILE: 
					sComp1 = m_Tracks[i]->m_sFilename;
					sComp2 = m_Tracks[j]->m_sFilename;
					break;
			}

			if (!bTypeInteger)
			{
				sComp1.MakeLower();
				sComp2.MakeLower();
				if (bAscending)
				{
					if (sComp2 < sComp1)
						bChange = true;
				}
				else
				{
					if (sComp2 > sComp1)
						bChange = true;
				}
			}
			else
			{
				if (bAscending)
				{
					if (iComp2 < iComp1)
						bChange = true;
				}
				else
				{
					if (iComp2 > iComp1)
						bChange = true;
				}
			}

			if (bChange)
			{
				CSoundEngineItem* pSaveItem = m_Tracks[i];

				m_Tracks[i] = m_Tracks[j];
				m_Tracks[j] = pSaveItem;
			}
		}
	}

	RecalcTrackTimes();

	for (int i=0;i<m_Tracks.GetCount();i++)
	{
		if (m_Tracks[i]->m_bPlaying)
		{
			m_Tracks[i]->m_bPlaying = FALSE;
			m_iCurrentTrack = i;
			break;
		}
	}
}

// Setzt bestimmte Optionen für die einzelnen Lieder.
BOOL CSoundEngine::SetTrackOption(int iTrack, DWORD dwFlag, DWORD dwValue)
{
	switch (dwFlag)
	{
	case VCTF_VORLAUFZEIT:
		m_Tracks[iTrack]->m_dwVorlaufZeitSeconds = dwValue;
		break;
	case VCTF_STOPTIME:
		m_Tracks[iTrack]->m_dwStopTimeSeconds = dwValue;
		break;
	default:
		ASSERT(FALSE);        // Unbekannte Option
		return FALSE;
	}

	return TRUE;
}

// Liefert die Option eines bestimmten Liedes zurück.
DWORD CSoundEngine::GetTrackOption(int iTrack, DWORD dwFlag)
{
	switch (dwFlag)
	{
	case VCTF_VORLAUFZEIT:
		{
			return m_Tracks[iTrack]->m_dwVorlaufZeitSeconds;
		}
	case VCTF_STOPTIME:
		{
			return m_Tracks[iTrack]->m_dwStopTimeSeconds;
		}
	default:
		ASSERT(FALSE);        // Unbekannte Option
		return 0;
	}
}

// Verschiebt das angegebene Lied um den angegebenen Offset (z.B. -1: Eins nach oben).
void CSoundEngine::Move(int iIndex, int iOffset)
{
	ASSERT(iIndex+iOffset >= 0);
	ASSERT(iIndex+iOffset < m_Tracks.GetSize());

	CSoundEngineItem* pSaveItem = m_Tracks[iIndex];

	m_Tracks.RemoveAt(iIndex);

	m_Tracks.InsertAt(iIndex+iOffset, pSaveItem);

	RecalcTrackTimes();

	if (iIndex == m_iCurrentTrack)
		m_iCurrentTrack += iOffset;
	else
	{
		if (iIndex > m_iCurrentTrack && iIndex+iOffset <= m_iCurrentTrack)
			m_iCurrentTrack++;
		else
		{
			if (iIndex < m_iCurrentTrack && iIndex+iOffset >= m_iCurrentTrack)
				m_iCurrentTrack--;
		}
	}
}

// Virtuelle CD durchschütteln (also Reihenfolge verändern)
void CSoundEngine::Shuffle()
{
	int iShuffleCount = m_Tracks.GetSize()*2;    // Sollte reichen

	m_Tracks[m_iCurrentTrack]->m_bPlaying = TRUE;

	srand( (unsigned)time( NULL ) );

	for (int i=0;i<iShuffleCount;i++)
	{
		int iFirst = GetRandom(0, m_Tracks.GetSize()-1);
		int iSecond = GetRandom(0, m_Tracks.GetSize()-1);

		CSoundEngineItem* pSaveItem = m_Tracks[iFirst];

		m_Tracks[iFirst] = m_Tracks[iSecond];
		m_Tracks[iSecond] = pSaveItem;
	}

	RecalcTrackTimes();

	for (int i=0;i<m_Tracks.GetCount();i++)
	{
		if (m_Tracks[i]->m_bPlaying)
		{
			m_Tracks[i]->m_bPlaying = FALSE;
			m_iCurrentTrack = i;
			break;
		}
	}
}

// Schreibt die MP3-Informationen in die MP3-Datei.
BOOL CSoundEngine::WriteMP3Information(const CString& sFilename, const CSoundEngineItemData& MP3Info)
{
	return CMp3Engine::WriteMP3Information(sFilename, MP3Info);
}

BOOL CSoundEngine::IsMediaPresent()
{
	return FALSE;
}

// Speichert die virtuelle CD.
BOOL CSoundEngine::Save(const CString &sFilename)
{
	CStdioFile file;

	if (!file.Open(sFilename, CFile::modeWrite|CFile::modeCreate))
		return FALSE;

	CString sLine;
	sLine.Format(L"FMT=%s%d\n", VIRTUALCDS_EXTENSION, VIRTUALCDS_VERSION);
	file.WriteString(sLine);

	sLine.Format(L"Count=%d\n", m_Tracks.GetSize());
	file.WriteString(sLine);

	for (int i=0;i<m_Tracks.GetSize();i++)
	{
		sLine = m_Tracks[i]->GetString(sFilename);
		file.WriteString(sLine);
	}

	file.Close();

	return TRUE;
}

// Läd eine Virtuelle CD (Programm).
BOOL CSoundEngine::Load(const CString &sFilename, CWnd* pWndParent, bool bSilent)
{
	CStdioFile file;

	if (!file.Open(sFilename, CFile::modeRead))
		return FALSE;

	CString sLine;
	file.ReadString(sLine);

	CString sFormat;
	sFormat.Format(L"FMT=%s%d", VIRTUALCDS_EXTENSION, VIRTUALCDS_VERSION);
	if (sLine != sFormat)
	{
		// Keine hpd-Datei!
		file.Close();
		return FALSE;
	}

	// Anzahl der Dateien lesen.
	file.ReadString(sLine);
	if (sLine.Left(6) != "Count=")
	{
		// Keine hvc-Datei!
		file.Close();
		return FALSE;
	}
	
	int iCount = _wtoi(sLine.Mid(6));

	for (int i=0;i<m_Tracks.GetCount();i++)
		delete m_Tracks[i];

	m_Tracks.RemoveAll();

	CLoadSoundFilesDlg* pLoadSoundFilesDlg;

	if (!bSilent)
	{
		pLoadSoundFilesDlg = new CLoadSoundFilesDlg(pWndParent);
		pLoadSoundFilesDlg->Create();

		pLoadSoundFilesDlg->m_ProgressCtrl.SetRange(0, iCount);
	}

	iCount = 0;
	CSoundEngineItemData item;
	while (item.Read(&file, sFilename))
	{
		if (!bSilent)
		{
			// Damit man abbrechen kann.
			MSG msg;

			while ( ::PeekMessage( &msg, NULL, 0, 0, PM_NOREMOVE ) ) 
			{
				if (!AfxGetApp() || !AfxGetApp()->PumpMessage()) 
				{ 
					::PostQuitMessage(0); 
					return FALSE; 
				} 
			}

			if (pLoadSoundFilesDlg->m_bCanceled)
				break;
		}

		// Abfragen, ob die Datei noch vorhanden ist!
		if (!_waccess(item.m_sFilename, 0))
		{
			if (!bSilent)
				pLoadSoundFilesDlg->m_FilenameCtrl.SetWindowText(::GetFileNameFromPath(item.m_sFilename));

// Ok, hier haben wir jetzt ein kleines Problem. Wir müssen wohl ein Pseudo-SoundEngineItem
// anlegen, bei dem dann erst beim Abspielen geprüft wird, um welchen Typ es sich handelt.
//!!!!!!!!!!!!!			m_Tracks.Add(item);
			AddFile(item.m_sFilename);
		}

		if (!bSilent)
			pLoadSoundFilesDlg->m_ProgressCtrl.SetPos(iCount++);
	}

	file.Close();

	if (!bSilent)
		delete pLoadSoundFilesDlg;

	// Ein vorhandenes virtuelles Verzeichnis löschen
//!!!!!!!!!!????????	SetVirtualDirectory("");

	RecalcTrackTimes();

	return TRUE;
}

CString CSoundEngine::GetDeviceName()
{
	return m_sDeviceName;
}

void CSoundEngine::SetDeviceName(const CString& sDeviceName)
{
	m_sDeviceName = sDeviceName;
}

BOOL CSoundEngine::ExportM3U(const CString& sM3UFilename)
{
	CStdioFile file;

	if (!file.Open(sM3UFilename, CFile::modeCreate|CFile::modeReadWrite))
	{
		MessageResBox(TEXT_CANTWRITE, MB_OK|MB_ICONEXCLAMATION, sM3UFilename);
		return FALSE;
	}

	file.WriteString((CString)"#EXTM3U" + L"\n");        // Das ist eine M3U-datei

	for (int i=0;i<GetNumberOfTracks();i++)
	{
		CString sFilename = GetTrack(i)->m_sFilename;

		CString sLine;
		if (GetTrack(i)->m_sArtist == "" && GetTrack(i)->m_sTitle == "")
			sLine.Format(L"#EXTINF:%d,%s\n", GetTrack(i)->m_dwLength/1000, CMisc::GetPureFileName(GetTrack(i)->m_sFilename));
		else
			sLine.Format(L"#EXTINF:%d,%s - %s\n", GetTrack(i)->m_dwLength/1000, GetTrack(i)->m_sArtist, GetTrack(i)->m_sTitle);
		file.WriteString(sLine);

		CString sRelFilename;
		// Die Pfade werden nun relativ zur m3u-Datei gespeichert.
		PathRelativePathTo(sRelFilename.GetBuffer(_MAX_PATH), sM3UFilename, 0, sFilename, 0);
		sRelFilename.ReleaseBuffer();
		if (sRelFilename.Left(2) == ".\\")
			sRelFilename = sRelFilename.Mid(2);
		file.WriteString(sRelFilename + L"\n");
	}

	file.Close();

	return TRUE;
}

BOOL CSoundEngine::ImportM3U(const CString& sM3UFilename)
{
	CStdioFile file;

	if (!file.Open(sM3UFilename, CFile::modeRead))
	{
		MessageResBox(TEXT_CANTOPEN, MB_OK|MB_ICONEXCLAMATION, sM3UFilename);
		return FALSE;
	}

	CString sM3UPath = GetPathFromFileName(sM3UFilename);
	CString sLine;

	file.ReadString(sLine);
	
	if (sLine != "#EXTM3U")        // Das ist eine M3U-datei
	{
		file.Close();
		return FALSE;
	}

	CString sArtist;
	CString sTitle;
	long lLength = 0;
	BOOL bDataGiven = FALSE;
	
	while (file.ReadString(sLine))
	{
		if (sLine.Left(1) == "#")
		{
			if (sLine.Left(8) == "#EXTINF:")
			{
				CString str = sLine.Mid(8);

				lLength = _wtoi(str)*1000;
				int iPos = str.Find(L",");
				if (iPos >= 0)
				{
					str = sLine.Mid(iPos+1);
					iPos = str.Find(L"-");
					if (iPos > 0)
					{
						sArtist = str.Left(iPos);
						sArtist.Trim();
						sTitle = str.Mid(iPos+1);
						sTitle.Trim();
						bDataGiven = TRUE;
					}
				}
			}
		}
		else
		{
			CString sAbsFilename;
			CString sPath = CMisc::CombinePathWithFileName(sM3UPath, sLine);
			PathCanonicalize(sAbsFilename.GetBuffer(_MAX_PATH), sPath);
			sAbsFilename.ReleaseBuffer();
			AddFile(sAbsFilename);
			bDataGiven = FALSE;
		}
	}

	file.Close();

	return TRUE;
}

// Globale Initialisierung der MP3-Routinen. Darf nur einmal gemacht werden, daher static!
bool CSoundEngine::GlobalInit()
{
	if (m_bMp3PlayerInitialized)
		return true;

    FMOD::System_Create(&m_pFMODSystem);

	switch (((CHitbaseWinAppBase*)AfxGetApp())->m_config.m_iVirtualCDOutputDevice)
	{
	case 0:
		m_pFMODSystem->setOutput(FMOD_OUTPUTTYPE_DSOUND);
		break;
	case 1:
		m_pFMODSystem->setOutput(FMOD_OUTPUTTYPE_WINMM);
		break;
	case 2:
		m_pFMODSystem->setOutput(FMOD_OUTPUTTYPE_ASIO);
		break;
	default:
		ASSERT(false);
	}

	m_pFMODSystem->setDriver(Big3::Hitbase::Configuration::Settings::Current->OutputDevice);

	switch (((CHitbaseWinAppBase*)AfxGetApp())->m_config.m_iVirtualCDBufferSize)
	{
	case 0:
		break;
	case 1:
		m_pFMODSystem->setDSPBufferSize(75, 8);
		break;
	case 2:
		m_pFMODSystem->setDSPBufferSize(100, 8);
		break;
	case 3:
		m_pFMODSystem->setDSPBufferSize(150, 8);
		break;
	case 4:
		m_pFMODSystem->setDSPBufferSize(200, 8);
		break;
	case 5:
		m_pFMODSystem->setDSPBufferSize(500, 8);
		break;
	default:
		ASSERT(false);
	}

	FMOD_RESULT res = m_pFMODSystem->init(32, FMOD_INIT_NORMAL, 0);
	if (res != FMOD_OK)
	{
		AfxMessageBox(CString(FMOD_ErrorString(res)));
		m_pFMODSystem->close();
		return false;
	}

	m_bMp3PlayerInitialized = true;

	return true;
}

// Globale Deinitialisierung der MP3-Routinen. Darf nur einmal gemacht werden, daher static!
bool CSoundEngine::GlobalExit()
{
	if (!m_bMp3PlayerInitialized)
		return true;

	if (m_pFMODSystem)
	{
		m_pFMODSystem->close();
		m_pFMODSystem = NULL;
	}

	m_bMp3PlayerInitialized = false;

	return true;
}


void* F_API CSoundEngine::FSoundDSPCallback(void *originalbuffer, void *newbuffer, int length, void* param) 
{ 
	CMp3Player* pThePlayer = (CMp3Player*)param;

	if (pThePlayer->m_pMp3Engine->m_iBitsPerSample == 1)	// 8 Bit wird z.Zt. nicht unterstützt
	{
		memcpy(newbuffer, originalbuffer, length);
		return newbuffer;
	}

	if (pThePlayer->m_pMp3Engine->m_iChannels == 2)
	{
		signed short   *dspbuff = (signed short *)newbuffer; // source pointer from the data 
		signed short   *srcleft, *srcright; 

		srcleft   = ((signed short *)newbuffer); 
		srcright  = ((signed short *)newbuffer)+1; 

		length <<= 1;   // *2 for stereo (number of 16 bit samples) 

		signed int l, r; 
		int iMaxPeek = 0;

	//	double dblGlobalVolumeAdjust = 32767.0 / (double)iMaxPeek;

		double dblGlobalVolumeAdjust = pThePlayer->m_pMp3Engine->GetAutoVolumeAdjustValue();

		// Zuerst den maximalen Peek ermitteln
		for (int count=0; count<length; count+=2) 
		{ 
			l = (signed int)srcleft[count]; 
			r = (signed int)srcright[count];

			if (abs(l) > iMaxPeek)
				iMaxPeek = abs(l);
			if (abs(r) > iMaxPeek)
				iMaxPeek = abs(r);
		}


		if (32767.0 / (double)iMaxPeek < dblGlobalVolumeAdjust)
		{
			dblGlobalVolumeAdjust = 32767.0 / (double)iMaxPeek;
			pThePlayer->m_pMp3Engine->SetAutoVolumeAdjustValue(dblGlobalVolumeAdjust);
		}

		for (int count=0; count<length; count+=2) 
		{ 
			l = (signed int)srcleft[count]; 
			r = (signed int)srcright[count];

			l = (int)((double)l * (double)dblGlobalVolumeAdjust); 
			r = (int)((double)r * (double)dblGlobalVolumeAdjust); 

			if      (l < -32768) l = -32768; 
			else if (l >  32767) l =  32767; 
			if      (r < -32768) r = -32768; 
			else if (r >  32767) r =  32767; 

			dspbuff[count] = (signed short)(l); 
			dspbuff[count+1] = (signed short)(r); 
		}
	}
	else
	{
		// Mono
		signed short   *dspbuff = (signed short *)newbuffer; // source pointer from the data 
		signed short   *srcbuff;

		CMp3Player* pThePlayer = (CMp3Player*)param;

		srcbuff   = ((signed short *)newbuffer); 

		signed int peek; 
		int iMaxPeek = 0;

		double dblGlobalVolumeAdjust = pThePlayer->m_pMp3Engine->GetAutoVolumeAdjustValue();

		// Zuerst den maximalen Peek ermitteln
		for (int count=0; count<length; count++) 
		{ 
			peek = (signed int)srcbuff[count]; 

			if (abs(peek) > iMaxPeek)
				iMaxPeek = abs(peek);
		}

		if (32767.0 / (double)iMaxPeek < dblGlobalVolumeAdjust)
		{
			dblGlobalVolumeAdjust = 32767.0 / (double)iMaxPeek;
			pThePlayer->m_pMp3Engine->SetAutoVolumeAdjustValue(dblGlobalVolumeAdjust);
		}

		for (int count=0; count<length; count++) 
		{ 
			peek = (signed int)srcbuff[count]; 

			peek = (int)((double)peek * (double)dblGlobalVolumeAdjust); 

			if      (peek < -32768) peek = -32768; 
			else if (peek >  32767) peek =  32767; 

			dspbuff[count] = (signed short)(peek); 
		}
	}

	return newbuffer; 
} 

BOOL CSoundEngine::SetSpeed(double dblSpeed)
{
	return TRUE;
}


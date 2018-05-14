// CDText.cpp: Implementierung der Klasse CCDText.
//
// ACHTUNG! Der SRB_Status member MUSS in allen ASPI-Strukturen als
// volatile definiert sein!!!
// Falls also eine neue wnaspi32.h eingespielt wird, muss darauf geachtet werden!
// BYTE        volatile SRB_Status;                     // 01/001 ASPI command status byte
/////////////////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "SoundEngineIntern.h"
#include "../hitmisc/hitcommn.h"
#include "../../App/hitbase/resource.h"
#include "CDText.h"

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

extern DWORD __declspec (dllimport) (*pfnGetASPI32SupportInfo)(void);
extern DWORD __declspec (dllimport) (*pfnSendASPI32Command)(LPSRB);

//////////////////////////////////////////////////////////////////////
// Konstruktion/Destruktion
//////////////////////////////////////////////////////////////////////

CCDText::CCDText()
{
}

CCDText::~CCDText()
{

}

BOOL CCDText::Init()
{
	// Then, inside your Main/WinMain function:
	
//	ADD_FUNCTION_ENTRY;

	/* Load Real ASPI Layer, WNASPI32.DLL */
/*	m_hWNASPI32 = LoadLibrary("WNASPI32.DLL");
	
	if (!m_hWNASPI32) // If Load Failed
		return FALSE;
	
	ADD_FUNCTION_CHECKPOINT("ASPI driver loaded");

	m_pGetASPI32SupportInfo = (GETASPI32SUPPORTINFO) GetProcAddress(m_hWNASPI32,"GetASPI32SupportInfo");
	m_pSendASPI32Command = (SENDASPI32COMMAND) GetProcAddress(m_hWNASPI32,"SendASPI32Command");
	m_pGetASPI32Buffer = (GETASPI32BUFFER) GetProcAddress(m_hWNASPI32,"GetASPI32Buffer");
	m_pFreeASPI32Buffer = (FREEASPI32BUFFER) GetProcAddress(m_hWNASPI32,"FreeASPI32Buffer");
	m_pTranslateASPI32Address = (TRANSLATEASPI32ADDRESS) GetProcAddress(m_hWNASPI32,"TranslateASPI32Address");

	// ASPI-Treiber nicht gefunden.
	if (!m_pGetASPI32SupportInfo || !m_pSendASPI32Command)
	{
		ADD_FUNCTION_CHECKPOINT("ASPI driver init failed!");
		ADD_FUNCTION_EXIT;
		return FALSE;
	}

	ADD_FUNCTION_CHECKPOINT("ASPI driver initialized successfully");
*/

	// Jetzt über AKRip
//	m_pGetASPI32SupportInfo = MyGetASPI32SupportInfo;
//	m_pSendASPI32Command = MySendASPI32Command;
//	m_pGetASPI32Buffer = (GETASPI32BUFFER) GetProcAddress(m_hWNASPI32,"GetASPI32Buffer");
//	m_pFreeASPI32Buffer = (FREEASPI32BUFFER) GetProcAddress(m_hWNASPI32,"FreeASPI32Buffer");
//	m_pTranslateASPI32Address = (TRANSLATEASPI32ADDRESS) GetProcAddress(m_hWNASPI32,"TranslateASPI32Address");

//	ADD_FUNCTION_EXIT;
	return TRUE;
}

BOOL CCDText::Release()
{
	return TRUE;
}

BOOL CCDText::ReadCDText(int iLogicalDrive)
{
//	ADD_FUNCTION_ENTRY;

	BYTE HACount;
	BYTE ASPIStatus;
	DWORD SupportInfo;
	int iDevice=0;
	
	CDTextLog::AddLog("ASPI - query support info!");
	SupportInfo = MyGetASPI32SupportInfo();
	HACount = LOBYTE(LOWORD(SupportInfo));
	ASPIStatus = HIBYTE(LOWORD(SupportInfo));

	if (ASPIStatus != SS_COMP && ASPIStatus != SS_NO_ADAPTERS)
	{
		CDTextLog::AddLog("no adapters found!");
//		ADD_FUNCTION_EXIT;
		return FALSE;
	}

	CDTextLog::AddLog("ASPI - query support info successful!");

	SRB_HAInquiry HAInquiry; // Create a data struct for our command.

	int iNumberOfCDROMs = 0;

	for (int i=0;i<HACount;i++)
	{
		memset( &HAInquiry, 0, sizeof(SRB_HAInquiry) ); // Zero out the SRB
		HAInquiry.SRB_Cmd = SC_HA_INQUIRY;
		HAInquiry.SRB_HaID = i;
		MySendASPI32Command( (LPSRB)&HAInquiry ); // Give it a pointer to our packet
		
		SRB_GDEVBlock GDEVBlock;
		memset( &GDEVBlock, 0, sizeof( SRB_GDEVBlock ) ); // Zero out all fields
		GDEVBlock.SRB_Cmd = SC_GET_DEV_TYPE;
		GDEVBlock.SRB_HaID = i; // Host Adapter

		for (int iSCSIID=0;iSCSIID<8;iSCSIID++)
		{
			for (int iLUN=0;iLUN<8;iLUN++)
			{
				GDEVBlock.SRB_Target = iSCSIID; // SCSI ID
				GDEVBlock.SRB_Lun = iLUN;
				MySendASPI32Command( (LPSRB)&GDEVBlock );
				
				// Now, check the SRB_Status field to see if it completed properly with a SS_COMP.
				if ( GDEVBlock.SRB_Status != SS_COMP )
				{ // Handle Error Code
				}
				else // No error
				{
					if ( GDEVBlock.SRB_DeviceType == DTYPE_CDROM )
					{ // I found it!
						iNumberOfCDROMs++;

						CString sLog;
						sLog.Format(L"Found CD in drive: %d, %d, %d", GDEVBlock.SRB_HaID, GDEVBlock.SRB_Target, GDEVBlock.SRB_Lun);
						CDTextLog::AddLog(gcnew System::String(sLog));
						ScsiInquiry ( GDEVBlock.SRB_HaID, GDEVBlock.SRB_Target, GDEVBlock.SRB_Lun) ;

						CDTextLog::AddLog("Read CD-Text");
						if (ReadCDTextFromDevice( GDEVBlock.SRB_HaID, GDEVBlock.SRB_Target, GDEVBlock.SRB_Lun))
						{
							CDTextLog::AddLog("CD-Text found!");
//							ADD_FUNCTION_EXIT;
							return TRUE;
						}
/*						if (iDevice == iLogicalDrive)
						{
							iHaID = GDEVBlock.SRB_HaId;
							iTarget = GDEVBlock.SRB_Target;
							iLun = GDEVBlock.SRB_Lun;
						}*/

						iDevice++;
					}
				}
			}
		}
	}

	if (!iNumberOfCDROMs)
	{
		BOOL bWarningBoxDisableIncompatibleASPI = CConfig::ReadGlobalRegistryKeyInt(L"WarningBoxDisableIncompatibleASPI", FALSE);
		if (!bWarningBoxDisableIncompatibleASPI)
		{
			CCommonCheckDlg dlgWarning(IDD_COMMON_CHECKDLG, get_string(IDS_INCOMPATIBLE_ASPI_DRIVER));
			dlgWarning.DoModal();
			CConfig::WriteGlobalRegistryKeyInt(L"WarningBoxDisableIncompatibleASPI", dlgWarning.m_Check1 ? TRUE : FALSE);
		}
	}

//	ADD_FUNCTION_EXIT;

	return FALSE;
}

BOOL CCDText::ReadCDTextFromDevice(int iHaID, int iTarget, int iLun)
{
//	ADD_FUNCTION_ENTRY;

	tagCDText CDText;
	memset(&CDText, 0, sizeof(CDText));

	// Initialize required variables before calling function
	SRB_ExecSCSICmd SRB;
	memset(&SRB, 0, sizeof(SRB));
	SRB.SRB_Cmd = SC_EXEC_SCSI_CMD;
	SRB.SRB_HaID = iHaID;
	SRB.SRB_Flags = SRB_DIR_IN; // read data from cd
	SRB.SRB_Hdr_Rsvd = 0;
	SRB.SRB_Target = iTarget;
	SRB.SRB_Lun = iLun;
	SRB.SRB_Rsvd1 = 0;
	SRB.SRB_BufLen = sizeof(CDText); // Number of bytes to read.
	SRB.SRB_BufPointer = (unsigned char*)&CDText; // Point to structure to hold data read
	SRB.SRB_SenseLen = SENSE_LEN;
	SRB.SRB_CDBLen = 10; // this is a 10 Byte CDB
	SRB.SRB_PostProc = NULL;
		
	// // init command descriptor block (CDB)
	SRB.CDBByte[0] = SCSI_READ_TOC; // command-code
	SRB.CDBByte[2] = 0x5; // CD-TEXT lesen
	SRB.CDBByte[7] = 0x7F; // allocation length 32KByte

	DWORD dwASPIStatus = MySendASPI32Command((LPSRB)&SRB);
		
	while ( SRB.SRB_Status == SS_PENDING );

	if (SRB.SRB_Status == SS_COMP)
	{
		CDTextLog::AddLog("CD-TEXT found!");

		// JUS 03.03.2003: Zu Debug-Zwecken in eine Temp-Datei schreiben
/*		CFile file;
		if (file.Open("d:\\cdtext.tmp", CFile::modeRead))
		{
			file.Read(&CDText, file.GetLength());
			file.Close();
		}
*/

		long lDataLength = CDText.cDataLength[0] * 0x100 + CDText.cDataLength[1] - 2;
		int iNumberOfPacks = lDataLength / sizeof(CDText.Packs[0]);

		CString strDebug;
		strDebug.Format(L"CD-Text Buffer: lDataLength: %d, iNumberOfPacks: %d", lDataLength, iNumberOfPacks);
		CDTextLog::AddLog(gcnew System::String(strDebug));

		// Success
		if (lDataLength < 0)     // Nichts gefunden (cDataLength = 0?)
		{
//			ADD_FUNCTION_EXIT;
			return FALSE;
		}

		// JUS 03.03.2003: Zu Debug-Zwecken in eine Temp-Datei schreiben
		int bWriteLog = CConfig::ReadGlobalRegistryKeyInt(L"Log", 0);
		if (bWriteLog > 0)
		{
			CFile file;
			if (file.Open(L"c:\\cdtext.dat", CFile::modeReadWrite|CFile::modeCreate))
			{
				file.Write(&CDText, lDataLength);
				file.Close();
			}
		}
		
		CDTextLog::AddLog("Reading track titles!");
		m_saTrackTitle.RemoveAll();
		if (!EncodeGroup(0x80, CDText.Packs, iNumberOfPacks, m_saTrackTitle))
		{
//			ADD_FUNCTION_EXIT;
			return FALSE;
		}

		CDTextLog::AddLog("Reading track artists!");
		m_saTrackArtist.RemoveAll();
		if (!EncodeGroup(0x81, CDText.Packs, iNumberOfPacks, m_saTrackArtist))
		{
//			ADD_FUNCTION_EXIT;
			return FALSE;
		}

		CDTextLog::AddLog("Reading track ISRC!");
		m_saUPCCodes.RemoveAll();
		EncodeGroup(0x8E, CDText.Packs, iNumberOfPacks, m_saUPCCodes);

//		ADD_FUNCTION_EXIT;
		return TRUE;
	}		
	else
	{
//		ADD_FUNCTION_EXIT;
		return FALSE;
	}
}

//--------------------------------------------------------------------------

//Please note that "strsub" is a function that retrieves a substring from a
//string:

//--------------------------------------------------------------------------
CString CCDText::strsub ( char* src, int from, int to ) 
{
	CString sSub;

	for (int i=from;i<to;i++)
		sSub += src[i];

	return sSub;
}
//--------------------------------------------------------------------------


CString CCDText::ScsiInquiry ( BYTE HA_num, BYTE SCSI_Id, BYTE SCSI_Lun ) 
{
	CString sReturnString;
	char szBuffer [ 37 ];

	strcpy ( szBuffer, "" );
	SRB_ExecSCSICmd srbExec;
	memset ( &srbExec, 0, sizeof ( SRB_ExecSCSICmd ) );
	srbExec.SRB_Cmd = SC_EXEC_SCSI_CMD;
	srbExec.SRB_HaID = HA_num;
	srbExec.SRB_Flags = SRB_DIR_IN;
	srbExec.SRB_Target = SCSI_Id;
	srbExec.SRB_Lun = SCSI_Lun;
	srbExec.SRB_BufLen = 36;
	srbExec.SRB_BufPointer = (unsigned char*)&szBuffer[0];
	srbExec.SRB_SenseLen = SENSE_LEN;
	srbExec.SRB_CDBLen = 6;
	srbExec.CDBByte [ 0 ] = SCSI_INQUIRY;
	srbExec.CDBByte [ 4 ] = 36;  // allocation length per szBuffer [ ]
	
	MySendASPI32Command ( ( LPSRB ) &srbExec );
	
	// ACHTUNG! Der SRB_Status member MUSS volatile definiert sein!
	while ( srbExec.SRB_Status == SS_PENDING );

	if ( srbExec.SRB_Status != SS_COMP )
		sReturnString = "Inquiry error.";
	else
	{
		sReturnString = strsub(szBuffer, 8, 36);
	}

	return sReturnString;
}


// Album / Tracktitel aus Group 0x80 encodieren, nur Single Byte Character Text (ID4 Bit7 muss 0 sein)
// Interpret aus Group 0x81
BOOL CCDText::EncodeGroup(unsigned char cType, struct tagPacks* pPacks, int iNumberOfPacks, CStringArray& saTitle)
{
//	ADD_FUNCTION_ENTRY;

/*	for (l=0;l<lPacks;l++)
	{
		if (pPacks[l].Header.Type == cType)
		{
			if (pPacks[l].Header.ID4 & 0x80)
			{
				// DBCC not supported at this place
				return FALSE;
			}
	     
			break;
		}
	}
*/
	int iTrack = 0;

	for (int iPack=0;iPack < iNumberOfPacks;iPack++)
	{
		if (pPacks[iPack].Header.Type == cType && !pPacks[iPack].Header.bDBC && !pPacks[iPack].Header.block)
		{
			int iTrack = pPacks[iPack].Header.Track;

			for (int k=0;k<12;k++)
			{
				// Text bereits für nächsten Track?
				if (pPacks[iPack].Data[k] == 0)
				{
					iTrack++;
					saTitle.SetAtGrow(iTrack, L"");
					continue;
				}

				if (pPacks[iPack].Data[k] == 9) // ' TAB String wiederholt sich
				{
					iTrack++;
					// in der nächsten Zeile ist er heute einmal abgestürzt..... weiß nicht warum. 16.03.2004
					// JUS: 27.01.2008: Jetz weiß ich auch warum! ;-) Elemente, die man in die Liste hinzufügt,
					// dürfen nicht bereits Teil der Liste sein, weil SetAtGrow ein realloc macht, und damit das
					// Element freigibt... wenn man Glück hat, klappt es oder auch nicht ;-)
					// Deshalb legen wir vorher eine Kopie an und fügen diese dann hinzu.
					// Fixed für 11.0.2.112
					CString str = saTitle[iTrack-1];
					saTitle.SetAtGrow(iTrack, str);
				}
				else
				{
					if (pPacks[iPack].Data[k] != 0)
					{
						CString str;
						str.Format(L"%c", pPacks[iPack].Data[k]);
						if (saTitle.GetSize() <= iTrack)
							saTitle.SetAtGrow(iTrack, L"");
						saTitle[iTrack] +=  str;
					}
				}
			}
		}
	}


/*
	while (1)
	{
		while (l < lPacks && (pPacks[l].Header.Type != cType || (pPacks[l].Header.bDBC)))
		{
			l++;
			break;
		}

		if (l >= lPacks)
			break;

		if (pPacks[l].Header.ID4 & 0x80)
		{
			// DBCC not supported at this place
			return FALSE;
		}
	    
     // encode tracktitel
		int iTrack = pPacks[l].Header.Track;
//        if ((pPacks[l].Header.ID4 & 0xf) == 0 || saTitle.GetSize() <= iTrack)
//			saTitle.SetAtGrow(iTrack, "");

		for (int k=0;k<12;k++)
		{
			// Text bereits für nächsten Track?
            if (pPacks[l].Data[k] == 0)
			{
                 iTrack++;
				 k++;
                 if (k > 11)
					break;
			}

            if (pPacks[l].Data[k] == 9) // ' TAB String wiederholt sich
			{
				iTrack++;
                saTitle.SetAtGrow(iTrack, saTitle[iTrack-1]);
                if (k > 11)
					break;
			}
            else
			{
				if (pPacks[l].Data[k] != 0)
				{
					CString str;
					str.Format("%c", pPacks[l].Data[k]);
					if (saTitle.GetSize() <= iTrack)
						saTitle.SetAtGrow(iTrack, "");
					saTitle[iTrack] +=  str;
				}
			}
		}
		
		l++;
	}
*/

	if (!saTitle.GetCount())       // Dann wurde keine Information mit diesem Typ gefunden
	{
		CDTextLog::AddLog("CD-Text Info empty!");
//		ADD_FUNCTION_EXIT;
		return FALSE;
	}

	for (int i=0;i<saTitle.GetSize();i++)
	{
		saTitle[i].Trim();
		saTitle[i] = saTitle[i].Left(100);    // Maximale Länge
	}

//	ADD_FUNCTION_EXIT;
	return TRUE;
}

CString CCDText::GetTrackTitle(int iTrack)
{
	// Sicherheitsabfrage, damit wir hier nicht abstürzen, falls die CD-Text
	// Informationen fehlerhaft sind.
	if (iTrack+1 >= m_saTrackTitle.GetCount())
		return L"";
	else
		return m_saTrackTitle[iTrack+1];
}

CString CCDText::GetTrackArtist(int iTrack)
{
	// Sicherheitsabfrage, damit wir hier nicht abstürzen, falls die CD-Text
	// Informationen fehlerhaft sind.
	if (iTrack+1 >= m_saTrackArtist.GetCount())
		return L"";
	else
		return m_saTrackArtist[iTrack+1];
}

CString CCDText::GetTrackISRC(int iTrack)
{
	// Sicherheitsabfrage, damit wir hier nicht abstürzen, falls die CD-Text
	// Informationen fehlerhaft sind.
	if (iTrack+1 >= m_saUPCCodes.GetCount())
		return L"";
	else
		return m_saUPCCodes[iTrack+1];
}

CString CCDText::GetCDTitle()
{
	// Sicherheitsabfrage, damit wir hier nicht abstürzen, falls die CD-Text
	// Informationen fehlerhaft sind.
	if (m_saTrackTitle.GetCount() < 1)
		return L"";
	else
		return m_saTrackTitle[0];
}

CString CCDText::GetCDArtist()
{
	// Sicherheitsabfrage, damit wir hier nicht abstürzen, falls die CD-Text
	// Informationen fehlerhaft sind.
	if (m_saTrackArtist.GetCount() < 1)
		return L"";
	else
		return m_saTrackArtist[0];
}

CString CCDText::GetUPCCode()
{
	// Sicherheitsabfrage, damit wir hier nicht abstürzen, falls die CD-Text
	// Informationen fehlerhaft sind.
	if (m_saUPCCodes.GetCount() < 1)
		return L"";
	else
		return m_saUPCCodes[0];
}


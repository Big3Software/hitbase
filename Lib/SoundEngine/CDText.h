// CDText.h: Schnittstelle für die Klasse CCDText.
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_CDTEXT_H__9CF1DD58_01AF_4B5A_A3D4_E1E8A5F01377__INCLUDED_)
#define AFX_CDTEXT_H__9CF1DD58_01AF_4B5A_A3D4_E1E8A5F01377__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

// ACHTUNG! Der SRB_Status member MUSS in allen ASPI-Strukturen als
// volatile definiert sein!!!
// Falls also eine neue wnaspi32.h eingespielt wird, muss darauf geachtet werden!
// BYTE        volatile SRB_Status;                     // 01/001 ASPI command status byte
/////////////////////////////////////////////////////////////////////////////////

//#include "wnaspi32.h"
#include "scsidefs.h"
#include "../akrip/akrip32.h"
#include "../hitmisc/config.h"
/* Function Prototypes for WNASPI32.DLL */
//typedef DWORD (*MYGETASPI32SUPPORTINFO)(void);
//typedef DWORD (*MYSENDASPI32COMMAND)(LPSRB);
//typedef BOOL (*GETASPI32BUFFER)( PASPI32BUFF );
//typedef BOOL (*FREEASPI32BUFFER)( PASPI32BUFF );
//typedef BOOL (*TRANSLATEASPI32ADDRESS)( PDWORD, PDWORD );

#pragma pack(1)

// CD-TEXT types
struct tagPackHeader
{
	unsigned char Type;     // ID1: Pack Type Indicator
                 // 80h(128dez): = Album (ID2=0) und Tracktitel(ID2=01h..63h)
                 // 81h(129dez): = Interpret
	unsigned char Track;    // ID2: Track Nummer
	unsigned char Element;  // ID3: Packet Nummer (fortlaufend)

	unsigned char characterPosition	:4;  // Bit0-3: Character Position (zeigt die Pos. im Text an)
	unsigned char block			:3;      // Bit4-6: Block-Number
	unsigned char bDBC			:1;      // Bit7: DBCC (double Byte characters)
};

struct tagPacks // Packet Descriptor (je 18 Bytes)
{
   struct tagPackHeader Header;
   unsigned char Data[12];        // Text daten
   unsigned char CRC[2];          // Reserved Control-Bytes
};

typedef struct {
   unsigned char cDataLength[2];
   unsigned char cReserved[2];
   struct tagPacks Packs[255];
} tagCDText;

typedef struct {
  BYTE ha;
  BYTE tgt;
  BYTE lun;
  BYTE driveLetter;
  BOOL bUsed;
  HANDLE hDevice;
  BYTE inqData[36];
} DRIVE;

typedef struct {
  BYTE numAdapters;
  DRIVE drive[26];
} SPTIGLOBAL;

#pragma pack()

class SOUNDENGINE_INTERFACE CCDText  
{
public:
	CString GetTrackArtist(int iTrack);
	CString GetCDArtist();
	CString GetCDTitle();
	CString GetTrackTitle(int iTrack);
	CString GetTrackISRC(int iTrack);
	CString GetUPCCode();
	CCDText();
	virtual ~CCDText();

	BOOL Init();
	BOOL Release();
	BOOL ReadCDText(int iLogicalDrive);

private:
	BOOL LoadASPIDLL();

	/* Function Prototypes for WNASPI32.DLL */
//	MYGETASPI32SUPPORTINFO m_pGetASPI32SupportInfo;
//	MYSENDASPI32COMMAND m_pSendASPI32Command;
//	GETASPI32BUFFER m_pGetASPI32Buffer;
//	FREEASPI32BUFFER m_pFreeASPI32Buffer;
//	TRANSLATEASPI32ADDRESS m_pTranslateASPI32Address;

//	HINSTANCE m_hWNASPI32; // Handle to ASPI

	CString strsub ( char* src, int from, int to ) ;
	CString ScsiInquiry ( BYTE HA_num, BYTE SCSI_Id, BYTE SCSI_Lun ) ;
	BOOL EncodeGroup(unsigned char cType, struct tagPacks* pPacks, int iNumberOfPacks, CStringArray& saTrackTitle);
	BOOL ReadCDTextFromDevice(int iHaID, int iTarget, int iLun);

	CStringArray m_saTrackTitle;
	CStringArray m_saTrackArtist;
	CStringArray m_saUPCCodes;
};

ref class CDTextLog
{
public:
	static void AddLog(System::String^ str)
	{
		try
		{
			int bWriteLog = CConfig::ReadGlobalRegistryKeyInt(L"Log", 0);
			if (bWriteLog > 0)
				System::IO::File::AppendAllText(L"c:\\cdtext.log", str + "\r\n");
		}
		catch (System::Exception^ e)
		{
		}
	}
};

#endif // !defined(AFX_CDTEXT_H__9CF1DD58_01AF_4B5A_A3D4_E1E8A5F01377__INCLUDED_)

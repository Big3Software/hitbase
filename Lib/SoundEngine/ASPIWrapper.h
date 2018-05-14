#pragma once

#include "../akrip/myaspi32.h"
#include "../akrip/scsidefs.h"
#include "../akrip/aspilib.h"
#include "../akrip/akrip32.h"
#include "../akrip/scsipt.h"

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

class CASPIWrapper
{
public:
	CASPIWrapper(void);
	~CASPIWrapper(void);

	static int InitSCSIPT( void );
	static int DeinitSCSIPT( void );
	static DWORD SPTIGetASPI32SupportInfo( void );
	static DWORD SPTISendASPI32Command( LPSRB lpsrb );
	static BOOL UsingSCSIPT( void );
	static void SPTIOpenCDHandle( BYTE ha, BYTE tgt, BYTE lun );
	static void GetDriveInformation( BYTE i, DRIVE *pDrive );
	static BYTE SPTIGetNumAdapters( void );
	static BYTE SPTIGetDeviceIndex( BYTE ha, BYTE tgt, BYTE lun );
	static DWORD SPTIHandleHaInquiry( LPSRB_HAInquiry lpsrb );
	static DWORD SPTIGetDeviceType( LPSRB_GDEVBlock lpsrb );
	static DWORD SPTIExecSCSICommand( LPSRB_ExecSCSICmd lpsrb, BOOL bBeenHereBefore );
	static HANDLE GetFileHandle( BYTE i );
};

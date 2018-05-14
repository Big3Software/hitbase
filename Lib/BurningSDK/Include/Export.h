#ifndef __BS_EXPORT_H__
#	define __BS_EXPORT_H__


#	include "Definitions.h"

#	ifdef __cplusplus
extern "C" {
#	endif

#	define BS_CALL		__stdcall


//// license key checking
__int32 BS_CALL	CheckLicenseKey				(const char chLicenseKey[35]);		// null-terminated
//// end (license key checking)


//// initialization / deinitialization
#	ifdef __cplusplus
__int32 BS_CALL	Initialize					(const char chLicenseKey[35], BS_BOOL bUseInteralAspiFirst = BS_TRUE);
#	else
__int32 BS_CALL	Initialize					(const char chLicenseKey[35], BS_BOOL bUseInteralAspiFirst);
#endif																			// null-terminated

__int32 BS_CALL	DeInitialize				(void);
#	ifdef __cplusplus
__int32 BS_CALL SetASPI						(BS_BOOL bUseInteralAspiFirst = BS_TRUE);
#	else
__int32 BS_CALL SetASPI						(BS_BOOL bUseInteralAspiFirst);
#	endif
//// end (initialization / deinitialization)


//// callbacks
__int32 BS_CALL	SetGetTextEventCallback     (GetTextEvent GetTextEventCallback, void *pUserData);
__int32 BS_CALL	SetInfoTextEventCallback	(InfoTextEvent InfoTextCallback, __int32 nLevel, void *pUserData);
InfoTextEvent BS_CALL GetInfoTextEventCallback(void **pUserData);
__int32 BS_CALL	SetProcessEventCallback		(ProcessEvent ProcessCallback, void *pUserData);
ProcessEvent BS_CALL	GetProcessEventCallback	(void **pUserData);
__int32 BS_CALL	SetEraseDoneEventCallback(EraseDoneEvent EraseDoneEventCallback, void *pUserData);
EraseDoneEvent BS_CALL	GetEraseDoneEventCallback(void **pUserData);
__int32 BS_CALL	SetFinalizeEventCallback	(FinalizeEvent FinalizeEventCallback, void *pUserData);
FinalizeEvent BS_CALL GetFinalizeEventCallback(void **pUserData);
__int32 BS_CALL	SetBurnDoneEventCallback	(BurnDoneEvent WriteDoneEventCallback, void *pUserData);
BurnDoneEvent BS_CALL GetBurnDoneEventCallback(void **pUserData);
__int32 BS_CALL	SetAddFileEventCallback		(AddFileEvent AddFileEventCallback, void *pUserData);
__int32 BS_CALL	SetRemoveFileEventCallback  (RemoveFileEvent RemoveFileEventCallback, void *pUserData);
__int32 BS_CALL	SetAudioDecoderEventCallback(AudioDecoderEvent AudioDecoderEventCallback, void *pUserData);
__int32 BS_CALL	SetAudioDecodeDoneEventCallback(AudioDecodeDoneEvent AudioDecodeDoneCallback, void *pUserData);
__int32 BS_CALL	SetVideoScannerEventCallback(VideoScannerEvent VideoScannerCallback, void *pUserData);
__int32 BS_CALL	SetVideoScanDoneEventCallback(VideoScanDoneEvent VideoScanDoneCallback, void *pUserData);
__int32 BS_CALL	ResetCallbacks				(void);
//// end (callbacks)


//// general settings
__int32 BS_CALL	SetTmpPath					(const char *pchPath);				// null-terminated
__int32 BS_CALL	GetTmpPath					(char chPath[_MAX_PATH]);			// null-terminated
#	ifdef __cplusplus
__int32 BS_CALL	SetLanguage					(const char *pchLanguage, const char *pchLanguageFile = BS_DEFAULT_LANGUAGE_FILE);
#	else
__int32 BS_CALL	SetLanguage					(const char *pchLanguage, const char *pchLanguageFile);
#	endif																		// null-terminated
__int32 BS_CALL	GetLanguage					(char chLanguage[100], char chLanguageFile[_MAX_PATH]);
																				// null-terminated
__int32 BS_CALL	GetText                     (__int32 nTextID, char *pchText, __int32 *pnLength);
																				// null-terminated
																				// valid pointer
//// end (general settings)


//// device / medium
#	ifdef __cplusplus
__int32 BS_CALL	GetDevices					(char chListDevices[26][50], __int32 *pnUsed, BS_BOOL bBurningDevicesOnly = BS_FALSE);
#	else
__int32 BS_CALL	GetDevices					(char chListDevices[26][50], __int32 *pnUsed, BS_BOOL bBurningDevicesOnly);
#	endif																		// each	null-terminated
__int32 BS_CALL	SetDevice					(const char cLetter);
__int32 BS_CALL	GetDevice					(char chLetter[2]);					// null-terminated
__int32 BS_CALL	GetDeviceInformation		(char chVendorID[9], char chProductID[17], char chProductRevision[5]);
																				// each	null-terminated
__int32 BS_CALL	GetDeviceCapabilities		(__int32 *pnDeviceCapabilities);	// valid pointer
__int32 BS_CALL	GetMaxReadSpeed				(__int32 *pnSpeed);					// valid pointer
#	ifdef __cplusplus
__int32 BS_CALL	SetReadSpeed				(__int32 nSpeed = BS_MAX_SPEED);
#	else
__int32 BS_CALL	SetReadSpeed				(__int32 nSpeed);
#	endif
__int32 BS_CALL	GetReadSpeed				(__int32 *pnSpeed);					// valid pointer
__int32 BS_CALL	GetMaxBurnSpeed				(__int32 *pnSpeed);					// valid pointer
__int32 BS_CALL ConvertSpeedFromKBPerSec    (__int32 nSpeedInKBPerSec, float *pfConvertedSpeed);   // valid pointer
//__int32 BS_CALL ConvertSpeedToKBPerSec      (float fConvertedSpeed, __int32 *pnSpeedInKBPerSec);   // valid pointer
#	ifdef __cplusplus
__int32 BS_CALL	SetBurnSpeed				(__int32 nSpeed = BS_MAX_SPEED);
#	else
__int32 BS_CALL	SetBurnSpeed				(__int32 nSpeed);
#	endif
__int32 BS_CALL GetPossibleBurnSpeeds       (struct SSpeed *pSpeeds, __int32 *pnSize);     // pnSize = valid pointer
__int32 BS_CALL GetPossibleReadSpeeds       (float *pfSpeeds, __int32 *pnSize);     // pnSize = valid pointer

__int32 BS_CALL	GetBurnSpeed				(__int32 *pnSpeed);					// valid pointer
__int32 BS_CALL	EjectDevice					(void);
__int32 BS_CALL	CloseDevice					(void);
__int32 BS_CALL	IsDeviceReady				(BS_LP_BOOL pbReady);				// valid pointer
#	ifdef __cplusplus
__int32 BS_CALL	Erase						(BS_BOOL bFast = BS_TRUE);
#	else
__int32 BS_CALL	Erase						(BS_BOOL bFast);
#	endif
__int32 BS_CALL GetMediumInformation		(struct SMediumInfo *pMediumInfo);	// valid pointer
__int32 BS_CALL	GetSessionInformation		(__int32 nSession, struct SSessionInfo *pSessionInfo);
																				// valid pointer
__int32 BS_CALL CloseSession				(void);
#	ifdef __cplusplus
__int32 BS_CALL	SaveDeviceSettingsToRegistry(const char *pchKey = BS_DEFAULT_REGISTRY_KEY, const char *pchRegistryPath = BS_DEFAULT_REGISTRY_PATH);
#	else
__int32 BS_CALL	SaveDeviceSettingsToRegistry(const char *pchKey, const char *pchRegistryPath);
#	endif																		// null-terminated
#	ifdef __cplusplus
__int32 BS_CALL	SetDeviceSettingsFromRegistry(const char *pchKey = BS_DEFAULT_REGISTRY_KEY, const char *pchRegistryPath = BS_DEFAULT_REGISTRY_PATH);
#	else
__int32 BS_CALL	SetDeviceSettingsFromRegistry(const char *pchKey, const char *pchRegistryPath);
#	endif																		// null-terminated
//// end (device / medium)


//// project
__int32 BS_CALL	CreateProject				(__int32 nType);
__int32 BS_CALL	DeleteProject				(void);
__int32 BS_CALL	GetProjectType				(__int32 *pnType);					// valid pointer
//// end (project)


////////////////////////////////////////////////////////////////////////////////
// Each of the following functions is only available for the current project. //
////////////////////////////////////////////////////////////////////////////////


//// file handling
__int32 BS_CALL	CreateDir					(struct SDirToCreate DirToCreate);
__int32 BS_CALL	AddDir						(struct SDirToAdd DirToAdd);
__int32 BS_CALL	RemoveDir					(struct SDirToRemove DirToRemove);
__int32 BS_CALL	AddFile						(struct SFileToAdd FileToAdd);
__int32 BS_CALL	RemoveFile					(struct SFileToRemove FileToRemove);
__int32 BS_CALL	ClearAll					(void);
__int32 BS_CALL GetPlayTime					(const char *strFileName,__int32 *time);
__int32 BS_CALL PlayAudioFile				(const char *strFileName);
__int32 BS_CALL AudioFileStop				(void);
//// end (file handling)


//// burning options
__int32 BS_CALL	SetOptions					(struct SOptions Options);
__int32 BS_CALL	GetOptions					(struct SOptions *pOptions);		// valid pointer
#	ifdef __cplusplus
__int32 BS_CALL	SaveOptionsToRegistry		(const char *pchKey = BS_DEFAULT_REGISTRY_KEY, const char *pchRegistryPath = BS_DEFAULT_REGISTRY_PATH);
#	else
__int32 BS_CALL	SaveOptionsToRegistry		(const char *pchKey, const char *pchRegistryPath);
#	endif																		// null-terminated
#	ifdef __cplusplus
__int32 BS_CALL	SetOptionsFromRegistry		(const char *pchKey = BS_DEFAULT_REGISTRY_KEY, const char *pchRegistryPath = BS_DEFAULT_REGISTRY_PATH);
#	else
__int32 BS_CALL	SetOptionsFromRegistry		(const char *pchKey, const char *pchRegistryPath);
#	endif																		// null-terminated
//// end (burning options)


//// burning
__int32 BS_CALL	PrepareISO                  (void);
__int32 BS_CALL	Prepare						(void);
__int32 BS_CALL	GetImageSize				(double *pdImageSize);				// valid pointer
__int32 BS_CALL	BuildISO					(const char *pchFilePath);			// null-terminated
__int32 BS_CALL	BurnISO						(const char *pchFilePath);			// null-terminated
__int32 BS_CALL	Burn						(void);
__int32 BS_CALL	Abort						(void);
//// end (burning)


//// GUI
#	ifdef BS_GUI
#		ifdef __cplusplus
__int32 BS_CALL	EraseDialog					(const char *pchTitle = NULL);		// null-terminated
#		else
__int32 BS_CALL	EraseDialog					(const char *pchTitle);				// null-terminated
#		endif
#		ifdef __cplusplus
__int32 BS_CALL	BurnDialog					(BS_BOOL bOpenSettings = BS_FALSE, BS_BOOL bViewSettings = BS_TRUE, const char *pchTitle = NULL, const char *pchIsoFilePath = NULL);
#		else
__int32 BS_CALL	BurnDialog					(BS_BOOL bOpenSettings, BS_BOOL bViewSettings, const char *pchTitle, const char *pchIsoFilePath);
#		endif																			// null-terminated
#endif	// BS_GUI
//// end (GUI)


#	ifdef __cplusplus
}
#	endif


#endif	// __BS_EXPORT_H__

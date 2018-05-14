#ifndef __BS_DEFINITIONS_H__
#	define __BS_DEFINITIONS_H__


#	ifdef __cplusplus
extern "C" {
#	endif


//// needed defines
#	ifndef _WINDOWS_
#		ifndef _MAX_PATH
#			define _MAX_PATH					260
#		endif
#		ifndef __int8
#			define __int8						char
#		endif
#		ifndef __int16
#			define __int16						short
#		endif
#		ifndef __int32
#			define __int32						long
#		endif
#		ifndef __int64
#			define __int64						long
#		endif
#		ifndef BYTE
typedef unsigned char							BYTE;
#		endif
#	endif
//// end (needed defines)


//// BOOL
#	define int2bool(nTmp)						(0 != nTmp)
#	define BOOL2bool(bTmp)						(FALSE != bTmp)
#	define BS_BOOL2bool(bTmp)					(BS_FALSE != bTmp)
#	define int2BS_BOOL(nTmp)					((0 != nTmp) ? BS_TRUE : BS_FALSE)

#	define BS_NDEF								-1							// -> BS_NDEF is interpreted as BS_TRUE.
#	define BS_BOOL								__int8
#	define BS_LP_BOOL							__int8*
#	define BS_FALSE								0
#	define BS_TRUE								1
//// end (BOOL)


//// default values
#	define BS_DEFAULT_LANGUAGE_FILE				"Burning.lng"
#	define BS_DEFAULT_REGISTRY_KEY				"HKEY_CURRENT_USER"
#	define BS_DEFAULT_REGISTRY_PATH				"\\Software\\Pixbyte\\Burning SDK\\"
//// end (default values)


//// callbacks
	//// info levels
#	define BS_IL_INFO							0
#	define BS_IL_LOW_DEBUG						1
#	define BS_IL_MEDIUM_DEBUG					2
#	define BS_IL_HIGH_DEBUG						3
	//// end (info levels)


	//// structs
		//// aspectratios
#		define BS_AR_SQUARE_PIXELS			    0
#		define BS_AR_4TO3_DISPLAY			    1
#		define BS_AR_16TO9_DISPLAY			    2
#		define BS_AR_221TO2_DISPLAY				3
#		define BS_AR_UNKNOWN				    4
		//// end (aspectratios)


	struct SVideoFormat
	{
		BS_BOOL bUseable;
		__int32 nWidth;
		__int32 nHeight;
		__int32 nBitRate;
		double fFPS;
		__int32 nAspectRatio;

#	ifdef __cplusplus
		SVideoFormat(void)
		{
			bUseable		= BS_NDEF;
			nWidth			= BS_NDEF;
			nHeight			= BS_NDEF;
			nBitRate		= BS_NDEF;
			fFPS			= BS_NDEF;
			nAspectRatio	= BS_NDEF;
		}
#	endif
	};

	struct SSpeed
	{
		float fSpeed;
		int   nSpeedInKBPerSec;
#   ifdef __cplusplus
		SSpeed(void)
		{
			fSpeed          = BS_NDEF;
			nSpeedInKBPerSec= BS_NDEF;
		}
#   endif
	};
	//// end (structs)


	//// definitions
	typedef __int32(*GetTextEvent)				(__int32 nTextID, char *pchText, __int32 *pnLength, void *pUserData);
	typedef void (*InfoTextEvent)				(const char *pcInfoText, __int32 nLevel, void *pUserData);
	typedef void (*ProcessEvent)				(float fPercent, float fDeviceBuffer, float fCache, double dBytesWritten, double dImageSize, void *pUserData);
	typedef void (*EraseDoneEvent)				(const char *pcError, void *pUserData);
	typedef void (*FinalizeEvent)				(void *pUserData);
	typedef void (*BurnDoneEvent)				(const char *pcError, void *pUserData);
	typedef void (*AddFileEvent)				(const char *pcFullPath, const char *pcLongName, const char *pcShortName, double dDateTime, double dFileSize, void *pUserData);
	typedef void (*RemoveFileEvent)				(const char *pcFullPath, const char *pcDestinationPath, const char *pcFile, void *pUserData);
	typedef void (*AudioDecoderEvent)			(float fPercent, const char *pcFileName, __int32 nType, void *pUserData);
	typedef void (*AudioDecodeDoneEvent)		(const char *pcFileName, const char *pcError, __int32 nErrorCode, void *pUserData);
	typedef void (*VideoScannerEvent)			(float fPercent, const char *pcFileName, void *pUserData);
	typedef void (*VideoScanDoneEvent)			(const char *pcFileName, const char *pcError, __int32 nErrorCode, struct SVideoFormat CurrentFormat, void *pUserData);
	//// end (definitions)
//// end (callbacks)


//// device / medium
	//// structs
		//// MediumStatus
#		define BS_MS_EmptyDisc					0
#		define BS_MS_IncompleteDisc				1
#		define BS_MS_CompleteDisc				2
#		define BS_MS_Other						3
		//// end (MediumStatus)


		//// LastSession
#		define BS_LS_EmptySession				0
#		define BS_LS_IncompleteSession			1
#		define BS_LS_DamagedSession				2
#		define BS_LS_CompleteSession			3
		//// end (LastSession)


		struct SMediumInfo
		{
			char chMediumType[25];
			__int32 nMediumStatus;
			double dMediumSize;
			double dMediumUsedSize;
			double dMediumFreeSize;
			__int32 nFirstSession;
			__int32 nLastSession;
			__int32 nLastSessionStatus;
			__int16 wMediumTypeCode;

#	ifdef __cplusplus
			SMediumInfo(void)
			{
				strcpy(chMediumType, "BS_NDEF");
				nMediumStatus		= BS_NDEF;
				dMediumSize			= BS_NDEF;
				dMediumUsedSize		= BS_NDEF;
				dMediumFreeSize		= BS_NDEF;
				nFirstSession		= BS_NDEF;
				nLastSession		= BS_NDEF;
				nLastSessionStatus	= BS_NDEF;
				wMediumTypeCode     = BS_NDEF;
			}
#	endif
		};


		struct SSessionInfo
		{
			double dSessionSize;
			BS_BOOL bLastSession;

#	ifdef __cplusplus
			SSessionInfo(void)
			{
				dSessionSize		= BS_NDEF;
				bLastSession		= BS_NDEF;
			}
#	endif
		};
	//// end (medium / session information)


	//// device capabilities
#	define BS_READ_CDR							1
#	define BS_READ_CDRW							2
#	define BS_READ_DVD							4
#	define BS_READ_DVDR							8
#	define BS_READ_DVDRW						16
#	define BS_READ_DVDRAM						32
#	define BS_READ_DVDR_PLUS					64
#	define BS_READ_DVDRW_PLUS					128
#	define BS_WRITE_CDR							256
#	define BS_WRITE_CDRW						512
#	define BS_WRITE_DVDR						1024
#	define BS_WRITE_DVDRW						2048
#	define BS_WRITE_DVDRAM						4096
#	define BS_WRITE_DVDR_PLUS					8192
#	define BS_WRITE_DVDRW_PLUS					16384
#	define BS_WRITE_TEST						32768
#	define BS_UNDERRUN_PROTECTION				65536
	//// end (device capabilities)

	//// mediumtype
#   define BS_UNKNOWN    0
#   define BS_CD_ROM     1
#   define BS_CD_R       2
#   define BS_CD_RW      3
#   define BS_DVD_ROM    4
#   define BS_DVD_R      5
#   define BS_DVD_RAM    6
#   define BS_DVD_RW_RO  7
#   define BS_DVD_RW     8
#   define BS_DVD_RW_SR  9
#   define BS_DVD_PLUSRW 10
#   define BS_DVD_PLUSR  11
#   define BS_DDCD_ROM   12
#   define BS_DDCD_R     13
#   define BS_DDCD_RW    14
# 	define BS_DVD_RDL	 15
	//// end (mediumtype)

	//// speed
#	define BS_MAX_SPEED							0
	//// end (speed)
//// end (device / medium)

//// Audio
	//// types
#	define BS_AUDIO_NO							0
#	define BS_AUDIO_PCM							1
#	define BS_AUDIO_MP3							2
#	define BS_AUDIO_OGG							3
	//// end (types)
//// end (project)

//// project
	//// types
#	define BS_BURNER_AUDIO						0
#	define BS_BURNER_CUE						1
#	define BS_BURNER_DATA						2
#	define BS_BURNER_VCD						3
#	define BS_BURNER_SVCD						4
#	define BS_BURNER_VIDEODVD					5
	//// end (types)
//// end (project)


//// file handling
	//// structs
		//// SavePath
		#define BS_DONT_SAVE_PATH			    0
		#define BS_WHOLE_PATH				    1
		#define BS_PARENTDIR_ONLY			    2
		//// end (SavePath)


		//// FileAttributes
		#define BS_FA_ReadOnly					0x00000001
		#define BS_FA_Hidden					0x00000002
		#define BS_FA_System					0x00000004
		#define BS_FA_Directory					0x00000010
		#define BS_FA_Archive					0x00000020
		#define BS_FA_All						0x0000003F
		//// end (FileAttributes)


		struct SDirToCreate
		{
			char chDir[_MAX_PATH];
			char chDestinationPath[_MAX_PATH];

#	ifdef __cplusplus
			SDirToCreate(void)
			{
				strcpy(chDir, "");
				strcpy(chDestinationPath, "\\");
			}
#	endif
		};

		struct SDirToAdd
		{
			char chSourceDirPath[_MAX_PATH];
			char chDestinationPath[_MAX_PATH];
			char chFileSpecification[10];
			__int32 nFileAttributes;
			BS_BOOL bRecursive;
			__int32 nSavePath;

#	ifdef __cplusplus
			SDirToAdd(void)
			{
				strcpy(chSourceDirPath, "");
				strcpy(chDestinationPath, "\\");
				strcpy(chFileSpecification, "*.*");
				nFileAttributes		= BS_FA_All;
				bRecursive			= BS_TRUE;
				nSavePath			= BS_PARENTDIR_ONLY;
			}
#	endif
		};

		struct SDirToRemove
		{
			char chDestinationPath[_MAX_PATH];

#	ifdef __cplusplus
			SDirToRemove(void)
			{
				strcpy(chDestinationPath, "");
			}
#	endif
		};

		struct SFileToAdd
		{
			char chSourceFilePath[_MAX_PATH];
			char chDestinationPath[_MAX_PATH];
			BS_BOOL bVideoFile;
			__int32 nSavePath;

#	ifdef __cplusplus
			SFileToAdd(void)
			{
				strcpy(chSourceFilePath, "");
				strcpy(chDestinationPath, "\\");
				bVideoFile			= BS_FALSE;
				nSavePath			= BS_DONT_SAVE_PATH;
			}
#	endif
		};

		struct SFileToRemove
		{
			char chFile[_MAX_PATH];
			char chDestinationPath[_MAX_PATH];

#	ifdef __cplusplus
			SFileToRemove(void)
			{
				strcpy(chFile, "");
				strcpy(chDestinationPath, "");
			}
#	endif
		};
	//// end (structs)
//// end (file handling)


//// burning options
	//// SessionToImport
#	define BS_LAST_SESSION						9999
	//// end (SessionToImport)


	//// structs
	struct SOptions
	{
		char chVolumeLabel[128];
		__int32 nSessionToImport;
		BS_BOOL bJolietFileSystem;
		BS_BOOL bBootable;
		char chBootImage[_MAX_PATH];
		BS_BOOL bFinalize;
		BS_BOOL bTestBurn;
		BS_BOOL bPerformOPC;
		BS_BOOL bVerifyAfterBurn;
		__int32 nCacheSize;
		BS_BOOL bUnderrunProtection;
		BS_BOOL bEjectAfterBurn;

		//// only available for GUI functions
		__int32 nCopies;
		//// Ende (only available for GUI functions)

#	ifdef __cplusplus
		SOptions(void)
		{
			Empty();
		}

		void Empty(void)
		{
			strcpy(chVolumeLabel, "BS_NDEF");
			nSessionToImport	= BS_NDEF;
			bJolietFileSystem	= BS_NDEF;
			bBootable			= BS_NDEF;
			strcpy(chBootImage, "BS_NDEF");
			bFinalize			= BS_NDEF;

			bTestBurn			= BS_NDEF;
			bPerformOPC			= BS_NDEF;
			bVerifyAfterBurn	= BS_NDEF;

			nCacheSize			= BS_NDEF;
			bUnderrunProtection	= BS_NDEF;
			bEjectAfterBurn		= BS_NDEF;

			//// only available for GUI functions
			nCopies				= BS_NDEF;
			//// end (only available for GUI functions)
		}
#	endif
	};
	//// end (structs)
//// end (burning options)


//// texts
	//// license key
	#define BS_SDK_KEY_INVALID              -1
	#define BS_SDK_KEY_VALID                0
	//// end (license key)


	//// error codes
	#define BS_SDK_ERROR_NO                 0
	#define BS_SDK_ERROR_NOT_ALLOWED        1
	#define BS_SCSI_ERROR_PARAM_10	        2
	#define BS_SDK_ERROR_BAD_REQUEST        3
	#define BS_SDK_ERROR_GENERAL			4
	#define BS_SCSI_ERROR_ASPI_01           5
	#define BS_SCSI_ERROR_ASPI_06		    6
	#define BS_SDK_ERROR_INVALID_PATH       7
	#define BS_SDK_ERROR_INVALID_SRC_PATH   8
	#define BS_SDK_ERROR_INVALID_DEST_PATH	9
	#define BS_SDK_ERROR_INVALID_FILE_NAME  10
	#define BS_SDK_ERROR_PATH_EXISTS        11
	#define BS_SDK_ERROR_FILE_EXISTS        12
	#define BS_SDK_CUE_ERROR_COMMAND_06     13
	#define BS_SDK_ERROR_INVALID_FILE_FORMAT 15
	#define BS_SDK_ERROR_FILE_OPEN          16
	#define BS_SDK_ERROR_CORRUPT_OR_INVALID_CUE_FILE 17
    #define BS_SDK_ERROR_BIN_FILE_NOT_FOUND 18
	#define BS_SDK_ERROR_NOT_IMPLEMENTED    19
	#define BS_SDK_ERROR_NOT_ALLOWED_FOR_THIS_BURNER 20
	#define BS_SCSI_ERROR_DISC_29           21
	#define BS_SCSI_ERROR_DISC_30           22
	#define BS_SCSI_ERROR_DISC_31           23
	#define BS_SCSI_ERROR_DISC_32           24
	#define BS_SDK_ERROR_UNKNOWN_TEXTID     25
	#define BS_SDK_ERROR_MORE_SPACE_NEEDED  26

	#define BS_SDK_ERROR_UNKNOWN            101
	#define BS_SCSI_ERROR_ASPI_02           102
	#define BS_SCSI_ERROR_ASPI_03           103
	#define BS_SDK_ERROR_INVALIDSRB         104
	#define BS_SDK_ERROR_BUFFERALIGNMENT    105
	#define BS_SCSI_ERROR_ASPI_04           106
	#define BS_SDK_ERROR_BUFFERTOOBIG       107
	#define BS_SDK_ERROR_TIMEOUT            108
	#define BS_SDK_ERROR_SRBTIMEOUT         109
	#define BS_SDK_ERROR_MESSAGEREJECT      110
	#define BS_SCSI_ERROR_ASPI_05           111
	#define BS_SDK_ERROR_PARITYERR          112
	#define BS_SCSI_ERROR_107               113
	#define BS_SDK_ERROR_SELECTIONTIMEOUT   114
	#define BS_SDK_ERROR_DATAOVERRUN        115
	#define BS_SDK_ERROR_UNEXPECTEDBUSFREE  116
	#define BS_SDK_ERROR_CHECKCONDITION     117
	#define BS_SCSI_ERROR_ASPI_08           118
	#define BS_SCSI_ERROR_ASPI_09           119
	#define BS_SCSI_ERROR_ASPI_10           120
	#define BS_SDK_ERROR_RECOVEREDERROR     121
	#define BS_SDK_ERROR_NOTREADY           122
	#define BS_SCSI_ERROR_DISC_33           123
	#define BS_SDK_ERROR_HARDWAREERROR      124
	#define BS_SDK_ERROR_UNITATTENTION      126
	#define BS_SDK_ERROR_DATAPROTECT        127
	#define BS_SDK_ERROR_ERASECHECK         128
	#define BS_SDK_ERROR_COPYABORTED        129
	#define BS_SDK_ERROR_ABORTEDCOMMAND     130
	#define BS_SDK_ERROR_VOLUMEOVERFLOW     131
	#define BS_SDK_ERROR_MISCOMPARE         132
	#define BS_SDK_ERROR_RESERVED           133
	#define BS_SDK_ERROR_FILEMARK           134
	#define BS_SDK_ERROR_ILLEGALLENGTH      136
	#define BS_SDK_ERROR_INCORRECTLENGTH    137
	#define BS_SDK_ERROR_ABORTED            138
	#define BS_SDK_ERROR_FILEINUSE          139
	#define BS_SDK_ERROR_CREATEFILE         140
	#define BS_SCSI_ERROR_ASPI_07           141
	#define BS_SCSI_ERROR_WRITE_12          142
	#define BS_SDK_ERROR_NOTSUPPORTED       143
	#define BS_SDK_ERROR_NEXTADRESS         144
    #define BS_SDK_ERROR_TOO_MUCH_DATA      145
	#define BS_SDK_ERROR_MAXDIRS            146
	#define BS_SDK_ERROR_MAXFILES           147
	#define BS_SDK_ERROR_ERRINVALIDFILENAME 148
	#define BS_SDK_ERROR_IMPORTSESSION      149
	#define BS_SDK_ERROR_ISOIMAGENOTFOUND   150
	#define BS_SDK_INT_ERROR_1              151
	#define BS_SDK_INT_ERROR_2              152
	#define BS_SDK_INT_ERROR_3              153
	#define BS_SDK_INT_ERROR_4              154
	#define BS_SDK_INT_ERROR_5              155
    #define BS_SDK_CUE_ERROR_SENDING_CUE    156
	#define BS_SDK_CUE_ERROR_UEOL			157
	#define BS_SDK_CUE_ERROR_FIELD			158
	#define BS_SDK_CUE_ERROR_FILE			159
	#define BS_SDK_CUE_ERROR_COMMAND_01     160
    #define BS_SDK_INT_ERROR_FORMAT         161

	#define BS_SCSI_ERROR_01                200
	#define BS_SCSI_ERROR_02                201
	#define BS_SCSI_ERROR_AUDIO_01          202
	#define BS_SCSI_ERROR_AUDIO_02          203
	#define BS_SCSI_ERROR_AUDIO_03          204
	#define BS_SCSI_ERROR_AUDIO_04          205
	#define BS_SCSI_ERROR_AUDIO_05          206
	#define BS_SCSI_ERROR_001               207
	#define BS_SCSI_ERROR_002               208
	#define BS_SCSI_ERROR_UNIT_01           209
	#define BS_SCSI_ERROR_UNIT_02           210
	#define BS_SCSI_ERROR_UNIT_03           211
	#define BS_SCSI_ERROR_UNIT_04           212
	#define BS_SCSI_ERROR_UNIT_05           213
	#define BS_SCSI_ERROR_UNIT_06           214
	#define BS_SCSI_ERROR_UNIT_07           215
	#define BS_SCSI_ERROR_UNIT_08           216
	#define BS_SCSI_ERROR_UNIT_09           217
	#define BS_SCSI_ERROR_UNIT_10           218
	#define BS_SCSI_ERROR_004 				219
	#define BS_SCSI_ERROR_005 				220
	#define BS_SCSI_ERROR_UNIT_11           221
	#define BS_SCSI_ERROR_UNIT_12           222
	#define BS_SCSI_ERROR_UNIT_13           223
	#define BS_SCSI_ERROR_UNIT_14           224
	#define BS_SCSI_ERROR_006 				225
	#define BS_SCSI_ERROR_007 				226
	#define BS_SCSI_ERROR_008 				227
	#define BS_SCSI_ERROR_009 				228
	#define BS_SCSI_ERROR_010 				229
	#define BS_SCSI_ERROR_011 				230
	#define BS_SCSI_ERROR_LOG_01            231
	#define BS_SCSI_ERROR_ATT_01            232
	#define BS_SCSI_ERROR_ATT_02            233
	#define BS_SCSI_ERROR_ATT_03            234
	#define BS_SCSI_ERROR_WRITE_01          235
	#define BS_SCSI_ERROR_WRITE_02          236
	#define BS_SCSI_ERROR_WRITE_03          237
	#define BS_SCSI_ERROR_WRITE_04          238
	#define BS_SCSI_ERROR_WRITE_05          239
	#define BS_SCSI_ERROR_EXT_01            240
	#define BS_SCSI_ERROR_EXT_02            241
	#define BS_SCSI_ERROR_TARGET_01         242
	#define BS_SCSI_ERROR_TARGET_02         243
	#define BS_SCSI_ERROR_TARGET_03         244
	#define BS_SCSI_ERROR_TARGET_04         245
	#define BS_SCSI_ERROR_READ_01           246
	#define BS_SCSI_ERROR_READ_02           247
	#define BS_SCSI_ERROR_012 				248
	#define BS_SCSI_ERROR_013 				249
	#define BS_SCSI_ERROR_CIRC_01           250
	#define BS_SCSI_ERROR_CRC_01            251
	#define BS_SCSI_ERROR_DECOM_01          252
	#define BS_SCSI_ERROR_DRIVE_01          253
	#define BS_SCSI_ERROR_DRIVE_02          254
	#define BS_SCSI_ERROR_014 				255
	#define BS_SCSI_ERROR_015 				256
	#define BS_SCSI_ERROR_016 				257
	#define BS_SCSI_ERROR_017 				258
	#define BS_SCSI_ERROR_MECH_01           259
	#define BS_SCSI_ERROR_MECH_02           260
	#define BS_SCSI_ERROR_RECOVER_01        261
	#define BS_SCSI_ERROR_RECOVER_02        262
	#define BS_SCSI_ERROR_RECOVER_03        263
	#define BS_SCSI_ERROR_RECOVER_04        264
	#define BS_SCSI_ERROR_RECOVER_05        265
	#define BS_SCSI_ERROR_RECOVER_06        266
	#define BS_SCSI_ERROR_RECOVER_07        267
	#define BS_SCSI_ERROR_RECOVER_08        268
	#define BS_SCSI_ERROR_RECOVER_09        269
	#define BS_SCSI_ERROR_RECOVER_10        270
	#define BS_SCSI_ERROR_RECOVER_11        271
	#define BS_SCSI_ERROR_RECOVER_12        272
	#define BS_SCSI_ERROR_RECOVER_13        273
	#define BS_SCSI_ERROR_RECOVER_14        274
	#define BS_SCSI_ERROR_RECOVER_15        275
	#define BS_SCSI_ERROR_RECOVER_16        276
	#define BS_SCSI_ERROR_RECOVER_17        277
	#define BS_SCSI_ERROR_018 				278
	#define BS_SCSI_ERROR_019 				279
	#define BS_SCSI_ERROR_020 				280
	#define BS_SCSI_ERROR_021 				281
	#define BS_SCSI_ERROR_022 				282
	#define BS_SCSI_ERROR_023 				283
	#define BS_SCSI_ERROR_024 				284
	#define BS_SCSI_ERROR_CDB_01            285
	#define BS_SCSI_ERROR_CDB_02            286
	#define BS_SCSI_ERROR_UNIT_16           287
	#define BS_SCSI_ERROR_PARAM_01          288
	#define BS_SCSI_ERROR_PARAM_02          289
	#define BS_SCSI_ERROR_PARAM_03          290
	#define BS_SCSI_ERROR_PARAM_04          291
	#define BS_SCSI_ERROR_025 				292
	#define BS_SCSI_ERROR_026 				293
	#define BS_SCSI_ERROR_SEGM_01           296
	#define BS_SCSI_ERROR_SEGM_02           297
	#define BS_SCSI_ERROR_SEGM_03           298
	#define BS_SCSI_ERROR_027 				299
	#define BS_SCSI_ERROR_028 				300
	#define BS_SCSI_ERROR_SEGM_04           301
	#define BS_SCSI_ERROR_WRITE_06          302
	#define BS_SCSI_ERROR_WRITE_07          303
	#define BS_SCSI_ERROR_WRITE_08          305
	#define BS_SCSI_ERROR_WRITE_09          306
	#define BS_SCSI_ERROR_WRITE_10          307
	#define BS_SCSI_ERROR_WRITE_11          308
	#define BS_SCSI_ERROR_029 				309
	#define BS_SCSI_ERROR_030 				310
	#define BS_SCSI_ERROR_031 				311
	#define BS_SCSI_ERROR_032 				312
	#define BS_SCSI_ERROR_033 				313
	#define BS_SCSI_ERROR_034 				314
	#define BS_SCSI_ERROR_035 				315
	#define BS_SCSI_ERROR_036 				316
	#define BS_SCSI_ERROR_037 				317
	#define BS_SCSI_ERROR_PARAM_05          318
	#define BS_SCSI_ERROR_PARAM_06          319
	#define BS_SCSI_ERROR_PARAM_07          320
	#define BS_SCSI_ERROR_038 				321
	#define BS_SCSI_ERROR_039 				322
	#define BS_SCSI_ERROR_040 				323
	#define BS_SCSI_ERROR_041 				324
	#define BS_SCSI_ERROR_COMMAND_02        325
	#define BS_SCSI_ERROR_042 				326
	#define BS_SCSI_ERROR_043 				327
	#define BS_SCSI_ERROR_044 				328
	#define BS_SCSI_ERROR_045 				329
	#define BS_SCSI_ERROR_COMMAND_03        330
	#define BS_SCSI_ERROR_DISC_01           331
	#define BS_SCSI_ERROR_DISC_02           332
	#define BS_SCSI_ERROR_DISC_03           333
	#define BS_SCSI_ERROR_DISC_04           334
	#define BS_SCSI_ERROR_DISC_05           335
	#define BS_SCSI_ERROR_DISC_06           336
	#define BS_SCSI_ERROR_DISC_07           337
	#define BS_SCSI_ERROR_DISC_08           338
	#define BS_SCSI_ERROR_DISC_09           339
	#define BS_SCSI_ERROR_DISC_10           340
	#define BS_SCSI_ERROR_DISC_11           341
	#define BS_SCSI_ERROR_DISC_12           342
	#define BS_SCSI_ERROR_DISC_13           343
	#define BS_SCSI_ERROR_046               344
	#define BS_SCSI_ERROR_047               345
	#define BS_SCSI_ERROR_048 				346
	#define BS_SCSI_ERROR_049 				347
	#define BS_SCSI_ERROR_050 				348
	#define BS_SCSI_ERROR_051 				349
	#define BS_SCSI_ERROR_052 				350
	#define BS_SCSI_ERROR_PARAM_08          351
	#define BS_SCSI_ERROR_PARAM_09          352
	#define BS_SCSI_ERROR_DISC_14           353
	#define BS_SCSI_ERROR_DISC_15           354
	#define BS_SCSI_ERROR_DISC_16           355
	#define BS_SCSI_ERROR_DISC_17           356
	#define BS_SCSI_ERROR_DISC_18           357
	#define BS_SCSI_ERROR_DISC_19           358
	#define BS_SCSI_ERROR_DISC_20           359
	#define BS_SCSI_ERROR_DISC_21           360
	#define BS_SCSI_ERROR_DISC_22           361
	#define BS_SCSI_ERROR_DISC_23           362
	#define BS_SCSI_ERROR_DISC_24           363
	#define BS_SCSI_ERROR_DISC_25           364
	#define BS_SCSI_ERROR_DISC_26           365
	#define BS_SCSI_ERROR_MECH_03           366
	#define BS_SCSI_ERROR_053 				367
	#define BS_SCSI_ERROR_UNIT_17           368
	#define BS_SCSI_ERROR_UNIT_18           369
	#define BS_SCSI_ERROR_UNIT_19           370
	#define BS_SCSI_ERROR_UNIT_20           371
	#define BS_SCSI_ERROR_UNIT_21           372
	#define BS_SCSI_ERROR_054 				373
	#define BS_SCSI_ERROR_055 				374
	#define BS_SCSI_ERROR_056 				375
    #define BS_SCSI_ERROR_057 				376
	#define BS_SCSI_ERROR_058 				377
	#define BS_SCSI_ERROR_059 				378
	#define BS_SCSI_ERROR_060 				379
	#define BS_SCSI_ERROR_061 				380
	#define BS_SCSI_ERROR_062 				381
	#define BS_SCSI_ERROR_063 				382
	#define BS_SCSI_ERROR_VOL_01            383
	#define BS_SCSI_ERROR_VOL_02            384
	#define BS_SCSI_ERROR_VOL_03            385
	#define BS_SCSI_ERROR_VOL_04            386
	#define BS_SCSI_ERROR_064 				387
	#define BS_SCSI_ERROR_065 				388
	#define BS_SCSI_ERROR_DISC_27           389
	#define BS_SCSI_ERROR_DISC_28           390
	#define BS_SCSI_ERROR_066 				391
	#define BS_SCSI_ERROR_067 				392
	#define BS_SCSI_ERROR_068 				393
	#define BS_SCSI_ERROR_069 				394
	#define BS_SCSI_ERROR_070 				395
	#define BS_SCSI_ERROR_071 				396
	#define BS_SCSI_ERROR_072 				397
	#define BS_SCSI_ERROR_073 				398
	#define BS_SCSI_ERROR_074 				399
	#define BS_SCSI_ERROR_075 				400
	#define BS_SCSI_ERROR_076 				401
	#define BS_SCSI_ERROR_COMMAND_04        402
	#define BS_SCSI_ERROR_077 				403
	#define BS_SCSI_ERROR_UNIT_22           404
	#define BS_SCSI_ERROR_COMMAND_05        405
	#define BS_SCSI_ERROR_078 				410
	#define BS_SCSI_ERROR_079 				411
	#define BS_SCSI_ERROR_080 				412
	#define BS_SCSI_ERROR_081 				413
	#define BS_SCSI_ERROR_082 				414
	#define BS_SCSI_ERROR_083 				415
	#define BS_SCSI_ERROR_084 				416
	#define BS_SCSI_ERROR_085 				417
	#define BS_SCSI_ERROR_LOG_02            418
	#define BS_SCSI_ERROR_086 				419
	#define BS_SCSI_ERROR_LOG_03            420
	#define BS_SCSI_ERROR_LOG_04            421
	#define BS_SCSI_ERROR_087 				422
	#define BS_SCSI_ERROR_UNIT_23           424
	#define BS_SCSI_ERROR_088 				425

	#define BS_SCSI_ERROR_089 				426
	#define BS_SCSI_ERROR_090 				427
	#define BS_SCSI_ERROR_091 				428
	#define BS_SCSI_ERROR_092 				429
	#define BS_SCSI_ERROR_093 				430
	#define BS_SCSI_ERROR_094 				431
	#define BS_SCSI_ERROR_095 				432
	#define BS_SCSI_ERROR_096 				433
	#define BS_SCSI_ERROR_097 				434
	#define BS_SCSI_ERROR_098 				435
	#define BS_SCSI_ERROR_099 				436
	#define BS_SCSI_ERROR_DCSS_01           437
	#define BS_SCSI_ERROR_DCSS_02           438
	#define BS_SCSI_ERROR_DCSS_03           439
	#define BS_SCSI_ERROR_DCSS_04           440
	#define BS_SCSI_ERROR_DCSS_05           441
	#define BS_SCSI_ERROR_DCSS_06           442
	#define BS_SCSI_ERROR_SESSION_01        443
	#define BS_SCSI_ERROR_SESSION_02        444
	#define BS_SCSI_ERROR_SESSION_03        445
	#define BS_SCSI_ERROR_SESSION_04        446
	#define BS_SCSI_ERROR_100               447
	#define BS_SCSI_ERROR_101 				448
	#define BS_SCSI_ERROR_102 				449
	#define BS_SCSI_ERROR_103 				450
	#define BS_SCSI_ERROR_104 				451
	#define BS_SCSI_ERROR_105               452
	#define BS_SCSI_ERROR_ALLOC_01          453
	#define BS_SCSI_ERROR_ALLOC_02 			454
	#define BS_SCSI_ERROR_106 				455
	//// error codes


	//// info codes
	#define BS_SDK_MESSAGE_WAIT             500
	#define BS_SDK_MESSAGE_WRITESTART       501
    #define BS_SDK_MESSAGE_EREASESTART      502
    #define BS_SDK_MESSAGE_EXTR_FILE        503
	#define BS_SDK_MESSAGE_SIMULATE         504
	#define BS_SDK_MESSAGE_IMPORT           505
    #define BS_SDK_MESSAGE_FORMAT           506
	//// end (info codes)


	//// text codes
	#define BS_SDK_MESSAGE_01               600
	#define BS_SDK_MESSAGE_02               601
	#define BS_SDK_MESSAGE_03               602
	#define BS_SDK_MESSAGE_04				603
	#define BS_SDK_MESSAGE_05               604
	#define BS_SDK_MESSAGE_06               605
	#define BS_SDK_MESSAGE_07               606
	#define BS_SDK_MESSAGE_08               607
	#define BS_SDK_MESSAGE_10               608
	#define BS_SDK_MESSAGE_11               609
	#define BS_SDK_MESSAGE_12               610
	#define BS_SDK_MESSAGE_13               611
	#define BS_GUI_RESOURCE_01              612
	#define BS_GUI_RESOURCE_02              613
	#define BS_GUI_RESOURCE_03              614
	#define BS_GUI_RESOURCE_04              615
	#define BS_GUI_RESOURCE_05              616
	#define BS_GUI_RESOURCE_06              617
	#define BS_GUI_RESOURCE_07              618
	#define BS_GUI_RESOURCE_08              619
	#define BS_GUI_RESOURCE_09              620
	#define BS_GUI_RESOURCE_10              621
	#define BS_GUI_RESOURCE_11              622
	#define BS_GUI_RESOURCE_12              623
	#define BS_GUI_RESOURCE_13              624
	#define BS_GUI_RESOURCE_14              625
	#define BS_GUI_RESOURCE_15              626
	#define BS_GUI_RESOURCE_16              627
	#define BS_GUI_RESOURCE_17              628
	#define BS_GUI_RESOURCE_18              629
	#define BS_GUI_RESOURCE_19              630
	#define BS_GUI_RESOURCE_20              631
	#define BS_GUI_RESOURCE_21              632
	#define BS_GUI_RESOURCE_22              633
	#define BS_GUI_RESOURCE_23              634
	#define BS_GUI_RESOURCE_24              635
	#define BS_GUI_RESOURCE_25              636
	#define BS_GUI_RESOURCE_26              637
	#define BS_GUI_RESOURCE_27              638
	#define BS_GUI_RESOURCE_28              639
	//// end (text codes)
//// end (texts)


#	ifdef __cplusplus
}
#	endif


#endif	// __BS_DEFINITIONS_H__

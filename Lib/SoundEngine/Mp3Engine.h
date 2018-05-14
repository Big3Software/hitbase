// Mp3Engine.h: interface for the CMp3Engine class.
//
//////////////////////////////////////////////////////////////////////

#pragma once

#include "SoundEngineItem.h"

#include "fmod.hpp"
class CMp3Player;

struct tagMP3Header
{
	unsigned int iFrameSync : 11;
	unsigned int iMPEGAudioVersionID : 2;
	unsigned int iLayerDescription : 2;
	unsigned int iProtectedbyCRC : 1;
	unsigned int iBitrateIndex : 4;
	unsigned int iSamplingrateIndex : 2;
	unsigned int iPadding : 1;
	unsigned int iPrivate : 1;
	unsigned int iChannelMode : 2;
	unsigned int iModeExtension : 2;
	unsigned int iCopyright : 1;
	unsigned int iOriginal : 1;
	unsigned int iEmphasis : 2;
};

class SOUNDENGINE_INTERFACE CMp3Engine : public CSoundEngineItem
{
	friend CMp3Player;
public:
	CMp3Engine();
	virtual ~CMp3Engine();

public:
	BOOL IsPlaying();
	BOOL IsPlayCompleted();
	static CMp3Engine* CreateFromFile(const CString& sFilename);
	BOOL Open();
	BOOL Close();
	BOOL Play(DWORD dwMSStart, DWORD dwMSEnd);
	BOOL Pause(BOOL bPause = TRUE);
	BOOL Stop();
	BOOL Seek(DWORD dwPosition);
	DWORD GetPlayPositionMS();

	BOOL SetSpeed(double dblSpeed);
	BOOL SetVolume(double dblVolume);
	void SetAutoVolumeAdjust(BOOL bAutoVolumeAdjust);

	void SetPlayer(CMp3Player* pPlayer);

	BOOL SaveToFile();

	static BOOL WriteMP3Information(const CString& sFilename, const CSoundEngineItemData& MP3Info);
	static CString MapID3GenreToHitbaseGenre(byte id3Genre);

protected:
    CMp3Player* m_pPlayer;
	
	tagMP3Header m_MP3Header;

	BOOL GetMP3Information();
	BOOL GetMP3Header();
	//void ReadID3Tag(ID3_Tag& myTag1, ID3_Tag& myTag2, ID3_FrameID ID3Field, CString& sValue);
	void GetTags(FMOD::Sound* stream);
	CString GetTag(FMOD::Sound* sound, const CString& tagV1, const CString& tagV2);
};


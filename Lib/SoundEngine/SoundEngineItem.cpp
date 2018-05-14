// SoundEngine1.cpp: implementation of the CSoundEngineItem class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "SoundEngineIntern.h"
#include "SoundEngineItem.h"
#include "MP3Engine.h"
#include "WMAEngine.h"
#include "WaveEngine.h"
#include "../hitmisc/HitbaseWinAppBase.h"
#include <io.h>

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

CSoundEngineItem::CSoundEngineItem(CSoundEngine* pSoundEngine)
{
	m_pSoundEngine = pSoundEngine;

    // Wir fangen mit maximaler Lautstärke an (standardmäßig 200%) und passen uns dann kontinuierlich an
	m_dblAutoVolumeAdjustValue = (double)((CHitbaseWinAppBase*)AfxGetApp())->m_config.m_lAutoVolumeAdjustMax / 100.0;
	m_bAutoVolumeAdjust = ((CHitbaseWinAppBase*)AfxGetApp())->m_config.m_bAutoVolumeAdjust;
}

CSoundEngineItem::~CSoundEngineItem()
{
}

// Statische Erzeung eines Elements
CSoundEngineItem* CSoundEngineItem::CreateFromFile(const CString& fileName)
{
	if (fileName.GetLength() < 1)		// leerer String direkt abbrechen
		return NULL;

	CString sFilename = fileName;
	CSoundEngineItem* pSoundEngine;
	CString sExt = CMisc::GetFileNameExtension(sFilename);

	if (!sExt.CompareNoCase(L".lnk"))
	{
		CMisc::ResolveShortcut(sFilename, sFilename);
	}

	if (_waccess(sFilename, 0))		// Wenn die Datei nicht vorhanden ist, dann direkt abbrechen
		return NULL;

//	if (sExt.CompareNoCase(".wma") && 
//		sExt.CompareNoCase(".asf"))
	{
		pSoundEngine = CMp3Engine::CreateFromFile(sFilename);

		if (pSoundEngine)
			return pSoundEngine;
	}

	pSoundEngine = CWaveEngine::CreateFromFile(sFilename);

	if (pSoundEngine)
		return pSoundEngine;

	pSoundEngine = CWMAEngine::CreateFromFile(sFilename);

	if (pSoundEngine)
		return pSoundEngine;

	return NULL;
}

void CSoundEngineItem::SetAutoVolumeAdjustValue(double dblAutoVolumeAdjustValue) 
{ 
	m_dblAutoVolumeAdjustValue = dblAutoVolumeAdjustValue; 
}

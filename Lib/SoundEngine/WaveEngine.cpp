// WaveEngine.cpp: implementation of the CWaveEngine class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "SoundEngineIntern.h"
#include "WaveEngine.h"
#include "WavePlay.h"

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

CWaveEngine::CWaveEngine()
{
	m_pWavePlay = NULL;
}

CWaveEngine::~CWaveEngine()
{
	delete m_pWavePlay;
}

CWaveEngine* CWaveEngine::CreateFromFile(const CString &sFilename)
{
	CWaveEngine* pWaveEngine = new CWaveEngine;

	pWaveEngine->m_sFilename = sFilename;

	if (pWaveEngine->Open())
		return pWaveEngine;

	delete pWaveEngine;

	return NULL;
}

BOOL CWaveEngine::Open()
{
	Close();

	m_pWavePlay = new CWavePlay;

	if (!m_pWavePlay->Open(m_sFilename))
		return FALSE;

	return TRUE;
}

BOOL CWaveEngine::Close()
{
	return TRUE;
}

BOOL CWaveEngine::Play(DWORD dwMSStart, DWORD dwMSEnd)
{
	m_pWavePlay->Play(dwMSStart, dwMSEnd);

	return TRUE;
}

BOOL CWaveEngine::Stop()
{
	m_pWavePlay->Stop();

	return TRUE;
}

BOOL CWaveEngine::Pause(BOOL bPause /* = TRUE */)
{
	m_pWavePlay->Pause(bPause);

	return TRUE;
}

BOOL CWaveEngine::Seek(DWORD dwPosition)
{ 
	return TRUE;
}

int CWaveEngine::GetTotalTime()
{
	return m_pWavePlay->GetDuration();
}

CString CWaveEngine::GetArtist()
{
	return L"";
}

CString CWaveEngine::GetTitle()
{
	return L"";
}

CString CWaveEngine::GetComment()
{
	return L"";
}

int CWaveEngine::GetChannels()
{
	return 0;       // TODO!!!!!!!!!
}

int CWaveEngine::GetBitRate()
{
	return 0;       // TODO!!!!!!!!!
}

int CWaveEngine::GetSampleRate()
{
	return 0;       // TODO!!!!!!!!!
}

DWORD CWaveEngine::GetPlayPositionMS()
{
	DWORD dwPos = m_pWavePlay->GetPlayPosition();

	return dwPos;
}

BOOL CWaveEngine::IsPlaying()
{
	return m_pWavePlay->IsPlaying();
}

BOOL CWaveEngine::IsPlayCompleted()
{
	return !m_pWavePlay->IsPlaying();
}


BOOL CWaveEngine::SetSpeed(double dblSpeed)
{
	return TRUE;
}

BOOL CWaveEngine::SetVolume(double dblVolume)
{
	return TRUE;
}

BOOL CWaveEngine::SaveToFile()
{
	return TRUE;
}


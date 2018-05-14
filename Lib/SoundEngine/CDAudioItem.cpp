// CDAudioItem.cpp: implementation of the CCDAudioEngine class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "CDAudioItem.h"

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

CCDAudioItem::CCDAudioItem()
{
}

CCDAudioItem::~CCDAudioItem()
{
}

BOOL CCDAudioItem::IsPlayCompleted()
{
	return FALSE;
}

BOOL CCDAudioItem::Open()
{
	return FALSE;
}

BOOL CCDAudioItem::Close()
{
	return FALSE;
}

BOOL CCDAudioItem::Play(DWORD dwMSStart, DWORD dwMSEnd)
{
	return FALSE;
}

BOOL CCDAudioItem::Pause(BOOL bPause)
{
	return FALSE;
}

BOOL CCDAudioItem::Stop()
{
	return FALSE;
}

BOOL CCDAudioItem::Seek(DWORD dwPosition)
{
	return FALSE;
}

int CCDAudioItem::GetTotalTime()
{
	return FALSE;
}

DWORD CCDAudioItem::GetPlayPositionMS()
{
	return FALSE;
}

BOOL CCDAudioItem::IsPlaying()
{
	return FALSE;
}

BOOL CCDAudioItem::SetSpeed(double dblSpeed)
{
	return TRUE;
}

BOOL CCDAudioItem::SetVolume(double dblVolume)
{
	return TRUE;
}


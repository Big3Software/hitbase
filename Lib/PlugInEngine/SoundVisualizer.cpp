// SoundVisualizer.cpp: implementation of the CSoundVisualizer class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "SoundVisualizer.h"
#include "SoundVisualizerWnd.h"
#include "../hitmisc/hitbaseWinAppBase.h"
#include "PlugInManager.h"
#include <math.h>

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

CSoundVisualizer::CSoundVisualizer()
{
}

CSoundVisualizer::~CSoundVisualizer()
{

}

BOOL CSoundVisualizer::IsActive(int nIndex)
{
	return ((CHitbaseWinAppBase*)AfxGetApp())->m_pPlugInManager->IsActive(nIndex); 
}

void CSoundVisualizer::SetActive(int nPlugInIndex, BOOL bActive)
{
    if (bActive)
	{
		CSoundVisualizerWnd* pSoundVisualizerWnd;

		pSoundVisualizerWnd = (CSoundVisualizerWnd*) CSoundVisualizerWnd::CreateObject();
		pSoundVisualizerWnd->m_pSoundVisualizer = this;
		pSoundVisualizerWnd->m_nPlugInIndex = nPlugInIndex;
		
		HPI_INFO* phpiInfo = ((CHitbaseWinAppBase*)AfxGetApp())->m_pPlugInManager->GetPlugInInformation(nPlugInIndex);

		// JUS 990313
		DWORD dwStyle;

		if (phpiInfo->nType & HPIT_FULLSCREEN)
			dwStyle = WS_VISIBLE;
		else
			dwStyle = WS_POPUP|WS_CAPTION|WS_SYSMENU|WS_VISIBLE|MFS_SYNCACTIVE|MFS_THICKFRAME;
		// JUS_

		pSoundVisualizerWnd->Create(NULL, 
			                        CString(phpiInfo->szFullName), 
									dwStyle, 
			                        ((CHitbaseWinAppBase*)AfxGetApp())->m_pPlugInManager->m_hpiList[nPlugInIndex].m_WindowRect, 
									AfxGetMainWnd());
// Ist jetzt im Hauptprogramm!		((CHitbaseApp*)AfxGetApp())->m_SignalProcessor->Start();

	    ((CHitbaseWinAppBase*)AfxGetApp())->m_pPlugInManager->SetActive(nPlugInIndex, pSoundVisualizerWnd, TRUE);
	}
	else
	{
	    ((CHitbaseWinAppBase*)AfxGetApp())->m_pPlugInManager->SetActive(nPlugInIndex, NULL, FALSE);

// Ist jetzt im Hauptprogramm!		((CHitbaseApp*)AfxGetApp())->m_SignalProcessor->End();
	}
}

BOOL CSoundVisualizer::AccessWaveData(const char* pWaveData, DWORD dwCount)
{
	// Jetzt an alle aktiven Plug-Ins die Daten schicken!
	return ((CHitbaseWinAppBase*)AfxGetApp())->m_pPlugInManager->DisplaySoundData(pWaveData, dwCount);
}

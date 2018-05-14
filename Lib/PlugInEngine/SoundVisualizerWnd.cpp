// SoundVisualizerWnd.cpp : implementation file
//

#include "stdafx.h"
#include "../hitmisc/HitbaseWinAppBase.h"
#include "../../app/hitbase/resource.h"
#include "PlugInManager.h"
#include "SoundVisualizerWnd.h"
#include "SoundVisualizer.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CSoundVisualizerWnd

IMPLEMENT_DYNCREATE(CSoundVisualizerWnd, CMiniFrameWnd)

CSoundVisualizerWnd::CSoundVisualizerWnd()
{
}

CSoundVisualizerWnd::~CSoundVisualizerWnd()
{
}


BEGIN_MESSAGE_MAP(CSoundVisualizerWnd, CMiniFrameWnd)
	//{{AFX_MSG_MAP(CSoundVisualizerWnd)
	ON_WM_PAINT()
	ON_WM_CLOSE()
	ON_WM_SIZE()
	ON_WM_RBUTTONDOWN()
	ON_COMMAND(ID_PROPERTIES_DISPLAY, OnProperties)
	ON_WM_CREATE()
	ON_WM_DESTROY()
	ON_WM_MOVE()
	ON_WM_KEYDOWN()
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CSoundVisualizerWnd message handlers

void CSoundVisualizerWnd::OnPaint() 
{
	CPaintDC dc(this); // device context for painting
	CRect rect;
	GetClientRect(&rect);

	CBrush* pOldBrush = (CBrush*)dc.SelectStockObject(BLACK_BRUSH);

	dc.Rectangle(&rect);

	dc.SelectObject(pOldBrush);
}

void CSoundVisualizerWnd::OnClose() 
{
	m_pSoundVisualizer->SetActive(m_nPlugInIndex, FALSE);

//	CMiniFrameWnd::OnClose();        Das hier macht schon das SetActive(..., FALSE)!
}

void CSoundVisualizerWnd::OnSize(UINT nType, int cx, int cy) 
{
	CMiniFrameWnd::OnSize(nType, cx, cy);
	
	Invalidate();

	if (((CHitbaseWinAppBase*)AfxGetApp())->m_pPlugInManager->m_hpiList[m_nPlugInIndex].m_pSoundVisualizerWnd)
		((CHitbaseWinAppBase*)AfxGetApp())->m_pPlugInManager->m_hpiList[m_nPlugInIndex].m_pSoundVisualizerWnd->GetWindowRect(&((CHitbaseWinAppBase*)AfxGetApp())->m_pPlugInManager->m_hpiList[m_nPlugInIndex].m_WindowRect);
}

void CSoundVisualizerWnd::OnRButtonDown(UINT nFlags, CPoint point) 
{
	// JUS 990311
	if (((CHitbaseWinAppBase*)AfxGetApp())->m_pPlugInManager->FullScreenPlugInActive())
		return;
	// JUS_

	CPoint pt;
    GetCursorPos(&pt);

 	CMenu menu;
    menu.CreatePopupMenu();
    menu.AppendMenu(MF_STRING, ID_PROPERTIES_DISPLAY, get_string(TEXT_PROPERTIES)+ L"...");
    menu.TrackPopupMenu(TPM_LEFTALIGN|TPM_RIGHTBUTTON, pt.x, pt.y, this);
	
	CMiniFrameWnd::OnRButtonDown(nFlags, point);
}

void CSoundVisualizerWnd::OnProperties() 
{
	((CHitbaseWinAppBase*)AfxGetApp())->m_pPlugInManager->ChangeOptions(m_nPlugInIndex);
}

int CSoundVisualizerWnd::OnCreate(LPCREATESTRUCT lpCreateStruct) 
{
	if (CMiniFrameWnd::OnCreate(lpCreateStruct) == -1)
		return -1;
	
	return 0;
}

void CSoundVisualizerWnd::OnDestroy() 
{
	CMiniFrameWnd::OnDestroy();
}

void CSoundVisualizerWnd::OnMove(int x, int y) 
{
	CMiniFrameWnd::OnMove(x, y);
	
	if (((CHitbaseWinAppBase*)AfxGetApp())->m_pPlugInManager->m_hpiList[m_nPlugInIndex].m_pSoundVisualizerWnd)
		((CHitbaseWinAppBase*)AfxGetApp())->m_pPlugInManager->m_hpiList[m_nPlugInIndex].m_pSoundVisualizerWnd->GetWindowRect(&((CHitbaseWinAppBase*)AfxGetApp())->m_pPlugInManager->m_hpiList[m_nPlugInIndex].m_WindowRect);
}

void CSoundVisualizerWnd::OnKeyDown(UINT nChar, UINT nRepCnt, UINT nFlags) 
{
	// Für Fullscreen Plugins (Direct Draw)
	// JUS 990206
	if (nChar == VK_ESCAPE)
		PostMessage(WM_CLOSE);
	// JUS_

	CMiniFrameWnd::OnKeyDown(nChar, nRepCnt, nFlags);
}

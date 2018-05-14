// CCDCoverFrame.cpp : implementation of the CCDCoverFrame class
//

#include "stdafx.h"
#include "coverlayout.h"
#include "cdcover.h"
#include "CDCoverFrame.h"
#include "cdcoverdoc.h"
#include "cdcoverview.h"
#include "coverzweckformdlg.h"
#include "dlgcoversizes.h"
#include <io.h>

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

using namespace System;
using namespace Big3::Hitbase::DataBaseEngine;

/////////////////////////////////////////////////////////////////////////////
// CCDCoverFrame

IMPLEMENT_DYNCREATE(CCDCoverFrame, CFrameWnd)

BEGIN_MESSAGE_MAP(CCDCoverFrame, CFrameWnd)
	//{{AFX_MSG_MAP(CCDCoverFrame)
	ON_WM_CREATE()
	ON_COMMAND(ID_BUTTON_ZOOM, OnToolbarButton)
	ON_COMMAND(ID_MYMENU_SETDEFAULT, OnFileNew)
	ON_COMMAND(ID_MYMENU_OPEN, OnFileOpen)
	ON_COMMAND(ID_MYMENU_SAVE, OnFileSave)
	ON_COMMAND(ID_MYMENU_SAVEAS, OnFileSaveAs)
	ON_COMMAND(ID_MYMENU_QUIT, OnMyQuit)
	ON_UPDATE_COMMAND_UI(ID_BUTTON_FRONTCOVER, MySetRadio)
	ON_COMMAND(ID_BUTTON_BACKCOVER, OnToolbarButton)
	ON_COMMAND(ID_BUTTON_FRONTCOVER, OnToolbarButton)
	ON_COMMAND(ID_BUTTON_BOTH, OnToolbarButton)
	ON_COMMAND(ID_BUTTON_LABELCOVER, OnToolbarButton)
	ON_COMMAND(ID_BUTTON_ZWECKFORM, OnToolbarButton)
	ON_COMMAND(ID_BUTTON_SIZES, OnToolbarButton)
	ON_COMMAND(ID_BUTTON_DRAWBORDERS, OnToolbarButton)
	ON_COMMAND(ID_ANSICHT_BEIDECOVER, OnToolbarButton)
	ON_COMMAND(ID_ANSICHT_CDLABEL, OnToolbarButton)
	ON_COMMAND(ID_ANSICHT_FORMATVORLAGEN, OnToolbarButton)
	ON_COMMAND(ID_ANSICHT_HINTERESCOVER, OnToolbarButton)
	ON_COMMAND(ID_ANSICHT_VORDERESCOVER, OnToolbarButton)
	ON_COMMAND(ID_ANSICHT_ZOOM, OnToolbarButton)
	ON_COMMAND(ID_ANSICHT_BORDER, OnToolbarButton)
	ON_UPDATE_COMMAND_UI(ID_BUTTON_BACKCOVER, MySetRadio)
	ON_UPDATE_COMMAND_UI(ID_BUTTON_BOTH, MySetRadio)
	ON_UPDATE_COMMAND_UI(ID_BUTTON_LABELCOVER, MySetRadio)
	ON_UPDATE_COMMAND_UI(ID_BUTTON_DRAWBORDERS, MySetRadio)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

static UINT indicators[] =
{
	ID_SEPARATOR,           // status line indicator
	ID_INDICATOR_CAPS,
	ID_INDICATOR_NUM,
	ID_INDICATOR_SCRL,
};

/////////////////////////////////////////////////////////////////////////////
// CCDCoverFrame construction/destruction

CCDCoverFrame::CCDCoverFrame()
{
}


CCDCoverFrame::~CCDCoverFrame()
{
}


int CCDCoverFrame::OnCreate(LPCREATESTRUCT lpCreateStruct)
{
	if (CFrameWnd::OnCreate(lpCreateStruct) == -1)
		return -1;
	
	if (!m_wndToolBar.CreateEx(this) ||
		!m_wndToolBar.LoadToolBar(IDR_CDCOVERMAINFRAME))
	{
		TRACE0("Failed to create toolbar\n");
		return -1;      // fail to create
	}
/*	if (!m_wndDlgBar.Create(this, IDR_CDCOVERMAINFRAME, 
		CBRS_ALIGN_TOP, AFX_IDW_DIALOGBAR))
	{
		TRACE0("Failed to create dialogbar\n");
		return -1;		// fail to create
	}
*/
	if (!m_wndReBar.Create(this) ||
		!m_wndReBar.AddBar(&m_wndToolBar) /*||
		!m_wndReBar.AddBar(&m_wndDlgBar)*/)
	{
		TRACE0("Failed to create rebar\n");
		return -1;      // fail to create
	}

	if (!m_wndStatusBar.Create(this) ||
		!m_wndStatusBar.SetIndicators(indicators,
		  sizeof(indicators)/sizeof(UINT)))
	{
		TRACE0("Failed to create status bar\n");
		return -1;      // fail to create
	}

	m_wndToolBar.SetBarStyle(m_wndToolBar.GetBarStyle() |
		CBRS_TOOLTIPS | CBRS_FLYBY);

	// Toolbar Buttons einrichten

	m_wndToolBar.SetButtonStyle(m_wndToolBar.CommandToIndex(ID_BUTTON_DRAWBORDERS), TBBS_CHECKBOX);
	m_wndToolBar.SetButtonStyle(m_wndToolBar.CommandToIndex(ID_BUTTON_ZOOM), TBBS_CHECKBOX);
	m_wndToolBar.SetButtonStyle(m_wndToolBar.CommandToIndex(ID_BUTTON_BACKCOVER), TBBS_CHECKBOX | TBBS_CHECKGROUP);
	m_wndToolBar.SetButtonStyle(m_wndToolBar.CommandToIndex(ID_BUTTON_FRONTCOVER), TBBS_CHECKBOX);
	m_wndToolBar.SetButtonStyle(m_wndToolBar.CommandToIndex(ID_BUTTON_BOTH), TBBS_CHECKBOX);
	m_wndToolBar.SetButtonStyle(m_wndToolBar.CommandToIndex(ID_BUTTON_LABELCOVER), TBBS_CHECKBOX);


	// Menu einrichten

	CMenu *pMenu = GetMenu();
	int nButton = ID_ANSICHT_VORDERESCOVER;

	switch (m_Layout.GetCoverMode())
	{
		case CCoverLayout::label:
			nButton = ID_ANSICHT_CDLABEL;
			break;
		case CCoverLayout::back:
			nButton = ID_ANSICHT_HINTERESCOVER;
			break;
		case CCoverLayout::both:
			nButton = ID_ANSICHT_BEIDECOVER;
	}

	pMenu->CheckMenuRadioItem(ID_ANSICHT_VORDERESCOVER, ID_ANSICHT_CDLABEL, nButton, MF_BYCOMMAND);
	pMenu->CheckMenuItem(ID_ANSICHT_ZOOM, MF_BYCOMMAND | (m_Layout.m_bZoomMode ? MF_CHECKED : MF_UNCHECKED));
	pMenu->CheckMenuItem(ID_ANSICHT_BORDER, MF_BYCOMMAND | (m_Layout.m_bDrawBorders ? MF_CHECKED : MF_UNCHECKED));


#ifdef _CDCOVER_DLL
	CCreateContext cc;
	cc.m_pCurrentDoc = NULL;
	cc.m_pCurrentFrame = this;
	cc.m_pLastView = NULL;
	cc.m_pNewDocTemplate = NULL;
	cc.m_pNewViewClass = RUNTIME_CLASS(CCdcoverView);
	CCdcoverView* pView = (CCdcoverView*)CreateView(&cc);
	pView->m_pLayout = &m_Layout;
	pView->m_pOldLayout = &m_oldLayout;
	pView->m_pwndStatusBar = &m_wndStatusBar;
	SetActiveView(pView);
#endif

	return 0;
}

BOOL CCDCoverFrame::PreCreateWindow(CREATESTRUCT& cs)
{
	if( !CFrameWnd::PreCreateWindow(cs) )
	{
		return FALSE;
	}

	return TRUE;
}


/////////////////////////////////////////////////////////////////////////////
// CCDCoverFrame diagnostics

#ifdef _DEBUG
void CCDCoverFrame::AssertValid() const
{
	CFrameWnd::AssertValid();
}

void CCDCoverFrame::Dump(CDumpContext& dc) const
{
	CFrameWnd::Dump(dc);
}

#endif //_DEBUG

/////////////////////////////////////////////////////////////////////////////
// CCDCoverFrame message handlers


void CCDCoverFrame::OnToolbarButton() 
{
	int nOldMode = m_Layout.GetCoverMode();
	BOOL bDontRecalc = TRUE;
	
	CMenu *pMenu = GetMenu();
	int nButton = ID_ANSICHT_VORDERESCOVER;

	const MSG *msg = GetCurrentMessage();
	const int ButtonID = msg->wParam;

	BOOL bRedraw = FALSE;

	switch (ButtonID)
	{
		case ID_BUTTON_SIZES:
		{
			CCoverLayout temp = m_Layout;

			if (! m_Layout.m_bZweckformUser)
			{
				CString s,t;
				s.LoadString(IDS_COVER_SIZESERROR);
				t.LoadString(IDS_COVER_SIZESERRORERROR);
				MessageBox(s, t, MB_ICONSTOP | MB_OK);
				break;
			}

			CDlgCoverSizes dlg(&m_Layout);
			dlg.DoModal();

			bDontRecalc &= (m_Layout.gl_COVERBACKHEIGHT == temp.gl_COVERBACKHEIGHT);
			bDontRecalc &= (m_Layout.gl_COVERBACKWIDTH == temp.gl_COVERBACKWIDTH);
			bDontRecalc &= (m_Layout.gl_COVERFRONTHEIGHT == temp.gl_COVERFRONTHEIGHT);
			bDontRecalc &= (m_Layout.gl_COVERFRONTWIDTH == temp.gl_COVERFRONTWIDTH);
			bDontRecalc &= (m_Layout.gl_COVERBORDERWIDTH == temp.gl_COVERBORDERWIDTH);
			bDontRecalc &= (m_Layout.gl_COVERLABELWIDTH == temp.gl_COVERLABELWIDTH);
			bDontRecalc &= (m_Layout.gl_COVERINNERLABELRADIUS == temp.gl_COVERINNERLABELRADIUS);


			bRedraw = TRUE;
			break;
		}
		case ID_ANSICHT_FORMATVORLAGEN:
		case ID_BUTTON_ZWECKFORM:
		{
			if ((int)m_Layout.m_vecZweckformen.size() != 0)
			{
				CCoverLayout temp = m_Layout;
				CCoverZweckformDlg dlg(&temp);
				m_Layout = (dlg.DoModal() == IDOK) ? temp : m_Layout;
				bRedraw = TRUE;
			}
			break;
		}

		case ID_ANSICHT_ZOOM:
		case ID_BUTTON_ZOOM:
			m_Layout.ToggleZoom();
			break;

		case ID_ANSICHT_BORDER:
		case ID_BUTTON_DRAWBORDERS:
			m_Layout.ToggleBorder();
			bRedraw = TRUE;
			break;

		case ID_ANSICHT_CDLABEL:
		case ID_BUTTON_LABELCOVER:
			m_Layout.SetCoverMode(CCoverLayout::label);
			bRedraw = (nOldMode != CCoverLayout::label);
			nButton = ID_ANSICHT_CDLABEL;
			break;
		case ID_ANSICHT_VORDERESCOVER:
		case ID_BUTTON_FRONTCOVER:
			m_Layout.SetCoverMode(CCoverLayout::front);
			bRedraw = (nOldMode != CCoverLayout::front);
			nButton = ID_ANSICHT_VORDERESCOVER;
			break;
		case ID_ANSICHT_HINTERESCOVER:
		case ID_BUTTON_BACKCOVER:
			m_Layout.SetCoverMode(CCoverLayout::back);
			bRedraw = (nOldMode != CCoverLayout::back);
			nButton = ID_ANSICHT_HINTERESCOVER;
			break;
		case ID_ANSICHT_BEIDECOVER:
		case ID_BUTTON_BOTH:
			m_Layout.SetCoverMode(CCoverLayout::both);
			bRedraw = (nOldMode != CCoverLayout::both);
			nButton = ID_ANSICHT_BEIDECOVER;

	}

	pMenu->CheckMenuRadioItem(ID_ANSICHT_VORDERESCOVER, ID_ANSICHT_CDLABEL, nButton, MF_BYCOMMAND);
	pMenu->CheckMenuItem(ID_ANSICHT_ZOOM, MF_BYCOMMAND | (m_Layout.m_bZoomMode ? MF_CHECKED : MF_UNCHECKED));
	pMenu->CheckMenuItem(ID_ANSICHT_BORDER, MF_BYCOMMAND | (m_Layout.m_bDrawBorders ? MF_CHECKED : MF_UNCHECKED));

	if (! bDontRecalc && m_Layout.m_bZweckformUser)
		m_Layout.ResetDrawPos();

	if (bRedraw)
		RedrawAll();

	return;
}


void CCDCoverFrame::ActivateFrame(int nCmdShow) 
{
	CCdcoverView *pView = (CCdcoverView *)GetActiveView();

	// ACHTUNG: Alles was hier initialisiert wird
	// muss auch im Constructor bei der DLL eingetragen werden

	pView->m_pLayout = &m_Layout;
	pView->m_pOldLayout = &m_oldLayout;
	pView->m_pwndStatusBar = &m_wndStatusBar;
	
	CFrameWnd::ActivateFrame(nCmdShow);

	return;
}


void CCDCoverFrame::BindDBToLayout(const BOOL bLoadData)
{

	// Nicht druckbaren Bereich ermitteln

	CPrintDialog dlg(FALSE);
	dlg.GetDefaults();
	CDC dc;
	dc.Attach(dlg.CreatePrinterDC());
	m_Layout.m_nRandX = dc.GetDeviceCaps(PHYSICALOFFSETX);
	m_Layout.m_nRandY = dc.GetDeviceCaps(PHYSICALOFFSETY);


	// DBs eintragen

	m_Layout.m_pccd = cd;
	m_Layout.m_pdb = db;

	// Gfx eintragen

/*	m_Layout.m_pgfxFront = &m_gfxFront;
	m_Layout.m_pgfxBack = &m_gfxBack;
	m_Layout.m_pgfxLabel = &m_gfxLabel;
	m_Layout.m_pgfxFront->m_bInitComplete = FALSE;
	m_Layout.m_pgfxBack->m_bInitComplete = FALSE;
	m_Layout.m_pgfxLabel->m_bInitComplete = FALSE;
*/

	// Die CD-Grafiken dem Cover zuweisen
	if (!String::IsNullOrEmpty(cd->CDCoverFrontFilename) && !_waccess(CString(cd->CDCoverFrontFilename), 0))
	{
		m_Layout.m_strFrontGfx = cd->CDCoverFrontFilename;
		m_Layout.m_bFrontGfx = TRUE;
		m_Layout.m_gfxFront.m_bInitComplete = FALSE;
	}

	if (!String::IsNullOrEmpty(cd->CDCoverBackFilename) && !_waccess(CString(cd->CDCoverBackFilename), 0))
	{
		m_Layout.m_strBackGfx = cd->CDCoverBackFilename;
		m_Layout.m_bBackGfx = TRUE;
		m_Layout.m_gfxBack.m_bInitComplete = FALSE;
	}

	if (!String::IsNullOrEmpty(cd->CDCoverLabelFilename) && !_waccess(CString(cd->CDCoverLabelFilename), 0))
	{
		m_Layout.m_strLabelGfx = cd->CDCoverLabelFilename;
		m_Layout.m_bLabelGfx = TRUE;
		m_Layout.m_gfxLabel.m_bInitComplete = FALSE;
	}

	// Bei Sampler wird das Defaultlayout (current...)
	// abgeaendert

	if (! cd->Sampler)
	{
		m_Layout.m_vecColumns.clear();
		CCoverColumn column;
		column.m_dWidthRel = 1.0 / 14.0;
		column.m_teColumn.m_nXAlignment = CTextElement::left;
		column.m_nType = CCoverColumn::TRACKNR;
		m_Layout.m_vecColumns.push_back(column);
		column.m_dWidthRel = 11.0 / 14.0;
		column.m_nType = CCoverColumn::TITLE;
		m_Layout.m_vecColumns.push_back(column);
		column.m_dWidthRel = 2.0 / 14.0;
		column.m_nType = CCoverColumn::LENGTH;
		column.m_teColumn.m_nXAlignment = CTextElement::right;
		m_Layout.m_vecColumns.push_back(column);
	}


	// Coverformat Definitionen ermitteln

    wchar_t Buffer[MAX_PATH];
	wchar_t Buffer2[MAX_PATH];
	::GetModuleFileName(NULL, Buffer2, MAX_PATH);
	const CString strHitbasePath = GetPurePathName(CString(Buffer2));
    ::GetCurrentDirectory(MAX_PATH, Buffer);
	::SetCurrentDirectory(strHitbasePath);

	CString strS, strD;
	strS.LoadString(IDS_COVER_DEFAULT_SAMPLER);
	strD.LoadString(IDS_COVER_DEFAULT_NORMAL);

	strS = strHitbasePath + strS;
	strD = strHitbasePath + strD;

	CCoverLayout Temp = m_Layout;
	if (bLoadData && Temp.Load(cd->Sampler ? strS : strD))
	{
		m_Layout = Temp;
		SetWindowText(GetPureFileName(m_Layout.m_strCurrentLayoutName));
	}


	CString strFile, strFmtError, strA, strB;
	strFile.LoadString(IDS_COVERFORMATFILE);
	strFmtError.LoadString(IDS_COVERVORLAGE_ERROR);
	strA.LoadString(IDS_COVERVORLAGE_ERRA);
	strB.LoadString(IDS_COVERVORLAGE_ERRB);

	if (! m_Layout.ParseEngineData(strFile, &m_Layout.m_vecZweckformen))
		MessageBox(strA + strFile + strB, strFmtError, MB_OK | MB_ICONWARNING);


	// Titel und Artist Infos eintragen

	m_Layout.m_teCDTitle.m_strText = cd->Title;
	m_Layout.m_teArtist.m_strText = cd->Artist;
	m_Layout.m_strTrenn = (m_Layout.m_teCDTitle.m_strText == "" || m_Layout.m_teArtist.m_strText == "") ? "" : " - ";

	::SetCurrentDirectory(Buffer);

	return;
}


BOOL CCDCoverFrame::LoadFrame()
{
   return CFrameWnd::LoadFrame(IDR_MAINFRAME);
}


void CCDCoverFrame::OnFileNew() 
{
	if (! (m_oldLayout == m_Layout))
	{
		CString s, t;
		s.LoadString(IDS_SAVECHANGE);
		t.LoadString(IDS_SAVECHANGETITLE);
		if (MessageBox(s, t, MB_ICONQUESTION | MB_YESNO) == IDYES)
			OnFileSave();
	}

	CCoverLayout layoutDefault;
	cd = m_Layout.m_pccd;
	db = m_Layout.m_pdb;
	m_Layout = layoutDefault;

	BindDBToLayout(FALSE);

	RedrawAll();
}
	
void CCDCoverFrame::RedrawAll()
{
	CCdcoverView *pView = (CCdcoverView *)GetActiveView();
	pView->m_bRecalcScroll = TRUE;
	pView->m_bFirstDraw = TRUE;
	pView->m_bWaitForDraw = TRUE;
	m_Layout.m_pActiveRect = NULL;
	m_Layout.m_pLastActiveRect = NULL;

	InvalidateRect(NULL);

	return;
}


void CCDCoverFrame::OnMyQuit() 
{
	if (! (m_oldLayout == m_Layout))
	{
		CString s, t;
		s.LoadString(IDS_SAVECHANGE);
		t.LoadString(IDS_SAVECHANGETITLE);
		if (MessageBox(s, t, MB_ICONQUESTION | MB_YESNO) == IDYES)
			OnFileSave();
	}

	DestroyWindow();
	return;
}


void CCDCoverFrame::OnFileOpen() 
{
	if (! (m_oldLayout == m_Layout))
	{
		CString s, t;
		s.LoadString(IDS_SAVECHANGE);
		t.LoadString(IDS_SAVECHANGETITLE);
		if (MessageBox(s, t, MB_ICONQUESTION | MB_YESNO) == IDYES)
			OnFileSave();
	}

	CString strErr, strLoadErr;

	strErr.LoadString(IDS_COVERERROR);
	strLoadErr.LoadString(IDS_COVERLOAD_ERROR);

	CString strFileName = CCoverLayout::FileBrowser(".cly", "", get_string (IDS_COVER_LAYOUT_EXT), this, TRUE);

	if (strFileName != "")
	{
		CCoverLayout Temp = m_Layout;
		if (Temp.Load(strFileName))
		{
			m_Layout = Temp;
			SetWindowText(GetPureFileName(m_Layout.m_strCurrentLayoutName));
		}
		else
		{
			MessageBox(strLoadErr, strErr, MB_OK);
			m_oldLayout = m_Layout;
		}
	}

	RedrawAll();

	return;
}

void CCDCoverFrame::OnFileSave() 
{
	CString strErr, strSaveErr;
	strErr.LoadString(IDS_COVERERROR);
	strSaveErr.LoadString(IDS_COVERSAVE_ERROR);

	CString strFileName = m_Layout.m_strCurrentLayoutName;

	if (strFileName == "")
		strFileName = CCoverLayout::FileBrowser(".cly", "", get_string (IDS_COVER_LAYOUT_EXT), this, FALSE);

	if (strFileName != "")
	{
		if (! m_Layout.Save(strFileName))
			MessageBox(strSaveErr, strErr, MB_OK);
		else
		{
			SetWindowText(GetPureFileName(m_Layout.m_strCurrentLayoutName));
			m_oldLayout = m_Layout;
		}
	}

	RedrawAll();

	return;
}

void CCDCoverFrame::OnFileSaveAs() 
{
	CString strErr, strSaveErr;
	strErr.LoadString(IDS_COVERERROR);
	strSaveErr.LoadString(IDS_COVERSAVE_ERROR);

	CString strFileName = CCoverLayout::FileBrowser(".cly", m_Layout.m_strCurrentLayoutName, get_string (IDS_COVER_LAYOUT_EXT), this, FALSE);

	if (strFileName != "")
	{
		if (! m_Layout.Save(strFileName))
			MessageBox(strSaveErr, strErr, MB_OK);
		else
		{
			SetWindowText(GetPureFileName(m_Layout.m_strCurrentLayoutName));
			m_oldLayout = m_Layout;
		}
	}

	RedrawAll();

	return;
}


CString CCDCoverFrame::GetPurePathName(const CString& strFullPathName)
{
    CString strReturn;
    int Pos;

    if (strFullPathName.GetLength() != 0)
    {
        if ((Pos = strFullPathName.ReverseFind('\\')) == -1)
        {
            if ((Pos = strFullPathName.ReverseFind('/')) == -1)
            {
                if ((Pos = strFullPathName.ReverseFind(':')) == -1)
                {
                    return strReturn;
                }
            }
        }

        strReturn = strFullPathName.Left(Pos) + "\\";
    }

    return strReturn;
}


void CCDCoverFrame::MySetRadio(CCmdUI* pCmdUI)
{
	BOOL bOn = FALSE;
	if (pCmdUI->m_nID == ID_BUTTON_DRAWBORDERS)
	{
		bOn |= ! m_Layout.m_bDrawBorders;
	}
	else
	{
		int nMode = m_Layout.GetCoverMode();

		bOn |= (nMode == CCoverLayout::front && pCmdUI->m_nID == ID_BUTTON_FRONTCOVER);
		bOn |= (nMode == CCoverLayout::back && pCmdUI->m_nID == ID_BUTTON_BACKCOVER);
		bOn |= (nMode == CCoverLayout::label && pCmdUI->m_nID == ID_BUTTON_LABELCOVER);
		bOn |= (nMode == CCoverLayout::both && pCmdUI->m_nID == ID_BUTTON_BOTH);
	}

	pCmdUI->SetRadio(bOn);

	return;
}


CString CCDCoverFrame::GetPureFileName(const CString& strPath)
{
    int nLen = strPath.GetLength() - 1;
    int nStart = 0;
    const int nStop = nLen;

    if (nLen == -1 || strPath[nLen] == '\\' || strPath[nLen] == ':' || strPath[nLen] == '/')
    {
        CString strEmpty;
        return strEmpty;
    }


    while (nLen > 0)
    {
        if (strPath[nLen] == '\\' || strPath[nLen] == ':' || strPath[nLen] == '/')
        {
            nStart = nLen + 1;
            break;
        }

        nLen--;
    }

    return strPath.Mid(nStart, 1 + nStop - nStart);
}

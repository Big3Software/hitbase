// CoverPreviewView.cpp : Implementierungsdatei
//

#include "stdafx.h"
#include "cdcover.h"
#include "cdcoverview.h"
#include "CoverPreviewView.h"


// CCoverPreviewView

IMPLEMENT_DYNCREATE(CCoverPreviewView, CPreviewView)

CCoverPreviewView::CCoverPreviewView()
{
}

CCoverPreviewView::~CCoverPreviewView()
{
}

BEGIN_MESSAGE_MAP(CCoverPreviewView, CPreviewView)
	ON_COMMAND(AFX_ID_PREVIEW_PRINT, OnFilePrint)
END_MESSAGE_MAP()


// CCoverPreviewView-Diagnose

#ifdef _DEBUG
void CCoverPreviewView::AssertValid() const
{
	CPreviewView::AssertValid();
}

void CCoverPreviewView::Dump(CDumpContext& dc) const
{
	CPreviewView::Dump(dc);
}
#endif //_DEBUG

void CCoverPreviewView::OnFilePrint()
{
	((CCdcoverView*)m_pOrigView)->OnFilePrint();
}


// CCoverPreviewView-Meldungshandler

// cdcoverView.cpp : implementation of the CCdcoverView class
//

#include "stdafx.h"

#include "cdcoverView.h"
#include "resource.h"
#include "cover.h"
#include "coverenterstringdlg.h"
#include "coverfreevaluedlg.h"
#include "coverentermultistringdlg.h"
#include "coverpreviewview.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CCdcoverView

IMPLEMENT_DYNCREATE(CCdcoverView, CScrollView)

BEGIN_MESSAGE_MAP(CCdcoverView, CScrollView)
	//{{AFX_MSG_MAP(CCdcoverView)
	ON_WM_LBUTTONDOWN()
	ON_WM_RBUTTONDOWN()
	ON_WM_SETCURSOR()
	ON_WM_MOUSEMOVE()
	ON_COMMAND(ID_BORDERPOPUP_INTERPRETTITEL, OnBorderpopup)
	ON_COMMAND(ID_BACKPOPUP_AUSRICHTUNG_HOOK, OnBackpopup)
	ON_COMMAND(ID_BACKOPTIONS_ADDGRAFIK, OnBackMainPopup)
	ON_COMMAND(ID_FRONTMAIN_TEXTGFX, OnFrontMainPopup)
	ON_COMMAND(ID_FRONTPOPUP_SETZEFARBEN_TEXTFARBE, OnFrontPopup)
	ON_WM_LBUTTONUP()
	ON_WM_HSCROLL()
	ON_WM_MOUSEWHEEL()
	ON_COMMAND(ID_TEXTBOXPOPUP_NEUESTEXTELEMENT, OnTextBoxMain)
	ON_COMMAND(ID_TBPOPUP_ZEICHENSATZ, OnTBPopup)
	ON_WM_RBUTTONUP()
	ON_COMMAND(ID_LABELMAIN_ADDGRAFIK, OnLabelMainPopup)
	ON_WM_VSCROLL()
	ON_WM_LBUTTONDBLCLK()
	ON_COMMAND(ID_BORDERPOPUP_TEXTTRANSPARENZ, OnBorderpopup)
	ON_COMMAND(ID_BORDERPOPUP_AUSRICHTUNG_OBEN, OnBorderpopup)
	ON_COMMAND(ID_BORDERPOPUP_AUSRICHTUNG_UNTEN, OnBorderpopup)
	ON_COMMAND(ID_BORDERPOPUP_AUSRICHTUNG_ZENTRIERT, OnBorderpopup)
	ON_COMMAND(ID_BORDERPOPUP_ZEICHENSATZ, OnBorderpopup)
	ON_COMMAND(ID_BORDERPOPUP_SETZEFARBEN_TEXTFARBE, OnBorderpopup)
	ON_COMMAND(ID_BORDERPOPUP_SETZEFARBEN_TEXTHINTERGRUND, OnBorderpopup)
	ON_COMMAND(ID_BORDERPOPUP_EDITTEXT, OnBorderpopup)
	ON_COMMAND(ID_BORDERPOPUP_JOINTEXTS, OnBorderpopup)
	ON_COMMAND(ID_BORDERPOPUP_EQUALIZE, OnBorderpopup)
	ON_COMMAND(ID_BACKPOPUP_AUSRICHTUNG_LINKS, OnBackpopup)
	ON_COMMAND(ID_BACKPOPUP_AUSRICHTUNG_RECHTS, OnBackpopup)
	ON_COMMAND(ID_BACKPOPUP_AUSRICHTUNG_ZENTRIERT, OnBackpopup)
	ON_COMMAND(ID_BACKPOPUP_DELCOLUMN, OnBackpopup)
	ON_COMMAND(ID_BACKPOPUP_JOINCOLUMNS, OnBackpopup)
	ON_COMMAND(ID_BACKPOPUP_NEWCOLUMN_BPM, OnBackpopup)
	ON_COMMAND(ID_BACKPOPUP_NEWCOLUMN_INTERPRET, OnBackpopup)
	ON_COMMAND(ID_BACKPOPUP_NEWCOLUMN_KOMMENTAR, OnBackpopup)
	ON_COMMAND(ID_BACKPOPUP_NEWCOLUMN_LAENGE, OnBackpopup)
	ON_COMMAND(ID_BACKPOPUP_NEWCOLUMN_SEPARATOR, OnBackpopup)
	ON_COMMAND(ID_BACKPOPUP_NEWCOLUMN_SONGNAME, OnBackpopup)
	ON_COMMAND(ID_BACKPOPUP_NEWCOLUMN_TRACKNUMMER, OnBackpopup)
	ON_COMMAND(ID_BACKPOPUP_NEWCOLUMN_USER1, OnBackpopup)
	ON_COMMAND(ID_BACKPOPUP_NEWCOLUMN_USER2, OnBackpopup)
	ON_COMMAND(ID_BACKPOPUP_NEWCOLUMN_USER3, OnBackpopup)
	ON_COMMAND(ID_BACKPOPUP_NEWCOLUMN_USER4, OnBackpopup)
	ON_COMMAND(ID_BACKPOPUP_NEWCOLUMN_USER5, OnBackpopup)
	ON_COMMAND(ID_BACKPOPUP_SETTYPE_BPM, OnBackpopup)
	ON_COMMAND(ID_BACKPOPUP_SETTYPE_INTERPRET, OnBackpopup)
	ON_COMMAND(ID_BACKPOPUP_SETTYPE_KOMMENTAR, OnBackpopup)
	ON_COMMAND(ID_BACKPOPUP_SETTYPE_LAENGE, OnBackpopup)
	ON_COMMAND(ID_BACKPOPUP_SETTYPE_SEPARATOR, OnBackpopup)
	ON_COMMAND(ID_BACKPOPUP_SETTYPE_SONGNAME, OnBackpopup)
	ON_COMMAND(ID_BACKPOPUP_SETTYPE_TRACKNUMMER, OnBackpopup)
	ON_COMMAND(ID_BACKPOPUP_SETTYPE_USER1, OnBackpopup)
	ON_COMMAND(ID_BACKPOPUP_SETTYPE_USER2, OnBackpopup)
	ON_COMMAND(ID_BACKPOPUP_SETTYPE_USER3, OnBackpopup)
	ON_COMMAND(ID_BACKPOPUP_SETTYPE_USER4, OnBackpopup)
	ON_COMMAND(ID_BACKPOPUP_SETTYPE_USER5, OnBackpopup)
	ON_COMMAND(ID_BACKPOPUP_SETZEFARBEN_TEXTFARBE, OnBackpopup)
	ON_COMMAND(ID_BACKPOPUP_SETZEFARBEN_TEXTHINTERGRUND, OnBackpopup)
	ON_COMMAND(ID_BACKPOPUP_TEXTTRANSPARENZ, OnBackpopup)
	ON_COMMAND(ID_BACKPOPUP_ZEICHENSATZ, OnBackpopup)
	ON_COMMAND(ID_BACKOPTIONS_DELGRAFIK, OnBackMainPopup)
	ON_COMMAND(ID_BACKOPTIONS_GRAFIKPOSITION_BACK, OnBackMainPopup)
	ON_COMMAND(ID_BACKOPTIONS_GRAFIKPOSITION_BACKBORDER, OnBackMainPopup)
	ON_COMMAND(ID_BACKOPTIONS_XAUSRICHTUNG_LINKS, OnBackMainPopup)
	ON_COMMAND(ID_BACKOPTIONS_XAUSRICHTUNG_RECHTS, OnBackMainPopup)
	ON_COMMAND(ID_BACKOPTIONS_XAUSRICHTUNG_ZENTRIERT, OnBackMainPopup)
	ON_COMMAND(ID_BACKOPTIONS_YAUSRICHTUNG_OBEN, OnBackMainPopup)
	ON_COMMAND(ID_BACKOPTIONS_YAUSRICHTUNG_UNTEN, OnBackMainPopup)
	ON_COMMAND(ID_BACKOPTIONS_YAUSRICHTUNG_ZENTRIERT, OnBackMainPopup)
	ON_COMMAND(ID_BACKMAIN_GRAFIKPOSITION_TEXTGFX, OnBackMainPopup)
	ON_COMMAND(ID_BACKPOPUP_NEWCOLUMN_KENNZEICHEN, OnBackpopup)
	ON_COMMAND(ID_BACKPOPUP_SETTYPE_KENNZEICHEN, OnBackpopup)
	ON_COMMAND(ID_FRONTOPTIONS_ADDGRAFIK, OnFrontMainPopup)
	ON_COMMAND(ID_FRONTOPTIONS_DELGRAFIK, OnFrontMainPopup)
	ON_COMMAND(ID_FRONTMAIN_ARTIST, OnFrontMainPopup)
	ON_COMMAND(ID_FRONTMAIN_CDTITEL, OnFrontMainPopup)
	ON_COMMAND(ID_FRONTPOPUP_SETZEFARBEN_TEXTHINTERGRUND, OnFrontPopup)
	ON_COMMAND(ID_FRONTPOPUP_TEXTTRANSPARENZ, OnFrontPopup)
	ON_COMMAND(ID_FRONTPOPUP_XAUSRICHTUNG_LINKS, OnFrontPopup)
	ON_COMMAND(ID_FRONTPOPUP_XAUSRICHTUNG_RECHTS, OnFrontPopup)
	ON_COMMAND(ID_FRONTPOPUP_XAUSRICHTUNG_ZENTRIERT, OnFrontPopup)
	ON_COMMAND(ID_FRONTPOPUP_YAUSRICHTUNG_OBEN, OnFrontPopup)
	ON_COMMAND(ID_FRONTPOPUP_YAUSRICHTUNG_UNTEN, OnFrontPopup)
	ON_COMMAND(ID_FRONTPOPUP_YAUSRICHTUNG_ZENTRIERT, OnFrontPopup)
	ON_COMMAND(ID_FRONTPOPUP_ZEICHENSATZ, OnFrontPopup)
	ON_COMMAND(ID_BORDERPOPUP_SETZEFARBEN_HINTERGRUND, OnBorderpopup)
	ON_COMMAND(ID_BORDERPOPUP_SETZEFARBEN_RAHMEN, OnBorderpopup)
	ON_COMMAND(ID_BACKMAIN_SETZEFARBEN_HINTERGRUND, OnBackMainPopup)
	ON_COMMAND(ID_BACKMAIN_SETZEFARBEN_RAHMEN, OnBackMainPopup)
	ON_COMMAND(ID_FRONTMAIN_SETZEFARBEN_HINTERGRUND, OnFrontMainPopup)
	ON_COMMAND(ID_FRONTMAIN_SETZEFARBEN_RAHMEN, OnFrontMainPopup)
	ON_COMMAND(ID_FRONTPOPUP_XAUSRICHTUNG_FREI, OnFrontPopup)
	ON_COMMAND(ID_FRONTPOPUP_YAUSRICHTUNG_FREI, OnFrontPopup)
	ON_COMMAND(ID_TEXTBOXPOPUP_CDTITEL, OnTextBoxMain)
	ON_COMMAND(ID_TEXTBOXPOPUP_INTERPRET, OnTextBoxMain)
	ON_COMMAND(ID_TBPOPUP_YAUSRICHTUNG_ZENTRIERT, OnTBPopup)
	ON_COMMAND(ID_TBPOPUP_YAUSRICHTUNG_UNTEN, OnTBPopup)
	ON_COMMAND(ID_TBPOPUP_YAUSRICHTUNG_OBEN, OnTBPopup)
	ON_COMMAND(ID_TBPOPUP_XAUSRICHTUNG_ZENTRIERT, OnTBPopup)
	ON_COMMAND(ID_TBPOPUP_XAUSRICHTUNG_RECHTS, OnTBPopup)
	ON_COMMAND(ID_TBPOPUP_XAUSRICHTUNG_LINKS, OnTBPopup)
	ON_COMMAND(ID_TBPOPUP_TEXTTRANSPARENZ, OnTBPopup)
	ON_COMMAND(ID_TBPOPUP_SETZEFARBEN_TEXTHINTERGRUND, OnTBPopup)
	ON_COMMAND(ID_TBPOPUP_SETZEFARBEN_TEXTFARBE, OnTBPopup)
	ON_COMMAND(ID_TBPOPUP_REMOVE, OnTBPopup)
	ON_COMMAND(ID_BACKPOPUP_NEWCOLUMN_FREI, OnBackpopup)
	ON_COMMAND(ID_BACKPOPUP_SETTYPE_FREI, OnBackpopup)
	ON_COMMAND(ID_LABELMAIN_DELGRAFIK, OnLabelMainPopup)
	ON_COMMAND(ID_LABELMAIN_SETZEFARBEN_HINTERGRUND, OnLabelMainPopup)
	ON_COMMAND(ID_LABELMAIN_SETZEFARBEN_RAHMEN, OnLabelMainPopup)
	ON_COMMAND(ID_LABELMAIN_TEXTGFX, OnLabelMainPopup)
	ON_COMMAND(ID_TEXTBOXPOPUP_EDITIERETEXT, OnTBPopup)
	ON_COMMAND(ID_TBPOPUP_SETZEFARBEN_RAHMEN, OnTBPopup)
	ON_COMMAND(ID_TBPOPUP_SETZEFARBEN_HINTERGRUND, OnTBPopup)
	ON_COMMAND(ID_TBPOPUP_BOXTRANSPARENZ, OnTBPopup)
	ON_COMMAND(ID_TEXTBOXPOPUP_GRAFIK, OnTextBoxMain)
	ON_COMMAND(ID_TBPOPUP_REMOVEGFX, OnTBPopup)
	ON_COMMAND(ID_TBPOPUP_FLIP, OnTBPopup)
	ON_COMMAND(ID_TBPOPUP_ASPECTRATIO, OnTBPopup)
	ON_COMMAND(ID_TBPOPUP_MIRROR, OnTBPopup)
	ON_COMMAND(ID_POSITION_NACHVORNE, OnTBPopup)
	ON_COMMAND(ID_POSITION_NACHHINTEN, OnTBPopup)
	ON_COMMAND(ID_POSITION_TOPMOST, OnTBPopup)
	ON_COMMAND(ID_POSITION_BACKMOST, OnTBPopup)
	ON_COMMAND(ID_POSITION_CENTERX, OnTBPopup)
	ON_COMMAND(ID_POSITION_CENTERXY, OnTBPopup)
	ON_COMMAND(ID_POSITION_CENTERY, OnTBPopup)
	ON_COMMAND(ID_TB_POSITION_NACHVORNE, OnTBPopup)
	ON_COMMAND(ID_TB_POSITION_CENTERX, OnTBPopup)
	ON_COMMAND(ID_TB_POSITION_CENTERXY, OnTBPopup)
	ON_COMMAND(ID_TB_POSITION_CENTERY, OnTBPopup)
	ON_COMMAND(ID_TB_POSITION_NACHHINTEN, OnTBPopup)
	ON_COMMAND(ID_TB_POSITION_TOPMOST, OnTBPopup)
	ON_COMMAND(ID_TB_POSITION_BACKMOST, OnTBPopup)

	ON_COMMAND(ID_EXTRAS_ANZAHLLIEDER, OnTextBoxMain)
	ON_COMMAND(ID_EXTRAS_KATEGORIE, OnTextBoxMain)
	ON_COMMAND(ID_EXTRAS_MEDIUM, OnTextBoxMain)
	ON_COMMAND(ID_EXTRAS_DATUM, OnTextBoxMain)
	ON_COMMAND(ID_EXTRAS_ARCHIVNUMMER, OnTextBoxMain)
	ON_COMMAND(ID_EXTRAS_CODES, OnTextBoxMain)
	ON_COMMAND(ID_EXTRAS_KOMMENTAR, OnTextBoxMain)
	ON_COMMAND(ID_EXTRAS_AUFNAHMEJAHR, OnTextBoxMain)
	ON_COMMAND(ID_EXTRAS_COPYRIGHT, OnTextBoxMain)
	ON_COMMAND(ID_EXTRAS_GESAMTLEN, OnTextBoxMain)
	ON_COMMAND(ID_EXTRAS_CDSETNR, OnTextBoxMain)
	ON_COMMAND(ID_EXTRAS_CDSETNAME, OnTextBoxMain)

	ON_COMMAND(ID_EXTRAS_USER1, OnTextBoxMain)
	ON_COMMAND(ID_EXTRAS_USER2, OnTextBoxMain)
	ON_COMMAND(ID_EXTRAS_USER3, OnTextBoxMain)
	ON_COMMAND(ID_EXTRAS_USER4, OnTextBoxMain)
	ON_COMMAND(ID_EXTRAS_USER5, OnTextBoxMain)


	//}}AFX_MSG_MAP
	// Standard printing commands
	ON_COMMAND(ID_FILE_PRINT, OnFilePrint)
	ON_COMMAND(ID_FILE_PRINT_DIRECT, OnFilePrint)
	ON_COMMAND(ID_FILE_PRINT_PREVIEW, OnFilePrintPreview)
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CCdcoverView construction/destruction

CCdcoverView::CCdcoverView()
{
	m_bRecalcScroll = FALSE;

	m_bWaitForDraw = FALSE;

	m_bFirstDraw = TRUE;
	m_nCurColumn = 0;
	m_bFirstDrag = TRUE;

	m_bMoveTB = FALSE;
	m_bResizeTB = FALSE;

	m_bRightButtonDown = FALSE;
	m_bLeftButtonDown = FALSE;

	m_bMoveCur = FALSE;

	m_bResizeRightCur = FALSE;
	m_bResizeLeftCur = FALSE;
	m_bResizeTopCur = FALSE;
	m_bResizeBottomCur = FALSE;
	m_bResizeD1 = FALSE;
	m_bResizeD2 = FALSE;
	m_bResizeD3 = FALSE;
	m_bResizeD4 = FALSE;

}

CCdcoverView::~CCdcoverView()
{
}

BOOL CCdcoverView::PreCreateWindow(CREATESTRUCT& cs)
{
	return CScrollView::PreCreateWindow(cs);
}

/////////////////////////////////////////////////////////////////////////////
// CCdcoverView drawing

void CCdcoverView::OnDraw(CDC* pDC)
{
	int nOldMode = pDC->SetMapMode(MM_ANISOTROPIC);

	if (m_bRecalcScroll)
	{
		m_bRecalcScroll = FALSE;
		MyInvalidateRect(NULL, TRUE);
	}


	// Zoom Stuff

	if (! pDC->IsPrinting())
		pDC->ScaleViewportExt(m_pLayout->m_nZoomFactor, 100, m_pLayout->m_nZoomFactor, 100);

	// Cover zeichnen

	CPoint ptZweckformFront(m_pLayout->m_nFrontX, m_pLayout->m_nFrontY);
	CPoint ptZweckformBack(m_pLayout->m_nBackX, m_pLayout->m_nBackY);
	CPoint ptZweckformLabel(m_pLayout->m_nLabelX, m_pLayout->m_nLabelY);

	int nCoverMode = m_pLayout->GetCoverMode();

	CFrontCover covFront(pDC, ptZweckformFront.x, ptZweckformFront.y, m_pLayout->m_nRandX, m_pLayout->m_nRandY);
	CBackCover covBack(pDC, ptZweckformBack.x, ptZweckformBack.y, m_pLayout->m_nRandX, m_pLayout->m_nRandY);
	CLabelCover covLabel(pDC, ptZweckformLabel.x, ptZweckformLabel.y, m_pLayout->m_nRandX, m_pLayout->m_nRandY);

	covFront.CreateCoverFromLayout(m_pLayout, pDC);
	covBack.CreateCoverFromLayout(m_pLayout, pDC);
	covLabel.CreateCoverFromLayout(m_pLayout);

	if ((nCoverMode == CCoverLayout::front) || (nCoverMode == CCoverLayout::both))
	{
		if (pDC->RectVisible(covFront.GetFrontRect()))
		{
			covFront.Draw(m_pLayout, pDC);
			if (! pDC->IsPrinting())
				for (int t = 0; t < (int)m_pLayout->m_tbsF.size(); t++)
					FrameTB(pDC, &m_pLayout->m_tbsF[t].m_rectArea, &m_ptF, m_pLayout->m_colFront);
		}
	}

	if ((nCoverMode == CCoverLayout::back) || (nCoverMode == CCoverLayout::both))
	{
		if (pDC->RectVisible(covBack.GetLeftRect()) || pDC->RectVisible(covBack.GetBackRect()) || pDC->RectVisible(covBack.GetRightRect()))
		{
			covBack.Draw(m_pLayout, pDC);
			if (! pDC->IsPrinting())
				for (int t = 0; t < (int)m_pLayout->m_tbsB.size(); t++)
					FrameTB(pDC, &m_pLayout->m_tbsB[t].m_rectArea, &m_ptB, m_pLayout->m_colBack);
		}
	}

	if (nCoverMode == CCoverLayout::label)
	{
		if (pDC->RectVisible(covLabel.GetLabelRect()))
		{
			CRgn rgn1, rgn2, rgn3, rgnNew;
			CRect rect1 = CRect(covLabel.m_nXPos, -covLabel.m_nYPos, covLabel.m_nXPos + m_pLayout->gl_COVERLABELWIDTH, -covLabel.m_nYPos - m_pLayout->gl_COVERLABELHEIGHT);
			CRect rect2 = CRect(covLabel.m_nXPos + ((m_pLayout->gl_COVERLABELWIDTH-m_pLayout->gl_COVERINNERLABELRADIUS)/2), -covLabel.m_nYPos - ((m_pLayout->gl_COVERLABELWIDTH-m_pLayout->gl_COVERINNERLABELRADIUS)/2), covLabel.m_nXPos + m_pLayout->gl_COVERLABELWIDTH - ((m_pLayout->gl_COVERLABELWIDTH-m_pLayout->gl_COVERINNERLABELRADIUS)/2), -covLabel.m_nYPos - m_pLayout->gl_COVERLABELHEIGHT + ((m_pLayout->gl_COVERLABELWIDTH-m_pLayout->gl_COVERINNERLABELRADIUS)/2));
			CRect rect3 = CRect(covLabel.m_nXPos, -covLabel.m_nYPos, covLabel.m_nXPos + m_pLayout->gl_COVERLABELWIDTH, -covLabel.m_nYPos - m_pLayout->gl_COVERLABELHEIGHT);
			rect1.NormalizeRect();
			rect2.NormalizeRect();
			rect3.NormalizeRect();
			rgnNew.CreateRectRgn(0,0,0,0);

			if (! pDC->IsPrinting())
			{
				rect1 = RectInWE(&rect1);
				rect2 = RectInWE(&rect2);
				rect2.NormalizeRect();
				rect2.NormalizeRect();
				rect1.bottom++;
				rect1.right++;
				rect2.bottom++;
				rect2.right++;
				rgn1.CreateEllipticRgn(rect1.left, rect1.top, rect1.right, rect1.bottom);
				rgn2.CreateEllipticRgn(rect2.left, rect2.top, rect2.right, rect2.bottom);
				rgnNew.CombineRgn(&rgn1, &rgn2, RGN_DIFF);

				pDC->SelectClipRgn(&rgnNew);
			}

			covLabel.Draw(m_pLayout, pDC);

			if (! pDC->IsPrinting())
			{
				for (int t = 0; t < (int)m_pLayout->m_tbsL.size(); t++)
					FrameTB(pDC, &m_pLayout->m_tbsL[t].m_rectArea, &m_ptL, m_pLayout->m_colLabel);
			}
			else
			{
				rgn1.CreateEllipticRgn(rect1.left, rect1.top, rect1.right, rect1.bottom);
				rgn2.CreateEllipticRgn(rect2.left, rect2.top, rect2.right, rect2.bottom);
				rect3 = RectInWE(&rect3);
				rect3.NormalizeRect();
				rect1.bottom++;
				rect1.right++;
				rect2.bottom++;
				rect2.right++;
				rect3.bottom++;
				rect3.right++;
				rect3 = RectInVP(&rect3);
				rect3.NormalizeRect();
				rgn3.CreateRectRgn(rect3.left, rect3.top, rect3.right, rect3.bottom);
				rgnNew.CombineRgn(&rgn1, &rgn2, RGN_DIFF);
				rgnNew.CombineRgn(&rgn3, &rgnNew, RGN_DIFF);

				CBrush brush((COLORREF) 0x00ffffff);
				pDC->FillRgn(&rgnNew, &brush);
			}
		}
	}

	pDC->SelectClipRgn(NULL);

	for (int i = 0; i < (int)covLabel.m_vecCircles.size(); i++)
	{
		CBrush brushBorder;
		brushBorder.CreateStockObject(NULL_BRUSH);
		CPen penBorder(PS_SOLID, 0, m_pLayout->m_bDrawBorders ? covLabel.m_vecCircles[i].m_colBorder : covLabel.m_vecCircles[i].m_colBackground);
		CBrush *pOldBrush = pDC->SelectObject(&brushBorder);
		CPen *pOldPen = pDC->SelectObject(&penBorder);
		pDC->Ellipse(&(covLabel.m_vecCircles[i].m_rectFrame));
		pDC->SelectObject(pOldPen);
		pDC->SelectObject(pOldBrush);
	}

	if (! pDC->IsPrinting() && m_pLayout->m_pActiveRect != NULL && ! m_bMoveTB && ! m_bResizeTB)
	{
		CPen pen(PS_SOLID, 0, (COLORREF) 0x80);
		CPen *pOldPen = pDC->SelectObject(&pen);
		CRect *pRect = m_pLayout->m_pActiveRect;
		*pRect = RectInVP(pRect);

		pDC->MoveTo(pRect->left + 10, pRect->top + 10);
		pDC->LineTo(pRect->left - 10, pRect->top + 10);
		pDC->LineTo(pRect->left - 10, pRect->top - 10);

		pDC->MoveTo(pRect->right - 10, pRect->top + 10);
		pDC->LineTo(pRect->right + 10, pRect->top + 10);
		pDC->LineTo(pRect->right + 10, pRect->top - 10);

		pDC->MoveTo(pRect->right - 10, pRect->bottom - 10);
		pDC->LineTo(pRect->right + 10, pRect->bottom - 10);
		pDC->LineTo(pRect->right + 10, pRect->bottom + 10);

		pDC->MoveTo(pRect->left + 10, pRect->bottom - 10);
		pDC->LineTo(pRect->left - 10, pRect->bottom - 10);
		pDC->LineTo(pRect->left - 10, pRect->bottom + 10);
	
		pDC->SelectObject(pOldPen);
	}


	if (! pDC->IsPrinting())
	{

		// Regionsupdate

		m_ptF = CPoint(covFront.GetFrontRect().left, covFront.GetFrontRect().top);
		m_ptB = CPoint(covBack.GetBackRect().left, covBack.GetBackRect().top);
		m_ptL = CPoint(covLabel.GetLabelRect().left, covLabel.GetLabelRect().top);

		m_pLayout->m_rectArtist = covFront.GetArtistRect();
		pDC->LPtoDP(&m_pLayout->m_rectArtist);
		m_pLayout->m_rectArtist.NormalizeRect();

		m_pLayout->m_rectTitle = covFront.GetTitleRect();
		pDC->LPtoDP(&m_pLayout->m_rectTitle);
		m_pLayout->m_rectTitle.NormalizeRect();

		m_pLayout->m_rectLeft = covBack.GetLeftRect();
		pDC->LPtoDP(&m_pLayout->m_rectLeft);
		m_pLayout->m_rectLeft.NormalizeRect();
		m_pLayout->m_rectRight = covBack.GetRightRect();
		pDC->LPtoDP(&m_pLayout->m_rectRight);
		m_pLayout->m_rectLeft.NormalizeRect();
		m_pLayout->m_rectBack = covBack.GetBackRect();
	
		CRect rectTMPBack = m_pLayout->m_rectBack;
		rectTMPBack.left += 50;
		rectTMPBack.right -= 50;
		pDC->LPtoDP(&rectTMPBack);
		rectTMPBack.NormalizeRect();
	
		pDC->LPtoDP(&m_pLayout->m_rectBack);
		m_pLayout->m_rectBack.NormalizeRect();

		m_pLayout->m_rectFront = covFront.GetFrontRect();
		pDC->LPtoDP(&m_pLayout->m_rectFront);
		m_pLayout->m_rectFront.NormalizeRect();
	
		m_pLayout->m_rectLabel = covLabel.GetLabelRect();
		m_pLayout->m_rectInnerLabel = covLabel.GetInnerLabelRect();
		pDC->LPtoDP(&m_pLayout->m_rectLabel);
		m_pLayout->m_rectLabel.NormalizeRect();
		pDC->LPtoDP(&m_pLayout->m_rectInnerLabel);
		m_pLayout->m_rectInnerLabel.NormalizeRect();


		m_pLayout->m_vecTBLabel.clear();
		for (int i = 0; i < (int)m_pLayout->m_tbsL.size(); i++)
		{
			CCoverTextBox tb;

			tb.m_teText = m_pLayout->m_tbsL[i].m_teText;
			tb.m_rectArea = m_pLayout->m_tbsL[i].m_rectArea;
			tb.m_rectArea.left += m_ptL.x;
			tb.m_rectArea.right += m_ptL.x;
			tb.m_rectArea.top += m_ptL.y;
			tb.m_rectArea.bottom += m_ptL.y;
			pDC->LPtoDP(&tb.m_rectArea);
			tb.m_rectArea.NormalizeRect();
			m_pLayout->m_vecTBLabel.push_back(tb);
		}
		m_pLayout->m_vecTBFront.clear();
		for (int i = 0; i < (int)m_pLayout->m_tbsF.size(); i++)
		{
			CCoverTextBox tb;
			tb.m_teText = m_pLayout->m_tbsF[i].m_teText;
			tb.m_rectArea = m_pLayout->m_tbsF[i].m_rectArea;
			tb.m_rectArea.left += m_ptF.x;
			tb.m_rectArea.right += m_ptF.x;
			tb.m_rectArea.top += m_ptF.y;
			tb.m_rectArea.bottom += m_ptF.y;
			pDC->LPtoDP(&tb.m_rectArea);
			tb.m_rectArea.NormalizeRect();
			m_pLayout->m_vecTBFront.push_back(tb);
		}
		m_pLayout->m_vecTBBack.clear();
		for (int i = 0; i < (int)m_pLayout->m_tbsB.size(); i++)
		{
			CCoverTextBox tb;
			tb.m_teText = m_pLayout->m_tbsB[i].m_teText;
			tb.m_rectArea = m_pLayout->m_tbsB[i].m_rectArea;
			tb.m_rectArea.left += m_ptB.x;
			tb.m_rectArea.right += m_ptB.x;
			tb.m_rectArea.top += m_ptB.y;
			tb.m_rectArea.bottom += m_ptB.y;
			pDC->LPtoDP(&tb.m_rectArea);
			tb.m_rectArea.NormalizeRect();
			m_pLayout->m_vecTBBack.push_back(tb);
		}

		if (m_bFirstDraw)
		{
			m_bFirstDraw = FALSE;

			m_header.DestroyWindow();

			if ((nCoverMode == CCoverLayout::back) || (nCoverMode == CCoverLayout::both))
				BuildHeader(rectTMPBack);

			*m_pOldLayout = *m_pLayout;
		}
	}

	pDC->SetMapMode(nOldMode);

	return;
}

/////////////////////////////////////////////////////////////////////////////
// CCdcoverView printing

BOOL CCdcoverView::OnPreparePrinting(CPrintInfo* pInfo)
{
	// default preparation
	return DoPreparePrinting(pInfo);
}
void CCdcoverView::OnBeginPrinting(CDC* pDC, CPrintInfo *pInfo)
{
	return;
}

void CCdcoverView::OnEndPrinting(CDC* /*pDC*/, CPrintInfo* /*pInfo*/)
{
	return;
}

/////////////////////////////////////////////////////////////////////////////
// CCdcoverView diagnostics

#ifdef _DEBUG
void CCdcoverView::AssertValid() const
{
	CScrollView::AssertValid();
}

void CCdcoverView::Dump(CDumpContext& dc) const
{
	CScrollView::Dump(dc);
}

#endif //_DEBUG

/////////////////////////////////////////////////////////////////////////////
// CCdcoverView message handlers

void CCdcoverView::OnInitialUpdate() 
{
	SetScrollSizes(MM_LOMETRIC, CSize(2100, 2970));
	CScrollView::OnInitialUpdate();

	m_hNormalCursor = ::LoadCursor(NULL, IDC_ARROW);
	m_hMoveCursor = ::LoadCursor(NULL, IDC_SIZEALL);
	m_hResizeRightCursor = ::LoadCursor(NULL, IDC_SIZEWE);
	m_hResizeTopCursor = ::LoadCursor(NULL, IDC_SIZENS);
	m_hZoomCursor = AfxGetApp()->LoadCursor(IDC_CURSOR_ZOOM);
	m_hResizeNWSECursor= ::LoadCursor(NULL, IDC_SIZENWSE);
	m_hResizeNESWCursor= ::LoadCursor(NULL, IDC_SIZENESW);

	return;
}

void CCdcoverView::OnLButtonDown(UINT nFlags, CPoint point) 
{
	m_bWaitForDraw = FALSE;
	UpdateActiveRect(point);

	if (m_bLeftButtonDown)
		return;

	m_bLeftButtonDown = TRUE;
	RefreshSelector(m_pLayout->m_pLastActiveRect, TRUE);

	if (m_pLayout->ZoomEnabled())
	{
		if (m_pLayout->SetZoomFactor(m_pLayout->m_nZoomFactor + 10))
		{
			m_bFirstDraw = TRUE;
			MyInvalidateRect(NULL, TRUE);
		}

		UpdateZoomStatusLine();
	}
	else
	{
		CheckTBState(&point);

		if (m_pLayout->m_pActiveTB == NULL)
		{
			if (m_pLayout->m_pActiveRect != NULL && /*m_pLayout->m_pActiveRect != m_pLayout->m_rectLeft && m_pLayout->m_pActiveRect != m_pLayout->m_rectRight &&*/ m_pLayout->m_pActiveRect != m_pLayout->m_rectArtist && m_pLayout->m_pActiveRect != m_pLayout->m_rectTitle)
			{
				m_bLeftButtonDown = TRUE;
				m_bFirstDrag = TRUE;
				m_rectGrab.top = point.y;
				m_rectGrab.left = point.x;
				m_rectGrab.bottom = point.y;
				m_rectGrab.right = point.x;
			}
		}
		else
		{
			m_ptMouseSet = PointInVP(&point);
			m_ptMouseSet = PtRelToTB(&m_ptMouseSet);

			m_bResizeTB = (m_bResizeRightCur || m_bResizeLeftCur || m_bResizeTopCur || m_bResizeBottomCur || m_bResizeD4 || m_bResizeD3 || m_bResizeD2 || m_bResizeD1);
			m_bMoveTB = m_bMoveCur;
			m_rectSizeRect = m_pLayout->m_pActiveTB->m_rectArea;
		}
	}

	CScrollView::OnLButtonDown(nFlags, point);

	return;
}

void CCdcoverView::OnRButtonDown(UINT nFlags, CPoint point) 
{
	m_bWaitForDraw = FALSE;
	UpdateActiveRect(point);

	if (m_bRightButtonDown)
		return;

	m_bRightButtonDown = TRUE;
	RefreshSelector(m_pLayout->m_pLastActiveRect, TRUE);

	if (m_bMoveTB || m_bResizeTB)
	{
		m_bMoveTB = FALSE;
		m_bLeftButtonDown = FALSE;
		m_rectGrab = CRect(0,0,0,0);
		MyInvalidateRect(NULL);
		return;
	}

	if (m_pLayout->ZoomEnabled())
	{
		if (m_pLayout->SetZoomFactor(m_pLayout->m_nZoomFactor - 10))
		{
			m_bFirstDraw = TRUE;
			MyInvalidateRect(NULL, TRUE);
		}

		UpdateZoomStatusLine();
	}
	else
	{
		if (m_pLayout->m_pActiveRect == &m_pLayout->m_rectLabel)
			WorkOnLabel();

		if (m_pLayout->m_pActiveRect == &m_pLayout->m_rectLeft || m_pLayout->m_pActiveRect == &m_pLayout->m_rectRight)
			WorkOnBorder();

		if (m_pLayout->m_pActiveRect == &m_pLayout->m_rectBack)
			WorkOnBack();

		if (m_pLayout->m_pActiveRect == &m_pLayout->m_rectFront)
			WorkOnFront();

		if (m_pLayout->m_pActiveRect == &m_pLayout->m_rectArtist || m_pLayout->m_pActiveRect == &m_pLayout->m_rectTitle)
			WorkOnArtistTitle();

		if (m_pLayout->m_pActiveTB != NULL)
			WorkOnTextBox();		

		m_bRightButtonDown = FALSE;
		m_bLeftButtonDown = FALSE;

	}

	CScrollView::OnRButtonDown(nFlags, point);

	return;
}


BOOL CCdcoverView::OnSetCursor(CWnd* pWnd, UINT nHitTest, UINT message) 
{
	CWnd::OnSetCursor(pWnd, nHitTest, message);

	HCURSOR *pcur = &m_hNormalCursor;

	if (m_pLayout->ZoomEnabled() && nHitTest == HTCLIENT)
	{
		pcur = &m_hZoomCursor;
		UpdateZoomStatusLine();
	}
	if (m_bMoveCur && nHitTest == HTCLIENT)
		pcur = &m_hMoveCursor;

	if ((m_bResizeRightCur || m_bResizeLeftCur) && nHitTest == HTCLIENT)
		pcur = &m_hResizeRightCursor;

	if ((m_bResizeTopCur || m_bResizeBottomCur) && nHitTest == HTCLIENT)
		pcur = &m_hResizeTopCursor;

	if ((m_bResizeD1 || m_bResizeD4) && nHitTest == HTCLIENT)
		pcur = &m_hResizeNWSECursor;

	if ((m_bResizeD2 || m_bResizeD3) && nHitTest == HTCLIENT)
		pcur = &m_hResizeNESWCursor;
	
	::SetCursor(*pcur);

	return FALSE;
}



void CCdcoverView::OnMouseMove(UINT nFlags, CPoint point) 
{
	if (! m_pLayout->m_bZoomMode)
	{
		CPoint mp = PointInVP(&point);
		CString strX, strY;
		strX.Format(L"X =  %d mm", (abs(mp.x))); //+ m_pLayout->m_nRandX) / 10);
		strY.Format(L"Y =  %d mm", (abs(mp.y))); //+ m_pLayout->m_nRandY) / 10);
		m_pwndStatusBar->SetPaneText(0, strX + " , " + strY, TRUE);
	}

	if (m_pLayout->m_pActiveTB == NULL)
	{
		m_bMoveCur = FALSE;
		m_bResizeRightCur = FALSE;
		m_bResizeLeftCur = FALSE;
		m_bResizeTopCur = FALSE;
		m_bResizeBottomCur = FALSE;
		m_bResizeD1 = FALSE;
		m_bResizeD2 = FALSE;
		m_bResizeD3 = FALSE;
		m_bResizeD4 = FALSE;
	}

	if (m_bResizeTB && m_pLayout->m_pActiveTB != NULL)
	{
		CPoint p = PointInVP(&point);
		p = PtRelToTB(&p);
		p.x -= m_ptMouseSet.x;
		p.y -= m_ptMouseSet.y;

		CRect rectMain = RectInVP(m_pLayout->m_pActiveMainRect);
		CRect rectOld = m_pLayout->m_pActiveTB->m_rectArea;
		
		if (m_bResizeLeftCur)
		{
			m_pLayout->m_pActiveTB->m_rectArea.left = rectOld.left + p.x;
			if (m_pLayout->m_pActiveTB->m_rectArea.left >= m_pLayout->m_pActiveTB->m_rectArea.right - 30)
				m_pLayout->m_pActiveTB->m_rectArea.left = m_pLayout->m_pActiveTB->m_rectArea.right - 30;

			if (m_pLayout->m_pActiveTB->m_rectArea.left < 0)
				m_pLayout->m_pActiveTB->m_rectArea.left = 0;
		}
		if (m_bResizeRightCur)
		{
			m_pLayout->m_pActiveTB->m_rectArea.right = m_rectSizeRect.right + p.x;
			if (m_pLayout->m_pActiveTB->m_rectArea.right <= m_pLayout->m_pActiveTB->m_rectArea.left + 80)
				m_pLayout->m_pActiveTB->m_rectArea.right = m_pLayout->m_pActiveTB->m_rectArea.left + 80;

			if (m_pLayout->m_pActiveTB->m_rectArea.right + rectMain.left > rectMain.right)
				m_pLayout->m_pActiveTB->m_rectArea.right = rectMain.right - rectMain.left;
		}
		if (m_bResizeTopCur)
		{
			m_pLayout->m_pActiveTB->m_rectArea.top = rectOld.top + p.y;
			if (m_pLayout->m_pActiveTB->m_rectArea.top <= m_pLayout->m_pActiveTB->m_rectArea.bottom + 80)
				m_pLayout->m_pActiveTB->m_rectArea.top = m_pLayout->m_pActiveTB->m_rectArea.bottom + 80;

			if (m_pLayout->m_pActiveTB->m_rectArea.top > 0)
				m_pLayout->m_pActiveTB->m_rectArea.top = 0;
		}
		if (m_bResizeBottomCur)
		{
			m_pLayout->m_pActiveTB->m_rectArea.bottom = m_rectSizeRect.bottom + p.y;
			if (m_pLayout->m_pActiveTB->m_rectArea.bottom >= m_pLayout->m_pActiveTB->m_rectArea.top - 80)
				m_pLayout->m_pActiveTB->m_rectArea.bottom = m_pLayout->m_pActiveTB->m_rectArea.top - 80;

			if (m_pLayout->m_pActiveTB->m_rectArea.bottom < rectMain.bottom - rectMain.top)
				m_pLayout->m_pActiveTB->m_rectArea.bottom = rectMain.bottom - rectMain.top;
		}

		RefreshTextBox(&rectOld, &m_pLayout->m_pActiveTB->m_rectArea);

		return;
	}

	if (m_bMoveTB && m_pLayout->m_pActiveTB != NULL)
	{
		CPoint p = PointInVP(&point);
		p = PtRelToTB(&p);
		p.x -= m_ptMouseSet.x;
		p.y -= m_ptMouseSet.y;

		CRect rectOld = m_pLayout->m_pActiveTB->m_rectArea;

		int oldw = m_pLayout->m_pActiveTB->m_rectArea.right - m_pLayout->m_pActiveTB->m_rectArea.left;
		int oldh = m_pLayout->m_pActiveTB->m_rectArea.bottom - m_pLayout->m_pActiveTB->m_rectArea.top;

		m_pLayout->m_pActiveTB->m_rectArea.left += p.x;
		m_pLayout->m_pActiveTB->m_rectArea.top += p.y;
		m_pLayout->m_pActiveTB->m_rectArea.right += p.x;
		m_pLayout->m_pActiveTB->m_rectArea.bottom += p.y;

		CRect rectMain = RectInVP(m_pLayout->m_pActiveMainRect);

		if (m_pLayout->m_pActiveTB->m_rectArea.left < 0)
		{
			m_pLayout->m_pActiveTB->m_rectArea.left = 0;
			m_pLayout->m_pActiveTB->m_rectArea.right = m_pLayout->m_pActiveTB->m_rectArea.left + oldw;
		}
		if (m_pLayout->m_pActiveTB->m_rectArea.right + rectMain.left > rectMain.right)
		{
			m_pLayout->m_pActiveTB->m_rectArea.right = rectMain.right - rectMain.left;
			m_pLayout->m_pActiveTB->m_rectArea.left = m_pLayout->m_pActiveTB->m_rectArea.right - oldw;
		}
		if (m_pLayout->m_pActiveTB->m_rectArea.top > 0)
		{
			m_pLayout->m_pActiveTB->m_rectArea.top = 0;
			m_pLayout->m_pActiveTB->m_rectArea.bottom = m_pLayout->m_pActiveTB->m_rectArea.top + oldh;
		}
		if (m_pLayout->m_pActiveTB->m_rectArea.bottom < rectMain.bottom - rectMain.top)
		{
			m_pLayout->m_pActiveTB->m_rectArea.bottom = rectMain.bottom - rectMain.top;
			m_pLayout->m_pActiveTB->m_rectArea.top = m_pLayout->m_pActiveTB->m_rectArea.bottom - oldh;
		}

		RefreshTextBox(&rectOld, &m_pLayout->m_pActiveTB->m_rectArea);

		return;
	}

	if (! m_bMoveTB && ! m_bResizeTB && m_bLeftButtonDown && m_pLayout->m_pActiveRect != NULL)
	{
		if (m_pLayout->m_pActiveRect != &m_pLayout->m_rectArtist && m_pLayout->m_pActiveRect != &m_pLayout->m_rectTitle)
		{
			if (! m_pLayout->m_pActiveRect->PtInRect(point))
			{
				if (point.x < m_pLayout->m_pActiveRect->left)
					point.x = m_pLayout->m_pActiveRect->left;

				if (point.x > m_pLayout->m_pActiveRect->right)
					point.x = m_pLayout->m_pActiveRect->right;

				if (point.y > m_pLayout->m_pActiveRect->bottom)
					point.y = m_pLayout->m_pActiveRect->bottom;

				if (point.y < m_pLayout->m_pActiveRect->top)
					point.y = m_pLayout->m_pActiveRect->top;
			}
			
			m_rectGrab.bottom = point.y;
			m_rectGrab.right = point.x;
	
			CDC *pDC = GetDC();

			CRect rect = m_rectGrab;
			pDC->DPtoLP(&rect);
			rect.NormalizeRect();

			SIZE s;
			s.cx = 1;
			s.cy = 1;
			pDC->DrawDragRect(&rect, s, m_bFirstDrag ? NULL : &m_rectOldGrab, s);

			if (m_bFirstDrag)
				m_bFirstDrag = FALSE;

			m_rectOldGrab = rect;

			ReleaseDC(pDC);
		}

		return;
	}

	UpdateActiveRect(point);

	CheckTBState(&point);

	CScrollView::OnMouseMove(nFlags, point);

	return;
}



void CCdcoverView::WorkOnBorder()
{
	CTextElement *pTextElement = &m_pLayout->m_teBorderRight;
	BOOL *pbAT = &m_pLayout->m_bRightBorder_AT;

	if (m_pLayout->m_pActiveRect == &m_pLayout->m_rectLeft)
	{
		pTextElement = &m_pLayout->m_teBorderLeft;
		pbAT = &m_pLayout->m_bLeftBorder_AT;
	}

	CPoint pt;
	::GetCursorPos(&pt);
	
	CMenu menu;
	menu.LoadMenu(IDR_MENU_BORDERPOPUP);
	CMenu* pSubMenu = menu.GetSubMenu(0);

	UINT nCheckedTransparent = ((pTextElement->m_nBkMode == TRANSPARENT) ? MF_CHECKED : MF_UNCHECKED);

	int nAlign = ID_BORDERPOPUP_AUSRICHTUNG_ZENTRIERT;

	switch (pTextElement->m_nYAlignment)
	{
		case CTextElement::top:
			nAlign = ID_BORDERPOPUP_AUSRICHTUNG_OBEN;
			break;
		case CTextElement::bottom:
			nAlign = ID_BORDERPOPUP_AUSRICHTUNG_UNTEN;
	}


	pSubMenu->CheckMenuItem(nAlign, MF_BYCOMMAND | MF_CHECKED);
	pSubMenu->CheckMenuItem(ID_BORDERPOPUP_TEXTTRANSPARENZ, MF_BYCOMMAND | nCheckedTransparent);
	
	pSubMenu->CheckMenuItem(ID_BORDERPOPUP_JOINTEXTS, MF_BYCOMMAND | (m_pLayout->m_bJoinBorderTexts ? MF_CHECKED : MF_UNCHECKED));
	pSubMenu->EnableMenuItem(ID_BORDERPOPUP_EDITTEXT, MF_BYCOMMAND | (*pbAT ? MF_GRAYED : MF_ENABLED));

	pSubMenu->CheckMenuItem(ID_BORDERPOPUP_INTERPRETTITEL, MF_BYCOMMAND | (*pbAT ? MF_CHECKED : MF_UNCHECKED));
	pSubMenu->TrackPopupMenu(TPM_LEFTALIGN | TPM_RIGHTBUTTON, pt.x, pt.y, this);

	MyInvalidateRect(&m_pLayout->m_rectLeft);
	MyInvalidateRect(&m_pLayout->m_rectRight);
	
	return;
}



void CCdcoverView::OnBorderpopup() 
{
	const MSG *msg = GetCurrentMessage();
	const int MenuID = msg->wParam;

	CTextElement *pTextElement = &m_pLayout->m_teBorderRight;
	CTextElement *pTextElementOther = &m_pLayout->m_teBorderLeft;

	if (m_pLayout->m_pActiveRect == &m_pLayout->m_rectLeft)
	{
		pTextElement = &m_pLayout->m_teBorderLeft;
		pTextElementOther = &m_pLayout->m_teBorderRight;
	}

	switch (MenuID)
	{
		case ID_BORDERPOPUP_INTERPRETTITEL:
		{
			m_pLayout->m_bBorderToArtistTitle = ! m_pLayout->m_bBorderToArtistTitle;

			if (m_pLayout->m_pActiveRect == &m_pLayout->m_rectLeft)
			{
				m_pLayout->m_bLeftBorder_AT = m_pLayout->m_bBorderToArtistTitle;
				m_pLayout->m_bRightBorder_Dominate = FALSE;
				m_pLayout->m_bLeftBorder_Dominate = TRUE;
			}
			else
			{
				m_pLayout->m_bRightBorder_AT = m_pLayout->m_bBorderToArtistTitle;
				m_pLayout->m_bLeftBorder_Dominate = FALSE;
				m_pLayout->m_bRightBorder_Dominate = TRUE;
			}
			if (m_pLayout->m_bJoinBorderTexts && m_pLayout->m_bLeftBorder_Dominate)
			{
				m_pLayout->m_bRightBorder_AT = m_pLayout->m_bLeftBorder_AT;
			}
			if (m_pLayout->m_bJoinBorderTexts && m_pLayout->m_bRightBorder_Dominate)
			{
				m_pLayout->m_bLeftBorder_AT = m_pLayout->m_bRightBorder_AT;
			}

			break;
		}

		case ID_BORDERPOPUP_TEXTTRANSPARENZ:
			pTextElement->m_nBkMode = (pTextElement->m_nBkMode == OPAQUE) ? TRANSPARENT : OPAQUE;
			if (m_pLayout->m_bJoinBorderTexts)
			{
				pTextElementOther->m_nBkMode = pTextElement->m_nBkMode;
			}
			break;
		case ID_BORDERPOPUP_AUSRICHTUNG_OBEN:
			pTextElement->m_nYAlignment = CTextElement::top;
			if (m_pLayout->m_bJoinBorderTexts)
			{
				pTextElementOther->m_nYAlignment = pTextElement->m_nYAlignment;
			}
			break;
		case ID_BORDERPOPUP_AUSRICHTUNG_UNTEN:
			pTextElement->m_nYAlignment = CTextElement::bottom;
			if (m_pLayout->m_bJoinBorderTexts)
			{
				pTextElementOther->m_nYAlignment = pTextElement->m_nYAlignment;
			}
			break;
		case ID_BORDERPOPUP_AUSRICHTUNG_ZENTRIERT:
			pTextElement->m_nYAlignment = CTextElement::center;
			if (m_pLayout->m_bJoinBorderTexts)
			{
				pTextElementOther->m_nYAlignment = pTextElement->m_nYAlignment;
			}
			break;
		case ID_BORDERPOPUP_ZEICHENSATZ:
			CreateFontDlg(&pTextElement->m_lfFont);
			if (m_pLayout->m_bJoinBorderTexts)
			{
				pTextElementOther->m_lfFont = pTextElement->m_lfFont;
			}

			break;
		case ID_BORDERPOPUP_SETZEFARBEN_TEXTFARBE:
			CreateColorDlg(&pTextElement->m_colText);
			if (m_pLayout->m_bJoinBorderTexts)
			{
				pTextElementOther->m_colText = pTextElement->m_colText;
			}
			break;
		
		case ID_BORDERPOPUP_SETZEFARBEN_HINTERGRUND:
		{
			if (m_pLayout->m_pActiveRect == &m_pLayout->m_rectLeft)
			{
				CreateColorDlg(&m_pLayout->m_colBorderLeftBack);
				if (m_pLayout->m_bJoinBorderTexts)
				{
					m_pLayout->m_colBorderRightBack = m_pLayout->m_colBorderLeftBack;
				}
			}
			else
			{
				CreateColorDlg(&m_pLayout->m_colBorderRightBack);
				if (m_pLayout->m_bJoinBorderTexts)
				{
					m_pLayout->m_colBorderLeftBack = m_pLayout->m_colBorderRightBack;
				}
			}
			break;
		}

		case ID_BORDERPOPUP_SETZEFARBEN_RAHMEN:
		{
			if (m_pLayout->m_pActiveRect == &m_pLayout->m_rectLeft)
			{
				CreateColorDlg(&m_pLayout->m_colBorderLeftRand);
				if (m_pLayout->m_bJoinBorderTexts)
				{
					m_pLayout->m_colBorderRightRand = m_pLayout->m_colBorderLeftRand;
				}
			}
			else
			{
				CreateColorDlg(&m_pLayout->m_colBorderRightRand);
				if (m_pLayout->m_bJoinBorderTexts)
				{
					m_pLayout->m_colBorderLeftRand = m_pLayout->m_colBorderRightRand;
				}
			}
			break;
		}

		case ID_BORDERPOPUP_SETZEFARBEN_TEXTHINTERGRUND:
			CreateColorDlg(&pTextElement->m_colTextbk);
			if (m_pLayout->m_bJoinBorderTexts)
			{
				pTextElementOther->m_colTextbk = pTextElement->m_colTextbk;
			}
			break;
		
		case ID_BORDERPOPUP_EDITTEXT:
		{
			// JUS 28.09.2002
			if (pTextElement->m_strText == "")
				pTextElement->m_strText = m_pLayout->m_pccd->Artist + " - " + m_pLayout->m_pccd->Title;
			CString str = pTextElement->m_strText;
			CCoverEnterStringDlg dlgString(&pTextElement->m_strText, this);
			if (dlgString.DoModal() != IDOK)
			{
				pTextElement->m_strText =  str;
			}
			if (m_pLayout->m_bJoinBorderTexts)
			{
				pTextElementOther->m_strText = pTextElement->m_strText;
			}
			
			break;
		}

		case ID_BORDERPOPUP_EQUALIZE:
			*pTextElementOther = *pTextElement;
			break;

		case ID_BORDERPOPUP_JOINTEXTS:
		{
			m_pLayout->ToggleBorderJoin();

			if (m_pLayout->m_bJoinBorderTexts)
			{
				if (m_pLayout->m_pActiveRect == &m_pLayout->m_rectLeft)
				{
					m_pLayout->m_bLeftBorder_AT = m_pLayout->m_bRightBorder_AT;
					m_pLayout->m_bRightBorder_Dominate = FALSE;
					m_pLayout->m_bLeftBorder_Dominate = TRUE;
				}
				else
				{
					m_pLayout->m_bRightBorder_AT = m_pLayout->m_bLeftBorder_AT;
					m_pLayout->m_bLeftBorder_Dominate = FALSE;
					m_pLayout->m_bRightBorder_Dominate = TRUE;
				}
			}
		}
	}

	MyInvalidateRect(&m_pLayout->m_rectLeft);
	MyInvalidateRect(&m_pLayout->m_rectRight);

	m_bRightButtonDown = FALSE;

	return;
}




void CCdcoverView::CreateFontDlg(LOGFONT *plf, const BOOL bShowSize)
{
	DWORD flags = CF_INITTOLOGFONTSTRUCT | CF_TTONLY | CF_SCREENFONTS;

	if (! bShowSize)
		flags |= CF_NOSIZESEL;

	CFontDialog dlgFont(plf, flags, NULL, this);
	if (dlgFont.DoModal() == IDOK)
		dlgFont.GetCurrentFont(plf);

	MyInvalidateRect();

	return;
}


void CCdcoverView::CreateColorDlg(COLORREF *pcol)
{
	CColorDialog dlgColor(*pcol, CC_FULLOPEN, this);
	if (dlgColor.DoModal() == IDOK)
		*pcol = dlgColor.GetColor();

	MyInvalidateRect();

	return;
}


BOOL CCdcoverView::OnNotify(WPARAM wParam, LPARAM lParam, LRESULT* pResult) 
{
	
	HD_NOTIFY *Msg = (HD_NOTIFY*)lParam;

	// Nur Messages vom HeaderCtrl abfangen und bearbeiten

	if (Msg->hdr.idFrom == ID_COVERHEADERCTRL)
	{
		switch (Msg->hdr.code)
		{
			case HDN_ITEMCLICK:
			{
				m_nCurColumn = Msg->iItem;

				CTextElement *pTextElement = &m_pLayout->m_vecColumns[m_nCurColumn].m_teColumn;

				CPoint pt;
				::GetCursorPos(&pt);
	
				CMenu menu;
				menu.LoadMenu(IDR_MENU_BACKPOPUP);
				CMenu* pSubMenu = menu.GetSubMenu(0);

				UINT nCheckedTransparent = ((pTextElement->m_nBkMode == TRANSPARENT) ? MF_CHECKED : MF_UNCHECKED);

				int nAlign = ID_BACKPOPUP_AUSRICHTUNG_ZENTRIERT;

				switch (pTextElement->m_nXAlignment)
				{
					case CTextElement::left:
						nAlign = ID_BACKPOPUP_AUSRICHTUNG_LINKS;
						break;
					case CTextElement::right:
						nAlign = ID_BACKPOPUP_AUSRICHTUNG_RECHTS;
						break;
					case CTextElement::hook:
						nAlign = ID_BACKPOPUP_AUSRICHTUNG_HOOK;
				}


				int nType1 = ID_BACKPOPUP_SETTYPE_SEPARATOR;
				int nType2 = ID_BACKPOPUP_NEWCOLUMN_SEPARATOR;
					
				switch (m_pLayout->m_vecColumns[m_nCurColumn].m_nType)
				{
					case CCoverColumn::FREI:
						nType1 = ID_BACKPOPUP_SETTYPE_FREI;
						nType2 = ID_BACKPOPUP_NEWCOLUMN_FREI;
						break;
					case CCoverColumn::TRACKNR:
						nType1 = ID_BACKPOPUP_SETTYPE_TRACKNUMMER;
						nType2 = ID_BACKPOPUP_NEWCOLUMN_TRACKNUMMER;
						break;
					case CCoverColumn::ARTIST:
						nType1 = ID_BACKPOPUP_SETTYPE_INTERPRET;
						nType2 = ID_BACKPOPUP_NEWCOLUMN_INTERPRET;
						break;
					case CCoverColumn::TITLE:
						nType1 = ID_BACKPOPUP_SETTYPE_SONGNAME;
						nType2 = ID_BACKPOPUP_NEWCOLUMN_SONGNAME;
						break;
					case CCoverColumn::LENGTH:
						nType1 = ID_BACKPOPUP_SETTYPE_LAENGE;
						nType2 = ID_BACKPOPUP_NEWCOLUMN_LAENGE;
						break;
					case CCoverColumn::BPM:
						nType1 = ID_BACKPOPUP_SETTYPE_BPM;
						nType2 = ID_BACKPOPUP_NEWCOLUMN_BPM;
						break;
					case CCoverColumn::COMMENT:
						nType1 = ID_BACKPOPUP_SETTYPE_KOMMENTAR;
						nType2 = ID_BACKPOPUP_NEWCOLUMN_KOMMENTAR;
						break;
					case CCoverColumn::USER1:
						nType1 = ID_BACKPOPUP_SETTYPE_USER1;
						nType2 = ID_BACKPOPUP_NEWCOLUMN_USER1;
						break;
					case CCoverColumn::USER2:
						nType1 = ID_BACKPOPUP_SETTYPE_USER2;
						nType2 = ID_BACKPOPUP_NEWCOLUMN_USER2;
						break;
					case CCoverColumn::USER3:
						nType1 = ID_BACKPOPUP_SETTYPE_USER3;
						nType2 = ID_BACKPOPUP_NEWCOLUMN_USER3;
						break;
					case CCoverColumn::USER4:
						nType1 = ID_BACKPOPUP_SETTYPE_USER4;
						nType2 = ID_BACKPOPUP_NEWCOLUMN_USER4;
						break;
					case CCoverColumn::USER5:
						nType1 = ID_BACKPOPUP_SETTYPE_USER5;
						nType2 = ID_BACKPOPUP_NEWCOLUMN_USER5;
						break;
					case CCoverColumn::CODES:
						nType1 = ID_BACKPOPUP_SETTYPE_KENNZEICHEN;
						nType2 = ID_BACKPOPUP_NEWCOLUMN_KENNZEICHEN;
						break;
				}


				CString str = m_pLayout->m_pdb->Master->UserTrackFields[0]->Name;
				if (str == "")
				{
					str.LoadString(IDS_COVER_FIELD_MENUUSER1);
				}
				pSubMenu->ModifyMenu(ID_BACKPOPUP_SETTYPE_USER1, MF_BYCOMMAND | MF_ENABLED | MF_STRING, ID_BACKPOPUP_SETTYPE_USER1, LPCTSTR(str));
				pSubMenu->ModifyMenu(ID_BACKPOPUP_NEWCOLUMN_USER1, MF_BYCOMMAND | MF_ENABLED | MF_STRING, ID_BACKPOPUP_NEWCOLUMN_USER1, LPCTSTR(str));

				str = m_pLayout->m_pdb->Master->UserTrackFields[1]->Name;
				if (str == "")
				{
					str.LoadString(IDS_COVER_FIELD_MENUUSER2);
				}
				pSubMenu->ModifyMenu(ID_BACKPOPUP_SETTYPE_USER2, MF_BYCOMMAND | MF_ENABLED | MF_STRING, ID_BACKPOPUP_SETTYPE_USER2, LPCTSTR(str));
				pSubMenu->ModifyMenu(ID_BACKPOPUP_NEWCOLUMN_USER2, MF_BYCOMMAND | MF_ENABLED | MF_STRING, ID_BACKPOPUP_NEWCOLUMN_USER2, LPCTSTR(str));

				str = m_pLayout->m_pdb->Master->UserTrackFields[2]->Name;
				if (str == "")
				{
					str.LoadString(IDS_COVER_FIELD_MENUUSER3);
				}
				pSubMenu->ModifyMenu(ID_BACKPOPUP_SETTYPE_USER3, MF_BYCOMMAND | MF_ENABLED | MF_STRING, ID_BACKPOPUP_SETTYPE_USER3, LPCTSTR(str));
				pSubMenu->ModifyMenu(ID_BACKPOPUP_NEWCOLUMN_USER3, MF_BYCOMMAND | MF_ENABLED | MF_STRING, ID_BACKPOPUP_NEWCOLUMN_USER3, LPCTSTR(str));

				str = m_pLayout->m_pdb->Master->UserTrackFields[3]->Name;
				if (str == "")
				{
					str.LoadString(IDS_COVER_FIELD_MENUUSER4);
				}
				pSubMenu->ModifyMenu(ID_BACKPOPUP_SETTYPE_USER4, MF_BYCOMMAND | MF_ENABLED | MF_STRING, ID_BACKPOPUP_SETTYPE_USER4, LPCTSTR(str));
				pSubMenu->ModifyMenu(ID_BACKPOPUP_NEWCOLUMN_USER4, MF_BYCOMMAND | MF_ENABLED | MF_STRING, ID_BACKPOPUP_NEWCOLUMN_USER4, LPCTSTR(str));

				str = m_pLayout->m_pdb->Master->UserTrackFields[4]->Name;
				if (str == "")
				{
					str.LoadString(IDS_COVER_FIELD_MENUUSER5);
				}
				pSubMenu->ModifyMenu(ID_BACKPOPUP_SETTYPE_USER5, MF_BYCOMMAND | MF_ENABLED | MF_STRING, ID_BACKPOPUP_SETTYPE_USER5, LPCTSTR(str));
				pSubMenu->ModifyMenu(ID_BACKPOPUP_NEWCOLUMN_USER5, MF_BYCOMMAND | MF_ENABLED | MF_STRING, ID_BACKPOPUP_NEWCOLUMN_USER5, LPCTSTR(str));




				pSubMenu->CheckMenuItem(nType1, MF_BYCOMMAND | MF_CHECKED);
				pSubMenu->CheckMenuItem(nType2, MF_BYCOMMAND | MF_CHECKED);
				pSubMenu->CheckMenuItem(nAlign, MF_BYCOMMAND | MF_CHECKED);

				pSubMenu->CheckMenuItem(ID_BACKPOPUP_TEXTTRANSPARENZ, MF_BYCOMMAND | nCheckedTransparent);
	
				pSubMenu->CheckMenuItem(ID_BACKPOPUP_JOINCOLUMNS, MF_BYCOMMAND | (m_pLayout->m_bJoinColumns ? MF_CHECKED : MF_UNCHECKED));


				if (m_pLayout->m_vecColumns.size() == 1)
				{
					pSubMenu->EnableMenuItem(ID_BACKPOPUP_DELCOLUMN, MF_BYCOMMAND | MF_GRAYED);
				}
				if (m_nCurColumn == 0)
				{
					pSubMenu->EnableMenuItem(ID_BACKPOPUP_AUSRICHTUNG_HOOK, MF_BYCOMMAND | MF_GRAYED);
				}
				if (m_pLayout->m_vecColumns.size() == 10)
				{
					pSubMenu->EnableMenuItem(1, MF_BYPOSITION | MF_GRAYED);
				}

				
				pSubMenu->TrackPopupMenu(TPM_LEFTALIGN | TPM_RIGHTBUTTON, pt.x, pt.y, this);

				MyInvalidateRect(&m_pLayout->m_rectBack);
				break;
			}

				// sollte sich etwas beim HeaderCtrl geändert haben
				// werden die Breiten upgedatet

			case HDN_ITEMCHANGED:
				InitColumnWidths();
				MyInvalidateRect(&m_pLayout->m_rectBack);
		}
	}


	return CScrollView::OnNotify(wParam, lParam, pResult);
}


void CCdcoverView::InitColumnWidths()
{
	HD_ITEM MyItem;

	MyItem.mask = HDI_WIDTH;

	for (int i = 0; i != (int)m_pLayout->m_vecColumns.size(); i++)
	{
		m_header.GetItem(i, &MyItem);
		m_pLayout->m_vecColumns[i].m_dWidthRel = (double) MyItem.cxy / (double) m_rectHeader.Width();
	}

	return;
}



void CCdcoverView::OnBackpopup() 
{
	const MSG *msg = GetCurrentMessage();
	const int MenuID = msg->wParam;

	CTextElement *pTextElement = &m_pLayout->m_vecColumns[m_nCurColumn].m_teColumn;
	int nColumns = m_pLayout->m_vecColumns.size();

	switch (MenuID)
	{

		case ID_BACKPOPUP_DELCOLUMN:
		{
			std::vector<CCoverColumn>::iterator Pos = m_pLayout->m_vecColumns.begin();
			Pos += m_nCurColumn;
			m_pLayout->m_vecColumns.erase(Pos);
			m_bFirstDraw = TRUE;
			MyInvalidateRect(&m_pLayout->m_rectBack);
		}

		break;

		case ID_BACKPOPUP_NEWCOLUMN_FREI:
		{
			CString s;
			CCoverEnterStringDlg dlgString(&s, this);

			if (dlgString.DoModal() == IDOK)
			{
				AddColumnFromMenuID(MenuID, s);
			}
			break;
		}
		
		case ID_BACKPOPUP_NEWCOLUMN_TRACKNUMMER:
		case ID_BACKPOPUP_NEWCOLUMN_INTERPRET:
		case ID_BACKPOPUP_NEWCOLUMN_SONGNAME:
		case ID_BACKPOPUP_NEWCOLUMN_LAENGE:
		case ID_BACKPOPUP_NEWCOLUMN_BPM:
		case ID_BACKPOPUP_NEWCOLUMN_KOMMENTAR:
		case ID_BACKPOPUP_NEWCOLUMN_USER1:
		case ID_BACKPOPUP_NEWCOLUMN_USER2:
		case ID_BACKPOPUP_NEWCOLUMN_USER3:
		case ID_BACKPOPUP_NEWCOLUMN_USER4:
		case ID_BACKPOPUP_NEWCOLUMN_USER5:
		case ID_BACKPOPUP_NEWCOLUMN_SEPARATOR:
		case ID_BACKPOPUP_NEWCOLUMN_KENNZEICHEN:

			AddColumnFromMenuID(MenuID);
			break;

		case ID_BACKPOPUP_SETTYPE_FREI:
		{
			CString s;
			CCoverEnterStringDlg dlgString(&s, this);

			if (dlgString.DoModal() == IDOK)
			{
				SetColumnTypeFromMenuID(MenuID, s);
			}
			break;
		}
		
		case ID_BACKPOPUP_SETTYPE_TRACKNUMMER:
		case ID_BACKPOPUP_SETTYPE_INTERPRET:
		case ID_BACKPOPUP_SETTYPE_SONGNAME:
		case ID_BACKPOPUP_SETTYPE_LAENGE:
		case ID_BACKPOPUP_SETTYPE_BPM:
		case ID_BACKPOPUP_SETTYPE_KOMMENTAR:
		case ID_BACKPOPUP_SETTYPE_USER1:
		case ID_BACKPOPUP_SETTYPE_USER2:
		case ID_BACKPOPUP_SETTYPE_USER3:
		case ID_BACKPOPUP_SETTYPE_USER4:
		case ID_BACKPOPUP_SETTYPE_USER5:
		case ID_BACKPOPUP_SETTYPE_SEPARATOR:
		case ID_BACKPOPUP_SETTYPE_KENNZEICHEN:

			SetColumnTypeFromMenuID(MenuID);
			break;

		case ID_BACKPOPUP_TEXTTRANSPARENZ:
			pTextElement->m_nBkMode = (pTextElement->m_nBkMode == OPAQUE) ? TRANSPARENT : OPAQUE;
			if (m_pLayout->m_bJoinColumns)
			{
				for (int i = 0; i != nColumns; i++)
				{
					m_pLayout->m_vecColumns[i].m_teColumn.m_nBkMode = pTextElement->m_nBkMode;
				}
			}
			break;

		case ID_BACKPOPUP_AUSRICHTUNG_LINKS:
		case ID_BACKPOPUP_AUSRICHTUNG_RECHTS:
		case ID_BACKPOPUP_AUSRICHTUNG_ZENTRIERT:
		case ID_BACKPOPUP_AUSRICHTUNG_HOOK:
			SetXAlignmentFromMenuID(MenuID);
			break;

		case ID_BACKPOPUP_ZEICHENSATZ:
			CreateFontDlg(&pTextElement->m_lfFont, FALSE);
			if (m_pLayout->m_bJoinColumns)
			{
				for (int i = 0; i != nColumns; i++)
				{
					m_pLayout->m_vecColumns[i].m_teColumn.m_lfFont = pTextElement->m_lfFont;
				}
			}

			break;
		case ID_BACKPOPUP_SETZEFARBEN_TEXTFARBE:
			CreateColorDlg(&pTextElement->m_colText);
			if (m_pLayout->m_bJoinColumns)
			{
				for (int i = 0; i != nColumns; i++)
				{
					m_pLayout->m_vecColumns[i].m_teColumn.m_colText = pTextElement->m_colText;
				}
			}
			break;
		case ID_BACKPOPUP_SETZEFARBEN_TEXTHINTERGRUND:
			CreateColorDlg(&pTextElement->m_colTextbk);
			if (m_pLayout->m_bJoinColumns)
			{
				for (int i = 0; i != nColumns; i++)
				{
					m_pLayout->m_vecColumns[i].m_teColumn.m_colTextbk = pTextElement->m_colTextbk;
				}
			}
			break;
		
		case ID_BACKPOPUP_JOINCOLUMNS:
			m_pLayout->ToggleColumnsJoin();
	}

	m_bRightButtonDown = FALSE;

	return;
}


void CCdcoverView::SetColumnTypeFromMenuID(const int nMenuID, const CString& strColumnText)
{
	int nType = CCoverColumn::TRACKNR;

	switch (nMenuID)
	{
		case ID_BACKPOPUP_SETTYPE_INTERPRET:
			nType = CCoverColumn::ARTIST;
			break;
		case ID_BACKPOPUP_SETTYPE_SONGNAME:
			nType = CCoverColumn::TITLE;
			break;
		case ID_BACKPOPUP_SETTYPE_LAENGE:
			nType = CCoverColumn::LENGTH;
			break;
		case ID_BACKPOPUP_SETTYPE_BPM:
			nType = CCoverColumn::BPM;
			break;
		case ID_BACKPOPUP_SETTYPE_KOMMENTAR:
			nType = CCoverColumn::COMMENT;
			break;
		case ID_BACKPOPUP_SETTYPE_USER1:
			nType = CCoverColumn::USER1;
			break;
		case ID_BACKPOPUP_SETTYPE_USER2:
			nType = CCoverColumn::USER2;
			break;
		case ID_BACKPOPUP_SETTYPE_USER3:
			nType = CCoverColumn::USER3;
			break;
		case ID_BACKPOPUP_SETTYPE_USER4:
			nType = CCoverColumn::USER4;
			break;
		case ID_BACKPOPUP_SETTYPE_USER5:
			nType = CCoverColumn::USER5;
			break;
		case ID_BACKPOPUP_SETTYPE_SEPARATOR:
			nType = CCoverColumn::SEPARATOR;
			break;
		case ID_BACKPOPUP_SETTYPE_KENNZEICHEN:
			nType = CCoverColumn::CODES;
			break;
		case ID_BACKPOPUP_SETTYPE_FREI:
			nType = CCoverColumn::FREI;
	}

	m_pLayout->m_vecColumns[m_nCurColumn].m_nType = nType;
	m_pLayout->m_vecColumns[m_nCurColumn].m_teColumn.m_strText = strColumnText;
	m_bFirstDraw = TRUE;
	MyInvalidateRect(&m_pLayout->m_rectBack);

	return;
}



void CCdcoverView::SetXAlignmentFromMenuID(const int nMenuID)
{
	int nAlignment = CTextElement::center;

	switch (nMenuID)
	{
		case ID_BACKPOPUP_AUSRICHTUNG_LINKS:
			nAlignment = CTextElement::left;
			break;
		case ID_BACKPOPUP_AUSRICHTUNG_RECHTS:
			nAlignment = CTextElement::right;
			break;
		case ID_BACKPOPUP_AUSRICHTUNG_HOOK:
			nAlignment = CTextElement::hook;
	}
	
	m_pLayout->m_vecColumns[m_nCurColumn].m_teColumn.m_nXAlignment = nAlignment;

	if (m_pLayout->m_bJoinColumns)
	{
		for (int i = 0; i != (int)m_pLayout->m_vecColumns.size(); i++)
		{
			m_pLayout->m_vecColumns[i].m_teColumn.m_nXAlignment = nAlignment;
		}
	}

	return;
}


void CCdcoverView::BuildHeader(CRect rectBack)
{
	m_rectHeader = rectBack;

	m_rectHeader.top = - 21 + m_rectHeader.top;
	m_rectHeader.bottom = 20 + m_rectHeader.top;

	m_header.DestroyWindow();
	m_header.Create(HDS_BUTTONS | HDS_HORZ | WS_VISIBLE | WS_BORDER, m_rectHeader, this, ID_COVERHEADERCTRL);
	m_header.SetFont(GetFont());

	m_Font.CreateStockObject(ANSI_VAR_FONT);
	m_header.SetFont(&m_Font);


	for (int i = 0; i != (int)m_pLayout->m_vecColumns.size(); i++)
	{
		HDITEM item;
		CTextElement *pTE = &m_pLayout->m_vecColumns[i].m_teColumn;

		item.mask = HDI_FORMAT | HDI_TEXT | HDI_WIDTH;
		item.cxy = (int) ((m_pLayout->m_vecColumns[i].m_dWidthRel * (double) m_rectHeader.Width()) + 0.5);

		CString str;
		str.LoadString(IDS_COVER_FIELD_SEPARATOR);

		switch (m_pLayout->m_vecColumns[i].m_nType)
		{
			case CCoverColumn::FREI:
				str = pTE->m_strText;
				break;
			case CCoverColumn::TRACKNR:
				str.LoadString(IDS_COVER_FIELD_TRACKNR);
				break;
			case CCoverColumn::ARTIST:
				str.LoadString(IDS_COVER_FIELD_ARTIST);
				break;
			case CCoverColumn::TITLE:
				str.LoadString(IDS_COVER_FIELD_SONGNAME);
				break;
			case CCoverColumn::LENGTH:
				str.LoadString(IDS_COVER_FIELD_LENGTH);
				break;
			case CCoverColumn::BPM:
				str.LoadString(IDS_COVER_FIELD_BPM);
				break;
			case CCoverColumn::COMMENT:
				str.LoadString(IDS_COVER_FIELD_COMMENT);
				break;
			case CCoverColumn::USER1:
				str = m_pLayout->m_pdb->Master->UserTrackFields[0]->Name;
				if (str == "")
				{
					str.LoadString(IDS_COVER_FIELD_USER1);
				}
				break;
			case CCoverColumn::USER2:
				str = m_pLayout->m_pdb->Master->UserTrackFields[1]->Name;
				if (str == "")
				{
					str.LoadString(IDS_COVER_FIELD_USER2);
				}
				break;
			case CCoverColumn::USER3:
				str = m_pLayout->m_pdb->Master->UserTrackFields[2]->Name;
				if (str == "")
				{
					str.LoadString(IDS_COVER_FIELD_USER3);
				}
				break;
			case CCoverColumn::USER4:
				str = m_pLayout->m_pdb->Master->UserTrackFields[3]->Name;
				if (str == "")
				{
					str.LoadString(IDS_COVER_FIELD_USER4);
				}
				break;
			case CCoverColumn::USER5:
				str = m_pLayout->m_pdb->Master->UserTrackFields[4]->Name;
				if (str == "")
				{
					str.LoadString(IDS_COVER_FIELD_USER5);
				}
				break;
			case CCoverColumn::CODES:
				str.LoadString(IDS_COVER_FIELD_KENNZEICHEN);
		}

		item.pszText = (LPTSTR)LPCTSTR(str);
		item.cchTextMax = wcslen(item.pszText);
		item.fmt = HDF_LEFT | HDF_STRING;

		m_header.InsertItem(i, &item);
	}

	return;
}


void CCdcoverView::AddColumnFromMenuID(const int nMenuID, const CString& strColumnText)
{
	CCoverColumn NewColumn;

	int nType = CCoverColumn::TRACKNR;

	switch (nMenuID)
	{
		case ID_BACKPOPUP_NEWCOLUMN_INTERPRET:
			nType = CCoverColumn::ARTIST;
			break;
		case ID_BACKPOPUP_NEWCOLUMN_SONGNAME:
			nType = CCoverColumn::TITLE;
			break;
		case ID_BACKPOPUP_NEWCOLUMN_LAENGE:
			nType = CCoverColumn::LENGTH;
			break;
		case ID_BACKPOPUP_NEWCOLUMN_BPM:
			nType = CCoverColumn::BPM;
			break;
		case ID_BACKPOPUP_NEWCOLUMN_KOMMENTAR:
			nType = CCoverColumn::COMMENT;
			break;
		case ID_BACKPOPUP_NEWCOLUMN_USER1:
			nType = CCoverColumn::USER1;
			break;
		case ID_BACKPOPUP_NEWCOLUMN_USER2:
			nType = CCoverColumn::USER2;
			break;
		case ID_BACKPOPUP_NEWCOLUMN_USER3:
			nType = CCoverColumn::USER3;
			break;
		case ID_BACKPOPUP_NEWCOLUMN_USER4:
			nType = CCoverColumn::USER4;
			break;
		case ID_BACKPOPUP_NEWCOLUMN_USER5:
			nType = CCoverColumn::USER5;
			break;
		case ID_BACKPOPUP_NEWCOLUMN_SEPARATOR:
			nType = CCoverColumn::SEPARATOR;
			break;
		case ID_BACKPOPUP_NEWCOLUMN_KENNZEICHEN:
			nType = CCoverColumn::CODES;
			break;
		case ID_BACKPOPUP_NEWCOLUMN_FREI:
			nType = CCoverColumn::FREI;
	}

	NewColumn.m_nType = nType;
	NewColumn.m_dWidthRel = 5.0 / 14.0;

	NewColumn.m_teColumn.m_strText = strColumnText;

	std::vector<CCoverColumn>::iterator Pos = m_pLayout->m_vecColumns.begin();
	Pos += m_nCurColumn+1;
	m_pLayout->m_vecColumns.insert(Pos, NewColumn);
	m_bFirstDraw = TRUE;
	MyInvalidateRect(&m_pLayout->m_rectBack);

	return;
}



void CCdcoverView::WorkOnBack()
{
	CPoint pt;
	::GetCursorPos(&pt);
	
	CMenu menu;
	menu.LoadMenu(IDR_MENU_BACKMAIN);
	CMenu* pSubMenu = menu.GetSubMenu(0);

	int nYAlign = ID_BACKOPTIONS_YAUSRICHTUNG_ZENTRIERT;
	switch (m_pLayout->m_nBackYAlignment)
	{
		case CTextElement::top:
			nYAlign = ID_BACKOPTIONS_YAUSRICHTUNG_OBEN;
			break;
		case CTextElement::bottom:
			nYAlign = ID_BACKOPTIONS_YAUSRICHTUNG_UNTEN;
	}
	int nXAlign = ID_BACKOPTIONS_XAUSRICHTUNG_ZENTRIERT;
	switch (m_pLayout->m_nBackXAlignment)
	{
		case CTextElement::left:
			nXAlign = ID_BACKOPTIONS_XAUSRICHTUNG_LINKS;
			break;
		case CTextElement::right:
			nXAlign = ID_BACKOPTIONS_XAUSRICHTUNG_RECHTS;
	}

	pSubMenu->CheckMenuItem(nYAlign, MF_BYCOMMAND | MF_CHECKED);
	pSubMenu->CheckMenuItem(nXAlign, MF_BYCOMMAND | MF_CHECKED);

	pSubMenu->CheckMenuItem(m_pLayout->m_bGfxBackBorder ? ID_BACKOPTIONS_GRAFIKPOSITION_BACKBORDER : ID_BACKOPTIONS_GRAFIKPOSITION_BACK, MF_BYCOMMAND | MF_CHECKED);

	if (m_pLayout->m_bTextGfxBack)
	{
		pSubMenu->CheckMenuItem(ID_BACKMAIN_GRAFIKPOSITION_TEXTGFX, MF_BYCOMMAND | MF_CHECKED);
	}
	
	if (! m_pLayout->m_bBackGfx)
	{
		pSubMenu->EnableMenuItem(ID_BACKOPTIONS_DELGRAFIK, MF_BYCOMMAND | MF_GRAYED);
	}


	CString str = m_pLayout->m_pdb->Master->UserTrackFields[0]->Name;
	if (str == "")
	{
		str.LoadString(IDS_COVER_FIELD_USER1);
	}
	pSubMenu->ModifyMenu(ID_BACKPOPUP_SETTYPE_USER1, MF_BYCOMMAND | MF_ENABLED | MF_STRING, ID_BACKPOPUP_SETTYPE_USER1, LPCTSTR(str));
	pSubMenu->ModifyMenu(ID_BACKPOPUP_NEWCOLUMN_USER1, MF_BYCOMMAND | MF_ENABLED | MF_STRING, ID_BACKPOPUP_NEWCOLUMN_USER1, LPCTSTR(str));

	str = m_pLayout->m_pdb->Master->UserTrackFields[1]->Name;
	if (str == "")
	{
		str.LoadString(IDS_COVER_FIELD_USER2);
	}
	pSubMenu->ModifyMenu(ID_BACKPOPUP_SETTYPE_USER2, MF_BYCOMMAND | MF_ENABLED | MF_STRING, ID_BACKPOPUP_SETTYPE_USER2, LPCTSTR(str));
	pSubMenu->ModifyMenu(ID_BACKPOPUP_NEWCOLUMN_USER2, MF_BYCOMMAND | MF_ENABLED | MF_STRING, ID_BACKPOPUP_NEWCOLUMN_USER2, LPCTSTR(str));

	str = m_pLayout->m_pdb->Master->UserTrackFields[2]->Name;
	if (str == "")
	{
		str.LoadString(IDS_COVER_FIELD_USER3);
	}
	pSubMenu->ModifyMenu(ID_BACKPOPUP_SETTYPE_USER3, MF_BYCOMMAND | MF_ENABLED | MF_STRING, ID_BACKPOPUP_SETTYPE_USER3, LPCTSTR(str));
	pSubMenu->ModifyMenu(ID_BACKPOPUP_NEWCOLUMN_USER3, MF_BYCOMMAND | MF_ENABLED | MF_STRING, ID_BACKPOPUP_NEWCOLUMN_USER3, LPCTSTR(str));

	str = m_pLayout->m_pdb->Master->UserTrackFields[3]->Name;
	if (str == "")
	{
		str.LoadString(IDS_COVER_FIELD_USER4);
	}
	pSubMenu->ModifyMenu(ID_BACKPOPUP_SETTYPE_USER4, MF_BYCOMMAND | MF_ENABLED | MF_STRING, ID_BACKPOPUP_SETTYPE_USER4, LPCTSTR(str));
	pSubMenu->ModifyMenu(ID_BACKPOPUP_NEWCOLUMN_USER4, MF_BYCOMMAND | MF_ENABLED | MF_STRING, ID_BACKPOPUP_NEWCOLUMN_USER4, LPCTSTR(str));

	str = m_pLayout->m_pdb->Master->UserTrackFields[4]->Name;
	if (str == "")
	{
		str.LoadString(IDS_COVER_FIELD_USER5);
	}
	pSubMenu->ModifyMenu(ID_BACKPOPUP_SETTYPE_USER5, MF_BYCOMMAND | MF_ENABLED | MF_STRING, ID_BACKPOPUP_SETTYPE_USER5, LPCTSTR(str));
	pSubMenu->ModifyMenu(ID_BACKPOPUP_NEWCOLUMN_USER5, MF_BYCOMMAND | MF_ENABLED | MF_STRING, ID_BACKPOPUP_NEWCOLUMN_USER5, LPCTSTR(str));


	pSubMenu->TrackPopupMenu(TPM_LEFTALIGN | TPM_RIGHTBUTTON, pt.x, pt.y, this);

	MyInvalidateRect(&m_pLayout->m_rectBack);
	MyInvalidateRect(&m_pLayout->m_rectLeft);
	MyInvalidateRect(&m_pLayout->m_rectRight);

	return;
}

void CCdcoverView::OnBackMainPopup() 
{
	const MSG *msg = GetCurrentMessage();
	const int MenuID = msg->wParam;

	switch (MenuID)
	{
		case ID_BACKOPTIONS_XAUSRICHTUNG_LINKS:
			m_pLayout->m_nBackXAlignment = CTextElement::left;
			break;
		case ID_BACKOPTIONS_XAUSRICHTUNG_RECHTS:
			m_pLayout->m_nBackXAlignment = CTextElement::right;
			break;
		case ID_BACKOPTIONS_XAUSRICHTUNG_ZENTRIERT:
			m_pLayout->m_nBackXAlignment = CTextElement::center;
			break;
		case ID_BACKOPTIONS_YAUSRICHTUNG_OBEN:
			m_pLayout->m_nBackYAlignment = CTextElement::top;
			break;
		case ID_BACKOPTIONS_YAUSRICHTUNG_UNTEN:
			m_pLayout->m_nBackYAlignment = CTextElement::bottom;
			break;
		case ID_BACKOPTIONS_YAUSRICHTUNG_ZENTRIERT:
			m_pLayout->m_nBackYAlignment = CTextElement::center;
			break;
		case ID_BACKOPTIONS_ADDGRAFIK:
			m_pLayout->AddGfxBack(this);
			break;
		case ID_BACKOPTIONS_DELGRAFIK:
			m_pLayout->DelGfxBack();
			break;
		case ID_BACKOPTIONS_GRAFIKPOSITION_BACKBORDER:
			m_pLayout->m_bGfxBackBorder = TRUE;
			break;
		case ID_BACKOPTIONS_GRAFIKPOSITION_BACK:
			m_pLayout->m_bGfxBackBorder = FALSE;
			break;
		case ID_BACKMAIN_GRAFIKPOSITION_TEXTGFX:
			m_pLayout->ToggleTextGfxBack();
			break;
		case ID_BACKMAIN_SETZEFARBEN_HINTERGRUND:
			CreateColorDlg(&m_pLayout->m_colBack);
			break;
		case ID_BACKMAIN_SETZEFARBEN_RAHMEN:
			CreateColorDlg(&m_pLayout->m_colBackRand);
	}

	m_bRightButtonDown = FALSE;

	MyInvalidateRect();

	return;
}

void CCdcoverView::WorkOnFront()
{
	CPoint pt;
	::GetCursorPos(&pt);
	
	CMenu menu;
	menu.LoadMenu(IDR_MENU_FRONTMAIN);
	CMenu* pSubMenu = menu.GetSubMenu(0);

	if (m_pLayout->m_bTextGfxFront)
		pSubMenu->CheckMenuItem(ID_FRONTMAIN_TEXTGFX, MF_BYCOMMAND | MF_CHECKED);
	
	if (! m_pLayout->m_bFrontGfx)
		pSubMenu->EnableMenuItem(ID_FRONTOPTIONS_DELGRAFIK, MF_BYCOMMAND | MF_GRAYED);

	if (m_pLayout->m_bShowFrontArtist)
		pSubMenu->CheckMenuItem(ID_FRONTMAIN_ARTIST, MF_BYCOMMAND | MF_CHECKED);

	if (m_pLayout->m_bShowFrontCDTitle)
		pSubMenu->CheckMenuItem(ID_FRONTMAIN_CDTITEL, MF_BYCOMMAND | MF_CHECKED);

	pSubMenu->TrackPopupMenu(TPM_LEFTALIGN | TPM_RIGHTBUTTON, pt.x, pt.y, this);

	MyInvalidateRect(&m_pLayout->m_rectFront);

	return;
}



void CCdcoverView::OnFrontMainPopup() 
{
	const MSG *msg = GetCurrentMessage();
	const int MenuID = msg->wParam;

	switch (MenuID)
	{
		case ID_FRONTOPTIONS_ADDGRAFIK:
			m_pLayout->AddGfxFront(this);
			break;
		case ID_FRONTOPTIONS_DELGRAFIK:
			m_pLayout->DelGfxFront();
			break;
		case ID_FRONTMAIN_TEXTGFX:
			m_pLayout->ToggleTextGfxFront();
			break;
		case ID_FRONTMAIN_ARTIST:
			m_pLayout->m_bShowFrontArtist = ! m_pLayout->m_bShowFrontArtist;
			break;
		case ID_FRONTMAIN_CDTITEL:
			m_pLayout->m_bShowFrontCDTitle = ! m_pLayout->m_bShowFrontCDTitle;
			break;
		case ID_FRONTMAIN_SETZEFARBEN_HINTERGRUND:
			CreateColorDlg(&m_pLayout->m_colFront);
			break;
		case ID_FRONTMAIN_SETZEFARBEN_RAHMEN:
			CreateColorDlg(&m_pLayout->m_colFrontRand);
			break;
	}

	m_bRightButtonDown = FALSE;
	
	MyInvalidateRect();

	return;
}


void CCdcoverView::WorkOnArtistTitle()
{
	CTextElement *pTextElement = &m_pLayout->m_teArtist;

	if (m_pLayout->m_pActiveRect == &m_pLayout->m_rectTitle)
	{
		pTextElement = &m_pLayout->m_teCDTitle;
	}

	CPoint pt;
	::GetCursorPos(&pt);
	
	CMenu menu;
	menu.LoadMenu(IDR_MENU_FRONTPOPUP);
	CMenu* pSubMenu = menu.GetSubMenu(0);

	UINT nCheckedTransparent = ((pTextElement->m_nBkMode == TRANSPARENT) ? MF_CHECKED : MF_UNCHECKED);

	int nAlign = ID_FRONTPOPUP_XAUSRICHTUNG_ZENTRIERT;

	switch (pTextElement->m_nXAlignment)
	{
		case CTextElement::left:
			nAlign = ID_FRONTPOPUP_XAUSRICHTUNG_LINKS;
			break;
		case CTextElement::right:
			nAlign = ID_FRONTPOPUP_XAUSRICHTUNG_RECHTS;
			break;
		case CTextElement::userdef:
			nAlign = ID_FRONTPOPUP_XAUSRICHTUNG_FREI;
			break;
	}

	int nYAlign = ID_FRONTPOPUP_YAUSRICHTUNG_ZENTRIERT;
	switch (pTextElement->m_nYAlignment)
	{
		case CTextElement::top:
			nYAlign = ID_FRONTPOPUP_YAUSRICHTUNG_OBEN;
			break;
		case CTextElement::bottom:
			nYAlign = ID_FRONTPOPUP_YAUSRICHTUNG_UNTEN;
			break;
		case CTextElement::userdef:
			nYAlign = ID_FRONTPOPUP_YAUSRICHTUNG_FREI;
			break;
	}


	pSubMenu->CheckMenuItem(nAlign, MF_BYCOMMAND | MF_CHECKED);
	pSubMenu->CheckMenuItem(nYAlign, MF_BYCOMMAND | MF_CHECKED);
	pSubMenu->CheckMenuItem(ID_FRONTPOPUP_TEXTTRANSPARENZ, MF_BYCOMMAND | nCheckedTransparent);
	
	pSubMenu->TrackPopupMenu(TPM_LEFTALIGN | TPM_RIGHTBUTTON, pt.x, pt.y, this);

	MyInvalidateRect(&m_pLayout->m_rectFront);

	return;
}



void CCdcoverView::OnFrontPopup()
{
	const MSG *msg = GetCurrentMessage();
	const int MenuID = msg->wParam;

	CTextElement *pTextElement = &m_pLayout->m_teArtist;

	if (m_pLayout->m_pActiveRect == &m_pLayout->m_rectTitle)
	{
		pTextElement = &m_pLayout->m_teCDTitle;
	}

	switch (MenuID)
	{
		case ID_FRONTPOPUP_TEXTTRANSPARENZ:
			pTextElement->m_nBkMode = (pTextElement->m_nBkMode == OPAQUE) ? TRANSPARENT : OPAQUE;
			break;
		case ID_FRONTPOPUP_YAUSRICHTUNG_OBEN:
			pTextElement->m_nYAlignment = CTextElement::top;
			break;
		case ID_FRONTPOPUP_YAUSRICHTUNG_UNTEN:
			pTextElement->m_nYAlignment = CTextElement::bottom;
			break;
		case ID_FRONTPOPUP_YAUSRICHTUNG_ZENTRIERT:
			pTextElement->m_nYAlignment = CTextElement::center;
			break;
		case ID_FRONTPOPUP_YAUSRICHTUNG_FREI:
		{
			pTextElement->m_nYAlignment = CTextElement::userdef;
			if (m_pLayout->m_pActiveRect == &m_pLayout->m_rectTitle)
			{
				CCoverFreeValueDlg fvDlg(&m_pLayout->m_nCDTitleFrontYFreeValue, this);
				fvDlg.DoModal();
			}
			else
			{
				CCoverFreeValueDlg fvDlg(&m_pLayout->m_nArtistFrontYFreeValue, this);
				fvDlg.DoModal();
			}

			break;
		}

		case ID_FRONTPOPUP_XAUSRICHTUNG_LINKS:
			pTextElement->m_nXAlignment = CTextElement::left;
			break;
		case ID_FRONTPOPUP_XAUSRICHTUNG_RECHTS:
			pTextElement->m_nXAlignment = CTextElement::right;
			break;
		case ID_FRONTPOPUP_XAUSRICHTUNG_ZENTRIERT:
			pTextElement->m_nXAlignment = CTextElement::center;
			break;
		case ID_FRONTPOPUP_XAUSRICHTUNG_FREI:
		{
			pTextElement->m_nXAlignment = CTextElement::userdef;
			if (m_pLayout->m_pActiveRect == &m_pLayout->m_rectTitle)
			{
				CCoverFreeValueDlg fvDlg(&m_pLayout->m_nCDTitleFrontXFreeValue, this);
				fvDlg.DoModal();
			}
			else
			{
				CCoverFreeValueDlg fvDlg(&m_pLayout->m_nArtistFrontXFreeValue, this);
				fvDlg.DoModal();
			}
			break;
		}

		
		case ID_FRONTPOPUP_ZEICHENSATZ:
			CreateFontDlg(&pTextElement->m_lfFont);
			break;
		case ID_FRONTPOPUP_SETZEFARBEN_TEXTFARBE:
			CreateColorDlg(&pTextElement->m_colText);
			break;
		case ID_FRONTPOPUP_SETZEFARBEN_TEXTHINTERGRUND:
			CreateColorDlg(&pTextElement->m_colTextbk);
			break;
	}

	m_bRightButtonDown = FALSE;
	
	MyInvalidateRect(&m_pLayout->m_rectFront);

	return;
}


void CCdcoverView::MyInvalidateRect(CRect *pRect, const BOOL bClear)
{
	double dZoomX = ((double) m_pLayout->m_nZoomFactor / (double) 100.0) * (double) 2100.0;
	double dZoomY = ((double) m_pLayout->m_nZoomFactor / (double) 100.0) * (double) 2970.0;
	SetScrollSizes(MM_LOMETRIC, CSize((int) (dZoomX + 0.5), (int) (dZoomY + 0.5)));
	InvalidateRect(pRect, bClear);
	return;
}


void CCdcoverView::OnLButtonUp(UINT nFlags, CPoint point) 
{
	m_pLayout->m_pActiveRect = NULL;
	m_pLayout->m_pLastActiveRect = NULL;
	m_bWaitForDraw = TRUE;

	m_bMoveTB = FALSE;
	m_bResizeTB = FALSE;
	m_bMoveCur = FALSE;
	m_bResizeTopCur = FALSE;
	m_bResizeBottomCur = FALSE;
	m_bResizeLeftCur = FALSE;
	m_bResizeRightCur = FALSE;
	m_bResizeD1 = FALSE;
	m_bResizeD2 = FALSE;
	m_bResizeD3 = FALSE;
	m_bResizeD4 = FALSE;

	if (m_bLeftButtonDown && m_rectGrab.right != m_rectGrab.left && m_rectGrab.top != m_rectGrab.bottom)
	{
		m_rectGrab.NormalizeRect();
		m_rectGrabClient = RectInVP(&m_rectGrab);

		CMenu menu;
		menu.LoadMenu(IDR_MENU_TEXTBOXPOPUP);
		CMenu* pSubMenu = menu.GetSubMenu(0);

		POINT p;
		::GetCursorPos(&p);

		pSubMenu->TrackPopupMenu(TPM_LEFTALIGN | TPM_RIGHTBUTTON, p.x, p.y, this);
		RefreshSelector(m_pLayout->m_pLastActiveRect, TRUE);
		switch (m_pLayout->m_nCurrentArea)
		{
			case CCoverLayout::front:
				MyInvalidateRect(&m_pLayout->m_rectFront, TRUE);
				break;
			case CCoverLayout::back:
				MyInvalidateRect(&m_pLayout->m_rectBack, TRUE);
				break;
			case CCoverLayout::label:
				MyInvalidateRect(&m_pLayout->m_rectLabel, TRUE);
		}
	}

	
	m_rectGrab = CRect(0,0,0,0);
	m_bFirstDrag = TRUE;
	m_bLeftButtonDown = FALSE;

	CScrollView::OnLButtonUp(nFlags, point);

	return;
}

void CCdcoverView::OnVScroll(UINT nSBCode, UINT nPos, CScrollBar* pScrollBar) 
{
	RefreshSelector(m_pLayout->m_pLastActiveRect, TRUE);

	m_pLayout->m_pActiveRect = NULL;
	m_pLayout->m_pLastActiveRect = NULL;
	m_bWaitForDraw = TRUE;
	
	m_bLeftButtonDown = FALSE;
	m_bRightButtonDown = FALSE;
	MyInvalidateRect(&m_rectGrab);
	m_rectGrab = CRect(0,0,0,0);
	m_bFirstDrag = FALSE;
	
	CScrollView::OnVScroll(nSBCode, nPos, pScrollBar);

	return;
}

void CCdcoverView::OnHScroll(UINT nSBCode, UINT nPos, CScrollBar* pScrollBar) 
{
	RefreshSelector(m_pLayout->m_pLastActiveRect, TRUE);

	m_pLayout->m_pActiveRect = NULL;
	m_pLayout->m_pLastActiveRect = NULL;
	m_bWaitForDraw = TRUE;
	
	m_bLeftButtonDown = FALSE;
	m_bRightButtonDown = FALSE;
	MyInvalidateRect(&m_rectGrab);
	m_rectGrab = CRect(0,0,0,0);
	m_bFirstDrag = FALSE;
	
	CScrollView::OnHScroll(nSBCode, nPos, pScrollBar);

	return;
}

BOOL CCdcoverView::OnMouseWheel(UINT nFlags, short zDelta, CPoint pt) 
{
	RefreshSelector(m_pLayout->m_pLastActiveRect, TRUE);

	m_pLayout->m_pActiveRect = NULL;
	m_pLayout->m_pLastActiveRect = NULL;
	m_bWaitForDraw = TRUE;
	
	m_bLeftButtonDown = FALSE;
	m_bRightButtonDown = FALSE;
	MyInvalidateRect(&m_rectGrab);
	m_rectGrab = CRect(0,0,0,0);
	m_bFirstDrag = FALSE;

	if (m_pLayout->m_bZoomMode)
	{
		if (m_pLayout->SetZoomFactor(m_pLayout->m_nZoomFactor + (zDelta > 0 ? 10 : -10)))
		{
			m_bFirstDraw = TRUE;
			MyInvalidateRect(NULL, TRUE);
		}

		UpdateZoomStatusLine();

		return TRUE;
	}
	else
	{
		return CScrollView::OnMouseWheel(nFlags, zDelta, pt);
	}
}


void CCdcoverView::UpdateActiveRect(CPoint point)
{
	int nCoverMode = m_pLayout->GetCoverMode();

	m_pLayout->m_nCurrentArea = CCoverLayout::nothing;

	if (m_pLayout->m_pActiveMainRect != NULL)
	{
		if (m_pLayout->m_pActiveMainRect == &m_pLayout->m_rectLabel)
			m_pLayout->m_nCurrentArea = CCoverLayout::label;

		if (m_pLayout->m_pActiveMainRect == &m_pLayout->m_rectFront)
			m_pLayout->m_nCurrentArea = CCoverLayout::front;

		if (m_pLayout->m_pActiveMainRect == &m_pLayout->m_rectBack)
			m_pLayout->m_nCurrentArea = CCoverLayout::back;

		if (m_pLayout->m_pActiveMainRect == &m_pLayout->m_rectRight)
			m_pLayout->m_nCurrentArea = CCoverLayout::back;

		if (m_pLayout->m_pActiveMainRect == &m_pLayout->m_rectLeft)
			m_pLayout->m_nCurrentArea = CCoverLayout::back;
	}

	m_pLayout->m_pActiveMainRect = NULL;
	m_pLayout->m_pActiveRect = NULL;
	m_pLayout->m_pActiveTB = NULL;


	if (! m_pLayout->ZoomEnabled())
	{
		if ((nCoverMode == CCoverLayout::label))
		{
			CRgn creg, creg2;
			creg.CreateEllipticRgn(m_pLayout->m_rectLabel.left, m_pLayout->m_rectLabel.top, m_pLayout->m_rectLabel.right, m_pLayout->m_rectLabel.bottom);
			creg2.CreateEllipticRgn(m_pLayout->m_rectInnerLabel.left, m_pLayout->m_rectInnerLabel.top, m_pLayout->m_rectInnerLabel.right, m_pLayout->m_rectInnerLabel.bottom);

			if (creg.PtInRegion(point) && (! creg2.PtInRegion(point)))
			{
				m_pLayout->m_pActiveRect = &m_pLayout->m_rectLabel;
				m_pLayout->m_pActiveMainRect = &m_pLayout->m_rectLabel;
				m_pLayout->m_nCurrentArea = CCoverLayout::label;
			}

			for (int i = 0; i != (int)m_pLayout->m_vecTBLabel.size(); i++)
			{
				if (m_pLayout->m_vecTBLabel[i].m_rectArea.PtInRect(point))
				{
					m_pLayout->m_pActiveMainRect = &m_pLayout->m_rectLabel;
					m_pLayout->m_pActiveRect = &m_pLayout->m_vecTBLabel[i].m_rectArea;
					m_pLayout->m_pActiveTB = &m_pLayout->m_tbsL[i];
					m_pLayout->m_nActiveTBIndex = i;
				}
			}
		}
		if ((nCoverMode == CCoverLayout::back || nCoverMode == CCoverLayout::both) && m_pLayout->m_rectLeft.PtInRect(point))
		{
			m_pLayout->m_pActiveMainRect = &m_pLayout->m_rectLeft;
			m_pLayout->m_pActiveRect = &m_pLayout->m_rectLeft;
			m_pLayout->m_nCurrentArea = CCoverLayout::back;
		}
		if ((nCoverMode == CCoverLayout::back || nCoverMode == CCoverLayout::both) && m_pLayout->m_rectBack.PtInRect(point))
		{
			m_pLayout->m_pActiveMainRect = &m_pLayout->m_rectBack;
			m_pLayout->m_pActiveRect = &m_pLayout->m_rectBack;
			m_pLayout->m_nCurrentArea = CCoverLayout::back;

		}
		if ((nCoverMode == CCoverLayout::back || nCoverMode == CCoverLayout::both) && m_pLayout->m_rectRight.PtInRect(point))
		{
			m_pLayout->m_pActiveMainRect = &m_pLayout->m_rectRight;
			m_pLayout->m_pActiveRect = &m_pLayout->m_rectRight;
			m_pLayout->m_nCurrentArea = CCoverLayout::back;
		}

		
		if (nCoverMode == CCoverLayout::back || nCoverMode == CCoverLayout::both)
		{
			for (int i = 0; i < (int)m_pLayout->m_vecTBBack.size(); i++)
			{
				if (m_pLayout->m_vecTBBack[i].m_rectArea.PtInRect(point))
				{
					m_pLayout->m_pActiveMainRect = &m_pLayout->m_rectBack;
					m_pLayout->m_pActiveRect = &m_pLayout->m_vecTBBack[i].m_rectArea;
					m_pLayout->m_pActiveTB = &m_pLayout->m_tbsB[i];
					m_pLayout->m_nActiveTBIndex = i;
					m_pLayout->m_nCurrentArea = CCoverLayout::back;
				}
			}
		}
		
		if ((nCoverMode == CCoverLayout::front || nCoverMode == CCoverLayout::both) && m_pLayout->m_rectFront.PtInRect(point))
		{
			m_pLayout->m_pActiveMainRect = &m_pLayout->m_rectFront;
			m_pLayout->m_pActiveRect = &m_pLayout->m_rectFront;
			m_pLayout->m_nCurrentArea = CCoverLayout::front;
		}
		if ((nCoverMode == CCoverLayout::front || nCoverMode == CCoverLayout::both) && m_pLayout->m_rectArtist.PtInRect(point))
		{
			m_pLayout->m_pActiveMainRect = &m_pLayout->m_rectFront;
			m_pLayout->m_pActiveRect = &m_pLayout->m_rectArtist;
			m_pLayout->m_nCurrentArea = CCoverLayout::front;
		}
		if ((nCoverMode == CCoverLayout::front || nCoverMode == CCoverLayout::both) && m_pLayout->m_rectTitle.PtInRect(point))
		{
			m_pLayout->m_pActiveMainRect = &m_pLayout->m_rectFront;
			m_pLayout->m_pActiveRect = &m_pLayout->m_rectTitle;
			m_pLayout->m_nCurrentArea = CCoverLayout::front;
		}

		if (nCoverMode == CCoverLayout::front || nCoverMode == CCoverLayout::both)
		{
			for (int i = 0; i != (int)m_pLayout->m_vecTBFront.size(); i++)
			{
				if (m_pLayout->m_vecTBFront[i].m_rectArea.PtInRect(point))
				{
					m_pLayout->m_pActiveMainRect = &m_pLayout->m_rectFront;
					m_pLayout->m_pActiveRect = &m_pLayout->m_vecTBFront[i].m_rectArea;
					m_pLayout->m_pActiveTB = &m_pLayout->m_tbsF[i];
					m_pLayout->m_nActiveTBIndex = i;
					m_pLayout->m_nCurrentArea = CCoverLayout::front;
				}
			}
		}

		if (m_pLayout->m_pLastActiveRect != m_pLayout->m_pActiveRect && m_pLayout->m_pActiveRect != NULL)
		{
			RefreshSelector(m_pLayout->m_pLastActiveRect, TRUE);
			m_pLayout->m_pLastActiveRect = m_pLayout->m_pActiveRect;
			m_bWaitForDraw = TRUE;
		}
	}

	if (m_bWaitForDraw && m_pLayout->m_pActiveRect != NULL)
	{
		RefreshSelector(m_pLayout->m_pActiveRect, FALSE);
		m_bWaitForDraw = FALSE;
		return;
	}

	return;
}


void CCdcoverView::RefreshSelector(CRect *pRect, const BOOL bClear)
{
	if (pRect != NULL)
	{
		CRect r = *pRect;
		r = RectInVP(&r);
		r.left -= 15;
		r.top +=15;
		r.bottom = r.top - 30;
		r.right = r.left + 30;
		r = RectInWE(&r);
		MyInvalidateRect(&r, bClear);
		r = *pRect;
		r = RectInVP(&r);
		r.top += 15;
		r.right += 15;
		r.bottom = r.top - 30;
		r.left = r.right - 30;
		r = RectInWE(&r);
		MyInvalidateRect(&r, bClear);
		r = *pRect;
		r = RectInVP(&r);
		r.left -= 15;
		r.bottom -= 15;
		r.top = r.bottom + 30;
		r.right = r.left + 30;
		r = RectInWE(&r);
		MyInvalidateRect(&r, bClear);
		r = *pRect;
		r = RectInVP(&r);
		r.right += 15;
		r.bottom -= 15;
		r.top = r.bottom + 30;
		r.left = r.right - 30;
		r = RectInWE(&r);
		MyInvalidateRect(&r, bClear);
	}

	return;
}


void CCdcoverView::OnTextBoxMain() 
{
	const MSG *msg = GetCurrentMessage();
	const int MenuID = msg->wParam;

	switch (MenuID)
	{
		case ID_TEXTBOXPOPUP_GRAFIK:
		{
			CCoverTextBox tbNew(m_rectGrabClient);
			if (tbNew.AddGfx(this))
			{
				tbNew.m_nType= CCoverTextBox::grafik;
				AddTextBoxToArea(&tbNew);
			}
			break;
		}

		case ID_TEXTBOXPOPUP_NEUESTEXTELEMENT:
		{
			CCoverTextBox tbNew(m_rectGrabClient);
			CCoverEnterMultiStringDlg dlgString(&tbNew.m_teText.m_strText, this);
			if (dlgString.DoModal() == IDOK)
			{
				GetMultiLines(&tbNew);
				AddTextBoxToArea(&tbNew);
			}
			break;
		}
		case ID_TEXTBOXPOPUP_CDTITEL:
		{
			CCoverTextBox tbNew(m_rectGrabClient);
			tbNew.m_nType = CCoverTextBox::cdtitle;
			AddTextBoxToArea(&tbNew);
			break;
		}
		case ID_TEXTBOXPOPUP_INTERPRET:
		{
			CCoverTextBox tbNew(m_rectGrabClient);
			tbNew.m_nType = CCoverTextBox::cdartist;
			AddTextBoxToArea(&tbNew);
			break;
		}
		case ID_EXTRAS_ANZAHLLIEDER:
		{
			CCoverTextBox tbNew(m_rectGrabClient);
			tbNew.m_nType = CCoverTextBox::anzahllieder;
			AddTextBoxToArea(&tbNew);
			break;
		}
		case ID_EXTRAS_KATEGORIE:
		{
			CCoverTextBox tbNew(m_rectGrabClient);
			tbNew.m_nType = CCoverTextBox::kategorie;
			AddTextBoxToArea(&tbNew);
			break;
		}
		case ID_EXTRAS_MEDIUM:
		{
			CCoverTextBox tbNew(m_rectGrabClient);
			tbNew.m_nType = CCoverTextBox::medium;
			AddTextBoxToArea(&tbNew);
			break;
		}
		case ID_EXTRAS_ARCHIVNUMMER:
		{
			CCoverTextBox tbNew(m_rectGrabClient);
			tbNew.m_nType = CCoverTextBox::archivnummer;
			AddTextBoxToArea(&tbNew);
			break;
		}
		case ID_EXTRAS_CODES:
		{
			CCoverTextBox tbNew(m_rectGrabClient);
			tbNew.m_nType = CCoverTextBox::codes;
			AddTextBoxToArea(&tbNew);
			break;
		}
		case ID_EXTRAS_KOMMENTAR:
		{
			CCoverTextBox tbNew(m_rectGrabClient);
			tbNew.m_nType = CCoverTextBox::kommentar;
			AddTextBoxToArea(&tbNew);
			break;
		}
		case ID_EXTRAS_AUFNAHMEJAHR:
		{
			CCoverTextBox tbNew(m_rectGrabClient);
			tbNew.m_nType = CCoverTextBox::aufnahmejahr;
			AddTextBoxToArea(&tbNew);
			break;
		}
		case ID_EXTRAS_COPYRIGHT:
		{
			CCoverTextBox tbNew(m_rectGrabClient);
			tbNew.m_nType = CCoverTextBox::copyright;
			AddTextBoxToArea(&tbNew);
			break;
		}
		case ID_EXTRAS_DATUM:
		{
			CCoverTextBox tbNew(m_rectGrabClient);
			tbNew.m_nType = CCoverTextBox::datum;
			AddTextBoxToArea(&tbNew);
			break;
		}
		case ID_EXTRAS_GESAMTLEN:
		{
			CCoverTextBox tbNew(m_rectGrabClient);
			tbNew.m_nType = CCoverTextBox::gesamtlen;
			AddTextBoxToArea(&tbNew);
			break;
		}
		case ID_EXTRAS_CDSETNR:
		{
			CCoverTextBox tbNew(m_rectGrabClient);
			tbNew.m_nType = CCoverTextBox::cdsetnr;
			AddTextBoxToArea(&tbNew);
			break;
		}
		case ID_EXTRAS_CDSETNAME:
		{
			CCoverTextBox tbNew(m_rectGrabClient);
			tbNew.m_nType = CCoverTextBox::cdsetname;
			AddTextBoxToArea(&tbNew);
			break;
		}
		case ID_EXTRAS_USER1:
		{
			CCoverTextBox tbNew(m_rectGrabClient);
			tbNew.m_nType = CCoverTextBox::user0;
			AddTextBoxToArea(&tbNew);
			break;
		}
		case ID_EXTRAS_USER2:
		{
			CCoverTextBox tbNew(m_rectGrabClient);
			tbNew.m_nType = CCoverTextBox::user1;
			AddTextBoxToArea(&tbNew);
			break;
		}
		case ID_EXTRAS_USER3:
		{
			CCoverTextBox tbNew(m_rectGrabClient);
			tbNew.m_nType = CCoverTextBox::user2;
			AddTextBoxToArea(&tbNew);
			break;
		}
		case ID_EXTRAS_USER4:
		{
			CCoverTextBox tbNew(m_rectGrabClient);
			tbNew.m_nType = CCoverTextBox::user3;
			AddTextBoxToArea(&tbNew);
			break;
		}
		case ID_EXTRAS_USER5:
		{
			CCoverTextBox tbNew(m_rectGrabClient);
			tbNew.m_nType = CCoverTextBox::user4;
			AddTextBoxToArea(&tbNew);
			break;
		}
	}

	return;
}


void CCdcoverView::AddTextBoxToArea(CCoverTextBox *ptbNew)
{
	ptbNew->m_teText.m_lfFont.lfHeight = 80;

	CPoint *pPt = &m_ptL;
	std::vector<CCoverTextBox> *pvecTB = &m_pLayout->m_tbsL;

	switch (m_pLayout->m_nCurrentArea)
	{
		case CCoverLayout::front:
		{
			pPt = &m_ptF;
			pvecTB = &m_pLayout->m_tbsF;
			break;
		}
		case CCoverLayout::back:
		{
			pPt = &m_ptB;
			pvecTB = &m_pLayout->m_tbsB;
		}
	}

	CRect rect = m_rectGrabClient;
	rect.left -= pPt->x;
	rect.right -= pPt->x;
	rect.top -= pPt->y;
	rect.bottom -= pPt->y;
	ptbNew->m_rectArea = rect;
	pvecTB->push_back(*ptbNew);

	RefreshTextBox(&ptbNew->m_rectArea, &ptbNew->m_rectArea);

	return;
}


void CCdcoverView::WorkOnTextBox()
{
	CTextElement *pTextElement = &m_pLayout->m_pActiveTB->m_teText;

	CPoint pt;
	::GetCursorPos(&pt);
	
	CMenu menu;
	menu.LoadMenu(IDR_MENU_TBPOPUP);
	CMenu* pSubMenu = menu.GetSubMenu(0);

	int nAlign = ID_TBPOPUP_XAUSRICHTUNG_ZENTRIERT;

	switch (pTextElement->m_nXAlignment)
	{
		case CTextElement::left:
			nAlign = ID_TBPOPUP_XAUSRICHTUNG_LINKS;
			break;
		case CTextElement::right:
			nAlign = ID_TBPOPUP_XAUSRICHTUNG_RECHTS;
	}

	int nYAlign = ID_TBPOPUP_YAUSRICHTUNG_ZENTRIERT;
	switch (pTextElement->m_nYAlignment)
	{
		case CTextElement::top:
			nYAlign = ID_TBPOPUP_YAUSRICHTUNG_OBEN;
			break;
		case CTextElement::bottom:
			nYAlign = ID_TBPOPUP_YAUSRICHTUNG_UNTEN;
	}

	if (m_pLayout->m_pActiveTB->m_nType != CCoverTextBox::text)
	{
		pSubMenu->EnableMenuItem(ID_TEXTBOXPOPUP_EDITIERETEXT, MF_BYCOMMAND | MF_GRAYED);
	}

	pSubMenu->CheckMenuItem(nAlign, MF_BYCOMMAND | MF_CHECKED);
	pSubMenu->CheckMenuItem(nYAlign, MF_BYCOMMAND | MF_CHECKED);
	pSubMenu->CheckMenuItem(ID_TBPOPUP_BOXTRANSPARENZ, MF_BYCOMMAND | (m_pLayout->m_pActiveTB->m_bTransparent ? MF_CHECKED : MF_UNCHECKED));
	pSubMenu->CheckMenuItem(ID_TBPOPUP_TEXTTRANSPARENZ, MF_BYCOMMAND | (pTextElement->m_nBkMode == TRANSPARENT ? MF_CHECKED : MF_UNCHECKED));
	pSubMenu->EnableMenuItem(ID_TBPOPUP_SETZEFARBEN_RAHMEN, MF_BYCOMMAND | m_pLayout->m_pActiveTB->m_bTransparent ? MF_GRAYED : MF_ENABLED);
	pSubMenu->EnableMenuItem(ID_TBPOPUP_SETZEFARBEN_HINTERGRUND, MF_BYCOMMAND | m_pLayout->m_pActiveTB->m_bTransparent ? MF_GRAYED : MF_ENABLED);

	if (m_pLayout->m_pActiveTB->m_nType == CCoverTextBox::grafik)
	{
		CMenu menuGfx;
		menuGfx.LoadMenu(IDR_MENU_TBPOPUPGFX);
		CMenu *pSubMenuGfx = menuGfx.GetSubMenu(0);
		pSubMenuGfx->CheckMenuItem(ID_TBPOPUP_FLIP, MF_BYCOMMAND | (m_pLayout->m_pActiveTB->m_bIsFlipped ? MF_CHECKED : MF_UNCHECKED));
		pSubMenuGfx->CheckMenuItem(ID_TBPOPUP_MIRROR, MF_BYCOMMAND | (m_pLayout->m_pActiveTB->m_bIsMirrored ? MF_CHECKED : MF_UNCHECKED));

		pSubMenuGfx->TrackPopupMenu(TPM_LEFTALIGN | TPM_RIGHTBUTTON, pt.x, pt.y, this);

	}
	else
	{
		pSubMenu->TrackPopupMenu(TPM_LEFTALIGN | TPM_RIGHTBUTTON, pt.x, pt.y, this);
	}

	return;
}

void CCdcoverView::ResortTBs(const int nNewIndex, const BOOL bForward)
{
	if (m_pLayout->m_nActiveTBIndex == nNewIndex)
		return;

	if (m_pLayout->m_pActiveTB == NULL)
		return;

	std::vector<CCoverTextBox> *pvecTextBox = &m_pLayout->m_tbsF;
	switch (m_pLayout->m_nCurrentArea)
	{
		case CCoverLayout::back:
			pvecTextBox = &m_pLayout->m_tbsB;
			break;
		case CCoverLayout::label:
			pvecTextBox = &m_pLayout->m_tbsL;
	}

	std::vector<CCoverTextBox> vecTemp;

	for (int i = 0; i < (int)pvecTextBox->size(); i++)
	{
		if (i == nNewIndex && ! bForward)
            vecTemp.push_back(*m_pLayout->m_pActiveTB);
		if (i != m_pLayout->m_nActiveTBIndex)
			vecTemp.push_back((*pvecTextBox)[i]);
		if (i == nNewIndex && bForward)
            vecTemp.push_back(*m_pLayout->m_pActiveTB);
	}

	*pvecTextBox = vecTemp;

	return;
}

void CCdcoverView::OnTBPopup() 
{
	const MSG *msg = GetCurrentMessage();
	const int MenuID = msg->wParam;

	CTextElement *pTextElement = &(m_pLayout->m_pActiveTB->m_teText);

	std::vector<CCoverTextBox> *pvecTextBox = &m_pLayout->m_tbsF;
	switch (m_pLayout->m_nCurrentArea)
	{
		case CCoverLayout::back:
			pvecTextBox = &m_pLayout->m_tbsB;
			break;
		case CCoverLayout::label:
			pvecTextBox = &m_pLayout->m_tbsL;
	}

	switch (MenuID)
	{
		case ID_POSITION_CENTERXY:
		case ID_TB_POSITION_CENTERXY:
			CenterX();
			CenterY();
			break;
		case ID_POSITION_CENTERX:
		case ID_TB_POSITION_CENTERX:
			CenterX();
			break;
		case ID_POSITION_CENTERY:
		case ID_TB_POSITION_CENTERY:
			CenterY();
			break;

		case ID_TBPOPUP_BOXTRANSPARENZ:
			m_pLayout->m_pActiveTB->m_bTransparent = ! m_pLayout->m_pActiveTB->m_bTransparent;
			break;

		case ID_TBPOPUP_SETZEFARBEN_HINTERGRUND:
			CreateColorDlg(&m_pLayout->m_pActiveTB->m_colBack);
			break;
		case ID_TBPOPUP_SETZEFARBEN_RAHMEN:
			CreateColorDlg(&m_pLayout->m_pActiveTB->m_colBorder);
			break;

		case ID_TEXTBOXPOPUP_EDITIERETEXT:
		{
			CCoverEnterMultiStringDlg dlgString(&m_pLayout->m_pActiveTB->m_teText.m_strText, this);
			if (dlgString.DoModal() == IDOK)
			{
				GetMultiLines(m_pLayout->m_pActiveTB);
				RefreshTextBox(&m_pLayout->m_pActiveTB->m_rectArea, &m_pLayout->m_pActiveTB->m_rectArea);
			}
			break;
		}

		case ID_TBPOPUP_REMOVEGFX:
		case ID_TBPOPUP_REMOVE:
		{
			std::vector<CCoverTextBox>::iterator Pos1;
			std::vector<CCoverTextBox>::iterator Pos2;
			std::vector<CCoverTextBox> *pList1 = NULL;
			std::vector<CCoverTextBox> *pList2 = NULL;

			switch (m_pLayout->m_nCurrentArea)
			{
				case CCoverLayout::front:
					Pos1 = m_pLayout->m_vecTBFront.begin();
					Pos1 += m_pLayout->m_nActiveTBIndex;

					Pos2 = m_pLayout->m_tbsF.begin();
					Pos2 += m_pLayout->m_nActiveTBIndex;

					pList1 = &m_pLayout->m_vecTBFront;
					pList2 = &m_pLayout->m_tbsF;
					break;

				case CCoverLayout::back:
					Pos1 = m_pLayout->m_vecTBBack.begin();
					Pos1 += m_pLayout->m_nActiveTBIndex;

					Pos2 = m_pLayout->m_tbsB.begin();
					Pos2 += m_pLayout->m_nActiveTBIndex;

					pList1 = &m_pLayout->m_vecTBBack;
					pList2 = &m_pLayout->m_tbsB;
					break;

				case CCoverLayout::label:
					Pos1 = m_pLayout->m_vecTBLabel.begin();
					Pos1 += m_pLayout->m_nActiveTBIndex;

					Pos2 = m_pLayout->m_tbsL.begin();
					Pos2 += m_pLayout->m_nActiveTBIndex;

					pList1 = &m_pLayout->m_vecTBLabel;
					pList2 = &m_pLayout->m_tbsL;
			}

			Pos1->DelGfx();
			Pos2->DelGfx();
			pList1->erase(Pos1);
			pList2->erase(Pos2);

			break;
		}
	
		case ID_TBPOPUP_TEXTTRANSPARENZ:
			pTextElement->m_nBkMode = (pTextElement->m_nBkMode == OPAQUE) ? TRANSPARENT : OPAQUE;
			break;
		case ID_TBPOPUP_YAUSRICHTUNG_OBEN:
			pTextElement->m_nYAlignment = CTextElement::top;
			break;
		case ID_TBPOPUP_YAUSRICHTUNG_UNTEN:
			pTextElement->m_nYAlignment = CTextElement::bottom;
			break;
		case ID_TBPOPUP_YAUSRICHTUNG_ZENTRIERT:
			pTextElement->m_nYAlignment = CTextElement::center;
			break;

		case ID_TBPOPUP_XAUSRICHTUNG_LINKS:
			pTextElement->m_nXAlignment = CTextElement::left;
			break;
		case ID_TBPOPUP_XAUSRICHTUNG_RECHTS:
			pTextElement->m_nXAlignment = CTextElement::right;
			break;
		case ID_TBPOPUP_XAUSRICHTUNG_ZENTRIERT:
			pTextElement->m_nXAlignment = CTextElement::center;
			break;
		
		case ID_TBPOPUP_ZEICHENSATZ:
			CreateFontDlg(&pTextElement->m_lfFont);
			break;
		case ID_TBPOPUP_SETZEFARBEN_TEXTFARBE:
			CreateColorDlg(&pTextElement->m_colText);
			break;
		case ID_TBPOPUP_SETZEFARBEN_TEXTHINTERGRUND:
			CreateColorDlg(&pTextElement->m_colTextbk);
			break;

		case ID_TBPOPUP_FLIP:
			m_pLayout->m_pActiveTB->m_bIsFlipped = ! m_pLayout->m_pActiveTB->m_bIsFlipped;
			m_pLayout->m_vecGfxLookup[m_pLayout->m_pActiveTB->m_nGfxIndex].m_bFlip = TRUE;
			m_pLayout->m_vecGfxLookup[m_pLayout->m_pActiveTB->m_nGfxIndex].m_bApplyEffectsOnce = TRUE;
			break;

		case ID_TBPOPUP_ASPECTRATIO:
		{
			if (m_pLayout->m_vecGfxLookup[m_pLayout->m_pActiveTB->m_nGfxIndex].m_dAspectRatio != (double) 0.0)
			{
				CRect rectOldTB = m_pLayout->m_pActiveTB->m_rectArea;
				CRect rectTB = m_pLayout->m_pActiveTB->m_rectArea;
				const int nNewHeight = (int)(double)((double) rectTB.Width() / m_pLayout->m_vecGfxLookup[m_pLayout->m_pActiveTB->m_nGfxIndex].m_dAspectRatio);
				rectTB.bottom = rectTB.top - nNewHeight;

				const CRect rectMain = RectInVP(m_pLayout->m_pActiveMainRect);
				if (rectTB.bottom > rectMain.bottom - rectMain.top)
					m_pLayout->m_pActiveTB->m_rectArea = rectTB;
			}
			break;
		}

		case ID_TBPOPUP_MIRROR:
			m_pLayout->m_pActiveTB->m_bIsMirrored = ! m_pLayout->m_pActiveTB->m_bIsMirrored;
			m_pLayout->m_vecGfxLookup[m_pLayout->m_pActiveTB->m_nGfxIndex].m_bMirror = TRUE;
			m_pLayout->m_vecGfxLookup[m_pLayout->m_pActiveTB->m_nGfxIndex].m_bApplyEffectsOnce = TRUE;
			break;

		case ID_TB_POSITION_NACHVORNE:
		case ID_POSITION_NACHVORNE:
			ResortTBs(m_pLayout->m_nActiveTBIndex == (int)pvecTextBox->size() - 1 ? (int)pvecTextBox->size() - 1 : m_pLayout->m_nActiveTBIndex + 1, TRUE);
			break;
		case ID_TB_POSITION_NACHHINTEN:
		case ID_POSITION_NACHHINTEN:
			ResortTBs(m_pLayout->m_nActiveTBIndex != 0 ? m_pLayout->m_nActiveTBIndex - 1 : 0, FALSE);
			break;
		case ID_TB_POSITION_TOPMOST:
		case ID_POSITION_TOPMOST:
			ResortTBs((int)pvecTextBox->size() - 1, TRUE);
			break;
		case ID_TB_POSITION_BACKMOST:
		case ID_POSITION_BACKMOST:
			ResortTBs(0, FALSE);
			break;


	}

	m_bWaitForDraw = FALSE;
	MyInvalidateRect();

	m_bRightButtonDown = FALSE;

	m_rectGrab = CRect(0,0,0,0);
	m_bFirstDrag = TRUE;
	m_bLeftButtonDown = FALSE;

	return;
}

void CCdcoverView::OnRButtonUp(UINT nFlags, CPoint point) 
{
	m_bRightButtonDown = FALSE;

	if (m_bMoveTB || m_bResizeTB)
	{
		m_bMoveTB = FALSE;
		m_bResizeTB = FALSE;
		m_bLeftButtonDown = FALSE;
		m_rectGrab = CRect(0,0,0,0);
		MyInvalidateRect(NULL);
		return;
	}

	CScrollView::OnRButtonUp(nFlags, point);

	return;
}


CPoint CCdcoverView::PtRelToTB(CPoint *ppoint)
{
	CPoint ptMouseSet = *ppoint;
	CPoint *pPt = &m_ptL;
	switch (m_pLayout->m_nCurrentArea)
	{
		case CCoverLayout::back:
			pPt = &m_ptB;
			break;
		case CCoverLayout::front:
			pPt = &m_ptF;
	}

	ptMouseSet.x -= pPt->x;
	ptMouseSet.y -= pPt->y;
	ptMouseSet.x -= m_pLayout->m_pActiveTB->m_rectArea.left;
	ptMouseSet.y -= m_pLayout->m_pActiveTB->m_rectArea.top;

	return ptMouseSet;
}


void CCdcoverView::RefreshTextBox(CRect *prectOld, CRect *prectNew)
{
	if (prectOld == NULL || prectNew == NULL)
		return;

	CPoint *pPt = &m_ptL;
	switch (m_pLayout->m_nCurrentArea)
	{
		case CCoverLayout::back:
			pPt = &m_ptB;
			break;
		case CCoverLayout::front:
			pPt = &m_ptF;
	}

	CRect rectNew = *prectNew;
	CRect rectOld = *prectOld;

	rectNew.left += pPt->x;
	rectNew.right += pPt->x;
	rectNew.top += pPt->y;
	rectNew.bottom += pPt->y;
	rectOld.left += pPt->x;
	rectOld.right += pPt->x;
	rectOld.top += pPt->y;
	rectOld.bottom += pPt->y;
	
	rectOld = RectInWE(&rectOld);
	rectNew = RectInWE(&rectNew);
	rectOld.NormalizeRect();
	rectNew.NormalizeRect();

	rectOld.left -= 15;
	rectOld.top -= 15;
	rectOld.right += 15;
	rectOld.bottom += 15;
	rectNew.left -= 15;
	rectNew.top -= 15;
	rectNew.right += 15;
	rectNew.bottom += 15;

	CRect r = rectNew;
	r |= rectOld;

	MyInvalidateRect(&r);

	return;
}



void CCdcoverView::WorkOnLabel()
{
	CPoint pt;
	::GetCursorPos(&pt);
	
	CMenu menu;
	menu.LoadMenu(IDR_MENU_LABELMAIN);
	CMenu* pSubMenu = menu.GetSubMenu(0);

	if (m_pLayout->m_bTextGfxLabel)
	{
		pSubMenu->CheckMenuItem(ID_LABELMAIN_TEXTGFX, MF_BYCOMMAND | MF_CHECKED);
	}
	
	if (! m_pLayout->m_bLabelGfx)
	{
		pSubMenu->EnableMenuItem(ID_LABELMAIN_DELGRAFIK, MF_BYCOMMAND | MF_GRAYED);
	}


	pSubMenu->TrackPopupMenu(TPM_LEFTALIGN | TPM_RIGHTBUTTON, pt.x, pt.y, this);

	MyInvalidateRect(&m_pLayout->m_rectLabel);

	return;
}


void CCdcoverView::OnLabelMainPopup() 
{
	const MSG *msg = GetCurrentMessage();
	const int MenuID = msg->wParam;

	switch (MenuID)
	{
		case ID_LABELMAIN_ADDGRAFIK:
			m_pLayout->AddGfxLabel(this);
			break;
		case ID_LABELMAIN_DELGRAFIK:
			m_pLayout->DelGfxLabel();
			break;
		case ID_LABELMAIN_TEXTGFX:
			m_pLayout->ToggleTextGfxLabel();
			break;
		case ID_LABELMAIN_SETZEFARBEN_HINTERGRUND:
			CreateColorDlg(&m_pLayout->m_colLabel);
			break;
		case ID_LABELMAIN_SETZEFARBEN_RAHMEN:
			CreateColorDlg(&m_pLayout->m_colLabelRand);
	}

	m_bRightButtonDown = FALSE;
	
	MyInvalidateRect(NULL, FALSE);

	return;
}


void CCdcoverView::UpdateZoomStatusLine()
{
	CString strZoom, strZ;
	strZ.LoadString(IDS_CDCOVER_ZOOM);
	strZoom.Format(L" %03d", m_pLayout->m_nZoomFactor);
	m_pwndStatusBar->SetPaneText(0, strZ + strZoom, TRUE);

	return;
}


CPoint CCdcoverView::PointInVP(CPoint *ppoint, CDC *ppDC)
{
	CDC *pDC = (ppDC == NULL) ? GetDC() : ppDC;

	CPoint pt = *ppoint;
	int nOldMode = pDC->SetMapMode(MM_LOMETRIC);
	pt.y += GetScrollPos(SB_VERT);
	pt.x += GetScrollPos(SB_HORZ);
	pDC->DPtoLP(&pt);
	pDC->SetMapMode(nOldMode);

	if (ppDC == NULL)
	{
		ReleaseDC(pDC);
	}

	return WEtoVP(&pt);
}


CRect CCdcoverView::RectInVP(CRect *prect, CDC *ppDC)
{
	CPoint p1(prect->left, prect->top);
	CPoint p2(prect->right, prect->bottom);

	p1 = PointInVP(&p1, ppDC);
	p2 = PointInVP(&p2, ppDC);

	return CRect(p1.x, p1.y, p2.x, p2.y);
}


CPoint CCdcoverView::WEtoVP(CPoint *ppt)
{
	double dx = 100.0 / (double) m_pLayout->m_nZoomFactor;
	dx = ((double) ppt->x * dx) + 0.5;
	double dy = 100.0 / (double) m_pLayout->m_nZoomFactor;
	dy = ((double) ppt->y * dy) + 0.5;
	return CPoint((int) dx, (int) dy);
}


CPoint CCdcoverView::VPtoWE(CPoint *ppt)
{
	double dx = (double) m_pLayout->m_nZoomFactor / 100.0;
	dx = ((double) ppt->x * dx) + 0.5;
	double dy = (double) m_pLayout->m_nZoomFactor / 100.0;
	dy = ((double) ppt->y * dy) + 0.5;
	return CPoint((int) dx, (int) dy);
}


CPoint CCdcoverView::PointInWE(CPoint *ppoint)
{
	CPoint pt = *ppoint;
	pt = VPtoWE(&pt);
	CDC *pDC = GetDC();
	int nOldMode = pDC->SetMapMode(MM_LOMETRIC);
	pDC->LPtoDP(&pt);
	pDC->SetMapMode(nOldMode);
	pt.y -= GetScrollPos(SB_VERT);
	pt.x -= GetScrollPos(SB_HORZ);
	ReleaseDC(pDC);

	return pt;
}


CRect CCdcoverView::RectInWE(CRect *prect)
{
	CPoint p1(prect->left, prect->top);
	CPoint p2(prect->right, prect->bottom);

	p1 = PointInWE(&p1);
	p2 = PointInWE(&p2);

	return CRect(p1.x, p1.y, p2.x, p2.y);
}


void CCdcoverView::FrameTB(CDC *pDC, CRect *prectTB, CPoint *pptOffset, COLORREF col)
{
	CPen pen(PS_SOLID | PS_DOT, 0, (col ^ 0x00ffffff));
	CPen *pOldPen = pDC->SelectObject(&pen);
	CRect rr = *prectTB;

	rr = RectInWE(&rr);
	rr.top++;
	rr.left++;
	rr.right--;
	rr.bottom--;
	rr = RectInVP(&rr);

	rr.NormalizeRect();

	rr.left += pptOffset->x;
	rr.right += pptOffset->x;
	rr.top += pptOffset->y;
	rr.bottom += pptOffset->y;

	pDC->MoveTo(rr.left, rr.top);
	pDC->LineTo(rr.right, rr.top);
	pDC->LineTo(rr.right, rr.bottom);
	pDC->LineTo(rr.left, rr.bottom);
	pDC->LineTo(rr.left, rr.top);
	pDC->SelectObject(pOldPen);

	return;
}



void CCdcoverView::CheckTBState(CPoint *ppoint)
{
	if (! m_bResizeTB && ! m_bMoveTB && m_pLayout->m_pActiveTB != NULL)
	{
		CPoint p = PointInVP(ppoint);
		p = PtRelToTB(&p);

		m_bResizeRightCur = (p.x >= m_pLayout->m_pActiveTB->m_rectArea.Width() - 30);
		m_bResizeLeftCur = (p.x <= 30);
		m_bResizeTopCur = (p.y >= -30);
		m_bResizeBottomCur = (p.y <= m_pLayout->m_pActiveTB->m_rectArea.Height() + 30);
		m_bResizeD4 = (m_bResizeBottomCur && m_bResizeRightCur);
		m_bResizeD3 = (m_bResizeBottomCur && m_bResizeLeftCur);
		m_bResizeD2 = (m_bResizeTopCur && m_bResizeRightCur);
		m_bResizeD1 = (m_bResizeTopCur && m_bResizeLeftCur);
		m_bMoveCur = ! (m_bResizeRightCur || m_bResizeLeftCur || m_bResizeTopCur || m_bResizeBottomCur || m_bResizeD1 || m_bResizeD2 || m_bResizeD3 || m_bResizeD4);
	}

	return;
}


void CCdcoverView::GetMultiLines(CCoverTextBox *pTB)
{
	pTB->m_vecTextlines.clear();

	switch (pTB->m_nType)
	{
		case CCoverTextBox::cdtitle:
			pTB->m_vecTextlines.push_back(m_pLayout->m_pccd->Title);
			break;
		case CCoverTextBox::cdartist:
			pTB->m_vecTextlines.push_back(m_pLayout->m_pccd->Artist);
			break;

		default:
		{
			CString s;
			s.Format(L"%s", pTB->m_teText.m_strText);
	
			wchar_t *token = wcstok(s.GetBuffer(-1), L"\n");
			CString strLine(token);
			strLine.Remove('\r');

			pTB->m_vecTextlines.push_back(strLine);

			while (token != NULL)
			{
				token = wcstok(NULL, L"\n");
				if (token == NULL)
				{
					break;
				}
				strLine = (CString) token;
				strLine.Remove('\r');
				pTB->m_vecTextlines.push_back(strLine);
			}
		}
	}

	return;
}

void CCdcoverView::OnLButtonDblClk(UINT nFlags, CPoint point) 
{
	if (m_pLayout->m_pActiveTB != NULL && m_pLayout->m_pActiveTB->m_nType == CCoverTextBox::text)
	{
		CCoverEnterMultiStringDlg dlgString(&m_pLayout->m_pActiveTB->m_teText.m_strText, this);
		if (dlgString.DoModal() == IDOK)
		{
			GetMultiLines(m_pLayout->m_pActiveTB);
			RefreshTextBox(&m_pLayout->m_pActiveTB->m_rectArea, &m_pLayout->m_pActiveTB->m_rectArea);
		}
	}
	
	CScrollView::OnLButtonDblClk(nFlags, point);

	return;
}

void CCdcoverView::OnFilePrint()
{
	CScrollView::OnFilePrint();
}

void CCdcoverView::OnFilePrintPreview() 
{
    // In derived classes, implement special window handling here
    // Be sure to Unhook Frame Window close if hooked.

//	CScrollView::OnFilePrintPreview();

    // must not create this on the frame. Must outlive this function
    CPrintPreviewState* pState = new CPrintPreviewState;

    if (!DoPrintPreview(IDD_COVER_OWN_PREVIEW_TOOLBAR, this,
                RUNTIME_CLASS(CCoverPreviewView), pState))
    {
        // In derived classes, reverse special window handling
        // here for Preview failure case

        TRACE0("Error: DoPrintPreview failed");
        AfxMessageBox(AFX_IDP_COMMAND_FAILURE);
        delete pState;      // preview failed to initialize, 
                    // delete State now
    }
}

void CCdcoverView::CenterX()
{
	if (m_pLayout != NULL && m_pLayout->m_pActiveTB != NULL && m_pLayout->m_pActiveMainRect != NULL)
	{
		CRect rectTB = m_pLayout->m_pActiveTB->m_rectArea;
		const CRect rectMain = RectInVP(m_pLayout->m_pActiveMainRect);
		const int xWidth = rectTB.Width();
		const int xCenter = (rectMain.Width() - xWidth) / 2;
		rectTB.left = xCenter;
		rectTB.right = xCenter + xWidth;
		m_pLayout->m_pActiveTB->m_rectArea = rectTB;
	}
	return;
}

void CCdcoverView::CenterY()
{
	if (m_pLayout != NULL && m_pLayout->m_pActiveTB != NULL && m_pLayout->m_pActiveMainRect != NULL)
	{
		CRect rectTB = m_pLayout->m_pActiveTB->m_rectArea;
		const CRect rectMain = RectInVP(m_pLayout->m_pActiveMainRect);
		const int yHeight = rectTB.Height();
		const int yCenter = (rectMain.Height() - yHeight) / 2;
		rectTB.top = yCenter;
		rectTB.bottom = yCenter + yHeight;
		m_pLayout->m_pActiveTB->m_rectArea = rectTB;
	}
	return;
}

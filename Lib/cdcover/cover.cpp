#include "stdafx.h"
#include "cover.h"
#include <vector>
#include "textelement.h"
#include "covergrafik.h"
#include <algorithm>

using namespace Big3::Hitbase::DataBaseEngine;

/////////////////////////////////////////////////////////////////////////


CCoverZweckform::CCoverZweckform()
{
	m_strFirma = "";
	m_vecFormate.clear();
}

CCoverFormat::CCoverFormat()
{
	m_strNummer = "";
	m_nFX = -1;
	m_nFY = -1;
	m_nBX = -1;
	m_nBY = -1;
	m_nLX = -1;
	m_nLY = -1;
}


/////////////////////////////////////////////////////////////////////////



CCover::CCover(CDC *pDC, const int XPos, const int YPos, const int nRandX, const int nRandY)
{
	m_vecFrames.clear();
	m_vecCircles.clear();
	m_vecTextElements.clear();
	m_nXPos = XPos - (pDC->IsPrinting() ? nRandX : 0);
	m_nYPos = YPos - (pDC->IsPrinting() ? nRandY : 0);
}


int CCover::GetRealFontHeight(CDC *pDC, LOGFONT *plogfont)
{
	CFont *pOldFont = pDC->GetCurrentFont();
	CFont ftFont;
	ftFont.CreateFontIndirect(plogfont);
	pDC->SelectObject(&ftFont);
	pDC->SetTextAlign(TA_LEFT | TA_TOP | TA_NOUPDATECP);

	TEXTMETRIC tm;
	pDC->GetTextMetrics(&tm);

	pDC->SelectObject(pOldFont);

	return abs(tm.tmHeight);
}


void CCover::DrawTB(CCoverLayout *pLayout, std::vector<CCoverTextBox> *pvecTBs, const BOOL bTextGfx, const int nXOffset, CDC *pDC)
{
	for (int t = 0; t != (int)pvecTBs->size(); t++)
	{
		CCoverTextBox *pTB = &((*pvecTBs)[t]);
		CTextElement CurrentTE = pTB->m_teText;

		if (pTB->m_nType == CCoverTextBox::grafik)
		{
			CCoverGrafik *pGfx = NULL;
			BOOL bInit = FALSE;

			if (pTB->m_nGfxIndex == -1)
			{
				CCoverGrafik Dummy;
				Dummy.m_strFileName = pTB->m_strGfxName;
				Dummy.m_bApplyEffectsOnce = TRUE;
				if (pTB->m_bIsFlipped)
					Dummy.m_bFlip = TRUE;
				if (pTB->m_bIsMirrored)
					Dummy.m_bMirror = TRUE;
			
				if (Dummy.Init(Dummy.m_strFileName))
				{
					bInit = TRUE;
					pLayout->m_vecGfxLookup.push_back(Dummy);
					pTB->m_nGfxIndex = pLayout->m_vecGfxLookup.size() - 1;
				}
			}
			else
				bInit = TRUE;

			pGfx = &pLayout->m_vecGfxLookup[pTB->m_nGfxIndex];

			if (bInit && pGfx != NULL)
			{
				CRect rectGfx;

				rectGfx.left = pTB->m_rectArea.left + m_nXPos + nXOffset;
				rectGfx.top = pTB->m_rectArea.top - m_nYPos;

				rectGfx.right = pTB->m_rectArea.right + m_nXPos + nXOffset;
				rectGfx.bottom = pTB->m_rectArea.bottom - m_nYPos;
				
				pGfx->Draw(&rectGfx, pDC);
			}
		}
		else
		{

			if (pTB->m_vecTextlines.size() == 0 && pTB->m_nType != CCoverTextBox::text)
			{
				CString strText;

				switch (pTB->m_nType)
				{
					case CCoverTextBox::cdartist:
						strText = pLayout->m_pccd->Artist;
						break;
					case CCoverTextBox::cdtitle:
						strText = pLayout->m_pccd->Title;
						break;
					case CCoverTextBox::gesamtlen:
						strText = CCover::TimeToString(pLayout->m_pccd->TotalLength);
						break;
					case CCoverTextBox::anzahllieder:
						strText.Format(L"%d", pLayout->m_pccd->NumberOfTracks);
						break;
					case CCoverTextBox::cdsetnr:
						strText.Format(L"%d", pLayout->m_pccd->CDSetNumber);
						break;
					case CCoverTextBox::cdsetname:
						strText = pLayout->m_pccd->CDSetName;
						break;
					case CCoverTextBox::kategorie:
						strText = pLayout->m_pccd->Category;
						break;
					case CCoverTextBox::medium:
						strText = pLayout->m_pccd->Medium;
						break;
					case CCoverTextBox::datum:
						strText = pLayout->m_pccd->Date;
						break;
					case CCoverTextBox::archivnummer:
						strText = pLayout->m_pccd->ArchiveNumber;
						break;
					case CCoverTextBox::codes:
						strText = pLayout->m_pccd->Codes;
						break;
					case CCoverTextBox::kommentar:
						strText = pLayout->m_pccd->Comment;
						break;
					case CCoverTextBox::aufnahmejahr:
						strText.Format(L"%d", pLayout->m_pccd->YearRecorded);
						break;
					case CCoverTextBox::copyright:
						strText = pLayout->m_pccd->Copyright;
						break;
					case CCoverTextBox::user0:
						strText = pLayout->m_pccd->UserField1;
						break;
					case CCoverTextBox::user1:
						strText = pLayout->m_pccd->UserField2;
						break;
					case CCoverTextBox::user2:
						strText = pLayout->m_pccd->UserField3;
						break;
					case CCoverTextBox::user3:
						strText = pLayout->m_pccd->UserField4;
						break;
					case CCoverTextBox::user4:
						strText = pLayout->m_pccd->UserField5;
						break;
				}
				if (strText != "")
					pTB->m_vecTextlines.push_back(strText);
			}

			const int nFontHeight = GetRealFontHeight(pDC, &CurrentTE.m_lfFont);

			CurrentTE.m_lfFont.lfEscapement = 0;
			CurrentTE.m_lfFont.lfOrientation = 0;

			CRect rr = pTB->m_rectArea;
			rr.NormalizeRect();
			const int nHeight = rr.Height();

			int nYExtend = 0;
			for (int nn = 0; nn != (int)pTB->m_vecTextlines.size(); nn++)
			{
				if (rr.Height() > nYExtend + nFontHeight)
				{
					nYExtend += nFontHeight;
				}
			}

			int y = 0;
			switch (CurrentTE.m_nYAlignment)
			{
				case CTextElement::center:
					y = ((nHeight - nYExtend) / 2);
					break;

				case CTextElement::bottom:
					y = nHeight - nYExtend;
			}

			int nYOffset = 0;

			if (! pTB->m_bTransparent)
			{
				CBrush brushBorder(pTB->m_colBack);
				CPen penBorder(PS_SOLID, 0, pTB->m_colBorder);
				CBrush *pOldBrush = pDC->SelectObject(&brushBorder);
				CPen *pOldPen = pDC->SelectObject(&penBorder);
				CRect rr(pTB->m_rectArea.left + m_nXPos + nXOffset, pTB->m_rectArea.top - m_nYPos, pTB->m_rectArea.right + m_nXPos + nXOffset, pTB->m_rectArea.bottom - m_nYPos);
				pDC->Rectangle(&rr);
				pDC->SelectObject(pOldPen);
				pDC->SelectObject(pOldBrush);
			}

			for (int n = 0; n != (int)pTB->m_vecTextlines.size(); n++)
			{
				CurrentTE.m_strText = pTB->m_vecTextlines[n];

				CurrentTE.m_nYpos = pTB->m_rectArea.top - m_nYPos - nYOffset - y;
				CurrentTE.m_nXpos = pTB->m_rectArea.left + m_nXPos + nXOffset;
				CSize sizeLastExtend;
				CurrentTE.m_strText = TextFitRectX(pDC, &CurrentTE, pTB->m_rectArea.Width(), &sizeLastExtend);
				CurrentTE.m_nXpos += AlignString(pDC, &CurrentTE, CurrentTE.m_nXAlignment, pTB->m_rectArea.Width(), &sizeLastExtend);

				if (abs(rr.Height()) - nYOffset > sizeLastExtend.cy && bTextGfx)
				{
					CurrentTE.Draw(pDC);
				}

				nYOffset += nFontHeight;
			}
		}
	}

	return;
}


CString CCover::GetStringFromColumn(CCoverColumn *pColumn, int t, Track^ CurrentTrack)
{
	CString strText;

	switch (pColumn->m_nType)
	{
		case CCoverColumn::TRACKNR:
			strText.Format(L"%02d", t + 1);
			break;
		case CCoverColumn::ARTIST:
			strText = CurrentTrack->Artist;
			break;
		case CCoverColumn::TITLE:
			strText = CurrentTrack->Title;
			break;
		case CCoverColumn::LENGTH:
			strText = CCover::TimeToString(CurrentTrack->Length);
			break;
		case CCoverColumn::SEPARATOR:
			strText = " - ";
			break;
		case CCoverColumn::BPM:
			strText.Format(L"%04d", CurrentTrack->Bpm);
			break;
		case CCoverColumn::COMMENT:
			strText = CurrentTrack->Comment;
			break;
		case CCoverColumn::USER1:
			strText = CurrentTrack->UserField1;
			break;
		case CCoverColumn::USER2:
			strText = CurrentTrack->UserField2;
			break;
		case CCoverColumn::USER3:
			strText = CurrentTrack->UserField3;
			break;
		case CCoverColumn::USER4:
			strText = CurrentTrack->UserField4;
			break;
		case CCoverColumn::USER5:
			strText = CurrentTrack->UserField5;
			break;
		case CCoverColumn::CODES:
			strText = CurrentTrack->Codes;
			break;
		case CCoverColumn::FREI:
			strText = pColumn->m_teColumn.m_strText;
			break;
		default:
			strText = "";
	}

	return strText;
}


CString CCover::TextFitRectX(CDC *pDC, CTextElement *pte, int nWidth, CSize *psizeExtend)
{
	CString str = pte->m_strText;

	CFont fnt;
	fnt.CreateFontIndirect(&pte->m_lfFont);
	CFont *pOldFont = pDC->SelectObject(&fnt);

	nWidth = (nWidth < 0) ? - nWidth : nWidth;

	int nTextLen = str.GetLength();

	CSize sizeTextExt = pDC->GetOutputTextExtent(str);

	while (sizeTextExt.cx > nWidth && nTextLen > 0)
	{
		nTextLen--;
		str = str.Left(nTextLen);
		str += "...";
		sizeTextExt = pDC->GetOutputTextExtent(str);
	}

	if (psizeExtend != NULL)
	{
		*psizeExtend = sizeTextExt;
	}

	pDC->SelectObject(pOldFont);

	return (nTextLen > 0) ? str : "";
}



int CCover::AlignString(CDC *pDC, CTextElement *pte, const int nMode, int nWidth, CSize *psizeExtend)
{
	CFont fnt;
	fnt.CreateFontIndirect(&pte->m_lfFont);
	CFont *pOldFont = pDC->SelectObject(&fnt);

	nWidth = (nWidth < 0) ? - nWidth : nWidth;

	CSize sizeTextExt = pDC->GetOutputTextExtent(pte->m_strText);

	int x = 0;

	switch (nMode)
	{
		case CTextElement::center:
			x = (nWidth - sizeTextExt.cx) / 2;
			break;

		case CTextElement::right:
		case CTextElement::top:
			x = nWidth - sizeTextExt.cx;
	}

	if (psizeExtend != NULL)
	{
		*psizeExtend = sizeTextExt;
	}

	pDC->SelectObject(pOldFont);

	return x;
}


int CCover::AlignStringY(CDC *pDC, CTextElement *pte, const int nMode, int nHeight, CSize *psizeExtend)
{
	CFont fnt;
	fnt.CreateFontIndirect(&pte->m_lfFont);
	CFont *pOldFont = pDC->SelectObject(&fnt);

	CSize sizeTextExt = pDC->GetOutputTextExtent(pte->m_strText);

	int y = 0;

	switch (nMode)
	{
		case CTextElement::center:
			y = ((nHeight - sizeTextExt.cy) / 2);
			break;

		case CTextElement::bottom:
			y = nHeight - sizeTextExt.cy;
	}

	if (psizeExtend != NULL)
	{
		*psizeExtend = sizeTextExt;
	}

	pDC->SelectObject(pOldFont);

	return y;
}


CString CCover::TimeToString(const long nMiliSek)
{
	CString strCurSek, strCurMin;

	long nSek = nMiliSek / 1000;
	const long nMin = nSek / 60;
	nSek -= nMin * 60;

	strCurMin.Format(L"%02d", nMin);
	strCurSek.Format(L"%02d", nSek);

	return (CString) ("(" + strCurMin + ":" + strCurSek + ")");
}


void CCover::DrawFrames(const BOOL bDrawFrames, CDC *pDC, const BOOL bFill)
{
	for (int i = 0; i != (int)m_vecFrames.size(); i++)
	{
		CBrush brushBorder;
		CBrush *pOldBrush;

		if (bFill)
		{
			brushBorder.CreateSolidBrush(m_vecFrames[i].m_colBackground);
		}
		else
		{
			LOGBRUSH lbrush;
			lbrush.lbStyle = BS_NULL;
			brushBorder.CreateBrushIndirect(&lbrush);
		}

		pOldBrush = pDC->SelectObject(&brushBorder);
		
		CPen penBorder(PS_SOLID, 0, bDrawFrames ? m_vecFrames[i].m_colBorder : m_vecFrames[i].m_colBackground);
		CPen *pOldPen = pDC->SelectObject(&penBorder);
		pDC->Rectangle(&(m_vecFrames[i].m_rectFrame));
		pDC->SelectObject(pOldPen);
		pDC->SelectObject(pOldBrush);
	}

	for (int i = 0; i != (int)m_vecCircles.size(); i++)
	{
		CBrush brushBorder;
		CBrush *pOldBrush;

		if (bFill)
		{
			brushBorder.CreateSolidBrush(m_vecCircles[i].m_colBackground);
		}
		else
		{
			LOGBRUSH lbrush;
			lbrush.lbStyle = BS_NULL;
			brushBorder.CreateBrushIndirect(&lbrush);
		}

		pOldBrush = pDC->SelectObject(&brushBorder);

		CPen penBorder(PS_SOLID, 0, bDrawFrames ? m_vecCircles[i].m_colBorder : m_vecCircles[i].m_colBackground);
		CPen *pOldPen = pDC->SelectObject(&penBorder);
		pDC->Ellipse(&(m_vecCircles[i].m_rectFrame));
		pDC->SelectObject(pOldPen);
		pDC->SelectObject(pOldBrush);
	}

	return;
}


void CCover::DrawTexts(CDC *pDC)
{
	for (int i = 0; i != (int)m_vecTextElements.size(); i++)
	{
		m_vecTextElements[i].Draw(pDC);
	}

	return;
}


/////////////////////////////////////////////////////////////////////////


CFrontCover::CFrontCover(CDC *pDC, const int XPos, const int YPos, const int nRandX, const int nRandY) : CCover(pDC, XPos, YPos, nRandX, nRandY)
{
}

CRect CFrontCover::GetArtistRect()
{
	return m_rectArtist;
}

CRect CFrontCover::GetTitleRect()
{
	return m_rectTitle;
}


CRect CFrontCover::GetFrontRect()
{
	return m_rectFront;
}

void CFrontCover::CreateCoverFromLayout(CCoverLayout *pLayout, CDC *pDC)
{
	BOOL bShowFrontTexts = TRUE;

	if (pLayout->m_bFrontGfx)
	{
		if (! pLayout->m_bTextGfxFront)
		{
			bShowFrontTexts = FALSE;
		}
	}

	m_rectFront = CRect(m_nXPos, -m_nYPos, m_nXPos + pLayout->gl_COVERFRONTWIDTH, -m_nYPos - pLayout->gl_COVERFRONTHEIGHT);

	m_vecTextElements.clear();

	m_rectArtist = CRect(0,0,0,0);
	m_rectTitle = CRect(0,0,0,0);

	if (pLayout->m_bShowFrontArtist)
	{
		CSize Extend;
		CTextElement te = pLayout->m_teArtist;
		te.m_strText = TextFitRectX(pDC, &te, pLayout->gl_COVERFRONTWIDTH - 2 * 50, &Extend);

		if (te.m_nXAlignment == CTextElement::userdef)
		{
			te.m_nXpos = pLayout->m_nArtistFrontXFreeValue * 10;

			if (te.m_nXpos + Extend.cx + 100 > pLayout->gl_COVERFRONTWIDTH)
			{
				te.m_nXpos = pLayout->gl_COVERFRONTWIDTH - 2 * 50 - Extend.cx;
				pLayout->m_nArtistFrontXFreeValue = te.m_nXpos / 10;
			}
		}
		else
		{
			te.m_nXpos = AlignString(pDC, &te, te.m_nXAlignment, pLayout->gl_COVERFRONTWIDTH - 2 * 50);
			pLayout->m_nArtistFrontXFreeValue = te.m_nXpos / 10;
		}

		if (te.m_nYAlignment == CTextElement::userdef)
		{
			te.m_nYpos = pLayout->m_nArtistFrontYFreeValue * 10;

			if (te.m_nYpos + Extend.cy + 100 > pLayout->gl_COVERFRONTHEIGHT)
			{
				te.m_nYpos = pLayout->gl_COVERFRONTHEIGHT - 2 * 50 - Extend.cy;
				pLayout->m_nArtistFrontYFreeValue = te.m_nYpos / 10;
			}
		}
		else
		{
			te.m_nYpos = AlignStringY(pDC, &te, te.m_nYAlignment, pLayout->gl_COVERFRONTHEIGHT - 2 * 50, &Extend);
			pLayout->m_nArtistFrontYFreeValue = te.m_nYpos / 10;
		}

		te.m_nXpos += m_nXPos + 50;
		te.m_nYpos = -te.m_nYpos + -m_nYPos - 50;

		if (bShowFrontTexts)
		{
			m_vecTextElements.push_back(te);
		}

		m_rectArtist = CRect(te.m_nXpos, te.m_nYpos, te.m_nXpos + Extend.cx, te.m_nYpos - Extend.cy);
	}

	if (pLayout->m_bShowFrontCDTitle)
	{
		CSize Extend;
		CTextElement te = pLayout->m_teCDTitle;
		te.m_strText = TextFitRectX(pDC, &te, pLayout->gl_COVERFRONTWIDTH - 2 * 50, NULL);
		if (te.m_nXAlignment == CTextElement::userdef)
		{
			te.m_nXpos = pLayout->m_nCDTitleFrontXFreeValue * 10;

			if (te.m_nXpos + Extend.cx + 100 > pLayout->gl_COVERFRONTWIDTH)
			{
				te.m_nXpos = pLayout->gl_COVERFRONTWIDTH - 2 * 50 - Extend.cx;
				pLayout->m_nCDTitleFrontXFreeValue = te.m_nXpos / 10;
			}
		}
		else
		{
			te.m_nXpos = AlignString(pDC, &te, te.m_nXAlignment, pLayout->gl_COVERFRONTWIDTH - 2 * 50, NULL);
			pLayout->m_nCDTitleFrontXFreeValue = te.m_nXpos / 10;
		}

		if (te.m_nYAlignment == CTextElement::userdef)
		{
			te.m_nYpos = pLayout->m_nCDTitleFrontYFreeValue * 10;

			if (te.m_nYpos + Extend.cy + 100 > pLayout->gl_COVERFRONTHEIGHT)
			{
				te.m_nYpos = pLayout->gl_COVERFRONTHEIGHT - 2 * 50 - Extend.cy;
				pLayout->m_nCDTitleFrontYFreeValue = te.m_nYpos / 10;
			}
		}
		else
		{
			te.m_nYpos = AlignStringY(pDC, &te, te.m_nYAlignment, pLayout->gl_COVERFRONTHEIGHT - 2 * 50, &Extend);
			pLayout->m_nCDTitleFrontYFreeValue = te.m_nYpos / 10;
		}

		te.m_nXpos += m_nXPos + 50;
		te.m_nYpos = -te.m_nYpos + -m_nYPos - 50;
		if (bShowFrontTexts)
		{
			m_vecTextElements.push_back(te);
		}
		m_rectTitle = CRect(te.m_nXpos, te.m_nYpos, te.m_nXpos + Extend.cx, te.m_nYpos - Extend.cy);
	}

	return;
}

void CFrontCover::Draw(CCoverLayout *pLayout, CDC *pDC)
{
	CCoverFrame frame;

	frame.m_colBackground = pLayout->m_colFront;
	frame.m_colBorder = pLayout->m_colFrontRand;
	frame.m_rectFrame = CRect(m_nXPos, -m_nYPos, m_nXPos + pLayout->gl_COVERFRONTWIDTH, -m_nYPos - pLayout->gl_COVERFRONTHEIGHT);
	m_vecFrames.clear();
	m_vecFrames.push_back(frame);

	DrawFrames(pLayout->m_bDrawBorders, pDC);

	if (pLayout->m_bFrontGfx)
	{
		CRect GfxRect = frame.m_rectFrame;
		GfxRect.right++;
		GfxRect.bottom--;
		if (pLayout->m_gfxFront.Init(pLayout->m_strFrontGfx))
			pLayout->m_gfxFront.Draw(&GfxRect, pDC);
	}

	DrawFrames(pLayout->m_bDrawBorders, pDC, FALSE);

	DrawTexts(pDC);
	DrawTB(pLayout, &pLayout->m_tbsF, pLayout->m_bTextGfxFront, 0, pDC);

	return;
}


/////////////////////////////////////////////////////////////////////////


CBackCover::CBackCover(CDC *pDC, const int XPos, const int YPos, const int nRandX, const int nRandY) : CCover(pDC, XPos, YPos, nRandX, nRandY)
{
}

void CBackCover::Draw(CCoverLayout *pLayout, CDC *pDC)
{
	m_vecFrames.clear();

	CCoverFrame frame;
	frame.m_colBackground = pLayout->m_colBorderLeftBack;
	frame.m_colBorder = pLayout->m_colBorderLeftRand;
	frame.m_rectFrame = m_rectLeft;
	m_vecFrames.push_back(frame);

	frame.m_colBackground = pLayout->m_colBack;
	frame.m_colBorder = pLayout->m_colBackRand;
	frame.m_rectFrame = m_rectBack;
	m_vecFrames.push_back(frame);

	frame.m_colBackground = pLayout->m_colBorderRightBack;
	frame.m_colBorder = pLayout->m_colBorderRightRand;
	frame.m_rectFrame = m_rectRight;
	m_vecFrames.push_back(frame);

	DrawFrames(pLayout->m_bDrawBorders, pDC);

	if (pLayout->m_bBackGfx)
	{
		CRect rect = m_rectBack;

		if (pLayout->m_bGfxBackBorder)
		{
			rect.left = m_rectLeft.left;
			rect.right = m_rectRight.right;
		}

		CRect GfxRect = rect;
		GfxRect.right++;
		GfxRect.bottom--;

		if (pLayout->m_gfxBack.Init(pLayout->m_strBackGfx))
			pLayout->m_gfxBack.Draw(&GfxRect, pDC);
	}

	DrawFrames(pLayout->m_bDrawBorders, pDC, FALSE);

	DrawTexts(pDC);
	DrawTB(pLayout, &pLayout->m_tbsB, pLayout->m_bTextGfxBack, pLayout->gl_COVERBORDERWIDTH, pDC);

	return;
}

CRect CBackCover::GetBackRect()
{
	return m_rectBack;
}
CRect CBackCover::GetLeftRect()
{
	return m_rectLeft;
}
CRect CBackCover::GetRightRect()
{
	return m_rectRight;
}


void CBackCover::CreateCoverFromLayout(CCoverLayout *pLayout, CDC *pDC)
{
	BOOL bShowBorderTexts = TRUE;
	BOOL bShowBackTexts = TRUE;

	if (pLayout->m_bBackGfx)
	{
		if (pLayout->m_bGfxBackBorder && ! pLayout->m_bTextGfxBack)
		{
			bShowBorderTexts = FALSE;
			bShowBackTexts = FALSE;
		}
		if (! pLayout->m_bGfxBackBorder && ! pLayout->m_bTextGfxBack)
		{
			bShowBackTexts = FALSE;
		}
	}

	m_rectBack = CRect(m_nXPos + pLayout->gl_COVERBORDERWIDTH, -m_nYPos, m_nXPos + pLayout->gl_COVERBACKWIDTH + pLayout->gl_COVERBORDERWIDTH, -m_nYPos - pLayout->gl_COVERBACKHEIGHT);
	m_rectLeft = CRect(m_nXPos, -m_nYPos, m_nXPos + pLayout->gl_COVERBORDERWIDTH, -m_nYPos - pLayout->gl_COVERBACKHEIGHT);
	m_rectRight = CRect(m_nXPos + pLayout->gl_COVERBACKWIDTH + pLayout->gl_COVERBORDERWIDTH, -m_nYPos, m_nXPos + pLayout->gl_COVERBORDERWIDTH + pLayout->gl_COVERBACKWIDTH + pLayout->gl_COVERBORDERWIDTH, -m_nYPos - pLayout->gl_COVERBACKHEIGHT);

	m_vecTextElements.clear();

	CTextElement tel, ter;

	tel = pLayout->m_teBorderLeft;
	ter = pLayout->m_teBorderRight;

	if (pLayout->m_bLeftBorder_AT)
	{
		tel.m_strText = pLayout->m_teArtist.m_strText + pLayout->m_strTrenn + pLayout->m_teCDTitle.m_strText;
	}
	if (pLayout->m_bRightBorder_AT)
	{
		ter.m_strText = pLayout->m_teArtist.m_strText + pLayout->m_strTrenn + pLayout->m_teCDTitle.m_strText;
	}

	if (pLayout->m_bJoinBorderTexts)
	{
		if (pLayout->m_bLeftBorder_Dominate)
		{
			ter.m_strText = tel.m_strText;
		}
		if (pLayout->m_bRightBorder_Dominate)
		{
			tel.m_strText = ter.m_strText;
		}
	}

	tel.m_lfFont.lfHeight = 40;
	tel.m_lfFont.lfWidth = 0;
	tel.m_nXpos = m_nXPos + ((pLayout->gl_COVERBORDERWIDTH - 40) / 2);
	tel.m_nYpos = -m_nYPos - pLayout->gl_COVERBACKHEIGHT + 50;
	tel.m_lfFont.lfEscapement = -900;
	tel.m_lfFont.lfOrientation = -900;
	tel.m_strText = TextFitRectX(pDC, &tel, pLayout->gl_COVERBACKHEIGHT - 2 * 50);
	tel.m_nYpos += AlignString(pDC, &tel, tel.m_nYAlignment, pLayout->gl_COVERBACKHEIGHT - 2 * 50);
	if (bShowBackTexts)
	{
		m_vecTextElements.push_back(tel);
	}

	ter.m_lfFont.lfHeight = 40;
	ter.m_lfFont.lfWidth = 0;
	ter.m_nXpos = m_nXPos + pLayout->gl_COVERBORDERWIDTH + pLayout->gl_COVERBORDERWIDTH + pLayout->gl_COVERBACKWIDTH - ((pLayout->gl_COVERBORDERWIDTH - 40) / 2);
	ter.m_nYpos = -m_nYPos - 50;
	ter.m_lfFont.lfEscapement = 900;
	ter.m_lfFont.lfOrientation = 900;
	ter.m_strText = TextFitRectX(pDC, &ter, pLayout->gl_COVERBACKHEIGHT - 2 * 50);
	int nYAlign = CTextElement::center;
	if (ter.m_nYAlignment == CTextElement::top)
	{
		nYAlign = CTextElement::bottom;
	}
	if (ter.m_nYAlignment == CTextElement::bottom)
	{
		nYAlign = CTextElement::top;
	}
	ter.m_nYpos -= AlignString(pDC, &ter, nYAlign, pLayout->gl_COVERBACKHEIGHT - 2 * 50);
	if (bShowBackTexts)
	{
		m_vecTextElements.push_back(ter);
	}

	int nTracks = pLayout->m_pccd->NumberOfTracks;

	double dFontHeight = 50.0;

	if (nTracks > 20)
	{
		dFontHeight = ::abs(m_rectBack.Height());
		dFontHeight -= 2 * 50;
		dFontHeight -= 4 * (nTracks - 1);
		dFontHeight /= nTracks;
	}

	int nYPos = m_rectBack.top - 50;

	const int nSpace = (int) (dFontHeight + 0.5) * nTracks + 4 * (nTracks - 1) + m_rectBack.Height() + 2 * 50;

	switch (pLayout->m_nBackYAlignment)
	{
		case CTextElement::bottom:
			nYPos += nSpace;
			break;
		case CTextElement::center:
			nYPos += nSpace / 2;
	}

	for (int t = 0; t != nTracks; t++)
	{
		if (abs(nYPos) > m_rectBack.bottom + 50 + (int)(dFontHeight + 0.5))
		{
			Track^ CurrentTrack = pLayout->m_pccd->Tracks[t];

			int x = 0;
			if (pLayout->m_nBackXAlignment != CTextElement::left)
			{
				int nColumnWidth = DrawColumn(FALSE, nYPos, CurrentTrack, t, dFontHeight, pLayout, 0, pDC);
				switch (pLayout->m_nBackXAlignment)
				{
					case CTextElement::center:
						x = (pLayout->gl_COVERBACKWIDTH - 100 - nColumnWidth) / 2;
						break;
					case CTextElement::right:
						x = pLayout->gl_COVERBACKWIDTH - 100 - nColumnWidth;
				}
			}

			DrawColumn(bShowBackTexts, nYPos, CurrentTrack, t, dFontHeight, pLayout, x, pDC);

			nYPos -= (int) (dFontHeight + 0.5) + 4;
		}
	}

	return;
}

int CBackCover::DrawColumn(const BOOL bDraw, const int nYPos, Track^ CurrentTrack, const int t, const double dFontHeight, CCoverLayout *pLayout, const int nXOffset, CDC *pDC)
{
	int nColumnLeft = m_rectBack.left + 50;
	int nColumnRight = nColumnLeft;


	int nXPosColumns = m_rectBack.left + 50;

	int c = 0;
	while (c < (int)pLayout->m_vecColumns.size())
	{
		// aktuelles Column TextElement holen
		CCoverColumn *pCurrentColumn = &(pLayout->m_vecColumns[c]);
		CTextElement CurrentTE = pCurrentColumn->m_teColumn;
		CurrentTE.m_strText = GetStringFromColumn(&pLayout->m_vecColumns[c], t, CurrentTrack);
		CurrentTE.m_lfFont.lfHeight = (int) (dFontHeight + 0.5);
		CurrentTE.m_lfFont.lfWidth = 0;
		CurrentTE.m_lfFont.lfEscapement = 0;
		CurrentTE.m_lfFont.lfOrientation = 0;
		CurrentTE.m_nYpos = nYPos;

		// Existiert ein HOOK ?

		BOOL bHook = FALSE;
		if (c < (int)pLayout->m_vecColumns.size() - 1)
		{
			bHook = (pLayout->m_vecColumns[c + 1].m_teColumn.m_nXAlignment == CTextElement::hook);
		}

		// Breite der Hook Area feststellen

		int hStart = c;
		int hStop = c;
		int cc = c;
		int nXPosHookStart = nXPosColumns;
		int nXPosHookEnd = nXPosColumns;
		BOOL bAtLeastOneHook = FALSE;
		while (bHook && cc != (int)pLayout->m_vecColumns.size())
		{
			int nColumnAbsWidth = (int) (0.5 + (pCurrentColumn->m_dWidthRel * (double) (m_rectBack.Width() - 2 * 50)));
			nXPosHookEnd += nColumnAbsWidth;

			cc++;
			if (cc < (int)pLayout->m_vecColumns.size())
			{
				pCurrentColumn = &(pLayout->m_vecColumns[cc]);
				CurrentTE = pCurrentColumn->m_teColumn;
				bHook = (CurrentTE.m_nXAlignment == CTextElement::hook);
			}

			bAtLeastOneHook = TRUE;
			hStop = cc;
			nXPosColumns += nColumnAbsWidth;
		}

		// Und HOOK gesondert bearbeiten !
		if (bAtLeastOneHook)
		{
			c = cc;
			int nRestWidth = nXPosHookEnd - nXPosHookStart;
			int nWidth = 0;
			while (hStart < hStop)
			{
				pCurrentColumn = &(pLayout->m_vecColumns[hStart]);
				CurrentTE = pCurrentColumn->m_teColumn;
				CurrentTE.m_strText = GetStringFromColumn(&pLayout->m_vecColumns[hStart], t, CurrentTrack);
				CurrentTE.m_lfFont.lfHeight = (int) (dFontHeight + 0.5);
				CurrentTE.m_lfFont.lfWidth = 0;
				CurrentTE.m_lfFont.lfEscapement = 0;
				CurrentTE.m_lfFont.lfOrientation = 0;
				CurrentTE.m_nYpos = nYPos;

				CSize sizeExt;
				CurrentTE.m_strText = TextFitRectX(pDC, &CurrentTE, nRestWidth, &sizeExt);
				nRestWidth -= sizeExt.cx - 25;
				CurrentTE.m_nXpos = nXPosHookStart + nWidth;
				nWidth += sizeExt.cx + 25;
				nColumnRight = CurrentTE.m_nXpos + sizeExt.cx;

				if ((CurrentTE.m_nXpos + nXOffset >= m_rectBack.left + 50)  && (CurrentTE.m_nXpos + sizeExt.cx <= m_rectBack.right - 50) && bDraw)
				{
					CurrentTE.m_nXpos += nXOffset;
					m_vecTextElements.push_back(CurrentTE);
				}

				hStart++;
			}
		}

		if (c < (int)pLayout->m_vecColumns.size())
		{
			pCurrentColumn = &(pLayout->m_vecColumns[c]);
			CurrentTE = pCurrentColumn->m_teColumn;
			CurrentTE.m_strText = GetStringFromColumn(&pLayout->m_vecColumns[c], t, CurrentTrack);
			CurrentTE.m_lfFont.lfHeight = (int) (dFontHeight + 0.5);
			CurrentTE.m_lfFont.lfWidth = 0;
			CurrentTE.m_lfFont.lfEscapement = 0;
			CurrentTE.m_lfFont.lfOrientation = 0;
			CurrentTE.m_nYpos = nYPos;
			CurrentTE.m_nXpos = nXPosColumns;
			CSize sizeExt;
			int nColumnAbsWidth = (int) (0.5 + (pCurrentColumn->m_dWidthRel * (double) (m_rectBack.Width() - 2 * 50)));
			CurrentTE.m_strText = TextFitRectX(pDC, &CurrentTE, nColumnAbsWidth, &sizeExt);
			CurrentTE.m_nXpos += AlignString(pDC, &CurrentTE, CurrentTE.m_nXAlignment, nColumnAbsWidth, &sizeExt);
			nColumnRight = CurrentTE.m_nXpos + sizeExt.cx;

			if ((CurrentTE.m_nXpos + nXOffset >= m_rectBack.left + 50)  && (CurrentTE.m_nXpos + sizeExt.cx <= m_rectBack.right - 50) && bDraw)
			{
				CurrentTE.m_nXpos += nXOffset;
				m_vecTextElements.push_back(CurrentTE);
			}

			nXPosColumns += nColumnAbsWidth;
		}

		c++;
	}

	return nColumnRight - nColumnLeft;
}

/////////////////////////////////////////////////////////////////////////


void CLabelCover::Draw(CCoverLayout *pLayout, CDC *pDC)
{
	m_vecCircles.clear();

	CCoverFrame frame;

	frame.m_colBackground = pLayout->m_colLabel;
	frame.m_colBorder = pLayout->m_colLabelRand;
	frame.m_rectFrame = CRect(m_nXPos, -m_nYPos, m_nXPos + pLayout->gl_COVERLABELWIDTH, -m_nYPos - pLayout->gl_COVERLABELHEIGHT);
	m_vecCircles.push_back(frame);
	frame.m_rectFrame = CRect(m_nXPos + ((pLayout->gl_COVERLABELWIDTH-pLayout->gl_COVERINNERLABELRADIUS)/2), -m_nYPos - ((pLayout->gl_COVERLABELWIDTH-pLayout->gl_COVERINNERLABELRADIUS)/2), m_nXPos + pLayout->gl_COVERLABELWIDTH - ((pLayout->gl_COVERLABELWIDTH-pLayout->gl_COVERINNERLABELRADIUS)/2), -m_nYPos - pLayout->gl_COVERLABELHEIGHT + ((pLayout->gl_COVERLABELWIDTH-pLayout->gl_COVERINNERLABELRADIUS)/2));
	frame.m_colBackground = pLayout->m_colLabel;
	m_vecCircles.push_back(frame);

	DrawFrames(pLayout->m_bDrawBorders, pDC);

	if (pLayout->m_bLabelGfx)
	{
		CRect GfxRect = CRect(m_nXPos, -m_nYPos, m_nXPos + pLayout->gl_COVERLABELWIDTH, -m_nYPos - pLayout->gl_COVERLABELHEIGHT);
		GfxRect.right++;
		GfxRect.bottom--;
		if (pLayout->m_gfxLabel.Init(pLayout->m_strLabelGfx))
			pLayout->m_gfxLabel.Draw(&GfxRect, pDC);
	}

	DrawFrames(pLayout->m_bDrawBorders, pDC, FALSE);

	DrawTexts(pDC);
	DrawTB(pLayout, &pLayout->m_tbsL, pLayout->m_bTextGfxLabel, 0, pDC);

	return;
}

CLabelCover::CLabelCover(CDC *pDC, const int XPos, const int YPos, const int nRandX, const int nRandY) : CCover(pDC, XPos, YPos, nRandX, nRandY)
{
}

void CLabelCover::CreateCoverFromLayout(CCoverLayout *pLayout)
{
	BOOL bShowBackTexts = TRUE;

	if (pLayout->m_bLabelGfx)
	{
		if (! pLayout->m_bTextGfxLabel)
		{
			bShowBackTexts = FALSE;
		}
	}

	m_rectLabel = CRect(m_nXPos, -m_nYPos, m_nXPos + pLayout->gl_COVERLABELWIDTH, -m_nYPos - pLayout->gl_COVERLABELHEIGHT);
	m_rectInnerLabel = CRect(m_nXPos + ((pLayout->gl_COVERLABELWIDTH-pLayout->gl_COVERINNERLABELRADIUS)/2), -m_nYPos - ((pLayout->gl_COVERLABELWIDTH-pLayout->gl_COVERINNERLABELRADIUS)/2), m_nXPos + pLayout->gl_COVERLABELWIDTH - ((pLayout->gl_COVERLABELWIDTH-pLayout->gl_COVERINNERLABELRADIUS)/2), -m_nYPos - pLayout->gl_COVERLABELHEIGHT + ((pLayout->gl_COVERLABELWIDTH-pLayout->gl_COVERINNERLABELRADIUS)/2));

	m_vecTextElements.clear();

	return;
}

CRect CLabelCover::GetLabelRect()
{
	return m_rectLabel;
}
CRect CLabelCover::GetInnerLabelRect()
{
	return m_rectInnerLabel;
}

#include "stdafx.h"
#include "textelement.h"

#define COVERTEXTELEMENT_ALIGN_LEFT		0
#define COVERTEXTELEMENT_ALIGN_RIGHT	1
#define COVERTEXTELEMENT_ALIGN_CENTER	2
#define COVERTEXTELEMENT_ALIGN_TOP		3
#define COVERTEXTELEMENT_ALIGN_BOTTOM	4
#define COVERTEXTELEMENT_ALIGN_FREE		5



CTextElement::CTextElement()
{
	m_strText = "";

	m_nXpos = 0;
	m_nYpos = 0;

	m_nBkMode = OPAQUE;

	m_nXAlignment = CTextElement::left;
	m_nYAlignment = CTextElement::top;

	m_colText = (COLORREF) 0x00000000;
	m_colTextbk = (COLORREF) 0x00ffffff;

	m_lfFont.lfHeight = 32;
	m_lfFont.lfWidth = 0;
	m_lfFont.lfEscapement = 0;
	m_lfFont.lfOrientation = 0;
	m_lfFont.lfWeight = FW_NORMAL;
	m_lfFont.lfItalic = FALSE;
	m_lfFont.lfUnderline = FALSE;
	m_lfFont.lfStrikeOut = FALSE;
	m_lfFont.lfCharSet = ANSI_CHARSET;
	m_lfFont.lfOutPrecision = OUT_DEFAULT_PRECIS;
	m_lfFont.lfClipPrecision = CLIP_DEFAULT_PRECIS;
	m_lfFont.lfQuality = DEFAULT_QUALITY;
	m_lfFont.lfPitchAndFamily = DEFAULT_PITCH | FF_DONTCARE | FF_DONTCARE;

	wcscpy(m_lfFont.lfFaceName, L"Times New Roman");
}


CTextElement::CTextElement(const CTextElement& te)
{
	m_strText = te.m_strText;
	m_nXpos = te.m_nXpos;
	m_nYpos = te.m_nYpos;
	m_nBkMode = te.m_nBkMode;
	m_colText = te.m_colText;
	m_colTextbk = te.m_colTextbk;
	m_nXAlignment = te.m_nXAlignment;
	m_nYAlignment = te.m_nYAlignment;


	m_lfFont.lfHeight = te.m_lfFont.lfHeight;
	m_lfFont.lfWidth = te.m_lfFont.lfWidth;
	m_lfFont.lfEscapement = te.m_lfFont.lfEscapement;
	m_lfFont.lfOrientation = te.m_lfFont.lfOrientation;
	m_lfFont.lfWeight = te.m_lfFont.lfWeight;
	m_lfFont.lfItalic = te.m_lfFont.lfItalic;
	m_lfFont.lfUnderline = te.m_lfFont.lfUnderline;
	m_lfFont.lfStrikeOut = te.m_lfFont.lfStrikeOut;
	m_lfFont.lfCharSet = te.m_lfFont.lfCharSet;
	m_lfFont.lfOutPrecision = te.m_lfFont.lfOutPrecision;
	m_lfFont.lfClipPrecision = te.m_lfFont.lfClipPrecision;
	m_lfFont.lfQuality = te.m_lfFont.lfQuality;
	m_lfFont.lfPitchAndFamily = te.m_lfFont.lfPitchAndFamily;
	wcscpy(m_lfFont.lfFaceName, te.m_lfFont.lfFaceName);
}

CTextElement::CTextElement(const CString& strText)
{
	CTextElement();

	m_strText = strText;
}



bool CTextElement::operator== (const CTextElement& te)
{
	bool bReturn = TRUE;

	bReturn &= (te.m_strText == m_strText);
	bReturn &= (te.m_nXpos == m_nXpos);
	bReturn &= (te.m_nYpos == m_nYpos);
	bReturn &= (te.m_nBkMode == m_nBkMode);
	bReturn &= (te.m_colText == m_colText);
	bReturn &= (te.m_colTextbk == m_colTextbk);
	bReturn &= (te.m_nXAlignment == m_nXAlignment);
	bReturn &= (te.m_nYAlignment == m_nYAlignment);

	bReturn &= (te.m_lfFont.lfHeight == m_lfFont.lfHeight);
	bReturn &= (te.m_lfFont.lfWidth == m_lfFont.lfWidth);
	bReturn &= (te.m_lfFont.lfEscapement == m_lfFont.lfEscapement);
	bReturn &= (te.m_lfFont.lfOrientation == m_lfFont.lfOrientation);
	bReturn &= (te.m_lfFont.lfWeight == m_lfFont.lfWeight);
	bReturn &= (te.m_lfFont.lfItalic == m_lfFont.lfItalic);
	bReturn &= (te.m_lfFont.lfUnderline == m_lfFont.lfUnderline);
	bReturn &= (te.m_lfFont.lfStrikeOut == m_lfFont.lfStrikeOut);
	bReturn &= (te.m_lfFont.lfCharSet == m_lfFont.lfCharSet);
	bReturn &= (te.m_lfFont.lfOutPrecision == m_lfFont.lfOutPrecision);
	bReturn &= (te.m_lfFont.lfClipPrecision == m_lfFont.lfClipPrecision);
	bReturn &= (te.m_lfFont.lfQuality == m_lfFont.lfQuality);
	bReturn &= (te.m_lfFont.lfPitchAndFamily == m_lfFont.lfPitchAndFamily);
	const CString f1 = CString(te.m_lfFont.lfFaceName);
	const CString f2 = CString(m_lfFont.lfFaceName);
	bReturn &= (f1.CompareNoCase(f2) == 0);

	return bReturn;
}



CTextElement& CTextElement::operator= (const CTextElement& te)
{
	m_strText = te.m_strText;
	m_nXpos = te.m_nXpos;
	m_nYpos = te.m_nYpos;
	m_nBkMode = te.m_nBkMode;
	m_colText = te.m_colText;
	m_colTextbk = te.m_colTextbk;
	m_nXAlignment = te.m_nXAlignment;
	m_nYAlignment = te.m_nYAlignment;

	m_lfFont.lfHeight = te.m_lfFont.lfHeight;
	m_lfFont.lfWidth = te.m_lfFont.lfWidth;
	m_lfFont.lfEscapement = te.m_lfFont.lfEscapement;
	m_lfFont.lfOrientation = te.m_lfFont.lfOrientation;
	m_lfFont.lfWeight = te.m_lfFont.lfWeight;
	m_lfFont.lfItalic = te.m_lfFont.lfItalic;
	m_lfFont.lfUnderline = te.m_lfFont.lfUnderline;
	m_lfFont.lfStrikeOut = te.m_lfFont.lfStrikeOut;
	m_lfFont.lfCharSet = te.m_lfFont.lfCharSet;
	m_lfFont.lfOutPrecision = te.m_lfFont.lfOutPrecision;
	m_lfFont.lfClipPrecision = te.m_lfFont.lfClipPrecision;
	m_lfFont.lfQuality = te.m_lfFont.lfQuality;
	m_lfFont.lfPitchAndFamily = te.m_lfFont.lfPitchAndFamily;
	wcscpy(m_lfFont.lfFaceName, te.m_lfFont.lfFaceName);
	
	return *this;
}


void CTextElement::Draw(CDC *pDC)
{
	m_lfFont.lfClipPrecision |= CLIP_LH_ANGLES;

	const int nOldBkMode = pDC->GetBkMode();

	CFont *pOldFont = pDC->GetCurrentFont();
	COLORREF colOldBk = pDC->GetBkColor();
	COLORREF colOldText = pDC->GetTextColor();

	CFont ftFont;
	ftFont.CreateFontIndirect(&m_lfFont);
	pDC->SelectObject(&ftFont);
	pDC->SetTextAlign(TA_LEFT | TA_TOP | TA_NOUPDATECP);
	pDC->SetTextColor(m_colText);
	pDC->SetBkColor(m_colTextbk);
	pDC->SetBkMode(m_nBkMode);
	pDC->TextOut(m_nXpos, m_nYpos, m_strText);
	pDC->SetBkMode(nOldBkMode);
	pDC->SelectObject(pOldFont);
	pDC->SetBkColor(colOldBk);
	pDC->SetTextColor(colOldText);

	return;
}


BOOL CTextElement::SaveLOGFONT(CArchive& ar, LOGFONT *plf)
{
	BOOL bReturn = FALSE;
	try
	{
		ar << plf->lfHeight;
		ar << plf->lfWidth;
		ar << plf->lfEscapement;
		ar << plf->lfOrientation;
		ar << plf->lfWeight;
		ar << plf->lfItalic;
		ar << plf->lfUnderline;
		ar << plf->lfStrikeOut;
		ar << plf->lfCharSet;
		ar << plf->lfOutPrecision;
		ar << plf->lfClipPrecision;
		ar << plf->lfQuality;
		ar << plf->lfPitchAndFamily;

		const CString strFaceName(plf->lfFaceName);
		ar << strFaceName;

		bReturn = TRUE;

	}
	catch(CException *e)
	{
		e->Delete();
	}

	return bReturn;
}


BOOL CTextElement::LoadLOGFONT(CArchive& ar, LOGFONT *plf)
{
	BOOL bReturn = FALSE;
	try
	{
		ar >> plf->lfHeight;
		ar >> plf->lfWidth;
		ar >> plf->lfEscapement;
		ar >> plf->lfOrientation;
		ar >> plf->lfWeight;
		ar >> plf->lfItalic;
		ar >> plf->lfUnderline;
		ar >> plf->lfStrikeOut;
		ar >> plf->lfCharSet;
		ar >> plf->lfOutPrecision;
		ar >> plf->lfClipPrecision;
		ar >> plf->lfQuality;
		ar >> plf->lfPitchAndFamily;

		CString strFaceName;
		ar >> strFaceName;
		wcscpy(plf->lfFaceName, strFaceName);

		bReturn = TRUE;
	}
	catch(CException *e)
	{
		e->Delete();
	}

	return bReturn;
}


BOOL CTextElement::Load(CArchive& ar)
{
	BOOL bReturn = FALSE;

	try
	{
		ar >> m_nXAlignment;
		ar >> m_nYAlignment;
		ar >> m_nXpos;
		ar >> m_nYpos;
		ar >> m_colTextbk;
		ar >> m_colText;
		ar >> m_nBkMode;
		bReturn = TRUE;
	}
	catch(CException *e)
	{
		e->Delete();
	}

	bReturn &= LoadLOGFONT(ar, &m_lfFont);

	return bReturn;
}


BOOL CTextElement::Save(CArchive& ar)
{
	BOOL bReturn = FALSE;

	try
	{
		ar << m_nXAlignment;
		ar << m_nYAlignment;
		ar << m_nXpos;
		ar << m_nYpos;
		ar << m_colTextbk;
		ar << m_colText;
		ar << m_nBkMode;
		bReturn = TRUE;
	}
	catch(CException *e)
	{
		e->Delete();
	}

	bReturn &= SaveLOGFONT(ar, &m_lfFont);

	return bReturn;
}



BOOL CTextElement::OldLoad(CArchive& ar)
{
	BOOL bReturn = FALSE;

	try
	{
		int nDummy;
		ar >> m_nXAlignment;
		ar >> m_nYAlignment;
		ar >> nDummy; // m_nFreeXpos;
		ar >> nDummy; // m_nFreeYpos;
		ar >> m_colTextbk;
		ar >> m_colText;
		ar >> m_nBkMode;
		bReturn = TRUE;
	}
	catch(CException *e)
	{
		e->Delete();
	}

	switch (m_nXAlignment)
	{
		case COVERTEXTELEMENT_ALIGN_LEFT:
			m_nXAlignment =CTextElement::left;
			break;
		case COVERTEXTELEMENT_ALIGN_RIGHT:
			m_nXAlignment =CTextElement::right;
			break;
		case COVERTEXTELEMENT_ALIGN_CENTER:
			m_nXAlignment =CTextElement::center;
			break;
		case COVERTEXTELEMENT_ALIGN_TOP:
			m_nXAlignment =CTextElement::top;
			break;
		case COVERTEXTELEMENT_ALIGN_BOTTOM:
			m_nXAlignment =CTextElement::bottom;
			break;
		case COVERTEXTELEMENT_ALIGN_FREE:
			m_nXAlignment =CTextElement::userdef;
	}
	switch (m_nYAlignment)
	{
		case COVERTEXTELEMENT_ALIGN_LEFT:
			m_nYAlignment =CTextElement::left;
			break;
		case COVERTEXTELEMENT_ALIGN_RIGHT:
			m_nYAlignment =CTextElement::right;
			break;
		case COVERTEXTELEMENT_ALIGN_CENTER:
			m_nYAlignment =CTextElement::center;
			break;
		case COVERTEXTELEMENT_ALIGN_TOP:
			m_nYAlignment =CTextElement::top;
			break;
		case COVERTEXTELEMENT_ALIGN_BOTTOM:
			m_nYAlignment =CTextElement::bottom;
			break;
		case COVERTEXTELEMENT_ALIGN_FREE:
			m_nYAlignment =CTextElement::userdef;
	}

	bReturn &= LoadLOGFONT(ar, &m_lfFont);

	return bReturn;
}

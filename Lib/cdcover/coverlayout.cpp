#include "stdafx.h"
#include "textelement.h"
#include "coverlayout.h"
#include "cover.h"
#include "resource.h"
#include "../hitmisc/newfiledialog.h"

#define CCOVERTEXTBOX_VERSION 1

CCoverTextBox::~CCoverTextBox()
{
}

CCoverTextBox::CCoverTextBox(const CCoverTextBox& tb)
{
	m_bGfx = tb.m_bGfx;
	m_bIsFlipped = tb.m_bIsFlipped;
	m_bIsMirrored = tb.m_bIsMirrored;
	m_bTransparent = tb.m_bTransparent;
	m_colBack = tb.m_colBack;
	m_colBorder = tb.m_colBorder;
	m_nGfxIndex = tb.m_nGfxIndex;
	m_nType = tb.m_nType;
	m_strGfxName = tb.m_strGfxName;
	m_rectArea = tb.m_rectArea;
	m_vecTextlines = tb.m_vecTextlines;
	m_teText = tb.m_teText;
}

CCoverTextBox& CCoverTextBox::operator= (const CCoverTextBox& tb)
{
	m_bGfx = tb.m_bGfx;
	m_bIsFlipped = tb.m_bIsFlipped;
	m_bIsMirrored = tb.m_bIsMirrored;
	m_bTransparent = tb.m_bTransparent;
	m_colBack = tb.m_colBack;
	m_colBorder = tb.m_colBorder;
	m_nGfxIndex = tb.m_nGfxIndex;
	m_nType = tb.m_nType;
	m_strGfxName = tb.m_strGfxName;
	m_rectArea = tb.m_rectArea;
	m_vecTextlines = tb.m_vecTextlines;
	m_teText = tb.m_teText;
	return *this;
}

CCoverTextBox::CCoverTextBox()
{
	m_nGfxIndex = -1;

	m_bIsMirrored = FALSE;
	m_bIsFlipped = FALSE;

	m_bTransparent = TRUE;
	m_vecTextlines.clear();
	m_rectArea = CRect(0,0,0,0);

	CTextElement defTE;
	m_teText = defTE;
	m_colBack = (COLORREF) 0x00ffffff;
	m_colBorder = (COLORREF) 0x00ffffff;

	m_strGfxName = "";
	m_bGfx = FALSE;

	m_nType = CCoverTextBox::text;
};


CCoverTextBox::CCoverTextBox(CRect rect)
{
	m_nGfxIndex = -1;

	m_bIsMirrored = FALSE;
	m_bIsFlipped = FALSE;

	m_bTransparent = TRUE;
	m_vecTextlines.clear();
	m_rectArea = rect;

	CTextElement defTE;
	m_teText = defTE;
	m_colBack = (COLORREF) 0x00ffffff;
	m_colBorder = (COLORREF) 0x00ffffff;

	m_strGfxName = "";
	m_bGfx = FALSE;

	m_nType = CCoverTextBox::text;
};

BOOL CCoverTextBox::AddGfx(CWnd *pWnd)
{
	CString strMask;
	strMask.LoadString(IDS_GFXMASKS);

	m_strGfxName = CCoverLayout::FileBrowser("", "", strMask, pWnd, TRUE);
	m_bGfx = (m_strGfxName != "");
	m_nType = CCoverTextBox::grafik;

	return m_bGfx;
}

void CCoverTextBox::DelGfx()
{
	m_bGfx = FALSE;
	m_strGfxName = "";
	
	return;
}

BOOL CCoverTextBox::Load(CArchive& ar, const int nVer)
{
	BOOL bReturn = FALSE;

	try
	{
		int nLines;
		ar >> nLines;

		if (nLines != -1)
		{
			bReturn = LoadOldFormat(ar, nLines);
		}
		else
		{
			int nVersion;
			ar >> nVersion;

			ar >> nLines;

			for (int n = 0; n < nLines; n++)
			{
				CString strLine;
				ar >> strLine;
				m_vecTextlines.push_back(strLine);
			}
	
			ar >> m_rectArea;
			ar >> m_nType;
			if (m_nType == CCoverTextBox::text)
			{
				ar >> m_teText.m_strText;
			}
		
			ar >> m_colBack;
			ar >> m_colBorder;
			ar >> m_bTransparent;

			bReturn = m_teText.Load(ar);

			ar >> m_strGfxName;

			if (nVer > 3)
			{
				ar >> m_bIsFlipped;
				ar >> m_bIsMirrored;
			}
		}

	}
	catch(CException *e)
	{
		e->Delete();
		bReturn = FALSE;
	}

	return bReturn;
}

BOOL CCoverTextBox::LoadOldFormat(CArchive& ar, const int nLines)
{
	for (int n = 0; n < nLines; n++)
	{
		CString strLine;
		ar >> strLine;
		m_vecTextlines.push_back(strLine);
	}
	
	ar >> m_rectArea;
	ar >> m_nType;
	if (m_nType == CCoverTextBox::text)
	{
		ar >> m_teText.m_strText;
	}
		
	ar >> m_colBack;
	ar >> m_colBorder;
	ar >> m_bTransparent;

	return m_teText.Load(ar);
}

bool CCoverTextBox::operator== (const CCoverTextBox& tb)
{
	bool bReturn = TRUE;

	bReturn &= (tb.m_bGfx == m_bGfx);
	bReturn &= (tb.m_bIsFlipped == m_bIsFlipped);
	bReturn &= (tb.m_bIsMirrored == m_bIsMirrored);
	bReturn &= (tb.m_bTransparent == m_bTransparent);
	bReturn &= (tb.m_colBack == m_colBack);
	bReturn &= (tb.m_colBorder == m_colBorder);
	bReturn &= (tb.m_nGfxIndex == m_nGfxIndex);
	bReturn &= (tb.m_nType == m_nType);
	bReturn &= (tb.m_strGfxName == m_strGfxName);

	if (tb.m_rectArea != m_rectArea)
		bReturn = false;

	CCoverTextBox t = tb;
	bReturn &= (t.m_teText == m_teText);

	bReturn &= ((int)tb.m_vecTextlines.size() == (int)m_vecTextlines.size());
	if (bReturn)
		for (int i = 0; i < (int)m_vecTextlines.size(); i++)
			bReturn &= tb.m_vecTextlines[i] == m_vecTextlines[i];

	return bReturn;
}


BOOL CCoverTextBox::Save(CArchive& ar)
{
	BOOL bReturn = FALSE;

	try
	{
		ar << (int) -1;
		ar << (int) CCOVERTEXTBOX_VERSION;

		ar << m_vecTextlines.size();

		for (int n = 0; n < (int)m_vecTextlines.size(); n++)
		{
			ar << m_vecTextlines[n];
		}
		
		ar << m_rectArea;
		ar << m_nType;
		
		if (m_nType == CCoverTextBox::text)
		{
			ar << m_teText.m_strText;
		}
		
		ar << m_colBack;
		ar << m_colBorder;
		ar << m_bTransparent;

		bReturn = m_teText.Save(ar);

		ar << m_strGfxName;
		ar << m_bIsFlipped;
		ar << m_bIsMirrored;

	}
	catch(CException *e)
	{
		e->Delete();
		bReturn = FALSE;
	}

	return bReturn;
}


CCoverFrame::CCoverFrame()
{
	m_colBorder = 0x0;
	m_colBackground = 0x00ffffff;
}

bool CCoverColumn::operator== (const CCoverColumn& column)
{
	bool bReturn = TRUE;
	bReturn &= column.m_dWidthRel == m_dWidthRel;
	bReturn &= column.m_nType == m_nType;
	bReturn &= m_teColumn == m_teColumn;

	return bReturn;
}


CCoverColumn::CCoverColumn()
{
	m_dWidthRel = 0.0;
	CTextElement te;
	m_teColumn = te;
}

BOOL CCoverColumn::Load(CArchive& ar)
{
	BOOL bReturn = FALSE;

	try
	{
		ar >> m_dWidthRel;
		ar >> m_nType;
		bReturn = m_teColumn.Load(ar);
		if (m_nType == CCoverColumn::FREI)
		{
			ar >> m_teColumn.m_strText;
		}

	}
	catch(CException *e)
	{
		e->Delete();
	}

	return bReturn;
}


BOOL CCoverColumn::Save(CArchive& ar)
{
	BOOL bReturn = FALSE;

	try
	{
		ar << m_dWidthRel;
		ar << m_nType;
		bReturn = m_teColumn.Save(ar);
		if (m_nType == CCoverColumn::FREI)
		{
			ar << m_teColumn.m_strText;
		}
	}
	catch(CException *e)
	{
		e->Delete();
	}

	return bReturn;
}

CCoverLayout::~CCoverLayout()
{
	for (int i = 0; i < (int)m_vecGfxLookup.size(); i++)
		m_vecGfxLookup[i].Kill();
	m_vecGfxLookup.clear();

	m_gfxFront.Kill();
	m_gfxBack.Kill();
	m_gfxLabel.Kill();
}

CCoverLayout::CCoverLayout()
{
	m_vecGfxLookup.clear();

	gl_COVERBACKWIDTH = 1380;
	gl_COVERBACKHEIGHT = 1180;
	gl_COVERFRONTWIDTH = 1200;
	gl_COVERFRONTHEIGHT = 1200;
	gl_COVERBORDERWIDTH = 65;
	gl_COVERLABELHEIGHT = 1180;
	gl_COVERLABELWIDTH = 1180;
	gl_COVERINNERLABELRADIUS = 410;
		
	m_strCurrentLayoutName = "";

	m_bDrawBorders = TRUE;

	m_nRandX = 0;
	m_nRandY = 0;
	
	m_vecZweckformen.clear();

	m_nZweckformIndex = 0;
	m_nZweckformFirmaIndex = 0;
	m_bZweckformUser = TRUE;

	m_nCurrentArea = CCoverLayout::nothing;

	m_nCDTitleFrontYFreeValue = 0;
	m_nArtistFrontYFreeValue = 0;
	m_nCDTitleFrontXFreeValue = 0;
	m_nArtistFrontXFreeValue = 0;

	m_colBorderLeftRand = 0x00;
	m_colBorderRightRand = 0x00;
	m_colBorderLeftBack = 0x00ffffff;
	m_colBorderRightBack = 0x00ffffff;
	m_colBack = 0x00ffffff;
	m_colBackRand = 0x00;
	m_colFront = 0x00ffffff;
	m_colFrontRand = 0x00;
	m_colLabel = 0x00ffffff;
	m_colLabelRand = 0x00;

	m_bTextGfxLabel = TRUE;
	m_bTextGfxBack = TRUE;
	m_bTextGfxFront = TRUE;

	m_bLabelGfx = FALSE;
	m_bBackGfx = FALSE;
	m_bFrontGfx = FALSE;
	m_bGfxBackBorder = TRUE;
	m_strLabelGfx = "";
	m_strBackGfx = "";
	m_strFrontGfx = "";

	m_nBackYAlignment = CTextElement::top;
	m_nBackXAlignment = CTextElement::left;

	m_nZoomFactor = 100;
	m_bZoomMode = FALSE;
	m_pActiveRect = NULL;
	m_pLastActiveRect = NULL;
	m_nCoverMode = CCoverLayout::both;
	CTextElement teDefault;

	m_teBorderLeft = teDefault;
	m_teBorderLeft.m_nYAlignment = CTextElement::bottom;
	m_teBorderRight = teDefault;
	m_teBorderRight.m_nYAlignment = CTextElement::top;

	m_bBorderToArtistTitle = TRUE;

	m_bLeftBorder_AT = TRUE;
	m_bRightBorder_AT = TRUE;

	m_bJoinBorderTexts = TRUE;
	m_bJoinColumns = FALSE;

	m_pdb = NULL;
	m_pccd = NULL;

	CCoverColumn column;

	column.m_dWidthRel = 1.0 / 14.0;
	column.m_nType = CCoverColumn::TRACKNR;
	m_vecColumns.push_back(column);
	column.m_dWidthRel = 4.0 / 14.0;
	column.m_nType = CCoverColumn::ARTIST;
	m_vecColumns.push_back(column);
	column.m_dWidthRel = 1.0 / 14.0;
	column.m_nType = CCoverColumn::SEPARATOR;
	column.m_teColumn.m_nXAlignment = CTextElement::hook;
	m_vecColumns.push_back(column);
	column.m_dWidthRel = 6.0 / 14.0;
	column.m_teColumn.m_nXAlignment = CTextElement::hook;
	column.m_nType = CCoverColumn::TITLE;
	m_vecColumns.push_back(column);
	column.m_dWidthRel = 2.0 / 14.0;
	column.m_nType = CCoverColumn::LENGTH;
	column.m_teColumn.m_nXAlignment = CTextElement::right;
	m_vecColumns.push_back(column);

	m_teCDTitle = teDefault;
	m_teArtist = teDefault;
	m_teArtist.m_lfFont.lfHeight = 120;
	m_teCDTitle.m_lfFont.lfHeight = 120;
	m_teCDTitle.m_nYAlignment = CTextElement::bottom;
	m_teArtist.m_nYAlignment = CTextElement::top;

	m_bShowFrontArtist = TRUE;
	m_bShowFrontCDTitle = TRUE;

	ResetDrawPos();

/*	m_nBackX = (2100 - gl_COVERBACKWIDTH) / 2;
	m_nBackY = (2970 - gl_COVERFRONTHEIGHT - gl_COVERBACKHEIGHT) / 2;

	m_nFrontX = (2100 - gl_COVERFRONTWIDTH) / 2;
	m_nFrontY = m_nBackY + gl_COVERBACKHEIGHT;

	m_nLabelX = (2100 - gl_COVERLABELWIDTH) / 2;
	m_nLabelY = (2970 - gl_COVERLABELHEIGHT) / 2;;
*/
	m_strTrenn = "";

	CTextElement te;
	te.m_nXAlignment = CTextElement::center;
	te.m_nYAlignment = CTextElement::center;
	te.m_lfFont.lfHeight = 80;

	CCoverTextBox tb;
	tb.m_nType = CCoverTextBox::cdartist;
	tb.m_teText = te;
	tb.m_rectArea = CRect (150, -250, 1050, -350);
	m_tbsL.push_back(tb);


	CCoverTextBox tb1;
	tb1.m_nType = CCoverTextBox::cdtitle;
	tb1.m_teText = te;
	tb1.m_rectArea = CRect (150, -850, 1050, -950);
	m_tbsL.push_back(tb1);

	m_pActiveRect = NULL;
	m_pActiveTB = NULL;
	m_pActiveMainRect = NULL;

	return;
}

void CCoverLayout::ToggleTextGfxLabel()
{
	m_bTextGfxLabel = ! m_bTextGfxLabel;

	return;
}

void CCoverLayout::ToggleTextGfxBack()
{
	m_bTextGfxBack = ! m_bTextGfxBack;

	return;
}

void CCoverLayout::ToggleTextGfxFront()
{
	m_bTextGfxFront = ! m_bTextGfxFront;

	return;
}

void CCoverLayout::ToggleBorderJoin()
{
	m_bJoinBorderTexts = ! m_bJoinBorderTexts;

	return;
}

void CCoverLayout::ToggleColumnsJoin()
{
	m_bJoinColumns = ! m_bJoinColumns;

	return;
}

void CCoverLayout::ToggleZoom()
{
	m_bZoomMode = ! m_bZoomMode;

	return;
}

void CCoverLayout::ToggleBorder()
{
	m_bDrawBorders = ! m_bDrawBorders;

	return;
}

void CCoverLayout::SetCoverMode(const int nMode)
{
	m_nCoverMode = nMode;

	return;
}


BOOL CCoverLayout::SetZoomFactor(int nZoomFactor)
{
	BOOL bReturn = TRUE;

	if (nZoomFactor < 30)
	{
		nZoomFactor = 30;
		bReturn = FALSE;
	}

	if (nZoomFactor > 200)
	{
		nZoomFactor = 200;
		bReturn = FALSE;
	}

	m_nZoomFactor = nZoomFactor;

	return bReturn;
}


BOOL CCoverLayout::ZoomEnabled()
{
	return m_bZoomMode;
}

int CCoverLayout::GetCoverMode()
{
	return m_nCoverMode;
}

CString CCoverLayout::GetPureFileName(const CString& strPath)
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


CString CCoverLayout::FileBrowser(const CString& strExtension, CString strFileName, const CString& strMask, CWnd *pWnd, BOOL bLoad)
{
	if (CCoverLayout::GetPureFileName(strFileName) == "")
	{
		strFileName = "";
	}

	CString strReturn;
	CNewFileDialog dlgFile(bLoad, strExtension, strFileName, OFN_OVERWRITEPROMPT, strMask, pWnd);

	if (dlgFile.DoModal("CDCover") == IDOK)
	{
		strReturn = dlgFile.GetPathName();
	}

	return strReturn;
}

void CCoverLayout::DelGfxLabel()
{
	m_bLabelGfx = FALSE;
	m_strLabelGfx = "";
	m_bTextGfxLabel = TRUE;
	return;
}

BOOL CCoverLayout::AddGfxLabel(CWnd *pWnd)
{
	CString strMask;
	strMask.LoadString(IDS_GFXMASKS);

	m_strLabelGfx = FileBrowser("", "", strMask, pWnd, TRUE);
	m_bLabelGfx = (m_strLabelGfx != "");
	m_gfxLabel.m_bInitComplete = FALSE;

	return m_bLabelGfx;
}

BOOL CCoverLayout::AddGfxBack(CWnd *pWnd)
{
	CString strMask;
	strMask.LoadString(IDS_GFXMASKS);

	m_strBackGfx = FileBrowser("", "", strMask, pWnd, TRUE);
	m_bBackGfx = (m_strBackGfx != "");
	m_gfxBack.m_bInitComplete = FALSE;

	return m_bBackGfx;
}

void CCoverLayout::DelGfxBack()
{
	m_bBackGfx = FALSE;
	m_strBackGfx = "";
	m_bTextGfxBack = TRUE;
	return;
}

BOOL CCoverLayout::AddGfxFront(CWnd *pWnd)
{
	CString strMask;
	strMask.LoadString(IDS_GFXMASKS);

	m_strFrontGfx = FileBrowser("", "", strMask, pWnd, TRUE);
	m_bFrontGfx = (m_strFrontGfx != "");
	m_gfxFront.m_bInitComplete = FALSE;

	return m_bBackGfx;
}

void CCoverLayout::DelGfxFront()
{
	m_bFrontGfx = FALSE;
	m_strFrontGfx = "";
	m_bTextGfxFront = TRUE;

	return;
}

BOOL CCoverLayout::Save(const CString& strFileName)
{
	BOOL bReturn = TRUE;

	try
	{
		CFile File(strFileName, CFile::modeCreate | CFile::modeWrite);

		try
		{
			CArchive ar(&File, CArchive::store, 4096, NULL);

			ar << COVER_LAYOUT_ID;
			ar << COVER_LAYOUT_VER;

			ar << m_bBorderToArtistTitle;

			ar << m_bJoinBorderTexts;
			ar << m_bJoinColumns;

			ar << m_bLeftBorder_AT;
			ar << m_bRightBorder_AT;
			ar << m_bLeftBorder_Dominate;
			ar << m_bRightBorder_Dominate;

			ar << m_bGfxBackBorder;
			ar << m_bTextGfxBack;
			ar << m_bTextGfxFront;
			ar << m_bTextGfxLabel;
			ar << m_bBackGfx;
			ar << m_bFrontGfx;
			ar << m_bLabelGfx;
			ar << m_strBackGfx;
			ar << m_strFrontGfx;
			ar << m_strLabelGfx;

			ar << m_nBackYAlignment;

			bReturn &= m_teBorderLeft.Save(ar);
			bReturn &= m_teBorderRight.Save(ar);

			ar << m_vecColumns.size();

			int i = 0;
			while (bReturn && i != (int)m_vecColumns.size())
			{
				bReturn &= m_vecColumns[i].Save(ar);
				i++;
			}

			bReturn &= m_teArtist.Save(ar);
			bReturn &= m_teCDTitle.Save(ar);

			ar << m_bShowFrontArtist;
			ar << m_bShowFrontCDTitle;

			ar << m_colBorderLeftRand;
			ar << m_colBorderRightRand;
			ar << m_colBorderLeftBack;
			ar << m_colBorderRightBack;
			ar << m_colBack;
			ar << m_colBackRand;
			ar << m_colFront;
			ar << m_colFrontRand;
			ar << m_colLabel;
			ar << m_colLabelRand;

			ar << m_nCDTitleFrontYFreeValue;
			ar << m_nArtistFrontYFreeValue;
			ar << m_nCDTitleFrontXFreeValue;
			ar << m_nArtistFrontXFreeValue;

			ar << m_tbsF.size();
			i = 0;
			while (bReturn && i != (int)m_tbsF.size())
			{
				bReturn &= m_tbsF[i].Save(ar);
				i++;
			}
			ar << m_tbsB.size();
			i = 0;
			while (bReturn && i != (int)m_tbsB.size())
			{
				bReturn &= m_tbsB[i].Save(ar);
				i++;
			}

			ar << m_tbsL.size();
			i = 0;
			while (bReturn && i != (int)m_tbsL.size())
			{
				bReturn &= m_tbsL[i].Save(ar);
				i++;
			}

			ar << m_nFrontX;
			ar << m_nFrontY;
			ar << m_nBackX;
			ar << m_nBackY;
			ar << m_nLabelX;
			ar << m_nLabelY;

			ar << m_nZweckformIndex;
			ar << m_nZweckformFirmaIndex;
			ar << m_bZweckformUser;

			ar << m_bDrawBorders;

			ar << m_nBackXAlignment;

			ar << gl_COVERBACKHEIGHT;
			ar << gl_COVERBACKWIDTH;
			ar << gl_COVERFRONTHEIGHT;
			ar << gl_COVERFRONTWIDTH;
			ar << gl_COVERLABELHEIGHT;
			ar << gl_COVERLABELWIDTH;
			ar << gl_COVERINNERLABELRADIUS;
			ar << gl_COVERBORDERWIDTH;
			
			ar.Flush();
			ar.Close();
		}

		catch(CException *e)
		{
			e->Delete();
			bReturn = FALSE;
		}

		File.Flush();
		File.Close();
	}
	catch(CFileException *e)
	{
		e->Delete();
		bReturn = FALSE;
	}

	if (bReturn)
	{
		m_strCurrentLayoutName = strFileName;
	}

	return bReturn;
}






BOOL CCoverLayout::Load(const CString& strFileName)
{
	BOOL bReturn = TRUE;

	try
	{
		CFile File(strFileName, CFile::modeRead);

		try
		{
			CArchive ar(&File, CArchive::load, 4096, NULL);

			unsigned long nCDCoverID, nCDCoverVer;

			ar >> nCDCoverID;
			ar >> nCDCoverVer;

			if (nCDCoverID == COVER_LAYOUT_ID)
			{
				if (nCDCoverVer < 2)
				{
					bReturn &= LoadOldLayout(ar);
				}
				else
				{
					ar >> m_bBorderToArtistTitle;

					ar >> m_bJoinBorderTexts;
					ar >> m_bJoinColumns;

					ar >> m_bLeftBorder_AT;
					ar >> m_bRightBorder_AT;
					ar >> m_bLeftBorder_Dominate;
					ar >> m_bRightBorder_Dominate;

					ar >> m_bGfxBackBorder;
					ar >> m_bTextGfxBack;
					ar >> m_bTextGfxFront;
					ar >> m_bTextGfxLabel;
					ar >> m_bBackGfx;
					ar >> m_bFrontGfx;
					ar >> m_bLabelGfx;
					ar >> m_strBackGfx;
					ar >> m_strFrontGfx;
					ar >> m_strLabelGfx;

					ar >> m_nBackYAlignment;

					bReturn &= m_teBorderLeft.Load(ar);
					bReturn &= m_teBorderRight.Load(ar);

					int nColumns;
					ar >> nColumns;
					
					m_vecColumns.clear();

					int i = 0;
					while (bReturn && i != nColumns)
					{
						CCoverColumn cur;
						bReturn &= cur.Load(ar);
						m_vecColumns.push_back(cur);
						i++;
					}

					bReturn &= m_teArtist.Load(ar);
					bReturn &= m_teCDTitle.Load(ar);

					ar >> m_bShowFrontArtist;
					ar >> m_bShowFrontCDTitle;

					ar >> m_colBorderLeftRand;
					ar >> m_colBorderRightRand;
					ar >> m_colBorderLeftBack;
					ar >> m_colBorderRightBack;
					ar >> m_colBack;
					ar >> m_colBackRand;
					ar >> m_colFront;
					ar >> m_colFrontRand;
					ar >> m_colLabel;
					ar >> m_colLabelRand;

					ar >> m_nCDTitleFrontYFreeValue;
					ar >> m_nArtistFrontYFreeValue;
					ar >> m_nCDTitleFrontXFreeValue;
					ar >> m_nArtistFrontXFreeValue;

					int nTBs;
					ar >> nTBs;
					m_tbsF.clear();
					i = 0;
					while (bReturn && i != nTBs)
					{
						CCoverTextBox cur;
						bReturn &= cur.Load(ar, nCDCoverVer);
						if (bReturn)
						{
							m_tbsF.push_back(cur);
						}
						i++;
					}
		
					ar >> nTBs;
					m_tbsB.clear();
					i = 0;
					while (bReturn && i != nTBs)
					{
						CCoverTextBox cur;
						bReturn &= cur.Load(ar, nCDCoverVer);
						if (bReturn)
						{
							m_tbsB.push_back(cur);
						}
						i++;
					}

					ar >> nTBs;
					m_tbsL.clear();
					i = 0;
					while (bReturn && i != nTBs)
					{
						CCoverTextBox cur;
						bReturn &= cur.Load(ar, nCDCoverVer);
						if (bReturn)
						{
							m_tbsL.push_back(cur);
						}
						i++;
					}

					ar >> m_nFrontX;
					ar >> m_nFrontY;
					ar >> m_nBackX;
					ar >> m_nBackY;
					ar >> m_nLabelX;
					ar >> m_nLabelY;

					ar >> m_nZweckformIndex;
					ar >> m_nZweckformFirmaIndex;
					ar >> m_bZweckformUser;

					ar >> m_bDrawBorders;

					if (nCDCoverVer > 2)
					{
						ar >> m_nBackXAlignment;
					}

					if (nCDCoverVer > 3)
					{
						ar >> gl_COVERBACKHEIGHT;
						ar >> gl_COVERBACKWIDTH;
						ar >> gl_COVERFRONTHEIGHT;
						ar >> gl_COVERFRONTWIDTH;
						ar >> gl_COVERLABELHEIGHT;
						ar >> gl_COVERLABELWIDTH;
						ar >> gl_COVERINNERLABELRADIUS;
						ar >> gl_COVERBORDERWIDTH;
					}
				}
			}

			ar.Close();
		}
		catch(CException *e)
		{
			e->Delete();
			bReturn = FALSE;
		}

		File.Close();
	}
	catch(CFileException *e)
	{
		e->Delete();
		bReturn = FALSE;
	}

	if (bReturn)
	{
		m_strCurrentLayoutName = strFileName;
	}

	return bReturn;
}




BOOL CCoverLayout::LoadOldLayout(CArchive& ar)
{
	BOOL bReturn = TRUE;

	try
	{
		m_bShowFrontArtist = TRUE;
		m_bShowFrontCDTitle = TRUE;

		bReturn &= m_teArtist.OldLoad(ar);
		bReturn &= m_teCDTitle.OldLoad(ar);
		bReturn &= m_teBorderLeft.OldLoad(ar);
		bReturn &= m_teBorderRight.OldLoad(ar);

		CTextElement teDummy;
		bReturn &= teDummy.OldLoad(ar);
		m_colFrontRand = teDummy.m_colText;
		m_colFront = teDummy.m_colTextbk;

		bReturn &= teDummy.OldLoad(ar);
		m_colBackRand = teDummy.m_colText;
		m_colBack = teDummy.m_colTextbk;
		m_colBorderLeftRand = teDummy.m_colText;
		m_colBorderLeftBack = teDummy.m_colTextbk;
		m_colBorderRightRand = teDummy.m_colText;
		m_colBorderRightBack = teDummy.m_colTextbk;
		m_nBackYAlignment = teDummy.m_nYAlignment;
		m_nBackXAlignment = teDummy.m_nXAlignment;

		int nRandTextType;
		ar >> nRandTextType;

		switch (nRandTextType)
		{
			case COVER_RANDTEXT_DEFAULT:
				m_bBorderToArtistTitle = FALSE;
				break;
			case COVER_RANDTEXT_INTERPRETTITEL:
				m_bBorderToArtistTitle = TRUE;
				break;
			case COVER_RANDTEXT_EQUAL:
				m_bJoinColumns = TRUE;
		}

		ar >> m_strBackGfx;
		ar >> m_strFrontGfx;

		int nBitmapType;

		ar >> nBitmapType;

		switch (nBitmapType)
		{
			case COVER_BITMAP_TEXTONLY:
				m_bBackGfx = FALSE;
				m_bGfxBackBorder = FALSE;
				m_bTextGfxBack = FALSE;
				break;
			case COVER_BITMAP_BMPONLY_SMALL:
				m_bBackGfx = TRUE;
				m_bGfxBackBorder = FALSE;
				m_bTextGfxBack = FALSE;
				break;
			case COVER_BITMAP_BMPONLY_LARGE:
				m_bBackGfx = TRUE;
				m_bGfxBackBorder = TRUE;
				m_bTextGfxBack = FALSE;
				break;
			case COVER_BITMAP_BOTH_SMALL:
				m_bBackGfx = TRUE;
				m_bGfxBackBorder = FALSE;
				m_bTextGfxBack = TRUE;
				break;
			case COVER_BITMAP_BOTH_LARGE:
				m_bBackGfx = TRUE;
				m_bGfxBackBorder = TRUE;
				m_bTextGfxBack = TRUE;
		}

		ar >> nBitmapType;

		
		switch (nBitmapType)
		{
			case COVER_BITMAP_TEXTONLY:
				m_bFrontGfx = FALSE;
				m_bTextGfxFront = FALSE;
				break;
			case COVER_BITMAP_BMPONLY_SMALL:
				m_bFrontGfx = TRUE;
				m_bTextGfxFront = FALSE;
				break;
			case COVER_BITMAP_BMPONLY_LARGE:
				m_bFrontGfx = TRUE;
				m_bTextGfxFront = FALSE;
				break;
			case COVER_BITMAP_BOTH_SMALL:
				m_bFrontGfx = TRUE;
				m_bTextGfxFront = TRUE;
				break;
			case COVER_BITMAP_BOTH_LARGE:
				m_bFrontGfx = TRUE;
				m_bTextGfxFront = TRUE;
		}

		BOOL bHook;

		ar >> bHook;

		ar >> m_bJoinColumns;

		int LineXAlign;
		ar >> LineXAlign;	// gibt's nicht mehr....

		int nAnzSpalten;
		ar >> nAnzSpalten;

		m_vecColumns.clear();
		CCoverColumn cur;

		for (int i = 0; i != nAnzSpalten; i++)
		{
			int absColumnWidth;
			double relColumnWidth;
			int absColumnXPos;
			double relColumnXPos;
			int Field;
			CTextElement te;
			
			ar >> Field;
			ar >> absColumnWidth;
			ar >> relColumnWidth;
			ar >> absColumnXPos;
			ar >> relColumnXPos;
			bReturn &= te.OldLoad(ar);
			
			if (bHook)
			{
				cur.m_teColumn.m_nXAlignment = CTextElement::hook;
			}

			cur.m_teColumn = te;
			cur.m_dWidthRel = relColumnWidth;

			switch (Field)
			{
				case COVER_FIELD_EMPTY:
					cur.m_nType = CCoverColumn::SEPARATOR;
					break;
				case COVER_FIELD_TRACKNUMBER:
					cur.m_nType = CCoverColumn::TRACKNR;
					break;
				case COVER_FIELD_INTERPRET:
					cur.m_nType = CCoverColumn::ARTIST;
					break;
				case COVER_FIELD_SONGNAME:
					cur.m_nType = CCoverColumn::TITLE;
					break;
				case COVER_FIELD_TRACKLENGHT:
					cur.m_nType = CCoverColumn::LENGTH;
					break;
				case COVER_FIELD_SEPARATOR:
					cur.m_nType = CCoverColumn::SEPARATOR;
					break;
				case COVER_FIELD_BPM:
					cur.m_nType = CCoverColumn::BPM;
					break;
				case COVER_FIELD_USER_1:
					cur.m_nType = CCoverColumn::USER1;
					break;
				case COVER_FIELD_USER_2:
					cur.m_nType = CCoverColumn::USER2;
					break;
				case COVER_FIELD_COMMENT:
					cur.m_nType = CCoverColumn::COMMENT;
					break;
				case COVER_FIELD_STARTPOSITION:
					cur.m_nType = CCoverColumn::SEPARATOR;
					break;
				case COVER_FIELD_MARK:
					cur.m_nType = CCoverColumn::CODES;
			}
			m_vecColumns.push_back(cur);
		}
	}
	catch(CException *e)
	{
		e->Delete();
		bReturn = FALSE;
	}

	return bReturn;
}



BOOL CCoverLayout::ParseEngineData(const CString& strFileName, std::vector<CCoverZweckform> *pvecZweckformen)
{
	pvecZweckformen->clear();

    try
    {
        CStdioFile stioFileLIOut(strFileName, CFile::modeRead | CFile::typeText);

        CString CurrentWord;

        while (GetNextWord(&stioFileLIOut, &CurrentWord) == 1)
        {
			if (CurrentWord.CompareNoCase(L"format") == 0)
            {
                CCoverZweckform CurEngine;

                int nKlammer = 0;

                while (GetNextWord(&stioFileLIOut, &CurrentWord) == 1)
                {
                    if (CurrentWord == "(")
                    {
                        nKlammer++;
                    }
                    if (CurrentWord == ")")
                    {
                        nKlammer--;
                    }
                    if (nKlammer == 0)
                    {
						if (CurEngine.m_strFirma != "" && CurEngine.m_vecFormate.size() > 0)
						{
							pvecZweckformen->push_back(CurEngine);
						}
                        break;
                    }

                    if (nKlammer == 1)
                    {
                        BOOL bFoundKeyword = FALSE;

                        if (CurrentWord.CompareNoCase(L"firma") == 0)
                        {
                            bFoundKeyword = TRUE;

                            if (GetNextWord(&stioFileLIOut, &CurrentWord) == 1)
                            {
                                CurEngine.m_strFirma = CurrentWord;
                            }
                        }

                        if (! bFoundKeyword && CurrentWord.CompareNoCase(L"vorlage") == 0)
                        {
                            bFoundKeyword = TRUE;

                            CCoverFormat CurSet;
                            int nBIOSKlammer = 0;

                            while (GetNextWord(&stioFileLIOut, &CurrentWord) == 1)
                            {
                                if (CurrentWord == "(")
                                {
                                    nBIOSKlammer++;
                                }
                                if (CurrentWord == ")")
                                {
                                    nBIOSKlammer--;
                                }
                                if (nBIOSKlammer == 0)
                                {
									if (CurSet.m_strNummer != "")
									{
										CurEngine.m_vecFormate.push_back(CurSet);
									}
                                    break;
                                }

                                if (nBIOSKlammer == 1)
                                {
                                    BOOL bFoundBIOSSETKeyword = FALSE;

                                    if (CurrentWord.CompareNoCase(L"name") == 0)
                                    {
                                        bFoundBIOSSETKeyword = TRUE;

                                        if (GetNextWord(&stioFileLIOut, &CurrentWord) == 1)
                                        {
                                            CurSet.m_strNummer = CurrentWord;
                                        }
                                    }

                                    if (! bFoundBIOSSETKeyword && CurrentWord.CompareNoCase(L"Fx") == 0)
			                        {
						                bFoundBIOSSETKeyword = TRUE;

                                        if (GetNextWord(&stioFileLIOut, &CurrentWord) == 1)
			                            {
											swscanf(LPCTSTR(CurrentWord), L"%d", &CurSet.m_nFX);
			                            }
						            }
                                    if (! bFoundBIOSSETKeyword && CurrentWord.CompareNoCase(L"Bx") == 0)
			                        {
						                bFoundBIOSSETKeyword = TRUE;

                                        if (GetNextWord(&stioFileLIOut, &CurrentWord) == 1)
			                            {
											swscanf(LPCTSTR(CurrentWord), L"%d", &CurSet.m_nBX);
			                            }
						            }
                                    if (! bFoundBIOSSETKeyword && CurrentWord.CompareNoCase(L"Lx") == 0)
			                        {
						                bFoundBIOSSETKeyword = TRUE;

                                        if (GetNextWord(&stioFileLIOut, &CurrentWord) == 1)
			                            {
											swscanf(LPCTSTR(CurrentWord), L"%d", &CurSet.m_nLX);
			                            }
						            }
                                    if (! bFoundBIOSSETKeyword && CurrentWord.CompareNoCase(L"Fy") == 0)
			                        {
						                bFoundBIOSSETKeyword = TRUE;

                                        if (GetNextWord(&stioFileLIOut, &CurrentWord) == 1)
			                            {
											swscanf(LPCTSTR(CurrentWord), L"%d", &CurSet.m_nFY);
			                            }
						            }
                                    if (! bFoundBIOSSETKeyword && CurrentWord.CompareNoCase(L"By") == 0)
			                        {
						                bFoundBIOSSETKeyword = TRUE;

                                        if (GetNextWord(&stioFileLIOut, &CurrentWord) == 1)
			                            {
											swscanf(LPCTSTR(CurrentWord), L"%d", &CurSet.m_nBY);
			                            }
						            }
                                    if (! bFoundBIOSSETKeyword && CurrentWord.CompareNoCase(L"Ly") == 0)
			                        {
						                bFoundBIOSSETKeyword = TRUE;

                                        if (GetNextWord(&stioFileLIOut, &CurrentWord) == 1)
			                            {
											swscanf(LPCTSTR(CurrentWord), L"%d", &CurSet.m_nLY);
			                            }
						            }
								}
                            }
                        }
                    }
                }
            }
        }

        stioFileLIOut.Close();

    }
    catch(CFileException *e)
    {
		e->Delete();
    }

	return (pvecZweckformen->size() > 0);
}


int CCoverLayout::GetNextWord(CStdioFile *stioFile, CString *strWord)
{
    static BOOL bNewLine = TRUE;
    static CString strLine;

    if (bNewLine)
    {
        try
        {
            if (stioFile->ReadString(strLine) == FALSE)
            {
                return 0;
            }
        }
        catch (CFileException *e)
        {
            e->Delete();
            return -1;
        }
    }

    CString s = strLine;

    s.TrimLeft();

    if (s == "")
    {
        bNewLine = TRUE;
        *strWord = "";
        return 1;
    }

    BOOL bQuotes = FALSE;
    CString st;
    int qpos = 0;

    if (s.Left(1) == '\"')
    {
        st = s.Mid(1);

        int q = 0;

        while (! bQuotes && q != st.GetLength())
        {
            if (st[q] == '\"' && q > 1 && st[q - 1] != '\\')
            {
                bQuotes = TRUE;
                qpos = q;
            }

            q++;
        }
    }

    s = bQuotes ? st.Left(qpos) : s.SpanExcluding(L"\t ");
    s.TrimRight();

    *strWord = s;

    int nPos = strLine.Find(s);

    if (nPos != -1)
    {
        strLine = strLine.Mid(nPos + s.GetLength() + (bQuotes ? 1 : 0));
    }

    bNewLine = (strLine == "");

    return 1;
}


void CCoverLayout::ResetDrawPos()
{
	const int nMode = GetCoverMode();

	m_nBackX = (2100 - gl_COVERBACKWIDTH) / 2;
	m_nFrontX = (2100 - gl_COVERFRONTWIDTH) / 2;
	m_nLabelX = (2100 - gl_COVERLABELWIDTH) / 2;
	m_nLabelY = (2970 - gl_COVERLABELHEIGHT) / 2;;

	if (nMode == CCoverLayout::both)
	{
		m_nBackY = (2970 - gl_COVERFRONTHEIGHT - gl_COVERBACKHEIGHT) / 2;
		m_nFrontY = m_nBackY + gl_COVERBACKHEIGHT;
	}
	else
	{
		m_nBackY = (2970 - gl_COVERBACKHEIGHT) / 2;
		m_nFrontY = (2970 - gl_COVERFRONTHEIGHT) / 2;
	}

	return;
}


bool CCoverLayout::operator== (const CCoverLayout& layout)
{
	bool bReturn = TRUE;

//		m_nCoverMode;

//		m_vecGfxLookup;

	CCoverGrafik gfx;
	gfx = layout.m_gfxFront;
	bReturn &= gfx == m_gfxFront;
	gfx = layout.m_gfxBack;
	bReturn &= gfx == m_gfxBack;
	gfx = layout.m_gfxLabel;
	bReturn &= gfx == m_gfxLabel;

		bReturn &= layout.m_strCurrentLayoutName == m_strCurrentLayoutName;

		bReturn &= layout.m_bDrawBorders == m_bDrawBorders;

		bReturn &= layout.m_nRandX == m_nRandX;
		bReturn &= layout.m_nRandY == m_nRandY;

		bReturn &= layout.m_bBorderToArtistTitle == m_bBorderToArtistTitle;

		bReturn &= layout.m_bJoinBorderTexts == m_bJoinBorderTexts;
		bReturn &= layout.m_bJoinColumns == m_bJoinColumns;

//		m_nZoomFactor;
//		m_bZoomMode;

		bReturn &= layout.m_bGfxBackBorder == m_bGfxBackBorder;
		bReturn &= layout.m_bTextGfxBack == m_bTextGfxBack;
		bReturn &= layout.m_bTextGfxFront == m_bTextGfxFront;
		bReturn &= layout.m_bTextGfxLabel == m_bTextGfxLabel;
		bReturn &= layout.m_bBackGfx == m_bBackGfx;
		bReturn &= layout.m_bFrontGfx == m_bFrontGfx;
		bReturn &= layout.m_bLabelGfx == m_bLabelGfx;
		bReturn &= layout.m_strBackGfx == m_strBackGfx;
		bReturn &= layout.m_strFrontGfx == m_strFrontGfx;
		bReturn &= layout.m_strLabelGfx == m_strLabelGfx;

		bReturn &= layout.m_nBackYAlignment == m_nBackYAlignment;
		bReturn &= layout.m_nBackXAlignment == m_nBackXAlignment;

/*		m_pccd;
		m_pdb;
		m_pActiveTB;
		m_nActiveTBIndex;
		m_pActiveRect;
		m_pLastActiveRect;
		m_pActiveMainRect;
*/		
		if (layout.m_rectLeft != m_rectLeft)
			bReturn = FALSE;
		if (layout.m_rectRight != m_rectRight)
			bReturn = FALSE;
		if (layout.m_rectBack != m_rectBack)
			bReturn = FALSE;
		if (layout.m_rectFront != m_rectFront)
			bReturn = FALSE;
		if (layout.m_rectArtist != m_rectArtist)
			bReturn = FALSE;
		if (layout.m_rectTitle != m_rectTitle)
			bReturn = FALSE;
		if (layout.m_rectLabel != m_rectLabel)
			bReturn = FALSE;
		if (layout.m_rectInnerLabel != m_rectInnerLabel)
			bReturn = FALSE;

		CCoverColumn column;
		bReturn &= (int)layout.m_vecColumns.size() == (int)m_vecColumns.size();
		if (bReturn)
			for (int i = 0; i < (int)m_vecColumns.size(); i++)
			{
				column = layout.m_vecColumns[i];
				bReturn &= column == m_vecColumns[i];
			}

		CCoverTextBox tb;
		bReturn &= (int)layout.m_vecTBFront.size() == (int)m_vecTBFront.size();
		if (bReturn)
			for (int i = 0; i < (int)m_vecTBFront.size(); i++)
			{
				tb = layout.m_vecTBFront[i];
				bReturn &= tb == m_vecTBFront[i];
			}

		bReturn &= (int)layout.m_vecTBBack.size() == (int)m_vecTBBack.size();
		if (bReturn)
			for (int i = 0; i < (int)m_vecTBBack.size(); i++)
			{
				tb = layout.m_vecTBBack[i];
				bReturn &= tb == m_vecTBBack[i];
			}

		bReturn &= (int)layout.m_vecTBLabel.size() == (int)m_vecTBLabel.size();
		if (bReturn)
			for (int i = 0; i < (int)m_vecTBLabel.size(); i++)
			{
				tb = layout.m_vecTBLabel[i];
				bReturn &= tb == m_vecTBLabel[i];
			}

//		m_tbsF;
//		m_tbsB;
//		m_tbsL;
//		m_nCurrentArea;

		CTextElement te;

		te = m_teArtist;
		bReturn &= te == m_teArtist;
		te = m_teCDTitle;
		bReturn &= te == m_teCDTitle;

		bReturn &= layout.m_bShowFrontArtist == m_bShowFrontArtist;
		bReturn &= layout.m_bShowFrontCDTitle == m_bShowFrontCDTitle;

		bReturn &= layout.m_colBorderLeftRand == m_colBorderLeftRand;
		bReturn &= layout.m_colBorderRightRand == m_colBorderRightRand;
		bReturn &= layout.m_colBorderLeftBack == m_colBorderLeftBack;
		bReturn &= layout.m_colBorderRightBack == m_colBorderRightBack;
		bReturn &= layout.m_colBack == m_colBack;
		bReturn &= layout.m_colBackRand == m_colBackRand;
		bReturn &= layout.m_colFront == m_colFront;
		bReturn &= layout.m_colFrontRand == m_colFrontRand;
		bReturn &= layout.m_colLabel == m_colLabel;
		bReturn &= layout.m_colLabelRand == m_colLabelRand;

		bReturn &= layout.m_nCDTitleFrontYFreeValue == m_nCDTitleFrontYFreeValue;
		bReturn &= layout.m_nArtistFrontYFreeValue == m_nArtistFrontYFreeValue;
		bReturn &= layout.m_nCDTitleFrontXFreeValue == m_nCDTitleFrontXFreeValue;
		bReturn &= layout.m_nArtistFrontXFreeValue == m_nArtistFrontXFreeValue;

		bReturn &= layout.m_nFrontX == m_nFrontX;
		bReturn &= layout.m_nFrontY == m_nFrontY;
		bReturn &= layout.m_nBackX == m_nBackX;
		bReturn &= layout.m_nBackY == m_nBackY;
		bReturn &= layout.m_nLabelX == m_nLabelX;
		bReturn &= layout.m_nLabelY == m_nLabelY;

		bReturn &= layout.m_nZweckformIndex == m_nZweckformIndex;
		bReturn &= layout.m_nZweckformFirmaIndex == m_nZweckformFirmaIndex;
		bReturn &= layout.m_bZweckformUser == m_bZweckformUser;

		bReturn &= layout.m_strTrenn == m_strTrenn;

		bReturn &= layout.m_bLeftBorder_AT == m_bLeftBorder_AT;
		bReturn &= layout.m_bRightBorder_AT == m_bRightBorder_AT;
		bReturn &= layout.m_bLeftBorder_Dominate == m_bLeftBorder_Dominate;
		bReturn &= layout.m_bRightBorder_Dominate == m_bRightBorder_Dominate;

//		m_vecZweckformen;

		bReturn &= layout.gl_COVERBACKWIDTH == gl_COVERBACKWIDTH;
		bReturn &= layout.gl_COVERBACKHEIGHT == gl_COVERBACKHEIGHT;
		bReturn &= layout.gl_COVERFRONTWIDTH == gl_COVERFRONTWIDTH;
		bReturn &= layout.gl_COVERFRONTHEIGHT == gl_COVERFRONTHEIGHT;
		bReturn &= layout.gl_COVERBORDERWIDTH == gl_COVERBORDERWIDTH;
		bReturn &= layout.gl_COVERLABELHEIGHT == gl_COVERLABELHEIGHT;
		bReturn &= layout.gl_COVERLABELWIDTH == gl_COVERLABELWIDTH;
		bReturn &= layout.gl_COVERINNERLABELRADIUS == gl_COVERINNERLABELRADIUS;

		te = m_teBorderLeft;
		bReturn &= te == m_teBorderLeft;
		te = m_teBorderRight;
		bReturn &= te == m_teBorderRight;



	return bReturn;
}

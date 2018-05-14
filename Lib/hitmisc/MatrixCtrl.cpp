/****************************************************************/
/*																*/
/*  MatrixCtrl.cpp												*/
/*																*/
/*  Implementation of the CMatrixCtrl class.					*/
/*	Attempt to create Matrix-like movie credits...				*/
/*																*/
/*  Programmed by Pablo van der Meer							*/
/*  Copyright Pablo Software Solutions 2002						*/
/*  http://www.pablovandermeer.nl								*/
/*																*/
/*  Last updated: 21 may 2002									*/
/*																*/
/****************************************************************/

#include "stdafx.h"

#include "hitmisc.h"
#include "MatrixCtrl.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__ ;
#endif

#define getrandom(min,max) ((rand()%(int)(((max)+1)-(min)))+(min));
#define GARBAGE "¿PA©¢ª£¥§©Bþ§¿ÞLçOª" 


CMatrixCtrl::CMatrixCtrl()
{
	// background, Matrix and data colors
	// these are public variables and can be set directly
	m_crBackColor  = RGB(0, 0, 0);  
	m_crTextColor  = RGB(0, 255, 0); 

	// protected variables
	m_brushBack.CreateSolidBrush(m_crBackColor);
	
	// protected bitmaps to restore the memory DC's
	m_pbitmapOldMatrix = NULL;

	m_Font.CreateFont(14, 0, 0, 0, FW_BOLD,
                       FALSE, FALSE, 0, ANSI_CHARSET,
                       OUT_DEFAULT_PRECIS, 
                       CLIP_DEFAULT_PRECIS,
                       DEFAULT_QUALITY, 
                       DEFAULT_PITCH|FF_SWISS, L"Tahoma");
	m_yLabel1 = 0;
	m_yLabel2 = 0;

	m_nCount = 0;
	m_nBkCount = 0;
	srand((unsigned)time(NULL));
	m_strLabel1 = GARBAGE;
	m_strLabel2 = GARBAGE;
	m_strLabel3 = "";
	m_strCredits = "";

	m_fFactor = 1;

	m_nCols = -1;
	m_bFinale = FALSE;
}


CMatrixCtrl::~CMatrixCtrl()
{
	if (m_pbitmapOldMatrix != NULL)
		m_dcMatrix.SelectObject(m_pbitmapOldMatrix);  

	for (int i = 0; i < m_nCols; i++)
	{
		delete [] m_MatrixColumns[i].pItem;
	}
}


BEGIN_MESSAGE_MAP(CMatrixCtrl, CWnd)
	//{{AFX_MSG_MAP(CMatrixCtrl)
	ON_WM_PAINT()
	ON_WM_SIZE()
	ON_WM_TIMER()
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()



/********************************************************************/
/*																	*/
/* Function name : Create											*/
/* Description   : Member to create the Matrix control.				*/
/*																	*/
/********************************************************************/
BOOL CMatrixCtrl::Create(DWORD dwStyle, const RECT& rect, 
                         CWnd* pParentWnd, UINT nID) 
{
	BOOL result;
	static CString className = AfxRegisterWndClass(CS_HREDRAW | CS_VREDRAW);

	result = CWnd::CreateEx(WS_EX_CLIENTEDGE,
		className, NULL, dwStyle, 
		rect.left, rect.top, rect.right-rect.left, rect.bottom-rect.top,
        pParentWnd->GetSafeHwnd(), (HMENU)nID);
  
	if (result != 0)
		InvalidateCtrl();

	m_yLabel1 = -(m_nClientHeight/3);
	m_yLabel2 = -(m_nClientWidth/2);

	Initialize();

	SetTimer(1, 10, NULL);
	SetTimer(2, 15, NULL);
	SetTimer(3, 120, NULL);
	return result;
}


/********************************************************************/
/*																	*/
/* Function name : SetTextColor										*/
/* Description   : Set the color of text.							*/
/*																	*/
/********************************************************************/
void CMatrixCtrl::SetTextColor(COLORREF color)
{
	m_crTextColor = color;

	// clear out the existing garbage, re-start with a clean plot
	InvalidateCtrl();
}


/********************************************************************/
/*																	*/
/* Function name : SetBackgroundColor								*/
/* Description   : Set background color.							*/
/*																	*/
/********************************************************************/
void CMatrixCtrl::SetBackgroundColor(COLORREF color)
{
	m_crBackColor = color;

	m_brushBack.DeleteObject();
	m_brushBack.CreateSolidBrush(m_crBackColor);

	// clear out the existing garbage, re-start with a clean plot
	InvalidateCtrl();
}



/********************************************************************/
/*																	*/
/* Function name : InvalidateCtrl									*/
/* Description   : Draw the Matrix to a bitmap.						*/
/*																	*/
/********************************************************************/
void CMatrixCtrl::InvalidateCtrl()
{
	CFont *oldFont;
	CString strTemp;
	CRect rcText;

	// in case we haven't established the memory dc's
	CClientDC dc(this);

	// if we don't have one yet, set up a memory dc for the Matrix
	if (m_dcMatrix.GetSafeHdc() == NULL)
	{
		m_dcMatrix.CreateCompatibleDC(&dc);
		m_bitmapMatrix.CreateCompatibleBitmap(&dc, m_nClientWidth, m_nClientHeight);
		m_pbitmapOldMatrix = m_dcMatrix.SelectObject(&m_bitmapMatrix);
	}
  
	m_dcMatrix.SetBkColor(m_crBackColor);

	// fill the Matrix background
	m_dcMatrix.BitBlt(0, 0, m_nClientWidth, m_nClientHeight, &m_BackgroundDC, 0, 0, SRCCOPY);
	
	// grab the horizontal font
	oldFont = m_dcMatrix.SelectObject(&m_Font);

	m_dcMatrix.SetTextColor(m_crTextColor);
	
	m_dcMatrix.DrawText(m_strLabel3, m_rectClient, DT_VCENTER|DT_SINGLELINE|DT_CENTER);

	CSize sz1 = m_dcMatrix.GetTextExtent(m_strLabel2);
	CSize sz2 = m_dcMatrix.GetTextExtent(m_strLabel3);
	
	if ((m_yLabel2 <= m_nStartOfNextLabel) && ((m_yLabel2 + sz2.cx) > (m_rectClient.Width()/2 + sz2.cx/2)))
		m_dcMatrix.FillSolidRect(m_rectClient.left, m_rectClient.Height()/2 - m_nTextHeight/2, m_rectClient.Width() - m_nStartOfNextLabel, m_rectClient.Height()/2 + m_nTextHeight/2, RGB(0,0,0));

	rcText = m_rectClient;
	rcText.top += m_yLabel1;
	m_dcMatrix.DrawText(m_strLabel1, rcText, DT_CENTER|DT_SINGLELINE|DT_TOP);
	rcText = m_rectClient;
	rcText.left += m_yLabel2;
	
	m_dcMatrix.DrawText(m_strLabel2, rcText, DT_VCENTER|DT_SINGLELINE);
	
	// restore the font
	m_dcMatrix.SelectObject(oldFont);

	// finally, force the plot area to redraw
	InvalidateRect(m_rectClient);
} 


/********************************************************************/
/*																	*/
/* Function name : OnPaint											*/
/* Description   : Called when the application makes a request to	*/
/*				   repaint a portion of the window.					*/
/*																	*/
/********************************************************************/
void CMatrixCtrl::OnPaint() 
{
	CPaintDC dc(this);
	CDC memDC;
	CBitmap memBitmap;
	CBitmap* oldBitmap;

	// to avoid flicker, establish a memory dc, draw to it 
	// and then BitBlt it to the client
	memDC.CreateCompatibleDC(&dc);
	memBitmap.CreateCompatibleBitmap(&dc, m_nClientWidth, m_nClientHeight);
	oldBitmap = (CBitmap *)memDC.SelectObject(&memBitmap);

	if (memDC.GetSafeHdc() != NULL)
	{
		// first drop the bitmap on the memory dc
		memDC.BitBlt(0, 0, m_nClientWidth, m_nClientHeight, &m_dcMatrix, 0, 0, SRCCOPY);
		// finally send the result to the display
		dc.BitBlt(0, 0, m_nClientWidth, m_nClientHeight, &memDC, 0, 0, SRCCOPY);
	}
	memDC.SelectObject(oldBitmap);
}


/********************************************************************/
/*																	*/
/* Function name : OnSize											*/
/* Description   : The framework calls this member function after	*/
/*				   the window’s size has changed.					*/
/*																	*/
/********************************************************************/
void CMatrixCtrl::OnSize(UINT nType, int cx, int cy) 
{
	CWnd::OnSize(nType, cx, cy);

	// NOTE: OnSize automatically gets called during the setup of the control
	GetClientRect(m_rectClient);

	// set some member variables to avoid multiple function calls
	m_nClientHeight = m_rectClient.Height();
	m_nClientWidth  = m_rectClient.Width();

	// destroy and recreate the matrix bitmap
	CClientDC dc(this);
	if (m_pbitmapOldMatrix && m_bitmapMatrix.GetSafeHandle() && m_dcMatrix.GetSafeHdc())
	{
		m_dcMatrix.SelectObject(m_pbitmapOldMatrix);
		m_bitmapMatrix.DeleteObject();
		m_bitmapMatrix.CreateCompatibleBitmap(&dc, m_nClientWidth, m_nClientHeight);
		m_pbitmapOldMatrix = m_dcMatrix.SelectObject(&m_bitmapMatrix);
	}
}


/********************************************************************/
/*																	*/
/* Function name : OnTimer											*/
/* Description   : Handle time event								*/
/*																	*/
/********************************************************************/
void CMatrixCtrl::OnTimer(UINT nIDEvent) 
{
	CString strTemp = GARBAGE;

	switch(nIDEvent)
	{
		case 1:
		{
			m_yLabel2+=1;
			m_strLabel2 = "";
			int nLength = strTemp.GetLength();
			for (int i=1; i<nLength; i++)
			{
				int k = getrandom(0, 13);
				m_strLabel2 += strTemp.Mid(k, 1);
			}

			TEXTMETRIC tm;
			m_dcMatrix.GetTextMetrics(&tm);
			int nWidth = tm.tmAveCharWidth;
			nWidth*=13;

			// is this the middle ?
			if (m_yLabel2 == m_nStartOfNextLabel)
			{
				// get next string
				if (AfxExtractSubString(m_strLabel3, m_strCredits, m_nCount, ','))
				{
					m_nCount++;
				}
				else
				{
					m_nCount = 0;
				}
			}

			// outside right border ?
			if ((m_yLabel2) > m_nClientWidth)
			{
				if (m_nCount != 0)
				{
					m_nBkCount++;
					CalculateNextLabelPos();
				}
				else
					m_nBkCount = -1;

				KillTimer(1);
				// set label position to begin
				m_yLabel2 = 0;
				m_strLabel2 = "";
				m_strLabel1 = "";
				// next string
				if (m_nCount)
				{
					m_yLabel1 = -(m_nClientHeight/3);
					m_yLabel2 = -(m_nClientWidth/2);
					SetTimer(1, 10, NULL);
					SetTimer(2, 15, NULL);
				}
			}
			break;
		}
		case 2:
		{
			CString strTemp = GARBAGE;
			
			TEXTMETRIC tm;
			m_dcMatrix.GetTextMetrics(&tm);
			int nHeight = tm.tmHeight;

			double fPos = ((double)m_yLabel2/(double)(m_nClientWidth/2));
			
			m_yLabel1 = (int)(m_nClientHeight * fPos);

			m_strLabel1 = "";
			int nLength = strTemp.GetLength();
			for (int i=1; i<nLength; i++)
			{
				int k = getrandom(0, 13);
				m_strLabel1 += strTemp.Mid(k, 1);
			}
			if (m_yLabel1 > m_nClientHeight)
			{
				KillTimer(2);
				m_strLabel1 = "";
			}
			break;
		}
		case 3:
		{
			CClientDC dc(this);
			DoBackground(&dc);
			break;
		}
	}
	InvalidateCtrl();
	CWnd::OnTimer(nIDEvent);
}


/********************************************************************/
/*																	*/
/* Function name : DoBackground										*/
/* Description   : Update background of Matrix Control				*/
/*																	*/
/********************************************************************/
void CMatrixCtrl::DoBackground(CDC *pDC)
{
	if (m_nCols == -1)
		return;

	m_BackgroundDC.SetBkColor(RGB(0,0,0));
	
	int nStartColumn;

	if ((m_nTotalColumns <= m_nCols) && (m_bAlternate == TRUE))
	{
		int nSafetyCounter = 0;
		do
		{
			nStartColumn = rand() % m_nCols;
			nSafetyCounter++;
			// check safety counter
			if (nSafetyCounter > m_nCols)
				break;
		}
		while (m_MatrixColumns[nStartColumn].bActive == TRUE);

		m_MatrixColumns[nStartColumn].bActive = TRUE;

		m_BackgroundDC.SetTextColor(RGB(255,255,255));
		
		CString strTextOut = (char)m_MatrixColumns[nStartColumn].pItem[0].character;
		m_BackgroundDC.TextOut(nStartColumn * m_nTextWidth, 0, strTextOut, 1);
		
		m_MatrixColumns[nStartColumn].nCounter++;
		m_nTotalColumns++;
		m_bAlternate = FALSE;
	}
	else
	{
		m_bAlternate = TRUE;
	}

	for (int i = 0; i < m_nCols; i++)
	{
		if (m_MatrixColumns[i].bActive == TRUE)
		{
			if (m_MatrixColumns[i].nCounter < m_MatrixColumns[i].nLength)
			{
				if (m_MatrixColumns[i].bOffScreen == FALSE)
				{
					CString strTextOut = (char)m_MatrixColumns[i].pItem[m_MatrixColumns[i].nCounter].character;

					m_BackgroundDC.SetTextColor(RGB(0,255,0));
					m_BackgroundDC.TextOut(i * m_nTextWidth, m_MatrixColumns[i].nCounter * m_nTextHeight, strTextOut, 1);

					m_BackgroundDC.SetTextColor(RGB(0,0,0));
					m_BackgroundDC.TextOut(i * m_nTextWidth, (m_MatrixColumns[i].nCounter -1) * m_nTextHeight, L" ", 1);

					strTextOut = (char)m_MatrixColumns[i].pItem[m_MatrixColumns[i].nCounter - 1].character;
					m_BackgroundDC.SetTextColor(RGB(0, m_MatrixColumns[i].pItem[m_MatrixColumns[i].nCounter- 1].clrGreen, 0));
					m_BackgroundDC.TextOut(i * m_nTextWidth, (m_MatrixColumns[i].nCounter -1) * m_nTextHeight, strTextOut, 1);

					m_MatrixColumns[i].nCounter++;
				}
				if (m_MatrixColumns[i].bOffScreen == TRUE)
				{
					if (m_nTotalColumns > 10)
					{
						CString strTextOut = (char)m_MatrixColumns[i].pItem[m_MatrixColumns[i].nCounter].character;
						m_BackgroundDC.SetTextColor(RGB(0,0,0));
						m_BackgroundDC.TextOut(i * m_nTextWidth, m_MatrixColumns[i].nCounter * m_nTextHeight, strTextOut, 1);

						m_MatrixColumns[i].nCounter++;
						if (m_MatrixColumns[i].nCounter == m_MatrixColumns[i].nLength)
						{
							UpdateCharacters(i);
							m_MatrixColumns[i].bActive = FALSE;
							m_MatrixColumns[i].bOffScreen = FALSE;
							m_nTotalColumns--;
							m_MatrixColumns[i].nCounter = 0;
						}
					}
				}
            
				if ((m_MatrixColumns[i].nCounter == m_MatrixColumns[i].nLength) || (m_MatrixColumns[i].nCounter == 75))
				{
					CString strTextOut = (char)m_MatrixColumns[i].pItem[m_MatrixColumns[i].nCounter -1].character;
					m_BackgroundDC.SetTextColor(RGB(0,m_MatrixColumns[i].pItem[m_MatrixColumns[i].nCounter - 1].clrGreen, 0));
					m_BackgroundDC.TextOut(i * m_nTextWidth, (m_MatrixColumns[i].nCounter - 1) * m_nTextHeight, strTextOut, 1);

					m_MatrixColumns[i].bOffScreen = TRUE;
					m_MatrixColumns[i].nCounter = 0;
				}
            }
		}
    }
}


/********************************************************************/
/*																	*/
/* Function name : Initialize										*/
/* Description   : Initialize background variables					*/
/*																	*/
/********************************************************************/
void CMatrixCtrl::Initialize()
{
	TEXTMETRIC tm;
	
	CClientDC dc(this);
		
	dc.GetTextMetrics(&tm);
	m_nTextWidth = tm.tmAveCharWidth+10;
	m_nTextHeight= tm.tmHeight;
	
	m_nCols = m_nClientWidth/m_nTextWidth;
	m_nRows = m_nClientHeight/m_nTextHeight;

	for (int i = 0; i < m_nCols; i++)
	{
		m_MatrixColumns[i].nLength = getrandom(5, 15);//m_nRows);
		
		m_MatrixColumns[i].pItem = new CMatrixItem[m_MatrixColumns[i].nLength];
		
		m_MatrixColumns[i].bActive = FALSE;
		m_MatrixColumns[i].nCounter = 0;
		m_MatrixColumns[i].bOffScreen = FALSE;
		m_MatrixColumns[i].x = i;
		m_MatrixColumns[i].y = 0;
		for (int j = 0; j < m_MatrixColumns[i].nLength; j++)
		{
			m_MatrixColumns[i].pItem[j].character = ' ';
			m_MatrixColumns[i].pItem[j].clrGreen = getrandom(75, 200);
		}
		UpdateCharacters(i);
	}

	m_nTotalColumns = 0;
	m_bAlternate = TRUE;
	
    //  create offscreen drawing area in memory
    m_BackgroundDC.CreateCompatibleDC(NULL);
    m_Bitmap.CreateCompatibleBitmap(&dc, m_nClientWidth, m_nClientHeight);
    m_pOldBitmap = (CBitmap *)m_BackgroundDC.SelectObject(m_Bitmap);

	// initialize to black
	m_BackgroundDC.FillSolidRect(0,0,m_nClientWidth, m_nClientHeight, RGB(0,0,0));

    m_BackgroundDC.SetBkMode(TRANSPARENT);

	CalculateNextLabelPos();
}


/********************************************************************/
/*																	*/
/* Function name : UpdateCharacters									*/
/* Description   : Update background characters in column so they	*/
/*				   match the displayed name.						*/
/*																	*/
/********************************************************************/
void CMatrixCtrl::UpdateCharacters(int nColumn)
{
	if (!AfxExtractSubString(m_strCurrentLabel, m_strCredits, m_nBkCount, ','))
	{
		if (m_nBkCount != -1)
			return;
	}

	int nPos = getrandom(0, (m_nRows/2));	
	for (int j = 0; j < m_MatrixColumns[nColumn].nLength; j++)
	{
		int nLength = m_strCurrentLabel.GetLength();
		if (nLength)
		{
			int nIndex = j - nPos;
			if ((nIndex >= 0) && (nIndex < nLength))
				m_MatrixColumns[nColumn].pItem[j].character = m_strCurrentLabel.GetAt(nIndex);
			else
				m_MatrixColumns[nColumn].pItem[j].character = ' ';
		}
		else
			m_MatrixColumns[nColumn].pItem[j].character = ' ';
	}
}


/********************************************************************/
/*																	*/
/* Function name : CalculateNextLabelPos							*/
/* Description   : Calculate start position of next label			*/
/*																	*/
/********************************************************************/
void CMatrixCtrl::CalculateNextLabelPos()
{
	// calculate postion of next label
	CString strNextLabel;
	AfxExtractSubString(strNextLabel, m_strCredits, m_nCount, ',');
	
	CRect rc = m_rectClient;
	CClientDC dc(this);
	dc.DrawText(strNextLabel, rc, DT_VCENTER|DT_SINGLELINE|DT_CENTER|DT_CALCRECT);
	m_nStartOfNextLabel = m_rectClient.Width()/2 - rc.Width()/2;
}


/********************************************************************/
/*																	*/
/* Function name : SetCredits										*/
/* Description   : Set text for Matrix control						*/
/*																	*/
/********************************************************************/
void CMatrixCtrl::SetCredits(LPCTSTR lpszCredits)
{
	m_strCredits = lpszCredits;
}

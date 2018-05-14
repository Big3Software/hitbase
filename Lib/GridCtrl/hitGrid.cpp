// hitGrid.cpp : implementation file
//

#include "stdafx.h"
#include "hitGrid.h"
#include "../hitmisc/HitbaseWinAppBase.h"
#include "../../app/hitbase/resource.h"
#include ".\hitgrid.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CGridListCtrl

IMPLEMENT_DYNAMIC(CGridListCtrl, CListCtrl) 

CGridListCtrl::CGridListCtrl(DWORD dwFlags/* = GLCF_GRID */)
{
	m_ActiveItem = 0;
	m_SelectedItem = 0;
	m_EditActive = 0;
	m_ComboBoxActive = FALSE;
	m_dwFlags = dwFlags;
	
	for (int i=0;i<_MAX_COLUMNS;i++)
	{
		m_Column[i].dwFlags = 0;
		m_Column[i].nEditWidth = 0;
	}
	
//	m_Font.CreateFont(14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, "ms sans serif"); 
	
	m_EditSubItem = -1;
	m_ItemColor = NULL;
	m_nItemColorLast = 0;
	m_bActiveItemColorSet = false;

	m_bFontCreated = FALSE;
	m_nSortedColumn = -1;

	m_bEnableDoubleColoring = TRUE;		// Standardmäßig an

}

CGridListCtrl::~CGridListCtrl()
{
	if (m_ItemColor)
		delete m_ItemColor;
	m_ItemColor = NULL;
	m_nItemColorLast = 0;
}

BEGIN_MESSAGE_MAP(CGridListCtrl, CListCtrl)
	//{{AFX_MSG_MAP(CGridListCtrl)
	ON_NOTIFY_REFLECT(NM_CLICK, OnClick)
	ON_WM_LBUTTONDOWN()
	ON_WM_HSCROLL()
	ON_WM_VSCROLL()
	ON_NOTIFY_REFLECT(LVN_DELETEALLITEMS, OnDeleteallitems)
	ON_WM_KEYDOWN()
	ON_COMMAND(101, OnChangeColor)
	ON_WM_SETCURSOR()
	//}}AFX_MSG_MAP
	ON_NOTIFY_REFLECT(NM_SETFOCUS, OnNMSetfocus)
	ON_NOTIFY_REFLECT(NM_KILLFOCUS, OnNMKillfocus)
	ON_WM_MEASUREITEM_REFLECT()
	ON_MESSAGE(WM_SETFONT, OnSetFont)
	ON_WM_ERASEBKGND()
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CGridListCtrl message handlers

void CGridListCtrl::DrawItem(LPDRAWITEMSTRUCT lpDrawItemStruct) 
{
	CDC dc, *pDC;
	CString str;
	int i=0, xpos=0;
	LV_COLUMN col;
	CRect rect, rect1;
	COLORREF TextColor, BkColor;
	CFont *oldfont = NULL;
	CPen GridPen, *oldpen;
	CBrush *oldbrush;
	CPen RectPen;
	COLORREF oldBkColor = GetBkColor();

	if (!m_bFontCreated)
	{
		LOGFONT lf;
		GetFont()->GetLogFont(&lf);
		lf.lfWeight = FW_BOLD;
		m_BoldFont.CreateFontIndirect(&lf);
		m_bFontCreated = TRUE;
	}

//	m_Font.Attach(*GetParent()->GetFont());

	GridPen.CreatePen(PS_SOLID, 0, ::GetSysColor(COLOR_3DLIGHT));
	RectPen.CreatePen(PS_SOLID, 0, ::GetSysColor(COLOR_ACTIVECAPTION));
	pDC = dc.FromHandle(lpDrawItemStruct->hDC);
	
	// Platzhalter für
	if (GetCellFlags(lpDrawItemStruct->itemID, 0) == GLCCELLFLAGS_GROUPHEADER_EMPTY ||
		GetCellFlags(lpDrawItemStruct->itemID, 0) == GLCCELLFLAGS_GROUPHEADER_TEXT ||
		GetCellFlags(lpDrawItemStruct->itemID, 0) == GLCCELLFLAGS_GROUPHEADER_LINE)
	{
		if (GetCellFlags(lpDrawItemStruct->itemID, 0) == GLCCELLFLAGS_GROUPHEADER_TEXT)
		{
			CFont* pOldFont = pDC->SelectObject(&m_BoldFont);
			CRect rect = lpDrawItemStruct->rcItem;
			CString str = GetItemText(lpDrawItemStruct->itemID, 0);
			pDC->ExtTextOut(rect.left + 5, rect.top, ETO_CLIPPED|ETO_OPAQUE, &rect, str, NULL);
			pDC->SelectObject(pOldFont);
		}

		return;
	}

	while (1)
	{
		memset(&col, 0, sizeof(col));
		col.mask = LVCF_WIDTH;
		
		if (!GetColumn(i, &col))
			break;
		
		str = GetItemText(lpDrawItemStruct->itemID, i);
	
		int nImage = GetCellValue(lpDrawItemStruct->itemID, i);
		
		// JUS 981031: Hier stand vorher GetTextColor(). Das klappte aber nicht, da
		// die Farbe standardmäßig nicht gesetzt ist (also schwarz). Ich lese jetzt
		// hier einfach die Windows-Farbe für den Fenstertext.
		TextColor = ::GetSysColor(COLOR_WINDOWTEXT);
		BkColor = GetBkColor();

		if (i == m_nSortedColumn)
			BkColor = DarkenColor(10, BkColor);
//		else
		{
			// Jede 2.Zeile leicht einfärben
			if (m_bEnableDoubleColoring && (lpDrawItemStruct->itemID % 2))
				BkColor = DarkenColor(10, BkColor);
		}

		if ((int)lpDrawItemStruct->itemID < m_nItemColorLast && m_ItemColor[lpDrawItemStruct->itemID])
			TextColor = m_ItemColor[lpDrawItemStruct->itemID];
		
		if ((lpDrawItemStruct->itemState & ODS_SELECTED))
		{
			if (m_ActiveItem > 0 && (int)lpDrawItemStruct->itemID == m_ActiveItem-1)
			{
//				pDC->SetBkColor(LightenColor(193, TextColor));
//				pDC->SetTextColor(RGB(0, 0, 0));
				pDC->SetBkColor(LightenColor(193, ::GetSysColor(COLOR_ACTIVECAPTION)));
				pDC->SetTextColor(RGB(0, 0, 0));
				oldfont = pDC->SelectObject(&m_BoldFont);
			}
			else
			{
				pDC->SetBkColor(LightenColor(193, ::GetSysColor(COLOR_ACTIVECAPTION)));
				pDC->SetTextColor(RGB(0, 0, 0));
			}
		}
		else
		{
			if (m_ActiveItem > 0 && (int)lpDrawItemStruct->itemID == m_ActiveItem-1)
			{
/*				if (m_bActiveItemColorSet)
				{
					pDC->SetBkColor(m_rgbActiveItemBkColor);
					pDC->SetTextColor(m_rgbActiveItemTextColor);
				}
				else
				{
					pDC->SetBkColor(m_rgbActiveItemBkColor);
					pDC->SetTextColor(m_rgbActiveItemTextColor);
//					pDC->SetBkColor(((CHitbaseWinAppBase*)AfxGetApp())->m_config.m_colorCurrentTrackBackground);
//					pDC->SetTextColor(TextColor);
				}*/
					// Keine benutzerdefinierte Hintergrundfarbe mehr für aktuelles Lied
				pDC->SetBkColor(BkColor);
				pDC->SetTextColor(TextColor);
				oldfont = pDC->SelectObject(&m_BoldFont);
			}
			else
			{
				pDC->SetBkColor(BkColor);
				pDC->SetTextColor(TextColor);
			}
		}
		
		if (m_Column[i].dwFlags & GLCCF_GRAYFIELDS)
			pDC->SetBkColor(RGB(192, 192, 192));

		rect.left = lpDrawItemStruct->rcItem.left+xpos;
		rect.top = lpDrawItemStruct->rcItem.top;
		rect.right = lpDrawItemStruct->rcItem.left+xpos+col.cx;
		rect.bottom = lpDrawItemStruct->rcItem.bottom;

		if (nImage > 0 && GetCellType(lpDrawItemStruct->itemID, i) == GLCCT_BITMAP)   // Bitmap
		{
			pDC->ExtTextOut(rect.left+5, rect.top, ETO_CLIPPED|ETO_OPAQUE, 
				&rect, L"", NULL);

			CImageList* pil = GetImageList(LVSIL_SMALL);
			pil->Draw(pDC, nImage-1, CPoint(lpDrawItemStruct->rcItem.left+xpos+5, lpDrawItemStruct->rcItem.top), ILD_TRANSPARENT);
		}
		else
		{
			CRect rectDraw(rect.left+5, rect.top, rect.right, rect.bottom);
			CRect rectFill(rect.left, rect.top, rect.right-1, rect.bottom);
//			if (!m_pBackgroundPicture || (lpDrawItemStruct->itemState & ODS_SELECTED))
//			{
//				pDC->ExtTextOut(rectDraw.left, rectDraw.top, ETO_CLIPPED|ETO_OPAQUE, 
//						&rectFill, L"", NULL);
//			}
//			else
//			{
				// Hintergrund mit dem Hintergrundbild füllen
//				m_pBackgroundPicture->DrawPart(pDC, rectFill.TopLeft(), 
//					CRect(m_ptBackgroundPictureOffset+rectFill.TopLeft(), rectFill.Size()));
//			}

			CMisc::CutLength(pDC, str, rectDraw.Width());
//			if (m_pBackgroundPicture)
//				pDC->SetBkMode(TRANSPARENT);
			pDC->DrawText(str, &rectDraw, DT_VCENTER|DT_SINGLELINE|DT_NOPREFIX);
//			if (m_pBackgroundPicture)
//				pDC->SetBkMode(OPAQUE);
		}
		
		if (oldfont)
			pDC->SelectObject(oldfont);

		if (GetCellFlags(lpDrawItemStruct->itemID, i) & GLCCELLFLAGS_CROSS)
		{
			CPen pen;
			pDC->FillSolidRect(CRect(rect.left, (rect.top+rect.bottom)/2, rect.right, (rect.top+rect.bottom)/2+1), TextColor);
		}

		if (m_dwFlags & GLCF_GRID)
		{
			oldpen = pDC->SelectObject(&GridPen);
			oldbrush = (CBrush *)pDC->SelectStockObject(NULL_BRUSH);
			
			rect1.SetRect(rect.left-1, rect.top-1, rect.right, rect.bottom);
			pDC->MoveTo(rect.right-1, rect.top);
			pDC->LineTo(rect.right-1, rect.bottom-1);
			pDC->LineTo(rect.left, rect.bottom-1);
			
//			pDC->Rectangle(&rect1);
			pDC->SelectObject(oldpen);
			pDC->SelectObject(oldbrush);
		}
		
		xpos += col.cx;
		i++;
	}
	
	if (lpDrawItemStruct->itemState & ODS_FOCUS && GetFocus() == this)
	{
		rect.left = lpDrawItemStruct->rcItem.left;
		rect.top = lpDrawItemStruct->rcItem.top;
		rect.right = lpDrawItemStruct->rcItem.left+xpos+col.cx;
		rect.bottom = lpDrawItemStruct->rcItem.bottom;

		oldpen = pDC->SelectObject(&RectPen);
		oldbrush = (CBrush *)pDC->SelectStockObject(NULL_BRUSH);
//		pDC->DrawFocusRect(&rect);
		pDC->Rectangle(&rect);
		pDC->SelectObject(oldpen);
		pDC->SelectObject(oldbrush);
	}

	pDC->SetBkColor(oldBkColor);
}

BOOL CGridListCtrl::SetTextColor(int nItem, COLORREF cr, BOOL bUpdate /* = TRUE */)
{
	int *newItemColor;
	
	if (nItem >= m_nItemColorLast)
	{
		newItemColor = new int[nItem+1];
		
		::ZeroMemory(newItemColor, (nItem+1)*sizeof(int));
		if (m_ItemColor)
		{
			memcpy(newItemColor, m_ItemColor, m_nItemColorLast*sizeof(int));
			delete m_ItemColor;
		}
		m_ItemColor = newItemColor;
		m_nItemColorLast = nItem+1;
	}
	
	m_ItemColor[nItem] = cr;
	
	if (bUpdate)
		InvalidateRect(NULL, FALSE);

	return TRUE;
}

// Liefert die Farbe der angegebenen Zeile zurück.
BOOL CGridListCtrl::GetItemColor(int nItem)
{
   if (nItem < m_nItemColorLast)
   {
      return m_ItemColor[nItem];
   }
   
   return GetTextColor();
}


// Liefert TRUE zurück, wenn die angegebene Zeile eine User-Defined Farbe besitzt.
// Wenn cr angegeben wird, wird direkt die Farbe zurückgeliefert.
BOOL CGridListCtrl::HasUserColor(int nItem, COLORREF* cr /* = NULL */)
{
   if (nItem < m_nItemColorLast)
   {
      if (cr)
         *cr = m_ItemColor[nItem];

      return (m_ItemColor[nItem] != 0);
   }
   
   return FALSE;
}

void CGridListCtrl::SetActiveItem(int item)
{
	if (item+1 != m_ActiveItem)
	{
		m_ActiveItem = item+1;
		InvalidateRect(NULL, FALSE);
	}
}

void CGridListCtrl::SetColumnFlags(int col, DWORD dwFlags)
{
    m_Column[col].dwFlags = dwFlags;
}

void CGridListCtrl::OnClick(NMHDR* pNMHDR, LRESULT* pResult) 
{
	int ClickedItem;
	UINT result;
	POINT point;
	
	GetCursorPos(&point);
	GetDesktopWindow()->MapWindowPoints(this, &point, 1);
	
	CloseEditWindow();
	
	ClickedItem = HitTest(point, &result) + 1;
	
	if (!ClickedItem)     // Auf keine Element geklickt!
		return;
	
	if (m_dwFlags & GLCF_SIMPLE)
		m_SelectedItem = ClickedItem;
	
	if (ClickedItem == m_SelectedItem)    // Jetzt wird das Feld editiert
	{
		LV_COLUMN col;
		int xpos=0, Column=0;
		CRect ItemRect;
		CRect rect;
		
		GetItemRect(ClickedItem-1, &ItemRect, LVIR_BOUNDS);
		rect = ItemRect;
		
		// Zuerst noch prüfen, ob die angeklickte Spalte geändert werden darf.
		while (1)
		{
			memset(&col, 0, sizeof(col));
			col.mask = LVCF_WIDTH;
			
			if (!GetColumn(Column, &col))
				break;
			
			ItemRect.left = xpos + rect.left;
			ItemRect.right = xpos + col.cx + rect.left;
			
			if (ItemRect.PtInRect(point))
			{
				EditSubItemLabel(ClickedItem-1, Column);
			}
			
			xpos += col.cx;
			Column++;
		}
	}
	else
	{
		m_SelectedItem = ClickedItem;
	}

	// Jetzt noch testen, ob auf ein Symbol geklickt wurden.
	LVHITTESTINFO hti;
	memset(&hti, 0, sizeof(hti));
	hti.pt.x = point.x;
	hti.pt.y = point.y;

	ClickedItem = HitTest(&hti) + 1;
	
	CRect ItemRect, rect;
	GetItemRect(ClickedItem-1, &ItemRect, LVIR_BOUNDS);
	rect = ItemRect;
	
	int xpos = 0;
	int nColumn;
	// Zuerst noch prüfen, ob die angeklickte Spalte geändert werden darf.
	for (nColumn=0;nColumn < GetColumnCount();nColumn++)
	{
		int colWidth = GetColumnWidth(nColumn);
		
		ItemRect.left = xpos + rect.left;
		ItemRect.right = xpos + colWidth + rect.left;
		
		if (ItemRect.PtInRect(point))
		{
            break;
		}
		
		xpos += colWidth;
	}

	if (ClickedItem && GetCellType(hti.iItem, nColumn) == GLCCT_BITMAP && GetCellValue(hti.iItem, nColumn) > 0)     // Cursor steht auf einem Element
	{
		::SendMessage(GetParent()->m_hWnd, GLCM_CLICKEDSYMBOL, GetDlgCtrlID(), MAKELPARAM(nColumn, hti.iItem));
	}
	
	*pResult = 0;
}

void CGridListCtrl::OnLButtonDown(UINT nFlags, CPoint point) 
{
	BOOL bCurrentlyActive = m_EditActive;

	CListCtrl::OnLButtonDown(nFlags, point);

	if (bCurrentlyActive)
		CloseEditWindow();
}

void CGridListCtrl::OnNMSetfocus(NMHDR *pNMHDR, LRESULT *pResult)
{
	int iCurSel = GetCurSel();

	if (iCurSel < 0)
		iCurSel = 0;

	SetItemState(iCurSel, LVIS_FOCUSED|LVIS_SELECTED, LVIS_FOCUSED|LVIS_SELECTED);
}

void CGridListCtrl::OnNMKillfocus(NMHDR *pNMHDR, LRESULT *pResult)
{
	int iCurSel = GetCurSel();

	if (iCurSel < 0)
		iCurSel = 0;

	SetItemState(iCurSel, LVIS_FOCUSED|LVIS_SELECTED, LVIS_SELECTED);
}

BOOL CGridListCtrl::EditSubItemLabel(int nItem, int nSubItem)
{
	CRect rect;
	CRect rect1;
	CString label = L"";
	LV_COLUMN col;
	int xpos=0, Column=0;
	
	CloseEditWindow();
	
	GetItemRect(nItem, &rect, LVIR_BOUNDS);
	
	// Position des Feldes ermitteln
	while (Column < nSubItem)
	{
		memset(&col, 0, sizeof(col));
		col.mask = LVCF_WIDTH;
		
		if (!GetColumn(Column, &col))
			break;
		
		xpos += col.cx;
		Column++;
	}
	
	memset(&col, 0, sizeof(col));
	col.mask = LVCF_WIDTH;
	if (!GetColumn(Column, &col))
		return NULL;
	
	label = GetItemText(nItem, nSubItem);
	
	if (m_Column[Column].dwFlags & GLCCF_EDIT &&
		(nItem >= m_Cell.GetSize() || Column >= m_Cell[nItem].GetSize() ||
		 !m_Cell[nItem][Column].m_nType))
	{
		rect1.left = xpos + rect.left;
		rect1.right = xpos + col.cx + rect.left;
		rect1.top = rect.top;
		rect1.bottom = rect.bottom-1;
		rect1.right --;
		
		m_EditSubItemLabel.m_GridListCtrl = this;
		if (nItem < m_Cell.GetSize() && Column < m_Cell[nItem].GetSize())	
			m_EditSubItemLabel.SetFormat(m_Cell[nItem][Column].m_iFormat);
		if (m_Column[Column].dwFlags & GLCCF_CODES)
			m_EditSubItemLabel.Create(WS_CHILD|WS_VISIBLE|ES_AUTOHSCROLL|ES_UPPERCASE, rect1, this, 100);
		else
			m_EditSubItemLabel.Create(WS_CHILD|WS_VISIBLE|ES_AUTOHSCROLL, rect1, this, 100);
		m_EditSubItemLabel.SetFont(GetFont());
		
		m_EditSubItemLabel.SetWindowText(label);
		m_EditSubItemLabel.SetFocus();
		m_EditSubItemLabel.SetSel(0, -1, FALSE);
		if (m_Column[Column].nEditWidth > 0)
			m_EditSubItemLabel.LimitText(m_Column[Column].nEditWidth);
		
		m_EditActive = TRUE;
		m_EditItem = nItem;
		m_EditSubItem = nSubItem;
	}
	
	if (nItem < m_Cell.GetSize() && Column < m_Cell[nItem].GetSize())
	{
		switch (m_Cell[nItem][Column].m_nType)
		{
			case GLCCT_COMBOBOX:
			{
				rect1.left = xpos + rect.left;
				rect1.right = xpos + col.cx + rect.left;
				rect1.top = rect.top-3;
				rect1.bottom = rect.bottom+1;
				CreateComboBox(nItem, Column, &rect1, label);
				
				m_EditItem = nItem;
				m_EditSubItem = nSubItem;
				break;
			}

			case GLCCT_COLOR:
			{
				rect1.left = xpos + rect.left;
				rect1.right = xpos + col.cx + rect.left;
				rect1.top = rect.top;
				rect1.bottom = rect.bottom-1;
				rect1.right -=15;
				
				m_EditSubItemLabel.m_GridListCtrl = this;
				m_EditSubItemLabel.Create(WS_CHILD|WS_VISIBLE|ES_AUTOHSCROLL, rect1, this, 100);
				m_EditSubItemLabel.SetFont(GetFont());

				rect1.top --;
				rect1.left = rect1.right;
				rect1.right += 15;
				rect1.bottom ++;

				m_EditColorCtrl.Create(L"...", WS_CHILD|WS_VISIBLE|BS_PUSHBUTTON, rect1, this, 101);
				m_EditColorCtrl.SetFont(GetFont());
				
				m_EditSubItemLabel.SetWindowText(label);
				m_EditSubItemLabel.SetFocus();
				m_EditSubItemLabel.SetSel(0, -1, FALSE);
				if (m_Column[Column].nEditWidth > 0)
					m_EditSubItemLabel.LimitText(m_Column[Column].nEditWidth);
				
				m_EditActive = TRUE;
				m_EditItem = nItem;
				m_EditSubItem = nSubItem;
			}
		}
	}
	
	return TRUE;
}

// Liefert die aktuell selektierte Zeile zurück (-1, wenn ja nix)
int CGridListCtrl::GetCurSel()
{
	POSITION pos = NULL;
	return GetNextItem(-1, LVIS_SELECTED);
 /* JUS 15.6.2002: ALT! VIEL ZU LANGSAM!!!
 for (int i=0;i<GetItemCount();i++)
   {
      if (GetItemState(i, LVIS_SELECTED))
         return i;
   }

   return -1;*/
}

void CGridListCtrl::SetCurSel(int iCurSel)
{
	for (int i=0;i<GetItemCount();i++)
		SetItemState(i, 0, LVIS_SELECTED);

	SetItemState(iCurSel, LVIS_SELECTED, LVIS_SELECTED);
}

void CGridListCtrl::CloseEditWindow()
{
	if (m_EditActive)
	{
		if (m_EditItem < m_Cell.GetSize() && m_EditSubItem < m_Cell[m_EditItem].GetSize())
		{
			CString sText;
			m_EditSubItemLabel.GetWindowText(sText);
			
			switch (m_Cell[m_EditItem][m_EditSubItem].m_iFormat)
			{
			case UDFF_TEXT:
				break;
			case UDFF_CURRENCY:
				{
					if (!sText.IsEmpty())
						m_EditSubItemLabel.SetWindowText(CMisc::FormatCurrencyValue(GetCurrencyValue(sText)));
					break;
				}
			case UDFF_NUMBER:
				// Text eliminieren und in Zahl konvertieren!
				if (!sText.IsEmpty())
					sText.Format(L"%g", _wtof(sText));
				m_EditSubItemLabel.SetWindowText(sText);
				break;
			}
		}
		
		CString text;
		
		m_EditSubItemLabel.GetWindowText(text);
		SetItemText(m_EditItem, m_EditSubItem, text);
		m_EditSubItemLabel.DestroyWindow();
		m_EditActive = FALSE;
		
		// Prüfen, ob auch noch Color-Button gelöscht werden muß!
		if (m_EditItem < m_Cell.GetSize() && m_EditSubItem < m_Cell[m_EditItem].GetSize() &&
			m_Cell[m_EditItem][m_EditSubItem].m_nType & GLCCT_COLOR)
		{
			m_EditColorCtrl.DestroyWindow();
		}
		
		NMLVDISPINFO dispinfo;
		dispinfo.hdr.hwndFrom = *this;
		dispinfo.hdr.idFrom = GetDlgCtrlID();
		dispinfo.hdr.code = LVN_ENDLABELEDIT;
		dispinfo.item.iItem = m_EditItem;
		dispinfo.item.iSubItem = m_EditSubItem;
		dispinfo.item.pszText = (LPTSTR)(LPCTSTR)text;
		::SendMessage(GetParent()->m_hWnd, WM_NOTIFY, GetDlgCtrlID(), (LPARAM)&dispinfo);

		m_EditSubItem = -1;
	}
	
	if (m_ComboBoxActive)
	{
		m_ComboBoxActive = FALSE;
		CString text;
		int sel = m_ComboBox.GetCurSel();
		
		if (sel >= 0)
		{
			m_ComboBox.GetLBText(sel, text);
			SetItemText(m_EditItem, m_EditSubItem, text);
		}
		m_ComboBox.DestroyWindow();
	}
}

void CGridListCtrl::OnHScroll(UINT nSBCode, UINT nPos, CScrollBar* pScrollBar) 
{
	CloseEditWindow();
	
//	if (m_pBackgroundPicture)
//		Invalidate(FALSE);

	CListCtrl::OnHScroll(nSBCode, nPos, pScrollBar);
}

void CGridListCtrl::OnVScroll(UINT nSBCode, UINT nPos, CScrollBar* pScrollBar) 
{
	CloseEditWindow();
	
//	if (m_pBackgroundPicture)
//		Invalidate(FALSE);

	CListCtrl::OnVScroll(nSBCode, nPos, pScrollBar);
}

int CGridListCtrl::GetColumnCount(void)
{
   //int Column = 0;
   //LV_COLUMN col;

   //while (1)
   //   {
   //   memset(&col, 0, sizeof(col));
	  // col.mask = LVCF_WIDTH;

	  // if (!GetColumn(Column, &col))
		 // break;

   //   Column++;
   //   }

   //return Column;

	//Get the header control
    CHeaderCtrl* pHeader = (CHeaderCtrl*)GetHeaderCtrl();

    //Return the number of items in it (i.e. the number of columns)
    return pHeader->GetItemCount();
}

void CGridListCtrl::SetColumnEditWidth(int col, int MaxLength)
{
   m_Column[col].nEditWidth = MaxLength;
}

void CGridListCtrl::OnDeleteallitems(NMHDR* pNMHDR, LRESULT* pResult) 
{
	CloseEditWindow();
	
	if (m_ItemColor)
		delete m_ItemColor;
	m_nItemColorLast = 0;
	m_ItemColor = NULL;

	m_Cell.RemoveAll();
	
	*pResult = 0;
}

// Vertauscht den Inhalt der angegebenen Zeilen.
// Wenn nSecond = -1 ist, so wird mit der nächsten Zeile getauscht.
void CGridListCtrl::SwapLines(int nFirst, int nSecond /* = -1 */)
{
   int count = GetColumnCount();
   CString str[20], str1;
   int i;

   if (nSecond == -1)
      nSecond = nFirst + 1;

   // Zuerst alle Inhalte merken
   for (i=0;i<count;i++)
      str[i] = GetItemText(nFirst, i);

   for (i=0;i<count;i++)
   {
      str1 = GetItemText(nSecond, i);
      SetItem(nFirst, i, LVIF_TEXT, str1, 0, 0, 0, 0);
      SetItem(nSecond, i, LVIF_TEXT, str[i], 0, 0, 0, 0);
   }
}


void CGridListCtrl::MoveLines(int iIndex, int iOffset)
{
	int count = GetColumnCount();
	CString str[20], str1;
	int i;
	
	// Zuerst alle Inhalte merken
	for (i=0;i<count;i++)
		str[i] = GetItemText(iIndex, i);
	
	DeleteItem(iIndex);

	int iNewIndex = InsertItem(iIndex+iOffset, L"");

	for (i=0;i<count;i++)
		SetItem(iNewIndex, i, LVIF_TEXT, str[i], 0, 0, 0, 0);
}

BOOL CGridListCtrl::DeleteColumn(int nCol)
{
   for (int i=0;i<_MAX_COLUMNS;i++)
	{
      m_Column[i].dwFlags = 0;
      m_Column[i].nEditWidth = 0;
	}

	return CListCtrl::DeleteColumn(nCol);
}

void CGridListCtrl::DeleteAllColumns()
{
	int nColumnCount = GetHeaderCtrl()->GetItemCount();

	// Delete all of the columns.
	for (int i=0;i < nColumnCount;i++)
	{
		DeleteColumn(0);
	}

	m_nSortedColumn = -1;
}

void CGridListCtrl::OnKeyDown(UINT nChar, UINT nRepCnt, UINT nFlags) 
{
   short nKeyStateShift = GetKeyState(VK_SHIFT); 
   short nKeyStateCtrl = GetKeyState(VK_CONTROL);

   if (nChar == VK_F2
       && nKeyStateShift >= 0        // kein Shift gedrückt
       && nKeyStateCtrl >= 0)        // kein Ctrl gedrückt
   {
      int i, sc;

      CloseEditWindow();

      i = GetCurSel();

      if (i != -1)
      {
         for (sc=0;sc<GetColumnCount()-1;sc++)
             if (m_Column[sc].dwFlags & GLCCF_EDIT)
                break;

         EditSubItemLabel(i, sc);
      }
   }

   if (m_dwFlags & GLCF_ENABLEKEYS)
   {
	   if (nChar == VK_DELETE)
         DeleteSelectedItems();
   }
	
	CListCtrl::OnKeyDown(nChar, nRepCnt, nFlags);
}

// Löscht alle selektierten Elemente aus der Liste.
void CGridListCtrl::DeleteSelectedItems()
{
	int sel=-1, FirstSelected, count = 0;

	while (GetCurSel() >= 0)
	{
		sel = GetCurSel();
		if (OnItemDelete(sel))
		{
			CListCtrl::DeleteItem(sel);
			if (!count)
				FirstSelected = sel;
			count ++;
		}
	}

	if (count == 1)
	{
		if (FirstSelected >= GetItemCount())
			FirstSelected = GetItemCount()-1;

		if (FirstSelected >= 0)
			SetItemState(FirstSelected, LVIS_SELECTED, LVIS_SELECTED);
	}
}

BOOL CGridListCtrl::OnItemDelete(int iItem)
{
	return TRUE;
}

int CGridListCtrl::GetCellType(int nRow, int nColumn)
{
	if (nRow >= m_Cell.GetSize() || nColumn >= m_Cell[nRow].GetSize())
		return GLCCT_STRING;

	return m_Cell[nRow][nColumn].m_nType;
}

void CGridListCtrl::SetCellType(int nRow, int nColumn, int nType)
{
	if (nRow >= m_Cell.GetSize())
		m_Cell.SetSize(nRow+1);

	if (nColumn >= m_Cell[nRow].GetSize())
		m_Cell[nRow].SetSize(nColumn+1);

	m_Cell[nRow][nColumn].m_nType = nType;
}

DWORD CGridListCtrl::GetCellFlags(int nRow, int nColumn)
{
	if (nRow >= m_Cell.GetSize() || nColumn >= m_Cell[nRow].GetSize())
		return 0;

	return m_Cell[nRow][nColumn].m_dwFlags;
}

void CGridListCtrl::SetCellFlags(int nRow, int nColumn, DWORD dwFlags)
{
	if (nRow >= m_Cell.GetSize())
		m_Cell.SetSize(nRow+1);

	if (nColumn >= m_Cell[nRow].GetSize())
		m_Cell[nRow].SetSize(nColumn+1);

	m_Cell[nRow][nColumn].m_dwFlags = dwFlags;

	InvalidateRect(NULL, FALSE);
}

void CGridListCtrl::SetCellFormat(int nRow, int nColumn, int iFormat)
{
	if (iFormat == UDFF_BOOL)
	{
		SetCellType(nRow, nColumn, GLCCT_COMBOBOX);
		CStringArray cbItems;
		cbItems.Add(get_string(TEXT_YES));
		cbItems.Add(get_string(TEXT_NO));
		SetComboBoxItems(&cbItems, nRow, nColumn);
		return;
	}

	if (nRow >= m_Cell.GetSize())
		m_Cell.SetSize(nRow+1);

	if (nColumn >= m_Cell[nRow].GetSize())
		m_Cell[nRow].SetSize(nColumn+1);

	m_Cell[nRow][nColumn].m_iFormat = iFormat;
}

void CGridListCtrl::SetFlags(DWORD dwFlags)
{
   m_dwFlags = dwFlags;
}

BOOL CGridListCtrl::SetComboBoxItems(CStringArray* cbItems, int nRow, int nColumn, DWORD dwcbFlags)
{
	m_Cell.SetSize(nRow+1);
	m_Cell[nRow].SetSize(nColumn+1);

	ASSERT(m_Cell[nRow][nColumn].m_nType & GLCCT_COMBOBOX);

	for (int i=0;i<cbItems->GetSize();i++)
		m_Cell[nRow][nColumn].m_cbItems.Add(cbItems->ElementAt(i));
	m_Cell[nRow][nColumn].m_dwcbFlags = dwcbFlags;
	m_Cell[nRow][nColumn].m_nValue = -1;
	return TRUE;
}

BOOL CGridListCtrl::CreateComboBox(int nRow, int nColumn, CRect* pRect, CString label)
{
	int i;
	
	pRect->bottom += 200;
	m_ComboBox.Create(m_Cell[nRow][nColumn].m_dwcbFlags|WS_CHILD|WS_VISIBLE|WS_VSCROLL, *pRect, this, 1);
	
	for (i=0;i<m_Cell[nRow][nColumn].m_cbItems.GetSize();i++)
		m_ComboBox.AddString(m_Cell[nRow][nColumn].m_cbItems[i]);
	
	int sel = m_ComboBox.FindStringExact(-1, label);
	m_ComboBox.SetCurSel(sel);
	m_ComboBox.SetFocus();
	
	m_ComboBox.ShowDropDown();

	m_ComboBox.m_iRow = nRow;
	m_ComboBox.m_iColumn = nColumn;
	m_ComboBox.m_pGridListCtrl = this;

	m_ComboBoxActive = TRUE;
	
	return TRUE;
}

void CGridListCtrl::OnChangeColor() 
{
	CString sColor = GetItemText(m_EditItem, m_EditSubItem);

	CColorDialog ColorDlg(wcstol(sColor, 0, 16));

	if (ColorDlg.DoModal() == IDOK)
	{
		sColor.Format(L"%06x", ColorDlg.GetColor());
		m_EditSubItemLabel.SetWindowText(sColor);
	}
}

int CGridListCtrl::SetCellValue(int nRow, int nCol, int nValue)
{
	if (nRow >= m_Cell.GetSize())
		m_Cell.SetSize(nRow+1);
	if (nCol >= m_Cell[nRow].GetSize())
		m_Cell[nRow].SetSize(nCol+1);

	m_Cell[nRow][nCol].m_nValue = nValue;

	if (GetCellType(nRow, nCol) == GLCCT_COMBOBOX)
	{
		SetItemText(nRow, nCol, m_Cell[nRow][nCol].m_cbItems[nValue]);
	}

	return TRUE;
}

int CGridListCtrl::GetCellValue(int nRow, int nCol)
{
	if (nRow >= m_Cell.GetSize() || nCol >= m_Cell[nRow].GetSize())
		return 0;

	return m_Cell[nRow][nCol].m_nValue;
}

// In einer Spalte mit Bildern ändert sich der Cursor zu einer Hand, wenn
// in der aktuellen Zeile ein Bild steht (der Cursor also über dem Symbol).

BOOL CGridListCtrl::OnSetCursor(CWnd* pWnd, UINT nHitTest, UINT message) 
{
    int PointedItem;
    POINT point;

	GetCursorPos(&point);
	GetDesktopWindow()->MapWindowPoints(this, &point, 1);
	
	LVHITTESTINFO hti;
	memset(&hti, 0, sizeof(hti));
	hti.pt.x = point.x;
	hti.pt.y = point.y;

	PointedItem = HitTest(&hti) + 1;
	
	CRect ItemRect, rect;
	GetItemRect(PointedItem-1, &ItemRect, LVIR_BOUNDS);
	rect = ItemRect;
	
	int xpos = 0;
	int nColumn;
	// Zuerst noch prüfen, ob die angeklickte Spalte geändert werden darf.
	for (nColumn=0;nColumn < GetColumnCount();nColumn++)
	{
		int colWidth = GetColumnWidth(nColumn);
		
		ItemRect.left = xpos + rect.left;
		ItemRect.right = xpos + colWidth + rect.left;
		
		if (ItemRect.PtInRect(point))
		{
            break;
		}
		
		xpos += colWidth;
	}

	if (PointedItem && GetCellType(hti.iItem, nColumn) == GLCCT_BITMAP && GetCellValue(hti.iItem, nColumn) > 0)     // Cursor steht auf einem Element
	{
		::SetCursor(AfxGetApp()->LoadCursor(IDC_HANDPOINTER));

		return TRUE;
	}
	
	return CListCtrl::OnSetCursor(pWnd, nHitTest, message);
}


BOOL CGridListCtrl::AddToolTip(int nCol, const CString &sText)
{
	return TRUE;

/*	if (!::IsWindow(m_ttc))
	{
		m_ttc.Create(this);
	}

	CRect rect, rectCol;
	GetClientRect(&rect);
	GetSubItemRect(0, nCol, LVIR_BOUNDS, rectCol);
	//rect.left = rectCol.left;
	//rect.right = rectCol.right;
	m_ttc.AddTool(this, sText, &rect, 1);
	m_ttc.Activate(TRUE);
	EnableToolTips();

	return TRUE;*/
}


// Speichert die Breite aller Spalten in der Registry im subkey "ListCtrlSizes"
// Als Prefix wird KeyName benutzt (z.B. "MainList" -> "MainList[0-n]")
void CGridListCtrl::SaveColumnWidth(const CString& KeyName)
{
   int Column = 0, width;
   DWORD ret;
   LV_COLUMN col;
   HKEY hHitKey;
   CString str;

   RegCreateKeyEx(HKEY_CURRENT_USER, (CString)REGISTRY_KEY + L"\\ListCtrlSizes", 0,
                  L"Hitbase", REG_OPTION_NON_VOLATILE, KEY_ALL_ACCESS, NULL,
                  &hHitKey, &ret);

   while (1)
   {
      memset(&col, 0, sizeof(col));
      col.mask = LVCF_WIDTH;
      
      if (!GetColumn(Column, &col))
         break;
      
      str.Format(L"%s%d", KeyName, Column+1);
      RegSetValueEx(hHitKey, str, 0, REG_DWORD, (unsigned char *)&col.cx, sizeof(int));
      
      Column++;
   }

   width = 0;
   str.Format(L"%s%d", KeyName, Column+1);
   RegSetValueEx(hHitKey, str, 0, REG_DWORD, (unsigned char *)&width, sizeof(int));
      
   RegCloseKey(hHitKey);
}

// Läd die Breite aller Spalten aus der Registry im subkey "ListCtrlSizes"
void CGridListCtrl::LoadColumnWidth(const CString& KeyName)
{
   int Column = 0, width;
   DWORD ret;
   HKEY hHitKey;
   CString str;

   if (RegOpenKeyEx(HKEY_CURRENT_USER, (CString)REGISTRY_KEY + L"\\ListCtrlSizes", 0,
                  KEY_ALL_ACCESS, &hHitKey) != ERROR_SUCCESS)
      return;

   while (1)
   {
      str.Format(L"%s%d", KeyName, Column+1);

      width = 0;
      ret = sizeof(int);
      RegQueryValueEx(hHitKey, str, 0, NULL, (unsigned char *)&width, &ret);
      if (!ret || !width)
         break;

      if (!SetColumnWidth(Column, width))
         break;
      
      Column++;
   }

   RegCloseKey(hHitKey);
}

// Füllt das Control mit einer großen Anzahl von (Leer-)Elementen
void CGridListCtrl::SetNumberOfItems(const int nItemCount)
{

}

void CGridListCtrl::SetActiveItemColor(COLORREF rgbTextColor, COLORREF rgbBkColor)
{
	m_rgbActiveItemTextColor = rgbTextColor;
	m_rgbActiveItemBkColor = rgbBkColor;
	m_bActiveItemColorSet = true;
}

// Elemente in der angegebenen Spalte sortieren.
void CGridListCtrl::Sort(int nColumn, BOOL bAscending, ListCompareType nCompareAs)
{
	CWaitCursor wait;

	int max = GetItemCount();
	DWORD dw;
	CString txt;
	
	// replace Item data with pointer to CSortItem structure
	for (int t = 0; t < max; t++)
	{
		dw = GetItemData(t); // save current data to restore it later
		txt = GetItemText(t, nColumn); 
		SetItemData(t, (DWORD) new CGridListSortItem(dw, txt, GetItemColor(t)));
	}

	long lParamSort = nCompareAs;
	
	// if lParamSort positive - ascending sort order, negative - descending
	if (!bAscending)
		lParamSort *= -1; 
	
	SortItems(CompareItems, lParamSort);

	// Und wieder die alten Inhalte rein.
	CGridListSortItem * pItem;
	for (int t = 0; t < max; t++)
	{
		pItem = (CGridListSortItem *) GetItemData(t);
		ASSERT(pItem);
		SetItemData(t, pItem->m_dwParam);
		SetTextColor(t, pItem->m_dwItemColor);
		delete pItem;
	}

	m_Cell.RemoveAll();
}

int CALLBACK CGridListCtrl::CompareItems(LPARAM lParam1, LPARAM lParam2, LPARAM lParamSort)
{
	CGridListSortItem * item1 = (CGridListSortItem *) lParam1;
	CGridListSortItem * item2 = (CGridListSortItem *) lParam2;
	ASSERT(item1 && item2);
	
	// restore data type and sort order from lParamSort
	// if lParamSort positive - ascending sort order, negative - descending
	int iOrder = lParamSort < 0 ? -1 : 1; 
	ListCompareType dType  = (ListCompareType) (lParamSort * iOrder); // get rid of sign
	
	// declare typed buffers
	COleDateTime t1, t2;
	
	switch (dType)
	{
	case ELCT_INTEGER:
		return (_wtol(item1->m_sText) - _wtol(item2->m_sText)) * iOrder;
	case ELCT_DOUBLE:
		return (_wtof(item1->m_sText) < _wtof(item2->m_sText) ? -1 : 1) * iOrder;
	case ELCT_DATE_TIME:
		if (t1.ParseDateTime(item1->m_sText) && t2.ParseDateTime(item2->m_sText))
			return (t1 < t2 ? -1 : 1 ) * iOrder;
		else
			return 0;
	case ELCT_STRING_CASE:
		return wcscmp(item1->m_sText, item2->m_sText) * iOrder;
	case ELCT_STRING_NOCASE:
		return item1->m_sText.CompareNoCase(item2->m_sText) * iOrder;
		
	default:
		ASSERT(FALSE);
		return 0;
	}
}

// Die Spalte markieren, nach der sortiert wird (leichter grau-schimmer und Pfeil)
void CGridListCtrl::SetSortColumn(int iColumn, BOOL bAscending)
{
	m_nSortedColumn = iColumn;
	m_bSortAscending = bAscending;

	if (!CMisc::IsXPVisualStyle())
	{
		CreateSortIcons();
		GetHeaderCtrl()->SetImageList(&m_ImageListSortIcons);
	}

	SetSortIcon();
}

// Function splits a color into its RGB components and
// transforms the color using a scale 0..255
COLORREF CGridListCtrl::DarkenColor( long lScale, COLORREF lColor)
{ 
  long red   = MulDiv(GetRValue(lColor),(255-lScale),255);
  long green = MulDiv(GetGValue(lColor),(255-lScale),255);
  long blue  = MulDiv(GetBValue(lColor),(255-lScale),255);

  return RGB(red, green, blue); 
}

// Function splits a color into its RGB components and
// transforms the color using a scale 0..255
COLORREF CGridListCtrl::LightenColor( long lScale, COLORREF lColor)
{ 
  long R = MulDiv(255-GetRValue(lColor),lScale,255)+GetRValue(lColor);
  long G = MulDiv(255-GetGValue(lColor),lScale,255)+GetGValue(lColor);
  long B = MulDiv(255-GetBValue(lColor),lScale,255)+GetBValue(lColor);

  return RGB(R, G, B); 
}

void CGridListCtrl::MeasureItem(LPMEASUREITEMSTRUCT lpMeasureItemStruct)
{
	LOGFONT lf;
	GetFont()->GetLogFont( &lf );

	lpMeasureItemStruct->itemHeight = abs(lf.lfHeight)+6;
}

LRESULT CGridListCtrl::OnSetFont(WPARAM wParam, LPARAM)
{
	LRESULT res = Default();

	CRect rc;
	GetWindowRect( &rc );

	WINDOWPOS wp;
	wp.hwnd = m_hWnd;
	wp.cx = rc.Width();
	wp.cy = rc.Height();
	wp.flags = SWP_NOACTIVATE | SWP_NOMOVE | SWP_NOOWNERZORDER | SWP_NOZORDER;
	::SendMessage( m_hWnd, WM_WINDOWPOSCHANGED, 0, (LPARAM)&wp );

	return res;
}

void CGridListCtrl::SearchFirstPrefix(CEdit* pEdit, int iType)
{
}

void CGridListCtrl::CreateSortIcons()
{
	if (!m_ImageListSortIcons.m_hImageList)
	{
		m_ImageListSortIcons.Create(9, 5, ILC_COLOR24 | ILC_MASK, 2, 0);
		m_bmpUpArrow.LoadBitmap(IDB_GRIDCTRL_SORTUP);
		m_ImageListSortIcons.Add(&m_bmpUpArrow, RGB(0, 0, 0));
		m_bmpDownArrow.LoadBitmap(IDB_GRIDCTRL_SORTDOWN);
		m_ImageListSortIcons.Add(&m_bmpDownArrow, RGB(0, 0, 0));
	}
}

// ------------------------------------------------------------------------------------------------

void CGridListCtrl::SetSortIcon()
{
	//ASSERT(m_pListCtrl);
	CHeaderCtrl* pHeaderCtrl = GetHeaderCtrl();
	ASSERT(pHeaderCtrl);

	int iNumColumns = GetColumnCount();

	for (int iCol = iNumColumns - 1; iCol >= 0; --iCol)
	{
		HDITEM hdrItem;

		if (CMisc::IsXPVisualStyle())
		{
			hdrItem.mask = HDI_FORMAT;
			pHeaderCtrl->GetItem(iCol, &hdrItem);
			if (m_nSortedColumn == iCol) 
			{
				if (m_bSortAscending)
					hdrItem.fmt = hdrItem.fmt & HDF_JUSTIFYMASK | HDF_STRING | HDF_SORTUP;
				else 
					hdrItem.fmt = hdrItem.fmt & HDF_JUSTIFYMASK | HDF_STRING | HDF_SORTDOWN;
			}
			else
				hdrItem.fmt = hdrItem.fmt & HDF_JUSTIFYMASK | HDF_STRING;
		}
		else
		{
			hdrItem.mask = HDI_FORMAT | HDI_IMAGE;
			pHeaderCtrl->GetItem(iCol, &hdrItem);
			if (m_nSortedColumn == iCol)
			{
				if (m_bSortAscending)
				{
					hdrItem.iImage = IMAGE_INDEX_UP_ARROW;
					hdrItem.fmt    = hdrItem.fmt & HDF_JUSTIFYMASK | HDF_IMAGE | HDF_STRING | HDF_BITMAP_ON_RIGHT;
				}
				else
				{
					hdrItem.iImage = IMAGE_INDEX_DOWN_ARROW;
					hdrItem.fmt    = hdrItem.fmt & HDF_JUSTIFYMASK | HDF_IMAGE | HDF_STRING | HDF_BITMAP_ON_RIGHT;
				}
			}
			else
				hdrItem.fmt = hdrItem.fmt & HDF_JUSTIFYMASK | HDF_STRING;
		}

		pHeaderCtrl->SetItem(iCol, &hdrItem);
	}
}

BOOL CGridListCtrl::OnEraseBkgnd(CDC* pDC)
{
/*	if (m_pBackgroundPicture)
	{
		Invalidate(FALSE);			// Wegen des Watermark-Bitmaps müssen wir immer alles neu zeichnen
		CRect rect;
		GetClientRect(&rect);
//		m_pBackgroundPicture->DrawPart(pDC, CPoint(0, 0), 
//			CRect(m_ptBackgroundPictureOffset, rect.Size()));
		return TRUE;
	}*/

	return CListCtrl::OnEraseBkgnd(pDC);
}

//void CGridListCtrl::SetTransparency(CPicture* pPicture, const CPoint& ptOffset)
//{
//	m_pBackgroundPicture = pPicture;
//	m_ptBackgroundPictureOffset = ptOffset;
////	m_bmpBackgroundPicture = pPicture->GetBitmap();
////	SetBkImage(m_bmpBackgroundPicture);
//}

// Jede zweite Zeile leicht einfärben
void CGridListCtrl::EnableDoubleColoring(BOOL bEnable)
{
	m_bEnableDoubleColoring = bEnable;
}

long CGridListCtrl::GetCurrencyValue(const CString& sValue)
{
	long lValue;
	CString sText = sValue;

	// Alle Nicht-Zahlen entfernen (bis auf das Komma)
	for (int i=0;i<sText.GetLength();)
	{
		if (!isdigit((unsigned char)sText[i]) && sText[i] != ',')
			sText.Delete(i);
		else
			i++;
	}
	
	int iKomma = sText.Find(L",");

	CString sEuro;

	sEuro = sText.Left(7);

	lValue = _wtoi(sEuro) * 100;
	if (iKomma >= 0)
	{
		if (sText.Mid(iKomma+1).GetLength() == 1)
			lValue += _wtoi(sText.Mid(iKomma+1))*10;
		else
			lValue += _wtoi(sText.Mid(iKomma+1));
	}

	return lValue;
}

/////////////////////////////////////////////////////////////////////////////
// CGridListEditCtrl
// Der Kram hier wird zum "Subclassen" des Edit-Felds gebraucht

CGridListEditCtrl::CGridListEditCtrl()
{
	m_iFormat = UDFF_TEXT;          // Text ist standard
}

CGridListEditCtrl::~CGridListEditCtrl()
{
}


BEGIN_MESSAGE_MAP(CGridListEditCtrl, CEdit)
	//{{AFX_MSG_MAP(CGridListEditCtrl)
	ON_WM_KEYDOWN()
	ON_WM_GETDLGCODE()
	ON_CONTROL_REFLECT(EN_KILLFOCUS, OnKillfocus)
	ON_WM_CHAR()
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CGridListEditCtrl message handlers

void CGridListEditCtrl::OnKeyDown(UINT nChar, UINT nRepCnt, UINT nFlags) 
{
   SHORT nKeyState;
   int item, col;

   nKeyState = GetKeyState(VK_SHIFT); 

   if (nChar == VK_TAB && nKeyState < 0)    // Shift+TAB
   {
      item = m_GridListCtrl->m_EditItem;
      col = m_GridListCtrl->m_EditSubItem-1;
      while (!(m_GridListCtrl->m_Column[col].dwFlags & GLCCF_EDIT))
      {
         col--;
         if (col < 0)
         {
            item--;
            if (item < 0)
               break;
            col = m_GridListCtrl->GetColumnCount();
         }
      }

      if (item < 0)
         return;
         
      m_GridListCtrl->CloseEditWindow();

      m_GridListCtrl->EnsureVisible(item, FALSE);

      m_GridListCtrl->EditSubItemLabel(item, col);

      return;
   }

   if (nChar == VK_UP)
   {
      if (m_GridListCtrl->m_EditItem > 0)
      {
         int saveSubItem = m_GridListCtrl->m_EditSubItem;
         m_GridListCtrl->CloseEditWindow();

         m_GridListCtrl->EnsureVisible(m_GridListCtrl->m_EditItem-1, FALSE);

         m_GridListCtrl->EditSubItemLabel(--m_GridListCtrl->m_EditItem, saveSubItem);
      }

      return;
   }

   if (nChar == VK_DOWN)
   {
      if (m_GridListCtrl->m_EditItem < m_GridListCtrl->GetItemCount()-1)
      {
         int saveSubItem = m_GridListCtrl->m_EditSubItem;
         m_GridListCtrl->CloseEditWindow();

         m_GridListCtrl->EnsureVisible(m_GridListCtrl->m_EditItem+1, FALSE);

         m_GridListCtrl->EditSubItemLabel(++m_GridListCtrl->m_EditItem, saveSubItem);
      }

      return;
   }

   if (nChar == VK_TAB && nKeyState >= 0)      // TAB
   {
      item = m_GridListCtrl->m_EditItem;
      col = m_GridListCtrl->m_EditSubItem+1;
      while (!(m_GridListCtrl->m_Column[col].dwFlags & GLCCF_EDIT))
      {
            col++;
            if (col >= m_GridListCtrl->GetColumnCount())
            {
               item++;
               if (item >= m_GridListCtrl->GetItemCount())
                  break;
               col = 0;
            }
      }

      if (item >= m_GridListCtrl->GetItemCount())
         return;
         
      m_GridListCtrl->CloseEditWindow();

      m_GridListCtrl->EnsureVisible(item, FALSE);

      m_GridListCtrl->EditSubItemLabel(item, col);

      return;
   }

   if (nChar == VK_RETURN)
   {
      m_GridListCtrl->CloseEditWindow();
      return;
   }

   if (nChar == VK_ESCAPE)
   {
      Undo();
      m_GridListCtrl->CloseEditWindow();
      return;
   }

	CEdit::OnKeyDown(nChar, nRepCnt, nFlags);
}

UINT CGridListEditCtrl::OnGetDlgCode() 
{
   // Damit auch TAB, RETURN, etc. im Control ankommen
	return (CEdit::OnGetDlgCode() | DLGC_WANTALLKEYS);
}

void CGridListEditCtrl::OnKillfocus() 
{
//   m_GridListCtrl->CloseEditWindow();
// ACHTUNG: Windows 95 Fehler: Muß im PARENT-Window gemacht werden!
}

void CGridListEditCtrl::OnChar(UINT nChar, UINT nRepCnt, UINT nFlags) 
{
	CEdit::OnChar(nChar, nRepCnt, nFlags);

    if (m_GridListCtrl->m_EditSubItem >= 0 && (m_GridListCtrl->m_Column[m_GridListCtrl->m_EditSubItem].dwFlags & GLCCF_ARTIST|GLCCF_TRACKNAME))
	{
		if (nChar >= 32 && nChar <= 255)
		{
			if (m_GridListCtrl->m_Column[m_GridListCtrl->m_EditSubItem].dwFlags & GLCCF_ARTIST)
				m_GridListCtrl->SearchFirstPrefix(this, 1);
			if (m_GridListCtrl->m_Column[m_GridListCtrl->m_EditSubItem].dwFlags & GLCCF_TRACKNAME)
				m_GridListCtrl->SearchFirstPrefix(this, 3);
		}
	}
}

BOOL CGridListEditCtrl::PreTranslateMessage(MSG* pMsg) 
{
   int col;

   col = m_GridListCtrl->m_EditSubItem;
   if (m_GridListCtrl->m_Column[col].dwFlags & GLCCF_CODES)
   {
      if (pMsg->message == WM_KEYDOWN)
      {
         CString str;

         GetWindowText(str);

         if (str.Find(pMsg->wParam) >= 0)
            return TRUE;
      }
      if (pMsg->message == WM_CHAR)
         if (!isalpha(pMsg->wParam) && pMsg->wParam > 31)
            return TRUE;
   }
   
	return CEdit::PreTranslateMessage(pMsg);
}

/////////////////////////////////////////////////////////////////////////////
// CGridListComboBox

CGridListComboBox::CGridListComboBox()
{
}

CGridListComboBox::~CGridListComboBox()
{
}


BEGIN_MESSAGE_MAP(CGridListComboBox, CComboBox)
	//{{AFX_MSG_MAP(CGridListComboBox)
	ON_WM_CREATE()
	//}}AFX_MSG_MAP
	ON_CONTROL_REFLECT(CBN_SELCHANGE, OnCbnSelchange)
	ON_CONTROL_REFLECT(CBN_KILLFOCUS, OnCbnKillfocus)
	ON_WM_KEYDOWN()
	ON_WM_GETDLGCODE()
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CGridListComboBox message handlers

int CGridListComboBox::OnCreate(LPCREATESTRUCT lpCreateStruct) 
{
	if (CComboBox::OnCreate(lpCreateStruct) == -1)
		return -1;
	
	CFont* font = GetParent()->GetFont();
	SetFont(font);
	
	return 0;
}


void CGridListEditCtrl::SetFormat(int iFormat)
{
	m_iFormat = iFormat;
}

void CGridListComboBox::OnCbnSelchange()
{
	CString sText;
	int sel = GetCurSel();

	if (sel >= 0)
	{
		GetLBText(sel, sText);
		m_pGridListCtrl->SetItemText(m_iRow, m_iColumn, sText);
		m_pGridListCtrl->SetCellValue(m_iRow, m_iColumn, sel);
	}

	::SendMessage(GetParent()->GetParent()->m_hWnd, GLCM_SELCHANGED, GetDlgCtrlID(), MAKELPARAM(m_iColumn, m_iRow));
	// TODO: Add your control notification handler code here
}


void CGridListComboBox::OnCbnKillfocus()
{
//	m_pGridListCtrl->CloseEditWindow();
}
void CGridListComboBox::OnKeyDown(UINT nChar, UINT nRepCnt, UINT nFlags)
{
	SHORT nKeyState;
	int item, col;

	nKeyState = GetKeyState(VK_SHIFT); 

	if (nChar == VK_TAB && nKeyState < 0)    // Shift+TAB
	{
		item = m_pGridListCtrl->m_EditItem;
		col = m_pGridListCtrl->m_EditSubItem-1;
		while (!(m_pGridListCtrl->m_Column[col].dwFlags & GLCCF_EDIT))
		{
			col--;
			if (col < 0)
			{
				item--;
				if (item < 0)
					break;
				col = m_pGridListCtrl->GetColumnCount();
			}
		}

		if (item < 0)
			return;

		m_pGridListCtrl->CloseEditWindow();

		m_pGridListCtrl->EnsureVisible(item, FALSE);

		m_pGridListCtrl->EditSubItemLabel(item, col);

		return;
	}

	if (nChar == VK_TAB && nKeyState >= 0)      // TAB
	{
		item = m_pGridListCtrl->m_EditItem;
		col = m_pGridListCtrl->m_EditSubItem+1;
		while (!(m_pGridListCtrl->m_Column[col].dwFlags & GLCCF_EDIT))
		{
			col++;
			if (col >= m_pGridListCtrl->GetColumnCount())
			{
				item++;
				if (item >= m_pGridListCtrl->GetItemCount())
					break;
				col = 0;
			}
		}

		if (item >= m_pGridListCtrl->GetItemCount())
			return;

		m_pGridListCtrl->CloseEditWindow();

		m_pGridListCtrl->EnsureVisible(item, FALSE);

		m_pGridListCtrl->EditSubItemLabel(item, col);

		return;
	}

	if (nChar == VK_RETURN || nChar == VK_ESCAPE)
	{
		m_pGridListCtrl->CloseEditWindow();
		return;
	}

	CComboBox::OnKeyDown(nChar, nRepCnt, nFlags);
}

UINT CGridListComboBox::OnGetDlgCode()
{
   // Damit auch TAB, RETURN, etc. im Control ankommen
	return (CComboBox::OnGetDlgCode() | DLGC_WANTALLKEYS);
}


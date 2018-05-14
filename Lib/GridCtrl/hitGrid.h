// hitGrid.h : header file
//

#pragma once

#ifdef _GRIDCTRL_DLL
#define GRIDCTRL_INTERFACE __declspec ( dllexport )
#else
#define GRIDCTRL_INTERFACE __declspec ( dllimport )
#endif

/////////////////////////////////////////////////////////////////////////////
// CGridListEditCtrl window

class CGridListCtrl;

/////////////////////////////////////////////////////////////////////////////
// CGridListComboBox window

class CGridListComboBox : public CComboBox
{
// Construction
public:
	CGridListComboBox();

// Attributes
public:

// Operations
public:

// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CGridListComboBox)
	//}}AFX_VIRTUAL

// Implementation
public:
	virtual ~CGridListComboBox();

	// Generated message map functions
protected:
	//{{AFX_MSG(CGridListComboBox)
	afx_msg int OnCreate(LPCREATESTRUCT lpCreateStruct);
	//}}AFX_MSG

	DECLARE_MESSAGE_MAP()
public:
	afx_msg void OnCbnSelchange();

	int m_iRow;
	int m_iColumn;
    CGridListCtrl* m_pGridListCtrl;
	afx_msg void OnCbnKillfocus();
	afx_msg void OnKeyDown(UINT nChar, UINT nRepCnt, UINT nFlags);
	afx_msg UINT OnGetDlgCode();
};

/////////////////////////////////////////////////////////////////////////////

class CField
{
public:
	CField() 
	{
		m_nRow = 0;
		m_nColumn = 0;
		m_nType = 0;
		m_dwcbFlags = 0;
		m_nValue = 0;
		m_iFormat = 0;
		m_dwFlags = 0;
	}

	int m_nRow;
	int m_nColumn;
	int m_nType;
	CStringArray m_cbItems;
	int m_nValue;
	DWORD m_dwcbFlags;
	int m_iFormat;
	DWORD m_dwFlags;
};

class CFieldArray : public CArray <CField, CField>
{
};

/////////////////////////////////////////////////////////////////////////////

class CGridListEditCtrl : public CEdit
{
// Construction
public:
	CGridListEditCtrl();

// Attributes
public:

// Operations
public:

// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CGridListEditCtrl)
	public:
	virtual BOOL PreTranslateMessage(MSG* pMsg);
	//}}AFX_VIRTUAL

// Implementation
public:
	virtual ~CGridListEditCtrl();

	// Generated message map functions
protected:
	int m_iFormat;
	//{{AFX_MSG(CGridListEditCtrl)
	afx_msg void OnKeyDown(UINT nChar, UINT nRepCnt, UINT nFlags);
	afx_msg UINT OnGetDlgCode();
	afx_msg void OnKillfocus();
	afx_msg void OnChar(UINT nChar, UINT nRepCnt, UINT nFlags);
	//}}AFX_MSG

	DECLARE_MESSAGE_MAP()
public:
	virtual void SetFormat(int iFormat);
    CGridListCtrl *m_GridListCtrl;
};

/////////////////////////////////////////////////////////////////////////////
// CGridListCtrl window

#define _MAX_COLUMNS   100

// GridListCtrl Flags
#define GLCF_GRID        1      // Gitternetzlinien aktivieren
#define GLCF_SIMPLE      2      // Bereits nach einem Click Feld editieren
#define GLCF_ENABLEKEYS  4      // Erlaubt das löschen von Zeilen mit der DEL Taste

// Column Flags
#define GLCCF_EDIT	 	 1      // Erlaubt das Ändern von Werten in der Spalte
#define GLCCF_ARTIST     2      // Ist ein Artisten-Feld (also in DB Eingabe vorgeben)
#define GLCCF_CODES      4      // Ist ein Kennzeichen-Feld (nur eindeutige Großbuchstaben)
#define GLCCF_GRAYFIELDS 8      // Die Spalte ist read-only und der Hintergrund wird grau dargestellt!
#define GLCCF_TRACKNAME  16     // Ist ein Lied-Feld (also in DB Eingabe vorgeben)

// Cell types
#define GLCCT_STRING    0       // String (text feld)
#define GLCCT_COMBOBOX  1       // ComboBox Control
#define GLCCT_COLOR     2       // Farb-Feld
#define GLCCT_BITMAP    3       // Bitmap-Feld

// Cell flags
#define GLCCELLFLAGS_CROSS       1  // Zelle durchstreichen
#define GLCCELLFLAGS_GROUPHEADER_EMPTY 2  // Platzhalter für neue Gruppe
#define GLCCELLFLAGS_GROUPHEADER_TEXT  4  // Platzhalter für neue Gruppe
#define GLCCELLFLAGS_GROUPHEADER_LINE  8  // Platzhalter für neue Gruppe

// User-defined messages
#define GLCM_CLICKEDSYMBOL  WM_USER+1       // Ein Symbol wurde angeklickt
#define GLCM_SELCHANGED     WM_USER+2       // Combo-Box Selchanged

// Sortierparameter
enum ListCompareType 
{  
	ELCT_INTEGER = 0,
	ELCT_DOUBLE,
	ELCT_STRING_CASE,
	ELCT_STRING_NOCASE,
	ELCT_DATE_TIME
};

enum ListOperatorType 
{ 
	ELOT_LT = 0,    //  <   less than
	ELOT_GT,  //  >   greater than
	ELOT_LTE, //  <=  less than or equal
	ELOT_GTE, //  >=  greather than or equal
	ELOT_EQ   //  ==  equal
};

class GRIDCTRL_INTERFACE CGridListCtrl : public CListCtrl
{
	DECLARE_DYNAMIC(CGridListCtrl)

	friend CGridListEditCtrl;

	static const int IMAGE_INDEX_UP_ARROW = 0;
	static const int IMAGE_INDEX_DOWN_ARROW = 1;
	
// Construction
public:
	void SetActiveItemColor(COLORREF rgbTextColor, COLORREF rgbBkColor);
	BOOL HasUserColor(int nItem, COLORREF* cr = NULL);
	int *m_ItemColor;
    int m_nItemColorLast;            // Höchste Zeile mit einer Farbe
	BOOL SetTextColor(int nItem, COLORREF cr, BOOL bUpdate = TRUE);
	BOOL GetItemColor(int nItem);
	void LoadColumnWidth(const CString& KeyName);
	void SaveColumnWidth(const CString& KeyName);
	void SwapLines(int nFirst, int nSecond = -1);
	virtual void DeleteSelectedItems();
	virtual BOOL OnItemDelete(int iItem);
	int GetCurSel();
	void DeleteAllColumns();
	COLORREF m_BkColor;
	BOOL SetComboBoxItems(CStringArray* cbItems, int nRow, int nColumn, DWORD dwcbFlags = CBS_DROPDOWNLIST);
	void SetFlags(DWORD dwFlags);
	CGridListCtrl(DWORD dwFlags = GLCF_GRID);

// Attributes
public:
    void SetColumnEditWidth(int col, int MaxLength);
	int GetColumnCount(void);
    void SetActiveItem(int item);
	void SetColumnFlags(int col, DWORD dwFlags);
	void SetCellFormat(int iRow, int iCol, int iFormat);
	DWORD GetCellFlags(int nRow, int nColumn);
	void SetCellFlags(int nRow, int nColumn, DWORD dwFlags);

// Operations
public:
    BOOL DeleteColumn(int nCol);

// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CGridListCtrl)
	public:
	//}}AFX_VIRTUAL

// Implementation
public:
	virtual ~CGridListCtrl();

	// Generated message map functions
protected:
	//{{AFX_MSG(CGridListCtrl)
	afx_msg void OnLButtonDown(UINT nFlags, CPoint point);
	afx_msg void OnHScroll(UINT nSBCode, UINT nPos, CScrollBar* pScrollBar);
	afx_msg void OnVScroll(UINT nSBCode, UINT nPos, CScrollBar* pScrollBar);
	afx_msg void OnDeleteallitems(NMHDR* pNMHDR, LRESULT* pResult);
	afx_msg void OnKeyDown(UINT nChar, UINT nRepCnt, UINT nFlags);
	afx_msg void OnChangeColor();
	afx_msg BOOL OnSetCursor(CWnd* pWnd, UINT nHitTest, UINT message);
	//}}AFX_MSG

	afx_msg virtual void OnClick(NMHDR* pNMHDR, LRESULT* pResult);
	DECLARE_MESSAGE_MAP()

// Private members
public:
	int m_EditSubItem;
	int m_EditItem;
	void CloseEditWindow();
	BOOL m_EditActive;
	CGridListEditCtrl m_EditSubItemLabel;
	BOOL EditSubItemLabel(int nItem, int nSubItem);
	struct tagColumn {
		DWORD dwFlags;
		int nEditWidth;      // LIMITTEXT für die einzelnen Spalten
	} m_Column[_MAX_COLUMNS];
	int m_SelectedItem;
	CFont m_BoldFont;
	int m_ActiveItem;
    int m_dwFlags;           // Siehe #defines GLCF_*

// Overridables
public:
	virtual void MoveLines(int iIndex, int iOffset);
	BOOL AddToolTip(int nCol, const CString& sText);
	int GetCellValue(int nRow, int nCol);
	int SetCellValue(int nRow, int nCol, int nValue);
	int GetCellType(int nRow, int nColumn);
	void SetCellType(int nRow, int nColumn, int nType);
	void SetNumberOfItems(const int nItemCount);
    void DrawItem(LPDRAWITEMSTRUCT lpDrawItemStruct);
	void Sort(int nColumn, BOOL bAscending, ListCompareType nCompareAs);
	void SetSortColumn(int iColumn, BOOL bAscending);
	virtual void SearchFirstPrefix(CEdit* pEdit, int iType);

protected:
	CToolTipCtrl m_ttc;
	BOOL m_ComboBoxActive;
	BOOL CreateComboBox(int nRow, int nColumn, CRect* pRect, CString label);
	CGridListComboBox m_ComboBox;

	CButton m_EditColorCtrl;

	CArray <CFieldArray, CFieldArray> m_Cell;

	COLORREF m_rgbActiveItemTextColor;
	COLORREF m_rgbActiveItemBkColor;
	bool m_bActiveItemColorSet;

	// Für Sortierung
	void QuickSort(int p, int r);
	int Partition(int p, int r);
	BOOL CompareBy(CString str1, CString str2, ListOperatorType op);
	BOOL SwapRow(int nRow1, int nRow2);
	COLORREF LightenColor( long lScale, COLORREF lColor);
	COLORREF DarkenColor( long lScale, COLORREF lColor);
	LRESULT OnSetFont(WPARAM wParam, LPARAM);
	void CreateSortIcons();
	void SetSortIcon();
	CImageList m_ImageListSortIcons;
	CBitmap m_bmpUpArrow;
	CBitmap m_bmpDownArrow;

	CPoint m_ptBackgroundPictureOffset;
	HBITMAP m_bmpBackgroundPicture;

	int m_nSortedColumn;
	BOOL m_bSortAscending;
	ListCompareType m_nCompareAs;
	int m_nSortCacheMaxRows;
	int m_nSortCacheMaxColumns;

	BOOL m_bFontCreated;

	BOOL m_bEnableDoubleColoring;

	class CGridListSortItem
	{
	public:
		CGridListSortItem(const DWORD dwParam, const CString &sText, COLORREF dwItemColor)
		{
			m_dwParam = dwParam;
			m_sText = sText;
			m_dwItemColor = dwItemColor;
		}
		
		DWORD m_dwParam; 
		CString m_sText;
		COLORREF m_dwItemColor;
	};
	static int CALLBACK CompareItems(LPARAM lParam1, LPARAM lParam2, LPARAM lParamSort);

	long GetCurrencyValue(const CString& sValue);

public:
	afx_msg void OnNMSetfocus(NMHDR *pNMHDR, LRESULT *pResult);
	afx_msg void OnNMKillfocus(NMHDR *pNMHDR, LRESULT *pResult);
	afx_msg void MeasureItem(LPMEASUREITEMSTRUCT lpMeasureItemStruct);
	void SetCurSel(int iCurSel);
	afx_msg BOOL OnEraseBkgnd(CDC* pDC);
	void EnableDoubleColoring(BOOL bEnable);
};


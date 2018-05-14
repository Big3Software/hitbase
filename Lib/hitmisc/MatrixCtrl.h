// CMatrixCtrl.h : header file
//

#pragma once

/////////////////////////////////////////////////////////////////////////////
// CMatrixCtrl window

class HITMISC_INTERFACE CMatrixCtrl : public CWnd
{
	struct CMatrixItem
	{
		int x;
		int y;
		int clrGreen;
		int character;
	};

	struct CMatrixColumn
	{
		CMatrixItem *pItem;
		int x;
		int y;
		int nLength;
		BOOL bActive;
		BOOL bOffScreen;
		int nCounter;
	};

	int m_nRows;
	int m_nCols;
	int m_nTextHeight;
	int m_nTextWidth;

	CMatrixColumn m_MatrixColumns[150];
	
	int m_nTotalColumns;
	int m_bAlternate;
	
	void DoStuff();
	void UpdateCharacters(int nColumn);

// Construction
public:
  CMatrixCtrl();

// Attributes
public:
  void SetTextColor(COLORREF color);
  void SetBackgroundColor(COLORREF color);
  void InvalidateCtrl();

  // Operations
public:

// Overrides
  // ClassWizard generated virtual function overrides
  //{{AFX_VIRTUAL(CMatrixCtrl)
  public:
  virtual BOOL Create(DWORD dwStyle, const RECT& rect, CWnd* pParentWnd, UINT nID=NULL);
  //}}AFX_VIRTUAL

// Implementation
public:
	void SetCredits(LPCTSTR lpszCredits);
	CString m_strCurrentLabel;
	void DoBackground(CDC *pDC);
	COLORREF m_crBackColor;        // background color
	COLORREF m_crTextColor;        // text color
  
	virtual ~CMatrixCtrl();

	// Generated message map functions
protected:
	BOOL m_bFinale;
	void CalculateNextLabelPos();
	int m_nStartOfNextLabel;
	double m_fFactor;
	CBitmap m_bmLogo;
	int m_nBkCount;
	CBitmap *m_pOldBitmap;
	CBitmap m_Bitmap;
	CDC m_BackgroundDC;
	void Initialize();
	int m_nCount;
	CString m_strCredits;
	CString m_strLabel1;
	CString m_strLabel2;
	CString m_strLabel3;
	int m_yLabel1;
	int m_yLabel2;
	//{{AFX_MSG(CMatrixCtrl)
	afx_msg void OnPaint();
	afx_msg void OnSize(UINT nType, int cx, int cy); 
	afx_msg void OnTimer(UINT nIDEvent);
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()

	CFont m_Font;
	int m_nClientHeight;
	int m_nClientWidth;

	CRect  m_rectClient;
	CBrush m_brushBack;

	CDC     m_dcMatrix;
	CBitmap *m_pbitmapOldMatrix;
	CBitmap m_bitmapMatrix;
};


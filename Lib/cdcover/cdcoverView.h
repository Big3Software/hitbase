// cdcoverView.h : interface of the CCdcoverView class
//
/////////////////////////////////////////////////////////////////////////////

#if !defined(AFX_CDCOVERVIEW_H__910F52EB_FE6C_11D3_A4D5_0080AD834EB5__INCLUDED_)
#define AFX_CDCOVERVIEW_H__910F52EB_FE6C_11D3_A4D5_0080AD834EB5__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "coverlayout.h"

class CDCOVER_INTERFACE CCdcoverView : public CScrollView
{

	private:

		BOOL m_bResizeD1;
		BOOL m_bResizeD2;
		BOOL m_bResizeD3;
		BOOL m_bResizeD4;
		BOOL m_bResizeTopCur;
		BOOL m_bResizeBottomCur;
		BOOL m_bResizeLeftCur;
		BOOL m_bResizeRightCur;
		BOOL m_bMoveCur;

		BOOL m_bResizeTB;
		BOOL m_bMoveTB;
		CPoint m_ptMouseSet;

		CPoint m_ptF;
		CPoint m_ptB;
		CPoint m_ptL;

		BOOL m_bRightButtonDown;
		BOOL m_bLeftButtonDown;

		void CenterX();
		void CenterY();

		void ResortTBs(const int nNewIndex, const BOOL bForward);

		void GetMultiLines(CCoverTextBox *pTB);
		void RefreshSelector(CRect *pRect, const BOOL bClear);
		void CheckTBState(CPoint *ppoint);
		void FrameTB(CDC *pDC, CRect *prectTB, CPoint *pptOffset, COLORREF col);

		CPoint PointInVP(CPoint *ppoint, CDC *ppDC = NULL);

		CPoint VPtoWE(CPoint *ppt);
		CPoint WEtoVP(CPoint *ppt);
		CPoint PointInWE(CPoint *ppoint);
		CRect RectInVP(CRect *prect, CDC *ppDC = NULL);
		CRect RectInWE(CRect *prect);

		void SetColumnTypeFromMenuID(const int nMenuID, const CString& strColumnText = L"");
		void AddColumnFromMenuID(const int nMenuID, const CString& strColumnText = L"");
		void AddTextBoxToArea(CCoverTextBox *ptbNew);
		void UpdateActiveRect(CPoint point);
		void BuildHeader(CRect rectBack);
		void WorkOnLabel();
		void WorkOnTextBox();
		void WorkOnBorder();
		void WorkOnArtistTitle();
		void WorkOnBack();
		void WorkOnFront();
		void CreateFontDlg(LOGFONT *plf, const BOOL bShowSize = TRUE);
		void CreateColorDlg(COLORREF *pcol);
		void InitColumnWidths();
		void SetXAlignmentFromMenuID(const int nMenuID);
		void MyInvalidateRect(CRect *pRect = NULL, const BOOL bClear = FALSE);
		void RefreshTextBox(CRect *prectOld, CRect *prectNew);

		CPoint PtRelToTB(CPoint *ppoint);

		void UpdateZoomStatusLine();


		BOOL m_bBackRegion;

		BOOL m_bFirstDrag;

		CRect m_rectGrab;
		CRect m_rectOldGrab;
		CRect m_rectGrabClient;

		CRect m_rectSizeRect;

		CHeaderCtrl m_header;
		CRect m_rectHeader;

		int m_nCurColumn;
		CFont m_Font;

	protected: // create from serialization only

		CCdcoverView();
		DECLARE_DYNCREATE(CCdcoverView)

	public:

		CStatusBar  *m_pwndStatusBar;
		CCoverLayout *m_pLayout;
		CCoverLayout *m_pOldLayout;
		HCURSOR m_hMoveCursor;
		HCURSOR m_hNormalCursor;
		HCURSOR m_hZoomCursor;
		HCURSOR m_hResizeTopCursor;
		HCURSOR m_hResizeRightCursor;
		HCURSOR m_hResizeNWSECursor;
		HCURSOR m_hResizeNESWCursor;
		
		BOOL m_bFirstDraw;
		BOOL m_bWaitForDraw;

		BOOL m_bRecalcScroll;

	// Operations

	public:

	// Overrides
		// ClassWizard generated virtual function overrides
		//{{AFX_VIRTUAL(CCdcoverView)
	public:
			virtual void OnDraw(CDC* pDC);  // overridden to draw this view
			virtual BOOL PreCreateWindow(CREATESTRUCT& cs);
			virtual void OnInitialUpdate();
	protected:
			virtual BOOL OnPreparePrinting(CPrintInfo* pInfo);
			virtual void OnBeginPrinting(CDC* pDC, CPrintInfo* pInfo);
			virtual void OnEndPrinting(CDC* pDC, CPrintInfo* pInfo);
	virtual BOOL OnNotify(WPARAM wParam, LPARAM lParam, LRESULT* pResult);
	//}}AFX_VIRTUAL

	// Implementation

	public:

		virtual ~CCdcoverView();
		#ifdef _DEBUG
			virtual void AssertValid() const;
			virtual void Dump(CDumpContext& dc) const;
		#endif

		void OnFilePrint();

	// Generated message map functions
	protected:
		//{{AFX_MSG(CCdcoverView)
	afx_msg void OnLButtonDown(UINT nFlags, CPoint point);
	afx_msg void OnRButtonDown(UINT nFlags, CPoint point);
	afx_msg BOOL OnSetCursor(CWnd* pWnd, UINT nHitTest, UINT message);
	afx_msg void OnMouseMove(UINT nFlags, CPoint point);
	afx_msg void OnBorderpopup();
	afx_msg void OnBackpopup();
	afx_msg void OnBackMainPopup();
	afx_msg void OnFrontMainPopup();
	afx_msg void OnFrontPopup();
	afx_msg void OnLButtonUp(UINT nFlags, CPoint point);
	afx_msg void OnHScroll(UINT nSBCode, UINT nPos, CScrollBar* pScrollBar);
	afx_msg BOOL OnMouseWheel(UINT nFlags, short zDelta, CPoint pt);
	afx_msg void OnTextBoxMain();
	afx_msg void OnTBPopup();
	afx_msg void OnRButtonUp(UINT nFlags, CPoint point);
	afx_msg void OnLabelMainPopup();
	afx_msg void OnVScroll(UINT nSBCode, UINT nPos, CScrollBar* pScrollBar);
	afx_msg void OnLButtonDblClk(UINT nFlags, CPoint point);
	//}}AFX_MSG
		DECLARE_MESSAGE_MAP()

		void OnFilePrintPreview();
};
/*
#ifndef _DEBUG  // debug version in cdcoverView.cpp
inline CCdcoverDoc* CCdcoverView::GetDocument()
   { return (CCdcoverDoc*)m_pDocument; }
#endif
*/
/////////////////////////////////////////////////////////////////////////////

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_CDCOVERVIEW_H__910F52EB_FE6C_11D3_A4D5_0080AD834EB5__INCLUDED_)

#if !defined(AFX_SOUNDVISUALIZERWND_H__40E22AD4_F499_11D1_A42F_000000000000__INCLUDED_)
#define AFX_SOUNDVISUALIZERWND_H__40E22AD4_F499_11D1_A42F_000000000000__INCLUDED_

#if _MSC_VER >= 1000
#pragma once
#endif // _MSC_VER >= 1000
// SoundVisualizerWnd.h : header file
//

class CSoundVisualizer;

/////////////////////////////////////////////////////////////////////////////
// CSoundVisualizerWnd frame

class CSoundVisualizerWnd : public CMiniFrameWnd
{
	DECLARE_DYNCREATE(CSoundVisualizerWnd)
protected:
	CSoundVisualizerWnd();           // protected constructor used by dynamic creation

// Attributes
public:
	CSoundVisualizer* m_pSoundVisualizer;
	int m_nPlugInIndex;

// Operations
public:

// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CSoundVisualizerWnd)
	//}}AFX_VIRTUAL

// Implementation
protected:
	virtual ~CSoundVisualizerWnd();

	// Generated message map functions
	//{{AFX_MSG(CSoundVisualizerWnd)
	afx_msg void OnPaint();
	afx_msg void OnClose();
	afx_msg void OnSize(UINT nType, int cx, int cy);
	afx_msg void OnRButtonDown(UINT nFlags, CPoint point);
	afx_msg void OnProperties();
	afx_msg int OnCreate(LPCREATESTRUCT lpCreateStruct);
	afx_msg void OnDestroy();
	afx_msg void OnMove(int x, int y);
	afx_msg void OnKeyDown(UINT nChar, UINT nRepCnt, UINT nFlags);
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};

/////////////////////////////////////////////////////////////////////////////

//{{AFX_INSERT_LOCATION}}
// Microsoft Developer Studio will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_SOUNDVISUALIZERWND_H__40E22AD4_F499_11D1_A42F_000000000000__INCLUDED_)

#if !defined(AFX_OPTIONSCDARCHIVEFIELDSDLG_H__FCB708B3_A261_11D1_A36D_000000000000__INCLUDED_)
#define AFX_OPTIONSCDARCHIVEFIELDSDLG_H__FCB708B3_A261_11D1_A36D_000000000000__INCLUDED_

#if _MSC_VER >= 1000
#pragma once
#endif // _MSC_VER >= 1000
// OptionsCDArchiveFieldsDlg.h : header file
//

/////////////////////////////////////////////////////////////////////////////
// COptionsCDArchiveFieldsDlg dialog

class COptionsCDArchiveFieldsDlg : public CDialog
{
// Construction
public:
	COptionsCDArchiveFieldsDlg(CWnd* pParent = NULL);   // standard constructor

// Dialog Data
	//{{AFX_DATA(COptionsCDArchiveFieldsDlg)
	enum { IDD = DLG_OPTIONS_CDARCHIV_FIELDS };
	BOOL	m_bCategory;
	BOOL	m_bMedium;
	BOOL	m_bCDComment;
	BOOL	m_bBPM;
	BOOL	m_bTrackComment;
	BOOL	m_bLyrics;
	//}}AFX_DATA


// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(COptionsCDArchiveFieldsDlg)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:

	// Generated message map functions
	//{{AFX_MSG(COptionsCDArchiveFieldsDlg)
		// NOTE: the ClassWizard will add member functions here
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Developer Studio will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_OPTIONSCDARCHIVEFIELDSDLG_H__FCB708B3_A261_11D1_A36D_000000000000__INCLUDED_)

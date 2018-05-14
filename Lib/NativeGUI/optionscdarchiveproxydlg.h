#if !defined(AFX_OPTIONSCDARCHIVEPROXYDLG_H__167EE6D6_63A8_11D2_A53A_000000000000__INCLUDED_)
#define AFX_OPTIONSCDARCHIVEPROXYDLG_H__167EE6D6_63A8_11D2_A53A_000000000000__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// OptionsCDArchiveProxyDlg.h : Header-Datei
//

/////////////////////////////////////////////////////////////////////////////
// Dialogfeld COptionsCDArchiveProxyDlg

class COptionsCDArchiveProxyDlg : public CDialog
{
// Konstruktion
public:
	COptionsCDArchiveProxyDlg(CWnd* pParent = NULL);   // Standardkonstruktor

// Dialogfelddaten
	//{{AFX_DATA(COptionsCDArchiveProxyDlg)
	enum { IDD = DLG_OPTIONS_CDARCHIV_PROXY };
	CEdit	m_ProxyServerPortCtrl;
	CEdit	m_ProxyServerNameCtrl;
	CButton	m_NoProxyCtrl;
	CButton	m_ProxyCtrl;
	int		m_nProxyType;
	CString	m_sProxyServerName;
	int		m_iPort;
	//}}AFX_DATA


// Überschreibungen
	// Vom Klassen-Assistenten generierte virtuelle Funktionsüberschreibungen
	//{{AFX_VIRTUAL(COptionsCDArchiveProxyDlg)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV-Unterstützung
	//}}AFX_VIRTUAL

// Implementierung
protected:

	// Generierte Nachrichtenzuordnungsfunktionen
	//{{AFX_MSG(COptionsCDArchiveProxyDlg)
	afx_msg void OnProxy();
	afx_msg void OnNoProxy();
	virtual BOOL OnInitDialog();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
private:
	void UpdateWindowState();
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ fügt unmittelbar vor der vorhergehenden Zeile zusätzliche Deklarationen ein.

#endif // AFX_OPTIONSCDARCHIVEPROXYDLG_H__167EE6D6_63A8_11D2_A53A_000000000000__INCLUDED_

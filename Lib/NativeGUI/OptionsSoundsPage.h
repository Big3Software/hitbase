#include "afxwin.h"
#if !defined(AFX_OPTIONSSOUNDSPAGE_H__57221C34_6331_4EAB_B73E_287EBE82C452__INCLUDED_)
#define AFX_OPTIONSSOUNDSPAGE_H__57221C34_6331_4EAB_B73E_287EBE82C452__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// OptionsSoundsPage.h : Header-Datei
//

/////////////////////////////////////////////////////////////////////////////
// Dialogfeld COptionsSoundsPage 

class COptionsSoundsPage : public CPropertyPage
{
	DECLARE_DYNCREATE(COptionsSoundsPage)

// Konstruktion
public:
	COptionsSoundsPage();
	~COptionsSoundsPage();

// Dialogfelddaten
	//{{AFX_DATA(COptionsSoundsPage)
	enum { IDD = DLG_OPTIONS_SOUND };
	CComboBox	m_cboOutputDevice;
	CComboBox	m_cboID3Version;
	CString	m_sDescription;
	BOOL	m_bWarnWriteID3Tags;
	//}}AFX_DATA


// Überschreibungen
	// Der Klassen-Assistent generiert virtuelle Funktionsüberschreibungen
	//{{AFX_VIRTUAL(COptionsSoundsPage)
	public:
	virtual void OnOK();
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV-Unterstützung
	//}}AFX_VIRTUAL

// Implementierung
protected:
	// Generierte Nachrichtenzuordnungsfunktionen
	//{{AFX_MSG(COptionsSoundsPage)
	virtual BOOL OnInitDialog();
	afx_msg void OnSelchangeId3Version();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()

private:
	void ShowText();
public:
	CComboBox m_cboBufferSize;
	CComboBox m_cboOutputCard;
	CComboBox m_cboOutputCard2;
	BOOL m_bDetectTrackInfoFromFilename;
	CEdit m_edtDetectTrackInfoDelimiter;
	CString m_sDetectTrackInfoDelimiter;
	void FillSoundDevicesInComboBox(CComboBox& comboBox);
	afx_msg void OnBnClickedDetectTrackinfoFromFilename();
	void UpdateWindowState(void);
	BOOL m_bDeleteLeadingNumbers;
	CButton m_chkDeleteLeadingNumbers;
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ fügt unmittelbar vor der vorhergehenden Zeile zusätzliche Deklarationen ein.

#endif // AFX_OPTIONSSOUNDSPAGE_H__57221C34_6331_4EAB_B73E_287EBE82C452__INCLUDED_

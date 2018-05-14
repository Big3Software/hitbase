#pragma once

#include "coverlayout.h"

// CDlgCoverSizes dialog

class CDlgCoverSizes : public CDialog
{
	DECLARE_DYNAMIC(CDlgCoverSizes)

public:
	CDlgCoverSizes(CCoverLayout *pLayout, CWnd* pParent = NULL);   // standard constructor
	virtual ~CDlgCoverSizes();

// Dialog Data
	enum { IDD = IDD_DIALOG_COVERSIZES };

private:
	CCoverLayout *m_pLayout;
	void SetValues(CCoverLayout *pLayout);
	void ValuesToLayout();



protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support

	DECLARE_MESSAGE_MAP()
public:
	DWORD m_dwFrontWidth;
	DWORD m_dwFrontHeight;
	DWORD m_dwBackWidth;
	DWORD m_dwBackHeight;
	DWORD m_dwLabelWidth;
	DWORD m_dwLabelHeight;
	DWORD m_dwLabelRadius;
	DWORD m_dwBorderWidth;
	virtual BOOL OnInitDialog();
	afx_msg void OnBnClickedOk();
	afx_msg void OnBnClickedButtonSizeStandard();
};

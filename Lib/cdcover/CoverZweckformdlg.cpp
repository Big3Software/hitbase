// CoverZweckformdlg.cpp : implementation file
//

#include "stdafx.h"
#include "cdcover.h"
#include "CoverZweckformdlg.h"
#include "cover.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CCoverZweckformDlg dialog


CCoverZweckformDlg::CCoverZweckformDlg(CCoverLayout *pLayout, CWnd* pParent /*=NULL*/)
	: CDialog(CCoverZweckformDlg::IDD, pParent)
{
	//{{AFX_DATA_INIT(CCoverZweckformDlg)
		// NOTE: the ClassWizard will add member initialization here
	//}}AFX_DATA_INIT

	m_pLayout = pLayout;
}


void CCoverZweckformDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CCoverZweckformDlg)
	DDX_Control(pDX, IDC_EDIT_COVERFORM_OBEN, m_editRandOben);
	DDX_Control(pDX, IDC_EDIT_COVERFORM_LINKS, m_editRandLinks);
	DDX_Control(pDX, IDC_COMBO_COVERFORM_NUMMER, m_comboNummer);
	DDX_Control(pDX, IDC_COMBO_COVERFORM_MARKE, m_comboMarke);
	DDX_Control(pDX, IDC_EDIT_COVERFORM_LABELY, m_editLabelY);
	DDX_Control(pDX, IDC_EDIT_COVERFORM_LABELX, m_editLabelX);
	DDX_Control(pDX, IDC_EDIT_COVERFORM_FRONTY, m_editFrontY);
	DDX_Control(pDX, IDC_EDIT_COVERFORM_FRONTX, m_editFrontX);
	DDX_Control(pDX, IDC_EDIT_COVERFORM_BACKY, m_editBackY);
	DDX_Control(pDX, IDC_EDIT_COVERFORM_BACKX, m_editBackX);
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(CCoverZweckformDlg, CDialog)
	//{{AFX_MSG_MAP(CCoverZweckformDlg)
	ON_CBN_SELCHANGE(IDC_COMBO_COVERFORM_MARKE, OnSelchangeComboMarke)
	ON_CBN_SELCHANGE(IDC_COMBO_COVERFORM_NUMMER, OnSelchangeComboNummer)
	ON_EN_KILLFOCUS(IDC_EDIT_COVERFORM_BACKX, OnKillfocusEdit)
	ON_EN_KILLFOCUS(IDC_EDIT_COVERFORM_BACKY, OnKillfocusEdit)
	ON_EN_KILLFOCUS(IDC_EDIT_COVERFORM_FRONTX, OnKillfocusEdit)
	ON_EN_KILLFOCUS(IDC_EDIT_COVERFORM_FRONTY, OnKillfocusEdit)
	ON_EN_KILLFOCUS(IDC_EDIT_COVERFORM_LABELX, OnKillfocusEdit)
	ON_EN_KILLFOCUS(IDC_EDIT_COVERFORM_LABELY, OnKillfocusEdit)
	ON_EN_KILLFOCUS(IDC_EDIT_COVERFORM_LINKS, OnKillfocusEdit)
	ON_EN_KILLFOCUS(IDC_EDIT_COVERFORM_OBEN, OnKillfocusEdit)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CCoverZweckformDlg message handlers

BOOL CCoverZweckformDlg::OnInitDialog() 
{
	CDialog::OnInitDialog();

	UpdateData(TRUE);

	m_nBackX = m_pLayout->m_nBackX;
	m_nBackY = m_pLayout->m_nBackY;
	m_nFrontX = m_pLayout->m_nFrontX;
	m_nFrontY = m_pLayout->m_nFrontY;
	m_nLabelX = m_pLayout->m_nLabelX;
	m_nLabelY = m_pLayout->m_nLabelY;

	if (m_pLayout->m_nZweckformFirmaIndex >= (int)m_pLayout->m_vecZweckformen.size() || m_pLayout->m_nZweckformFirmaIndex < 0)
	{		
		m_pLayout->m_bZweckformUser = TRUE;
		m_pLayout->m_nZweckformIndex = 0;
		m_pLayout->m_nZweckformFirmaIndex = 0;
	}
	if (m_pLayout->m_nZweckformIndex >= (int)m_pLayout->m_vecZweckformen[m_pLayout->m_nZweckformFirmaIndex].m_vecFormate.size())
	{
		m_pLayout->m_nZweckformIndex = 0;
	}

	m_comboNummer.ResetContent();
	if (! m_pLayout->m_bZweckformUser)
	{
		for (int i = 0; i != (int)m_pLayout->m_vecZweckformen[m_pLayout->m_nZweckformFirmaIndex].m_vecFormate.size(); i++)
		{
			m_comboNummer.AddString(m_pLayout->m_vecZweckformen[m_pLayout->m_nZweckformFirmaIndex].m_vecFormate[i].m_strNummer);
		}

		m_comboNummer.SetCurSel(m_pLayout->m_nZweckformIndex);
	}

	CString strUserDef;
	strUserDef.LoadString(IDS_ZWECKFORM_USERDEF);
	m_comboMarke.ResetContent();
	m_comboMarke.AddString(strUserDef);
	for (int i = 0; i != (int)m_pLayout->m_vecZweckformen.size(); i++)
	{
		m_comboMarke.AddString(m_pLayout->m_vecZweckformen[i].m_strFirma);
	}

	m_comboMarke.SetCurSel(m_pLayout->m_bZweckformUser ? 0 : m_pLayout->m_nZweckformFirmaIndex + 1);

	SetValues();

	InitEditBoxes();

	UpdateData(FALSE);

	return TRUE;  // return TRUE unless you set the focus to a control
	              // EXCEPTION: OCX Property Pages should return FALSE
}

void CCoverZweckformDlg::OnOK() 
{
	UpdateData(TRUE);

	m_pLayout->m_nZweckformFirmaIndex = m_comboMarke.GetCurSel() - 1;
	m_pLayout->m_bZweckformUser = (m_pLayout->m_nZweckformFirmaIndex == -1);
	int nindex = m_comboNummer.GetCurSel();
	m_pLayout->m_nZweckformIndex = nindex == -1 ? 0 : nindex;

	ValidateValues();

	m_pLayout->m_nBackX = m_nBackX;
	m_pLayout->m_nBackY = m_nBackY;
	m_pLayout->m_nFrontX = m_nFrontX;
	m_pLayout->m_nFrontY = m_nFrontY;
	m_pLayout->m_nLabelX = m_nLabelX;
	m_pLayout->m_nLabelY = m_nLabelY;

	CDialog::OnOK();

	return;
}


void CCoverZweckformDlg::InitEditBoxes()
{
	UpdateData(TRUE);

	if (! m_pLayout->m_bZweckformUser)
	{
		m_comboNummer.EnableWindow(TRUE);
		m_comboNummer.SetCurSel(m_pLayout->m_nZweckformIndex);
		m_editLabelX.EnableWindow(FALSE);
		m_editLabelY.EnableWindow(FALSE);
		m_editFrontX.EnableWindow(FALSE);
		m_editFrontY.EnableWindow(FALSE);
		m_editBackX.EnableWindow(FALSE);
		m_editBackY.EnableWindow(FALSE);
	}
	else
	{
		m_comboNummer.SetCurSel(-1);
		m_comboNummer.EnableWindow(FALSE);
		m_editLabelX.EnableWindow(TRUE);
		m_editLabelY.EnableWindow(TRUE);
		m_editFrontX.EnableWindow(TRUE);
		m_editFrontY.EnableWindow(TRUE);
		m_editBackX.EnableWindow(TRUE);
		m_editBackY.EnableWindow(TRUE);
	}


	UpdateData(FALSE);

	return;
}

void CCoverZweckformDlg::OnSelchangeComboMarke() 
{
	m_pLayout->m_nZweckformFirmaIndex = m_comboMarke.GetCurSel() - 1;
	m_pLayout->m_bZweckformUser = (m_pLayout->m_nZweckformFirmaIndex == -1);

	if (m_pLayout->m_nZweckformFirmaIndex < 0 || m_pLayout->m_nZweckformIndex >= (int)m_pLayout->m_vecZweckformen[m_pLayout->m_nZweckformFirmaIndex].m_vecFormate.size())
	{
		m_pLayout->m_nZweckformIndex = 0;
	}
	
	InitEditBoxes();
	if (! m_pLayout->m_bZweckformUser)
	{
		m_comboNummer.ResetContent();
		for (int i = 0; i != (int)m_pLayout->m_vecZweckformen[m_pLayout->m_nZweckformFirmaIndex].m_vecFormate.size(); i++)
		{
			m_comboNummer.AddString(m_pLayout->m_vecZweckformen[m_pLayout->m_nZweckformFirmaIndex].m_vecFormate[i].m_strNummer);
		}

		m_comboNummer.SetCurSel(m_pLayout->m_nZweckformIndex);
		OnSelchangeComboNummer();
	}
	return;	
}

void CCoverZweckformDlg::OnSelchangeComboNummer() 
{
	m_pLayout->m_nZweckformIndex = m_comboNummer.GetCurSel();

	m_nFrontX = (m_pLayout->m_vecZweckformen[m_pLayout->m_nZweckformFirmaIndex].m_vecFormate[m_pLayout->m_nZweckformIndex].m_nFX) != -1 ? m_pLayout->m_vecZweckformen[m_pLayout->m_nZweckformFirmaIndex].m_vecFormate[m_pLayout->m_nZweckformIndex].m_nFX : m_pLayout->m_nFrontX;
	m_nFrontY = (m_pLayout->m_vecZweckformen[m_pLayout->m_nZweckformFirmaIndex].m_vecFormate[m_pLayout->m_nZweckformIndex].m_nFY) != -1 ? m_pLayout->m_vecZweckformen[m_pLayout->m_nZweckformFirmaIndex].m_vecFormate[m_pLayout->m_nZweckformIndex].m_nFY : m_pLayout->m_nFrontY;;

	m_nBackX = (m_pLayout->m_vecZweckformen[m_pLayout->m_nZweckformFirmaIndex].m_vecFormate[m_pLayout->m_nZweckformIndex].m_nBX) != -1 ? m_pLayout->m_vecZweckformen[m_pLayout->m_nZweckformFirmaIndex].m_vecFormate[m_pLayout->m_nZweckformIndex].m_nBX : m_pLayout->m_nBackX;
	m_nBackY = (m_pLayout->m_vecZweckformen[m_pLayout->m_nZweckformFirmaIndex].m_vecFormate[m_pLayout->m_nZweckformIndex].m_nBY) != -1 ? m_pLayout->m_vecZweckformen[m_pLayout->m_nZweckformFirmaIndex].m_vecFormate[m_pLayout->m_nZweckformIndex].m_nBY : m_pLayout->m_nBackY;

	m_nLabelX = (m_pLayout->m_vecZweckformen[m_pLayout->m_nZweckformFirmaIndex].m_vecFormate[m_pLayout->m_nZweckformIndex].m_nLX) != -1 ? m_pLayout->m_vecZweckformen[m_pLayout->m_nZweckformFirmaIndex].m_vecFormate[m_pLayout->m_nZweckformIndex].m_nLX : m_pLayout->m_nLabelX;
	m_nLabelY = (m_pLayout->m_vecZweckformen[m_pLayout->m_nZweckformFirmaIndex].m_vecFormate[m_pLayout->m_nZweckformIndex].m_nLY) != -1 ? m_pLayout->m_vecZweckformen[m_pLayout->m_nZweckformFirmaIndex].m_vecFormate[m_pLayout->m_nZweckformIndex].m_nLY : m_pLayout->m_nLabelY;

	SetValues();
	InitEditBoxes();
	return;
}


int CCoverZweckformDlg::TransformValue(const CString& strValue)
{
	CString str = strValue;
	double dValue;
	swscanf(str, L"%lf", &dValue);
	dValue *= 100;
	return (int) (dValue + 0.5);
}


void CCoverZweckformDlg::ValidateValues()
{
	CString str;
	m_editFrontX.GetWindowText(str);
	m_nFrontX = TransformValue(str);
	m_editFrontY.GetWindowText(str);
	m_nFrontY = TransformValue(str);

	m_editBackX.GetWindowText(str);
	m_nBackX = TransformValue(str);
	m_editBackY.GetWindowText(str);
	m_nBackY = TransformValue(str);

	m_editLabelX.GetWindowText(str);
	m_nLabelX = TransformValue(str);
	m_editLabelY.GetWindowText(str);
	m_nLabelY = TransformValue(str);

	m_editRandLinks.GetWindowText(str);
	m_pLayout->m_nRandX = TransformValue(str);
	m_editRandOben.GetWindowText(str);
	m_pLayout->m_nRandY = TransformValue(str);

	if (m_nLabelX < 0)
		m_nLabelX = 0;
	if (m_nLabelY < 0)
		m_nLabelY = 0;
	if (m_nFrontX < 0)
		m_nFrontX = 0;
	if (m_nFrontY < 0)
		m_nFrontY = 0;
	if (m_nBackX < 0)
		m_nBackX = 0;
	if (m_nBackY < 0)
		m_nBackY = 0;
	if (m_nLabelX + m_pLayout->gl_COVERLABELWIDTH > 2100)
		m_nLabelX = 2100 - m_pLayout->gl_COVERLABELWIDTH;
	if (m_nLabelY + m_pLayout->gl_COVERLABELHEIGHT > 2970)
		m_nLabelY = 2970 - m_pLayout->gl_COVERLABELHEIGHT;
	if (m_nFrontX + m_pLayout->gl_COVERFRONTWIDTH > 2100)
		m_nFrontX = 2100 - m_pLayout->gl_COVERFRONTWIDTH;
	if (m_nFrontY + m_pLayout->gl_COVERFRONTHEIGHT > 2970)
		m_nFrontY = 2970 - m_pLayout->gl_COVERFRONTHEIGHT;
	if (m_nBackX + m_pLayout->gl_COVERBACKWIDTH > 2100)
		m_nBackX = 2100 - m_pLayout->gl_COVERBACKWIDTH;
	if (m_nBackY + m_pLayout->gl_COVERBACKHEIGHT > 2970)
		m_nBackY = 2970 - m_pLayout->gl_COVERBACKHEIGHT;

	if (m_pLayout->m_nRandX > 2100 || m_pLayout->m_nRandX < 0)
		m_pLayout->m_nRandX = 0;
	if (m_pLayout->m_nRandY > 2970 || m_pLayout->m_nRandY < 0)
		m_pLayout->m_nRandY = 0;


	return;
}

void CCoverZweckformDlg::SetValues()
{
	CString str;
	double d = (double) m_nFrontX / 100.0;
	str.Format(L"%0#5.2lf", d);
	m_editFrontX.SetWindowText(str);

	d = (double) m_nFrontY / 100.0;
	str.Format(L"%0#5.2lf", d);
	m_editFrontY.SetWindowText(str);
	
	d = (double) m_nBackX / 100.0;
	str.Format(L"%0#5.2lf", d);
	m_editBackX.SetWindowText(str);
	
	d = (double) m_nBackY / 100.0;
	str.Format(L"%0#5.2lf", d);
	m_editBackY.SetWindowText(str);
	
	d = (double) m_nLabelX/ 100.0;
	str.Format(L"%0#5.2lf", d);
	m_editLabelX.SetWindowText(str);
	
	d = (double) m_nLabelY / 100.0;
	str.Format(L"%0#5.2lf", d);
	m_editLabelY.SetWindowText(str);

	d = (double) m_pLayout->m_nRandY / 100.0;
	str.Format(L"%0#5.2lf", d);
	m_editRandOben.SetWindowText(str);

	d = (double) m_pLayout->m_nRandX / 100.0;
	str.Format(L"%0#5.2lf", d);
	m_editRandLinks.SetWindowText(str);

	return;
}

void CCoverZweckformDlg::OnKillfocusEdit() 
{
	ValidateValues();
	SetValues();
	return;	
}

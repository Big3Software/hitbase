// PlugInOptionsDlg.cpp : implementation file
//

#include "stdafx.h"
#include "../../App/Hitbase/resource.h"
#include "PlugInManager.h"
#include "PlugInOptionsDlg.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CPlugInOptionsDlg dialog


CPlugInOptionsDlg::CPlugInOptionsDlg(CPlugIn* pPlugIn, CWnd* pParent /*=NULL*/)
	: CDialog(CPlugInOptionsDlg::IDD, pParent)
{
	//{{AFX_DATA_INIT(CPlugInOptionsDlg)
	m_sDescription = _T("");
	//}}AFX_DATA_INIT

	m_pPlugIn = pPlugIn;
}


void CPlugInOptionsDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CPlugInOptionsDlg)
	DDX_Control(pDX, IDC_OPTIONSLIST, m_OptionsListCtrl);
	DDX_Text(pDX, IDC_DESCRIPTION, m_sDescription);
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(CPlugInOptionsDlg, CDialog)
	//{{AFX_MSG_MAP(CPlugInOptionsDlg)
	ON_NOTIFY(LVN_ITEMCHANGED, IDC_OPTIONSLIST, OnItemchangedOptionslist)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CPlugInOptionsDlg message handlers

BOOL CPlugInOptionsDlg::OnInitDialog() 
{
	CDialog::OnInitDialog();
	
	CRect rect;
	m_OptionsListCtrl.GetClientRect(&rect);

	m_OptionsListCtrl.InsertColumn(0, get_string(TEXT_FIELD), LVCFMT_LEFT, rect.right/2);
	m_OptionsListCtrl.InsertColumn(1, get_string(TEXT_VALUE), LVCFMT_LEFT, rect.right/2);
	
	m_OptionsListCtrl.SetColumnFlags(1, GLCCF_EDIT);
	m_OptionsListCtrl.SetColumnEditWidth(1, _MAX_PARAM_LENGTH-1);

	CStringArray sa;
	sa.Add(get_string(TEXT_NO));
	sa.Add(get_string(TEXT_YES));

	for (int i=0;i<m_pPlugIn->m_nNumberOfParameters;i++)
	{
		CString str;

		m_OptionsListCtrl.InsertItem(i, CString(m_pPlugIn->m_Param[i].szName));

		switch (m_pPlugIn->m_Param[i].nType)
		{
		case HPIPT_BOOLEAN:
			if (m_pPlugIn->m_Param[i].nValue == 1)
				str = get_string(TEXT_YES);
			else
				str = get_string(TEXT_NO);
			m_OptionsListCtrl.SetCellType(i, 1, GLCCT_COMBOBOX);
			m_OptionsListCtrl.SetComboBoxItems(&sa, i, 1);
			break;
		case HPIPT_INTEGER:
			str.Format(L"%d", m_pPlugIn->m_Param[i].nValue);
			break;
		case HPIPT_STRING:
			str = m_pPlugIn->m_Param[i].szValue;
			break;
		case HPIPT_COLOR:
			str.Format(L"%06x", m_pPlugIn->m_Param[i].nValue);
			m_OptionsListCtrl.SetCellType(i, 1, GLCCT_COLOR);
			break;
		default:
			ASSERT(FALSE);
		}		

		m_OptionsListCtrl.SetItem(i, 1, LVIF_TEXT, str, 0, 0, 0, 0);
	}

	return TRUE;  // return TRUE unless you set the focus to a control
	              // EXCEPTION: OCX Property Pages should return FALSE
}

void CPlugInOptionsDlg::OnOK() 
{
	USES_CONVERSION;
	m_OptionsListCtrl.CloseEditWindow();

	for (int i=0;i<m_OptionsListCtrl.GetItemCount();i++)
	{
		CString str;

		str = m_OptionsListCtrl.GetItemText(i, 1);

		switch (m_pPlugIn->m_Param[i].nType)
		{
		case HPIPT_BOOLEAN:
			m_pPlugIn->m_Param[i].nValue = (str == get_string(TEXT_YES));
			break;
		case HPIPT_INTEGER:
			m_pPlugIn->m_Param[i].nValue = _wtoi(str);
			break;
		case HPIPT_STRING:
			strcpy(m_pPlugIn->m_Param[i].szValue, W2A(str));
			break;
		case HPIPT_COLOR:
			m_pPlugIn->m_Param[i].nValue = wcstol(str, 0, 16);
			break;
		default:
			ASSERT(FALSE);
		}
	}
	
	CDialog::OnOK();
}

void CPlugInOptionsDlg::OnItemchangedOptionslist(NMHDR* pNMHDR, LRESULT* pResult) 
{
	int nSel = m_OptionsListCtrl.GetCurSel();

	if (nSel >= 0)
		m_sDescription = m_pPlugIn->m_Param[nSel].szDescription;
	else
		m_sDescription = "";

	UpdateData(FALSE);
}

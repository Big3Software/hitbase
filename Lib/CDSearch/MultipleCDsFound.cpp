// MultipleCDsFound.cpp : implementation file
//

#include "stdafx.h"
#include "../../app/hitbase/resource.h"
#include "MultipleCDsFound.h"
#include "../hitmisc/misc.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#endif

/////////////////////////////////////////////////////////////////////////////
// CMultipleCDsFound dialog


CMultipleCDsFound::CMultipleCDsFound(List<Big3::Hitbase::DataBaseEngine::CD^>^ cdarray, CWnd* pParent /*=NULL*/)
	: CDialog(CMultipleCDsFound::IDD, pParent)
{
	//{{AFX_DATA_INIT(CMultipleCDsFound)
		// NOTE: the ClassWizard will add member initialization here
	//}}AFX_DATA_INIT

	CDArray = cdarray;
}


void CMultipleCDsFound::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CMultipleCDsFound)
	DDX_Control(pDX, IDOK, m_OKCtrl);
	DDX_Control(pDX, IDC_LIST1, m_ListCtrl);
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(CMultipleCDsFound, CDialog)
	//{{AFX_MSG_MAP(CMultipleCDsFound)
	ON_NOTIFY(LVN_ITEMCHANGED, IDC_LIST1, OnItemchangedList)
	ON_NOTIFY(NM_DBLCLK, IDC_LIST1, OnDblclkList)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CMultipleCDsFound message handlers

BOOL CMultipleCDsFound::OnInitDialog() 
{
	CDialog::OnInitDialog();
	
	CRect rect;
	m_ListCtrl.GetClientRect(&rect);
	m_ListCtrl.InsertColumn(0, get_string(TEXT_ARTIST), LVCFMT_LEFT, rect.right/2);
	m_ListCtrl.InsertColumn(1, get_string(TEXT_TITLE), LVCFMT_LEFT, rect.right/2);

	for (int i=0;i<CDArray->Count;i++)
	{
		m_ListCtrl.InsertItem(i, (CString)CDArray->default[i]->Artist);
		m_ListCtrl.SetItem(i, 1, LVIF_TEXT, (CString)CDArray->default[i]->Title, 0, 0, 0, 0);
	}

	UpdateWindowState();
	
	return TRUE;  // return TRUE unless you set the focus to a control
	              // EXCEPTION: OCX Property Pages should return FALSE
}

void CMultipleCDsFound::OnOK() 
{
	m_nSelected = m_ListCtrl.GetCurSel();
	
	CDialog::OnOK();
}

void CMultipleCDsFound::UpdateWindowState()
{
	m_OKCtrl.EnableWindow(m_ListCtrl.GetCurSel() >= 0);
}

void CMultipleCDsFound::OnItemchangedList(NMHDR* pNMHDR, LRESULT* pResult) 
{
	UpdateWindowState();
	
	*pResult = 0;
}

void CMultipleCDsFound::OnDblclkList(NMHDR* pNMHDR, LRESULT* pResult) 
{
	if (m_ListCtrl.GetCurSel() >= 0)
		OnOK();
	
	*pResult = 0;
}

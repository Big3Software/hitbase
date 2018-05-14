// ChooseFieldsDlg.cpp : implementation file
//

#include "stdafx.h"
#include "hitdb.h"
#include "../hitmisc/misc.h"
#include "ChooseFieldsDlg.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CChooseFieldsDlg dialog


CChooseFieldsDlg::CChooseFieldsDlg(CDataBase* pdb, CWnd* pParent /*=NULL*/)
	: CDialog(CChooseFieldsDlg::IDD, pParent)
{
	//{{AFX_DATA_INIT(CChooseFieldsDlg)
		// NOTE: the ClassWizard will add member initialization here
	//}}AFX_DATA_INIT

    hfontwing.CreateFont(16, 0, 0, 0, 0, 0, 0, 0, DEFAULT_CHARSET, 0, 0, 0, 0, L"Wingdings");
	m_pdb = pdb;
}

void CChooseFieldsDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CChooseFieldsDlg)
	DDX_Control(pDX, IDUP, m_UpCtrl);
	DDX_Control(pDX, IDDOWN, m_DownCtrl);
	DDX_Control(pDX, IDC_ADD, m_AddCtrl);
	DDX_Control(pDX, IDC_REMOVE, m_RemoveCtrl);
	DDX_Control(pDX, IDC_LIST2, m_SelectedCtrl);
	DDX_Control(pDX, IDC_LIST1, m_AvailableCtrl);
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(CChooseFieldsDlg, CDialog)
	//{{AFX_MSG_MAP(CChooseFieldsDlg)
	ON_BN_CLICKED(IDC_STANDARD, OnStandard)
	ON_BN_CLICKED(IDUP, OnUp)
	ON_BN_CLICKED(IDDOWN, OnDown)
	ON_BN_CLICKED(IDC_ADD, OnAdd)
	ON_BN_CLICKED(IDC_REMOVE, OnRemove)
	ON_LBN_SELCHANGE(IDC_LIST1, OnSelchangeList1)
	ON_LBN_SELCHANGE(IDC_LIST2, OnSelchangeList2)
	ON_LBN_DBLCLK(IDC_LIST1, OnDblclkList1)
	ON_LBN_DBLCLK(IDC_LIST2, OnDblclkList2)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CChooseFieldsDlg message handlers

void CChooseFieldsDlg::OnStandard() 
{
	// Nur die Felder übertragen!!
	m_FieldList.RemoveAll();

	for (int i=0;i<m_StandardFields.GetSize();i++)
		m_FieldList.Add(m_StandardFields[i]);
	
	FillListboxes();
}

void CChooseFieldsDlg::OnUp() 
{
	int i;
	
	i = m_SelectedCtrl.GetCurSel();
	
	if (i < 1)                                              // Weiter hoch geht's nicht mehr
		return;
	
	m_FieldList.Move(i, -1);
//	m_FieldList.InsertAt(i-1, m_FieldList[i]);
//	m_FieldList.RemoveAt(i+1);
	
	FillListboxes();
	
	m_SelectedCtrl.SetCurSel(i-1);
	
	UpdateWindowState();
}

void CChooseFieldsDlg::OnDown() 
{
	int i;
	
	i = m_SelectedCtrl.GetCurSel();
	
	if (i < 0 || i >= m_SelectedCtrl.GetCount()-1)            // Weiter runter geht's nicht mehr
		return;
	
	m_FieldList.Move(i, 1);
//	m_FieldList.InsertAt(i+2, m_FieldList[i]);
//	m_FieldList.RemoveAt(i);
	
	FillListboxes();
	
	m_SelectedCtrl.SetCurSel(i+1);
	
	UpdateWindowState();
}

void CChooseFieldsDlg::OnAdd() 
{
	int i;
	
	i = m_AvailableCtrl.GetCurSel();
	
	if (i < 0)
		return;

	m_FieldList.Add(m_AllFields[i], 0);
	m_AllFields.RemoveAt(i);
	
	FillListboxes();
	
	UpdateWindowState();
}

void CChooseFieldsDlg::OnRemove() 
{
	int i;
	
	i = m_SelectedCtrl.GetCurSel();
	
	if (i < 0)
		return;

	m_AllFields.Add(m_FieldList[i], 0);
	m_FieldList.RemoveAt(i);
	
	FillListboxes();
	
	UpdateWindowState();
}

BOOL CChooseFieldsDlg::OnInitDialog() 
{
	CDialog::OnInitDialog();
	
	m_DownCtrl.SetFont(&hfontwing);
	m_UpCtrl.SetFont(&hfontwing);
	
	FillListboxes();
	
	return TRUE;
}

void CChooseFieldsDlg::UpdateWindowState()
{
	int i;
	
	i = m_SelectedCtrl.GetCurSel();
	
	m_AddCtrl.EnableWindow(m_AvailableCtrl.GetCurSel() >= 0 ? TRUE : FALSE);
	m_RemoveCtrl.EnableWindow(m_SelectedCtrl.GetCurSel() >= 0 ? TRUE : FALSE);
	m_UpCtrl.EnableWindow(i > 0 ? TRUE : FALSE);
	m_DownCtrl.EnableWindow((i < m_SelectedCtrl.GetCount()-1 && i >= 0) ? TRUE : FALSE);
}

void CChooseFieldsDlg::OnSelchangeList1() 
{
	UpdateWindowState();
}

void CChooseFieldsDlg::OnSelchangeList2() 
{
	UpdateWindowState();
}

void CChooseFieldsDlg::OnDblclkList1() 
{
	OnAdd();
}

void CChooseFieldsDlg::OnDblclkList2() 
{
	OnRemove();
}

void CChooseFieldsDlg::SetDefault(const CFieldList& FieldList)
{
	m_StandardFields = FieldList;
}

void CChooseFieldsDlg::FillListboxes()
{
	int i, j;
	CString str;
	
	m_AvailableCtrl.ResetContent();
	m_SelectedCtrl.ResetContent();

	m_AllFields.SetType(m_FieldList.GetType());
	m_AllFields = m_FieldList.GetAllFields();	

	for (i=0;i < m_FieldList.GetSize();i++)
	{
		for (j=0;j<m_AllFields.GetSize();j++)
		{
			if (m_FieldList[i] == m_AllFields[j])
			{
				m_AllFields.RemoveAt(j);
				break;
			}
		}
	}

	int iDummy;
	for (i=0;i < m_AllFields.GetSize();i++)
	{
		if (m_FieldList.GetType() & (FLF_CD|FLF_TRACK))
			m_AvailableCtrl.AddString(m_pdb->GetFieldName(m_AllFields[i], iDummy, TRUE));
		else
			m_AvailableCtrl.AddString(m_pdb->GetFieldName(m_AllFields[i], iDummy, FALSE));
	}

	for (i=0;i<m_FieldList.GetSize();i++)
	{
		if (m_FieldList.GetType() & (FLF_CD|FLF_TRACK))
			m_SelectedCtrl.AddString(m_pdb->GetFieldName(m_FieldList[i], iDummy, TRUE));
		else
			m_SelectedCtrl.AddString(m_pdb->GetFieldName(m_FieldList[i], iDummy, FALSE));
	}
}

void CChooseFieldsDlg::SetFields(const CFieldList& FieldList)
{
	m_FieldList = FieldList;
}

void CChooseFieldsDlg::AddField(UINT uiField)
{
	m_FieldList.Add(uiField);
}

void CChooseFieldsDlg::OnOK() 
{
	CDialog::OnOK();
}


// hitcommn.cpp : implementation file
//

#include "stdafx.h"
#include "hitmisc.h"
#include "../../App/hitbase/resource.h"
#include "hitcommn.h"

#ifdef _DEBUG
#undef THIS_FILE
static char BASED_CODE THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CCommonCheckDlg dialog


CCommonCheckDlg::CCommonCheckDlg(UINT nIDTemplate, const CString& message, CWnd* pParent /*=NULL*/) : 
                CDialog(nIDTemplate, pParent)
{
	//{{AFX_DATA_INIT(CCommonCheckDlg)
	//}}AFX_DATA_INIT

   m_Check1 = 0;
   m_Message = message;
}


void CCommonCheckDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CCommonCheckDlg)
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(CCommonCheckDlg, CDialog)
	//{{AFX_MSG_MAP(CCommonCheckDlg)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()



BOOL CCommonCheckDlg::OnCommand(WPARAM wParam, LPARAM lParam) 
{
   switch (LOWORD(wParam))
   {
   case IDYES:
   case IDNO:
   case IDOK:
   case IDCANCEL:
   case IDC_BUTTON1:
   case IDC_BUTTON2:
   case IDC_BUTTON3:
   case IDC_BUTTON4:
   case IDC_BUTTON5:
      EndDialog(LOWORD(wParam));
      return TRUE;

   case IDC_CHECK1:
      m_Check1 = IsDlgButtonChecked(IDC_CHECK1);
      break;
   }
   
	return CDialog::OnCommand(wParam, lParam);
}

BOOL CCommonCheckDlg::OnInitDialog() 
{
	CDialog::OnInitDialog();
	
   if (m_Message != "")
      SetDlgItemText(IDC_TEXT1, m_Message);
	
	return TRUE;  // return TRUE unless you set the focus to a control
	              // EXCEPTION: OCX Property Pages should return FALSE
}

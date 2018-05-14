// NewFileDialog.cpp : implementation file
//
// Neue CFileDialog, damit unter Win2k/ME/XP der neue Common Dialog kommt!

#include "stdafx.h"
#include "hitmisc.h"
#include "config.h"

#undef CFileDialog

#include "NewFileDialog.h"

// CNewFileDialog

IMPLEMENT_DYNAMIC(CNewFileDialog, CFileDialog)
CNewFileDialog::CNewFileDialog(BOOL bOpenFileDialog, LPCTSTR lpszDefExt, LPCTSTR lpszFileName,
		DWORD dwFlags, LPCTSTR lpszFilter, CWnd* pParentWnd) :
		CFileDialog(bOpenFileDialog, lpszDefExt, lpszFileName, dwFlags, lpszFilter, pParentWnd)
{
}

CNewFileDialog::~CNewFileDialog()
{
}

BEGIN_MESSAGE_MAP(CNewFileDialog, CFileDialog)
END_MESSAGE_MAP()

// CNewFileDialog message handlers

bool CNewFileDialog::IsNewWindows()
{
	OSVERSIONINFO ovi;

	ovi.dwOSVersionInfoSize = sizeof(OSVERSIONINFO);
	BOOL bRet = ::GetVersionEx(&ovi);

	return (bRet && ((ovi.dwMajorVersion >= 5) || (ovi.dwMajorVersion == 4 && ovi.dwMinorVersion >= 90)));
}

INT_PTR CNewFileDialog::DoModal(const CString& sSaveDirectory /* = "" */)
{
	CString sDirectory;

	if (!sSaveDirectory.IsEmpty())
	{
		sDirectory = CConfig::ReadGlobalRegistryKeyString("LastFolder" + sSaveDirectory, "");
		m_ofnEx.lpstrInitialDir = sDirectory;
	}

	INT_PTR nResult = CFileDialog::DoModal();

	if (nResult && !sSaveDirectory.IsEmpty())
	{
		CConfig::WriteGlobalRegistryKeyString("LastFolder" + sSaveDirectory, GetPathFromFileName(GetPathName()));
	}

	return nResult ? nResult : IDCANCEL;
}

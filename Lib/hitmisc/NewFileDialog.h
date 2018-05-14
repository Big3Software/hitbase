#pragma once

#undef CFileDialog

// Windows 2000 version of OPENFILENAME.
// The new version has three extra members.
// This is copied from commdlg.h
struct OPENFILENAMEEX : public OPENFILENAME { 
  void *        pvReserved;
  DWORD         dwReserved;
  DWORD         FlagsEx;
};

// CNewFileDialog
class HITMISC_INTERFACE CNewFileDialog : public CFileDialog
{
	DECLARE_DYNAMIC(CNewFileDialog)

public:
	CNewFileDialog(BOOL bOpenFileDialog, // TRUE for FileOpen, FALSE for FileSaveAs
		LPCTSTR lpszDefExt = NULL,
		LPCTSTR lpszFileName = NULL,
		DWORD dwFlags = OFN_HIDEREADONLY | OFN_OVERWRITEPROMPT | OFN_ENABLETEMPLATE,
		LPCTSTR lpszFilter = NULL,
		CWnd* pParentWnd = NULL);
	virtual ~CNewFileDialog();

	bool IsNewWindows();
protected:
	DECLARE_MESSAGE_MAP()

    OPENFILENAMEEX m_ofnEx; // new Windows 2000 version of OPENFILENAME
public:
	virtual INT_PTR DoModal(const CString& sSaveDirectory = L"");
};

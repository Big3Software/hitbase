#include "stdafx.h"
#include "hitmisc.h"
#include "HitbaseWinAppBase.h"
#include "httpfileasync.h"

using namespace Big3::Hitbase::Configuration;

CHttpFileAsync::CHttpFileAsync(void)
{
	m_bAbortTransfer = false;
	m_bTransferActive = false;
	m_pWinThread = NULL;
}

CHttpFileAsync::~CHttpFileAsync(void)
{
}

BOOL CHttpFileAsync::ReadFileFromURL(const CString& sServer, const CString& sFilename, BOOL bDisplayErrorMessage /* = TRUE */, BOOL bBackground /* = TRUE */)
{
	CInternetSession * pInternetSession;

	switch (Settings::Current->ProxyType)
	{
	case 0:
		pInternetSession = new CInternetSession;
		break;
	case 1:
		pInternetSession = new CInternetSession(NULL, 1, INTERNET_OPEN_TYPE_PROXY, (CString)Settings::Current->ProxyServerName);
		break;
	default:
		ASSERT(FALSE);
	}
	
	ASSERT(pInternetSession);

	CHttpConnection * pHttpConnection;
	try
	{
	    pHttpConnection = pInternetSession->GetHttpConnection(sServer);
	}
	catch (CInternetException* e)
	{
		if (bDisplayErrorMessage)
		{
			wchar_t szCause[255];
			e->GetErrorMessage(szCause, 255);
			AfxMessageBox(szCause);
		}

		e->Delete();
		pInternetSession->Close();
		delete pInternetSession;

		return FALSE;
	}

	BOOL bRet = ReadFileFromConnection(pHttpConnection, sFilename, bDisplayErrorMessage, bBackground);

	pHttpConnection->Close();
	delete pHttpConnection;

	pInternetSession->Close();
	delete pInternetSession;

	return bRet;
}

BOOL CHttpFileAsync::ReadFileFromConnection(CHttpConnection * pHttpConnection, const CString &sFilename, BOOL bDisplayErrorMessage /* = TRUE */, BOOL bBackground /* = TRUE */)
{
	m_pHttpConnection = pHttpConnection;
	m_sFilename = sFilename;
	m_bDisplayErrorMessage = bDisplayErrorMessage;
	m_bTransferActive = true;
	m_sContent = "";

	if (bBackground)
		m_pWinThread = AfxBeginThread(HTTPDownloadThread, (LPVOID)this);
	else
		HTTPDownloadThread((LPVOID)this);

	return (m_pWinThread != NULL);
}

UINT CHttpFileAsync::HTTPDownloadThread(LPVOID pParam)
{
	USES_CONVERSION;
	CHttpFileAsync* pHttpFileAsync = (CHttpFileAsync*)pParam;

	const TCHAR szHeaders[] =
		_T("Accept: text/*\r\nUser-Agent: Hitbase 2010\r\n");

	DWORD dwHttpRequestFlags =
		INTERNET_FLAG_RELOAD | INTERNET_FLAG_NO_AUTO_REDIRECT;

	CHttpFile * pFile = pHttpFileAsync->m_pHttpConnection->OpenRequest(CHttpConnection::HTTP_VERB_GET,
		pHttpFileAsync->m_sFilename, NULL, 1, NULL, NULL, dwHttpRequestFlags);
	pFile->AddRequestHeaders(szHeaders);

	try
	{
		pFile->SendRequest();
	}
	catch (CInternetException *e)
	{
		if (pHttpFileAsync->m_bDisplayErrorMessage)
		{
			wchar_t szCause[255];
			e->GetErrorMessage(szCause, 255);
			AfxMessageBox(szCause);
		}

		e->Delete();
		pFile->Close();
		delete pFile;

		pHttpFileAsync->m_bTransferActive = false;
		return FALSE;
	}

	//CString str;

	char str[10000];
	while (pFile->ReadString((LPTSTR)str, 10000))
	{
		pHttpFileAsync->m_sContent += CString(A2W(str)) + "\r\n";
	}

	pFile->Close();
	delete pFile;

	pHttpFileAsync->m_bTransferActive = false;
	pHttpFileAsync->m_pWinThread = NULL;
	return TRUE;
}

void CHttpFileAsync::AbortTransfer(void)
{
	m_bAbortTransfer = true;

	// Den Thread einfach killen!
	if (m_pWinThread)
		::TerminateThread(m_pWinThread->m_hThread, 0);

	m_bTransferActive = false;
}

bool CHttpFileAsync::IsTransferActive(void)
{
	return m_bTransferActive;
}

CString CHttpFileAsync::GetResult(void)
{
	return m_sContent;
}

bool CHttpFileAsync::WasTransferAborted(void)
{
	return m_bAbortTransfer;
}


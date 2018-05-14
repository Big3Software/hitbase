#pragma once

class HITMISC_INTERFACE CHttpFileAsync
{
public:
	CHttpFileAsync(void);
	~CHttpFileAsync(void);

	BOOL ReadFileFromURL(const CString& sServer, const CString& sFilename, BOOL bDisplayErrorMessage = TRUE, BOOL bBackground = TRUE);
	BOOL ReadFileFromConnection(CHttpConnection * pHttpConnection, const CString &sFilename, BOOL bDisplayErrorMessage = TRUE, BOOL bBackground = TRUE);

private:
	static UINT HTTPDownloadThread(LPVOID pParam);

	CHttpConnection* m_pHttpConnection;
	CString m_sFilename;
	CString m_sContent;
	BOOL m_bDisplayErrorMessage;

	volatile bool m_bAbortTransfer;
	volatile bool m_bTransferActive;

	CWinThread* m_pWinThread;
public:
	void AbortTransfer(void);
	bool IsTransferActive(void);
	CString GetResult(void);
	bool WasTransferAborted(void);
};

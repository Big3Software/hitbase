#pragma once

// CDDBSock.h : header file
//

/////////////////////////////////////////////////////////////////////////////
// CCDDBSock command target

class CCDDBSock : public CSocket
{
// Attributes
public:

// Operations
public:
	CCDDBSock();
	virtual ~CCDDBSock();

// Overrides
public:
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CCDDBSock)
	public:
	virtual void OnReceive(int nErrorCode);
	virtual void OnSend(int nErrorCode);
	virtual BOOL OnMessagePending();
	virtual int Receive(void* lpBuf, int nBufLen, int nFlags = 0);
	virtual int Send(const char* lpBuf, int nBufLen, int nFlags = 0);
	//}}AFX_VIRTUAL

	// Generated message map functions
	//{{AFX_MSG(CCDDBSock)
		// NOTE - the ClassWizard will add and remove member functions here.
	//}}AFX_MSG

// Implementation
protected:
	unsigned int m_TimeoutSec; // Timeout für Receive in Sekunden - 0 = ewig und 3 Tage
	time_t m_PendingSocketIOTime; // Receive/Send Start Zeit - warten bis m_TimeoutSec vorbei
};

/////////////////////////////////////////////////////////////////////////////


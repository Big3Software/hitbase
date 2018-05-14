// CDDBSock.cpp : implementation file
//

#include "stdafx.h"
//#include "../../app/hitbase/hitbase.h"
#include "CDDBSock.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#endif

/////////////////////////////////////////////////////////////////////////////
// CCDDBSock

CCDDBSock::CCDDBSock()
{
	// TODO!!!!!!!!!!!! Read from REGISTRYENTRY !!!!!!!!!!!!!
	m_PendingSocketIOTime = -1;
	m_TimeoutSec = 20;
}

CCDDBSock::~CCDDBSock()
{
}


// Do not edit the following lines, which are needed by ClassWizard.
#if 0
BEGIN_MESSAGE_MAP(CCDDBSock, CSocket)
	//{{AFX_MSG_MAP(CCDDBSock)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()
#endif	// 0

/////////////////////////////////////////////////////////////////////////////
// CCDDBSock member functions

// Jetzt kommen Daten zum empfangen
void CCDDBSock::OnReceive(int nErrorCode) 
{
	// TODO: Add your specialized code here and/or call the base class
	
	CSocket::OnReceive(nErrorCode);
}

// Daten könne nun gesendet werden
void CCDDBSock::OnSend(int nErrorCode) 
{
	// TODO: Add your specialized code here and/or call the base class
	
	CSocket::OnSend(nErrorCode);
}

// Receive/Send wartet auf Server Antwort CSocket
BOOL CCDDBSock::OnMessagePending() 
{
	time_t current_time;

	if (m_TimeoutSec > 0)
	{
		if (m_PendingSocketIOTime == -1)
			time(&m_PendingSocketIOTime);

		time (&current_time);

		if ((unsigned int)current_time - m_PendingSocketIOTime > m_TimeoutSec)
		{
			// Abbruch des aktuellen Receive/Send Vorgangs
			CancelBlockingCall ();
		}
	}

	return CSocket::OnMessagePending();
}


int CCDDBSock::Receive(void* lpBuf, int nBufLen, int nFlags) 
{
	// Reset Pending value for next Receive
	m_PendingSocketIOTime = -1;

	return CSocket::Receive(lpBuf, nBufLen, nFlags);
}

int CCDDBSock::Send(const char* lpBuf, int nBufLen, int nFlags) 
{
	// Reset Pending value for next Send
	m_PendingSocketIOTime = -1;

	return CSocket::Send(lpBuf, nBufLen, nFlags);
}

// Generelle CLASS zum lesen in der cddb Datenbank
// Es wird HTTP und Sockets connect unterstützt

#pragma once

#include "cddbsock.h"

class CCDDBConnect
{
public:
	CCDDBConnect();
	virtual ~CCDDBConnect();
	enum enumConnectTypes { connectHTTP, connectSockets };

protected:
	CInternetSession * m_pInternetSession;
	CHttpConnection * m_pHttpConnection;

	CCDDBSock * m_cddbSocket;
	BOOL m_CDDBProtocol;
	int m_PortNumber;
	CString m_CDDBServerName;
	CString m_CDDBInitString;
	CString m_CDDBHelloString;
	int m_CDDBProtocolLevel;
	CHttpFile * m_pFile;
	BOOL m_HTTPTransmitPending;
	void CharToHTTPTransmit (CString & sInternetFileName);

public:
	BOOL CDDBOpen(CDArchiveConfig^ CDArchive);
	BOOL CDDBInit();
	BOOL CDDBSetProtocolLevel(int nProtocolLevel);
	BOOL CDDBReceive(CString & ReceiveCDDBData, BOOL WaitForTermChar=FALSE);
	BOOL CDDBTransmit(CString TransmitCDDBData);
	BOOL CDDBClose();
	int CDDBGetProtocolLevel();
};

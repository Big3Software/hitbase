// CDDBConnect.cpp : implementation file
//

#include "stdafx.h"
#include "../hitmisc/HitbaseWinAppBase.h"
#include "../../app/hitbase/resource.h"
#include "cdarchive.h"
#include "CDDBConnect.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#endif

/////////////////////////////////////////////////////////////////////////////
// CCDDBConnect

CCDDBConnect::CCDDBConnect()
{
	m_CDDBProtocolLevel = 1;
	m_HTTPTransmitPending = FALSE;
	m_pFile = NULL;
	m_cddbSocket = NULL;
	m_pHttpConnection = NULL;
	m_pInternetSession = NULL;
}

CCDDBConnect::~CCDDBConnect()
{
	// !!!!!!!!!!!!! Hier noch auto-Close
}

// CCDDBConnect member functions
BOOL CCDDBConnect::CDDBOpen(CDArchiveConfig^ CDArchive) 
{
	CString ServerInfo = CDArchive->ArchiveName;

	if (ServerInfo.Find(':') > 0)
	{
		m_CDDBProtocol = connectSockets;
	}
	else
	{
		m_CDDBProtocol = connectHTTP;
	}

	if (m_CDDBProtocol == connectHTTP)
	{
		// use HTTP - do some initial work here :-)

		switch (Settings::Current->ProxyType)
		{
		case 0:
			m_pInternetSession = new CInternetSession;
			break;
		case 1:
			m_pInternetSession = new CInternetSession(NULL, 1, INTERNET_OPEN_TYPE_PROXY, (CString)Settings::Current->ProxyServerName);
			break;
		default:
			ASSERT(FALSE);
		}
		
		ASSERT(m_pInternetSession);

		if (!m_pInternetSession)
			return FALSE;

		// Server Namen ermitteln
		m_CDDBServerName = ServerInfo.Left(ServerInfo.Find('/'));
		m_CDDBInitString = ServerInfo.Mid(ServerInfo.Find('/'));
		m_CDDBServerName.TrimRight();
		m_CDDBServerName.TrimLeft();
		m_CDDBInitString.TrimRight();
		m_CDDBInitString.TrimLeft();
		m_pHttpConnection = 
			m_pInternetSession->GetHttpConnection(m_CDDBServerName);

		CWaitCursor wait;
		BOOL bRet = FALSE;

		ASSERT(m_pHttpConnection);

		if (!m_pHttpConnection)
		{
			m_pInternetSession->Close();
			delete m_pInternetSession;

			return FALSE;
		}
		return TRUE;
	}
	if (m_CDDBProtocol == connectSockets)
	{
		// use Sockets
		int nPortPos;
		CString buf;

		m_cddbSocket = new CCDDBSock;

		ASSERT (m_cddbSocket);

		if (!m_cddbSocket->Create ())
		{
			int lastError = GetLastError();
			return FALSE;
		}
		// Look for correct Portnumber...
		// i.e.: cddb.cddb.com:8880 - sometimes 888
		m_PortNumber = 0;
		if ((nPortPos = ServerInfo.Find(':')) != -1)
		{
			buf = ServerInfo.Mid(nPortPos+1);
			m_PortNumber = _wtol(buf);
		}
		if (m_PortNumber == 0)
		{
			return FALSE;
		}

		m_CDDBServerName = ServerInfo.Left(nPortPos);

		// First try to establish a connection to Server:Port
		if (!m_cddbSocket->Connect (m_CDDBServerName, m_PortNumber))
		{
			return FALSE;
		}

		// Connect to CDDB Server established
		return TRUE;
	}

	return FALSE;
}

// Senden InitString
// sockets: Senden des Strings
// HTTP: speichern - wird für jeden Aufruf gebraucht
BOOL CCDDBConnect::CDDBInit() 
{
	USES_CONVERSION;

	unsigned long num;
	CString username = "unknown";
	CString SendCmd;

	num = 200;

// Username jetzt immer "unknown". Freedb hat irgendwas umgestellt, jetzt funktionieren 
//	die Umlaute nicht mehr!!! Ggggrrrrrrrrrrrr....... cdarchiv.de rulez!
//	GetUserName(username.GetBuffer(200), &num);
//	username.ReleaseBuffer();

	// Alle Blanks fot!
	username = username.SpanExcluding(L" ");

	if (m_CDDBProtocol == connectSockets)
	{
		SendCmd.Format(L"cddb hello %s local.net Hitbase %s\n", username, HIT_VERSION);

		if (m_cddbSocket->Send (W2A(SendCmd), SendCmd.GetLength()) <= 0)
		{
			return FALSE;
		}
	}
	if (m_CDDBProtocol == connectHTTP)
	{
		m_CDDBHelloString.Format(L"hello=%s local.net Hitbase %s", username, HIT_VERSION);
		CharToHTTPTransmit (m_CDDBHelloString);
	}


/*
The "+" characters in the input represent spaces, and will be translated
by the server before performing the request. Special characters may be
represented by the sequence "%XX" where "XX" is a two-digit hex number
corresponding to the ASCII (ISO-8859-1) sequence of that character. The
"&" characters denote separations between the command, hello and proto
arguments. Newlines and carriage returns must not appear anywhere in the
string except at the end.
All CDDBP commands are supported under HTTP, except for "cddb hello",
"cddb write", "proto" and "quit".
*/
		// Sichern - wird für jeden Aufruf gebraucht...
		//m_CDDBInitString = InitString;
		// Beispiel HTTP String
		// http://www.cddb.com/cgi-bin/cddb.cgi?cmd=cddb+read+rock+6d0b4409&hello=gun+local.net+Hitbase+98&proto=2
	/*	// use HTTP protocol

		//SetStatusText("Informationen werden gelesen...");

		CString sInternetFileName = "/cgi-bin/cddb.cgi?cmd=cddb+read+rock+6d0b4409&hello=gun+local.net+Hitbase+98&proto=2";

		const TCHAR szHeaders[] =
			_T("Accept: text/*\r\nUser-Agent: Hitbase_98\r\n");

		DWORD dwHttpRequestFlags =
			INTERNET_FLAG_EXISTING_CONNECT | INTERNET_FLAG_NO_AUTO_REDIRECT | INTERNET_FLAG_DONT_CACHE;

		CHttpFile * pFile = pHttpConnection->OpenRequest(CHttpConnection::HTTP_VERB_GET,
			sInternetFileName, NULL, 1, NULL, NULL, dwHttpRequestFlags);

		CString H;
		if (pFile)
		{
			pFile->AddRequestHeaders(szHeaders);
			pFile->SendRequest();

			pFile->ReadString(H);

			pFile->Close();
			delete pFile;
		}

		//SetStatusText("");
		*/

	return TRUE;
}

// return vom aktuellen Protocol level
int CCDDBConnect::CDDBGetProtocolLevel() 
{
	return m_CDDBProtocolLevel;
}

// Setzen Protokol level
// sockets: Senden des Strings
// HTTP: speichern - wird für jeden Aufruf gebraucht
BOOL CCDDBConnect::CDDBSetProtocolLevel(int nProtocolLevel) 
{
	USES_CONVERSION;
	CString SendString;

	if (nProtocolLevel < 1 || nProtocolLevel > 4)
	{
		// ProtocolLevel currently 1-4
		return FALSE;
	}

	if (m_CDDBProtocol == connectSockets)
	{
		SendString.Format(L"proto %d\n", nProtocolLevel);
		if (m_cddbSocket->Send (W2A(SendString), SendString.GetLength()) <= 0)
		{
			return FALSE;
		}

		m_CDDBProtocolLevel = nProtocolLevel;
	}

	if (m_CDDBProtocol == connectHTTP)
	{
		m_CDDBProtocolLevel = nProtocolLevel;
	}

	return TRUE;
}

// Connect to Server
// WaitForTermChar auf TRUE setzen
// damit solange gelesen wird bis '.' kommt
BOOL CCDDBConnect::CDDBReceive(CString & ReceiveCDDBData, BOOL WaitForTermChar) 
{
	USES_CONVERSION;
	if (m_CDDBProtocol == connectSockets)
	{
		char sockbuf[513];
		int nBytesRead;

		ReceiveCDDBData.Empty();
		while (1)
		{
			memset(sockbuf, 0, sizeof(sockbuf));
			if ((nBytesRead = (int)m_cddbSocket->Receive( &sockbuf, sizeof (sockbuf)-1)) > 0)
			{
				ReceiveCDDBData = ReceiveCDDBData + sockbuf;
			}
			else
			{
				return FALSE;
			}

			// Optionale Ende Bedingung
			if (WaitForTermChar == TRUE)
			{
				if (ReceiveCDDBData.Find (L"\n.") >= 0)
					break;
			}
			else
				break;
		}
		
		return TRUE;
	}

	if (m_CDDBProtocol == connectHTTP)
	{
		char ReadBuf[10000];
		ReceiveCDDBData.Empty();

		if (m_HTTPTransmitPending && m_pFile)
		{
			while (m_pFile->ReadString((LPTSTR)ReadBuf, sizeof(ReadBuf)))
			{
				ReceiveCDDBData = ReceiveCDDBData + CString(A2W(ReadBuf)) + "\n";
			}

			m_pFile->Close();
			delete m_pFile;
			m_HTTPTransmitPending = FALSE;

			return TRUE;
		}
		return FALSE;
	}

	return FALSE;
}

BOOL CCDDBConnect::CDDBTransmit(CString TransmitCDDBData) 
{
	USES_CONVERSION;
	if (m_CDDBProtocol == connectSockets)
	{
		if (m_cddbSocket->Send (W2A(TransmitCDDBData), TransmitCDDBData.GetLength()) <= 0)
		{
			return FALSE;
		}
		return TRUE;
	}

	if (m_CDDBProtocol == connectHTTP)
	{
		CString sInternetFileName;

		// Nun String zusammenbauen
		CharToHTTPTransmit (TransmitCDDBData);
		sInternetFileName.Format(L"%s?cmd=%s&%s&proto=%d",m_CDDBInitString, TransmitCDDBData, m_CDDBHelloString, m_CDDBProtocolLevel);
		
		// Sonderzeichen und " ", "+", "&", "/", "?" in %XX konvertieren
		// CR/LF müssen raus
		const TCHAR szHeaders[] =
			_T("Accept: text/*\r\nUser-Agent: Hitbase 2010\r\n");

		DWORD dwHttpRequestFlags =
			INTERNET_FLAG_EXISTING_CONNECT | INTERNET_FLAG_NO_AUTO_REDIRECT | INTERNET_FLAG_DONT_CACHE;

		if (m_HTTPTransmitPending && m_pFile)
		{
			m_pFile->Close();
			delete m_pFile;
		}
		m_pFile = m_pHttpConnection->OpenRequest(CHttpConnection::HTTP_VERB_GET,
			sInternetFileName, NULL, 1, NULL, NULL, dwHttpRequestFlags);

		if (m_pFile)
		{
			m_pFile->AddRequestHeaders(szHeaders);
			try
			{
				m_pFile->SendRequest();
			}
			catch (CInternetException *e)
			{
				e->Delete();
				m_pFile->Close();
				delete m_pFile;

				return FALSE;
			}

			m_HTTPTransmitPending = TRUE;

			return TRUE;
		}

		return FALSE;
	}

	return FALSE;
}


BOOL CCDDBConnect::CDDBClose() 
{
	if (m_CDDBProtocol == connectSockets)
	{
		if (m_cddbSocket)
		{
			m_cddbSocket->Close();
			delete m_cddbSocket;
		}
	}

	if (m_CDDBProtocol == connectHTTP)
	{
		m_pHttpConnection->Close();
		delete m_pHttpConnection;

		m_pInternetSession->Close();
		delete m_pInternetSession;
	}
	
	m_CDDBProtocolLevel = 1;
	m_HTTPTransmitPending = FALSE;
	m_pFile = NULL;
	m_cddbSocket = NULL;
	m_pHttpConnection = NULL;
	m_pInternetSession = NULL;

	return TRUE;
}

// Entfernen/Umwandeln von Sonderzeichen +,&,/,?...
// Diese dürfen für http nicht gesetzt sein
void CCDDBConnect::CharToHTTPTransmit (CString & sInternetFileName)
{
	int nPos;
	CString RetBuf;
	CString HexValue;

	for (nPos = 0; nPos < sInternetFileName.GetLength(); nPos++)
	{
		if (sInternetFileName[nPos] == ' ')
		{
				RetBuf = RetBuf + '+';
				continue;
		}

		if ((unsigned)sInternetFileName[nPos] < 32)
		{
			continue;
		}
		if (sInternetFileName[nPos] == '&' ||
			sInternetFileName[nPos] == '+' ||
			sInternetFileName[nPos] == '/' ||
			sInternetFileName[nPos] == '?' ||
			(unsigned)sInternetFileName[nPos] > 126)
		{
			HexValue.Format (L"%%%02X", (unsigned char)sInternetFileName[nPos]);
			RetBuf = RetBuf + HexValue;
			continue;
		}

		RetBuf = RetBuf + sInternetFileName[nPos];
	} // For nPos...

	sInternetFileName = RetBuf;
}
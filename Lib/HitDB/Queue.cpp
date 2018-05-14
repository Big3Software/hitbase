// Queue.cpp : implementation file
//

#include "stdafx.h"
#include "hitdb.h"
#include "Queue.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CQueue

IMPLEMENT_DYNAMIC(CQueue, CDBQuery)

CQueue::CQueue(CDataBase* pdb)
	: CDBQuery(pdb)
{
	//{{AFX_FIELD_INIT(CQueue)
	m_dwIDCD = 0;
	m_dwAction = 0;
	m_nFields = 4;
	//}}AFX_FIELD_INIT
	m_nDefaultType = dbOpenDynaset;
}

CQueue::CQueue(const CQueue& theOther)
{
	*this = theOther;
}

CQueue& CQueue::operator= (const CQueue& theOther)
{
	m_dwIDCD = theOther.m_dwIDCD;
	m_dwAction = theOther.m_dwAction;
	m_sIdentity = theOther.m_sIdentity;
	m_sIdentityCDDB = theOther.m_sIdentityCDDB;

	return *this;
}

CQueue::~CQueue()
{
}

CString CQueue::GetDefaultSQL()
{
	return _T("[Queue]");
}

void CQueue::DoFieldExchange(CDaoFieldExchange* pFX)
{
	//{{AFX_FIELD_MAP(CQueue)
	pFX->SetFieldType(CDaoFieldExchange::outputColumn);
	DFX_Long(pFX, _T("[Q_lIDCD]"), m_dwIDCD);
	DFX_Long(pFX, _T("[Q_lAction]"), m_dwAction);
	DFX_Text(pFX, _T("[Q_sIdentity]"), m_sIdentity);
	DFX_Text(pFX, _T("[Q_sIdentityCDDB]"), m_sIdentityCDDB);
	//}}AFX_FIELD_MAP
}

/////////////////////////////////////////////////////////////////////////////
// CQueue diagnostics

#ifdef _DEBUG
void CQueue::AssertValid() const
{
	CDaoRecordset::AssertValid();
}

void CQueue::Dump(CDumpContext& dc) const
{
	CDaoRecordset::Dump(dc);
}
#endif //_DEBUG

// Fügt eine neue CD in die Queue ein.
BOOL CQueue::AddCD(long dwID, long dwAction, const CString& sIdentity, const CString& sCDDBQuery)
{
	// JUS 26.09.2002: Prüfen, ob der Eintrag schon vorhanden ist.
	bool bAlreadyInQueue = false;
	if (dwAction == QUEUE_ACTION_DOWNLOAD)
	{
		CString sSQL;
		sSQL.Format(L"Q_lAction = %d AND Q_sIdentity='%s' OR Q_sIdentityCDDB='%s'", QUEUE_ACTION_DOWNLOAD, sIdentity, sCDDBQuery);
		QueryStart(sSQL);
		if (!IsEOF())
			bAlreadyInQueue = true;

		QueryEnd();
	}

	if (dwAction == QUEUE_ACTION_UPLOAD)
	{
		CString sSQL;
		sSQL.Format(L"Q_lAction = %d AND Q_lIDCD=%d", QUEUE_ACTION_UPLOAD, dwID);
		QueryStart(sSQL);
		if (!IsEOF())
			bAlreadyInQueue = true;

		QueryEnd();
	}

	if (bAlreadyInQueue)
		return TRUE;

	if (!OpenTable())
		return FALSE;

	AddNew();
	m_dwIDCD = dwID;
	m_dwAction = dwAction;
	m_sIdentity = sIdentity;
	m_sIdentityCDDB = sCDDBQuery;
	Update();

	Close();

	return TRUE;
}

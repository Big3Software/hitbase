#if !defined(AFX_QUEUE_H__E52676B0_76A8_11D3_BF82_404E57434431__INCLUDED_)
#define AFX_QUEUE_H__E52676B0_76A8_11D3_BF82_404E57434431__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// Queue.h : header file
//

#define QUEUE_ACTION_UNKNOWN   0      // Undefiniert (Fehler)
#define QUEUE_ACTION_UPLOAD    1      // CD uploaden (ins Internet)
#define QUEUE_ACTION_DOWNLOAD  2      // CD downloaden (aus dem Internet)

/////////////////////////////////////////////////////////////////////////////
// CQueue DAO recordset

class HITDB_INTERFACE CQueue : public CDBQuery
{
public:
	virtual BOOL AddCD(long dwID, long dwAction, const CString& sIdentity, const CString& sCDDBQuery);
	CQueue(CDataBase* pDatabase = NULL);
	CQueue(const CQueue& theOther);
	CQueue& operator= (const CQueue& theOther);
	~CQueue();


	DECLARE_DYNAMIC(CQueue)

	// Tabellenname
	static const CString GetTableName() { return L"Queue"; }

// Field/Param Data
	//{{AFX_FIELD(CQueue, CDaoRecordset)
	long	m_dwIDCD;			// ID der CD
	long	m_dwAction;         // Auszuführende Aktion (siehe QUEUE_xxx)
	CString m_sIdentity;        // Identity der CD
	CString m_sIdentityCDDB;    // CDDB-Identity der CD
	//}}AFX_FIELD

// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CQueue)
	public:
	virtual CString GetDefaultSQL();		// Default SQL for Recordset
	virtual void DoFieldExchange(CDaoFieldExchange* pFX);  // RFX support
	//}}AFX_VIRTUAL

// Implementation
#ifdef _DEBUG
	virtual void AssertValid() const;
	virtual void Dump(CDumpContext& dc) const;
#endif
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_QUEUE_H__E52676B0_76A8_11D3_BF82_404E57434431__INCLUDED_)

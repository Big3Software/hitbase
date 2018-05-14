// DBQuery.h: interface for the CDBQuery class.
//
//////////////////////////////////////////////////////////////////////

#pragma once

class CDataBase;
class CDataBaseProgressDlg;

class HITDB_INTERFACE CDBQuery : public CDaoRecordset
{
public:
	CDBQuery(CDataBase* pDataBase = NULL);

	DECLARE_DYNAMIC(CDBQuery)

public:
	virtual CDataBase* GetDataBase();
	virtual BOOL OpenTable();
	virtual BOOL DeleteFromID(long dwID);
	virtual BOOL DeleteAll();
	virtual BOOL WriteRecord();
	virtual BOOL GetRecordFromID(long dwID);
	virtual long GetCount();
	virtual BOOL QueryStart(const CString &sWhere = L"", const CString& sOrder = L"");
	virtual BOOL QueryStartFreeSQL(const CString &sSQL);
	virtual BOOL QueryEnd();
	virtual BOOL QueryFindFirst();
	virtual BOOL QueryFindNext();
	virtual void SetDataBase(CDataBase* pDataBase);

	virtual void SetProgressCtrl(CProgressCtrl* pProgressCtrl, BOOL bBackground = TRUE);
	virtual void SetProgressDlg(CDataBaseProgressDlg* pProgressDlg, BOOL bBackground = TRUE);
	virtual CProgressCtrl* GetProgressCtrl();
	virtual void StopBackgroundSearch();

	virtual long GetTotalCount();

protected:
	CDataBase* m_pdb;

	int m_iProgressedRecords;

	CProgressCtrl* m_pProgressCtrl;
	BOOL m_bStopBackgroundSearch;
	BOOL m_bBackgroundSearch;

private:
	BOOL BackgroundProcessing();
};

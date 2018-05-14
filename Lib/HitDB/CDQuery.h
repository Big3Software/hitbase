// CDQuery.h: Schnittstelle für die Klasse CCDQuery.
//
//////////////////////////////////////////////////////////////////////

#pragma once

#include "cd.h"
#include "track.h"
#include "artist.h"

class CSelection;

class HITDB_INTERFACE CCDQuery : public CCDData
{
public:
	CCDQuery(CDataBase* pDataBase);
	virtual ~CCDQuery();

	enum eQueryType { queryAll, queryCDOnly, queryTrack };

	BOOL QueryStart(CSelection* pSelection, CProgressCtrl* pProgressCtrl = NULL, BOOL bBackground = TRUE, BOOL bQueryTrack = FALSE);
	BOOL QueryStartFreeWhere(const CString& sWhere, CSelection* pSelection, CProgressCtrl* pProgressCtrl = NULL, BOOL bBackground = TRUE, BOOL bQueryTrack = FALSE);
	BOOL QueryStartCDOnly(CSelection* pSelection, CProgressCtrl* pProgressCtrl = NULL, BOOL bBackground = TRUE);
	BOOL QueryStartTrack(CSelection* pSelection, CProgressCtrl* pProgressCtrl = NULL, BOOL bBackground = TRUE);
	BOOL QueryStartCDFreeWhere(const CString& sWhere, CSelection* pSelection, CProgressCtrl* pProgressCtrl = NULL, BOOL bBackground = TRUE);
	BOOL GetRecordFromID(long dwID);
	BOOL QueryFindNext();
	BOOL QueryEnd();
	long GetCount();
	void StopSearch();
	CDataBase* GetDataBase() { return m_pDataBase; }

public:
	CArray<CTrackData, CTrackData&> m_Track;
	CArray<CArtistData, CArtistData&> m_ArtistTrack;
	CArray<CArtistData, CArtistData&> m_ComposerTrack;

	CArtistData m_Artist;
	CArtistData m_Composer;

	// Folgende Felder kommen aus anderen Tabellen, werden aber aus Performancegründen direkt
	// hierein gemapped.
	CString m_sCategory;
	CString m_sMedium;
	CString m_sCDSetName;

private:
	int ReadRecordset();
	CString GetFieldString(const CString& sFieldName);
	short GetFieldShort(const CString& sFieldName, BOOL bNoPseudoNull = FALSE);
	long GetFieldLong(const CString& sFieldName, BOOL bNoPseudoNull = FALSE);
	BOOL GetFieldBool(const CString& sFieldName);
	COleDateTime GetFieldDateTime(const CString& sFieldName);

	BOOL m_bSearchStopped;
	CDataBase* m_pDataBase;
	CDBQuery* m_pDBQuery;

	eQueryType m_QueryType;

public:
	COleVariant GetField(const CString& sFieldName);
	CString GetFieldValue(int iField);
	CString GetTrackFieldValue(int iTrack, int iField);
	CString GetArtistText();
	CString GetTitleText();
};

// hitdb.h : header file
//

#pragma once

/////////////////////////////////////////////////////////////////////////////
// CDataBase class

class CDBQuery;
class CCD;
class CGridListCtrl;

#include "Selection.h"
#include "Master.h"

class HITDB_INTERFACE CDataBase
{
public:
    // Construction
	CDataBase();
	virtual ~CDataBase();

	enum enumFieldType { fieldTypeBoolean, fieldTypeByte, fieldTypeShort, fieldTypeLong, fieldTypeString, fieldTypeDateTime };

	CString GetCategoryFromID(int iID);
	CString GetMediumFromID(int iID);
	BOOL DeleteAllCDsFromArtist(long dwID);
	CString GetWhereCodesString(const CString& Codes, BOOL bTrack);
	COleVariant Abfrage(const CString& SQLString, BOOL* bFound = NULL);
	BOOL DeleteAllCategories();
	int m_ProgressedRecords;
	BOOL SwapMedium(int nMedium);
	BOOL SwapCategories(int nCategory);
	BOOL AddMedium(LPCTSTR pszMedium);
	BOOL EditMedium(LPCTSTR pszOldMedium, LPCTSTR pszNewMedium);
	BOOL DeleteMedium(LPCTSTR pszMedium, LPCTSTR pszNewMedium = NULL);
	BOOL WriteDBProperties();
	BOOL ReadDBProperties();
	virtual BOOL ReadDatabaseVersion();
	BOOL AddCategory(LPCTSTR pszCategory);
	BOOL DeleteCategory(LPCTSTR pszCategory, LPCTSTR pszNewCategory, int nCategory);
	BOOL UpdateCategory(LPCTSTR pszOldCategory, LPCTSTR pszNewCategory);
	BOOL ReadAllMedia();
	int AddCDSetsToComboBox(CComboBox *cb);
	CString GetWhereString(const CSelection *Selection, BOOL bSimple = FALSE, BOOL bNoFirstAnd = FALSE);
	long GetIDFromCDSet(CString szCDSetName, BOOL* bAdded = NULL);
	BOOL FindArtist(CString szArtistPrefix, CString &ArtistName);
	BOOL FindCDTitle(CString sPrefix, CString &sCDTitle);
	BOOL FindTrackname(CString sPrefix, CString &sTrackname);
	long GetIDFromArtist(const CString& szArtist, BOOL* bAdded = NULL, BOOL bDoNotAdd = FALSE);
	BOOL WriteAllCodes();
	BOOL ReadAllCodes();
	BOOL ReadAllCategories();
	BOOL IsDataBaseOpened();
	void StopSearchProcess();
	void SetProgressUpdate(BOOL update);
	
	BOOL Open(const CString szDBFileName=L"", BOOL bReadOnly = FALSE, BOOL bDisplayMessages = FALSE);
	BOOL Close();
	static BOOL Create(const CString & pszFileName, const CString & sTemplate);
	
	int Select(CSelection *Selection);
	
	int SetProgressCtrl(CProgressCtrl *ProgressCtrl, BOOL bBackground = FALSE);
	CProgressCtrl *GetProgressCtrl();
	
	BOOL ChooseFields(CFieldList& FieldList, CWnd* pParent = NULL);
	CString GetDatabaseFieldName(UINT uiField);
	enumFieldType GetDatabaseFieldType(UINT uiField);
	CString GetFilterWhereString(const CSelection& sel);

	void FillUserFieldListCtrl(CGridListCtrl* pListCtrl, CCD* pCD, int iTrack);

public:
	CMaster m_MasterTable;

	CString m_Codes[MAX_CODES];
	
	long m_IDCategories[MAX_CATEGORIES];
	CString m_Categories[MAX_CATEGORIES];
	int m_nNumberOfCategories;
	
	int m_nNumberOfMedia;
	CString m_Medium[MAX_MEDIUM];
	long m_IDMedium[MAX_MEDIUM];
	
	void AddCodesToComboBox(CComboBox *cb, BOOL bShowEmpty = TRUE);
	CString GetDate();
	CString DateShort2Long(CString sDate);
	CString DateLong2Short(CString lDate);
	int DateCheckFormat(CString date);
	CString GetDateFormatString(void);
	CString GetFieldName(UINT uiField, int& iDefaultSize, BOOL bCDAndTrack = FALSE, BOOL bUseDefaultUserFieldName = FALSE);
	void SetDateText(CWnd *pWnd);
	void AddCategoriesToComboBox(CComboBox *cb, BOOL bResetContents=TRUE);
	int AddMediaToComboBox(CComboBox *cb, BOOL bResetContents = TRUE);
	long GetIDFromCategory(LPCTSTR pszCategory);
	long GetIDFromMedium(LPCTSTR pszMedium);
	BOOL ExecuteSQL(CString sqlStr);
	
private:
	long GetMSDeluxeTrackLength(CString& sTrackLength, int nTrackNr);
	int m_nFlags;
	BOOL m_bCustomSqlString;
	void CheckAbortProc();
	BOOL m_bReadOnly;
    
public:
	CDaoDatabase m_db;
	
	CSelection *m_CurrentSelection;
	BOOL m_bBackground;
	
	CProgressCtrl *m_ProgressCtrl;
	volatile BOOL m_StopSearchProcess;
	
public:
	BOOL ImportCDArchivSnapshot(const CString& sFilename, CProgressCtrl& ctlProgress, CStatic& stcStatus);
	BOOL CreateMedium(const CString& sMedium, BOOL bAsk = TRUE);
	BOOL CreateCategory(const CString& sCategory, BOOL bAsk = TRUE);
	virtual int GetVersion();
	virtual void SearchFirstPrefix(CEdit *pEditCtrl, int iType);
	virtual CString GetWhereStringTrack(const CSelection &sel, BOOL bSimple = FALSE, BOOL bNoFirstAnd = FALSE, BOOL bNoArtist = FALSE);
	virtual CString GetWhereStringCD(const CSelection &sel, BOOL bSimple = FALSE, BOOL bNoFirstAnd = FALSE, BOOL bNoArtist = FALSE);
	virtual CString GetWhereStringArtist(const CSelection& sel, BOOL bSimple = FALSE, BOOL bNoFirstAnd = FALSE);
	CString GetFilterWhereStringTable(const CSelection& sel, CSelection::enumFilterTable filterTable);
	BOOL RollbackTransaction();
	BOOL CommitTransaction();
	BOOL BeginTransaction();
	int AbfrageInt(const CString& sSQLString, BOOL* bFound=NULL);
	CString GetDataBasePath();
	void SetDataBasePath(const CString& sFilename);

	BOOL m_bTabelleVerliehenVorh;
	BOOL WriteIDList(DWORD dwRecNr, const CStringArray& IDList);
	BOOL GetIDList(DWORD dwRecNr, CStringArray& IDList);
	BOOL IsReadOnly();
	BOOL ReadAllTables();
	DWORD CheckArchiveNumber(const CString& ArchiveNumber);
	
	BOOL WriteIdentity(const CString& strIdentity, DWORD dwRecNr);

	int convert_xmcd (const CString& src, CProgressCtrl& ctlProgress, CStatic& stcStatus);
	int convert_mscd (const CString& src, CProgressCtrl& ctlProgress, CStatic& stcStatus);
	int convert_mscddeluxe (CString sFileName, CProgressCtrl& ctlProgress, CStatic& stcStatus);
    void GetDiscIDXmcd (CStringArray& DiscIDs, const CString& pXmcdDiscID);

private:
	int m_DataBaseOpened;
	char *buff;
	char *buff1;
	int  UserKeyCreated;
	BOOL m_ProgressUpdateStopped;
    CDataBaseProgressDlg *m_DataBaseProgressDlg;
	
	// für convertierung von Hit3/4 nach neu
	int m_ConvertUser;
	int m_NotConverted;
	int m_PossibleError;
	
protected:
	CString m_sDBFilename;

	int m_nQueryType;
	int m_iDatabaseVersion;         // Version der Hitbase-Datenbank (z.B. 801)
public:
	int TransferFields(int iSourceField, int iTargetField, bool bMoveFields, CProgressCtrl* pProgressCtrl, CStatic* pstcText, CSelection* pSelection);
	void FillFieldsInListbox(CFieldList* pFieldList, CListBox* pListBox);
	CString GetNextFreeArchiveNumber(void);
	CString GetCoverFolder(void);

	bool Compress();
};

/////////////////////////////////////////////////////////////////////////////


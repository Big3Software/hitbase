// hitsel.h
//
// Selektions-Struktur für Select-Abfragen in der Datenbank

#ifndef _CLASS_CSELECTION 
#define _CLASS_CSELECTION

#include "../Hitmisc/FieldList.h"

using namespace Big3::Hitbase::DataBaseEngine;

class CFilterElement
{
public:
	enum enumOperator { opGreater, opGreaterEqual, opEqual, opLessEqual, opLess, opNotEqual };
	enum enumLogical { opAnd, opOr, opNone };

	bool m_bNot;
	int m_iField;
	enumOperator m_op;
	CString m_sValue;
	enumLogical m_log;
};

class CFilterElementArray : public CArray<CFilterElement, CFilterElement&>
{
public:
	CFilterElementArray() {};
	CFilterElementArray(const CFilterElementArray& theOther)
	{
		*this = theOther;
	}

	virtual CFilterElementArray& operator =(const CFilterElementArray& theOther)
	{
		Copy(theOther);

		return *this;
	}

	// Liefert TRUE zurück, wenn in der Feldliste die Kategorie vorhanden ist!
	BOOL CategoryFieldSelected()
	{
		for (int i=0;i<GetSize();i++)
		{
			if ((*this)[i].m_iField == FIELD_CATEGORY)
				return TRUE;
		}

		return FALSE;
	}

};

class HITDB_INTERFACE CSelection
{
public:
	enum enumFilterTable { filterTableAll, filterTableCD, filterTableTrack, filterTableArtist, filterTableCDArtist };

	void SetSortKey(int iSortKey, BOOL bAscending);
	CString GetSortString(BOOL bTrackAndCD);
	CString GetSelectionString();    // Liefert den Selektions-String zurück
	CString GetFilterString(enumFilterTable filterTable) const;
	CString GetFilterText(enumFilterTable filterTable) const;
	CString GetOrderString(BOOL bCDOnly = FALSE) const;
	BOOL IsEmptyCD();
	BOOL IsEmptyTrack();
	BOOL IsEmpty();
	BOOL IsSortedByCDField();
	bool IsFilterEmpty(enumFilterTable filterTable = filterTableAll) const;
	void Restore(const CString & sKey);
	void Save(const CString & sSaveKey);
	void Reset();
	void ResetCD();
	void ResetCDArtist();
	void ResetTrack();
	void ResetTrackArtist();
	CSelection();
	virtual ~CSelection();
	int DoSortDlg(DWORD dwFlags);
//	operator = (const CSelection& theOther);
	
	gcroot<Big3::Hitbase::DataBaseEngine::DataBase^> DataBase;

public:
	long m_dwTotalLengthFrom, m_dwTotalLengthTo;
	short m_wNumberOfTracksFrom, m_wNumberOfTracksTo;
	short m_wCDSetNumberFrom, m_wCDSetNumberTo;
	CString m_szCDSetName;
	short m_wCDType;
	CString m_szCDArtist;                          
	CString m_szCDTitle;                         
	CString m_szCategory;                         
	CString m_szMedium;
	CString m_sArchivNummerFrom;
	CString m_szDateFrom, m_szDateTo;             
	CString m_szCodes;                 
	CString m_szCDComment;            
	CString m_szCDUser[MAX_USER_FIELDS];
	CString m_sCopyright;
	long m_lYearRecordedFrom;
	long m_lYearRecordedTo;

	// Version 10
	int m_iOriginalCD;
	CString m_sArchivNummerTo;
	CString m_sLabel;
	CString m_sURL;
	CString m_sUPC;
	long m_lRatingFrom;
	long m_lRatingTo;
	long m_lPriceFrom;
	long m_lPriceTo;
	CString m_sFrontCover;
	CString m_sBackCover;
	CString m_sCDLabel;

	// Version 11
	CString m_sComposer;
	CString m_sLanguage;
	CString m_sLocation;
	CString m_sParticipant;

	CString m_szTrackArtist;          
	CString m_szTrackTitle;            
	long m_dwTrackLengthFrom, m_dwTrackLengthTo; 
	short m_wTrackBpmFrom, m_wTrackBpmTo;      
	CString m_szTrackCodes;                 
	CString m_szTrackComment;              
	CString m_szTrackLyrics;
	CString m_szTrackUser[MAX_USER_FIELDS];
	CString m_sTrackSoundFilename;   
	long m_lTrackYearRecordedFrom;
	long m_lTrackYearRecordedTo;

	// Version 10
	long m_lTrackRatingFrom;
	long m_lTrackRatingTo;

	// Version 11
	CString m_sTrackComposer;
	CString m_szTrackLanguage;
	CString m_szTrackCategory;

	BOOL m_bMatchCase;

	BOOL m_bExactMatch;

	CFieldList m_SortKeys;
	BOOL m_bSortKeysDirection[3];

	// Neuer "erweiterter" Filter
	CFilterElementArray m_Filter;
};


#define SSD_CD     1              // Nach CD-Feldern sortieren
#define SSD_TRACK  2              // Nach Lied-Feldern sortieren

/////////////////////////////////////////////////////////////////////////////
// CSearchSortDlg dialog

class HITDB_INTERFACE CSearchSortDlg : public CDialog
{
// Construction
public:
	CSearchSortDlg(DWORD dwFlags = 0, CWnd* pParent = NULL);   // standard constructor

// Dialog Data
	//{{AFX_DATA(CSearchSortDlg)
	enum { IDD = IDD_SORT };
	CComboBox	m_SortKey3Ctrl;
	CComboBox	m_SortKey2Ctrl;
	CComboBox	m_SortKey1Ctrl;
	int		m_SortKey1;
	int		m_SortKey2;
	int		m_SortKey3;
	//}}AFX_DATA

	BOOL m_bSortDirection1Desc;
	BOOL m_bSortDirection2Desc;
	BOOL m_bSortDirection3Desc;

	gcroot<Big3::Hitbase::DataBaseEngine::DataBase^> DataBase;

	// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CSearchSortDlg)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:

	// Generated message map functions
	//{{AFX_MSG(CSearchSortDlg)
	virtual BOOL OnInitDialog();
	virtual void OnOK();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()

private:
	void SelectItemData(CComboBox& cboSort, int iData);
    void AddStringToSortList(const CString& str, int iValue);
    DWORD m_dwFlags;
};

#endif

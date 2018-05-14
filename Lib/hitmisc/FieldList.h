// FieldList.h: interface for the CFieldList class.
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_FIELDLIST_H__18D112BC_0386_4885_A28E_854051546A94__INCLUDED_)
#define AFX_FIELDLIST_H__18D112BC_0386_4885_A28E_854051546A94__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#define FLF_CD				1					// Nur CD-Felder
#define FLF_TRACK			2					// Nur Lied-Felder
#define FLF_ALL				FLF_CD|FLF_TRACK	// Alle Felder 
#define FLF_CD_NO_ARTIST	4					// Bei den CD-Feldern keine Interpreten-Daten
#define FLF_TRACK_NO_ARTIST 8					// Bei den Lied-Feldern keine Interpreten-Daten

class CDataBase;

class HITMISC_INTERFACE CFieldList : public CUIntArray
{
public:
	CFieldList();
	CFieldList(const CFieldList& theOther);
	CFieldList(const CUIntArray& theOther);
	virtual ~CFieldList();

	virtual CFieldList& operator = (const CFieldList& theOther);
	virtual BOOL operator != (const CFieldList& theOther) const;

public:
	virtual void Add(UINT newElement);
	virtual void Add(UINT newElement, UINT uiSize);
	virtual BOOL CategoryFieldSelected();
	virtual BOOL CDFieldsSelected();
	virtual int GetColumnSize(int iCol, int iDefaultSize);
	virtual void SetColumnSize(int iCol, int iSize);
	virtual void GetColumnSizeFromListCtrl(CListCtrl& ListCtrl);
	virtual void Load(const CString& sRegistryKey, HKEY hKey = NULL);
	virtual void Save(const CString& sRegistryKey, HKEY hKey = NULL);

	virtual int Find(UINT uiField);

	virtual CFieldList GetAllFields();
	static CFieldList GetAllFields(DWORD dwType);
	virtual DWORD GetType();
	virtual void SetType(DWORD dwType);
	virtual void SetStandardFields(const CFieldList& StandardFields);
	virtual CFieldList GetStandardFields();
	static bool IsCDField(UINT uiField);
	virtual void Move(int iIndex, int iOffset);

private:
	DWORD m_dwType;

	CUIntArray m_StandardFields;
	CUIntArray m_StandardFieldsSize;
	CUIntArray m_aSize;
};

#endif // !defined(AFX_FIELDLIST_H__18D112BC_0386_4885_A28E_854051546A94__INCLUDED_)

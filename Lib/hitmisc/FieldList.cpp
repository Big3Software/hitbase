// FieldList.cpp: implementation of the CFieldList class.
//
// Klasse zur Speicherung und Verwaltung von Spalten-Feldern. 
// Für CD- oder Lieder-Listen, in denen die Spalten konfigurierbar sind!
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "hitmisc.h"
#include "config.h"
#include "tokenex.h"
#include "FieldList.h"
#include "HitbaseWinAppBase.h"
#include ".\fieldlist.h"

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

CFieldList::CFieldList()
{
	m_dwType = 0;
}

CFieldList::CFieldList(const CFieldList& theOther)
{
	*this = theOther;
}

CFieldList::CFieldList(const CUIntArray& theOther)
{
	for (int i=0;i<theOther.GetSize();i++)
		Add(theOther[i]);

	m_dwType = 0;
}

CFieldList::~CFieldList()
{

}

CFieldList& CFieldList::operator = (const CFieldList& theOther)
{
	RemoveAll();

	for (int i=0;i<theOther.GetSize();i++)
		Add(theOther[i]);

	m_aSize.RemoveAll();
	for (int i=0;i<theOther.m_aSize.GetSize();i++)
		m_aSize.Add(theOther.m_aSize[i]);

	m_dwType = theOther.m_dwType;

	return *this;
}

BOOL CFieldList::operator != (const CFieldList& theOther) const
{
	if (theOther.GetSize() != GetSize())
		return TRUE;

	for (int i=0;i<theOther.GetSize();i++)
		if (theOther[i] != (*this)[i])
			return TRUE;

	if (theOther.m_aSize.GetSize() != m_aSize.GetSize())
		return TRUE;

	for (int i=0;i<theOther.m_aSize.GetSize();i++)
		if (theOther.m_aSize[i] != m_aSize[i])
			return TRUE;

	return FALSE;
}

void CFieldList::Add(UINT newElement)
{
	CUIntArray::Add(newElement);
}

void CFieldList::Add(UINT newElement, UINT uiSize)
{
	CUIntArray::Add(newElement);
	m_aSize.Add(uiSize);
}

void CFieldList::SetType(DWORD dwType)
{
	m_dwType = dwType;
}

DWORD CFieldList::GetType()
{
	return m_dwType;
}

void CFieldList::SetStandardFields(const CFieldList& StandardFields)
{
	m_StandardFields.RemoveAll();

	for (int i=0;i<StandardFields.GetSize();i++)
		m_StandardFields.Add(StandardFields[i]);

	m_StandardFieldsSize.RemoveAll();
	for (int i=0;i<StandardFields.m_aSize.GetSize();i++)
		m_StandardFieldsSize.Add(StandardFields.m_aSize[i]);
}

CFieldList CFieldList::GetStandardFields()
{
	return m_StandardFields;
}

CFieldList CFieldList::GetAllFields()
{
	CFieldList FullList;

	FullList.m_dwType = m_dwType;

	if (m_dwType & FLF_CD)
	{
		FullList.Add(FIELD_TOTALLENGTH);
		FullList.Add(FIELD_NUMBEROFTRACKS);
		FullList.Add(FIELD_CDSET);
		FullList.Add(FIELD_CDSAMPLER);
		FullList.Add(FIELD_CDNAME);
		FullList.Add(FIELD_CDTITLE);
		FullList.Add(FIELD_COMPOSER_CD_NAME);
		FullList.Add(FIELD_CATEGORY);
		FullList.Add(FIELD_DATE);
		FullList.Add(FIELD_CODES);
		FullList.Add(FIELD_CDCOMMENT);
		FullList.Add(FIELD_ARCHIVNUMMER);
		FullList.Add(FIELD_MEDIUM);
		FullList.Add(FIELD_CDUSER1);
		FullList.Add(FIELD_CDUSER2);
		FullList.Add(FIELD_CDUSER3);
		FullList.Add(FIELD_CDUSER4);
		FullList.Add(FIELD_CDUSER5);
		FullList.Add(FIELD_YEAR_RECORDED);
		FullList.Add(FIELD_COPYRIGHT);
		FullList.Add(FIELD_CDCOVER_FILENAME);

		FullList.Add(FIELD_CDCOVERBACK_FILENAME);
		FullList.Add(FIELD_CDCOVERLABEL_FILENAME);
		FullList.Add(FIELD_ORIGINAL_CD);
		FullList.Add(FIELD_LABEL);
		FullList.Add(FIELD_UPC);
		FullList.Add(FIELD_URL);
		FullList.Add(FIELD_RATING);
		FullList.Add(FIELD_PRICE);

		FullList.Add(FIELD_LANGUAGE);
		FullList.Add(FIELD_LOCATION);

		if (!(m_dwType & FLF_CD_NO_ARTIST))
		{
			FullList.Add(FIELD_ARTIST_CD_SORTKEY);
			FullList.Add(FIELD_ARTIST_CD_GROUPTYPE);
			FullList.Add(FIELD_ARTIST_CD_SEX);
			FullList.Add(FIELD_ARTIST_CD_COMMENT);
			FullList.Add(FIELD_ARTIST_CD_URL);
			FullList.Add(FIELD_ARTIST_CD_COUNTRY);
			FullList.Add(FIELD_ARTIST_CD_BIRTHDAY);
			FullList.Add(FIELD_ARTIST_CD_DAYOFDEATH);
			FullList.Add(FIELD_ARTIST_CD_IMAGEFILENAME);
		}

		if (!(m_dwType & FLF_CD_NO_ARTIST))
		{
			FullList.Add(FIELD_COMPOSER_CD_SORTKEY);
			FullList.Add(FIELD_COMPOSER_CD_GROUPTYPE);
			FullList.Add(FIELD_COMPOSER_CD_SEX);
			FullList.Add(FIELD_COMPOSER_CD_COMMENT);
			FullList.Add(FIELD_COMPOSER_CD_URL);
			FullList.Add(FIELD_COMPOSER_CD_COUNTRY);
			FullList.Add(FIELD_COMPOSER_CD_BIRTHDAY);
			FullList.Add(FIELD_COMPOSER_CD_DAYOFDEATH);
			FullList.Add(FIELD_COMPOSER_CD_IMAGEFILENAME);
		}
	}

	if (m_dwType & FLF_TRACK)
	{
		FullList.Add(FIELD_TRACK_NUMBER);
		FullList.Add(FIELD_TRACK_ARTIST);
		FullList.Add(FIELD_COMPOSER_TRACK_NAME);
		FullList.Add(FIELD_TRACK_TITLE);
		FullList.Add(FIELD_TRACK_LENGTH);
		FullList.Add(FIELD_TRACK_BPM);
		FullList.Add(FIELD_TRACK_CODES);
		FullList.Add(FIELD_TRACK_COMMENT);
		FullList.Add(FIELD_TRACK_USER1);
		FullList.Add(FIELD_TRACK_USER2);
		FullList.Add(FIELD_TRACK_USER3);
		FullList.Add(FIELD_TRACK_USER4);
		FullList.Add(FIELD_TRACK_USER5);
		FullList.Add(FIELD_TRACK_LYRICS);
		FullList.Add(FIELD_TRACK_SOUNDFILE);
		FullList.Add(FIELD_TRACK_YEAR_RECORDED);
		FullList.Add(FIELD_TRACK_RATING);
		FullList.Add(FIELD_TRACK_CATEGORY);
		FullList.Add(FIELD_TRACK_LANGUAGE);

		if (!(m_dwType & FLF_TRACK_NO_ARTIST))
		{
			FullList.Add(FIELD_ARTIST_TRACK_SORTKEY);
			FullList.Add(FIELD_ARTIST_TRACK_GROUPTYPE);
			FullList.Add(FIELD_ARTIST_TRACK_SEX);
			FullList.Add(FIELD_ARTIST_TRACK_COMMENT);
			FullList.Add(FIELD_ARTIST_TRACK_URL);
			FullList.Add(FIELD_ARTIST_TRACK_COUNTRY);
			FullList.Add(FIELD_ARTIST_TRACK_BIRTHDAY);
			FullList.Add(FIELD_ARTIST_TRACK_DAYOFDEATH);
			FullList.Add(FIELD_ARTIST_TRACK_IMAGEFILENAME);
		}

		if (!(m_dwType & FLF_TRACK_NO_ARTIST))
		{
			FullList.Add(FIELD_COMPOSER_TRACK_SORTKEY);
			FullList.Add(FIELD_COMPOSER_TRACK_GROUPTYPE);
			FullList.Add(FIELD_COMPOSER_TRACK_SEX);
			FullList.Add(FIELD_COMPOSER_TRACK_COMMENT);
			FullList.Add(FIELD_COMPOSER_TRACK_URL);
			FullList.Add(FIELD_COMPOSER_TRACK_COUNTRY);
			FullList.Add(FIELD_COMPOSER_TRACK_BIRTHDAY);
			FullList.Add(FIELD_COMPOSER_TRACK_DAYOFDEATH);
			FullList.Add(FIELD_COMPOSER_TRACK_IMAGEFILENAME);
		}
	}

	return FullList;
}

/*static*/CFieldList CFieldList::GetAllFields(DWORD dwType)
{
	CFieldList fl;
	fl.SetType(dwType);
	return fl.GetAllFields();
}

void CFieldList::Load(const CString& sRegistryKey, HKEY hKey /* = NULL */)
{
	CString str;

	if (hKey)
		str = CConfig::RegQueryString(hKey, sRegistryKey, "");
	else
		str = CConfig::ReadGlobalRegistryKeyString(sRegistryKey, "");

	if (str.IsEmpty())
	{				// Wenn leer, dann Standardwerte
		DWORD dwType = m_dwType;     // Type merken, da er auf 0 gesetzt wird.
		*this = m_StandardFields;
		m_dwType = dwType;
		return;
	}

	CTokenEx tok;
	CStringArray saTokens;
	tok.Split(str, ",", saTokens);

	RemoveAll();

	for (int i=0;i<saTokens.GetSize();i++)
		Add(_wtoi(saTokens[i]));

	if (hKey)
		str = CConfig::RegQueryString(hKey, sRegistryKey+"Size", "");
	else
		str = CConfig::ReadGlobalRegistryKeyString(sRegistryKey+"Size", "");

	if (str.IsEmpty())
	{
		// Standardwerte nehmen
		DWORD dwType = m_dwType;     // Type merken, da er auf 0 gesetzt wird.
		
		m_aSize.RemoveAll();
		for (int i=0;i < m_StandardFieldsSize.GetCount();i++)
			m_aSize.Add(m_StandardFieldsSize[i]);
		m_dwType = dwType;
		
		return;
	}

	saTokens.RemoveAll();
	tok.Split(str, ",", saTokens);

	m_aSize.RemoveAll();

	for (int i=0;i<saTokens.GetSize();i++)
		m_aSize.Add(_wtoi(saTokens[i]));
}

void CFieldList::Save(const CString& sRegistryKey, HKEY hKey /* = NULL */)
{
	CString str, str1;

	for (int i=0;i<GetSize();i++)
	{
		str1.Format(L"%d,", (*this)[i]);
		str += str1;
	}

	if (!str.IsEmpty())
		str = str.Left(str.GetLength()-1);     // Das letzte "," löschen!

	if (hKey)
		CConfig::RegWriteString(hKey, sRegistryKey, str);
	else
		CConfig::WriteGlobalRegistryKeyString(sRegistryKey, str);

	// Jetzt noch die Größen speichern!
	str = "";
	for (int i=0;i<m_aSize.GetSize();i++)
	{
		str1.Format(L"%d,", m_aSize[i]);
		str += str1;
	}

	if (!str.IsEmpty())
		str = str.Left(str.GetLength()-1);     // Das letzte "," löschen!

	if (hKey)
		CConfig::RegWriteString(hKey, sRegistryKey+"Size", str);
	else
		CConfig::WriteGlobalRegistryKeyString(sRegistryKey+"Size", str);
}

void CFieldList::GetColumnSizeFromListCtrl(CListCtrl &ListCtrl)
{
	m_aSize.RemoveAll();

	// Erst mal schauen, wie viele Spalten überhaupt da sind!
	LVCOLUMN col;
	memset(&col, 0, sizeof(col));
	col.mask = LVCF_WIDTH;

	int iColCount;
	for (iColCount=0;ListCtrl.GetColumn(iColCount, &col);iColCount++);

	// Jetzt die Spaltenbreiten einlesen
	for (int i=0;i<GetSize() && i < iColCount;i++)
	{
		m_aSize.Add(ListCtrl.GetColumnWidth(i));
	}
}

// Liefert die Spaltenbreite des angegebenen Feldes zurück.
int CFieldList::GetColumnSize(int iCol, int iDefaultSize)
{
	if (m_aSize.GetSize() <= iCol || m_aSize[iCol] == 0)
		return iDefaultSize;
	else
		return m_aSize[iCol];
}

// Setzt die Spaltenbreite des angegebenen Feldes.
void CFieldList::SetColumnSize(int iCol, int iSize)
{
	m_aSize.SetAtGrow(iCol, iSize);
}

// Liefert TRUE zurück, wenn in der Feldliste mindestens ein CD-Feld vorhanden ist!
BOOL CFieldList::CDFieldsSelected()
{
	for (int i=0;i<GetSize();i++)
	{
		if ((*this)[i] < FIELD_TRACK_ARTIST)
			return TRUE;
	}

	return FALSE;
}

// Liefert TRUE zurück, wenn in der Feldliste die Kategorie vorhanden ist!
BOOL CFieldList::CategoryFieldSelected()
{
	for (int i=0;i<GetSize();i++)
	{
		if ((*this)[i] == FIELD_CATEGORY)
			return TRUE;
	}

	return FALSE;
}

// Sucht das Element in der Liste und liefert den Index zurück.
// -1, wenn es nicht gefunden wurde.
int CFieldList::Find(UINT uiField)
{
	for (int i=0;i<GetSize();i++)
	{
		if ((*this)[i] == uiField)
			return i;
	}

	return -1;
}

// Prüft, ob das angegebene Feld ein CD-Feld ist.
// Returns: true, wenn es sich um ein CD-Feld handelt.
//          false, wenn es sich um ein Lied-Feld handelt.
bool CFieldList::IsCDField(UINT uiField)
{
	if (uiField <= FIELD_LAST_CD_FIELD || uiField >= FIELD_ARTIST_CD_SORTKEY && uiField <= FIELD_LAST_ARTIST_CD_FIELD ||
		uiField >= FIELD_COMPOSER_CD_NAME && uiField <= FIELD_LAST_COMPOSER_CD_FIELD )
		return true;
	else
		return false;
}

void CFieldList::Move(int iIndex, int iOffset)
{
	UINT uiSaveField = (*this)[iIndex];
	(*this).RemoveAt(iIndex);
	(*this).InsertAt(iIndex+iOffset, uiSaveField);

	if (iIndex < m_aSize.GetSize())
	{
		UINT uiSaveFieldSize = m_aSize[iIndex];
		m_aSize.RemoveAt(iIndex);
		m_aSize.InsertAt(iIndex+iOffset, uiSaveFieldSize);
	}
}


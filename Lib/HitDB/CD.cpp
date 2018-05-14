// CD.cpp: implementation of the CCD class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "hitdb.h"
#include "CD.h"
#include "Identity.h"
#include "..\..\app\hitbase\resource.h"

using namespace System;
using namespace Big3::Hitbase::DataBaseEngine;
using namespace System::Windows::Forms;
using namespace System::Collections::Generic;

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

/////////////////////////////////////////////////////////////////////////////
// CCD

IMPLEMENT_DYNAMIC(CCD, CDBQuery)

CCD::CCD(CDataBase* pDataBase)
: CDBQuery(pDataBase)
{
	//{{AFX_FIELD_INIT(CCD)
	m_dwID = 0;
	m_dwTotalLength = 0;
	m_wNumberOfTracks = 0;
	m_bSampler = FALSE;
	m_wCDSetNumber = 0;
	m_dwIDCDSet = 0;
	m_dwIDArtist = 0;
	m_dwIDCategory = 0;
	m_dwIDMedium = 0;
	m_sTitle = _T("");
	m_sDate = _T("");
	m_sArchiveNumber = _T("");
	m_sBitmapPath = _T("");
	m_sCodes = _T("");
	m_sComment = _T("");
	m_lType = 0;
	m_lYearRecorded = 0;
	m_sCopyright = _T("");
	m_bOriginal = FALSE;
	m_sBackCoverFilename = _T("");
	m_sCDLabelBitmap = _T("");
	m_lRating = 0;
	m_sLabel = _T("");
	m_sURL = _T("");
	m_lPrice = 0;
	m_sUPC = _T("");
	m_dwIDComposer = 0;
	m_sLanguage = _T("");
	m_sLocation = _T("");
	m_nFields = 34;
	//}}AFX_FIELD_INIT
	m_nDefaultType = dbOpenDynaset;

	for (int i=0;i<MAX_USER_FIELDS;i++)
		m_sUser[i] = _T("");

	m_pArtist = NULL;
	m_pComposer = NULL;
	m_pCDSet = NULL;
	m_pIdentity = NULL;
	m_pMedium = NULL;
	m_pCategory = NULL;
	m_pTracks = NULL;
	m_pPrograms = NULL;
	m_pIndexes = NULL;
	m_pParticipants = NULL;
}

// Copy-Konstruktur
CCD::CCD(CCD& theOther)
{
	*this = theOther;

	m_pArtist = NULL;
	m_pComposer = NULL;
	m_pCDSet = NULL;
	m_pIdentity = NULL;
	m_pMedium = NULL;
	m_pCategory = NULL;
	m_pTracks = NULL;
	m_pPrograms = NULL;
	m_pIndexes = NULL;
	m_pParticipants = NULL;
}

// Hier kommt die Müllabführ!
CCD::~CCD()
{
	Clear();
}

CString CCD::GetDefaultSQL()
{
	return _T("[" + GetTableName() + "]");
}

void CCD::DoFieldExchange(CDaoFieldExchange* pFX)
{
	//{{AFX_FIELD_MAP(CCD)
	pFX->SetFieldType(CDaoFieldExchange::outputColumn);
	DFX_Long(pFX, _T("[IDCD]"), m_dwID);
	DFX_Long(pFX, _T("[dwGesamtlaenge]"), m_dwTotalLength);
	DFX_Long(pFX, _T("[cAnzahlLieder]"), m_wNumberOfTracks);
	DFX_Bool(pFX, _T("[bCDSampler]"), m_bSampler);
	DFX_Short(pFX, _T("[wNummerImSet]"), m_wCDSetNumber);
	DFX_Long(pFX, _T("[IDCDSet]"), m_dwIDCDSet);
	DFX_Long(pFX, _T("[IDArtist]"), m_dwIDArtist);
	DFX_Long(pFX, _T("[IDKategorie]"), m_dwIDCategory);
	DFX_Long(pFX, _T("[IDMedium]"), m_dwIDMedium);
	DFX_Text(pFX, _T("[szTitel]"), m_sTitle);
	DFX_Text(pFX, _T("[szDatum]"), m_sDate);
	DFX_Text(pFX, _T("[szArchivNummer]"), m_sArchiveNumber);
	DFX_Text(pFX, _T("[szPfadBitmap]"), m_sBitmapPath);
	DFX_Text(pFX, _T("[szKennzeichen]"), m_sCodes);
	DFX_Text(pFX, _T("[szKommentar]"), m_sComment);
	//}}AFX_FIELD_MAP

	if (m_pdb->GetVersion() < 801)      // Bei alten Datenbanken nur 2 benutzerdefinierte Felder!
	{
		for (int i=0;i<2;i++)
		{
			CString sField;
			sField.Format(L"szFeld%d", i+1);
			DFX_Text(pFX, sField, m_sUser[i]);
		}
		m_nFields = 17;
	}
	else
	{
		for (int i=0;i<MAX_USER_FIELDS;i++)
		{
			CString sField;
			sField.Format(L"szFeld%d", i+1);
			DFX_Text(pFX, sField, m_sUser[i]);
		}

		if (m_pdb->GetVersion() < 901)
			m_nFields = 20;
		else
		{
			DFX_Long(pFX, _T("[C_Type]"), m_lType);
			DFX_Long(pFX, _T("[C_YearRecorded]"), m_lYearRecorded);
			DFX_Text(pFX, _T("[C_Copyright]"), m_sCopyright);

			if (m_pdb->GetVersion() < 1001)
				m_nFields = 23;
			else
			{
				DFX_Bool(pFX, _T("[C_Original]"), m_bOriginal);
				DFX_Text(pFX, _T("[C_BackCoverBitmap]"), m_sBackCoverFilename);
				DFX_Text(pFX, _T("[C_CDLabelBitmap]"), m_sCDLabelBitmap);
				DFX_Long(pFX, _T("[C_Rating]"), m_lRating);
				DFX_Text(pFX, _T("[C_Label]"), m_sLabel);
				DFX_Text(pFX, _T("[C_URL]"), m_sURL);
				DFX_Long(pFX, _T("[C_Price]"), m_lPrice);
				DFX_Text(pFX, _T("[C_UPC]"), m_sUPC);

				if (m_pdb->GetVersion() < 1101)
				{
					m_nFields = 31;
				}
				else
				{
					DFX_Long(pFX, _T("[C_IDComposer]"), m_dwIDComposer);
					DFX_Text(pFX, _T("[C_Language]"), m_sLanguage);
					DFX_Text(pFX, _T("[C_Location]"), m_sLocation);
				}
			}
		}
	}

	// Wenn das Feld leer ist, nehmen wir als Standardwert CDTYPE_AUDIOCD.
	if (m_lType == AFX_RFX_LONG_PSEUDO_NULL)
		m_lType = CDTYPE_AUDIOCD;

	// Wenn das Feld leer ist, ist das Jahr 0 (unbekannt).
	if (m_lYearRecorded == AFX_RFX_LONG_PSEUDO_NULL)
		m_lYearRecorded = 0;

	// Wenn das Feld leer ist, nehmen wir als Standardwert 0.
	if (m_lRating == AFX_RFX_LONG_PSEUDO_NULL)
		m_lRating = 0;

	// Wenn das Feld leer ist, nehmen wir als Standardwert 0.
	if (m_lPrice == AFX_RFX_LONG_PSEUDO_NULL)
		m_lPrice = 0;

	// Wenn das Feld leer ist, nehmen wir als Standardwert 0.
	if (m_dwIDComposer == AFX_RFX_LONG_PSEUDO_NULL)
		m_dwIDComposer = 0;
}

/////////////////////////////////////////////////////////////////////////////
// CCD diagnostics

#ifdef _DEBUG
void CCD::AssertValid() const
{
	CDaoRecordset::AssertValid();
}

void CCD::Dump(CDumpContext& dc) const
{
	CDaoRecordset::Dump(dc);
}
#endif //_DEBUG

CCD& CCD::operator =(CCD &theOther)
{
	m_dwID = theOther.m_dwID;
	m_dwTotalLength = theOther.m_dwTotalLength;
	m_wNumberOfTracks = theOther.m_wNumberOfTracks;
	m_bSampler = theOther.m_bSampler;
	m_wCDSetNumber = theOther.m_wCDSetNumber;
	m_dwIDCDSet = theOther.m_dwIDCDSet;
	m_dwIDArtist = theOther.m_dwIDArtist;
	m_dwIDCategory = theOther.m_dwIDCategory;
	m_dwIDMedium = theOther.m_dwIDMedium;
	m_sTitle = theOther.m_sTitle;
	m_sDate = theOther.m_sDate;
	m_sArchiveNumber = theOther.m_sArchiveNumber;
	m_sBitmapPath = theOther.m_sBitmapPath;
	m_sCodes = theOther.m_sCodes;
	m_sComment = theOther.m_sComment;

	for (int i=0;i<MAX_USER_FIELDS;i++)
		m_sUser[i] = theOther.m_sUser[i];

	m_lType = theOther.m_lType;

	// 14.01.2002
	m_lYearRecorded = theOther.m_lYearRecorded;
	m_sCopyright = theOther.m_sCopyright;

	// 27.07.2003
	m_bOriginal = theOther.m_bOriginal;						// Original-CD (kann ins CD-Archiv übertragen werden)
	m_sBackCoverFilename = theOther.m_sBackCoverFilename;	// Dateiname des Back-Cover (Grafik)
	m_sCDLabelBitmap = theOther.m_sCDLabelBitmap;			// Dateiname des CD-Labels (Grafik)
	m_lRating = theOther.m_lRating;							// Rating der CD (z.Zt. 1-5 Sterne)
	m_sLabel = theOther.m_sLabel;							// Label (z.B. Sony, EMI, etc.)
	m_sURL = theOther.m_sURL;								// Verknüpfungen (getrennt mit Semikolon)
	m_lPrice = theOther.m_lPrice;							// Preis (Wähung * 100, z.B. 29,99€ = 2999)
	m_sUPC = theOther.m_sUPC;								// UPC oder EAN Code

	// 14.12.2005
	m_dwIDComposer = theOther.m_dwIDComposer;
	m_sLanguage = theOther.m_sLanguage;						// Sprache der CD
	m_sLocation = theOther.m_sLocation;						// Standort der CD

	return *this;
}

// Kopiert ALLE Daten der angegebenen CD in das aktuelle CD-Objekt
void CCD::Copy(CCD &theOther)
{
//	Clear();

	m_dwTotalLength = theOther.m_dwTotalLength;
	m_wNumberOfTracks = theOther.m_wNumberOfTracks;
	m_bSampler = theOther.m_bSampler;
	m_wCDSetNumber = theOther.m_wCDSetNumber;
	m_sTitle = theOther.m_sTitle;
	m_sDate = theOther.m_sDate;
	m_sArchiveNumber = theOther.m_sArchiveNumber;
	m_sBitmapPath = theOther.m_sBitmapPath;
	m_sCodes = theOther.m_sCodes;
	m_sComment = theOther.m_sComment;

	for (int i=0;i<MAX_USER_FIELDS;i++)
		m_sUser[i] = theOther.m_sUser[i];

	m_lType = theOther.m_lType;

	// 14.01.2002
	m_lYearRecorded = theOther.m_lYearRecorded;
	m_sCopyright = theOther.m_sCopyright;
	//_14.01.2002

	// 27.07.2003
	m_bOriginal = theOther.m_bOriginal;						// Original-CD (kann ins CD-Archiv übertragen werden)
	m_sBackCoverFilename = theOther.m_sBackCoverFilename;	// Dateiname des Back-Cover (Grafik)
	m_sCDLabelBitmap = theOther.m_sCDLabelBitmap;			// Dateiname des CD-Labels (Grafik)
	m_lRating = theOther.m_lRating;							// Rating der CD (z.Zt. 1-5 Sterne)
	m_sLabel = theOther.m_sLabel;							// Label (z.B. Sony, EMI, etc.)
	m_sURL = theOther.m_sURL;								// Verknüpfungen (getrennt mit Semikolon)
	m_lPrice = theOther.m_lPrice;							// Preis (Wähung * 100, z.B. 29,99€ = 2999)
	m_sUPC = theOther.m_sUPC;								// UPC oder EAN Code

	// 14.12.2005
	m_sLanguage = theOther.m_sLanguage;						// Sprache der CD
	m_sLocation = theOther.m_sLocation;						// Standort der CD


	GetArtist()->Copy(*theOther.GetArtist());
	GetComposer()->Copy(*theOther.GetComposer());
	GetCategory()->Copy(*theOther.GetCategory());
	GetCDSet()->Copy(*theOther.GetCDSet());
	GetIdentity()->Copy(*theOther.GetIdentity());
	GetIndexes()->Copy(*theOther.GetIndexes());
	GetMedium()->Copy(*theOther.GetMedium());
	GetPrograms()->Copy(*theOther.GetPrograms());
	GetParticipants()->Copy(*theOther.GetParticipants());

	for (int i=0;i<theOther.m_wNumberOfTracks;i++)
	{
		GetTrack(i)->Copy(*theOther.GetTrack(i));
	}
}

void CCD::Clear()
{
	m_dwID = 0;
	m_dwTotalLength = 0;
	m_wNumberOfTracks = 0;
	m_bSampler = FALSE;
	m_wCDSetNumber = 0;
	m_dwIDCDSet = 0;
	m_dwIDArtist = 0;
	m_dwIDCategory = 0;
	m_dwIDMedium = 0;
	m_sTitle = _T("");
	m_sDate = _T("");
	m_sArchiveNumber = _T("");
	m_sBitmapPath = _T("");
	m_sCodes = _T("");
	m_sComment = _T("");

	for (int i=0;i<MAX_USER_FIELDS;i++)
		m_sUser[i] = _T("");

	m_lType = 0;
	
	// 14.01.2002
	m_lYearRecorded = 0;
	m_sCopyright = _T("");

	// 27.07.2003
	m_bOriginal = FALSE;		// Original-CD (kann ins CD-Archiv übertragen werden)
	m_sBackCoverFilename = "";	// Dateiname des Back-Cover (Grafik)
	m_sCDLabelBitmap = "";		// Dateiname des CD-Labels (Grafik)
	m_lRating = 0;				// Rating der CD (z.Zt. 1-5 Sterne)
	m_sLabel = "";				// Label (z.B. Sony, EMI, etc.)
	m_sURL = "";				// Verknüpfungen (getrennt mit Semikolon)
	m_lPrice = 0;				// Preis (Wähung * 100, z.B. 29,99€ = 2999)
	m_sUPC = "";				// UPC oder EAN Code

	// 14.12.2005
	m_dwIDComposer = 0;
	m_sLanguage = "";						// Sprache der CD
	m_sLocation = "";						// Standort der CD

	if (m_pArtist)
	{
		delete m_pArtist;
		m_pArtist = NULL;
	}

	if (m_pComposer)
	{
		delete m_pComposer;
		m_pComposer = NULL;
	}

	if (m_pCDSet)
	{
		delete m_pCDSet;
		m_pCDSet = NULL;
	}

	if (m_pIdentity)
	{
		delete m_pIdentity;
		m_pIdentity = NULL;
	}

	if (m_pMedium)
	{
		delete m_pMedium;
		m_pMedium = NULL;
	}

	if (m_pCategory)
	{
		delete m_pCategory;
		m_pCategory = NULL;
	}

	if (m_pTracks)
	{
		delete m_pTracks;
		m_pTracks = NULL;
	}

	if (m_pPrograms)
	{
		delete m_pPrograms;
		m_pPrograms = NULL;
	}

	if (m_pIndexes)
	{
		delete m_pIndexes;
		m_pIndexes = NULL;
	}

	if (m_pParticipants)
	{
		delete m_pParticipants;
		m_pParticipants = NULL;
	}
}

CArtist* CCD::GetArtist()
{
	// Wurde der Interpret schon mal gelesen?
	if (!m_pArtist || m_pArtist->m_dwID != m_dwIDArtist)
	{
		if (m_pArtist)
		{
			delete m_pArtist;
			m_pArtist = NULL;
		}

		// OK, jetzt lesen!
		m_pArtist = new CArtist(m_pdb);

		if (m_dwIDArtist)
		{
			// Interpreten jetzt lesen!
			CString sWhere;
			sWhere.Format(L"IDArtist=%d", m_dwIDArtist);

			if (m_pArtist->QueryStart(sWhere))
			{
				BOOL bFound = m_pArtist->QueryFindFirst();
				ASSERT(bFound);

				m_pArtist->QueryEnd();
			}
		}
	}

	ASSERT (m_pArtist);

	return m_pArtist;
}

CArtist* CCD::GetComposer()
{
	// Wurde der Interpret schon mal gelesen?
	if (!m_pComposer || m_pComposer->m_dwID != m_dwIDComposer)
	{
		if (m_pComposer)
		{
			delete m_pComposer;
			m_pComposer = NULL;
		}

		// OK, jetzt lesen!
		m_pComposer = new CArtist(m_pdb);

		if (m_dwIDComposer)
		{
			// Komponisten jetzt lesen!
			CString sWhere;
			sWhere.Format(L"IDArtist=%d", m_dwIDComposer);

			if (m_pComposer->QueryStart(sWhere))
			{
				BOOL bFound = m_pComposer->QueryFindFirst();
				ASSERT(bFound);

				m_pComposer->QueryEnd();
			}
		}
	}

	ASSERT (m_pComposer);

	return m_pComposer;
}

CCDSet* CCD::GetCDSet()
{
	// Wurde das CD-Set schon mal gelesen?
	if (!m_pCDSet || m_pCDSet->m_dwID != m_dwIDCDSet)
	{
		if (m_pCDSet)
		{
			delete m_pCDSet;
			m_pCDSet = NULL;
		}

		// OK, jetzt lesen!
		m_pCDSet = new CCDSet(m_pdb);

		if (m_dwIDCDSet)
		{
			// Interpreten jetzt lesen!
			CString sWhere;
			sWhere.Format(L"IDCDSet=%d", m_dwIDCDSet);
			m_pCDSet->QueryStart(sWhere);

			BOOL bFound = m_pCDSet->QueryFindFirst();
			ASSERT(bFound);

			m_pCDSet->QueryEnd();
		}
	}

	ASSERT (m_pCDSet);

	return m_pCDSet;
}

CIdentity* CCD::GetIdentity()
{
	// Wurde die Identity schon mal gelesen?
	if (!m_pIdentity || m_pIdentity->m_dwIDCD != m_dwID)
	{
		if (m_pIdentity)
		{
			delete m_pIdentity;
			m_pIdentity = NULL;
		}

		// OK, jetzt lesen!
		m_pIdentity = new CIdentity(m_pdb);

		// Identity jetzt lesen!
		if (m_dwID)
		{
			CString sWhere;
			sWhere.Format(L"IDCD=%d", m_dwID);
			m_pIdentity->QueryStart(sWhere);

			if (!m_pIdentity->IsEOF())
			{
				BOOL bFound = m_pIdentity->QueryFindFirst();
	//			ASSERT(bFound);             // Es gibt auch CDs ohne Identity!!

			}

			m_pIdentity->QueryEnd();
		}
	}

	ASSERT (m_pIdentity);

	return m_pIdentity;
}

CCategory* CCD::GetCategory()
{
	// Wurde die Identity schon mal gelesen?
	if (!m_pCategory || m_pCategory->m_dwID != m_dwIDCategory)
	{
		if (m_pCategory)
		{
			delete m_pCategory;
			m_pCategory = NULL;
		}

		// OK, jetzt lesen!
		m_pCategory = new CCategory(m_pdb);

		if (m_dwIDCategory)
		{
			// Interpreten jetzt lesen!
			CString sWhere;
			sWhere.Format(L"IDKategorie=%d", m_dwIDCategory);
			m_pCategory->QueryStart(sWhere);

			BOOL bFound = m_pCategory->QueryFindFirst();
			// Es muß keins gefunden werden!

			m_pCategory->QueryEnd();
		}
	}

	ASSERT (m_pCategory);

	return m_pCategory;
}

CMedium* CCD::GetMedium()
{
	// Wurde die Identity schon mal gelesen?
	if (!m_pMedium || m_pMedium->m_dwID != m_dwIDMedium)
	{
		if (m_pMedium)
		{
			delete m_pMedium;
			m_pMedium = NULL;
		}

		// OK, jetzt lesen!
		m_pMedium = new CMedium(m_pdb);

		if (m_dwIDMedium)
		{
			// Interpreten jetzt lesen!
			CString sWhere;
			sWhere.Format(L"IDMedium=%d", m_dwIDMedium);
			m_pMedium->QueryStart(sWhere);

			BOOL bFound = m_pMedium->QueryFindFirst();
			// Es muß keins gefunden werden!

			m_pMedium->QueryEnd();
		}
	}

	ASSERT (m_pMedium);

	return m_pMedium;
}

CTrackList* CCD::GetTracks()
{
	// Wurde diese CD schon mal gelesen?
	if (!m_pTracks || m_pTracks->m_dwIDCD != m_dwID)
	{
		if (m_pTracks)
		{
			delete m_pTracks;
			m_pTracks = NULL;
		}

		// OK, jetzt lesen!
		CTrack track(m_pdb);
		m_pTracks = new CTrackList(m_dwID);

		if (m_dwID)
		{
			// Interpreten jetzt lesen!
			CString sWhere;
			sWhere.Format(L"IDCD=%d", m_dwID);
			track.QueryStart(sWhere);

			if (track.IsEOF())   // Keine Lieder vorhanden. Also jetzt anlegen!
			{
/*				for (int i=0;i<m_wNumberOfTracks;i++)
				{
					CTrack newTrack(m_pdb);
					newTrack.m_dwIDCD = m_dwID;
					newTrack.m_wTrack = i+1;
					m_pTracks->SetAtGrow(newTrack.m_wTrack-1, newTrack);
				}*/
			}
			else
			{
				BOOL bFound = track.QueryFindFirst();
				while (bFound)
				{
					m_pTracks->SetAtGrow(track.m_wTrack-1, track);

					bFound = track.QueryFindNext();
				}
			}

			track.QueryEnd();
		}
	}

	ASSERT (m_pTracks);

	return m_pTracks;
}

// Liefert die Liste der Programme der CD zurück.
CProgramList* CCD::GetPrograms()
{
	// Wurde die CD schon mal gelesen?
	if (!m_pPrograms || m_pPrograms->m_dwIDCD != m_dwID)
	{
		if (m_pPrograms)
		{
			delete m_pPrograms;
			m_pPrograms = NULL;
		}

		// OK, jetzt lesen!
		CProgram program(m_pdb);
		m_pPrograms = new CProgramList(m_pdb, m_dwID);

		if (m_dwID)
		{
			// Programme jetzt lesen!
			CString sWhere;
			sWhere.Format(L"IDCD=%d", m_dwID);
			program.QueryStart(sWhere);

			int nCount = 0;
			BOOL bFound = program.QueryFindFirst();
			while (bFound)
			{
				m_pPrograms->SetAtGrow(nCount, program);

				bFound = program.QueryFindNext();

				nCount++;
			}

			program.QueryEnd();
		}
	}

	ASSERT (m_pPrograms);

	return m_pPrograms;
}

// Liefert die Liste der Indizes der CD zurück.
CIndexList* CCD::GetIndexes()
{
	// Wurde die CD schon mal gelesen?
	if (!m_pIndexes || m_pIndexes->m_dwIDCD != m_dwID)
	{
		if (m_pIndexes)
		{
			delete m_pIndexes;
			m_pIndexes = NULL;
		}

		// OK, jetzt lesen!
		CIndex index(m_pdb);
		m_pIndexes = new CIndexList(m_pdb, m_dwID);

		if (m_dwID)
		{
			// Interpreten jetzt lesen!
			CString sWhere;
			sWhere.Format(L"IDCD=%d", m_dwID);
			index.QueryStart(sWhere);

			int nCount = 0;
			BOOL bFound = index.QueryFindFirst();
			while (bFound)
			{
				m_pIndexes->SetAtGrow(nCount, index);

				bFound = index.QueryFindNext();

				nCount++;
			}

			index.QueryEnd();
		}
	}

	ASSERT (m_pIndexes);

	return m_pIndexes;
}

// Liefert die Liste der Mitwirkenden der CD zurück.
CParticipantList* CCD::GetParticipants()
{
	// Wurde die CD schon mal gelesen?
	if (!m_pParticipants || m_pParticipants->m_dwIDCD != m_dwID)
	{
		if (m_pParticipants)
		{
			delete m_pParticipants;
			m_pParticipants = NULL;
		}

		// OK, jetzt lesen!
		CParticipant participant(m_pdb);
		m_pParticipants = new CParticipantList(m_pdb, m_dwID);

		if (m_dwID)
		{
			// Interpreten jetzt lesen!
			CString sWhere;
			sWhere.Format(L"P_IDCD=%d", m_dwID);
			participant.QueryStart(sWhere);

			int nCount = 0;
			BOOL bFound = participant.QueryFindFirst();
			while (bFound)
			{
				m_pParticipants->SetAtGrow(nCount, participant);

				bFound = participant.QueryFindNext();

				nCount++;
			}

			participant.QueryEnd();
		}
	}

	ASSERT (m_pParticipants);

	return m_pParticipants;
}

// Liefert eine Adresse auf das angegebene Lied der CD zurück.
CTrack* CCD::GetTrack(int iTrack)
{
	ASSERT(iTrack >= 0 && iTrack <= m_wNumberOfTracks);

	// Unter Umständen das Lied-Array vergrößern.
	if (iTrack >= GetTracks()->GetSize())
	{
		for (int i=GetTracks()->GetSize();i<=iTrack;i++)
		{
			CTrack newTrack(m_pdb);
			newTrack.m_dwIDCD = m_dwID;
			newTrack.m_wTrack = i+1;
			m_pTracks->SetAtGrow(newTrack.m_wTrack-1, newTrack);
		}
	}

	return &(*GetTracks())[iTrack];
}

// Schreibt die CD mit allen Untertabellen in die Datenbank. 
// Die CD muß schon vorhanden sein! Um neue CDs einzufügen, die Funktion Add
// benutzen! 

BOOL CCD::Write()
{
    m_dwIDCategory = m_pdb->GetIDFromCategory(GetCategory()->m_sCategory);
    m_dwIDMedium = m_pdb->GetIDFromMedium(GetMedium()->m_sMedium);

//JUS 26.09.2002	m_dwIDArtist = m_pdb->GetIDFromArtist(GetArtist()->m_sArtist);
	m_dwIDArtist = GetArtist()->GetIDFromName();
	m_dwIDComposer = GetComposer()->GetIDFromName();
	m_dwIDCDSet = m_pdb->GetIDFromCDSet(GetCDSet()->m_sCDSetName);

	// Abfragen, ob das CD-Set gelöscht wurde.
	if (!m_dwIDCDSet && m_pCDSet)
	{
		delete m_pCDSet;
		m_pCDSet = NULL;
	}

    CCD CDSave;

	// Tabellennamen ermitteln, um ihn für die Erstellung der ID zu benutzen
	CString sTablename = GetDefaultSQL().Mid(1);
	sTablename = sTablename.Left(sTablename.GetLength()-1);

	BOOL bAddNew = m_dwID <= 0;

	// JUS 001104: Prüfen, ob das Recordset schon offen ist! Dann läuft wohl gerade eine Query!
	BOOL bOpen = IsOpen();
	if (!bOpen)
	{
		CDSave = *this;

		// Die ID heißt in unseren Tabellen immer ID[Tabellenname]
		CString sWhere;
		sWhere.Format(L"ID%s = %ld", sTablename, m_dwID);

		if (!QueryStart(sWhere))
			return FALSE;
	}
	else
		CDSave.Copy(*this);

	if (!bAddNew)
		Edit();
	else
		AddNew();

	if (!bOpen)
		*this = CDSave;
	else
		Copy(CDSave);
	
	DWORD dwID = m_dwID;
	if (bAddNew)
	{
		COleVariant var;
		GetFieldValue(L"IDCD", var);
		dwID = var.lVal;
	}

	SetFieldDirty(NULL);		          // JUS 001104: Damit auf jeden Fall geschrieben wird!
	SetFieldDirty(&m_dwID, FALSE);        // JUS 001104: Die ID darf nicht verändert werden!
	
	Update();

	// JUS 001104
	if (!bOpen)
		QueryEnd();

	*this = CDSave;
	m_dwID = dwID;

	if (m_pTracks || bAddNew)    // Wenn der Pointer nicht gesetzt ist, dann hat auch keiner was geändert!
	{
		for (int i=0;i<m_wNumberOfTracks;i++)
		{
			GetTrack(i)->Write();
		}
	}

	// Hier direkt auf m_pIdentity zugreifen, da sich ja die ID ändert und sonst
	// die Identity neu gelesen würde.
	if (m_pIdentity)
	{
		m_pIdentity->m_dwIDCD = m_dwID;
		if (!m_pIdentity->Write())
			return FALSE;
	}

	if (m_pParticipants)
	{
		m_pParticipants->m_dwIDCD = m_dwID;
		m_pParticipants->AddAll();
	}

	return TRUE;	
}

// Fügt die CD in die Datenbank ein. Wenn Sie schon vorhanden ist (also schon eine
// ID hat), dann wird sie überschrieben.
BOOL CCD::Add()
{
	try
	{
		if (GetArtist()->m_sArtist.IsEmpty())      // CD ohne Interpret ist nicht erlaubt!
			return FALSE;

		// JUS 20010910: Bei Sampler-CD muss auch der Titel erfasst werden!
		// JUS 20040719: Wieder rausgenommen, weil beim MP3-CDs das ganze für den User nicht transparent genug war.
//		if (m_bSampler && m_sTitle.IsEmpty())
//			return FALSE;

		// Dann ist die CD ja schon vorhanden!
		if (m_dwID > 0)
			return Write();

		// Ab hier: Neue CD hinzufügen
		m_dwIDCategory = m_pdb->GetIDFromCategory(GetCategory()->m_sCategory);
		m_dwIDMedium = m_pdb->GetIDFromMedium(GetMedium()->m_sMedium);

		m_dwIDArtist = GetArtist()->GetIDFromName();
		m_dwIDComposer = GetComposer()->GetIDFromName();
		m_dwIDCDSet = m_pdb->GetIDFromCDSet(GetCDSet()->m_sCDSetName);

		CCD CDSave = *this;

		if (!OpenTable())
			return FALSE;

		AddNew();

		*this = CDSave;
		
		DWORD dwID;
		COleVariant var;
		GetFieldValue(L"IDCD", var);
		m_dwID = dwID = var.lVal;
		
		Update();

		QueryEnd();

		*this = CDSave;
		m_dwID = dwID;

		// Noch die ID der CD für die Lieder definieren!
		if (m_pTracks)
			m_pTracks->m_dwIDCD = dwID;

		for (int i=0;i<m_wNumberOfTracks;i++)
			GetTrack(i)->m_dwIDCD = dwID;
	//		((*m_pTracks)[i]).m_dwIDCD = dwID;

		// Jetzt noch die Lieder wegschreiben!
		if (m_pTracks)      // Wenn CD keine Lieder hat, könnte das hier abstürzen, darum Abfrage.
			m_pTracks->AddAll(this);

		if (m_pIdentity)
		{
			// Hier direkt auf m_pIdentity zugreifen, da sich ja die ID ändert und sonst
			// die Identity neu gelesen würde.
			m_pIdentity->m_dwIDCD = m_dwID;
			if (!m_pIdentity->Write())
				return FALSE;
		}

		if (m_pParticipants)      // Wenn CD keine Mitwirkenden hat, könnte das hier abstürzen, darum Abfrage.
		{
			m_pParticipants->m_dwIDCD = dwID;
			m_pParticipants->AddAll();
		}
	}
	catch (CException *e)
	{
		e->ReportError();
		e->Delete();
		return FALSE;
	}

	return TRUE;	
}

// Sucht nach der CD mit der angegebenen Identity.
BOOL CCD::Find(const CString& sIdentity)
{
	CString sWhere;
	sWhere.Format(L"szIdentity='%s'", sIdentity);

	CIdentity Identity(m_pdb);
	if (!Identity.QueryStart(sWhere))
		return FALSE;

	if (Identity.IsEOF())
	{
		Identity.QueryEnd();
		return FALSE;
	}

	Identity.QueryEnd();

	// JUS 14.05.2002: Alle Felder löschen, damit auch alles initialisiert und neu eingelesen wird.
	Clear();

	if (!GetRecordFromID(Identity.m_dwIDCD))
		return FALSE;

	return TRUE;
}

// Sucht nach der CD mit dem angegebenen UPC/EAN Code.
BOOL CCD::FindUPC(const CString& sUPC)
{
	CString sWhere;
	sWhere.Format(L"C_UPC='%s'", sUPC);

	CCD theCD(m_pdb);
	if (!theCD.QueryStart(sWhere))
		return FALSE;

	if (theCD.IsEOF())
	{
		theCD.QueryEnd();
		return FALSE;
	}

	theCD.QueryEnd();

	// JUS 14.05.2002: Alle Felder löschen, damit auch alles initialisiert und neu eingelesen wird.
	Clear();

	if (!GetRecordFromID(theCD.m_dwID))
		return FALSE;

	return TRUE;
}

CProgram* CCD::GetProgram(int iProgram)
{
	return &(*GetPrograms())[iProgram];
}

CIndex* CCD::GetIndex(int iIndex)
{
	return &(*GetIndexes())[iIndex];
}

int CCD::cddb_sum(int n)
{
    int	ret = 0;

    while (n > 0)
    {
        ret += (n % 10);
        n /= 10;
    }

    return ret;
}

unsigned long CCD::GetCDDBDiscID()
{
    int	i, t, n = 0;

    for (i = 0; i < m_wNumberOfTracks; i++)
    {
        n += cddb_sum(GetTrack(i)->m_dwStartPosition/1000);
    }

    //t = m_dwTotalLength/1000+1;

	t = GetRoundedTime(GetCDDBDiscLength());

    return ((n % 0xff) << 24 | t << 8 | m_wNumberOfTracks);
}

unsigned long CCD::GetRoundedTime(long t)
{
	long minutes = t/1000/60;
	long seconds = t/1000%60;

	long first = GetTrack(0)->m_dwStartPosition;
	long minutesFirst = first/1000/60;
	long secondsFirst = first/1000%60;

	return minutes*60+seconds - (minutesFirst*60+secondsFirst);
}

// Liefert die tatsächliche Länge der CD zurück auch wenn der letzte Track ein Datentrack ist.
unsigned long CCD::GetCDDBDiscLength()
{
	int offsetInFrames = GetTrack(m_wNumberOfTracks-1)->m_dwStartPosition * 75 / 1000;
	int lengthInFrames = GetTrack(m_wNumberOfTracks-1)->m_dwLength * 75 / 1000;

	// Die + 1 ist hier wohl nötig, da die Länge des letzten Frames beim MCI-Interface ein Frame zu kurz ist 
	// (siehe freedb Doku).
	//(offset_minutes * 60 * 75) + (offset_seconds * 75) + offset_frames +
//(length_minutes * 60 * 75) + (length_seconds * 75) + length_frames + 1 = X

	int realLength = (offsetInFrames + lengthInFrames + 1) * 1000 / 75;

	return realLength;
}

/*int CCD::cddb_sum(int n)
{
	char	buf[12],
		*p;
	int	ret = 0;

	// For backward compatibility this algorithm must not change
	sprintf(buf, "%lu", n);
	for (p = buf; *p != '\0'; p++)
		ret += (*p - '0');

	return (ret);
}

unsigned long CCD::GetCDDBDiscID()
{
   int i, t = 0, n = 0;

   // For backward compatibility this algorithm must not change
   for (i = 0; i < m_wNumberOfTracks; i++) {
      n += cddb_sum(GetTrack(i)->m_dwStartPosition/1000);
      
      t += GetTrack(i+1)->m_dwStartPosition/1000-GetTrack(i)->m_dwStartPosition/1000;
   }
   
   return (((n % 0xff) << 24) | (t << 8) | (m_wNumberOfTracks));
}*/

CString CCD::GetArtistText()
{
	CString str, title;
	
	if (m_bSampler)
	{
		title.LoadString(IDS_TITLE);
		str.Format(L"%s 1", title);
	}
	else
		str.LoadString(IDS_ARTIST);
	
	return str;
}

CString CCD::GetTitleText()
{
	CString str, title;
	
	title.LoadString(IDS_TITLE);
	if (m_bSampler)
	{
		str.Format(L"%s 2", title);
	}
	else
		str = title;
	
	return str;
}

/*
 * Füllt aus dem angegebenen Buffer, der normalerweise vorher aus dem
 * Isam gelesen wurde, die CCD-Klasse mit den entsprechenden Werten.
 */

int CCD::ReadOldBuffer(CDataBase* pdb, char *buff)
{
   char *savebuff = buff;
   CString strbuff;
   int i, j;
   CString recname;
   int PossibleError;
 
   // Warnung ausgeben und Datensatz in Text-Datei
   PossibleError = FALSE;

   buff+=2;
   buff += CopyOldBuff(GetIdentity()->m_sIdentity, buff, 20, 0);
   buff += CopyOldBuff(m_dwTotalLength, buff);
   short wNumberOfTracks;
   buff += CopyOldBuff(wNumberOfTracks, buff);
   m_wNumberOfTracks = wNumberOfTracks;

   buff+=3*sizeof(long);

   buff += CopyOldBuff(m_wCDSetNumber, buff);
   buff += CopyOldBuff(GetCDSet()->m_sCDSetName, buff, 101, 0);
   buff += CopyOldBuff(m_bSampler, buff);

   buff += CopyOldBuff(GetArtist()->m_sArtist, buff, 101, 0);
   buff += CopyOldBuff(m_sTitle, buff, 101, 0);
   buff += CopyOldBuff(recname, buff, 101, 0);
   buff += CopyOldBuff(GetCategory()->m_sCategory, buff, 51,0);
   buff += CopyOldBuff(strbuff, buff, 9, 0);

   if (!strbuff.IsEmpty())
      m_sDate = pdb->DateShort2Long(strbuff);
     else
      m_sDate = "";

   buff += CopyOldBuff(m_sCodes, buff, 6, 0);

   buff += CopyOldBuff(m_sComment, buff, 101, 0);
   buff += CopyOldBuff(m_sUser[0], buff, 101, 0);
   buff += CopyOldBuff(m_sUser[1], buff, 101, 0);
//----- Bis hier mögliche Index-Felder, weil feste Feldlängen -----

   short wProgramCount;
   short wProgramDefault;
   buff += CopyOldBuff(wProgramCount, buff);
   buff += CopyOldBuff(wProgramDefault, buff);
   
   for (i=0;i<wProgramCount;i++)
   {
	   CString sProgName, sTracks, str;
       buff += CopyOldBuff(sProgName, buff, -1, 51);
       for (j=0;*(short *)buff;j++)
	   {
           str.Format(L"%d,", *buff);
		   sTracks += str;
           buff+=sizeof(short);
	   }
       if (!str.IsEmpty())
		   str = str.Left(str.GetLength()-1);
	   GetPrograms()->AddNew(sProgName, sTracks);
       buff+=sizeof(short);
   }
   
   if (wProgramCount > 0)
        GetPrograms()->SetDefault(wProgramDefault);

   short wIndexCount;
   buff += CopyOldBuff(wIndexCount, buff);
   
   for (i=0;i<wIndexCount;i++)
   {
	   CString sIndexName;
	   long dwPosition;
       buff += CopyOldBuff(sIndexName, buff, -1, 51);
       buff += CopyOldBuff(dwPosition, buff);

	   GetIndexes()->AddNew(sIndexName, dwPosition);
   }
   
   for (i=0;i<m_wNumberOfTracks;i++)
   {
       buff += CopyOldBuff(GetTrack(i)->GetArtist()->m_sArtist, buff, -1, 101);
       buff += CopyOldBuff(GetTrack(i)->m_sTitle, buff, -1, 101);
       buff += CopyOldBuff(GetTrack(i)->m_dwLength, buff);
       buff += CopyOldBuff(GetTrack(i)->m_wBpm, buff);
       buff += CopyOldBuff(GetTrack(i)->m_sCodes, buff, 6, 0);
       buff += CopyOldBuff(GetTrack(i)->m_sComment, buff, -1, 101);
       buff += CopyOldBuff(GetTrack(i)->m_sUser[0], buff, -1, 101);
       buff += CopyOldBuff(GetTrack(i)->m_sUser[1], buff, -1, 101);
       GetTrack(i)->m_sWavePath = recname;
   }
   
   buff += CopyOldBuff(m_sBitmapPath, buff, -1, 256);
   
   //if (Version != FILE_VERSION) PossibleError = TRUE;
   if (m_wNumberOfTracks < 0 || m_wNumberOfTracks > 100) PossibleError = TRUE;
   
   if (wProgramDefault > wProgramCount) PossibleError = TRUE;
   if (wProgramDefault > 100 || wProgramCount > 100) PossibleError = TRUE;
   if (wProgramDefault < 0 || wProgramCount < 0) PossibleError = TRUE;
   
   if (wIndexCount < 0 || wIndexCount > 100) PossibleError = TRUE;
   
   if (PossibleError)
	   return FALSE;
   
   return TRUE;
   
}

// Überträgt aus dem Buffer bis zum Null-Terminator und liefert
// die Länge zurück.

int CCD::CopyOldBuff(CString &str1, char *buff, int n, int maxlen)
{
	char *savebuff = buff;
	int count=0;
	wchar_t *str = str1.GetBuffer(300);
	
	while (((*buff && n == -1) || n > 0) && (!maxlen || count < maxlen))
	{
		*str++ = *buff++;
		if (n > 0)
			n--;
		count ++;
	}
	if (n)
		*str = *buff++;           // Null-Terminator mitkopieren und zählen!
	
	//   if (count >= maxlen && maxlen)         // Fehler im Datensatz!
	//      PossibleError = TRUE;
	
	str1.ReleaseBuffer();
	return (int)(buff-savebuff);
}

/*
 * Überträgt aus dem Buffer bis zum Null-Terminator und liefert
 * die Länge zurück.
 */
 
 int CCD::CopyOldBuff(short &str, char *buff)
 {
	 str = *((short *)buff);
	 
	 return (sizeof(short));
 }
 
 /*
 * Überträgt aus dem Buffer bis zum Null-Terminator und liefert
 * die Länge zurück.
 */
 
 int CCD::CopyOldBuff(long &str, char *buff)
 {
	 str = *((long *)buff);
	 
	 return (sizeof(long));
 }
 
 /*
 * Überträgt aus dem Buffer bis zum Null-Terminator und liefert
 * die Länge zurück.
 */
 
 int CCD::CopyOldBuff(BOOL &str, char *buff)
 {
	 str = *((short *)buff);
	 
	 return (2);
 }
 
// Der angegebene Datensatz wird geladen. Alle gefundenen CDs werden im
// Array übergeben (nur die Record-Number).
// Zurückgeliefert wird der erste gefundene Datensatz, oder 0 wenn keiner
// gefunden wurde.

int CCD::Find(CArray <long, long&> * pRecordNumberArray)
{
	DWORD dwRecordNumber;
	CWaitCursor wait;
	
	if (!(dwRecordNumber = IsCDInDataBase(pRecordNumberArray)))
		return 0;
	
	return dwRecordNumber;
}

// Prüft, ob die angegebene CD in der Datenbank vorhanden ist.

DWORD CCD::IsCDInDataBase(CArray <long, long&> *pRecordNumberList)
{
   DWORD dwRecordNumber;
   CWaitCursor wait;

   if (!(dwRecordNumber = CheckForIdentity()))     // Identity nicht gefunden
      if (!(dwRecordNumber = CheckForXMCDIdentity()))    // XMCD-Identity nicht gefunden 
         if (!(dwRecordNumber = CheckForOldIdentity( pRecordNumberList)))    // Alte Identity auch nicht gefunden 
            return 0;              // Nix da. CD nicht vorhanden!

   return dwRecordNumber;
}


// Prüft anhand der Identity der CD, ob diese schon in der Datanbank vorhanden ist!
DWORD CCD::CheckForIdentity()
{
	return CheckForIdentity(GetIdentity()->m_sIdentity);
}

// Prüft anhand der Identity der CD, ob diese schon in der Datanbank vorhanden ist!
DWORD CCD::CheckForIdentity(const CString& sIdentity)
{
	if (sIdentity.IsEmpty())
		return 0;

	CIdentity Identity(m_pdb);

	CString sWhere;
	sWhere.Format(L"szIdentity=\"%s\"", sIdentity);
	Identity.QueryStart(sWhere);

	DWORD dwCDID=0;
	if (Identity.QueryFindFirst())
		dwCDID = Identity.m_dwIDCD;

	Identity.QueryEnd();

	return dwCDID;
}

// Prüft anhand der XMCD-Identity der CD, ob diese schon in der Datanbank vorhanden ist!
DWORD CCD::CheckForXMCDIdentity()
{
	CIdentity Identity(m_pdb);

	CString sWhere;
    sWhere.Format(L"szIdentity = \"XMCD%08lx\"", GetCDDBDiscID());
	Identity.QueryStart(sWhere);

	DWORD dwCDID=0;
	if (Identity.QueryFindFirst())
		dwCDID = Identity.m_dwIDCD;

	Identity.QueryEnd();

	return dwCDID;
}

// Prüft anhand des alten Format (Gesamtlänge + Länge der ersten drei Lieder),
// ob die CD schon in der Datenbank ist.
// Da möglicherweise mehrere CDs gefunden werden können, wird eine Liste
// der gefundenen CDs zurückgeliefert.
DWORD CCD::CheckForOldIdentity(CArray <long, long&> *pRecordNumberArray)
{
	CString s;
	DWORD dwRecordNumber = 0;
	CDaoRecordset RecordSet(&m_pdb->m_db);
	COleVariant var;
	
	if (!m_dwTotalLength)
		return 0;
	
	s.Format(L"SELECT CD.IDCD FROM CD WHERE CD.dwGesamtLaenge = %ld", m_dwTotalLength);
	
	RecordSet.Open(dbOpenDynaset, s, dbSeeChanges);
	
	DWORD dwLength1=0, dwLength2=0, dwLength3=0;
	if (m_wNumberOfTracks > 0)
		dwLength1 = GetTrack(0)->m_dwLength;
	if (m_wNumberOfTracks > 1)
		dwLength2 = GetTrack(1)->m_dwLength;
	if (m_wNumberOfTracks > 2)
		dwLength3 = GetTrack(2)->m_dwLength;
	if (!RecordSet.IsEOF())
	{      // Da nur nach Gesamtlaenge gesucht wurde, können mehrere gefunden worden sein.
		s.Format((CString)"SELECT CD.IDCD "+
			"FROM CD, Lied AS L1, Lied AS L2, Lied AS L3 "+
			"WHERE CD.dwGesamtlaenge = %ld " +
			"and (CD.IDCD=L1.IDCD "+
			"and CD.IDCD=L2.IDCD "+
			"and CD.IDCD=L3.IDCD "+
			"and (L1.wLiednummer=1 and L1.dwLaenge = %ld) "+
			"and (L2.wLiednummer=2 and L2.dwLaenge = %ld) "+
			"and (L3.wLiednummer=3 and L3.dwLaenge = %ld))",
			m_dwTotalLength, dwLength1,
			dwLength2, dwLength3);
		RecordSet.Close();
		RecordSet.Open(dbOpenDynaset, s, dbSeeChanges);
		
		while (!RecordSet.IsEOF())
		{
			var = RecordSet.GetFieldValue(L"IDCD");
			if (pRecordNumberArray)
				pRecordNumberArray->Add(var.lVal);
			else
			{
				dwRecordNumber = var.lVal;
				break;
			}
			
			RecordSet.MoveNext();
		}
	}
	
	RecordSet.Close();

	if (pRecordNumberArray && pRecordNumberArray->GetSize() > 0)
		return pRecordNumberArray->GetAt(0);
	else
		return dwRecordNumber;
}

// Schreibweise von Titel und Interpreten anpassen
bool CCD::AdjustSpelling(int iAdjustSpelling)
{
	AdjustString(GetArtist()->m_sArtist, iAdjustSpelling);
	AdjustString(GetComposer()->m_sArtist, iAdjustSpelling);
	AdjustString(m_sTitle, iAdjustSpelling);

	for (int i=0;i<m_wNumberOfTracks;i++)
	{
		AdjustString(GetTrack(i)->GetArtist()->m_sArtist, iAdjustSpelling);
		AdjustString(GetTrack(i)->GetComposer()->m_sArtist, iAdjustSpelling);
		AdjustString(GetTrack(i)->m_sTitle, iAdjustSpelling);
	}

	return true;
}

void CCD::AdjustString(CString& sString, int iAdjustSpelling)
{
	switch (iAdjustSpelling)
	{
	case 0:  // keine Änderung
		break;
	case 1: // Erster Buchstabe groß, rest klein (Ausnahme: Wort "I", englisch für Ich).
		sString.MakeLower();
		if (!sString.IsEmpty())
			sString.SetAt(0, toupper(sString[0]));

		// Ausnahme: Wort "I", englishc für Ich.
		sString.Replace(L" i ", L" I ");
		if (sString.Left(2) == "i ")
			sString = "I " + sString.Mid(2);

		if (sString.Right(2) == " i")
			sString = sString.Left(sString.GetLength()-2) + " I";

		break;
	case 2: // Erster Buchstabe jedes Wortes groß
		{
			bool bWordBegin = true;
			const CString& sWordDelimiters = " ().:,;\"!/[]{}+*-<>=";
			for (int i=0;i<sString.GetLength();i++)
			{
				if (sWordDelimiters.Find((unsigned char)sString[i]) >= 0)
					bWordBegin = true;
				else
				{
					if (isalpha((unsigned char)sString[i]))
					{
						if (bWordBegin)
						{
							sString.SetAt(i, toupper((unsigned char)sString[i]));
							bWordBegin = false;
						}
						else
							sString.SetAt(i, tolower((unsigned char)sString[i]));
					}
				}
			}

			break;
		}
	default:
		ASSERT(FALSE);
		break;
	}
}

COleVariant CCD::GetField(UINT uiField)
{
	COleVariant var;

	switch (uiField)
	{
	case FIELD_TOTALLENGTH:
		var = m_dwTotalLength;
		break;
	case FIELD_NUMBEROFTRACKS:
		var = m_wNumberOfTracks;
		break;
	case FIELD_CDSET:
		var = GetCDSet()->m_sCDSetName;
		break;
	case FIELD_CDSAMPLER:
		var = COleVariant((short)m_bSampler, VT_BOOL);
		break;
	case FIELD_CDNAME:
		var = GetArtist()->m_sArtist;
		break;
	case FIELD_CDTITLE:
		var = m_sTitle;
		break;
	case FIELD_CATEGORY:
		var = GetCategory()->m_sCategory;
		break;
	case FIELD_DATE:
		var = m_sDate;
		break;
	case FIELD_CODES:
		var = m_sCodes;
		break;
	case FIELD_CDCOMMENT:
		var = m_sComment;
		break;
	case FIELD_ARCHIVNUMMER:
		var = m_sArchiveNumber;
		break;
	case FIELD_MEDIUM:
		var = GetMedium()->m_sMedium;
		break;
	case FIELD_YEAR_RECORDED:
		var = m_lYearRecorded;
		break;
	case FIELD_COPYRIGHT:
		var = m_sCopyright;
		break;
	case FIELD_CDCOVER_FILENAME:     // JUS 02.09.2002
		var = m_sBitmapPath;
		break;
	case FIELD_CDUSER1:
		var = m_sUser[0];
		break;
	case FIELD_CDUSER2:
		var = m_sUser[1];
		break;
	case FIELD_CDUSER3:
		var = m_sUser[2];
		break;
	case FIELD_CDUSER4:
		var = m_sUser[3];
		break;
	case FIELD_CDUSER5:
		var = m_sUser[4];
		break;
	case FIELD_CDCOVERBACK_FILENAME:
		var = m_sBackCoverFilename;
		break;
	case FIELD_CDCOVERLABEL_FILENAME:
		var = m_sCDLabelBitmap;
		break;
	case FIELD_ORIGINAL_CD:
		var = COleVariant((short)m_bOriginal, VT_BOOL);
		break;
	case FIELD_LABEL:
		var = m_sLabel;
		break;
	case FIELD_UPC:
		var = m_sUPC;
		break;
	case FIELD_URL:
		var = m_sURL;
		break;
	case FIELD_RATING:
		var = m_lRating;
		break;
	case FIELD_PRICE:
		var = m_lPrice;
		break;
	case FIELD_LANGUAGE:
		var = m_sLanguage;
		break;
	case FIELD_LOCATION:
		var = m_sLocation;
		break;
	case FIELD_ARTIST_CD_SORTKEY:
		var = GetArtist()->m_sSortKey;
		break;
	case FIELD_ARTIST_CD_GROUPTYPE:
		var = GetArtist()->GetGroupString();
		break;
	case FIELD_ARTIST_CD_SEX:
		var = GetArtist()->GetSexString();
		break;
	case FIELD_ARTIST_CD_COMMENT:
		var = GetArtist()->m_sComment;
		break;
	case FIELD_ARTIST_CD_URL:
		var = GetArtist()->m_sURL;
		break;
	case FIELD_ARTIST_CD_COUNTRY:
		var = GetArtist()->m_sCountry;
		break;
	case FIELD_ARTIST_CD_BIRTHDAY:
		var = GetArtist()->m_dtBirthDay;
		break;
	case FIELD_ARTIST_CD_DAYOFDEATH:
		var = GetArtist()->m_dtDayOfDeath;
		break;
	case FIELD_ARTIST_CD_IMAGEFILENAME:
		var = GetArtist()->m_sImageFilename;
		break;
	case FIELD_COMPOSER_CD_NAME:
		var = GetComposer()->m_sArtist;
		break;
	case FIELD_COMPOSER_CD_SORTKEY:
		var = GetComposer()->m_sSortKey;
		break;
	case FIELD_COMPOSER_CD_GROUPTYPE:
		var = GetComposer()->GetGroupString();
		break;
	case FIELD_COMPOSER_CD_SEX:
		var = GetComposer()->GetSexString();
		break;
	case FIELD_COMPOSER_CD_COMMENT:
		var = GetComposer()->m_sComment;
		break;
	case FIELD_COMPOSER_CD_URL:
		var = GetComposer()->m_sURL;
		break;
	case FIELD_COMPOSER_CD_COUNTRY:
		var = GetComposer()->m_sCountry;
		break;
	case FIELD_COMPOSER_CD_BIRTHDAY:
		var = GetComposer()->m_dtBirthDay;
		break;
	case FIELD_COMPOSER_CD_DAYOFDEATH:
		var = GetComposer()->m_dtDayOfDeath;
		break;
	case FIELD_COMPOSER_CD_IMAGEFILENAME:
		var = GetComposer()->m_sImageFilename;
		break;
	default:
		ASSERT(false);
	}

	return var;
}

CString CCD::GetFieldAsString(UINT uiField)
{
	CString sValue;

	switch (uiField)
	{
	case FIELD_TOTALLENGTH:
		sValue = CMisc::Long2Time(m_dwTotalLength);
		break;
	case FIELD_NUMBEROFTRACKS:
		sValue.Format(L"%d", m_wNumberOfTracks);
		break;
	case FIELD_CDSET:
		sValue = GetCDSet()->m_sCDSetName;
		break;
	case FIELD_CDSAMPLER:
		sValue.Format(L"%d", m_bSampler);
		break;
	case FIELD_CDNAME:
		sValue = GetArtist()->m_sArtist;
		break;
	case FIELD_CDTITLE:
		sValue = m_sTitle;
		break;
	case FIELD_CATEGORY:
		sValue = GetCategory()->m_sCategory;
		break;
	case FIELD_DATE:
		sValue = m_pdb->DateShort2Long(m_sDate);
		break;
	case FIELD_CODES:
		sValue = m_sCodes;
		break;
	case FIELD_CDCOMMENT:
		sValue = m_sComment;
		break;
	case FIELD_ARCHIVNUMMER:
		sValue = m_sArchiveNumber;
		break;
	case FIELD_MEDIUM:
		sValue = GetMedium()->m_sMedium;
		break;
	case FIELD_YEAR_RECORDED:
		sValue.Format(L"%d", m_lYearRecorded);
		break;
	case FIELD_COPYRIGHT:
		sValue = m_sCopyright;
		break;
	case FIELD_CDCOVER_FILENAME:     // JUS 02.09.2002
		sValue = m_sBitmapPath;
		break;
	case FIELD_CDUSER1:
		sValue = m_sUser[0];
		break;
	case FIELD_CDUSER2:
		sValue = m_sUser[1];
		break;
	case FIELD_CDUSER3:
		sValue = m_sUser[2];
		break;
	case FIELD_CDUSER4:
		sValue = m_sUser[3];
		break;
	case FIELD_CDUSER5:
		sValue = m_sUser[4];
		break;
	case FIELD_CDCOVERBACK_FILENAME:
		sValue = m_sBackCoverFilename;
		break;
	case FIELD_CDCOVERLABEL_FILENAME:
		sValue = m_sCDLabelBitmap;
		break;
	case FIELD_ORIGINAL_CD:
		sValue.Format(L"%d", m_bOriginal);
		break;
	case FIELD_LABEL:
		sValue = m_sLabel;
		break;
	case FIELD_UPC:
		sValue = m_sUPC;
		break;
	case FIELD_URL:
		sValue = m_sURL;
		break;
	case FIELD_RATING:
		sValue.Format(L"%d", m_lRating);
		break;
	case FIELD_PRICE:
		sValue = CMisc::FormatCurrencyValue(m_lPrice);
		break;
	case FIELD_LOCATION:
		sValue = m_sLocation;
		break;
	case FIELD_LANGUAGE:
		sValue = m_sLanguage;
		break;
	case FIELD_ARTIST_CD_SORTKEY:
		sValue = GetArtist()->m_sSortKey;
		break;
	case FIELD_ARTIST_CD_GROUPTYPE:
		sValue = GetArtist()->GetGroupString();
		break;
	case FIELD_ARTIST_CD_SEX:
		sValue = GetArtist()->GetSexString();
		break;
	case FIELD_ARTIST_CD_COMMENT:
		sValue = GetArtist()->m_sComment;
		break;
	case FIELD_ARTIST_CD_URL:
		sValue = GetArtist()->m_sURL;
		break;
	case FIELD_ARTIST_CD_COUNTRY:
		sValue = GetArtist()->m_sCountry;
		break;
	case FIELD_ARTIST_CD_BIRTHDAY:
		sValue = GetArtist()->m_dtBirthDay.Format(VAR_DATEVALUEONLY);
		break;
	case FIELD_ARTIST_CD_DAYOFDEATH:
		sValue = GetArtist()->m_dtDayOfDeath.Format(VAR_DATEVALUEONLY);
		break;
	case FIELD_ARTIST_CD_IMAGEFILENAME:
		sValue = GetArtist()->m_sImageFilename;
		break;
	case FIELD_COMPOSER_CD_NAME:
		sValue = GetComposer()->m_sArtist;
		break;
	case FIELD_COMPOSER_CD_SORTKEY:
		sValue = GetComposer()->m_sSortKey;
		break;
	case FIELD_COMPOSER_CD_GROUPTYPE:
		sValue = GetComposer()->GetGroupString();
		break;
	case FIELD_COMPOSER_CD_SEX:
		sValue = GetComposer()->GetSexString();
		break;
	case FIELD_COMPOSER_CD_COMMENT:
		sValue = GetComposer()->m_sComment;
		break;
	case FIELD_COMPOSER_CD_URL:
		sValue = GetComposer()->m_sURL;
		break;
	case FIELD_COMPOSER_CD_COUNTRY:
		sValue = GetComposer()->m_sCountry;
		break;
	case FIELD_COMPOSER_CD_BIRTHDAY:
		sValue = GetComposer()->m_dtBirthDay.Format(VAR_DATEVALUEONLY);
		break;
	case FIELD_COMPOSER_CD_DAYOFDEATH:
		sValue = GetComposer()->m_dtDayOfDeath.Format(VAR_DATEVALUEONLY);
		break;
	case FIELD_COMPOSER_CD_IMAGEFILENAME:
		sValue = GetComposer()->m_sImageFilename;
		break;
	default:
		ASSERT(false);
	}

	return sValue;
}

void CCD::SetField(UINT uiField, const COleVariant& var)
{
	switch (uiField)
	{
	case FIELD_TOTALLENGTH:
		m_dwTotalLength = var.lVal;
		break;
	case FIELD_NUMBEROFTRACKS:
		m_wNumberOfTracks = var.iVal;
		break;
	case FIELD_CDSET:
		GetCDSet()->m_sCDSetName = var.bstrVal;
		break;
	case FIELD_CDSAMPLER:
		m_bSampler = var.boolVal;
		break;
	case FIELD_CDNAME:
		GetArtist()->m_sArtist = var.bstrVal;
		break;
	case FIELD_COMPOSER_CD_NAME:
		GetComposer()->m_sArtist = var.bstrVal;
		break;
	case FIELD_CDTITLE:
		m_sTitle = var.bstrVal;
		break;
	case FIELD_CATEGORY:
		GetCategory()->m_sCategory = var.bstrVal;
		break;
	case FIELD_DATE:
		m_sDate = var.bstrVal;
		break;
	case FIELD_CODES:
		m_sCodes = var.bstrVal;
		break;
	case FIELD_CDCOMMENT:
		m_sComment = var.bstrVal;
		break;
	case FIELD_ARCHIVNUMMER:
		m_sArchiveNumber = var.bstrVal;
		break;
	case FIELD_MEDIUM:
		GetMedium()->m_sMedium = var.bstrVal;
		break;
	case FIELD_YEAR_RECORDED:
		m_lYearRecorded = var.lVal;
		break;
	case FIELD_COPYRIGHT:
		m_sCopyright = var.bstrVal;
		break;
	case FIELD_CDCOVER_FILENAME:
		m_sBitmapPath = var.bstrVal;
		break;
	case FIELD_CDUSER1:
		m_sUser[0] = var.bstrVal;
		break;
	case FIELD_CDUSER2:
		m_sUser[1] = var.bstrVal;
		break;
	case FIELD_CDUSER3:
		m_sUser[2] = var.bstrVal;
		break;
	case FIELD_CDUSER4:
		m_sUser[3] = var.bstrVal;
		break;
	case FIELD_CDUSER5:
		m_sUser[4] = var.bstrVal;
		break;
	case FIELD_CDCOVERBACK_FILENAME:
		m_sBackCoverFilename = var.bstrVal;
		break;
	case FIELD_CDCOVERLABEL_FILENAME:
		m_sCDLabelBitmap = var.bstrVal;
		break;
	case FIELD_ORIGINAL_CD:
		m_bOriginal = var.boolVal;
		break;
	case FIELD_LABEL:
		m_sLabel = var.bstrVal;
		break;
	case FIELD_UPC:
		m_sUPC = var.bstrVal;
		break;
	case FIELD_URL:
		m_sURL = var.bstrVal;
		break;
	case FIELD_RATING:
		m_lRating = var.lVal;
		break;
	case FIELD_PRICE:
		m_lPrice = var.lVal;
		break;
	case FIELD_LANGUAGE:
		m_sLanguage = var.bstrVal;
		break;
	case FIELD_LOCATION:
		m_sLocation = var.bstrVal;
		break;
	default:
		ASSERT(false);
	}
}

// Die Gesamtlänge der CD neu berechnen
void CCD::UpdateTotalTime(void)
{
	int i;
	
	m_dwTotalLength = 0L;
	for (i=0;i<m_wNumberOfTracks;i++)
		m_dwTotalLength += GetTrack(i)->m_dwLength;
}


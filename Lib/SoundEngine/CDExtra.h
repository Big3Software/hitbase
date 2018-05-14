#pragma once

class CCDExtra
{
public:
	CCDExtra();
	BOOL ReadInformation(const CString& drive);
	
	// Alle möglichen Informationen der CD-Extra (* = Pflicht!)
	// Date_string = YYYYMMDD
	CStringArray m_AlbumTitel;                // (*) String
	CStringArray m_UPC;                       // String: Universal Product Code
	CStringArray m_ISBN;                      // String
	CStringArray m_Copyright;                 // String 
	CStringArray m_TrackTitel;                // (*) String
	CStringArray m_Notes;                     // String
	CStringArray m_HauptInterpret;            // (*) String
	CStringArray m_ZweiterInterpret;          // String
	CStringArray m_Komponist;                 // String
	CStringArray m_KomponistOriginal;         // String
	CStringArray m_Schaffensdatum;            // Date_string
	CStringArray m_Veroeffentlichungsdatum;   // Date_string
	CStringArray m_Herausgeber;               // String
	CArray <short, short> m_Kategorie;       // Bis zu vier 2-Byte-Werte
	CArray <short, short> m_Bpm;             // Takte/Minute
	CArray <short, short> m_Tonart;          // 1 Byte (Format??)

	BOOL m_bSampler;
};


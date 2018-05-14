#if ! defined(_COVERLAYOUT_)
#define _COVERLAYOUT_

#include <vector>
#include "textelement.h"
#include "covergrafik.h"

class CCD;


// alte defines...Hitbase 98...

#define COVER_LAYOUT_ID					0x594c4443		// 4-Byte-ID
#define COVER_LAYOUT_VER				4				// cly-Datei Version
#define COVER_RANDTEXT_DEFAULT			0				// Standard
#define COVER_RANDTEXT_EQUAL			1				// Beide Texte gleich
#define COVER_RANDTEXT_INTERPRETTITEL	2				// Texte werden erzeugt aus Interpret-Titel
#define COVER_BITMAP_TEXTONLY			0				// Nur Text
#define COVER_BITMAP_BMPONLY_SMALL		1				// Nur Grafik (nur in der Mittelbox)
#define COVER_BITMAP_BOTH_SMALL			2				// Bitmap und Text (nur in der Mittelbox)
#define COVER_BITMAP_BMPONLY_LARGE		3				// Nur Grafik (über die Seitenränder)
#define COVER_BITMAP_BOTH_LARGE			4				// Bitmap und Text (über die Seitenränder)

#define COVER_FIELD_EMPTY			0
#define COVER_FIELD_TRACKNUMBER		1
#define COVER_FIELD_INTERPRET		2
#define COVER_FIELD_SONGNAME		3
#define COVER_FIELD_TRACKLENGHT		4
#define COVER_FIELD_SEPARATOR		5
#define COVER_FIELD_BPM				6
#define COVER_FIELD_USER_1			7
#define COVER_FIELD_USER_2			8
#define COVER_FIELD_COMMENT			9
#define COVER_FIELD_STARTPOSITION	10
#define COVER_FIELD_MARK			11



class CCoverFormat
{
	public:

		CCoverFormat();
	
	public:

		CString m_strNummer;
		int m_nFX;
		int m_nFY;
		int m_nBX;
		int m_nBY;
		int m_nLX;
		int m_nLY;
};

class CCoverZweckform
{
	public:

		CCoverZweckform();
	
	public:

		CString m_strFirma;
		std::vector<CCoverFormat> m_vecFormate;
};

class CCoverTextBox
{
	public:

		CCoverTextBox();
		~CCoverTextBox();
		CCoverTextBox(CRect rect);
		bool operator== (const CCoverTextBox& tb);
		CCoverTextBox& operator= (const CCoverTextBox& tb);
		CCoverTextBox(const CCoverTextBox& tb);


		enum TEXTBOX_TYPE {text, cdtitle, cdartist, grafik, gesamtlen, anzahllieder, cdsetnr, cdsetname, kategorie, medium, datum, archivnummer, codes, kommentar, aufnahmejahr, copyright, user0, user1, user2, user3, user4};

	public:

		BOOL Load(CArchive& ar, const int nVer);
		BOOL LoadOldFormat(CArchive& ar, const int nLines);
		BOOL Save(CArchive& ar);
		BOOL AddGfx(CWnd *pWnd);
		void DelGfx();

	public:

		int m_nGfxIndex;
		CString m_strGfxName;
		BOOL m_bGfx;

		BOOL m_bIsFlipped;
		BOOL m_bIsMirrored;

		BOOL m_bTransparent;
		COLORREF m_colBack;
		COLORREF m_colBorder;
		int m_nType;
		CRect m_rectArea;
		CTextElement m_teText;
		std::vector<CString> m_vecTextlines;
};

class CCoverFrame
{
	public:

		CCoverFrame();

	public:

		COLORREF m_colBorder;
		COLORREF m_colBackground;
		CRect m_rectFrame;
};


class CCoverColumn
{
	public:

		CCoverColumn();
		bool operator== (const CCoverColumn& column);

		enum COVER_FIELD {TRACKNR, ARTIST, TITLE, LENGTH, NUMBER, BPM, CODES, COMMENT, CDTITLE, ARCHIVNR, LYRICS, SOUNDFILE, FILENAME, USER1, USER2, USER3, USER4, USER5, SEPARATOR, FREI};

	public:

		int m_nType;
		CTextElement m_teColumn;
		double m_dWidthRel;
	public:
		BOOL Save(CArchive& ar);
		BOOL Load(CArchive& ar);
};


class CCoverLayout
{
	public:

		CCoverLayout();
		~CCoverLayout();

		bool operator== (const CCoverLayout& gfx);


	public:

		void ResetDrawPos();

		BOOL ParseEngineData(const CString& strFileName, std::vector<CCoverZweckform> *pvecZweckformen);

		BOOL Save(const CString& strFileName);
		BOOL Load(const CString& strFileName);
		void ToggleTextGfxLabel();
		void ToggleTextGfxBack();
		void ToggleTextGfxFront();
		void ToggleBorderJoin();
		void ToggleColumnsJoin();
		void ToggleZoom();
		void ToggleBorder();
		static CString FileBrowser(const CString& strExtension, CString strFileName, const CString& strMask, CWnd *pWnd, BOOL bLoad);
		static CString GetPureFileName(const CString& strPath);

		BOOL AddGfxLabel(CWnd *pWnd);
		void DelGfxLabel();
		BOOL AddGfxBack(CWnd *pWnd);
		void DelGfxBack();
		BOOL AddGfxFront(CWnd *pWnd);
		void DelGfxFront();

		void SetCoverMode(const int nMode);
		BOOL SetZoomFactor(int nZoomFactor);
		int GetCoverMode();
		BOOL ZoomEnabled();
		
		enum COVERMODE {front, back, both, label, nothing};

	private:

		int GetNextWord(CStdioFile *stioFile, CString *strWord);
		BOOL LoadOldLayout(CArchive& ar);

	private:

		int m_nCoverMode;


	public:

		std::vector<CCoverGrafik> m_vecGfxLookup;

		CCoverGrafik m_gfxFront;
		CCoverGrafik m_gfxBack;
		CCoverGrafik m_gfxLabel;

		CString m_strCurrentLayoutName;


		BOOL m_bDrawBorders;

		int m_nRandX;
		int m_nRandY;

		BOOL m_bBorderToArtistTitle;

		BOOL m_bJoinBorderTexts;
		BOOL m_bJoinColumns;

		int m_nZoomFactor;
		BOOL m_bZoomMode;

		BOOL m_bGfxBackBorder;
		BOOL m_bTextGfxBack;
		BOOL m_bTextGfxFront;
		BOOL m_bTextGfxLabel;
		BOOL m_bBackGfx;
		BOOL m_bFrontGfx;
		BOOL m_bLabelGfx;
		CString m_strBackGfx;
		CString m_strFrontGfx;
		CString m_strLabelGfx;

		int m_nBackYAlignment;
		int m_nBackXAlignment;

		gcroot<Big3::Hitbase::DataBaseEngine::CD^> m_pccd;
		gcroot<Big3::Hitbase::DataBaseEngine::DataBase^> m_pdb;

		CCoverTextBox *m_pActiveTB;
		int m_nActiveTBIndex;

		CRect *m_pActiveRect;
		CRect *m_pLastActiveRect;
		CRect *m_pActiveMainRect;
		CRect m_rectLeft;
		CRect m_rectRight;
		CRect m_rectBack;
		CRect m_rectFront;
		CRect m_rectArtist;
		CRect m_rectTitle;
		CRect m_rectLabel;
		CRect m_rectInnerLabel;
	
		CTextElement m_teBorderLeft;
		CTextElement m_teBorderRight;

		std::vector<CCoverColumn> m_vecColumns;

		std::vector<CCoverTextBox> m_vecTBBack;
		std::vector<CCoverTextBox> m_vecTBFront;
		std::vector<CCoverTextBox> m_vecTBLabel;

		std::vector<CCoverTextBox> m_tbsF;
		std::vector<CCoverTextBox> m_tbsB;
		std::vector<CCoverTextBox> m_tbsL;

		int m_nCurrentArea;

		CTextElement m_teArtist;
		CTextElement m_teCDTitle;

		BOOL m_bShowFrontArtist;
		BOOL m_bShowFrontCDTitle;

		COLORREF m_colBorderLeftRand;
		COLORREF m_colBorderRightRand;
		COLORREF m_colBorderLeftBack;
		COLORREF m_colBorderRightBack;
		COLORREF m_colBack;
		COLORREF m_colBackRand;
		COLORREF m_colFront;
		COLORREF m_colFrontRand;
		COLORREF m_colLabel;
		COLORREF m_colLabelRand;

		int m_nCDTitleFrontYFreeValue;
		int m_nArtistFrontYFreeValue;
		int m_nCDTitleFrontXFreeValue;
		int m_nArtistFrontXFreeValue;

		int m_nFrontX;
		int m_nFrontY;
		int m_nBackX;
		int m_nBackY;
		int m_nLabelX;
		int m_nLabelY;

		int m_nZweckformIndex;
		int m_nZweckformFirmaIndex;
		BOOL m_bZweckformUser;

		CString m_strTrenn;

		BOOL m_bLeftBorder_AT;
		BOOL m_bRightBorder_AT;
		BOOL m_bLeftBorder_Dominate;
		BOOL m_bRightBorder_Dominate;

		std::vector<CCoverZweckform> m_vecZweckformen;

		int gl_COVERBACKWIDTH;// = 1380;
		int gl_COVERBACKHEIGHT;// = 1180;
		int gl_COVERFRONTWIDTH;// = 1200;
		int gl_COVERFRONTHEIGHT;// = 1200;
		int gl_COVERBORDERWIDTH;// = 65;
		int gl_COVERLABELHEIGHT;// = 1180;
		int gl_COVERLABELWIDTH;// = 1180;
		int gl_COVERINNERLABELRADIUS;// = 410;


};


#endif

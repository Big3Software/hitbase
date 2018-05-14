#pragma once

// CHTMLTemplate

class CSelection;
class CDataBase;
class CCDQuery;

class CHTMLTemplate  : CObject
{
public:
	CHTMLTemplate(Big3::Hitbase::DataBaseEngine::DataBase^ db);
	bool Open(CString sTemplate, CString sHTMLExportFile); // Open Template and Export File
	bool WriteCD2HTML (Big3::Hitbase::DataBaseEngine::CD^ CD);
	bool WriteTrack2HTML (Big3::Hitbase::DataBaseEngine::TrackDataView^ Track, int index);
	bool Close ();
	virtual ~CHTMLTemplate();
	BOOL DoExport(Big3::Hitbase::DataBaseEngine::Condition^ condition, Big3::Hitbase::DataBaseEngine::SortFieldCollection^ sortFields, const CString& sHTMLTemplate, const CString& sHTMLOutput);

protected:
    DWORD m_dwAnzahlLieder;
	DWORD m_dwVerliehendeCDs;
	DWORD m_dwAnzahlCDs;
	DWORD m_dwAnzahlInterpreten;
	DWORD m_dwCounter;

	CFile m_cfTemplate;
    CString m_sTemplate;
	CFile m_cfHTMLExport;
	CString m_sHTMLExportFile;

	CString m_sCodeHTMLHeader;
	CString m_sCodeHTMLFooter;
	CString m_sCodeHTMLCDHeader;
	CString m_sCodeHTMLCDFooter;
	CString m_sCodeHTMLTitleHeader;
	CString m_sCodeHTMLTitleFooter;
	CString m_sCodeHTMLTRACK;

	bool WriteBuffer (CString sBuffer, Big3::Hitbase::DataBaseEngine::CD^ CD, int nTrack = -1);
	bool WriteBuffer (CString sBuffer, Big3::Hitbase::DataBaseEngine::TrackDataView^ Track, int index);
	bool WriteBuffer (CString sBuffer);

	bool ReplaceKeywords (CString &sBuffer, Big3::Hitbase::DataBaseEngine::CD^ CD, int nTrack);
	void ReplaceKeywords (CString &sBuffer, Big3::Hitbase::DataBaseEngine::TrackDataView^ Track, int index);
	void ReplaceKeywords(CString& sBuffer, Big3::Hitbase::DataBaseEngine::Track^ track);
	void ReplaceGlobalKeywords (CString &sBuffer);
	CString Text2HTMLText(CString sText);

private:
	gcroot<Big3::Hitbase::DataBaseEngine::DataBase^> DataBase;
};

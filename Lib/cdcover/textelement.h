#if ! defined(_TEXTELEMENT_)
#define _TEXTELEMENT_


class CTextElement
{
	public:

		CTextElement();
		CTextElement(const CString& strText);
		CTextElement(const CTextElement&);


	private:

		BOOL LoadLOGFONT(CArchive& ar, LOGFONT *plf);
		BOOL SaveLOGFONT(CArchive& ar, LOGFONT *plf);


	public:

		void Draw(CDC *pDC);
		BOOL OldLoad(CArchive& ar);
		BOOL Save(CArchive& ar);
		BOOL Load(CArchive& ar);

	public:

		CTextElement& operator= (const CTextElement& te);
		bool operator== (const CTextElement& te);

		enum TEXTELEMENT_ALIGNMENT {top, bottom, left, right, center, hook, userdef};

		CString m_strText;

		int m_nBkMode;
		int m_nXAlignment;
		int m_nYAlignment;

		int m_nXpos;
		int m_nYpos;

		COLORREF m_colText;
		COLORREF m_colTextbk;
		
		LOGFONT m_lfFont;
	
};

#endif
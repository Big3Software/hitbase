#if ! defined(_CDCOVER_)
#define _CDCOVER_

#include <vector>
#include "coverlayout.h"


class CCover
{
	public:
		
		CCover(CDC *pDC, const int XPos, const int YPos, const int nRandX, const int nRandY);

	public:

		static int GetRealFontHeight(CDC *pDC, LOGFONT *plogfont);

		void DrawFrames(const BOOL bDrawFrames, CDC *pDC, const BOOL bFill = TRUE);
		void DrawTexts(CDC *pDC);
		void DrawTB(CCoverLayout *pLayout, std::vector<CCoverTextBox> *pvecTBs, const BOOL bTextGfx, const int nXOffset, CDC *pDC);
		CString TextFitRectX(CDC *pDC, CTextElement *pte, int nWidth, CSize *psizeExtend = NULL);
		CString GetStringFromColumn(CCoverColumn *pColumn, int t, Big3::Hitbase::DataBaseEngine::Track^ CurrentTrack);
		static CString TimeToString(const long nMiliSek);
		int AlignString(CDC *pDC, CTextElement *pte, const int nMode, int nWidth, CSize *psizeExtend = NULL);
		int AlignStringY(CDC *pDC, CTextElement *pte, const int nMode, int nHeight, CSize *psizeExtend = NULL);

	public:

		std::vector<CTextElement> m_vecTextElements;
		std::vector<CCoverFrame> m_vecFrames;
		std::vector<CCoverFrame> m_vecCircles;
		int m_nXPos;
		int m_nYPos;
};



class CBackCover : public CCover
{
	public:

		CBackCover(CDC *pDC, const int XPos, const int YPos, const int nRandX, const int nRandY);

	public:

		int DrawColumn(const BOOL bDraw, const int nYPos, Big3::Hitbase::DataBaseEngine::Track^ pCurrentTrack, const int t, const double dFontHeight, CCoverLayout *pLayout, const int nXOffset, CDC *pDC);
		void CreateCoverFromLayout(CCoverLayout *pLayout, CDC *pDC);
		void Draw(CCoverLayout *pLayout, CDC *pDC);
		CRect GetLeftRect();
		CRect GetRightRect();
		CRect GetBackRect();

	private:

		CRect m_rectLeft;
		CRect m_rectRight;
		CRect m_rectBack;
};

class CLabelCover : public CCover
{
	
	public:

		CLabelCover(CDC *pDC, const int XPos, const int YPos, const int nRandX, const int nRandY);

	public:

		void CreateCoverFromLayout(CCoverLayout *pLayout);
		void Draw(CCoverLayout *pLayout, CDC *pDC);
		CRect GetLabelRect();
		CRect GetInnerLabelRect();

	private:

		CRect m_rectLabel;
		CRect m_rectInnerLabel;
};

class CFrontCover : public CCover
{
	public:

		CFrontCover(CDC *pDC, const int XPos, const int YPos, const int nRandX, const int nRandY);

	public:

		void Draw(CCoverLayout *pLayout, CDC *pDC);
		void CreateCoverFromLayout(CCoverLayout *pLayout, CDC *pDC);
		CRect GetFrontRect();
		CRect GetArtistRect();
		CRect GetTitleRect();

	private:

		CRect m_rectFront;
		CRect m_rectArtist;
		CRect m_rectTitle;
};


#endif

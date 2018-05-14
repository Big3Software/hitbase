#if ! defined(_COVERGRAFIK_)
#define _COVERGRAFIK_

class BTCImageObject;
class BTCImageData;


class CCoverGrafik
{
	public:

		CCoverGrafik();
		~CCoverGrafik();
		CCoverGrafik(const CCoverGrafik& gfx);
		CCoverGrafik& operator= (const CCoverGrafik& gfx);


		bool operator < (const CCoverGrafik& cur) const;
		bool operator== (const CCoverGrafik& gfx);

		void Kill();
		void ApplyEffects();
		BOOL Init(const CString& strFileName);
		void Draw(CRect *pRect, CDC *pDC);

	public:
		
		BOOL m_bInitComplete;

		CString m_strFileName;

		double m_dAspectRatio;

		BOOL m_bFlip;
		BOOL m_bMirror;
		BOOL m_bApplyEffectsOnce;

		BTCImageObject *m_pgfxObj;
		BTCImageData *m_pgfxData;

		CRect m_rectSource;
		int m_nSourceHeight;
		int m_nSourceWidth;
};

#endif

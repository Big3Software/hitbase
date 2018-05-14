#include "stdafx.h"
#include "covergrafik.h"
#include "CoverBTCIFStream.h"


CCoverGrafik::CCoverGrafik()
{
	m_bInitComplete = FALSE;
	m_bApplyEffectsOnce = FALSE;
	
	m_rectSource = CRect(0,0,0,0);
	m_nSourceHeight = 0;
	m_nSourceWidth = 0;

	m_pgfxObj = NULL;
	m_pgfxData = NULL;

	m_bFlip = FALSE;
	m_bMirror = FALSE;
	m_strFileName = "";
	m_dAspectRatio = (double)0.0;
}

CCoverGrafik::CCoverGrafik(const CCoverGrafik& gfx)
{
	m_bApplyEffectsOnce = gfx.m_bApplyEffectsOnce;
	m_bFlip = gfx.m_bFlip;
	m_bMirror = gfx.m_bMirror;
	m_strFileName = gfx.m_strFileName;
	m_rectSource = gfx.m_rectSource;
	m_nSourceHeight = gfx.m_nSourceHeight;
	m_nSourceWidth = gfx.m_nSourceWidth;
	m_bInitComplete = FALSE;
	m_pgfxObj = NULL;
	m_pgfxData = NULL;
	m_dAspectRatio = (double)0.0;
	if (m_strFileName != "")
		Init(m_strFileName);
}


CCoverGrafik& CCoverGrafik::operator= (const CCoverGrafik& gfx)
{
	m_bApplyEffectsOnce = gfx.m_bApplyEffectsOnce;
	m_bFlip = gfx.m_bFlip;
	m_bMirror = gfx.m_bMirror;
	m_strFileName = gfx.m_strFileName;
	m_rectSource = gfx.m_rectSource;
	m_nSourceHeight = gfx.m_nSourceHeight;
	m_nSourceWidth = gfx.m_nSourceWidth;
	m_bInitComplete = FALSE;
	m_pgfxObj = NULL;
	m_pgfxData = NULL;
	if (m_strFileName != "")
		Init(m_strFileName);
	return *this;
}




CCoverGrafik::~CCoverGrafik()
{
	Kill();
}

bool CCoverGrafik::operator < (const CCoverGrafik& cur) const
{
	return m_strFileName < cur.m_strFileName;
}


bool CCoverGrafik::operator== (const CCoverGrafik& gfx)
{
	bool bReturn = TRUE;

	bReturn &= (gfx.m_bApplyEffectsOnce == m_bApplyEffectsOnce);
	bReturn &= (gfx.m_bFlip == m_bFlip);
	bReturn &= (gfx.m_bMirror == m_bMirror);
	bReturn &= (gfx.m_bInitComplete == m_bInitComplete);
	bReturn &= (gfx.m_nSourceWidth == m_nSourceWidth);
	bReturn &= (gfx.m_nSourceHeight == m_nSourceHeight);
	if (gfx.m_rectSource != m_rectSource)
		bReturn = false;
	bReturn &= (gfx.m_strFileName == m_strFileName);

	return bReturn;
}


BOOL CCoverGrafik::Init(const CString& strFileName)
{
	if (! m_bInitComplete && (m_pgfxObj != NULL || m_pgfxData != NULL))
	{
		Kill();
//		return FALSE;
	}

	if (m_bInitComplete && (m_pgfxObj == NULL || m_pgfxData == NULL))
	{
		m_bInitComplete = FALSE;
		return FALSE;
	}

	if (m_bInitComplete)
	{
		if (m_bApplyEffectsOnce)
		{
			m_bApplyEffectsOnce = FALSE;
			ApplyEffects();
		}
		return TRUE;
	}

	BOOL bReturn = FALSE;

	m_pgfxObj = new BTCImageObject();

	if (m_pgfxObj != NULL)
	{
		CoverBTCIFStream MyInputStream;

		if (MyInputStream.Open(strFileName))
		{
			if (m_pgfxObj->Load( &MyInputStream, NULL))
			{
				m_pgfxData = m_pgfxObj->GetObjectDataPtr();

				if (m_pgfxData != NULL)
				{
					m_rectSource.top = 0;
					m_rectSource.left = 0;
					m_rectSource.right = m_pgfxData->GetWidth();
					m_rectSource.bottom = m_pgfxData->GetHeight();
					m_nSourceWidth = m_rectSource.Width();
					m_nSourceHeight = m_rectSource.Height();

					m_dAspectRatio = (double) ((double)m_rectSource.Width() / (double) m_rectSource.Height());

					m_bInitComplete = TRUE;
					bReturn = TRUE;
				}
			}
	
			MyInputStream.Close();
		}
	}

	if (! bReturn)
	{
		delete m_pgfxObj;
		m_pgfxObj = NULL;
	}
	return bReturn;
}

void CCoverGrafik::Kill()
{
	if (m_pgfxData != NULL)
	{
		m_pgfxData->DeleteObject();
		m_pgfxData = NULL;
	}

	if (m_pgfxObj != NULL)
	{
		delete m_pgfxObj;
		m_pgfxObj = NULL;
	}

	m_bApplyEffectsOnce = TRUE;
	m_bInitComplete = FALSE;

	return;
}

void CCoverGrafik::ApplyEffects()
{
	if (m_pgfxData == NULL)
		return;

	if (m_bFlip)
	{
		m_pgfxData->Flip();
		m_bFlip = FALSE;
	}

	if (m_bMirror)
	{
		m_pgfxData->Mirror();
		m_bMirror = FALSE;
	}

	return;
}

void CCoverGrafik::Draw(CRect *pRect, CDC *pDC)
{
	if (m_bApplyEffectsOnce)
	{
		m_bApplyEffectsOnce = FALSE;
		ApplyEffects();
	}

	if (m_pgfxData != NULL)
    	m_pgfxData->Stretch(pDC->m_hDC, pRect->left, pRect->top, pRect->Width(), pRect->Height(), m_rectSource.left, m_rectSource.top, m_nSourceWidth, m_nSourceHeight);

	return;
}

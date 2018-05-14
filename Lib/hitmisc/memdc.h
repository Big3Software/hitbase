#ifndef _MEMDC_H_
#define _MEMDC_H_
 
//////////////////////////////////////////////////
// CMemDC - memory DC
//
// Author: Keith Rule
// Email:  keithr@europa.com
// Copyright 1996-2002, Keith Rule
//
// You may freely use or modify this code provided this
// Copyright is included in all derived versions.
//
// History - 10/3/97 Fixed scrolling bug.
//               Added print support. - KR
//
//       11/3/99 Fixed most common complaint. Added
//            background color fill. - KR
//
//       11/3/99 Added support for mapping modes other than
//            MM_TEXT as suggested by Lee Sang Hun. - KR
//
//       02/11/02 Added support for CScrollView as supplied
//             by Gary Kirkham. - KR
//
// This class implements a memory Device Context which allows
// flicker free drawing.
 
class CMemoryDC : public CDC {
private:       
    CBitmap    m_bitmap;        // Offscreen bitmap
    CBitmap*   m_oldBitmap; // bitmap originally found in CMemDC
    CDC*       m_pDC;           // Saves CDC passed in constructor
    CRect      m_rect;          // Rectangle of drawing area.
    BOOL       m_bMemDC;        // TRUE if CDC really is a Memory DC.
	BOOL	   m_bAlphaBlend;
	int        m_iTransparency;
public:
    
    CMemoryDC(CDC* pDC, const CRect* pRect = NULL, BOOL bAlphaBlend = FALSE, int iTransparency = -1) : CDC()
    {
        ASSERT(pDC != NULL); 
 
        // Some initialization
        m_pDC = pDC;
		m_bAlphaBlend = bAlphaBlend;
		m_iTransparency = iTransparency;
        m_oldBitmap = NULL;
        m_bMemDC = !pDC->IsPrinting();
 
        // Get the rectangle to draw
        if (pRect == NULL) {
             pDC->GetClipBox(&m_rect);
        } else {
             m_rect = *pRect;
        }
 
        if (m_bMemDC) {
             // Create a Memory DC
             CreateCompatibleDC(pDC);
             pDC->LPtoDP(&m_rect);
 
             m_bitmap.CreateCompatibleBitmap(pDC, m_rect.Width(), 
                                                  m_rect.Height());
             m_oldBitmap = SelectObject(&m_bitmap);
 
             SetMapMode(pDC->GetMapMode());
 
             SetWindowExt(pDC->GetWindowExt());
             SetViewportExt(pDC->GetViewportExt());
 
             pDC->DPtoLP(&m_rect);
             SetWindowOrg(m_rect.left, m_rect.top);
        } else {
             // Make a copy of the relevent parts of the current 
             // DC for printing
             m_bPrinting = pDC->m_bPrinting;
             m_hDC       = pDC->m_hDC;
             m_hAttribDC = pDC->m_hAttribDC;
        }
 
        // Fill background 
        FillSolidRect(m_rect, pDC->GetBkColor());
    }

    ~CMemoryDC()      
    {        
//		CopyBuffer();

        if (m_bMemDC) {
             // Copy the offscreen bitmap onto the screen.
			if (m_bAlphaBlend)
			{
				BLENDFUNCTION bf;
				bf.AlphaFormat = 0;
				if (m_iTransparency < 0)
					bf.SourceConstantAlpha = 110;			// 110 = Default
				else
					bf.SourceConstantAlpha = m_iTransparency;
				bf.BlendFlags = 0;
				bf.BlendOp = AC_SRC_OVER;
				m_pDC->AlphaBlend(m_rect.left, m_rect.top, m_rect.Width(), m_rect.Height(), this, m_rect.left, m_rect.top, m_rect.Width(), m_rect.Height(), bf);
			}
			else
			{
				m_pDC->BitBlt(m_rect.left, m_rect.top, 
							m_rect.Width(),  m_rect.Height(),
					this, m_rect.left, m_rect.top, SRCCOPY);            
			}
             
             //Swap back the original bitmap.
             SelectObject(m_oldBitmap);        
        } else {
             // All we need to do is replace the DC with an illegal
             // value, this keeps us from accidentally deleting the 
             // handles associated with the CDC that was passed to 
             // the constructor.              
             m_hDC = m_hAttribDC = NULL;
        }       
    }
    
    // Allow usage as a pointer    
    CMemoryDC* operator->() 
    {
        return this;
    }       
 
    // Allow usage as a pointer    
    operator CMemoryDC*() 
    {
        return this;
    }
};
 
#endif
// cdcoverDoc.cpp : implementation of the CCdcoverDoc class
//

#include "stdafx.h"
#include "cdcover.h"

#include "cdcoverDoc.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CCdcoverDoc

IMPLEMENT_DYNCREATE(CCdcoverDoc, CDocument)

BEGIN_MESSAGE_MAP(CCdcoverDoc, CDocument)
	//{{AFX_MSG_MAP(CCdcoverDoc)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CCdcoverDoc construction/destruction

CCdcoverDoc::CCdcoverDoc()
{
}

CCdcoverDoc::~CCdcoverDoc()
{
}

BOOL CCdcoverDoc::OnNewDocument()
{
	if (!CDocument::OnNewDocument())
		return FALSE;

	return TRUE;
}



/////////////////////////////////////////////////////////////////////////////
// CCdcoverDoc serialization

void CCdcoverDoc::Serialize(CArchive& ar)
{
	if (ar.IsStoring())
	{
	}
	else
	{
	}
}

/////////////////////////////////////////////////////////////////////////////
// CCdcoverDoc diagnostics

#ifdef _DEBUG
void CCdcoverDoc::AssertValid() const
{
	CDocument::AssertValid();
}

void CCdcoverDoc::Dump(CDumpContext& dc) const
{
	CDocument::Dump(dc);
}
#endif //_DEBUG

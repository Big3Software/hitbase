// DlgCoverSizes.cpp : implementation file
//

#include "stdafx.h"
#include "cdcover.h"
#include "DlgCoverSizes.h"
#include "cover.h"
#include ".\dlgcoversizes.h"

// CDlgCoverSizes dialog

IMPLEMENT_DYNAMIC(CDlgCoverSizes, CDialog)
CDlgCoverSizes::CDlgCoverSizes(CCoverLayout *pLayout, CWnd* pParent /*=NULL*/)
	: CDialog(CDlgCoverSizes::IDD, pParent)
	, m_dwFrontWidth(0)
	, m_dwFrontHeight(0)
	, m_dwBackWidth(0)
	, m_dwBackHeight(0)
	, m_dwLabelWidth(0)
	, m_dwLabelHeight(0)
	, m_dwLabelRadius(0)
	, m_dwBorderWidth(0)
{
	m_pLayout = pLayout; 
}

CDlgCoverSizes::~CDlgCoverSizes()
{
}

void CDlgCoverSizes::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	DDX_Text(pDX, IDC_EDIT_SIZES_FRONT_WIDTH, m_dwFrontWidth);
	DDV_MinMaxUInt(pDX, m_dwFrontWidth, 0, 10000);
	DDX_Text(pDX, IDC_EDIT_SIZES_FRONT_HEIGHT, m_dwFrontHeight);
	DDV_MinMaxUInt(pDX, m_dwFrontHeight, 0, 10000);
	DDX_Text(pDX, IDC_EDIT_SIZES_BACK_WIDTH, m_dwBackWidth);
	DDV_MinMaxUInt(pDX, m_dwBackWidth, 0, 10000);
	DDX_Text(pDX, IDC_EDIT_SIZES_BACK_HEIGHT, m_dwBackHeight);
	DDV_MinMaxUInt(pDX, m_dwBackHeight, 0, 10000);
	DDX_Text(pDX, IDC_EDIT_SIZES_LABEL_WIDTH, m_dwLabelWidth);
	DDV_MinMaxUInt(pDX, m_dwLabelWidth, 0, 10000);
	DDX_Text(pDX, IDC_EDIT_SIZES_LABEL_HEIGHT, m_dwLabelHeight);
	DDV_MinMaxUInt(pDX, m_dwLabelHeight, 0, 10000);
	DDX_Text(pDX, IDC_EDIT_SIZES_LABEL_RADIUS, m_dwLabelRadius);
	DDV_MinMaxUInt(pDX, m_dwLabelRadius, 0, 10000);
	DDX_Text(pDX, IDC_EDIT_SIZES_BORDER_WIDTH, m_dwBorderWidth);
	DDV_MinMaxUInt(pDX, m_dwBorderWidth, 0, 10000);
}


BEGIN_MESSAGE_MAP(CDlgCoverSizes, CDialog)
	ON_BN_CLICKED(IDOK, OnBnClickedOk)
	ON_BN_CLICKED(IDC_BUTTON_SIZE_STANDARD, OnBnClickedButtonSizeStandard)
END_MESSAGE_MAP()



BOOL CDlgCoverSizes::OnInitDialog()
{
	CDialog::OnInitDialog();
	SetValues(m_pLayout);
	return TRUE;
}

void CDlgCoverSizes::SetValues(CCoverLayout *pLayout)
{
	m_dwFrontWidth = pLayout->gl_COVERFRONTWIDTH;
	m_dwFrontHeight = pLayout->gl_COVERFRONTHEIGHT;
	m_dwBackWidth = pLayout->gl_COVERBACKWIDTH;
	m_dwBackHeight = pLayout->gl_COVERBACKHEIGHT;
	m_dwLabelWidth = pLayout->gl_COVERLABELWIDTH;
	m_dwLabelHeight = pLayout->gl_COVERLABELHEIGHT;
	m_dwBorderWidth = pLayout->gl_COVERBORDERWIDTH;
	m_dwLabelRadius = pLayout->gl_COVERINNERLABELRADIUS;

	UpdateData(FALSE);
	return;
}

void CDlgCoverSizes::OnBnClickedOk()
{
	OnOK();
	ValuesToLayout();
	return;
}

void CDlgCoverSizes::ValuesToLayout()
{
	UpdateData(TRUE);

	m_pLayout->gl_COVERFRONTWIDTH = m_dwFrontWidth;
	m_pLayout->gl_COVERFRONTHEIGHT = m_dwFrontHeight;
	m_pLayout->gl_COVERBACKWIDTH = m_dwBackWidth;
	m_pLayout->gl_COVERBACKHEIGHT = m_dwBackHeight;
	m_pLayout->gl_COVERLABELWIDTH = m_dwLabelWidth;
	m_pLayout->gl_COVERLABELHEIGHT = m_dwLabelHeight;
	m_pLayout->gl_COVERBORDERWIDTH = m_dwBorderWidth;
	m_pLayout->gl_COVERINNERLABELRADIUS = m_dwLabelRadius;
	return;
}

void CDlgCoverSizes::OnBnClickedButtonSizeStandard()
{
	CCoverLayout def;
	SetValues(&def);
	ValuesToLayout();
	return;
}

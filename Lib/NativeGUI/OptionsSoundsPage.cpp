// OptionsSoundsPage.cpp: Implementierungsdatei
//

#include "stdafx.h"
#include "OptionsSoundsPage.h"
#include "..\..\Lib\hitmisc\hitmisc.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// Eigenschaftenseite COptionsSoundsPage 

IMPLEMENT_DYNCREATE(COptionsSoundsPage, CPropertyPage)

COptionsSoundsPage::COptionsSoundsPage() : CPropertyPage(COptionsSoundsPage::IDD)
{
	//{{AFX_DATA_INIT(COptionsSoundsPage)
	m_sDescription = _T("");
	m_bWarnWriteID3Tags = FALSE;
	//}}AFX_DATA_INIT

	//m_bWarnWriteID3Tags = (CConfig::ReadGlobalRegistryKeyInt(L"EditMP3Tags", 0) == 0);
	m_bDetectTrackInfoFromFilename = Big3::Hitbase::Configuration::Settings::Current->DetectTrackInfoFromFilename;
	m_sDetectTrackInfoDelimiter = (CString)Big3::Hitbase::Configuration::Settings::Current->DetectTrackInfoDelimiter;
	m_bDeleteLeadingNumbers = Big3::Hitbase::Configuration::Settings::Current->DeleteLeadingNumbers;
}

COptionsSoundsPage::~COptionsSoundsPage()
{
}

void COptionsSoundsPage::DoDataExchange(CDataExchange* pDX)
{
	CPropertyPage::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(COptionsSoundsPage)
	DDX_Control(pDX, IDC_OUTPUT_DEVICE, m_cboOutputDevice);
	DDX_Control(pDX, IDC_ID3_VERSION, m_cboID3Version);
	DDX_Text(pDX, IDC_DESCRIPTION, m_sDescription);
	DDX_Check(pDX, IDC_WARNMSG_ID3TAGS, m_bWarnWriteID3Tags);
	//}}AFX_DATA_MAP
	DDX_Control(pDX, IDC_BUFFERSIZE, m_cboBufferSize);
	DDX_Control(pDX, IDC_OUTPUT_CARD, m_cboOutputCard);
	DDX_Control(pDX, IDC_OUTPUT_CARD2, m_cboOutputCard2);
	DDX_Check(pDX, IDC_DETECT_TRACKINFO_FROM_FILENAME, m_bDetectTrackInfoFromFilename);
	DDX_Control(pDX, IDC_DETECT_TRACKINFO_FROM_FILENAME_DELIMITER, m_edtDetectTrackInfoDelimiter);
	DDX_Text(pDX, IDC_DETECT_TRACKINFO_FROM_FILENAME_DELIMITER, m_sDetectTrackInfoDelimiter);
	DDX_Check(pDX, IDC_DELETE_LEADING_NUMBERS, m_bDeleteLeadingNumbers);
	DDX_Control(pDX, IDC_DELETE_LEADING_NUMBERS, m_chkDeleteLeadingNumbers);
}


BEGIN_MESSAGE_MAP(COptionsSoundsPage, CPropertyPage)
	//{{AFX_MSG_MAP(COptionsSoundsPage)
	ON_CBN_SELCHANGE(IDC_ID3_VERSION, OnSelchangeId3Version)
	//}}AFX_MSG_MAP
	ON_BN_CLICKED(IDC_DETECT_TRACKINFO_FROM_FILENAME, OnBnClickedDetectTrackinfoFromFilename)
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// Behandlungsroutinen für Nachrichten COptionsSoundsPage 

BOOL COptionsSoundsPage::OnInitDialog() 
{
	CPropertyPage::OnInitDialog();
	
	m_cboOutputDevice.AddString(L"DirectSound");
	m_cboOutputDevice.AddString(L"Wave output");
	m_cboOutputDevice.AddString(L"ASIO");

	m_cboOutputDevice.SetCurSel(Big3::Hitbase::Configuration::Settings::Current->VirtualCDOutputDevice);

	m_cboID3Version.AddString(L"ID3 Version 1");
	m_cboID3Version.AddString(L"ID3 Version 2");
	m_cboID3Version.AddString(L"ID3 Version 1+2");
	
	m_cboID3Version.SetCurSel(Big3::Hitbase::Configuration::Settings::Current->UseID3Version);

	m_cboBufferSize.AddString(L"100 ms");
	m_cboBufferSize.AddString(L"150 ms");
	m_cboBufferSize.AddString(L"200 ms");
	m_cboBufferSize.AddString(L"500 ms (" + get_string(TEXT_STANDARD) + L")");

	m_cboBufferSize.SetCurSel(Big3::Hitbase::Configuration::Settings::Current->VirtualCDBufferSize);


	FillSoundDevicesInComboBox(m_cboOutputCard);
	FillSoundDevicesInComboBox(m_cboOutputCard2);
	m_cboOutputCard.SetCurSel(Big3::Hitbase::Configuration::Settings::Current->OutputDevice);
	m_cboOutputCard2.SetCurSel(Big3::Hitbase::Configuration::Settings::Current->OutputDevicePreListen);

	ShowText();
	
/*TODO!!!!!!!!!!!!!!!	if (CConfig::ReadGlobalRegistryKeyInt(L"EditMP3Tags", 0) == 0)
		GetDlgItem(IDC_WARNMSG_ID3TAGS)->EnableWindow(FALSE);*/

	UpdateWindowState();

	return TRUE;  // return TRUE unless you set the focus to a control
	              // EXCEPTION: OCX-Eigenschaftenseiten sollten FALSE zurückgeben
}

void COptionsSoundsPage::FillSoundDevicesInComboBox(CComboBox& comboBox)
{
	for each (String^ soundDevice in Big3::Hitbase::SoundEngine::SoundEngine::Instance->GetSoundDevices())
	{
		comboBox.AddString((CString)soundDevice);
	}
}

void COptionsSoundsPage::ShowText()
{
	int iCurSel = m_cboID3Version.GetCurSel();

	switch (iCurSel)
	{
	case 0:
		m_sDescription = get_string(TEXT_ID3_V1);
		break;
	case 1:
		m_sDescription = get_string(TEXT_ID3_V2);
		break;
	case 2:
		m_sDescription = get_string(TEXT_ID3_V1_V2);
		break;
	default:
		ASSERT(FALSE);
	}

	UpdateData(FALSE);
}

void COptionsSoundsPage::OnOK() 
{
	UpdateData(TRUE);

	Settings::Current->VirtualCDOutputDevice = m_cboOutputDevice.GetCurSel();
	Big3::Hitbase::Configuration::Settings::Current->UseID3Version = m_cboID3Version.GetCurSel();
	Settings::Current->VirtualCDBufferSize = m_cboBufferSize.GetCurSel();

	Big3::Hitbase::Configuration::Settings::Current->OutputDevice = m_cboOutputCard.GetCurSel();
	Big3::Hitbase::Configuration::Settings::Current->OutputDevicePreListen = m_cboOutputCard2.GetCurSel();

/*TODO!!!!!!!!!!!!!	if (m_bWarnWriteID3Tags)
		CConfig::WriteGlobalRegistryKeyInt(L"EditMP3Tags", 0);*/
	
	Big3::Hitbase::Configuration::Settings::Current->DetectTrackInfoFromFilename = m_bDetectTrackInfoFromFilename;
	Big3::Hitbase::Configuration::Settings::Current->DetectTrackInfoDelimiter = gcnew String(m_sDetectTrackInfoDelimiter);

	Big3::Hitbase::Configuration::Settings::Current->DeleteLeadingNumbers = m_bDeleteLeadingNumbers;

	CPropertyPage::OnOK();
}

void COptionsSoundsPage::OnSelchangeId3Version() 
{
	ShowText();
}

void COptionsSoundsPage::OnBnClickedDetectTrackinfoFromFilename()
{
	UpdateWindowState();
}

void COptionsSoundsPage::UpdateWindowState(void)
{
	UpdateData(TRUE);

	m_edtDetectTrackInfoDelimiter.EnableWindow(m_bDetectTrackInfoFromFilename);
	m_chkDeleteLeadingNumbers.EnableWindow(m_bDetectTrackInfoFromFilename);
}

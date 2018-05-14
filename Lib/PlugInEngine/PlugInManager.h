// PlugInManager.h: interface for the CPlugInManager class.
//
//////////////////////////////////////////////////////////////////////

#pragma once

#define MAX_PLUGINS 99

#include "../../app/hitbase/plug-ins/hpi.h"

typedef BOOL fphpiInit(HPI_INFO*);
typedef BOOL fphpiStart();
typedef BOOL fphpiStart2(HWND hWnd);
typedef BOOL fphpiEnd();
typedef BOOL fphpiGetParamInfo(int nParam, HPI_PARAM_INFO* pParamInfo);
typedef BOOL fphpiSetParamInfo(int nParam, HPI_PARAM_INFO* pParamInfo);
typedef HBITMAP fphpiGetBitmap();
typedef BOOL fphpiDisplaySoundData(HWND hWnd, const char* pSoundData, DWORD dwCount);

class CSoundVisualizerWnd;

#ifdef _PLUGINENGINE_DLL
#define PLUGINENGINE_INTERFACE __declspec ( dllexport )
#else
#define PLUGINENGINE_INTERFACE __declspec ( dllimport )
#endif

class PLUGINENGINE_INTERFACE CPlugIn
{
public:
	CPlugIn();
	CPlugIn(const CPlugIn& theOther);
	CPlugIn& operator =(const CPlugIn& theOther);

public:
	void LoadParameters();
	void SaveParameters(BOOL bSaveActiveState = FALSE);
	HBITMAP GetBitmap();
	HINSTANCE m_hpiInstance;
	fphpiInit* m_fphpiInit;
	fphpiStart* m_fphpiStart;
	fphpiStart2* m_fphpiStart2;
	fphpiEnd* m_fphpiEnd;
	fphpiGetParamInfo* m_fphpiGetParamInfo;
	fphpiSetParamInfo* m_fphpiSetParamInfo;
	fphpiDisplaySoundData* m_fphpiDisplaySoundData;
	fphpiGetBitmap* m_fphpiGetBitmap;

	HPI_INFO m_hpiInfo;

	BOOL m_bActive;
	CSoundVisualizerWnd* m_pSoundVisualizerWnd;
	CRect m_WindowRect;

	// Benutzerfelder
	int m_nNumberOfParameters;
	CArray <HPI_PARAM_INFO, HPI_PARAM_INFO> m_Param;
};

class PLUGINENGINE_INTERFACE CPlugInManager  
{
public:
	CPlugInManager();
	virtual ~CPlugInManager();

public:
	BOOL FullScreenPlugInActive();
	void CloseAllPlugIns();
	void ReadAllParameters();
	void SaveAllParameters();
	BOOL ChangeOptions(int nPlugInIndex);
	BOOL DisplaySoundData(const char*  pWaveData, DWORD dwCount);
	void SetActive(int nIndex, CSoundVisualizerWnd* pSoundVisualizerWnd, BOOL bActive);
	BOOL IsActive(int nIndex);
	void AddPlugInsToMenu(HMENU hMenu);
	HPI_INFO* GetPlugInInformation(int nIndex);
	void FillModulesInComboBox(CComboBox* pComboBox, int nBibliothek = -1);
	void FillLibrariesInComboBox(CComboBox* pComboBox);
	BOOL ReadPlugIns(const CString& sDirectory);
	CArray <CPlugIn, CPlugIn> m_hpiList;

protected:
	BOOL hpiLoad(const CString& shpiFilename, CPlugIn* pPlugIn);
};

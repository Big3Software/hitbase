// SoundVisualizer.h: interface for the CSoundVisualizer class.
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_SOUNDVISUALIZER_H__40E22AD3_F499_11D1_A42F_000000000000__INCLUDED_)
#define AFX_SOUNDVISUALIZER_H__40E22AD3_F499_11D1_A42F_000000000000__INCLUDED_

#if _MSC_VER >= 1000
#pragma once
#endif // _MSC_VER >= 1000

class CSoundVisualizerWnd;

#ifdef _PLUGINENGINE_DLL
#define PLUGINENGINE_INTERFACE __declspec ( dllexport )
#else
#define PLUGINENGINE_INTERFACE __declspec ( dllimport )
#endif

class PLUGINENGINE_INTERFACE CSoundVisualizer : public CObject
{
public:
	BOOL IsActive(int nIndex);
	void SetActive(int nIndex, BOOL bActive);

	BOOL AccessWaveData(const char* pWaveData, DWORD dwCount);

public:
	CSoundVisualizer();
	virtual ~CSoundVisualizer();

private:
};

#endif // !defined(AFX_SOUNDVISUALIZER_H__40E22AD3_F499_11D1_A42F_000000000000__INCLUDED_)

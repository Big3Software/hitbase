// WavePlay.h

#ifndef _INC_WAVEPLAY
#define _INC_WAVEPLAY

#include <dsound.h>
#include "timer.h"
#include "wavefile.h"

// Constants
#ifndef SUCCESS
#define SUCCESS TRUE        // Error returns for all member functions
#define FAILURE FALSE
#endif // SUCCESS


// CWavePlay
//
class CWavePlay
{
public:
    CWavePlay (void);
    ~CWavePlay (void);
    BOOL Open (const CString& sFilename);
    BOOL Close();
    BOOL Destroy (void);
    BOOL Play (DWORD dwMSStart, DWORD dwMSEnd);
    BOOL Stop (void);
	void Pause (BOOL bPause);
	DWORD GetDuration (void);
	BOOL IsPlaying(void);
	DWORD GetPlayPosition(void);
	
protected:
    BOOL m_fPlaying;               // semaphore (stream playing)
    UINT m_nDuration;              // duration of wave file
    UINT m_nTimeElapsed;           // elapsed time in msec since playback started

	BOOL m_bPause;

private:
	MCIDEVICEID m_mciHandle;
};

#endif // _INC_WAVEPLAY 
// CWavePlayTimer.cpp

#include "stdafx.h"
#include "Timer.h"

// constructor
CWavePlayTimer::CWavePlayTimer (void)
{
    TRACE0("CWavePlayTimer::CWavePlayTimer\n\r");

    m_nIDTimer = NULL;
}


// Destructor
CWavePlayTimer::~CWavePlayTimer (void)
{
    TRACE0("CWavePlayTimer::~CWavePlayTimer\n\r");

    if (m_nIDTimer)
    {
        timeKillEvent (m_nIDTimer);
    }
}


// Create
BOOL CWavePlayTimer::Create (UINT nPeriod, UINT nRes, DWORD dwUser, TIMERCALLBACK pfnCallback)
{
    BOOL bRtn = SUCCESS;    // assume success
    
    TRACE0("CWavePlayTimer::Create\n\r");

    ASSERT (pfnCallback);
    ASSERT (nPeriod > 10);
    ASSERT (nPeriod >= nRes);

    m_nPeriod = nPeriod;
    m_nRes = nRes;
    m_dwUser = dwUser;
    m_pfnCallback = pfnCallback;

    if ((m_nIDTimer = timeSetEvent (m_nPeriod, m_nRes, TimeProc, (DWORD) this, TIME_PERIODIC)) == NULL)
    {
        bRtn = FAILURE;
    }

    return (bRtn);
}


// Timer proc for multimedia timer callback set with timeSetTime().
//
// Calls procedure specified when Timer object was created. The 
// dwUser parameter contains "this" pointer for associated Timer object.
// 
void CALLBACK CWavePlayTimer::TimeProc(UINT uID, UINT uMsg, DWORD dwUser, DWORD dw1, DWORD dw2)
{
    // dwUser contains ptr to Timer object
    CWavePlayTimer * ptimer = (CWavePlayTimer *) dwUser;

    // Call user-specified callback and pass back user specified data
    (ptimer->m_pfnCallback) (ptimer->m_dwUser);
}

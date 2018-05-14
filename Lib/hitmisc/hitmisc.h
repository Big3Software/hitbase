// hitmisc.h : header file
//

#pragma once

#ifdef _HITMISC_DLL
#define HITMISC_INTERFACE __declspec ( dllexport )
#else
#define HITMISC_INTERFACE __declspec ( dllimport )
#endif

#include "misc.h"

// SoundEngine.h : main header file for the SOUNDENGINE DLL
//

#pragma once

#ifdef _SOUNDENGINE_DLL
#define SOUNDENGINE_INTERFACE __declspec ( dllexport )
#else
#define SOUNDENGINE_INTERFACE __declspec ( dllimport )
#endif


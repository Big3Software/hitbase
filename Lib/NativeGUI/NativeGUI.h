// NativeGUI.h : main header file for the NativeGUI DLL
//

#pragma once

#ifndef __AFXWIN_H__
	#error "include 'stdafx.h' before including this file for PCH"
#endif

#include "resource.h"		// main symbols


// CNativeGUIApp
// See NativeGUI.cpp for the implementation of this class
//

class CNativeGUIApp : public CWinApp
{
public:
	CNativeGUIApp();

// Overrides
public:
	virtual BOOL InitInstance();

	DECLARE_MESSAGE_MAP()
};

// SkinEngine.cpp : Defines the initialization routines for the DLL.
//

//#include "stdafx.h"
#include <afxwin.h>         // MFC core and standard components
#include <afxdllx.h>

// ACHTUNG: Die Seriennummer ist ein Platzhalter für die tatsächliche
// Seriennummer, wenn das Programm registriert ist. So muß für jede
// neue Version, die nicht mehr kostenlos registrierbar ist, geändert werden!
__declspec(dllexport) char* RegisteredTo = "o30v3ffwfjsafäp#pof32#üt32#t9p32g90u89ukahfiusajfl";
__declspec(dllexport) char* ActivationKey = "gllkp3";
__declspec(dllexport) char* SerialNumber = "ä.,.,:=???..a..";

#ifdef _DEBUG
#define new DEBUG_NEW
#endif

static AFX_EXTENSION_MODULE SkinEngineDLL = { NULL, NULL };

extern "C" int APIENTRY
DllMain(HINSTANCE hInstance, DWORD dwReason, LPVOID lpReserved)
{
	// Remove this if you use lpReserved
	UNREFERENCED_PARAMETER(lpReserved);

	if (dwReason == DLL_PROCESS_ATTACH)
	{
		TRACE0("SkinEngine.DLL Initializing!\n");
		
		// Extension DLL one-time initialization
		if (!AfxInitExtensionModule(SkinEngineDLL, hInstance))
			return 0;

		// Insert this DLL into the resource chain
		// NOTE: If this Extension DLL is being implicitly linked to by
		//  an MFC Regular DLL (such as an ActiveX Control)
		//  instead of an MFC application, then you will want to
		//  remove this line from DllMain and put it in a separate
		//  function exported from this Extension DLL.  The Regular DLL
		//  that uses this Extension DLL should then explicitly call that
		//  function to initialize this Extension DLL.  Otherwise,
		//  the CDynLinkLibrary object will not be attached to the
		//  Regular DLL's resource chain, and serious problems will
		//  result.

		new CDynLinkLibrary(SkinEngineDLL);

	}
	else if (dwReason == DLL_PROCESS_DETACH)
	{
		TRACE0("SkinEngine.DLL Terminating!\n");

		// Terminate the library before destructors are called
		AfxTermExtensionModule(SkinEngineDLL);
	}
	return 1;   // ok
}


// stdafx.h : include file for standard system include files,
//  or project specific include files that are used frequently, but
//      are changed infrequently
//

#pragma once

#define _CRT_SECURE_NO_DEPRECATE

#define VC_EXTRALEAN		// Exclude rarely-used stuff from Windows headers

#include <afxwin.h>         // MFC core and standard components
#include <afxext.h>         // MFC extensions
#include <afxtempl.h>

#ifndef _AFX_NO_DAO_SUPPORT
#include <afxdao.h>			// MFC DAO database classes
#endif // _AFX_NO_DAO_SUPPORT

#ifndef _AFX_NO_AFXCMN_SUPPORT
#include <afxcmn.h>			// MFC support for Windows Common Controls
#endif // _AFX_NO_AFXCMN_SUPPORT

// Dao ist deprecated.... datt is uns doch ejal!
#pragma warning(disable : 4995)

#include "../hitmisc/HitbaseWinAppBase.h"

/*extern "C" {
#include "../BTree/btgconst.h"
#include "../BTree/wtf.he"
#include "../BTree/vlen3.he"
#include "../BTree/isamv3.he"
}*/


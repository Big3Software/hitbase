// stdafx.h : include file for standard system include files,
//  or project specific include files that are used frequently, but
//      are changed infrequently
//

#if !defined(AFX_STDAFX_H__910F52E5_FE6C_11D3_A4D5_0080AD834EB5__INCLUDED_)
#define AFX_STDAFX_H__910F52E5_FE6C_11D3_A4D5_0080AD834EB5__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#define VC_EXTRALEAN        // Exclude rarely-used stuff from Windows headers

#include <afxwin.h>         // MFC core and standard components
#include <afxext.h>         // MFC extensions
#include <afxdtctl.h>       // MFC support for Internet Explorer 4 Common Controls
#ifndef _AFX_NO_AFXCMN_SUPPORT
#include <afxcmn.h>         // MFC support for Windows Common Controls
#endif // _AFX_NO_AFXCMN_SUPPORT

#ifndef _AFX_NO_DAO_SUPPORT
#include <afxdao.h>         // MFC DAO database classes
#endif // _AFX_NO_DAO_SUPPORT

#include <afxtempl.h>

#include <afxpriv.h>

//#include <secall.h>

#include <BTCImageData.h>
#include <BTCImageObject.h>
#include <BTCIOStream.h>

// Dao ist deprecated.... datt is uns doch ejal!
#pragma warning(disable : 4995)

#include "../hitmisc/HitbaseWinAppBase.h"

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_STDAFX_H__910F52E5_FE6C_11D3_A4D5_0080AD834EB5__INCLUDED_)

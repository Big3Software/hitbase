/////////////////////////////////////////////////////////////////////////////
//
// NexgenIPL - Next Generation Image Processing Library
// Copyright (c) 1999-2003 Binary Technologies
// All Rights Reserved.
//
// This code may be used in compiled form in any way you desire. This
// file may be redistributed unmodified by any means PROVIDING it is 
// not sold for profit without the authors written consent, and 
// providing that this notice and the authors name is included. If 
// the source code in this file is used in any commercial application 
// then a simple email would be nice.
//
// THIS SOFTWARE IS PROVIDED BY BINARY TECHNOLOGIES ``AS IS'' AND ANY
// EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL BINARY TECHNOLOGIES OR
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
// EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
// PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
// PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY
// OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE
// USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH
// DAMAGE.
//
// Binary Technologies
// http://www.binary-technologies.com
// info@binary-technologies.com
//
/////////////////////////////////////////////////////////////////////////////

#ifndef BTNEXGENIPL_INCLUDED
#define BTNEXGENIPL_INCLUDED

#pragma once

/////////////////////////////////////////////////////////////////////////////
// The following will ensure that we are exporting C++ classes when 
// building the DLL and importing the classes when build an application 
// using this DLL.
/////////////////////////////////////////////////////////////////////////////

#ifdef _BTNEXGENIPL
	#define BTNEXGENIPL_CLASS_EXPORT	__declspec(dllexport)
#else
	#define BTNEXGENIPL_CLASS_EXPORT	__declspec(dllimport)
#endif

/////////////////////////////////////////////////////////////////////////////
// The following will ensure that when building an application (or another
// DLL) using this DLL, the appropriate .LIB file will automatically be used
// when linking.
/////////////////////////////////////////////////////////////////////////////

#ifndef _BTNEXGENIPL_NOAUTOLIB
//	#if defined(UNICODE) || defined(_UNICODE)
//		#define _BTNEXGENIPL_LIBPATH "BTNexgenIPL32u.lib"
//	#else
		#define _BTNEXGENIPL_LIBPATH "BTNexgenIPL32.lib"
//	#endif

	#define _BTNEXGENIPL_MESSAGE "Automatically linking with " _BTNEXGENIPL_LIBPATH

	#pragma comment(lib, _BTNEXGENIPL_LIBPATH)
	#pragma message(_BTNEXGENIPL_MESSAGE)
#endif

#endif // BTNEXGENIPL_INCLUDED

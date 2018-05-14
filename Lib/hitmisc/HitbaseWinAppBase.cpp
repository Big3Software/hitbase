// HitbaseWinAppBase.cpp : implementation file
//

#include "stdafx.h"
#include "hitmisc.h"
#include "HitbaseWinAppBase.h"
#include "afxwinappex.h"


// CHitbaseWinAppBase

IMPLEMENT_DYNCREATE(CHitbaseWinAppBase, CWinAppEx)

CHitbaseWinAppBase::CHitbaseWinAppBase()
{
}

CHitbaseWinAppBase::~CHitbaseWinAppBase()
{
}

BOOL CHitbaseWinAppBase::InitInstance()
{
	// TODO:  perform and per-thread initialization here
	return TRUE;
}

int CHitbaseWinAppBase::ExitInstance()
{
	// TODO:  perform any per-thread cleanup here
	return CWinAppEx::ExitInstance();
}

BEGIN_MESSAGE_MAP(CHitbaseWinAppBase, CWinApp)
END_MESSAGE_MAP()


// CHitbaseWinAppBase message handlers

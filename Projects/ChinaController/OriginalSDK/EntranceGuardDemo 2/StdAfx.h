// stdafx.h : include file for standard system include files,
//  or project specific include files that are used frequently, but
//      are changed infrequently
//

#if !defined(AFX_STDAFX_H__241F999A_AAEA_40B7_A273_893FB25B34CB__INCLUDED_)
#define AFX_STDAFX_H__241F999A_AAEA_40B7_A273_893FB25B34CB__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#define WIN32_LEAN_AND_MEAN		// Exclude rarely-used stuff from Windows headers

#include <stdio.h>
#include "Commons.h"
#include <iostream>
// TODO: reference additional headers your program requires here

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.
#include <string>
#include <list>
#include <assert.h>
//#include "Include/mdump.h"

#include "Include/dhnetsdk.h"
#pragma comment(lib, "./Bin/dhnetsdk.lib")
#include "Include/dhconfigsdk.h"
#pragma comment(lib, "./Bin/dhconfigsdk.lib")
//#include "Include/dhplay.h"
//#pragma comment(lib, "../../../Bin/Debug/dhplay.lib")

#if defined(WIN32) || defined(WIN64)
#elif defined(__linux__)
#define __min(a, b) (a <= b ? a : b)
#endif

#endif // !defined(AFX_STDAFX_H__241F999A_AAEA_40B7_A273_893FB25B34CB__INCLUDED_)

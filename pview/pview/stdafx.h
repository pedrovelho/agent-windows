// stdafx.h

#define VC_EXTRALEAN
#define _WIN32_DCOM

#include <afxwin.h>
#include <afxext.h>
#include <afxdtctl.h>
#include <afxcmn.h>
#include <afxtempl.h>
#include <afxcoll.h>

#include <afxpriv.h>
#include <wbemidl.h>
#include <aclapi.h>
#include <aclui.h>
#include <vdmdbg.h>

#ifndef countof
#   define countof(a)	    (sizeof(a)/sizeof(a[0]))
#endif

#ifndef offsetof
#   define offsetof(a, b)   ((size_t)&(((a *)0)->b))
#endif

#ifndef _VERIFY
#   ifdef _DEBUG
#		define _VERIFY(x)   _ASSERTE(x)
#   else
#		define _VERIFY(x)   ((void)(x))
#   endif
#endif

#ifndef _DEBUG_ONLY
#   ifdef _DEBUG
#		define _DEBUG_ONLY(x)	((void)(x))
#   else
#		define _DEBUG_ONLY(x)	((void)0)
#   endif
#endif

#ifndef _UNUSED
#   if _MSC_VER >= 1000
#		define _UNUSED(x)	x
#   else
#		define _UNUSED(x)	x = x
#   endif
#endif

#ifdef _DEBUG
#	define _RPT_API_FAILED(name)	\
	    do { unsigned __err = GetLastError(); \
		 _RPTF1(_CRT_WARN, #name "() failed; ERR=%d\n", __err); \
		 SetLastError(__err); } while(0)
#else
#	define _RPT_API_FAILED(name)	((void)0)
#endif

// secedit.cpp
//
// Process security editor.
//
// $Id: $
//

#include "stdafx.h"
#include "resource.h"
#include "secedit.h"

typedef struct _ACLHELPCONTROL {
    PCWSTR				HelpFile;
    DWORD				MainDialogTopic;
    DWORD				ACLEditorDialogTopic;
    DWORD				Reserved1;
    DWORD				AddEntryDialogTopic;
    DWORD				Reserved2;
    DWORD				Reserved3;
    DWORD				AccountDialogTopic;
} ACLHELPCONTROL, * PACLHELPCONTROL; 

typedef struct _ACLDLGCONTROL {
    DWORD				Version;
    PGENERIC_MAPPING	GenericAccessMap; 
    PGENERIC_MAPPING	SpecificAccessMap; 
    PCWSTR				DialogTitle;
    PACLHELPCONTROL		HelpInfo;
    PCWSTR				SubReplaceTitle;
    DWORD				Reserved;
    PCWSTR				SubReplaceConfirmation;
    PCWSTR				SpecialAccess;
} ACLDLGCONTROL, * PACLDLGCONTROL; 

typedef struct _ACLEDITENTRY {
    DWORD				Type;
    DWORD				AccessMask;
    DWORD				Reserved;
    PCWSTR				Name;
} ACLEDITENTRY, * PACLEDITENTRY;

typedef struct _ACLEDITCONTROL {
    DWORD				NumberOfEntries;
    PACLEDITENTRY       Entries;
} ACLEDITCONTROL, * PACLEDITCONTROL; 

typedef DWORD (CALLBACK * PACLCHANGE)(
            IN PVOID DontKnow1, 
            IN PVOID DontKnow2, 
            IN PVOID CallbackContext, 
            IN PSECURITY_DESCRIPTOR NewSD,
            IN PVOID DontKnow3, 
            IN PVOID DontKnow4,
            IN PVOID DontKnow5, 
            IN PDWORD ChangeContextStatus
			);

typedef DWORD (APIENTRY * PFNSEDDACLEDITOR)(
			IN PVOID Unknown,
			IN HINSTANCE Instance,               
            IN PCWSTR MachineName,
            IN PACLDLGCONTROL AclDlgControl,
            IN PACLEDITCONTROL AclEditControl,
            IN PCWSTR ObjectName,
            IN PACLCHANGE ChangeCallback,
            IN PVOID ChangeCallbackContext,
            IN PSECURITY_DESCRIPTOR ObjectSecurity,
            IN BOOLEAN NoReadPermission,
            IN BOOLEAN ReadOnly,
            OUT PDWORD ChangeContextStatus,
            IN PVOID Reserved
			);

typedef BOOL (APIENTRY * PFNEDITSECURITY)(
			IN HWND, 
			IN LPSECURITYINFO
			);

#define PROCESS_GENERIC_READ	(STANDARD_RIGHTS_READ		| \
								 PROCESS_VM_READ			| \
								 PROCESS_QUERY_INFORMATION	| \
								 SYNCHRONIZE)

#define PROCESS_GENERIC_WRITE	(STANDARD_RIGHTS_WRITE		| \
								 PROCESS_VM_OPERATION		| \
								 PROCESS_VM_WRITE			| \
								 PROCESS_SET_INFORMATION	| \
								 PROCESS_SET_QUOTA			| \
								 PROCESS_SUSPEND_RESUME		| \
								 PROCESS_SET_SESSIONID		| \
								 PROCESS_CREATE_THREAD		| \
								 PROCESS_CREATE_PROCESS		| \
								 PROCESS_DUP_HANDLE			| \
								 SYNCHRONIZE)

#define PROCESS_GENERIC_EXECUTE (STANDARD_RIGHTS_EXECUTE	| \
								 SYNCHRONIZE)


const SI_ACCESS CProcessSecInfo::s_rgAccess[] = {
	// general access rights
	{ &GUID_NULL, PROCESS_GENERIC_READ,		MAKEINTRESOURCEW(IDS_GENERIC_READ),				SI_ACCESS_GENERAL  },
	{ &GUID_NULL, PROCESS_ALL_ACCESS,		MAKEINTRESOURCEW(IDS_FULL_CONTROL),				SI_ACCESS_GENERAL  },

	// specific access rights
	{ &GUID_NULL, PROCESS_TERMINATE,		MAKEINTRESOURCEW(IDS_PROCESS_TERMINATE),		SI_ACCESS_SPECIFIC },
	{ &GUID_NULL, PROCESS_CREATE_THREAD,	MAKEINTRESOURCEW(IDS_PROCESS_CREATE_THREAD),	SI_ACCESS_SPECIFIC },
	{ &GUID_NULL, PROCESS_SET_SESSIONID,	MAKEINTRESOURCEW(IDS_PROCESS_SET_SESSIONID),	SI_ACCESS_SPECIFIC },
	{ &GUID_NULL, PROCESS_VM_OPERATION,		MAKEINTRESOURCEW(IDS_PROCESS_VM_OPERATION),		SI_ACCESS_SPECIFIC },
	{ &GUID_NULL, PROCESS_VM_READ,			MAKEINTRESOURCEW(IDS_PROCESS_VM_READ),			SI_ACCESS_SPECIFIC },
	{ &GUID_NULL, PROCESS_VM_WRITE,			MAKEINTRESOURCEW(IDS_PROCESS_VM_WRITE),			SI_ACCESS_SPECIFIC },
	{ &GUID_NULL, PROCESS_DUP_HANDLE,		MAKEINTRESOURCEW(IDS_PROCESS_DUP_HANDLE),		SI_ACCESS_SPECIFIC },
	{ &GUID_NULL, PROCESS_CREATE_PROCESS,	MAKEINTRESOURCEW(IDS_PROCESS_CREATE_PROCESS),	SI_ACCESS_SPECIFIC },
	{ &GUID_NULL, PROCESS_SET_QUOTA,		MAKEINTRESOURCEW(IDS_PROCESS_SET_QUOTA),		SI_ACCESS_SPECIFIC },
	{ &GUID_NULL, PROCESS_SET_INFORMATION,	MAKEINTRESOURCEW(IDS_PROCESS_SET_INFORMATION),	SI_ACCESS_SPECIFIC },
	{ &GUID_NULL, PROCESS_QUERY_INFORMATION,MAKEINTRESOURCEW(IDS_PROCESS_QUERY_INFORMATION),SI_ACCESS_SPECIFIC },
	{ &GUID_NULL, PROCESS_SUSPEND_RESUME,	MAKEINTRESOURCEW(IDS_PROCESS_SUSPEND_RESUME),	SI_ACCESS_SPECIFIC },
	{ &GUID_NULL, SYNCHRONIZE,				MAKEINTRESOURCEW(IDS_SYNCHRONIZE),				SI_ACCESS_SPECIFIC },
	{ &GUID_NULL, DELETE,					MAKEINTRESOURCEW(IDS_DELETE),					SI_ACCESS_SPECIFIC },
	{ &GUID_NULL, READ_CONTROL,				MAKEINTRESOURCEW(IDS_READ_CONTROL),				SI_ACCESS_SPECIFIC },
	{ &GUID_NULL, WRITE_DAC,				MAKEINTRESOURCEW(IDS_WRITE_DAC),				SI_ACCESS_SPECIFIC },
	{ &GUID_NULL, WRITE_OWNER,				MAKEINTRESOURCEW(IDS_WRITE_OWNER),				SI_ACCESS_SPECIFIC }
};

const GENERIC_MAPPING CProcessSecInfo::s_GenericMapping = {
	PROCESS_GENERIC_READ,
	PROCESS_GENERIC_WRITE,
	PROCESS_GENERIC_EXECUTE,
	PROCESS_ALL_ACCESS
};

//---------------------------------------------------------------------------
// CProcessSecInfo
//
//  Constructor, initializes the object.
//
//  Parameters:
//    none.
//
//  Returns:
//	  no return value.
//
CProcessSecInfo::CProcessSecInfo()
{
	m_hProcess = NULL;
	m_pszProcessName = NULL;
	m_bReadOnly = FALSE;
	m_pSecDesc = NULL;
	m_pAclDlgControl = NULL;
	m_pAclEditControl = NULL;
}

//---------------------------------------------------------------------------
// ~CProcessSecInfo
//
//  Destructor, performs necessary cleanup.
//
//  Parameters:
//    none.
//
//  Returns:
//	  no return value.
//
CProcessSecInfo::~CProcessSecInfo()
{
	if (m_hProcess != NULL)
		_VERIFY(CloseHandle(m_hProcess));
	if (m_pszProcessName != NULL)
		free(m_pszProcessName);
	if (m_pSecDesc != NULL)
		LocalFree((HLOCAL)m_pSecDesc);
	if (m_pAclDlgControl != NULL)
		free(m_pAclDlgControl);
	if (m_pAclEditControl != NULL)
		free(m_pAclEditControl);
}

//---------------------------------------------------------------------------
// EditDacl
//
//  Edits the process' discretionary access control list.
//
//  Parameters:
//    hWnd      - owner window handle
//	  hProcess  - process handle
//	  pszName   - process name
//    bReadOnly - indicates read-only operation
//
//  Returns:
//	  TRUE, if successful, FALSE - otherwise.'
//
BOOL
CProcessSecInfo::EditDacl(
	IN HWND hWnd,
	IN DWORD dwProcessId,
	IN PCTSTR pszName
	)
{
	// open process handle
	if (!OpenProcessHandle(dwProcessId))
		return FALSE;

	// convert process name to Unicode
	int cch = lstrlen(pszName) + 1;

	m_pszProcessName = (PWSTR)malloc(cch * sizeof(WCHAR));
	if (m_pszProcessName == NULL)
		return SetLastError(ERROR_NOT_ENOUGH_MEMORY), FALSE;

#ifndef UNICODE
	MultiByteToWideChar(CP_ACP, 0, pszName, -1, m_pszProcessName, cch);
#else
	lstrcpy(m_pszProcessName, pszName);
#endif

	HINSTANCE hLibrary;
	DWORD dwError = ERROR_SUCCESS;
	
	// try to use new aclui.dll
	hLibrary = LoadLibrary(_T("aclui.dll"));
	if (hLibrary != NULL)
	{
		PFNEDITSECURITY pfnEditSecurity =
			(PFNEDITSECURITY)GetProcAddress(hLibrary, "EditSecurity");
		_ASSERTE(pfnEditSecurity != NULL);
		
		if (!pfnEditSecurity(hWnd, this))
			dwError = GetLastError();

		_VERIFY(FreeLibrary(hLibrary));
		
		SetLastError(dwError);
		return dwError == ERROR_SUCCESS;
	}

	// try to use undocumented (and ugly) acledit.dll
	hLibrary = LoadLibrary(_T("acledit.dll"));
	if (hLibrary != NULL)
	{
		PFNSEDDACLEDITOR pfnSedDaclEditor =
			(PFNSEDDACLEDITOR)GetProcAddress(hLibrary, "SedDiscretionaryAclEditor");
		_ASSERTE(pfnSedDaclEditor != NULL);
		
		PACLDLGCONTROL pAclDlgControl = GetAclDlgControl();
		PACLEDITCONTROL pAclEditControl = GetAclEditControl();
		PSECURITY_DESCRIPTOR pSecDesc = GetSecurityDescriptor();
		DWORD dwStatus = ERROR_SUCCESS;

		if (pAclDlgControl == NULL ||
			pAclEditControl == NULL ||
			pSecDesc == NULL)
			return SetLastError(ERROR_NOT_ENOUGH_MEMORY), FALSE;

		// invoke the security editor
		dwError = pfnSedDaclEditor(NULL, AfxGetInstanceHandle(), NULL,
								   pAclDlgControl, pAclEditControl,
								   m_pszProcessName, ChangeCallback, this,
								   pSecDesc, FALSE, m_bReadOnly, &dwStatus,
								   NULL);
		if (dwError == ERROR_SUCCESS)
			dwError = dwStatus;

		_VERIFY(FreeLibrary(hLibrary));
		
		SetLastError(dwError);
		return dwError == ERROR_SUCCESS;
	}

	return SetLastError(ERROR_CALL_NOT_IMPLEMENTED), FALSE;
}

//---------------------------------------------------------------------------
// IsDaclEditorAvailable
//
//  Checks whether any version of the security editor is available
//
//  Parameters:
//    none.
//
//  Returns:
//	  TRUE, if the security editor is available, FALSE - otherwise.
//
BOOL
CProcessSecInfo::IsDaclEditorAvailable()
{
	HINSTANCE hLibrary;
	BOOL bAvailable = FALSE;
	
	// try to use new aclui.dll
	hLibrary = LoadLibrary(_T("aclui.dll"));
	if (hLibrary != NULL)
	{
		bAvailable = GetProcAddress(hLibrary, "EditSecurity") != NULL;
		_VERIFY(FreeLibrary(hLibrary));
	}

	if (!bAvailable)
	{
		// check the presense of old acledit.dll
		hLibrary = LoadLibrary(_T("acledit.dll"));
		if (hLibrary != NULL)
		{
			bAvailable = GetProcAddress(hLibrary, 
										"SedDiscretionaryAclEditor") != NULL;
			_VERIFY(FreeLibrary(hLibrary));
		}
	}

	return bAvailable;
}

//---------------------------------------------------------------------------
// QueryInterface
//
//  Returns a pointer to the requested interface.
//
//  Parameters:
//	  riid  - interface identifier
//	  ppObj - pointer to a variable that receives the interface pointer
//
//  Returns:
//	  standard COM return code.
//
STDMETHODIMP
CProcessSecInfo::QueryInterface(
	IN REFIID riid,
	OUT PVOID * ppObj
	)
{
	if (IsEqualIID(riid, IID_IUnknown) ||
		IsEqualIID(riid, IID_ISecurityInformation))
	{
		*ppObj = static_cast<ISecurityInformation *>(this);
		return S_OK;
	}

	return E_NOINTERFACE;
}

//---------------------------------------------------------------------------
// AddRef
//
//  Increments the reference count.
//
//  Parameters:
//	  none.
//
//  Returns:
//	  new number of references.
//
STDMETHODIMP_(ULONG)
CProcessSecInfo::AddRef()
{
	return 1;
}

//---------------------------------------------------------------------------
// Release
//
//  Decrements the reference count.
//
//  Parameters:
//	  none.
//
//  Returns:
//	  new number of references.
//
STDMETHODIMP_(ULONG)
CProcessSecInfo::Release()
{
	return 1;
}

//---------------------------------------------------------------------------
// GetObjectInformation
//
//  Returns information about the object being edited.
//
//  Parameters:
//	  pObjectInfo - pointer to a structure that receives object information
//
//  Returns:
//	  standard COM return code.
//
STDMETHODIMP
CProcessSecInfo::GetObjectInformation(
	OUT PSI_OBJECT_INFO pObjectInfo
	)
{
	pObjectInfo->dwFlags = SI_EDIT_PERMS|SI_ADVANCED|SI_NO_ACL_PROTECT|
						   SI_NO_TREE_APPLY;

	if (m_bReadOnly)
		pObjectInfo->dwFlags |= SI_READONLY;

	pObjectInfo->hInstance = AfxGetResourceHandle();
	pObjectInfo->pszObjectName = (LPWSTR)m_pszProcessName;
	pObjectInfo->pszServerName = NULL;
	pObjectInfo->pszPageTitle = NULL;

	return S_OK;
}

//---------------------------------------------------------------------------
// GetSecurity
//
//  Retrieves the security descriptor of the object being edited.
//
//  Parameters:
//	  SecurityInformation  - specifies which information to include into the
//						     security descriptor
//	  ppSecurityDescriptor - pointer to a variable that receives a pointer
//							 to the security descriptor
//	  bDefault			   - specifies whether the default security
//							 descriptor should be retrieved
//
//  Returns:
//	  standard COM return code.
//
STDMETHODIMP
CProcessSecInfo::GetSecurity(
	IN SECURITY_INFORMATION SecurityInformation,
	OUT PSECURITY_DESCRIPTOR * ppSecurityDescriptor,
	IN BOOL bDefault
	)
{
	_UNUSED(bDefault);

	if (ppSecurityDescriptor == NULL)
		return E_POINTER;

	DWORD dwError = GetSecurityInfo(m_hProcess, SE_KERNEL_OBJECT, 
							SecurityInformation, NULL, NULL, NULL, NULL, 
							ppSecurityDescriptor);

	if (dwError != ERROR_SUCCESS)
		return HRESULT_FROM_WIN32(dwError);

	return S_OK;
}

//---------------------------------------------------------------------------
// SetSecurity
//
//  Sets the security descriptor of the object being edited.
//
//  Parameters:
//	  SecurityInformation  - specifies which information to set
//	  pSecurityDescriptor - pointer to the security descriptor
//
//  Returns:
//	  standard COM return code.
//
STDMETHODIMP
CProcessSecInfo::SetSecurity(
	IN SECURITY_INFORMATION SecurityInformation,
	IN PSECURITY_DESCRIPTOR pSecurityDescriptor
	)
{
	if (pSecurityDescriptor == NULL)
		return E_POINTER;

	PSID pOwner = NULL, pGroup = NULL;
	PACL pDacl = NULL, pSacl = NULL;
	BOOL bDefaulted, bPresent;

	if (SecurityInformation & OWNER_SECURITY_INFORMATION)
	{
		_VERIFY(GetSecurityDescriptorOwner(pSecurityDescriptor, 
										   &pOwner, &bDefaulted));
	}
	if (SecurityInformation & GROUP_SECURITY_INFORMATION)
	{
		_VERIFY(GetSecurityDescriptorGroup(pSecurityDescriptor, 
										   &pGroup, &bDefaulted));
	}
	if (SecurityInformation & DACL_SECURITY_INFORMATION)
	{
		_VERIFY(GetSecurityDescriptorDacl(pSecurityDescriptor, &bPresent,
										  &pDacl, &bDefaulted));
	}
	if (SecurityInformation & SACL_SECURITY_INFORMATION)
	{
		_VERIFY(GetSecurityDescriptorSacl(pSecurityDescriptor, &bPresent,
										  &pSacl, &bDefaulted));
	}

	DWORD dwError = SetSecurityInfo(m_hProcess, SE_KERNEL_OBJECT,
								SecurityInformation, pOwner, pGroup,
								pDacl, pSacl);
	if (dwError != ERROR_SUCCESS)
		return HRESULT_FROM_WIN32(dwError);

	return S_OK;
}

//---------------------------------------------------------------------------
// GetAccessRights
//
//  Returns information about the access rights supported by the object 
//  being edited. 
//
//  Parameters:
//	  pObjectType	  - identifies type of the object which access rights
//						are being requested
//	  dwFlags		  - identifies the page being initialized
//	  ppAccess		  - pointer to a variable that receives a pointer to
//						an array of SI_ACCESS structures
//	  pcAccess		  - pointer to a variable that receives the number of
//						items in the array
//	  piDefaultAccess - pointer to a variable that receives the index of
//						the default access rights item in the array
//
//  Returns:
//	  standard COM error code.
//
STDMETHODIMP
CProcessSecInfo::GetAccessRights(
	IN const GUID * pObjectType,
	IN DWORD dwFlags,
	OUT PSI_ACCESS * ppAccess,
	OUT PULONG pcAccess,
	OUT PULONG piDefaultAccess
	)
{
	_UNUSED(pObjectType);
	_UNUSED(dwFlags);

	if (ppAccess == NULL ||
		pcAccess == NULL ||
		piDefaultAccess == NULL)
		return E_POINTER;
	
	*ppAccess = (PSI_ACCESS)s_rgAccess;
	*pcAccess = sizeof(s_rgAccess)/sizeof(s_rgAccess[0]);
	*piDefaultAccess = 0;

	return S_OK;
}

//---------------------------------------------------------------------------
// MapGeneric
//
//  Maps generic access rights into specific access rights.
//
//  Parameters:
//    pObjectType - specifies object type
//	  pAceFlags   - pointer to the AceFlags member of the ACE_HEADER struc-
//					ture from the ACE whose access mask is being mapped
//    pMask		  - pointer to a variable containing generic access mask to
//					map
// 
//  Returns:
//	  standard COM error code.
//
STDMETHODIMP
CProcessSecInfo::MapGeneric(
	IN const GUID * pObjectType,
	IN PUCHAR pAceFlags,
	IN OUT PACCESS_MASK pMask
	)
{
	_UNUSED(pObjectType);
	_UNUSED(pAceFlags);

	if (pMask == NULL)
		return E_POINTER;

	MapGenericMask(pMask, (PGENERIC_MAPPING)&s_GenericMapping);
	return S_OK;
}

//---------------------------------------------------------------------------
// GetInheritTypes
//
//  Returns information about how the object's ACEs can be inherited by
//  child objects.
//
//  Parameters:
//    ppInheritType - pointer to a variable that receives a pointer to an
//					  array of SI_INHERIT_TYPE structures
//	  pcInheritType - pointer to a variable that receives the number of items
//					  in the array
//
//  Returns:
//    standard COM return code.
//
STDMETHODIMP
CProcessSecInfo::GetInheritTypes(
	OUT PSI_INHERIT_TYPE * ppInheritType,
	OUT PULONG pcInheritType
	)
{
	if (ppInheritType == NULL ||
		pcInheritType == NULL)
		return E_POINTER;

	*ppInheritType = NULL;
	*pcInheritType = 0;

	return S_OK;
}

//---------------------------------------------------------------------------
// PropertySheetPageCallback
//
//  Notifies the application that an access control editor property page is
//  being created or destroyed.
//
//  Parameters:
//    hWnd  - property page window handle
//	  uMsg  - notification message
//	  nType - type of the page being created or destroyed
//
//  Returns:
//	  standard COM return code.
//
STDMETHODIMP
CProcessSecInfo::PropertySheetPageCallback(
	HWND hWnd,
	UINT uMsg,
	SI_PAGE_TYPE nType
	)
{
	_UNUSED(hWnd);
	_UNUSED(uMsg);
	_UNUSED(nType);

	return S_OK;
}

//---------------------------------------------------------------------------
// OpenProcessHandle
//
//  Opens a handle to the specified process with access rights suitable
//  for editing or viewing its security descriptor.
//
//  Parameters:
//    dwProcessId - process identifier
//
//  Returns:
//    TRUE, if successful, FALSE - otherwise.
//
BOOL
CProcessSecInfo::OpenProcessHandle(
	IN DWORD dwProcessId
	)
{
	_ASSERTE(m_hProcess == NULL);

	// first try to open the process with both READ_CONTROL and WRITE_DAC
	// access flags; this will allow us both view and edit the security
	// descriptor of the process
	m_hProcess = OpenProcess(READ_CONTROL|WRITE_DAC, FALSE, dwProcessId);
	if (m_hProcess == NULL)
	{
		if (GetLastError() != ERROR_ACCESS_DENIED)
			return FALSE;

		// try to open the process with the only READ_CONTROL access flag;
		// that means we can only view the security descriptor but cannot
		// edit it
		m_hProcess = OpenProcess(READ_CONTROL, FALSE, dwProcessId);
		if (m_hProcess == NULL)
			return FALSE;

		// if opened this way, proceed with read-only operation
		m_bReadOnly = TRUE;
	}

	return TRUE;
}

//---------------------------------------------------------------------------
// GetAclDlgControl
//
//  Creates ACLDLGCONTROL structure.
//
//  Parameters:
//	  none.
//
//  Returns:
//	  pointer to ACLDLGCONTROL structure if successful, or NULL - otherwise.
//
PACLDLGCONTROL
CProcessSecInfo::GetAclDlgControl()
{
	// determine how much space we have to allocate
	DWORD cb = sizeof(ACLDLGCONTROL) + 
			   sizeof(ACLHELPCONTROL) +
			   80 + 80 + MAX_PATH;

	m_pAclDlgControl = (PACLDLGCONTROL)calloc(1, cb);
	if (m_pAclDlgControl != NULL)
	{
		PACLHELPCONTROL pHelpControl = (PACLHELPCONTROL)(m_pAclDlgControl + 1);
		PWSTR pszText = (PWSTR)(pHelpControl + 1);

		m_pAclDlgControl->Version = 0x101;
		m_pAclDlgControl->GenericAccessMap = (PGENERIC_MAPPING)&s_GenericMapping;
		m_pAclDlgControl->SpecificAccessMap = (PGENERIC_MAPPING)&s_GenericMapping;
		m_pAclDlgControl->HelpInfo = pHelpControl;

		m_pAclDlgControl->SpecialAccess = pszText;
		pszText += LoadStringW(AfxGetResourceHandle(), IDS_SPECIAL_ACCESS,
							   pszText, 80) + 1;

		m_pAclDlgControl->DialogTitle = pszText;
		pszText += LoadStringW(AfxGetResourceHandle(), IDS_PROCESS_OBJECT,
							   pszText, 80) + 1;

		m_pAclDlgControl->SubReplaceTitle = NULL;
		m_pAclDlgControl->SubReplaceConfirmation = NULL;

		pHelpControl->HelpFile = pszText;

#ifndef UNICODE
		MultiByteToWideChar(CP_ACP, 0, AfxGetApp()->m_pszHelpFilePath, -1,
							pszText, MAX_PATH);
#else
		lstrcpyn(pszText, AfxGetApp()->m_pszHelpFilePath, MAX_PATH);
#endif
	}

	return m_pAclDlgControl;
}

//---------------------------------------------------------------------------
// GetAclEditControl
//
//  Creates ACLEDITCONTROL structure.
//
//  Parameters:
//	  none.
//
//  Returns:
//	  pointer to ACLEDITCONTROL structure if successful, or NULL - otherwise.
//
PACLEDITCONTROL
CProcessSecInfo::GetAclEditControl()
{
	// determine how much space we have to allocate
	DWORD cb = sizeof(ACLEDITCONTROL) + 
			   countof(s_rgAccess) * (sizeof(ACLEDITENTRY) + 80);

	m_pAclEditControl = (PACLEDITCONTROL)calloc(1, cb);
	if (m_pAclEditControl != NULL)
	{
		m_pAclEditControl->NumberOfEntries = countof(s_rgAccess);
		m_pAclEditControl->Entries = (PACLEDITENTRY)(m_pAclEditControl + 1);

		PACLEDITENTRY pEntries = m_pAclEditControl->Entries;
		PWSTR pszName = (PWSTR)(pEntries + countof(s_rgAccess));

		for (int i = 0; i < countof(s_rgAccess); i++)
		{
			pEntries[i].AccessMask = s_rgAccess[i].mask;
	
			if (pEntries[i].AccessMask == PROCESS_ALL_ACCESS)
				pEntries[i].AccessMask = GENERIC_ALL;

			if (s_rgAccess[i].dwFlags & SI_ACCESS_SPECIFIC)
				pEntries[i].Type = 2;
			else if (s_rgAccess[i].dwFlags & SI_ACCESS_GENERAL)
				pEntries[i].Type = 1;

			pEntries[i].Name = pszName;

			pszName += LoadStringW(AfxGetResourceHandle(), 
								   LOWORD(s_rgAccess[i].pszName),
								   pszName, 80) + 1;
		}
	}

	return m_pAclEditControl;
}

//---------------------------------------------------------------------------
// GetSecurityDescriptor
//
//  Returns process' security descriptor.
//
//  Parameters:
//    none.
//
//  Returns:
//    pointer to the security descriptor, if successful and FALSE - 
//    otherwise.
//
PSECURITY_DESCRIPTOR
CProcessSecInfo::GetSecurityDescriptor()
{
	GetSecurityInfo(m_hProcess, SE_KERNEL_OBJECT, 
					DACL_SECURITY_INFORMATION|OWNER_SECURITY_INFORMATION,
					NULL, NULL, NULL, NULL, &m_pSecDesc);
	return m_pSecDesc;
}

//---------------------------------------------------------------------------
// ChangeCallback
//
//  Applies the security descriptor.
//
//  Parameters:
//	  pContext  - context pointer
//	  pSecDesc  - pointer to the security descriptor
//	  pdwStatus - pointer to a variable that receives operation status
//
//  Returns:
//    Win32 error code.
//
DWORD 
CALLBACK
CProcessSecInfo::ChangeCallback(
	PVOID, 
	PVOID, 
	PVOID pContext, 
	PSECURITY_DESCRIPTOR pSecDesc, 
	PVOID,
	PVOID, 
	PVOID,
	PDWORD pdwStatus
	)
{
	PACL pDacl = NULL;
	BOOL bPresent, bDefault;
	GetSecurityDescriptorDacl(pSecDesc, &bPresent, &pDacl, &bDefault);

	CProcessSecInfo * pInfo = (CProcessSecInfo *)pContext;

	*pdwStatus = SetSecurityInfo(pInfo->m_hProcess, SE_KERNEL_OBJECT,
								 DACL_SECURITY_INFORMATION, NULL, NULL,
								 pDacl, NULL);
	return ERROR_SUCCESS;
}

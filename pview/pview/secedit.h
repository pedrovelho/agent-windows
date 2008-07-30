// secedit.h
//
// Process security editor.
//
// $Id: $
//

#ifndef __secedit_h_included
#define __secedit_h_included

typedef struct _ACLDLGCONTROL * PACLDLGCONTROL;
typedef struct _ACLEDITCONTROL * PACLEDITCONTROL;

class CProcessSecInfo :
    public ISecurityInformation
{
  public:

	// constructor
	CProcessSecInfo();
	~CProcessSecInfo();
	
	// edit process DACL
	BOOL EditDacl(HWND hWnd, DWORD dwProcessId, PCTSTR pszName);

	// check security editor availability
	static BOOL IsDaclEditorAvailable();
	
  protected:

	// IUnknown
	STDMETHOD(QueryInterface)(REFIID, PVOID *);
	STDMETHOD_(ULONG, AddRef)();
	STDMETHOD_(ULONG, Release)();

	// ISecurityInformation
	STDMETHOD(GetObjectInformation)(PSI_OBJECT_INFO);
	STDMETHOD(GetSecurity)(SECURITY_INFORMATION, PSECURITY_DESCRIPTOR *, BOOL);
	STDMETHOD(SetSecurity)(SECURITY_INFORMATION, PSECURITY_DESCRIPTOR);
	STDMETHOD(GetAccessRights)(const GUID *, DWORD, PSI_ACCESS *, PULONG, PULONG);
	STDMETHOD(MapGeneric)(const GUID *, PUCHAR, PACCESS_MASK);
	STDMETHOD(GetInheritTypes)(PSI_INHERIT_TYPE *, PULONG);
	STDMETHOD(PropertySheetPageCallback)(HWND, UINT, SI_PAGE_TYPE);

  protected:

	HANDLE					m_hProcess;
	PWSTR					m_pszProcessName;
	BOOLEAN					m_bReadOnly;
	PSECURITY_DESCRIPTOR	m_pSecDesc;
	PACLEDITCONTROL			m_pAclEditControl;
	PACLDLGCONTROL			m_pAclDlgControl;

	static const SI_ACCESS			s_rgAccess[];
	static const GENERIC_MAPPING	s_GenericMapping;

  protected:

	BOOL OpenProcessHandle(DWORD dwProcessId);
	PACLDLGCONTROL GetAclDlgControl();
	PACLEDITCONTROL GetAclEditControl();
	PSECURITY_DESCRIPTOR GetSecurityDescriptor();

	static DWORD CALLBACK ChangeCallback(PVOID, PVOID, PVOID, 
										 PSECURITY_DESCRIPTOR, PVOID, PVOID, 
										 PVOID, PDWORD);
};


#endif // __secedit_h_included


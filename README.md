# WhoamiAlternatives

## Method 1 (PRTL_USER_PROCESS_PARAMETERS)

- Function [NtQueryInformationProcess](https://learn.microsoft.com/en-us/windows/win32/api/winternl/nf-winternl-ntqueryinformationprocess) returns a "PROCESS_BASIC_INFORMATION" structure containing a pointer to the PEB base address.

- The PEB structure contains a pointer "ProcessParameters" to a [RTL_USER_PROCESS_PARAMETERS](https://www.geoffchappell.com/studies/windows/km/ntoskrnl/inc/api/pebteb/rtl_user_process_parameters.htm) structure.

- From that structure you can get a pointer "Environment" to the environment variables and a pointer "EnvironmentSize" to the size of the environment variables.

- Reading the number of bytes indicated in "EnvironmentSize" from the address "Environment" as UNICODE text, parse the environment variables and print the one called "USERNAME"

![esquema](https://raw.githubusercontent.com/ricardojoserf/ricardojoserf.github.io/master/images/stealthyenv/Screenshot_0.png)


## Method 2 (LookupAccountSid)

- Function [NtOpenProcessToken](https://learn.microsoft.com/en-us/windows-hardware/drivers/ddi/ntifs/nf-ntifs-ntopenprocesstoken) creates an access token associated with the current process.

- Function [NtQueryInformationToken](https://learn.microsoft.com/en-us/windows-hardware/drivers/ddi/ntifs/nf-ntifs-ntqueryinformationtoken) gets information from the token we created, using the value "tokenUser" (1) in the field "TOKEN_INFORMATION_CLASS" we get information about the username which is stored in the pointer "TokenInformation".

- Function [ConvertSidToStringSid](https://learn.microsoft.com/en-us/windows/win32/api/sddl/nf-sddl-convertsidtostringsida) converts the username's SID in binary format to string format.

- Function [LookupAccountSid](https://learn.microsoft.com/en-us/windows/win32/api/winbase/nf-winbase-lookupaccountsida) takes the SID in string format and returns the username. 


## Method 3 (LsaLookupSids)

- Functions NtOpenProcessToken and NtQueryInformationToken are used like in method 2, return a pointer "TokenInformation" with the user's SID in binary format. 

- Function [LsaOpenPolicy](https://learn.microsoft.com/en-us/windows/win32/api/ntsecapi/nf-ntsecapi-lsaopenpolicy) creates a handle to the Policy object in the current system.

- Function [LsaLookupSids](https://learn.microsoft.com/en-us/windows/win32/api/ntsecapi/nf-ntsecapi-lsalookupsids) takes a pointer to the SID and returns an structure LSA_TRANSLATED_NAME containing the username.


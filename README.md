# Alternatives to whoami

Some experiments to retrieve the current username without calling whoami.exe or similar binaries.


------------------------------------------------

## Method 1: PRTL_USER_PROCESS_PARAMETERS

Get the environment variables from the PEB structure and parse it to find the username.

- Function [NtQueryInformationProcess](https://learn.microsoft.com/en-us/windows/win32/api/winternl/nf-winternl-ntqueryinformationprocess) returns a "PROCESS_BASIC_INFORMATION" structure containing a pointer to the PEB base address.

- The PEB structure contains a pointer "ProcessParameters" to a [RTL_USER_PROCESS_PARAMETERS](https://www.geoffchappell.com/studies/windows/km/ntoskrnl/inc/api/pebteb/rtl_user_process_parameters.htm) structure.

- From that structure you can get a pointer "Environment" to the environment variables and a pointer "EnvironmentSize" to the size of the environment variables.

- Reading the number of bytes indicated in "EnvironmentSize" from the address "Environment" as UNICODE text, parse the environment variables and print the one called "USERNAME"

![esquema](https://raw.githubusercontent.com/ricardojoserf/ricardojoserf.github.io/master/images/stealthyenv/Screenshot_0.png)

![img](https://github.com/ricardojoserf/ricardojoserf.github.io/blob/master/images/whoamialternatives/Screenshot_1.png?raw=true)

------------------------------------------------

## Method 2: LookupAccountSid

Get access to a token, find the user's SID in string format and translate it using the function LookupAccountSid.

- Function [NtOpenProcessToken](https://learn.microsoft.com/en-us/windows-hardware/drivers/ddi/ntifs/nf-ntifs-ntopenprocesstoken) creates an access token associated with the current process.

- Function [NtQueryInformationToken](https://learn.microsoft.com/en-us/windows-hardware/drivers/ddi/ntifs/nf-ntifs-ntqueryinformationtoken) gets information from the token we created, using the value "tokenUser" (1) in the field "TOKEN_INFORMATION_CLASS" we get information about the username which is stored in the pointer "TokenInformation".

- Function [ConvertSidToStringSid](https://learn.microsoft.com/en-us/windows/win32/api/sddl/nf-sddl-convertsidtostringsida) converts the username's SID in binary format to string format.

- Function [LookupAccountSid](https://learn.microsoft.com/en-us/windows/win32/api/winbase/nf-winbase-lookupaccountsida) takes the SID in string format and returns the username. 

![esquema](https://github.com/ricardojoserf/ricardojoserf.github.io/blob/master/images/whoamialternatives/LookupAccountSid_esquema.png?raw=true)

![img](https://github.com/ricardojoserf/ricardojoserf.github.io/blob/master/images/whoamialternatives/Screenshot_2.png?raw=true)

------------------------------------------------

## Method 3: LsaLookupSids

Get acccess to a token and a Policy object and get the username with the function LsaLookupSids. 

- Functions NtOpenProcessToken and NtQueryInformationToken are used like in method 2, return a pointer "TokenInformation" with the user's SID in binary format. 

- Function [LsaOpenPolicy](https://learn.microsoft.com/en-us/windows/win32/api/ntsecapi/nf-ntsecapi-lsaopenpolicy) creates a handle to the Policy object in the current system.

- Function [LsaLookupSids](https://learn.microsoft.com/en-us/windows/win32/api/ntsecapi/nf-ntsecapi-lsalookupsids) takes a pointer to the SID and returns an structure LSA_TRANSLATED_NAME containing the username.

![esquema](https://github.com/ricardojoserf/ricardojoserf.github.io/blob/master/images/whoamialternatives/LsaLookupSids_esquema.drawio.png?raw=true)

![img](https://github.com/ricardojoserf/ricardojoserf.github.io/blob/master/images/whoamialternatives/Screenshot_3.png?raw=true)

------------------------------------------------

## Method 4: NamedPipe

Create a named pipe and a secondary thread, write and read from the named pipe and get the username from the undocumented function NpGetUsername. 

![esquema](https://github.com/ricardojoserf/ricardojoserf.github.io/blob/master/images/whoamialternatives/NamedPipe_esquema.png?raw=true)

![img](https://raw.githubusercontent.com/ricardojoserf/ricardojoserf.github.io/master/images/whoamialternatives/Screenshot_4.png)

------------------------------------------------

## Method 5: ADSystemInfo

Get username if the computer is domain joined using the [CoCreateInstance](https://learn.microsoft.com/en-us/windows/win32/api/combaseapi/nf-combaseapi-cocreateinstance) function as in [MSDN example](https://learn.microsoft.com/es-es/windows/win32/api/iads/nn-iads-iadsadsysteminfo). It uses the class ADSystemInfoClass and the interfaces ADSystemInfo and IADsADSystemInfo from ActiveDS.dll, which are already in the project folder so you don't need the DLL.

If there is no connection with the AD:

![img](https://raw.githubusercontent.com/ricardojoserf/ricardojoserf.github.io/master/images/whoamialternatives/Screenshot_5.png)

If there is connection:

![img](https://raw.githubusercontent.com/ricardojoserf/ricardojoserf.github.io/master/images/whoamialternatives/Screenshot_6.png)

------------------------------------------------

### Source

[vx-underground's Twitter account](https://twitter.com/vxunderground)

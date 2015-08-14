﻿using System;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace Build.Extensions.Security
{
    public class Impersonation : IDisposable
    {
        private WindowsImpersonationContext _impersonatedUser = null;
        private IntPtr _userHandle;

        public Impersonation(Credential credentials)
        {
            if (credentials != null && !string.IsNullOrEmpty(credentials.UserName))
            {
                _userHandle = new IntPtr(0);

                string userName = credentials.UserName;
                string userDomain = "";
                int pos = userName.IndexOf(@"\"); // domain.com\user
                if (pos > -1) { userDomain = userName.Substring(0, pos); userName = userName.Substring(pos + 1); }
                else
                {
                    pos = userName.IndexOf("@"); // user@domain.com
                    if (pos > -1) { userDomain = userName.Substring(pos + 1); userName = userName.Substring(0, pos); }
                }


                bool returnValue = LogonUser(userName, userDomain, credentials.Password, LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT, ref _userHandle);

                if (!returnValue)
                    throw new ApplicationException("Could not impersonate user");

                WindowsIdentity newId = new WindowsIdentity(_userHandle);
                _impersonatedUser = newId.Impersonate();
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (_impersonatedUser != null)
            {
                _impersonatedUser.Undo();
                CloseHandle(_userHandle);
            }
        }

        #endregion

        #region Interop imports/constants
        public const int LOGON32_LOGON_INTERACTIVE = 2;
        public const int LOGON32_LOGON_SERVICE = 3;
        public const int LOGON32_PROVIDER_DEFAULT = 0;

        [DllImport("advapi32.dll", CharSet = CharSet.Auto)]
        public static extern bool LogonUser(String lpszUserName, String lpszDomain, String lpszPassword, int dwLogonType, int dwLogonProvider, ref IntPtr phToken);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public extern static bool CloseHandle(IntPtr handle);
        #endregion
    }
}

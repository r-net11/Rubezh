using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Principal;
using HANDLE = System.IntPtr;

namespace Infrastructure.Common.Windows
{
	public static class ProcessHelper
	{
		public const int TOKEN_QUERY = 0X00000008;

		const int ERROR_NO_MORE_ITEMS = 259;

		enum TOKEN_INFORMATION_CLASS
		{
			TokenUser = 1,
			TokenGroups,
			TokenPrivileges,
			TokenOwner,
			TokenPrimaryGroup,
			TokenDefaultDacl,
			TokenSource,
			TokenType,
			TokenImpersonationLevel,
			TokenStatistics,
			TokenRestrictedSids,
			TokenSessionId
		}

		[StructLayout(LayoutKind.Sequential)]
		struct TOKEN_USER
		{
			public _SID_AND_ATTRIBUTES User;
		}
		[StructLayout(LayoutKind.Sequential)]
		public struct _SID_AND_ATTRIBUTES
		{
			public IntPtr Sid;
			public int Attributes;
		}

		[DllImport("advapi32")]
		static extern bool OpenProcessToken(
			HANDLE ProcessHandle, // handle to process
			int DesiredAccess, // desired access to process
			ref IntPtr TokenHandle // handle to open access token
		);

		[DllImport("kernel32")]
		static extern HANDLE GetCurrentProcess();

		[DllImport("advapi32", CharSet = CharSet.Auto)]
		static extern bool GetTokenInformation(
			HANDLE hToken,
			TOKEN_INFORMATION_CLASS tokenInfoClass,
			IntPtr TokenInformation,
			int tokeInfoLength,
			ref int reqLength
		);

		[DllImport("kernel32")]
		static extern bool CloseHandle(HANDLE handle);

		[DllImport("advapi32", CharSet = CharSet.Auto)]
		static extern bool ConvertSidToStringSid(
			IntPtr pSID,
			[In, Out, MarshalAs(UnmanagedType.LPTStr)] ref string pStringSid
		);

		[DllImport("advapi32", CharSet = CharSet.Auto)]
		static extern bool ConvertStringSidToSid(
			[In, MarshalAs(UnmanagedType.LPTStr)] string pStringSid,
			ref IntPtr pSID
		);

		/// <summary>
		/// Collect User Info
		/// </summary>
		/// <param name="pToken">Process Handle</param>
		public static bool DumpUserInfo(HANDLE pToken, out IntPtr SID)
		{
			int Access = TOKEN_QUERY;
			HANDLE procToken = IntPtr.Zero;
			bool ret = false;
			SID = IntPtr.Zero;
			try
			{
				if (OpenProcessToken(pToken, Access, ref procToken))
				{
					ret = ProcessTokenToSid(procToken, out SID);
					CloseHandle(procToken);
				}
				return ret;
			}
			catch (Exception)
			{
				return false;
			}
		}

		private static bool ProcessTokenToSid(HANDLE token, out IntPtr SID)
		{
			TOKEN_USER tokUser;
			const int bufLength = 256;
			IntPtr tu = Marshal.AllocHGlobal(bufLength);
			bool ret = false;
			SID = IntPtr.Zero;
			try
			{
				int cb = bufLength;
				ret = GetTokenInformation(token,
						TOKEN_INFORMATION_CLASS.TokenUser, tu, cb, ref cb);
				if (ret)
				{
					tokUser = (TOKEN_USER)Marshal.PtrToStructure(tu, typeof(TOKEN_USER));
					SID = tokUser.User.Sid;
				}
				return ret;
			}
			catch (Exception)
			{
				return false;
			}
			finally
			{
				Marshal.FreeHGlobal(tu);
			}
		}

		public static string ExGetProcessInfoByPID
			(int PID, out string SID)//, out string OwnerSID)
		{
			IntPtr _SID = IntPtr.Zero;
			SID = String.Empty;
			try
			{
				Process process = Process.GetProcessById(PID);
				if (DumpUserInfo(process.Handle, out _SID))
				{
					ConvertSidToStringSid(_SID, ref SID);
				}
				return process.ProcessName;
			}
			catch
			{
				return "Unknown";
			}
		}

		public static bool IsCurrentUserProcess(int ProcessID)
		{
			WindowsIdentity _user = WindowsIdentity.GetCurrent();
			string stringSID = String.Empty;
			string process = ExGetProcessInfoByPID(ProcessID, out stringSID);
			return stringSID == _user.User.Value;
		}
	}
}
using Common;
using Defender;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infrastructure.Common
{
	public class FiresecLicenseWrapper
	{
		bool _isNull;
		InitialKey _initialKey;
		string _version = "1.0";
		int _remoteWorkplacesCount;
		bool _fire;
		bool _security;
		bool _access;
		bool _video;
		bool _opcServer;

		public bool IsNull 
		{
			get { return _isNull; } 
			private set
			{
				_isNull = value;
				if (_isNull)
				{
					_remoteWorkplacesCount = 0;
					_fire = _security = _access = _video = _opcServer = false;
				}
			}
		}

		public InitialKey InitialKey
		{
			get { return _initialKey; }
			set 
			{ 
				_initialKey = value; 
				if (_initialKey != null)
					IsNull = false; 
			}
		}

		public string Version
		{
			get { return _version; }
			set { _version = value; }
		}

		public License License
		{
			get { return Assemble(); }
			set { Disassemble(value); }
		}
		
		public int RemoteWorkplacesCount
		{
			get { return _remoteWorkplacesCount; }
			set { _remoteWorkplacesCount = value; IsNull = false; }
		}

		public bool Fire
		{
			get { return _fire; }
			set { _fire = value; IsNull = false; }
		}
		
		public bool Security
		{
			get { return _security; }
			set { _security = value; IsNull = false; }
		}
		
		public bool Access
		{
			get { return _access; }
			set { _access = value; IsNull = false; }
		}
		
		public bool Video
		{
			get { return _video; }
			set { _video = value; IsNull = false; }
		}

		public bool OpcServer
		{
			get { return _opcServer; }
			set { _opcServer = value; IsNull = false; }
		}
		
		public FiresecLicenseWrapper(InitialKey initialKey)
		{
			InitialKey = initialKey;
		}

		public FiresecLicenseWrapper(License license)
		{
			if (license != null)
				InitialKey = license.InitialKey;
			License = license;
		}

		License Assemble()
		{
			if (IsNull)
				return null;

			var result = License.Create(InitialKey);
			result.Parameters.Add(new LicenseParameter("version", "Версия", Version));
			result.Parameters.Add(new LicenseParameter("remoteWorkplacesCount", "GLOBAL Удаленное рабочее место (количество)", RemoteWorkplacesCount));
			result.Parameters.Add(new LicenseParameter("fire", "GLOBAL Пожаротушение", Fire));
			result.Parameters.Add(new LicenseParameter("security", "GLOBAL Охрана", Security));
			result.Parameters.Add(new LicenseParameter("access", "GLOBAL Доступ", Access));
			result.Parameters.Add(new LicenseParameter("video", "GLOBAL Видео", Video));
			result.Parameters.Add(new LicenseParameter("opcServer", "GLOBAL OPC сервер", OpcServer));
			return result;
		}

		void Disassemble(License value)
		{
			if (value == null)
			{
				IsNull = true;
				return;
			}

			try
			{
				LicenseParameter parameter = value.Parameters.FirstOrDefault(x => x.Id == "version");
				Version = parameter == null ? null : parameter.Value.ToString();

				RemoteWorkplacesCount = (int)value.Parameters.First(x => x.Id == "remoteWorkplacesCount").Value;
				Fire = (bool)value.Parameters.First(x => x.Id == "fire").Value;
				Security = (bool)value.Parameters.First(x => x.Id == "security").Value;
				Access = (bool)value.Parameters.First(x => x.Id == "access").Value;
				Video = (bool)value.Parameters.First(x => x.Id == "video").Value;
				OpcServer = (bool)value.Parameters.First(x => x.Id == "opcServer").Value;
			}
			catch (Exception e)
			{
				IsNull = true;
				Logger.Error(e, "FiresecLicenseWrapper.Disassemble");
			}

		}
	}
}

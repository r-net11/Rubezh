using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpcEnumLib;

namespace OpcDaServer
{
	/// <summary>
	/// OPC DA сервер
	/// </summary>
	public class OpcDaServer
	{
		#region Methods

		/// <summary>
		/// Возвращает список зарегистрированных в систмеме OPC DA серверов
		/// </summary>
		/// <returns></returns>
		public static OpcDaServer[] GetRegistredServers()
		{
			List<OpcDaServer> list = new List<OpcDaServer>();
			// Создаём объект списка OPC серверов
			OpcServerList pServerList = new OpcServerList();
			// Идентификатор категории OPC DA 2.0
			Guid clsidcat = new Guid("{63D5F432-CFE4-11D1-B2C8-0060083BA1FB}");
			// перечислитель, в котором будут храниться GUID серверов
			IOPCEnumGUID pIOPCEnumGuid;
			// запрос по группе серверов спецификации OPC DA 2.0
			try
			{
				pServerList.EnumClassesOfCategories(1, ref clsidcat, 0, ref clsidcat, out pIOPCEnumGuid);

				string pszProgID; // буфер для записи ProgID серверов
				string pszUserType; // буфер для записи описания серверов
				string pszVerIndProgId;
				Guid guid = new Guid();
				//int nServerCnt = 0;
				uint iRetSvr; // количество серверов, предоставленных запросом

				// Получение идентификаторов серверов
				pIOPCEnumGuid.Next(1, out guid, out iRetSvr);
				while (iRetSvr != 0)
				{
					//nServerCnt++;
					pServerList.GetClassDetails(ref guid, out pszProgID, out pszUserType, out pszVerIndProgId);
					list.Add(new OpcDaServer
					{
						Id = guid,
						ServerName = pszProgID
					});
					pIOPCEnumGuid.Next(1, out guid, out iRetSvr);
				}
			}
			catch
			{

			}
			return list.ToArray();
		}

		#endregion

		#region Fields And Properties

		public Guid Id { get; set; }

		public string ServerName { get; set; }

		#endregion
	}
}

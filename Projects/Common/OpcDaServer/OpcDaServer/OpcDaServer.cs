using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using OpcEnumLib;
using opcproxy;

namespace OpcDaServer
{
	/// <summary>
	/// OPC DA сервер
	/// </summary>
	public class OpcDaServer
	{
		#region Constructors

		public OpcDaServer()
		{
			Id = Guid.Empty;
		}

		#endregion

		#region Fields And Properties

		public Guid Id { get; set; }

		public string ServerName { get; set; }

		/// <summary>
		/// Возвращает объкт структуры директорий и тегов OPC DA сервера
		/// </summary>
		public OpcDaDirectory Tags
		{
			get 
			{
				OpcDaDirectory rootDirectory = null;

				if (Id == Guid.Empty)
				{
					return rootDirectory;
				}

				try
				{
					Type typeOfServer = Type.GetTypeFromCLSID(Id);
					var s = Activator.CreateInstance(typeOfServer);

					//if (m_pIfaceObj is IOPCBrowse) // Код для спецификации DA 3.0
					//{
					//	IOPCBrowse pBrowse = (IOPCBrowse)m_pIfaceObj;
					//	DisplayChildren(null, "", pBrowse);
					//}
					//else
					{
						if (s is IOPCBrowseServerAddressSpace)
						{
							IOPCBrowseServerAddressSpace pBrowse = (IOPCBrowseServerAddressSpace)s;
							BuildTreeTags(ref rootDirectory, pBrowse);
						}
					}
					return rootDirectory;
				}
				catch //(ApplicationException ex)
				{
					//string msg;
					// Получаем HRESULT, соответсвующий сгенерированному исключению
					//int hRes = Marshal.GetHRForException(ex);
					// Запрашиваем у сервера текст ошибки
					// pServer.GetErrorString(hRes, 2, out msg);

					//MessageBox.Show(ex.Message, "Ошибка");

					throw;
				}
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// Получает дерево тегов OPC DA сервера
		/// </summary>
		/// <param name="parentDirectory">При первом вызове передать null - 
		/// будеть создана корневая директория</param>
		/// <param name="server"></param>
		void BuildTreeTags(ref OpcDaDirectory parentDirectory, IOPCBrowseServerAddressSpace server)
		{
			opcproxy.IEnumString ptrEnum;
			uint cnt;
			string strName;
			string szItemID;

			if (parentDirectory == null)
			{
				parentDirectory = new OpcDaDirectory(); // Создаём корневую директорию
			}


			// Получаем все теги
			server.BrowseOPCItemIDs(tagOPCBROWSETYPE.OPC_LEAF, "", (ushort)VarEnum.VT_EMPTY, 0, out ptrEnum);
			ptrEnum.RemoteNext(1, out strName, out cnt);
			while (cnt != 0)
			{
			
				// получаем полный идентификатор тега
				server.GetItemID(strName, out szItemID);
				parentDirectory.Tags.Add(new OpcDaTag(szItemID, strName, parentDirectory));
				ptrEnum.RemoteNext(1, out strName, out cnt);
			}

			// Получем директории 
			server.BrowseOPCItemIDs(tagOPCBROWSETYPE.OPC_BRANCH, "", 1, 0, out ptrEnum);
			ptrEnum.RemoteNext(1, out strName, out cnt);
			while (cnt != 0)
			{
				server.ChangeBrowsePosition(tagOPCBROWSEDIRECTION.OPC_BROWSE_DOWN, strName);
				var childDirectory = new OpcDaDirectory(strName, parentDirectory);
				BuildTreeTags(ref childDirectory, server);
				server.ChangeBrowsePosition(tagOPCBROWSEDIRECTION.OPC_BROWSE_UP, strName);
				ptrEnum.RemoteNext(1, out strName, out cnt);
			}
		}

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
	}
}

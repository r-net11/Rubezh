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
	public class OpcDaServer: IDisposable
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
					if (!IsConnected)
					{
						Connect();
					}

					//if (m_pIfaceObj is IOPCBrowse) // Код для спецификации DA 3.0
					//{
					//	IOPCBrowse pBrowse = (IOPCBrowse)m_pIfaceObj;
					//	DisplayChildren(null, "", pBrowse);
					//}
					//else
					{
						if (_server is IOPCBrowseServerAddressSpace)
						{
							IOPCBrowseServerAddressSpace pBrowse = (IOPCBrowseServerAddressSpace)_server;
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

					// Для отладки
					return rootDirectory = new OpcDaDirectory();
					//throw;
				}
			}
		}

		object _server = null;
		uint m_hGroup; // Описатель группы
		public bool IsConnected
		{
			get { return _server != null; }
		}

		#endregion

		#region Methods

		/// <summary>
		/// Подключиться к OPC Сереверу
		/// </summary>
		public void Connect()
		{
			try
			{
				Type typeOfServer = Type.GetTypeFromCLSID(Id);
				_server = Activator.CreateInstance(typeOfServer);
			}
			catch (ApplicationException ex)
			{
				//string msg;
				// Получаем HRESULT, соответсвующий сгенерированному исключению
				//int hRes = Marshal.GetHRForException(ex);
				// Запрашиваем у сервера текст ошибки
				// pServer.GetErrorString(hRes, 2, out msg);
				throw;
			}
		}

		public void Disconnect()
		{
			_server = null;
		}

		public OpcDaTagValue ReadTag(string tagId)
		{
			if (!IsConnected)
			{
				throw new InvalidOperationException("Невозможно прочитать тег. OPC сервер не подключен");
			}

			var tag = Find(tagId);
			
			if (tag == null)
			{
				throw new InvalidOperationException("Невозможно прочитать тег. Тег не найден");
			}

			// Запрашиваем интерфейс IOPServer
			IOPCServer pServer = (IOPCServer)_server;

			uint updateRate = 1000; // время опроса создаваемой группы
			int bActive = 1; // активность группы - активна
			uint hClientGroup = 1; // клиентский описатель группы

			object iFaceObj; // сюда вернем интерфейс к группе
			Guid riid = typeof(IOPCItemMgt).GUID;

			int TimeBias = 0; // Не используем смещения по времени
			float DeadBand = 0; // Не используем зону нечувствительности

			try
			{
				// Этот участок на данном этапе должен быть закомментирован. Его
				// необходимо будет использовать тогда, когда добавим асинхронную
				// операцию чтения по подписке
				//if (m_dwCookie != 0)
				//{
				//	m_pDataCallback.Unadvise(m_dwCookie);
				//	m_dwCookie = 0;
				//}

				if (m_hGroup != 0) // Если группа была создана
				{
					pServer.RemoveGroup(m_hGroup, 1); // удалим её
					m_hGroup = 0;
				}

				pServer.AddGroup("MyGroup", bActive, updateRate, hClientGroup,
					ref TimeBias, ref DeadBand, 2, out m_hGroup, out updateRate,
					ref riid, out iFaceObj);
				IOPCItemMgt pItemMgt = (IOPCItemMgt)iFaceObj;

				// Создаём список элементов для добавления в группу, размером 1 элемент
				uint dwCount = 1;

				// Создаём описатель добавляемого элемента
				tagOPCITEMDEF pItems = new tagOPCITEMDEF();
				pItems.szItemID = tag.TagId;
				pItems.szAccessPath = null;
				pItems.bActive = 1;
				pItems.hClient = 1;
				pItems.vtRequestedDataType = (ushort)VarEnum.VT_EMPTY;
				pItems.dwBlobSize = 0;
				pItems.pBlob = IntPtr.Zero;

				// В эти две переменные будут записаны массивы ошибок и 
				// результатов выполнения

				IntPtr iptrErrors = IntPtr.Zero;
				IntPtr iptrResults = IntPtr.Zero;

				// Добавляем элемент данных в группу
				pItemMgt.AddItems(dwCount, pItems, out iptrResults, out iptrErrors);

				// Переносим результаты и ошибки зи неуправляемой памяти в управляемую
				tagOPCITEMRESULT pResults =
					(tagOPCITEMRESULT)Marshal.PtrToStructure(iptrResults, typeof(tagOPCITEMRESULT));
				int[] hRes = new int[1];
				Marshal.Copy(iptrResults, hRes, 0, 1);

				// Генерируем исключение в случае ошибки в HRESULT
				Marshal.ThrowExceptionForHR(hRes[0]);

				// Получаем интерфейс IOPCSyncIO для операций синхронного чтения
				IOPCSyncIO pSyncIO = (IOPCSyncIO)iFaceObj;

				// В эту переменную будут записаны результаты чтения
				IntPtr iptrItemState = IntPtr.Zero;

				iptrErrors = IntPtr.Zero;

				// Читаем данные из сервера 
				pSyncIO.Read(tagOPCDATASOURCE.OPC_DS_DEVICE, 1, ref pResults.hServer, out iptrItemState, out iptrErrors);

				// Переносим результаты и ошибки из неуправляемой памяти в управляемую
				tagOPCITEMSTATE pItemState =
					(tagOPCITEMSTATE)Marshal.PtrToStructure(iptrItemState, typeof(tagOPCITEMSTATE));

				Marshal.Copy(iptrErrors, hRes, 0, 1);

				// Генерируем исключение в случае ошибки в HRESULT
				Marshal.ThrowExceptionForHR(hRes[0]);

				return new OpcDaTagValue(tag.TagId, pResults.vtCanonicalDataType,
					pItemState.vDataValue, pItemState.ftTimeStamp, pItemState.wQuality);
			}
			catch (System.Exception ex)
			{
				//string msg;
				//Получаем HRESULT соответствующий сгененрированному исключению
				//int hRes = Marshal.GetHRForException(ex);
				//Запрашиваем у сервера текст ошибки, соответствующий текущему HRESULT 
				//pServer.GetErrorString(hRes, 2, out msg);
				//Показываем сообщение ошибки
				//MessageBox.Show(msg, "Ошибка");
				throw;
			}
		}

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
				parentDirectory.Items.Add(new OpcDaTag(szItemID, strName, parentDirectory));
				ptrEnum.RemoteNext(1, out strName, out cnt);
			}

			// Получем директории 
			server.BrowseOPCItemIDs(tagOPCBROWSETYPE.OPC_BRANCH, "", 1, 0, out ptrEnum);
			ptrEnum.RemoteNext(1, out strName, out cnt);
			while (cnt != 0)
			{
				server.ChangeBrowsePosition(tagOPCBROWSEDIRECTION.OPC_BROWSE_DOWN, strName);
				var childDirectory = new OpcDaDirectory(strName, parentDirectory);
				parentDirectory.Items.Add(childDirectory);
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

		public OpcDaTag Find(string tagId)
		{
			// Обходбим всё дерево в поисках тега
			return Find(Tags, tagId);
		}

		OpcDaTag Find(OpcDaDirectory directory, string tagId)
		{
			OpcDaTag result = null;

			var tag = directory.Items
				.Where(x => !x.IsDirectory)
				.Select(x => (OpcDaTag)x)
				.FirstOrDefault(t => t.TagId == tagId);

			if (tag != null)
			{
				return tag;
			}

			// Обходим вложенные директории
			var directories = directory.Items
				.Where(x => x.IsDirectory)
				.Select(x => (OpcDaDirectory)x);


			foreach (var dir in directories)
			{
				result = Find(dir, tagId);
				if (result != null)
				{
					return result;
				}
			}

			return result;
		}

		public void Dispose()
		{
			if (IsConnected)
			{
				Disconnect();
			}
		}

		#endregion
	}
}

using Infrastructure.Automation;
using OpcClientSdk;
using OpcClientSdk.Da;
using RubezhAPI.Automation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RubezhService.Processor
{
	public static class OpcDaServersProcessor
	{
		static OpcDaServersProcessor()
		{
			_Servers = new List<Tuple<TsCDaServer, TsCDaSubscription>>();
			_tags = new List<OpcDaTagValue>();

			LicenseHandler.Validate("Technosoftware GmbH", "GHSV-X3ZA-9CR6-PH92-32WZ-H52U-4BR4-NQU5-4FVR-E3VR-WERE-Q674-Y9UJ-J5HE-JPMV-RVU2-2C9Y-A42X-XX7T-B");
		}

		#region Fields And Properties

		static List<Tuple<TsCDaServer, TsCDaSubscription>> _Servers;

		static List<OpcDaTagValue> _tags;

		public static OpcDaServer[] OpcDaServers { get; private set; }

		#endregion

		#region Methods

		public static void Start()
		{
			var servers = GetOpcDaServers();

			OpcDaServers =
				ConfigurationCashHelper.SystemConfiguration.AutomationConfiguration.OpcDaTsServers.ToArray();

			foreach (var server in OpcDaServers)
			{
				if (!servers.Any(x => x.Url == server.Url))
				{
					Notifier.UILog(string.Format("Не удалось запусть OPC DA сервер {0}. Сервер не найден", server.Url), true);
					//MainViewModel.SetReportAddress("<Ошибка>");
					continue;
				}
				var url = new OpcUrl(OpcSpecification.OPC_DA_20, OpcUrlScheme.DA, server.Url);
				var opcServer = new TsCDaServer();

				try
				{
					opcServer.Connect(url, null);
					opcServer.ServerShutdownEvent += reason =>
					{
						var srv = _Servers.FirstOrDefault(x => x.Item1.ServerName == opcServer.ServerName);
						if (srv != null)
						{
							try
							{
								srv.Item1.CancelSubscription(srv.Item2);
							}
							catch { }

							// Ищем все теги для данного сервера у удаляем их
							var tags = _tags.Where(t => t.ServerName == opcServer.ServerName);

							foreach (var tag in tags)
							{
								_tags.Remove(tag);
							}

							_Servers.Remove(srv);
						}
					};

					// Создаём объект подписки
					var id = Guid.NewGuid().ToString();
					var subscriptionState = new TsCDaSubscriptionState
					{
						Name = id,
						ClientHandle = id,
						Deadband = 0,
						UpdateRate = 1000,
						KeepAlive = 10000
					};

					var subscription = (TsCDaSubscription)opcServer.CreateSubscription(subscriptionState);

					_Servers.Add(Tuple.Create<TsCDaServer, TsCDaSubscription>(opcServer, subscription));

					// Добавляем в объект подписки выбранные теги
					List<TsCDaItem> list = server.Tags.Select(tag => new TsCDaItem
						{
							ItemName = tag.ElementName,
							ClientHandle = tag.ElementName // Уникальный Id определяемый пользователем
						}).ToList();

					// Добавляем теги и проверяем результат данной операции
					var results = subscription.AddItems(list.ToArray());

					var errors = results.Where(result => result.Result.IsError());

					if (errors.Count() > 0)
					{
						StringBuilder msg = new StringBuilder();
						msg.Append("Не удалось добавить теги для подписки. Возникли ошибки в тегах:");
						foreach (var error in errors)
						{
							msg.Append(String.Format("ItemName={0} ClientHandle={1} Description={2}; ",
								error.ItemName, error.ClientHandle, error.Result.Description()));
						}
						//throw new InvalidOperationException(msg.ToString());
					}

					subscription.DataChangedEvent += EventHandler_Subscription_DataChangedEvent;

					_tags.AddRange(server.Tags.Select(tag => new OpcDaTagValue
					{
						ElementName = tag.ElementName,
						TagId = tag.TagId,
						Uid = tag.Uid,
						Path = tag.Path,
						TypeNameOfValue = tag.TypeNameOfValue,
						AccessRights = tag.AccessRights,
						ScanRate = tag.ScanRate,
						ServerId = server.Uid,
						ServerName = server.ServerName
					}));
				}
				catch (Exception ex)
				{
					Notifier.UILog(ex.Message, true);
				}
			}
		}

		public static void Stop()
		{
			_tags.Clear();

			foreach (var server in _Servers)
			{
				// Прекращем все подписки
				var subscription = server.Item2;
				server.Item1.CancelSubscription(subscription);
				subscription.DataChangedEvent -= EventHandler_Subscription_DataChangedEvent;
				subscription.Dispose();

				// Отключаемся от сервера
				server.Item1.Disconnect();
				server.Item1.Dispose();
			}

			_Servers.Clear();
		}

		public static void SetNewConfig()
		{
			Stop();
			Start();
		}

		public static OpcDaServer[] GetOpcDaServers()
		{
			OpcDaServer[] result;

			try
			{
				result = (OpcDiscovery.GetServers(OpcSpecification.OPC_DA_20)
				.Select(srv => new OpcDaServer
				{
					IsChecked = false,
					Login = string.Empty,
					Password = string.Empty,
					ServerName = srv.ServerName,
					Url = srv.Url.ToString()
				})
				.ToArray());
			}
			catch (Exception ex)
			{
				result = new OpcDaServer[0];
				Notifier.UILog(ex.Message, true);
			}

			return result;
		}

		public static OpcServerStatus GetServerStatus(OpcDaServer server)
		{
			var opcServer = _Servers.FirstOrDefault(x => x.Item1.ServerName == server.ServerName);

			if (opcServer == null)
			{
				var srv = GetOpcDaServers().FirstOrDefault(s => s.ServerName == server.ServerName);
				if (srv != null)
				{
					var url = new OpcUrl(OpcSpecification.OPC_DA_20, OpcUrlScheme.DA, server.Url);
					var opcSrv = new TsCDaServer();
					try
					{
						opcSrv.Connect(url, null);
						var result = opcSrv.GetServerStatus();
						opcSrv.Disconnect();
						return result;
					}
					catch
					{
						return null;
					}
				}
				else
					return null;
			}
			else
			{
				return opcServer.Item1.IsConnected ? opcServer.Item1.GetServerStatus() : null;
			}
		}

		static TsCDaBrowseElement[] Browse(TsCDaServer server)
		{
			TsCDaBrowsePosition position;
			var path = new OpcItem();

			var filters = new TsCDaBrowseFilters();
			filters.BrowseFilter = TsCDaBrowseFilter.All;
			filters.ReturnAllProperties = true;
			filters.ReturnPropertyValues = true;

			var elementList = new List<TsCDaBrowseElement>();
			var elements = server.Browse(path, filters, out position);

			foreach (var item in elements)
			{
				item.ItemPath = OpcDaTag.ROOT + OpcDaTag.SPLITTER + item.ItemName;
				elementList.Add(item);

				if (!item.IsItem)
				{
					path = new OpcItem(item.ItemPath, item.Name);
					BrowseChildren(path, filters, elementList, server);
				}

			}
			return elementList.ToArray();
		}

		static void BrowseChildren(OpcItem opcItem, TsCDaBrowseFilters filters,
			IList<TsCDaBrowseElement> elementList, TsCDaServer server)
		{
			TsCDaBrowsePosition position;
			OpcItem path;

			var elements = server.Browse(opcItem, filters, out position);

			if (elements != null)
			{
				foreach (var item in elements)
				{
					item.ItemPath = opcItem.ItemPath + OpcDaTag.SPLITTER + item.ItemName;
					elementList.Add(item);

					if (!item.IsItem)
					{
						path = new OpcItem(item.ItemPath, item.ItemName);
						BrowseChildren(path, filters, elementList, server);
					}
				}
			}
		}

		public static OpcDaElement[] GetOpcDaServerGroupAndTags(OpcDaServer server)
		{
			OpcDaElement[] result;
			var srv = _Servers.FirstOrDefault(x => x.Item1.ServerName == server.ServerName);
			var opcServer = srv == null ? null : srv.Item1;

			if (opcServer == null)
			{
				opcServer = (TsCDaServer)OpcDiscovery.GetServers(OpcSpecification.OPC_DA_20)
					.FirstOrDefault(y => y.ServerName == server.ServerName);

				if (opcServer == null)
				{
					result = null;
				}
				else
				{
					opcServer.Connect(new OpcUrl(OpcSpecification.OPC_DA_20, OpcUrlScheme.DA, server.Url), null);
					result = Browse(opcServer).Select(tag =>
					{
						return tag.IsItem ? OpcDaElement.Create(tag) : OpcDaElement.Create(tag);
					})
					.ToArray(); ;
					opcServer.Disconnect();
				}
			}
			else
			{
				if (opcServer.IsConnected)
				{
					result = Browse(opcServer)
						.Select(tag =>
						{
							return tag.IsItem ? OpcDaElement.Create(tag) : OpcDaElement.Create(tag);
						})
						.ToArray();
				}
				else
				{
					result = null;
				}
			}
			return result;
		}

		public static bool WriteTag(Guid tagId, object value, out string error)
		{
			var opcTag = _tags.FirstOrDefault(t => t.Uid == tagId);
			var server = _Servers.FirstOrDefault(srv => srv.Item1.ServerName == opcTag.ServerName);

			if (value.GetType().ToString() != opcTag.TypeNameOfValue)
			{
				try
				{
					if (opcTag.TypeNameOfValue == typeof(int).ToString())
					{
						value = Convert.ToInt32(value);
					}
					else if (opcTag.TypeNameOfValue == typeof(double).ToString())
					{
						value = Convert.ToDouble(value);
					}
					else if (opcTag.TypeNameOfValue == typeof(float).ToString())
					{
						value = Convert.ToSingle(value);
					}
					else
					{
						error = "Тип данный заначения тега не соответствует заданному. Не удалось привести типы";
						return false;
					}
				}
				catch (Exception ex)
				{
					error = String.Format("Тип данный заначения тега не соответствует заданному. Не удалось привести типы. {0}",
						ex.Message);
					return false;
				}
			}

			var subscription = server.Item2;
			var tag = subscription.Items.FirstOrDefault(t => t.ItemName == opcTag.ElementName);

			var item = new TsCDaItemValue(tag)
			{
				Value = value,
				Timestamp = DateTime.Now,
				Quality = TsCDaQuality.Good
			};

			var result = server.Item1.Write(new TsCDaItemValue[] { item });

			if (result.Length == 1)
			{
				error = string.Format("Code: {0}; Description: {1}; Name: {2}",
					result[0].Result.Code, result[0].Result.Description(), result[0].Result.Name);
				return !result[0].Result.IsError();
			}
			else
			{
				error = string.Format("Неопределённый результат операции записи тега");
				return false;
			}
		}

		public static OpcDaTagValue[] ReadTags(OpcDaServer server)
		{
			return _tags.Where(tag => tag.ServerId == server.Uid).ToArray();
		}

		#endregion

		#region Event handlers

		static void EventHandler_Subscription_DataChangedEvent(object subscriptionHandle,
			object requestHandle, TsCDaItemValueResult[] values)
		{
			var server = _Servers.FirstOrDefault(s => s.Item2.ClientHandle == subscriptionHandle);

			if (values.Length > 0)
			{
				foreach (var result in values)
				{
					var tag = _tags.FirstOrDefault(tg => tg.ServerName == server.Item1.ServerName &&
						tg.ElementName == result.ItemName);
					if (tag != null)
					{
						tag.Quality = result.Quality.GetCode();
						tag.Timestamp = result.Timestamp;
						tag.Value = result.Value;
						tag.OperationResult = result.Result.IsSuccess();
						OpcDaHelper.OnReadTagValue(tag.Uid, tag.Value);
						//AutomationProcessor.RunOnOpcTagFilters(null, null);
					}
				}
			}
		}

		#endregion
	}
}
using System.Collections.Generic;
using System.Linq;
using Common;
using Firesec.Models.ZonesLogic;
using FiresecAPI;
using FiresecAPI.Models;

namespace Firesec
{
	public static class ZoneLogicConverter
	{
		public static ZoneLogic Convert(DeviceConfiguration deviceConfiguration, expr innerZoneLogic)
		{
			var zoneLogic = new ZoneLogic();

			if (innerZoneLogic != null && innerZoneLogic.clause.IsNotNullOrEmpty())
			{
				foreach (var innerClause in innerZoneLogic.clause)
				{
					var clause = new Clause();
					if (innerClause.zone != null)
					{
						foreach (var item in innerClause.zone)
						{
							if (string.IsNullOrWhiteSpace(item) == false)
							{
								var zoneNo = int.Parse(item);
								var zone = deviceConfiguration.Zones.FirstOrDefault(x => x.No == zoneNo);
								if (zone != null)
								{
									clause.ZoneUIDs.Add(zone.UID);
								}
							}
						}
					}
					if (innerClause.device != null)
					{
						foreach (var innerDevice in innerClause.device)
						{
							if (innerDevice != null)
							{
								if (!string.IsNullOrEmpty(innerDevice.UID))
								{
									clause.DeviceUIDs.Add(GuidHelper.ToGuid(innerDevice.UID));
								}
							}
						}
					}

					clause.State = (ZoneLogicState)int.Parse(innerClause.state);

					switch (innerClause.operation)
					{
						case "and":
							clause.Operation = ZoneLogicOperation.All;
							break;

						case "or":
							clause.Operation = ZoneLogicOperation.Any;
							break;

						default:
							clause.Operation = null;
							break;
					}

					switch (innerClause.joinOperator)
					{
						case "and":
							zoneLogic.JoinOperator = ZoneLogicJoinOperator.And;
							break;

						case "or":
							zoneLogic.JoinOperator = ZoneLogicJoinOperator.Or;
							break;
					}

					if (!string.IsNullOrEmpty(innerClause.MRO_MessageNo))
					{
						switch (innerClause.MRO_MessageNo)
						{
							case "0":
								clause.ZoneLogicMROMessageNo = ZoneLogicMROMessageNo.Message1;
								break;

							case "1":
								clause.ZoneLogicMROMessageNo = ZoneLogicMROMessageNo.Message2;
								break;

							case "2":
								clause.ZoneLogicMROMessageNo = ZoneLogicMROMessageNo.Message3;
								break;

							case "3":
								clause.ZoneLogicMROMessageNo = ZoneLogicMROMessageNo.Message4;
								break;

							case "4":
								clause.ZoneLogicMROMessageNo = ZoneLogicMROMessageNo.Message5;
								break;

							case "5":
								clause.ZoneLogicMROMessageNo = ZoneLogicMROMessageNo.Message6;
								break;

							case "6":
								clause.ZoneLogicMROMessageNo = ZoneLogicMROMessageNo.Message7;
								break;

							case "7":
								clause.ZoneLogicMROMessageNo = ZoneLogicMROMessageNo.Message8;
								break;
						}
					}

					if (!string.IsNullOrEmpty(innerClause.MRO_MessageType))
					{
						switch (innerClause.MRO_MessageType)
						{
							case "0":
								clause.ZoneLogicMROMessageType = ZoneLogicMROMessageType.Add;
								break;

							case "1":
								clause.ZoneLogicMROMessageType = ZoneLogicMROMessageType.Remove;
								break;
						}
					}

					zoneLogic.Clauses.Add(clause);
				}
			}

			return zoneLogic;
		}

		public static expr ConvertBack(ZoneLogic zoneLogic)
		{
			var innerZoneLogic = new expr();

			var innerClauses = new List<clauseType>();
			foreach (var clause in zoneLogic.Clauses)
			{
				var innerClause = new clauseType();
				innerClause.state = ((int)clause.State).ToString();

				switch (clause.Operation)
				{
					case ZoneLogicOperation.All:
						innerClause.operation = "and";
						break;

					case ZoneLogicOperation.Any:
						innerClause.operation = "or";
						break;

					default:
						innerClause.operation = null;
						break;
				}

				switch (zoneLogic.JoinOperator)
				{
					case ZoneLogicJoinOperator.And:
						innerClause.joinOperator = "and";
						break;

					case ZoneLogicJoinOperator.Or:
						innerClause.joinOperator = "or";
						break;

					default:
						innerClause.joinOperator = null;
						break;
				}

				if (clause.ZoneLogicMROMessageNo != ZoneLogicMROMessageNo.Message1)
				{
					switch (clause.ZoneLogicMROMessageNo)
					{
						case ZoneLogicMROMessageNo.Message1:
							innerClause.MRO_MessageNo = "0";
							break;

						case ZoneLogicMROMessageNo.Message2:
							innerClause.MRO_MessageNo = "1";
							break;

						case ZoneLogicMROMessageNo.Message3:
							innerClause.MRO_MessageNo = "2";
							break;

						case ZoneLogicMROMessageNo.Message4:
							innerClause.MRO_MessageNo = "3";
							break;

						case ZoneLogicMROMessageNo.Message5:
							innerClause.MRO_MessageNo = "4";
							break;

						case ZoneLogicMROMessageNo.Message6:
							innerClause.MRO_MessageNo = "5";
							break;

						case ZoneLogicMROMessageNo.Message7:
							innerClause.MRO_MessageNo = "6";
							break;

						case ZoneLogicMROMessageNo.Message8:
							innerClause.MRO_MessageNo = "7";
							break;

						default:
							innerClause.MRO_MessageNo = null;
							break;
					}
				}

				if (clause.ZoneLogicMROMessageType != ZoneLogicMROMessageType.Add)
				{
					switch (clause.ZoneLogicMROMessageType)
					{
						case ZoneLogicMROMessageType.Add:
							innerClause.MRO_MessageNo = "0";
							break;

						case ZoneLogicMROMessageType.Remove:
							innerClause.MRO_MessageNo = "1";
							break;

						default:
							innerClause.MRO_MessageNo = null;
							break;
					}
				}

				if (clause.DeviceUIDs != null && clause.DeviceUIDs.Count > 0)
				{
					var deviceTypes = new List<deviceType>();
					foreach (var deviceUID in clause.DeviceUIDs)
					{
						var deviceType = new deviceType() { UID = deviceUID.ToString() };
						deviceTypes.Add(deviceType);
					}
					innerClause.device = deviceTypes.ToArray();
				}

				innerClause.zone = clause.Zones.Select(x => x.No.ToString()).ToArray();
				innerClauses.Add(innerClause);
			}

			innerZoneLogic.clause = innerClauses.ToArray();
			return innerZoneLogic;
		}
	}
}
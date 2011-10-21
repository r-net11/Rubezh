using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Firesec.ZonesLogic;
using FiresecAPI.Models;

namespace FiresecService.Converters
{
    public static class ZoneLogicConverter
    {
        public static ZoneLogic Convert(expr innerZoneLogic)
        {
            var zoneLogic = new ZoneLogic();

            if (innerZoneLogic != null && innerZoneLogic.clause.IsNotNullOrEmpty())
            {
                foreach (var innerClause in innerZoneLogic.clause)
                {
                    var clause = new Clause();
                    if (innerClause.zone != null)
                    {
                        clause.Zones = new List<ulong?>();
                        foreach (var zoneNo in innerClause.zone)
                        {
                            clause.Zones.Add(ulong.Parse(zoneNo));
                        }
                    }

                    clause.State = (ZoneLogicState) int.Parse(innerClause.state);

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

                        default:
                            zoneLogic.JoinOperator = null;
                            break;
                    }

                    if (innerClause.device != null)
                    {
                        clause.DeviceUID = GuidHelper.ToGuid(innerClause.device[0].UID);
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

                innerClause.state = ((int) clause.State).ToString();

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

                if (clause.DeviceUID != Guid.Empty)
                {
                    innerClause.device = new deviceType[0];
                    innerClause.device[0] = new deviceType()
                    {
                        UID = clause.DeviceUID.ToString()
                    };
                }

                innerClause.zone = clause.Zones.Cast<string>().ToArray();
                innerClauses.Add(innerClause);
            }

            innerZoneLogic.clause = innerClauses.ToArray();

            return innerZoneLogic;
        }
    }
}
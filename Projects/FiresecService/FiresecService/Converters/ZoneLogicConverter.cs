using System.Collections.Generic;
using System.Linq;
using Firesec.ZoneLogic;
using FiresecAPI.Models;

namespace FiresecService.Converters
{
    public static class ZoneLogicConverter
    {
        public static ZoneLogic Convert(Firesec.ZoneLogic.expr innerZoneLogic)
        {
            var zoneLogic = new ZoneLogic();

            if (innerZoneLogic != null && innerZoneLogic.clause != null && innerZoneLogic.clause.Length > 0)
            {
                foreach (var innerClause in innerZoneLogic.clause)
                {
                    var clause = new Clause();
                    if (innerClause.zone != null)
                    {
                        clause.Zones = innerClause.zone.ToList();
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

                    if (innerClause.device != null)
                    {
                        clause.DeviceUID = innerClause.device[0].UID;
                    }

                    zoneLogic.Clauses.Add(clause);
                }
            }

            return zoneLogic;
        }

        public static Firesec.ZoneLogic.expr ConvertBack(ZoneLogic zoneLogic)
        {
            var innerZoneLogic = new Firesec.ZoneLogic.expr();

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
                }

                switch (zoneLogic.JoinOperator)
                {
                    case ZoneLogicJoinOperator.And:
                        innerClause.joinOperator = "and";
                        break;

                    case ZoneLogicJoinOperator.Or:
                        innerClause.joinOperator = "or";
                        break;
                }

                if (clause.DeviceUID != null)
                {
                    innerClause.device = new deviceType[0];
                    innerClause.device[0] = new deviceType();
                    innerClause.device[0].UID = clause.DeviceUID;
                }

                innerClause.zone = clause.Zones.ToArray();
                innerClauses.Add(innerClause);
            }

            innerZoneLogic.clause = innerClauses.ToArray();

            return innerZoneLogic;
        }
    }
}
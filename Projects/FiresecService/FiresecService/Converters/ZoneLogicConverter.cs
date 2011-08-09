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
            ZoneLogic zoneLogic = new ZoneLogic();

            if ((innerZoneLogic != null) && (innerZoneLogic.clause != null))
            {
                foreach (var innerClause in innerZoneLogic.clause)
                {
                    Clause clause = new Clause();

                    clause.Zones = innerClause.zone.ToList();

                    switch (innerClause.state)
                    {
                        case "0":
                            clause.State = ZoneLogicState.AutomaticOn;
                            break;

                        case "1":
                            clause.State = ZoneLogicState.Alarm;
                            break;

                        case "2":
                            clause.State = ZoneLogicState.Fre;
                            break;

                        case "5":
                            clause.State = ZoneLogicState.Warning;
                            break;

                        case "6":
                            clause.State = ZoneLogicState.MPTOn;
                            break;
                    }

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

                    zoneLogic.Clauses.Add(clause);
                }
            }

            return zoneLogic;
        }

        public static Firesec.ZoneLogic.expr ConvertBack(ZoneLogic zoneLogic)
        {
            Firesec.ZoneLogic.expr innerZoneLogic = new Firesec.ZoneLogic.expr();

            List<clauseType> innerClauses = new List<clauseType>();
            foreach (var clause in zoneLogic.Clauses)
            {
                clauseType innerClause = new clauseType();

                switch (clause.State)
                {
                    case ZoneLogicState.AutomaticOn:
                        innerClause.state = "0";
                        break;

                    case ZoneLogicState.Alarm:
                        innerClause.state = "1";
                        break;

                    case ZoneLogicState.Fre:
                        innerClause.state = "2";
                        break;

                    case ZoneLogicState.Warning:
                        innerClause.state = "5";
                        break;

                    case ZoneLogicState.MPTOn:
                        innerClause.state = "6";
                        break;
                }

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

                innerClause.zone = clause.Zones.ToArray();
                innerClauses.Add(innerClause);
            }

            innerZoneLogic.clause = innerClauses.ToArray();

            return innerZoneLogic;
        }
    }
}
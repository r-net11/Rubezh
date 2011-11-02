using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
    [DataContract]
    public class ZoneLogic
    {
        public ZoneLogic()
        {
            Clauses = new List<Clause>();
        }

        [DataMember]
        public List<Clause> Clauses { get; set; }

        [DataMember]
        public ZoneLogicJoinOperator JoinOperator { get; set; }

        public override string ToString()
        {
            string result = "";

            for (int i = 0; i < Clauses.Count; ++i)
            {
                var clause = Clauses[i];

                if (i > 0)
                {
                    switch (JoinOperator)
                    {
                        case ZoneLogicJoinOperator.And:
                            result += " и ";
                            break;
                        case ZoneLogicJoinOperator.Or:
                            result += " или ";
                            break;
                        default:
                            break;
                    }
                }

                if (clause.DeviceUID != Guid.Empty)
                {
                    result += "Сработка устройства " + clause.Device.PresentationAddress + " - " + clause.Device.Driver.Name;
                    continue;
                }

                if (clause.State == ZoneLogicState.Failure)
                {
                    result += "состояние неисправность прибора";
                    continue;
                }

                result += "состояние " + EnumsConverter.ZoneLogicStateToString(clause.State);

                string stringOperation = null;
                switch (clause.Operation)
                {
                    case ZoneLogicOperation.All:
                        stringOperation = "во всех зонах из";
                        break;

                    case ZoneLogicOperation.Any:
                        stringOperation = "в любой зоне из";
                        break;

                    default:
                        break;
                }

                result += " " + stringOperation + " [";

                for (int j = 0; j < clause.Zones.Count; ++j)
                {
                    if (j > 0)
                        result += ", ";
                    result += clause.Zones[j];
                }

                result += "]";
            }

            return result;
        }
    }
}
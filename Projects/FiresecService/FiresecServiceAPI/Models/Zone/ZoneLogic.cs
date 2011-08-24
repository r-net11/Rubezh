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
            string stringZoneLogic = "";

            if (Clauses.Count > 0)
            {
                for (int i = 0; i < Clauses.Count; i++)
                {
                    var clause = Clauses[i];

                    if (i > 0)
                    {
                        switch (JoinOperator)
                        {
                            case ZoneLogicJoinOperator.And:
                                stringZoneLogic += " и ";
                                break;
                            case ZoneLogicJoinOperator.Or:
                                stringZoneLogic += " или ";
                                break;
                        }
                    }

                    string stringState = EnumsConverter.ZoneLogicStateToString(clause.State);

                    string stringOperation = "";
                    switch (clause.Operation)
                    {
                        case ZoneLogicOperation.All:
                            stringOperation = "во всех зонах из";
                            break;

                        case ZoneLogicOperation.Any:
                            stringOperation = "в любой зонах из";
                            break;
                    }

                    stringZoneLogic += "состояние " + stringState + " " + stringOperation + " [";

                    foreach (var zoneNo in clause.Zones)
                    {
                        stringZoneLogic += zoneNo + ", ";
                    }
                    if (stringZoneLogic.EndsWith(", "))
                        stringZoneLogic = stringZoneLogic.Remove(stringZoneLogic.Length - 2);

                    stringZoneLogic += "]";
                }
            }

            return stringZoneLogic;
        }
    }
}
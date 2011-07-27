using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FiresecClient.Models
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
            string stringZoneLogic = " ";

            if (Clauses.Count>0)
            {
                foreach (var clause in Clauses)
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

                    string stringState = "";
                    switch (clause.State)
                    {
                        case ZoneLogicState.AutomaticOn:
                            stringState = "Включение автоматики";
                            break;

                        case ZoneLogicState.Alarm:
                            stringState = "Тревога";
                            break;

                        case ZoneLogicState.Fre:
                            stringState = "Пожар";
                            break;

                        case ZoneLogicState.Warning:
                            stringState = "Внимание";
                            break;

                        case ZoneLogicState.MPTOn:
                            stringState = "Включение модуля пожаротушения";
                            break;
                    }

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

    [DataContract]
    public class Clause
    {
        public Clause()
        {
            Zones = new List<string>();
        }

        [DataMember]
        public ZoneLogicState State { get; set; }

        [DataMember]
        public ZoneLogicOperation Operation { get; set; }

        [DataMember]
        public List<string> Zones { get; set; }
    }

    public enum ZoneLogicOperation
    {
        All,
        Any
    }

    public enum ZoneLogicState
    {
        AutomaticOn,
        Alarm,
        Fre,
        Warning,
        MPTOn
    }

    public enum ZoneLogicJoinOperator
    {
        And,
        Or
    }
}

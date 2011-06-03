using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FiresecClient
{
    public static class ZoneLogicToText
    {
        public static string Convert(Firesec.ZoneLogic.expr zoneLogic)
        {
            string stringZoneLogic = " ";

            if (zoneLogic != null)
            {
                foreach (var clause in zoneLogic.clause)
                {
                    if (clause.joinOperator != null)
                    {
                        switch (clause.joinOperator)
                        {
                            case "and":
                                stringZoneLogic += " и ";
                                break;
                            case "or":
                                stringZoneLogic += " или ";
                                break;
                        }
                    }

                    string stringState = "";
                    switch (clause.state)
                    {
                        case "0":
                            stringState = "Включение автоматики";
                            break;

                        case "1":
                            stringState = "Тревога";
                            break;

                        case "2":
                            stringState = "Пожар";
                            break;

                        case "5":
                            stringState = "Внимание";
                            break;

                        case "6":
                            stringState = "Включение модуля пожаротушения";
                            break;
                    }

                    string stringOperation = "";
                    switch (clause.operation)
                    {
                        case "and":
                            stringOperation = "во всех зонах из";
                            break;

                        case "or":
                            stringOperation = "в любой зонах из";
                            break;
                    }

                    stringZoneLogic += "состояние " + stringState + " " + stringOperation + " [";

                    foreach (var zoneNo in clause.zone)
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

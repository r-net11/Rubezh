using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
    public enum StateType
    {
        Fire = 0,
        Attention = 1,
        Failure = 2,
        Service = 3,
        Off = 4,
        Unknown = 5,
        Info = 6,
        Norm = 7,
        No = 8
    }

    [DataContract]
    public class State
    {
        [DataMember]
        public int Id { get; set; }

        public State(int id)
        {
            Id = id;
        }

        public StateType StateType
        {
            get { return (StateType)Id; }
        }

        public override string ToString()
        {
            switch (Id)
            {
                case 0:
                    return "Тревога";

                case 1:
                    return "Внимание (предтревожное)";

                case 2:
                    return "Неисправность";

                case 3:
                    return "Требуется обслуживание";

                case 4:
                    return "Обход устройств";

                case 5:
                    return "Неопределено";

                case 6:
                    return "Норма(*)";

                case 7:
                    return "Норма";

                case 8:
                    return "Нет состояния";

                default:
                    return "";
            }
        }

        public static bool operator ==(State value1, State value2)
        {
            return value1.Id == value2.Id;
        }

        public static bool operator !=(State value1, State value2)
        {
            return value1.Id != value2.Id;
        }

        public override bool Equals(object obj)
        {
            return ((State) obj).Id == Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}

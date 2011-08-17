namespace FiresecAPI.Models
{
    public enum AlarmType
    {
        Fire,
        Attention,
        Failure,
        Off,
        Info,
        Service,
        Auto
    }

    //Почем не совпадает порядок следования с
    //    public enum StateType
    //    {
    //        Fire = 0,
    //        Attention = 1,
    //        Failure = 2,
    //        Service = 3,
    //        Off = 4,
    //        Unknown = 5,
    //        Info = 6,
    //        Norm = 7,
    //        No = 8
    //    }
    // Из-за этого приходится использовать конвертер FiresecAPI.Models.EnumsConverter.AlarmTypeToStateType
    // Вообще по идее можно StateType и тут использовать =)
}
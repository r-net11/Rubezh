namespace FiresecAPI.Models
{
    public enum ZoneLogicState
    {
        Fire = 0,
        Attention = 1,
        MPTAutomaticOn = 2,
        MPTOn = 3,
        //zsExitDelay_Unused = 4,
        Alarm = 5,
        GuardSet = 6,
        GuardUnSet = 7,
        PCN = 8,
        Lamp = 9,
        Failure = 10,
        AM1TOn = 11
    }
}

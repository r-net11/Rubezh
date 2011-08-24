namespace FiresecAPI.Models
{
    public enum ZoneLogicState
    {
        Fire = 2,
        Attention = 5,
        MPTAutomaticOn = 0,
        MPTOn = 6,
        //zsExitDelay_Unused = 4,
        Alarm = 1,
        GuardSet = 7,
        GuardUnSet = 8,
        PCN = 9,
        Lamp = 10,
        Failure = 3,
        AM1TOn = 11
    }
}

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
    public enum PermissionType
    {
        Adm_ChangeConfigDb = 6,
        Adm_ChangeConfigDevices = 7,
        Adm_ChangeDevicesSoft = 8,
        Adm_ClearJournal = 4,
        Adm_ViewConfig = 5,
        Adm_Security = 9,
        Oper_AutoOn = 20,
        Oper_Login = 1,
        Oper_Logout = 2,
        Oper_LogoutWithoutPassword = 13,
        Oper_AddToIgnoreList = 16,
        Oper_AdditionalMode = 11,
        Oper_ChangeLayout = 18,
        Oper_NoAlarmConfirm = 17,
        Oper_AlarmEdit = 3,
        Oper_SecurityZone = 19,
        Oper_IgnoreListEditing = 14,
        Oper_RemoveFromIgnoreList = 15,
        Oper_ShowPlans = 12
    }

    [DataContract]
    public class UserRole
    {
        public UserRole()
        {
            Permissions = new List<PermissionType>();
        }

        [DataMember]
        public UInt64 Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public List<PermissionType> Permissions { get; set; }
    }
}
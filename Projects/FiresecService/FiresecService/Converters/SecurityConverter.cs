using System.Collections.Generic;
using System.Linq;
using Firesec.CoreConfig;
using FiresecAPI.Models;

namespace FiresecService.Converters
{
    public class SecurityConverter
    {
        public static void Convert(Firesec.CoreConfig.config firesecConfig)
        {
            FiresecManager.SecurityConfiguration = new SecurityConfiguration();

            foreach (var firesecUser in FiresecManager.CoreConfig.user)
            {
                var user = new User();
                user.Id = firesecUser.param.value;
                user.Name = firesecUser.name;
                user.FullName = firesecUser.fullName;
                user.PasswordHash = firesecUser.password;
                user.IsBuiltIn = (firesecUser.builtin != "0");

                if (firesecUser.grp != null)
                {
                    foreach (var groupIdx in firesecUser.grp)
                    {
                        var firesecGroup = FiresecManager.CoreConfig.userGroup.FirstOrDefault(x => x.idx == groupIdx.idx);
                        user.Groups.Add(firesecGroup.param.value);
                    }
                }

                FiresecManager.SecurityConfiguration.Users.Add(user);
            }

            foreach (var firesecUserGroup in FiresecManager.CoreConfig.userGroup)
            {
                var userGroup = new UserGroup();
                userGroup.Id = firesecUserGroup.param.value;
                userGroup.Name = firesecUserGroup.name;
                FiresecManager.SecurityConfiguration.UserGroups.Add(userGroup);
            }

            var secObj = FiresecManager.CoreConfig.secObjType.FirstOrDefault(x => x.name == "Функции программы");
            foreach (var firesecPerimission in secObj.secAction)
            {
                var perimission = new Perimission();
                perimission.Id = firesecPerimission.num;
                perimission.Name = firesecPerimission.name;
                FiresecManager.SecurityConfiguration.Perimissions.Add(perimission);
            }

            foreach (var secRight in FiresecManager.CoreConfig.secGUI.FirstOrDefault(x => x.name == "Функции программы").secRight)
            {
                var permissionIdx = secRight.act;
                var permission = secObj.secAction.FirstOrDefault(x => x.idx == permissionIdx);
                var permissionId = permission.param.value;

                var idx = secRight.subj;
                var firesecUser = FiresecManager.CoreConfig.user.FirstOrDefault(x => x.idx == idx);
                var firesecGroup = FiresecManager.CoreConfig.userGroup.FirstOrDefault(x => x.idx == idx);

                if (firesecUser != null)
                {
                    string userId = firesecUser.param.value;
                    var user = FiresecManager.SecurityConfiguration.Users.FirstOrDefault(x => x.Id == userId);

                    if (secRight.deleteflag != "1")
                    {
                        user.Permissions.Add(permissionId);
                    }
                    else
                    {
                        user.RemovedPermissions.Add(permissionId);
                    }
                }

                if (firesecGroup != null)
                {
                    string groupId = firesecGroup.param.value;
                    var group = FiresecManager.SecurityConfiguration.UserGroups.FirstOrDefault(x => x.Id == groupId);
                    group.Permissions.Add(permissionId);
                }
            }
        }

        public static void ConvertBack(SecurityConfiguration securityConfiguration)
        {
            int idx = 0;

            var internalGroups = new List<userGroupType>();
            foreach (var group in securityConfiguration.UserGroups)
            {
                var internalGroup = new userGroupType();
                internalGroup.idx = idx.ToString();
                idx++;
                internalGroup.param = new Firesec.CoreConfig.paramType();
                internalGroup.param.name = "DB$IDUsers";
                internalGroup.param.type = "Int";
                internalGroup.param.value = group.Id;
                internalGroup.extSecurity = new extSecurityType();
                internalGroup.extSecurity.remoteAccess = "0";
                internalGroup.name = group.Name;
                internalGroups.Add(internalGroup);
            }
            FiresecManager.CoreConfig.userGroup = internalGroups.ToArray();

            var internalUsers = new List<userType>();
            foreach (var user in securityConfiguration.Users)
            {
                var internalUser = new userType();
                internalUser.idx = idx.ToString();
                idx++;
                internalUser.name = user.Name;
                internalUser.fullName = user.FullName;
                internalUser.password = user.PasswordHash;
                internalUser.builtin = user.IsBuiltIn ? "-1" : "0";
                internalUser.param = new Firesec.CoreConfig.paramType();
                internalUser.param.name = "DB$IDUsers";
                internalUser.param.type = "Int";
                internalUser.param.value = user.Id;
                internalUser.extSecurity = new extSecurityType();
                internalUser.extSecurity.remoteAccess = "0";

                var internalUserGroups = new List<grpType>();
                foreach (var group in user.Groups)
                {
                    var firesecGroup = FiresecManager.CoreConfig.userGroup.FirstOrDefault(x => x.param.value == group);
                    var internalUserGroup = new grpType();
                    internalUserGroup.idx = firesecGroup.idx;
                    internalUserGroups.Add(internalUserGroup);
                }
                internalUser.grp = internalUserGroups.ToArray();

                internalUsers.Add(internalUser);
            }
            FiresecManager.CoreConfig.user = internalUsers.ToArray();

            FiresecManager.CoreConfig.secGUI = new secGUIType[1];
            FiresecManager.CoreConfig.secGUI[0] = new secGUIType();
            FiresecManager.CoreConfig.secGUI[0].name = "Функции программы";
            FiresecManager.CoreConfig.secGUI[0].param = new paramType();
            FiresecManager.CoreConfig.secGUI[0].param.name = "DB$IDSecObj";
            FiresecManager.CoreConfig.secGUI[0].param.type = "Int";
            FiresecManager.CoreConfig.secGUI[0].param.value = "1";

            List<secRightType> permissionBindings = new List<secRightType>();

            foreach (var group in FiresecManager.SecurityConfiguration.UserGroups)
            {
                var internalGroup = FiresecManager.CoreConfig.userGroup.FirstOrDefault(x => x.name == group.Name);

                foreach (var permission in group.Permissions)
                {
                    var internalPermission = FiresecManager.CoreConfig.secObjType[0].secAction.FirstOrDefault(x => x.num == permission);
                    var permissionBinding = new secRightType();
                    permissionBinding.deleteflag = "0";
                    permissionBinding.act = internalPermission.idx;
                    permissionBinding.subj = internalGroup.idx;
                    permissionBindings.Add(permissionBinding);
                }
            }

            foreach (var user in FiresecManager.SecurityConfiguration.Users)
            {
                var internalUser = FiresecManager.CoreConfig.user.FirstOrDefault(x => x.name == user.Name);

                foreach (var permission in user.Permissions)
                {
                    var internalPermission = FiresecManager.CoreConfig.secObjType[0].secAction.FirstOrDefault(x => x.num == permission);
                    var permissionBinding = new secRightType();
                    permissionBinding.deleteflag = "0";
                    permissionBinding.act = internalPermission.idx;
                    permissionBinding.subj = internalUser.idx;
                    permissionBindings.Add(permissionBinding);
                }

                foreach (var permission in user.RemovedPermissions)
                {
                    var internalPermission = FiresecManager.CoreConfig.secObjType[0].secAction.FirstOrDefault(x => x.num == permission);
                    var permissionBinding = new secRightType();
                    permissionBinding.deleteflag = "1";
                    permissionBinding.act = internalPermission.idx; ;
                    permissionBinding.subj = internalUser.idx;
                    permissionBindings.Add(permissionBinding);
                }
            }

            FiresecManager.CoreConfig.secGUI[0].secRight = permissionBindings.ToArray();
        }
    }
}

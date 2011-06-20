using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecClient.Models;

namespace FiresecClient.Converters
{
    public class SecurityConverter
    {
        public static void Convert(Firesec.CoreConfig.config firesecConfig)
        {
            FiresecManager.Configuration.Users = new List<User>();
            FiresecManager.Configuration.UserGroups = new List<UserGroup>();
            FiresecManager.Configuration.Perimissions = new List<Perimission>();

            foreach (var firesecUser in FiresecManager.CoreConfig.user)
            {
                User user = new User();
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

                FiresecManager.Configuration.Users.Add(user);
            }

            foreach (var firesecUserGroup in FiresecManager.CoreConfig.userGroup)
            {
                UserGroup userGroup = new UserGroup();
                userGroup.Id = firesecUserGroup.param.value;
                userGroup.Name = firesecUserGroup.name;
                FiresecManager.Configuration.UserGroups.Add(userGroup);
            }

            var secObj = FiresecManager.CoreConfig.secObjType.FirstOrDefault(x => x.name == "Функции программы");
            foreach (var firesecPerimission in secObj.secAction)
            {
                Perimission perimission = new Perimission();
                perimission.Id = firesecPerimission.num;
                perimission.Name = firesecPerimission.name;
                FiresecManager.Configuration.Perimissions.Add(perimission);
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
                    var user = FiresecManager.Configuration.Users.FirstOrDefault(x => x.Id == userId);

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
                    var group = FiresecManager.Configuration.UserGroups.FirstOrDefault(x => x.Id == groupId);
                    group.Permissions.Add(permissionId);
                }
            }
        }

        public static void ConvertBack(CurrentConfiguration configuration)
        {
            return;

            List<Firesec.CoreConfig.userGroupType> internalGroups = new List<Firesec.CoreConfig.userGroupType>();
            foreach (var group in configuration.UserGroups)
            {
                Firesec.CoreConfig.userGroupType internalGroup = new Firesec.CoreConfig.userGroupType();
                internalGroup.param = new Firesec.CoreConfig.paramType();
                internalGroup.param.name = "DB$IDUsers";
                internalGroup.param.value = group.Id;
                internalGroup.name = group.Name;
                internalGroups.Add(internalGroup);
            }
            FiresecManager.CoreConfig.userGroup = internalGroups.ToArray();

            List<Firesec.CoreConfig.userType> internalUsers = new List<Firesec.CoreConfig.userType>();
            foreach (var user in configuration.Users)
            {
                Firesec.CoreConfig.userType internalUser = new Firesec.CoreConfig.userType();
                internalUser.name = user.Name;
                internalUser.fullName = user.FullName;
                internalUser.password = user.PasswordHash;
                internalUsers.Add(internalUser);
            }
            FiresecManager.CoreConfig.user = internalUsers.ToArray();
        }
    }
}

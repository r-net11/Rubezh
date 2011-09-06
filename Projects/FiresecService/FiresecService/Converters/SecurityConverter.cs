using System.Collections.Generic;
using System.Linq;
using Firesec.CoreConfiguration;
using FiresecAPI.Models;

namespace FiresecService.Converters
{
    public class SecurityConverter
    {
        public static void Convert(config firesecConfig)
        {
            FiresecManager.SecurityConfiguration = new SecurityConfiguration();

            foreach (var firesecUser in FiresecManager.CoreConfig.user)
            {
                var user = new User()
                {
                    Id = firesecUser.param.value,
                    Name = firesecUser.name,
                    FullName = firesecUser.fullName,
                    PasswordHash = firesecUser.password,
                    IsBuiltIn = (firesecUser.builtin != "0")
                };

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
                FiresecManager.SecurityConfiguration.UserGroups.Add(new UserGroup()
                {
                    Id = firesecUserGroup.param.value,
                    Name = firesecUserGroup.name
                });
            }

            var secObj = FiresecManager.CoreConfig.secObjType.FirstOrDefault(x => x.name == "Функции программы");
            foreach (var firesecPerimission in secObj.secAction)
            {
                FiresecManager.SecurityConfiguration.Perimissions.Add(new Perimission()
                {
                    Id = firesecPerimission.num,
                    Name = firesecPerimission.name
                });
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
                internalGroups.Add(new userGroupType()
                {
                    idx = idx.ToString(),
                    name = group.Name,
                    param = new paramType()
                    {
                        name = "DB$IDUsers",
                        type = "Int",
                        value = group.Id
                    },
                    extSecurity = new extSecurityType()
                    {
                        remoteAccess = "0"
                    }
                });

                ++idx;
            }
            FiresecManager.CoreConfig.userGroup = internalGroups.ToArray();

            var internalUsers = new List<userType>();
            foreach (var user in securityConfiguration.Users)
            {
                var internalUser = new userType()
                {
                    idx = idx.ToString(),
                    name = user.Name,
                    fullName = user.FullName,
                    password = user.PasswordHash,
                    builtin = user.IsBuiltIn ? "-1" : "0",
                    param = new paramType()
                    {
                        name = "DB$IDUsers",
                        type = "Int",
                        value = user.Id
                    },
                    extSecurity = new extSecurityType()
                    {
                        remoteAccess = "0"
                    }
                };
                ++idx;

                var internalUserGroups = new List<grpType>();
                foreach (var group in user.Groups)
                {
                    var firesecGroup = FiresecManager.CoreConfig.userGroup.FirstOrDefault(x => x.param.value == group);
                    var internalUserGroup = new grpType()
                    {
                        idx = firesecGroup.idx
                    };

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

                    permissionBindings.Add(new secRightType()
                    {
                        deleteflag = "0",
                        act = internalPermission.idx,
                        subj = internalGroup.idx
                    });
                }
            }

            foreach (var user in FiresecManager.SecurityConfiguration.Users)
            {
                var internalUser = FiresecManager.CoreConfig.user.FirstOrDefault(x => x.name == user.Name);

                foreach (var permission in user.Permissions)
                {
                    var internalPermission = FiresecManager.CoreConfig.secObjType[0].secAction.FirstOrDefault(x => x.num == permission);

                    permissionBindings.Add(new secRightType()
                    {
                        deleteflag = "0",
                        act = internalPermission.idx,
                        subj = internalUser.idx
                    });
                }

                foreach (var permission in user.RemovedPermissions)
                {
                    var internalPermission = FiresecManager.CoreConfig.secObjType[0].secAction.FirstOrDefault(x => x.num == permission);

                    permissionBindings.Add(new secRightType()
                    {
                        deleteflag = "1",
                        act = internalPermission.idx,
                        subj = internalUser.idx
                    });
                }
            }

            FiresecManager.CoreConfig.secGUI[0].secRight = permissionBindings.ToArray();
        }
    }
}
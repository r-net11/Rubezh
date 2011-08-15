using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;
using Firesec.CoreConfig;

namespace FiresecService.Converters
{
    public static class GuardUserConverter
    {
        public static void Convert(Firesec.CoreConfig.config firesecConfig)
        {
            FiresecManager.DeviceConfiguration.GuardUsers = new List<GuardUser>();

            if (FiresecManager.CoreConfig.part != null)
            {
                foreach (var innerDirection in FiresecManager.CoreConfig.part)
                {
                    if (innerDirection.type == "guarduser")
                    {
                        var guardUser = new GuardUser();
                        guardUser.Id = int.Parse(innerDirection.id);
                        guardUser.Gid = innerDirection.gid;
                        guardUser.Name = innerDirection.name;

                        if (innerDirection.PinZ != null)
                        {
                            foreach (var partZone in innerDirection.PinZ)
                            {
                                guardUser.Zones.Add(partZone.pidz);
                            }
                        }

                        if (innerDirection.param != null)
                        {
                            var KeyTMParameter = innerDirection.param.FirstOrDefault(x => x.name == "KeyTM");
                            if (KeyTMParameter != null)
                            {
                                guardUser.KeyTM = KeyTMParameter.value;
                            }

                            var DeviceUIDParameter = innerDirection.param.FirstOrDefault(x => x.name == "DeviceUID");
                            if (DeviceUIDParameter != null)
                            {
                                guardUser.DeviceUID = DeviceUIDParameter.value;
                            }

                            var PasswordParameter = innerDirection.param.FirstOrDefault(x => x.name == "Password");
                            if (PasswordParameter != null)
                            {
                                guardUser.Password = PasswordParameter.value;
                            }

                            var FunctionParameter = innerDirection.param.FirstOrDefault(x => x.name == "Function");
                            if (FunctionParameter != null)
                            {
                                guardUser.Function = FunctionParameter.value;
                            }

                            var FIOParameter = innerDirection.param.FirstOrDefault(x => x.name == "FIO");
                            if (FIOParameter != null)
                            {
                                guardUser.FIO = FIOParameter.value;
                            }

                            var CanSetZoneParameter = innerDirection.param.FirstOrDefault(x => x.name == "SetZone");
                            if (CanSetZoneParameter != null)
                            {
                                guardUser.CanSetZone = CanSetZoneParameter.value == "1";
                            }

                            var CanUnSetZoneParameter = innerDirection.param.FirstOrDefault(x => x.name == "UnSetZone");
                            if (CanUnSetZoneParameter != null)
                            {
                                guardUser.CanUnSetZone = CanUnSetZoneParameter.value == "1";
                            }
                        }

                        FiresecManager.DeviceConfiguration.GuardUsers.Add(guardUser);
                    }
                }
            }
        }

        public static void ConvertBack(DeviceConfiguration currentConfiguration)
        {
            var innerGuardUsers = new List<Firesec.CoreConfig.partType>();
            int no = 0;

            foreach (var guardUser in FiresecManager.DeviceConfiguration.GuardUsers)
            {
                var innerGuardUser = new Firesec.CoreConfig.partType();
                innerGuardUser.type = "guarduser";
                innerGuardUser.no = no.ToString();
                no++;
                innerGuardUser.id = guardUser.Id.ToString();
                innerGuardUser.gid = guardUser.Gid;
                innerGuardUser.name = guardUser.Name;

                var zones = new List<Firesec.CoreConfig.partTypePinZ>();
                foreach (var zone in guardUser.Zones)
                {
                    zones.Add(new Firesec.CoreConfig.partTypePinZ() { pidz = zone });
                }
                innerGuardUser.PinZ = zones.ToArray();

                var innerGuardUsersParameters = new List<paramType>();

                if (guardUser.CanSetZone)
                {
                    var parameter = new paramType() { name = "SetZone", type = "Bool", value = "1" };
                    innerGuardUsersParameters.Add(parameter);
                }

                if (guardUser.CanUnSetZone)
                {
                    var parameter = new paramType() { name = "UnSetZone", type = "Bool", value = "1" };
                    innerGuardUsersParameters.Add(parameter);
                }

                if (string.IsNullOrEmpty(guardUser.FIO) == false)
                {
                    var parameter = new paramType() { name = "FIO", type = "String", value = guardUser.FIO };
                    innerGuardUsersParameters.Add(parameter);
                }

                if (string.IsNullOrEmpty(guardUser.Function) == false)
                {
                    var parameter = new paramType() { name = "Function", type = "String", value = guardUser.Function };
                    innerGuardUsersParameters.Add(parameter);
                }

                if (string.IsNullOrEmpty(guardUser.Password) == false)
                {
                    var parameter = new paramType() { name = "Password", type = "String", value = guardUser.Password };
                    innerGuardUsersParameters.Add(parameter);
                }

                if (string.IsNullOrEmpty(guardUser.DeviceUID) == false)
                {
                    var parameter = new paramType() { name = "DeviceUID", type = "String", value = guardUser.DeviceUID };
                    innerGuardUsersParameters.Add(parameter);
                }

                if (string.IsNullOrEmpty(guardUser.KeyTM) == false)
                {
                    var parameter = new paramType() { name = "KeyTM", type = "String", value = guardUser.KeyTM };
                    innerGuardUsersParameters.Add(parameter);
                }

                innerGuardUser.param = innerGuardUsersParameters.ToArray();

                innerGuardUsers.Add(innerGuardUser);
            }

            var innerDirections = FiresecManager.CoreConfig.part.ToList();
            if (innerDirections != null)
            {
                innerGuardUsers.AddRange(innerDirections);
            }

            FiresecManager.CoreConfig.part = innerGuardUsers.ToArray();
        }
    }
}

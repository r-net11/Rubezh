using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.Models;

namespace DevicesModule.ViewModels
{
    class GuardConfigurationViewModel : DialogViewModel
    {
        
        ObservableCollection<UserViewModel> _users;
        public ObservableCollection<UserViewModel> Users
        {
            get { return _users; }
            set
            {
                _users = value;
                OnPropertyChanged("Users");
            }
        }

        ObservableCollection<Zone> _userZones;
        public ObservableCollection<Zone> UserZones
        {
            get { return _userZones; }
            set
            {
                _userZones = value;
                OnPropertyChanged("UserZones");
            }
        }
        public GuardConfigurationViewModel(Device selectedDevice, ObservableCollection<Zone> deviceZones)
        {
            //var result = FiresecManager.DeviceGetGuardUsersList(selectedDevice.UID);
            //string res = result.Result.ToString(); //3,104,8,20,12,6,64,64
            Title = "Синхронизация охранных пользователей";
            Users = new ObservableCollection<UserViewModel>();
            UserZones = new ObservableCollection<Zone>();//(FiresecManager.Zones);
            string res = GuardViewModel.Userlist;
            int CountUsers = byte.Parse(res.ToString().Substring(0,3));
            for (int i = 0; i < CountUsers; i++)
            {
                User user = new User();
                var userViewModel = new UserViewModel(new GuardUser());
                var guardUser = userViewModel.GuardUser;
                //guardUser.CanUnSetZone = Convert.ToBoolean(res[174*i + 107].ToString());
                //guardUser.CanSetZone = Convert.ToBoolean(res[174 * i + 108].ToString());
                guardUser.Name = res.Substring(174*i + 115, 20);
                guardUser.Password = res.Substring(174*i + 147, 6);
                guardUser.Password = guardUser.Password.Remove(guardUser.Password.IndexOf('F'));
                guardUser.Id = i;
                for (int j = 0; j < 64; j++)
                {
                    if (res.Substring(174*i + 153, 64)[j] == '1')
                    {
                        //?(find zone with localNo == j) UserZones.Add(FiresecManager.Zones[(FiresecManager.FiresecConfiguration.GetZoneLocalSecNo() == j));
                    }
                    //var localNo = FiresecManager.FiresecConfiguration.GetZoneLocalSecNo - 1;
                    //guardUser.Zones.Add(deviceZones.FirstOrDefault() res.Substring(174*i + 153, 64).IndexOf(res.Substring(174*i + 153, 64).FirstOrDefault(chr => chr == '1')));
                }
                    //user.Attr = res.Substring(174 * i + 107, 8).Select(chr => chr == '1').ToArray();
                    //user.Name = res.Substring(174 * i + 115, 20).ToCharArray();
                    //user.KeyTM = res.Substring(174 * i + 135, 12).ToCharArray();
                    //user.Pass = res.Substring(174 * i + 147, 6).ToCharArray();
                    //user.Zones = res.Substring(174 * i + 153, 64).Select(chr => chr == '1').ToArray();
                    //user.ReservedZones = res.Substring(174 * i + 217, 64).Select(chr => chr == '1').ToArray();
                    //UserViewModel userViewModel = new UserViewModel(new GuardUser());
                    //userViewModel.GuardUser.CanUnSetZone = user.Attr[0];
                    //userViewModel.GuardUser.CanSetZone = user.Attr[1];
                    //userViewModel.GuardUser.Name = user.Name.ToString();
                    //userViewModel.GuardUser.Password = user.Pass.ToString();
                    Users.Add(userViewModel);
            }
        }
    }
}

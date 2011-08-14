using System.Windows.Controls;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;

namespace FireMonitor
{
    public partial class UserView : UserControl
    {
        public UserView()
        {
            InitializeComponent();
            ChangeUserCommand = new RelayCommand(OnChangeUser);
            DataContext = this;
        }

        public RelayCommand ChangeUserCommand { get; private set; }
        void OnChangeUser()
        {
            ServiceFactory.Get<ISecurityService>().ChangeUser();
        }
    }
}

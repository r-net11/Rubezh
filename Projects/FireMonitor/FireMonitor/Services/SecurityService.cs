using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure;
using FiresecClient;
using FireMonitor.Services;

namespace FireMonitor
{
    public class SecurityService : ISecurityService
    {
        public bool Check()
        {
            PasswordView passwordView = new PasswordView();
            passwordView.ShowDialog();
            return passwordView.IsAutorised;
        }
    }
}

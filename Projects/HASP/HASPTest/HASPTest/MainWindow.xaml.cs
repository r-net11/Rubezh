using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Aladdin.HASP;

namespace HASPTest
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Login();
        }

        void Login()
        {
            HaspFeature feature = HaspFeature.Default;

            string vendorCode =
            "ybt42jIEO4LwdtdUUAkMt/7iD7LskXpOvt4G7u7blOTyuNEoyT5r68eAUCt0CDD3sL+ZqddeZhGu2SrQ" +
            "JNUbjjy9XJhD8b8SC+W7QMvGA1xtLWUxFmdBnyDqAoVgO8aqaUYSJ+YTDdm41jfu2NQdqFR0HGWrUzVD" +
            "5EJRFTxiPqiYs0wXSrK1k7DH7OewLE7OQPYDqjyTHoqkgi3anJ+QwEz1xBq8lm8usIB1QmSQYU34xgWT" +
            "GceP/VI1du4J/WznW07OlnYh7CygYJ6VsVQRrnvr8AF5ALC+rlCEjQW9+DIcMTK95alliQrDI5HCLUeZ" +
            "dptYoMiP6QzqhCfxpCT1ye/bv2SXb1w92ewBO1tQhJBiIEymfgBUdY0++SZMG87IH2iAtJ+6RaoHNJjV" +
            "RJcfVbHqV0r3HlsCVbXtp5R+JjTnZXU02Rlo8saPYUvS/zqOetEJFqHNLyEe4pneFYRWaLe0xrSF0cFA" +
            "QJTGpPCr/AfS3wofk/o1m/yGs5Z/wAvSy2ONq+0l2HaFD6qGBTURPF9l4XVjhfmjXalzoMC1FKVIDnBV" +
            "HGBvz7zGrBsxvBDJ1vBFqngWAWj9/XKBg1J3uH9xqygE0u+ygaC99Jzy5H/FWmo19w5tsx58Q7UpwBdq" +
            "S7IA9q/FDAiXTE1NUe80ExMLRoIKteA1JCR3fef/mkR03puZuEao3yZMKGm52QAF5PfEbykfPj8jJWD8" +
            "+iG/dzUOkmCzGdNqSZQpQjqYCngYu13tq8X1AzIj4sdntz4/5uMxlyfH+str4/18XLyXGrHq8YpNRXZ7" +
            "P9FBR8xERLyX5K91/7uTkVGmlhR8MCgsk8p79niDjgci9wKOaTwknYX2nQ6qi02fBEcb/YFr+CIdwm0X" +
            "Lm5VmgI7JKDIgCopLUNNk7aDHvjoMQtJA1BK9PctYX8WAtDvv679dBJbndw=";

            Hasp hasp = new Hasp(feature);
            HaspStatus status = hasp.Login(vendorCode);

            status = hasp.Logout();
        }
    }
}

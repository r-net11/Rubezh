using System.Windows;
using Aladdin.HASP;

namespace HASPTest
{
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
            HaspFeature feature = HaspFeature.FromFeature(10);

            string vendorCode =
            "hSxuvX+4Ik4ehlUbRjIi8NVx128oM0LHXfM8gyi5P+uUYY6yXKu798W1a7gFrjiAbLSg1taawgkszHhG" + 
            "zW0nlUzPN19fkiyseshhe7ag1ZChQihaMgBXyJfDOlC24bz8F01H7didmW/kZIbXC38CA2CQ4VPosoC4" +
            "3Lxq06xEBckzM9EQnTBF5tWimUhu4Cdvh4xkB57jqjmvthXkia7RYTwaVv7ZmP5kzadxz//lLLOhgBuD" +
            "j+/h6SgUy9z5vcqb8MJFXtJOf0F/u+C5NKN1wHb9l7EPuFagb+u1/tZrWdDGBDpp6VqRC2F6/u9OElFQ" + 
            "iDj3aPhDxtumE+LPt7Rt6ErpYXGzmWOnVzHGPljGfLVbacNLMDM2uHyKZTduTPvVOKjG+XbtnnJXqmIG" +
            "Y/tdzthAl89D4NyMwK5buGgEKibuzI1fK6xjnMNn/s/oZCsQxP4GPZGasWIqBHy44RtkXvIi/1E7m//w" +
            "zpXwgAimx8ZeFAGelD6Af4eaHVg0Bo0A+JBDlrRhQszUNGuJDiZY8NAHq34JITGQNALrMsdrc7KWibQg" +
            "jTfBAjsfsHRJPzJKvD0I0vrlnTs/HvQIUuX9mJh3D+/NlIGpVx7KmfssRBvoTUipicyYc0M4DS2mffLw" +
            "IqXgLzS+PFZbi0abwaikNk4Gfx4VlbsLPeH+Jm/3RCrmo29f5thYxbsAg9fvTLC2gfSxfAafzvrRlr5Q" +
            "nIF+jhEsDXgGoMWTeT/ogFLMlQlSp9WPzbZFWRFrg5FyUr805pgrCbD4n/mOMbOqlJ8E7LFr/MWjaUQQ" +
            "F4cgpYyaSSojCNmY1dC9aFUd9jbpEm1ucKTZvaL0IDrz1cZ92OxkV8AmPkW2KeIdq8MkPyTDK9DyYAz2" +
            "Nqwe4FFLz8dvlUjtoQrSW5xMYYT+MoEHFJfZ1yE8nd2QUmni7/OTTYyhaF4=";

            Hasp hasp = new Hasp(feature);
            HaspStatus status = hasp.Login(vendorCode);

            status = hasp.Logout();
        }
    }
}
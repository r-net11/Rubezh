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

namespace WpfApplication2
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            //InitializeComponent();
        }
        public List<League> ListLeagueList;
    }
   
    public class League
    {
        public string Name { get; set; }
        public List<Division> Divisions { get; set; }
    }

    public class Division
    {
        public string Name { get; set; }
        public List<Team> Teams { get; set; }
    }

    public class Team
    {
        public string Name { get; set; }
    }
}

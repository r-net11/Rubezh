using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Reflection;

namespace ServiceVisualizer
{
    public static class WindowManager
    {
        public static void Show()
        {
            try
            {
                Assemblies = new List<Assembly>();
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);

                Assembly assembly_1 = Assembly.LoadFile(@"C:/Rubezh/Assad/Projects/ServiceManager/TreeListView/bin/Debug/Ricciolo.Controls.TreeListView.dll");
                Assembly assembly_2 = Assembly.LoadFile(@"C:/Rubezh/Assad/3rdParty/WPF Toolkit/v3.5.50211.1/WPFToolkit.dll");

                Assemblies.Add(assembly_1);
                Assemblies.Add(assembly_2);
            }
            catch
            {
            }

            View view = new View();
            ViewModel viewModel = new ViewModel();
            view.DataContext = viewModel;
            view.ShowDialog();
        }

        static System.Reflection.Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            return Assemblies.First(x => x.FullName == args.Name);
        }

        static List<Assembly> Assemblies;

        public static string DriverId { get; set; }
    }
}

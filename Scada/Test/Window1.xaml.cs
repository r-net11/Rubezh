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
using Controls;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using ControlBase;
using System.Reflection;

namespace Test
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ControlRepository controlRepository = new ControlRepository();
            controlRepository.Controls = new List<UserControlBase>();

            foreach(UIElement uIElement in stackPanel.Children)
            {
                controlRepository.Controls.Add(uIElement as UserControlBase);
            }

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(@"C:/Save.bin", FileMode.Create, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, controlRepository);
            stream.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            ControlRepository userControls;

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(@"C:/Save.bin", FileMode.Open);
            userControls = (ControlRepository)formatter.Deserialize(stream);
            stream.Close();

            loadStackPanel.Children.Clear();
            foreach (UserControlBase userControl in userControls.Controls)
            {
                loadStackPanel.Children.Add(userControl);
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            //UserControlBase data = stackPanel.Children[0] as UserControlBase;

            //foreach (PropertyInfo propertyInfo in data.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
            //{
            //    if (propertyInfo.DeclaringType.IsSubclassOf(typeof(UserControlBase)) || propertyInfo.DeclaringType == typeof(UserControlBase))
            //    {
            //        if (propertyInfo.CanRead && propertyInfo.CanWrite)
            //            textBox.Text += propertyInfo.Name + "\n";
            //    }
            //}


            System.Reflection.Assembly assem = Assembly.Load("Controls.dll");


            string strName = "Controls.dll";
            			foreach (System.Reflection.Assembly a in System.AppDomain.CurrentDomain.GetAssemblies()) 
		{
			try 
			{
				string[] strAssemblyName  = a.FullName.Split(new char[]{','});
				string strNameTemp = strAssemblyName[0] + "." + strName;
				if (System.Activator.CreateInstance(a.FullName, strNameTemp) != null) 
				{
                    ;                                                       
		// Creating Instance
//Sample a = (Sample)System.Activator.CreateInstance(a.FullName, strNameTemp)

				}
			}
			catch (System.Exception)
			{
				continue;
			}
		}
        }
    }

    [Serializable]
    public class ControlRepository
    {
        public List<UserControlBase> Controls { get; set; }
    }
}

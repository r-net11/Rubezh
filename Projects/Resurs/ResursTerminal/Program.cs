using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ResursTerminal.Forms;
using ResursNetwork.Networks;
using ResursNetwork.Incotex.Models;
using ResursNetwork.Incotex.NetworkControllers.ApplicationLayer;
using ResursNetwork.Incotex.NetworkControllers.DataLinkLayer;

namespace ResursTerminal
{
	class Program
	{
		//static class Program
		//{
		//	[STAThread]
		//	static void Main()
		//	{
		//		Application.EnableVisualStyles();
		//		Application.SetCompatibleTextRenderingDefault(false);
		//		Application.Run(new FormMain());
		//	}
		//}

		static Mercury203 _Device; 

		static void Main(string[] args)
		{
			Init();
			//ReadGroupAddress();
			//WriteDateTime();
			ReadDateTime();

			Console.WriteLine("Для завершения работы нажми ENTER...");
			Console.ReadLine();
		}

		static void Init()
		{
			Console.WriteLine("Инициализация системы...");

			var connection = new ComPort
			{
				PortName = "COM2",
				BaudRate = 9600,
			};

			var network = new IncotexNetworkController
			{
				Connection = connection
			};

			_Device = new Mercury203
			{
				Address = 23801823
			};

			network.Devices.Add(_Device);

			NetworksManager.Instance.Networks.Add(network);

			_Device.Start();
			network.Start();
		}

		static void ReadDateTime()
		{
			Console.WriteLine("Прочитать время и дату...");
			
			var result = _Device.ReadParameter(
				ResursAPI.ParameterNames.ParameterNamesMercury203.DateTime);
			var dt = ((IncotexDateTime)result.Value).ToDateTime();
			Console.WriteLine("Результат: {0}", dt);
		}

		static void WriteDateTime()
		{
			Console.WriteLine("Запусать время и дату...");
			var dt = DateTime.Now.AddHours(1);
			var result = _Device.WriteParameter(
				ResursAPI.ParameterNames.ParameterNamesMercury203.DateTime, dt);
			Console.WriteLine("Результат: {0}", dt);
		}

		static void ReadGroupAddress() 
		{
			Console.WriteLine("Читаем групповой адрес устройства...");

			var result = _Device.ReadParameter(
				ResursAPI.ParameterNames.ParameterNamesMercury203.GADDR);
			Console.WriteLine("Результат: {0}", result.Value);
		}
	}
}

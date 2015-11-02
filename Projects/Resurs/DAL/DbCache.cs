using ResursAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.Runtime.Serialization;
using Common;
using Infrastructure.Common.Windows;

namespace ResursDAL
{
	public static partial class DbCache
	{
		public static Consumer RootConsumer { get; set; }

		static DbCache()
		{
			RootDevice = GetRootDevice();
			if (RootDevice == null)
				CreateSystem();
			if(GetAllUsers()!= null)
			Users = GetAllUsers();
			RootConsumer = GetRootConsumer();
			Tariffs = ReadAllTariffs();
		}

		public static List<Device> GetAllChildren(Device device, bool isWithSelf = true)
		{
			var result = new List<Device>();
			if (isWithSelf)
				result.Add(device);
			result.AddRange(device.Children.SelectMany(x => GetAllChildren(x)));
			return result;
		}
		public static bool CheckConnection()
		{
			try
			{
				using (var databaseContext = DatabaseContext.Initialize())
				{
					databaseContext.Measures.FirstOrDefault();
					return true;
				}
			}
			catch (Exception e)
			{
				return false;
			}
		}
	}
}

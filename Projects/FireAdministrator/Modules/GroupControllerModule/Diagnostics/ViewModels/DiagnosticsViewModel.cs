using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Ionic.Zip;
using RubezhAPI.GK;
using RubezhClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using RubezhAPI;
using RubezhClient.SKDHelpers;
using RubezhAPI.SKD;
using System;

namespace GKModule.ViewModels
{
	public class DiagnosticsViewModel : ViewPartViewModel
	{
		public DiagnosticsViewModel()
		{
			TestCommand = new RelayCommand(OnTest);
			AddAllDoorsToCardCommand = new RelayCommand(OnAddAllDoorsToCard);
			TestIonicZipCommand = new RelayCommand(OnTestIonicZip);
		}

		public RelayCommand TestIonicZipCommand { get; private set; }
		void OnTestIonicZip()
		{
			var buffer = new byte[917504];
			foreach (var i in buffer)
			{
				buffer[i] = (byte)'a';
			}

			using (var zippedStream = new MemoryStream())
			{
				using (var zip = new ZipFile(Encoding.UTF8))
				{
					// uncommenting the following line can be used as a work-around
					// zip.ParallelDeflateThreshold = -1;
					zip.AddEntry("entry.txt", buffer);
					zip.Save(zippedStream);
				}
				zippedStream.Position = 0;

				using (var zip = ZipFile.Read(zippedStream))
				{
					using (var ms = new MemoryStream())
					{
						// This line throws a BadReadException
						zip.Entries.First().Extract(ms);
					}
				}
			}
		}
		public RelayCommand TestCommand { get; private set; }
		void OnTest()
		{
			GKManager.Doors.RemoveAll(x => true);
			var doorNo = 1;
			var shleifDevices = GKManager.Devices.Where(x => x.DriverType == GKDriverType.RSR2_KAU_Shleif && (x.IntAddress == -1));
				//|| x.IntAddress == 8));
			foreach (var shleifDevice in shleifDevices)
			{
				AddDevicesOnSchleif(shleifDevice, ref doorNo);	
			}
			
			GKManager.UpdateConfiguration();
			ServiceFactory.Events.GetEvent<ConfigurationChangedEvent>().Publish(null);
			ServiceFactory.SaveService.GKChanged = true;
		}

		void AddDevicesOnSchleif(GKDevice shleifDevice, ref int doorNo)
		{
			shleifDevice.Children = new List<GKDevice>();
			for (int i = 0; i < 54; i++)
			{
				var cardReaderDriver = GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.RSR2_CardReader);
				var rmDriver = GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.RSR2_RM_1);

				var enterDevice = new GKDevice();
				enterDevice.DriverUID = cardReaderDriver.UID;
				enterDevice.IntAddress = (byte)(3 * i + 1);
				enterDevice.Description = "ТД " + doorNo + " вход";
				shleifDevice.Children.Add(enterDevice);

				var exitDevice = new GKDevice();
				exitDevice.DriverUID = cardReaderDriver.UID;
				exitDevice.IntAddress = (byte)(3 * i + 2);
				exitDevice.Description = "ТД " + doorNo + " выход";
				shleifDevice.Children.Add(exitDevice);

				var lockDevice = new GKDevice();
				lockDevice.DriverUID = rmDriver.UID;
				lockDevice.IntAddress = (byte)(3 * i + 3);
				lockDevice.Description = "ТД " + doorNo + " замок";
				shleifDevice.Children.Add(lockDevice);

				var door = new GKDoor();
				door.No = doorNo;
				door.Name = "ТД " + doorNo;
				door.DoorType = GKDoorType.TwoWay;
				door.EnterDeviceUID = enterDevice.UID;
				door.ExitDeviceUID = exitDevice.UID;
				door.LockDeviceUID = lockDevice.UID;
				GKManager.Doors.Add(door);

				doorNo++;
			}
		}

		public RelayCommand AddAllDoorsToCardCommand { get; private set; }
		void OnAddAllDoorsToCard()
		{
			var cards = CardHelper.Get(new CardFilter());
			var card = cards.FirstOrDefault();
			var random = new Random();
			card.CardDoors = new List<CardDoor>(GKManager.Doors.Select(x => new CardDoor
			{ 
				CardUID = card.UID, 
				DoorUID = x.UID, 
				EnterScheduleNo = random.Next(100), 
				ExitScheduleNo = random.Next(100) 
			}));
			CardHelper.Edit(card, card.EmployeeName);
		}
	}
}
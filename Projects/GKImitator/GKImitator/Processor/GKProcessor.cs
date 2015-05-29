using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using FiresecAPI.GK;
using GKImitator.ViewModels;
using GKProcessor;
using FiresecClient;

namespace GKImitator.Processor
{
	public class GKProcessor
	{
		GkDatabase GkDatabase;
		Socket serverSocket;
		byte[] byteData = new byte[64];

		public void Start()
		{
			GkDatabase = DescriptorsManager.GkDatabases.FirstOrDefault();

			serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, 1025);
			serverSocket.Bind(ipEndPoint);

			IPEndPoint ipeSender = new IPEndPoint(IPAddress.Any, 0);
			EndPoint epSender = (EndPoint)ipeSender;

			serverSocket.BeginReceiveFrom(byteData, 0, byteData.Length, SocketFlags.None, ref epSender, new AsyncCallback(OnReceive), epSender);
		}

		void OnReceive(IAsyncResult ar)
		{
			IPEndPoint ipeSender = new IPEndPoint(IPAddress.Any, 0);
			EndPoint epSender = (EndPoint)ipeSender;
			serverSocket.EndReceiveFrom(ar, ref epSender);

			var bytes = CreateAnswer();
			if (bytes != null)
			{
				var sendBytes = new List<byte>();
				sendBytes.Add(byteData[0]);
				sendBytes.Add(byteData[1]);
				sendBytes.AddRange(ToBytes((short)bytes.Count));
				sendBytes.Add(byteData[4]);
				sendBytes.AddRange(bytes);
				byte[] message = sendBytes.ToArray();

				serverSocket.BeginSendTo(message, 0, message.Length, SocketFlags.None, epSender, new AsyncCallback(OnSend), epSender);
			}
			serverSocket.BeginReceiveFrom(byteData, 0, byteData.Length, SocketFlags.None, ref epSender, new AsyncCallback(OnReceive), epSender);
		}

		void OnSend(IAsyncResult ar)
		{
			serverSocket.EndSend(ar);
		}

		List<byte> CreateAnswer()
		{
			switch (byteData[4])
			{
				case 1:
					return new List<byte>() { 44 };
				case 2:
					int softwareNo = 33;
					int hardwareNo = 55;
					var result = new List<byte>();
					result.AddRange(BitConverter.GetBytes(softwareNo));
					result.AddRange(BitConverter.GetBytes(hardwareNo));
					return result;
				case 5: // DateTime Synchrinysation
					return new List<byte>();

				case 6:
					var count = JournalHelper.ImitatorJournalItemCollection.ImitatorJournalItems.Count;
					if (count > 0)
						return GetJournalBytes(count);
					return null;

				case 7:
					var no = BytesHelper.SubstructInt(byteData.ToList(), 5);
					return GetJournalBytes(no);

				case 12: // Get State
					no = BytesHelper.SubstructShort(byteData.ToList(), 5);
					return GetObjectState(no);

				case 23: // Infoormational Block
					var blockNo = byteData[5];

					byte minorVersion = 1;
					byte majorVersion = 1;
					var hash1 = HashHelper.CreateHash1(GKManager.DeviceConfiguration, GKManager.DeviceConfiguration.RootDevice.Children[0]);
					var infoBlock = new List<byte>(256) { minorVersion, majorVersion };
					infoBlock.AddRange(hash1);
					var descriptorsCount = GkDatabase.Descriptors.Count();
					infoBlock.AddRange(BitConverter.GetBytes(descriptorsCount));
					var fileSize = 1000;
					infoBlock.AddRange(BitConverter.GetBytes(fileSize));
					infoBlock.AddRange(BitConverter.GetBytes(DateTime.Now.Ticks));
					while (infoBlock.Count < 256)
						infoBlock.Add(0);

					return infoBlock;
			}
			return new List<byte>();
		}

		public List<byte> GetObjectState(int no)
		{
			var binaryObjectBase = GkDatabase.Descriptors.FirstOrDefault(x => x.GetDescriptorNo() == no);
			if (binaryObjectBase == null)
				return new List<byte>();

			var result = new List<byte>();

			var typeNo = 0;
			if (binaryObjectBase.GKBase is GKDevice)
				typeNo = (binaryObjectBase.GKBase as GKDevice).Driver.DriverTypeNo;
			if (binaryObjectBase.GKBase is GKZone)
				typeNo = 0x100;
			if (binaryObjectBase.GKBase is GKDirection)
				typeNo = 0x106;
			if (binaryObjectBase.GKBase is GKPumpStation)
				typeNo = 0x106;
			if (binaryObjectBase.GKBase is GKMPT)
				typeNo = 0x106;
			if (binaryObjectBase.GKBase is GKDelay)
				typeNo = 0x101;
			if (binaryObjectBase.GKBase is GKPim)
				typeNo = 0x107;
			if (binaryObjectBase.GKBase is GKGuardZone)
				typeNo = 0x108;
			if (binaryObjectBase.GKBase is GKCode)
				typeNo = 0x109;
			if (binaryObjectBase.GKBase is GKDoor)
				typeNo = 0x104;
			result.AddRange(ToBytes((short)typeNo));

			var controllerAddress = binaryObjectBase.ControllerAdress;
			result.AddRange(ToBytes((short)controllerAddress));

			var addressOnController = binaryObjectBase.AdressOnController;
			result.AddRange(ToBytes((short)addressOnController));

			var physicalAddress = binaryObjectBase.PhysicalAdress;
			result.AddRange(ToBytes((short)physicalAddress));

			result.AddRange(binaryObjectBase.Description);

			var serialNo = 0;
			result.AddRange(IntToBytes((int)serialNo));

			var state = 0;
			//var stateBits = new List<XStateBit>();
			//stateBits.Add(XStateBit.Norm);
			var binaryObjectViewModel = MainViewModel.Current.BinaryObjects.FirstOrDefault(x => x.BinaryObject.GetDescriptorNo() == binaryObjectBase.GetDescriptorNo());
			foreach (var stateBitViewModel in binaryObjectViewModel.StateBits)
			{
				if (stateBitViewModel.IsActive)
				{
					state += (1 << (int)stateBitViewModel.StateBit);
				}
			}
			result.AddRange(IntToBytes((int)state));

			for (int i = 0; i < 20; i++)
			{
				result.Add(0);
			}

			return result;
		}

		public static List<byte> GetJournalBytes(int no)
		{
			var imitatorJournalItems = JournalHelper.ImitatorJournalItemCollection.ImitatorJournalItems[no - 1];
			return imitatorJournalItems.ToBytes();
		}

		public static List<byte> ToBytes(short shortValue)
		{
			return BitConverter.GetBytes(shortValue).ToList();
		}

		public static List<byte> IntToBytes(int intValue)
		{
			return BitConverter.GetBytes(intValue).ToList();
		}
	}
}
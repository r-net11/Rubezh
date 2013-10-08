using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using Common.GK;
using XFiresecAPI;
using System.Diagnostics;

namespace GKImitator.Processor
{
	public class GKProcessor
	{
		GkDatabase GkDatabase;
		Socket serverSocket;
		byte[] byteData = new byte[64];

		public void Start()
		{
			DatabaseManager.Convert();
			GkDatabase = DatabaseManager.GkDatabases.FirstOrDefault();

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
			}
			return new List<byte>();
		}

		public List<byte> GetObjectState(int no)
		{
			var binaryObjectBase = GkDatabase.BinaryObjects.FirstOrDefault(x => x.GkDescriptorNo == no);
			if (binaryObjectBase == null)
				return new List<byte>();

			var result = new List<byte>();
			//var typeNo = binaryObjectBase.ObjectType;
			var typeNo = 0;

			if (binaryObjectBase.Device != null)
				typeNo = binaryObjectBase.Device.Driver.DriverTypeNo;
			if (binaryObjectBase.Zone != null)
				typeNo = 0x100;
			if (binaryObjectBase.Direction != null)
				typeNo = 0x106;

			result.AddRange(ToBytes((short)typeNo));

			Trace.WriteLine(no + " - " + typeNo);

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
			var stateBits = new List<XStateBit>();
			stateBits.Add(XStateBit.Norm);
			foreach (var stateBit in stateBits)
			{
				state += (1 << (int)stateBit);
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

		//static List<byte> StringToBytes(string str, int length = 32)
		//{
		//    if (str == null)
		//        str = "";
		//    if (str.Length > length)
		//        str = str.Substring(0, length);
		//    var bytes = Encoding.Default.GetBytes(str).ToList();
		//    for (int i = 0; i < length - str.Length; i++)
		//    {
		//        bytes.Add(32);
		//    }
		//    return bytes;
		//}
	}
}
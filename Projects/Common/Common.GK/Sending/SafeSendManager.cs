using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XFiresecAPI;

namespace Common.GK
{
	public static class SafeSendManager
	{
		public static void Send(SendPriority sendPriority, XDevice device, ushort length, byte command, ushort inputLenght, List<byte> data = null, bool hasAnswer = true, bool sleepInsteadOfRecieve = false)
		{
			SendManager.Send(device, length, command, inputLenght, data, hasAnswer, sleepInsteadOfRecieve);
		}
	}

	public enum SendPriority
	{
		Low,
		Normal,
		Heigh
	}
}
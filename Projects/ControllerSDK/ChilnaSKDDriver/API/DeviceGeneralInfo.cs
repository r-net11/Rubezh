namespace ChinaSKDDriverAPI
{
	public class DeviceGeneralInfo
	{
		public string OpenDoorAudioPath { get; set; }
		public string CloseDoorAudioPath { get; set; }
		public string InUsedAuidoPath { get; set; }
		public string PauseUsedAudioPath { get; set; }
		public string NotClosedAudioPath { get; set; }
		public string WaitingAudioPath { get; set; }
		public int UnlockReloadTime { get; set; }
		public int UnlockHoldTime { get; set; }
		public bool IsProjectPassword { get; set; }
		public string ProjectPassword { get; set; }
	}
}
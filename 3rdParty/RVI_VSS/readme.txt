RVI VSS видео ячейка по состоянию на 14.11.2015 г.

В состав инсталлятора необходимо включить следующие файлы:

1. Для FiresecService

API.dll
Entities.dll
GalaSoft.MvvmLight.WPF4.dll
RVI_VSS.Utils.dll
SdkWrapper.dll

2. Для FireAdministrator / FireMonitor

DLL\*.*
API.dll
Entities.dll
GalaSoft.MvvmLight.WPF4.dll
MediaSourcePlayer.dll
NativePlayerWrap.dll
PlayerPoolService.dll
PlayerService.dll
PlayerServiceApi.dll
RVI_VSS.Utils.dll
RviVssDec.exe
SdkWrapper.dll

Видео ячейка – класс «MediaPlayer» из пространства имен «MediaSourcePlayer»  библиотеки MediaSourcePlayer.dll

Пример использования:

// Указываем поток для воспроизведения (Сокет, открытый в Операторе)
MediaSourcePlayer.Open(MediaSourceFactory.CreateFromTcpSocket(ipEndPoint, vendorId));

где ipEndPoint и vendorId определяются следующим образом:

public bool PrepareToTranslation(out IPEndPoint ipEndPoint, out int vendorId)
{
	return RviClientHelper.PrepareToTranslation(FiresecManager.SystemConfiguration, Camera, out ipEndPoint, out vendorId);
}

public static class RviClientHelper
{
	...

	public static bool PrepareToTranslation(SystemConfiguration systemConfiguration, Camera camera, out IPEndPoint ipEndPoint, out int vendorId)
	{
		ipEndPoint = null;
		vendorId = -1;

		var devices = GetDevices(systemConfiguration);
		var device = devices.FirstOrDefault(d => d.Guid == camera.RviDeviceUID);
			
		if (device == null)
			return false;

		var channel = device.Channels.FirstOrDefault(ch => ch.Number == camera.RviChannelNo);
			
		if (channel == null)
			return false;

		vendorId = channel.Vendor;

		using (IntegrationClient client = CreateIntegrationClient(systemConfiguration))
		{
			var sessionUID = Guid.NewGuid();

			var sessionInitialiazationIn = new SessionInitialiazationIn();
			sessionInitialiazationIn.Header = new HeaderRequest()
			{
				Request = Guid.NewGuid(),
				Session = sessionUID
			};
			sessionInitialiazationIn.Login = systemConfiguration.RviSettings.Login;
			sessionInitialiazationIn.Password = systemConfiguration.RviSettings.Password;
			var sessionInitialiazationOut = client.SessionInitialiazation(sessionInitialiazationIn);
			//var errorMessage = sessionInitialiazationOut.Header.HeaderResponseMessage.Information;

			var response = client.VideoStreamingStart(new ChannelStreamingStartIn()
			{
				Header = new HeaderRequest() { Request = new Guid(), Session = sessionUID },
				DeviceGuid = device.Guid,
				ChannelNumber = channel.Number,
				StreamNumber = camera.StreamNo
			});

			var sessionCloseIn = new SessionCloseIn();
			sessionCloseIn.Header = new HeaderRequest()
			{
				Request = Guid.NewGuid(),
				Session = sessionUID
			};
			var sessionCloseOut = client.SessionClose(sessionCloseIn);

			if (response.EndPointPort == 0)
			{
				return false;
			}
			ipEndPoint = new IPEndPoint(IPAddress.Parse(response.EndPointAdress), response.EndPointPort);
		}

		return true;

	}
}


// Начинаем воспроизведение
MediaSourcePlayer.Play();

// Останавливаем воспроизведение
MediaSourcePlayer.Stop();

// Останавливаем воспроизведение (если еще не остановлено) и очищаем занятые ресурсы
MediaSourcePlayer.Close(); 

Замечание: Если MediaSourcePlayer.Close() не будет выполнен, то останется висящим процесс RviVssDec.exe. Причем для каждого экземпляра MediaSourcePlayer создается свой процесс RviVssDec.exe

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
NativePlayerIpcProtocol.dll
NativePlayerWrap.dll
PlayerPoolService.dll
PlayerService.dll
PlayerServiceApi.dll
RVI_VSS.Utils.dll
RviVssDec.exe
SdkWrapper.dll

Видео ячейка – класс «MediaPlayer» из пространства имен «MediaSourcePlayer»  библиотеки MediaSourcePlayer.dll

Пример использования:

// Указываем поток для воспроизведения (rtsp-ссылка в данном случае)
MediaSourcePlayer.Open(MediaSourceFactory.CreateFromRtspStream(viewModel.Camera.RviRTSP));

// Начинаем воспроизведение
MediaSourcePlayer.Play();

// Останавливаем воспроизведение
MediaSourcePlayer.Stop();

// Останавливаем воспроизведение (если еще не остановлено) и очищаем занятые ресурсы
MediaSourcePlayer.Close(); 

Замечание: Если MediaSourcePlayer.Close() не будет выполнен, то останется висящим процесс RviVssDec.exe. Причем для каждого экземпляра MediaSourcePlayer создается свой процесс RviVssDec.exe

RVI VSS ����� ������ �� ��������� �� 14.11.2015 �.

� ������ ������������ ���������� �������� ��������� �����:

1. ��� FiresecService

API.dll
Entities.dll
GalaSoft.MvvmLight.WPF4.dll
RVI_VSS.Utils.dll
SdkWrapper.dll

2. ��� FireAdministrator / FireMonitor

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

����� ������ � ����� �MediaPlayer� �� ������������ ���� �MediaSourcePlayer�  ���������� MediaSourcePlayer.dll

������ �������������:

// ��������� ����� ��� ��������������� (rtsp-������ � ������ ������)
MediaSourcePlayer.Open(MediaSourceFactory.CreateFromRtspStream(viewModel.Camera.RviRTSP));

// �������� ���������������
MediaSourcePlayer.Play();

// ������������� ���������������
MediaSourcePlayer.Stop();

// ������������� ��������������� (���� ��� �� �����������) � ������� ������� �������
MediaSourcePlayer.Close(); 

���������: ���� MediaSourcePlayer.Close() �� ����� ��������, �� ��������� ������� ������� RviVssDec.exe. ������ ��� ������� ���������� MediaSourcePlayer ��������� ���� ������� RviVssDec.exe

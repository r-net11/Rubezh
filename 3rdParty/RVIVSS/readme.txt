RVI VSS ����� ������ �� ��������� �� 22.07.2015 �.

� ������ ������������ ���������� �������� ��������� �����:

1. ��� FiresecService

API.dll
Entities.dll
GalaSoft.MvvmLight.WPF4.dll
SdkWrapper.dll

2. ��� FireAdministrator / FireMonitor

DLL\*.*
API.dll
Entities.dll
GalaSoft.MvvmLight.WPF4.dll
MediaSourcePlayer.dll
NativePlayerIpcProtocol.dll
NativePlayerWrap.dll
ProcessCommunication.dll
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

unit FS_Rubezh03_Driver;
{
  Драйвер второй версии панели. Основные изменения:
  - Новый протокол (отсутствует БД)
  - Новый транспорт, базирующийся на USB
  - Полностью новый формат базы данных
  - Поддержка охранной части
}

{$DEFINE CACHEPDU} // ускорение формирования БД ПДУ

{$I ..\Options.inc}

interface
uses windows, ActiveX, sysutils, classes, contnrs, math,
  FS_Intf,
  FS_DevClasses, FS_Server_Intf, FS_Rubezh_Common, FS_HexPackage, SyncObjs,
  FS_Rubezh03_Interface, FS_Rubezh03_DBCommon, FS_Rubezh03_DBClassic,
  FS_Rubezh_Logic, FS_Extensions_Common, FS_XML_Streaming, FS_Rubezh03_Config, FS_Strings,
  FS_USBModBus_Driver,
  FS_MD5, FS_SupportClasses
  {$IFDEF IMITATOR}, FS_Imitator {$ENDIF};

procedure RegisterDevices(const DeviceRegistry: IDeviceRegistry);

type
  ISetDatatransport = interface;

  TOnInitTransport = procedure(const Sender: ISetDatatransport) of object;

  ISetDatatransport = interface
    ['{2AC46DB7-4670-4B49-B1A8-E25530A21656}']
    procedure SetDataTransport(const Datatransport: IDataTransport);
    function GetFixedDataTransport: IDataTransport;

    procedure SetOnInitTransportInstance(Value: TOnInitTransport);
    procedure SetOnFInitTransportInstance(Value: TOnInitTransport);
  end;

  TCustomRubezh3Device = class(TCustomDeviceInstance , ISetDatatransport)
  private
    FDataTransport: IDataTransport;
    FOnInitTransport, FOnFinitTransport: TOnInitTransport;
  protected
    procedure SetDatatransport(const Datatransport: IDataTransport);
    procedure SetOnInitTransportInstance(Value: TOnInitTransport);
    procedure SetOnFInitTransportInstance(Value: TOnInitTransport);

    function GetFixedDataTransport: IDataTransport;
  public
    constructor Create(const Parent: IDeviceInstance; const Config: IDevice;
      PanelDeviceMod: PPanelDeviceMod; ServerReq: IServerRequesting); virtual;
  end;

  TRubezh3DeviceClass = class of TCustomRubezh3Device;

  TRubezh3Driver = class(TServerDeviceDriver, IRubezh3DeviceDriver, ICrossLinkedDevice, IAutoDetectInstances, ICustomIOCTLFunctions)
  protected
    FModel: IRubezh3DatabaseModelExt;
    FPanelDeviceMod: PPanelDeviceMod;
    FDeviceClass: TRubezh3DeviceClass;
    FServerReq: IServerRequesting;
  protected
    function USBInterfaceSelected(const Config: IDevice): boolean;
    function CreateUSBParentInstance(const Sender: IUnknown): IDeviceInstance;
    procedure InitUSBTransport(const Sender: ISetDatatransport);
    procedure FInitUSBTransport(const Sender: ISetDatatransport);
    function CreateDeviceInstance(const Parent: IDeviceInstance;
      const Config: IDevice; ServerReq: IServerRequesting): IDeviceInstance; override;
    function DecodeEventAttributes(Buffer: Pointer; Size: Integer;
      out EventReasonInfo: TEventReasonInfo): TString; override;


    function GetDatabaseModelExt: IRubezh3DatabaseModelExt;
    function doGetCurrentSignature: TDeviceSignature;
    function GetLineCount: integer;



    { IAutoDetectInstances }
    function AutoDetectInstances(const ParentInstance: IDeviceInstance; PNPOnly: Boolean): TDetectResult; virtual;

    { ICrossLinkedDevice }
    function HasCrossLinks(const Device: IDevice): boolean;
    function GetInternalDeviceType: integer;

    { ICustomIOCTLFunctions }
    function IOCTL_GetFunctionCount: integer;
    function IOCTL_GetFunctionInfo(Index: Integer): TIOCTLFunctionInfo;

  public
    constructor Create(const DeviceRegistry: IDeviceRegistry; PanelDeviceMod: PPanelDeviceMod;
      DeviceClass: TRubezh3DeviceClass);
  end;

  TConnectionState = (csNotConnected, csConnected, csIntermediate,  csConnectionLost);

  TDatabaseState = (dbUnknown, dbValidated, dbInvalid);

  // Очередь для экстренного подчитывания событий
  TQueuedRead = record
    DevAddress: integer;
    DeviceType: TRubezh3DeviceType;
    StartTime: TDatetime;
    FirstTime: boolean;
    RetryCount: integer;
  end;

  // дата время, первая буква автора версии прошивки
  TRawVerSoft = array[0..4] of byte;

  IRubezhInternal = interface
    ['{E7ADCC0C-B44A-4DBB-AA9A-E11A380088A0}']
    procedure QueueDatabaseRead(DevAddress: Integer; DevType: TRubezh3DeviceType; NoDelay: boolean);
  end;

  TMD5DigestInfo = record
    version: integer;
    digest: TMD5Digest;
  end;

  TRubezh3Device = class(TCustomRubezh3Device, IRubezh3DeviceInstance, IRubezhInternal, IIndexedEventLog, IDeviceDatabaseRead,
    IDeviceDatabaseWrite, IDeviceTimer, ISoftUpdates, ISoftUpdates2, IRemoteReload, {$IFDEF Debug_DUMPMEM} IReadDump, {$ENDIF}
    IPasswordManagement, IDescriptionString, IGuardData, IMDSData, IDeviceActionLog, ISupportBackgroundTasks,
    ISelfIdentifyingDevice, IChildStatePersist,
    ISupportHardwareIgnore, ISupportRuntimeMethods, ISoftUpdatesVerifyVersions, IDescriptionStringEx,
    IDeviceAsHardwareKey, IWriteRawProperties, ICustomIOCTLFunctionExecute, IChildIOCTLFunctionExecute)
  private
    FSetDateTime: Boolean;
    FConnectionState: TConnectionState;
    FServerReq: IServerRequesting;

    FTotalSize: Integer;
    FCompleteSize: Integer;
    FStage: Integer;

    FConfigHashCache: array of TMD5DigestInfo;
    FLastDoubleAddressOnChannel: TDoubleAddressOnChannel;

    FStatesReaded: Boolean;
    FDeviceActionLogList: TEventList;
    FBackgroundTask: IBackgroundTask;
//    FOutDevicesStateKnown: Boolean;
    FCSIndex, FMLIndex, FHardwareIgnore: Integer;
    FLastStateChange: TDatetime;
    FLastSyncTime: TDateTime;

    FEventList: TEventList;

    FUIDReaded: Boolean;
    FUID: TGUID;
    FUIDChanged: Boolean;
    FLastUIDCheck: TDatetime;
{$IFDEF Debug_DUMPMEM}
    FMemDumpID: integer;
{$ENDIF}
//    FStartDropAlarm: TDatetime;
    FLastExceptMessage: string;
    FLastRealExceptMessage: string;

    FNeedCachedDatabase: boolean;
    FDatabase: IRubezh3Database;

    FIgnoreAdditions: IInterfaceList;
    FIgnoreToRemove: IInterfaceList;
    FRuntimeCmdDevices: IInterfaceList;
    FRuntimeCmdNames: TStringList;
    FRuntimeCmdUserInfos: TStringList;
    FRuntimeCmdParams: TStringList;
    FRuntimeCmdRequestIDs: TStringList;

    FLock: TMsgLock;

    FLineStates: array[0..MAX_LINE_COUNT-1] of boolean;
    FNoLoopBreak: array[0..MAX_LINE_COUNT div 2 - 1] of boolean;

{    FLine1Fail: boolean;
    FLine2Fail: boolean; }

    FLastAlaramReset: TDateTime;
    FLastAlarmDelayStarted: TDateTime;

    FLoaderRAMAddrStream: TMemoryStream;
    FLoaderReady: boolean;

    FPackageVersion: word;
    FUserUpdated: Boolean;

    FAVRInfo: TAVRInfo;

    FNeedClearDatabase: Boolean;
    FDelayedSynchronizationFailed: Boolean;

    FReadQueue: array of TQueuedRead;

    FLastNonUsedtDeviceCount: integer;

    function GetDatabaseState: TDatabaseState;
    procedure SetDatabaseState(Value: TDatabaseState);

    function GetIntErrorState: TDatabaseState;
    procedure SetIntErrorState(Value: TDatabaseState);

    function GetReservedLineState(LineNo: integer): TChannelState;
    procedure SetReservedLineState(LineNo: integer; Value: TChannelState);

    function GetDatabaseModelExt: IRubezh3DatabaseModelExt;
    function GetDatabase: IRubezh3Database;

    procedure TransportOperationNotify(Sender: TObject);
    procedure SetChildState(const Device: IDeviceInstance; const State: IDeviceState; bSetState: Boolean;
      AddressHighPart: word);
    function LogDeviceActionNewEvent(First: boolean = False): PEventRecord;
    function LogEvent(const AnEventMessage: string): PEventRecord;

    procedure UpdateReserveStatus(FirstDT: TDatetime);
    procedure HandleNormalConnection(FirstDT: TDatetime);
    procedure HandleRealtimeException(const ExceptionObj: Exception);
    procedure ReadDeviceRecord(Index: Integer; DeviceType: TRubezh3DeviceType);
(*    function DecodeASPTState(Index: Integer; DeviceType: TRubezh3DeviceType; IntState: word; ForState: TDeviceStateClass): string;
    function DecodeBoltState(Index: Integer; DeviceType: TRubezh3DeviceType; IntState: word; ForState: TDeviceStateClass): string;
    function DecodeMdu1eState(Index: Integer; DeviceType: TRubezh3DeviceType; IntState: word;
      ForState: TDeviceStateClass; var AddDebug: string): string;*)

    procedure DoSynchronizationFailed;
    procedure DoDatabaseHashIsDifferent(const Msg: string);
    procedure DoBackgroundTasks;
    function GetBGTaskStateInfo: TBGTaskStateInfo;
    procedure DoSynchronizationRestored;

    procedure doStartWork(TotalSize: integer);
    procedure doEndWork;
    procedure doIncComplete(value: integer);
    procedure doValidateDeviceType;
    function doGetCurrentSignature: TDeviceSignature;
    procedure doPrepareRAMLoader(const Offset: integer; Stream: TMemoryStream; InROMLoader: boolean = false);
    function GetLineCount: integer;
    function IsRecursionAllowed: boolean;

    procedure doSetDateTime;
    procedure DoResetAltState(const DeviceInstance: IDeviceInstance; State: IDeviceState; Index: Integer; AltStates: TAltStates);
    procedure DoSetAltState(const DeviceInstance: IDeviceInstance; State: IDeviceState; Index: Integer; AltStates: TAltStates; StateStartTime: TDateTime);
    function FindAltState(State: IDeviceState; Index: Integer; AltStates: TAltStates): IDeviceState;
    function HasAltState(const DeviceInstance: IDeviceInstance; State: IDeviceState; Index: Integer; AltStates: TAltStates): boolean;

    procedure InstanceSetState(const DeviceInstance: IDeviceInstance; const State: IDeviceState; StartTime: double = 0);

    function SpoilDatabaseUser: boolean;
    function SpoilDatabaseLoader: boolean;
    function CalculateConfigMD5Cached(const ParentDevice: IDevice; HashVersion: integer): TMD5Digest;
    procedure UpdateDoubling;

  protected
    FPanelDeviceMod: PPanelDeviceMod;
    FInterface: TRubezh3Interface;

    procedure doProgress(const Caption: string; UnknownProgress: Boolean = False);

    function WaitPanelUserMode: Boolean;
    function GetDataTransport: IDataTransport; virtual;

    procedure InternalInitialize; override;
    procedure InternalFinalize; override;
    procedure ResetIOCounter; override;

    procedure SetTimeSlicer(const TimeSlicer: ITimeSlicer); override;

    function GetRawAddrList(const SelfDevice: IDevice; Full: boolean = False): TRawAddressList;
    procedure UpdateAddressList(const SelfDevice: IDevice);

    { IIndexedEventLog }
    function GetNewRecords(SysLastFireIndex: Integer; SysLastSecIndex: Integer; AllRecords: Boolean;
      var FireOverflow: Boolean; var SecOverflow: Boolean; var ResultLastFireIndex: Variant; var ResultLastSecIndex: Variant): IEnumLogRecords; safecall;
    function GetAllRecordsText(HTML: Boolean; SubSystem: TEventSubSystem = essAll): string;

    { IDeviceActionLog }
    function GetActionLogRecords: IEnumlogRecords;
    procedure ClearActionLog;

    { IDeviceDatabaseRead }
    function GetSelf(const DeviceConfig: IDeviceConfig): IDevice;

    { IDeviceDatabaseWrite }
    procedure SetDevices(const DeviceConfig: IDeviceConfig;
      const Device: IDevice; out ResultStr: string; Param: string = '');

    { IDeviceTimer }
    function GetTime: TDateTime;
    procedure SetTime(Value: TDateTime);

    { ISoftUpdates }

    procedure BeginUpdate(PackageVersion: Integer); safecall;
    procedure EndUpdate; safecall;
    function GetMaxBlockSize: Integer; safecall;
    procedure RawRead(MemoryType: Integer; StartAddr, Size: LongWord; var Buf); safecall;
    function RawWrite(MemoryType: Integer; StartAddr, Size: LongWord; var Buf): Boolean; safecall;
    function GetByteMode(MemoryType: Integer): TByteMode; safecall;
    function RequestSoftVersion: Integer; safecall;
    procedure SetSoftwareVersion(Version: Integer); safecall;

    { ISoftUpdates2 }

    function NeedUpdateMemType(MemoryType, Version, CRC: Integer): Boolean;
    procedure BeginUpdateMemType(MemoryType, AddrLow, AddrHigh: Integer);
    procedure EndUpdateMemType(MemoryType: Integer);

    function CompatiblePackageDeviceName(const Value: string): Boolean; virtual;

    { ISoftUpdateVerifyVersions }
    function VerifyVersions(const VersionData: string): string;

    { IRemoteReload }
    procedure Reload;

    { IPasswordManagement }
    procedure SetPassword(Role: Integer; Password: TString);
    Function VerifyAdminPass(PassWord: TString): TString;

    { IDescriptionString }
    function GetDescriptionString: TString;
    function GetDescriptionStringFor(DeviceID: Tstring): TString;

    { IGuardData }
    function GetGuardData: WideString;
    Procedure SetGuardData(GuardData: WideString);

    { IMDSData }
    function GetMDSData: WideString;
{$IFDEF Debug_DUMPMEM}
    { IReadDump }
    function GetDump(Part: Integer; var ASize: dword): String;
{$ENDIF}
    { ISupportBackgroundTasks }
    function CreateBackgroundTask(TaskInfo: TBackgroundTaskInfo): IBackgroundTask;
    procedure SetExecutingTask(const BackgroundTask: IBackgroundTask);
    function GetExecutingTask: IBackgroundTask;

    { ISelfIdentifyingDevice }
    function GetGUIDIdentity(out UID: TGUID): Boolean;

    { ILogWatcher }
{    function NeedEvent: TNeedEventAnswer;
    function NeededEventMessage: string; }

    { IChildStatesPersist}
    procedure SaveChildState(const Device: IDevice; const DeviceStorage: IDeviceStorage);
    procedure RestoreChildState(const Device: IDevice; const DeviceStorage: IDeviceStorage);
    procedure SaveRootParams(const DeviceStorage: IDeviceStorage);
    procedure RestoreRootParams(const DeviceStorage: IDeviceStorage);

    procedure SetNeededEventRecord(EventRecord: PEventRecord);

    { ISupportHardwareIgnore }
    procedure AppendToIgnoreList(const Device: IDeviceInstance);
    procedure RemoveFromIgnoreList(const Device: IDeviceInstance);

    { ISupportRuntimeMethods }
    procedure AppendRuntimeCall(const Device: IDeviceInstance; const MethodName, AnUserInfo: string; Params: string = ''; RequestID: integer = 0);

    { IRubezhInternal }
    procedure QueueDatabaseRead(ADevAddress: Integer; DevType: TRubezh3DeviceType; NoDelay: boolean);

    { IDeviceAsHardwareKey }
    function KeyValid: boolean;

    { IWriteRawProperties }
    function WriteRawProperties(const DeviceCondig: IDeviceConfig; const SelfDevice: IDevice): string;

    { ICustomIOCTLFunctionExecute }
    function IOCTL_ExecuteFunction(const FunctionCode: string; out Reason: string): boolean;

    { IChildIOCTLFunctionExecute }
    function ChildIOCTL_ExecuteFunction(const Device: IDevice; const FunctionCode: string; out Reason: string): boolean;

  public
    constructor Create(const Parent: IDeviceInstance; const Config: IDevice;
      PanelDeviceMod: PPanelDeviceMod; ServerReq: IServerRequesting); override;
    destructor Destroy; override;
    function GetLastProgressTime: TDateTime; safecall;
  end;

type
  TEventOffsetType = (otIndicator, otLine, otZoneExt, otDevice);

//function CheckLogRecordCRC(LogRecord: TRawEventRecord): Boolean;
function RubezhDBDecodeDeviceState(StateClass: TDeviceStateClass; const ExtraData: IParams;
  Gateway: PDBTableGateway; HTMLTags: boolean = false): string;
function RubezhLogDecodeDeviceState(var Rec: TRawEventRecord32; gw: PDBTableGateway): string;
function ParseEventLogMessage(const msg: string; Value: byte): string;
function GetEventOffset(gw: PEventLogGateway; OffsetType: TEventOffsetType): Shortint;
function GetEventIndicatorValue(gw: PEventLogGateway; const EventRecord: TRawEventRecord16): Shortint;
function GetEventPassword(gw: PEventLogGateway; const Model: IRubezh3DatabaseModel; const EventRecord: TRawEventRecord32): Integer;

function GetRubezh3DeviceClass: TRubezh3DeviceClass;

function IsRubezh3FileDB: boolean;
function Rubezh3ParamUnits: string;

const
  COUNT_RUNTIMECALL_PER_REQUEST = 1;

implementation
uses SysConst, FS_ModBus_Common, ExSysutils, XMLClasses,
  {$IFDEF DEBUG}FS_ComModBus_Driver_MM, LogFile,{$ENDIF} FS_BitMaskEdit,
  DateUtils, StrUtils, Variants,
  { На данный момент используем датчики от первого рубежа}
  xdBase, FS_Rubezh03_DBModern;

{.$Define ZoneTest}
{.$Define UseSerials}

type

  TDevicePanelV3 = class(TCustomDeviceClass)
  protected
    procedure Initialize; override;
  public
    constructor Create(const DeviceRegistry: IDeviceRegistry);
  end;

  TSimpleDeviceClassV3 = class(TCustomDeviceClass)
  protected
    procedure Initialize; override;
  public
    constructor Create(const DeviceRegistry: IDeviceRegistry);
    constructor CreateCustom(const DeviceClassID: TDeviceClassID; const DeviceRegistry: IDeviceRegistry);
  end;

  TDeviceNSClass = class(TCustomDeviceClass)
  protected
    procedure Initialize; override;
  public
    constructor Create(const DeviceRegistry: IDeviceRegistry);
  end;

  TDeviceDetectorClassV3 = class(TSimpleDeviceClassV3)
  protected
    procedure Initialize; override;
  public
    constructor Create(const DeviceRegistry: IDeviceRegistry);
  end;

  TDeviceEffectorClassV3 = class(TSimpleDeviceClassV3)
  protected
    procedure Initialize; override;
  public
    constructor Create(const DeviceRegistry: IDeviceRegistry);
  end;

  TDeviceAMClass = class(TSimpleDeviceClassV3)
  protected
    procedure Initialize; override;
  public
    constructor Create(const DeviceRegistry: IDeviceRegistry);
  end;

  TDeviceAsptClassV3 = class(TCustomDeviceClass)
  protected
    procedure Initialize; override;
  public
    constructor Create(const DeviceRegistry: IDeviceRegistry);
  end;

  TDeviceOutputClassV3 = class(TCustomDeviceClass)
  protected
    procedure Initialize; override;
  public
    constructor Create(const DeviceRegistry: IDeviceRegistry);
  end;

  TDeviceBunsV3 = class(TCustomDeviceClass)
  protected
    procedure Initialize; override;
  public
    constructor Create(const DeviceRegistry: IDeviceRegistry);
  end;

  TDeviceBolt = class(TCustomDeviceClass)
  protected
    procedure Initialize; override;
  public
    constructor Create(const DeviceRegistry: IDeviceRegistry);
  end;

  TDevice10AM = class(TCustomDeviceClass)
  protected
    procedure Initialize; override;
  public
    constructor Create(const DeviceRegistry: IDeviceRegistry);
  end;

{$IFDEF BUNSv2}
  TDeviceBunsV3_2 = class(TCustomDeviceClass)
  protected
    procedure Initialize; override;
  public
    constructor Create(const DeviceRegistry: IDeviceRegistry);
  end;
{$ENDIF}

{$IFNDEF DISABLE_SECURITY}
  TDeviceSecPanel = class(TCustomDeviceClass)
  protected
    procedure Initialize; override;
  public
    constructor Create(const DeviceRegistry: IDeviceRegistry);
  end;
{$ENDIF}

  TDevice4APanel = class(TCustomDeviceClass)
  protected
    procedure Initialize; override;
  public
    constructor Create(const DeviceRegistry: IDeviceRegistry);
  end;

  TDeviceMDS = class(TCustomDeviceClass)
  protected
    procedure Initialize; override;
  public
    constructor Create(const DeviceRegistry: IDeviceRegistry);
  end;

  TDeviceIndicator = class(TCustomDeviceClass)
  protected
    procedure Initialize; override;
  public
    constructor Create(const DeviceRegistry: IDeviceRegistry);
  end;

  TDevicePumpV3 = class(TCustomDeviceClass)
  protected
    procedure Initialize; override;
  public
    constructor Create(const DeviceRegistry: IDeviceRegistry);
  end;

  TDeviceASPTV3 = class(TCustomDeviceClass)
  protected
    procedure Initialize; override;
  public
    constructor Create(const DeviceRegistry: IDeviceRegistry);
  end;

  TDeviceMROrev2 = class(TCustomDeviceClass)
  protected
    procedure Initialize; override;
  public
    constructor Create(const DeviceRegistry: IDeviceRegistry);
  end;

  TDeviceFan = class(TCustomDeviceClass)
  protected
    procedure Initialize; override;
  public
    constructor Create(const DeviceRegistry: IDeviceRegistry);
  end;

  TDeviceFanChild = class(TCustomDeviceClass)
  protected
    procedure Initialize; override;
  public
    constructor Create(const DeviceRegistry: IDeviceRegistry);
  end;

  TDevicePPUV3 = class(TCustomDeviceClass)
  protected
    procedure Initialize; override;
  public
    constructor Create(const DeviceRegistry: IDeviceRegistry);
  end;

  TDeviceLED = class(TCustomDeviceClass)
  protected
    procedure Initialize; override;
  public
    constructor Create(const DeviceRegistry: IDeviceRegistry);
  end;

  TDeviceLEDGroup = class(TCustomDeviceClass)
  protected
    procedure Initialize; override;
  public
    constructor Create(const DeviceRegistry: IDeviceRegistry);
  end;

{$IFDEF RubezhRemoteControl}
  TDeviceRemoteControl = class(TCustomDeviceClass)
  protected
    procedure Initialize; override;
  public
    constructor Create(const DeviceRegistry: IDeviceRegistry);
  end;

  TDeviceRemoteControlFire = class(TCustomDeviceClass)
  protected
    procedure Initialize; override;
  public
    constructor Create(const DeviceRegistry: IDeviceRegistry);
  end;

  TDeviceRCGroup = class(TCustomDeviceClass)
  protected
    procedure Initialize; override;
  public
    constructor Create(const DeviceRegistry: IDeviceRegistry);
  end;

  TRCGroupDriver = class(TServerDeviceDriver)
  protected
    function CreateDeviceInstance(const Parent: IDeviceInstance;
      const Config: IDevice; ServerReq: IServerRequesting): IDeviceInstance; override;
  public
    constructor Create(const DeviceRegistry: IDeviceRegistry);
  end;

  TDeviceRCFireGroup = class(TCustomDeviceClass)
  protected
    procedure Initialize; override;
  public
    constructor Create(const DeviceRegistry: IDeviceRegistry);
  end;

  TRCFireGroupDriver = class(TServerDeviceDriver)
  protected
    function CreateDeviceInstance(const Parent: IDeviceInstance;
      const Config: IDevice; ServerReq: IServerRequesting): IDeviceInstance; override;
  public
    constructor Create(const DeviceRegistry: IDeviceRegistry);
  end;
{$ENDIF}

// групповые устройства
  TGroupAMDeviceClass = class(TSimpleDeviceClassV3)
  protected
    procedure Initialize; override;
  public
    constructor Create(const DeviceRegistry: IDeviceRegistry);
  end;

  TGroupAMPDeviceClass = class(TSimpleDeviceClassV3)
  protected
    procedure Initialize; override;
  public
    constructor Create(const DeviceRegistry: IDeviceRegistry);
  end;

  TGroupRMDeviceClass = class(TSimpleDeviceClassV3)
  protected
    procedure Initialize; override;
  public
    constructor Create(const DeviceRegistry: IDeviceRegistry);
  end;

  TGroupAMPСhildDeviceClass = class(TSimpleDeviceClassV3)
  protected
    procedure Initialize; override;
  public
    constructor Create(const DeviceRegistry: IDeviceRegistry);
  end;

  TMRK30DeviceClass = class(TSimpleDeviceClassV3)
  protected
    procedure Initialize; override;
  public
    constructor Create(const DeviceRegistry: IDeviceRegistry);
  end;

  TMRK30ChildDeviceClass = class(TSimpleDeviceClassV3)
  protected
    procedure Initialize; override;
  public
    constructor Create(const DeviceRegistry: IDeviceRegistry);
  end;

  TCustomSimpleDevice = class(TCustomDeviceInstance, ICustomIOCTLFunctionExecute)
  protected
    { ICustomIOCTLFunctionExecute }
    function IOCTL_ExecuteFunction(const FunctionCode: string; out Reason: string): boolean;
  end;


  { Класс инкапсулирующий работу с панелью }

//  TValidateState = (vsNone, vsValidating, vsValidated);

  TCustomSimpleDriver = class(TServerDeviceDriver, ICustomIOCTLFunctions)
  protected
    function CreateDeviceInstance(const Parent: IDeviceInstance;
      const Config: IDevice; ServerReq: IServerRequesting): IDeviceInstance; override;

    { ICustomIOCTLFunctions }
    function IOCTL_GetFunctionCount: integer;
    function IOCTL_GetFunctionInfo(Index: Integer): TIOCTLFunctionInfo;

  public
    constructor Create(Gateway: Pointer; const DeviceName, DriverName, ShortName: String;
      const DeviceDriverID: TDeviceDriverID;
      const DeviceDriverAliasID: TDeviceDriverID;
      const DeviceRegistry: IDeviceRegistry;
      const AddressMask: string;
      AdditionalOptions: TDriverOptions;
      MinZoneCardinality: Integer = 1;
      MaxZoneCardinality: Integer = 1;
      AddressGroup: Integer = 0;
      AutoCreateRange: PRange = nil;
      ReservedAddresses: Integer = 1;
      NoAddress: Boolean = False;
      DevCategory: TDeviceCategory = dcOther;
      LimitedParentDriver: TDeviceDriverID = '';
      AChildDeviceDriverID: TDeviceDriverID = '';
      ADeviceCount: integer = 0);
  end;

  TLEDDriver = class(TServerDeviceDriver)
  protected
    function CreateDeviceInstance(const Parent: IDeviceInstance;
      const Config: IDevice; ServerReq: IServerRequesting): IDeviceInstance; override;
  public
    constructor Create(const DeviceRegistry: IDeviceRegistry);
  end;

  TLEDGroupDriver = class(TServerDeviceDriver)
  protected
    function CreateDeviceInstance(const Parent: IDeviceInstance;
      const Config: IDevice; ServerReq: IServerRequesting): IDeviceInstance; override;
  public
    constructor Create(const DeviceRegistry: IDeviceRegistry);
  end;

  TDeviceParamsTask = class(TCustomBackgroundTask)
  private
    FDeviceTable: TRubezh3DeviceType;
    FRecordNo: Integer;
    FCurTableRecords: Integer;
    FDevice: TRubezh3Device;
    FTotalRecords: Integer;
    FReadedRecords: Integer;
    FValidated: Boolean;
  protected
    procedure InternalQuantum; override;
    function StateInfo: TBGTaskStateInfo; override;
    function PercentComplete: Integer; override;
  public
    constructor Create(Info: TBackgroundTaskInfo; const Device: TRubezh3Device);
  end;

var
  FIsRubezh3Debug: TIsR3Debug;
  FParamUnits: string; // единица измерения дыма и пыли (дБ/м или %)
  // Вместо реальной записи и чтения БД в приборе, используется файл данных
  // с именем (test\"DB_Тип прибора_ПолныйАдрес.bin" }
  FIsRubezh3FileDB: TIsR3Debug;
  
  FIsBAES: TIsR3Debug;

function GetIniName: string;
begin
  result := IncludeTrailingPathDelimiter(ExtractFilePath(ParamStr(0))) +
      ChangeFileExt(ExtractFileName(ParamStr(0)), '.ini');
end;

function IsRubezh3Debug: boolean;
begin
  if FIsRubezh3Debug = dUnknown then
  begin
    if GetPrivateProfileInt('Options', 'R3_Debug', 0, PChar(GetIniName)) <> 0 then
      FIsRubezh3Debug := dYes else
      FIsRubezh3Debug := dNo;
  end;

  result := FIsRubezh3Debug = dYes;
end;

function Rubezh3ParamUnits: string;
begin
  if FParamUnits = '' then
  try
    FParamUnits := GetIniString(GetIniName, 'Options', 'ParamUnits', 'dB');
  except
    FParamUnits := 'dB';
  end;
  if (FParamUnits <> 'dB') and (FParamUnits <> 'percent') then
    FParamUnits := 'dB';

  result := FParamUnits;
end;

function IsRubezh3FileDB: boolean;
begin
  if FIsRubezh3FileDB = dUnknown then
  begin
    if GetPrivateProfileInt('Options', 'R3_FileDB', 0, PChar(GetIniName)) <> 0 then
      FIsRubezh3FileDB := dYes else
      FIsRubezh3FileDB := dNo;
  end;

  result := FIsRubezh3FileDB = dYes;
end;

function IsBAES: boolean;
begin
  if FIsBAES = dUnknown then
  begin
    if GetPrivateProfileInt('Options', 'R3_BAES', 0, PChar(GetIniName)) <> 0 then
      FIsBAES := dYes else
      FIsBAES := dNo;
  end;

  result := FIsBAES = dYes;
end;

// Возвращает смещение в записи журнала
function GetEventOffset(gw: PEventLogGateway; OffsetType: TEventOffsetType): Shortint;
var
  offset: Shortint;
  HasIndicator: Boolean;
begin
  offset := 0;
  result := -1;

  HasIndicator := True{(gw.EventClass1 <> dscNull) or (gw.EventClass2 <> dscNull)};

  if (OffsetType = otIndicator) then
  begin
    if HasIndicator then
      result := offset else
      result := -1;
    exit;
  end;

  if HasIndicator then
    inc(offset);

  if gw.hasGap3 then
    inc(offset, 1);

  // Тип устройства (1 байт), Панель + Шлейф (1 байт), Адрес (1 байт)
  if (OffsetType = otDevice) then
  begin
    if gw.HasDevice  then
    begin
      result := offset;
    end else
      result := -1;
    exit;
  end;
  if gw.HasDevice then
    inc(offset, Sizeof(TRawEventDeviceInfo));


  if (OffsetType = otZoneExt) then
  begin
    if gw.HasZoneExt then
      result := offset else
      result := -1;
    exit;
  end;
  if gw.HasZone then
    inc(offset, Sizeof(TRawEventExtZoneInfo));


  if (OffsetType = otLine) then
  begin
    if gw.HasLine then
      result := offset else
      result := -1;
    exit;
  end;

{  if gw.HasLine then
    inc(offset); }

end;

function GetEventPassword(gw: PEventLogGateway; const Model: IRubezh3DatabaseModel; const EventRecord: TRawEventRecord32): Integer;
var
  ofs: Shortint;
begin
  result := 0;
  if gw.HasPassword then
  begin
    ofs := GetEventOffset(gw, otDevice);
    if ofs <> -1 then
      with PRawEventDeviceInfo(@EventRecord.Record16.Context[Ofs])^ do
        result := Model.GetPassword(ZoneNoAndPsw, EventRecord.Ext) else
        result := EventRecord.Record16.Context[1];
  end;
end;

function GetEventIndicatorValue(gw: PEventLogGateway; const EventRecord: TRawEventRecord16): Shortint;
var
  Offset: Shortint;
begin
  // Из индикатора вырезаем старший бит, который означает различия дивайсов
  Offset := GetEventOffset(gw, otIndicator);
  if Offset <> -1 then
    result := PByte(@EventRecord.Context[Offset])^ and $7F else
    result := 0;
end;

function GetEventIndicatorSign(gw: PEventLogGateway; const EventRecord: TRawEventRecord16): Shortint;
var
  Offset: Shortint;
begin
  // Если в пятом поле (индикаторе) единица по маске 0x80, то это МУКО, иначе МУКД (МДУ)
  // Для МУКО возвращаем -1

  result := 1;
  Offset := GetEventOffset(gw, otIndicator);
  if (Offset <> -1) and ((PByte(@EventRecord.Context[Offset])^ and $80) <> 0) then
    result := -1;
end;

function GetEventClassByVal(gw: PEventLogGateway; indicator: byte): TDeviceStateClass;
begin
  result := gw.EventClass[0];
  if (indicator <> 0) and (indicator <= High(gw.EventClass)) then
    result := gw.EventClass[Indicator];
end;

function GetEventClass(gw: PEventLogGateway; const EventRecord: TRawEventRecord16): TDeviceStateClass;
begin
  result := GetEventClassByVal(gw, GetEventIndicatorValue(gw, EventRecord) and $7F);
end;

function FindAltMessage(TableType: integer; State: byte; EventLogGateway: PEventLogGateway; var Indicator: Shortint): string;
var
  i: integer;
begin
  result := '';
  for i := 0 to High(EventLogGateway.AltMessages) do
    if (EventLogGateway.AltMessages[i].DevType = TableType) then
    begin
      if EventLogGateway.AltMessages[i].UseState then
        Indicator := State * 2 + Indicator;

      result := EventLogGateway.AltMessages[i].EventMessage;
      break;
    end;
end;

function GetDBTableGatewayByEvent(const Model: IRubezh3DatabaseModel; EventLogGateway: PEventLogGateway; LogRecord: PRawEventRecord32;
  var addr: integer; var AltEventMessage: string; var indcopy: shortint; AlternativeGateway: PPDBTableGateway): PDBTableGateway;
var
  offset: integer;
  DeviceInfo: PRawEventDeviceInfo;
  dtype: integer;
begin
  result := nil;
  AltEventMessage := '';
  addr := 0;
  indcopy := GetEventIndicatorValue(EventLogGateway, LogRecord.Record16);

  if AlternativeGateway <> nil then
    AlternativeGateway^ := nil;

  if EventLogGateway.RawEventCode in [$40, $41] then
  begin
    // АСПТ
    result := GetDBTableGatewayList3.FindByType(17);
    addr := Logrecord.Record16.Context[1] + 1;
  end else
  if EventLogGateway.RawEventCode in [$83] then
  begin
    // Выход
    result := GetDBTableGatewayList3.FindByType(19);
    addr := Logrecord.Record16.Context[1] + 1;
  end else
  begin
    offset := GetEventOffset(EventLogGateway, otDevice);
    if offset <> -1  then
    begin
      DeviceInfo := @LogRecord.Record16.Context[offset];

      // Случай ППУ.
      if (GetEventIndicatorSign(EventLogGateway, LogRecord.Record16) < 0) and (DeviceInfo.DeviceType in [$76, $d4, $d2]) then
      begin
        result := GetDBTableGatewayList3.FindByType(16);
        addr := Model.GetAddress(atComplete, DeviceInfo.LocalAddress, DeviceInfo.SystemAddress, LogRecord.Ext);
        if alternativeGateway <> nil then
          alternativeGateway^ := GetDBTableGatewayByOldType(DeviceInfo.DeviceType);
      end else
      if DeviceInfo.DeviceType = $FF then
      begin
        result := GetDBTableGatewayList3.FindByType(10);
      end else
      if DeviceInfo.DeviceType = $70 then
      begin

        if DeviceInfo.LocalAddress < 12  then
          dtype := 1024 else
          dtype := DeviceInfo.LocalAddress + 1024;

        if EventLogGateway.RawEventCode = $37 then
          result := GetDBTableGatewayList3.FindByType(10) else
          result := GetDBTableGatewayList3.FindByType(dtype);

        addr := DeviceInfo.LocalAddress - dtype + 1024;
      end else
      begin
        result := GetDBTableGatewayByOldType(DeviceInfo.DeviceType *
          GetEventIndicatorSign(EventLogGateway, LogRecord.Record16));
        addr := Model.GetAddress(atComplete, DeviceInfo.LocalAddress, DeviceInfo.SystemAddress, LogRecord.Ext);
      end;

      // Меняем сообщение, если есть подходящая альтернатива
      if result <> nil then
      begin
        AltEventMessage := FindAltMessage(result.RawTableType, DeviceInfo.DevState, EventLogGateway, indcopy);
        if AltEventMessage <> '' then
          AltEventMessage := ParseEventLogMessage(AltEventMessage, indcopy);

          // Для неисправности не трогаем индикатор
        if EventLogGateway.RawEventCode = $23 then
          indcopy := GetEventIndicatorValue(EventLogGateway, LogRecord.Record16);
      end;

    end;
  end;
end;

function DoDecodeEventAttributes(const Model: IRubezh3DatabaseModel; const DeviceRegistry: IDeviceRegistry;
  Buf: Pointer; Size: Integer; out EventReasonInfo: TEventReasonInfo): TString;

  procedure SetAlaramReason;
  var
    Gw: PEventLogGateway;
  begin
    with GetEventLogGatewayList3 do
      gw := FindByRawEventCode($0E);
    if gw <> nil then
    begin
      EventReasonInfo.Options := [erSameZone];
      EventReasonInfo.NeededEventMessage := ParseEventLogMessage(gw.EventMessage, 1);
      EventReasonInfo.LimitTimeBack := EVENT_REASON_TIME_BACK;
    end;
  end;

var
  offset: Integer;
  gw: PEventLogGateway;
  DeviceDriver: IDeviceDriver;
  dtype: Integer;
  tt: integer;
  Rec: TRawEventRecord32;
  s: string;
begin
  // Приходит полная запись

  result := '';

  if Size = SizeOf(TRawEventRecord16) then
    Rec.Record16 := PRawEventRecord16(Buf)^ else
  if Size = SizeOf(TRawEventRecord32) then
    Rec := PRawEventRecord32(Buf)^ else
    begin
      result := '<font color=red>' + rsInvalidEventAttrs + '</font>';
      exit;
    end;

  with Rec.Record16 do
  begin
    gw := GetEventLogGatewayList3.FindByRawEventCode(EventCode);
    if gw <> nil then
    begin
      // Системная неисправность
      if EventCode = $4A then
      begin
        case Context[1] of
          0: result := result + '<li>' + rsFireAlaramOut + '</li>';
          1: result := result + '<li>' + rsASPT_Out+ '</li>';
          2: result := result + '<li>' + rsMalfunctionOut + '</li>';
          3: result := result + '<li>' + rsReserveOut + '</li>';
        end;
        result := result + '<li>' + rsInputNo + IntToStr(Context[1] + 1) +  '</li>';
      end else
      if EventCode = $83 then
      begin
        case Context[1] of
          0: result := result + '<li>' + rsASPT_Out + '</li>';
          1: result := result + '<li>' + rsFireAlaramOut +'</li>';
          2, 3: result := result + '<li>' + rsDeviceOutput + IntToStr(Context[1] + 1);
        end;  { расшифоровка отменена в связи с сопоставлением события по Context[0]
        case Context[2] of
          0: result := result + '<li>' + rsShortCircuit + '</li>';
          1: result := result + '<li>' + rsWireBreakage + '</li>';
        end;   }
      end else
      if EventCode in [$31, $45] then // Охранные события (сброс тревоги, постановка, снятие)
      begin
        if rec.Ext[21] = 0 then result := result + '<li>' + rsPasswordPC + '</li>';
      end else
      if EventCode = $28 then // Охранные события (сброс тревоги, постановка, снятие)
      begin
        case rec.Ext[24] of
            0:
              begin
                result := result + '<li>' + rsPasswordPC + '</li>';
                if rec.Ext[23] = 0 then
                  result := result + '<li>' + rsThroughUSB + '</li>' else
                  result := result + '<li>' + rsThroughMSChannel + ' ' + IntToStr(rec.Ext[23]) + '</li>';
              end;
            3: result := result + '<li>' + rsIndicator_ShortName + ' ' + IntToStr(rec.Ext[23]) + '</li>';
            7: result := result + '<li>' + rsRemoteControl_ShortName + ' ' + IntToStr(rec.Ext[23]) + '</li>';
            9: result := result + '<li>' + rsRemoteControlFire_ShortName + ' ' + IntToStr(rec.Ext[23]) + '</li>';
          100: result := result + '<li>' + rsMDS_ShortName + ' ' + IntToStr(rec.Ext[23]) + '</li>';
          101: result := result + '<li>' + rsMDS4_ShortName + ' ' + IntToStr(rec.Ext[23]) + '</li>';
          102: result := result + '<li>' + rsMDS5_ShortName + ' ' + IntToStr(rec.Ext[23]) + '</li>';
          else result := result + '<li>' + rsOper_DeviceTypeUnknown + '(' + IntToStr(rec.Ext[24]) + ')' + ' ' + IntToStr(rec.Ext[23]) + '</li>';
        end;
      end else
      if EventCode =$47 then // Неверный пароль/ключ ТМ
      begin
        case Context[1] of
          0: result := result + '<li>' + rsPanelGuardUser + '</li>';
          1: result := result + '<li>' + rsPanelUserOper + '</li>';
          2: result := result + '<li>' + rsPanelUserInst + '</li>';
          3: result := result + '<li>' + rsPanelUserAdmin + '</li>';
        end;
      end else
      if EventCode = $80 then
      begin
        if Context[1] <> 0 then
          result := result + '<li>' + rsInputNo + IntToStr(Context[1]) +  '</li>';
      end else
      if EventCode = $85 then // потеря связи с мониторинговой станцией (БИ, ПДУ, УОО-ТЛ, МС-1, МС-2)
      begin
        if Context[2] <> 0 then
        begin
          case Context[2] of
              3: s := rsIndicator_ShortName;
              7: s := rsRemoteControl_ShortName;
            100: s := rsMDS_ShortName;
            101: s := rsMDS4_ShortName;
            102: s := rsMDS5_ShortName;
            else s := rsOper_DeviceTypeUnknown + '(' + IntToStr(Context[2]) + ')';
          end;
          result := result + '<li>' + rsDevice + ': ' + s + ' ' + rsAddress + ': ' + IntToStr(Context[1]) + '</li>';
        end;
      end else
      if EventCode = $0D then
      begin
        // 6-ст, 9-мл -> Context[1], Context[4]
        tt := (swap(pword(@Context[1])^) shl 16) or swap(pword(@Context[3])^);

        if tt and $01 <> 0 then
          result := result + '<li>' + rsPanelQueryError +'</li>';
        if tt and $02 <> 0 then
          result := result + '<li>' + rsPanelUnknownAnnType +'</li>';
        if tt and $04 <> 0 then
          result := result + '<li>' + rsPanelInvalidCode + '</li>';
        if tt and $08 <> 0 then
          result := result + '<li>' + rsPanelInvalidParamNo + '</li>';
        if tt and $10 <> 0 then
          result := result + '<li>' + rsPanelInvalidRemoteData +'</li>';
        if tt and $20 <> 0 then
          result := result + '<li>' + rsPanelBadSignature + '</li>';
        if tt and $40 <> 0 then
          result := result + '<li>' + rsPanelIncompatibleVersion+ '</li>';
        if tt and $80 <> 0 then
          result := result + '<li>' + rsPanelDeviceTypeDifferent + '</li>';
        if tt and $100 <> 0 then
          result := result + '<li>' + rsPanelErrorWritingToFlash + '</li>';
        if tt and $200 <> 0 then
          result := result + '<li>' + rsPanelErrorWritingDB + '</li>';
        if tt and $400 <> 0 then
          result := result + '<li>' + rsPanelErrorReadingDB + '</li>';
        if tt and $800 <> 0 then
          result := result + '<li>' + rsPaneInvalidProgramID + '</li>';
        if tt and $1000 <> 0 then
          result := result + '<li>' + rsPanelDatabaseLogicError + '</li>';
        if tt and $2000 <> 0 then
          result := result + '<li>' + rsPanelDuplicateAdresses + '</li>';
        if tt and $4000 <> 0 then
          result := result + '<li>' + rsPanelHardwareFailure + '</li>';

(*        if Context[5] and $01 <> 0 then                                         { $2000 ????}
          result := result + '<li>' + rsPanelDuplicateAdresses + '</li>'; *)

      end;

      if gw.HasPassword then
      begin
        offset := GetEventPassword(gw, Model, Rec);
        case offset of
          0: result := result + '<li>' + rsPasswordPC + '</li>';
          111, $ef: result := result + '<li>' + rsPasswordOper + '</li>';
          42, $aa: result := result + '<li>' + rsPasswordInstaller + '</li>';
          76, $CC: result := result + '<li>' + rsPasswordAdmin + '</li>';
          77, $4E: result := result + '<li>' + rsPasswordButton + '</li>';
          else
            result + '<li>' + rsPasswordUnknown + IntToStr(offset) + '</li>';
        end;
      end;

      offset := GetEventOffset(gw, otLine);
      if offset <> -1 then
      begin
        result := result + '<li>' + rsALS + ': '+ IntToStr(Model.GetLineNo(PByte(@Context[Offset])^))+ '</li>';
      end;

      offset := GetEventOffset(gw, otDevice);
      if offset <> -1 then

      if EventCode in [$40, $41] then
      begin
        // АСПТ
        result := result + '<li>' + rsDeviceASPT +
          GetDBTableGatewayList3.FindByType(17).DeviceName + ' ' + IntToStr(Context[1] + 1);
      end else
      if not (EventCode in [$83]) then
      with PRawEventDeviceInfo(@Context[Offset])^ do
        begin

          if (EventCode = $01) and (Context[9] > 1) then
            SetAlaramReason;

          dtype := integer(DeviceType) * GetEventIndicatorSign(gw, Rec.Record16);

          if (GetEventIndicatorSign(gw, Rec.Record16) = -1)  and  (DeviceType in [$76, $d4, $d2]) then
            dtype := -dtype;

          if dtype = $34 then
          begin
            case Rec.Ext[22] of
              0: result := result + '<li>' + rsDetectorType + ': ' + rsDetectorTypeUnknown + '</li>';
              1: result := result + '<li>' + rsDetectorType + ': ' + rsDetectorTypeGlass + '</li>';
              2: result := result + '<li>' + rsDetectorType + ': ' + rsDetectorTypeDoor + '</li>';
              3: result := result + '<li>' + rsDetectorType + ': ' + rsDetectorTypeVolume + '</li>';
              4: result := result + '<li>' + rsDetectorType + ': ' + rsDetectorTypeAlarm + '</li>';
            end;
          end;

          // Декодируем насосы
          if dtype = $70 then
          begin
            if LocalAddress < 12  then
              tt := 1024 else
              tt := LocalAddress + 1024;

            result := result + RubezhLogDecodeDeviceState(Rec, GetDBTableGatewayList3.FindByType(tt));

            DeviceDriver := GetDeviceDriverByType(DeviceRegistry, tt, 1);
            if DeviceDriver <> nil then
            begin
              result := result + '<li>' + rsDeviceNS + DeviceDriver.DriverShortName + ' ';
              if LocalAddress - tt + 1024 <> 0 then
                result := result + IntToStr(LocalAddress - tt + 1024);

              result := result + '</li>';
            end;
          end else
          begin
            if dtype = $FF then
              DeviceDriver := GetDeviceDriverByType(DeviceRegistry, 10, 1) else
              DeviceDriver := GetDeviceDriverByOldType(DeviceRegistry, dtype);

            if DeviceDriver <> nil then
            begin
              result := result + RubezhLogDecodeDeviceState(Rec,
                GetDBTableGatewayList3.FindByDeviceDriverID(DeviceDriver.DeviceDriverID));

              result := result + '<li>' + rsOper_Device;

              // для ППУ здесь нужна спец. ветка
              if (GetEventIndicatorSign(gw, Rec.Record16) = -1)  and  (DeviceType in [$76, $d4, $d2]) then
              begin
                result := result + rsPPU_ShortName + ' - ';
              end;

              result := result + DeviceDriver.DriverShortName + ' ';

              // Адрес панели за минусом старшего бита шлейфа
              if Model.GetAddress(atSystem, LocalAddress, SystemAddress, Rec.Ext) <> 0 then
                result := result + ' ' + IntToStr(Model.GetAddress(atSystem, LocalAddress, SystemAddress, Rec.Ext)) + '.';
              result := result + IntToStr(Model.GetAddress(atLineNo, LocalAddress, SystemAddress, Rec.Ext)) + '.';
              result := result + IntToStr(Model.GetAddress(atLocal, LocalAddress, SystemAddress, Rec.Ext)) + '</li>';

{              if (SystemAddress and $7F) <> 0 then
                result := result + ' ' + IntToStr(SystemAddress and $7F) + '.';
              result := result + IntToStr((SystemAddress and $80) shr 7 + 1) + '.';
              result := result + IntToStr(LocalAddress) + '</li>'; }
            end;

          end;

          if Model.GetZoneNo(ZoneNoAndPsw) <> 0 then
            result := result + '<li>' + rsOper_Zone + IntToStr(Model.GetZoneNo(ZoneNoAndPsw)) + '</li>';
        end;

      offset := GetEventOffset(gw, otZoneExt);
      if offset <> -1 then
      begin
        with PRawEventExtZoneInfo(@Context[Offset])^ do
        begin
          if Model.GetZoneNo(ZoneOrDivNo) <> 0 then
            result := result + '<li>' + rsOper_Zone + IntToStr(Model.GetZoneNo(ZoneOrDivNo)) + '</li>';
        end;
      end;
    end;

    if EventCode = $FF then
    begin
       result := result + '<li>' + IOCTLNames[TIOCTLCode(Context[1])] + '</li>';
    end;

    {$IFDEF Rubezh3Developer}
    if IsRubezh3Debug then
    begin
      result := result + '<br><br><b>' + rsDebugContext + '</b> <br>' + BufferToHexString(Context[0], Sizeof(Context));
      if Size = SizeOf(TRawEventRecord32) then
        result := result + '<br>' + BufferToHexString(Rec.Ext[15], Sizeof(Rec.Ext));
    end;

    {$ENDIF}

  end;

end;

function SameDigest(D1, D2: TMD5Digest): Boolean;
begin
  result := CompareMem(@D1, @D2, Sizeof(TMD5Digest));
end;


function GetDeviceStateByID(const StateCode: string): IDeviceState;
var
  i: integer;
begin
  result := nil;
  for i := 0 to High(arDeviceStates) do
    if AnsiSameText(arDeviceStates[i].StateCode, StateCode) then
    begin
      result := arDeviceStates[i].Intf;
      break;
    end;
  if result = nil then
    raise Exception.CreateFmt(rsStateByCodeNotFound, [StateCode]);
end;

function GetDeviceStateIndexByID(const StateCode: string): Integer;
var
  i: integer;
begin
  result := -1;
  for i := 0 to High(arDeviceStates) do
    if AnsiSameText(arDeviceStates[i].StateCode, StateCode) then
    begin
      result := i;
      break;
    end;
  if result = -1 then
    raise Exception.CreateFmt(rsStateByCodeNotFound, [StateCode]);
end;

function IsStateComaptibleWithDevice(const State: TPanelState; const Device: IDeviceInstance): boolean;
var
  gateway: PDBTableGateway;
begin
  result := not state.HasFilter;
  if not result then
  begin
    // получаем драйвер для типа устройства
    gateway := GetDBTableGatewayList3.FindByType(state.FilterTableType);
    result := (gateway <> nil) and SameText(Device.DeviceDriver.DeviceDriverID, gateway.DeviceDriverID);

    if result and State.HasNotFilter then
      result := gateway.RawTableType <> State.NotForTableType;
  end;
end;

function IsCompatibleTableType(const State: TPanelState; TableType: integer; OnlySelfType: boolean = false): boolean;
begin
  result := not State.HasFilter and not OnlySelfType;
  if not result then
    result := (State.FilterTableType = TableType);

  if result and State.HasNotFilter then
    result := TableType <> State.NotForTableType;
end;

function OnlyAnsiUpper(const s: string): string;
const
  Alpha = ['A'..'Z', 'a'..'z'];
var
  i: integer;
begin
  result := s;
  for i := 1 to Length(Result) do
  begin
    if not (Result[i] in Alpha) then
      CharUpperBuff(@result[i], 1);
    if Result[i] = '№' then
      Result[i] := 'N';
  end;
end;

function DeviceGetDataTransport(const DeviceInstance: IDeviceInstance): IDataTransport;
var
  Instance: IDeviceInstance;
begin
  result := nil;
  Instance := DeviceInstance;
  while Instance <> nil do
  begin
    if (Instance.DeviceInitState = dsActive) and
      Supports(Instance, IDataTransport, Result) then
        break;
    Instance := Instance.ParentInstance;
  end;
end;

procedure SetFailureType(const Device: IDeviceInstance; const FailureType: string; Recursive: boolean = True);
var
  Param: IParam;
  Child: IDeviceInstance;
begin
  if Device.DeviceDriver.DeviceClassID = sdcDevicePumpV3 then
    exit;

  Param := Device.DeviceParams.FindParam('FailureType');
  if Param <> nil then
  begin
    if FailureType <> '' then
      Param.SetValue(FailureType) else
      Param.SetValue(NULL);
  end;

  if Recursive then
    with Device.EnumChildren do
      while Next(Child) = S_OK do
        SetFailureType(Child, FailureType, Recursive);
end;

procedure SetLineFailureType(const Device: IDeviceInstance; const FailureType: string; AddressHighPart: integer; SkipSelf: boolean);
var
  Param: IParam;
  Child: IDeviceInstance;
begin
  if Device.DeviceDriver.DeviceClassID = sdcDevicePumpV3 then
    exit;

  if not SkipSelf and (GetAddressHighPart(Device.Device) = AddressHighPart) then
  begin
    Param := Device.DeviceParams.FindParam('FailureType');
    if Param <> nil then
    begin
      if FailureType <> '' then
        Param.SetValue(FailureType) else
        Param.SetValue(NULL);
    end;
  end;

  with Device.EnumChildren do
    while Next(Child) = S_OK do
      SetLineFailureType(Child, FailureType, AddressHighPart, False);
end;

function GetRubezh3DeviceClass: TRubezh3DeviceClass;
begin
  result := TRubezh3Device;
end;

procedure RegisterDevices(const DeviceRegistry: IDeviceRegistry);
var
  i: integer;
  AutoCreateRange: TRange;
  Options: TDriverOptions;
  DevCategory: TDeviceCategory;
begin
  with DeviceRegistry do
  begin
    RegisterDeviceClass(TSimpleDeviceClassV3.Create(DeviceRegistry));
    RegisterDeviceClass(TDeviceNSClass.Create(DeviceRegistry));
    RegisterDeviceClass(TDeviceDetectorClassV3.Create(DeviceRegistry));
    RegisterDeviceClass(TDeviceEffectorClassV3.Create(DeviceRegistry));
    RegisterDeviceClass(TDeviceAMClass.Create(DeviceRegistry));
    RegisterDeviceClass(TDeviceAsptClassV3.Create(DeviceRegistry));
    RegisterDeviceClass(TDeviceBolt.Create(DeviceRegistry));

// Выходы (автосоздаваемые 4шт) в 2ОП и 4А
    RegisterDeviceClass(TDeviceOutputClassV3.Create(DeviceRegistry));

    RegisterDeviceClass(TDevicePanelV3.Create(DeviceRegistry));
    RegisterDeviceClass(TDevice10AM.Create(DeviceRegistry));
    if not IsBAES then
    begin
      RegisterDeviceClass(TDevice4APanel.Create(DeviceRegistry));
      RegisterDeviceClass(TDeviceIndicator.Create(DeviceRegistry));
      {$IFDEF RubezhRemoteControl}
      RegisterDeviceClass(TDeviceRemoteControl.Create(DeviceRegistry));
      RegisterDeviceClass(TDeviceRemoteControlFire.Create(DeviceRegistry));
      {$ENDIF}
      RegisterDeviceClass(TDeviceMDS.Create(DeviceRegistry));
      RegisterDeviceClass(TDeviceMROrev2.Create(DeviceRegistry));
      RegisterDeviceClass(TDeviceFan.Create(DeviceRegistry));
      RegisterDeviceClass(TDeviceFanChild.Create(DeviceRegistry));
      RegisterDeviceClass(TMRK30DeviceClass.Create(DeviceRegistry));
      {$IFDEF BUNSv2}
      RegisterDeviceClass(TDeviceBunsV3_2.Create(DeviceRegistry));
      {$ENDIF}
      {$IFNDEF DISABLE_SECURITY}
      RegisterDeviceClass(TDeviceSecPanel.Create(DeviceRegistry));
      {$ENDIF}
    end;
    RegisterDeviceClass(TDeviceASPTV3.Create(DeviceRegistry));
    RegisterDeviceClass(TDevicePPUV3.Create(DeviceRegistry));
    RegisterDeviceClass(TDeviceLEDGroup.Create(DeviceRegistry));
    {$IFDEF RubezhRemoteControl}
    RegisterDeviceClass(TDeviceRCGroup.Create(DeviceRegistry));
    RegisterDeviceClass(TDeviceRCFireGroup.Create(DeviceRegistry));
    {$ENDIF}

    RegisterDeviceClass(TDeviceLED.Create(DeviceRegistry));
    RegisterDeviceClass(TMRK30ChildDeviceClass.Create(DeviceRegistry));
    RegisterDeviceClass(TGroupAMDeviceClass.Create(DeviceRegistry));
    RegisterDeviceClass(TGroupAMPDeviceClass.Create(DeviceRegistry));
    RegisterDeviceClass(TGroupRMDeviceClass.Create(DeviceRegistry));
    RegisterDeviceClass(TGroupAMPСhildDeviceClass.Create(DeviceRegistry));

    {$IFDEF Rubezh3BUNS}
    RegisterDeviceClass(TDeviceBunsV3.Create(DeviceRegistry));
    RegisterDeviceClass(TDevicePumpV3.Create(DeviceRegistry));
    {$ENDIF}

{$IFNDEF NoRubezhPanels}
    if GetIniString(GetIniName, 'Options', 'R3_Name', '') <> '' then
      Rubezh3Mod.DeviceName := GetIniString(GetIniName, 'Options', 'R3_Name', '');

    if GetIniString(GetIniName, 'Options', 'R3_ShortName', '') <> '' then
    begin
      Rubezh3Mod.DeviceShortNameAlias := Rubezh3Mod.DeviceShortName;
      Rubezh3Mod.DeviceShortName := GetIniString(GetIniName, 'Options', 'R3_ShortName', '');
    end;

    RegisterDeviceDriver(TRubezh3Driver.Create(DeviceRegistry, @Rubezh3Mod, TRubezh3Device));
    {$IFDEF Rubezh3BUNS}
    RegisterDeviceDriver(TRubezh3Driver.Create(DeviceRegistry, @Buns3Mod, TRubezh3Device));
    {$ENDIF}
    if not IsBAES then
    begin
      RegisterDeviceDriver(TLEDGroupDriver.Create(DeviceRegistry));
      RegisterDeviceDriver(TLEDDriver.Create(DeviceRegistry));
      {$IFDEF RubezhRemoteControl}
      RegisterDeviceDriver(TRCGroupDriver.Create(DeviceRegistry));
      RegisterDeviceDriver(TRCFireGroupDriver.Create(DeviceRegistry));
      {$ENDIF}
      {$IFDEF BUNSv2}
      RegisterDeviceDriver(TRubezh3Driver.Create(DeviceRegistry, @Buns3v2Mod, TRubezh3Device));
      {$ENDIF}
      RegisterDeviceDriver(TRubezh3Driver.Create(DeviceRegistry, @MDS3Mod, TRubezh3Device));
      RegisterDeviceDriver(TRubezh3Driver.Create(DeviceRegistry, @MDS4Mod, TRubezh3Device));
      RegisterDeviceDriver(TRubezh3Driver.Create(DeviceRegistry, @MDS5Mod, TRubezh3Device));
      RegisterDeviceDriver(TRubezh3Driver.Create(DeviceRegistry, @IndicatorMod, TRubezh3Device));
      RegisterDeviceDriver(TRubezh3Driver.Create(DeviceRegistry, @Rubezh4Mod, TRubezh3Device));
      {$IFNDEF DISABLE_SECURITY}
      RegisterDeviceDriver(TRubezh3Driver.Create(DeviceRegistry, @Rubezh2OPMod, TRubezh3Device));
      {$ENDIF}
      {$IFDEF RubezhRemoteControl}
      RegisterDeviceDriver(TRubezh3Driver.Create(DeviceRegistry, @RemoteControlMod, TRubezh3Device));
      RegisterDeviceDriver(TRubezh3Driver.Create(DeviceRegistry, @RemoteControlFireMod, TRubezh3Device));
      {$ENDIF}
    end;

    RegisterDeviceDriver(TRubezh3Driver.Create(DeviceRegistry, @Rubezh10Mod, TRubezh3Device));
//    RegisterDeviceDriver(TRubezh3Driver.Create(DeviceRegistry, @Rubezh2AOMod, TRubezh3Device));

{$ENDIF}

    if IsBAES then
    begin
      with GetDBTableGatewayList3 do
        for i := Count - 1 downto 0 do
          if Items[i]^.deviceVisibility < 1  then
            Delete(i)
    end else
    begin
      with GetDBTableGatewayList3 do
        for i := Count - 1 downto 0 do
          if (Items[i]^.deviceVisibility < 0) or (Items[i]^.deviceVisibility >= 2)   then
            Delete(i)
         {$IFDEF DISABLE_SECURITY}
         else
         if SameText(Items[i].DevType, 'Security') then
           Delete(i)
        {$ENDIF}
    end;


    with GetDBTableGatewayList3 do
      for i := 0 to Count - 1 do
        with Items[i]^ do
        if (DeviceDriverID <> '') {and (not IsBAES or BaseDevice)} then
        begin
          Options := [];

          case SingleType of
            stInParent: Options := Options + [optSingleInParent];
            stInZone: Options := Options + [optSingleInZone];
          end;

          if SameText(Category, 'effector') then
            DevCategory := dcEffector else
          if SameText(Category, 'detector') then
            DevCategory := dcDetector else
          if SameText(Category, 'panel') then
            DevCategory := dcPanel else
          if SameText(Category, 'transport') then
            DevCategory := dcTransport else
            DevCategory := dcOther;

          if SameText(DevType, 'Fire') then
            Options := Options + [optFireOnly] else
          if SameText(DevType, 'Tech') then
            Options := Options + [optTechOnly] else
          if SameText(DevType, 'Security') then
          begin
            {$IFDEF DISABLE_SECURITY}
            Continue;
            {$ENDIF}
            Options := Options + [optSecOnly];
          end;
          if IgnoreInZoneState then
            Options := Options + [optIgnoreInZoneState];
          if Ignorable then
            Options := Options + [optIgnorable];


          {$IFNDEF Rubezh3BUNS}
          if (DeviceClassID = sdcDevicePumpV3) then
            Continue;
          {$ENDIF}

          if (DeviceClassID = sdcDevicePumpV3) then
          begin
            if (AddressMask = '') then
              AddressMask := Buns3Mod.ChildAddressMask;
          end else
          if (DeviceClassID <> sdcRealASPTV3) and (DeviceClassID <> sdcOutputV3) and (DeviceClassID <> sdcMRK30Child) then
          begin
            AddressMask := Rubezh3Mod.ChildAddressMask;
          end;

          if (DeviceClassID = sdcMRK30Child) or (DeviceClassID = sdcFanChild) then
            Options := Options + [optAddresIsRelative, optUseParentAddressSystem]; 

          if DBRecordType = rtDevice then
          begin
            if DeviceClassID = '' then
              DeviceClassID := sdcSimpleDeviceV3;

            if IsOutDevice or IsOutDevice2  then
              Options := Options + [optOutDevice];

            RegisterDeviceDriver(TCustomSimpleDriver.Create(Items[i], DeviceClassID,
              DeviceName, ShortName,
              DeviceDriverID, DeviceDriverAliasID, DeviceRegistry, AddressMask, Options, Min(1, abs(MaxOutputDeviceCount)), MaxOutputDeviceCount,
              AddressGroup, nil, 1, NoAddress, DevCategory, LimitParentDriverID));
          end else
          if DBRecordType = rtChild then
          begin
            AutoCreateRange.Enabled := True;
            AutoCreateRange.AddrFrom := 1;
            AutoCreateRange.AddrTo := AutoCreateRange.AddrFrom + deviceCount - 1;

            if IsOutDevice or IsOutDevice2  then
              Options := Options + [optOutDevice];

            RegisterDeviceDriver(TCustomSimpleDriver.Create(Items[i], DeviceClassID,
              DeviceName, ShortName,
              DeviceDriverID, DeviceDriverAliasID, DeviceRegistry, AddressMask, Options, Min(1, abs(MaxOutputDeviceCount)), MaxOutputDeviceCount,
              AddressGroup, @AutoCreateRange, 1, NoAddress, DevCategory, LimitParentDriverID));
          end else
          if DBRecordType = rtComposite then
          begin
            Options := Options + [optHasInternalAddress];

            RegisterDeviceDriver(TCustomSimpleDriver.Create(Items[i], DeviceClassID,
              DeviceName, ShortName,
              DeviceDriverID, DeviceDriverAliasID, DeviceRegistry, AddressMask, Options,
              0, 0, AddressGroup, nil, deviceCount, NoAddress, DevCategory, LimitParentDriverID, ChildDeviceDriverID, DeviceCount));
          end else
          if DBRecordType = rtOutputDevice then
          begin
            if DeviceClassID = '' then
              DeviceClassID := sdcDeviceNS;

            AutoCreateRange.Enabled := True;
            if RawTableType = Integer(dtValve) then
              AutoCreateRange.AddrFrom := 1 else
              AutoCreateRange.AddrFrom := 0;
            AutoCreateRange.AddrTo := MaxOutputDeviceCount - 1 ;

            if DeviceClassID = sdcRealASPTV3 then
            begin
              AutoCreateRange.AddrFrom := 1;
              RegisterDeviceDriver(TCustomSimpleDriver.Create(Items[i], DeviceClassID,
                DeviceName, ShortName,
                DeviceDriverID, DeviceDriverAliasID, DeviceRegistry, AddressMask, Options + [optOutDevice], 0, 0,
                AddressGroup, @AutoCreateRange, 1, NoAddress, DevCategory, LimitParentDriverID));
              AutoCreateRange.AddrFrom := 1
            end else
            if DeviceClassID = sdcOutputV3 then
            begin
              AutoCreateRange.AddrFrom := 1;
              RegisterDeviceDriver(TCustomSimpleDriver.Create(Items[i], DeviceClassID,
                DeviceName, ShortName,
                DeviceDriverID, DeviceDriverAliasID, DeviceRegistry, AddressMask, Options + [optOutDevice], 0, 0,
                AddressGroup, @AutoCreateRange, 1, NoAddress, DevCategory));
              AutoCreateRange.AddrFrom := 1
            end else
            RegisterDeviceDriver(TCustomSimpleDriver.Create(Items[i], DeviceClassID,
              DeviceName, ShortName,
              DeviceDriverID, DeviceDriverAliasID, DeviceRegistry, AddressMask, Options + [optOutDevice], 0, -1{MaxOutputDeviceCount},
              AddressGroup, @AutoCreateRange, 1, NoAddress, DevCategory));
          end;
    end;

    // Удаляем композитные устройства
    with GetDBTableGatewayList3 do
      for i := Count - 1 downto 0 do
        if (Items[i]^.DBRecordType = rtComposite) then
          Delete(i)

  end;
end;

function CheckBitInStateWord(StateWord: word; BitNo: Byte; SwapWord: Boolean = False): Boolean;
begin
  if SwapWord then
    result := swap(StateWord) and (1 shl BitNo) <> 0 else
    result := StateWord and (1 shl BitNo) <> 0;
end;

function CheckValueInStateWord(StateWord: word; SBit, EBit: Byte; Value: dword): Boolean;
begin
  result := GetBitRange(StateWord, SBit, EBit) = Value;
end;

function TriadFromInteger(Value: Integer): TTriad;
begin
  result[0] := Value and $00FF0000 shr 16;
  result[1] := Value and $0000FF00 shr 8;
  result[2] := Value and $000000FF;
end;

function GetByte(const Datablock: IDataBlock; BytePos: Integer): byte;
begin
  if Datablock.Size > BytePos then
    result := PByteArray(Datablock.Memory)^[BytePos] else
    raise Exception.Create(rsInvalidPacketReceived);
end;

function GetWord(const Datablock: IDataBlock; BytePos: Integer): Word;
begin
  if Datablock.Size > BytePos + 1 then
    result := Word(Pointer(Integer(Datablock.Memory) + BytePos)^) else
    raise Exception.Create(rsInvalidPacketReceived);
end;

function CheckLogRecordCRC(LogRecord: TRawEventRecord16): Boolean;
var
  b, i: byte;
begin
  b := PByteArray(@LogRecord)[0];
  for I := 1 to Sizeof(TRawEventRecord16) - 1 do
    b := b xor PByteArray(@LogRecord)[i];
  result := (b = 0)
end;

function FormatStateItem(const s: string; Tags: Boolean): string;
begin
  if tags then
    result := result + '<li>' + s + '</li>' else
    result := s;
end;

function DecodeStateByte(State: byte; Bits: array of TStateBit; Tags: Boolean = True): string;
var
  i: integer;
begin
  result := '';
  for i := Low(Bits) to High(Bits) do
  begin
    if (Bits[i].IsFullValue and (State = Bits[i].No) and Tags) or
      (not Bits[i].IsFullValue and (State and (1 shl Bits[i].No) <> 0)) then
    begin
      if not Tags then
      begin
        if result <> '' then
          result := result + ', ';
        result := result + Bits[i].Value;
      end else
        result := result + '<li>' + Bits[i].Value + '</li>';
    end;

  end;
end;

function RubezhDBDecodeDeviceState(StateClass: TDeviceStateClass; const ExtraData: IParams;
  Gateway: PDBTableGateway; HTMLTags: boolean = false): string;

  procedure AppendResultStr(const s: string);
  begin
    if s = '' then
      exit;

    if not HTMLTags then
    begin
      if result <> '' then
        result := result + ', ';
      result := result + s;
    end else
      result := result + s;
  end;

var
  i: integer;
  Param: IParam;
begin
  result := '';

  if Gateway = nil then
    exit;

  for i := 0 to High(Gateway.Detalizations) do
    if Gateway.Detalizations[i].StateClass = StateClass then
    begin
      Param := ExtraData.FindParam(Gateway.Detalizations[i].SourceName);
      if Param <> nil then
      begin
        Assert(Gateway.Detalizations[i].Dictionary <> nil);
        AppendResultStr(Gateway.Detalizations[i].Dictionary.GetFormatedResult(Param.GetValue, HTMLTags));
      end;
    end;

end;

function RubezhLogDecodeDeviceState(var Rec: TRawEventRecord32; gw: PDBTableGateway): string;
var
  evgw: PEventLogGateway;
  offset: integer;
  i: integer;
  ExtraData: IParams;
begin
  result := '';

  if gw = nil then
    exit;


  with Rec.Record16 do
  begin
    evgw := GetEventLogGatewayList3.FindByRawEventCode(EventCode);
    if evgw = nil then
      exit;

    offset := GetEventOffset(evgw, otDevice);
    if offset <> -1 then
      with PRawEventDeviceInfo(@Context[Offset])^ do
      begin
        // Спец. детализации
        for i := 0 to High(evgw.Detalizations) do
          if (evgw.Detalizations[i].DevType = gw.RawTableType) and
            ((evgw.Detalizations[i].FilterField5 = nil) or
              (evgw.Detalizations[i].FilterField5.IndexOf(IntToStr(Context[0])) <> -1))  then
          begin
            result := evgw.Detalizations[i].Dictionary.GetFormatedResult(DevState, True);
            exit;
          end;

        // Если нет специальных применяем общие
        ExtraData := TParamsImpl.Create;
        ExtraData.SetParamValue('Common_0x80L', Lo(DevState));
        ExtraData.SetParamValue('Common_0x80H', Hi(DevState));
        result := RubezhDBDecodeDeviceState(GetEventClass(evgw, Rec.Record16), ExtraData, gw, True);
      end;
  end;

end;


(*r
  i: integer;
  gw: PDBTableGateway;

  procedure AppendResultStr(const s: string);
  begin
    if not Tags then
    begin
      if result <> '' then
        result := result + ', ';
      result := result + FormatStateItem(s, Tags);
    end else
      result := result + FormatStateItem(s, Tags);
  end;

begin
  result := '';

  if Gateway <> nil then
    gw := Gateway else
    gw := GetDBTableGatewayByOldType(DevType);

  if gw = nil then
    exit;

  if  (TRubezh3DeviceType(abs(gw.RawTableType)) = dtSound) then
  begin

    if (StateClass = dscWarning)  then
      case GetBitRange(DevState, 0, 1) of
        3: result := FormatStateItem(rsPlayingAnalogSound, Tags);
        1: result := FormatStateItem(rsPlayingInternalMessage, Tags);
      end;

    if (StateClass in [dscError, dscNormal]) then
    begin
      case GetBitRange(DevState, 2, 3) of
        1: AppendResultStr(Format(rsResistanceGreater, ['OUT1'{, '9/8'}]));
        3: AppendResultStr(Format(rsResistanceLesser, ['OUT1'{, '7/8'}]));
      end;
      case GetBitRange(DevState, 4, 5) of
        1: AppendResultStr(Format(rsResistanceGreater, ['OUT2'{, '9/8'}]));
        3: AppendResultStr(Format(rsResistanceLesser, ['OUT2'{, '7/8'}]));
      end;
      case GetBitRange(DevState, 6, 7) of
        1: AppendResultStr(Format(rsResistanceGreater, ['OUT3'{, '9/8'}]));
        3: AppendResultStr(Format(rsResistanceLesser, ['OUT3'{, '7/8'}]));
      end;
    end;

  end;

  if (Tags = True) and (TRubezh3DeviceType(abs(gw.RawTableType)) = dtClapan) and (StateClass = dscWarning) then
  begin
    case GetBitRange(DevState, 4, 5) of
      0: result := FormatStateItem(rsClapanOnManual, Tags);
      1: result := FormatStateItem(rsClapanOnButton, Tags);
      2: result := FormatStateItem(rsClapanOnPanel, Tags);
    end;
  end;

  if result = '' then
    for i := 0 to High(gw.Detalizations[StateClass].Bits) do
    begin
      result := DecodeStateByte(DevState, gw.Detalizations[StateClass].Bits, Tags);
      break;
    end;
end; *)

function ParseEventLogMessage(const msg: string; Value: byte): string;
var
  i, j, k: integer;
  s: string;
begin
  result := msg;

  // Если в тексте есть [s1/s2], то s1 - для value=0, а s2 для Value=1
  i := pos('[', msg);
  if i = 0 then
    exit;

  j := pos(']', msg);
  if j < i then
    exit;

  s := copy(msg, i + 1, j - i - 1);

  k := pos('/', s);

  if k = 0 then
    exit;

  delete(result, i, j - i + 1);

  while Value > 0 do
  begin
    s := copy(s, k+1, MaxInt);
    k := pos('/', s);
    if k = 0 then
      break;

    dec(value);
  end;

  if (Value = 0) and (k <> 0) then
  begin
    Insert(copy(s, 1, k - 1), result, i);
    exit;
  end;

  Insert(s, result, i);

end;

{ TDevicePanelV3 }

constructor TDevicePanelV3.Create(const DeviceRegistry: IDeviceRegistry);
begin
  inherited Create(sdcDevicePanelV3, DeviceRegistry);
end;

procedure TDevicePanelV3.Initialize;
begin
  inherited;
//  RegisterParentClass(sdcDeviceBus);
  RegisterParentClass(sdcDeviceBusV3);
  RegisterParentClass(sdcDeviceUSBChannel);
  RegisterClassProperty('Icon', 'Device_Panel');
  RegisterClassProperty('DeviceClassName', rsClass_Panel);
  RegisterClassProperty('Hidden', False);
end;

constructor TSimpleDeviceClassV3.Create(const DeviceRegistry: IDeviceRegistry);
begin
  inherited Create(sdcSimpleDeviceV3, DeviceRegistry);
end;

constructor TSimpleDeviceClassV3.CreateCustom(const DeviceClassID: TDeviceClassID; const DeviceRegistry: IDeviceRegistry);
begin
  inherited Create(DeviceClassID, DeviceRegistry);
end;

procedure TSimpleDeviceClassV3.Initialize;
begin
  inherited;
  RegisterParentClass(sdcDevicePanelV3);
  RegisterParentClass(sdcDevice10AMPanel);
  RegisterParentClass(sdcDeviceUSBPanelV3);
  RegisterParentClass(sdcDevice4APanel);
  RegisterParentClass(sdcDeviceUSB4APanel);

  RegisterParentClass(sdcGroupAM);
  RegisterParentClass(sdcGroupAMP);
  RegisterParentClass(sdcGroupRM);

{$IFDEF Rubezh3BUNS}
  RegisterParentClass(sdcDeviceBunsV3);
  RegisterParentClass(sdcDeviceUSBBunsV3);
{$ENDIF}
{$IFDEF BUNSv2}
  RegisterParentClass(sdcDeviceBunsV3_2);
  RegisterParentClass(sdcDeviceUSBBunsV3_2);
{$ENDIF}
{$IFNDEF DISABLE_SECURITY}
  RegisterParentClass(sdcDeviceSecPanel);
  RegisterParentClass(sdcDeviceUSBSecPanel);
{$ENDIF}
(*{$IFDEF IMITATOR}
  ??? RegisterParentClass(sdcDeviceImitator);
{$ENDIF} *)
  RegisterClassProperty('Icon', 'Device_Device');
  RegisterClassProperty('DeviceClassName', rsClass_EndDevice);
  RegisterClassProperty('HideInTree', True);
end;

  { TDeviceEffectorClassV3 }

constructor TDeviceNSClass.Create(
  const DeviceRegistry: IDeviceRegistry);
begin
  inherited Create(sdcDeviceNS, DeviceRegistry);
end;

procedure TDeviceNSClass.Initialize;
begin
  inherited;
  {$IFDEF Rubezh3BUNS}
  RegisterParentClass(sdcDeviceBunsV3);
  RegisterParentClass(sdcDeviceUSBBunsV3);
  {$ENDIF}
  {$IFDEF BUNSv2}
  RegisterParentClass(sdcDeviceBunsV3_2);
  RegisterParentClass(sdcDeviceUSBBunsV3_2);
  {$ENDIF}
  RegisterClassProperty('Icon', 'Device_Effector');
  RegisterClassProperty('DeviceClassName', rsClass_Effector);
  RegisterClassProperty('HideInTree', True);
end;

{ TDeviceEffectorClassV3 }

constructor TDeviceEffectorClassV3.Create(
  const DeviceRegistry: IDeviceRegistry);
begin
  inherited CreateCustom(sdcDeviceEffectorV3, DeviceRegistry);
end;

procedure TDeviceEffectorClassV3.Initialize;
begin
  inherited;
  GetClassProperties.FindParam('DeviceClassName').SetValue(rsClass_Effector);
end;

{ TDeviceBunsV3 }

constructor TDeviceBunsV3.Create(const DeviceRegistry: IDeviceRegistry);
begin
  inherited Create(sdcDeviceBunsV3, DeviceRegistry);
end;

procedure TDeviceBunsV3.Initialize;
begin
  inherited;
//  RegisterParentClass(sdcDeviceBus);
  RegisterParentClass(sdcDeviceBusV3);
  RegisterParentClass(sdcDeviceUSBChannel);
  RegisterClassProperty('Icon', 'БУНС-01-1');
  RegisterClassProperty('DeviceClassName', rsClass_BUNS);
  RegisterClassProperty('Hidden', False);
end;

{$IFNDEF DISABLE_SECURITY}
{ TDeviceSecPanel }

constructor TDeviceSecPanel.Create(const DeviceRegistry: IDeviceRegistry);
begin
  inherited Create(sdcDeviceSecPanel, DeviceRegistry);
end;

procedure TDeviceSecPanel.Initialize;
begin
  inherited;
  if not Is485Protect then
  begin
    RegisterParentClass(sdcDeviceBusV3);
    RegisterParentClass(sdcDeviceUSBChannel);
  end;
  RegisterClassProperty('Icon', 'Рубеж-2ОП');
  RegisterClassProperty('DeviceClassName', rsClass_2OP);
  RegisterClassProperty('Hidden', False);
end;
{$ENDIF}

{ TDevice4APanel }

constructor TDevice4APanel.Create(const DeviceRegistry: IDeviceRegistry);
begin
  inherited Create(sdcDevice4APanel, DeviceRegistry);
end;

procedure TDevice4APanel.Initialize;
begin
  inherited;
  if not Is485Protect then
  begin
    RegisterParentClass(sdcDeviceBusV3);
    RegisterParentClass(sdcDeviceUSBChannel);
  end;
  RegisterClassProperty('Icon', 'Рубеж-2АМ');
  RegisterClassProperty('DeviceClassName', rsClass_Panel);
  RegisterClassProperty('Hidden', False);
end;

{ TCustomSimpleDriver }

constructor TCustomSimpleDriver.Create(Gateway: Pointer; const DeviceName, DriverName, ShortName: String;
  const DeviceDriverID: TDeviceDriverID;
  const DeviceDriverAliasID: TDeviceDriverID;
  const DeviceRegistry: IDeviceRegistry;
  const AddressMask: string;
  AdditionalOptions: TDriverOptions;
  MinZoneCardinality: Integer = 1;
  MaxZoneCardinality: Integer = 1;
  AddressGroup: Integer = 0;
  AutoCreateRange: PRange = nil;
  ReservedAddresses: Integer = 1;
  NoAddress: Boolean = False;
  DevCategory: TDeviceCategory = dcOther;
  LimitedParentDriver: TDeviceDriverID = '';
  AChildDeviceDriverID: TDeviceDriverID = '';
  ADeviceCount: integer = 0);

  procedure RegisterStates;
  var
    i: integer;
    State: IDeviceState;
  begin
    Rubezh3_ReadConfig;

    for i := 0 to High(arDeviceStates) do
    begin
      if not IsCompatibleTableType(arDeviceStates[i], PDBTableGateway(Gateway).RawTableType)
        {arDeviceStates[i].HasFilter and
        (arDeviceStates[i].FilterTableType <> PDBTableGateway(Gateway).RawTableType) } then
          Continue;

      { Поскольку драйверов все таки может быть несколько, то у них должен быть один интерфейс состояния,
        поэтому создавать интерфейс нужно только однажды }
      if arDeviceStates[i].Intf <> nil then
        RegisterDeviceStateIntf(arDeviceStates[i].Intf) else
        arDeviceStates[i].Intf := RegisterDeviceState(i, arDeviceStates[i].Name, arDeviceStates[i].StateCode,
          arDeviceStates[i].NameSourceParam, arDeviceStates[i].StateClass,
          False, arDeviceStates[i].SubSystem, arDeviceStates[i].Options, arDeviceStates[i].ManualReset, arDeviceStates[i].IsPrimaryState);
    end;

    if (PDBTableGateway(Gateway).DBRecordType = rtComposite) then
    begin
      RegisterDeviceState(psLostConnection, rsLineMalfunction, 'LineFail', '', dscError, True);
    end;

    // ППУ - регистрируем целую пачку состояний для 4-з АМ4Т
    if PDBTableGateway(Gateway).RawTableType = Integer(dtPPU) then
    begin

      for I := 1 to 4 do
      begin
        with RegisterDriverProperty('DevPrefix'+IntToStr(i))^ do
        begin
          Caption := rsChannel + ' ' +IntToStr(i);
          DefaultValue := rsChannel + ' ' +IntToStr(i) + '. [*]';
          ValueType := varString;
          EditType := 'pkText';
          Hidden := True;
        end;
      end;

      with GetDeviceDriverByType(DeviceRegistry, dtAM4T, 1) do
        with EnumSupportedStates do
          while next(State) = S_OK do
            if State.GetStateID >= 0 then
              for I := 1 to 4 do
              begin
                SetLength(PPUAltStates, Length(PPUAltStates) + 1);
                with PPUAltStates[High(PPUAltStates)] do
                begin
                  Source := State;
                  Index := I;
                  Target := RegisterDeviceState(100 * I  + State.GetStateID, {rsChannel + ' ' +IntToStr(i) + '. ' +} State.GetStateName,
                    'Channel' + IntToStr(i) + State.GetStateCode, 'DevPrefix'+IntToStr(i), State.GetStateClass,
                    State.AffectChildren, ssNone, State.GetOptions, State.ManualReset);
                end;
              end;

        with GetDeviceDriverByType(DeviceRegistry, dtAMT, 1) do
          with EnumSupportedStates do
            while next(State) = S_OK do
              if State.GetStateID >= 0 then
                for I := 5 to 6 do
                begin
                  SetLength(PPUAltStates, Length(PPUAltStates) + 1);
                  with PPUAltStates[High(PPUAltStates)] do
                  begin
                    Source := State;
                    Index := I;
                    Target := RegisterDeviceState(100 * I  + State.GetStateID, 'Контроль ' + IntToStr(i-4) + ' ввода ППУ' + '. ' + State.GetStateName,
                      'Channel' + IntToStr(i) + State.GetStateCode, State.GetNameSourceParam + IntToStr(i-4), State.GetStateClass,
                      State.AffectChildren, ssNone, State.GetOptions, State.ManualReset);
                  end;
                end;
    end;
  end;

  procedure RegisterCommonParams(const PostFix, NamePostFix: string; const ANameSourceParam: string = '');
  begin
    with RegisterDeviceParam('FailureType' + PostFix)^ do
    begin
      Caption := rsMalfunction + NamePostFix;
      ValueType := varString;
      EditType := 'pkText';
      Hint := rsMalfunctionType;
      NameSourceParam := ANameSourceParam;


      if PostFix <> '' then
        ShowOnlyInState := True;
    end;

    with RegisterDeviceParam('AlarmReason' + PostFix)^ do
    begin
      Caption := rsAlarmReason + NamePostFix;
      ValueType := varString;
      EditType := 'pkText';
      Hint := rsAlarmReason;
      NameSourceParam := ANameSourceParam;

      if PostFix <> '' then
        ShowOnlyInState := True;
    end;

    with RegisterDeviceParam('OtherMessage' + PostFix)^ do
    begin
      Caption := rsComment + NamePostFix;
      ValueType := varString;
      EditType := 'pkText';
      Hint := rsComment;
      NameSourceParam := ANameSourceParam;

      if PostFix <> '' then
        ShowOnlyInState := True;

    end;
  end;

  procedure RegisterDeviceParams;
  var
    i: integer;
  begin
    if (PDBTableGateway(Gateway).DBRecordType = rtDevice) or (PDBTableGateway(Gateway).DBRecordType = rtChild) then
    begin
      RegisterCommonParams('', '', 'DeviceName');

      if PDBTableGateway(Gateway).RawTableType = Integer(dtPPU) then
        for i := 1 to 4 do
          RegisterCommonParams(IntToStr(i), {'. ' + rsChannel + ' ' + IntToStr(i)}'', 'DevPrefix' + IntToStr(i));
        for i := 1 to 2 do
          RegisterCommonParams(IntToStr(i + 4), '. Контроль ' + IntToStr(i) + ' ввода ППУ');
    end;

    if PDBTableGateway(Gateway).HasSmokeChanel then
    begin
      with RegisterDeviceParam('Smokiness')^ do
      begin
        // новые датчики (старше 15 версии ПО устройства) измеряются в дБ/м, старые в %
        if Rubezh3ParamUnits = 'dB' then
        begin
          Min := 0;
          Max := 0.2;
          ChangeDownDelta := 0.02;
          ChangeUpDelta := 0.02;
          Caption := rsParamSmokinessdB
        end else
        begin
          Min := 0;
          Max := 75;
          ChangeDownDelta := 10;
          ChangeUpDelta := 10;
          Caption := rsParamSmokinessp;
        end;
        ValueType := varDouble;
        DefaultValue := NULL;
        EditType := 'pkText';
        Hint := rsParamSmokinessHint;
        TimeScaleLength := 10; { 10 дней }
{        ChangeDownDelta := Max * 5 / 100;
        ChangeUpDelta := Max * 3 / 100;}
      end;

      with RegisterDeviceParam('Dustiness')^ do
      begin
        if Rubezh3ParamUnits = 'dB' then
        begin
          Min := 0;
          Max := 0.2;
          Caption := rsParamDustinessdB;
          ChangeDownDelta := 0.02;
          ChangeUpDelta := 0.02;
        end else
        begin
          Min := 0;
          Max := 75;
          Caption := rsParamDustinessp;
          ChangeDownDelta := Max * 5 / 100;
          ChangeUpDelta := Max * 3 / 100;
        end;
        ValueType := varDouble;
        DefaultValue := NULL;
        EditType := 'pkText';
        Hint := rsParamDustinessHint;
        TimeScaleLength := 10; { 10 дней }
      end;

    end;

    if PDBTableGateway(Gateway).HasTempChanel then
    begin
      with RegisterDeviceParam('Temperature')^ do
      begin
        Caption := rsParamTemperature;
        ValueType := varInteger;
        DefaultValue := NULL;
        EditType := 'pkText';
        Hint := rsParamTemperatureHint;
        Min := 0;
        Max := 60;
        TimeScaleLength := 10; { 10 дней }
        ChangeDownDelta := Max * 5 / 100;
        ChangeUpDelta := Max * 3 / 100;
      end;
    end;

    if PDBTableGateway(Gateway).HasPressureChanel then
    begin
      with RegisterDeviceParam('Pressure')^ do
      begin
        Caption := rsParamPressure;
        ValueType := varInteger;
        DefaultValue := NULL;
        EditType := 'pkGraphic';
        Hint := rsParamPressureHint;
        Min := 0;
        Max := 100;
        TimeScaleLength := 10; { 10 дней }
        ChangeDownDelta := Max * 5 / 100;
        ChangeUpDelta := Max * 3 / 100;
      end;

{      with RegisterDeviceParam('PressureState')^ do
      begin
        Caption := 'Давление';
        ValueType := varString;
        EditType := 'pkText';
        Hint := 'Давление';
      end; }

    end;



  end;

  procedure AddPropInfo;

  var i: integer;
      PropInfo: PPropertyTypeInfo;
      PropName: string;
  begin
    for i := 0 to High(arDevicePropInfos) do
    begin
      if not (arDevicePropInfos[i].DeviceType = PDBTableGateway(Gateway).RawTableType) then
        Continue;

      if arDevicePropInfos[i].PropClass = pcControl then
        PropName := 'Control$' + arDevicePropInfos[i].Name
      else if arDevicePropInfos[i].PropClass = pcConfig then
        PropName := 'Config$' + arDevicePropInfos[i].Name;

      PropInfo := RegisterDriverProperty(PropName);
      PropInfo^ := arDevicePropInfos[i];
      PropInfo^.Name := PropName;

      if arDevicePropInfos[i].PropClass = pcConfig then
      begin
        PropInfo := RegisterDeviceParam(PropName);
        PropInfo^ := arDevicePropInfos[i];
        PropInfo^.Name := PropName;
        PropInfo^.DefaultValue := -1;
        PropInfo^.TimeScaleLength := 10; { 10 дней }
      end;

    end;

  end;

var
  Options: TDriverOptions;
  i: integer;
begin
  inherited Create(DriverName, DeviceName, DeviceDriverID, DeviceRegistry);
  Options := AdditionalOptions + [optCannotDisable];

  // групповое устройство
  if (DeviceName <> 'CB831623-1A79-41F1-8327-8F0BBE7CDC81')         // групповые АМ-4
  and (DeviceName <> '49CA3A66-352B-4C38-83B5-649D126E63F2')        // групповые АМП-4
  and (DeviceName <> 'E4FAF59E-9710-448C-9DDA-544BEC105278') then   // групповые РМ
    Options := Options + [optAllowChangeDriver];

  if ReservedAddresses = 1 then
    Options := Options + [optPlaceable];

  if (PDBTableGateway(Gateway).RawTableType = Integer(dtPPU)) then
  begin
    ReservedAddresses := 7;
    Options := Options + [optPlaceable];
  end;

  // Внимание к последовательности регистрации
  if LimitedParentDriver <> '' then
    self.SetLimitedParentDriver(DeviceRegistry.GetDeviceDriver(DeviceDriverID));

  with RegisterDriverProperty('NotUsed')^ do
  begin
    Caption := rsNotUsed;
    ValueType := varBoolean;
    DefaultValue := false;
    EditType := 'pkBoolean';
    Hint := '';
    hidden := true;
  end;

  if PDBTableGateway(Gateway).IsOutDevice2 and (PDBTableGateway(Gateway).RawTableType <> Integer(dtPPU))
  and (PDBTableGateway(Gateway).RawTableType <> Integer(dtValve)) then
  begin

    Options := Options + [optExtendedZoneLogic];
    with RegisterDriverProperty('ExtendedZoneLogic')^ do
    begin
      Caption := rsParamExtLogic;
      ValueType := varString;
      DefaultValue := 0;
      EditType := 'pkText';
      Hint := '';
      Hidden := True;
    end;

    with RegisterDriverProperty('IsAlarmDevice')^ do
    begin
      Caption := 'IsAlarmDevice';
      ValueType := varBoolean;
      DefaultValue := False;
      EditType := 'pkText';
      Hint := '';
      Hidden := True;
    end;

  end else
  if (PDBTableGateway(Gateway).RawTableType = Integer(dtRealASPT)) then
  begin
    Options := Options + [optExtendedZoneLogic];
    SetcaseCount(0);
    with RegisterDriverProperty('ExtendedZoneLogic')^ do
    begin
      Caption := rsParamExtLogic;
      ValueType := varString;
      DefaultValue := 0;
      EditType := 'pkText';
      Hint := '';
      Hidden := True;
    end;

{$IFNDEF DISABLE_SECURITY}
  end else
  if (PDBTableGateway(Gateway).RawTableType = Integer(dtOutput)) then
  begin
    Options := Options + [optExtendedZoneLogic];
    SetcaseCount(0);
    with RegisterDriverProperty('ExtendedZoneLogic')^ do
    begin
      Caption := rsParamExtLogic;
      ValueType := varString;
      DefaultValue := 0;
      EditType := 'pkText';
      Hint := '';
      Hidden := True;
    end;
{$ENDIF}
  end else
  if PDBTableGateway(Gateway).DeviceClassID = sdcMRK30 then
  begin
    with RegisterDriverProperty('MRK30ChildCount')^ do
    begin
      Caption := rsMRK30ChildCount;
      ValueType := varInteger;
      DefaultValue := 30;
      EditType := 'pkText';
      Hint := Caption;
    end;
  end else
  if (PDBTableGateway(Gateway).RawTableType = Integer(dtAspt)) or
    (PDBTableGateway(Gateway).RawTableType = Integer(dtPPU))   then
  begin
    Options := Options + [optUseParentAddressSystem];

    with RegisterDriverProperty('Config')^ do
    begin
      Caption := rsParamAsptConfig;
      ValueType := varInteger;
      DefaultValue := 0;
      EditType := 'pkText';
      Hint := '';
      hidden := true;
    end;

    with RegisterDriverProperty('InfoTable')^ do
    begin
      Caption := 'Информационное табло';
      ValueType := varBoolean;
      DefaultValue := False;
      EditType := 'pkBoolean';
      Hint := '';
      hidden := true;
    end;

    with RegisterDriverProperty('DeviceSettings')^ do
    begin
      Caption := 'Настройки';
      ValueType := varString;
      DefaultValue := False;
      EditType := 'pkText';
      Hint := '';
      hidden := true;
    end;

    if PDBTableGateway(Gateway).RawTableType = Integer(dtPPU) then
    begin
      with RegisterDriverProperty('{AE9C8097-E0E0-4912-B8E3-128544C3E77D}')^ do
        hidden := true;

      for i := 1 to 4 do
      begin
        with RegisterDriverProperty('MinTreshold'+IntToStr(i))^ do
        begin
          Caption := rsMinTreshold + '. ' + rsChannel + ' ' + IntToStr(i);
          ValueType := varInteger;
          DefaultValue := 0;
          EditType := 'pkText';
          Hint := '';
          hidden := true;
        end;

        with RegisterDriverProperty('MaxTreshold' + IntToStr(i))^ do
        begin
          Caption := rsMaxTreshold + '. ' + rsChannel + ' ' + IntToStr(i);
          ValueType := varInteger;
          DefaultValue := 0;
          EditType := 'pkText';
          Hint := '';
          hidden := true;
        end;

        with RegisterDriverProperty('MaxValue' + IntToStr(i))^ do
        begin
          Caption := rsMaxValue + '. ' + rsChannel + ' ' + IntToStr(i);
          ValueType := varInteger;
          DefaultValue := 0;
          EditType := 'pkText';
          Hint := '';
          hidden := true;
        end;

        with RegisterDriverProperty('MaxDispValue' + IntToStr(i))^ do
        begin
          Caption := rsMinDispValue + '. ' + rsChannel + ' ' + IntToStr(i);
          ValueType := varInteger;
          DefaultValue := 0;
          EditType := 'pkText';
          Hint := '';
          hidden := true;
        end;

        with RegisterDriverProperty('MinDispValue' + IntToStr(i))^ do
        begin
          Caption := rsMinDispValue + '. ' + rsChannel + ' ' + IntToStr(i);
          ValueType := varInteger;
          DefaultValue := 0;
          EditType := 'pkText';
          Hint := '';
          hidden := true;
        end;

        with RegisterDriverProperty('Offset' + IntToStr(i))^ do
        begin
          Caption := rsOffset + '. ' + rsChannel + ' ' + IntToStr(i);
          ValueType := varInteger;
          DefaultValue := 0;
          EditType := 'pkText';
          Hint := '';
          hidden := true;
        end;

        with RegisterDriverProperty('Disabled' + IntToStr(i))^ do
        begin
          Caption := rsDisableMonitoring + '. ' + rsChannel + ' ' + IntToStr(i);
          ValueType := varBoolean;
          DefaultValue := False;
          EditType := 'pkBoolean';
          hidden := true;
        end;

        with RegisterDriverProperty('Precision' + IntToStr(i))^ do
        begin
          Caption := 'Точность' + '. ' + rsChannel + ' ' + IntToStr(i);
          ValueType := varBoolean;
          DefaultValue := False;
          EditType := 'pkBoolean';
          hidden := true;
        end;

        with RegisterDeviceParam('Pressure' + IntToStr(i))^ do
        begin
          Caption := rsParamPressure {rsPressure + '. ' + rsChannel + ' ' + IntToStr(i) + ', бар'};
          ValueType := varInteger;
          DefaultValue := NULL;
          EditType := 'pkGraphic';
          Hint := rsParamPressureHint;
          Min := 0;
          Max := 100;
          TimeScaleLength := 10; { 10 дней }
          NameSourceParam := 'DevPrefix' + IntToStr(i);
        end;

      end;

      with RegisterDriverProperty('Event11')^ do
      begin
        Caption := rsEventNorm;
        ValueType := varString;
        DefaultValue := GetAMTForPPUText(5, 1);
        EditType := 'pkText';
        Hint := '';
        hidden := true;
      end;

      with RegisterDriverProperty('Event21')^ do
      begin
        Caption := rsEventOn;
        ValueType := varString;
        DefaultValue := GetAMTForPPUText(5, 2);
        EditType := 'pkText';
        Hint := '';
        hidden := true;
      end;

      with RegisterDriverProperty('Event12')^ do
      begin
        Caption := rsEventNorm;
        ValueType := varString;
        DefaultValue := GetAMTForPPUText(6, 1);
        EditType := 'pkText';
        Hint := '';
        hidden := true;
      end;

      with RegisterDriverProperty('Event22')^ do
      begin
        Caption := rsEventOn;
        ValueType := varString;
        DefaultValue := GetAMTForPPUText(6, 2);
        EditType := 'pkText';
        Hint := '';
        hidden := true;
      end;

      with RegisterDriverProperty('DeviceName')^ do
      begin
        Caption := 'Имя устройства';
        ValueType := varString;
        DefaultValue := rsMPT_Name + '. [*]';
        EditType := 'pkText';
        Hint := '';
        hidden := true;
      end;

      with RegisterDriverProperty('PPUVersion')^ do
      begin
        Caption := 'Год выпуска';
        ValueType := varInteger;
        DefaultValue := 0;
        EditType := 'pkTextList';
        Hint := '';
        hidden := true;
        ListItems := TParamsImpl.Create;
        with ListItems do
        begin
          NewParam('до 2012 г.', 00);
          NewParam('с 2012 г.', 01);
        end;
      end;

    end else
      with RegisterDriverProperty('{7CA94E08-B31C-4FA9-A23A-24A62A2C639C}')^ do
        hidden := true;

  end else
  if PDBTableGateway(Gateway).RawTableType = Integer(dtGuard)  then
  begin
    with RegisterDriverProperty('GuardType')^ do
    begin
      Caption := rsDetectorType;
      ValueType := varInteger;
      DefaultValue := 0; // 9.03.2011 Саша просил поменять
      EditType := 'pkTextList';
      Hint := 'Тип охранного датчика';
      ListItems := TParamsImpl.Create;
      with ListItems do
      begin
        NewParam(rsDetectorTypeUnknown, 00);
        NewParam(rsDetectorTypeGlass, 01);
        NewParam(rsDetectorTypeDoor, 02);
        NewParam(rsDetectorTypeVolume, 03);
        NewParam(rsDetectorTypeAlarm, 04);
      end;
  end;

  end else
  if PDBTableGateway(Gateway).RawTableType = Integer(dtBolt)  then
  begin

    Options := Options + [optHasControlPanel];

    with RegisterDriverProperty('{920114AE-69BD-4026-A5CE-22BFCF12893E}')^ do
      hidden := true;

    with RegisterDriverProperty('MoveTime')^ do
    begin
      Caption := rsParamMoveTime;
      ValueType := varInteger;
      DefaultValue := 0;
      EditType := 'pkText';
      Hint := '';
      hidden := true;
    end;

    with RegisterDriverProperty('Config8d')^ do
    begin
      Caption := rsParamConfig8d;
      ValueType := varInteger;
      DefaultValue := 0;
      EditType := 'pkText';
      Hint := '';
      hidden := true;
    end;

    with RegisterDriverProperty('Action')^ do
    begin
      Caption := rsEventAction;
      ValueType := varInteger;
      DefaultValue := 0;
      EditType := 'pkText';
      Hint := '';
      hidden := true;
    end;

  end else
  if PDBTableGateway(Gateway).RawTableType = Integer(dtAMT)  then
  begin

    with RegisterDriverProperty('{EC67016F-B7BF-4EFC-AD09-8D3F15BD4D55}')^ do
      hidden := true;

    with RegisterDriverProperty('Event1')^ do
    begin
      Caption := rsEventNorm;
      ValueType := varString;
      DefaultValue := rsEventNormValue;
      EditType := 'pkText';
      Hint := '';
//      hidden := true;
    end;

    with RegisterDriverProperty('Event2')^ do
    begin
     Caption := rsEventOn;
      ValueType := varString;
      DefaultValue := rsEventOnValue;
      EditType := 'pkText';
      Hint := '';
//      hidden := true;
    end;

  end else
  if PDBTableGateway(Gateway).RawTableType = Integer(dtAM4T)  then
  begin

    with RegisterDriverProperty('{9F0D0F18-F1C3-4A9E-AC9A-4BBB8E43D23A}')^ do
      hidden := true;

    with RegisterDriverProperty('MinTreshold')^ do
    begin
      Caption := rsMinTreshold;
      ValueType := varInteger;
      DefaultValue := 0;
      EditType := 'pkText';
      Hint := '';
      hidden := true;
    end;

    with RegisterDriverProperty('MaxTreshold')^ do
    begin
      Caption := rsMaxTreshold;
      ValueType := varInteger;
      DefaultValue := 0;
      EditType := 'pkText';
      Hint := '';
      hidden := true;
    end;

    with RegisterDriverProperty('MaxValue')^ do
    begin
      Caption := rsMaxValue;
      ValueType := varInteger;
      DefaultValue := 0;
      EditType := 'pkText';
      Hint := '';
      hidden := true;
    end;

    with RegisterDriverProperty('MaxDispValue')^ do
    begin
      Caption := rsMinDispValue;
      ValueType := varInteger;
      DefaultValue := 0;
      EditType := 'pkText';
      Hint := '';
      hidden := true;
    end;

    with RegisterDriverProperty('MinDispValue')^ do
    begin
      Caption := rsMinDispValue;
      ValueType := varInteger;
      DefaultValue := 0;
      EditType := 'pkText';
      Hint := '';
      hidden := true;
    end;

    with RegisterDriverProperty('Offset')^ do
    begin
      Caption := rsOffset;
      ValueType := varInteger;
      DefaultValue := 0;
      EditType := 'pkText';
      Hint := '';
      hidden := true;
    end;

    with RegisterDriverProperty('PPUVersion')^ do
    begin
      Caption := rsVersion;
      ValueType := varInteger;
      DefaultValue := 0;
      EditType := 'pkText';
      Hint := '';
      hidden := true;
    end;

    with RegisterDriverProperty('Precision')^ do
    begin
      Caption := 'Точность';
      ValueType := varBoolean;
      DefaultValue := False;
      EditType := 'pkBoolean';
      hidden := true;
    end;

  end else
  if PDBTableGateway(Gateway).RawTableType = Integer(dtBUNS)  then
  begin
    Options := Options + [optUseParentAddressSystem];
    SetCaseCount(0);
  end else
  if PDBTableGateway(Gateway).RawTableType = Integer(dtMROrev2)  then
  begin
    Options := Options + [optUseParentAddressSystem];
  end;

  if PDBTableGateway(Gateway).RawTableType >= 1024 then
  begin
    with RegisterDriverProperty('{9915702F-7B3A-43A1-99F6-EBB1EA239923}')^ do
      hidden := true;

    with RegisterDriverProperty('Manual')^ do
    begin
      Caption := rsManualRuling;
      ValueType := varBoolean;
      EditType := 'pkBoolean';
      DefaultValue := False;
      hidden := true;
    end;

    with RegisterDriverProperty('Time')^ do
    begin
      Caption := rsTime;
      ValueType := varInteger;
      DefaultValue := 0;
      EditType := 'pkText';
      Hint := '';
      hidden := true;
    end;

//    Options := Options - [optIgnorable];
  end;


  with PDBTableGateway(Gateway)^ do
    if (DBRecordType = rtOutputDevice) or IsOutDevice or IsOutDevice2 then
    begin

      with RegisterDriverProperty('AllowControl')^ do
      begin
        Caption := rsAllowControl;
        ValueType := varBoolean;
        DefaultValue := False;
        EditType := 'pkBoolean';
        Hint := rsAllowControlDesc;
      end;

      with RegisterDriverProperty('Timeout')^ do
      begin
        Caption := rsParamAsptTimeout;
        ValueType := varInteger;
        DefaultValue := 0;
        EditType := 'pkText';
        Hint := rsParamAsptTimeoutHint;
        hidden := false;
      end;

    end;

  if PDBTableGateway(Gateway).RawTableType = Integer(dtAspt) then
    with RegisterDriverProperty('RunDelay')^ do
    begin
      Caption := rsParamAsptTimeout;
      ValueType := varInteger;
      DefaultValue := 0;
      EditType := 'pkText';
      Hint := '';
      hidden := true;
    end;

  {$IFDEF RubezhNewDBExtLogic}
  with PDBTableGateway(Gateway)^ do
    if (RawTableType <> Integer(dtValve))
    and (RawTableType <> Integer(dtAspt))
    and (RawTableType <> Integer(dtPPU))
    and ((DBRecordType = rtOutputDevice)
      or IsOutDevice or IsOutDevice2) then
    begin
      MaxZoneCardinality := 0;
      MinZoneCardinality := 0;
      Options := Options + [optExtendedZoneLogic];
      if not HasDeviceProperty(Self, 'ExtendedZoneLogic') then
        with RegisterDriverProperty('ExtendedZoneLogic')^ do
        begin
          Caption := rsParamExtLogic;
          ValueType := varString;
          DefaultValue := 0;
          EditType := 'pkText';
          Hint := '';
          Hidden := True;
        end;
    end else
    if RawTableType = Integer(dtValve) then
    begin
      MaxZoneCardinality := 0;
      MinZoneCardinality := 0;
    end;

  {$ENDIF}

  SetOptions(Options);
  SetMaxZoneCardinality(MaxZoneCardinality);
  SetMinZoneCardinality(MinZoneCardinality);
  SetAddressMask(AddressMask);
  SetAddressGroup(AddressGroup);

  SetAddressIncludesParent(True);

  SetDriverShortName(ShortName);
  SetReservedAddresses(ReservedAddresses);
  SetDeviceDriverIDAlias(DeviceDriverAliasID);
  SetDeviceCategory(DevCategory);

  SetChildDriverID(AChildDeviceDriverID);
  SetChildCount(ADeviceCount);

  if AutoCreateRange <> nil then
    with AutoCreateRange^ do
      SetAutoCreateRange(Enabled, AddrFrom, AddrTo);

  if NoAddress then
    SetAddressRange(True, 0, 0, True);

  RegisterStates;
  RegisterDeviceParams;
  AddPropInfo; // параметры для управления и конфигурации устройством

  {$IFDEF UseSerials}
  if PDBTableGateway(Gateway).DBRecordType = rtDevice then
  begin

    with RegisterDriverProperty('Config$SerialNum')^ do
    begin
      Caption := rsFactoryNumber;
      ValueType := varInteger;
      ReadOnly := true;
      DefaultValue := -1;
      EditType := 'pkText';
      Hint := rsDeviceFactoryNumber;
      RawType := rdtWord;
      Multiplier := 1;
      shiftInMemory := $03;
      shiftInMemory2 := $04;
    end;

    with RegisterDeviceParam('Config$SerialNum')^ do
    begin
      Caption := rsFactoryNumber;
      ValueType := varInteger;
      ReadOnly := true;
      DefaultValue := -1;
      EditType := 'pkText';
      Hint := rsDeviceFactoryNumber;
      TimeScaleLength := 10;
    end;

    with RegisterDriverProperty('Config$SoftVersion')^ do
    begin
      Caption := rsFirmwareVersion;
      ValueType := varInteger;
      DefaultValue := -1;
      ReadOnly := true;
      EditType := 'pkText';
      Hint := rsDeviceFirmwareVersion;
      RawType := rdtWord;
      Multiplier := 1;
      shiftInMemory := $02;
    end;

    with RegisterDeviceParam('Config$SoftVersion')^ do
    begin
      Caption := rsFirmwareVersion;
      ValueType := varInteger;
      DefaultValue := -1;
      ReadOnly := true;
      EditType := 'pkText';
      Hint := rsDeviceFirmwareVersion;
      TimeScaleLength := 10;
    end;

  end;
  {$ENDIF}

  with PDBTableGateway(Gateway)^ do
  begin
    SetDeviceIconName(IconName);
//    SetLimitedParentDriver(DeviceRegistry.GetDeviceDriver(Rubezh3Mod.DriverID));
  end;
end;

function TCustomSimpleDriver.CreateDeviceInstance(
  const Parent: IDeviceInstance; const Config: IDevice; ServerReq: IServerRequesting): IDeviceInstance;
begin
  result := TCustomSimpleDevice.Create(Parent, Config);
end;

const
  TEST_IOCTLFunctions: array[0..1] of TIOCTLFunctionInfo =
    (
      (FunctionCode: 'IO_DeviceType'; FunctionName: 'Тип устройства';
        FunctionDesc: 'Получить тип устройства прямым запросом по 1-wire'),
      (FunctionCode: 'IO_StartExec'; FunctionName: 'Включение ИУ';
        FunctionDesc: 'Включить ИУ запросом к панели')
    );

function TCustomSimpleDriver.IOCTL_GetFunctionCount: integer;
begin
  result := 0 {length(TEST_IOCTLFunctions)}; // Отключено !!!
end;

function TCustomSimpleDriver.IOCTL_GetFunctionInfo(Index: Integer): TIOCTLFunctionInfo;
begin
  result := TEST_IOCTLFunctions[Index];
end;

{ TRubezh3Driver }

function TRubezh3Driver.AutoDetectInstances(const ParentInstance: IDeviceInstance; PNPOnly: Boolean): TDetectResult;

var
  DataTransport: IDataTransport;
  Progress: IInternalProgress;
  i: integer;
  Instance: IDevice;
  ResultDataBlock: IDataBlock;
  Readed, Written: Integer;

  procedure ShowProgress(Address: Integer; ValidateTime: Boolean);
  var SearchDevStr: string;
  begin
    if (Progress <> nil) and (not ValidateTime or ((Now - Progress.GetLastProgressTime) > MIN_NOTIFY_TIME)) then
    begin

      if PNPOnly then
        SearchDevStr := 'PNP-устройств ' else
        SearchDevStr := 'устройств ';
      if FPanelDeviceMod.DeviceSubtype = dsOrdinaryPanel then
        SearchDevStr := SearchDevStr + 'Рубеж' else
        SearchDevStr := SearchDevStr + DriverShortName;

      if not Progress.Progress(-100, Format(rsRubezhSearching,
        [GetDeviceText(ParentInstance.Device, [dtoShortName, dtoParents]), SearchDevStr,
          Address, MAX_EXT_PANEL_ADDRESS]), Round(Address / MAX_EXT_PANEL_ADDRESS * 100), Readed shl 16 or Written, False) then
            Abort;
    end;
  end;

  function FindLocalPanelDriverID(const DeviceType: byte): string;
  var
    i: integer;
  begin
    result := '';
    for i := 0 to High(Local3Mods) do
      if Local3Mods[i].BaseNodeType = DeviceType then
      begin
        result := Local3Mods[i].DriverID;
        break;
      end;
  end;


var
  DeviceType: byte;
  DeviceDriverID: string;
  w: word;
  s: string;
  Intf: TRubezh3Interface;
begin
  result := drNoDevicesFound;

  DataTransport := DeviceGetDataTransport(ParentInstance);
  if DataTransport = nil then
    raise Exception.Create(rsDeviceMustBeConnectedToTransport);
  Supports(ParentInstance, IInternalProgress, Progress);

  if not Supports(DataTransport, IUSBChannel) and PNPOnly then
    exit;

  Readed := 0;
  Written := 0;

  try
    for i := MIN_PANEL_ADDRESS to MAX_EXT_PANEL_ADDRESS do
    begin
      ShowProgress(i, False);
      Intf := TRubezh3Interface.Create(i, DataTransport, FPanelDeviceMod);
      with Intf do
      try
        RetryCount := AUTO_DETECT_RETRY_COUNT;
        try
          while True do
          begin
            case SendPing(ResultDatablock) of
              crError:
                if GetLastErrorCode in [errUSBReportNoDevice, errUSBCannotDeliver] then
                  Abort else
                  break;
              crAnswer:
                break;
              crTimeout, crBadCRC:
                begin
                  RetryCount := RetryCount - 1;
                  if RetryCount <= 0 then
                    abort;
                end;
            end;
            ShowProgress(i, True);
          end;

{          DeviceType := RequestDeviceType;
          if DeviceType = Rubezh10AMod.BaseNodeType then
            DeviceDriverID := Rubezh10AMod.DriverID else
          if DeviceType = Rubezh2AMod.BaseNodeType then
            DeviceDriverID := Rubezh2AMod.DriverID else
            { Нашли устройство неизвестного типа
            break; }

        SoftUpdateMode := true;
        try
          DeviceType := GetDeviceType;
          if DeviceType = $FF then
          begin
            DeviceDriverID := AcquireDriverIDByDatabaseSignature;
            if DeviceDriverID = '' then
              Continue;
          end else
          begin
            DeviceDriverID := FindLocalPanelDriverID(DeviceType);
            if DeviceDriverID = '' then
              Continue;
          end;
        finally
          SoftUpdateMode := false;
        end;

{          MemRead(GetDatabaseStart,Sizeof(FHeader.Head), PChar(@FHeader.Head));
          if CompareMem(@FHeader.Head.Signature, @Rubezh3Mod.Signature, Sizeof(TDeviceSignature)) then
            DeviceDriverID := Rubezh3Mod.DriverID else
          if CompareMem(@FHeader.Head.Signature, @Buns3Mod.Signature, Sizeof(TDeviceSignature)) then
            DeviceDriverID := Buns3Mod.DriverID else
            Continue; }

          Instance := CreateNewDevice(GetDeviceRegistry.GetDeviceDriver(DeviceDriverID), ParentInstance.Device, GetRootDevice(ParentInstance.Device).DeviceConfig);
          try
            w := GetFirmwareVersion;
            case DeviceType of
              1,2,4,5,6,8: s := FModel.CreateDatabase_Device(Intf).GetSerialNo;
              3,7,100,101,102: s := GetSerialNo; // мониторинговые станции
              else s := '';
            end;

            // Устанавливаем версию ПО и заводской номер
            SetDeviceProp(Instance, 'SerialNo', rsFoundFactoryNo + s);
//            SetDeviceProp(Instance, 'SerialNo', rsFoundFactoryNo + FModel.CreateDatabase_Device(Intf).GetSerialNo);
            SetDeviceProp(Instance, 'SoftVersion', rsFoundFirmwareVersion + IntToStr(WordRec(w).hi) + '.' + IntToStr(WordRec(w).lo));
          except
            // Если здесь происходит ошибка, то устройство в конфе появляется, а
            // его адрес неверный
          end;
          Instance.DeviceAddress := i;
          result := drDevicesFound;
        except
        end;
        Readed := BytesReaded;
        Written := BytesWritten;
      finally
        free;
      end;
    end;
  except
    On E: EAbort do
      if result = drDevicesFound then
        result := drDevicesFoundButAbort else
        raise;
    else
      raise;
  end;
end;

constructor TRubezh3Driver.Create(const DeviceRegistry: IDeviceRegistry; PanelDeviceMod: PPanelDeviceMod;
  DeviceClass: TRubezh3DeviceClass);

  procedure RegisterMaxRecCounts;
  var
    i: integer;
    RootProp: PPropertyTypeInfo;
    RootList: TPropertyList;
  begin
    RootProp := nil;
    RootList := nil;

    with GetDBTableGatewayList3 do
    begin
      for i := 0 to Count - 1 do
        if Items[i].DBRecordType <> rtOutputDevice then
        begin
          if RootProp = nil then
          begin
            RootList := TPropertyList.Create;
            RootProp := RegisterDriverProperty('MaxRecCounts');
            with RootProp^ do
            begin
              Caption := rsParamMaxRecCount;
              ValueType := varEmpty;
              EditType := 'pkFolder';
              Hint := rsParamMaxRecCountHint;
              NestedProperties := TPropertyEnumerator.Create(TObjectOwner.Create(RootList, ioOwned));
            end;
          end;

          with RootList.NewProperty('Count'+IntToStr(Items[i].RawTableType))^ do
          begin
            Caption := Items[i].DeviceName;
            ValueType := varWord;
            EditType := 'pkText';
            DefaultValue := 0;
          end;
        end;
    end;
  end;

  procedure RegisterStates;
  var
    i: Integer;
  begin
    { Поскольку драйверов на данный момент несколько, то у них должен быть один интерфейс состояния,
      поэтому создавать интерфейс нужно только однажды }
    for i := 0 to High(arPanelStates) do
      if (arPanelStates[i].Intf <> nil) and (arPanelStates[i].Name <> '') then
        RegisterDeviceStateIntf(arPanelStates[i].Intf) else
          arPanelStates[i].Intf := RegisterDeviceState(i, arPanelStates[i].Name, arPanelStates[i].StateCode,
            arPanelStates[i].NameSourceParam, arPanelStates[i].StateClass, False, arPanelStates[i].SubSystem, arPanelStates[i].Options, arPanelStates[i].ManualReset);

    RegisterDeviceState(psLostConnection, rsPanelNoAnswer, 'ParentConnectionLost', '', dscError, True);
    RegisterDeviceState(psSyncronizationFailed, rsErrorSyncFailed, 'SynchroNotCorrect', '', dscUnknown, True);
    RegisterDeviceState(psIntError, rsIntErrorState, 'DetectDeviceError', '', dscUnknown, False);
    RegisterDeviceState(psStateReset, rsStateReset, 'ResetState', '', dscNull, True);
    RegisterDeviceState(psReservedLine1Failed, rsReserved1Failed, 'Reserved1Failed', '', dscError, False);
    RegisterDeviceState(psReservedLine2Failed, rsReserved2Failed, 'Reserved1Failed', '', dscError, False);

    // Слишком просто
    for i := 1 to 10 do
      RegisterDeviceState(psReservedLine2Failed + i, rsALS + ' ' +IntToStr(i) + ' ' + rsALSMalfunction, 'ALS'+IntToStr(i)+'Fail' , '', dscError);

    RegisterDeviceState(psBIConnectionWithPanelIsLost, rsConnectionWithPanelIsLost, 'ConnectWithPanelIsLost', '', dscServiceRequired, False);
    RegisterDeviceState(psBIDifferenceDBwithPanel, rsDifferenceDBWithPanel, 'DifferenceDBWithPanel', '', dscServiceRequired, False);

    RegisterDeviceState(psRCConnectionWithPanelIsLost, rsConnectionWithPanelIsLost, 'RCConnectWithPanelIsLost', '', dscServiceRequired, False, ssFire);
    RegisterDeviceState(psRCDifferenceDBwithPanel, rsDifferenceDBWithPanel, 'RCDifferenceDBWithPanel', '', dscServiceRequired, False, ssFire);
    RegisterDeviceState(psRCKeyBoardUnlock, rsRCKeyBoardUnlock, 'RCKeyBoardUnlock', '', dscNormalDefault, False, ssNone);
    RegisterDeviceState(psRCKeyBoardlock, rsRCKeyBoardlock, 'RCKeyBoardlock', '', dscNormal, False, ssNone);
    RegisterDeviceState(psRCPower1Fail, rsRCPower1Fail, 'RCPower1Fail', '', dscError, False, ssNone);
    RegisterDeviceState(psRCPower2Fail, rsRCPower2Fail, 'RCPower2Fail', '', dscError, False, ssNone);
    // МДС
    RegisterDeviceState(psMDSLineFail, rsMDSLineFail, 'MDSLineFail', '', dscServiceRequired, False, ssFire);
    RegisterDeviceState(psMDSDeliverFail, rsMDSDeliverFail, 'MDSDeliverFail', '', dscServiceRequired, False, ssFire);
    RegisterDeviceState(psMDSBufferOverflow, rsMDSBufferOverflow, 'MDSBufferOverflow', '', dscServiceRequired, False, ssFire);
  end;

var
  AnOptions: TDriverOptions;
  i: integer;
begin

  FDeviceClass := DeviceClass;
  FPanelDeviceMod := PanelDeviceMod;
  inherited Create(FPanelDeviceMod.DeviceName, FPanelDeviceMod.ClassID,
    FPanelDeviceMod.DriverID, DeviceRegistry);

  case FPanelDeviceMod.DeviceSubtype of
    dsOrdinaryPanel:
      begin
        AnOptions := [optEventSource, optDeviceDatabaseRead, optDeviceDatabaseWrite,
          optHasTimer, optSoftUpdates, optSoftUpdateVerifyVersions, optPasswordManagement,
          optDescriptionString, optDeviceAsHardwareKey, optPlaceable];

        if (FPanelDeviceMod.BaseNodeType = 1) then
          include(AnOptions, optCustomIOCTLFunctions);

        SetAddressRange(True, MIN_PANEL_ADDRESS, MAX_PANEL_ADDRESS);
      end;
    dsMDS:
      begin
        AnOptions := [optEventSource, optDeviceDatabaseWrite, optSoftUpdates, optSoftUpdateVerifyVersions,
          optDescriptionString, optNotValidateZoneAndChildren, optDeviceAsHardwareKey];
        SetAddressRange(True, 124, 125);
      end;
    dsMDS5:
      begin
        AnOptions := [optEventSource, optDeviceDatabaseWrite, optSoftUpdates, optSoftUpdateVerifyVersions,
          optDescriptionString, optSoftUpdates, optNotValidateZoneAndChildren, optDeviceAsHardwareKey,
          optSecondary];
        SetAddressRange(True, 124, 125);
      end;
    dsIndicator{$IFDEF RubezhRemoteControl}, dsRemoteControl, dsRemoteControlFire {$ENDIF}:
      begin
        AnOptions := [optEventSource, optDeviceDatabaseWrite, optSoftUpdates, optSoftUpdateVerifyVersions,
          optDescriptionString, optNotValidateZoneAndChildren, optDeviceAsHardwareKey,
          optSecondary, optCustomIOCTLFunctions];
        SetAddressRange(True, MIN_PANEL_ADDRESS, MAX_PANEL_ADDRESS);
      end;
  end;

  case FPanelDeviceMod.DatabaseType of
    dbtNone: FModel := nil;
    dbtClassic: FModel := GetClassicDatabaseModel(FPanelDeviceMod);
    dbtModern:
      begin
        FModel := GetModernDatabaseModel(FPanelDeviceMod);
{$IFNDEF READ_NEW_DB}
        AnOptions := AnOptions - [optDeviceDatabaseRead];
{$ENDIF}
      end;
  end;

  case FPanelDeviceMod.SubSystem of
    ssFire: AnOptions := AnOptions + [optFireOnly];
    ssSecurity: AnOptions := AnOptions + [optSecOnly];
    ssTech: AnOptions := AnOptions + [optTechOnly];
  end;

  if not FPanelDeviceMod.HasOutDevices then
    AnOptions := AnOptions + [optDisableAutoCreateChildren];
  if FPanelDeviceMod.AutoDetectable then
    AnOptions := AnOptions + [optCanAutoDetectInstances];

  SetOptions(AnOptions);
  SetMaxZoneCardinality(0);
  SetValidChars(GetValidChars);
  SetDriverShortName(FPanelDeviceMod.DeviceShortName);
  SetChildAddressMask(FPanelDeviceMod.ChildAddressMask);
  SetAddressIncludesParent(not IsBAES);
  SetAddressGroup(1);
  SetDeviceCategory(dcPanel);
  SetAlternateInterfaces(rsAltUSBInterface);

  if PanelDeviceMod.IconName <> '' then
    SetDeviceIconName(PanelDeviceMod.IconName);

(*
  {$IFDEF HexFileDatabase}
  with RegisterDriverProperty('TestFileName')^ do
  begin
    Caption := '.Hex file name';
    ValueType := varString ;
    DefaultValue := 'c:\base.hex';
    EditType := 'pkText';
    Hint := 'Server .Hex file name';
  end;

  with RegisterDriverProperty('TestStartAddr')^ do
  begin
    Caption := 'Database start';
    ValueType := varString;
    DefaultValue := 0;
    EditType := 'pkText';
    Hint := 'Database start address (for HEX values, use "$" prefix)';
  end;
  {$ENDIF}
*)

  with RegisterDeviceParam('DatabaseState')^ do
  begin
    Caption := rsDatabaseState;
    ValueType := varInteger;
    DefaultValue := 0;
    EditType := 'pkText';
    if not IsRubezh3Debug then
      Hidden := True;
    ShowOnlyInState := True;
  end;

  with RegisterDeviceParam('IntErrorState')^ do
  begin
    Caption := rsIntErrorState;
    ValueType := varInteger;
    DefaultValue := 0;
    EditType := 'pkText';
    if not IsRubezh3Debug then
      Hidden := True;
    ShowOnlyInState := True;
  end;

  with RegisterDeviceParam('ReservedLineState_1')^ do
  begin
    Caption := rsReservedChannelState + '1';
    ValueType := varInteger;
    DefaultValue := 0;
    EditType := 'pkText';
    if not IsRubezh3Debug then
      Hidden := True;
    ShowOnlyInState := True;
  end;

  with RegisterDeviceParam('ReservedLineState_2')^ do
  begin
    Caption := rsReservedChannelState + '2';
    ValueType := varInteger;
    DefaultValue := 0;
    EditType := 'pkText';
    if not IsRubezh3Debug then
      Hidden := True;
    ShowOnlyInState := True;
  end;

  if IsRubezh3Debug then
  begin
    with RegisterDeviceParam('Debug')^ do
    begin
      Caption := rsDebug;
      ValueType := varString;
      EditType := 'pkText';
      Hint := rsDebug;
      ShowOnlyInState := True;
    end;

    with RegisterDeviceParam('FailureType')^ do
    begin
      Caption := rsMalfunction;
      ValueType := varString;
      EditType := 'pkText';
      Hint := rsMalfunctionType;
      ShowOnlyInState := True;
    end;

    with RegisterDeviceParam('DBHash')^ do
    begin
      Caption := 'DBHash';
      ValueType := varString;
      EditType := 'pkText';
      Hint := 'DBHash';
      ShowOnlyInState := True;
    end;

    with RegisterDeviceParam('DBHashR')^ do
    begin
      Caption := 'Hash in panel ';
      ValueType := varString;
      EditType := 'pkText';
      Hint := 'DBHash';
      ShowOnlyInState := True;
    end;

{    with RegisterDeviceParam('AddrList')^ do
    begin
      Caption := 'AddrList';
      ValueType := varString;
      EditType := 'pkText';
      Hint := 'AddrList';
      ShowOnlyInState := True;
    end; }

  end;

{$IFNDEF DISABLE_SECURITY}
  // 28.02.2011 Решили отказаться от этого свойства,
  //            проверка будет вестись только по общему количеству охр. устройств
  // 03.10.2011 Вернулись, Устройства будут располагатся с 176 адреса на каждой АЛС
  if (PanelDeviceMod.ClassID = sdcDeviceSecPanel)
  or (PanelDeviceMod.ClassID = sdcDeviceUSBSecPanel)  then
  begin

    with RegisterDriverProperty('DeviceCountSecDev')^ do
    begin
      Caption := rsAssignSecDevices;
      ValueType := varInteger;
      DefaultValue := 0;
      EditType := 'pkTextList';
      Hint := '';
      ListItems := TParamsImpl.Create;
      with ListItems do
      begin
        NewParam('64 - 00', 0);
        NewParam('48 - 16', 1);
        NewParam('32 - 32', 2);
        NewParam('16 - 48', 3);
        NewParam('00 - 64', 4);
      end;
    end;

  end;
{$ENDIF}

  if (PanelDeviceMod.DeviceSubtype = dsOrdinaryPanel) then
  begin

    // Числа устройств
    with RegisterDeviceParam('DeviceTotalCount')^ do
    begin
      Caption := rsDeviceTotalCount;
      ValueType := varInteger;
      ShowOnlyInState := True;
    end;

    with RegisterDeviceParam('DeviceLostCount')^ do
    begin
      Caption := rsDeviceLostCount;
      ValueType := varInteger;
      ShowOnlyInState := True;
    end;

    with RegisterDeviceParam('DeviceSmokeCount')^ do
    begin
      Caption := rsDeviceSmokeCount;
      ValueType := varInteger;
      ShowOnlyInState := True;
    end;

    with RegisterDeviceParam('DeviceFailureCount')^ do
    begin
      Caption := rsDeviceFailedCount;
      ValueType := varInteger;
      ShowOnlyInState := True;
    end;

    with RegisterDeviceParam('DeviceIgnoreCount')^ do
    begin
      Caption := rsDeviceIgnoredCount;
      ValueType := varInteger;
      ShowOnlyInState := True;
    end;

    with RegisterDeviceParam('DeviceNonUsedCount')^ do
    begin
      Caption := rsDeviceNonUsedCount;
      ValueType := varInteger;
      ShowOnlyInState := True;
    end;

    with RegisterDeviceParam('DeviceExternalCount')^ do
    begin
      Caption := rsDeviceExternalCount;
      ValueType := varInteger;
      ShowOnlyInState := True;
    end;

    {$IFDEF LoopLines}
    if  not (PanelDeviceMod.BaseNodeType in [1, 2]) then // выбрасываем БУНС и 2АМ then then
      for i := 0 to FPanelDeviceMod.AVRCount - 1 do
        with RegisterDriverProperty('LoopLine' + IntToStr(i + 1))^ do
        begin
          Caption := 'АЛС ' + IntToStr(i * 2 + 1) + ',' + IntToStr(i * 2 + 2) + ' - кольцевая АЛС';
          ValueType := varBoolean;
          DefaultValue := false;
          EditType := 'pkBoolean';
          Hint := '';
          hidden := false;
        end;

    {$ENDIF}

  end else

  if PanelDeviceMod.DeviceSubtype = dsMDS  then
  begin
    with RegisterDriverProperty('{63FCFDCD-111F-47D5-8E4E-C0974792A9C1}')^ do
      hidden := true;

    with RegisterDriverProperty('EventsFilter')^ do
    begin
      Caption := rsMDSEventsFilter;
      ValueType := varString;
      DefaultValue := 0;
      EditType := 'pkText';
      Hint := '';
      hidden := true;
    end;

    with RegisterDeviceParam('CountNotSendMsg')^ do
    begin
      Caption := rsMDSCountNotSendMsg;
      ValueType := varInteger;
      ShowOnlyInState := True;
    end;

  end else


  if PanelDeviceMod.DeviceSubtype = dsMDS5  then
  begin

    with RegisterDriverProperty('{63FCFDCD-111F-47D5-8E4E-C0974792A9C1}')^ do
      hidden := true;

    with RegisterDriverProperty('Phone1')^ do
    begin
      Caption := rsMDS5Phone1;
      ValueType := varString;
      DefaultValue := 0;
      EditType := 'pkText';
      Hint := '';
      hidden := true;
    end;

    with RegisterDriverProperty('Phone2')^ do
    begin
      Caption := rsMDS5Phone2;
      ValueType := varString;
      DefaultValue := 0;
      EditType := 'pkText';
      Hint := '';
      hidden := true;
    end;

    with RegisterDriverProperty('Phone3')^ do
    begin
      Caption := rsMDS5Phone3;
      ValueType := varString;
      DefaultValue := 0;
      EditType := 'pkText';
      Hint := '';
      hidden := true;
    end;

    with RegisterDriverProperty('Phone4')^ do
    begin
      Caption := rsMDS5Phone4;
      ValueType := varString;
      DefaultValue := 0;
      EditType := 'pkText';
      Hint := '';
      hidden := true;
    end;

    with RegisterDriverProperty('ObjectNumber')^ do
    begin
      Caption := rsMDS5ObjectNumber;
      ValueType := varString;
      DefaultValue := 0;
      EditType := 'pkText';
      Hint := '';
      hidden := true;
    end;

    with RegisterDriverProperty('TestDialtone')^ do
    begin
      Caption := rsMDS5TestDialtone;
      ValueType := varInteger;
      DefaultValue := 0;
      EditType := 'pkText';
      Hint := '';
      hidden := true;
    end;

    with RegisterDriverProperty('TestVoltage')^ do
    begin
      Caption := rsMDS5TestVoltage;
      ValueType := varInteger;
      DefaultValue := 0;
      EditType := 'pkText';
      Hint := '';
      hidden := true;
    end;

    with RegisterDriverProperty('CountRecalls')^ do
    begin
      Caption := rsMDS5CountRecalls;
      ValueType := varInteger;
      DefaultValue := 0;
      EditType := 'pkText';
      Hint := '';
      hidden := true;
    end;

    with RegisterDriverProperty('Timeout')^ do
    begin
      Caption := rsMDS5Timeout;
      ValueType := varInteger;
      DefaultValue := 0;
      EditType := 'pkText';
      Hint := '';
      hidden := true;
    end;

    with RegisterDriverProperty('OutcomingTest')^ do
    begin
      Caption := rsMDS5OutcomingTest;
      ValueType := varInteger;
      DefaultValue := 0;
      EditType := 'pkText';
      Hint := '';
      hidden := true;
    end;

    with RegisterDriverProperty('EventsFilter')^ do
    begin
      Caption := rsMDSEventsFilter;
      ValueType := varString;
      DefaultValue := 0;
      EditType := 'pkText';
      Hint := '';
      hidden := true;
    end;

    with RegisterDeviceParam('CountNotSendMsg')^ do
    begin
      Caption := rsMDSCountNotSendMsg;
      ValueType := varInteger;
      ShowOnlyInState := True;
    end;

  end;

  if (PanelDeviceMod.BaseNodeType in [2,8]) then
  begin
    with RegisterDriverProperty('StartDelay')^ do
    begin
      Caption := rsBunsStartTimeout;
      ValueType := varByte;
      EditType := 'pkText';
      Hint := rsBunsStartTimeout + ' 0 - 60 ' +  rsSecondsShort;
      DefaultValue := 0;
    end;

    with RegisterDriverProperty('DistTime')^ do
    begin
      Caption := rsBunsDistTime;
      ValueType := varInteger;
      EditType := 'pkText';
      Hint := rsBunsDistTime + ' 10 - 600 ' + rsMinutesShort;
      DefaultValue := 100;
    end;

    with RegisterDriverProperty('PumpTime')^ do
    begin
      Caption := rsBunsPumpTime;
      ValueType := varByte;
      EditType := 'pkText';
      Hint := rsBunsPumpTimeHint + ' 0 - 10 ' + rsSecondsShort;
      DefaultValue := 0;
    end;

    with RegisterDriverProperty('PumpCount')^ do
    begin
      Caption := rsBunsPumpCount;
      ValueType := varByte;
      EditType := 'pkText';
      Hint := rsBunsPumpCountHint;
      DefaultValue := 1;
    end;

  end;

{  with RegisterDriverProperty('DeviceChanged')^ do
  begin
    Caption := 'Устройство заменено';
    ValueType := varBoolean;
    EditType := 'pkBoolean';
    DefaultValue := False;
    Hint := 'Указывает на то что произошла замена прибора и необходимо считать записи из журнала. Сбрасывается автоматически.';
  end; }

{  with RegisterDriverProperty('RetryCount')^ do
  begin
    Caption := 'Число повторов';
    ValueType := varInteger;
    DefaultValue := DEFAULT_RETRY_COUNT;
    EditType := 'pkReadOnlyText';
    Hint := 'Число повторных запросов к устройству';
  end; }

(*  with RegisterDriverProperty('HasTimer')^ do
  begin
    Caption := 'Таймер работает';
    ValueType := varBoolean;
    EditType := 'pkBoolean';
    DefaultValue := True;
    Hint := 'Пока не работает таймер в устройстве к времени добавляются миллисекунды';
  end; *)

  with RegisterDriverProperty('SetDateTime')^ do
  begin
    Caption := 'Устанавливать время';
    ValueType := varBoolean;
    EditType := 'pkBoolean';
    DefaultValue := True;
    Hint := 'При начале работы с прибором устанавливать такие дату и время, как системные';
  end;

{  with RegisterDriverProperty('AllRecords')^ do
  begin
    Caption := 'Все записи журнала';
    ValueType := varBoolean;
    EditType := 'pkBoolean';
    DefaultValue := True;
    Hint := 'Считывать все записи хранящиеся в журнале событий панели. Иначе записи будут считаны с момента последнего измененения конфигурации панели';
  end; }

  {$IFDEF UsePanelMaxRecs}
  RegisterMaxRecCounts;
  {$ENDIF}
  RegisterStates;
end;

function TRubezh3Driver.CreateDeviceInstance(const Parent: IDeviceInstance;
  const Config: IDevice; ServerReq: IServerRequesting): IDeviceInstance;
var
  SetDataTransport: ISetDatatransport;
begin
  FServerReq := ServerReq;
  if USBInterfaceSelected(Config) then
  begin
    result := FDeviceClass.Create(Parent, Config, FPanelDeviceMod, FServerReq);
    if Supports(result, ISetDatatransport, SetDataTransport) then
    begin
      SetDataTransport.SetOnInitTransportInstance(InitUSBTransport);
      SetDataTransport.SetOnFInitTransportInstance(FInitUSBTransport);
    end;
  end else
    result := FDeviceClass.Create(Parent, Config, FPanelDeviceMod, FServerReq);
end;

function TRubezh3Driver.CreateUSBParentInstance(const Sender: IInterface): IDeviceInstance;
var
  Driver: IDeviceDriver;
  Device: IDevice;
  ServerDriver: IServerDeviceDriver;
begin
  Driver := GetDeviceRegistry.GetDeviceDriver(sddMs01); // МС-01
  if not Supports(Driver, IServerDeviceDriver, ServerDriver) then
    raise Exception.Create(rsUnableCreateInternalDevice);

  Device := TDevice.Create(Driver);
  if Supports(Sender, IDeviceInstance, result) then
    SetDeviceProp(Device, 'SerialNo', GetDevicePropDef(result.Device, 'SerialNo', ''));

  //  Изменение фиксированного канала для Дозвонки на 0x01
  case FPanelDeviceMod.DeviceSubtype of
    dsMDS: result := TRubezhIntenalUSB.Create(nil, Device, FPanelDeviceMod.USBDescription, 1);
    dsMDS5: result := TRubezhIntenalUSB.Create(nil, Device, FPanelDeviceMod.USBDescription, 1);
    else result := TRubezhIntenalUSB.Create(nil, Device, FPanelDeviceMod.USBDescription, 2);
  end;

end;

function TRubezh3Driver.DecodeEventAttributes(Buffer: Pointer;
  Size: Integer; out EventReasonInfo: TEventReasonInfo): TString;
begin
  result := DoDecodeEventAttributes(FModel, GetDeviceRegistry,
    Buffer, Size, EventReasonInfo);
end;

function TRubezh3Driver.doGetCurrentSignature: TDeviceSignature;
begin
  result := FPanelDeviceMod.Signature;
end;

procedure TRubezh3Driver.FInitUSBTransport(const Sender: ISetDatatransport);
var
  DataTransport: IDataTransport;
  DeviceInstance: IDeviceInstance;
begin
  DataTransport := Sender.GetFixedDataTransport;
  if DataTransport <> nil then
  begin
    try
      if Supports(DataTransport, IDeviceInstance, DeviceInstance) then
        DeviceInstance.Finalize;
    except

    end;

    Sender.SetDataTransport(nil);
  end;
end;

function TRubezh3Driver.GetDatabaseModelExt: IRubezh3DatabaseModelExt;
begin
  result := FModel;
end;

function TRubezh3Driver.GetInternalDeviceType: integer;
begin
  result := FPanelDeviceMod.BaseNodeType
end;

function TRubezh3Driver.GetLineCount: integer;
begin
  result := FPanelDeviceMod.AVRCount * 2;
end;

function TRubezh3Driver.HasCrossLinks(const Device: IDevice): boolean;

  var
    ExtZoneMap: IInterfaceAssociations;

  procedure ProcessExtendedZoneLogic(const Device: IDevice);
  var
    ChildDevice: IDevice;
    s: string;
    Node, ChildNode: TXBNode;
    Base: TDOMBase;
    i: integer;
  begin
    s := GetDevicePropDef(Device, 'ExtendedZoneLogic', '');
    if (s <> '') and (s <> '0') then
    begin

      if ExtZoneMap = nil then
        ExtZoneMap := TInterfaceDictionary.Create;

      Base := TDOMBase.Create;
      try
        try
          if s <> '' then
            Base.Data := s;
          Node := GetDocumentNode(Base.Document);

          for i := 0 to GetChildNodeCount(Node) - 1 do
          begin
            ChildNode := GetChildNode(Node, i);
            if (GetNodeType(ChildNode) = xntElement_Node) and (GetNodeName(ChildNode) = 'clause') then
              XMLReadZoneList2(ExtZoneMap, ChildNode, Device);
          end;
        except
          //
        end;
      finally
        Base.Free;
      end;
    end;

    with Device.EnumChildren do
      while Next(ChildDevice) = S_OK do
        ProcessExtendedZoneLogic(ChildDevice); 
  end;

  function ZoneEnumAll(const Zone: IZone): IEnumDevice;
  var
    List, List2: IInterfaceList;
    CD: IDevice;
    i: integer;
  begin
    if ExtZoneMap <> nil then
    begin
      List := TInterfaceList.Create;

      with Zone.EnumDevices(nil) do
        while Next(CD) = S_OK do
          List.Add(CD);

      List2 := ExtZoneMap.FindAssociation(Pointer(Zone.ZoneID)) as IInterfaceList;
      if List2 <> nil then
        for I := 0 to List2.Count - 1 do
          List.Add(List2[i]);

      result := TDeviceEnumerator.Create(List);
    end else
      result := Zone.EnumDevices(nil);
  end;

(*  function DeviceEnumAllZones(const Device: IDevice): IEnumZones;
  var
    ZoneList: IZonesList;
    Zone: IZone;
    RootZoneList: IZonesList;
    Node, ChildNode: TXBNode;
    Base: TDOMBase;
    s: string;
    i: integer;
  begin
    s := GetDevicePropDef(Device, 'ExtendedZoneLogic', '');
    if (s <> '') and (s <> '0') then
    begin
      ZoneList := TZonesList.Create;
      with Device.EnumZones do
        while Next(Zone) = S_OK do
          ZoneList.Add(Zone);

      RootZoneList := GetDeviceConfigFromDevice(Device).GetZoneList;

      Base := TDOMBase.Create;
      try
        try
          Base.Data := s;
          Node := GetDocumentNode(Base.Document);

          for i := 0 to GetChildNodeCount(Node) - 1 do
          begin
            ChildNode := GetChildNode(Node, i);
            if (GetNodeType(ChildNode) = xntElement_Node) and (GetNodeName(ChildNode) = 'clause') then
              ReadZoneList(ChildNode, RootZoneList, ZoneList);
          end;
        except
          //
        end;

        result := TZonesEnumerator.Create(ZoneList);
      finally
        Base.Free;
      end;

    end else
      result := Device.EnumZones;
  end; *)

var
  ChildDevice, OtherDevice: IDevice;
  Zone: IZone;
begin
  // Активными являются те устройства, которые:
  // 1. Имеют свои ИУ в зонах с чужими не ИУ
  // 2. Имеют свои датчики в зонах с чужими ИУ

  // 18.12.2008 По разговору с Николаем - только те приборы которые имеют датчики - активны.
  // 22.12.2008 По решению В.Ю. все остается как раньше.

  // 11.03.2008 РМ тоже могут иметь внешние зоны. А из-за отсутствия прямой связи придется
  // произвести временную конвертацию.
  // Для этого нужен список соответствия зоны и устройств РМ

  if FPanelDeviceMod.DeviceSubtype <> dsOrdinaryPanel then
  begin
    result := true;
    exit;
  end;


  result := false;

  if (Device.ParentDevice <> nil) and (optAllowCrossDevice in Device.ParentDevice.DeviceDriver.DriverOptions) then
  begin

    ExtZoneMap := nil;

    ProcessExtendedZoneLogic(GetRootDevice(Device));

    // Перебираем свои устройства
    with Device.EnumChildren do
      while Next(ChildDevice) = S_OK do
        if not IsDeviceOut(ChildDevice) then
        begin
          // Это наше "не ИУ"
          with ChildDevice.EnumZones do
            while Next(Zone) = S_OK do
              with ZoneEnumAll(Zone) {Zone.EnumDevices(nil)}   do
                while Next(OtherDevice) = S_OK do
                if (OtherDevice <> ChildDevice) and (GetOwnerPanel(OtherDevice.ParentDevice) <> GetOwnerPanel(ChildDevice.ParentDevice)) and
                    IsDeviceOut(OtherDevice) then
                begin
                  result := True;
                  exit;
                end;
        end else
        begin
          // Это наше ИУ
          // здесь опять же нужно перечислить все зоны с учетом РМ
          with DeviceEnumAllZones(ChildDevice) {ChildDevice.EnumZones} do
            while Next(Zone) = S_OK do
              with ZoneEnumAll(Zone) { Zone.EnumDevices(nil) } do
                while Next(OtherDevice) = S_OK do
                if (OtherDevice <> ChildDevice) and (GetOwnerPanel(OtherDevice.ParentDevice) <> GetOwnerPanel(ChildDevice.ParentDevice)) and
                    not IsDeviceOut(OtherDevice) then
                begin
                  result := True;
                  exit;
                end;
        end;
  end;

end;

procedure TRubezh3Driver.InitUSBTransport(const Sender: ISetDatatransport);
var
  DeviceInstance: IDeviceInstance;
  DataTransport: IDataTransport;
begin
  // Создаем экземпляр USB устройства, инициализируем его и присваиваем в панель
  DeviceInstance := CreateUSBParentInstance(Sender);
  DeviceInstance.Initialize(False);

  if not Supports(DeviceInstance, IDataTransport, DataTransport) then
    raise Exception.Create(rsInvalidUSBInterface);

  Sender.SetDataTransport(DataTransport);
end;

const
  Indicator_IOCTLFunctions: array[0..2] of TIOCTLFunctionInfo =
    (
      (FunctionCode: 'Touch_SetMaster'; FunctionName: 'Записать мастер-ключ';
        FunctionDesc: 'Записать мастер-ключ TouchMemory'),
      (FunctionCode: 'Touch_ClearMaster'; FunctionName: 'Стереть пользовательские ключи';
        FunctionDesc: 'Стереть пользовательские ключи TouchMemory'),
      (FunctionCode: 'Touch_ClearAll'; FunctionName: 'Стереть все ключи';
        FunctionDesc: 'Стереть все ключи TouchMemory')
    );

  R2AM_IOCTLFunctions: array[0..1] of TIOCTLFunctionInfo =
    (
      (FunctionCode: 'Set_BlindMode'; FunctionName: 'Установить режим "глухой панели"';
        FunctionDesc: 'Установить режим "глухой панели"'),
      (FunctionCode: 'Reset_BlindMode'; FunctionName: 'Снять режим "глухой панели"';
        FunctionDesc: 'Снять режим "глухой панели"')
    );

function TRubezh3Driver.IOCTL_GetFunctionCount: integer;
begin
  if (FPanelDeviceMod.DeviceSubtype in [dsIndicator{$IFDEF RubezhRemoteControl}, dsRemoteControl, dsRemoteControlFire {$ENDIF}]) then
    result := length(Indicator_IOCTLFunctions) else
  if (FPanelDeviceMod.DeviceSubtype = dsOrdinaryPanel) and (FPanelDeviceMod.BaseNodeType = 1) then
    result := length(R2AM_IOCTLFunctions) else
    result := 0;
end;

function TRubezh3Driver.IOCTL_GetFunctionInfo(Index: Integer): TIOCTLFunctionInfo;
begin
  if (FPanelDeviceMod.DeviceSubtype in [dsIndicator{$IFDEF RubezhRemoteControl}, dsRemoteControl, dsRemoteControlFire {$ENDIF}]) then
    result := Indicator_IOCTLFunctions[Index] else
  if (FPanelDeviceMod.DeviceSubtype = dsOrdinaryPanel) and (FPanelDeviceMod.BaseNodeType = 1) then
    result := R2AM_IOCTLFunctions[Index] else
    Assert(False, '7C9DADED-9970-498D-890B-0F4FF09090B0');
end;

function TRubezh3Driver.USBInterfaceSelected(const Config: IDevice): boolean;
begin
  result := GetDeviceParamDef(Config, 'SYS$Alt_Interface', '') = rsAltUSBInterface;
end;

{ TRubezh3Device }

const
  winLowAscii = [#0..#31]; {characters 0 to 31 are control codes}
  winNonFilename = ['\','/',':','*','?','"','<','>','|']; {reserved characters}
  winIllegalFilename = winLowAscii + winNonFilename; {can't be in a filename}

function MakeSafeFileName(const FileName: string): string;
var
  i: integer;
begin
  Result := FileName;
  for i:=1 to Length(result) do
    if result[i] in winIllegalFilename then
      Result[i] := '_';
end;

function TRubezh3Device.GetDatabase: IRubezh3Database;
var
  Path: string;
begin
  if FDatabase = nil then
  begin
    if GetDatabaseModelExt <> nil then
    begin
      if IsRubezh3FileDB then
      begin
        Path := IncludeTrailingPathDelimiter(ExtractFilePath(ParamStr(0))) + 'TstData';
        ForceDirectories(Path);
        FDatabase := GetDatabaseModelExt.CreateDatabase_File(Path + '\' +
          MakeSafeFileName(Device.DeviceDriver.DriverShortName + ' - ' + FormatDeviceAddr(Device, True, False) + '.bin'));
        FDatabase.InitVirtual;
      end else
      if FNeedCachedDatabase and (Self.Device.DeviceCount > GetPrivateProfileInt('Options', 'R3_CacheCount', 50, PChar(GetIniName)) ) then
      begin
        FDatabase := GetDatabaseModelExt.CreateDatabase_DeviceSnapshot(FInterface);
        if FDatabase = nil then
          FDatabase := GetDatabaseModelExt.CreateDatabase_Device(FInterface);
      end else
        FDatabase := GetDatabaseModelExt.CreateDatabase_Device(FInterface);
    end else
      FDatabase := nil;
  end;
  result := FDatabase;

  if result = nil then
    raise Exception.Create('No database created');
end;

function TRubezh3Device.GetDatabaseModelExt: IRubezh3DatabaseModelExt;
begin
  Supports(FInterface.Model, IRubezh3DatabaseModelExt, result);
end;

function TRubezh3Device.GetDatabaseState: TDatabaseState;
begin
  result := GetDeviceParamDef(Config, 'DatabaseState', 0);
end;

function TRubezh3Device.GetDataTransport: IDataTransport;
begin
  if FDataTransport = nil then
    result := DeviceGetDataTransport(ParentInstance) else
    result := FDataTransport;
end;

function TRubezh3Device.GetSelf(const DeviceConfig: IDeviceConfig): IDevice;
begin
  result := GetDatabase.ReadDatabase(Config, Self);
end;

function TRubezh3Device.GetTime: TDateTime;
begin
  result := 0;
end;

procedure TRubezh3Device.HandleNormalConnection(FirstDT: TDatetime);

  procedure DoLogRestoreConnection;
  begin
    {$IFDEF DEBUG} // CONNECT
    WriteToLog(rkMessage, rtConnect,  FPanelDeviceMod.DeviceShortName + ' ' + IntToStr(Self.Device.DeviceAddress)+
      ' Logging restore connection');
    {$ENDIF}
    Statefull.ResetStateByID(GetDeviceDriver, psLostConnection);
    with LogDeviceActionNewEvent(True)^ do
    begin
      EventClass := dscNormal;
      EventMessage := rsConnectionRestored;
      DateTime := FirstDT;
      SysDateTime := FirstDT;
    end;
  end;

  procedure SetParamValue(const ParamName: string; const ParamValue: Variant);
  var
    Param: IParam;
  begin
    Param := Self.DeviceParams.FindParam(ParamName);
    if Param <> nil then
      Param.SetValue(ParamValue);
  end;

begin
  UpdateReserveStatus(FirstDT);
  if not IsUSBDevice(Device) then
    UpdateDoubling;

  if FConnectionState = csConnectionLost then
    DoLogRestoreConnection else
    begin
      {$IFDEF DEBUG} // CONNECT
      WriteToLog(rkMessage, rtConnect, FPanelDeviceMod.DeviceShortName + ' ' + IntToStr(Self.Device.DeviceAddress)+
      ' Clearing lost connection');
      {$ENDIF}
      Statefull.ResetStateByID(GetDeviceDriver, psLostConnection);
    end;

  FConnectionState := csConnected;

  SetFailureType(Self, '', False);
end;

procedure TRubezh3Device.HandleRealtimeException(const ExceptionObj: Exception);

  function GetUserExceptMessage(E: Exception): string;
  begin
    if not (E is ERubezh3UserError) then
      result := rsPanelNoAnswer else
      result := E.Message;
  end;

  procedure DoLogException(E: Exception);
  begin
      if not Statefull.HasStateID(GetDeviceDriver, psLostConnection) then
      begin
        with LogDeviceActionNewEvent^ do
        begin
          EventClass := dscError;
          EventMessage := GetUserExceptMessage(E);
          {$IFDEF DEBUG} // EXCEPT
          WriteToLog(rkMessage, rtExcept, FPanelDeviceMod.DeviceShortName + ' ' + IntToStr(Self.Device.DeviceAddress)+' Exception: '+E.Message);
          {$ENDIF}
  //        Copy(E.Message, 1, Min(Length(E.Message), 254));
          DateTime := Now;
          SysDateTime := Now;
        end;
      end;
      ClearAnyState(Self, True);

      Statefull.SetStateByID(GetDeviceDriver, psLostConnection);
      FLastExceptMessage := GetUserExceptMessage(E);
      FLastRealExceptMessage := E.Message;

      if IsRubezh3Debug then
        SetFailureType(Self, FLastRealExceptMessage) else
        SetFailureType(Self, rsPanelNoAnswer);
  end;

  procedure SetParamValue(const ParamName: string; const ParamValue: Variant);
  var
    Param: IParam;
  begin
    Param := Self.DeviceParams.FindParam(ParamName);
    if Param <> nil then
      Param.SetValue(ParamValue);
  end;

begin
  {$IFDEF DEBUG} // EXCEPT
  WriteToLog(rkMessage, rtExcept, FPanelDeviceMod.DeviceShortName + ' ' + IntToStr(Self.Device.DeviceAddress)+' Realtime exception');
  {$ENDIF}

  UpdateReserveStatus(Now);
  if not IsUSBDevice(Device) then
    UpdateDoubling;

  if OperationCanceled then
  begin
    {$IFDEF DEBUG} // EXCEPT
    WriteToLog(rkMessage, rtExcept, 'Is not exception is Cancel');
    {$ENDIF}
    exit;
  end;

  case FConnectionState of
    csNotConnected: assert(False, '{C9437E3F-81EA-4461-A489-0D32AFD04DD5}');
    csIntermediate:
      begin
        if (now - FLastStateChange) > (FConnectionLostDelay * OneDTSecond) then
        begin
          FConnectionState := csConnectionLost;
          FLastStateChange := Now;

          FInterface.ResetDevice;

          FStatesReaded := False;
          FUIDReaded := False;
          FLastUIDCheck := 0;
//          FOutDevicesStateKnown := False;

          DoLogException(ExceptionObj);
        end;
      end;
    csConnectionLost:
      begin
        if FLastExceptMessage <> GetUserExceptMessage(ExceptionObj) then
          DoLogException(ExceptionObj);

        FInterface.doTimeSlice;

        FInterface.ResetDevice;
        FStatesReaded := False;
        FUIDReaded := False;
        FLastUIDCheck := 0;
//        FOutDevicesStateKnown := False;

      end;
    csConnected:
      begin
        if (FConnectionLostDelay = 0) or Statefull.HasStateID(GetDeviceDriver, devInitializing) then
        begin
          FConnectionState := csConnectionLost;
          DoLogException(ExceptionObj);

          FStatesReaded := False;
          FUIDReaded := False;
//          FOutDevicesStateKnown := False;
        end else
          FConnectionState := csIntermediate;

        FLastUIDCheck := 0;

        FInterface.ResetDevice;
        FLastStateChange := Now;
      end;
  end;
end;

procedure TRubezh3Device.InstanceSetState(const DeviceInstance: IDeviceInstance; const State: IDeviceState;
  StartTime: double = 0);
begin
  // Центральная процедура для установки состояний для всех устройств.
  // Нужна для фильтрации

  if (DeviceInstance.ParentInstance <> nil) and (DeviceInstance.DeviceDriver = DeviceInstance.ParentInstance.DeviceDriver) and
    (SameText(State.GetStateCode, 'MPT_Auto_On') or SameText(State.GetStateCode, 'PPU_Auto_On')) then
      exit;
  
  (DeviceInstance as IStatefull).SetState(State, StartTime);
end;

procedure TRubezh3Device.InternalFinalize;
begin
  FDatabase := nil;
  FreeAndNil(FInterface);
  FConnectionState := csNotConnected;

  if assigned(FOnFInitTransport) then
    FOnFInitTransport(Self);
end;

procedure TRubezh3Device.InternalInitialize;
var
  DataTransport: IDataTransport;
  CrossLinkedDevice: ICrossLinkedDevice;
  Rubezh3DeviceDriver: IRubezh3DeviceDriver;
begin
  FConnectionState := csNotConnected;
  if ((GetDeviceAddress < MIN_PANEL_ADDRESS) or (GetDeviceAddress > MAX_EXT_PANEL_ADDRESS)) and
    (GetDeviceParamDef(Device, 'SYS$SKIPCHECK', 0) <> 1) then
      raise Exception.Create(rsInvalidAddress);

  if assigned(FOnInitTransport) then
    FOnInitTransport(Self);

  DataTransport := GetDataTransport;
  if DataTransport = nil then
    raise Exception.Create(rsDeviceMustBeConnectedToTransport);

  if not GetIsIOCTLFunctionMode and (Supports(Device.DeviceDriver, ICrossLinkedDevice, CrossLinkedDevice) and
    CrossLinkedDevice.HasCrossLinks(Device)) then
  begin
    if not Supports(DataTransport, IUSBChannel) then
      raise Exception.Create(rsErrorExternalCom);
  end;

  FInterface := TRubezh3Interface.Create(GetDeviceAddress, DataTransport, FPanelDeviceMod);
  FInterface.RetryCount := DEFAULT_RETRY_COUNT{GetDeviceProp(Self, 'RetryCount')};
  FInterface.OnTransportOperation := TransportOperationNotify;
  FInterface.Canceled := False;
  if not Supports(GetDeviceDriver, IRubezh3DeviceDriver, Rubezh3DeviceDriver) then
    raise Exception.Create('Is not an R3 device driver');
  FInterface.Model := Rubezh3DeviceDriver.GetDatabaseModelExt;
  FConnectionState := csConnected;

{
  FReserveLine1Fail := False;
  FReserveLine2Fail := False;
}

  FSetDateTime := GetDevicePropDef(Config, 'SetDateTime', True);

  FCSIndex := GetDeviceStateIndexByID('ConnectionLost');
  FMlIndex := GetDeviceStateIndexByID('Malfunction');
  FHardwareIgnore := GetDeviceStateIndexByID('HardwareIgnore');

  FDelayedSynchronizationFailed := False;

  SetLength(FConfigHashCache, 0);

//  UpdateReservedLinesStates;

end;

function TRubezh3Device.IOCTL_ExecuteFunction(const FunctionCode: string; out Reason: string): boolean;
begin
  result := false;

  if (FPanelDeviceMod.DeviceSubtype in [dsIndicator{$IFDEF RubezhRemoteControl}, dsRemoteControl, dsRemoteControlFire {$ENDIF}]) then
  begin
    if SameText(FunctionCode, 'Touch_SetMaster') then
    begin
      FInterface.TouchMemoryOperation(0);
      result := true;
      reason := 'Приложенный в течении 30 секунд ключ будет назначен мастер-ключом';
    end else
    if SameText(FunctionCode, 'Touch_ClearMaster') then
    begin
      FInterface.TouchMemoryOperation(1);
      result := true;
      reason := 'Пользовательские ключи стерты';
    end else
    if SameText(FunctionCode, 'Touch_ClearAll') then
    begin
      FInterface.TouchMemoryOperation(2);
      result := true;
      reason := 'Все ключи стерты';
    end else
  end else
  if (FPanelDeviceMod.DeviceSubtype = dsOrdinaryPanel) and (FPanelDeviceMod.BaseNodeType = 1) then
  begin
    if SameText(FunctionCode, 'Set_BlindMode') then
    begin
      FInterface.SetBlindMode(True);
      result := true;
      reason := 'Режим "глухой панели" установлен';
    end else
    if SameText(FunctionCode, 'Reset_BlindMode') then
    begin
      FInterface.SetBlindMode(False);
      result := true;
      reason := 'Режим "глухой панели" снят';
    end;
  end;
end;

function TRubezh3Device.IsRecursionAllowed: boolean;
begin
  result := True;
end;

function TRubezh3Device.KeyValid: boolean;
begin
  result := IsBAES and FSeenThird;
end;

function TRubezh3Device.LogDeviceActionNewEvent(First: boolean = False): PEventRecord;
begin
  { Если событие пишется в момент чтения журнала событий, то пишем его именно в
    журнал }
  if FEventList = nil then
  begin
    if FDeviceActionLogList = nil then
      FDeviceActionLogList := TEventList.Create;
    result := FDeviceActionLogList.NewEventRecord;
  end else
    if First then
      result := FEventList.NewFirstEventRecord else
      result := FEventList.NewEventRecord;
end;

function TRubezh3Device.LogEvent(const AnEventMessage: string): PEventRecord;
begin
  result := LogDeviceActionNewEvent;
  with result^ do
  begin
    EventClass := dscNormal;
    DateTime := Now;
    SysDateTime := DateTime;
    EventMessage := AnEventMessage;
  end;
end;

function TRubezh3Device.NeedUpdateMemType(MemoryType, Version, CRC: Integer): Boolean;
var
  DeviceVersion, DeviceCRC: word;
  OldMode: Boolean;
begin
  OldMode := FInterface.SoftUpdateMode;
  FInterface.SoftUpdateMode := True;
  try
    case MemoryType of
      memTypeClearDatabase:
        begin
          {$IFDEF DEBUG} // SETTING
          WriteToLog(rkMessage, rtSetting, 'Setting FNeedClearDatabase');
          {$ENDIF}
          FNeedClearDatabase := True;
          result := true;
        end;
      memTypeLoaderRAM485, memTypeLoaderRAMUSB : result := False;
      else
        begin

          DeviceVersion := 0;

          // Запрашиваем информацию о прошивке
          try

            // Запрашиваем версию установленную в системе.
            case MemoryType of
              memTypeLoaderFlash: DeviceVersion := FInterface.GetFirmwareInfo(02, DeviceCRC);
              memTypeApp: DeviceVersion := FInterface.GetFirmwareInfo(01, DeviceCRC);
              memTypeAVR:
                begin
                  result := not FInterface.SameAVRVersion(Version, CRC, FAVRInfo);
                  exit;
                end;
            end;

          except
          end;

          result := (DeviceVersion <> Version) or (CRC <> DeviceCRC);

        end;
    end;
  finally
    FInterface.SoftUpdateMode := OldMode;
  end;
end;

procedure TRubezh3Device.QueueDatabaseRead(ADevAddress: Integer; DevType: TRubezh3DeviceType; NoDelay: boolean);
begin
  FLock.Acquire;
  try
    SetLength(FReadQueue, Length(FReadQueue) + 1);
    with FReadQueue[High(FReadQueue)] do
    begin
      {$IFDEF DEBUG} // SETTING
      WriteToLog(rkMessage, rtSetting, 'Queueing operative database read');
      {$ENDIF}
      DeviceType := devtype;
      DevAddress := ADevAddress;
      if NoDelay then
        StartTime := Now + DELAY_BEFORE_EXT_READ else
        StartTime := Now;

      FirstTime := True;
      RetryCount := 3;
    end;
  finally
    FLock.Release;
  end;
end;

procedure TRubezh3Device.Reload;
begin
  FInterface.EndSoftUpdate_Reset;
end;

procedure TRubezh3Device.RemoveFromIgnoreList(const Device: IDeviceInstance);
begin
  FLock.Acquire;
  try
    FIgnoreAdditions.Remove(Device);
    if FIgnoreToRemove.IndexOf(Device) = -1 then
      FIgnoreToRemove.Add(Device)
  finally
    FLock.Release;
  end;
end;

function TRubezh3Device.RequestSoftVersion: Integer;
var
  OldMode: Boolean;
begin
  OldMode := FInterface.SoftUpdateMode;
  FInterface.SoftUpdateMode := True;
  try
    result := (FInterface.GetFirmwareVersion);
  finally
    FInterface.SoftUpdateMode := OldMode;
  end;
end;

procedure TRubezh3Device.doEndWork;
begin

end;

function TRubezh3Device.doGetCurrentSignature: TDeviceSignature;
begin
  result := FPanelDeviceMod.Signature;
end;

procedure TRubezh3Device.doIncComplete(value: integer);
begin
  inc(FCompleteSize, value);
end;

procedure TRubezh3Device.doPrepareRAMLoader(const Offset: integer; Stream: TMemoryStream; InROMLoader: boolean = false);

  procedure MemWrite(Addr, ASize: Dword; Buffer: PChar);
  var
    sent, tosend: dword;
    p: pchar;
  begin
    p := Buffer;
    sent := 0;

    while sent < ASize do
    begin
      tosend := min(MAX_PROTOCOL_MEM_BLOCK_SIZE, ASize - sent);
      RawWrite(memTypeApp, Addr + sent, tosend, p^);
//      VerifyBlock(Addr + sent, tosend, p^);
      inc(sent, tosend);
      inc(p, tosend);
    end;
  end;

var
  i: integer;
  ds: word;
begin
  if InROMLoader then
  begin
    ds := FInterface.GetDeviceState;

    if FInterface.GetDeviceState = 0 then
    begin
//      doProgress(rsSettingFirmwareUpdateMode, True);
      // Переходим в режим обновления ПО
      try
        FInterface.BeginSoftUpdate(1);
      except
        // Игнорируем ошибку
      end;

      if FInterface.GetDeviceState and $01 = 0 then
        raise Exception.Create(rsUnableToSetFWUpdateMode);

    end else
    if (ds = 8) then
      exit;
  end;


  for I := 2 downto 0 do
  begin

    // Грузим RAM загрузчик. Пишем его в память, поэтому стирать ничего не надо
    // Адрес всегда фиксированный 0x40000000
    doProgress(rsFormingTempLoader, True);
    MemWrite(offset, Stream.Size, Stream.Memory);

    // Передаем ему управление
    doProgress(rsTransferingControlToLoader, True);
    try
      FInterface.ExceptionOnRepeat := true;
      try
        FInterface.BeginSoftUpdate(4);
      finally
        FInterface.ExceptionOnRepeat := false;
      end;
      break;
    except
      On E: ERubezh3MemError do
        if i = 0 then
          raise;
      else raise;
    end;

  end;

  // Проверяем что управление передалось
  if FInterface.GetDeviceState <> $08 then
    raise Exception.Create(rsTransferingControlFailed);

end;

procedure TRubezh3Device.doProgress(const Caption: string; UnknownProgress: Boolean = False);
var
  BytesRW: Integer;
begin
  BytesRW := GetIOCounter;
  if UnknownProgress then
    Progress(-1, Caption, -1, BytesRW) else
  begin
    if FTotalSize <> 0 then
      Progress(FStage, Caption, Round((FCompleteSize/FTotalSize)*100), BytesRW) else
      Progress(FStage, Caption, 0, BytesRW);
  end;
end;

procedure TRubezh3Device.doSetDateTime;
{var
  a: TDatetime;}
begin
{  a := FInterface.DateTimeGet;

  exit; {!!!!!!!!!!!}

  if FSetDateTime and (Now - FLastSyncTime > 0.5) then
  begin
    {$IFDEF DEBUG}
    WriteToLog(rkMessage, rtModBus, 'Synchonizing date and time');
    {$ENDIF}
    if not FInterface.FastLoseDetection then
      Progress(0, rsSyncDateTime, 0);
    FInterface.DateTimeSet(Now);
    FLastSyncTime := Now;
  end;
end;

procedure TRubezh3Device.doStartWork(TotalSize: integer);
begin
  Progress(0, rsStartingWork, 0);

  FCompleteSize := 0;
  FStage := 0;
  FTotalSize := TotalSize;
end;

procedure TRubezh3Device.DoSynchronizationFailed;
begin
  if GetIntErrorState <> dbInvalid then
    with LogDeviceActionNewEvent^ do
    begin
      EventClass := dscServiceRequired;
      EventMessage := rsErrorDeviceListDif;
      DateTime := Now;
      SysDateTime := Now;
    end;
  SetIntErrorState(dbInvalid);
end;

function TRubezh3Device.FindAltState(State: IDeviceState; Index: Integer; AltStates: TAltStates): IDeviceState;
var
  i: integer;
begin
  result := nil;

  for i := 0 to High(AltStates) do
    if (AltStates[i].Source = State) and (AltStates[i].Index = Index) then
    begin
      result := AltStates[i].Target;
      break;
    end;

  if result = nil then
    result := State;
end;

function TRubezh3Device.HasAltState(const DeviceInstance: IDeviceInstance; State: IDeviceState; Index: Integer; AltStates: TAltStates): boolean;
begin
  result := (DeviceInstance as IStatefull).HasState(FindAltState(State, Index, AltStates));
end;

procedure TRubezh3Device.DoSetAltState(const DeviceInstance: IDeviceInstance; State: IDeviceState; Index: Integer; AltStates: TAltStates; StateStartTime: TDateTime);
begin
  InstanceSetState(DeviceInstance, FindAltState(State, Index, AltStates), StateStartTime);
end;

procedure TRubezh3Device.DoResetAltState(const DeviceInstance: IDeviceInstance; State: IDeviceState; Index: Integer; AltStates: TAltStates);
begin
  (DeviceInstance as IStatefull).ResetState(FindAltState(State, Index, AltStates));
end;

procedure TRubezh3Device.SetDatabaseState(Value: TDatabaseState);
begin
  Config.DeviceParams.SetParamValue('DatabaseState', Integer(Value));
{$IFNDEF TestSkipCalcHash}
  if Value = dbValidated then
    Statefull.ResetStateByID(GetDeviceDriver, psSyncronizationFailed) else
    Statefull.SetStateByID(GetDeviceDriver, psSyncronizationFailed);
{$ENDIF}
end;

type

  TRawPointer = array[0..2] of byte;
  TColoring = array[0..14] of byte;

  PRCGroupDevRec = ^TRCGroupDevRec;
  TRCGroupDevRec = packed record
    offset: TRawPointer;
    full_addr: word;  {адрес, шлейф}
    reserved: array[0..12] of byte;
    DelayOn: byte;
    DelayOff: byte;
    options: byte;
    ExecType: byte;
    GroupNum: byte;
  end;

  PRCTMRec = ^TRCTMRec;
  TRCTMRec = packed record
    offset: TRawPointer;
    full_addr: word;  {адрес, шлейф}
    reserved: array[0..15] of byte;
    GroupNum: byte;
  end;

  TLedType = (ltFireZone, ltSecZone, ltExecutive, ltTM, ltNS, ltPump, ltSilentSecZone);

  PRCRecord = ^TRCRecord;
  TRCRecord = packed record
    offset: TRawPointer;
    PanelAddr: byte;   //адрес прибора
    full_addr: word;  //адрес, шлейф
    reserved: array[0..12] of byte;
    DelayOn: byte;
    DelayOff: byte;
    options: byte;
    ExecType: byte;
    GroupNum: byte;
    IdRec: TLedType;
  end;


  PRCRecords = ^TRCRecords;
  TRCRecords = array of TRCRecord;

  // ПДУ-ПТ

  PRCFRecord = ^TRCFRecord;
  TRCFRecord = record
    DeviceInGroup: IDevice;
    offset: TRawPointer;
    PanelAddr: byte;   //адрес прибора
    full_addr: word;  //адрес, шлейф
    DelayOn: byte;
    ZoneNo: word;
    ExecType: byte;
    GroupNum: byte;
    IdRec: TLedType;
  end;

  PRCFDBHeader = ^TRCFDBHeader;
  TRCFDBHeader = packed record
    version: word;
    CRC: word;
    DBSize: dword;
    PanelCount: word;
    GroupCount: word;
    HashVersion: byte;
    Hash: TMD5Digest;
    Reserved: array [1..47] of byte;
  end;

  PRCFPanelHeader = ^TRCFPanelHeader;
  TRCFPanelHeader = packed record
    addr: byte;
    MD5: TMD5Digest;
    MD5Offset: dword;
    PanelType: byte;
  end;

  PRCFGroupRecord = ^TRCFGroupRecord;
  TRCFGroupRecord = packed record
    addr: byte;  //адрес
    Delay: byte;
    Options: byte;
    CountDev: byte;
    offset: dword;
  end;

  PRCFDevRecord = ^TRCFDevRecord;
  TRCFDevRecord = packed record
    full_addr: word;  //адрес, шлейф
    PanelAddr: byte;   //адрес прибора
    ExecType: byte;
    ZoneNo: word;
    GroupNum: byte;
    offset: dword;
  end;

  PRCFRecords = ^TRCFRecords;
  TRCFRecords = array of TRCFRecord;
  /////////////

  TLEDRecord = packed record
    MemAddr: TRawPointer;
    DevAdr: byte;    //адрес прибора
    ZoneNum: word;   //номер зоны (локальный в приборе)
    coloring: TColoring;
    options: byte;   // в последнем байте младший бит будет отвечать за включение в направление.
    ExecType: byte;
    VDNum: byte;     //# светодиода, соответствующего зоне или ИУ
    zoneid: TLedType;    //идентификатор (зона (пожарная или охранная) или ИУ) 0 - пожарная, 1 - охранная, 2 - ИУ, 3 - ТМ.
  end;

  PLEDRecords = ^TLEDRecords;
  TLEDRecords = array of TLEDRecord;


  PLedDBHeader = ^TLedDBHeader;
  TLedDBHeader = packed record
    version: word;
    CRC: word;
    DBSize: dword;
    PanelCount: word;
  end;

  PLEDPanelHeader = ^TLedPanelHeader;
  TLEDPanelHeader = packed record
    addr: byte;
    MD5: TMD5Digest;
    MD5Offset: TRawPointer;
    ZoneCount: word;
    ZonesStart: TRawPointer;
    ExecCount: word;
    ExecStart: TRawPointer;
    TMCount: word;
    TMStart: TRawPointer;
  end;

  PLEDPanelHeaderV3 = ^TLedPanelHeaderV3;
  TLEDPanelHeaderV3 = packed record
    addr: byte;
    MD5: TMD5Digest;
    MD5Offset: TRawPointer;
    ZoneCount: word;
    ZonesStart: TRawPointer;
    ExecCount: word;
    ExecStart: TRawPointer;
    TMCount: word;
    TMStart: TRawPointer;
    NSCount: word;
    NSStart: TRawPointer;
    PumpCount: word;
    PumpStart: TRawPointer;
  end;

  // 12.09.2011 для новой база ПДУ (версия 2)
  PRCDBHeaderV2 = ^TRCDBHeaderV2;
  TRCDBHeaderV2 = packed record
    version: word;
    CRC: word;
    DBSize: dword;
    PanelCount: word;
    DeviceCount: word;
  end;

  PRCPanelHeaderV2 = ^TRCPanelHeaderV2;
  TRCPanelHeaderV2 = packed record
    addr: byte;
    MD5: TMD5Digest;
    MD5Offset: dword;
    PanelType: byte;
  end;

  TLinkedRCs = array[0..8] of byte;

  PRCGroupDevRecV2 = ^TRCGroupDevRecV2;
  TRCGroupDevRecV2 = packed record
    full_addr: word;  //адрес, шлейф
    PanelAddr: byte;  //адрес прибора
    ExecType: byte;
    options: byte;
    GroupNum: byte;
    IdDouble: byte;
    offset: dword;
    DelayOn: byte;
    DelayOff: byte;
    LinkedRCs: TLinkedRCs;
  end;
{
  PRCRecordV2 = ^TRCRecordV2;
  TRCRecordV2 = packed record
    full_addr: word;  //адрес, шлейф
    PanelAddr: byte;  //адрес прибора
    ExecType: byte;
    options: byte;
    GroupNum: byte;
    offset: dword;
    DelayOn: byte;
    DelayOff: byte;
    LinkedPDUs: TLinkedPDUs;
  end;   }
  //////////////////

  TPanelHeaders = array[0..0] of TLEDPanelHeader;

  PLEDZoneRec = ^TLEDZoneRec;
  TLEDZoneRec = packed record
    offset: TRawPointer;
    ZoneNum: word;
    ZoneType: byte;
    VDNum: byte;
  end;

  PLEDExecRec = ^TLEDExecRec;
  TLEDExecRec = packed record
    offset: TRawPointer;
    full_addr: word;  {адрес, шлейф}
    coloring: TColoring;
    options: byte;
    ExecType: byte;
    VDNum: byte;
  end;

  PLEDNsRec = ^TLEDNsRec;
  TLEDNsRec = packed record
    offset: TRawPointer;
    addr: byte;
    VDNum: byte;
  end;

  PLEDPumpRec = ^TLEDPumpRec;
  TLEdPumpRec = packed record
    offset: TRawPointer;
    full_addr: word;  {адрес, шлейф}
    VDNum: byte;
  end;

  TLEDRecordsSort = class(TQuickSort)
  private
    FLEDRecords: PLEDRecords;
  protected
    function Low: Integer; override;
    function High: Integer; override;
    procedure Swap(Index1, Index2: Integer); override;
    function Compare(Index1, Index2: Integer): Integer; override;
  public
    constructor Create(LEDRecords: PLEDRecords);
  end;

{$IFDEF RubezhRemoteControl}
  TRCRecordsSort = class(TQuickSort)
  private
    FRCRecords: PRCRecords;
  protected
    function Low: Integer; override;
    function High: Integer; override;
    procedure Swap(Index1, Index2: Integer); override;
    function Compare(Index1, Index2: Integer): Integer; override;
  public
    constructor Create(RCRecords: PRCRecords);
  end;
{$ENDIF}

function GetExtDevicePartitionList(const Device: IDevice; const PartitionList: IPartitionList; const PartitionType: string): IInterfaceList;
var
  Zone: IZone;
  Partition: IPartition;
begin
  result := TInterfaceList.Create;


  with DeviceEnumAllZones(Device) do
    while Next(Zone) = S_OK do
      with PartitionList.EnumPartitions(Zone, PartitionType) do
        while Next(Partition) = S_OK do
          if result.IndexOf(Partition) = -1 then
            result.Add(Partition);
end;

function GetExtDeviceDirection(const Device: IDevice; const PartitionList: IPartitionList): IPartition;
begin
  with GetExtDevicePartitionList(Device, PartitionList, 'direction') do
    case Count  of
      0: result := nil;
      1: result := Items[0] as IPartition;
      else
        raise Exception.Create(rsecTooManyPartions);
    end;
end;

procedure TRubezh3Device.SetDevices(const DeviceConfig: IDeviceConfig;
  const Device: IDevice; out ResultStr: string; Param: string = '');

  procedure doWriteMDS(AlsoFullList: boolean = true);
  var
    i: integer;
    RawAddressList: TRawAddressList;
    OldDataRec, DataRec: TDatarec;
    USBDataTransport: IUSBDataTransport;
  begin
    // Прошиваем адресный лист и список устройств

    if AlsoFullList then
    begin
      RawAddressList := GetRawAddrList(Device, True);
      FInterface.SetAddressList(RawAddressList, True);
    end;

    ConvertZoneLogicToZone(DeviceConfig.GetRootDevice);
    if GetFixedDataTransport = nil then
    begin
      UpdateAddressList(Device);
    end else
    begin
      RawAddressList := GetRawAddrList(Device, False);

      FillChar(Datarec, Sizeof(Datarec), 0);

      for I := 0 to High(RawAddressList.adresses) do
        Datarec.RawAddressList[i] := RawAddressList.adresses[i];

      DataRec.address := Device.DeviceAddress;

      if Device.ParentDevice.DeviceProperties.FindParam('BaudRate') <> nil then
      begin
        i := GetDevicePropDef(Device.ParentDevice, 'BaudRate', 0);
        if i = CBR_9600 then
          DataRec.speed := 0 else
        if i = CBR_19200 then
          DataRec.speed := 1 else
        if i = CBR_38400 then
          DataRec.speed := 2 else
        if i = CBR_57600 then
          DataRec.speed := 3 else
        if i = CBR_115200 then
          DataRec.speed := 4 else
        if i in [0..4] then
          DataRec.speed := i else
        raise Exception.Create('Неверное значение скорости: ' + IntToStr(i));
      end else
        DataRec.speed := GetDevicePropDef(Device.ParentDevice.ParentDevice, 'BaudRate', USB_DEFAULT_BAUDRATE);

      FInterface.GetUSBConfig(OldDataRec);

      if not CompareMem(@OldDataRec, @DataRec, Sizeof(DataRec)) then
      begin
        FInterface.SetUSBConfig(DataRec);

        if AlsoFullList then
        begin
          if (GetFixedDataTransport <> nil) and Supports(GetFixedDataTransport, IUSBDataTransport, USBDataTransport) then
          begin
            USBDataTransport.ResetDevice;
            if not USBWaitUserMode(USBDataTransport, True, 300 {150}) then
              raise Exception.Create(rsUSBNotInUserMode);
          end;
        end;
      end;
    end;

  end;

  procedure DoWriteMDS345;
  var
    data: TMDS5Data;

    procedure GetDatabase(const Panel: IDevice);
    Const
      Mask : array[0..7] of byte = (1,2,4,8,16,32,64,128);
    var
      i,j: integer;
      Str, TmpStr: string;
      Bytes: array[0..3] of byte;
    begin
      // у МС-3 и МС-4 в базе имеется только фильтр событий
      if Panel.DeviceDriver.GetBaseType = 102 then
      begin
        Data.Phone1:=GetDevicePropDef(Panel,'Phone1','');
        Data.Phone2:=GetDevicePropDef(Panel,'Phone2','');
        Data.Phone3:=GetDevicePropDef(Panel,'Phone3','');
        Data.Phone4:=GetDevicePropDef(Panel,'Phone4','');

        Str := GetDevicePropDef(Panel,'ObjectNumber',0);
        TmpStr:='0000';
        if Str<>'' then begin
          for i := 4 downto 4-Length(Str)+1 do TmpStr[i] := Str[Length(Str)-4+i];
        end;

        Str := TmpStr;
        for i := 0 to 3 do
          Bytes[i] := Ord(Str[i+1])-48;
        Data.ObjectNumber := 0;
        for i := 0 to 3 do
          Data.ObjectNumber := Data.ObjectNumber*256+BCD(Bytes[3-i]);

        Data.TestDialtone := ReverseBytes(GetDevicePropDef(Panel,'TestDialtone',0) div 5);
        Data.TestVoltage := GetDevicePropDef(Panel,'TestVoltage',0) div 10;
        Data.CountRecalls := GetDevicePropDef(Panel,'CountRecalls',0);
        Data.Timeout := GetDevicePropDef(Panel,'Timeout',0) div 10;
        Data.OutcomingTest := GetDevicePropDef(Panel,'OutcomingTest',0)div 10;
      end;

      Str := GetDevicePropDef(Panel,'EventsFilter',0);
      FillChar(Data.EventsFilter,8,0);
      for j := 0 to 7 do
        for i := 0 to 7 do
          if Str[j*8+i+1] = '1' then Data.EventsFilter[j] := Data.EventsFilter[j] or Mask[i]
    end;

  var
    Stream: TMemoryStream;
    i: integer;
    Empty: Byte;

    DataBlock: IDataBlock;
  begin
    if not IsRubezh3FileDB
    {$IFDEF SimulateWriteDB} and not SameText(Param, 'simulate') {$ENDIF} then
    begin
      if not (FInterface.SendPing(DataBlock) = crAnswer) then
        raise Exception.Create(rsErrorUSBNoDevice);
    end;

    {$IFDEF DEBUG} // ACTION
    WriteToLog(rkMessage, rtAction, 'WriteMDS5. Preparing database...');
    {$ENDIF}
    Progress(0, rsPreparingDatabase, 0);

// В данное время проверка версии БД не нужна
//    {$IFDEF DEBUG} WriteToLog(rkMessage, 'WriteMDS5. Writing database...'); {$ENDIF}
{    if not IsRubezh3FileDB then
      ADBVersion := swap(FInterface.GetDatabaseVersion) else
      ADBVersion := 1;

    if (ADBVersion = 1) then
      raise Exception.Create('Версия ' + IntToStr(ADBVersion) +  ' базы в блоке модуля доставки сообщения не поддерживаются Обновите прибор');
    if (ADBVersion > 1) or (ADBVersion = 1) then
      raise Exception.Create('Версия ' + IntToStr(ADBVersion) +  ' базы в блоке модуля доставки сообщения не поддерживается. Обновите FireSec');
}

    Stream := TMemoryStream.Create;
    try
      GetDatabase(Device);
      if Device.DeviceDriver.GetBaseType = 102 then
      begin
        Empty := 0;
        for i := 1 to 21 do
          if i<=Length(Data.Phone1) then Stream.Write(Data.Phone1[i],1) else Stream.Write(Empty,1);
        for i := 1 to 21 do
          if i<=Length(Data.Phone2) then Stream.Write(Data.Phone2[i],1) else Stream.Write(Empty,1);
        for i := 1 to 21 do
          if i<=Length(Data.Phone3) then Stream.Write(Data.Phone3[i],1) else Stream.Write(Empty,1);
        for i := 1 to 21 do
          if i<=Length(Data.Phone4) then Stream.Write(Data.Phone4[i],1) else Stream.Write(Empty,1);

        Stream.Write(Data.ObjectNumber,SizeOf(Data.ObjectNumber));
        Stream.Write(Data.TestDialtone,SizeOf(Data.TestDialtone));
        Stream.Write(Data.TestVoltage,SizeOf(Data.TestVoltage));
        Stream.Write(Data.CountRecalls,SizeOf(Data.CountRecalls));
        Stream.Write(Data.Timeout,SizeOf(Data.Timeout));
        Stream.Write(Data.OutcomingTest,SizeOf(Data.OutcomingTest));
      end;
      Stream.Write(Data.EventsFilter,SizeOf(Data.EventsFilter));

      if not IsRubezh3FileDB
      {$IFDEF SimulateWriteDB} and not SameText(Param, 'simulate') {$ENDIF} then
      begin
        {$IFDEF DEBUG} // OPERATION
        WriteToLog(rkMessage, rtOperation, 'WriteMDS5. Writing AddressList...');
        {$ENDIF}
        Progress(0, rsWritingAddressList, 0);
        doWriteMDS(true);

        {$IFDEF DEBUG} // OPERATION
        WriteToLog(rkMessage, rtOperation, 'WriteMDS5. Writing Database...');
        {$ENDIF}
        doProgress(rsioSoftUpdate, True);
        if FInterface.GetDeviceState = 0 then
        try
          FInterface.MemWriteBlockToMDSDB(Stream.Size, Stream.Memory^);
        except
          FInterface.WaitPanelUserMode;
        end;

      end else
        {$IFDEF SimulateWriteDB} if not SameText(Param, 'simulate') then {$ENDIF}
        with TFileStream.Create(IncludeTrailingPathDelimiter(ExtractFilePath(ParamStr(0))) + 'TstData\'+ GetDeviceText(Device, [dtoNameFirst, dtoShortName, dtoParents]) +'.bin', fmCreate) do
        try
          Stream.Position := 0;
          CopyFrom(Stream, 0);
        finally
          Free;
        end;

    finally
      Stream.Free;
    end;

    if not IsRubezh3FileDB
    {$IFDEF SimulateWriteDB} and not SameText(Param, 'simulate') {$ENDIF} then
    begin
      try
        FInterface.EndSoftUpdate_Reset;
        sleep(500);
      except
        // Игнорируем ошибку
      end;
      // Проверяем, что обновление успешно
      if not WaitPanelUserMode then
        raise Exception.Create(rsFWUpdateFailed);
    end;

  end;

  procedure doWriteIndicator;
  var
    Databases: array of IRubezh3Database;
    ADBVersion: word;
    data: TLEDRecords;
    Addr, StartOffset: dword;

    function IntegerToRawPointer(value: integer): TRawPointer;
    begin
      result[0] := value and $000000FF;
      result[1] := (value and $0000FF00) shr 8;
      result[2] := (value and $00FF0000) shr 16;
    end;
(*
    function GetActivePanelForZone(const Zone: IZone): IDevice;
    var
      ZoneDevice: IDevice;
    begin
      result := nil;
      with Zone.EnumDevices(DeviceConfig.GetRootDevice) do
        while Next(ZoneDevice) = S_OK do
          if {not DeviceHasAsParent(ZoneDevice, Device) and } not IsDeviceOut(ZoneDevice) then
          begin
            result := GetOwnerPanel(ZoneDevice);
            break;
          end;
    end;
*)
    function GetActivePanelsForZone(const Zone: IZone): IDeviceList;
    var
      ZoneDevice: IDevice;
    begin
      result := TInterfaceList.Create;
      with Zone.EnumDevices(DeviceConfig.GetRootDevice) do
        while Next(ZoneDevice) = S_OK do
          if {not DeviceHasAsParent(ZoneDevice, Device) and } not IsDeviceOut(ZoneDevice) then
          begin
            if Result.IndexOf(GetOwnerPanel(ZoneDevice)) = -1 then
              result.Add(GetOwnerPanel(ZoneDevice));
          end;
    end;

    function GetDatabase(const Panel: IDevice): IRubezh3Database;
    var
      Rubezh3DeviceDriver: IRubezh3DeviceDriver;
      s: string;
    begin
      // формируем базу для прибора и сохраняем ее в кэш
      if Length(Databases) = 0 then
        SetLength(Databases, MAX_PANEL_ADDRESS + 1);

      assert(Panel.DeviceAddress <= MAX_PANEL_ADDRESS);

      if Databases[Panel.DeviceAddress] = nil then
      begin
        if not Supports(Panel.DeviceDriver, IRubezh3DeviceDriver, Rubezh3DeviceDriver) then
          raise Exception.Create('Неизвестный тип базы данных для указанного прибора');

        if Rubezh3DeviceDriver.GetDatabaseModelExt = nil then
          raise Exception.Create('Указанный прибор не имеет модели базы данных');

        Databases[Panel.DeviceAddress] := Rubezh3DeviceDriver.GetDatabaseModelExt.CreateDatabase_Memory;
        if Databases[Panel.DeviceAddress] = nil then
          raise Exception.Create('Не удалось создать базу данных для прибора');

        Databases[Panel.DeviceAddress].InitVirtual;

        Databases[Panel.DeviceAddress].WriteDatabase(Panel, TFakeInstance.Create(Rubezh3DeviceDriver), s, Param);
      end;

      result := Databases[Panel.DeviceAddress];
    end;

    procedure SortLEDRecords(var data: TLEDRecords);
    begin
      with TLEDRecordsSort.Create(@Data) do
      try
        Sort;
      finally
        free;
      end;
    end;

    procedure DoRawMemWrite(Addr, ASize: Dword; Buffer: PChar; SetByteProgress: Boolean = False);
    var
      p: pchar;
      sent, tosend: dword;
    begin
      if SetByteProgress then
      begin
        FInterface.BytesTotal := ASize;
        FInterface.BytesComplete := 0;
      end;
      try

        p := Buffer;
        sent := 0;
        while sent < ASize do
        begin
          tosend := min(MAX_PROTOCOL_MEM_BLOCK_SIZE, ASize - sent);
          FInterface.RawPushBlock(Addr + sent, tosend, p^, True);
          FInterface.RawWriteRepeatable(Addr + sent, tosend, p^);
          inc(sent, tosend);
          inc(p, tosend);

          if SetByteProgress then
            FInterface.BytesComplete := sent;
        end;

      finally
        if SetByteProgress then
        begin
          FInterface.BytesTotal := 0;
          FInterface.BytesComplete := 0;
        end;
      end;

    end;

    procedure DoWriteDatabase(Addr, Size: longword; Buf: PChar);
    begin
      // Стираем память
      doProgress(rsClearingMemorySectors + ' ' + IntToStr(FInterface.GetMemBlockByAddress(Addr)) + ' - ' +
        IntToStr(FInterface.GetMemBlockByAddress(Addr+Size)), True);
      FInterface.MemClear(FInterface.GetMemBlockByAddress(Addr), FInterface.GetMemBlockByAddress(Addr+Size));

      Progress(0, rsWritingDatabase, 0);

      DoRawMemWrite(Addr, Size, Buf, False);
    end;

    function GetWriteable(const Device: IDevice): IDevice;
    var
      P: IDevice;
    begin
      P := Device;
      result := nil;
      while P <> nil do
        if optDeviceDatabaseWrite in P.DeviceDriver.DriverOptions then
        begin
          result := P;
          break;
        end else
          P := P.ParentDevice;
    end;

    procedure ProcessDevice(const Device: IDevice);
    var
      Panel: IDevice;
      Panels: IDeviceList;

      Child: IDevice;
      Base: TDOMBase;
      Node, CNode: TXBNode;
      s: string;
      i, j: integer;
      ZoneList: IZonesList;
      Zone: IZone;
    begin

      if HasDeviceProperty(Device.DeviceDriver, 'C4D7C1BE-02A3-4849-9717-7A3C01C23A24') then
      begin
        Base := TDOMBase.Create;
        try
          s := VarToStr(GetDevicePropDef(Device, 'C4D7C1BE-02A3-4849-9717-7A3C01C23A24', ''));
          if s <> '' then
          begin
            Base.Data := s;
            Node := GetDocumentNode(Base.Document);

            if GetAttributeValue(Node, 'type') = '0' then
            begin
              ZoneList := TZonesList.Create;
              ReadZoneList(Node, GetDeviceConfigFromDevice(Device).GetZoneList, ZoneList);

              for I := 0 to ZoneList.Count - 1 do
              begin
                Zone := ZoneList[i] as IZone;
                Panels := GetActivePanelsForZone(Zone);
                if Panels = nil then
                  raise Exception.Create('Нет активных приборов в зоне ' + IntToStr(Zone.ZoneID));

                for J := 0 to Panels.Count - 1 do
                begin
                  Panel := Panels[J] as IDevice;
                  SetLength(data, length(data) + 1);
                  fillchar(data[high(data)], Sizeof(data[high(data)]), 0);
                  with data[high(data)] do
                  begin
                    {$IFDEF SimulateWriteDB}
                    if not SameText(Param, 'simulate') then
                    begin
                    {$ENDIF}
                      Addr := GetDatabase(Panel).GetZoneStateAddress(Zone, ZoneNum);
                      Assert(Addr <> 0);
                    {$IFDEF SimulateWriteDB}
                    end else
                      Addr := ZoneNum;
                    {$ENDIF}

                    DevAdr := Panel.DeviceAddress;
                    VDNum := Device.DeviceAddress + (Device.ParentDevice.DeviceAddress - 1) * 50;
                    if GetZonePropDef(Zone,'ZoneType', 0) = ztFire then
                      zoneid := ltFireZone
                    else if GetZonePropDef(Zone,'ZoneType', 0) = ztGuard then
                    begin
                      if GetZonePropDef(Zone,'Skipped', false) then
                        zoneid := ltSilentSecZone
                      else
                        zoneid := ltSecZone;
                    end;

                    MemAddr := IntegerToRawPointer(Addr);
  {                  MemAdrLow := Addr and $000000FF;
                    MemAdrMid := (Addr and $0000FF00) shr 8;
                    MemAdrHi := (Addr and $00FF0000) shr 16; }
                  end;
                end;
              end;

            end else
            if GetAttributeValue(Node, 'type') = '1' then
            begin
              Child := XMLReadDevice(DeviceConfig.GetRootDevice, Node);
              if Child <> nil then
              begin
                Panel := GetWriteable(Child);
                if Panel = nil then
                  raise Exception.Create('Для ИУ ' + GetDeviceText(Child, [dtoNameFirst, dtoShortName]) +  ' не найдена панель');

                SetLength(data, length(data) + 1);
                fillchar(data[high(data)], Sizeof(data[high(data)]), 0);

                with GetDatabase(Panel) do
                begin

                  with data[high(data)] do
                  begin
                    if IsNSDevice(Child) then
                    begin
                      addr := GetSubDeviceAddress(dtBUNS, 0, 0) + 8;
                      DevAdr := Panel.DeviceAddress;
                    end else
                    if IsPumpDevice(Child) then
                    begin
                      i := GetSubDeviceIndex(dtBUNS, 0, GetDeviceTableType(Child), Child.DeviceAddress);
                      addr := GetSubDeviceAddress(dtBUNS, 0, i) + 9;
                      DevAdr := Panel.DeviceAddress;
                    end else
                    begin
                      {$IFDEF SimulateWriteDB}
                      if not SameText(Param, 'simulate') then
                      begin
                      {$ENDIF}
                        i := GetDeviceIndex(GetDeviceType(Child), GetRealDeviceAddress(Child));
                      {$IFDEF SimulateWriteDB}
                      end else
                        i := GetRealDeviceAddress(Child);
                      {$ENDIF}

                      if i = -1  then
                        raise Exception.Create('Для ИУ ' + GetDeviceText(Child, [dtoNameFirst, dtoShortName]) +  ' не удалось определить индекс в БД');

                      {$IFDEF SimulateWriteDB}
                      if not SameText(Param, 'simulate') then
                      begin
                      {$ENDIF}
                        Addr := ReadExtDeviceStateAddr(GetDeviceType(Child), i);
                      {$IFDEF SimulateWriteDB}
                      end else
                        Addr := GetRealDeviceAddress(Child);
                      {$ENDIF}

                      Assert(Addr <> 0);
                      DevAdr := Panel.DeviceAddress;
                    end;

                    VDNum := Device.DeviceAddress + (Device.ParentDevice.DeviceAddress - 1) * 50;
                    ExecType := GetOldTableType(Child.DeviceDriver.DeviceDriverID);


                    if IsNSDevice(Child) then
                    begin
                      zoneid := ltNS;
                      ZoneNum := Child.DeviceAddress;
                    end else
                    if IsPumpDevice(Child) then
                    begin
                      zoneid := ltPump;
                      ZoneNum := abs(GetDeviceTableType(Child)) - 1024 + Child.DeviceAddress;
                    end else
                    if IsDeviceOut(Child) or (optExtendedZoneLogic in Child.DeviceDriver.DriverOptions) then
                    begin

                      zoneid := ltExecutive;
                      ZoneNum := GetRealDeviceAddress(Child) - $100;
                      // Необходимо проверить, что к ИУ привязано одно или ни одного направление
                      if GetExtDeviceDirection(Child, DeviceConfig.GetPartitionList) <> nil then
                        options := 1;
                    end else
                    begin
                      zoneid := ltTM;
                      ZoneNum := GetRealDeviceAddress(Child) - $100;
                    end;

                    MemAddr := IntegerToRawPointer(Addr);
{                    MemAdrLow := Addr and $000000FF;
                    MemAdrMid := (Addr and $0000FF00) shr 8;
                    MemAdrHi := (Addr and $00FF0000) shr 16; }

                    CNode := FindChildNode(Node, 'device');
                    if CNode <> nil then
                      for i := 0 to { 31 } 30 do
                      begin
                        j := StrToIntDef(GetAttributeValue(CNode, 'state' + IntToStr(i + 1)), -1);
                        if j <> -1 then
                        begin
                          if i mod 2 = 0 then
                            j := j shl 4;

                          coloring[i div 2] := coloring[i div 2] or j;
                        end;
                      end;

                  end;

                end;

              end;
            end;

          end;
        finally
          Base.Free;
        end;
      end;

      with Device.EnumChildren do
        while Next(Child) = S_OK do
          ProcessDevice(Child);

    end;

    procedure doFormZones(PanelAddr, Offset: Integer; CurHeader: PLEDPanelHeader; Stream: TMemoryStream);
    var
      l: integer;
      ZoneRec: TLEDZoneRec;
    begin
      for l := 0 to High(data) do
        if (data[l].DevAdr = PanelAddr) and (data[l].zoneid in [ltFireZone, ltSecZone, ltSilentSecZone]) then
        begin
          FillChar(ZoneRec, Sizeof(ZoneRec), 0);
          ZoneRec.offset := Data[l].MemAddr;
          ZoneRec.ZoneNum := data[l].ZoneNum;
          if data[l].zoneid = ltFireZone then
            ZoneRec.ZoneType := 0
          else if data[l].zoneid = ltSecZone then
            ZoneRec.ZoneType := 1
          else if data[l].zoneid = ltSilentSecZone then
            ZoneRec.ZoneType := 6;
          ZoneRec.VDNum := data[l].VDNum;

          if CurHeader.ZoneCount  = 0 then
            CurHeader.ZonesStart := IntegerToRawPointer(StartOffset + Offset + Stream.Size);

          CurHeader.ZoneCount := CurHeader.ZoneCount + 1;

          Stream.WriteBuffer(ZoneRec, Sizeof(ZoneRec));
        end;
    end;

    procedure doFormExecutives(PanelAddr, Offset: Integer; CurHeader: PLEDPanelHeader; Stream: TMemoryStream);
    var
      l: integer;
      ExecRec: TLEDExecRec;
    begin
      for l := 0 to High(data) do
        if (data[l].DevAdr = PanelAddr) and (data[l].zoneid = ltExecutive) then
        begin
          FillChar(ExecRec, Sizeof(ExecRec), 0);
          ExecRec.offset := Data[l].MemAddr;
          ExecRec.full_addr := data[l].ZoneNum;
          ExecRec.coloring := data[l].coloring;
          ExecRec.options := data[l].options;
          ExecRec.ExecType := data[l].ExecType;
          ExecRec.VDNum := data[l].VDNum;

          if CurHeader.ExecCount  = 0 then
            CurHeader.ExecStart := IntegerToRawPointer(StartOffset + Offset + Stream.Size);

          CurHeader.ExecCount := CurHeader.ExecCount + 1;

          Stream.WriteBuffer(ExecRec, Sizeof(ExecRec));
        end;
    end;

    procedure doFormTM(PanelAddr, Offset: Integer; CurHeader: PLEDPanelHeader; Stream: TMemoryStream);
    var
      l: integer;
      ExecRec: TLEDExecRec;
    begin
      for l := 0 to High(data) do
        if (data[l].DevAdr = PanelAddr) and (data[l].zoneid = ltTM) then
        begin
          FillChar(ExecRec, Sizeof(ExecRec), 0);
          ExecRec.offset := Data[l].MemAddr;
          ExecRec.full_addr := data[l].ZoneNum;
          ExecRec.coloring := data[l].coloring;
          ExecRec.ExecType := data[l].ExecType;
          ExecRec.options := data[l].options;
          ExecRec.VDNum := data[l].VDNum;

          if CurHeader.TMCount  = 0 then
            CurHeader.TMStart := IntegerToRawPointer(StartOffset + Offset + Stream.Size);

          CurHeader.TMCount := CurHeader.TMCount + 1;

          Stream.WriteBuffer(ExecRec, Sizeof(ExecRec));
        end;
    end;

    procedure doFormNS(PanelAddr, Offset: Integer; CurHeader: PLEDPanelHeaderV3; Stream: TMemoryStream);
    var
      l: integer;
      Rec: TLEDNsRec;
    begin
      for l := 0 to High(data) do
        if (data[l].DevAdr = PanelAddr) and (data[l].zoneid = ltNS) then
        begin
          FillChar(Rec, Sizeof(Rec), 0);
          Rec.offset := Data[l].MemAddr;
          Rec.addr := data[l].ZoneNum;
          Rec.VDNum := data[l].VDNum;

          if CurHeader.NSCount  = 0 then
            CurHeader.NSStart := IntegerToRawPointer(StartOffset + Offset + Stream.Size);

          CurHeader.NSCount := CurHeader.NSCount + 1;

          Stream.WriteBuffer(Rec, Sizeof(Rec));
        end;
    end;

    procedure doFormPump(PanelAddr, Offset: Integer; CurHeader: PLEDPanelHeaderV3; Stream: TMemoryStream);
    var
      l: integer;
      Rec: TLEDPumpRec;
    begin
      for l := 0 to High(data) do
        if (data[l].DevAdr = PanelAddr) and (data[l].zoneid = ltPump) then
        begin
          FillChar(Rec, Sizeof(Rec), 0);
          Rec.offset := Data[l].MemAddr;
          Rec.full_addr := data[l].ZoneNum;
          Rec.VDNum := data[l].VDNum;

          if CurHeader.PumpCount  = 0 then
            CurHeader.PumpStart := IntegerToRawPointer(StartOffset + Offset + Stream.Size);

          CurHeader.PumpCount := CurHeader.PumpCount + 1;

          Stream.WriteBuffer(Rec, Sizeof(Rec));
        end;
    end;

  var
    Stream, TmpStream: TMemoryStream;
    i, cnt: integer;

//    PanelHeaders: packed array of TLEDPanelHeader;
    PanelHeadersV3: packed array of TLEDPanelHeaderV3;

//    CurHeader: PLEDPanelHeader;
    CurHeaderV3: PLEDPanelHeaderV3;
    DataBlock: IDataBlock;
  begin
    {$IFDEF SimulateWriteDB}
    if SameText(Param, 'simulate') then
      exit;
    {$ENDIF}

    if not IsRubezh3FileDB
    {$IFDEF SimulateWriteDB} and not SameText(Param, 'simulate') {$ENDIF} then
    begin
      if not (FInterface.SendPing(DataBlock) = crAnswer) then
        raise Exception.Create(rsErrorUSBNoDevice);

      Progress(0, rsWritingAddressList, 0);

      try
        i := FInterface.GetDeviceState;
      except
        i := -1;
      end;

      if i <> 0 then
        // Переходим в режим пользовательского ПО
        try
            FInterface.EndSoftUpdate_Reset;
            sleep(500);
        except
          // Игнорируем ошибку
        end;

      if not WaitPanelUserMode then
        raise Exception.Create(rsFWUpdateFailed);
    end;

    {$IFDEF DEBUG} // ACTION
    WriteToLog(rkMessage, rtAction, 'WriteIndicator. Preparing database...');
    {$ENDIF}
    Progress(0, rsPreparingDatabase, 0);

    ProcessDevice(Device);

    // сортируем список по адресу панели DevAddr
    SortLEDRecords(data);

    // Добавляем пустую
    SetLength(data, length(data) + 1);
    FillChar(data[high(data)], Sizeof(TLEDRecord), 0);

    StartOffset := INDICATOR_DB_START_OFFSET;  // начальный адрес базы в БИ -  0x4000

    {$IFDEF DEBUG} // OPERATION
    WriteToLog(rkMessage, rtOperation, 'WriteIndicator. Get database version...');
    {$ENDIF}

    if not IsRubezh3FileDB
    {$IFDEF SimulateWriteDB} and not SameText(Param, 'simulate') {$ENDIF} then
      ADBVersion := swap(FInterface.GetDatabaseVersion) else
      ADBVersion := 4;

    if (ADBVersion <= 3) then
      raise Exception.Create('Версия ' + IntToStr(ADBVersion) +  ' базы в блоке индикации не поддерживается. Обновите прибор');

    if (ADBVersion > 4) or (ADBVersion = 1) then
      raise Exception.Create('Версия ' + IntToStr(ADBVersion) +  ' базы в блоке индикации не поддерживается. Обновите FireSec');

    // 18.05.2010 Новый формат БД
    Stream := TMemoryStream.Create;
    TmpStream := TMemoryStream.Create;
    try
      Stream.Size := Sizeof(TLEDDBHeader);
      Stream.Position := Stream.Size;

      with PLEDDBHeader(Stream.Memory)^ do
      begin
        version := ADBVersion;
        CRC := 0;
        DBSize := 0;
        PanelCount := 0;
//        CRC := swap(CCITT_CRC(data[0], Length(data) * Sizeof(TLEDRecord)));
      end;

      cnt := 0;
      for I := 0 to High(Databases) do
        if Databases[i] <> nil then
          inc(cnt);

      {$IFDEF DEBUG} // ACTION
      WriteToLog(rkMessage, rtAction, 'WriteIndicator. Forming database...');
      {$ENDIF}

      case ADBVersion of
{        2:
        begin
          SetLength(PanelHeaders, cnt);
          FillChar(PanelHeaders[0], cnt * Sizeof(TLEDPanelHeader), 0);
          Stream.Write(PanelHeaders[0], cnt * Sizeof(PanelHeaders[0]));

          cnt := 0;
          for I := 0 to High(Databases) do
            if Databases[i] <> nil then
            begin
              with PLEDDBHeader(Stream.Memory)^ do
              begin
                inc(PanelCount);
                inc(cnt);
              end;

              TmpStream.Clear;

              CurHeader := PLedPanelHeader(PChar(Stream.Memory) + Sizeof(TLEDDBHeader) + Sizeof(TLEDPanelHeader) * (cnt - 1));
              CurHeader.addr := i;
              CurHeader.MD5 := Databases[i].GetDatabaseHash;
              CurHeader.MD5Offset := IntegerToRawPointer(Databases[i].GetDatabaseHashAddr);

              doFormZones(i, Stream.Size, CurHeader, TmpStream);
              doFormExecutives(i, Stream.Size, CurHeader, TmpStream);
              doFormTM(i, Stream.Size, CurHeader, TmpStream);

              Stream.CopyFrom(TmpStream, 0);
            end;
          end;     }
        4:
        begin

          SetLength(PanelHeadersV3, cnt);
          FillChar(PanelHeadersV3[0], cnt * Sizeof(TLEDPanelHeaderV3), 0);
          Stream.Write(PanelHeadersV3[0], cnt * Sizeof(PanelHeadersV3[0]));

          cnt := 0;
          for I := 0 to High(Databases) do
            if Databases[i] <> nil then
            begin
              with PLEDDBHeader(Stream.Memory)^ do
              begin
                inc(PanelCount);
                inc(cnt);
              end;

              TmpStream.Clear;

              CurHeaderV3 := PLedPanelHeaderV3(PChar(Stream.Memory) + Sizeof(TLEDDBHeader) + Sizeof(TLEDPanelHeaderV3) * (cnt - 1));
              CurHeaderV3.addr := i;
              {$IFDEF SimulateWriteDB}
              if not SameText(Param, 'simulate') then
              begin
              {$ENDIF}
                CurHeaderV3.MD5 := Databases[i].GetDatabaseHash;
                CurHeaderV3.MD5Offset := IntegerToRawPointer(Databases[i].GetDatabaseHashAddr);
              {$IFDEF SimulateWriteDB}
              end;
              {$ENDIF}

              doFormZones(i, Stream.Size, PLedPanelHeader(CurHeaderV3), TmpStream);
              doFormExecutives(i, Stream.Size, PLedPanelHeader(CurHeaderV3), TmpStream);
              doFormTM(i, Stream.Size, PLedPanelHeader(CurHeaderV3), TmpStream);
              doFormNS(i, Stream.Size, CurHeaderV3, TmpStream);
              doFormPump(i, Stream.Size, CurHeaderV3, TmpStream);

              Stream.CopyFrom(TmpStream, 0);
            end;
          end;
      end;

      with PLEDDBHeader(Stream.Memory)^ do
      begin
        DBSize := Stream.Size - Sizeof(TLEDDBHeader);
        CRC := swap(CCITT_CRC( (PChar(Stream.memory) + Sizeof(TLEDDBHeader))^,
          Stream.Size - Sizeof(TLEDDBHeader)));
      end;

      if not IsRubezh3FileDB
      {$IFDEF SimulateWriteDB} and not SameText(Param, 'simulate') {$ENDIF} then
      begin
        {$IFDEF DEBUG} // OPERATION
        WriteToLog(rkMessage, rtOperation, 'WriteIndicator. Writing AddressList...');
        {$ENDIF}

//        Progress(0, rsWritingAddressList, 0);
        doWriteMDS(False);
        Progress(0, rsWritingAddressList, 100);

        doProgress(rsSettingFirmwareUpdateMode, True);
        if FInterface.GetDeviceState = 0 then
          // Переходим в режим обновления ПО
          try
              FInterface.BeginSoftUpdate(1);
          except
            // Игнорируем ошибку
          end;

        if FInterface.GetDeviceState and $01 = 0 then
          raise Exception.Create(rsUnableToSetFWUpdateMode);
        {$IFDEF DEBUG} // OPERATION
        WriteToLog(rkMessage, rtOperation, 'WriteIndicator. Writing database...');
        {$ENDIF}
        DoWriteDatabase(StartOffset, Stream.Size, Stream.Memory);
      end else
      {$IFDEF SimulateWriteDB} if not SameText(Param, 'simulate') then {$ENDIF}
        with TFileStream.Create(IncludeTrailingPathDelimiter(ExtractFilePath(ParamStr(0))) + 'TstData\'+ GetDeviceText(Device, [dtoNameFirst, dtoShortName, dtoParents]) +'.bin', fmCreate) do
        try
          Stream.Position := 0;
          CopyFrom(Stream, 0);
        finally
          Free;
        end;

    finally
      Stream.Free;
      TmpStream.Free;
    end;

    if not IsRubezh3FileDB
    {$IFDEF SimulateWriteDB} and not SameText(Param, 'simulate') {$ENDIF} then
    begin
      {$IFDEF DEBUG} // OPERATION
      WriteToLog(rkMessage, rtOperation, 'WriteIndicator. Resetting device...');
      {$ENDIF}
      doProgress(rsPanelReloading, True);
      FInterface.EndSoftUpdate_Reset;

      // Проверяем, что обновление успешно
      if not WaitPanelUserMode then
        raise Exception.Create(rsFWUpdateFailed);
    end;

  end;

{$IFDEF RubezhRemoteControl}
  procedure doWriteRemoteControl;
  var
    // кэш БД приборов
    Databases: array of IRubezh3Database;
    DeviceTypes: array of byte;

    ADBVersion: word;
    data: TRCRecords;
    Addr, StartOffset: dword;

    function IntegerToRawPointer(value: integer): TRawPointer;
    begin
      result[0] := value and $000000FF;
      result[1] := (value and $0000FF00) shr 8;
      result[2] := (value and $00FF0000) shr 16;
    end;

    function GetDatabase(const Panel: IDevice): IRubezh3Database;
    var
      Rubezh3DeviceDriver: IRubezh3DeviceDriver;
      s: string;
    begin

      // формируем базу для прибора и сохраняем ее в кэш
      if Length(Databases) = 0 then
        SetLength(Databases, MAX_PANEL_ADDRESS + 1);

      if Length(DeviceTypes) = 0 then
        SetLength(DeviceTypes, MAX_PANEL_ADDRESS + 1);

      assert(Panel.DeviceAddress <= MAX_PANEL_ADDRESS);

      if Databases[Panel.DeviceAddress] = nil then
      begin
        if not Supports(Panel.DeviceDriver, IRubezh3DeviceDriver, Rubezh3DeviceDriver) then
          raise Exception.Create('Неизвестный тип базы данных для указанного прибора');

        if Rubezh3DeviceDriver.GetDatabaseModelExt = nil then
          raise Exception.Create('Указанный прибор не имеет модели базы данных');

        Databases[Panel.DeviceAddress] := Rubezh3DeviceDriver.GetDatabaseModelExt.CreateDatabase_Memory;
        if Databases[Panel.DeviceAddress] = nil then
          raise Exception.Create('Не удалось создать базу данных для прибора');

        Databases[Panel.DeviceAddress].InitVirtual;

        Databases[Panel.DeviceAddress].WriteDatabase(Panel, TFakeInstance.Create(Rubezh3DeviceDriver), s);

        DeviceTypes[Panel.DeviceAddress] := Panel.DeviceDriver.GetBaseType;

      end;

      result := Databases[Panel.DeviceAddress];
    end;

    procedure SortRCRecords(var data: TRCRecords);
    begin
      with TRCRecordsSort.Create(@Data) do
      try
        Sort;
      finally
        free;
      end;
    end;

    procedure DoRawMemWrite(Addr, ASize: Dword; Buffer: PChar; SetByteProgress: Boolean = False);
    var
      p: pchar;
      sent, tosend: dword;
    begin
      if SetByteProgress then
      begin
        FInterface.BytesTotal := ASize;
        FInterface.BytesComplete := 0;
      end;
      try

        p := Buffer;
        sent := 0;
        while sent < ASize do
        begin
          tosend := min(MAX_PROTOCOL_MEM_BLOCK_SIZE, ASize - sent);
          FInterface.RawPushBlock(Addr + sent, tosend, p^, True);
          FInterface.RawWriteRepeatable(Addr + sent, tosend, p^);
          inc(sent, tosend);
          inc(p, tosend);

          if SetByteProgress then
            FInterface.BytesComplete := sent;
        end;

      finally
        if SetByteProgress then
        begin
          FInterface.BytesTotal := 0;
          FInterface.BytesComplete := 0;
        end;
      end;

    end;

    procedure DoWriteDatabase(Addr, Size: longword; Buf: PChar);
    begin
      // Стираем память
      doProgress(rsClearingMemorySectors + ' ' + IntToStr(FInterface.GetMemBlockByAddress(Addr)) + ' - ' +
        IntToStr(FInterface.GetMemBlockByAddress(Addr+Size)), True);
      FInterface.MemClear(FInterface.GetMemBlockByAddress(Addr), FInterface.GetMemBlockByAddress(Addr+Size));

      Progress(0, rsWritingDatabase, 0);

      DoRawMemWrite(Addr, Size, Buf, False);
    end;

    function GetWriteable(const Device: IDevice): IDevice;
    var
      P: IDevice;
    begin
      P := Device;
      result := nil;
      while P <> nil do
        if optDeviceDatabaseWrite in P.DeviceDriver.DriverOptions then
        begin
          result := P;
          break;
        end else
          P := P.ParentDevice;
    end;

    procedure ProcessDevice(const Device: IDevice);
    var
      Panel: IDevice;
      GroupData: TRCGroupData;
      Child: IDevice;
      i, j: integer;
    begin
      if HasDeviceProperty(Device.DeviceDriver, 'E98669E4-F602-4E15-8A64-DF9B6203AFC5') then
      begin
        GroupData := GetRCGroupData(Device, GetRootDevice(Device));
        if GroupData.DevCount > 0 then
          for i := 0 to GroupData.DevCount - 1 do
          begin
            Child := GroupData.Devices[i].Device;
            Panel := GetOwnerPanel(Child);
            if (Child = nil) or (Panel = nil) then continue;
            with GetDatabase(Panel) do
            begin
              SetLength(data, Length(data) + 1);
              with data[high(data)] do
              begin
                PanelAddr := Panel.DeviceAddress;
                j := GetDeviceIndex(GetDeviceType(Child), GetRealDeviceAddress(Child));
                if j = -1  then
                  raise Exception.Create('Для ИУ ' + GetDeviceText(Child, [dtoNameFirst, dtoShortName]) +  ' не удалось определить индекс в БД');
                Addr := ReadExtDeviceStateAddr(GetDeviceType(Child), j);

                Offset := IntegerToRawPointer(Addr);
                full_addr := GetRealDeviceAddress(Child) - $100;
                DelayOn := GroupData.Devices[i].DelayOn;
                DelayOff := GroupData.Devices[i].DelayOff;
                Reserved[0] := GroupData.IDDouble;
                if GroupData.Devices[i].Inverse then
                  Options := Options or $80;

                ExecType := OldDeviceTypes[GetDeviceType(Child)];

                GroupNum := Device.DeviceAddress;
                  if GetDeviceType(Child) = dtAMT then
                    IdRec := ltTM else
                    IdRec := ltExecutive;
              end;
            end;
          end;
      end;

      with Device.EnumChildren do
        while Next(Child) = S_OK do
          ProcessDevice(Child);

    end;

    procedure doFormExecutives(PanelAddr, Offset: Integer; CurHeaderV3: PLEDPanelHeaderV3; Stream: TMemoryStream);
    var
      l: integer;
      Rec: TRCGroupDevRec;
    begin
      for l := 0 to High(data) do
        if (PanelAddr = Data[l].PanelAddr) and (Data[l].IdRec = ltExecutive) then
        begin
          FillChar(Rec, Sizeof(Rec), 0);
          Rec.offset := Data[l].offset;
          Rec.full_addr := data[l].full_addr;
          Rec.DelayOn := data[l].DelayOn;
          Rec.DelayOff := data[l].DelayOff;
          Rec.options := data[l].options;
          Rec.ExecType := data[l].ExecType;
          Rec.GroupNum := data[l].GroupNum;

          if CurHeaderV3.ExecCount  = 0 then
            CurHeaderV3.ExecStart := IntegerToRawPointer(StartOffset + Offset + Stream.Size);

          CurHeaderV3.ExecCount := CurHeaderV3.ExecCount + 1;

          Stream.WriteBuffer(Rec, Sizeof(Rec));
        end;
    end;

    procedure doFormExecutives_V2(Stream, PDUStream: TMemoryStream; var RCCnt: integer);
    var
      l: integer;
      Rec: TRCGroupDevRecV2;
      GlobalLinkedRCs : array of integer;
      LinkedRCs: TLinkedRCs;
      RCCount, CountGroups: integer;
      {$IFDEF CACHEPDU}
      // кеширование направлений ПДУ для ускорения поиска устройств
      // в других направлениях своего и других ПДУ
      CachedPDUs: boolean;
      PDUCache: array of array of TRCGroupData;
      {$ENDIF}

      procedure doSearchInOtherRC(ADevice: IDevice);

        {$IFDEF CACHEPDU}
        procedure CachePDUs(Dev: IDevice);
        var Child, GroupDev: IDevice;
        begin
          case Dev.DeviceDriver.GetBaseType of
            -1:
              with Dev.EnumChildren do
                while Next(Child) = S_OK do
                  CachePDUs(Child);
            7:
              begin
                if Length(PDUCache) - 1 < Dev.DeviceAddress then
                  SetLength(PDUCache, Dev.DeviceAddress + 1);

                if Length(PDUCache[Dev.DeviceAddress]) < 11 then
                  SetLength(PDUCache[Dev.DeviceAddress], 11);

                with Dev.EnumChildren do
                  while Next(GroupDev) = S_OK do
                    PDUCache[Dev.DeviceAddress][GroupDev.DeviceAddress] := GetRCGroupData(GroupDev, DeviceConfig.GetRootDevice);
              end
          end;

        end;
        {$ENDIF}

        procedure AddDevice(DevAddress: integer);
        var k: integer;
            preset: boolean;
            PDUHeader: TRCPanelHeaderV2;
        begin

          // Список ПДУ устройства
          preset := false;
          for k := 0 to RCCount do
            preset := preset or (LinkedRCs[k] = DevAddress);

          if not preset then
          begin
            LinkedRCs[RCCount] := DevAddress;
            inc(RCCount);
          end;

          // Список ПДУ всего прибора (в БД прибора v2 в блок приборов пишутся и ПДУ
          preset := false;
          for k := 0 to Length(GlobalLinkedRCs) - 1 do
            preset := preset or (GlobalLinkedRCs[k] = DevAddress);

          if not preset then
          begin
            SetLength(GlobalLinkedRCs, Length(GlobalLinkedRCs) + 1);
            GlobalLinkedRCs[Length(GlobalLinkedRCs) - 1] := DevAddress;

            FillChar(PDUHeader, SizeOf(TRCPanelHeaderV2), $FF);
            PDUHeader.addr := DevAddress;
            PDUHeader.PanelType := 7;
            PDUStream.WriteBuffer(PDUHeader, SizeOf(PDUHeader));
          end;

        end;

      var
        GroupData: TRCGroupData;
        Child, GroupDev: IDevice;
        PDUAddr, GroupAddr, j: integer;
      begin
        {$IFDEF CACHEPDU}
        if not CachedPDUs then
        begin
          CachePDUs(ADevice);
          CachedPDUs := true;
        end;

        for PDUAddr := 0 to Length(PDUCache) - 1 do
          for GroupAddr := 0 to Length(PDUCache[PDUAddr]) - 1 do
            begin
              GroupData := PDUCache[PDUAddr][GroupAddr];

              for j := 0 to GroupData.DevCount - 1 do
              begin
                if GroupData.Devices[j].Device = nil then continue;

                if (rec.full_addr = GetRealDeviceAddress(GroupData.Devices[j].Device) - 256)
                and (GroupData.Devices[j].Device.ParentDevice <> nil)
                and (rec.PanelAddr = GetOwnerPanel(GroupData.Devices[j].Device).DeviceAddress) then
                begin
                  // если устройство принадлежит только 1 дублю - то оно не групповое
                  if CountGroups = 0 then Inc(CountGroups)
                  else if ((rec.IdDouble <> GroupData.IDDouble)
                   or (GroupData.IDDouble = 0)) then
                     Inc(CountGroups);
                  if PDUAddr <> Device.DeviceAddress then
                    AddDevice(PDUAddr);
                end;
              end;
            end;

        {$ELSE}
        case ADevice.DeviceDriver.GetBaseType of
          -1:
            with ADevice.EnumChildren do
              while Next(Child) = S_OK do
                doSearchInOtherRC(Child);
          7:
//            if not ADevice.IsEqualTo(Device) then
              with ADevice.EnumChildren do
                while Next(GroupDev) = S_OK do
                  begin
                    GroupData := GetRCGroupData(GroupDev, DeviceConfig.GetRootDevice);
                    for j := 0 to GroupData.DevCount - 1 do
                    begin
                      if GroupData.Devices[j].Device = nil then continue;

                      if (rec.full_addr = GetRealDeviceAddress(GroupData.Devices[j].Device) - 256)
                      and (GroupData.Devices[j].Device.ParentDevice <> nil)
                      and (rec.PanelAddr = GetOwnerPanel(GroupData.Devices[j].Device).DeviceAddress) then
                      begin
                        // если устройство принадлежит только 1 дублю - то оно не групповое
                        if CountGroups = 0 then Inc(CountGroups)
                        else if ((rec.IdDouble <> GroupData.IDDouble)
                         or (GroupData.IDDouble = 0)) then
                           Inc(CountGroups);
                          // 21.10.11 Добавляем все ПДУ кроме себя (максимум их может быть в системе 10 - места хватит)
                        if not ADevice.IsEqualTo(Device) then
//                        if (not ADevice.IsEqualTo(Device))
//                        and (rec.PanelAddr = GroupData.Devices[j].Device.ParentDevice.DeviceAddress) then
                          AddDevice(ADevice.DeviceAddress);
                      end;
                    end;
                  end
        end;
        {$ENDIF}
      end;

    begin
    {$IFDEF CACHEPDU}
      CachedPDUs := false;
    {$ENDIF}
      for l := 0 to High(data) do
        if data[l].IdRec in [ltExecutive, ltTM] then
        begin
          FillChar(Rec, Sizeof(Rec), 0);
          Rec.offset := (Data[l].offset[0]) or
                        (Data[l].offset[1] shl 8) or
                        (Data[l].offset[2] shl 16);
          Rec.full_addr := data[l].full_addr;
          Rec.PanelAddr := Data[l].PanelAddr;
          Rec.DelayOn := data[l].DelayOn;
          Rec.DelayOff := data[l].DelayOff;
          Rec.options := data[l].options;
          Rec.ExecType := data[l].ExecType;
          Rec.GroupNum := data[l].GroupNum;
          Rec.IdDouble := data[l].Reserved[0];

          FillChar(LinkedRCs, length(LinkedRCs), $FF);
          RCCount := 0;
          CountGroups := 0;

          doSearchInOtherRC(GetOwnerTransport(Device));
          Rec.LinkedRCs := LinkedRCs;
          if (CountGroups > 1) and (data[l].IdRec = ltExecutive) then
            Rec.Options := Rec.Options or $40;
          Stream.WriteBuffer(Rec, Sizeof(Rec));
        end;

      RCCnt := Length(GlobalLinkedRCs);
    end;

    procedure doFormTM(PanelAddr, Offset: Integer; CurHeaderV3: PLEDPanelHeaderV3; Stream: TMemoryStream);
    var
      l: integer;
      Rec: TRCTMRec;
    begin
      for l := 0 to High(data) do
        if (PanelAddr = Data[l].PanelAddr) and (data[l].IdRec = ltTM) then
        begin
          FillChar(Rec, Sizeof(Rec), 0);
          Rec.offset := Data[l].offset;
          Rec.full_addr := data[l].full_addr;
          Rec.GroupNum := data[l].GroupNum;

          if CurHeaderV3.TMCount  = 0 then
            CurHeaderV3.TMStart := IntegerToRawPointer(StartOffset + Offset + Stream.Size);

          CurHeaderV3.TMCount := CurHeaderV3.TMCount + 1;

          Stream.WriteBuffer(Rec, Sizeof(Rec));
        end;
    end;

  var
    Stream, TmpStream, PDUStream: TMemoryStream;
    i, cnt: integer;
    b: byte;

    PanelHeadersV1: packed array of TLEDPanelHeaderV3;
    PanelHeadersV2: packed array of TRCPanelHeaderV2;

    CurHeaderV1: PLEDPanelHeaderV3;
    CurHeaderV2: PRCPanelHeaderV2;

    DataBlock: IDataBlock;
  begin
    {$IFDEF SimulateWriteDB}
    if SameText(Param, 'simulate') then
      exit;
    {$ENDIF}

    if not IsRubezh3FileDB
    {$IFDEF SimulateWriteDB} and not SameText(Param, 'simulate') {$ENDIF} then
    begin
      if not (FInterface.SendPing(DataBlock) = crAnswer) then
        raise Exception.Create(rsErrorUSBNoDevice);

      Progress(0, rsWritingAddressList, 0);

      try
        i := FInterface.GetDeviceState;
      except
        i := -1;
      end;

      if i <> 0 then
        // Переходим в режим пользовательского ПО
        try
            FInterface.EndSoftUpdate_Reset;
            sleep(500);
        except
          // Игнорируем ошибку
        end;

      if not WaitPanelUserMode then
        raise Exception.Create(rsFWUpdateFailed);
    end;

    {$IFDEF DEBUG} // ACTION
    WriteToLog(rkMessage, rtAction, 'WriteRemoteControl. Preparing database...');
    {$ENDIF}
    Progress(0, rsPreparingDatabase, 0);

    ProcessDevice(Device);
    // сортируем список по адресу устройства
    SortRCRecords(data);

    // Добавляем пустую
    SetLength(data, length(data) + 1);
    FillChar(data[high(data)], Sizeof(TRCRecord), 0);

    StartOffset := INDICATOR_DB_START_OFFSET;  // начальный адрес базы в БИ и ПДУ -  0x4000

    {$IFDEF DEBUG} // OPERATION
    WriteToLog(rkMessage, rtOperation, 'WriteRemoteControl. Get database version...');
    {$ENDIF}
    if not IsRubezh3FileDB
    {$IFDEF SimulateWriteDB} and not SameText(Param, 'simulate') {$ENDIF} then
      ADBVersion := swap(FInterface.GetDatabaseVersion) else
      ADBVersion := 2;

    if (ADBVersion <= 0) then
      raise Exception.Create('Версия ' + IntToStr(ADBVersion) +  ' базы в ПДУ не поддерживается. Обновите прибор');

    if (ADBVersion > 2) then
      raise Exception.Create('Версия ' + IntToStr(ADBVersion) +  ' базы в ПДУ не поддерживается. Обновите FireSec');

    Stream := TMemoryStream.Create;
    TmpStream := TMemoryStream.Create;
    PDUStream := TMemoryStream.Create;
    try

      cnt := 0;
      for I := 0 to High(Databases) do
        if Databases[i] <> nil then
          inc(cnt);

      {$IFDEF DEBUG} // ACTION
      WriteToLog(rkMessage, rtAction, 'WriteRemoteControl. Forming database...');
      {$ENDIF}
      case ADBVersion of
        1:
        begin
          Stream.Size := Sizeof(TLEDDBHeader);
          Stream.Position := Stream.Size;

          with PLEDDBHeader(Stream.Memory)^ do
          begin
            version := ADBVersion;
            CRC := 0;
            DBSize := 0;
            PanelCount := 0;
          end;

          SetLength(PanelHeadersV1, cnt);
          FillChar(PanelHeadersV1[0], cnt * Sizeof(TLEDPanelHeaderV3), 0);
          Stream.Write(PanelHeadersV1[0], cnt * Sizeof(PanelHeadersV1[0]));

          cnt := 0;
          for I := 0 to High(Databases) do
            if Databases[i] <> nil then
            begin
              with PLEDDBHeader(Stream.Memory)^ do
              begin
                inc(PanelCount);
                inc(cnt);
              end;

              TmpStream.Clear;

              CurHeaderV1 := PLedPanelHeaderV3(PChar(Stream.Memory) + Sizeof(TLEDDBHeader) + Sizeof(TLEDPanelHeaderV3) * (cnt - 1));
              CurHeaderV1.addr := i;
              CurHeaderV1.MD5 := Databases[i].GetDatabaseHash;
              CurHeaderV1.MD5Offset := IntegerToRawPointer(Databases[i].GetDatabaseHashAddr);
              doFormExecutives(i, Stream.Size, CurHeaderV1, TmpStream);
              doFormTM(i, Stream.Size, CurHeaderV1, TmpStream);

              Stream.CopyFrom(TmpStream, 0);
            end;

          with PLEDDBHeader(Stream.Memory)^ do
          begin
            DBSize := Stream.Size - Sizeof(TLEDDBHeader);
            CRC := swap(CCITT_CRC( (PChar(Stream.memory) + Sizeof(TLEDDBHeader))^,
              Stream.Size - Sizeof(TLEDDBHeader)));
          end;

          end;
        2:
        begin
          Stream.Size := Sizeof(TRCDBHeaderV2);
          Stream.Position := Stream.Size;

          with PRCDBHeaderV2(Stream.Memory)^ do
          begin
            version := ADBVersion;
            CRC := 0;
            DBSize := 0;
            PanelCount := 0;
            DeviceCount := Length(data) - 1;
          end;

          SetLength(PanelHeadersV2, cnt);
          FillChar(PanelHeadersV2[0], cnt * Sizeof(TRCPanelHeaderV2), 0);
          Stream.Write(PanelHeadersV2[0], cnt * Sizeof(PanelHeadersV2[0]));

          cnt := 0;
          for i := 0 to High(Databases) do
            if Databases[i] <> nil then
            begin
              with PRCDBHeaderV2(Stream.Memory)^ do
              begin
                inc(PanelCount);
                inc(cnt);
              end;

              TmpStream.Clear;

              CurHeaderV2 := PRCPanelHeaderV2(PChar(Stream.Memory) + Sizeof(TRCDBHeaderV2) + Sizeof(TRCPanelHeaderV2) * (cnt - 1));
              CurHeaderV2.addr := i;
              CurHeaderV2.MD5 := Databases[i].GetDatabaseHash;
              CurHeaderV2.MD5Offset := Databases[i].GetDatabaseHashAddr;
              CurHeaderV2.PanelType := DeviceTypes[i];
              Stream.CopyFrom(TmpStream, 0);
            end;

          doFormExecutives_V2(TmpStream, PDUStream, cnt);
          with PRCDBHeaderV2(Stream.Memory)^ do
            inc(PanelCount, cnt);
          Stream.CopyFrom(PDUStream, 0);

          // отвели под записи приборов
          b := $FF;
          while Stream.Size < $2CC do
            Stream.Write(b, 1);

          Stream.CopyFrom(TmpStream, 0);

          with PLEDDBHeader(Stream.Memory)^ do
          begin
            DBSize := Stream.Size - Sizeof(TRCDBHeaderV2);
            CRC := swap(CCITT_CRC( (PChar(Stream.memory) + Sizeof(TRCDBHeaderV2))^,
              Stream.Size - Sizeof(TRCDBHeaderV2)));
          end;

        end;

      end;

      if not IsRubezh3FileDB
      {$IFDEF SimulateWriteDB} and not SameText(Param, 'simulate') {$ENDIF} then
      begin
        {$IFDEF DEBUG} // OPERATION
        WriteToLog(rkMessage, rtOperation, 'WriteRemoteControl. Writing AddressList...');
        {$ENDIF}
//        Progress(0, rsWritingAddressList, 0);
        doWriteMDS(False);
        Progress(0, rsWritingAddressList, 100);

        doProgress(rsSettingFirmwareUpdateMode, True);
        if FInterface.GetDeviceState = 0 then
          // Переходим в режим обновления ПО
          try
              FInterface.BeginSoftUpdate(1);
          except
            // Игнорируем ошибку
          end;

        if FInterface.GetDeviceState and $01 = 0 then
          raise Exception.Create(rsUnableToSetFWUpdateMode);

        {$IFDEF DEBUG}
        WriteToLog(rkMessage, rtOperation, 'WriteRemoteControl. Writing database...');
        {$ENDIF}
        DoWriteDatabase(StartOffset, Stream.Size, Stream.Memory);
      end else
      {$IFDEF SimulateWriteDB} if not SameText(Param, 'simulate') then {$ENDIF}
        with TFileStream.Create(IncludeTrailingPathDelimiter(ExtractFilePath(ParamStr(0))) + 'TstData\'+ GetDeviceText(Device, [dtoNameFirst, dtoShortName, dtoParents]) +'.bin', fmCreate) do
        try
          Stream.Position := 0;
          CopyFrom(Stream, 0);
        finally
          Free;
        end;

    finally
      Stream.Free;
      TmpStream.Free;
    end;

    if not IsRubezh3FileDB
    {$IFDEF SimulateWriteDB} and not SameText(Param, 'simulate') {$ENDIF} then
    begin
      {$IFDEF DEBUG} // OPERATION
      WriteToLog(rkMessage, rtOperation, 'WriteIndicator. Resetting device...');
      {$ENDIF}
      doProgress(rsPanelReloading, True);
      FInterface.EndSoftUpdate_Reset;

      // Проверяем, что обновление успешно
      if not WaitPanelUserMode then
        raise Exception.Create(rsFWUpdateFailed);
    end;

  end;

  procedure doWriteRemoteControlFire;
  var
    Databases: array of IRubezh3Database;
    DeviceTypes: array of byte;
    ADBVersion: word;
    data: TRCFRecords;
    Addr: dword;

    function GetDatabase(const Panel: IDevice): IRubezh3Database;
    var
      Rubezh3DeviceDriver: IRubezh3DeviceDriver;
      s: string;
    begin
      // формируем базу для прибора и сохраняем ее в кэш
      if Length(Databases) = 0 then
        SetLength(Databases, MAX_PANEL_ADDRESS + 1);

      if Length(DeviceTypes) = 0 then
        SetLength(DeviceTypes, MAX_PANEL_ADDRESS + 1);

      assert(Panel.DeviceAddress <= MAX_PANEL_ADDRESS);

      if Databases[Panel.DeviceAddress] = nil then
      begin
        if not Supports(Panel.DeviceDriver, IRubezh3DeviceDriver, Rubezh3DeviceDriver) then
          raise Exception.Create('Неизвестный тип базы данных для указанного прибора');

        if Rubezh3DeviceDriver.GetDatabaseModelExt = nil then
          raise Exception.Create('Указанный прибор не имеет модели базы данных');

        {$IFDEF DBCache}
        {$Message Hint 'Добавить получение кешированной БД'}
        if Databases[Panel.DeviceAddress] = nil then
        begin
        {$ENDIF}
          Databases[Panel.DeviceAddress] := Rubezh3DeviceDriver.GetDatabaseModelExt.CreateDatabase_Memory;
          if Databases[Panel.DeviceAddress] = nil then
            raise Exception.Create('Не удалось создать базу данных для прибора');
          Databases[Panel.DeviceAddress].InitVirtual;
          Databases[Panel.DeviceAddress].WriteDatabase(Panel, TFakeInstance.Create(Rubezh3DeviceDriver), s);
        {$IFDEF DBCache}
        end;
        {$ENDIF}
        DeviceTypes[Panel.DeviceAddress] := Panel.DeviceDriver.GetBaseType;
      end;
      result := Databases[Panel.DeviceAddress];
    end;

    procedure SortRCRecords(var data: TRCFRecords);
    begin
      with TRCRecordsSort.Create(@Data) do
      try
        Sort;
      finally
        free;
      end;
    end;

    function GetWriteable(const Device: IDevice): IDevice;
    var
      P: IDevice;
    begin
      P := Device;
      result := nil;
      while P <> nil do
        if optDeviceDatabaseWrite in P.DeviceDriver.DriverOptions then
        begin
          result := P;
          break;
        end else
          P := P.ParentDevice;
    end;

    function IntegerToRawPointer(value: integer): TRawPointer;
    begin
      result[0] := value and $000000FF;
      result[1] := (value and $0000FF00) shr 8;
      result[2] := (value and $00FF0000) shr 16;
    end;

    function RawPointerToUnteger(value: TRawPointer): integer;
    begin
      result := (value[0])
             or (value[1] shl 8)
             or (value[2] shl 16);
    end;

    procedure DoRawMemWrite(Addr, ASize: Dword; Buffer: PChar; SetByteProgress: Boolean = False);
    var
      p: pchar;
      sent, tosend: dword;
    begin
      if SetByteProgress then
      begin
        FInterface.BytesTotal := ASize;
        FInterface.BytesComplete := 0;
      end;
      try

        p := Buffer;
        sent := 0;
        while sent < ASize do
        begin
          tosend := min(MAX_PROTOCOL_MEM_BLOCK_SIZE, ASize - sent);
          FInterface.RawPushBlock(Addr + sent, tosend, p^, True);
          FInterface.RawWriteRepeatable(Addr + sent, tosend, p^);
          inc(sent, tosend);
          inc(p, tosend);

          if SetByteProgress then
            FInterface.BytesComplete := sent;
        end;

      finally
        if SetByteProgress then
        begin
          FInterface.BytesTotal := 0;
          FInterface.BytesComplete := 0;
        end;
      end;

    end;

    procedure DoWriteDatabase(Addr, Size: longword; Buf: PChar);
    begin
      // Стираем память
      doProgress(rsClearingMemorySectors + ' ' + IntToStr(FInterface.GetMemBlockByAddress(Addr)) + ' - ' +
        IntToStr(FInterface.GetMemBlockByAddress(Addr+Size)), True);
      FInterface.MemClear(FInterface.GetMemBlockByAddress(Addr), FInterface.GetMemBlockByAddress(Addr+Size));

      Progress(0, rsWritingDatabase, 0);

      DoRawMemWrite(Addr, Size, Buf, False);
    end;

    procedure ProcessDevice(const Device: IDevice);

    var GroupData: TRCGroupData;
        j: integer;
        Zone: IZone;

      procedure AddDevice(const Panel, ADevice: IDevice);
      begin
        with GetDatabase(Panel) do
        begin
          SetLength(data, Length(data) + 1);
          with data[high(data)] do
          begin
            PanelAddr := Panel.DeviceAddress;
            j := GetDeviceIndex(GetDeviceType(ADevice), GetRealDeviceAddress(ADevice));
            if j = -1  then
              raise Exception.Create('Для ИУ ' + GetDeviceText(ADevice, [dtoNameFirst, dtoShortName]) +  ' не удалось определить индекс в БД');
            DeviceInGroup := ADevice;
            Addr := ReadExtDeviceStateAddr(GetDeviceType(ADevice), j);
            DelayOn := GetDevicePropDef(Device, 'RunDelay', 0);
            Offset := IntegerToRawPointer(Addr);
            full_addr := GetRealDeviceAddress(ADevice) - $100;
            ExecType := OldDeviceTypes[GetDeviceType(ADevice)];
            GroupNum := Device.DeviceAddress;
            IdRec := ltExecutive;
            with ADevice.EnumZones do
              if Next(Zone) = S_OK then
                ZoneNo := GetLocalZoneID(Panel, Zone);

          end;
        end;

      end;

    var
      Panel: IDevice;
      Child, SubChild: IDevice;
      i: integer;
    begin
      if HasDeviceProperty(Device.DeviceDriver, 'E98669E4-F602-4E15-8A64-DF9B6203AFC5') then
      begin
        GroupData := GetRCGroupData(Device, GetRootDevice(Device));
        if GroupData.DevCount > 0 then
          for i := 0 to GroupData.DevCount - 1 do
          begin
            Child := GroupData.Devices[i].Device;
            Panel := FS_SupportClasses.GetOwnerPanel(Child);
            if (Child = nil) or (Panel = nil) then continue;
            AddDevice(Panel, Child);
            with Child.EnumChildren do
              while Next(SubChild) = S_OK do
                AddDevice(Panel, SubChild);
          end;
      end;

      with Device.EnumChildren do
        while Next(Child) = S_OK do
          ProcessDevice(Child);

    end;

    procedure doFormExecutives(GroupNum: Integer; CurGroupHeader: PRCFGroupRecord; Stream: TMemoryStream);
    var
      l: integer;
      Rec: TRCFDevRecord;
      Zone: IZone;
    begin
      for l := 0 to High(data) do
        if (GroupNum = Data[l].GroupNum) and (Data[l].IdRec = ltExecutive) then
        begin
          FillChar(Rec, Sizeof(Rec), 0);
          Rec.offset := RawPointerToUnteger(Data[l].offset);
          Rec.full_addr := data[l].full_addr;
          Rec.PanelAddr := data[l].PanelAddr;
          Rec.GroupNum := data[l].GroupNum;
          Rec.ExecType := data[l].ExecType;
          Rec.offset := RawPointerToUnteger(data[l].offset);
          Rec.ZoneNo := data[l].ZoneNo;

          CurGroupHeader.CountDev := CurGroupHeader.CountDev + 1;

          // задержку берем у ведущего МПТ
          if data[l].DeviceInGroup.EnumZones.Next(Zone) <> S_OK then
            if (data[l].DeviceInGroup.ParentDevice <> nil) then
              data[l].DeviceInGroup.ParentDevice.EnumZones.Next(Zone);

          if Zone <> nil then
          begin
            CurGroupHeader.Delay := GetZonePropDef(Zone, 'ExitTime', 0);
            CurGroupHeader.Options := GetZonePropDef(Zone, 'ExitRestoreType', 0);
          end;

          Stream.WriteBuffer(Rec, Sizeof(Rec));
        end;
    end;

    function CountDevicesInGroup(GroupNo: integer): integer;
    var i: integer;
    begin
      result := 0;
      for i := 0 to Length(data) - 1 do
        if data[i].GroupNum = GroupNo then
          inc(result);
    end;

  var
    TmpStream,Stream: TMemoryStream;
    i, cnt: integer;
    b: byte;
    GroupHeaders: packed array of TRCFGroupRecord;
    PanelHeaders: packed array of TRCFPanelHeader;
    CurPanelHeader: PRCFPanelHeader;
    CurGroupHeader: PRCFGroupRecord;
    DataBlock: IDataBlock;
    StartOffset: integer;
  begin

    {$IFDEF SimulateWriteDB}
    if SameText(Param, 'simulate') then
      exit;
    {$ENDIF}

    if not IsRubezh3FileDB
    {$IFDEF SimulateWriteDB} and not SameText(Param, 'simulate') {$ENDIF} then
    begin
      if not (FInterface.SendPing(DataBlock) = crAnswer) then
        raise Exception.Create(rsErrorUSBNoDevice);

      Progress(0, rsWritingAddressList, 0);

      try
        i := FInterface.GetDeviceState;
      except
        i := -1;
      end;

      if i <> 0 then
        // Переходим в режим пользовательского ПО
        try
            FInterface.EndSoftUpdate_Reset;
            sleep(500);
        except
          // Игнорируем ошибку
        end;

      if not WaitPanelUserMode then
        raise Exception.Create(rsFWUpdateFailed);
    end;

    {$IFDEF DEBUG} // ACTION
    WriteToLog(rkMessage, rtAction, 'WriteRemoteControl. Preparing database...');
    {$ENDIF}
    Progress(0, rsPreparingDatabase, 0);

    ProcessDevice(Device);
    // сортируем список по адресу устройства
//    SortRCRecords(data);

    // Добавляем пустую
    SetLength(data, length(data) + 1);
    FillChar(data[high(data)], Sizeof(TRCRecord), 0);

    StartOffset := INDICATOR_DB_START_OFFSET;  // начальный адрес базы в БИ и ПДУ -  0x4000

    {$IFDEF DEBUG} // OPERATION
    WriteToLog(rkMessage, rtOperation, 'WriteRemoteControl. Get database version...');
    {$ENDIF}
    if not IsRubezh3FileDB
    {$IFDEF SimulateWriteDB} and not SameText(Param, 'simulate') {$ENDIF} then
      ADBVersion := swap(FInterface.GetDatabaseVersion) else
      ADBVersion := 1;

    if (ADBVersion <= 0) then
      raise Exception.Create('Версия ' + IntToStr(ADBVersion) +  ' базы в ПДУ не поддерживается. Обновите прибор');

    if (ADBVersion > 1) then
      raise Exception.Create('Версия ' + IntToStr(ADBVersion) +  ' базы в ПДУ не поддерживается. Обновите FireSec');

    Stream := TMemoryStream.Create;
    TmpStream := TMemoryStream.Create;

    try
      cnt := 0;
      for I := 0 to High(Databases) do
        if Databases[i] <> nil then
          inc(cnt);

      {$IFDEF DEBUG} // ACTION
      WriteToLog(rkMessage, rtAction, 'WriteRemoteControl. Forming database...');
      {$ENDIF}
      case ADBVersion of
        1:
        begin
          Stream.Size := Sizeof(TRCFDBHeader);
          Stream.Position := Stream.Size;

          with PRCFDBHeader(Stream.Memory)^ do
          begin
            version := ADBVersion;
            CRC := 0;
            DBSize := 0;
            PanelCount := 0;
            HashVersion := $01;
            FillChar(Hash, SizeOf(Hash), $FF);
            FillChar(Reserved, SizeOf(Reserved), $FF);
          end;

          // пошем записи приборов

          SetLength(PanelHeaders, cnt);
          FillChar(PanelHeaders[0], cnt * Sizeof(TRCFPanelHeader), 0);
          Stream.Write(PanelHeaders[0], cnt * Sizeof(PanelHeaders[0]));

          cnt := 0;
          for I := 0 to High(Databases) do
            if Databases[i] <> nil then
            begin
              with PRCFDBHeader(Stream.Memory)^ do
              begin
                inc(PanelCount);
                inc(cnt);
              end;

              CurPanelHeader := PRCFPanelHeader(PChar(Stream.Memory) + Sizeof(TRCFDBHeader) + Sizeof(TRCFPanelHeader) * (cnt - 1));
              CurPanelHeader.addr := i;
              CurPanelHeader.MD5 := Databases[i].GetDatabaseHash;
              CurPanelHeader.MD5Offset := Databases[i].GetDatabaseHashAddr;
              CurPanelHeader.PanelType := DeviceTypes[i];
            end;

          // запись направлений начинается с 0x430C
          b := $FF;
          Stream.Position := Stream.Size;
          for i := Stream.Size to $30C do
            Stream.Write(b, 1);

          // пишем записи направлений

          cnt := 0;
          for i := 1 to 5 do
            if CountDevicesInGroup(i) > 0 then
              inc(cnt);

          SetLength(GroupHeaders, cnt);
          FillChar(GroupHeaders[0], cnt * Sizeof(TRCFGroupRecord), 0);
          Stream.Write(GroupHeaders[0], cnt * Sizeof(GroupHeaders[0]));

          // запись устройств начинается с 0x4334
          b := $FF;
          Stream.Position := Stream.Size;
          for i := Stream.Size to $333 do
            Stream.Write(b, 1);

          with PRCFDBHeader(Stream.Memory)^ do
            GroupCount := cnt;

          cnt := 0;
          for I := 1 to 5 do
          begin
            if CountDevicesInGroup(i) > 0 then
            begin
              Inc(cnt);
              TmpStream.Clear;

              CurGroupHeader := PRCFGroupRecord(PChar(Stream.Memory) +
                $30C + Sizeof(TRCFGroupRecord) * (cnt - 1));

              CurGroupHeader.addr := i;
              CurGroupHeader.offset := StartOffset + Stream.Size;

              doFormExecutives(i, CurGroupHeader, TmpStream);

              Stream.CopyFrom(TmpStream, 0);
            end;
          end;

          with PRCFDBHeader(Stream.Memory)^ do
          begin
            DBSize := Stream.Size - Sizeof(TRCFDBHeader);
            CRC := swap(CCITT_CRC( (PChar(Stream.memory) + Sizeof(TRCFDBHeader))^,
              Stream.Size - Sizeof(TRCFDBHeader)));
          end;

        end;
      end;

      if not IsRubezh3FileDB
      {$IFDEF SimulateWriteDB} and not SameText(Param, 'simulate') {$ENDIF} then
      begin
        {$IFDEF DEBUG} // OPERATION
        WriteToLog(rkMessage, rtOperation, 'WriteRemoteControl. Writing AddressList...');
        {$ENDIF}
//        Progress(0, rsWritingAddressList, 0);
        doWriteMDS(False);
        Progress(0, rsWritingAddressList, 100);

        doProgress(rsSettingFirmwareUpdateMode, True);
        if FInterface.GetDeviceState = 0 then
          // Переходим в режим обновления ПО
          try
              FInterface.BeginSoftUpdate(1);
          except
            // Игнорируем ошибку
          end;

        if FInterface.GetDeviceState and $01 = 0 then
          raise Exception.Create(rsUnableToSetFWUpdateMode);

        {$IFDEF DEBUG}
        WriteToLog(rkMessage, rtOperation, 'WriteRemoteControl. Writing database...');
        {$ENDIF}
        DoWriteDatabase(StartOffset, Stream.Size, Stream.Memory);
      end else
      {$IFDEF SimulateWriteDB} if not SameText(Param, 'simulate') then {$ENDIF}
        with TFileStream.Create(IncludeTrailingPathDelimiter(ExtractFilePath(ParamStr(0))) + 'TstData\'+ GetDeviceText(Device, [dtoNameFirst, dtoShortName, dtoParents]) +'.bin', fmCreate) do
        try
          Stream.Position := 0;
          CopyFrom(Stream, 0);
        finally
          Free;
        end;

    finally
      Stream.Free;
      TmpStream.Free;
    end;

    if not IsRubezh3FileDB
    {$IFDEF SimulateWriteDB} and not SameText(Param, 'simulate') {$ENDIF} then
    begin
      {$IFDEF DEBUG} // OPERATION
      WriteToLog(rkMessage, rtOperation, 'WriteIndicator. Resetting device...');
      {$ENDIF}
      doProgress(rsPanelReloading, True);
      FInterface.EndSoftUpdate_Reset;

      // Проверяем, что обновление успешно
      if not WaitPanelUserMode then
        raise Exception.Create(rsFWUpdateFailed);
    end;

  end;

{$ENDIF}

  procedure doWritePanel;
  var DataBlock: IDataBlock;
      CurHash, PanelHash: TMD5Digest;
  begin
    if {$IFDEF SimulateWriteDB} SameText(Param, 'simulate') or {$ENDIF}
    IsRubezh3FileDB or (FInterface.SendPing(DataBlock) = crAnswer) then
    begin
      if SameText(Param, 'VerifyDBHash') then
      begin
        CurHash := CalculateConfigMD5(Device, CurrentHashVersion);
        PanelHash := GetDatabase.GetDatabaseHash;
        if not SameDigest(CurHash, PanelHash) then
          GetDatabase.WriteDatabase(Device, Self, ResultStr, Param);
      end else
        GetDatabase.WriteDatabase(Device, Self, ResultStr, Param);
    end else
      raise Exception.Create(rsErrorUSBNoDevice);
  end;

begin

  try
    case FPanelDeviceMod.DeviceSubtype of
//      dsMDS: doWriteMDS;
      dsMDS, dsMDS5: DoWriteMDS345;
      dsIndicator: DoWriteIndicator;
{$IFDEF RubezhRemoteControl}
      dsRemoteControl: DoWriteRemoteControl;
      dsRemoteControlFire: DoWriteRemoteControlFire;
{$ENDIF}
      dsOrdinaryPanel: doWritePanel
    end;
  except
    on E: Exception do
    begin
      {$IFDEF DEBUG} // EXCEPT
      WriteToLog(rkMessage, rtExcept, 'SetDevices exception: ' + E.Message);
      {$ENDIF}
      raise;
    end;
  end;

end;

procedure TRubezh3Device.SetTime(Value: TDateTime);
begin
  FInterface.DateTimeSet(Value);
  LogEvent(rsSettingTimeFromPC);
end;

function IsEmptyGUID(const guid: TGUID): Boolean;
var
  a: PIntegerArray;
begin
  a := PIntegerArray(@guid);
  Result :=
    ((a^[0] = -1) and (a^[1] = -1) and (a^[2] = -1) and (a^[3] = -1)) or
      ((a^[0] = 0) and (a^[1] = 0) and (a^[2] = 0) and (a^[3] = 0));
end;


function FilesExists(const FileMask: string) : Boolean;
var
  SearchRec : TSearchRec ;
  RetFind   : Integer ;
  LastFile  : string ;
begin
  LastFile := '' ;
  Result   := false ;
  RetFind  := SysUtils.FindFirst(FileMask, faAnyFile, SearchRec) ;
  try
     while (not Result) and (RetFind = 0) and (LastFile <> SearchRec.Name) do
     begin
        LastFile := SearchRec.Name ;
        Result   := (pos(LastFile, '..') = 0) ;   
        SysUtils.FindNext(SearchRec) ;
     end ;
  finally
     SysUtils.FindClose(SearchRec) ;
  end;
end;

function TRubezh3Device.GetNewRecords(SysLastFireIndex: Integer; SysLastSecIndex: Integer;
  AllRecords: Boolean; var FireOverflow: Boolean; var SecOverflow: Boolean; var ResultLastFireIndex: Variant; var ResultLastSecIndex: Variant): IEnumLogRecords;

  var
    CurrentAddr: Integer;
    CurLogType: byte;

  procedure DoLogNewDevice(CompositeIndex: Integer; IsIndex: Boolean; SubSyst: TEventSubSystem);
  begin
    with LogDeviceActionNewEvent^ do
    begin
      EventClass := dscNormal;
      if IsIndex then
        EventMessage := rsNewSerialFound + ' ' + rsResetLastIndex else
        EventMessage := rsNewSerialFound;
      DateTime := Now;
      SysDateTime := Now;
{$IFNDEF DISABLE_SECURITY}
      case SubSyst of
        essFire:     RecordFireIndex := CompositeIndex;
        essSecurity: RecordSecIndex := CompositeIndex;
      end;
{$ELSE}
      RecordFireIndex := CompositeIndex;
{$ENDIF}
      DeviceUID := FUID;
    end;
  end;

  procedure DoReceiveFailIndex(SubSyst: TEventSubSystem);
  begin
    with LogDeviceActionNewEvent^ do
    begin
      EventClass := dscNormal;
      DateTime := Now;
      SysDateTime := Now;
      case SubSyst of
        essFire:     EventMessage := rsReceiveFailFireIndex;
        essSecurity: EventMessage := rsReceiveFailSecIndex;
      end;
      DeviceUID := FUID;
    end;
  end;

  function IsUIDChanged: Boolean;
  var
    UID, UID2: TGUID;
    i: integer;
  begin
    result := False;

    if FUIDReaded and ((Now - FLastUIDCheck) > UID_CHANGE_CHECK_PERIOD) then
    begin
      GetDatabase.GetPanelUID(UID);

      for i := 0 to 15 do
      begin
        GetDatabase.GetPanelUID(UID2);
        if not IsEqualGUID(UID2, UID) then
        begin
          FUIDChanged := True;
          raise ERubezh3UserError.Create(rsProbableDuplicateAddresses, 0);
        end;
      end;


      FUIDChanged := False;

      if not IsEqualGUID(UID, FUID) then
      begin
        result := True;
        if IsEmptyGUID(UID) then
          FUIDReaded := False else
          FUID := UID;
      end;

//      UpdateReservedLinesStates;

      FLastUIDCheck := Now;
    end;
  end;

  procedure DoSetStateByID(const Device: IDeviceInstance; StateID: Integer; StateStartTime: double);
  var
    State: IDeviceState;
  begin
    // 31.03.2011 переопределил состояния для АМ1-О в XML. Роман
    if ((StateID in [0, 4]) and (Device.DeviceDriver.DeviceDriverID = 'EFCA74B2-AD85-4C30-8DE8-8115CC6DFDD2')) then
    begin
      if StateID = 0 then
        State := Device.DeviceDriver.FindStateByCode('ConnectionLost')
      else if StateID = 4 then
        State := Device.DeviceDriver.FindStateByCode('Malfunction_am4o')
    end else
      State := Device.DeviceDriver.GetStateByID(StateID);

    // Для ППУ
    if Device.DeviceDriver.DeviceDriverID = sddPPU then
      DoSetAltState(Device, State, Lo(CurrentAddr) - Lo(Device.DeviceAddress), PPUAltStates, StateStartTime) else
      InstanceSetState(Device, State, StateStartTime);
  end;

  procedure DoResetStateByID(const Device: IDeviceInstance; StateID: Integer);
  var
    State: IDeviceState;
  begin
    // 31.03.2011 переопределил состояния для АМ1-О в XML. Роман
    try
    if ((StateID in [0, 4]) and (Device.DeviceDriver.DeviceDriverID = 'EFCA74B2-AD85-4C30-8DE8-8115CC6DFDD2')) then
    begin
      if StateID = 0 then
        State := Device.DeviceDriver.FindStateByCode('ConnectionLost')
      else if StateID = 4 then
        State := Device.DeviceDriver.FindStateByCode('Malfunction')
    end else
      State := Device.DeviceDriver.GetStateByID(StateID);
    except // если состояние для устройства не найдено (например Тревога для АМ1-Т просто игнорим)
      exit;
    end;

    // Для ППУ
    if Device.DeviceDriver.DeviceDriverID = sddPPU then
      DoResetAltState(Device, State, Lo(CurrentAddr) - Lo(Device.DeviceAddress), PPUAltStates) else
      (Device as IStatefull).ResetState(State);
  end;

  procedure SetChildStates(const Device: IDeviceInstance; var StateArray: TStateArray; bSetState: Boolean);
  var
    Child: IDeviceInstance;
    i, j: integer;
  begin
    if StateArray <> nil then
    begin
      with Device.EnumChildren do
        while Next(Child) = S_OK do
        begin
          for i:=Low(StateArray) to High(StateArray) do
            if IsStateComaptibleWithDevice(arDeviceStates[StateArray[i]], Child) { and
              not (((HiByte(Child.DeviceAddress) = 1) and FLine1Fail) or ((HiByte(Child.DeviceAddress) = 2) and FLine2Fail)) } then
            begin
              if Child.DeviceDriver.DeviceDriverID = sddPPU then
                for j := 0 to 6 do
                begin
                  CurrentAddr := Child.DeviceAddress + j;
                  if bSetState then
                    DoSetStateByID(Child, StateArray[i], 0) else
                    DoResetStateByID(Child, StateArray[i]);
                end else
                  if bSetState then
                    DoSetStateByID(Child, StateArray[i], 0) else
                    DoResetStateByID(Child, StateArray[i]);
            end;
          SetChildStates(Child, StateArray, bSetState);
        end;
    end;
  end;

  procedure CheckStates(PanelState: int64);
  var
    i, j: Integer;
    state1, state2 : dword;
  begin
    state1 := PanelState;                 // Делим int64 на 2 dword т.к. не верно
    state2 := PanelState div $100000000;  // работает смещение с номером бита > 31
    for i := Low(arPanelStates) to High(arPanelStates) do
    if arPanelStates[i].Name <> '' then
    begin
      if (i < 32) and (state1 and (1 shl arPanelStates[i].BitNo) <> 0) then
      begin
        Statefull.SetStateByID(GetDeviceDriver, i);
        SetChildStates(Self, arPanelStates[i].EnterDeviceStates, True);
      end else
      if (i >=32) and (state2 and (1 shl arPanelStates[i].BitNo) <> 0) then
      begin
        Statefull.SetStateByID(GetDeviceDriver, i);
        SetChildStates(Self, arPanelStates[i].EnterDeviceStates, True);
      end else
      begin
        Statefull.ResetStateByID(GetDeviceDriver, i);

        // Не сбрасываем потерю для неисправности шлейфов
        for j := 0 to FPanelDeviceMod.AVRCount * 2 - 1 do
          if not FLineStates[j] then
            continue;

{        if not ((Fline1Fail or FLine2Fail) and (arPanelStates[i].BitNo = 0)) then }
        SetChildStates(Self, arPanelStates[i].LeaveDeviceStates, False);
      end;
    end;
  end;

  var
    TotalRecords: Integer;
    ReadedRecords: Integer;

  procedure NotifyReadedRecord(RecNo: Integer);
  begin
    if ((ReadedRecords mod 20) = 0) then
    begin
      if Statefull.HasStateID(GetDeviceDriver, devInitializing) then
      begin
        if ReadedRecords = 0 then
        Progress(0, IntToStr(Device.DeviceAddress) + ' - ' + FPanelDeviceMod.DeviceShortName +
          '. ' + rsReadingEvents + '. ' + rsTotal+ ': ' +
            IntToStr(TotalRecords) , 0, 0) else
        Progress(0, IntToStr(Device.DeviceAddress) + ' - ' + FPanelDeviceMod.DeviceShortName +
          '. ' + rsReadingEvents + '. ' + rsReaded + IntToStr(ReadedRecords)+' ' + rsTotal+ ': ' +
            IntToStr(TotalRecords) , 0, 0);
      end;

      FInterface.doTimeSlice;

    end;
    inc(ReadedRecords);
  end;

  procedure ChangeStates(var States: TStateArray; bSetState: Boolean;
    const Device: IDeviceInstance; ParentAlso: Boolean; StateStartTime: Double);
  var
    i: integer;
    s: string;
  begin
    s:= Device.DeviceDriver.DriverName;
    for i := Low(States) to High(States) do
      with (Device as IStatefull) do
      begin
        // states[i] - фактически индекс в массиве arDeviceStates
        if IsStateComaptibleWithDevice(arDeviceStates[states[i]], Device) then
        begin
          if bSetState then
          begin
            if ParentAlso and (Device.ParentInstance <> nil) then
              with (Device.ParentInstance as IStatefull) do
                SetStateByID(Device.ParentInstance.DeviceDriver, states[i]);

            DoSetStateByID(Device, states[i], StateStartTime); //            SetStateByID(Device.DeviceDriver, states[i]);
            if states[i] = FCSIndex then
              SetFailureType(Device, rsConnectionLost);
          end else
          begin
            if (states[i] = FCSIndex) or (states[i] = FMLIndex) then
              SetFailureType(Device, '');
            DoResetStateByID(Device, states[i]); //          ResetStateByID(Device.DeviceDriver, states[i]);

            if ParentAlso and (Device.ParentInstance <> nil) then
              with (Device.ParentInstance as IStatefull) do
                ResetStateByID(Device.ParentInstance.DeviceDriver, states[i]);
          end;
        end;
      end;
  end;

  procedure ZoneChangeStates(var States: TStateArray; bSetState: Boolean;
    const Zone: IZone; StateStartTime: Double);
  var
    Device: IDevice;
    ZDeviceInstance: IDeviceInstance;
  begin
    with Zone.EnumDevices(nil) do
      while next(Device) = S_OK do
        if GetOwnerPanel(Device) = Self.Device then
        begin
          ZDeviceInstance := FindDeviceInstance(Self, Device);
          if ZDeviceInstance <> nil then
            ChangeStates(States, bSetState, ZDeviceInstance, False, StateStartTime);
        end;
  end;
{$IFNDEF DISABLE_SECURITY}
  var
    SubSyst: TEventSubSystem;
{$ENDIF}

{$IFDEF Debug_DUMPMEM}
  procedure SaveDump;

    function RawDateTimeToDateTimeStr(Value: TRawVerSoft): string;
    var
      RawDate: dword;
      Y, M, D, H, Min, S: integer;
    begin
      RawDate := Value[0] + Value[1] shl 8 + Value[2] shl 16 + Value[3] shl 24;
      D := Byte(RawDate) and $1F;
      M := Byte(RawDate shr 05) and $0F;
      Y := Byte(RawDate shr 09) and $2F + 2000;
      H := Byte(RawDate shr 15) and $1F;
    Min := Byte(RawDate shr 20) and $2F;
      S := Byte(RawDate shr 26) and $1F;

      result := IntToStr(D) + '.' +
                IntToStr(M) + '.' +
                IntToStr(Y) + ' ' +
                IntToStr(H) + ':' +
                IntToStr(Min) + ':' +
                IntToStr(S) +
                Char(Value[4]);
    //  result := D or (M shl 5) or (Y shl 9) or (H shl 15) or (Min shl 20) or (S shl 26);
    end;

  var Path, FileName: string;
      ASize: dword;
      s: String;
      MemDumpID: byte;
      AStream: TStream;
      FileStream: TFileStream;
      cDate: TRawVerSoft;
  begin
    if not (FPanelDeviceMod.BaseNodeType in [4,5,6]) then exit;

    try
      FInterface.MemRead($3FFFF, SizeOf(MemDumpID), @MemDumpID);
    except
      FMemDumpID := 0;
      Exit;
    end;

    if MemDumpID <> FMemDumpID then
      FMemDumpID := MemDumpID else
      Exit;

    Path := IncludeTrailingPathDelimiter(ExtractFilePath(ParamStr(0))) + 'Dumps\';
    if not DirectoryExists(Path) then
      CreateDirectory(pchar(Path),0);

    FileName := Path + Device.DeviceDriver.DriverShortName + '_' + IntTostr(Device.DeviceAddress) + '_' +
      IntToHex(MemDumpID, 2);

    // Пропускаем чтение если дамп c таким номером уже есть
    if FilesExists(FileName + '_*.hex') then
      exit;

    FileName := FileName + '_' + FormatDateTime('DD_MM_YYYY_HH_MM',Now);

    FileStream := TFileStream.Create(FileName + '.hex', fmCreate or fmOpenWrite);
    with FileStream do
    try
      s := GetDump(1, ASize);
      AStream := TStringStream.Create(s);
      try
        BIN2IntelHex($40000000, AStream, FileStream, False);
      finally
        AStream.Free;
      end;

      s := GetDump(2, ASize);
      AStream := TStringStream.Create(s);
      try
        BIN2IntelHex($7FE00000, AStream, FileStream);
      finally
        AStream.Free;
      end;

    finally
      free;
    end;

    FileStream := TFileStream.Create(FileName + '.txt', fmCreate or fmOpenWrite);
    with FileStream do
    try
      s := 'Версия FireSec: ' + FileVersion(Paramstr(0));
      s := s + #13#10 + GetDescriptionString;
      FInterface.MemRead($8007CFB7, SizeOf(cDate), @cDate);
      s := s + #13#10 + 'Дата/время прошивки: ' + RawDateTimeToDateTimeStr(cDate);

//      for i := 0 to Length(cDate) - 1 do
//        s := s + IntToHex(cDate[i], 2) + ' ';
      Write(Pointer(s)^, Length(s));
    finally
      free;
    end;

  end;
{$ENDIF}

  function ReadLogRecord(LogType: integer; CompositeIndex: Integer): TRawEventRecord32;
  {$IFDEF DEBUG} var StartTime: TDateTime; {$ENDIF}
  begin
    {$IFDEF DEBUG}
    StartTime := Now;
    WriteToLog(rkMessage, rtDB_Transfer, 'Reading record No '+IntToHex(CompositeIndex, 8));
    {$ENDIF}

    if FInterface.Model.IsEventLog32 then
      FInterface.EventLogReadFullRecord32(LogType, CompositeIndex, result) else
      FInterface.EventLogReadFullRecord16(LogType, CompositeIndex, result.Record16);

    {$IFDEF DEBUG}
    WriteToLog(rkMessage, rtDB_Transfer, 'Record readed. Time: ' + FormatFloat('#.###', (Now-StartTime) * 24 * 60 * 60) + ' сек.');
    {$ENDIF}
  end;

  procedure ParseLogRecord(CompositeIndex: integer; LogRecord: TRawEventRecord32);

    procedure QueueRead(const Panel: IDeviceInstance; DevAddress: Integer; DevType: TRubezh3DeviceType; NoDelay: boolean);
    var
      RubezhInternal: IRubezhInternal;
    begin
      if Supports(Panel, IRubezhInternal, RubezhInternal) then
        RubezhInternal.QueueDatabaseRead(DevAddress, DevType, NoDelay);
    end;

    procedure UpdateGuardDeviceStatesOnZone(AZone: IZone; LogRec: TRawEventRecord32);
    var child: IDevice;
        DeviceInstance: IDeviceInstance;
        DeviceState: IDeviceState;
    begin
      with Device.EnumChildren do
        while Next(Child) = S_OK do
          if Child.InZone(AZone) then
//            if GetDevicePropDef(Child,'GuardType',-1) <> 4 then // 4 - тревожная кнопка
//            13.09.2011 тревожная кнопка ведется себя как и остальные типы АМ1-О
//            за функций тревожной кнопки отвечает зона с типом "без права снятия"
            begin
              DeviceState := Child.DeviceDriver.FindStateByCode('OnGuard');
              DeviceInstance := FindDevice(GetDeviceDriver.GetDeviceRegistry, Self, Child.DeviceDriver.DeviceDriverID, Child.DeviceAddress);

              if LogRec.Record16.EventCode = $31 then
              begin
                if LogRec.Record16.Context[0] = 0 then // постановка зоны на охрану
                  DoSetStateByID(DeviceInstance, DeviceState.GetStateID, Now);
                if LogRec.Record16.Context[0] = 1 then // снятие зоны с охраны
                  DoReSetStateByID(DeviceInstance, DeviceState.GetStateID);
                // Снятие неудачной постановки
                DoReSetStateByID(DeviceInstance, Child.DeviceDriver.FindStateByCode('FailSetOnGuard').GetStateID);
              end else
              if LogRec.Record16.EventCode = $45 then // снятие тревоги
                DoReSetStateByID(DeviceInstance, DeviceState.GetStateID);
            end;
    end;

    function GetUserName(UserIndex: integer): string;
    var PartitionsList: IPartitionList;
    begin
       PartitionsList := GetGuardUsersOnDevice(GetDeviceConfigFromDevice(Device).GetPartitionList,Device);
      // Здесь надо будет сделать проверку соответствия списка пользователей системы
      // с прибором
      if UserIndex = 0 then
      begin
        result :='';
        exit;
      end;
      if PartitionsList.Count >= UserIndex then
        result := IPartition(PartitionsList[UserIndex-1]).PartitionName
      else result :='';
    end;

  var
//    LogRecord: TRawEventRecord32;
    DeviceInfo: PRawEventDeviceInfo;
    EventLogGateway: PEventLogGateway;
    DBTablegateway, AltGateway: PDBTableGateway;
    {$IFDEF DEBUG}
    StartTime: TDateTime;
    {$ENDIF}
    EventRecord: PEventRecord;
    indicator, realind, offset: Shortint;
    s: string;
    Zone: IZone;
    addr, i, ZoneID: integer;
    EvDateTime: TDateTime;
    RealPanel, PDevice: IDeviceInstance;

  begin
    CurrentAddr := 0;

    {$IFDEF DEBUG}
    StartTime := Now;
    WriteToLog(rkMessage, rtDB_Work, 'Start parse record No '+IntToHex(CompositeIndex, 8));
    {$ENDIF}
(*
    if FInterface.Model.IsEventLog32 then
      FInterface.EventLogReadFullRecord32(CurLogType, CompositeIndex, LogRecord) else
      FInterface.EventLogReadFullRecord16(CurLogType, CompositeIndex, LogRecord.Record16);
*)
    { Пропускаем записи старше часа }
(*  убрано это работает только для 10а и 2а
    try
      EvDateTime := RawDateTimeToDateTime(LogRecord.Record16.EventTime);
    except
      EvDateTime := Now;
    end;

    if Now - EvDateTime > OneHour then
    begin
      {$IFDEF DEBUG}WriteToLog(rkMessage, rtDB_Transfer, 'Old record skipped: SubSystem:'+ IntToStr(Integer(SubSyst)) +' CNo '+IntToHex(CompositeIndex, 8));{$ENDIF}
      case SubSyst of
        essFire: ResultLastFireIndex := CompositeIndex;
        essSecurity: ResultLastSecIndex := CompositeIndex;
      end;
      exit;
    end;
*)

//    if not CheckLogRecordCRC(LogRecord) then

// перенесено ниже
//    {$IFDEF DEBUG}WriteToLog(rkMessage, rtDB_Transfer, 'Record readed. Time: ' + FormatFloat('#.###', (Now-StartTime) * 24 * 60 * 60) + ' сек.');{$ENDIF}

    NotifyReadedRecord(CompositeIndex);

    try
      EventLogGateway := GetEventLogGatewayList3.FindByRawEventCode(LogRecord.Record16.EventCode);
      if EventLogGateway <> nil then
      begin
        EventRecord := LogDeviceActionNewEvent;
        with EventRecord^ do
        begin
          DeviceUID := FUID;
{$IFNDEF DISABLE_SECURITY}
          Case CurLogType of
            LOG_FIRE_TYPE_16byte, LOG_FIRE_TYPE_32byte:
              begin
                EventSubSystem := essFire;
                RecordFireIndex := CompositeIndex;
                RecordSecIndex := -1;
              end;
            LOG_SEC_TYPE_16byte, LOG_SEC_TYPE_32byte:
              begin
                EventSubSystem := essSecurity;
                RecordFireIndex := -1;
                RecordSecIndex := CompositeIndex;
              end;
          end;
{$ELSE}
          RecordFireIndex := CompositeIndex;
{$ENDIF}
          indicator := GetEventIndicatorValue(EventLogGateway, LogRecord.Record16);
          EventClass := GetEventClass(EventLogGateway, LogRecord.Record16);
          EventMessage := ParseEventLogMessage(EventLogGateway.EventMessage, Indicator);

          try
            DateTime := RawDateTimeToDateTime(LogRecord.Record16.EventTime);
          except
            DateTime := Now;
          end;
          SysDateTime := Now;

{$IFDEF DEBUG}
          WriteToLog(rkMessage, rtDB_Transfer, 'Record readed. Time: ' + FormatFloat('#.###', (Now-StartTime) * 24 * 60 * 60) + ' сек.' +
          'Device Time: ' + DateTimeToStr(DateTime) + ' SystemTime: ' + DateTimeToStr(SysDateTime) +
          ' DeltaTime: ' + FormatFloat('#.###', (SysDateTime-DateTime) * 24 * 60 * 60) + ' сек.');
{$ENDIF}

          if (LogRecord.Record16.EventCode = $80) and (LogRecord.Record16.Context[1] <> 0) then
          begin
            EventMessage := rsInputFailure;
            if GetEventIndicatorValue(EventLogGateway, LogRecord.Record16) = 0 then
              EventMessage := EventMessage + ' ' + rsFailureSolved;
          end;

          offset := GetEventOffset(EventLogGateway, otDevice);
          if offset <> -1  then
          begin
            DeviceInfo := @LogRecord.Record16.Context[offset];

            DBTablegateway := GetDBTableGatewayByEvent(FInterface.Model, EventLogGateway, @LogRecord, addr, s, realind,
              @AltGateway);
            EventClass := GetEventClassByVal(EventLogGateway, realind);

            if s <> '' then
              EventMessage := s;

            if DBTablegateway <> nil then
            begin
              // Учитываем, что девайс может быть внешним
              if (DBTablegateway.DeviceClassID <> sdcRealASPTV3)
              and (DBTablegateway.DeviceClassID <> sdcOutputV3)
              and (FInterface.Model.GetAddress(atSystem, DeviceInfo.LocalAddress, DeviceInfo.SystemAddress, LogRecord.Ext) <> 0) then
                RealPanel := FindDevice(GetDeviceDriver.GetDeviceRegistry, Self.ParentInstance, '',
                  FInterface.Model.GetAddress(atSystem, DeviceInfo.LocalAddress, DeviceInfo.SystemAddress, LogRecord.Ext)) else
                RealPanel := Self;

              PDevice := RealPanel;

              // Учитваем что насосы вложены в НС
              if (PDevice <> nil) and (DeviceInfo.DeviceType = $70) and
                (PDevice.DeviceDriver.DeviceDriverID <> 'AF05094E-4556-4CEE-A3F3-981149264E89') then
                  PDevice := FindDevice(GetDeviceDriver.GetDeviceRegistry, PDevice, 'AF05094E-4556-4CEE-A3F3-981149264E89', 0);

              if PDevice <> nil then
                Device := FindDevice(GetDeviceDriver.GetDeviceRegistry, PDevice,
                  DBTableGateway.DeviceDriverID, addr);

              if (Device = nil) and (LogRecord.Record16.EventCode <> 08) then
              begin
                FDelayedSynchronizationFailed := True;

                {$IFDEF DEBUG}
                WriteToLog(rkMessage, rtDB_Error, 'Не удалось найти соотв. устройство при чтении журнала. ' +
                  ' EventCode: 0x' + IntToHex(LogRecord.Record16.EventCode, 2) +
                  ' SysAddr: ' + IntToStr(FInterface.Model.GetAddress(atSystem, DeviceInfo.LocalAddress, DeviceInfo.SystemAddress, LogRecord.Ext)) +
                  ' Addr: 0x' + IntToHex(Addr, 4) + ' Тип: ' + dbtablegateway.ShortName);
                {$ENDIF}
              end;

              if Device <> nil then
              begin

                CurrentAddr := addr;

                EventRecord.IgnoreThis := (Device.DeviceDriver.DeviceDriverID = sddPPU) and (Lo(CurrentAddr) > Lo(Device.DeviceAddress)) and
                  GetDevicePropDef(Device.Device, 'Disabled' + IntToStr(Lo(CurrentAddr) - Lo(Device.DeviceAddress)), False);

                if not EventRecord.IgnoreThis then
                begin
                  if LogRecord.Record16.EventCode = $3A then
                  begin

                    if (Device.DeviceDriver.DeviceDriverID = sddPPU) and (Lo(CurrentAddr) > Lo(Device.DeviceAddress) + 4) then
                    begin
                      EventMessage := GetAMTForPPUText(Lo(CurrentAddr) - Lo(Device.DeviceAddress), indicator + 1);
                      if indicator = 1 then
                        EventClass := dscError;
                    end else
                    begin
                      // Подменяем текст сообщения, текстом из БД
                      if (indicator = 0) and (Trim(GetDevicePropDef(Device.Device, 'Event1', '')) <> '') then
                        EventMessage := GetDevicePropDef(Device.Device, 'Event1', '') else
                      if (indicator = 1) and (Trim(GetDevicePropDef(Device.Device, 'Event2', '')) <> '') then
                        EventMessage := GetDevicePropDef(Device.Device, 'Event2', '');
                    end;

                  end;

                  // 22.06.2009 не меняем состояние для внешних
                  // 30.07.2009 неверное было сравнение
                  if (DBTablegateway.DeviceClassID = sdcRealASPTV3)
                      or (DBTablegateway.DeviceClassID = sdcOutputV3)
                      or (FInterface.Model.GetAddress(atSystem, DeviceInfo.LocalAddress, DeviceInfo.SystemAddress, LogRecord.Ext)  = 0) then
                  begin

                    for i := 0 to High(EventLogGateway.EnterStates) do
                    begin
                      if (realind = i) and (EventLogGateway.EnterStates[i] <> nil) then
                        ChangeStates(EventLogGateway.EnterStates[i], True, Device, DBTablegateway.DBRecordType = rtChild, DateTime);
                      if (realind = i) and (EventLogGateway.LeaveStates[i] <> nil) then
                        ChangeStates(EventLogGateway.LeaveStates[i], False, Device, DBTablegateway.DBRecordType = rtChild, DateTime);
                    end;


                  end;

                  if LogRecord.Record16.EventCode = $46 then
                    UserInfo := GetUserName(LogRecord.Ext[21]);

                  // 25.02.2010 не читаем ничего экстренно, если это первый зпуск

                  if not Statefull.HasStateID(GetDeviceDriver, devInitializing) and
                    (LogRecord.Record16.EventCode in [
                       $01, { Пожарная тревога }
                       $04, { Выключение/Включение/Отмена задержки автозапуска устройства оператором}
                       $05, { Исполнительное устройство [выключено/включено] }
                       $0E, { Внимание от обойденного ИП/Внимание }
                       $23, { Устранение неисправности/Неисправность }
                       $25, { Предварительный уровень запыленности датчика [устранен/достигнут] }
                       $27, { Критический уровень запыленности датчика [устранен/достигнут]}
                       $30, { Охранная тревога }
                       $34, { Обход устройства [снят/] }
                       $37, { Сообщение НС }
                       $38, { Сообщение ШУЗ }
                       $3A, { Состояние 1/Состояние 2 }
                       $3d, { Давление в норме/Давление ниже нормы/Давление выше нормы }
                       $43, { /Вращение к положению 'ОТКРЫТО'/Вращение к положению 'ЗАКРЫТО'/Задержанный пуск/... }
                       $46  { Неудачная постановка зоны на охрану}
                    ]) and
                      (abs(DBTablegateway.RawTableType) <= integer(High(TRubezh3DeviceType))) then
                  begin
                    try
                      if (Device.DeviceDriver.DeviceDriverID = sddPPU) and (AltGateway <> nil) then
                      begin
                        QueueRead(RealPanel, CurrentAddr, TRubezh3DeviceType(AltGateway.RawTableType), false);
                        if LogRecord.Record16.EventCode = $3A then
                          QueueRead(RealPanel, GetRealDeviceAddress(Device.Device), TRubezh3DeviceType(abs(DBTablegateway.RawTableType)), False);
                      end else
                        QueueRead(RealPanel, GetRealDeviceAddress(Device.Device), TRubezh3DeviceType(abs(DBTablegateway.RawTableType)),
                          LogRecord.Record16.EventCode in [38]);
                    except
                      {$IFDEF DEBUG}
                      WriteToLog(rkMessage, rtDB_Error, 'Error operative database read');
                      {$ENDIF}
                    end;
                  end;

                  // Для тревоги, записываем в последний байт контекста настройку зоны - число датчиков тревоги
                  if LogRecord.Record16.EventCode = $01 then
                  begin
                    Zone := GetFirstZone(Device.Device);
                    if Zone <> nil then
                      LogRecord.Record16.Context[9] := GetZonePropDef(Zone, 'FireDeviceCount', 2) else
                      LogRecord.Record16.Context[9] := 0;
                  end;
                end;

              end;
            end else
              EventMessage := EventMessage + '. ' + rsUnknownType;

          end;

          offset := GetEventOffset(EventLogGateway, otLine);
          if offset <> -1 then
          begin
            // что-то случилось со шлейфомм
            if (indicator in [1, 0]) then
            begin
              i := FInterface.Model.GetLineNo(Pbyte(@LogRecord.Record16.Context[Offset])^);
              if not IsSecPanel(Self.Device) then   // Не выставляем признак неисправности
                FLineStates[i-1] := indicator = 0;  // шлейфа для 2ОП чтобы продолжать реагировать на события и статусы
              SetChildState(Self, GetDeviceStateByID('ConnectionLost'), not FLineStates[i-1], i);
              if not FLineStates[i - 1] then
                Self.Statefull.SetStateByID(Self.Device.DeviceDriver, psReservedLine2Failed + i) else
                Self.Statefull.ResetStateByID(Self.Device.DeviceDriver, psReservedLine2Failed + i);

              if indicator = 0 then
              begin
                FNoLoopBreak[(i - 1) div 2] := true;
                SetChildState(Self, GetDeviceStateByID('Par_Line'), false, i);
                SetChildState(Self, GetDeviceStateByID('NPar_Line'), false, i);
              end;
            end else
            if (indicator =  2) then
            begin
              i := FInterface.Model.GetLineNo(Pbyte(@LogRecord.Record16.Context[Offset])^);
              FNoLoopBreak[(i - 1) div 2] := false;
            end;
          end;

          offset := GetEventOffset(EventLogGateway, otZoneExt);
          if offset <> -1 then
          begin
            ZoneID := FInterface.Model.GetZoneNo(PRawEventExtZoneInfo(@LogRecord.Record16.Context[Offset])^.ZoneOrDivNo);
            ZoneID := GetDatabase.GetZoneIDByZoneNo(ZoneID);
            Zone := FindZone(GetDeviceConfigFromDevice(Self.Device).GetZoneList, ZoneID);
            if Zone <> nil then
              IDZone := GetZonePropDef(Zone, 'DB$IDZones', -1);
            if GetZonePropDef(Zone, 'ZoneType' , -1) = ztGuard then
              if (LogRecord.Record16.EventCode in [$31, $45]) then // сброс постановка зоны на охрану
              begin
                // сброс постановка на охрану всех устройств зоны кроме тревожной кнопки
              //  UpdateGuardDeviceStatesOnZone(Zone, LogRecord);
                UserInfo := GetUserName(LogRecord.Ext[21]);
              end;
            for i := 0 to High(EventLogGateway.ZoneEnterStates) do
            begin
              if (indicator = i) and (EventLogGateway.ZoneEnterStates[i] <> nil) then
                ZoneChangeStates(EventLogGateway.ZoneEnterStates[i], True, Zone, DateTime);
              if (indicator = i) and (EventLogGateway.ZoneLeaveStates[i] <> nil) then
                ZoneChangeStates(EventLogGateway.ZoneLeaveStates[i], False, Zone, DateTime);
            end;
          end;


          // Обрабатывем сбросы. Для этого поднимаем левое состояние, которое будет сброшено на следующем цикле
          if (LogRecord.Record16.EventCode = $32) or ((indicator = 0) and (LogRecord.Record16.EventCode = $0D)) then
            Statefull.SetStateByID(GetDeviceDriver, psStateReset);

          EventMessage := EventMessage{ + IntToStr(Index)};
          Attributes[0] := Char(Sizeof(LogRecord));
          Move(LogRecord, Attributes[1], Sizeof(LogRecord));

{$IFNDEF DISABLE_SECURITY}
          case SubSyst of
            essFire: ResultLastFireIndex := CompositeIndex;
            essSecurity: ResultLastSecIndex := CompositeIndex;
          end;
{$ELSE}
          ResultLastFireIndex := CompositeIndex;
{$ENDIF}
          {$IFDEF DEBUG}
          WriteToLog(rkMessage, rtDB_Work, 'Parse record operation complete. Time: ' + FormatFloat('#.###', (Now-StartTime) * 24 * 60 * 60) + ' сек.');
          {$ENDIF}

{$IFDEF Debug_DUMPMEM}
          if (LogRecord.Record16.EventCode = $0B) then
            SaveDump;
{$ENDIF}

        end;
      end else
      begin
{$IFNDEF DISABLE_SECURITY}
        case SubSyst of
          essFire: ResultLastFireIndex := CompositeIndex;
          essSecurity: ResultLastSecIndex := CompositeIndex;
        end;
{$ELSE}
        ResultLastFireIndex := CompositeIndex;
{$ENDIF}

        {$IFDEF DEBUG}
        WriteToLog(rkMessage, rtDB_Error, 'Unknown event message ' + IntToStr(LogRecord.Record16.EventCode));
        {$ENDIF}
        if IsUIDChanged then
          Abort;
  {      EventClass := dscServiceRequired;
        EventMessage := Format('Неизвестный тип события: %d', [LogRecord.EventCode]); }
      end;
    except
      // Если это ошибка связи, то выйдем на повтор, иначе просто пропускаем событие
      On E: ERubezh3Error do
        raise else
        begin
          with LogEvent('Не удалось проанализировать запись журнала событий. Запись пропущена.')^ do
            EventClass := dscError;
{$IFNDEF DISABLE_SECURITY}
            case SubSyst of
              essFire: ResultLastFireIndex := CompositeIndex;
              essSecurity: ResultLastSecIndex := CompositeIndex;
            end;
{$ELSE}
            ResultLastFireIndex := CompositeIndex;
{$ENDIF}
        end;
    end;

  end;

{  procedure ProcessChildResetState(const Instance: IDeviceInstance);
  var
    child: IDeviceInstance;
    ManualResetStates: IStatefull;
    DeviceState: IDeviceState;
  begin
    ManualResetStates := Instance.GetManualResetStates;

    while ManualResetStates <> nil do
    begin
      with ManualResetStates.EnumStates do
        while Next(DeviceState) = S_OK do
          (Instance as IStatefull).ResetState(DeviceState);

      ManualResetStates := Instance.GetManualResetStates;
    end;

    with Instance.EnumChildren do
      while next(child) = s_ok do
        ProcessChildResetState(child);
  end; }

  procedure ReadDevicesStateFromDatabase;

    procedure ReadOneTable(TableType: TRubezh3DeviceType; Gateway: PDBTableGateway);
    var
      i: Integer;
      FCurTableRecords: Integer;
    begin
      if (TableType <> dtExternal) and (Gateway <> nil)
        and ((Gateway.DBRecordType in [rtDevice, rtComposite, rtOutputDevice])
        or (Gateway.DeviceClassID = sdcRealASPTV3)
        or (Gateway.DeviceClassID = sdcOutputV3) ) then
      begin
        FCurTableRecords := GetDatabase.GetDeviceCount(TableType, True);

        for i := 0 to FCurTableRecords - 1 do
        begin
          if ((i mod 20) = 0) then
          begin
            if i = 0 then
            begin
              if Statefull.HasStateID(GetDeviceDriver, devInitializing) then
                Progress(0, IntToStr(Self.Device.DeviceAddress) + ' - ' + FPanelDeviceMod.DeviceShortName + '. Опрос: '+Gateway.ShortName + '. Устройств: '+
                  IntToStr(FCurTableRecords), 0, 0)
            end else
            begin
              if Statefull.HasStateID(GetDeviceDriver, devInitializing) then
                Progress(0, IntToStr(Self.Device.DeviceAddress) + ' - ' + FPanelDeviceMod.DeviceShortName + '. Опрос: '+Gateway.ShortName + '. Опрошено: '+
                  IntToStr(i) + ' из ' + IntToStr(FCurTableRecords), 0, 0);
              // 15.07.2010 - это лишнее ?
              // FInterface.doTimeSlice;
            end;
          end;

          ReadDeviceRecord(i, TableType);
        end;
      end;
    end;

  var
    Gateway: PDBTableGateway;
    i: TRubezh3DeviceType;
    j: integer;
    CachedDatabase: IRubezh3Database;
    LineStates: TLineStates;
    ver: integer;
  begin
    if IsRubezh3FileDB or FStatesReaded then Exit;

// Неисправности АВР-ок: 0-й бит - КЗ шлейфа 1
//                       1-й бит - КЗ шлейфа 2
//                       2-й бит - Перегрузка шлейфа 1
//                       3-й бит - Перегрузка шлейфа 2
//                       4-й бит - Обрыв кольцевого шлейфа

    if FInterface.Model.IsEventLog32 then
    begin
      // состояние шлейфов читаем командой
      FInterface.ReadLineStates(LineStates);

      for j := 0 to FPanelDeviceMod.AVRCount - 1 do
      begin
{        if FStatesReaded then // Сделано что б при первоначальном запуске не вис сервер
        begin
          FLineStates[j*2] := (GetBitRange(LineStates[j], 0, 0) = 0) and
            (GetBitRange(LineStates[j], 2, 2) = 0);
          FLineStates[j*2 + 1] := (GetBitRange(LineStates[j], 1,  1) = 0) and
            (GetBitRange(LineStates[j], 3, 3) = 0);
          FNoLoopBreak[j] := GetBitRange(LineStates[j], 4, 4) = 0;
        end else
        begin }
          FLineStates[j*2] := true;
          FLineStates[j*2 + 1] := true;
          FNoLoopBreak[j] := GetBitRange(LineStates[j], 4, 4) = 0;
//          FNoLoopBreak[j] := true;
//        end;
      end;

    end;

    if (FDatabase = nil) or not FDatabase.HasDatabase then
    begin
      {$IFDEF DEBUG} // DB_ERROR
      WriteToLog(rkMessage, rtDB_Error, 'No Database. Skipping reading devices state from database');
      {$ENDIF}
      exit;
    end;

    ver := FDatabase.GetDatabaseHashVersion;

    // оптимизированное вычисление хэша конфига
    {$IFNDEF TestSkipCalcHash}
    if not SameDigest(CalculateConfigMD5Cached(Device, ver), FDatabase.GetDatabaseHash) then
    begin
      {$IFDEF DEBUG}
      WriteToLog(rkMessage, rtDB_Error, 'Different hash. Skipping reading devices state from database');
      {$ENDIF}
      exit;
    end;
    {$ENDIF}

    FDatabase := nil;
    FNeedCachedDatabase := True;
    try
      if Statefull.HasStateID(GetDeviceDriver, devInitializing) then
        Progress(0, IntToStr(Self.Device.DeviceAddress) + ' - ' + FPanelDeviceMod.DeviceShortName + '. Чтение БД панели', 0, 0);

      CachedDatabase := GetDatabase;

      SetReservedLineState(1, GetReservedLineState(1));
      SetReservedLineState(2, GetReservedLineState(2));

      {$IFDEF DEBUG}
      WriteToLog(rkMessage, rtDB_Transfer, 'Reading devices state from database');
      {$ENDIF}

  //    FTotalRecords := FDevice.GetDatabase.GetTotalDeviceCount;
      for i := Low(TRubezh3DeviceType) to high(TRubezh3DeviceType) do
      begin
        Gateway := GetDBTableGatewayList3.FindByType(integer(i));
        ReadOneTable(i, Gateway);
      end;

    finally
      FNeedCachedDatabase := False;
      FDatabase := nil;
    end;

    if CachedDatabase <> nil then
      GetDatabase.Assign(CachedDatabase.GetObj);


    {$IFDEF DEBUG} // DB_TRANSFER
    WriteToLog(rkMessage, rtDB_Transfer, 'Reading devices state finished');
    {$ENDIF}

    FStatesReaded := True;
  end;

  function ProcessChildResetState(const Instance: IDeviceInstance): boolean;
  var
    child: IDeviceInstance;
    ManualResetStates: IStatefull;
    DeviceState: IDeviceState;
  begin
    result := false;
    ManualResetStates := Instance.GetManualResetStates;

    while ManualResetStates <> nil do
    begin
      with ManualResetStates.EnumStates do
        while Next(DeviceState) = S_OK do
        begin
          result := SameText(DeviceState.GetStateCode, 'ClapanOn');
          (Instance as IStatefull).ResetState(DeviceState);
        end;

      ManualResetStates := Instance.GetManualResetStates;
    end;

    with Instance.EnumChildren do
      while next(child) = s_ok do
        result := result or ProcessChildResetState(child);
  end;

  procedure ProcessManualResetStates;
  var
    ManualResetStates: IStatefull;
    DeviceState: IDeviceState;
    NeedResetByZone, IsSecurity: boolean;
    OldPanelState, NewPanelState: int64;
    i: integer;
  begin
    // Для данной панели не имеет значение, что мы сбрасываем,
    // а имеет в какой зоне. На данный момент делаем сброс в зоне 0
    // подразуемвая, что это сброс всех состояний панели

    NewPanelState := 0;
    NeedResetByZone := False;
    ManualResetStates := GetManualResetStates;
    IsSecurity := false;
    
    while ManualResetStates <> nil do
    begin
      with ManualResetStates.EnumStates do
        while Next(DeviceState) = S_OK do
        begin
          if DeviceState.GetStateSubSystem = ssSecurity then
            IsSecurity := true else
            IsSecurity := false;
          if DeviceState.GetStateClass in [dscCritical, dscWarning] then
            NeedResetByZone := true else
            begin
              // здесь делается только сброс тестов и неисправностей
              // необходимо выявить что именно сбрасывать, а что нет
              // для этого нужно сравнить старое состояние с маской нового
//              NewPanelState := $FFFFFFFF;
              for i := Low(arPanelStates) to High(arPanelStates) do
                if arPanelStates[i].Intf = DeviceState then
                  NewPanelState := NewPanelState or (1 shl arPanelStates[i].BitNo);
            end;
        end;
      ManualResetStates := GetManualResetStates;
    end;

    if NeedResetByZone then
    begin
      {$IFDEF DEBUG}WriteToLog(rkMessage, rtAction, 'Начат сброс тревоги прибора ');{$ENDIF}
      try
        FLastAlaramReset := Now;
        FLastAlarmDelayStarted := 0;

        for i := 0 to 2 do
        begin
          try
            if IsSecurity then
              FInterface.GuardUnSetZoneState($FF00, False) else // FF00 - сброс тревоги
                                                                // 0 - снятие с охраны всех зон
              FInterface.ResetZoneState(0, False);
            break;
          except
          end;
        end;

        {$IFDEF DEBUG}WriteToLog(rkMessage, rtAction, 'Cброс тревоги прибора закончен успешно');{$ENDIF}
      except
        {$IFDEF DEBUG}WriteToLog(rkMessage, rtAction, 'Cброс тревоги прибора закончен ошибкой');{$ENDIF}
        raise;
      end;
    end;

    if (NewPanelState <> 0) then
    begin
//      if IsSecPanel(Device) then
//      begin
        OldPanelState := FInterface.GetPanelState64;
        FInterface.SetPanelState64(OldPanelState and not NewPanelState);
//      end else
//      begin
//        OldPanelState := FInterface.GetPanelState;
//        FInterface.SetPanelState(OldPanelState and not NewPanelState);
//      end;
    end;

    // Для дочерних устройств обрабатываем сброс без каких-бы то ни было обращений к панели
    // но если есть состоояние ClapanOn, то выполняем сброс через панель
    if ProcessChildResetState(Self) and ((now - FLastAlaramReset) > OneDTSecond * 15) then
    begin
      FLastAlarmDelayStarted := Now;
      {$IFDEF DEBUG}WriteToLog(rkMessage, rtAction, 'Старт таймера сброса ИУ. Со времени последнего сброса тревоги прошло:  ' + FormatFloat('#.###', (Now-FLastAlaramReset) * 24 * 60 * 60) + ' сек.');{$ENDIF}
    end;

    if (FLastAlarmDelayStarted <> 0) and (now - FLastAlarmDelayStarted > OneDTSecond * 15) then
    begin
      {$IFDEF DEBUG}WriteToLog(rkMessage, rtAction, 'Сброс ИУ. Со времени последнего таймера сброса прошло: ' + FormatFloat('#.###', (Now-FLastAlarmDelayStarted) * 24 * 60 * 60) + ' сек.');{$ENDIF}
      FLastAlarmDelayStarted := 0;
      FInterface.ResetClapanState;
    end;

  end;

  procedure ProcessIgnoreLists;
  begin
    FLock.Acquire;
    try
      while FIgnoreAdditions.Count > 0 do
      begin
        FInterface.IgnoreListOp((FIgnoreAdditions[0] as IDeviceInstance).Device, True);
        FIgnoreAdditions.Delete(0);
      end;
      while FIgnoreToRemove.Count > 0 do
      begin
        FInterface.IgnoreListOp((FIgnoreToRemove[0] as IDeviceInstance).Device, False);
        FIgnoreToRemove.Delete(0);
      end;
    finally
      FLock.Release;
    end;
  end;

  procedure ProcessRuntimeCommands;

    function IsPresetRequest(RequestID: integer): boolean;
    begin
      if RequestID = 0 then
        result := true else
        result := FServerReq.GetRequest(RequestID).ID <> 0;
    end;

    {$IFDEF ParamsUseDelta}
    procedure ChangeDeviceParams(Device: IDevice);
    begin
      FServerReq.AddChangedDevice(Device);
    end;
    {$ENDIF}

    function GetSplitByte(Device: IDevice; ParamNo: integer; RawType: TRawDeviceType): integer;
    var PropInfo: TPropertyTypeInfo;
    begin
      result := -1;
      with Device.DeviceDriver.EnumSupportedProperties do
        while Next(PropInfo) = S_OK do
          if Pos('Config$', PropInfo.Name) = 1 then
            if (PropInfo.ShiftInMemory = ParamNo) and (PropInfo.RawType <> RawType) then
            begin
              result := GetDevicePropDef(Device, PropInfo.Name, PropInfo.DefaultValue);
              break;
            end;
    end;

    procedure ReadSimpleDeviceParam(Device: IDevice; ParamNo: integer; RequestID: integer);
    var Value: double;
        Params: IParams;
    begin
      Params := TParamsImpl.Create;
      try
        Value := FInterface.ReadSimpleParamFromDevice(Device, ParamNo);
      except
        Value := -1;
      end;

      Params.NewParam('ParamNo', ParamNo);
      Params.NewParam('ParamValue', Value);
      if Value <> -1 then
      begin
        FServerReq.SetRequestState(RequestID, Params, rsComplete);
      end else
      begin
        FServerReq.SetRequestState(RequestID, Params, rsFail);
      end;
    end;

    procedure WriteSimpleDeviceParam(Device: IDevice; ParamNo, ParamValue: integer; RequestID: integer);
    var Value: double;
        Params: IParams;
    begin
      Params := TParamsImpl.Create;
      try
        Value := FInterface.WriteSimpleParamFromDevice(Device, ParamNo, ParamValue);
      except
        Value := -1;
      end;

      Params.NewParam('ParamNo', ParamNo);
      Params.NewParam('ParamValue', Value);
      if Value = ParamValue then
      begin
        FServerReq.SetRequestState(RequestID, Params, rsComplete);
      end else
      begin
        FServerReq.SetRequestState(RequestID, Params, rsFail);
      end;

    end;

    procedure ReadDeviceParam(Device: IDevice; PropInfo: PPropertyTypeInfo; RequestID: integer; Multi: boolean = false; Complete: boolean = false);
    var Value: double;
        Params: IParams;
    begin
      if Multi then
        Params := FServerReq.GetRequest(RequestID).Params;
      if Complete then
      begin
        FServerReq.SetRequestState(RequestID, Params, rsComplete);
        exit;
      end;
      if PropInfo <> nil then
      begin
        if Params = nil then
          Params := TParamsImpl.Create;
        try
          Value := FInterface.ReadPropFromDevice(PropInfo, Device);
        except
          Value := -1;
        end;
        Params.NewParam(PropInfo.Name, Value);
        if Value <> -1 then
        begin
          SetDeviceParam(Device, PropInfo.Name, Value);
          {$IFDEF ParamsUseDelta}
          ChangeDeviceParams(Device);
          {$ENDIF}
          if Multi then
            FServerReq.SetRequestState(RequestID, Params, rsWait) else
            FServerReq.SetRequestState(RequestID, Params, rsComplete);
        end else
        begin
          if Multi then
            FServerReq.SetRequestState(RequestID, Params, rsWait) else
            FServerReq.SetRequestState(RequestID, Params, rsFail);
        end;
      end else
      begin
        if Multi then
          FServerReq.SetRequestState(RequestID, Params, rsWait) else
          FServerReq.SetRequestState(RequestID, Params, rsFail);
        raise Exception.Create('Device property not found');
      end;
    end;

    procedure WriteDeviceProp(Device: IDevice; PropInfo: PPropertyTypeInfo; RequestID: integer; Multi: boolean = false; Complete: boolean = false);
    var Value: double;
        Params: IParams;
        SplitByte: integer;
    begin
      if Multi then
        Params := FServerReq.GetRequest(RequestID).Params;
      if Complete then
      begin
        FServerReq.SetRequestState(RequestID, Params, rsComplete);
        exit;
      end;
      if PropInfo <> nil then
      begin
        if PropInfo.RawType <> rdtWord then
          SplitByte := GetSplitByte(Device, PropInfo.ShiftInMemory, PropInfo.RawType) else
          SplitByte := -1;
        if Params = nil then
          Params := TParamsImpl.Create;
        try
          Value := FInterface.WritePropToDevice(PropInfo, Device, SplitByte);
        except
          Value := -1;
        end;
        Params.NewParam(PropInfo.Name, Value);
        if Value <> -1 then
        begin
          SetDeviceParam(Device, PropInfo.Name, Value);
          {$IFDEF ParamsUseDelta}
          ChangeDeviceParams(Device);
          {$ENDIF}
          if Multi then
            FServerReq.SetRequestState(RequestID, Params, rsWait) else
            FServerReq.SetRequestState(RequestID, Params, rsComplete);
        end else
        begin
          if Multi then
            FServerReq.SetRequestState(RequestID, Params, rsWait) else
            FServerReq.SetRequestState(RequestID, Params, rsFail);
        end;
      end else
      begin
        if Multi then
          FServerReq.SetRequestState(RequestID, Params, rsWait) else
          FServerReq.SetRequestState(RequestID, Params, rsFail);
        raise Exception.Create('Device property not found');
      end;
    end;

  var
    ADevice: IDeviceInstance;
    CmdDevCopy: IInterfaceList;
    CmdNameCopy: TStringList;
    CmdUserInfoCopy: TStringList;
    CmdParamsCopy: TStringList;
    CmdRequestIDsCopy: TStringList;
    CmdCaption: string;
    i, k: integer;
    ParamNo, ParamValue: integer;
    ZoneNo: integer;
    PPropInfo: PPropertyTypeInfo;
    TPropInfo: TPropertyTypeInfo;
  begin
    CmdNameCopy := TStringList.Create;
    CmdUserInfoCopy := TStringList.Create;
    CmdParamsCopy := TStringList.Create;
    CmdRequestIDsCopy := TStringList.Create;
    try

      FLock.Acquire;
      try
        if FRuntimeCmdDevices.Count = 0 then
          exit;

        CmdDevCopy := TInterfaceList.Create;

        // добавлена константа COUNT_RUNTIMECALL_PER_REQUEST, которая определяет
        // количество вызовов рантаймовых методов, 0 - вся очередь.
        // необходимомость появилась при конфигурировании устройсов, так как
        // очередь может накопиться больша при записи параметров во все устр. прибора
        //                                                   2.04.2012 Роман
        if COUNT_RUNTIMECALL_PER_REQUEST = 0 then
        begin
          for I := 0 to FRuntimeCmdDevices.Count - 1 do
          begin
            CmdDevCopy.Add(FRuntimeCmdDevices[i]);
            CmdNameCopy.Add(FRuntimeCmdNames[i]);
            CmdUserInfoCopy.Add(FRuntimeCmdUserInfos[i]);
            CmdParamsCopy.Add(FRuntimeCmdParams[i]);
            CmdRequestIDsCopy.Add(FRuntimeCmdRequestIDs[i]);
          end;

          FRuntimeCmdDevices.Clear;
          FRuntimeCmdNames.Clear;
          FRuntimeCmdUserInfos.Clear;
          FRuntimeCmdParams.Clear;
          FRuntimeCmdRequestIDs.Clear;
        end else
        begin
          k := Min(COUNT_RUNTIMECALL_PER_REQUEST, FRuntimeCmdDevices.Count);
          i := 0;
          while (CmdDevCopy.Count < k) and (i < FRuntimeCmdDevices.Count) do
          begin
            if IsPresetRequest(StrToInt(FRuntimeCmdRequestIDs[i])) then
            begin
              CmdDevCopy.Add(FRuntimeCmdDevices[i]);
              CmdNameCopy.Add(FRuntimeCmdNames[i]);
              CmdUserInfoCopy.Add(FRuntimeCmdUserInfos[i]);
              CmdParamsCopy.Add(FRuntimeCmdParams[i]);
              CmdRequestIDsCopy.Add(FRuntimeCmdRequestIDs[i]);
            end;
            FRuntimeCmdDevices.Delete(i);
            FRuntimeCmdNames.Delete(i);
            FRuntimeCmdUserInfos.Delete(i);
            FRuntimeCmdParams.Delete(i);
            FRuntimeCmdRequestIDs.Delete(i);
            Inc(i)
          end;
        end;

      finally
        FLock.Release;
      end;

      for i := 0 to CmdDevCopy.Count - 1 do
      begin
        try
          ADevice := IDeviceInstance(CmdDevCopy[i]);
          if SameText(CmdNameCopy[i], 'BoltOpen') then
          begin
            FInterface.BoltDeviceLevelCmd(bcOpen, ADevice.Device);
            CmdCaption := 'Открыть';
          end else
          if SameText(CmdNameCopy[i], 'BoltClose') then
          begin
            FInterface.BoltDeviceLevelCmd(bcClose, ADevice.Device);
            CmdCaption := 'Закрыть';
          end else
          if SameText(CmdNameCopy[i], 'BoltStop') then
          begin
            FInterface.BoltDeviceLevelCmd(bcStop, ADevice.Device);
            CmdCaption := 'Стоп';
          end else
          if SameText(CmdNameCopy[i], 'BoltAutoOn') then
          begin
            FInterface.SetAutomaticState(ADevice.Device, True);
            CmdCaption := 'Включение автоматики';
          end else
          if SameText(CmdNameCopy[i], 'BoltAutoOff') then
          begin
            FInterface.SetAutomaticState(ADevice.Device, False);
            CmdCaption := 'Выключение автоматики';
          end else
          if SameText(CmdNameCopy[i], 'SetZoneToGuard') then
          begin
            if CmdParamsCopy[i] <> '' then
            begin
              ZoneNo := StrToInt(CmdParamsCopy[i]);
              CmdCaption := 'Рубеж-2ОП. Постановка зоны на охрану';
            end else
            begin
              ZoneNo := 0;
              CmdCaption := 'Рубеж-2ОП. Постановка прибора на охрану';
            end;
            FInterface.GuardSetZoneState(ZoneNo, False);
          end else
          if SameText(CmdNameCopy[i], 'UnSetZoneFromGuard') then
          begin
            if CmdParamsCopy[i] <> '' then
            begin
              ZoneNo := StrToInt(CmdParamsCopy[i]);
              CmdCaption := 'Рубеж-2ОП. Снятие зоны с охраны';
            end else
            begin
              ZoneNo := 0;
              CmdCaption := 'Рубеж-2ОП. Снятие прибора с охраны';
            end;
            FInterface.GuardUnSetZoneState(ZoneNo, False);
          end else

// Глупые процедуры чтения записи параметра с устройства для Севастьянова
          if SameText(CmdNameCopy[i], 'Device$ReadSimpleParam') then
          begin
            ReadSimpleDeviceParam(ADevice.Device, StrToInt(CmdParamsCopy[i]), StrToInt(CmdRequestIDsCopy[i]));
          end else
          if SameText(CmdNameCopy[i], 'Device$WriteSimpleParam') then
          begin
            k := pos('=', CmdParamsCopy[i]);
            if k > 0 then
            begin
              ParamNo := StrToInt(Copy(CmdParamsCopy[i], 1, k-1));
              ParamValue := StrToInt(Copy(CmdParamsCopy[i], k + 1, Length(CmdParamsCopy[i]) - k));
              WriteSimpleDeviceParam(ADevice.Device, ParamNo, ParamValue, StrToInt(CmdRequestIDsCopy[i]));
            end;
          end else

// чтение из устройства одного параметра
          if SameText(CmdNameCopy[i], 'Device$ReadParam') then
          begin
            PPropInfo := ADevice.DeviceDriver.FindSupportedDeviceProp(CmdParamsCopy[i]);
            ReadDeviceParam(ADevice.Device, PPropInfo, StrToInt(CmdRequestIDsCopy[i]));
          end else

// запись в устройство одного параметра
          if SameText(CmdNameCopy[i], 'Device$WriteParam') then
          begin
            PPropInfo := ADevice.DeviceDriver.FindSupportedDeviceProp(CmdParamsCopy[i]);
            WriteDeviceProp(ADevice.Device, PPropInfo, StrToInt(CmdRequestIDsCopy[i]));
          end else

// чтение из устройства параметров
          if SameText(CmdNameCopy[i], 'Device$ReadParams') then
          begin
            with ADevice.Device.DeviceDriver.EnumSupportedProperties do
              while Next(TPropInfo) = S_OK do
                if Pos('Config$', TPropInfo.Name) = 1 then
                  ReadDeviceParam(ADevice.Device, @TPropInfo, StrToInt(CmdRequestIDsCopy[i]), true);
            ReadDeviceParam(ADevice.Device, nil, StrToInt(CmdRequestIDsCopy[i]), true, true);
          end else

// запись в устройство параметров
          if SameText(CmdNameCopy[i], 'Device$WriteParams') then
          begin
            with ADevice.Device.DeviceDriver.EnumSupportedProperties do
              while Next(TPropInfo) = S_OK do
                if Pos('Config$', TPropInfo.Name) = 1 then
                  if Sametext('SkipEqual', CmdParamsCopy[i]) then
                  begin
                    if GetDeviceProp(ADevice.Device, TPropInfo.Name) <> GetDeviceParam(ADevice.Device, TPropInfo.Name) then
                      WriteDeviceProp(ADevice.Device, @TPropInfo, StrToInt(CmdRequestIDsCopy[i]), true);
                  end else
                    WriteDeviceProp(ADevice.Device, @TPropInfo, StrToInt(CmdRequestIDsCopy[i]), true);
            WriteDeviceProp(ADevice.Device, nil, StrToInt(CmdRequestIDsCopy[i]), true, true);
          end else

          if pos('Control$', CmdNameCopy[i]) = 1 then
          begin
            PPropInfo := ADevice.DeviceDriver.FindSupportedDeviceProp(CmdNameCopy[i]);
            if PPropInfo.Name = '' then
               raise Exception.Create('Неизвестная команда: ' + CmdNameCopy[i]);
            FInterface.RunSingleDeviceCommand(PPropInfo, ADevice.Device);
            CmdCaption := PPropInfo.Caption;
          end else
            raise Exception.Create('Неизвестная команда: ' + CmdNameCopy[i]);

          if CmdCaption <> '' then
            with LogDeviceActionNewEvent(True)^ do
            begin
              EventClass := dscNormal;
              EventMessage := 'Команда оператора ПК: ' + CmdCaption;
              DateTime := Now;
              SysDateTime := Now;
              if (SameText(CmdNameCopy[i], 'UnSetZoneFromGuard'))
              or (SameText(CmdNameCopy[i], 'SetZoneToGuard')) then
                //IDZone := FInterface.Model.GetZoneNo(ZoneNo)
              else
                Device := ADevice;
              UserInfo := CmdUserInfoCopy[i];
            end;

        except
          On E: Exception do
          with LogDeviceActionNewEvent(True)^ do
          begin
            EventClass := dscNormal;
            EventMessage := 'Попытка управления устройством неуспешна. ' + E.Message;
            DateTime := Now;
            SysDateTime := Now;
            UserInfo := CmdUserInfoCopy[i];
          end;
        end;

      end;
    finally
      CmdNameCopy.Free;
      CmdUserInfoCopy.Free;
      CmdParamsCopy.Free;
      CmdRequestIDsCopy.Free;
      
      CmdDevCopy := nil;
    end;
  end;

  procedure SetParamValue(const ParamName: string; const ParamValue: Variant);
  var
    Param: IParam;
  begin
    Param := Self.DeviceParams.FindParam(ParamName);
    if Param <> nil then
      Param.SetValue(ParamValue);
  end;

  var
    NonUsedDevices, ExternalDevices: Integer;

  procedure UpdateDeviceCounts;

    procedure IncCnt(var Counter: integer; const Device: IDeviceInstance);
    begin
      inc(Counter, 1 {Device.DeviceDriver.ReservedAddresses});
    end;

    function CheckStateID(const Device: IDeviceInstance; StateID: integer): boolean;
    var
      State: IDeviceState;
      i: integer;
    begin
      with (Device as IStatefull) do
        result := HasStateID(Device.DeviceDriver, StateID);

      if not result and (Device.DeviceDriver.DeviceDriverID = sddPPU) then
      begin
        State := Device.DeviceDriver.GetStateByID(StateID);
        for I := 1 to 4 do
          if HasAltState(Device, State, i, PPUAltStates) then
          begin
            result := true;
            break;
          end;
      end;
      
    end;

  var
    TotalCount, LostCount, SmokeCount, FailureCount, IgnoreCount: Integer;

    procedure ProcessDeviceCounter(const Device: IDeviceInstance);
    var
      ChildDevice: IDeviceInstance;
    begin
      with Device.EnumChildren do
        while Next(ChildDevice) = S_OK do
          if not ChildDevice.DeviceDriver.AutoCreateRange.Enabled then
            with (ChildDevice as IStatefull) do
            begin
              incCnt(TotalCount, ChildDevice);

              if CheckStateID(ChildDevice, FCSIndex) then
                incCnt(LostCount, ChildDevice);

              if HasStateID(ChildDevice.DeviceDriver, FHardwareIgnore) then
                incCnt(IgnoreCount, ChildDevice);

              if CheckStateID(ChildDevice, FMlIndex) then
                incCnt(FailureCount, ChildDevice);

              if HasStateCode(ChildDevice.DeviceDriver, 'HighDustiness') or
                HasStateCode(ChildDevice.DeviceDriver, 'LowDustiness') then
                  incCnt(SmokeCount, ChildDevice);

              ProcessDeviceCounter(ChildDevice);
            end;

    end;

  begin
    LostCount := 0; SmokeCount := 0; FailureCount := 0; IgnoreCount :=0;

    ProcessDeviceCounter(Self);


    SetParamValue('DeviceTotalCount', TotalCount);
    SetParamValue('DeviceLostCount', LostCount);
    SetParamValue('DeviceIgnoreCount', IgnoreCount);
    SetParamValue('DeviceFailureCount', FailureCount);
    SetParamValue('DeviceSmokeCount', SmokeCount);
    if FConnectionState = csConnected then
    begin
      SetParamValue('DeviceNonUsedCount', NonUsedDevices);
      SetParamValue('DeviceExternalCount', ExternalDevices);
    end else
    begin
      SetParamValue('DeviceNonUsedCount', NAN);
      SetParamValue('DeviceExternalCount', NAN);
    end;


  end;

var
  EventRecordsDynArray: array of TRawEventRecord32;
  ReadingStartedAt: TDatetime;
  PanelState: int64;
  LineShort: boolean;
  i, idx, FromRec, PanelLastFireIndex, {$IFNDEF DISABLE_SECURITY} PanelLastSecIndex, {$ENDIF} BufSize: Integer;
  {$IFDEF DEBUG}
  OldThreadName: string;
  {$ENDIF}
begin
  {$IFDEF DEBUG}
  OldThreadName := CurrentUserName;
  CurrentUserName := 'Query Rubezh3 device # ' + IntToStr(Device.DeviceAddress);
  try
  {$ENDIF}

{$IFDEF Debug_DUMPMEM}
  if FMemDumpID = -1 then
    SaveDump;
{$ENDIF}

  NonUsedDevices := 0;
  ExternalDevices := 0;

  FInterface.SoftUpdateMode := False;
  FInterface.FastLoseDetection := True;
  try
    FEventList := TEventList.Create;
    try
      result := TEnumLogRecords.Create(FEventList, ioOwned);
      try
        if not FUIDReaded and Statefull.HasStateID(GetDeviceDriver, devInitializing) then
          Progress(0, IntToStr(Device.DeviceAddress) + ' - ' + FPanelDeviceMod.DeviceShortName + '. ' + rsMonitoringStarts, 0, 0);

        if FPanelDeviceMod.DeviceSubtype in [dsIndicator, dsMDS, dsMDS5{$IFDEF RubezhRemoteControl}, dsRemoteControl, dsRemoteControlFire {$ENDIF}] then
          begin
            if FPanelDeviceMod.DeviceSubtype = dsIndicator then
            begin
              PanelState := FInterface.GetPanelState;
              if (PanelState and $01) <> 0 then
                Statefull.SetStateByID(GetDeviceDriver, psBIConnectionWithPanelIsLost)
              else Statefull.ResetStateByID(GetDeviceDriver, psBIConnectionWithPanelIsLost);
              if (PanelState and $02) <> 0 then
                Statefull.SetStateByID(GetDeviceDriver, psBIDifferenceDBwithPanel)
              else Statefull.ResetStateByID(GetDeviceDriver, psBIDifferenceDBwithPanel);
            end else
            if (FPanelDeviceMod.DeviceSubtype in [dsMDS, dsMDS5]) then
            begin
              PanelState := FInterface.GetPanelState(2, false);
              if (PanelState and $01) <> 0 then
                Statefull.SetStateByID(GetDeviceDriver, psMDSLineFail)
              else Statefull.ResetStateByID(GetDeviceDriver, psMDSLineFail);
              if (PanelState and $02) <> 0 then
                Statefull.SetStateByID(GetDeviceDriver, psMDSDeliverFail)
              else Statefull.ResetStateByID(GetDeviceDriver, psMDSDeliverFail);
              if (PanelState and $04) <> 0 then
                Statefull.SetStateByID(GetDeviceDriver, psMDSBufferOverflow)
              else Statefull.ResetStateByID(GetDeviceDriver, psMDSBufferOverflow);
              if (PanelState and $20) <> 0 then
                Statefull.SetStateByID(GetDeviceDriver, psRCConnectionWithPanelIsLost)
              else Statefull.ResetStateByID(GetDeviceDriver, psRCConnectionWithPanelIsLost);
              i := Byte(PanelState shr 8);
              Config.DeviceParams.SetParamValue('CountNotSendMsg', i);
{$IFDEF RubezhRemoteControl}
            end else
            if (FPanelDeviceMod.DeviceSubtype in [dsRemoteControl, dsRemoteControlFire]) then
            begin
              PanelState := FInterface.GetPanelState;
              if (PanelState and $01) <> 0 then
                Statefull.SetStateByID(GetDeviceDriver, psRCConnectionWithPanelIsLost)
              else Statefull.ResetStateByID(GetDeviceDriver, psRCConnectionWithPanelIsLost);
              if (PanelState and $02) <> 0 then
                Statefull.SetStateByID(GetDeviceDriver, psRCDifferenceDBwithPanel)
              else Statefull.ResetStateByID(GetDeviceDriver, psRCDifferenceDBwithPanel);

              if (FPanelDeviceMod.DeviceSubtype = dsRemoteControlFire) then
              begin
                if (PanelState and $04) <> 0 then
                begin
                  Statefull.SetStateByID(GetDeviceDriver, psRCKeyBoardlock);
                  Statefull.ResetStateByID(GetDeviceDriver, psRCKeyBoardUnlock);
                end else
                begin
                  Statefull.SetStateByID(GetDeviceDriver, psRCKeyBoardUnlock);
                  Statefull.ResetStateByID(GetDeviceDriver, psRCKeyBoardlock);
                end;

                if (PanelState and $08) = 0 then
                  Statefull.SetStateByID(GetDeviceDriver, psRCPower1Fail)
                else Statefull.ResetStateByID(GetDeviceDriver, psRCPower1Fail);
                if (PanelState and $10) = 0 then
                  Statefull.SetStateByID(GetDeviceDriver, psRCPower2Fail)
                else Statefull.ResetStateByID(GetDeviceDriver, psRCPower2Fail);
              end;
{$ENDIF}
            end;

            HandleNormalConnection(Now);

            SetIntErrorState(dbValidated);
            FDelayedSynchronizationFailed := False;
            SetDatabaseState(dbValidated);

            exit;
          end;

        ReadingStartedAt := Now;

        doSetDateTime;

        if FUIDChanged and IsUIDChanged then
          Exit;

        ProcessManualResetStates;
        ProcessIgnoreLists;
        ProcessRuntimeCommands;
        Statefull.ResetStateByID(GetDeviceDriver, psStateReset);

        ////// Чтение пожарного журнала
{$IFNDEF DISABLE_SECURITY}
        SubSyst := essFire;
{$ENDIF}
        {$IFDEF DEBUG}WriteToLog(rkMessage, rtDB_Work, 'Reading log fire records');{$ENDIF}

        if FInterface.Model.IsEventLog32 then
          CurLogType := LOG_FIRE_TYPE_32byte else
          CurLogType := LOG_FIRE_TYPE_16byte;

        PanelLastFireIndex := FInterface.EventLogLastRecord(CurLogType);
        {$IFDEF DEBUG}WriteToLog(rkMessage, rtDB_Work, 'Reading log fire records. Last panel index: ' + IntToStr(PanelLastFireIndex));{$ENDIF}
        // Данная ситуация проявилась при зависании и перезагрузке прибора
        // В приборе не успели инициализироваться все переменные
        if PanelLastFireIndex = -1 then
        begin
          DoReceiveFailIndex(essFire);
          PanelLastFireIndex := SysLastFireIndex;
        end;

        // Если панель подменена или это вообще новая панель не читаем
        // ее события, т.к. они относятся к другому объекту, а делаем
        // запись в журнале о том что обнаружена новая панель

        if (PanelLastFireIndex < SysLastFireIndex) or AllRecords then
        begin
          ResultLastFireIndex := PanelLastFireIndex;
          FireOverflow := True;
           {$IFDEF DEBUG}WriteToLog(rkMessage, rtDB_Work, 'Addr: '+IntToStr(Device.DeviceAddress) + ' UID: '+ GuidToString(FUid)+
             ': SysLastFireIndex: '+ IntToStr(SysLastFireIndex) +
             '. PanelLastFireIndex: '+ IntToStr(PanelLastFireIndex)+
             ': Flag AllRecords set. Logging new device...');{$ENDIF}
          DoLogNewDevice(ResultLastFireIndex, PanelLastFireIndex < SysLastFireIndex, {$IFNDEF DISABLE_SECURITY} SubSyst {$ELSE} essFire {$ENDIF});
          {$IFDEF DEBUG}
          LogFlushBuffer;
          {$ENDIF}
        end else
        if PanelLastFireIndex > SysLastFireIndex then
        begin
          // Читать записи нужно от PanelLastIndex до SysLastIndex,
          // но не более объема журнала
          BufSize := FInterface.EventLogBufferSize(CurLogType);

          // Необходимо также учесть, что записей может быть менее BufSize
          if SysLastFireIndex < (PanelLastFireIndex - BufSize) then
            FromRec := max(0, PanelLastFireIndex - BufSize + 1) else
            FromRec := SysLastFireIndex + 1;

          // Читаем пачками по 20 событий
          if not Statefull.HasStateID(GetDeviceDriver, devInitializing) then
           PanelLastFireIndex := Min(PanelLastFireIndex, FromRec + 20);

          {$IFDEF DEBUG}WriteToLog(rkMessage, rtDB_Error,
            'Addr: '+IntToStr(Device.DeviceAddress) + ' UID: '+ GuidToString(FUid)+
            ': SysLastFireIndex: '+ IntToStr(SysLastFireIndex) +
            '. PanelLastFireIndex: '+ IntToStr(PanelLastFireIndex)+
            '. Reading records from '+IntToStr(FromRec) +' to '+IntToStr(PanelLastFireIndex));
          {$ENDIF}

          TotalRecords := PanelLastFireIndex - FromRec + 1;
          try
            SetLength(EventRecordsDynArray, 0);
            for i := FromRec to PanelLastFireIndex do
            begin
              SetLength(EventRecordsDynArray, Length(EventRecordsDynArray) + 1);
              EventRecordsDynArray[High(EventRecordsDynArray)] := ReadLogRecord(CurLogType, i);
            end;
            for i := 0 to Length(EventRecordsDynArray) - 1 do
              ParseLogRecord(FromRec + i, EventRecordsDynArray[i]);
          except
            On E: Exception do
            begin
              {$IFDEF DEBUG}WriteToLog(rkMessage, rtDB_Error, 'Addr: '+IntToStr(Device.DeviceAddress) + ' UID: '+ GuidToString(FUid)+
              ': ReadRecordException: ' + E.Message);{$ENDIF}
              raise;
            end else
              raise;
          end;

        end;

{$IFNDEF DISABLE_SECURITY}
///////////////////////////////////////////////////////////////////////////////////
        //// Чтение охранного журнала с 2ОП
        if (FPanelDeviceMod.DeviceName = rsUSBRubezh2OP_Name)
          or (FPanelDeviceMod.DeviceName = rsRubezh2OP_Name) then
        begin
          SubSyst := essSecurity;
          {$IFDEF DEBUG}WriteToLog(rkMessage, rtDB_Work, 'Reading log security records');{$ENDIF}

          if FInterface.Model.IsEventLog32 then
            CurLogType := LOG_SEC_TYPE_32byte else
            CurLogType := LOG_SEC_TYPE_16byte;

          PanelLastSecIndex := FInterface.EventLogLastRecord(CurLogType);
          {$IFDEF DEBUG}WriteToLog(rkMessage, rtDB_Work, 'Reading log security records. Last panel index: ' + IntToStr(PanelLastSecIndex));{$ENDIF}
          // Данная ситуация проявилась при зависании и перезагрузке прибора
          // В приборе не успели инициализироваться все переменные
          if PanelLastSecIndex = -1 then
          begin
            DoReceiveFailIndex(essSecurity);
            PanelLastSecIndex := SysLastSecIndex;
          end;

          // Если панель подменена или это вообще новая панель не читаем
          // ее события, т.к. они относятся к другому объекту, а делаем
          // запись в журнале о том что обнаружена новая панель

          if (PanelLastSecIndex < SysLastSecIndex) or AllRecords then
          begin
            ResultLastSecIndex := PanelLastSecIndex;
            SecOverflow := True;
             {$IFDEF DEBUG}WriteToLog(rkMessage, rtDB_Work, 'Addr: '+IntToStr(Device.DeviceAddress) + ' UID: '+ GuidToString(FUid)+
               ': SysLastSecIndex: '+ IntToStr(SysLastSecIndex) +
               '. PanelLastSecIndex: '+ IntToStr(PanelLastSecIndex)+
               ': Flag AllRecords set. Logging new device...');{$ENDIF}
            DoLogNewDevice(ResultLastSecIndex, PanelLastSecIndex < SysLastSecIndex, SubSyst);
            {$IFDEF DEBUG}
            LogFlushBuffer;
            {$ENDIF}
          end else
          if PanelLastSecIndex > SysLastSecIndex then
          begin
            // Читать записи нужно от PanelLastIndex до SysLastIndex,
            // но не более объема журнала
            BufSize := FInterface.EventLogBufferSize(CurLogType);

            // Необходимо также учесть, что записей может быть менее BufSize
            if SysLastSecIndex < (PanelLastSecIndex - BufSize) then
              FromRec := max(0, PanelLastSecIndex - BufSize + 1) else
              FromRec := SysLastSecIndex + 1;

            // Читаем пачками по 20 событий
            if not Statefull.HasStateID(GetDeviceDriver, devInitializing) then
             PanelLastSecIndex := Min(PanelLastSecIndex, FromRec + 20);

            {$IFDEF DEBUG}WriteToLog(rkMessage, rtDB_Transfer,
              'Addr: '+IntToStr(Device.DeviceAddress) + ' UID: '+ GuidToString(FUid)+
              ': SysLastSecIndex: '+ IntToStr(SysLastSecIndex) +
              '. PanelLastSecIndex: '+ IntToStr(PanelLastSecIndex)+
              '. Reading records from '+IntToStr(FromRec) +' to '+IntToStr(PanelLastSecIndex));
            {$ENDIF}

            TotalRecords := PanelLastSecIndex - FromRec + 1;
            try
              SetLength(EventRecordsDynArray, 0);
              for i := FromRec to PanelLastSecIndex do
              begin
                SetLength(EventRecordsDynArray, Length(EventRecordsDynArray) + 1);
                EventRecordsDynArray[High(EventRecordsDynArray)] := ReadLogRecord(CurLogType, i);
              end;
              for i := 0 to Length(EventRecordsDynArray) - 1 do
                ParseLogRecord(FromRec + i, EventRecordsDynArray[i]);
            except
              On E: Exception do
              begin
                {$IFDEF DEBUG}WriteToLog(rkMessage, rtDB_Error, 'Addr: '+IntToStr(Device.DeviceAddress) + ' UID: '+ GuidToString(FUid)+
                ': ReadRecordException: ' + E.Message);{$ENDIF}
                raise;
              end else
                raise;
            end;


          end;
        end;
/////////////////////////////////////////////////////////////////////////////////////////
{$ENDIF}
        if not FStatesReaded then
        begin
          PanelState := FInterface.GetPanelState64;
          if FPanelDeviceMod.DatabaseType = dbtClassic { AVRCount <= 1 } then
          begin
            FLineStates[0] := (PanelState and $80000) = 0;
            FLineStates[1] := (PanelState and $100000) = 0;
          end;

        end;


        {$IFDEF DEBUG}WriteToLog(rkMessage, rtOperation, 'Overriding states with actual');{$ENDIF}

// 23.06.2011 На текущий момент по словам Александра Рыбина
//            все современные версии прошивок поддерживают
//            доп. 4-х байтное словосостояние панели
        PanelState := FInterface.GetPanelState64;

        // в режиме конфигурации
        if (PanelState and $2000) <> 0 then
        begin
          HandleNormalConnection(ReadingStartedAt);
          exit;
        end;

        FStatesReaded := FStatesReaded or ((PanelState and $80000) = 1);

        ReadDevicesStateFromDatabase;

        if FPanelDeviceMod.DatabaseType = dbtClassic { AVRCount <= 1 } then
        begin
          FLineStates[0] := (PanelState and $80000) = 0;
          FLineStates[1] := (PanelState and $100000) = 0;
        end;

        CheckStates(PanelState);

        for i := 0 to FPanelDeviceMod.AVRCount * 2 - 1 do
        begin
          LineShort := not FLineStates[i];

          {$IFDEF LoopLines}
          // Для кольцевых шлефов только КЗ обоих шлейфов является потерей
          if GetDevicePropDef(Config, 'LoopLine' + IntToStr(i div 2 + 1), false) then
            LineShort := not FLineStates[i div 2] and not FLineStates[i div 2 + 1];
          {$ENDIF}

          if LineShort then
          begin
            SetChildState(Self, GetDeviceStateByID('ConnectionLost'), not FLineStates[i], i + 1);
            SetLineFailureType(Self, rsConnectionLost, i + 1, True);
          end;

          if LineShort then
            Self.Statefull.SetStateByID(Device.DeviceDriver, psReservedLine2Failed + i + 1) else
            Self.Statefull.ResetStateByID(Device.DeviceDriver, psReservedLine2Failed + i+ 1);
        end;

{        if FPanelDeviceMod.AVRCount <= 2 then
        begin
          // Это работает только для 2ам, для поздних приборов нет флага в панели
          if FLine1Fail then
            SetChildState(Self, GetDeviceStateByID('ConnectionLost'), True, 1);
          if FLine2Fail then
            SetChildState(Self, GetDeviceStateByID('ConnectionLost'), True, 2);
        end }

        if IsRubezh3Debug then
          SetParamValue('Debug', 'State:' + IntToHex(PanelState, 4) + GetDatabase.ReadExtraDebugStr);

        with GetBGTaskStateInfo do
        begin
        if (CurrentDeviceTable = 0) and (CurrentDeviceNum <= 0) then
          begin // опрос устройст на начале
            NonUsedDevices := FInterface.GetNonUsedDevices;
            FLastNonUsedtDeviceCount := NonUsedDevices;
          end else
            NonUsedDevices := FLastNonUsedtDeviceCount
        end;
        ExternalDevices := GetDatabase.GetDeviceCount(dtExternal);

        if Length(FReadQueue) <> 0 then
        begin
          FLock.Acquire;
          try
            for I := High(FReadQueue) downto 0 do
              if FReadQueue[i].FirstTime then
              begin
                FReadQueue[i].FirstTime := False;
                Continue;
              end else
              if Now - FReadQueue[i].StartTime >  DELAY_BEFORE_EXT_READ  then
              begin
                try
                  {$IFDEF DEBUG}WriteToLog(rkMessage, rtDB_Transfer, 'Starting delayed device database read.'+
                    ' Locating device type: ' + IntToStr(Integer(FReadQueue[i].DeviceType)) + ' Address: ' + IntToHex(FReadQueue[i].DevAddress, 8));{$ENDIF}

                  idx := GetDatabase.GetDeviceIndex(FReadQueue[i].DeviceType, FReadQueue[i].DevAddress);
                  if idx <> -1 then
                  begin
                    {$IFDEF DEBUG}WriteToLog(rkMessage, rtDB_Transfer, 'Delayed reading device index=' + IntToStr(idx));{$ENDIF}
                    ReadDeviceRecord(idx, FReadQueue[i].DeviceType);
                    {$IFDEF DEBUG}WriteToLog(rkMessage, rtDB_TRansfer, 'Delayed device database read compete');{$ENDIF}
                  end;

                  if i < High(FReadQueue) then
                    move(FReadQueue[i+1], FReadQueue[i], Sizeof(TQueuedRead)* (High(FReadQueue) - i));

                  SetLength(FReadQueue, Length(FReadQueue) - 1);
                except
                  {$IFDEF DEBUG}WriteToLog(rkMessage, rtDB_Error, 'Error in delayed device database read');{$ENDIF}
                  dec(FReadQueue[i].RetryCount);
                  if FReadQueue[i].RetryCount <= 0 then
                  begin
                    if i < High(FReadQueue) then
                      move(FReadQueue[i+1], FReadQueue[i], Sizeof(TQueuedRead)* (High(FReadQueue) - i));

                    SetLength(FReadQueue, Length(FReadQueue) - 1);
                  end;
                end;
              end;
          finally
            FLock.Release;
          end;
        end;

        HandleNormalConnection(ReadingStartedAt);

      except
        On E: Exception do
          HandleRealtimeException(E);
      end;
    finally
      UpdateDeviceCounts;

      if Statefull.HasStateID(GetDeviceDriver, devInitializing) then
      begin
        if not FUIDReaded then
          Progress(0, IntToStr(Device.DeviceAddress) + ' - ' + FPanelDeviceMod.DeviceShortName + '. ' + rsMonitoringFinished + '.', 0, 0)
      end else
        DoBackgroundTasks;

      FEventList := nil;
    end;
  finally
    FInterface.FastLoseDetection := False;
  end;

  {$IFDEF DEBUG}
  finally
    WriteToLog(rkMessage, rtDB_Work, 'Finish reading log records');
    CurrentUserName := OldThreadName;
  end;
  {$ENDIF}

end;

function TRubezh3Device.GetRawAddrList(const SelfDevice: IDevice; Full: boolean = False): TRawAddressList;
var
  AddressList: TAddressList;

  procedure AppendAddress(Address, DeviceType: Integer);
  begin
    SetLength(AddressList.adresses, Length(AddressList.adresses) + 1);
    AddressList.adresses[High(AddressList.adresses)] := Address;

    SetLength(AddressList.types, Length(AddressList.types) + 1);
    AddressList.types[High(AddressList.types)] := DeviceType;
  end;

  function GetInternalDeviceType(const Device: IDevice): byte;
  var
    Rubezh3DeviceDriver: IRubezh3DeviceDriver;
  begin
    try
      if Supports(Device.DeviceDriver, IRubezh3DeviceDriver, Rubezh3DeviceDriver) then
        result := Rubezh3DeviceDriver.GetDatabaseModelExt.GetActiveMod.BaseNodeType else
        result := 0;
    except
      result := 0;
    end;
  end;

  function PresetDeviceInDevice(dtype, addr: integer): boolean;

  var Child: IDevice;
      Panels: IDeviceList;

  begin
    with SelfDevice.ParentDevice.EnumChildren do
      while Next(Child) = S_OK do
        if Child.DeviceAddress = addr then
        begin
          Panels := GetLinkedPanels(Child, dtype);
          result := Panels.IndexOf(SelfDevice) <> -1;
          break;
        end;
  end;

var
  MakeAddressList: IMakeAddressList;
  i, j: integer;
  Found: boolean;
  Child: IDevice;
begin
  FillChar(Result, Sizeof(Result), 0);

  // адресный лист монитора - все присутствующие в сети устройства,
  if Full then
  begin
    with SelfDevice.ParentDevice.EnumChildren do
      while Next(Child) = S_OK do
        if (optEventSource in Child.DeviceDriver.DriverOptions) and
          (not (Child.DeviceDriver.GetBaseType in [3,7, 100,101, 102])) then // БИ, ПДУ УОО-ТЛ
        begin
          // Необходимо узнать его код прибора.
          // Это можно сделать получив PanelDeviceMod
          AppendAddress(Child.DeviceAddress, GetInternalDeviceType(Child));
        end;

    FillChar(Result, Sizeof(Result), 0);

    for I := 0 to High(AddressList.adresses) do
    begin
      Result.adresses[i] := AddressList.adresses[i];
      Result.types[i] := AddressList.Types[i];
    end;
  end else
  begin
    // 0. Формируем адресный лист
    if not supports(SelfDevice.ParentDevice.DeviceDriver, IMakeAddressList, MakeAddressList) then
      exit;
    AddressList := MakeAddressList.MakeAddressList(SelfDevice.ParentDevice, false {GetIsIOCTLFunctionMode});

    // адресный лист прибора-мастера - все активные устройства в сети(кроме пассивов).
    // поэтому он будет непустым, только если устройство само есть в нем
    Found := False;
    for I := 0 to High(AddressList.adresses) do
      if AddressList.adresses[i] = SelfDevice.DeviceAddress then
      begin
        Found := True;
        break;
      end;

    FillChar(Result, Sizeof(Result), 0);

    if Found then
    begin

      // Мы являемся мастером
      for I := 0 to High(AddressList.adresses) do
      begin
        // исключаем со списка мониторов ПДУ и БИ которые не связаны с нами
        if AddressList.Types[i] in [3,7] then
          if not PresetDeviceInDevice(AddressList.Types[i], AddressList.adresses[i]) then
            AddressList.Types[i] := 0;

        Result.adresses[i] := AddressList.adresses[i];
        Result.types[i] := AddressList.Types[i];
      end;

    end else
    begin
      // Мы пассив :(
      // адресный лист прибора-пассива - только мониторинговые станции, 

      j := 0;
      for I := 0 to High(AddressList.adresses) do
        if (AddressList.Types[i] >= 100) or (AddressList.Types[i] in [3,7]) then
        begin
          // для приборов откидываем ПДУ и БИ в которых нет наших устройств
          if (SelfDevice.DeviceDriver.GetBaseType < 100) and
          not (SelfDevice.DeviceDriver.GetBaseType in [3,7]) then
            if AddressList.Types[i] in [3,7] then
              if not PresetDeviceInDevice(AddressList.Types[i], AddressList.adresses[i]) then
                continue;

          Result.adresses[j] := AddressList.adresses[i];
          Result.types[j] := AddressList.Types[i];
          inc(j);
        end;

    end;

  end;

  // Надо сортировать его. Иначе это вызывает проблемы
  with TRawAdrressListSort.Create(@result) do
  try
    Sort;
  finally
    free;
  end;  
end;

function TRubezh3Device.GetReservedLineState(LineNo: integer): TChannelState;
begin
  result := GetDeviceParamDef(Config, 'ReservedLineState_' + IntToStr(LineNo), csTesting);
end;

procedure TRubezh3Device.TransportOperationNotify(Sender: TObject);
begin
  BytesReaded := FInterface.BytesReaded;
  BytesWritten := FInterface.BytesWritten;
  if ((Now - LastProgressTime) > MIN_NOTIFY_TIME) or (FInterface.BytesTotal <> 0) then
  begin
    if (FInterface.BytesTotal <> 0) then
    begin
      FTotalSize := FInterface.BytesTotal;
      FCompleteSize := FInterface.BytesComplete;
      doProgress('', False);
    end else
      doProgress('', True);
  end;
end;

procedure TRubezh3Device.UpdateAddressList(const SelfDevice: IDevice);

  procedure SetParamValue(const ParamName: string; const ParamValue: Variant);
  var
    Param: IParam;
  begin
    Param := Self.DeviceParams.FindParam(ParamName);
    if Param <> nil then
      Param.SetValue(ParamValue);
  end;

var
  PanelAddressList, RawAddressList: TRawAddressList;
begin
  // 0. Формируем адресный лист
  if not supports(SelfDevice.ParentDevice.DeviceDriver, IMakeAddressList) then
    exit;

  RawAddressList := GetRawAddrList(SelfDevice);

  // 1. Читаем адресный лист из прибора
  PanelAddressList := FInterface.GetAddressList;

  {$IFDEF DEBUG}WriteToLog(rkMessage, rtOperation, 'Calculated address list: ' + BufferToHexString(RawAddressList, Sizeof(RawAddressList)));{$ENDIF}
  {$IFDEF DEBUG}WriteToLog(rkMessage, rtOperation, 'Readed address list: ' + BufferToHexString(PanelAddressList, Sizeof(PanelAddressList)));{$ENDIF}

  // 2. Сверяем его с тем, который сложился.
  // 3. Если не совпадает, то записываем новый
  if not CompareMem(@PanelAddressList.Adresses, @RawAddressList.Adresses, Sizeof(RawAddressList.Adresses)) then
    FInterface.SetAddressList(RawAddressList);

  (* в нормальном режиме работы прибора запись в БД невозможно, база заблокирована
     таблица мониторов пишется только при записи всей базы
  try
    if FInterface.Model <> nil then
      GetDatabase.WriteRuntimeData('AddressListTypes', RawAddressList.types, Sizeof(RawAddressList.types));
  except
    // При записи базы, в определенном случае будет ошибка. Игнориурем
  end;
  *)
end;

procedure TRubezh3Device.UpdateDoubling;

  procedure DoLogRestoreDoublingOnChannel(i: Integer);
  begin
    with LogDeviceActionNewEvent(True)^ do
    begin
      FLastDoubleAddressOnChannel[i] := FInterface.DoubleAddressOnChannel[i];
      EventClass := dscNormal;
      EventMessage := rsFixedDoubling + IntToStr(i+1);
      DateTime := Now;
      SysDateTime := Now;
    end;
  end;

  procedure DoLogFailDoublingOnChannel(i: Integer);
  begin
    with LogDeviceActionNewEvent^ do
    begin
      FLastDoubleAddressOnChannel[i] := FInterface.DoubleAddressOnChannel[i];
      EventClass := dscServiceRequired;
      EventMessage := rsIssueDoubling + IntToStr(i+1);
      DateTime := Now;
      SysDateTime := Now;
    end;
  end;

var i: integer;

begin
  for i := 0 to 1 do
    if FLastDoubleAddressOnChannel[i] <> FInterface.DoubleAddressOnChannel[i] then
      if FInterface.DoubleAddressOnChannel[i] then
        DoLogFailDoublingOnChannel(i)
      else
        DoLogRestoreDoublingOnChannel(i);
end;

procedure TRubezh3Device.UpdateReserveStatus(FirstDT: TDatetime);

  procedure DoLogRestoreReservedChannel(State, i: Integer);
  begin
//    Statefull.resetStateByID(GetDeviceDriver, State);
    with LogDeviceActionNewEvent(True)^ do
    begin
      EventClass := dscNormal;
      EventMessage := rsReservedChannel + ' ' + IntToStr(i) + ' ' + rsFunctional;
      DateTime := FirstDT;
      SysDateTime := FirstDT;
    end;
  end;

  procedure DoLogFailReservedChannel(State, i: Integer);
  begin
//    Statefull.SetStateByID(GetDeviceDriver, State);
    with LogDeviceActionNewEvent^ do
    begin
      EventClass := dscError;
      EventMessage := rsReservedChannel + ' ' + IntToStr(i) + ' ' +rsNotFunctional;
      DateTime := FirstDT;
      SysDateTime := FirstDT;
    end;
  end;

begin
  if (FInterface.ReserveLine1Fail = csFailed) and (GetReservedLineState(1) <> csFailed) then
  begin
    DoLogFailReservedChannel(psReservedLine1Failed, 1);
  end else
  if (FInterface.ReserveLine1Fail = csWorking) and (GetReservedLineState(1) <> csWorking) then
  begin
    DoLogRestoreReservedChannel(psReservedLine1Failed, 1);
  end;

  if (FInterface.ReserveLine2Fail = csFailed) and (GetReservedLineState(2) <> csFailed) then
  begin
    DoLogFailReservedChannel(psReservedLine2Failed, 2);
  end else
  if (FInterface.ReserveLine2Fail = csWorking) and (GetReservedLineState(2) <> csWorking) then
  begin
    DoLogRestoreReservedChannel(psReservedLine2Failed, 2);
  end;

  if not (FInterface.ReserveLine1Fail in [csTesting, csWarning]) then
    SetReservedLineState(1, FInterface.ReserveLine1Fail);

  if not (FInterface.ReserveLine2Fail in [csTesting, csWarning]) then
    SetReservedLineState(2, FInterface.ReserveLine2Fail);

end;

function TRubezh3Device.VerifyVersions(const VersionData: string): string;
var
  i: integer;
//  HasOlder, HasNewer: boolean;
  DeviceVersion, DeiviceCRC, LoaderVer: word;
  InSoftUpdate: Boolean;
begin
  // Режим работы прибора нам не важен, поскольку функция должна поддерживаться во всех режимах

  InSoftUpdate := FInterface.SoftUpdateMode;

  FInterface.SoftUpdateMode := True;
  try

{    HasOlder := False;
    HasNewer := False; }

    // Нам приходит Hex пакет с вырезанными данными
    with THexPackage.Create do
    try
      XMLLoadFromString(VersionData, [hxoSkipData]);

      if not CompatiblePackageDeviceName(RequiredDevice) then
        raise Exception.Create(rsIncompatibleFirmwarePackage);

      if FInterface.GetDeviceState = 8 then
      begin
        if FInterface.GetExtMemError <> 0 then
        begin
          result := rsWarningFirmwareCanBeLost;
          exit;
        end else
          FInterface.EndSoftUpdate_Reset;
      end;

      try
        LoaderVer  := FInterface.GetFirmwareInfo(02, DeiviceCRC);

        if (FInterface.GetFirmwareInfo(01, DeiviceCRC) = 0) or (LoaderVer = 0) or
          (hasMemType(memTypeAVR) and FInterface.HasAVRVersion(0)) then
            result := rsWarningNoFirmware else
        begin
          DeviceVersion := FInterface.GetFirmwareVersion;

          if DeviceVersion > Version then
           result := rsWarningOldFirmware else
          if DeviceVersion < Version then
            result := rsFirmwareUpdateRecommended else
            result := rsFirmwareSameVersion;
        end;
      except
        result := rsFirmwareVersionUnknown;
        LoaderVer := 0;
      end;

      // Если в пакете есть загрузчик - находим этот item и сверяем версии
      if (device.DeviceAddress <> 0) and (LoaderVer <> 0) then
        for I := 0 to ItemCount - 1 do
          if Item[i].MemoryType = memTypeLoaderFlash then
          begin
            if Hi(Item[i].Version) <> Hi(LoaderVer) then
              result := result + #13#10#13#10 +
                rsWarningFirmwareAndHardwareIncompatible;
            break;
          end;

{      if HasOlder then
        result := 'В приборе содержится более новая версия программного обеспечения. ' +
          'Вы уверены что хотите вернуться к старой версии ПО ?' else
      if HasNewer then
        result := 'Вы уверены что хотите обновить программное обеспечение в приборе ?' else
        result := 'В приборе уже установлена данная версия программного обеспечения. ' +
          'Продолжить ?'; }
    finally
      Free;
    end;


  finally
    FInterface.SoftUpdateMode := InSoftUpdate;
  end;
end;


type
  TLoadProgrammToPanel = function(amessage: pchar; messageSize: integer): integer; cdecl;
  TWriteConfigToDevice = function(config: pchar; configSize: integer;
    addressLine: byte; address: byte; amessage: pchar; messageSize: integer): integer;

function TRubezh3Device.WaitPanelUserMode: Boolean;
begin
  result := FInterface.WaitPanelUserMode;
end;

function TRubezh3Device.WriteRawProperties(const DeviceCondig: IDeviceConfig; const SelfDevice: IDevice): string;
var
  RawPropLibHandle: THandle;
  LoadProgrammToPanel: TLoadProgrammToPanel;
  ErrMsg: string;
  Child: IDevice;
  list: IInterfaceList;

  procedure ProcessDevice(const Device: IDevice);
  begin
    with Device.EnumChildren do
      while Next(Child) = S_OK do
      begin
        // У дочерних устройств должны быть 1. метка 2. Настроенные свойства
        if GetDevicePropDef(Child, 'SYS$SELECTED', False) and
          (GetDevicePropDef(Child, 'DeviceSettings', False) <> '') then
            list.Add(Child);

        ProcessDevice(Device);
      end;
  end;

begin
  RawPropLibHandle := SafeLoadLibrary(PChar(GetRawPropLibDLLFileName));
  if RawPropLibHandle = 0 then
    raise EPackageError.CreateResFmt(@sErrorLoadingPackage,
      [GetRawPropLibDLLFileName, SysErrorMessage(GetLastError)]);

  list := TInterfaceList.Create;

  ProcessDevice(SelfDevice);

  if list.Count = 0 then
    raise Exception.Create('Не указано ни одного устройства с настроенными свойствами');

  LoadProgrammToPanel := GetProcAddress(RawPropLibHandle, 'LoadProgrammToPanel');
  if @LoadProgrammToPanel <> nil then
  begin
    SetLength(ErrMsg, 250);
    if LoadProgrammToPanel(@ErrMsg[1], Length(ErrMsg)-1) = 0 then
      raise Exception.Create(ErrMsg);

    { 'DeviceSettings' }

  end;
end;

procedure TRubezh3Device.ResetIOCounter;
begin
  FInterface.ResetByteCount;
  inherited;
end;

procedure TRubezh3Device.SetPassword(Role: Integer; Password: TString);
var
  Addr: dword;
  psw: array[0..2] of byte;
  i, j, c, v: byte;
begin
  psw[0] := $FF;
  psw[1] := $FF;
  psw[2] := $FF;

  i := 1;
  j := 0;
  c := min(Length(Password), 6);
  while i <= c do
  begin
    v := (ord(Password[i]) - ord('0')) shl 4;
    inc(i);

    if i <= c then
      v := v or (ord(Password[i]) - ord('0')) else
      v := v or $0F;
    psw[j] := v;
    
    inc(i);
    inc(j);
  end;
  
  case Role of
    0: addr := 4 + 3;     {Инсталятор}
    1: addr := 4 + 3 + 3; {Администратор}
    else
      addr := 4;          {Оператор }
  end;

  FInterface.MemWrite(Addr, Sizeof(psw), @psw);
  
end;

Function TRubezh3Device.VerifyAdminPass(PassWord: TString): TString;
begin
  if FInterface.VerifyPanelAdminPassWord(Password) then
    result := '1' else result := '0';
end;

procedure TRubezh3Device.SetReservedLineState(LineNo: integer; Value: TChannelState);
begin
  Config.DeviceParams.SetParamValue('ReservedLineState_' + IntToStr(LineNo), Value);

  if LineNo = 1 then
  begin
    if Value = csFailed then
      Statefull.SetStateByID(GetDeviceDriver, psReservedLine1Failed) else
    if Value = csWorking then
      Statefull.ResetStateByID(GetDeviceDriver, psReservedLine1Failed);
  end else
  begin
    if Value = csFailed then
      Statefull.SetStateByID(GetDeviceDriver, psReservedLine2Failed) else
    if Value = csWorking then
      Statefull.ResetStateByID(GetDeviceDriver, psReservedLine2Failed);
  end;
end;

function TRubezh3Device.GetDescriptionString: TString;

  function GetVerInfo(const Prefix: string; MemType: word): string;
  var
    w: word;
    CRC: word;
  begin
    w := FInterface.GetFirmwareInfo(MemType, CRC);
    result := Prefix + ':' + IntToStr(DCB(Hi(w))) + '.' + IntToStr(DCB(Lo(w))) + '(0x' + IntToHex(CRC, 4)+ ')' ;
  end;

  function GetSettingsInfo: string;
  var
    DataRec: TDatarec;
  begin
    FInterface.GetUSBConfig(DataRec);

    result := ' Адрес: ' + IntToStr(Datarec.address) + '. Скорость: ';

    case Datarec.speed of
      0: result := result + '9600';
      1: result := result + '19200';
      2: result := result + '38400';
      3: result := result + '57600';
      4: result := result + '115200';
      else
        result := result + '???';
    end;

  end;

var
  Serial, DeviceType: string;
  w, CRC: word;
  i, State: integer;
  Percent: real;
  buf: pointer;
  AddrList: TRawAddressList;
  DevType: byte;
begin
  FInterface.SoftUpdateMode := True;
  State := FInterface.GetDeviceState;
  try
    case State of
    0:
    begin

      Serial := '';

      case FPanelDeviceMod.DeviceSubtype of
        dsOrdinaryPanel:
          begin
            Serial := #13#10 + rsFactoryNumber + ' : ' + GetDatabase.GetSerialNo;

            if GetDatabase.HasDatabase then
              Serial := Serial + #13#10 + rsDatabaseVersion +  ': ' + GetDatabase.GetVersionStr else
              Serial := Serial + #13#10 + rsNoDatabase;
          end;
{$IFDEF IMITATOR}
        dsImitator:
          begin
            w := FInterface.GetDatabaseVersion;
            Serial := rsDatabaseVersion +  ': ' + IntToStr(w);
          end;
{$ENDIF}
        dsIndicator, dsMDS, dsMDS5{$IFDEF RubezhRemoteControl}, dsRemoteControl, dsRemoteControlFire {$ENDIF}:
          begin
            Serial := #13#10 + rsFactoryNumber + ' : ' + FInterface.GetSerialNo + '.' + GetSettingsInfo;
            // для Мониторинговых станций нужно еще и адрес и скорость
          end;
      end;
   end;
   1:
   begin
      result := result + rsPanelServiceMode + #13#10;
      if (FInterface.GetFirmwareInfo(01, CRC) = 0) or (FInterface.GetFirmwareInfo(02, CRC) = 0) then
        begin
          result := rsPanelNoFirmware + ' ' + rsDoFirmwareUpdate;
          exit;
        end else
    {$IFDEF IMITATOR}
         if FPanelDeviceMod.DeviceSubtype = dsImitator then
           begin
             w := FInterface.GetDatabaseVersion;
             Serial := rsDatabaseVersion +  ': ' + IntToStr(w);
           end;
    {$ENDIF}
    end;
    8:
    begin
      result := rsPanelInServiceMode;
      FInterface.SoftUpdateMode := False;
      exit;
    end;
    end;

    w := FInterface.GetDeviceType;
    DevType := w;

    DeviceType := FPanelDeviceMod.DeviceName;
    for I := 0 to High(Local3Mods) do
      if Local3Mods[i].BaseNodeType = w then
      begin
        DeviceType := Local3Mods[i].DeviceName;
        break;
      end;

    w := FInterface.GetFirmwareVersion;

    result := DeviceType + #13#10 + result +
      rsFirmwareVersion + ': ' + IntToStr(WordRec(w).hi) + '.' + IntToStr(WordRec(w).Lo) + #13#10 + Serial;

//    Процент использования БД
    if (State = 0) and IsRubezh3Debug and
      (FPanelDeviceMod.DeviceSubtype = dsOrdinaryPanel) then
    begin
      Percent := FDatabase.GetFillDBPercent(0);
      if Percent <> 0 then
        result := result + #13#10 + format('%s: %4.2f%%', [rsPercentUsageDBInt, Percent]);

      Percent := FDatabase.GetFillDBPercent(1);
      if Percent <> 0 then
        result := result + #13#10 + format('%s: %4.2f%%', [rsPercentUsageDBExt, Percent]);
    end;

    // По просьбе Алексея Киндерова выводим адр. лист
    if (State = 0) and IsRubezh3Debug and (DevType <> $FF) then
    begin
      result := result + #13#10 + rsAddressList;
      AddrList := FInterface.GetAddressList;
      for i := 0 to 31 do
        if AddrList.adresses[i] = 0 then break;
      buf := @AddrList.adresses[0];
      result := result + #13#10 + rsAddressListAddresses + BufferToHexString(buf^, 32);
      result := result + #13#10 + 'Количество: ' + IntToStr(i);
//     buf := @AddrList.types[0];
//      result := result + #13#10 + rsAddressListTypes + BufferToHexString(buf^, 32);
     end;

//      result := result + #13#10 + rsPercentUsageDBExt + ': ' + FloatToStr(FDatabase.GetFillDBPercent(1))+'%';

{
    // Получаем версии всех компонентов.
    result := FPanelDeviceMod.DeviceName + #13#10 +
    'Версия прошивки: ' + GetVerInfo('USR', 01) + ';' + GetVerInfo('LDR', 02) + ';' +
      GetVerInfo('RSR', 03) + Serial;
}
  finally
    FInterface.SoftUpdateMode := False;
  end;


end;

function TRubezh3Device.GetGuardData: WideString;

  function CodeGuardData(Source: WideString): WideString;

    function ByteToStr(b: Byte): String;
    begin
      result:='';
      if (b and $01 <> 0) then result := result + '1' else result := result + '0';
      if (b and $02 <> 0) then result := result + '1' else result := result + '0';
      if (b and $04 <> 0) then result := result + '1' else result := result + '0';
      if (b and $08 <> 0) then result := result + '1' else result := result + '0';
      if (b and $10 <> 0) then result := result + '1' else result := result + '0';
      if (b and $20 <> 0) then result := result + '1' else result := result + '0';
      if (b and $40 <> 0) then result := result + '1' else result := result + '0';
      if (b and $80 <> 0) then result := result + '1' else result := result + '0';
    end;

  var j, k, addr, size: integer;
      b: Byte;
      s,s1: string;
      c: char;
  begin
    b:= Ord(Source[1]);
    result := format('%.3u', [b]);                // 3
    for j := 1 to 13 do
    begin
      s := Copy(Source,j+1,1);
      b := Ord(s[1]);
      result:= result + ByteToStr(b);
    end;                                          // 104

    for j := 1 to GUARD_USERS_COUNT do            // x80
    begin
      // Начальный адрес пользователя
      addr := 14 + (j-1)*GUARD_USER_DATA_SIZE +1;

      // Битовое поле атрибут пользователя
      b := Ord(Source[addr]);
      result := result + ByteToStr(b);            // 8

      // Имя пользователя
      s:= Copy(Source,addr+1, 20);
      s[20]:=' '; // с прибора приходит 20-й байт нулевой
      result := result + s; // 20

      // Ключ ТМ
        size := 6;
        s1 := Copy(Source, addr + 21, size);
        BinToHex(@s1[1],@s[1],size);
        SetLength(s,size*2);
        result := result + s;                    // 12

      // Пароль
        size := 3;
        s1 := Copy(Source, addr + 27, size);
        BinToHex(@s1[1],@s[1],size);
        SetLength(s,size*2);

//        s := s[2] + s[1] + s[4] + s[3] + s[6] + s[5];
        for k := 1 to size do
        begin
          c := s[2*k];
          s[2*k] := s[2*k-1];
          s[2*k-1] := c;
        end;
        result := result + s;                      // 6

      // Биты зон пользователя
      for k := 0 to 7 do
      begin
        s := Copy(Source,addr + 30 + k,1);
        b:= Ord(s[1]);
        result := result + ByteToStr(b);
      end;                                        // 64

      // Резервные биты зон пользователя
      for k := 0 to 7 do
      begin
        s := Copy(Source,addr + 38 + k,1);
        b:= Ord(s[1]);
        result := result + ByteToStr(b);
      end;                                        // 64
    end;
  end;

var i: integer;
    Answ: String;
    UserOffset: integer;
    Total: WideString;
begin
  FInterface.SoftUpdateMode := True;
  try
    if FInterface.GetDeviceState = 0 then
    begin
        Answ := ' ';
        SetLength(Answ,14);
        FInterface.MemRead(GUARD_DATA_OFFSET, 14, @Answ[1]);
        Total := Answ;
      for i := 1 to GUARD_USERS_COUNT do
      begin
        Answ := ' ';
        SetLength(Answ,GUARD_USER_DATA_SIZE);
        UserOffset := GUARD_DATA_OFFSET+14 + (i-1)*GUARD_USER_DATA_SIZE;
        FInterface.MemRead(UserOffset, GUARD_USER_DATA_SIZE, @Answ[1]);
        Total := Total + Answ;
      end;
      result := CodeGuardData(Total);
    end;
  finally
    FInterface.SoftUpdateMode := False;
  end;

end;

procedure TRubezh3Device.SetGuardData(GuardData: WideString);

  function BinStrToByte(str: string): Byte;
  begin
    result := 0;
    if Length(str) = 8 then
    begin
      if str[1] = '1' then result := result + 1;
      if str[2] = '1' then result := result + 2;
      if str[3] = '1' then result := result + 4;
      if str[4] = '1' then result := result + 8;
      if str[5] = '1' then result := result + 16;
      if str[6] = '1' then result := result + 32;
      if str[7] = '1' then result := result + 64;
      if str[8] = '1' then result := result + 128;
    end;
  end;

  function HexToByte(str: string): Byte;
  var b: byte;
  begin
    result := 0;
    if Length(str) = 2 then
    begin
      if (str[1] in ['A'..'F']) then
        b:=Ord(str[1])-55
      else if (str[1] in ['0'..'9']) then
        b:=Ord(str[1])-48
      else b:=0;
      result := b*16;

      if (str[2] in ['A'..'F']) then
        b:=Ord(str[2])-55
      else if (str[2] in ['0'..'9']) then
        b:=Ord(str[2])-48
      else b:=0;
      result := result + b;
    end;
  end;

  function DecStrToBCD(str: string): Byte;
  var b: byte;
  begin
    result := 0;

    if Length(str) = 2 then
    begin
      if (str[1] in ['0'..'9']) then
        b:=Ord(str[1])-48
      else b:=0;
      result := b;

      if (str[2] in ['0'..'9']) then
        b:=Ord(str[2])-48
      else b:=0;
      result := result + b*10;

      result := BCD(result);
      if str[1] = 'F' then
        result := result or $0F;
      if str[2] = 'F' then
        result := result or $F0;
    end;
  end;

var
    Stream: TMemoryStream;
    i,j, addr: integer;
    b: Byte;
    s: string;
    DataBlock: IDataBlock;
begin
  if not (FInterface.SendPing(DataBlock) = crAnswer) then
    raise Exception.Create(rsErrorUSBNoDevice);

  Stream := TMemoryStream.Create;
  Stream.SetSize(14 + GUARD_USERS_COUNT * GUARD_USER_DATA_SIZE);

  s := Copy(GuardData,1,3);
  b := StrToInt(s);
  Stream.Write(b,1);

  for i := 0 to 12 do
  begin
    s := Copy(GuardData,4+i*8,8);
    b := BinStrToByte(s);
    Stream.Write(b,1);
  end;

  for i := 1 to GUARD_USERS_COUNT do
  begin
    addr := 108 + (i-1)*174;

    s := Copy(GuardData,addr,8);
    b := BinStrToByte(s);
    Stream.Write(b,1);

    s:= Copy(GuardData,addr+8,20);
    for j := 1 to 20 do
      begin
      b := Ord(s[j]);
      // проверка доступных символов
      Stream.Write(b,1);
      end;

    for j := 0 to 5 do
    begin
      s:= Copy(GuardData, addr+28+2*j, 2);
      b:= HexToByte(s);
      Stream.Write(b, 1);
    end;

    for j := 0 to 2 do
    begin
      s:= Copy(GuardData, addr+40+2*j, 2);
      b:= DecStrToBCD(s);
      Stream.Write(b, 1);
    end;

    for j := 0 to 7 do
    begin
      s:= Copy(GuardData,addr+46+8*j, 8);
      b:=BinStrToByte(s);
      Stream.Write(b,1);
    end;

    for j := 0 to 7 do
    begin
      s:= Copy(GuardData,addr+110+8*j, 8);
      b:=BinStrToByte(s);
      Stream.Write(b,1);
    end;

  end;
  FInterface.SoftUpdateMode := True;

  FInterface.MemWrite(GUARD_DATA_OFFSET, Stream.Size, Stream.Memory);
  FInterface.SoftUpdateMode := False;

  Stream.Free;
end;

function TRubezh3Device.GetMDSData: WideString;

  function CodeData(Data: String; OnlyFilter: boolean = false): String;

    function ByteToStr(b: Byte): String;
    begin
      result:='';
      if (b and $01 <> 0) then result := result + '1' else result := result + '0';
      if (b and $02 <> 0) then result := result + '1' else result := result + '0';
      if (b and $04 <> 0) then result := result + '1' else result := result + '0';
      if (b and $08 <> 0) then result := result + '1' else result := result + '0';
      if (b and $10 <> 0) then result := result + '1' else result := result + '0';
      if (b and $20 <> 0) then result := result + '1' else result := result + '0';
      if (b and $40 <> 0) then result := result + '1' else result := result + '0';
      if (b and $80 <> 0) then result := result + '1' else result := result + '0';
    end;

  var num: string;
      j,i: integer;
      b: byte;
  begin
    if not OnlyFilter then
    begin
      setlength(num, 21);
      for j := 0 to 3 do
        begin
          for i := 1 to 21 do
            if Data[i+j*21] <> #0 then
              num[i] := Data[i+j*21] else
              num[i] := ' ';
           result := result + num;
        end;

      for i := 85 to 88 do
      begin
        b := ord(Data[i]);
        result := result + IntToStr(b);
      end;

      b := ord(Data[90]);
      i := b shl 8;
//      result := result + IntToStr(b);
      b := ord(Data[89]);
      i := i or b;
      result := result + Format('%3d', [i]);
//      result := result + IntToStr(b);

      for i := 91 to 94 do
      begin
        b := ord(Data[i]);
        result := result + IntToStr(b);
      end;

      for i := 95 to 102 do
      begin
        b := ord(Data[i]);
        result := result + ByteToStr(b);
      end;

    end else
      for i := 1 to 8 do
      begin
        b := ord(Data[i]);
        result := result + ByteToStr(b);
      end;

  end;

var Answ: WideString;
begin
  if FPanelDeviceMod.DeviceSubType = dsMDS5 then
  begin
    Answ := FInterface.MemReadBlockFromMDSDB(102);
    if Answ <> '' then
      result := CodeData(Answ)
  end else
  if FPanelDeviceMod.DeviceSubType = dsMDS then
  begin
    Answ := FInterface.MemReadBlockFromMDSDB(8);
    if Answ <> '' then
      result := CodeData(Answ, true)
  end;
end;

function TRubezh3Device.GetDescriptionStringFor(DeviceID: TString): TString;

  function GetVerInfo(const Prefix: string; MemType: word): string;
  var
    w: word;
    CRC: word;
  begin
    w := FInterface.GetFirmwareInfo(MemType, CRC);
    result := Prefix + ': ' + IntToStr(DCB(Hi(w))) + '.' + IntToStr(DCB(Lo(w)));
    // + '(0x' + IntToHex(CRC, 4)+ ')' ;
  end;

  function IsLocalPanelDeviceID(const DeviceID: string): boolean;
  var
    i: integer;
  begin
    result := false;
    for i := 0 to High(Local3Mods) do
      if SameText(Local3Mods[i].DriverID, DeviceID) then
      begin
        result := True;
        break;
      end;
  end;

var
  w: word;
  i, j: integer;
  buf: pointer;
  AddrList: TRawAddressList;
  MSChanelInfos: TRawMSChannelInfos;
  sAddress: string;
begin
  if not IsLocalPanelDeviceID(DeviceID) then
  begin
    // просто выводим инфу о версиях
    FInterface.SoftUpdateMode := True;
    try
      w := FInterface.GetFirmwareVersion;
      result := rsFirmwareVersion + ': ' + IntToStr(WordRec(w).hi) + '.' + IntToStr(WordRec(w).Lo);
// выводим адресный лист, закомментировано до реализации возвращения 14 параметра на МС-1/2
   if IsRubezh3Debug then
   begin
     result := result + #13#10 + rsAddressList;
     if DeviceID = sddMs01 then
     begin
       MSChanelInfos := FInterface.GetMSChannelInfo(1);
     end else
     if DeviceID = sddMs02 then
     begin
       MSChanelInfos := FInterface.GetMSChannelInfo(2);
     end;
     if (DeviceID = sddMs01) or (DeviceID = sddMs02) then
     begin
       for j := 0 to Length(MSChanelInfos) - 1 do
       begin
         for i := 0 to 31 do
           if MSChanelInfos[j].adresses[i] = 0 then break;
         result := result + #13#10 + 'Канал: '+ IntToStr(j+1) + ' ' +  'Адрес: ' + IntToHex(MSChanelInfos[j].SelfAddress, 2);
         case MSChanelInfos[j].Speed of
           0: sAddress := '9600';
           1: sAddress := '19200';
           2: sAddress := '38400';
           3: sAddress := '57600';
           4: sAddress := '115200';
           else sAddress := '???';
         end;
         result := result + ', Скорость: ' + sAddress;
         buf := @MSChanelInfos[j].adresses[0];
         result := result + ', ' +  rsAddressListAddresses + BufferToHexString(buf^, 32);
         result := result + #13#10 + 'Количество адресов: ' + IntToStr(i);
       end;
     end else
     begin
       for i := 0 to 31 do
         if AddrList.adresses[i] = 0 then break;
       buf := @AddrList.adresses[0];
       result := result + #13#10 + rsAddressListAddresses + BufferToHexString(buf^, 32);
       result := result + #13#10 + 'Количество: ' + IntToStr(i);
     end;
   end;

//      result := GetVerInfo('Версия загрузчика', 02) + #13#10  + GetVerInfo('Версия пользовательского ПО', 01);
    finally
      FInterface.SoftUpdateMode := False;
    end;

  end else
    result := GetDescriptionString;
end;

{$IFDEF Debug_DUMPMEM}
function TRubezh3Device.GetDump(Part: integer; var ASize: dword): String;
var Answ: String;
begin
  FInterface.SoftUpdateMode := True;
  try
    Answ := '';
    if FInterface.GetDeviceState = 0 then
    begin
      case Part of
        1: begin
             ASize := 32768;
             SetLength(Answ, ASize);
             FInterface.MemRead($28000, ASize, @Answ[1]);
           end;
        2: begin
             ASize := 16384;
             SetLength(Answ, ASize);
             FInterface.MemRead($30000, ASize, @Answ[1]);
           end;
      end;
    end;
    result := Answ;
  finally
    FInterface.SoftUpdateMode := False;
  end;
end;
{$ENDIF}

function TRubezh3Device.CalculateConfigMD5Cached(const ParentDevice: IDevice; HashVersion: integer): TMD5Digest;
var
  i: integer;
begin
  for I := 0 to High(FConfigHashCache) do
    if FConfigHashCache[i].version = HashVersion then
    begin
      result := FConfigHashCache[i].digest;
      exit;
    end;

  SetLength(FConfigHashCache, Length(FConfigHashCache) + 1);
  with FConfigHashCache[High(FConfigHashCache)] do
  begin
    version := HashVersion;
    digest := CalculateConfigMD5(ParentDevice, HashVersion);
    result := digest;
  end;


end;

function TRubezh3Device.ChildIOCTL_ExecuteFunction(const Device: IDevice; const FunctionCode: string; out Reason: string): boolean;
begin
  result := false;

{  if SameText(FunctionCode, 'Touch_SetMaster') then
  begin
    FInterface.ResetZoneState();
    result := true;
    reason := 'Приложенный в течении 30 секунд ключ будет назначен мастер-ключом';
  end; }

end;

procedure TRubezh3Device.ClearActionLog;
begin
  { Пока вообще не нужна }
end;

function TRubezh3Device.CompatiblePackageDeviceName(const Value: string): Boolean;
var
  DeviceType, PackageDeviceType: byte;

  function GetLocalPanelDeviceType(const DeviceName: string): byte;
  var
    i: integer;
  begin
    result := $FF;

    // изменилось имя "Блок индикации" - > "Рубеж БИ"
    // в предыдущих прошивках оно останется старым поэтому поправляем тип устройства вручную
    if SameText(DeviceName, 'Блок индикации') then
    begin
      result := 3;
      exit;
    end;
    
    for i := 0 to High(Local3Mods) do
      if SameText(Local3Mods[i].DeviceShortName, DeviceName) or
        ((Local3Mods[i].DeviceShortNameAlias <> '') and SameText(Local3Mods[i].DeviceShortNameAlias, DeviceName)) then
      begin
        result := Local3Mods[i].BaseNodeType;
        break;
      end;
  end;

begin

  // 18.01.10 - исправление для прошивки МС, чтобы проверка на совместимость
  // проходила раньше
  if (device.DeviceAddress = 0) then
  begin
    result := SameText(Value, ParentInstance.DeviceDriver.DriverShortName);
    exit;
  end;

  PackageDeviceType := GetLocalPanelDeviceType(Value);

  if (FPanelDeviceMod.BaseNodeType <> PackageDeviceType) then
  begin
    result := false;
    exit;
  end;

  // Проверяем сигнатуру базы
  try
    if GetDatabase.HasDatabase and (dword(GetDatabase.GetSignature) <> dword(doGetCurrentSignature)) then
    begin
      result := false;
      exit;
    end;
  except

  end;

  DeviceType := FInterface.GetDeviceType;
  result := (DeviceType = $FF) or (PackageDeviceType = DeviceType);

end;

function TRubezh3Device.GetActionLogRecords: IEnumlogRecords;
begin
  if FDeviceActionLogList <> nil then
  begin
    result := TEnumLogRecords.Create(FDeviceActionLogList, ioOwned);
    FDeviceActionLogList := nil;
  end else
    result := nil;
end;

(*function TRubezh3Device.DecodeASPTState(Index: Integer; DeviceType: TRubezh3DeviceType;
  IntState: word; ForState: TDeviceStateClass): string;

  procedure AppendResultStr(const s: string);
  begin
    if result <> '' then
      result := result + ', ';

    result := result + s;
  end;

  procedure CheckBit(Byte, BitNo: byte; errMessage: string);
  begin
    if GetBitRange(Byte, BitNo, BitNo) <> 0 then
      AppendResultStr(ErrMessage);
  end;

var
  DevAspt: TExtraPropsMPT;
begin
  {$IFDEF DEBUG}WriteToLog(rkMessage, 'Начало декодирования МПТ');{$ENDIF}
  GetDatabase.GetExtraPropsMpt(DeviceType, Index, @DevASPT);

  {  GetDatabase.ReadDeviceRecordBytes(DeviceType, Index, @DevAspt.Byte82,
    Integer(@DevAspt.Byte82) - Integer(@DevAspt.LocalAddress), 3); }

  result := '' ;
  // result + '[Декодируем ' + IntToHex(IntState, 4) + ', ' + IntToHex(DevAspt.Byte82, 2) + ', ' + IntToHex(DevAspt.Byte83, 2) + ']';

  if ForState in [dscError, dscServiceRequired] then
  begin
    // Байт 0x81
    CheckBit(IntState, 2, rsAsptVoltageLow);
    CheckBit(IntState, 3, rsAsptMemoryError);
    CheckBit(IntState, 4, rsAsptBreakage);
    CheckBit(IntState, 5, rsASPTShortCircuit);
    CheckBit(IntState, 6, rsASPTWireBreakage);
    CheckBit(IntState, 7, rsAsptVoltageLow2);

    // Байт 0x82
    CheckBit(DevAspt.Byte82, 0, rsShortCircuit + ' ' + rsWhatOutput + ' 1');
    CheckBit(DevAspt.Byte82, 1, rsShortCircuit + ' ' + rsWhatOutput + ' 2');
    CheckBit(DevAspt.Byte82, 2, rsShortCircuit + ' ' + rsWhatOutput + ' 3');
    CheckBit(DevAspt.Byte82, 3, rsShortCircuit + ' ' + rsWhatOutput + ' 4');
    CheckBit(DevAspt.Byte82, 4, rsShortCircuit + ' ' + rsWhatOutput + ' 5');

    // Байт 0x83
    CheckBit(DevAspt.Byte83, 0, rsWireBreakage + ' ' + rsWhatOutput + ' 1');
    CheckBit(DevAspt.Byte83, 1, rsWireBreakage + ' ' + rsWhatOutput + ' 2');
    CheckBit(DevAspt.Byte83, 2, rsWireBreakage + ' ' + rsWhatOutput + ' 3');
    CheckBit(DevAspt.Byte83, 3, rsWireBreakage + ' ' + rsWhatOutput + ' 4');
    CheckBit(DevAspt.Byte83, 4, rsWireBreakage + ' ' + rsWhatOutput + ' 5');
  end;

  if ForState in [dscNormalDefault] then
  begin
    CheckBit(DevAspt.Byte83, 5, rsAsptLaunchFinished);
  end;

  {$IFDEF DEBUG}WriteToLog(rkMessage, 'Конец декодирования МПТ');{$ENDIF}
end;

function TRubezh3Device.DecodeBoltState(Index: Integer; DeviceType: TRubezh3DeviceType;
  IntState: word; ForState: TDeviceStateClass): string;

  procedure AppendResultStr(const s: string);
  begin
    if result <> '' then
      result := result + ', ';

    result := result + s;
  end;

  procedure CheckBit(Byte: word; BitNo: byte; errMessage: string);
  begin
    if GetBitRange(Byte, BitNo, BitNo) <> 0 then
      AppendResultStr(ErrMessage);
  end;

  procedure CheckzBit(Byte: word; BitNo: byte; errMessage: string);
  begin
    if GetBitRange(Byte, BitNo, BitNo) = 0 then
      AppendResultStr(ErrMessage);
  end;

var
  DevBolt: TExtraPropsBolt;
begin
  result := '';

  if ForState in [dscError, dscServiceRequired] then
  begin
    GetDatabase.GetExtraPropsBolt(DeviceType, Index, @DevBolt);

    if IsRubezh3Debug then
      result := '[Декодируем ' + IntToHex(DevBolt.Byte83, 4) + ', ' + IntToHex(DevBolt.Byte88, 4) + ']';

    // Байт 0x81
    CheckBit(DevBolt.Byte83, 1, 'Обрыв концевика положения «Открыто»');
    CheckBit(DevBolt.Byte83, 2, 'КЗ концевика положения «Открыто»');

    CheckBit(DevBolt.Byte83, 4, 'Обрыв муфтового выключателя положения «Открыто»');
    CheckBit(DevBolt.Byte83, 5, 'КЗ муфтового выключателя положения «Открыто»');

    CheckBit(DevBolt.Byte83, 7, 'Обрыв концевика положения «Закрыто»');
    CheckBit(DevBolt.Byte83, 8 + 0, 'КЗ концевика положения «Закрыто»');

    CheckBit(DevBolt.Byte83, 8 + 2, 'Обрыв муфтового выключателя положения «Закрыто»');
    CheckBit(DevBolt.Byte83, 8 + 3, 'КЗ муфтового выключателя положения «Закрыто»');

    CheckzBit(DevBolt.Byte88, 1, 'Не задан режим');
    CheckzBit(DevBolt.Byte88, 3, rsBoltDoorOpen);
    CheckzBit(DevBolt.Byte88, 4, rsBoltPowerMalfunction);
    CheckzBit(DevBolt.Byte88, 5, rsBoltMoveTimeTooBig);
    CheckzBit(DevBolt.Byte88, 6, 'Не замкнулся пускатель');

    CheckzBit(DevBolt.Byte88, 8 + 1, 'Не задан режим');
    CheckzBit(DevBolt.Byte88, 8 + 3, rsBoltMalfunction1);
    CheckzBit(DevBolt.Byte88, 8 + 6, rsBoltMarkerLost);

    CheckzBit(IntState, 6, 'Заклинило');
  end;
end;

function TRubezh3Device.DecodeMdu1eState(Index: Integer; DeviceType: TRubezh3DeviceType;
  IntState: word; ForState: TDeviceStateClass; var AddDebug: string): string;

  procedure AppendResultStr(const s: string);
  begin
    if result <> '' then
      result := result + ', ';

    result := result + s;
  end;

  procedure CheckBit(Byte: word; BitNo: byte; errMessage: string);
  begin
    if GetBitRange(Byte, BitNo, BitNo) <> 0 then
      AppendResultStr(ErrMessage);
  end;

var
  DevMdu1e: TExtraPropsMdu1e;
begin
  result := '';

  if ForState in [dscError, dscServiceRequired] then
  begin
    GetDatabase.GetExtraPropsMdu1e(DeviceType, Index, @DevMdu1e);

    AddDebug := AddDebug + ' MDU 81h = ' + IntToHex(DevMdu1e.Param81h, 2) +
      ' 81L = ' + IntToHex(DevMdu1e.Param81l, 2);

    // Байт 0x81h
    CheckBit(DevMdu1e.Param81h, 0, 'Обрыв обмотки 2 двигателя');
    CheckBit(DevMdu1e.Param81h, 1, 'Обрыв обмотки 1 двигателя');
    CheckBit(DevMdu1e.Param81h, 2, 'Замкнуты оба концевика');
    CheckBit(DevMdu1e.Param81h, 3, 'Лимит времени достижения конечного выключателя');

    // Байт 0x81l
    CheckBit(DevMdu1e.Param81l, 0, 'Невозможно выполнить команду с ППКП');
    CheckBit(DevMdu1e.Param81l, 1, 'Низкое напряжение питания привода');
    CheckBit(DevMdu1e.Param81l, 2, 'Разрыв цепи кнопки "НОРМА"');
    CheckBit(DevMdu1e.Param81l, 3, 'Короткое замыкание кнопки "НОРМА"');
    CheckBit(DevMdu1e.Param81l, 4, 'Разрыв цепи кнопки "ЗАЩИТА"');
    CheckBit(DevMdu1e.Param81l, 5, 'Короткое замыкание цепи кнопки "ЗАЩИТА"');
    CheckBit(DevMdu1e.Param81l, 6, 'Обрыв цепи конечного выкл. "ЗАКРЫТО"');
    CheckBit(DevMdu1e.Param81l, 7, 'Обрыв цепи конечного выкл. "ОТКРЫТО"');

  end;
end;
*)

destructor TRubezh3Device.destroy;
begin
  FreeAndNil(FDeviceActionLogList);
  FLoaderRAMAddrStream.Free;
  FRuntimeCmdNames.Free;
  FRuntimeCmdUserInfos.Free;
  FRuntimeCmdParams.Free;
  FRuntimeCmdRequestIDs.Free;
  FLock.Free;
  inherited;
end;

procedure TRubezh3Device.DoBackgroundTasks;
var
  InternalTask: IInternalBackgroundTask;
begin
  try
    if FBackgroundTask <> nil then
    begin
      Assert(Supports(FBackgroundTask, IInternalBackgroundTask, InternalTask));
      If FPanelDeviceMod.DeviceSubtype in [dsIndicator, dsMDS, dsMDS5{$IFDEF RubezhRemoteControl}, dsRemoteControl, dsRemoteControlFire {$ENDIF}] then
        InternalTask.SetState(btsComplete) else
      while InternalTask.doQuantum do ;
    end;
  except
  end;
end;

function TRubezh3Device.GetBGTaskStateInfo: TBGTaskStateInfo;
var
  InternalTask: IInternalBackgroundTask;
begin
  try
    if FBackgroundTask <> nil then
    begin
      Assert(Supports(FBackgroundTask, IInternalBackgroundTask, InternalTask));
      result := InternalTask.StateInfo;
    end;
  except
  end;
end;

procedure TRubezh3Device.DoDatabaseHashIsDifferent(const Msg: string);
begin
  if GetDatabaseState <> dbInvalid then
    with LogDeviceActionNewEvent^ do
    begin
      EventClass := dscServiceRequired;
      EventMessage := Msg;
      DateTime := Now;
      SysDateTime := Now;
    end;
  SetDatabaseState(dbInvalid);
end;

procedure TRubezh3Device.SetTimeSlicer(const TimeSlicer: ITimeSlicer);
begin
  inherited;
  FInterface.SetTimeSlicer(TimeSlicer);
end;

function TRubezh3Device.SpoilDatabaseLoader: boolean;

    procedure DoRawMemWrite(Addr, ASize: Dword; Buffer: PChar);
    var
      p: pchar;
      sent, tosend: dword;
    begin
      p := Buffer;
      sent := 0;
      while sent < ASize do
      begin
        tosend := min(MAX_PROTOCOL_MEM_BLOCK_SIZE, ASize - sent);
        FInterface.RawPushBlock(Addr + sent, tosend, p^, True);
        FInterface.RawWriteRepeatable(Addr + sent, tosend, p^);
        inc(sent, tosend);
        inc(p, tosend);
      end;
    end;

var
  Zero: Integer;
begin
  result := false;
  Zero := 0;

  try
    case FPanelDeviceMod.DeviceSubtype of
      dsIndicator{$IFDEF RubezhRemoteControl}, dsRemoteControl, dsRemoteControlFire {$ENDIF}:
        begin
          {$IFDEF DEBUG}WriteToLog(rkMessage, rtDB_Work, 'Spoiling database indicator');{$ENDIF}
          DoRawMemWrite(INDICATOR_DB_START_OFFSET, 4, @Zero);
          result := true;
        end;
    end;
  except

  end;
end;

function TRubezh3Device.SpoilDatabaseUser: boolean;
var
  Zero: Integer;
  Start: dword;
begin
  Zero := 0;
  result := false;

  try

    case FPanelDeviceMod.DeviceSubtype of
      dsOrdinaryPanel:
        begin
          {$IFDEF DEBUG}WriteToLog(rkMessage, rtDB_Work, 'Spoiling database ordinary panel');{$ENDIF}
          try
            Start := FInterface.GetDatabaseStart;
          except
            Start := $100;
          end;

          FInterface.MemWrite(Start, 4, @Zero);

          result := true;
        end;
    end;
  except

  end;
end;


function TRubezh3Device.CreateBackgroundTask(
  TaskInfo: TBackgroundTaskInfo): IBackgroundTask;
begin
  result := TDeviceParamsTask.Create(TaskInfo, Self);
end;

function TRubezh3Device.GetExecutingTask: IBackgroundTask;
begin
  result := FBackgroundTask;
end;

procedure TRubezh3Device.SetChildState(const Device: IDeviceInstance; const State: IDeviceState; bSetState: Boolean;
  AddressHighPart: word);
var
  Child: IDeviceInstance;
begin
  with Device.EnumChildren do
    while Next(Child) = S_OK do
    if (Child.Device.DeviceDriver.AdressMask <> '')
    and ((Child.Device.DeviceDriver.DeviceClassID <> sdcRealASPTV3)
      or (Child.Device.DeviceDriver.DeviceClassID <> sdcOutputV3) )
    and (GetAddressHighPart(Child.Device) = AddressHighPart) then
    begin
      if bSetState then
        (Child as IStatefull).SetState(State) else
        (Child as IStatefull).ResetState(State);
      SetChildState(Child, State, bSetState, AddressHighPart);
    end;
end;

procedure TRubezh3Device.SetExecutingTask(
  const BackgroundTask: IBackgroundTask);
begin
  FBackgroundTask := BackgroundTask;
end;

procedure TRubezh3Device.SetIntErrorState(Value: TDatabaseState);
begin
  Config.DeviceParams.SetParamValue('IntErrorState', Integer(Value));
  if Value = dbValidated then
    Statefull.ResetStateByID(GetDeviceDriver, psIntError) else
    Statefull.SetStateByID(GetDeviceDriver, psIntError);
end;

procedure TRubezh3Device.RawRead(MemoryType: Integer; StartAddr, Size: LongWord; var Buf);
begin
  // Not realized
end;

function TRubezh3Device.RawWrite(MemoryType: Integer; StartAddr, Size: LongWord; var Buf): Boolean;

  procedure PushBlock(Addr: Longword);
  begin
    FInterface.RawPushBlock(Addr, Size, Buf, (MemoryType <> memTypeLoaderFlash));
  end;

begin
  case MemoryType of
    memTypeApp, memTypeLoaderFlash:
    begin
      // ROM загрузчик пишем в нулевой адрес.
      // App также пишем куда написано
      if StartAddr < $40000000 then
        PushBlock(StartAddr);
      FInterface.RawWriteRepeatable(StartAddr, Size, Buf);
      result := True;
    end;
    memTypeAVR:
    begin
      PushBlock($7D000 + StartAddr);

      // Пока не пишем ничего
//      DoWrite($7D000 + StartAddr, Size, Buf);
      result := True;
    end;
    memTypeLoaderRAM485, memTypeLoaderRAMUSB:
    begin
      // Если это USB
      if (FInterface.DataTransport.IsAlternateDevice and (MemoryType = memTypeLoaderRAMUSB)) or
        (not FInterface.DataTransport.IsAlternateDevice and (MemoryType = memTypeLoaderRAM485)) then
         // Запоминаем в стриме
         FLoaderRAMAddrStream.WriteBuffer(Buf, Size);
    end;
  end;
end;

procedure TRubezh3Device.ReadDeviceRecord(Index: Integer; DeviceType: TRubezh3DeviceType);

var
  addr: TDeviceAddress;
  Flags: TDatabaseDeviceFlags;
  gw: PDBTableGateway;
  AddDebug: string;

  procedure DeleteParamValue(const DeviceInstance: IDeviceInstance; const ParamName: string);
  var
    Param: IParam;
  begin
    Param := DeviceInstance.DeviceParams.FindParam(ParamName);
    if Param <> nil then
      DeviceInstance.DeviceParams.Remove(Param);
  end;

  function ValueChanged(Previous, Current: Double; PropertyTypeInfo: PPropertyTypeInfo): boolean;
  begin
    if (PropertyTypeInfo.ChangeDownDelta <> 0) or (PropertyTypeInfo.ChangeUpDelta <> 0) then
      result := ((Current > Previous) and (Current - Previous >= PropertyTypeInfo.ChangeUpDelta)) or
        ((Current < Previous) and (Previous - Current >= PropertyTypeInfo.ChangeDownDelta)) else
      result := Previous <> Current;
  end;

  procedure SetParamValuePrim(const DeviceInstance: IDeviceInstance;
    const ParamName: string; const ParamValue: Variant);
  var
    Param: IParam;
    TypeInfo: PPropertyTypeInfo;
  begin
    Param := DeviceInstance.DeviceParams.FindParam(ParamName);

    if Param <> nil then
    begin
      TypeInfo := DeviceInstance.DeviceDriver.FindSupportedDeviceParam(ParamName);
      if (TypeInfo.ValueType in [varInteger, varSmallInt, varDouble, varSingle]) and
        not VarIsNull(ParamValue) and not VarIsEmpty(ParamValue) and not IsNan(ParamValue) and
        not VarIsNull(Param.GetValue) and not VarIsEmpty(Param.GetValue) and not IsNan(Param.GetValue) and
        not ValueChanged(Param.GetValue, ParamValue, TypeInfo) then
        begin
          {$IFDEF DEBUG}
          try
            if VarToStr(Param.Getvalue) <> VarToStr(ParamValue) then
              WriteToLog(rkMessage, rtChangeParam, 'Skipping "param:" change name: ' + ParamName +
                'oldval: ' + VarToStr(Param.Getvalue) + ' newval: ' + VarToStr(ParamValue));
          except
          end;
          {$ENDIF}
          exit;
        end;
     {$IFDEF ParamsUseDelta}
      if not (Param.IsEqualValue(ParamValue)) then
        FServerReq.AddChangedDevice(DeviceInstance);
     {$ENDIF}
      Param.SetValue(ParamValue);
    end;
  end;

  function GetRealTableType: Integer;
  begin
    if (TRubezh3DeviceType(abs(gw.RawTableType)) = dtAspt) and (ddfInPPU in Flags) then
      result := Integer(dtPPU) else
      result := gw.RawTableType;
  end;

  procedure ValidateDeviceInstance(var DeviceInstance: IDeviceInstance);
  var
    sNo: string;
  begin
    if DeviceInstance = nil then
    begin
      // Спец обработка для ППУ
      if (TRubezh3DeviceType(abs(gw.RawTableType)) in [dtAspt, dtAM4T, dtAMT]) and (ddfInPPU in Flags) then
      begin
        DeviceInstance := FindDevice(GetDeviceDriver.GetDeviceRegistry, Self, sddPPU, addr, false);

        if (DeviceInstance <> nil) then
        begin
          if (Lo(addr) > Lo(DeviceInstance.DeviceAddress)) and
            GetDevicePropDef(DeviceInstance.Device, 'Disabled' + IntToStr(Lo(Addr) - Lo(DeviceInstance.DeviceAddress)), False) then
          begin
            sNo := IntToStr(Lo(Addr) - Lo(DeviceInstance.DeviceAddress));
            SetParamValuePrim(DeviceInstance, 'Pressure' + SNo, NULL);
            SetParamValuePrim(DeviceInstance, 'FailureType' + SNo, NULL);
            SetParamValuePrim(DeviceInstance, 'OtherMessage' + SNo, NULL);
            SetParamValuePrim(DeviceInstance, 'AlarmReason' + SNo, NULL);
            abort;
          end;
        end;
      end else
        DeviceInstance := FindDevice(GetDeviceDriver.GetDeviceRegistry, Self,
          gw.DeviceDriverID, addr);

      if DeviceInstance = nil then
      begin
        {$IFDEF DEBUG}
        WriteToLog(rkMessage, rtDB_Error, 'Не удалось найти соотв. устройство при чтении БД. Index = ' +
          IntToStr(Index) + ' Addr: ' + IntToHex(Addr, 4) + ' Тип: ' + gw.ShortName);
        {$ENDIF}
        fDelayedSynchronizationFailed := True;
        Abort;
      end;
    end;
  end;

  function GetRealParamName(var DeviceInstance: IDeviceInstance;
    const ParamName: string): string;
  var
    i: integer;
  begin
    ValidateDeviceInstance(DeviceInstance);

    if (TRubezh3DeviceType(abs(gw.RawTableType)) in [dtAM4T, dtAMT]) and (ddfInPPU in Flags) then
    begin
      // Случай ППУ. Необходимо выяснить каким каналом является данная АМ4Т
      // Алгоритм такой: нужно взять адрес из ППУ и вычесть свой адрес
      i := Lo(addr) - Lo(DeviceInstance.DeviceAddress);
      result := ParamName + IntToStr(i);
    end else
      result := ParamName;
  end;

  function GetParamValue(var DeviceInstance: IDeviceInstance;
    const ParamName: string): string;
  var
    Param: IParam;
  begin
    ValidateDeviceInstance(DeviceInstance);
    Param := DeviceInstance.DeviceParams.FindParam(GetRealParamName(DeviceInstance, ParamName));
    if Param <> nil then
      result := VarToStr(Param.GetValue) else
      result := '';
  end;

  function GetPropValue(var DeviceInstance: IDeviceInstance;
    const ParamName: string): string;
  var
    Param: IParam;
  begin
    ValidateDeviceInstance(DeviceInstance);
    Param := DeviceInstance.DeviceProperties.FindParam(GetRealParamName(DeviceInstance, ParamName));
    if Param <> nil then
      result := VarToStr(Param.GetValue) else
      result := '';
  end;

  procedure SetNotNANValue(var DeviceInstance: IDeviceInstance;
    const ParamName: string; const ParamValue: Double);
  begin
    if not isNan(ParamValue) then
    begin
      ValidateDeviceInstance(DeviceInstance);

      SetParamValuePrim(DeviceInstance, GetRealParamName(DeviceInstance, ParamName), ParamValue)
    end;
  end;


  procedure SetParamValue(var DeviceInstance: IDeviceInstance;
    const ParamName: string; const ParamValue: Variant);
  begin
    ValidateDeviceInstance(DeviceInstance);

    SetParamValuePrim(DeviceInstance, GetRealParamName(DeviceInstance, ParamName), ParamValue)
  end;

  procedure DoSetState(const DeviceInstance: IDeviceInstance; State: IDeviceState);
  begin
    if (TRubezh3DeviceType(abs(gw.RawTableType)) in [dtAM4T, dtAMT]) and (ddfInPPU in Flags) then
      DoSetAltState(DeviceInstance, State, Lo(addr) - Lo(DeviceInstance.DeviceAddress), PPUAltStates, 0) else
    if (State.GetStateCode = 'HardwareIgnore') and IsOutDeviceForValidate(DeviceInstance.Device) then
      // Не устанавливаем обход для ИУ
      exit else
      InstanceSetState(DeviceInstance, State);
  end;

  procedure DoResetState(const DeviceInstance: IDeviceInstance; State: IDeviceState);
  begin
    if (TRubezh3DeviceType(abs(gw.RawTableType)) in [dtAM4T, dtAMT]) and (ddfInPPU in Flags) then
      DoResetAltState(DeviceInstance, State, Lo(addr) - Lo(DeviceInstance.DeviceAddress), PPUAltStates) else
      (DeviceInstance as IStatefull).ResetState(State);
  end;

  function DoHasState(const DeviceInstance: IDeviceInstance; State: IDeviceState): boolean;
  begin
    if (TRubezh3DeviceType(abs(gw.RawTableType)) in [dtAM4T, dtAMT]) and (ddfInPPU in Flags) then
      result := HasAltState(DeviceInstance, State, Lo(addr) - Lo(DeviceInstance.DeviceAddress), PPUAltStates) else
      result := (DeviceInstance as IStatefull).HasState(State);
  end;

  procedure AppendResultStr(var result: string; const s: string);
  begin
    if result <> '' then
      result := result + ', ';

    result := result + s;
  end;


  function GetExtraData(ExtState: byte; IntState: word): IParams;

    procedure SetExtraParamValue(const Name: string; const Value: Variant);
    begin
      if result = nil then
        result := TParamsImpl.Create;

      result.SetParamValue(Name, Value);

    end;

  var
    DevAspt: TExtraPropsMPT;
    DevBolt: TExtraPropsBolt;
    DevMdu1e: TExtraPropsMdu1e;
    offset: integer;
    {$IFDEF DEBUG}
    i: integer;
    {$ENDIF}
  begin
    result := nil;

    SetExtraParamValue('Common_0x80L', Lo(IntState));
    SetExtraParamValue('Common_0x80H', Hi(IntState));
    SetExtraParamValue('Data_DB_0x80L', Lo(IntState));
    SetExtraParamValue('Data_DB_0x80H', Hi(IntState));
    SetExtraParamValue('Common_State', Hi(ExtState));

    if DeviceType in [dtASPT, dtPPU] then
    begin
      {$IFDEF DEBUG}WriteToLog(rkMessage, rtDB_Transfer, 'Чтение доп. параметров МПТ');{$ENDIF}
      GetDatabase.GetExtraPropsMpt(DeviceType, Index, @DevASPT);
      SetExtraParamValue('Data_Mpt_0x82', DevAspt.byte82);
      SetExtraParamValue('Data_Mpt_0x83', DevAspt.byte83);
      SetExtraParamValue('Data_Mpt_0x84', DevAspt.byte84);
    end else
    if DeviceType = dtBUNS then
    begin
      {$IFDEF DEBUG}WriteToLog(rkMessage, rtDB_Transfer, 'Чтение доп. параметров насосов');{$ENDIF}
      offset := GetDatabase.GetSubDeviceAddress(dtBUNS, 0, index);
      FInterface.MemRead(offset + 10, SizeOf(DevBolt), @DevBolt);

      SetExtraParamValue('Data_Pump_0x83L', Hi(DevBolt.Byte83));
      SetExtraParamValue('Data_Pump_0x83H', Lo(DevBolt.Byte83));
      SetExtraParamValue('Data_Pump_0x88L', Hi(DevBolt.Byte88));
      SetExtraParamValue('Data_Pump_0x88H', Lo(DevBolt.Byte88));
    end else
    if DeviceType = dtBolt then
    begin
      {$IFDEF DEBUG}WriteToLog(rkMessage, rtDB_Transfer, 'Чтение доп. параметров задвижки');{$ENDIF}
      GetDatabase.GetExtraPropsBolt(DeviceType, Index, @DevBolt);
      SetExtraParamValue('Data_Bolt_0x83L', Lo(DevBolt.Byte83));
      SetExtraParamValue('Data_Bolt_0x83H', Hi(DevBolt.Byte83));
      SetExtraParamValue('Data_Bolt_0x88L', Lo(DevBolt.Byte88));
      SetExtraParamValue('Data_Bolt_0x88H', Hi(DevBolt.Byte88));
    end else
    if DeviceType = dtMDU1e then
    begin
      {$IFDEF DEBUG}WriteToLog(rkMessage, rtDB_Transfer, 'Чтение доп. параметров МДУ-1Э');{$ENDIF}
      GetDatabase.GetExtraPropsMdu1e(DeviceType, Index, @DevMdu1e);
      SetExtraParamValue('Data_MDU_0x81L', DevMdu1e.Param81l);
      SetExtraParamValue('Data_MDU_0x81H', DevMdu1e.Param81h);
    end;

    {$IFDEF DEBUG}
    for i := 0 to Result.ParamCount - 1 do
      AddDebug := AddDebug  + ', ' + Result.ParamByIndex(i).GetName + ': ' +
        IntToHex(Result.ParamByIndex(i).GetValue, 2);
    {$ENDIF}

  end;

  function InitDevGateway(var DeviceInstance: IDeviceInstance; out Flags: TDatabaseDeviceFlags; out Addr: TDeviceAddress;
    out ExtState: TExtState; out IntState: word; out gw: PDBTableGateway): boolean;
  var
    sign: integer;
    dummy: byte;
    offset, tt: integer;
    NSDevice :IDeviceInstance;
    Driver: IDeviceDriver;
  begin
    result := false;
    Flags := [];

    if DeviceType = dtBUNS then
    begin
      addr := 0;
      IntState := 0;

      if Index > 0 then
      begin
        // для Modern нет первой записи самого БУНС
        if FPanelDeviceMod.DatabaseType = dbtModern then
          dec(index);

        offset := GetDatabase.GetSubDeviceAddress(dtBUNS, 0, index);
        FInterface.MemRead(offset + 1, 1, @dummy);
        addr := dummy;
        if addr < 12  then
          tt := 1024 else
          tt := Addr + 1024;
        gw := GetDBTableGatewayList3.FindByType(tt, 0);
        Assert(gw <> nil);

        NSDevice := FindDeviceInstance(Self, FindDeviceByAddress(Config,
          GetDeviceDriver.GetDeviceRegistry.GetDeviceDriver('AF05094E-4556-4CEE-A3F3-981149264E89'), 0));

        Driver := GetDeviceDriver.GetDeviceRegistry.GetDeviceDriver(gw.DeviceDriverID);
        Assert(Driver <> nil);

        addr := addr - tt + 1024;
        DeviceInstance := FindDeviceInstance(NSDevice, FindDeviceByAddress(NSDevice.Device, Driver, addr));
        FInterface.MemRead(offset + 9, 1, @ExtState);
      end else
      begin
        gw := GetDBTableGatewayList3.FindByType(10);
        DeviceInstance := FindDeviceInstance(Self,
          FindDeviceByAddress(Config, GetDeviceDriver.GetDeviceRegistry.GetDeviceDriver('AF05094E-4556-4CEE-A3F3-981149264E89'), 0));

        if FPanelDeviceMod.DatabaseType = dbtClassic then
        begin
          offset := GetDatabase.GetSubDeviceAddress(dtBUNS, 0, index);
          FInterface.MemRead(offset + 8, 1, @ExtState);
        end else
          ExtState := GetDatabase.ReadExtDeviceState(dtBuns, 0);
      end;

      if DeviceInstance = nil then
        exit;

      result := true;
    end else
    begin
      if GetDatabase.IsServiceDeviceType(DeviceType) then
        exit;

      // Получаем актуальное состояние устройства из БД панели
      addr := GetDatabase.GetDeviceAddress(DeviceType, Index, Dummy, Flags);

        // Для таблицы МУКО/МУКД   нужно извлечь еще уточняющий бит из таблицы
      if (ddfNegClapan in Flags) then
        sign := -1 else
        sign := 1;

        // Если шлейф неисправен, то не читаем ничего, чтобы не сбросить состояние потери связи
      if (HiByte(Addr) <> 0) and (not FLineStates[HiByte(Addr) - 1]) then
        exit;

      ExtState := GetDatabase.ReadExtDeviceState(DeviceType, Index);
      IntState := GetDatabase.ReadIntDeviceState(DeviceType, Index);

      if (DeviceType = dtAM1) and (Lo(IntState) <> 0) { subtype } then
        gw := GetDBTableGatewayList3.FindByType(Integer(DeviceType) * sign, Lo(IntState)) else
        gw := GetDBTableGatewayList3.FindByType(Integer(DeviceType) * sign);
      assert(gw <> nil);

      result := true;
    end;
  end;


var
  DeviceInstance: IDeviceInstance;
  DevState: IDeviceState;
  ExtState: TExtState;
  IntState: word;
  dState: TDeviceStateClass;
  States: set of TDeviceStateClass;
  i: integer;
  s: string;
  IsAlaramSet, IsFailureSet, IsOtherSet: boolean;
  ZoneState: Word;
  v, MinV, Dimension: integer;
  DevAM4T: TExtraPropsAm4t;
  vbool, OnlySelfType: boolean;
  ExtraData: IParams;
  StateP, StateNP: IDeviceState;

begin
  AddDebug := '';

  // Получаем актуальное состояние устройства из БД панели
  try
    if not InitDevGateway(DeviceInstance, Flags, addr, ExtState, IntState, gw) then
      exit;

    // Для НС и насосов анализируем только те состояния, в которых они явно указаны
    OnlySelfType := DeviceInstance <> nil;

    // Разбираем внешн. состояние устройства по битам находящимся в arDeviceStates
    for I := 0 to high(arDeviceStates) do
      with arDeviceStates[i] do
      if (not HasRange and HasBitNo) and
        IsCompatibleTableType(arDeviceStates[i], gw.RawTableType, OnlySelfType) then
//        ((not HasFilter and not OnlySelfType) or (FilterTableType = gw.RawTableType)) then
      begin
        ValidateDeviceInstance(DeviceInstance);
        if DeviceInstance <> nil then
        begin
          if HasZoneBitNo and IsRubezh3Debug and GetDatabase.GetZoneStateByDevice(DeviceType, Index, ZoneState) then
            AddDebug := rsZoneState + IntToHex(ZoneState, 4);

          vbool := CheckBitInStateWord(ExtState.b, BitNo);
          if inverse then
            vbool := not vbool;

          if vbool and
            (not HasZoneBitNo or not GetDatabase.GetZoneStateByDevice(DeviceType, Index, ZoneState) or
              CheckBitInStateWord(ZoneState, ZoneBitNo)) then
           begin
             DoSetState(DeviceInstance, Intf);
           end else
            DoResetState(DeviceInstance, Intf);

        end;
      end;

    // Разбираем внутр. состояние устройства

    IsAlaramSet := False;
    IsFailureSet := False;
    IsOtherSet := False;

    // 0x80 - состояние в базе корректно
    {$IFDEF TestPressure}
    if True then
    {$ELSE}   
    if not DoHasState(DeviceInstance, DeviceInstance.DeviceDriver.GetStateByCode('ConnectionLost')) then
    {$ENDIF}
    begin
      {$IFDEF TestPressure}
      if True then
      {$ELSE}    // у насосов нормальное состояние 0x00 (0x80 - потеря связи)
      if (((ExtState.b and $80) <> 0) and (DeviceType <> dtBUNS) )
      or (((ExtState.b and $80) = 0) and (DeviceType = dtBUNS)) then
      {$ENDIF}
      begin

        ValidateDeviceInstance(DeviceInstance);
        if not (DeviceType in [dtBUNS, dtBolt]) then
        begin
          stateP := DeviceInstance.DeviceDriver.GetStateByCode('Par_Line');
          stateNP := DeviceInstance.DeviceDriver.GetStateByCode('NPar_Line');
        end;
        if (stateP <> nil) and  (HiByte(Addr) <> 0) and not FNoLoopBreak[(HiByte(Addr) - 1) div 2] then
        begin
          // Для каждого датчика можно считать бит доступности с одного из шлейфов
          // mik: бит в след. байте за внешним состоянием по маске 0x01
          if (ExtState.ExtByte and $01) <> 0 then
          begin
            DoSetState(DeviceInstance, stateP);
            DoResetState(DeviceInstance, stateNP);
          end else
          begin
            DoSetState(DeviceInstance, stateNP);
            DoResetState(DeviceInstance, stateP);
          end;
        end else
        begin
          DoResetState(DeviceInstance, stateNP);
          DoResetState(DeviceInstance, stateP);
        end;

        for I := 0 to high(arDeviceStates) do
          with arDeviceStates[i] do
          if ((HasRange and HasBitNo) or HasIntBitNo)  and
//            ((not HasFilter and not OnlySelfType) or (FilterTableType = GetRealTableType {gw.RawTableType})) then
            IsCompatibleTableType(arDeviceStates[i], GetRealTableType, OnlySelfType) then
          begin
            ValidateDeviceInstance(DeviceInstance);
            if DeviceInstance <> nil then
            begin
              if HasIntBitNo then
              begin
                vbool := CheckBitInStateWord(IntState, IntBitNo);
                if inverse then
                  vbool := not vbool;

                if vbool then
                  DoSetState(DeviceInstance, Intf) else
                  DoResetState(DeviceInstance, Intf);
              end;

              if HasRange then
              begin
                vbool := CheckValueInStateWord(IntState, BitNo, EndBitNo, RangeValue);
                if inverse then
                  vbool := not vbool;

                if vbool then
                  DoSetState(DeviceInstance, Intf) else
                  DoResetState(DeviceInstance, Intf);
              end;

            end;
          end;

        // Анализируем реальную детальную причину состояния устройства
        States := [];
        with (DeviceInstance as IStatefull).EnumStates do
          while Next(DevState) = S_OK do
            States := States + [DevState.GetStateClass];

        ExtraData := GetExtraData(ExtState.b, IntState);

        for dState := Low(DState) to High(DState) do
          if (dState in States) then
          begin
            // формируем словарь доп. свойств по необходимости
            s := RubezhDBDecodeDeviceState(dState, ExtraData, gw);

            if s <> '' then
            begin
              if dState in [dscCritical, dscWarning] then
              begin
                SetParamValue(DeviceInstance, 'AlarmReason', s);
                IsAlaramSet := True;
              end else
              if dState in [dscError, dscServiceRequired] then
              begin
                if (GetParamValue(DeviceInstance, 'AlarmReason') <> s) then
                begin
                  SetParamValue(DeviceInstance, 'FailureType', s);
                  IsFailureSet := True;
                end;
              end else
              begin
                if (GetParamValue(DeviceInstance, 'FailureType') <> s) and
                  (GetParamValue(DeviceInstance, 'AlarmReason') <> s) then
                  begin
                    SetParamValue(DeviceInstance, 'OtherMessage', s);
                    IsOtherSet := True;
                  end;
              end;
            end;
          end;


        if gw.HasSmokeChanel then
        begin
          if Rubezh3ParamUnits = 'dB' then
          begin
            SetNotNANValue(DeviceInstance, 'Dustiness', GetDatabase.ReadDustiness(DeviceType, Addr)/100);
            SetNotNANValue(DeviceInstance, 'Smokiness', GetDatabase.ReadSmokiness(DeviceType, Addr)/100);
          end else
          begin
            SetNotNANValue(DeviceInstance, 'Dustiness', GetDatabase.ReadDustiness(DeviceType, Addr));
            SetNotNANValue(DeviceInstance, 'Smokiness', GetDatabase.ReadSmokiness(DeviceType, Addr));
          end;
        end;

        if gw.HasTempChanel then
          SetParamValue(DeviceInstance, 'Temperature', GetDatabase.ReadTemperature(DeviceType, Addr));

        if gw.HasPressureChanel then
        begin
          if (swap(IntState) and 2 = 0) and (swap(IntState) and 4 = 0) then
          begin
            GetDatabase.GetExtraPropsAm4t(DeviceType, Index, @DevAM4T);

            AddDebug := AddDebug + ' АМ4-Т Макс.: ' + IntToStr(DevAM4T.MaxValue) + 'бар, тек: ' + FormatFloat('##', DevAM4T.Value) +
             'ед., offs = ' + GetPropValue(DeviceInstance, 'Offset') + 'бар ver: ' + IntToStr(GetDevicePropDef(DeviceInstance.Device, 'PPUVersion', 0))
             + ' J1: ' + GetPropValue(DeviceInstance, 'Precision');
            if not IsNan(DevAM4T.Value) then
            begin
              case GetDevicePropDef(DeviceInstance.Device, 'PPUVersion', 0) of
                // старая (до 2012 г.)
                0: v := Round(DevAM4T.Value / 106 * DevAM4T.MaxValue) + StrToIntDef(GetPropValue(DeviceInstance, 'Offset'), 0);
                // новая. Имеет перемычку точности
                1:
                  begin
//                    MinV := StrToInt(GetPropValue(DeviceInstance, 'MinDispValue'));
//                    Dimension := DevAM4T.MaxValue - MinV;
                    if GetPropValue(DeviceInstance, 'Precision') = 'False' then
                    // без точности (75-225 единиц)
                      v := Round((DevAM4T.Value - 75) / 150 * DevAM4T.MaxValue / 2) + StrToIntDef(GetPropValue(DeviceInstance, 'Offset'), 0)
                    else
                    // с точностью (45-225 единиц)
                      v := Round((DevAM4T.Value - 45) / 180 * DevAM4T.MaxValue) + StrToIntDef(GetPropValue(DeviceInstance, 'Offset'), 0);
                  end;
              end;

              if v < 0  then
                v := 0;
              SetParamValue(DeviceInstance, 'Pressure', v);
            end else
              SetParamValue(DeviceInstance, 'Pressure', NAN);

          end else
            SetParamValue(DeviceInstance, 'Pressure', NAN);

          {$IFDEF TestPressure}
//            SetParamValue(DeviceInstance, 'Pressure', Random(DevAM4T.MaxValue));
          {$ENDIF}
            AddDebug := AddDebug + ', пороги:' + IntToStr(DevAM4T.MinTreshold)
            + ', ' + IntToStr(DevAM4T.MaxTreshold);
           AddDebug := AddDebug + ' v: ' + GetParamValue(DeviceInstance, 'Pressure') + 'бар ';

        end;

      end else
      begin
        // нет 0x80
        if gw.HasPressureChanel then
         SetParamValue(DeviceInstance, 'Pressure', NAN);
      end;

      if not IsFailureSet then
        SetParamValue(DeviceInstance, 'FailureType', '');

    end else
    begin
      SetParamValue(DeviceInstance, 'FailureType', rsConnectionLost);
      for I := 0 to high(arDeviceStates) do
        with arDeviceStates[i] do
        if (HasRange and HasBitNo) and IsCompatibleTableType(arDeviceStates[i], gw.RawTableType) then
          // (not HasFilter or (FilterTableType = gw.RawTableType)) then
          DoResetState(DeviceInstance, Intf);
      if gw.HasPressureChanel then
        SetParamValue(DeviceInstance, 'Pressure', NAN);

    end;

    if not IsAlaramSet then
      SetParamValue(DeviceInstance, 'AlarmReason', '');

    if not IsOtherSet then
    begin
      if IsRubezh3Debug then
        s := 'Internal state: 0x' + IntToHex(IntState, 4) +
        ' ext. state: 0x' + IntToHex(ExtState.b, 2) +
        ' ext. state 2: 0x' + IntToHex(ExtState.ExtByte, 2)  {' internal addr.: 0x' +
          IntToHex(GetDatabase.ReadIntDeviceStateAddr(DeviceType, Index), 8)} + ' ' + AddDebug else
        s := '';

      SetParamValue(DeviceInstance, 'OtherMessage', s);
    end;

  except
    On E: EAbort do else raise;
  end;

end;

function TRubezh3Device.GetGUIDIdentity(out UID: TGUID): Boolean;
begin

  if IsRubezh3FileDB or (FPanelDeviceMod.DeviceSubtype in [dsIndicator, dsMDS, dsMDS5
    {$IFDEF RubezhRemoteControl}, dsRemoteControl, dsRemoteControlFire {$ENDIF}
    {$IFDEF IMITATOR}, dsImitator {$ENDIF}]) then
  begin
    FillChar(FUID, SizeOf(FUID), 0);
    UID := FUID;
    result := true;
    exit;
  end;

  result := False;

  // если устройство потеряно ждем время задержки до след запроса
  if (FConnectionState = csConnectionLost) then
    MsgSleep(DELAY_SEND_DATA_TO_LOST_DEVICE * 1000);
//  and (now() - FInterface.LastSendBlockTime < OneDTSecond * DELAY_SEND_DATA_TO_LOST_DEVICE) then
//    exit;

  if not FUIDReaded then
  begin
    FInterface.FastLoseDetection := True;
    try
      try
        if Statefull.HasStateID(GetDeviceDriver, devInitializing) then
          Progress(0, IntToStr(Device.DeviceAddress) + ' - ' + FPanelDeviceMod.DeviceShortName + '. ' + rsMonitoringStarts, 0, 0);
        GetDatabase.GetPanelUID(FUID);
        if IsEmptyGUID(FUID) then
        begin
          {$IFDEF Rubezh3Developer}
          CoCreateGuid(FUID);
          {$IFDEF DEBUG}WriteToLog(rkMessage, rtOperation, 'Addr: '+IntToStr(Device.DeviceAddress) + ' UID: '+ GuidToString(FUid)+
            '. New UID generated');{$ENDIF}
          GetDatabase.SetPanelUID(FUID);
          {$ELSE}
          raise ERubezh3UserError.Create(rsErrorNoSerialNo, 0);
         {$ENDIF}
        end;

        if not GetDatabase.HasDatabase then
          raise ERubezh3UserError.Create(rsErrorNoDatabase, 0);

        FUIDReaded := True;
        result := True;
      except
        On E: Exception do
        begin
          DoBackgroundTasks;
          HandleRealtimeException(E);
          Progress(0, IntToStr(Device.DeviceAddress) + ' - ' + FPanelDeviceMod.DeviceShortName + '. ' + rsMonitoringFinished + '.', 0, 0);
        end;
      end;
    finally
      FInterface.FastLoseDetection := False;
    end;
  end else
    result := True;

  UID := FUID;
end;



function TRubezh3Device.GetIntErrorState: TDatabaseState;
begin
  result := GetDeviceParamDef(Config, 'IntErrorState', 0);
end;

function TRubezh3Device.GetLastProgressTime: TDateTime;
begin

end;

function TRubezh3Device.GetLineCount: integer;
begin
  result := FPanelDeviceMod.AVRCount * 2;
end;

function TRubezh3Device.GetMaxBlockSize: Integer;
begin
  result := 256;
end;

procedure TRubezh3Device.SetNeededEventRecord(EventRecord: PEventRecord);
begin
//  FOutDevicesStateKnown := True;
end;

procedure TRubezh3Device.SetSoftwareVersion(Version: Integer);
begin
  if (FPanelDeviceMod.DatabaseType = dbtClassic) and FLoaderReady then
  begin
    doProgress(rsPanelReloading, True);
    FInterface.EndSoftUpdate_Reset;
    FLoaderReady := False;
  end;

  // записать версию пакета
  try
    FInterface.SetFirmwareVersion(Version);
    if FInterface.GetFirmwareVersion <> Version then
    begin
      FInterface.SetFirmwareVersion(Version);
      if FInterface.GetFirmwareVersion <> Version then
        raise Exception.Create(rsErrorUnableToUpdateFWVersion)
    end;
  except

  end;
end;

procedure TRubezh3Device.SaveChildState(const Device: IDevice; const DeviceStorage: IDeviceStorage);
begin
end;

procedure TRubezh3Device.SaveRootParams(const DeviceStorage: IDeviceStorage);
begin
  DeviceStorage.GetRootParams.SetParamValue('StatesReaded' + IntToHex(Integer(Self), 8), FStatesReaded);
end;

procedure TRubezh3Device.RestoreChildState(const Device: IDevice; const DeviceStorage: IDeviceStorage);
begin

end;

procedure TRubezh3Device.RestoreRootParams(const DeviceStorage: IDeviceStorage);
begin
  FStatesReaded := GetParamDef(DeviceStorage.GetRootParams, 'StatesReaded' + IntToHex(Integer(Self), 8), False);
end;

constructor TRubezh3Device.Create(const Parent: IDeviceInstance;
  const Config: IDevice; PanelDeviceMod: PPanelDeviceMod; ServerReq: IServerRequesting);
begin
  FServerReq :=  ServerReq;
  inherited Create(Parent, Config, PanelDeviceMod, FServerReq);
{$IFDEF Debug_DUMPMEM}
  FMemDumpID := -1;
{$ENDIF}  
  FPanelDeviceMod := PanelDeviceMod;
  FIgnoreAdditions := TInterfaceList.Create;
  FIgnoreToRemove := TInterfaceList.Create;
  FRuntimeCmdDevices := TInterfaceList.Create;
  FRuntimeCmdNames := TStringList.Create;
  FRuntimeCmdUserInfos := TStringList.Create;
  FRuntimeCmdParams := TStringList.Create;
  FRuntimeCmdRequestIDs := TStringList.Create;
  FLock := TMsgLock.Create;
end;

procedure TRubezh3Device.AppendRuntimeCall(const Device: IDeviceInstance; const MethodName, AnUserInfo: string; Params: string = ''; RequestID: integer = 0);
begin
  FLock.Acquire;
  try
    if FConnectionState <> csConnected then
      raise Exception.Create('Управление устройством невозможно. Нет связи с прибором');

    FRuntimeCmdDevices.Add(Device);
    FRuntimeCmdNames.Add(MethodName);
    FRuntimeCmdUserInfos.Add(AnUserInfo);
    FRuntimeCmdParams.Add(Params);
    FRuntimeCmdRequestIDs.Add(IntToStr(RequestID));
  finally
    FLock.Release;
  end;
end;

procedure TRubezh3Device.AppendToIgnoreList(const Device: IDeviceInstance);
begin
  FLock.Acquire;
  try
{    if FConnectionState = csNotConnected then
      exit; }

    FIgnoreToRemove.Remove(Device);
    if FIgnoreAdditions.IndexOf(Device) = -1 then
      FIgnoreAdditions.Add(Device)
  finally
    FLock.Release;
  end;
end;

procedure TRubezh3Device.BeginUpdate(PackageVersion: Integer);
var
  ds: word;
begin
  {$IFDEF DEBUG}WriteToLog(rkMessage, rtOperation, 'BeginUpdate');{$ENDIF}

  FInterface.SoftUpdateMode := True;
  FUserUpdated := False;
  FPackageVersion := PackageVersion;
  FInterface.RawClearBlock;

  if FLoaderRAMAddrStream = nil then
    FLoaderRAMAddrStream := TMemoryStream.Create else
    FLoaderRAMAddrStream.Clear;
  FLoaderReady := False;

  // Если мы уже не находимся в режиме обновления
  ds := FInterface.GetDeviceState;
  if ds = 0 then
  begin
    if FNeedClearDatabase then
    begin
      {$IFDEF DEBUG}WriteToLog(rkMessage, rtOperation, 'Going to spoil database user BU');{$ENDIF}
      FNeedClearDatabase := not SpoilDatabaseUser;
    end;

    doProgress(rsSettingFirmwareUpdateMode, True);
    // Переходим в режим обновления ПО
    try
      FInterface.BeginSoftUpdate(1);
    except
      // Игнорируем ошибку
    end;

    if FInterface.GetDeviceState and $01 = 0 then
      raise Exception.Create(rsUnableToSetFWUpdateMode);

  end else
  begin
    if (ds = 8) and (FInterface.GetExtMemError <> 0) then
      FLoaderReady := True;
  end;

  if FNeedClearDatabase then
  begin
    {$IFDEF DEBUG}WriteToLog(rkMessage, rtOperation, 'Going to spoil database loader BU');{$ENDIF}
    FNeedClearDatabase := not SpoilDatabaseLoader;
  end;
end;

procedure TRubezh3Device.BeginUpdateMemType(MemoryType, AddrLow, AddrHigh: Integer);

  procedure MemWrite(Addr, ASize: Dword; Buffer: PChar);
  var
    sent, tosend: dword;
    p: pchar;
  begin
    p := Buffer;
    sent := 0;

    while sent < ASize do
    begin
      tosend := min(MAX_PROTOCOL_MEM_BLOCK_SIZE, ASize - sent);
      RawWrite(memTypeApp, Addr + sent, tosend, p^);
//      VerifyBlock(Addr + sent, tosend, p^);
      inc(sent, tosend);
      inc(p, tosend);
    end;
  end;

  procedure PrepareLoader;
  begin
    if not FLoaderReady then
    begin
      // Нужно проверить есть ли у нас уже RAM загрузчик
      if (FLoaderRAMAddrStream = nil) or (FLoaderRAMAddrStream.Size = 0) then
        raise Exception.Create(rsNoRAMLoaderInPackage);

      doPrepareRAMLoader($40000000 {+ AddrLow}, FLoaderRAMAddrStream);
      FLoaderReady := True;
    end;
  end;

begin
  case MemoryType of
    memTypeLoaderRAM485, memTypeLoaderRAMUSB:
      begin
        // Этот тип мы будем копить в буфере и ничего пока делать не будем...;
        if (FInterface.DataTransport.IsAlternateDevice and (MemoryType = memTypeLoaderRAMUSB)) or
          (not FInterface.DataTransport.IsAlternateDevice and (MemoryType = memTypeLoaderRAM485)) then
        begin
          if FLoaderRAMAddrStream = nil then
            FLoaderRAMAddrStream := TMemoryStream.Create else
            FLoaderRAMAddrStream.Clear;
        end;
      end;
    memTypeLoaderFlash, memTypeAVR:
      begin
        PrepareLoader;

        case MemoryType of
          memTypeLoaderFlash: doProgress(rsWritingROMLoader, True);
          memTypeAVR: doProgress(rsWritingAVR, True);
        end;

      end;
    memTypeApp:
    begin
      if (FPanelDeviceMod.DatabaseType <> dbtModern) or (device.DeviceAddress = 0) then
        begin
          if FLoaderReady then
          begin
            doProgress(rsPanelReloading, True);
            FInterface.EndSoftUpdate_Reset;
            FLoaderReady := False;
          end;

          if FInterface.GetDeviceState = 0 then
          begin

            doProgress(rsSettingFirmwareUpdateMode, True);
            // Переходим в режим обновления ПО
            try
              FInterface.BeginSoftUpdate(1);
            except
              // Игнорируем ошибку
            end;

            if FInterface.GetDeviceState and $01 = 0 then
              raise Exception.Create(rsUnableToSetFWUpdateMode);
          end;
        end else
          PrepareLoader;

      // Стираем память
      doProgress(rsClearingMemorySectors + ' ' + IntToStr(FInterface.GetMemBlockByAddress(AddrLow)) + ' - ' + IntToStr(FInterface.GetMemBlockByAddress(AddrHigh)), True);
      FInterface.MemClear(FInterface.GetMemBlockByAddress(AddrLow), FInterface.GetMemBlockByAddress(AddrHigh));
    end;
  end;
end;

  type
    TColorRec = packed record
      case Integer of
        0: (Value: Longint);
        1: (Red, Green, Blue: Byte);
        2: (R, G, B, Flag: Byte);
        {$IFDEF MSWINDOWS}
        3: (Index: Word); // GetSysColor, PaletteIndex
        {$ENDIF MSWINDOWS}
      end;

function TRubezh3Device.GetAllRecordsText(HTML: Boolean; SubSystem: TEventSubSystem = essAll): string;

var
  CurLogType: byte;

  function DoFormatDeviceAddr(Address: word; Mask: string): string;
  begin
    if Mask <> '' then
      with TBitFieldValue.Create do
      try
        BitEditMask := Mask;
        Value := Address;
        result := AsString;
      finally
        free;
      end else
        result := IntToStr(Address);
  end;

  function ColorToHTML(const Color: Integer): String;

    function ColorToRGB(Color: Integer): Longint;
    begin
      if Color < 0 then
        Result := GetSysColor(Color and $000000FF) else
        Result := Color;
    end;

  var
    Temp: TColorRec;
  begin
    Temp.Value := ColorToRGB(Color);
    Result := Format('#%.2x%.2x%.2x', [Temp.R, Temp.G, Temp.B]);
  end;

  var PanelLastIndex: integer;

  procedure ReadRecord(Index: dword);
  var
    LogRecord: TRawEventRecord32;
    EventLogGateway: PEventLogGateway;
    s, s1: string;
    Indicator, IndCopy: Shortint;
    EventClass: TDeviceStateClass;
    EventMessage: string;
    ERI: TEventReasonInfo;
    addr: integer;
  begin
    if ((FTotalSize div 100) = 0) then
      doProgress(rsReadingEvents + '. ' + rsTotal+ ' ' + IntToStr(FTotalSize)) else
    if ((FCompleteSize mod (FTotalSize div 100)) = 0) then
      doProgress(rsReadingEvents + '. ' + rsTotal+ ' ' + IntToStr(FTotalSize));

    if CurLogType = LOG_FIRE_TYPE_32byte then
//    if FInterface.Model.IsEventLog32 then
      FInterface.EventLogReadFullRecord32(CurLogType, Index, LogRecord) else
      FInterface.EventLogReadFullRecord16(CurLogType, Index, LogRecord.Record16);

//    LogRecord.Record16.EventTime := LogRecord.Record16.EventTime;
    s := Format('<td>%d</td>', [PanelLastIndex - Index + 1]) +
      Format('<td>%2.2d.%2.2d.%2.2d<br></td><td>%2.2d:%2.2d:%2.2d<br></td>', [
        GetBitRange(LogRecord.Record16.EventTime, 0, 4), GetBitRange(LogRecord.Record16.EventTime, 5, 8),
        GetBitRange(LogRecord.Record16.EventTime, 9, 14),
        GetBitRange(LogRecord.Record16.EventTime, 15, 19), GetBitRange(LogRecord.Record16.EventTime, 20, 25),
        GetBitRange(LogRecord.Record16.EventTime, 26, 31)]);

    EventLogGateway := GetEventLogGatewayList3.FindByRawEventCode(LogRecord.Record16.EventCode);
    if EventLogGateway <> nil then
    begin
      indicator := GetEventIndicatorValue(EventLogGateway, LogRecord.Record16);
//      EventClass := GetEventClass(EventLogGateway, LogRecord.Record16);
      EventMessage := ParseEventLogMessage(EventLogGateway.EventMessage, Indicator);

      if (LogRecord.Record16.EventCode = $80) and (LogRecord.Record16.Context[1] <> 0) then
      begin
        EventMessage := rsInputFailure;
        if GetEventIndicatorValue(EventLogGateway, LogRecord.Record16) = 0 then
          EventMessage := EventMessage + ' ' + rsFailureSolved;
      end;

      GetDBTableGatewayByEvent(FInterface.Model, EventLogGateway, @LogRecord, addr, s1, IndCopy, nil);
      EventClass := GetEventClassByVal(EventLogGateway, IndCopy);

      if s1 <> '' then
        EventMessage := s1;

      s := s + '<td>' + EventMessage + '<br></td>';
      s := s + '<td>' + DoDecodeEventAttributes(FInterface.Model, GetDeviceDriver.GetDeviceRegistry,
        @LogRecord, Sizeof(LogRecord), ERI) + '<br></td>';
      if EventClass in [dscNormal, dscNormalDefault] then
        s := '<tr BGColor=' + ColorToHTML($FFFFFF) + '>' + s else
        s := '<tr BGColor=' + ColorToHTML(DeviceStateClassColor(EventClass)) + '>' + s;

    end else
      s := '<tr>' + s + '<td>' + rsUnknownEventCode + ' ' +IntToStr(LogRecord.Record16.EventCode) +'</td>';

    s := s + '</tr>';
    result := s + result;
    inc(FCompleteSize);
  end;

var
  BufSize, i: Integer;
begin
  result := '';

// Читаем пожарные события
  if (SubSystem in [essFire, essAll]) then
  begin
    if FPanelDeviceMod.DatabaseType = dbtModern then
//    if FPanelDeviceMod.AVRCount > 1 then
      CurLogType := LOG_FIRE_TYPE_32byte else
      CurLogType := LOG_FIRE_TYPE_16byte;

    PanelLastIndex := FInterface.EventLogLastRecord(CurLogType);
    BufSize := FInterface.EventLogBufferSize(CurLogType);

    try
      // читаем либо от нуля, либо от Last - 1000
      FTotalSize := PanelLastIndex - Max(0, PanelLastIndex - BufSize + 1) + 1;
      FCompleteSize := 0;
      for I := Max(0, PanelLastIndex - BufSize + 1) to PanelLastIndex do
        ReadRecord(i);
    except
      On E: EAbort do exit;
      else raise;
    end;
    result := '<tr><th colspan="5" BGColor="#75FF75" scope="col">Пожарные события</th></tr>' + result;
  end;
{$IFNDEF DISABLE_SECURITY}
// Читаем охранные события
  if (SubSystem in [essSecurity, essAll]) then
  begin
    if FPanelDeviceMod.DatabaseType = dbtModern then
      CurLogType := LOG_SEC_TYPE_32byte else
      CurLogType := LOG_SEC_TYPE_16byte;
    PanelLastIndex := FInterface.EventLogLastRecord(CurLogType);
    // 25.03.2011 Пока что вручную указываем 500 охранных событий
    // До устранения возвращения буфера прибором
//    BufSize := FInterface.EventLogBufferSize(CurLogType);
    BufSize := 500;
    try
      // читаем либо от нуля, либо от Last - 1000
      FTotalSize := PanelLastIndex - Max(0, PanelLastIndex - BufSize + 1) + 1;
      FCompleteSize := 0;
      for I := Max(0, PanelLastIndex - BufSize + 1) to PanelLastIndex do
        ReadRecord(i);
    except
      On E: EAbort do exit;
      else raise;
    end;
    result := '<tr><th colspan="5" BGColor="#75FF75" scope="col">Охранные события</th></tr>' + result;
  end;
{$ENDIF}
  // Вставка заголовка
  if Result <> '' then
    result := '<h2>' + rsDeviceEventLog + ' ' + GetDeviceText(Device, [dtoParents, dtoShortName, dtoNameFirst]) +
      '</h2><br>'  + rsReadingFinished + ' ' + DateTimeToStr(Now) + '<br><table Border=1 BorderColor="#BABABA"  CellSpacing=1 CellPadding=3 Border=0><tr BGColor="#F0F0F0">'+
      '<th>' + '№ </th><th>' + rsDate + '</th><th>' + rsTime + '</th><th>' + rsEvent + '</th><th>' + rsDetailedInfo + '</th></tr>' + result + '</table>';
end;

function TRubezh3Device.GetByteMode(MemoryType: Integer): TByteMode;
begin
  result := bmBuffers;
end;

procedure TRubezh3Device.DoSynchronizationRestored;
begin
  with LogDeviceActionNewEvent^ do
  begin
    EventClass := dscNormal;
    EventMessage := rsDatabaseSyncronized;
    DateTime := Now;
    SysDateTime := Now;
  end;
end;

procedure TRubezh3Device.doValidateDeviceType;
var
  dtype: integer;
begin
  dtype := FInterface.GetDeviceType;
  if (dtype <> $FF) and (FPanelDeviceMod.BaseNodeType <> dtype) then
    raise ERubezh3UserError.Create(rsPanelIncompatibleType, 0);
end;

procedure TRubezh3Device.EndUpdate;
begin
  try
    if FNeedClearDatabase then
    begin
      {$IFDEF DEBUG}WriteToLog(rkMessage, rtOperation, 'Going to spoil database loader AU');{$ENDIF}
      FNeedClearDatabase := not SpoilDatabaseLoader;
    end;

    // Сброс прибора
    doProgress(rsPanelReloading, True);
    {$IFDEF DEBUG}WriteToLog(rkMessage, rtOperation, 'EndUpdate - resetting');{$ENDIF}
    FInterface.EndSoftUpdate_Reset;

    {$IFDEF DEBUG}WriteToLog(rkMessage, rtOperation, 'EndUpdate - Waiting user mode. Pass 1');{$ENDIF}

{$IFDEF IMITATOR} // после перезагрузки Имитатор уходит в загрузчик, т.к. отсутствует БД
    if FPanelDeviceMod.DeviceSubtype <> dsImitator then
{$ENDIF}
      if not WaitPanelUserMode then
      begin
        if FUserUpdated then
          raise Exception.Create(rsFWUpdateFailed) else
          raise Exception.Create(rsNoUserFirmware);
      end;
    {$IFDEF DEBUG}WriteToLog(rkMessage, rtOperation, 'EndUpdate - checking DeviState <> 0. Pass 1 complete');{$ENDIF}

(*    // Проверяем, что обновление успешно
    if FInterface.GetDeviceState <> 0 then
    begin
      {$IFDEF DEBUG}WriteToLog(rkMessage, 'EndUpdate - checking DeviState <> 0. Pass 1 failed');{$ENDIF}
      msgSleep(4000);

      {$IFDEF DEBUG}WriteToLog(rkMessage, 'EndUpdate - checking DeviState <> 0. Pass 2');{$ENDIF}
      if FInterface.GetDeviceState <> 0 then
      begin
        {$IFDEF DEBUG}WriteToLog(rkMessage, 'EndUpdate - checking DeviState <> 0. Pass 2 failed');{$ENDIF}
        if FUserUpdated then
          raise Exception.Create(rsFWUpdateFailed) else
          raise Exception.Create(rsNoUserFirmware);
      end;
      {$IFDEF DEBUG}WriteToLog(rkMessage, 'EndUpdate - checking DeviState <> 0. Pass 2 complete');{$ENDIF}
    end else
    begin
    {$IFDEF DEBUG}WriteToLog(rkMessage, 'EndUpdate - checking DeviState <> 0. Pass 1 complete');{$ENDIF}
    end; *)

    if FNeedClearDatabase then
    begin
      {$IFDEF DEBUG}WriteToLog(rkMessage, rtOperation, 'Going to spoil database user AU');{$ENDIF}
      FNeedClearDatabase := not SpoilDatabaseUser;
      FInterface.EndSoftUpdate_Reset;
    end;

//    FInterface.DatabaseSetLock(True);
//    FInterface.DatabaseSetLock(False);

  finally
    FInterface.SoftUpdateMode := False;
    {$IFDEF DEBUG}WriteToLog(rkMessage, rtOperation, 'EndUpdate');{$ENDIF}
  end;
end;

procedure TRubezh3Device.EndUpdateMemType;

  procedure DoWriteAllAVR;
  var
    i: integer;
  begin
    for I := 1 to FPanelDeviceMod.AVRCount do
    begin
      if not FAVRInfo[i] then
        FInterface.RawWriteRepeatable(FInterface.LastSectorAddr, FInterface.LastSector.Size, FInterface.LastSector.Memory^);

      FInterface.LastSectorAddr := FInterface.LastSectorAddr + $2000; // переходим к следующему AVR
      FInterface.LastSectorNo := FInterface.LastSectorNo + 1;
    end;
  end;

begin
  if MemoryType = memTypeApp then
    FUserUpdated := True else
  if (MemoryType = memTypeAVR) and (FInterface.LastSector <> nil) and (FInterface.LastSector.Size <> 0) then
    DoWriteAllAVR;
end;

{ TDeviceParamsTask }

constructor TDeviceParamsTask.Create(Info: TBackgroundTaskInfo;
  const Device: TRubezh3Device);
begin
  inherited Create(Info);
  FDevice := Device;
  FRecordNo := -1;
end;

procedure TDeviceParamsTask.InternalQuantum;

{    procedure CalculateTotalRecords;
    var
      i: integer;
      Header: PDBRawTableHeader;
    begin
      FTotalRecords := 0;
      for i := 0 to FGatewayList.Count -1 do
      if (FGatewayList[i].DBRecordType in [rtDevice, rtComposite]) then
      begin
        Header := FTableList.FindByType(FGatewayList[i].RawTableType);
        if Header <> nil then
          inc(FTotalRecords, Header.RecordCount);
      end;
    end; }

  procedure UpdateUID;
  begin
    FDevice.GetDatabase.GetPanelUID(FDevice.FUID);
  end;

  procedure SetParamValue(const ParamName: string; const ParamValue: Variant);
  var
    Param: IParam;
  begin
    Param := FDevice.DeviceParams.FindParam(ParamName);
    if Param <> nil then
      Param.SetValue(ParamValue);
  end;

 var
  Gateway: PDBTableGateway;
  FLastRecord: Boolean;
  devicemd5, md5: TMD5Digest;
  ver: byte;
begin
  SetState(btsRunning);
  try
    if FDevice.FConnectionState <> csConnected then
    begin
      if FDevice.FConnectionState <> csIntermediate then
      begin
        if IsRubezh3Debug then
          SetFailureType(FDevice, FDevice.FLastRealExceptMessage) else
          SetFailureType(FDevice, rsPanelNoAnswer);
      end;
      SetState(btsFailed);
      FTotalRecords := 0;
      FReadedRecords := 0;
      FValidated := False;
    end else
    begin
      {$IFNDEF TestCalcHashAlways}
      if not FValidated then
      {$ENDIF}
      begin
        {$IFDEF HashDatabase}
        If FDevice.FPanelDeviceMod.DeviceSubtype in [dsIndicator{$IFDEF RubezhRemoteControl}, dsRemoteControl, dsRemoteControlFire {$ENDIF}] then
          ver := 0 else
          ver := FDevice.GetDatabase.GetDatabaseHashVersion;
        MD5 := FDevice.CalculateConfigMD5Cached(FDevice.Device, ver);
        SetParamValue('DBHash', BufferToHexString(MD5, Sizeof(MD5)));
        {$IFDEF DEBUG}WriteToLog(rkMessage, rtHash, 'Rubezh3Driver(' + IntToStr(FDevice.FInterface.Address)+
          ').Calculated DBHash (version ' + IntToStr(ver) + ': ' + BufferToHexString(MD5, Sizeof(MD5)));{$ENDIF}

        if FDevice.GetDatabase.HasDatabase then
        begin
          devicemd5 := FDevice.GetDatabase.GetDatabaseHash;
          {$IFDEF DEBUG}WriteToLog(rkMessage, rtHash, 'Rubezh3Driver(' + IntToStr(FDevice.FInterface.Address)+ ') Readed DBHash: ' + BufferToHexString(deviceMD5, Sizeof(deviceMD5)));{$ENDIF}
        end else
          {$IFDEF DEBUG}WriteToLog(rkMessage, rtHash, 'Rubezh3Driver(' + IntToStr(FDevice.FInterface.Address)+ ') DBHash. No Database: ');{$ENDIF}


        if IsRubezh3Debug then
          SetParamValue('DBHashR', BufferToHexString(devicemd5, Sizeof(devicemd5)));

        if (not FDevice.GetDatabase.HasDatabase)  then
        begin
          if (FDevice.GetDatabaseState <> dbInvalid) then
            FDevice.DoDatabaseHashIsDifferent(rsPanelNoDatabase);
          FDevice.SetDatabaseState(dbInvalid);
        end else
        if not SameDigest(devicemd5, md5) then
        begin
          {$IFDEF DEBUG}WriteToLog(rkMessage, rtHash, 'Rubezh3Driver(' + IntToStr(FDevice.FInterface.Address)+ ') DBHash is different.');{$ENDIF}
          if (FDevice.GetDatabaseState <> dbInvalid) then
            FDevice.DoDatabaseHashIsDifferent(rsDatabaseHashDifferent);
          FDevice.SetDatabaseState(dbInvalid);
          FDevice.FDelayedSynchronizationFailed := False;
          FDevice.SetIntErrorState(dbValidated);
        end else
        {$ENDIF}
        begin
          if FDevice.GetDatabaseState <> dbValidated then
            FDevice.DoSynchronizationRestored else
          if FDevice.FDelayedSynchronizationFailed then
          begin
            {$IFDEF DEBUG}WriteToLog(rkMessage, rtAction, 'Rubezh3Driver(' + IntToStr(FDevice.FInterface.Address)+ ') Delayed flag is set. SyncFailed');{$ENDIF}
            FDevice.DoSynchronizationFailed;
          end else
            FDevice.SetIntErrorState(dbValidated);

          FDevice.FDelayedSynchronizationFailed := False;

          FDevice.SetDatabaseState(dbValidated);

        end;

        FDevice.UpdateAddressList(FDevice.Device {FDevice.ParentInstance.Device});

        FValidated := True;

        If FDevice.FPanelDeviceMod.DeviceSubtype in [dsIndicator{$IFDEF RubezhRemoteControl}, dsRemoteControl, dsRemoteControlFire {$ENDIF}] then
          FTotalRecords := 0 else
          FTotalRecords := FDevice.GetDatabase.GetTotalDeviceCount;
        FDeviceTable := Low(TRubezh3DeviceType);
        FCurTableRecords := FDevice.GetDatabase.GetDeviceCount(FDeviceTable, True);

        exit;
      end;

      FLastRecord := True;

      Gateway := GetDBTableGatewayList3.FindByType(integer(FDeviceTable));
      if (FDeviceTable <> dtExternal)
        and (Gateway <> nil)
        and (FDeviceTable <= high(FDeviceTable))
        and ((Gateway.DBRecordType in [rtDevice, rtComposite, rtOutputDevice])
          or (Gateway.DeviceClassID = sdcRealASPTV3)
          or (Gateway.DeviceClassID = sdcOutputV3) ) then
      begin
        inc(FRecordNo);
        if FRecordNo < FCurTableRecords then
        begin
          {$IFDEF DEBUG}WriteToLog(rkMessage, rtDB_Transfer, IntToStr(FDevice.Device.DeviceAddress)+
            '. Reading device state. Table: '+ Gateway.ShortName+
            ' Record:  ' + IntToStr(FRecordNo + 1) + ' from ' + IntToStr(FCurTableRecords));
          {$ENDIF}
          FDevice.ReadDeviceRecord(FRecordNo, FDeviceTable);
          inc(FReadedRecords);
          FLastRecord := FRecordNo = FCurTableRecords - 1;
        end;
      end;

      if FLastRecord then
      begin
        {$IFDEF DEBUG}WriteToLog(rkMessage, rtDB_Work, IntToStr(FDevice.Device.DeviceAddress)+ '. LastRecord'); {$ENDIF}
        FRecordNo := -1;
        inc(FDeviceTable);
        if (FDeviceTable > High(FDeviceTable)) then
        begin
          SetState(btsComplete);
          UpdateUID;
        end else
          FCurTableRecords := Fdevice.GetDatabase.GetDeviceCount(FDeviceTable, True);

        {$IFDEF DEBUG}WriteToLog(rkMessage, rtDB_Work, IntToStr(FDevice.Device.DeviceAddress)+ '. Reading device states complete'); {$ENDIF}
      end;

    end;
  except
// сбрасывать цикл опроса при ошибке
    SetState(btsFailed);
    raise;
  end;
end;

function TDeviceParamsTask.PercentComplete: Integer;
begin
  if FTotalRecords <> 0 then
    result := Round((FReadedRecords/FTotalRecords) * 100) else
    result := 0;
end;


function TDeviceParamsTask.StateInfo: TBGTaskStateInfo;
begin
  result := inherited StateInfo;
  with result do
  begin
    CurrentDeviceTable := FCurTableRecords;
    CurrentDeviceNum := Integer(FDeviceTable);
  end;
end;

{ TDevicePumpV3 }

constructor TDevicePumpV3.Create(const DeviceRegistry: IDeviceRegistry);
begin
  inherited Create(sdcDevicePumpV3, DeviceRegistry);
end;

procedure TDevicePumpV3.Initialize;
begin
  inherited;
  RegisterParentClass(sdcDeviceNS);
  RegisterClassProperty('Icon', 'Device_Effector');
  RegisterClassProperty('DeviceClassName', rsClass_Pumps);
  RegisterClassProperty('HideInTree', True);
end;

{ TCustomRubezh3Device }

constructor TCustomRubezh3Device.Create(const Parent: IDeviceInstance; const Config: IDevice; PanelDeviceMod: PPanelDeviceMod; ServerReq: IServerRequesting);
begin
  inherited Create(Parent, Config)
end;

function TCustomRubezh3Device.GetFixedDataTransport: IDataTransport;
begin
  result := FDataTransport;
end;

procedure TCustomRubezh3Device.SetDatatransport(const Datatransport: IDataTransport);
begin
  FDataTransport := Datatransport;
end;

procedure TCustomRubezh3Device.SetOnFInitTransportInstance(Value: TOnInitTransport);
begin
  FOnFInitTransport := Value;
end;

procedure TCustomRubezh3Device.SetOnInitTransportInstance(Value: TOnInitTransport);
begin
  FOnInitTransport := Value;
end;

{ TDeviceASPTV3 }

constructor TDeviceASPTV3.Create(const DeviceRegistry: IDeviceRegistry);
begin
  inherited Create(sdcASPTV3, DeviceRegistry);
end;

procedure TDeviceASPTV3.Initialize;
begin
  inherited;

  RegisterParentClass(sdcDevicePanelV3);
  RegisterParentClass(sdcDevice10AMPanel);
  RegisterParentClass(sdcDeviceUSBPanelV3);
  RegisterParentClass(sdcDevice4APanel);
  RegisterParentClass(sdcDeviceUSB4APanel);
  RegisterParentClass(sdcASPTV3);
  {$IFDEF Rubezh3BUNS}
  RegisterParentClass(sdcDeviceBunsV3);
  RegisterParentClass(sdcDeviceUSBBunsV3);
  {$ENDIF}
  {$IFDEF BUNSv2}
  RegisterParentClass(sdcDeviceBunsV3_2);
  RegisterParentClass(sdcDeviceUSBBunsV3_2);
  {$ENDIF}
{$IFNDEF DISABLE_SECURITY}
  RegisterParentClass(sdcDeviceSecPanel);
  RegisterParentClass(sdcDeviceUSBSecPanel);
{$ENDIF}
(*{$IFDEF IMITATOR}
???  RegisterParentClass(sdcDeviceImitator);
{$ENDIF}*)
  RegisterClassProperty('Icon', 'Device_Device');
  RegisterClassProperty('DeviceClassName', rsMPT_Name);
  RegisterClassProperty('HideInTree', True);

end;

{ TDeviceMDS }

constructor TDeviceMDS.Create(const DeviceRegistry: IDeviceRegistry);
begin
  inherited Create(sdcDeviceMDS, DeviceRegistry);
end;

procedure TDeviceMDS.Initialize;
begin
  inherited;
  RegisterParentClass(sdcDeviceBusV3);
  RegisterParentClass(sdcDeviceUSBChannel);
  RegisterClassProperty('Icon', 'Device_Panel');
  RegisterClassProperty('DeviceClassName', rsClass_MDS_Name);
  RegisterClassProperty('Hidden', False);
end;

{ TDevicePPUV3 }

constructor TDevicePPUV3.Create(const DeviceRegistry: IDeviceRegistry);
begin
  inherited Create(sdcPPUV3, DeviceRegistry);
end;

procedure TDevicePPUV3.Initialize;
begin
  inherited;

  RegisterParentClass(sdcDevicePanelV3);
  RegisterParentClass(sdcDevice10AMPanel);
//  RegisterParentClass(sdcDeviceUSBPanelV3);
//  RegisterParentClass(sdcDevice4APanel);
//  RegisterParentClass(sdcDeviceUSB4APanel);
  RegisterParentClass(sdcPPUV3);
  {$IFDEF Rubezh3BUNS}
  RegisterParentClass(sdcDeviceBunsV3);
//  RegisterParentClass(sdcDeviceUSBBunsV3);
  {$ENDIF}
  {$IFDEF BUNSv2}
//  RegisterParentClass(sdcDeviceBunsV3_2);
//  RegisterParentClass(sdcDeviceUSBBunsV3_2);
  {$ENDIF}
{$IFNDEF DISABLE_SECURITY}
//  RegisterParentClass(sdcDeviceSecPanel);
//  RegisterParentClass(sdcDeviceUSBSecPanel);
{$ENDIF}
(*{$IFDEF IMITATOR}
???  RegisterParentClass(sdcDeviceImitator);
{$ENDIF} *)
  RegisterClassProperty('Icon', 'Device_Device');
  RegisterClassProperty('DeviceClassName', rsPPU_Name);
  RegisterClassProperty('HideInTree', True);

end;

{ TDeviceLED }

constructor TDeviceLED.Create(const DeviceRegistry: IDeviceRegistry);
begin
  inherited Create(sdcDeviceLED, DeviceRegistry);
end;

procedure TDeviceLED.Initialize;
begin
  inherited;

  RegisterParentClass(sdcDeviceLEDGroup);
  RegisterClassProperty('Icon', 'Device_Device');
  RegisterClassProperty('DeviceClassName', rsClass_Effector);
  RegisterClassProperty('HideInTree', True);
end;

{ TLEDDriver }

constructor TLEDDriver.Create(const DeviceRegistry: IDeviceRegistry);
begin
  inherited Create(rsDeviceLED_Name, sdcDeviceLED, sddLED, DeviceRegistry);

  SetOptions([optNotValidateZoneAndChildren, optIgnoreInZoneState]);

{  SetMaxZoneCardinality(-1);
  SetMinZoneCardinality(1); }

  SetMaxZoneCardinality(0);
  SetMinZoneCardinality(0);

  SetAutoCreateRange(True, 1, 50);

  SetDeviceCategory(dcOther);

  SetCaseCount(0);

  with RegisterDriverProperty('C4D7C1BE-02A3-4849-9717-7A3C01C23A24')^ do
  begin
    ValueType := varString;
    EditType := 'pkText';
    Hidden := True;
  end;

end;

function TLEDDriver.CreateDeviceInstance(const Parent: IDeviceInstance; const Config: IDevice; ServerReq: IServerRequesting): IDeviceInstance;
begin
  result := TCustomDeviceInstance.Create(Parent, Config);
end;

{ TDeviceIndicator }

constructor TDeviceIndicator.Create(const DeviceRegistry: IDeviceRegistry);
begin
  inherited Create(sdcDeviceIndicator, DeviceRegistry);
end;

procedure TDeviceIndicator.Initialize;
begin
  inherited;

  RegisterParentClass(sdcDeviceBusV3);
  RegisterParentClass(sdcDeviceUSBChannel);
  RegisterClassProperty('Icon', 'Device_Panel');
  RegisterClassProperty('DeviceClassName', rsClass_Indicator);
  RegisterClassProperty('Hidden', False);
end;

{ TLEDRecordsSort }

function TLEDRecordsSort.Compare(Index1, Index2: Integer): Integer;
begin
  result := FLedRecords^[Index1].DevAdr - FLedRecords^[Index2].DevAdr;
  if result = 0 then
    result := FLedRecords^[Index1].ZoneNum - FLedRecords^[Index2].ZoneNum;
end;

constructor TLEDRecordsSort.Create(LEDRecords: PLEDRecords);
begin
  inherited Create;
  FLEDRecords := LedRecords;
end;

function TLEDRecordsSort.High: Integer;
begin
  result := System.High(FLedRecords^);
end;

function TLEDRecordsSort.Low: Integer;
begin
  result := 0;
end;

procedure TLEDRecordsSort.Swap(Index1, Index2: Integer);
var
  rec: TLEDRecord;
begin

  rec := FLedRecords^[Index1];
  FLedRecords^[Index1] := FLedRecords^[Index2];
  FLedRecords^[Index2] := rec;

end;

{$IFDEF RubezhRemoteControl}
{ TRCRecordsSort }

function TRCRecordsSort.Compare(Index1, Index2: Integer): Integer;
begin
  result := FRCRecords^[Index1].full_addr - FRCRecords^[Index2].full_addr;
  if result = 0 then
    result := FRCRecords^[Index1].GroupNum - FRCRecords^[Index2].GroupNum;
end;

constructor TRCRecordsSort.Create(RCRecords: PRCRecords);
begin
  inherited Create;
  FRCRecords := RCRecords;
end;

function TRCRecordsSort.High: Integer;
begin
  result := System.High(FRCRecords^);
end;

function TRCRecordsSort.Low: Integer;
begin
  result := 0;
end;

procedure TRCRecordsSort.Swap(Index1, Index2: Integer);
var
  rec: TRCRecord;
begin
  rec := FRCRecords^[Index1];
  FRCRecords^[Index1] := FRCRecords^[Index2];
  FRCRecords^[Index2] := rec;
end;
{$ENDIF}

{ TDeviceLEDGroup }

constructor TDeviceLEDGroup.Create(const DeviceRegistry: IDeviceRegistry);
begin
  inherited Create(sdcDeviceLEDGroup, DeviceRegistry);
end;

procedure TDeviceLEDGroup.Initialize;
begin
  inherited;

  RegisterParentClass(sdcDeviceIndicator);
  RegisterClassProperty('Icon', 'Device_Device');
  RegisterClassProperty('DeviceClassName', rsClass_Effector);
  RegisterClassProperty('HideInTree', True);
end;

{ TLEDGroupDriver }

constructor TLEDGroupDriver.Create(const DeviceRegistry: IDeviceRegistry);
begin
  inherited Create(rsDeviceLEDGroup_Name, sdcDeviceLEDGroup, sddLEDGroup, DeviceRegistry);

  SetOptions([optNotValidateZoneAndChildren, optIgnoreInZoneState]);

{  SetMaxZoneCardinality(-1);
  SetMinZoneCardinality(1); }

  SetMaxZoneCardinality(0);
  SetMinZoneCardinality(0);

  SetDeviceCategory(dcOther);

  SetAutoCreateRange(True, 1, 5);

  SetCaseCount(0);
end;

function TLEDGroupDriver.CreateDeviceInstance(const Parent: IDeviceInstance; const Config: IDevice; ServerReq: IServerRequesting): IDeviceInstance;
begin
  result := TCustomDeviceInstance.Create(Parent, Config);
end;

{$IFDEF RubezhRemoteControl}
{ TDeviceRemoteControl }

constructor TDeviceRemoteControl.Create(const DeviceRegistry: IDeviceRegistry);
begin
  inherited Create(sdcDeviceRemoteControl, DeviceRegistry);
end;

procedure TDeviceRemoteControl.Initialize;
begin
  inherited;

  RegisterParentClass(sdcDeviceBusV3);
  RegisterParentClass(sdcDeviceUSBChannel);
  RegisterClassProperty('Icon', 'Device_Panel');
  RegisterClassProperty('DeviceClassName', rsClass_Indicator);
  RegisterClassProperty('Hidden', False);
end;

{ TDeviceRemoteControlFire }

constructor TDeviceRemoteControlFire.Create(const DeviceRegistry: IDeviceRegistry);
begin
  inherited Create(sdcDeviceRemoteControlFire, DeviceRegistry);
end;

procedure TDeviceRemoteControlFire.Initialize;
begin
  inherited;

  RegisterParentClass(sdcDeviceBusV3);
  RegisterParentClass(sdcDeviceUSBChannel);
  RegisterClassProperty('Icon', 'Device_Panel');
  RegisterClassProperty('DeviceClassName', rsClass_Indicator);
  RegisterClassProperty('Hidden', False);
end;

{ TDeviceRCGroup }

constructor TDeviceRCGroup.Create(const DeviceRegistry: IDeviceRegistry);
begin
  inherited Create(sdcDeviceRCGroup, DeviceRegistry);
end;

procedure TDeviceRCGroup.Initialize;
begin
  inherited;

  RegisterParentClass(sdcDeviceRemoteControl);
  RegisterClassProperty('Icon', 'Device_Device');
  RegisterClassProperty('DeviceClassName', rsClass_Effector);
  RegisterClassProperty('HideInTree', True);
end;

constructor TRCGroupDriver.Create(const DeviceRegistry: IDeviceRegistry);
begin
  inherited Create(rsDeviceRCGroup_Name, sdcDeviceRCGroup, sddRCGroup, DeviceRegistry);

  SetOptions([optNotValidateZoneAndChildren, optIgnoreInZoneState]);
  SetMaxZoneCardinality(0);
  SetMinZoneCardinality(0);
  SetDeviceCategory(dcOther);
  SetAutoCreateRange(True, 1, 10);
  SetCaseCount(0);

  with RegisterDriverProperty('E98669E4-F602-4E15-8A64-DF9B6203AFC5')^ do
  begin
    ValueType := varString;
    EditType := 'pkText';
    Hidden := True;
  end;

end;

function TRCGroupDriver.CreateDeviceInstance(const Parent: IDeviceInstance; const Config: IDevice; ServerReq: IServerRequesting): IDeviceInstance;
begin
  result := TCustomDeviceInstance.Create(Parent, Config);
end;


{ TDeviceRCFireGroup }

procedure TDeviceRCFireGroup.Initialize;
begin
  inherited;

  RegisterParentClass(sdcDeviceRemoteControlFire);
  RegisterClassProperty('Icon', 'Device_Device');
  RegisterClassProperty('DeviceClassName', rsClass_Effector);
  RegisterClassProperty('HideInTree', True);
end;

constructor TDeviceRCFireGroup.Create(const DeviceRegistry: IDeviceRegistry);
begin
  inherited Create(sdcDeviceRCFireGroup, DeviceRegistry);
end;

{ TRCFireGroupDriver }

function TRCFireGroupDriver.CreateDeviceInstance(const Parent: IDeviceInstance;
  const Config: IDevice; ServerReq: IServerRequesting): IDeviceInstance;
begin
  result := TCustomDeviceInstance.Create(Parent, Config);
end;

constructor TRCFireGroupDriver.Create(const DeviceRegistry: IDeviceRegistry);
begin
  inherited Create(rsDeviceRCGroup_Name, sdcDeviceRCFireGroup, sddRCFireGroup, DeviceRegistry);

  SetOptions([optNotValidateZoneAndChildren, optIgnoreInZoneState]);
  SetMaxZoneCardinality(0);
  SetMinZoneCardinality(0);
  SetDeviceCategory(dcOther);
  SetAutoCreateRange(True, 1, 5);
  SetCaseCount(0);

  with RegisterDriverProperty('E98669E4-F602-4E15-8A64-DF9B6203AFC5')^ do
  begin
    ValueType := varString;
    EditType := 'pkText';
    Hidden := True;
  end;

end;
{$ENDIF}

{ TDeviceAsptClassV3 }

constructor TDeviceAsptClassV3.Create(const DeviceRegistry: IDeviceRegistry);
begin
  inherited Create(sdcRealASPTV3, DeviceRegistry);
end;

procedure TDeviceAsptClassV3.Initialize;
begin
  inherited;
//  RegisterParentClass(sdcDevicePanelV3);
  RegisterParentClass(sdcDevice10AMPanel);

  RegisterClassProperty('Icon', 'Device_Effector');
  RegisterClassProperty('DeviceClassName', rsClass_Effector);
  RegisterClassProperty('HideInTree', True);
end;

{ TDeviceOutputClassV3 }

constructor TDeviceOutputClassV3.Create(const DeviceRegistry: IDeviceRegistry);
begin
  inherited Create(sdcOutputV3, DeviceRegistry);
end;

procedure TDeviceOutputClassV3.Initialize;
begin
  inherited;
{$IFNDEF DISABLE_SECURITY}
  RegisterParentClass(sdcDeviceSecPanel);
  RegisterParentClass(sdcDeviceUSBSecPanel);
{$ENDIF}
{$IFDEF BUNSv2}
  RegisterParentClass(sdcDeviceBunsV3_2);
  RegisterParentClass(sdcDeviceUSBBunsV3_2);
{$ENDIF}
  RegisterParentClass(sdcDevice4APanel);
  RegisterParentClass(sdcDeviceUSB4APanel);

  RegisterClassProperty('Icon', 'Device_Effector');
  RegisterClassProperty('DeviceClassName', rsClass_Effector);
  RegisterClassProperty('HideInTree', True);
end;

{ TCustomSimpleDevice }

function TCustomSimpleDevice.IOCTL_ExecuteFunction(const FunctionCode: string; out Reason: string): boolean;
var
  ChildIOCTLFunctionExecute: IChildIOCTLFunctionExecute;
begin
  result := false;

  if Supports(ParentInstance, IChildIOCTLFunctionExecute, ChildIOCTLFunctionExecute ) then
    result := ChildIOCTLFunctionExecute.ChildIOCTL_ExecuteFunction(Device, FunctionCode, Reason);

end;

{ TMRK30DeviceClass }

constructor TMRK30DeviceClass.Create(const DeviceRegistry: IDeviceRegistry);
begin
  inherited CreateCustom(sdcMRK30, DeviceRegistry);
end;

procedure TMRK30DeviceClass.Initialize;
begin
  inherited;
  GetClassProperties.FindParam('DeviceClassName').SetValue(rsClass_RadioDevice);
end;

{ TMRK30ChildDeviceClass }

constructor TMRK30ChildDeviceClass.Create(const DeviceRegistry: IDeviceRegistry);
begin
  inherited CreateCustom(sdcMRK30Child, DeviceRegistry);
end;

procedure TMRK30ChildDeviceClass.Initialize;
begin
  RegisterParentClass(sdcMRK30);
  RegisterClassProperty('Icon', 'Device_Device');
  RegisterClassProperty('DeviceClassName', rsClass_RadioDevice);
  RegisterClassProperty('HideInTree', True);
end;

{$IFDEF BUNSv2}
{ TDeviceBunsV3_2 }

constructor TDeviceBunsV3_2.Create(const DeviceRegistry: IDeviceRegistry);
begin
  inherited Create(sdcDeviceBunsV3_2, DeviceRegistry);
end;

procedure TDeviceBunsV3_2.Initialize;
begin
  inherited;
//  RegisterParentClass(sdcDeviceBus);
  if not Is485Protect then
  begin
    RegisterParentClass(sdcDeviceBusV3);
    RegisterParentClass(sdcDeviceUSBChannel);
  end;
  RegisterClassProperty('Icon', 'БУНС-01-1');
  RegisterClassProperty('DeviceClassName', rsClass_BUNS);
  RegisterClassProperty('Hidden', False);
end;
{$ENDIF}

{ TGroupAMDeviceClass }

constructor TGroupAMDeviceClass.Create(const DeviceRegistry: IDeviceRegistry);
begin
  inherited CreateCustom(sdcGroupAM, DeviceRegistry);
end;

procedure TGroupAMDeviceClass.Initialize;
begin
  inherited;
  GetClassProperties.FindParam('DeviceClassName').SetValue(rsClass_AMDevice);
end;

{ TGroupRMDeviceClass }

constructor TGroupRMDeviceClass.Create(const DeviceRegistry: IDeviceRegistry);
begin
  inherited CreateCustom(sdcGroupRM, DeviceRegistry);
end;

procedure TGroupRMDeviceClass.Initialize;
begin
  inherited;
  GetClassProperties.FindParam('DeviceClassName').SetValue(rsClass_Effector);
end;

{ TGroupAMPDeviceClass }

constructor TGroupAMPDeviceClass.Create(const DeviceRegistry: IDeviceRegistry);
begin
  inherited CreateCustom(sdcGroupAMP, DeviceRegistry);
end;

procedure TGroupAMPDeviceClass.Initialize;
begin
  inherited;
  GetClassProperties.FindParam('DeviceClassName').SetValue(rsClass_AMDevice);
end;

{ TGroupAMCPhildDeviceClass }

constructor TGroupAMPСhildDeviceClass.Create(
  const DeviceRegistry: IDeviceRegistry);
begin
  inherited CreateCustom(sdcGroupAMPChild, DeviceRegistry);
end;

procedure TGroupAMPСhildDeviceClass.Initialize;
begin
  RegisterParentClass(sdcGroupAMP);
  RegisterClassProperty('Icon', 'Device_Device');
  RegisterClassProperty('DeviceClassName', rsClass_AMDevice);
  RegisterClassProperty('HideInTree', True);
end;


{ TDeviceDetectorClassV3 }

constructor TDeviceDetectorClassV3.Create(
  const DeviceRegistry: IDeviceRegistry);
begin
  inherited CreateCustom(sdcDeviceDetectorV3, DeviceRegistry);
end;

procedure TDeviceDetectorClassV3.Initialize;
begin
  inherited;
  GetClassProperties.FindParam('DeviceClassName').SetValue(rsClass_Detector);
end;

{ TDeviceAMClass }

constructor TDeviceAMClass.Create(const DeviceRegistry: IDeviceRegistry);
begin
  inherited CreateCustom(sdcAMClass, DeviceRegistry);
end;

procedure TDeviceAMClass.Initialize;
begin
  inherited;
  GetClassProperties.FindParam('DeviceClassName').SetValue(rsClass_AMDevice);
end;

{ TDevice10AM }

constructor TDevice10AM.Create(const DeviceRegistry: IDeviceRegistry);
begin
  inherited Create(sdcDevice10AMPanel, DeviceRegistry);
end;

procedure TDevice10AM.Initialize;
begin
  inherited;
//  RegisterParentClass(sdcDeviceBus);
  if Is485Protect then
  begin
    RegisterParentClass(sdcDeviceBusV3);
    RegisterParentClass(sdcDeviceUSBChannel);
  end;
  RegisterClassProperty('Icon', 'Рубеж-2АМ');
  RegisterClassProperty('DeviceClassName', rsClass_Panel);
  RegisterClassProperty('Hidden', False);

end;

{ TDeviceMROrev2 }

constructor TDeviceMROrev2.Create(const DeviceRegistry: IDeviceRegistry);
begin
  inherited Create(sdcMROrev2, DeviceRegistry);
end;

procedure TDeviceMROrev2.Initialize;
begin
  inherited;

//  RegisterParentClass(sdcDevicePanelV3);
//  RegisterParentClass(sdcDeviceUSBPanelV3);
  RegisterParentClass(sdcDevice10AMPanel);
  RegisterParentClass(sdcDevice4APanel);
  RegisterParentClass(sdcDeviceUSB4APanel);
  RegisterParentClass(sdcMROrev2);
//  {$IFDEF Rubezh3BUNS}
//  RegisterParentClass(sdcDeviceBunsV3);
//  RegisterParentClass(sdcDeviceUSBBunsV3);
//  {$ENDIF}
  {$IFDEF BUNSv2}
  RegisterParentClass(sdcDeviceBunsV3_2);
  RegisterParentClass(sdcDeviceUSBBunsV3_2);
  {$ENDIF}
  {$IFNDEF DISABLE_SECURITY}
  RegisterParentClass(sdcDeviceSecPanel);
  RegisterParentClass(sdcDeviceUSBSecPanel);
  {$ENDIF}
  RegisterClassProperty('Icon', 'Device_Device');
  RegisterClassProperty('DeviceClassName', rsClass_Effector);
  RegisterClassProperty('HideInTree', True);
end;

{ TDeviceBolt }

constructor TDeviceBolt.Create(const DeviceRegistry: IDeviceRegistry);
begin
  inherited Create(sdcDeviceBolt, DeviceRegistry);
end;

procedure TDeviceBolt.Initialize;
begin
  inherited;
  RegisterParentClass(sdcDeviceBunsV3);
  RegisterParentClass(sdcDeviceUSBBunsV3);
  RegisterParentClass(sdcDevicePanelV3);
  RegisterParentClass(sdcDeviceUSBPanelV3);
{$IFDEF BUNSv2}
  RegisterParentClass(sdcDeviceBunsV3_2);
  RegisterParentClass(sdcDeviceUSBBunsV3_2);
{$ENDIF}

  RegisterClassProperty('Icon', 'Device_Effector');
  RegisterClassProperty('DeviceClassName', rsClass_Effector);
  RegisterClassProperty('HideInTree', False);
end;

{ TDeviceFan }

constructor TDeviceFan.Create(const DeviceRegistry: IDeviceRegistry);
begin
  inherited Create(sdcFan, DeviceRegistry);
end;

procedure TDeviceFan.Initialize;
begin
  inherited;
//  RegisterParentClass(sdcDevicePanelV3);
//  RegisterParentClass(sdcDeviceUSBPanelV3);
  RegisterParentClass(sdcDevice10AMPanel);
  RegisterParentClass(sdcDevice4APanel);
  RegisterParentClass(sdcDeviceUSB4APanel);
//  {$IFDEF Rubezh3BUNS}
//  RegisterParentClass(sdcDeviceBunsV3);
//  RegisterParentClass(sdcDeviceUSBBunsV3);
//  {$ENDIF}
  {$IFDEF BUNSv2}
  RegisterParentClass(sdcDeviceBunsV3_2);
  RegisterParentClass(sdcDeviceUSBBunsV3_2);
  {$ENDIF}
  {$IFNDEF DISABLE_SECURITY}
  RegisterParentClass(sdcDeviceSecPanel);
  RegisterParentClass(sdcDeviceUSBSecPanel);
  {$ENDIF}
  RegisterClassProperty('Icon', 'Device_Device');
  RegisterClassProperty('DeviceClassName', rsClass_Effector);
  RegisterClassProperty('HideInTree', True);
end;

{ TDeviceFanChild }

constructor TDeviceFanChild.Create(const DeviceRegistry: IDeviceRegistry);
begin
  inherited Create(sdcFanChild, DeviceRegistry);
end;

procedure TDeviceFanChild.Initialize;
begin
  inherited;
  RegisterParentClass(sdcFan);
  RegisterClassProperty('Icon', 'Device_Device');
  RegisterClassProperty('DeviceClassName', rsClass_Effector);
  RegisterClassProperty('HideInTree', True);
end;

end.



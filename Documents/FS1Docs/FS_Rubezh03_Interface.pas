{
  Интерфейс для работы со второй версией панели. Основные изменения:
}

unit FS_Rubezh03_Interface;
interface

{$I ..\Options.inc}

uses windows, sysutils, classes, dateutils, fs_intf, fs_server_intf,
  FS_SupportClasses;

const
  MAX_LINE_COUNT = 16; // макс. теоретическое число шлейфов

type

  TIsR3Debug = (dUnknown, dYes, dNo);

  TCommandState = (csNone, csInitializing, csNormalMode, csConfirmMode);

  TSendBlockResult = (
    crAnswer,      { Панель вернула ответ - ошибок нет }
    crTimeout,     { Панель не ответила за отведенный таймаут. Будем пробовать еще. }
    crError,       { Панель вернула код ошибки. }
    crBadCRC,      { Ошибка CRC. Будем пробовать еще. }
    crExtTimeout   { Плановый длинный таймаут }
  );

  TDeviceSignature = array[0..3] of byte;

  TRubezhSubtype = (dsOrdinaryPanel, dsMDS, dsIndicator, dsMDS5{$IFDEF RubezhRemoteControl}, dsRemoteControl, dsRemoteControlFire {$ENDIF} {$IFDEF IMITATOR}, dsImitator {$ENDIF});
  TRubezh3DatabaseType = (dbtNone, dbtClassic, dbtModern);

  PPanelDeviceMod = ^TPanelDeviceMod;
  TPanelDeviceMod = record
    ClassID: string;
    DriverID: string;
    BaseNodeType: Integer;
    ServiceNodeType: Integer;
    DeviceName: string;
    DeviceShortName: string;
    DeviceShortNameAlias: string;
    HasOutDevices: Boolean;
    ChildAddressMask: string;
    AutoDetectable: Boolean;
    Signature: TDeviceSignature;
    USBDescription: string;
    DeviceSubtype: TRubezhSubtype;
    AVRCount: integer;                         // 0 = 1 в данном случае (пока)
    DatabaseType: TRubezh3DatabaseType;        // определяет, используемую модель базы
    IconName: string;
    MaxDevices: integer;                       // Макс. кол-во устройств в панели. 0 - неограниченно
    SubSystem: TStateSubSystem;                // Подсистема: ssFire, ssSecurity, ssTech
  end;

  PRawEventRecord16 = ^TRawEventRecord16;
  TRawEventRecord16 = packed record
    EventCode: byte;
    EventTime: dword;
    Context: array[0..9] of byte;
//    LineNo: byte;
//    CRC8: byte;
  end;

  TRawEventRecordExtension = array[15..31] of byte;

  PRawEventRecord32 = ^TRawEventRecord32;
  TRawEventRecord32 = packed record
    Record16: TRawEventRecord16;
    Ext: TRawEventRecordExtension;
  end;

  TZoneNo = Word;

  PRawAddressList = ^TRawAddressList;
  TRawAddressList = packed record
    adresses: array[0..31] of byte;
    types: array[0..31] of byte;
  end;

  TRawMSChannelInfo = packed record
    SelfAddress: byte;
    Speed: byte;
    adresses: array[0..31] of byte;
  end;

  TRawMSChannelInfos = array of TRawMSChannelInfo;

  TRawMSAddressList = array of array[0..31] of byte;

  PSingleAddressList = ^TSingleAddressList;
  TSingleAddressList = array[0..31] of byte;

  PRawEventExtZoneInfo = ^TRawEventExtZoneInfo;
  TRawEventExtZoneInfo = packed record
    res: byte;
    ZoneType: byte;
    ZoneAttr: byte;
    ZoneOrDivNo: TZoneNo;
    AddrPtr: array[0..2] of byte;
  end;

  PRawEventDeviceInfo = ^TRawEventDeviceInfo;
  TRawEventDeviceInfo = packed record
    SystemAddress: byte;
    DeviceType: byte;
    LocalAddress: byte;
    DevState: byte;

    ZoneNoAndPsw: TZoneNo;
    AddrPtr: array[0..2] of byte;
  end;

  TAVRInfo = array of boolean;

  TDatarec = packed record
    address: byte;
    speed: byte;
    RawAddressList: TSingleAddressList;
  end;

  TLineStates = array[0..(MAX_LINE_COUNT div 2)-1] of byte;

  TRubezh3Interface = class;

  TDeviceAddressType = (atSystem, atLocal, atLineNo, atComplete);

  // Интерфейс для работы с моделью базы. Определяет все характеристики
  // БД и реализует инструменты для работы с примитивами
  IRubezh3DatabaseModel = interface
    ['{03560CC3-74C3-4236-8017-7FB8F409B7F9}']
    // Получает чистое значение шлейфа из байтового значения
    function GetLineNo(RawValue: byte): byte;
    // Получает адрес из локального, системного и нового номера шлейфа
    function GetAddress(AddressType: TDeviceAddressType; LocalAddress, SystemAddress: byte; const ExtData: TRawEventRecordExtension): Integer;
    // Получает чистое значение номера зоны из хранимого в базе
    function GetZoneNo(RawValue: word): word;
    // Запись адреса устройства в данные комманды прибора
    function WriteNativeDeviceAddress(const Device: IDevice; const Datablock: IDataBlock): integer;
    // Определить драйвер по сигнатуре БД
    function AcquireDriverIDByDatabaseSignature(AnInterface: TRubezh3Interface): string;
    // Используется ли 32 байтный журнал
    function IsEventLog32: boolean;
    // Получает чистое значение пароля из значения с учетом 32 биного журнала
    function GetPassword(RawValue: byte; const ExtData: TRawEventRecordExtension): byte;
  end;

  TDeviceLevelBoltCmd = (bcOpen, bcClose, bcStop);
  TDoubleAddressOnChannel = array [0..1] of boolean;

  TAnswerOnWriteParam = packed record
    cmd: byte;
    OldType: byte;
    Address: byte;
    Panel: byte;
    Param: byte;
    Value: word;
    Als: byte;
  end;

  TRubezh3Interface = class
  private
    FDataTransport: IDataTransport;
    FModel: IRubezh3DatabaseModel;
    FPanelDeviceMod: PPanelDeviceMod;
    FAddress: Byte;
    FActualAddress: Byte;
    FLastErrorCode: Integer;
    FRetryCount: Integer;
    FNonAnswerCount: Integer;

    FBaseCommandState: TCommandState;
    FServiceCommandState: TCommandState;
    FConfirmedBlock: IDataBlock;
    FCanceled: Boolean;
    FServiceMode: Boolean;
    FOnTransportOperation: TNotifyEvent;

    FBytesReaded: Integer;
    FBytesWritten: Integer;
    FLastSendBlockTime: TDateTime;
    FFastLoseDetection: Boolean;

    FTimeSlicer: ITimeSlicer;

    FConstantsAreValid: Boolean;
    FDeviceEventLogSize: Integer;
    FDeviceDatabaseStart: Integer;

    FProtocolPausedBefore: TDatetime;
    FBytesComplete: Integer;
    FBytesTotal: Integer;

    FBusyStart: TDatetime;

    FSoftUpdateMode: Boolean;
    FExceptionOnRepeat: Boolean;
    FReserveLine1Fail: TChannelState;
    FReserveLine2Fail: TChannelState;
    FDoubleAddressOnChannel: TDoubleAddressOnChannel;
    FLastCheckDoubling: TDateTime;

    FIsThirdVersion: boolean;


    FLastSector: TMemoryStream;
    FLastSectorNo: integer;
    FLastSectorAddr: longword;

    function EnterConfirmMode(const DataBlock: IDataBlock): Boolean;
    procedure LeaveConfirmMode;
    function GetCommandState: TCommandState;
    procedure SetCommandState(State: TCommandState);
    function IsWriteCommand(code: byte): Boolean;
    function IsReadCommand(code: byte): Boolean;
    function IsMemCommand(code: byte): Boolean;
    procedure RepeatLastSector;
  protected
    // Выполнить команду. Возвращается код результата. Если это crError, то ошибка - FLastErrorCode
    function InternalSend(const DataBlock: IDataBlock; out ResultDataBlock: IDataBlock): TSendBlockResult;

    // Выполнить команду с повторами передачи. Если возвращается False - это ошибка с кодом FLastErrorCode.
    function SendDataBlock(const DataBlock: IDataBlock; out ResultDataBlock: IDataBlock): Boolean;

    // Выполнить команду с повторами передачи. При ошибки поднимает Exception
    function CheckSendDataBlock(const DataBlock: IDataBlock; Approval: Boolean = True): IDataBlock;

    function CreateCommand(Fun: Byte; Context: Pointer; ContextSize: Integer; AddCRC: Boolean = True): IDataBlock;
    function CreateParamQuery(Param: Byte; Context: Pointer; ContextSize: Integer; WriteParam: Boolean; AddCRC: Boolean = True): IDataBlock;
    procedure AddModBusCRCToDataBlock(const DataBlock: IDataBlock);

    procedure InitDeviceConstants;

  public
    constructor Create(Address: Byte; const DataTransport: IDataTransport; PanelDeviceMod: PPanelDeviceMod);
    destructor Destroy; override;

    procedure SetTimeSlicer(const TimeSlicer: ITimeSlicer);

    { Общие команды панели }
    procedure RequestApproval;

    function GetLastErrorCode: Integer;

    function SendCustomCommand(Code: Integer; Context: pointer; ContextSize: Cardinal; out ResultDataBlock: IDataBlock): TSendBlockResult;

    function SendPing(out ResultDataBlock: IDataBlock): TSendBlockResult;
    procedure doTimeSlice;

    procedure EventLogReadFullRecord16(LogType: byte; RecordIndex: dword; var EventRec: TRawEventRecord16);
    procedure EventLogReadFullRecord32(LogType: byte; RecordIndex: dword; var EventRec: TRawEventRecord32);
    function EventLogLastRecord(LogType: byte): dword;
    function EventLogBufferSizeInt(LogType: byte): word;
    function EventLogBufferSize(LogType: byte): word;

    function GetDatabaseVersion: word;
    function GetDatabaseStart: Integer;
    function GetDatabaseStart2: Integer;
    function GetDatabaseStartInt: Integer;
    function GetLastOperationStatus: byte;

    procedure IgnoreListOp(Device: IDevice; Append: Boolean);
    function ReadSmokiness(Device: IDevice): byte; overload;
    function ReadSmokiness(DeviceAddress: integer): byte; overload;
    procedure ReadLineStates(out Data: TLineStates);
    procedure RawDateTimeSet(value: dword);
    procedure DateTimeSet(DateTime: TDateTime);
    function RawDateTimeGet: dword;
    function DateTimeGet: TDateTime;
    function GetFirmwareVersion: word;
    procedure SetFirmwareVersion(version: word);
    function GetNonUsedDevices: word;
    procedure SetAutomaticState(Device: IDevice; AutoOn: Boolean);

    function GetAddressList: TRawAddressList;
    procedure SetAddressList(AddressList: TRawAddressList; Full: boolean = False);

    function GetSerialNo: string;
    function VerifyPanelAdminPassWord(PassWord: String): boolean;

    procedure ResetCustomState(ZoneNo: TZoneNo; IsPartition: Boolean; Command: byte);
    procedure ResetZoneState(ZoneNo: TZoneNo; IsPartition: Boolean);
    procedure GuardSetZoneState(ZoneNo: TZoneNo; IsPartition: Boolean);
    procedure GuardUnSetZoneState(ZoneNo: TZoneNo; IsPartition: Boolean);
    procedure ResetClapanState;

    procedure BoltDeviceLevelCmd(Cmd: TDeviceLevelBoltCmd; const Device: IDevice);
    procedure RunSingleDeviceCommand(PropInfo: PPropertyTypeInfo; const Device: IDevice);
    // чтение-запись произвольных параметров с устройств по АЛС
    function ReadSimpleParamFromDevice(const Device: IDevice; ParamNo: integer): double;
    function WriteSimpleParamFromDevice(const Device: IDevice; ParamNo, ParamValue: integer): double;

    function WritePropToDevice(PropInfo: PPropertyTypeInfo; const Device: IDevice; SplitByte: integer = -1): double;
    function ReadPropFromDevice(PropInfo: PPropertyTypeInfo; const Device: IDevice): double;

    function GetDeviceState: word;
    function GetDeviceType: byte;
    function GetFirmwareInfo(MemType: byte; out CRC: word): word;

    function GetMSChannelInfo(CountChannels: integer): TRawMSChannelInfos;

    function SameAVRVersion(NeededVersion, NeededCRC: word; var AVRInfo: TAVRInfo): boolean;
    function HasAVRVersion(NeededVersion: word): boolean;

    function GetPanelState(CountBytes: integer = 0; Reverse: boolean = true): int64;
    function GetPanelState64: int64;
    procedure SetPanelState(State: dword);
    procedure SetPanelState64(State: int64);

    procedure DatabaseSetLock(Lock: Boolean);
    procedure SetBlindMode(Active: Boolean);

    procedure BeginSoftUpdate(MemType: byte);
    procedure EndSoftUpdate_Reset;
{$IFDEF IMITATOR}
    procedure Im_SetControlToPO;
{$ENDIF}
    procedure GetUSBConfig(out DataRec: TDataRec);
    procedure SetUSBConfig(const DataRec: TDataRec);

    procedure MemClear(StartSector, EndSector: Integer);
    procedure MemWriteRaw(Addr, ASize: Dword; Buffer: PChar);

    procedure MemWriteSec(Addr, ASize: Dword; Buffer: PChar);
    procedure RawWriteRepeatable(Addr, Size: LongWord; var Buf);
    procedure RawPushBlock(Addr, Size: Longword; var Buf; OnlyOneSector: boolean);
    procedure RawClearBlock;
    class function GetMemBlockByAddress(Address: Integer): Integer;

    procedure TouchMemoryOperation(subfunc: byte);

    function GetExtMemError: Integer;

    procedure MemWrite(Addr, ASize: Dword; Buffer: PChar; SetByteProgress: Boolean = False);
    procedure MemWriteBlock(Addr, ASize: Dword; const Buffer);

    procedure MemWriteBlockToMDSDB(ASize: Dword; const Buffer);
    function MemReadBlockFromMDSDB(ASize: integer): String;

    procedure VerifyBlock(Addr, ASize: Dword; const Buffer);

    function MaxMemoryBlockSize: Integer;

    procedure MemRead(Addr, ASize: Dword; Buffer: PChar);
    procedure MemReadBlock(Addr, ASize: Dword; var Buffer);

    procedure ResetDevice;
    procedure ValidateDevices;
    procedure ResetByteCount;

    function AcquireDriverIDByDatabaseSignature: string;
    function WaitPanelUserMode: Boolean;

    property LastSendBlockTime: TDateTime read FLastSendBlockTime;
    property RetryCount: Integer read FRetryCount write FRetryCount;
    property Canceled: Boolean read FCanceled write FCanceled;
    property SoftUpdateMode: Boolean read FSoftUpdateMode write FSoftUpdateMode;
    property ExceptionOnRepeat: Boolean read FExceptionOnRepeat write FExceptionOnRepeat;
    property DataTransport: IDataTransport read FDataTransport;
    property Address: byte read FAddress;


    property LastSector: TMemoryStream read FLastSector;
    property LastSectorNo: integer read FLastSectorNo write FLastSectorNo;
    property LastSectorAddr: longword read FLastSectorAddr write FLastSectorAddr;

    property OnTransportOperation: TNotifyEvent read FOnTransportOperation
      write FOnTransportOperation;

    property BytesReaded: Integer read FBytesReaded;
    property BytesWritten: Integer read FBytesWritten;

    property BytesTotal: Integer read FBytesTotal write FBytesTotal;
    property BytesComplete: Integer read FBytesComplete write FBytesComplete;

    property FastLoseDetection: Boolean read FFastLoseDetection write FFastLoseDetection;

    property ReserveLine1Fail: TChannelState read FReserveLine1Fail;
    property ReserveLine2Fail: TChannelState read FReserveLine2Fail;

    property DoubleAddressOnChannel: TDoubleAddressOnChannel read FDoubleAddressOnChannel;
    property Model: IRubezh3DatabaseModel read FModel write FModel;
  end;

  ERubezh3Error = class(Exception)
  private
    FErrorCode: Integer;
  public
    constructor Create(const Msg: string; ErrorCode: Integer);
    property ErrorCode: Integer read FErrorCode write FErrorCode;
  end;

  ERubezh3UserError = class(ERubezh3Error)
  end;

  ERubezh3MemError = class(ERubezh3Error)
  end;

  TRawAdrressListSort = class(TQuickSort)
  private
    FRecords: PRawAddressList;
  protected
    function Low: Integer; override;
    function High: Integer; override;
    procedure Swap(Index1, Index2: Integer); override;
    function Compare(Index1, Index2: Integer): Integer; override;
  public
    constructor Create(Records: PRawAddressList);
  end;

  TSingleAdrressListSort = class(TQuickSort)
  private
    FRecords: PSingleAddressList;
  protected
    function Low: Integer; override;
    function High: Integer; override;
    procedure Swap(Index1, Index2: Integer); override;
    function Compare(Index1, Index2: Integer): Integer; override;
  public
    constructor Create(Records: PSingleAddressList);
  end;

const
  { Коды ошибок транспорта/панели }
  errInvalidPacketReceived = -1; { Пакет полученный от транспорта - не соответствует формату }
  errPanelNoAnswer         = -2; { Панель не ответила длительное время }
  errOperationCanceled     = -3; { Операция прервана оператором }
  errBadCRC                = -4; { Ошибка CRC }
  errUnexpectedAnswer      = -5; { Получен неожидаемый ответ }
  errLineError             = -6; { Ошибка связи }
  errConfirmed             = -7; { Длительная операция завершена }

  errInvalidFunction   = $01; { ERR: Функция не поддерживается данным прибороом или в текущем режим работы прибора }
  errInvalidAddress    = $02; { ERR: Адрес (номер параметра, подфункция) является недопустимым }
  errIllegalDataValue  = $03; { ERR: Значение в поле данных недопустимо }
  errFailure           = $04; { FAIL: Прибор не может ответить на запрос }
  errAcknowledge       = $05; { ACK: Долговременная операция }
  errDeviceBusy        = $06; { BUSY: Устройство занято }
  errNAK               = $07; { NAK: Функция замены ПО не может быть выполнена }
  errDenialOfService   = $08; { DOS: Отказ от обслуживания }
  errUSBReportNoDevice = $20; { USB: Устройство не отвечает }
  errUSBCannotDeliver  = $21; { USB: невозможно доставить запрос }
  errUSBPanelBusy      = $22; { USB: Прибор занят }

  errExtDataOutside    = $0100; { Данные выходят за границы доступной памяти }
  errExtPrepare_Failed = $0200; { Команда подготовки сектора завершилась неудачей }
  errExtEraseFailed    = $0300; { Команда стирания сектора завершилась неудачей }
  errRAMToFlashFailed  = $0400; { Команда копирования данных их RAM во Flash завершилась неудачей }
  errCompareFailed     = $0500; { Ошибка сравнения }
  errSanityCheckFailed = $0600; { Проверка на чистоту завершилась ошибкой }
  errCodeCorrupted     = $0700; { Загруженный код поврежден }
  errRSRDisconnect     = $0800; { Нет связи с контроллером RSR }
  errRSRUnknownType    = $0900; { Неизвестный тип контроллера RSR }
  errRSRConfigError    = $0A00; { Ошибка конфигурации контроллера RSR }

  cmdTouchMemInfo      = $33; { Операции с ключами TouchMemory }
  cmdGetFirmwareVer    = $37; { Информация о прошивке }
  cmdMemReadROM        = $38; { Чтение блока ROM }
  cmdBeginSoftUpdate   = $39; { Начать обновление прошивки bStorage }
  cmdEndSoftUpdate     = $3A; { Окончание записи памяти - сброс.  }
  cmdMemClear          = $3B; { Очистка сектора памяти bSectorStart, bSectorEnd }
  cmdRequestApproval   = $3C; { Подтверждение / завершение долговременной операции }
  cmdGetExtErr         = $3D; { Запрос информации об аппаратной ошибке }
  cmdMemWrite          = $3E; { Запись памяти dwAddr, bytes }

  cmdParamRequest      = $01; { Запрос параметра }
  cmdParamWrite        = $02; { Установка параметра }

  prDeviceState       = $01; { Статус ОС прибора }
  prChannelNo         = $02; { Номер канал RS прибора, для многоканальных }
  prDeviceType        = $03; { Тип устройства 0xFF - неподписан (всё можно писать), 1 - Рубеж-2АМ, 2 - БУНС, 3 - панель индикации и т.д.}
  prLineStates        = $04; { Состояния шлейфов. По 5 бит на шлейф }
  prDeviceList        = $05; { Список устройств. Формат такой же как у адресного листа }
  prPanelState        = $10; { Статус прибора/режим работы (статус БИ : 0x01 - нет связи с приборами, 0x02 - не совпадает база ) }
  prExtPanelState     = $0F; { Дополнительные статусы прибора }
  prDatetime          = $11; { Дата/время в приборе }
  prVersion           = $12; { Версия прошивки }
  prNonUsed           = $13; { Число лишних датчиков }
  prAddrList          = $14; { Адресный лист }
  prOperationStatus   = $1F; { Статус последней операции }
  prEventRecordRead   = $20; { Определенная запись в журнале }
  prEventLogCounter   = $21; { Глобальный счетчик записей }
  prPanelCurState     = $23; { Текущее состояние устройства }
  prEventLogSize      = $24; { Размер буфера журнала событий }
  prUSBConfig         = $31; { конфигурация USB устройства }
  prSerialNo          = $32; { серийный номер }
  prMDSMemory         = $34; { запись в MDS}
  prVerifyPass        = $35; { проверка пароля администратора}
  prBlindMode         = $42; { установить/снять режим "глухой панели" }

  prDatabaseVersion   = $50; { Та версия БД, которую понимает данный прибор }
  prDatabaseOffset    = $51; { Начальное смещение БД }
  prMemoryBlock       = $52; { Чтение/запись блока базы данных }
  prDeviceLevelCmd    = $53; { Команда к устройству. Прямое управление }
  prHighLevelCmd      = $54; { Команда к базе. Состояние зоны/раздела Добавление/Удаление уст-ва в список обхода}
  prDatabaseLock      = $55; { Установка блокировки БД }
  prSmokiness         = $56; { Задымленность }
  prDatabaseOffset2   = $57; { Начальное смещение БД во внутр. флэш}
  prDeviceMuteTermCmd = $58; { Терминальная команда к устройству. Прямое управление. Событие в прибор не пишется}

const
  OneDTSecond = 1 / (24 * 60 * 60);

  MIN_PANEL_ADDRESS = $01;
  MAX_PANEL_ADDRESS = 100 {127};
  MAX_EXT_PANEL_ADDRESS = 127 {127};

  LOG_FIRE_TYPE_16byte = 1;
  LOG_FIRE_TYPE_32byte = 0;

{$IFNDEF DISABLE_SECURITY}
  LOG_SEC_TYPE_16byte = 3;
  LOG_SEC_TYPE_32byte = 2;
{$ENDIF}
  HighLevelRetryCount = 10;

  DEFAULT_COMPUTER_ADDRESS = $FF;

  AUTO_DETECT_RETRY_COUNT = 2;
  DELAY_SEND_DATA_TO_LOST_DEVICE = 5; // задержка до следующего запроса к потерянному прибору, сек

  DATABASE_RESERVE_SIZE = 20;

  MIN_NOTIFY_TIME = OneDTSecond * 2;
  AUTO_RESET_TIMEOUT = OneDTSecond * 30;
  EVENT_REASON_TIME_BACK = OneDTSecond * 60 * 20; // 20 минут
  UID_CHANGE_CHECK_PERIOD = OneMinute * 2;
  DELAY_BEFORE_EXT_READ = OneDTSecond * 3;

  BUSY_WAIT = OneMinute * 1;

  DEFAULT_RETRY_COUNT = 3;
  ACK_RETRY_COUNT = 20;

  PANEL_MAX_SERIAL_SIZE = 11;
  PANEL_MAX_VERSION = 2;
  INDICATOR_DB_START_OFFSET = $4000;
{$IFDEF IMITATOR}  
  IMITATOR_DB_START_OFFSET =$40000100;
  IMITATOR_DB_END_OFFSET =$40003DDF; // последние 2 байта CRC
//  IMITATOR_DB_END_OFFSET =$400031DF; // последние 2 байта CRC
{$ENDIF}
  // Константы 2ОП
  GUARD_DATA_OFFSET = $14000;  // Начальный адрес охранной базы прибора
  GUARD_USERS_COUNT = 80;      // Кол-во пользователей
  GUARD_ZONES_COUNT = 64;      // Максимальное количество охранных зон в приборе
  GUARD_USER_DATA_SIZE = 46;   // Размер данных пользователя

  MAX_PROTOCOL_MEM_BLOCK_SIZE = 256;  // Максимальный размер блока памяти, который можно писать/читать одним запросом


var
  FSeenThird: boolean = false;

function RawDateTimeToDateTime(RawDateTime: dword): TDateTime;
function DateTimeToRawDateTime(Value: TDateTime): dword;
function IsSubProtect: boolean;
function ReverseBytes(Value: Cardinal): Cardinal;

procedure DataBlockGet(const Datablock: IDataBlock; Buffer: Pointer; BytePos, Count: Integer);
procedure DataBlockGetData(const Datablock: IDataBlock; Buffer: Pointer; Count: Integer);


implementation
uses FS_Strings, FS_ModBus_Common, FS_DevClasses, math, fs_hexPackage, FS_Rubezh03_Config {$IFDEF DEBUG}, LogFile{$ENDIF};

var
  FIsSubProtect: TIsR3Debug;

function ErrorCodeToString(ErrorCode: Integer): string;
begin
  case ErrorCode of
    errInvalidPacketReceived: result := rsInvalidPacketReceived;
    errPanelNoAnswer: result := rsPanelNoAnswer; {Не отвечает}
    errOperationCanceled: result := rsOperationCanceled;
    errBadCRC: result := rsBadCRC;
    errUnexpectedAnswer: result := rsUnexpectedAnswer;
    errLineError: result := rsLineError;
    errConfirmed: result := rsConfirmed;

    errInvalidFunction: result := rsErrorInvalidFunction;
    errInvalidAddress: result := rsErrorInvalidAddress;
    errIllegalDataValue: result := rsErrorIllegalDataValue;
    errFailure: result := rsErrorFailure;
    errAcknowledge: result := rsErrorAcknoweledge;
    errDeviceBusy: result := rsErrorDeviceBusy;
    errNAK: result := rsErrorNAK;
    errDenialOfService: result := rsErrorDOS;
    errUSBReportNoDevice: result := rsErrorUSBNoDevice;
    errUSBCannotDeliver: result := rsErrorUSBCannotDeliver;
    errUSBPanelBusy: result := rsErrorUSBPanelBusy;

    errExtDataOutside: result := rsErrorDataOutside;
    errExtPrepare_Failed: result := rsErrorPrepareFailed;
    errExtEraseFailed: result := rsErrorEraseFailed;
    errRAMToFlashFailed: result := rsRAMtoFlashFailed;
    errCompareFailed: result := rsErrorCompareFailed;
    errSanityCheckFailed: result := rsSanityCheckFaield;
    errCodeCorrupted: result := rsErrorCodeCorrupted;
    errRSRDisconnect: result := rsErrorRSRDiconnect;
    errRSRUnknownType: result := rsErrorRSRUnknownType;
    errRSRConfigError: result := rsErrorRSRConfigError;

    else result := Format(rsErrorUnknown, [ErrorCode]);
  end;
//  result := Format('%s. (Код: %d)', [result, ErrorCode]);
end;

// Преобразование сырого времени в журнале в TDatetime
function RawDateTimeToDateTime(RawDateTime: dword): TDateTime;
var
  Y, M, D, H, Min, S: word;
  rdt: dword;
begin
  rdt :=  RawDateTime;

  Y := GetBitRange(rdt, 9, 14);
  M := GetBitRange(rdt, 5, 8);
  D := GetBitRange(rdt, 0, 4);

  H := GetBitRange(rdt, 15, 19);
  Min := GetBitRange(rdt, 20, 25);
  S := GetBitRange(rdt, 26, 31);

  Result :=
    EncodeDate(2000 + Y, M, D) +
    EncodeTime(H, Min, S, 0);
end;

function DateTimeToRawDateTime(Value: TDateTime): dword;
var
  Y, M, D, H, Min, S, MS: word;
begin
  DecodeDate(Value, Y, M, D);
  DecodeTime(Value, H, Min, S, Ms);

  Y := Y - 2000;

  result := D or (M shl 5) or (Y shl 9) or (H shl 15) or (Min shl 20) or (S shl 26);
end;

function GetIniName: string;
begin
  result := IncludeTrailingPathDelimiter(ExtractFilePath(ParamStr(0))) +
      ChangeFileExt(ExtractFileName(ParamStr(0)), '.ini');
end;

function IsSubProtect: boolean;
begin
  if FIsSubProtect = dUnknown then
  begin
    if GetPrivateProfileInt('Options', 'R3_Protect', 0, PChar(GetIniName)) <> 0 then
      FIsSubProtect := dYes else
      FIsSubProtect := dNo;
  end;

  result := FIsSubProtect = dYes;
end;

function IsKnownPanelError(ErrorCode: Integer): Boolean;
begin
  result := True;
end;

function ValidateModBusCRC(const Datablock: IDataBlock): Boolean;
begin
  result := ValidateModBusPacketCRC(DataBlock.Memory^, DataBlock.Size, True);
end;

// Можно ли повторять комманду, в случае неувереннсти в выполнении
function CanRepeatCommand(Command, Param: byte): Boolean; overload;
begin
  result := false
end;

function CanRepeatCommand(const DataBlock: IDataBlock): Boolean; overload;
begin
  Assert(DataBlock.Size > 3, '{C6A22870-1AD7-4CA5-9991-0B14785B1C39}');
  result := CanRepeatCommand(PByteArray(DataBlock.Memory)^[1], PByteArray(DataBlock.Memory)^[2]);
end;

function DataBlockToHexString(const DataBlock: IDataBlock): string;
begin
  result := BufferToHexString(DataBlock.Memory^, DataBlock.Size);
end;

procedure DataBlockGet(const Datablock: IDataBlock; Buffer: Pointer; BytePos, Count: Integer);
begin
  if Datablock.Size > BytePos + Count - 1 then
    Move(PByteArray(Datablock.Memory)^[BytePos], Buffer^, Count) else
    raise Exception.Create(rsInvalidPacketReceived);
end;

procedure DataBlockGetData(const Datablock: IDataBlock; Buffer: Pointer; Count: Integer);
const
  BytePos = 3;
begin
  if Datablock.Size > BytePos + Count - 1 then
    Move(PByteArray(Datablock.Memory)^[BytePos], Buffer^, Count) else
    raise Exception.Create(rsInvalidPacketReceived);
end;

function ReverseBytes(Value: Cardinal): Cardinal;
begin
  result := system.swap(LoWord(Value)) shl 16 + system.swap(HiWord(Value));
end;

{ TRubezh3Interface }

function TRubezh3Interface.AcquireDriverIDByDatabaseSignature: string;
begin
  result := FModel.AcquireDriverIDByDatabaseSignature(Self)
end;

procedure TRubezh3Interface.AddModBusCRCToDataBlock(const DataBlock: IDataBlock);
var
  CRC: word;
begin
  with DataBlock do
  begin
    CRC := CCITT_CRC(Memory^, Size);
    Write(CRC, Sizeof(CRC))
  end;
end;

procedure TRubezh3Interface.IgnoreListOp(Device: IDevice; Append: Boolean);
var
  DataBlock: IDataBlock;
  b: byte;
  filldw: dword;
  i, DeviceAddress: integer;
begin
  FillDw := $FFFFFFFF;
  DataBlock := CreateParamQuery(prHighLevelCmd, nil, 0, True, False);
  with DataBlock do
  begin
    b := 11; Write(b, sizeof(b)); // код комманды
    if Append then
    begin
      b := $01; Write(b, sizeof(b)); // Добавление
    end else
    begin
      b := $00; Write(b, sizeof(b)); // Удаление
    end;
    if FModel.IsEventLog32 then
    begin
      FillDw := 0;
      b := 0; // номер панели
      DeviceAddress := GetRealDeviceAddress(Device);
      Datablock.Write(b, sizeof(b)); // номер панели
      b := LoByte(DeviceAddress);
      Datablock.Write(b, sizeof(b)); // лок. адрес
      Write(filldw, 3); // дополн. до 8 байт
      b := HiByte(DeviceAddress) - 1; // шлейф
      Datablock.Write(b, sizeof(b)); // номер шлейфа
    end else
    begin
      i := FModel.WriteNativeDeviceAddress(Device, DataBlock);
      Write(filldw, 5 - i {3}); // дополн. до 7 байт
    end;

  end;
  AddModBusCRCToDataBlock(DataBlock);
  CheckSendDataBlock(DataBlock);
end;

procedure TRubezh3Interface.BeginSoftUpdate(MemType: byte);
begin
  CheckSendDataBlock(CreateCommand(cmdBeginSoftUpdate, @MemType, 1));
end;

procedure TRubezh3Interface.BoltDeviceLevelCmd(Cmd: TDeviceLevelBoltCmd; const Device: IDevice);
var
  DataBlock: IDataBlock;
  b: byte;
begin
  DataBlock := CreateParamQuery(prDeviceLevelCmd, nil, 0, True, False);
  with DataBlock do
  begin
    b := $03; Write(b, sizeof(b));      // код комманды 1
    b := $71; Write(b, sizeof(b));      // код комманды 2

    b := LoByte(Device.DeviceAddress);
    Write(b, sizeof(b));                // лок. адрес

    b := 0; Write(b, sizeof(b));        // адрес панели

    b := $81; Write(b, sizeof(b));      // код комманды 3
    case cmd of
      bcOpen: b := $44;
      bcClose: b := $42;
      bcStop: b := $48;
    end;

    Write(b, sizeof(b));                // код комманды задвижки
    Write(b, sizeof(b));                // маска комманды задвижки

    b := HiByte(Device.DeviceAddress) - 1;
    Write(b, sizeof(b));                // шлейф

    b := 0;
    while DataBlock.Size < 8 do
      Write(b, sizeof(b)); // дополнениеи до 8 байт

  end;
  AddModBusCRCToDataBlock(DataBlock);
  CheckSendDataBlock(DataBlock);
end;

function TRubezh3Interface.CheckSendDataBlock(
  const DataBlock: IDataBlock; Approval: Boolean = True): IDataBlock;
var
  RetryCount: Integer;

  function PostCheckReceive(SendDB, ReceiveDB: IDataBlock): boolean;
  begin
    result := false;
    {$IFDEF DEBUG}
    WriteToLog(rkMessage, rtError, 'Bad Packet');
    {$ENDIF}
    if ReceiveDB = nil then
    begin
      {$IFDEF DEBUG}
      WriteToLog(rkMessage, rtError, 'Receive DataBlock is NULL');
      {$ENDIF}
      exit;
    end;

    {$IFDEF DEBUG}
    WriteToLog(rkMessage, rtError, 'Send DataBlock: ' + BufferToHexString(SendDB.Memory^, SendDB.Size));
    WriteToLog(rkMessage, rtError, 'Receive DataBlock: ' + BufferToHexString(ReceiveDB.Memory^, ReceiveDB.Size));
    {$ENDIF}
  end;

begin
  FBusyStart := 0;

  RetryCount := HighLevelRetryCount;
  repeat
    dec(RetryCount);
    if not SendDataBlock(DataBlock, Result) then
    begin
      if FLastErrorCode = errNAK then
        FLastErrorCode := GetExtMemError;

      if ((FLastErrorCode = errConfirmed) or (FLastErrorCode = errExtPrepare_Failed) or
        (FLastErrorCode = errExtEraseFailed) or (FLastErrorCode = errCompareFailed) or
        (FLastErrorCode = errRAMToFlashFailed) or (FLastErrorCode = errCodeCorrupted))
         and (RetryCount > 0) then
      begin
        // Do notthing - repeat

        //  Для аппаратных ошибок записи в память. Повторяем
        if FExceptionOnRepeat and (FLastErrorCode <> errConfirmed) then
          raise ERubezh3MemError.Create(ErrorCodeToString(FLastErrorCode), FLastErrorCode);

        {$IFDEF DEBUG}
        WriteToLog(rkMessage, rtModBus, Format('High level repeat. Retries left: %d', [RetryCount]));
        {$ENDIF}
      end else
		// проверка запроса-ответа
      if PostCheckReceive(DataBlock, result) then
        break else
        raise ERubezh3Error.Create(ErrorCodeToString(FLastErrorCode), FLastErrorCode);
    end else
      break;
  until RetryCount = 0;
end;

constructor TRubezh3Interface.Create(Address: Byte;
  const DataTransport: IDataTransport; PanelDeviceMod: PPanelDeviceMod);
begin
  inherited Create;
  FPanelDeviceMod := PanelDeviceMod;
  FAddress := Address;
  FActualAddress := Address;
  FDataTransport := DataTransport;
  FRetryCount := 10;
  ResetDevice;
end;

function TRubezh3Interface.CreateCommand(Fun: Byte; Context: Pointer;
  ContextSize: Integer; AddCRC: Boolean = True): IDataBlock;
var
  b: byte;
begin
  result := TDataBlock.Create;
  with result do
  begin
    b := DEFAULT_COMPUTER_ADDRESS;
    Write(b, 1);
    Write(FActualAddress, 1);
    Write(Fun, 1);
    if Context <> nil then
      Write(Context^, ContextSize);
  end;

  if AddCRC then
    AddModBusCRCToDataBlock(result);
end;

function TRubezh3Interface.CreateParamQuery(Param: Byte; Context: Pointer;
  ContextSize: Integer; WriteParam: Boolean; AddCRC: Boolean = True): IDataBlock;
var
  b: byte;
begin
  result := TDataBlock.Create;
  with result do
  begin
    b := DEFAULT_COMPUTER_ADDRESS;
    Write(b, 1);                                // 0
    Write(FActualAddress, 1);                   // 1
    if WriteParam then                          // 2
      b := cmdParamWrite else
      b := cmdParamRequest;
    Write(b, 1);                                // 3
    Write(Param, 1);                            // 4 
    if Context <> nil then
      Write(Context^, ContextSize);             // 5
  end;
  if AddCRC then
    AddModBusCRCToDataBlock(result);
end;

procedure TRubezh3Interface.DatabaseSetLock(Lock: Boolean);
var
  b: byte;
begin
  if Lock then
    b := 1 else
    b := 0;

  CheckSendDataBlock(CreateParamQuery(prDatabaseLock, @b, 1, True));
end;

function TRubezh3Interface.DateTimeGet: TDateTime;
begin
  result := RawDateTimeToDateTime(RawDateTimeGet);
end;

procedure TRubezh3Interface.DateTimeSet(DateTime: TDateTime);
begin
  RawDateTimeSet(DateTimeToRawDateTime(Datetime));
end;

destructor TRubezh3Interface.Destroy;
begin
  FLastSector.Free;
  inherited;
end;

procedure TRubezh3Interface.doTimeSlice;
begin
  if FTimeSlicer <> nil then
    FTimeSlicer.doTimeSlice;
end;

procedure TRubezh3Interface.InitDeviceConstants;
begin
  if not FConstantsAreValid then
  begin
    FDeviceEventLogSize := EventLogBufferSizeInt(LOG_FIRE_TYPE_16byte);
    FDeviceDatabaseStart := GetDatabaseStartInt;
    FConstantsAreValid := True;
  end;
end;

function TRubezh3Interface.InternalSend(const DataBlock: IDataBlock;
  out ResultDataBlock: IDataBlock): TSendBlockResult;

  function ValidateErrorCode: Boolean;

    function GetDataBlock: IDataBlock;
    begin
      if GetCommandState = csConfirmMode then
        result := FConfirmedBlock else
        result := DataBlock;
    end;

    function SameParamNo: boolean;
    var
      Offs: integer;
    begin
      Offs := ResultDataBlock.Size - 3;
      if IsSubProtect and (FAddress <> 0) and (Offs > 0) and (PByteArray(DataBlock.Memory)^[2] in [cmdParamRequest, cmdParamWrite]) then
        result := (PByteArray(ResultDataBlock.Memory)^[Offs] = PByteArray(DataBlock.Memory)^[3]) else
        result := true;
    end;

  begin
    result := False;
    if ResultDataBlock.Size > 3 then
    begin
      { Тот ли адрес }
      if (PByteArray(ResultDataBlock.Memory)^[1] = PByteArray(GetDataBlock.Memory)^[0]) and
        (PByteArray(ResultDataBlock.Memory)^[0] = PByteArray(GetDataBlock.Memory)^[1])
          then
      begin
        { Ошибка или не та команда }
        if ((PByteArray(ResultDataBlock.Memory)^[2] and $80) <> 0) or
          ((PByteArray(ResultDataBlock.Memory)^[2] and $3F) <> (PByteArray(DataBlock.Memory)^[2] and $3F))
          or not SameParamNo then
        begin
          if (GetCommandState = csInitializing) and ((PByteArray(ResultDataBlock.Memory)^[2] and $80) = 0) then
          begin
            // В состоянии инициализации, игнорируем прочие команды, кроме ошибок
            FLastErrorCode := 0;
            result := true;
          end else
          begin
            if (PByteArray(ResultDataBlock.Memory)^[2] and $80) = 0 then
            begin
              if not SameParamNo then
              begin
                {$IFDEF DEBUG}WriteToLog(rkMessage, rtModBus, 'Rubezh3Driver. Received answer with subparam different from query.');{$ENDIF}
                FLastErrorCode := errUnexpectedAnswer;
              end else
              begin
                {$IFDEF DEBUG}WriteToLog(rkMessage, rtModBus, 'Rubezh3Driver. Received answer with code different from query.');{$ENDIF}
                FLastErrorCode := errUnexpectedAnswer;
              end;
            end else
            if (PByteArray(ResultDataBlock.Memory)^[2] and $80) <> 0 then
            begin
              {$IFDEF DEBUG}WriteToLog(rkMessage, rtModBus, 'Rubezh3Driver. Received error code: 0x' + IntToHex(PByteArray(ResultDataBlock.Memory)^[3], 2));{$ENDIF}
              FLastErrorCode := PByteArray(ResultDataBlock.Memory)^[3];
            end;

            if (FLastErrorCode = errAcknowledge) and (ResultDataBlock.Size >= 5) then
            begin
              // В байте 4 - степнь двойки для миллисек
              if PByteArray(ResultDataBlock.Memory)^[4] <> 0 then
                FProtocolPausedBefore := Now + Power(2, PByteArray(ResultDataBlock.Memory)^[4]) / 1000 * OneDTSecond  else
                FProtocolPausedBefore := Now - 1;
            end;

            if not IsKnownPanelError(FLastErrorCode) then
            begin
              {$IFDEF DEBUG}
//                  MessageBeep(0);
              WriteToLog(rkMessage, rtError, 'Unknown error received. '+#13#10+
                'Query: ' + DataBlockToHexString(GetDataBlock)+#13#10+
                'Answer' + DataBlockToHexString(ResultDataBlock));
              {$ENDIF}
              FLastErrorCode := errLineError;
            end;
          end;
        end else
        begin
          // Если операция выполнена успешно
          if (GetCommandState = csConfirmMode) and ((PByteArray(ResultDataBlock.Memory)^[2] and $3F) = cmdRequestApproval)
            and not IsWriteCommand(PByteArray(GetDataBlock.Memory)^[2]) then
          begin
            FLastErrorCode := errConfirmed;
          end else
          begin
            result := True;
            FLastErrorCode := 0;
          end;
        end;
      end else
      begin
        {$IFDEF DEBUG}WriteToLog(rkMessage, rtModBus, 'Rubezh3Driver. Received data has invalid addresses.');{$ENDIF}
        FLastErrorCode := errInvalidPacketReceived;
      end;
    end else
    begin
      {$IFDEF DEBUG}WriteToLog(rkMessage, rtModBus, 'Rubezh3Driver. Received data length less 4 bytes');{$ENDIF}
      FLastErrorCode := errInvalidPacketReceived;
    end;
  end;

  procedure ValidateReservedLine;
  var
    RC: IReservedChannel;
  begin
    if Supports(FDataTransport, IReservedChannel, RC) then
    begin
      FReserveLine1Fail := RC.IsChannelFailed(FActualAddress, 0);
      FReserveLine2Fail := RC.IsChannelFailed(FActualAddress, 1);
    end else
    begin
      FReserveLine1Fail := csWorking;
      FReserveLine2Fail := csWorking;
    end;
  end;

  procedure CheckDoublingAddresses;
  begin
    FDataTransport.CheckDouble;
  end;

var
{$IFDEF DEBUG}
  DebugLog: IDebugLog;
{$ENDIF}
  N: TDatetime;
begin
  if Assigned(fOnTransportOperation) then
    FOnTransportOperation(Self);

  try
    N := Now;
    if N < FProtocolPausedBefore then
    begin
      {$IFDEF DEBUG}
      WriteToLog(rkMessage, rtModBus, 'Waiting for device. Remaining' +  FloatToStr((FProtocolPausedBefore - N) * 24 * 60 * 60) + ' сек.');
      {$ENDIF}
      sleep(Round((FProtocolPausedBefore - N) * 24 * 60 * 60 * 1000));
      result := crExtTimeout;
      exit;
    end;

    Assert(DataBlock.Size > 3, '{429EE007-BFC3-4CCC-A44C-1C3DAF4AA81B}');
    FLastSendBlockTime := now();
    ResultDataBlock := FDataTransport.SendDataBlock(DataBlock, True);
    {$IFDEF DEBUG}
    Assert(Supports(FDataTransport, IDebugLog, DebugLog));
    WriteToLog(rkMessage, rtModBus, DebugLog.GetDebugLog);
    {$ENDIF}

    inc(FBytesWritten, DataBlock.Size);
    if ResultDataBlock <> nil then
    begin
      {$IFDEF DEBUG}
      WriteToLog(rkMessage, rtModBus, 'Rubezh3Driver(' + IntToStr(FAddress)+ ').DATA: ' + BufferToHexString(ResultDataBlock.Memory^, ResultDataBlock.Size));
      {$ENDIF}
      inc(FBytesReaded, ResultDataBlock.Size);
      if ValidateModBusCRC(ResultDataBlock) then
      begin
        if ValidateErrorCode then
          result := crAnswer else
          result := crError;
      end else
      begin
        {$IFDEF DEBUG}WriteToLog(rkMessage, rtModBus, 'Rubezh3Driver. Data with invalid CRC received');{$ENDIF}
        FLastErrorCode := errBadCRC;
        result := crBadCRC;
      end;
      FNonAnswerCount := 0;
    end else
    begin
      inc(FNonAnswerCount);
      FLastErrorCode := errPanelNoAnswer;
      result := crTimeout;
    end;
  finally
    if FDataTransport.IsReserved then
      ValidateReservedLine;
    CheckDoublingAddresses;
  end;

end;

function TRubezh3Interface.IsWriteCommand(code: byte): Boolean;
begin
  result := code in [cmdParamWrite, cmdEndSoftUpdate, cmdMemClear, cmdMemWrite, cmdBeginSoftUpdate];
end;

function TRubezh3Interface.IsMemCommand(code: byte): Boolean;
begin
  result := (code = cmdMemClear) or (code = cmdMemWrite) or (code = cmdBeginSoftUpdate)
    or ((code = cmdParamWrite) and (FPanelDeviceMod.DeviceSubtype <> dsOrdinaryPanel));
end;

function TRubezh3Interface.IsReadCommand(code: byte): Boolean;
begin
  result := code = cmdParamRequest;
end;

function TRubezh3Interface.SendCustomCommand(Code: Integer; Context: pointer; ContextSize: Cardinal; out ResultDataBlock: IDataBlock): TSendBlockResult;
begin
  result := InternalSend(CreateCommand(Code, Context, ContextSize), ResultDataBlock);
end;

function TRubezh3Interface.SendDataBlock(const DataBlock: IDataBlock;
  out ResultDataBlock: IDataBlock): Boolean;

var
  RetryCount, TempCode: Integer;
  CurAnswer: TSendBlockResult;

  function ProcessBusy: boolean;
  begin
    result := True;

    if FBusyStart = 0 then
    begin
     {$IFDEF DEBUG}WriteToLog(rkMessage, rtModBus, 'Starting busy timer');{$ENDIF}
      FBusyStart := Now;
{                  RetryCount := RetryCount + ExtraRetryCount;
      ExtraRetryCount := 0; }
    end else
    if Now - FBusyStart > BUSY_WAIT  then
    begin
     {$IFDEF DEBUG}WriteToLog(rkMessage, rtModBus, 'Busy timer expired');{$ENDIF}
      result := False;
    end;

    RetryCount := ACK_RETRY_COUNT;
  end;

  function doConfirm(var SendResult: Boolean): Boolean;
  begin
    result := False;

    if EnterConfirmMode(DataBlock) then
    try
      repeat
        {$IFDEF DEBUG}WriteToLog(rkMessage, rtModBus,
          Format('MODBUS.Sending RequestAproval to Address: %d. Retries left: %d',[FActualAddress, RetryCount]));{$ENDIF}

        SendResult := SendDataBlock(CreateCommand(cmdRequestApproval, nil, 0), ResultDataBlock);
        if not SendResult then
        begin
          case FLastErrorCode of
            errAcknowledge, errDeviceBusy, errBadCRC, errInvalidPacketReceived, errLineError:
              begin
                if (FLastErrorCode in [errAcknowledge, errDeviceBusy]) or (FLastErrorCode = errConfirmed) then
                  if not ProcessBusy then
                    break;
              end; { Продолжаем слать подтверждения }
            errUnexpectedAnswer: if CanRepeatCommand(DataBlock) then
            begin
              {$IFDEF DEBUG}WriteToLog(rkMessage, rtModBus, 'MODBUS.Unexpected answer: Repeating command. Retries left:' + IntToStr(RetryCount));{$ENDIF}
              break; { Выходим из режима подтверждения и повторяем комманду }
            end;
            else exit; { При прочих ошибках прекращаем работу (result = false) }
          end;
          if CurAnswer <> crExtTimeout then
            dec(RetryCount);
        end else
        begin
          // проверяем, что это не ответ о завершении долговременной операции

        end;
      until (SendResult  = True) or (RetryCount <= 0);
      result := True;
    finally
      LeaveConfirmMode;
    end;
  end;

  procedure ValidateReservedLine;
  var
    RC: IReservedChannel;
  begin
    // Это делается для того чтобы резеврный канал вынюхал в пакетах,
    // который из каналов первый
    if Supports(FDataTransport, IReservedChannel, RC) then
    begin
      try
        CheckSendDataBlock(CreateParamQuery(prChannelNo, nil, 0, False));
        CheckSendDataBlock(CreateParamQuery(prChannelNo, nil, 0, False));
      except
        //
      end;
    end;
  end;

var
  ExtraRetryCount: integer;
begin
  result := False;
  if GetCommandState = csNone then
  begin
    {$IFDEF DEBUG}WriteToLog(rkMessage, rtModBus, 'MODBUS: Reinitializing:' + IntToStr(FActualAddress));{$ENDIF}
    SetCommandState(csInitializing);

    if not SendDataBlock(CreateCommand(cmdRequestApproval, nil, 0), ResultDataBlock) then
    begin
      SetCommandState(csNone);
      exit;
    end else
      SetCommandState(csNormalMode);

    if FDataTransport.IsReserved then
      ValidateReservedLine;

    if not FSoftUpdateMode then
      ValidateDevices;
  end;

  RetryCount := FRetryCount;
  ExtraRetryCount := ACK_RETRY_COUNT;

  repeat
    CurAnswer := InternalSend(DataBlock, ResultDatablock);
    case CurAnswer of
      crAnswer:
        begin
          result := True;
          break;
        end;
      crTimeout, crExtTimeout:
        begin
(*          if (CurAnswer = crExtTimeout) and IsReadCommand(PByteArray(DataBlock.Memory)^[2]) then
          begin
            FLastErrorCode := errConfirmed;
            exit;
          end; *)

          {$IFDEF DEBUG}
          WriteToLog(rkMessage, rtModBus, 'MODBUS.Timeout: Repeating command. Retries left:' + IntToStr(RetryCount));
          {$ENDIF};
//          MessageBeep(0);
          { Алгоритм следующий: Пока панель хоть раз отвечает, то мы ее
            спрашиваем часто, как только перестала отвечать, т.е.
            исчерпана 1/10 часть повторов, то на каждом цикле опроса мы
            ее спрашиваем только один раз }

{          if (FNonAnswerCount >= RetryCount div 10) then
          begin
            if not doConfirm(Result) then
              break;
          end;       }
        end;
      crError:
        begin
          case FLastErrorCode of
            errUSBPanelBusy:
              begin
                RetryCount := RetryCount + ExtraRetryCount;
                ExtraRetryCount := 0;
              end;
            errUSBReportNoDevice, errUSBCannotDeliver:
             {$IFDEF DEBUG}
             WriteToLog(rkMessage, rtUSB, 'USB.Timeout: Repeating command. Retries left:' + IntToStr(RetryCount))
             {$ENDIF} // обрабатываем это также как таймаут
             ;
            errAcknowledge, errDeviceBusy, errInvalidPacketReceived, errLineError:
            begin

              if (FLastErrorCode in [errAcknowledge, errDeviceBusy]) or (FLastErrorCode = errConfirmed) then
                if not ProcessBusy then
                  break;

              if (FLastErrorCode in [errAcknowledge, errDeviceBusy]) and IsReadCommand(PByteArray(DataBlock.Memory)^[2]) then
              begin
                FLastErrorCode := errConfirmed;
                exit;
              end;

              {$IFDEF DEBUG}WriteToLog(rkMessage, rtModBus, Format('MODBUS.Error: %d. Going do confirm. Retries left: %d', [FLastErrorCode, RetryCount]));{$ENDIF}
              if not doConfirm(Result) then break;

              if IsMemCommand(PByteArray(DataBlock.Memory)^[2]) then
              begin
                FLastErrorCode := GetExtMemError;
                if FLastErrorCode <> 0 then
                  result := False;
                exit;
              end;

            end;
            else
            begin
              if (FLastErrorCode = errInvalidAddress) then
              begin
                TempCode := FLastErrorCode;
                 if (GetDeviceState and $01 <> 0) then
                   FLastErrorCode := errInvalidFunction else
                   FLastErrorCode := TempCode;
              end;
              break;
            end;
          end;
        end;
      crBadCRC:
        begin
          {$IFDEF DEBUG}WriteToLog(rkMessage, rtModBus, 'MODBUS.Bad CRC: Going do confirm. Retries left:' + IntToStr(RetryCount));{$ENDIF}
          if not doConfirm(Result) then break;
        end;
    end;
    if CurAnswer <> crExtTimeout then
      dec(RetryCount);
  until (result = True) or (RetryCount <= 0) or FCanceled;
end;

function TRubezh3Interface.SendPing(out ResultDataBlock: IDataBlock): TSendBlockResult;
begin
  result := InternalSend(CreateCommand(cmdRequestApproval, nil, 0), ResultDataBlock);
end;

procedure TRubezh3Interface.SetAddressList(AddressList: TRawAddressList; Full: boolean = False);
var
  Datablock: IDataBlock;
begin
  if Full then
    DataBlock := CreateParamQuery(prDeviceList, @AddressList.adresses, Sizeof(AddressList.adresses), True) else
    DataBlock := CreateParamQuery(prAddrList, @AddressList.adresses, Sizeof(AddressList.adresses), True);
  CheckSendDataBlock(DataBlock);
end;

procedure TRubezh3Interface.SetAutomaticState(Device: IDevice; AutoOn: Boolean);
var
  DataBlock: IDataBlock;
  b: byte;
  filldw: dword;
begin
  FillDw := $FFFFFFFF;
  DataBlock := CreateParamQuery(prHighLevelCmd, nil, 0, True, False);
  with DataBlock do
  begin
    b := $19; Write(b, sizeof(b)); // код команды
    if AutoOn then
    begin
      b := $01; Write(b, sizeof(b)); // Добавление
    end else
    begin
      b := $00; Write(b, sizeof(b)); // Удаление
    end;

    if FModel.IsEventLog32 then
    begin
      FillDw := 0;
      b := LoByte(Device.DeviceAddress);
      Datablock.Write(b, sizeof(b)); // лок. адрес
      Write(filldw, 4); // дополн. до 8 байт
      b := HiByte(Device.DeviceAddress) - 1; // шлейф
      Datablock.Write(b, sizeof(b)); // номер шлейфа
    end;

  end;
  AddModBusCRCToDataBlock(DataBlock);
  CheckSendDataBlock(DataBlock);
end;

procedure TRubezh3Interface.SetBlindMode(Active: Boolean);
var
  b: byte;
begin
  if Active then
    b := 1 else
    b := 0;

  CheckSendDataBlock(CreateParamQuery(prBlindMode, @b, 1, True));
end;

procedure TRubezh3Interface.EndSoftUpdate_Reset;
begin
  CheckSendDataBlock(CreateCommand(cmdEndSoftUpdate, nil, 0));
end;
{$IFDEF IMITATOR}
procedure TRubezh3Interface.Im_SetControlToPO;
var i: byte;
begin
  i := 1;
  CheckSendDataBlock(CreateCommand(cmdBeginSoftUpdate, @i, 1));
end;
{$ENDIF}
function TRubezh3Interface.EnterConfirmMode(const DataBlock: IDataBlock): Boolean;
begin
  {$IFDEF DEBUG}WriteToLog(rkMessage, rtModBus, 'MODBUS.Trying to enter confirm mode');{$ENDIF}
  result := GetCommandState <> csConfirmMode;
  if Result then
  begin
    {$IFDEF DEBUG}WriteToLog(rkMessage, rtModBus, 'MODBUS.EnteringS to confirm mode');{$ENDIF}
    SetCommandState(csConfirmMode);
    FConfirmedBlock := DataBlock;
  end;
end;

type
  TELReqRecord = packed record
    LogType: byte;
    RecordIndex: dword;
  end;

function TRubezh3Interface.EventLogBufferSize(LogType: byte): word;
begin
  InitDeviceConstants;
  result := FDeviceEventLogSize;
end;

function TRubezh3Interface.EventLogBufferSizeInt(LogType: byte): word;
var
  Answer: IDataBlock;
begin
  Answer := CheckSendDataBlock(CreateParamQuery(prEventLogSize, @LogType, Sizeof(LogType), False));
  DataBlockGetData(Answer, @result, Sizeof(result));
  result := swap(result);
end;

procedure TRubezh3Interface.EventLogReadFullRecord16(LogType: byte; RecordIndex: dword; var EventRec: TRawEventRecord16);
var
  ReqRecord: TELReqRecord;
  Answer: IDataBlock;
begin
  ReqRecord.RecordIndex := ReverseBytes(RecordIndex);
  ReqRecord.LogType := LogType;
  Answer := CheckSendDataBlock(CreateParamQuery(prEventRecordRead, @ReqRecord, Sizeof(ReqRecord), False));
  DataBlockGetData(Answer, @EventRec, Sizeof(EventRec));
end;

procedure TRubezh3Interface.EventLogReadFullRecord32(LogType: byte; RecordIndex: dword; var EventRec: TRawEventRecord32);
var
  ReqRecord: TELReqRecord;
  Answer: IDataBlock;
begin
  ReqRecord.RecordIndex := ReverseBytes(RecordIndex);
  ReqRecord.LogType := LogType;
  Answer := CheckSendDataBlock(CreateParamQuery(prEventRecordRead, @ReqRecord, Sizeof(ReqRecord), False));
  DataBlockGetData(Answer, @EventRec, Sizeof(EventRec));
end;

function TRubezh3Interface.EventLogLastRecord(LogType: byte): dword;
var
  Answer: IDataBlock;
begin
  Answer := CheckSendDataBlock(CreateParamQuery(prEventLogCounter, @LogType, Sizeof(LogType), False));
  DataBlockGetData(Answer, @result, Sizeof(result));
  result := ReverseBytes(result);
end;

procedure TRubezh3Interface.LeaveConfirmMode;
begin
  {$IFDEF DEBUG}WriteToLog(rkMessage, rtModBus, 'MODBUS.Leaving confirm mode');{$ENDIF}
  SetCommandState(csNormalMode);
  FConfirmedBlock := nil;
end;

function TRubezh3Interface.MaxMemoryBlockSize: Integer;
begin
  if FDataTransport.MaxBlockSize = 0 then
    result := MAX_PROTOCOL_MEM_BLOCK_SIZE else
    result := min(MAX_PROTOCOL_MEM_BLOCK_SIZE, FDataTransport.MaxBlockSize);
end;

type
  TSectorRec = packed record
    StartS: byte;
    EndS: byte;
  end;

procedure TRubezh3Interface.MemClear(StartSector, EndSector: Integer);
var
  SecRec: TSectorRec;
begin
  SecRec.StartS := StartSector;
  SecRec.EndS := EndSector;
  CheckSendDataBlock(CreateCommand(cmdMemClear, @SecRec, Sizeof(SecRec)));
end;

procedure TRubezh3Interface.MemRead(Addr, ASize: Dword; Buffer: PChar);
var
  p: pchar;
  sent, tosend: dword;
begin
  p := Buffer;
  sent := 0;
  while sent < ASize do
  begin
    tosend := min(MaxMemoryBlockSize, ASize - sent);
    MemReadBlock(Addr + sent, tosend, p^);
    inc(sent, tosend);
    inc(p, tosend);
  end;
end;

procedure TRubezh3Interface.MemReadBlock(Addr, ASize: Dword; var Buffer);
var
  Result, DataBlock: IDataBlock;
  b: dword;
  sz: byte;
begin
  if (Addr and $FF000000) = $80000000 then
    DataBlock := CreateCommand(cmdMemReadROM, nil, 0, False) else
    DataBlock := CreateParamQuery(prMemoryBlock, nil, 0, False, False);

  with DataBlock do
  begin
    b := ReverseBytes(Addr and $00FFFFFF);
    Write(b, 4);

    // Значащий ноль
    sz := ASize - 1;
    Write(sz, 1);
  end;
  AddModBusCRCToDataBlock(DataBlock);
  Result := CheckSendDataBlock(DataBlock);
  DataBlockGetData(Result, @Buffer, ASize);
end;

procedure TRubezh3Interface.MemWrite(Addr, ASize: Dword; Buffer: PChar; SetByteProgress: Boolean = False);
var
  p: pchar;
  sent, tosend: dword;
begin
  if SetByteProgress then
  begin
    FBytesTotal := ASize;
    FBytesComplete := 0;
  end;
  try

    p := Buffer;
    sent := 0;
    while sent < ASize do
    begin
      tosend := min(MAX_PROTOCOL_MEM_BLOCK_SIZE, ASize - sent);
      MemWriteBlock(Addr + sent, tosend, p^);
      VerifyBlock(Addr + sent, tosend, p^);
      inc(sent, tosend);
      inc(p, tosend);

      if SetByteProgress then
        FBytesComplete := sent;
    end;

  finally
    if SetByteProgress then
    begin
      FBytesTotal := 0;
      FBytesComplete := 0;
    end;
  end;

end;
     // Запись базы в МС-ТЛ, МС-3, МС-4
procedure TRubezh3Interface.MemWriteBlockToMDSDB(ASize: DWord; const Buffer);
var
  DataBlock: IDataBlock;
begin
  DataBlock := CreateParamQuery(prMDSMemory, @Buffer, ASize, True, False);
  AddModBusCRCToDataBlock(DataBlock);
  CheckSendDataBlock(DataBlock);
end;
     // Чтение базы с МС-ТЛ, МС-3, МС-4
function TRubezh3Interface.MemReadBlockFromMDSDB(ASize: integer): String;
var
  DataBlock: IDataBlock;
  Answ: String;
begin
  DataBlock := CreateParamQuery(prMDSMemory, nil, 0, False, False);

  AddModBusCRCToDataBlock(DataBlock);
  DataBlock := CheckSendDataBlock(DataBlock);
  SetLength(Answ, ASize);
  DataBlockGetData(DataBlock, @Answ[1], ASize);
  result := Answ;
end;

procedure TRubezh3Interface.MemWriteBlock(Addr, ASize: DWord; const Buffer);
var
  DataBlock: IDataBlock;
  b: dword;
  sz: byte;
begin
  DataBlock := CreateParamQuery(prMemoryBlock, nil, 0, True, False);
  with DataBlock do
  begin
    b := ReverseBytes(Addr);
    Write(b, 4);
    // Значащий ноль
    sz := ASize - 1;
    Write(sz, 1);
    Write(Buffer, ASize);
  end;
  AddModBusCRCToDataBlock(DataBlock);
  CheckSendDataBlock(DataBlock);
end;

procedure TRubezh3Interface.MemWriteSec(Addr, ASize: Dword; Buffer: PChar);
var
  sent, tosend: dword;
  p: pchar;
begin
  p := Buffer;
  sent := 0;

  while sent < ASize do
  begin
    tosend := min(MAX_PROTOCOL_MEM_BLOCK_SIZE, ASize - sent);
    MemWriteRaw(Addr + sent, tosend, p);
    inc(sent, tosend);
    inc(p, tosend);
  end;

end;

procedure TRubezh3Interface.MemWriteRaw(Addr, ASize: Dword; Buffer: PChar);
var
  DataBlock: IDataBlock;
  b: dword;
//  sz: byte;
  i: integer;
begin
  for I := ASize-1 downto 0 do
    if byte(Buffer[i]) = $FF then
      dec(ASize) else
      break;

  if ASize = 0 then
    exit;

  DataBlock := CreateCommand(cmdMemWrite, nil, 0, False);
  with DataBlock do
  begin
    b := ReverseBytes(Addr);
    Write(b, 4);
    Write(Buffer^, ASize);
  end;
  AddModBusCRCToDataBlock(DataBlock);
  CheckSendDataBlock(DataBlock);
end;

procedure TRubezh3Interface.ResetDevice;
begin
  SetCommandState(csNone);
  FConstantsAreValid := false;
  FConfirmedBlock := nil;
end;

procedure TRubezh3Interface.ResetZoneState(ZoneNo: TZoneNo; IsPartition: Boolean);
begin
  ResetCustomState(ZoneNo, IsPartition, $10);
end;

procedure TRubezh3Interface.RunSingleDeviceCommand(PropInfo: PPropertyTypeInfo;
  const Device: IDevice);
var
  DataBlock: IDataBlock;
  b: byte;
begin
  DataBlock := CreateParamQuery(prDeviceLevelCmd, nil, 0, True, False);
  with DataBlock do
  begin
    // код комманды 1
    b := PropInfo.Command1; Write(b, sizeof(b));
    // тип устройства
    b := GetOldTableType(Device.DeviceDriver.DeviceDriverID); Write(b, sizeof(b));
    // лок. адрес
    b := LoByte(GetRealDeviceAddress(Device)); Write(b, sizeof(b));
    // адрес панели
    b := 0; Write(b, sizeof(b));
    // номер параметра в уст-ве
    b := PropInfo.shiftInMemory; Write(b, sizeof(b));
    // маска комманды устройства
    b := PropInfo.MaskCmdDev; Write(b, sizeof(b));
     // код комманды устройства
    b := PropInfo.CommandDev; Write(b, sizeof(b));
    // шлейф
    b := HiByte(GetRealDeviceAddress(Device)) - 1; Write(b, sizeof(b));
    // дополнениеи до 8 байт
    b := 0;
    while DataBlock.Size < 8 do
      Write(b, sizeof(b)); 
  end;
  AddModBusCRCToDataBlock(DataBlock);
  CheckSendDataBlock(DataBlock);
end;

procedure TRubezh3Interface.GuardSetZoneState(ZoneNo: TZoneNo; IsPartition: Boolean);
begin
  ResetCustomState(ZoneNo, IsPartition, $08);
end;

procedure TRubezh3Interface.GuardUnSetZoneState(ZoneNo: TZoneNo; IsPartition: Boolean);
begin
  ResetCustomState(ZoneNo, IsPartition, $09);
end;

function TRubezh3Interface.GetAddressList: TRawAddressList;
var
  Answer: IDataBlock;
begin
  Answer := CheckSendDataBlock(CreateParamQuery(prAddrList, nil, 0, False));
  DataBlockGetData(Answer, @result.Adresses, sizeof(result.Adresses));
  FillChar(result.types, Sizeof(result.types), 0);
end;

function TRubezh3Interface.SameAVRVersion(NeededVersion, NeededCRC: word; var AVRInfo: TAVRInfo): boolean;
var
  i: integer;
  ver, CRC: word;
begin
  result := true;

  // нумерация AVR с 1
  SetLength(AVRInfo, FPanelDeviceMod.AVRCount + 1);
  for i := 1 to FPanelDeviceMod.AVRCount do
  begin
    ver := GetFirmwareInfo(03 + i - 1, CRC);
    AVRInfo[i] := (ver = NeededVersion) and (CRC = NeededCRC);
    if not AVRInfo[i] then
      result := false;
  end;

end;

function TRubezh3Interface.GetCommandState: TCommandState;
begin
  if FServiceMode then
    result := FServiceCommandState else
    result := FBaseCommandState;
end;

function TRubezh3Interface.GetDatabaseStart: Integer;
begin
  InitDeviceConstants;
  result := FDeviceDatabaseStart;
end;

function TRubezh3Interface.GetDatabaseStart2: Integer;
var
  Answer: IDataBlock;
begin
  Answer := CheckSendDataBlock(CreateParamQuery(prDatabaseOffset2, nil, 0, False));
  DataBlockGetData(Answer, @result, sizeof(result));
  result := ReverseBytes(result);
end;

function TRubezh3Interface.GetDatabaseStartInt: Integer;
var
  Answer: IDataBlock;
begin
  Answer := CheckSendDataBlock(CreateParamQuery(prDatabaseOffset, nil, 0, False));
  DataBlockGetData(Answer, @result, sizeof(result));
  result := ReverseBytes(result);
end;

function TRubezh3Interface.GetDatabaseVersion: word;
var
  Answer: IDataBlock;
begin
  Answer := CheckSendDataBlock(CreateParamQuery(prDatabaseVersion, nil, 0, False));
  DataBlockGetData(Answer, @result, Sizeof(result));
end;

function TRubezh3Interface.GetDeviceState: word;
var
  Answer: IDataBlock;
begin
  Answer := CheckSendDataBlock(CreateParamQuery(prDeviceState, nil, 0, False));
  DataBlockGetData(Answer, @result, Sizeof(result));
  result := swap(result);
end;

function TRubezh3Interface.GetDeviceType: byte;
var
  Answer: IDataBlock;
begin
  try
    Answer := CheckSendDataBlock(CreateParamQuery(prDeviceType, nil, 0, False));
    DataBlockGetData(Answer, @result, Sizeof(result));
  except
    result := $FF;
  end;
end;

function TRubezh3Interface.GetMSChannelInfo(CountChannels: integer): TRawMSChannelInfos;
var
  Answer: IDataBlock;
begin
  FillChar(result,SizeOf(result), 0);
  try
    if CountChannels >= 1 then
    begin
      SetLength(result, Length(result) + 1);
      Answer := CheckSendDataBlock(CreateParamQuery($03, nil, 0, False));
      DataBlockGetData(Answer, @Result[0], Sizeof(Result[0]));
    end;
    if CountChannels >= 2 then
    begin
      SetLength(result, Length(result) + 1);
      Answer := CheckSendDataBlock(CreateParamQuery($04, nil, 0, False));
      DataBlockGetData(Answer, @Result[1], Sizeof(Result[1]));
    end;

  except

  end;
end;

function TRubezh3Interface.GetExtMemError: Integer;
var
  Answer: IDataBlock;
  b: byte;
begin
  Answer := CheckSendDataBlock(CreateCommand(cmdGetExtErr, nil, 0));
  DataBlockGetData(Answer, @b, Sizeof(b));

  result := b shl 8;
end;

function TRubezh3Interface.GetFirmwareInfo(MemType: byte; out CRC: word): word;
var
  Answer: IDataBlock;
  dw: dword;
begin
  // 01 (версия пользовательского ПО),
  // 02 (версия загрузчика)
  // 03 (версия ПО контроллера RSR).

  //Возвращает 4 байта: номер версии (в формате BCD) - 2 байта и CRC16 - 2 байта. Старшим байтом вперед.

  Answer := CheckSendDataBlock(CreateCommand(cmdGetFirmwareVer, @MemType, Sizeof(MemType)));
  DataBlockGetData(Answer, @dw, Sizeof(dw));
  result := swap(Word(dw));
  CRC := swap(Hiword(dw));
end;

function TRubezh3Interface.GetFirmwareVersion: word;
var
  Answer: IDataBlock;
begin
  Answer := CheckSendDataBlock(CreateParamQuery(prVersion, nil, 0, False));
  DataBlockGetData(Answer, @result, Sizeof(result));
  result := swap(dcb(Hi(result)) shl 8 + dcb(Lo(result)));
end;

function TRubezh3Interface.GetLastErrorCode: Integer;
begin
  result := FLastErrorCode;
end;

function TRubezh3Interface.GetLastOperationStatus: byte;
var
  Answer: IDataBlock;
begin
  Answer := CheckSendDataBlock(CreateParamQuery(prOperationStatus, nil, 0, False));
  DataBlockGetData(Answer, @result, Sizeof(result));
end;

function TRubezh3Interface.GetNonUsedDevices: word;
var
  Answer: IDataBlock;
begin
  Answer := CheckSendDataBlock(CreateParamQuery(prNonUsed, nil, 0, False));
  DataBlockGetData(Answer, @result, Sizeof(result));
  result := swap(result);
end;

function TRubezh3Interface.GetPanelState(CountBytes: integer = 0; Reverse: boolean = true): int64;
var
  Answer: IDataBlock;
  States: dword;
begin
  Answer := CheckSendDataBlock(CreateParamQuery(prPanelState, nil, 0, False));
  States := 0;

  if CountBytes = 0 then
    DataBlockGetData(Answer, @States, Sizeof(States)) else
    DataBlockGetData(Answer, @States, CountBytes);

  if Reverse then
    States := ReverseBytes(States);

  result := States;
end;

function TRubezh3Interface.GetPanelState64: int64;
var
  Answer: IDataBlock;
  States, ExtStates: dword;
begin
  Answer := CheckSendDataBlock(CreateParamQuery(prPanelState, nil, 0, False));
  DataBlockGetData(Answer, @States, Sizeof(States));
  States := ReverseBytes(States);
  // Дополнительные статусы
  Answer := CheckSendDataBlock(CreateParamQuery(prExtPanelState, nil, 0, False));
  DataBlockGetData(Answer, @ExtStates, Sizeof(ExtStates));
  ExtStates := ReverseBytes(ExtStates);
  {$IFDEF DEBUG}WriteToLog(rkMessage, rtOperation, 'States readed. States:' + IntToHex(States,4) +
    ', ExtStates:' + IntToHex(ExtStates,4));{$ENDIF}
  result := ExtStates * $100000000 + States;
end;

function TRubezh3Interface.GetSerialNo: string;
var
  Answer: IDataBlock;
begin
  Answer := CheckSendDataBlock(CreateParamQuery(prSerialNo, nil, 0, False));

  SetLength(Result, PANEL_MAX_SERIAL_SIZE + 1);
  FillChar(Result[1], PANEL_MAX_SERIAL_SIZE + 1, 0);
  DataBlockGetData(Answer, @result[1], Length(result));
  Result := StrPas(PChar(Result));
end;

function TRubezh3Interface.VerifyPanelAdminPassWord(PassWord: String): boolean;
var
  Answer: IDataBlock;
  Answ: Byte;
begin
  Answer := CheckSendDataBlock(CreateParamQuery(prVerifyPass, @PassWord, SizeOf(PassWord), False));
  DataBlockGetData(Answer, @Answ, Sizeof(Answ));
  if Answ = $01 then Result := true else Result := false;
end;

procedure TRubezh3Interface.GetUSBConfig(out DataRec: TDataRec);
var
  Datablock: IDataBlock;
begin
  Datablock := CheckSendDataBlock(CreateParamQuery(prUSBConfig, nil, 0, False, True));
  DataBlockGetData(Datablock, @Datarec, Sizeof(DataRec));
end;

function TRubezh3Interface.HasAVRVersion(NeededVersion: word): boolean;
var
  i: integer;
  ver, CRC: word;
begin
  result := false;

  // нумерация AVR с 1
  for i := 1 to FPanelDeviceMod.AVRCount do
  begin
    ver := GetFirmwareInfo(03 + i - 1, CRC);
    if ver = NeededVersion then
    begin
      result := true;
      break;
    end;
  end;

end;

procedure TRubezh3Interface.SetCommandState(State: TCommandState);
begin
  if FServiceMode then
    FServiceCommandState := State else
    FBaseCommandState := State;
end;

procedure TRubezh3Interface.SetFirmwareVersion(version: word);
var
  Datablock: IDataBlock;
  v: word;
begin
  v := swap(bcd(Hi(version)) shl 8 + bcd(Lo(version)));
  DataBlock := CreateParamQuery(prVersion, @v, Sizeof(v), True);
  CheckSendDataBlock(DataBlock);
end;

procedure TRubezh3Interface.SetPanelState(State: dword);
var
  Datablock: IDataBlock;
  d: dword;
begin
  d := ReverseBytes(state);
  DataBlock := CreateParamQuery(prPanelState, @d, Sizeof(d), True);
  CheckSendDataBlock(DataBlock);
end;

procedure TRubezh3Interface.SetPanelState64(State: int64);
var
  Datablock: IDataBlock;
  d: dword;
begin
  // пишутся только младшие 4 байта
  d := state;
  d := ReverseBytes(d);
  DataBlock := CreateParamQuery(prPanelState, @d, Sizeof(d), True);
  CheckSendDataBlock(DataBlock);
end;

procedure TRubezh3Interface.SetTimeSlicer(const TimeSlicer: ITimeSlicer);
begin
  FTimeSlicer := TimeSlicer;
end;

procedure TRubezh3Interface.SetUSBConfig(const DataRec: TDataRec);
begin
  CheckSendDataBlock(CreateParamQuery(prUSBConfig, @DataRec, Sizeof(DataRec), True));
end;

procedure TRubezh3Interface.TouchMemoryOperation(subfunc: byte);
begin
  CheckSendDataBlock(CreateCommand(cmdTouchMemInfo, @subfunc, Sizeof(subfunc)));
end;

procedure TRubezh3Interface.RawClearBlock;
begin
  FLastSectorNo := -1;
  if FLastSector <> nil then
    FLastSector.Clear;
end;

function TRubezh3Interface.RawDateTimeGet: dword;
var
  Answer: IDataBlock;
begin
  Answer := CheckSendDataBlock(CreateParamQuery(prDatetime, nil, 0, False));
  DataBlockGetData(Answer, @result, sizeof(result));
end;

procedure TRubezh3Interface.RawDateTimeSet(value: dword);
var
  Datablock: IDataBlock;
begin
  DataBlock := CreateParamQuery(prDatetime, @value, Sizeof(value), True);
  CheckSendDataBlock(DataBlock);
end;

class function TRubezh3Interface.GetMemBlockByAddress(Address: Integer): Integer;
begin
  if Address < $1000 then
    result := 0 else
  if Address <  $2000 then
     result := 1 else
  if Address <  $3000 then
     result := 2 else
  if Address <  $4000 then
     result := 3 else
  if Address <  $5000 then
     result := 4 else
  if Address <  $6000 then
     result := 5 else
  if Address <  $7000 then
    result := 6 else
  if Address <  $8000 then
    result := 7 else
  if Address <  $10000 then
    result := 8 else
  if Address <  $18000 then
    result := 9 else
  if Address <  $20000 then
    result := 10 else
  if Address <  $28000 then
    result := 11 else
  if Address <  $30000 then
    result := 12 else
  if Address <  $38000 then
    result := 13 else
  if Address <  $40000 then
    result := 14 else
  if Address <  $48000 then
    result := 15 else
  if Address <  $50000 then
    result := 16 else
  if Address <  $58000 then
    result := 17 else
  if Address <  $60000 then
    result := 18 else
  if Address <  $68000 then
    result := 19 else
  if Address <  $70000 then
    result := 20 else
  if Address <  $79000 then
    result := 21 else
  if Address <  $7A000 then
    result := 23 else
  if Address <  $7b000 then
    result := 24 else
  if Address <  $7c000 then
    result := 25 else
  if Address <  $7d000 then
    result := 26 else
  if Address <  $7f000 then
    result := 27 else
  if Address <  $81000 then
    result := 28 else
  if Address <  $83000 then
    result := 29 else
  if Address <  $85000 then
    result := 30 else
  if Address <  $85000 then
    result := 31 else
    raise Exception.Create(rsErrorUnableToDetermineSector + IntToHex(Address, 8));
end;

procedure TRubezh3Interface.RawPushBlock(Addr, Size: Longword; var Buf; OnlyOneSector: boolean);
var
  SectorNo: integer;
begin
  if FLastSector = nil then
    FLastSector := TMemoryStream.Create;

  SectorNo := GetMemBlockByAddress(Addr);

  // для загрузика копим весь загрузчик
  if (FLastSectorNo = -1) or (OnlyOneSector and (SectorNo <> FLastSectorNo)) then
  begin
    FLastSectorNo := SectorNo;
    FLastSectorAddr := Addr;
    FlastSector.Clear;
  end;

  FLastSector.WriteBuffer(Buf, Size);
end;

procedure TRubezh3Interface.RawWriteRepeatable(Addr, Size: LongWord; var Buf);
begin
  ExceptionOnRepeat := true;
  try
    try
      MemWriteSec(Addr, Size, @Buf);
    except
      On E: ERubezh3MemError do
        RepeatLastSector else
        raise;
    end;
  finally
    ExceptionOnRepeat := false;
  end;
end;

function TRubezh3Interface.ReadSmokiness(Device: IDevice): byte;
var
  DataBlock: IDataBlock;
  b: byte;
  Answer: IDataBlock;
  DeviceAddress: TDeviceAddress;
begin
  DataBlock := CreateParamQuery(prSmokiness, nil, 0, False, False);
  with DataBlock do
  begin
    DeviceAddress := GetRealDeviceAddress(Device);
    b := HiByte(DeviceAddress); Write(b, sizeof(b)); //
    b := LoByte(DeviceAddress); Write(b, sizeof(b)); //
  end;
  AddModBusCRCToDataBlock(DataBlock);
  Answer := CheckSendDataBlock(DataBlock);
  DataBlockGetData(Answer, @result, sizeof(result));
end;

procedure TRubezh3Interface.ReadLineStates(out Data: TLineStates);
var
  Answer: IDataBlock;
begin
  Answer := CheckSendDataBlock(CreateParamQuery(prLineStates, nil, 0, False));
  DataBlockGetData(Answer, @Data, Sizeof(Data));
end;

function TRubezh3Interface.ReadPropFromDevice(PropInfo: PPropertyTypeInfo;
  const Device: IDevice): double;

  function ReadParam(ParamNo: byte): integer;
    var
      Answer, DataBlock: IDataBlock;
      Check: boolean;
      Query, Answ: TAnswerOnWriteParam;
      Value: integer;
    begin
      DataBlock := CreateParamQuery(prDeviceMuteTermCmd, nil, 0, True, False);
      with DataBlock do
      begin
        // код комманды 1 2 - чтение, 3 - запись, F - запись без подтверждения
        Query.cmd := $02;
        // тип устройства
        Query.OldType := GetOldTableType(Device.DeviceDriver.DeviceDriverID);
        // лок. адрес
        Query.Address := LoByte(GetRealDeviceAddress(Device));
        // адрес панели
        Query.Panel := 0;
        // номер параметра в уст-ве
        Query.Param := ParamNo;
        // значение параметра 2 байта
        Query.Value := 0;
        // шлейф
        Query.Als := HiByte(GetRealDeviceAddress(Device)) - 1;
        // дополнениеи до 8 байт
        Write(Query, sizeof(Query));
      end;
      AddModBusCRCToDataBlock(DataBlock);
      try
        Answer := CheckSendDataBlock(DataBlock);
      except
        result := -1;
        exit;
      end;
      DataBlockGetData(Answer, @Answ, SizeOf(Answ));
      Check := (Answer.Size > 12)
        and (Query.OldType = Answ.OldType)
        and (Query.Address = Answ.Address)
        and (Query.Als = Answ.Als)
        and (Query.Panel = Answ.Panel)
        and (Query.Param = Answ.Param)
        ;

      if Check then
      begin
        case PropInfo.RawType of
          rdtWord: result := swap(Answ.Value);
          rdtLoByte: result := lo(swap(Answ.Value));
          rdtHiByte: result := hi(swap(Answ.Value));
        end;
        if PropInfo.HasBitRange then
          result := GetBitRange(Round(result), PropInfo.StartBit, PropInfo.EndBit);
      end else
        result := -1;
    end;

  var
    ExtValue: integer;
  begin
    result := ReadParam(PropInfo.shiftInMemory);
    if (result <> -1) and (PropInfo.shiftInMemory2 <> 0) then
    begin
      ExtValue := ReadParam(PropInfo.shiftInMemory2);
      if ExtValue <> -1 then
      begin
        result := (Round(result) shl 16) or ExtValue;
      end;
    end;

    if (result <> -1) and (PropInfo.Multiplier <> 0) then
      result := result / PropInfo.Multiplier;
  end;

function TRubezh3Interface.ReadSimpleParamFromDevice(const Device: IDevice;
  ParamNo: integer): double;
var
  Answer, DataBlock: IDataBlock;
  Check: boolean;
  Query, Answ: TAnswerOnWriteParam;
begin
  DataBlock := CreateParamQuery(prDeviceLevelCmd, nil, 0, True, False);
  with DataBlock do
  begin
    // код комманды 1: 2 - чтение, 3 - запись, F - запись без подтверждения
    Query.cmd := $02;
    // тип устройства
    Query.OldType := GetOldTableType(Device.DeviceDriver.DeviceDriverID);
    // лок. адрес
    Query.Address := LoByte(GetRealDeviceAddress(Device));
    // адрес панели
    Query.Panel := 0;
    // номер параметра в уст-ве
    Query.Param := ParamNo;
    // значение параметра 2 байта
    Query.Value := 0;
    // шлейф
    Query.Als := HiByte(GetRealDeviceAddress(Device)) - 1;
    // дополнениеи до 8 байт
    Write(Query, sizeof(Query));
  end;
  AddModBusCRCToDataBlock(DataBlock);
  try
    Answer := CheckSendDataBlock(DataBlock);
  except
    result := -1;
    exit;
  end;

  DataBlockGetData(Answer, @Answ, SizeOf(Answ));
  Check := (Answer.Size > 12)
    and (Query.OldType = Answ.OldType)
    and (Query.Address = Answ.Address)
    and (Query.Als = Answ.Als)
    and (Query.Panel = Answ.Panel)
    and (Query.Param = Answ.Param)
    ;

  if Check then
    result := swap(Answ.Value)
  else
    result := -1;
end;

function TRubezh3Interface.ReadSmokiness(DeviceAddress: Integer): byte;
var
  DataBlock: IDataBlock;
  b: byte;
  Answer: IDataBlock;
begin
  DataBlock := CreateParamQuery(prSmokiness, nil, 0, False, False);
  with DataBlock do
  begin
    b := HiByte(DeviceAddress); Write(b, sizeof(b)); //
    b := LoByte(DeviceAddress); Write(b, sizeof(b)); //
  end;
  AddModBusCRCToDataBlock(DataBlock);
  Answer := CheckSendDataBlock(DataBlock);
  DataBlockGetData(Answer, @result, sizeof(result));
end;

procedure TRubezh3Interface.RequestApproval;
var
  Answer: IDataBlock;
begin
  Answer := CheckSendDataBlock(CreateCommand(cmdRequestApproval, nil, 0), False);
end;

procedure TRubezh3Interface.RepeatLastSector;
var
  i: integer;
begin
  for I := 2 downto 0 do
  begin
    try
      // Стираем память
//        FInterface.MemClear(FLastSectorNo, FLastSectorNo);
      MemWriteSec(FLastSectorAddr, FLastSector.Size, FLastSector.Memory);
      exit;
    except
      On E: ERubezh3MemError do
      begin
        if i = 0 then
          raise;
      end else
        raise;

    end;
  end;
end;

procedure TRubezh3Interface.ResetByteCount;
begin
  FBytesReaded := 0;
  FBytesWritten := 0;
end;

procedure TRubezh3Interface.ResetClapanState;
begin
  ResetCustomState($FFFF, False, 18);
end;

procedure TRubezh3Interface.ResetCustomState(ZoneNo: TZoneNo; IsPartition: Boolean; Command: byte);
var
  DataBlock: IDataBlock;
  b: byte;
  w: word;
  filldw: dword;
begin
{  if IsPartition then
    b := 1 else
    b := 0; }
  b := Command;
  w := swap(ZoneNo);
  filldw := $FFFFFFFF;

  DataBlock := CreateParamQuery(prHighLevelCmd, nil, 0, True, False);
  with DataBlock do
  begin
    Write(b, sizeof(b));
    Write(w, sizeof(w));
    if FModel.IsEventLog32 then
    begin
      filldw := 0;
      Write(filldw, sizeof(filldw));
      b := 0;
      Write(b, sizeof(b));
    end else
    begin
      filldw := $FFFFFFFF;
      Write(filldw, sizeof(filldw));
    end;

{    if FModel.IsEventLog32 then
    begin
      b := 0; // байт 8
      Datablock.Write(b, sizeof(b));
    end; }
  end;

  AddModBusCRCToDataBlock(DataBlock);
  CheckSendDataBlock(DataBlock);
end;

procedure TRubezh3Interface.ValidateDevices;
var
  ver: word;
  ds: word;
  dtype: byte;
begin
  ver := GetFirmwareInfo(01, ds);
//  ver := GetFirmwareVersion;

  dtype := GetDeviceType;

  if (dtype <> $FF) and (FPanelDeviceMod.BaseNodeType <> dtype) then
    raise ERubezh3UserError.Create(rsPanelIncompatibleType, 0);

  if not FSoftUpdateMode then
  begin
    if Ver = 0 then
      raise ERubezh3UserError.Create(rsPanelNoFirmware, 0);

    if not (FPanelDeviceMod.DeviceSubtype in [dsIndicator{$IFDEF IMITATOR}, dsImitator {$ENDIF}]) then
    begin
      ds := GetDeviceState;
      if ds <> 0 then
        raise ERubezh3UserError.Create(rsPanelServiceMode, 0);
    end;
  end;
{$IFDEF IMITATOR}
  if not (FPanelDeviceMod.DeviceSubtype = dsImitator) then
{$ENDIF}
  if not (FPanelDeviceMod.DeviceSubtype = dsMDS5) then
    if WordRec(ver).hi > PANEL_MAX_VERSION then
      raise ERubezh3UserError.Create(rsPanelInvalidFirmwareVersion + IntToStr(WordRec(ver).hi) + '. ' + rsPleaseUpdateFS, 0);

  try
    FIsThirdVersion := (Hi(GetFirmwareVersion) = 3);
    if FIsThirdVersion then
    begin
      FSeenThird := True;
      ReplaceHaspPresence := True;
    end;
  except
    FIsThirdVersion := False;
  end;

end;


procedure TRubezh3Interface.VerifyBlock(Addr, ASize: Dword; const Buffer);
{var
  p: pointer; }
begin
  if not FDataTransport.SupportSmartRetry then
  begin
    if (GetExtMemError <> 0) then
//    if (GetLastOperationStatus = 0) then
      raise Exception.Create(rsErrorWritingMemory + IntToHex(Addr, 8));
  end;

{  GetMem(p, ASize);
  try
    MemReadBlock(Addr, ASize, p^);
    if not CompareMem(p, @Buffer, ASize) then
      raise Exception.Create('Ошибка при проверке записи в память. Адрес: ' + IntToHex(Addr, 8));
  finally
    FreeMem(p);
  end; }
end;

function TRubezh3Interface.WaitPanelUserMode: Boolean;
var
  i, mode: integer;
begin
  result := false;
  i := 100;
  while i > 0 do
  begin
    try
      mode := GetDeviceState;
    except
      mode := -1;
    end;
    case Mode of
      -1: MsgSleep(500);
      1,2: MsgSleep(500);
      0:
        begin
          result := true;
          break;
        end;
        else
          break;
      end;
      dec(i);
    end;
end;

function TRubezh3Interface.WritePropToDevice(PropInfo: PPropertyTypeInfo;
  const Device: IDevice; SplitByte: integer = -1): double;

  function CollectValue: word;
  var aPropInfo: TPropertyTypeInfo;
      Value: word;
      val2, mask: byte;
  begin
    if not PropInfo.HasBitRange then
    begin
      Value := Round(GetDevicePropDef(Device, PropInfo.Name, PropInfo.DefaultValue) * PropInfo.Multiplier);
      if IsMPTDevice(Device) then
        result := ($FF shl 8) or Value
      else
        result := Round(GetDevicePropDef(Device, PropInfo.Name, PropInfo.DefaultValue) * PropInfo.Multiplier);
    end else
    begin
      mask := RangeBitMask(PropInfo.StartBit, PropInfo.EndBit);
      val2 := Round(GetDevicePropDef(Device, PropInfo.Name, PropInfo.DefaultValue) * PropInfo.Multiplier);
      val2 := val2 shl PropInfo.StartBit;
      if IsMPTDevice(Device) then
      begin
        result := (mask shl 8) or val2;
      end else
      begin
        Value := Round(ReadSimpleParamFromDevice(Device, PropInfo.shiftInMemory));
        if Value = -1 then Value := 0;
        mask := not mask;
        Value := Value and mask;
        result := Value or val2;
      end;
    end;
  end;

var
  Answer, DataBlock: IDataBlock;
  Check: boolean;
  Query, Answ: TAnswerOnWriteParam;
begin
  if PropInfo.ReadOnly then exit;
  
  DataBlock := CreateParamQuery(prDeviceMuteTermCmd, nil, 0, True, False);
  with DataBlock do
  begin
    // У МПТ нет старшего байта параметра!
    // при записи параметра в качестве старшего выступает маска;
    // даже если надо записать параметр целиком надо указать маску (0хFF)
    // при чтении всегда - 0х00
    // код комманды 1 2 - чтение, 3 - запись, F - запись без подтверждения
    Query.cmd := $03;
    // тип устройства
    Query.OldType := GetOldTableType(Device.DeviceDriver.DeviceDriverID);
    // лок. адрес
    Query.Address := LoByte(GetRealDeviceAddress(Device));
    // адрес панели
    Query.Panel := 0;
    // номер параметра в уст-ве
    Query.Param := PropInfo.shiftInMemory;
    // значение параметра
    Query.Value := CollectValue; //Round(GetDevicePropDef(Device, PropInfo.Name, PropInfo.DefaultValue) * PropInfo.Multiplier);
    case PropInfo.RawType of
      rdtWord: Query.Value := swap(Query.Value);
      rdtLoByte: Query.Value := swap(Query.Value or (SplitByte shl 8));
      rdtHiByte: Query.Value := swap((Query.Value shl 8) or SplitByte);
    end;
    // шлейф
    Query.Als := HiByte(GetRealDeviceAddress(Device)) - 1;

    Write(Query, sizeof(Query));

  end;
  AddModBusCRCToDataBlock(DataBlock);
  try
    Answer := CheckSendDataBlock(DataBlock);
  except
    result := -1;
    exit;
  end;
  DataBlockGetData(Answer, @Answ, SizeOf(Answ));
  Check := (Answer.Size > 12)
    and (Query.OldType = Answ.OldType)
    and (Query.Address = Answ.Address)
    and (Query.Als = Answ.Als)
    and (Query.Panel = Answ.Panel)
    and (Query.Param = Answ.Param)
    and (Query.Value = Answ.Value)
    ;

  if Check then
  begin
    case PropInfo.RawType of
      rdtWord: result := swap(Answ.Value);
      rdtLoByte: result := lo(swap(Answ.Value));
      rdtHiByte: result := hi(swap(Answ.Value));
    end;
    
    if PropInfo.HasBitRange then
      result := GetBitRange(Round(result), PropInfo.StartBit, PropInfo.EndBit);
    if IsMPTDevice(Device) then
      result := Round(result) and $00FF;
    result := result / PropInfo.Multiplier;
  end
  else
    result := -1;
end;

function TRubezh3Interface.WriteSimpleParamFromDevice(const Device: IDevice;
  ParamNo, ParamValue: integer): double;
var
  Answer, DataBlock: IDataBlock;
  Check: boolean;
  Query, Answ: TAnswerOnWriteParam;
begin
  DataBlock := CreateParamQuery(prDeviceLevelCmd, nil, 0, True, False);
  with DataBlock do
  begin
    // код комманды 1: 2 - чтение, 3 - запись, F - запись без подтверждения
    Query.cmd := $03;
    // тип устройства
    Query.OldType := GetOldTableType(Device.DeviceDriver.DeviceDriverID);
    // лок. адрес
    Query.Address := LoByte(GetRealDeviceAddress(Device));
    // адрес панели
    Query.Panel := 0;
    // номер параметра в уст-ве
    Query.Param := ParamNo;
    // значение параметра
    Query.Value := ParamValue;

    // шлейф
    Query.Als := HiByte(GetRealDeviceAddress(Device)) - 1;

    Write(Query, sizeof(Query));

  end;
  AddModBusCRCToDataBlock(DataBlock);
  try
    Answer := CheckSendDataBlock(DataBlock);
  except
    result := -1;
    exit;
  end;

  DataBlockGetData(Answer, @Answ, SizeOf(Answ));
  Check := (Answer.Size > 12)
    and (Query.OldType = Answ.OldType)
    and (Query.Address = Answ.Address)
    and (Query.Als = Answ.Als)
    and (Query.Panel = Answ.Panel)
    and (Query.Param = Answ.Param)
    and (Query.Value = Answ.Value)
    ;

  if Check then
    result := swap(Answ.Value)
  else
    result := -1;
end;

{ ERubezh3Error }

constructor ERubezh3Error.Create(const Msg: string; ErrorCode: Integer);
begin
  inherited Create(Msg);
  FErrorCode := ErrorCode;
end;


{ TRawAdrressListSort }

function TRawAdrressListSort.Compare(Index1, Index2: Integer): Integer;

  function GetV(Index: integer): integer;
  begin
    if FRecords.adresses[Index] = 0 then
      result := 1024 else
      result := FRecords.adresses[Index];
  end;

begin
  // 0 - надо в конец
  result := GetV(Index1) - GetV(Index2);
end;

constructor TRawAdrressListSort.Create(Records: PRawAddressList);
begin
  inherited Create;
  FRecords := Records;
end;

function TRawAdrressListSort.High: Integer;
begin
  result := System.High(FRecords.adresses);
end;

function TRawAdrressListSort.Low: Integer;
begin
  result := 0;
end;

procedure TRawAdrressListSort.Swap(Index1, Index2: Integer);
var
  a: integer;
begin
  a := FRecords.adresses[Index1];
  FRecords.adresses[Index1] := FRecords.adresses[Index2];
  FRecords.adresses[Index2] := a;

  a := FRecords.types[Index1];
  FRecords.types[Index1] := FRecords.types[Index2];
  FRecords.types[Index2] := a;

end;


{ TSingleAdrressListSort }

function TSingleAdrressListSort.Compare(Index1, Index2: Integer): Integer;

  function GetV(Index: integer): integer;
  begin
    if FRecords[Index] = 0 then
      result := 1024 else
      result := FRecords[Index];
  end;

begin
  // 0 - надо в конец
  result := GetV(Index1) - GetV(Index2);
end;

constructor TSingleAdrressListSort.Create(Records: PSingleAddressList);
begin
  inherited Create;
  FRecords := Records;
end;

function TSingleAdrressListSort.High: Integer;
begin
  result := System.High(FRecords^);
end;

function TSingleAdrressListSort.Low: Integer;
begin
  result := 0;
end;

procedure TSingleAdrressListSort.Swap(Index1, Index2: Integer);
var
  a: integer;
begin
  a := FRecords[Index1];
  FRecords[Index1] := FRecords[Index2];
  FRecords[Index2] := a;
end;

end.

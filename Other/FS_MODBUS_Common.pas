unit FS_MODBUS_Common;

interface
uses sysutils;

function ModBusCRC(const Data; DataSize: Integer): Word;
function CCITT_CRC(const Data; DataSize: Integer): Word;
function UpdateCCITT_CRC(b: byte; CRC: word): word;
function ValidateModBusPacketCRC(const Data; DataSize: Integer; CCITT: Boolean): Boolean;
function FindModBusPacket(const Data; DataSize: Integer): Integer;
function BufferToHexString(const Buffer; Size: Integer): string;
function Crc_8n(Data: pointer; DataLen: word) : Byte;
procedure SwapWords(Data: Pointer; ByteCount: integer);

implementation

function ModBusCRC(const Data; DataSize: Integer): Word;
var
 i,j,f : Integer;
begin
 // CRC ModBus in RTU mode
 result := $FFFF;
 for i := 0 to DataSize - 1 do
 begin
   result := result xor Ord(PByteArray(@Data)[i]);
   for j := 1 to 8 do begin
     f := result and $0001;
     result := result shr 1;
     if f = 1 then result := result xor $A001;
   end;
 end;
end;

// 16-bit CRC using CCITT polynomial.
function UpdateCCITT_CRC(b: byte; CRC: word): word;
var
  i: integer;
begin
  CRC := CRC xor word(word(b) shl 8);
  for i := 1 to 8 do
    if (CRC and $8000)<>0 then
      CRC := (CRC shl 1) xor $1021
    else
      CRC := CRC shl 1;
  Result := CRC;
end;


(*const
  CRC16Table : Array [0..255] of Word = (
    $0000, $1021, $2042, $3063, $4084, $50a5, $60c6, $70e7,
    $8108, $9129, $a14a, $b16b, $c18c, $d1ad, $e1ce, $f1ef,
    $1231, $0210, $3273, $2252, $52b5, $4294, $72f7, $62d6,
    $9339, $8318, $b37b, $a35a, $d3bd, $c39c, $f3ff, $e3de,
    $2462, $3443, $0420, $1401, $64e6, $74c7, $44a4, $5485,
    $a56a, $b54b, $8528, $9509, $e5ee, $f5cf, $c5ac, $d58d,
    $3653, $2672, $1611, $0630, $76d7, $66f6, $5695, $46b4,
    $b75b, $a77a, $9719, $8738, $f7df, $e7fe, $d79d, $c7bc,
    $48c4, $58e5, $6886, $78a7, $0840, $1861, $2802, $3823,
    $c9cc, $d9ed, $e98e, $f9af, $8948, $9969, $a90a, $b92b,
    $5af5, $4ad4, $7ab7, $6a96, $1a71, $0a50, $3a33, $2a12,
    $dbfd, $cbdc, $fbbf, $eb9e, $9b79, $8b58, $bb3b, $ab1a,
    $6ca6, $7c87, $4ce4, $5cc5, $2c22, $3c03, $0c60, $1c41,
    $edae, $fd8f, $cdec, $ddcd, $ad2a, $bd0b, $8d68, $9d49,
    $7e97, $6eb6, $5ed5, $4ef4, $3e13, $2e32, $1e51, $0e70,
    $ff9f, $efbe, $dfdd, $cffc, $bf1b, $af3a, $9f59, $8f78,
    $9188, $81a9, $b1ca, $a1eb, $d10c, $c12d, $f14e, $e16f,
    $1080, $00a1, $30c2, $20e3, $5004, $4025, $7046, $6067,
    $83b9, $9398, $a3fb, $b3da, $c33d, $d31c, $e37f, $f35e,
    $02b1, $1290, $22f3, $32d2, $4235, $5214, $6277, $7256,
    $b5ea, $a5cb, $95a8, $8589, $f56e, $e54f, $d52c, $c50d,
    $34e2, $24c3, $14a0, $0481, $7466, $6447, $5424, $4405,
    $a7db, $b7fa, $8799, $97b8, $e75f, $f77e, $c71d, $d73c,
    $26d3, $36f2, $0691, $16b0, $6657, $7676, $4615, $5634,
    $d94c, $c96d, $f90e, $e92f, $99c8, $89e9, $b98a, $a9ab,
    $5844, $4865, $7806, $6827, $18c0, $08e1, $3882, $28a3,
    $cb7d, $db5c, $eb3f, $fb1e, $8bf9, $9bd8, $abbb, $bb9a,
    $4a75, $5a54, $6a37, $7a16, $0af1, $1ad0, $2ab3, $3a92,
    $fd2e, $ed0f, $dd6c, $cd4d, $bdaa, $ad8b, $9de8, $8dc9,
    $7c26, $6c07, $5c64, $4c45, $3ca2, $2c83, $1ce0, $0cc1,
    $ef1f, $ff3e, $cf5d, $df7c, $af9b, $bfba, $8fd9, $9ff8,
    $6e17, $7e36, $4e55, $5e74, $2e93, $3eb2, $0ed1, $1ef0);

Function UpdateCCITT_CRC(const Octet : Byte; const CRC16 : Word) : Word;
Begin
  Result := CRC16Table [Hi (CRC16) xor Octet] xor (CRC16 * 256);
End;*)

function CCITT_CRC(const Data; DataSize: Integer): Word;
var
  i: integer;
begin
  result := 0;
  for i := 0 to DataSize - 1 do
    result := UpdateCCITT_CRC(PByteArray(@Data)[i], result);

  result := swap(result);
end;

function ValidateModBusPacketCRC(const Data; DataSize: Integer; CCITT: Boolean): Boolean;
var
  CRC: word;
begin
  result := False;
  if DataSize > 2 then
  begin
    CRC := Word(Pointer(Integer(@Data) + DataSize - 2)^);
    if CCITT then
      result := CCITT_CRC(Data, DataSize - 2) = CRC else
      result := ModBusCRC(Data, DataSize - 2) = CRC;
  end
end;

function FindModBusPacket(const Data; DataSize: Integer): Integer;
var
  CRC: word;
  i: integer;
  RealDataSize: Integer;
begin
  result := -1;
  if DataSize > 2 then
  begin
    RealDataSize := DataSize - 2;
    CRC := Word(Pointer(Integer(@Data) + RealDataSize)^);
    for i := RealDataSize - 1 downto 0 do
    begin
      if ModBusCRC(PByteArray(@Data)[i], RealDataSize - i) = CRC then
      begin
        result := i;
        break;
      end;
    end;
  end
end;

function BufferToHexString(const Buffer; Size: Integer): string;
var
  i: integer;
begin
  result := '';
  for i := 0 to Size-1 do
    result := result + IntToHex(PByteArray(@Buffer)[i], 2) + ' ';
end;

function Crc_8n(Data: pointer; DataLen: word) : Byte;
Var
  j, cbit, aout, crc, crc_a, crc_b : Byte;
  i : integer;
  p: PByteArray;
begin
  P := Data;
  crc := 0;
  i := 0;
  repeat
    crc_a := p[i];
    inc(i);
    j := 8;
    cbit := 1;
    repeat
      crc_b := crc_a;
      crc_b := crc_b xor crc;
      aout := crc_b and cbit;
      if aout<>0 then
      begin
        crc := crc xor $18;
        crc := crc shr 1;
        crc := crc or $80;
      end else
      begin
        crc := crc shr 1;
      end;
      crc_a := crc_a shr 1;
      dec(j);
    until j = 0;
    dec(DataLen);
  until DataLen = 0;

  result := crc;
end;

procedure SwapWords(Data: Pointer; ByteCount: integer);
var
  i: integer;
begin
  for i := 0 to ByteCount div 2 - 1 do
    PWordArray(Data)[i] := Swap(PWordArray(Data)[i])
end;

end.

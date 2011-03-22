/******************************************************************************/
/***                                 Домены                                 ***/
/******************************************************************************/

CREATE DOMAIN TDATETIME AS 
TIMESTAMP;

CREATE DOMAIN TDATETIMEMS AS 
DOUBLE PRECISION;

CREATE DOMAIN TFOREIGNKEY AS 
INTEGER;

CREATE DOMAIN TINTEGER AS 
INTEGER;

CREATE DOMAIN TMEMO AS 
BLOB SUB_TYPE 1 SEGMENT SIZE 80 CHARACTER SET WIN1251;

CREATE DOMAIN TNAME AS 
VARCHAR(64) CHARACTER SET WIN1251 
COLLATE WIN1251;

CREATE DOMAIN TPLACEMENT AS 
VARCHAR(128) CHARACTER SET WIN1251 
COLLATE WIN1251;

CREATE DOMAIN TPRIMARYKEY AS 
INTEGER
NOT NULL;

CREATE DOMAIN TTEXT AS 
VARCHAR(256) CHARACTER SET WIN1251 
COLLATE WIN1251;



/******************************************************************************/
/***                               Генераторы                               ***/
/******************************************************************************/

CREATE GENERATOR GN;
CREATE GENERATOR GNDEVICENODES;
CREATE GENERATOR GNDEVICES;
CREATE GENERATOR GNDRIVERS;
CREATE GENERATOR GNEVENTACTIONS;
CREATE GENERATOR GNEVENTS;
CREATE GENERATOR GNGROUPRIGHTS;
CREATE GENERATOR GNSEACT;
CREATE GENERATOR GNSECACT;
CREATE GENERATOR GNSECOBJ;
CREATE GENERATOR GNSECOBJTYPE;
CREATE GENERATOR GNTYPEEVENTS;
CREATE GENERATOR GNUSERGROUPS;
CREATE GENERATOR GNUSERRIGHTS;
CREATE GENERATOR GNUSERS;
CREATE GENERATOR GNUSERS_GROUPS;
CREATE GENERATOR GNVERSIONS;
CREATE GENERATOR GNZONEDEV;
CREATE GENERATOR GNPARTITIONS;
CREATE GENERATOR GNPARTZONE;
CREATE GENERATOR GNZONES;
CREATE GENERATOR GNDEVICEUID;
CREATE GENERATOR GNEVENTTEXT;
CREATE GENERATOR GNSETTINGS;
CREATE GENERATOR GNPARAMHISTORY;
CREATE GENERATOR GNPARAMNAMES;
CREATE GENERATOR GNIGNORE_LIST;

/******************************************************************************/
/***                               Исключения                               ***/
/******************************************************************************/

CREATE EXCEPTION ERROR '';

/******************************************************************************/
/***                               Процедуры                                ***/
/******************************************************************************/

SET TERM ^ ;

CREATE PROCEDURE DEVICES_DELETE(
    IDDEVICENODES INTEGER,
    NEWIDVERSIONS INTEGER,
    RECURSIVE INTEGER)
AS
BEGIN
  EXIT;
END^

CREATE PROCEDURE DEVICES_INSERT(
    IN_IDDEVICENODES INTEGER,
    DRIVERID VARCHAR(64),
    ADDRESS INTEGER,
    PLACEMENT VARCHAR(128) CHARACTER SET WIN1251,
    IDDEVICENODESPARENT INTEGER,
    NEWIDVERSIONS INTEGER,
    PROPERTIES BLOB SUB_TYPE 0 SEGMENT SIZE 80)
RETURNS (
    IDDEVICENODES INTEGER,
    IDDEVICES INTEGER)
AS
BEGIN
  EXIT;
END^


CREATE PROCEDURE DEVICES_SELECTACTUAL(
    IDDEVICENODESPARENT INTEGER,
    RECURSIVE INTEGER)
RETURNS (
    IDDEVICES INTEGER,
    IDDEVICENODES INTEGER,
    IDVERSIONS INTEGER)
AS
BEGIN
  EXIT;
END^


CREATE PROCEDURE VERSION_CREATE
RETURNS (
    IDVERSIONS INTEGER)
AS
BEGIN
  EXIT;
END^


CREATE PROCEDURE GETUSERRIGHTS(
    NAME VARCHAR(64) CHARACTER SET WIN1251)
RETURNS (
    IDSECACT INTEGER,
    IDSECOBJ INTEGER)
AS
BEGIN
  EXIT;
END^


CREATE PROCEDURE ZONE_INSERT(
    IDZONES INTEGER,
    NAME VARCHAR(64) CHARACTER SET WIN1251,
    DESCRIPTION VARCHAR(256) CHARACTER SET WIN1251,
    ZONEID INTEGER,
    PROPERTIES BLOB SUB_TYPE 0 SEGMENT SIZE 80)
RETURNS (
    ID_ZONES INTEGER,
    ID_SECOBJ INTEGER)
AS
BEGIN
  EXIT;
END^


CREATE PROCEDURE ZONEDEV_INSERT(
    IDZONES INTEGER,
    IDDEVICES INTEGER)
RETURNS (
    ID_ZONEDEV INTEGER)
AS
BEGIN
  EXIT;
END^


CREATE PROCEDURE USERS_INSERT(
    IDUSERS INTEGER,
    NAME VARCHAR(64) CHARACTER SET WIN1251,
    PASSWRD VARCHAR(64),
    FULLNAME VARCHAR(64) CHARACTER SET WIN1251,
    EXTSecurity BLOB SUB_TYPE 1 SEGMENT SIZE 80 CHARACTER SET WIN1251)
RETURNS (
    ID_USERS INTEGER)
AS
BEGIN
  EXIT;
END^


CREATE PROCEDURE USERGROUPS_INSERT(
    IDUSERGROUPS INTEGER,
    NAME VARCHAR(64) CHARACTER SET WIN1251,
    EXTSecurity BLOB SUB_TYPE 1 SEGMENT SIZE 80 CHARACTER SET WIN1251)
RETURNS (
    ID_USERGROUPS INTEGER)
AS
BEGIN
  EXIT;
END^


CREATE PROCEDURE USERS_GROUPS_INSERT(
    IDUSERS INTEGER,
    IDUSERGROUPS INTEGER)
RETURNS (
    IDUSERS_GROUPS INTEGER)
AS
BEGIN
  EXIT;
END^


CREATE PROCEDURE USERS_DELETE(
    IDUSERS INTEGER)
AS
BEGIN
  EXIT;
END^


SET TERM ; ^



/******************************************************************************/
/***                                Таблицы                                 ***/
/******************************************************************************/

CREATE TABLE DEVICENODES (
    IDDEVICENODES TPRIMARYKEY NOT NULL);


CREATE TABLE DEVICES (
    IDDEVICES TPRIMARYKEY,
    IDDRIVERS TFOREIGNKEY NOT NULL,
    ADDRESS TINTEGER,
    PLACEMENT TPLACEMENT,
    IDVERSIONS TFOREIGNKEY NOT NULL,
    DELETEIDVERSIONS TFOREIGNKEY,
    IDDEVICENODES TFOREIGNKEY NOT NULL,
    IDDEVICENODESPARENT TFOREIGNKEY,
    PROPERTIES TMEMO,
    DISABLED TINTEGER
);


CREATE TABLE DRIVERS (
    IDDRIVERS TPRIMARYKEY NOT NULL,
    DRIVERID TNAME,
    CATEGORY TINTEGER);


CREATE TABLE EVENTACTIONS (
    IDEVENTACTIONS TPRIMARYKEY,
    IDEVENTS TFOREIGNKEY,
    IDUSERS TFOREIGNKEY NOT NULL,
    DT TDATETIME,
    ACTIONNO TINTEGER,
    COMMENT TTEXT);


CREATE TABLE EVENTS (
    IDEVENTS TPRIMARYKEY,
    IDTYPEEVENTS TFOREIGNKEY,
    IDSUBSYSTEM TFOREIGNKEY,
    IDZONES TFOREIGNKEY,
    IDDEVICES TFOREIGNKEY,
    IDDEVICESSOURCE TFOREIGNKEY,
    EVENTPRIORITY TINTEGER,
/*    EVENTDESC TTEXT, */
    DT TDATETIMEMS,
    SYSDT TDATETIMEMS,
    DEVINDEX TINTEGER,
    DEVSECINDEX TINTEGER,
    DEVINDEX_OVF TINTEGER,
    DEVSECINDEX_OVF TINTEGER,
    EventAttr TMemo,
    USERINFO TMEMO);

CREATE TABLE GROUPRIGHTS (
    IDGROUPRIGHTS TPRIMARYKEY NOT NULL,
    IDUSERGROUPS TFOREIGNKEY NOT NULL,
    IDSECOBJ INTEGER NOT NULL,
    IDSECACT INTEGER NOT NULL);

CREATE TABLE PARTITIONS (
    IDPARTITIONS TPRIMARYKEY NOT NULL,
    PARTTYPE TNAME,
    NUMBER TINTEGER,
    NAME TNAME,
    PROPERTIES TMEMO,
    DESCRIPTION TTEXT);

CREATE TABLE PARTZONE (
    IDPARTZONE TPRIMARYKEY NOT NULL,
    IDPARTITIONS TFOREIGNKEY NOT NULL,
    IDZONES TFOREIGNKEY);

CREATE TABLE SECACT (
    IDSECACT INTEGER NOT NULL,
    NAME TNAME,
    NUM TINTEGER,
    IDSECOBJTYPE TFOREIGNKEY);


CREATE TABLE SECOBJ (
    IDSECOBJ INTEGER NOT NULL,
    NAME TNAME,
    IDSECOBJTYPE TFOREIGNKEY);


CREATE TABLE SECOBJTYPE (
    IDSECOBJTYPE TPRIMARYKEY,
    NAME TNAME,
    NUM TINTEGER);


CREATE TABLE USERGROUPS (
    IDUSERGROUPS TPRIMARYKEY,
    NAME TNAME);


CREATE TABLE USERRIGHTS (
    IDUSERRIGHTS TPRIMARYKEY,
    IDUSERS TFOREIGNKEY NOT NULL,
    DELETEFLAG TINTEGER,
    IDSECACT INTEGER NOT NULL,
    IDSECOBJ INTEGER NOT NULL);


CREATE TABLE USERS (
    IDUSERS TPRIMARYKEY NOT NULL,
    NAME TNAME,
    PASSWRD TNAME,
    FULLNAME TTEXT,
    DELETED TINTEGER,
    ISBUILTIN TINTEGER);


CREATE TABLE USERS_GROUPS (
    IDUSERS_GROUPS INTEGER NOT NULL,
    IDUSERS TFOREIGNKEY NOT NULL,
    IDUSERGROUPS TFOREIGNKEY NOT NULL);


CREATE TABLE VERSIONS (
    IDVERSIONS INTEGER NOT NULL,
    DT TDATETIME);


CREATE TABLE ZONEDEV (
    IDZONEDEV TPRIMARYKEY,
    IDZONES TFOREIGNKEY NOT NULL,
    IDDEVICES TFOREIGNKEY);


CREATE TABLE ZONES (
    IDZONES TPRIMARYKEY NOT NULL,
    NAME TNAME,
    DESCRIPTION TTEXT,
    IDSECOBJ TFOREIGNKEY NOT NULL,
    ZONEID TINTEGER,
    PROPERTIES TMEMO
    );


CREATE TABLE DEVICEUID (
    IDDEVICEUID TPRIMARYKEY,
    UID TNAME);

CREATE TABLE EVENTTEXT (
    IDEVENTTEXT TPRIMARYKEY,
    EVENTTEXT TTEXT);

CREATE TABLE SETTINGS (
    IDSETTINGS TPRIMARYKEY,
    SNAME TNAME,
    SDATA TMEMO);
    
ALTER TABLE EVENTS ADD IDDEVICEUID TFOREIGNKEY;
ALTER TABLE EVENTS ADD IDEVENTTEXT TFOREIGNKEY;

ALTER TABLE USERGROUPS ADD ExtSecurity TMemo;
ALTER TABLE USERS ADD ExtSecurity TMemo;

ALTER TABLE DEVICES ADD DEVPARAMS TMEMO;


CREATE TABLE PARAMNAMES (
    IDPARAMNAMES  TPRIMARYKEY ,
    PARAMNAME     TNAME
);

CREATE TABLE PARAMHISTORY (
    IDPARAMHISTORY  TPRIMARYKEY ,
    IDDEVICENODES TFOREIGNKEY,
    IDPARAMNAMES    TFOREIGNKEY,
    PARAMVALUE      DOUBLE PRECISION,
    PTIME           TDATETIMEMS
);

CREATE TABLE IGNORE_LIST (
    IDIGNORE_LIST TPRIMARYKEY,
    IDDEVICES TFOREIGNKEY,
    USERINFO TMEMO,
    IGNORED_SINCE TDATETIME);

/******************************************************************************/
/***                             Представления                              ***/
/******************************************************************************/



/******************************************************************************/
/***                              Primary keys                              ***/
/******************************************************************************/


ALTER TABLE DEVICENODES ADD CONSTRAINT DEVICENODES_PK PRIMARY KEY (IDDEVICENODES);
ALTER TABLE DEVICES ADD CONSTRAINT DEVICES_PK PRIMARY KEY (IDDEVICES);
ALTER TABLE DRIVERS ADD CONSTRAINT DRIVERS_PK PRIMARY KEY (IDDRIVERS);
ALTER TABLE EVENTACTIONS ADD CONSTRAINT EVENTACTIONS_PK PRIMARY KEY (IDEVENTACTIONS);
ALTER TABLE EVENTS ADD CONSTRAINT EVENTS_PK PRIMARY KEY (IDEVENTS);
ALTER TABLE GROUPRIGHTS ADD CONSTRAINT GROUPRIGHTS_PK PRIMARY KEY (IDGROUPRIGHTS);
alter table PARTITIONS add CONSTRAINT PK_PARTITIONS primary key (IDPARTITIONS);
alter table PARTZONE add constraint PK_PARTZONE primary key (IDPARTZONE);
ALTER TABLE SECACT ADD CONSTRAINT SECACTIONS_PK PRIMARY KEY (IDSECACT);
ALTER TABLE SECOBJ ADD CONSTRAINT SECOBJECTS_PK PRIMARY KEY (IDSECOBJ);
ALTER TABLE SECOBJTYPE ADD CONSTRAINT SECOBJTYPE_PK PRIMARY KEY (IDSECOBJTYPE);
ALTER TABLE USERGROUPS ADD CONSTRAINT UERGROUPS_PK PRIMARY KEY (IDUSERGROUPS);
ALTER TABLE USERRIGHTS ADD CONSTRAINT USERRIGHTS_PK PRIMARY KEY (IDUSERRIGHTS);
ALTER TABLE USERS ADD CONSTRAINT USERS_PK PRIMARY KEY (IDUSERS);
ALTER TABLE USERS_GROUPS ADD CONSTRAINT USERS_GROUPS_PK PRIMARY KEY (IDUSERS_GROUPS);
ALTER TABLE VERSIONS ADD CONSTRAINT VERSIONS_PK PRIMARY KEY (IDVERSIONS);
ALTER TABLE ZONEDEV ADD CONSTRAINT ZONEDEV_PK PRIMARY KEY (IDZONEDEV);
ALTER TABLE ZONES ADD CONSTRAINT ZONES_PK PRIMARY KEY (IDZONES);
alter table DEVICEUID add constraint DEVICEUID_PK primary key (IDDEVICEUID);
alter table EVENTTEXT add constraint EVENTTEXT_PK primary key (IDEVENTTEXT);
ALTER TABLE SETTINGS ADD CONSTRAINT SETTINGS_PK PRIMARY KEY (IDSETTINGS);
ALTER TABLE PARAMNAMES ADD CONSTRAINT PK_PARAMNAMES PRIMARY KEY (IDPARAMNAMES);
ALTER TABLE PARAMHISTORY ADD CONSTRAINT PK_PARAMHISTORY PRIMARY KEY (IDPARAMHISTORY);

/******************************************************************************/
/***                           Unique constraints                           ***/
/******************************************************************************/
alter table DEVICEUID add constraint UNQ_DEVICEUID unique (UID);
ALTER TABLE SETTINGS ADD CONSTRAINT UNQ_SETTINGS UNIQUE (SNAME);

/******************************************************************************/
/***                              Foreign keys                              ***/
/******************************************************************************/


ALTER TABLE DEVICES ADD CONSTRAINT FK_DEV_DRIVERS FOREIGN KEY (IDDRIVERS) REFERENCES DRIVERS (IDDRIVERS);
ALTER TABLE DEVICES ADD CONSTRAINT FK_DEV_VERSIONS FOREIGN KEY (IDVERSIONS) REFERENCES VERSIONS (IDVERSIONS);
ALTER TABLE DEVICES ADD CONSTRAINT FK_DEVICENODES FOREIGN KEY (IDDEVICENODES) REFERENCES DEVICENODES (IDDEVICENODES) ON DELETE CASCADE ON UPDATE CASCADE;
ALTER TABLE DEVICES ADD CONSTRAINT FK_DEVICENODESPARENT FOREIGN KEY (IDDEVICENODESPARENT) REFERENCES DEVICENODES (IDDEVICENODES) ON DELETE CASCADE ON UPDATE CASCADE;
ALTER TABLE EVENTACTIONS ADD CONSTRAINT FK_EVENTS FOREIGN KEY (IDEVENTS) REFERENCES EVENTS (IDEVENTS) ON DELETE CASCADE ON UPDATE CASCADE;
ALTER TABLE EVENTACTIONS ADD CONSTRAINT FK_USERS FOREIGN KEY (IDUSERS) REFERENCES USERS (IDUSERS);
ALTER TABLE EVENTS ADD CONSTRAINT FK_EV_ZONES FOREIGN KEY (IDZONES) REFERENCES ZONES (IDZONES) ON DELETE SET NULL ON UPDATE CASCADE;
ALTER TABLE EVENTS ADD CONSTRAINT FK_EV_DEVICES FOREIGN KEY (IDDEVICES) REFERENCES DEVICES (IDDEVICES);
ALTER TABLE EVENTS ADD CONSTRAINT FK_EV_DEVICES_SOURCE FOREIGN KEY (IDDEVICESSOURCE) REFERENCES DEVICES (IDDEVICES);
ALTER TABLE GROUPRIGHTS ADD CONSTRAINT FK_GR_USERGROUPS FOREIGN KEY (IDUSERGROUPS) REFERENCES USERGROUPS (IDUSERGROUPS) ON DELETE CASCADE ON UPDATE CASCADE;
ALTER TABLE GROUPRIGHTS ADD CONSTRAINT FK_GR_SECOBJ FOREIGN KEY (IDSECOBJ) REFERENCES SECOBJ (IDSECOBJ) ON DELETE CASCADE ON UPDATE CASCADE;
ALTER TABLE GROUPRIGHTS ADD CONSTRAINT FK_GR_SECACT FOREIGN KEY (IDSECACT) REFERENCES SECACT (IDSECACT) ON DELETE CASCADE ON UPDATE CASCADE;
ALTER TABLE SECACT ADD CONSTRAINT FK_SECACT_SECOBJTYPE FOREIGN KEY (IDSECOBJTYPE) REFERENCES SECOBJTYPE (IDSECOBJTYPE) ON UPDATE CASCADE;
ALTER TABLE SECOBJ ADD CONSTRAINT FK_SECOBJ_SECOBJTYPE FOREIGN KEY (IDSECOBJTYPE) REFERENCES SECOBJTYPE (IDSECOBJTYPE) ON UPDATE CASCADE;
ALTER TABLE USERRIGHTS ADD CONSTRAINT FK_UR_USERS FOREIGN KEY (IDUSERS) REFERENCES USERS (IDUSERS) ON DELETE CASCADE ON UPDATE CASCADE;
ALTER TABLE USERRIGHTS ADD CONSTRAINT FK_UR_SECACT FOREIGN KEY (IDSECACT) REFERENCES SECACT (IDSECACT) ON DELETE CASCADE ON UPDATE CASCADE;
ALTER TABLE USERRIGHTS ADD CONSTRAINT FK_UR_SECOBJ FOREIGN KEY (IDSECOBJ) REFERENCES SECOBJ (IDSECOBJ) ON DELETE CASCADE ON UPDATE CASCADE;
ALTER TABLE USERS_GROUPS ADD CONSTRAINT FK_UG_USERS FOREIGN KEY (IDUSERS) REFERENCES USERS (IDUSERS) ON DELETE CASCADE ON UPDATE CASCADE;
ALTER TABLE USERS_GROUPS ADD CONSTRAINT FK_UG_USERGROUPS FOREIGN KEY (IDUSERGROUPS) REFERENCES USERGROUPS (IDUSERGROUPS) ON DELETE CASCADE ON UPDATE CASCADE;
ALTER TABLE ZONEDEV ADD CONSTRAINT FK_ZD_ZONES FOREIGN KEY (IDZONES) REFERENCES ZONES (IDZONES) ON DELETE CASCADE ON UPDATE CASCADE;
ALTER TABLE ZONEDEV ADD CONSTRAINT FK_ZD_DEVICES FOREIGN KEY (IDDEVICES) REFERENCES DEVICES (IDDEVICES) ON DELETE CASCADE ON UPDATE CASCADE;
ALTER TABLE ZONES ADD CONSTRAINT FK_ZONES_SECOBJ FOREIGN KEY (IDSECOBJ) REFERENCES SECOBJ (IDSECOBJ);

alter table EVENTS add constraint FK_EV_DEVICEUID foreign key (IDDEVICEUID) references DEVICEUID(IDDEVICEUID) on delete SET NULL on update CASCADE;

alter table EVENTS add constraint FK_EVENTS_EVENTTEXT foreign key (IDEVENTTEXT) references EVENTTEXT(IDEVENTTEXT) on update CASCADE;

alter table PARAMHISTORY add constraint FK_PARAMHISTORY foreign key (IDDEVICENODES) references DEVICENODES(IDDEVICENODES) on delete CASCADE on update CASCADE using index FK_PARAMHISTORY_IDDEVICENODES;

ALTER TABLE PARAMHISTORY ADD CONSTRAINT FK_PARAMHISTORY_IDPARAMNAMES FOREIGN KEY (IDPARAMNAMES) REFERENCES PARAMNAMES (IDPARAMNAMES) ON UPDATE CASCADE;

alter table IGNORE_LIST add constraint FK_IGNORE_LIST_IDDEVICES foreign key (IDDEVICES) references DEVICES(IDDEVICES) on delete CASCADE on update CASCADE;



alter table PARTITIONS ADD CONSTRAINT FK_PARTITIONS FOREIGN KEY (IDPARTITIONS) REFERENCES PARTITIONS (IDPARTITIONS);

ALTER TABLE PARTZONE ADD CONSTRAINT FK_PARTZONE_IDPARTITIONS FOREIGN KEY (IDPARTITIONS) REFERENCES PARTITIONS (IDPARTITIONS) ON DELETE CASCADE ON UPDATE CASCADE;
ALTER TABLE PARTZONE ADD CONSTRAINT FK_PARTZONE_IDZONESS FOREIGN KEY (IDZONES) REFERENCES ZONES (IDZONES) ON DELETE CASCADE ON UPDATE CASCADE;
ALTER TABLE PARTZONE ADD CONSTRAINT FK_ZONE_PART FOREIGN KEY (IDPARTZONE) REFERENCES PARTZONE (IDPARTZONE);

/******************************************************************************/
/***                           Check constraints                            ***/
/******************************************************************************/



/******************************************************************************/
/***                                Индексы                                 ***/
/******************************************************************************/

CREATE DESCENDING INDEX EVENTS_IDX3 ON EVENTS (SYSDT, DT, IDEVENTS);
CREATE INDEX IDX_EVENTS_IDTYPEVENTS ON EVENTS (IDTYPEEVENTS);

CREATE UNIQUE INDEX UNIQUE_1 ON DEVICES (IDDEVICENODES, IDVERSIONS);
CREATE INDEX DRIVERS_DRIVERID ON DRIVERS (DRIVERID);
CREATE INDEX SECOBJTYPE_NUM ON SECOBJTYPE (NUM);
CREATE INDEX U_NAME ON USERS (NAME);
CREATE INDEX PARAMHISTORY_PTIME ON PARAMHISTORY (PTIME);
CREATE DESCENDING INDEX PARAMHISTORY_PTIMED ON PARAMHISTORY (PTIME);
CREATE UNIQUE INDEX PARAMNAMES_NAME ON PARAMNAMES (PARAMNAME);

/******************************************************************************/
/***                                Триггеры                                ***/
/******************************************************************************/

SET TERM ^ ;

CREATE TRIGGER TREVENTACTIONS_BI_GENID FOR EVENTACTIONS
ACTIVE BEFORE INSERT POSITION 0
as
begin
  if (New.IDEventActions is Null) then
    New.IDEventActions = Gen_ID(gnEventActions, 1);
end

^

CREATE TRIGGER TRG_DEVICENODE_BI FOR DEVICENODES
ACTIVE BEFORE INSERT POSITION 0
as
begin
  if (New.IDDeviceNodes is Null) then
    New.IDDeviceNodes = Gen_ID(gnDeviceNodes, 1);
end

^

CREATE TRIGGER TRG_SECOBJTYPE_BI_GENID FOR SECOBJTYPE
ACTIVE BEFORE INSERT POSITION 0
as
begin
  if (New.IDSecOBJType is Null) then
    New.IDSecOBJType = Gen_ID(gnSecOBJType, 1);
end

^

CREATE TRIGGER TRGDEVICES_BI_GENID FOR DEVICES
ACTIVE BEFORE INSERT POSITION 0
as
begin
  if (New.IDDevices is Null) then
    New.IDDevices = Gen_ID(gnDevices, 1);
end

^

CREATE TRIGGER TRGDRIVERS_BI_GENID FOR DRIVERS
ACTIVE BEFORE INSERT POSITION 0
as
begin
  if (New.IDDrivers is Null) then
    New.IDDrivers = Gen_ID(gnDrivers, 1);
end

^

CREATE TRIGGER TRGEVENTS_BI_GENID FOR EVENTS
ACTIVE BEFORE INSERT POSITION 0
as
begin
  if (New.IDEvents is Null) then
    New.IDEvents = Gen_ID(gnEvents, 1);
end

^

CREATE TRIGGER TRGGROUPRIGHTS_AD_DELETE_UR FOR GROUPRIGHTS
ACTIVE AFTER DELETE POSITION 0
as
declare IDUsers Integer;
begin
  /* Выбираем всех пользователей группы */
  for select IDUsers from Users_Groups where IDUserGroups = OLD.idusergroups
    into :idusers do
  begin
    /* Если нет группы у пользователя, которая дает такое же право, то удаляем у пользователя удаляющее право */
    if (not Exists(select GR.idgrouprights from users_groups UG left join grouprights GR on GR.idusergroups = UG.idusergroups
      and GR.idsecobj = Old.idsecobj and GR.idsecact = old.idsecact
    where UG.IDUsers = :IDUsers and UG.IDUserGroups <> Old.idusergroups and GR.idgrouprights is not NULL)) then
      delete from userrights UR where UR.idsecact = OLD.idsecact and UR.idsecobj = OLD.idsecobj
        and (UR.idusers = :idusers) and UR.deleteflag = 1;
  end
end

^

CREATE TRIGGER TRGGROUPRIGHTS_BI_GENID FOR GROUPRIGHTS
ACTIVE BEFORE INSERT POSITION 0
as
begin
  if (New.IDGroupRights is Null) then
    New.IDGroupRights = Gen_ID(gnGroupRights, 1);
end

^

CREATE TRIGGER TRGSECACT_BI_GENID FOR SECACT
ACTIVE BEFORE INSERT POSITION 0
as
begin
  if (New.IDSecAct is Null) then
    New.IDSecAct = Gen_ID(gnSecAct, 1);
end


^

CREATE TRIGGER TRGSECOBJ_BI_GENID FOR SECOBJ
ACTIVE BEFORE INSERT POSITION 0
as
begin
  if (New.IDSecOBJ is Null) then
    New.IDSecOBJ = Gen_ID(gnSecOBJ, 1);
end

^

CREATE TRIGGER TRGUSERGROUPS_BI_GENID FOR USERGROUPS
ACTIVE BEFORE INSERT POSITION 0
as
begin
  if (New.IDUserGroups is Null) then
    New.IDUserGroups = Gen_ID(gnUserGroups, 1);
end

^

CREATE TRIGGER TRGUSERRIGTHS_BI_GENID FOR USERRIGHTS
ACTIVE BEFORE INSERT POSITION 0
as
begin
  if (New.IDUserRights is Null) then
    New.IDUserRights = Gen_ID(gnUserRights, 1);
end

^

CREATE TRIGGER TRGUSERS_BI_GENID FOR USERS
ACTIVE BEFORE INSERT POSITION 0
as
begin
  if (New.IDUsers is Null) then
    New.IDUsers = Gen_ID(gnUsers, 1);
end

^

CREATE TRIGGER TRGUSERS_GROUPS_BI_GENID FOR USERS_GROUPS
ACTIVE BEFORE INSERT POSITION 0
as
begin
  if (New.IDUsers_Groups is Null) then
    New.IDUsers_Groups = Gen_ID(gnUsers_Groups, 1);
end

^

CREATE TRIGGER TRGVERSIONS_BI_GENID FOR VERSIONS
ACTIVE BEFORE INSERT POSITION 0
as
begin
  if (New.IDVersions is Null) then
    New.IDVersions = Gen_ID(gnVersions, 1);
end

^

CREATE TRIGGER TRGZONEDEV_BI_GENID FOR ZONEDEV
ACTIVE BEFORE INSERT POSITION 0
as
begin
  if (New.IDZoneDev is Null) then
    New.IDZoneDev = Gen_ID(gnZoneDev, 1);
end

^

CREATE trigger trgpartzone_bi_genid for partzone
active before insert position 0
AS
begin
  if (New.IDPARTZONE is Null) then
    New.IDPARTZONE = Gen_ID(gnPartZone, 1);
end

^

CREATE trigger trgpartitions_bi_genid for partitions
active before insert position 0
AS
begin
  if (New.IDPARTITIONS is Null) then
    New.IDPARTITIONS = Gen_ID(gnPartitions, 1);
end

^

CREATE TRIGGER TRGZONES_AD_SECOBJ FOR ZONES
ACTIVE AFTER DELETE POSITION 0
as
begin
  delete from SecObj where IDSecObj = OLD.idsecobj;
end

^

CREATE TRIGGER TRGZONES_BI_GENID FOR ZONES
ACTIVE BEFORE INSERT POSITION 0
as
begin
  if (New.IDZones is Null) then
    New.IDZones = Gen_ID(gnZones, 1);
end

^

CREATE TRIGGER DEVICEUID_BI_GENID FOR DEVICEUID
ACTIVE BEFORE INSERT POSITION 0
as
begin
  if (New.Iddeviceuid is Null) then
    New.Iddeviceuid = Gen_ID(Gndeviceuid, 1);
end

^

CREATE TRIGGER TREVENTTEXT_BI_GENID FOR EventText
ACTIVE BEFORE INSERT POSITION 0
as
begin
  if (New.IDEventText is Null) then
    New.IDEventText = Gen_ID(gnEventText, 1);
end

^

CREATE TRIGGER SETTINGS_BI_GENID FOR SETTINGS
ACTIVE BEFORE INSERT POSITION 0
as
begin
  if (New.IDSettings is Null) then
    New.IDSettings = Gen_ID(gnSettings, 1);
end

^
CREATE TRIGGER PARAMHISTORY_BI FOR PARAMHISTORY
ACTIVE BEFORE INSERT POSITION 0
AS
BEGIN
  IF (NEW.IDPARAMHISTORY IS NULL) THEN
    NEW.IDPARAMHISTORY = GEN_ID(GNPARAMHISTORY,1);
END
^

CREATE TRIGGER PARAMNAMES_BI FOR PARAMNAMES
ACTIVE BEFORE INSERT POSITION 0
AS
BEGIN
  IF (NEW.IDPARAMNAMES IS NULL) THEN
    NEW.IDPARAMNAMES = GEN_ID(GNPARAMNAMES,1);
END
^

CREATE TRIGGER IGNORE_LIST_BI FOR IGNORE_LIST
ACTIVE BEFORE INSERT POSITION 0
AS
BEGIN
  IF (NEW.IDIGNORE_LIST IS NULL) THEN
    NEW.IDIGNORE_LIST = GEN_ID(GNIGNORE_LIST,1);
END
^

SET TERM ; ^

/******************************************************************************/
/***                               Процедуры                                ***/
/******************************************************************************/

SET TERM ^ ;

ALTER PROCEDURE DEVICES_DELETE(
    IDDEVICENODES INTEGER,
    NEWIDVERSIONS INTEGER,
    RECURSIVE INTEGER)
AS
declare DeleteIDVersions Integer;
declare IDVERSIONS Integer;
declare IDDeviceNodesChild Integer ;
begin
  if (NewIDVersions is NULL) then
    select IDVersions from version_create into :NewIDVersions;

  select Max(IDVersions) from Devices where IDDeviceNodes = :iddevicenodes
      and IDVersions < :NewIDVersions
    into :IDVersions;
  select DeleteIDVersions from devices
    where IDDeviceNodes = :iddevicenodes and IDVersions = :IDVersions
    into :DeleteIDVersions;
  if (:DeleteIDVersions is not NULL) then
    exception Error 'Устройство уже удалено';

  update Devices set DeleteIDVersions = :NewIDVersions where
    IDDeviceNodes = :iddevicenodes and IDVersions = :IDVersions;

  if (Recursive = 1) then
    for select Distinct IdDeviceNodes from devices where IDDeviceNodesParent = :IDDeviceNodes
      into :IDDeviceNodesChild do
        execute procedure Devices_Delete(:IDDeviceNodesChild, :NewIDVersions, 1);
end^

ALTER PROCEDURE DEVICES_INSERT(
    IN_IDDEVICENODES INTEGER,
    DRIVERID VARCHAR(64),
    ADDRESS INTEGER,
    PLACEMENT VARCHAR(128) CHARACTER SET WIN1251,
    IDDEVICENODESPARENT INTEGER,
    NEWIDVERSIONS INTEGER,
    PROPERTIES BLOB SUB_TYPE 0 SEGMENT SIZE 80,
    DISABLED INTEGER,
    DEVPARAMS BLOB SUB_TYPE 0 SEGMENT SIZE 80)
RETURNS (
    IDDEVICENODES INTEGER,
    IDDEVICES INTEGER)
AS
declare variable IDDrivers Integer;
declare variable IDVersions Integer;
BEGIN
  IDDeviceNodes = IN_IDDeviceNodes;
  if (IDDeviceNodes is NULL) then
  begin
    if (IDDeviceNodesParent is NULL) then
    begin
      select Max(IDVersions) from Devices where IDDeviceNodesParent is NULL
        and DeleteIDVersions is NULL
      into :IDVersions;

      select IDDeviceNodes from Devices where IDDeviceNodesParent is NULL
        and IDVersions = :IDVersions
      into :IDDeviceNodes;
    end

    if (IDDeviceNodes is NULL) then
    begin
      IDDeviceNodes = GEN_ID(gndevicenodes, 1);
      insert into DeviceNodes(IDDeviceNodes) values(:IDDeviceNodes);
    end
  end

  select IDDrivers from drivers where DriverID = :driverid into :idDrivers;
  if (IDDrivers is NULL) then
  begin
    IDDrivers = Gen_ID(gnDrivers, 1);
    insert into Drivers(IDDrivers, DriverID) values(:IDDrivers, :DriverID);
  end

  if (NewIDVersions is NULL) then
    select IDVersions from version_create into :NewIDVersions;

  IDDevices = Gen_ID(gndevices, 1);

  INSERT INTO
   DEVICES (IDDevices, IDDRIVERS, ADDRESS, PLACEMENT, IDDeviceNodesParent, IDDeviceNodes, IDVersions, Properties, Disabled, DevParams)
    VALUES (:IDDevices, :IDDRIVERS, :ADDRESS, :PLACEMENT, :IDDeviceNodesParent, :iddevicenodes, :NEWIDVERSIONS, :Properties, :Disabled, :DevParams);

  suspend;
END^

ALTER procedure DEVICES_SELECTACTUAL (
    IDDEVICENODESPARENT integer,
    RECURSIVE integer)
returns (
    IDDEVICES integer,
    IDDEVICENODES integer,
    IDVERSIONS integer)
AS
declare variable CurNodeID Integer;
declare variable DeleteIDVersions Integer;
begin
  /* Когда в FireBird введут оператор == нужно будет убрать две ветки */
  if (:iddevicenodesparent is NULL) then
  for select D.iddevicenodes, Max(D.idversions) from
    devices D
  where D.iddevicenodesparent is NULL
  group by D.iddevicenodes
    into :iddevicenodes, :idversions do
  begin
    CurNodeID = iddevicenodes;
    select IDDevices, deleteidversions from Devices where iddevicenodes = :CurNodeid and IDVersions = :idversions
      into :iddevices, :DeleteIDVersions;
    if (DeleteIDVersions is NULL) then
    begin
      suspend;
      if (Recursive = 1) then
        if (Exists(select IDDevices from devices DC where DC.iddevicenodesparent = :CurNodeID)) then
        begin
          for select IDDevices, IDDeviceNodes, IDVersions from DEVICES_SELECTACTUAL(:CurNodeID, :recursive)
            into :iddevices, :IDDeviceNodes, :idversions do
              suspend;
        end
    end
  end else
  for select D.iddevicenodes, Max(D.idversions) from
    devices D
  where D.iddevicenodesparent = :iddevicenodesparent
  group by D.iddevicenodes
    into :iddevicenodes, :idversions do
  begin
    CurNodeID = iddevicenodes;
    for select IDDevices from Devices where iddevicenodes = :CurNodeid and IDVersions = :idversions and DeleteIDVersions is NULL
    into :iddevices do
    begin
      suspend;
      if (Recursive = 1) then
        if (Exists(select IDDevices from devices DC where DC.iddevicenodesparent = :CurNodeID)) then
        begin
          for select IDDevices, IDDeviceNodes, IDVersions from DEVICES_SELECTACTUAL(:CurNodeID, :recursive)
            into :iddevices, :IDDeviceNodes, :idversions do
              suspend;
        end
    end
  end

end^


ALTER PROCEDURE VERSION_CREATE
RETURNS (
    IDVERSIONS INTEGER)
AS
begin
  IDVersions = Gen_ID(gnVersions, 1);
  insert into Versions(IDVersions, DT) values(:IDVersions, Cast('TODAY' as TimeStamp));
  suspend;
end^


ALTER PROCEDURE GETUSERRIGHTS(
    NAME VARCHAR(64) CHARACTER SET WIN1251)
RETURNS (
    IDSECACT INTEGER,
    IDSECOBJ INTEGER)
AS
DECLARE VARIABLE IDUSER INTEGER;
BEGIN
  select IDUsers
    from Users
    where Name = :name
    into :IDUSER;

  For
    select R.idsecact, R.IDSecobj
      from Users_Groups G, GroupRights R, SecAct SA
      where (G.idusers = :IDUSER) and
            (G.idusergroups = R.IDUserGroups)
    union
    select U.idsecact, U.IDSecobj
        from UserRights U, SecAct SA
        where (U.IDUsers = :IDUSER) and
              (Coalesce(U.DeleteFlag, 0) = 0)
    into :idsecact, :IDsecobj
  do
  begin
    if (not Exists(Select U.IduserRights from UserRights U where
      U.idsecact = :idsecact and U.IDSecobj = :IDsecobj
      and U.idusers = :IDUser and Coalesce(U.deleteflag, 0) = 1)) then
        Suspend;
  end
END^


ALTER PROCEDURE ZONE_INSERT(
    IDZONES INTEGER,
    NAME VARCHAR(64) CHARACTER SET WIN1251,
    DESCRIPTION VARCHAR(256) CHARACTER SET WIN1251,
    ZONEID INTEGER,
    PROPERTIES BLOB SUB_TYPE 0 SEGMENT SIZE 80)
RETURNS (
    ID_ZONES INTEGER,
    ID_SECOBJ INTEGER)
AS
declare variable IDSecObjtype Integer;
BEGIN
  select z.IDZones, z.idsecobj from Zones z where z.IDZones = :IDZones into :ID_ZONES, :id_secobj;
  if (ID_ZONES is Null) then
  begin
    select IDSecObjtype from secobjtype where Num = 2 into :IDSecObjtype;

    ID_SecObj = Gen_ID(gnsecobj, 1);
    if (IDSecObjtype is not NULL) then
      insert into SecObj(IDSecObj, Name, IDSecObjtype) values(:id_secobj, :name, :idsecobjtype);

    if (IdZones is Null) then
      ID_ZONES = Gen_ID(GnZones, 1); else
      ID_ZONES = IdZones;
    insert into Zones
      (IdZones, Name, Description, IDSecObj, ZoneID, Properties)
    values
      (:ID_ZONES, :Name, :Description, :ID_SecObj, :ZoneID, :Properties);
  end
  else
  begin
    Update Zones z set
      z.Name = :Name,
      z.Description = :Description,
      z.zoneid = :ZoneID,
      z.Properties = :Properties
    where z.IDZones = :ID_ZONES;

    Update SecObj so set
      so.name = :Name
    where so.idsecobj = :id_secobj;
  end
  suspend;
END^


ALTER PROCEDURE ZONEDEV_INSERT(
    IDZONES INTEGER,
    IDDEVICES INTEGER)
RETURNS (
    ID_ZONEDEV INTEGER)
AS
begin
  select zd.IdZoneDev from ZoneDev zd where zd.idzones = :idzones and zd.iddevices = :iddevices
  into :Id_ZoneDev;

  if (Id_ZoneDev is Null) then
  begin
    Id_ZoneDev= Gen_ID(GnZones, 1);
    insert into ZoneDev
      (IdZoneDev, IdZones, IdDevices)
    values (:Id_ZoneDev, :IdZones, :IdDevices);
  end
  suspend;
end^

create procedure PARTZONE_INSERT (
    IDPART integer,
    IDZONE integer)
returns (
    ID_PARTZONE integer)
as
begin
  select pz.IdPartZone from PARTZONE pz where pz.IDPartitions = :IDPART and pz.IDZones = :IDZONE
  into :ID_PARTZONE;

  if (ID_PARTZONE is Null) then
  begin
    ID_PARTZONE= Gen_ID(GNPARTZONE, 1);
    insert into PARTZONE
      (IdPartZone, IdPartitions, IdZones)
    values (:ID_PARTZONE, :IDPART, :IDZONE);
  end
  suspend;
end^

create procedure PARTITION_INSERT (
    IDPARTITIONS integer,
    PARTTYPE varchar(64) character set WIN1251,
    NUMBER integer,
    NAME varchar(64) character set WIN1251,
    PROPERTIES blob sub_type 1 segment size 80 character set WIN1251,
    DESCRIPTION varchar(256) character set WIN1251)
returns (
    ID_PARTITIONS integer)
as
BEGIN
  select p.IDPartitions from Partitions p where p.IDPartitions = :IDPARTITIONS into :ID_PARTITIONS;
  if (ID_PARTITIONS is Null) then
  begin
  if (IDPartitions is Null) then
    ID_PARTITIONS = Gen_ID(GnPartitions, 1); else
    ID_PARTITIONS = IDPartitions;
  insert into Partitions
    (IDPartitions, PartType, Number, Name, Description, Properties)
  values
    (:ID_PARTITIONS, :PartType, :Number, :Name, :Description, :Properties);
  end
  else
  begin
     Update Partitions p set
      p.PartType = :PartType,
      p.Number = :Number,
      p.Name = :Name,
      p.Description = :Description,
      p.Properties = :Properties
    where p.IDPartitions = :ID_PARTITIONS;
  end
  suspend;
END^

ALTER PROCEDURE USERS_INSERT(
    IDUSERS INTEGER,
    NAME VARCHAR(64) CHARACTER SET WIN1251,
    PASSWRD VARCHAR(64),
    FULLNAME VARCHAR(64) CHARACTER SET WIN1251,
    EXTSecurity BLOB SUB_TYPE 1 SEGMENT SIZE 80 CHARACTER SET WIN1251)
RETURNS (
    ID_USERS INTEGER)
AS
BEGIN
  select U.idusers from users U where U.idusers = :IDUSERS into :ID_USERS;
  if (ID_USERS is Null) then
  begin
    if (IDUSERS is Null) then
      ID_USERS = Gen_ID(gnusers, 1); else
      ID_userS = IDuserS;
    insert into users
      (IDuserS, Name, passwrd, FullName, ExtSecurity)
    values
      (:ID_userS, :Name, :passwrd, :fullname, :ExtSecurity);
  end
  else
  begin
    Update users u set
      u.Name = :Name,
      u.passwrd = :passwrd,
      u.fullname = :fullname,
      u.ExtSecurity = :ExtSecurity
    where u.IDusers = :ID_users;
  end
  suspend;
END^


ALTER PROCEDURE USERGROUPS_INSERT(
    IDUSERGROUPS INTEGER,
    NAME VARCHAR(64) CHARACTER SET WIN1251,
    EXTSecurity BLOB SUB_TYPE 1 SEGMENT SIZE 80 CHARACTER SET WIN1251)
RETURNS (
    ID_USERGROUPS INTEGER)
AS
BEGIN
  select UG.idusergroups from usergroups UG where UG.idusergroups = :IDUSERGROUPS into :ID_USERGROUPS;
  if (ID_USERGROUPS is Null) then
  begin
    if (IDUSERGROUPS is Null) then
      ID_USERGROUPS = Gen_ID(gnusergroups, 1); else
      ID_USERGROUPS = IDUSERGROUPS;
    insert into usergroups
      (IDUSERGROUPS, Name, ExtSecurity)
    values
      (:ID_USERGROUPS, :Name, :ExtSecurity);
  end
  else
  begin
    Update usergroups ug set
      ug.Name = :Name,
      ug.ExtSecurity = :ExtSecurity
	
    where ug.IDusergroups = :ID_usergroups;
  end
  suspend;
END^


ALTER PROCEDURE USERS_GROUPS_INSERT(
    IDUSERS INTEGER,
    IDUSERGROUPS INTEGER)
RETURNS (
    IDUSERS_GROUPS INTEGER)
AS
begin
  select ug.idusers_groups from users_groups ug where ug.idusers = :idusers and ug.idusergroups = :idusergroups
  into :idusers_groups;

  if (idusers_groups is Null) then
  begin
    idusers_groups = Gen_ID(gnusers_groups, 1);
    insert into users_groups
      (idusers_groups, Idusers, IDUserGroups)
    values (:idusers_groups, :idusers, :idusergroups);
  end
  suspend;
end^


ALTER PROCEDURE USERS_DELETE(
    IDUSERS INTEGER)
AS
begin
  if (not Exists(select IDEventActions from eventactions where IDUsers = :IDUsers)) then
    delete from Users where IDUsers = :IDUsers; else
    Update Users set Deleted = 1 where IDUsers = :IDUsers;

end^



CREATE PROCEDURE FAST_SELECTEVENTS(
    NREC INTEGER,
    D1 DOUBLE PRECISION,
    D2 DOUBLE PRECISION,
    USESYSDATE INTEGER)
RETURNS (
    IDEVENTS INTEGER)
AS
begin
  if (UseSysDate <> 1) then
  for select Ev.idevents from events Ev
  where Ev.Dt between :D1 and :D2
  order by Ev.SysDT desc, ev.dt desc, Ev.IDEvents desc
  into :IDEvents do
  begin
    suspend;
    NRec = NRec - 1;
    if (NRec <= 0) then
      break;
  end else
  for select Ev.idevents from events Ev
  where Ev.SysDt between :D1 and :D2
  order by Ev.SysDT desc, ev.dt desc, Ev.IDEvents desc
  into :IDEvents do
  begin
    suspend;
    NRec = NRec - 1;
    if (NRec <= 0) then
      break;
  end
end^


create procedure DeviceUID_Insert (
  UID VarChar(64))
returns (
  IDDeviceUID Integer)
as
begin
  select IDDeviceUID from Deviceuid where UID = :UID into :Iddeviceuid;
  if (IDDeviceUID is NULL) then
  begin
    IDDeviceUID = Gen_ID(Gndeviceuid, 1);
    insert into Deviceuid(IDDeviceUID, UID) values(:Iddeviceuid, :Uid);
  end
  suspend;
end^

CREATE PROCEDURE GETIDEVENTTEXT(
    EVENTTEXT VARCHAR(256) CHARACTER SET WIN1251)
RETURNS (
    IDEVENTTEXT INTEGER)
AS
begin
  Select IDEventText from EventText where EventText = :Eventtext into :IdeventText;
  if (IDEventText is NULL) then
  begin
    IDEventText = Gen_ID(Gneventtext, 1);
    insert into EventText(IDEventText, Eventtext) values(:Ideventtext, :Eventtext);
  end
  suspend;
end^

CREATE PROCEDURE GETIDPARAMNAMES(
    PARAMNAME VARCHAR(32) CHARACTER SET WIN1251)
RETURNS (
    IDPARAMNAMES INTEGER)
AS
begin
  Select IDParamNames from paramnames where paramName = :ParamName into :idparamnames;
  if (idparamnames is NULL) then
  begin
    idparamnames = Gen_ID(gnparamnames, 1);
    insert into paramnames(idparamnames, paramname) values(:idparamnames, :paramname);
  end
  suspend;
end^

CREATE PROCEDURE GETMINMAXTIME(
    IDDEVICENODES INTEGER,
    IDPARAMNAMES INTEGER)
RETURNS (
    MINTIME DOUBLE PRECISION,
    MAXTIME DOUBLE PRECISION)
AS
begin
  select Min(PTime) from paramhistory where idDeviceNodes = :IDDeviceNodes and
  IDParamNames = :IDParamNames
  PLAN (paramhistory INDEX (ParamHistory_PTime, FK_ParamHistory_IDDeviceNodes, FK_ParamHistory_IDParamNames))
  into :MinTime;

  select Max(PTime) from paramhistory where idDeviceNodes = :IDDeviceNodes and
  IDParamNames = :IDParamNames
  PLAN (paramhistory INDEX (ParamHistory_PTimeD, FK_ParamHistory_IDDeviceNodes, FK_ParamHistory_IDParamNames))
  into :MaxTime;

  suspend;
end^

CREATE PROCEDURE GETTIMEDIAP(
    IDDEVICENodes INTEGER,
    STARTTIME DOUBLE PRECISION,
    ENDTIME DOUBLE PRECISION,
    IDFLTRPARAMNAMES INTEGER,
    INDCOUNT INTEGER)
RETURNS (
    IDPARAMNAMES INTEGER,
    MINVALUE DOUBLE PRECISION,
    MAXVALUE DOUBLE PRECISION,
    AVGVALUE DOUBLE PRECISION,
    RTIME DOUBLE PRECISION)
AS
  declare variable StepTime Double Precision;
  declare variable sTime Double Precision;
  declare variable eTime Double Precision;
  declare variable CurTime Double Precision;
  declare variable CurValue Double Precision;
  declare variable cnt Integer;
begin
  IDPARAMNames = IDFLTRParamNames;
  IndCount = IndCount - 2;
  StepTime = (EndTime - StartTime) / IndCount;
  sTime = StartTime;
  eTime = StartTime + StepTime;
  RTime = (eTime + sTime) / 2;
  cnt = 0;

  /* Левая точка */
  for select first 1 i.paramvalue, i.Ptime from paramhistory i
  where i.iddeviceNodes = :iddeviceNodes and
        i.idparamnames = :idfltrparamnames and
        i.Ptime < :StartTime
  order by i.PTime desc
  into :CurValue, :CurTime do
  begin
    RTIME = StartTime;
    MinValue = curvalue;
    maxvalue = curvalue;
    avgvalue = NULL;
    suspend;
  end

  RTime = NULL;
  MinValue = NULL;
  MaxValue = NULL;
  AvgValue = NULL;
  CurTime = NULL;


  for select i.paramvalue, i.Ptime from paramhistory i
  where i.iddeviceNodes = :iddeviceNodes and
        i.idparamnames = :idfltrparamnames and
        i.Ptime between :StartTime and :EndTime
  order by i.PTime
  into :CurValue, :CurTime do
  begin
    cnt = cnt + 1;
    if (CurTime between sTime and eTime) then
    begin
      if (MinValue is Null) then MinValue = CurValue; else
        if (CurValue < MinValue) then MinValue = CurValue;
      if (CurValue > MaxValue) then MaxValue = CurValue; else
        if (MaxValue is Null) then MaxValue = CurValue;
      if (cnt = 1) then
        AVGValue = CurValue; else
        AVGValue = AVGValue / cnt  * (cnt - 1) + CurValue / cnt;
    end
    if (CurTime > eTime) then
    begin
      if (MinValue is Not Null) then suspend;
      cnt = 1;
      AvgValue = CurValue;
      if (CurTime between eTime and eTime + StepTime) then
      begin
        /* Проверили, что следующий отсчет в следующем шаге*/
        sTime = sTime + StepTime;
        eTime = eTime + StepTime;
        RTime = (eTime + sTime) / 2;
        MinValue = CurValue;
        MaxValue = CurValue;
        CurTime = Null;
      end else
      begin
        /* Попали в разрыв данных */
        while (not CurTime between sTime and sTime + StepTime) do
          stime = stime + StepTime;

        etime = stime + StepTime;
        RTime = (eTime + sTime) / 2;
        MinValue = CurValue;
        MaxValue = CurValue;
      end
    end
  end
  if (CurTime is Not Null) then suspend;

  /* Дополнительно выбираем точки лежащие за диапазоном */

  /* Правая точка */
  for select first 1 i.paramvalue, i.Ptime from paramhistory i
  where i.iddeviceNodes = :iddeviceNodes and
        i.idparamnames = :idfltrparamnames and
        i.Ptime > :EndTime
  order by i.PTime
  into :CurValue, :CurTime do
  begin
    RTIME = EndTime;
    MaxValue = curvalue;
    Minvalue = curvalue;
    avgvalue = NULL;
    suspend;
  end
end
^

SET TERM ; ^



insert into usergroups(IDUserGroups, Name) values (1, 'Администраторы');
insert into usergroups(IDUserGroups, Name) values (2, 'Операторы');
insert into usergroups(IDUserGroups, Name) values (3, 'Инсталляторы');
insert into usergroups(IDUserGroups, Name) values (4, 'Операторы (лайт)');
set generator gnusergroups to 5;

insert into users(IDUsers, Name, FullName, Passwrd, ISBUILTIN) values(1, 'adm', 'Администратор',      'D41D8CD98F00B204E9800998ECF8427E' /* пусто */, 1);
insert into users(IDUsers, Name, FullName, Passwrd) values(2, 'oper', 'Дежурный',          'D41D8CD98F00B204E9800998ECF8427E' /* пусто */);
insert into users(IDUsers, Name, FullName, Passwrd) values(3, 'inst', 'Инсталлятор',       'D41D8CD98F00B204E9800998ECF8427E' /* пусто */);
insert into users(IDUsers, Name, FullName, Passwrd) values(4, 'operl', 'Дежурный (лайт)',  'D41D8CD98F00B204E9800998ECF8427E' /* пусто */);
set generator gnusers to 5;

insert into users_groups(IDUsers, IDUserGroups) values (1, 1);
insert into users_groups(IDUsers, IDUserGroups) values (2, 2);
insert into users_groups(IDUsers, IDUserGroups) values (3, 3);
insert into users_groups(IDUsers, IDUserGroups) values (4, 4);
set generator gnusergroups to 5;

insert into SecObjType(IDSecObjtype, Name, NUM) values (1, 'Функции программы', 1);
insert into SecObjType(IDSecObjtype, Name, NUM) values (2, 'Зоны', 2);
set generator gnsecobjtype to 3;

insert into SecObj(IDSecObj, Name, IDSecObjtype) values(1, 'Функции программы', 1);
set generator gnsecobj to 2;

/* Оперативная задача */
insert into SecAct(IDSecAct, Name, Num, IDSecObjtype) values(1, 'ОЗ: Вход', 1, 1);
insert into SecAct(IDSecAct, Name, Num, IDSecObjtype) values(2, 'ОЗ: Выход', 2, 1);
insert into SecAct(IDSecAct, Name, Num, IDSecObjtype) values (3, 'ОЗ: Обработка тревог', 3, 1);

/* Пропускаем право прошивки неверного конфига */
/* insert into SecAct(IDSecAct, Name, Num, IDSecObjtype) values (10, 'АДМ: Запись конфигурации не прошедшей проверку ', 10, 1); */

insert into SecAct(IDSecAct, Name, Num, IDSecObjtype) values (11, 'ОЗ: Дополнительные режимы', 11, 1);
insert into SecAct(IDSecAct, Name, Num, IDSecObjtype) values (12, 'ОЗ: Управление показом планов', 12, 1);
insert into SecAct(IDSecAct, Name, Num, IDSecObjtype) values (13, 'ОЗ: Выход без пароля', 13, 1);
insert into SecAct(IDSecAct, Name, Num, IDSecObjtype) values (14, 'ОЗ: Расширенное редактирование списка обхода', 14, 1);
insert into SecAct(IDSecAct, Name, Num, IDSecObjtype) values (15, 'ОЗ: Удаление из списка обхода', 15, 1);
insert into SecAct(IDSecAct, Name, Num, IDSecObjtype) values (16, 'ОЗ: Добавление в список обхода', 16, 1);
insert into SecAct(IDSecAct, Name, Num, IDSecObjtype) values (17, 'ОЗ: Не требуется подтверждение тревог', 17, 1);
insert into SecAct(IDSecAct, Name, Num, IDSecObjtype) values (18, 'ОЗ: Изменение размеров и положения окон', 18, 1);

/* Администратор */
insert into SecAct(IDSecAct, Name, Num, IDSecObjtype) values (4, 'АДМ: Очистка журнала', 4, 1);
insert into SecAct(IDSecAct, Name, Num, IDSecObjtype) values (5, 'АДМ: Просмотр конфигурации', 5, 1);
insert into SecAct(IDSecAct, Name, Num, IDSecObjtype) values (6, 'АДМ: Изменение конфигурации в БД', 6, 1);
insert into SecAct(IDSecAct, Name, Num, IDSecObjtype) values (7, 'АДМ: Изменение конфигурации в устройствах', 7, 1);
insert into SecAct(IDSecAct, Name, Num, IDSecObjtype) values (8, 'АДМ: Изменение ПО в устройствах', 8, 1);
insert into SecAct(IDSecAct, Name, Num, IDSecObjtype) values (9, 'АДМ: Управление правами пользователей', 9, 1);

set generator gnsecact to 18;

/* Администраторы */
insert into grouprights(IDUserGroups, IDSecObj, IDSecAct)
  select 1, so.idsecobj, sa.idsecact
    from SecObj so left join SecAct sa on sa.idsecobjtype = so.idsecobjtype
  where IDSecAct is not NULL;


/* Операторы */
insert into grouprights(IDUserGroups, IDSecObj, IDSecAct) values(2, 1, 1);
insert into grouprights(IDUserGroups, IDSecObj, IDSecAct) values(2, 1, 2);
insert into grouprights(IDUserGroups, IDSecObj, IDSecAct) values(2, 1, 3);
insert into grouprights(IDUserGroups, IDSecObj, IDSecAct) values(2, 1, 11);
insert into grouprights(IDUserGroups, IDSecObj, IDSecAct) values(2, 1, 12);
insert into grouprights(IDUserGroups, IDSecObj, IDSecAct) values(2, 1, 15);
insert into grouprights(IDUserGroups, IDSecObj, IDSecAct) values(2, 1, 16);
insert into grouprights(IDUserGroups, IDSecObj, IDSecAct) values(2, 1, 18);

/* Инсталляторы */
insert into grouprights(IDUserGroups, IDSecObj, IDSecAct) values(3, 1, 5);
insert into grouprights(IDUserGroups, IDSecObj, IDSecAct) values(3, 1, 6);
insert into grouprights(IDUserGroups, IDSecObj, IDSecAct) values(3, 1, 7);
insert into grouprights(IDUserGroups, IDSecObj, IDSecAct) values(3, 1, 8);

insert into grouprights(IDUserGroups, IDSecObj, IDSecAct) values(3, 1, 1);
insert into grouprights(IDUserGroups, IDSecObj, IDSecAct) values(3, 1, 2);
insert into grouprights(IDUserGroups, IDSecObj, IDSecAct) values(3, 1, 3);
insert into grouprights(IDUserGroups, IDSecObj, IDSecAct) values(3, 1, 4);

insert into grouprights(IDUserGroups, IDSecObj, IDSecAct) values(3, 1, 11);
insert into grouprights(IDUserGroups, IDSecObj, IDSecAct) values(3, 1, 12);
insert into grouprights(IDUserGroups, IDSecObj, IDSecAct) values(3, 1, 13);
insert into grouprights(IDUserGroups, IDSecObj, IDSecAct) values(3, 1, 14);
insert into grouprights(IDUserGroups, IDSecObj, IDSecAct) values(3, 1, 15);
insert into grouprights(IDUserGroups, IDSecObj, IDSecAct) values(3, 1, 16);
insert into grouprights(IDUserGroups, IDSecObj, IDSecAct) values(3, 1, 17);
insert into grouprights(IDUserGroups, IDSecObj, IDSecAct) values(2, 1, 18);

/* Операторы  (лайт) */
insert into grouprights(IDUserGroups, IDSecObj, IDSecAct) values(4, 1, 1);
insert into grouprights(IDUserGroups, IDSecObj, IDSecAct) values(4, 1, 3);

commit work;

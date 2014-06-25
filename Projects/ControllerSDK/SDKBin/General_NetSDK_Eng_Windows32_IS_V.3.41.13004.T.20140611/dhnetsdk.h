
#ifndef DHNETSDK_H
#define DHNETSDK_H


#if (defined(WIN32) || defined(WIN64))
#include <windows.h>
#ifdef NETSDK_EXPORTS

#ifndef CLIENT_API
#define CLIENT_API  __declspec(dllexport)
#endif

#else

#ifndef CLIENT_API
#define CLIENT_API  __declspec(dllimport)
#endif

#endif

#define CALLBACK __stdcall
#define CALL_METHOD  __stdcall  //__cdecl

#define INT64    __int64

#ifndef LLONG
#ifdef WIN64
#define LLONG INT64
#else 
#define LLONG LONG
#endif
#endif

#ifndef LDWORD
#ifdef WIN64
#define LDWORD INT64
#else 
#define LDWORD DWORD
#endif
#endif

#else    //non-windows

#define CLIENT_API    extern "C"
#define CALL_METHOD 
#define CALLBACK

#ifndef INTERNAL_COMPILE
#define RELEASE_HEADER 
#endif
 
#ifdef RELEASE_HEADER

#define WORD        unsigned short
#define DWORD       unsigned int
#define LONG        int
#define LPDWORD     DWORD*
#ifdef __APPLE__
#include "objc/objc.h"
#else
#define BOOL        int
#endif
#define TRUE        1
#define FALSE       0
#define BYTE        unsigned char
#define UINT        unsigned int
#define HDC         void*
#define HWND        void*
#define LPVOID      void*
#define NULL        0
#define LLONG       long
#define INT64       long long
#define LDWORD      long 

#ifndef MAX_PATH
#define MAX_PATH    260
#endif

typedef struct  tagRECT
{
    LONG left;
    LONG top;
    LONG right;
    LONG bottom;
} RECT;

#else	//Internal translation
#include "../Platform/osIndependent.h"
#endif

#endif
#include "dhassistant.h"

#ifndef LDWORD
#if (defined(WIN32) || defined(WIN64))
#ifdef WIN64
#define LDWORD __int64
#else //WIN32 
#define LDWORD DWORD
#endif
#else    //linux
#define LDWORD long 
#endif
#endif

#ifdef __cplusplus
extern "C" {
#endif

/************************************************************************
 ** Constant Definition
 ***********************************************************************/
#define DH_SERIALNO_LEN             48          // Device SN string length
#define DH_MAX_DISKNUM 				256			// Max HDD number
#define DH_MAX_SDCARDNUM			32			// Max SD card number
#define DH_MAX_BURNING_DEV_NUM		32			// Max buner amount
#define DH_BURNING_DEV_NAMELEN		32			// Burner name max length 
#define DH_MAX_LINK 				6
#define DH_MAX_CHANNUM 				16			// Max channel amount
#define DH_MAX_DECODER_CHANNUM		64			// decoder device max channel number
#define DH_MAX_ALARMIN 				128			// Max alarm input amount 
#define DH_MAX_ALARMOUT 			64			// Max alarm output amount 
#define DH_MAX_RIGHT_NUM			100			// Max user right amount
#define DH_MAX_GROUP_NUM			20			// Max user group amount
#define DH_MAX_USER_NUM				200			// Max user account amount
#define DH_RIGHT_NAME_LENGTH		32			// Right name length
#define DH_USER_NAME_LENGTH			8			// User name length 
#define DH_USER_PSW_LENGTH			8			// User password length 
#define DH_MEMO_LENGTH				32			// Note length 
#define DH_MAX_STRING_LEN			128
#define MAX_STRING_LINE_LEN			6			// Max six rows
#define MAX_PER_STRING_LEN			20			// Line max length 
#define DH_MAX_MAIL_NAME_LEN		64			// The user name length the new mail structure supported
#define DH_MAX_MAIL_PSW_LEN			64			// Password length the new mail structhre supported
#define DH_SPEEDLIMIT_TYPE_LEN      32          // speed limit string max length
#define DH_VEHICLE_TYPE_LEN         32          // vehicle info type max length
#define DH_VEHICLE_INFO_LEN         32          // vehicle info string max length
#define DH_VEHICLE_DRIVERNO_LEN     32          // Driver no string max length=
#define DH_MAX_CROSSING_NUM         128         // Max supported crossing num 
#define DH_MAX_CROSSING_ID          32          // Max crossing ID length 
#define DH_MAX_CARD_INFO_LEN		256			// max card info length
#define DH_MAX_CHANNUM_EX           32          // extended max channel number
#define DH_MAX_SAERCH_IP_NUM        256         // max number of IP for saerch
#define DH_MAX_HARDDISK_TYPE_LEN	32			// hard disk type maximum length
#define DH_MAX_HARDDISK_SERIAL_LEN	32			// hard disk serial maximum length
#define DH_MAX_SIM_LEN				16			// value of SIM maximum length
#define DH_MAX_SIM_NUM				10			// pick up SIM maximum number
#define DH_MAX_VERSION_LEN			32			// version maximum length
#define DH_MAX_MDN_LEN				36			// value of MDN maximum length
#define DH_MAX_NETINTERFACE_NUM     64          // support the card number
#define DH_EVENT_NAME_LEN			128			// length of event name
#define DH_STORAGE_NAME_LEN			128			// length of storage name
#define DH_MAX_CARPORTLIGHT_NUM     4           // carport liht maximum number
#define DH_STATION_NAME_LEN         32          // length of station name
#define	PTZ_PRESET_NAME_LEN         64			// Length of PTZ preset name
	
#define DH_MAX_VERSION_STR			64			// Max length of version string

#define DH_COMMON_STRING_16			16          // Common string length 16
#define DH_COMMON_STRING_32			32          // Common string length 32
#define DH_COMMON_STRING_64			64          // Common string length 64
#define DH_COMMON_STRING_128		128         // Common string length 128
#define DH_COMMON_STRING_256		256         // Common string length 256
#define DH_COMMON_STRING_512		512         // Common string length 512
#define DH_COMMON_STRING_1024		1024        // Common string length 1024

#define DH_MAX_ACCESS_NAME_LEN		64			// Length of access name
#define DH_MAX_EXALARMCHANNEL_NAME_LEN	 128	// length of extension module alarm channel name


// Remote configuration structure corresponding constant 
#define DH_MAX_MAIL_ADDR_LEN		128			// Mail address max length
#define DH_MAX_MAIL_SUBJECT_LEN		64			// Mail subject max length 
#define DH_MAX_IPADDR_LEN			16			// IP address string length 
#define DH_MAX_IPADDR_LEN_EX		40			// extension Ip address support IPV6
#define DH_MACADDR_LEN				40			// MACE address string length
#define DH_MAX_URL_LEN				128			// URL string length 
#define DH_MAX_DEV_ID_LEN			48			// Device serial number max length 
#define	DH_MAX_HOST_NAMELEN			64			// Host name length 
#define DH_MAX_HOST_PSWLEN			32			// Password length 
#define DH_MAX_NAME_LEN				16			// Universal name string length 
#define DH_MAX_ETHERNET_NUM			2			// Ethernet max amount 
#define DH_MAX_ETHERNET_NUM_EX	    10			// extended ethernet max amout
#define	DH_DEV_SERIALNO_LEN			48			// Serial number string length 
#define DH_DEV_TYPE_LEN				32			// Device type string length 
#define DH_N_WEEKS					7			// The days in one week 
#define DH_N_TSECT					6			// Time period amount 
#define DH_N_REC_TSECT				6			// Record period amount 
#define DH_N_COL_TSECT				2			// Color period amount 
#define DH_CHAN_NAME_LEN			32			// Channel name lengh. DVR DSP capacity limit. Max 32 bytes.		
#define DH_N_ENCODE_AUX				3			// Extra stream amount 
#define DH_N_TALK					1			// Max audio talk channel amount 
#define DH_N_COVERS					1			// Privacy mask zone amount 
#define DH_N_CHANNEL				16			// Max channel amount 
#define DH_N_ALARM_TSECT			2			// Alarm prompt period amount 
#define DH_MAX_ALARMOUT_NUM			16			// Alarm output ports max amount 
#define DH_MAX_AUDIO_IN_NUM			16			// Audio input ports max amount 
#define DH_MAX_VIDEO_IN_NUM			16			// Video input ports max amount 
#define DH_MAX_ALARM_IN_NUM			16			// Alarm input ports max amount 
#define DH_MAX_DISK_NUM				16			// HDD max amount. Now the value is 16.
#define DH_MAX_DECODER_NUM			16			// Decoder(485) max amount 
#define DH_MAX_232FUNCS				10			// 232 COM function max amount 
#define DH_MAX_232_NUM				2			// 232 COM port max amount 
#define DH_MAX_232_NUM_EX           16          // extended 232 COM port max amount 
#define DH_MAX_DECPRO_LIST_SIZE		100			// Decoder protocol list max amount 
#define DH_FTP_MAXDIRLEN			240			// FTP file folder max length 
#define DH_MATRIX_MAXOUT			16			// Matrix output ports max amount
#define DH_TOUR_GROUP_NUM			6			// Matrix output ports max amount 
#define DH_MAX_DDNS_NUM				10			// ddns max amount the device supported 
#define DH_MAX_SERVER_TYPE_LEN		32			// ddns type and max string length 
#define DH_MAX_DOMAIN_NAME_LEN		256			// ddns domain name and max string length 
#define DH_MAX_DDNS_ALIAS_LEN		32			// ddns alias and max string length 
#define DH_MAX_DEFAULT_DOMAIN_LEN	60			// ddns default domain, max sring length	
#define DH_MOTION_ROW				32			// Motion detection zone row amount 
#define DH_MOTION_COL				32			// Motion detection zone column amount 
#define DH_STATIC_ROW				32			// Static detection zone row amount 
#define DH_STATIC_COL				32			// Static detection zone column amount 
#define	DH_FTP_USERNAME_LEN			64			// FTP setup:user name max lengh 
#define	DH_FTP_PASSWORD_LEN			64			// FTP setup:password max length 
#define	DH_TIME_SECTION				2			// FTP setup:time periods in each day.
#define DH_FTP_MAX_PATH				240			// FTP setup:file path max length 
#define DH_FTP_MAX_SUB_PATH			128 		// FTP setup:sub path max lenth
#define DH_INTERVIDEO_UCOM_CHANID	32			// Platform embedded setup:U China Net Communication (CNC)channel ID
#define DH_INTERVIDEO_UCOM_DEVID	32			// Platform embedded setup:UCNC device ID
#define DH_INTERVIDEO_UCOM_REGPSW	16			// Platform embedded setup:UCNC registration password 
#define DH_INTERVIDEO_UCOM_USERNAME	32			// Platform embedded setup:UCNC user name 
#define DH_INTERVIDEO_UCOM_USERPSW	32			// Platform embedded setup: UCNC password 
#define DH_INTERVIDEO_NSS_IP		32			// Platform embedded setup:ZTE Netview IP
#define DH_INTERVIDEO_NSS_SERIAL	32			// Serial Platform embedded setup:ZTE Netview  serial
#define DH_INTERVIDEO_NSS_USER		32			// User Platform embedded setup:ZTE Netview user
#define DH_INTERVIDEO_NSS_PWD		50			// Password Platform embedded setup:ZTE Netview password
#define DH_MAX_VIDEO_COVER_NUM		16			// Privacy mask zones max amount 
#define DH_MAX_WATERMAKE_DATA		4096		// Watermark data max length 
#define DH_MAX_WATERMAKE_LETTER		128			// Watermark text max length
#define DH_MAX_WLANDEVICE_NUM		10			// Max searched wireless device amount 
#define DH_MAX_WLANDEVICE_NUM_EX	32			// Max searched wireless device amount
#define DH_MAX_ALARM_NAME			64			// Address length 
#define DH_MAX_REGISTER_SERVER_NUM	10			// Auto registration server amount 
#define DH_SNIFFER_FRAMEID_NUM		6			// 6 FRAME ID options
#define DH_SNIFFER_CONTENT_NUM		4			// 4 sniffer in each FRAME
#define DH_SNIFFER_CONTENT_NUM_EX	8			// 8 sniffer in each FRAME
#define DH_SNIFFER_PROTOCOL_SIZE	20			// protocol length
#define DH_MAX_PROTOCOL_NAME_LENGTH 20
#define DH_SNIFFER_GROUP_NUM		4			// 4 group sniffer setup 
#define MAX_PATH_STOR				240			// Remote folder length 
#define DH_ALARM_OCCUR_TIME_LEN		40			// New alarm upload time length 
#define DH_VIDEO_OSD_NAME_NUM		64			// Overlay name length. Now it supports 32-digit English and 16-digit Chinese.
#define DH_VIDEO_CUSTOM_OSD_NUM		8			// The self-defined amount supported excluding time and channel.
#define DH_CONTROL_AUTO_REGISTER_NUM 100        // Targeted initiatives to support the number of registration servers
#define DH_MMS_RECEIVER_NUM          100        // Support the number of messages the recipient
#define DH_MMS_SMSACTIVATION_NUM     100        // Support the number of SMS sender
#define DH_MMS_DIALINACTIVATION_NUM  100        // Support for dial-up number of the sender
#define DH_MAX_ALARMOUT_NUM_EX		 32			// Alarm output amount max limit
#define DH_MAX_VIDEO_IN_NUM_EX		 32			// Video input amount max limit
#define DH_MAX_ALARM_IN_NUM_EX		 32			// Alarm input amount max limit
#define DH_MAX_IPADDR_OR_DOMAIN_LEN	 64			// IP or domain name length
#define DH_MAX_OBJECT_LIST				 16			// the upper limit number of object id that detected by device in intelligent analysis 
#define DH_MAX_RULE_LIST				 16			// the upper limit number of device rule in intelligent analysis 
#define DH_MAX_POLYGON_NUM				 16			// the max number of polygon's culmination
#define DH_MAX_DETECT_LINE_NUM       20          // rule detect line's max point number
#define DH_MAX_DETECT_REGION_NUM     20          // rule detect region's max point number
#define DH_MAX_TRACK_LINE_NUM        20         // object moving track's max point number
#define DH_MAX_CANDIDATE_NUM         50         // max candidate number
#define DH_MAX_PERSON_IMAGE_NUM      48         // max image of ervery person
#define DH_MAX_FENCE_LINE_NUM        2          // max fance line number
#define MAX_SMART_VALUE_NUM          30         // the max number of smart disk information
#define DH_MACHINE_NAME_NUM			 64         // Device name length
#define DH_INTERVIDEO_AMP_DEVICESERIAL    48    // Platform embedded setup, AMP,Device serial num length
#define DH_INTERVIDEO_AMP_DEVICENAME      16    // Platform embedded setup, AMP,Device Name length
#define DH_INTERVIDEO_AMP_USER            32    // Platform embedded setup, AMP,User Name Length
#define DH_INTERVIDEO_AMP_PWD             32    // Platform embedded setup, AMP,password length
#define MAX_SUBMODULE_NUM            32         // Max number of submodule infomation
#define DH_MAX_CARWAY_NUM            8          // traffic snapshot,the max car way number
#define DH_MAX_SNAP_SIGNAL_NUM       3          // one car way max time for snapshot
#define DH_MAX_CARD_NUM              128        // max number of card
#define DH_MAX_CARDINFO_LEN          32         // max lenth of card infomation
#define DH_MAX_CONTROLER_NUM         64         // max supported controller number
#define DH_MAX_LIGHT_NUM             32         // max control light group number
#define DH_MAX_SNMP_COMMON_LEN       64         // max Snmp read or write buffer
#define DH_MAX_DDNS_STATE_LEN        128        // max DDNS state buffer
#define DH_MAX_PHONE_NO_LEN          16         // max phone len
#define DH_MAX_MSGTYPE_LEN           32         // max type len
#define DH_MAX_MSG_LEN               256        // max message len
#define DH_MAX_DRIVINGDIRECTION      256        // max drivingdirection len
#define DH_MAX_GRAB_INTERVAL_NUM     4	        // max snapshot count
#define DH_MAX_FLASH_NUM			 5			// max flash count
#define DH_MAX_LANE_NUM              8          // max number of road in every channel
#define DH_MAX_ISCSI_PATH_NUM		 64			// max ISCSI remote path number
#define DH_MAX_WIRELESS_CHN_NUM		 256		// max wireless channel number
#define DH_PROTOCOL3_BASE			 100		// protocol 3 base value
#define DH_PROTOCOL3_SUPPORT		 11			// prococol 3 support
#define DH_MAX_CHANMASK              64         // max channel mask
#define DH_MAX_STAFF_NUM			 20			// max of compressed video configuration staff number
#define DH_MAX_CALIBRATEBOX_NUM		 10			// max of compressed video configuration calibrate region
#define DH_MAX_EXCLUDEREGION_NUM	 10			// max of compressed video configuration exclude region
#define DH_MAX_POLYLINE_NUM			 20			// number of compressed video configuration poly line
#define	DH_MAX_COLOR_NUM			 16			// color maximum number
#define MAX_OBJFILTER_NUM			 16			// max number of  filter type 
#define DH_MAX_SYNOPSIS_STATE_NAME   64         // string state length of compressed video 
#define DH_MAX_SYNOPSIS_QUERY_FILE_COUNT	10	// the file number limit
#define DH_MAX_SSID_LEN              36         // length of SSID
#define DH_MAX_APPIN_LEN             16         // length of PIN
#define DH_NETINTERFACE_NAME_LEN     260        // length of net port
#define DH_NETINTERFACE_TYPE_LEN     260        // length of net type
#define DH_MAX_CONNECT_STATUS_LEN    260        // length of connect status
#define DH_MAX_MODE_LEN              64         // length of 3G mode support
#define DH_MAX_MODE_NUM              64         // number of 3G mode support
#define DH_MAX_CAPTURE_SIZE_NUM      64         // number of video resolution 
#define DH_NODE_NAME_LEN			 64			// length of node name
#define MAX_CALIBPOINTS_NUM          256        // max number of point support
#define DH_MAX_ATTR_NUM				 32			// the maximum number display element attributes
#define DH_MAX_CLOUDCONNECT_STATE_LEN 128       // length of cloud connect state
#define DH_MAX_IPADDR_EX_LEN         128        // IP entended maximum length
#define DH_MAX_PLATE_NUMBER_LEN      32         // plate string length    
#define DH_MAX_AUTHORITY_LIST_NUM    16         // max in authority list   
#define DH_MAX_CITY_NAME_LEN         64         // max length of city name
#define DH_MAX_PROVINCE_NAME_LEN     64         // max length of province name
#define DH_MAX_PERSON_ID_LEN         32         // max length of person ID
#define MAX_FACE_AREA_NUM            8          // max number if face area
#define MAX_FACE_DB_NUM              8          // max number of face DB
#define MAX_EVENT_NAME				128			// max event name
#define DH_MAX_ETH_NAME			     64			// max network card name
#define DH_MAX_PERSON_NAME_LEN       64         // max length of person name
#define DH_N_SCHEDULE_TSECT			 8			// element number of schedule	
#define DH_MAX_URL_NUM				 8			// max URL number 
#define DH_MAX_LOWER_MITRIX_NUM		16			// max lower mitrix number
#define DH_MAX_BURN_CHANNEL_NUM		32			// max burn channel number
#define DH_MAX_NET_STRORAGE_BLOCK_NUM	64		// max remote storage block number 
#define DH_MAX_CASE_PERSON_NUM		32			// max number of case person
#define DH_MAX_MULTIPLAYBACK_CHANNEL_NUM 64     // max channel number of multiplayback channel
#define DH_MAX_MULTIPLAYBACK_SPLIT_NUM   32     // max split number of multipalyback channel
#define DH_MAX_AUDIO_ENCODE_TYPE          64               // ��������������͸���

#define DH_BATTERY_NUM_MAX			 16			// max battery number	
#define DH_POWER_NUM_MAX			 16			// max power number	
#define DH_MAX_AUDIO_PATH			 260		// max path of audio file
#define DH_MAX_DOORNAME_LEN			 128		// max length of door access name
#define DH_MAX_CARDPWD_LEN			 64			// max length of door access name

// Search type, corresponding to CLIENT_QueryDevState
#define DH_DEVSTATE_COMM_ALARM		0x0001		// Search general alarm status(including external alarm,video loss, motion detection)
#define DH_DEVSTATE_SHELTER_ALARM	0x0002		// Search camera masking alarm status
#define DH_DEVSTATE_RECORDING		0x0003		// Search record status 
#define DH_DEVSTATE_DISK			0x0004		// Search HDD information 
#define DH_DEVSTATE_RESOURCE		0x0005		// Search system resources status 
#define DH_DEVSTATE_BITRATE			0x0006		// Search channel bit stream 
#define DH_DEVSTATE_CONN			0x0007		// Search device connection status 
#define DH_DEVSTATE_PROTOCAL_VER	0x0008		// Search network protocol version number , pBuf = int*
#define DH_DEVSTATE_TALK_ECTYPE		0x0009		// Search the audio talk format the device supported. Please refer to structure DHDEV_TALKFORMAT_LIST
#define DH_DEVSTATE_SD_CARD			0x000A		// Search SD card information(FOR IPC series)
#define DH_DEVSTATE_BURNING_DEV		0x000B		// Search burner information
#define DH_DEVSTATE_BURNING_PROGRESS 0x000C		// Search burning information
#define DH_DEVSTATE_PLATFORM		0x000D		// Search the embedded platform the device supported
#define DH_DEVSTATE_CAMERA			0x000E		// Search camera property information ( for IPC series),pBuf = DHDEV_CAMERA_INFO *,there can be several structure.
#define DH_DEVSTATE_SOFTWARE		0x000F		// Search device software version information
#define DH_DEVSTATE_LANGUAGE        0x0010		// Search the audio type the device supported 
#define DH_DEVSTATE_DSP				0x0011		// Search DSP capacity description 
#define	DH_DEVSTATE_OEM				0x0012		// Search OEM information 
#define	DH_DEVSTATE_NET				0x0013		// Search network running status information 
#define DH_DEVSTATE_TYPE			0x0014		// Search function attributes(for IPC serirs)
#define DH_DEVSTATE_SNAP			0x0015		// Search snapshot function attribute(For IPC series)
#define DH_DEVSTATE_RECORD_TIME		0x0016		// Query the first time and the recent video time
#define DH_DEVSTATE_NET_RSSI        0x0017      // Query the wireless network signal strength,Please refer to structure DHDEV_WIRELESS_RSS_INFO
#define DH_DEVSTATE_BURNING_ATTACH	0x0018		// Burning options inquiry
#define DH_DEVSTATE_BACKUP_DEV		0x0019		// Query the list of backup device
#define DH_DEVSTATE_BACKUP_DEV_INFO	0x001a		// Query the backup device information
#define DH_DEVSTATE_BACKUP_FEEDBACK	0x001b		// backup rate of progress
#define DH_DEVSTATE_ATM_QUERY_TRADE 0x001c		// Query ATM trade type
#define DH_DEVSTATE_SIP				0x001d		// Query sip state
#define DH_DEVSTATE_VICHILE_STATE	0x001e		// Query wifi state of mobile DVR
#define DH_DEVSTATE_TEST_EMAIL      0x001f      // Query Email Function
#define DH_DEVSTATE_SMART_HARD_DISK 0x0020      // Query Hard Disk Information
#define DH_DEVSTATE_TEST_SNAPPICTURE 0x0021     // Query Snap Picture Function
#define DH_DEVSTATE_STATIC_ALARM    0x0022      // Query static alarm state
#define DH_DEVSTATE_SUBMODULE_INFO  0x0023      // Query submodule information
#define DH_DEVSTATE_DISKDAMAGE      0x0024      // Query harddisk damage ability     
#define DH_DEVSTATE_IPC             0x0025      // Query device supported IPC ability,see struct DH_DEV_IPC_INFO 
#define DH_DEVSTATE_ALARM_ARM_DISARM 0x0026     // Query alarm arm disarm state
#define DH_DEVSTATE_ACC_POWEROFF_ALARM 0x0027   // Query ACC power off state(return a DWORD type value, 1 means power off,0 means power on)
#define DH_DEVSTATE_TEST_FTP_SERVER 0x0028      // FTP server connect test
#define DH_DEVSTATE_3GFLOW_EXCEED   0x0029      // Query 3G Flow exceed state info(see struct DHDEV_3GFLOW_EXCEED_STATE_INFO)
#define DH_DEVSTATE_3GFLOW_INFO     0x002a      // Query 3G Flow info(see struct DH_DEV_3GFLOW_INFO)
#define DH_DEVSTATE_VIHICLE_INFO_UPLOAD  0x002b // Vihicle information uploading
#define DH_DEVSTATE_SPEED_LIMIT     0x002c      // Speed limit alarm
#define DH_DEVSTATE_DSP_EX          0x002d      // Query DSP expended cap(struct DHDEV_DSP_ENCODECAP_EX)
#define DH_DEVSTATE_3GMODULE_INFO    0x002e     // Query 3G module info(struct DH_DEV_3GMODLE_INFO)
#define DH_DEVSTATE_MULTI_DDNS      0x002f      // Query multi DDNS status(struct DH_DEV_MULTI_DDNS_INFO)
#define DH_DEVSTATE_CONFIG_URL      0x0030      // Query Device URL info(struct DH_DEV_URL_INFO)
#define DH_DEVSTATE_HARDKEY        0x0031       // Query Hard key state(struct DHDEV_HARDKEY_STATE)
#define DH_DEVSTATE_ISCSI_PATH		0x0032		// Query ISCSI path(struct DHDEV_ISCSI_PATHLIST)
#define DH_DEVSTATE_DLPREVIEW_SLIPT_CAP   0x0033      // Query local preview split capability(struct DEVICE_LOCALPREVIEW_SLIPT_CAP)
#define DH_DEVSTATE_WIFI_ROUTE_CAP	0x0034		// Query wifi capablity(struct DHDEV_WIFI_ROUTE_CAP)
#define DH_DEVSTATE_ONLINE          0x0035      // Query device online state(return a DWORD value, 1-online, 0-offline)
#define DH_DEVSTATE_PTZ_LOCATION    0x0036      // Query ptz state(struct DH_PTZ_LOCATION_INFO)
#define DH_DEVSTATE_MONITOR_INFO    0x0037      // Query monitor state(state DHDEV_MONITOR_INFO)
#define DH_DEVSTATE_SUBDEVICE		0x0300		// Query subdevcie(fan,cpu...) state(struct CFG_DEVICESTATUS_INFO)
#define DH_DEVSTATE_RAID_INFO       0x0038      // Query RAID state(struct ALARM_RAID_INFO)  
#define DH_DEVSTATE_TEST_DDNSDOMAIN 0x0039      // test DDNS domain enable
#define DH_DEVSTATE_VIRTUALCAMERA   0x003a      // query virtual camera state(struct DHDEV_VIRTUALCAMERA_STATE_INFO)
#define DH_DEVSTATE_TRAFFICWORKSTATE 0x003b     // get device's state of video/coil module(struct DHDEV_TRAFFICWORKSTATE_INFO)
#define DH_DEVSTATE_ALARM_CAMERA_MOVE 0x003c    // get camera move alarm state(struct ALARM_CAMERA_MOVE_INFO)
#define DH_DEVSTATE_ALARM           0x003e      // get external alarm(struct NET_CLIENT_ALARM_STATE) 
#define DH_DEVSTATE_VIDEOLOST       0x003f      // get video loss alarm(struct NET_CLIENT_VIDEOLOST_STATE) 
#define DH_DEVSTATE_MOTIONDETECT    0x0040      // get motion alarm(struct NET_CLIENT_MOTIONDETECT_STATE)
#define DH_DEVSTATE_DETAILEDMOTION  0x0041      // get detailed motion alarm(struct NET_CLIENT_DETAILEDMOTION_STATE)
#define DH_DEVSTATE_VEHICLE_INFO   0x0042		// get vehicle device state(struct DHDEV_VEHICLE_INFO)
#define DH_DEVSTATE_VIDEOBLIND      0x0043       // get blind alarm(struct NET_CLIENT_VIDEOBLIND_STATE)
#define DH_DEVSTATE_3GSTATE_INFO   0x0044       // Query 3G state(struct DHDEV_VEHICLE_3GMODULE)
#define DH_DEVSTATE_NETINTERFACE   0x0045       // Query net interface(struct DHDEV_NETINTERFACE_INFO)
#define DH_DEVSTATE_PICINPIC_CHN	0x0046		// Query picinpic channel(struct DWORD)
#define DH_DEVSTATE_COMPOSITE_CHN	0x0047		// Query splice screen(struct DH_COMPOSITE_CHANNEL)
#define DH_DEVSTATE_WHOLE_RECORDING	0x0048		// Query whole recording status(struct BOOL),as long as ther is a channel running,it indicates that the overall state
#define DH_DEVSTATE_WHOLE_ENCODING	0x0049		// Query whole encoding(struct BOOL),as long as ther is a channel running,it indicates that the overall state
#define DH_DEVSTATE_DISK_RECORDE_TIME 0x004a    // Query disk record time(pBuf = DEV_DISK_RECORD_TIME*)
#define DH_DEVSTATE_BURNER_DOOR     0x004b      // whether have pop-up optical dirve(struct NET_DEVSTATE_BURNERDOOR)
#define DH_DEVSTATE_GET_DATA_CHECK  0x004c      // get data validation process(struct NET_DEVSTATE_DATA_CHECK)
#define DH_DEVSTATE_ALARM_IN_CHANNEL 0x004f		// Query alarm input channel information(struct NET_ALARM_IN_CHANNEL)
#define DH_DEVSTATE_ALARM_CHN_COUNT	0x0050		// Query alarm channel number(struct NET_ALARM_CHANNEL_COUNT)
#define DH_DEVSTATE_PTZ_VIEW_RANGE	0x0051		// Query PTZ view range status(struct DH_OUT_PTZ_VIEW_RANGE_STATUS)
#define DH_DEVSTATE_DEV_CHN_COUNT	0x0052		// Query device channel information(struct NET_DEV_CHN_COUNT_INFO)
#define DH_DEVSTATE_RTSP_URL		0x0053		// Query RTSP URL list supported by device, struct DEV_RTSPURL_LIST
#define DH_DEVSTATE_LIMIT_LOGIN_TIME	0x0054	// Query online overtime of device logging in and return a BTY��UNIT��MIN�� ,0 means no limite and The non-zero positive integer means restrictions on the number of minutes
#define DH_DEVSTATE_GET_COMM_COUNT	0x0055		// get com count (struct NET_GET_COMM_COUNT)
#define DH_DEVSTATE_RECORDING_DETAIL	0x0056	// Query recording status detail information(pBuf = NET_RECORD_STATE_DETAIL*)
#define DH_DEVSTATE_PTZ_PRESET_LIST     0x0057  // get state PTZ preset list (struct NET_PTZ_PRESET_LIST)
#define DH_DEVSTATE_EXTERNAL_DEVICE	0x0058		// exteral device information (pBuf = NET_EXTERNAL_DEVICE*)
#define DH_DEVSTATE_GET_UPGRADE_STATE   0x0059  // get device upgrade state(struct DHDEV_UPGRADE_STATE_INFO)
#define DH_DEVSTATE_MULTIPLAYBACK_SPLIT_CAP 0x005a // get mulipalyback split (struct NET_MULTIPLAYBACK_SPLIT_CAP )
#define DH_DEVSTATE_BURN_SESSION_NUM	0x005b  // get burn session number(pBuf = int*)
#define DH_DEVSTATE_PROTECTIVE_CAPSULE    0X005c           // ��ѯ������״̬(��Ӧ�ṹ��ALARM_PROTECTIVE_CAPSULE_INFO)

#define DH_DEVSTATE_POWER_STATE		0x0152		// Query power state(struct DH_POWER_STATUS)
#define DH_DEVSTATE_ALL_ALARM_CHANNELS_STATE	0x153	// Query alarm channel state(struct NET_CLIENT_ALARM_CHANNELS_STATE)
#define DH_DEVSTATE_ALARMKEYBOARD_COUNT	0x0154	// Query alarm keyboard count connected on com(struct NET_ALARMKEYBOARD_COUNT)
#define DH_DEVSTATE_EXALARMCHANNELS	0x0155		// Query mapping relationship of extension alarm module channel (struct NET_EXALARMCHANNELS)
#define DH_DEVSTATE_GET_BYPASS		0x0156		// Query channel bypass state(struct NET_DEVSTATE_GET_BYPASS)
#define DH_DEVSTATE_ACTIVATEDDEFENCEAREA	0x0157	// get active sector information(struct NET_ACTIVATEDDEFENCEAREA)
#define DH_DEVSTATE_DEV_RECORDSET	0x0158		// Query device recording information(struct NET_CTRL_RECORDSET_PARAM)
#define DH_DEVSTATE_DOOR_STATE		0x0159		// Query door access state(struct NET_DOOR_STATUS_INFO)
#define DH_DEVSTATE_ANALOGALARM_CHANNELS	0x1560	// analog alarm input channel mapping (struct NET_ANALOGALARM_CHANNELS)
#define DH_DEVSTATE_GET_SENSORLIST        0x1561 // ��ȡ�豸֧�ֵĴ������б�(��Ӧ NET_SENSOR_LIST)
#define DH_DEVSTATE_ALARM_CHANNELS	0x1562		// ��ѯ����������ģ��ͨ��ӳ���ϵ(��Ӧ�ṹ�� NET_ALARM_CHANNELS)
												// ����豸��֧�ֲ�ѯ��չ����ģ��ͨ��,�����øù��ܲ�ѯ��չͨ�����߼�ͨ���ţ����������ر���ͨ��ʹ��
#define DH_DEVSTATE_GET_ALARM_SUBSYSTEM_ACTIVATESTATUS	0x1563	// ��ȡ��ǰ��ϵͳ����״̬( ��Ӧ NET_GET_ALARM_SUBSYSTEM_ACTIVATE_STATUES)


// ����������ͨ����Ϣ
typedef struct tagNET_ALARM_CHANNEL_INFO 
{
	DWORD			dwSize;
	int				nSlot;					// ����ַ, 0��ʾ����ͨ��, 1��ʾ�����ڵ�һ�������ϵ���չͨ��, 2��3...�Դ�����
	int				nChannel;				// �Ǳ��ر���ͨ������չģ���ϵ�ͨ����, ��0��ʼ
	char			szName[DH_COMMON_STRING_128];	// ͨ������
}NET_ALARM_CHANNEL_INFO;

// ����(��Զ��)����������ͨ�����߼�ͨ����ӳ���ϵ
typedef struct tagNET_ALARM_CHANNELS 
{
	DWORD			dwSize;
	int				nMaxAlarmChannels;		// ���ͨ����
	int				nRetAlarmChannels;		// ���ص�ͨ����
	NET_ALARM_CHANNEL_INFO*	pstuChannelInfo;// ͨ����Ϣ���û������ڴ�
}NET_ALARM_CHANNELS;

// Configuration type,corresponding to CLIENT_QueryRemotDevState
#define DH_DEVSTATE_ALARM_REMOTE   0x1000        // get the remote device external alarm(struct ALARM_REMOTE_ALARM_INFO)
#define DH_DEVSTATE_ALARM_FRONTDISCONNECT 0x1001 // get the front IPC disconnect alarm (struct ALARM_FRONTDISCONNET_INFO)

// Configuration type,corresponding to CLIENT_GetDevConfig and CLIENT_SetDevConfig
#define DH_DEV_DEVICECFG			0x0001		// Device property setup 
#define DH_DEV_NETCFG				0x0002		// Network setup 
#define DH_DEV_CHANNELCFG			0x0003		// Video channel setup
#define DH_DEV_PREVIEWCFG 			0x0004		// Preview parameter setup
#define DH_DEV_RECORDCFG			0x0005		// Record setup
#define DH_DEV_COMMCFG				0x0006		// COM property setup 
#define DH_DEV_ALARMCFG 			0x0007		// Alarm property setup
#define DH_DEV_TIMECFG 				0x0008		// DVR time setup 
#define DH_DEV_TALKCFG				0x0009		// Audio talk parameter setup 
#define DH_DEV_AUTOMTCFG			0x000A		// Auto matrix setup
#define	DH_DEV_VEDIO_MARTIX			0x000B		// Local matrix control strategy setup
#define DH_DEV_MULTI_DDNS			0x000C		//  Multiple ddns setup 
#define DH_DEV_SNAP_CFG				0x000D		// Snapshot corresponding setup 
#define DH_DEV_WEB_URL_CFG			0x000E		// HTTP path setup 
#define DH_DEV_FTP_PROTO_CFG		0x000F		// FTP upload setup 
#define DH_DEV_INTERVIDEO_CFG		0x0010		// Plaform embedded setup. Now the channel parameter represents the platform type. 
												// channel=4:Bell alcatel;channel=10:ZTE Netview;channel=11:U CNC  channel = 51 AMP
#define DH_DEV_VIDEO_COVER			0x0011		// Privacy mask setup
#define DH_DEV_TRANS_STRATEGY		0x0012		// Transmission strategy. Video quality\Fluency
#define DH_DEV_DOWNLOAD_STRATEGY	0x0013		//  Record download strategy setup:high-speed\general download
#define DH_DEV_WATERMAKE_CFG		0x0014		// Video watermark setup
#define DH_DEV_WLAN_CFG				0x0015		// Wireless network setup 
#define DH_DEV_WLAN_DEVICE_CFG		0x0016		// Search wireless device setup 
#define DH_DEV_REGISTER_CFG			0x0017		// Auto register parameter setup 
#define DH_DEV_CAMERA_CFG			0x0018		// Camera property setup 
#define DH_DEV_INFRARED_CFG 		0x0019		// IR alarm setup 
#define DH_DEV_SNIFFER_CFG			0x001A		// Sniffer setup 
#define DH_DEV_MAIL_CFG				0x001B		// Mail setup 
#define DH_DEV_DNS_CFG				0x001C		// DNS setup 
#define DH_DEV_NTP_CFG				0x001D		// NTP setup
#define DH_DEV_AUDIO_DETECT_CFG		0x001E		// Audio detection setup 
#define DH_DEV_STORAGE_STATION_CFG  0x001F      // Storage position setup 
#define DH_DEV_PTZ_OPT_CFG			0x0020		// PTZ operation property(It is invalid now. Please use CLIENT_GetPtzOptAttr to get PTZ operation property.)
#define DH_DEV_DST_CFG				0x0021      // Daylight Saving Time (DST)setup
#define DH_DEV_ALARM_CENTER_CFG		0x0022		// Alarm centre setup
#define DH_DEV_VIDEO_OSD_CFG        0x0023		// VIdeo OSD setup
#define DH_DEV_CDMAGPRS_CFG         0x0024		// CDMA\GPRS configuration

#define DH_DEV_IPFILTER_CFG         0x0025		// IP Filter configuration
#define DH_DEV_TALK_ENCODE_CFG      0x0026      // Talk encode configuration
#define DH_DEV_RECORD_PACKET_CFG    0X0027      // The length of the video package configuration
#define DH_DEV_MMS_CFG              0X0028		// SMS MMS configuration 
#define DH_DEV_SMSACTIVATION_CFG	0X0029		// SMS to activate the wireless connection configuration
#define DH_DEV_DIALINACTIVATION_CFG	0X002A		// Dial-up activation of a wireless connection configuration
#define DH_DEV_SNIFFER_CFG_EX		0x0030		// Capture network configuration
#define DH_DEV_DOWNLOAD_RATE_CFG	0x0031		// Download speed limit
#define DH_DEV_PANORAMA_SWITCH_CFG	0x0032		// Panorama switch alarm configuration
#define DH_DEV_LOST_FOCUS_CFG		0x0033		// Lose focus alarm configuration
#define DH_DEV_ALARM_DECODE_CFG		0x0034		// Alarm decoder configuration
#define DH_DEV_VIDEOOUT_CFG         0x0035      // Video output configuration
#define DH_DEV_POINT_CFG			0x0036		// Preset enable configuration
#define DH_DEV_IP_COLLISION_CFG     0x0037      // IP conflication configurationIp
#define DH_DEV_OSD_ENABLE_CFG		0x0038		// OSD overlay enable configuration
#define DH_DEV_LOCALALARM_CFG 		0x0039		// Local alarm configuration(Structure DH_ALARMIN_CFG_EX)
#define DH_DEV_NETALARM_CFG 		0x003A		// Network alarm configuration(Structure DH_ALARMIN_CFG_EX)
#define DH_DEV_MOTIONALARM_CFG 		0x003B		// Motion detection configuration(Structure DH_MOTION_DETECT_CFG_EX)
#define DH_DEV_VIDEOLOSTALARM_CFG 	0x003C		// Video loss configuration(Structure DH_VIDEO_LOST_CFG_EX)
#define DH_DEV_BLINDALARM_CFG 		0x003D		// Camera masking configuration(Structure DH_BLIND_CFG_EX)
#define DH_DEV_DISKALARM_CFG 		0x003E		// HDD alarm configuration(Structure DH_DISK_ALARM_CFG_EX)
#define DH_DEV_NETBROKENALARM_CFG 	0x003F		// Network disconnection alarm configuration(Structure DH_NETBROKEN_ALARM_CFG_EX)
#define DH_DEV_ENCODER_CFG          0x0040      // Digital channel info of front encoders(Hybrid DVR use,Structure DEV_ENCODER_CFG)
#define DH_DEV_TV_ADJUST_CFG        0x0041      // TV adjust configuration(Now the channel parameter represents the TV numble(from 0), Structure DHDEV_TVADJUST_CFG)
#define DH_DEV_ABOUT_VEHICLE_CFG	0x0042		// about vehicle configuration
#define DH_DEV_ATM_OVERLAY_ABILITY	0x0043		// ATM ability information
#define DH_DEV_ATM_OVERLAY_CFG		0x0044		// ATM overlay configuration
#define DH_DEV_DECODER_TOUR_CFG		0x0045		// Decoder tour configuration
#define DH_DEV_SIP_CFG				0x0046		// SIP configuration
#define DH_DEV_VICHILE_WIFI_AP_CFG	0x0047		// wifi ap configuration
#define DH_DEV_STATICALARM_CFG      0x0048      // Static 
#define DH_DEV_DECODE_POLICY_CFG    0x0049      // decode policy configuration(Structure DHDEV_DECODEPOLICY_CFG,channel start with 0) 
#define DH_DEV_MACHINE_CFG			0x004A		// Device relative config (Structure DHDEV_MACHINE_CFG)
#define DH_DEV_MAC_COLLISION_CFG    0x004B      // MACconflication configuration(Structure ALARM_MAC_COLLISION_CFG)
#define DH_DEV_RTSP_CFG             0x004C      // RTSP configuration(structure DHDEV_RTSP_CFG)
#define DH_DEV_232_COM_CARD_CFG     0x004E      // 232 com card signal event configuration(structure COM_CARD_SIGNAL_LINK_CFG)
#define DH_DEV_485_COM_CARD_CFG     0x004F      // 485 com card signal event configuration(structure COM_CARD_SIGNAL_LINK_CFG)
#define DH_DEV_FTP_PROTO_CFG_EX		0x0050		// extend FTP upload setup(Structure DHDEV_FTP_PROTO_CFG_EX)
#define DH_DEV_SYSLOG_REMOTE_SERVER	0x0051		// SYSLOG remote server config (Structure DHDEV_SYSLOG_REMOTE_SERVER)
#define DH_DEV_COMMCFG_EX           0x0052      // Extended com configuration(structure DHDEV_COMM_CFG_EX)
#define DH_DEV_NETCARD_CFG          0x0053      // net card configuration(structure DHDEV_NETCARD_CFG)
#define DH_DEV_BACKUP_VIDEO_FORMAT  0x0054		// Video backup format(structure DHDEV_BACKUP_VIDEO_FORMAT)
#define DH_DEV_STREAM_ENCRYPT_CFG   0x0055      // stream encrypt configuration(structure DHEDV_STREAM_ENCRYPT)
#define DH_DEV_IPFILTER_CFG_EX      0x0056		// IP filter extended configuration(structure DHDEV_IPIFILTER_CFG_EX)
#define DH_DEV_CUSTOM_CFG           0x0057      // custom configuration(structure DHDEV_CUSTOM_CFG)
#define DH_DEV_WLAN_DEVICE_CFG_EX   0x0058      // Search wireless device extended setup(structure DHDEV_WLAN_DEVICE_LIST_EX)
#define DH_DEV_ACC_POWEROFF_CFG     0x0059      // ACC power off configuration(structure DHDEV_ACC_POWEROFF_CFG)
#define DH_DEV_EXPLOSION_PROOF_CFG  0x005a      // explosion proof alarm configuration(structure DHDEV_EXPLOSION_PROOF_CFG)
#define DH_DEV_NETCFG_EX			0x005b		// Network extended setup(struct DHDEV_NET_CFG_EX)
#define DH_DEV_LIGHTCONTROL_CFG     0x005c      // light control config(struct DHDEV_LIGHTCONTROL_CFG)
#define DH_DEV_3GFLOW_CFG           0x005d      // 3G flow info config(struct DHDEV_3GFLOW_INFO_CFG)
#define DH_DEV_IPV6_CFG             0x005e      // IPv6 config(struct DHDEV_IPV6_CFG)
#define DH_DEV_SNMP_CFG             0X005f      // Snmp config(struct DHDEV_NET_SNMP_CFG)
#define DH_DEV_SNAP_CONTROL_CFG     0x0060      // snap control config(struct DHDEV_SNAP_CONTROL_CFG)
#define DH_DEV_GPS_MODE_CFG         0X0061      // GPS mode config(struct DHDEV_GPS_MODE_CFG)
#define DH_DEV_SNAP_UPLOAD_CFG      0X0062      // Snap upload config(struct DHDEV_SNAP_UPLOAD_CFG)
#define DH_DEV_SPEED_LIMIT_CFG      0x0063      // Speed limit config(struct DHDEV_SPEED_LIMIT_CFG)
#define DH_DEV_ISCSI_CFG	    0x0064		// iSCSI config(struct DHDEV_ISCSI_CFG), need reboot
#define DH_DEV_WIRELESS_ROUTING_CFG  0x0065		// wifi config(struc DHDEV_WIRELESS_ROUTING_CFG)
#define DH_DEV_ENCLOSURE_CFG         0x0066      // enclosure config(stuct DHDEV_ENCLOSURE_CFG)
#define DH_DEV_ENCLOSURE_VERSION_CFG 0x0067     // enclosure version config(struct DHDEV_ENCLOSURE_VERSION_CFG)
#define DH_DEV_RIAD_EVENT_CFG        0x0068     // Raid event config(struct DHDEV_RAID_EVENT_CFG)
#define DH_DEV_FIRE_ALARM_CFG        0x0069     // fire alarm config(struct DHDEV_FIRE_ALARM_CFG)
#define DH_DEV_LOCALALARM_NAME_CFG   0x006a     // local alarm name config(string like "Name1&&name2&&name3...")
#define DH_DEV_URGENCY_RECORD_CFG    0x0070     // urgency storage config(struct DHDEV_URGENCY_RECORD_CFG)
#define DH_DEV_ELEVATOR_ATTRI_CFG    0x0071     // elevator parameter config(struct DHDEV_ELEVATOR_ATTRI_CFG)
#define DH_DEV_ATM_OVERLAY_CFG_EX   0x0072     // TM overlay function. For latest ATM series product only. 
	                                        // Support devices of 32-channel or higher.( struct DHDEV_ATM_OVERLAY_CONFIG_EX)
#define DH_DEV_MACFILTER_CFG		 0x0073		// MAC filter config(struct DHDEV_MACFILTER_CFG)
#define DH_DEV_MACIPFILTER_CFG		 0x0074		// MAC,IP filter config(need ip,mac one to one corresponding)(struct DHDEV_MACIPFILTER_CFG)
#define DH_DEV_STREAM_ENCRYPT_TIME_CFG 0x0075   // stream encrypt(encryot plan)(struct DHEDV_STREAM_ENCRYPT)
#define DH_DEV_LIMIT_BIT_RATE_CFG    0x0076     // limit bit rate config(struct DHDEV_LIMIT_BIT_RATE) 
#define DH_DEV_SNAP_CFG_EX			 0x0077		// snap extern config(struct DHDEV_SNAP_CFG_EX)
#define DH_DEV_DECODER_URL_CFG		0x0078		// decoder url config(struct DHDEV_DECODER_URL_CFG)
#define DH_DEV_TOUR_ENABLE_CFG		0x0079		// toyr enable config(struct DHDEV_TOUR_ENABLE_CFG)
#define DH_DEV_VICHILE_WIFI_AP_CFG_EX 0x007a    // wifi ap extern config(struct DHDEV_VEHICLE_WIFI_AP_CFG_EX)
#define DH_DEV_ENCODER_CFG_EX         0x007b    // encoder extern config(struct DEV_ENCODER_CFG_EX)
#define DH_DEV_USER_END_CFG			1000

// Alarm type, corresponding to CLIENT_StartListen
#define DH_COMM_ALARM				0x1100		// General alarm(Including external alarm, video loss and motion detection)
#define DH_SHELTER_ALARM			0x1101		// Camera masking alarm 
#define DH_DISK_FULL_ALARM			0x1102		// HDD full alarm 
#define DH_DISK_ERROR_ALARM			0x1103		// HDD malfunction alarm 
#define DH_SOUND_DETECT_ALARM		0x1104		// Audio detection alarm 
#define DH_ALARM_DECODER_ALARM		0x1105		// Alarm decoder alarm 

// Extensive alarm type,corresponding to CLIENT_StartListenEx
#define DH_ALARM_ALARM_EX			0x2101		// External alarm 
#define DH_MOTION_ALARM_EX			0x2102		// Motion detection alarm 
#define DH_VIDEOLOST_ALARM_EX		0x2103		// Video loss alarm 
#define DH_SHELTER_ALARM_EX			0x2104		// Camera masking alarm 
#define DH_SOUND_DETECT_ALARM_EX	0x2105		// Audio detection alarm 
#define DH_DISKFULL_ALARM_EX		0x2106		// HDD full alarm 
#define DH_DISKERROR_ALARM_EX		0x2107		// HDD malfunction alarm 
#define DH_ENCODER_ALARM_EX			0x210A		// Encoder alarm 
#define DH_URGENCY_ALARM_EX			0x210B		// Emergency alarm 
#define DH_WIRELESS_ALARM_EX		0x210C		// Wireless alarm 
#define DH_NEW_SOUND_DETECT_ALARM_EX 0x210D		// New auido detection alarm. Please refer to DH_NEW_SOUND_ALARM_STATE for alarm information structure;
#define DH_ALARM_DECODER_ALARM_EX	0x210E		// Alarm decoder alarm 
#define DH_DECODER_DECODE_ABILITY	0x210F		// NVD:Decoding capacity
#define DH_FDDI_DECODER_ABILITY		0x2110		// Fiber encoder alarm
#define DH_PANORAMA_SWITCH_ALARM_EX	0x2111		// Panorama switch alarm
#define DH_LOSTFOCUS_ALARM_EX		0x2112		// Lost focus alarm
#define DH_OEMSTATE_EX				0x2113		// oem state
#define DH_DSP_ALARM_EX				0x2114		// DSP alarm
#define DH_ATMPOS_BROKEN_EX			0x2115		// atm and pos disconnection alarm, 0:disconnection 1:connection
#define DH_RECORD_CHANGED_EX		0x2116		// Record state changed alarm
#define DH_CONFIG_CHANGED_EX		0x2117		// Device config changed alarm
#define DH_DEVICE_REBOOT_EX			0x2118		// Device rebooting alarm
#define DH_WINGDING_ALARM_EX        0x2119      // CoilFault alarm
#define DH_TRAF_CONGESTION_ALARM_EX 0x211A      // traffic congestion alarm
#define DH_TRAF_EXCEPTION_ALARM_EX  0x211B      // traffic exception alarm
#define DH_EQUIPMENT_FILL_ALARM_EX  0x211C      // FlashFault alarm
#define DH_ALARM_ARM_DISARM_STATE	0x211D		// alarm arm disarm 
#define DH_ALARM_ACC_POWEROFF       0x211E      // ACC power off alarm
#define DH_ALARM_3GFLOW_EXCEED      0x211F      // Alarm of 3G flow exceed(see struct DHDEV_3GFLOW_EXCEED_STATE_INFO)
#define DH_ALARM_SPEED_LIMIT        0x2120      // Speed limit alarm 
#define DH_ALARM_VEHICLE_INFO_UPLOAD  0x2121    // Vehicle information uploading 
#define DH_STATIC_ALARM_EX          0x2122      // Static detection alarm
#define DH_PTZ_LOCATION_EX          0x2123      // ptz location info
#define DH_ALARM_CARD_RECORD_UPLOAD	0x2124		// card record info(struct ALARM_CARD_RECORD_INFO_UPLOAD)
#define DH_ALARM_ATM_INFO_UPLOAD	0x2125		// ATM trade info(struct ALARM_ATM_INFO_UPLOAD)
#define DH_ALARM_ENCLOSURE          0x2126      // enclosure alarm(struct ALARM_ENCLOSURE_INFO)
#define DH_ALARM_SIP_STATE          0x2127      // SIP state alarm(struct ALARM_SIP_STATE)
#define DH_ALARM_RAID_STATE         0x2128      // RAID state alarm(struct ALARM_RAID_INFO)
#define DH_ALARM_CROSSING_SPEED_LIMIT 0x2129	// crossing speed limit alarm(struct ALARM_SPEED_LIMIT)
#define DH_ALARM_OVER_LOADING         0x212A      // over loading alarm(struct ALARM_OVER_LOADING)
#define DH_ALARM_HARD_BRAKING         0x212B      // hard brake alarm(struct ALARM_HARD_BRAKING)
#define DH_ALARM_SMOKE_SENSOR         0x212C      // smoke sensor alarm(struct ALARM_SMOKE_SENSOR)
#define DH_ALARM_TRAFFIC_LIGHT_FAULT 0x212D     // traffic light fault alarm(struct ALARM_TRAFFIC_LIGHT_FAULT) 
#define DH_ALARM_TRAFFIC_FLUX_STAT   0x212E     // traffic flux alarm(struct ALARM_TRAFFIC_FLUX_LANE_INFO)
#define DH_ALARM_CAMERA_MOVE         0x212F     // camera move alarm(struct ALARM_CAMERA_MOVE_INFO)
#define DH_ALARM_DETAILEDMOTION      0x2130     // detailed motion alarm(struct ALARM_DETAILEDMOTION_CHNL_INFO)
#define DH_ALARM_STORAGE_FAILURE     0x2131     // storage failure alarm(struct ALARM_STORAGE_FAILURE)
#define DH_ALARM_FRONTDISCONNECT     0x2132     // front IPC disconnect alarm(struct ALARM_FRONTDISCONNET_INFO)
#define DH_ALARM_ALARM_EX_REMOTE     0x2133     // remote external alarm
#define DH_ALARM_BATTERYLOWPOWER     0x2134     // battery low power alarm(struct ALARM_BATTERYLOWPOWER_INFO)
#define DH_ALARM_TEMPERATURE         0x2135     // temperature alarm(struct ALARM_TEMPERATURE_INFO)
#define DH_ALARM_TIREDDRIVE          0x2136     // tired drive alarm(struct ALARM_TIREDDRIVE_INFO)
#define DH_ALARM_LOST_RECORD         0x2137     // Alarm of record loss (corresponding structure ALARM_LOST_RECORD)
#define DH_ALARM_HIGH_CPU            0x2138     // Alarm of High CPU Occupancy rate (corresponding structure ALARM_HIGH_CPU) 
#define DH_ALARM_LOST_NETPACKET      0x2139     // Alarm of net package loss (corresponding structure ALARM_LOST_NETPACKET)
#define DH_ALARM_HIGH_MEMORY         0x213A     // Alarm of high memory occupancy rate(corresponding structure ALARM_HIGH_MEMORY)
#define DH_LONG_TIME_NO_OPERATION	 0x213B	    // WEB user have no operation for long time (no extended info)
#define DH_BLACKLIST_SNAP            0x213C     // blacklist snap(corresponding to DH_BLACKLIST_SNAP_INFO)         
#define DH_ALARM_DISK				 0x213E		// alarm of disk(corresponding to ALARM_DISK_INFO)
#define	DH_ALARM_FILE_SYSTEM		 0x213F		// alarm of file systemcorresponding to ALARM_FILE_SYSTEM_INFO)
#define DH_ALARM_IVS                 0x2140     // alarm of ivs(corresponding to ALARM_IVS_INFO)
#define DH_ALARM_GOODS_WEIGHT_UPLOAD 0x2141		// goods weight (corresponding to ALARM_GOODS_WEIGHT_UPLOAD_INFO)
#define DH_ALARM_GOODS_WEIGHT		 0x2142		// alarm of goods weight(corresponding to ALARM_GOODS_WEIGHT_INFO)
#define DH_GPS_STATUS                0x2143     // GPS orientation info(corresponding to NET_GPS_STATUS_INFO)
#define DH_ALARM_DISKBURNED_FULL     0x2144     // alarm disk burned full(corresponding to ALARM_DISKBURNED_FULL_INFO)
#define DH_ALARM_STORAGE_LOW_SPACE	 0x2145		// storage low space(corresponding to ALARM_STORAGE_LOW_SPACE_INFO)
#define DH_ALARM_DISK_FLUX			 0x2160		// disk flux abnormal(corresponding to ALARM_DISK_FLUX)
#define DH_ALARM_NET_FLUX			 0x2161		// net flux abnormal(corresponding to ALARM_NET_FLUX)
#define	DH_ALARM_FAN_SPEED			 0x2162		// fan speed abnormal(corresponding to ALARM_FAN_SPEED)
#define DH_ALARM_STORAGE_FAILURE_EX  0x2163     // storage mistake(corresponding to ALARM_STORAGE_FAILURE_EX)
#define	DH_ALARM_RECORD_FAILED		 0x2164		// record abnormal(corresponding to ALARM_RECORD_FAILED_INFO)
#define DH_ALARM_STORAGE_BREAK_DOWN	 0x2165		// storage break down(corresponding to ALARM_STORAGE_BREAK_DOWN_INFO)
#define DH_ALARM_VIDEO_ININVALID     0x2166     // ALARM_VIDEO_ININVALID_INFO
#define DH_ALARM_VEHICLE_TURNOVER	 0x2167		// vehicle turnover arm event(struct ALARM_VEHICEL_TURNOVER_EVENT_INFO)
#define DH_ALARM_VEHICLE_COLLISION	 0x2168     // vehicle collision event(struct ALARM_VEHICEL_COLLISION_EVENT_INFO)
#define DH_ALARM_VEHICLE_CONFIRM     0x2169     // vehicle confirm information event(struct ALARM_VEHICEL_CONFIRM_INFO)
#define DH_ALARM_VEHICLE_LARGE_ANGLE 0x2170     // vehicle camero large angle event(struct ALARM_VEHICEL_LARGE_ANGLE)
#define DH_ALARM_TALKING_INVITE		 0x2171		// device talking invite event (struct ALARM_TALKING_INVITE_INFO)
#define DH_ALARM_ALARM_EX2			 0x2175		// local alarm event (struct ALARM_ALARM_INFO_EX2��upgrade DH_ALARM_ALARM_EX)
#define DH_ALARM_VIDEO_TIMING        0x2176     // video timing detecting event(struct ALARM_VIDEO_TIMING)
#define DH_ALARM_COMM_PORT			 0x2177     // COM event(struct ALARM_COMM_PORT_EVENT_INFO)
#define DH_ALARM_AUDIO_ANOMALY       0x2178     // audio anomaly event(struct ALARM_AUDIO_ANOMALY)
#define DH_ALARM_AUDIO_MUTATION      0x2179     // audio mutation event(struct ALARM_AUDIO_MUTATION)
#define DH_EVENT_TYREINFO            0x2180     // Tyre information report event (struct EVENT_TYRE_INFO)
#define DH_ALARM_POWER_ABNORMAL      0X2181     // Redundant power supplies abnormal alarm(struct ALARM_POWER_ABNORMAL_INFO)
#define DH_EVENT_REGISTER_OFF		 0x2182		// On-board equipment active offline events(struct  EVENT_REGISTER_OFF_INFO)
#define DH_ALARM_NO_DISK             0x2183     // No hard disk alarm(struct ALARM_NO_DISK_INFO)
#define DH_ALARM_FALLING             0x2184     // The fall alarm(struct ALARM_FALLING_INFO)
#define DH_ALARM_PROTECTIVE_CAPSULE       0X2185           // �������¼�(��Ӧ�ṹ��ALARM_PROTECTIVE_CAPSULE_INFO)


#define DH_ALARM_STORAGE_NOT_EXIST   0x3167		// A storage group does not exist(struct ALARM_STORAGE_NOT_EXIST_INFO)
#define DH_ALARM_NET_ABORT			 0x3169		// Network fault event(struct ALARM_NETABORT_INFO)
#define DH_ALARM_IP_CONFLICT		 0x3170		// IP conflict event(struct ALARM_IP_CONFLICT_INFO)
#define DH_ALARM_MAC_CONFLICT		 0x3171		// MAC conflict event(struct ALARM_MAC_CONFLICT_INFO)
#define DH_ALARM_POWERFAULT			 0x3172		// power fault event(struct ALARM_POWERFAULT_INFO)
#define DH_ALARM_CHASSISINTRUDED	 0x3173		// Chassis intrusion, tamper alarm events(struct ALARM_CHASSISINTRUDED_INFO)
#define DH_ALARM_ALARMEXTENDED		 0x3174		// Native extension alarm events(struct ALARM_ALARMEXTENDED_INFO)

#define DH_ALARM_ARMMODE_CHANGE_EVENT		 0x3175		// Cloth removal state change events(struct ALARM_ARMMODE_CHANGE_INFO)
#define DH_ALARM_BYPASSMODE_CHANGE_EVENT	 0x3176		// The bypass state change events(struct ALARM_BYPASSMODE_CHANGE_INFO)

#define DH_ALARM_ACCESS_CTL_NOT_CLOSE		0x3177		// Entrance guard did not close events(struct ALARM_ACCESS_CTL_NOT_CLOSE_INFO)
#define DH_ALARM_ACCESS_CTL_BREAK_IN		0x3178		// break-in event(struct ALARM_ACCESS_CTL_BREAK_IN_INFO)
#define DH_ALARM_ACCESS_CTL_REPEAT_ENTER	0x3179		//access Again and again event(struct ALARM_ACCESS_CTL_REPEAT_ENTER_INFO)
#define DH_ALARM_ACCESS_CTL_DURESS			0x3180		// Stress CARDS event(struct ALARM_ACCESS_CTL_DURESS_INFO)
#define DH_ALARM_ACCESS_CTL_EVENT			0x3181		// Access event(struct ALARM_ACCESS_CTL_EVENT_INFO)

#define DH_URGENCY_ALARM_EX2		0x3182		// Emergency ALARM EX2(upgrade DH_URGENCY_ALARM_EX,struct ALARM_URGENCY_ALARM_EX2, Artificially triggered emergency, general processing is linked external communication function requests for help
#define DH_ALARM_INPUT_SOURCE_SIGNAL		0x3183		// Alarm input source signal events (as long as there is input will generate the event, whether to play the current mode, unable to block, struct ALARM_INPUT_SOURCE_SIGNAL_INFO)
#define DH_ALARM_ANALOGALARM_EVENT	0x3184		//  analog alarm(struct ALARM_ANALOGALARM_EVENT_INFO)
#define DH_ALARM_ACCESS_CTL_STATUS	0x3185		// �Ž�״̬�¼�(��Ӧ�ṹ��ALARM_ACCESS_CTL_STATUS_INFO)

// Event type
#define DH_CONFIG_RESULT_EVENT_EX	0x3000		// Modify the return code of the setup. Please refer to DEV_SET_RESULT for returned structure.
#define DH_REBOOT_EVENT_EX			0x3001		//  Device reboot event. Current modification becomes valid unitl sending out the reboot command. 
#define DH_AUTO_TALK_START_EX		0x3002		// Device automatically invites to begin audio talk 
#define DH_AUTO_TALK_STOP_EX		0x3003		// Device actively stop audio talk 
#define DH_CONFIG_CHANGE_EX			0x3004		// Device setup changes.
#define DH_IPSEARCH_EVENT_EX        0x3005      // IP search event, the return value format is:DevName:::Manufacturer::MAC:: IP:: Port&& DevName:: Manufacturer::MAC:: IP:: Port&&?-
#define DH_AUTO_RECONNECT_FAILD     0x3006      // reconnect failed
#define DH_REALPLAY_FAILD_EVENT     0x3007      // real play failed
#define DH_PLAYBACK_FAILD_EVENT     0x3008      // playback failed
#define DH_IVS_TRAFFIC_REALFLOWINFO 0x3009      // traffic real flow info ALARM_IVS_TRAFFIC_REALFLOW_INFO

// Alarm type of alarm upload function,corresponding to CLIENT_StartService.NEW_ALARM_UPLOAD structure.
#define DH_UPLOAD_ALARM					0x4000		// External alarm 		
#define DH_UPLOAD_MOTION_ALARM			0x4001		// Motion detection alarm 
#define DH_UPLOAD_VIDEOLOST_ALARM		0x4002		// Video loss alarm 
#define DH_UPLOAD_SHELTER_ALARM			0x4003		// Camera masking alarm 
#define DH_UPLOAD_SOUND_DETECT_ALARM	0x4004		// Audio detection alarm 
#define DH_UPLOAD_DISKFULL_ALARM		0x4005		// HDD full alarm 
#define DH_UPLOAD_DISKERROR_ALARM		0x4006		// HDD malfunction alarm 
#define DH_UPLOAD_ENCODER_ALARM			0x4007		// Encoder alarm 
#define DH_UPLOAD_DECODER_ALARM			0x400B		// Alarm decoder alarm 
#define DH_UPLOAD_EVENT					0x400C		// Scheduled upload 
#define DH_UPLOAD_IVS					0x400D		// intelligent alarm,corresponding to ALARM_UPLOAD_IVS_INFO
#define DH_UPLOAD_SMOKESENSOR_ALARM 0x400E		// Smoke alarm, struct ALARM_UPLOAD_SMOKESENSOR_INFO

// Asynchronous interface callback type
#define RESPONSE_DECODER_CTRL_TV	0x00000001		// refer to CLIENT_CtrlDecTVScreen interface
#define RESPONSE_DECODER_SWITCH_TV	0x00000002		// refer to CLIENT_SwitchDecTVEncoder interface
#define RESPONSE_DECODER_PLAYBACK	0x00000003		// refer to CLIENT_DecTVPlayback interface

#define RESPONSE_EXCHANGE_DATA		0X00000004	// correspoding CLIENT_ExchangeData interface
#define RESPONSE_ASYN_QUERY_RECORDFILE 0X00000005  // correspoding CLIENT_StartQueryRecordFile interface
// CLIENT_FileTransmit Interface transmission file type
#define DH_DEV_UPGRADEFILETRANS_START	0x0000		// Begin sending update file(Corresponding structure DHDEV_UPGRADE_FILE_INFO)
#define DH_DEV_UPGRADEFILETRANS_SEND	0x0001		// Send update file  
#define DH_DEV_UPGRADEFILETRANS_STOP	0x0002		// Stop sending update file
#define DH_DEV_BLACKWHITETRANS_START  	0x0003    	// begin to send blackwhite list(Corresponding structure DHDEV_BLACKWHITE_LIST_INFO)
#define DH_DEV_BLACKWHITETRANS_SEND   	0x0004    	// send blackwhite list
#define DH_DEV_BLACKWHITETRANS_STOP   	0x0005    	// stop to send blackwhite list
#define DH_DEV_BLACKWHITE_LOAD        	0x0006    	// blackwhite list load (Corresponding structure DHDEV_LOAD_BLACKWHITE_LIST_INFO)
#define DH_DEV_BLACKWHITE_LOAD_STOP   	0x0007    	// blackwhite list load stop
#define DH_DEV_FILETRANS_STOP			0x002B		// Stop file upload
#define DH_DEV_FILETRANS_BURN			0x002C		// Burn File Upload

// Resolution list. Use to AND & OR of resolution subnet mask 
#define	DH_CAPTURE_SIZE_D1			0x00000001
#define DH_CAPTURE_SIZE_HD1			0x00000002
#define DH_CAPTURE_SIZE_BCIF		0x00000004
#define DH_CAPTURE_SIZE_CIF			0x00000008
#define DH_CAPTURE_SIZE_QCIF		0x00000010	
#define DH_CAPTURE_SIZE_VGA			0x00000020	
#define DH_CAPTURE_SIZE_QVGA		0x00000040
#define DH_CAPTURE_SIZE_SVCD		0x00000080
#define DH_CAPTURE_SIZE_QQVGA		0x00000100
#define DH_CAPTURE_SIZE_SVGA		0x00000200
#define DH_CAPTURE_SIZE_XVGA		0x00000400
#define DH_CAPTURE_SIZE_WXGA		0x00000800
#define DH_CAPTURE_SIZE_SXGA		0x00001000
#define DH_CAPTURE_SIZE_WSXGA		0x00002000   
#define DH_CAPTURE_SIZE_UXGA		0x00004000
#define DH_CAPTURE_SIZE_WUXGA       0x00008000
#define DH_CAPTURE_SIZE_LFT         0x00010000
#define DH_CAPTURE_SIZE_720		    0x00020000
#define DH_CAPTURE_SIZE_1080		0x00040000
#define DH_CAPTURE_SIZE_1_3M        0x00080000
#define DH_CAPTURE_SIZE_2M			0x00100000
#define DH_CAPTURE_SIZE_5M			0x00200000
#define DH_CAPTURE_SIZE_3M			0x00400000	
#define DH_CAPTURE_SIZE_5_0M        0x00800000
#define DH_CPTRUTE_SIZE_1_2M 		0x01000000
#define DH_CPTRUTE_SIZE_1408_1024   0x02000000
#define DH_CPTRUTE_SIZE_8M	        0x04000000	
#define DH_CPTRUTE_SIZE_2560_1920   0x08000000
#define DH_CAPTURE_SIZE_960H        0x10000000	
#define DH_CAPTURE_SIZE_960_720     0x20000000				

// Encode mode list. Use to work AND & OR operation of encode mode mask.
#define DH_CAPTURE_COMP_DIVX_MPEG4	0x00000001
#define DH_CAPTURE_COMP_MS_MPEG4 	0x00000002
#define DH_CAPTURE_COMP_MPEG2		0x00000004
#define DH_CAPTURE_COMP_MPEG1		0x00000008
#define DH_CAPTURE_COMP_H263		0x00000010
#define DH_CAPTURE_COMP_MJPG		0x00000020
#define DH_CAPTURE_COMP_FCC_MPEG4	0x00000040
#define DH_CAPTURE_COMP_H264		0x00000080

// Alarm activation operation. Use to work AND & OR operation of alarm activation operation.
#define DH_ALARM_UPLOAD				0x00000001
#define DH_ALARM_RECORD				0x00000002
#define DH_ALARM_PTZ				0x00000004
#define DH_ALARM_MAIL				0x00000008
#define DH_ALARM_TOUR				0x00000010
#define DH_ALARM_TIP				0x00000020
#define DH_ALARM_OUT				0x00000040
#define DH_ALARM_FTP_UL				0x00000080
#define DH_ALARM_BEEP				0x00000100
#define DH_ALARM_VOICE				0x00000200
#define DH_ALARM_SNAP				0x00000400

// Restore default setup mask. Can use to AND & OR operation
#define DH_RESTORE_COMMON			0x00000001	// General setup
#define DH_RESTORE_CODING			0x00000002	// Encode setup
#define DH_RESTORE_VIDEO			0x00000004	// Record setup
#define DH_RESTORE_COMM				0x00000008	// COM setup
#define DH_RESTORE_NETWORK			0x00000010	//network setup
#define DH_RESTORE_ALARM			0x00000020	// Alarm setup
#define DH_RESTORE_VIDEODETECT		0x00000040	// Video detection
#define DH_RESTORE_PTZ				0x00000080	// PTZ control 
#define DH_RESTORE_OUTPUTMODE		0x00000100	// Output mode
#define DH_RESTORE_CHANNELNAME		0x00000200	// Channel name
#define DH_RESTORE_VIDEOINOPTIONS   0x00000400  // Camera attribute
#define DH_RESTORE_CPS              0x00000800  // TrafficSnapshot
#define DH_RESTORE_INTELLIGENT      0x00001000  // Intelligent Component
#define DH_RESTORE_REMOTEDEVICE     0x00002000  // Remote device configuration
#define DH_RESTORE_DECODERVIDEOOUT  0x00004000  // decode video out
#define DH_RESTORE_LINKMODE         0x00008000  // link mode
#define DH_RESTORE_COMPOSITE        0x00010000  // split screen   
#define DH_RESTORE_ALL				0x80000000	// Reset all

// PTZ property list
// Lower four bytes subnet mask
#define PTZ_DIRECTION				0x00000001	// Direction
#define PTZ_ZOOM					0x00000002	// Zoom
#define PTZ_FOCUS					0x00000004	// Focus
#define PTZ_IRIS					0x00000008	// Aperture
#define PTZ_ALARM					0x00000010	// Alarm function 
#define PTZ_LIGHT					0x00000020	// Light 
#define PTZ_SETPRESET				0x00000040	// Set preset 
#define PTZ_CLEARPRESET				0x00000080	// Delete preset
#define PTZ_GOTOPRESET				0x00000100	// Go to a preset
#define PTZ_AUTOPANON				0x00000200	// Enable pan
#define PTZ_AUTOPANOFF				0x00000400	// isable pan
#define PTZ_SETLIMIT				0x00000800	// Set limit
#define PTZ_AUTOSCANON				0x00001000	// Enable auto scan
#define PTZ_AUTOSCANOFF				0x00002000	// Disable auto scan 
#define PTZ_ADDTOUR					0x00004000	// Add tour point
#define PTZ_DELETETOUR				0x00008000	// Delete tour point
#define PTZ_STARTTOUR				0x00010000	// Begin tour
#define PTZ_STOPTOUR				0x00020000	// Stop tour
#define PTZ_CLEARTOUR				0x00040000	// Delete tour
#define PTZ_SETPATTERN				0x00080000	// Set pattern
#define PTZ_STARTPATTERN			0x00100000	// Enbale pattern
#define PTZ_STOPPATTERN				0x00200000	// Disable pattern
#define PTZ_CLEARPATTERN			0x00400000	// Delete pattern
#define PTZ_POSITION				0x00800000	// Position 
#define PTZ_AUX						0x01000000	// auxiliary button 
#define PTZ_MENU					0x02000000	// Speed dome menu 
#define PTZ_EXIT					0x04000000	// Exit speed dome menu 
#define PTZ_ENTER					0x08000000	// Confirm
#define PTZ_ESC						0x10000000	// Cancel 
#define PTZ_MENUUPDOWN				0x20000000	// Menu up/down
#define PTZ_MENULEFTRIGHT			0x40000000	// Menu left/right 
#define PTZ_OPT_NUM					0x80000000	// Operation amount
// Higher four bytes subnet mask
#define PTZ_DEV						0x00000001	// PTZ control 
#define PTZ_MATRIX					0x00000002	// Matrix control 

// Snapshot video encode type
#define CODETYPE_MPEG4				0
#define CODETYPE_H264				1
#define CODETYPE_JPG				2

// Bit stream control control list
#define DH_CAPTURE_BRC_CBR			0
#define DH_CAPTURE_BRC_VBR			1
//#define DH_CAPTURE_BRC_MBR		2

//The frame type mask definition
#define FRAME_TYPE_MOTION            0x00000001  // MD frame

// the type of intelligent analysis event 
#define EVENT_IVS_ALL						0x00000001		// subscription all event
#define EVENT_IVS_CROSSLINEDETECTION		0x00000002		// cross line event
#define EVENT_IVS_CROSSREGIONDETECTION		0x00000003		// cross region event
#define EVENT_IVS_PASTEDETECTION			0x00000004		// past event
#define EVENT_IVS_LEFTDETECTION				0x00000005		// left event 
#define EVENT_IVS_STAYDETECTION				0x00000006		// stay event
#define EVENT_IVS_WANDERDETECTION			0x00000007		// wander event
#define EVENT_IVS_PRESERVATION				0x00000008		// reservation event 
#define EVENT_IVS_MOVEDETECTION				0x00000009		// move event
#define EVENT_IVS_TAILDETECTION				0x0000000A		// tail event
#define EVENT_IVS_RIOTERDETECTION			0x0000000B		// rioter event
#define EVENT_IVS_FIREDETECTION				0x0000000C		// fire event
#define EVENT_IVS_SMOKEDETECTION			0x0000000D		// smoke event
#define EVENT_IVS_FIGHTDETECTION			0x0000000E		// fight event
#define EVENT_IVS_FLOWSTAT					0x0000000F		// flow stat event
#define EVENT_IVS_NUMBERSTAT				0x00000010		// number stat event
#define EVENT_IVS_CAMERACOVERDDETECTION		0x00000011		// camera cover event
#define EVENT_IVS_CAMERAMOVEDDETECTION		0x00000012		// camera move event
#define EVENT_IVS_VIDEOABNORMALDETECTION	0x00000013		// video abnormal event
#define EVENT_IVS_VIDEOBADDETECTION			0x00000014		// video bad event
#define EVENT_IVS_TRAFFICCONTROL			0x00000015		// traffic control event
#define EVENT_IVS_TRAFFICACCIDENT			0x00000016		// traffic accident event
#define EVENT_IVS_TRAFFICJUNCTION			0x00000017		// traffic junction event
#define EVENT_IVS_TRAFFICGATE				0x00000018		// traffic gate event
#define EVENT_TRAFFICSNAPSHOT				0x00000019		// traffic snapshot
#define EVENT_IVS_FACEDETECT                0x0000001A      // face detection
#define EVENT_IVS_TRAFFICJAM                0x0000001B      // traffic-Jam
#define EVENT_IVS_TRAFFIC_RUNREDLIGHT		0x00000100		// traffic-RunRedLight
#define EVENT_IVS_TRAFFIC_OVERLINE			0x00000101		// traffic-Overline
#define EVENT_IVS_TRAFFIC_RETROGRADE		0x00000102		// traffic-Retrograde
#define EVENT_IVS_TRAFFIC_TURNLEFT			0x00000103		// traffic-TurnLeft
#define EVENT_IVS_TRAFFIC_TURNRIGHT			0x00000104		// traffic-TurnRight	
#define EVENT_IVS_TRAFFIC_UTURN				0x00000105		// traffic-Uturn
#define EVENT_IVS_TRAFFIC_OVERSPEED			0x00000106		// traffic-Overspeed
#define EVENT_IVS_TRAFFIC_UNDERSPEED		0x00000107		// traffic-Underspeed
#define EVENT_IVS_TRAFFIC_PARKING           0x00000108      // traffic-Parking
#define EVENT_IVS_TRAFFIC_WRONGROUTE        0x00000109      // traffic-WrongRoute
#define EVENT_IVS_TRAFFIC_CROSSLANE         0x0000010A      // traffic-CrossLane
#define EVENT_IVS_TRAFFIC_OVERYELLOWLINE    0x0000010B      // traffic-OverYellowLine
#define EVENT_IVS_TRAFFIC_DRIVINGONSHOULDER 0x0000010C      // traffic-DrivingOnShoulder   
#define EVENT_IVS_TRAFFIC_YELLOWPLATEINLANE 0x0000010E      // traffic-YellowPlateInLane
#define EVENT_IVS_TRAFFIC_PEDESTRAINPRIORITY 0x0000010F		// Traffic offense-Pedestral has higher priority at the  crosswalk
#define EVENT_IVS_CROSSFENCEDETECTION       0x0000011F      // cross fence 
#define EVENT_IVS_ELECTROSPARKDETECTION     0X00000110      // ElectroSpark event 
#define EVENT_IVS_TRAFFIC_NOPASSING         0x00000111      // no passing
#define EVENT_IVS_ABNORMALRUNDETECTION      0x00000112      // abnormal run
#define EVENT_IVS_RETROGRADEDETECTION       0x00000113      // retrograde
#define EVENT_IVS_INREGIONDETECTION         0x00000114      // in region detection
#define EVENT_IVS_TAKENAWAYDETECTION        0x00000115      // taking away things
#define EVENT_IVS_PARKINGDETECTION          0x00000116      // parking
#define EVENT_IVS_FACERECOGNITION			0x00000117		// face recognition
#define EVENT_IVS_TRAFFIC_MANUALSNAP        0x00000118      // manual snap
#define EVENT_IVS_TRAFFIC_FLOWSTATE			0x00000119		// traffic flow state
#define EVENT_IVS_TRAFFIC_STAY				0x0000011A		// traffic stay
#define EVENT_IVS_TRAFFIC_VEHICLEINROUTE	0x0000011B		// traffic vehicle route
#define EVENT_ALARM_MOTIONDETECT            0x0000011C      // motion detect
#define EVENT_ALARM_LOCALALARM              0x0000011D      // local alarm
#define EVENT_IVS_PRISONERRISEDETECTION		0x0000011E		// prisoner rise detect
#define EVENT_IVS_TRAFFIC_TOLLGATE			0x00000120		// traffic tollgate
#define EVENT_IVS_DENSITYDETECTION			0x00000121      // density detection of persons
#define EVENT_IVS_VIDEODIAGNOSIS            0x00000122		// video diagnosis result
#define EVENT_IVS_QUEUEDETECTION            0x00000123      // queue detection
#define EVENT_IVS_TRAFFIC_VEHICLEINBUSROUTE 0x00000124      // take up in bus route
#define EVENT_IVS_TRAFFIC_BACKING           0x00000125      // illegal astern 
#define EVENT_IVS_AUDIO_ABNORMALDETECTION   0x00000126      // audio abnormity
#define EVENT_IVS_TRAFFIC_RUNYELLOWLIGHT    0x00000127      // run yellow light
#define EVENT_IVS_CLIMBDETECTION            0x00000128      // climb detection 
#define EVENT_IVS_LEAVEDETECTION            0x00000129      // leave detection
#define EVENT_IVS_TRAFFIC_PARKINGONYELLOWBOX 0x0000012A	    // parking on yellow box
#define EVENT_IVS_TRAFFIC_PARKINGSPACEPARKING 0x0000012B	// parking space parking
#define EVENT_IVS_TRAFFIC_PARKINGSPACENOPARKING 0x0000012C	// parking space no parking
#define EVENT_IVS_TRAFFIC_PEDESTRAIN        0x0000012D      // passerby
#define EVENT_IVS_TRAFFIC_THROW             0x0000012E      // throw
#define EVENT_IVS_TRAFFIC_IDLE              0x0000012F      // idle
#define EVENT_ALARM_VEHICLEACC              0x00000130      // Vehicle ACC power outage alarm events
#define EVENT_ALARM_VEHICLE_TURNOVER		0x00000131		// Vehicle rollover alarm events
#define EVENT_ALARM_VEHICLE_COLLISION		0x00000132		// Vehicle crash alarm events
#define EVENT_ALARM_VEHICLE_LARGE_ANGLE		0x00000133		// On-board camera large Angle turn events
#define EVENT_IVS_TRAFFIC_PARKINGSPACEOVERLINE 0x00000134   // Parking line pressing events
#define EVENT_IVS_MULTISCENESWITCH			0x00000135		// Many scenes switching events
#define EVENT_IVS_TRAFFIC_RESTRICTED_PLATE  0X00000136      // Limited license plate event
#define EVENT_ALARM_ANALOGALARM                 0x00000150        // ģ��������ͨ���ı����¼�(��ӦDEV_EVENT_ALARM_ANALOGALRM_INFO)

//Traffic statistics event using macros
#define FLOWSTAT_ADDR_NAME			16			//Has long place name

// interface CLIENT_OperateTrafficList, All kinds of string length definition
#define DH_TARFFIC_NAME_LEN					16				
#define DH_CREATE_TIME_LEN					32
#define DH_AUTHORITY_NUM					16

// 
#define EVENT_IVS_TRAFFIC_ALL				0x000001FF		// All event start with [TRAFFIC]
															// EVENT_IVS_TRAFFICCONTROL -> EVENT_TRAFFICSNAPSHOT
															// EVENT_IVS_TRAFFIC_RUNREDLIGHT -> EVENT_IVS_TRAFFIC_UNDERSPEED
// Error type code. Corresponding to the return value of CLIENT_GetLastError
#define _EC(x)						(0x80000000|x)
#define NET_NOERROR 				0			// No error 
#define NET_ERROR					-1			// Unknown error
#define NET_SYSTEM_ERROR			_EC(1)		// Windows system error
#define NET_NETWORK_ERROR			_EC(2)		// Protocol error it may result from network timeout
#define NET_DEV_VER_NOMATCH			_EC(3)		// Device protocol does not match 
#define NET_INVALID_HANDLE			_EC(4)		// Handle is invalid
#define NET_OPEN_CHANNEL_ERROR		_EC(5)		// Failed to open channel 
#define NET_CLOSE_CHANNEL_ERROR		_EC(6)		// Failed to close channel 
#define NET_ILLEGAL_PARAM			_EC(7)		// User parameter is illegal 
#define NET_SDK_INIT_ERROR			_EC(8)		// SDK initialization error 
#define NET_SDK_UNINIT_ERROR		_EC(9)		// SDK clear error 
#define NET_RENDER_OPEN_ERROR		_EC(10)		// Error occurs when apply for render resources.
#define NET_DEC_OPEN_ERROR			_EC(11)		// Error occurs when opening the decoder library 
#define NET_DEC_CLOSE_ERROR			_EC(12)		// Error occurs when closing the decoder library 
#define NET_MULTIPLAY_NOCHANNEL		_EC(13)		// The detected channel number is 0 in multiple-channel preview. 
#define NET_TALK_INIT_ERROR			_EC(14)		// Failed to initialize record library 
#define NET_TALK_NOT_INIT			_EC(15)		// The record library has not been initialized
#define	NET_TALK_SENDDATA_ERROR		_EC(16)		// Error occurs when sending out audio data 
#define NET_REAL_ALREADY_SAVING		_EC(17)		// The real-time has been protected.
#define NET_NOT_SAVING				_EC(18)		// The real-time data has not been save.
#define NET_OPEN_FILE_ERROR			_EC(19)		// Error occurs when opening the file.
#define NET_PTZ_SET_TIMER_ERROR		_EC(20)		// Failed to enable PTZ to control timer.
#define NET_RETURN_DATA_ERROR		_EC(21)		// Error occurs when verify returned data.
#define NET_INSUFFICIENT_BUFFER		_EC(22)		// There is no sufficient buffer.
#define NET_NOT_SUPPORTED			_EC(23)		// The current SDK does not support this fucntion.
#define NET_NO_RECORD_FOUND			_EC(24)		// There is no searched result.
#define NET_NOT_AUTHORIZED			_EC(25)		// You have no operation right.
#define NET_NOT_NOW					_EC(26)		// Can not operate right now. 
#define NET_NO_TALK_CHANNEL			_EC(27)		// There is no audio talk channel.
#define NET_NO_AUDIO				_EC(28)		// There is no audio.
#define NET_NO_INIT					_EC(29)		// The network SDK has not been initialized.
#define NET_DOWNLOAD_END			_EC(30)		// The download completed.
#define NET_EMPTY_LIST				_EC(31)		// There is no searched result.
#define NET_ERROR_GETCFG_SYSATTR	_EC(32)		// Failed to get system property setup.
#define NET_ERROR_GETCFG_SERIAL		_EC(33)		// Failed to get SN.
#define NET_ERROR_GETCFG_GENERAL	_EC(34)		// Failed to get general property.
#define NET_ERROR_GETCFG_DSPCAP		_EC(35)		// Failed to get DSP capacity description.
#define NET_ERROR_GETCFG_NETCFG		_EC(36)		// Failed to get network channel setup.
#define NET_ERROR_GETCFG_CHANNAME	_EC(37)		// Failed to get channel name.
#define NET_ERROR_GETCFG_VIDEO		_EC(38)		// Failed to get video property.
#define NET_ERROR_GETCFG_RECORD		_EC(39)		// Failed to get record setup
#define NET_ERROR_GETCFG_PRONAME	_EC(40)		// Failed to get decoder protocol name 
#define NET_ERROR_GETCFG_FUNCNAME	_EC(41)		// Failed to get 232 COM function name.
#define NET_ERROR_GETCFG_485DECODER	_EC(42)		// Failed to get decoder property
#define NET_ERROR_GETCFG_232COM		_EC(43)		// Failed to get 232 COM setup
#define NET_ERROR_GETCFG_ALARMIN	_EC(44)		// Failed to get external alarm input setup
#define NET_ERROR_GETCFG_ALARMDET	_EC(45)		// Failed to get motion detection alarm
#define NET_ERROR_GETCFG_SYSTIME	_EC(46)		// Failed to get device time
#define NET_ERROR_GETCFG_PREVIEW	_EC(47)		// Failed to get preview parameter
#define NET_ERROR_GETCFG_AUTOMT		_EC(48)		// Failed to get audio maintenance setup
#define NET_ERROR_GETCFG_VIDEOMTRX	_EC(49)		// Failed to get video matrix setup
#define NET_ERROR_GETCFG_COVER		_EC(50)		// Failed to get privacy mask zone setup
#define NET_ERROR_GETCFG_WATERMAKE	_EC(51)		// Failed to get video watermark setup
#define NET_ERROR_SETCFG_GENERAL	_EC(55)		// Failed to modify general property
#define NET_ERROR_SETCFG_NETCFG		_EC(56)		// Failed to modify channel setup
#define NET_ERROR_SETCFG_CHANNAME	_EC(57)		// Failed to modify channel name
#define NET_ERROR_SETCFG_VIDEO		_EC(58)		// Failed to modify video channel 
#define NET_ERROR_SETCFG_RECORD		_EC(59)		// Failed to modify record setup 
#define NET_ERROR_SETCFG_485DECODER	_EC(60)		// Failed to modify decoder property 
#define NET_ERROR_SETCFG_232COM		_EC(61)		// Failed to modify 232 COM setup 
#define NET_ERROR_SETCFG_ALARMIN	_EC(62)		// Failed to modify external input alarm setup
#define NET_ERROR_SETCFG_ALARMDET	_EC(63)		// Failed to modify motion detection alarm setup 
#define NET_ERROR_SETCFG_SYSTIME	_EC(64)		// Failed to modify device time 
#define NET_ERROR_SETCFG_PREVIEW	_EC(65)		// Failed to modify preview parameter
#define NET_ERROR_SETCFG_AUTOMT		_EC(66)		// Failed to modify auto maintenance setup 
#define NET_ERROR_SETCFG_VIDEOMTRX	_EC(67)		// Failed to modify video matrix setup 
#define NET_ERROR_SETCFG_COVER		_EC(68)		// Failed to modify privacy mask zone
#define NET_ERROR_SETCFG_WATERMAKE	_EC(69)		// Failed to modify video watermark setup 
#define NET_ERROR_SETCFG_WLAN		_EC(70)		// Failed to modify wireless network information 
#define NET_ERROR_SETCFG_WLANDEV	_EC(71)		// Failed to select wireless network device
#define NET_ERROR_SETCFG_REGISTER	_EC(72)		// Failed to modify the actively registration parameter setup.
#define NET_ERROR_SETCFG_CAMERA		_EC(73)		// Failed to modify camera property
#define NET_ERROR_SETCFG_INFRARED	_EC(74)		// Failed to modify IR alarm setup
#define NET_ERROR_SETCFG_SOUNDALARM	_EC(75)		// Failed to modify audio alarm setup
#define NET_ERROR_SETCFG_STORAGE    _EC(76)		// Failed to modify storage position setup
#define NET_AUDIOENCODE_NOTINIT		_EC(77)		// The audio encode port has not been successfully initialized. 
#define NET_DATA_TOOLONGH			_EC(78)		// The data are too long.
#define NET_UNSUPPORTED				_EC(79)		// The device does not support current operation. 
#define NET_DEVICE_BUSY				_EC(80)		// Device resources is not sufficient.
#define NET_SERVER_STARTED			_EC(81)		// The server has boot up 
#define NET_SERVER_STOPPED			_EC(82)		// The server has not fully boot up 
#define NET_LISTER_INCORRECT_SERIAL	_EC(83)		// Input serial number is not correct.
#define NET_QUERY_DISKINFO_FAILED	_EC(84)		// Failed to get HDD information.
#define NET_ERROR_GETCFG_SESSION	_EC(85)		// Failed to get connect session information.
#define NET_USER_FLASEPWD_TRYTIME	_EC(86)		// The password you typed is incorrect. You have exceeded the maximum number of retries.
#define NET_LOGIN_ERROR_PASSWORD	_EC(100)	// Password is not correct
#define NET_LOGIN_ERROR_USER		_EC(101)	// The account does not exist
#define NET_LOGIN_ERROR_TIMEOUT		_EC(102)	// Time out for log in returned value.
#define NET_LOGIN_ERROR_RELOGGIN	_EC(103)	// The account has logged in 
#define NET_LOGIN_ERROR_LOCKED		_EC(104)	// The account has been locked
#define NET_LOGIN_ERROR_BLACKLIST	_EC(105)	// The account bas been in the black list
#define NET_LOGIN_ERROR_BUSY		_EC(106)	// Resources are not sufficient. System is busy now.
#define NET_LOGIN_ERROR_CONNECT		_EC(107)	// Time out. Please check network and try again.
#define NET_LOGIN_ERROR_NETWORK		_EC(108)	// Network connection failed.
#define NET_LOGIN_ERROR_SUBCONNECT	_EC(109)	// Successfully logged in the device but can not create video channel. Please check network connection.
#define NET_LOGIN_ERROR_MAXCONNECT  _EC(110)    // exceed the max connect number
#define NET_LOGIN_ERROR_PROTOCOL3_ONLY _EC(111)	// protocol 3 support
#define NET_LOGIN_ERROR_UKEY_LOST	_EC(112)	// There is no USB or USB info error
#define NET_LOGIN_ERROR_NO_AUTHORIZED _EC(113)  // Client-end IP address has no right to login
#define NET_RENDER_SOUND_ON_ERROR	_EC(120)	// Error occurs when Render library open audio.
#define NET_RENDER_SOUND_OFF_ERROR	_EC(121)	// Error occurs when Render library close audio 
#define NET_RENDER_SET_VOLUME_ERROR	_EC(122)	// Error occurs when Render library control volume
#define NET_RENDER_ADJUST_ERROR		_EC(123)	// Error occurs when Render library set video parameter
#define NET_RENDER_PAUSE_ERROR		_EC(124)	// Error occurs when Render library pause play
#define NET_RENDER_SNAP_ERROR		_EC(125)	// Render library snapshot error
#define NET_RENDER_STEP_ERROR		_EC(126)	// Render library stepper error
#define NET_RENDER_FRAMERATE_ERROR	_EC(127)	// Error occurs when Render library set frame rate.
#define NET_RENDER_DISPLAYREGION_ERROR	_EC(128)// Error occurs when Render lib setting show region
#define NET_GROUP_EXIST				_EC(140)	// Group name has been existed.
#define	NET_GROUP_NOEXIST			_EC(141)	// The group name does not exist. 
#define NET_GROUP_RIGHTOVER			_EC(142)	// The group right exceeds the right list!
#define NET_GROUP_HAVEUSER			_EC(143)	// The group can not be removed since there is user in it!
#define NET_GROUP_RIGHTUSE			_EC(144)	// The user has used one of the group right. It can not be removed. 
#define NET_GROUP_SAMENAME			_EC(145)	// New group name has been existed
#define	NET_USER_EXIST				_EC(146)	// The user name has been existed
#define NET_USER_NOEXIST			_EC(147)	// The account does not exist.
#define NET_USER_RIGHTOVER			_EC(148)	// User right exceeds the group right. 
#define NET_USER_PWD				_EC(149)	// Reserved account. It does not allow to be modified.
#define NET_USER_FLASEPWD			_EC(150)	// password is not correct
#define NET_USER_NOMATCHING			_EC(151)	// Password is invalid
#define NET_USER_INUSE				_EC(152)	// account in use
#define NET_ERROR_GETCFG_ETHERNET	_EC(300)	// Failed to get network card setup.
#define NET_ERROR_GETCFG_WLAN		_EC(301)	// Failed to get wireless network information.
#define NET_ERROR_GETCFG_WLANDEV	_EC(302)	// Failed to get wireless network device.
#define NET_ERROR_GETCFG_REGISTER	_EC(303)	// Failed to get actively registration parameter.
#define NET_ERROR_GETCFG_CAMERA		_EC(304)	// Failed to get camera property 
#define NET_ERROR_GETCFG_INFRARED	_EC(305)	// Failed to get IR alarm setup
#define NET_ERROR_GETCFG_SOUNDALARM	_EC(306)	// Failed to get audio alarm setup
#define NET_ERROR_GETCFG_STORAGE    _EC(307)	// Failed to get storage position 
#define NET_ERROR_GETCFG_MAIL		_EC(308)	// Failed to get mail setup.
#define NET_CONFIG_DEVBUSY			_EC(309)	// Can not set right now. 
#define NET_CONFIG_DATAILLEGAL		_EC(310)	// The configuration setup data are illegal.
#define NET_ERROR_GETCFG_DST        _EC(311)    // Failed to get DST setup
#define NET_ERROR_SETCFG_DST        _EC(312)    // Failed to set DST 
#define NET_ERROR_GETCFG_VIDEO_OSD  _EC(313)    // Failed to get video osd setup.
#define NET_ERROR_SETCFG_VIDEO_OSD  _EC(314)    // Failed to set video osd 
#define NET_ERROR_GETCFG_GPRSCDMA   _EC(315)    // Failed to get CDMA\GPRS configuration
#define NET_ERROR_SETCFG_GPRSCDMA   _EC(316)    // Failed to set CDMA\GPRS configuration
#define NET_ERROR_GETCFG_IPFILTER   _EC(317)    // Failed to get IP Filter configuration
#define NET_ERROR_SETCFG_IPFILTER   _EC(318)    // Failed to set IP Filter configuration
#define NET_ERROR_GETCFG_TALKENCODE _EC(319)    // Failed to get Talk Encode configuration
#define NET_ERROR_SETCFG_TALKENCODE _EC(320)    // Failed to set Talk Encode configuration
#define NET_ERROR_GETCFG_RECORDLEN  _EC(321)    // Failed to get The length of the video package configuration
#define NET_ERROR_SETCFG_RECORDLEN  _EC(322)    // Failed to set The length of the video package configuration
#define	NET_DONT_SUPPORT_SUBAREA	_EC(323)	// Not support Network hard disk partition
#define	NET_ERROR_GET_AUTOREGSERVER	_EC(324)	// Failed to get the register server information
#define	NET_ERROR_CONTROL_AUTOREGISTER		_EC(325)	// Failed to control actively registration
#define	NET_ERROR_DISCONNECT_AUTOREGISTER	_EC(326)	// Failed to disconnect actively registration
#define NET_ERROR_GETCFG_MMS				_EC(327)	// Failed to get mms configuration
#define NET_ERROR_SETCFG_MMS				_EC(328)	// Failed to set mms configuration
#define NET_ERROR_GETCFG_SMSACTIVATION      _EC(329)	// Failed to get SMS configuration
#define NET_ERROR_SETCFG_SMSACTIVATION      _EC(330)	// Failed to set SMS configuration
#define NET_ERROR_GETCFG_DIALINACTIVATION	_EC(331)	// Failed to get activation of a wireless connection
#define NET_ERROR_SETCFG_DIALINACTIVATION	_EC(332)	// Failed to set activation of a wireless connection
#define NET_ERROR_GETCFG_VIDEOOUT           _EC(333)    // Failed to get the parameter of video output
#define NET_ERROR_SETCFG_VIDEOOUT			_EC(334)	// Failed to set the configuration of video output
#define NET_ERROR_GETCFG_OSDENABLE			_EC(335)	// Failed to get osd overlay enabling
#define NET_ERROR_SETCFG_OSDENABLE			_EC(336)	// Failed to set OSD overlay enabling
#define NET_ERROR_SETCFG_ENCODERINFO        _EC(337)    // Failed to set digital input configuration of front encoders
#define NET_ERROR_GETCFG_TVADJUST		    _EC(338)	// Failed to get TV adjust configuration
#define NET_ERROR_SETCFG_TVADJUST			_EC(339)	// Failed to set TV adjust configuration

#define NET_ERROR_CONNECT_FAILED			_EC(340)	// Failed to request to establish a connection
#define NET_ERROR_SETCFG_BURNFILE			_EC(341)	// Failed to request to upload burn files
#define NET_ERROR_SNIFFER_GETCFG			_EC(342)	// // Failed to get capture configuration information
#define NET_ERROR_SNIFFER_SETCFG			_EC(343)	// Failed to set capture configuration information
#define NET_ERROR_DOWNLOADRATE_GETCFG		_EC(344)	// Failed to get download restrictions information
#define NET_ERROR_DOWNLOADRATE_SETCFG		_EC(345)	// Failed to set download restrictions information
#define NET_ERROR_SEARCH_TRANSCOM			_EC(346)	// Failed to query serial port parameters
#define NET_ERROR_GETCFG_POINT				_EC(347)	// Failed to get the preset info
#define NET_ERROR_SETCFG_POINT				_EC(348)	// Failed to set the preset info
#define NET_SDK_LOGOUT_ERROR				_EC(349)    // SDK log out the device abnormally
#define NET_ERROR_GET_VEHICLE_CFG			_EC(350)	// Failed to get vehicle configuration
#define NET_ERROR_SET_VEHICLE_CFG			_EC(351)	// Failed to set vehicle configuration
#define NET_ERROR_GET_ATM_OVERLAY_CFG		_EC(352)	// Failed to get ATM overlay configuration
#define NET_ERROR_SET_ATM_OVERLAY_CFG		_EC(353)	// Failed to set ATM overlay configuration
#define NET_ERROR_GET_ATM_OVERLAY_ABILITY	_EC(354)	// Failed to get ATM overlay ability
#define NET_ERROR_GET_DECODER_TOUR_CFG		_EC(355)	// Failed to get decoder tour configuration
#define NET_ERROR_SET_DECODER_TOUR_CFG		_EC(356)	// Failed to set decoder tour configuration
#define NET_ERROR_CTRL_DECODER_TOUR			_EC(357)	// Failed to control decoder tour
#define NET_GROUP_OVERSUPPORTNUM			_EC(358)	// Beyond the device supports for the largest number of user groups
#define NET_USER_OVERSUPPORTNUM				_EC(359)	// Beyond the device supports for the largest number of users 
#define NET_ERROR_GET_SIP_CFG				_EC(368)	// Failed to get SIP configuration
#define NET_ERROR_SET_SIP_CFG				_EC(369)	// Failed to set SIP configuration
#define NET_ERROR_GET_SIP_ABILITY			_EC(370)	// Failed to get SIP capability
#define NET_ERROR_GET_WIFI_AP_CFG			_EC(371)	// Failed to get "WIFI ap' configuration 
#define NET_ERROR_SET_WIFI_AP_CFG			_EC(372)	// Failed to set "WIFI ap" configuration  
#define NET_ERROR_GET_DECODE_POLICY		    _EC(373)	// Failed to get decode policy 
#define NET_ERROR_SET_DECODE_POLICY			_EC(374)	// Failed to set decode policy 
#define NET_ERROR_TALK_REJECT				_EC(375)	// refuse talk
#define NET_ERROR_TALK_OPENED				_EC(376)	// talk has opened by other client
#define NET_ERROR_TALK_RESOURCE_CONFLICIT   _EC(377)	// resource conflict
#define NET_ERROR_TALK_UNSUPPORTED_ENCODE   _EC(378)	// unsupported encode type
#define NET_ERROR_TALK_RIGHTLESS			_EC(379)	// no right
#define NET_ERROR_TALK_FAILED				_EC(380)	// request failed
#define NET_ERROR_GET_MACHINE_CFG			_EC(381)	// Failed to get device relative config
#define NET_ERROR_SET_MACHINE_CFG			_EC(382)	// Failed to set device relative config
#define NET_ERROR_GET_DATA_FAILED			_EC(383)	// get data failed
#define NET_ERROR_MAC_VALIDATE_FAILED       _EC(384)    // MAC validate failed
#define NET_ERROR_GET_INSTANCE              _EC(385)    // Failed to get server instance 
#define NET_ERROR_JSON_REQUEST              _EC(386)    // Generated json string is error
#define NET_ERROR_JSON_RESPONSE             _EC(387)    // The responding json string is error
#define NET_ERROR_VERSION_HIGHER            _EC(388)    // The protocol version is lower than current version
#define NET_SPARE_NO_CAPACITY				_EC(389)	// Hotspare disk operation failed. The capacity is low
#define NET_ERROR_SOURCE_IN_USE				_EC(390)	// Display source is used by other output
#define NET_ERROR_REAVE                     _EC(391)    // advanced users grab low-level user resource
#define NET_ERROR_NETFORBID                 _EC(392)    // net forbid
#define NET_ERROR_GETCFG_MACFILTER			_EC(393)    // get MAC filter configuration error
#define NET_ERROR_SETCFG_MACFILTER			_EC(394)    // set MAC filter configuration error
#define NET_ERROR_GETCFG_IPMACFILTER		_EC(395)    // get IP/MAC filter configuration error
#define NET_ERROR_SETCFG_IPMACFILTER		_EC(396)    // set IP/MAC filter configuration error
#define NET_ERROR_OPERATION_OVERTIME        _EC(397)    // operation over time 
#define NET_ERROR_SENIOR_VALIDATE_FAILED    _EC(398)    // senior validation failure
 
#define NET_ERROR_DEVICE_ID_NOT_EXIST		_EC(399)	// device ID is not exist
#define NET_ERROR_UNSUPPORTED               _EC(400)    // unsupport operation

#define NET_ERROR_PROXY_DLLLOAD				_EC(401)	// proxy dll load error
#define NET_ERROR_PROXY_ILLEGAL_PARAM		_EC(402)	// proxy user parameter is not legal
#define NET_ERROR_PROXY_INVALID_HANDLE		_EC(403)	// handle invalid
#define NET_ERROR_PROXY_LOGIN_DEVICE_ERROR	_EC(404)	// login device error
#define NET_ERROR_PROXY_START_SERVER_ERROR	_EC(405)	// start proxy server error
#define NET_ERROR_SPEAK_FAILED				_EC(406)	// request speak failed
#define NET_ERROR_NOT_SUPPORT_F6            _EC(407)    // unsupport F6
#define NET_ERROR_CD_UNREADY				_EC(408)	// CD is not ready
#define NET_ERROR_DIR_NOT_EXIST				_EC(409)	// Directory does not exist
#define NET_ERROR_UNSUPPORTED_SPLIT_MODE	_EC(410)	// The device does not support the segmentation model
#define NET_ERROR_OPEN_WND_PARAM			_EC(411)	// Open the window parameter is illegal
#define NET_ERROR_LIMITED_WND_COUNT			_EC(412)	// Open the window more than limit
#define NET_ERROR_UNMATCHED_REQUEST			_EC(413)	// Request command with the current pattern don't match
#define NET_RENDER_ENABLELARGEPICADJUSTMENT_ERROR	_EC(414)	// Render Library to enable high-definition image internal adjustment strategy error
#define NET_ERROR_UPGRADE_FAILED            _EC(415)    // Upgrade equipment failure
#define	NET_ERROR_NO_TARGET_DEVICE			_EC(416)	// Can't find the target device
#define	NET_ERROR_NO_VERIFY_DEVICE			_EC(417)	// Can't find the target 
#define	NET_ERROR_CASCADE_RIGHTLESS			_EC(418)	// No cascade permissions
#define NET_ERROR_LOW_PRIORITY				_EC(419)	// low priority
#define NET_ERROR_REMOTE_REQUEST_TIMEOUT	_EC(420)	// The remote device request timeout
#define NET_ERROR_LIMITED_INPUT_SOURCE		_EC(421)	//Input source beyond maximum route restrictions
#define NET_ERROR_VISITE_FILE				_EC(510)	// Access to the file failed
#define NET_ERROR_DEVICE_STATUS_BUSY		_EC(511)	// Device busy

/************************************************************************
 ** Enumeration Definition
 ***********************************************************************/
//Card information types CLIENT_QueryNetStat  interface using
enum EM_NET_QUERY_TYPE {
	NET_APP_DATA_STAT , // Statistics for protocol stack,(input struct NET_IN_NETAPP_NET_DATA_STAT,output struct NET_OUT_NETAPP_NET_DATA_STAT)
	NET_APP_LINK_STAT , // Access to physical link state,(input structNET_IN_NETAPP_LINK_STATUS,output struct NET_OUT_NETAPP_LINK_STATUS)
};

// Catch a figure type CLIENT_CapturePictureEx interface using
enum NET_CAPTURE_FORMATS
{
	NET_CAPTURE_BMP,
	NET_CAPTURE_JPEG, // 100% quality JPEG
	NET_CAPTURE_JPEG_70, // 70% quality JPEG
	NET_CAPTURE_JPEG_50,
	NET_CAPTURE_JPEG_30,
};

// Device type
enum NET_DEVICE_TYPE 
{
	NET_PRODUCT_NONE = 0,
	NET_DVR_NONREALTIME_MACE,					// Non real-time MACE
	NET_DVR_NONREALTIME,						// Non real-time
	NET_NVS_MPEG1,								// Network video server
	NET_DVR_MPEG1_2,							// MPEG1 2-ch DVR
	NET_DVR_MPEG1_8,							// MPEG1 8-ch DVR
	NET_DVR_MPEG4_8,							// MPEG4 8-ch DVR
	NET_DVR_MPEG4_16,							// MPEG4 16-ch DVR
	NET_DVR_MPEG4_SX2,							// LB series DVR
	NET_DVR_MEPG4_ST2,							// GB  series DVR
	NET_DVR_MEPG4_SH2,							// HB  series DVR
	NET_DVR_MPEG4_GBE,							// GBE  series DVR
	NET_DVR_MPEG4_NVSII,						// II network video server
	NET_DVR_STD_NEW,							// New standard configuration protocol
	NET_DVR_DDNS,								// DDNS server
	NET_DVR_ATM,								// ATM series 
	NET_NB_SERIAL,								// 2nd non real-time NB series DVR
	NET_LN_SERIAL,								// LN  series 
	NET_BAV_SERIAL,								// BAV series
	NET_SDIP_SERIAL,							// SDIP series
	NET_IPC_SERIAL,								// IPC series
	NET_NVS_B,									// NVS B series
	NET_NVS_C,									// NVS H series 
	NET_NVS_S,									// NVS S series
	NET_NVS_E,									// NVS E series
	NET_DVR_NEW_PROTOCOL,						// Search device type from QueryDevState. it is in string format
	NET_NVD_SERIAL,								// NVD
	NET_DVR_N5,									// N5
	NET_DVR_MIX_DVR,							// HDVR
	NET_SVR_SERIAL,								// SVR series
	NET_SVR_BS,									// SVR-BS
	NET_NVR_SERIAL,								// NVR series
	NET_DVR_N51,                                // N51
	NET_ITSE_SERIAL,							// ITSE Intelligent Analysis Box
	NET_ITC_SERIAL,                             // Intelligent traffic camera equipment
	NET_HWS_SERIAL,                             // radar speedometer HWS
	NET_PVR_SERIAL,                             // portable video record
	NET_IVS_SERIAL,                             // IVS(intelligent video server series)
	NET_IVS_B,                                  // universal intelligent detect video server series 
	NET_IVS_F,                                  // face recognisation server
	NET_IVS_V,                                  // video quality diagnosis server
	NET_MATRIX_SERIAL,							// matrix
	NET_DVR_N52,								// N52
	NET_DVR_N56,								// N56
	NET_ESS_SERIAL,                             // ESS
	NET_IVS_PC,                                 // number statistic server
	NET_PC_NVR,                                 // pc-nvr
	NET_DSCON,									// screen controller
	NET_EVS,									// network video storage server
	NET_EIVS,									// an embedded intelligent video analysis system
	NET_DVR_N6,                                 // DVR-N6
	NET_UDS,                                    // K-Lite Codec Pack
	NET_AF6016,									// Bank alarm host
	NET_AS5008,									//Video network alarm host
	NET_AH2008,									//Network alarm host
	NET_A_SERIAL,								// Alarm host series
	NET_BSC_SERIAL,								// Access control series of products
    NET_NVS_SERIAL,               // NVSϵ�в�Ʒ
};

// Language type
typedef enum __LANGUAGE_TYPE
{
	DH_LANGUAGE_ENGLISH,						// English 
	DH_LANGUAGE_CHINESE_SIMPLIFIED,				// Simplified Chinese
	DH_LANGUAGE_CHINESE_TRADITIONAL,			// Traditional Chinese
	DH_LANGUAGE_ITALIAN,						// Italian 
	DH_LANGUAGE_SPANISH,						// Spanish
	DH_LANGUAGE_JAPANESE,						// Japanese
	DH_LANGUAGE_RUSSIAN,						// Russian 
	DH_LANGUAGE_FRENCH,							// French
	DH_LANGUAGE_GERMAN, 						// German
	DH_LANGUAGE_PORTUGUESE,						// Portuguese
	DH_LANGUAGE_TURKEY,							// Turkey	
	DH_LANGUAGE_POLISH,							// Polish
	DH_LANGUAGE_ROMANIAN,						// Romanian 
	DH_LANGUAGE_HUNGARIAN,						// Hungarian
	DH_LANGUAGE_FINNISH,						// Finnish
	DH_LANGUAGE_ESTONIAN,						// Estonian	
	DH_LANGUAGE_KOREAN,							// Korean
	DH_LANGUAGE_FARSI,							// Farsi	
	DH_LANGUAGE_DANSK,							// Denmark
	DH_LANGUAGE_CZECHISH,						// Czechish
	DH_LANGUAGE_BULGARIA,						// Bulgaria
	DH_LANGUAGE_SLOVAKIAN,						// Slovakian
	DH_LANGUAGE_SLOVENIA,						// Slovenia
	DH_LANGUAGE_CROATIAN,						// Croatian
	DH_LANGUAGE_DUTCH,							// Dutch
	DH_LANGUAGE_GREEK,							// Greek
	DH_LANGUAGE_UKRAINIAN,						// Ukrainian
	DH_LANGUAGE_SWEDISH,						// Swedish
	DH_LANGUAGE_SERBIAN,						// Serbian
	DH_LANGUAGE_VIETNAMESE,						// Vietnamese
	DH_LANGUAGE_LITHUANIAN,						// Lithuanian
	DH_LANGUAGE_FILIPINO,						// Filipino
	DH_LANGUAGE_ARABIC,							// Arabic
	DH_LANGUAGE_CATALAN,						// Catalan
	DH_LANGUAGE_LATVIAN,						// Latvian
	DH_LANGUAGE_THAI,							// Thai
	DH_LANGUAGE_HEBREW,							// Hebrew
	DH_LANGUAGE_Bosnian,						// Bosnian
} DH_LANGUAGE_TYPE;

// Upgrade type
typedef enum __EM_UPGRADE_TYPE
{
	DH_UPGRADE_BIOS_TYPE = 1,					// BIOS  upgrade
	DH_UPGRADE_WEB_TYPE,						// WEB upgrade
	DH_UPGRADE_BOOT_YPE,						// BOOT upgrade
	DH_UPGRADE_CHARACTER_TYPE,					// Chinese character library
	DH_UPGRADE_LOGO_TYPE,						// LOGO
	DH_UPGRADE_EXE_TYPE,						// EXE such as player
    DH_UPGRADE_DEVCONSTINFO_TYPE,               // upgrade device information
    DH_UPGRADE_PERIPHERAL_TYPE,                 // Peripheral access from (such as car 287 chip)
	DH_UPGRADE_GEOINFO_TYPE,                    // Geographic information positioning chip
	DH_UPGRADE_MENU,                            // Menu (equipment operation interface of the picture)
	DH_UPGRADE_ROUTE,                           // Line files (such as bus lines)
	DH_UPGRADE_ROUTE_STATE_AUTO,                // Stops the audio (with line stops audio)
	DH_UPGRADE_SCREEN,                          // Scheduling screen (e.g., bus operation panel)
} EM_UPGRADE_TYPE;

// Record related (schedule,motion detection,alarm)
typedef enum __REC_TYPE
{
	DH_REC_TYPE_TIM = 0,
	DH_REC_TYPE_MTD,
	DH_REC_TYPE_ALM,
	DH_REC_TYPE_NUM,
} REC_TYPE;

// network type  
typedef enum __GPRSCDMA_NETWORK_TYPE
{
	DH_TYPE_AUTOSEL = 0,						// Automatic selection
	DH_TYPE_TD_SCDMA,							// TD-SCDMA network 
	DH_TYPE_WCDMA,								// WCDMA network
	DH_TYPE_CDMA_1x,							// CDMA 1.x network
	DH_TYPE_EDGE,								// GPRS network
	DH_TYPE_EVDO,								// EVDO network
	DH_TYPE_WIFI,
} EM_GPRSCDMA_NETWORK_TYPE;

// Interface type,responding to the interface CLIENT_SetSubconnCallBack
typedef enum __EM_INTERFACE_TYPE
{
	DH_INTERFACE_OTHER = 0x00000000,			// Unknown interface
	DH_INTERFACE_REALPLAY,						// Realtime monitoring interface
	DH_INTERFACE_PREVIEW,						// Realtime multiple-window preview
	DH_INTERFACE_PLAYBACK,						// Playback interface
	DH_INTERFACE_DOWNLOAD,						// Download interface
	DH_INTERFACE_REALLOADPIC,                   // Download picture interface
} EM_INTERFACE_TYPE;

// realplay disconnect event
typedef enum _EM_REALPLAY_DISCONNECT_EVENT_TYPE
{
	DISCONNECT_EVENT_REAVE,                     // resources is taked by advanced user
	DISCONNECT_EVENT_NETFORBID,                 // forbidden
	DISCONNECT_EVENT_SUBCONNECT,                // sublink disconnect
}EM_REALPLAY_DISCONNECT_EVENT_TYPE;

// event file's tag type
typedef enum __EM_EVENT_FILETAG
{
	DH_ATMBEFOREPASTE = 1,                      // Before ATM Paste
	DH_ATMAFTERPASTE,                           // After ATM Paste
}EM_EVENT_FILETAG;

// IPC type
typedef enum __EM_IPC_TYPE
{
	DH_IPC_PRIVATE,                             // private
	DH_IPC_AEBELL,                              // AEBell
	DH_IPC_PANASONIC,                           // panasonic
	DH_IPC_SONY,                                // sony
	DH_IPC_DYNACOLOR,                           // Dynacolor
	DH_IPC_TCWS = 5 ,                           // TCWS	
	DH_IPC_SAMSUNG,                             // Samsung
	DH_IPC_YOKO,                                // YOKO
	DH_IPC_AXIS,                                // AXIS
	DH_IPC_SANYO,							    // sanyo       
	DH_IPC_BOSH = 10,							// Bosch
	DH_IPC_PECLO,								// PECLO
	DH_IPC_PROVIDEO,							// Provideo
	DH_IPC_ACTI,								// ACTi
	DH_IPC_VIVOTEK,								// Vivotek
	DH_IPC_ARECONT = 15,                        // Arecont
	DH_IPC_PRIVATEEH,			                // PrivateEH	
	DH_IPC_IMATEK,					            // IMatek
	DH_IPC_SHANY,                               // Shany
	DH_IPC_VIDEOTREC,                           // videorec
	DH_IPC_URA = 20,                            // Ura
	DH_IPC_BITICINO,                            // Bticino 
	DH_IPC_ONVIF,                               // Onvif protocol type
	DH_IPC_SHEPHERD,                            // Shepherd
	DH_IPC_YAAN,                                // Yaan
	DH_IPC_AIRPOINT = 25,                       // Airpoint
	DH_IPC_TYCO,                                // TYCO
	DH_IPC_XUNMEI,								// Xunmei
	DH_IPC_HIKVISION,							// HIKVISION
	DH_IPC_LG,                                  // LG
	DH_IPC_AOQIMAN = 30,                        // Aoqiman
	DH_IPC_BAOKANG,                             // baokang  
	DH_IPC_WATCHNET,                            // Watchnet
	DH_IPC_XVISION,                             // Xvision
	DH_IPC_FUSITSU,                             // Fisitu
	DH_IPC_CANON = 35,							// Canon
	DH_IPC_GE,								    // GE
	DH_IPC_Basler,								// Basler
	DH_IPC_Patro,								// Patro
	DH_IPC_CPKNC,								// CPPLUS K series
	DH_IPC_CPRNC = 40,							// CPPLUS R series
	DH_IPC_CPUNC,								// CPPLUS U series
	DH_IPC_CPPLUS,								// cpplus oem 
	DH_IPC_XunmeiS,								// XunmeiS
	DH_IPC_GDDW,									// guangdong power grid
	DH_IPC_PSIA = 45,                               // PSIA
	DH_IPC_GB2818,                                  // GB2818	
	DH_IPC_GDYX,                                    // GDYX
	DH_IPC_OTHER,                                   // custom
	DH_IPC_CPUNR,									// CPPLUS NVR
	DH_IPC_CPUAR = 50,								// CPPLUS DVR
	DH_IPC_AIRLIVE,                                 // Airlive	
	DH_IPC_NPE,										// NPE	
	DH_IPC_AXVIEW,									// AXVIEW
	DH_IPC_HYUNDAI = 56,							// HYUNDAI DVR
	DH_IPC_APHD,									// APHD
	DH_IPC_WELLTRANS ,								// WELLTRANS
	DH_IPC_CDJF,									// CDJF
	DH_IPC_JVC = 60,						    	// JVC
	DH_IPC_INFINOVA,								// INFINOVA
	DH_IPC_ADT,										// ADT
	DH_IPC_SIVIDI,									// SIVIDI
	DH_IPC_CPUNP,									// CPPLUS PTZ
	DH_IPC_HX = 65,									// HX
	DH_IPC_TJGS,                                    // TJGS
}EM_IPC_TYPE;

// H264 profile rank
typedef enum __EM_H264_PROFILE_RANK
{
    	DH_PROFILE_BASELINE = 1,                 // offer I/P frame, only support progressive and CAVLC
		DH_PROFILE_MAIN,                         // offer I/P/B frame, support progressiv and interlaced,offer CAVLC and CABAC
		DH_PROFILE_EXTENDED,                     // offer I/P/B/SP/SI frame,only support progressive and CAVLC
		DH_PROFILE_HIGH,                         // FRExt,base on Main_Profile:8x8 intra prediction, custom quant, lossless video coding, more yuv
}EM_H264_PROFILE_RANK;

typedef enum __EM_DISK_TYPE
{
    DH_DISK_READ_WRITE,                          // read write disk
		DH_DISK_READ_ONLY,                       // read only disk
		DH_DISK_BACKUP,	                         // back up disk or media disk
		DH_DISK_REDUNDANT,                       // redundancy disk
		DH_DISK_SNAPSHOT,	                     // snapshot disk
}EM_DISK_TYPE;

// stream encrypt algorithm work mode
typedef enum  __EM_ENCRYPT_ALOG_WORKMODE
{
		ENCRYPT_ALOG_WORKMODE_ECB,				// ECB mode
		ENCRYPT_ALOG_WORKMODE_CBC,				// CBC mode
		ENCRYPT_ALOG_WORKMODE_CFB,				// CFB mode
		ENCRYPT_ALOG_WORKMODE_OFB,				// OFB mode
}EM_ENCRYPT_ALOG_WORKMODE;

typedef enum __EM_MOBILE_PPP_STATE
{
	MOBILE_PPP_UP = 0,							// connect has being build
	MOBILE_PPP_DOWN,							// ppp connect has being cut		
	MOBILE_PPP_CONNECTING,						// be connecting		
	MOBILE_PPP_CLOSEING,						// be closing PPP connect
} EM_MOBILE_PPP_STATE;

typedef enum __EM_3GMOBILE_STATE
{
    MOBILE_MODULE_OFF,							// means 3g net card mod is closed   		
	MOBILE_MODULE_STARTING,						// means 3g net card mod is starting
	MOBILE_MODULE_WORKING,						// means 3g net card mod is working
}EM_3GMOBILE_STATE;
/////////////////////////////////Monitor related/////////////////////////////////

// Preview type.Corresponding to CLIENT_RealPlayEx
typedef enum _RealPlayType
{
	DH_RType_Realplay = 0,						// Real-time preview
	DH_RType_Multiplay,							// Multiple-channel preview 
		
	DH_RType_Realplay_0,						// Real-time monitor-main stream. It is the same as DH_RType_Realplay
	DH_RType_Realplay_1,						// 1 Real-time monitor---extra stream 1
	DH_RType_Realplay_2,						// 2Real-time monitor-- extra stream 2
	DH_RType_Realplay_3,						// 3 Real-time monitor -- extra stream 3
		
	DH_RType_Multiplay_1,						// Multiple-channel preview-- 1-window 
	DH_RType_Multiplay_4,						// Multiple-channel preview--4-window
	DH_RType_Multiplay_8,						// Multiple-channel preview--8-window
	DH_RType_Multiplay_9,						// Multiple-channel preview--9-window
	DH_RType_Multiplay_16,						// Multiple-channel preview--16-window
	DH_RType_Multiplay_6,						// Multiple-channel preview--6-window
	DH_RType_Multiplay_12,						// Multiple-channel preview--12-window
} DH_RealPlayType;

/////////////////////////////////About PTZ/////////////////////////////////

// General PTZ control command
typedef enum _PTZ_ControlType
{
	DH_PTZ_UP_CONTROL = 0,						// Up
	DH_PTZ_DOWN_CONTROL,						// Down
	DH_PTZ_LEFT_CONTROL,						// Left
	DH_PTZ_RIGHT_CONTROL,						// Right
	DH_PTZ_ZOOM_ADD_CONTROL,					// +Zoom in 
	DH_PTZ_ZOOM_DEC_CONTROL,					// -Zoom out 
	DH_PTZ_FOCUS_ADD_CONTROL,					// +Zoom in 
	DH_PTZ_FOCUS_DEC_CONTROL,					// -Zoom out 
	DH_PTZ_APERTURE_ADD_CONTROL,				// + Aperture 
	DH_PTZ_APERTURE_DEC_CONTROL,				// -Aperture
    DH_PTZ_POINT_MOVE_CONTROL,					// Go to preset 
    DH_PTZ_POINT_SET_CONTROL,					// Set 
    DH_PTZ_POINT_DEL_CONTROL,					// Delete
    DH_PTZ_POINT_LOOP_CONTROL,					// Tour 
    DH_PTZ_LAMP_CONTROL							// Light and wiper 
} DH_PTZ_ControlType;

// PTZ control extensive command 
typedef enum _EXTPTZ_ControlType
{
	DH_EXTPTZ_LEFTTOP = 0x20,					// Upper left
	DH_EXTPTZ_RIGHTTOP,							// Upper right 
	DH_EXTPTZ_LEFTDOWN,							// Down left
	DH_EXTPTZ_RIGHTDOWN,						// Down right 
	DH_EXTPTZ_ADDTOLOOP,						// Add preset to tour		tour	 preset value
	DH_EXTPTZ_DELFROMLOOP,						// Delete preset in tour	tour	 preset value
    DH_EXTPTZ_CLOSELOOP,						// Delete tour				tour		
	DH_EXTPTZ_STARTPANCRUISE,					// Begin pan rotation		
	DH_EXTPTZ_STOPPANCRUISE,					// Stop pan rotation		
	DH_EXTPTZ_SETLEFTBORDER,					// Set left limit		
	DH_EXTPTZ_SETRIGHTBORDER,					// Set right limit	
	DH_EXTPTZ_STARTLINESCAN,					// Begin scanning			
    DH_EXTPTZ_CLOSELINESCAN,					// Stop scanning		
    DH_EXTPTZ_SETMODESTART,						// Start mode	mode line		
    DH_EXTPTZ_SETMODESTOP,						// Stop mode	mode line		
	DH_EXTPTZ_RUNMODE,							// Enable mode	Mode line		
	DH_EXTPTZ_STOPMODE,							// Disable mode	Mode line	
	DH_EXTPTZ_DELETEMODE,						// Delete mode	Mode line
	DH_EXTPTZ_REVERSECOMM,						// Flip
	DH_EXTPTZ_FASTGOTO,							// 3D position	X address(8192)	Y address(8192)	zoom(4)
	DH_EXTPTZ_AUXIOPEN,							// auxiliary open	Auxiliary point	
	DH_EXTPTZ_AUXICLOSE,						// Auxiliary close	Auxiliary point
	DH_EXTPTZ_OPENMENU = 0x36,					// Open dome menu 
	DH_EXTPTZ_CLOSEMENU,						// Close menu 
	DH_EXTPTZ_MENUOK,							// Confirm menu 
	DH_EXTPTZ_MENUCANCEL,						// Cancel menu 
	DH_EXTPTZ_MENUUP,							// menu up 
	DH_EXTPTZ_MENUDOWN,							// menu down
	DH_EXTPTZ_MENULEFT,							// menu left
	DH_EXTPTZ_MENURIGHT,						// Menu right 
	DH_EXTPTZ_ALARMHANDLE = 0x40,				// Alarm activate PTZ parm1:Alarm input channel;parm2:Alarm activation type  1-preset 2-scan 3-tour;parm 3:activation value,such as preset value.
	DH_EXTPTZ_MATRIXSWITCH = 0x41,				// Matrix switch parm1:monitor number(video output number);parm2:video input number;parm3:matrix number 
	DH_EXTPTZ_LIGHTCONTROL,						// Light controller
	DH_EXTPTZ_EXACTGOTO,						// 3D accurately positioning parm1:Pan degree(0~3600); parm2: tilt coordinates(0~900); parm3:zoom(1~128)
	DH_EXTPTZ_RESETZERO,                        // Reset  3D positioning as zero
	DH_EXTPTZ_MOVE_ABSOLUTELY,                  // Absolute motion control commands��param4 corresponding struct PTZ_CONTROL_ABSOLUTELY
	DH_EXTPTZ_MOVE_CONTINUOUSLY,                // Continuous motion control commands��param4 corresponding struct PTZ_CONTROL_CONTINUOUSLY
	DH_EXTPTZ_GOTOPRESET,                       // PTZ control command, at a certain speed to preset locus��parm4 corresponding struct PTZ_CONTROL_GOTOPRESET
	DH_EXTPTZ_SET_VIEW_RANGE = 0x49,           	// Set to horizon(param4 corresponding struct PTZ_VIEW_RANGE_INFO)
	DH_EXTPTZ_FOCUS_ABSOLUTELY = 0x4A,         	// Absolute focus(param4 corresponding struct PTZ_FOCUS_ABSOLUTELY)
	DH_EXTPTZ_HORSECTORSCAN = 0x4B,             // Level fan sweep(param4 corresponding PTZ_CONTROL_SECTORSCAN,param1��param2��param3 is invalid)
	DH_EXTPTZ_VERSECTORSCAN = 0x4C,             // Vertical sweep fan(param4��ӦPTZ_CONTROL_SECTORSCAN,param1��param2��param3 is invalid)
	DH_EXTPTZ_SET_ABS_ZOOMFOCUS = 0x4D,         // Set absolute focus, focus on value, param1 for focal length, range: [0255], param2 as the focus, scope: [0255], param3, param4 is invalid

	DH_EXTPTZ_UP_TELE = 0x70,					// Up + TELE param1=speed (1-8) 
	DH_EXTPTZ_DOWN_TELE,						// Down + TELE
	DH_EXTPTZ_LEFT_TELE,						// Left + TELE
	DH_EXTPTZ_RIGHT_TELE,						// Right + TELE
	DH_EXTPTZ_LEFTUP_TELE,						// Upper left + TELE
	DH_EXTPTZ_LEFTDOWN_TELE,					// Down left + TELE
	DH_EXTPTZ_TIGHTUP_TELE,						// Upper right + TELE
	DH_EXTPTZ_RIGHTDOWN_TELE,					// Down right + TELE
	DH_EXTPTZ_UP_WIDE,							// Up + WIDE param1=speed (1-8) 
	DH_EXTPTZ_DOWN_WIDE,						// Down + WIDE
	DH_EXTPTZ_LEFT_WIDE,						// Left + WIDE
	DH_EXTPTZ_RIGHT_WIDE,						// Right + WIDE
	DH_EXTPTZ_LEFTUP_WIDE,						// Upper left + WIDE
	DH_EXTPTZ_LEFTDOWN_WIDE,					// Down left+ WIDE
	DH_EXTPTZ_TIGHTUP_WIDE,						// Upper right + WIDE
	DH_EXTPTZ_RIGHTDOWN_WIDE,					// Down right + WIDE
	DH_EXTPTZ_TOTAL,							// max command value
} DH_EXTPTZ_ControlType;

/////////////////////////////////About Log /////////////////////////////////

// Log search type 
typedef enum _DH_LOG_QUERY_TYPE
{
	DHLOG_ALL = 0,								// All logs
	DHLOG_SYSTEM,								// System logs 
	DHLOG_CONFIG,								// Configuration logs 
	DHLOG_STORAGE,								// Storage logs
	DHLOG_ALARM,								// Alarm logs 
	DHLOG_RECORD,								// Record related
	DHLOG_ACCOUNT,								// Account related
	DHLOG_CLEAR,								// Clear log 
	DHLOG_PLAYBACK,								// Playback related 
	DHLOG_MANAGER                               // Concerning the front-end management and running
} DH_LOG_QUERY_TYPE;

// Log Type
typedef enum _DH_LOG_TYPE
{
	DH_LOG_REBOOT = 0x0000,						// Device reboot 
	DH_LOG_SHUT,								// Shut down device 
    DH_LOG_REPORTSTOP,
    DH_LOG_REPORTSTART,
	DH_LOG_UPGRADE = 0x0004,					// Device Upgrade
	DH_LOG_SYSTIME_UPDATE = 0x0005,             // system time update
	DH_LOG_GPS_TIME_UPDATE = 0x0006,			// GPS time update
	DH_LOG_AUDIO_TALKBACK,	  					// Voice intercom, true representative open, false on behalf of the closed
	DH_LOG_COMM_ADAPTER,						// Transparent transmission, true representative open, false on behalf of the closed
    DH_LOG_NET_TIMING,                          // ����Уʱ
	DH_LOG_CONFSAVE = 0x0100,					// Save configuration 
	DH_LOG_CONFLOAD,							// Read configuration 
	DH_LOG_FSERROR = 0x0200,					// File system error
	DH_LOG_HDD_WERR,							// HDD write error 
	DH_LOG_HDD_RERR,							// HDD read error
	DH_LOG_HDD_TYPE,							// Set HDD type 
	DH_LOG_HDD_FORMAT,							// Format HDD
	DH_LOG_HDD_NOSPACE,							// Current working HDD space is not sufficient
	DH_LOG_HDD_TYPE_RW,							// Set HDD type as read-write 
	DH_LOG_HDD_TYPE_RO,							// Set HDD type as read-only
	DH_LOG_HDD_TYPE_RE,							// Set HDD type as redundant 
	DH_LOG_HDD_TYPE_SS,							// Set HDD type as snapshot
	DH_LOG_HDD_NONE,							// No HDD
	DH_LOG_HDD_NOWORKHDD,						// No work HDD
	DH_LOG_HDD_TYPE_BK,							// Set HDD type to backup HDD
	DH_LOG_HDD_TYPE_REVERSE,					// Set HDD type to reserve subarea
	DH_LOG_HDD_START_INFO = 0x200 +14,          // note the boot-strap's hard disk info
	DH_LOG_HDD_WORKING_DISK,                    // note the disk number after the disk change
	DH_LOG_HDD_OTHER_ERROR,                     // note other errors of disk
	DH_LOG_HDD_SLIGHT_ERR,						// there has some little errors on disk
	DH_LOG_HDD_SERIOUS_ERR,                     // there has some serious errors on disk
	DH_LOG_HDD_NOSPACE_END,                     // the end of the alarm that current disk has no space left 
	DH_LOG_HDD_TYPE_RAID_CONTROL,               // Raid control
	DH_LOG_HDD_TEMPERATURE_HIGH,				// excess temperature
	DH_LOG_HDD_TEMPERATURE_LOW,					//  lower die temperature
	DH_LOG_HDD_ESATA_REMOVE,					// remove eSATA
	DH_LOG_ALM_IN = 0x0300,						// External alarm begin 
	DH_LOG_NETALM_IN,							// Network alarm input 
	DH_LOG_ALM_END = 0x0302,					// External input alarm stop 
	DH_LOG_LOSS_IN,								// Video loss alarm begin 
	DH_LOG_LOSS_END,							// Video loss alarm stop
	DH_LOG_MOTION_IN,							// Motion detection alarm begin 
	DH_LOG_MOTION_END,							// Motion detection alarm stop 
	DH_LOG_ALM_BOSHI,							// Annunciator alarm input 
	DH_LOG_NET_ABORT = 0x0308,					// Network disconnected 
	DH_LOG_NET_ABORT_RESUME,					// Network connection restore 
	DH_LOG_CODER_BREAKDOWN,						// Encoder error
	DH_LOG_CODER_BREAKDOWN_RESUME,				// Encoder error restore 
	DH_LOG_BLIND_IN,							// Camera masking 
	DH_LOG_BLIND_END,							// Restore camera masking 
	DH_LOG_ALM_TEMP_HIGH,						// High temperature 
	DH_LOG_ALM_VOLTAGE_LOW,						// Low voltage
	DH_LOG_ALM_BATTERY_LOW,						// Battery capacity is not sufficient 
	DH_LOG_ALM_ACC_BREAK,						// ACC power off 
    DH_LOG_ALM_ACC_RES,
	DH_LOG_GPS_SIGNAL_LOST,						// GPS signal lost
	DH_LOG_GPS_SIGNAL_RESUME,					// GPS signal resume
	DH_LOG_3G_SIGNAL_LOST,						// 3G signal lost
	DH_LOG_3G_SIGNAL_RESUME,					// 3G signal resume
    DH_LOG_ALM_IPC_IN,							// IPC external alarms
	DH_LOG_ALM_IPC_END,							// IPC external alarms recovery
	DH_LOG_ALM_DIS_IN,							//Broken network alarm
	DH_LOG_ALM_DIS_END,							// Broken network alarm recovery
	DH_LOG_INFRAREDALM_IN = 0x03a0,				// Wireless alarm begin 
	DH_LOG_INFRAREDALM_END,						// Wireless alarm end 
	DH_LOG_IPCONFLICT,							// IP conflict 
	DH_LOG_IPCONFLICT_RESUME,					// IP restore
	DH_LOG_SDPLUG_IN,							// SD Card insert
	DH_LOG_SDPLUG_OUT,							// SD Card Pull-out
	DH_LOG_NET_PORT_BIND_FAILED,				// Failed to bind port
	DH_LOG_HDD_BEEP_RESET,                      // Hard disk error beep reset 
	DH_LOG_MAC_CONFLICT,                        // MAC conflict
	DH_LOG_MAC_CONFLICT_RESUME,                 // MAC conflict resume
	DH_LOG_ALARM_OUT,							// alarm out
	DH_LOG_ALM_RAID_STAT_EVENT,                 // RAID state event    
	DH_LOG_ABLAZE_ON,				            // Fire alarm, smoker or high temperature
	DH_LOG_ABLAZE_OFF,			                // Fire alarm reset 
	DH_LOG_INTELLI_ALARM_PLUSE,					// Intelligence pulse alarm
	DH_LOG_INTELLI_ALARM_IN,					// Intelligence alarm start
	DH_LOG_INTELLI_ALARM_END,					// Intelligence alarm end
	DH_LOG_3G_SIGNAL_SCAN,						// 3G signal scan
	DH_LOG_GPS_SIGNAL_SCAN,						// GPS signal scan
	DH_LOG_AUTOMATIC_RECORD = 0x0400,			// Auto record 
	DH_LOG_MANUAL_RECORD = 0x0401,				// Manual record 
	DH_LOG_CLOSED_RECORD,						// Stop record 
	DH_LOG_LOGIN = 0x0500,						// Log in 
	DH_LOG_LOGOUT,								// Log off 
	DH_LOG_ADD_USER,							// Add user
	DH_LOG_DELETE_USER,							// Delete user
	DH_LOG_MODIFY_USER,							// Modify user 
	DH_LOG_ADD_GROUP,							// Add user group 
	DH_LOG_DELETE_GROUP,						// Delete user group 
	DH_LOG_MODIFY_GROUP,						// Modify user group 
	DH_LOG_NET_LOGIN = 0x0508,					// Network Login
	DH_LOG_MODIFY_PASSWORD,						// Modify password
	DH_LOG_CLEAR = 0x0600,						// Clear log 
	DH_LOG_SEARCHLOG,							// Search log 
	DH_LOG_SEARCH = 0x0700,						// Search record 
	DH_LOG_DOWNLOAD,							// Record download
	DH_LOG_PLAYBACK,							// Record playback
	DH_LOG_BACKUP,								// Backup recorded file 
	DH_LOG_BACKUPERROR,							// Failed to backup recorded file
	DH_LOG_BACK_UPRT,							// Real-time backup, that is, copy CD
	DH_LOG_BACKUPCLONE,							//CD copy
	DH_LOG_DISK_CHANGED,						// Manual changed
	DH_LOG_IMAGEPLAYBACK,						// Image playback
	DH_LOG_LOCKFILE,							// Lock the video
	DH_LOG_UNLOCKFILE,							// Unlock the video
	DH_LOG_ATMPOS,								// Add log superposition of ATM card number

    DH_LOG_TIME_UPDATE  = 0x0800,               // Time update
    DH_LOG_REMOTE_STATE = 0x0850,               // remote diary 
    DH_LOG_USER_DEFINE = 0x0900,
	DH_LOG_TYPE_NR = 10,
} DH_LOG_TYPE;

// Extensive log type. Correponding to CLIENT_QueryLogEx, Condition (int nType = 1; parameter reserved = &nType)
typedef enum _DH_NEWLOG_TYPE
{
	DH_NEWLOG_REBOOT = 0x0000,					// Device reboot 
	DH_NEWLOG_SHUT,								// Shut down device
    DH_NEWLOG_REPORTSTOP,
    DH_NEWLOG_REPORTSTART,
	DH_NEWLOG_UPGRADE = 0x0004,					// Device upgrade
	DH_NEWLOG_SYSTIME_UPDATE = 0x0005,          // system time update
	DH_NEWLOG_GPS_TIME_UPDATE = 0x0006,			// GPS time update

	DH_NEWLOG_AUDIO_TALKBACK,	  				// Voice intercom, true representative open, false on behalf of the closed
	DH_NEWLOG_COMM_ADAPTER,						// Transparent transmission, true representative open, false on behalf of the closed	
	DH_NEWLOG_NET_TIMING,						// NTP

	DH_NEWLOG_CONFSAVE = 0x0100,				// Save configuration 
	DH_NEWLOG_CONFLOAD,							// Read configuration 
	DH_NEWLOG_FSERROR = 0x0200,					// File system error
	DH_NEWLOG_HDD_WERR,							// HDD write error 
	DH_NEWLOG_HDD_RERR,							// HDD read error
	DH_NEWLOG_HDD_TYPE,							// Set HDD type 
	DH_NEWLOG_HDD_FORMAT,						// Format HDD
	DH_NEWLOG_HDD_NOSPACE,						// Current working HDD space is not sufficient
	DH_NEWLOG_HDD_TYPE_RW,						// Set HDD type as read-write 
	DH_NEWLOG_HDD_TYPE_RO,						// Set HDD type as read-only
	DH_NEWLOG_HDD_TYPE_RE,						// Set HDD type as redundant 
	DH_NEWLOG_HDD_TYPE_SS,						// Set HDD type as snapshot
	DH_NEWLOG_HDD_NONE,							// No HDD
	DH_NEWLOG_HDD_NOWORKHDD,					// No work HDD
	DH_NEWLOG_HDD_TYPE_BK,						// Set HDD type to backup HDD
	DH_NEWLOG_HDD_TYPE_REVERSE,					// Set HDD type to reserve subareas
	DH_NEWLOG_HDD_START_INFO = 0x200 +14,       // note the boot-strap's hard disk info
	DH_NEWLOG_HDD_WORKING_DISK,                 // note the disk number after the disk change
	DH_NEWLOG_HDD_OTHER_ERROR,                  // note other errors of disk
	DH_NEWLOG_HDD_SLIGHT_ERR,					// there has some little errors on disk
	DH_NEWLOG_HDD_SERIOUS_ERR,                  // there has some serious errors on disk
	DH_NEWLOG_HDD_NOSPACE_END,                  // the end of the alarm that current disk has no space left 

	DH_NEWLOG_HDD_TYPE_RAID_CONTROL,            // Raid operation
	DH_NEWLOG_HDD_TEMPERATURE_HIGH,				// excess temperature
	DH_NEWLOG_HDD_TEMPERATURE_LOW,				// lower die temperature
	DH_NEWLOG_HDD_ESATA_REMOVE,					// remove eSATA

	DH_NEWLOG_ALM_IN = 0x0300,					// External alarm begin 
	DH_NEWLOG_NETALM_IN,						// Network alarm input 
	DH_NEWLOG_ALM_END = 0x0302,					// External input alarm stop 
	DH_NEWLOG_LOSS_IN,							// Video loss alarm begin 
	DH_NEWLOG_LOSS_END,							// Video loss alarm stop
	DH_NEWLOG_MOTION_IN,						// Motion detection alarm begin 
	DH_NEWLOG_MOTION_END,						// Motion detection alarm stop 
	DH_NEWLOG_ALM_BOSHI,						// Annunciator alarm input 
	DH_NEWLOG_NET_ABORT = 0x0308,				// Network disconnected 
	DH_NEWLOG_NET_ABORT_RESUME,					// Network connection restore 
	DH_NEWLOG_CODER_BREAKDOWN,					// Encoder error
	DH_NEWLOG_CODER_BREAKDOWN_RESUME,			// Encoder error restore 
	DH_NEWLOG_BLIND_IN,							// Camera masking 
	DH_NEWLOG_BLIND_END,						// Restore camera masking 
	DH_NEWLOG_ALM_TEMP_HIGH,					// High temperature 
	DH_NEWLOG_ALM_VOLTAGE_LOW,					// Low voltage
	DH_NEWLOG_ALM_BATTERY_LOW,					// Battery capacity is not sufficient 
	DH_NEWLOG_ALM_ACC_BREAK,					// ACC power off 
    DH_NEWLOG_ALM_ACC_RES,
	DH_NEWLOG_GPS_SIGNAL_LOST,					// GPS signal lost
	DH_NEWLOG_GPS_SIGNAL_RESUME,				// GPS signal resume
	DH_NEWLOG_3G_SIGNAL_LOST,					// 3G signal lost
	DH_NEWLOG_3G_SIGNAL_RESUME,					// 3G signal resume

	DH_NEWLOG_ALM_IPC_IN,						// IPC external alarms
	DH_NEWLOG_ALM_IPC_END,						// IPC external alarms recovery 
	DH_NEWLOG_ALM_DIS_IN,						//Broken network alarm
	DH_NEWLOG_ALM_DIS_END,						// Broken network alarm recovery

	DH_NEWLOG_INFRAREDALM_IN = 0x03a0,			// Wireless alarm begin 
	DH_NEWLOG_INFRAREDALM_END,					// Wireless alarm end 
	DH_NEWLOG_IPCONFLICT,						// IP conflict 
	DH_NEWLOG_IPCONFLICT_RESUME,				// IP restore
	DH_NEWLOG_SDPLUG_IN,						// SD Card insert
	DH_NEWLOG_SDPLUG_OUT,						// SD Card Pull-out
	DH_NEWLOG_NET_PORT_BIND_FAILED,				// Failed to bind port
	DH_NEWLOG_HDD_BEEP_RESET,                   // Hard disk error beep reset
	DH_NEWLOG_MAC_CONFLICT,                     // MAC conflict
	DH_NEWLOG_MAC_CONFLICT_RESUME,              // MAC conflict resume
	DH_NEWLOG_ALARM_OUT,						// alarm of output state
	DH_NEWLOG_ALM_RAID_STAT_EVENT,              // RAID state change
	DH_NEWLOG_ABLAZE_ON,				        // fire alarm, smoke or temperature
	DH_NEWLOG_ABLAZE_OFF,			            // fire alarm resume
	DH_NEWLOG_INTELLI_ALARM_PLUSE,				// alarm of pulse type
	DH_NEWLOG_INTELLI_ALARM_IN,					// begin of alarm
	DH_NEWLOG_INTELLI_ALARM_END,				// end of alarm
	DH_NEWLOG_3G_SIGNAL_SCAN,					// 3G signal scan
	DH_NEWLOG_GPS_SIGNAL_SCAN,					// GPS signal scan
	DH_NEWLOG_AUTOMATIC_RECORD = 0x0400,		// Auto record 
	DH_NEWLOG_MANUAL_RECORD,					// Manual record 
	DH_NEWLOG_CLOSED_RECORD,					// Stop record 
	DH_NEWLOG_LOGIN = 0x0500,					// Log in 
	DH_NEWLOG_LOGOUT,							// Log off 
	DH_NEWLOG_ADD_USER,							// Add user
	DH_NEWLOG_DELETE_USER,						// Delete user
	DH_NEWLOG_MODIFY_USER,						// Modify user 
	DH_NEWLOG_ADD_GROUP,						// Add user group 
	DH_NEWLOG_DELETE_GROUP,						// Delete user group 
	DH_NEWLOG_MODIFY_GROUP,						// Modify user group
	DH_NEWLOG_NET_LOGIN = 0x0508,				// Network user login
	DH_NEWLOG_CLEAR = 0x0600,					// Clear log 
	DH_NEWLOG_SEARCHLOG,						// Search log 
	DH_NEWLOG_SEARCH = 0x0700,					// Search record 
	DH_NEWLOG_DOWNLOAD,							// Record download
	DH_NEWLOG_PLAYBACK,							// Record playback
	DH_NEWLOG_BACKUP,							// Backup recorded file 
	DH_NEWLOG_BACKUPERROR,						// Failed to backup recorded file

	DH_NEWLOG_BACK_UPRT,						// Real-time backup, that is, copy CD
	DH_NEWLOG_BACKUPCLONE,						// CD copy
	DH_NEWLOG_DISK_CHANGED,						// Manual  changed
	DH_NEWLOG_IMAGEPLAYBACK,					// image palyback
	DH_NEWLOG_LOCKFILE,							// Lock the video
	DH_NEWLOG_UNLOCKFILE,						// Unlock the video
	DH_NEWLOG_ATMPOS,							// Add log superposition of ATM card number

	DH_NEWLOG_TIME_UPDATE  = 0x0800,            // Time update
	DH_NEWLOG_REMOTE_STATE = 0x0850,            // remote diary 

    DH_NEWLOG_USER_DEFINE = 0x0900,
    DH_NEWLOG_TYPE_NR = 10,        
} DH_NEWLOG_TYPE;

///////////////////////////////About audio talk ///////////////////////////////

// Audio encode type 
typedef enum __TALK_CODING_TYPE
{
	DH_TALK_DEFAULT = 0,						// No-head PCM
	DH_TALK_PCM = 1,							// With head PCM
	DH_TALK_G711a,								// G711a
	DH_TALK_AMR,								// AMR
	DH_TALK_G711u,								// G711u
	DH_TALK_G726,								// G726
	DH_TALK_G723_53,							// G723_53
	DH_TALK_G723_63,							// G723_63
	DH_TALK_AAC,								// AAC
	DH_TALK_OGG,                                // OGG
	DH_TALK_ADPCM = 21,                         // ADPCM
	DH_TALK_MP3   = 22,							// MP3
} DH_TALK_CODING_TYPE;

// ¼���ļ�����
typedef enum __NET_RECORD_TYPE
{
	NET_RECORD_TYPE_ALL,                        // All the video
	NET_RECORD_TYPE_NORMAL,                     // common  video
	NET_RECORD_TYPE_ALARM,                      // External alarm video
	NET_RECORD_TYPE_MOTION,                     // DM alarm video
}NET_RECORD_TYPE;

// Audio talk way 
typedef enum __EM_USEDEV_MODE
{
	DH_TALK_CLIENT_MODE,						// Set client-end mode to begin audio talk 
	DH_TALK_SERVER_MODE,						// Set server mode to begin audio talk 
	DH_TALK_ENCODE_TYPE,						// Set encode format for audio talk 
	DH_ALARM_LISTEN_MODE,						// Set alarm subscribe way 
	DH_CONFIG_AUTHORITY_MODE,					// Set user right to realize configuration management
	DH_TALK_TALK_CHANNEL,						// set talking channel(0~MaxChannel-1)
	DH_RECORD_STREAM_TYPE,                      // set the stream type of the record for query(0-both main and extra stream,1-only main stream,2-only extra stream)  
	DH_TALK_SPEAK_PARAM,                        // set speaking parameter,corresponding to NET_SPEAK_PARAM
	DH_RECORD_TYPE,                             // Set by time video playback and download the video file TYPE (see.net RECORD TYPE)
	DH_TALK_MODE3,								// Set voice intercom parameters of three generations of equipment and the corresponding structure NET TALK the EX
} EM_USEDEV_MODE;


typedef enum __EM_TALK_DATA_TYPE
{ 
	NET_TALK_DATA_LOCAL_AUDIO = 0,				// Local recording audio data from the library
	NET_TALK_DATA_RECV_AUDIO,					// Receiving device sending audio data
	NET_TALK_DATA_RESPOND,						// Intercom call response data
}EM_TALK_DATA_TYPE;

typedef struct tagNET_TALK_VIDEO_FORMAT
{
	DWORD				dwSize;
	DWORD				dwCompression;			// ��Ƶѹ����ʽ
	int					nFrequency;				// ��Ƶ����Ƶ��
}NET_TALK_VIDEO_FORMAT;
// Three generations of equipment parameters of voice intercom, corresponding to the CLIENT SetDeviceMode DH TALK MODE3 () command
typedef struct tagNET_TALK_EX
{
    DWORD               dwSize;
	int					nChannel;               // channel number 
	int                 nAudioPort;             // Audio transmission listener ports
	int					nWaitTime;              // Ms wait time, unit, use the default value is 0
    HWND				hVideoWnd;				// ���ӶԽ���Ƶ��ʾ����
	NET_TALK_VIDEO_FORMAT stuVideoFmt;			// ��Ƶ�����ʽ
	char				szMulticastAddr[DH_MAX_IPADDR_LEN_EX]; // �鲥��ַ
	WORD				wMulticastLocalPort;	// �鲥���ض˿�
	WORD				wMulticastRemotePort;	// �鲥Զ�̶˿�
}NET_TALK_EX;

// AMR Encode Type
typedef enum __EM_ARM_ENCODE_MODE
{
	DH_TALK_AMR_AMR475 = 1,						// AMR475
	DH_TALK_AMR_AMR515,							// AMR515
	DH_TALK_AMR_AMR59,							// AMR59
	DH_TALK_AMR_AMR67,							// AMR67
	DH_TALK_AMR_AMR74,							// AMR74
	DH_TALK_AMR_AMR795,							// AMR795
	DH_TALK_AMR_AMR102,							// AMR102
	DH_TALK_AMR_AMR122,							// AMR122
} EM_ARM_ENCODE_MODE;

typedef struct __NET_SPEAK_PARAM
{
	DWORD           dwSize;                     // struct size 
	int             nMode;                      // 0:talk back(default), 1: propaganda,from propaganda ro talk back,need afresh to configure
	int             nSpeakerChannel;            // reproducer channel
} NET_SPEAK_PARAM;
/////////////////////////////////Control Related/////////////////////////////////

// Control type    Corresponding to CLIENT_ControlDevice
typedef enum _CtrlType
{
	DH_CTRL_REBOOT = 0,							// Reboot device	
	DH_CTRL_SHUTDOWN,							// Shut down device
	DH_CTRL_DISK,								// HDD management
	DH_KEYBOARD_POWER = 3,						// Network keyboard
	DH_KEYBOARD_ENTER,
	DH_KEYBOARD_ESC,
	DH_KEYBOARD_UP,
	DH_KEYBOARD_DOWN,
	DH_KEYBOARD_LEFT,
	DH_KEYBOARD_RIGHT,
	DH_KEYBOARD_BTN0,
	DH_KEYBOARD_BTN1,
	DH_KEYBOARD_BTN2,
	DH_KEYBOARD_BTN3,
	DH_KEYBOARD_BTN4,
	DH_KEYBOARD_BTN5,
	DH_KEYBOARD_BTN6,
	DH_KEYBOARD_BTN7,
	DH_KEYBOARD_BTN8,
	DH_KEYBOARD_BTN9,
	DH_KEYBOARD_BTN10,
	DH_KEYBOARD_BTN11,
	DH_KEYBOARD_BTN12,
	DH_KEYBOARD_BTN13,
	DH_KEYBOARD_BTN14,
	DH_KEYBOARD_BTN15,
	DH_KEYBOARD_BTN16,
	DH_KEYBOARD_SPLIT,
	DH_KEYBOARD_ONE,
	DH_KEYBOARD_NINE,
	DH_KEYBOARD_ADDR,
	DH_KEYBOARD_INFO,
	DH_KEYBOARD_REC,
	DH_KEYBOARD_FN1,
	DH_KEYBOARD_FN2,
	DH_KEYBOARD_PLAY,
	DH_KEYBOARD_STOP,
	DH_KEYBOARD_SLOW,
	DH_KEYBOARD_FAST,
	DH_KEYBOARD_PREW,
	DH_KEYBOARD_NEXT,
	DH_KEYBOARD_JMPDOWN,
	DH_KEYBOARD_JMPUP,
	DH_TRIGGER_ALARM_IN = 100,					// Activate alarm input
	DH_TRIGGER_ALARM_OUT,						// Activate alarm output 
	DH_CTRL_MATRIX,								// Matrix control 
	DH_CTRL_SDCARD,								// SD card control(for IPC series). Please refer to HDD control
	DH_BURNING_START,							// Burner control:begin burning 
	DH_BURNING_STOP,							// Burner control:stop burning 
	DH_BURNING_ADDPWD,							// Burner control:overlay password(The string ended with '\0'. Max length is 8 bits. )
	DH_BURNING_ADDHEAD,							// Burner control:overlay head title(The string ended with '\0'. Max length is 1024 bytes. Use '\n' to Enter.)
	DH_BURNING_ADDSIGN,							// Burner control:overlay dot to the burned information(No parameter) 
	DH_BURNING_ADDCURSTOMINFO,					// Burner control:self-defined overlay (The string ended with '\0'. Max length is 1024 bytes. Use '\n' to Enter)
	DH_CTRL_RESTOREDEFAULT,						// restore device default setup 
	DH_CTRL_CAPTURE_START,						// Activate device snapshot
	DH_CTRL_CLEARLOG,							// Clear log
	DH_TRIGGER_ALARM_WIRELESS = 200,			// Activate wireless alarm (IPC series)
	DH_MARK_IMPORTANT_RECORD,					// Mark important record
	DH_CTRL_DISK_SUBAREA, 						// Network hard disk partition	
	DH_BURNING_ATTACH,							// Annex burning
	DH_BURNING_PAUSE,							// Burn Pause
	DH_BURNING_CONTINUE,						// Burn Resume
	DH_BURNING_POSTPONE,						// Burn Postponed
	DH_CTRL_OEMCTRL,							// OEM control
	DH_BACKUP_START,							// Start to device backup
	DH_BACKUP_STOP,								// Stop to device backup
	DH_VIHICLE_WIFI_ADD,						// Add WIFI configuration manually for car device
	DH_VIHICLE_WIFI_DEC,						// Delete WIFI configuration manually for car device
	DH_BUZZER_START,                            // Start to buzzer control 
	DH_BUZZER_STOP,                             // Stop to buzzer control
	DH_REJECT_USER,                             // Reject User
	DH_SHIELD_USER,                             // Shield User
	DH_RAINBRUSH,                               // Rain Brush 
	DH_MANUAL_SNAP,                             // manual snap (struct MANUAL_SNAP_PARAMETER)
	DH_MANUAL_NTP_TIMEADJUST,                   // manual ntp time adjust
	DH_NAVIGATION_SMS,                          // navigation info and note
	DH_CTRL_ROUTE_CROSSING,                     // route info
	DH_BACKUP_FORMAT,							// backup device format
	DH_DEVICE_LOCALPREVIEW_SLIPT,               // local preview split(struct DEVICE_LOCALPREVIEW_SLIPT_PARAMETER)    
	DH_CTRL_INIT_RAID,							// RAID init
	DH_CTRL_RAID,								// RAID control
	DH_CTRL_SAPREDISK,							// sapredisk control
	DH_WIFI_CONNECT,							// wifi connect(struct WIFI_CONNECT)
	DH_WIFI_DISCONNECT,							// wifi disconnect(struct WIFI_CONNECT)
	DH_CTRL_ARMED,                              // Arm/disarm operation 
	DH_CTRL_IP_MODIFY,                          // IP modify(struct DHCTRL_IPMODIFY_PARAM)                     
	DH_CTRL_WIFI_BY_WPS,                        // wps connect wifi(struct DHCTRL_CONNECT_WIFI_BYWPS)
	DH_CTRL_FORMAT_PATITION,					// format pattion (struct DH_FORMAT_PATITION)
	DH_CTRL_EJECT_STORAGE,						// eject storage device(struct DH_EJECT_STORAGE_DEVICE)
	DH_CTRL_LOAD_STORAGE,						// load storage device(struct DH_LOAD_STORAGE_DEVICE)
	DH_CTRL_CLOSE_BURNER,                       // close burner(struct NET_CTRL_BURNERDOOR) need wait 6s
	DH_CTRL_EJECT_BURNER,                       // eject burner(struct NET_CTRL_BURNERDOOR) need wait 4s
	DH_CTRL_CLEAR_ALARM,						// alarm elimination corresponding structure NET (CTRL CLEAR ALARM)
	DH_CTRL_MONITORWALL_TVINFO,					// TV wall information display corresponding structure NET (CTRL MONITORWALL TVINFO)
	DH_CTRL_START_VIDEO_ANALYSE,                //  start Intelligent VIDEO analysis (corresponding structure NET CTRL START VIDEO ANALYSE)
	DH_CTRL_STOP_VIDEO_ANALYSE,                 // STOP intelligent VIDEO analysis corresponding structure NET (CTRL STOP VIDEO ANALYSE)
	DH_CTRL_UPGRADE_DEVICE,                     //Controlled start equipment upgrades, independently complete the upgrade process by the equipment do not need to upgrade file
	DH_CTRL_MULTIPLAYBACK_CHANNALES,            // Multi-channel preview playback channel switching corresponding structure NET (CTRL MULTIPLAYBACK CHANNALES)
	DH_CTRL_SEQPOWER_OPEN,						// Turn on the switch power supply timing device output corresponding.net (CTRL SEQPOWER PARAM)
	DH_CTRL_SEQPOWER_CLOSE,						// Close the switch power supply timing device output corresponding.net (CTRL SEQPOWER PARAM)
	DH_CTRL_SEQPOWER_OPEN_ALL,					// Power timing group open the switch quantity output corresponding.net (CTRL SEQPOWER PARAM)
	DH_CTRL_SEQPOWER_CLOSE_ALL,					// Power sequence set close the switch quantity output corresponding.net (CTRL SEQPOWER PARAM)
	DH_CTRL_PROJECTOR_RISE,						// PROJECTOR up corresponding.net (CTRL PROJECTOR PARAM)
	DH_CTRL_PROJECTOR_FALL,						// PROJECTOR drop (corresponding to the.net CTRL PROJECTOR PARAM)
	DH_CTRL_PROJECTOR_STOP,						// PROJECTOR stop (corresponding to the.net CTRL PROJECTOR PARAM)
	DH_CTRL_INFRARED_KEY,						// INFRARED buttons (corresponding to the.net CTRL INFRARED KEY PARAM)
	DH_CTRL_START_PLAYAUDIO,					// Device START playback of audio file corresponding structure NET (CTRL START PLAYAUDIO)
	DH_CTRL_STOP_PLAYAUDIO,						// Equipment stop playback of audio file
	DH_CTRL_START_ALARMBELL,					// Corresponding structure NET open alarm (CTRL ALARMBELL)
	DH_CTRL_STOP_ALARMBELL,						// Close the warning signal corresponding structure NET (CTRL ALARMBELL)
	DH_CTRL_ACCESS_OPEN,						// OPEN ACCESS control - corresponding structure NET (CTRL ACCESS OPEN)
	DH_CTRL_SET_BYPASS,							//Corresponding structure NET BYPASS function (CTRL SET BYPASS)
	DH_CTRL_RECORDSET_INSERT,					// Add records to record set number (corresponding to the.net CTRL you INSERT PARAM)
	DH_CTRL_RECORDSET_UPDATE,					// Update a record of the number (corresponding to the.net CTRL you PARAM)
	DH_CTRL_RECORDSET_REMOVE,					// According to the record set number to delete a record (corresponding to the.net CTRL you PARAM)
	DH_CTRL_RECORDSET_CLEAR,					// Remove all RECORDSET information corresponding.net (CTRL you PARAM)
	DH_CTRL_ACCESS_CLOSE,						// Entrance guard control - CLOSE corresponding structure NET (CTRL ACCESS CLOSE)
	DH_CTRL_ALARM_SUBSYSTEM_ACTIVE_SET,			// ������ϵͳ��������(��Ӧ�ṹ��NET_CTRL_ALARM_SUBSYSTEM_SETACTIVE)
} CtrlType;

// IO control command. Corresponding to CLIENT_QueryIOControlState
typedef enum _IOTYPE
{
	DH_ALARMINPUT = 1,							// Control alarm input 
	DH_ALARMOUTPUT = 2,							// ontrol alarm output 
	DH_DECODER_ALARMOUT = 3,					// Control alarm decoder output 
	DH_WIRELESS_ALARMOUT = 5,					// Control wireless alarm output 
	DH_ALARM_TRIGGER_MODE = 7,					// Alarm activation type(auto/manual/close). Use TRIGGER_MODE_CONTROL structure 
} DH_IOTYPE;

/////////////////////////////////Configuration Related/////////////////////////////////

// Resolution enumeration. For DH_DSP_ENCODECAP to use 
typedef enum _CAPTURE_SIZE
{
	CAPTURE_SIZE_D1,							// 704*576(PAL)  704*480(NTSC)
	CAPTURE_SIZE_HD1,							// 352*576(PAL)  352*480(NTSC)
	CAPTURE_SIZE_BCIF,							// 704*288(PAL)  704*240(NTSC)
	CAPTURE_SIZE_CIF,							// 352*288(PAL)  352*240(NTSC)
	CAPTURE_SIZE_QCIF,							// 176*144(PAL)  176*120(NTSC)
	CAPTURE_SIZE_VGA,							// 640*480
	CAPTURE_SIZE_QVGA,							// 320*240
	CAPTURE_SIZE_SVCD,							// 480*480
	CAPTURE_SIZE_QQVGA,							// 160*128
	CAPTURE_SIZE_SVGA,							// 800*592
	CAPTURE_SIZE_XVGA,							// 1024*768
	CAPTURE_SIZE_WXGA,							// 1280*800
	CAPTURE_SIZE_SXGA,							// 1280*1024  
	CAPTURE_SIZE_WSXGA,							// 1600*1024  
	CAPTURE_SIZE_UXGA,							// 1600*1200
	CAPTURE_SIZE_WUXGA,							// 1920*1200
	CAPTURE_SIZE_LTF,							// 240*192
	CAPTURE_SIZE_720,							// 1280*720
	CAPTURE_SIZE_1080,							// 1920*1080
	CAPTURE_SIZE_1_3M,							// 1280*960
	CAPTURE_SIZE_2M,							// 1872*1408
	CAPTURE_SIZE_5M,							// 3744*1408
	CAPTURE_SIZE_3M,							// 2048*1536
	CAPTURE_SIZE_5_0M,                          // 2432*2050
	CPTRUTE_SIZE_1_2M,							// 1216*1024
	CPTRUTE_SIZE_1408_1024,                     // 1408*1024
	CPTRUTE_SIZE_8M,                            // 3296*2472
	CPTRUTE_SIZE_2560_1920,                     // 2560*1920(5M)
	CAPTURE_SIZE_960H,                          // 960*576(PAL) 960*480(NTSC)
	CAPTURE_SIZE_960_720,                       // 960*720
	CAPTURE_SIZE_NHD,							// 640*360
	CAPTURE_SIZE_QNHD,							// 320*180
	CAPTURE_SIZE_QQNHD,							// 160*90
	CAPTURE_SIZE_960_540,						// 960*540
	CAPTURE_SIZE_640_352,						// 640*352
	CAPTURE_SIZE_640_400,						// 640*400
	CAPTURE_SIZE_320_192,						// 320*192	
	CAPTURE_SIZE_320_176,						// 320*176
	CAPTURE_SIZE_NR=255  
} CAPTURE_SIZE;

// Configuration file type. For CLIENT_ExportConfigFile to use. 
typedef enum __DH_CONFIG_FILE_TYPE
{
	DH_CONFIGFILE_ALL = 0,						// All configuration file 
	DH_CONFIGFILE_LOCAL,						// Local configuration file 
	DH_CONFIGFILE_NETWORK,						// Network configuration file 
	DH_CONFIGFILE_USER,							// User configuration file 
} DH_CONFIG_FILE_TYPE;

// NTP
typedef enum __DH_TIME_ZONE_TYPE
{
	DH_TIME_ZONE_0,								// {0, 0*3600,"GMT+00:00"}
	DH_TIME_ZONE_1,								// {1, 1*3600,"GMT+01:00"}
	DH_TIME_ZONE_2,								// {2, 2*3600,"GMT+02:00"}
	DH_TIME_ZONE_3,								// {3, 3*3600,"GMT+03:00"}
	DH_TIME_ZONE_4,								// {4, 3*3600+1800,"GMT+03:30"}
	DH_TIME_ZONE_5,								// {5, 4*3600,"GMT+04:00"}
	DH_TIME_ZONE_6,								// {6, 4*3600+1800,"GMT+04:30"}
	DH_TIME_ZONE_7,								// {7, 5*3600,"GMT+05:00"}
	DH_TIME_ZONE_8,								// {8, 5*3600+1800,"GMT+05:30"}
	DH_TIME_ZONE_9,								// {9, 5*3600+1800+900,"GMT+05:45"}
	DH_TIME_ZONE_10,							// {10, 6*3600,"GMT+06:00"}
	DH_TIME_ZONE_11,							// {11, 6*3600+1800,"GMT+06:30"}
	DH_TIME_ZONE_12,							// {12, 7*3600,"GMT+07:00"}
	DH_TIME_ZONE_13,							// {13, 8*3600,"GMT+08:00"}
	DH_TIME_ZONE_14,							// {14, 9*3600,"GMT+09:00"}
	DH_TIME_ZONE_15,							// {15, 9*3600+1800,"GMT+09:30"}
	DH_TIME_ZONE_16,							// {16, 10*3600,"GMT+10:00"}
	DH_TIME_ZONE_17,							// {17, 11*3600,"GMT+11:00"}
	DH_TIME_ZONE_18,							// {18, 12*3600,"GMT+12:00"}
	DH_TIME_ZONE_19,							// {19, 13*3600,"GMT+13:00"}
	DH_TIME_ZONE_20,							// {20, -1*3600,"GMT-01:00"}
	DH_TIME_ZONE_21,							// {21, -2*3600,"GMT-02:00"}
	DH_TIME_ZONE_22,							// {22, -3*3600,"GMT-03:00"}
	DH_TIME_ZONE_23,							// {23, -3*3600-1800,"GMT-03:30"}
	DH_TIME_ZONE_24,							// {24, -4*3600,"GMT-04:00"}
	DH_TIME_ZONE_25,							// {25, -5*3600,"GMT-05:00"}
	DH_TIME_ZONE_26,							// {26, -6*3600,"GMT-06:00"}
	DH_TIME_ZONE_27,							// {27, -7*3600,"GMT-07:00"}
	DH_TIME_ZONE_28,							// {28, -8*3600,"GMT-08:00"}
	DH_TIME_ZONE_29,							// {29, -9*3600,"GMT-09:00"}
	DH_TIME_ZONE_30,							// {30, -10*3600,"GMT-10:00"}
	DH_TIME_ZONE_31,							// {31, -11*3600,"GMT-11:00"}
	DH_TIME_ZONE_32,							// {32, -12*3600,"GMT-12:00"}
} DH_TIME_ZONE_TYPE;

typedef enum _SNAP_TYPE
{
	SNAP_TYP_TIMING = 0,
	SNAP_TYP_ALARM,
	SNAP_TYP_NUM,
} SNAP_TYPE;

typedef enum _CONNECT_STATE
{
	CONNECT_STATE_UNCONNECT  = 0,
	CONNECT_STATE_CONNECTING,
	CONNECT_STATE_CONNECTED,
	CONNECT_STATE_ERROR = 255,
} CONNECT_STATE;

// Snap mode
typedef enum tagDH_TRAFFIC_SNAP_MODE
{	
	DH_TRAFFIC_SNAP_MODE_COIL = 1,				       // Loop snap
	DH_TRAFFIC_SNAP_MODE_COIL_PICANALYSIS,		       // Loop snap   picture analysis
	DH_TRAFFIC_SNAP_MODE_STREAM,				       // Video snap
	DH_TRAFFIC_SNAP_MODE_STREAM_IDENTIFY,		       // Video snap and recognize
} DH_TRAFFIC_SNAP_MODE;

// carport light type
typedef enum 
{
	NET_CARPORTLIGHT_TYPE_RED,                           // red
	NET_CARPORTLIGHT_TYPE_GREEN,                         // green
}NET_CARPORTLIGHT_TYPE;

// carport light mode
typedef enum
{
	NET_CARPORTLIGHT_MODE_OFF,                           // off 
	NET_CARPORTLIGHT_MODE_ON,                            // on
	NET_CARPORTLIGHT_MODE_GLINT,                         // glint
}NET_CARPORTLIGHT_MODE;

/////////////////////////////////Intelligent transportation related/////////////////////////////////
//Black and white list type operation
typedef enum _EM_OPERATE_TYPE
{
	NET_TRAFFIC_LIST_INSERT	    ,								// Increase the record operation
	NET_TRAFFIC_LIST_UPDATE		,								// Record update operation
	NET_TRAFFIC_LIST_REMOVE		,								// Delete the record operation
	NET_TRAFFIC_LIST_MAX		,
}EM_RECORD_OPERATE_TYPE ;

//License plate type
typedef enum _EM_NET_PLATE_TYPE
{
	NET_PLATE_TYPE_UNKNOWN					,
	NET_PLATE_TYPE_NORMAL					,		// "Normal" Blue card black card
	NET_PLATE_TYPE_YELLOW					,		// "Yellow" yellow card
	NET_PLATE_TYPE_DOUBLEYELLOW				,		// "DoubleYellow" Double yellow back card
	NET_PLATE_TYPE_POLICE					,		// "Police" Police card
	NET_PLATE_TYPE_ARMED					,		// "Armed" Armed card
	NET_PLATE_TYPE_MILITARY					,		// "Military" Force plate
	NET_PLATE_TYPE_DOUBLEMILITARY			,		// "DoubleMilitary" Forces double
	NET_PLATE_TYPE_SAR						,		// "SAR" Hong Kong and Macao SAR plate	
	NET_PLATE_TYPE_TRAINNING				,		// "Trainning" Drivers Ed plate
	NET_PLATE_TYPE_PERSONAL					,		// "Personal" Personality plate
	NET_PLATE_TYPE_AGRI						,		// "Agri" Agri-using card
	NET_PLATE_TYPE_EMBASSY					,		// "Embassy" The embassy of plate
	NET_PLATE_TYPE_MOTO						,		// "Moto" Motorcycle plate
	NET_PLATE_TYPE_TRACTOR					,		// "Tractor" The tractor plate
	NET_PLATE_TYPE_OFFICIALCAR				,		// "OfficialCar " officer's car
	NET_PLATE_TYPE_PERSONALCAR				,		// "PersonalCar" private car
	NET_PLATE_TYPE_WARCAR					,		// "WarCar"  for military use
	NET_PLATE_TYPE_OTHER					,		// "Other" The other plate
}EM_NET_PLATE_TYPE;


//The license plate color
typedef enum _EM_NET_PLATE_COLOR_TYPE
{
	NET_PLATE_COLOR_OTHER 						,				//	other colors
	NET_PLATE_COLOR_BLUE						,				//	blue		"Blue"
	NET_PLATE_COLOR_YELLOW						,				//	yellow		"Yellow"	
	NET_PLATE_COLOR_WHITE						,				//	white		"White"
	NET_PLATE_COLOR_BLACK						,				//	black		"Black"
	NET_PLATE_COLOR_YELLOW_BOTTOM_BLACK_TEXT	,				//	 Yellow Bottom Positive Figure	"YellowbottomBlackText"
	NET_PLATE_COLOR_BLUE_BOTTOM_WHITE_TEXT		,				//	blue-mask LCD" 
	NET_PLATE_COLOR_BLACK_BOTTOM_WHITE_TEXT		,				//	White on Black	"BlackBottomWhiteText"
}EM_NET_PLATE_COLOR_TYPE;

//vehicle type
typedef enum _EM_NET_VEHICLE_TYPE
{
	NET_VEHICLE_TYPE_UNKNOW					  ,				//  unknown type
	NET_VEHICLE_TYPE_MOTOR					  ,				// "Motor" Motor vehicles"		   
	NET_VEHICLE_TYPE_NON_MOTOR				  ,				// "Non-Motor"non-Motor vehicles"		
	NET_VEHICLE_TYPE_BUS					  ,				// "Bus"bus		
	NET_VEHICLE_TYPE_BICYCLE				  ,				// "Bicycle"Bicycle		
    NET_VEHICLE_TYPE_MOTORCYCLE,                        // "Motorcycle"Ħ�г�        
	NET_VEHICLE_TYPE_UNLICENSEDMOTOR		  ,				// "UnlicensedMotor": A motor vehicle without a license
	NET_VEHICLE_TYPE_LARGECAR				  ,				// "LargeCar"  LargeCar
	NET_VEHICLE_TYPE_MICROCAR				  ,				// "MicroCar" MicroCar
	NET_VEHICLE_TYPE_EMBASSYCAR				  ,				// "EmbassyCar" EmbassyCa
	NET_VEHICLE_TYPE_MARGINALCAR			  ,				// "MarginalCar" MarginalCar
	NET_VEHICLE_TYPE_AREAOUTCAR				  ,				// "AreaoutCar" AreaoutCar
	NET_VEHICLE_TYPE_FOREIGNCAR				  ,				// "ForeignCar" ForeignCar
	NET_VEHICLE_TYPE_DUALTRIWHEELMOTORCYCLE	  ,				// "DualTriWheelMotorcycle"Two or three rounds of motorcycle
	NET_VEHICLE_TYPE_LIGHTMOTORCYCLE		  , 			// "LightMotorcycle"  light motorcycle
	NET_VEHICLE_TYPE_EMBASSYMOTORCYCLE		  ,				// "EmbassyMotorcycle "The embassy of the motorcycle
	NET_VEHICLE_TYPE_MARGINALMOTORCYCLE		  ,				// "MarginalMotorcycle "Consulate motorcycle
	NET_VEHICLE_TYPE_AREAOUTMOTORCYCLE		  ,				// "AreaoutMotorcycle "Outside the motorcycle
	NET_VEHICLE_TYPE_FOREIGNMOTORCYCLE		  ,				// "ForeignMotorcycle "Foreign motorcycle
	NET_VEHICLE_TYPE_FARMTRANSMITCAR		  ,				// "FarmTransmitCar" agricultural vehicle
	NET_VEHICLE_TYPE_TRACTOR				  ,				// "Tractor" tractor
	NET_VEHICLE_TYPE_TRAILER				  ,				// "Trailer"  trailer
	NET_VEHICLE_TYPE_COACHCAR				  ,				// "CoachCar "Car coach
	NET_VEHICLE_TYPE_COACHMOTORCYCLE		  ,				// "CoachMotorcycle " coach Motorcycle
	NET_VEHICLE_TYPE_TRIALCAR				  ,				// "TrialCar" trial car 
    NET_VEHICLE_TYPE_TRIALMOTORCYCLE,                   // "TrialMotorcycle "����Ħ�г�
	NET_VEHICLE_TYPE_TEMPORARYENTRYCAR		  ,				// "TemporaryEntryCar"Temporary entry vehicle
	NET_VEHICLE_TYPE_TEMPORARYENTRYMOTORCYCLE ,				// "TemporaryEntryMotorcycle"Temporary entry of motorcycle
	NET_VEHICLE_TYPE_TEMPORARYSTEERCAR		  ,			    // "TemporarySteerCar"Temporary driving car
	NET_VEHICLE_TYPE_PASSENGERCAR			  ,				// "PassengerCar" passenger car
	NET_VEHICLE_TYPE_LARGETRUCK				  ,				// "LargeTruck" LargeTruck
	NET_VEHICLE_TYPE_MIDTRUCK				  ,				// "MidTruck" MidTruck
	NET_VEHICLE_TYPE_SALOONCAR				  ,				// "SaloonCar" SaloonCar
	NET_VEHICLE_TYPE_MICROBUS                 ,				// "Microbus" Microbus
	NET_VEHICLE_TYPE_MICROTRUCK				  ,				// "MicroTruck" MicroTruck
    NET_VEHICLE_TYPE_TRICYCLE				  ,				// "Tricycle" Tricycle
	NET_VEHICLE_TYPE_PASSERBY				  ,				// "Passerby" Passerby
}EM_NET_VEHICLE_TYPE;


//body color
typedef enum _EM_NET_VEHICLE_COLOR_TYPE
{
	NET_VEHICLE_COLOR_OTHER					,				//other color	
	NET_VEHICLE_COLOR_WHITE					,				//white		"White"
	NET_VEHICLE_COLOR_BLACK					,				//black		"Black"
	NET_VEHICLE_COLOR_RED					,				//red		"Red"
	NET_VEHICLE_COLOR_YELLOW				,				//yellow		"Yellow"
	NET_VEHICLE_COLOR_GRAY					,				//grey		"Gray"
	NET_VEHICLE_COLOR_BLUE					,				//��ɫ		"Blue"
	NET_VEHICLE_COLOR_GREEN					,				//green		"Green"
	NET_VEHICLE_COLOR_PINK					,				//pink	"Pink"
	NET_VEHICLE_COLOR_PURPLE				,				//purple		"Purple"
	NET_VEHICLE_COLOR_BROWN					,				//brown		"Brown"
}EM_NET_VEHICLE_COLOR_TYPE;

//Open the coil
typedef enum _EM_NET_TRAFFIC_CAR_CONTROL_TYPE
{
	NET_CAR_CONTROL_OTHER					,
	NET_CAR_CONTROL_OVERDUE_NO_CHECK		,				// Back inspection "OverdueNoCheck"
	NET_CAR_CONTROL_BRIGANDAGE_CAR			,				//  steal "BrigandageCar"
	NET_CAR_CONTROL_BREAKING				,				// hit-and-run "CausetroubleEscape"
	NET_CAR_CONTROL_CAUSETROUBLE_ESCAPE		,				// break rules and regulations		"Breaking"
}EM_NET_TRAFFIC_CAR_CONTROL_TYPE;

typedef enum _EM_NET_AUTHORITY_TYPE
{
	NET_AUTHORITY_UNKNOW					,
	NET_AUTHORITY_OPEN_GATE					,				//c
}EM_NET_AUTHORITY_TYPE;

typedef enum _EM_NET_RECORD_TYPE
{
	NET_RECORD_UNKNOWN,
	NET_RECORD_TRAFFICREDLIST,								// Traffic white list account records
	NET_RECORD_TRAFFICBLACKLIST,							// Traffic black list account records
	NET_RECORD_BURN_CASE,									// Imprinted case
	NET_RECORD_ACCESSCTLCARD,							    // Entrance CARD, corresponding.net you ACCESS CTL CARD
	NET_RECORD_ACCESSCTLPWD,								// Corresponding NET ACCESS password, you ACCESS CTL PWD
	NET_RECORD_ACCESSCTLCARDREC,							// Entrance guard ACCESS records, corresponding.net you ACCESS CTL CARDREC
	NET_RECORD_ACCESSCTLHOLIDAY,							// HOLIDAY RECORDSET, corresponding.net you HOLIDAY
}EM_NET_RECORD_TYPE;

// time type
typedef enum
{
	NET_TIME_TYPE_ABSLUTE,                                  //absolote time  
	NET_TIME_TYPE_RELATIVE,                                 // Relative time, relative to the video file header frame as the time basis points, the first frame corresponding to the UTC (0000-00-00 00:00:00)
}EM_TIME_TYPE;

// color type
typedef enum
{
	NET_COLOR_TYPE_RED,                                     // red
	NET_COLOR_TYPE_YELLOW,                                  // yellow
	NET_COLOR_TYPE_GREEN,                                   // green
	NET_COLOR_TYPE_CYAN,                                    // cyan
	NET_COLOR_TYPE_BLUE,                                    // glue
	NET_COLOR_TYPE_PURPLE,                                  // purple
	NET_COLOR_TYPE_BLACK,                                   // black
	NET_COLOR_TYPE_WHITE,                                   // white
	NET_COLOR_TYPE_MAX,
}EM_COLOR_TYPE;

/////////////////////////////////Face recognition related/////////////////////////////////
// Personnel type
typedef enum 
{
	PERSON_TYPE_UNKNOWN,
	PERSON_TYPE_NORMAL,                                     // common person
	PERSON_TYPE_SUSPICION,                                  // Suspects
}EM_PERSON_TYPE;

// ID type
typedef enum
{
	CERTIFICATE_TYPE_UNKNOWN,
	CERTIFICATE_TYPE_IC,                                    // ID
	CERTIFICATE_TYPE_PASSPORT,                              // passport 
}EM_CERTIFICATE_TYPE;

// Face recognition database operations
typedef enum
{
	NET_FACERECONGNITIONDB_UNKOWN, 
	NET_FACERECONGNITIONDB_ADD,                        // Add personnel information and face samples, if researchers already exists, image data and the original data
	NET_FACERECONGNITIONDB_DELETE,                     // Delete the personnel information and face samples
}EM_OPERATE_FACERECONGNITIONDB_TYPE;

// Face contrast pattern
typedef enum 
{
	NET_FACE_COMPARE_MODE_UNKOWN,
		NET_FACE_COMPARE_MODE_NORMAL,                  // normal
		NET_FACE_COMPARE_MODE_AREA,                    // Specify the face region combination area
		NET_FACE_COMPARE_MODE_AUTO,                    // Intelligent model, the algorithm according to the situation of facial regions automatically select combination
}EM_FACE_COMPARE_MODE;

// Face region
typedef enum
{
	NET_FACE_AREA_TYPE_UNKOWN,
		NET_FACE_AREA_TYPE_EYEBROW,                    // eyebrow
		NET_FACE_AREA_TYPE_EYE,                        // eye 
		NET_FACE_AREA_TYPE_NOSE,                       // nose
		NET_FACE_AREA_TYPE_MOUTH,                      // mouth
		NET_FACE_AREA_TYPE_CHEEK,                      // face
}EM_FACE_AREA_TYPE;

// face data type
typedef enum
{
	    NET_FACE_DB_TYPE_UNKOWN,
		NET_FACE_DB_TYPE_HISTORY,                      // History database, storage is to detect the human face information, usually does not contain face corresponding personnel information
		NET_FACE_DB_TYPE_BLACKLIST,                    // The blacklist database
		NET_FACE_DB_TYPE_WHITELIST,                    // The whitelist database
}EM_FACE_DB_TYPE;

// Face recognition event type
typedef enum 
{
	NET_FACERECOGNITION_ALARM_TYPE_UNKOWN,
	NET_FACERECOGNITION_ALARM_TYPE_ALL,                // blacklist and whitelist
	NET_FACERECOGNITION_ALARM_TYPE_BLACKLIST,          // The blacklist
	NET_FACERECOGNITION_ALARM_TYPE_WHITELIST,          // The whitelist
}EM_FACERECOGNITION_ALARM_TYPE;

// Face recognition face type
typedef enum
{
	EM_FACERECOGNITION_FACE_TYPE_UNKOWN,
	EM_FACERECOGNITION_FACE_TYPE_ALL,                  // All the faces 
	EM_FACERECOGNITION_FACE_TYPE_REC_SUCCESS,          // recognition success
	EM_FACERECOGNITION_FACE_TYPE_REC_FAIL,             // recognition fail
}EM_FACERECOGNITION_FACE_TYPE;

// Frame type enumeration values  
typedef enum __EM_FRAME_TYPE
{
	EM_FRAME_UNKOWN,                                   // unknown type
	EM_FRAME_TYPE_MOTION,                              // DM frame, corresponding frame information structure NET MOTION FRAM INFO
}EM_FRAME_TYPE;

/////////////////////////////////Cancelled Type/////////////////////////////////

// Configuration type. The interface that uses the enumberaiton has been canceled. Please do not use. 
typedef enum _CFG_INDEX
{
    CFG_GENERAL = 0,							// General 
	CFG_COMM,									// COM
	CFG_NET,									// Network
	CFG_RECORD,									// Record
	CFG_CAPTURE,								// Video setup
	CFG_PTZ,									// PTZ
	CFG_DETECT,									// Motion detection
	CFG_ALARM,									// Alarm 
	CFG_DISPLAY,								// Display 
	CFG_RESERVED,								// Reserved to keet type consecutive
	CFG_TITLE = 10,								// channel title 
	CFG_MAIL = 11,								// Mail function 
	CFG_EXCAPTURE = 12,							// preview video setup
	CFG_PPPOE = 13,								// pppoe setup
	CFG_DDNS = 14,								// DDNS  setup
	CFG_SNIFFER	= 15,							// Network monitor capture setup
	CFG_DSPINFO	= 16,							// Encode capacity information
	CFG_COLOR = 126,							// Color setup information 
	CFG_ALL,									// Reserved 
} CFG_INDEX;


/************************************************************************
 ** Structure Definition 
 ***********************************************************************/
//display area relative to the original display area coordinates
typedef struct
{
    double				dleft;					//left
	double				dright;					//right
	double				dtop;					//top
	double				dbottom;				//bottom
} DH_DISPLAYRREGION;
// Time
typedef struct 
{
	DWORD				dwYear;					// Year
	DWORD				dwMonth;				// Month
	DWORD				dwDay;					// Date
	DWORD				dwHour;					// Hour
	DWORD				dwMinute;				// Minute
	DWORD				dwSecond;				// Second
} NET_TIME,*LPNET_TIME;

typedef struct 
{
	DWORD				dwYear;					// Year
	DWORD				dwMonth;				// Month
	DWORD				dwDay;					// Date
	DWORD				dwHour;					// Hour
	DWORD				dwMinute;				// Minute
	DWORD				dwSecond;				// Second
	DWORD               dwMillisecond;          // Millisecond
	DWORD               dwReserved[2];          // reserved
} NET_TIME_EX,*LPNET_TIME_EX;

// The time definition in the log information
typedef struct _DHDEVTIME
{
	DWORD				second		:6;			// Second	1-60		
	DWORD				minute		:6;			// Minute	1-60		
	DWORD				hour		:5;			// Hour		1-24		
	DWORD				day			:5;			// Date		1-31		
	DWORD				month		:4;			// Month	1-12		
	DWORD				year		:6;			// Year	2000-2063	
} DHDEVTIME, *LPDHDEVTIME;

// callback data(Asynchronous interface)
typedef struct __NET_CALLBACK_DATA 
{
	int					nResultCode;			// Result code;0:Success
	char				*pBuf;					// Receive data,buffer is opened by the user,from the interface parameters
	int					nRetLen;				// the length of receive data
	LLONG				lOperateHandle;			// Operating handle
	void*				userdata;				// User parameters
	char				reserved[16];
} NET_CALLBACK_DATA, *LPNET_CALLBACK_DATA;

///////////////////////////////Monitor Related Definition ///////////////////////////////

// Frame parameter structure of Callback video data frame 
typedef struct _tagVideoFrameParam
{
	BYTE				encode;					// Encode type 
	BYTE				frametype;				// I = 0, P = 1, B = 2...
	BYTE				format;					// PAL - 0, NTSC - 1
	BYTE				size;					// CIF - 0, HD1 - 1, 2CIF - 2, D1 - 3, VGA - 4, QCIF - 5, QVGA - 6 ,
												// SVCD - 7,QQVGA - 8, SVGA - 9, XVGA - 10,WXGA - 11,SXGA - 12,WSXGA - 13,UXGA - 14,WUXGA - 15,
	DWORD				fourcc;					// If it is H264 encode it is always 0,Fill in FOURCC('X','V','I','D') in MPEG 4;
	DWORD				reserved;				// Reserved
	NET_TIME			struTime;				// Time information 
} tagVideoFrameParam;

// Frame parameter structure of audio data callback 
typedef struct _tagCBPCMDataParam
{
	BYTE				channels;				// Track amount 
	BYTE				samples;				// sample 0 - 8000, 1 - 11025, 2 - 16000, 3 - 22050, 4 - 32000, 5 - 44100, 6 - 48000
	BYTE				depth;					// Sampling depth. Value:8/16 show directly
	BYTE				param1;					// 0 - indication no symbol,1-indication with symbol
	DWORD				reserved;				// Reserved
} tagCBPCMDataParam;

// Data structure of channel video title overlay 
typedef struct _DH_CHANNEL_OSDSTRING
{
	BOOL				bEnable;				// Enable 
	DWORD				dwPosition[MAX_STRING_LINE_LEN];//Character position in each line. The value ranges from 1 to 9.Corresponding to the small keyboard.
												//		7upper left 	8upper		9upper right 
												//		4left			5middle 	6right 
												//		1down left		2down	    3down right 
	char				szStrings[MAX_STRING_LINE_LEN][MAX_PER_STRING_LEN];	// Max 6 lines. Each line max 20 bytes.
} DH_CHANNEL_OSDSTRING;

// Para struct of YUV callback
typedef struct _tagCBYUVDataParam
{
	long				nWidth;                 // Width of image
	long				nHeight;				// Height of image
	DWORD				reserved[8];			// reserved
} tagCBYUVDataParam;

///////////////////////////////Definition relate with playback///////////////////////////////

// Record file information
typedef struct
{
    unsigned int		ch;						// Channel number
    char				filename[124];			// File name 
	unsigned int        framenum;               // the total number of file frames
    unsigned int		size;					// File length 
    NET_TIME			starttime;				// Start time 
    NET_TIME			endtime;				// End time 
    unsigned int		driveno;				// HDD number 
    unsigned int		startcluster;			// Initial cluster number 
	BYTE				nRecordFileType;		// Recorded file type  0:general record;1:alarm record ;2:motion detection;3:card number record ;4:image 
	BYTE                bImportantRecID;		// 0:general record 1:Important record
	BYTE                bHint;					// Document Indexing
	BYTE                bRecType;               // 0-main stream record 1-sub1 stream record 2-sub2 stream record 3-sub3 stream record
} NET_RECORDFILE_INFO, *LPNET_RECORDFILE_INFO;

// info of enrichment record file 
typedef struct tagNET_SynopsisFileInfo
{
	DWORD				dwSize;					// struct size 
    char				szFileName[MAX_PATH];	// file name,like :\a.dav
    NET_TIME			stuStartTime;			// start time
    NET_TIME			stuEndTime;				// end time
	unsigned int		nTaskID;				// server mark,with szFileName[] a choise
	BYTE				bFileType;				// 1-record file, 2- source file
	BYTE				byMode;					// Download mode: 0 - by file downloads, 1 - according to the time to download, 2 - according to download file offset
	BYTE				bReserved[2];
	unsigned int		nFileLength;			// file length (byte)
	unsigned int		nStartFileOffset;		// Starting file offset, unit: KB
	unsigned int		nEndFileOffset;			// The end of the file offset, the unit: KB
}NET_SYNOPSISFILE_INFO, *LPNET_SYNOPSISFILE_INFO;

// Playback data callback function prototype
typedef int (CALLBACK *fDataCallBack)(LLONG lRealHandle, DWORD dwDataType, BYTE *pBuffer, DWORD dwBufSize, LDWORD dwUser);

typedef struct __NET_MULTI_PLAYBACK_PARAM 
{
	DWORD                      dwSize; 
	int                        nChannels[DH_MAX_MULTIPLAYBACK_CHANNEL_NUM]; // Preview the channel number
	int                        nChannelNum;                         // Preview the channel number
	int                        nType;                               // Playback file type, 0: ordinary video; 1: alarm video; 2: the mobile detection; 3: video card number; Picture 4:
	NET_TIME                   stStartTime;                         // Playback start time
	NET_TIME                   stEndTime;                           // The playback end time
	int                        nFPS;                                // Frame rate, 1 ~ 25
	int                        nBitRate;                            // Code flow values, 192 ~ 1024
	char                       szResolution[DH_MAX_CAPTURE_SIZE_NUM]; // Resolution, "D1", "HD1", "2 CIF", "CIF", "QCIF"
	int                        nWaitTime;                           // Timeout waiting time
	HWND                       hWnd;                                // Video playback window handle
	fDataCallBack              fDownLoadDataCallBack;               // Video data correction
	LDWORD                     dwDataUser;                          // 
}NET_MULTI_PLAYBACK_PARAM;

// record state of everyday in one month
typedef struct
{
	BYTE 	    flag[32];						// has record this day 0-no, 1-yes
	BYTE		Reserved[64];					// reserved
}NET_RECORD_STATUS, *LPNET_RECORD_STATUS;

// Asynchronous query results callback function prototype, nError = 0 means query success, nError = 1 said memory application failure, timeout nError = 2, nError = 3 said equipment return data validation is not through, nError = 4 send query request failed
typedef void (CALLBACK *fQueryRecordFileCallBack)(LLONG lQueryHandle, LPNET_RECORDFILE_INFO pFileinfos, int nFileNum, int nError, void *pReserved, LDWORD dwUser);

// CLIENT_StartQueryRecordFile Interface input parameters
typedef struct tagNET_IN_START_QUERY_RECORDFILE
{ 
	DWORD               dwSize;                            // The structure size
	int                 nChannelId;                        // To query the channel number
	int                 nRecordFileType;                   // For video query types
	int                 nStreamType;                       // Query stream type, 0 to advocate complementary code stream, 1 - the main stream, 2 - auxiliary stream
	NET_TIME            stStartTime;                       // Query starting time
	NET_TIME            stEndTime;                         // Query the end time
    char*               pchCardid;                         // card id information
	int                 nWaitTime;                         // Timeout waiting time, ms
	fQueryRecordFileCallBack cbFunc;                       // The query results callback function 
	LDWORD              dwUser;                            // userinfo
}NET_IN_START_QUERY_RECORDFILE;

typedef struct tagNET_OUT_START_QUERY_RECORDFILE
{
	DWORD                dwSize;                           //The structure size
	LLONG                lQueryHandle;                     //return handle    
}NET_OUT_START_QUERY_RECORDFILE;

typedef struct
{
	unsigned short      left;                   // 0~8192
    unsigned short      right;                  // 0~8192
    unsigned short      top;                    // 0~8192
    unsigned short      bottom;                 // 0~8192
} MotionDetectRect;

// Smart Playback Information
typedef struct 
{
	MotionDetectRect    motion_rect;             // MotionDetect area
    NET_TIME            stime;                   // PlayBack start time
    NET_TIME            etime;                   // PlayBack stop time
	BYTE                bStart;                  // 1,start,2:stop
    BYTE                reserved[116];
} IntelligentSearchPlay, *LPIntelligentSearchPlay;

// The first recording time
typedef struct  
{
	int					nChnCount;				// Channel amount
	NET_TIME			stuFurthestTime[16];	// The first recording time, valid value is 0 to (nChnCount-1).If there is no video, the first recording time is 0.
	DWORD				dwFurthestTimeAllSize;	// when channel >16,use this field.means pStuFurthestTimeAll memory size.
	NET_TIME*			pStuFurthestTimeAll;	// when channel >16,use this field.need user apply, memory size(nChnCount*sizeof(NET_TIME)).
	BYTE				bReserved[376];			// Reserved words
} NET_FURTHEST_RECORD_TIME;

// CLIENT_FindFramInfo Interface input parameters
typedef struct __NET_IN_FIND_FRAMEINFO_PRAM
{
	DWORD                 dwSize;               // The structure size 
	BOOL                  abFileName;           // Whether the file name as a valid query conditions, if the file name is valid, is don't have to fill the file information (stRecordInfo)
	char                  szFileName[MAX_PATH]; // file name
	NET_RECORDFILE_INFO   stuRecordInfo;        // file information
	DWORD                 dwFramTypeMask;       // Frame type mask, see the "frame type mask defined"
}NET_IN_FIND_FRAMEINFO_PRAM;

// CLIENT_FindFramInfo Interface input parameters
typedef struct __NET_OUT_FIND_FRAMEINFO_PRAM
{
	DWORD                 dwSize;               // The structure size 
	LLONG                 lFindHandle;          // File search handle
}NET_OUT_FIND_FRAMEINFO_PRAM;

// DM frame information
typedef struct __NET_MOTION_FRAME_INFO
{
	DWORD                 dwSize;               //  The structure size 
	NET_TIME              stuTime;              // The current frame, timestamp
	int					  nMotionRow;		    // The number of rows dynamic detection area
	int					  nMotionCol;		    // The number of columns of dynamic detection area
	BYTE				  byRegion[DH_MOTION_ROW][DH_MOTION_COL];// Detection area, up to 32 * 32 area
}NET_MOTION_FRAME_INFO;

// file frame information
typedef struct __NET_FILE_FRAME_INFO
{
	DWORD                 dwSize;               // The structure size 
	int                   nChannelId;           // channel number 
	NET_TIME              stuStartTime;         // the starting time 
	NET_TIME              stuEndTime;           // the ending time
	WORD                  wRecType;             // 0- main stream video code 1-auxiliary stream video 2 - auxiliary stream 3 auxiliary stream video
	WORD                  wFameType;            // Frame TYPE, see EM FRAM TYPE
	void*                 pFramInfo;            // Corresponding to the type of frame information, the space application by the user
}NET_FILE_FRAME_INFO;

// CLIENT_FindNextFramInfo Interface input parameters
typedef struct __NET_IN_FINDNEXT_FRAMEINFO_PRAM
{
	DWORD                 dwSize;               // The structure size
	int                   nFramCount;           // To query the frame number, 0, said all the frame information query conforms to the query conditions
}NET_IN_FINDNEXT_FRAMEINFO_PRAM;

// CLIENT_FindNextFramInfo Interface output parameters
typedef struct __NET_OUT_FINDNEXT_FRAMEINFO_PRAM
{
	DWORD                 dwSize;               // The structure size 
	NET_FILE_FRAME_INFO*   pFramInfos;           // Frame information, by the user application space, the space size of sizeof (.net FILE FRAM INFO) * nMaxFramCount
	int                   nMaxFramCount;        // The number of the frame of information in the user application
    int                   nRetFramCount;        // The actual returns the number of frame information
}NET_OUT_FINDNEXT_FRAMEINFO_PRAM;

///////////////////////////////Alarm Related Definition ///////////////////////////////

// General alarm informaiton 
typedef struct
{
	int					channelcount;
	int					alarminputcount;
	unsigned char		alarm[16];				// External alarm 
	unsigned char		motiondection[16];		// Motion detection 
	unsigned char		videolost[16];			// Video loss 
} NET_CLIENT_STATE;

// General alarm information
typedef struct
{
	int					channelcount;
	int					alarminputcount;
    unsigned char		alarm[32];				// External alarm 
	unsigned char		motiondection[32];		// Motion detection 
	unsigned char		videolost[32];			// Video loss 
	BYTE                bReserved[32];
} NET_CLIENT_STATE_EX;

// struct of input alarm
typedef struct
{
	DWORD              dwSize;
	int				   alarminputcount;
	DWORD              dwAlarmState[DH_MAX_CHANMASK]; //DWORD value is the state by bit of 32 channels,0-no alarm,1-alarm
}NET_CLIENT_ALARM_STATE;

// struct of video loss alarm
typedef struct
{
	DWORD              dwSize;
	int				   channelcount;
	DWORD              dwAlarmState[DH_MAX_CHANMASK]; //DWORD value is the state by bit of 32 channels,0-no alarm,1-alarm
}NET_CLIENT_VIDEOLOST_STATE;

// struct of motion alarm
typedef struct
{
	DWORD              dwSize;
	int				   channelcount;
	DWORD              dwAlarmState[DH_MAX_CHANMASK]; //DWORD value is the state by bit of 32 channels,0-no alarm,1-alarm
}NET_CLIENT_MOTIONDETECT_STATE;

// struct of blind alarm
typedef struct
{
	DWORD              dwSize;
	int				   channelcount;
	DWORD              dwAlarmState[DH_MAX_CHANMASK]; //DWORD value is the state by bit of 32 channels,0-no alarm,1-alarm
}NET_CLIENT_VIDEOBLIND_STATE;
// struct of querying detailed motion alarm
typedef struct
{
	DWORD              dwSize;
	int                nChannelID;				// channel id
	BOOL               bAlarm;					// alarm or not,value is TRUE/FALSE
	int                nLevel;					// alarm level,in 1/1000 as a unit
}NET_CLIENT_DETAILEDMOTION_STATE;

// Alarm IO control 
typedef struct 
{
	unsigned short		index;					// Port serial number 
	unsigned short		state;					// Port status 
} ALARM_CONTROL;

//Activation type 
typedef struct
{
	unsigned short		index;					// Port serial number 
	unsigned short		mode;					// Activation way(0:close.1:manual.2:auto); The SDK reserves the original setup if you do not set channel here. 
	BYTE				bReserved[28];			
} TRIGGER_MODE_CONTROL;

// Alarm decoder control 
typedef struct 
{
	int					decoderNo;				// Alarm decoder. It begins from 0. 
	unsigned short		alarmChn;				// Alarm output port. It begin from o., 
	unsigned short		alarmState;				// Alarm output status;1:open,0:close.
} DECODER_ALARM_CONTROL;

// Alarm information of alarm upload function
typedef struct  
{
	DWORD				dwAlarmType;			// Alarm type,when dwAlarmType = DH_UPLOAD_EVENT,dwAlarmMask and bAlarmDecoderIndex are invalid.
	DWORD				dwAlarmMask;			// Alarm information subnet mask. Bit represents each alarm status
	char				szGlobalIP[DH_MAX_IPADDR_LEN];				// Client-end IP address 
	char				szDomainName[DH_MAX_DOMAIN_NAME_LEN];		// Client-end domain name
	int					nPort;					// The port client-end connected when upload alarm 
	char				szAlarmOccurTime[DH_ALARM_OCCUR_TIME_LEN];	// Alarm occurred time 
	BYTE				bAlarmDecoderIndex;		// It means which alarm decoder. It is valid when dwAlarmType = DH_UPLOAD_DECODER_ALARM.
	BYTE				bReservedSpace[15];
} NEW_ALARM_UPLOAD;

// Smoke alarm events, alarm center
typedef struct __ALARM_UPLOAD_SMOKESENSOR_INFO
{
	DWORD			dwSize;
	char			szGlobalIP[DH_MAX_IPADDR_LEN];				// Remote IP address
	char			szDomainName[DH_MAX_DOMAIN_NAME_LEN];		// The client domain name
	int				nPort;										// When alarm upload client connection port
	char			szAlarmOccurTime[DH_ALARM_OCCUR_TIME_LEN];	// The time of the alarm
	int             nChannel;									// Alarm channel Starting from 0
	BYTE            byHighTemperature;							// 1: high temperature alarm, 0: high temperature alarm
	BYTE            bySmoke;									// 1: smoke alarm, 0: smoke alarm over
}ALARM_UPLOAD_SMOKESENSOR_INFO;

// intelligent alarm center
typedef struct __ALARM_UPLOAD_IVS_INFO
{
	DWORD				dwSize;
	NET_TIME			stuTime;                                  // occur time	
	char				szDomainName[DH_MAX_DOMAIN_NAME_LEN];	  // client domain name
	int					nChannelID;                               // channel,from 0
	char				szType[MAX_PATH];                         // alarm type,check rule
	int					nState;                                   // alarm state, 0-reset,1-setting,2-pulse
} ALARM_UPLOAD_IVS_INFO;
// Recording-changed alarm information
typedef struct
{
	int					nChannel;				// Record channel number
	char				reserved[12];
} ALARM_RECORDING_CHANGED;

// CoilFault alarm event
typedef struct __ALARM_WINGDING_INFO
{
	int                 nDriveWayID;            // road number
	int                 nWindingID;             // coil ID
	NET_TIME            stuTime;                // event happen time 
	int                 nState;                 // Device state,0 indicate fault recover,1 indicate fault happen
	DWORD				dwChannel;				// Channel of alarm
	char                reserve[28];
} ALARM_WINGDING_INFO;

// traffic congestion alarm 
typedef struct __ALARM_TRAF_CONGESTION_INFO
{
	int                 nDriveWayID;            // road number
	int                 nCongestionLevel;       // traffic congestion grade:1,2,3,4,5,6;1 indicate the most one 
	NET_TIME            stuTime;                // event happen time 
	int                 nState;                 // Device state,0 indicate fault recover,1 indicate fault happen
	DWORD				dwChannel;				// Channel of alarm
	char                reserve[28];
} ALARM_TRAF_CONGESTION_INFO;

// traffic exception alarm
typedef struct __ALARM_TRAF_EXCEPTION_INFO
{
	int                 nDriveWayID;            // road number
	NET_TIME            stuTime;                // event happen time
	int                 nState;                 // Device state,0 indicate fault recover,1 indicate fault happen
	DWORD				dwChannel;				// Channel of alarm
	char                reserve[28];
} ALARM_TRAF_EXCEPTION_INFO;

// fill equipment alarm 
typedef struct __ALARM_EQUIPMENT_FILL_INFO
{
	int                 nDriveWayID;            // road number
	NET_TIME            stuTime;                // event happen time
	int                 nState;                 // Device state,0 indicate fault recover,1 indicate fault happen
	DWORD				dwChannel;				// Channel of alarm
	char                reserve[28];
} ALARM_EQUIPMENT_FILL_INFO;

// alarm arm disarm state info
typedef struct __ALARM_ARM_DISARM_STATE_INFO
{
	BYTE                bState;                 // arm-disarm,0 means disarm,1 means arm, 2 means ForceOn
	char                reserve[31];
} ALARM_ARM_DISARM_STATE_INFO;

// 3G flow exceed state info 
typedef struct __DHDEV_3GFLOW_EXCEED_STATE_INFO
{
	BYTE                bState;                 //  3G flow exceed state,0 means not exceed,1 means exceed
	char                reserve[31];
} DHDEV_3GFLOW_EXCEED_STATE_INFO;

// alarm of speed limit (DH_DEVSTATE_SPEED_LIMIT)
typedef struct __ALARM_SPEED_LIMIT
{
	char                szType[DH_SPEEDLIMIT_TYPE_LEN];   // alarm type oLowerSpeed, UpperSpeed
	int                 iSpeedLimit;            // limit speed KM/H 
	int                 iSpeed;                 // speed KM/H
	char                szCrossingID[DH_MAX_CROSSING_ID]; // crossing ID
	DWORD				dwLongitude;			// longitude(millionth degree, 0-360)
    DWORD				dwLatidude;				// latidude(millionth degree,0-180)
	DHDEVTIME           stTime;                 // time
	BYTE                bOffline;               // 0-in real time,1-to fill 
	char				reserve[19];
}ALARM_SPEED_LIMIT;

// alarm of over loading
typedef struct __ALARM_OVER_LOADING
{
    NET_TIME			stuCurTime;             // current time
    DWORD				dwLatidude;				// latidude(millionth degree,0-180)
    DWORD				dwLongitude;			// longitude(millionth degree, 0-360)
    char				szDriverNo[DH_VEHICLE_DRIVERNO_LEN];    // driver id
    DWORD				dwCurSpeed;             // current speed
    BYTE				byReserved[128]; 
}ALARM_OVER_LOADING;

// alarm of hard braking
typedef struct __ALARM_HARD_BRAKING
{
    NET_TIME			stuCurTime;             // current time
    DWORD				dwLatidude;				// latidude(millionth degree,0-180)
    DWORD				dwLongitude;			// longitude(millionth degree, 0-360)
    char				szDriverNo[DH_VEHICLE_DRIVERNO_LEN];    // driver id
    DWORD				dwCurSpeed;             // current speed
    BYTE				byReserved[128]; 
}ALARM_HARD_BRAKING;

// alarm of smoke sensor
typedef struct __ALARM_SMOKE_SENSOR
{
	NET_TIME			stuCurTime;             // current time
	int					nChannel;               // channel
	BYTE				byHighTemperature;      // 1:hight temperature alarm start,0:end
	BYTE				bySmoke;                // 1:smoke alarm start,0:end
	BYTE				byReservrd[126];
}ALARM_SMOKE_SENSOR;

// alarm of traffic light
typedef struct _LIGHT_INFO
{
	BYTE               byDirection;            // direction: 1-left,2-right,3-straight , 4-turn round
	BYTE               byState;                // light state: 1-fault,2-normal
	BYTE               byReserved[62];         // reserved
}LIGHT_INFO;

typedef struct __ALARM_TRAFFIC_LIGHT_FAULT 
{
	NET_TIME           stTime;                 // alarm time
	int                nInfoNumber;            // info number
	LIGHT_INFO         stLightInfo[8];         // info
	BYTE               byReserved[128];        // reserved
}ALARM_TRAFFIC_LIGHT_FAULT;

// Flux alarm channel information
typedef struct __ALARM_TRAFFIC_FLUX_LANE_INFO
{
	NET_TIME            stuCurTime;            // current time
	int                 nLane;                 // lane number
	int                 nState;                // state:1-traffic jam, 2-traffic jam recover, 3-normal 4-break, 5-break recover
	int                 nFlow;                 // flow value,unit:per/second
	BYTE                byReserved[124];       // reserved
}ALARM_TRAFFIC_FLUX_LANE_INFO;

// SIP state alarm
typedef struct __ALARM_SIP_STATE
{
	int					nChannelID;
	BYTE				byStatus;               //0:succeed,1:unregistered ,2:invalid,3:registing,4:talking
	BYTE				bReserved[63];			//reserved
}ALARM_SIP_STATE;

// vehicle info uploading (DH_DEVSTATE_VIHICLE_INFO_UPLOAD)
typedef struct __ALARM_VEHICLE_INFO_UPLOAD
{
    char                szType[DH_VEHICLE_TYPE_LEN];            //type: DriverCheck ,Driver check in or check out
	char                szCheckInfo[DH_VEHICLE_INFO_LEN];       //Check in, Check out
    char                szDirverNO[DH_VEHICLE_DRIVERNO_LEN];    //Driver NO. string
	DHDEVTIME           stTime;                                      // time
	BYTE                bOffline;                                    // 0-real time 1-to fill 
	char                reserved[59];                     
}ALARM_VEHICLE_INFO_UPLOAD;

// card record uploading
typedef struct __ALARM_CARD_RECORD_INFO_UPLOAD
{
	int					nChannel;				// channel id
	BOOL				bEnable;				// is recording
	char				szCardInfo[DH_MAX_CARD_INFO_LEN];	// card info
	NET_TIME			stuTime;				// card start time
	BOOL                bPreviewOverlayEn;      // preview overlay enable
	BYTE                byOverlayPos;           // overlay position,1-left top,2-left bottom,3-right top,4-right bottom
	char				reserved[59];
}ALARM_CARD_RECORD_INFO_UPLOAD;


typedef enum __ATMTradeTypes{
	TRADE_TYPE_INC = 0,							// INSERTCARD
	TRADE_TYPE_WDC,								// WITHDRAWCARD
	TRADE_TYPE_CKT,								// CHECKTIME
	TRADE_TYPE_INQ,								// Query
	TRADE_TYPE_CWD,								// draw 
	TRADE_TYPE_PIN,								// change password
	TRADE_TYPE_TFR,								// debit
	TRADE_TYPE_DEP,								// deposit
	TRADE_TYPE_NCINQ,							// card less query
	TRADE_TYPE_NCDEP,							// card less deposit
	TRADE_TYPE_OTHERS,							// other
	TRADE_TYPE_ALL,								// all
}DH_eATMTradeTypes;

typedef struct __ALARM_ATM_INFO_UPLOAD_CHNL
{
	int					nChannel;				// channel
	char				szATMID[32];			// ATM id
	char				szCardNo[32];			// card number, 6222421541208230456 
	char				szTradetime[32];		// trade time, 20111118112200 means 2011-11-18 11:22:00
	DH_eATMTradeTypes	emTradeType;			// trade type
	int					nAmount;				// trade mount	0-4294967296
	BYTE				byRerved[32];			// reserved
}ALARM_ATM_INFO_UPLOAD_CHNL;

// ATM trade info uploading info
typedef struct __ALARM_ATM_INFO_UPLOAD
{
	int							nCnt;
	ALARM_ATM_INFO_UPLOAD_CHNL	m_stAtmInfo[DH_MAX_CHANNUM];
}ALARM_ATM_INFO_UPLOAD;

// camera move alarm
typedef struct __ALARM_CAMERA_MOVE_INFO
{
	int                         nChannelNum;                        // the number of alarm channel
	unsigned char               alarmChannels[DH_MAX_CHANNUM_EX];   // the channel information of alarm happened
	BYTE                        byReserved[128];
}ALARM_CAMERA_MOVE_INFO;
// detailed motion alarm
typedef struct __ALARM_DETAILEDMOTION_CHNL_INFO
{
	DWORD              dwSize;
	int                nChannelID;    // channel
	BOOL               bAlarm;        // alarm or not,value is TRUE/FALSE
	int                nLevel;        // alarm level,in 1/1000 as a unit
}ALARM_DETAILEDMOTION_CHNL_INFO;

// storage failure alarm info
typedef struct __ALARM_STORAGE_FAILURE
{
	DWORD     dwSize;                                     // struct size
	UINT      ActionType;                                 // 0:stop 1:start
	char      szProtocol[DH_MAX_STRING_LEN];              // protocol type
	char      szServerAddr[DH_MAX_IPADDR_OR_DOMAIN_LEN];  // server device's ip 
	DWORD     dwPort;                                     // port number
	NET_TIME  stuTime;                                    // event happen time	
	int		  nChannel;									  // channel, from 1, 0 means does not distinguish
}ALARM_STORAGE_FAILURE;

// front IPC disconnect alarm info
typedef struct __ALARM_FRONTDISCONNET_INFO
{
	DWORD              dwSize;                           // struct size
	int                nChannelID;                       // channel id
	int                nAction;                          // 0:stop 1:start
	NET_TIME           stuTime;                          // event happen time
	char               szIpAddress[MAX_PATH];            // front IP's address
}ALARM_FRONTDISCONNET_INFO;

// battery low power alarm info
typedef struct __ALARM_BATTERYLOWPOWER_INFO
{
	DWORD             dwSize;                            // struct size
	int               nAction;                           // 0:stop 1:start
	int               nBatteryLeft;                      // battery left, unit:%
	NET_TIME          stTime;                            // event happen time
	
}ALARM_BATTERYLOWPOWER_INFO;

// temperature alarm info
typedef struct __ALARM_TEMPERATURE_INFO
{
	DWORD              dwSize;                           // struct size
	char               szSensorName[DH_MACHINE_NAME_NUM];// sensor name
	int                nChannelID;                       // channel id
	int                nAction;                          // 0:stop 1:start
	float              fTemperature;                     // current temperature, unit:degree centigrade
	NET_TIME           stTime;                           // event happen time
}ALARM_TEMPERATURE_INFO;

// Fatigue Driving Alarm
typedef struct __ALARM_TIREDDRIVE_INFO
{
	DWORD             dwSize;                            // struct size
	int               nAction;                           // 0:stop 1:start
	int               nDriveTime;                        // drive time, unit:minute
	NET_TIME          stTime;                            // event happen time
}ALARM_TIREDDRIVE_INFO;

//Alarm of record loss
typedef struct __ALARM_LOST_RECORD
{
	DWORD     dwSize;                                     //Structure size
	UINT      ActionType;                                 // 0: Stop, 1: start
	UINT      nChannelID;                                 // Channel ID, start from 1
	UINT      nStreamType;                                // Bit Stream type, 0: main stream; 1: extra stream1; 2: extra stream2; 3: extra stream3; 4: snapshot stream
	NET_TIME  stuTime;                                    // Event occurrence time	
}ALARM_LOST_RECORD;

// Alarm of High CPU Occupancy rate, temporarily we set the max limit as 95%
typedef struct __ALARM_HIGH_CPU
{
	DWORD     dwSize;                                     //Structure size
	UINT      ActionType;                                 // 0: Stop, 1: start
	UINT      nUsed;                                      // 1000 times of  CPU Occupancy rate
	NET_TIME  stuTime;                                    // Event occurrence time
}ALARM_HIGH_CPU;


// Alarm of netpackage loss
typedef struct __ALARM_LOST_NETPACKET
{
	DWORD     dwSize;                                     //Structure size
	UINT      ActionType;                                 //0: Stop, 1: start
	UINT      nChannelID;                                 // Channel ID, start from 1
	UINT      nStreamType;                                // Bit stream type, 0: main stream; 1: extra stream1; 2: extra stream2; 3: extra stream3; 4: snapshot stream
	char      szRemoteIP[DH_MAX_IPADDR_LEN];              // IP address of sending
	DWORD     dwRemotePort;                               // Port address of sending 
	NET_TIME  stuTime;                                    // Event occurrence time
}ALARM_LOST_NETPACKET;

// Alarm of High memory Occupancy rate, temporarily we set the max limit as 95%
typedef struct __ALARM_HIGH_MEMORY
{
	DWORD     dwSize;                                     //Structure size
	UINT      ActionType;                                 // 0: Stop, 1: start
	UINT      nUsed;                                      // 1000 times of  memory Occupancy rate
	NET_TIME  stuTime;                                    // Event occurrence time	
}ALARM_HIGH_MEMORY;
// black list snap info
typedef struct __DH_BLACKLIST_SNAP_INFO
{
	DWORD     dwSize;
	char      szPlateNumber[32];                          // number of plate
	NET_TIME  stuTime;                                    // time
}DH_BLACKLIST_SNAP_INFO;

// disk flux abnormal
typedef struct __ALARM_DISK_FLUX
{
	DWORD				dwSize;	
	DWORD				dwAction;							// 0-start, 1-stop
	DWORD				dwDataFlux;							// data flux,KB			
	NET_TIME			stuTime;							// time
} ALARM_DISK_FLUX;

// net flux abnormal
typedef struct __ALARM_NET_FLUX
{
	DWORD				dwSize;	
	DWORD				dwAction;							// 0-start, 1-stop
	DWORD				dwDataFlux;							// data flux,KB	
	NET_TIME			stuTime;							// time
} ALARM_NET_FLUX;

// fan speed abnormal
typedef struct __ALARM_FAN_SPEED
{
	DWORD				dwSize;	
	DWORD				dwAction;							// 0-start, 1-stop
	DWORD				dwIndex;							// index
	char				szName[DH_MACHINE_NAME_NUM];		// name
	NET_TIME			stuTime;							// time
	DWORD				dwCurrent;							// current speed
} ALARM_FAN_SPEED;

// disk alarm
typedef struct __ALARM_DISK_INFO 
{
	DWORD				dwSize;
	DWORD				nChannel;							// channel number
	DWORD				nHDDNumber;							// number, 0: main trank,1: extern trank1, 2: extern trank2 
	DWORD				nHDDState;							// state, 0: Unknown, 1: Running, 2: Offline, 3: Warning, 4: Failed
} ALARM_DISK_INFO;

// alarm of file system
typedef struct __ALARM_FILE_SYSTEM_INFO 
{
	DWORD				dwSize;
	char				szMountDir[MAX_PATH];				// mount name
	DWORD				nState;								// state, 0: Unkown, 1: Normal, 2: Error
} ALARM_FILE_SYSTEM_INFO;

// alarm of remote 
typedef struct __ALARM_REMOTE_ALARM_INFO
{
	DWORD      dwSize;
	int        nChannelID;                               // channel ID,from 1
	int        nState;                                   // state,0-reset,1-setting
}ALARM_REMOTE_ALARM_INFO;

// alarm of ivs
typedef struct __ALARM_IVS_INFO
{
	DWORD      dwSize;
	NET_TIME   stuTime;                                  // time	
	int        nChannelID;                               // channel ID,from 0
	char	   szType[MAX_PATH];                         // type
	int        nState;                                   // state,0-reset,1-setting,2-pulse
}ALARM_IVS_INFO;

// alarm of good weight
typedef struct __ALARM_GOODS_WEIGHT_INFO
{
	DWORD		dwSize;
	int			nAction;							// 0-start, 1-stop
	int			nAlarmType;							// 0-greater than dwMaxGoodsWeight, 1-less than dwMinGoodsWeight, 2-the change greater than dwAlarmWeight
	DWORD		dwGoodsWeight;						// good weight(kg)
	DWORD		dwSelfWeight;						// self weight(kg)
	DWORD		dwTotalWeight;						// total weight(kg)
	DWORD		dwStandardWeight;					// standard weight(kg)
	DWORD		dwWeightScale;						// weight scale
	DWORD		dwMaxGoodsWeight;					// the max(kg)
	DWORD		dwMinGoodsWeight;					// the min(kg)
	DWORD		dwAlarmWeight;						// alarm of threshold(kg)
	int			nWeightChange;						// whight change when gather(kg)
	int			nCheckTime;							// gather time(s)
} ALARM_GOODS_WEIGHT_INFO;

// goods weight upload
typedef struct __ALARM_GOODS_WEIGHT_UPLOAD_INFO 
{
	DWORD		dwSize;
	DWORD		dwGoodsWeight;						// goods weight(kg)
	DWORD		dwSelfWeight;						// self weight(kg)
	DWORD		dwTotalWeight;						// total weight(kg)
	DWORD		dwStandardWeight;					// standard weight(kg)
	DWORD		dwWeightScale;						// weight scale	
} ALARM_GOODS_WEIGHT_UPLOAD_INFO;

// GPS status info
typedef struct _NET_GPS_STATUS_INFO
{
    NET_TIME			revTime;				    // time
	char				DvrSerial[50];			    // device number
    double				longitude;				    // longitude(1/1000000,range[0-360])
    double				latidude;				    // latitude(1/1000000,range[0-180])
    double				height;					    // highness(m)
    double				angle;					    // angle(north is source point,clockwise is positive)
    double				speed;					    // speed(sea mile,speed/1000*1.852km/h)
    WORD				starCount;				    // star count
    BOOL				antennaState;			    // antenna state(true good, false bad)
    BOOL				orientationState;		    // orientation state(true orientation, false not)
	BOOL                workStae;                   // working state(true normal, false abnormity)
	int                 nAlarmCount;                 // alarm count
	int                 nAlarmState[128];            // alarm type
	BYTE                bOffline;                    // 0- real time 1-fill 
	BYTE                byRserved[127];              // reserve
} NET_GPS_STATUS_INFO,*LPNET_GPS_STATUS_INFO;

// alarm of disk burned full
typedef struct __ALARM_DISKBURNED_FULL_INFO
{
	DWORD		dwSize;
	int         nIndex;                             //index
}ALARM_DISKBURNED_FULL_INFO;

// alarm of storage low space
typedef struct tagALARM_STORAGE_LOW_SPACE_INFO 
{
	DWORD				dwSize;
	int					nAction;						// 0:start 2:stop
	char				szName[DH_EVENT_NAME_LEN];		// name
	char				szDevice[DH_STORAGE_NAME_LEN];	// device name
	char				szGroup[DH_STORAGE_NAME_LEN];	// group name 
	INT64				nTotalSpace;					// total space byte
	INT64				nFreeSpace;						// free space byte
	int					nPercent;						// used percent 
} ALARM_STORAGE_LOW_SPACE_INFO;

// storage error
typedef enum __EM_STORAGE_ERROR
{
	STORAGE_ERROR_NONE,							// no error
    STORAGE_ERROR_PATITION,						// patition error 		
	STORAGE_ERROR_INIT_FS,						// init system file error	
	STORAGE_ERROR_READ_DATA,					// read data error
	STORAGE_ERROR_WRITE_DATA,					// write data error
	STORAGE_ERROR_RAID_FAILED,					// RAID error
	STORAGE_ERROR_RAID_DEGRADED,				// RAID degranded 
    STORAGE_ERROR_ISCSI_FAILED,                     // iSCSI����
} EM_STORAGE_ERROR;

// alarm of storage failure
typedef struct __ALARM_STORAGE_FAILURE_EX
{
	DWORD				dwSize;
	int					nAction;						// 0:start 1:stop
	char				szName[DH_EVENT_NAME_LEN];		// name 
	char				szDevice[DH_STORAGE_NAME_LEN];	// device name
	char				szGroup[DH_STORAGE_NAME_LEN];	// group name
	char				szPath[MAX_PATH];				// path
	EM_STORAGE_ERROR	emError;						// error type
} ALARM_STORAGE_FAILURE_EX;

// alarm of record failed 
typedef struct __ALARM_RECORD_FAILED_INFO 
{
	DWORD				dwSize;
	int					nAction;						// 0:start 1:stop
	int					nIndex;							// index
} ALARM_RECORD_FAILED_INFO;

// storage break down
typedef struct __ALARM_STORAGE_BREAK_DOWN_INFO 
{
	DWORD				dwSize;
	int					nAction;						// 0:start 1:stop
} ALARM_STORAGE_BREAK_DOWN_INFO;

typedef enum tagEM_NET_UPS_STATUS
{
	EM_NET_UPS_SYS_SIGN=0	 	,	//System temperature the sign bit. 1: negative temperature; 0:positive temperature
	EM_NET_UPS_SYS_SHUTDOWN 	,	//System shutdown. 1: shutdown activated state
	EM_NET_UPS_SYS_TEST 		,	//System tests. 1: said tests
	EM_NET_UPS_SYS_TYPE 		,	//UPS type. 1: backup machine; 0: said online machine
	EM_NET_UPS_SYS_FAULT 		,	//UPS fault. 1: UPS internal fault; Zero: normal
	EM_NET_UPS_ELE_SUPPLY 		,	//The bypass/inverter state. 1: AC power supply; 0: batteries
	EM_NET_UPS_VOL_LOW 			,	//Low battery voltage. 1: low battery voltage; Zero: it means the battery is normal
	EM_NET_UPS_BYPASS_STATUS	,	//Mains failure. 1: mains failure; Zero: it means the mains is normal
	EM_NET_UPS_MAX = 64			,	//
}EM_NET_UPS_STATUS;

typedef struct tagNET_UPS_INFO
{
	DWORD dwSize;
	float fInputVoltage;						//Specific reference input voltage intelligent transportation special power communication protocol (V1.2), blue shield special definition
	float fInputAbnormalVoltage;				//Abnormal input voltage
	float fOutputVoltage;						//output voltage
	float fOutputCurrent;						//Output current percentage, the unit (%)
	float fInputFrequency;						//incoming frequency
	float fVoltage;								//voltage
	float fTemp;								// temperature
	BYTE  bStatusInfo[EM_NET_UPS_MAX];			//Set UPS STATUS, see.net UPS STATUS enumeration
	char  szVersionInfo[DH_MAX_VERSION_STR];	//version
}NET_UPS_INFO;

typedef struct tagALARM_COMM_PORT_EVENT_INFO
{
	DWORD           dwSize;
	UINT      		nEventAction;	// Event Action��0=Impulse Event,1=Begin a Continued Event,2=the Event End;
	NET_UPS_INFO	stUPSInfo;		// UPS's COM Information
}ALARM_COMM_PORT_EVENT_INFO;

// ininvalid of video input channel(example:)DH_ALARM_VIDEO_ININVALID
typedef struct __ALARM_VIDEO_ININVALID_INFO 
{
	DWORD               dwSize;                         // struct size
	int                 nChannelID;                     // channel ID,from 0
} ALARM_VIDEO_ININVALID_INFO;


// No Information of Event in Storage Group
typedef struct tagALARM_STORAGE_NOT_EXIST_INFO 
{
	DWORD			dwSize;
	char			szName[DH_EVENT_NAME_LEN];		// Event Name
	char			szDevice[DH_STORAGE_NAME_LEN];	// Storage Device Name
	char			szGroup[DH_STORAGE_NAME_LEN];	// Storage Droup Name
	NET_TIME		stuTime;						// the Time of Event triggering)
}ALARM_STORAGE_NOT_EXIST_INFO;

//the Type of Network Fault Event)
typedef enum __EM_NETABORT_EVENT_TYPE
{
	EM_NETABORT_EVENT_TYPE_WIRE = 0,				// Wired Network Fault Event 
	EM_NETABORT_EVENT_TYPE_WIRELESS,				// Wireless Network Fault Event 
	EM_NETABORT_EVENT_TYPE_3G,						//3G Network Fault Event
}EM_NETABORT_EVENT_TYPE;

// Network Fault Event
typedef struct tagALARM_NETABORT_INFO
{
	DWORD			dwSize;
	int				nAction;						//0=Start 1=Stop
	EM_NETABORT_EVENT_TYPE	emNetAbortType;			// Event Type
	NET_TIME		stuTime;						// Event Triggering Time
}ALARM_NETABORT_INFO;

// IP Clash Event
typedef struct tagALARM_IP_CONFLICT_INFO
{
	DWORD			dwSize;
	int				nAction;						//0=Start 1=Stop
	NET_TIME		stuTime;						// Event Triggering Time)
}ALARM_IP_CONFLICT_INFO;


// MAC Clash Evnet)
typedef struct tagALARM_MAC_CONFLICT_INFO
{
	DWORD			dwSize;
	int				nAction;						// 0=Start 1=Stop
	NET_TIME		stuTime;						// Event Triggering Time
}ALARM_MAC_CONFLICT_INFO;


// Power Type
typedef enum __EM_POWER_TYPE
{
	EM_POWER_TYPE_MAIN = 0,							// Main Power
	EM_POWER_TYPE_BACKUP,							// Spare Power
}EM_POWER_TYPE;

// the Type of Power Fault Event
typedef enum __EM_POWERFAULT_EVENT_TYPE
{
	EM_POWERFAULT_EVENT_LOST = 0					// Power Down
}EM_POWERFAULT_EVENT_TYPE;

// Power Fault Event
typedef struct tagALARM_POWERFAULT_INFO
{
	DWORD					dwSize;			
	EM_POWER_TYPE			emPowerType;			// Power Type
	EM_POWERFAULT_EVENT_TYPE	emPowerFaultEvent;	// Power Fault Event
	NET_TIME				stuTime;				// Alarm Event Begin Time 
	int						nAction;				// 0=Start 1=Stop 
}ALARM_POWERFAULT_INFO;

// Tamper Alarm Event 
typedef struct tagALARM_CHASSISINTRUDED_INFO
{
	DWORD				dwSize;
	int					nAction;				//  0=Start 1=Stop 
	NET_TIME			stuTime;				// Alarm Event Begin Time
	int                 nChannelID;             // Channel ID
}ALARM_CHASSISINTRUDED_INFO;

// Expand Module Alarm Event
typedef struct tagALARM_ALARMEXTENDED_INFO
{
	DWORD				dwSize;
	int                 nChannelID;             //  Channel ID
	int					nAction;				// 0=Start 1=Stop (1=stop) 
	NET_TIME			stuTime;				//  Alarm Event Begin Time
}ALARM_ALARMEXTENDED_INFO;


// Interphone's Initiator 
typedef enum __EM_TALKING_CALLER
{
	EM_TALKING_CALLER_UNKNOWN = 0,				// Uunbeknown Initiator 
	EM_TALKING_CALLER_PLATFORM,					//Interphone's initiator is Platform
}EM_TALKING_CALLER;

// Alarm Event TypeDH_ALARM_TALKING_INVITE Device ask Other Side InitiateInterphone Event )Corresponding Data Description Information 
typedef struct tagALARM_TALKING_INVITE_INFO
{
	DWORD				dwSize;
	EM_TALKING_CALLER	emCaller;				// Interphone's Initiator is Device Desired 
	NET_TIME			stuTime;				//Event Triggering Time
}ALARM_TALKING_INVITE_INFO;

// Sensor's Sense Method Enumeration Type
typedef enum tagNET_SENSE_METHOD
{
	NET_SENSE_UNKNOWN = -1,		//Unknowed type
	NET_SENSE_DOOR=0,			//Door Contact
	NET_SENSE_PASSIVEINFRA,		//Passive Infrared
	NET_SENSE_GAS,				//Gase Induce)
	NET_SENSE_SMOKING,			//Smoking Induce
	NET_SENSE_WATER,			//Wwater Induce)
	NET_SENSE_ACTIVEFRA,		//Initiative Infrared
	NET_SENSE_GLASS,			//Glass Broken
	NET_SENSE_EMERGENCYSWITCH,	//Emergency switch
	NET_SENSE_SHOCK,			//Shock
	NET_SENSE_DOUBLEMETHOD,		//Double Method(Infrare+Microwave)
	NET_SENSE_THREEMETHOD,		//Three Method
	NET_SENSE_TEMP,				//Temperature
	NET_SENSE_HUMIDITY,			//Humidity
	NET_SENSE_WIND,              //Wind
	NET_SENSE_CALLBUTTON,		//Call button
    NET_SENSE_GASPRESSURE,       //Gas Pressure
    NET_SENSE_GASCONCENTRATION,  //Gas Concentration
    NET_SENSE_GASFLOW,           //Gas Flow
	NET_SENSE_OTHER,			//Other
	NET_SENSE_NUM,				//Numiber of enumeration type
}NET_SENSE_METHOD;

// Local Alarm Event (DH_ALARM_ALARM_EX Update
typedef struct tagALARM_ALARM_INFO_EX2
{
	DWORD		dwSize;
	int			nChannelID;             // Channel ID
	int			nAction;				// 0=Start 1=Stop 
	NET_TIME	stuTime;				// Alarm Event Begin Time
	NET_SENSE_METHOD emSenseType;		// The Sensor's Type
}ALARM_ALARM_INFO_EX2;

// Protect/Cancel Protect Mode 
typedef enum tagNET_ALARM_MODE
{	
	NET_ALARM_MODE_UNKNOWN = -1,	// Unknown
	NET_ALARM_MODE_DISARMING,		// Cancel Protect 
	NET_ALARM_MODE_ARMING,			// Install protect
	NET_ALARM_MODE_FORCEON,			// Forceon protect
}NET_ALARM_MODE;

// ����������ģʽ
typedef enum tagNET_SCENE_MODE
{
	NET_SCENE_MODE_UNKNOWN,			// Unknown scene
	NET_SCENE_MODE_OUTDOOR,			// Outdoor mode
	NET_SCENE_MODE_INDOOR,			// Inner mode
}NET_SCENE_MODE;
// Protect Transformate Event's Information
typedef struct tagALARM_ARMMODE_CHANGE_INFO
{
	DWORD				dwSize;
	NET_TIME			stuTime;			// Alarm Event Begin Time
	NET_ALARM_MODE		bArm;				// Statue of Transformated
	NET_SCENE_MODE		emSceneMode;		// ContextualMode
}ALARM_ARMMODE_CHANGE_INFO;

// defence zone type
typedef enum
{
	NET_DEFENCEAREA_TYPE_UNKNOWN,		//Unknown Type Defence Area
	NET_DEFENCEAREA_TYPE_ALARM,			// Switching Value Defence Area 
}NET_DEFENCEAREA_TYPE;

// Bypass Statue Type
typedef enum
{
	NET_BYPASS_MODE_UNKNOW,			//UnknownBypass Statue
	NET_BYPASS_MODE_BYPASS,			//Bypass
	NET_BYPASS_MODE_NORMAL,			//Normal
	NET_BYPASS_MODE_ISOLATED,		//Isolated
}NET_BYPASS_MODE;

// The Information of Bypass's Statue Change Event
typedef struct tagALARM_BYPASSMODE_CHANGE_INFO
{
	DWORD			dwSize;
	int				nChannelID;             // Channel ID
	NET_TIME		stuTime;				// Alarm Event Begin Time
	NET_DEFENCEAREA_TYPE emDefenceType;		// Defence Area Type
	int				nIsExtend;				// Whether or not It Is Expand��Channel��Defence Area ��1=Expand Channel��0=Non Expand Channel)
	NET_BYPASS_MODE	emMode;					// Changed Mode 
}ALARM_BYPASSMODE_CHANGE_INFO;


// Emergency Event��Correspond DH_URGENCY_ALARM_EX2,DH_URGENCY_ALARM_EX Update��That is Artificially Triggered Emergency,General Treatment is Linkage External Communications Functions for Help��
typedef struct tagALARM_URGENCY_ALARM_EX2 
{
	DWORD		dwSize;
	NET_TIME	stuTime;			//Event Begin Time
    DWORD           nID;                         // ���ڱ�ʶ��ͬ�Ľ����¼�
}ALARM_URGENCY_ALARM_EX2;

// Alarm Input Source Event Details(As Long As There Will Have to Change The Input Event, Regardless of the Current Mode of The Defence Zone Can not be Shielded)
typedef struct tagALARM_INPUT_SOURCE_SIGNAL_INFO
{
	DWORD		dwSize;
	int			nChannelID;             // Channel ID
	int			nAction;				//0=Start 1=Stop 
	NET_TIME	stuTime;				// Alarm Event Begin Time
}ALARM_INPUT_SOURCE_SIGNAL_INFO;

// Analog Input Channel Alarm Event Information(Corresponding DH_ALARM_ANALOGALARM_EVENT)��
typedef struct tagALARM_ANALOGALARM_EVENT_INFO 
{
	DWORD		dwSize;
	int			nChannelID;             // Alarm Event Begin Time
	int			nAction;				// 0=Start 1=Stop 
	NET_TIME	stuTime;				//  Alarm Event Begin Time
	NET_SENSE_METHOD	emSensorType;	// the Sensor's Type
	char		szName[DH_COMMON_STRING_128];	// Channel Name
    int             nIsValid;                           // �����Ƿ���Ч,-1:δ֪,0:��Ч,1:��Ч
    int             nStatus;                            // ����״̬, -1:δ֪,0:����,1:������Ч(��������),
                                                        // 2:������ֵ1,3:������ֵ2,4:������ֵ3,5:������ֵ4,
                                                        // 6:������ֵ1,7:������ֵ2,8:������ֵ3,9:������ֵ4
    float           fValue;                             // ̽������ֵ
    NET_TIME        stuCollectTime;                     // ���ݲɼ�ʱ��(UTC)
}ALARM_ANALOGALARM_EVENT_INFO;


// �Ž�״̬����
typedef enum tagNET_ACCESS_CTL_STATUS_TYPE
{
	NET_ACCESS_CTL_STATUS_TYPE_UNKNOWN = 0,
	NET_ACCESS_CTL_STATUS_TYPE_OPEN,		// ����
	NET_ACCESS_CTL_STATUS_TYPE_CLOSE,		// ����
}NET_ACCESS_CTL_STATUS_TYPE;

// �Ž�״̬�¼�
typedef struct tagALARM_ACCESS_CTL_STATUS_INFO 
{
	DWORD		dwSize;
	int			nDoor;						// ��ͨ����
	NET_TIME	stuTime;					// �¼�������ʱ��
	NET_ACCESS_CTL_STATUS_TYPE	emStatus;	// �Ž�״̬
}ALARM_ACCESS_CTL_STATUS_INFO;

//////////////////////////////////////////////////////////////////////////

// New Record Set Operation(Insert)Parameter
typedef struct tagNET_CTRL_RECORDSET_IN
{
	DWORD	dwSize;
	EM_NET_RECORD_TYPE	emType;					// Record Information Type
	void*	pBuf;								// Record Information Cache,The EM_NET_RECORD_TYPE Note is Details
	int		nBufLen;							// Record Information Cache Size
}NET_CTRL_RECORDSET_INSERT_IN;

// Record New Operation(Insert) Parameter
typedef struct tagNET_CTRL_RECORDSET_OUT 
{
    DWORD           dwSize;
    int             nRecNo;                             // Record Number(The Device Come Back When New Insert )
}NET_CTRL_RECORDSET_INSERT_OUT;

// Record New Operation (Insert)Parameter 
typedef struct tagNET_CTRL_RECORDSET_INSERT_PARAM 
{
    DWORD                           dwSize;
    NET_CTRL_RECORDSET_INSERT_IN    stuCtrlRecordSetInfo;       // Record Information(User Write)
    NET_CTRL_RECORDSET_INSERT_OUT   stuCtrlRecordSetResult;     // Record Information(the Device Come Back)
}NET_CTRL_RECORDSET_INSERT_PARAM;

// Record Operation Parameter
typedef struct tagNET_CTRL_RECORDSET_PARAM 
{
    DWORD               dwSize;
    EM_NET_RECORD_TYPE  emType;                         // Record Information Type
    void*               pBuf;                           // New/Renew/Inquire,It is Record Information Cache��the EM_NET_RECORD_TYPE Note is Details)
                                                        // Delete,It is Record Number(Int Model)
    int                    nBufLen;                     // Record Information Cache Size
}NET_CTRL_RECORDSET_PARAM;

// Card Statue
typedef enum tagNET_ACCESSCTLCARD_STATE
{
    NET_ACCESSCTLCARD_STATE_UNKNOWN = -1,
    NET_ACCESSCTLCARD_STATE_NORMAL = 0,                 // Normal
    NET_ACCESSCTLCARD_STATE_LOSE   = 0x01,              // Lose
    NET_ACCESSCTLCARD_STATE_LOGOFF = 0x02,              // Logoff
    NET_ACCESSCTLCARD_STATE_FREEZE = 0x04,              // Freeze
}NET_ACCESSCTLCARD_STATE;

// Card Type 
typedef enum tagNET_ACCESSCTLCARD_TYPE
{
    NET_ACCESSCTLCARD_TYPE_UNKNOWN = -1,
    NET_ACCESSCTLCARD_TYPE_GENERAL,                     // General Card
    NET_ACCESSCTLCARD_TYPE_VIP,                         // VIP Card
    NET_ACCESSCTLCARD_TYPE_GUEST,                       // Guest Card
    NET_ACCESSCTLCARD_TYPE_PATROL,                      // Patrol Card
    NET_ACCESSCTLCARD_TYPE_BLACKLIST,                   // Blacklist Card
    NET_ACCESSCTLCARD_TYPE_CORCE,                       // Corce Card
    NET_ACCESSCTLCARD_TYPE_MOTHERCARD = 0xff,           // Mother Card
}NET_ACCESSCTLCARD_TYPE;

#define DH_MAX_DOOR_NUM               32                // Max Door Number 
#define DH_MAX_TIMESECTION_NUM        32                // Max Time Section Number
#define DH_MAX_CARDNO_LEN             32                // Max Card-Number Len
#define DH_MAX_USERID_LEN             32                // Max User ID Len

//Entrance Guard Record  Information
typedef struct tagNET_RECORDSET_ACCESS_CTL_CARD
{
    DWORD           dwSize;
    int             nRecNo;                                 // Record Number,Read-Only
    NET_TIME        stuCreateTime;                          // Creat Time
    char            szCardNo[DH_MAX_CARDNO_LEN];            // Card number
    char            szUserID[DH_MAX_USERID_LEN];            // User's ID
    NET_ACCESSCTLCARD_STATE       emStatus;                 // Card Stetue
    NET_ACCESSCTLCARD_TYPE        emType;                   // Card Type
    char            szPsw[DH_MAX_CARDPWD_LEN];              // Card Password
    int             nDoorNum;                               // Valid Door Number;
    int             sznDoors[DH_MAX_DOOR_NUM];              // Privileged Door Number,That is CFG_CMD_ACCESS_EVENT Configure Array Subscript
    int             nTimeSectionNum;                        // the Number of Effective Open Time
    int             sznTimeSectionNo[DH_MAX_TIMESECTION_NUM];  // Open Time Segment Index,That is CFG_ACCESS_TIMESCHEDULE_INFO Array subscript
    int             nUserTime;                              // Frequency of Use
    NET_TIME        stuValidStartTime;                      // Valid Start Time 
    NET_TIME        stuValidEndTime;                        // Valid End Time
    BOOL            bIsValid;                               // Wether Valid,True =Valid,False=Invalid
}NET_RECORDSET_ACCESS_CTL_CARD;

// Entrance Guard Record  Information
typedef struct tagNET_RECORDSET_ACCESS_CTL_PWD 
{
    DWORD           dwSize;
    int             nRecNo;                                 // Record Number,Read-Only
    NET_TIME        stuCreateTime;                          // Creat Time
    char            szUserID[DH_MAX_USERID_LEN];            // User's ID
    char            szDoorOpenPwd[DH_MAX_CARDPWD_LEN];      // Open Password
    char            szAlarmPwd[DH_MAX_CARDPWD_LEN];         // Alarm Password
    int             nDoorNum;                               // Valid Door Number
    int             sznDoors[DH_MAX_DOOR_NUM];              // Privileged Door Number,That is CFG_CMD_ACCESS_EVENT Configure Array Subscript
}NET_RECORDSET_ACCESS_CTL_PWD;


// Door Open Method(Entrance Guard Configure,One Way of Door Work )
typedef enum tagNET_DOOR_OPEN_METHOD
{
    NET_DOOR_OPEN_METHOD_UNKNOWN = 0,
    NET_DOOR_OPEN_METHOD_PWD_ONLY,                          // Password Open is Only
    NET_DOOR_OPEN_METHOD_CARD,                              // Card  open is Only
    NET_DOOR_OPEN_METHOD_PWD_OR_CARD,                       // Password or Card
    NET_DOOR_OPEN_METHOD_CARD_FIRST,                        // First Card then Password
    NET_DOOR_OPEN_METHOD_PWD_FIRST,                         // First Card then Password
    NET_DOOR_OPEN_METHOD_SECTION,                           // Sub-Periods
}NET_DOOR_OPEN_METHOD;

// Door Open Method(Entrance Guard Event,Entrance Guard get In/Out Record��Actual Open Door Method)
typedef enum tagNET_ACCESS_DOOROPEN_METHOD
{
    NET_ACCESS_DOOROPEN_METHOD_UNKNOWN = 0,
    NET_ACCESS_DOOROPEN_METHOD_PWD_ONLY,                    // Password
    NET_ACCESS_DOOROPEN_METHOD_CARD,                        // Card
    NET_ACCESS_DOOROPEN_METHOD_CARD_FIRST,                  // First Card Then Password
    NET_ACCESS_DOOROPEN_METHOD_PWD_FIRST,                   // First Password Then Card 
    NET_ACCESS_DOOROPEN_METHOD_REMOTE,                      // Long-Range Open,Such as Through theIndoor Unit or Unlock the Door Machine Platform
    NET_ACCESS_DOOROPEN_METHOD_BUTTON,                      // Open Door Button
}NET_ACCESS_DOOROPEN_METHOD;

// Access Control card Record Information
typedef struct tagNET_RECORDSET_ACCESS_CTL_CARDREC
{
    DWORD           dwSize;
    int             nRecNo;                                 // Record Number,Read-Only
    char            szCardNo[DH_MAX_CARDNO_LEN];            // Card Number
    char            szPwd[DH_MAX_CARDPWD_LEN];              // Password
    NET_TIME        stuTime;                                // Swing Card Time
    BOOL            bStatus;                                // Swing Card Result,True is Succes,False is Fail
    NET_ACCESS_DOOROPEN_METHOD    emMethod;                 // Open Door Method
    int             nDoor;                                  // Door Number,That is CFG_CMD_ACCESS_EVENT Configure Array Subscript 
}NET_RECORDSET_ACCESS_CTL_CARDREC;

// Holiday Record Iinformation
typedef struct tagNET_RECORDSET_HOLIDAY
{
    DWORD           dwSize;
    int             nRecNo;                                 // Record Number,Read-Only
    int             nDoorNum;                               // Valid Door Number    
    int             sznDoors[DH_MAX_DOOR_NUM];              // Privileged Door Number,That is CFG_CMD_ACCESS_EVENT Configure Array Subscript
    NET_TIME        stuStartTime;                           // Start Time
    NET_TIME        stuEndTime;                             // End Time
    BOOL            bEnable;                                // Holiday Ennable
}NET_RECORDSET_HOLIDAY;

// entrance guard don't close event details information
typedef struct tagALARM_ACCESS_CTL_NOT_CLOSE_INFO 
{
    DWORD           dwSize;
    int             nDoor;                                  // Door Channel Number
    char            szDoorName[DH_MAX_DOORNAME_LEN];        // Entrance Guard Name
    NET_TIME        stuTime;                                // Alarm Event Happend Time
    int             nAction;                                // 0=Start 1=Stop 
}ALARM_ACCESS_CTL_NOT_CLOSE_INFO;

// Break Event Detail Information
typedef struct tagALARM_ACCESS_CTL_BREAK_IN_INFO 
{
    DWORD           dwSize;
    int             nDoor;                                  // Door Channel Number
    char            szDoorName[DH_MAX_DOORNAME_LEN];        // Entrance Guard Name
    NET_TIME        stuTime;                                // Alarm Event Happend Time
}ALARM_ACCESS_CTL_BREAK_IN_INFO;


// Repeatedly Entry  Event Detail Information
typedef struct tagALARM_ACCESS_CTL_REPEAT_ENTER_INFO 
{
    DWORD           dwSize;
    int             nDoor;                                  // Door Channel Number
    char            szDoorName[DH_MAX_DOORNAME_LEN];        // Entrance Guard Name
    NET_TIME        stuTime;                                // Alarm Event Happend Time
}ALARM_ACCESS_CTL_REPEAT_ENTER_INFO;


// Forced Card Swing Card  Event Detail Information
typedef struct tagALARM_ACCESS_CTL_DURESS_INFO 
{
    DWORD           dwSize;
    int             nDoor;                                  // Door Channel Number
    char            szDoorName[DH_MAX_DOORNAME_LEN];        // Entrance Guard Name
    char            szCardNo[DH_MAX_CARDNO_LEN];            // Forced Card Number
    NET_TIME        stuTime;                                // Alarm Event Happend Time
}ALARM_ACCESS_CTL_DURESS_INFO;


// Entrance Guard Event Type
typedef enum tagNET_ACCESS_CTL_EVENT_TYPE
{
    NET_ACCESS_CTL_EVENT_UNKNOWN = 0,
    NET_ACCESS_CTL_EVENT_ENTRY,                             // Get In
    NET_ACCESS_CTL_EVENT_EXIT,                              // Get Out
}NET_ACCESS_CTL_EVENT_TYPE;


// Entrance Guard Event
typedef struct tagALARM_ACCESS_CTL_EVENT_INFO 
{
    DWORD                       dwSize;
    int                         nDoor;                              // Door Channel Number
    char                        szDoorName[DH_MAX_DOORNAME_LEN];    // Entrance Guard Name
    NET_TIME                    stuTime;                            // Alarm Event Happend Time
    NET_ACCESS_CTL_EVENT_TYPE   emEventType;                        // Entrance Guard Event Type
    BOOL                        bStatus;                            // Swing Card Result,True is Succes,False is Fail
    NET_ACCESSCTLCARD_TYPE      emCardType;                         // Card Type
    NET_ACCESS_DOOROPEN_METHOD  emOpenMethod;                       // Open The Door Method
    char                        szCardNo[DH_MAX_CARDNO_LEN];        // Card Number
    char                        szPwd[DH_MAX_CARDPWD_LEN];          // Password
}ALARM_ACCESS_CTL_EVENT_INFO;

///////////////////////// Alarm of intelligent trasportation[CLIENT_StartListenEx] /////////////////////////////////////

//type DH_IVS_TRAFFIC_REALFLOWINFO(traffic real flow info)
typedef struct tagALARM_IVS_TRAFFIC_REALFLOW_INFO
{
	int                 nChannelID;                       // channel ID
	char                szName[128];                      // name
	char                bReserved1[4];                    // reserved
	double              PTS;                              // PTS(ms)
	NET_TIME_EX         UTC;                              // time of occurrence
	int                 nEventID;                         // event ID
	unsigned int        nSequence;                        // sequence
	BYTE                bEventAction;                     // event action,0 pulse,1 durable events begin, 2 durable events end
	BYTE                byReserved[3];
	int                 nLane;                            // lane number
	int                 nCount;                           // cars count
	int					nSpeed;							  // speed,km/h
	int                 nOverPercent;                     // over precent
	int                 nMetersUints;                     // meter unit 0:km,1:mile
	BYTE                bReserved[1024];                  // reserved
}ALARM_IVS_TRAFFIC_REALFLOW_INFO;

// Alarm Event Type,DH_ALARM_VEHICLE_TURNOVER's Data Describe Information
typedef struct tagALARM_VEHICEL_TURNOVER_EVENT_INFO
{
    NET_GPS_STATUS_INFO stGPSStatusInfo;                // GPS Information
    BYTE                bEventAction;                   // Event Action,0=Impluse Event,1=Continued Event Start,2=Continued Event End;
    BYTE                bReserved[1023];                // Hold Byte,For Extension.
}ALARM_VEHICEL_TURNOVER_EVENT_INFO;

// Alarm Event Type,DH_ALARM_VEHICLE_COLLISION'sData Describe Information
typedef struct tagALARM_VEHICEL_COLLISION_EVENT_INFO
{
    NET_GPS_STATUS_INFO stGPSStatusInfo;                // GPS Information
    BYTE                bEventAction;                   // Event Action,0=Impluse Event,1=Continued Event Start,2=Continued Event End;
    BYTE                bReserved[1023];                // Hold Byte,For Extension.
}ALARM_VEHICEL_COLLISION_EVENT_INFO;

// Alarm Event Type,DH_ALARM_VEHICLE_CONFIRM'sData Describe Information
typedef struct tagALARM_VEHICEL_CONFIRM_INFO
{
    DWORD               dwStructSize;                   // StructSize
    NET_GPS_STATUS_INFO stGPSStatusInfo;                // GPS Information
    BYTE                bEventAction;                   // Event Action,0=Impluse Event,1=Continued Event Start,2=Continued Event End;
    BYTE                byRserved[3];                   // Hold Byte,For Extension.
    char                szInfo[128];                    // Upload Alarm Concrete Information
}ALARM_VEHICEL_CONFIRM_INFO;

//Alarm Event Type,DH_EVENT_REGISTER_OFF's Data Describe Information
typedef struct tagEVENT_REGISTER_OFF_INFO
{
    DWORD               dwStructSize;                   // StructSize
    NET_GPS_STATUS_INFO stGPSStatusInfo;                // GPS Information
    BYTE                bEventAction;                   // Event Action,0=Impluse Event,1=Continued Event Start,2=Continued Event End;
    BYTE                byRserved[3];                   // Aline
}EVENT_REGISTER_OFF_INFO;

// Alarm Event Type,DH_ALARM_VIDEO_TIMING's Data Describe Information
typedef struct tagALARM_VIDEO_TIMING
{
    DWORD               dwStructSize;                   // StructSize
    DWORD               dwAction;                       // Event Action,0=Pause,1=Start,2=Stop
    DWORD               dwChannelID;                    // Video Channel ID
}ALARM_VIDEO_TIMING;

// Alarm Event Type,DH_ALARM_VEHICLE_LARGE_ANGLE's Data Describe Information
typedef struct tagALARM_VEHICEL_LARGE_ANGLE
{
    DWORD               dwStructSize;                   // StructSize
    NET_GPS_STATUS_INFO stGPSStatusInfo;                // GPS Information
    BYTE                bEventAction;                   // Event Action,0=Impluse Event,1=Continued Event Start,2=Continued Event End;
    BYTE                byRserved[3];                   // Aline
}ALARM_VEHICEL_LARGE_ANGLE;

// Alarm Event Type,DH_ALARM_AUDIO_ANOMALY's Data Describe Information
typedef struct tagALARM_AUDIO_ANOMALY
{
    DWORD               dwStructSize;                   // StructSize
    DWORD               dwAction;                       // Event Action,0=Pause,1=Start,2=Stop
    DWORD               dwChannelID;                    // Audio Channel ID
}ALARM_AUDIO_ANOMALY;

// Alarm Event Type,DH_ALARM_AUDIO_MUTATION's Data Describe Information
typedef struct tagALARM_AUDIO_MUTATION
{
    DWORD               dwStructSize;                   // StructSize
    DWORD               dwAction;                       // Event Action,0=Pause,1=Start,2=Stop
    DWORD               dwChannelID;                    // Audio Channel ID
}ALARM_AUDIO_MUTATION;

#define MAX_SENSORID_NUMBER 20                          // Max sensor number
#define MAX_TYRE_NUM        128                         // Max Tyre Number
// Tyre Alarm Flag Bit Corresponding Enumeration
typedef enum tagAlarmTyreFlag
{
    ALARM_TYRE_FLAG_NONE,                               // Nnon Valid Data
    ALARM_TYRE_FLAG_NORMAL,                             // Normal
    ALARM_TYRE_FLAG_HIGHPRESSURE,                       // High Pressure
    ALARM_TYRE_FLAG_LOWERPRESSURE,                      // Low Pressure
    ALARM_TYRE_FLAG_HIGHTEMP,                           // High Temperature
    ALARM_TYRE_FLAG_HIGHTEMP_HIGHPRESSURE,              // High Pressure��High Temperature
    ALARM_TYRE_FLAG_HIGHTEMP_LOWERPRESSURE,             // High Temperature ��Low Pressure

    ALARM_TYRE_FLAG_INVALID,                            // Illegal Flag Data
}EM_ALARM_TYER_FALG;

// Tyre Information Struct
typedef struct tagTYRE_INFO_UNIT
{
    DWORD               dwStructSize;                    // StructSize
    DWORD               dwSensorIDNum;                   // Sensor Number
    DWORD               dwSensorID[MAX_SENSORID_NUMBER]; // Sensor ID Information
    DWORD               dwTyreNum;                       // Tyre Number
    DWORD               dwTyrePlace;                     // Tyre at the First Few Shaft

    EM_ALARM_TYER_FALG  emAlarmFlag;                     // Tyre Alarm Flag

    int                 nTemp;                           // Temperature,Unit��
    int                 nTempLimit;                      // Temperature,Unit��

    float               fPressure;                       // Pressure,Unit:Kpa
    float               fUpperLimit;                     // Alarm Threshold Pressure Upper Limit
    float               fLowerLimit;                     // Alarm Threshold Pressure Lower Limit
    float               fVoltage;                        // Sensor Current Voltage,Unit:V
}TYRE_INFO_UNIT;

// Alarm Event Type,DH_EVENT_TYREINFO's Data Describe Information
typedef struct tagEVENT_TYRE_INFO
{
    DWORD               dwStructSize;                   // StructSize
    DWORD               dwAction;                       // Event Action,0=Pause,1=Start,2=Stop
    DWORD               dwTyreNum;                      // Tyre Number
    TYRE_INFO_UNIT      *pstuTyre;                      // Tyre Information
}EVENT_TYRE_INFO;

/////////////////////////////Audio Talk Related Definition/////////////////////////////

// Audio format information 
typedef struct
{
	BYTE				byFormatTag;			// Encode type such as 0:PCM
	WORD				nChannels;				// Track number 
	WORD				wBitsPerSample;			// Sampling depth 	
	DWORD				nSamplesPerSec;			// Sampling rate
} DH_AUDIO_FORMAT, *LPDH_AUDIO_FORMAT;

/////////////////////////////User Management Corresponding Definition /////////////////////////////

// Support the device with max 8 bits user name. Corresponding to the CLIENT_QueryUserInfo and CLIENT_OperateUserInfo.
// Right information 
typedef struct _OPR_RIGHT
{
	DWORD				dwID;
	char				name[DH_RIGHT_NAME_LENGTH];
	char				memo[DH_MEMO_LENGTH];
} OPR_RIGHT;

// User information 
typedef struct _USER_INFO
{
	DWORD				dwID;
	DWORD				dwGroupID;
	char				name[DH_USER_NAME_LENGTH];
	char				passWord[DH_USER_PSW_LENGTH];
	DWORD				dwRightNum;
	DWORD				rights[DH_MAX_RIGHT_NUM];
	char				memo[DH_MEMO_LENGTH];
	DWORD				dwReusable;				// Reuse or not;1:enable reuse;0:disable reuser 
} USER_INFO;

// User group information 
typedef struct _USER_GROUP_INFO
{
	DWORD				dwID;
	char				name[DH_USER_NAME_LENGTH];
	DWORD				dwRightNum;
	DWORD				rights[DH_MAX_RIGHT_NUM];
	char				memo[DH_MEMO_LENGTH];
} USER_GROUP_INFO;

// User information list 
typedef struct _USER_MANAGE_INFO
{
	DWORD				dwRightNum;				// Right information 
	OPR_RIGHT			rightList[DH_MAX_RIGHT_NUM];
	DWORD				dwGroupNum;				// User group information 
	USER_GROUP_INFO		groupList[DH_MAX_GROUP_NUM];
	DWORD				dwUserNum;				// User information 
	USER_INFO			userList[DH_MAX_USER_NUM];
	DWORD				dwSpecial;				// User account reuse; 1:support account to be reused. 0:Does not support account to be reused.
} USER_MANAGE_INFO;

// Support the device of max 8-bit or 16-bit name. Corresponding interface to CLIENT_QueryUserInfoEx and CLIENT_OperateUserInfoEx
#define DH_USER_NAME_LENGTH_EX		16			// User name length 
#define DH_USER_PSW_LENGTH_EX		16			// Password 

// Right information
typedef struct _OPR_RIGHT_EX
{
	DWORD				dwID;
	char				name[DH_RIGHT_NAME_LENGTH];
	char				memo[DH_MEMO_LENGTH];
} OPR_RIGHT_EX;

// User information 
typedef struct _USER_INFO_EX
{
	DWORD				dwID;
	DWORD				dwGroupID;
	char				name[DH_USER_NAME_LENGTH_EX];
	char				passWord[DH_USER_PSW_LENGTH_EX];
	DWORD				dwRightNum;
	DWORD				rights[DH_MAX_RIGHT_NUM];
	char				memo[DH_MEMO_LENGTH];
	DWORD				dwFouctionMask;			// Subnet mask,0x00000001 - support reuse  
	BYTE				byReserve[32];
} USER_INFO_EX;

// User group information 
typedef struct _USER_GROUP_INFO_EX
{
	DWORD				dwID;
	char				name[DH_USER_NAME_LENGTH_EX];
	DWORD				dwRightNum;
	DWORD				rights[DH_MAX_RIGHT_NUM];
	char				memo[DH_MEMO_LENGTH];
} USER_GROUP_INFO_EX;

// User information sheet 
typedef struct _USER_MANAGE_INFO_EX
{
	DWORD				dwRightNum;				// Right information 
	OPR_RIGHT_EX		rightList[DH_MAX_RIGHT_NUM];
	DWORD				dwGroupNum;				// User group information 
	USER_GROUP_INFO_EX  groupList[DH_MAX_GROUP_NUM];
	DWORD				dwUserNum;				// User information 
	USER_INFO_EX		userList[DH_MAX_USER_NUM];
	DWORD				dwFouctionMask;			// Subnet mask;0x00000001 - support reuse, 0x00000002 - Password has been modified , it needs to be verified.
	BYTE				byNameMaxLength;		// The supported user name max length 
	BYTE				byPSWMaxLength;			// The supported password max length
	BYTE				byReserve[254];
} USER_MANAGE_INFO_EX;

// Max support a device of 64-channel. Corresponding extension interface CLIENT_QueryUserInfoNew and CLIENT_OperateUserInfoNew
#define DH_NEW_MAX_RIGHT_NUM        1024
#define DH_NEW_USER_NAME_LENGTH	 128			// User name length
#define DH_NEW_USER_PSW_LENGTH	 128			// password

// Rights info
typedef struct _OPR_RIGHT_NEW
{
	DWORD               dwSize;
	DWORD				dwID;
	char				name[DH_RIGHT_NAME_LENGTH];
	char				memo[DH_MEMO_LENGTH];
} OPR_RIGHT_NEW;

// User info
typedef struct _USER_INFO_NEW
{
	DWORD               dwSize;
	DWORD				dwID;
	DWORD				dwGroupID;
	char				name[DH_NEW_USER_NAME_LENGTH];
	char				passWord[DH_NEW_USER_PSW_LENGTH];
	DWORD				dwRightNum;
	DWORD				rights[DH_NEW_MAX_RIGHT_NUM];
	char				memo[DH_MEMO_LENGTH];
	DWORD				dwFouctionMask;			// Sub mask,0x00000001 - Support account reusable
    NET_TIME            stuTime;                // Last Revise Time
    BYTE                byIsAnonymous;          // Whether Can Be Anonymous Logon,0=Can't Be Anonymous Logon,1=Can be Anonymous Logon
    BYTE                byReserve[7];
} USER_INFO_NEW;

// User group info
typedef struct _USER_GROUP_INFO_NEW
{
	DWORD               dwSize;
	DWORD				dwID;
	char				name[DH_USER_NAME_LENGTH_EX];
	DWORD				dwRightNum;
	DWORD				rights[DH_NEW_MAX_RIGHT_NUM];
	char				memo[DH_MEMO_LENGTH];
} USER_GROUP_INFO_NEW;

// user group information expand,user group lengthen
typedef struct _USER_GROUP_INFO_EX2
{
    DWORD               dwSize;
    DWORD               dwID;
    char                name[DH_NEW_USER_NAME_LENGTH];
    DWORD               dwRightNum;
    DWORD               rights[DH_NEW_MAX_RIGHT_NUM];
    char                memo[DH_MEMO_LENGTH];
} USER_GROUP_INFO_EX2;

// User info list
typedef struct _USER_MANAGE_INFO_NEW
{
	DWORD               dwSize;
	DWORD				dwRightNum;				// Rights info
	OPR_RIGHT_NEW		rightList[DH_NEW_MAX_RIGHT_NUM];
	DWORD				dwGroupNum;				// User group info
	USER_GROUP_INFO_NEW  groupList[DH_MAX_GROUP_NUM];
	DWORD				dwUserNum;				// User info
	USER_INFO_NEW		userList[DH_MAX_USER_NUM];
	DWORD				dwFouctionMask;			// Sub mask; 0x00000001 - Support account reusable,0x00000002 - Verification needed when change password
	BYTE				byNameMaxLength;		// Max user name length supported
	BYTE				byPSWMaxLength;			// Max password length supported
	BYTE				byReserve[254];
    USER_GROUP_INFO_EX2 groupListEx[DH_MAX_GROUP_NUM];      // User Group Information Expand
} USER_MANAGE_INFO_NEW;

///////////////////////////////Search Related Definition ///////////////////////////////

// The language types device supported 
typedef struct _DH_LANGUAGE_DEVINFO
{
	DWORD				dwLanguageNum;			// The language amount supported
	BYTE				byLanguageType[252];	// Enumeration value please refer to DH_LANGUAGE_TYPE
} DH_DEV_LANGUAGE_INFO, *LPDH_DEV_LANGUAGE_INFO;

// the IPC types device supported
typedef struct
{
	int                 nTypeCount;              // The IPC type amount supported
	BYTE                bSupportTypes[128];      // Enumeration value please refer to EM_IPC_TYPE
}DH_DEV_IPC_INFO;

//3G net flow info
typedef struct
{
	int					nStrategy;              // strategy,0: charged by flow every month 1:charged by time every month
	int                 nUplimit;               // up limit flow, by flow: MB, by time: h
	int                 nUsed;                  // have used flow, by flow: MB, by time: h
	BYTE                bReserved[64];          // reserved
}DH_DEV_3GFLOW_INFO;

typedef struct
{
	BYTE			    byEthNum;				// 3G model number
	BYTE				byReserved[255];        // reserved
}DH_DEV_3GMODULE_INFO;
typedef struct
{
	DWORD				dwId;                            // ddns server id
	char				szState[DH_MAX_DDNS_STATE_LEN];	 // ddns status
	BYTE				bReserved[512];                  // reserved
}DH_DEV_DNS_INFO;

typedef struct
{
	DWORD				dwDdnsServerNum;	
	DH_DEV_DNS_INFO     stDdns[DH_MAX_DDNS_NUM];
}DH_DEV_MULTI_DDNS_INFO;
// device URL info
typedef struct
{
	char				szURLInfo[512];         // device URL info, by string
	BYTE				bReserved[512];
}DH_DEV_URL_INFO;
// HDD informaiton 
typedef struct
{
	DWORD				dwVolume;				// HDD capacity 
	DWORD				dwFreeSpace;			// HDD free space 
	BYTE				dwStatus;				// higher 4 byte instruct hdd type, see the enum struct EM_DISK_TYPE; lower four byte instruct HDD status,0-hiberation,1-active,2-malfucntion and etc.;Devide DWORD into four BYTE
	BYTE				bDiskNum;				// HDD number
	BYTE				bSubareaNum;			// Subarea number
	BYTE				bSignal;				// Symbol. 0:local. 1:remote
} NET_DEV_DISKSTATE,*LPNET_DEV_DISKSTATE;

// Device HDD informaiton 
typedef struct _DH_HARDDISK_STATE
{
	DWORD				dwDiskNum;				// Amount 
	NET_DEV_DISKSTATE	stDisks[DH_MAX_DISKNUM];// HDD or subarea information 
} DH_HARDDISK_STATE, *LPDH_HARDDISK_STATE;

typedef DH_HARDDISK_STATE	DH_SDCARD_STATE;	// SD card. Please refer to HDD information for data structure. 

// Audio encode information 
typedef struct  
{
	DH_TALK_CODING_TYPE	encodeType;				// Encode type 
	int					nAudioBit;				// Bit:8/16
	DWORD				dwSampleRate;			// Sampling rate such as 8000 or 16000
    int                 nPacketPeriod;          // Pack Period,Unit ms
    char                reserved[60];
} DHDEV_TALKDECODE_INFO;

// The audio talk type the device supported
typedef struct 
{
	int					nSupportNum;			// Amount
	DHDEV_TALKDECODE_INFO type[64];				// Encode type 
	char				reserved[64];
} DHDEV_TALKFORMAT_LIST;

// PTZ property information
#define  NAME_MAX_LEN 16
typedef struct 
{
	DWORD				dwHighMask;				// Operation mask high bit 
	DWORD				dwLowMask;				// Operation mask low bit 
	char				szName[NAME_MAX_LEN];	// Operation protocol name 
	WORD				wCamAddrMin;			// Channel address min value
	WORD				wCamAddrMax;			// Channel address max value
	WORD				wMonAddrMin;			// Monitor address min value
	WORD				wMonAddrMax;			// Monitor address max value
	BYTE				bPresetMin;				// Preset min value
	BYTE				bPresetMax;				// Preset max value
	BYTE				bTourMin;				// Auto tour min value
	BYTE				bTourMax;				// Auto tour max value
	BYTE				bPatternMin;			// Pattern min value
	BYTE				bPatternMax;			// Pattern max value
	BYTE				bTileSpeedMin;			// Tilt speed min value
	BYTE				bTileSpeedMax;			// Tilt speed max value
	BYTE				bPanSpeedMin;			// Pan speed min value
	BYTE				bPanSpeedMax;			// Pan speed max value
	BYTE				bAuxMin;				// Aux function min value
	BYTE				bAuxMax;				// Aux function max value
	int					nInternal;				// Command interval
	char				cType;					// Protocol type
	BYTE				bReserved_1;			// Reserved
	BYTE				bFuncMask;				// function mask :0x01 - support PTZ-inside function
	BYTE				bReserved_2;
	char				Reserved[4];
} PTZ_OPT_ATTR;

// Burner informaiton 
typedef struct _NET_DEV_BURNING
{
	DWORD				dwDriverType;			// Burner driver type;0:DHFS,1:DISK,2:CDRW
	DWORD				dwBusType;				// Bus type;0:USB,1:1394,2:IDE
	DWORD				dwTotalSpace;			// Total space(KB)
	DWORD				dwRemainSpace;			// Free space(KB)
	BYTE				dwDriverName[DH_BURNING_DEV_NAMELEN];	// Burner driver name
} NET_DEV_BURNING, *LPNET_DEV_BURNING;

// Device burner informaiton 
typedef struct _DH_BURNING_DEVINFO
{
	DWORD				dwDevNum;				// Burner device amount
	NET_DEV_BURNING		stDevs[DH_MAX_BURNING_DEV_NUM];	// Each burner device information 
} DH_BURNING_DEVINFO, *LPDH_BURNING_DEVINFO;

// Burner progress 
typedef struct _DH_BURNING_PROGRESS
{
	BYTE				bBurning;				// Burner status;0:ready to burn,1:burner typs is not correct. It does not match. 
												// 2:there is no burner available,3:There is burning in process,4:Burner is not free(It is backup or buring or playback.) 
	BYTE				bRomType;				// CD type;0:private file system,1:Portable HDD or flash disk,2:CD
	BYTE				bOperateType;			// Operation type;0:free,1:backup,2:burning,3:playback from the cd 
	BYTE				bType;					// Backup pr burning status:0:stop or end,1:start,2:error,3:full,4:initializing
	NET_TIME			stTimeStart;			// Begin time 
	DWORD				dwTimeElapse;			// Burning time(second)
	DWORD				dwTotalSpace;			// Total space
	DWORD				dwRemainSpace;			// Free space
	DWORD				dwBurned;				// Burned capacity 
	WORD				dwStatus;				// Reserved
	WORD				wChannelMask;			// The burning channel mask 
} DH_BURNING_PROGRESS, *LPDH_BURNING_PROGRESS;

// Log information. Corresponding to CLIENT_QueryLog
typedef struct _DH_LOG_ITEM
{
    DHDEVTIME			time;					// Date 
    unsigned short		type;					// Type
    unsigned char		reserved;				// Reserved
    unsigned char		data;					// Data 
    unsigned char		context[8];				// Content
} DH_LOG_ITEM, *LPDH_LOG_ITEM;

// Log information. Corresponding to CLIENT_QueryLogEx, paramter reserved(int nType=1;reserved=&nType;)
typedef struct _DH_NEWLOG_ITEM
{
	DHDEVTIME			time;					// Date
	WORD				type;					// Type
	WORD				data;					// data
	char				szOperator[8]; 			// User name 
	BYTE				context[16];		    // Content	
} DH_NEWLOG_ITEM, *LPDH_NEWLOG_ITEM;

// Log information. Corresponding to CLIENT_QueryDeviceLog
typedef struct _DH_DEVICE_LOG_ITEM
{
	int					nLogType;				// Log type 
	DHDEVTIME			stuOperateTime;			// Date
	char				szOperator[16]; 		// Operator
	BYTE				bReserved[3];
	BYTE				bUnionType;				// union structure type,0:szLogContext;1:stuOldLog
	union
	{
		char			szLogContext[64];		// Log content
		struct 
		{
			DH_LOG_ITEM		stuLog;				// Old log structure 
			BYTE			bReserved[48];		// Reserved
		}stuOldLog;
	};
	char				reserved[16];
} DH_DEVICE_LOG_ITEM, *LPDH_DEVICE_LOG_ITEM;

// new Log information. Corresponding to CLIENT_QueryDeviceLog
typedef struct _DH_DEVICE_LOG_ITEM_EX
{
	int					nLogType;				// Log type 
	DHDEVTIME			stuOperateTime;			// Date
	char				szOperator[16]; 		// Operator
	BYTE				bReserved[3];
	BYTE				bUnionType;				// union structure type,0:szLogContext;1:stuOldLog
	union
	{
		char			szLogContext[64];		// Log content
		struct 
		{
			DH_LOG_ITEM		stuLog;				// Old log structure 
			BYTE			bReserved[48];		// Reserved
		}stuOldLog;
	};
	char                szOperation[32];        // Detail operation
	char				szDetailContext[4 * 1024];  // DetailContext
} DH_DEVICE_LOG_ITEM_EX, *LPDH_DEVICE_LOG_ITEM_EX;

// Record log informaiton. Corresponding to the context of log structure 
typedef struct _LOG_ITEM_RECORD
{
	DHDEVTIME			time;					// Time 
	BYTE				channel;				// Channel
	BYTE				type;					// Record type
	BYTE				reserved[2];
} LOG_ITEM_RECORD, *LPLOG_ITEM_RECORD;

typedef struct _QUERY_DEVICE_LOG_PARAM
{
	 DH_LOG_QUERY_TYPE	emLogType;				// Searched log type
	 NET_TIME			stuStartTime;			// The searched log start time
	 NET_TIME			stuEndTime;				// The searched log end time. 
	 int				nStartNum;				// The search begins from which log in one period. It shall begin with 0 if it is the first time search.
	 int				nEndNum;				// The ended log serial number in one search,the max returning number is 1024 
	 BYTE                nLogStuType;           // log struct type,0:DH_DEVICE_LOG_ITEM;1:DH_DEVICE_LOG_ITEM_EX
	BYTE                reserved[3];            // Reserved
	unsigned int        nChannelID;             // Channel no. 0:Compatible with previous all channel numbers. The channel No. begins with 1.1: The first channel.
	BYTE				bReserved[40];
} QUERY_DEVICE_LOG_PARAM;

// record information in the hard disk
typedef struct __DEV_DISK_RECORD_INFO 
{
	NET_TIME			stuBeginTime;			// The first time video
	NET_TIME			stuEndTime;				// Recently video
	char				reserved[128];
} DEV_DISK_RECORD_INFO;

// disk record time
typedef struct __DEV_DISK_RECORD_TIME
{
	NET_TIME             stuStartTime1;		    // start time 1
	NET_TIME	         stuEndTime1;			// end time 1
	BOOL	             bTwoPart;			    // have two part or not
	NET_TIME	         stuStartTime2;		    // start time 2
	NET_TIME	         stuEndTime2;			// end time 2
	BYTE			  	 bDiskNum;				// disk number 
	BYTE				 bSubareaNum;			// subarea num
	BYTE                 byReserved[62];        // reserved

}DEV_DISK_RECORD_TIME;
///////////////////////////////Control Related Definition///////////////////////////////

// HDD operation
typedef struct _DISKCTRL_PARAM
{
	DWORD				dwSize;					// Structure size. For version control.
	int					nIndex;					// Subscript of array stDisks of information structure DH_HARDDISK_STATE. It begins from 0. 
	int					ctrlType;				// Operation type
												// 0 -  clear data, 1 - set as read-write HDD , 2 -  set as read-only HDD
												// 3 - set as redundant , 4 - restore error , 5 -set as snapshot disk  
	NET_DEV_DISKSTATE	stuDisk;				// diskInfo, replace nIndex
} DISKCTRL_PARAM;

typedef struct  
{
	BYTE				bSubareaNum;			// The number of pre-partition
	BYTE				bIndex;					// Subscript of array stDisks of informaiton structure DH_HARDDISK_STATE. It begins from 0. 
	BYTE				bSubareaSize[32];		// partition Size(Percentage)
	BYTE				bReserved[30];			// Reservations
} DISKCTRL_SUBAREA;

// Alarm status
typedef struct _ALARMCTRL_PARAM
{
	DWORD				dwSize;
	int					nAlarmNo;				// Alarm channel. It begins from 0. 
	int					nAction;				// 1:activate alarm,0:stop alarm  
} ALARMCTRL_PARAM;

// Matrix control 
typedef struct _MATRIXCTRL_PARAM
{
	DWORD				dwSize;
	int					nChannelNo;				// Video input number. It begins from  0.
	int					nMatrixNo;				// Matrix output number. It begins from 0
} MATRIXCTRL_PARAM;

// Burner control 
typedef struct _BURNING_PARM
{
	int					channelMask;			// channel subnet mask. Bit means the channel to be burned.
	BYTE				devMask;				// Burner subnet mask.  Use bit to represent according to the searched burner list 
	BYTE                bySpicalChannel;        // PIP channel(Channel No.+32)
	BYTE                byReserved[2];          // Reserved
} BURNNG_PARM;

// Annex burn
typedef struct _BURNING_PARM_ATTACH
{
	BOOL				bAttachBurn;			// Whether,0:Not; 1:Yes
	BYTE				bReserved[12];			// Reservations
} BURNING_PARM_ATTACH;

// Manual snap parameter
typedef struct  _MANUAL_SNAP_PARAMETER{
	int                 nChannel;               // snap channel,start with 0
	BYTE                bySequence[64];	        // snap sequence string
	BYTE                byReserved[60];         // reserved
}MANUAL_SNAP_PARAMETER;

// local preview split parameter
typedef struct _DEVICE_LOCALPREVIEW_SLIPT_PARAMETER
{
	int                 nSliptMode;            // split mode
	int                 nSubSliptNum;          // split num, start with 1
	BYTE                byReserved[64];        // reserved
}DEVICE_LOCALPREVIEW_SLIPT_PARAMETER;

// local preview split capability
typedef struct _DEVICE_LOCALPREVIEW_SLIPT_CAP
{
	int                 nModeNumber;
	int                 nSliptMode[36];        // split array
	BYTE                byReserved[64];        // reserved
}DEVICE_LOCALPREVIEW_SLIPT_CAP;

// Crossing information
typedef struct  _CROSSING_INFO
{
	char              szCrossingID[DH_MAX_CROSSING_ID];  // Crossing ID
	DWORD             dwLatitude;				// Latitude(0-180 Degree, 30.183382 degree==120183382)
	DWORD             dwLongitude;				// Longitude(0-360 Degree, 120.178274 degree==300178274)
	WORD              wSpeedLimit;				// limit speed(KM/H)
	BYTE 	          byReserved[22];
}CROSSING_INFO;

// Route crossing information
typedef struct  _ROUTE_CROSSING_INFO
{
	BYTE 			   byCrossingNum;             // valid number is 1~DH_MAX_CROSSING_NUM
	BYTE			   byReserved1; 
	WORD               wCrossingRange;            // Crossing range(m)
	CROSSING_INFO 	   stCrossingInfo[DH_MAX_CROSSING_NUM]; // Crossing information struct array
    BYTE			   byReserved2[2044];
}ROUTE_CROSSING_INFO;

// raid control
typedef struct _CTRL_RAID_INFO
{
	char				szAction[16];						// Add,Delete
	char				szName[16];							// Raid name
	int					nType;								// type 1:Jbod     2:Raid0      3:Raid1     4:Raid5
	int					nStatus;							// status 0:unknown ,1:active,2:degraded,3:inactive,4:recovering
	int					nCntMem;							// nMember count
	int					nMember[32];						// 1,2,3,... 
	int					nCapacity;							// capacity(G)
	int					nRemainSpace;						// remain space(M)
	int					nTank;								// tank 0:main,1:tank1,2:tank2.....
	char				reserved[128];
}CTRL_RAID_INFO;

// spake disk info
typedef struct _CTRL_SPARE_DISK_INFO
{
	char				szAction[16];						// Enable
	char				szScope[16];						// Global,Local
	char				szName[16];							// name
	unsigned int        nIndex;								// disk index
    const char*         pszDevName;                         // device name
    char                reserved[124];
}CTRL_SPARE_DISK_INFO;

typedef struct _CTRL_SPARE_DISK_INFO_A
{
	int						nCnt;							// count
	CTRL_SPARE_DISK_INFO	stSpaceDiskInfo[32];			// spare disk info array
}CTRL_SPARE_DISK_INFO_A;

// Alarm arm and disarm control
typedef struct __CTRL_ARM_DISARM_PARAM
{
	BYTE                bState;                             // Arm/disarm state, 0 means disarm, 1 means arm, 2 means forced arm
    char                reserve[3];
    const char*         szDevPwd;                           // User's Password
    NET_SCENE_MODE      emSceneMode;                        // Contextual Mode
    const char*         szDevID;                            // Forwarding the Target Device ID,Null=Non Forward
    char                reserve1[16];
} CTRL_ARM_DISARM_PARAM;


typedef struct tagDHCTRL_CONNECT_WIFI_BYWPS_IN
{
	DWORD				dwSize;
	int                 nType;                         // WPS connect type,0:virtual buttons; 1:(device)pin; 2:(wifi hot point)pin
	char                szSSID[DH_MAX_SSID_LEN];       // when SSID,nType is 1 or 2,availability,biggest support 32-bit
	char                szApPin[DH_MAX_APPIN_LEN];     // APPIN, when nType = 2,availability,PIN is 8 digits,from the wife(hot point)
	char                szWLanPin[DH_MAX_APPIN_LEN];   // device pin,when nType = 1,availability.produced by the device when empty,not empty soecified by the user,need to increase in wifi
}DHCTRL_CONNECT_WIFI_BYWPS_IN;

typedef struct tagDHCTRL_CONNECT_WIFI_BYWPS_OUT
{
	DWORD				dwSize;
	char                szRetWLanPin[DH_MAX_APPIN_LEN];// return pin,when WPS's connect type is device end,this parameter is valid.
}DHCTRL_CONNECT_WIFI_BYWPS_OUT;

// CLIENT_ControlDevice interface DH_CTRL_WIFI_BY_WPS command parameter(WPS rapid configuration WIFI)
typedef struct tagDHCTRL_CONNECT_WIFI_BYWPS
{
	DWORD				dwSize;
	DHCTRL_CONNECT_WIFI_BYWPS_IN     stuWpsInfo;         // connect parameter(user to fill in)
	DHCTRL_CONNECT_WIFI_BYWPS_OUT    stuWpsResult;       // return data(device to return to)
} DHCTRL_CONNECT_WIFI_BYWPS;

// CLIENT_ControlDevice interface DH_CTRL_CLOSE_BURNER 
typedef struct tagNET_CTRL_BURNERDOOR
{
	DWORD		        dwSize;
	const char*         szBurnerName;                  // cd name,like "/dev/sda"
	BOOL                bResult;                       // operate result
    BOOL                bSafeEject;                    // Whether The Security Pop-up Drive, Data Save TRUE- pop Up Before, FALSE- Display)
}NET_CTRL_BURNERDOOR;

// CLIENT_ControlDevice interface DH_CTRL_START_PLAYAUDIO Order Parameter
typedef struct tagNET_CTRL_START_PLAYAUDIO
{
    DWORD               dwSize;
    char                szAudioPath[DH_MAX_AUDIO_PATH];
}NET_CTRL_START_PLAYAUDIO;

// CLIENT_ControlDevice�ӿڵ� DH_CTRL_START_ALARMBELL��DH_CTRL_STOP_ALARMBELL�������
typedef struct tagNET_CTRL_ALARMBELL
{
    DWORD               dwSize;
    int                 nChannelID;             // Channel ID (start from 0)            
}NET_CTRL_ALARMBELL;

// CLIENT_ControlDevice's param: DH_CTRL_ACCESS_OPEN
typedef struct tagNET_CTRL_ACCESS_OPEN
{
    DWORD               dwSize;
    int                 nChannelID;             // Channel ID (start from 0) 
}NET_CTRL_ACCESS_OPEN;

// CLIENT_ControlDevice's param: DH_CTRL_ACCESS_CLOSE
typedef struct tagNET_CTRL_ACCESS_CLOSE
{
    DWORD               dwSize;
    int                 nChannelID;             // Channel ID (start from 0) 
}NET_CTRL_ACCESS_CLOSE;

typedef enum
{
    NET_ALARM_LOCAL,                            // SwitchingValue Defence Area's Alarm Information
    NET_ALARM_ALARMEXTENDED,                    // Expand Module Alarm Event)
    NET_ALARM_TEMP,                             // Temperature Alarm Event)
    NET_ALARM_URGENCY,                          // Emergency Alarm Event)
}NET_ALARM_TYPE;

// CLIENT_ControlDevice's param: DH_CTRL_CLEAR_ALARM
typedef struct tagNET_CTRL_CLEAR_ALARM
{
    DWORD               dwSize;
    int                 nChannelID;             // Defence Area's ID
    NET_ALARM_TYPE      emAlarmType;            // Event Type
    const char*         szDevPwd;               // Landing Equipment Such As Password, Do not Use Encryption Disappear the Police, Direct Assignment of NULL
}NET_CTRL_CLEAR_ALARM;

// TV Wall Display Information Control Parameters
typedef struct tagNET_CTRL_MONITORWALL_TVINFO 
{
    DWORD               dwSize;
    int                 nMonitorWallID;         // TV Wall ID
    BOOL                bDecodeChannel;         // Display Decoding Channel Information
} NET_CTRL_MONITORWALL_TVINFO;

// CLIENT_ControlDevice's param: DH_CTRL_START_VIDEO_ANALYSE
typedef struct tagNET_CTRL_START_VIDEO_ANALYSE
{
    DWORD               dwSize; 
    int                 nChannelId;             // Channel ID  
}NET_CTRL_START_VIDEO_ANALYSE;

// CLIENT_ControlDevice's param: DH_CTRL_STOP_VIDEO_ANALYSE 
typedef struct tagNET_CTRL_STOP_VIDEO_ANALYSE
{
    DWORD               dwSize; 
    int                 nChannelId;             // Channel ID  
}NET_CTRL_STOP_VIDEO_ANALYSE;

// CLIENT_ControlDevice's param DH_CTRL_MULTIPLAYBACK_CHANNALES 
typedef struct tagNET_CTRL_MULTIPLAYBACK_CHANNALES
{
    DWORD               dwSize;
    LLONG               lPlayBackHandle;        // Playback Handle��CLIENT_MultiPlayBack Returns the Value
    int                 nChannels[DH_MAX_MULTIPLAYBACK_CHANNEL_NUM];// Preview Channel ID
    int                 nChannelNum;            // Preview Channel Number 
}NET_CTRL_MULTIPLAYBACK_CHANNALES;

// CLIENT_ControlDevice's param: DH_CTRL_SET_BYPASS 
typedef struct tagNET_CTRL_SET_BYPASS
{
    DWORD               dwSize;
    const char*         szDevPwd;               // Login Device Password
    NET_BYPASS_MODE     emMode;                 // Channel Statue
    int                 nLocalCount;            // Local Alarm Input Channnel Number
    int*                pnLocal;                // Local Alarm Input Channnel ID    
    int                 nExtendedCount;         // Expand Module Alarm Input Channel Number
    int*                pnExtended;             // Expand Module Alarm Input Channel ID
}NET_CTRL_SET_BYPASS;

// CLIENT_QueryDevState's param: DH_DEVSTATE_GET_BYPASS 
typedef struct tagNET_DEVSTATE_GET_BYPASS
{
    DWORD               dwSize;
    int                 nLocalCount;            // Local Alarm Input Channnel Number
    int*                pnLocal;                // Local Alarm Input Channnel ID    
    int                 nExtendedCount;         // Expand Module Alarm Input Channel Number
    int*                pnExtended;             // Expand Module Alarm Input Channel ID
    NET_BYPASS_MODE*    pemLocal;               // Local Alarm Input Channnel Statue
    NET_BYPASS_MODE*    pemExtended;            // Expand Module Alarm Input Channel Statue
}NET_DEVSTATE_GET_BYPASS;

// CLIENT_QueryDevState's param: DH_DEVSTATE_BURNERDOOR 
typedef struct tagNET_DEVSTATE_BURNERDOOR
{
	DWORD		        dwSize;
	const char*         szBurnerName;                  // cd name,like "/dev/sda"
	bool                bEjected;                      // ejected or not
	BYTE				Reserved[3];			       // reserved
}NET_DEVSTATE_BURNERDOOR;

// CLIENT_QueryDevState iterface DH_DEVSTATE_GET_DATA_CHECK
typedef struct tagNET_DEVSTATE_DATA_CHECK
{
    DWORD		        dwSize;                     // struct size
    const char*         szBurnerName;               // cd name,like"/dev/s",if it is multiple tracke,inclding one of path is ok
    char                szState[DH_MAX_STRING_LEN]; // "NotStart" "Verifying" "Failed" "Successed"
    int                 nPercent;                   // check percent:0-100,when state  = Verifying,availability.
}NET_DEVSTATE_DATA_CHECK;

// CLIENT_ListenServer backcall interface, fServiceCallBack support command mode
enum { 
	DH_DVR_DISCONNECT=-1,	                        // Device disconnection callback during the verification period
	DH_DVR_SERIAL_RETURN=1,                         // Device send out SN callback char* szDevSerial
	NET_DEV_AUTOREGISTER_RETURN,                    // when device registering,serial number and token to carry, corresponding NET_CB_AUTOREGISTER
};
typedef struct tagNET_CB_AUTOREGISTER
{
	DWORD			dwSize;                          // struct size
	char            szDevSerial[DH_DEV_SERIALNO_LEN];// device serial
	char            szToken[MAX_PATH];               // token
}NET_CB_AUTOREGISTER;

// public agency registration
typedef struct tagCLOUDSERVICE_CONNECT_PARAM
{
	DWORD               dwSize;   
	char                szDstIp[DH_MAX_IPADDR_EX_LEN];    // server IP
	int                 nDstPort;                         // server port 
	DWORD               dwConnectType;                    // connect type,0:main connect, 1:the tunnel connection,2:data connect,3:dynamic registration link(use 0xb4)
	char                szToken[MAX_PATH];                // the only id token
}NET_CLOUDSERVICE_CONNECT_PARAM;

typedef struct tagCLOUDSERVICE_CONNECT_RESULT
{
	DWORD               dwSize;  
	DWORD               dwConnectState;                   // connect state ,1 succeed, 2 failed
	char                szMessage[DH_MAX_CLOUDCONNECT_STATE_LEN]; // connect status info,"Success","Password Error","Network Error","Timeout"
}NET_CLOUDSERVICE_CONNECT_RESULT;
///////////////////////////////definition of configuration///////////////////////////////

//-------------------------------Device Property ---------------------------------
// Device information 
typedef struct
{
	BYTE				sSerialNumber[DH_SERIALNO_LEN];	// SN
	BYTE				byAlarmInPortNum;		// DVR alarm input amount
	BYTE				byAlarmOutPortNum;		// DVR alarm output amount
	BYTE				byDiskNum;				// DVR HDD amount 
	BYTE				byDVRType;				// DVR type.Please refer to DHDEV_DEVICE_TYPE
    union
    {
	BYTE				byChanNum;				// DVR channel amount 
        BYTE            byLeftLogTimes;         // ����½ʧ��ԭ��Ϊ�������ʱ��ͨ���˲���֪ͨ�û���ʣ���½������Ϊ0ʱ��ʾ�˲�����Ч
    };
} NET_DEVICEINFO, *LPNET_DEVICEINFO;

// Device extension info 
typedef struct
{
	BYTE				sSerialNumber[DH_SERIALNO_LEN];	// serial number
	int					nAlarmInPortNum;		// count of DVR alarm input
	int					nAlarmOutPortNum;		// count of DVR alarm output
	int					nDiskNum;				// number of DVR disk
	int					nDVRType;				// DVR type, refer to DHDEV_DEVICE_TYPE
	int					nChanNum;				// number of DVR channel
	BYTE                byLimitLoginTime;       // Online Timeout, Not Limited Access to 0, not 0 Minutes Limit Said
    BYTE                byLeftLogTimes;         // ����½ʧ��ԭ��Ϊ�������ʱ��ͨ���˲���֪ͨ�û���ʣ���½������Ϊ0ʱ��ʾ�˲�����Ч
	char				Reserved[30];			// reserved
} NET_DEVICEINFO_Ex, *LPNET_DEVICEINFO_Ex;

//Device software version information. The higher 16-bit means the main version number and low 16-bit means second version number. 
typedef struct 
{
	DWORD				dwSoftwareVersion;
	DWORD				dwSoftwareBuildDate;
	DWORD				dwDspSoftwareVersion;
	DWORD				dwDspSoftwareBuildDate;
	DWORD				dwPanelVersion;
	DWORD				dwPanelSoftwareBuildDate;
	DWORD				dwHardwareVersion;
	DWORD				dwHardwareDate;
	DWORD				dwWebVersion;
	DWORD				dwWebBuildDate;
} DH_VERSION_INFO, *LPDH_VERSION_INFO;

// Device software version information. Corresponding to CLIENT_QueryDevState
typedef struct  
{
	char				szDevSerialNo[DH_DEV_SERIALNO_LEN];	// Serial number 
	char				byDevType;							// Device type, please refer to DHDEV_DEVICE_TYPE
	char				szDevType[DH_DEV_TYPE_LEN];			// Device detailed type. String format. It may be null.
	int					nProtocalVer;						// Protocol version number 
	char				szSoftWareVersion[DH_MAX_URL_LEN];
	DWORD				dwSoftwareBuildDate;
    char                szPeripheralSoftwareVersion[DH_MAX_URL_LEN];// From the Slice Version Information, The String Format, May Be Empty
    DWORD               dwPeripheralSoftwareBuildDate;
    char                szGeographySoftwareVersion[DH_MAX_URL_LEN]; // Geographical Iinformation Positioning Chip Version Information, The String Format, May Be empty
    DWORD               dwGeographySoftwareBuildDate;
	char				szHardwareVersion[DH_MAX_URL_LEN];
	DWORD				dwHardwareDate;
	char				szWebVersion[DH_MAX_URL_LEN];
	DWORD				dwWebBuildDate;
	char				reserved[256];
} DHDEV_VERSION_INFO;

// DSP capacity description. Correspondign to CLIENT_GetDevConfig
typedef struct 
{
	DWORD				dwVideoStandardMask;	// video format mask. Bit stands for the video format device supported.
	DWORD				dwImageSizeMask;		// Resolution mask bit. Bit stands for the resolution setup devices supported.
	DWORD				dwEncodeModeMask;		// Encode mode mask bit. Bit stands for the encode mode devices supported.
	DWORD				dwStreamCap;			// The multiple-media function the devices supported
												// The first bit:main stream
												// The second bit:extra stream 1
												// The third bit:extra stream 2
												// The fifth bit: snapshot in .jpg format
	DWORD				dwImageSizeMask_Assi[8];// When the main stream is the corresponding resolution, the supported extra stream resolution mask.
	DWORD				dwMaxEncodePower;		// The highest encode capacity DSP supported
	WORD				wMaxSupportChannel;		// The max video channel amount each DSP supported.
	WORD				wChannelMaxSetSync;		// Max encode bit setup in each DSP channel  are synchronized or not ;0:does not synchronized,1:synchronized
} DH_DSP_ENCODECAP, *LPDH_DSP_ENCODECAP;

// DSP capacity description. Extensive type. Corresponding to CLIENT_QueryDevState
typedef struct 
{
	DWORD				dwVideoStandardMask;	// video format mask. Bit stands for the video format device supported.
	DWORD				dwImageSizeMask;		// Resolution mask bit. Bit stands for the resolution setup devices supported.
	DWORD				dwEncodeModeMask;		// Encode mode mask bit. Bit stands for the encode mode devices supported.
	DWORD				dwStreamCap;			// The multiple-media function the devices supported
												// The first bit:main stream
												// The second bit:extra stream 1
												// The third bit:extra stream 2
												// The fifth bit: snapshot in .jpg format
	DWORD				dwImageSizeMask_Assi[32];// When the main stream is the corresponding resolution, the supported extra stream resolution mask.
	DWORD				dwMaxEncodePower;		// The highest encode capacity DSP supported
	WORD				wMaxSupportChannel;		// The max video channel amount each DSP supported.
	WORD				wChannelMaxSetSync;		// Max encode bit setup in each DSP channel  are synchronized or not;0:do not synchronized,1:synchronized
	BYTE				bMaxFrameOfImageSize[32];// The max sampling frame rate in different resolution. Bit corresponding to the dwVideoStandardMask.
	BYTE				bEncodeCap;				// Symbol. The configuration shall meet the following requirements, otherwise the configuration is invalid.
												// 0:main stream encode capacity+extra stream encode capacity<= device encode capacity 
												// 1:main stream encode capacity +extra stream encode capacity<= device encode capacity 
												// Extra stream encode capacity <=main stream encode capacity 
												// Extra stream resolution<=main stream resolution 
												// Main stream resolution and extra stream resolution <=front-end video collection frame rate
												// 2:N5 Device
												// Extra stream encode capacity <=main stream encode capacity
												// Query  the supported resolution and the corresponding maximum frame rate
	char				reserved[95];
} DHDEV_DSP_ENCODECAP, *LPDHDEV_DSP_ENCODECAP;

// DSP extend capacity description. Corresponding to CLIENT_QueryDevState
typedef struct 
{
	DWORD				dwVideoStandardMask;	// video format mask. Bit stands for the video format device supported.
	DWORD				dwImageSizeMask;		// Resolution mask bit. Bit stands for the resolution setup devices supported.
	DWORD				dwEncodeModeMask;		// Encode mode mask bit. Bit stands for the encode mode devices supported.
	DWORD				dwStreamCap;			// The multiple-media function the devices supported.
												// The first bit:main stream
												// The second bit:extra stream 1
												// The third bit:extra stream 2
												// The forth bit:extra stream 3
												// The fifth bit: snapshot in .jpg format
	DWORD				dwImageSizeMask_Assi[3][64];// When the main stream is the corresponding resolution, the supported extra stream resolution mask, the 0,1,2 member in the group correspond extra stream1,2,3
	DWORD				dwMaxEncodePower;		// The highest encode capacity DSP supported
	WORD				wMaxSupportChannel;		// The max video channel amount each DSP supported.
	WORD				wChannelMaxSetSync;		// Max encode bit setup in each DSP channel  are synchronized or not;0:do not synchronized,1:synchronized
	BYTE				bMaxFrameOfImageSize[32];// The max sampling frame rate in different resolution. Bit corresponding to the dwVideoStandardMask.
	BYTE				bEncodeCap;				// Symbol. The configuration shall meet the following requirements, otherwise the configuration is invalid.
												// 0:main stream encode capacity+extra stream encode capacity<= device encode capacity 
												// 1:main stream encode capacity +extra stream encode capacity<= device encode capacity 
												// Extra stream encode capacity <=main stream encode capacity 
												// Extra stream resolution<=main stream resolution 
												// Main stream resolution and extra stream resolution <=front-end video collection frame rate
												// 2:N5 Device
												// Extra stream encode capacity <=main stream encode capacity
												// Query  the supported resolution and the corresponding maximum frame rate
	BYTE				btReserve1[3];			// reserved
	DWORD				dwExtraStream;			// bit0-main stream, bit1-extra stream1, bit2-extra stream2
	DWORD				dwCompression[3];		// extra stream compression

	char				reserved[108];
} DHDEV_DSP_ENCODECAP_EX, *LPDHDEV_DSP_ENCODECAP_EX;

// System information
typedef struct 
{
	DWORD				dwSize;
	/* The following are read only for device. */
	DH_VERSION_INFO		stVersion;
	DH_DSP_ENCODECAP	stDspEncodeCap;			// DSP  capacity description 
	BYTE				szDevSerialNo[DH_DEV_SERIALNO_LEN];	// SN
	BYTE				byDevType;				// device type. Please refer to enumeration DHDEV_DEVICE_TYPE
	BYTE				szDevType[DH_DEV_TYPE_LEN];	// Device detailed type. String format. It may be empty.
	BYTE				byVideoCaptureNum;		// Video port amount
	BYTE				byAudioCaptureNum;		// Audio port amount 
	BYTE				byTalkInChanNum;		// NSP
	BYTE				byTalkOutChanNum;		// NSP
	BYTE				byDecodeChanNum;		// NSP
	BYTE				byAlarmInNum;			// Alarm input port amount
	BYTE				byAlarmOutNum;			// Alarm output amount port
	BYTE				byNetIONum;				// network port amount 
	BYTE				byUsbIONum;				// USB USB port amount
	BYTE				byIdeIONum;				// IDE amount 
	BYTE				byComIONum;				// COM amount 
	BYTE				byLPTIONum;				// LPT amount
	BYTE				byVgaIONum;				// NSP
	BYTE				byIdeControlNum;		// NSP
	BYTE				byIdeControlType;		// NSP
	BYTE				byCapability;			// NSP, expansible description 
	BYTE				byMatrixOutNum;			// video matrix output amount 
	/* The following are read-write part for device */
	BYTE				byOverWrite;			// Operate when HDD is full(overwrite/stop)
	BYTE				byRecordLen;			// Video file Package length
	BYTE				byDSTEnable;			// Enable  DTS or not  1--enable. 0--disable
	WORD				wDevNo;					// Device serial number. Remote control can use this SN to control.
	BYTE				byVideoStandard;		// Video format
	BYTE				byDateFormat;			// Date format
	BYTE				byDateSprtr;			// Date separator(0:".",1:"-",2:"/")
	BYTE				byTimeFmt;				// Time separator  (0-24H,1-12H)
	BYTE				byLanguage;				// Please refer to DH_LANGUAGE_TYPE for enumeration value
} DHDEV_SYSTEM_ATTR_CFG, *LPDHDEV_SYSTEM_ATTR_CFG;

// The returned information after modify device 
typedef struct
{
	DWORD				dwType;					// Type (type of GetDevConfig and SetDevConfig)
	WORD				wResultCode;			// Returned bit;0:succeeded,1:failed,2:illegal data,3:can not set right now,4:have no right 
	WORD   				wRebootSign;			// Reboot symbol;0:do not need to reboot,1:need to reboot to get activated 
	DWORD				dwReserved[2];			// Reserved 
} DEV_SET_RESULT;

// play result
typedef struct
{
	DWORD              dwResultCode;            // result code,1:no authority,2:device not support,3:insufficient resources,4:get data error, 11:resouces are taked by advance users, 12:forbidden
	LLONG               lPlayHandle;             // play handle
	BYTE               byReserved[32];          // reserved                       
}DEV_PLAY_RESULT;

//DST(Daylight Save Time) setup
typedef struct  
{
	int					nYear;					// Year[200 - 2037]
	int					nMonth;					// Month[1 - 12]
	int					nHour;					// Hour[0 - 23]
	int					nMinute;				// Minute [0 - 59]
	int					nWeekOrDay;				// [-1 - 4]0:It means it adopts the date calculation method. 
												// 1:in week: the first week ,2:the second week,3:the third week,4: The fourth week ,-1: the last week  
	union
	{
		int				iWeekDay;				// week[0 - 6](nWeekOrDay:calculated in week )0: Sunday, 1:Monday , 2:Tuesday ,3: Wednesday,4:Thirsday ,5: Friday,6:Saturday 
		int				iDay;					// date[1 - 31] (nWeekOrDay: in date)
	};
	
	DWORD				dwReserved[8];			// Reserved 
}DH_DST_POINT;

typedef struct  
{
	DWORD				dwSize;
	int					nDSTType;				// DST position way. 0: position in date  , 1:position in week  
	DH_DST_POINT        stDSTStart;             // Enable DTS
	DH_DST_POINT        stDSTEnd;				// End DTS
	DWORD				dwReserved[16];			// Reserved
}DHDEV_DST_CFG;


// Auto maintenance setup
typedef struct
{
	DWORD				dwSize;
	BYTE				byAutoRebootDay;		// Auto reboot;0:never, 1:each day,2:each Sunday,3:Each Monday ,......
	BYTE				byAutoRebootTime;		// 0:0:00,1:1:00,......23:23:00
	BYTE				byAutoDeleteFilesTime;	// Auto delete file;0:never,1:24H,2:48H,3:72H,4:96H,5:ONE WEEK,6:ONE MONTH
	BYTE				reserved[13];			// Reserved bit
} DHDEV_AUTOMT_CFG;

// car's disk info
typedef struct  
{
	DWORD				dwSize;										// struct size,must initialize
	DWORD				dwVolume;									// disk volume
	DWORD				dwFreeSpace;								// free space MB
	BYTE				byModle[DH_MAX_HARDDISK_TYPE_LEN];			// disk mode
	BYTE				bySerialNumber[DH_MAX_HARDDISK_SERIAL_LEN];	// serial number
}DHDEV_VEHICLE_DISK;

// car's 3G mode info,the largest support DH_MAX_SIM_NUM
typedef struct
{
	DWORD				dwSize;						// struct size,must initialize
	BYTE				szIMSI[DH_MAX_SIM_LEN];		// SIM's value = 460012002778636,top three for the country code MCC,4-6for the network code MNC,the last is MSIN,a total of not more than 15 characters
	BYTE				szMDN[DH_MAX_MDN_LEN];		// MDN
}DHDEV_VEHICLE_3GMODULE;

// car basic info
typedef struct 
{
	DWORD					dwSize;								// struct size,must initialize
	DWORD					dwSoftwareBuildDate;				// soft ware build date
	char					szVersion[DH_MAX_VERSION_LEN];		// version
	char					szDevSerialNo[DH_DEV_SERIALNO_LEN];	// device serial no.
	char					szDevType[DH_DEV_TYPE_LEN];			// device type,string format,can empty
	DWORD					dwDiskNum;							// disk number
	DHDEV_VEHICLE_DISK		stuHarddiskInfo[DH_MAX_DISKNUM];	// disk info
	DWORD					dw3GModuleNum;						// count of 3G module
	DHDEV_VEHICLE_3GMODULE	stu3GModuleInfo[DH_MAX_SIM_NUM];	// 3G module info
}DHDEV_VEHICLE_INFO;

// net interface,the largest support DH_MAX_NETINTERFACE_NUM
typedef struct tagDHDEV_NETINTERFACE_INFO
{
	int             dwSize;
	BOOL			bValid;								// valid or not
	BOOL			bVirtual;							// support virtual or not
	int             nSpeed;								// theory of speed (Mbps)
	int             nDHCPState;							// 0-disable, 1-getting, 2-get succeed
	char			szName[DH_NETINTERFACE_NAME_LEN];	// net port mane
	char			szType[DH_NETINTERFACE_TYPE_LEN];	// net type
	char			szMAC[DH_MACADDR_LEN];			    // MAC addr
	char			szSSID[DH_MAX_SSID_LEN];			// SSID, if only szType == "Wireless",availability
	char            szConnStatus[DH_MAX_CONNECT_STATUS_LEN]; // Wifi,3G connect status,"Inexistence" : not exist, "Down": close "Disconn": disconnect "Connecting" "Connected"
	int             nSupportedModeNum;                  // support mode number
	char            szSupportedModes[DH_MAX_MODE_NUM][DH_MAX_MODE_LEN];// support 3G net mode	"TD-SCDMA", "WCDMA", "CDMA1x", "EDGE", "EVDO"
} DHDEV_NETINTERFACE_INFO;
//-----------------------------Video Channel Property -------------------------------

// Time period structure 															    
typedef struct 
{
	int				bEnable;				// Current record period . Bit means the four Enable functions. From the low bit to the high bit:Motion detection record, alarm record and general record, when Motion detection and alarm happened at the same time can record.
	int					iBeginHour;
	int					iBeginMin;
	int					iBeginSec;
	int					iEndHour;
	int					iEndMin;
	int					iEndSec;
} DH_TSECT, *LPDH_TSECT;

// Zone;Each margin is total lenght :8192
typedef struct 
{
   long					left;
   long					top;
   long					right;
   long					bottom;
} DH_RECT, *LPDH_RECT;

// 2 dimension point
typedef struct 
{
	short					nx;
	short					ny;
} DH_POINT, *LPDH_POINT;

// 
typedef struct
{
	int        nPointNum;                           // point number
	DH_POINT   stuPoints[DH_MAX_DETECT_REGION_NUM]; // point info
}DH_POLY_POINTS;
typedef struct  tagENCODE_WIDGET
{
	DWORD				rgbaFrontground;		// Object front view. Use bit to represent:red, green, blue and transparency.
	DWORD				rgbaBackground;			// Object back ground. Use bit to represent:red, green, blue and transparency.
	DH_RECT				rcRect;					// Position
	BYTE				bShow;					// Enable display
	BYTE                bExtFuncMask;           // Extended function,mask 
                                                // bit0 indicate week showing,0-not show 1-show 
	BYTE				byReserved[2];
} DH_ENCODE_WIDGET, *LPDH_ENCODE_WIDGET;

// Channel audio property 
typedef struct 
{
	// Video property 
	BYTE				byVideoEnable;			// Enable video;1:open,0:close 
	BYTE				byBitRateControl;		// Bit stream control;Please refer to constant Bit Stream Control definition 
	BYTE				byFramesPerSec;			// Frame rate
	BYTE				byEncodeMode;			// Encode mode:please refer to constant Encode Mode definition
	BYTE				byImageSize;			// Resolution:please refer to constant Resolution definition.
	BYTE				byImageQlty:7;			// Level 1-6
	BYTE                byImageQltyType:1;       
	WORD				wLimitStream;			// Limit stream parameter
	// Audio property 
	BYTE				byAudioEnable;			// Enable audio;1:open,0:close
	BYTE				wFormatTag;				// Audio encode mode:0:G711A,1:PCM,2:G711U,3:AMR,4:AAC 
	WORD				nChannels;				// Track amount 
	WORD				wBitsPerSample;			// Sampling depth 	
	BYTE				bAudioOverlay;			// Enabling audio
	BYTE				bH264ProfileRank;       // H.264 Profile rank(when byEncodeMode is h264, this value works ), see enmu struct EM_H264_PROFILE_RANK,when this value is 0, it means nothing
	DWORD				nSamplesPerSec;			// Sampling rate 
	BYTE				bIFrameInterval;		// 0-149. I frame interval amount. Describe the P frame amount between two I frames.
	BYTE				bScanMode;				// NSP
	BYTE				bReserved_3;
	BYTE				bReserved_4;
} DH_VIDEOENC_OPT, *LPDH_VIDEOENC_OPT;

// Image color property  
typedef struct 
{
	DH_TSECT			stSect;
	BYTE				byBrightness;			// Brightness:0-100
	BYTE				byContrast;				// Contrast:0-100
	BYTE				bySaturation;			// Saturation:0-100
	BYTE				byHue;					// Hue:0-100
	BYTE				byGainEn;				// Enable gain
	BYTE				byGain;					// Gain:0-100
	BYTE				byGamma;                // gamma 0-100
	BYTE				byReserved[1];
} DH_COLOR_CFG, *LPDH_COLOR_CFG;

// Image channel property structure 
typedef struct 
{
	//DWORD				dwSize;
	WORD				dwSize;
	BYTE				bNoise;
	BYTE				bMode;   // (vehicle's special need)model 1 (quality first): 4-way video resolution D1, frame rate 2fps, bit rate 128kbps(225MB per hour) 
	                             // Mode 2 (fluency preferred): 4-way video resolution CIF, frame rate of 12fps, bit rate 256kbps (550MB per hour) 
                                 // Mode 3 (Custom) video resolution can be defined by the user, limited the maximum capacity of 4CIF/25fps
	char				szChannelName[DH_CHAN_NAME_LEN];
	DH_VIDEOENC_OPT		stMainVideoEncOpt[DH_REC_TYPE_NUM];
	DH_VIDEOENC_OPT		stAssiVideoEncOpt[DH_N_ENCODE_AUX];		
	DH_COLOR_CFG		stColorCfg[DH_N_COL_TSECT];
	DH_ENCODE_WIDGET	stTimeOSD;
	DH_ENCODE_WIDGET	stChannelOSD;
	DH_ENCODE_WIDGET	stBlindCover[DH_N_COVERS];	// Single privacy mask zone
	BYTE				byBlindEnable;				// Privacy mask zone enable button;0x00:disable privacy mask ,0x01:privacy mask local preview ,0x10:privacy mask record and network preview,0x11: Privacy mask all
	BYTE				byBlindMask;				// Privacy mask zone subnet mask. The first bit; device local preview ;The second bit :record (and network preview ) */
	BYTE				bVolume;					// volume(0~100)
	BYTE				bVolumeEnable;				// volume enable
} DHDEV_CHANNEL_CFG, *LPDHDEV_CHANNEL_CFG;

// Preview image property 
typedef struct 
{
	DWORD				dwSize;
	DH_VIDEOENC_OPT		stPreView;
	DH_COLOR_CFG		stColorCfg[DH_N_COL_TSECT];
}DHDEV_PREVIEW_CFG;

// snap control configuration
typedef struct _config_snap_control
{
	BYTE               bySnapState[32];           // every channel's snap on-off: 0 Autumn(other configuration and event will control whether snap picture ); 1: turn on snap; 2: turn off snap
    BYTE               byReserved[480];
}DHDEV_SNAP_CONTROL_CFG;

enum _gps_mode
{
	GPS_OR_GLONASS_MODE,	// GPS,GLONASS
	GLONASS_MODE,			// GLONASS
	GPS_MODE,				// GPS
};
// DH_DEV_GPS_MODE_CFG configuration
typedef struct tagDHDEV_GPS_MODE_CFG
{
	BYTE				byGPSMode; // GPS mode
	BYTE				byRev[255];
}DHDEV_GPS_MODE_CFG;

// snap upload DH_DEV_SNAP_UPLOAD_CFG configuration
typedef struct tagDHDEV_SNAP_UPLOAD_CFG
{
	int					nUploadInterval;		// the interval time of upload picture(s)
	BYTE				byReserved[252];	
}DHDEV_SNAP_UPLOAD_CFG;

// DH_DEV_SPEED_LIMIT_CFG configuration
typedef struct tagDHDEV_SPEED_LIMIT_CFG
{
	BYTE       byEnable;						// Enable speed limit;1:enable,0:disable
	BYTE       byReserved1;      
	WORD       wMaxSpeed;						// upper speed(KM/H) 0: none limit speed, >0:limit speed
	WORD       wMinSpeed;						// low speed(KM/H) 0: none limit speed, >0:limit speed
	BYTE       byReserved2[122];  
}DHDEV_SPEED_LIMIT_CFG;

// wireless routing config
typedef struct
{
	BOOL		bEnable;						// enable
	char		szSSID[36];						// SSID
	BOOL		bHideSSID;						// hide SSID
	char		szIP[DH_MAX_IPADDR_LEN];		// IP
	char		szSubMark[DH_MAX_IPADDR_LEN];	// sub mark
	char		szGateWay[DH_MAX_IPADDR_LEN];	// gateway
	char		szCountry[32];					// country
	int			nSafeType;						// safe type: 1-no encryption; 2-WEP; 2-WPA-PSK; 3-WPA2-PSK
	int			nEncryption;					// encryption: WEP(1-auto 2-open 3-shareable); WPA-PSK/WPA2-PSK(4-TKIP 5-AES)
	char		szKey[32];						// key
	int			nChannel;						// channel
	BOOL		bAutoChannelSelect;				// auto channel select
}DHDEV_WIRELESS_ROUTING_CFG;

//-------------------------------COM property ---------------------------------

// COM basic property 
typedef struct
{
	BYTE				byDataBit;				// Data bit;0:5,1:6,2:7,3:8
	BYTE				byStopBit;				// Stop bit;0:1 bit,1:1.5 bit,2:2 bits
	BYTE				byParity;				// Parity;0: None,1: Odd;2: even;3:sign;4:empty
	BYTE				byBaudRate;				// Baud rate;0:300,1:600,2:1200,3:2400,4:4800,
												// 5:9600,6:19200,7:38400,8:57600,9:115200
} DH_COMM_PROP;

// 485 decoder setup 
typedef struct
{ 
	DH_COMM_PROP		struComm;
	BYTE				wProtocol;				// Protocol type. Corresponding to the subscript of Protocol Name List
	BYTE				bPTZType;				// 0-Compatible,local ptz 1-remote network ptz, the capability refer to DEV_ENCODER_CFG.
	BYTE				wDecoderAddress;		// Decoder address;0 - 255
	BYTE 				byMartixID;				// Matrix number 
} DH_485_CFG;

// 232 COM setup 
typedef struct
{
	DH_COMM_PROP		struComm;
	BYTE				byFunction;				// COM function,Corresponding to the subscript of Function Name list. 
	BYTE				byReserved[3];
} DH_RS232_CFG;

// COM configuration structure 
typedef struct
{
	DWORD				dwSize;

	DWORD				dwDecProListNum;										// Decoder protocol amount
	char				DecProName[DH_MAX_DECPRO_LIST_SIZE][DH_MAX_NAME_LEN];	// Protocol name list
	DH_485_CFG			stDecoder[DH_MAX_DECODER_NUM];							// Each decoder current property

	DWORD				dw232FuncNameNum;										// 232 function amount 
	char				s232FuncName[DH_MAX_232FUNCS][DH_MAX_NAME_LEN];			// Function name list 
	DH_RS232_CFG		st232[DH_MAX_232_NUM];									// Current 232 COM property 
} DHDEV_COMM_CFG;

// Extended COM configuration structure 
typedef struct
{
	DWORD				dwSize;
	
	DWORD				dwDecProListNum;			                           // Decoder protocol amount
	char				DecProName[DH_MAX_DECPRO_LIST_SIZE][DH_MAX_NAME_LEN];  // Protocol name list
	DH_485_CFG			stDecoder[DH_MAX_DECODER_NUM];						   // Each decoder current property
	
	DWORD				dw232FuncNameNum;									   // 232 function amount
	char				s232FuncName[DH_MAX_232FUNCS][DH_MAX_NAME_LEN];	       // Function name list
	DWORD               dw232ComNum;										   // 232 com amount  
	DH_RS232_CFG		st232[DH_MAX_232_NUM_EX];	                           // Current 232 COM property
} DHDEV_COMM_CFG_EX;

// Serial port status
typedef struct
{
	unsigned int		uBeOpened;
	unsigned int		uBaudRate;
	unsigned int		uDataBites;
	unsigned int		uStopBits;
	unsigned int		uParity;
	BYTE				bReserved[32];
} DH_COMM_STATE;

//-------------------------------Record configuration---------------------------------

// Scheduled record 
typedef struct 
{
	DWORD				dwSize;
	DH_TSECT			stSect[DH_N_WEEKS][DH_N_REC_TSECT];
	BYTE				byPreRecordLen;			// Pre-record button. Unit is second. 0 means disble pre-record 
	BYTE				byRedundancyEn;			// Record redundant enable button 
	BYTE                byRecordType;           // Video bit-stream type:0:Main Stream 1:Assist Stream1 2:Assist Stream2 3:Assist Stream3
	BYTE				byReserved;
} DHDEV_RECORD_CFG, *LPDH_RECORD_CFG;

// NTP setup 
typedef struct  
{
	BOOL				bEnable;				// Enable or not
	int					nHostPort;				// NTP  server default port is 123
	char				szHostIp[32];			// Host IP
	char				szDomainName[128];		// Domain name 
	int					nType;					// Read only ,0:IP,1:domain name ,2:IP and domain name 
	int					nUpdateInterval;		// Update time(minute)
	int					nTimeZone;				// Please refer to DH_TIME_ZONE_TYPE
	char				reserved[128];
} DHDEV_NTP_CFG;

// FTP upload setup 
typedef struct
{
	struct
	{
		DH_TSECT		struSect;				// Enable function is disabled during the period. Can ignore
		BOOL			bMdEn;					// Upload motion detection record
		BOOL			bAlarmEn;				// Upload external alarm record 
		BOOL			bTimerEn;				// Upload scheduled record 
		DWORD			dwRev[4];
	} struPeriod[DH_TIME_SECTION];
} DH_FTP_UPLOAD_CFG;

typedef struct
{
	DWORD				dwSize;
	BOOL				bEnable;							// Enable or not
	char				szHostIp[DH_MAX_IPADDR_LEN];		// Host IP
	WORD				wHostPort;							// Host port 
	char				szDirName[DH_FTP_MAX_PATH];			// FTP path
	char				szUserName[DH_FTP_USERNAME_LEN];	// User name
	char				szPassword[DH_FTP_PASSWORD_LEN];	// Password 
	int					iFileLen;							// File length
	int					iInterval;							// Time interval between two near files.
	DH_FTP_UPLOAD_CFG	struUploadCfg[DH_MAX_CHANNUM][DH_N_WEEKS];
	char 				protocol;							// 0-FTP 1-SMB 2-NFS,3-ISCSI
	char				NASVer;								// Network storage server version 0=Old FTP(There is time period in the user interface),1=NAS storage(There is no time period in the user interface. )
	DWORD				dwFunctionMask;						// Function capacity mask. Use bit to represent. Lower 16-bit:FTP,SMB,NFS, ISCSI and higher 16-bit:(Local storage)DISK,U
	BYTE                bDataType;                          // data type, 0-all, 1-video, 2-image
	BYTE 				reserved[123];
} DHDEV_FTP_PROTO_CFG;

// extend FTP upload setup (support setting the FTP's path, maximal picture number)
typedef struct 
{
    int            nMaxPictures;                             // the maximal number of the FTP can save,can set limit of the picture number in the every chennel's direct;
	                                                         // if the saved picture number extended the limit the old picture will be covered by new one; the value 0 indecate that it will save picture untill the disck was full.
    char           szPreChannelName[DH_FTP_MAX_SUB_PATH];    // the picture named mod
    char           szFTPChannelPath[DH_FTP_MAX_SUB_PATH];    // the FTP's sub direction
    char           szRev[128];                               // reserved
}DHDEV_FTP_CHANNEL_CFG; 


typedef struct 
{ 
	DHDEV_FTP_PROTO_CFG      stuFtpNormalSet;                // the normal function of the FTP
	DHDEV_FTP_CHANNEL_CFG     stuFtpChannelSet[DH_MAX_CHANNUM]; // the extended function of the FTP
	char                      szRev[128];                    // reserved
}DHDEV_FTP_PROTO_CFG_EX;

//-------------------------------Network Configuration---------------------------------

// Ethernet Configuration
typedef struct 
{
	char				sDevIPAddr[DH_MAX_IPADDR_LEN];	// DVR IP  address
	char				sDevIPMask[DH_MAX_IPADDR_LEN];	// DVR IP subnet mask    
	char				sGatewayIP[DH_MAX_IPADDR_LEN];	// Gateway address

	/*
	 * 1:10Mbps full-duplex
	 * 2:10Mbps auto-duplex
	 * 3:10Mbps half-duplex
	 * 4:100Mbps full-duplex
	 * 5:100Mbps auto-duplex
	 * 6:100Mbps half-duplex
	 * 7:auto
	 */
	// Divide DWORD into four to future development
	BYTE				dwNetInterface;			// NSP
	BYTE				bTranMedia;				// 0:cable,1:wireless
	BYTE				bValid;					// Use bit to represent, The first bit:1:valid 0:invalid;The second bit:0:Disable DHCP 1:Enable DHCP;The third bit:0:Do not support DHCP 1:Support DHCP
	BYTE				bDefaultEth;			// To be the default network card or not.  1: default 0:non-default 
	char				byMACAddr[DH_MACADDR_LEN];	// MAC address , read-only., 
} DH_ETHERNET; 

// Remote host setup 
typedef struct 
{
	BYTE				byEnable;				// Enable connection 
	BYTE				byAssistant;            // Only for PPPoE server,0:on the cabled network; 1:on the wireless network
	WORD				wHostPort;				// Remote host Port 
	char				sHostIPAddr[DH_MAX_IPADDR_LEN];		// Remote host IP address  				
	char				sHostUser[DH_MAX_HOST_NAMELEN];		// Remote host User name   
	char				sHostPassword[DH_MAX_HOST_PSWLEN];	// Remote host Password   
} DH_REMOTE_HOST;

// Mail setup 
typedef struct 
{
	char				sMailIPAddr[DH_MAX_IPADDR_LEN];		// Email server IP
	WORD				wMailPort;							// Email server port 
	WORD				wReserved;							// Reserved
	char				sSenderAddr[DH_MAX_MAIL_ADDR_LEN];	// Send out address 
	char				sUserName[DH_MAX_NAME_LEN];			// User name 
	char				sUserPsw[DH_MAX_NAME_LEN];			// User password 
	char				sDestAddr[DH_MAX_MAIL_ADDR_LEN];	// Destination address 
	char				sCcAddr[DH_MAX_MAIL_ADDR_LEN];		// CC address 
	char				sBccAddr[DH_MAX_MAIL_ADDR_LEN];		// BCC address 
	char				sSubject[DH_MAX_MAIL_SUBJECT_LEN];	// Subject 
} DH_MAIL_CFG;

// Network configuration structure 
typedef struct
{ 
	DWORD				dwSize; 

	char				sDevName[DH_MAX_NAME_LEN];	// Device host name

	WORD				wTcpMaxConnectNum;		// TCP max connection amount
	WORD				wTcpPort;				// TCP listening port 
	WORD				wUdpPort;				// UDP listening port 
	WORD				wHttpPort;				// HTTP port number 
	WORD				wHttpsPort;				// HTTPS port number 
	WORD				wSslPort;				// SSL port number 
	DH_ETHERNET			stEtherNet[DH_MAX_ETHERNET_NUM];	// Ethernet port 

	DH_REMOTE_HOST		struAlarmHost;			// Alarm server 
	DH_REMOTE_HOST		struLogHost;			// Log server 
	DH_REMOTE_HOST		struSmtpHost;			// SMTP server 
	DH_REMOTE_HOST		struMultiCast;			// Multiple-cast group 
	DH_REMOTE_HOST		struNfs;				// NFS server
	DH_REMOTE_HOST		struPppoe;				// PPPoE server 
	char				sPppoeIP[DH_MAX_IPADDR_LEN]; // returned IP after PPPoE registration 
	DH_REMOTE_HOST		struDdns;				// DDNS server
	char				sDdnsHostName[DH_MAX_HOST_NAMELEN];	// DDNS host name
	DH_REMOTE_HOST		struDns;				// DNS server 
	DH_MAIL_CFG			struMail;				// Email setup 
} DHDEV_NET_CFG;

// Ethernet extended Configuration
typedef struct 
{
	char				sDevIPAddr[DH_MAX_IPADDR_LEN];	// DVR IP  address
	char				sDevIPMask[DH_MAX_IPADDR_LEN];	// DVR IP subnet mask
	char				sGatewayIP[DH_MAX_IPADDR_LEN];	// Gateway address

	/*
	 * 1:10Mbps full-duplex
	 * 2:10Mbps auto-duplex
	 * 3:10Mbps half-duplex
	 * 4:100Mbps full-duplex
	 * 5:100Mbps auto-duplex
	 * 6:100Mbps half-duplex
	 * 7:auto
	 */
	// Divide DWORD into four to future development
	BYTE				dwNetInterface;			// NSP
	BYTE				bTranMedia;				// 0:cable,1:wireless
	BYTE				bValid;					// Use bit to represent, The first bit:1:valid 0:invalid;The second bit:0:Disable DHCP 1:Enable DHCP;The third bit:0:Do not support DHCP 1:Support DHCP
	BYTE				bDefaultEth;			//To be the default network card or not.  1: default 0:non-default 
	char				byMACAddr[DH_MACADDR_LEN];	// MAC address , read-only.
	BYTE                bMode;                  // mode, 1:balance, 2:multi, 3:fault-toerant
	BYTE                bReserved[31];         // reserved   
} DH_ETHERNET_EX; 

// Network extended configuration structure
typedef struct
{ 
	DWORD				dwSize; 
	
	char				sDevName[DH_MAX_NAME_LEN];	// Device host name
	
	WORD				wTcpMaxConnectNum;		// TCP max connection amount
	WORD				wTcpPort;				// TCP listening port
	WORD				wUdpPort;				// UDP listening port 
	WORD				wHttpPort;				// HTTP port number
	WORD				wHttpsPort;				// HTTPS port number
	WORD				wSslPort;				// SSL port number
	int                 nEtherNetNum;           // Ethernet port number
	DH_ETHERNET_EX		stEtherNet[DH_MAX_ETHERNET_NUM_EX];	// Ethernet info
	
	DH_REMOTE_HOST		struAlarmHost;			// Alarm server
	DH_REMOTE_HOST		struLogHost;			// Log server
	DH_REMOTE_HOST		struSmtpHost;			// SMTP server
	DH_REMOTE_HOST		struMultiCast;			// Multiple-cast group
	DH_REMOTE_HOST		struNfs;				// NFS server
	DH_REMOTE_HOST		struPppoe;				// PPPoE server
	char				sPppoeIP[DH_MAX_IPADDR_LEN]; // returned IP after PPPoE registration
	DH_REMOTE_HOST		struDdns;				// DDNS server
	char				sDdnsHostName[DH_MAX_HOST_NAMELEN];	// DDNS host name
	DH_REMOTE_HOST		struDns;				// DNS server
	DH_MAIL_CFG			struMail;				// Email setup
	BYTE                bReserved[128];         // reserved
} DHDEV_NET_CFG_EX;

// ddns configuraiton structure 
typedef struct
{
	DWORD				dwId;					// ddns server id
	BOOL				bEnable;				// Enable. There is only one valid ddns server at one time.
	char				szServerType[DH_MAX_SERVER_TYPE_LEN];	// Server type. www.3322.org.
	char				szServerIp[DH_MAX_DOMAIN_NAME_LEN];		// Server IP or domain name 
	DWORD				dwServerPort;			// Server port 
	char				szDomainName[DH_MAX_DOMAIN_NAME_LEN];	// DVR domain name such as jeckean.3322.org
	char				szUserName[DH_MAX_HOST_NAMELEN];		// User name
	char				szUserPsw[DH_MAX_HOST_PSWLEN];			// Password
	char				szAlias[DH_MAX_DDNS_ALIAS_LEN];			// Server alias 
	DWORD				dwAlivePeriod;							// DDNS alive period
	BYTE				ByMode;									// domain mode:0-manual,szDomainName enable; 1-default szDefaultDomainName enable
	char				szDefaultDomainName[DH_MAX_DEFAULT_DOMAIN_LEN];// default domain, real only
	char				reserved[195];
} DH_DDNS_SERVER_CFG;

typedef struct
{
	DWORD				dwSize;
	DWORD				dwDdnsServerNum;	
	DH_DDNS_SERVER_CFG	struDdnsServer[DH_MAX_DDNS_NUM];	
} DHDEV_MULTI_DDNS_CFG;

// Mail configuration structure 
typedef struct 
{
	char				sMailIPAddr[DH_MAX_DOMAIN_NAME_LEN];	// Mail server address(IP and domain name )
	char				sSubMailIPAddr[DH_MAX_DOMAIN_NAME_LEN];
	WORD				wMailPort;								// Mail server port 
	WORD				wSubMailPort;
	WORD				wReserved;								// Reserved 
	char				sSenderAddr[DH_MAX_MAIL_ADDR_LEN];		// From
	char				sUserName[DH_MAX_MAIL_NAME_LEN];		// User name
	char				sUserPsw[DH_MAX_MAIL_NAME_LEN];			// password
	char				sDestAddr[DH_MAX_MAIL_ADDR_LEN];		// To
	char				sCcAddr[DH_MAX_MAIL_ADDR_LEN];			// CC
	char				sBccAddr[DH_MAX_MAIL_ADDR_LEN];			// BCC
	char				sSubject[DH_MAX_MAIL_SUBJECT_LEN];		// Subject
	BYTE				bEnable;								// Enable 0:false,	1:true
	BYTE				bSSLEnable;								// SSL enable
	WORD				wSendInterval;							// Send interval,[0,3600]s
	BYTE				bAnonymous;								// Anonymous Options[0,1], 0:FALSE,1:TRUE.
	BYTE				bAttachEnable;							// Attach enable[0,1], 0:FALSE,1:TRUE.
	char				reserved[154];
} DHDEV_MAIL_CFG;

// DNS server setup 
typedef struct  
{
	char				szPrimaryIp[DH_MAX_IPADDR_LEN];
	char				szSecondaryIp[DH_MAX_IPADDR_LEN];
	char				reserved[256];
} DHDEV_DNS_CFG;

// Record download strategy setup 
typedef struct
{
	DWORD				dwSize;
	BOOL				bEnable;				// TRUE: high-speed download,FALSE:general download  
}DHDEV_DOWNLOAD_STRATEGY_CFG;

// Network transmission strategy setup 
typedef struct
{
	DWORD				dwSize;
	BOOL				bEnable;
	int					iStrategy;				// 0: video quality,1:fluency ,2: auto
}DHDEV_TRANSFER_STRATEGY_CFG;

// The corresponding parameter when setting log in
typedef struct  
{
	int					nWaittime;				// Waiting time(unit is ms), 0:default 5000ms.
	int					nConnectTime;			// Connection timeout value(Unit is ms), 0:default 1500ms.
	int					nConnectTryNum;			// Connection trial times(Unit is ms), 0:default 1.
	int					nSubConnectSpaceTime;	// Sub-connection waiting time(Unit is ms), 0:default 10ms.
	int					nGetDevInfoTime;		// Access to device information timeout, 0:default 1000ms.
	int					nConnectBufSize;		// Each connected to receive data buffer size(Bytes), 0:default 250*1024
	int					nGetConnInfoTime;		// Access to sub-connect information timeout(Unit is ms), 0:default 1000ms.
	int                 nSearchRecordTime;      // Timeout value of search video (unit ms), default 3000ms
	int                 nsubDisconnetTime;      // dislink disconnect time,0:default 60000ms
	BYTE				byNetType;				// net type, 0-LAN, 1-WAN
	BYTE                byPlaybackBufSize;      // playback data from the receive buffer size(m),when value = 0,default 4M
	BYTE                byReserved1[2];         // reserve
	int                 nPicBufSize;            // actual pictures of the receive buffer size(byte)when value = 0,default 2*1024*1024
	BYTE				bReserved[4];			// reserved
} NET_PARAM;

// Corresponding to CLIENT_SearchDevices
typedef struct 
{
	char				szIP[DH_MAX_IPADDR_LEN];				// IP
	int					nPort;													// Port
	char				szSubmask[DH_MAX_IPADDR_LEN];		// Subnet mask
	char				szGateway[DH_MAX_IPADDR_LEN];		// Gateway
	char				szMac[DH_MACADDR_LEN];					// MAC address
	char				szDeviceType[DH_DEV_TYPE_LEN];	// Device type
	BYTE        byManuFactory;				    			// manufactory
	BYTE        byIPVersion;                    // 4: IPv4, szXXX is in format with dot; 6:IPv6, szXXX is 128-bit *16 bytes) numerical format
	BYTE				bReserved[30];									// reserved
} DEVICE_NET_INFO;

// Corresponding to CLIENT_StartSearchDevices
typedef struct 
{
	int                 iIPVersion;                      // 4 for IPV4, 6 for IPV6
	char				szIP[64];		                 // IP IPV4 like"192.168.0.1" IPV6 like "2008::1/64"
	int				    nPort;		                     // port
	char				szSubmask[64];	                 // Subnet mask(IPV6 do not have subnet mask)
	char				szGateway[64];	                 // Gateway
	char				szMac[DH_MACADDR_LEN];           // MAC address
	char				szDeviceType[DH_DEV_TYPE_LEN];	 // Device type
	BYTE                byManuFactory;				     // device manufactory, see EM_IPC_TYPE
	BYTE                byDefinition;                    // 1-Standard definition 2-High definition
	bool                bDhcpEn;                         // Dhcp, true-open, false-close
	BYTE                byReserved1;                     // reserved
	char                verifyData[88];                  // ECC data 
	char                szSerialNo[DH_DEV_SERIALNO_LEN]; // serial no
	char                szDevSoftVersion[DH_MAX_URL_LEN];// soft version    
    char                szDetailType[DH_DEV_TYPE_LEN];   // device detail type
	char                szVendor[DH_MAX_STRING_LEN];     // OEM type
	char                szDevName[DH_MACHINE_NAME_NUM];  // device name
	
	char                szUserName[DH_USER_NAME_LENGTH_EX];  // user name for log in device(it need be filled when modify device ip)
	char                szPassWord[DH_USER_NAME_LENGTH_EX];  // pass word for log in device(it need be filled when modify device ip)
	unsigned short		nHttpPort;							 // HTTP server port
	WORD                wVideoInputCh;                       // count of video input channel
	WORD                wRemoteVideoInputCh;                 // count of remote video input
	WORD                wVideoOutputCh;                      // count of video output channel 
	WORD                wAlarmInputCh;                       // count of alarm input
	WORD                wAlarmOutputCh;                      // count of alarm output
	char                cReserved[244];
}DEVICE_NET_INFO_EX;

// Corresponding to CLIENT_SearchDevicesByIPs
typedef struct
{
	DWORD               dwSize;                          // struct size 
	int                 nIpNum;                          // the IPs number for search
	char                szIP[DH_MAX_SAERCH_IP_NUM][64];  // the IPs for search
}DEVICE_IP_SEARCH_INFO;

// struct SNMP config struct
typedef struct
{
	BOOL                bEnable;                                // SNMP enable
	int                 iSNMPPort;                              // SNMP port
	char                szReadCommon[DH_MAX_SNMP_COMMON_LEN];   // read common
	char                szWriteCommon[DH_MAX_SNMP_COMMON_LEN];  // write common
	char                szTrapServer[64];                       // trap address
	int                 iTrapPort;                              // trap port
	BYTE				bSNMPV1;								// SNMP V1 enable
	BYTE				bSNMPV2;								// SNMP V2 enable
	BYTE				bSNMPV3;								// SNMP V3 enable
	char                szReserve[125];
}DHDEV_NET_SNMP_CFG;
// ISCSI server
typedef struct
{
	char				szServerName[32];				// name
	union
	{ 
		BYTE	c[4];
		WORD	s[2];
		DWORD	l;
	}					stuIP;							// IP
	int					nPort;							// port
	char				szUserName[32];					// username
	char				szPassword[32];					// password
	BOOL				bAnonymous;						// anonymous
}DHDEV_ISCSI_SERVER;

// ISCSI config
typedef struct
{
	BOOL				bEnable;						// enable
	DHDEV_ISCSI_SERVER	stuServer;						// server
	char				szRemotePath[240];				// remote path
	BYTE				reserved[256];
}DHDEV_ISCSI_CFG;
//-------------------------------Alarm Property ---------------------------------

// PTZ Activation
typedef struct 
{
	int					iType;
	int					iValue;
} DH_PTZ_LINK, *LPDH_PTZ_LINK;

//Alarm activation structure 
typedef struct 
{
	/* Message process way. There can be several process ways.
	 * 0x00000001 -  Alarm upload
	 * 0x00000002 -  Activation alarm 
	 * 0x00000004 -  PTZ activation
	 * 0x00000008 -  Send out mail
	 * 0x00000010 -  Local tour
	 * 0x00000020 -  Local prompt
	 * 0x00000040 -  Alarm output
	 * 0x00000080 - Ftp upload
	 * 0x00000100 -  Buzzer 
	 * 0x00000200 -  Video prompt
	 * 0x00000400 -  Snapshot
	*/

	/* The process way current alarm supported. Use bit mask to represent */
	DWORD				dwActionMask;

	/* Use bit mask to represent. The parameters of detailed operation are in its correspond configuration */
	DWORD				dwActionFlag;
	
	/* The output channel the alarm activated. The output alarm activated. 1 means activate current output. */ 
	BYTE				byRelAlarmOut[DH_MAX_ALARMOUT_NUM];
	DWORD				dwDuration;				/*  Alarm duration time*/

	/* Activation record */
	BYTE				byRecordChannel[DH_MAX_VIDEO_IN_NUM]; /*  The record channel alarm activated. 1 means activate current channel. */
	DWORD				dwRecLatch;				/*  Record duration time */

	/* Snapshot channel  */
	BYTE				bySnap[DH_MAX_VIDEO_IN_NUM];
	/* Tour channel */
	BYTE				byTour[DH_MAX_VIDEO_IN_NUM];

	/* PTZ activation  */
	DH_PTZ_LINK			struPtzLink[DH_MAX_VIDEO_IN_NUM];
	DWORD				dwEventLatch;			/* The latch time after activation begins. Unit is second. The value ranges from 0 to 15. Default value is 0. */
	/* The wireless output channel alarm activated.The output alarm activated. 1 means activate current output.*/ 
	BYTE				byRelWIAlarmOut[DH_MAX_ALARMOUT_NUM];
	BYTE				bMessageToNet;
	BYTE                bMMSEn;					/* SMS Alarm enabled */
	BYTE                bySnapshotTimes;		/* the number of sheets of drawings */
	BYTE				bMatrixEn;				/*enable matrix */
	DWORD				dwMatrix;				/*matrix mask */			
	BYTE				bLog;					/*enable log */
	BYTE				bSnapshotPeriod;		/*<Snapshot frame interval. System can snapshot regularly at the interval you specify here. The snapshot amount in a period of time also has relationship with the snapshot frame rate. 0 means there is no interval, system just snapshot continuously.*/
	BYTE                byEmailType;            /*<0,picture,1,record>*/
	BYTE                byEmailMaxLength;       /*<max record length,unit:MB>*/
	BYTE                byEmailMaxTime;         /*<max time length, unit:second>*/
	BYTE				byReserved[99];     
} DH_MSG_HANDLE;

// External alarm 
typedef struct
{
	BYTE				byAlarmType;			// Annunciator type,0: normal close,1:normal open 
	BYTE				byAlarmEn;				// Enable alarm 
	BYTE				byReserved[2];						
	DH_TSECT			stSect[DH_N_WEEKS][DH_N_REC_TSECT]; // NSP
	DH_MSG_HANDLE		struHandle;				// Process way 
} DH_ALARMIN_CFG, *LPDHDEV_ALARMIN_CFG; 

//Motion detection alarm 
typedef struct 
{
	BYTE				byMotionEn;				// Enable motion detection alarm 
	BYTE				byReserved;
	WORD				wSenseLevel;			// Sensitivity 
	WORD				wMotionRow;				// Row amount in motion detection zones
	WORD				wMotionCol;				// Column amount in motion detection zones 
	BYTE				byDetected[DH_MOTION_ROW][DH_MOTION_COL]; // Detection zones .Max 32*32 zones.
	DH_TSECT			stSect[DH_N_WEEKS][DH_N_REC_TSECT];	// NSP
	DH_MSG_HANDLE		struHandle;				// Process way 
} DH_MOTION_DETECT_CFG;

// Video loss alarm 
typedef struct
{
	BYTE				byAlarmEn;				// Enable video loss alarm 
	BYTE				byReserved[3];
	DH_TSECT			stSect[DH_N_WEEKS][DH_N_REC_TSECT];	// NSP
	DH_MSG_HANDLE		struHandle;				// Process way
} DH_VIDEO_LOST_CFG;

// Camera masking alarm 
typedef struct
{
	BYTE				byBlindEnable;						// Enable
	BYTE				byBlindLevel;						// Sensitivity 1-6
	BYTE				byReserved[2];
	DH_TSECT			stSect[DH_N_WEEKS][DH_N_REC_TSECT];	// NSP
	DH_MSG_HANDLE		struHandle;							// Process way 
} DH_BLIND_CFG;

// HDD information(External alarm )
typedef struct 
{
	BYTE				byNoDiskEn;								// Alarm when there is no HDD
	BYTE				byReserved_1[3];
	DH_TSECT			stNDSect[DH_N_WEEKS][DH_N_REC_TSECT];	// NSP
	DH_MSG_HANDLE		struNDHandle;							// Process way 

	BYTE				byLowCapEn;								// Alarm when HDD capacity is low 
	BYTE				byLowerLimit;							// Capacity threshold 0-99
	BYTE				byReserved_2[2];
	DH_TSECT			stLCSect[DH_N_WEEKS][DH_N_REC_TSECT];	// NSP
	DH_MSG_HANDLE		struLCHandle;							// Process way 

	BYTE				byDiskErrEn;							// HDD malfunction alarm 
	BYTE				bDiskNum;
	BYTE				byReserved_3[2];
	DH_TSECT			stEDSect[DH_N_WEEKS][DH_N_REC_TSECT];	// NSP
	DH_MSG_HANDLE		struEDHandle;							// Process way 
} DH_DISK_ALARM_CFG;

typedef struct
{
	BYTE				byEnable;
	BYTE				byReserved[3];
	DH_MSG_HANDLE		struHandle;
} DH_NETBROKEN_ALARM_CFG;

// Alarm deployment 
typedef struct
{
	DWORD dwSize;
	DH_ALARMIN_CFG 		struLocalAlmIn[DH_MAX_ALARM_IN_NUM];
	DH_ALARMIN_CFG		struNetAlmIn[DH_MAX_ALARM_IN_NUM];
	DH_MOTION_DETECT_CFG struMotion[DH_MAX_VIDEO_IN_NUM];
	DH_VIDEO_LOST_CFG	struVideoLost[DH_MAX_VIDEO_IN_NUM];
	DH_BLIND_CFG		struBlind[DH_MAX_VIDEO_IN_NUM];
	DH_DISK_ALARM_CFG	struDiskAlarm;
	DH_NETBROKEN_ALARM_CFG	struNetBrokenAlarm;
} DHDEV_ALARM_SCHEDULE;

#define DECODER_OUT_SLOTS_MAX_NUM 		16
#define DECODER_IN_SLOTS_MAX_NUM 		16

// Alarm decoder configuration
typedef struct  
{
	DWORD				dwAddr;									// Alarm decoder address
	BOOL				bEnable;								// Alarm decoder enable
	DWORD				dwOutSlots[DECODER_OUT_SLOTS_MAX_NUM];	// Now only support 8
	int					nOutSlotNum;							// Effective number of array elements.
	DH_TSECT			stSect[DH_N_WEEKS][DH_N_REC_TSECT];
	DH_MSG_HANDLE		struHandle[DECODER_IN_SLOTS_MAX_NUM];	// Now only support 8
	int					nMsgHandleNum;							// Effective number of array elements.
	BYTE				bReserved[120];
} DH_ALARMDEC_CFG;

// The setup of alarm upload
typedef struct  
{
	BYTE				byEnable;						// Enable upload
	BYTE				bReserverd;						// Reserved
	WORD				wHostPort;						// Alarm center listening port 
	char				sHostIPAddr[DH_MAX_IPADDR_LEN];	// Alarm center IP

	int					nByTimeEn;						// Enable scheduled upload.Use it to upload IP or domain name to the centre.
	int					nUploadDay;						/*  Set upload date 
														"Never = 0", "Everyday = 1", "Sunday = 2", 
														"Monday = 3", Tuesday = 4", "Wednesday = 5",
														"Thursday = 6", "Friday = 7", "Saturday = 8"*/	
	int					nUploadHour;					// Set upload time ,[0~23]o'clock
	
	DWORD				dwReserved[300]; 				// Reserved for future development 
} ALARMCENTER_UP_CFG;

// Panorama switch alarm configuration
typedef struct __DH_PANORAMA_SWITCH_CFG 
{
	BOOL				bEnable;				// Enabled
	int					nReserved[5];			// Reserved
	DH_TSECT			stSect[DH_N_WEEKS][DH_N_REC_TSECT];
	DH_MSG_HANDLE		struHandle;				// Process way
} DH_PANORAMA_SWITCH_CFG;

typedef struct __ALARM_PANORAMA_SWITCH_CFG 
{
	int					nAlarmChnNum;			// Number of alarm channels
	DH_PANORAMA_SWITCH_CFG stuPanoramaSwitch[DH_MAX_VIDEO_IN_NUM];
} ALARM_PANORAMA_SWITCH_CFG;

// Lose focus alarm configuration
typedef struct __DH_LOST_FOCUS_CFG
{
	BOOL				bEnable;				// Enabled
	int					nReserved[5];			// Reserved
	DH_TSECT			stSect[DH_N_WEEKS][DH_N_REC_TSECT];
	DH_MSG_HANDLE		struHandle;				// Process way
} DH_LOST_FOCUS_CFG;

typedef struct __ALARM_LOST_FOCUS_CFG 
{
	int					nAlarmChnNum;			// Number of alarm channels
	DH_LOST_FOCUS_CFG stuLostFocus[DH_MAX_VIDEO_IN_NUM];
} ALARM_LOST_FOCUS_CFG;

// IP collision detection alarm configuration
typedef struct __ALARM_IP_COLLISION_CFG
{
	BOOL				bEnable;				// Enable
	DH_MSG_HANDLE		struHandle;				// Alarm activation
	int                 nReserved[300];			// Reserved
} ALARM_IP_COLLISION_CFG;

// MACcollision detection configuration
typedef struct __ALARM_MAC_COLLISION_CFG
{
	BOOL				bEnable;				// Enable
	DH_MSG_HANDLE		struHandle;				// Alarm activation
	int                 nReserved[300];			// Reserved
} ALARM_MAC_COLLISION_CFG;

// 232/485 com card signal info configuration
typedef struct __COM_CARD_SIGNAL_INFO
{
	WORD                wCardStartPose;          // card number start location
	WORD                wCardLenth;              // card number length
	char                cStartCharacter[32];     // start string
	char                cEndCharacter[32];       // end string
	BYTE                byReserved[28];          // reserved
}COM_CARD_SIGNAL_INFO;

// 232/485 com card  linked configuration(when the info from the com fit the configuration, it will trigger the device snap picture)
typedef struct __COM_CARD_SIGNAL_LINK_CFG
{
	COM_CARD_SIGNAL_INFO  struCardInfo;          // card info
	DH_MSG_HANDLE         struHandle;            // event link
	BYTE                  byReserved[24];        // reserved
	
}COM_CARD_SIGNAL_LINK_CFG;


//------------------------------Multiple privacy mask zones--------------------------------

// Privacy mask information
typedef struct __VIDEO_COVER_ATTR
{
	DH_RECT				rcBlock;				// Privacy mask zone coordinates
	int					nColor;					// Privacy mask color
	BYTE				bBlockType;				// Mask type;0:Black block,1: mosaic
	BYTE				bEncode;				// Encode lelel mask;1:valid,0:invalid
	BYTE				bPriview;				// Preview mask; 1:valid,0:invalid
	char				reserved[29];			// Reserved 
} VIDEO_COVER_ATTR;

// Multiple privacy mask zones 
typedef struct __DHDEV_VIDEOCOVER_CFG 
{
	DWORD				dwSize;
	char				szChannelName[DH_CHAN_NAME_LEN]; // Read-only
	BYTE				bTotalBlocks;			// The mask zones supported
	BYTE				bCoverCount;			// The mask zones have been set 
	VIDEO_COVER_ATTR	CoverBlock[DH_MAX_VIDEO_COVER_NUM]; // The mask zones 
	char				reserved[30];			// Reserved 
}DHDEV_VIDEOCOVER_CFG;

// decode policy configuration
typedef struct __DHDEV_DECODEPOLICY_CFG 
{
	int					nMinTime;				// (read only):minimal delay time(ms)
	int					nMaxTime;				// (read only):maximal delay time(ms)
	int					nDeocdeBufTime;			// device decode delay time(ms)
	
	char				reserved[128];			// reserved
}DHDEV_DECODEPOLICY_CFG;

// Device relative configuration
typedef struct __DHDEV_MACHINE_CFG 
{
	char				szMachineName[DH_MACHINE_NAME_NUM];		// Device name or SN 
	char				szMachineAddress[DH_MACHINE_NAME_NUM];	// Device location
	char				reserved[128];							// reserved
}DHDEV_MACHINE_CFG;


////////////////////////////////IPC series ////////////////////////////////

// Set wireless network information 
typedef struct 
{
	int					nEnable;				// Enable wireless
	char				szSSID[36];				// SSID
	int					nLinkMode;				// connection mode;0:auto,1:adhoc,2:Infrastructure
	int					nEncryption;			// encrypt;0:off,2:WEP64bit,3:WEP128bit, 4:WPA-PSK-TKIP, 5: WPA-PSK-CCMP
	int					nKeyType;				// 0:Hex,1:ASCII
    int					nKeyID;					// Serial number
	union
	{
		char			szKeys[4][32];			// Four group passwords
		char			szWPAKeys[128];			// when nEncryption is 4 or 5, use szWPAKeys
	};
	int					nKeyFlag;
	BYTE                byConnectedFlag;        // 0: not connect, 1: connect 
	char				reserved[11];
} DHDEV_WLAN_INFO;

// Select to use one wireless device 
typedef struct  
{
	char				szSSID[36];
	int					nLinkMode;				// Connection mode;0:adhoc,1:Infrastructure
	int 				nEncryption;			// Encrypt;0:off,2:WEP64bit,3:WEP128bit
	char				reserved[48];
} DHDEV_WLAN_DEVICE;

// The searched wireless device list 
typedef struct  
{
	DWORD				dwSize;
	BYTE				bWlanDevCount;			// The wireless devices searched 
	DHDEV_WLAN_DEVICE	lstWlanDev[DH_MAX_WLANDEVICE_NUM];
	char				reserved[255];
} DHDEV_WLAN_DEVICE_LIST;

// wireless device expended configuration
typedef struct
{
	char                szSSID[36];                         // server id
	char                szMacAddr[18];                      // mac address 
	BYTE                byApConnected;                      // connect state 0: not connect,1: connected
	BYTE                byLinkMode;                         // connect mod 0:adhoc 1:Infrastructure;
	int                 nRSSIQuality;                       // rssi quality(dbm)
	unsigned int        unApMaxBitRate;                     // max transmit speed
	BYTE                byAuthMode;                         // attestation mod:0:OPEN;1:SHARED;2:WPA;3:WPA-PSK;4:WPA2;5:WPA2-PSK;
															// 6:WPA-NONE(only use in Adhoc mode),
															// 7-11 are mix mode,choose one of them can to be connected 
															// 7:WPA-PSK | WPA2-PSK; 8:WPA | WPA2; 9:WPA | WPA-PSK;
															// 10:WPA2 | WPA2-PSK; 11:WPA | WPA-PSK |WPA2 |WPA2-PSK //12: UnKnown
	BYTE                byEncrAlgr;                         // encrypt mod 0:off; 2:WEP64bit; 3:WEP128bit; 4:WEP; 5:TKIP; 6:AES(CCMP)
															// 7: TKIP+AES( mix Mode) 8: UnKnown
	BYTE                byLinkQuality;                      // link quality 0~100(%)
    BYTE                byReserved[129];                     // Reserved 
}DHDEV_WLAN_DEVICE_EX;

// The searched wireless device expended configuration list 
typedef struct  
{
	DWORD				dwSize;
	BYTE				bWlanDevCount;			// The wireless device number searched
	DHDEV_WLAN_DEVICE_EX  lstWlanDev[DH_MAX_WLANDEVICE_NUM_EX];
	char				reserved[255];
} DHDEV_WLAN_DEVICE_LIST_EX;

// Function Test
typedef struct
{
	int                 nResult;                  // 0:success,1:failure
	BYTE                reserved[32];
} DHDEV_FUNC_TEST;

// FTP server info
typedef struct
{
	char                szServerName[32];          // server name
	char                szIp[16];                  // IP address
	int                 nPort;                     // port number
	char                szUserName[32];            // user name
	char                szPassword[32];	           // pass word
	BOOL                bAnonymity;                // whether anonymity log in 
	BYTE                byReserved[256];           // reserved
}FTP_SERVER_CFG;

// ftp server connect test
typedef struct
{
	FTP_SERVER_CFG     stuFtpServerInfo;           // ftp server info(filled by user)
	DHDEV_FUNC_TEST    stuTestResult;              // ftp server connect state
	BYTE               byReserved[64];
}DHDEV_FTP_SERVER_TEST;

// DDNS domain info
typedef struct
{

	char				szServerType[DH_MAX_SERVER_TYPE_LEN];	//server type
	char				szServerIp[DH_MAX_DOMAIN_NAME_LEN];		// server ip or domain
	DWORD				dwServerPort;			// server port
	char				szDomainName[DH_MAX_DOMAIN_NAME_LEN];	// dvr domain,such as jeckean.3322.org
	char				szUserName[DH_MAX_HOST_NAMELEN];		// username
	char				szUserPsw[DH_MAX_HOST_PSWLEN];			// password
	BYTE                byReserved[256];           // reserved
}DDNS_DOMAIN_INFO;


// DDNS domain test
typedef struct
{
	DDNS_DOMAIN_INFO   stuDomainInfo;				// DDNS domain
	DHDEV_FUNC_TEST    stuTestResult;				// test result
	char			   szMemo[128];					// test result memo
	BYTE               byReserved[64];
}DHDEV_DDNS_DOMAIN_TEST;

// hard disk's basic information
typedef struct 
{
	BYTE                byModle[32];				// model
	BYTE                bySerialNumber[32];			// serial number
	BYTE                byFirmWare[32];				// firmware no
	int                 nAtaVersion;				// ATA protocol version no
	int                 nSmartNum ;					// smart information no
	INT64				Sectors;	
	int                 nStatus;					// disk state 0-normal 1-abnormal
	int                 nReserved[33]; 
} DHDEV_DEVICE_INFO;

//smart information of harddisk,there may be many items up to more than 30????
typedef struct
{
	BYTE				byId;						// ID
	BYTE				byCurrent;					// attribute values 
	BYTE				byWorst;					// maximum error value 
	BYTE				byThreshold;				// threshold value 
	char				szName[64];					// property name
	char				szRaw[8];					// actual value
	int					nPredict;					// state
	char				reserved[128];
} DHDEV_SMART_VALUE;

//search hard disk smart information
typedef struct
{
	BYTE               nDiskNum;       // disk number
	BYTE               byRaidNO;       // Raid sub disk, 0:single disk
	BYTE               byReserved[2];  
	DHDEV_DEVICE_INFO  deviceInfo;    
	DHDEV_SMART_VALUE  smartValue[MAX_SMART_VALUE_NUM];
} DHDEV_SMART_HARDDISK;

// submodule information
typedef struct
{
	char               szModuleName[64];			//  submodule name
	char               szHardWareVersion[32];		//  submodule HardWareVersion
	char               szSoftwareVersion[32];		//  submodule SoftWareVersion
	BYTE               reserved[128]; 
} DHDEV_SUBMODELE_VALUE;

// search submodule information
typedef struct
{
	int                    nSubModuleNum;							//  nSubModuleNum
	DHDEV_SUBMODELE_VALUE  stuSubmoduleValue[MAX_SUBMODULE_NUM];	// Submodule information
	BYTE				   bReserved[256];
} DHDEV_SUBMODULE_INFO;

// Query harddisk damage ability
typedef struct
{
	BYTE                bDiskDamageLevel[DH_MAX_DISK_NUM];  // every disk's damage level
	BYTE                bReserved[128];
} DHDEV_DISKDAMAGE_INFO;

// SYSLOG remote server ability
typedef struct 
{
	char				szServerIp[DH_MAX_IPADDR_OR_DOMAIN_LEN];	//Server IP address
	int					nServerPort;								//Server port
	BYTE				bEnable;									//Server Enable
	BYTE				bReserved[255];								//Reserved bytes
}DHDEV_SYSLOG_REMOTE_SERVER;

// Video backup config parameter
typedef struct
{
	BYTE				backupVideoFormat;							//file type to backup, 0:dav, 1:asf
	BYTE				password[6];								//password
	BYTE				reversed[505];								//reserved
}DHDEV_BACKUP_VIDEO_FORMAT;


// Auto register config parameter
typedef struct  
{
	char				szServerIp[32];			// Registration server IP ; no use it,use szServerIpEx
	int					nServerPort;			// Port number 
	BYTE                byReserved[3];          // 
	BYTE                bServerIpExEn;          // Extend Registration server IP enable, 0-not enable, 1-enable
	char				szServerIpEx[60];       // Extend Registration server IP
} DHDEV_SERVER_INFO;

typedef struct  
{
	DWORD				dwSize;
	BYTE				bServerNum;				// The max IP amount supported
	DHDEV_SERVER_INFO	lstServer[DH_MAX_REGISTER_SERVER_NUM];
	BYTE				bEnable;				// Enable
	char				szDeviceID[32];			// Device ID
	char				reserved[94];
} DHDEV_REGISTER_SERVER;

// Camera property 
typedef struct __DHDEV_CAMERA_INFO
{
	BYTE				bBrightnessEn;			// Brightness adjustable;1:adjustable,0:can not be adjusted
	BYTE				bContrastEn;			// Contrast adjustable
	BYTE				bColorEn;				// Hue adjustable
	BYTE				bGainEn;				// Gain adjustable
	BYTE				bSaturationEn;			// Saturation adjustable
	BYTE				bBacklightEn;			// Backlight compensation adjustable
	BYTE				bExposureEn;			// Exposure option adjustable
	BYTE				bColorConvEn;			// Day/night switch 
	BYTE				bAttrEn;				// Property option; 1:Enable, 0:Disable
	BYTE				bMirrorEn;				// Mirror;1:support,0:do not support 
    BYTE				bFlipEn;				// Flip;1:support,0:do not support 
	BYTE				iWhiteBalance;			// White Balance 1 Support,0 :Do not support
	BYTE				iSignalFormatMask;		// Signal format mask,Bitwise:0-Inside(Internal input) 1- BT656 2-720p 3-1080i  4-1080p  5-1080sF
	BYTE				bRotate90;				// 90-degree rotation 1:support,0:do not support 
    BYTE				bLimitedAutoExposure;   // Support the time limit with automatic exposure 1:support,0:do not support 
    BYTE				bCustomManualExposure;  // support user-defined manual exposure time 1:support,0:do not support
	BYTE				bFlashAdjustEn;			// Support the flash lamp adjust
	BYTE				bNightOptions;			// Support day and night change
	BYTE                iReferenceLevel;    	// Support electric reference setting
	BYTE                bExternalSyncInput;     // Support external sync Input
	unsigned short      usMaxExposureTime;      // Max exposure time, unit:ms         
	unsigned short      usMinExposureTime;      // Min exposure time, unit:ms
	BYTE                bWideDynamicRange;      // Wide dynamic range,0-present not support,2~n max supported range value
	BYTE                bDoubleShutter;         // Double Shutter
	BYTE				byExposureCompensation; // 1 support, 0 not support
	BYTE				bRev[109];				// reserved 
} DHDEV_CAMERA_INFO;

// Special configuration for night,will automatically switch to night configuration when low light
typedef struct __DHDEV_NIGHTOPTIONS 
{
	BYTE  bEnable;								// 0-Do not switch,1-Switch 
												// Roughly sunrise and sunset time, before sunrise or after sunset, will use a special configuration for night.
												// 00:00:00 ~23:59:59
	BYTE  bSunriseHour;
	BYTE  bSunriseMinute;
	BYTE  bSunriseSecond;
	BYTE  bSunsetHour;
	BYTE  bSunsetMinute;
	BYTE  bSunsetSecond;  
	BYTE  bWhiteBalance ;						// White balance  0:Disabled,1:Auto 2:sunny 3:cloudy 4:home 5:office 6:night 7: Custom
	BYTE  bGainRed;								// Red gain adjustment, white balance to "Custom" mode will effect 0~100
	BYTE  bGainBlue;							// Green gain adjustment, white balance to "Custom" mode will effect 0~100
	BYTE  bGainGreen;							// Blue gain adjustment, white balance to "Custom" mode will effect 0~100
	BYTE  bGain;								// 0~100
	BYTE  bGainAuto;							// 0-Without auto gain 1-Auto gain
	BYTE  bBrightnessThreshold ;				// Brightness value 0~100
	BYTE  ReferenceLevel;                       // electric value 0~100   
	BYTE  bExposureSpeed;						// Range depends on the device capability: 0-Auto Exposure  1~n-1-manual exposure level;  n-Auto Exposure with time limit;  n+1-manual exposure customized time (n means exposure level)
	float ExposureValue1;						// Lower limit of automatic exposure time or customized manual exposure time, in milliseconds, value 0.1ms ~ 80ms
	float ExposureValue2;						// Automatic exposure time limit, in milliseconds, value 0.1ms ~ 80ms
	BYTE  bAutoApertureEnable;                  // Auto Aperture Enable,1:open,0:close
	BYTE  bWideDynamicRange;                    // wide dynamic value, it depends on max support value
	WORD  wNightSyncValue;						// night sync 0~360
	WORD  wNightSyncValueMillValue;             // night sync mill value 0~999
	BYTE res[10];								// Reserve
} DHDEV_NIGHTOPTIONS;

// camera attribute configration
typedef struct __DHDEV_CAMERA_CFG 
{ 
	DWORD				dwSize;
	BYTE				bExposure;				// Exposure mode;1-9:Manual exposure level  ,0: Auto exposure 
	BYTE				bBacklight;				// Backlight compensation:3:High,2:Medium,1:Low,0:close
	BYTE				bAutoColor2BW;			// Day/night mode;2:Open,1:Auto,0:Close
	BYTE				bMirror;				// Mirror;1:Open,0:Close
	BYTE				bFlip;					// Flip;1:Open,0:Close  support;0 :do not support 
	BYTE				bLensEn;				// the capacity of Auto Iris function 1:support;0 :do not support
	BYTE				bLensFunction;			// Auto aperture function: 1:Enable aperture  ;0: Disable aperture  
	BYTE				bWhiteBalance;			// White Balance 0:Disabled,1:Auto 2:sunny 3:cloudy 4:home 5:office 6:night
	BYTE				bSignalFormat;			// Signal format 0-Inside(Internal input) 1- BT656 2-720p 3-1080i  4-1080p  5-1080sF
	BYTE				bRotate90;				// 0-Not rotating,1-90 degrees clockwise,2-90 degrees counterclockwise
	BYTE                bReferenceLevel;        // electric value 0~100  
	BYTE                byReserve;              // Reserved
	float				ExposureValue1;			// Auto exposure or manual exposure limit custom time,Milliseconds(0.1ms~80ms)
	float				ExposureValue2;			// Auto exposure limit,Milliseconds(0.1ms~80ms)
	DHDEV_NIGHTOPTIONS	stuNightOptions;		// Configuration parameter options for night 
	BYTE				bGainRed;				// Red gain adjustment, effective when white balance to "Custom" mode,  0 ~ 100
	BYTE				bGainBlue;				// Green gain adjustment, effective when white balance to "Custom" mode,  0 ~ 100
	BYTE				bGainGreen;				// Blue gain adjustment, effective when white balance to "Custom" mode,  0 ~ 100
	BYTE				bFlashMode;				// Flash mode,0-close,1-always,2-auto 
	BYTE				bFlashValue;			// Flash work values,  0-0us, 1-64us, 2-128us,...15-960us
	BYTE				bFlashPole;				// Flash trigger mode 0 - low level 1 - high level
	BYTE                bExternalSyncPhase;     // External single input
	BYTE                bFlashInitValue;        // Flash brightness prevlue, 0~100
	WORD                wExternalSyncValue ;    // External value 0~360
	WORD                wExternalSyncValueMillValue; //  External SyncValue Mill Value0~999
	BYTE                bWideDynamicRange;           // wide dynamic range, it depends on max support value
	BYTE				byExposureCompensation;		// default compensation value,default is 7,range[0~14]
	char				bRev[54];				// Reserved
} DHDEV_CAMERA_CFG;

#define ALARM_MAX_NAME 64
// (wireless)IR alarm setup
typedef struct
{
	BOOL				bEnable;				// Enable alarm input
	char				szAlarmName[DH_MAX_ALARM_NAME];	// Alarm input name
	int					nAlarmInPattern;		// Annunciator input wave
	int					nAlarmOutPattern;		// Annunciator output wave
	char				szAlarmInAddress[DH_MAX_ALARM_NAME];// Alarm input address
	int					nSensorType;			// External device sensor type normal open or normal close 
	int					nDefendEfectTime;		// Deploy and cancel latch time. The alarm input becomes activated after the specified time.
	int					nDefendAreaType;		// Defend area 
	int					nAlarmSmoothTime;		// Alarm smooth time:system ignores the second alarm if one alarm inputs in two times. 
	char				reserved[128];
	DH_TSECT			stSect[DH_N_WEEKS][DH_N_REC_TSECT];
	DH_MSG_HANDLE		struHandle;				// Process way 
} DH_INFRARED_INFO;

// Wireless remote control setup 
typedef struct 
{
	BYTE				address[ALARM_MAX_NAME];// Remote control address
	BYTE				name[ALARM_MAX_NAME];	// Remote control name
	BYTE				reserved[32];			// Reserved string 
} DH_WI_CONFIG_ROBOT;

// Wireless alarm output setup 
typedef struct 
{
	BYTE				address[ALARM_MAX_NAME];// Alarm output address
	BYTE				name[ALARM_MAX_NAME];	// Aalarm output name
	BYTE				reserved[32];			// Reserved string 
} DH_WI_CONFIG_ALARM_OUT;

typedef struct  
{
	DWORD				dwSize;
	BYTE				bAlarmInNum;			// Wireless alarm input amount 
	BYTE				bAlarmOutNum;			// Wireless alarm output amount 
	DH_WI_CONFIG_ALARM_OUT AlarmOutAddr[16];	// Alarm output address
	BYTE				bRobotNum;				// Remote control amount
	DH_WI_CONFIG_ROBOT RobotAddr[16];			// Remote control address 
	DH_INFRARED_INFO	InfraredAlarm[16];
	char				reserved[256];
} DH_INFRARED_CFG;

// New audio detection alarm information 
typedef struct
{
	int					channel;				// Alarm channel number
	int					alarmType;				// Alarm type;0:Volumn value is too low ,1:Volumn value is too high. 
	unsigned int		volume;					// Volume
	BYTE                byState;                // volume alarm state, 0: alarm appear, 1: alarm disappear
	char				reserved[255];
} NET_NEW_SOUND_ALARM_STATE;

typedef struct  
{
	int					channelcount;			// Alarm channel amount 
	NET_NEW_SOUND_ALARM_STATE SoundAlarmInfo[DH_MAX_ALARM_IN_NUM];
} DH_NEW_SOUND_ALARM_STATE;

// Snapshot function property structure 
typedef struct 
{
	int					nChannelNum;			// Channel amount 
	DWORD				dwVideoStandardMask;	// Resolution(Bit). Please refer to enumeration CAPTURE_SIZE						
	int					nFramesCount;			// Valid length of Frequency[128] array
	char				Frames[128];			// Frame rate(value) 
												// -25:1f/25s;-24:1f/24s;-23:1f/23s;-22:1f/23s
												// ?-?-
												// 0: invalid;1:1f/s;2:2f/s;3:13f/s
												// 4:4f/s;5:5f/s;17:17f/s;18:18f/s
												// 19:19f/s;20:20f/s
												// ?-?-
												// 25: 25f/s
	int					nSnapModeCount;			// valid length of SnapMode[16] array
	char				SnapMode[16];			// (value)0:activate scheduled snapshot,1:Manually activate snapshot
	int					nPicFormatCount;		// Valid length of Format[16] array 
	char 				PictureFormat[16];		// (Value)0:BMP format,1:JPG format
	int					nPicQualityCount;		// valid length of Quality[32] array
	char 				PictureQuality[32];		// value
												// 100:Image quality 100%;80:Image quality 80%;60:Image quality60%
												// 50:Image quality50%;30:Image quality30%;10:Image quality10%
	char 				nReserved[128];			// Reserved
} DH_QUERY_SNAP_INFO;

typedef struct 
{
	int					nChannelCount;			// Channel amount 
	DH_QUERY_SNAP_INFO  stuSnap[DH_MAX_CHANNUM];
} DH_SNAP_ATTR_EN;


/* IP Filtering configuration */
#define DH_IPIFILTER_NUM			200			// IP
#define DH_IPIFILTER_NUM_EX			512			// IP

// IP Information
typedef struct 
{
	DWORD				dwIPNum;				// IP count
	char				SZIP[DH_IPIFILTER_NUM][DH_MAX_IPADDR_LEN]; // IP
	char				byReserve[32];			// Reserved
} IPIFILTER_INFO;

// IP Filtering configuration
typedef struct
{
	DWORD				dwSize;
	DWORD				dwEnable;				// Enable
	DWORD				dwType;					// The current list type;0:White List 1:Blacklist(The device can enable only one type of list)
	IPIFILTER_INFO		BannedIP;				// Black list
	IPIFILTER_INFO		TrustIP;				// Trusted sites
	char				byReserve[256];			// Reserved
} DHDEV_IPIFILTER_CFG;

// IP Information extended
typedef struct 
{
	DWORD				dwIPNum;				// IP count
	char				SZIP[DH_IPIFILTER_NUM_EX][DH_MAX_IPADDR_LEN]; // IP
	char				byReserve[32];			// Reserved
} IPIFILTER_INFO_EX;

// IP Filtering extended configuration
typedef struct
{
	DWORD				dwSize;
	DWORD				dwEnable;				// Enable
	DWORD				dwType;					// The current list type;0:White List 1:Blacklist(The device can enable only one type of list) can only user one kind of device
	IPIFILTER_INFO_EX		BannedIP;			// Black list
	IPIFILTER_INFO_EX		TrustIP;			// Trusted sites
	char				byReserve[256];			// Reserved
} DHDEV_IPIFILTER_CFG_EX;

/* MAC filter configuration */
#define DH_MACFILTER_NUM			512			// MAC

// MAC info
typedef struct 
{
	DWORD				dwSize;					// struct size
	DWORD				dwMacNum;				// MAC count
	char				szMac[DH_MACFILTER_NUM][DH_MACADDR_LEN]; // MAC
} MACFILTER_INFO;
// MAC filter configuration
typedef struct
{
	DWORD					dwSize;				// struct size
	DWORD					dwEnable;			// enable
	DWORD					dwType;				// current list type,0:white list, 1:blacklist (The device can enable only one type of list) can only user one kind of device
	MACFILTER_INFO			stuBannedMac;		// black list Mac
	MACFILTER_INFO			stuTrustMac;		// white list Mac
} DHDEV_MACFILTER_CFG;

/* MAC,IP filter configuration */
#define DH_MACIPFILTER_NUM			512			// MAC,IP
// MAC, IP filter configuration info
typedef struct
{
	DWORD	dwSize;					// struct size
	char	szMac[DH_MACADDR_LEN];	// mac
	char	szIp[DH_MAX_IPADDR_LEN];// ip
}MACIP_INFO;

// MAC,IP filter configuration
typedef struct
{
	DWORD					dwSize;								// struct size
	DWORD					dwEnable;							// enable
	DWORD					dwType;								// The current list type;0:White List 1:Blacklist(The device can enable only one type of list) can only user one kind of device
	DWORD					dwBannedMacIpNum;					// black list MAC,IP count (MAC,IP one to one)
	MACIP_INFO				stuBannedMacIp[DH_MACIPFILTER_NUM];	// black list Mac,IP
	DWORD					dwTrustMacIpNum;			// white list MAC,IP count(MAC,IP one to one)
	MACIP_INFO				stuTrustMacIp[DH_MACIPFILTER_NUM];	// white list Mac,IP
} DHDEV_MACIPFILTER_CFG;
typedef struct
{
	int                nCardNum;                // card number
	char 	           cardInfo [DH_MAX_CARD_NUM][DH_MAX_CARDINFO_LEN]; // card info string 
	BYTE               byReserve[32];           // reserved
}DHDEV_NETCARD_CFG;

// RTSP configuration
typedef struct
{
	WORD               wPort;                  // port number(can't be the same as tcp or udp's port number)
	BYTE               byReserved[62];         // reserved
}DHDEV_RTSP_CFG;

// stream encrypt configuration
// encrypt key info
typedef struct _ENCRYPT_KEY_INFO
{
	BYTE        byEncryptEnable;       //  encrypt enable, 0: not encrypt, 1:encrypt
	BYTE        byReserved[3]; 
	union
	{
		BYTE    byDesKey[8];           // des key
		BYTE    by3DesKey[3][8];       // 3des key
		BYTE    byAesKey[32];          // aes key
		
	};
}ENCRYPT_KEY_INFO;

// encrypt algorithm
typedef struct _ALGO_PARAM_INFO
{
	WORD       wEncryptLenth;       // encrypt length, for example: wEncryptLenth = 128,the effictive encprypt key vlue only include byAesKey[0]~[15] in the  ENCRYPT_KEY_INFO struct
									// when the encrypt algorithm is AES,it only support 3 kind length such as 128,192,256
									// when the encrypt algorithm is DES,it has a fixed value 64
									// when the encrypt algorithm is DES,it means the encrypt key's number(2 or 3 encrypt key)
	BYTE       byAlgoWorkMode ;     // work mode, see EM_ENCRYPT_ALOG_WORKMODE 
	BYTE       reserved[13];        // reserved
}ALGO_PARAM_INFO;

// stream encrypt configuration
typedef struct _DHEDV_STREAM_ENCRYPT
{
    BYTE                    byEncrptAlgoType;        // encrypt algorithm type:00: AES,01:DES,02: 3DES
    BYTE                    byReserved1[3];
    ALGO_PARAM_INFO         stuEncrptAlgoparam;      // encrypt algorithm param
    ENCRYPT_KEY_INFO        stuEncryptKeys[32];      // each channel's encrypt key's info
	BYTE					byEncrptPlanEnable;		 // encrpt plan enable
	BYTE					byReserved3[3];
	NET_TIME				stuPreTime;				 // encrpt plan beginning time
	BYTE					reserved2[1360];
}DHEDV_STREAM_ENCRYPT;

// limit stream configuration
typedef struct _DHDEV_BIT_RATE
{
	DWORD                   nExpectCodeRate;          // limit stream (kps)
	BYTE                    byReserved[64];           // reserved
}DHDEV_LIMIT_BIT_RATE;
// custom configuration
typedef struct _DHDEV_CUSTOM_CFG
{
	char                   szData[1024];            // custom configuration information
	char                   reserved[3072];          // reserved
}DHDEV_CUSTOM_CFG;

/*audio talk configuration*/
typedef struct
{
	DWORD				dwSize;
	int					nCompression;			// Compression type,value,refer to DH_TALK_CODING_TYPE,please set the compression type according to the mode the device supports
	int					nMode;					// Encode mode, value, 0 means not support the compression tyep
												// Set the responding type according to compression type
												// like AMR, please refer to EM_ARM_ENCODE_MODE
	char				byReserve[256];			// Reserved
} DHDEV_TALK_ENCODE_CFG;

// According to the mobile function
// (Event triggers message)MMS Configuration Structure
typedef struct
{
	DWORD				dwSize;
	DWORD				dwEnable;				// Enable
	DWORD				dwReceiverNum;			// Receiver amount
	char				SZReceiver[DH_MMS_RECEIVER_NUM][32];	// Receiver,normally cellphone
    BYTE                byType;					// Message type 0:MMS;1:SMS
	char                SZTitle[32];			// Message title
	char				byReserve[223];			// Reserved
} DHDEV_MMS_CFG;

// (Message triggers wireless connection configuration)
typedef struct  
{
	DWORD				dwSize;
	DWORD				dwEnable;				// Enable
	DWORD				dwSenderNum;			// Sender amount
	char				SZSender[DH_MMS_SMSACTIVATION_NUM][32];	// Sender, normally the cellphone number
 	char				byReserve[256];			// Reserved
}DHDEV_SMSACTIVATION_CFG;

// (Dailing triggers the wireless connection)
typedef struct  
{
	DWORD				dwSize;
	DWORD				dwEnable;				// Enable
	DWORD				dwCallerNum;			// Sender amount
	char				SZCaller[DH_MMS_DIALINACTIVATION_NUM][32];	// Sender, normally the cellphone
 	char				byReserve[256];			// Reserved
}DHDEV_DIALINACTIVATION_CFG;
// Above is with the mobile phone functions


// Wireless network signal strength structure
typedef struct
{
	DWORD				dwSize;
	DWORD				dwTDSCDMA;				// TD-SCDMA strength,range:0-100
	DWORD				dwWCDMA;				// WCDMA strength,range:0-100
	DWORD				dwCDMA1x;				// CDMA1x strength,range:0-100
	DWORD				dwEDGE;					// EDGE strength,range:0-100
	DWORD				dwEVDO;					// EVDO strength,range:0-100
	int					nCurrentType;			// Current type
												// 0	The device can't support this
												// 1    TD_SCDMA
												// 2	WCDMA
												// 3	CDMA_1x
												// 4	EDGE
												// 5	EVDO
                                                    // 6 TD-LTE
    DWORD               dwTDLTE;                    // TD-LTE Strength��Range:0:100
	char				byReserve[248];			// Reserved
} DHDEV_WIRELESS_RSS_INFO;

typedef struct _DHDEV_SIP_CFG
{
	BOOL				bUnregOnBoot;			//Unregister on Reboot
	char				szAccoutName[64];		//Account Name
	char				szSIPServer[128];		//SIP Server
	char				szOutBoundProxy[128];	//Outbound Proxy
	DWORD				dwSIPUsrID;				//SIP User ID
	DWORD				dwAuthID;				//Authenticate ID
	char				szAuthPsw[64];			//Authenticate Password
	char				szSTUNServer[128];		//STUN Server
	DWORD				dwRegExp;				//Registration Expiration
	DWORD				dwLocalSIPPort;			//Local SIP Port
	DWORD				dwLocalRTPPort;			//Local RTP Port
	BOOL				bEnable;				// Enable
	char				szNotifyID[128];        // Notify ID
	NET_TIME			stuRegTime;             // register time, read only 
	BYTE				bReserved[868];		    //reserved
} DHDEV_SIP_CFG;

typedef struct _DHDEV_SIP_STATE
{
	int					nStatusNum;				//SIP state number 
	BYTE				byStatus[64];           //0:on line,1:off line,2:invalid sip,3:registering,4:talking
	BYTE				bReserved[64];		    //reserved
}DHDEV_SIP_STATE;

typedef struct _DHDEV_HARDKEY_STATE
{
	BOOL				bState;                 // 0:Hard Key disconnect, 1:Hard Key connect
	BYTE				bReserved[64];          // reserved
}DHDEV_HARDKEY_STATE;

typedef struct _DHDEV_ISCSI_PATHLIST
{
	int					nCount;
	char				szPaths[DH_MAX_ISCSI_PATH_NUM][MAX_PATH_STOR];	// remote path
} DHDEV_ISCSI_PATHLIST;

// wifi capability country
typedef struct _DHDEV_WIFI_ROUTE_CAP_COUNTRY
{
	char				szCountry[32];			// country
	int					nValidChnNum;			// valid channel number
	int					nValideChannels[32];	// valid channel array
	char				reserved[64];			// reserved
} DHDEV_WIFI_ROUTE_CAP_COUNTRY;

typedef struct _DHDEV_WIFI_ROUTE_CAP
{
	int					nCountryNum;				// country number
	DHDEV_WIFI_ROUTE_CAP_COUNTRY stuCountry[256];	// country config
	char				reserved[256];				// reserved
} DHDEV_WIFI_ROUTE_CAP;

//  monitor info
typedef struct _MONITOR_INFO
{
	int					nPresetObjectNum;        // preset object number
	int					nActualObjectNum;        // actural object number
	char				reserved[64]; 
}MONITOR_INFO;

typedef struct _DHDEV_MONITOR_INFO
{
	int					nChannelNumber;
	MONITOR_INFO		stMonitorInfo[64];			//  monitor info
	char				reserved[128];
}DHDEV_MONITOR_INFO;

//Multi Channel Preview Playback Segmentation Ability
typedef struct tagNET_MULTIPLAYBACK_SPLIT_CAP
{
    DWORD            dwSize;
    int              nSliptMode[DH_MAX_MULTIPLAYBACK_SPLIT_NUM]; // Support segmentation Mode  
    int              nModeNumber;       
}NET_MULTIPLAYBACK_SPLIT_CAP;

/***************************** PTZ preset configuration ***********************************/
typedef struct _POINTEANBLE
{
	BYTE				bPoint;					// Preset range[1,80], invalid setting is 0.
	BYTE				bEnable;				// Valid or not,0 invalid,1 valid
	BYTE				bReserved[2];
} POINTEANBLE;

typedef struct _POINTCFG
{
	char				szIP[DH_MAX_IPADDR_LEN];// ip
	int					nPort;					// Port	
	POINTEANBLE			stuPointEnable[80];		// Preset enable
	BYTE				bReserved[256];
}POINTCFG;

typedef struct _DHDEV_POINT_CFG
{
	int					nSupportNum;			//Read-only parameter, shall return to sdk when set, it means supported pre-set number
	POINTCFG			stuPointCfg[16];		// 2D config means point info.
	BYTE				bReserved[256];			// Reserved
}DHDEV_POINT_CFG;
////////////////////////////////Mobile DVR supported////////////////////////////////

// GPS information(Mobile device)
typedef struct _GPS_Info
{
    NET_TIME			revTime;				// position time 
	char				DvrSerial[50];			// Device serial number
    double				longitude;				// Longitude
    double				latidude;				// Latitude 
    double				height;					// Height(meter)
    double				angle;					// Angle(The north pole is the original point and clockwise is +)
    double				speed;					// Speed(Kilometer/hour)
    WORD				starCount;				// Starcount
    BOOL				antennaState;			// Antenna status(true=good,false =bad)
    BOOL				orientationState;		// Orientation status (true=position,false = no position )
} GPS_Info,*LPGPS_Info;

// alarm state info
typedef struct 
{
	int                nAlarmCount;             // alarm count
	int                nAlarmState[128];        // alarm state
	BYTE               byRserved[128];          // reserved
}ALARM_STATE_INFO;

// Snapshot parameter structure 
typedef struct _snap_param
{
	unsigned int		Channel;				// Snapshot channel
	unsigned int		Quality;				// Image quality:level 1 to level 6
	unsigned int		ImageSize;				// Video size;0:QCIF,1:CIF,2:D1
	unsigned int		mode;					// Snapshot mode;0:request one frame,1:send out requestion regularly,2: Request consecutively
	unsigned int		InterSnap;				// Time unit is second.If mode=1, it means send out requestion regularly. The time is valid.
	unsigned int		CmdSerial;				// Request serial number
	unsigned int		Reserved[4];
} SNAP_PARAMS, *LPSNAP_PARAMS;

// Snapshot function setup 
typedef struct 
{
	DWORD				dwSize;
	BYTE				bTimingEnable;				// Schedule snapshot button(The alarm snapshot button is in alarm activation configuration)
	BYTE                bPicIntervalHour;           // timing snapshot interval hour
	short	            PicTimeInterval;			// The time intervals of timing capture (s). At present, the capture device supports the largest time interval is 30 minutes 
	DH_VIDEOENC_OPT		struSnapEnc[SNAP_TYP_NUM]; // Snapshot encode setup. The resolution, video quality, frame rate setup and fram rate are all negative. It means the snapshot times in each second. 
} DHDEV_SNAP_CFG;

// snap function extern configuration
typedef struct 
{
	DWORD				dwSize;
	BYTE				bTimingEnable;				// timing diagram switch(in tach of the alarm configuration)
	BYTE                bPicIntervalHour;           // the number of hours interval
	short	            PicTimeInterval;			// time interval(s),the biggest capture device support interval 30min now                        
	DH_VIDEOENC_OPT		struSnapEnc[SNAP_TYP_NUM];  // snap encode configuration,support resolution ,image quality and frame rate setting,frame rate is negative,say for a second to grasp figure number
	DWORD               dwTrigPicIntervalSecond;    // 
	BYTE                byRserved[256];             // reserved
} DHDEV_SNAP_CFG_EX;
//wifi state of car device
typedef struct  
{
	char	szSSID[128];							//SSID
	BOOL	bEnable;								//If enable wifi function, 0:Disable 1:Enable
	int		nSafeType;								//Verify type
													//0:OPEN 
													//1:RESTRICTE
													//2:WEP
													//3:WPA
													//4:WPA2
													//5:WPA-PSK
													//6:WPA2-PSK
	int		nEncryprion;							//Encryption method
													//0:OPEN
													//1:TKIP
													//2:WEP
													//3:AES
													//4:NONE(without verify)
													//5:AUTO
	                                                //6:SHARED
	int		nStrength;								//AP site signal
	char	szHostIP[128];							//host address
	char	szHostNetmask[128];						//host mask
	char	szHostGateway[128];						//host gate  way
	int		nPriority;								//Priority,(1-32)
	int     nEnDHCP;                                //0:disable 1:enable(default value is 1)
	BYTE	bReserved[1016];
} DHDEV_VEHICLE_WIFI_STATE;

typedef struct
{
	char	szSSID[128];							//SSID
	int		nPriority;								//Priority,(1-32)
	int		nSafeType;								//Verify type
													//0:OPEN 
													//1:RESTRICTE
													//2:WEP
													//3:WPA
													//4:WPA2
													//5:WPA-PSK
													//6:WPA2-PSK
	int		nEncryprion;							//Encryption method
													//0:OPEN
													//1:TKIP
													//2:WEP
													//3:AES
													//4:NONE(No verify)
													//5:AUTO
	                                                //6:SHARED
	char	szKey[128];								//Connection key
	char	szHostIP[128];							//Host address
	char	szHostNetmask[128];						//Host mask
	char	szHostGateway[128];						//Host gateway
	int     nEnDHCP;                                //0:disable 1:enable(default value is 1)
	BYTE    byKeyIndex;                             //Verify index of WEP,0:no support,1-4 is index
	BYTE	bReserved[1019];
} DHDEV_VEHICLE_WIFI_CONFIG;
typedef struct
{
	char    szSSID[128];                            // SSID   
	BYTE    bReserved[256];                         // reserved
}WIFI_CONNECT;
// IP modify configuration
typedef struct __DHCTRL_IPMODIFY_PARAM
{
	int                 nStructSize;
	char				szRemoteIP[DH_MAX_IPADDR_OR_DOMAIN_LEN];		// device IP
	char				szSubmask[DH_MAX_IPADDR_LEN];	                // submask
	char				szGateway[DH_MAX_IPADDR_OR_DOMAIN_LEN];	        // gatway
	char				szMac[DH_MACADDR_LEN];			                // MAC addr
	char				szDeviceType[DH_DEV_TYPE_LEN];	                // device type
}DHCTRL_IPMODIFY_PARAM;

typedef struct 
{
	BOOL	bIsScan;								//0:Not scan wifi (Manually added), 1: scanned wifi
	char	szSSID[128];							//SSID
	int		nSafeType;								//Verify type
													//0:OPEN 
													//1:RESTRICTE
													//2:WEP
													//3:WPA
													//4:WPA2
													//5:WPA-PSK
													//6:WPA2-PSK
	int		nEncryprion;							//Encryption method
													//0:OPEN
													//1:TKIP
													//2:WEP
													//3:AES
													//4:NONE(No verify)	
													//5:AUTO
	                                                //6:SHARED
	char	szKey[128];								//Connection key
	int		nStrength;								//AP site signal
	int		nMaxBitRate;							//AP site maximum transmission rate, with units, read-only
	int		nIsCnnted;								//If success connect read-only
	int		nIsSaved;								//If save read-only
	int		nPriority;								//Priority,(1-32)
	char	szHostIP[128];							//Host address
	char	szHostNetmask[128];						//Host network mask
	char	szHostGateway[128];						//Host gateway
	int		nWifiFreq;								//Radio frequency, using channel logo
	int     nEnDHCP;                                //0:disable 1:enable(default value is 1)
	BYTE    byKeyIndex;                             //Verify index of WEP,0:no support,1-4 is index
	BYTE	bReserved[1019];
}DHDEV_VEHICLE_SINGLE_WIFI_AP_CFG;

typedef struct 
{
	BOOL	bEnable;								//if enable wifi, 0:disable, 1:enable
	int		nWifiApNum;								// Effective number of structure:DHDEV_VEHICLE_WIFI_AP_CFG 
	DHDEV_VEHICLE_SINGLE_WIFI_AP_CFG struWifiAp[64];//Single WIFI AP configration
	int	nReserved[512];								//reserved
}DHDEV_VEHICLE_WIFI_AP_CFG;

typedef struct  
{	
	BOOL   bEnable;									//to enable wift, 0:disable 1:enable
	int    nRetWifiApNum;							//get number of DHDEV_VEHICLE_WIFI_AP_CFG
	int    nMaxWifiApNum;							//apply number of DHDEV_VEHICLE_SINGLE_WIFI_AP_CFG
	DHDEV_VEHICLE_SINGLE_WIFI_AP_CFG* pWifiAp;		//a WIFI AP configuration
	int nReserved[512];								//reserved
}DHDEV_VEHICLE_WIFI_AP_CFG_EX;
// GPS log structure
typedef struct _DH_DEVICE_GPS_LOG_ITEM
{
	DHDEVTIME       stuDevTime;             // device time  
	DWORD		    dwLatidude;				// Longitude(0-180 degree)north Longitude 30.183382 = (30.183382 + 90) * 100000 = 120183382
	DWORD		    dwLongitude;			// Latitude(0-360 degree)east Latitude 120.178274 =(120.178274 + 180) * 100000 = 300178274
	DWORD           dwSpeed;                // speed,dwSpeed/1000*1.852km/h
	DWORD           dwHight;                // hight,m
	DWORD           dwAngle;                // direction,0~360,the north is it's origin, clockwise
	DHDEVTIME       stuGpsTime;             // GPS time 
	BYTE            bAntStatus; 		    // GPS antenna state,0 means good;!=0 meams there has some exception 
	BYTE            bOriStatus; 			// orientation state, != 0 means orientation sucess;
	BYTE            bSatCount; 				// satellite count
	BYTE            bGPSStatus; 			// GPS state,0:not orientation, 1:non differential position information 2:differential position information
	DWORD           dwTemp;                 // temperature(centigrade),if real value is 30.0 centigrade,this param will be valued as 30000
	DWORD           dwHumidity;             // humidity(%),if real value is 30.0%,this param will be valued as 30000
	BYTE            bReserved[24];          // reserved
    
}DH_DEVICE_GPS_LOG_ITEM;

// query GPS param
typedef struct _QUERY_GPS_LOG_PARAM
{
	NET_TIME			stuStartTime;			// start time
	NET_TIME			stuEndTime;				// end time
	int					nStartNum;				// start index,the first time to query an be valued with 0
	BYTE				bReserved[20];
} QUERY_GPS_LOG_PARAM;

typedef struct _GPS_TEMP_HUMIDITY_INFO
{
	double              dTemperature;          // temperature(centigrade),if real value is 30.0 centigrade,this param will be valued as 30000
	double              dHumidity;             // humidity(%),if real value is 30.0%,this param will be valued as 30000
	BYTE                bReserved[128];        // reserved
}GPS_TEMP_HUMIDITY_INFO;

// enclosure type
typedef enum
{
	ENCLOSURE_LIMITSPEED = 0x01,				// speed limit
	ENCLOSURE_DRIVEALLOW = 0x02,				// drive allow
	ENCLOSURE_FORBIDDRIVE = 0x04,				// forbind drive
	ENCLOSURE_LOADGOODS = 0x08,					// load goods
	ENCLOSURE_UPLOADGOODS = 0x10,				// upload goods
}ENCLOSURE_TYPE;

typedef enum
{
	 ENCLOSURE_ALARM_DRIVEIN ,	                 // drive in
	 ENCLOSURE_ALARM_DRIVEOUT,	                 // drive out
	 ENCLOSURE_ALARM_OVERSPEED,				     // over speed
	 ENCLOSURE_ALARM_SPEEDCLEAR,				 // speed clear
}ENCLOSURE_ALARM_TYPE;

typedef struct 
{
	DWORD				dwLongitude;			// longitude
    DWORD				dwLatidude;				// latidude
}GPS_POINT;

// enclosure config
typedef struct _DHDEV_ENCLOSURE_CFG
{
	UINT				unType;                 // mask
	BYTE				bRegion[8];             // front 4 bit means country, province, city, town
	UINT				unId;                   // rect id
	UINT				unSpeedLimit;           // speed limit(km/h)
	UINT				unPointNum;             // rect point number
	GPS_POINT			stPoints[128];			// rect point info
	char          		szStationName[DH_STATION_NAME_LEN];  //station name
	BYTE				reserved[32];           // reserved
}DHDEV_ENCLOSURE_CFG;

// enclosure version config
typedef struct _DHDEV_ENCLOSURE_VERSION_CFG
{
	UINT				unType;                 // type mask,such as LIMITSPEED | DRIVEALLOW
    UINT				unVersion[32];          // version
    int					nReserved;              // reserved 
}DHDEV_ENCLOSURE_VERSION_CFG;

// enclosure alarm info
typedef struct __ALARM_ENCLOSURE_INFO
{
	int					nTypeNumber;                    // type number
	BYTE				bType[16];						// type
	int					nAlarmTypeNumber;               // alarm type number
	BYTE				bAlarmType[16];                 // alarm type
	char				szDriverId[DH_VEHICLE_DRIVERNO_LEN];  // drive id
	UINT				unEnclosureId;      	        // enclosure id
	UINT				unLimitSpeed;	                // speed limit(km/h)
	UINT				unCurrentSpeed;                 // current speed
	NET_TIME			stAlarmTime;                    // alarm time
	DWORD				dwLongitude;					// longitude
	DWORD				dwLatidude;						// latidude
	BYTE          		bOffline;                       // 0-real time 1-tofill  
	BYTE				byReserved[119];                // reserved
}ALARM_ENCLOSURE_INFO;

// RAID state
#define DH_MAX_RAID_NUM  16
typedef struct __RAID_STATE_INFO
{
	char				szName[16];					// Raid name
	BYTE				byType;						// type 1:Jbod     2:Raid0      3:Raid1     4:Raid5
	BYTE				byStatus;					// status 0:unknown ,1:active,2:degraded,3:inactive,4:recovering
	BYTE                byReserved[2];
	int					nCntMem;					// nMember number
	int					nMember[32];				// 1,2,3,.
	int					nCapacity;					// capacity(G)
	int					nRemainSpace;				// remain space(M)
	int					nTank;						// Tank 0:main,1:tank1,2:tank2 ...
	char				reserved[32];
}RAID_STATE_INFO;

typedef struct __ALARM_RAID_INFO
{
	int              nRaidNumber;                   // RAID number
	RAID_STATE_INFO  stuRaidInfo[DH_MAX_RAID_NUM];  // RAID info
	char             reserved[128];
}ALARM_RAID_INFO;
//////////////////////////////////ATM support//////////////////////////////////

typedef struct
{
	int					Offset;					// Symbol position bit Offset
	int					Length;					// Symbol position length
	char				Key[16];				// Symbol position value 
} DH_SNIFFER_FRAMEID;

typedef struct 
{
	int					Offset;					// Symbol position bit offset 
	int					Offset2;				// It is invalid now 
	int					Length;					// The symbol position length
	int					Length2;				// It is invalid now
	char				Title[12];  			// Title value 
	char                Key[12];                // Key value
} DH_SNIFFER_CONTENT;

// Network sniffer setup 
typedef struct 
{
	DH_SNIFFER_FRAMEID	snifferFrameId;			// Each FRAME ID option
	DH_SNIFFER_CONTENT	snifferContent[DH_SNIFFER_CONTENT_NUM];	// The corresponding four sniffers in each FRAME
} DH_SNIFFER_FRAME;

// Configuration structure of each sniffer 
typedef struct
{
	char				SnifferSrcIP[DH_MAX_IPADDR_LEN];	// Sniffer source address 	
	int					SnifferSrcPort;						// Sniffer source port 
	char				SnifferDestIP[DH_MAX_IPADDR_LEN];	// Sniffer destination address 
	int					SnifferDestPort;					// Sniffer destination port 
	char				reserved[28];						// Reserved string 
	DH_SNIFFER_FRAME	snifferFrame[DH_SNIFFER_FRAMEID_NUM];// 6 FRAME options 
	int					displayPosition;					// Display position 
	int					recdChannelMask;					// Channel mask
} DH_ATM_SNIFFER_CFG;

typedef struct  
{
	DWORD				dwSize;
	DH_ATM_SNIFFER_CFG	SnifferConfig[4];
	char				reserved[256];						// Reserved string 
} DHDEV_SNIFFER_CFG;

typedef DH_SNIFFER_FRAMEID DH_SNIFFER_FRAMEID_EX;
typedef DH_SNIFFER_CONTENT DH_SNIFFER_CONTENT_EX;

// Capture network configuration
typedef struct  
{
	DH_SNIFFER_FRAMEID	snifferFrameId;								// Each FRAME ID Options
	DH_SNIFFER_CONTENT	snifferContent[DH_SNIFFER_CONTENT_NUM_EX];	// Each FRAME Corresponding to the contents of capture	
} DH_SNIFFER_FRAME_EX;

// Capture each of the corresponding structure
typedef struct
{
	char				SnifferSrcIP[DH_MAX_IPADDR_LEN];					// Source address capture		
	int					SnifferSrcPort;										// Capture source port
	char				SnifferDestIP[DH_MAX_IPADDR_LEN];					// Destination address capture
	int					SnifferDestPort;									// Capture the target port
	DH_SNIFFER_FRAME_EX	snifferFrame[DH_SNIFFER_FRAMEID_NUM];				// 6 FRAME Options
	int					displayPosition;									// Display Position
	int					recdChannelMask;									// Channel mask
	BOOL				bDateScopeEnable;									// Data sources enable
	BOOL				bProtocolEnable;									// Protocol enable
	char				szProtocolName[DH_SNIFFER_PROTOCOL_SIZE];			// Protocol name
	int					nSnifferMode;										// Capture mode; 0:net,1:232.
	int					recdChannelMask1;									// Channel submask  32 ~ 63 channel
	char				reserved[252];
} DH_ATM_SNIFFER_CFG_EX;

// Atm trade type
#define ATM_MAX_TRADE_TYPE_NAME	64
#define ATM_MAX_TRADE_NUM		1024

typedef struct __DH_ATM_QUERY_TRADE   
{
	int					nTradeTypeNum;										// number of trade types
	int					nExceptionTypeNum;									// number of exception events
	char				szSupportTradeType[ATM_MAX_TRADE_NUM][ATM_MAX_TRADE_TYPE_NAME];    // trade events
	char				szSupportExceptionType[ATM_MAX_TRADE_NUM][ATM_MAX_TRADE_TYPE_NAME];// exception events
} DH_ATM_QUERY_TRADE, *LPDH_ATM_QUERY_TRADE;

/////////////////////////////////NVD support/////////////////////////////////
#define nEncoderID nDecoderID
#define byEncoderID byDecoderID

// Decoder information
typedef struct __DEV_DECODER_INFO 
{
	char			szDecType[64];			// type
	int				nMonitorNum;			// TV number
	int				nEncoderNum;			// Decoder channel number
	BYTE			szSplitMode[16];		// Supported by a number of TV screen partition
	BYTE            bMonitorEnable[16];		// TV enable
	BYTE            bTVTipDisplay;          // TV tip display enable 0:not support 1:support.
	BYTE            reserved1[3];
	BYTE            byLayoutEnable[48];     // every channel's tip display enable
    DWORD           dwLayoutEnMask[2];      // ������ͨ����ʾ������Ϣʹ������,�ӵ�λ����λ֧��64��ͨ��,����dwLayoutEnMask[0]�ǵ�32λ
	char			reserved[4];
} DEV_DECODER_INFO, *LPDEV_DECODER_INFO;

// Encoder information
#ifndef NANJINGDITIE_NVD
typedef struct __DEV_ENCODER_INFO 
{
	char			szDevIp[DH_MAX_IPADDR_LEN];			// IP address of Front-end DVR 
	WORD			wDevPort;							// Port of Front-end DVR
	BYTE			bDevChnEnable;                      // Decoder channel enable
	BYTE			byDecoderID;						// The corresponding channel number decoder
	char			szDevUser[DH_USER_NAME_LENGTH_EX];	// User Name
	char			szDevPwd[DH_USER_PSW_LENGTH_EX];	// Password
	int				nDevChannel;						// Channel Number
	int				nStreamType;						// Stream type; 0:Main Stream, 1:Sub-stream
	BYTE			byConnType;							// -1: auto, 0:TCP, 1:UDP, 2:Multicast
	BYTE			byWorkMode;							// 0:Direct Connect, 1:transmit
	WORD			wListenPort;						// Listening port services, for transmit
	DWORD			dwProtoType;						// Protocol type
														// 0:compatible with each other.
														// 1:private 2nd protocol
														// 2:private system integration protocol
														// 3:private DSS protocol
														// 4:private rtsp protocol
	char			szDevName[64];						// Front device name
	BYTE            byVideoInType;                      // video source type:0-SD,1-HD		
	char			szDevIpEx[DH_MAX_IPADDR_OR_DOMAIN_LEN];// szDevIp extended, IP or domain name
	BYTE            bySnapMode;                         // snap mode(when nStreamType==2 effective) 0:That request a frame,1:Time to send a request
	BYTE            byManuFactory;						// The target device manufacturers,See the enum struct EM_IPC_TYPE
	BYTE            byDeviceType;                       // The target device's device type,0:IPC
	BYTE            byDecodePolicy;                     // The target device's decode policy
														// 0:LatencyLevel3 1:LatencyLevel2
														// 2:LatencyLevel1 3:MiddleLevel
														// 4:FluencyLevel3 5:FluencyLevel2
														// 6:FluencyLevel1
	BYTE            bReserved[3];                          // reserved
	DWORD           dwHttpPort;                         // http port 0-65535
	DWORD           dwRtspPort;                         // RTSP port 0-65535
	char			szChnName[32];						// remote channel name
	char			reserved[4];
} DEV_ENCODER_INFO, *LPDEV_ENCODER_INFO;

#else

// encoder info
typedef struct __DEV_ENCODER_INFO 
{
	char			szDevIp[DH_MAX_IPADDR_LEN];			// IP address of Front-end DVR 
	WORD			wDevPort;							// Port of Front-end DVR
	BYTE			bDevChnEnable;                      // Decoder channel enable
	BYTE			byDecoderID;						// The corresponding channel number decoder
	char			szDevUser[DH_USER_NAME_LENGTH_EX];	// User Name
	char			szDevPwd[DH_USER_PSW_LENGTH_EX];	// Password
	int				nDevChannel;						// Channel Number
	int				nStreamType;						// Stream type; 0:Main Stream, 1:Sub-stream
	BYTE			byConnType;							// -1: auto, 0:TCP, 1:UDP, 2:Multicast
	BYTE			byWorkMode;							// 0:Direct Connect, 1:transmit
	WORD			wListenPort;						// Listening port services, for transmit
	DWORD			dwProtoType;						// Protocol type
														// 0:compatible with each other.
														// 1:private 2nd protocol
														// 2:private system integration protocol
														// 3:private DSS protocol
														// 4:private rtsp protocol
	char			szDevName[32];						// Front device name
	BYTE            byVideoInType;                      // video source type:0-SD,1-HD		
	char			szDevIpEx[DH_MAX_IPADDR_OR_DOMAIN_LEN];// szDevIp extended, IP or domain name
	BYTE            bySnapMode;                         // snap mode(when nStreamType==2 effective) 0:That request a frame,1:Time to send a request
	BYTE            byManuFactory;						// The target device manufacturers,See the enum struct EM_IPC_TYPE
	BYTE            byDeviceType;                       // The target device's device type,0:IPC
	BYTE            byDecodePolicy;                     // The target device's decode policy
														// 0:LatencyLevel3 1:LatencyLevel2
														// 2:LatencyLevel1 3:MiddleLevel
														// 4:FluencyLevel3 5:FluencyLevel2
														// 6:FluencyLevel1
	BYTE            bReserved[3];                          // reserved
	DWORD           dwHttpPort;                         // http port 0-65535
	DWORD           dwRtspPort;                         // RTSP port 0-65535
	char			szChnName[32];						// remote channel name
	char			szMcastIP[DH_MAX_IPADDR_LEN];       // multicast address
	char            reserved[128];
} DEV_ENCODER_INFO, *LPDEV_ENCODER_INFO;

#endif

// decoder protocol rtsp url configuration
typedef struct __DHDEV_DECODER_URL_CFG
{
	DWORD			dwSize;
	char			szMainStreamUrl[MAX_PATH];			// main stream url
	char			szExtraStreamUrl[MAX_PATH];			// extra stream url
} DHDEV_DECODER_URL_CFG;

enum DH_SPLIT_DISPLAY_TYPE
{
    DH_SPLIT_DISPLAY_TYPE_GENERAL=1,          // Common display types
    DH_SPLIT_DISPLAY_TYPE_PIP=2,              // PIP Display Type
};

// CLIENT_CtrlDecTVScreen Interface parameters
typedef struct tagDH_CTRL_DECTV_SCREEN
{
    DWORD           dwSize;                             // The size of the structure
    int             nSplitType;                         // Split mode
    BYTE *          pEncoderChannel;                    // Display Channel, dwDisplayType = DH_SPLIT_DISPLAY_TYPE_GENERAL when Effect, the caller can not be less than the length of the allocated memory size nSplitType
    BYTE            byGroupNo;                          // Input channel group No, when dwDisplayType = DH_SPLIT_DISPLAY_TYPE_PIP said PIP display types are valid
    char            reserved[3];                        // Reserved
    DWORD           dwDisplayType;                      // Display type; see specific display DH_SPLIT_DISPLAY_TYPE (note each mode Content is determined by the "PicInPic", the contents of each mode is displayed by the old rules NVD decision (ie DisChn field decision). Compatible, without which an item, the default display for the general category Type, i.e., "General"
}DH_CTRL_DECTV_SCREEN;

// TV parameters 
typedef struct __DEV_DECODER_TV 
{
	int				nID;								// TV ID
	BOOL			bEnable;							// Enable, open or close
	int				nSplitType;							// Partition number
	DEV_ENCODER_INFO stuDevInfo[16];					// All encoder information
	BYTE			bySupportSplit[10];					// Split mode supported
    BYTE            byGroupNo;     						// Input channel group No, when dwDisplayType = DH_SPLIT_DISPLAY_TYPE_PIP said PIP display types are valid
    char			reserved[1];                        // Reserved bytes
    DWORD           dwDisplayType;                      // Display type; see specific display DH_SPLIT_DISPLAY_TYPE (note each mode 
                                                        // Content is determined by the "PicInPic", the contents of each mode is displayed by the old rules NVD decision (ie DisChn field decision). Compatible, without which an item, the default display for the general category 
                                                        // Type, i.e., "General")

} DEV_DECODER_TV, *LPDEV_DECODER_TV;

// Screen combination of information
typedef struct __DEC_COMBIN_INFO
{
	int				nCombinID;							// Combin ID
	int             nSplitType;							// Partition number
	BYTE            bDisChn[16];						// Display channel
	char			reserved[16];
} DEC_COMBIN_INFO, *LPDEC_COMBIN_INFO;

// Tour Information
#define DEC_COMBIN_NUM 			32						// the number of tour combination
typedef struct __DEC_TOUR_COMBIN 
{
	int				nTourTime;							// Tour Interval(s)
	int				nCombinNum;							// the number of combination
	BYTE			bCombinID[DEC_COMBIN_NUM];			// Combination Table
	char			reserved1[32];
	BYTE			bCombinState[DEC_COMBIN_NUM];		// Combination option state,0:close;1:open
	char			reserved2[32];
} DEC_TOUR_COMBIN, *LPDEC_TOUR_COMBIN;

// Decoder Playback type
typedef enum __DEC_PLAYBACK_MODE
{
	Dec_By_Device_File = 0,								// Front-end DVR--By File
	Dec_By_Device_Time,									// Front-end DVR--By Time
} DEC_PLAYBACK_MODE;

// Decoder Playback control type
typedef enum __DEC_CTRL_PLAYBACK_TYPE
{
	Dec_Playback_Seek = 0,								// Drag
	Dec_Playback_Play,									// Play
	Dec_Playback_Pause,									// Pause
	Dec_Playback_Stop,									// Stop
} DEC_CTRL_PLAYBACK_TYPE;

// tour ctrol type
typedef enum __DEC_CTRL_TOUR_TYPE
{
	Dec_Tour_Stop = 0,									// stop
	Dec_Tour_Start,										// start
	Dec_Tour_Pause,										// pause
	Dec_Tour_Resume,									// resume
} DEC_CTRL_TOUR_TYPE;

// Playback by file Conditions
typedef struct __DEC_PLAYBACK_FILE_PARAM 
{
	char			szDevIp[DH_MAX_IPADDR_LEN];			// IP address of Front-end DVR
	WORD			wDevPort;							// Port of Front-end DVR
	BYTE			bDevChnEnable;                      // Decoder channel enable
	BYTE			byDecoderID;						// The corresponding channel number
	char			szDevUser[DH_USER_NAME_LENGTH_EX];	// User Name
	char			szDevPwd[DH_USER_PSW_LENGTH_EX];	// Password
	NET_RECORDFILE_INFO stuRecordInfo;					// Record file information
	char			reserved[12];
} DEC_PLAYBACK_FILE_PARAM, *LPDEC_PLAYBACK_FILE_PARAM;

// Playback by time Conditions
typedef struct __DEC_PLAYBACK_TIME_PARAM 
{
	char			szDevIp[DH_MAX_IPADDR_LEN];			// IP address of Front-end DVR
	WORD			wDevPort;							// Port of Front-end DVR
	BYTE			bDevChnEnable;                      // Decoder channel enable
	BYTE			byDecoderID;						// The corresponding channel number
	char			szDevUser[DH_USER_NAME_LENGTH_EX];	// User Name
	char			szDevPwd[DH_USER_PSW_LENGTH_EX];	// Password
	int				nChannelID;
	NET_TIME		startTime;
	NET_TIME		endTime;
	char			reserved[12];
} DEC_PLAYBACK_TIME_PARAM, *LPDEC_PLAYBACK_TIME_PARAM;

// Current decoding channel status(including channel status, decoding stream info and etc.)
typedef struct __DEV_DECCHANNEL_STATE
{
	BYTE			byDecoderID;						// Responding decoding channel number
	BYTE            byChnState;                         // Current decoding channel in opertion status:0 -free,1 -realtime monitoring,2 - playback 3 - Decode Tour
	BYTE			byFrame;                            // Current data frame rate
	BYTE            byReserved;                         // Reserved
	int				nChannelFLux;						// Decoding channel data amount
	int             nDecodeFlux;						// Decoding data amount
	char            szResolution[16];                   // Current data resolution
	char			reserved[256];
} DEV_DECCHANNEL_STATE, *LPDEV_DECCHANNEL_STATE;

// Device TV display info
typedef struct __DEV_VIDEOOUT_INFO
{
	DWORD				dwVideoStandardMask;			// NSP,video standard mask,according to the bit which shows video format(not support now)
	int					nVideoStandard;                 // NSP,current format(not support now,please use DHDEV_SYSTEM_ATTR_CFG by VideoStandard to read and config the video format)
	DWORD				dwImageSizeMask;				// Resolution mask,according the bit which shows video resolution
	int                 nImageSize;                     // Current resolution
	char				reserved[256];
}DEV_VIDEOOUT_INFO, *LPDEV_VIDEOOUT_INFO;

// TV adjust
typedef struct __DEV_TVADJUST_CFG
{
	int					nTop;							// top(0 - 100)
	int					nBotton;						// botton(0 - 100)
	int					nLeft;							// left(0 - 100)
	int                 nRight;							// right(0 - 100)
	int					reserved[32];
}DHDEV_TVADJUST_CFG, *LPDHDEV_TVADJUST_CFG;

// decoder tour configuration
typedef struct __DEV_DECODER_TOUR_SINGLE_CFG
{
	char		szIP[128];								// Front-end device's ip.Such as"10.7.5.21". Support retention of the domain name, end by '\0'.
	int			nPort;									// Front-end device's port.(0, 65535).
	int			nPlayChn;								// front-end device's Request channel [1, 16].
	int			nPlayType;								// front-end device's Stream type, 0:main; 1:sub.
	char		szUserName[32];							// front-end device's user name,end by '\0'.
	char		szPassword[32];							// front-end device's Password,end by '\0'.
	int			nInterval;								// Round tour interval [10, 120],s.
	DWORD		nManuFactory;							// device's factory(enum see IPC_TYPE)
	UINT		nHttpPport;								// device's http port
	UINT		nRtspPort;								// device's rtsp port
	BYTE		byServiceType;							// service type -1:auto,0:TCP;1:UDP;2:Multicast 
	BYTE		bReserved[51];							// reserved.
}DHDEV_DECODER_TOUR_SINGLE_CFG;

typedef struct __DEV_DECODER_TOUR_CFG
{
	int								nCfgNum;			// the number of Configure structures. The biggest support 32. Specific number of support inquiries through capacity.
	DHDEV_DECODER_TOUR_SINGLE_CFG	tourCfg[64];		// Polling configuration array, the effective number of structures by the members of the "nCfgNum" designation. Keep left to expand 32.
	BYTE							bReserved[256];		// reserved.
}DHDEV_DECODER_TOUR_CFG;

/////////////////////////////////intelligent support/////////////////////////////////
// picture info
typedef struct  
{
	DWORD                dwOffSet;                       // current picture file's offset in the binary file, byte
	DWORD                dwFileLenth;                    // current picture file's size, byte
	WORD                 wWidth;                         // picture width, pixel
	WORD                 wHeight;                        // picture high, pixel
	BYTE                 bReserved[16];                 
}DH_PIC_INFO;

// Extension fields added int64, forced 4 byte alignment
#pragma pack(push)
#pragma pack(4)
// Struct of object info for video analysis 
typedef struct 
{
	int					nObjectID;						// Object ID,each ID represent a unique object
	char				szObjectType[128];				// Object type
	int					nConfidence;					// Confidence(0~255),a high value indicate a high confidence
	int					nAction;						// Object action:1:Appear 2:Move 3:Stay 4:Remove 5:Disappear 6:Split 7:Merge 8:Rename
	DH_RECT				BoundingBox;					// BoundingBox
	DH_POINT			Center;							// The shape center of the object
	int					nPolygonNum;					// the number of culminations for the polygon
	DH_POINT			Contour[DH_MAX_POLYGON_NUM];	// a polygon that have a exactitude figure
	DWORD				rgbaMainColor;					// The main color of the object;the first byte indicate red value, as byte order as green, blue, transparence, for example:RGB(0,255,0),transparence = 0, rgbaMainColor = 0x00ff0000.
    char				szText[128];					// the interrelated text of object,such as number plate,container number
                                                            // "ObjectType"Ϊ"Vehicle"����"Logo"ʱ������ʹ��Logo��Vehicle��Ϊ�˼����ϲ�Ʒ����ʾ���֧꣬�֣�
                                                            // "Unknown"δ֪ 
                                                            // "Audi" �µ�
                                                            // "Honda" ����
                                                            // "Buick" ���
                                                            // "Volkswagen" ����
                                                            // "Toyota" ����
                                                            // "BMW" ����
                                                            // "Peugeot" ����
                                                            // "Ford" ����
                                                            // "Mazda" ���Դ�
                                                            // "Nissan" ��ɣ
                                                            // "Hyundai" �ִ�
                                                            // "Suzuki" ��ľ
                                                            // "Citroen" ѩ����
                                                            // "Benz" ����
                                                            // "BYD" ���ǵ�
                                                            // "Geely" ����
                                                            // "Lexus" �׿���˹
                                                            // "Chevrolet" ѩ����
                                                            // "Chery" ����
                                                            // "Kia" ����
                                                            // "Charade" ����
                                                            // "DF" ����
                                                            // "Naveco" ��ά��
                                                            // "SGMW" ����
                                                            // "Jinbei" ��

	char                szObjectSubType[64];            // object sub type,different object type has different sub type:
														// Vehicle Category:"Unknown","Motor","Non-Motor","Bus","Bicycle","Motorcycle"
														// Plate Category:"Unknown","mal","Yellow","DoubleYellow","Police","Armed",
														// "Military","DoubleMilitary","SAR","Trainning"
														// "Personal" ,"Agri","Embassy","Moto","Tractor","Other"
														// HumanFace Category:"Normal","HideEye","HideNose","HideMouth"
	BYTE                byReserved1[3];
	bool                bPicEnble;                     // picture info enable
	DH_PIC_INFO         stPicInfo;                     // picture info
	bool				bShotFrame;						// is shot frame
	bool				bColor;							// rgbaMainColor is enable
	BYTE				byReserved2;
	BYTE                byTimeType;                     // ��Time indicates the type of detailed instructions��EM_TIME_TYP
    NET_TIME_EX			stuCurrentTime;					// in view of the video compression,current time(when object snap or reconfnition, the frame will be attached to the frame in a video or pictures,means the frame in the original video of the time)
	NET_TIME_EX			stuStartTime;					// strart time(object appearing for the first time)
	NET_TIME_EX			stuEndTime;						// end time(object appearing for the last time)
	DH_RECT				stuOriginalBoundingBox;			// original bounding box(absolute coordinates)
	DH_RECT             stuSignBoundingBox;             // sign bounding box coordinate
	DWORD				dwCurrentSequence;				// The current frame number (frames when grabbing the object)
	DWORD				dwBeginSequence;				// Start frame number (object appeared When the frame number��
	DWORD				dwEndSequence;					// The end of the frame number (when the object disappearing Frame number)
	INT64				nBeginFileOffset;				// At the beginning of the file offset, Unit: Word Section (when objects began to appear, the video frames in the original video file offset relative to the beginning of the file��
	INT64				nEndFileOffset;					// At the end of the file offset, Unit: Word Section (when the object disappeared, video frames in the original video file offset relative to the beginning of the file)
	BYTE                byColorSimilar[NET_COLOR_TYPE_MAX];// Object color similarity, the range :0-100, represents an array subscript Colors, see EM_COLOR_TYPE��
 	BYTE                byUpperBodyColorSimilar[NET_COLOR_TYPE_MAX]; // When upper body color similarity (valid object type man ��
 	BYTE                byLowerBodyColorSimilar[NET_COLOR_TYPE_MAX]; // Lower body color similarity when objects (object type human valid ��
    int                 nRelativeID;                        // ID of relative object
	BYTE				byReserved[24];
} DH_MSG_OBJECT;
#pragma pack(pop)

// snapshot info
typedef struct
{
	short              snSpeed;                          // current car speed,km/h
	short              snCarLength;                      // current car length, dm
	float              fRedTime;                         // current red light time, s.ms
	float              fCapTime;                         // current car way snapshot time, s.ms 
	BYTE               bSigSequence;                     // current snapshot Sequence
	BYTE               bType;                            // current snapshot type
														 // 0: radar up speed limit;1: radar low speed limit;2: car detector up speed limit;3:car detector low speed limit
														 // 4: reverse;5: break red light;6: red light on;7: red light off;8: snapshot or traffic gate
	BYTE               bDirection;                       // breaking type :01:left turn;02:straight;03:right
	BYTE               bLightColor;                      // current car way traffic light state,0: green, 1: red, 2: yellow
	BYTE               bSnapFlag[16];                    // snap flag from device
}DH_SIG_CARWAY_INFO;

// Vehicle detector redundancy info
typedef struct
{
	BYTE                byRedundance[8];                // The vehicle detector generates the snap signal redundancy info
	BYTE                bReserved[120];                 // Reserved field
}DH_SIG_CARWAY_INFO_EX;


// car way info
typedef struct  
{
	BYTE                bCarWayID;                           // current car way id 
	BYTE                bReserve[2];                         // reserved
	BYTE                bSigCount;                           // being snapshotted
	DH_SIG_CARWAY_INFO  stuSigInfo[DH_MAX_SNAP_SIGNAL_NUM];  // the snapshot info	
	BYTE                bReserved[12];                       // reserved
}DH_CARWAY_INFO;

// event file info
typedef struct  
{
	BYTE               bCount;                               // the file count in the current file's group
	BYTE               bIndex;                               // the index of the file in the group
	BYTE               bFileTag;                             // file tag, see the enum struct EM_EVENT_FILETAG
	BYTE               bFileType;                            // file type,0-normal 1-compose 2-cut picture
	NET_TIME_EX        stuFileTime;                          // file time
	DWORD              nGroupId;                             // the only id of one group file
}DH_EVENT_FILE_INFO;

// pic resolution 
typedef struct
{
	unsigned short   snWidth;    // width
 	unsigned short   snHight;    // hight
}DH_RESOLUTION_INFO;

// person info
typedef struct tagFACERECOGNITION_PERSON_INFO
{
	char                szPersonName[DH_MAX_NAME_LEN];		// name                 
	WORD				wYear;								// birth year
	BYTE				byMonth;							// birth month
	BYTE				byDay;								// birth day
	char                szID[DH_MAX_PERSON_ID_LEN];			// the unicle ID for the person
	BYTE                bImportantRank;						// importance level,1~10,the higher value the higher level
	BYTE                bySex;								// sex, 0-man, 1-female
	WORD                wFacePicNum;						// picture number
	DH_PIC_INFO         szFacePicInfo[DH_MAX_PERSON_IMAGE_NUM]; // picture info
	BYTE                byType;                                         // Personnel types, see EM_PERSON_TYPE
	BYTE                byIDType;                                       // Document types, see EM_CERTIFICATE_TYPE
	BYTE                bReserved1[2];                                  // Byte alignment
	char                szProvince[DH_MAX_PROVINCE_NAME_LEN];           // province
	char                szCity[DH_MAX_CITY_NAME_LEN];                   // city
	char                szPersonNameEx[DH_MAX_PERSON_NAME_LEN];	        // Name, the name is too long due to the presence of 16 bytes can not be Storage problems, the increase in this parameter
	BYTE                bReserved[60];
}FACERECOGNITION_PERSON_INFO;

// cadidate person info
typedef struct tagCANDIDATE_INFO
{
	FACERECOGNITION_PERSON_INFO  stPersonInfo;            // person info
	BYTE                         bySimilarity;            // similarity
	BYTE                         byRange;                  // Range officer's database, see EM_FACE_DB_TYPE
	BYTE                         byReserved1[2];
	NET_TIME                     stTime;                  // When byRange historical database effectively, which means that the query time staff appeared
	char                         szAddress[MAX_PATH];     // When byRange historical database effectively, which means that people place a query appears
	BYTE                         byReserved[128];         // Reserved bytes
}CANDIDATE_INFO;

// TrafficCar
typedef struct tagDEV_EVENT_TRAFFIC_TRAFFICCAR_INFO
{
	char               szPlateNumber[32];               // plate number
	char               szPlateType[32];                 // plate type
	char               szPlateColor[32];                // plate color, "Blue","Yellow", "White","Black"
	char               szVehicleColor[32];              // vehicle color, "White", "Black", "Red", "Yellow", "Gray", "Blue","Green"
	int                nSpeed;                          // speed, Km/H
	char               szEvent[64];                     // trigger event type
	char               szViolationCode[32];             // violation code, see TrafficGlobal.ViolationCode
	char               szViolationDesc[64];             // violation describe
	int                nLowerSpeedLimit;                // lower speed limit
	int                nUpperSpeedLimit;                // upper speed limit
	int                nOverSpeedMargin;                // over speed margin, km/h 
	int                nUnderSpeedMargin;               // under speed margin, km/h 
	int                nLane;                           // lane	
	int                nVehicleSize;                    // vehicle size, see VideoAnalyseRule's describe
                                                            // ��0λ:"Light-duty", С�ͳ�
                                                            // ��1λ:"Medium", ���ͳ�
                                                            // ��2λ:"Oversize", ���ͳ�
                                                            // ��3λ:"Minisize", ΢�ͳ�
                                                            // ��4λ:"Largesize", ����
	float              fVehicleLength;                  // vehicle length, m
	int                nSnapshotMode;                   // snap mode 0-normal,1-globle,2-near,4-snap on the same side,8-snap on the reverse side,16-plant picture
	char               szChannelName[32];               // channel name
	char               szMachineName[256];              // Machine name
	char               szMachineGroup[256];             // machine group
	char               szRoadwayNo[64];                 // road way number	
	char               szDrivingDirection[3][DH_MAX_DRIVINGDIRECTION];   
																			// DrivingDirection: for example ["Approach", "Shanghai", "Hangzhou"]
													     					// "Approach" means driving direction,where the car is more near;"Leave"-means where if mor far to the car
														 					// the second and third param means the location of the driving direction
	char              *szDeviceAddress;                 // device address,OSD superimposed onto the image,from TrafficSnapshot.DeviceAddress,'\0'means end.
	char			   szVehicleSign[32];				// Vehicle identification, such as "Unknown" - unknown "Audi" - Audi, "Honda" - Honda ...
	DH_SIG_CARWAY_INFO_EX stuSigInfo;                   // Generated by the vehicle inspection device to capture the signal redundancy
	char			  *szMachineAddr;					// Equipment deployment locations
	float              fActualShutter;                  // Current picture exposure time, in milliseconds
	BYTE               byActualGain;                    // Current picture gain, ranging from 0 to 100
	BYTE			   byDirection;						// 0 - south to north 1- Southwest to northeast 2 - West to east, 3 - Northwest to southeast 4 - north to south 5 - northeast to southwest 6 - East to West 7 - Southeast to northwest 8 - Unknown
	BYTE			   byReserved[2];
	char*			   szDetailedAddress;				// Address, as szDeviceAddress supplement��
    BYTE               bReserved[848];                  // reserved
}DEV_EVENT_TRAFFIC_TRAFFICCAR_INFO;

// the describe of EVENT_IVS_CROSSLINEDETECTION's data
typedef struct tagDEV_EVENT_CROSSLINE_INFO 
{
	int					nChannelID;						// ChannelId
	char				szName[128];					// event name
	char                bReserved1[4];                  // byte alignment
	double				PTS;							// PTS(ms)
	NET_TIME_EX			UTC;							// the event happen time
	int					nEventID;						// event ID
	DH_MSG_OBJECT		stuObject;						// have being detected object
	DH_EVENT_FILE_INFO  stuFileInfo;                    // event file info
	DH_POINT            DetectLine[DH_MAX_DETECT_LINE_NUM];    // rule detect line
	int                 nDetectLineNum;                        // rule detect line's point number
	DH_POINT            TrackLine[DH_MAX_TRACK_LINE_NUM];      // object moveing track
	int                 nTrackLineNum;                         // object moveing track's point number
	BYTE                bEventAction;                   // Event action,0 means pulse event,1 means continuous event's begin,2means continuous event's end;
	BYTE                bDirection;                     // direction, 0-left to right, 1-right to left
	BYTE                byReserved[1];
	BYTE				byImageIndex;					// Serial number of the picture, in the same time (accurate to seconds) may have multiple images, starting from 0
	DWORD               dwSnapFlagMask;	                // flag(by bit),see NET_RESERVED_COMMON
	int                 nSourceIndex;                   // the source device's index,-1 means data in invalid
	char                szSourceDevice[MAX_PATH];       // the source device's sign(exclusive),field said local device does not exist or is empty
	BYTE				bReserved[612];					// reserved

} DEV_EVENT_CROSSLINE_INFO;

// the describe of EVENT_IVS_CROSSREGIONDETECTION's data
typedef struct tagDEV_EVENT_CROSSREGION_INFO 
{
	int					nChannelID;						// ChannelId
	char				szName[128];					// event name
	char                bReserved2[4];                  // byte alignment
	double				PTS;							// PTS(ms)
	NET_TIME_EX			UTC;							// the event happen time
	int					nEventID;						// event ID
	DH_MSG_OBJECT		stuObject;						// have being detected object
	DH_EVENT_FILE_INFO  stuFileInfo;                    // event file info
		DH_POINT            DetectRegion[DH_MAX_DETECT_REGION_NUM];    // rule detect region
	int                 nDetectRegionNum;                          // rule detect region's point number
	DH_POINT            TrackLine[DH_MAX_TRACK_LINE_NUM];          // object moving track
	int                 nTrackLineNum;                             // object moving track's point number
		BYTE                bEventAction;                   // Event action,0 means pulse event,1 means continuous event's begin,2means continuous event's end;
	BYTE                bDirection;                     // direction, 0-in, 1-out,2-apaer,3-leave
	BYTE                bActionType;                    // action type,0-appear 1-disappear 2-in area 3-cross area
	BYTE				byImageIndex;					// Serial number of the picture, in the same time (accurate to seconds) may have multiple images, starting from 0
    DWORD               dwSnapFlagMask;	                // flag(by bit),see NET_RESERVED_COMMON
	int                 nSourceIndex;                   // the source device's index,-1 means data in invalid
	char                szSourceDevice[MAX_PATH];       // the source device's sign(exclusive),field said local device does not exist or is empty
	BYTE				bReserved[540];					// reserved
	int                 nObjectNum;                     // Detect object amount
	DH_MSG_OBJECT		stuObjectIDs[DH_MAX_OBJECT_LIST];// Detected object
	int                 nTrackNum;                      // Locus amount(Corresponding to the detected object amount.)
	DH_POLY_POINTS      stuTrackInfo[DH_MAX_OBJECT_LIST];// Locus info(Corresponding to the detected object)
} DEV_EVENT_CROSSREGION_INFO;

// the describe of EVENT_IVS_PASTEDETECTION's data
typedef struct tagDEV_EVENT_PASTE_INFO 
{
	int					nChannelID;						// ChannelId
	char				szName[128];					// event name
	char                bReserved1[4];                  // byte alignment
	double				PTS;							// PTS(ms)
	NET_TIME_EX			UTC;							// the event happen time
	int					nEventID;						// event ID
	DH_MSG_OBJECT		stuObject;						// have being detected object
	DH_EVENT_FILE_INFO  stuFileInfo;                    // event file info
	int                 nDetectRegionNum;				// rule detect region's point number
	DH_POINT            DetectRegion[DH_MAX_DETECT_REGION_NUM];    // rule detect region
	BYTE                bEventAction;                   // Event action,0 means pulse event,1 means continuous event's begin,2means continuous event's end;
	BYTE                byReserved[2];
	BYTE				byImageIndex;					// Serial number of the picture, in the same time (accurate to seconds) may have multiple images, starting from 0
    DWORD               dwSnapFlagMask;	                // flag(by bit),see NET_RESERVED_COMMON
	BYTE				bReserved[888];			    	// Reserved

} DEV_EVENT_PASTE_INFO;

// the describe of EVENT_IVS_LEFTDETECTION's data
typedef struct tagDEV_EVENT_LEFT_INFO 
{
	int					nChannelID;						// ChannelId
	char				szName[128];					// event name
	char                bReserved1[4];                  // byte alignment
	double				PTS;							// PTS(ms)
	NET_TIME_EX			UTC;							// the event happen time
	int					nEventID;						// event ID
	DH_MSG_OBJECT		stuObject;						// have being detected object
	DH_EVENT_FILE_INFO  stuFileInfo;                    // event file info
	BYTE                bEventAction;                   // Event action,0 means pulse event,1 means continuous event's begin,2means continuous event's end;
	BYTE                byReserved[2];
	BYTE				byImageIndex;					// Serial number of the picture, in the same time (accurate to seconds) may have multiple images, starting from 0
	int                 nDetectRegionNum;				// detect region point number
	DH_POINT            DetectRegion[DH_MAX_DETECT_REGION_NUM];    // detect region
    DWORD               dwSnapFlagMask;	                // flag(by bit),see NET_RESERVED_COMMON
	int                 nSourceIndex;                   // the source device's index,-1 means data in invalid
	char                szSourceDevice[MAX_PATH];       // the source device's sign(exclusive),field said local device does not exist or is empty
	BYTE				bReserved[624];			    	// Reserved

} DEV_EVENT_LEFT_INFO;

// the describe of EVENT_IVS_PRESERVATION's data
typedef struct tagDEV_EVENT_PRESERVATION_INFO 
{
	int					nChannelID;						// ChannelId
	char				szName[128];					// event name
	char                bReserved1[4];                  // byte alignment
	double				PTS;							// PTS(ms)
	NET_TIME_EX			UTC;							// the event happen time
	int					nEventID;						// event ID
	DH_MSG_OBJECT		stuObject;						// have being detected object
	DH_EVENT_FILE_INFO  stuFileInfo;                    // event file info
	BYTE                bEventAction;                   // Event action,0 means pulse event,1 means continuous event's begin,2means continuous event's end;
    BYTE                byReserved[2];
    BYTE                byImageIndex;                   // Serial number of the picture, in the same time (accurate to seconds) may have multiple images, starting from 0
	int                 nDetectRegionNum;				// detect region point number
	DH_POINT            DetectRegion[DH_MAX_DETECT_REGION_NUM];    // detect region
    DWORD               dwSnapFlagMask;	                // flag(by bit),see NET_RESERVED_COMMON
	int                 nSourceIndex;                   // the source device's index,-1 means data in invalid
	char                szSourceDevice[MAX_PATH];       // the source device's sign(exclusive),field said local device does not exist or is empty
	BYTE				bReserved[624];			    	// Reserved

} DEV_EVENT_PRESERVATION_INFO;

// the describe of EVENT_IVS_STAYDETECTION's data
typedef struct tagDEV_EVENT_STAY_INFO 
{
	int					nChannelID;						// ChannelId
	char				szName[128];					// event name
	char                bReserved1[4];                  // byte alignment
	double				PTS;							// PTS(ms)
	NET_TIME_EX			UTC;							// the event happen time
	int					nEventID;						// event ID
	DH_MSG_OBJECT		stuObject;						// have being detected object
	DH_EVENT_FILE_INFO  stuFileInfo;                    // event file info
	BYTE                bEventAction;                   // Event action,0 means pulse event,1 means continuous event's begin,2means continuous event's end;
    BYTE                byReserved[2];
    BYTE                byImageIndex;                   // Serial number of the picture, in the same time (accurate to seconds) may have multiple images, starting from 0
	int                 nDetectRegionNum;				// detect region point number
	DH_POINT            DetectRegion[DH_MAX_DETECT_REGION_NUM];    // detect region
    DWORD               dwSnapFlagMask;	                // flag(by bit),see NET_RESERVED_COMMON
	int                 nSourceIndex;                   // the source device's index,-1 means data in invalid
	char                szSourceDevice[MAX_PATH];       // the source device's sign(exclusive),field said local device does not exist or is empty
	BYTE				bReserved[624];			    	// Reserved

} DEV_EVENT_STAY_INFO;

// the describe of EVENT_IVS_WANDERDETECTION's data
typedef struct tagDEV_EVENT_WANDER_INFO 
{
	int					nChannelID;						// ChannelId
	char				szName[128];					// event name
	char                bReserved1[4];                  // byte alignment
	double				PTS;							// PTS(ms)
	NET_TIME_EX			UTC;							// the event happen time
	int					nEventID;						// event ID
	DH_EVENT_FILE_INFO  stuFileInfo;                    // event file info
	BYTE                bEventAction;                   // Event action,0 means pulse event,1 means continuous event's begin,2means continuous event's end;
    BYTE                byReserved[2];                  // 
    BYTE                byImageIndex;                   // Serial number of the picture, in the same time (accurate to seconds) may have multiple images, starting from 0
	int                 nObjectNum;                     // detected objects number
	DH_MSG_OBJECT		stuObjectIDs[DH_MAX_OBJECT_LIST];	// detected objects
	int                 nTrackNum;                      // track number
	DH_POLY_POINTS      stuTrackInfo[DH_MAX_OBJECT_LIST];// track info
	int                 nDetectRegionNum;				// detect region point number
	DH_POINT            DetectRegion[DH_MAX_DETECT_REGION_NUM];    // detect region
    DWORD               dwSnapFlagMask;	                // flag(by bit),see NET_RESERVED_COMMON
	int                 nSourceIndex;                   // the source device's index,-1 means data in invalid
	char                szSourceDevice[MAX_PATH];       // the source device's sign(exclusive),field said local device does not exist or is empty
	BYTE				bReserved[760];					// reserved

} DEV_EVENT_WANDER_INFO;

// the describe of EVENT_IVS_MOVEDETECTION's data
typedef struct tagDEV_EVENT_MOVE_INFO 
{
	int					nChannelID;						// ChannelId
	char				szName[128];					// event name
	char                bReserved1[4];                  // byte alignment
	double				PTS;							// PTS(ms)
	NET_TIME_EX			UTC;							// the event happen time
	int					nEventID;						// event ID
	DH_MSG_OBJECT		stuObject;						// have being detected object
	DH_EVENT_FILE_INFO  stuFileInfo;                    // event file info
	BYTE                bEventAction;                   // Event action,0 means pulse event,1 means continuous event's begin,2means continuous event's end;
    BYTE                byReserved[2];
    BYTE                byImageIndex;                   // Serial number of the picture, in the same time (accurate to seconds) may have multiple images, starting from 0
	int                 nDetectRegionNum;				// detect region point 
	DH_POINT            DetectRegion[DH_MAX_DETECT_REGION_NUM];    // detect region
    DWORD               dwSnapFlagMask;	                // flag(by bit),see NET_RESERVED_COMMON
	int                 nSourceIndex;                   // the source device's index,-1 means data in invalid
	char                szSourceDevice[MAX_PATH];       // the source device's sign(exclusive),field said local device does not exist or is empty
	int                 nTrackLineNum;                  // Object trajectories vertices              
	DH_POINT            stuTrackLine[DH_MAX_TRACK_LINE_NUM]; // Object trajectories			
	BYTE				bReserved[540];					 // Reserved bytes, leave extended

} DEV_EVENT_MOVE_INFO;

// the describe of EVENT_IVS_TAILDETECTION's data
typedef struct tagDEV_EVENT_TAIL_INFO 
{
	int					nChannelID;						// ChannelId
	char				szName[128];					// event name
	char                bReserved1[4];                  // byte alignment
	double				PTS;							// PTS(ms)
	NET_TIME_EX			UTC;							// the event happen time
	int					nEventID;						// event ID
	DH_MSG_OBJECT		stuObject;						// have being detected object
	DH_EVENT_FILE_INFO  stuFileInfo;                    // event file info
	BYTE                bEventAction;                   // Event action,0 means pulse event,1 means continuous event's begin,2means continuous event's end;
    BYTE                byReserved[2];
    BYTE                byImageIndex;                   // Serial number of the picture, in the same time (accurate to seconds) may have multiple images, starting from 0
	int                 nDetectRegionNum;				// detect region point 
	DH_POINT            DetectRegion[DH_MAX_DETECT_REGION_NUM];    //  detect region
    DWORD               dwSnapFlagMask;	                // flag(by bit),see NET_RESERVED_COMMON
	int                 nSourceIndex;                   // the source device's index,-1 means data in invalid
	char                szSourceDevice[MAX_PATH];       // the source device's sign(exclusive),field said local device does not exist or is empty
	BYTE				bReserved[624];					

} DEV_EVENT_TAIL_INFO;

// the describe of EVENT_IVS_RIOTERDETECTION's data
typedef struct tagDEV_EVENT_RIOTER_INFO 
{
	int					nChannelID;						// ChannelId
	char				szName[128];					// event name
	char                bReserved1[4];                  // byte alignment
	double				PTS;							// PTS(ms)
	NET_TIME_EX			UTC;							// the event happen time
	int					nEventID;						// event ID
	int					nObjectNum;						// have being detected object number
	DH_MSG_OBJECT		stuObjectIDs[DH_MAX_OBJECT_LIST];// have being detected object list
	DH_EVENT_FILE_INFO  stuFileInfo;                    // event file info
	BYTE                bEventAction;                   // Event action,0 means pulse event,1 means continuous event's begin,2means continuous event's end;
    BYTE                byReserved[2];                  // 
    BYTE                byImageIndex;                   // Serial number of the picture, in the same time (accurate to seconds) may have multiple images, starting from 0
	int                 nDetectRegionNum;				// detect region point 
	DH_POINT            DetectRegion[DH_MAX_DETECT_REGION_NUM];    // detect region

    DWORD               dwSnapFlagMask;	                // flag(by bit),see NET_RESERVED_COMMON
	int                 nSourceIndex;                   // the source device's index,-1 means data in invalid
	char                szSourceDevice[MAX_PATH];       // the source device's sign(exclusive),field said local device does not exist or is empty
	BYTE				bReserved[628];					

} DEV_EVENT_RIOTERL_INFO;

// the describe of EVENT_IVS_FIGHTDETECTION's data
typedef struct tagDEV_EVENT_FIGHT_INFO 
{
	int					nChannelID;						// ChannelId
	char				szName[128];					// event name
	char                bReserved1[4];                  // byte alignment
	double				PTS;							// PTS(ms)
	NET_TIME_EX			UTC;							// the event happen time
	int					nEventID;						// event ID
	int					nObjectNum;						// have being detected object number
	DH_MSG_OBJECT		stuObjectIDs[DH_MAX_OBJECT_LIST];// have being detected object list
	DH_EVENT_FILE_INFO  stuFileInfo;                    // event file info
	BYTE                bEventAction;                   // Event action,0 means pulse event,1 means continuous event's begin,2means continuous event's end;
    BYTE                byReserved[2];                  // 
    BYTE                byImageIndex;                   // Serial number of the picture, in the same time (accurate to seconds) may have multiple images, starting from 0
	int                 nDetectRegionNum;				// detect region point 
	DH_POINT            DetectRegion[DH_MAX_DETECT_REGION_NUM];    // detect region
	
    DWORD               dwSnapFlagMask;	                // flag(by bit),see NET_RESERVED_COMMON
	int                 nSourceIndex;                   // the source device's index,-1 means data in invalid
	char                szSourceDevice[MAX_PATH];       // the source device's sign(exclusive),field said local device does not exist or is empty
	BYTE				bReserved[628];					
} DEV_EVENT_FIGHT_INFO;

// the describe of EVENT_IVS_FIREDETECTION's data
typedef struct tagDEV_EVENT_FIRE_INFO 
{
	int					nChannelID;						// ChannelId
	char				szName[128];					// event name
	char                bReserved1[4];                  // byte alignment
	double				PTS;							// PTS(ms)
	NET_TIME_EX			UTC;							// the event happen time
	int					nEventID;						// event ID
	DH_MSG_OBJECT		stuObject;						// have being detected object
	DH_EVENT_FILE_INFO  stuFileInfo;                    // event file info
	BYTE                bEventAction;                   // Event action,0 means pulse event,1 means continuous event's begin,2means continuous event's end;
    BYTE                byReserved[2];                  // 
    BYTE                byImageIndex;                   // Serial number of the picture, in the same time (accurate to seconds) may have multiple images, starting from 0
	int                 nDetectRegionNum;				// detect region point
	DH_POINT            DetectRegion[DH_MAX_DETECT_REGION_NUM];    // detect region
	
    DWORD               dwSnapFlagMask;	                // flag(by bit),see NET_RESERVED_COMMON
	int                 nSourceIndex;                   // the source device's index,-1 means data in invalid
	char                szSourceDevice[MAX_PATH];       // the source device's sign(exclusive),field said local device does not exist or is empty
	BYTE				bReserved[624];					// Reserved

} DEV_EVENT_FIRE_INFO;

// add by lihu 2011/8/8 begin
// the describe of EVENT_IVS_FIREDETECTION's data
typedef struct tagDEV_EVENT_ELECTROSPARK_INFO 
{
	int					nChannelID;						// ChannelId
	char				szName[128];					// event name
	char                bReserved1[4];                  // byte alignment
	double				PTS;							// PTS(ms)
	NET_TIME_EX			UTC;							// the event happen time
	int					nEventID;						// event ID
	DH_MSG_OBJECT		stuObject;						// have being detected object
	DH_EVENT_FILE_INFO  stuFileInfo;                    // event file info
	BYTE                bEventAction;                   // Event action,0 means pulse event,1 means continuous event's begin,2means continuous event's end;
    BYTE                byReserved[2];
    BYTE                byImageIndex;                   // Serial number of the picture, in the same time (accurate to seconds) may have multiple images, starting from 0
    DWORD               dwSnapFlagMask;	                // flag(by bit),see NET_RESERVED_COMMON
	BYTE				bReserved[972];					// reserved
	
} DEV_EVENT_ELECTROSPARK_INFO;
// add by lihu 2011/8/8 end

// the describe of EVENT_IVS_SMOKEDETECTION's data
typedef struct tagDEV_EVENT_SMOKE_INFO 
{
	int					nChannelID;						// ChannelId
	char				szName[128];					// event name
	char                bReserved1[4];                  // byte alignment
	double				PTS;							// PTS(ms)
	NET_TIME_EX			UTC;							// the event happen time
	int					nEventID;						// event ID
	DH_MSG_OBJECT		stuObject;						// have being detected object
	DH_EVENT_FILE_INFO  stuFileInfo;                    // event file info
	BYTE                bEventAction;                   // Event action,0 means pulse event,1 means continuous event's begin,2means continuous event's end;
    BYTE                byReserved[2];
    BYTE                byImageIndex;                   // Serial number of the picture, in the same time (accurate to seconds) may have multiple images, starting from 0
    DWORD               dwSnapFlagMask;	                // flag(by bit),see NET_RESERVED_COMMON
	BYTE				bReserved[972];					

} DEV_EVENT_SMOKE_INFO;

// the describe of EVENT_IVS_FLOWSTAT's data
typedef struct tagDEV_EVENT_FLOWSTAT_INFO 
{
	int					nChannelID;						// ChannelId
	char				szName[128];					// event name
	char                bReserved1[4];                  // byte alignment
	double				PTS;							// PTS(ms)
	NET_TIME_EX			UTC;							// the event happen time
	int					nEventID;						// event ID
	int					nNumberLeft;					// the number of person across from left
	int					nNumberRight;					// the number of person across from right
	int					nUpperLimit;					// upper limit
	DH_EVENT_FILE_INFO  stuFileInfo;                    // event file info
	BYTE                bEventAction;                   // Event action,0 means pulse event,1 means continuous event's begin,2means continuous event's end;
    BYTE                byReserved[2];
    BYTE                byImageIndex;                   // Serial number of the picture, in the same time (accurate to seconds) may have multiple images, starting from 0
    DWORD               dwSnapFlagMask;	                // flag(by bit),see NET_RESERVED_COMMON
	BYTE				bReserved[976];					

} DEV_EVENT_FLOWSTAT_INFO;

// the describe of EVENT_IVS_NUMBERSTAT's data
typedef struct tagDEV_EVENT_NUMBERSTAT_INFO 
{
	int					nChannelID;						// ChannelId
	char				szName[128];					// event name
	char                bReserved2[4];                  // byte alignment
	double				PTS;							// PTS(ms)
	NET_TIME_EX			UTC;							// the event happen time
	int					nEventID;						// event ID
	int					nNumber;						// the number of object which is in the area
	int					nUpperLimit;					// upper limit
	DH_EVENT_FILE_INFO  stuFileInfo;                    // event file info
	BYTE                bEventAction;                   // Event action,0 means pulse event,1 means continuous event's begin,2means continuous event's end;
    BYTE                bReserved1[2];                  // 
    BYTE                byImageIndex;                   // Serial number of the picture, in the same time (accurate to seconds) may have multiple images, starting from 0
	int                 nEnteredNumber;                 // entered object number
	int                 nExitedNumber;                  // exited object number
    DWORD               dwSnapFlagMask;	                // flag(by bit),see NET_RESERVED_COMMON
	BYTE				bReserved[964];					// Reserved

} DEV_EVENT_NUMBERSTAT_INFO;

// the describe of EVENT_IVS_CROSSFENCEDETECTION's data
typedef struct tagDEV_EVENT_CROSSFENCEDETECTION_INFO 
{
	int					nChannelID;						// ChannelId
	char				szName[128];					// event name
	char                bReserved1[4];                  // byte alignment
	double				PTS;							// PTS(ms)
	NET_TIME_EX			UTC;							// the event happen time
	int					nEventID;						// event ID
	DH_MSG_OBJECT		stuObject;						// have being detected object
	int					nUpstairsLinePointNumber;		               // Upstairs Line Point Number
	DH_POINT		    stuUpstairsLine[DH_MAX_DETECT_LINE_NUM];	   // Upstairs Line info
	int					nDownstairsLinePointNumber;		               // Downstairs Line Point Number
	DH_POINT		    stuDownstairsLine[DH_MAX_DETECT_LINE_NUM];     // Downstairs Line info
	int                 nTrackLineNum;                                 // track line point number                
	DH_POINT            TrackLine[DH_MAX_TRACK_LINE_NUM];              // track line info
	DH_EVENT_FILE_INFO  stuFileInfo;                    // event file info
	BYTE                bEventAction;                   // Event action,0 means pulse event,1 means continuous event's begin,2means continuous event's end;
	BYTE                bDirection;                     // direction, 0-left to right, 1-right to left
    BYTE                byReserved[1];
    BYTE                byImageIndex;                   // Serial number of the picture, in the same time (accurate to seconds) may have multiple images, starting from 0
    DWORD               dwSnapFlagMask;	                // flag(by bit),see NET_RESERVED_COMMON
	int                 nSourceIndex;                   // the source device's index,-1 means data in invalid
	char                szSourceDevice[MAX_PATH];       // the source device's sign(exclusive),field said local device does not exist or is empty
	BYTE				bReserved[752];				// Reserved
	
} DEV_EVENT_CROSSFENCEDETECTION_INFO;

// the describe of EVENT_IVS_INREGIONDETECTION's data
typedef struct tagDEV_EVENT_INREGIONDETECTION_INFO 
{
	int					nChannelID;						// ChannelId
	char				szName[128];					// event name
	char                bReserved1[4];                  // byte alignment
	double				PTS;							// PTS(ms)
	NET_TIME_EX			UTC;							// the event happen time
	int					nEventID;						// event ID
	int                 nObjectNum;                     // have being detected objects number
	DH_MSG_OBJECT		stuObjectIDs[DH_MAX_OBJECT_LIST];	// have being detected objects
	int                 nTrackNum;                      // track line number
	DH_POLY_POINTS      stuTrackInfo[DH_MAX_OBJECT_LIST];// track lines info
	int                 nDetectRegionNum;				// detect regions number
	DH_POINT            DetectRegion[DH_MAX_DETECT_REGION_NUM];    // detect regions
	DH_EVENT_FILE_INFO  stuFileInfo;                    // event file info
	BYTE                bEventAction;                   // Event action,0 means pulse event,1 means continuous event's begin,2means continuous event's end;
    BYTE                byReserved[2];
    BYTE                byImageIndex;                   // Serial number of the picture, in the same time (accurate to seconds) may have multiple images, starting from 0
    DWORD               dwSnapFlagMask;	                // flag(by bit),see NET_RESERVED_COMMON
	BYTE				bReserved[1016];				// Reserved
	
} DEV_EVENT_INREGIONDETECTION_INFO;

// the describe of EVENT_IVS_TAKENAWAYDETECTION's data
typedef struct tagDEV_EVENT_TAKENAWAYDETECTION_INFO
{
	int					nChannelID;						// ChannelId
	char				szName[128];					// event name
	char                bReserved1[4];                  // byte alignment
	double				PTS;							// PTS(ms)
	NET_TIME_EX			UTC;							// the event happen time
	int					nEventID;						// event ID
	DH_MSG_OBJECT		stuObject;						//  have being detected object
	int                 nDetectRegionNum;				// detect region's point number
	DH_POINT            DetectRegion[DH_MAX_DETECT_REGION_NUM];    // detect region info
	DH_EVENT_FILE_INFO  stuFileInfo;                    // event file info
	BYTE                bEventAction;                   // Event action,0 means pulse event,1 means continuous event's begin,2means continuous event's end;
    BYTE                byReserved[2];
    BYTE                byImageIndex;                   // Serial number of the picture, in the same time (accurate to seconds) may have multiple images, starting from 0
    DWORD               dwSnapFlagMask;	                // flag(by bit),see NET_RESERVED_COMMON
	int                 nSourceIndex;                   // the source device's index,-1 means data in invalid
	char                szSourceDevice[MAX_PATH];       // the source device's sign(exclusive),field said local device does not exist or is empty
	BYTE				bReserved[752];				    // Reserved
	
} DEV_EVENT_TAKENAWAYDETECTION_INFO;

// the describe of EVENT_IVS_VIDEOABNORMALDETECTION's data
typedef struct tagDEV_EVENT_VIDEOABNORMALDETECTION_INFO
{
	int					nChannelID;						// ChannelId
	char				szName[128];					// event name
	char                bReserved1[4];                  // byte alignment
	double				PTS;							// PTS(ms)
	NET_TIME_EX			UTC;							// the event happen time
	int					nEventID;						// event ID
	DH_EVENT_FILE_INFO  stuFileInfo;                    // event file info
	BYTE                bEventAction;                   // Event action,0 means pulse event,1 means continuous event's begin,2means continuous event's end;
	BYTE                bType;                          // type, 0-video lost, 1-video freeze, 2-video blind, 3-camera moving, 4-too dark, 5-too light
    BYTE                byReserved[1];
    BYTE                byImageIndex;                   // Serial number of the picture, in the same time (accurate to seconds) may have multiple images, starting from 0
    DWORD               dwSnapFlagMask;	                // flag(by bit),see NET_RESERVED_COMMON
	int                 nSourceIndex;                   // the source device's index,-1 means data in invalid
	char                szSourceDevice[MAX_PATH];       // the source device's sign(exclusive),field said local device does not exist or is empty
	BYTE				bReserved[756];					// Reserved
	
} DEV_EVENT_VIDEOABNORMALDETECTION_INFO;

// the describe of EVENT_IVS_PARKINGDETECTION's data
typedef struct tagDEV_EVENT_PARKINGDETECTION_INFO
{
	int					nChannelID;						// ChannelId
	char				szName[128];					// event name
	char                bReserved1[4];                  // byte alignment
	double				PTS;							// PTS(ms)
	NET_TIME_EX			UTC;							// the event happen time
	int					nEventID;						// event ID
	DH_MSG_OBJECT		stuObject;						// have being detected object
	int                 nDetectRegionNum;				// detect region's point number
	DH_POINT            DetectRegion[DH_MAX_DETECT_REGION_NUM];    // detect region info
	DH_EVENT_FILE_INFO  stuFileInfo;                    // event file info
	BYTE                bEventAction;                   // Event action,0 means pulse event,1 means continuous event's begin,2means continuous event's end;
    BYTE                byReserved[2];
    BYTE                byImageIndex;                   // Serial number of the picture, in the same time (accurate to seconds) may have multiple images, starting from 0
    DWORD               dwSnapFlagMask;	                // flag(by bit),see NET_RESERVED_COMMON
	int                 nSourceIndex;                   // the source device's index,-1 means data in invalid
	char                szSourceDevice[MAX_PATH];       // the source device's sign(exclusive),field said local device does not exist or is empty
	BYTE				bReserved[752];				    // Reserved 
	
} DEV_EVENT_PARKINGDETECTION_INFO;

// the describe of EVENT_IVS_ABNORMALRUNDETECTION's data
typedef struct tagDEV_EVENT_ABNORMALRUNDETECTION 
{
	int					nChannelID;						// ChannelId
	char				szName[128];					// event name
	char                bReserved1[4];                  // byte alignment
	double				PTS;							// PTS(ms)
	NET_TIME_EX			UTC;							// the event happen time
	int					nEventID;						// event ID
	DH_MSG_OBJECT		stuObject;						// have being detected object
	double              dbSpeed;                        // speed ,km/h
	double              dbTriggerSpeed;                 // triggerSpeed,km/h
	int                 nDetectRegionNum;				// detect region's point number
	DH_POINT            DetectRegion[DH_MAX_DETECT_REGION_NUM];    // detect region info
	int                 nTrackLineNum;                         // track line point number              
	DH_POINT            TrackLine[DH_MAX_TRACK_LINE_NUM];      // track line info
	DH_EVENT_FILE_INFO  stuFileInfo;                    // event file info
	BYTE                bEventAction;                   // Event action,0 means pulse event,1 means continuous event's begin,2means continuous event's end;
	BYTE                bRunType;                       // type, 0-run fast, 1-sudden speedup, 2-sudden speed-down
    BYTE                byReserved[1];
    BYTE                byImageIndex;                   // Serial number of the picture, in the same time (accurate to seconds) may have multiple images, starting from 0
    DWORD               dwSnapFlagMask;	                // flag(by bit),see NET_RESERVED_COMMON
	int                 nSourceIndex;                   // the source device's index,-1 means data in invalid
	char                szSourceDevice[MAX_PATH];       // the source device's sign(exclusive),field said local device does not exist or is empty
	BYTE				bReserved[752];				    // Reserved 
	
} DEV_EVENT_ABNORMALRUNDETECTION_INFO;

// the describe of EVENT_IVS_RETROGRADEDETECTION's data
typedef struct tagDEV_EVENT_RETROGRADEDETECTION_INFO 
{
	int					nChannelID;						// ChannelId
	char				szName[128];					// event name
	char                bReserved1[4];                  // byte alignment
	double				PTS;							// PTS(ms)
	NET_TIME_EX			UTC;							// the event happen time
	int					nEventID;						// event ID
	DH_MSG_OBJECT		stuObject;						// have being detected object
	int                 nTrackLineNum;                           // track line point number                   
	DH_POINT            TrackLine[DH_MAX_TRACK_LINE_NUM];        // track line info
	int                 nDirectionPointNum;                      // direction point number
	DH_POINT            stuDirections[DH_MAX_DETECT_LINE_NUM];   // direction info
	int                 nDetectRegionNum;				         // detect region's point number
	DH_POINT            DetectRegion[DH_MAX_DETECT_REGION_NUM];  // detect region info
	DH_EVENT_FILE_INFO  stuFileInfo;                    // event file info
	BYTE                bEventAction;                   // Event action,0 means pulse event,1 means continuous event's begin,2means continuous event's end;
    BYTE                byReserved[2];
    BYTE                byImageIndex;                   // Serial number of the picture, in the same time (accurate to seconds) may have multiple images, starting from 0
    DWORD               dwSnapFlagMask;	                // flag(by bit),see NET_RESERVED_COMMON
	int                 nSourceIndex;                   // the source device's index,-1 means data in invalid
	char                szSourceDevice[MAX_PATH];       // the source device's sign(exclusive),field said local device does not exist or is empty
	BYTE				bReserved[752];				   // Reserved  
	
} DEV_EVENT_RETROGRADEDETECTION_INFO;

// the describe of EVENT_IVS_FACERECOGNITION's data
typedef struct tagDEV_EVENT_FACERECOGNITION_INFO
{
	int					nChannelID;						// ChannelId
	char				szName[128];					// event name
    int                 nEventID;                       // event ID
	NET_TIME_EX			UTC;							// the event happen time
	DH_MSG_OBJECT		stuObject;						// have being detected object
	int                 nCandidateNum;                  // candidate number
    CANDIDATE_INFO      stuCandidates[DH_MAX_CANDIDATE_NUM]; // candidate info
	BYTE                bEventAction;                   // Event action,0 means pulse event,1 means continuous event's begin,2means continuous event's end;
    BYTE                byImageIndex;                   // Serial number of the picture, in the same time (accurate to seconds) may have multiple images, starting from 0
	BYTE				byReserved1[2];				    // reserved
	BOOL                bGlobalScenePic;                // The existence panorama
	DH_PIC_INFO         stuGlobalScenePicInfo;          // Panoramic Photos
    BYTE                bReserved[988];                 // reserved
}DEV_EVENT_FACERECOGNITION_INFO;

// Event type EVENT_IVS_DENSITYDETECTION(Population amount detect) corresponding data block description info
typedef struct tagDEV_EVENT_DENSITYDETECTTION_INFO
{
	int					nChannelID;						// Channel No.
	char				szName[128];					// Event name
	char                bReserved1[4];                  // byte alignment
	double				PTS;							// Time stamp(ms)
	NET_TIME_EX			UTC;							// Event occurred time
	int					nEventID;						// Event ID
	int					nObjectNum;						// Detected object amount
	DH_MSG_OBJECT		stuObjectIDs[DH_MAX_OBJECT_LIST];	// Detected object list
	DH_EVENT_FILE_INFO  stuFileInfo;                    // The corresponding file info of the event
	BYTE                bEventAction;                   // Event operation.0=pulse event,1=begin of the durative event,2=end of the durative event;
	BYTE                byReserved[2];                  // Reserved field
    BYTE                byImageIndex;                   // Serial number of the picture, in the same time (accurate to seconds) may have multiple images, starting from 0
	int                 nDetectRegionNum;				// Acme amount of the rule detect zone
	DH_POINT            DetectRegion[DH_MAX_DETECT_REGION_NUM];    // Rule detect zone
	
    DWORD               dwSnapFlagMask;	                // Snap flag(by bit).please refer to NET_RESERVED_COMMON
	int                 nSourceIndex;                   // the source device's index,-1 means data in invalid
	char                szSourceDevice[MAX_PATH];       // the source device's sign(exclusive),field said local device does not exist or is empty
	BYTE				bReserved[756];				    // Reserved field. For extension use.
}DEV_EVENT_DENSITYDETECTION_INFO;

// Event type  EVENT_IVS_QUEUEDETECTION(queue detection)corresponding data block description info
typedef struct tagDEV_EVENT_QUEUEDETECTION_INFO 
{
	int					nChannelID;						// channel ID
	char				szName[128];					// event name
	char                bReserved2[4];                  // byte alignment
	double				PTS;							// PTS(ms)
	NET_TIME_EX			UTC;							// the event happen time
	int					nEventID;						// event ID
	DH_EVENT_FILE_INFO  stuFileInfo;                    // event file info
	BYTE                bEventAction;                   // Event action,0 means pulse event,1 means continuous event's begin,2means continuous event's end;
	BYTE				bReserved1[2];				    // reserved
    BYTE                byImageIndex;                   // Serial number of the picture, in the same time (accurate to seconds) may have multiple images, starting from 0
	DH_POINT            stuDetectLine[2];               // detect line
	BYTE				bReserved[1016];				// reserved
}DEV_EVENT_QUEUEDETECTION_INFO;

// Event type EVENT_IVS_TRAFFICCONTROL(traffic control)corresponding data block description info
typedef struct tagDEV_EVENT_TRAFFICCONTROL_INFO 
{
	int					nChannelID;						// ChannelId
	char				szName[128];					// event name
	char                bReserved1[4];                  // byte alignment
	double				PTS;							// PTS(ms)
	NET_TIME_EX			UTC;							// the event happen time
	int					nEventID;						// event ID
	DH_MSG_OBJECT		stuObject;						// have being detected object
	DH_EVENT_FILE_INFO  stuFileInfo;                    // event file info
	BYTE                bEventAction;                   // Event action,0 means pulse event,1 means continuous event's begin,2means continuous event's end;
	BYTE                byReserved[2];
    BYTE                byImageIndex;                   // Serial number of the picture, in the same time (accurate to seconds) may have multiple images, starting from 0
    DWORD               dwSnapFlagMask;	                // flag(by bit),see NET_RESERVED_COMMON
	BYTE				bReserved[972];					// Reserved field. For extension use. 

} DEV_EVENT_TRAFFICCONTROL_INFO;

// the describe of EVENT_IVS_TRAFFICACCIDENT's data
typedef struct tagDEV_EVENT_TRAFFICACCIDENT_INFO 
{
	int					nChannelID;						// ChannelId
	char				szName[128];					// event name
	char                bReserved1[4];                  // byte alignment
	double				PTS;							// PTS(ms)
	NET_TIME_EX			UTC;							// the event happen time
	int					nEventID;						// event ID
	int					nObjectNum;						// have being detected object number
	DH_MSG_OBJECT		stuObjectIDs[DH_MAX_OBJECT_LIST];// have being detected object list
	DH_EVENT_FILE_INFO  stuFileInfo;                    // event file info
	BYTE                bEventAction;                   // Event action,0 means pulse event,1 means continuous event's begin,2means continuous event's end;
	BYTE                byReserved[2];
    BYTE                byImageIndex;                   // Serial number of the picture, in the same time (accurate to seconds) may have multiple images, starting from 0
    DWORD               dwSnapFlagMask;	                // flag(by bit),see NET_RESERVED_COMMON
	BYTE				bReserved[976];					// Reserved

} DEV_EVENT_TRAFFICACCIDENT_INFO;

#define DH_EVENT_MAX_CARD_NUM       16    // Incident reporting information includes the maximum number of cards
#define DH_EVENT_CARD_LEN           36    // Card Name Length
 
// Incidents reported to carry the card information
typedef struct tagEVENT_CARD_INFO
{
    char szCardNumber[DH_EVENT_CARD_LEN];     // Card number string
    BYTE bReserved[32];	                      // Reserved bytes, leave extended
}EVENT_CARD_INFO;

//��Event Type EVENT_IVS_TRAFFICJUNCTION (transportation card traffic junctions old rule event / video port on the old electric alarm event rules) corresponds to the description of the data block��
//��Due to historical reasons, if you want to deal with bayonet event, DEV_EVENT_TRAFFICJUNCTION_INFO and EVENT_IVS_TRAFFICGATE be processed together to prevent police and video electrical coil electric alarm occurred while the case access platform��
// ��Also EVENT_IVS_TRAFFIC_TOLLGATE only support the new bayonet events��
typedef struct tagDEV_EVENT_TRAFFICJUNCTION_INFO 
{
	int					nChannelID;						// ChannelId
	char				szName[128];					// event name
	char                bReserved1[4];                  // byte alignment
	double				PTS;							// PTS(ms)
	NET_TIME_EX			UTC;							// the event happen time
	int					nEventID;						// event ID
	DH_MSG_OBJECT		stuObject;						// have being detected object
	int					nLane;							// road number
	DWORD				dwBreakingRule;					// BreakingRule's mask,first byte: crash red light; 
	                                                    // secend byte:break the rule of driving road number; 
	                                                    // the third byte:converse; the forth byte:break rule to turn around;
														// the five byte:traffic jam; the six byte:traffic vacancy; 
														// the seven byte: Overline; defalt:trafficJunction                                                        
	NET_TIME_EX			RedLightUTC;					// the begin time of red light
	DH_EVENT_FILE_INFO  stuFileInfo;                    // event file info
	int                 nSequence;                      // snap index,such as 3,2,1,1 means the last one,0 means there has some exception and snap stop
	int                 nSpeed;                         // car's speed (km/h)
	BYTE                bEventAction;                   // Event action,0 means pulse event,1 means continuous event's begin,2means continuous event's end;
	BYTE                byDirection;                    // Intersection direction 1 - denotes the forward 2 - indicates the opposite
    BYTE                byLightState;                               // LightState��ʾ���̵�״̬:0 δ֪,1 �̵�,2 ���,3 �Ƶ�
	BYTE                byReserved;                  // reserved
    BYTE                byImageIndex;                   // Serial number of the picture, in the same time (accurate to seconds) may have multiple images, starting from 0
	DH_MSG_OBJECT       stuVehicle;                     // vehicle info
    DWORD               dwSnapFlagMask;	                // flag(by bit),see NET_RESERVED_COMMON
	DH_RESOLUTION_INFO  stuResolution;                  // picture resolution
	char                szRecordFile[DH_COMMON_STRING_128]; // Alarm corresponding original video file information
	BYTE				bReserved[344];				    // Reserved bytes, leave extended_
	DEV_EVENT_TRAFFIC_TRAFFICCAR_INFO stTrafficCar;     // Traffic vehicle info
	DWORD               dwRetCardNumber;                // Card Number
    EVENT_CARD_INFO     stuCardInfo[DH_EVENT_MAX_CARD_NUM];  // Card information
} DEV_EVENT_TRAFFICJUNCTION_INFO;


// the describe of EVENT_IVS_TRAFFICGATE's data
typedef struct tagDEV_EVENT_TRAFFICGATE_INFO 
{
	int					nChannelID;						// ChannelId
	char				szName[128];					// event name
	char                bReserved1[4];                  // byte alignment
	double				PTS;							// PTS(ms)
	NET_TIME_EX			UTC;							// the event happen time
	int					nEventID;						// event ID
	DH_MSG_OBJECT		stuObject;						// have being detected object
	int					nLane;							// road number
	int					nSpeed;							// the car's actual rate(Km/h)
	int					nSpeedUpperLimit;				// rate upper limit(km/h)
	int					nSpeedLowerLimit;				// rate lower limit(km/h) 
	DWORD				dwBreakingRule;					// BreakingRule's mask,first byte: Retrograde; 
	                                                    // second byte:Overline; the third byte:Overspeed; 
														// the forth byte:UnderSpeed;the five byte: crash red light;the six byte:passing(trafficgate)
	                                                    // the seven byte: OverYellowLine; the eight byte: WrongRunningRoute; the nine byte: YellowVehicleInRoute; default: trafficgate
	DH_EVENT_FILE_INFO  stuFileInfo;                    // event file info
	DH_MSG_OBJECT       stuVehicle;                     // vehicle info
	BYTE                szManualSnapNo[64];             // manual snap sequence string                 
	int                 nSequence;                      // snap index,such as 3,2,1,1 means the last one,0 means there has some exception and snap stop
	BYTE                bEventAction;                   // Event action,0 means pulse event,1 means continuous event's begin,2means continuous event's end;
	BYTE                byReserved[3];                  // reserved
	BYTE                szSnapFlag[16];                 // snap flag from device
	BYTE                bySnapMode;                     // snap mode,0-normal 1-globle 2-near 4-snap on the same side 8-snap on the reverse side 16-plant picture
	BYTE                byOverSpeedPercentage;          // over speed percentage
	BYTE                byUnderSpeedingPercentage;      // under speed percentage
	BYTE                byRedLightMargin;               // red light margin, s
	BYTE                byDriveDirection;               // drive direction,0-"Approach",where the car is more near,1-"Leave",means where if mor far to the car
	char                szRoadwayNo[32];                // road way number
	char                szViolationCode[16];            // violation code
	char                szViolationDesc[128];           // violation desc
	DH_RESOLUTION_INFO  stuResolution;                  // picture resolution
	char                szVehicleType[32];              // car type,"Motor", "Light-duty", "Medium", "Oversize", "Huge", "Other" 
	BYTE                byVehicleLenth;                 // car length, m
    BYTE                byLightState;                               // LightState��ʾ���̵�״̬:0 δ֪,1 �̵�,2 ���,3 �Ƶ�
	BYTE                byReserved1;                 // reserved
    BYTE                byImageIndex;                   // Serial number of the picture, in the same time (accurate to seconds) may have multiple images, starting from 0
	int                 nOverSpeedMargin;               // over speed margin, km/h 
	int                 nUnderSpeedMargin;              // under speed margin, km/h 
	char                szDrivingDirection[3][DH_MAX_DRIVINGDIRECTION]; //
                                                      	// "DrivingDirection" : ["Approach", "Shanghai", "Hangzhou"],
	                                                  	// "Approach" means driving direction,where the car is more near;"Leave"-means where if mor far to the car
	                                                  	// the second and third param means the location of the driving direction
	char                szMachineName[256];             // machine name
	char                szMachineAddress[256];          // machine address
	char                szMachineGroup[256];            // machine group
    DWORD               dwSnapFlagMask;	                // flag(by bit),see NET_RESERVED_COMMON
	DH_SIG_CARWAY_INFO_EX stuSigInfo;                   // The vehicle detector generates the snap signal redundancy info
	char                szFilePath[MAX_PATH];           // File path
	NET_TIME_EX			RedLightUTC;					// the begin time of red light
	char              * szDeviceAddress;                // device address,OSD superimposed onto the image,from TrafficSnapshot.DeviceAddress,'\0'means end.
	float               fActualShutter;                 // Current picture exposure time, in milliseconds
	BYTE                byActualGain;                   // Current picture gain, ranging from 0 to 1000
    BYTE                bReserve[2];                    // Reserved bytes, byte alignment
    BYTE                bRetCardNumber;                 // Card Number
    EVENT_CARD_INFO     stuCardInfo[DH_EVENT_MAX_CARD_NUM];// Card information
	BYTE				bReserved[2568];				// Reserved bytes, leave extended
} DEV_EVENT_TRAFFICGATE_INFO;

//the describe of EVENT_TRAFFICSNAPSHOT's data
typedef struct tagDEV_EVENT_TRAFFICSNAPSHOT_INFO 
{
	int					nChannelID;						// ChannelId
	char				szName[128];					// event name
	char                bReserved1[4];                  // byte alignment
	double				PTS;							// PTS(ms)
	NET_TIME_EX			UTC;							// the event happen time
	int					nEventID;						// event ID
	BYTE                bReserv[3];                       // reserved 
	BYTE                bCarWayCount;                     // car way number being snapshotting
	DH_CARWAY_INFO      stuCarWayInfo[DH_MAX_CARWAY_NUM]; // car way info being snapshotting
	DH_EVENT_FILE_INFO  stuFileInfo;                    // event file info
	BYTE                bEventAction;                   // Event action,0 means pulse event,1 means continuous event's begin,2means continuous event's end;
    BYTE                byReserved[2];
    BYTE                byImageIndex;                   // Serial number of the picture, in the same time (accurate to seconds) may have multiple images, starting from 0
    DWORD               dwSnapFlagMask;	                // flag(by bit),see NET_RESERVED_COMMON
	BYTE				bReserved[344];		    		// Reserved 
} DEV_EVENT_TRAFFICSNAPSHOT_INFO;

//the describe of EVENT_IVS_TRAFFIC_RUNREDLIGHT's data
typedef struct tagDEV_EVENT_TRAFFIC_RUNREDLIGHT_INFO
{
	int					nChannelID;						  // channel ID
	char				szName[128];					  // event name
	char                bReserved1[4];                    // byte alignment
	double				PTS;							  // PTS(ms)
	NET_TIME_EX			UTC;				              // the event happen time
	int					nEventID;			              // event ID
	int					nLane;				              // Corresponding Lane number
	DH_MSG_OBJECT		stuObject;	              	      // have being detected object
	DH_MSG_OBJECT       stuVehicle;                       // vehicle info
	DH_EVENT_FILE_INFO  stuFileInfo;                      // event file info
	int					nLightState;	              	  // state of traffic light 0:unknown 1:green 2:red 3:yellow
	int					nSpeed;			              	  // speed,km/h
	int                 nSequence;                        // snap index,such as 3,2,1,1 means the last one,0 means there has some exception and snap stop
	BYTE                bEventAction;					  // Event action,0 means pulse event,1 means continuous event's begin,2means continuous event's end;
    BYTE                byReserved[2];
    BYTE                byImageIndex;                   // Serial number of the picture, in the same time (accurate to seconds) may have multiple images, starting from 0
    DWORD               dwSnapFlagMask;					  // flag(by bit),see NET_RESERVED_COMMON
	NET_TIME_EX         stRedLightUTC;                    // time of red light starting
	DH_RESOLUTION_INFO  stuResolution;                    // picture resolution
	BYTE                byRedLightMargin;               // red light margin, s
	BYTE				bReserved[975];	                  // Reserved
	DEV_EVENT_TRAFFIC_TRAFFICCAR_INFO stTrafficCar;       // Traffic vehicle info
} DEV_EVENT_TRAFFIC_RUNREDLIGHT_INFO;

//the describe of EVENT_IVS_TRAFFIC_OVERLINE's data
typedef struct tagDEV_EVENT_TRAFFIC_OVERLINE_INFO
{
	int					nChannelID;			// channel ID
	char				szName[128];		// event name
	char                bReserved1[4];      // byte alignment
	double				PTS;				// PTS(ms)
	NET_TIME_EX			UTC;				// the event happen time
	int					nEventID;			// event ID
	int					nLane;				// Corresponding Lane number
	DH_MSG_OBJECT		stuObject;			// have being detected object
	DH_MSG_OBJECT       stuVehicle;         // vehicle info
	DH_EVENT_FILE_INFO  stuFileInfo;        // event file info
	int                 nSequence;          // snap index,such as 3,2,1,1 means the last one,0 means there has some exception and snap stop
	int                 nSpeed;             // speed,km/h
	BYTE                bEventAction;		// Event action,0 means pulse event,1 means continuous event's begin,2means continuous event's end;
    BYTE                byReserved[2];
    BYTE                byImageIndex;                   // Serial number of the picture, in the same time (accurate to seconds) may have multiple images, starting from 0
    DWORD               dwSnapFlagMask;	    // flag(by bit),see NET_RESERVED_COMMON
	DH_RESOLUTION_INFO  stuResolution;      // picture resolution	
	BYTE				bReserved[1008];	// Reserved 
	DEV_EVENT_TRAFFIC_TRAFFICCAR_INFO stTrafficCar;       // Traffic vehicle info
} DEV_EVENT_TRAFFIC_OVERLINE_INFO;


//the describe of EVENT_IVS_TRAFFIC_RETROGRADE's data
typedef struct tagDEV_EVENT_TRAFFIC_RETROGRADE_INFO
{
	int					nChannelID;			// channel ID
	char				szName[128];		// event name
	char                bReserved1[4];      // byte alignment
	double				PTS;				// PTS(ms)
	NET_TIME_EX			UTC;				// the event happen time
	int					nEventID;			// event ID
	int					nLane;				// Corresponding Lane number
	DH_MSG_OBJECT		stuObject;			// have being detected object
	DH_MSG_OBJECT       stuVehicle;         // vehicle info
	DH_EVENT_FILE_INFO  stuFileInfo;        // event file info
	int                 nSequence;          // snap index: such as 3,2,1,1 means the last one,0 means there has some exception and snap stop
	int                 nSpeed;             // speed, km/h
	BYTE                bEventAction;		// Event action,0 means pulse event,1 means continuous event's begin,2means continuous event's end;
    BYTE                byReserved[2];
    BYTE                byImageIndex;        // Serial number of the picture, in the same time (accurate to seconds) may have multiple images, starting from 0
    DWORD               dwSnapFlagMask;	     // flag(by bit),see NET_RESERVED_COMMON
	DH_RESOLUTION_INFO  stuResolution;       // picture resolution
	BOOL                bIsExistAlarmRecord;            // a corresponding alarm recording; false: no corresponding alarm recording
	DWORD               dwAlarmRecordSize;              // Video size
	char                szAlarmRecordPath[DH_COMMON_STRING_256]; // Video Path
	BYTE				bReserved[656];	// Reserved bytes
	DEV_EVENT_TRAFFIC_TRAFFICCAR_INFO stTrafficCar;    // Traffic vehicle info
	int                 nDetectNum;				  // Acme amount of the rule detect zone
	DH_POINT            DetectRegion[DH_MAX_DETECT_REGION_NUM];    // Rule detect zone 
} DEV_EVENT_TRAFFIC_RETROGRADE_INFO;

//the describe of EVENT_IVS_TRAFFIC_TURNLEFT's data
typedef struct tagDEV_EVENT_TRAFFIC_TURNLEFT_INFO
{
	int					nChannelID;			// channel ID
	char				szName[128];		// event name
	char                bReserved1[4];      // byte alignment
	double				PTS;				// PTS(ms)
	NET_TIME_EX			UTC;				// the event happen time
	int					nEventID;			// event ID
	int					nLane;				// Corresponding Lane number
	DH_MSG_OBJECT		stuObject;			// have being detected object
	DH_MSG_OBJECT       stuVehicle;         // vehicle info
	DH_EVENT_FILE_INFO  stuFileInfo;        // event file info
	int                 nSequence;          // snap index: such as 3,2,1,1 means the last one,0 means there has some exception and snap stop
	int                 nSpeed;             // speed,km/h
	BYTE                bEventAction;		// Event action,0 means pulse event,1 means continuous event's begin,2means continuous event's end;
    BYTE                byReserved[2];
    BYTE                byImageIndex;                   // Serial number of the picture, in the same time (accurate to seconds) may have multiple images, starting from 0
    DWORD               dwSnapFlagMask;	    // flag(by bit),see NET_RESERVED_COMMON
	DH_RESOLUTION_INFO  stuResolution;      // picture resolution

	BYTE				bReserved[1008];	// Reserved 
	DEV_EVENT_TRAFFIC_TRAFFICCAR_INFO stTrafficCar;     // Traffic vehicle info
} DEV_EVENT_TRAFFIC_TURNLEFT_INFO;


//the describe of EVENT_IVS_TRAFFIC_TURNRIGHT's data
typedef struct tagDEV_EVENT_TRAFFIC_TURNRIGHT_INFO
{
	int					nChannelID;			// channel ID
	char				szName[128];		// event name
	char                bReserved1[4];      // byte alignment
	double				PTS;				// PTS(ms)
	NET_TIME_EX			UTC;				// the event happen time
	int					nEventID;			// event ID
	int					nLane;				// Corresponding Lane number
	DH_MSG_OBJECT		stuObject;			// have being detected object
	DH_MSG_OBJECT       stuVehicle;         // vehicle info
	DH_EVENT_FILE_INFO  stuFileInfo;        // event file info
	int                 nSequence;          // snap index: such as 3,2,1,1 means the last one,0 means there has some exception and snap stop
	int                 nSpeed;             // speed,km/h
	BYTE                bEventAction;		// Event action,0 means pulse event,1 means continuous event's begin,2means continuous event's end;
    BYTE                byReserved[2];
    BYTE                byImageIndex;                   // Serial number of the picture, in the same time (accurate to seconds) may have multiple images, starting from 0
    DWORD               dwSnapFlagMask;	    // flag(by bit),see NET_RESERVED_COMMON
	DH_RESOLUTION_INFO  stuResolution;      // picture resolution

	BYTE				bReserved[1008];	// Reserved 
	DEV_EVENT_TRAFFIC_TRAFFICCAR_INFO stTrafficCar;     // Traffic vehicle info
} DEV_EVENT_TRAFFIC_TURNRIGHT_INFO;

//the describe of EVENT_IVS_TRAFFIC_UTURN's data
typedef struct tagDEV_EVENT_TRAFFIC_UTURN_INFO 
{
	int					nChannelID;						  // channel ID
	char				szName[128];					  // event name
	char                bReserved1[4];                    // byte alignment
	double				PTS;							  // PTS(ms)
	NET_TIME_EX			UTC;						   	  // the event happen time
	int					nEventID;					      // event ID
	int					nLane;							  // Corresponding Lane number
	DH_MSG_OBJECT		stuObject;						  // have being detected object
	DH_MSG_OBJECT       stuVehicle;                       // vehicle info
	DH_EVENT_FILE_INFO  stuFileInfo;					  // event file info
	int                 nSequence;                        // snap index: such as 3,2,1,1 means the last one,0 means there has some exception and snap stop
	int                 nSpeed;             			  // speed,km/h
	BYTE                bEventAction;		              // Event action,0 means pulse event,1 means continuous event's begin,2means continuous event's end;
    BYTE                byReserved[2];
    BYTE                byImageIndex;                   // Serial number of the picture, in the same time (accurate to seconds) may have multiple images, starting from 0
    DWORD               dwSnapFlagMask;	                  // flag(by bit),see NET_RESERVED_COMMON
	DH_RESOLUTION_INFO  stuResolution;                    // picture resolution
	
	BYTE                bReserved[1008];				  // Reserved 
	DEV_EVENT_TRAFFIC_TRAFFICCAR_INFO stTrafficCar;       // Traffic vehicle info
}DEV_EVENT_TRAFFIC_UTURN_INFO;

//the describe of EVENT_IVS_TRAFFIC_OVERSPEED's data
typedef struct tagDEV_EVENT_TRAFFIC_OVERSPEED_INFO 
{
	int					nChannelID;						  // channel ID
	char				szName[128];					  // event name
	char                bReserved1[4];                    // byte alignment
	double				PTS;							  // PTS(ms)
	NET_TIME_EX			UTC;						   	  // the event happen time
	int					nEventID;					      // event ID
	int					nLane;							  // Corresponding Lane number
	DH_MSG_OBJECT		stuObject;						  // have being detected object
	DH_MSG_OBJECT       stuVehicle;                       // vehicle info
	DH_EVENT_FILE_INFO  stuFileInfo;					  // event file info
    int                 nSpeed;                           // vehicle speed Unit:Km/h
	int					nSpeedUpperLimit;			      // Speed Up limit Unit:km/h
	int					nSpeedLowerLimit;				  // Speed Low limit Unit:km/h 
	int                 nSequence;                        // snap index:such as 3,2,1,1 means the last one,0 means there has some exception and snap stop
	BYTE                bEventAction;		              // Event action,0 means pulse event,1 means continuous event's begin,2 means continuous event's end;
    BYTE                byReserved[2];
    BYTE                byImageIndex;                   // Serial number of the picture, in the same time (accurate to seconds) may have multiple images, starting from 0
    DWORD               dwSnapFlagMask;	                  // flag(by bit),see NET_RESERVED_COMMON
	DH_RESOLUTION_INFO  stuResolution;                    // picture resolution

	BYTE                bReserved[1008];				  // Reserved 
	DEV_EVENT_TRAFFIC_TRAFFICCAR_INFO stTrafficCar;       // Traffic vehicle info
}DEV_EVENT_TRAFFIC_OVERSPEED_INFO;

//the describe of EVENT_IVS_TRAFFIC_UNDERSPEED's data
typedef struct tagDEV_EVENT_TRAFFIC_UNDERSPEED_INFO 
{
	int					nChannelID;						  // channel ID
	char				szName[128];					  // event name
	char                bReserved2[4];                    // byte alignment
	double				PTS;							  // PTS(ms)
	NET_TIME_EX			UTC;						   	  // the event happen time
	int					nEventID;					      // event ID
	int					nLane;							  // Corresponding Lane number
	DH_MSG_OBJECT		stuObject;						  // have being detected object
	DH_MSG_OBJECT       stuVehicle;                       // vehicle info
	DH_EVENT_FILE_INFO  stuFileInfo;					  // event file info
    int                 nSpeed;                           // vehicle speed Unit:Km/h
	int					nSpeedUpperLimit;			      // Speed Up limit Unit:km/h
	int					nSpeedLowerLimit;				  // Speed Low limit Unit:km/h 
	int                 nSequence;                        // snap index: such as 3,2,1,1 means the last one,0 means there has some exception and snap stop
	BYTE                bEventAction;		              // Event action,0 means pulse event,1 means continuous event's begin,2means continuous event's end;
    BYTE                bReserved1[2];                    // reserved
    BYTE                byImageIndex;                   // Serial number of the picture, in the same time (accurate to seconds) may have multiple images, starting from 0
	int                 nUnderSpeedingPercentage;         // under speed percentage
    DWORD               dwSnapFlagMask;	                  // flag(by bit),see NET_RESERVED_COMMON
	DH_RESOLUTION_INFO  stuResolution;                    // picture resolution
	
	BYTE                bReserved[1004];				  // Reserved 
	DEV_EVENT_TRAFFIC_TRAFFICCAR_INFO stTrafficCar;       // Traffic vehicle info
}DEV_EVENT_TRAFFIC_UNDERSPEED_INFO;

//the describe of EVENT_IVS_FACEDETECT's data
typedef struct tagDEV_EVENT_FACEDETECT_INFO 
{
	int					nChannelID;						  // channel ID
	char				szName[128];					  // event name
	char                bReserved1[4];                    // byte alignment
	double				PTS;							  // PTS(ms)
	NET_TIME_EX			UTC;						   	  // the event happen time
	int					nEventID;					      // event ID
	DH_MSG_OBJECT		stuObject;						  // have being detected object
	DH_EVENT_FILE_INFO  stuFileInfo;					  // event file info
	BYTE                bEventAction;                     // Event action: 0 means pulse event,1 means continuous event's begin,2means continuous event's end;
	BYTE                reserved[2];                      // reserved
    BYTE                byImageIndex;                   // Serial number of the picture, in the same time (accurate to seconds) may have multiple images, starting from 0
	int                 nDetectRegionNum;				  // detect region point number
	DH_POINT            DetectRegion[DH_MAX_DETECT_REGION_NUM];    // detect region
    DWORD               dwSnapFlagMask;	                  // flag(by bit),see NET_RESERVED_COMMON
	BYTE				bReserved[936];					  // Reserved 
} DEV_EVENT_FACEDETECT_INFO;

// the describe of EVENT_IVS_TRAFFICJAM's data
typedef struct tagDEV_EVENT_TRAFFICJAM_INFO 
{
	int					nChannelID;						// channel ID
	char				szName[128];					// event name
	char                bReserved1[4];                  // byte alignment
	double				PTS;							// PTS(ms)
	NET_TIME_EX			UTC;							// the event happen time
	int					nEventID;						// event ID
	int					nLane;							// Corresponding Lane number
	DH_EVENT_FILE_INFO  stuFileInfo;                    // event file info              
	BYTE                bEventAction;                   // Event action,0 means pulse event,1 means continuous event's begin,2means continuous event's end;
	BYTE                bJamLenght;                     // Mean congestion length (percentage of total lane length) 0-100
	BYTE                reserved;                    // reserved
    BYTE                byImageIndex;                   // Serial number of the picture, in the same time (accurate to seconds) may have multiple images, starting from 0
	NET_TIME_EX         stuStartJamTime;				// the time of starting jam 
	int                 nSequence;                      // snap index: such as 3,2,1,1 means the last one,0 means there has some exception and snap stop(this param work when bEventAction=2) 
	int                 nAlarmIntervalTime;             // interval time of alarm(s).(this is a continuous event,if the interval time of recieving next event go beyond this parm, we can judge that this event is over with exception)
    DWORD               dwSnapFlagMask;	                // flag(by bit),see NET_RESERVED_COMMON
	DH_RESOLUTION_INFO  stuResolution;                  // picture resolution
	BYTE				bReserved[1012];				// Reserved 
	DEV_EVENT_TRAFFIC_TRAFFICCAR_INFO stTrafficCar;     // Traffic vehicle info	
} DEV_EVENT_TRAFFICJAM_INFO;

// the describe of EVENT_IVS_TRAFFIC_PARKING's data
typedef struct tagDEV_EVENT_TRAFFIC_PARKING_INFO 
{
	int					nChannelID;						// channel ID
	char				szName[128];					// event name
	char                bReserved1[4];                  // byte alignment
	double				PTS;							// PTS(ms)
	NET_TIME_EX			UTC;							// the event happen time
	int					nEventID;						// event ID
	DH_MSG_OBJECT		stuObject;						// have being detected object
	DH_MSG_OBJECT       stuVehicle;                     // vehicle info
	int					nLane;							// Corresponding Lane number
	DH_EVENT_FILE_INFO  stuFileInfo;                    // event file info               
	BYTE                bEventAction;                   // Event action,0 means pulse event,1 means continuous event's begin,2means continuous event's end;
	BYTE                reserved[2];                    // Reserved bytes
    BYTE                byImageIndex;                   // Serial number of the picture, in the same time (accurate to seconds) may have multiple images, starting from 0
	NET_TIME_EX         stuStartParkingTime;            // the time of starting parking
	int                 nSequence;                      // snap index: such as 3,2,1,1 means the last one,0 means there has some exception and snap stop(this param work when bEventAction=2) 
	int                 nAlarmIntervalTime;             // interval time of alarm(s) (this is a continuous event,if the interval time of recieving next event go beyond this parm, we can judge that this event is over with exception)
	int                 nParkingAllowedTime;            // the time of legal parking
	int                 nDetectRegionNum;				// detect region point number
	DH_POINT            DetectRegion[DH_MAX_DETECT_REGION_NUM];    // detect region point number
    DWORD               dwSnapFlagMask;	                // flag(by bit),see NET_RESERVED_COMMON
	DH_RESOLUTION_INFO  stuResolution;                  // picture resolution
	BOOL                bIsExistAlarmRecord;            // true:corresponding alarm recording; false: no corresponding alarm recording
	DWORD               dwAlarmRecordSize;              // Video size
	char                szAlarmRecordPath[DH_COMMON_STRING_256]; // Video Path
	BYTE				bReserved[660];				    // Reserved 
	DEV_EVENT_TRAFFIC_TRAFFICCAR_INFO stTrafficCar;     // Traffic vehicle info
} DEV_EVENT_TRAFFIC_PARKING_INFO;

//the describe of EVENT_IVS_TRAFFIC_WRONGROUTE's data
typedef struct tagDEV_EVENT_TRAFFIC_WRONGROUTE_INFO 
{
	int					nChannelID;						// channel ID
	char				szName[128];					// event name
	char                bReserved1[4];                  // byte alignment
	double				PTS;							// PTS(ms)
	NET_TIME_EX			UTC;							// the event happen time
	int					nEventID;						// event ID
	DH_MSG_OBJECT		stuObject;						// have being detected object
	DH_MSG_OBJECT       stuVehicle;                     // vehicle info
	int					nLane;							// Corresponding Lane number
	DH_EVENT_FILE_INFO  stuFileInfo;                    // event file info               
	BYTE                bEventAction;                   // Event action,0 means pulse event,1 means continuous event's begin,2means continuous event's end;
    BYTE                byReserved[2];           
    BYTE                byImageIndex;                   // Serial number of the picture, in the same time (accurate to seconds) may have multiple images, starting from 0
	int                 nSpeed;                         // speed,km/h
    DWORD               dwSnapFlagMask;	                // flag(by bit),see NET_RESERVED_COMMON
	DH_RESOLUTION_INFO  stuResolution;                  // picture resolution
	BYTE				bReserved[1012];				// Reserved 
	DEV_EVENT_TRAFFIC_TRAFFICCAR_INFO stTrafficCar;     // Traffic vehicle info
	
} DEV_EVENT_TRAFFIC_WRONGROUTE_INFO;

//the describe of EVENT_IVS_TRAFFIC_CROSSLANE's data
typedef struct tagDEV_EVENT_TRAFFIC_CROSSLANE_INFO 
{
	int					nChannelID;						// channel ID
	char				szName[128];					// event name
	char                bReserved1[4];                  // byte alignment
	double				PTS;							// PTS(ms)
	NET_TIME_EX			UTC;							// the event happen time
	int					nEventID;						// event ID
	DH_MSG_OBJECT		stuObject;						// have being detected object
	DH_MSG_OBJECT       stuVehicle;                     // vehicle info
	int					nLane;							// Corresponding Lane number
	DH_EVENT_FILE_INFO  stuFileInfo;                    // event file info                 
	BYTE                bEventAction;                   // Event action,0 means pulse event,1 means continuous event's begin,2means continuous event's end;
    BYTE                byReserved[2];       
    BYTE                byImageIndex;                   // Serial number of the picture, in the same time (accurate to seconds) may have multiple images, starting from 0
	int                 nSpeed;                         // speed,km/h
    DWORD               dwSnapFlagMask;	                // flag(by bit),see NET_RESERVED_COMMON
	DH_RESOLUTION_INFO  stuResolution;                  // picture resolution
	BYTE				bReserved[1008];				// Reserved 
	
} DEV_EVENT_TRAFFIC_CROSSLANE_INFO;

//the describe of EVENT_IVS_TRAFFIC_OVERYELLOWLINE's data
typedef struct tagDEV_EVENT_TRAFFIC_OVERYELLOWLINE_INFO 
{
	int					nChannelID;						// channel ID
	char				szName[128];					// event name
	char                bReserved1[4];                  // byte alignment
	double				PTS;							// PTS(ms)
	NET_TIME_EX			UTC;							// the event happen time
	int					nEventID;						// event ID
	DH_MSG_OBJECT		stuObject;						// have being detected object
	DH_MSG_OBJECT       stuVehicle;                     // vehicle info
	int					nLane;							// Corresponding Lane number
	DH_EVENT_FILE_INFO  stuFileInfo;                    // event file info                 
	BYTE                bEventAction;                   // Event action,0 means pulse event,1 means continuous event's begin,2means continuous event's end;
    BYTE                byReserved[2];  
    BYTE                byImageIndex;                   // Serial number of the picture, in the same time (accurate to seconds) may have multiple images, starting from 0
	int                 nSpeed;                         // speed,km/h
    DWORD               dwSnapFlagMask;	                // flag(by bit),see NET_RESERVED_COMMON
	DH_RESOLUTION_INFO  stuResolution;                  // picture resolution
	BOOL                bIsExistAlarmRecord;            // true:corresponding alarm recording; false: no corresponding alarm recording
	DWORD               dwAlarmRecordSize;              // Video size
	char                szAlarmRecordPath[DH_COMMON_STRING_256]; // Video Path
	BYTE				bReserved[664];				// Reserved 
	DEV_EVENT_TRAFFIC_TRAFFICCAR_INFO stTrafficCar;     // Traffic vehicle info
	
	int                 nDetectNum;				                   // Acme amount of the rule detect zone 
	DH_POINT            DetectRegion[DH_MAX_DETECT_REGION_NUM];    // Rule detect zone 	
} DEV_EVENT_TRAFFIC_OVERYELLOWLINE_INFO;

//the describe of EVENT_IVS_TRAFFIC_DRIVINGONSHOULDER's data
typedef struct tagDEV_EVENT_TRAFFIC_DRIVINGONSHOULDER_INFO
{
	int					nChannelID;						// channel ID
	char				szName[128];					// event name
	char                bReserved1[4];                  // byte alignment
	double				PTS;							// PTS(ms)
	NET_TIME_EX			UTC;							// the event happen time
	int					nEventID;						// event ID
	DH_MSG_OBJECT		stuObject;						// have being detected object
	DH_MSG_OBJECT       stuVehicle;                     // vehicle info
	int					nLane;							// Corresponding Lane number
	DH_EVENT_FILE_INFO  stuFileInfo;                    // event file info                                
	BYTE                bEventAction;                   // Event action,0 means pulse event,1 means continuous event's begin,2means continuous event's end;
    BYTE                byReserved[2];      
    BYTE                byImageIndex;                   // Serial number of the picture, in the same time (accurate to seconds) may have multiple images, starting from 0
	int                 nSpeed;                         // speed,km/h
    DWORD               dwSnapFlagMask;	                // flag(by bit),see NET_RESERVED_COMMON
	DH_RESOLUTION_INFO  stuResolution;                  // picture resolution
	BYTE				bReserved[1008];				//  
	
} DEV_EVENT_TRAFFIC_DRIVINGONSHOULDER_INFO;

//the describe of EVENT_IVS_TRAFFIC_YELLOWPLATEINLANE's data
typedef struct tagDEV_EVENT_TRAFFIC_YELLOWPLATEINLANE_INFO
{
	int					nChannelID;						// channel ID
	char				szName[128];					// event name
	char                bReserved1[4];                  // byte alignment
	double				PTS;							// PTS(ms)
	NET_TIME_EX			UTC;							// the event happen time
	int					nEventID;						// event ID
	DH_MSG_OBJECT		stuObject;						// have being detected object
	DH_MSG_OBJECT       stuVehicle;                     // vehicle info
	int					nLane;							// Corresponding Lane number
	DH_EVENT_FILE_INFO  stuFileInfo;                    // event file info                                               
	BYTE                bEventAction;                   // Event action,0 means pulse event,1 means continuous event's begin,2means continuous event's end;
    BYTE                byReserved[2];     
    BYTE                byImageIndex;                   // Serial number of the picture, in the same time (accurate to seconds) may have multiple images, starting from 0
	int                 nSpeed;                         // speed,km/h
    DWORD               dwSnapFlagMask;	                // flag(by bit),see NET_RESERVED_COMMON
	DH_RESOLUTION_INFO  stuResolution;                  // picture resolution
	BYTE				bReserved[1016];				// Reserved 
	DEV_EVENT_TRAFFIC_TRAFFICCAR_INFO stTrafficCar;     // Traffic vehicle info
	
} DEV_EVENT_TRAFFIC_YELLOWPLATEINLANE_INFO;


//the describe of EVENT_IVS_TRAFFIC_NOPASSING's data
typedef struct tagDEV_EVENT_TRAFFIC_NOPASSING_INFO
{
	int					nChannelID;						// channel ID
	char				szName[DH_EVENT_NAME_LEN];					// event name
    int                 nTriggerType;                   // Trigger Type, 0 vehicle inspection device, 1 radar, 2 video
	DWORD				PTS;							// PTS(ms)
	NET_TIME_EX			UTC;							// the event happen time
	int					nEventID;						// event ID
	int                 UTCMS;                          // 
	int                 nMark;                          // 
	int                 nSequence;                      // snap index: such as 3,2,1,1 means the last one,0 means there has some exception and snap stop
	DH_EVENT_FILE_INFO  stuFileInfo;                    // event file info  
	BYTE                bEventAction;                   // Event action,0 means pulse event,1 means continuous event's begin,2means continuous event's end;
	DEV_EVENT_TRAFFIC_TRAFFICCAR_INFO   stTrafficCar;   // TrafficCar info
    DWORD               dwSnapFlagMask;	                // flag(by bit),see NET_RESERVED_COMMON
	DH_RESOLUTION_INFO  stuResolution;                  // picture resolution
    BYTE                byImageIndex;                   // Serial number of the picture, in the same time (accurate to seconds) may have multiple images, starting from 0
    BYTE                byReserved1[3];
    int                 nLane;                          // Corresponding lane number
    DH_MSG_OBJECT		stuObject;						// Object to be detected
    DH_MSG_OBJECT       stuVehicle;                     // car body information 
    int                 nFrameSequence;                 // Video analysis frame number
    int                 nSource;                        // Data source address of the video analysis
    BYTE				byReserved[1024];	            // Reserved bytes
}DEV_EVENT_TRAFFIC_NOPASSING_INFO;
typedef struct tagDH_TRAFFICFLOWSTAT
{
	char				szMachineAddress[256];			// same as DEV_EVENT_TRAFFIC_TRAFFICCAR_INFO.MachineGroup
	char				szMachineName[256];				// same as DEV_EVENT_TRAFFIC_TRAFFICCAR_INFO.MachineName
	char				szDrivingDirection[3][32];		// DrivingDirection "Approach" means driving direction,where the car is more near;"Leave"-means where if mor far to the car,the scend and third param means the location of the driving direction
	int					nLane;							// lane number
	NET_TIME_EX			UTC;							// Statistic time
	int					nPeriod;						// Statistic period, m
	int					nVehicles;						// passed vehicle number
	float				fAverageSpeed;					// average speed, km/h
	float				fAverageLength;					// average length, m
	float				fTimeOccupyRatio;				// time occupy ratio,
	float				fSpaceOccupyRatio;				// space occupy ratio,
	float				fSpaceHeadway;					// space between two cars,m
	float				fTimeHeadway;					// time between two cars, s
	float				fDensity;						// car density, every km
	int					nOverSpeedVehicles;				// over speed vehicle number
	int					nUnderSpeedVehicles;			// under speed vehicle number
	int					nLargeVehicles;					// big car number  
	int					nMediumVehicles;				// mid car number
	int					nSmallVehicles;					// small car number
	int					nMotoVehicles;					// moto car number
	int					nLongVehicles;					// long vehicle number
}DH_TRAFFICFLOWSTAT;
// the describe of EVENT_IVS_TRAFFIC_FLOWSTATE's data
typedef struct tagDEV_EVENT_TRAFFIC_FLOWSTAT_INFO
{
	char				szName[128];					// name
	double				PTS;							// time stamp(ms)
	NET_TIME_EX			UTC;							// occurrence time
	int					nEventID;						// event id
	int					nLaneCnt;						// channel number
	DH_TRAFFICFLOWSTAT	stTrafficFlowStats[DH_MAX_LANE_NUM];//traffic flow state info
	char				Reserved[4];					// byte alignment
}DEV_EVENT_TRAFFIC_FLOWSTAT_INFO;

//the describe of EVENT_IVS_TRAFFIC_MANUALSNAP's data
typedef struct tagDEV_EVENT_TRAFFIC_MANUALSNAP_INFO
{
	int					nChannelID;						// channel ID
	char				szName[128];					// event name
	char                bReserved1[4];                  // byte alignment
	double				PTS;							// PTS(ms)
	NET_TIME_EX			UTC;							// the event happen time
	int					nEventID;						// event ID
	int					nLane;							// lane number
	BYTE                szManualSnapNo[64];             // manual snap number 
	DH_MSG_OBJECT		stuObject;						// have being detected object
	DH_MSG_OBJECT       stuVehicle;                     // have being detected vehicle
	DEV_EVENT_TRAFFIC_TRAFFICCAR_INFO   stTrafficCar;   // TrafficCar info
	DH_EVENT_FILE_INFO  stuFileInfo;                    // event file info
	BYTE                bEventAction;                   // Event action,0 means pulse event,1 means continuous event's begin,2means continuous event's end;
    BYTE                byReserved[2];
    BYTE                byImageIndex;                   // Serial number of the picture, in the same time (accurate to seconds) may have multiple images, starting from 0
    DWORD               dwSnapFlagMask;	                // flag(by bit),see NET_RESERVED_COMMON
	DH_RESOLUTION_INFO  stuResolution;                  // picture resolution
	BYTE				bReserved[1016];				// 
}DEV_EVENT_TRAFFIC_MANUALSNAP_INFO;

//the describe of EVENT_IVS_TRAFFIC_STAY's data
typedef struct tagDEV_EVENT_TRAFFIC_STAY_INFO
{
	int					nChannelID;						// channel id
	char				szName[128];					// event name
	char                bReserved1[4];                  // byte alignment
	double				PTS;							// PTS(ms)
	NET_TIME_EX			UTC;							// the event happen time
	int					nEventID;						// event ID
	DH_MSG_OBJECT		stuObject;						// object info
	DH_MSG_OBJECT       stuVehicle;                     // vehicle info
	int					nLane;							// lane number
	int					nSequence;						// snap index
	DEV_EVENT_TRAFFIC_TRAFFICCAR_INFO   stTrafficCar;   // TrafficCar info
	DH_EVENT_FILE_INFO  stuFileInfo;                    // event file info  
	BYTE                bEventAction; 					// Event action,0 means pulse event,1 means continuous event's begin,2means continuous event's end;
    BYTE                byReserved0[2];
    BYTE                byImageIndex;                   // Serial number of the picture, in the same time (accurate to seconds) may have multiple images, starting from 0
    DWORD               dwSnapFlagMask;	                // flag(by bit),see NET_RESERVED_COMMON
	DH_RESOLUTION_INFO  stuResolution;                  // picture resolution
	BYTE                byReserved[1012];           
}DEV_EVENT_TRAFFIC_STAY_INFO;

//the describe of EVENT_IVS_TRAFFIC_VEHICLEINROUTE's data
typedef struct tagDEV_EVENT_TRAFFIC_VEHICLEINROUTE_INFO
{
	int					nChannelID;						// channel id
	char				szName[128];					// event name
	char                bReserved1[4];                  // byte alignment
	double				PTS;							// PTS(ms)
	NET_TIME_EX			UTC;							// the event happen time
	int					nEventID;						// event ID
	DH_MSG_OBJECT		stuObject;						// object info
	DH_MSG_OBJECT       stuVehicle;                     // vehicle info
	int					nLane;							// lane number
	int					nSequence;						// snap index
	int					nSpeed;							// speed
	DEV_EVENT_TRAFFIC_TRAFFICCAR_INFO   stTrafficCar;   // TrafficCar info
	DH_EVENT_FILE_INFO  stuFileInfo;                    // event file info            
	BYTE                bEventAction;                   // Event action,0 means pulse event,1 means continuous event's begin,2means continuous event's end;
    BYTE                byReserved0[2];
    BYTE                byImageIndex;                   // Serial number of the picture, in the same time (accurate to seconds) may have multiple images, starting from 0
    DWORD               dwSnapFlagMask;	                // flag(by bit),see NET_RESERVED_COMMON
	DH_RESOLUTION_INFO  stuResolution;                  // picture resolution
	BYTE                byReserved[1016];           
}DEV_EVENT_TRAFFIC_VEHICLEINROUTE_INFO;

// the describe of EVENT_ALARM_LOCALALARM and EVENT_ALARM_MOTIONALARM's data
typedef struct tagDEV_EVENT_ALARM_INFO
{
	int					nChannelID;						// channel id
	char				szName[128];					// evnent name
	char				Reserved[4];					// byte alignment
	double				PTS;							// PTS(ms)
	NET_TIME_EX			UTC;							// the event happen time
	int					nEventID;						// evnet ID                                                                                                                                                                                                                                                                    
}DEV_EVENT_ALARM_INFO;

// �����¼����� EVENT_ALARM_ANALOGALARM(ģ��������ͨ������),
typedef struct tagDEV_EVENT_ALARM_ANALOGALRM_INFO 
{
    int                 nChannelID;                                 // (��Ƶ)ͨ����
    char                szName[DH_COMMON_STRING_128];               // ģ��������ͨ������
    char                Reserved[4];                                // �����ֽڶ���	
    double              PTS;                                        // ʱ���(��λ�Ǻ���)
    NET_TIME_EX         UTC;                                        // �¼�������ʱ��
    int                 nEventID;                                   // �¼�ID 
    DH_EVENT_FILE_INFO  stuFileInfo;                                // �¼���Ӧ�ļ���Ϣ   
    int                 nIndex;                                     // ģ��������ͨ����
    NET_SENSE_METHOD    emSensor;                                   // ����������
    int                 nStatus;                                    // ����״̬, -1:δ֪,0:����,1:������Ч(��������),
                                                                    // 2:������ֵ1,3:������ֵ2,4:������ֵ3,5:������ֵ4,
                                                                    // 6:������ֵ1,7:������ֵ2,8:������ֵ3,9:������ֵ4
    float               fValue;                                     // ̽������ֵ
    NET_TIME            stuCollectTime;                             // ���ݲɼ�ʱ��(UTC)
    DWORD               dwSnapFlagMask;                             // ץͼ��־(��λ)�������NET_RESERVED_COMMON
    BYTE                bEventAction;                               // �¼�������0��ʾ�����¼�,1��ʾ�������¼���ʼ,2��ʾ�������¼�����;
    BYTE                byReserved2[1023];                          // �����ֽ�,������չ
}DEV_EVENT_ALARM_ANALOGALRM_INFO;

//EVENT_ALARM_VEHICLEACC(ACC�ϵ籨��)
typedef struct tagDEV_EVENT_ALARM_VEHICLEACC_INFO
{
	int					nChannelID;						// (Channel number)
	char				szName[DH_COMMON_STRING_128];	// (Event Name)
	char				Reserved[4];					// (Reserved byte alignment)
	double				PTS;							// (Timestamp (in milliseconds)
	NET_TIME_EX			UTC;							// (Time the event occurred)
	int					nEventID;						// (Event ID)   
	NET_GPS_STATUS_INFO stGPSStatusInfo;				// (GPS information)
	int					nACCStatus;						// (ACC status 0: Invalid (compatible with), 1: On, 2: Off)                                                                                 	
	BYTE                bEventAction;                   // (Event action, 0 represents the pulse event, an event starts, said persistent, 2 for persistent event ends;)
	BYTE				bConstantElectricStatus;		// (Often charged state 0: Invalid (compatible with), 1: Connection 2: Disconnect)						
	BYTE                bReserved[1022];                // (Reserved bytes, left extensions.)
}DEV_EVENT_ALARM_VEHICLEACC_INFO;


// �����¼����� EVENT_ALARM_VEHICLE_TURNOVER(�����෭) , EVENT_ALARM_VEHICLE_COLLISION(����ײ��)
typedef struct tagDEV_EVENT_VEHICEL_ALARM_INFO
{
	int					nChannelID;						// (Channel number)
	char				szName[128];					// ( event name )
	char				Reserved[4];					// ��Reserved bytes, left extensions.)
	double				PTS;							// (Timestamp (in milliseconds))
	NET_TIME_EX			UTC;							// (Time for the event occurred )
	int					nEventID;						// (Event ID)
	NET_GPS_STATUS_INFO stGPSStatusInfo;				// (GPS information)
	DH_EVENT_FILE_INFO  stuFileInfo;                    // (Event corresponds to file information)
	BYTE                bEventAction;                   // (Event action, 0 represents the pulse event, 1 persistent event starts, 2 persistent event ends;)
	BYTE                byReserved[2];					// (With Byte alignment)
	BYTE				byImageIndex;					// (Serial number of the picture, in the same time (accurate to seconds) may have multiple images, starting from 0��)
	DWORD               dwSnapFlagMask;                 // (Grab flag (bit), see specific NET_RESERVED_COMMON)
	DH_RESOLUTION_INFO  stuResolution;                  // (Corresponding to the resolution of the picture��
	BYTE                bReserved[1024];                // (Reserved bytes, left extensions.)
}DEV_EVENT_VEHICEL_ALARM_INFO;

// the describe of  EVENT_IVS_PRISONERRISEDETECTION's data
typedef struct tagDEV_EVENT_PRISONERRISEDETECTION_INFO
{
	int					nChannelID;						  // channel id
	char				szName[128];					  // evnent name
	char                bReserved1[4];                    // byte alignment
	double				PTS;							  // PTS(ms)
	NET_TIME_EX			UTC;						   	  // he event happen time
	int					nEventID;					      // evnet ID           
	DH_MSG_OBJECT		stuObject;						  // object info
	int                 nDetectRegionNum;				  // region point number
	DH_POINT            DetectRegion[DH_MAX_DETECT_REGION_NUM];    // region
	DH_EVENT_FILE_INFO  stuFileInfo;                      // event file info
	double				dInitialUTC;			  		  // UTC init time
	BYTE                bEventAction;                     // Event action,0 means pulse event,1 means continuous event's begin,2means continuous event's end;
    BYTE                byReserved[2];
    BYTE                byImageIndex;                     // Serial number of the picture, in the same time (accurate to seconds) may have multiple images, starting from 0
    DWORD               dwSnapFlagMask;	                  // flag(by bit),see NET_RESERVED_COMMON
	int                 nSourceIndex;                     // the source device's index,-1 means data in invalid
	char                szSourceDevice[MAX_PATH];         // the source device's sign(exclusive),field said local device does not exist or is empty
	BYTE				bReserved[752];				  	  // Reserved 
}DEV_EVENT_PRISONERRISEDETECTION_INFO;

//Event type  EVENT_IVS_TRAFFIC_PEDESTRAINPRIORITY(Pedestal has higher priority at the  crosswalk) corresponding data block description info
typedef struct tagDEV_EVENT_TRAFFIC_PEDESTRAINPRIORITY_INFO
{
	int                 nChannelID;                       // Channel No.
	char                szName[128];                      // Event name
	char                bReserved1[4];                    // byte alignment
	double              PTS;                              // Time stamp(ms)
	NET_TIME_EX         UTC;                              // Event occurred time
	int                 nEventID;                         // Event ID
	DH_MSG_OBJECT       stuObject;                        // Detected object
	DH_MSG_OBJECT       stuVehicle;                       // Vehicle body info
	DH_EVENT_FILE_INFO  stuFileInfo;                      // The corresponding file info of the event
	int                 nLane;                            // Corresponding lane No.
	double				dInitialUTC;			  		  // Event initial UTC time 	UTC is the second of the event UTC (1970-1-1 00:00:00)
	BYTE                bEventAction;                     // Event operation.0=pulse event,1=begin of the durative event,2=end of the durative event;
    BYTE                byReserved[2];
    BYTE                byImageIndex;                     // Serial number of the picture, in the same time (accurate to seconds) may have multiple images, starting from 0
	DWORD               dwSnapFlagMask;                   // Snap flag(by bit),please refer to NET_RESERVED_COMMON		
	DEV_EVENT_TRAFFIC_TRAFFICCAR_INFO   stTrafficCar;     // The record of the database of the traffic vehicle 
	DH_RESOLUTION_INFO  stuResolution;                    // picture resolution
	BYTE                bReserved[1024];                  // Reserved field for future extension.
}DEV_EVENT_TRAFFIC_PEDESTRAINPRIORITY_INFO;

//Event type  EVENT_IVS_TRAFFIC_VEHICLEINBUSROUTE(vehicle in bus route)corresponding data block description info
typedef struct tagDEV_EVENT_TRAFFIC_VEHICLEINBUSROUTE_INFO
{
	int                 nChannelID;                       // channel ID
	char                szName[128];                      // event name
	char                bReserved1[4];                    // byte alignment
	double              PTS;                              // Time stamp(ms)
	NET_TIME_EX         UTC;                              // Event occurred time
	int                 nEventID;                         // Event ID
	DH_MSG_OBJECT       stuObject;                        // Detected object
	DH_MSG_OBJECT       stuVehicle;                       // Vehicle body info
	DH_EVENT_FILE_INFO  stuFileInfo;                      // The corresponding file info of the event
	int                 nLane;                            // Corresponding lane No.
	int					nSequence;						  // snap index: such as 3,2,1,1 means the last one,0 means there has some exception and snap stop
	int					nSpeed;							  // speed km/h
	BYTE                bEventAction;                     // Event operation.0=pulse event,1=begin of the durative event,2=end of the durative event;
    BYTE                byReserved[2];
    BYTE                byImageIndex;                     // Serial number of the picture, in the same time (accurate to seconds) may have multiple images, starting from 0
	DWORD               dwSnapFlagMask;                   // Snap flag(by bit),please refer to NET_RESERVED_COMMON		
	DEV_EVENT_TRAFFIC_TRAFFICCAR_INFO stTrafficCar;       // The record of the database of the traffic vehicle 
	DH_RESOLUTION_INFO  stuResolution;                    // picture resolution
	BYTE                bReserved[1020];                  // reserved
}DEV_EVENT_TRAFFIC_VEHICLEINBUSROUTE_INFO;

//Event type  EVENT_IVS_TRAFFIC_BACKING(traffic backing)corresponding data block description info
typedef struct tagDEV_EVENT_IVS_TRAFFIC_BACKING_INFO
{
	int                 nChannelID;                       // channel ID
	char                szName[128];                      // event name
	char                bReserved1[4];                    // byte alignment
	double              PTS;                              // Time stamp(ms)
	NET_TIME_EX         UTC;                              // Event occurred time
	int                 nEventID;                         // Event ID
	DH_MSG_OBJECT       stuObject;                        // Detected object
	DH_MSG_OBJECT       stuVehicle;                       // Vehicle body info
	DH_EVENT_FILE_INFO  stuFileInfo;                      // The corresponding file info of the event
	int                 nLane;                            // Corresponding lane No.
	int					nSequence;						  // snap index: such as 3,2,1,1 means the last one,0 means there has some exception and snap stop
	int					nSpeed;							  // speed km/h
	BYTE                bEventAction;                     // Event operation.0=pulse event,1=begin of the durative event,2=end of the durative event;
    BYTE                byReserved[2];
    BYTE                byImageIndex;                     // Serial number of the picture, in the same time (accurate to seconds) may have multiple images, starting from 0
	DWORD               dwSnapFlagMask;                   // Snap flag(by bit),please refer to NET_RESERVED_COMMON		
	DEV_EVENT_TRAFFIC_TRAFFICCAR_INFO stTrafficCar;       // The record of the database of the traffic vehicle 
	DH_RESOLUTION_INFO  stuResolution;                    // picture resolution
	BYTE                bReserved[1020];                  // reserved
}DEV_EVENT_IVS_TRAFFIC_BACKING_INFO;

//Event type EVENT_IVS_AUDIO_ABNORMALDETECTION(audio abnormal detection)corresponding data block description info
typedef struct tagDEV_EVENT_IVS_AUDIO_ABNORMALDETECTION_INFO
{
	int                 nChannelID;                       // channel ID
	char                szName[128];                      // event name
	char                bReserved1[4];                    // byte alignment
	double              PTS;                              // Time stamp(ms)
	NET_TIME_EX         UTC;                              // Event occurred time
	int                 nEventID;                         // Event ID
	DH_EVENT_FILE_INFO  stuFileInfo;                      // The corresponding file info of the event
	int                 nDecibel;                         // decubel
	int                 nFrequency;                       // frequency
	BYTE                bEventAction;                     // Event operation.0=pulse event,1=begin of the durative event,2=end of the durative event;
    BYTE                byReserved[2];
    BYTE                byImageIndex;                     // Serial number of the picture, in the same time (accurate to seconds) may have multiple images, starting from 0
	DWORD               dwSnapFlagMask;                   // Snap flag(by bit),please refer to NET_RESERVED_COMMON	
	DH_RESOLUTION_INFO  stuResolution;                    // picture resolution
	BYTE                bReserved[1024];                  // reserved
}DEV_EVENT_IVS_AUDIO_ABNORMALDETECTION_INFO;

//Event type  EVENT_IVS_TRAFFIC_RUNYELLOWLIGHT(traffic run yellow light)corresponding data block description info
typedef struct tagDEV_EVENT_TRAFFIC_RUNYELLOWLIGHT_INFO
{
	int					nChannelID;						  // channel ID
	char				szName[128];					  // event name
	char                bReserved1[4];                    // byte alignment
	double				PTS;							  // Time stamp(ms)
	NET_TIME_EX			UTC;				              // Event occurred time
	int					nEventID;			              // Event ID
	int					nLane;				              // Corresponding lane No.
	DH_MSG_OBJECT		stuObject;	              	      // have being detected object
	DH_MSG_OBJECT       stuVehicle;                       // Vehicle body info
	DH_EVENT_FILE_INFO  stuFileInfo;                      // The corresponding file info of the event 
	int					nLightState;	              	  // state of traffic light 0:unknown 1:green 2:red 3:yellow
	int					nSpeed;			              	  // speed km/h
	int                 nSequence;                        // snap index: such as 3,2,1,1 means the last one,0 means there has some exception and snap stop
	BYTE                bEventAction;					  // Event operation.0=pulse event,1=begin of the durative event,2=end of the durative event;
    BYTE                byReserved[2];
    BYTE                byImageIndex;                     // Serial number of the picture, in the same time (accurate to seconds) may have multiple images, starting from 0
    DWORD               dwSnapFlagMask;	                  // Snap flag(by bit),please refer to NET_RESERVED_COMMON		
	NET_TIME_EX         stYellowLightUTC;                 // begin time of yellow light
	unsigned int        nYellowLightPeriod;               // yellow light period time (s)
	DH_RESOLUTION_INFO  stuResolution;                    // picture resolution
 	BYTE                byRedLightMargin;                 // time interval(s)
	char                szSourceDevice[MAX_PATH];         // the source device's sign(exclusive),field said local device does not exist or is empty
	DEV_EVENT_TRAFFIC_TRAFFICCAR_INFO stTrafficCar;       // Traffic vehicle info
	BYTE				bReserved[1024];	              // reserved
} DEV_EVENT_TRAFFIC_RUNYELLOWLIGHT_INFO;

//Event type  EVENT_IVS_LEAVEDETECTION(leave check)corresponding data block description info
typedef struct tagDEV_EVENT_IVS_LEAVE_INFO
{
	int					nChannelID;						// channel ID
	char				szName[128];					// event name
	char                bReserved1[4];                  // byte alignment
	double				PTS;							// Time stamp(ms)
	NET_TIME_EX			UTC;							// Event occurred time
	int					nEventID;						// Event ID
	DH_MSG_OBJECT		stuObject;						// Detected object
	DH_EVENT_FILE_INFO  stuFileInfo;                    // The corresponding file info of the event
	DH_RESOLUTION_INFO  stuResolution;                  // picture resolution
	int                 nDetectRegionNum;				// Rule Detect Area Top Number
	DH_POINT            DetectRegion[DH_MAX_DETECT_REGION_NUM];    //Rule Detect Area
	BYTE                bEventAction;                   // Event operation.0=pulse event,1=begin of the durative event,2=end of the durative event;
    BYTE                byImageIndex;                     // Serial number of the picture, in the same time (accurate to seconds) may have multiple images, starting from 0
	BYTE				bReserved[1026];	            // reserved
} DEV_EVENT_IVS_LEAVE_INFO;

//Event type  EVENT_IVS_CLIMBDETECTION(climb check)corresponding data block description info
typedef struct tagDEV_EVENT_IVS_CLIMB_INFO
{
	int					nChannelID;						// channel ID
	char				szName[128];					// event name
	char                bReserved1[4];                  // byte alignment
	double				PTS;							// Time stamp(ms)
	NET_TIME_EX			UTC;							// Event occurred time
	int					nEventID;						// event ID
	DH_MSG_OBJECT		stuObject;						// Detected object
	DH_EVENT_FILE_INFO  stuFileInfo;                    // The corresponding file info of the event
	DH_RESOLUTION_INFO  stuResolution;                  // picture resolution
	int                 nDetectLineNum;                        // Acme amount of the rule detect zone
	DH_POINT            DetectLine[DH_MAX_DETECT_LINE_NUM];    // Rule detect zone 
	BYTE                bEventAction;                   // Event operation.0=pulse event,1=begin of the durative event,2=end of the durative event;
	BYTE				byImageIndex;					// Serial number of the picture, in the same time (accurate to seconds) may have multiple images, starting from 0
	BYTE				bReserved[1026];	            // reserved
}DEV_EVENT_IVS_CLIMB_INFO;

//EVENT_IVS_MULTISCENESWITCH Event Type EVENT_IVS_MULTISCENESWITCH (multi-scene change event) corresponds to the description of the data block
typedef struct tagDEV_EVENT_IVS_MULTI_SCENE_SWICH_INFO
{
	int					nChannelID;						// (Channel number)
	char				szName[128];					// (Event name)
	char                bReserved1[4];                  // 
	double				PTS;							// (Timestamp (in milliseconds))
	NET_TIME_EX			UTC;							// (Time for the event occurred )
	int					nEventID;						// (Event ID)
	BYTE                bEventAction;                   // (Event action, 0 represents the pulse event, 1 persistent event starts, 2 persistent event ends;)
	BYTE				bReserved[1027];	            // (Reserved bytes)
} DEV_EVENT_IVS_MULTI_SCENE_SWICH_INFO;

//Event type  EVENT_IVS_TRAFFIC_PARKINGONYELLOWBOX(parking on yellow)corresponding data block description info
typedef struct tagDEV_EVENT_TRAFFIC_PARKINGONYELLOWBOX_INFO
{
	int					nChannelID;						// channel ID 
	char				szName[128];					// event name
	char                bReserved1[8];                  // byte alignment
	DWORD				PTS;							// Time stamp(ms)
	NET_TIME_EX			UTC;							// Event occurred time
	int					nEventID;						// Event ID
	int					nLane;							// Corresponding lane No.
	DH_MSG_OBJECT		stuObject;						// Detected object
	DH_MSG_OBJECT       stuVehicle;                     // Vehicle body info
	DH_EVENT_FILE_INFO  stuFileInfo;                    // The corresponding file info of the event

	int					nInterval1;						// the first and second time interval(s)
	int					nInterval2;						// 3rd and 2nd delay time, unit is second
	int					nFollowTime;					// follow time,if a car and a car before entering the pornographic websites,is less than this value,just as with car to enter, to enter the case if the parkingis not illegal

	BYTE                bEventAction;                   // Event operation.0=pulse event,1=begin of the durative event,2=end of the durative event;
    BYTE                byReserved[2];
    BYTE                byImageIndex;                     // Serial number of the picture, in the same time (accurate to seconds) may have multiple images, starting from 0
	DWORD               dwSnapFlagMask;	                // Snap flag(by bit),please refer to NET_RESERVED_COMMON		
	DH_RESOLUTION_INFO  stuResolution;                  // picture resolution
	DEV_EVENT_TRAFFIC_TRAFFICCAR_INFO stTrafficCar;     // Traffic vehicle info
	BYTE				bReserved[1024];	            // reserved

}DEV_EVENT_TRAFFIC_PARKINGONYELLOWBOX_INFO;

//Event type  EVENT_IVS_TRAFFIC_PARKINGSPACEPARKING(parking space parking)corresponding data block description info
typedef struct tagDEV_EVENT_TRAFFIC_PARKINGSPACEPARKING_INFO
{
	int					nChannelID;						// channel ID
    char                szName[DH_EVENT_NAME_LEN];      // event name
	char                bReserved1[8];                  // byte alignment
	DWORD				PTS;							// Time stamp(ms)
	NET_TIME_EX			UTC;							// Event occurred time
	int					nEventID;						// Event ID
	int					nLane;							// Corresponding lane No.
	DH_MSG_OBJECT		stuObject;						// Detected object
	DH_MSG_OBJECT       stuVehicle;                     // Vehicle body info
	DH_EVENT_FILE_INFO  stuFileInfo;                    // The corresponding file info of the event
	
	int                 nSequence;                      // snap index: such as 3,2,1,1 means the last one,0 means there has some exception and snap stop
	BYTE                bEventAction;                   // Event operation.0=pulse event,1=begin of the durative event,2=end of the durative event;
    BYTE                byReserved[2];
    BYTE                byImageIndex;                     // Serial number of the picture, in the same time (accurate to seconds) may have multiple images, starting from 0
	DWORD               dwSnapFlagMask;	                // Snap flag(by bit),please refer to NET_RESERVED_COMMON	
	DH_RESOLUTION_INFO  stuResolution;                  // picture resolution
	DEV_EVENT_TRAFFIC_TRAFFICCAR_INFO stTrafficCar;     // Traffic vehicle info
	BYTE				bReserved[1024];	            // reserved
	
}DEV_EVENT_TRAFFIC_PARKINGSPACEPARKING_INFO;

//Event type  EVENT_IVS_TRAFFIC_PARKINGSPACENOPARKING(parking space no parking)corresponding data block description info
typedef struct tagDEV_EVENT_TRAFFIC_PARKINGSPACENOPARKING_INFO
{
	int					nChannelID;						// channel ID
    char                szName[DH_EVENT_NAME_LEN];      // event name
	char                bReserved1[8];                  // byte alignment
	DWORD				PTS;							// Time stamp(ms)
	NET_TIME_EX			UTC;							// Event occurred time
	int					nEventID;						// Event ID
	int					nLane;							// Corresponding lane No.
	DH_MSG_OBJECT		stuObject;						// Detected object
	DH_MSG_OBJECT       stuVehicle;                     // Vehicle body info
	DH_EVENT_FILE_INFO  stuFileInfo;                    // The corresponding file info of the event
	
	int                 nSequence;                       // snap index: such as 3,2,1,1 means the last one,0 means there has some exception and snap stop
	BYTE                bEventAction;                   // Event operation.0=pulse event,1=begin of the durative event,2=end of the durative event;
    BYTE                byReserved[2];
    BYTE                byImageIndex;                     // Serial number of the picture, in the same time (accurate to seconds) may have multiple images, starting from 0
	DWORD               dwSnapFlagMask;	                // Snap flag(by bit),please refer to NET_RESERVED_COMMON	
	DH_RESOLUTION_INFO  stuResolution;                  // picture resolution
	DEV_EVENT_TRAFFIC_TRAFFICCAR_INFO stTrafficCar;     // Traffic vehicle info
	BYTE				bReserved[1024];	            // reserved
	
}DEV_EVENT_TRAFFIC_PARKINGSPACENOPARKING_INFO;

// EVENT_IVS_TRAFFIC_PARKINGSPACEOVERLINE Corresponding data block description
typedef struct tagDEV_EVENT_TRAFFIC_PARKINGSPACEOVERLINE_INFO
{
	int					nChannelID;						// ��Channel number��
    char                szName[DH_EVENT_NAME_LEN];      // ��Event name��
	DWORD				PTS;							// ��Timestamp (in milliseconds)��
	NET_TIME_EX			UTC;							// ��The time of events��
	int					nEventID;						// (Event ID)
	int					nLane;							// (Corresponding lane number)
	DH_MSG_OBJECT		stuObject;						// ( object detected)
	DH_MSG_OBJECT       stuVehicle;                     // (Car Body Information)
	DH_EVENT_FILE_INFO  stuFileInfo;                    // ��Event corresponds to file information��
	
	int                 nSequence;                      // ��Means��capture serial number, such as 3,2,1,1 capture end, 0 indicates abnormal end��
	BYTE                byEventAction;                  // ��Event action, 0 represents the pulse event, 1 means persistent event starts, 2 means persistent event ends;...��
	BYTE				byImageIndex;					// (Serial chip, the same time (accurate to seconds) may have multiple images, starting from 0)
	BYTE                byReserved1[2];
	DWORD               dwSnapFlagMask;	                // (Grab flag (bit), see specific NET_RESERVED_COMMON)
	DH_RESOLUTION_INFO  stuResolution;                  // (the resolution of relative picture)
	DEV_EVENT_TRAFFIC_TRAFFICCAR_INFO stuTrafficCar;    // (Transportation Vehicle Information)
	BYTE				byReserved[1024];	            // (Reserved bytes)
}DEV_EVENT_TRAFFIC_PARKINGSPACEOVERLINE_INFO;

//Event type  EVENT_IVS_TRAFFIC_PEDESTRAIN(pedestrain)corresponding data block description info
typedef struct tagDEV_EVENT_TRAFFIC_PEDESTRAIN_INFO
{
    int                 nChannelID;                     // channel ID
    char                szName[DH_EVENT_NAME_LEN];      // event name
    char                bReserved1[8];                  // byte alignment
    DWORD               PTS;                            // Time stamp(ms)
    NET_TIME_EX         UTC;                            // Event occurred time
    int                 nEventID;                       // Event ID
    DH_EVENT_FILE_INFO  stuFileInfo;                    // The corresponding file info of the event
    DH_RESOLUTION_INFO  stuResolution;                  // picture resolution
    DWORD               dwSnapFlagMask;                 // Snap flag(by bit)0 bit:"*",1 bit:"Timing",2 bit:"Manual",3 bit:"Marked",4 bit:"Event",5 bit:"Mosaic",6 bit:"Cutout" 
    BYTE                bEventAction;                   // Event operation.0=pulse event,1=begin of the durative event,2=end of the durative event;
    BYTE                bReserved2[2];
	BYTE				byImageIndex;					// Serial number of the picture, in the same time (accurate to seconds) may have multiple images, starting from 0
    int                 nLane;                          // Corresponding lane No.
    DH_MSG_OBJECT       stuObject;                      // Detected object
    BYTE                bReserved[1024];                // reserved
}DEV_EVENT_TRAFFIC_PEDESTRAIN_INFO;

//Event type  EVENT_IVS_TRAFFIC_THROW(throw)corresponding data block description info
typedef struct tagDEV_EVENT_TRAFFIC_THROW_INFO
{
    int                 nChannelID;                     // channel ID
    char                szName[DH_EVENT_NAME_LEN];      // event name
    char                bReserved1[8];                  // byte alignment
    DWORD               PTS;                            // Time stamp(ms)
    NET_TIME_EX         UTC;                            // Event occurred time
    int                 nEventID;                       // Event ID
    DH_EVENT_FILE_INFO  stuFileInfo;                    // The corresponding file info of the event
    DH_RESOLUTION_INFO  stuResolution;                  // picture resolution
    DWORD               dwSnapFlagMask;                 // Snap flag(by bit)0 bit:"*",1 bit:"Timing",2 bit:"Manual",3 bit:"Marked",4 bit:"Event",5 bit:"Mosaic",6 bit:"Cutout" 
    BYTE                bEventAction;                   // Event operation.0=pulse event,1=begin of the durative event,2=end of the durative event;
    BYTE                bReserved2[2];
	BYTE				byImageIndex;					// Serial number of the picture, in the same time (accurate to seconds) may have multiple images, starting from 0
    int                 nLane;                          // Corresponding lane No.
    DH_MSG_OBJECT       stuObject;                      // Detected object
    BYTE                bReserved[1024];                // reserved
}DEV_EVENT_TRAFFIC_THROW_INFO;

//Event type  EVENT_IVS_TRAFFIC_IDLE(idle)corresponding data block description info
typedef struct tagDEV_EVENT_TRAFFIC_IDLE_INFO
{
    int                 nChannelID;                     // channel ID
    char                szName[DH_EVENT_NAME_LEN];      // event name
    char                bReserved1[8];                  // byte alignment
    DWORD               PTS;                            // Time stamp(ms)
    NET_TIME_EX         UTC;                            // Event occurred time
    int                 nEventID;                       // Event ID
    DH_EVENT_FILE_INFO  stuFileInfo;                    // The corresponding file info of the event
    DH_RESOLUTION_INFO  stuResolution;                  // picture resolution
    DWORD               dwSnapFlagMask;                 // Snap flag(by bit)0 bit:"*",1 bit:"Timing",2 bit:"Manual",3 bit:"Marked",4 bit:"Event",5 bit:"Mosaic",6 bit:"Cutout" 
    BYTE                bEventAction;                   // Event operation.0=pulse event,1=begin of the durative event,2=end of the durative event;
    BYTE                bReserved2[2];
	BYTE				byImageIndex;					// Serial number of the picture, in the same time (accurate to seconds) may have multiple images, starting from 0
    int                 nLane;                          // Corresponding lane No.
    BYTE                bReserved[1024];                // reserved
}DEV_EVENT_TRAFFIC_IDLE_INFO;

#define MAX_DRIVING_DIR_NUM          16                             //  ������ʻ����������

// ������ʻ����
typedef enum tagNET_FLOWSTAT_DIRECTION
{
	DRIVING_DIR_UNKNOW = 0 ,		// (Before compatible)
	DRIVING_DIR_APPROACH ,			// (Uplink, the vehicle away from the device deployment point is getting closer)
	DRIVING_DIR_LEAVE ,				// (Go down, that the vehicle is farther away from  equipment deployment point)
}NET_FLOWSTAT_DIRECTION;

// ��·����
typedef enum tagNET_ROAD_DIRECTION
{
    ROAD_DIR_UNKNOW,             // δ֪  
    ROAD_DIR_TURNLEFT,           // ��ת 
    ROAD_DIR_TURNRIGHT,          // ��ת
    ROAD_DIR_STRAIGHT,           // ֱ��
    ROAD_DIR_UTURU,              // ��ͷ
    ROAD_DIR_NUM,    
}NET_ROAD_DIRECTION;

//Vehicle flow statistics lane direction information
typedef struct tagNET_TRAFFIC_FLOWSTAT_INFO_DIR
{
	NET_FLOWSTAT_DIRECTION		emDrivingDir;							//(Driving direction)
	char						szUpGoing[FLOWSTAT_ADDR_NAME];			//(Uplink locations)
	char						szDownGoing[FLOWSTAT_ADDR_NAME];		//(Go down location)
	BYTE						reserved[32];							//(Reserved bytes)
	
}NET_TRAFFIC_FLOWSTAT_INFO_DIR;

typedef struct tagNET_TRAFFIC_FLOW_STATE 
{
	int								nLane;				// (Lane number)
	DWORD							dwState;			// (State value)
														// (1 - heavy traffic)
														// (2-heavy traffic recovery)
														// (3-normal)
														// (4 - Flow is too  little)
														// (5-Traffic too low recovery)
	DWORD							dwFlow;				// (Flow value, units: vehicles)
	DWORD							dwPeriod;			// (Corresponding statistical time of the flow value )
	NET_TRAFFIC_FLOWSTAT_INFO_DIR	stTrafficFlowDir;	// (Lane direction information)
	int					            nVehicles;			// (Total number of passing vehicles)
	float				            fAverageSpeed;		// (Average speed, unit km / h)
	float				            fAverageLength;		// (The average vehicle length, unit meters)
	float				            fTimeOccupyRatio;	// (Share of the time , i.e., The ratio of the sum time for the vehicle passing the cross-section  in  the unit  time and per unit time )
	float				            fSpaceOccupyRatio;	// (Share of the space ,is the result that the average driving distance intervals vehicle is divided the sum of the length of the vehicle measured by the percentage 
	float			            	fSpaceHeadway;		// (Headway, the distance between adjacent vehicles in m / vehicle)
	float				            fTimeHeadway;		// (Headway in seconds / vehicle)
	float				            fDensity;			// (Vehicle density, the number of vehicles per kilometer, unit vehicles / km)
	int					            nOverSpeedVehicles;	// (The number of Speeding vehicles)
	int					            nUnderSpeedVehicles;// (The number of low speeding vehicles)
	int				            	nLargeVehicles;		// (Carts traffic (9 m <car length <12 m), vehicle / unit time)
	int				            	nMediumVehicles;	// (Medium car Traffic 6 m ??<car length <9 meters), vehicle / unit time
	int				            	nSmallVehicles;		// (Car Traffic 4 m ??<car length <6 meters), vehicle / unit time
	int				            	nMotoVehicles;		// (Motorized traffic (mini-car, car length <4 m), vehicle / unit time��
	int				            	nLongVehicles;		// (long traffic (car length> = 12 m), vehicle / unit time��
	int                             nVolume;            // (Traffic, vehicles / unit time, the number of vehicles which pass through the lane, the road and other vehicles, caculated in one hour)
	int                             nFlowRate;          // (Flow rate of the car, Vehicles / hour, equivalent hours for Vehicle through the lane, a section or a section of the road)
	int                             nBackOfQueue;       // (Queue length, unit: m, distance from the signalized intersection stop line between the upstream end of the line vehicle)
	int                             nTravelTime;        // (Travel time, unit: second, a road vehicle used by a certain time. Including all parking delays)
	int                             nDelay;             // (Delay unit: seconds, extra travel time for the driver, passenger or pedestrian spend)
    BYTE                            byDirection[MAX_DRIVING_DIR_NUM]; // �����������NET_ROAD_DIRECTION
    BYTE                            byDirectionNum;                 // ������ʻ�������
	BYTE							reserved[859];		// (Reserved bytes)
}NET_TRAFFIC_FLOW_STATE;
 
// EVENT_IVS_TRAFFIC_FLOWSTATE (Corresponding data block description)
typedef struct tagDEV_EVENT_TRAFFIC_FLOW_STATE
{
    int                 nChannelID;                     // (Channel number) 
    char                szName[DH_EVENT_NAME_LEN];      // (Event name)
    char                bReserved1[8];                  // (Byte alignment)
    DWORD               PTS;                            // (Timestamp (in milliseconds))
    NET_TIME_EX         UTC;                            // (Time for the event occurred)
    int                 nEventID;                       // (Event ID)
	int					nSequence;						// (No.)
	int					nStateNum;						// (the number of traffic state)
	NET_TRAFFIC_FLOW_STATE stuStates[DH_MAX_LANE_NUM];	// (Flow state, each lane corresponds to an element in the array)
    BYTE                bReserved[1024];                // (Reserved bytes)
}DEV_EVENT_TRAFFIC_FLOW_STATE;

// Media file search criteria
typedef enum __EM_FILE_QUERY_TYPE
{
	DH_FILE_QUERY_TRAFFICCAR,							// Vehicle information 
	DH_FILE_QUERY_ATM,									// ATM information
	DH_FILE_QUERY_ATMTXN,								// ATM transaction information 
	DH_FILE_QUERY_FACE,  								// Face info
    DH_FILE_QUERY_FILE,                                 // file info, corresponding to NET_IN_MEDIA_QUERY_FILE and NET_OUT_MEDIAFILE_FILE
	DH_FILE_QUERY_TRAFFICCAR_EX,						// Transportation vehicle information, expand DH_FILE_QUERY_TRAFFICCAR, support more fields
    DH_FILE_QUERY_FACE_DETECTION,                       // ��������¼���Ϣ MEDIAFILE_FACE_DETECTION_PARAM �� MEDIAFILE_FACE_DETECTION_INFO
} EM_FILE_QUERY_TYPE;

// record info, corresponde to CLIENT_FindFileEx, search condition
// support paths search in curent
typedef struct  
{
    DWORD               dwSize;                 // size 
    char*               szDirs;                 // working directory list,can inquire multiple directory at a atime,separated by ";",example "/mnt/dvr/sda0;/mnt/dvr/sda1",if szDirs==null or szDirs == "" ,means search all
    int					nMediaType;		        // file info,0:any type,1:search jpg image,2:search dav
}NET_IN_MEDIA_QUERY_FILE;

// record info, corresponde to CLIENT_FindFileEx, search result
typedef struct  
{
    DWORD               dwSize;                 // size
    int					nChannelID;				// channel ID,from 0,-1 means search all
    NET_TIME			stuStartTime;			// start time
    NET_TIME			stuEndTime;				// end time
    unsigned int		nFileSize;				// size of file
    BYTE				byFileType;				// file type 1:jpg, 2: dav
    BYTE                byDriveNo;              // drive no.
    BYTE                byReserved1[2];         // reserved
    unsigned int        nCluster;               // cluster
	char				szFilePath[MAX_PATH];	// FilePath
}NET_OUT_MEDIA_QUERY_FILE;
//The corresponding search criteria of  DH_MEDIA_QUERY_TRAFFICCAR
typedef struct  
{
	int					nChannelID;						// The channel number begins with 0. -1 is to search information of all channels .
	NET_TIME			StartTime;						// Start time 	
	NET_TIME			EndTime;						// End time 
	int					nMediaType;						// File type:0=search any type.1=search jpg file
	int					nEventType;						// Event type,please refer to Intelligent Analytics Event Type. 0 means search any event. 
	char				szPlateNumber[32];				// Vehicle plate. "\0" is to search any plate number.
	int					nSpeedUpperLimit;				// The searched vehicle speed range. Max speed unit is km/h
	int					nSpeedLowerLimit;				// The searched vehicle speed range. Min speed unit is km/h
	BOOL				bSpeedLimit;					// Search according to the speed or not.  TRUE: search according to the speed.nSpeedUpperLimit and nSpeedLowerLimit is valid.
    DWORD				dwBreakingRule;					// Illegal type:
														// When event type is EVENT_IVS_TRAFFICGATE
														//		bit1: Retrograde;   bit2: Overline; 
														//		bit3: Overspend; 	bit4:Under speed; 
														//		bit5: RunRedLight;
														// When event type is EVENT_IVS_TRAFFICJUNCTION
														//		bit1: RunRedLight;  bit2: WrongLan;  
														//		bit3: Retrograde; 	bit4:UTurn;
														//	    bit5: Overline;
	char                szPlateType[32];                // plate type,"Unknown" ,"Normal" ,"Yellow" ,"DoubleYellow" ,"Police" "Armed" 
	char                szPlateColor[16];               // plate color, "Blue","Yellow", "White","Black"
	char				szVehicleColor[16];		        // vehicle color:"White", "Black", "Red", "Yellow", "Gray", "Blue","Green"
	char				szVehicleSize[16];		        // vehicle type:"Light-duty";"Medium"; "Oversize"
	int                 nGroupID;                       // id of event group(it works when >= 0)
	short               byLane;                         // lane number(it works when >= 0)
	BYTE				byFileFlag;						// file flag, 0xFF-use nFileFlagEx, 0-all record, 1-timing file, 2-manual, 3-event, 4-important, 5-mosaic
	BYTE				byRandomAccess;                 // The need for random jumps in the query process, 0 - no need 1 - need
	int					nFileFlagEx;					// file flag, bit0-timing, bit1-manual, bit2-event, bit3-important, bit4-mosaic, 0xFFFFFFFF-all
	int					nDirection;				        // direction(to the direction of car)	0-north 1-northeast 2-east 3-southeast 4-south 5-southwest 6-west 7-northwest 8-unknown -1-all directions
	char*               szDirs;                         // working directory list,can inquire multiple directory at a atime,separated by ";",example "/mnt/dvr/sda0;/mnt/dvr/sda1",if szDirs==null or szDirs == "" ,means search all
	int*                pEventTypes;                    // Check the event type to be an array of pointers, event type, see "intelligent analysis event type", if the query is NULL considered all events (buffer required to apply by the user)
	int                 nEventTypeNum;                  // Event Type array size
	char*				pszDeviceAddress;               // Device address, NULL indicates that the field does not work
	char*				pszMachineAddress;				// Machine deployment locations, NULL indicates that the field does not work
	char*				pszVehicleSign;					// Vehicle identification, such as "Unknown" - unknown, "Audi" - Audi, "Honda" - Honda ... NULL indicates that the field does not work
	int					bReserved[32];					// Reserved field for future extension.
} MEDIA_QUERY_TRAFFICCAR_PARAM;


// The media file information searched by DH_MEDIA_QUERY_TRAFFICCAR
typedef struct
{
    unsigned int		ch;						// Channel number
    char				szFilePath[128];		// File path 
    unsigned int		size;					// File length 
    NET_TIME			starttime;				// Start time
    NET_TIME			endtime;				// End time
    unsigned int		nWorkDirSN;				// Working directory serial number									
	BYTE				nFileType;				// File type.  1:jpg file
	BYTE                bHint;					// File location index
	BYTE                bDriveNo;               // drive number
	BYTE                bReserved2;
	unsigned int        nCluster;               // cluster number
	BYTE				byPictureType;			// flags
	BYTE                bReserved[3];           // Reserved field for future extension. 

	//The following contents is the vehicle information 
	char				szPlateNumber[32];		// Vehicle plate number
	char				szPlateType[32];		// Plate type: "Unknown" =Unknown; "Normal"=Blue and black plate. "Yellow"=Yellow plate. "DoubleYellow"=Double-layer yellow plate 
												// "Police"=Police plate ; "Armed"= =Military police plate; "Military"=Army plate; "DoubleMilitary"=Army double-layer 
												// "SAR" =HK SAR or Macao SAR plate; "Trainning" =rehearsal plate; "Personal"=Personal plate; "Agri"=Agricultural plate
												// "Embassy"=Embassy plate; "Moto"=Moto plate ; "Tractor"=Tractor plate; "Other"=Other plate 
	char				szPlateColor[16];		// Plate color:"Blue","Yellow", "White","Black"
	char				szVehicleColor[16];		// Vehicle color:"White", "Black", "Red", "Yellow", "Gray", "Blue","Green"
	int					nSpeed;					// Speed. The unit is Km/H
	int					nEventsNum;				// Activation event amount 
	int					nEvents[32];			// Activation event list. The number refers to the corresponding event. Please refer to Intelligent Analytics Event Type.		
	DWORD				dwBreakingRule;			// Detailed offense type subnet mask. The first bit means redlight offense, the second bit is illegal straight/left-turn/right-turn driving. 
	                                            // The third bit is the wrong way driving; the four bit is illegal U-turn. Otherwise default value is intersection accident. 
	char				szVehicleSize[16];		// Vehicle type:"Light-duty"=small;"Medium"=medium; "Oversize"=large
	char				szChannelName[DH_CHAN_NAME_LEN];// Local or remote channel name
	char				szMachineName[DH_MAX_NAME_LEN];	// Local or remote device name
	int					nSpeedUpperLimit;	    // up limit of speed, km/h
	int					nSpeedLowerLimit;		// lower limit of speed km/h	
	int                 nGroupID;               // id of event group
	BYTE                byCountInGroup;         // total count of the event group
	BYTE                byIndexInGroup;         // the index of this event
	BYTE                byLane;                 // lane number
	BYTE                bReserved1[45];			// reserved
	int					nDirection;				// direction,MEDIA_QUERY_TRAFFICCAR_PARAM
	char                szMachineAddress[MAX_PATH]; // machine address
} MEDIAFILE_TRAFFICCAR_INFO, *LPMEDIAFILE_TRAFFICCAR_INFO;

// DH_MEDIA_QUERY_TRAFFICCAR_EX Corresponding query
typedef struct tagMEDIA_QUERY_TRAFFICCAR_PARAM_EX
{
	DWORD				dwSize;
	MEDIA_QUERY_TRAFFICCAR_PARAM stuParam;	        	// The basic query parameters
} MEDIA_QUERY_TRAFFICCAR_PARAM_EX;

// DH_MEDIA_QUERY_TRAFFICCAR_EX Check out the file information
typedef struct tagMEDIAFILE_TRAFFICCAR_INFO_EX
{
	DWORD				dwSize;
	MEDIAFILE_TRAFFICCAR_INFO stuInfo;			        // Basic Information
	char				szDeviceAddr[DH_COMMON_STRING_256];	// Device Address
	char				szVehicleSign[DH_COMMON_STRING_32];	// Vehicle identification, such as "Unknown" - unknown, "Audi" - Audi, "Honda" - Honda ..
} MEDIAFILE_TRAFFICCAR_INFO_EX;

// FINDNEXT Find input parameter
typedef struct __NET_FINDNEXT_RESERVED
{
	DWORD               dwSize;                 // Structure size
	
	unsigned int        nBeginNumber;           // Search begin number, start from begin number, 0<=beginNumber<= totalCount-1
}NET_FINDNEXT_RESERVED;

// Enquiry jump condition
typedef struct __NET_FINDING_JUMP_OPTION_INFO
{
	DWORD           dwSize;
	int             nOffset;                            // Query results offset relative to the first query results position offset current query
}NET_FINDING_JUMP_OPTION_INFO;

// DH_FILE_QUERY_FACE Corresponding face recognition service search parameter
typedef struct __MEDIAFILE_FACERECOGNITION_PARAM
{
	DWORD               dwSize;                 // Structure size

	// Search filter criteria
   	NET_TIME			stStartTime;			       // start time 
    NET_TIME			stEndTime;				       // closing time
	char                szMachineAddress[MAX_PATH];    // Place to support fuzzy matching
	int                 nAlarmType;                    // To query the type of alarm, see EM_FACERECOGNITION_ALARM_TYPE
}MEDIAFILE_FACERECOGNITION_PARAM;

typedef struct  tagDH_PIC_INFO_EX
{
	DWORD                dwSize;                        // structure size
	DWORD                dwFileLenth;                   // file size,unit:bite
	char                 szFilePath[MAX_PATH];          // file path   
}DH_PIC_INFO_EX;

typedef struct __NET_CANDIDAT_PIC_PATHS
{
	DWORD                dwSize;                        // structure size
	int                  nFileCount;                    // actual file amount
	DH_PIC_INFO_EX       stFiles[DH_MAX_PERSON_IMAGE_NUM];// file information
}NET_CANDIDAT_PIC_PATHS;

// corresponding facial recognition service  DH_FILE_QUERY_FACE FINDNEXT search returned parameter
typedef struct __MEDIAFILE_FACERECOGNITION_INFO
{
	DWORD               dwSize;                                   // Structure size
	BOOL                bGlobalScenePic;                          // The existence panorama
	DH_PIC_INFO_EX      stGlobalScenePic;                         // Panoramic image file path
	DH_MSG_OBJECT		stuObject;						          // the target face object information
	DH_PIC_INFO_EX      stObjectPic;                              // the target face file path
	int                 nCandidateNum;                            // Face Matching the current number of candidates
    CANDIDATE_INFO      stuCandidates[DH_MAX_CANDIDATE_NUM];      // Face candidates to match this informatio
	NET_CANDIDAT_PIC_PATHS stuCandidatesPic[DH_MAX_CANDIDATE_NUM];// The current face matching candidates to the image file path
	NET_TIME            stTime;                                   // time for an alarm
	char                szAddress[MAX_PATH];                      // Place for an alarm
}MEDIAFILE_FACERECOGNITION_INFO; 

typedef enum __EM_FACEPIC_TYPE
{
    NET_FACEPIC_TYPE_UNKOWN,            // δ֪����
    NET_FACEPIC_TYPE_GLOBAL_SENCE,      // ����ȫ����ͼ
    NET_FACEPIC_TYPE_SMALL,             // ����Сͼ
}EM_FACEPIC_TYPE;

#define  NET_MAX_FRAMESEQUENCE_NUM     2
#define  NET_MAX_TIMESTAMP_NUM         2

typedef struct __MEDIAFILE_FACE_DETECTION_DETAIL_PARAM
{
    DWORD               dwSize;
    DWORD               dwObjectId;                     // ����ID
    DWORD               dwFrameSequence;                // ֡���
    NET_TIME_EX         stTime;                         // ����ʱ��
}MEDIAFILE_FACE_DETECTION_DETAIL_PARAM;
 
// DH_FILE_QUERY_FACE_DETECTION ��Ӧ������ʶ������ѯ����
typedef struct __MEDIAFILE_FACE_DETECTION_PARAM
{
    DWORD               dwSize;                         // �ṹ���С
    
    // ��ѯ��������
    int                 nChannelID;                     // ͨ����
    NET_TIME            stuStartTime;                   // ��ʼʱ��
    NET_TIME            stuEndTime;                     // ����ʱ��
    EM_FACEPIC_TYPE     emPicType;                      // ͼƬ���ͣ���� EM_FACEPIC_TYPE
    BOOL                bDetailEnable;                  // �Ƿ�����ϸ��Ϣ
    MEDIAFILE_FACE_DETECTION_DETAIL_PARAM stuDetail;    // ������ϸ��Ϣ

}MEDIAFILE_FACE_DETECTION_PARAM;

// DH_FILE_QUERY_FACE_DETECTION��Ӧ������ʶ�����FINDNEXT��ѯ���ز���
typedef struct __MEDIAFILE_FACE_DETECTION_INFO
{
    DWORD               dwSize;                                     // �ṹ���С

    unsigned int        ch;                                         // ͨ����
    char                szFilePath[128];                            // �ļ�·��
    unsigned int        size;                                       // �ļ�����
    NET_TIME            starttime;                                  // ��ʼʱ��
    NET_TIME            endtime;                                    // ����ʱ��
    unsigned int        nWorkDirSN;                                 // ����Ŀ¼���                                    
    BYTE                nFileType;                                  // �ļ�����  1��jpgͼƬ
    BYTE                bHint;                                      // �ļ���λ����
    BYTE                bDriveNo;                                   // ���̺�
    BYTE                byPictureType;                              // ͼƬ����, 0-��ͨ, 1-�ϳ�, 2-��ͼ
    unsigned int        nCluster;                                   // �غ�
    
    EM_FACEPIC_TYPE     emPicType;                                  // ͼƬ���ͣ����EM_FACE_PIC_TYPE
    DWORD               dwObjectId;                                 // ����ID
    DWORD               dwFrameSequence[NET_MAX_FRAMESEQUENCE_NUM]; // ֡���,������2��Ԫ��ʱ����һ����ʾСͼ���ڶ�����ʾ��ͼ
    int                 nFrameSequenceNum;                          // ֡��Ÿ��� 
    NET_TIME_EX         stTimes[NET_MAX_TIMESTAMP_NUM];             // ����ʱ�䣬������2��Ԫ��ʱ����һ����ʾСͼ���ڶ�����ʾ��ͼ
    int                 nTimeStampNum;
}MEDIAFILE_FACE_DETECTION_INFO;

// query video synopsis param
typedef struct __MEDIA_QUERY_VIDEOSYNOPSIS_PARAM
{
	DWORD               dwSize;
	NET_TIME			StartTime;						// start time	
	NET_TIME			EndTime;						// end time
	int					nMediaType;						// file type,0:arbitrariness,1:image,2:record
	int                 nQueryType;                     // query type,1:source video file 2:reduce video file
}MEDIA_QUERY_VIDEOSYNOPSIS_PARAM;

typedef struct __MEDIAFILE_VIDEOSYNOPSIS_INFO
{
    char				szFilePath[128];		// file path
    unsigned int		size;					// file size
    NET_TIME			starttime;				// start time
    NET_TIME			endtime;				// end time
    unsigned int		nWorkDirSN;				// worl dir serial number						
	BYTE				nFileType;				// file type  1:jpg
	BYTE                bHint;					// index file location
	BYTE                bDriveNo;               // drive no.
	BYTE                bReserved2;
	unsigned int        nCluster;               // cluster
	BYTE				byPictureType;			// picture type, 0-normal, 1-synthesis, 2-cutout
	BYTE                bReserved[3];           // reserved

	// video source file info
	int                nTaskID;                 // task ID
	char               szCurrentState[DH_MAX_SYNOPSIS_STATE_NAME]; //video synopsis state
	int                nProgress;               // sorresponding state
	int                nObjectNum;              // object num

	// video synopsis file info
	int                nDurationTime;          // duration time (s)
}MEDIAFILE_VIDEOSYNOPSIS_INFO;
// correlate with NET_IN_SNAPSHOT, flash control,array,each element corresponding to a flash light configuration
typedef struct __NET_FLASHCONTROL
{
	DWORD				dwSize;					// struct size
	int					nMode;					// work mode 0-no flash,1-always flash,2-auto flash
}NET_FLASHCONTROL;

// Capture client type
typedef enum tagSNAP_CLIENT_TYPE
{
    SNAP_CLIENT_TYPE_COMMON,               // Corresponds to "Common" type, the default type
    SNAP_CLIENT_TYPE_PARKINGSPACE,         // Correspondence "ParkingSpace" type, parking
}SNAP_CLIENT_TYPE;

#define DH_MAX_USER_DEFINE_INFO        1024

// when nTriggerType==2, client snap info
typedef struct _NET_CLIENT_SNAP_INFO
{
	DWORD				dwSize;					// struct info 
	unsigned int        nFrameSequence;         // frame sequence
	double              PTS;                    // pts,64 bit
	char				szUserDefinedInfo[DH_MAX_USER_DEFINE_INFO];	// String, custom client, "\ 0" at the end
	SNAP_CLIENT_TYPE    emSNAP_CLIENT_TYPE;     // Client Type
	DWORD               dwRetCardNumber;        // card amount
    EVENT_CARD_INFO     stuCardInfo[DH_EVENT_MAX_CARD_NUM];  // card information
}NET_CLIENT_SNAP_INFO;

// CLIENT_TrafficSnapByNetwork's input param
typedef struct __NET_IN_SNAPSHOT
{
	DWORD				dwSize;					// struct size
	int					nTriggerType;			// trigger type	0-unknown 1-zhongmeng net trigger
	int					nLaneID;				// lane id
	int					nGroupID;				// group id
	int					nGrabTimes;				// picture number
	int					nStartPicNum;			// the start picture id
	int					nDirection;				// road direction 0-north 1-east north 2-east 3-east south 4-south 5-west south 6-west 7-west north 8-unknown
	int					nGrabWaitTime;			// pGrabWaitTime group member number
	DWORD*				pGrabWaitTime;			// interval time between two picture
	int					nLowerSpeedLimit;		// lower speed limit, km/h
	int					nUpperSpeedLimit;		// upper speed limit, km/h
	int					nSpeed;					// speed, km/h
	int					nViolationNo;			// violation number 0-not transgress
												// 1-black shit
												// 2-over speed not over 50% 
												// 3-over speed between 50% and 100% 
												// 4-over speed over 100% 
												// 5-retrograde
												// 6-run red
												// 7-under speed
												// 8-no passing,wrong route
	int					nRedLightTime;			// red light time,s
	int					nFlashControl;			// pFlashControl group member number
	NET_FLASHCONTROL*   pFlashControl;			// flash control
	DWORD				dwUser;					// user data
	NET_CLIENT_SNAP_INFO stClientInfo;          // the incoming snap parameter
}NET_IN_SNAPSHOT;

// CLIENT_TrafficSnapByNetwork's output param
typedef struct __NET_OUT_SNAPSHOT
{
	DWORD				dwSize;					// structure size
}NET_OUT_SNAPSHOT;

// interface(CLIENT_TrafficForceLightState)input parameter
typedef struct __NET_IN_FORCELIGHTSTATE
{
	DWORD				dwSize;					// struct size
	unsigned int        nDirection;             // 0 bit:"Straight",1 bit:"TurnLeft",2 bit:"TurnRight",3 bit:"U-Turn"
}NET_IN_FORCELIGHTSTATE;

// interface (CLIENT_TrafficForceLightState)output parameter
typedef struct __NET_OUT_FORCELIGHTSTATE
{
	DWORD				dwSize;					// struct size
}NET_OUT_FORCELIGHTSTATE;
// CLIENT_StartTrafficFluxStat's callback function
typedef int  (CALLBACK *fFluxStatDataCallBack)(LLONG lFluxStatHandle, DWORD dwEventType, void* pEventInfo, BYTE *pBuffer, DWORD dwBufSize, LDWORD dwUser, int nSequence, void *reserved);

// CLIENT_StartTrafficFluxStat's input param
typedef struct __NET_IN_TRAFFICFLUXSTAT
{
	DWORD				dwSize;					// structure size
	fFluxStatDataCallBack		cbData;			// callback function pointer
	LDWORD				dwUser;					// user data
}NET_IN_TRAFFICFLUXSTAT;

// CLIENT_StartTrafficFluxStat's output param
typedef struct __NET_OUT_TRAFFICFLUXSTAT
{
	DWORD				dwSize;					// structure size	
}NET_OUT_TRAFFICFLUXSTAT;

// CLIENT_StartFindFluxStat's input param
typedef struct __NET_IN_TRAFFICSTARTFINDSTAT
{
	DWORD				dwSize;					// structure size
	NET_TIME			stStartTime;			// start time, temporarily  
	NET_TIME			stEndTime;				// end time, temporarily 
	int					nWaittime;				// the time to wait result
}NET_IN_TRAFFICSTARTFINDSTAT;

// CLIENT_StartFindFluxStat's output param
typedef struct __NET_OUT_TRAFFICSTARTFINDSTAT
{
	DWORD				dwSize;					// structure size
	DWORD               dwTotalCount;           // The total amount that matched current search criteria                 
}NET_OUT_TRAFFICSTARTFINDSTAT;

// CLIENT_DoFindFluxStat's input param
typedef struct __NET_IN_TRAFFICDOFINDSTAT
{
	DWORD				dwSize;					// structure size
	unsigned int		nCount;					// the number of flow Statistic for query
	int					nWaittime;				// the time to wait result
}NET_IN_TRAFFICDOFINDSTAT;

typedef struct
{
	DWORD				dwSize;					// structure size

	int					nStatInfo;				// the number of statistic info
	DH_TRAFFICFLOWSTAT *pStatInfo;				// the statistic pointer
}DH_TRAFFICFLOWSTAT_OUT;

// CLIENT_DoFindFluxStat's out param
typedef struct __NET_OUT_TRAFFICDOFINDSTAT
{
	DWORD				dwSize;					// structure size
	
	DH_TRAFFICFLOWSTAT_OUT stStatInfo;			// the statistic pointer
}NET_OUT_TRAFFICDOFINDSTAT;

// interface(CLIENT_StartFindNumberStat)'s input param
typedef struct __NET_IN_FINDNUMBERSTAT
{
	DWORD				dwSize;					// size
	int                 nChannelID;             // channel ID
	NET_TIME			stStartTime;			// start time
	NET_TIME			stEndTime;				// end time
	int                 nGranularityType;       // granularity type, 0:minute,1:hour,2:day,3:week,4:month,5:quarter,6:year
	int					nWaittime;				// wait time
}NET_IN_FINDNUMBERSTAT;

// CLIENT_StartFindNumberStat's output param
typedef struct __NET_OUT_FINDNUMBERSTAT
{
	DWORD				dwSize;					 
	DWORD               dwTotalCount;           // total count
}NET_OUT_FINDNUMBERSTAT;

// CLIENT_DoFindNumberStat's input param
typedef struct __NET_IN_DOFINDNUMBERSTAT
{
	DWORD				dwSize;					 
	unsigned int        nBeginNumber;           // [0, totalCount-1]
	unsigned int		nCount;					// count
	int					nWaittime;				// wait time
}NET_IN_DOFINDNUMBERSTAT;

typedef struct __DH_NUMBERSTAT
{
	DWORD    dwSize;
	int      nChannelID;                           // channel id
	char     szRuleName[DH_CHAN_NAME_LEN];         // rule name
	NET_TIME stuStartTime;                         // start time
	NET_TIME stuEndTime;                           // end time
    int      nEnteredSubTotal;                     // entered total
	int      nExitedSubtotal;                      // entered total
    int      nAvgInside;                           // average number inside
	int      nMaxInside;                           // max number inside
}DH_NUMBERSTAT;

// CLIENT_DoFindNumberStat's ouput param
typedef struct __NET_OUT_DOFINDNUMBERSTAT
{
	DWORD				dwSize;					             
	int                 nCount;                              // count
	DH_NUMBERSTAT       *pstuNumberStat;                     // state array
    int                 nBufferLen;                          
}NET_OUT_DOFINDNUMBERSTAT;

//// intelligent traffic detector

// CLIENT_GetParkingSpaceStatus's input param
typedef struct tagNET_IN_GET_PARKINGSPACE_STATUS
{
	DWORD                dwSize;                         // struct size
	DWORD                dwWaitTime;                     // wait time
	int                  nChannelID;                     // channel ID
	int *                pLaneID;                        // range[0,255], pLaneID==NULL means all parking space, the max number is DH_PRODUCTION_DEFNITION.nMaxRoadWays
	int                  nLaneCount;                     // apply to sizeof(int)*nLaneCount memory
} NET_IN_GET_PARKINGSPACE_STATUS;

typedef struct tagNET_LANE_PARKINGSPACE_STATUS
{
	DWORD                dwSize;                         // struct size
	int                  nLaneID;                        // lane ID
	unsigned int         nPictureId;                     // picture ID,get picture data
	DEV_EVENT_TRAFFIC_TRAFFICCAR_INFO stTrafficCar;      // traffic car info
} NET_LANE_PARKINGSPACE_STATUS;

// CLIENT_GetParkingSpaceStatus's output param
typedef struct tagNET_OUT_GET_PARKINGSPACE_STATUS
{
	DWORD                dwSize;                         // struct size
	NET_LANE_PARKINGSPACE_STATUS* pStatus;               // status
	int                  nMaxStatus;                     // need apply to sizeof(NET_LANE_PARKINGSPACE_STATUS)*nMaxStatus, and nMaxStatus == DH_PRODUCTION_DEFNITION.nMaxRoadWays memory
	int                  nRetStatus;                     // actual returns status number of parking spaces
} NET_OUT_GET_PARKINGSPACE_STATUS;

// CLIENT_AttachParkingSpaceData's input param
typedef struct tagNET_CB_PARKINGSPACE_DATA
{
	DWORD                dwSize;                         // struct size
	void*                pPicBuffer;                     // image binary data
	unsigned int         nPicLength;                     // picture length
} NET_CB_PARKINGSPACE_DATA;

// CLIENT_AttachParkingSpaceData callback function , pBuf is json and image data info , nBufLen is pBuf length,for forwarding services
typedef int (CALLBACK *fNotifySnapData)(LLONG lParkingHandle, NET_CB_PARKINGSPACE_DATA* pDiagnosisInfo, void* pBuf, int nBufLen, LDWORD dwUser);

// CLIENT_AttachParkingSpaceData's input param
typedef struct tagNET_IN_ATTACH_PARKINGSPACE
{
	DWORD                dwSize;                         // struct size
	DWORD                dwWaitTime;                     // wait time
	int                  nChannelID;                     // channel ID
	unsigned int         nPictureId;                     // picture ID, can look for NET_LANE_PARKINGSPACE_STATUS.nPictureId
	
	fNotifySnapData      cbNotifySnapData;               // callback function
	LDWORD               dwUser;                         // user-defined parameters
} NET_IN_ATTACH_PARKINGSPACE;

// CLIENT_AttachParkingSpaceData's output param
typedef struct tagNET_OUT_ATTACH_PARKINGSPACE
{
	DWORD                dwSize;                         // struct size
} NET_OUT_ATTACH_PARKINGSPACE;

// CLIENT_DetachParkingSpaceData's input param
typedef struct tagNET_IN_DETACH_PARKINGSPACE
{
	DWORD                dwSize;                         // struct size
	DWORD                dwWaitTime;                     // wait time
	LLONG                lParkingHandle;                 // CLIENT_AttachParkingSpaceData return handle
} NET_IN_DETACH_PARKINGSPACE;
// CLIENT_DetachParkingSpaceData's output param
typedef struct tagNET_OUT_DETACH_PARKINGSPACE
{
	DWORD                dwSize;                         // struct size
} NET_OUT_DETACH_PARKINGSPACE;

// Batch download file

// Download progress callback function prototypes�� nError Indicates that an error occurred during the download of��1-���治�㣬2-�Է������ݵ�У�������3-���ص�ǰ�ļ�ʧ�ܣ�4-������Ӧ�����ļ�ʧ��
typedef void (CALLBACK *fMultiFileDownLoadPosCB)(LLONG lDownLoadHandle, DWORD dwFileID, DWORD dwFileTotalSize, DWORD dwDownLoadSize, int nError, LDWORD dwUser, void* pReserved);

// CLIENT_DownLoadMultiFile �ӿڲ���
typedef struct tagNET_DOWNLOADFILE_INFO
{
	DWORD               dwSize;
	DWORD               dwFileID;                       // Document ID, assigned by the user
	int                 nFileSize;                      // Query to the file size
	char                szSourceFilePath[MAX_PATH];     // Query to the file path
	char                szSavedFileName[MAX_PATH];      // Save the file path
}NET_DOWNLOADFILE_INFO;

typedef struct tagNET_IN_DOWNLOAD_MULTI_FILE
{
	DWORD               dwSize;
    int                 nFileCount;                     // The number of files to be downloaded
	NET_DOWNLOADFILE_INFO* pFileInfos;                  // File information to be downloaded
	fMultiFileDownLoadPosCB cbPosCallBack;              // Progress callback function
	LDWORD              dwUserData;                     // User data
}NET_IN_DOWNLOAD_MULTI_FILE;

typedef struct tagNET_OUT_DOWNLOAD_MULTI_FILE
{
	DWORD               dwSize;
	LLONG               lDownLoadHandle;                // Download the handle
}NET_OUT_DOWNLOAD_MULTI_FILE;

typedef struct __NET_VIDEOANALYSE_STATE
{
	DWORD              dwSize;
	DWORD              dwProgress;                   // Analysis of progress��0-100
	char               szState[DH_COMMON_STRING_64]; // Channel Status,Running"��Run��"Stop"��Stop��"NoStart"��Not started��"Failed"��failed ��"Successed"��successed
	char               szFailedCode[DH_COMMON_STRING_64]; // Error code
}NET_VIDEOANALYSE_STATE;

//Real-time video analysis progress status callbacks
typedef int (CALLBACK *fVideoAnalyseState)(LLONG lAttachHandle, NET_VIDEOANALYSE_STATE* pAnalyseStateInfos, LDWORD dwUser, void* pReserved);

// CLIENT_AttachVideoAnalyseState  Interface input parameters
typedef struct __NET_IN_ATTACH_VIDEOANALYSE_STATE 
{
	DWORD              dwSize;
	int                nChannleId;            // Channel number
	fVideoAnalyseState cbVideoAnalyseState;   // Video analysis status callback function
	LDWORD             dwUser;                //User Information
}NET_IN_ATTACH_VIDEOANALYSE_STATE;

// CLIENT_AttachVideoAnalyseState  Interface output parameters
typedef struct __NET_OUT_ATTACH_VIDEOANALYSE_STATE 
{
	DWORD              dwSize;
	LLONG              lAttachHandle;         // Analysis of the progress of the analysis progress handle that uniquely identifies a particular channel
}NET_OUT_ATTACH_VIDEOANALYSE_STATE;

///////////////////////////////// IVS server video analysis module /////////////////////////////////
// Video analysis  report result detect type definition
#define NET_DIAGNOSIS_DITHER                    "VideoDitherDetection"                       // Video vibration detect  Corresponding structure body(NET_VIDEO_DITHER_DETECTIONRESULT)
#define NET_DIAGNOSIS_STRIATION                 "VideoStriationDetection"                    // Video stria detect  Corresponding structure body(NET_VIDEO_STRIATION_DETECTIONRESULT)
#define NET_DIAGNOSIS_LOSS                      "VideoLossDetection"                         // Video loss detect  Corresponding structure body(NET_VIDEO_LOSS_DETECTIONRESULT)
#define NET_DIAGNOSIS_COVER                     "VideoCoverDetection"                        // Camera masking detect Corresponding structure body(NET_VIDEO_COVER_DETECTIONRESULT)
#define NET_DIAGNOSIS_FROZEN                    "VideoFrozenDetection"                       // Video freeze detect Corresponding structure body(NET_VIDEO_FROZEN_DETECTIONRESULT)
#define NET_DIAGNOSIS_BRIGHTNESS                "VideoBrightnessDetection"                   // Video brightness abnormal detect Corresponding structure body(NET_VIDEO_BRIGHTNESS_DETECTIONRESULT)
#define NET_DIAGNOSIS_CONTRAST                  "VideoContrastDetection"                     // Video contrast abnormal detect  Corresponding structure body(NET_VIDEO_CONTRAST_DETECTIONRESULT)
#define NET_DIAGNOSIS_UNBALANCE                 "VideoUnbalanceDetection"                    // Video color cast detect Corresponding structure body(NET_VIDEO_UNBALANCE_DETECTIONRESULT)
#define NET_DIAGNOSIS_NOISE                     "VideoNoiseDetection"                        // Video noise detect Corresponding structure body(NET_VIDEO_NOISE_DETECTIONRESULT)
#define NET_DIAGNOSIS_BLUR                      "VideoBlurDetection"                         // Video blur detect Corresponding structure body(NET_VIDEO_BLUR_DETECTIONRESULT)
#define NET_DIAGNOSIS_SCENECHANGE               "VideoSceneChangeDetection"                  // Video scene change detect Corresponding structure body(NET_VIDEO_SCENECHANGE_DETECTIONRESULT)

enum NET_STATE_TYPE
{
	NET_EM_STATE_ERR,        // Others
	NET_EM_STATE_NORMAL,     // "Normal"  
	NET_EM_STATE_WARNING,    // "Warning" 
	NET_EM_STATE_ABNORMAL,   // "Abnormal" 
};
// video stream type
enum NET_STREAM_TYPE
{
	NET_EM_STREAM_ERR,                   // Others
	NET_EM_STREAM_MAIN,					// "Main"-Main stream
	NET_EM_STREAM_EXTRA_1,				// "Extra1"-Extra stream 1
	NET_EM_STREAM_EXTRA_2,				// "Extra2"-Extra stream 2
	NET_EM_STREAM_EXTRA_3,				// "Extra3"-Extra stream 3
	NET_EM_STREAM_SNAPSHOT,				// "Snapshot"-Snap bit stream
	NET_EM_STREAM_OBJECT,				// "Object"-Object stream
	NET_EM_STREAM_AUTO,                 // "Auto"
	NET_EM_STREAM_PREVIEW,              // "Preview"
	NET_EM_STREAM_NONE,					// No video stream (audio only)
};
// Video diagnosis type
enum NET_VIDEODIAGNOSIS_RESULT_TYPE
{
	NET_EM_ROTATION   ,					// "Rotation"	-Video analysis of polling
	NET_EM_REAL		  ,					// "Real" -Real-time video analysis
	NET_EM_NR_UNKNOW  ,					// Undefined
};
//Video causes of diagnostic error
enum NET_VIDEODIAGNOSIS_FAIL_TYPE
{
	NET_EM_NO_ERROR				 ,			// Diagnostic success
	NET_EM_DISCONNECT			 ,			// "Disconnect"				- End devices can be connected
	NET_EM_CH_NOT_EXIST			 ,			// "ChannelNotExist"		- Channel does not exist
	NET_EM_LOGIN_OVER_TIME		 ,			// "LoginOverTime"			- Login Timeout
	NET_EM_NO_VIDEO				 ,			// "NoVideo"				- No video successful login
	NET_EM_NO_RIGHT				 ,			// "NoRight"				- No operating authority
	NET_EM_PLATFROM_LOGIN_FAILED ,			// "PlatformLoginFailed"	- Login failed platform
	NET_EM_PLATFROM_DISCONNECT 	 ,			// "PlatformDisconnect"		- Disconnect platform
	NET_EM_GET_STREAM_OVER_TIME  ,			// "GetStreamOverTime"		- Get stream timeout
	NET_EM_NF_UNKNOW			 ,			// Other reasons, as detailed in the structure described in the reason for the failure 
};

// General long character ended with '\0'
typedef struct tagNET_ARRAY
{
	DWORD                dwSize;                         // Current structure body size 
	char*                pArray;                         // Buffer zone. Now the min value is 260 byte.Caller shall apply for the memory. The filling in data shall ended with '\0'.
	DWORD                dwArrayLen;                     // Buffer space length
}NET_ARRAY;

// Video analysis result report general data
typedef struct tagNET_VIDEODIAGNOSIS_COMMON_INFO
{
	DWORD                dwSize;                         // Current structure body size 
	int                  nDiagnosisID;                   // Detect channel
	NET_ARRAY            stProject;                      // Project name
	NET_ARRAY            stTask;                         // Task name
	NET_ARRAY            stProfile;                      // Parameter list name
	NET_ARRAY            stDeviceID;                     // Device unique ID
	NET_TIME             stStartTime;                    // Start time
	NET_TIME             stEndTime;                      // End time
	int                  nVideoChannelID;                // Video channel No. The channel of the front-end device such as DVR,IPC.
	NET_STREAM_TYPE      emVideoStream;                  // Video bit stream
	NET_VIDEODIAGNOSIS_RESULT_TYPE	emResultType;					// Diagnosis type
	BOOL							bCollectivityState;             // Diagnostic results
	NET_VIDEODIAGNOSIS_FAIL_TYPE	emFailedCause;					// Reasons for failure
	char                            szFailedCode[DH_COMMON_STRING_64]; // Describe the reason for the failure
}NET_VIDEODIAGNOSIS_COMMON_INFO;

// The result of detect type (NET_DIAGNOSIS_DITHER)  Video vibration detect -- Video change,wind or vibration,rotation including the PTZ movement.
typedef struct tagNET_VIDEO_DITHER_DETECTIONRESULT
{
	DWORD                dwSize;                         // Current structure body size 
	int                  nValue;                         // Detect result value 
	NET_STATE_TYPE       emState;                        // Detect result status  Usually smaller than is normal. Larger than is abnormal and the value in the middle is warning. 
	int                  nDuration;                      // Status lasts time  Detect item last time. It is null right now. 
}NET_VIDEO_DITHER_DETECTIONRESULT;

// The result of detect type (NET_DIAGNOSIS_STRIATION) Video stria detect  -- There is abnormal stria on the camera resulting from the interference.
typedef struct tagNET_VIDEO_STRIATION_DETECTIONRESULT
{
	DWORD                dwSize;                         // Current structure body size 
	int                  nValue;                         // Detect result value 
	NET_STATE_TYPE       emState;                        // Detect result status
	int                  nDuration;                      // Status lasts time
}NET_VIDEO_STRIATION_DETECTIONRESULT;

// The result of detect type (NET_DIAGNOSIS_LOSS) Video loss detect  -- Result from power failure or disconnection.
typedef struct tagNET_VIDEO_LOSS_DETECTIONRESULT
{
	DWORD                dwSize;                         // Current structure body size 
	NET_STATE_TYPE       emState;                        // Detect result status
	int                  nDuration;                      // Status lasts time
}NET_VIDEO_LOSS_DETECTIONRESULT;

// The result of detect type (NET_DIAGNOSIS_COVER) Camera masking detect -- The camera masking occurred
typedef struct tagNET_VIDEO_COVER_DETECTIONRESULT
{
	DWORD                dwSize;                         // Current structure body size 
	int                  nValue;                         // Detect result value 
	NET_STATE_TYPE       emState;                        // Detect result status
	int                  nDuration;                      // Status lasts time
}NET_VIDEO_COVER_DETECTIONRESULT;

// The result of detect type (NET_DIAGNOSIS_FROZEN) Video freeze detect -- The video idles for specified time is regarded as freeze.
typedef struct tagNET_VIDEO_FROZEN_DETECTIONRESULT
{
	DWORD                dwSize;                         // Current structure body size 
	NET_STATE_TYPE       emState;                        // Detect result status
	int                  nDuration;                      // Status lasts time
}NET_VIDEO_FROZEN_DETECTIONRESULT;

// The result of detect type (NET_DIAGNOSIS_BRIGHTNESS) Video brightness abnormal detect --The following contents are some camera improper setup detect.
typedef struct tagNET_VIDEO_BRIGHTNESS_DETECTIONRESULT
{
	DWORD                dwSize;                         // Current structure body size 
	int                  nValue;                         // Detect result value 
	NET_STATE_TYPE       emState;                        // Detect result status
	int                  nDuration;                      // Status lasts time
}NET_VIDEO_BRIGHTNESS_DETECTIONRESULT;

// The result of detect type (NET_DIAGNOSIS_CONTRAST) Video contrast abnormal detect
typedef struct tagNET_VIDEO_CONTRAST_DETECTIONRESULT
{
	DWORD                dwSize;                         // Current structure body size 
	int                  nValue;                         // Detect result value 
	NET_STATE_TYPE       emState;                        // Detect result status
	int                  nDuration;                      // Status lasts time
}NET_VIDEO_CONTRAST_DETECTIONRESULT;

// The result of detect type (NET_DIAGNOSIS_UNBALANCE) Video color cast detect
typedef struct tagNET_VIDEO_UNBALANCE_DETECTIONRESULT
{
	DWORD                dwSize;                         // Current structure body size 
	int                  nValue;                         // Detect result value 
	NET_STATE_TYPE       emState;                        // Detect result status
	int                  nDuration;                      // Status lasts time
}NET_VIDEO_UNBALANCE_DETECTIONRESULT;

// The result of detect type (NET_DIAGNOSIS_NOISE) Video noise detect
typedef struct tagNET_VIDEO_NOISE_DETECTIONRESULT
{
	DWORD                dwSize;                         // Current structure body size 
	int                  nValue;                         // Detect result value 
	NET_STATE_TYPE       emState;                        // Detect result status
	int                  nDuration;                      // Status lasts time
}NET_VIDEO_NOISE_DETECTIONRESULT;

// The result of detect type (NET_DIAGNOSIS_BLUR) Video blur detect
typedef struct tagNET_VIDEO_BLUR_DETECTIONRESULT
{
	DWORD                dwSize;                         // Current structure body size 
	int                  nValue;                         // Detect result value 
	NET_STATE_TYPE       emState;                        // Detect result status
	int                  nDuration;                      // Status lasts time
}NET_VIDEO_BLUR_DETECTIONRESULT;

// The result of detect type (NET_DIAGNOSIS_SCENECHANGE) Video scene change detect
typedef struct tagNET_VIDEO_SCENECHANGE_DETECTIONRESULT
{
	DWORD                dwSize;                         // Current structure body size 
	int                  nValue;                         // Detect result value 
	NET_STATE_TYPE       emState;                        // Detect result status
	int                  nDuration;                      // Status lasts time
}NET_VIDEO_SCENECHANGE_DETECTIONRESULT;

typedef struct tagNET_DIAGNOSIS_RESULT_HEADER
{
	DWORD                dwSize;                         // Current structure body size 
	
	char                 szDiagnosisType[MAX_PATH];      // Analysis type. Please refer to dhnetsdk.h for "video analysis report result type definition"  such as NET_DIAGNOSIS_DITHER
	int                  nDiagnosisTypeLen;              // The structure body size of current analysis type
}NET_DIAGNOSIS_RESULT_HEADER;

// cbVideoDiagnosis Call parameter type
typedef struct tagNET_REAL_DIAGNOSIS_RESULT
{
	DWORD                dwSize;                         // Current structure body size 
	
    NET_VIDEODIAGNOSIS_COMMON_INFO* pstDiagnosisCommonInfo;  //Video analysisi general info
	
	int					 nTypeCount;					 // Analysis result data analysis type amount
	void*                pDiagnosisResult;               // The analysis result data for once. The format is as NET_DIAGNOSIS_RESULT_HEADER+analysis type1+NET_DIAGNOSIS_RESULT_HEADER+analysis type 2+...
	DWORD                dwBufSize;                      // Buffer length
}NET_REAL_DIAGNOSIS_RESULT;

// Video analysis result report call function
typedef int (CALLBACK *fRealVideoDiagnosis)(LLONG lDiagnosisHandle, NET_REAL_DIAGNOSIS_RESULT* pDiagnosisInfo, void* pBuf, int nBufLen, LDWORD dwUser);

//The input parameter of interface  CLIENT_StartVideoDiagnosis
typedef struct tagNET_IN_VIDEODIAGNOSIS
{
	DWORD                dwSize;                         // Current structure body size 
	int                  nDiagnosisID;                   // Video analysis ID begins with 0
	DWORD                dwWaitTime;                     // Interface time out waiting time
	
	fRealVideoDiagnosis  cbVideoDiagnosis;               // Video analysis result call function.
	LDWORD                dwUser;                         // User customized parameter
}NET_IN_VIDEODIAGNOSIS;

// The output parameter of interface CLIENT_StartVideoDiagnosis 
typedef struct tagNET_OUT_ATTACH_REALDIAGNOSIS
{
	DWORD                dwSize;                         // Current structure body size 
	LLONG                 lDiagnosisHandle;               // Subscribe handle
}NET_OUT_VIDEODIAGNOSIS;

// The input parameter of interface CLIENT_StartFindDiagnosisResult 
typedef struct tagNET_IN_FIND_DIAGNOSIS
{
	DWORD                dwSize;                         // Current structure body size 
	int                  nDiagnosisID;                   // Video analysis ID begins with 0
	DWORD                dwWaitTime;                     // Interface time out waiting time
	
	NET_ARRAY            stuDeviceID;                    // Device unique ID.pArray:NULL means does not search by device ID.
	NET_TIME             stuStartTime;                   // Start time
	NET_TIME             stuEndTime;                     // End time
	int                  nVideoChannel;                  // Video channel No.,-1: All channel No.
	int                  nTypeCount;                     // Analysis type amount.0:Do not use analysis type to search.
	NET_ARRAY*           pstDiagnosisTypes;              // Analysis type array. It is the analysis type to be searched. 
	char                 szProjectName[MAX_PATH];        // Project name
} NET_IN_FIND_DIAGNOSIS;

// The output parameter of interface CLIENT_StartFindDiagnosisResult
typedef struct tagNET_OUT_FIND_DIAGNOSIS
{
	DWORD                dwSize;                         // Current structure body size 
	LLONG                 lFindHandle;                    // Search handle
	DWORD                dwTotalCount;                   // The total amount that matched the criteria
}NET_OUT_FIND_DIAGNOSIS;

// The input parameter of interface CLIENT_DoFindDiagnosisResult 
typedef struct tagNET_IN_DIAGNOSIS_INFO
{
	DWORD                dwSize;                         // Current structure body size 
	int                  nDiagnosisID;                   // Video analysis ID begins with 0
	DWORD                dwWaitTime;                     // Interface time out waiting time
	
	int                  nFindCount;                     // The video analysis result amount of each search
	int                  nBeginNumber;                   // Search initial SN 0<=beginNumber<= totalCount-1
}NET_IN_DIAGNOSIS_INFO;

// CLIENT_StartRealTimeProject Interface input parameters
typedef struct tagNET_IN_START_RT_PROJECT_INFO
{
	DWORD                dwSize;                         // This structure size
	char*                pszProjectInfo;                 // Real-time schedule information by calling dhconfigsdk.dll get the package in the interface CLIENT_PacketData
                            	                         // Package command corresponding CFG_CMD_VIDEODIAGNOSIS_REALPROJECT                      
}NET_IN_START_RT_PROJECT_INFO;

// CLIENT_StartRealTimeProject Interface output parameters
typedef struct tagNET_OUT_START_RT_PROJECT_INFO
{
	DWORD                dwSize;                         // This structure size
}NET_OUT_START_RT_PROJECT_INFO;

// CLIENT_StopRealTimeProject Interface output parameters
typedef struct tagNET_IN_STOP_RT_PROJECT_INFO
{
	DWORD                dwSize;                         // This structure size
	char*                 pszProjectNames;               // Program name to the name of the separator && as the example��Project1&&Project2   
}NET_IN_STOP_RT_PROJECT_INFO; 

// CLIENT_StopRealTimeProject Interface output parameters
typedef struct tagNET_OUT_STOP_RT_PROJECT_INFO
{
	DWORD                dwSize;                         // This structure size
}NET_OUT_STOP_RT_PROJECT_INFO;

// carport light info
typedef struct tagNET_CARPORTLIGHT_INFO
{
	DWORD               dwSize;  
	NET_CARPORTLIGHT_TYPE emType;                        // carport type
	NET_CARPORTLIGHT_MODE emMode;                        // light way
}NET_CARPORTLIGHT_INFO;

// carpor light status
typedef struct tagNET_CARPORTLIGHT_STATUS
{
	DWORD                dwSize;                         // struct size 
	int                  nLightNum;                      // light num
	NET_CARPORTLIGHT_INFO stuLights[DH_MAX_CARPORTLIGHT_NUM]; // light info,don't repeat
	int                  nFiringTime;                   // firing time(s)
	int                  nHoldTime;                     // hold time(s), -1 means keep,0 auto control
}NET_CARPORTLIGHT_STATUS;

// CLIENT_GetCarPortLightStatus's interface input
typedef struct tagNET_IN_GET_CARPORTLIGHT_STATUS
{
	DWORD                dwSize;                         // struct size 
	int                  nChannelId;                     // channel ID
}NET_IN_GET_CARPORTLIGHT_STATUS;

// CLIENT_GetCarPortLightStatus's interface output
typedef struct tagNET_OUT_GET_CARPORTLIGHT_STATUS
{
	DWORD                dwSize;                         //struct size 
	NET_CARPORTLIGHT_STATUS stuLightStatus;              //light status
}NET_OUT_GET_CARPORTLIGHT_STATUS;

// CLIENT_SetCarPortLightStatus's interface input
typedef struct tagNET_IN_SET_CARPORTLIGHT_STATUS
{
	DWORD                dwSize;                         // struct size 
	int                  nChannelId;
	NET_CARPORTLIGHT_STATUS stuLightStatus;              // light status
}NET_IN_SET_CARPORTLIGHT_STATUS;

// CLIENT_SetCarPortLightStatus's interface output
typedef struct tagNET_OUT_SET_CARPORTLIGHT_STATUS
{
	DWORD                dwSize;                         // struct size 
	
}NET_OUT_SET_CARPORTLIGHT_STATUS;
typedef struct tagNET_DEV_VIDEODIAGNOSIS_MULTI_INFO
{
	DWORD                dwSize;                         // Current structure body size 
	
    NET_VIDEODIAGNOSIS_COMMON_INFO*          pstDiagnosisCommonInfo;  //Video analysis general info. You need to allocate the following pointer of the structure body.
	
	BOOL                 abDither;						 // It is to stand for current analysis item is valid or not in this result.
	NET_VIDEO_DITHER_DETECTIONRESULT*        pstDither;  //  Video vibration detect
	
	BOOL                 abStration;
	NET_VIDEO_STRIATION_DETECTIONRESULT*     pstStration;// Video stria detect
	
	BOOL                 abLoss;
	NET_VIDEO_LOSS_DETECTIONRESULT*          pstLoss;    // Video loss detect 
	
	BOOL                 abCover;
	NET_VIDEO_COVER_DETECTIONRESULT*         pstCover;   // Camera masking detect
	
	BOOL                 abFrozen;
	NET_VIDEO_FROZEN_DETECTIONRESULT*        pstFrozen;  // Video freeze detect
	
	BOOL                 abBrightness;
	NET_VIDEO_BRIGHTNESS_DETECTIONRESULT*    pstBrightness;// Video brightness abnormal detect 
	
	BOOL                 abContrast;
	NET_VIDEO_CONTRAST_DETECTIONRESULT*      pstContrast;//Video contrast abnormal detect 
	
	BOOL                 abUnbalance;
	NET_VIDEO_UNBALANCE_DETECTIONRESULT*     pstUnbalance;//  Video color cast detect
	
	BOOL                 abNoise;
	NET_VIDEO_NOISE_DETECTIONRESULT*         pstNoise;   //Video noise detect 
	
	BOOL                 abBlur;
	NET_VIDEO_BLUR_DETECTIONRESULT*          pstBlur;    // Video blur detect
	
	BOOL                 abSceneChange;
	NET_VIDEO_SCENECHANGE_DETECTIONRESULT*   pstSceneChange;// Video scene change detect 
}NET_VIDEODIAGNOSIS_RESULT_INFO;

// The output parameter of interface CLIENT_DoFindDiagnosisResult
typedef struct tagNET_OUT_DIAGNOSIS_INFO
{
	DWORD                dwSize;                         // Current structure body size 
	
	int                  nInputNum;                      // The amount of the NET_DEV_RESULT_VIDEODIAGNOSIS_INFO you allocate
	int                  nReturnNum;                     // Returned amount
	NET_VIDEODIAGNOSIS_RESULT_INFO*  pstDiagnosisResult; // Result data You need to allocate the pointer of the structure body
}NET_OUT_DIAGNOSIS_INFO;

///////////////////////////////// master-slave device control module////////////////////////////
///////////////////////////////// CLIENT_OperateMasterSlaveDevice /////////////////////////////////
#define        MASTERSLAVE_CMD_START                   "masterSlaveTracker.start"                   // start
#define        MASTERSLAVE_CMD_STOP                    "masterSlaveTracker.stop"                    // stop
#define        MASTERSLAVE_CMD_MANUALTRACK             "masterSlaveTracker.manualSelectObjectTrack" // manual select object
#define        MASTERSLAVE_CMD_POINTTRACK              "masterSlaveTracker.selectPointTrack"        // points tracking
#define        MASTERSLAVE_CMD_GETCALIBPOINTS          "masterSlaveTracker.getCalibratePoints"      // get calibrate points
#define        MASTERSLAVE_CMD_CALIBRATE               "masterSlaveTracker.calibrate"               // save calibrate points
#define        MASTERSLAVE_CMD_ADDCALIBPOINT           "masterSlaveTracker.addCalibratePoint"       // add calibrate points
#define        MASTERSLAVE_CMD_REMOVECALIBPOINT        "masterSlaveTracker.removeCalibratePoint"    // remove calibrate points
#define        MASTERSLAVE_CMD_MARKMAXZOOM             "masterSlaveTracker.markSceneMaxZoom"        // mark scene max zoom

// master-slave device pair points
typedef struct __NET_MS_PAIRPOINTS
{
	int                nStructSize;
    DH_POINT           stMasterPoint;    // master point,range[0,8192]
    DH_POINT           stSlavePoint;     // slave point,range[0,8192]
}NET_MS_PAIRPOINTS;
//MASTERSLAVE_CMD_START
typedef struct __NET_IN_MS_START
{
	int                nStructSize;
}NET_IN_MS_START;

//MASTERSLAVE_CMD_START
typedef struct __NET_OUT_MS_START
{
	int                nStructSize;
}NET_OUT_MS_START;

//MASTERSLAVE_CMD_STOP
typedef struct __NET_IN_MS_STOP
{
	int                nStructSize;
}NET_IN_MS_STOP;

//MASTERSLAVE_CMD_STOP
typedef struct __NET_OUT_MS_STOP
{
	int                nStructSize;
}NET_OUT_MS_STOP;

//MASTERSLAVE_CMD_MANUALTRACK
typedef struct __NET_IN_MS_MANUALTRACK
{
	int                nStructSize;
	DWORD              dwObject;          // -1 any position of the entire region, >=0 specify the object(can be obtained from the device to return to intelligent frame)
}NET_IN_MS_MANUALTRACK;

//MASTERSLAVE_CMD_MANUALTRACK
typedef struct __NET_OUT_MS_MANUALTRACK
{
	int                nStructSize;
}NET_OUT_MS_MANUALTRACK;

//MASTERSLAVE_CMD_POINTTRACK
typedef struct __NET_IN_MS_POINTTRACK
{
	int                nStructSize;
	DH_POINT           stTrackerPoint;    // Tracker point,range[0,8192]
}NET_IN_MS_POINTTRACK;

//MASTERSLAVE_CMD_POINTTRACK
typedef struct __NET_OUT_MS_POINTTRACK
{
	int                nStructSize;
}NET_OUT_MS_POINTTRACK;

//MASTERSLAVE_CMD_GETCALIBPOINTS
typedef struct __NET_IN_MS_GETCALIBPOINTS
{
	int                nStructSize;
}NET_IN_MS_GETCALIBPOINTS;

//MASTERSLAVE_CMD_GETCALIBPOINTS
typedef struct __NET_OUT_MS_GETCALIBPOINTS
{
	int                        nStructSize;
	int                        nPointsNum;                           // practicality points num
	NET_MS_PAIRPOINTS          stPairPoints[MAX_CALIBPOINTS_NUM];    // pair points
}NET_OUT_MS_GETCALIBPOINTS;

//MASTERSLAVE_CMD_CALIBRATE
typedef struct __NET_IN_MS_CALIBRATE
{
	int                nStructSize;
}NET_IN_MS_CALIBRATE;

//MASTERSLAVE_CMD_CALIBRATE
typedef struct __NET_OUT_MS_CALIBRATE
{
	int                nStructSize;      
}NET_OUT_MS_CALIBRATE;

//MASTERSLAVE_CMD_ADDCALIBPOINT
typedef struct __NET_IN_MS_ADDCALIBPOINT
{
	int                        nStructSize;
	BOOL                       bSlavePointEn;        // enable, TURE able;FASLE disable
	NET_MS_PAIRPOINTS          stPairPoints;         // master-slave camera pair point
}NET_IN_MS_ADDCALIBPOINT;

//MASTERSLAVE_CMD_ADDCALIBPOINT
typedef struct __NET_OUT_MS_ADDCALIBPOINT
{
	int                nStructSize;
	DH_POINT           stPoint;      // slave point
}NET_OUT_MS_ADDCALIBPOINT;


//MASTERSLAVE_CMD_REMOVECALIBPOINT
typedef struct __NET_IN_MS_REMOVECALIBPOINT
{
	int                nStructSize;
	DH_POINT           stPoint;      // master point
}NET_IN_MS_REMOVECALIBPOINT;

//MASTERSLAVE_CMD_REMOVECALIBPOINT
typedef struct __NET_OUT_MS_REMOVECALIBPOINT
{
	int                nStructSize;      
}NET_OUT_MS_REMOVECALIBPOINT;

//MASTERSLAVE_CMD_MARKMAXZOOM
typedef struct __NET_IN_MS_MARKMAXZOOM
{
	int                nStructSize; 
}NET_IN_MS_MARKMAXZOOM;

//MASTERSLAVE_CMD_MARKMAXZOOM
typedef struct __NET_OUT_MS_MARKMAXZOOM
{
	int                nStructSize; 
}NET_OUT_MS_MARKMAXZOOM;

///////////////////////////////// CLIENT_OperateCommDevice Interface parameters defined /////////////////////////////////
#define		COMMDEV_GET_EXTERNLDATA							"devComm.getExternalData"		//��ȡ�����������	���ڵ�����ͨ������

#define        MAX_EXTERN_DATA_LEN                2048

//COMMDEV_GET_EXTERNLDATA Input parameters
typedef struct __COMMDEV_IN_GET_EXTERNDATA
{
    DWORD        dwSize;

}COMMDEV_IN_GET_EXTERNDATA;

//COMMDEV_GET_EXTERNLDATA Output parameters
typedef struct __COMMDEV_OUT_GET_EXTERNDATA
{
	DWORD		dwSize;
	char		szGetData[MAX_EXTERN_DATA_LEN];				//External serial devices to collect data (data format based on custom projects)
}COMMDEV_OUT_GET_EXTERNDATA;


/////////////////////////////////CLIENT_OperateVideoAnalyseDevice Interface parameter /////////////////////////////////
#define     NET_SET_MODULESTATE                         "devVideoAnalyse.setModuleState"                        // Set module status
#define		NET_TEST_WITH_SCREENPOINTS					"devVideoAnalyse.testCalibrateWithScreenPoints"			// Detect depth of the field according to the coordinates on the screen. It is the actual distance between these two points. Please complete the parameter setup before you test.
#define		NET_TEST_WITH_METRICLENGTH					"devVideoAnalyse.testCalibrateWithMetricLength"		    // Detect parameter of depth of the field according to actual lenght,start point,and direction coordinates.
#define		NET_GET_INTERNALOPTIONS						"devVideoAnalyse.getInternalOptions"					// Get internal option
#define		NET_TUNE_INTERNALOPTIONS					"devVideoAnalyse.tuneInternalOptions"					// Debug internal item
#define		NET_RESET_INTERNALOPTIONS					"devVideoAnalyse.resetInternalOptions"					// Reset all debug internal item
#define     NET_SET_PTZ_PRESET_STATUS                   "devVideoAnalyse.setPtzPresetStatus"                    // ��ȡԤ�õ���Ϣ

// Line type
enum NET_EM_LINETYPE
{
	NET_EM_HORIZONTAL,
	NET_EM_VERTICAL,
};

//PTZ positioning information alarm
typedef struct
{
	int     nChannelID;             // Channel number
	int     nPTZPan;                // Horizontal movement of the head position, effective range: [0,3600]
	int     nPTZTilt;               // PTZ vertical position, the effective range: [-1800,1800]
	int     nPTZZoom;               // PTZ iris position changes, effective range: [0,128]
	BYTE    bState;                 // PTZ motion, 0 - Unknown 1 - Movement 2 - Idle
	BYTE    bAction;                // PTZ movement, 0 - preset 1 - line scan 2 - Cruise 3 - patrol track 4 - horizontal rotation
	BYTE    bFocusState;            // PTZ focus state, 0 - unknown 1 - state of motion 2 - Idle
	BYTE    bEffectiveInTimeSection; //In the period of validity of the preset state
									 //If the current is preset reported preset period of time, compared with one, otherwise 0
	int     nPtzActionID;           // Cruise ID number
	DWORD   dwPresetID;             // PTZ preset number where
	float   fFocusPosition;         // Focus position
	BYTE	bZoomState;				// ZOOM PTZ status, 0 - Unknown,1-ZOOM, 2 - Idle
	BYTE	bReserved[3];			// Alignment
	DWORD   dwSequence;             // Packet sequence number, used to verify whether the loss
	DWORD   dwUTC;                  // Corresponding UTC (1970-1-1 00:00:00) seconds.
	int     reserved[249];          // Reserved field
}DH_PTZ_LOCATION_INFO;

// NET_SET_PTZ_PRESET_STATUS
typedef struct __NET_IN_PTZ_PRESET_STATUS
{
    DWORD                    dwSize;
    DH_PTZ_LOCATION_INFO     stPTZStatus;
}NET_IN_PTZ_PRESET_STATUS;

typedef struct __NET_OUT_PTZ_PRESET_STATUS
{
    DWORD    dwSize;
}NET_OUT_PTZ_PRESET_STATUS;

// NET_TEST_WITH_SCREENPOINTS
typedef struct __NET_IN_CALIBRATE_BY_POINTS
{
	DWORD               dwSize;                 // Current structure body size 
	DH_POINT            stStartPoint;           // Start point of the line 	 The point of the line is within [0,8191].
	DH_POINT            stEndPoint;             // End point 	The point of the line is within [0,8191]
	NET_EM_LINETYPE     emType;                 // Line type	em_Horizontal("Horizontal")  em_Vertical("Vertical")
}NET_IN_CALIBRATE_BY_POINTS;

typedef struct __NET_OUT_CALIBRATE_BY_POINTS
{
	DWORD               dwSize;                 // Current structure body size 	
	double              dlength;                // The actual length between these two points. Unit is meter.
}NET_OUT_CALIBRATE_BY_POINTS;

// NET_TEST_WITH_METRICLENGTH
typedef struct __NET_IN_CALIBRATE_BY_LENGTH
{
	DWORD               dwSize;                 // Current structure body size 
	double				nLength;				// The actual length between these two points. Unit is meter.
	DH_POINT			stStartPoint;			// Start point of the line	The point of the line is within [0,8191]
	DH_POINT			stDirection;			// Line direction coordinates	For vertical line ony. The point of the line is within [0,8191]
	NET_EM_LINETYPE		emType;					// Line type	em_Horizontal("Horizontal")  em_Vertical("Vertical")				
}NET_IN_CALIBRATE_BY_LENGTH;

typedef struct __NET_OUT_CALIBRATE_BY_LENGTH
{
	DWORD               dwSize;                 // Current structure body size 
	DH_POINT			stEndPoint;				// Coordinates of the end of the line on the screen. 	The point of the line is within[0,8191]	
}NET_OUT_CALIBRATE_BY_LENGTH;

typedef struct __NET_INTERNAL_OPTION
{
	DWORD               dwSize;                 // Current structure body size 
	int					nIndex;					// Item SN
	NET_ARRAY           stKey;				    // Option name. 256 bytes including'\0'. Encryption data
	int					nValue;					// Option value
	int					nMinValue;				// Min value
	int					nMaxValue;				// Max value
}NET_INTERNAL_OPTION;

// Debug internal option NET_GET_INTERNALOPTIONS
typedef struct __NET_IN_GETINTERNAL_OPTIONS
{
	DWORD               dwSize;                 // Current structure body size
}NET_IN_GETINTERNAL_OPTIONS;

typedef struct __NET_OUT_GETINTERNAL_OPTIONS
{
	DWORD               dwSize;                 // Current structure body size 
	
	int					nTotalOptionNum;		// Option amount. Max value search capability.
	int					nReturnOptionNum;		// returned actual option amount.
	NET_INTERNAL_OPTION* pstInternalOption;	    // Option list. Caller shall allocate the address.
}NET_OUT_GETINTERNAL_OPTIONS;

// NET_TUNE_INTERNALOPTIONS
typedef struct __NET_IN_TUNEINTERNAL_OPTION
{
	DWORD               dwSize;                 // Current structure body size 
	int					nOptionCount;	    	// Option amount
	NET_INTERNAL_OPTION* pstInternalOption;	    // Option list. The amount stands for the capability.
}NET_IN_TUNEINTERNAL_OPTION;

typedef struct __NET_OUT_TUNEINTERNAL_OPTION
{
	DWORD               dwSize;                 // Current structure body size 
}NET_OUT_TUNEINTERNAL_OPTION;

// NET_RESET_INTERNALOPTIONS
typedef struct __NET_IN_RESETINTERNAL_OPTION
{
	DWORD               dwSize;                 // Current structure body size 
}NET_IN_RESETINTERNAL_OPTION;

typedef struct __NET_OUT_RESETINTERNAL_OPTION
{
	DWORD               dwSize;                 // Current structure body size 
}NET_OUT_RESETINTERNAL_OPTION;

// NET_SET_MODULESTATE
enum NET_EM_MODULESTATE
{
	NET_EM_MODULESTATE_OK,
	NET_EM_MODULESTATE_PAUSE,
};
typedef struct __NET_IN_SETMODULESTATE
{
	DWORD				dwSize;					// Current structure body size 
	NET_EM_MODULESTATE  emState;				// 0-Normal work.1-Pause. Need to rebuild background when you switch to the normal video since it may change.
	int                 nIndex;                 // Model SN.	-1 stands for all modules.
}NET_IN_SETMODULESTATE;

typedef struct __NET_OUT_SETMODULESTATE
{
	DWORD				dwSize;					// Current structure body size
}NET_OUT_SETMODULESTATE;

/////////////////////////////////CLIENT_DeleteDevConfig Interface parameter/////////////////////////////////
/*
 * CLIENT_DeleteDevConfig Input and output parameter
 */

enum NET_EM_CONFIGOPTION
{
	NET_EM_CONFIGOPTION_OK,
	NET_EM_CONFIGOPTION_NEEDRESTART,
	NET_EM_CONFIGOPTION_NEEDREBOOT=2,
	NET_EM_CONFIGOPTION_WRITEFILEERROR=4,
	NET_EM_CONFIGOPTION_CAPSNOTSUPPORT=8,
	NET_EM_CONFIGOPTION_VALIDATEFAILED=16,
};

typedef struct tagNET_IN_DELETECFG
{
	DWORD         dwSize;						// Structure body size
	char*         szCommand;                    // Configuration command
}NET_IN_DELETECFG;

typedef struct tagNET_OUT_DELETECFG
{
	DWORD         dwSize;                       // Structure body size
    int           nError;                       // The error code device returns
    int           nRestart;                     // Device reboot or not
	DWORD         dwOptionMask;                 // Option. Please refer to enumeration NET_EM_CONFIGOPTION
}NET_OUT_DELETECFG;

/////////////////////////////////CLIENT_GetMemberNames interface parameter/////////////////////////////////
/*
 * CLIENT_GetMemberNames Input and output parameter
 */
typedef struct tagNET_IN_MEMBERNAME
{
	DWORD         dwSize;                       // Structure body size
	char*         szCommand;                    // Configuration command
}NET_IN_MEMBERNAME;

typedef struct tagNET_OUT_MEMBERNAME
{
	DWORD         dwSize;                       // Structure body size 
	int           nError;                       // The error code device returns
    int           nRestart;                     // Device reboot or not	
	int           nTotalNameCount;              // Name amount. Fromt the capability set. Caller fill in.
	int           nRetNameCount;                // The returned actual name amount
	NET_ARRAY*    pstNames;                     // Name array. The caller apply for the memory. The amount is nTotalNameCount.
}NET_OUT_MEMBERNAME;

////////////////////////////////////video compression//////////////////////////////////////
// staff type
enum NET_EM_STAFF_TYPE
{
	NET_EM_STAFF_TYPE_ERR,
	NET_EM_STAFF_TYPE_HORIZONTAL,                       // "Horizontal" 
	NET_EM_STAFF_TYPE_VERTICAL,                         // "Vertical" 
	NET_EM_STAFF_TYPE_ANY,                              // "Any" 
	NET_EM_STAFF_TYPE_CROSS,                            // "Cross" 
};

// region type
enum NET_EM_CALIBRATEAREA_TYPE
{
	NET_EM_CALIBRATEAREA_TYPE_ERR,
	NET_EM_CALIBRATEAREA_TYPE_GROUD,		            // "Groud" 
	NET_EM_CALIBRATEAREA_TYPE_HORIZONTAL,	            // "Horizontal" 		
	NET_EM_CALIBRATEAREA_TYPE_VERTICAL,		            // "Vertical" 
	NET_EM_CALIBRATEAREA_TYPE_ANY,			            // "Any"
};

// The concentrated output data subtype
enum NET_EM_OUTPUT_SUB_TYPE
{
	NET_EM_OUTPUT_SUB_TYPE_ERR,
	NET_EM_OUTPUT_SUB_TYPE_NORMAL,                      // Normal playback speed (the parent type is concentrated Video valid)
	NET_EM_OUTPUT_SUB_TYPE_SYNOPSIS,                    //Concentrated by the playback speed (parent type is effective when concentrated video), quick release in the absence of an object, the object appears when playing at normal speed
};

// staff info
typedef struct tagNET_STAFF_INFO
{
	DH_POINT	        stuStartLocation;	            // start location
	DH_POINT	        stuEndLocation;		            // end location
	float			    nLenth;				            // length(m)
	NET_EM_STAFF_TYPE	emType;			                // type
}NET_STAFF_INFO;

// calibration area,common scenario use
typedef struct tagNET_CALIBRATEAREA_INFO
{
	int				    nLinePoint;					    // horizontal line point
	DH_POINT	        stuLine[DH_MAX_POLYLINE_NUM];	// horizontal line
	float			    fLenth;						    // lenth
	DH_POLY_POINTS	    stuArea;					    // area
	int				    nStaffNum;					    // number of vertical line
	NET_STAFF_INFO	    stuStaffs[DH_MAX_STAFF_NUM];    // vertical line         
	NET_EM_CALIBRATEAREA_TYPE emType;			        // area type
}NET_CALIBRATEAREA_INFO;

typedef struct tagNET_LOCALFILE_INFO
{
	DWORD			    dwSize;
	char			    szFilePath[MAX_PATH];	        // file path
}NET_LOCALFILE_INFO;

typedef struct tagNET_REMOTEFILE_INFO
{
	DWORD			    dwSize;
	char			    szIP[MAX_PATH];	                // the remote device IP
	unsigned int	    nPort;				            // the remote device  port
	char			    szName[DH_MAX_NAME_LEN];        // the remote device  userName
	char			    szPassword[DH_MAX_NAME_LEN];    // the remote device  password
	BYTE			    byProtocolType;			        // the remote device protocol type,2-the second generation of private 3-the third generation of private
	BYTE			    byReserved[3];			        // byte alignment
	char			    szFilePath[MAX_PATH];           // file path,when byProtocolType == 3 effective.
	int				    nStartCluster;		  	        // start cluster,when byProtocolType == 2 effective.
	int				    nDriverNo;				        // driver no.when byProtocolType == 2 effective.
	
}NET_REMOTEFILE_INFO;

// file path info
typedef struct tagNET_FILEPATH_INFO
{
	DWORD				dwSize;
	BOOL				bFileType;			            // TRUE: local server's file; FALSE: remote device's file
	NET_LOCALFILE_INFO	stuLocalFile;		            // local file, when bFileType==TRUE effective
	NET_REMOTEFILE_INFO	stuRemoteFile;		            // remote file, when bFileType==FALSE effective
}NET_FILEPATH_INFO;

// global param info
typedef struct tagNET_GLOBAL_INFO
{
	DWORD			    dwSize;
	char			    szSceneType[DH_MAX_NAME_LEN];	// scene type,only support "Normal"
	int				    nStaffNum;						// staff num
	NET_STAFF_INFO	    stuStaffs[DH_MAX_STAFF_NUM];	// staffs
	int				    nCalibrateAreaNum;				// number of calibrate area
	NET_CALIBRATEAREA_INFO stuCalibrateArea[DH_MAX_CALIBRATEBOX_NUM]; // calibrate area(if the filed does not exist,return the entire tegion)
	int				    nJitter;		                // vidicon jitter, range[0-100],the value of static camera shake,the more powerful shake,the bigger the value.
	BOOL			    bDejitter;		                // if start to wobble
}NET_GLOBAL_INFO;

// module info
typedef struct tagNET_MODULE_INFO
{
	DWORD			    dwSize;							// size
	BYTE			    bSensitivity;					// sensitivity,range[1-10],the lower the value the sensitivity.
	BYTE			    bReserved[3];
	int				    nDetectRegionPoint;				// detect region point
	DH_POINT		    stuDetectRegion[DH_MAX_POLYGON_NUM]; // detect region
	int				    nExcludeRegionNum;				// number of exclude region 
	DH_POLY_POINTS	    stuExcludeRegion[DH_MAX_EXCLUDEREGION_NUM];	// exclude region
}NET_MODULE_INFO;

// object filter info
typedef struct tagNET_OBJFILTER_INFO
{
	DWORD		        dwSize;						    // size
	NET_TIME	        stuStartTime;				    // start time
	NET_TIME	        stuEndTime;					    // end time
	char		        szObjectType[DH_MAX_NAME_LEN];	// object type, support for the following:
											            //"Unknown", "Human", "Vehicle",
											            //"Fire", "Smoke", "Plate", "HumanFace", "Container", "Animal", "TrafficLight", "PastePaper", "HumanHead", "Entity"
	char		        szObjectSubType[64];		    // object sub type,support for the following:
											            // Vehicle Category:"Unknown"  "Motor" "Non-Motor" "Bus""Bicycle"  "Motorcycle" 
											            // Plate Category: "Unknown" "Normal" "Yellow" "DoubleYellow" "Police" Armed" 
											            // "Military" "DoubleMilitary" "SAR" "Trainning" 
											            // "Personal" "Agri" "Embassy" "Moto" "Tractor" "Other"
											            // HumanFace Category:"Normal" "HideEye" "HideNose" "HideMouth" 
	DWORD		        dwRGBMainColor[DH_MAX_COLOR_NUM];	// main color,object to the person,said the upper part of the color,in bytes,is red,green,blue and diaphaneity,example:RGB value is(0,255,0),when diaphaneity = 0, the value = 0x00ff0000
	DWORD		        dwLowerBodyColor[DH_MAX_COLOR_NUM];	// object to the person,said the lower half of the color,in bytes,is red,green,blue and diaphaneity,example:RGB value is(0,255,0),when diaphaneity = 0, the value = 0x00ff0000
	int			        nMinSize;					    // min siez, m
	int			        nMaxSize;					    // max size, m
	int			        nMinSpeed;					    // min speed km/h
	int			        nMaxSpeed;					    // max speed km/h
	DWORD		        dwRGBMainColorNum;			    // object is the person,dwRGBMainColor effective number of colors
	DWORD		        dwLowerBodyColorNum;		    // object is the person, and bRGBMainColor==true,is the lower half of the effective color
	BOOL		        bRGBMainColor;				    // true-dwRGBMainColor the upper part of the color,dwLowerBodyColor the lower half of the color; false-dwRGBMainColor means main color,dwLowerBodyColor nullity
}NET_OBJFILTER_INFO;

// param of rule
typedef struct tagNET_VIDEOSYNOPSISRULE_INFO
{
	DWORD				dwSize;				            // struct size
	BYTE				byOutputType;		            // out tyope,1-snapshot;2-condensatal video;3-snapshot with video
	BYTE				byDensity;			            // density, 1-10, the more value,the density higher
	bool				bShowTime;			            // show time
	bool				bShowBox;			            // show object box
	bool				bEnableRecordResult;            // the result recoded or not
	BYTE				byReserved[3];
	NET_OBJFILTER_INFO	stuFilter[MAX_OBJFILTER_NUM];   // fileter info
	DWORD				dwObjFilterNum;		            // number of filter

	// NET_OUT_QUERY_VIDEOSYNOPSIS , CFG_CMD_ANALYSERULE transfer dhconfigsdk.dll get CLIENT_ParseData
	// NET_IN_ADD_VIDEOSYNOPSIS ,CFG_CMD_ANALYSERULEtransfer  dhconfigsdk.dll get CLIENT_PacketData
	char*		        szAnalyseRuleInfo;				// JSON rule info
	int                 nBufferLen;                     // NET_OUT_QUERY_VIDEOSYNOPSIS,apply szAnalyseRuleInfo length
	BOOL				bCustomResolution;				// Custom resolution enabled, TRUE-resolution is determined by emResolution, FALSE-concentrate production based on the original video resolution video
	CAPTURE_SIZE		emResolution;					// Resolution, bCustomResolution is TRUE only valid
	BOOL				bEnableMaxUsedCPU;				// Are maximize occupy CPU resource requirements
	BOOL				bEnableSmoothBorder;			// Target is smooth border
	NET_EM_OUTPUT_SUB_TYPE emOutputSubType;             // After concentrating the output data sub-type, see NET_EM_OUTPUT_SUB_TYPE
	int					nExtractFrameRate;				// Video frame rate pumping concentrated 1-32, the value, the higher the pumping rate of the frame, the client presents the faster playback
														// This field is concentrated only in the output data sub-type (emOutputSubType) is valid NET_EM_OUTPUT_SUB_TYPE_SYNOPSIS
}NET_VIDEOSYNOPSISRULE_INFO;

// add video synopsis input param
typedef struct tagNET_IN_ADD_VIDEOSYNOPSIS
{
	DWORD			    dwSize;			                // size
	NET_FILEPATH_INFO*	pFilePathInfo;	                // file path info
	DWORD				dwFileCount;	                // effective number of files
	NET_GLOBAL_INFO		stuGlobalInfo;	                // global info
	NET_MODULE_INFO		stuModuleInfo;	                // module info
	NET_VIDEOSYNOPSISRULE_INFO	stuRuleInfo;	        // rule info
	unsigned int		nWaitTime;		                // wait time(ms)
}NET_IN_ADD_VIDEOSYNOPSIS;

// add video synopsis output param
typedef struct tagNET_OUT_ADD_VIDEOSYNOPSIS
{
	DWORD	            dwSize;			                // size
	DWORD*	            pnTaskID;		                // TaskID array,users are assigned space.
	DWORD	            nTaskIDCount;	                // count of TaskID
}NET_OUT_ADD_VIDEOSYNOPSIS;

// pause video synopsis input param
typedef struct tagNET_IN_PAUSE_VIDEOSYNOPSIS
{
	DWORD	            dwSize;			                // size 
	BOOL                bPause;                         // TRUE-pause  FALSE-resume 
	DWORD*	            pnTaskID;		                // TaskID array,users are assigned space.
	DWORD	            nTaskIDCount;	                // count of TaskID
	DWORD	            nWaitTime;		                // wait time(ms)
}NET_IN_PAUSE_VIDEOSYNOPSIS;

//  remove video synopsis input param
typedef struct tagNET_IN_REMOVE_VIDEOSYNOPSIS
{
	DWORD	            dwSize;			                // size
	DWORD*	            pnTaskID;		                // TaskID array,users are assigned space.
	DWORD	            nTaskIDCount;	                // count of TaskID
	DWORD	            nWaitTime;		                // wait time(ms)
}NET_IN_REMOVE_VIDEOSYNOPSIS;

// return objece info of video synopsis
typedef struct tagNET_REAL_SYNOPSIS_OBJECT_INFO
{
	DWORD               dwSize;
	
	DWORD               dwTaskID;                       // task ID
	DH_MSG_OBJECT       stuObjectInfo;                  // object info
}NET_REAL_SYNOPSIS_OBJECT_INFO;

typedef struct tagNET_REAL_SYNOPSIS_STATE_INFO
{
	DWORD               dwSize;
	
	DWORD               dwTaskID;                       // task ID   
	int                 nObjectNum;                     // number of object
	int                 nTaolProgress;                  // progress of condensation,0~100
	char                szState[DH_MAX_SYNOPSIS_STATE_NAME]; // state of condensation,"Downloading"  "Synopsising"
                                                        // "DownloadFailed""DownloadSucceeded"
	                                                  	// "SynopsisFailed" ,"Succeeded" 
		                                                // "DownloadPause" ,"SynopsisPause" 
	int           nProgressOfCurrentState;              //  progress of current state,0~100
	char				szFailedCode[DH_MAX_STRING_LEN];// Failure code��szState="SynopsisFailed"Effective
														// "OutOfMemory" Out of memory;
														// "TooMany_TargetInVideo"  Too many goals the entire video;
														// "InvalidFilePointer" File pointer is invalid;
														// "InvalidFilePath" File path is invalid;
														// "CreateDirectoryFailed"  Create a folder path fails;
														// "WriteDataFailed" Intermediate file failed to write Tube
														// "DataSizeTooLarge" Tube file is too large
														// "Detect_Nothing" Scene without moving target
														// "OpenDataFailed" Failed to open the intermediate file
														// "InvalidSynopsisBackgroud" Invalid concentrated background
														// "ColorRetrieval" Wrong color retrieve configuration
														// "UnsupportRuleType" Unsupported rule type
														// "RuleNumberExceedLimit" Number of rules overrun
														// "NoFunctionCfgInfo" But did not specify the configuration information corresponding function
														// "FunctionNumberExceedLimit" The number of feature quantity of each rule overrun
														// "PointsExceedLimit" Point line or area overrun
														// "BadFunctionType" Error function type
														// "RulePointNumber" Points rule error
														// "MaskRegionNumberExceedLimit" Shielded area overrun quantity
														// "CameraSceneSwitch" Cameras scene change
														// "TooManyTargeInFrame" Excessive number of single-frame target
														// "InalidSynopsisDesity" Concentrated density invalid
														// "ExtrackColorFeatureFailed" Failed to extract color features
														// "JpegEncodeFrameFailed" Jpeg encoder failure
														// "JpegParamRestFailed" Jpeg encoding parameters fail reset
														// "JpegDecodeFrameFailed" Jpeg decoding failure
														// "RetrieveObjectIdInvalid" Retrieving Target ID is invalid
														// "RetrieveImageInvalid" Invalid image retrieval
														// "NetworkAnomaly" DISH Network anomaly map
														// "ObjectNumberLimit" Limit the number of goals over analysis
														// "CreateChannelFailed" Create a channel failure
														// "ReadUGFileFailed" Read UG file failed
														// "GetSynopsisInfoFailed" Failed to obtain concentrated Video
														// "ObjectNumberExceedLimit" The number of targets in the scene over the limit
														// "RebuildBackgroudFailed" Background reconstruction failure
														// "NotExistMiddleFile" Intermediate file does not exist
														// "NotExistSourceFile" Original file does not exist
														// "NotDog" No dongle
														// "NotEnoughFreeDisk" Disk space is not enough
														// "StartDecodeFail" Start decoding failure
														// "DecodeTimeOut" Decoding wait timeout
														// "EncodeTimeOut" Coding wait timeout
														// "ExactTimeOut" Extract snapshots wait timeout
														// "ReadMidlleFileFail" Failed to read the intermediate file
														// "ExactPictureFail" Failed to extract pictures
}NET_REAL_SYNOPSIS_STATE_INFO;

//callback function of synopsis video info
typedef int (CALLBACK *fVideoSynopsisObjData)(LLONG lRealLoadHandle, NET_REAL_SYNOPSIS_OBJECT_INFO* pSynopsisObjectInfo, void* pBuf, int nBufLen, LDWORD dwUser, void* pReserved);

//callback function of synopsis video state
typedef int (CALLBACK *fVideoSynopsisState)(LLONG lRealLoadHandle, NET_REAL_SYNOPSIS_STATE_INFO* pSynopsisStateInfos, int nInfoCount, LDWORD dwUser, void* pReserved);

// CLIENT_RealLoadObjectData's interface define
typedef struct tagNET_IN_REALLOAD_OBJECTDATA
{
	DWORD               dwSize;                         // size
	
	int                 nTaskIDNum;                     // number of taskID
	DWORD               *pTaskIDs;                      // material taskID
	BOOL                bNeedPicFile;                   // need download corresponding to the picture 
	int                 nWaitTime;                      // wait time(ms)
	fVideoSynopsisObjData cbVideoSynopsisObjData;       // callback function's pointer
	LDWORD         dwUser;
}NET_IN_REALLOAD_OBJECTDATA;

typedef struct tagNET_OUT_REALLOAD_OBJECTDATA
{
	DWORD               dwSize;                         // size

	LLONG               lRealLoadHandle;                // return subscriptal handle
}NET_OUT_REALLOAD_OBJECTDATA;

// CLIENT_StopLoadObjectData's interface define
typedef struct tagNET_IN_STOPLOAD_OBJECTDATA
{
	DWORD               dwSize;

	int                 nTackIDNum;                     // number of tackID
	DWORD               *pTaskIDs;                      // specific taskID  
}NET_IN_STOPLOAD_OBJECTDATA;

// CLIENT_RealLoadSynopsisState's interface define
typedef struct tagNET_IN_REALLAOD_SYNOPSISSTATE
{
	DWORD               dwSize;                         // size
	
	int                 nTaskIDNum;                     // number of taskID
	DWORD               *pTaskIDs;                      // specific taskID  
	int                 nWaitTime;                      // wait time(ms)
	fVideoSynopsisState cbVideoSynopsisState;           // callback function's pointer
	LDWORD         dwUser;
}NET_IN_REALLAOD_SYNOPSISSTATE;

typedef struct tagNET_OUT_REALLOAD_SYNOPSISSTATE
{
	DWORD               dwSize;                         // size
	
	LLONG               lRealLoadHandle;                // return subscriptal handle
}NET_OUT_REALLOAD_SYNOPSISSTATE;

// CLIENT_StopLoadSynopsisState's interface define
typedef struct tagNET_IN_STOPLOAD_SYNOPSISSTATE
{
	DWORD               dwSize;
	
	int                 nTackIDNum;                     // number of taskID
	DWORD               *pTaskIDs;                      // specific taskID    
}NET_IN_STOPLOAD_SYNOPSISSTATE;

// type of video synopsis's query
typedef enum tagEM_VIDEOSYNOPSIS_QUERY_TYPE
{
	EM_VIDEOSYNOPSIS_TASK,	                            // task info
	EM_VIDEOSYNOPSIS_OBJ,	                            // snapshot
}EM_VIDEOSYNOPSIS_QUERY_TYPE;

// query the task of video synopsis infomation
typedef struct tagNET_QUERY_VSTASK_INFO
{
	DWORD	            dwSize;			                // size
	DWORD	            dwTaskID;		                // taskID,when value = 0,query all task
}NET_QUERY_VSTASK_INFO;

// query conditions of video objects in the task infomation
typedef struct tagNET_QUERY_VSOBJECT_INFO
{
	DWORD			    dwSize;			                // size
	DWORD*			    pdwTaskID;                		// taskID,when value = -1,query all task
	DWORD			    dwTaskIDCount;                	// count of taskID
	DH_MSG_OBJECT	    stuObjInfo;	                	// object info,when objectID!=0,query all
}NET_QUERY_VSOBJECT_INFO;

//--CLIENT_QueryVideoSynopsisInfo's interface input define
typedef struct tagNET_IN_QUERY_VIDEOSYNOPSIS
{
	DWORD	            dwSize;							// size
	EM_VIDEOSYNOPSIS_QUERY_TYPE emQueryType;            // query type
	DWORD	            dwCount;						// count of query
	DWORD	            dwBeginNumber;					// begin number
	void*	            pQueryInfo;						// information query condition
											            // when emQueryType==EM_VIDEOSYNOPSIS_TASK, corresponding to NET_QUERY_VSTASK_INFO
											            // when emQueryType==EM_VIDEOSYNOPSIS_OBJ, corresponding to NET_QUERY_VSOBJECT_INFO

	DWORD	            dwWaitTime;						// wait time(ms)
}NET_IN_QUERY_VIDEOSYNOPSIS;

// query to the task of each synopsis video information
typedef struct tagNET_VSTASK_INFO
{
	DWORD		        dwSize;			                // size
	DWORD				dwTaskID;		                // task ID
	NET_MODULE_INFO		stuModuleInfo;                	// module info
	NET_VIDEOSYNOPSISRULE_INFO	stuRuleInfo;           	// info of task rule
	NET_GLOBAL_INFO		stuGlobalInfo;	                // global param
	NET_TIME			stuCreateTime;	                // create time
	char				szCurrState[DH_MAX_SYNOPSIS_STATE_NAME];  // current state,"Downloading" "Synopsising" 
																  // "DownloadFailed" "DownloadSucceeded" 
																  // "SynopsisFailed" "Succeeded" 
																  // "DownloadPause" "SynopsisPause" 
	char				szCreateUser[DH_MAX_NAME_LEN];	// the creator
	int					nProgressOfCurrentState;		// progress of current state [0~100]
	char				szLocalFilePath[MAX_PATH];		// local file path
	int					nObjectNum;						// number of object
	NET_FILEPATH_INFO	stuVideoSourceFilePath;			// path of video source file
	NET_FILEPATH_INFO	stuSynopsisVideoFilePath;		// path of synopsis video file
	char                szFailedCode[DH_MAX_STRING_LEN];// Concentrated video failed error code��szCurrState when "SynopsisFailed" Effective
                                                        // "OutOfMemory" Out of memory;
                                                        // "TooMany_TargetInVideo"  Too many goals the entire video;
                                                        // "InvalidFilePointer" File pointer is invalid;
                                                        // "InvalidFilePath" File path is invalid;
                                                        // "CreateDirectoryFailed"  Create a folder path fails;
                                                        // "WriteDataFailed" Intermediate file failed to write Tube
                                                        // "DataSizeTooLarge" Tube file is too large
                                                        // "Detect_Nothing" Scene without moving target
                                                        // "OpenDataFailed" Failed to open the intermediate file
                                                        // "InvalidSynopsisBackgroud" Invalid concentrated background
                                                        // "ColorRetrieval" Wrong color retrieve configuration
                                                        // "UnsupportRuleType" Unsupported rule type
                                                        // "RuleNumberExceedLimit" Number of rules overrun
                                                        // "NoFunctionCfgInfo" But did not specify the configuration information corresponding function
                                                        // "FunctionNumberExceedLimit" The number of feature quantity of each rule overrun
                                                        // "PointsExceedLimit" Point line or area overrun
                                                        // "BadFunctionType" Error function type
                                                        // "RulePointNumber" Points rule error
                                                        // "MaskRegionNumberExceedLimit" Shielded area overrun quantity
                                                        // "CameraSceneSwitch" Cameras scene change
                                                        // "TooManyTargeInFrame" Excessive number of single-frame target
                                                        // "InalidSynopsisDesity" Concentrated density invalid
                                                        // "ExtrackColorFeatureFailed" Failed to extract color features
                                                        // "JpegEncodeFrameFailed" Jpeg encoder failure
                                                        // "JpegParamRestFailed" Jpeg encoding parameters fail reset
                                                        // "JpegDecodeFrameFailed" Jpeg decoding failure
                                                        // "RetrieveObjectIdInvalid" Retrieving Target ID is invalid
                                                        // "RetrieveImageInvalid" RetrieveImageInvalid
                                                        // "NetworkAnomaly" NetworkAnomaly
                                                        // "ObjectNumberLimit" bjectNumberLimit
                                                        // "CreateChannelFailed" CreateChannelFailed
                                                        // "ReadUGFileFailed" ReadUGFileFailed
                                                        // "GetSynopsisInfoFailed" GetSynopsisInfoFailed
                                                        // "ObjectNumberExceedLimit" ObjectNumberExceedLimit
                                                        // "RebuildBackgroudFailed" RebuildBackgroudFailed
                                                        // "NotExistMiddleFile" NotExistMiddleFile
                                                        // "NotExistSourceFile" NotExistSourceFile
                                                        // "NotDog" NotDog
                                                        // "NotEnoughFreeDisk" NotEnoughFreeDisk
                                                        // "StartDecodeFail"StartDecodeFail
                                                        // "DecodeTimeOut" DecodeTimeOut
                                                        // "EncodeTimeOut" EncodeTimeOut
                                                        // "ExactTimeOut" ExactTimeOut
                                                        // "ReadMidlleFileFail" ReadMidlleFileFail
                                                        // "ExactPictureFail" ExactPictureFail
}NET_VSTASK_INFO;

// each object in the synopsis video task info
typedef struct tagNET_VSOBJECT_INFO
{
	DWORD				dwSize;			                // size 
	DWORD				dwTaskID;		                // task ID
	DH_MSG_OBJECT		stuObjInfo;		                // object info
	NET_FILEPATH_INFO	stuFilePathInfo;                // file info
	DWORD				dwFileLength;	                // file length
}NET_VSOBJECT_INFO;

//--CLIENT_QueryVideoSynopsisInfo's interface output define
typedef struct tagNET_OUT_QUERY_VIDEOSYNOPSIS
{
	DWORD	            dwSize;			                // size
	DWORD	            dwTotalCount;	                // total count
	DWORD            	dwFoundCount;	                // found count
	void*	            pResult;                		// return result info
					                		            // if emQueryType==EM_VIDEOSYNOPSIS_TASK,corresponding to NET_VSTASK_INFO
							                            // if emQueryType==EM_VIDEOSYNOPSIS_OBJ,corresponding to NET_VSOBJECT_INFO
	DWORD	            dwMaxCount;		                // hope tp get the number
}NET_OUT_QUERY_VIDEOSYNOPSIS;

// CLIENT_FindSynopsisFile's interface define 
// query type of file
enum NET_EM_QUERY_SYNOPSIS_FILE
{
	DH_FILE_QUERY_VIDEO,                                // normal record file info,corresponding to:NET_SYNOPSIS_QUERY_VIDEO_PARAM, return result:NET_SYNOPSISFILE_VIDEO_INFO
	DH_FILE_QUERY_SYNOPSISVIDEO,                        // synopsis video file,corresponding to:NET_QUERY_SYNOPSISVIDEO_PARAM,return result:NET_QUERY_SYNOPSISVIDEO_INFO
};

typedef struct __NET_SYNOPSIS_QUERY_VIDEO_PARAM
{
	DWORD               dwSize;
	DWORD				dwQueryMask;					// query type mask, according to from low to high,the first bit is period of time, the second is file path 
	NET_TIME			stuStartTime;					// start time
	NET_TIME			stuEndTime;						// end time
	char				szFilePath[DH_MAX_SYNOPSIS_QUERY_FILE_COUNT][MAX_PATH];	// file path
	DWORD				dwFileCount;					// file count
}NET_SYNOPSIS_QUERY_VIDEO_PARAM;

typedef struct  
{
	DWORD               dwSize;
	int                 nTaskId;                        // taskID
	DWORD               dwOutPutTypeMask;               // output type in current,the first bit is snapshot,the second bit is synopsis video 
	char                szCurrentState[DH_MAX_SYNOPSIS_STATE_NAME]; // current state
}NET_VIDEOSYNOPSIS_TASK_INFO;

// DH_FILE_QUERY_VIDEO return video file info
typedef struct
{
	DWORD               dwSize;
    unsigned int		nchannelId;						// channel ID
    char				szFilePath[MAX_PATH];		    // file path
    unsigned int		nFileLenth;					    // file lenth(byte)
    NET_TIME			stuStarttime;				    // start time 
    NET_TIME			stuEndtime;				        // end time
    unsigned int		nWorkDirSN;				        // work dir SN	
	unsigned int        nCluster;                       // cluster				
	BYTE                bHint;					        // hint
	BYTE                bDriveNo;                       // drive no.
	BYTE                bReserved[18];                  // reserved
	
	int                 nTaskInfoNum;                   // task info number
	NET_VIDEOSYNOPSIS_TASK_INFO stuTaskInfo[16];        // video synopsis task info
}NET_SYNOPSISFILE_VIDEO_INFO;

// DH_FILE_QUERY_SYNOPSISVIDEO synopsis viedo file query param
typedef struct __NET_QUERY_SYNOPSISVIDEO_PARAM
{
	DWORD               dwSize;
	int                 nTaskID;                        // taskID
}NET_QUERY_SYNOPSISVIDEO_PARAM;

// DH_FILE_QUERY_SYNOPSISVIDEO query result
typedef struct __NET_QUERY_SYNOPSISVIDEO_INFO
{
	DWORD               dwSize;
    char				szFilePath[MAX_PATH];        	// file path
    unsigned int		nFileLenth;					    // file lenth
	int                 nDurationTime;                  // duration time(s)
}NET_QUERY_SYNOPSISVIDEO_INFO;

// CLIENT_FindSynopsisFile's interface param
typedef struct tagNET_IN_FIND_SYNOPSISFILE
{
	DWORD               dwSize;                          
	NET_EM_QUERY_SYNOPSIS_FILE emQueryType;             // query's type
	void*               pQueryCondition;                // query's type
	int                 nWaitTime;                      // wait time(ms)
}NET_IN_FIND_SYNOPSISFILE;

typedef struct tagNET_OUT_FIND_SYNOPSISFILE
{
	DWORD               dwSize;
	LLONG               lFindHandle;                    // find handle         
}NET_OUT_FIND_SYNOPSISFILE;

// CLIENT_FindNextSynopsisFile's interface param
typedef struct tagNET_IN_FINDNEXT_SYNOPSISFILE
{
	DWORD               dwSize;
    int                 nFileCount;                     // count of file
	NET_EM_QUERY_SYNOPSIS_FILE emQueryType;             // query's type
	void*               pSynopsisFileInfo;              // info of file buf
	int                 nMaxlen;                        // size of buf
	int                 nWaitTime;                      // wait time(ms)
}NET_IN_FINDNEXT_SYNOPSISFILE;

typedef struct tagNET_OUT_FINDNEXT_SYNOPSISFILE
{
	DWORD               dwSize;
	int                 nRetFileCount;                  // the actual return information artucle number,return<nFileCount(input param) corresponding period of the finished file query
}NET_OUT_FINDNEXT_SYNOPSISFILE;

// download progress's callback,nError means downloading appear error,1 - cashe is insufficient,2-data validation error,3.download failed,4,create file failed
typedef void (CALLBACK *fSynopsisFileDownLoadPosCB)(LLONG lDownLoadHandle, DWORD dwFileID, DWORD dwFileTotalSize, DWORD dwDownLoadSize, int nError, LDWORD dwUser, void* pReserved);

// CLIENT_DownLoadSynosisFile's interface param
#define NET_SYNOPSIS_DOWNLOADFILE_INFO NET_DOWNLOADFILE_INFO

typedef struct tagNET_IN_DOWNLOAD_SYNOPSISFILE
{
	DWORD               dwSize;
    int                 nFileCount;                     // count of file
	NET_SYNOPSIS_DOWNLOADFILE_INFO* pFileInfos;         // file info
	fSynopsisFileDownLoadPosCB cbPosCallBack;           // post of callback function
	LDWORD              dwUserData;                     // user's data
	int                 nWaitTime;                      // wait time(ms)
}NET_IN_DOWNLOAD_SYNOPSISFILE;

typedef struct tagNET_OUT_DOWNLOAD_SYNOPSISFILE
{
	DWORD               dwSize;
	LLONG               lDownLoadHandle;                // handle of download
}NET_OUT_DOWNLOAD_SYNOPSISFILE;

// path of file's info 
typedef struct	tagNET_SET_FILEPATH_INFO
{
	DWORD		        dwSize;
	char		        szFilePath[MAX_PATH];	        // can be a folder ,can be the file,current server only supports the dav format file
}NET_SET_FILEPATH_INFO;

// CLIENT_SetFilePathInfo()interface input param
typedef struct tagNET_IN_SET_FILEPATHINFO
{
	DWORD		        dwSize;
	DWORD		        dwCount;		                // count of added file
	void*	        	pFilePathInfo;                	// look for NET_SET_FILEPATH_INFO
	DWORD		        dwWaitTime;		                // wait time(ms)
}NET_IN_SET_FILEPATHINFO;


// fAddFileStateCB param
typedef struct tagNET_CB_ADDFILESTATE
{
	DWORD		        dwSize;
	const char*         szFileName;                     // file name
	const char*         szState;                        // analyse file's state, "Successed" ; "Failed" ;
}NET_CB_ADDFILESTATE;

// burning device callback function, pBuf->dwSize == nBufLen
typedef void (CALLBACK *fAddFileStateCB) (LLONG lLoginID, LLONG lAttachHandle, NET_CB_ADDFILESTATE* pBuf,  int nBufLen, LDWORD dwUser);

// CLIENT_AttacAddFileState()interface input param
typedef struct tagNET_IN_ADDFILE_STATE
{
	DWORD		        dwSize;
	fAddFileStateCB     cbAttachState;                 // listenint to increase callback file state
	LDWORD              dwUser;                        // user's data
}NET_IN_ADDFILE_STATE;
typedef struct tagNET_OUT_ADDFILE_STATE
{
	DWORD		        dwSize;
}NET_OUT_ADDFILE_STATE;

///////////////////////////////////Face recognition module related structures///////////////////////////////////////

// CLIENT_OperateFaceRecognitionDBInterface input parameters
typedef struct __NET_IN_OPERATE_FACERECONGNITIONDB
{
	DWORD             dwSize;
	EM_OPERATE_FACERECONGNITIONDB_TYPE emOperateType;  // Type of operation
	FACERECOGNITION_PERSON_INFO        stPersonInfo;   // Personnel information

	// Picture binary data
	char                *pBuffer;                      // Buffer address
	int                 nBufferLen;                    // Buffer data length
}NET_IN_OPERATE_FACERECONGNITIONDB;

// CLIENT_OperateFaceRecognitionDB�ӿ��������
typedef struct __NET_OUT_OPERATE_FACERECONGNITIONDB
{
    DWORD               dwSize;
}NET_OUT_OPERATE_FACERECONGNITIONDB;

typedef struct __NET_FACE_MATCH_OPTIONS
{
	DWORD               dwSize;
	unsigned int        nMatchImportant;               // Important level 1 to 10 staff, the higher the number the more important (check important level greater than or equal to this level of staff)
	EM_FACE_COMPARE_MODE emMode;                       // Face comparison mode, see EM_FACE_COMPARE_MODE
	int                 nAreaNum;                      // Face the number of regional
	EM_FACE_AREA_TYPE   szAreas[MAX_FACE_AREA_NUM];    // Regional groupings of people face is NET_FACE_COMPARE_MODE_AREA effective when emMode
	int                 nAccuracy;                     // Recognition accuracy (ranging from 1 to 10, with the value increases, the detection accuracy is improved, the detection rate of decline. Minimum value of 1 indicates the detection speed priority, the maximum is 10, said detection accuracy preferred. Temporarily valid only for face detection)
	int                 nSimilarity;                   // Similarity (must be greater than the degree of acquaintance before the report; expressed as a percentage, from 1 to 100)
	int                 nMaxCandidate;                 // Reported the largest number of candidate (based on similarity to sort candidates to take the maximum number of similarity report)
	
}NET_FACE_MATCH_OPTIONS;

typedef struct __NET_FACE_FILTER_CONDTION
{
	DWORD               dwSize;
	NET_TIME			stStartTime;			       // Start time
    NET_TIME			stEndTime;				       // End Time
	char                szMachineAddress[MAX_PATH];    // Place to support fuzzy matching
	int                 nRangeNum;                     // The actual number of database
	BYTE                szRange[MAX_FACE_DB_NUM];      // To query the database type, see EM_FACE_DB_TYPE
	EM_FACERECOGNITION_FACE_TYPE emFaceType;           // Face to query types, see EM_FACERECOGNITION
}NET_FACE_FILTER_CONDTION;
// CLIENT_StartFindFaceRecognitionInterface input parameters
typedef struct __NET_IN_STARTFIND_FACERECONGNITION
{
	DWORD               dwSize;
	BOOL                bPersonEnable;                 // Personnel information query is valid
	FACERECOGNITION_PERSON_INFO stPerson;              // Personnel information query
	NET_FACE_MATCH_OPTIONS stMatchOptions;             // Face Matching Options
	NET_FACE_FILTER_CONDTION stFilterInfo;             // Query filters
  
	// Picture binary data
	char                *pBuffer;                      // Buffer address
	int                 nBufferLen;                    //Buffer data length
}NET_IN_STARTFIND_FACERECONGNITION;

// CLIENT_StartFindFaceRecognitionInterface output parameters
typedef struct __NET_OUT_STARTFIND_FACERECONGNITION
{
	DWORD               dwSize;
	int                 nTotalCount;                   // Record number of returns that match the query criteria
	LLONG               lFindHandle;                   // Query handle
}NET_OUT_STARTFIND_FACERECONGNITION;

#define MAX_FIND_COUNT  20

// CLIENT_DoFindFaceRecognition Interface input parameters
typedef struct __NET_IN_DOFIND_FACERECONGNITION
{
	DWORD               dwSize;
	LLONG               lFindHandle;                   // Query handle
	int                 nBeginNum;                     // Queries starting serial number
	int                 nCount;                        // The current number of records you want to search for
}NET_IN_DOFIND_FACERECONGNITION;

// CLIENT_DoFindFaceRecognitionInterface output parameters
typedef struct __NET_OUT_FINDNEXT_FACERECONGNITION
{
	DWORD               dwSize;
	int                 nCadidateNum;                  // The actual number of candidate information structure returned
	CANDIDATE_INFO      stCadidateInfo[MAX_FIND_COUNT];// An array of candidate information
	
	// Picture binary data
	char                *pBuffer;                      // Buffer address
	int                 nBufferLen;                    // Buffer data length
}NET_OUT_DOFIND_FACERECONGNITION;

// CLIENT_DetectFaceInterface input parameters
typedef struct __NET_IN_DETECT_FACE
{
	DWORD               dwSize; 
	DH_PIC_INFO         stPicInfo;                     // Big picture information

	// Picture binary data
	char                *pBuffer;                      // Buffer address
	int                 nBufferLen;                    // Buffer data length
}NET_IN_DETECT_FACE;

// CLIENT_DetectFaceInterface output parameters
typedef struct __NET_OUT_DETECT_FACE
{
	DWORD               dwSize; 
	DH_PIC_INFO         *pPicInfo;                     // The detected face image information from the user space applications
	int                 nMaxPicNum;                    // The maximum number of face image information
	int                 nRetPicNum;                    // The actual number of returning faces pictures
	
	// Picture binary data
	char                *pBuffer;                      // Buffer address, the user application space to store the detected face image data
	int                 nBufferLen;                    // Buffer data length
}NET_OUT_DETECT_FACE;

//////////////////////////////// burn the upload////////////////////////////////

// fBurnCheckCallBack Parameter
typedef struct tagNET_CB_BURN_CHECK_STATE
{
	DWORD		        dwSize;
	const char*         szType;							// Message Type
														// "Checking": Check in
														// "CheckResult": CheckResult
	BOOL				bCheckResult;					// Calibration results for "CheckResult", TRUE-success, FALSE-failure
	BYTE				byProgress;						// Calibration schedule for "Checking", the percentage of 0 to 100
	BYTE				reserved[3];
} NET_CB_BURN_CHECK_STATE;

// Burn verification callback function prototype, lAttachHandle return value is CLIENT_AttachBurnCheckState
typedef void (CALLBACK *fBurnCheckCallBack)(LLONG lLoginID, LLONG lAttachHandle, NET_CB_BURN_CHECK_STATE* pstState, void* reserved, LDWORD dwUser);

// CLIENT_AttachBurnCheckState Input parameter interface (monitor burn parity status)
typedef struct tagNET_IN_ATTACH_BURN_CHECK 
{
	DWORD				dwSize;
	fBurnCheckCallBack  cbBurnCheck;					// Burn verification callback
	LDWORD              dwUser;							// User data
} NET_IN_ATTACH_BURN_CHECK;

// CLIENT_AttachBurnCheckState Output parameters of the interface (listening burning parity status)
typedef struct tagNET_OUT_ATTACH_BURN_CHECK
{
    DWORD            dwSize;
} NET_OUT_ATTACH_BURN_CHECK;

///////////////////////////////// logical device /////////////////////////////////

typedef struct tagNET_CB_CAMERASTATE
{
	DWORD		        dwSize;
	int                 nChannel;             // channel
	CONNECT_STATE       emConnectState;       // state of connect
}NET_CB_CAMERASTATE;

// CLIENT_AttachCameraState()callback function, pBuf->dwSize == nBufLen
typedef void (CALLBACK *fCameraStateCallBack) (LLONG lLoginID, LLONG lAttachHandle, const NET_CB_CAMERASTATE *pBuf, int nBufLen, LDWORD dwUser);

// CLIENT_AttachCameraState()input param
typedef struct tagNET_IN_CAMERASTATE
{
	DWORD		        dwSize;
	int *               pChannels;             // observation of the channel, if the value = -1,is boservate all channel
	int                 nChannels;             // length of pChannels pointer
    fCameraStateCallBack cbCamera;             // state of callback function
	LDWORD				dwUser;                // user's data
}NET_IN_CAMERASTATE;
typedef struct tagNET_OUT_CAMERASTATE
{
	DWORD		        dwSize;
}NET_OUT_CAMERASTATE;

////////////////////////////////Special version/////////////////////////////////

// Activate device to snapshot overlay card number information 
typedef struct __NET_SNAP_COMMANDINFO 
{
	char				szCardInfo[16];			// Card information
	char				reserved[64];			// Reserved 
} NET_SNAP_COMMANDINFO, LPNET_SNAP_COMMANDINFO;

typedef struct
{
	int					nChannelNum;			// Channel number 
	char				szUseType[32];			// Channel type 
	DWORD				dwStreamSize;			// Stream size(Unit:kb/s)
	char				reserved[32];			// Reserved 
} DHDEV_USE_CHANNEL_STATE;

typedef struct 
{
	char				szUserName[32];			// User name 
	char				szUserGroup[32];		// User group 
	NET_TIME			time;					// Log in time
	int					nOpenedChannelNum;		// Enabled channel amount 
	DHDEV_USE_CHANNEL_STATE	channelInfo[DH_MAX_CHANNUM];
	char                szIpAddress[DH_MAX_IPADDR_LEN_EX];  // ip
    char                reserved[24];
} DHDEV_USER_NET_INFO;

// Network running status information 
typedef	struct 
{
	int					nUserCount;				// User amount
	DHDEV_USER_NET_INFO	stuUserInfo[32];
	char				reserved[256];
}DHDEV_TOTAL_NET_STATE;

typedef struct
{
    char                szIpAddress[DH_MAX_IPADDR_LEN];  // ip
	char                szUserGroup[32];                 // User Group
	char                szUserName[32];                  // User Name
	char                reserved[64];
}DHDEV_USER_REJECT_INFO;

// Reject User
typedef struct
{ 
	int                       nUserCount;				// User Account
	DHDEV_USER_REJECT_INFO    stuUserInfo[10];     
	char				      reserved[256];
}DHDEV_REJECT_USER;

typedef struct
{
	char                szIpAddress[DH_MAX_IPADDR_LEN];  // ip
	char                szUserGroup[32];                 // User Group
	char                szUserName[32];                  // User Name
	int                 nForbiddenTime;                  // Forbidden Time
	char                reserved[64];
}DHDEV_USER_SHIELD_INFO;

// Shield User
typedef struct
{ 
	int                       nUserCount;          // User Account
	DHDEV_USER_SHIELD_INFO    stuUserInfo[10];     
	char				      reserved[256];
}DHDEV_SHIELD_USER;

// longitude latitude
typedef struct
{//  longitude first,then latitude
	char                chPreLogi;        // preflag of longitude:N(north),S(south),W(west),E(east)
	char                chPreLati;        // preflag of latitude:N(north),S(south),W(west),E(east)
	BYTE                reserved[6];      // 
	double              dbLongitude;      // longitude
	double              dbLatitude;       // latitude
}DHDEV_LONGI_LATI;

// NAVIGATION OR SMS
typedef struct
{
	char                szPhoneNum[DH_MAX_PHONE_NO_LEN];    // phone no
	NET_TIME            stMsgTime;                          // time of sending message
	char                szMsgType[DH_MAX_MSGTYPE_LEN];      // type:Navigation message-Navigation,short message--SMS
	char                szSmsContext[DH_MAX_MSG_LEN];       // message to send

	DHDEV_LONGI_LATI    stLogiLati;                         // longitude
	unsigned int        uFlag;                              // 01:marking true longitude
	char                szNavigationType[16];               // TNC,TXZ
	char                szAddress[32];                      // address
	char                szNavigationMode[32];               // mode
	DHDEV_LONGI_LATI    stPassLogiLati[5];                  // passing
	DHDEV_LONGI_LATI    stNoPassLogiLati[5];                // no passing
	BYTE				reserved[256];
}DHDEV_NAVIGATION_SMSS;

// Image watermark setup 
typedef struct __DHDEV_WATERMAKE_CFG 
{
	DWORD				dwSize;
	int					nEnable;				// Enable 
	int					nStream;				// Bit stream(1~n)0- All bit streams
	int					nKey;					// Data type(1- text,2- image )
	char				szLetterData[DH_MAX_WATERMAKE_LETTER];	// text
	char				szData[DH_MAX_WATERMAKE_DATA]; // Image data
	BYTE				bReserved[512];			// Reserved
} DHDEV_WATERMAKE_CFG;

// Storage position set structure.  Each channel set separately.Each channel can set several storage types including local,portableand remote storage.
typedef struct 
{
	DWORD				dwSize;
	DWORD				dwLocalMask;			// Local storage mask. Use bit to represent.
												// The first bit:system pre-record. The second bit:scheduled record. The third bit:motion detection record.
												// The fourth bit:alarm record. The fifth bit:card record. The sixth bit:manual record.
	DWORD				dwMobileMask;			// Moving storage mask. Storage mask such as local storage mask.
	int					RemoteType;				// Remote storage type 0: Ftp  1: Smb 
	DWORD				dwRemoteMask;			// Remote storage mask.  Storage mask such as local storage mask.
	DWORD				dwRemoteSecondSelLocal;	// Local storage mask when remote is abnormal
	DWORD				dwRemoteSecondSelMobile;// Moving storage mask when remote is abnormal.
	char				SubRemotePath[MAX_PATH_STOR]; // Remote path. Its length is 240
	DWORD				dwFunctionMask;			// Function shield bit. Use bit to represent bit0 = 1:Shield sanpshot event activate storage position function.
	DWORD				dwAutoSyncMask;			// If synchronous mask; after remote storage network recovery, the local storage data will automatically synchronized to the remote storage.
	BYTE				bAutoSyncRange;			// the time for synchronous from the network synchronous time. In hour. 0:all data  1:data in one hour n:data in n hours
	char				reserved[119];
} DH_STORAGE_STATION_CFG;

#define MAX_ALARM_DECODER_NUM 16
typedef struct  
{
	DWORD				dwAlarmDecoder;			// Now it supports max 8 alarm input ports. Reserved 8 bits for future development.
	BYTE				bDecoderIndex;			// It means it is from which alarm decoder.
	BYTE				bReserved[3];
} ALARM_DECODER;

// Alarm decoder alarm 
typedef struct
{
	int					nAlarmDecoderNum;
	ALARM_DECODER		stuAlarmDecoder[MAX_ALARM_DECODER_NUM];
	BYTE				bReserved[32];
} ALARM_DECODER_ALARM;

//DSP alarm
typedef struct  
{
	BOOL				bError;			//0,DSP normal 1,DSP abnormal
	DWORD				dwErrorMask;	//Bit shows,Nor 0 shows havening this error,0 shows no.(Now alarm has only 1 bit valid)
										//bit		DSP alarm
										//1			DSP downloading failure 
										//2			DSP error
										//3			Format not correct 
										//4			Resolution not support
										//5			Data format not support
										//6			Can't find I frame
	DWORD               dwDecChnnelMask;//Bit shows,alarm decoding channel number,dwErrorMask is DSP fault,incorrect format,incorrect resolution,data format not support
	DWORD               dwDecChnnelMask1;//Bit shows,33-64 is alarm of decoding channel, dwErrorMask is DSP mistake,format is wrong, effective when not support resolution or data format
	BYTE				bReserved[24];
} DSP_ALARM;

// Fiber-optic alarm
typedef struct  
{
	int		nFDDINum;
	BYTE	bAlarm[256];
} ALARM_FDDI_ALARM;

#define ALARM_PTZ_LOCATION_INFO DH_PTZ_LOCATION_INFO

// New audio detection alarm setup 
typedef struct
{
	BOOL				bEnable;				// Enable alarm input 
	int					Volume_min;				// Min volume
	int					Volume_max;				// Max volume
	char				reserved[128];	
	DH_TSECT			stSect[DH_N_WEEKS][DH_N_REC_TSECT];	
	DH_MSG_HANDLE		struHandle;				// Process way 
} DH_AUDIO_DETECT_INFO;

typedef struct  
{
	DWORD				dwSize;
	int					AlarmNum;
	DH_AUDIO_DETECT_INFO AudioDetectAlarm[DH_MAX_AUDIO_IN_NUM];
	char				reserved[256];
} DH_AUDIO_DETECT_CFG;

typedef struct
{
	BOOL				bTourEnable;			// Enable tour
	int					nTourPeriod;			// Tour interval. Unit is second. The value ranges from 5 to 300 
	DWORD				dwChannelMask;			// Tour channel. Use mas bit to represent.
	char				reserved[64];
}DH_VIDEOGROUP_CFG;

// Local matrix control strategy setup 
typedef struct
{
	DWORD				dwSize;
	int					nMatrixNum;				// Matrix amount(Read only )
	DH_VIDEOGROUP_CFG	struVideoGroup[DH_MATRIX_MAXOUT];
	char				reserved[32];
} DHDEV_VIDEO_MATRIX_CFG;   

// WEB path setup 
typedef struct 
{
	DWORD				dwSize;
	BOOL				bSnapEnable;					// Snapshot or not
	int					iSnapInterval;					// Snapshot interval
	char				szHostIp[DH_MAX_IPADDR_LEN];	// HTTP host IP
	WORD				wHostPort;
	int					iMsgInterval;					// Message sending out interval
	char				szUrlState[DH_MAX_URL_LEN];		// Status message upload URL
	char				szUrlImage[DH_MAX_URL_LEN];		// Image upload Url
	char				szDevId[DH_MAX_DEV_ID_LEN];		// Device web serial number
	BYTE				byReserved[2];
} DHDEV_URL_CFG;

// OEM search 
typedef struct  
{
	char				szVendor[DH_MAX_STRING_LEN];
	char				szType[DH_MAX_STRING_LEN];
	char				reserved[128];
} DHDEV_OEM_INFO;


//Video OSD
typedef struct 
{ 
	DWORD	rgbaFrontground;		// Object front view. Use bit to represent:red,green, blue and transparency.
	DWORD	rgbaBackground;			// Object background. Use bit to represent:red,green, blue and transparency
	RECT	rcRelativePos;			// Position. The proportion of border distance and whole length *8191
	BOOL	bPreviewBlend;			// Enable preview overlay	
	BOOL	bEncodeBlend;			// Enable encode overlay
	BYTE    bReserved[4];           // Reserved 
} DH_DVR_VIDEO_WIDGET;

typedef struct 
{
	DH_DVR_VIDEO_WIDGET	StOSD_POS; 								// OSD position and background color.
	char 				SzOSD_Name[DH_VIDEO_OSD_NAME_NUM]; 		// OSD name 
}DH_DVR_VIDEOITEM;
 
// OSD information in each channel
typedef struct 
{
	DWORD 				dwSize;
	DH_DVR_VIDEOITEM 	StOSDTitleOpt [DH_VIDEO_CUSTOM_OSD_NUM];// Each channel OSD information
	BYTE    			bReserved[16];							// Reserved
} DH_DVR_VIDEOOSD_CFG;

// CDMA/GPRS Configuration
// Time structure															    
typedef struct 
{
	BYTE				bEnable;					// Time period enable,1 shows this time period valid,0 shows invalid.
	BYTE				bBeginHour;
	BYTE				bBeginMin;
	BYTE				bBeginSec;
	BYTE				bEndHour;
	BYTE				bEndMin;
	BYTE				bEndSec;
	BYTE    			bReserved;					//Reserved
} DH_3G_TIMESECT, *LPDH_3G_TIMESECT;

typedef struct 
{
	DWORD 				dwSize;
	BOOL				bEnable;					// Wireless module enable
	DWORD               dwTypeMask;                 // Network type mask;Use bit to represent,The first bit:Automatic selection
													// The second bit:TD-SCDMA network;The third bit:WCDMA network;
													// The fourth bit:CDMA 1.x network;The fifth bit:GPRS network;The sixth bit:EVDO network
													// The seventh bit:EVDO network;The eighth bit:WIFI network;
	DWORD               dwNetType;                  // Types of wireless networks,EM_GPRSCDMA_NETWORK_TYPE
	char				szAPN[128];					// Access Point Name
	char				szDialNum[128];				// Dial-up number
	char				szUserName[128];			// Dial-up user name
	char				szPWD[128];					// Dial-up Password
	
	/* Read-only part */
	BOOL				iAccessStat;				// Wireless network registration status
    char				szDevIP[16];				// dial-up IP of Front-end equipment  
	char				szSubNetMask [16];			// Dial-up mask of Front-end equipment
	char				szGateWay[16];				// Dial-up Gateway of Front-end equipment

	int                 iKeepLive;					// Keep-alive time
	DH_3G_TIMESECT		stSect[DH_N_WEEKS][DH_N_TSECT];// 3G dial period,valid time period,start dialing;beyond this period,shut the dial.
	BYTE                byActivate;                  //Whether need to be active by voice or message
	
	BYTE                bySimStat;                    //SIM(UIM) State: 1, 0 (read-only data) be occupied the 1 byte
	char                szIdentify[128];               // identify mode
	bool                bPPPEnable;                    // PPP Dial-up,0- cut down ppp connect,1-ppp dial-up
	BYTE                bPPPState;                     // PPP Dial-up statr(real only),see EM_MOBILE_PPP_STATE
	BYTE                bNetCardState;                 // net card state(read only),see EM_3GMOBILE_STATE
	char				szPhyCardNo[32];			  // physics card no.
	char				Reserved[4];				   // reserved
	BYTE				byPinShow;					// PIN show or not(read only), 0-don't support 1-show, 2-don't show
	BYTE				byPinEnable;				// PIN setting enable(read only), 0-disable, 1-enable
	BYTE				byPinCount;					// the rest of pin setting number(read only), 0~3
	//-------------------------------the following fields in setting up effective after dwSize------------------------------------
	char				szPinNo[16];				// value of PIN, a combination of letters or numbers
} DHDEV_CDMAGPRS_CFG;

// The length of the video package configuration
typedef struct  
{
	DWORD 				dwSize;
	int					nType;						// 0:by time,1:by size
	int					nValue;						// nType = 0:(minutes),nType = 1:(KB)
	char				Reserved[128];				// Reserved
} DHDEV_RECORD_PACKET_CFG;

// (Directional)register the server information
typedef struct __DEV_AUTOREGISTER_INFO 
{
	LONG			lConnectionID;					// Connection ID
	char			szServerIp[DH_MAX_IPADDR_LEN];	// Server IP
	int				nPort;							// Port:0- 65535
	int             nState;                         // Server status:0-Registration failure;1-Registration success; 2-Connection failed
	char			reserved[16];
} DEV_AUTOREGISTER_INFO;

typedef struct __DEV_SERVER_AUTOREGISTER 
{
	DWORD					dwSize;
	int						nRegisterSeverCount;									// Number of registered servers
	DEV_AUTOREGISTER_INFO	stuDevRegisterSeverInfo[DH_CONTROL_AUTO_REGISTER_NUM];	// Server information
	char					reserved[256];
} DEV_SERVER_AUTOREGISTER, *LPDEV_SERVER_AUTOREGISTER;

// Upload burning annex
typedef struct
{
	DWORD				dwSize;		
	char				szSourFile[MAX_PATH_STOR];	// Source file path
	int					nFileSize;					// Source file size,If less than or equal to 0, sdk to calculate file size.
	char				szBurnFile[MAX_PATH_STOR];	// After burning the file name
	BYTE				bReserved[64];
} DHDEV_BURNFILE_TRANS_CFG;

// Update file upload
typedef struct
{
	char                 szFile[MAX_PATH_STOR];      // Update file path
	int                  nFileSize;                  // Update file size  
	BYTE                 byChannelId;                // Channel No.
	BYTE                 byManufactryType;           // Manufacturer type    Please refer to EM_IPC_TYPE
	
	BYTE                 byReserved[126];            // Reserved      
}DHDEV_UPGRADE_FILE_INFO;

// upload black-white list
typedef struct  
{
	char                 szFile[MAX_PATH_STOR];     // path of file
	int                  nFileSize;                 // size of upgrade file
	BYTE                 byFileType;                // type of file.0-black list,1-white list 
	BYTE                 byAction;                  // action,0-overload,1-additional

	BYTE                 byReserved[126];           // reserved
}DHDEV_BLACKWHITE_LIST_INFO;

// download black-white list
typedef struct
{
	char                 szFile[MAX_PATH_STOR];     // path of file
	BYTE                 byFileType;                // type of file,0-black list,1-white of list
	BYTE                 byReserved[127];           // reserved
}DHDEV_LOAD_BLACKWHITE_LIST_INFO;
// Zhengzhou EC_U video data overlay enabling configuration
typedef struct 
{
	BOOL				bEnable;					// Enable
	int					nPort;						// Port
	BYTE				bReserved[64];
}DHDEV_OSD_ENABLE_CFG;

// about vehicle configuration
typedef struct	
{
	BOOL				bAutoBootEnable;				//auto boot enable
	DWORD				dwAutoBootTime;					//auto boot time, Unit is second. The value ranges from 0 to 82800.
	BOOL				bAutoShutDownEnable;			//auto shut down enable
	DWORD				dwAutoShutDownTime;				//auto shut down time ,  Unit is second. The value ranges from 0 to 82800.
	DWORD				dwShutDownDelayTime;			//shut down delay time, Unit is second.
	BYTE				bEventNoDisk;					//1,(Ignore); 2,(Reboot)
	BYTE				bWifiEnable;					//If support car wifi module.
	BYTE				bUpperSpeedEnable;				//if use over speed verdict
	BYTE				bLowerSpeedEnable;				//if use low speed verdict
	DWORD				dwUpperSpeedValue;				//Over speed area(1,1000).km/hour.
	DWORD               dwLowerSpeedValue;              //Low speed area(1,1000).km/hour.
    DWORD               dwUpperSpeedDelayTime;          //Upper Speed Delay Time
	DWORD               dwLowerSpeedDelayTime;          //Lower Speed Delay Time
	DWORD               dwAlarmMaskEnable;              //Search/set OSD alarm information enable, every bit present one external alarm channel, 0:close;1:open
	BOOL                bSpeedOverAlarmRecordEnable;    // Over Speed Alarm Record Enable
	BOOL                bSpeedLowAlarmRecordEnable;     // Low Speed Alarm Record Enable
	BYTE				bReserved[480];					//reserved byte.
} DHDEV_ABOUT_VEHICLE_CFG;

// atm: query overlay ability information
typedef struct  
{
	DWORD				dwDataSource;					//1:Network, 2:Comm, 3:Network&Comm
	char				szProtocol[32][33];				//protocol name
	BYTE				bReserved[256];
} DHDEV_ATM_OVERLAY_GENERAL;
 
// atm: overlay configuration
typedef struct 
{
	DWORD				dwDataSource;					// 1:Network, 2:Comm
	char				szProtocol[33];					// protocol name, from DHDEV_ATM_OVERLAY_GENERAL
	BYTE				bReserved_1[3];
	DWORD				dwChannelMask;					// channel mask
	BYTE				bMode;							// 1:Preview, 2:Encode, 3:Preview&Encode
	BYTE				bLocation;						// 1:LeftTop, 2:LeftBottom, 3:RightTop, 4:RightBottom
	BYTE                bReserved_3[2];
	int                 nLatchTime;                     // display latch time on overlay, instruct the video's close latch time, 0~65535
	BYTE                bReserved_4[3];
	BYTE                bRecordSrcKeyNum;               // key number
	int                 nRecordSrcKey[32];              // key of channels 
	BYTE				bReserved_2[120];
} DHDEV_ATM_OVERLAY_CONFIG;

// atm:overlay set configuration
typedef struct 
{
	DWORD               dwSize;                         // size
	DWORD				dwDataSource;					// 1:Net, 2:Com, 3:Com422
	char				szProtocol[33];					// protocol name
	BYTE				bReserved_1[3];
	DWORD				dwChannelMask[8];				// overlay channel,the mask means: dwChannelMask[0] is channel 1,so on
	BYTE				bMode;							// 1:Preview(preview), 2:Encode(encode), 3:Preview&Encode(preview and encode)
	BYTE				bLocation;						// 1:LeftTop, 2:LeftBottom , 3:RightTop , 4:RightBottom 
	BYTE                bReserved_3[2];           
	int                 nLatchTime;                     // latch time,0~65535s
	BYTE                bReserved_4[3];
	BYTE                bRecordSrcKeyNum;               // number of key word
	int                 nRecordSrcKey[256];             // key word
} DHDEV_ATM_OVERLAY_CONFIG_EX;
#define DH_MAX_BACKUPDEV_NUM			16
#define DH_MAX_BACKUP_NAME				32

// backup device list
typedef struct 
{
	int					nBackupDevNum;												// number of backup devices
	char				szBackupDevNames[DH_MAX_BACKUPDEV_NUM][DH_MAX_BACKUP_NAME]; // backup device name
} DHDEV_BACKUP_LIST, *LPDHDEV_BACKUP_LIST;

// backup device type
typedef enum __BACKUP_TYPE
{
	BT_DHFS = 0,							// dvr file system
	BT_DISK,								// floating disk
	BT_CDRW									// CD
} DHDEV_BACKUP_TYPE;	

// backup device interface
typedef enum __BACKUP_BUS
{
	BB_USB = 0,								// usb
	BB_1394,								// 1394
	BB_IDE,									// ide
	BB_ESATA,								// esata
} DHDEV_BACKUP_BUS;	

typedef struct 
{
	char				szName[32];						// name
	BYTE				byType;							// see BACKUP_TYPE
	BYTE				byBus;							// see BACKUP_BUS
	UINT				nCapability;					// total capability(kBytes)
	UINT				nRemain;						// remain capability(kBytes)
	char				szDirectory[128];				// Remote Directory
} DHDEV_BACKUP_INFO, *LPDHDEV_BACKUP_INFO;

typedef struct 
{
	char				szName[32];						// name 
	UINT				nCapability;					// total capability(kBytes)
	UINT				nRemain;						// remain capability(kBytes)
} DHDEV_BACKUP_FEEDBACK, *LPDHDEV_BACKUP_FEEDBACK;

#define  DH_MAX_BACKUPRECORD_NUM 1024

typedef struct  
{
	char				szDeviceName[DH_MAX_BACKUP_NAME];			//name
	int					nRecordNum;									//number of records
	NET_RECORDFILE_INFO	stuRecordInfo[DH_MAX_BACKUPRECORD_NUM];		//record information
} BACKUP_RECORD, *LPBACKUP_RECORD;

// multiplay input param
typedef struct
{
	int                 iChannelID;      // channel id
	DH_RealPlayType     realplayType;    // real play type
	char                reserve[32];
}DHDEV_IN_MULTIPLAY_PARAM;

// multiplay output param
typedef struct
{
	int                 iChannelID;      // channel id
	LLONG                lRealHandle;     // real play handle
	char                reserve[32];
}DHDEV_OUT_MULTIPLAY_PARAM;
/////////////////////////////////Embedded platform/////////////////////////////////

// Platform embedded setup - U China Network Communication(CNC)
typedef struct
{
    BOOL				bChnEn;
    char				szChnId[DH_INTERVIDEO_UCOM_CHANID];
} DH_INTERVIDEO_UCOM_CHN_CFG;

typedef struct
{
	DWORD				dwSize;
	BOOL				bFuncEnable;			// Enable connection
	BOOL				bAliveEnable;			// Enable alive
	DWORD				dwAlivePeriod;			// Alive period. Unit is second. The value ranges from 0 to 3600.
	char				szServerIp[DH_MAX_IPADDR_LEN];			// CMS IP
	WORD				wServerPort;							// CMS Port
    char				szRegPwd[DH_INTERVIDEO_UCOM_REGPSW];	// Registration password 
	char				szDeviceId[DH_INTERVIDEO_UCOM_DEVID];	// Device id
	char				szUserName[DH_INTERVIDEO_UCOM_USERNAME];
	char				szPassWord[DH_INTERVIDEO_UCOM_USERPSW];
    DH_INTERVIDEO_UCOM_CHN_CFG  struChnInfo[DH_MAX_CHANNUM];	// Channel id,en
} DHDEV_INTERVIDEO_UCOM_CFG;

//  Platform embedded setup - Alcatel
typedef struct
{
	DWORD				dwSize;
    unsigned short		usCompanyID[2];			// Company ID,Value. the different three party service company. Now use the first array considering the four bytes in line.
    char				szDeviceNO[32];			// Strings including '\0' are 32 bytes.
    char				szVSName[32];			// Front-end device name. Strings including '\0' are 16 bytes.
    char				szVapPath[32];			// VAP path
    unsigned short		usTcpPort;				// TCP port,value:value ranges from 1 to 65535 
    unsigned short		usUdpPort;				// UDP port,Value:Value ranges from 1 to 65535
    bool				bCsEnable[4];			// Enable central server symbol. Value:true=enable,false=disable.Now only use the first array considering the four bytes in line.
    char				szCsIP[16];				// Central server IP address.Strings including '\0' are 16 bytes.
    unsigned short		usCsPort[2];			// Central server port. Value ranges from 1 to 65535.Now only use the first array considering the four bytes in line.
    bool				bHsEnable[4];			// Enable pulse server symbol. Value:true-enable,false=disable.Now only use the first array considering the four bytes in line.
    char				szHsIP[16];				// Pulse server IP address. Strings including '\0' are 16 bytes.
    unsigned short		usHsPort[2];			// Pulse server port. Value ranges from 1 to 65535.Now only use the first array considering the four bytes in line
    int					iHsIntervalTime;		// Pulse server interval. Value(unit is second)
    bool				bRsEnable[4];			// Enable registration server symbol. Value:,true=enable,false=disable.Now only use the first array considering the four bytes in line. 
    char				szRsIP[16];				// Registration server IP address. Strings including '\0' are 16 bytes.
    unsigned short		usRsPort[2];			// Registration server port. Value. The value ranges from 1 to 65535.Now use the first array considering the four bytes in line
    int					iRsAgedTime;			// Registration server valid duration. Value(unit is hour)
    char				szAuthorizeServerIp[16];// IP Authorization server IP
    unsigned short		usAuthorizePort[2];		// Authorization server port. Now only use the first array considering the four bytes in line
    char				szAuthorizeUsername[32];// Authorization server account
    char				szAuthorizePassword[36];// Authorization server password
    
    char				szIpACS[16];			// ACS( auto registration server) IP
    unsigned short		usPortACS[2];			// ACS Port,Now only use the first array considering the four bytes in line. 
    char				szUsernameACS[32];		// ACS user name
    char				szPasswordACS[36];		// ACS password
    bool				bVideoMonitorEnabled[4];// DVS regularly uploads front-end video monitor message or not.Value:true=enable,false=disable
    int					iVideoMonitorInterval;	// Upload interval(minute)
    
    char				szCoordinateGPS[64];	// GPS coordinates
    char				szPosition[32];			// Device position
    char				szConnPass[36];			// Device connection bit 
} DHDEV_INTERVIDEO_BELL_CFG;

//  Platform embedded setup - ZTE Netview
typedef struct  
{
	DWORD				dwSize;
	unsigned short		nSevPort;								// Server port. Value. The value ranges from 1 to 65535
    char				szSevIp[DH_INTERVIDEO_NSS_IP];			// Server IP address,string,including '\0' end symbol, there are total 32byte.
    char				szDevSerial[DH_INTERVIDEO_NSS_SERIAL];	// Front-end device serial number, string including '\0' end symbol, there are total 32byte.
    char				szUserName[DH_INTERVIDEO_NSS_USER];
    char				szPwd[DH_INTERVIDEO_NSS_PWD];
} DHDEV_INTERVIDEO_NSS_CFG;

// Platform embedded setup - AMP
typedef struct  
{
	char               szDevSerial[DH_INTERVIDEO_AMP_DEVICESERIAL];                  // Front Device Serial num(encoder serial num), read only	
	char               szDevName[DH_INTERVIDEO_AMP_DEVICENAME];                      // Front Device Name(encoder name), read only
	char               szRegUserName[DH_INTERVIDEO_AMP_USER];                        // Server Name
	char               szRegPwd[DH_INTERVIDEO_AMP_PWD];                              // Server Password
	BYTE			   bReserved[128];
} DHDEV_INTERVIDEO_AMP_CFG;  
////////////////////////////////HDVR special use//////////////////////////////////
// Alarm relay structure
typedef struct 
{
	/* Message triggered methods,multiple methods,including
	 * 0x00000001 - alarm upload
	 * 0x00000002 - triggering recording
	 * 0x00000004 - PTZ movement
	 * 0x00000008 - sending email
	 * 0x00000010 - local tour
	 * 0x00000020 - local tips
	 * 0x00000040 - alarm output
	 * 0x00000080 - ftp upload
	 * 0x00000100 - buzzer
	 * 0x00000200 - voice tips 
	 * 0x00000400 - snapshot
	*/

	/* Current alarm supporting methods, shown by bit mask */
	DWORD				dwActionMask;

	/* Triggering action,shown by bit mask,concrete action parameter is shows in the configuration */
	DWORD				dwActionFlag;
	
	/* Triggering alarm output channel,1 means triggering this output */ 
	BYTE				byRelAlarmOut[DH_MAX_ALARMOUT_NUM_EX];
	DWORD				dwDuration;				/* Alarm lasting period */

	/* Triggering recording */
	BYTE				byRecordChannel[DH_MAX_VIDEO_IN_NUM_EX]; /* Record channel triggered by alarm,1 means triggering this channel */
	DWORD				dwRecLatch;				/* Recording period */

	/* Snapshot channel */
	BYTE				bySnap[DH_MAX_VIDEO_IN_NUM_EX];
	/* Tour channel */
	BYTE				byTour[DH_MAX_VIDEO_IN_NUM_EX];

	/* PTZ movement */
	DH_PTZ_LINK			struPtzLink[DH_MAX_VIDEO_IN_NUM_EX];
	DWORD				dwEventLatch;			/* Event delay time, s for unit,range is 0~15,default is 0 */
	/* Alarm trigerring wireless output,alarm output,1 for trigerring output */ 
	BYTE				byRelWIAlarmOut[DH_MAX_ALARMOUT_NUM_EX];
	BYTE				bMessageToNet;
	BYTE                bMMSEn;                /*Message triggering alarm enabling*/
	BYTE                bySnapshotTimes;       /* the number of sheets of drawings */
	BYTE				bMatrixEn;				/*!< Matrix output enable */
	DWORD				dwMatrix;				/*!< Matrix mask */			
	BYTE				bLog;					/*!< Log enable,only used in WTN motion detection */
	BYTE				bSnapshotPeriod;		/*!<Snapshot frame interval. System can snapshot regularly at the interval you specify here. The snapshot amount in a period of time also has relationship with the snapshot frame rate. 0 means there is no interval, system just snapshot continuously.*/
	BYTE				byTour2[DH_MAX_VIDEO_IN_NUM_EX];/* Tour channel 32-63*/
	BYTE                byEmailType;             /*<0,picture,1,record>*/
	BYTE                byEmailMaxLength;        /*<max record length,unit:MB>*/
	BYTE                byEmailMaxTime;          /*<max time length, unit:second>*/
	BYTE				byReserved[475];   
} DH_MSG_HANDLE_EX;

// External alarm expansion
typedef struct
{
	BYTE				byAlarmType;			// Alarm type,0:normal closed,1:normal open
	BYTE				byAlarmEn;				// Alarm enable
	BYTE				byReserved[2];						
	DH_TSECT			stSect[DH_N_WEEKS][DH_N_REC_TSECT]; //NSP
	DH_MSG_HANDLE_EX	struHandle;				// Handle method
} DH_ALARMIN_CFG_EX, *LPDHDEV_ALARMIN_CFG_EX; 

// Motion detection alarm
typedef struct 
{
	BYTE				byMotionEn;				// Motion detection alarm enabling
	BYTE				byReserved;
	WORD				wSenseLevel;			// Sensitivity
	WORD				wMotionRow;				// Motion detection area rows
	WORD				wMotionCol;				// Motion detection area lines
	BYTE				byDetected[DH_MOTION_ROW][DH_MOTION_COL]; // Motion detection area,most 32*32
	DH_TSECT			stSect[DH_N_WEEKS][DH_N_REC_TSECT];	//NSP
	DH_MSG_HANDLE_EX		struHandle;				//Handle method
} DH_MOTION_DETECT_CFG_EX;

// fire alarm
typedef struct
{
	BOOL                byFireEn;              // enable
	DH_MSG_HANDLE_EX    struHandle;            // handler
	BYTE                byReserved[128];
}DHDEV_FIRE_ALARM_CFG;

// Static detection alarm

typedef struct 
{
	BYTE				byStaticEn;				// Static detection alarm enabling
	BYTE				byLatch;                // detect delay (old struct)
	WORD				wSenseLevel;			// Sensitivity
	WORD				wStaticRow;				// Static detection area rows
	WORD				wStaticCol;				// Static detection area lines
	BYTE				byDetected[DH_STATIC_ROW][DH_STATIC_COL]; // Static detection area,most 32*32
	DH_TSECT			stSect[DH_N_WEEKS][DH_N_REC_TSECT];	//NSP
	DH_MSG_HANDLE_EX		struHandle;				//Handle method
	int                 nLatch;                 // detect delay (new struct)
	BYTE                bReserved[64];          // reserved
} DH_STATIC_DETECT_CFG_EX;

// ACC power off configuration
typedef struct _DHDEV_ACC_POWEROFF_CFG
{
	BOOL                bEnable;               // ACC power off configuration enable
	int                 nDelayTime;            // ACC power off delay time(m)
	DH_MSG_HANDLE_EX    struHandle;            // handle method
	BYTE                Reserved[128];         // reserved
}DHDEV_ACC_POWEROFF_CFG;

// explosion proof alarm configuration
typedef struct _DHDEV_EXPLOSION_PROOF_CFG
{
	BOOL                bEnable;               // explosion proof alarm configuration enable
	DH_MSG_HANDLE_EX    struHandle;            // handle method
	BYTE                Reserved[128];         // reserved
}DHDEV_EXPLOSION_PROOF_CFG;

// Raid evnet config
typedef struct _DHDEV_RAID_EVENT_CFG
{
	BOOL                bEnable;               // enable
	DH_MSG_HANDLE_EX    struHandle;            // handle
	BYTE                Reserved[128];         // 
}DHDEV_RAID_EVENT_CFG;

// Video loss alarm
typedef struct
{
	BYTE				byAlarmEn;				// Video loss alarm enabling
	BYTE				byReserved[3];
	DH_TSECT			stSect[DH_N_WEEKS][DH_N_REC_TSECT];	//NSP
	DH_MSG_HANDLE_EX	struHandle;				// Handle method
} DH_VIDEO_LOST_CFG_EX;

// Camera masking alarm
typedef struct
{
	BYTE				byBlindEnable;			// Enable
	BYTE				byBlindLevel;			// Sensitivity 1-6
	BYTE				byReserved[2];
	DH_TSECT			stSect[DH_N_WEEKS][DH_N_REC_TSECT];	//NSP
	DH_MSG_HANDLE_EX	struHandle;				// Handle method
} DH_BLIND_CFG_EX;

// HDD info(alarm)
typedef struct 
{
	BYTE				byNoDiskEn;				// No HDD alarm
	BYTE				byReserved_1[3];
	DH_TSECT			stNDSect[DH_N_WEEKS][DH_N_REC_TSECT]; //NSP
	DH_MSG_HANDLE_EX	struNDHandle;			// Handle method
	
	BYTE				byLowCapEn;				// Low capacity alarm
	BYTE				byLowerLimit;			// Lower limit,0-99
	BYTE				byReserved_2[2];
	DH_TSECT			stLCSect[DH_N_WEEKS][DH_N_REC_TSECT]; //NSP
	DH_MSG_HANDLE_EX	struLCHandle;			// Handle method
	
	BYTE				byDiskErrEn;			// HDD error alarm
	BYTE				bDiskNum;
	BYTE				byReserved_3[2];
	DH_TSECT			stEDSect[DH_N_WEEKS][DH_N_REC_TSECT]; //NSP
	DH_MSG_HANDLE_EX	struEDHandle;			// Handle method
} DH_DISK_ALARM_CFG_EX;

typedef struct
{
	BYTE				byEnable;
	BYTE				byReserved[3];
	DH_MSG_HANDLE_EX	struHandle;
} DH_NETBROKEN_ALARM_CFG_EX;

// Front encoder configuration parameter
typedef struct __DEV_ENCODER_CFG 
{
	int					nChannels;				// Digital channel number
	DEV_ENCODER_INFO	stuDevInfo[32];			// Digital channel info
	BYTE				byHDAbility;			// The max high definition video amount the digital channel supported. (0 means it does not support high definition setup.)
												// If it supports high definition setup, the high definition channel ranges from 0 to N-1. If your high definition channel number is more than N, the save operation may fail. 
	BYTE				bTVAdjust;				// TV adjust. 0:not support, 1:support.
	BYTE				bDecodeTour;			// Decode tour. 0:not support, greater than zero: it means the maximal number supported.
	BYTE				bRemotePTZCtl;			// Remote PTZ control. 0:not support, 1:support.
	char				reserved[12];
} DEV_ENCODER_CFG, *LPDEV_ENCODER_CFG;

// front end access configuratiion parameter expansion
typedef struct __DEV_ENCODER_CFG_EX 
{
	int					nChannels;				// number of channel
	DEV_ENCODER_INFO	stuDevInfo[128];	    // the front end of the every digital channel encoder information
	BYTE				byHDAbility;			// maximum number of hd video(0 means not support)

	BYTE				bTVAdjust;				// support TV adjust,0:not support,1:support.
	BYTE				bDecodeTour;			// support tour or not, 0:not support, >0:the current number of device support
	BYTE				bRemotePTZCtl;			// support remote PTZ control
	char				reserved[256];
} DEV_ENCODER_CFG_EX, *LPDEV_ENCODER_CFG_EX;
// Controller
typedef struct tagDHCONFIG_CONTROLER{
	DH_COMM_PROP        struComm;	                     // Com attribution
	BYTE                bLightGroup[DH_MAX_LIGHT_NUM];   // Controlled light group,light number start from 1,such as[1,3,8,0?-0]present control light number 1,3,8 light
	BYTE	            bDeviceAddr;	                 // Controller address
	BYTE                bComPortType;                    // Com Type 0:485 COM, 1:232 COM
	BYTE                bReserved[6];		             // Reserved
} DH_CONFIG_CONTROLER;

// Light control config
typedef struct __DHDEV_LIGHTCONTROL_CFG
{
	DH_CONFIG_CONTROLER    struControlers[DH_MAX_CONTROLER_NUM]; // Control config
	BYTE                   bControlerNum;                        // Effective controller number
	BYTE                   bReserved[1023];                      // Reserved
} DHDEV_LIGHTCONTROL_CFG, *LPDHDEV_LIGHTCONTROL_CFG;

// 3G Flow info config
typedef struct
{
	int                 nStrategy;       // strategy, 0: charged by flow every month 1:charged by time every month
	int                 nUpLimit;        // up limit, by flow: MB, by time: h
	int                 nreserved[22];   // reserved
}DHDEV_3GFLOW_INFO_CFG;

// IPv6 config
typedef struct 
{
	char               szLinkAddr[44];	 // link address: string length = 44;(every host has an exclusive link address, read only)
	char               szHostIP[40]; 	 // host IP
	DWORD              dwPreFix;		 // net prefix, 1-128
	char               szGateWay[40];	 // gateway
	BOOL               bAutoGet;		 // enable of auto get ip 
	char               szPrimaryDns[40]; // primary DNS
	char               szSecondDns[40];	 // second DNS
    char               cReserved[256];   // reserved
}DHDEV_IPV6_CFG;

//Emergency storage configuration
typedef struct __DEV_URGENCY_RECORD_CFG
{
	DWORD dwSize;           // Structure body size
	BOOL bEnable;           // Enable or not. 1=enable.0=disable
	UINT nMaxRecordTime;    // Max record time. Unit is second.
}DHDEV_URGENCY_RECORD_CFG;

// Lift running parameter configuration
typedef struct __DEV_ELEVATOR_ATTRI_CFG
{
	DWORD dwSize;           // Structure body size
	UINT   nMAXFloorNum;     //Max floor. Min is 2.
	UINT   nFloorDelayTime;  //Stay verification time ranges from 5-60. It regards as stop current floor if keep this time period.
	UINT   nIntervalTime;    //The max time for the lift to go up/down a floor. It regars the lift is malfunction if the time is out of the threshold. The lift stopsa between two floors.
}DHDEV_ELEVATOR_ATTRI_CFG;

// Virtual camera status search
typedef struct tagDHDEV_VIRTUALCAMERA_STATE_INFO
{
	DWORD              nStructSize;                  // Structure body size
	int                nChannelID;                   // Channel No.
	CONNECT_STATE      emConnectState;               // Connection status
	UINT               uiPOEPort;                    // The PoE port the virtual camera connected to. 0=It is not a PoE connection.
	char               szDeviceName[64];             // Device name
	char               szDeviceType[128];            // Device type
	char               szSystemType[128];            // system type
	char               szSerialNo[DH_SERIALNO_LEN];  // serial no
	int                nVideoInput;                  // video input number
	int                nAudioInput;                  // audio input number
	int                nAlarmOutput;                 // alarm output number
}DHDEV_VIRTUALCAMERA_STATE_INFO;

// Device working video/loop mode status info and etc search 
typedef struct tagDHDEV_TRAFFICWORKSTATE_INFO
{
	DWORD                nStructSize;      // Structure body size
	int                  nChannelID;       // Channel No.
	DH_TRAFFIC_SNAP_MODE emSnapMode;       // Snap mode
}DHDEV_TRAFFICWORKSTATE_INFO;

/////////////////////////////////ITS picture subscription interface parameter/////////////////////////////////
typedef struct RESERVED_DATA_INTEL_BOX
{
	DWORD  dwEventCount;	 //Event count
	DWORD* dwPtrEventType;	 //Point to continuous value of event type, user should request and fr
    DWORD  dwInternalTime;      // ͼƬ�ϴ����ʱ�䣬��λ��s
	BYTE   bReserved[1020];	 //Reserved
}ReservedDataIntelBox;

#define RESERVED_TYPE_FOR_INTEL_BOX 0x00000001
typedef struct RESERVED_PARA
{
	DWORD 	dwType;	//pData's type
					//when [dwType] is RESERVED_TYPE_FOR_INTEL_BOX, pData is address of type ReservedDataIntelBox					
					//when [dwType] is ...,[pData] is ...
	void*	pData;	//data
}ReservedPara;
#define RESERVED_TYPE_FOR_COMMON   0x00000010
typedef struct tagNET_RESERVED_COMMON
{
	DWORD            dwStructSize;
	ReservedDataIntelBox* pIntelBox;    // include RESERVED_TYPE_FOR_INTEL_BOX
	DWORD            dwSnapFlagMask;	// snap flags(by bit),0bit:"*",1bit:"Timing",2bit:"Manual",3bit:"Marked",4bit:"Event",5bit:"Mosaic",6bit:"Cutout"
}NET_RESERVED_COMMON;

/////////////////////////////////Intelligent speed dome control interface parameter/////////////////////////////////
// Scene structure info
typedef struct 
{
	DWORD       dwSize;
	int         nScene;			//Scene SN
}DHDEV_INTELLI_SCENE_INFO;

// Scene margin limit position info
typedef struct 
{
	DWORD       dwSize;
	int         nScene;			//Scene SN
	int         nType;			//0:Top margin limit;1:Bottom margin limit;2:Left margin;3:Right margin
}DHDEV_INTELLI_SCENELIMIT_INFO;

// Manually track object structure body info
typedef struct
{
	DWORD       dwSize;
	int         nObjectID;		// Object ID -1 = Set the intelligent frame upload the position of any object out of the frame in the Web >=0: select the intelligent frame to send the object in the frame. 
	RECT        stuBound;		// Rectangle range. The coordinates of the point is within [0,8192].
}DHDEV_INTELLI_TRACKOBJECT_INFO;

typedef enum __TRACKCONTROL_TYPE
{
	DH_TRACK_MARKSCENE,				// Specified scene. Corresponding to the structure body of the DHDEV_INTELLI_SCENE_INFO
		DH_TRACK_GOTOSCENE,			// Go to scene. Corresponding to the structure body of the DHDEV_INTELLI_SCENE_INFO
		DH_TRACK_MARKSCENELIMIT,	// The margin limit position of the specified scene. Corresponding to the structure body of the DHDEV_INTELLI_SCENELIMIT_INFO
		DH_TRACK_GOTOSCENELIMIT,	// Go to the marin limit position of the scene. Corresponding to the structure body of the DHDEV_INTELLI_SCENELIMIT_INFO
		DH_TRACK_MARKSCENEMAXZOOM,	// The max track rate in the specified scene. Corresponding to the structure body of the DHDEV_INTELLI_SCENE_INFO
		DH_TRACK_OBJECT,			// The tracking object in the selected scene. Corresponding to the structure body of the DHDEV_INTELLI_TRACKOBJECT_INFO
		DH_TRACK_START,				// Begin intelligent track. No need to specify parameter information.
		DH_TRACK_STOP,				// Stop intelligent track. No need to specify parameter information.
		DH_TRACK_TYPE_NUM,
}DH_TRACKCONTROL_TYPE;

//Intelligent speed dome control input parameter
typedef struct tagNET_IN_CONTROL_INTELLITRACKER
{
    DWORD       dwSize;
    int         nChannelID;            // Channel ID
    DH_TRACKCONTROL_TYPE emCtrlType;   // Control type
    void*       pCtrlInfo;             // The corresponding inform structure pointer of the control type. Please refer to DH_TRACKCONTROL_TYPE for definition.
    int         nMaxLen;               // Structure body size of the control information
	int         nWaittime;             // Wait time out time
}NET_IN_CONTROL_INTELLITRACKER;

//Intelligent speed dome control output parameter
typedef struct tagNET_OUT_CONTROL_INTELLITRACKER
{
    DWORD       dwSize;
}NET_OUT_CONTROL_INTELLITRACKER;


/////////////////////////////////Abandoned Type/////////////////////////////////

// Search device working status channel information. Corresponding interfaces have been abandoned. Do not user.
typedef struct
{
	BYTE				byRecordStatic;			// Channel is recording or not. ;0: does not record,1: manual record,2:auto record  
	BYTE				bySignalStatic;			// Connected signal status ;0:normal,1:signal loss  
	BYTE				byHardwareStatic;		// Channel hardware status;0:normal ,1:abnormal such as DSP is down , 
	char				reserve;
	DWORD				dwBitRate;				// Actual bit rate
	DWORD				dwLinkNum;				// The client-end connected amount 
	DWORD				dwClientIP[DH_MAX_LINK];// Client-end IP address 
} NET_DEV_CHANNELSTATE, *LPNET_DEV_CHANNELSTATE;

// Search device working status. The corresponding interfaces have been abandoned. Please do not use
typedef struct
{
	DWORD				dwDeviceStatic;			// Device status;0x00:normal ,0x01:CPU occupation is too high ,0x02:hardware error 
	NET_DEV_DISKSTATE	stHardDiskStatic[DH_MAX_DISKNUM]; 
	NET_DEV_CHANNELSTATE stChanStatic[DH_MAX_CHANNUM];	// Channel status
	BYTE				byAlarmInStatic[DH_MAX_ALARMIN];// Alarm port status;0:no alarm,1: alarm
	BYTE				byAlarmOutStatic[DH_MAX_ALARMOUT]; // Alarm output port status;0:no alarm,1:alarm  
	DWORD				dwLocalDisplay;			// Local display status  ;0:normal,1:abnormal 
} NET_DEV_WORKSTATE, *LPNET_DEV_WORKSTATE;

// Protocol information
typedef struct 
{
	char				protocolname[12];		// Protocol name
	unsigned int		baudbase;				// Baud rate
	unsigned char		databits;				// Data bit
	unsigned char		stopbits;				// Stop bit
	unsigned char		parity;					// Pairty bit 
	unsigned char		reserve;
} PROTOCOL_INFO, *LPPROTOCOL_INFO;

// Audio talk parameter setup
typedef struct 
{
	// Audio input parameter
	BYTE				byInFormatTag;			// Encode type such as PCM
	BYTE				byInChannels;			// Track amount
	WORD				wInBitsPerSample;		// Sampling depth 				
	DWORD				dwInSamplesPerSec;		// Sampling rate 

	// Audio output parameter
	BYTE				byOutFormatTag;			// Encode type such as PCM
	BYTE				byOutChannels;			// Track amount
	WORD				wOutBitsPerSample;		// Sampling depth 		
	DWORD				dwOutSamplesPerSec;		// Sampling rate
} DHDEV_TALK_CFG, *LPDHDEV_TALK_CFG;

/////////////////////////////////// Matrix ///////////////////////////////////////

#define DH_MATRIX_INTERFACE_LEN		16		// Signal interface name length
#define DH_MATRIX_MAX_CARDS			128		// Matrix sub card max amount
#define DH_SPLIT_PIP_BASE			1000	// The basic value of the PIP split mode
#define	DH_MAX_SPLIT_MODE_NUM		64		// Max split matrix amount
#define DH_MATRIX_MAX_CHANNEL_IN	1500	// Matrix max input channel amount
#define DH_MATRIX_MAX_CHANNEL_OUT	256		// Matrix max output channel amount
#define DH_DEVICE_NAME_LEN			64		// Device name length
#define DH_MAX_CPU_NUM				16		// Max CPU amount
#define DH_MAX_FAN_NUM				16		// Max fan amount
#define DH_MAX_POWER_NUM			16		// Max power amount
#define DH_MAX_BATTERY_NUM          16      // ���������
#define DH_MAX_TEMPERATURE_NUM		32		// Max temperature sensor amount
#define DH_MAX_ISCSI_NAME_LEN		128		// ISCSI Name length
#define DH_VERSION_LEN				64		// Version info length
#define DH_MAX_STORAGE_PARTITION_NUM 32		//  Storage partition max number
#define DH_STORAGE_MOUNT_LEN		64		// Mount length
#define DH_STORAGE_FILE_SYSTEM_LEN	16		// File system name length
#define DH_MAX_MEMBER_PER_RAID		32		// RAID max members
#define DH_DEV_ID_LEN_EX			128		// Device ID max length
#define DH_MAX_BLOCK_NUM			32		// max number of block
#define DH_MAX_SPLIT_WINDOW			128		// max number of split window
#define DH_FILE_TYPE_LEN			64		// length of file type
#define DH_DEV_ID_LEN				128		// length of device ID
#define DH_DEV_NAME_LEN				128		// max length of device name  
#define DH_TSCHE_DAY_NUM			8		// schedule the first dimension size, means days
#define DH_TSCHE_SEC_NUM			6		// schedule the second dimension size, means time
#define    DH_SPLIT_INPUT_NUM           256         // ˾���豸�����л�ʱ��һ��split֧�ֵ�����ͨ����

#define DH_DEVICE_ID_LOCAL		"Local"		// local device ID
#define DH_DEVICE_ID_REMOTE		"Remote"	// remote device ID
#define DH_DEVICE_ID_UNIQUE		"Unique"	// unique ID

// �ָ�ģʽ
typedef enum tagDH_SPLIT_MODE
{
	DH_SPLIT_1 = 1,							// 1-window
	DH_SPLIT_2 = 2, 						// 2-window
	DH_SPLIT_4 = 4, 						// 4-window
	DH_SPLIT_6 = 6, 						// 6-window
	DH_SPLIT_8 = 8, 						// 8-window
	DH_SPLIT_9 = 9, 						// 9-window
	DH_SPLIT_12 = 12, 						// 12-window
	DH_SPLIT_16 = 16, 						// 16-window
	DH_SPLIT_20 = 20, 						// 20-window
	DH_SPLIT_25 = 25, 						// 25-window
	DH_SPLIT_36 = 36, 						// 36-window
	DH_SPLIT_64 = 64, 						// 64-window
	DH_SPLIT_144 = 144, 					// 144-window
	DH_PIP_1 = DH_SPLIT_PIP_BASE + 1,		// PIP mode.1-full screen+1-small window
	DH_PIP_3 = DH_SPLIT_PIP_BASE + 3,		// PIP mode.1-full screen+3-small window
	DH_SPLIT_FREE = DH_SPLIT_PIP_BASE * 2,	// free open window mode,are free to create,close, window position related to the z axis
	DH_COMPOSITE_SPLIT_1 = DH_SPLIT_PIP_BASE * 3 + 1,	// integration of a split screen
	DH_COMPOSITE_SPLIT_4 = DH_SPLIT_PIP_BASE * 3 + 4,	// fusion of four split screen
} DH_SPLIT_MODE;

#define DH_PROTOCOL_DAHUA2 DH_PROTOCOL_PRIVATE2
#define DH_PROTOCOL_DAHUA3 DH_PROTOCOL_PRIVATE3

// Device protocol type
typedef enum tagDH_DEVICE_PROTOCOL
{
	DH_PROTOCOL_PRIVATE2,						// private 2nd protocol
	DH_PROTOCOL_PRIVATE3,						// private 3rd protocol
	DH_PROTOCOL_ONVIF,						// Onvif	
	DH_PROTOCOL_VNC,						// virtual network computer
    DH_PROTOCOL_TS,                         // ��׼TS
    
	DH_PROTOCOL_PRIVATE = 100,              // private protocol of private        
	DH_PROTOCOL_AEBELL,                     // aebell
	DH_PROTOCOL_PANASONIC,                  // panasonic       
	DH_PROTOCOL_SONY,                       // sony   
	DH_PROTOCOL_DYNACOLOR,                  // Dynacolor        
	DH_PROTOCOL_TCWS,						// tcsw        
	DH_PROTOCOL_SAMSUNG,                    // sansung        
	DH_PROTOCOL_YOKO,                       // YOKO        
	DH_PROTOCOL_AXIS,                       // axis        
	DH_PROTOCOL_SANYO,						// sanyo       		
	DH_PROTOCOL_BOSH,						// Bosch		
	DH_PROTOCOL_PECLO,						// Peclo		
	DH_PROTOCOL_PROVIDEO,					// Provideo		
	DH_PROTOCOL_ACTI,						// ACTi		
	DH_PROTOCOL_VIVOTEK,					// Vivotek		
	DH_PROTOCOL_ARECONT,					// Arecont        
	DH_PROTOCOL_PRIVATEEH,			        // PrivateEH	        
	DH_PROTOCOL_IMATEK,					    // IMatek        
	DH_PROTOCOL_SHANY,                      // Shany        
	DH_PROTOCOL_VIDEOTREC,                  // videotrec        
	DH_PROTOCOL_URA,						// Ura        
	DH_PROTOCOL_BITICINO,                   // Bticino         
	DH_PROTOCOL_ONVIF2,                     // Onvif's protocol type, same to DH_PROTOCOL_ONVIF    
	DH_PROTOCOL_SHEPHERD,                   // shepherd        
	DH_PROTOCOL_YAAN,                       // yaan      
	DH_PROTOCOL_AIRPOINT,					// Airpop        
	DH_PROTOCOL_TYCO,                       // TYCO        
	DH_PROTOCOL_XUNMEI,                     // xunmei      
	DH_PROTOCOL_HIKVISION,                  // hikvision        
	DH_PROTOCOL_LG,                         // LG        
	DH_PROTOCOL_AOQIMAN,					// aoqiman       
	DH_PROTOCOL_BAOKANG,                    // baokang            
	DH_PROTOCOL_WATCHNET,                   // Watchnet        
	DH_PROTOCOL_XVISION,                    // Xvision        
	DH_PROTOCOL_FUSITSU,                    // fusitsu        
	DH_PROTOCOL_CANON,						// Canon		
	DH_PROTOCOL_GE,							// GE		
	DH_PROTOCOL_Basler,						// basler		
	DH_PROTOCOL_Patro,						// patro	    
	DH_PROTOCOL_CPKNC,						// CPPLUS K series		
	DH_PROTOCOL_CPRNC,						// CPPLUS R series		
	DH_PROTOCOL_CPUNC,						// CPPLUS U series		
	DH_PROTOCOL_CPPLUS,						// CPPLUS IPC		
	DH_PROTOCOL_XunmeiS,					// xunmeis,protocal is Onvif		
	DH_PROTOCOL_GDDW,						// GDDW		
	DH_PROTOCOL_PSIA,                       // PSIA        
	DH_PROTOCOL_GB2818,                     // GB2818	        
	DH_PROTOCOL_GDYX,                       // GDYX        
	DH_PROTOCOL_OTHER,                      // others   
} DH_DEVICE_PROTOCOL;

// Split mode info
typedef struct tagDH_SPLIT_MODE_INFO
{
	DWORD				dwSize;
	DH_SPLIT_MODE		emSplitMode;			// Split mode
	int					nGroupID;				// Group SN
    DWORD               dwDisplayType;          // ��ʾ���ͣ������DH_SPLIT_DISPLAY_TYPE��ע�͸�ģʽ����ʾ������"PicInPic"����, ��ģʽ����ʾ���ݰ�NVD���й����������DisChn�ֶξ����������ݣ�û����һ����ʱ��Ĭ��Ϊ��ͨ��ʾ����,��"General"��
} DH_SPLIT_MODE_INFO;

// Split capability
typedef struct tagDH_SPLIT_CAPS 
{
	DWORD				dwSize;
	int					nModeCount;				// The split amount supported
	DH_SPLIT_MODE		emSplitMode[DH_MAX_SPLIT_MODE_NUM];	// The split mode supported
	int				    nMaxSourceCount;		// Max display source allocation amount
	int					nFreeWindowCount;		// count of free window support
	BOOL				bCollectionSupported;	// support collection
    DWORD               dwDisplayType;                          // �����ʾ�����ʾ���ͣ������DH_SPLIT_DISPLAY_TYPE��ע�͸�ģʽ����ʾ������"PicInPic"����, ��ģʽ����ʾ���ݰ�NVD���й����������DisChn�ֶξ����������ݣ�û����һ����ʱ��Ĭ��Ϊ��ͨ��ʾ����,��"General"��
    int                 nPIPModeCount;                          // ���л�֧�ֵķָ�ģʽ����
    DH_SPLIT_MODE       emPIPSplitMode[DH_MAX_SPLIT_MODE_NUM];  // ���л�֧�ֵķָ�ģʽ
    int                 nInputChannels[DH_SPLIT_INPUT_NUM];     // ֧�ֵ�����ͨ��
    int                 nInputChannelCount;                     // ֧�ֵ�����ͨ������, 0��ʾû������ͨ������
} DH_SPLIT_CAPS;

// even the authentication information
typedef struct tagDH_CASCADE_AUTHENTICATOR
{
	DWORD				dwSize;
	char				szUser[DH_NEW_USER_NAME_LENGTH];		// user name
	char				szPwd[DH_NEW_USER_PSW_LENGTH];			// passwd
	char				szSerialNo[DH_SERIALNO_LEN];			// serial no.
} DH_CASCADE_AUTHENTICATOR;
// Display source
typedef struct tagDH_SPLIT_SOURCE
{
	DWORD				dwSize;
	BOOL			    bEnable;						    // Enable
	char			    szIp[DH_MAX_IPADDR_LEN];		    // IP, null means there is no setup.
	char			    szUser[DH_USER_NAME_LENGTH];	    // User name
	char			    szPwd[DH_USER_PSW_LENGTH];	    	// Password
	int			    	nPort;							    // Port
	int				    nChannelID;						    // Channel No.
	int				    nStreamType;					    // Video bit stream. -1-auto, 0-main stream, 1-extra stream 1, 2-extra stream 2, 3-extra stream 3
	int				    nDefinition;					    // Definition, 0-standard definition, 1-high definition
	DH_DEVICE_PROTOCOL  emProtocol;							// Protocol type
	char			    szDevName[DH_DEVICE_NAME_LEN];		// Device name
	int					nVideoChannel;						// Video input channel amount
	int					nAudioChannel;						// Audio input channel amount
	// For decoder only
	BOOL				bDecoder;							// Decoder or not.
	BYTE				byConnType;							// 0:TCP;1:UDP;2:multicast
	BYTE				byWorkMode;							// 0:connect directly; 1:transfer 
	WORD				wListenPort;						// isten port, valid with transfer; when byConnType is multicast, it is multiport
	char				szDevIpEx[DH_MAX_IPADDR_OR_DOMAIN_LEN];// szDevIp extension, front DVR Ip address (enter domain name)
	BYTE				bySnapMode;                         //  snapshot mode (valid when nStreamType==4) 0: request for single frame, 1: sechdule sending request
	BYTE				byManuFactory;						// Target device manufacturer. Refer to EM_IPC_TYPE for detailed information.
	BYTE				byDeviceType;                       //  target device type, 0:IPC
	BYTE				byDecodePolicy;                     // target device decode policy, 0:compatible with previous  
															// 1:real time level high 2: real time level medium
															// 3: real time level low 4: default level 
															// 5: fluency level high 6: fluency level medium
															// 7: fluency level low
	DWORD				dwHttpPort;                         // Http port number, 0-65535
	DWORD				dwRtspPort;                         // Rtsp port number, 0-65535
	char				szChnName[DH_DEVICE_NAME_LEN];		// Remote channel name, modifiable only when name read is not vacant
	char				szMcastIP[DH_MAX_IPADDR_LEN];       // Multicast IP address. Valid only when byConnType is multicast
	char				szDeviceID[DH_DEV_ID_LEN_EX];		// device ID, ""-null, "Local"  "Remote"
	BOOL				bRemoteChannel;						// is remote channel or not(read only)
	unsigned int		nRemoteChannelID;					// remote channel ID (read only), effective when bRemoteChannel=TRUE
	char				szDevClass[DH_DEV_TYPE_LEN];		// type of device, such as IPC, DVR, NVR and so on
	char				szDevType[DH_DEV_TYPE_LEN];			// device specific type, such as IPC-HF3300
	char				szMainStreamUrl[MAX_PATH];			// main stream url, effective when byManuFactory =D H_IPC_OTHER
	char				szExtraStreamUrl[MAX_PATH];			// extra stream url, effective when byManuFactory =D H_IPC_OTHER
	int					nUniqueChannel;						// unique channel ID, read only
	DH_CASCADE_AUTHENTICATOR stuCascadeAuth;				// ssascade authemyication, effective when device ID = "Local/Cascade/SerialNo",  SerialNo is device seral no.
	} DH_SPLIT_SOURCE;

// Video output capability set
typedef struct tagDH_VIDEO_OUT_CAPS 
{
	DWORD				dwSize;
	BOOL				bColor;							// Support color output setup or not
	BOOL				bMargin;						// Support margin setup or not
	int					nLayers;						// The max layers supported at the same time
	BOOL				bPowerMode;						// Support power control or not.
	int					bSplitMode;						// The video split mode supported. 0-1-window. 1-'#' mode(inclduing 1-window).2-Any mode
} DH_VIDEO_OUT_CAPS;

// Color RGBA
typedef struct tagDH_COLOR_RGBA
{
	int					nRed;							// Red
	int					nGreen;							// Green
	int					nBlue;							// Blue
	int					nAlpha;							// Transparent
} DH_COLOR_RGBA;

// Color BCSH
typedef struct tagDH_COLOR_BCSH 
{
	int					nBirghtness;					// Brightness
	int					nContrast;						// Contrast
	int			        nSaturation;					// Saturation
	int					nHue;						    // Hue
} DH_COLOR_BCSH;

// Dimensions
typedef struct tagDH_SIZE 
{
	int					nWidth;							// Width
	int					nHeight;						// Height
} DH_SIZE;

// mode of hot plug
typedef struct tagDH_HOT_PLUG_MODE
{
	DWORD				dwSize;
	int					nMode;							// mode of hot plug, 0-hot plug, 1-force output mode
} DH_HOT_PLUG_MODE;
// Video output option 
typedef struct tagDH_VIDEO_OUT_OPT
{
	DWORD				dwSize;
	DH_RECT*			pstuMargin;						// Margin range
	DH_COLOR_BCSH*		pstuColor;						// Output color
	DH_COLOR_RGBA*		pstuBackground;					// Background color
	DH_SIZE*			pstuSize;						// Output dimensions
	DH_HOT_PLUG_MODE*	pstuHotPlugMode;				// mode of hot plug
} DH_VIDEO_OUT_OPT;

// Production definition
typedef struct tagDH_PRODUCTION_DEFNITION
{
	DWORD				dwSize;
	int					nVideoInChannel;				// Video input channel amount
	int					nVideoOutChannel;				// Video output channel amount
	int					nRemoteDecChannel;				// Remote decode channel amount
	char				szDevType[DH_DEV_TYPE_LEN];		// Device type
	char				szVendor[DH_MAX_NAME_LEN];		// OEM customer
	int					nOEMVersion;					// OEM version
	int					nMajorVerion;					// Main version No.
	int					nMinorVersion;					// SUb version No.
	int					nRevision;						// Revision version
	char				szWebVerion[DH_MAX_NAME_LEN];	// Web version
	char				szDefLanguage[DH_MAX_NAME_LEN];	// Default setup
	NET_TIME			stuBuildDateTime;				// Release time. Unit is second.
	int					nAudioInChannel;				// Audio input channel amount
	int					nAudioOutChannel;				// Audio output channel amount
	BOOL				bGeneralRecord;					// Support schedule storage or not.
	BOOL				bLocalStore;					// Support local storage or not.
	BOOL				bRemoteStore;					// Support network storage or not.
	BOOL				bLocalurgentStore;				// Support emergency local storage or not.
	BOOL				bRealtimeCompress;				// Support real-time compression storage or not.
	DWORD				dwVideoStandards;				// The video format supported. bit0-PAL, bit1-NTSC
	int					nDefVideoStandard;				// Default video format, 0-PAL, 1-NTSC
	int					nMaxExtraStream;				// Max extra stream channel amount
	int					nRemoteRecordChannel;			// Remote record channel amount
	int					nRemoteSnapChannel;				// Remote snap channel amount
	int					nRemoteVideoAnalyseChannel;		// Remote video analysis channel amount
	int					nRemoteTransmitChannel;			// Remote real-time stream transmit max channel amount
	int					nRemoteTransmitFileChannel;		// Remote transmit file channel amount
	int					nStreamTransmitChannel;			// Max network transmit channel amount
	int					nStreamReadChannel;				// Max read file channel amount
	int					nMaxStreamSendBitrate;			// Max bit stream network send capability, kbps
	int					nMaxStreamRecvBitrate;			// Max bit stream network interface capability, kbps
	BOOL				bCompressOldFile;				// Old compression file or not. Delete P frame and save I frame.
	BOOL				bRaid;							// Support RAID or not.
	int					nMaxPreRecordTime;				// Max pre-record time.Unit is second.
	BOOL				bPtzAlarm;						// Support PTZ alarm or not.
	BOOL				bPtz;							// Support PTZ or not.
	BOOL				bATM;							// Support corresponding function of the ATM or not.
	BOOL				b3G;							// Support 3G module or not.
	BOOL				bNumericKey;					// With number button or not.
	BOOL				bShiftKey;						// With Shift button or not.
	BOOL				bCorrectKeyMap;					// Number character map sheet is right or not.
	BOOL				bNewATM;						// The new 2nd ATM front panel.
	BOOL				bDecoder;						// Decoder or not
	DEV_DECODER_INFO	stuDecoderInfo;					// Decoder info. Valid when bDecoder=true.bDecoder=true
	int					nVideoOutputCompositeChannels;	// integration ceiling screen output channel
	BOOL                bSupportedWPS;                  // support WPS or not
	int					nVGAVideoOutputChannels;		// VGA video output channel number
	int					nTVVideoOutputChannels;			// TV video output channel number
	int					nMaxRemoteInputChannels;			// max number of remote channels
	int					nMaxMatrixInputChannels;			// max number of matrix channels
	int                 nMaxRoadWays;                   // max counts of road ways 1~6
	int                 nMaxParkingSpaceScreen;         // max counts of screen when docking with the camera 0~20

	int					nPtzHorizontalAngleMin;			// PTZ'horizontal minimum Angle, [0-360]
	int					nPtzHorizontalAngleMax;			// PTZ'horizontal maximum Angle, [0-360]
	int					nPtzVerticalAngleMin;			// PTZ'vertical  minimum Angle, [-90,90]
	int					nPtzVerticalAngleMax;			// PTZ'vertical  maximum Angle, [-90,90]
	BOOL				bPtzFunctionMenu;				// Whether to support PTZ's function menu 
	BOOL				bLightingControl;				// Whether to support lighting control 
	DWORD				dwLightingControlMode;			// Manual lighting control mode,bitwise,see NET_LIGHTING_CTRL_ON_OFF
	int					nNearLightNumber;				// dipped headlight group number, 0 means no support 
	int					nFarLightNumber;				// High beam group number, 0 means no support
	BOOL				bFocus;							// Whether to support control focus 
	BOOL				bIris;							// Whether to support control aperture 
	char				szPtzProtocolList[DH_COMMON_STRING_1024];	// PTZ support agreement list, can be more, each with '; 'delimited 
	BOOL				bRainBrushControl;				// Whether to support wiper control 
	int					nBrushNumber;					// Number of wiper, 0 means no support
	int					nLowerMatrixInputChannels[DH_MAX_LOWER_MITRIX_NUM];	// inferior video matrix input channel, the subscript corresponding matrix number 
	int					nLowerMatrixOutputChannels[DH_MAX_LOWER_MITRIX_NUM];	// inferior video matrix output channel, the subscript corresponding matrix number 
} DH_PRODUCTION_DEFNITION;

// Manual lighting control mode 
#define NET_LIGHTING_CTRL_ON_OFF				0x01	// Direct switch mode 
#define NET_LIGHTING_CTRL_ADJUST_BRIGHTNESS		0x02	// Manually adjust brightness mode 
#define NET_LIGHTING_CTRL_ZOOM_PRIO				0x04	// Ratio prefer 


// Matrix sub card type. Various setups.
#define DH_MATRIX_CARD_MAIN				0x10000000		// main card
#define DH_MATRIX_CARD_INPUT			0x00000001		// input card 
#define DH_MATRIX_CARD_OUTPUT			0x00000002		// output card
#define DH_MATRIX_CARD_ENCODE			0x00000004		// encode card
#define DH_MATRIX_CARD_DECODE			0x00000008		// decode card
#define DH_MATRIX_CARD_CASCADE			0x00000010		// cascade card
#define DH_MATRIX_CARD_INTELLIGENT		0x00000020		// intelligent card
#define DH_MATRIX_CARD_ALARM                0x00000040          // alarm card 

// Matrix sub card info
typedef struct tagDH_MATRIX_CARD
{
	DWORD				dwSize;
	BOOL				bEnable;					// Valid or not
	DWORD				dwCardType;					// Sub card type
	char		    	szInterface[DH_MATRIX_INTERFACE_LEN];	// Signal interface type, "CVBS", "VGA", "DVI"...
	char		    	szAddress[DH_MAX_IPADDR_OR_DOMAIN_LEN];	// Device IP or domain name. The sub card that has no network conneciton can be null.
	int			    	nPort;						// Port No. The sub card that has no network conneciton can be 0.
	int					nDefinition;				// Definition. 0=standard definition. 1=High definition
	int					nVideoInChn;				// Video input channel amount
	int					nAudioInChn;				// Audio input channel amount
	int					nVideoOutChn;				// Video output channel amount
	int				    nAudioOutChn;				// Audio output channel amount
	int			    	nVideoEncChn;				// Video encode channel amount
	int				    nAudioEncChn;				// Audio encode channel amount
	int			    	nVideoDecChn;				// Video decode channel amount
	int			    	nAudioDecChn;				// Audio decode channel amount
	int					nStauts;					// Status: 0-OK, 1-no respond, 2-network disconnection, 3-conflict, 4-upgrading now
	int					nCommPorts;					// COM amount
	int					nVideoInChnMin;				// Video input channel min value
	int					nVideoInChnMax;				// Video input channel max value
	int					nAudioInChnMin;				// Audio input channel min value
	int					nAudioInChnMax;				// Audio input channel max value
	int					nVideoOutChnMin;			// Video output channel min value
	int					nVideoOutChnMax;			// Video output channel max value
	int					nAudioOutChnMin;			// Audio output channel min value
	int					nAudioOutChnMax;			// Audio output channel max value	
	int					nVideoEncChnMin;			// Video encode channel min value
	int					nVideoEncChnMax;			// Video encode channel max value
	int					nAudioEncChnMin;			// Audio encode channel min value
	int					nAudioEncChnMax;			// Audio encode channel max value
	int					nVideoDecChnMin;			// Video decode channel min value
	int					nVideoDecChnMax;			// Video decode channel max value
	int					nAudioDecChnMin;			// Audio decode channel min value
	int					nAudioDecChnMax;			// Audio decode channel max value
	int					nCascadeChannels;			// number of cascade channel
	int					nCascadeChannelBitrate;		// cascade channel bitrate (Mbps)
	int					nAlarmInChnCount;			// Alarm input channel number 
	int					nAlarmInChnMin;				// Alarm input channel number minimum value 
	int					nAlarmInChnMax;				// Alarm input channel number maximum value 
	int					nAlarmOutChnCount;			// Alarm output channel number 
	int					nAlarmOutChnMin;			// Alarm output channel number minimum value
	int					nAlarmOutChnMax;			// Alarm output channel number maximum value
	int					nVideoAnalyseChnCount;		// Intelligent analysis of channel number 
	int					nVideoAnalyseChnMin;		// Intelligent analysis of channel number minimum value 
	int					nVideoAnalyseChnMax;		// Intelligent analysis of channel number maximum value
	int					nCommPortMin;				// minimum value of serial port number  
	int					nCommPortMax;				// maximum value of serial port number
    char                szVersion[DH_COMMON_STRING_32];         // �汾��Ϣ
    NET_TIME            stuBuildTime;                           // ����ʱ��
} DH_MATRIX_CARD;

// Matrix sub card list
typedef struct tagDH_MATRIX_CARD_LIST 
{
	DWORD				dwSize;
	int					nCount;							// sub card  amount
	DH_MATRIX_CARD		stuCards[DH_MATRIX_MAX_CARDS];	// Sub card list 
} DH_MATRIX_CARD_LIST;

// Video output window
typedef struct tagDH_VIDEO_OUT_WINDOW
{
	DWORD				dwSize;
	int					nBackboardID;				// Backboard ID
	int					nScreenID;					// Screen ID
	int					nWindowID;					// Window ID
} DH_VIDEO_OUT_WINDOW;

// CLIENT_GetISCSITargets
typedef struct tagDH_IN_ISCSI_TARGETS 
{
	DWORD				dwSize;
	const char*			pszAddress;					// Server address
	int					nPort;						// port
	const char*			pszUser;					// Username
	const char*			pszPwd;						// Password
} DH_IN_ISCSI_TARGETS;

// ISCSI Target Info
typedef struct tagDH_ISCSI_TARGET 
{
	DWORD				dwSize;
	char				szName[DH_MAX_ISCSI_NAME_LEN];	// Name
	char				szAddress[DH_MAX_IPADDR_OR_DOMAIN_LEN];	// service address
	char				szUser[DH_NEW_USER_NAME_LENGTH];// user name
	int					nPort;							// port
	UINT				nStatus;						// status, 0- unknow, 1-connected, 2-un connected, 3-connect failed, 4-authentication failed, 5-connect time out	
} DH_ISCSI_TARGET;

// CLIENT_GetISCSITargets Interface output parameter
typedef struct tagDH_OUT_ISCSI_TARGETS
{
	DWORD				dwSize;
	DH_ISCSI_TARGET*	pstuTargets;				// iscsi array
	int					nMaxCount;					// iscsi group size
	int					nRetCount;					// Received iscSi amount
} DH_OUT_ISCSI_TARGETS;

typedef enum tagDH_BITMAP_ACCURACY
{
    BA_DAY ,            //day 
    BA_HOUR ,           //day
    BA_5MIN ,           //5 miuntes
}DH_BITMAP_ACCURACY;

#define DH_MAX_BITMAPS        256                        // Bitmap bytes��DH_MAX_BITMAPS*8-bit
#define DH_MAX_CHN_NUM        256                        // The maximum channel number 
//CLIENT_GetBitmap Interface input parameters 
typedef struct tagDH_IN_BITMAP
{
    DWORD               dwSize;
    NET_TIME            stuStartTime;                   // start time, accurate to seconds 
    NET_TIME            stuEndTime;                     // Over time, accurate to seconds 
    int                 nChnList[DH_MAX_CHN_NUM];       // [1, ��, 10]	Channel number list ,-1 means all channels ,0 means channels end
    DH_BITMAP_ACCURACY  emAccuracy;                     // Precision of the bitmap     
}DH_IN_BITMAP;

typedef struct tagDH_BITMAP_INFO
{
    DWORD               dwSize;
    int                 nChnID;                         //Channel number��Channel number is 0��means the end of the list 
    BYTE                bBitmap[DH_MAX_BITMAPS];        //Bitmap (each bit means a query scale, from low to high filling) 
                                                        //support DH_MAX_BITMAPS * 8 scale
    int                 nBitNum;                        //the digits of bitmap 
}DH_BITMAP_INFO;

//CLIENT_GetBitmap  Interface output parameters 
typedef struct tagDH_OUT_BITMAP
{
    DWORD               dwSize;
    DH_BITMAP_INFO      *pstBitmapInfos;
    int                 nGetBitmapInfo;
    int                 nMaxBitmapInfo;    
}DH_OUT_BITMAP;

// Storage device name
typedef struct tagDH_STORAGE_DEVICE_NAME 
{
	DWORD				dwSize;
	char				szName[DH_STORAGE_NAME_LEN];
} DH_STORAGE_DEVICE_NAME;

// RAID state
#define DH_RAID_STATE_ACTIVE		0x00000001
#define DH_RAID_STATE_INACTIVE		0x00000002
#define DH_RAID_STATE_CLEAN			0x00000004
#define DH_RAID_STATE_FAILED		0x00000008
#define DH_RAID_STATE_DEGRADED		0x00000010
#define DH_RAID_STATE_RECOVERING	0x00000020
#define DH_RAID_STATE_RESYNCING		0x00000040
#define DH_RAID_STATE_RESHAPING		0x00000080
#define DH_RAID_STATE_CHECKING		0x00000100
#define DH_RAID_STATE_NOTSTARTED	0x00000200

// RAID��Ա��Ϣ
typedef struct tagNET_RAID_MEMBER_INFO 
{
    DWORD               dwSize;
    DWORD               dwID;                                       // ���̺�, ���������������ڴŹ�Ĳ�λ
    BOOL                bSpare;                                     // �Ƿ�ֲ��ȱ�, true-�ֲ��ȱ�, false-RAID����
} NET_RAID_MEMBER_INFO;

// RAID Info
typedef struct tagDH_STORAGE_RAID
{
	DWORD				dwSize;
	int					nLevel;										// level
	int					nState;										// RAID state combinationDH_RAID_STATE_ACTIVE | DH_RAID_STATE_DEGRADED
	int					nMemberNum;									// member amount
	char				szMembers[DH_MAX_MEMBER_PER_RAID][DH_STORAGE_NAME_LEN];	// RAID member
    float               fRecoverPercent;                            // ͬ���ٷֱ�, 0~100, RAID״̬����"Recovering"��"Resyncing"ʱ��Ч
    float               fRecoverMBps;                               // ͬ���ٶ�, ��λMBps, RAID״̬����"Recovering"��"Resyncing"ʱ��Ч
    float               fRecoverTimeRemain;                         // ͬ��ʣ��ʱ��, ��λ����, RAID״̬����"Recovering"��"Resyncing"ʱ��Ч
    NET_RAID_MEMBER_INFO stuMemberInfos[DH_MAX_MEMBER_PER_RAID];    // RAID��Ա��Ϣ
} DH_STORAGE_RAID;

// Storage partition info
typedef struct tagDH_STORAGE_PARTITION
{
	DWORD				dwSize;
	char				szName[DH_STORAGE_NAME_LEN];				// Name
	INT64				nTotalSpace;   							    //Total space(MB) 
	INT64				nFreeSpace;								    // free space(MB)
	char				szMountOn[DH_STORAGE_MOUNT_LEN];			// Mount point
	char				szFileSystem[DH_STORAGE_FILE_SYSTEM_LEN];	//File system
	int					nStatus;									// partition state, 0-LV not available, 1-LV available
} DH_STORAGE_PARTITION;

// storage tank info
typedef struct tagDH_STORAGE_TANK 
{
	DWORD				dwSize;
	int					nLevel;										// level, the host is 0 level
	int					nTankNo;									// extend port number from 0
} DH_STORAGE_TANK;

// �洢�豸״̬
#define NET_STORAGE_DEV_OFFLINE                 0                   // ����Ӳ���ѻ�״̬
#define NET_STORAGE_DEV_RUNNING                 1                   // ����Ӳ������״̬
#define NET_STORAGE_DEV_ACTIVE                  2                   // RAID�
#define NET_STORAGE_DEV_SYNC                    3                   // RAIDͬ��
#define NET_STORAGE_DEV_SPARE                   4                   // RAID�ȱ�(�ֲ�)
#define NET_STORAGE_DEV_FAULTY                  5                   // RAIDʧЧ
#define NET_STORAGE_DEV_REBUILDING              6                   // RAID�ؽ�
#define NET_STORAGE_DEV_REMOVED                 7                   // RAID�Ƴ�
#define NET_STORAGE_DEV_WRITE_ERROR             8                   // RAIDд����
#define NET_STORAGE_DEV_WANT_REPLACEMENT        9                   // RAID��Ҫ���滻
#define NET_STORAGE_DEV_REPLACEMENT             10                  // RAID������豸
#define NET_STORAGE_DEV_GLOBAL_SPARE            11                  // ȫ���ȱ�
#define NET_STORAGE_DEV_ERROR                   12                  // ����, ���ַ�������
#define NET_STORAGE_DEV_RAIDSUB                 13                  // ����Ŀǰ�ǵ���, ԭ���ǿ�Raid����, �п������������Զ�����Raid

// Storage device info
typedef struct tagDH_STORAGE_DEVICE 
{
	DWORD				dwSize;
	char				szName[DH_STORAGE_NAME_LEN];				// name
	INT64				nTotalSpace;								// Total space (MB) 
	INT64				nFreeSpace;									// free space (MB) 
	BYTE				byMedia;									// Media, 0-DISK, 1-CDROM, 2-FLASH medium,  
	BYTE				byBUS;										// BUS, 0-ATA, 1-SATA, 2-USB, 3-SDIO, 4-SCSI main line 0-ATA, 1-SATA, 2-USB, 3-SDIO, 4-SCSI
	BYTE				byVolume;									// volume type, 0-physics, 1-Raid, 2- VG virtual 
	BYTE				byState;									// Physics disk state, 0-physics disk offline state 1-physics disk  2- RAID activity 3- RAID sync 4-RAID hotspare 5-RAID invalidation 6- RAID re-creation  7- RAID delete
	int					nPhysicNo;									// storage interface of devices of same type logic number
	int					nLogicNo;									// storage interface of devices of same type physics number
	char				szParent[DH_STORAGE_NAME_LEN];				// superior storage group name
	char				szModule[DH_STORAGE_NAME_LEN];				// device module
	char				szSerial[DH_SERIALNO_LEN];					// device serial number
	char				szFirmware[DH_VERSION_LEN];					// Firmware version
	int					nPartitionNum;								//partition number
	DH_STORAGE_PARTITION stuPartitions[DH_MAX_STORAGE_PARTITION_NUM];// partition info
	DH_STORAGE_RAID		stuRaid;									// Raid info, for RAID use only(byVolume == 1) 
	DH_ISCSI_TARGET		stuISCSI;									// Iscsi info, for iscsi use only (byVolume == 2)
	BOOL				abTank;										// tank enable
	DH_STORAGE_TANK		stuTank;									// tank info, effectice when abTank = TRUE
} DH_STORAGE_DEVICE; 

// OSD channel info
typedef struct tagNET_SPLIT_OSD 
{
    DWORD               dwSize;
    BOOL                bEnable;                    // enable
    DH_COLOR_RGBA       stuFrontColor;              // The foreground color 
    DH_COLOR_RGBA       stuBackColor;               // The background color 
    DH_RECT             stuFrontRect;               // Foreground area 
    DH_RECT             stuBackRect;                // Background region 
    BOOL                bRoll;                      // Whether the scroll display, applies to the text only 
    BYTE                byRollMode;                 // Scroll mode, applies only to text, 0 -from left to right,1-from right to left 
    BYTE                byRoolSpeed;                // Rolling speed, applies to text  only , 0 ~ 4, the greater the value the faster the scrolling 
    BYTE                byFontSize;                 // The font size, applies to the text only  
    BYTE                byTextAlign;                // Alignment, 0 - left, 1 - centered, 2 - right 
    BYTE                byType;                     // OSD type, 0 - text, 1 - icon 
    BYTE                Reserved[3];                // retain byte
    char                szContent[MAX_PATH];        // OSD content, if the type is icon, the content is the name of the icon 
} NET_SPLIT_OSD;

// CLIENT_GetSplitOSD's interface input param(get split window input OSD info)
typedef struct tagDH_IN_SPLIT_GET_OSD
{
	DWORD					dwSize;
	int						nChannel;				// channel no.
	int						nWindow;				// window no.
} DH_IN_SPLIT_GET_OSD;

// CLIENT_GetSplitOSD's interface input param(get split window output OSD info)
typedef struct tagDH_OUT_SPLIT_GET_OSD
{
	DWORD					dwSize;
    int                 nOSDNum;                            // OSD number
    NET_SPLIT_OSD       stuOSD[DH_VIDEO_CUSTOM_OSD_NUM];    // OSD information
} DH_OUT_SPLIT_GET_OSD;

// CLIENT_SetSplitOSD's interface input param(setting split window input OSD info)
typedef struct tagDH_IN_SPLIT_SET_OSD
{
	DWORD					dwSize;
	int						nChannel;				// channel no.
	int						nWindow;				// window no.
    int                 nOSDNum;                            // OSD number
    NET_SPLIT_OSD       stuOSD[DH_VIDEO_CUSTOM_OSD_NUM];    // OSD information
} DH_IN_SPLIT_SET_OSD;

// CLIENT_SetSplitOSD's interface input param(setting split window output OSD info)
typedef struct tagDH_OUT_SPLIT_SET_OSD
{
	DWORD					dwSize;
} DH_OUT_SPLIT_SET_OSD;

// Video output control method 
typedef enum
{
    EM_VIDEO_OUT_CTRL_CHANNEL,              // Logical channel number control mode,effective for physical screen and splicing screen 
    EM_VIDEO_OUT_CTRL_COMPOSITE_ID,         // Splice screen ID control mode, applies to splice screen only  
} EM_VIDEO_OUT_CTRL_TYPE;

// CLIENT_SetSplitSourceEx  The input parameters of the interface 
typedef struct tagNET_IN_SET_SPLIT_SOURCE 
{
    DWORD                   dwSize;
    EM_VIDEO_OUT_CTRL_TYPE  emCtrlType;         // Video output control method 
    int                     nChannel;           // Video output logical channel number,when the emCtrlType is EM_VIDEO_OUT_CTRL_CHANNEL effective
    const char*             pszCompositeID;     // Splicing screen ID, when the emCtrlType is EM_VIDEO_OUT_CTRL_CHANNEL effective
    int                     nWindow;            // winder number, -1 means all windows of the current segmentation mode 
    DH_SPLIT_SOURCE*        pstuSources;        // Video source information, when nWindow=-1, Video source is an array, and the number and the window number
    int                     nSourceCount;       // Video source number 
} NET_IN_SET_SPLIT_SOURCE;

// Set the return result of video source  
typedef struct tagNET_SET_SPLIT_SOURCE_RESULT 
{
    DWORD                   dwSize;
    int                     nPushPort;          // Monitor Port Number of Push Flow Pattern Equipment 
} NET_SET_SPLIT_SOURCE_RESULT;

// CLIENT_SetSplitSourceEx output parameters of the interface 
typedef struct tagNET_OUT_SET_SPLIT_SOURCE
{
    DWORD                   dwSize;
    NET_SET_SPLIT_SOURCE_RESULT* pstuResults;   // returned value after successful setting , corresponding the window array in NET_IN_SET_SPLIT_SOURCE, user allocates memory , If don't need can be NULL 
    int                     nMaxResultCount;    // The size of the pstuResults array
    int                     nRetCount;          // The Number Of Return
} NET_OUT_SET_SPLIT_SOURCE;

// CLIENT_MatrixSwitch The input parameters 
typedef struct tagNET_IN_MATRIX_SWITCH
{
    DWORD           dwSize;
    DH_SPLIT_MODE   emSplitMode;                // Segmentation Model 
    int*            pnOutputChannels;           // Output channel, can specify multiple output channel switch together at the same time, the content is consistent 
    int             nOutputChannelCount;        // Output channel number 
    int*            pnInputChannels;            // Input channel, each split window corresponding to one input channel 
    int             nInputChannelCount;         // Input channel number
} NET_IN_MATRIX_SWITCH;

// CLIENT_MatrixSwitch Output Parameters 

typedef struct tagNET_OUT_MATRIX_SWITCH 
{
    DWORD            dwSize;
} NET_OUT_MATRIX_SWITCH;

// CLIENT_SplitSetMultiSource The Input Parameters of the Interface 
typedef struct tagNET_IN_SPLIT_SET_MULTI_SOURCE 
{
    DWORD                   dwSize;
    EM_VIDEO_OUT_CTRL_TYPE  emCtrlType;         // Video output control method 
    int                     nChannel;           // Video output logical channel number,Effective When emCtrlType is EM_VIDEO_OUT_CTRL_CHANNEL
    const char*             pszCompositeID;     // Splicing screen ID,When emCtrlType is EM_VIDEO_OUT_CTRL_COMPOSITE_ID
    BOOL                    bSplitModeEnable;   // Whether to change segmentation model 
    DH_SPLIT_MODE           emSplitMode;        // Segmentation model,Enable when bSplitModeEnable=TRUE
    int                     nGroupID;           // Segmentation group number,Effective when bSplitModeEnable=TRUE
    int*                    pnWindows;          // Window Number Array 
    int                     nWindowCount;       // Window Number
    DH_SPLIT_SOURCE*        pstuSources;        // Video source information, corresponding to each window, the number with the number of window 
} NET_IN_SPLIT_SET_MULTI_SOURCE;

// CLIENT_SplitSetMultiSource The output parameters of the interface
typedef struct tagNET_OUT_SPLIT_SET_MULTI_SOURCE 
{
    DWORD                   dwSize;
} NET_OUT_SPLIT_SET_MULTI_SOURCE;

// ��Ƶ�ָ��������
typedef enum tagNET_SPLIT_OPERATE_TYPE
{
    NET_SPLIT_OPERATE_SET_BACKGROUND,            // ���ñ���ͼƬ, ��Ӧ NET_IN_SPLIT_SET_BACKGROUND �� NET_OUT_SPLIT_SET_BACKBROUND
    NET_SPLIT_OPERATE_GET_BACKGROUND,            // ��ȡ����ͼƬ, ��Ӧ NET_IN_SPLIT_GET_BACKGROUND �� NET_OUT_SPLIT_GET_BACKGROUND
} NET_SPLIT_OPERATE_TYPE;

// ������Ƶ�������ͼ�������
typedef struct tagNET_IN_SPLIT_SET_BACKGROUND
{
    DWORD            dwSize;
    int              nChannel;                   // ��Ƶ���ͨ����
    BOOL             bEnable;                    // ʹ��
    const char*      pszFileName;                // ����ͼ����
} NET_IN_SPLIT_SET_BACKGROUND;

// ������Ƶ�������ͼ�������
typedef struct tagNET_OUT_SPLIT_SET_BACKGROUND 
{
    DWORD            dwSize;
} NET_OUT_SPLIT_SET_BACKGROUND;

// ��ȡ��Ƶ�������ͼ�������
typedef struct tagNET_IN_SPLIT_GET_BACKGROUND 
{
    DWORD            dwSize;
    int              nChannel;                   // ��Ƶ���ͨ����
} NET_IN_SPLIT_GET_BACKGROUND;

// ��ȡ��Ƶ�������ͼ�������
typedef struct tagNET_OUT_SPLIT_GET_BACKGROUND 
{
    DWORD            dwSize;
    BOOL            bEnable;                            // ʹ��
    char            szFileName[DH_COMMON_STRING_256];   // ����ͼ����
} NET_OUT_SPLIT_GET_BACKGROUND;
////////////////////////////////// System status////////////////////////////////////////

// CPU info
typedef struct tagDH_CPU_INFO
{
	DWORD				dwSize;
	int					nUsage;						// CPU usage
} DH_CPU_INFO;

// CPU status
typedef struct tagDH_CPU_STATUS
{
	DWORD				dwSize;
	BOOL				bEnable;					// Search succeeded or not
	int					nCount;						// CPU amount
	DH_CPU_INFO			stuCPUs[DH_MAX_CPU_NUM];	// CPU info
} DH_CPU_STATUS;

// Memory info
typedef struct tagDH_MEMORY_INFO
{
	DWORD				dwSize;
	DWORD				dwTotal;					// Total memory, M
	DWORD				dwFree;						// Free memory, M
} DH_MEMORY_INFO;

// Memory status
typedef struct tagDH_MEMORY_STATUS 
{
	DWORD				dwSize;
	BOOL				bEnable;					// Search succeeded or not
	DH_MEMORY_INFO		stuMemory;					// Memory info
} DH_MEMORY_STATUS;

// Fan info
typedef struct tagDH_FAN_INFO
{
	DWORD				dwSize;
	char				szName[DH_DEVICE_NAME_LEN];	// Name
	DWORD				nSpeed;						// Speed
} DH_FAN_INFO;

// Fan status
typedef struct tagDH_FAN_STATUS
{
	DWORD				dwSize;
	BOOL				bEnable;					// Search succeeded or not
	int					nCount;						// Fan amount
	DH_FAN_INFO			stuFans[DH_MAX_FAN_NUM];	// Fan status
} DH_FAN_STATUS;

// Power info
typedef struct tagDH_POWER_INFO
{
	DWORD				dwSize;
	BOOL				bPowerOn;					// Power is on or not
} DH_POWER_INFO;

// Battery Information 
typedef struct tagDH_BATTERY_INFO
{
    DWORD               dwSize;
    int                 nPercent;                       // Battery Capacity Percentage
    BOOL                bCharging;                      // Whether real charging 
} DH_BATTERY_INFO;

// Power status
typedef struct tagDH_POWER_STATUS
{
	DWORD				dwSize;
	BOOL				bEnable;					//Search succeeded or not
	int					nCount;						// Power amount
	DH_POWER_INFO		stuPowers[DH_MAX_POWER_NUM];// Power status
    int                 nBatteryNum;                        // Battery Number
    DH_BATTERY_INFO     stuBatteries[DH_MAX_BATTERY_NUM];   // Battery Status 
} DH_POWER_STATUS;

// Temperature info
typedef struct tagDH_TEMPERATURE_INFO
{
	DWORD				dwSize;
	char				szName[DH_DEVICE_NAME_LEN];	// Sensor name
	float				fTemperature;				// Temperature
} DH_TEMPERATURE_INFO;

//Temperature status
typedef struct tagDH_TEMPERATURE_STATUS
{
	DWORD				dwSize;
	BOOL				bEnable;							// Search succeeded or not
	int					nCount;								// Temperature  amount
	DH_TEMPERATURE_INFO	stuTemps[DH_MAX_TEMPERATURE_NUM];	// Temperature  info
} DH_TEMPERATURE_STATUS;

// System status
typedef struct tagDH_SYSTEM_STATUS
{
	DWORD				dwSize;
	DH_CPU_STATUS*		pstuCPU;					// CPU status
	DH_MEMORY_STATUS*	pstuMemory;					// Memory status
	DH_FAN_STATUS*		pstuFan;					// Fan status
	DH_POWER_STATUS*	pstuPower;					// Power status
	DH_TEMPERATURE_STATUS*	pstuTemp;				// Temperature  status
} DH_SYSTEM_STATUS;

// Corresponding CLIENT_QueryDevState() Interface's DH_DEVSTATE_ALL_ALARM_CHANNELS_STATE Command Parameter 
// For Alarm Channel Status 
enum NET_ALARM_CHANNEL_TYPE
{
    NET_ALARM_CHANNEL_TYPE_ALL,                             // All channels (including all of the following)
    NET_ALARM_CHANNEL_TYPE_ALARMIN,                         // Alarm input channel 
    NET_ALARM_CHANNEL_TYPE_ALARMOUT,                        // Alarm output channel 
    NET_ALARM_CHANNEL_TYPE_ALARMBELL,                       // Signal channel 
    NET_ALARM_CHANNEL_TYPE_EXALARMIN,                       // Extension module alarm input channel 
    NET_ALARM_CHANNEL_TYPE_EXALARMOUT,                      // Extension module and alarm output channel
};

typedef struct tagNET_CLIENT_ALARM_CHANNELS_STATE
{
    DWORD                       dwSize;
    NET_ALARM_CHANNEL_TYPE      emType;                     // The type of query alarm channel 	                                                                                                                                
    int                         nAlarmInCount;              // Alarm input number, specified by the user 
    int                         nAlarmInRetCount;           // The number of returned alarm input 
    BOOL*                       pbAlarmInState;             // Alarm state of input arrays, memory allocated by the user, each element represents a channel status, TRUE for input, FALSE for no input 
    int                         nAlarmOutCount;             // The number of alarm output,specified by the user 
    int                         nAlarmOutRetCount;          // The number of alarm output 
    BOOL*                       pbAlarmOutState;            // Alarm state of output arrays, memory allocated by the user, each element represents a channel status, TRUE for output, FALSE for no output 
    int                         nAlarmBellCount;            // Alarm Number,specified by the user 
    int                         nAlarmBellRetCount;         // Returned Alarm Number 
    BOOL*                       pbAlarmBellState;           // Alarm state array, allocate memory by the user, each element represents a channel status, TRUE for output, FALSE for no output
    int                         nExAlarmInCount;            // Extension module alarm input number, specified by the user 
    int                         nExAlarmInRetCount;         // The number of returned extension module alarm input 
    BOOL*                       pbExAlarmInState;           // Extension module alarm input state array��Memory allocated by the user, each element represents a channel status, TRUE for output, FALSE for no output 
    int*                        pnExAlarmInDestionation;    // The location of the extension module alarm input effective channels 
    int                         nExAlarmOutCount;           // Extension module alarm output number, specified by the user
    int                         nExAlarmOutRetCount;        // Extension module alarm output number 
    BOOL*                       pbExAlarmOutState;          // Extension module alarm output state array��Memory allocated by the user, each element represents a channel status, TRUE for output, FALSE for no output 
    int*                        pnExAlarmOutDestionation;   // The location of the extension module alarm output effective channels
}NET_CLIENT_ALARM_CHANNELS_STATE;


// the number of alarm keyboard connecting on a serial port
typedef struct tagNET_ALARMKEYBOARD_COUNT
{
    DWORD               dwSize;
    int                 nAlarmKeyboardCount;        // The number of alarm keyboard connected 
}NET_ALARMKEYBOARD_COUNT;

////////////////////////////////// screen control////////////////////////////////////////
// CLIENT_OpenSplitWindow's interface input param(open window)
typedef struct tagDH_IN_SPLIT_OPEN_WINDOW
{
	DWORD				dwSize;
	int					nChannel;					// channel no.
	DH_RECT				stuRect;					// windon position, 0~8192
	BOOL				bDirectable;				// coordinate whether meet the confitions
} DH_IN_SPLIT_OPEN_WINDOW;

// CLIENT_OpenSplitWindow's interface output param(open window)
typedef struct tagDH_OUT_SPLIT_OPEN_WINDOW
{
	DWORD				dwSize;
	unsigned int		nWindowID;					// window ID
	unsigned int		nZOrder;					// window order	
} DH_OUT_SPLIT_OPEN_WINDOW;

// CLIENT_CloseSplitWindow's interface input param(close window)
typedef struct tagDH_IN_SPLIT_CLOSE_WINDOW
{
	DWORD				dwSize;
	int					nChannel;					// channel ID
	UINT				nWindowID;					// window order
} DH_IN_SPLIT_CLOSE_WINDOW;

// CLIENT_CloseSplitWindow's interface output param(close window)
typedef struct tagDH_OUT_SPLIT_CLOSE_WINDOW
{
	DWORD				dwSize;
} DH_OUT_SPLIT_CLOSE_WINDOW;

// CLIENT_SetSplitWindowRect's interface input param(setting the window position)
typedef struct tagDH_IN_SPLIT_SET_RECT
{
	DWORD				dwSize;
	int					nChannel;					// channel ID
	UINT				nWindowID;					// window order
	DH_RECT				stuRect;					// window position, 0~8192
	BOOL				bDirectable;				//  coordinate whether meet the confitions
} DH_IN_SPLIT_SET_RECT;

// CLIENT_SetSplitWindowRect's interface output param(setting the window position)
typedef struct tagDH_OUT_SPLIT_SET_RECT
{
	DWORD				dwSize;
} DH_OUT_SPLIT_SET_RECT;

// CLIENT_GetSplitWindowRect's interface input param(get window position)
typedef struct tagDH_IN_SPLIT_GET_RECT
{
	DWORD				dwSize;
	int					nChannel;					// channel ID
	UINT				nWindowID;					// window ID
} DH_IN_SPLIT_GET_RECT;

// CLIENT_GetSplitWindowRect's interface output param(get window position)
typedef struct tagDH_OUT_SPLIT_GET_RECT
{
	DWORD				dwSize;	
	DH_RECT				stuRect;					// window position, 0~8192
} DH_OUT_SPLIT_GET_RECT;

// cascading order window
typedef struct tagDH_WND_ZORDER
{
	DWORD				dwSize;
	unsigned int		nWindowID;					// window ID
	unsigned int		nZOrder;					// Z order
} DH_WND_ZORDER;

// CLIENT_SetSplitWindowLevels's interface input param(setting window order)
typedef struct tagDH_IN_SPLIT_SET_TOP_WINDOW
{
	DWORD				dwSize;
	int					nChannel;					// channel ID
	int					nWindowID;					// windown ID
} DH_IN_SPLIT_SET_TOP_WINDOW;

// CLIENT_SetSplitWindowLevels's interface output param(setting window order)
typedef struct tagDH_OUT_SPLIT_SET_TOP_WINDOW
{
	DWORD				dwSize;
	DH_WND_ZORDER*		pZOders;					// window order array
	int					nMaxWndCount;				// window order array size
	int					nWndCount;					// count of returned window
} DH_OUT_SPLIT_SET_TOP_WINDOW;

// CLIENT_SetDisplayMode's interface input param(monitor wall display mode settings)
typedef struct tagDH_IN_WM_SET_DISPLAY_MODE 
{
	DWORD				dwSize;
	int					nMonitorWallID;				// monitor wall ID
	const char*			pszBlockID;					// block ID, NULL/""-all region
	int					nTVID;						// display unit ID, -1 means all unit
	UINT				nDisplayMode;				// display mode, 0-standard, 1-highlighting, 2-energy saving
} DH_IN_WM_SET_DISPLAY_MODE;

// CLIENT_SetDisplayMode's interface output param(monitor wall display mode settings)
typedef struct tagDH_OUT_WM_SET_DISPLAY_MODE
{
	DWORD				dwSize;
} DH_OUT_WM_SET_DISPLAY_MODE;

// all display unit in the area of the display mode
typedef struct tagDH_BLOCK_DISPLAY_MODE
{
	DWORD				dwSize;
	int*				pTVModes;					// TV display mode
	int					nMaxTVCount;				// max count of tV
	int					nRetTVCount;				// count of retTV
} DH_BLOCK_DISPLAY_MODE;

// CLIENT_GetDisplayMode's interface input param(get monitor wall display mode)
typedef struct tagDH_IN_WM_GET_DISPLAY_MODE 
{
	DWORD				dwSize;
	int					nMonitorWallID;				// monitor wall ID
	const char*			pszBlockID;					// block ID, NULL/""-means all
	int					nTVID;						// TV ID, -1 means all
} DH_IN_WM_GET_DISPLAY_MODE;

// CLIENT_GetDisplayMode's interface output param(get monitor wall display mode)
typedef struct tagDH_OUT_WM_GET_DISPLAY_MODE
{
	DWORD				dwSize;
	DH_BLOCK_DISPLAY_MODE* pBlockModes;				// display mode
	int					nMaxBlockCount;				// count of array
	int					nRetBlockCount;				// count of ret 
} DH_OUT_WM_GET_DISPLAY_MODE;

// CLIENT_PowerControl's interface input param(monitor wall power control)
typedef struct tagDH_IN_WM_POWER_CTRL 
{
	DWORD				dwSize;
	int					nMonitorWallID;				// monitor wall ID
	const char*			pszBlockID;					// block ID, NULL/""- means all
	int					nTVID;						// TV ID, -1 means all
	BOOL				bPowerOn;					// power on or not
} DH_IN_WM_POWER_CTRL;

// CLIENT_PowerControl's interface input param(monitor wall power control)
typedef struct tagDH_OUT_WM_POWER_CTRL
{
	DWORD				dwSize;
} DH_OUT_WM_POWER_CTRL;

// CLIENT_LoadMonitorWallCollection's interface input param(load monitor wall plans)
typedef struct tagDH_IN_WM_LOAD_COLLECTION 
{
	DWORD				dwSize;
	int					nMonitorWallID;				// monitor wall ID
	const char*			pszName;					// name of plan
} DH_IN_WM_LOAD_COLLECTION;

// CLIENT_LoadMonitorWallCollection's interface output param(load monitor wall plans)
typedef struct tagDH_OUT_WM_LOAD_COLLECTION 
{
	DWORD				dwSize;
} DH_OUT_WM_LOAD_COLLECTION;

// CLIENT_SaveMonitorWallCollection's interface input param(save monitor wall plans)
typedef struct tagDH_IN_WM_SAVE_COLLECTION 
{
	DWORD				dwSize;
	int					nMonitorWallID;				// monitor wall ID
	const char*			pszName;					// name pf plan
} DH_IN_WM_SAVE_COLLECTION;

// CLIENT_SaveMonitorWallCollection's interface output param(load monitor wall plans)
typedef struct tagDH_OUT_WM_SAVE_COLLECTION 
{
	DWORD				dwSize;
} DH_OUT_WM_SAVE_COLLECTION;

// CLIENT_RenameMonitorWallCollection's interface input param(plan rename)
typedef struct tagDH_IN_WM_RENAME_COLLECTION 
{
	DWORD				dwSize;
	int					nMonitorWallID;				// monitor wall ID
	const char*			pszOldName;					// old name
	const char*			pszNewName;					// new name
} DH_IN_WM_RENAME_COLLECTION;

// CLIENT_RenameMonitorWallCollection's interface input param(plan rename)
typedef struct tagDH_OUT_WM_RENAME_COLLECTION
{
	DWORD				dwSize;
} DH_OUT_WM_RENAME_COLLECTION;

// infomation window areas
typedef struct tagDH_WINDOW_COLLECTION
{
	DWORD				dwSize;
	int					nWindowID;					// window ID
	BOOL				bWndEnable;					// enable
	DH_RECT				stuRect;					// rect, effect when free split mode
	BOOL				bDirectable;				// coordinate whether meet the conditions
	int					nZOrder;					// z order
	BOOL				bSrcEnable;					// source enable
	char				szDeviceID[DH_DEV_ID_LEN_EX]; // device ID
	int					nVideoChannel;				// video channel
	int					nVideoStream;				// video stream type
	int					nAudioChannel;				// audio channel
	int					nAudioStream;				// audio stream type
	int					nUniqueChannel;				// unique channel
} DH_WINDOW_COLLECTION;

// areas collection
typedef struct tagDH_BLOCK_COLLECTION 
{
	DWORD				dwSize;
	DH_SPLIT_MODE		emSplitMode;				// split mode
	DH_WINDOW_COLLECTION stuWnds[DH_MAX_SPLIT_WINDOW];// window info array
	int					nWndsCount;					// count of window
	char				szName[DH_DEVICE_NAME_LEN];	// favorites name
    int                  nScreen;                       // The output channel number, including the splicing screen 
} DH_BLOCK_COLLECTION;

// plan of monitor wall
typedef struct tagDH_MONITORWALL_COLLECTION 
{
	DWORD				dwSize;
	char				szName[DH_DEVICE_NAME_LEN];	// name
	DH_BLOCK_COLLECTION	stuBlocks[DH_MAX_BLOCK_NUM];// area array
	int					nBlocksCount;				// area count
	char				szControlID[DH_DEV_ID_LEN_EX];	// control ID
} DH_MONITORWALL_COLLECTION;

// CLIENT_GetMonitorWallCollections's interface input param(get plan infomation of monitor wall)
typedef struct tagDH_IN_WM_GET_COLLECTIONS 
{
	DWORD				dwSize;
	int					nMonitorWallID;				// monitor wall ID
} DH_IN_WM_GET_COLLECTIONS;

// CLIENT_GetMonitorWallCollections's interface output param(get plan infomation of monitor wall)
typedef struct tagDH_OUT_WM_GET_COLLECTIONS 
{
	DWORD				dwSize;	
	DH_MONITORWALL_COLLECTION* pCollections;		// plan of monitor wall array
	int					nMaxCollectionsCount;		// array size
	int					nCollectionsCount;			// max count of plan
} DH_OUT_WM_GET_COLLECTIONS;

// CLIENT_GetSplitWindowsInfo's interface input param
typedef struct tagDH_IN_SPLIT_GET_WINDOWS 
{
	DWORD				dwSize;
	int					nChannel;					// channel
} DH_IN_SPLIT_GET_WINDOWS;

// CLIENT_GetSplitWindowsInfo's interface output param
typedef struct tagDH_OUT_SPLIT_GET_WINDOWS 
{
	DWORD				dwSize;
	DH_BLOCK_COLLECTION	stuWindows;					// channel
} DH_OUT_SPLIT_GET_WINDOWS;

// CLIENT_LoadSplitCollection's interface input param(load collection)
typedef struct tagDH_IN_SPLIT_LOAD_COLLECTION 
{
	DWORD				dwSize;
	int					nChannel;					// channel
	const char*			pszName;					// name
} DH_IN_SPLIT_LOAD_COLLECTION;

// CLIENT_LoadSplitCollection's interface output param(load collection)
typedef struct tagDH_OUT_SPLIT_LOAD_COLLECTION 
{
	DWORD				dwSize;
} DH_OUT_SPLIT_LOAD_COLLECTION;

// CLIENT_SaveSplitCollection's interface input param(save collection)
typedef struct tagDH_IN_SPLIT_SAVE_COLLECTION 
{
	DWORD				dwSize;
	int					nChannel;					// channle
	const char*			pszName;					// name
} DH_IN_SPLIT_SAVE_COLLECTION;

// CLIENT_SaveSplitCollection's interface input param(save collection)
typedef struct tagDH_OUT_SPLIT_SAVE_COLLECTION 
{
	DWORD				dwSize;
} DH_OUT_SPLIT_SAVE_COLLECTION;

// CLIENT_RenameSplitCollection's interface input param(collection rename)
typedef struct tagDH_IN_SPLIT_RENAME_COLLECTION 
{
	DWORD				dwSize;
	int					nChannel;					// channel
	const char*			pszOldName;					// old name
	const char*			pszNewName;					// new name
} DH_IN_SPLIT_RENAME_COLLECTION;

// CLIENT_RenameSplitCollection's interface output param(collection rename)
typedef struct tagDH_OUT_SPLIT_RENAME_COLLECTION
{
	DWORD				dwSize;
} DH_OUT_SPLIT_RENAME_COLLECTION;

// CLIENT_GetSplitCollections's interface input param(get infomation of favorites)
typedef struct tagDH_IN_SPLIT_GET_COLLECTIONS 
{
	DWORD				dwSize;
	int					nChannel;					// channel
} DH_IN_SPLIT_GET_COLLECTIONS;

// CLIENT_GetSplitCollections's interface output param(get infomation of favorites)
typedef struct tagDH_OUT_SPLIT_GET_COLLECTIONS 
{
	DWORD				dwSize;	
	DH_BLOCK_COLLECTION* pCollections;				// array
	int					nMaxCollectionsCount;		// size
	int					nCollectionsCount;			// count
} DH_OUT_SPLIT_GET_COLLECTIONS;

// CLIENT_DeleteSplitCollection's interface input param(delete favorites)
typedef struct tagDH_IN_SPLIT_DELETE_COLLECTION 
{
	DWORD				dwSize;
	int					nChannel;					// channel
	const char**		ppszNames;					// array of favorites name
	int					nNameCount;					// size of array
} DH_IN_SPLIT_DELETE_COLLECTION;

// CLIENT_DeleteSplitCollection's interface input param(delete favorites)
typedef struct tagDH_OUT_SPLIT_DELETE_COLLECTION 
{
	DWORD				dwSize;
} DH_OUT_SPLIT_DELETE_COLLECTION;

// composite screen channel information
typedef struct tagDH_COMPOSITE_CHANNEL
{
	DWORD				dwSize;
	char				szMonitorWallName[DH_DEVICE_NAME_LEN];	// monitor wall name
	char				szCompositeID[DH_DEV_ID_LEN_EX];		// composite ID
	int					nVirtualChannel;						// virtual channel
} DH_COMPOSITE_CHANNEL;

// tour enable configuration
typedef struct tagDHDEV_TOUR_ENABLE
{
	DWORD				dwSize;
	BOOL				bEnable;					// enable
	BOOL				bStart;						// is touring or not(read only). bStart=FALSE when tour is able but doesn't set
} DHDEV_TOUR_ENABLE_CFG;

// CLIENT_SetDecodePolicy's interface input param(set the decoding policy)
typedef struct tagDH_IN_SET_DEC_POLICY 
{
	DWORD				dwSize;
	int					nChannel;			// channel
	int					nWindow;			// window no., -1 means all
	int					nPolicyLevel;		// policy level, a total of 5 file(-2, -1, 0, 1, 2), the greater the vale ,the bigger the fluid but delay
											// -2 real-time good, 2 fluency good, 0 default
} DH_IN_SET_DEC_POLICY;

// CLIENT_SetDecodePolicy's interface output param(set the decoding policy)
typedef struct tagDH_OUT_SET_DEC_POLICY 
{
	DWORD				dwSize;
} DH_OUT_SET_DEC_POLICY;

// CLIENT_GetDecodePolicy's interface input param(get the decoding policy)
typedef struct tagDH_IN_GET_DEC_POLICY 
{
	DWORD				dwSize;
	int					nChannel;			// channel
	int					nWindow;			// window ID, -1 means all
} DH_IN_GET_DEC_POLICY;

// CLIENT_GetDecodePolicy's interface output param(get the decoding policy)
typedef struct tagDH_OUT_GET_DEC_POLICY 
{
	DWORD				dwSize;
	int					nPolicyLevel;		// policy level, a total of 5 file(-2, -1, 0, 1, 2), the greater the vale ,the bigger the fluid but delay
											// -2 real-time good, 2 fluency good, 0 default
} DH_OUT_GET_DEC_POLICY;

// audio output mode
typedef enum
{
	DH_AUDIO_AUTO,							// automatic switch
	DH_AUDIO_DISABLE,						// all output disable 
	DH_AUDIO_FORCE,							// mandatory audio output to the user to specify a window
} DH_AUDIO_OUTPUT_MODE;

// CLIENT_SetSplitAudioOuput's interface input param(set mode of audio output)
typedef struct tagDH_IN_SET_AUDIO_OUTPUT 
{
	DWORD				dwSize;
	int					nChannel;			// channel ID
	DH_AUDIO_OUTPUT_MODE emMode;			// mode
	int					nWindow;			// window no. , effect when emMode = DH_AUDIO_FORCE
} DH_IN_SET_AUDIO_OUTPUT;

// CLIENT_SetSplitAudioOuput's interface output param(set mode of audio output)
typedef struct tagDH_OUT_SET_AUDIO_OUTPUT
{
	DWORD				dwSize;
} DH_OUT_SET_AUDIO_OUTPUT;

// CLIENT_GetSplitAudioOuput's interface input param(get mode of audio output)
typedef struct tagDH_IN_GET_AUDIO_OUTPUT
{
	DWORD				dwSize;
	int					nChannel;			// channel ID
} DH_IN_GET_AUDIO_OUTPUT;

// CLIENT_GetSplitAudioOuput's interface output param(get mode of audio output)
typedef struct tagDH_OUT_GET_AUDIO_OUTPUT
{
	DWORD				dwSize;
	DH_AUDIO_OUTPUT_MODE emMode;			// mode of audio output
	int					nWindow;			// window no., effect when emMode = DH_AUDIO_FORCE
} DH_OUT_GET_AUDIO_OUTPUT;

// CLIENT_GetEncodePlan's interface output param(access to burn a CD to yer coding parameters)
typedef struct tagDH_IN_GET_ENCODE_PLAN
{
	DWORD				dwSize;
	unsigned int        nChannel;          // channel
	unsigned int        nExpectTime;       // expect time,int(min)
	unsigned int        nCapacity;         // capacity,int(M)
}DH_IN_GET_ENCODE_PLAN;
// CLIENT_GetEncodePlan's interface output param(access to burn a CD to yer coding parameters)
typedef struct tagDH_OUT_GET_ENCODE_PLAN
{
	DWORD				dwSize;
	int                 nResolutionType;    // emResolutionTypes counts
	CAPTURE_SIZE        emResolutionTypes[DH_MAX_CAPTURE_SIZE_NUM];// the scope of video resolution
	CAPTURE_SIZE        emResolution;       // the recommended video resolution
	unsigned int        nBitRate;           // bit rate
}DH_OUT_GET_ENCODE_PLAN;

// organization directory logical objects
typedef struct tagDH_ORGANIZATION_NODE_LOGIC_OBJECT 
{
	DWORD				dwSize;
	char				szName[DH_NODE_NAME_LEN];	// name
	char				szType[DH_DEV_TYPE_LEN];	// type
	char				szDevID[DH_DEV_ID_LEN_EX];	// deviceID
	char				szControlID[DH_DEV_ID_LEN_EX]; // control ID, read only
	int					nChannel;					// channel
} DH_ORGANIZATION_NODE_LOGIC_OBJECT;

// organization directory
typedef struct tagDH_ORGANIZATION_NODE_DIRECTORY
{
	DWORD				dwSize;
	char				szName[DH_NODE_NAME_LEN];		// name
	char				szControlID[DH_DEV_ID_LEN_EX];	// control ID
} DH_ORGANIZATION_NODE_DIRECTORY;

// info of organization directory node
typedef struct tagDH_ORGANIZATION_NODE 
{
	DWORD				dwSize;
	int					nNodeType;						// node type, 0-logical objects, 1-list
	DH_ORGANIZATION_NODE_LOGIC_OBJECT	stuLogicObject;	// logical object, effective when nNodeType==0
	DH_ORGANIZATION_NODE_DIRECTORY		stuDirectory;	// list,effective when nNodeType==1		
} DH_ORGANIZATION_NODE;

// add node param
typedef struct tagDH_ORGANIZATION_ADD_NODE_PARAM
{
	DWORD				dwSize;
	int					nPosition;					// position, -1-start, -2-end, 0~n
	DH_ORGANIZATION_NODE stuNode;					// info of node
} DH_ORGANIZATION_ADD_NODE_PARAM;

// CLIENT_OrganizationAddNodes's interface input param(add node)
typedef struct tagDH_IN_ORGANIZATION_ADD_NODES
{
	DWORD				dwSize;
	char*				pszPath;					// path
	DH_ORGANIZATION_ADD_NODE_PARAM* pstuNodes;		// node pointer
	int					nNodeCount;					// count of node
} DH_IN_ORGANIZATION_ADD_NODES;

// result of added node
typedef struct tagDH_ORGANIZATION_ADD_NODE_RESULT
{
	DWORD				dwSize;
	BOOL				bResult;					// result
	char				szPath[MAX_PATH];			// path, return node path when succeed,return add node path when failed
} DH_ORGANIZATION_ADD_NODE_RESULT;

// CLIENT_OrganizationAddNodess interface output param(add node)
typedef struct tagDH_OUT_ORGANIZATION_ADD_NODES
{
	DWORD				dwSize;
	DH_ORGANIZATION_ADD_NODE_RESULT* pstuResults;	// result pointer
	int					nMaxResultCount;			// size of result
	int					nRetResultCount;			// count of result
} DH_OUT_ORGANIZATION_ADD_NODES;

// node path
typedef struct tagDH_ORGANIZATION_NODE_PATH 
{
	DWORD				dwSize;
	char				szPath[MAX_PATH];			// path
} DH_ORGANIZATION_NODE_PATH;

// CLIENT_OrganizationDeleteNodes's interface input param(delete node)
typedef struct tagDH_IN_ORGANIZATION_DELETE_NODES
{
	DWORD				dwSize;
	DH_ORGANIZATION_NODE_PATH*	pstuPath;			// path array
	int					nPathCount;					// count
} DH_IN_ORGANIZATION_DELETE_NODES;

// CLIENT_OrganizationDeleteNodes's interface output param(delete node)
typedef struct tagDH_OUT_ORGANIZATION_DELETE_NODES 
{
	DWORD				dwSize;
} DH_OUT_ORGANIZATION_DELETE_NODES;

// CLIENT_OrganizationGetNodes's interface input param(get node info)
typedef struct tagDH_IN_ORGANIZATION_GET_NODES 
{
	DWORD				dwSize;
	char*				pszPath;					// path
	int					nLevel;						// level, 0-the level, 1-next level
} DH_IN_ORGANIZATION_GET_NODES;

// CLIENT_OrganizationGetNodes's interface output param(get node info)
typedef struct tagDH_OUT_ORGANIZATION_GET_NODES 
{
	DWORD				dwSize;
	DH_ORGANIZATION_NODE*	pstuNodes;				// node array
	int					nMaxNodeCount;				// size of array
	int					nRetNodeCount;				// count of node count
} DH_OUT_ORGANIZATION_GET_NODES;

// CLIENT_OrganizationSetNode's interface input param(get node info)
typedef struct tagDH_IN_ORGANIZATION_SET_NODE 
{
	DWORD				dwSize;
	char*				pszPath;					// path
	DH_ORGANIZATION_NODE stuNode;					// node info
} DH_IN_ORGANIZATION_SET_NODE;

// CLIENT_OrganizationSetNode's interface output param(set node)
typedef struct tagDH_OUT_ORGANIZATION_SET_NODE 
{
	DWORD				dwSize;
} DH_OUT_ORGANIZATION_SET_NODE;

// channel info of video input
typedef struct tagDH_VIDEO_INPUTS
{
	DWORD				dwSize;
	char				szChnName[DH_DEVICE_NAME_LEN];		// channel name
	BOOL				bEnable;							// enable
	char				szControlID[DH_DEV_ID_LEN_EX];		// control ID
	char				szMainStreamUrl[MAX_PATH];			// main stream url 
	char				szExtraStreamUrl[MAX_PATH];			// extra stream url
} DH_VIDEO_INPUTS;

typedef struct tagDH_REMOTE_DEVICE 
{
	DWORD				dwSize;
	BOOL				bEnable;							// enable
	char			    szIp[DH_MAX_IPADDR_LEN];		    // IP
	char			    szUser[DH_USER_NAME_LENGTH];	    // username
	char			    szPwd[DH_USER_PSW_LENGTH];	    	// password
	int			    	nPort;							    // port
	int				    nDefinition;					    // definition. 0-standard definition, 1-high definition
	DH_DEVICE_PROTOCOL  emProtocol;							// protocol type
	char			    szDevName[DH_DEVICE_NAME_LEN];		// device name
	int					nVideoInputChannels;				// count channel of video input
	int					nAudioInputChannels;				// count channel of audio input
	char				szDevClass[DH_DEV_TYPE_LEN];		// device type, such as IPC, DVR, NVR
	char				szDevType[DH_DEV_TYPE_LEN];			// device type, such as IPC-HF3300
	int					nHttpPort;							// Http port
	int					nMaxVideoInputCount;				// max count of video input
	int					nRetVideoInputCount;				// return count
	DH_VIDEO_INPUTS*	pstuVideoInputs;					// max count of audion input
	char				szMachineAddress[DH_MAX_CARD_INFO_LEN];	// machine address
	char				szSerialNo[DH_SERIALNO_LEN];		// serial no.
    int                 nRtspPort;                          // Rtsp Port
} DH_REMOTE_DEVICE;

typedef enum tagNET_LOGIC_CHANNEL_TYPE
{
    LOGIC_CHN_UNKNOWN,              // Unknow
    LOGIC_CHN_LOCAL,                // Local channel 
    LOGIC_CHN_REMOTE,               // Remote access channel 
    LOGIC_CHN_COMPOSE,              // Synthesis of channel, for the judicial equipment contains picture in picture channel and mixing channel
    LOGIC_CHN_MATRIX,               // ����ͨ��������ģ���������־���
} NET_LOGIC_CHN_TYPE;

// available according to the source of information
typedef struct tagDH_MATRIX_CAMERA_INFO
{
	DWORD				dwSize;
	char				szName[DH_DEV_ID_LEN_EX];		// name
	char				szDevID[DH_DEV_ID_LEN_EX];		// device ID
	char				szControlID[DH_DEV_ID_LEN_EX];	// control ID
	int					nChannelID;						// channel ID, DeviceID is unique
	int					nUniqueChannel;					// unique channel
	BOOL				bRemoteDevice;					// support remote device or not
	DH_REMOTE_DEVICE	stuRemoteDevice;				// info of remote device
	NET_STREAM_TYPE     emStreamType;                   // stream type
    NET_LOGIC_CHN_TYPE  emChannelType;                      // Channel Types 
} DH_MATRIX_CAMERA_INFO;

// CLIENT_MatrixGetCameras's interface input param
typedef struct tagDH_IN_MATRIX_GET_CAMERAS 
{
	DWORD				dwSize;
} DH_IN_MATRIX_GET_CAMERAS;

// CLIENT_MatrixGetCameras's interface output param
typedef struct tagDH_OUT_MATRIX_GET_CAMERAS 
{
	DWORD				dwSize;
	DH_MATRIX_CAMERA_INFO* pstuCameras;					// array
	int					nMaxCameraCount;				// size of source array
	int					nRetCameraCount;				// return count
} DH_OUT_MATRIX_GET_CAMERAS;

// CLIENT_MatrixSetCameras's interface input param
typedef struct tagDH_IN_MATRIX_SET_CAMERAS 
{
	DWORD				dwSize;
	DH_MATRIX_CAMERA_INFO* pstuCameras;					// array
	int					nCameraCount;				    // size of source array
} DH_IN_MATRIX_SET_CAMERAS;

// CLIENT_MatrixSetCameras's interface output param
typedef struct tagDH_OUT_MATRIX_SET_CAMERAS 
{
	DWORD				dwSize;
} DH_OUT_MATRIX_SET_CAMERAS;

// monitor wall output
typedef struct tagDH_MONITORWALL_OUTPUT 
{
	DWORD				dwSize;
	char				szDeviceID[DH_DEV_ID_LEN];		// device ID, when itself the value = ""
	int					nChannel;						// channel
	char				szName[DH_DEV_NAME_LEN];		// name
} DH_MONITORWALL_OUTPUT;

// monitor wall block
typedef struct tagDH_MONITORWALL_BLOCK
{
	DWORD				dwSize;
	char				szName[DH_DEV_NAME_LEN];		// block name
	char				szCompositeID[DH_DEV_ID_LEN];	// composite ID
	char				szControlID[DH_DEV_ID_LEN];		// control ID
	int					nSingleOutputWidth;				// single output width
	int					nSingleOutputHeight;			// single output height
	DH_RECT				stuRect;						// rect coordinates
	DH_TSECT			stuPowerSchedule[DH_TSCHE_DAY_NUM][DH_TSCHE_SEC_NUM];	// on  schedule,the first said the weekend or holidays
	DH_MONITORWALL_OUTPUT* pstuOutputs;					// array
	int					nMaxOutputCount;				// size of array
	int					nRetOutputCount;				// return count
} DH_MONITORWALL_BLOCK;

// monitor wall configuration
typedef struct tagDH_MONITORWALL 
{
	DWORD				dwSize;
	char				szName[DH_DEV_NAME_LEN];		// name
	int					nGridLine;						// grid line
	int					nGridColume;					// grid colume
	DH_MONITORWALL_BLOCK* pstuBlocks;					// block array
	int					nMaxBlockCount;					// size of block array
	int					nRetBlockCount;					// return count
} DH_MONITORWALL;

// source of window display info 
typedef struct tagDH_SPLIT_WND_SOURCE 
{
	DWORD				dwSize;
	BOOL				bEnable;						// enable
	char				szDeviceID[DH_DEV_ID_LEN];		// device ID
	char				szControlID[DH_DEV_ID_LEN];		// control ID
	int					nVideoChannel;					// video channel ID
	int					nVideoStream;					// video stream type
	int					nAudioChannel;					// audio channel ID
	int					nAudioStream;					// audio stream type
	int					nUniqueChannel;					// unique channel,read only
	BOOL				bRemoteDevice;					// support remote device
	DH_REMOTE_DEVICE	stuRemoteDevice;				// info of remote device
} DH_SPLIT_WND_SOURCE;

// info of split window
typedef struct tagDH_SPLIT_WINDOW 
{
	DWORD				dwSize;
	BOOL				bEnable;						// enable
	int					nWindowID;						// window ID
	char				szControlID[DH_DEV_ID_LEN];		// control ID
	DH_RECT				stuRect;						// rect of window, effective when free split
	BOOL				bDirectable;					// coordinate whether meet the conditions
	int					nZOrder;						// Z order
	DH_SPLIT_WND_SOURCE stuSource;						// source info
} DH_SPLIT_WINDOW;

// split scene
typedef struct tagDH_SPLIT_SCENE 
{
	DWORD				dwSize;
	char				szCompositeID[DH_DEV_ID_LEN];	// composite ID
	char				szControlID[DH_DEV_ID_LEN];		// control ID
	DH_SPLIT_MODE		emSplitMode;					// split mode
	DH_SPLIT_WINDOW*	pstuWnds;						// info of array
	int					nMaxWndCount;					// size of array
	int					nRetWndCount;					// return count
} DH_SPLIT_SCENE;

// monitor wall scene
typedef struct tagDH_MONITORWALL_SCENE
{
	DWORD				dwSize;
	char				szName[DH_DEV_NAME_LEN];		// name of plan
	char				szControlID[DH_DEV_ID_LEN];		// control ID
	DH_MONITORWALL		stuMonitorWall;					// monitor wall configuration	
	DH_SPLIT_SCENE*		pstuSplitScene;					// array of split scene
	int					nMaxSplitSceneCount;			// size of array
	int					nRetSplitSceneCount;			// return count
} DH_MONITORWALL_SCENE;

// CLIENT_MonitorWallGetScene's interface input param(get monitor wall scene)
typedef struct tagDH_IN_MONITORWALL_GET_SCENE 
{
	DWORD				dwSize;
	int					nMonitorWallID;					// monitor wall ID
} DH_IN_MONITORWALL_GET_SCENE;

// CLIENT_MonitorWallGetScene's interface output param(get monitor wall scene)
typedef struct tagDH_OUT_MONITORWALL_GET_SCENE 
{
	DWORD				dwSize;
	char				szName[DH_DEV_NAME_LEN];		// name of plan, can be null
	DH_MONITORWALL_SCENE stuScene;						// minotor wall scene
} DH_OUT_MONITORWALL_GET_SCENE;

// CLIENT_MonitorWallSetScene's interface input param(set monitor wall scene)
typedef struct tagDH_IN_MONITORWALL_SET_SCENE 
{
	DWORD				dwSize;
	int					nMonitorWallID;					// TV Wall No. 
	DH_MONITORWALL_SCENE stuScene;						// TV Wall Scene
} DH_IN_MONITORWALL_SET_SCENE;

// CLIENT_MonitorWallSetScene's interface output param(set monitor wall scene)
typedef struct tagDH_OUT_MONITORWALL_SET_SCENE 
{
	DWORD				dwSize;
} DH_OUT_MONITORWALL_SET_SCENE;

//CLIENT_QueryNetStat Port, input parameter when the Types of queries is NET_APP_DATA_STAT (statistics for protocol stack) 
typedef struct tagNET_IN_NETAPP_NET_DATA_STAT
{
    DWORD       dwSize;    
    char        szEthName[DH_MAX_ETH_NAME];                // The network card name
}NET_IN_NETAPP_NET_DATA_STAT;

//Receive relevant statistics, same meaning with the ifconfig 
typedef struct tagNET_NETAPP_RECEIVE_STAT
{
    DWORD        dwSize;
    DWORD        dwPackets;
    DWORD        dwBytes;
    DWORD        dwErrors;
    DWORD        dwDroped;
    DWORD        dwOverruns;
    DWORD        dwFrame;
}NET_NETAPP_RECEIVE_STAT;

//Transport statistics,same meaning with the ifconfig 
typedef struct tagNET_NETAPP_TRANSMIT_STAT
{
    DWORD        dwSize;
    DWORD        dwPackets;
    DWORD        dwBytes;
    DWORD        dwErrors;
    DWORD        dwDroped;
    DWORD        dwOverruns;
    DWORD        dwCarrier;
    DWORD        dwCollisions;
    DWORD        dwTxQueue;
}NET_NETAPP_TRANSMIT_STAT;

//CLIENT_QueryNetStat Port, output parameter when the Types of queries is NET_APP_DATA_STAT (statistics for protocol stack) 
typedef struct tagNET_OUT_NETAPP_NET_DATA_STAT
{                    
    DWORD                        dwSize;
    NET_NETAPP_RECEIVE_STAT      stuReceiveStat;        // Receive relevant statistics,same meaning with the ifconfig 
    NET_NETAPP_TRANSMIT_STAT     stuTransmitStat;       // Transport statistics,same meaning with the ifconfig 
    int                          nSpeed;                // Network speed��unit is Mbps
}NET_OUT_NETAPP_NET_DATA_STAT;

//CLIENT_QueryNetStatPort, input parameter when the Types of queries is NET_APP_DATA_STAT(access to physical link state)
typedef struct tagNET_IN_NETAPP_LINK_STATUS
{
    DWORD       dwSize;
    char        szEthName[DH_MAX_ETH_NAME];             // Network Card Name 
}NET_IN_NETAPP_LINK_STATUS;

//CLIENT_QueryNetStatPort, output parameter when the Types of queries is NET_APP_DATA_STAT(access to physical link state)
typedef struct tagNET_OUT_NETAPP_LINK_STATUS
{
    DWORD       dwSize;
    BOOL        bWorking;               // Whether working  
    BOOL        bIPConflict;            // Whether IP conflict 
}NET_OUT_NETAPP_LINK_STATUS;

// type of input signal
#define DH_VIDEO_SIGNAL_CVBS	0x0001
#define DH_VIDEO_SIGNAL_SDI		0x0002
#define DH_VIDEO_SIGNAL_VGA		0x0004 
#define DH_VIDEO_SIGNAL_DVI		0x0008
#define DH_VIDEO_SIGNAL_HDMI	0x0010 
#define DH_VIDEO_SIGNAL_YPBPR	0x0020 
#define DH_VIDEO_SIGNAL_SFP		0x0040 
#define DH_VIDEO_SIGNAL_HDCVI       0x0080 
#define DH_VIDEO_SIGNAL_DUAL_LINK   0x0100 

// caps of video inpt
typedef struct tagDH_VIDEO_IN_CAPS 
{
	DWORD					dwSize;
	DWORD					dwSignalType;				// type of signal in
	BOOL					bAutoFocusPeak;				// support auto focus peak or not
	BOOL					bElectricFocus;				// support electric focus or not
	BOOL					bSyncFocus;					// support sync focus
    BOOL             bSetColor;                     // �Ƿ�֧����Ƶ��ɫ����
} DH_VIDEO_IN_CAPS;

// CLIENT_GetVideoInCaps's interface input param
typedef struct tagDH_IN_GET_VIDEO_IN_CAPS 
{
	DWORD			dwSize;
	int				nChannel;							// channel ID of video in
} DH_IN_GET_VIDEO_IN_CAPS;

// CLIENT_GetVideoInCaps's interface output param
typedef struct tagDH_OUT_GET_VIDEO_IN_CAPS 
{
	DWORD			dwSize;
	DH_VIDEO_IN_CAPS stuCaps;							// ability
} DH_OUT_GET_VIDEO_IN_CAPS;

// mode of video out
typedef struct tagDH_VIDEO_OUT_MODE 
{
	DWORD			dwSize;
	int				nWidth;								// horizontal resolution
	int				nHeight;							// vertical resolution
	int				nBPP;								// color depth
	int				nFormat;							// format of signel out, 0-Auto, 1-TV, 2-VGA, 3-DVI
	int				nRefreshRate;						// refersh rate
	int				nPhysicalPort;						// physical port, VGA, TV, DVI independent number
    int              nScanFormat;                       // Scanning mode, 0 - line by line, 1 - interlaced 
} DH_VIDEO_OUT_MODE;

// CLIENT_EnumVideoOutModes's interface input param
typedef struct tagDH_IN_ENUM_VIDEO_OUT_MODES 
{
	DWORD			dwSize;
	int				nChannel;
} DH_IN_ENUM_VIDEO_OUT_MODES;

// CLIENT_EnumVideoOutModes's interface output param
typedef struct tagDH_OUT_ENUM_VIDEO_OUT_MODES 
{
	DWORD			dwSize;
	DH_VIDEO_OUT_MODE* pstuModes;						// mode array
	int				nMaxModeCount;						// count of mode
	int				nRetModeCount;						// return count
} DH_OUT_ENUM_VIDEO_OUT_MODES;

// minotor wall attribute type
typedef enum tagDH_OUTPUT_ATTRIBUTE_TYPE
{
	DH_OUTPUT_ATTRIBUTE_VIDEO, 
	DH_OUTPUT_ATTRIBUTE_YPBPR, 
	DH_OUTPUT_ATTRIBUTE_VGA, 
	DH_OUTPUT_ATTRIBUTE_DVI, 
	DH_OUTPUT_ATTRIBUTE_MENU, 
} DH_OUTPUT_ATTRIBUTE_TYPE;

// caps of monitor wall adjust (DH_OUTPUT_ATTRIBUTE_CAPS) Video ability
#define	DH_ATTR_VIDEO_BRIGHTNESS	0X00000001
#define	DH_ATTR_VIDEO_CONTRAST		0X00000002
#define	DH_ATTR_VIDEO_SATURATION	0X00000004
#define	DH_ATTR_VIDEO_SHAPENESS		0X00000008
#define	DH_ATTR_VIDEO_DENOISE		0X00000010

// caps of monitor wall adjust (DH_OUTPUT_ATTRIBUTE_CAPS) YPbPr ability
#define	DH_ATTR_YPBPR_BRIGHTNESS	0X00000001
#define	DH_ATTR_YPBPR_CONTRAST		0X00000002
#define	DH_ATTR_YPBPR_SATURATION	0X00000004
#define	DH_ATTR_YPBPR_SHAPENESS		0X00000008
#define	DH_ATTR_YPBPR_DENOISE		0X00000010

// caps of monitor wall adjust (DH_OUTPUT_ATTRIBUTE_CAPS) VGA ability
#define	DH_ATTR_VGA_BRIGHTNESS		0X00000001
#define	DH_ATTR_VGA_CONTRAST		0X00000002
#define	DH_ATTR_VGA_HORPOSITION		0X00000004
#define	DH_ATTR_VGA_VERPOSITION		0X00000008
#define	DH_ATTR_VGA_CLOCK			0X00000010

// caps of monitor wall adjust (DH_OUTPUT_ATTRIBUTE_CAPS) DVI ability
#define	DH_ATTR_DVI_BRIGHTNESS		0X00000001
#define	DH_ATTR_DVI_CONTRAST		0X00000002
#define	DH_ATTR_DVI_HORPOSITION		0X00000004
#define	DH_ATTR_DVI_VERPOSITION		0X00000008
#define	DH_ATTR_DVI_CLOCK			0X00000010

// caps of monitor wall adjust (DH_OUTPUT_ATTRIBUTE_CAPS) Menu ability
#define	DH_ATTR_MENU_MENU			0X00000001
#define	DH_ATTR_MENU_UP				0X00000002
#define	DH_ATTR_MENU_DOWN			0X00000004
#define	DH_ATTR_MENU_LEFT			0X00000008
#define	DH_ATTR_MENU_RIGHT			0X00000010
#define	DH_ATTR_MENU_FACTORYMENU	0X00000020

// caps of monitor wall adjust 
typedef struct tagDH_OUTPUT_ATTRIBUTE_CAPS
{
	DWORD			dwSize;
	BOOL			abVideo;			// Video enable
	DWORD			dwVideo;			// Video ability, by bite, specific to see DH_ATTR_VIDEO_BRIGHTNESS
	BOOL			abYPbPr;			// YPbPr enable
	DWORD			dwYPbPr;			// YPbPr ability, by bite, specific to see DH_ATTR_VGA_BRIGHTNESS
	BOOL			abVGA;				// VGA enable
	DWORD			dwVGA;				// VGA ability, by bite, specific to see DH_ATTR_VGA_BRIGHTNESS
	BOOL			abDVI;				// DVI enable
	DWORD			dwDVI;				// DVI ability, by bite, specific to see DH_ATTR_DVI_BRIGHTNESS
	BOOL			abMenu;				// Menu enable
	DWORD			dwMemu;				// Menu ability, by bite, specific to see DH_ATTR_MENU_MENU
} DH_OUTPUT_ATTRIBUTE_CAPS;

// CLIENT_MonitorWallGetAttributeCaps's interface input param
typedef struct tagDH_IN_MONITORWALL_GET_ARRT_CAPS
{
	DWORD			dwSize;
	int				nMonitorWallID;			// monitor wall ID
	const char*		pszCompositeID;			// composite ID
	int				nOutputID;				// output ID
} DH_IN_MONITORWALL_GET_ARRT_CAPS;

// CLIENT_MonitorWallGetAttributeCaps's interface output param
typedef struct tagDH_OUT_MONITORWALL_GET_ARRT_CAPS
{
	DWORD			dwSize;
	DH_OUTPUT_ATTRIBUTE_CAPS stuCaps;		// caps of attribute
} DH_OUT_MONITORWALL_GET_ARRT_CAPS;


// CLIENT_MonitorWallAutoAdjust's interface input param
typedef struct tagDH_IN_MONITORWALL_AUTO_ADJUST
{
	DWORD			dwSize;
	int				nMonitorWallID;			// monitor wall ID
	const char*		pszCompositeID;			// composite ID
	int*			pOutputs;				// array pointer
	int				nOutputCount;			// count of pOutputs
} DH_IN_MONITORWALL_AUTO_ADJUST;

// CLIENT_MonitorWallAutoAdjust's interface output param
typedef struct tagDH_OUT_MONITORWALL_AUTO_ADJUST
{
	DWORD			dwSize;
} DH_OUT_MONITORWALL_AUTO_ADJUST;

// display unit attribute key/value
typedef struct tagDH_ATTR_PAIR
{
	DWORD	dwKey;					// attribute key
									// dwKey = DH_OUTPUT_ATTRIBUTE_VIDEO , uses DH_ATTR_VIDEO_BRIGHTNESS
									// dwKey = DH_OUTPUT_ATTRIBUTE_YPBPR, usesDH_ATTR_YPBPR_BRIGHTNESS
									// so on
	int		nValue;					// value of attribute, 0-reduce 1-increase, 2-no change
} DH_ATTR_PAIR;

// CLIENT_MonitorWallSetAttribute's interface input param
typedef struct tagDH_IN_MONITORWALL_SET_ATTR
{
	DWORD					dwSize;
	int						nMonitorWallID;					// monitor wall ID
	const char*				pszCompositeID;					// composite ID
	int						nOutputID;						// output ID
	DH_OUTPUT_ATTRIBUTE_TYPE emAttrType;					// attribute, the differ type, the stuAttrs differ also
	DH_ATTR_PAIR			stuAttrs[DH_MAX_ATTR_NUM];		// value of attribute
} DH_IN_MONITORWALL_SET_ATTR;

// CLIENT_MonitorWallSetAttribute's interface output param
typedef struct tagDH_OUT_MONITORWALL_SET_ATTR
{
	DWORD			dwSize;
} DH_OUT_MONITORWALL_SET_ATTR;

// CLIENT_MonitorWallSetBackLight's interface input param
typedef struct tagDH_IN_MONITORWALL_SET_BACK_LIGHT
{
	DWORD					dwSize;
	int						nMonitorWallID;					// monitor wall ID
	const char*				pszCompositeID;					// omposite ID
	int						nOutputID;						// output ID
	int						nMode;							// backlight model, 0-circulation patterns,1-not circulation
} DH_IN_MONITORWALL_SET_BACK_LIGHT;

// CLIENT_MonitorWallSetBackLight's interface output param
typedef struct tagDH_OUT_MONITORWALL_SET_BACK_LIGHT
{
	DWORD			dwSize;
} DH_OUT_MONITORWALL_SET_BACK_LIGHT;

// CLIENT_MonitorWallGetPowerSchedule Interface Input Parameters 
typedef struct tagNET_IN_WM_GET_POWER_SCHEDULE
{
    DWORD               dwSize;
    int                 nMonitorWallID;                     // TV Wall Serial Number 
    const char*         pszCompositeID;                     // Splicing Screen ID 
} NET_IN_MW_GET_POWER_SCHEDULE;

// CLIENT_MonitorWallGetPowerSchedule Interface Output Parameters 
typedef struct tagNET_OUT_MW_GET_POWER_SCHEDULE
{
    DWORD               dwSize;
    DH_TSECT            stuPowerSchedule[DH_TSCHE_DAY_NUM][DH_TSCHE_SEC_NUM];    // starting up schedule, the first dimension means Saturday and Sunday and other holidays 
} NET_OUT_MW_GET_POWER_SCHEDULE;

// CLIENT_MonitorWallSetPowerSchedule Interface Input Parameters
typedef struct tagNET_IN_MW_SET_POWER_SCHEDULE
{
    DWORD               dwSize;
    int                 nMonitorWallID;                     // TV wall serial number 
    const char*         pszCompositeID;                     // Splicing Screen ID, NULL means all the screen 
    DH_TSECT            stuPowerSchedule[DH_TSCHE_DAY_NUM][DH_TSCHE_SEC_NUM];    // starting up schedule, the first dimension means Saturday and Sunday and other holidays
} NET_IN_MW_SET_POWER_SCHEDULE;

// CLIENT_MonitorWallSetPowerSchedule Interface Output Parameters 
typedef struct tagNET_OUT_MW_SET_POWER_SCHEDULE
{
    DWORD               dwSize;
} NET_OUT_MW_SET_POWER_SCHEDULE;

// CLIENT_MonitorWallGetScrnCtrlParam Interface Intput Parameters 
typedef struct tagNET_IN_MW_GET_SCRN_CTRL_PARAM
{
    DWORD               dwSize;
    int                 nMonitorWallID;                     // TV wall Serial Number
    const char*         pszCompositeID;                     // Splicing Screen ID 
} NET_IN_MW_GET_SCRN_CTRL_PARAM;

#define DH_MAX_SCREEN_PORT_NUM        16                    // The biggest screen input port number 
#define DH_MAX_COMPSITE_SCREEN_NUM    256                   // The biggest splicing screen number 

// Screen Input Port Information
typedef struct tagNET_SCREEN_PORT_INFO
{
    DWORD               dwSize;
    char                szType[DH_COMMON_STRING_16];        // Port type, "DVI", "VGA", "HDMI"Etc., Allow the same type of multiple ports 
    char                szAddress[DH_COMMON_STRING_16];     // Port address 
    char                szDeviceID[DH_DEV_ID_LEN_EX];       // The video output device of a binding 
    int                 nOutputChannel;                     // Video output channel number 
} NET_SCREEN_PORT_INFO;


typedef struct tagNET_SCREEN_PORT_GROUP 
{
    DWORD                dwSize;
    int                  nPortNum;                          // Port Number
    NET_SCREEN_PORT_INFO stuPorts[DH_MAX_SCREEN_PORT_NUM];  // Port Information
} NET_SCREEN_PORT_GROUP;

// TV wall screen control parameters 
typedef struct tagNET_WM_SCRN_CTRL_PARAM
{
    DWORD                   dwSize;
    int                     nCommPort;                                  // Serial Number
    DH_COMM_PROP            stuCommProp;                                // Port Attributes
    char                    szProtocol[DH_COMMON_STRING_32];            // Port Protocol 
    int                     nResolutionNum;                             // Resolution Number
    CAPTURE_SIZE            emResolutions[DH_MAX_CAPTURE_SIZE_NUM];     // Video Resolution List 
    int                     nScreenCodeNum;                             // Screen Code Number 
    char                    szScreenCode[DH_MAX_COMPSITE_SCREEN_NUM][DH_COMMON_STRING_16];    // Screen code, using for serial command addressing,this encoding configuration by the user, can be the same 
    int                     nScreenPortsNum;                            // Number of screen input port information
    NET_SCREEN_PORT_GROUP   stuScreenPorts[DH_MAX_COMPSITE_SCREEN_NUM]; // Screen input port information     
} NET_WM_SCRN_CTRL_PARAM;

// CLIENT_MonitorWallGetScrnCtrlParam Interface Input Parameters 
typedef struct tagNET_OUT_MW_GET_SCRN_CTRL_PARAM
{
    DWORD                   dwSize;
    NET_WM_SCRN_CTRL_PARAM  stuScrnCtrlParam;           // Screen Control Parameters
} NET_OUT_MW_GET_SCRN_CTRL_PARAM;

// CLIENT_MonitorWallSetScrnCtrlParam Interface Input Parameters
typedef struct tagNET_IN_MW_SET_SCRN_CTRL_PARAM
{
    DWORD                   dwSize;
    int                     nMonitorWallID;             // TV Wall Serial Number 
    const char*             pszCompositeID;             // Splicing Screen ID 
    NET_WM_SCRN_CTRL_PARAM  stuScrnCtrlParam;           // Screen Control Parameters
} NET_IN_MW_SET_SCRN_CTRL_PARAM;

// CLIENT_MonitorWallSetScrnCtrlParam    Interface Output Parameters 
typedef struct tagNET_OUT_MW_SET_SCRN_CTRL_PARAM
{
    DWORD           dwSize;
} NET_OUT_MW_SET_SCRN_CTRL_PARAM;

// CLIENT_MonitorWallGetBackgroudColor Interface Input Parameters 
typedef struct tagNET_IN_MW_GET_BACKGROUDND_COLOR
{
    DWORD           dwSize;
    int             nMonitorWallID;             // TV Wall Serial Number
} NET_IN_MW_GET_BACKGROUDND_COLOR;

// CLIENT_MonitorWallGetBackgroudColor    Interface Output Parameters 
typedef struct tagNET_OUT_MW_GET_BACKGROUDND_COLOR
{
    DWORD           dwSize;
    DH_COLOR_RGBA   stuScreenColor;             // Screen Background Color 
    DH_COLOR_RGBA   stuWindowColor;             // Window Background Color 
} NET_OUT_MW_GET_BACKGROUDND_COLOR;

// CLIENT_MonitorWallSetBackgroudColor Interface Input Parameters 
typedef struct tagNET_IN_MW_SET_BACKGROUD_COLOR 
{
    DWORD           dwSize;
    int             nMonitorWallID;             // TV Wall Serial Number
    DH_COLOR_RGBA   stuScreenColor;             // Screen Background Color 
    DH_COLOR_RGBA   stuWindowColor;             // Window Background Color 
} NET_IN_MW_SET_BACKGROUD_COLOR;

// CLIENT_MonitorWallSetBackgroudColor Interface Output Parameters
typedef struct tagNET_OUT_MW_SET_BACKGROUD_COLOR 
{
    DWORD           dwSize;
} NET_OUT_MW_SET_BACKGROUD_COLOR;

/************************************************************************/
/*  U disk caught
/************************************************************************/

// CLIENT_StartSniffer's interface input param
typedef struct tagDH_IN_START_SNIFFER 
{
	DWORD		dwSize;
	const char*	pszNetInterface;				// name of network card
	const char*	pszPath;						// path of caught file, it is means the default path when pszPath = NULL
	int			nSaveType;						// type of file, 0-Wireshark/Tcpdump
    const char*     pszFilter;                  // Filter Conditions ,Such as "host 172.9.88.200 and port 8080 and tcp"
} DH_IN_START_SNIFFER;

// CLIENT_StartSniffer's interface output param
typedef struct tagDH_OUT_START_SNIFFER 
{
	DWORD		dwSize;
} DH_OUT_START_SNIFFER;

// grasp the package information
typedef struct tagDH_SNIFFER_INFO
{
	DWORD		dwSize;
	DWORD		nSnifferID;					// Sniffer ID
} DH_SNIFFER_INFO;

// CLIENT_GetSnifferInfo's interface input param
typedef struct tagDH_IN_GET_SNIFFER_INFO
{
	DWORD		dwSize;
	const char*	pszNetInterface;
} DH_IN_GET_SNIFFER_INFO;

// CLIENT_GetSnifferInfo's interface output param
typedef struct tagDH_OUT_GET_SNIFFER_INFO
{
	DWORD		dwSize;
	DH_SNIFFER_INFO	* pstuSniffers;			// array of Sniffer
	int			nMaxSnifferCount;			// size of array
	int			nRetSnifferCount;			// return count
} DH_OUT_GET_SNIFFER_INFO;

/************************************************************************/
/*  manage file
/************************************************************************/

// format the partition information 
typedef struct tagDH_FORMAT_PATITION 
{
	DWORD			dwSize;
	const char*		pszStorageName;							// storage name
	const char*		pszPatitionName;						// partition name
} DH_FORMAT_PATITION;

// CLIENT_CreateRemoteFile's interface input param
typedef struct tagDH_IN_CREATE_REMOTE_FILE
{
	DWORD			dwSize;
	const char*		pszPath;								// path of file
	BOOL			bDirectory;								// is directory or not
} DH_IN_CREATE_REMOTE_FILE;

// CLIENT_CreateRemoteFile's interface output param
typedef struct tagDH_OUT_CREATE_REMOTE_FILE 
{
	DWORD			dwSize;
} DH_OUT_CREATE_REMOTE_FILE;

// CLIENT_RemoveRemoteFiles's interface input param
typedef struct tagDH_IN_REMOVE_REMOTE_FILES
{
	DWORD			dwSize;
	const char**	pszPath;								// pointer of file path
	int				nFileCount;								// count of file
} DH_IN_REMOVE_REMOTE_FILES;

// CLIENT_RemoveRemoteFiles's interface output param
typedef struct tagDH_OUT_REMOVE_REMOTE_FILES 
{
	DWORD		dwSize;
} DH_OUT_REMOVE_REMOTE_FILES;

// CLIENT_RenameRemoteFile's interface input param
typedef struct tagDH_IN_RENAME_REMOTE_FILE
{
	DWORD			dwSize;
	const char*		pszOldPath;								// old path
	const char*		pszNewPath;								// new path
} DH_IN_RENAME_REMOTE_FILE;

// CLIENT_RenameRemoteFile's interface output param
typedef struct tagDH_OUT_RENAME_REMOTE_FILE 
{
	DWORD			dwSize;
} DH_OUT_RENAME_REMOTE_FILE;

// file/catalog info
typedef struct tagDH_REMOTE_FILE_INFO 
{
	DWORD			dwSize;
	BOOL			bDirectory;								// is directory or not
	char			szPath[MAX_PATH];						// path
	NET_TIME		stuCreateTime;							// create time
	NET_TIME		stuModifyTime;							// modify time
	INT64			nFileSize;								// size of file
	char			szFileType[DH_FILE_TYPE_LEN];			// type of file
} DH_REMOTE_FILE_INFO;

// CLIENT_ListRemoteFile's interface input param
typedef struct tagDH_IN_LIST_REMOTE_FILE
{
	DWORD			dwSize;
	const char*		pszPath;								// path
	BOOL			bFileNameOnly;							// only for file name
} DH_IN_LIST_REMOTE_FILE;

// CLIENT_ListRemoteFile's interface output param
typedef struct tagDH_OUT_LIST_REMOTE_FILE 
{
	DWORD			dwSize;
	DH_REMOTE_FILE_INFO*	pstuFiles;						// array of file 
	int						nMaxFileCount;					// size of array
	int						nRetFileCount;					// return count
} DH_OUT_LIST_REMOTE_FILE;

// manual pop-up storage device
typedef struct tagDH_EJECT_STORAGE_DEVICE
{
	DWORD				dwSize;	
	const char*			pszStorageName;						// storage name
} DH_EJECT_STORAGE_DEVICE;

//  manual load storage device
typedef struct tagDH_LOAD_STORAGE_DEVICE 
{
	DWORD				dwSize;
	const char*			pszStorageName;						// storage name
} DH_LOAD_STORAGE_DEVICE;

// CLIENT_UploadRemoteFile's interface input param(upload file to the device)
typedef struct tagDH_IN_UPLOAD_REMOTE_FILE
{
	DWORD					dwSize;
	const char*				pszFileSrc;			// path of source file
	const char*				pszFileDst;			// path of goal file
} DH_IN_UPLOAD_REMOTE_FILE;

// CLIENT_UploadRemoteFile's interface output param(upload file to the device)
typedef struct tagDH_OUT_UPLOAD_REMOTE_FILE
{
	DWORD					dwSize;
} DH_OUT_UPLOAD_REMOTE_FILE;

// CLIENT_DownloadRemoteFile    Interface Input Parameters (the file download)
typedef struct tagDH_IN_DOWNLOAD_REMOTE_FILE
{
    DWORD               dwSize;
    const char*         pszFileName;                    // File Name Needs to Download 
    const char*         pszFileDst;                     // File Path 
} DH_IN_DOWNLOAD_REMOTE_FILE;

// CLIENT_DownloadRemoteFile Interface Output Parameters (the file download) 
typedef struct tagDH_OUT_DOWNLOAD_REMOTE_FILE
{
    DWORD               dwSize;
} DH_OUT_DOWNLOAD_REMOTE_FILE;

/************************************************************************/
/* cascade device
/************************************************************************/
// conditon of cascade device search
typedef struct tagDH_IN_SEARCH_CONDITON
{
	DWORD					dwSize;
	const char*				pMachineName;					// device name or no.
	const char*				pMachineAddr;					// machine address (XX.XX.XX)
	const char*				pSerialNo;						// serial no.
	const char*				pChannelName;					// channel name of video in
} DH_MATRIX_SEARCH_CONDITON;

// CLIENT_MatrixSearch's interface input param(search cascade device)
typedef struct tagDH_IN_MATIRX_SEARCH
{
	DWORD					dwSize;
	const char*				pSerialNo;						// servial no.,"Local""Root",other devices with a serial number
	DH_MATRIX_SEARCH_CONDITON stuCondition;					// condition of search, can be for a single or combination
} DH_IN_MATRIX_SEARCH;

// CLIENT_MatrixSearch's interface output param(search cascade device)
typedef struct tagDH_OUT_MATRIX_SEARCH
{
	DWORD					dwSize;
	DH_REMOTE_DEVICE*		pstuRemoteDevices;				// devices list
	int						nMaxDeviceCount;				// max count of device
	int						nRetDeviceCount;				// return count
} DH_OUT_MATRIX_SEARCH;

// CLIENT_GetMatrixTree's interface input param
typedef struct tagDH_IN_GET_MATRIX_TREE
{
	DWORD					dwSize;
	const char*				pSerialNo;						// servial no.,"Local""Root",other devices with a serial number
	int						nLevel;							// get device information,0-all, 1-Local, 2-Local+device under
															// 3-Local+device under+ the next next devuce
} DH_IN_GET_MATRIX_TREE;

// cascade device info
typedef struct tagDH_CASCADE_DEVICE_INFO
{
	DWORD					dwSize;
	int						nLevel;							// level
	char					szPath[MAX_PATH];				// set path,format:name1.name2.name3...
	DH_REMOTE_DEVICE		stuDeviceInfo;					// device info
} DH_CASCADE_DEVICE_INFO;

// CLIENT_GetMatrixTree's interface output param
typedef struct tagDH_OUT_GET_MATRIX_TREE
{
	DWORD					dwSize;
	DH_CASCADE_DEVICE_INFO*	pstuRemoteDevices;				// info of cascade device
	int						nMaxDeviceCount;				// max number of cascade device
	int						nRetDeviceCount;				// return count
} DH_OUT_GET_MATRIX_TREE;

// CLIENT_GetSuperiorMatrixList's interface input param
typedef struct tagDH_IN_GET_SUPERIOR_MATRIX_LIST
{
	DWORD					dwSize;
} DH_IN_GET_SUPERIOR_MATRIX_LIST;

// CLIENT_GetSuperiorMatrixList's interface output param
typedef struct tagDH_OUT_GET_SUPERIOR_MATRIX_LIST
{
	DWORD					dwSize;
	DH_REMOTE_DEVICE*		pstuRemoteDevices;				// device list
	int						nMaxDeviceCount;				// max count of device
	int						nRetDeviceCount;				// return count
} DH_OUT_GET_SUPERIOR_MATRIX_LIST;

////////////////////////////////////record backup to restore//////////////////////////////////////

// task of record backup to restore
typedef struct tagDH_RECORD_BACKUP_RESTORE_TASK
{
    DWORD				dwSize;
    unsigned int		nTaskID;                        // task ID
    char				szDeviceID[DH_DEV_ID_LEN_EX];   // device ID
    int			        nChannelID;                     // channek ID
    NET_TIME			stuStartTime;                   // start time
    NET_TIME			stuEndTime;                     // end time
    int			        nState;                         // state of backup, 0-wait, 1-working, 2-finish, 3-failed
} DH_REC_BAK_RST_TASK;

// CLIENT_AddRecordBackupRestoreTask's interface input param
typedef struct tagDH_IN_ADD_RECORD_BACKUP_RESTORE_TASK
{
    DWORD				dwSize;
    const char*			pszDeviceID;					// device ID
    int*		        pnChannels;						// array of channel
	int					nChannelCount;					// size of array
    NET_TIME			stuStartTime;					// start time
    NET_TIME			stuEndTime;						// end time
} DH_IN_ADD_REC_BAK_RST_TASK;

// CLIENT_RemoveRecordBackupRestoreTask's interface input param
typedef struct tagDH_IN_REMOVE_RECORD_BACKUP_RESTORE_TASK
{
    DWORD				dwSize;
    unsigned int*		pnTaskIDs;						// array of task ID
	int					nTaskCount;						// count of task
} DH_IN_REMOVE_REC_BAK_RST_TASK;

// CLIENT_QueryRecordBackupRestoreTask's interface input param
typedef struct tagDH_IN_QUERY_RECORD_BACKUP_RESTORE_TASK
{
    DWORD		        dwSize;
} DH_IN_QUERY_REC_BAK_RST_TASK;

// CLIENT_QueryRecordBackupRestoreTask's interface output param
typedef struct tagDH_OUT_QUERY_RECORD_BACKUP_RESTORE_TASK
{
    DWORD				 dwSize;
    DH_REC_BAK_RST_TASK* pTasks;						// array of task
    int					 nMaxCount;						// size of array
    int					 nReturnCount;					// return count
} DH_OUT_QUERY_REC_BAK_RST_TASK;

typedef struct tagDH_LOGIC_DEVICE_ADD_CAMERA_PARAM
{
	DWORD			dwSize;
	const char*		pszDeviceID;			// device ID
	int				nChannel;				// channel
} DH_LOGIC_DEVICE_ADD_CAMERA_PARAM;

typedef struct tagDH_LOGIC_DEVICE_ADD_CAMERA_RESULT
{
	DWORD			dwSize;
	char			szDeviceID[DH_DEV_ID_LEN];	// device ID
	int				nChannel;					// channel ID
	int				nUniqueChannel;				// uniform number
	int				nFailedCode;				// failure code, 0-succeed 1-Unique 2-have added
} DH_LOGIC_DEVICE_ADD_CAMERA_RESULT;

// CLIENT_AddLogicDeviceCamera's interface input param
typedef struct tagDH_IN_ADD_LOGIC_DEVICE_CAMERA
{
	DWORD			dwSize;
	DH_LOGIC_DEVICE_ADD_CAMERA_PARAM*	pCameras;	// array of source video 
	int				nCameraCount;					// count of sourcevideo
} DH_IN_ADD_LOGIC_DEVICE_CAMERA;

// CLIENT_AddLogicDeviceCamera's interface output param
typedef struct tagDH_OUT_ADD_LOGIC_DEVICE_CAMERA 
{
	DWORD			dwSize;
	DH_LOGIC_DEVICE_ADD_CAMERA_RESULT* pResults;	// result
	int				nMaxResultCount;				// size of array
	int				nRetResultCount;				// return count
} DH_OUT_ADD_LOGIC_DEVICE_CAMERA;

/************************************************************************/
/*                         Database Records                               */
/************************************************************************/
typedef struct tagNET_AUTHORITY_TYPE
{
  DWORD                       dwSize; 
  EM_NET_AUTHORITY_TYPE       emAuthorityType;                          //Permission Types 
  BOOL                        bAuthorityEnable;                         //Permission Enabled
}NET_AUTHORITY_TYPE;

// Information of recorded in transportation black and white list 
typedef struct tagNET_TRAFFIC_LIST_RECORD
{
  DWORD                      dwSize; 
  int                        nRecordNo;                                 // Queried Record Number 
  char                       szMasterOfCar[DH_MAX_NAME_LEN];            // Car Owner's Name
  char                       szPlateNumber[DH_MAX_PLATE_NUMBER_LEN];    // License Plate Number 
  EM_NET_PLATE_TYPE          emPlateType;                               // License Plate Type 
  EM_NET_PLATE_COLOR_TYPE    emPlateColor;                              // License Plate Color   
  EM_NET_VEHICLE_TYPE        emVehicleType;                             // Vehicle Type  
  EM_NET_VEHICLE_COLOR_TYPE  emVehicleColor;                            // Car Body Color
  NET_TIME                   stBeginTime;                               // Start Time 
  NET_TIME                   stCancelTime;                              // Undo Time
  int                        nAuthrityNum;                              // Permission Number
  NET_AUTHORITY_TYPE         stAuthrityTypes[DH_MAX_AUTHORITY_LIST_NUM];// Permissions List, White List Only 
  EM_NET_TRAFFIC_CAR_CONTROL_TYPE emControlType;                        // Monitor Type, Black List Only 
}NET_TRAFFIC_LIST_RECORD;

// Query Conditions Of Traffic Black And White List Account Records 
typedef struct
{
    DWORD                    dwSize;
    char                     szPlateNumber[DH_MAX_PLATE_NUMBER_LEN];      // License Plate Number
    char                     szPlateNumberVague[DH_MAX_PLATE_NUMBER_LEN]; // License Plate Number Fuzzy Query 
    int                      nQueryResultBegin;                           // Offset in the query results of first results returned   
    BOOL                     bRapidQuery;                                 // Whether support the quick query, TRUE: for quick, quick query time don't wait for all add, delete, change operation is completed. The default is non-quick query 
}FIND_RECORD_TRAFFICREDLIST_CONDITION;

//Record Case Record Query Conditions 
typedef struct
{
    DWORD                    dwSize;
    NET_TIME                 stuStartTime;                      // Start Time 
    NET_TIME                 stuEndTime;                        // End Time
}FIND_RECORD_BURN_CASE_CONDITION;


// Entrance Card Record Query Conditions 
typedef struct tagFIND_RECORD_ACCESSCTLCARD_CONDITION
{
    DWORD                    dwSize;
    char                     szCardNo[DH_MAX_CARDNO_LEN];      // Card Number 
    char                     szUserID[DH_MAX_USERID_LEN];      // User ID 
    BOOL                     bIsValid;                         // Whether effective, TRUE: effective, FALSE: invalid 
}FIND_RECORD_ACCESSCTLCARD_CONDITION;

// Access password record query conditions 
typedef struct tagFIND_RECORD_ACCESSCTLPWD_CONDITION
{
    DWORD                     dwSize;
    char                      szUserID[DH_MAX_USERID_LEN];      // User ID
}FIND_RECORD_ACCESSCTLPWD_CONDITION;

// Entrance guard access records query conditions 
typedef struct tagFIND_RECORD_ACCESSCTLCARDREC_CONDITION
{
    DWORD                     dwSize;
    char                      szCardNo[DH_MAX_CARDNO_LEN];      // User ID
    NET_TIME                  stStartTime;                      // Start Time 
    NET_TIME                  stEndTime;                        // End Time
}FIND_RECORD_ACCESSCTLCARDREC_CONDITION;

// Holiday Recordset Query Conditions 
typedef struct tagFIND_RECORD_ACCESSCTLHOLIDAY_CONDITION
{
    DWORD                     dwSize;
}FIND_RECORD_ACCESSCTLHOLIDAY_CONDITION;

// CLIENT_FindRecord    Interface Input Parameters 
typedef struct _NET_IN_FIND_RECORD_PARAM
{
    DWORD                     dwSize;          // The Structure Size 
    EM_NET_RECORD_TYPE        emType;          // The record type to query
    void*                     pQueryCondition; // Query types corresponding to the query conditions 
}NET_IN_FIND_RECORD_PARAM;

// CLIENT_FindRecord  Interface Output Parameters 
typedef struct _NET_OUT_FIND_RECORD_PARAM
{
    DWORD                     dwSize;          // Structure Size
    LLONG                     lFindeHandle;    // Query Log Handle,Uniquely identifies a certain query
}NET_OUT_FIND_RECORD_PARAM;

// CLIENT_FindNextRecord  Interface Input Parameters 
typedef struct _NET_IN_FIND_NEXT_RECORD_PARAM
{
    DWORD                     dwSize;          // Structure Size 
    LLONG                     lFindeHandle;    // Query Log Handle
    int                       nFileCount;      // The current number of records  need query 
}NET_IN_FIND_NEXT_RECORD_PARAM;

// CLIENT_FindNextRecord  Interface Output Parameters 
typedef struct _NET_OUT_FIND_NEXT_RECORD_PARAM
{
    DWORD                     dwSize;          // Structure Size 
    void*                     pRecordList;     // Record List, the user allocates memory 
    int                       nMaxRecordNum;   // List Record Number 
    int                       nRetRecordNum;   // Query to the number of records, when the query to the article number less than want to query the number, end 
}NET_OUT_FIND_NEXT_RECORD_PARAM;

typedef struct tagNET_INSERT_RECORD_INFO
{
    DWORD                     dwSize;
    NET_TRAFFIC_LIST_RECORD   *pRecordInfo;    // Record the content information                 
}NET_INSERT_RECORD_INFO;

typedef struct tagNET_UPDATE_RECORD_INFO
{
    DWORD                     dwSize;
    NET_TRAFFIC_LIST_RECORD   *pRecordInfo;    // Record the content information                 
}NET_UPDATE_RECORD_INFO;

typedef struct tagNET_REMOVE_RECORD_INFO
{
    DWORD                     dwSize;
    int                       nRecordNo;       // Queried Record Number    
}NET_REMOVE_RECORD_INFO;

typedef struct tagNET_IN_OPERATE_TRAFFIC_LIST_RECORD
{
    DWORD                     dwSize;
    
    EM_RECORD_OPERATE_TYPE    emOperateType;
    EM_NET_RECORD_TYPE        emRecordType;    // record type to operate 
    void                      *pstOpreateInfo;
    
}NET_IN_OPERATE_TRAFFIC_LIST_RECORD;

//In current implementation of operation interface, only return nRecordNo operation, stRetRecord is temporarily unavailable 
typedef struct tagNET_OUT_OPERATE_TRAFFIC_LIST_RECORD
{
    DWORD                     dwSize;
    int                       nRecordNo;        //Record Number     
}NET_OUT_OPERATE_TRAFFIC_LIST_RECORD;

//PTZ control coordinate unit 
typedef struct tagPTZ_SPEED_UNIT
{
    float                  fPositionX;           //PTZ horizontal speed, normalized to -1~1 
    float                  fPositionY;           //PTZ vertical speed, normalized to -1~1 
    float                  fZoom;                //PTZ aperture magnification, normalized to 0~1 
    char                   szReserve[32];        //Reserved
}PTZ_SPEED_UNIT;

//PTZ control coordinate unit 
typedef struct tagPTZ_SPACE_UNIT
{
    int                    nPositionX;           //PTZ horizontal motion position, effective range��[0,3600]
    int                    nPositionY;           //PTZ vertical motion position, effective range��[-1800,1800]
    int                    nZoom;                //PTZ aperture change position, the effective range��[0,128]
    char                   szReserve[32];        //Reserved
}PTZ_SPACE_UNIT;

//Continuous control PTZ corresponding structure 
typedef struct tagPTZ_Control_Continuously
{
    PTZ_SPEED_UNIT         stuSpeed;              //PTZ speed 
    int                    nTimeOut;              //Continuous motion timeout, the unit is in seconds 
    char                   szReserve[64];         //Reserved
}PTZ_CONTROL_CONTINUOUSLY;

//Absolute control PTZ corresponding structure 
typedef struct tagPTZ_Control_Absolutely
{
    PTZ_SPACE_UNIT         stuPosition;           //PTZ Absolute Speed 
    PTZ_SPEED_UNIT         stuSpeed;              //PTZ Operation Speed
    char                   szReserve[64];         //Reserved
}PTZ_CONTROL_ABSOLUTELY;

// Alarm input channel information 
typedef struct tagNET_ALARM_IN_CHANNEL
{
    DWORD                   dwSize;
    BOOL                    bValid;                             // whether effective,FALSE show the alarm channel is not used
    int                     nChannel;                           // Alarm channel number 
    char                    szDeviceID[DH_DEV_ID_LEN];          // Device ID, Local alarm channel: "Local",remote device : use uuid express
    char                    szName[DH_DEV_NAME_LEN];            // Alarm  Channel Names 
}NET_ALARM_IN_CHANNEL;

// Alarm Channel Number 
typedef struct tagNET_ALARM_CHANNEL_COUNT 
{
    DWORD                   dwSize;
    int                     nLocalAlarmIn;                      // Local Alarm Input Channel Number
    int                     nLocalAlarmOut;                     // Local Alarm Output Channel Number 
    int                     nRemoteAlarmIn;                     // Remote Alarm Input Channel Number 
    int                     nRemoteAlarmOut;                    // Remote Alarm Output Channel Number 
}NET_ALARM_CHANNEL_COUNT;

//With speed rotation site PTZ control corresponding to the preset structure 
typedef struct tagPTZ_Control_GotoPreset
{
    int                     nPresetIndex;           //Preset BIT Index 
    PTZ_SPEED_UNIT          stuSpeed;               //PTZ Operation Speed
    char                    szReserve[64];          //Reserved
}PTZ_CONTROL_GOTOPRESET;

// CLIENT_SetTourSource   Interface input parameters (Settings window round tour shows source) 
typedef struct tagNET_IN_SET_TOUR_SOURCE 
{
    DWORD                   dwSize;
    int                     nChannel;               // Output Channel Number 
    int                     nWindow;                // Window Number
    DH_SPLIT_SOURCE*        pstuSrcs;               // Display Origin Array,can be round tour in the window  
    int                     nSrcCount;              // Display Origin Number
} NET_IN_SET_TOUR_SOURCE;

// CLIENT_SetTourSource    Interface output parameters (Settings window round tour shows source) 
typedef struct tagNET_OUT_SET_TOUR_SOURCE
{
    DWORD                   dwSize;
} NET_OUT_SET_TOUR_SOURCE;

// fAttachRecordInfoCB parameter, information of reported video file 
typedef struct tagNET_CB_RECORD_INFO
{
    DWORD                   dwSize;
    int                     nChannel;               // Channel number 
    char                    szFileName[MAX_PATH];   // The name of the video 
    DWORD                   dwType;                 // Video type,defined by bit as follows��
                                                    // Bit0-Timing video 
                                                    // Bit1-Dynamic test video 
                                                    // Bit2-Alarm video
                                                    // Bit3-Video card number
    DWORD                   dwState;                // Video status, 0 - packaging, 1 - to delete 
} NET_CB_RECORD_INFO;

// Video update callback function prototype��lAttachHandle is return value of CLIENT_AttachRecordInfo, n strip per time��pBuf->dwSize * n == nBufLen
typedef void (CALLBACK *fAttachRecordInfoCB)(LLONG lLoginID, LLONG lAttachHandle, NET_CB_RECORD_INFO* pBuf, int nBufLen, LDWORD dwUser);

// CLIENT_AttachRecordInfo  Input Parameters
typedef struct tagNET_IN_ATTACH_RECORD_INFO
{
    DWORD                   dwSize;
    int                     nInterval;              // Time Interval, Every Interval second, equipment send video information 
    int                     nDelay;                 // Report Delay, after equipment choice a random number between 1 and nDelay value, delay report video state, nDelayMust be smaller than nInterval
    fAttachRecordInfoCB     cbAttachRecordInfo;     // Video update callback function 
    LDWORD                  dwUser;                 // User data 
} NET_IN_ATTACH_RECORD_INFO;

typedef struct tagNET_OUT_ATTACH_RECORD_INFO
{
    DWORD                   dwSize;
} NET_OUT_ATTACH_RECORD_INFO;


//Subscribe to PTZ metadata interface and callback function prototypes 
//Pbufs at this stage mainly DH_PTZ_LOCATION_INFO type 
typedef void (CALLBACK *fPTZStatusProcCallBack)(LLONG lLoginID, LLONG lAttachHandle, void* pBuf, int nBufLen, LDWORD dwUser);

// Subscribe to PTZ metadata interface input parameters 
typedef struct tagNET_IN_PTZ_STATUS_PROC
{
    DWORD                   dwSize;
    int                     nChannel;              // PTZ Channel 
    fPTZStatusProcCallBack  cbPTZStatusProc;       // Callback function 
    LDWORD                  dwUser;                // User data
}NET_IN_PTZ_STATUS_PROC;

// Subscribe to PTZ metadata interface output parameters 
typedef struct tagNET_OUT_PTZ_STATUS_PROC
{
    DWORD                   dwSize;
}NET_OUT_PTZ_STATUS_PROC;

//PTZ conditions for visual structure
typedef struct tagDH_OUT_PTZ_VIEW_RANGE_STATUS
{
    DWORD      dwSize;
    double     dbDistance;                         // Visual range, the unit: m 
    int        nAngelH;                            // Horizontal viewing angles, 0~1800, unit: degrees 
    int        nAzimuthH;                          // Horizontal azimuth Angle, 0 ~ 3600, unit: degrees 
}DH_OUT_PTZ_VIEW_RANGE_STATUS;

//Subscribe to yuntai horizon callback function prototype 
typedef void (CALLBACK *fViewRangeStateCallBack)(LLONG lLoginID, LLONG lAttachHandle, DH_OUT_PTZ_VIEW_RANGE_STATUS* pBuf, int nBufLen, LDWORD dwUser);

// Subscribe to the visible range input parameters 
typedef struct tagNET_IN_VIEW_RANGE_STATE
{
    DWORD                   dwSize;
    int                     nChannel;              // PTZ channel
    fViewRangeStateCallBack cbViewRange;           // State Callback Function 
    LDWORD                  dwUser;                // User Data
}NET_IN_VIEW_RANGE_STATE;

// Subscribe to the visible range output parameters 
typedef struct tagNET_OUT_VIEW_RANGE_STATE
{
    DWORD                   dwSize;
}NET_OUT_VIEW_RANGE_STATE;

//Set the PTZ vision information 
typedef struct tagPTZ_VIEW_RANGE_INFO
{
    int                     nStructSize;
    int                     nAzimuthH;              // Horizontal azimuth Angle, 0~3600, unit: degrees 
}PTZ_VIEW_RANGE_INFO;

// Channel number information 
typedef struct tagNET_CHN_COUNT_INFO
{
    DWORD                   dwSize;
    int                     nMaxTotal;              // Equipment to the total number of channels (the sum of all valid channel number
    int                     nCurTotal;              // the number of configured on channels 
    int                     nMaxLocal;              // Maximum number of local channels, including motherboard and pluggable cartoon 
    int                     nCurLocal;              // configured local channel number 
    int                     nMaxRemote;             // Maximum number of remote channel 
    int                     nCurRemote;             // Configured remote channel number
} NET_CHN_COUNT_INFO;

// Equipment channel number information  
typedef struct tagNET_DEV_CHN_COUNT_INFO
{
    DWORD                   dwSize;
    NET_CHN_COUNT_INFO      stuVideoIn;             // Video Input Channel 
    NET_CHN_COUNT_INFO      stuVideoOut;            // Video Output Channel 
} NET_DEV_CHN_COUNT_INFO;

//  detailed information of video state
typedef struct tagNET_RECORD_STATE_DETAIL 
{
    DWORD                   dwSize;
    BOOL                    bMainStream;            // The main stream, TRUE - are video, FALSE - not in the video 
    BOOL                    bExtraStream1;          // Auxiliary stream 1, TRUE - are video, FALSE - not in the video 
    BOOL                    bExtraStream2;          // Auxiliary stream 2, TRUE - are video, FALSE - not in the video 
    BOOL                    bExtraStream3;          // Auxiliary stream 3, TRUE - are video, FALSE - not in the video 
} NET_RECORD_STATE_DETAIL;

// OSN Interface return to status code 
typedef enum{
    EM_OSN_OK,                                      // Successful Operation 
    EM_OSN_OK_P2P,                                  // Operation is successful, create P2P mapping type 
    EM_OSN_OK_RELAYED,                              // Operation is successful, create RALAYED mapping type 
    EM_OSN_ERROR_API_NOT_INITIALIZED,               // Failed to initialize 
    EM_OSN_ERROR_NO_NETWORK,                        // No Available Net
    EM_OSN_ERROR_CANNOT_CONNECT_TO_AGENT,           // Unable to connect to the specified host 
    EM_OSN_ERROR_LOCAL_PORT_ALREADY_USED,           // Unable to connect to the specified host 
    EM_OSN_ERROR_AGENT_RESOURCES_LIMIT_REACHED,     // Unable to connect, maximum number of connections to achieve 
    EM_OSN_ERROR_INVALID_DEVICE_ID,                 // Device ID illegal 
    EM_OSN_ERROR_INVALID_SERVICE_PORT,              // Service port illegal 
    EM_OSN_ERROR_INVALID_LOCAL_PORT,                // Local port illegal
    EM_OSN_ERROR_INVALID_TUNNEL,                    // mapping information illegal(when deleting the mapping information mapping) 
    EM_OSN_ERROR                                    // unknown error 
}EM_OSN_STATUS;

//Mapping the input parameters 
typedef struct {
    unsigned int             servicePort;           // Service port (for example, 80900, 0102, 5 or 23) 
    unsigned int             localPort;             // local port 1024 - 65536��0 means automatically assigned 
    const char*              pdeviceId;             // Device ID 
    BYTE                     Reserved[64];          // reserve
}OSN_IN_CREATE_TUNNEL_PARAM, *LOSN_IN_CREATE_TUNNEL_PARAM;

// Establish a mapping output parameters 
typedef struct {
    unsigned int             servicePort;           // Service port (for example, 80900, 0102, 5 or 23) 
    unsigned int             localPort;             // local port  1024 - 65536��0 means automatically assigned
    EM_OSN_STATUS            tunnelStatus;
    const char*              pdeviceId;             // Device ID 
    BYTE                     Reserved[64];          // reserve
}OSN_OUT_CREATE_TUNNEL_PARAM, *LOSN_OUT_CREATE_TUNNEL_PARAM;

// Delete the map input parameters 
typedef struct {
    unsigned int             servicePort;           // Service port (for example, 80,9000,1025 or 23)
    unsigned int             localPort;             // local port 1024 - 65536��0 means automatically assigned
    EM_OSN_STATUS            tunnelStatus;
    const char*              pdeviceId;             // Device ID 
    BYTE                     Reserved[64];          // reserve
}OSN_IN_DESTROY_TUNNEL_PARAM, *LOSN_IN_DESTROY_TUNNEL_PARAM;

//PTZ Absolute Focus Corresponding Structure 
typedef struct tagPTZ_Focus_Absolutely
{
    DWORD                    dwValue;               // PTZ Focused On Location, range (0~8191) 
    DWORD                    dwSpeed;               // PTZ Focused On Speed, the scope (0~7) 
    char                     szReserve[64];         // reserved 64 bytes 
}PTZ_FOCUS_ABSOLUTELY;

// CLIENT_PlayAudioFile   Interface Input Parameters 
typedef struct tagNET_IN_PLAY_AUDIO_FILE
{
    DWORD                   dwSize;
    const char*             pszFilePath;            // File Path
    DWORD                   dwOutput;               // Output Path, can be a variety of output, according to the bit, bit0-mic, bit1-speaker
} NET_IN_PLAY_AUDIO_FILE;

// CLIENT_PlayAudioFile   Interface Output Parameters 
typedef struct tagNET_OUT_PLAY_FILE_STREAM
{
    DWORD                   dwSize;
} NET_OUT_PLAY_AUDIO_FILE;

// RTSP URLInformation list structure 
typedef struct tagNET_DEV_RTSPURL_LIST
{
    DWORD                   dwSize;
    int                     nChannelID;                                 // Channel number (user input parameters) 
    int                     nUrlNum;                                    // Url Address Number 
    char                    szURLList[DH_MAX_URL_NUM][DH_MAX_URL_LEN];  // UrlAddress List 
}NET_DEV_RTSPURL_LIST;

// PTZ control - fan and corresponding structure 
typedef struct tagPTZ_Control_SectorScan
{
    int             nBeginAngle;                    // Staring Angle,Range:[-180,180]
    int             nEndAngle;                      // Ending Angle,Range:[-180,180]
    int             nSpeed;                         // Speed,Range:[0,255]
    char            szReserve[64];                  // Reserved 64 bytes 
}PTZ_CONTROL_SECTORSCAN;

// CLIENT_TransmitInfoForWeb   Interface Extension Parameters 
typedef struct tagNET_TRANSMIT_EXT_INFO
{
    DWORD           dwSize;
    unsigned char*  pInBinBuf;                      // Binary Input Data Buffer 
    DWORD           dwInBinBufSize;                 // Binary Input Data Length 
} NET_TRANSMIT_EXT_INFO;

// Monitor CAN Bus Data 
// fAttachProxyCB Parameter 
typedef struct tagNET_CB_CANDATA
{
    DWORD           dwSize;
    int             nDataLength;                    // Common Data Length
    unsigned char*  pDataContent;                   // Common Data Content
}NET_CB_CANDATA;

// Monitor CAN bus data callback function prototype ��lAttachHandle is return value of CLIENT_AttachCAN
typedef void (CALLBACK *fAttachCANCB) (LLONG lLoginID, LLONG lAttachHandle, NET_CB_CANDATA* pBuf, LDWORD dwUser);

// CLIENT_AttachCAN() Input Parameters 
typedef struct tagNET_IN_ATTACH_CAN
{
    DWORD          dwSize;
    int            nChannel;                        // Monitor CAN channel number 
    fAttachCANCB   cbAttachCAN;                     // Callback Registered Video Phone State Changes 
    LDWORD         dwUser;                          // User Data 
}NET_IN_ATTACH_CAN;

// CLIENT_AttachCAN() Output Parameters 
typedef struct tagNET_OUT_ATTACH_CAN
{
    DWORD          dwSize;
}NET_OUT_ATTACH_CAN;

// Gets the current equipment serial number��corresponding CLIENT_QueryDevState() Interface's DH_DEVSTATE_GET_COMM_COUNT command parameter 
typedef struct tagNET_GET_COMM_COUNT
{
    DWORD          dwSize;
    int            nChannelCout;                    // Number Of Accessed serial port
}NET_GET_COMM_COUNT;

// CLIENT_ExChangeData �ӿ�����ṹ��
typedef struct tagNET_IN_EXCHANGEDATA
{
    DWORD           dwSize;
    int             nChannel;                       // ���ں�
    BOOL            bFlag;                          // TRUE Ϊ�������ݲ��ȴ��ظ���FALSE ֻ�ǲɼ�����
    int             nCollectTime;                   // ���ڲɼ�ʱ��(��λ������)
    int             nSendDataLength;                // �������ݳ��ȣ�bFlag = TRUE ��Ч
    unsigned char*  pSendData;                      // �����������ݣ�bFlag = TRUE ��Ч
}NET_IN_EXCHANGEDATA;

// CLIENT_ExChangeData �ӿ�����ṹ��
typedef struct tagNET_OUT_EXCHANGEDATA
{
    DWORD           dwSize;
}NET_OUT_EXCHANGEDATA;

// ��̨Ԥ�õ�
typedef struct tagNET_PTZ_PRESET 
{
    int                     nIndex;                         // ���
    char                    szName[PTZ_PRESET_NAME_LEN];    // ����
    char                    szReserve[64];                  // Ԥ��64�ֽ�
} NET_PTZ_PRESET;

// ��̨Ԥ�õ��б�
typedef struct tagNET_PTZ_PRESET_LIST 
{
    DWORD                   dwSize;
    DWORD                   dwMaxPresetNum;                 // Ԥ�õ�������
    DWORD                   dwRetPresetNum;                 // ����Ԥ�õ����
    NET_PTZ_PRESET          *pstuPtzPorsetList;             // Ԥ�õ��б�(�����Ҫ���������������ڴ�)
} NET_PTZ_PRESET_LIST;

// �¼����� EVENT_IVS_TRAFFIC_RESTRICTED_PLATE (���޳����¼�)��Ӧ�����ݿ�������Ϣ
typedef struct tagDEV_EVENT_TRAFFIC_RESTRICTED_PLATE
{
    int                     nChannelID;                     // ͨ����
    char                    szName[DH_EVENT_NAME_LEN];      // �¼�����
    int                     nTriggerType;                   // TriggerType:�������ͣ�0��������1�״2��Ƶ
    DWORD                   PTS;                            // ʱ���(��λ�Ǻ���)
    NET_TIME_EX             UTC;                            // �¼�������ʱ��
    int                     nEventID;                       // �¼�ID
    int                     nSequence;                      // ��ʾץ����ţ���3,2,1,1��ʾץ�Ľ���,0��ʾ�쳣����
    BYTE                    byEventAction;                  // �¼�������0��ʾ�����¼�,1��ʾ�������¼���ʼ,2��ʾ�������¼�����;
    BYTE                    byImageIndex;                   // ͼƬ�����, ͬһʱ����(��ȷ����)�����ж���ͼƬ, ��0��ʼ
    BYTE                    byReserved1[2];
    int                     nLane;                          // ��Ӧ������
    DH_MSG_OBJECT           stuObject;                      // ��⵽������
    DH_MSG_OBJECT           stuVehicle;                     // ������Ϣ
    DH_EVENT_FILE_INFO      stuFileInfo;                    // �¼���Ӧ�ļ���Ϣ
    int                     nMark;                          // �ײ�����Ĵ���ץ��֡���
    int                     nFrameSequence;                 // ��Ƶ����֡���
    int                     nSource;                        // ��Ƶ����������Դ��ַ
    DWORD                   dwSnapFlagMask;                 // ץͼ��־(��λ)�������NET_RESERVED_COMMON    
    DH_RESOLUTION_INFO      stuResolution;                  // ��ӦͼƬ�ķֱ���
    DEV_EVENT_TRAFFIC_TRAFFICCAR_INFO stuTrafficCar;        // ��ͨ������Ϣ
    BYTE                    byReserved[1024];               // �����ֽ�
}DEV_EVENT_TRAFFIC_RESTRICTED_PLATE;

// �����Դ�쳣����
typedef struct tagALARM_POWER_ABNORMAL_INFO
{
    DWORD                   dwSize;
    int                     nChannelID;                     // ͨ����
    NET_TIME                stuTime;                        // ʱ��
    DWORD                   dwAction;                       // �¼�����, 0:Start, 1:Stop
}ALARM_POWER_ABNORMAL_INFO;


#define UPDATE_VERSION_LEN        64

// ��ȡ�豸����״̬��Ӧ�ṹ��
typedef struct tagDHDEV_UPGRADE_STATE_INFO
{
    int                  nState;                            // 0:None-û�м�⵽����, Ĭ��״̬; 1:Regular-һ������; 
                                                            // 2:Emergency-ǿ������; 3:Upgrading-������
    
    char                szOldVersion[UPDATE_VERSION_LEN];   // �ɰ汾
    char                szNewVersion[UPDATE_VERSION_LEN];   // �°汾
    DWORD               dwProgress;                         // ��������
    int                 reserved[256];                      // �����ֶ�
}DHDEV_UPGRADE_STATE_INFO;


///////////////////////////////// ��¼�Ự /////////////////////////////////////////

// CLIENT_StartBurnSession �ӿ��������
typedef struct tagNET_IN_START_BURN_SESSION 
{
    DWORD               dwSize;
    unsigned int        nSessionID;                         // �ỰID
} NET_IN_START_BURN_SESSION;

typedef struct tagNET_OUT_START_BURN_SESSION 
{
    DWORD               dwSize;
} NET_OUT_START_BURN_SESSION;

// ��¼ģʽ
typedef enum tagNET_BURN_MODE
{
    BURN_MODE_SYNC,                     // ͬ��
    BURN_MODE_TURN,                     // ����
    BURN_MODE_CYCLE,                    // ѭ��
} NET_BURN_MODE;

// ��¼����ʽ
typedef enum tagNET_BURN_RECORD_PACK
{
    BURN_PACK_DHAV,                     // DHAV
    BURN_PACK_PS,                       // PS
    BURN_PACK_ASF,                      // ASF
    BURN_PACK_MP4,                      // MP4
    BURN_PACK_TS,                       // TS
} NET_BURN_RECORD_PACK;

// CLIENT_StartBurn �ӿ��������
typedef struct tagNET_IN_START_BURN 
{
    DWORD                dwSize;
    DWORD                dwDevMask;                             // ��¼�豸����, ��λ��ʾ�����¼�豸���
    int                  nChannels[DH_MAX_BURN_CHANNEL_NUM];    // ��¼ͨ������
    int                  nChannelCount;                         // ��¼ͨ����
    NET_BURN_MODE        emMode;                                // ��¼ģʽ
    NET_BURN_RECORD_PACK emPack;                                // ��¼����ʽ
} NET_IN_START_BURN;

// CLIENT_StartBurn �ӿ��������
typedef struct tagNET_OUT_START_BURN 
{
    DWORD               dwSize;
} NET_OUT_START_BURN;

// CLIENT_BurnMarkTag �ӿ��������
typedef struct tagNET_IN_BURN_MAAK_TAG
{
    DWORD               dwSize;
    const char*         pszDescInfo;                            // ���������Ϣ
} NET_IN_BURN_MARK_TAG;

// CLIENT_BurnMarkTag �ӿ��������
typedef struct tagNET_OUT_BURN_MAAK_TAG
{
    DWORD               dwSize;
} NET_OUT_BURN_MARK_TAG;

// CLIENT_BurnChangeDisk �ӿ��������
typedef struct tagNET_IN_BURN_CHANGE_DISK
{
    DWORD               dwSize;
    BOOL                nAction;                                // ���̶���, 0-��ʼ, 1-����
} NET_IN_BURN_CHANGE_DISK;

// CLIENT_BurnChangeDisk �ӿ��������
typedef struct tagNET_OUT_BURN_CHANGE_DISK
{
    DWORD               dwSize;
} NET_OUT_BURN_CHANGE_DISK;

// ��¼״̬
typedef enum tagNET_BURN_STATE
{
    BURN_STATE_STOP,                    // ֹͣ
    BURN_STATE_STOPING,                 // ֹͣ��
    BURN_STATE_INIT,                    // ��ʼ��
    BURN_STATE_BURNING,                 // ��¼��
    BURN_STATE_PAUSE,                   // ��ͣ
    BURN_STATE_CHANGE_DISK,             // ������
    BURN_STATE_PREPARE_EXTRA_FILE,      // ������ʼ��
    BURN_STATE_WAIT_EXTRA_FILE,         // �ȴ�������¼
    BURN_STATE_UPLOAD_FILE_START,       // ������¼��
    BURN_STATE_CHECKING_DISK,           // ��������
    BURN_STATE_DISK_READY,              // ����׼������
} NET_BURN_STATE;

// ��¼������
typedef enum tagNET_BURN_ERROR_CODE
{
    BURN_CODE_NORMAL,                   // ����
    BURN_CODE_UNKNOWN_ERROR,            // δ֪����
    BURN_CODE_SPACE_FULL,               // ��¼��
    BURN_CODE_START_ERROR,              // ��ʼ��¼����
    BURN_CODE_STOP_ERROR,               // ����
    BURN_CODE_WRITE_ERROR,              // ��¼����
} NET_BURN_ERROR_CODE;

// CLIENT_BurnGetState �ӿ��������
typedef struct tagNET_IN_BURN_GET_STATE
{
    DWORD                dwSize;
} NET_IN_BURN_GET_STATE;

// ��¼�豸״̬
typedef struct tagNET_BURN_DEV_STATE 
{
    DWORD                dwSize;
    int                  nDeviceID;                             // �����豸ID
    char                 szDevName[DH_BURNING_DEV_NAMELEN];     // �����豸����
    DWORD                dwTotalSpace;                          // ����������, ��λKB
    DWORD                dwRemainSpace;                         // ����ʣ������, ��λKB
} NET_BURN_DEV_STATE;

// CLIENT_BurnGetState �ӿ��������
typedef struct tagNET_OUT_BURN_GET_STATE
{
    DWORD                dwSize;
    NET_BURN_STATE       emState;                               // ��¼״̬
    NET_BURN_ERROR_CODE  emErrorCode;                           // ������
    DWORD                dwDevMask;                             // ��¼�豸����, ��λ��ʾ�����¼�豸���
    int                  nChannels[DH_MAX_BURN_CHANNEL_NUM];    // ��¼ͨ������
    int                  nChannelCount;                         // ��¼ͨ����
    NET_BURN_MODE        emMode;                                // ��¼ģʽ
    NET_BURN_RECORD_PACK emPack;                                // ��¼����ʽ
    int                  nFileIndex;                            // ��ǰ��¼�ļ����
    NET_TIME             stuStartTime;                          // ��¼��ʼʱ��
    NET_BURN_DEV_STATE   stuDevState[DH_MAX_BURNING_DEV_NUM];   // ��¼�豸״̬
} NET_OUT_BURN_GET_STATE;

// fAttachBurnStateCB ����
typedef struct tagNET_CB_BURNSTATE
{
    DWORD               dwSize;
    const char*         szState;                        // ��Ϣ����
                                                        //"UploadFileStart"�����Կ�ʼ�����ϴ�
                                                        //"InitBurnDevice":��ʼ����¼�豸
                                                        //"Burning":��¼��
                                                        //"BurnExtraFileStop"����¼ֹͣ
                                                        //"BurnFilePause":��¼��ͣ
                                                        //"SpaceFull":��¼�ռ���
                                                        //"BurnFileError":��¼����    
    const char*         szFileName;                     // ��ǰ��¼�����ļ���,����"UploadFileStart"��ʼ�����ϴ���Ϣ
    unsigned int        dwTotalSpace;                   // ������,��λKB������"Burning"��¼�У���ʾ������������
    unsigned int        dwRemainSpace;                  // ʣ������,��λKB������"Burning"��¼��
    const char*         szDeviceName;                   // ��¼�豸����,�������ֲ�ͬ�Ŀ�¼�豸
}NET_CB_BURNSTATE;

// ��¼�豸�ص�����ԭ�Σ�lAttachHandle��CLIENT_AttachBurnState����ֵ, ÿ��1����pBuf->dwSize == nBufLen
typedef void (CALLBACK *fAttachBurnStateCB) (LLONG lLoginID, LLONG lAttachHandle, NET_CB_BURNSTATE* pBuf, int nBufLen, LDWORD dwUser);

// ��¼�豸�ص���չ����ԭ��
typedef void (CALLBACK *fAttachBurnStateCBEx)(LLONG lLoginID, LLONG lAttachHandle, NET_OUT_BURN_GET_STATE* pBuf, int nBufLen, LDWORD dwUser);

// CLIENT_AttachBurnState()�������
typedef struct tagNET_IN_ATTACH_STATE
{
    DWORD                   dwSize;
    const char*             szDeviceName;                   // �������ƣ���"/dev/sda"
    fAttachBurnStateCB      cbAttachState;                  // ��¼�����ص�
    LDWORD                  dwUser;                         // �û�����
    LLONG                   lBurnSession;                   // ��¼�Ự���, CLIENT_StartBurnSession�ķ���ֵ. ��ֵΪ0ʱ, szDeviceName��Ч, ��ʱ����¼�豸���Ŀ�¼״̬
    fAttachBurnStateCBEx    cbAttachStateEx;                // ��չ��¼�����ص�
    LDWORD                  dwUserEx;                       // ��չ��¼�����ص��û�����
}NET_IN_ATTACH_STATE;

// CLIENT_AttachBurnState �������
typedef struct tagNET_OUT_ATTACH_STATE
{
    DWORD                   dwSize;
}NET_OUT_ATTACH_STATE;

// ��¼�豸�ص�����ԭ�Σ�lUploadHandle��CLIENT_StartUploadFileBurned����ֵ
typedef void (CALLBACK *fBurnFileCallBack) (LLONG lLoginID, LLONG lUploadHandle, int nTotalSize, int nSendSize, LDWORD dwUser);

// CLIENT_StartUploadFileBurned()�������
typedef struct tagNET_IN_FILEBURNED_START
{
    DWORD                   dwSize;
    const char*             szMode;                        // �ļ��ϴ���ʽ"append",׷��ģʽ,��ʱ��¼�ļ����̶�Ϊ" FILE.zip ",filename������
    const char*             szDeviceName;                  // �������ƣ��硰/dev/sda��
    const char*             szFilename;                    // �����ļ�����
    fBurnFileCallBack       cbBurnPos;                     // ��¼���Ȼص�
    LDWORD                  dwUser;                        // �û�����
    LLONG                   lBurnSession;                  // ��¼���, CLIENT_StartBurnSession�ķ���ֵ. ��ֵΪ0ʱ, szDeviceName��Ч, ��ʱ����¼�豸���Ŀ�¼״̬
}NET_IN_FILEBURNED_START;

// CLIENT_StartUploadFileBurned �������
typedef struct tagNET_OUT_FILEBURNED_START
{
    DWORD                   dwSize;
    char                    szState[DH_MAX_NAME_LEN];      // "start"��ϵͳ׼������,���Կ�ʼ�ϴ�; "busy"��ϵͳæ,�Ժ����ԡ�"error"��ϵͳδ�ڿ�¼��,���س���,����ʧ��
}NET_OUT_FILEBURNED_START;

// ��¼������Ϣ
typedef struct tagNET_BURN_CASE_INFO
{
    DWORD       dwSize;
    int         nChannel;                                   // ͨ����
    NET_TIME    stuStartTime;                               // ��¼��ʼʱ��
    NET_TIME    stuEndTime;                                 // ��¼����ʱ��
    int         nIndex;                                     // ���
    int         nCode;                                      // ���
    int         nDiscNum;                                   // ���̱��        
    char        szName[DH_COMMON_STRING_128];               // ����
    char        szPlace[DH_COMMON_STRING_128];              // �참�ص�
    char        szInvestigator[DH_MAX_CASE_PERSON_NUM][DH_COMMON_STRING_32]; // �참��Ա
    char        szSuspects[DH_MAX_CASE_PERSON_NUM][DH_COMMON_STRING_32];     // �永��Ա
    char        szMemo[DH_COMMON_STRING_256];               // ��ע
    char        szVideoName[DH_COMMON_STRING_128];          // ¼������
    char        szRecorder[DH_COMMON_STRING_32];            // ��¼��
} NET_BURN_CASE_INFO;

// ��¼�豸�ص�����ԭ�Σ�lAttachHandle��CLIENT_AttachBurnCase����ֵ
typedef void (CALLBACK *fBurnCaseCallBack) (LLONG lAttachHandle, NET_BURN_CASE_INFO* pBuf, DWORD dwBufLen, void* pReserved, LDWORD dwUser);

// CLIENT_AttachBurnCase �ӿ��������
typedef struct tagNET_IN_ATTACH_BURN_CASE
{
    DWORD                dwSize;
    fBurnCaseCallBack    cbBurnCase;                        // ������Ϣ�ص�����
    LDWORD               dwUser;                            // �û�����
} NET_IN_ATTACH_BURN_CASE;

// CLIENT_AttachBurnCase �ӿ��������
typedef struct tagNET_OUT_ATTACH_BURN_CASE
{
    DWORD                dwSize;
} NET_OUT_ATTACH_BURN_CASE;

/////////////////////////////////// Storage ///////////////////////////////////////

// Զ�̴洢д����
typedef enum tagNET_STORAGE_WRITE_STATE
{
    NS_WRITE_UNKNOWN,                                   // δ֪
    NS_WRITE_OK,                                        // �ɹ�
    NS_WRITE_DISK_IO_ERROR,                             // ����IO����
    NS_WRITE_DISK_NOT_READY,                            // ����δ׼����
    NS_WRITE_DISK_FAULT,                                // ���̹���
    NS_WRITE_STREAM_NOT_EXIST,                          // ������ID������
    NS_WRITE_MOUNT_DISK_ERROR,                          // ���̹��ش���
} NET_STORAGE_WRITE_STATE;

// Զ��������Ϣ
typedef struct tagNET_STORAGE_BLOCK 
{
    DWORD                   dwSize;
    unsigned int            nID;                        // CQFS��ID
    unsigned int            nStreamID;                  // ������Ψһ��ʶ
    unsigned int            nRecycleTimestamp;          // CQFS����ʱ���
    unsigned int            nBeginTimestamp;            // �������ݿ�ʼʱ���
    unsigned int            nEndTimestamp;              // �������ݽ���ʱ���
    unsigned int            nLockCount;                 // ��������ֵ, 0��ʾδ����
    char                    szDiskUUID[DH_COMMON_STRING_64]; // ����UUID
} NET_STORAGE_BLOCK;

// Զ�̴洢����Ϣ״̬
typedef struct tagNET_STORAGE_WRITE_INFO
{
    DWORD                    dwSize;
    NET_STORAGE_WRITE_STATE  emState;                   // д����
    int                      nBlockCount;               // ������
    NET_STORAGE_BLOCK        stuBlocks[DH_MAX_NET_STRORAGE_BLOCK_NUM]; // ����Ϣ
} NET_STORAGE_WRITE_INFO;

//////////////////////////////////////////////////////////////////////////
/// \fn ����д��Զ�̴洢����Ϣ״̬�ص�����
/// \brief 
/// \author yang_xiuqing
/// \param  LLONG lAttachHandle [OUT] ���ľ��, CLIENT_NetStorageAttachWriteInfo�ķ���ֵ 
/// \param  NET_STORAGE_WRITE_INFO * pBuf [OUT] Զ�̴洢����Ϣ״̬
/// \param  int nBufLen [OUT] ״̬��Ϣ����
/// \param  LDWORD dwUser �û�����
/// \return ��
///////////////////////////////////////////////////////////////////////////
typedef void (CALLBACK *fNetStorageAttachWriteInfoCB)(LLONG lAttachHandle, NET_STORAGE_WRITE_INFO* pBuf, int nBufLen, LDWORD dwUser);

// CLIENT_NetStorageAttachWriteInfo �ӿ��������
typedef struct tagNET_IN_STORAGE_ATTACH_WRITE_INFO
{
    DWORD                           dwSize;
    const char*                     pszName;                // Զ�̴洢����, ֵ��NAS�����л�ȡ
    fNetStorageAttachWriteInfoCB    cbISCSIBlcok;           // �ص�����
    LDWORD                          dwUser;                 // �û�����
} NET_IN_STORAGE_ATTACH_WRITE_INFO;

// CLIENT_NetStorageAttachWriteInfo �ӿ��������
typedef struct tagNET_OUT_STORAGE_ATTACH_WRITE_INFO
{
    DWORD                       dwSize;
} NET_OUT_STORAGE_ATTACH_WRITE_INFO;

// CLIENT_NetStorageGetWriteInfo �ӿ��������
typedef struct tagNET_IN_STORAGE_GET_WRITE_INFO 
{
    DWORD                       dwSize;
    const char*                 pszName;                    // Զ�̴洢����
} NET_IN_STORAGE_GET_WRITE_INFO;

// CLIENT_NetStorageGetWriteInfo �ӿ��������
typedef struct tagNET_OUT_STORAGE_GET_WRITE_INFO 
{
    DWORD                       dwSize;    
    int                         nBlockCount;                // ������
    NET_STORAGE_BLOCK           stuBlocks[DH_MAX_NET_STRORAGE_BLOCK_NUM]; // ����Ϣ
} NET_OUT_STORAGE_GET_WRITE_INFO;

// RAID��������
typedef enum tagNET_RAID_OPERATE_TYPE
{
    NET_RAID_OPERATE_ADD,                    // ����RAID, ��Ӧ�ṹ�� NET_IN_RAID_ADD �� NET_OUT_RAID_ADD
    NET_RAID_OPERATE_REMOVE,                 // ɾ��RAID, ��Ӧ�ṹ�� NET_IN_RAID_REMOVE �� NET_OUT_RAID_REMOVE
    NET_RAID_OPERATE_GET_SUBDEVICE,          // ��ȡRAID���豸��Ϣ, ��Ӧ�ṹ�� NET_IN_RAID_GET_SUBDEVICE �� NET_OUT_RAID_GET_SUBDEVICE
    NET_RAID_OPERATE_GET_SUBSMART,           // ��ȡRAID���豸SMART��Ϣ, ��Ӧ�ṹ�� NET_IN_RAID_GET_SUBSMART �� NET_OUT_RAID_GET_SUBSMART
} NET_RAID_OPERATE_TYPE;

// RAID��Ϣ
typedef struct tagNET_RAID_INFO 
{
    DWORD                dwSize;
    char                 szName[DH_COMMON_STRING_64];     // ����, Ϊ��ʱ�������豸����
    char                 szLevel[DH_COMMON_STRING_16];    // �ȼ�, ����"RAID0", "RAID5"��
    int                  nMemberNum;                      // ��Ա����
    char                 szMembers[DH_MAX_MEMBER_PER_RAID][DH_COMMON_STRING_64]; // ��Ա��Ϣ
} NET_RAID_INFO;

// ����RAID�ķ��ؽ��
typedef struct tagNET_RAID_ADD_RESULT 
{
    DWORD                dwSize;
    BOOL                 bResult;
    DWORD                dwErrorCode;                    // ʧ�ܴ�����
    char                 szName[DH_COMMON_STRING_64];    // ����
} NET_RAID_ADD_RESULT;

// ����RAID�������
typedef struct tagNET_IN_RAID_ADD 
{
    DWORD                dwSize;
    int                    nRaidNun;                        // RAID����
    NET_RAID_INFO        stuRaids[DH_MAX_RAID_NUM];         // RAID��Ϣ
} NET_IN_RAID_ADD;

// ����RAID�������
typedef struct tagNET_OUT_RAID_ADD 
{
    DWORD                dwSize;
    int                  nResultNum;                        // �����
    NET_RAID_ADD_RESULT  stuResults[DH_MAX_RAID_NUM];       // RAID�����Ľ��
} NET_OUT_RAID_ADD;

// ɾ��RAID�ķ��ؽ��
typedef struct tagNET_RAID_REMOVE_RESULT 
{
    DWORD                dwSize;
    BOOL                 bResult;
    DWORD                dwErrorCode;                       // ʧ�ܴ�����
} NET_RAID_REMOVE_RESULT;

// ɾ��RAID�������
typedef struct tagNET_IN_RAID_REMOVE 
{
    DWORD                dwSize;
    int                  nRaidNum;                         // RAID����
    char                 szRaids[DH_MAX_RAID_NUM][DH_COMMON_STRING_64];    // RAID��������
} NET_IN_RAID_REMOVE;

// ɾ��RAID�������
typedef struct tagNET_OUT_RAID_REMOVE
{
    DWORD                dwSize;
    int                  nResultNum;                       // �����
    NET_RAID_REMOVE_RESULT stuResults[DH_MAX_RAID_NUM];    // RAID�����Ľ��
} NET_OUT_RAID_REMOVE;

// ��ȡRAID���豸��Ϣ�������
typedef struct tagNET_IN_RAID_GET_SUBDEVICE 
{
    DWORD                dwSize;
    const char*          pszRaidName;                    // RAID����
} NET_IN_RAID_GET_SUBDEVICE;

// ��ȡRAID���豸��Ϣ�������
typedef struct tagNET_OUT_RAID_GET_SUBDEVICE 
{
    DWORD                dwSize;
    int                  nSubDeviceNum;                  // ���豸����
    DH_STORAGE_DEVICE    stuSubDevices[DH_MAX_MEMBER_PER_RAID]; // ���豸��Ϣ
} NET_OUT_RAID_GET_SUBDEVICE;

// RAID���豸SMART��Ϣ
typedef struct tagNET_RAID_SMART_INFO 
{
    DWORD                dwSize;
    unsigned int         nID;                            // ����ID
    char                 szName[DH_COMMON_STRING_64];    // ������
    int                  nCurrent;                       // ����ֵ
    int                  nWorst;                         // ������ֵ
    int                  nThreshold;                     // ��ֵ
    int                  nPredict;                       // ״̬
    char                 szRaw[DH_COMMON_STRING_16];     // ʵ��ֵ
} NET_RAID_SMART_INFO;

// ��ȡRAID���豸SMART��Ϣ�������
typedef struct tagNET_IN_RAID_GET_SUBSMART
{
    DWORD                dwSize;
    const char*          pszSubDevName;                  // RAID���豸����
} NET_IN_RAID_GET_SUBSMART;

// ��ȡRAID���豸SMART��Ϣ�������
typedef struct tagNET_OUT_RAID_GET_SUBSMART
{
    DWORD                dwSize;
    int                  nSmartNum;                          // SMART��Ϣ��
    NET_RAID_SMART_INFO  stuSmartInfos[MAX_SMART_VALUE_NUM]; // SMART��Ϣ
} NET_OUT_RAID_GET_SUBSMART;

//////////////////////////////// �ⲿ�豸 //////////////////////////////////////////

// ����豸����
typedef enum tagNET_EXT_DEV_TYPE
{
    EXT_DEV_UNKNOWN,                    // δ֪
    EXT_DEV_PROJECTOR,                  // ͶӰ��
    EXT_DEV_SEQUENCE_POWER,            // ��Դ������
} NET_EXT_DEV_TYPE;

// ����豸��Ϣ
typedef struct tagNET_EXTERNAL_DEVICE 
{
    DWORD               dwSize;
    NET_EXT_DEV_TYPE    emType;                             // �豸����
    char                szDevID[DH_DEV_ID_LEN_EX];          // �豸ID, Ψһ���
    char                szDevName[DH_DEV_ID_LEN_EX];        // �豸����
} NET_EXTERNAL_DEVICE;

// ��Դʱ�������Ʋ���
typedef struct tagNET_CTRL_SEQPOWER_PARAM
{
    DWORD               dwSize;
    const char*         pszDeviceID;                        // �豸ID
    int                 nChannel;                           // ����ڻ���������
} NET_CTRL_SEQPOWER_PARAM;

// ͶӰ�ǿ��Ʋ���
typedef struct tagNET_CTRL_PROJECTOR_PARAM 
{
    DWORD               dwSize;
    const char*         pszDeviceID;                        // �豸ID
} NET_CTRL_PROJECTOR_PARAM;

// ���ⰴ��
typedef struct tagNET_CTRL_INFRARED_KEY_PARAM 
{
    DWORD               dwSize;
    int                 nChannel;                           // �������ͨ����
    unsigned int        nKey;                               // ���ⰴ��ID
} NET_CTRL_INFRARED_KEY_PARAM;

// ��Ӳ�̱���
typedef struct tagALARM_NO_DISK_INFO
{
    DWORD               dwSize;
    NET_TIME            stuTime;                            // ʱ��
    DWORD               dwAction;                           // �¼�����, 0:Start, 1:Stop
}ALARM_NO_DISK_INFO;
//������״̬ö��
typedef enum __EM_CAPSULE_STATE
{
    CAPSULE_STATE_UNKNOW ,      //δ����
    CAPSULE_STATE_NORMAL ,      //����
    CAPSULE_STATE_TIME_OUT ,    //��ʱ
}EM_CAPSULE_STATE;

//�������¼�
typedef struct tagALARM_PROTECTIVE_CAPSULE_INFO
{
    DWORD               dwSize;
    EM_CAPSULE_STATE    emCapsuleState; //������״̬
    DWORD               nLock;          //0:δ����1:����; ����emCapsuleStateΪCAPSULE_STATE_NORMALʱ��Ч
    DWORD               nInfrared;      //0:����(����),1:����(����);����emCapsuleStateΪCAPSULE_STATE_NORMALʱ��Ч
}ALARM_PROTECTIVE_CAPSULE_INFO;

// �豸��������, ��ӦCLIENT_GetDevCaps�ӿ�
#define NET_DEV_CAP_SEQPOWER            0x01                // ��Դʱ��������, pInBuf=NET_IN_CAP_SEQPOWER*, pOutBuf=NET_OUT_CAP_SEQPOWER*
#define NET_ENCODE_CFG_CAPS             0x02                // �豸�������ö�Ӧ����, pInBuf=NET_IN_ENCODE_CFG_CAPS*, pOutBuf= NET_OUT_ENCODE_CFG_CAPS*

// ��ȡ��Դʱ���������������
typedef struct tagNET_IN_CAP_SEQPOWER 
{
    DWORD                dwSize;
    const char*          pszDeviceID;                       // �豸ID
} NET_IN_CAP_SEQPOWER;

// ��ȡ��Դʱ���������������
typedef struct tagNET_OUT_CAP_SEQPOWER
{
    DWORD                dwSize;
    int                  nChannelNum;                       // ͨ����
} NET_OUT_CAP_SEQPOWER;

// ��ȡ�豸�������ö�Ӧ�����������
typedef struct tagNET_IN_ENCODE_CFG_CAPS
{
    DWORD               dwSize;           
    int                 nChannelId;                         // ͨ����    
    int                 nStreamType;                        // �������ͣ�0����������1��������1��2��������2��3��������3��4��ץͼ����
    char*               pchEncodeJson;                      // Encode���ã�ͨ������dhconfigsdk.dll�нӿ�CLIENT_PacketData��װ�õ�
                                                            // ��Ӧ�ķ�װ����Ϊ CFG_CMD_ENCODE                 
}NET_IN_ENCODE_CFG_CAPS;

// �������ö�Ӧ����
typedef struct tagNET_STREAM_CFG_CAPS
{
    DWORD               dwSize;
    int                 nAudioCompressionTypes[DH_MAX_AUDIO_ENCODE_TYPE]; // ֧�ֵ���Ƶ�������ͣ����DH_TALK_CODING_TYPE
    int                 nAudioCompressionTypeNum;                   // ��Ƶѹ����ʽ����
    int                 dwEncodeModeMask;                           // ��Ƶ����ģʽ���룬���"����ģʽ"
    DH_RESOLUTION_INFO  stuResolutionTypes[DH_MAX_CAPTURE_SIZE_NUM];// ֧�ֵ���Ƶ�ֱ���
    int                 nResolutionFPSMax[DH_MAX_CAPTURE_SIZE_NUM]; // ��ͬ�ֱ�����֡�����ֵ,�±���nResolutionTypes��Ӧ 
    int                 nResolutionTypeNum;                         // ��Ƶ�ֱ��ʸ���
    int                 nMaxBitRateOptions;                         // �����Ƶ����(kbps) 
    int                 nMinBitRateOptions;                         // ��С��Ƶ����(kbps)
    BYTE                bH264ProfileRank[DH_PROFILE_HIGH];          // ֧�ֵ�H.264 Profile�ȼ�, ����ö������ EM_H264_PROFILE_RANK;  
    int                 nH264ProfileRankNum;                        // ֧�ֵ�H.264 Profile�ȼ�����
    int                 nCifPFrameMaxSize;                          // ���ֱ���Ϊcifʱ���p֡(Kbps)
    int                 nCifPFrameMinSize;                          // ���ֱ���Ϊcifʱ��Сp֡(Kbps)
}NET_STREAM_CFG_CAPS;

// ��ȡ�豸�������ö�Ӧ�����������
typedef struct tagNET_OUT_ENCODE_CFG_CAPS
{
    DWORD               dwSize;
    NET_STREAM_CFG_CAPS stuMainFormatCaps[DH_REC_TYPE_NUM];         // ���������ö�Ӧ����
    NET_STREAM_CFG_CAPS stuExtraFormatCaps[DH_N_ENCODE_AUX];        // ���������ö�Ӧ����
    NET_STREAM_CFG_CAPS stuSnapFormatCaps[SNAP_TYP_NUM];            // ���������ö�Ӧ����
}NET_OUT_ENCODE_CFG_CAPS;

// �����¼�����
typedef struct tagALARM_FALLING_INFO
{
    DWORD               dwStructSize;                               // �ṹ���С
    BYTE                bEventAction;                               // �¼�������0��ʾ�����¼�,1��ʾ�������¼���ʼ,2��ʾ�������¼�����;
    BYTE                byRserved[3];                               // ����                                                                                                                                                                                                                                                                        
}ALARM_FALLING_INFO;

// ��չģ�鱨��ͨ����Ϣ
typedef struct tagNET_EXALARMCHANNELS_INFO
{
    DWORD               dwSize;
    int                 nExAlarmBoxNum;                             // ��չ������ͨ����
    int                 nChannelNum;                                // ��ͨ������չ�������ϵ�ͨ����
    char                szChannelName[DH_MAX_EXALARMCHANNEL_NAME_LEN];// ����ͨ������
} NET_EXALARMCHANNELS_INFO;

// CLIENT_QueryDevState �ӿ��������
typedef struct tagNET_EXALARMCHANNELS
{
    DWORD                       dwSize;
    int                         nExAlarmInCount;                    // ��չģ�鱨������ͨ�����������û�ָ����ѯ����
    int                         nRetExAlarmInCount;                 // ��չģ�鱨������ͨ�����ظ���
    NET_EXALARMCHANNELS_INFO*   pstuExAlarmInInfo;                  // ��չģ�鱨������ͨ����Ϣ

    int                         nExAlarmOutCount;                   // ��չģ�鱨�����ͨ�����������û�ָ����ѯ����
    int                         nRetExAlarmOutCount;                // ��չģ�鱨�����ͨ�����ظ���
    NET_EXALARMCHANNELS_INFO*   pstuExAlarmOutInfo;                 // ��չģ�鱨�����ͨ����Ϣ
} NET_EXALARMCHANNELS;

// ����ķ�����Ϣ
typedef struct tagNET_ACTIVATEDDEFENCEAREA_INFO
{
    DWORD                       dwSize;
    int                         nChannel;                           // ����ͨ����
    NET_TIME                    stuActivationTime;                  // ��������ʱ�� 
}NET_ACTIVATEDDEFENCEAREA_INFO;
// CLIENT_QueryDevState �ӿ��������
typedef struct tagNET_ACTIVATEDEFENCEAREA
{
    DWORD                       dwSize;
    int                         nAlarmInCount;                      // ��ѯ���ر�������ͨ��������������û�ָ������
    int                         nRetAlarmInCount;                   // ���ر�������ͨ��ʵ�ʼ������
    NET_ACTIVATEDDEFENCEAREA_INFO* pstuAlarmInDefenceAreaInfo;      // ���ر�������ͨ����Ϣ

    int                         nExAlarmInCount;                    // ��ѯ��չģ�鱨������ͨ�����������û�ָ������
    int                         nRetExAlarmInCount;                 // ��չģ�鱨������ͨ��ʵ�ʼ������
    NET_ACTIVATEDDEFENCEAREA_INFO* pstuExAlarmInDefenceAreaInfo;    // ��չģ�鱨������ͨ����Ϣ
}NET_ACTIVATEDDEFENCEAREA;

// �Ž�״̬����
typedef enum tagEM_NET_DOOR_STATUS_TYPE
{
    EM_NET_DOOR_STATUS_UNKNOWN,
    EM_NET_DOOR_STATUS_OPEN,
    EM_NET_DOOR_STATUS_CLOSE,
}EM_NET_DOOR_STATUS_TYPE;

// �Ž�״̬��Ϣ(CLIENT_QueryDevState �ӿ��������)
typedef struct tagNET_DOOR_STATUS_INFO
{
    DWORD                       dwSize;
    int                         nChannel;               // �Ž�ͨ����
    EM_NET_DOOR_STATUS_TYPE     emStateType;            // �Ž�״̬��Ϣ
}NET_DOOR_STATUS_INFO;

// CLIENT_QueryRecordCount�ӿ��������
typedef struct _NET_IN_QUEYT_RECORD_COUNT_PARAM
{
    DWORD                       dwSize;                 // �ṹ���С
    LLONG                       lFindeHandle;           // ��ѯ���
}NET_IN_QUEYT_RECORD_COUNT_PARAM;

// CLIENT_QueryRecordCount�ӿ��������
typedef struct _NET_OUT_QUEYT_RECORD_COUNT_PARAM
{
    DWORD                       dwSize;                 // �ṹ���С
    int                         nRecordCount;           // �豸���صļ�¼����
}NET_OUT_QUEYT_RECORD_COUNT_PARAM;

// ģ������������ͨ����Ϣ
typedef struct tagNET_ANALOGALARM_CHANNELS_INFO 
{
    DWORD               dwSize;
    int                 nSlot;                          // ����ַ, 0��ʾ����ͨ��, 1��ʾ�����ڵ�һ�������ϵ���չͨ��, 2��3...�Դ�����
    int                 nLevel1;                        // ��һ��������ַ, ��ʾ�����ڵ�nSlot�����ϵĵ�nLevel1��̽����, ��0��ʼ
    int                 nLevel2;                        // �ڶ���������ַ, ��ʾ�����ڵ�nLevel1�ڵ��ϵĵ�nLevel2��̽����,��0��ʼ,-1��ʾ�����ڸýڵ�,
    char                szName[DH_COMMON_STRING_128];   // ͨ������
}NET_ANALOGALARM_CHANNELS_INFO;

// ģ������������ͨ��ӳ���ϵ(��ӦDH_DEVSTATE_ANALOGALARM_CHANNELS����)
typedef struct tagNET_ANALOGALARM_CHANNELS 
{
    DWORD                          dwSize;
    int                            nMaxAnalogAlarmChannels; // ���ͨ����
    int                            nRetAnalogAlarmChannels; // ���ص�ͨ����
    NET_ANALOGALARM_CHANNELS_INFO* pstuChannelInfo;         // ͨ����Ϣ���û������ڴ�
}NET_ANALOGALARM_CHANNELS;

// ������������Ϣ
typedef struct tagNET_ANALOGALARM_SENSE_INFO 
{
    DWORD                   dwSize;
    int                     nChannelID;                     // ͨ����(��0��ʼ)
    NET_SENSE_METHOD        emSense;                        // ����������
    float                   fData;                          // ��������ֵ
    NET_TIME                stuTime;                        // �ɼ�ʱ��
    int                     nStatus;                        // ����״̬, -1:δ֪,0:����,1:������Ч(��������),
                                                            // 2:������ֵ1,3:������ֵ2,4:������ֵ3,5:������ֵ4,
                                                            // 6:������ֵ1,7:������ֵ2,8:������ֵ3,9:������ֵ4
}NET_ANALOGALARM_SENSE_INFO;

//����ģ��������ͨ�����ݻص�����ԭ��
typedef void (CALLBACK *fAnalogAlarmDataCallBack)(LLONG lLoginID, LLONG lAttachHandle, NET_ANALOGALARM_SENSE_INFO* pInfo, int nBufLen, LDWORD dwUser);

// CLIENT_AttachAnalogAlarmData()�ӿ��������
typedef struct tagNET_IN_ANALOGALARM_DATA 
{
    DWORD                       dwSize;
    int                         nChannelId;                 // ��0��ʼ, -1��ʾȫ��ͨ��
    fAnalogAlarmDataCallBack    cbCallBack;                 // ���ݻص�����
    LDWORD                      dwUser;                     // �û��������
}NET_IN_ANALOGALARM_DATA;

// CLIENT_AttachAnalogAlarmData()�ӿ��������
typedef struct tagNET_OUT_ANALOGALARM_DATA 
{
    DWORD    dwSize;
}NET_OUT_ANALOGALARM_DATA;


// ��ѯ�豸֧�ֵĴ������豸���� 
// CLIENT_QueryDevState�ӿڵ� DH_DEVSTATE_GET_SENSORLIST  �������
#define MAX_SUPPORT_SENSORTYPE_NUM    128                   // ���֧�ִ������豸���͸���

typedef struct tagNET_SENSOR_LIST 
{
    DWORD       dwSize;
    int         nSupportSensorNum;                          // ����֧�ִ������豸���͸���
    char        szSensorList[MAX_SUPPORT_SENSORTYPE_NUM][DH_COMMON_STRING_64];
}NET_SENSOR_LIST;

// CLIENT_QueryDevLogCount��ȡ��־�����������
typedef struct tagNET_IN_GETCOUNT_LOG_PARAM
{
    DWORD                       dwSize;
    QUERY_DEVICE_LOG_PARAM      stuQueryCondition;          // ��ѯ��¼������
} NET_IN_GETCOUNT_LOG_PARAM;

// CLIENT_QueryDevLogCount��ȡ��־�����������
typedef struct tagNET_OUT_GETCOUNT_LOG_PARAM
{
    DWORD                       dwSize;
    int                         nLogCount;                  // ��־��(�豸����)
} NET_OUT_GETCOUNT_LOG_PARAM;


// ��ȡ��ǰ��ϵͳ����״̬(��ӦDH_DEVSTATE_GET_ALARM_SUBSYSTEM_ACTIVATESTATUS����)
typedef struct tagNET_GET_ALARM_SUBSYSTEM_ACTIVATE_STATUES
{
	DWORD				dwSize;
	int					nChannelId;			// ��ϵͳ��
	BOOL				bActive;			// ��ϵͳ����״̬, TRUE ��ʾ����,FALSE��ʾ������
}NET_GET_ALARM_SUBSYSTEM_ACTIVATE_STATUES;

// ������ϵͳ�������ò���(��ӦDH_CTRL_ALARM_SUBSYSTEM_ACTIVE_SET����)
typedef struct tagNET_CTRL_ALARM_SUBSYSTEM_SETACTIVE 
{
	DWORD				dwSize;
	int					nChannelId;			// ��ϵͳ��
	BOOL				bActive;			// ��ϵͳ����״̬,TRUE ��ʾ����,FALSE��ʾ������
}NET_CTRL_ALARM_SUBSYSTEM_SETACTIVE;

////////////////////////////////ϵͳ������//////////////////////////////////////////
// ����״̬
typedef struct __NET_PARTITION_STATE
{
    DWORD             dwSize;
    int               nStatus;                  // ����״̬��0-������1-���� 
    double            dbTotalSize;              // �������������ֽ�Ϊ��λ
    double            dbRemainSize;             // ʣ���������ֽ�Ϊ��λ
    
}NET_PARTITION_STATE;

// Ӳ��״̬
typedef struct __NET_HDD_STATE
{
    DWORD             dwSize; 
    int               nState;                   // Ӳ��״̬��0-������1-����   
    double            dbTotalSize;              // Ӳ�����������ֽ�Ϊ��λ
    NET_PARTITION_STATE stuPartitions[DH_MAX_STORAGE_PARTITION_NUM]; // ����״̬
    int               nPartitionNum;              // ������
}NET_HDD_STATE;

// ͨ��״̬
typedef struct __NET_CHANNLE_STATE
{
    DWORD             dwSize;
    BYTE              byRecState;               // ¼��״̬��1-¼��0-�ر�
    BYTE              byVideoInState;           // ��Ƶ����״̬��1-����Ƶ���룬0-����Ƶ  
    BYTE              byReserved[2];            // �ֽڶ���
}NET_CHANNLE_STATE;

// �豸�Լ���Ϣ
typedef struct __NET_SELFCHECK_INFO
{
    DWORD             dwSize;
    int               nAlarmIn;                 // ��������ͨ����
    int               nAlarmOut;                // �������ͨ����  
    NET_TIME          stuTime;                  // �ϱ�ʱ��
    char              szPlateNo[DH_MAX_PLATE_NUMBER_LEN]; // ����
    char              szICCID[DH_MAX_SIM_LEN];  // SIM����
    BYTE              byOrientation;            // ��λ״̬��0-δ��λ��1-��λ 
    BYTE              byACCState;               // ACC ״̬��0-�رգ�1-��
    BYTE              byConstantElecState;      // ����״̬��0-�������ӣ�1-�Ͽ���2-Ƿѹ��3-��ѹ
    BYTE              byAntennaState;           // ͨ���ź�״̬��0-������1-δ֪���ϣ�2-δ�ӣ�3-��·
    
    // �ⲿ�豸״̬
    BYTE              byReportStation;          // ��վ��״̬��0-δ�ӣ�1-������2-�쳣
    BYTE              byControlScreen;          // ������״̬��0-δ�ӣ�1-������2-�쳣
    BYTE              byPOS;                    // POS��״̬��0-δ�ӣ�1-������2-�쳣
    BYTE              byCoinBox;                // Ͷ����״̬��0-δ�ӣ�1-������2-�쳣
    
    // ������
    BOOL              bTimerSnap;               // ��ʱץͼ��TRUE-֧�֣�FALSE-��֧��
    BOOL              bElectronEnclosure;       // ����Χ����TRUE-֧�֣�FALSE-��֧��
    BOOL              bTeleUpgrade;             // Զ��������TRUE-֧�֣�FALSE-��֧��   
    
    NET_HDD_STATE     stuHddStates[DH_MAX_DISKNUM]; // Ӳ��״̬
    int               nHddNum;                  // Ӳ�̸���
    
    NET_CHANNLE_STATE* pChannleState;           // ͨ��״̬����һ�����飬 �ڴ���sdk�ڲ����룬�ͷ�Ҳ��sdk�ڲ��ͷ�
    int               nChannleNum;              // ͨ������
}NET_SELFCHECK_INFO;

typedef void (CALLBACK *fMissionInfoCallBack)(LLONG lAttachHandle, DWORD dwType, void* pMissionInfo, void* pReserved, LDWORD dwUserData);

typedef enum EM_MISSION_TYPE
{
    NET_MISSION_TYPE_UNKOWN,                    // δ֪
        NET_MISSION_TYPE_SELFCHECK,                 // �豸�Լ죬��Ӧ�ṹ�� NET_SELFCHECK_INFO
}EM_MISSION_TYPE;

//CLIENT_AttachMission�ӿ��������
typedef struct __NET_IN_ATTACH_MISSION_PARAM
{
    DWORD             dwSize;
    EM_MISSION_TYPE   emMissionType;            // ��������
    fMissionInfoCallBack cbMissionInfofunc;     // ������Ϣ�ص�����
    LDWORD            dwUser;                   // �û����� 
}NET_IN_ATTACH_MISSION_PARAM;

// CLIENT_DetachMission�ӿ��������
typedef struct NET_OUT_ATTACH_MISSION_PARAM
{
    DWORD             dwSize;
    LLONG             lAttachHandle;           // ���ľ��
}NET_OUT_ATTACH_MISSION_PARAM;

/***********************************************************************
 ** Callback Function Definition 
 ***********************************************************************/

// Network disconnection callback function original shape 
typedef void (CALLBACK *fDisConnect)(LLONG lLoginID, char *pchDVRIP, LONG nDVRPort, LDWORD dwUser);

// network re-connection callback function original shape
typedef void (CALLBACK *fHaveReConnect)(LLONG lLoginID, char *pchDVRIP, LONG nDVRPort, LDWORD dwUser);

// The prototype of dynamic sub connection offline call function 
typedef void (CALLBACK *fSubDisConnect)(EM_INTERFACE_TYPE emInterfaceType, BOOL bOnline, LLONG lOperateHandle, LLONG lLoginID, LDWORD dwUser);

// monitor disconnect callback function
typedef void (CALLBACK *fRealPlayDisConnect)(LLONG lOperateHandle, EM_REALPLAY_DISCONNECT_EVENT_TYPE dwEventType, void* param, LDWORD dwUser);

// Real-time monitor data callback function original shape
typedef void (CALLBACK *fRealDataCallBack)(LLONG lRealHandle, DWORD dwDataType, BYTE *pBuffer, DWORD dwBufSize, LDWORD dwUser);

// Real-time monitor data callback function original shape---extensive
typedef void (CALLBACK *fRealDataCallBackEx)(LLONG lRealHandle, DWORD dwDataType, BYTE *pBuffer, DWORD dwBufSize, LONG param, LDWORD dwUser);

// OSD callback function original shape 
typedef void (CALLBACK *fDrawCallBack)(LLONG lLoginID, LLONG lPlayHandle, HDC hDC, LDWORD dwUser);

// Playback process callback function original shape 
typedef void (CALLBACK *fDownLoadPosCallBack)(LLONG lPlayHandle, DWORD dwTotalSize, DWORD dwDownLoadSize, LDWORD dwUser);

// Playback process by time callback function original shape
typedef void (CALLBACK *fTimeDownLoadPosCallBack) (LLONG lPlayHandle, DWORD dwTotalSize, DWORD dwDownLoadSize, int index, NET_RECORDFILE_INFO recordfileinfo, LDWORD dwUser);

// Alarm message callback function original shape
typedef BOOL (CALLBACK *fMessCallBack)(LONG lCommand, LLONG lLoginID, char *pBuf, DWORD dwBufLen, char *pchDVRIP, LONG nDVRPort, LDWORD dwUser);

// Listening server callback function original shape
typedef int (CALLBACK *fServiceCallBack)(LLONG lHandle, char *pIp, WORD wPort, LONG lCommand, void *pParam, DWORD dwParamLen, LDWORD dwUserData);

// Audio data of audio talk callback function original shape 
typedef void (CALLBACK *pfAudioDataCallBack)(LLONG lTalkHandle, char *pDataBuf, DWORD dwBufSize, BYTE byAudioFlag, LDWORD dwUser);

// Upgrade device callback function original shape
typedef void (CALLBACK *fUpgradeCallBack) (LLONG lLoginID, LLONG lUpgradechannel, int nTotalSize, int nSendSize, LDWORD dwUser);

// Transparent COM callback function original shape
typedef void (CALLBACK *fTransComCallBack) (LLONG lLoginID, LLONG lTransComChannel, char *pBuffer, DWORD dwBufSize, LDWORD dwUser);

// Search device log data callback function original shape 
typedef void (CALLBACK *fLogDataCallBack)(LLONG lLoginID, char *pBuffer, DWORD dwBufSize, DWORD nTotalSize, BOOL bEnd, LDWORD dwUser);

// Snapshot callback function original shape 
typedef void (CALLBACK *fSnapRev)(LLONG lLoginID, BYTE *pBuf, UINT RevLen, UINT EncodeType, DWORD CmdSerial, LDWORD dwUser);

// GPS message subscription callback 
typedef void( CALLBACK *fGPSRev)(LLONG lLoginID, GPS_Info GpsInfo, LDWORD dwUserData);

// GPSGPS message subscription callback - extra
typedef void (CALLBACK *fGPSRevEx)(LLONG lLoginID, GPS_Info GpsInfo, ALARM_STATE_INFO stAlarmInfo, LDWORD dwUserData, void *reserved);

// GPS GPS subscription of temperature and humidity callback
typedef void (CALLBACK *fGPSTempHumidityRev)(LLONG lLoginID, GPS_TEMP_HUMIDITY_INFO GpsTHInfo, LDWORD dwUserData);

// Asynchronous data callback
typedef void (CALLBACK *fMessDataCallBack)(LLONG lCommand, LPNET_CALLBACK_DATA lpData, LDWORD dwUser);

// File Transfer callback
typedef void (CALLBACK *fTransFileCallBack)(LLONG lHandle, int nTransType, int nState, int nSendSize, int nTotalSize, LDWORD dwUser);

// intelligent analysis data callback;nSequence instruct the repeat picture's station,0 instruct the first time it appear, 2 instruct the last time it appear or it only appear once,1 instruct it will appear next time
// int nState = (int) reserved instruct current callback data's state, 0 means realtime data,1 means offline data,2 means send offline data over
typedef int  (CALLBACK *fAnalyzerDataCallBack)(LLONG lAnalyzerHandle, DWORD dwAlarmType, void* pAlarmInfo, BYTE *pBuffer, DWORD dwBufSize, LDWORD dwUser, int nSequence, void *reserved);

// Asynchronism search device call
typedef void (CALLBACK *fSearchDevicesCB)(DEVICE_NET_INFO_EX *pDevNetInfo, void* pUserData);

// Asynchronism register device call
typedef void (CALLBACK *fHaveLogin)(LLONG lLoginID, char *pchDVRIP, LONG nDVRPort, BOOL bOnline, NET_DEVICEINFO_Ex stuDeviceInfo, int nError, LDWORD dwUser, void *reserved);

// playback YUV callback function
typedef void (CALLBACK *fYUVDataCallBack)(LLONG lPlaybackHandle, BYTE *pBuffer, DWORD dwBufSize, LONG param, LDWORD dwUser, void *reserved);
/************************************************************************
 ** Interface Definition 
 ***********************************************************************/

// SDK Initialization 
CLIENT_API BOOL CALL_METHOD CLIENT_Init(fDisConnect cbDisConnect, LDWORD dwUser);

// SDK exit and clear
CLIENT_API void CALL_METHOD CLIENT_Cleanup();

//------------------------------------------------------------------------

// Set re-connection callback function after disconnection. Internal SDK  auto connect again after disconnection 
CLIENT_API void CALL_METHOD CLIENT_SetAutoReconnect(fHaveReConnect cbAutoConnect, LDWORD dwUser);

// Dynamic sub-set link disconnected callback function, the current monitoring and playback equipment SVR is a short connection
CLIENT_API void CALL_METHOD CLIENT_SetSubconnCallBack(fSubDisConnect cbSubDisConnect, LDWORD dwUser);

// Return the function execution failure code
CLIENT_API DWORD CALL_METHOD CLIENT_GetLastError(void);

// Set device connection timeout value and trial times 
CLIENT_API void CALL_METHOD CLIENT_SetConnectTime(int nWaitTime, int nTryTimes);

// Set log in network environment 
CLIENT_API void CALL_METHOD CLIENT_SetNetworkParam(NET_PARAM *pNetParam);

// Get SDK version information 
CLIENT_API DWORD CALL_METHOD CLIENT_GetSDKVersion();

//------------------------------------------------------------------------

// Register to the device 
CLIENT_API LLONG CALL_METHOD CLIENT_Login(char *pchDVRIP, WORD wDVRPort, char *pchUserName, char *pchPassword, LPNET_DEVICEINFO lpDeviceInfo, int *error = 0);

// Expension interfacenSpecCap =0 is login under TCP mode, nSpecCap =2 is login with active registeration, nSpecCap =3 is login under multicast mode,
//			 nSpecCap =4 is login under UDP mode, nSpecCap =6 is login with main connection,nSpecCap =7 is SSL encription
//			  ,nSpecCap =8 is Chengdu Jiafa login
//			 nSpecCap=9 is login in to remote device, enter void* pCapParam as remote device name
//          nSpecCap = 12 is login under LDAP mode
//          nSpecCap = 13 is login under AD mode
//           nSpecCap = 14ΪRadius��¼��ʽ 
//             nSpecCap = 15ΪSocks5��½��ʽ�����ʱ��void* pCapParam����Socks5��������IP&&port&&ServerName&&ServerPassword�ַ���
//             nSpecCap = 16Ϊ������½��ʽ�����ʱ��void* pCapParam����SOCKETֵ
//             nSpecCap = 19Ϊweb˽����͸��½��ʽ
//             nSpecCap = 20Ϊ�ֻ��ͻ��˵���
CLIENT_API LLONG CALL_METHOD CLIENT_LoginEx(char *pchDVRIP, WORD wDVRPort, char *pchUserName, char *pchPassword, int nSpecCap, void* pCapParam, LPNET_DEVICEINFO lpDeviceInfo, int *error = 0);

// Asynchronism login device 
// nSpecCap = 0 TCP login,nSpecCap = 6 only main connect login
// nSpecCap = 19Ϊ˽����͸��¼
CLIENT_API LLONG CALL_METHOD CLIENT_StartLogin(char *pchDVRIP, WORD wDVRPort, char *pchUserName, char *pchPassword, int nSpecCap, void* pCapParam, fHaveLogin cbLogin, LDWORD dwUser);

// stop login
CLIENT_API BOOL CALL_METHOD CLIENT_StopLogin(LLONG lLoginID);

// Log out the device 
CLIENT_API BOOL CALL_METHOD CLIENT_Logout(LLONG lLoginID);


//------------------------------------------------------------------------

// Begin real-time monitor 
CLIENT_API LLONG CALL_METHOD CLIENT_RealPlay(LLONG lLoginID, int nChannelID, HWND hWnd);

// Begin real-time monitor--extensive
CLIENT_API LLONG CALL_METHOD CLIENT_RealPlayEx(LLONG lLoginID, int nChannelID, HWND hWnd, DH_RealPlayType rType = DH_RType_Realplay);

//start real-time monitor
CLIENT_API LLONG CALL_METHOD CLIENT_StartRealPlay(LLONG lLoginID, int nChannelID, HWND hWnd, DH_RealPlayType rType, fRealDataCallBackEx cbRealData, fRealPlayDisConnect cbDisconnect, LDWORD dwUser, DWORD dwWaitTime = 10000);

// Multiple-window preview 
CLIENT_API LLONG CALL_METHOD CLIENT_MultiPlay(LLONG lLoginID, HWND hWnd);

// Stop multiple-window preview 
CLIENT_API BOOL CALL_METHOD CLIENT_StopMultiPlay(LLONG lMultiHandle);

// Snapshot;hPlayHandle is monitor or playback handle
CLIENT_API BOOL CALL_METHOD CLIENT_CapturePicture(LLONG hPlayHandle, const char *pchPicFileName);
CLIENT_API BOOL CALL_METHOD CLIENT_CapturePictureEx(LLONG hPlayHandle, const char *pchPicFileName, NET_CAPTURE_FORMATS eFormat);

// Set real-time monitor data callback 
CLIENT_API BOOL CALL_METHOD CLIENT_SetRealDataCallBack(LLONG lRealHandle, fRealDataCallBack cbRealData, LDWORD dwUser);

// Set real-time monitor data callback--extensive 
CLIENT_API BOOL CALL_METHOD CLIENT_SetRealDataCallBackEx(LLONG lRealHandle, fRealDataCallBackEx cbRealData, LDWORD dwUser, DWORD dwFlag);

// Set video fluency
CLIENT_API BOOL	CALL_METHOD CLIENT_AdjustFluency(LLONG lRealHandle, int nLevel);

// Save data as file
CLIENT_API BOOL CALL_METHOD CLIENT_SaveRealData(LLONG lRealHandle, const char *pchFileName);

// Stop saving data as file 
CLIENT_API BOOL CALL_METHOD CLIENT_StopSaveRealData(LLONG lRealHandle);

// Set video property
CLIENT_API BOOL CALL_METHOD CLIENT_ClientSetVideoEffect(LLONG lPlayHandle, unsigned char nBrightness, unsigned char nContrast, unsigned char nHue, unsigned char nSaturation);

// Get video property 
CLIENT_API BOOL CALL_METHOD CLIENT_ClientGetVideoEffect(LLONG lPlayHandle, unsigned char *nBrightness, unsigned char *nContrast, unsigned char *nHue, unsigned char *nSaturation);

// Set screen overlay callback 
CLIENT_API void CALL_METHOD CLIENT_RigisterDrawFun(fDrawCallBack cbDraw, LDWORD dwUser);

// Open audio 
CLIENT_API BOOL CALL_METHOD CLIENT_OpenSound(LLONG hPlayHandle);

// Set volume;lPlayHandle is monitor handle or playabck handle 
CLIENT_API BOOL CALL_METHOD CLIENT_SetVolume(LLONG lPlayHandle, int nVolume);

// �Ƿ����ø���ͼ���ڲ��������ԣ�Ĭ�����á��ò�������ʱ���ڿ��4��������ʱ��ֻ����I֡��������ʱ����ÿ֡������
CLIENT_API BOOL CALL_METHOD CLIENT_PlayEnableLargePicAdjustment(LLONG lPlayHandle, BOOL bEnable);

// Stop audio 
CLIENT_API BOOL CALL_METHOD CLIENT_CloseSound();

// Stop real-time preview 
CLIENT_API BOOL CALL_METHOD CLIENT_StopRealPlay(LLONG lRealHandle);

// stop real-time preview-extensive
CLIENT_API BOOL CALL_METHOD CLIENT_StopRealPlayEx(LLONG lRealHandle);

//------------------------------------------------------------------------

// general PTZ control
CLIENT_API BOOL CALL_METHOD CLIENT_PTZControl(LLONG lLoginID, int nChannelID, DWORD dwPTZCommand, DWORD dwStep, BOOL dwStop);

// Private PTZ control 
CLIENT_API BOOL CALL_METHOD CLIENT_DHPTZControl(LLONG lLoginID, int nChannelID, DWORD dwPTZCommand, unsigned char param1, unsigned char param2, unsigned char param3, BOOL dwStop,void* param4=NULL);

//  private PTZ control extensive port. support 3D intelligent position
CLIENT_API BOOL CALL_METHOD CLIENT_DHPTZControlEx(LLONG lLoginID, int nChannelID, DWORD dwPTZCommand, LONG lParam1, LONG lParam2, LONG lParam3, BOOL dwStop);

//------------------------------------------------------------------------
// query record state everyday in the month
CLIENT_API BOOL CALL_METHOD CLIENT_QueryRecordStatus(LLONG lLoginID, int nChannelId, int nRecordFileType, LPNET_TIME tmMonth, char* pchCardid, LPNET_RECORD_STATUS pRecordStatus, int waittime=1000);

// Search whether there is recorded file in specified period
CLIENT_API BOOL CALL_METHOD CLIENT_QueryRecordTime(LLONG lLoginID, int nChannelId, int nRecordFileType, LPNET_TIME tmStart, LPNET_TIME tmEnd, char* pchCardid, BOOL *bResult, int waittime=1000);

// Search all recorded file sin the specified periods
CLIENT_API BOOL CALL_METHOD CLIENT_QueryRecordFile(LLONG lLoginID, int nChannelId, int nRecordFileType, LPNET_TIME tmStart, LPNET_TIME tmEnd, char* pchCardid, LPNET_RECORDFILE_INFO nriFileinfo, int maxlen, int *filecount, int waittime=1000, BOOL bTime = FALSE);

// �첽��ѯʱ����ڵ�����¼���ļ�
CLIENT_API BOOL CALL_METHOD CLIENT_StartQueryRecordFile(LLONG lLoginID, NET_IN_START_QUERY_RECORDFILE *pInParam, NET_OUT_START_QUERY_RECORDFILE *pOutParam);

// search first 16 records
CLIENT_API BOOL CALL_METHOD CLIENT_QuickQueryRecordFile(LLONG lLoginID, int nChannelId, int nRecordFileType, LPNET_TIME tmStart, LPNET_TIME tmEnd, char* pchCardid, LPNET_RECORDFILE_INFO nriFileinfo, int maxlen, int *filecount, int waittime=1000, BOOL bTime = FALSE);


// Query the first record time 
CLIENT_API BOOL CALL_METHOD CLIENT_QueryFurthestRecordTime(LLONG lLoginID, int nRecordFileType, char *pchCardid, NET_FURTHEST_RECORD_TIME* pFurthrestTime, int nWaitTime);

// Begin searching recorded file
CLIENT_API LLONG	CALL_METHOD CLIENT_FindFile(LLONG lLoginID, int nChannelId, int nRecordFileType, char* cardid, LPNET_TIME time_start, LPNET_TIME time_end, BOOL bTime, int waittime);

// Search recorded file 
CLIENT_API int	CALL_METHOD CLIENT_FindNextFile(LLONG lFindHandle,LPNET_RECORDFILE_INFO lpFindData);

// Stop searching recorded file 
CLIENT_API BOOL CALL_METHOD CLIENT_FindClose(LLONG lFindHandle);

// Playback in file 
CLIENT_API LLONG CALL_METHOD CLIENT_PlayBackByRecordFile(LLONG lLoginID, LPNET_RECORDFILE_INFO lpRecordFile, HWND hWnd, fDownLoadPosCallBack cbDownLoadPos, LDWORD dwUserData);

// Playback in file--extensive
CLIENT_API LLONG CALL_METHOD CLIENT_PlayBackByRecordFileEx(LLONG lLoginID, LPNET_RECORDFILE_INFO lpRecordFile, HWND hWnd, fDownLoadPosCallBack cbDownLoadPos, LDWORD dwPosUser, fDataCallBack fDownLoadDataCallBack, LDWORD dwDataUser);

// playback in file - disconnect callback
CLIENT_API LLONG CALL_METHOD CLIENT_StartPlayBackByRecordFile(LLONG lLoginID,  LPNET_RECORDFILE_INFO lpRecordFile, HWND hWnd, 
															 fDownLoadPosCallBack cbDownLoadPos, LDWORD dwPosUser, 
															 fDataCallBack fDownLoadDataCallBack, LDWORD dwDataUser, 
															 fRealPlayDisConnect fDisConnectCallBack, LDWORD dwDisUser, DWORD dwWaitTime = 10000);

//Playback in file--Drop frame playback (not enough bandwidth can be used)
CLIENT_API LLONG CALL_METHOD CLIENT_FramCotrolPlayBackByRecordFile(LLONG lLoginID, LPNET_RECORDFILE_INFO lpRecordFile, HWND hWnd, 
																   fDownLoadPosCallBack cbDownLoadPos, LDWORD dwPosUser, 
																   fDataCallBack fDownLoadDataCallBack, LDWORD dwDataUser, unsigned int nCutFrameRate);

// Playback in time
CLIENT_API LLONG CALL_METHOD CLIENT_PlayBackByTime(LLONG lLoginID, int nChannelID, LPNET_TIME lpStartTime, LPNET_TIME lpStopTime, HWND hWnd, fDownLoadPosCallBack cbDownLoadPos, LDWORD dwPosUser);

// Playback in time--extensive
CLIENT_API LLONG CALL_METHOD CLIENT_PlayBackByTimeEx(LLONG lLoginID, int nChannelID, LPNET_TIME lpStartTime, LPNET_TIME lpStopTime, HWND hWnd, 
													 fDownLoadPosCallBack cbDownLoadPos, LDWORD dwPosUser, 
													 fDataCallBack fDownLoadDataCallBack, LDWORD dwDataUser);

// playback in time--disconnect callback
CLIENT_API LLONG CALL_METHOD CLIENT_StartPlayBackByTime(LLONG lLoginID, int nChannelID, 
													   LPNET_TIME lpStartTime, LPNET_TIME lpStopTime, HWND hWnd, 
													   fDownLoadPosCallBack cbDownLoadPos, LDWORD dwPosUser,
													   fDataCallBack fDownLoadDataCallBack, LDWORD dwDataUser, 
													   fRealPlayDisConnect fDisConnectCallBack, LDWORD dwDisUser, DWORD dwWaitTime = 10000);

//Playback in time--Drop frame playback (not enough bandwidth can be used)
CLIENT_API LLONG CALL_METHOD CLIENT_FramCotrolPlayBackByTime(LLONG lLoginID, int nChannelID, LPNET_TIME lpStartTime, LPNET_TIME lpStopTime, HWND hWnd, 
															 fDownLoadPosCallBack cbDownLoadPos, LDWORD dwPosUser, 
															 fDataCallBack fDownLoadDataCallBack, LDWORD dwDataUser, unsigned int nCutFrameRate);

//playback by synopsis file
CLIENT_API LLONG CALL_METHOD CLIENT_PlayBackBySynopsisFile(LLONG lLoginID, 
															LPNET_SYNOPSISFILE_INFO lpRecordFile, 
															HWND hWnd, 
															fDownLoadPosCallBack cbDownLoadPos,
															LDWORD dwPosUser, 
															fDataCallBack fDownLoadDataCallBack,
															LDWORD dwDataUser,
															LDWORD dwUser);

// ��ͨ��Ԥ���ط�
CLIENT_API LLONG CALL_METHOD CLIENT_MultiPlayBack(LLONG lLoginID, NET_MULTI_PLAYBACK_PARAM *pParam);

// ��λ¼��ط���ʼ��
CLIENT_API BOOL CALL_METHOD CLIENT_SeekPlayBack(LLONG lPlayHandle, unsigned int offsettime, unsigned int offsetbyte);

// Pause or restore file playback 
CLIENT_API BOOL CALL_METHOD CLIENT_PausePlayBack(LLONG lPlayHandle, BOOL bPause);

// Fast playback file 
CLIENT_API BOOL CALL_METHOD CLIENT_FastPlayBack(LLONG lPlayHandle);

// Slow playback file 
CLIENT_API BOOL CALL_METHOD CLIENT_SlowPlayBack(LLONG lPlayHandle);

// Step playback file 
CLIENT_API BOOL CALL_METHOD CLIENT_StepPlayBack(LLONG lPlayHandle, BOOL bStop);

// Control playback direction--Forward or Backward playback
CLIENT_API BOOL CALL_METHOD CLIENT_PlayBackControlDirection(LLONG lPlayHandle, BOOL bBackward);

// Set playback frame rate 
CLIENT_API BOOL CALL_METHOD CLIENT_SetFramePlayBack(LLONG lPlayHandle, int framerate);

// Get playback frame rate
CLIENT_API BOOL CALL_METHOD CLIENT_GetFramePlayBack(LLONG lPlayHandle, int *fileframerate, int *playframerate);

// Restore ordinary playback
CLIENT_API BOOL CALL_METHOD CLIENT_NormalPlayBack(LLONG lPlayHandle);

// smart search play back
CLIENT_API BOOL CALL_METHOD CLIENT_SmartSearchPlayBack(LLONG lPlayHandle, LPIntelligentSearchPlay lpPlayBackParam);

// Get playback OSD time 
CLIENT_API BOOL CALL_METHOD CLIENT_GetPlayBackOsdTime(LLONG lPlayHandle, LPNET_TIME lpOsdTime, LPNET_TIME lpStartTime, LPNET_TIME lpEndTime);

// Stop file playback 
CLIENT_API BOOL CALL_METHOD CLIENT_StopPlayBack(LLONG lPlayHandle);

// Download recorded file 
CLIENT_API LLONG CALL_METHOD CLIENT_DownloadByRecordFile(LLONG lLoginID,LPNET_RECORDFILE_INFO lpRecordFile, char *sSavedFileName, fDownLoadPosCallBack cbDownLoadPos, LDWORD dwUserData);

// Download file by time 
CLIENT_API LLONG CALL_METHOD CLIENT_DownloadByTime(LLONG lLoginID, int nChannelId, int nRecordFileType, LPNET_TIME tmStart, LPNET_TIME tmEnd, char *sSavedFileName, fTimeDownLoadPosCallBack cbTimeDownLoadPos, LDWORD dwUserData);

// Search record download process 
CLIENT_API BOOL CALL_METHOD CLIENT_GetDownloadPos(LLONG lFileHandle, int *nTotalSize, int *nDownLoadSize);

// Stop record download 
CLIENT_API BOOL CALL_METHOD CLIENT_StopDownload(LLONG lFileHandle);

//partial enlarged
CLIENT_API BOOL CALL_METHOD CLIENT_SetDisplayRegion(LLONG lPlayHandle,DWORD nRegionNum, DH_DISPLAYRREGION *pSrcRect, HWND hDestWnd, BOOL bEnable);

// ��ʼ����¼���ļ�֡��Ϣ
CLIENT_API BOOL    CALL_METHOD CLIENT_FindFrameInfo(LLONG lLoginID, NET_IN_FIND_FRAMEINFO_PRAM *pInParam, NET_OUT_FIND_FRAMEINFO_PRAM* pOutParam, int nWaitTime);

// ����¼���ļ�֡��Ϣ����ָ����Ϣ������ѯ
CLIENT_API BOOL    CALL_METHOD CLIENT_FindNextFrameInfo(LLONG lFindHandle, NET_IN_FINDNEXT_FRAMEINFO_PRAM *pInParam, NET_OUT_FINDNEXT_FRAMEINFO_PRAM* pOutParam, int nWaitTime);

// ����¼���ļ�����
CLIENT_API BOOL CALL_METHOD CLIENT_FindFrameInfoClose(LLONG lFindHandle);

//------------------------------------------------------------------------

// Set alarm callback function 
CLIENT_API void CALL_METHOD CLIENT_SetDVRMessCallBack(fMessCallBack cbMessage,LDWORD dwUser);

// subscribe alarm 
CLIENT_API BOOL CALL_METHOD CLIENT_StartListen(LLONG lLoginID);

// subscribe alarm---extensive
CLIENT_API BOOL CALL_METHOD CLIENT_StartListenEx(LLONG lLoginID);

// Stop subscribe alarm 
CLIENT_API BOOL CALL_METHOD CLIENT_StopListen(LLONG lLoginID);

// alarm reset
CLIENT_API BOOL CALL_METHOD CLIENT_AlarmReset(LLONG lLoginID, DWORD dwAlarmType, int nChannel, void* pReserved = NULL, int nWaitTime = 1000);
//------------------------------------------------------------------------

// actively registration function. enable service. nTimeout is invalid. 
CLIENT_API LLONG CALL_METHOD CLIENT_ListenServer(char* ip, WORD port, int nTimeout, fServiceCallBack cbListen, LDWORD dwUserData);

// stop service
CLIENT_API BOOL CALL_METHOD CLIENT_StopListenServer(LLONG lServerHandle);

// Respond the registration requestion from the device 
CLIENT_API BOOL CALL_METHOD CLIENT_ResponseDevReg(char *devSerial, char* ip, WORD port, BOOL bAccept);

//------------------------------------------------------------------------

// Alarm upload function. Enable service. dwTimeOut paramter is invalid 
CLIENT_API LLONG CALL_METHOD CLIENT_StartService(WORD wPort, char *pIp = NULL, fServiceCallBack pfscb = NULL, DWORD dwTimeOut = 0xffffffff, LDWORD dwUserData = 0);

// Stop service 
CLIENT_API BOOL CALL_METHOD CLIENT_StopService(LLONG lHandle);

//------------------------------------------------------------------------

// Set audio talk mode(client-end mode or server mode)
CLIENT_API BOOL CALL_METHOD CLIENT_SetDeviceMode(LLONG lLoginID, EM_USEDEV_MODE emType, void* pValue);

// Enable audio talk 
CLIENT_API LLONG CALL_METHOD CLIENT_StartTalkEx(LLONG lLoginID, pfAudioDataCallBack pfcb, LDWORD dwUser);

// Begin PC record 
CLIENT_API BOOL CALL_METHOD CLIENT_RecordStart();

// Stop PC record 
CLIENT_API BOOL CALL_METHOD CLIENT_RecordStop();


// ��ʼPC��¼��(��CLIENT_RecordStart()��չ)
CLIENT_API BOOL CALL_METHOD CLIENT_RecordStartEx(LLONG lLoginID);

// ����PC��¼��(��CLIENT_RecordStop()��չ)
CLIENT_API BOOL CALL_METHOD CLIENT_RecordStopEx(LLONG lLoginID);


// Send out audio data to the device 
CLIENT_API LONG CALL_METHOD CLIENT_TalkSendData(LLONG lTalkHandle, char *pSendBuf, DWORD dwBufSize);

// Decode audio data 
CLIENT_API void CALL_METHOD CLIENT_AudioDec(char *pAudioDataBuf, DWORD dwBufSize);
CLIENT_API BOOL CALL_METHOD CLIENT_AudioDecEx(LLONG lTalkHandle, char *pAudioDataBuf, DWORD dwBufSize);

// Set audio talk volume
CLIENT_API BOOL CALL_METHOD CLIENT_SetAudioClientVolume(LLONG lTalkHandle, WORD wVolume);

// Stop audio talk 
CLIENT_API BOOL CALL_METHOD CLIENT_StopTalkEx(LLONG lTalkHandle);

// add device into broadcast group 
CLIENT_API BOOL CALL_METHOD CLIENT_AudioBroadcastAddDev(LLONG lLoginID);

// Remove device from the broadcast group 
CLIENT_API BOOL CALL_METHOD CLIENT_AudioBroadcastDelDev(LLONG lLoginID);

// audio encode-initialization(specified standard format, private format)
CLIENT_API int  CALL_METHOD CLIENT_InitAudioEncode(DH_AUDIO_FORMAT aft);

// Audio encode--data encode
CLIENT_API int	CALL_METHOD	CLIENT_AudioEncode(LLONG lTalkHandle, BYTE *lpInBuf, DWORD *lpInLen, BYTE *lpOutBuf, DWORD *lpOutLen);

// audio encode--complete and then exit
CLIENT_API int	CALL_METHOD	CLIENT_ReleaseAudioEncode();

//------------------------------------------------------------------------

// Search device log
CLIENT_API BOOL CALL_METHOD CLIENT_QueryLog(LLONG lLoginID, char *pLogBuffer, int maxlen, int *nLogBufferlen, int waittime=3000);

// Search device log page by page.
CLIENT_API BOOL CALL_METHOD CLIENT_QueryDeviceLog(LLONG lLoginID, QUERY_DEVICE_LOG_PARAM *pQueryParam, char *pLogBuffer, int nLogBufferLen, int *pRecLogNum, int waittime=3000);

// ��ѯ�豸��־����
CLIENT_API BOOL CALL_METHOD CLIENT_QueryDevLogCount(LLONG lLoginID, NET_IN_GETCOUNT_LOG_PARAM *pInParam, NET_OUT_GETCOUNT_LOG_PARAM* pOutParam , int waittime=3000);

// Search channel record status 
CLIENT_API BOOL CALL_METHOD CLIENT_QueryRecordState(LLONG lLoginID, char *pRSBuffer, int maxlen, int *nRSBufferlen, int waittime=1000);

// Search channel extra record status (the returned byte number was equal to the channel number, every byte instruct the respond channel's state,0-stop,1-manual,2-schedule)
CLIENT_API BOOL CALL_METHOD CLIENT_QueryExtraRecordState(LLONG lLoginID, char *pRSBuffer, int maxlen, int *nRSBufferlen, void *pReserved, int waittime=1000);

// Search device status
CLIENT_API BOOL CALL_METHOD CLIENT_QueryDevState(LLONG lLoginID, int nType, char *pBuf, int nBufLen, int *pRetLen, int waittime=1000);

// query remote device state,when nType = DH_DEVSTATE_ALARM_FRONTDISCONNECT,the number form 1.
CLIENT_API BOOL CALL_METHOD CLIENT_QueryRemotDevState(LLONG lLoginID, int nType, int nChannelID, char *pBuf, int nBufLen, int *pRetLen, int waittime=1000);
// Search system capacity information 
CLIENT_API BOOL CALL_METHOD CLIENT_QuerySystemInfo(LLONG lLoginID, int nSystemType, char *pSysInfoBuffer, int maxlen, int *nSysInfolen, int waittime=1000);

// New Search system capacity information(by Json)
CLIENT_API BOOL CALL_METHOD CLIENT_QueryNewSystemInfo(LLONG lLoginID, char* szCommand, int nChannelID, char* szOutBuffer, DWORD dwOutBufferSize, int *error, int waittime=1000);

// Get channel bit stream 
CLIENT_API LONG CALL_METHOD CLIENT_GetStatiscFlux(LLONG lLoginID, LLONG lPlayHandle);

// Get PTZ information 
CLIENT_API BOOL  CALL_METHOD CLIENT_GetPtzOptAttr(LLONG lLoginID,DWORD dwProtocolIndex,LPVOID lpOutBuffer,DWORD dwBufLen,DWORD *lpBytesReturned,int waittime=500);

// ��ȡ�豸����
CLIENT_API BOOL  CALL_METHOD CLIENT_GetDevCaps(LLONG lLoginID, int nType, void* pInBuf, void* pOutBuf, int nWaitTime);

//------------------------------------------------------------------------

// Reboot device 
CLIENT_API BOOL CALL_METHOD CLIENT_RebootDev(LLONG lLoginID);

// Shut down devic e
CLIENT_API BOOL CALL_METHOD CLIENT_ShutDownDev(LLONG lLoginID);

// Device control 
CLIENT_API BOOL CALL_METHOD CLIENT_ControlDevice(LLONG lLoginID, CtrlType type, void *param, int waittime = 1000);

// Set channel record status 
CLIENT_API BOOL CALL_METHOD CLIENT_SetupRecordState(LLONG lLoginID, char *pRSBuffer, int nRSBufferlen);

// set channel extra record status
CLIENT_API BOOL CALL_METHOD CLIENT_SetupExtraRecordState(LLONG lLoginID, char *pRSBuffer, int nRSBufferlen, void* pReserved);

// Search IO status
CLIENT_API BOOL CALL_METHOD CLIENT_QueryIOControlState(LLONG lLoginID, DH_IOTYPE emType, 
                                           void *pState, int maxlen, int *nIOCount, int waittime=1000);

// IO control 
CLIENT_API BOOL CALL_METHOD CLIENT_IOControl(LLONG lLoginID, DH_IOTYPE emType, void *pState, int maxlen);

// Compulsive I frame
CLIENT_API BOOL CALL_METHOD CLIENT_MakeKeyFrame(LLONG lLoginID, int nChannelID, int nSubChannel=0);

// public agency registration
typedef void (CALLBACK *fConnectMessCallBack)(LLONG lConnectHandle, NET_CLOUDSERVICE_CONNECT_RESULT* pConnectResult, void* pReserved, LDWORD dwUser);

CLIENT_API LLONG CALL_METHOD CLIENT_ConnectCloudService(LLONG lLoginID, NET_CLOUDSERVICE_CONNECT_PARAM* pConnectParm, fConnectMessCallBack pConnectMessCB, LDWORD dwUser, void* pReserved);
//------------------------------------------------------------------------

// Search user information 
CLIENT_API BOOL CALL_METHOD CLIENT_QueryUserInfo(LLONG lLoginID, USER_MANAGE_INFO *info, int waittime=1000);

// Search user information--extensive
CLIENT_API BOOL CALL_METHOD CLIENT_QueryUserInfoEx(LLONG lLoginID, USER_MANAGE_INFO_EX *info, int waittime=1000);

// Search device info--Max supports device of 64-ch
CLIENT_API BOOL CALL_METHOD CLIENT_QueryUserInfoNew(LLONG lLoginID, USER_MANAGE_INFO_NEW *info, void* pReserved, int waittime = 1000);
// Device operation user 
CLIENT_API BOOL CALL_METHOD CLIENT_OperateUserInfo(LLONG lLoginID, int nOperateType, void *opParam, void *subParam, int waittime=1000);

// Device operation user--extensive
CLIENT_API BOOL CALL_METHOD CLIENT_OperateUserInfoEx(LLONG lLoginID, int nOperateType, void *opParam, void *subParam, int waittime=1000);

// User operates the device--Max supports device of 64-ch
CLIENT_API BOOL CALL_METHOD CLIENT_OperateUserInfoNew(LLONG lLoginID, int nOperateType, void *opParam, void *subParam, void* pReserved, int waittime = 1000);

//------------------------------------------------------------------------

// Create transparent COM channel ,TransComType: high 2 bytes represent the serial number,low 2 bytes of serail type,type 0: serial,1:485
CLIENT_API LLONG CALL_METHOD CLIENT_CreateTransComChannel(LLONG lLoginID, int TransComType, unsigned int baudrate, unsigned int databits, unsigned int stopbits, unsigned int parity, fTransComCallBack cbTransCom, LDWORD dwUser);

// Transparent COM send out data 
CLIENT_API BOOL CALL_METHOD CLIENT_SendTransComData(LLONG lTransComChannel, char *pBuffer, DWORD dwBufSize);

// Release transparent COM channel 
CLIENT_API BOOL CALL_METHOD CLIENT_DestroyTransComChannel(LLONG lTransComChannel);

// Query the status of a transparent serial port
CLIENT_API BOOL CALL_METHOD CLIENT_QueryTransComParams(LLONG lLoginID, int TransComType, DH_COMM_STATE* pCommState, int nWaitTime = 500);

//------------------------------------------------------------------------

// Begin upgrading device program 
CLIENT_API LLONG CALL_METHOD CLIENT_StartUpgrade(LLONG lLoginID, char *pchFileName, fUpgradeCallBack cbUpgrade, LDWORD dwUser);

// Begin upgrading device program--extensive
CLIENT_API LLONG CALL_METHOD CLIENT_StartUpgradeEx(LLONG lLoginID, EM_UPGRADE_TYPE emType, char *pchFileName, fUpgradeCallBack cbUpgrade, LDWORD dwUser);

// Send out data
CLIENT_API BOOL CALL_METHOD CLIENT_SendUpgrade(LLONG lUpgradeID);

// Stop upgrading device program 
CLIENT_API BOOL CALL_METHOD CLIENT_StopUpgrade(LLONG lUpgradeID);

//------------------------------------------------------------------------

// Search configuration information 
CLIENT_API BOOL  CALL_METHOD CLIENT_GetDevConfig(LLONG lLoginID, DWORD dwCommand, LONG lChannel, LPVOID lpOutBuffer, DWORD dwOutBufferSize, LPDWORD lpBytesReturned,int waittime=500);

// Set configuration information 
CLIENT_API BOOL  CALL_METHOD CLIENT_SetDevConfig(LLONG lLoginID, DWORD dwCommand, LONG lChannel, LPVOID lpInBuffer, DWORD dwInBufferSize, int waittime=500);

// New configuration interface, Search configuration information(using Json protocol, see configuration SDK)
CLIENT_API BOOL CALL_METHOD CLIENT_GetNewDevConfig(LLONG lLoginID, char* szCommand, int nChannelID, char* szOutBuffer, DWORD dwOutBufferSize, int *error, int waittime=500);

// New configuration interface, Set configuration information(using Json protocol, see configuration SDK)
CLIENT_API BOOL CALL_METHOD CLIENT_SetNewDevConfig(LLONG lLoginID, char* szCommand, int nChannelID, char* szInBuffer, DWORD dwInBufferSize, int *error, int *restart, int waittime=500);

// Delete configuration interface(Json format)
CLIENT_API BOOL CALL_METHOD CLIENT_DeleteDevConfig(LLONG lLoginID, NET_IN_DELETECFG* pInParam, NET_OUT_DELETECFG* pOutParam, int waittime=500);

// Get the configuration member name interface(Json format)
CLIENT_API BOOL CALL_METHOD CLIENT_GetMemberNames(LLONG lLoginID, NET_IN_MEMBERNAME* pInParam, NET_OUT_MEMBERNAME* pOutParam, int waittime=500);

// ��ȡ������Ϣ
CLIENT_API BOOL CALL_METHOD CLIENT_QueryNetStat(LLONG lLoginID , EM_NET_QUERY_TYPE emType , void *lpInParam , int nInParamLen , void *lpOutParam , int nOutParamLen , int *pError = NULL , int nWaitTime=1000);

//------------------------------------------------------------------------

// Search device channel name 
CLIENT_API BOOL CALL_METHOD CLIENT_QueryChannelName(LLONG lLoginID, char *pChannelName, int maxlen, int *nChannelCount, int waittime=1000);

// Set device channel name
CLIENT_API BOOL  CALL_METHOD CLIENT_SetupChannelName(LLONG lLoginID,char *pbuf, int nbuflen);

// Set device channel character overlay 
CLIENT_API BOOL  CALL_METHOD CLIENT_SetupChannelOsdString(LLONG lLoginID, int nChannelNo, DH_CHANNEL_OSDSTRING* struOsdString, int nbuflen);

// Search device current time
CLIENT_API BOOL CALL_METHOD CLIENT_QueryDeviceTime(LLONG lLoginID, LPNET_TIME pDeviceTime, int waittime=1000);

// Set device current time
CLIENT_API BOOL CALL_METHOD CLIENT_SetupDeviceTime(LLONG lLoginID, LPNET_TIME pDeviceTime);

// Set device max bit stream
CLIENT_API BOOL CALL_METHOD CLIENT_SetMaxFlux(LLONG lLoginID, WORD wFlux);

//------------------------------------------------------------------------

// Import configuration file 
CLIENT_API LLONG CALL_METHOD CLIENT_ImportConfigFile(LLONG lLoginID, char *szFileName, fDownLoadPosCallBack cbUploadPos, LDWORD dwUserData, DWORD param=0);

// Stop importing configuration file
CLIENT_API BOOL CALL_METHOD CLIENT_StopImportCfgFile(LLONG lImportHandle);

// Exporting configuration file
CLIENT_API LLONG CALL_METHOD CLIENT_ExportConfigFile(LLONG lLoginID, DH_CONFIG_FILE_TYPE emConfigFileType, char *szSavedFilePath, fDownLoadPosCallBack cbDownLoadPos, LDWORD dwUserData);

// top exporting configuration file
CLIENT_API BOOL CALL_METHOD CLIENT_StopExportCfgFile(LLONG lExportHandle);

//------------------------------------------------------------------------

// Search device IP in DDBS by device name or device serial number
CLIENT_API BOOL CALL_METHOD CLIENT_GetDVRIPByResolveSvr(char *pchDVRIP, WORD wDVRPort, BYTE *sDVRName, WORD wDVRNameLen, BYTE *sDVRSerialNumber, WORD wDVRSerialLen, char* sGetIP);

// Search IPC,NVS and etc in LAN 
CLIENT_API BOOL CALL_METHOD CLIENT_SearchDevices(char* szBuf, int nBufLen, int* pRetLen, DWORD dwSearchTime, char* szLocalIp=NULL);

// asynchronism search IPC, NVS and etc in LAN
CLIENT_API LLONG CALL_METHOD CLIENT_StartSearchDevices(fSearchDevicesCB cbSearchDevices, void* pUserData, char* szLocalIp=NULL);

// stop asynchronism search IPC, NVS and etc in LAN
CLIENT_API BOOL CALL_METHOD CLIENT_StopSearchDevices(LLONG lSearchHandle);

// modify Device ip
CLIENT_API BOOL CALL_METHOD CLIENT_ModifyDevice(DEVICE_NET_INFO_EX *pDevNetInfo, DWORD dwWaitTime, int *iError = NULL, char* szLocalIp = NULL, void *reserved = NULL);

// search device ip cross VLAN
CLIENT_API BOOL CALL_METHOD CLIENT_SearchDevicesByIPs(DEVICE_IP_SEARCH_INFO* pIpSearchInfo, fSearchDevicesCB cbSearchDevices, LDWORD dwUserData, char* szLocalIp, DWORD dwWaitTime);
//------------------------------------------------------------------------

// Platform embedded interface
CLIENT_API BOOL CALL_METHOD CLIENT_GetPlatFormInfo(LLONG lLoginID, DWORD dwCommand, int nSubCommand, int nParam, LPVOID lpOutBuffer, DWORD dwOutBufferSize, LPDWORD lpBytesReturned,int waittime=500);
CLIENT_API BOOL CALL_METHOD CLIENT_SetPlatFormInfo(LLONG lLoginID, DWORD dwCommand, int nSubCommand, int nParam, LPVOID lpInBuffer, DWORD dwInBufferSize, int waittime=500);

// control focus 
//	dwFocusCommand = 0 focus adjucy
//	dwFocusCommand = 1continuous focus adjustment
//	dwFocusCommand = 2 auto adjust ,adjust to the best position,nFocus and nZoominvalid
CLIENT_API BOOL CALL_METHOD CLIENT_FocusControl(LLONG lLoginID, int nChannelID, DWORD dwFocusCommand, double nFocus, double nZoom, void *reserved = NULL, int waittime=500);

///////////////////////////////Mobile DVR interface///////////////////////////////

// Set snapshot callback function 
CLIENT_API void CALL_METHOD CLIENT_SetSnapRevCallBack(fSnapRev OnSnapRevMessage, LDWORD dwUser);

// Snapshot request
CLIENT_API BOOL CALL_METHOD CLIENT_SnapPicture(LLONG lLoginID, SNAP_PARAMS par);

// Snapshot request--extensive
CLIENT_API BOOL CALL_METHOD CLIENT_SnapPictureEx(LLONG lLoginID, SNAP_PARAMS *par, int *reserved = 0);

// Set GPS subscription callback function 
CLIENT_API void CALL_METHOD CLIENT_SetSubcribeGPSCallBack(fGPSRev OnGPSMessage, LDWORD dwUser);

// Set GPS subscription callback function - extensive
CLIENT_API void CALL_METHOD CLIENT_SetSubcribeGPSCallBackEX(fGPSRevEx OnGPSMessage, LDWORD dwUser);

// GPS message subscription 
CLIENT_API BOOL CALL_METHOD CLIENT_SubcribeGPS (LLONG lLoginID, BOOL bStart, LONG KeepTime, LONG InterTime);

// Set GPS subscription of temperature and humidity callback function
CLIENT_API void CALL_METHOD CLIENT_SetSubcribeGPSTHCallBack(fGPSTempHumidityRev OnGPSMessage, LDWORD dwUser);

// GPS subscription of temperature and humidity
CLIENT_API BOOL CALL_METHOD CLIENT_SubcribeGPSTempHumidity (LLONG lLoginID, BOOL bStart,	int InterTime, void* Reserved);

//GPS log query
CLIENT_API BOOL CALL_METHOD CLIENT_QueryGPSLog(LLONG lLoginID,QUERY_GPS_LOG_PARAM *pQueryParam, char *pLogBuffer, int nLogBufferLen, int *pRecLogNum, BOOL *bContinue, int waittime);

// ��������
CLIENT_API BOOL CALL_METHOD CLIENT_AttachMission(LLONG lLoginID, NET_IN_ATTACH_MISSION_PARAM *pInParam, NET_OUT_ATTACH_MISSION_PARAM *pOutParam, int nWaitTime);

// ȡ��������
CLIENT_API BOOL CALL_METHOD CLIENT_DetachMission(LLONG lAttachHandle);


//////////////////////////////NVD interface//////////////////////////////

// Query decoder information
CLIENT_API BOOL CALL_METHOD CLIENT_QueryDecoderInfo(LLONG lLoginID, LPDEV_DECODER_INFO lpDecInfo, int waittime=1000);

// Query decoder TV information
CLIENT_API BOOL CALL_METHOD CLIENT_QueryDecoderTVInfo(LLONG lLoginID, int nMonitorID, LPDEV_DECODER_TV lpMonitorInfo, int waittime=1000);

// Query decoder channel information
CLIENT_API BOOL CALL_METHOD CLIENT_QueryDecEncoderInfo(LLONG lLoginID, int nDecoderID, LPDEV_ENCODER_INFO lpEncoderInfo, int waittime=1000);

// Set decoder TV enable
CLIENT_API BOOL CALL_METHOD CLIENT_SetDecTVOutEnable(LLONG lLoginID, BYTE *pDecTVOutEnable, int nBufLen, int waittime=1000);

// set decoder tip layout enable, channel number start at 0
CLIENT_API BOOL CALL_METHOD CLIENT_SetDecLayOutEnable(LLONG lLoginID, BYTE bDecLayOutEnable, int nChannel, int waittime=1000);
//------------------------------------------------------------------------

// Set up asynchronous callback function
CLIENT_API BOOL CALL_METHOD CLIENT_SetOperateCallBack(LLONG lLoginID, fMessDataCallBack cbMessData, LDWORD dwUser);

// Control decoder TV screen partition,Interface is asynchronous
CLIENT_API LLONG CALL_METHOD CLIENT_CtrlDecTVScreen(LLONG lLoginID, int nMonitorID, BOOL bEnable, int nSplitType, BYTE *pEncoderChannel, int nBufLen, void* userdata=NULL);

// Switch decoder TV screen,Interface is asynchronous
// According to nMonitorID(TV ID) nWndID(Screen ID) Convert to nDecoderID(decoder channel) formula:nEncoderID nMonitorID*nSplitNum(Partition number)+nWndID
CLIENT_API LLONG CALL_METHOD CLIENT_SwitchDecTVEncoder(LLONG lLoginID, int nDecoderID, LPDEV_ENCODER_INFO lpEncoderInfo, void* userdata=NULL);

//------------------------------------------------------------------------

// Add combination of screen
CLIENT_API int CALL_METHOD CLIENT_AddTourCombin(LLONG lLoginID, int nMonitorID, int nSplitType, BYTE *pEncoderChannnel, int nBufLen, int waittime=1000);

// Delete combination of screen
CLIENT_API BOOL CALL_METHOD CLIENT_DelTourCombin(LLONG lLoginID, int nMonitorID, int nCombinID, int waittime=1000);

// Modify combination of screen
CLIENT_API BOOL CALL_METHOD CLIENT_SetTourCombin(LLONG lLoginID, int nMonitorID, int nCombinID, int nSplitType, BYTE *pEncoderChannel, int nBufLen, int waittime=1000);

// Query combination of screen,nCombinID: 0??32
CLIENT_API BOOL CALL_METHOD CLIENT_QueryTourCombin(LLONG lLoginID, int nMonitorID, int nCombinID, LPDEC_COMBIN_INFO lpDecCombinInfo, int waittime=1000);

// Set up tour operation
CLIENT_API BOOL CALL_METHOD CLIENT_SetDecoderTour(LLONG lLoginID, int nMonitorID, LPDEC_TOUR_COMBIN lpDecTourInfo, int waittime=1000);

// Query tour operation
CLIENT_API BOOL CALL_METHOD CLIENT_QueryDecoderTour(LLONG lLoginID, int nMonitorID, LPDEC_TOUR_COMBIN lpDecTourInfo, int waittime=1000);

// Query the current flux information of decoding channel
CLIENT_API BOOL CALL_METHOD CLIENT_QueryDecChannelFlux(LLONG lLoginID, int nDecoderID, LPDEV_DECCHANNEL_STATE lpChannelStateInfo, int waittime=1000);

// control decoder tour operation
CLIENT_API BOOL CALL_METHOD CLIENT_CtrlDecoderTour(LLONG lLoginID, int nMonitorID, DEC_CTRL_TOUR_TYPE emActionParam, int waittime = 1000);
//------------------------------------------------------------------------

typedef void (CALLBACK *fDecPlayBackPosCallBack)(LLONG lLoginID, int nEncoderID, DWORD dwTotalSize, DWORD dwPlaySize, LDWORD dwUser);

// Set the playback progress callback function
CLIENT_API BOOL CALL_METHOD CLIENT_SetDecPlaybackPos(LLONG lLoginID, fDecPlayBackPosCallBack cbPlaybackPos, LDWORD dwUser);

// Decoder TV screen playback,Interface is asynchronous
CLIENT_API LLONG CALL_METHOD CLIENT_DecTVPlayback(LLONG lLoginID, int nDecoderID, DEC_PLAYBACK_MODE emPlaybackMode, LPVOID lpInBuffer, DWORD dwInBufferSize, void* userdata=NULL);

// Control TV screen playback
CLIENT_API BOOL CALL_METHOD CLIENT_CtrlDecPlayback(LLONG lLoginID, int nDecoderID, DEC_CTRL_PLAYBACK_TYPE emCtrlType, int nValue, int waittime=1000);

///////////////////////////////intelligent device interface///////////////////////////////

// real load picture of intelligent analysis 
CLIENT_API LLONG CALL_METHOD CLIENT_RealLoadPicture(LLONG lLoginID, int nChannelID, DWORD dwAlarmType, fAnalyzerDataCallBack cbAnalyzerData, LDWORD dwUser);

// real load picture of intelligent analysis(expand interface: 'bNeedPicFile == true' instruct load picture file, 'bNeedPicFile == false' instruct not load picture file ) 
CLIENT_API LLONG CALL_METHOD CLIENT_RealLoadPictureEx(LLONG lLoginID, int nChannelID, 
													 DWORD dwAlarmType, 
													 BOOL bNeedPicFile, 
													 fAnalyzerDataCallBack cbAnalyzerData, 
													 LDWORD dwUser, 
													 void* Reserved);

// stop load picture of intelligent analysis
CLIENT_API BOOL CALL_METHOD CLIENT_StopLoadPic(LLONG lAnalyzerHandle);

// Search according to the criteria
CLIENT_API LLONG	CALL_METHOD CLIENT_FindFileEx(LLONG lLoginID, EM_FILE_QUERY_TYPE emType, void* pQueryCondition, void *reserved, int waittime=1000);	

// Search file:nFilecount:the searched amount. The return value is media file amount. The search in the specified time completed if return <nFilecount.
CLIENT_API int	CALL_METHOD CLIENT_FindNextFileEx(LLONG lFindHandle, int nFilecount, void* pMediaFileInfo, int maxlen, void *reserved, int waittime=1000);

// End the file search
CLIENT_API BOOL CALL_METHOD CLIENT_FindCloseEx(LLONG lFindHandle);

// ��ȡ���ϲ�ѯ�������ļ�����
CLIENT_API BOOL CALL_METHOD CLIENT_GetTotalFileCount(LLONG lFindHandle, int* pTotalCount, void *reserved, int waittime=1000);

// ���ò�ѯ��ת����
CLIENT_API BOOL CALL_METHOD CLIENT_SetFindingJumpOption(LLONG lFindHandle, NET_FINDING_JUMP_OPTION_INFO* pOption, void *reserved, int waittime=1000);

// Download the specified intelligent analytics data-image
CLIENT_API LLONG CALL_METHOD CLIENT_DownloadMediaFile(LLONG lLoginID,EM_FILE_QUERY_TYPE emType, void* lpMediaFileInfo, char *sSavedFileName, fDownLoadPosCallBack cbDownLoadPos, LDWORD dwUserData,  void *reserved);

// Stop downloading the data
CLIENT_API BOOL CALL_METHOD CLIENT_StopDownloadMediaFile(LLONG lFileHandle);

// ���������ļ�
CLIENT_API BOOL CALL_METHOD CLIENT_DownLoadMultiFile(LLONG lLoginID, NET_IN_DOWNLOAD_MULTI_FILE *pstInParam, NET_OUT_DOWNLOAD_MULTI_FILE *pstOutParam, int waittime=1000);

// ֹͣ����
CLIENT_API BOOL CALL_METHOD CLIENT_StopLoadMultiFile(LLONG lDownLoadHandle);

// download picture of intelligent analysis when being off line
CLIENT_API LLONG CALL_METHOD CLIENT_LoadOffLineFile(LLONG lLoginID, int nChannelID, DWORD dwAlarmType, LPNET_TIME_EX lpStartTime, LPNET_TIME_EX lpEndTime, fAnalyzerDataCallBack cbAnalyzerData, LDWORD dwUser);

//Pause IVS data download(bPause=TRUE, it is to stop download , bPause=FALSE , it is to resume download.)
CLIENT_API BOOL CALL_METHOD CLIENT_PauseLoadPic(LLONG lLoadHadle, BOOL bPause);

// traffic snap--snapshot by network
CLIENT_API BOOL CALL_METHOD CLIENT_TrafficSnapByNetwork(LLONG lLoginID, int nChannelID, NET_IN_SNAPSHOT* pstInParam, NET_OUT_SNAPSHOT* pstOutParam);
// start traffic flux state
// traffic control --forced the red light
CLIENT_API BOOL CALL_METHOD CLIENT_TrafficForceLightState(LLONG lLoginID, int nChannelID, NET_IN_FORCELIGHTSTATE* pstInParam, NET_OUT_FORCELIGHTSTATE* pstOutParam, int waittime=1000);

//  �ڰ���������
CLIENT_API BOOL CALL_METHOD CLIENT_OperateTrafficList(LLONG lLoginID ,  NET_IN_OPERATE_TRAFFIC_LIST_RECORD* pstInParam , NET_OUT_OPERATE_TRAFFIC_LIST_RECORD *pstOutParam = NULL ,  int waittime = 1000);

// start traffic flux state
CLIENT_API LLONG CALL_METHOD CLIENT_StartTrafficFluxStat(LLONG lLoginID, NET_IN_TRAFFICFLUXSTAT* pstInParam,NET_OUT_TRAFFICFLUXSTAT* pstOutParam);

// stop traffic flux state
CLIENT_API BOOL CALL_METHOD CLIENT_StopTrafficFluxStat(LLONG lFluxStatHandle);

// start find flux state
CLIENT_API LLONG CALL_METHOD CLIENT_StartFindFluxStat(LLONG lLoginID, NET_IN_TRAFFICSTARTFINDSTAT* pstInParam, NET_OUT_TRAFFICSTARTFINDSTAT* pstOutParam);

// do find flux state
CLIENT_API int	CALL_METHOD CLIENT_DoFindFluxStat(LLONG lFindHandle, NET_IN_TRAFFICDOFINDSTAT* pstInParam,NET_OUT_TRAFFICDOFINDSTAT* pstOutParam);

// stop find flux state
CLIENT_API BOOL CALL_METHOD CLIENT_StopFindFluxStat(LLONG lFindHandle);
// start find number state
CLIENT_API LLONG CALL_METHOD CLIENT_StartFindNumberStat(LLONG lLoginID, NET_IN_FINDNUMBERSTAT* pstInParam, NET_OUT_FINDNUMBERSTAT* pstOutParam);

// do find number state
CLIENT_API int	CALL_METHOD CLIENT_DoFindNumberStat(LLONG lFindHandle, NET_IN_DOFINDNUMBERSTAT* pstInParam, NET_OUT_DOFINDNUMBERSTAT* pstOutParam);

// stop find number state
CLIENT_API BOOL CALL_METHOD CLIENT_StopFindNumberStat(LLONG lFindHandle);

// Call of the analysis device method
CLIENT_API BOOL CALL_METHOD CLIENT_OperateVideoAnalyseDevice(LLONG lLoginID, int nChannelID, char* szCmd, void *pstInParam, void *pstOutParam, int waittime=1000);

// �����豸�ķ�������
CLIENT_API BOOL CALL_METHOD CLIENT_OperateCommDevice(LLONG lLoginID, int nChannelID, char* szCmd, void *pstInParam, void *pstOutParam, int waittime=1000);

// Intelligent track speed dome control port.
CLIENT_API BOOL CALL_METHOD CLIENT_ControlIntelliTracker(LLONG lLoginID, NET_IN_CONTROL_INTELLITRACKER* pstInParam, NET_OUT_CONTROL_INTELLITRACKER* pstOutParam);

// master-slave device method,look for CLIENT_OperateMasterSlaveDevice
CLIENT_API BOOL CALL_METHOD CLIENT_OperateMasterSlaveDevice(LLONG lLoginID, int nChannelID, char* szCmd, void *pstInParam, void *pstOutParam, int waittime=1000);

////////////////////////////////  Video analysis /////////////////////////////////
// Real-time get video analysis result 
CLIENT_API BOOL CALL_METHOD CLIENT_StartVideoDiagnosis(LLONG lLoginID, NET_IN_VIDEODIAGNOSIS *pstInParam, NET_OUT_VIDEODIAGNOSIS *pstOutParam);

// Stop video analysis result report
CLIENT_API BOOL CALL_METHOD CLIENT_StopVideoDiagnosis(LLONG hDiagnosisHandle);

// Start video analysis result search
CLIENT_API BOOL CALL_METHOD CLIENT_StartFindDiagnosisResult(LLONG lLoginID, NET_IN_FIND_DIAGNOSIS* pstInParam, NET_OUT_FIND_DIAGNOSIS* pstOutParam);

// Get video analysis result info
CLIENT_API BOOL CALL_METHOD CLIENT_DoFindDiagnosisResult(LLONG hFindHandle,NET_IN_DIAGNOSIS_INFO* pstInParam, NET_OUT_DIAGNOSIS_INFO* pstOutParam);

// End video analysis result search
CLIENT_API BOOL CALL_METHOD CLIENT_StopFindDiagnosis(LLONG hFindHandle);

// ����ʵʱ��ϼƻ�
CLIENT_API BOOL CALL_METHOD CLIENT_StartRealTimeProject(LLONG lLoginID,NET_IN_START_RT_PROJECT_INFO* pstInParam, NET_OUT_START_RT_PROJECT_INFO* pstOutParam, int nWaitTime);

// ֹͣʵʱ��ϼƻ�
CLIENT_API BOOL CALL_METHOD CLIENT_StopRealTimeProject(LLONG lLoginID,NET_IN_STOP_RT_PROJECT_INFO* pstInParam, NET_OUT_STOP_RT_PROJECT_INFO* pstOutParam, int nWaitTime);

// get parking space status
CLIENT_API BOOL CALL_METHOD CLIENT_GetParkingSpaceStatus(LLONG lLoginID, NET_IN_GET_PARKINGSPACE_STATUS* pstInParam, NET_OUT_GET_PARKINGSPACE_STATUS* pstOutParam);

// attach parking space data
CLIENT_API LLONG CALL_METHOD CLIENT_AttachParkingSpaceData(LLONG lLoginID, NET_IN_ATTACH_PARKINGSPACE* pstInParam, NET_OUT_ATTACH_PARKINGSPACE* pstOutParam);

// detach parking space data
CLIENT_API BOOL CALL_METHOD CLIENT_DetachParkingSpaceData(NET_IN_DETACH_PARKINGSPACE* pstInParam, NET_OUT_DETACH_PARKINGSPACE* pstOutParam);

// get car port light status
CLIENT_API BOOL CALL_METHOD CLIENT_GetCarPortLightStatus(LLONG lLoginID, NET_IN_GET_CARPORTLIGHT_STATUS* pstInParam, NET_OUT_GET_CARPORTLIGHT_STATUS* pstOutParam, int waittime=1000);

// ser car port light status
CLIENT_API BOOL CALL_METHOD CLIENT_SetCarPortLightStatus(LLONG lLoginID, NET_IN_SET_CARPORTLIGHT_STATUS* pstInParam, NET_OUT_SET_CARPORTLIGHT_STATUS* pstOutParam, int waittime=1000);

// �������ܷ������ȣ���������Ƶ����ԴΪ¼���ļ�ʱ��
CLIENT_API BOOL CALL_METHOD CLIENT_AttachVideoAnalyseState(LLONG lLoginID, NET_IN_ATTACH_VIDEOANALYSE_STATE* pstInParam, NET_OUT_ATTACH_VIDEOANALYSE_STATE* pstOutParam, int nWaittime=1000);

// ֹͣ����
CLIENT_API BOOL CALL_METHOD CLIENT_DetachVideoAnalyseState(LLONG lAttachHandle);

////////////////////////////////synopsis video' interface////////////////////////////////

// add video synopsis task
CLIENT_API BOOL CALL_METHOD CLIENT_AddVideoSynopsisTask(LLONG lLoginID,	NET_IN_ADD_VIDEOSYNOPSIS* pstInParam, NET_OUT_ADD_VIDEOSYNOPSIS* pstOutParam);

// pause video synopsis task
CLIENT_API BOOL CALL_METHOD CLIENT_PauseVideoSynopsisTask(LLONG lLoginID, NET_IN_PAUSE_VIDEOSYNOPSIS* pstInParam);

// remove video synopsis task
CLIENT_API BOOL CALL_METHOD CLIENT_RemoveVideoSynopsisTask(LLONG lLoginID, NET_IN_REMOVE_VIDEOSYNOPSIS* pstInParam);

// subscibe real load object data
CLIENT_API BOOL CALL_METHOD CLIENT_RealLoadObjectData(LLONG lLoginID, NET_IN_REALLOAD_OBJECTDATA* pstInParam, NET_OUT_REALLOAD_OBJECTDATA* pstOutParam);

// stop load object data
CLIENT_API BOOL CALL_METHOD CLIENT_StopLoadObjectData(LLONG lRealLoadHandle, NET_IN_STOPLOAD_OBJECTDATA* pstInParam);

// subscribe real synopsis progress state
CLIENT_API BOOL CALL_METHOD CLIENT_RealLoadSynopsisState(LLONG lLoginID, NET_IN_REALLAOD_SYNOPSISSTATE* pstInParam, NET_OUT_REALLOAD_SYNOPSISSTATE* pstOutParam);

// stop subscribe real synopsis progress state
CLIENT_API BOOL CALL_METHOD CLIENT_StopLoadSynopsisState(LLONG lRealLoadHandle, NET_IN_STOPLOAD_SYNOPSISSTATE* pstInParam);

// query real synopsis video state
CLIENT_API BOOL CALL_METHOD CLIENT_QueryVideoSynopsisInfo(LLONG lLoginID, NET_IN_QUERY_VIDEOSYNOPSIS* pstInParam, NET_OUT_QUERY_VIDEOSYNOPSIS* pstuOutParam);

// according to the query criteria query synopsis file
CLIENT_API BOOL	CALL_METHOD CLIENT_FindSynopsisFile(LLONG lLoginID, NET_IN_FIND_SYNOPSISFILE *pstInParam, NET_OUT_FIND_SYNOPSISFILE *pstOutParam);	

// query synopsis file
CLIENT_API BOOL	CALL_METHOD CLIENT_FindNextSynopsisFile(LLONG lFindHandle, NET_IN_FINDNEXT_SYNOPSISFILE *pstInParam, NET_OUT_FINDNEXT_SYNOPSISFILE *pstOutParam);

// close query synopsis file
CLIENT_API BOOL CALL_METHOD CLIENT_SynopsisFindClose(LLONG lFindHandle);

// download synosis file
CLIENT_API BOOL CALL_METHOD CLIENT_DownLoadSynosisFile(LLONG lLoginID, NET_IN_DOWNLOAD_SYNOPSISFILE *pstInParam, NET_OUT_DOWNLOAD_SYNOPSISFILE *pstOutParam);

// stop load synosis file
CLIENT_API BOOL CALL_METHOD CLIENT_StopLoadSynosisFile(LLONG lDownLoadHandle);

// according to the path to the file request video service,generate the file information
CLIENT_API BOOL	CALL_METHOD	CLIENT_SetFilePathInfo(LLONG lLoginID, NET_IN_SET_FILEPATHINFO* pstInParam);

// attach add file state
CLIENT_API LLONG CALL_METHOD CLIENT_AttachAddFileState(LLONG lLoginID, const NET_IN_ADDFILE_STATE* pstInParam, NET_OUT_ADDFILE_STATE *pstOutParam, int nWaitTime = 1000);

// cancel attach add file state,  CLIENT_AttacAddFileState return lAttachHandle
CLIENT_API LLONG CALL_METHOD CLIENT_DetachAddFileState(LLONG lAttachHandle);

/////////////////////////////////����ʶ��ӿ�/////////////////////////////////////////
// ����ʶ�����ݿ���Ϣ�������������Ӻ�ɾ����
CLIENT_API BOOL CALL_METHOD CLIENT_OperateFaceRecognitionDB(LLONG lLoginID, const NET_IN_OPERATE_FACERECONGNITIONDB* pstInParam, NET_OUT_OPERATE_FACERECONGNITIONDB *pstOutParam, int nWaitTime = 1000);

// ��������ѯ����ʶ���� 
CLIENT_API BOOL CALL_METHOD CLIENT_StartFindFaceRecognition(LLONG lLoginID, const NET_IN_STARTFIND_FACERECONGNITION* pstInParam, NET_OUT_STARTFIND_FACERECONGNITION *pstOutParam, int nWaitTime = 1000);

// ��������ʶ����:nFilecount:��Ҫ��ѯ������, ����ֵΪý���ļ����� ����ֵ<nFilecount����Ӧʱ����ڵ��ļ���ѯ���(ÿ�����ֻ�ܲ�ѯ20����¼)
CLIENT_API BOOL CALL_METHOD CLIENT_DoFindFaceRecognition(const NET_IN_DOFIND_FACERECONGNITION* pstInParam, NET_OUT_DOFIND_FACERECONGNITION *pstOutParam, int nWaitTime = 1000);

// ������ѯ
CLIENT_API BOOL CALL_METHOD CLIENT_StopFindFaceRecognition(LLONG lFindHandle);

// �������(����һ�Ŵ�ͼ�������ͼ�б�������������ͼƬ)
CLIENT_API BOOL CALL_METHOD CLIENT_DetectFace(LLONG lLoginID, const NET_IN_DETECT_FACE* pstInParam, NET_OUT_DETECT_FACE *pstOutParam, int nWaitTime = 1000);

//////////////////////////////// burn the upload interface /////////////////////////////////

// �򿪿�¼�Ự, ���ؿ�¼�Ự���
CLIENT_API LLONG CALL_METHOD CLIENT_StartBurnSession(LLONG lLoginID, const NET_IN_START_BURN_SESSION* pstInParam, NET_OUT_START_BURN_SESSION *pstOutParam, int nWaitTime);

// �رտ�¼�Ự
CLIENT_API BOOL CALL_METHOD CLIENT_StopBurnSession(LLONG lBurnSession);

// ��ʼ��¼
CLIENT_API BOOL CALL_METHOD CLIENT_StartBurn(LLONG lBurnSession, const NET_IN_START_BURN* pstInParam, NET_OUT_START_BURN *pstOutParam, int nWaitTime);

// ֹͣ��¼
CLIENT_API BOOL CALL_METHOD CLIENT_StopBurn(LLONG lBurnSession);

// ��ͣ/�ָ���¼
CLIENT_API BOOL CALL_METHOD CLIENT_PauseBurn(LLONG lBurnSession, BOOL bPause);

// �ص���
CLIENT_API BOOL CALL_METHOD CLIENT_BurnMarkTag(LLONG lBurnSession, const NET_IN_BURN_MARK_TAG* pstInParam, NET_OUT_BURN_MARK_TAG *pstOutParam, int nWaitTime);

// ����
CLIENT_API BOOL CALL_METHOD CLIENT_BurnChangeDisk(LLONG lBurnSession, const NET_IN_BURN_CHANGE_DISK* pstInParam, NET_OUT_BURN_CHANGE_DISK *pstOutParam, int nWaitTime);

// ��ȡ��¼״̬
CLIENT_API BOOL CALL_METHOD CLIENT_BurnGetState(LLONG lBurnSession, const NET_IN_BURN_GET_STATE* pstInParam, NET_OUT_BURN_GET_STATE *pstOutParam, int nWaitTime);

// attach burn state
CLIENT_API LLONG CALL_METHOD CLIENT_AttachBurnState(LLONG lLoginID, const NET_IN_ATTACH_STATE* pstInParam, NET_OUT_ATTACH_STATE *pstOutParam, int nWaitTime = 1000);

// ȡ��������¼״̬��lAttachHandle��CLIENT_AttachBurnState����ֵ
CLIENT_API BOOL CALL_METHOD CLIENT_DetachBurnState(LLONG lAttachHandle);

// start burn upload
CLIENT_API LLONG CALL_METHOD CLIENT_StartUploadFileBurned(LLONG lLoginID, const NET_IN_FILEBURNED_START* pstInParam, NET_OUT_FILEBURNED_START *pstOutParam, int nWaitTime = 1000);

// send file burned,CLIENT_StartUploadFileBurned return lUploadHandle
CLIENT_API BOOL	CALL_METHOD	CLIENT_SendFileBurned(LLONG lUploadHandle);

// stop upload 
CLIENT_API BOOL	CALL_METHOD	CLIENT_StopUploadFileBurned(LLONG lUploadHandle);

// ������¼У��״̬
CLIENT_API LLONG CALL_METHOD CLIENT_AttachBurnCheckState(LLONG lLoginID, const NET_IN_ATTACH_BURN_CHECK* pstInParam, NET_OUT_ATTACH_BURN_CHECK* pstOutParam, int nWaitTime = 1000);

// cancle attach burn state,CLIENT_DetachBurnCheckState return lAttachHandle
CLIENT_API BOOL CALL_METHOD CLIENT_DetachBurnCheckState(LLONG lAttachHandle);

// ���Ŀ�¼������Ϣ
CLIENT_API LLONG CALL_METHOD CLIENT_AttachBurnCase(LLONG lLoginID, const NET_IN_ATTACH_BURN_CASE* pInParam, NET_OUT_ATTACH_BURN_CASE* pOutParam, int nWaitTime);

// ȡ�����Ŀ�¼������Ϣ��lAttachHandle��CLIENT_AttachBurnCase����ֵ
CLIENT_API BOOL CALL_METHOD CLIENT_DetachBurnCase(LLONG lAttachHandle);

//////////////////////////////// logical device /////////////////////////////////////////

// attach camerastate
CLIENT_API LLONG CALL_METHOD CLIENT_AttachCameraState(LLONG lLoginID, const NET_IN_CAMERASTATE* pstInParam, NET_OUT_CAMERASTATE *pstOutParam, int nWaitTime = 3000);

// detach camera state,CLIENT_AttachCameraState return lAttachHandle
CLIENT_API BOOL	CALL_METHOD	CLIENT_DetachCameraState(LLONG lAttachHandle);

//get all valid display source
CLIENT_API BOOL CALL_METHOD CLIENT_MatrixGetCameras(LLONG lLoginID, const DH_IN_MATRIX_GET_CAMERAS* pInParam, DH_OUT_MATRIX_GET_CAMERAS* pOutParam, int nWaitTime = 1000);

// add logical device
CLIENT_API BOOL CALL_METHOD CLIENT_MatrixAddCameras(LLONG lLoginID, const DH_IN_ADD_LOGIC_DEVICE_CAMERA* pInParam, DH_OUT_ADD_LOGIC_DEVICE_CAMERA* pOutParam, int nWaitTime = 1000);
////////////////////////////////  Matrix interface /////////////////////////////////

// Search product definition
CLIENT_API BOOL CALL_METHOD CLIENT_QueryProductionDefinition(LLONG lLoginID, DH_PRODUCTION_DEFNITION* pstuProdDef, int nWaitTime = 1000);

// Search matri sub card info
CLIENT_API BOOL CALL_METHOD CLIENT_QueryMatrixCardInfo(LLONG lLoginID, DH_MATRIX_CARD_LIST* pstuCardList, int nWaitTime = 1000);

// Search system status
CLIENT_API BOOL CALL_METHOD	CLIENT_QuerySystemStatus(LLONG lLoginID, DH_SYSTEM_STATUS* pstuStatus, int nWaitTime = 1000);

// Search split mode
CLIENT_API BOOL CALL_METHOD CLIENT_GetSplitCaps(LLONG lLoginID, int nChannel, DH_SPLIT_CAPS* pstuCaps, int nWaitTime = 1000);

// Search/set display source 
CLIENT_API BOOL CALL_METHOD CLIENT_GetSplitSource(LLONG lLoginID, int nChannel, int nWindow, DH_SPLIT_SOURCE* pstuSplitSrc, int nMaxCount, int* pnRetCount, int nWaitTime = 1000);
CLIENT_API BOOL CALL_METHOD CLIENT_SetSplitSource(LLONG lLoginID, int nChannel, int nWindow, const DH_SPLIT_SOURCE* pstuSplitSrc, int nSrcCount, int nWaitTime = 1000);
// ������ʾԴ, ֧���������
CLIENT_API BOOL CALL_METHOD CLIENT_SetSplitSourceEx(LLONG lLoginID, const NET_IN_SET_SPLIT_SOURCE* pInparam, NET_OUT_SET_SPLIT_SOURCE* pOutParam, int nWaitTime = 1000);

// Search/set split mode
CLIENT_API BOOL CALL_METHOD CLIENT_GetSplitMode(LLONG lLoginID, int nChannel, DH_SPLIT_MODE_INFO* pstuSplitInfo, int nWaitTime = 1000);
CLIENT_API BOOL CALL_METHOD CLIENT_SetSplitMode(LLONG lLoginID, int nChannel, const DH_SPLIT_MODE_INFO* pstuSplitInfo, int nWaitTime = 1000);

// Search split group amount
CLIENT_API BOOL CALL_METHOD CLIENT_GetSplitGroupCount(LLONG lLoginID, int nChannel, DH_SPLIT_MODE emSplitMode, int* pnGroupCount, int nWaitTime = 1000);

// Search video output capability
CLIENT_API BOOL CALL_METHOD CLIENT_GetVideoOutCaps(LLONG lLoginID, int nChannel, DH_VIDEO_OUT_CAPS* pstuCaps, int nWaitTime = 1000);

// Set video output option
CLIENT_API BOOL CALL_METHOD CLIENT_SetVideoOutOption(LLONG lLoginID, int nChannel, const DH_VIDEO_OUT_OPT* pstuVideoOut, int nWaitTime = 1000);

// Search the current window of video input channel
CLIENT_API BOOL CALL_METHOD CLIENT_QueryVideoOutWindows(LLONG lLoginID, int nChannel, DH_VIDEO_OUT_WINDOW* pstuWnds, int nMaxWndCount, int* pnRetWndCount, int nWaitTime = 1000);

// set windown position
CLIENT_API BOOL CALL_METHOD	CLIENT_SetSplitWindowRect(LLONG lLoginID, const DH_IN_SPLIT_SET_RECT* pInParam, DH_OUT_SPLIT_SET_RECT* pOutParam, int nWaitTime = 1000);
CLIENT_API BOOL	CALL_METHOD	CLIENT_GetSplitWindowRect(LLONG lLoginID, const DH_IN_SPLIT_GET_RECT* pInParam, DH_OUT_SPLIT_GET_RECT* pOutParam, int nWaitTime = 1000);

// open or close window
CLIENT_API BOOL CALL_METHOD CLIENT_OpenSplitWindow(LLONG lLoginID, const DH_IN_SPLIT_OPEN_WINDOW* pInParam, DH_OUT_SPLIT_OPEN_WINDOW* pOutParam, int nWaitTime = 1000);
CLIENT_API BOOL CALL_METHOD CLIENT_CloseSplitWindow(LLONG lLoginID, const DH_IN_SPLIT_CLOSE_WINDOW* pInParam, DH_OUT_SPLIT_CLOSE_WINDOW* pOutParam, int nWaitTime = 1000);

// set window order
CLIENT_API BOOL CALL_METHOD CLIENT_SetSplitTopWindow(LLONG lLoginID, const DH_IN_SPLIT_SET_TOP_WINDOW* pInParam, DH_OUT_SPLIT_SET_TOP_WINDOW* pOutParam, int nWaitTime = 1000);

// get windows info
CLIENT_API BOOL CALL_METHOD CLIENT_GetSplitWindowsInfo(LLONG lLoginID, const DH_IN_SPLIT_GET_WINDOWS* pInParam, DH_OUT_SPLIT_GET_WINDOWS* pOutParam, int nWaitTime = 1000);

// load or save collection
CLIENT_API BOOL CALL_METHOD	CLIENT_LoadSplitCollection(LLONG lLoginID, const DH_IN_SPLIT_LOAD_COLLECTION* pInParam, DH_OUT_SPLIT_LOAD_COLLECTION* pOutParam, int nWaitTime = 1000);
CLIENT_API BOOL CALL_METHOD	CLIENT_SaveSplitCollection(LLONG lLoginID, const DH_IN_SPLIT_SAVE_COLLECTION* pInParam, DH_OUT_SPLIT_SAVE_COLLECTION* pOutParam, int nWaitTime = 1000);

// get collection info
CLIENT_API BOOL CALL_METHOD CLIENT_GetSplitCollections(LLONG lLoginID, const DH_IN_SPLIT_GET_COLLECTIONS* pInParam, DH_OUT_SPLIT_GET_COLLECTIONS* pOutParam, int nWaitTime = 1000);

// rename collection
CLIENT_API BOOL	CALL_METHOD CLIENT_RenameSplitCollection(LLONG lLoginID, const DH_IN_SPLIT_RENAME_COLLECTION* pInParam, DH_OUT_SPLIT_RENAME_COLLECTION* pOutParam, int nWaitTime = 1000);

// delete collection
CLIENT_API BOOL CALL_METHOD CLIENT_DeleteSplitCollection(LLONG lLoginID, const DH_IN_SPLIT_DELETE_COLLECTION* pInParam, DH_OUT_SPLIT_DELETE_COLLECTION* pOutParam, int nWaitTime = 1000);

// decode policy
CLIENT_API BOOL CALL_METHOD CLIENT_SetDecodePolicy(LLONG lLoginID, const DH_IN_SET_DEC_POLICY* pInParam, DH_OUT_SET_DEC_POLICY* pOutParam, int nWaitTime = 1000);
CLIENT_API BOOL CALL_METHOD CLIENT_GetDecodePolicy(LLONG lLoginID, const DH_IN_GET_DEC_POLICY* pInParam, DH_OUT_GET_DEC_POLICY* pOutParam, int nWaitTime = 1000);

// mode of audio output
CLIENT_API BOOL CALL_METHOD CLIENT_SetSplitAudioOuput(LLONG lLoginID, const DH_IN_SET_AUDIO_OUTPUT* pInParam, DH_OUT_SET_AUDIO_OUTPUT* pOutParam, int nWaitTime = 1000);
CLIENT_API BOOL CALL_METHOD CLIENT_GetSplitAudioOuput(LLONG lLoginID, const DH_IN_GET_AUDIO_OUTPUT* pInParam, DH_OUT_GET_AUDIO_OUTPUT* pOutParam, int nWaitTime = 1000);

// set display source
CLIENT_API BOOL CALL_METHOD CLIENT_MatrixSetCameras(LLONG lLoginID, const DH_IN_MATRIX_SET_CAMERAS* pInParam, DH_OUT_MATRIX_SET_CAMERAS* pOutParam, int nWaitTime = 1000);

// get video in ability
CLIENT_API BOOL CALL_METHOD CLIENT_GetVideoInCaps(LLONG lLoginID, const DH_IN_GET_VIDEO_IN_CAPS* pInParam, DH_OUT_GET_VIDEO_IN_CAPS* pOutParam, int nWaitTime = 1000);

// get mode of video out
CLIENT_API BOOL CALL_METHOD CLIENT_EnumVideoOutModes(LLONG lLoginID, const DH_IN_ENUM_VIDEO_OUT_MODES* pInParam, DH_OUT_ENUM_VIDEO_OUT_MODES* pOutParam, int nWaitTime = 1000);

// get or set splite output OSD info
CLIENT_API BOOL CALL_METHOD CLIENT_GetSplitOSD(LLONG lLoginID, const DH_IN_SPLIT_GET_OSD* pInParam, DH_OUT_SPLIT_GET_OSD* pOutParam, int nWaitTime = 1000);
CLIENT_API BOOL CALL_METHOD CLIENT_SetSplitOSD(LLONG lLoginID, const DH_IN_SPLIT_SET_OSD* pInParam, DH_OUT_SPLIT_SET_OSD* pOutParam, int nWaitTime = 1000);

// ���ô�����Ѳ��ʾԴ
CLIENT_API BOOL CALL_METHOD CLIENT_SetTourSource(LLONG lLoginID, const NET_IN_SET_TOUR_SOURCE* pInParam, NET_OUT_SET_TOUR_SOURCE* pOutParam, int nWaitTime = 1000);

// ��λ�����л�
CLIENT_API BOOL CALL_METHOD CLIENT_MatrixSwitch(LLONG lLoginID, const NET_IN_MATRIX_SWITCH* pInParam, NET_OUT_MATRIX_SWITCH* pOutParam, int nWaitTime);

// ������ʾԴ, ֧��ͬʱ���ö������
CLIENT_API BOOL CALL_METHOD CLIENT_SplitSetMultiSource(LLONG lLoginID, const NET_IN_SPLIT_SET_MULTI_SOURCE* pInParam, NET_OUT_SPLIT_SET_MULTI_SOURCE* pOutParam, int nWaitTime);

// ��Ƶ�ָ����
CLIENT_API BOOL CALL_METHOD CLIENT_OperateSplit(LLONG lLoginID, NET_SPLIT_OPERATE_TYPE emType, void* pInParam, void* pOutParam, int nWaitTime);


//////////////////////////////////// monitor wall control //////////////////////////////////////

// power control 
CLIENT_API BOOL CALL_METHOD CLIENT_PowerControl(LLONG lLoginID, const DH_IN_WM_POWER_CTRL* pInParam, DH_OUT_WM_POWER_CTRL* pOutParam, int nWaitTime = 1000);

// get or set mode of display 
CLIENT_API BOOL CALL_METHOD CLIENT_SetDisplayMode(LLONG lLoginID, const DH_IN_WM_SET_DISPLAY_MODE* pInParam, DH_OUT_WM_SET_DISPLAY_MODE* pOutParam, int nWaitTime = 1000);
CLIENT_API BOOL CALL_METHOD CLIENT_GetDisplayMode(LLONG lLoginID, const DH_IN_WM_GET_DISPLAY_MODE* pInParam, DH_OUT_WM_GET_DISPLAY_MODE* pOutParam, int nWaitTime = 1000);

// load or save plan
CLIENT_API BOOL CALL_METHOD	CLIENT_LoadMonitorWallCollection(LLONG lLoginID, const DH_IN_WM_LOAD_COLLECTION* pInParam, DH_OUT_WM_LOAD_COLLECTION* pOutParam, int nWaitTime = 1000);
CLIENT_API BOOL CALL_METHOD	CLIENT_SaveMonitorWallCollection(LLONG lLoginID, const DH_IN_WM_SAVE_COLLECTION* pInParam, DH_OUT_WM_SAVE_COLLECTION* pOutParam, int nWaitTime = 1000);

// get plan of monitor wall
CLIENT_API BOOL CALL_METHOD CLIENT_GetMonitorWallCollections(LLONG lLoginID, const DH_IN_WM_GET_COLLECTIONS* pInParam, DH_OUT_WM_GET_COLLECTIONS* pOutParam, int nWaitTime = 1000);

// rename monitor wall's plan
CLIENT_API BOOL	CALL_METHOD CLIENT_RenameMonitorWallCollection(LLONG lLoginID, const DH_IN_WM_RENAME_COLLECTION* pInParam, DH_OUT_WM_RENAME_COLLECTION* pOutParam, int nWaitTime = 1000);

// get or set scene of monitor wall 
CLIENT_API BOOL CALL_METHOD CLIENT_MonitorWallGetScene(LLONG lLoginID, const DH_IN_MONITORWALL_GET_SCENE* pInParam, DH_OUT_MONITORWALL_GET_SCENE* pOutParam, int nWaitTime = 1000);
CLIENT_API BOOL CALL_METHOD CLIENT_MonitorWallSetScene(LLONG lLoginID, const DH_IN_MONITORWALL_SET_SCENE* pInParam, DH_OUT_MONITORWALL_SET_SCENE* pOutParam, int nWaitTime = 1000);

// get attribute caps of monitor wall
CLIENT_API BOOL CALL_METHOD CLIENT_MonitorWallGetAttributeCaps(LLONG lLoginID, const DH_IN_MONITORWALL_GET_ARRT_CAPS* pInParam, DH_OUT_MONITORWALL_GET_ARRT_CAPS* pOutParam, int nWaitTime = 1000);

// auto adjust of monitor wall
CLIENT_API BOOL CALL_METHOD CLIENT_MonitorWallAutoAdjust(LLONG lLoginID, const DH_IN_MONITORWALL_AUTO_ADJUST* pInParam, DH_OUT_MONITORWALL_AUTO_ADJUST* pOutParam, int nWaitTime = 1000);

// set attribute of monitor wall 
CLIENT_API BOOL CALL_METHOD CLIENT_MonitorWallSetAttribute(LLONG lLoginID, const DH_IN_MONITORWALL_SET_ATTR* pInParam, DH_OUT_MONITORWALL_SET_ATTR* pOutParam, int nWaitTime = 1000);

// set mode of backlight
CLIENT_API BOOL CALL_METHOD CLIENT_MonitorWallSetBackLight(LLONG lLoginID, const DH_IN_MONITORWALL_SET_BACK_LIGHT* pInParam, DH_OUT_MONITORWALL_SET_BACK_LIGHT* pOutParam, int nWaitTime = 1000);

// ��ѯ/������Ļ���ؼƻ�
CLIENT_API BOOL CALL_METHOD CLIENT_MonitorWallGetPowerSchedule(LLONG lLoginID, const NET_IN_MW_GET_POWER_SCHEDULE* pInParam, NET_OUT_MW_GET_POWER_SCHEDULE* pOutParam, int nWaitTime);
CLIENT_API BOOL CALL_METHOD CLIENT_MonitorWallSetPowerSchedule(LLONG lLoginID, const NET_IN_MW_SET_POWER_SCHEDULE* pInParam, NET_OUT_MW_SET_POWER_SCHEDULE* pOutParam, int nWaitTime);

// ��ѯ/������Ļ���Ʋ���
CLIENT_API BOOL CALL_METHOD CLIENT_MonitorWallGetScrnCtrlParam(LLONG lLoginID, const NET_IN_MW_GET_SCRN_CTRL_PARAM* pInParam, NET_OUT_MW_GET_SCRN_CTRL_PARAM* pOutParam, int nWaitTime);
CLIENT_API BOOL CALL_METHOD CLIENT_MonitorWallSetScrnCtrlParam(LLONG lLoginID, const NET_IN_MW_SET_SCRN_CTRL_PARAM* pInParam, NET_OUT_MW_SET_SCRN_CTRL_PARAM* pOutParam, int nWaitTime);

// ��ѯ/������Ļ�ʹ��ڱ�����ɫ
CLIENT_API BOOL CALL_METHOD CLIENT_MonitorWallGetBackgroudColor(LLONG lLoginID, const NET_IN_MW_GET_BACKGROUDND_COLOR* pInParam, NET_OUT_MW_GET_BACKGROUDND_COLOR* pOutParam, int nWaitTime);
CLIENT_API BOOL CALL_METHOD CLIENT_MonitorWallSetBackgroudColor(LLONG lLoginID, const NET_IN_MW_SET_BACKGROUD_COLOR* pInParam, NET_OUT_MW_SET_BACKGROUD_COLOR* pOutParam, int nWaitTime);

///////////////////////////////// directory management /////////////////////////////////////////

// add nodes
CLIENT_API BOOL CALL_METHOD CLIENT_OrganizationAddNodes(LLONG lLoginID, const DH_IN_ORGANIZATION_ADD_NODES* pInParam, DH_OUT_ORGANIZATION_ADD_NODES* pOutParam, int nWaitTime = 1000);

// delete nodes
CLIENT_API BOOL CALL_METHOD CLIENT_OrganizationDeleteNodes(LLONG lLoginID, const DH_IN_ORGANIZATION_DELETE_NODES* pInParam, DH_OUT_ORGANIZATION_DELETE_NODES* pOutParam, int nWaitTime = 1000);

// get info of nodes
CLIENT_API BOOL CALL_METHOD CLIENT_OrganizationGetNodes(LLONG lLoginID, const DH_IN_ORGANIZATION_GET_NODES* pInParam, DH_OUT_ORGANIZATION_GET_NODES* pOutParam, int nWaitTime = 1000);

//set node
CLIENT_API BOOL CALL_METHOD CLIENT_OrganizationSetNode(LLONG lLoginID, const DH_IN_ORGANIZATION_SET_NODE* pInParam, DH_OUT_ORGANIZATION_SET_NODE* pOutParam, int nWaitTime = 1000);


//////////////////////////////// network caught /////////////////////////////////

// start caught
CLIENT_API LLONG CALL_METHOD CLIENT_StartSniffer(LLONG lLoginID, const DH_IN_START_SNIFFER* pInParam, DH_OUT_START_SNIFFER* pOutParam, int nWaitTime = 1000);

// stop caught
CLIENT_API BOOL CALL_METHOD CLIENT_StopSniffer(LLONG lLoginID, LLONG lSnifferID);

// get caught state
CLIENT_API BOOL CALL_METHOD CLIENT_GetSnifferInfo(LLONG lLoginID, const DH_IN_GET_SNIFFER_INFO* pInParam, DH_OUT_GET_SNIFFER_INFO* pOutParam, int nWaitTime = 1000);


//////////////////////////////// manage remote file /////////////////////////////////

// create file
CLIENT_API BOOL CALL_METHOD CLIENT_CreateRemoteFile(LLONG lLoginID, const DH_IN_CREATE_REMOTE_FILE* pInParam, DH_OUT_CREATE_REMOTE_FILE* pOutParam, int nWaitTime = 1000);

// remove file
CLIENT_API BOOL CALL_METHOD CLIENT_RemoveRemoteFiles(LLONG lLoginID, const DH_IN_REMOVE_REMOTE_FILES* pInParam, DH_OUT_REMOVE_REMOTE_FILES* pOutParam, int nWaitTime = 1000);

// rename
CLIENT_API BOOL CALL_METHOD CLIENT_RenameRemoteFile(LLONG lLoginID, const DH_IN_RENAME_REMOTE_FILE* pInParam, DH_OUT_RENAME_REMOTE_FILE* pOutParam, int nWaitTime = 1000);

// display files and subdirectories in a directory
CLIENT_API BOOL CALL_METHOD CLIENT_ListRemoteFile(LLONG lLoginID, const DH_IN_LIST_REMOTE_FILE* pInParam, DH_OUT_LIST_REMOTE_FILE* pOutParam, int nWaitTime = 1000);

// sybcgribize fule upload
CLIENT_API BOOL CALL_METHOD CLIENT_UploadRemoteFile(LLONG lLoginID, const DH_IN_UPLOAD_REMOTE_FILE* pInParam, DH_OUT_UPLOAD_REMOTE_FILE* pOutParam, int nWaitTime = 1000);

// Զ��Ͷ��, ���豸�˲�����Ƶ�ļ�
CLIENT_API BOOL CALL_METHOD CLIENT_PlayAudioFile(LLONG lLoginID, const NET_IN_PLAY_AUDIO_FILE* pInParam, NET_OUT_PLAY_AUDIO_FILE* pOutParam, int nWaitTime = 1000);

// �ļ�����, ֻ������С�ļ�
CLIENT_API BOOL CALL_METHOD CLIENT_DownloadRemoteFile(LLONG lLoginID, const DH_IN_DOWNLOAD_REMOTE_FILE* pInParam, DH_OUT_DOWNLOAD_REMOTE_FILE* pOutParam, int nWaitTime = 1000);

////////////////////////////////// manage storage device ////////////////////////////////////////

// Get ISCSI target list
CLIENT_API BOOL CALL_METHOD	CLIENT_GetISCSITargets(LLONG lLoginID, const DH_IN_ISCSI_TARGETS* pInParam, DH_OUT_ISCSI_TARGETS* pOutParam, int nWaitTime = 1000);

CLIENT_API BOOL CALL_METHOD CLIENT_GetBitmap(LLONG lLoginID , const DH_IN_BITMAP* pInParam, DH_OUT_BITMAP* pOutParam, int nWaitTime = 1000);

// Get storage device name list
CLIENT_API BOOL CALL_METHOD	CLIENT_GetStorageDeviceNames(LLONG lLoginID, DH_STORAGE_DEVICE_NAME* pstuNames, int nMaxCount, int* pnRetCount, int nWaitTime = 1000);

// Get storage device info
CLIENT_API BOOL CALL_METHOD	CLIENT_GetStorageDeviceInfo(LLONG lLoginID, const char* pszDevName, DH_STORAGE_DEVICE* pDevice, int nWaitTime = 1000);

// ����¼���ļ�������Ϣ
CLIENT_API LLONG CALL_METHOD CLIENT_AttachRecordInfo(LLONG lLoginID, const NET_IN_ATTACH_RECORD_INFO* pInParam, NET_OUT_ATTACH_RECORD_INFO* pOutParam, int nWaitTime = 1000);

// ȡ������¼���ļ�������Ϣ��lAttachHandle��CLIENT_AttachRecordInfo�ķ���ֵ
CLIENT_API BOOL CALL_METHOD CLIENT_DetachRecordInfo(LLONG lAttachHandle);

// ����д��Զ�̴洢����Ϣ״̬
CLIENT_API LLONG CALL_METHOD CLIENT_NetStorageAttachWriteInfo(LLONG lLoginID, const NET_IN_STORAGE_ATTACH_WRITE_INFO* pInParam, NET_OUT_STORAGE_ATTACH_WRITE_INFO* pOutParam, int nWaitTime);

// ȡ��д��Զ���豸��Ϣ��lAttachHandle��CLIENT_NetStorageAttachWriteInfo�ķ���ֵ
CLIENT_API BOOL CALL_METHOD CLIENT_NetStorageDetachWriteInfo(LLONG lAttachHandle);

// ��ѯԶ�̴洢��д����Ϣ״̬
CLIENT_API BOOL CALL_METHOD CLIENT_NetStorageGetWriteInfo(LLONG lLoginID, const NET_IN_STORAGE_GET_WRITE_INFO* pInParam, NET_OUT_STORAGE_GET_WRITE_INFO* pOutParam, int nWaitTime);

// RAID����, ��ͬ�������Ͷ�Ӧ��ͬ�Ľṹ��
CLIENT_API BOOL CALL_METHOD CLIENT_OperateRaid(LLONG lLoginID, NET_RAID_OPERATE_TYPE emType, void* pInBuf, void* pOutBuf, int nWaitTime);

/////////////////////////////////// cascade device ///////////////////////////////////////

// search cascade video
CLIENT_API BOOL CALL_METHOD CLIENT_MatrixSearch(LLONG lLoginID, const DH_IN_MATRIX_SEARCH* pInParam, DH_OUT_MATRIX_SEARCH* pOutParam, int nWaitTime = 1000);

// get cascade tree
CLIENT_API BOOL CALL_METHOD CLIENT_GetMatrixTree(LLONG lLoginID, const DH_IN_GET_MATRIX_TREE* pInParam, DH_OUT_GET_MATRIX_TREE* pOutParam, int nWaitTime = 1000);

// get superior cascade device list info
CLIENT_API BOOL CALL_METHOD CLIENT_GetSuperiorMatrixList(LLONG lLoginID, const DH_IN_GET_SUPERIOR_MATRIX_LIST* pInParam, DH_OUT_GET_SUPERIOR_MATRIX_LIST* pOutParam, int nWaitTime = 1000);

/************************************************************************/
/*							backup record comes back
/************************************************************************/

// start record backup restore
CLIENT_API LLONG CALL_METHOD CLIENT_StartRecordBackupRestore(LLONG lLoginID);

// stop record backup restore
CLIENT_API void CALL_METHOD CLIENT_StopRecordBackupRestore(LLONG lRestoreID);

// add record backup restore task
CLIENT_API BOOL CALL_METHOD CLIENT_AddRecordBackupRestoreTask(LLONG lRestoreID, const DH_IN_ADD_REC_BAK_RST_TASK* pInParam, int nWaitTime = 1000);

// remove record backup restore task
CLIENT_API BOOL CALL_METHOD CLIENT_RemoveRecordBackupRestoreTask(LLONG lRestoreID, const DH_IN_REMOVE_REC_BAK_RST_TASK* pInParam, int nWaitTime = 1000);

// get record backup restore task
CLIENT_API BOOL CALL_METHOD CLIENT_QueryRecordBackupRestoreTask(LLONG lRestoreID, const DH_IN_QUERY_REC_BAK_RST_TASK* pInParam, DH_OUT_QUERY_REC_BAK_RST_TASK* pOutParam, int nWaitTime = 1000);

//////////////////////////////// Encode Manager  ////////////////////////////////
// judicial burn in plan parameter coding
CLIENT_API BOOL CALL_METHOD CLIENT_GetEncodePlan(LLONG lLoginID, const DH_IN_GET_ENCODE_PLAN* pInParam, DH_OUT_GET_ENCODE_PLAN* pOutParam, int nWaitTime = 1000);

/************************************************************************/
/*                           ���ݿ��¼��ز���                         */
/************************************************************************/
// ����ѯ������ѯ��¼
CLIENT_API BOOL    CALL_METHOD CLIENT_FindRecord(LLONG lLoginID, NET_IN_FIND_RECORD_PARAM* pInParam, NET_OUT_FIND_RECORD_PARAM* pOutParam, int waittime=1000);    

// ���Ҽ�¼:nFilecount:��Ҫ��ѯ������, ����ֵΪý���ļ����� ����ֵ<nFilecount����Ӧʱ����ڵ��ļ���ѯ���
CLIENT_API int    CALL_METHOD CLIENT_FindNextRecord(NET_IN_FIND_NEXT_RECORD_PARAM* pInParam, NET_OUT_FIND_NEXT_RECORD_PARAM* pOutParam, int waittime=1000);

// ���Ҽ�¼����
CLIENT_API BOOL    CALL_METHOD CLIENT_QueryRecordCount(NET_IN_QUEYT_RECORD_COUNT_PARAM* pInParam, NET_OUT_QUEYT_RECORD_COUNT_PARAM* pOutParam, int waittime=1000);

// ������¼����
CLIENT_API BOOL CALL_METHOD CLIENT_FindRecordClose(LLONG lFindHandle);

/************************************************************************/
/*                            ��̨Ԫ���ݽӿڶ���
/************************************************************************/
// ������̨Ԫ���ݽӿ�
CLIENT_API LLONG CALL_METHOD CLIENT_AttachPTZStatusProc(LLONG lLoginID, NET_IN_PTZ_STATUS_PROC *pstuInPtzStatusProc,  NET_OUT_PTZ_STATUS_PROC *pstuOutPtzStatusProc, int nWaitTime = 3000);

// ֹͣ������̨Ԫ���ݽӿڣ�lAttachHandle��CLIENT_AttachPTZStatusProc����ֵ
CLIENT_API BOOL    CALL_METHOD    CLIENT_DetachPTZStatusProc(LLONG lAttachHandle);

/************************************************************************/
/*                            ��̨��������
/************************************************************************/
// ������̨������
CLIENT_API LLONG CALL_METHOD CLIENT_AttachViewRangeState(LLONG lLoginID, NET_IN_VIEW_RANGE_STATE *pstuInViewRange, NET_OUT_VIEW_RANGE_STATE *pstuOutViewRange, int nWaitTime = 3000);

// ֹͣ������̨������lAttachHandle��CLIENT_AttachViewRangeState����ֵ
CLIENT_API BOOL    CALL_METHOD    CLIENT_DetachViewRangeState(LLONG lAttachHandle);

/************************************************************************/
/*                            ģ��������ͨ�����ݶ���
/************************************************************************/

// ����ģ��������ͨ������
CLIENT_API LLONG CALL_METHOD CLIENT_AttachAnalogAlarmData(LLONG lLoginID, const NET_IN_ANALOGALARM_DATA* pInParam, NET_OUT_ANALOGALARM_DATA* pOutParam, int nWaitTime);

// ֹͣ����ģ��������ͨ������
CLIENT_API BOOL CALL_METHOD CLIENT_DetachAnalogAlarmData(LLONG lAttachHandle);

////////////////////////////Special Version Interface///////////////////////////////
// Search device log--extensive
CLIENT_API BOOL CALL_METHOD CLIENT_QueryLogEx(LLONG lLoginID, DH_LOG_QUERY_TYPE logType, char *pLogBuffer, int maxlen, int *nLogBufferlen, void* reserved, int waittime=3000);

// Active registered redirect function,establish directed connections
CLIENT_API LONG CALL_METHOD CLIENT_ControlConnectServer(LLONG lLoginID, char* RegServerIP, WORD RegServerPort, int TimeOut=3000);

// Establish active registered connection
CLIENT_API BOOL CALL_METHOD CLIENT_ControlRegisterServer(LLONG lLoginID, LONG ConnectionID, int waittime=1000);

// Disconnected directional connection
CLIENT_API BOOL CALL_METHOD CLIENT_ControlDisconnectRegServer(LLONG lLoginID, LONG ConnectionID);

// Query active registered server information
CLIENT_API BOOL CALL_METHOD CLIENT_QueryControlRegServerInfo(LLONG lLoginID, LPDEV_SERVER_AUTOREGISTER lpRegServerInfo, int waittime=2000);

// Upload file
CLIENT_API LLONG CALL_METHOD CLIENT_FileTransmit(LLONG lLoginID, int nTransType, char* szInBuf, int nInBufLen, fTransFileCallBack cbTransFile, LDWORD dwUserData, int waittime);

// web info trasmit
CLIENT_API BOOL  CALL_METHOD CLIENT_TransmitInfoForWeb(LLONG lLoginID, char* szInBuffer, DWORD dwInBufferSize, char* szOutBuffer, DWORD dwOutBufferSize, void* pExtData, int waittime=500);

// watermark verify for picture *nResult = 0-means no verify *nResult = 1-means has verify
CLIENT_API BOOL  CALL_METHOD CLIENT_WatermarkVerifyForPicture(char* szFilePath, int* nResult, void* pReserved);

// multi realplay
CLIENT_API BOOL  CALL_METHOD CLIENT_MultiRealPlay(LLONG lLoginID, DHDEV_IN_MULTIPLAY_PARAM* pInBuf, int nInBufLen, DHDEV_OUT_MULTIPLAY_PARAM* pOutBuf, int nOutBufLen, int* pRetLen);
 
// stop multi realplay
CLIENT_API BOOL  CALL_METHOD CLIENT_StopMultiRealPlay(LLONG* lRealHandles, int nNumOfHandles);

// when hwnd != null,set playback yuv data callback 
CLIENT_API BOOL CALL_METHOD CLIENT_SetPlaybackYUVCallBack(LLONG lPlayHandle, fYUVDataCallBack cYUVData, LDWORD dwUser);

// get web configuration
CLIENT_API BOOL CALL_METHOD CLIENT_GetNewDevConfigForWeb(LLONG lLoginID, char* szCommand, int nChannelID, char* szOutBuffer, DWORD dwOutBufferSize, int *error, int waittime=500);

// set configuration for web
CLIENT_API BOOL CALL_METHOD CLIENT_SetNewDevConfigForWeb(LLONG lLoginID, char* szCommand, int nChannelID, char* szInBuffer, DWORD dwInBufferSize, int *error, int *restart, int waittime=500);

// �����˿�ӳ��
CLIENT_API int CALL_METHOD CLIENT_CreateOneTunnel(LOSN_IN_CREATE_TUNNEL_PARAM pInParam, LOSN_OUT_CREATE_TUNNEL_PARAM pOutParam, int waittime=1000);

// ɾ���˿�ӳ��
CLIENT_API int CALL_METHOD CLIENT_DestroyOneTunnel(LOSN_IN_DESTROY_TUNNEL_PARAM pInParam);

// ���ò��Ų��Բ�����ֻ��ʵʱ����Ч
CLIENT_API BOOL CALL_METHOD CLIENT_SetPlayMethod(LLONG lRealHandle, int nStartTime, int nSlowTime, int nFastTime, int nFailedTime);

// �ر��豸����ע������������
CLIENT_API BOOL CALL_METHOD CLIENT_CloseRegConnect(LLONG lHandle, char *pIp, WORD wPort, void *pParam);

//////�Ϻ�BUS//////

#ifdef SHANGHAIBUS
// �������ݽ����ӿ�,�첽��ȡ����
CLIENT_API LLONG CALL_METHOD CLIENT_ExChangeData(LLONG lLoginId, NET_IN_EXCHANGEDATA* pInParam, NET_OUT_EXCHANGEDATA* pOutParam, int nWaittime = 5000);

// ����CAN��������
CLIENT_API LLONG CALL_METHOD CLIENT_AttachCAN(LLONG lLoginID, const NET_IN_ATTACH_CAN* pstInParam, NET_OUT_ATTACH_CAN* pstOutParam, int nWaitTime = 3000);

// ȡ������CAN�������ݣ�lAttachHandle��CLIENT_AttachCAN����ֵ
CLIENT_API BOOL CALL_METHOD CLIENT_DetachCAN(LLONG lAttachHandle);
#endif

/////////////////////////////////Cancelled Interface/////////////////////////////////

// Search system server setup. This interface is invalid now please use  CLIENT_GetDevConfig
CLIENT_API BOOL CALL_METHOD CLIENT_QueryConfig(LLONG lLoginID, int nConfigType, char *pConfigbuf, int maxlen, int *nConfigbuflen, int waittime=1000);

// Set system server setup. This interface is invalid now please use  CLIENT_SetDevConfig
CLIENT_API BOOL CALL_METHOD CLIENT_SetupConfig(LLONG lLoginID, int nConfigType, char *pConfigbuf, int nConfigbuflen, int waittime=1000);

// This interface is invalid now. 
CLIENT_API BOOL CALL_METHOD CLIENT_Reset(LLONG lLoginID, BOOL bReset);

// Search COM protocol. This interface is invalid now please use  CLIENT_GetDevConfig
CLIENT_API BOOL CALL_METHOD CLIENT_QueryComProtocol(LLONG lLoginID, int nProtocolType, char *pProtocolBuffer, int maxlen, int *nProtocollen, int waittime=1000);

// Begin audio talk. This interface is invalid now. Please use  CLIENT_StartTalkEx
CLIENT_API BOOL CALL_METHOD CLIENT_StartTalk(LLONG lRealHandle, BOOL bCustomSend=false);

// Stop audio talk. This interface is invalid now , please use  CLIENT_StopTalkEx
CLIENT_API BOOL CALL_METHOD CLIENT_StopTalk(LLONG lRealHandle);

// Send out self-defined audio talk data. This interface is invalid now, please use  CLIENT_TalkSendData
CLIENT_API BOOL CALL_METHOD CLIENT_SendTalkData_Custom(LLONG lRealHandle, char *pBuffer, DWORD dwBufSize);

// Set real-time preview buffer size
CLIENT_API BOOL CALL_METHOD CLIENT_SetPlayerBufNumber(LLONG lRealHandle, DWORD dwBufNum);

// Download file by time
CLIENT_API BOOL CALL_METHOD CLIENT_GetFileByTime(LLONG lLoginID, int nChannelID, LPNET_TIME lpStartTime, LPNET_TIME lpStopTime, char *sSavedFileName);

// Network playback control 
CLIENT_API BOOL CALL_METHOD CLIENT_PlayBackControl(LLONG lPlayHandle, DWORD dwControlCode, DWORD dwInValue, DWORD *lpOutValue);

// Search device working status .This interface is invalid now, please use  CLIENT_QueryDevState
CLIENT_API BOOL CALL_METHOD CLIENT_GetDEVWorkState(LLONG lLoginID, LPNET_DEV_WORKSTATE lpWorkState, int waittime=1000);

// Asynchronism search device log 
CLIENT_API BOOL CALL_METHOD CLIENT_QueryLogCallback(LLONG lLoginID, fLogDataCallBack cbLogData, LDWORD dwUser);


#ifdef __cplusplus
}
#endif
#endif // DHNETSDK_H





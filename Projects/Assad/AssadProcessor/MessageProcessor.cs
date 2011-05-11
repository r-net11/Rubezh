using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Windows.Forms;

namespace AssadProcessor
{
    public class MessageProcessor
    {
        object GetMessageContent(string message, out string messageId)
        {
            try
            {
                byte[] bytes = Encoding.UTF8.GetBytes(message);
                MemoryStream memoryStream = new MemoryStream(bytes);
                XmlSerializer serializer = new XmlSerializer(typeof(Assad.MessageType));
                Assad.MessageType messageType = (Assad.MessageType)serializer.Deserialize(memoryStream);
                memoryStream.Close();

                messageId = messageType.msgId;

                return messageType.Item;
            }
            catch(Exception ex)
            {
                string s = message;
                throw new InvalidOperationException();
            }
        }

        internal void ProcessMessage(string message)
        {
            if (message.StartsWith("\0"))
            {
                return;
            }
            string refMessageId;
            object content = GetMessageContent(message, out refMessageId);
            string messageType = content.GetType().Name;

            OnNewMessage(messageType);

            switch (messageType)
            {
                case "MHqueryCPType":
                    Assad.modelInfoType RootModelInfo = Services.DeviceModelManager.RootModelInfo;

                    Assad.CPconfirmationType confirmation = new Assad.CPconfirmationType();
                    confirmation.commandId = "MHqueryCP";
                    confirmation.Items = new Assad.modelInfoType[1];
                    confirmation.Items[0] = RootModelInfo;

                    NetManager.Send(confirmation, refMessageId);
                    break;

                case "MHqueryConfiguredDevType":
                    confirmation = new Assad.CPconfirmationType();
                    confirmation.commandId = "MHqueryConfiguredDev";

                    NetManager.Send(confirmation, refMessageId);
                    break;

                case "MHconfigType":
                    Assad.MHconfigTypeDevice device = (content as Assad.MHconfigType).device;
                    Controller.Current.AssadConfig(device, (content as Assad.MHconfigType).all);
                    Controller.Current.Ready = true;

                    confirmation = new Assad.CPconfirmationType();
                    confirmation.commandId = "MHconfig";
                    confirmation.status = Assad.CommandStatus.OK;
                    
                    NetManager.Send(confirmation, refMessageId);
                    break;

                case "MHqueryStateType":
                    Assad.DeviceType[] deviceItems = Controller.Current.QueryState((Assad.MHqueryStateType)content);

                    confirmation = new Assad.CPconfirmationType();
                    confirmation.commandId = "MHqueryState";
                    confirmation.status = Assad.CommandStatus.OK;
                    confirmation.Items = deviceItems;

                    NetManager.Send(confirmation, refMessageId);
                    break;

                case "MHpingType":
                    confirmation = new Assad.CPconfirmationType();
                    confirmation.commandId = "MHPing";

                    NetManager.Send(confirmation, refMessageId);
                    break;

                case "MHdeviceControlType":
                    Controller.Current.AssadExecuteCommand((Assad.MHdeviceControlType)content);

                    confirmation = new Assad.CPconfirmationType();
                    confirmation.commandId = "MHDeviceControl";
                    confirmation.status = Assad.CommandStatus.OK;
                    NetManager.Send(confirmation, refMessageId);
                    break;

                case "MHremoveDeviceType":
                    confirmation = new Assad.CPconfirmationType();
                    confirmation.commandId = "MHremoveDeviceType";

                    NetManager.Send(confirmation, refMessageId);
                    break;

                case "MHqueryAbilityType":
                    confirmation = new Assad.CPconfirmationType();
                    confirmation.commandId = "MHqueryAbilityType";

                    Assad.DeviceType deviceAbility = Controller.Current.QueryAbility((Assad.MHqueryAbilityType)content);

                    confirmation.Items = new Assad.DeviceType[1];
                    confirmation.Items[0] = deviceAbility;

                    NetManager.Send(confirmation, refMessageId);
                    break;

                case "MHconfirmAlarmType":
                    confirmation = new Assad.CPconfirmationType();
                    confirmation.commandId = "MHconfirmAlarm";
                    confirmation.status = Assad.CommandStatus.OK;

                    NetManager.Send(confirmation, refMessageId);
                    break;

                case "MHresetAlarmType":
                    Assad.MHresetAlarmType mHresetAlarmType = (Assad.MHresetAlarmType)content;
                    Controller.Current.ResetAllStates(mHresetAlarmType.deviceId);

                    confirmation = new Assad.CPconfirmationType();
                    confirmation.commandId = "MHresetAlarm";
                    confirmation.status = Assad.CommandStatus.OK;

                    NetManager.Send(confirmation, refMessageId);
                    break;

                case "MHconfirmationType":
                    break;

                case "":
                    MessageBox.Show("Отсутствует Идентификатор Сообщения");
                    break;

                default:
                    MessageBox.Show("Неизвестный Идентификатор Сообщения");
                    break;
            }
        }

        public static event Action<string> NewMessage;
        static void OnNewMessage(string message)
        {
            if (NewMessage != null)
                NewMessage(message);
        }
    }
}

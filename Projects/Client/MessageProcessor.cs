using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml.Serialization;

namespace Client
{
    public class MessageProcessor
    {
        public object GetMessageContent(string message, out string messageId)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(message);
            MemoryStream memoryStream = new MemoryStream(bytes);

            XmlSerializer serializer = new XmlSerializer(typeof(x.MessageType));
            x.MessageType messageType = (x.MessageType)serializer.Deserialize(memoryStream);
            memoryStream.Close();

            messageId = messageType.msgId;

            return messageType.Item;
        }

        public void ProcessMessage(string message)
        {
            string refMessageId;
            object content = GetMessageContent(message, out refMessageId);
            string messageType = content.GetType().Name;

            switch (messageType)
            {
                case "MHqueryCPType":
                    x.modelInfoType RootModelInfo = DeviceTypesManager.RootModelInfo;

                    x.CPconfirmationType confirmation = new x.CPconfirmationType();
                    confirmation.commandId = "MHqueryCP";
                    confirmation.Items = new x.modelInfoType[1];
                    confirmation.Items[0] = RootModelInfo;

                    NetManager.Send(confirmation, refMessageId);
                    break;

                case "MHqueryConfiguredDevType":
                    confirmation = new x.CPconfirmationType();
                    confirmation.commandId = "MHqueryConfiguredDev";

                    NetManager.Send(confirmation, refMessageId);
                    break;

                case "MHconfigType":
                    x.MHconfigTypeDevice device = (content as x.MHconfigType).device;
                    MainForm.Config(device, (content as x.MHconfigType).all);

                    confirmation = new x.CPconfirmationType();
                    confirmation.commandId = "MHconfig";
                    confirmation.status = x.CommandStatus.OK;
                    
                    NetManager.Send(confirmation, refMessageId);
                    break;

                case "MHqueryStateType":
                    x.DeviceType[] deviceItems = DeviceManager.QueryState((x.MHqueryStateType)content, refMessageId);

                    confirmation = new x.CPconfirmationType();
                    confirmation.commandId = "MHqueryState";
                    confirmation.status = x.CommandStatus.OK;
                    confirmation.Items = deviceItems;

                    NetManager.Send(confirmation, refMessageId);
                    break;

                case "MHpingType":
                    confirmation = new x.CPconfirmationType();
                    confirmation.commandId = "MHPing";

                    NetManager.Send(confirmation, refMessageId);
                    break;

                case "MHdeviceControlType":
                    DeviceManager.ExecuteCommand((x.MHdeviceControlType)content, refMessageId);

                    confirmation = new x.CPconfirmationType();
                    confirmation.commandId = "MHDeviceControl";
                    confirmation.status = x.CommandStatus.OK;
                    NetManager.Send(confirmation, refMessageId);
                    break;

                case "MHremoveDeviceType":
                    MainForm.RemoveDevice((x.MHremoveDeviceType)content, refMessageId);
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
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace Client
{
    public class ProtocolEnglish
    {
        public string CreateMessage(string refMessageId)
        {
            x.MessageType messageType = CreateMessageType(refMessageId);

            XmlSerializer serializer = new XmlSerializer(typeof(x.MessageType));
            MemoryStream outMemoryStream = new MemoryStream();
            serializer.Serialize(outMemoryStream, messageType);
            byte[] outBytes = outMemoryStream.ToArray();
            outMemoryStream.Close();
            string outString = Encoding.ASCII.GetString(outBytes).Trim();
            //outString = outString.Remove(0, outString.IndexOf(">") + 3);
            outString = outString.Replace("<?xml version=\"1.0\"?>", "<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            outString = outString.Replace("xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" ", "");
            return outString;
        }

        public x.MessageType CreateMessageType(string refMessageId)
        {
            x.modelInfoType modelInfoType = CreateModelType();

            x.CPconfirmationType cPconfirmationType = new x.CPconfirmationType();
            cPconfirmationType.commandId = "MHqueryCP";
            cPconfirmationType.Items = new object[] { modelInfoType };

            x.MessageType messageType = new x.MessageType();
            messageType.msgId = "TestMessage.Rubezh.1";
            messageType.msgTime = DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss");
            messageType.refMsg = refMessageId;
            messageType.Item = cPconfirmationType;

            return messageType;
        }

        public x.modelInfoType CreateModelType()
        {
            x.modelInfoType modelInfoType = new x.modelInfoType();

            modelInfoType.name = "TestLine 485";
            modelInfoType.model = "1.0";
            modelInfoType.type1 = "TestRubezh.port.485";

            modelInfoType.param = AddParameters();

            modelInfoType.command = AddCommands();

            modelInfoType.state = AddStates();

            modelInfoType.@event = AddEvents();

            return modelInfoType;
        }

        public x.modelInfoTypeParam[] AddParameters()
        {
            x.modelInfoTypeParam modelInfoTypeParam_1 = new x.modelInfoTypeParam();
            modelInfoTypeParam_1.@default = "3";
            modelInfoTypeParam_1.param = "speed";
            modelInfoTypeParam_1.type = "single";

            x.modelInfoTypeParamValue modelInfoTypeParamValue_1_1 = new x.modelInfoTypeParamValue();
            modelInfoTypeParamValue_1_1.value = "1200";
            x.modelInfoTypeParamValue modelInfoTypeParamValue_1_2 = new x.modelInfoTypeParamValue();
            modelInfoTypeParamValue_1_2.value = "2400";
            x.modelInfoTypeParamValue modelInfoTypeParamValue_1_3 = new x.modelInfoTypeParamValue();
            modelInfoTypeParamValue_1_3.value = "4800";
            x.modelInfoTypeParamValue modelInfoTypeParamValue_1_4 = new x.modelInfoTypeParamValue();
            modelInfoTypeParamValue_1_4.value = "9600";

            modelInfoTypeParam_1.value = new x.modelInfoTypeParamValue[] { modelInfoTypeParamValue_1_1, modelInfoTypeParamValue_1_2, modelInfoTypeParamValue_1_3, modelInfoTypeParamValue_1_4 };


            x.modelInfoTypeParam modelInfoTypeParam_2 = new x.modelInfoTypeParam();
            modelInfoTypeParam_2.@default = "COM1";
            modelInfoTypeParam_2.param = "port";
            modelInfoTypeParam_2.type = "edit";

            return new x.modelInfoTypeParam[] { modelInfoTypeParam_1, modelInfoTypeParam_2 };
        }

        public x.modelInfoTypeCommand[] AddCommands()
        {
            x.modelInfoTypeCommand modelInfoTypeCommand_1 = new x.modelInfoTypeCommand();
            modelInfoTypeCommand_1.command = "command 1";
            modelInfoTypeCommand_1.order = "1";
            x.modelInfoTypeCommandParam modelInfoTypeCommandParam = new x.modelInfoTypeCommandParam();
            modelInfoTypeCommandParam.param = "parameter1";
            modelInfoTypeCommand_1.param = new x.modelInfoTypeCommandParam[] { modelInfoTypeCommandParam };

            x.modelInfoTypeCommand modelInfoTypeCommand_2 = new x.modelInfoTypeCommand();
            modelInfoTypeCommand_2.command = "command 2";
            modelInfoTypeCommand_2.order = "2";

            x.modelInfoTypeCommand modelInfoTypeCommand_3 = new x.modelInfoTypeCommand();
            modelInfoTypeCommand_3.command = "command 3";
            modelInfoTypeCommand_3.order = "3";

            return new x.modelInfoTypeCommand[] { modelInfoTypeCommand_1, modelInfoTypeCommand_2, modelInfoTypeCommand_3 };
        }

        public x.modelInfoTypeState[] AddStates()
        {
            x.modelInfoTypeState modelInfoTypeState_1 = new x.modelInfoTypeState();
            modelInfoTypeState_1.state = "error";

            x.modelInfoTypeStateValue modelInfoTypeStateValue_1_1 = new x.modelInfoTypeStateValue();
            modelInfoTypeStateValue_1_1.@value = "yes";
            x.modelInfoTypeStateValue modelInfoTypeStateValue_1_2 = new x.modelInfoTypeStateValue();
            modelInfoTypeStateValue_1_2.@value = "no";
            x.modelInfoTypeStateValue modelInfoTypeStateValue_1_3 = new x.modelInfoTypeStateValue();
            modelInfoTypeStateValue_1_3.@value = "information";
            modelInfoTypeState_1.@value = new x.modelInfoTypeStateValue[] { modelInfoTypeStateValue_1_1, modelInfoTypeStateValue_1_2, modelInfoTypeStateValue_1_3 };

            x.modelInfoTypeState modelInfoTypeState_2 = new x.modelInfoTypeState();
            modelInfoTypeState_2.state = "LineState";

            x.modelInfoTypeStateValue modelInfoTypeStateValue_2_1 = new x.modelInfoTypeStateValue();
            modelInfoTypeStateValue_2_1.@value = "norm";
            x.modelInfoTypeStateValue modelInfoTypeStateValue_2_2 = new x.modelInfoTypeStateValue();
            modelInfoTypeStateValue_2_2.@value = "error";
            modelInfoTypeState_2.@value = new x.modelInfoTypeStateValue[] { modelInfoTypeStateValue_2_1, modelInfoTypeStateValue_2_2 };

            return new x.modelInfoTypeState[] { modelInfoTypeState_1, modelInfoTypeState_2 };
        }

        public x.modelInfoTypeEvent[] AddEvents()
        {
            x.modelInfoTypeEvent modelInfoTypeEvent_1 = new x.modelInfoTypeEvent();
            modelInfoTypeEvent_1.@event = "TestEvent 1";

            x.modelInfoTypeEvent modelInfoTypeEvent_2 = new x.modelInfoTypeEvent();
            modelInfoTypeEvent_2.@event = "TestEvent 2";

            x.modelInfoTypeEvent modelInfoTypeEvent_3 = new x.modelInfoTypeEvent();
            modelInfoTypeEvent_3.@event = "TestEvent 3";

            return new x.modelInfoTypeEvent[] { modelInfoTypeEvent_1, modelInfoTypeEvent_2, modelInfoTypeEvent_3 };
        }

        public string GetMessageId(string message)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(message);
            MemoryStream memoryStream = new MemoryStream(bytes);

            XmlSerializer serializer = new XmlSerializer(typeof(x.MessageType));
            x.MessageType messageType = (x.MessageType)serializer.Deserialize(memoryStream);
            memoryStream.Close();

            string messageId = messageType.msgId;
            return messageId;
        }

        public string GetPredefinedDeviceModel(string refMessageId)
        {
            StreamReader reader = new StreamReader(new FileStream("DeviceMotel.xml", FileMode.Open));
            string message = reader.ReadToEnd();

            string time = DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss");
            message = message.Replace("___TIME___", time);
            message = message.Replace("___MESSAGEID___", refMessageId);

            message = message.Replace("\r\n", "");

            message = message.Replace("  ", "");
            message = message.Replace("  ", "");
            message = message.Replace("  ", "");
            message = message.Replace("  ", "");
            message = message.Replace("  ", "");
            message = message.Replace("  ", "");
            message = message.Replace("  ", "");
            message = message.Replace("  ", "");
            message = message.Replace("  ", "");
            message = message.Replace("  ", "");
            message = message.Replace("  ", "");

            return message;
        }

        public string GetPredefinedConfiguredDevicesModel(string refMessageId)
        {
            StreamReader reader = new StreamReader(new FileStream("ConfiguredDevices.xml", FileMode.Open));
            string message = reader.ReadToEnd();

            string time = DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss");
            message = message.Replace("___TIME___", time);
            message = message.Replace("___MESSAGEID___", refMessageId);

            message = message.Replace("\r\n", "");

            message = message.Replace("  ", "");
            message = message.Replace("  ", "");
            message = message.Replace("  ", "");
            message = message.Replace("  ", "");
            message = message.Replace("  ", "");
            message = message.Replace("  ", "");
            message = message.Replace("  ", "");
            message = message.Replace("  ", "");
            message = message.Replace("  ", "");
            message = message.Replace("  ", "");
            message = message.Replace("  ", "");

            return message;
        }
        
    }
}

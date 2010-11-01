//#define _SVB_DEBUG_

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Collections.Specialized;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Logger
{
    public partial class Logger : Form
    {
        static public LoggerCollection logCollection;
//        static LogCollection
        static bool FirstMsg; // признак первой записи в любой из боксов для фиксации в имени начала лога
        static public bool FinalMsg; // признак прекращения работы приложения - для фиксации конечного времени
        
        static string StartDataTime;
        static DateTime dt;
        public Logger()
        {
            InitializeComponent();
            form = this;
            logCollection = new LoggerCollection();
            FirstMsg = false;
            FinalMsg = false;
        }

        static long MsgInLog;
        struct MessageInfo
        {
            public string msgId;          // номер отправляемого сообщения
            public string msgRefId;       // номер сообщения-источника
            public string msgTime;        // время-дата сообщения
            public string msgCommandType;   // команда
         };

        object GetMessageContent(string message, out MessageInfo messageInfo)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(message);
            MemoryStream memoryStream = new MemoryStream(bytes);
            XmlSerializer serializer = new XmlSerializer(typeof(x.MessageType));
            x.MessageType messageType = (x.MessageType)serializer.Deserialize(memoryStream);
            memoryStream.Close();
            messageInfo.msgId = messageType.msgId;
           messageInfo.msgTime  = messageType.msgTime;
           messageInfo.msgRefId = messageType.refMsg;
           messageInfo.msgCommandType = messageType.Item.GetType().Name; 
            return messageType.Item;
        }




        public static void Create()
        {
            form = new Logger();
            MsgInLog = 0;
            form.Show();
            form.Hide();
        }

        private void Logger_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        public static void PrintOutgoing(byte[] bytes)
        {
            if(FirstMsg == false)
            {
                dt = DateTime.Now;
                FirstMsg = true;
            }
            try
            {
                form.Invoke(new PrintDelegate(form.printOutgoing), bytes);
            }
            catch
            {
            }
        }
        public static void PrintIncomming(byte[] bytes)
        {
            if (FirstMsg == false)
            {
                dt = DateTime.Now;
                FirstMsg = true;
            }
            try
            {
                form.Invoke(new PrintDelegate(form.printIncomming), bytes);
            }
            catch
            {
            }
        }

        public delegate void PrintDelegate(byte[] bytes);
        public static Logger form;

        public void printOutgoing(byte[] bytes)
        {
            string message;
            MsgInLog++;
            MessageInfo msgInfo = new MessageInfo();
            message = Encoding.UTF8.GetString(bytes).Trim();
            object content = GetMessageContent(message, out msgInfo);
            string messageType = content.GetType().Name;
            outgoinglistBox.Items.Add(msgInfo.msgTime + " "
            + msgInfo.msgCommandType + " " 
             + msgInfo.msgId + " " 
            + msgInfo.msgRefId);
//            message = Encoding.Unicode.GetString(bytes).Trim(); ;
            logCollection.AddToOutCollection(message);
            int count = logCollection.OutColCount;
            outLogCount.Text = count.ToString();
        
        
        
        }
        public void printIncomming(byte[] bytes)
        {
            MsgInLog++;
            MessageInfo msgInfo = new MessageInfo();
            string message = Encoding.UTF8.GetString(bytes).Trim();
//            incommingTextBox.Text += message;
//            incommingTextBox.Text += "  " + "\n";
//            incomminglistBox.Items.Add(message + "\n");
            object content = GetMessageContent(message, out msgInfo);
            string messageType = content.GetType().Name;
            incomminglistBox.Items.Add(msgInfo.msgTime + " "
            + msgInfo.msgCommandType + " "
             + msgInfo.msgId + " "
            + msgInfo.msgRefId);

//            message = Encoding.Unicode.GetString(bytes).Trim(); ;
                int endIndex = message.IndexOf("\0");
                if (endIndex > 0)
                    message = message.Remove(message.IndexOf("\0"));

            logCollection.AddToInCollection(message);
            int count = logCollection.InColCount;
            inLogCount.Text = count.ToString();
            
        }

        private void Logger_Load(object sender, EventArgs e)
        {

          


        }

        private void button1_Click(object sender, EventArgs e)
        {
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void incomminglistBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            XDocument xc;
            string message; 
            int i;
            RTB_incomming.Clear();
            if (logCollection.InColCount != 0)
            {
                i = incomminglistBox.SelectedIndex;
                message = logCollection.GetStringFromInCollection(i);
                xc = XDocument.Parse(message);
                RTB_incomming.Text += xc.ToString();
                
                //RTB_incomming.Text += message;
            }
        }

        private void outgoinglistBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            XDocument xc;
            string message;
            int i;
            RTB_outgoing.Clear();
            if (logCollection.OutColCount != 0)
            {
                i = outgoinglistBox.SelectedIndex;
                message = logCollection.GetStringFromOutCollection(i);
                xc = XDocument.Parse(message);
                RTB_outgoing.Text += xc.ToString();
//                RTB_outgoing.Text += message;
            }

        }

        static public bool  SaveLogToFile()
        {
            BinaryFormatter binFormat = new BinaryFormatter();
            Stream fStream = new FileStream("logger.xml", FileMode.Create, 
                FileAccess.Write, FileShare.None);
            binFormat.Serialize(fStream, logCollection);
            fStream.Close();
            return true;
        }

        static public bool SaveLogToFileAs(string fileName)
        {
            BinaryFormatter binFormat = new BinaryFormatter();
            Stream fStream = new FileStream(fileName, FileMode.Create,
                FileAccess.Write, FileShare.None);
            binFormat.Serialize(fStream, logCollection);
            fStream.Close();
            return true;
        }

        public  void Logger_Deactivate(object sender, EventArgs e)
        {
//            SaveLogToFile();
        }
        public bool ReadLogFromFile()
        {
            BinaryFormatter binFormat = new BinaryFormatter();
            Stream fStream = File.OpenRead("logger.xml");

            logCollection = (LoggerCollection)binFormat.Deserialize(fStream);
            fStream.Close();
            return true;
        
        }

        public bool ReadLogFromUserFile(string fileName)
        {
            BinaryFormatter binFormat = new BinaryFormatter();
            Stream fStream = File.OpenRead(fileName);

            logCollection = (LoggerCollection)binFormat.Deserialize(fStream);
            fStream.Close();
            return true;

        }




        private void button2_Click(object sender, EventArgs e)
        {
            ReadLogFromFile();
            string message;
            incomminglistBox.Items.Clear();
            if(logCollection.InColCount != 0)
            {
                for (int i = 0; i < logCollection.InColCount; i++)
                {
                    message = logCollection.GetStringFromInCollection(i);
                    MessageInfo msgInfo = new MessageInfo();
                    object content = GetMessageContent(message, out msgInfo);
                    string messageType = content.GetType().Name;
                    incomminglistBox.Items.Add(msgInfo.msgTime + " "
                    + msgInfo.msgCommandType + " "
                     + msgInfo.msgId + " "
                    + msgInfo.msgRefId);
                }
             }

            outgoinglistBox.Items.Clear();
            if (logCollection.OutColCount != 0)
            {
                for (int i = 0; i < logCollection.OutColCount; i++)
                {
                    message = logCollection.GetStringFromOutCollection(i);
                    MessageInfo msgInfo = new MessageInfo();
                    object content = GetMessageContent(message, out msgInfo);
                    string messageType = content.GetType().Name;
                    outgoinglistBox.Items.Add(msgInfo.msgTime + " "
                    + msgInfo.msgCommandType + " "
                     + msgInfo.msgId + " "
                    + msgInfo.msgRefId);
                }
            }

        
        
        
        
        }

        private void button3_Click(object sender, EventArgs e)
        {
            SaveLogToFile();
        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

        // сохранение лога в файле, указанном пользователем
        private void button4_Click(object sender, EventArgs e)
        {
            string fileName;
            DialogResult result = saveFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
            fileName = saveFileDialog1.FileName;
            SaveLogToFileAs(fileName);
        }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string fileName;
            DialogResult result;
            if (MsgInLog != 0)
            {
                MessageBox.Show("Невозможно выполнить операцию пока идет обмен с АПИ");
                return;
            }

            if ((result = openFileDialog1.ShowDialog()) == DialogResult.OK)
            {
            fileName = openFileDialog1.FileName;
            ReadLogFromUserFile(fileName);
            string message;
            incomminglistBox.Items.Clear();
            if (logCollection.InColCount != 0)
            {
                for (int i = 0; i < logCollection.InColCount; i++)
                {
                    message = logCollection.GetStringFromInCollection(i);
                    MessageInfo msgInfo = new MessageInfo();
                    object content = GetMessageContent(message, out msgInfo);
                    string messageType = content.GetType().Name;
                    incomminglistBox.Items.Add(msgInfo.msgTime + " "
                    + msgInfo.msgCommandType + " "
                     + msgInfo.msgId + " "
                    + msgInfo.msgRefId);
                }
            }

            outgoinglistBox.Items.Clear();
            if (logCollection.OutColCount != 0)
            {
                for (int i = 0; i < logCollection.OutColCount; i++)
                {
                    message = logCollection.GetStringFromOutCollection(i);
                    MessageInfo msgInfo = new MessageInfo();
                    object content = GetMessageContent(message, out msgInfo);
                    string messageType = content.GetType().Name;
                    outgoinglistBox.Items.Add(msgInfo.msgTime + " "
                    + msgInfo.msgCommandType + " "
                     + msgInfo.msgId + " "
                    + msgInfo.msgRefId);
                }
            }
        
        
        
        }
    
    }
   
    
    
    
        [Serializable]
        public class LoggerCollection
    {
        List<string> inCollection;
        List<string> outCollection;
        public LoggerCollection()
        {
            inCollection = new List<string>();
            outCollection = new List<string>();
        }
          public void AddToInCollection(string str)
        {
            inCollection.Add(str);
        }

          public void AddToOutCollection(string str)
        {
            outCollection.Add(str);
        }

          public string GetStringFromInCollection(int index)
        {
            return inCollection[index];
        }

                 public string GetStringFromOutCollection(int index)
        {
            return outCollection[index];
        }



          public int InColCount
        {
            get
            {
                return inCollection.Count;
            }

        }

           public int OutColCount 
        {
            get
            {
                return outCollection.Count;
            }

        }

    }    

    }

}

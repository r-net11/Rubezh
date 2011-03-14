using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;



namespace OPCRubezhServer
{
    /// <summary>
    /// 
    /// </summary>
    public class Device
    {

        public string Name { get; set; }
        public uint Adr { get; set; }
        public int Parent { get; set; }
        public int ChildCount { get; set; }
        public int IdDevice { get; set; }

        //        public string GetAdr()
        //        {



        //      }



        public int CommandCount
        {
            get { return commandString.Count; }
        }



        public struct StatusReg
        {
            public string Value;
            public int iValue;
            public bool ValueIsOk;
            public StatusReg(string value, int ivalue, bool valueIsOk)
            {
                iValue = ivalue;
                Value = value;
                ValueIsOk = valueIsOk;
            }
        }
        public struct CommandReg
        {
            public string Value;
            public int iValue;
            public bool ValueIsOk;
            public CommandReg(string value, int ivalue, bool valueIsOk)
            {
                Value = value;
                iValue = ivalue;
                ValueIsOk = valueIsOk;
            }
        }

        public struct EventReg
        {
            public string Value;
            public int iValue;
            public bool ValueIsOk;
            public EventReg(string value, int ivalue, bool valueIsOk)
            {
                Value = value;
                iValue = ivalue;
                ValueIsOk = valueIsOk;
            }
        }

        List<string> statusString = new List<string>();
        List<string> commandString = new List<string>();
        List<string> eventString = new List<string>();

        public string GetStringFromCommandList(int item)
        {
       //     string str;
            if (item < 0) 
            {
            //    str = "код" +  item.ToString();
                return "Код"  + item.ToString();
            }
         if(item >= commandString.Count)
             return "Код" + item.ToString();
         return commandString[item]; 
        }
        public bool FindStringInCommandList(string s)
        {
          string str = s.ToLower().Trim();
          string rezStr;
          try
          {
              //              rezStr = commandString.Find(x => x.ToLower() == str);
                            rezStr = commandString.Find(x => x == str);
          }
          catch (ArgumentNullException e)
          {
              return false;
          
          }

          return true;

        }


        private string status;
        private int istatus;
        public int IStatus { get { return istatus; } set { istatus = value; } }
        public string Status { get { return status; }

            set
            {
                status = value.ToUpper().Trim();
                istatus = statusString.IndexOf(status);

            } 
        
        
        
        }
        
        private string command;
        private int icommand;
        
        private int command_count;


        private string eevent;
        private int  ieevent;
        public int IEvent { get { return ieevent; } set { ieevent = value; } }
        public string Event { get { return eevent; } set 
            { 
            eevent = value.Trim().ToUpper();
            ieevent = eventString.IndexOf(eevent);
            } 
        }
       
        
        
        
        private Timer m_simTimer;
        /// <summary>
        /// Эмулятор генерирующий различные значения 
        /// для регистров устройства
        /// </summary>
        /// <param name="param">The Device object.</param>
        private void Simulate(object param)
        {
         /*   Device thisDevice = (Device)param;
            lock (thisDevice)
            {
                if (status_count != 0)
                {
                    status_num++;
                    if (status_num >= status_count)
                        status_num = 0;
                    status = statusString[status_num];
                }

                if (command_count != 0)
                {
                    command_num++;
                    if (command_num >= command_count)
                        command_num = 0;
                    command = commandString[command_num];
                }

                if (event_count != 0)
                {
                    event_num++;
                    if (event_num >= event_count)
                        event_num = 0;
                    eevent = eventString[event_num];
                }


            }*/
        }


        public string GetAdrString()
        {
            uint d;
            string adr;
            if (Adr < 255) return Adr.ToString();
            d = (Adr & 0xFF00) >> 8;
            adr = d.ToString() + ".";
            d = Adr & 0xFF;
            return (adr = adr + d.ToString());
        }


        /// <summary>
        /// Инициализация нового объекта класса Device
        /// </summary>
        public Device(string name, int id, uint adr, 
                      int parent, int child,
                       StateServiceReference.ComDevice comdevice )
        {
            int count;
            Name = name;
            IdDevice = id;
            Adr = adr;
            Parent = parent;
            ChildCount = child;
            Name = Name + "(" + GetAdrString() + ")";
            statusString.Add("ТРЕВОГА");
            statusString.Add("ВНИМАНИЕ (ПРЕДТРЕВОЖНОЕ)");
            statusString.Add("НЕИСПРАВНОСТЬ");
            statusString.Add("ТРЕБУЕТСЯ ОБСЛУЖИВАНИЕ");
            statusString.Add("ОБХОД УСТРОЙСТВ");
            statusString.Add("НЕОПРЕДЕЛЕНО");
            statusString.Add("НОРМА(*)");
            statusString.Add("НОРМА");
            statusString.Add("НЕТ СОСТОЯНИЯ");
            statusString.Add("НЕТ ДАННЫХ");

            
            this.status = comdevice.State.ToUpper();

            istatus = statusString.IndexOf(status.Trim());

            if (comdevice.AvailableEvents != null)
            {
                eventString = comdevice.AvailableEvents.ToList();
                try
                {
                    for (count = 0; count < eventString.Count; count++)
                    {
                        if (eventString[count] != null)
                        eventString[count] = eventString[count].ToUpper();

                    }    
                }
                catch(Exception e)
                {
                this.eevent = "";
                }

            }
            
            
            
            if (comdevice.LastEvents != null)
            {
                if (comdevice.LastEvents.Count != 0)
                {
                    bool flag = false;
                    int i;
                    eevent = comdevice.LastEvents[comdevice.LastEvents.Count - 1].Trim().ToUpper();
                    ieevent = eventString.IndexOf(eevent);
                }
                else
                {
                    eevent = "";
                    ieevent = -1;
                }          
             }
            else
            {
                ieevent = -1;
                eevent = "";
            }
            
            command = "";
            icommand = -1;
            commandString.Clear();
            
            
            
            if (comdevice.AvailableFunctions != null)
            {
                commandString = comdevice.AvailableFunctions.ToList();
                command = "";
                if (commandString.Count != 0)
                {
                    command_count = commandString.Count;
                }
            }
        }
        /// <summary>
        /// Releases all resources used by the Device object.
        /// </summary>
        ~Device()
        {
          // / m_simTimer.Dispose();
        }

        public StatusReg ReadStatusRegister()
        {
            lock (this)
            {
                return new StatusReg(status, istatus ,true);
            }
        }


        public CommandReg ReadCommandReg()
        {
            lock (this)
            {
                return new CommandReg(command, icommand,true);
            }
        }

        public EventReg ReadEventReg()
        {
            lock (this)
            {
                return new EventReg(eevent, ieevent ,true);
            }
        }


    }

    /// <summary>
    /// Перечисление типов регистров устройства
    /// </summary>
    public enum RegisterType
    {
        Status,
        Event,
        Command
    }

}

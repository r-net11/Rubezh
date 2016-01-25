using GKWebService.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using RubezhAPI.GK;

namespace GKWebService.Models.FireZone
{
    public class Device
    {
        /// <summary>
        /// Наименование устройства
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// Адрес устройства
        /// </summary>
        public String Address { get; set; }

        /// <summary>
        /// Примечание
        /// </summary>
        public String Note { get; set; }

        /// <summary>
        /// Индикатор статуса
        /// </summary>
        public Tuple<string, System.Drawing.Size> StateImageSource { get; set; }

        /// <summary>
        /// Логотип устройства
        /// </summary>
        public Tuple<string, System.Drawing.Size> ImageBloom { get; set; }
       
        public Device(GKDevice device)
        {
            Address = device.Address;
            Name = device.PresentationName;
            Note = Name.IndexOf('(') > 0 ? Name.Split('(', ')')[1] : String.Empty;
            //Получаем логотип устройства
            ImageBloom = InternalConverter.GetImageResource(device.ImageSource);

            //Получаем изображение индикатора устройства
            StateImageSource = InternalConverter.GetImageResource("StateClassIcons/" + Convert.ToString(device.State.StateClass) + ".png");
        }
    }
}
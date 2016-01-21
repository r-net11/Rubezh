using GKWebService.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace GKWebService.Models.FireZone
{
    public class Device
    {
        /// <summary>
        /// Адрес устройства
        /// </summary>
        public String Address { get; set; }

        /// <summary>
        /// Индикатор статуса
        /// </summary>
        public Tuple<string, System.Drawing.Size> StateImageSource { get; set; }

        /// <summary>
        /// Логотип устройства
        /// </summary>
        public Tuple<string, System.Drawing.Size> ImageBloom { get; set; }

        /// <summary>
        /// Сокращенное имя устройства
        /// </summary>
        public String ShortName { get; set; }

        public Device(string address, string imageSource, string shortName, object stateImageSourse)
        {
            Address = address;
            ShortName = shortName;
            
            //Получаем логотип устройства
            ImageBloom = InternalConverter.GetImageResource(imageSource);

            //Получаем изображение индикатора устройства
            StateImageSource = InternalConverter.GetImageResource("StateClassIcons/" + Convert.ToString(stateImageSourse) + ".png");
        }
    }
}
using RubezhAPI.GK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace GKWebService.Models.FireZone
{
    public class FireZone
    {
        /// <summary>
        /// Количество датчиков для перевода в Пожар1
        /// </summary>
        public int Fire1Count { get; set; }

        /// <summary>
        /// Количество датчиков для перевода в Пожар2
        /// </summary>
        public int Fire2Count { get; set; }

        /// <summary>
        /// Изображение-логотип зоны
        /// </summary>
        public Tuple<string, System.Drawing.Size> ImageSource { get; set; }

        /// <summary>
        /// Изображение состояния зоны
        /// </summary>
        public Tuple<string, System.Drawing.Size> StateImageSource { get; set; }
        
        /// <summary>
        /// Состояние зоны
        /// </summary>
        public String StateLabel { get; set; }
        
        /// <summary>
        /// Имя зоны
        /// </summary>
        public String DescriptorPresentationName { get; set; }
    }
}
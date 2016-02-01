using System;

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
        /// Состояние зоны
        /// </summary>
        public String StateIcon { get; set; }
        
        /// <summary>
        /// Имя зоны
        /// </summary>
        public String Name { get; set; }

        public Guid Uid { get; set; }

        public Int32 No { get; set; }

        public String StateColor { get; set; }

        public String StateMessage { get; set; }

        public Boolean CanTurnOff { get; set; }

        public Boolean CanTurnOn { get; set; }

        public Boolean CanReset { get; set; }

        public ushort GKDescriptorNo { get; set; }

    }
}
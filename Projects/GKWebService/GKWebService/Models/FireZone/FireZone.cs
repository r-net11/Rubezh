using System;
using Controls.Converters;
using RubezhAPI;
using RubezhAPI.GK;

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
        public String ImageSource { get; set; }

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

        public FireZone(GKZone gkZone)
        {
            StateIcon = "/Content/Image/Icon/GKStateIcons/" + Convert.ToString(gkZone.State.StateClasses[0]) + ".png";
            Name = gkZone.DescriptorPresentationName;
            Fire1Count = gkZone.Fire1Count;
            Fire2Count = gkZone.Fire2Count;
            ImageSource = "/Content/Image/" + gkZone.ImageSource.Replace("/Controls;component/", "");
            Uid = gkZone.UID;
            No = gkZone.No;
            StateColor = "'#" +
                         new XStateClassToColorConverter2().Convert(gkZone.State.StateClass, null, null, null)
                             .ToString()
                             .Substring(3) + "'";
            StateMessage = gkZone.State.StateClass.ToDescription();
            CanTurnOff = Convert.ToString(gkZone.State.StateClasses[0]) == "Norm";
            CanTurnOn = Convert.ToString(gkZone.State.StateClasses[0]) == "Ignore";
            GKDescriptorNo = gkZone.GKDescriptorNo;
        }
    }
}
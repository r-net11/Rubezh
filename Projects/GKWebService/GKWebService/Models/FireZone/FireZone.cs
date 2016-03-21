using System;
using Controls.Converters;
using RubezhAPI;
using RubezhAPI.GK;
using RubezhAPI.Models;
using RubezhClient;

namespace GKWebService.Models.FireZone
{
	public class FireZone : GKBaseModel
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

		public Int32 No { get; set; }

		public String StateColor { get; set; }

		public String StateMessage { get; set; }

		public Boolean CanSetIgnore { get; set; }

		public Boolean CanResetIgnore { get; set; }

		public Boolean CanResetFire { get; set; }

		public ushort GKDescriptorNo { get; set; }

		public FireZone()
		{
			
		}

        public FireZone(GKZone gkZone)
			: base(gkZone)
		{
			StateIcon = Convert.ToString(gkZone.State.StateClass);
			Name = gkZone.Name;
			Fire1Count = gkZone.Fire1Count;
			Fire2Count = gkZone.Fire2Count;
			ImageSource = gkZone.ImageSource.Replace("/Controls;component/", "");
			No = gkZone.No;
			StateColor = "'#" +
						 new XStateClassToColorConverter2().Convert(gkZone.State.StateClass, null, null, null)
							 .ToString()
							 .Substring(3) + "'";
			StateMessage = gkZone.State.StateClass.ToDescription();
			CanSetIgnore = !gkZone.State.StateClasses.Contains(XStateClass.Ignore);
			CanResetIgnore = gkZone.State.StateClasses.Contains(XStateClass.Ignore);
			CanResetFire = gkZone.State.StateClasses.Contains(XStateClass.Fire2) 
				|| gkZone.State.StateClasses.Contains(XStateClass.Fire1) 
				|| gkZone.State.StateClasses.Contains(XStateClass.Attention);
			GKDescriptorNo = gkZone.GKDescriptorNo;
		}
	}
}
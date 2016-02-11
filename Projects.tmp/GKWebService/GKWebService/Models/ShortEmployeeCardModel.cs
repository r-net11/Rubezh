using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RubezhAPI.SKD;

namespace GKWebService.Models
{
	public class ShortEmployeeCardModel
	{
		public Guid UID { get; set; }

		public uint Number { get; set; }

		public List<ReadOnlyAccessDoorModel> Doors { get; set; }

		public static ShortEmployeeCardModel Create(SKDCard card)
		{
			var model = new ShortEmployeeCardModel();
			model.Number = card.Number;
			model.UID = card.UID;
			return model;
		}
	}
}
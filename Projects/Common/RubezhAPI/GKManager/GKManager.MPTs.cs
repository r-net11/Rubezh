using RubezhAPI.GK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RubezhClient
{
	public partial class GKManager
	{
		/// <summary>
		/// Добавление МПТ
		/// </summary>
		/// <param name="mpt"></param>
		public static void AddMPT(GKMPT mpt)
		{
			MPTs.Add(mpt);
		}

		/// <summary>
		/// Удаление МПТ
		/// </summary>
		/// <param name="mpt"></param>
		public static void RemoveMPT(GKMPT mpt)
		{
			MPTs.Remove(mpt);
			mpt.InputDependentElements.ForEach(x =>
			{
				x.OutDependentElements.Remove(mpt);
			});

			mpt.OutDependentElements.ForEach(x =>
			{
				x.InputDependentElements.Remove(mpt);
				x.UpdateLogic();
				x.OnChanged();
			});
		}

		/// <summary>
		/// Изменение МПТ
		/// </summary>
		/// <param name="mpt"></param>
		public static void EditMPT(GKMPT mpt)
		{
			mpt.OutDependentElements.ForEach(x => x.OnChanged());
			mpt.OnChanged();
		}
	}
}
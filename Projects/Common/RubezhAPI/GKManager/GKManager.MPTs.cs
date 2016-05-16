using RubezhAPI.GK;

namespace RubezhAPI
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
				x.OutputDependentElements.Remove(mpt);
			});

			mpt.OutputDependentElements.ForEach(x =>
			{
				x.InputDependentElements.Remove(mpt);
				x.UpdateLogic(DeviceConfiguration);
				x.OnChanged();
			});

			mpt.OnChanged();
		}

		/// <summary>
		/// Изменение МПТ
		/// </summary>
		/// <param name="mpt"></param>
		public static void EditMPT(GKMPT mpt)
		{
			mpt.OutputDependentElements.ForEach(x => x.OnChanged());
			mpt.OnChanged();
		}

		public static void SetMPTLogic(GKMPT MPT, GKLogic logic)
		{
			MPT.MptLogic = logic;
			MPT.ChangedLogic();
		}
	}
}
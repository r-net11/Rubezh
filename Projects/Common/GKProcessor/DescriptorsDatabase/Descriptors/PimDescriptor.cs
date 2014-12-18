using FiresecAPI.GK;

namespace GKProcessor
{
	public class PimDescriptor : BaseDescriptor
	{
		protected GKPim Pim { get; set; }

		public PimDescriptor(GKPim pim, DatabaseType dataBaseType)
			: base(pim, dataBaseType)
		{
			DescriptorType = DescriptorType.Pim;
			Pim = pim;
		}

		public override void Build()
		{
			DeviceType = BytesHelper.ShortToBytes((ushort)0x107);
			SetAddress((ushort)0);
			SetFormulaBytes();
		}

		void SetFormulaBytes()
		{
			if (Formula == null)
			{
				Formula = new FormulaBuilder();
				Formula.Add(FormulaOperationType.END);
				FormulaBytes = Formula.GetBytes();
			}
		}
	}
}
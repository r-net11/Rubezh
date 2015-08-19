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
			DeviceType = BytesHelper.ShortToBytes(0x107);
			SetAddress(0);
		}

		public override void BuildFormula()
		{
			if (Formula == null || Formula.FormulaOperations.Count == 0)
			{
				Formula = new FormulaBuilder();
				Formula.Add(FormulaOperationType.END);
			}
		}
	}
}
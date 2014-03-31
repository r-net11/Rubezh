using XFiresecAPI;

namespace GKProcessor
{
	public class PimDescriptor : BaseDescriptor
	{
		public PimDescriptor(XPim pim)
		{
			DatabaseType = DatabaseType.Gk;
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
			Formula = new FormulaBuilder();
			Formula.Add(FormulaOperationType.END);
			FormulaBytes = Formula.GetBytes();
		}
	}
}
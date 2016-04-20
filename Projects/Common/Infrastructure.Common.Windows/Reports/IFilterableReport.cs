
namespace Infrastructure.Common.Windows.Reports
{
	public interface IFilterableReport
	{
		void Filter(RelayCommand refreshCommand);
	}
}

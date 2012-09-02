
namespace Infrastructure.Common.Reports
{
	public interface IFilterableReport
	{
		void Filter(RelayCommand refreshCommand);
	}
}

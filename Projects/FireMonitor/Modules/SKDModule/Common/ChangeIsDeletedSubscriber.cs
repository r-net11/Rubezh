using Infrastructure.Common.Services;
using StrazhAPI.SKD;
using Infrastructure;
using SKDModule.Events;

namespace SKDModule
{
	public interface ITimeTrackItemsViewModel
	{
		LogicalDeletationType LogicalDeletationType { get; set; }
		void Initialize();
	}

	public class ChangeIsDeletedSubscriber
	{
		readonly ITimeTrackItemsViewModel _parent;

		public ChangeIsDeletedSubscriber(ITimeTrackItemsViewModel parent)
		{
			_parent = parent;
			ServiceFactoryBase.Events.GetEvent<ChangeIsDeletedEvent>().Unsubscribe(OnChangeIsDeleted);
			ServiceFactoryBase.Events.GetEvent<ChangeIsDeletedEvent>().Subscribe(OnChangeIsDeleted);
		}

		void OnChangeIsDeleted(LogicalDeletationType logicalDeletationType)
		{
			_parent.LogicalDeletationType = logicalDeletationType;
			_parent.Initialize();
		}
	}
}

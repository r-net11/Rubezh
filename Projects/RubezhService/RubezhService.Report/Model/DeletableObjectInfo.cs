
namespace RubezhService.Report.Model
{
	public class DeletableObjectInfo<T> : ObjectInfo
	{
		public string Name { get; set; }
		public bool IsDeleted { get; set; }
		public T Item { get; set; }
	}
}

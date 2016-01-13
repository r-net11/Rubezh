
namespace Common
{
	public class Clipboard<T>
	{
		public void Clear()
		{
			this.SourceAction = ClipboardSourceAction.None;
			this.OnClear();
		}

		protected virtual void OnClear()
		{
		}

		public T Buffer { get; set; }
		public ClipboardSourceAction SourceAction { get; set; }
	}
}

using FiresecAPI.Models.Layouts;
using Infrastructure.Common.TreeList;

namespace LayoutModule.ViewModels
{
	public class LayoutViewModel : TreeNodeViewModel<LayoutViewModel>
	{
		public Layout Layout { get; private set; }
		public LayoutFolder LayoutFolder { get; private set; }
		public object LayoutObject
		{
			get { return IsLayout ? (object)Layout : LayoutFolder; }
		}

		public LayoutViewModel(Layout layout)
		{
			Layout = layout;
			LayoutFolder = null;
		}
		public LayoutViewModel(LayoutFolder layoutFolder)
		{
			Layout = null;
			LayoutFolder = layoutFolder;
			foreach (var folder in LayoutFolder.Folders)
				AddChild(new LayoutViewModel(folder));
			foreach (var layout in LayoutFolder.Layouts)
				AddChild(new LayoutViewModel(layout));
		}

		public void Update()
		{
			IsExpanded = false;
			IsExpanded = true;
			OnPropertyChanged(() => HasChildren);
			OnPropertyChanged(() => Caption);
			OnPropertyChanged(() => Description);
		}

		public string Caption
		{
			get { return IsLayout ? Layout.Caption : LayoutFolder.Caption; }
		}
		public string Description
		{
			get { return IsLayout ? Layout.Description : LayoutFolder.Description; }
		}
		public bool IsFolder
		{
			get { return LayoutFolder != null; }
		}
		public bool IsLayout
		{
			get { return Layout != null; }
		}

		public override string ToString()
		{
			return base.ToString() + " [" + Caption + "]";
		}
	}
}

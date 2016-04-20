using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Common;
using RubezhAPI.Models.Layouts;

namespace Infrastructure.Common.Windows.Services.Layout
{
	public class LayoutPartContainerCollection : ObservableCollection<ILayoutPartContainer>, ILayoutPartContainer
	{
		public LayoutPartContainerCollection()
		{
			CollectionChanged += LayoutPartContainerCollection_CollectionChanged;
		}

		private void LayoutPartContainerCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Add:
				case NotifyCollectionChangedAction.Remove:
				case NotifyCollectionChangedAction.Replace:
					if (e.NewItems != null)
						foreach (ILayoutPartContainer item in e.NewItems)
						{
							item.ActiveChanged += OnActiveChanged;
							item.SelectedChanged += OnSelectedChanged;
						}
					if (e.OldItems != null)
						foreach (ILayoutPartContainer item in e.OldItems)
						{
							item.ActiveChanged -= OnActiveChanged;
							item.SelectedChanged -= OnSelectedChanged;
						}
					break;
			}
		}

		#region ILayoutPartContainer Members

		public Guid UID
		{
			get { throw new NotSupportedException(); }
		}

		public string Title
		{
			get { throw new NotSupportedException(); }
			set { this.ForEach(item => item.Title = value); }
		}

		public string IconSource
		{
			get { throw new NotSupportedException(); }
			set { this.ForEach(item => item.IconSource = value); }
		}

		public bool IsActive
		{
			get { throw new NotSupportedException(); }
			set { this.ForEach(item => item.IsActive = value); }
		}

		public bool IsSelected
		{
			get { throw new NotSupportedException(); }
			set { this.ForEach(item => item.IsSelected = value); }
		}

		public bool IsVisibleLayout
		{
			get { throw new NotSupportedException(); }
		}

		public LayoutPart LayoutPart
		{
			get { throw new NotSupportedException(); }
		}

		public ILayoutPartPresenter LayoutPartPresenter
		{
			get { throw new NotSupportedException(); }
		}

		public void Activate()
		{
			this.ForEach(item => item.Activate());
		}

		public event EventHandler SelectedChanged;
		public event EventHandler ActiveChanged;

		#endregion

		private void OnActiveChanged(object sender, EventArgs e)
		{
			if (ActiveChanged != null)
				ActiveChanged(sender, e);
		}
		private void OnSelectedChanged(object sender, EventArgs e)
		{
			if (SelectedChanged != null)
				SelectedChanged(sender, e);
		}
	}
}

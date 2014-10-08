using System;
using System.Collections.Generic;
using Common;
using FiresecAPI.Models;
using Infrustructure.Plans;
using Infrustructure.Plans.Designer;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Interfaces;

namespace Infrastructure.Client.Plans
{
	public abstract class BasePlanExtension : IPlanExtension<Plan>
	{
		public MapSource Cache { get; private set; }
		protected CommonDesignerCanvas DesignerCanvas { get; private set; }

		public BasePlanExtension()
		{
			Cache = new MapSource();
		}

		public virtual void ExtensionRegistered(CommonDesignerCanvas designerCanvas)
		{
			DesignerCanvas = designerCanvas;
		}
		public virtual void ExtensionAttached()
		{
			Cache.BuildAllSafe();
		}
		public virtual void Initialize()
		{
			Cache.BuildAllSafe();
		}

		protected void RegisterDesignerItem<TItem>(DesignerItem designerItem, string group, string iconSource = null)
			where TItem : IChangedNotification, IPlanPresentable
		{
			designerItem.ItemPropertyChanged += DesignerItemPropertyChanged<TItem>;
			OnDesignerItemPropertyChanged<TItem>(designerItem);
			designerItem.Group = group;
			if (!string.IsNullOrEmpty(iconSource))
				designerItem.IconSource = iconSource;
			designerItem.UpdateProperties += UpdateProperties<TItem>;
			UpdateProperties<TItem>(designerItem);
		}
		private void DesignerItemPropertyChanged<TItem>(object sender, EventArgs e)
			where TItem : IChangedNotification, IPlanPresentable
		{
			DesignerItem designerItem = (DesignerItem)sender;
			OnDesignerItemPropertyChanged<TItem>(designerItem);
		}
		protected void OnDesignerItemPropertyChanged<TItem>(DesignerItem designerItem)
			where TItem : IChangedNotification, IPlanPresentable
		{
			var item = GetItem<TItem>((IElementReference)designerItem.Element);
			if (item != null)
				item.Changed += () =>
				{
					if (DesignerCanvas.IsPresented(designerItem))
					{
						Cache.BuildSafe<TItem>();
						UpdateProperties<TItem>(designerItem);
						designerItem.Painter.Invalidate();
						DesignerCanvas.Refresh();
					}
				};
		}
		protected virtual void UpdateProperties<TItem>(CommonDesignerItem designerItem)
			where TItem : IChangedNotification, IPlanPresentable
		{
			var elementReference = designerItem.Element as IElementReference;
			var item = GetItem<TItem>(elementReference);
			SetItem<TItem>(elementReference, item);
			UpdateDesignerItemProperties<TItem>(designerItem, item);
		}

		public TItem GetItem<TItem>(IElementReference element)
			where TItem : IChangedNotification, IPlanPresentable
		{
			return GetItem<TItem>(element.ItemUID);
		}
		public TItem GetItem<TItem>(Guid uid)
			where TItem : IChangedNotification, IPlanPresentable
		{
			return Cache.Get<TItem>(uid);
		}

		public void SetItem<TItem>(IElementReference element)
			where TItem : IChangedNotification, IPlanPresentable
		{
			var item = GetItem<TItem>(element);
			SetItem<TItem>(element, item);
		}
		public void SetItem<TItem>(IElementReference element, Guid itemUID)
			where TItem : IChangedNotification, IPlanPresentable
		{
			var item = GetItem<TItem>(itemUID);
			SetItem(element, item);
		}
		public void SetItem<TItem>(IElementReference element, TItem item)
			where TItem : IChangedNotification, IPlanPresentable
		{
			if (item != null && item.UID == element.ItemUID)
				ResetItem<TItem>(element, item);
			else
				ResetItem<TItem>(element);
			element.ItemUID = item == null ? Guid.Empty : item.UID;
			if (item != null)
				item.PlanElementUIDs.Add(element.UID);
			UpdateElementProperties<TItem>(element, item);
		}
		public void ResetItem<TItem>(IElementReference element)
			where TItem : IChangedNotification, IPlanPresentable
		{
			var item = GetItem<TItem>(element);
			ResetItem<TItem>(element, item);
		}
		public void ResetItem<TItem>(IElementReference element, TItem item)
			where TItem : IChangedNotification, IPlanPresentable
		{
			if (item != null)
				item.PlanElementUIDs.Remove(element.UID);
		}

		protected virtual void UpdateElementProperties<TItem>(IElementReference element, TItem item)
			where TItem : IChangedNotification, IPlanPresentable
		{
		}
		protected virtual void UpdateDesignerItemProperties<TItem>(CommonDesignerItem designerItem, TItem item)
			where TItem : IChangedNotification, IPlanPresentable
		{
		}

		#region IPlanExtension<Plan> Members

		public abstract int Index { get; }
		public abstract string Title { get; }
		public abstract bool ElementAdded(Plan plan, ElementBase element);
		public abstract bool ElementRemoved(Plan plan, ElementBase element);
		public abstract void RegisterDesignerItem(DesignerItem designerItem);
		public abstract IEnumerable<ElementBase> LoadPlan(Plan plan);
		public abstract IEnumerable<IInstrument> Instruments { get; }

		#endregion
	}
}

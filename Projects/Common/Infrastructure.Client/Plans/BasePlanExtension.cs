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
			Cache.BuildAll();
		}

		protected void RegisterDesignerItem<TElement, TItem>(DesignerItem designerItem, string group, string iconSource = null)
			where TElement : ElementBase, IElementReference
			where TItem : IChangedNotification, IPlanPresentable
		{
			designerItem.ItemPropertyChanged += DesignerItemPropertyChanged<TElement, TItem>;
			OnDesignerItemPropertyChanged<TElement, TItem>(designerItem);
			designerItem.Group = group;
			if (!string.IsNullOrEmpty(iconSource))
				designerItem.IconSource = iconSource;
			designerItem.UpdateProperties += UpdateProperties<TElement, TItem>;
			UpdateProperties<TElement, TItem>(designerItem);
		}
		private void DesignerItemPropertyChanged<TElement, TItem>(object sender, EventArgs e)
			where TElement : ElementBase, IElementReference
			where TItem : IChangedNotification, IPlanPresentable
		{
			DesignerItem designerItem = (DesignerItem)sender;
			OnDesignerItemPropertyChanged<TElement, TItem>(designerItem);
		}
		private void OnDesignerItemPropertyChanged<TElement, TItem>(DesignerItem designerItem)
			where TElement : ElementBase, IElementReference
			where TItem : IChangedNotification, IPlanPresentable
		{
			var item = GetItem<TElement, TItem>((TElement)designerItem.Element);
			if (item != null)
				item.Changed += () =>
				{
					if (DesignerCanvas.IsPresented(designerItem))
					{
						Cache.BuildSafe<TItem>();
						UpdateProperties<TElement, TItem>(designerItem);
						designerItem.Painter.Invalidate();
						DesignerCanvas.Refresh();
					}
				};
		}
		protected void UpdateProperties<TElement, TItem>(CommonDesignerItem designerItem)
			where TElement : ElementBase, IElementReference
			where TItem : IChangedNotification, IPlanPresentable
		{
			TElement element = designerItem.Element as TElement;
			var item = GetItem<TElement, TItem>(element);
			if (item != null)
			{
				SetItem<TElement, TItem>(element, item);
				UpdateDesignerItemProperties<TItem>(designerItem, item);
			}
		}

		public TItem GetItem<TElement, TItem>(TElement element)
			where TElement : ElementBase, IElementReference
			where TItem : IChangedNotification, IPlanPresentable
		{
			return GetItem<TItem>(element.ItemUID);
		}
		public TItem GetItem<TItem>(Guid uid)
			where TItem : IChangedNotification, IPlanPresentable
		{
			return Cache.Get<TItem>(uid);
		}

		public void SetItem<TElement, TItem>(TElement element)
			where TElement : ElementBase, IElementReference
			where TItem : IChangedNotification, IPlanPresentable
		{
			var item = GetItem<TElement, TItem>(element);
			SetItem<TElement, TItem>(element, item);
		}
		public void SetItem<TElement, TItem>(TElement element, Guid itemUID)
			where TElement : ElementBase, IElementReference
			where TItem : IChangedNotification, IPlanPresentable
		{
			var item = GetItem<TItem>(itemUID);
			SetItem(element, item);
		}
		public void SetItem<TElement, TItem>(TElement element, TItem item)
			where TElement : ElementBase, IElementReference
			where TItem : IChangedNotification, IPlanPresentable
		{
			if (item.UID == element.ItemUID)
				ResetItem<TElement, TItem>(element, item);
			else
				ResetItem<TElement, TItem>(element);
			element.ItemUID = item == null ? Guid.Empty : item.UID;
			if (item != null)
				item.PlanElementUIDs.Add(element.UID);
			UpdateElementProperties<TElement, TItem>(element, item);
		}
		public void ResetItem<TElement, TItem>(TElement element)
			where TElement : ElementBase, IElementReference
			where TItem : IChangedNotification, IPlanPresentable
		{
			var item = GetItem<TElement, TItem>(element);
			ResetItem<TElement, TItem>(element, item);
		}
		public void ResetItem<TElement, TItem>(TElement element, TItem item)
			where TElement : ElementBase, IElementReference
			where TItem : IChangedNotification, IPlanPresentable
		{
			if (item != null)
				item.PlanElementUIDs.Remove(element.UID);
		}


		public void SetZoneItem<TItem>(IElementZone element)
			where TItem : IChangedNotification, IPlanPresentable
		{
			SetZoneItem<TItem>(element, element.ZoneUID);
		}
		public void SetZoneItem<TItem>(IElementZone element, Guid itemUID)
			where TItem : IChangedNotification, IPlanPresentable
		{
			var item = GetItem<TItem>(itemUID);
			SetZoneItem(element, item);
		}
		public void SetZoneItem<TItem>(IElementZone element, TItem item)
			where TItem : IChangedNotification, IPlanPresentable
		{
			ResetZoneItem<TItem>(element);
			element.ZoneUID = item == null ? Guid.Empty : item.UID;
			if (item != null)
				item.PlanElementUIDs.Add(element.UID);
			UpdateZoneElementProperties<TItem>(element, item);
		}
		public void ResetZoneItem<TItem>(IElementZone element)
			where TItem : IChangedNotification, IPlanPresentable
		{
			var item = GetItem<TItem>(element.ZoneUID);
			ResetZoneItem<TItem>(element, item);
		}
		public void ResetZoneItem<TItem>(IElementZone element, TItem item)
			where TItem : IChangedNotification, IPlanPresentable
		{
			if (item != null)
				item.PlanElementUIDs.Remove(element.UID);
		}


		protected virtual void UpdateElementProperties<TElement, TItem>(TElement element, TItem item)
			where TElement : ElementBase, IElementReference
			where TItem : IChangedNotification, IPlanPresentable
		{
		}
		protected virtual void UpdateDesignerItemProperties<TItem>(CommonDesignerItem designerItem, TItem item)
			where TItem : IChangedNotification, IPlanPresentable
		{
		}
		protected virtual void UpdateZoneElementProperties<TItem>(IElementZone element, TItem item)
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

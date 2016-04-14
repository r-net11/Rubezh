using Common;
using Infrastructure.Common.Services;
using Infrustructure.Plans;
using Infrustructure.Plans.Designer;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Interfaces;
using Microsoft.Practices.Prism.Events;
using RubezhAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;

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
			{
				Action onChanged = () =>
				{
					Cache.BuildSafe<TItem>();
					UpdateProperties<TItem>(designerItem);
					if (DesignerCanvas.IsPresented(designerItem))
					{
						designerItem.Painter.Invalidate();
						DesignerCanvas.Refresh();
					}
				};
				Action<Guid, Guid> onUidChanged = (oldUID, newUID) =>
				 {
					 var elementReference = designerItem.Element as IElementReference;
					 if (elementReference != null && elementReference.ItemUID == oldUID)
						 elementReference.ItemUID = newUID;
				 };
				item.Changed += onChanged;
				item.UIDChanged += onUidChanged;
				designerItem.Removed += () => item.Changed -= onChanged;
				designerItem.Removed += () => item.UIDChanged -= onUidChanged;
			}
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
			{
				item.PlanElementUIDs.Add(element.UID);
			}
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
			{
				item.PlanElementUIDs.Remove(element.UID);
			}
		}

		protected virtual void UpdateElementProperties<TItem>(IElementReference element, TItem item)
			where TItem : IChangedNotification, IPlanPresentable
		{
		}
		protected virtual void UpdateDesignerItemProperties<TItem>(CommonDesignerItem designerItem, TItem item)
			where TItem : IChangedNotification, IPlanPresentable
		{
		}

		public static IEnumerable<TReference> FindUnbinded<TReference>(IEnumerable<TReference> elements)
			where TReference : IElementReference
		{
			return elements.Where(item => item.ItemUID == Guid.Empty);
		}
		public static IEnumerable<ElementError> FindUnbindedErrors<TReference, TEvent, TArg>(IEnumerable<TReference> elements, Guid planUID, string error, string imageSource, TArg arg = default(TArg))
			where TReference : ElementBase, IElementReference
			where TEvent : CompositePresentationEvent<TArg>, new()
		{
			return FindUnbinded<TReference>(elements).Select(element =>
				new ElementError()
				{
					PlanUID = planUID,
					Error = error,
					Element = element,
					IsCritical = false,
					ImageSource = imageSource,
					Navigate = () => ServiceFactoryBase.Events.GetEvent<TEvent>().Publish(arg),
				});
		}

		public IEnumerable<Guid> FindDuplicate<TReference>(IEnumerable<TReference> elements, IEnumerable<TReference> elements2 = null)
			where TReference : IElementReference
		{
			var source = elements2 == null ? elements : elements.Concat(elements2);
			var set = new HashSet<Guid>();
			foreach (var item in source)
			{
				if (set.Contains(item.ItemUID))
					yield return item.ItemUID;
				else
					set.Add(item.ItemUID);
			}
		}
		public IEnumerable<TItem> FindDuplicateItems<TItem, TReference>(IEnumerable<TReference> elements1, IEnumerable<TReference> elements2 = null)
			where TItem : IChangedNotification, IPlanPresentable
			where TReference : IElementReference
		{
			var duplicates = FindDuplicate<TReference>(elements1, elements2);
			foreach (var duplicate in duplicates)
			{
				var item = GetItem<TItem>(duplicate);
				if (item != null)
					yield return item;
			}
		}

		#region IPlanExtension<Plan> Members

		public abstract int Index { get; }
		public abstract string Title { get; }
		public abstract bool ElementAdded(Plan plan, ElementBase element);
		public abstract bool ElementRemoved(Plan plan, ElementBase element);
		public abstract void RegisterDesignerItem(DesignerItem designerItem);
		public abstract IEnumerable<ElementBase> LoadPlan(Plan plan);
		public abstract IEnumerable<IInstrument> Instruments { get; }
		public abstract IEnumerable<ElementError> Validate();

		#endregion
	}
}
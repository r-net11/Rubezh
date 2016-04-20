using System;
using System.Collections.Generic;
using System.Linq;
using RubezhAPI.Automation;
using RubezhAPI.GK;
using RubezhClient;
using Infrastructure.Common.Windows.Windows.ViewModels;
using Infrastructure.ViewModels;
using Softing.Opc.Ua.Toolkit.Client;
using Softing.Opc.Ua.Toolkit;

namespace AutomationModule.ViewModels
{
    public class OPCSelectTagsViewModel : SaveCancelDialogViewModel
	{
        public Session Session { get; private set; }
        public Subscription Subscription { get; private set; }

        public OPCSelectTagsViewModel(Session session, Subscription subscription)
		{
		    Session = session;
            Subscription = subscription;
            BuildTree();
		}

		public void Initialize()
		{
			BuildTree();
			if (RootTag != null)
			{
				RootTag.IsExpanded = true;
				SelectedTag = RootTag;
				/*foreach (var child in RootTag.Children)
				{
					if (child.Tag.DriverType == GKDriverType.GK)
						child.IsExpanded = true;
				}*/
			}

			/*foreach (var tag in AllTags)
			{
				if (tag.Device.DriverType == GKDriverType.RSR2_KAU)
					tag.ExpandToThis();
			}*/

			OnPropertyChanged(() => RootTags);
		}

		#region TagSelection
		public List<TagViewModel> AllTags;

		public void FillAllTags()
		{
			AllTags = new List<TagViewModel>();
			AddChildPlainTags(RootTag);
		}

		void AddChildPlainTags(TagViewModel parentViewModel)
		{
			AllTags.Add(parentViewModel);
			foreach (var childViewModel in parentViewModel.Children)
				AddChildPlainTags(childViewModel);
		}

		public void Select(Guid tagUID)
		{
			if (tagUID != Guid.Empty)
			{
				FillAllTags();
				var tagViewModel = AllTags.FirstOrDefault(x => x.UID == tagUID);
				if (tagViewModel != null)
					tagViewModel.ExpandToThis();
				SelectedTag = tagViewModel;
			}
		}
		#endregion

		TagViewModel _selectedTag;
		public TagViewModel SelectedTag
		{
			get { return _selectedTag; }
			set
			{
				_selectedTag = value;
				OnPropertyChanged(() => SelectedTag);
			}
		}

		TagViewModel _rootTag;
		public TagViewModel RootTag
		{
			get { return _rootTag; }
			private set
			{
				_rootTag = value;
				OnPropertyChanged(() => RootTag);
			}
		}

		public TagViewModel[] RootTags
		{
			get { return new TagViewModel[] { RootTag }; }
		}

		void BuildTree()
		{
			RootTag = AddTagInternal(null, null);
			FillAllTags();
		}

        private TagViewModel AddTagInternal(ReferenceDescription tag, TagViewModel parentTagViewModel)
		{
			var tagViewModel = new TagViewModel(tag);
            if (parentTagViewModel != null)
                tagViewModel.Path = parentTagViewModel.Path + "/" + tagViewModel.Address;
            if (Subscription.MonitoredItems.Any(x => x.NodeId.ToString() == tagViewModel.Address))
            {
                tagViewModel.IsTagUsed = true;
                tagViewModel.IsExpanded = true;
                tagViewModel.ExpandToThis();
            }
            /*tagViewModel.Path = parentTagViewModel == null
                ? tagViewModel.Tag.DisplayName.ToString()
                : parentTagViewModel.Path + tagViewModel.Tag.DisplayName.ToString();*/
            if (parentTagViewModel != null)
				parentTagViewModel.AddChild(tagViewModel);

            if (tag != null)
                foreach (var childTag in Session.Browse(new NodeId(tag.NodeId), null))
                    AddTagInternal(childTag, tagViewModel);
            else
                foreach (var childTag in Session.Browse(null, null))
                    AddTagInternal(childTag, tagViewModel);
            return tagViewModel;
		}
	}
}
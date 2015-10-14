using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Resurs.Processor;
using Resurs.Reports.Templates;
using ResursAPI;
using ResursDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Resurs.ViewModels
{
	public class ConsumersViewModel : BaseViewModel
	{
		public ConsumersViewModel()
		{
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			AddFolderCommand = new RelayCommand(OnAddFolder, CanAddFolder);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			RemoveCommand = new RelayCommand(OnRemove, CanRemove);
			ChangeParentCommand = new RelayCommand(OnChangeParent, CanChangeParent);
			OpenReceiptCommand = new RelayCommand(OnOpenReceipt, CanOpenReceipt);

			BuildTree();
			if (RootConsumer != null)
			{
				SelectedConsumer = RootConsumer;
				RootConsumer.IsExpanded = true;
				foreach (var child in RootConsumer.Children)
					child.IsExpanded = true;
			}

			foreach (var consumer in AllConsumers)
			{
				if (true)
					consumer.ExpandToThis();
			}

			OnPropertyChanged(() => RootConsumers);
		}

		ConsumerViewModel _selectedConsumer;
		public ConsumerViewModel SelectedConsumer
		{
			get { return _selectedConsumer; }
			set
			{
				_selectedConsumer = value;
				OnPropertyChanged(() => SelectedConsumer);
			}
		}

		ConsumerViewModel _rootConsumer;
		public ConsumerViewModel RootConsumer
		{
			get { return _rootConsumer; }
			private set
			{
				_rootConsumer = value;
				OnPropertyChanged(() => RootConsumer);
			}
		}

		public ConsumerViewModel[] RootConsumers
		{
			get { return new[] { RootConsumer }; }
		}

		void BuildTree()
		{
			RootConsumer = AddConsumerInternal(DBCash.RootConsumer, null);
			FillAllConsumers();
		}

		public ConsumerViewModel AddConsumer(Consumer consumer, ConsumerViewModel parentConsumerViewModel)
		{
			var consumerViewModel = AddConsumerInternal(consumer, parentConsumerViewModel);
			FillAllConsumers();
			return consumerViewModel;
		}
		private ConsumerViewModel AddConsumerInternal(Consumer consumer, ConsumerViewModel parentConsumerViewModel)
		{
			var consumerViewModel = new ConsumerViewModel(consumer);
			if (parentConsumerViewModel != null)
				parentConsumerViewModel.AddChild(consumerViewModel);

			foreach (var childConsumer in consumer.Children)
				AddConsumerInternal(childConsumer, consumerViewModel);
			return consumerViewModel;
		}

		public List<ConsumerViewModel> AllConsumers;

		public void FillAllConsumers()
		{
			AllConsumers = new List<ConsumerViewModel>();
			AddChildPlainConsumers(RootConsumer);
		}

		void AddChildPlainConsumers(ConsumerViewModel parentViewModel)
		{
			AllConsumers.Add(parentViewModel);
			foreach (var childViewModel in parentViewModel.Children)
				AddChildPlainConsumers(childViewModel);
		}

		public void Select(Guid consumerUID)
		{
			if (consumerUID != Guid.Empty)
			{
				FillAllConsumers();
				var consumerViewModel = AllConsumers.FirstOrDefault(x => x.Consumer.UID == consumerUID);
				if (consumerViewModel != null)
					consumerViewModel.ExpandToThis();
				SelectedConsumer = consumerViewModel;
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var consumerDetailsViewModel = new ConsumerDetailsViewModel(new Consumer() { ParentUID = SelectedConsumer.Consumer.IsFolder ? SelectedConsumer.Consumer.UID : SelectedConsumer.Consumer.ParentUID }, false, true);
			if (DialogService.ShowModalWindow(consumerDetailsViewModel))
			{
				var consumerViewModel = new ConsumerViewModel(consumerDetailsViewModel.GetConsumer());
				if (SelectedConsumer.Consumer.IsFolder)
				{
					SelectedConsumer.AddChild(consumerViewModel);
					SelectedConsumer.IsExpanded = true;
				}
				else
				{
					SelectedConsumer.Parent.AddChild(consumerViewModel);
					SelectedConsumer.Parent.IsExpanded = true;
				}
				AllConsumers.Add(consumerViewModel);
				SelectedConsumer = consumerViewModel;
			}
		}
		bool CanAdd()
		{
			return SelectedConsumer != null;
		}

		public RelayCommand AddFolderCommand { get; private set; }
		void OnAddFolder()
		{
			var consumersFolderDetailsViewModel = new ConsumersFolderDetailsViewModel(new Consumer() { IsFolder = true, Parent = SelectedConsumer.Consumer }, false, true);
			if (DialogService.ShowModalWindow(consumersFolderDetailsViewModel))
			{
				var consumerViewModel = new ConsumerViewModel(consumersFolderDetailsViewModel.GetConsumer());
				SelectedConsumer.AddChild(consumerViewModel);
				SelectedConsumer.IsExpanded = true;
				AllConsumers.Add(consumerViewModel);
				SelectedConsumer = consumerViewModel;
			}
		}
		bool CanAddFolder()
		{
			return SelectedConsumer != null && SelectedConsumer.Consumer.IsFolder && DBCash.CurrentUser.UserPermissions.Any(x => x.PermissionType == PermissionType.EditConsumer);
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var consumer = SelectedConsumer.Consumer.IsFolder ?
				SelectedConsumer.ConsumersFolderDetails.GetConsumer() :
				SelectedConsumer.ConsumerDetails.GetConsumer();
			var dialogViewModel = SelectedConsumer.Consumer.IsFolder ? (SaveCancelDialogViewModel)
				new ConsumersFolderDetailsViewModel(consumer, false) :
				new ConsumerDetailsViewModel(consumer, false);
			if (DialogService.ShowModalWindow(dialogViewModel))
			{
				consumer = SelectedConsumer.Consumer.IsFolder ?
				((ConsumersFolderDetailsViewModel)dialogViewModel).GetConsumer() :
				((ConsumerDetailsViewModel)dialogViewModel).GetConsumer();
				SelectedConsumer.Update(consumer);
			}
		}
		bool CanEdit()
		{
			return SelectedConsumer != null && DBCash.CurrentUser.UserPermissions.Any(x=> x.PermissionType == PermissionType.EditConsumer);
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			if (SelectedConsumer.ChildrenCount > 0)
			{
				MessageBoxService.Show(string.Format("Группа \"{0}\" содержит абонентов. Удаление невозможно.", SelectedConsumer.Consumer.Name));
				return;
			}

			if (MessageBoxService.ShowQuestion(string.Format("Вы уверенны, что хотите удалить {0} \"{1}\"?", SelectedConsumer.Consumer.IsFolder ? "группу" : "абонента", SelectedConsumer.Consumer.Name)))
			{
				var selectedConsumer = SelectedConsumer;
				var parent = selectedConsumer.Parent;
				if (parent != null)
				{
					DBCash.DeleteConsumer(selectedConsumer.Consumer);

					var index = selectedConsumer.VisualIndex;
					parent.Nodes.Remove(selectedConsumer);
					index = Math.Min(index, parent.ChildrenCount - 1);
					SelectedConsumer = index >= 0 ? parent.GetChildByVisualIndex(index) : parent;
				}
			}
		}
		
		bool CanRemove()
		{
			return SelectedConsumer != null && SelectedConsumer.Parent != null && DBCash.CurrentUser.UserPermissions.Any(x => x.PermissionType == PermissionType.EditConsumer);
		}

		public RelayCommand ChangeParentCommand { get; private set; }
		void OnChangeParent()
		{
			var consumersChangeParentViewModel = new ConsumerChangeParentViewModel(SelectedConsumer.Consumer.UID);
			if (DialogService.ShowModalWindow(consumersChangeParentViewModel) && consumersChangeParentViewModel.SelectedConsumer != null)
			{
				var parentConsumerViewModel = AllConsumers.FirstOrDefault(x => x.Consumer.UID == consumersChangeParentViewModel.SelectedConsumer.Consumer.UID);
				if (parentConsumerViewModel != null)
				{
					SelectedConsumer.Consumer = DBCash.GetConsumer(SelectedConsumer.Consumer.UID);
					SelectedConsumer.Consumer.ParentUID = consumersChangeParentViewModel.SelectedConsumer.Consumer.UID;

					DBCash.SaveConsumer(SelectedConsumer.Consumer);

					var consumerViewModel = SelectedConsumer;
					SelectedConsumer.Parent.RemoveChild(SelectedConsumer);

					parentConsumerViewModel.AddChild(consumerViewModel);
					consumerViewModel.ExpandToThis();

					SelectedConsumer = consumerViewModel;
				}
			}
		}

		bool CanChangeParent()
		{
			return SelectedConsumer != null && SelectedConsumer.Parent != null && DBCash.CurrentUser.UserPermissions.Any(x => x.PermissionType == PermissionType.EditConsumer);
		}
		public RelayCommand OpenReceiptCommand { get; private set; }
		void OnOpenReceipt()
		{
			//Infrastructure.Common.Windows.DialogService.ShowModalWindow(new ReceiptViewModel(SelectedConsumer.Consumer));
			Infrastructure.Common.Windows.DialogService.ShowModalWindow(new ReportDesignerViewModel(new ReceiptTemplate()));
		}
		bool CanOpenReceipt()
		{
			return SelectedConsumer != null && !SelectedConsumer.Consumer.IsFolder;
		}

		public bool IsVisibility
		{
			get { return DBCash.CurrentUser.UserPermissions.Any(x => x.PermissionType == PermissionType.Consumer); }
		}
	}
}
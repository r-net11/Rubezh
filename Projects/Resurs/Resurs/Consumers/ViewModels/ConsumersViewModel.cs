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

			ConsumerListViewModel = new ConsumerListViewModel();
			ConsumerListViewModel.OnSelectedConsumerChanged += ConsumerListViewModel_OnSelectedConsumerChanged;
			ConsumerListViewModel.OnItemActivated += ConsumerListViewModel_OnItemActivated;

			FillAllConsumers();
		}

		void ConsumerListViewModel_OnItemActivated(ConsumerViewModel obj)
		{
			EditCommand.Execute();
		}

		void ConsumerListViewModel_OnSelectedConsumerChanged(ConsumerViewModel consumer)
		{
			SelectedConsumer = consumer;
		}

		public ConsumerListViewModel ConsumerListViewModel { get; private set; }

		public ConsumerViewModel SelectedConsumer
		{
			get { return ConsumerListViewModel.SelectedConsumer; }
			set
			{
				if (ConsumerListViewModel.SelectedConsumer != value)
					ConsumerListViewModel.SelectedConsumer = value;
				OnPropertyChanged(() => SelectedConsumer);
			}
		}
				
		public List<ConsumerViewModel> AllConsumers;

		public void FillAllConsumers()
		{
			AllConsumers = new List<ConsumerViewModel>();
			AddChildPlainConsumers(ConsumerListViewModel.RootConsumer);
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
				Bootstrapper.MainViewModel.SelectedTabIndex = 1;
				SelectedConsumer = consumerViewModel;
				if (SelectedConsumer.ConsumerDetails != null)
					SelectedConsumer.ConsumerDetails.SelectedTabIndex = 1;
				Bootstrapper.MainViewModel.SelectedTabIndex = 1;
			}
		}

		public BillViewModel FindBillViewModel(Guid billUid)
		{
			foreach (var consumer in AllConsumers)
				if (consumer.GetConsumerDetails() != null && consumer.GetConsumerDetails().BillsViewModel != null)
					foreach (var bill in consumer.GetConsumerDetails().BillsViewModel.Bills)
						if (bill.Uid == billUid)
							return bill;
			return null;
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var consumerDetailsViewModel = new ConsumerDetailsViewModel(new Consumer 
			{ 
				ParentUID = SelectedConsumer.Consumer.IsFolder ? SelectedConsumer.Consumer.UID : SelectedConsumer.Consumer.ParentUID,
				Bills = new List<Bill> { new Bill() }
			}, false, true);
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
			return SelectedConsumer != null && DBCash.CheckPermission(PermissionType.EditConsumer);
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
			return SelectedConsumer != null && DBCash.CheckPermission(PermissionType.EditConsumer);
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
			return SelectedConsumer != null && SelectedConsumer.Parent != null && DBCash.CheckPermission(PermissionType.EditConsumer);
		}

		public RelayCommand ChangeParentCommand { get; private set; }
		void OnChangeParent()
		{
			var consumerChangeParentViewModel = new ConsumerChangeParentViewModel(SelectedConsumer.Consumer.UID);
			if (DialogService.ShowModalWindow(consumerChangeParentViewModel) && consumerChangeParentViewModel.SelectedConsumer != null)
			{
				var parentConsumerViewModel = AllConsumers.FirstOrDefault(x => x.Consumer.UID == consumerChangeParentViewModel.SelectedConsumer.Consumer.UID);
				if (parentConsumerViewModel != null)
				{
					SelectedConsumer.Consumer = DBCash.GetConsumer(SelectedConsumer.Consumer.UID);
					SelectedConsumer.Consumer.ParentUID = consumerChangeParentViewModel.SelectedConsumer.Consumer.UID;

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
			return SelectedConsumer != null && SelectedConsumer.Parent != null && DBCash.CheckPermission(PermissionType.EditConsumer);
		}
		public bool IsVisible
		{
			get { return DBCash.CheckPermission(PermissionType.ViewConsumer); }
		}
	}
}
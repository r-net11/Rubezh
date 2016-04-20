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

		public ConsumerDetailsViewModel FindConsumerDetailsViewModel(Guid consumerUID)
		{
			return AllConsumers.Where(x => x.Consumer.UID == consumerUID).Select(x => x.GetConsumerDetails()).FirstOrDefault();
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var consumerDetailsViewModel = new ConsumerDetailsViewModel(new Consumer
			{
				ParentUID = SelectedConsumer.Consumer.IsFolder ? SelectedConsumer.Consumer.UID : SelectedConsumer.Consumer.ParentUID,
			}, false, true);
			if (DialogService.ShowModalWindow(consumerDetailsViewModel))
			{
				var consumer = consumerDetailsViewModel.GetConsumer();
				DbCache.SaveConsumer(consumer);
				var consumerViewModel = new ConsumerViewModel(consumer);
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
				DbCache.AddJournalForUser(JournalType.AddConsumer, SelectedConsumer.Consumer);
				UpdateDeviceViewModels(null, SelectedConsumer.GetConsumer());
			}
		}
		bool CanAdd()
		{
			return SelectedConsumer != null && DbCache.CheckPermission(PermissionType.EditConsumer);
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
				DbCache.AddJournalForUser(JournalType.AddConsumer, SelectedConsumer.Consumer, "Добавление группы абонентов");
			}
		}
		bool CanAddFolder()
		{
			return SelectedConsumer != null && SelectedConsumer.Consumer.IsFolder && DbCache.CheckPermission(PermissionType.EditConsumer);
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var oldConsumer = SelectedConsumer.GetConsumer();

			var dialogViewModel = SelectedConsumer.Consumer.IsFolder ? (SaveCancelDialogViewModel)
				new ConsumersFolderDetailsViewModel(oldConsumer, false) :
				new ConsumerDetailsViewModel(oldConsumer, false);

			if (DialogService.ShowModalWindow(dialogViewModel))
			{
				var newConsumer = SelectedConsumer.Consumer.IsFolder ?
					((ConsumersFolderDetailsViewModel)dialogViewModel).GetConsumer() :
					((ConsumerDetailsViewModel)dialogViewModel).GetConsumer();
				DbCache.SaveConsumer(newConsumer);
				SelectedConsumer.Update(newConsumer);
				DbCache.AddJournalForUser(JournalType.EditConsumer, newConsumer);
				UpdateDeviceViewModels(oldConsumer, newConsumer);
			}
		}
		bool CanEdit()
		{
			return SelectedConsumer != null && DbCache.CheckPermission(PermissionType.EditConsumer);
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
					DbCache.DeleteConsumer(selectedConsumer.Consumer);

					var index = selectedConsumer.VisualIndex;
					parent.Nodes.Remove(selectedConsumer);
					index = Math.Min(index, parent.ChildrenCount - 1);
					SelectedConsumer = index >= 0 ? parent.GetChildByVisualIndex(index) : parent;
					DbCache.AddJournalForUser(JournalType.DeleteConsumer, selectedConsumer.Consumer);
					UpdateDeviceViewModels(selectedConsumer.GetConsumer(), null);
				}
			}
		}

		bool CanRemove()
		{
			return SelectedConsumer != null && SelectedConsumer.Parent != null && DbCache.CheckPermission(PermissionType.EditConsumer);
		}

		public RelayCommand ChangeParentCommand { get; private set; }
		void OnChangeParent()
		{
			var selectConsumerViewModel = new SelectConsumerViewModel("Выбор группы для перемещения", SelectedConsumer.Consumer.UID, true);
			if (DialogService.ShowModalWindow(selectConsumerViewModel) && selectConsumerViewModel.SelectedConsumer != null)
			{
				var parentConsumerViewModel = AllConsumers.FirstOrDefault(x => x.Consumer.UID == selectConsumerViewModel.SelectedConsumer.Consumer.UID);
				if (parentConsumerViewModel != null)
				{
					SelectedConsumer.Consumer = DbCache.GetConsumer(SelectedConsumer.Consumer.UID);
					SelectedConsumer.Consumer.ParentUID = selectConsumerViewModel.SelectedConsumer.Consumer.UID;

					DbCache.SaveConsumer(SelectedConsumer.Consumer);

					var consumerViewModel = SelectedConsumer;
					SelectedConsumer.Parent.RemoveChild(SelectedConsumer);

					parentConsumerViewModel.AddChild(consumerViewModel);
					consumerViewModel.ExpandToThis();

					SelectedConsumer = consumerViewModel;
					DbCache.AddJournalForUser(JournalType.EditConsumer, 
						SelectedConsumer.Consumer, 
						string.Format("Перемещение в группу \"{0}\"", selectConsumerViewModel.SelectedConsumer.Consumer.Name));
				}
			}
		}

		bool CanChangeParent()
		{
			return SelectedConsumer != null && SelectedConsumer.Parent != null && DbCache.CheckPermission(PermissionType.EditConsumer);
		}
		public bool IsVisible
		{
			get { return DbCache.CheckPermission(PermissionType.ViewConsumer); }
		}

		void UpdateDeviceViewModels(Consumer oldConsumer, Consumer newConsumer)
		{
			if (oldConsumer == null && newConsumer == null)
				return;

			var devicesToRemove = oldConsumer == null ?
				null :
				newConsumer == null ?
				oldConsumer.Devices :
				oldConsumer.Devices.Except(newConsumer.Devices);

			var devicesToAdd = newConsumer == null ?
				null :
				oldConsumer == null ?
				newConsumer.Devices :
				newConsumer.Devices.Except(oldConsumer.Devices);

			if (devicesToRemove != null)
				foreach (var device in devicesToRemove)
				{
					var deviceViewModel = Bootstrapper.MainViewModel.DevicesViewModel.AllDevices.FirstOrDefault(x => x.Device.UID == device.UID);
					if (deviceViewModel != null)
					{
						deviceViewModel.Device.Consumer = null;
						deviceViewModel.Device.ConsumerUID = null;
						DbCache.AddJournalForUser(JournalType.EditDevice, deviceViewModel.Device, string.Format("Разорвана связь с лицевым счетом [{0}]{1}", oldConsumer.Number, oldConsumer.Name));
					}
				}

			if (devicesToAdd != null)
				foreach (var device in devicesToAdd)
				{
					var deviceViewModel = Bootstrapper.MainViewModel.DevicesViewModel.AllDevices.FirstOrDefault(x => x.Device.UID == device.UID);
					if (deviceViewModel != null)
					{
						deviceViewModel.Device.Consumer = newConsumer;
						deviceViewModel.Device.ConsumerUID = newConsumer.UID;
						DbCache.AddJournalForUser(JournalType.EditDevice, deviceViewModel.Device, string.Format("Добавлена связь с лицевым счетом [{0}]{1}", newConsumer.Number, newConsumer.Name));
					}
				}
		}
	}
}
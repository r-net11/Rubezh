using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Documents;
using FiresecAPI.Automation;

namespace AutomationModule.ViewModels
{
	public class SendEmailStepViewModel : BaseStepViewModel
	{
		private SendEmailArguments SendEmailArguments { get; set; }

		public ArgumentViewModel EMailAddressFromArgument { get; private set; }
		public ObservableCollection<AddRemoveArgumentViewModel> EMailAddressToArguments { get; private set; }
		public ArgumentViewModel EMailTitleArgument { get; private set; }
		public ArgumentViewModel EMailContentArgument { get; private set; }
		public ObservableCollection<AddRemoveArgumentViewModel> EMailAttachedFileArguments { get; private set; }
		public ArgumentViewModel SmtpArgument { get; private set; }
		public ArgumentViewModel PortArgument { get; private set; }
		public ArgumentViewModel LoginArgument { get; private set; }
		public ArgumentViewModel PasswordArgument { get; private set; }

		public SendEmailStepViewModel(StepViewModel stepViewModel) : base(stepViewModel)
		{
			SendEmailArguments = stepViewModel.Step.SendEmailArguments;

			// Параметры сообщения
			EMailAddressFromArgument = new ArgumentViewModel(SendEmailArguments.EMailAddressFromArgument, stepViewModel.Update, UpdateContent);
			//EMailAddressToArgument = new ArgumentViewModel(SendEmailArguments.EMailAddressToArgument, stepViewModel.Update, UpdateContent);
			InitEMailAddressToArguments();
			EMailTitleArgument = new ArgumentViewModel(SendEmailArguments.EMailTitleArgument, stepViewModel.Update, UpdateContent);
			EMailContentArgument = new ArgumentViewModel(SendEmailArguments.EMailContentArgument, stepViewModel.Update, UpdateContent);
			InitEMailAttachedFileArguments();

			// Параметры подключения
			SmtpArgument = new ArgumentViewModel(SendEmailArguments.SmtpArgument, stepViewModel.Update, UpdateContent);
			PortArgument = new ArgumentViewModel(SendEmailArguments.PortArgument, stepViewModel.Update, UpdateContent);
			LoginArgument = new ArgumentViewModel(SendEmailArguments.LoginArgument, stepViewModel.Update, UpdateContent);
			PasswordArgument = new ArgumentViewModel(SendEmailArguments.PasswordArgument, stepViewModel.Update, UpdateContent);
		}

		private void InitEMailAddressToArguments()
		{
			EMailAddressToArguments = new ObservableCollection<AddRemoveArgumentViewModel>();
			if (SendEmailArguments.EMailAddressToArguments.Count == 0)
			{
				AddFirstAddressToArgument();
			}
			else
			{
				for (var i = 0; i < SendEmailArguments.EMailAddressToArguments.Count; i++)
				{
					AddAddressToArgument(SendEmailArguments.EMailAddressToArguments[i], i == 0);
				}
			}
		}

		/// <summary>
		/// Добавляет в конец коллекции EMailAddressToArguments новый элемент
		/// </summary>
		/// <param name="argument">Аргумент процедуры, хранящий адрес получателя</param>
		/// <param name="canAdd">true - для первого элемента коллекции, false - для всех остальных</param>
		private void AddAddressToArgument(Argument argument, bool canAdd)
		{
			var addingArgument = argument;

			if (addingArgument == null)
			{
				addingArgument = new Argument();
				SendEmailArguments.EMailAddressToArguments.Add(addingArgument);
			}
			var addRemoveArgumentViewModel = canAdd
			? new AddRemoveArgumentViewModel(addingArgument, UpdateDescriptionHandler, UpdateContent, canAdd: true, addAction: AddNextAddressToArgument)
			: new AddRemoveArgumentViewModel(addingArgument, UpdateDescriptionHandler, UpdateContent, removeAction: RemoveAddressToArgument);
			addRemoveArgumentViewModel.Update(Procedure, ExplicitType.String, false);
			EMailAddressToArguments.Add(addRemoveArgumentViewModel);
		}

		/// <summary>
		/// Добавляет в пустую коллекцию EMailAddressToArguments новый элемент
		/// </summary>
		private void AddFirstAddressToArgument()
		{
			AddAddressToArgument(null, true);
		}

		/// <summary>
		/// Добавляет в конец коллекции EMailAddressToArguments новый элемент
		/// </summary>
		private void AddNextAddressToArgument()
		{
			AddAddressToArgument(null, false);
		}

		/// <summary>
		/// Удаляет из коллекции EMailAddressToArguments указанный элемент
		/// </summary>
		/// <param name="addRemoveArgumentViewModel">Удаляемый элемент</param>
		private void RemoveAddressToArgument(AddRemoveArgumentViewModel addRemoveArgumentViewModel)
		{
			EMailAddressToArguments.Remove(addRemoveArgumentViewModel);
			SendEmailArguments.EMailAddressToArguments.Remove(addRemoveArgumentViewModel.Argument.Argument);
		}

		private void InitEMailAttachedFileArguments()
		{
			EMailAttachedFileArguments = new ObservableCollection<AddRemoveArgumentViewModel>();
			if (SendEmailArguments.EMailAttachedFileArguments.Count == 0)
			{
				AddFirstAttachedFileArgument();
			}
			else
			{
				for (var i = 0; i < SendEmailArguments.EMailAttachedFileArguments.Count; i++)
				{
					AddAttachedFileArgument(SendEmailArguments.EMailAttachedFileArguments[i], i == 0);
				}
			}
		}

		public override void UpdateContent()
		{
			EMailAddressFromArgument.Update(Procedure, ExplicitType.String, isList: false);
			foreach (var eMailAddressToArgument in EMailAddressToArguments)
			{
				eMailAddressToArgument.Update(Procedure, ExplicitType.String, false);
			}
			EMailTitleArgument.Update(Procedure, ExplicitType.String, isList: false);
			EMailContentArgument.Update(Procedure, ExplicitType.String, isList: false);
			foreach (var eMailAttachedFileArgument in EMailAttachedFileArguments)
			{
				eMailAttachedFileArgument.Update(Procedure, ExplicitType.String, false);
			}
			SmtpArgument.Update(Procedure, ExplicitType.String, isList: false);
			PortArgument.Update(Procedure, ExplicitType.Integer, isList: false);
			LoginArgument.Update(Procedure, ExplicitType.String, isList: false);
			PasswordArgument.Update(Procedure, ExplicitType.String, isList: false);
		}

		/// <summary>
		/// Выводит значение шага процедуры в списке шагов процедуры
		/// </summary>
		public override string Description
		{
			get
			{
				var addressTo = EMailAddressToArguments.Count > 0
					? EMailAddressToArguments[0].Argument.Description
					: string.Empty;
				return string.Format("От кого: {0} Кому: {1} Адрес сервера: {2} Порт: {3}",
					EMailAddressFromArgument.Description,
					addressTo,
					SmtpArgument.Description,
					PortArgument.Description);
			}
		}

		/// <summary>
		/// Добавляет в конец коллекции EMailAttachedFileArguments новый элемент
		/// </summary>
		/// <param name="argument">Аргумент процедуры, хранящий название вложенного файла</param>
		/// <param name="canAdd">true - для первого элемента коллекции, false - для всех остальных</param>
		private void AddAttachedFileArgument(Argument argument, bool canAdd)
		{
			var addingArgument = argument;

			if (addingArgument == null)
			{
				addingArgument = new Argument();
				SendEmailArguments.EMailAttachedFileArguments.Add(addingArgument);
			}
			var addRemoveArgumentViewModel = canAdd
			? new AddRemoveArgumentViewModel(addingArgument, UpdateDescriptionHandler, UpdateContent, canAdd: true, addAction: AddNextAttachedFileArgument)
			: new AddRemoveArgumentViewModel(addingArgument, UpdateDescriptionHandler, UpdateContent, removeAction: RemoveAttachedFileArgument);
			addRemoveArgumentViewModel.Update(Procedure, ExplicitType.String, false);
			EMailAttachedFileArguments.Add(addRemoveArgumentViewModel);
		}

		/// <summary>
		/// Добавляет в пустую коллекцию EMailAttachedFileArguments новый элемент
		/// </summary>
		private void AddFirstAttachedFileArgument()
		{
			AddAttachedFileArgument(null, true);
		}

		/// <summary>
		/// Добавляет в конец коллекции EMailAttachedFileArguments новый элемент
		/// </summary>
		private void AddNextAttachedFileArgument()
		{
			AddAttachedFileArgument(null, false);
		}

		/// <summary>
		/// Удаляет из коллекции EMailAttachedFileArguments указанный элемент
		/// </summary>
		/// <param name="addRemoveArgumentViewModel">Удаляемый элемент</param>
		private void RemoveAttachedFileArgument(AddRemoveArgumentViewModel addRemoveArgumentViewModel)
		{
			EMailAttachedFileArguments.Remove(addRemoveArgumentViewModel);
			SendEmailArguments.EMailAttachedFileArguments.Remove(addRemoveArgumentViewModel.Argument.Argument);
		}
	}
}

using Common;
using System;
using System.Windows;
using Localization.Common.InfrastructureCommon;

namespace Infrastructure.Common.Windows.ViewModels
{
	public class MessageBoxViewModel : DialogViewModel
	{
		public MessageBoxViewModel(string title, string message, MessageBoxButton messageBoxButton, MessageBoxImage messageBoxImage, bool isException = false, int width = 350, int height = 120)
		{
			OkCommand = new RelayCommand(OnOk);
			CancelCommand = new RelayCommand(OnCancel);
			YesCommand = new RelayCommand(OnYes);
			NoCommand = new RelayCommand(OnNo);
			CopyCommand = new RelayCommand(OnCopy);

			Title = title ?? CommonResources.Strazh_UpperCase;

			if (title == null && isException)
				Title = CommonResources.ErrorWhileAppWork;

			IsException = isException;
			Message = message;
			SetButtonsVisibility(messageBoxButton);
			SetImageVisibility(messageBoxImage);

			Result = MessageBoxResult.None;
			Sizable = false;

			Width = width;
			Height = height;
		}

		private void SetButtonsVisibility(MessageBoxButton messageBoxButton)
		{
			switch (messageBoxButton)
			{
				case MessageBoxButton.OK:
					IsOkButtonVisible = true;
					break;

				case MessageBoxButton.OKCancel:
					IsOkButtonVisible = true;
					IsCancelButtonVisible = true;
					break;

				case MessageBoxButton.YesNo:
					IsYesButtonVisible = true;
					IsNoButtonVisible = true;
					break;

				case MessageBoxButton.YesNoCancel:
					IsYesButtonVisible = true;
					IsNoButtonVisible = true;
					IsCancelButtonVisible = true;
					break;
			}
		}

		private void SetImageVisibility(MessageBoxImage messageBoxImage)
		{
			switch (messageBoxImage)
			{
				case MessageBoxImage.Information:
					IsInformationImageVisible = true;
					break;

				case MessageBoxImage.Question:
					IsQuestionImageVisible = true;
					break;

				case MessageBoxImage.Warning:
					IsWarningImageVisible = true;
					break;

				case MessageBoxImage.Error:
					IsErrorImageVisible = true;
					break;
			}
		}

		public bool IsOkButtonVisible { get; private set; }

		public bool IsCancelButtonVisible { get; private set; }

		public bool IsYesButtonVisible { get; private set; }

		public bool IsNoButtonVisible { get; private set; }

		public bool IsInformationImageVisible { get; private set; }

		public bool IsQuestionImageVisible { get; private set; }

		public bool IsWarningImageVisible { get; private set; }

		public bool IsErrorImageVisible { get; private set; }

		public bool IsException { get; private set; }

		public string Message { get; set; }

		public MessageBoxResult Result { get; set; }

		public RelayCommand OkCommand { get; private set; }

		private void OnOk()
		{
			Result = MessageBoxResult.OK;
			Close(true);
		}

		public RelayCommand CancelCommand { get; private set; }

		private void OnCancel()
		{
			Result = MessageBoxResult.Cancel;
			Close(true);
		}

		public RelayCommand YesCommand { get; private set; }

		private void OnYes()
		{
			Result = MessageBoxResult.Yes;
			Close(true);
		}

		public RelayCommand NoCommand { get; private set; }

		private void OnNo()
		{
			Result = MessageBoxResult.No;
			Close(true);
		}

		public RelayCommand CopyCommand { get; private set; }

		private void OnCopy()
		{
			try
			{
				Clipboard.SetText(Message);
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове MessageBoxViewModel.CopyCommand");
				return;
			}
		}

		public MessageBoxImage MessageBoxImage
		{
			get
			{
				var messageBoxImage = MessageBoxImage.None;
				if (IsInformationImageVisible)
					messageBoxImage = MessageBoxImage.Information;
				else if (IsQuestionImageVisible)
					messageBoxImage = MessageBoxImage.Question;
				else if (IsWarningImageVisible)
					messageBoxImage = MessageBoxImage.Warning;
				else if (IsErrorImageVisible)
					messageBoxImage = MessageBoxImage.Error;
				return messageBoxImage;
			}
		}

		public MessageBoxButton MessageBoxButton
		{
			get
			{
				var messageBoxButton = MessageBoxButton.OK;
				if (IsOkButtonVisible)
				{
					if (IsCancelButtonVisible)
						messageBoxButton = MessageBoxButton.OKCancel;
				}
				else
					messageBoxButton = IsCancelButtonVisible ? MessageBoxButton.YesNoCancel : MessageBoxButton.YesNo;
				return messageBoxButton;
			}
		}

		public int Width { get; private set; }

		public int Height { get; private set; }
	}
}
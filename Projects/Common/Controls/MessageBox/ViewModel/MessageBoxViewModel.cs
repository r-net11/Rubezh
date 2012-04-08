﻿using System;
using System.Windows;
using Infrastructure.Common;
using Common;

namespace Controls.MessageBox
{
    public class MessageBoxViewModel : DialogContent
    {
        public MessageBoxViewModel(string message, MessageBoxButton messageBoxButton, MessageBoxImage messageBoxImage, bool isException = false)
        {
            OkCommand = new RelayCommand(OnOk);
            CancelCommand = new RelayCommand(OnCancel);
            YesCommand = new RelayCommand(OnYes);
            NoCommand = new RelayCommand(OnNo);
            CopyCommand = new RelayCommand(OnCopy);
            Title = "Firesec";
            if (isException)
                Title = "В результате работы программы возникло исключение";

            IsException = isException;
            Message = message;
            SetButtonsVisibility(messageBoxButton);
            SetImageVisibility(messageBoxImage);

            Result = MessageBoxResult.None;
        }

        void SetButtonsVisibility(MessageBoxButton messageBoxButton)
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

        void SetImageVisibility(MessageBoxImage messageBoxImage)
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
        void OnOk()
        {
            Result = MessageBoxResult.OK;
            Close(true);
        }

        public RelayCommand CancelCommand { get; private set; }
        void OnCancel()
        {
            Result = MessageBoxResult.Cancel;
            Close(true);
        }

        public RelayCommand YesCommand { get; private set; }
        void OnYes()
        {
            Result = MessageBoxResult.Yes;
            Close(true);
        }

        public RelayCommand NoCommand { get; private set; }
        void OnNo()
        {
            Result = MessageBoxResult.No;
            Close(true);
        }

        public RelayCommand CopyCommand { get; private set; }
        void OnCopy()
        {
            try
            {
                Clipboard.SetText(Message);
            }
            catch (Exception e)
            {
				Logger.Error(e);
                return;
            }
        }
    }
}
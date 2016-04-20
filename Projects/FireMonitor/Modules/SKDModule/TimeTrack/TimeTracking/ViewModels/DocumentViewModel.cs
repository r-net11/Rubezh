using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using RubezhAPI.SKD;
using RubezhClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Microsoft.Win32;
using SKDModule.Common;
using SKDModule.Events;

namespace SKDModule.ViewModels
{
	public class DocumentViewModel : BaseViewModel
	{
		public TimeTrackDocument Document { get; private set; }

		public DocumentViewModel(TimeTrackDocument timeTrackDocument)
		{
			Document = timeTrackDocument;
			Update();
		}

		public void Update()
		{
			Name = Document.TimeTrackDocumentType.Name;
			ShortName = Document.TimeTrackDocumentType.ShortName;
			StartDateTime = Document.StartDateTime.ToString("yyyy-MM-dd HH:mm");
			EndDateTime = Document.EndDateTime.ToString("yyyy-MM-dd HH:mm");
			_fileName = Document.FileName;

			OnPropertyChanged(() => Name);
			OnPropertyChanged(() => ShortName);
			OnPropertyChanged(() => StartDateTime);
			OnPropertyChanged(() => EndDateTime);
			OnPropertyChanged(() => FileName);
			OnPropertyChanged(() => HasFile);
		}

		public void Update(TimeTrackDocument timeTrackDocument)
		{
			Document = timeTrackDocument;
			Update();
		}

		public string Name { get; private set; }
		public string ShortName { get; private set; }
		public string StartDateTime { get; private set; }
		public string EndDateTime { get; private set; }

		string _fileName;
		public string FileName
		{
			get { return _fileName; }
			set
			{
				_fileName = value;
				Document.FileName = _fileName;
				var operationResult = ClientManager.FiresecService.EditTimeTrackDocument(Document);
				if (operationResult.HasError)
				{
					MessageBoxService.ShowWarning(operationResult.Error);
				}
				OnPropertyChanged(() => FileName);
				OnPropertyChanged(() => HasFile);
			}
		}

		public void AddFile()
		{
			var openFileDialog = new OpenFileDialog();
			if (openFileDialog.ShowDialog() == true)
			{
				var docsDirectory = AppDataFolderHelper.GetFolder("Documents");
				if (!Directory.Exists(docsDirectory))
					Directory.CreateDirectory(docsDirectory);
				var fileName = openFileDialog.FileName;
				var length = new FileInfo(fileName).Length;
				if (length > 52428800)
				{
					MessageBoxService.Show("Размер загружаемого файла не должен превышать 50 мб");
					return;
				}
				var files = Directory.GetFiles(docsDirectory).Select(x => Path.GetFileName(x));
				var newFileName = CopyHelper.CopyFileName(openFileDialog.SafeFileName, files);
				if (newFileName.Length >= 100)
				{
					MessageBoxService.Show("Имя файла не может быть длиннее 100 символов");
					return;
				}
				var newFullFileName = Path.Combine(docsDirectory, newFileName);
				File.Copy(fileName, newFullFileName);
				FileName = newFileName;
				ServiceFactory.Events.GetEvent<EditDocumentEvent>().Publish(Document);
			}
		}

		public void OpenFile()
		{
			var fileName = AppDataFolderHelper.GetFileInFolder("Documents", FileName);
			if (File.Exists(fileName))
			{
				try
				{
					Process.Start(fileName);
				}
				catch (Exception e)
				{
					ShowOpenWithDialog(fileName);
				}
			}
			else
				MessageBoxService.Show("Файл не существует");
		}

		public void RemoveFile()
		{
			if (MessageBoxService.ShowQuestion("Вы действительно хотите удалить файл оправдательного документа?"))
			{
				var fileName = AppDataFolderHelper.GetFileInFolder("Documents", FileName);
				if (File.Exists(fileName))
				{
					File.Delete(fileName);
				}
				else
					MessageBoxService.Show("Файл не существует");
				FileName = null;
				ServiceFactory.Events.GetEvent<EditDocumentEvent>().Publish(Document);
			}
		}
		

		public bool HasFile { get { return FileName != null; } }

		void ShowOpenWithDialog(string path)
		{
			var args = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "shell32.dll");
			args += ",OpenAs_RunDLL " + path;
			Process.Start("rundll32.exe", args);
		}
	}
}
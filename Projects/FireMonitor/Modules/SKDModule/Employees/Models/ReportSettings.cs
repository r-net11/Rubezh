using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;
using ReactiveUI;
using ReportSystem.Api.Interfaces;
using StrazhAPI.Printing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SKDModule.Employees.Models
{
	public sealed class ReportSettings : BaseViewModel
	{
		private bool _isUseAttachedTemplates;
		private bool _isUseCutMarks;
		private int _width;
		private int _height;
		private Tuple<Guid, string> _selectedTemplate;
		private IPaperKindSetting _selectedPaperKindSetting;
		private List<Tuple<Guid, string>> _templateNames;
		private List<IPaperKindSetting> _paperKindSettings;

		public int Width
		{
			get { return _width; }
			set
			{
				_width = value;
				OnPropertyChanged(() => Width);
			}
		}

		public int Height
		{
			get { return _height; }
			set
			{
				_height = value;
				OnPropertyChanged(() => Height);
			}
		}

		public bool IsUseCutMarks
		{
			get { return _isUseCutMarks; }
			set
			{
				if (_isUseCutMarks == value) return;
				_isUseCutMarks = value;
				OnPropertyChanged(() => IsUseCutMarks);
			}
		}

		public bool IsUseAttachedTemplates
		{
			get { return _isUseAttachedTemplates; }
			set
			{
				if (_isUseAttachedTemplates == value) return;
				_isUseAttachedTemplates = value;
				OnPropertyChanged(() => IsUseAttachedTemplates);
			}
		}

		public Tuple<Guid, string> SelectedTemplate
		{
			get { return _selectedTemplate; }
			set
			{
				_selectedTemplate = value;
				OnPropertyChanged(() => SelectedTemplate);
			}
		}

		public IPaperKindSetting SelectedPaperKindSetting
		{
			get { return _selectedPaperKindSetting; }
			set
			{
				if (_selectedPaperKindSetting == value) return;
				_selectedPaperKindSetting = value;
				OnPropertyChanged(() => SelectedPaperKindSetting);
			}
		}

		public List<Tuple<Guid, string>> TemplateNames
		{
			get { return _templateNames; }
			private set
			{
				_templateNames = value;
				OnPropertyChanged(() => TemplateNames);
			}
		}

		public List<IPaperKindSetting> PaperKindSettings
		{
			get { return _paperKindSettings; }
			set
			{
				_paperKindSettings = value;
				OnPropertyChanged(() => PaperKindSettings);
			}
		}

		public ReportSettings()
		{
			IsUseCutMarks = true;
			IsUseAttachedTemplates = true;
			PaperKindSettings = new PaperKindSettingsFactory().GetAllPaperKindSettings();
			SelectedPaperKindSetting = PaperKindSettings.FirstOrDefault();

			this.WhenAny(x => x.SelectedPaperKindSetting, x => x.Value)
				.Subscribe(value =>
				{
					if (value == null)
						return;

					Width = value.Width;
					Height = value.Height;
				});
		}

		public void LoadTemplatesInOrganisation(Guid organisationId)
		{
			Task.Factory.StartNew(() => FiresecManager.FiresecService.GetTemplateNames(organisationId))
				.ContinueWith(t =>
				{
					if (t.Result != null && !t.Result.HasError)
					{
						TemplateNames = t.Result.Result.ToList();
						SelectedTemplate = TemplateNames.FirstOrDefault();
					}
				}, TaskContinuationOptions.OnlyOnRanToCompletion);
		}

		public PrintReportSettings ToDTO()
		{
			return new PrintReportSettings
			{
				IsUseCutMarks = IsUseCutMarks,
				PaperKindSetting = SelectedPaperKindSetting,
				TemplateGuid = IsUseAttachedTemplates ? (Guid?)null : SelectedTemplate.Item1
			};
		}
	}
}

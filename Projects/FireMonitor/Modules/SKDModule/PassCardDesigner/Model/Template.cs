using Infrastructure.Common.Windows.ViewModels;
using System;
using ReactiveUI;

namespace SKDModule.PassCardDesigner.Model
{
	public class Template : BaseViewModel
	{
		#region Fields
		//Максимальные ширина и высота контрола превью. Можно увеличить/уменьшить, при изменении размеров окна создания шаблона пропуска.
		//Необходимо для расчёта соотношения контрола для предварительного просмотра изображения
		private const int MaxPreviewWidth = 300;
		private const int MaxPreviewHeight = 240;

		private bool _isDualTemplateEnabled;
		private string _caption;
		private string _description;
		private Guid _organisationGuid;
		private Guid _guid;
		private double _width;
		private double _height;
		#endregion

		#region Properties
		public TemplateSide Front { get; set; }

		public TemplateSide Back { get; set; }

		public double Width
		{
			get { return _width; }
			set
			{
				_width = value;
				OnPropertyChanged(() => Width);
			}
		}

		public double Height
		{
			get { return _height; }
			set
			{
				_height = value;
				OnPropertyChanged(() => Height);
			}
		}

		public Guid UID
		{
			get { return _guid; }
			private set
			{
				_guid = value;
				OnPropertyChanged(() => UID);
			}
		}

		public Guid OrganisationUID
		{
			get { return _organisationGuid; }
			set
			{
				_organisationGuid = value;
				OnPropertyChanged(() => OrganisationUID);
			}
		}

		public bool IsDualTemplateEnabled
		{
			get { return _isDualTemplateEnabled; }
			set
			{
				_isDualTemplateEnabled = value;
				OnPropertyChanged(() => IsDualTemplateEnabled);
			}
		}

		public string Caption
		{
			get { return _caption; }
			set
			{
				_caption = value;
				OnPropertyChanged(() => Caption);
			}
		}

		public string Description
		{
			get { return _description; }
			set
			{
				_description = value;
				OnPropertyChanged(() => Description);
			}
		}
		#endregion

		public Template()
		{
			UID = Guid.NewGuid();
			Front =  new TemplateSide();
			Back = new TemplateSide();

			this.WhenAny(x => x.Width, x => x.Value)
				.Subscribe(value => SetPreviewProportions());

			this.WhenAny(x => x.Height, x => x.Value)
				.Subscribe(value => SetPreviewProportions());
		}

		public Template(StrazhAPI.SKD.PassCardTemplate template)
			: this()
		{
			if (template == null) return;

			UID = template.UID;
			OrganisationUID = template.OrganisationUID;
			Back = new TemplateSide(template.Back);
			Front = new TemplateSide(template.Front);
			IsDualTemplateEnabled = template.IsDualTemplateEnabled;
			Caption = template.Caption;
			Description = template.Description;
			Width = template.Width;
			Height = template.Height;
		}

		public StrazhAPI.SKD.PassCardTemplate ToDTO()
		{
			return new StrazhAPI.SKD.PassCardTemplate
			{
				UID = UID,
				OrganisationUID = OrganisationUID,
				Back = Back == null ? null : Back.ToDTO(),
				Front = Front == null ? null : Front.ToDTO(),
				IsDualTemplateEnabled = IsDualTemplateEnabled,
				Caption = Caption,
				Description = Description,
				Width = Width,
				Height = Height
			};
		}

		public void Save()
		{
			Front.Save(Width, Height);

			if(IsDualTemplateEnabled)
				Back.Save(Width, Height);
		}

		/// <summary>
		/// Устанавливает соотношения сторон контрола превью.
		/// </summary>
		private void SetPreviewProportions()
		{
			Front.PreviewImage.SetPreviewProportions(MaxPreviewWidth, MaxPreviewHeight, Width, Height);
			Back.PreviewImage.SetPreviewProportions(MaxPreviewWidth, MaxPreviewHeight, Width, Height);
		}
	}
}

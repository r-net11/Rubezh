using Common;
using DevExpress.XtraReports.UI;
using FiresecClient;
using Infrastructure.Client.Converters;
using Infrastructure.Client.Images;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Microsoft.Win32;
using SKDModule.Reports;
using StrazhAPI.Extensions;
using StrazhAPI.SKD;
using System;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SKDModule.PassCardDesigner.Model
{
	public class TemplateSide : BaseViewModel
	{
		#region Fields
		private bool _isUseCustomImage;
		private string _backgroundSourceName;
		private PassCardTemplateReport _report;
		private Guid? _tmpGuid;
		private PreviewImage _previewImage;
		private WatermarkImage _watermarkImage;
		#endregion

		#region Properties

		public PreviewImage PreviewImage
		{
			get { return _previewImage; }
			set
			{
				_previewImage = value;
				OnPropertyChanged(() => PreviewImage);
			}
		}

		public WatermarkImage WatermarkImage
		{
			get { return _watermarkImage; }
			set
			{
				_watermarkImage = value;
				OnPropertyChanged(() => WatermarkImage);
			}
		}

		public bool IsUseCustomImage
		{
			get { return _isUseCustomImage; }
			set
			{
				if (_isUseCustomImage == value) return;
				_isUseCustomImage = value;
				OnPropertyChanged(() => IsUseCustomImage);
			}
		}

		public PassCardTemplateReport Report
		{
			get { return _report; }
			set
			{
				_report = value;
				OnPropertyChanged(() => Report);
			}
		}
		public string BackgroundSourceName
		{
			get { return _backgroundSourceName; }
			set
			{
				if (string.Equals(_backgroundSourceName, value)) return;
				_backgroundSourceName = value;
				OnPropertyChanged(() => BackgroundSourceName);
			}
		}

		#endregion

		public TemplateSide()
		{
			SelectPictureCommand = new RelayCommand(OnSelectPicture);
			RemovePictureCommand = new RelayCommand(OnRemovePicture, () => _tmpGuid.HasValue);
			PreviewImage = new PreviewImage();
			WatermarkImage = new WatermarkImage();
		}

		/// <summary>
		/// В конструкторе выполняется установка поля _tmpGuid в null, если для подложки НЕ используется изображение.
		/// В том случае, если для файла подложки используется изображение, то переменная _tmpGuid принимает значение,
		///  соответствующее идентификатору файла оригинального изображения в таблице Attachments
		/// </summary>
		/// <param name="side">Бизнес-объект описывающий сторону пропуска</param>
		public TemplateSide(PassCardTemplateSide side) : this()
		{
			if (side == null) return;

			WatermarkImage = new WatermarkImage(side.WatermarkImage);
			PreviewImage = new PreviewImage(side.PreviewImage) { IsUseImage = !side.WatermarkImage.OriginalImageGuid.IsNullOrEmpty() };

			if (WatermarkImage == null) return;

			Report = side.Report.ToXtraReport(WatermarkImage.Image);
			_tmpGuid = WatermarkImage.OriginalImageGuid;
			UpdateImage();
		}

		public PassCardTemplateSide ToDTO()
		{
			return new PassCardTemplateSide
			{
				PreviewImage = PreviewImage.ToDTO(),
				WatermarkImage = WatermarkImage.ToDTO(),
				Report = Report.ToBytes()
			};
		}

		/// <summary>
		/// Метод сохранения шаблона пропуска.
		/// При сохранении, если указана переменная BackgroundSourceName, то это значит, что пользователь выбрал новое изображение. В этом случае
		/// происходит загрузка изображения из указанного источника и
		/// происходит генерация WatermarkImage на основе этого изображения, и WatermarkImage.OriginalImageGuid устанавливается в обновленное значение _tmpGuid.
		/// Если BackgroundSourceName == null, то это говорит о том, что изображение не менялось.
		/// В этом случае, при сохранении необходимо обращать внимание на переменную _tmpGuid и WatermarkImage.OriginalImageGuid.
		/// Если _tmpGuid равна NULL, а WatermarkImage.OriginalImageGuid НЕ равна NULL, то это говорит о том, что произошло удаление изображения.
		/// В данном случае необходимо произвести обновление оригинального файла изображения (удалить старое изображение с сервера и из папки Content, а в качестве нового установить NULL).
		/// Если переменная _tmpGuid равна NULL и WatermarkImage.OriginalImageGuid тоже, то это означает, что изображение не использовалось, а для подложки использовались простые цвета.
		/// В данном случае необходимо перегенерировать изображение в WatermarkImage.Image, оставив остальные параметры без изменений. При этом,
		/// на уровне файловой системы произойдёт обновление существующего изображения (в папке Content) на новое.
		/// </summary>
		/// <param name="templateWidth">Ширина пропуска в миллиметрах</param>
		/// <param name="templateHeight">Высота пропуска в миллиметрах</param>
		public void Save(double templateWidth, double templateHeight) //TODO: Refactoring
		{
			//Выполняется генерация изображения подложки, с последующим сохранением оригинального изображения.
			if (!string.IsNullOrEmpty(BackgroundSourceName) && _tmpGuid.HasValue)
			{
				Task.Factory.StartNew(() =>
				{
					var originalImageContent = File.ReadAllBytes(BackgroundSourceName);
					UpdateOriginalImage(WatermarkImage.OriginalImageGuid, _tmpGuid.Value, originalImageContent);
				});

				var previewImage = new ImageBuilder(BackgroundSourceName)
					.Height((int)templateHeight)
					.Width((int)templateWidth)
					.BorderColor(PreviewImage.BorderColor)
					.BorderWidth((int)PreviewImage.BorderThickness)
					.Build()
					.ToByteArray();

				WatermarkImage.OriginalImageGuid = _tmpGuid;
				WatermarkImage.Image = previewImage;
			}

			//Если BackgroundSourceName пустой, то значит либо оригинальный файл изображения не используется для генерации подложки, либо используются простые цвета.
			if (string.IsNullOrEmpty(BackgroundSourceName))
			{
				/*Если используется оригинальный файл изображения, то необходимо его найти в папке Content.
				Если файл обнаружен, то нужно сгенерировать новую подложку на основе данного файла*/
				if (WatermarkImage != null && WatermarkImage.OriginalImageGuid.HasValue && WatermarkImage.OriginalImageGuid == _tmpGuid)
				{
					if (ServiceFactoryBase.ContentService.CheckIfExists(WatermarkImage.OriginalImageGuid.Value))
					{
						var pathToImageFile = Path.Combine(ServiceFactoryBase.ContentService.ContentFolder, WatermarkImage.OriginalImageGuid.Value.ToString());
						var image = new ImageBuilder(pathToImageFile)
							.Height((int) templateHeight)
							.Width((int) templateWidth)
							.BorderColor(PreviewImage.BorderColor)
							.BorderWidth((int) PreviewImage.BorderThickness)
							.Build();

						if (image == null)
							return;

						WatermarkImage.Image = image.ToByteArray();
						ServiceFactoryBase.ContentService.AddContent(WatermarkImage.Image, WatermarkImage.Guid);
					}
					else
					{
						MessageBoxService.ShowError(
							"Оригинальный файл изображения не может быть найден. Необходимо добавить файл изображения подложки.");
						return;
					}
				}
				else //Оригинальный файл изображения не используются, а используются простые цвета. В этом случае необходимо просто перегенерировать изображение подложки
				{
					var image = new ImageBuilder()
						.Color(PreviewImage.BackgroundColor)
						.Height((int)templateHeight)
						.Width((int)templateWidth)
						.BorderColor(PreviewImage.BorderColor)
						.BorderWidth((int)PreviewImage.BorderThickness)
						.Build();

					if (WatermarkImage == null)
						return;

					if (WatermarkImage.OriginalImageGuid != null)//Оригинальное изображение не используется, поэтому необходимо удалить этот файл с сервера.
					{
						UpdateOriginalImage(WatermarkImage.OriginalImageGuid, _tmpGuid, null);
						WatermarkImage.OriginalImageGuid = null;
					}

					WatermarkImage.Image = image.ToByteArray();

					ServiceFactoryBase.ContentService.AddContent(WatermarkImage.Image, WatermarkImage.Guid);
				}
			}

			SaveReport(templateWidth, templateHeight);
		}

		private void SaveReport(double width, double height) //TODO: Refactoring
		{
			if (WatermarkImage.Image == null)
				throw new NoNullAllowedException("Image");

			using (var ms = new MemoryStream(WatermarkImage.Image))
			{
				Report = new PassCardTemplateReport(Image.FromStream(ms))
				{
					DisplayName = "Шаблон пропуска",
					WidthF = (float)width,
					HeightF = (float)height,
					PageWidth = (int)width,
					PageHeight = (int)height,
					PaperKind = PaperKind.Custom,
					CanGrow = false,
					CanShrink = false
				};
				Report.Bands.GetBandByType(typeof (DetailBand)).Height = (int) height;
				Report.Bands.GetBandByType(typeof (DetailBand)).Width = (int) width;
				Report.Bands.GetBandByType(typeof (DetailBand)).WidthF = (float) width;
				Report.Bands.GetBandByType(typeof (DetailBand)).HeightF = (float) height;
				Report.Bands.GetBandByType(typeof (DetailBand)).CanShrink = false;
				Report.Bands.GetBandByType(typeof (DetailBand)).CanGrow = false;
			}
		}

		public void UpdateImage()
		{
			if (WatermarkImage == null) return;

			try
			{
				//Используется оригинальное изображение для генерации подложки. Необходимо выполнить попытку достать
				//файл оригинального изображения из папки Content и в случае успешного выполнения операции, перегенерировать изображение подложки на интерфейсе,
				//при этом сам файл подложки перезаписываться не должен (это делает операцию отменяемой).
				if (WatermarkImage.OriginalImageGuid.HasValue)
				{
					if (ServiceFactoryBase.ContentService.CheckIfExists(WatermarkImage.OriginalImageGuid.Value))
					{
						PreviewImage.ImageBrush = ImageHelper.GetResourceBrush(WatermarkImage.OriginalImageGuid.Value, ResourceType.Image);
					}
				}
				else //Оригинальное изображение не используется. В этом случае загружаем файл подложки
				{
					if (WatermarkImage.Image != null && WatermarkImage.Guid != null && !ServiceFactoryBase.ContentService.CheckIfExists(WatermarkImage.Guid.Value))
						ServiceFactoryBase.ContentService.AddContent(WatermarkImage.Image, WatermarkImage.Guid);

					PreviewImage.ImageBrush = new ImageBrush(ImageBuilderHelper.CreateBitmapSource(PreviewImage.BackgroundColor));//ImageHelper.GetResourceBrush(WatermarkImage.Guid, ResourceType.Image);
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове ImagePropertiesViewModel.UpdateImage");
				MessageBoxService.ShowWarning("Возникла ошибка при загрузке изображения");
			}
		}

		/// <summary>
		/// При выборе изображения переменные генерируется новое значение переменной _tmpGuid, которое в будущем может стать идентификатором оригинального изображения.
		/// Изображение загружается в контрол Preview.
		/// Это действие отменяемое (при нажатии на кнопку отмены, выбранное изображение не сохраняется), так как переменная WatermarkImage и  не меняет своих значений.
		/// При последующем сохранении, происходит обновление оригинального изображения на новое загруженное изображение.
		/// </summary>
		private void OnSelectPicture()
		{
			var openFileDialog = new OpenFileDialog { Filter = ImageExtensions.ImageGraphicFilter };

			if (!openFileDialog.ShowDialog().Value) return;

			BackgroundSourceName = openFileDialog.FileName;

			try
			{
				PreviewImage.SetImageFrom(new Uri(BackgroundSourceName));
				_tmpGuid = Guid.NewGuid();
			}
			catch (Exception e)
			{
				MessageBoxService.ShowError(e.Message);
				Logger.Error(e);
			}
		}

		/// <summary>
		/// Действие удаления, которое происходит при нажатии на кнопку очистки изображения.
		/// Для того, что бы действие было отменяемым (при нажатии на кнопку отмены, в контроле превью снова загружалось бы предыдущее изображение),
		/// необходимо работать только с переменной _tmpGuid, так как при нарушении целостности объекта WatermarkImage, произойдёт перезапись изображения.
		/// </summary>
		private void OnRemovePicture()
		{
			_tmpGuid = null;
			BackgroundSourceName = null;
			PreviewImage.ClearImage();
		}


		/// <summary>
		///
		/// </summary>
		/// <param name="idToRemove">Идентификатор файла изобюражения, который необходимо удалить. Может быть пустым, если файл изображения не используется</param>
		/// <param name="newId">Идентификатор нового файла изображения</param>
		/// <param name="newImageContent">Изображение, представленное массивом байтов</param>
		private static void UpdateOriginalImage(Guid? idToRemove, Guid? newId, byte[] newImageContent)
		{
			Attachment attachment = null;

			if (newId.HasValue)
			{
				attachment = new Attachment
				{
					Data = newImageContent,
					FileName = newId.ToString(),
					UID = newId.Value
				};
			}

			Task.Factory.StartNew(() => FiresecManager.FiresecService.UpdatePassCardOriginalImage(idToRemove, attachment));
			if(idToRemove.HasValue)
				ServiceFactoryBase.ContentService.RemoveContent(idToRemove.Value);

			if(newId.HasValue && newImageContent != null)
				ServiceFactoryBase.ContentService.AddContent(newImageContent, newId);
		}

		public RelayCommand SelectPictureCommand { get; private set; }
		public RelayCommand RemovePictureCommand { get; private set; }
	}
}

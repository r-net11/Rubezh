using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using System.IO;
using SharpVectors.Converters;
using SharpVectors.Renderers.Wpf;
using System.Windows.Media;
using Infrastructure.Common;
using Microsoft.Win32;
using Infrastructure.Common.Windows;
using System.Windows.Markup;
using System.Xml;
using System.Windows.Markup.Primitives;
using System.Collections;

namespace DiagnosticsModule.ViewModels
{
	class SVGTestViewModel : DialogViewModel
	{
		private WpfDrawingSettings _settings;
		public SVGTestViewModel()
		{
			Title = "SVG";

			_settings = new WpfDrawingSettings();
			_settings.IncludeRuntime = false;
			_settings.TextAsGeometry = true;
			_settings.OptimizePath = true;

			BrowseCommand = new RelayCommand(OnBrowse);
		}

		public Brush Background { get; set; }
		public string Text { get; set; }

		public RelayCommand BrowseCommand { get; private set; }
		void OnBrowse()
		{
			var openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "SVG изображений|*.svg|Все файлы|*.*";
			if (openFileDialog.ShowDialog().Value)
				try
				{
					TimeSpan converting, total, serialize, deserialize;
					using (new WaitWrapper())
					{
						DateTime now = DateTime.Now;
						DrawingGroup drawing;
						using (FileSvgReader reader = new FileSvgReader(_settings))
							drawing = reader.Read(openFileDialog.FileName);
						drawing.Freeze();
						converting = DateTime.Now - now;
						Background = new DrawingBrush(drawing);
						OnPropertyChanged(() => Background);
						total = DateTime.Now - now;
						now = DateTime.Now;
						Text = XamlWriter.Save(drawing);
						//Text = XmlXamlWriter.Convert(drawing);
						serialize = DateTime.Now - now;
						now = DateTime.Now;
						var clone = XamlReader.Parse(Text);
						deserialize = DateTime.Now - now;
						OnPropertyChanged(() => Text);
					}
					MessageBoxService.Show(string.Format("Конвертация:\t{0}\nВсего:\t{1}\nСериализация:\t{2}\nДесериализация:\t{3}", converting, total, serialize, deserialize));
				}
				catch (Exception ex)
				{
					MessageBoxService.ShowException(ex);
				}
		}
	}
}

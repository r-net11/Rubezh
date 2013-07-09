using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using System.Windows.Controls;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.ComponentModel;

namespace DiagnosticsModule.ViewModels
{
	public class RibbonMenu : TabControl
	{
		public RibbonMenu()
		{
			TabStripPlacement = Dock.Left;

		}
		protected override void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e);
			ItemContainerGenerator.StatusChanged += new EventHandler(ItemContainerGenerator_StatusChanged);
			//var collection = CollectionViewSource.GetDefaultView(Items);
		}

		private void ItemContainerGenerator_StatusChanged(object sender, EventArgs e)
		{
			if (ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
				SelectedIndex = -1;
		}

		protected override DependencyObject GetContainerForItemOverride()
		{
			return new RibbonItem();
		}
		protected override bool IsItemItsOwnContainerOverride(object item)
		{
			return item is RibbonItem;
		}
		protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
		{
			base.PrepareContainerForItemOverride(element, item);
		}

	}
	public class RibbonItem : TabItem
	{
		public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register("ImageSource", typeof(string), typeof(RibbonItem));
		public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(RibbonItem));
		public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(RibbonItem));

		private CoerceValueCallback _callback;
		public RibbonItem()
		{
			DependencyPropertyDescriptor dpd = DependencyPropertyDescriptor.FromProperty(RibbonItem.IsSelectedProperty, typeof(RibbonItem));
			if (dpd != null)
			{
				_callback = dpd.DesignerCoerceValueCallback;
				dpd.DesignerCoerceValueCallback = new CoerceValueCallback(CoerceValueCallback);
				dpd.AddValueChanged(this, (s, e) =>
				{
					var item = s as RibbonItem;
					if (item != null && item.IsSelected && !item.HasContent)
						item.IsSelected = false;
				});
			}
		}
		private object CoerceValueCallback(DependencyObject obj, object val)
		{
			var item = obj as RibbonItem;
			var result = _callback == null ? val : _callback(obj, val);
			if (item == null)
				return result;
			return (bool)result && item.HasContent;
		}

		public string ImageSource
		{
			get { return (string)GetValue(ImageSourceProperty); }
			set { SetValue(ImageSourceProperty, value); }
		}
		public string Text
		{
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}
		public ICommand Command
		{
			get { return (ICommand)GetValue(CommandProperty); }
			set { SetValue(CommandProperty, value); }
		}
	}
	public class RibbonMenuItem : BaseViewModel
	{
		private string _text;
		public string Text
		{
			get { return _text; }
			set
			{
				_text = value;
				OnPropertyChanged(() => Text);
			}
		}

		private string _imageSource;
		public string ImageSource
		{
			get { return _imageSource; }
			set
			{
				_imageSource = value;
				OnPropertyChanged(() => ImageSource);
			}
		}

		private ICommand _command;
		public ICommand Command
		{
			get { return _command; }
			set
			{
				_command = value;
				OnPropertyChanged(() => Command);
			}
		}

		private ObservableCollection<RibbonMenuItem> _children;
		public ObservableCollection<RibbonMenuItem> Children
		{
			get { return _children; }
			set
			{
				_children = value;
				OnPropertyChanged(() => Children);
			}
		}
	}
}

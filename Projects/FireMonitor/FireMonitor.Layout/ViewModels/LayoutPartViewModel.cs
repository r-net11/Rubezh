using System;
using FiresecAPI.Models.Layouts;
using Infrastructure.Common.Services.Layout;
using Infrastructure.Common.Windows.ViewModels;

namespace FireMonitor.Layout.ViewModels
{
	public class LayoutPartViewModel : BaseViewModel
	{
		public ILayoutPartPresenter LayoutPartPresenter { get; private set; }
		public LayoutPart LayoutPart { get; private set; }

		public LayoutPartViewModel(LayoutPart layoutPart, ILayoutPartPresenter layoutPartPresenter)
		{
			LayoutPart = layoutPart;
			LayoutPartPresenter = layoutPartPresenter;
			Content = LayoutPartPresenter.CreateContent(LayoutPart.Properties);
            ImageSource = LayoutPartPresenter.IconSource;
            Title = LayoutPart.Title ?? LayoutPartPresenter.Name;
            if (Content is ILayoutPartContent)
            {
                var layoutPartContent =(ILayoutPartContent)Content;
                layoutPartContent.TitleChanged += (s, e) => Title = layoutPartContent.Title;
                layoutPartContent.ImageChanged += (s, e) => ImageSource = layoutPartContent.ImageSource;
            }
		}

		public Guid UID
		{
			get { return LayoutPart.UID; }
		}
		public BaseViewModel Content { get; private set; }
        
        private string _title;
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                OnPropertyChanged(() => Title);
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
        
	}
}
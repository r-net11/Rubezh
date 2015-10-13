using Infrastructure.Common.TreeList;
using ResursAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Resurs.ViewModels
{
	public class FilterConsumerViewModel : TreeNodeViewModel<FilterConsumerViewModel>
	{
		public Consumer Consumer { get; set; }
		public FilterConsumerViewModel(Consumer consumer)
		{
			Consumer = consumer;
		}

		public string ImageSource
		{
			get
			{
				return Consumer != null && Consumer.IsFolder ? "/Controls;component/Images/CFolder.png" : "/Controls;component/Images/AccessTemplate.png";
			}
		}
		
		bool _isCheked;
		public  bool IsChecked
		{
			get { return _isCheked;}
			set 
			{
				_isCheked = value;
				
			}
		}
	}
}
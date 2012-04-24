using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using Infrastructure.Common;
using FiresecAPI.Models.Skud;
using FiresecClient;
using SkudModule.Properties;

namespace SkudModule.ViewModels
{
	public class EmployeeDictionaryItemViewModel<T> : BaseViewModel
	{
		public T Item { get; private set; }

		public EmployeeDictionaryItemViewModel(T item)
		{
			Item = item;
		}

		public void Update()
		{
			OnPropertyChanged("Item");
		}
	}
}
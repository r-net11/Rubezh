using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class TotalViewModel : BaseViewModel
	{
		public TotalViewModel(string name, TimeSpan timeSpan)
		{
			Name = name;
			TimeSpan = timeSpan;
		}

		public string Name { get; private set; }
		public TimeSpan TimeSpan { get; private set; }
	}
}
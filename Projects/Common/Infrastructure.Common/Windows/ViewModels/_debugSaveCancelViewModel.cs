using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infrastructure.Common.Windows.ViewModels
{
	public class _debugSaveCancelViewModel : ShellViewModel
	{
		public _debugSaveCancelViewModel()
		{
			ContentFotter = new _debugReplacer("footter...");
			ContentHeader = new _debugReplacer("header...");
			MainContent = new _debugReplacer("MAIN CONTENT");
			Toolbar = new _debugReplacer("main toolbar");
		}
	}

	public class _debugReplacer : BaseViewModel
	{
		public string Value { get; private set; }
		public _debugReplacer(string value)
		{
			Value = value;
		}
		public override string ToString()
		{
			return Value;
		}
	}
}

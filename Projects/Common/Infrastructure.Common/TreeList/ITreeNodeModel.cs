using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.ComponentModel;

namespace Infrastructure.Common.TreeList
{
	public interface ITreeNodeModel : INotifyPropertyChanged
	{
		bool IsSelected { get; set; }
		bool IsExpanded { get; set; }
		IEnumerable GetChildren();
		bool HasChildren { get; }
	}
}

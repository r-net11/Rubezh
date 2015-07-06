﻿using System.Collections;
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
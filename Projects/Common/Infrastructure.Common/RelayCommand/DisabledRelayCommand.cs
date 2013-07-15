﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infrastructure.Common
{
	public static class DisabledRelayCommand
	{
		public static RelayCommand Instance { get; private set; }
		static DisabledRelayCommand()
		{
			Instance = new RelayCommand(() => { }, () => false);
		}
	}
}

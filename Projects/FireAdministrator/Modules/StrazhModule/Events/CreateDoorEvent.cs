﻿using System;
using FiresecAPI.SKD;
using Microsoft.Practices.Prism.Events;

namespace StrazhModule.Events
{
	public class CreateDoorEvent : CompositePresentationEvent<CreateDoorEventArg>
	{
	}

	public class CreateDoorEventArg
	{
		public SKDDoor Door { get; set; }
	}
}
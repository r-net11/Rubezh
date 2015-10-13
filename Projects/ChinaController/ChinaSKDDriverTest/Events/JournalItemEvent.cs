using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ChinaSKDDriverAPI;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Events;

namespace ControllerSDK.Events
{
	public class JournalItemEvent : CompositePresentationEvent<SKDJournalItem>
	{
	}
}

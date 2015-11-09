using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace FiresecClient.ClientLogic
{
	public class DocumentsFactory
	{
		private Hashtable _documentsMapping = new Hashtable();

		private static DocumentsFactory _documentsTypes;

		private DocumentsFactory()
		{
			Initialize();
		}
	}
}

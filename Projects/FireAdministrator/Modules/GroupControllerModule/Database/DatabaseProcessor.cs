using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecClient;
using XFiresecAPI;

namespace GKModule.Database
{
	public static class DatabaseProcessor
	{
		public static DatabaseCollection DatabaseCollection;

		public static void Convert()
		{
			DatabaseCollection = new DatabaseCollection();
			DatabaseCollection.Build();
		}
	}
}
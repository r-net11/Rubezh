using System.Collections.Generic;
using System.Linq;
using FiresecAPI;
using System;
using XFiresecAPI;

namespace SKDDriver
{
	public static class SKDDatabaseService
	{
		static DataAccess.SKUDDataContext Context;

		static SKDDatabaseService()
		{
			Context = new DataAccess.SKUDDataContext();
			DocumentTranslator = new DocumentTranslator(Context.Document, Context);
			PositionTranslator = new PositionTranslator(Context.Position, Context);
			CardZoneTranslator = new CardZoneTranslator(Context.CardZoneLink, Context);
			CardTranslator = new CardTranslator(Context.Card, Context, CardZoneTranslator);
			GUDTranslator = new GUDTranslator(Context.GUD, Context, CardZoneTranslator);
			OrganizationTranslator = new OrganizationTranslator(Context.Organization, Context);
			JournalItemTranslator = new JournalItemTranslator(Context.Journal, Context);
			EmployeeTranslator = new EmployeeTranslator(Context.Employee, Context);
			DepartmentTranslator = new DepartmentTranslator(Context.Department, Context);
			AdditionalColumnTypeTranslator = new AdditionalColumnTypeTranslator(Context.AdditionalColumnType, Context);
			AdditionalColumnTranslator = new AdditionalColumnTranslator(Context.AdditionalColumn, Context);
			PhotoTranslator = new PhotoTranslator(Context.Photo, Context);

			var e = EmployeeTranslator.Get(null);
		}

		public static DocumentTranslator DocumentTranslator { get; private set; }
		public static PositionTranslator PositionTranslator { get; private set; }
		public static CardTranslator CardTranslator { get; private set; }
		public static CardZoneTranslator CardZoneTranslator { get; private set; }
		public static GUDTranslator GUDTranslator { get; private set; }
		public static OrganizationTranslator OrganizationTranslator { get; private set; }
		public static JournalItemTranslator JournalItemTranslator { get; private set; }
		public static EmployeeTranslator EmployeeTranslator { get; private set; }
		public static DepartmentTranslator DepartmentTranslator { get; private set; }
		public static AdditionalColumnTypeTranslator AdditionalColumnTypeTranslator { get; private set; }
		public static AdditionalColumnTranslator AdditionalColumnTranslator { get; private set; }
		public static PhotoTranslator PhotoTranslator { get; private set; } 
	}
}
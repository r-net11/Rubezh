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
			DocumentTranslator = new DocumentTranslator(Context);
			PositionTranslator = new PositionTranslator(Context);
			CardZoneTranslator = new CardZoneTranslator(Context);
			CardTranslator = new CardTranslator(Context, CardZoneTranslator);
			GUDTranslator = new GUDTranslator(Context, CardZoneTranslator);
			OrganizationTranslator = new OrganizationTranslator(Context);
			JournalItemTranslator = new JournalItemTranslator(Context);
			DepartmentTranslator = new DepartmentTranslator(Context);
			AdditionalColumnTypeTranslator = new AdditionalColumnTypeTranslator(Context);
			AdditionalColumnTranslator = new AdditionalColumnTranslator(Context);
			EmployeeReplacementTranslator = new EmployeeReplacementTranslator(Context);
			EmployeeTranslator = new EmployeeTranslator(Context, EmployeeReplacementTranslator);
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
		public static EmployeeReplacementTranslator EmployeeReplacementTranslator { get; private set; } 
	}
}
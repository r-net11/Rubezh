using SKDDriver.Translators;

namespace SKDDriver
{
	public static class SKDDatabaseService
	{
		static DataAccess.SKDDataContext Context;

		static SKDDatabaseService()
		{
			Context = new DataAccess.SKDDataContext();
			DocumentTranslator = new DocumentTranslator(Context);
			CardZoneTranslator = new CardZoneTranslator(Context);
			CardTranslator = new CardTranslator(Context, CardZoneTranslator);
			AccessTemplateTranslator = new AccessTemplateTranslator(Context, CardZoneTranslator);
			OrganizationTranslator = new OrganizationTranslator(Context);
			JournalItemTranslator = new JournalItemTranslator(Context);
			PhotoTranslator = new PhotoTranslator(Context);
			PositionTranslator = new PositionTranslator(Context, PhotoTranslator);
			DepartmentTranslator = new DepartmentTranslator(Context, PhotoTranslator);
			AdditionalColumnTypeTranslator = new AdditionalColumnTypeTranslator(Context);
			AdditionalColumnTranslator = new AdditionalColumnTranslator(Context, PhotoTranslator, AdditionalColumnTypeTranslator);
			EmployeeReplacementTranslator = new EmployeeReplacementTranslator(Context);
			EmployeeTranslator = new EmployeeTranslator(Context, EmployeeReplacementTranslator, PositionTranslator, DepartmentTranslator, AdditionalColumnTranslator, CardTranslator, PhotoTranslator);
			NamedIntervalTranslator = new NamedIntervalTranslator(Context);
		}

		public static DocumentTranslator DocumentTranslator { get; private set; }
		public static PositionTranslator PositionTranslator { get; private set; }
		public static CardTranslator CardTranslator { get; private set; }
		public static CardZoneTranslator CardZoneTranslator { get; private set; }
		public static AccessTemplateTranslator AccessTemplateTranslator { get; private set; }
		public static OrganizationTranslator OrganizationTranslator { get; private set; }
		public static JournalItemTranslator JournalItemTranslator { get; private set; }
		public static EmployeeTranslator EmployeeTranslator { get; private set; }
		public static DepartmentTranslator DepartmentTranslator { get; private set; }
		public static AdditionalColumnTypeTranslator AdditionalColumnTypeTranslator { get; private set; }
		public static AdditionalColumnTranslator AdditionalColumnTranslator { get; private set; }
		public static PhotoTranslator PhotoTranslator { get; private set; }
		public static EmployeeReplacementTranslator EmployeeReplacementTranslator { get; private set; }
		public static NamedIntervalTranslator NamedIntervalTranslator { get; private set; }
	}
}
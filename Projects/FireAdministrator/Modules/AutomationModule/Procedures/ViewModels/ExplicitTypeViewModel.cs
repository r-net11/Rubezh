using Infrastructure.Common.TreeList;
using FiresecAPI.Automation;
using FiresecAPI;

namespace AutomationModule.ViewModels
{
	public class ExplicitTypeViewModel : TreeNodeViewModel<ExplicitTypeViewModel>
	{
		public ExplicitType ExplicitType { get; private set; }
		public EnumType EnumType { get; private set; }
		public ObjectType ObjectType { get; private set; }

		public ExplicitTypeViewModel(ExplicitType explicitType)
		{
			ExplicitType = explicitType;
		}

		public ExplicitTypeViewModel(EnumType enumType)
		{
			ExplicitType = ExplicitType.Enum;
			EnumType = enumType;
		}

		public ExplicitTypeViewModel(ObjectType objectType)
		{
			ExplicitType = ExplicitType.Object;
			ObjectType = objectType;
		}

		public string Name
		{
			get
			{
				if (Parent != null)
				{
					if (Parent.ExplicitType == ExplicitType.Enum)
						return EnumType.ToDescription();
					if (Parent.ExplicitType == ExplicitType.Object)
						return ObjectType.ToDescription();
				}
				return ExplicitType.ToDescription();
			}
		}

		public bool IsRealType
		{
			get
			{
				if (ExplicitType == ExplicitType.Enum || ExplicitType == ExplicitType.Object)
					if (Parent == null)
						return false;
				return true;
			}
		}
	}
}

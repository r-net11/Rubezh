using RubezhAPI.Automation;
using RubezhAPI.StepArguments;

namespace AutomationModule.ViewModels
{
	class RviOpenWindowStepViewModel : BaseStepViewModel
	{
		public RviOpenWindowArguments RviOpenWindowArguments { get; private set; }
		public ArgumentViewModel NameArgument { get; set; }
		public ArgumentViewModel XArgument { get; set; }
		public ArgumentViewModel YArgument { get; set; }
		public ArgumentViewModel MonitorNumberArgument { get; set; }
		public ArgumentViewModel LoginArgument { get; set; }
		public ArgumentViewModel IpArgument { get; set; }
		public RviOpenWindowStepViewModel(StepViewModel stepViewModel)
			: base(stepViewModel)
		{
			RviOpenWindowArguments = stepViewModel.Step.RviOpenWindowArguments;
			NameArgument = new ArgumentViewModel(RviOpenWindowArguments.NameArgument, stepViewModel.Update, null);
			XArgument = new ArgumentViewModel(RviOpenWindowArguments.XArgument, stepViewModel.Update, null);
			XArgument.ExplicitValue.MinIntValue = 0;
			XArgument.ExplicitValue.MaxIntValue = 1000;
			YArgument = new ArgumentViewModel(RviOpenWindowArguments.YArgument, stepViewModel.Update, null);
			YArgument.ExplicitValue.MinIntValue = 0;
			YArgument.ExplicitValue.MaxIntValue = 1000;
			MonitorNumberArgument = new ArgumentViewModel(RviOpenWindowArguments.MonitorNumberArgument, stepViewModel.Update, null);
			MonitorNumberArgument.ExplicitValue.MinIntValue = 1;
			MonitorNumberArgument.ExplicitValue.MaxIntValue = 4;
			LoginArgument = new ArgumentViewModel(RviOpenWindowArguments.LoginArgument, stepViewModel.Update, null);
			IpArgument = new ArgumentViewModel(RviOpenWindowArguments.IpArgument, stepViewModel.Update, null);
		}
		public override void UpdateContent()
		{
			NameArgument.Update(Procedure, ExplicitType.String, isList: false);
			XArgument.Update(Procedure, ExplicitType.Integer, isList: false);
			YArgument.Update(Procedure, ExplicitType.Integer, isList: false);
			MonitorNumberArgument.Update(Procedure, ExplicitType.Integer, isList: false);
			LoginArgument.Update(Procedure, ExplicitType.String, isList: false);
			IpArgument.Update(Procedure, ExplicitType.String, isList: false);
		}

		public override string Description
		{
			get
			{
				return string.Format("Название раскладки в Rvi: \"{0}\"; Сдвиг по осям: X = {1}, Y = {2}; Номер монитора: {3}; Ip-адрес рабочего места: {4}; Логин пользователя: {5}",
					new[] { NameArgument.Description, XArgument.Description, YArgument.Description, MonitorNumberArgument.Description, IpArgument.Description, LoginArgument.Description });
			}
		}
	}
}
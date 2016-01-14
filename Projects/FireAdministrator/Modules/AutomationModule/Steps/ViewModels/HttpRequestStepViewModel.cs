using Infrastructure.Automation;
using RubezhAPI;
using RubezhAPI.Automation;
using System.Collections.ObjectModel;

namespace AutomationModule.ViewModels
{
	public class HttpRequestStepViewModel : BaseStepViewModel
	{
		public HttpMethod HttpMethod { get; private set; }
		public ArgumentViewModel UrlArgument { get; private set; }
		public ArgumentViewModel ContentArgument { get; private set; }
		public ArgumentViewModel ResponseArgument { get; private set; }
		HttpRequestArguments HttpRequestArguments { get; set; }

		public HttpRequestStepViewModel(StepViewModel stepViewModel)
			: base(stepViewModel)
		{
			HttpRequestArguments = stepViewModel.Step.HttpRequestArguments;
			UrlArgument = new ArgumentViewModel(HttpRequestArguments.UrlArgument, stepViewModel.Update, UpdateContent);
			ContentArgument = new ArgumentViewModel(HttpRequestArguments.ContentArgument, stepViewModel.Update, UpdateContent);
			ResponseArgument = new ArgumentViewModel(HttpRequestArguments.ResponseArgument, stepViewModel.Update, UpdateContent, false);
			HttpMethods = AutomationHelper.GetEnumObs<HttpMethod>();
		}

		public override void UpdateContent()
		{
			UrlArgument.Update(Procedure, ExplicitType.String);
			ContentArgument.Update(Procedure, ExplicitType.String);
			ResponseArgument.Update(Procedure, ExplicitType.String);
		}

		public ObservableCollection<HttpMethod> HttpMethods { get; private set; }
		HttpMethod _selectedHttpMethod;
		public HttpMethod SelectedHttpMethod
		{
			get { return _selectedHttpMethod; }
			set
			{
				_selectedHttpMethod = value;
				HttpRequestArguments.HttpMethod = _selectedHttpMethod;
				OnPropertyChanged(() => SelectedHttpMethod);
			}
		}

		public override string Description
		{
			get
			{
				return "Адрес: " + UrlArgument.Description
					+ "; Метод: " + SelectedHttpMethod.ToDescription()
					+ (SelectedHttpMethod == HttpMethod.Post ? "; Данные: " + ContentArgument.Description : "")
					+ "; Ответ: " + ResponseArgument.Description;
			}
		}
	}
}
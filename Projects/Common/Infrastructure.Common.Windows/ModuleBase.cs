using System.Collections.Generic;
using System.ComponentModel;
using RubezhAPI;
using Infrastructure.Common.Navigation;
using Infrastructure.Common.Services;

namespace Infrastructure.Common
{
	public abstract class ModuleBase : IModule
	{
		public ModuleBase()
		{
			RegisterResource();
		}

		#region IModule Members

		public string Name
		{
			get { return ModuleType.ToDescription(); }
		}

		public abstract ModuleType ModuleType { get; }

		public virtual int Order
		{
			get { return 10; }
		}

		public virtual void RegisterResource()
		{
			ServiceFactoryBase.ResourceService.AddResource(GetType().Assembly, "DataTemplates/Dictionary.xaml");
		}

		public virtual bool BeforeInitialize(bool firstTime)
		{
			return true;
		}

		public virtual void AfterInitialize() { }

		public abstract void CreateViewModels();
		public abstract void Initialize();
		public virtual void RegisterPlanExtension() { }
		public abstract IEnumerable<NavigationItem> CreateNavigation();

		#endregion

		#region IDisposable Members

		public virtual void Dispose()
		{
		}

		#endregion
	}

	public enum ModuleType
	{
		[DescriptionAttribute("Администратор")]
		Administrator,
		[DescriptionAttribute("Автоматизация")]
		Automation,
		[DescriptionAttribute("Устройства, Зоны, Направления")]
		Devices,
		[DescriptionAttribute("Диагностика")]
		Diagnostics,
		[DescriptionAttribute("Фильтры журнала событий")]
		Filters,
		[DescriptionAttribute("Групповой контроллер")]
		GK,
		[DescriptionAttribute("Конфигуратор макетов ОЗ")]
		Layout,
		[DescriptionAttribute("OPC сервер")]
		OPC,
		[DescriptionAttribute("Графические планы")]
		Plans,
		[DescriptionAttribute("Графические планы - Курск")]
		PlansKursk,
		[DescriptionAttribute("Права доступа")]
		Security,
		[DescriptionAttribute("Настройки")]
		Settings,
		[DescriptionAttribute("СКД")]
		SKD,
		[DescriptionAttribute("Звуки")]
		Sounds,
		[DescriptionAttribute("Видео")]
		Video,
		[DescriptionAttribute("Cостояния")]
		Alarm,
		[DescriptionAttribute("Журнал событий и Архив")]
		Journal,
		[DescriptionAttribute("Отчёты")]
		Reports,
		[DescriptionAttribute("Монитор")]
		Monitor,
	}
}
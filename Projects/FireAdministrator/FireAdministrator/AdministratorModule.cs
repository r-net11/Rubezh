using System.Collections.Generic;
using Infrastructure.Client.Layout;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Common.Services.Layout;

namespace FireAdministrator
{
	public class AdministratorModule : ModuleBase, ILayoutDeclarationModule
	{
		public override void CreateViewModels()
		{
		}
		public override void Initialize()
		{
		}
		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			return new List<NavigationItem>();
		}
		public override string Name
		{
			get { return "Администратор"; }
		}

		public override bool BeforeInitialize(bool firstTime)
		{
			return true;
		}

		#region ILayoutDeclarationModule Members

		public IEnumerable<ILayoutPartDescription> GetLayoutPartDescriptions()
		{
			yield return new LayoutPartDescription()
			{
				Name = "Заглушка",
				Description = "Пустая панель",
				Index = 0,
				UID = LayoutPartIdentities.EmptySpace,
				IconSource = "/Controls;component/Images/BExit.png",
				AllowMultiple = true,
			};
			yield return new LayoutPartDescription()
			{
				Name = "Индикаторы",
				Description = "Панель индикаторов состояния",
				Index = 1,
				UID = LayoutPartIdentities.Indicator,
				IconSource = "/Controls;component/Images/BAlarm.png",
				AllowMultiple = false,
				//Content = new LayoutPartImageViewModel() { ImageSource = "/Controls;component/Images/Layout/IndicatorsPreview.png" },
			};
			yield return new LayoutPartDescription()
			{
				Name = "Навигатор",
				Description = "Панель навигации",
				Index = 2,
				UID = LayoutPartIdentities.Navigation,
				IconSource = "/Controls;component/Images/BTree.png",
				AllowMultiple = false,
				//Content = new LayoutPartImageViewModel() { ImageSource = "/Controls;component/Images/Layout/NavigationPreview.png" },
			};
			yield return new LayoutPartDescription()
			{
				Name = "Контейнер",
				Description = "Контейнер содержания",
				Index = 3,
				UID = LayoutPartIdentities.Content,
				IconSource = "/Controls;component/Images/BLayouts.png",
				AllowMultiple = false,
				//Content = new LayoutPartImageViewModel() { ImageSource = "/Controls;component/Images/Layout/ContentPreview.png" },
			};
			yield return new LayoutPartDescription()
			{
				Name = "Отчеты",
				Description = "Отчеты",
				Index = 201,
				UID = LayoutPartIdentities.Reports,
				IconSource = "/Controls;component/Images/BLevels.png",
				AllowMultiple = true,
				//Content = new LayoutPartImageViewModel() { ImageSource = "/Controls;component/Images/BLevels.png" },
			};
			yield return new LayoutPartDescription()
			{
				Name = "Диагностика",
				Description = "Диагностика",
				Index = 202,
				UID = LayoutPartIdentities.Diagnostics,
				IconSource = "/Controls;component/Images/BBug.png",
				AllowMultiple = true,
				//Content = new LayoutPartImageViewModel() { ImageSource = "/Controls;component/Images/BBug.png" },
			};
			yield return new LayoutPartDescription()
			{
				Name = "Видео",
				Description = "Видео",
				Index = 203,
				UID = LayoutPartIdentities.Video,
				IconSource = "/Controls;component/Images/BVideo.png",
				AllowMultiple = true,
				//Content = new LayoutPartImageViewModel() { ImageSource = "/Controls;component/Images/BVideo.png" },
			};
		}

		#endregion
	}
}
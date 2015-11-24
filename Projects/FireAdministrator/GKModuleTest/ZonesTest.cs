using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GKModule.ViewModels;
using Infrastructure;
using GKModule.Plans;
using Infrastructure.Common.Windows;

namespace GKModuleTest
{
	[TestClass]
	public class ZonesTest
	{
		[TestMethod]
		public void Add()
		{
			var fakeDialogService = new FakeDialogService();
			ServiceFactory.Initialize(null, null);
			ServiceFactory.DialogService = fakeDialogService;
			var gkPlanExtension = new GKPlanExtension(null, null, null, null, null, null, null, null);
			var zonesViewModel = new ZonesViewModel();
			zonesViewModel.Initialize();
			fakeDialogService.OnShow += x => { (x as ZoneDetailsViewModel).Name = "xxx"; };
			zonesViewModel.AddCommand.Execute();
			Assert.IsTrue(zonesViewModel.Zones.FirstOrDefault().Zone.Name == "xxx");

			zonesViewModel.DeleteCommand.Execute();
			Assert.IsTrue(zonesViewModel.Zones.Count ==0);
		}
	}

	public class XDialogService
	{
		public void ShowModalWindow()
		{
		}
	}
}
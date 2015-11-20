using GKModule.ViewModels;
using Infrastructure;
using Infrastructure.Common.Windows;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GKModuleTest
{
	[TestClass]
	public class ZonesTest
	{
		[TestMethod]
		public void CreateZone()
		{
			ServiceFactory.Initialize(null, null);
			var zonesViewModel = new ZonesViewModel();
			zonesViewModel.Initialize();
			zonesViewModel.AddCommand.Execute();
		}

		[TestMethod]
		public void MocksTest()
		{
			MockRepository mocks = new MockRepository();
			var projectView = mocks.StrictMock<NothingDoing>();
			Expect.Call(projectView.SaySomeThing()).Return("mocksad");
			Expect.Call(projectView.SaySomeThingAnother()).Return("mocksadanother");
			Expect.Call(projectView.GetValue()).Return("WWW");
			Expect.Call(projectView.Add(0, 0)).Return(10);
			mocks.ReplayAll();
			var q = projectView.SaySomeThingAnother();
			var c = projectView.SaySomeThing();
			var e = projectView.GetValue();
			var x = projectView.Add(0, 1);
			mocks.VerifyAll();
			Assert.IsTrue(c == "mocksad");
			Assert.IsTrue(q == "mocksadanother");
		}
	}

	public class NothingDoing //: IProjectView
	{
		public virtual string answers { get; set; }
		public virtual string questions { get; set; }
		public virtual string SaySomeThing()
		{
			Thread.Sleep(30000);
			return "Th";
		}
		public virtual string SaySomeThingAnother()
		{

			return "An";
		}

		public virtual int Add(int x, int y)
		{
			return x + y;
		}

		public virtual string GetValue()
		{
			return "W";
		}
	}
	public interface IProjectView
	{
		string SaySomeThing();
		string SaySomeThingAnother();
		string answers { get; set; }
		string questions { get; set; }
	}

	public class TestSystem
	{
		public int Add()
		{
			var calculator = new Calculator();
			var result = calculator.Add(5, 6);
			return result;
		}
	}

	public class Calculator
	{
		public int Add(int x, int y)
		{
			return x + y;
		}
	}

	public interface IDialogWindow
	{
		string Title { get; set; }
	}

	public interface ITestDialogService
	{
		bool ShowModalWindow(IDialogWindow dialogWindow);

		bool GetResult();
	}

	public class TestDialogService : ITestDialogService
	{
		public bool ShowModalWindow(IDialogWindow dialogWindow)
		{
			return false;
		}

		public bool GetResult()
		{
			return false;
		}
	}
}
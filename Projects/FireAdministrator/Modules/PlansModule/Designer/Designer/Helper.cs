﻿using System;
using System.Linq;
using System.Windows.Media;
using RubezhAPI.Models;
using RubezhClient;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Painters;
using System.Collections.Generic;
using System.Threading;
using Infrastructure.Client.Plans;
using Infrastructure;
using Infrastructure.Events;

namespace PlansModule.Designer
{
	public static class Helper
	{
		public const string SubPlanAlias = "SubPlan";
		public static Thread TwoThread {get; private set;}
		public static bool Flag {get;set;}
		public static List<Plan> Plans {get; private set;}
		public static Plan Plan { get; set;}
		public static EventWaitHandle WiteHandle { get; private set; }

		static Helper()
		{
			ServiceFactory.Events.GetEvent<ConfigurationClosedEvent>().Subscribe(CloseThread);
		}

		public static void ThreadMetod()
		{
			TwoThread = new Thread(() => { CreatPlans(); }) { IsBackground = true };
			Flag = true;
			Plans = new List<Plan>();
			WiteHandle = new AutoResetEvent(true);
			TwoThread.Start();
		}

		private static void CloseThread(object obj)
		{
			TwoThread.Abort();
			WiteHandle.Close();
		}

		static void CreatPlans()
		{
			while (Flag)
			{
				foreach (var plan in ClientManager.PlansConfiguration.AllPlans.Where(x => !Plans.Contains(x)))
				{
					if (plan.BackgroundImageSource.HasValue && !ServiceFactory.ContentService.CheckIfExists(plan.BackgroundImageSource.Value.ToString()))
						plan.BackgroundImageSource = null;

					if (!Flag)
						break;
					PainterCache.CacheBrush(plan);

					foreach (var elementBase in PlanEnumerator.Enumerate(plan))
					{
						if (!Flag)
							break;
						PainterCache.CacheBrush(elementBase);
					}

					if (!Flag)
						break;
					Plans.Add(plan);
				}

				break;
			}

			if (!Flag && !Plans.Contains(Plan))
			{
				Plans.Add(Plan);
				WiteHandle.WaitOne();
				Flag = true;
				CreatPlans();
			}
			else if (ClientManager.PlansConfiguration.AllPlans.Count != Plans.Count)
			{
				Flag = true;
				CreatPlans();
			}
			else
			{
				WiteHandle.Close();
				Flag = false;
				return;
			}
		}

		public static Plan GetPlan(IElementSubPlan element)
		{
			return ClientManager.PlansConfiguration.AllPlans.FirstOrDefault(x => x.UID == element.PlanUID);
		}
		public static string GetSubPlanTitle(ElementRectangleSubPlan element)
		{
			Plan plan = GetPlan(element);
			if (plan == null && element.PlanUID != Guid.Empty)
				SetSubPlan(element, null);
			return plan == null ? "Несвязанная ссылка на план" : plan.Caption;
		}
		public static void SetSubPlan(IElementSubPlan element)
		{
			Plan plan = GetPlan(element);
			SetSubPlan(element, plan);
		}
		public static void SetSubPlan(IElementSubPlan element, Plan plan)
		{
			element.PlanUID = plan == null ? Guid.Empty : plan.UID;
			element.Caption = plan == null ? string.Empty : plan.Caption;
			element.BackgroundColor = GetSubPlanColor(plan);
		}
		public static Color GetSubPlanColor(Plan plan)
		{
			Color color = Colors.Black;
			if (plan != null)
				color = Colors.Green;
			return color;
		}
	}
}
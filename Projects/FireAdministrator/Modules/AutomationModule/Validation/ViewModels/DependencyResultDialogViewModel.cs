using AutomationModule.Validation;
using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutomationModule.ViewModels
{
	public class DependencyResultDialogViewModel: SaveCancelDialogViewModel
	{
		#region Constructors
		public DependencyResultDialogViewModel(ResearchDependencyResult dependency)
		{
			Title = "Удаление объекта";
			Dependency = dependency;
		}
		#endregion

		#region Fields And Properties
		public ResearchDependencyResult Dependency { get; private set; }

		public string Plans
		{
			get
			{
				if ((Dependency.Plans == null) || (Dependency.Plans.Length == 0))
				{
					return "Отсутствуют";
				}
				else
				{
					StringBuilder sb = new StringBuilder();
					foreach (var plan in Dependency.Plans)
					{
						sb.Append(plan.Caption);
						sb.Append("; ");
					}
					return sb.ToString();
				}
			}
		}
		public string Procedures
		{
			get
			{
				if ((Dependency.Procedures == null) || (Dependency.Procedures.Length == 0))
				{
					return "Отсутствуют";
				}
				else
				{
					StringBuilder sb = new StringBuilder();
					foreach (var item in Dependency.Procedures)
					{
						sb.Append(item.Name);
						sb.Append("; ");
					}
					return sb.ToString();
				}
			}
		}
		public string Variables
		{
			get 
			{
				if ((Dependency.Variables == null) || (Dependency.Variables.Length == 0))
				{
					return "Отсутствуют";
				}
				else
				{
					StringBuilder sb = new StringBuilder();
					foreach (var item in Dependency.Variables)
					{
						sb.Append(item.Name);
						sb.Append("; ");
					}
					return sb.ToString();
				}
			}
		}
		public string OpcDaTags 
		{
			get
			{
				if ((Dependency.OpcDaTags == null) || (Dependency.OpcDaTags.Length == 0))
				{
					return "Отсутствуют";
				}
				else
				{
					StringBuilder sb = new StringBuilder();
					foreach (var item in Dependency.OpcDaTags)
					{
						sb.Append(item.ElementName);
						sb.Append("; ");
					}
					return sb.ToString();
				}
			}
		}
		public string OpcDaServers 
		{
			get
			{
				if ((Dependency.OpcDaServers == null) || (Dependency.OpcDaServers.Length == 0))
				{
					return "Отсутствуют";
				}
				else
				{
					StringBuilder sb = new StringBuilder();
					foreach (var item in Dependency.OpcDaServers)
					{
						sb.Append(item.ServerName);
						sb.Append("; ");
					}
					return sb.ToString();
				}
			}
		}
		public string OpcDaTagFilters 
		{
			get
			{
				if ((Dependency.OpcDaTagFilters == null) || (Dependency.OpcDaTagFilters.Length == 0))
				{
					return "Отсутствуют";
				}
				else
				{
					StringBuilder sb = new StringBuilder();
					foreach (var item in Dependency.OpcDaTagFilters)
					{
						sb.Append(item.Name);
						sb.Append("; ");
					}
					return sb.ToString();
				}
			}
		}
		public string Filters 
		{
			get
			{
				if ((Dependency.Filters == null) || (Dependency.Filters.Length == 0))
				{
					return "Отсутствуют";
				}
				else
				{
					StringBuilder sb = new StringBuilder();
					foreach (var item in Dependency.Filters)
					{
						sb.Append(item.Name);
						sb.Append("; ");
					}
					return sb.ToString();
				}
			}
		}

		#endregion
	}
}

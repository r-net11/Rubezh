﻿using System.Threading.Tasks;
using Common;
using StrazhAPI.Automation;
using StrazhAPI.Journal;
using StrazhAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using StrazhAPI.Models.Automation;
using FiresecService.Service;

namespace FiresecService
{
	public partial class ProcedureThread
	{
		public Guid UID { get; private set; }

		public Guid? ClientUID { get; private set; }

		public DateTime StartTime { get; private set; }

		public bool IsAlive { get; set; }

		public User User { get; private set; }

		public int TimeOut { get; private set; }

		public TimeType TimeType { get; private set; }

		private AutoResetEvent AutoResetEvent { get; set; }

		private List<IVariable> AllVariables { get; set; }

		private List<ProcedureStep> Steps { get; set; }

		private ProcedureThread ChildProcedureThread { get; set; }

		private JournalItem JournalItem { get; set; }

		public ProcedureThread(Procedure procedure, List<Argument> arguments, List<IVariable> callingProcedureVariables, JournalItem journalItem = null, User user = null, Guid? clientUID = null)
		{
			UID = Guid.NewGuid();
			ClientUID = clientUID;
			User = user;
			IsAlive = true;
			JournalItem = journalItem;
			TimeOut = procedure.TimeOut;
			TimeType = procedure.TimeType;
			AutoResetEvent = new AutoResetEvent(false);
			Steps = procedure.Steps;
			AllVariables = new List<IVariable>(Utils.Clone(procedure.Variables));
			var procedureArguments = Utils.Clone(procedure.Arguments);
			InitializeArguments(procedureArguments, arguments, callingProcedureVariables);

			AllVariables.AddRange(procedureArguments);
			AllVariables.AddRange(FiresecServiceManager.SafeFiresecService.GetCurrentGlobalVariables().Result);

			Task.Factory.StartNew(() => RunInThread(arguments))
				.ContinueWith(t =>
				{
					//FiresecServiceManager.SafeFiresecService.SaveGlobalVariables(AllVariables);
					FiresecServiceManager.SafeFiresecService.SaveEditedGlobalVariables(AllVariables.OfType<GlobalVariable>());
				}
				);
		}

		public void Start()
		{
			StartTime = DateTime.Now;
			//Thread.Start();
		}

		private bool _isTimeOut;

		public bool IsTimeOut
		{
			get { return _isTimeOut; }
			set
			{
				_isTimeOut = value;
				if (ChildProcedureThread != null)
					ChildProcedureThread.IsTimeOut = value;
				AutoResetEvent.Set();
			}
		}

		public void RunInThread(List<Argument> arguments)
		{
			try
			{
				if (Steps.Any(step => RunStep(step) == Result.Exit))
				{
				}
			}
			catch (Exception e)
			{
				Logger.Error(e);
			}
			finally
			{
				IsAlive = false;
			}
		}

		private Result RunStep(ProcedureStep procedureStep)
		{
			if (IsTimeOut)
				return Result.Normal;
			switch (procedureStep.ProcedureStepType)
			{
				case ProcedureStepType.If:
					if (Compare(procedureStep))
					{
						foreach (var childStep in procedureStep.Children[0].Children)
						{
							var result = RunStep(childStep);
							if (result != Result.Normal)
							{
								return result;
							}
						}
					}
					else
					{
						foreach (var childStep in procedureStep.Children[1].Children)
						{
							var result = RunStep(childStep);
							if (result != Result.Normal)
							{
								return result;
							}
						}
					}
					break;

				case ProcedureStepType.While:
					while (Compare(procedureStep))
					{
						if (IsTimeOut)
							return Result.Normal;
						foreach (var childStep in procedureStep.Children[0].Children)
						{
							var result = RunStep(childStep);
							if (result == Result.Break)
								return Result.Normal;
							if (result == Result.Continue)
								break;
							if (result == Result.Exit)
								return Result.Exit;
						}
					}
					break;

				case ProcedureStepType.GenerateGuid:
					var generateGuidArguments = procedureStep.GenerateGuidArguments;
					SetValue(generateGuidArguments.ResultArgument, Guid.NewGuid());
					break;

				case ProcedureStepType.SetJournalItemGuid:
					SetJournalItemGuid(procedureStep);
					break;

				case ProcedureStepType.Foreach:
					var foreachArguments = procedureStep.ForeachArguments;
					var listVariable = AllVariables.FirstOrDefault(x => x.UID == foreachArguments.ListArgument.VariableUid);
					var itemVariable = AllVariables.FirstOrDefault(x => x.UID == foreachArguments.ItemArgument.VariableUid);
					if (listVariable != null)
						foreach (var explicitValue in listVariable.VariableValue.ExplicitValues)
						{
							if (itemVariable != null)
								SetValue(itemVariable, GetValue<object>(explicitValue, itemVariable.VariableValue.ExplicitType, itemVariable.VariableValue.EnumType));
							foreach (var childStep in procedureStep.Children[0].Children)
							{
								var result = RunStep(childStep);
								if (result == Result.Break)
									return Result.Normal;
								if (result == Result.Continue)
									break;
								if (result == Result.Exit)
									return Result.Exit;
							}
						}
					break;

				case ProcedureStepType.For:
					var forArguments = procedureStep.ForArguments;
					var indexerVariable = AllVariables.FirstOrDefault(x => x.UID == forArguments.IndexerArgument.VariableUid);
					var initialValue = GetValue<int>(forArguments.InitialValueArgument);
					var value = GetValue<int>(forArguments.ValueArgument);
					var iterator = GetValue<int>(forArguments.IteratorArgument);
					if (indexerVariable != null)
					{
						var condition = Compare(initialValue, value, forArguments.ConditionType);
						var currentIntValue = indexerVariable.VariableValue.ExplicitValue.IntValue;
						for (indexerVariable.VariableValue.ExplicitValue.IntValue = initialValue; condition != null && condition.Value; )
						{
							if (IsTimeOut)
								return Result.Normal;
							foreach (var childStep in procedureStep.Children[0].Children)
							{
								var result = RunStep(childStep);
								if (result == Result.Break)
								{
									indexerVariable.VariableValue.ExplicitValue.IntValue = currentIntValue;
									return Result.Normal;
								}
								if (result == Result.Continue)
									break;
								if (result == Result.Exit)
									return Result.Exit;
							}
							indexerVariable.VariableValue.ExplicitValue.IntValue = indexerVariable.VariableValue.ExplicitValue.IntValue + iterator;
							condition = Compare(indexerVariable.VariableValue.ExplicitValue.IntValue, value, forArguments.ConditionType);
						}
						indexerVariable.VariableValue.ExplicitValue.IntValue = currentIntValue;
					}
					break;

				case ProcedureStepType.ProcedureSelection:
					{
						var childProcedure = ConfigurationCashHelper.SystemConfiguration.AutomationConfiguration.Procedures.
								FirstOrDefault(x => x.Uid == procedureStep.ProcedureSelectionArguments.ScheduleProcedure.ProcedureUid);
						if (childProcedure != null)
							ProcedureRunner.Run(childProcedure, procedureStep.ProcedureSelectionArguments.ScheduleProcedure.Arguments, AllVariables, User, JournalItem, ClientUID);
					}
					break;

				case ProcedureStepType.GetObjectProperty:
					GetObjectProperty(procedureStep);
					break;

				case ProcedureStepType.Arithmetics:
					Calculate(procedureStep);
					break;

				case ProcedureStepType.PlaySound:
					PlaySound(procedureStep);
					break;

				case ProcedureStepType.Pause:
					Pause(procedureStep);
					break;

				case ProcedureStepType.AddJournalItem:
					AddJournalItem(procedureStep);
					break;

				case ProcedureStepType.ShowMessage:
					ShowMessage(procedureStep);
					break;

				case ProcedureStepType.FindObjects:
					FindObjects(procedureStep);
					break;

				case ProcedureStepType.ControlSKDDevice:
					ControlSKDDevice(procedureStep);
					break;

				case ProcedureStepType.ControlDoor:
					ControlDoor(procedureStep);
					break;

				case ProcedureStepType.ControlSKDZone:
					ControlSKDZone(procedureStep);
					break;

				case ProcedureStepType.IncrementValue:
					IncrementValue(procedureStep);
					break;

				case ProcedureStepType.SetValue:
					SetValue(procedureStep);
					break;

				case ProcedureStepType.Random:
					GetRandomValue(procedureStep);
					break;

				case ProcedureStepType.ChangeList:
					ChangeList(procedureStep);
					break;

				case ProcedureStepType.CheckPermission:
					CheckPermission(procedureStep);
					break;

				case ProcedureStepType.GetListCount:
					GetListCount(procedureStep);
					break;

				case ProcedureStepType.GetListItem:
					GetListItem(procedureStep);
					break;

				case ProcedureStepType.GetJournalItem:
					GetJournalItem(procedureStep);
					break;

				case ProcedureStepType.ControlVisualGet:
					ControlVisual(procedureStep, ControlElementType.Get);
					break;

				case ProcedureStepType.ControlVisualSet:
					ControlVisual(procedureStep, ControlElementType.Set);
					break;

				case ProcedureStepType.ControlPlanGet:
					ControlPlan(procedureStep, ControlElementType.Get);
					break;

				case ProcedureStepType.ControlPlanSet:
					ControlPlan(procedureStep, ControlElementType.Set);
					break;

				case ProcedureStepType.ShowDialog:
					ShowDialog(procedureStep);
					break;

				case ProcedureStepType.ShowProperty:
					ShowProperty(procedureStep);
					break;

				case ProcedureStepType.SendEmail:
					SendEmail(procedureStep);
					break;

				case ProcedureStepType.Exit:
					return Result.Exit;

				case ProcedureStepType.Break:
					return Result.Break;

				case ProcedureStepType.Continue:
					return Result.Continue;

				case ProcedureStepType.ExportJournal:
					ExportJournal(procedureStep);
					break;

				case ProcedureStepType.ExportOrganisation:
					ExportOrganisation(procedureStep);
					break;

				case ProcedureStepType.ImportOrganisation:
					ImportOrganisation(procedureStep);
					break;

				case ProcedureStepType.ExportOrganisationList:
					ExportOrganisationList(procedureStep);
					break;

				case ProcedureStepType.ImportOrganisationList:
					ImportOrganisationList(procedureStep);
					break;

				case ProcedureStepType.ExportConfiguration:
					ExportConfiguration(procedureStep);
					break;

				case ProcedureStepType.Ptz:
					Ptz(procedureStep);
					break;

				case ProcedureStepType.StartRecord:
					StartRecord(procedureStep);
					break;

				case ProcedureStepType.StopRecord:
					StopRecord(procedureStep);
					break;

				case ProcedureStepType.RviAlarm:
					RviAlarm(procedureStep);
					break;
				case ProcedureStepType.ExportReport:
					ExportReport(procedureStep);
					break;
				case ProcedureStepType.GetDateTimeNow:
					GetDateTimeNowStep(procedureStep);
					break;

				// Получение свойства устройства СКД
				case ProcedureStepType.GetSkdDeviceProperty:
					GetSkdDeviceProperty(procedureStep);
					break;

				// Получение свойства точки доступа
				case ProcedureStepType.GetDoorProperty:
					GetDoorProperty(procedureStep);
					break;

				// Получение свойства зоны СКД
				case ProcedureStepType.GetSkdZoneProperty:
					GetSkdZoneProperty(procedureStep);
					break;
			}
			return Result.Normal;
		}

		private enum Result
		{
			Normal,
			Break,
			Continue,
			Exit
		}

		public void InitializeArguments(List<IVariable> variables, List<Argument> arguments, List<IVariable> callingProcedureVariables)
		{
			var i = 0;
			foreach (var variable in variables)
			{
				variable.VariableValue.ExplicitValues = new List<ExplicitValue>();
				if (arguments.Count <= i)
					break;
				var argument = arguments[i];
				if (argument == null)
					break;
				if (argument.VariableScope == VariableScope.ExplicitValue)
				{
					PropertyCopy.Copy(argument.ExplicitValue, variable.VariableValue.ExplicitValue);
					foreach (var explicitVal in argument.ExplicitValues)
					{
						var newExplicitValue = new ExplicitValue();
						PropertyCopy.Copy(explicitVal, newExplicitValue);
						variable.VariableValue.ExplicitValues.Add(newExplicitValue);
					}
				}
				else
				{
					var argumentVariable = callingProcedureVariables.FirstOrDefault(x => x.UID == argument.VariableUid);
					if (argumentVariable == null)
						continue;
					if (argumentVariable.IsReference)
					{
						variable.VariableValue.ExplicitValue = argumentVariable.VariableValue.ExplicitValue;
						variable.VariableValue.ExplicitValues = argumentVariable.VariableValue.ExplicitValues;
					}
					else
					{
						PropertyCopy.Copy(argumentVariable.VariableValue.ExplicitValue, variable.VariableValue.ExplicitValue);
						foreach (var explicitVal in argumentVariable.VariableValue.ExplicitValues)
						{
							var newExplicitValue = new ExplicitValue();
							PropertyCopy.Copy(explicitVal, newExplicitValue);
							variable.VariableValue.ExplicitValues.Add(newExplicitValue);
						}
					}
				}
				i++;
			}
		}
	}
}
using Common;
using RubezhAPI.Automation;
using RubezhAPI.Journal;
using RubezhAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Infrastructure.Automation
{
	public partial class ProcedureThread
	{
		public Guid UID { get; private set; }
		public Guid ClientUID { get; private set; }
		public ContextType ContextType { get; private set; }
		public DateTime StartTime { get; private set; }
		public bool IsAlive { get; set; }
		public User User { get; private set; }
		public int TimeOut { get; private set; }
		public TimeType TimeType { get; private set; }
		AutoResetEvent AutoResetEvent { get; set; }
		Thread Thread { get; set; }
		List<Variable> AllVariables { get; set; }
		List<ProcedureStep> Steps { get; set; }
		bool IsSync { get; set; }
		ProcedureThread ChildProcedureThread { get; set; }
		JournalItem JournalItem { get; set; }

		public ProcedureThread(Procedure procedure, List<Argument> arguments, List<Variable> callingProcedureVariables, JournalItem journalItem = null, User user = null, Guid? clientUID = null)
		{
			UID = Guid.NewGuid();
			ClientUID = clientUID.HasValue ? clientUID.Value : Guid.Empty;
			ContextType = procedure.ContextType;
			User = user;
			IsAlive = true;
			JournalItem = journalItem;
			TimeOut = procedure.TimeOut;
			TimeType = procedure.TimeType;
			AutoResetEvent = new AutoResetEvent(false);
			Steps = procedure.Steps;
			AllVariables = Utils.Clone(procedure.Variables);
			var procedureArguments = Utils.Clone(procedure.Arguments);
			InitializeArguments(procedureArguments, arguments, callingProcedureVariables);
			AllVariables.AddRange(procedureArguments);
			AllVariables.AddRange(ProcedureExecutionContext.GlobalVariables);
			Thread = new Thread(() => RunInThread(arguments))
			{
				Name = string.Format("ProcedureThread [{0}]", UID),
			};
			IsSync = procedure.IsSync;
		}

		public void Start()
		{
			StartTime = DateTime.Now;
			Thread.Start();
			if (IsSync)
				Thread.Join();
		}

		bool _isTimeOut;
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

		public bool RunInThread(List<Argument> arguments)
		{
			try
			{
				Steps.Any(x => RunStep(x) == Result.Exit);
				return true;
			}
			catch
			{
				return false;
			}
			finally
			{
				IsAlive = false;
			}
		}

		Result RunStep(ProcedureStep procedureStep)
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
					GenerateGuidStep(procedureStep);
					break;

				case ProcedureStepType.SetJournalItemGuid:
					SetJournalItemGuidStep(procedureStep);
					break;

				case ProcedureStepType.Foreach:
					var foreachStep = (ForeachStep)procedureStep;
					var listVariable = AllVariables.FirstOrDefault(x => x.Uid == foreachStep.ListArgument.VariableUid);
					var itemVariable = AllVariables.FirstOrDefault(x => x.Uid == foreachStep.ItemArgument.VariableUid);
					if (listVariable != null && listVariable.IsList)
						foreach (var listVariableItem in listVariable.Value as object[])
						{
							if (itemVariable != null)
								ProcedureExecutionContext.SetVariableValue(itemVariable, listVariableItem, ClientUID);
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
					var forStep = (ForStep)procedureStep;
					var indexerVariable = AllVariables.FirstOrDefault(x => x.Uid == forStep.IndexerArgument.VariableUid);
					var initialValue = GetValue<int>(forStep.InitialValueArgument);
					var value = GetValue<int>(forStep.ValueArgument);
					var iterator = GetValue<int>(forStep.IteratorArgument);
					if (indexerVariable != null)
					{
						var condition = Compare(initialValue, value, forStep.ConditionType);
						var currentIntValue = indexerVariable.ExplicitValue.IntValue;
						for (indexerVariable.ExplicitValue.IntValue = initialValue; condition != null && condition.Value; )
						{
							if (IsTimeOut)
								return Result.Normal;
							foreach (var childStep in procedureStep.Children[0].Children)
							{
								var result = RunStep(childStep);
								if (result == Result.Break)
								{
									indexerVariable.ExplicitValue.IntValue = currentIntValue;
									return Result.Normal;
								}
								if (result == Result.Continue)
									break;
								if (result == Result.Exit)
									return Result.Exit;
							}
							indexerVariable.ExplicitValue.IntValue = indexerVariable.ExplicitValue.IntValue + iterator;
							condition = Compare(indexerVariable.ExplicitValue.IntValue, value, forStep.ConditionType);
						}
						indexerVariable.ExplicitValue.IntValue = currentIntValue;
					}
					break;


				case ProcedureStepType.ProcedureSelection:
					ProcedureSelectionStep(procedureStep);
					break;

				case ProcedureStepType.GetObjectProperty:
					GetObjectPropertyStep(procedureStep);
					break;

				case ProcedureStepType.Arithmetics:
					ArithmeticStep(procedureStep);
					break;

				case ProcedureStepType.CreateColor:
					CreateColorStep(procedureStep);
					break;

				case ProcedureStepType.PlaySound:
					SoundStep(procedureStep);
					break;

				case ProcedureStepType.Pause:
					PauseStep(procedureStep);
					break;

				case ProcedureStepType.AddJournalItem:
					JournalStep(procedureStep);
					break;

				case ProcedureStepType.ShowMessage:
					ShowMessageStep(procedureStep);
					break;

				case ProcedureStepType.FindObjects:
					FindObjectStep(procedureStep);
					break;

				case ProcedureStepType.ControlGKDevice:
					ControlGKDeviceStep(procedureStep);
					break;

				case ProcedureStepType.ControlGKFireZone:
					ControlGKFireZoneStep(procedureStep);
					break;

				case ProcedureStepType.ControlGKGuardZone:
					ControlGKGuardZoneStep(procedureStep);
					break;

				case ProcedureStepType.ControlDirection:
					ControlDirectionStep(procedureStep);
					break;

				case ProcedureStepType.ControlGKDoor:
					ControlGKDoorStep(procedureStep);
					break;

				case ProcedureStepType.ControlDelay:
					ControlDelayStep(procedureStep);
					break;

				case ProcedureStepType.ControlPumpStation:
					ControlPumpStationStep(procedureStep);
					break;

				case ProcedureStepType.ControlMPT:
					ControlMPTStep(procedureStep);
					break;

				case ProcedureStepType.IncrementValue:
					IncrementValueStep(procedureStep);
					break;

				case ProcedureStepType.SetValue:
					SetValueStep(procedureStep);
					break;

				case ProcedureStepType.Random:
					RandomStep(procedureStep);
					break;

				case ProcedureStepType.ChangeList:
					ChangeListStep(procedureStep);
					break;

				case ProcedureStepType.CheckPermission:
					CheckPermissionStep(procedureStep);
					break;

				case ProcedureStepType.GetListCount:
					GetListCountStep(procedureStep);
					break;

				case ProcedureStepType.GetListItem:
					GetListItemStep(procedureStep);
					break;

				case ProcedureStepType.GetJournalItem:
					GetJournalItemStep(procedureStep);
					break;

				case ProcedureStepType.ControlVisualGet:
					ControlVisualGetStep(procedureStep);
					break;
				case ProcedureStepType.ControlVisualSet:
					ControlVisualSetStep(procedureStep);
					break;

				case ProcedureStepType.ControlPlanGet:
					ControlPlanGetStep(procedureStep);
					break;

				case ProcedureStepType.ControlPlanSet:
					ControlPlanSetStep(procedureStep);
					break;

				case ProcedureStepType.ControlOpcDaTagGet:
					ControlOpcDaTagGetStep(procedureStep);
					break;

				case ProcedureStepType.ControlOpcDaTagSet:
					ControlOpcDaTagSetStep(procedureStep);
					break;

				case ProcedureStepType.ShowDialog:
					ShowDialogStep(procedureStep);
					break;

				case ProcedureStepType.CloseDialog:
					CloseDialogStep(procedureStep);
					break;

				case ProcedureStepType.ShowProperty:
					ShowPropertyStep(procedureStep);
					break;

				case ProcedureStepType.SendEmail:
					SendEmailStep(procedureStep);
					break;

				case ProcedureStepType.Exit:
					return Result.Exit;

				case ProcedureStepType.Break:
					return Result.Break;

				case ProcedureStepType.Continue:
					return Result.Continue;

				case ProcedureStepType.ExportJournal:
					ExportJournalStep(procedureStep);
					break;

				case ProcedureStepType.ExportOrganisation:
					ExportOrganisationStep(procedureStep);
					break;

				case ProcedureStepType.ImportOrganisation:
					ImportOrganisationStep(procedureStep);
					break;

				case ProcedureStepType.ExportOrganisationList:
					ExportOrganisationListStep(procedureStep);
					break;

				case ProcedureStepType.ImportOrganisationList:
					ImportOrganisationListStep(procedureStep);
					break;

				case ProcedureStepType.ExportConfiguration:
					ExportConfigurationStep(procedureStep);
					break;

				case ProcedureStepType.Ptz:
					PtzStep(procedureStep);
					break;

				case ProcedureStepType.StartRecord:
					StartRecordStep(procedureStep);
					break;

				case ProcedureStepType.StopRecord:
					StopRecordStep(procedureStep);
					break;

				case ProcedureStepType.RviAlarm:
					RviAlarmStep(procedureStep);
					break;

				case ProcedureStepType.RviOpenWindow:
					RviOpenWindowStep(procedureStep);
					break;

				case ProcedureStepType.Now:
					NowStep(procedureStep);
					break;

				case ProcedureStepType.RunProgram:
					RunProgramStep(procedureStep);
					break;

				case ProcedureStepType.HttpRequest:
					HttpRequestStep(procedureStep);
					break;
			}
			return Result.Normal;
		}

		enum Result
		{
			Normal,
			Break,
			Continue,
			Exit
		}
	}
}
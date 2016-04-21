using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Common;
using FiresecAPI;
using FiresecAPI.Models.Automation;
using SKDDriver.DataAccess;

namespace SKDDriver.Translators
{
	public class GlobalVariablesTranslator
	{
		private readonly SKDDataContext _context;
		public GlobalVariablesTranslator(SKDDatabaseService databaseService)
		{
			_context = databaseService.Context;
		}

		public OperationResult<bool> Save(GlobalVariable variable)
		{
			if (variable == null) return new OperationResult<bool>(false);

			var serverVariable = _context.GlobalVariables.FirstOrDefault(x => x.UID == variable.UID);

			try
			{
				if (serverVariable != null)
					EditGlobalVariable(variable, serverVariable);
				else
					AddNewGlobalVariable(variable);
			}
			catch (Exception e)
			{
				Logger.Error(e);
				return new OperationResult<bool>(false);
			}

			return new OperationResult<bool>(true);
		}

		public OperationResult<bool> Save(List<IVariable> variables)
		{
			foreach (var variable in variables)
			{
				Save(variable as GlobalVariable);
			}

			return new OperationResult<bool>(true);
		}

		public static string SerializeObject<T>(T obj)
		{
			var xmlSerializer = new XmlSerializer(obj.GetType());

			using (var wr = new StringWriter())
			{
				xmlSerializer.Serialize(wr, obj);
				return wr.ToString();
			}
		}

		public static T DeserializeObject<T>(string xmlString)
		{
			var xmlSerializer = new XmlSerializer(typeof (T));

			using (TextReader reader = new StringReader(xmlString))
			{
				return (T)xmlSerializer.Deserialize(reader);
			}
		}

		public OperationResult<bool> Remove(GlobalVariable variable)
		{
			var item = _context.GlobalVariables.FirstOrDefault(x => x.UID == variable.UID);

			if (item != null)
			{
				_context.GlobalVariables.DeleteOnSubmit(item);
				_context.SubmitChanges();
			}

			return new OperationResult<bool>(true);
		}

		public OperationResult<List<GlobalVariable>> GetInitialGlobalVariables()
		{
			var result = _context.GlobalVariables.Select(variable => new GlobalVariable
			{
				IsReference = variable.IsReference,
				IsSaveWhenRestart = variable.IsSaveWhenRestart,
				Name = variable.Name,
				UID = variable.UID,
				VariableValue = DeserializeObject<VariableValue>(variable.InitialValueXML)
			})
			.ToList();

			return new OperationResult<List<GlobalVariable>>(result);
		}

		public OperationResult<bool> ResetGlobalVariables()
		{
			try
			{
				foreach (var variable in _context.GlobalVariables.Where(variable => !variable.IsSaveWhenRestart))
					variable.CurrentValueXML = variable.InitialValueXML;

				_context.SubmitChanges();
			}
			catch (Exception e)
			{
				Logger.Error(e);
				return new OperationResult<bool>(false);
			}

			return new OperationResult<bool>(true);
		}


		private void EditGlobalVariable(GlobalVariable variable, GlobalVariables serverVariable)
		{
			var xmlValue = SerializeObject(variable.VariableValue);
			serverVariable.UID = variable.UID;
			serverVariable.Name = variable.Name;
			serverVariable.IsReference = variable.IsReference;
			serverVariable.IsSaveWhenRestart = variable.IsSaveWhenRestart;
			serverVariable.CurrentValueXML = xmlValue;
			serverVariable.InitialValueXML = xmlValue;

			_context.SubmitChanges();
		}

		private void AddNewGlobalVariable(GlobalVariable variable)
		{
			var xmlValue = SerializeObject(variable.VariableValue);

			var serverVariable = new GlobalVariables
			{
				UID = variable.UID,
				Name = variable.Name,
				IsReference = variable.IsReference,
				IsSaveWhenRestart = variable.IsSaveWhenRestart,
				InitialValueXML = xmlValue,
				CurrentValueXML = xmlValue
			};

			_context.GlobalVariables.InsertOnSubmit(serverVariable);
			_context.SubmitChanges();
		}

		public OperationResult<List<GlobalVariable>> GetCurrentGlobalVariables()
		{
			var result = _context.GlobalVariables.Select(variable => new GlobalVariable
			{
				IsReference = variable.IsReference,
				IsSaveWhenRestart = variable.IsSaveWhenRestart,
				Name = variable.Name,
				UID = variable.UID,
				VariableValue = DeserializeObject<VariableValue>(variable.CurrentValueXML)
			})
			.ToList();

			return new OperationResult<List<GlobalVariable>>(result);
		}

	}
}

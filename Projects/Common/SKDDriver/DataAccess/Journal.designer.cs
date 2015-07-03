﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18444
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SKDDriver.DataAccess
{
	using System.Data.Linq;
	using System.Data.Linq.Mapping;
	using System.Data;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Linq;
	using System.Linq.Expressions;
	using System.ComponentModel;
	using System;
	
	
	[global::System.Data.Linq.Mapping.DatabaseAttribute(Name="Journal_1")]
	public partial class JournalDataContext : System.Data.Linq.DataContext
	{
		
		private static System.Data.Linq.Mapping.MappingSource mappingSource = new AttributeMappingSource();
		
    #region Extensibility Method Definitions
    partial void OnCreated();
    partial void InsertJournal(Journal instance);
    partial void UpdateJournal(Journal instance);
    partial void DeleteJournal(Journal instance);
    #endregion
		
		public JournalDataContext() : 
				base(global::SKDDriver.Properties.Settings.Default.Journal_1ConnectionString, mappingSource)
		{
			OnCreated();
		}
		
		public JournalDataContext(string connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public JournalDataContext(System.Data.IDbConnection connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public JournalDataContext(string connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public JournalDataContext(System.Data.IDbConnection connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public System.Data.Linq.Table<Journal> Journals
		{
			get
			{
				return this.GetTable<Journal>();
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.Journal")]
	public partial class Journal : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private System.Guid _UID;
		
		private System.DateTime _SystemDate;
		
		private System.Nullable<System.DateTime> _DeviceDate;
		
		private int _Subsystem;
		
		private int _Name;
		
		private int _Description;
		
		private string _NameText;
		
		private string _DescriptionText;
		
		private int _ObjectType;
		
		private string _ObjectName;
		
		private System.Guid _ObjectUID;
		
		private string _Detalisation;
		
		private string _UserName;
		
		private System.Nullable<System.Guid> _EmployeeUID;
		
		private System.Nullable<System.Guid> _VideoUID;
		
		private System.Nullable<System.Guid> _CameraUID;
		
		private System.Nullable<int> _ErrorCode;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnUIDChanging(System.Guid value);
    partial void OnUIDChanged();
    partial void OnSystemDateChanging(System.DateTime value);
    partial void OnSystemDateChanged();
    partial void OnDeviceDateChanging(System.Nullable<System.DateTime> value);
    partial void OnDeviceDateChanged();
    partial void OnSubsystemChanging(int value);
    partial void OnSubsystemChanged();
    partial void OnNameChanging(int value);
    partial void OnNameChanged();
    partial void OnDescriptionChanging(int value);
    partial void OnDescriptionChanged();
    partial void OnNameTextChanging(string value);
    partial void OnNameTextChanged();
    partial void OnDescriptionTextChanging(string value);
    partial void OnDescriptionTextChanged();
    partial void OnObjectTypeChanging(int value);
    partial void OnObjectTypeChanged();
    partial void OnObjectNameChanging(string value);
    partial void OnObjectNameChanged();
    partial void OnObjectUIDChanging(System.Guid value);
    partial void OnObjectUIDChanged();
    partial void OnDetalisationChanging(string value);
    partial void OnDetalisationChanged();
    partial void OnUserNameChanging(string value);
    partial void OnUserNameChanged();
    partial void OnEmployeeUIDChanging(System.Nullable<System.Guid> value);
    partial void OnEmployeeUIDChanged();
    partial void OnVideoUIDChanging(System.Nullable<System.Guid> value);
    partial void OnVideoUIDChanged();
    partial void OnCameraUIDChanging(System.Nullable<System.Guid> value);
    partial void OnCameraUIDChanged();
    partial void OnErrorCodeChanging(System.Nullable<int> value);
    partial void OnErrorCodeChanged();
    #endregion
		
		public Journal()
		{
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_UID", DbType="UniqueIdentifier NOT NULL", IsPrimaryKey=true)]
		public System.Guid UID
		{
			get
			{
				return this._UID;
			}
			set
			{
				if ((this._UID != value))
				{
					this.OnUIDChanging(value);
					this.SendPropertyChanging();
					this._UID = value;
					this.SendPropertyChanged("UID");
					this.OnUIDChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_SystemDate", DbType="DateTime NOT NULL")]
		public System.DateTime SystemDate
		{
			get
			{
				return this._SystemDate;
			}
			set
			{
				if ((this._SystemDate != value))
				{
					this.OnSystemDateChanging(value);
					this.SendPropertyChanging();
					this._SystemDate = value;
					this.SendPropertyChanged("SystemDate");
					this.OnSystemDateChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_DeviceDate", DbType="DateTime")]
		public System.Nullable<System.DateTime> DeviceDate
		{
			get
			{
				return this._DeviceDate;
			}
			set
			{
				if ((this._DeviceDate != value))
				{
					this.OnDeviceDateChanging(value);
					this.SendPropertyChanging();
					this._DeviceDate = value;
					this.SendPropertyChanged("DeviceDate");
					this.OnDeviceDateChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Subsystem", DbType="Int NOT NULL")]
		public int Subsystem
		{
			get
			{
				return this._Subsystem;
			}
			set
			{
				if ((this._Subsystem != value))
				{
					this.OnSubsystemChanging(value);
					this.SendPropertyChanging();
					this._Subsystem = value;
					this.SendPropertyChanged("Subsystem");
					this.OnSubsystemChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Name", DbType="Int NOT NULL")]
		public int Name
		{
			get
			{
				return this._Name;
			}
			set
			{
				if ((this._Name != value))
				{
					this.OnNameChanging(value);
					this.SendPropertyChanging();
					this._Name = value;
					this.SendPropertyChanged("Name");
					this.OnNameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Description", DbType="Int NOT NULL")]
		public int Description
		{
			get
			{
				return this._Description;
			}
			set
			{
				if ((this._Description != value))
				{
					this.OnDescriptionChanging(value);
					this.SendPropertyChanging();
					this._Description = value;
					this.SendPropertyChanged("Description");
					this.OnDescriptionChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_NameText", DbType="NVarChar(MAX)")]
		public string NameText
		{
			get
			{
				return this._NameText;
			}
			set
			{
				if ((this._NameText != value))
				{
					this.OnNameTextChanging(value);
					this.SendPropertyChanging();
					this._NameText = value;
					this.SendPropertyChanged("NameText");
					this.OnNameTextChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_DescriptionText", DbType="NVarChar(MAX)")]
		public string DescriptionText
		{
			get
			{
				return this._DescriptionText;
			}
			set
			{
				if ((this._DescriptionText != value))
				{
					this.OnDescriptionTextChanging(value);
					this.SendPropertyChanging();
					this._DescriptionText = value;
					this.SendPropertyChanged("DescriptionText");
					this.OnDescriptionTextChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ObjectType", DbType="Int NOT NULL")]
		public int ObjectType
		{
			get
			{
				return this._ObjectType;
			}
			set
			{
				if ((this._ObjectType != value))
				{
					this.OnObjectTypeChanging(value);
					this.SendPropertyChanging();
					this._ObjectType = value;
					this.SendPropertyChanged("ObjectType");
					this.OnObjectTypeChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ObjectName", DbType="NVarChar(50)")]
		public string ObjectName
		{
			get
			{
				return this._ObjectName;
			}
			set
			{
				if ((this._ObjectName != value))
				{
					this.OnObjectNameChanging(value);
					this.SendPropertyChanging();
					this._ObjectName = value;
					this.SendPropertyChanged("ObjectName");
					this.OnObjectNameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ObjectUID", DbType="UniqueIdentifier NOT NULL")]
		public System.Guid ObjectUID
		{
			get
			{
				return this._ObjectUID;
			}
			set
			{
				if ((this._ObjectUID != value))
				{
					this.OnObjectUIDChanging(value);
					this.SendPropertyChanging();
					this._ObjectUID = value;
					this.SendPropertyChanged("ObjectUID");
					this.OnObjectUIDChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Detalisation", DbType="Text", UpdateCheck=UpdateCheck.Never)]
		public string Detalisation
		{
			get
			{
				return this._Detalisation;
			}
			set
			{
				if ((this._Detalisation != value))
				{
					this.OnDetalisationChanging(value);
					this.SendPropertyChanging();
					this._Detalisation = value;
					this.SendPropertyChanged("Detalisation");
					this.OnDetalisationChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_UserName", DbType="NVarChar(50)")]
		public string UserName
		{
			get
			{
				return this._UserName;
			}
			set
			{
				if ((this._UserName != value))
				{
					this.OnUserNameChanging(value);
					this.SendPropertyChanging();
					this._UserName = value;
					this.SendPropertyChanged("UserName");
					this.OnUserNameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_EmployeeUID", DbType="UniqueIdentifier")]
		public System.Nullable<System.Guid> EmployeeUID
		{
			get
			{
				return this._EmployeeUID;
			}
			set
			{
				if ((this._EmployeeUID != value))
				{
					this.OnEmployeeUIDChanging(value);
					this.SendPropertyChanging();
					this._EmployeeUID = value;
					this.SendPropertyChanged("EmployeeUID");
					this.OnEmployeeUIDChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_VideoUID", DbType="UniqueIdentifier")]
		public System.Nullable<System.Guid> VideoUID
		{
			get
			{
				return this._VideoUID;
			}
			set
			{
				if ((this._VideoUID != value))
				{
					this.OnVideoUIDChanging(value);
					this.SendPropertyChanging();
					this._VideoUID = value;
					this.SendPropertyChanged("VideoUID");
					this.OnVideoUIDChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_CameraUID", DbType="UniqueIdentifier")]
		public System.Nullable<System.Guid> CameraUID
		{
			get
			{
				return this._CameraUID;
			}
			set
			{
				if ((this._CameraUID != value))
				{
					this.OnCameraUIDChanging(value);
					this.SendPropertyChanging();
					this._CameraUID = value;
					this.SendPropertyChanged("CameraUID");
					this.OnCameraUIDChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ErrorCode", DbType="int")]
		public System.Nullable<int> ErrorCode
		{
			get
			{
				return this._ErrorCode;
			}
			set
			{
				if ((this._ErrorCode != value))
				{
					this.OnErrorCodeChanging(value);
					this.SendPropertyChanging();
					this._ErrorCode = value;
					this.SendPropertyChanged("ErrorCode");
					this.OnErrorCodeChanged();
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
#pragma warning restore 1591

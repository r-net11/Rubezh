﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18444
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SKDDriver.Resources.Language {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class TimeTrackTranslator {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal TimeTrackTranslator() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("SKDDriver.Resources.Language.TimeTrackTranslator", typeof(TimeTrackTranslator).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Не найден сотрудник.
        /// </summary>
        internal static string GetEmployeeTimeTrack_Employee_Error {
            get {
                return ResourceManager.GetString("GetEmployeeTimeTrack_Employee_Error", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Не найден график.
        /// </summary>
        internal static string GetEmployeeTimeTrack_Schedule_Error {
            get {
                return ResourceManager.GetString("GetEmployeeTimeTrack_Schedule_Error", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to До начала действия графика.
        /// </summary>
        internal static string GetEmployeeTimeTrack_Schedule_Start_Error {
            get {
                return ResourceManager.GetString("GetEmployeeTimeTrack_Schedule_Start_Error", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Не найдена схема работы.
        /// </summary>
        internal static string GetEmployeeTimeTrack_ScheduleScheme_Error {
            get {
                return ResourceManager.GetString("GetEmployeeTimeTrack_ScheduleScheme_Error", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Не найден день.
        /// </summary>
        internal static string GetPlannedTimeTrackPart_Schedule_Day_Error {
            get {
                return ResourceManager.GetString("GetPlannedTimeTrackPart_Schedule_Day_Error", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Не найден дневной интервал.
        /// </summary>
        internal static string GetPlannedTimeTrackPart_Schedule_DayInterval_Error {
            get {
                return ResourceManager.GetString("GetPlannedTimeTrackPart_Schedule_DayInterval_Error", resourceCulture);
            }
        }
    }
}

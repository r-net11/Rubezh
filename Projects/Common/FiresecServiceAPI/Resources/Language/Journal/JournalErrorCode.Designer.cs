﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18444
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FiresecAPI.Resources.Language.Journal {
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
    internal class JournalErrorCode {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal JournalErrorCode() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("FiresecAPI.Resources.Language.Journal.JournalErrorCode", typeof(JournalErrorCode).Assembly);
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
        ///   Looks up a localized string similar to Открыта другая дверь шлюза.
        /// </summary>
        internal static string AbInterlockStatus {
            get {
                return ResourceManager.GetString("AbInterlockStatus", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Повторный проход в зону.
        /// </summary>
        internal static string AntipassBack {
            get {
                return ResourceManager.GetString("AntipassBack", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Неверный пароль пропуска.
        /// </summary>
        internal static string CardCorrectInputPasswordError {
            get {
                return ResourceManager.GetString("CardCorrectInputPasswordError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Пропуск заблокирован.
        /// </summary>
        internal static string CardLostOrCancelled {
            get {
                return ResourceManager.GetString("CardLostOrCancelled", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &quot;Замок в состоянии &apos;Взлом&apos;.
        /// </summary>
        internal static string DeviceIsUnderIntrusionAlam {
            get {
                return ResourceManager.GetString("DeviceIsUnderIntrusionAlam", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Замок в режиме &apos;Закрыто&apos;.
        /// </summary>
        internal static string DoorNcStatus {
            get {
                return ResourceManager.GetString("DoorNcStatus", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Настройки замка не поддерживает пропуск &apos;Принуждение&apos;.
        /// </summary>
        internal static string IntimidationAlarmNotOn {
            get {
                return ResourceManager.GetString("IntimidationAlarmNotOn", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Нет ошибки.
        /// </summary>
        internal static string None {
            get {
                return ResourceManager.GetString("None", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Нет прав доступа.
        /// </summary>
        internal static string NoRight {
            get {
                return ResourceManager.GetString("NoRight", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Нарушение графика доступа.
        /// </summary>
        internal static string PeriodError {
            get {
                return ResourceManager.GetString("PeriodError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Неверный идентификатор.
        /// </summary>
        internal static string Unauthorized {
            get {
                return ResourceManager.GetString("Unauthorized", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Неверный метод открытия замка.
        /// </summary>
        internal static string UnlockModeError {
            get {
                return ResourceManager.GetString("UnlockModeError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Срок действия пропуска истек или не наступил.
        /// </summary>
        internal static string ValidityError {
            get {
                return ResourceManager.GetString("ValidityError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Ожидание подтверждения прохода.
        /// </summary>
        internal static string VerificationPassedControlNotAuthorized {
            get {
                return ResourceManager.GetString("VerificationPassedControlNotAuthorized", resourceCulture);
            }
        }
    }
}

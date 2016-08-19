using System.Collections;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Localization.Converters;
using Infrastructure.Common;
using License.Model.Entities;
using System;
using System.ComponentModel;

namespace KeyGenerator.Entities
{

    public class LicenseEntity
    {
		public string UID { get; private set; }
		public DateTime CreateDateTime { get; private set; }
        //[Description("Оперативная задача (подключение)")]
        [LocalizedDescription(typeof(Resources.Language.LicenseEntity), "OperatorConnectionsNumber")]
		public int OperatorConnectionsNumber { get; private set; }

		//[Description("Всего пользователей")]
        [LocalizedDescription(typeof(Resources.Language.LicenseEntity), "TotalUsers")]
		public int TotalUsers { get; private set; }

		//[Description("Лицензия неограниченное количество пользователей")]
        [LocalizedDescription(typeof(Resources.Language.LicenseEntity), "IsUnlimitedUsers")]
		public bool IsUnlimitedUsers { get; private set; }

		//[Description("Модуль \"Учет рабочего времени\"")]
        [LocalizedDescription(typeof(Resources.Language.LicenseEntity), "IsEnabledURV")]
		public bool IsEnabledURV { get; private set; }
		//[Description("Модуль \"Фотоверификация\"")]
        [LocalizedDescription(typeof(Resources.Language.LicenseEntity), "IsEnabledPhotoVerification")]
		public bool IsEnabledPhotoVerification { get; private set; }
		//[Description("Модуль \"Интеграция видео RVi\"")]
        [LocalizedDescription(typeof(Resources.Language.LicenseEntity), "IsEnabledRVI")]
		public bool IsEnabledRVI { get; private set; }
		//[Description("Модуль \"Автоматизация\"")]
        [LocalizedDescription(typeof(Resources.Language.LicenseEntity), "IsEnabledAutomation")]
		public bool IsEnabledAutomation { get; private set; }

		//[Description("Сервер A.C. Tech")]
        [LocalizedDescription(typeof(Resources.Language.LicenseEntity), "IsEnabledServer")]
		public bool IsEnabledServer { get; private set; }

		public LicenseEntity(ILicenseEntity entity)
		{
			if (entity == null) return;

			UID = entity.UID;
			CreateDateTime = entity.CreateDateTime;
			OperatorConnectionsNumber = entity.OperatorConnectionsNumber;
			TotalUsers = entity.TotalUsers;
			IsUnlimitedUsers = entity.IsUnlimitedUsers;
			IsEnabledURV = entity.IsEnabledURV;
			IsEnabledPhotoVerification = entity.IsEnabledPhotoVerification;
			IsEnabledRVI = entity.IsEnabledRVI;
			IsEnabledAutomation = entity.IsEnabledAutomation;
			IsEnabledServer = entity.IsEnabledServer;
		}
	}
}

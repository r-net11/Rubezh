using Localization.Converters;
using License.Model.Entities;
using System;
using Localization.KeyGenerator.Common;

namespace KeyGenerator.Entities
{

    public class LicenseEntity
    {
		public string UID { get; private set; }
		public DateTime CreateDateTime { get; private set; }

        //[Description("Оперативная задача (подключение)")]
        [LocalizedDescription(typeof(CommonResources), "OperatorConnectionsNumber")]
		public int OperatorConnectionsNumber { get; private set; }

		//[Description("Всего пользователей")]
        [LocalizedDescription(typeof(CommonResources), "TotalUsers")]
		public int TotalUsers { get; private set; }

		//[Description("Лицензия неограниченное количество пользователей")]
        [LocalizedDescription(typeof(CommonResources), "IsUnlimitedUsers")]
		public bool IsUnlimitedUsers { get; private set; }

		//[Description("Модуль \"Учет рабочего времени\"")]
        [LocalizedDescription(typeof(CommonResources), "IsEnabledURV")]
		public bool IsEnabledURV { get; private set; }

		//[Description("Модуль \"Фотоверификация\"")]
        [LocalizedDescription(typeof(CommonResources), "IsEnabledPhotoVerification")]
		public bool IsEnabledPhotoVerification { get; private set; }

		//[Description("Модуль \"Интеграция видео RVi\"")]
        [LocalizedDescription(typeof(CommonResources), "IsEnabledRVI")]
		public bool IsEnabledRVI { get; private set; }

		//[Description("Модуль \"Автоматизация\"")]
        [LocalizedDescription(typeof(CommonResources), "IsEnabledAutomation")]
		public bool IsEnabledAutomation { get; private set; }

		//[Description("Сервер A.C. Tech")]
        [LocalizedDescription(typeof(CommonResources), "IsEnabledServer")]
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

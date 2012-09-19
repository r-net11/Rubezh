using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Validation;
using FiresecClient;

namespace OPCModule.Validation
{
    public static class Validator
    {
        public static IEnumerable<IValidationError> Validate()
        {
            var opcCount = 0;
            foreach (var device in FiresecManager.Devices)
            {
                if (device.IsOPCUsed)
                    opcCount++;
            }
            foreach (var zone in FiresecManager.Zones)
            {
                if (zone.IsOPCUsed)
                    opcCount++;
            }

            var _errors = new List<IValidationError>();
            if (opcCount >= 100)
            {
                _errors.Add(new CommonValidationError("FS", "OPC", string.Empty, string.Format("Суммарное количество устройств и зон, использующихся в OPC-сервере, не должно превышать 100"), ValidationErrorLevel.CannotSave));
            }
            return _errors;
        }
    }
}
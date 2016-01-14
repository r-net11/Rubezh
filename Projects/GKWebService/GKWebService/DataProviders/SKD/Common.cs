using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RubezhAPI;

namespace GKWebService.DataProviders.SKD
{
    static class Common
    {
        public static bool ThrowErrorIfExists(OperationResult operationResult)
        {
            if (operationResult == null)
            {
                throw new InvalidOperationException("Сервис вернул ссылку на объект не указывающую на экземпляр объекта");
            }
            if (operationResult.HasWarnings)
            {
            }
            if (operationResult.HasError)
            {
                if (operationResult.Error.Contains("String or binary data would be truncated"))
                    operationResult.Error = "Превышен максимальный размер строки";

                throw new InvalidOperationException(operationResult.Error);
            }
            return true;
        }

        public static T ThrowErrorIfExists<T>(OperationResult<T> operationResult)
        {
            if (operationResult == null)
            {
                throw new InvalidOperationException("Сервис вернул ссылку на объект не указывающую на экземпляр объекта");
            }
            if (operationResult.HasError)
            {
                throw new InvalidOperationException(operationResult.Error);
            }
            return operationResult.Result;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RubezhAPI;

namespace GKWebService.DataProviders.SKD
{
    static class Common
    {
		public static T ThrowErrorIfExists<T>(OperationResult<T> operationResult)
        {
            if (operationResult == null)
            {
                throw new InvalidOperationException("Сервис вернул ссылку на объект не указывающую на экземпляр объекта");
            }
            if (operationResult.Errors != null && operationResult.HasError)
            {
                throw new InvalidOperationException(operationResult.Error);
            }
            return operationResult.Result;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Infrastructure.Common.Windows.Windows;
using RubezhAPI.SKD;

namespace GKWebService.Models.SKD.Common
{
    public static class DetailsValidateHelper
    {
        public static string Validate(IOrganisationElement item)
        {
            if (item.Name != null && item.Name.Length > 50)
            {
                return "Значение поля 'Название' не может быть длиннее 50 символов";
            }
            if (item.Description != null && item.Description.Length > 50)
            {
                return "Значение поля 'Примечание' не может быть длиннее 50 символов";
            }
            return string.Empty;
        }
    }
}
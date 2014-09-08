//using System;
//using System.Collections.Generic;
//using System.Linq;
//using FiresecAPI.SKD;
//using FiresecClient.SKDHelpers;
//using Infrastructure.Common.Validation;

//namespace SKDModule.Validation
//{
//    public partial class Validator
//    {
//        const string DifferentOrganisationInSameTemplate = "Шаблон содержит дополнительные колонки относящиеся к разным организациям";
//        const string UnknownAddtionColumn = "Шаблон содержит неопределенные дополнительные колонки";

//        void ValidatePassCards()
//        {
//            SKDManager.SKDPassCardLibraryConfiguration.Templates.ForEach(template =>
//            {
//                var additionalColumnUIDs = new List<Guid>();
//                var haveEmpty = false;
//                foreach (var imageProperty in template.ElementImageProperties)
//                    if (imageProperty.PropertyType == PassCardImagePropertyType.Additional)
//                    {
//                        if (imageProperty.AdditionalColumnUID != Guid.Empty)
//                            additionalColumnUIDs.Add(imageProperty.AdditionalColumnUID);
//                        else
//                            haveEmpty = true;
//                    }
//                foreach (var textProperty in template.ElementTextProperties)
//                    if (textProperty.PropertyType == PassCardTextPropertyType.Additional)
//                    {
//                        if (textProperty.AdditionalColumnUID != Guid.Empty)
//                            additionalColumnUIDs.Add(textProperty.AdditionalColumnUID);
//                        else
//                            haveEmpty = true;
//                    }
//                if (haveEmpty)
//                    Errors.Add(new PassCardValidationError(template, UnknownAddtionColumn, ValidationErrorLevel.Warning));
//                if (additionalColumnUIDs.Count > 1)
//                {
//                    var organisationUID = GetOrganisationUID(additionalColumnUIDs[0]);
//                    if (!organisationUID.HasValue)
//                        Errors.Add(new PassCardValidationError(template, DifferentOrganisationInSameTemplate, ValidationErrorLevel.Warning));
//                    else
//                        for (int i = 1; i < additionalColumnUIDs.Count; i++)
//                            if (organisationUID != GetOrganisationUID(additionalColumnUIDs[i]))
//                            {
//                                Errors.Add(new PassCardValidationError(template, DifferentOrganisationInSameTemplate, ValidationErrorLevel.Warning));
//                                break;
//                            }
//                }
//            });
//        }

//        Guid? GetOrganisationUID(Guid additionalColumnGuid)
//        {
//            var additionalColumn = AdditionalColumns.FirstOrDefault(item => item.UID == additionalColumnGuid);
//            return additionalColumn == null ? (Guid?)null : additionalColumn.OrganisationUID;
//        }

//        List<ShortAdditionalColumnType> _addtionalColumns = null;
//        List<ShortAdditionalColumnType> AdditionalColumns
//        {
//            get
//            {
//                if (_addtionalColumns == null)
//                {
//                    var filter = new AdditionalColumnTypeFilter()
//                    {
//                        LogicalDeletationType = LogicalDeletationType.Active,
//                    };
//                    _addtionalColumns = AdditionalColumnTypeHelper.Get(filter).ToList();
//                }
//                return _addtionalColumns;
//            }
//        }
//    }
//}
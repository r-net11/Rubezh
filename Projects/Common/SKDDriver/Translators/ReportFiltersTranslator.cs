using Common;
using StrazhAPI;
using StrazhAPI.Enums;
using StrazhAPI.Models;
using StrazhAPI.SKD.ReportFilters;
using StrazhDAL.DataAccess;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

namespace StrazhDAL
{
    public class ReportFiltersTranslator
    {
        private readonly SKDDataContext _context;

        public ReportFiltersTranslator(SKDDatabaseService databaseService)
        {
            _context = databaseService.Context;
        }

        public OperationResult<List<SKDReportFilter>> GetAllFilters()
        {
            var result = GetDefaultFilters().ToList();
            result.AddRange(_context.Filters.Select(x => GetFilterFromXml(x.XMLContent, (ReportType)x.Type)));

            return new OperationResult<List<SKDReportFilter>>(result);
        }

        private static IEnumerable<SKDReportFilter> GetDefaultFilters()
        {
            return Assembly.GetAssembly(typeof(SKDReportFilter)).GetTypes()
                .Where(x => x.IsSubclassOf(typeof(SKDReportFilter)))
                .Select(type => ((SKDReportFilter)Activator.CreateInstance(type)));
        }

        public OperationResult<List<SKDReportFilter>> GetReportFiltersByType(User user, ReportType type)
        {
            var result = new List<SKDReportFilter>();
            try
            {
                var allFilters = _context.Filters.Where(x => x.UserID == user.UID && string.Equals(x.Type, type));
                foreach (var reportFilter in allFilters)
                {
                    var filter = GetFilterFromXml(reportFilter.XMLContent, type);
                    if (filter != null)
                    {
                        result.Add(filter);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return OperationResult<List<SKDReportFilter>>.FromError(e.Message);
            }

            return new OperationResult<List<SKDReportFilter>>(result);
        }

        private static SKDReportFilter GetFilterFromXml(string xml, ReportType filterType)
        {
            switch (filterType)
            {
                case ReportType.SchedulesReport:
                    return DeserializeFilter<SchedulesReportFilter>(xml);
                case ReportType.CardsReport:
                    return DeserializeFilter<CardsReportFilter>(xml);
                case ReportType.DepartmentsReport:
                    return DeserializeFilter<DepartmentsReportFilter>(xml);
                case ReportType.DisciplineReport:
                    return DeserializeFilter<DisciplineReportFilter>(xml);
                case ReportType.DocumentsReport:
                    return DeserializeFilter<DocumentsReportFilter>(xml);
                case ReportType.DoorsReport:
                    return DeserializeFilter<DoorsReportFilter>(xml);
                case ReportType.EmployeeAccessReport:
                    return DeserializeFilter<EmployeeAccessReportFilter>(xml);
                case ReportType.EmployeeDoorsReport:
                    return DeserializeFilter<EmployeeDoorsReportFilter>(xml);
                case ReportType.EmployeeReport:
                    return DeserializeFilter<EmployeeReportFilter>(xml);
                case ReportType.EmployeeRootReport:
                    return DeserializeFilter<EmployeeRootReportFilter>(xml);
                case ReportType.EmployeeZonesReport:
                    return DeserializeFilter<EmployeeZonesReportFilter>(xml);
                case ReportType.EventsReport:
                    return DeserializeFilter<EventsReportFilter>(xml);
                case ReportType.PositionsReport:
                    return DeserializeFilter<PositionsReportFilter>(xml);
                case ReportType.WorkingTimeReport:
                    return DeserializeFilter<WorkingTimeReportFilter>(xml);
                default:
                    return null;
            }
        }

        public OperationResult<List<SKDReportFilter>> GetForUser(User user)
        {
            var allFilters = _context.Filters.Where(x => x.UserID == user.UID);
            var result = allFilters
                        .Select(reportFilter => GetFilterFromXml(reportFilter.XMLContent, (ReportType)reportFilter.Type))
                        .Where(filter => filter != null)
                        .ToList();

            return new OperationResult<List<SKDReportFilter>>(result);
        }

        public OperationResult<bool> Remove(SKDReportFilter filter, User user)
        {
            try
            {
                var exist = _context.Filters.FirstOrDefault(x => x.Name == filter.Name && x.Type == (int)filter.ReportType && x.UserID == user.UID);

                if (exist == null) return new OperationResult<bool>(false);

                _context.Filters.DeleteOnSubmit(exist);
                _context.SubmitChanges();
                return new OperationResult<bool>(true);
            }
            catch (Exception e)
            {
                Logger.Error(e);
				throw;
            }
        }

        public OperationResult<bool> Save(SKDReportFilter filter, User user)
        {
            try
            {
                var existing = _context.Filters.FirstOrDefault(x => x.Name == filter.Name && x.UserID == user.UID && x.Type == (int)filter.ReportType);

                if (existing == null)
                    _context.Filters.InsertOnSubmit(new Filters
                    {
                        UID = Guid.NewGuid(),
                        Name = filter.Name,
                        UserID = user.UID,
                        Type = (int)filter.ReportType,
                        XMLContent = SerializeFilter<SKDReportFilter>(filter)
                    });
                else
                    existing.XMLContent = SerializeFilter<SKDReportFilter>(filter);

                _context.SubmitChanges();
            }
            catch (Exception e)
            {
                Logger.Error(e);
				throw;
            }

            return new OperationResult<bool>(true);
        }

        private static string SerializeFilter<T>(SKDReportFilter filter) where T : SKDReportFilter
        {
            var serializer = new XmlSerializer<T>();
            var result = serializer.Serialize(filter as T);
            return result;
        }

        private static TSourse DeserializeFilter<TSourse>(string xml)
            where TSourse : class
        {
            if (string.IsNullOrEmpty(xml)) return null;
            var serializer = new XmlSerializer<TSourse>();
            return serializer.DeserializeFromString(xml);
        }

        public class XmlSerializer<T> where T : class
        {
            private readonly XmlSerializer _serializer;

            public XmlSerializer()
            {
                _serializer = new XmlSerializer(typeof(T));
            }

            public string Serialize(T obj)
            {
                return _serializer.Serialize(obj);
            }

            public void Serialize(T obj, string fileName)
            {
                _serializer.Serialize(obj, fileName);
            }

            public void Serialize(T obj, Stream stream)
            {
                _serializer.Serialize(obj, stream);
            }

            public void Serialize(T obj, XmlWriter xmlWriter)
            {
                _serializer.Serialize(obj, xmlWriter);
            }

            public T DeserializeFromString(string xml)
            {
                return (T)_serializer.DeserializeFromString(xml);
            }

            public T Deserialize(string fileName)
            {
                return (T)_serializer.Deserialize(fileName);
            }

            public T Deserialize(XmlReader xmlReader)
            {
                return (T)_serializer.Deserialize(xmlReader);
            }

            public T Deserialize(Stream stream)
            {
                return (T)_serializer.Deserialize(stream);
            }
        }

        public class XmlSerializer
        {
            private readonly DataContractSerializer _serializer;

            public XmlSerializer(Type type)
            {
                _serializer = new DataContractSerializer(type);
            }

            public string Serialize(object obj)
            {
                var sb = new StringBuilder();
                using (var writer = XmlWriter.Create(sb, new XmlWriterSettings { Indent = true }))
                {
                    _serializer.WriteObject(writer, obj);
                }
                return sb.ToString();
            }

            public void Serialize(object obj, string fileName)
            {
                using (XmlWriter xmlWriter = XmlWriter.Create(fileName, new XmlWriterSettings { Indent = true }))
                {
                    _serializer.WriteObject(xmlWriter, obj);
                }
            }

            public void Serialize(object obj, Stream stream)
            {
                _serializer.WriteObject(stream, obj);
                stream.Position = 0;
            }

            public void Serialize(object obj, XmlWriter xmlWriter)
            {
                _serializer.WriteObject(xmlWriter, obj);
            }

            public object DeserializeFromString(string xml)
            {
                object result;
                using (var reader = new XmlTextReader(new StringReader(xml)))
                {
                    result = _serializer.ReadObject(reader);
                }
                return result;
            }

            public object Deserialize(string fileName)
            {
                object result;

                using (XmlReader reader = XmlReader.Create(fileName))
                {
                    result = _serializer.ReadObject(reader);
                }
                return result;
            }

            public object Deserialize(XmlReader xmlReader)
            {
                return _serializer.ReadObject(xmlReader);
            }

            public object Deserialize(Stream stream)
            {
                stream.Position = 0;

                return _serializer.ReadObject(stream);
            }
        }
    }
}

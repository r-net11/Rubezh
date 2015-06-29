using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using API = FiresecAPI.SKD;

namespace SKDDriver.DataClasses
{
	public class PassCardTemplateTranslator : OrganisationItemTranslatorBase<PassCardTemplate, API.PassCardTemplate, API.PassCardTemplateFilter>
	{
		DataContractSerializer _serializer;
		public ShortPassCardTemplateTranslator ShortTranslator { get; private set; }
		
		public PassCardTemplateTranslator(DbService context)
			: base(context)
		{
			_serializer = new DataContractSerializer(typeof(API.PassCardTemplate));
			ShortTranslator = new ShortPassCardTemplateTranslator(this);
		}

		public override DbSet<PassCardTemplate> Table
		{
			get { return Context.PassCardTemplates; }
		}
		
		public override API.PassCardTemplate Translate(PassCardTemplate tableItem)
		{
            if (tableItem == null)
                return null;
            using (var ms = new MemoryStream(tableItem.Data.ToArray()))
			{
				var result = (API.PassCardTemplate)_serializer.ReadObject(ms);
				return result;
			}
		}

		public override void TranslateBack(API.PassCardTemplate apiItem, PassCardTemplate tableItem)
		{
			base.TranslateBack(apiItem, tableItem);
			using (var ms = new MemoryStream())
			{
				_serializer.WriteObject(ms, apiItem);
				tableItem.Data = ms.ToArray();
			}
		}
	}

	public class ShortPassCardTemplateTranslator : ShortTranslatorBase<PassCardTemplate, API.ShortPassCardTemplate, API.PassCardTemplate, API.PassCardTemplateFilter>
	{
		public ShortPassCardTemplateTranslator(PassCardTemplateTranslator translator) : base(translator) { }
	}
}
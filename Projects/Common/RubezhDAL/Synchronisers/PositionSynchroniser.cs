using System;
using System.Data.Linq;
using System.Linq.Expressions;
using RubezhAPI.SKD;
using LinqKit;
using System.Data.Entity;
using System.Linq;

namespace RubezhDAL.DataClasses
{
	public class PositionSynchroniser : Synchroniser<ExportPosition, Position>
	{
		public PositionSynchroniser(DbSet<Position> table, DbService databaseService) : base(table, databaseService) { }

		public override ExportPosition Translate(Position item)
		{
			return new ExportPosition 
			{ 
				Name = item.Name, 
				Description = item.Description,

				OrganisationUID = GetUID(item.OrganisationUID),
				OrganisationExternalKey = GetExternalKey(item.OrganisationUID, item.Organisation)
			};
		}

		protected override IQueryable<Position> GetFilteredItems(RubezhAPI.SKD.ExportFilter filter)
		{
			return base.GetFilteredItems(filter).Where(x => x.OrganisationUID == filter.OrganisationUID);
		}

		protected override string Name
		{
			get { return "Positions"; }
		}

		protected override string XmlHeaderName
		{
			get { return "ArrayOfExportPosition"; }
		}

		public override void TranslateBack(ExportPosition exportItem, Position tableItem)
		{
			tableItem.Name  = exportItem.Name;
			tableItem.Description = exportItem.Description;
			tableItem.IsDeleted = exportItem.IsDeleted;
			tableItem.RemovalDate = exportItem.RemovalDate;
		}

		protected override void UpdateForignKeys(ExportPosition exportItem, Position tableItem, OrganisationHRCash hrCash)
		{
			tableItem.OrganisationUID = hrCash.OrganisationUID;
		}
	}
}

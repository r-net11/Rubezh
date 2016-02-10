using RubezhAPI;
using System.Collections.Generic;
using System.Data.Entity;
using System.Runtime.Serialization;
using API = RubezhAPI.SKD;
using System.Linq;
using System;

namespace RubezhDAL.DataClasses
{
	public class PositionTranslator : OrganisationItemTranslatorBase<Position, API.Position, API.PositionFilter>
	{
		public PositionShortTranslator ShortTranslator { get; private set; }
		public PositionSynchroniser Synchroniser { get; private set; }
		public PositionTranslator(DbService context)
			: base(context)
		{
			ShortTranslator = new PositionShortTranslator(this);
			Synchroniser = new PositionSynchroniser(Table, DbService);
		}

		public override System.Linq.IQueryable<Position> GetFilteredTableItems(API.PositionFilter filter, System.Linq.IQueryable<Position> tableItems)
		{
			return base.GetFilteredTableItems(filter, tableItems);
		}

		public override DbSet<Position> Table
		{
			get { return Context.Positions; }
		}

		public override System.Linq.IQueryable<Position> GetTableItems()
		{
			return base.GetTableItems().Include(x => x.Photo);
		}

		public override void TranslateBack(API.Position apiItem, Position tableItem)
		{
			base.TranslateBack(apiItem, tableItem);
			tableItem.Photo = Photo.Create(apiItem.Photo);
		}

		public override OperationResult<bool> Save(API.Position item)
		{
			return base.Save(item);
		}

		protected override void ClearDependentData(Position tableItem)
		{
			if (tableItem.Photo != null)
				Context.Photos.Remove(tableItem.Photo);
		}

		protected override IEnumerable<API.Position> GetAPIItems(IQueryable<Position> tableItems)
		{
			return tableItems.Select(tableItem => new API.Position
			{
				UID = tableItem.UID,
				Name = tableItem.Name,
				Description = tableItem.Description,
				IsDeleted = tableItem.IsDeleted,
				RemovalDate = tableItem.RemovalDate != null ? tableItem.RemovalDate.Value : new DateTime(),
				OrganisationUID = tableItem.OrganisationUID != null ? tableItem.OrganisationUID.Value : Guid.Empty,
				Photo = tableItem.Photo != null ? new API.Photo { UID = tableItem.Photo.UID, Data = tableItem.Photo.Data } : null,
			});
		}

		public OperationResult<List<Guid>> GetEmployeeUIDs(Guid positionUID)
		{
			return DbServiceHelper.InTryCatch(() => 
			{
				return Context.Employees.Where(x => x.PositionUID == positionUID).Select(x => x.UID).ToList();
			});
		}
	}

	public class PositionShortTranslator : OrganisationShortTranslatorBase<Position, API.ShortPosition, API.Position, API.PositionFilter>
	{
		public PositionShortTranslator(PositionTranslator translator) : base(translator) { }

		public API.ShortPosition Translate(Position tableItem)
		{
			if (tableItem == null)
				return null;
			return new API.ShortPosition
			{
				UID = tableItem.UID,
				Name = tableItem.Name,
				Description = tableItem.Description,
				IsDeleted = tableItem.IsDeleted,
				RemovalDate = tableItem.RemovalDate.GetValueOrDefault(),
				OrganisationUID = tableItem.OrganisationUID.GetValueOrDefault()
			};
		}

		protected override IEnumerable<API.ShortPosition> GetAPIItems(System.Linq.IQueryable<Position> tableItems)
		{
			return tableItems.Select(tableItem => new API.ShortPosition
			{
				UID = tableItem.UID,
				Name = tableItem.Name,
				Description = tableItem.Description,
				IsDeleted = tableItem.IsDeleted,
				RemovalDate = tableItem.RemovalDate != null ? tableItem.RemovalDate.Value : new DateTime(),
				OrganisationUID = tableItem.OrganisationUID != null ? tableItem.OrganisationUID.Value : Guid.Empty
			});
		}
	}
}
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GKWebService.DataProviders.SKD;
using GKWebService.Models.SKD.Common;
using GKWebService.Utils;
using RubezhAPI.SKD;
using RubezhClient;

namespace GKWebService.Models.SKD.Positions
{
    public class PositionDetailsViewModel
    {
        public Position Position { get; set; }
        public string PhotoData { get; set; }
        public void Initialize(Guid orgnaisationUID, Guid? positionUID = null)
        {
            var isNew = positionUID == null;
            if (isNew)
            {
                Position = new Position()
                {
                    Name = "Новая должность",
                    OrganisationUID = orgnaisationUID
                };
            }
            else
            {
                var operationResult = PositionHelper.GetDetails(positionUID.Value);
                Position = operationResult;
            }
            if (Position.Photo != null && Position.Photo.Data != null)
            {
                PhotoData = string.Format("data:image/gif;base64,{0}", Convert.ToBase64String(Position.Photo.Data));
                Position.Photo.Data = null;
            }

        }

        public bool Save(bool isNew)
        {
            if ((PhotoData != null && PhotoData.Length > 0) || Position.Photo != null)
            {
                Position.Photo = new Photo();
                byte[] data = null;
                if (PhotoData != null)
                {
                    data = Convert.FromBase64String(PhotoData.Remove(0, "data:image/gif;base64,".Length));
                }
                Position.Photo.Data = data;
            }

            string error = DetailsValidateHelper.Validate(Position);

            if (!string.IsNullOrEmpty(error))
            {
                throw new InvalidOperationException(error);
            }

            var operationResult = PositionHelper.Save(Position, isNew);
            return operationResult;
        }

        public bool Paste()
        {
            var filter = new PositionFilter { OrganisationUIDs = new List<Guid> { Position.OrganisationUID }};
            var getPositionsResult = PositionHelper.Get(filter);

            var positions = getPositionsResult;

            Position.Name = CopyHelper.CopyName(Position.Name, positions.Select(x => x.Name));

            return Save(true);
        }
    }
}
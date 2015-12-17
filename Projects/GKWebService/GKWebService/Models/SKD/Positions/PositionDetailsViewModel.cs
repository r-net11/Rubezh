﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GKWebService.Models.SKD.Common;
using GKWebService.Utils;
using RubezhAPI.SKD;
using RubezhClient;
using RubezhClient.SKDHelpers;

namespace GKWebService.Models.SKD.Positions
{
    public class PositionDetailsViewModel
    {
        public Position Position { get; set; }
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
                var operationResult = ClientManager.FiresecService.GetPositionDetails(positionUID.Value);
                if (operationResult.HasError)
                {
                    throw new InvalidOperationException(operationResult.Error);
                }
                Position = operationResult.Result;
            }
            Position.Photo = null;
        }

        public string Save(bool isNew)
        {
            string error = DetailsValidateHelper.Validate(Position);

            if (!string.IsNullOrEmpty(error))
            {
                return error;
            }

            var operationResult = ClientManager.FiresecService.SavePosition(Position, isNew);
            return operationResult.Error;
        }

        public string Paste()
        {
            var filter = new PositionFilter { OrganisationUIDs = new List<Guid> { Position.OrganisationUID }};
            var getPositionsResult = ClientManager.FiresecService.GetPositionList(filter);
            if (getPositionsResult.HasError)
            {
                return getPositionsResult.Error;
            }

            var positions = getPositionsResult.Result;

            Position.Name = CopyHelper.CopyName(Position.Name, positions.Select(x => x.Name));

            return Save(true);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using GKWebService.Models;

namespace GKWebService.Controllers
{
    public class ShapesController : ApiController
    {
        public IEnumerable<Shape> Get()
        {
            var shape = new Shape
            {
                Id = 1,
                Name = "Sample Triangle 1",
                Fill = Color.Blue,
                Border = Color.Red,
                FillMouseOver = Color.Red,
                BorderMouseOver = Color.Blue,
                Path = "M 0 0 L 80 0 L 40 80 L 0 0 z"
            };
            var shape1 = new Shape
            {
                Id = 2,
                Name = "Sample Triangle 2",
                Fill = Color.Red,
                Border = Color.Blue,
                FillMouseOver = Color.Blue,
                BorderMouseOver = Color.Red,
                Path = "M 0 90 L 80 130 L 0 170 L 0 90 z"
            };
            var result = new List<Shape>
                         {
                             shape,
                             shape1
                         };
            return result;
        }
    }
}

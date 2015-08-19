using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

namespace GKWebService.Models
{
    public class Shape
    {
        public Color Border
        { get; set; }

        public Color BorderMouseOver
        { get; set; }

        public Color Fill
        { get; set; }

        public Color FillMouseOver
        { get; set; }

        public int Id
        { get; set; }

        public string Name
        { get; set; }

        public string Path
        { get; set; }
    }
}
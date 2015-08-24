using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using Color = System.Drawing.Color;

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

        public double BorderThickness { get; set; }

        public Guid Id
        { get; set; }

        public string Name
        { get; set; }

        public string Path
        { get; set; }

        public string Hint
        { get; set; }

        public string Image { get; set; }

        public double X { get; set; }
        public double Y { get; set; }

        public double Width { get; set; }
        public double Height { get; set; }
    }
}
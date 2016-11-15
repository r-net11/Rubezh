﻿using System.Runtime.Serialization;

namespace StrazhAPI.Models.Layouts
{
    [DataContract(Name = "LayoutPartProcedureProperties", Namespace = "")]
    public class LayoutPartProcedureProperties : LayoutPartReferenceProperties
    {
        public LayoutPartProcedureProperties()
        {
            Text = string.Empty;
            ForegroundColor = Colors.Black;
            BackgroundColor = Colors.White;
            BorderColor = Colors.Black;
            BorderThickness = 1;
            FontSize = 10;
            FontFamilyName = "Arial";
        }

        [DataMember]
        public bool UseCustomStyle { get; set; }

        [DataMember]
        public string Text { get; set; }

        [DataMember]
        public Color BorderColor { get; set; }

        [DataMember]
        public double BorderThickness { get; set; }

        [DataMember]
        public Color BackgroundColor { get; set; }

        [DataMember]
        public Color ForegroundColor { get; set; }

        [DataMember]
        public double FontSize { get; set; }

        [DataMember]
        public bool FontItalic { get; set; }

        [DataMember]
        public bool FontBold { get; set; }

        [DataMember]
        public string FontFamilyName { get; set; }

        [DataMember]
        public bool Stretch { get; set; }

        [DataMember]
        public int TextAlignment { get; set; }
    }
}
using System.Runtime.Serialization;

namespace StrazhAPI.Models.Layouts
{
    [DataContract(Name = "LayoutPartTextProperties", Namespace = "")]
    public class LayoutPartTextProperties : ILayoutProperties
    {
        public LayoutPartTextProperties()
        {
            BackgroundColor = Colors.Transparent;
            ForegroundColor = Colors.Black;
            Text = string.Empty;
            FontFamilyName = "Arial";
            FontSize = 12;
        }

        [DataMember]
        public int HorizontalAlignment { get; set; }

        [DataMember]
        public int VerticalAlignment { get; set; }

        [DataMember]
        public Color BackgroundColor { get; set; }

        [DataMember]
        public Color ForegroundColor { get; set; }

        [DataMember]
        public string Text { get; set; }

        [DataMember]
        public string FontFamilyName { get; set; }

        [DataMember]
        public double FontSize { get; set; }

        [DataMember]
        public bool FontItalic { get; set; }

        [DataMember]
        public bool FontBold { get; set; }

        [DataMember]
        public int TextAlignment { get; set; }

        [DataMember]
        public bool WordWrap { get; set; }

        [DataMember]
        public bool TextTrimming { get; set; }

        [DataMember]
        public bool AcceptReturn { get; set; }

        [DataMember]
        public bool AcceptTab { get; set; }
    }
}
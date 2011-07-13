using System;
using System.Xml.Serialization;

namespace DeviceLibrary.Models
{
    [Serializable]
    public class Frame
    {
        [XmlAttribute]
        public int Id { get; set; }

        [XmlAttribute]
        public int Duration { get; set; }

        [XmlAttribute]
        public int Layer { get; set;}

        private string _image;
        public string Image
        {
            get { return _image; }
            set
            {
                _image = value;
                //image = SvgConverter.Svg2Xaml(value, ResourceHelper.svg2xaml_xsl); // для загрузки из SVG
            } 
        }

    }
}

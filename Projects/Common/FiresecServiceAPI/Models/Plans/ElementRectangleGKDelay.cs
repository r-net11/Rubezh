using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Interfaces;

namespace FiresecAPI.Models
{
    [DataContract()]
    public class ElementRectangleGKDelay : ElementBaseRectangle, IPrimitive, IElementDelay, IElementReference
    {
        public ElementRectangleGKDelay()
        {
            PresentationName = "Задержка";
        }

        public override ElementBase Clone()
        {
            var elementBase = new ElementRectangleGKDelay();
            Copy(elementBase);
            return elementBase;
        }

        public override void Copy(ElementBase element)
        {
            base.Copy(element);
            ((ElementRectangleGKDelay)element).DelayUID = DelayUID;
        }

        [DataMember()]
        public Guid DelayUID { get; set; }

        [DataMember()]
        public bool ShowState { get; set; }

        #region IPrimitive Members

        [XmlIgnore()]
        public Primitive Primitive
        {
            get { return Infrustructure.Plans.Elements.Primitive.RectangleZone; }
        }

        #endregion

        public void SetZLayer(int zlayer)
        {
            ZLayer = zlayer;
        }

        #region IElementReference Members

        Guid IElementReference.ItemUID
        {
            get { return this.DelayUID; }
            set { this.DelayUID = value; }
        }

        #endregion

    }
}
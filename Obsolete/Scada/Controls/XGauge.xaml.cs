using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.Serialization;

namespace Controls
{
[Serializable]
    public partial class XGauge : ControlBase.UserControlBase
    {
        public XGauge()
            :base()
        {
            InitializeComponent();
        }

        protected XGauge(SerializationInfo info, StreamingContext ctxt)
            : base(info, ctxt)
        {
            InitializeComponent();
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            base.GetObjectData(info, ctxt);
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using Infrustructure.Plans.Elements;

namespace Infrustructure.Plans.Painters
{
	public interface IGeometryPainter : IPainter
	{
		Geometry Geometry { get; }
	}
}

using System;
using Infrustructure.Plans.Interfaces;

namespace Infrustructure.Plans.Elements
{
    /// <summary>
    /// Defines an Interface of a Delay Element.
    /// </summary>
    public interface IElementDelay : IElementReference
    {
        Guid DelayUID { get; set; }
        bool ShowState { get; set; }
    }
}

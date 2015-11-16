#region Usings

using System.Collections.Generic;

#endregion

namespace GKWebService.Models
{
    public class ElementHint
    {
        public ElementHint() {
            StateHintLines = new List<HintLine>();
        }
        public List<HintLine> StateHintLines { get; set; }
    }
}

#region Usings

using System.Collections.Generic;

#endregion

namespace GKWebService.Models.Plan.PlanElement.Hint
{
    public class ElementHint
    {
        public ElementHint() {
            StateHintLines = new List<HintLine>();
        }
        public List<HintLine> StateHintLines { get; set; }
		public string HintImage { get; set; }
		public double HintImageHeight { get; set; }
		public double HintImageWidth { get; set; }
    }
}

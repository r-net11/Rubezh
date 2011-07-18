
namespace FiresecClient.Models
{
    public class InnerState
    {
        Firesec.Metadata.configDrvState _innerState;

        public InnerState(Firesec.Metadata.configDrvState innerState)
        {
            _innerState = innerState;
        }

        public bool IsActive { get; set; }

        public string Id
        {
            get { return _innerState.id; }
        }

        public string Name
        {
            get { return _innerState.name; }
        }

        public bool AffectChildren
        {
            get { return _innerState.affectChildren == "1" ? true : false; }
        }

        public int Priority
        {
            get { return System.Convert.ToInt32(_innerState.@class); }
        }

        public State State
        {
            get { return new State(Priority); }
        }

        public bool IsManualReset
        {
            get { return _innerState.manualReset == "1" ? true : false; }
        }

        public bool CanResetOnPanel
        {
            get { return _innerState.CanResetOnPanel == "1" ? true : false; }
        }

        public bool IsAutomatic
        {
            get { return _innerState.type == "Auto" ? true : false; }
        }
    }
}

using Firesec.Metadata;

namespace FiresecClient.Models
{
    public class Parameter
    {
        configDrvParamInfo _parameter;

        public Parameter(configDrvParamInfo parameter)
        {
            _parameter = parameter;
        }

        public string Value { get; set; }

        public string Name
        {
            get { return _parameter.name; }
        }

        public string Caption
        {
            get { return _parameter.caption; }
        }

        public bool Visible
        {
            get { return ((_parameter.hidden == "0") && (_parameter.showOnlyInState == "0")); }
        }
    }
}

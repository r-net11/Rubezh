using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Data
{
    [Serializable]
    public class FormData
    {
        public UserControlCollection userControlCollection { get; set; }
        public DataSource dataSource { get; set; }
        public BindingManager bindingManager { get; set; }
        public EventBindingManager eventBindingManager { get; set; }
    }
}

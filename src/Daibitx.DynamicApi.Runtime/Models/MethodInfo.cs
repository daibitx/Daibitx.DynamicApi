using System;
using System.Collections.Generic;
using System.Text;

namespace Daibitx.DynamicApi.Runtime.Models
{
    public class MethodInfo
    {
        public string Name { get; set; }

        public string ReturnType { get; set; }
        public string HttpMethod { get; set; }
        public List<ParameterInfo> Parameters { get; set; } = new List<ParameterInfo>();
    }

    public class ParameterInfo
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string BindingSource { get; set; }
        public bool IsOptional { get; set; }
        public string DefaultValue { get; set; }
    }


}

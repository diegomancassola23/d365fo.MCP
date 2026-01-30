using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace d365fo.MCP.Tools
{
    public class ToolInputBase
    {
        public bool Validate(out string validationError)
        {
            bool ret = true;
            validationError = string.Empty;
            return ret;
        }
    }
}

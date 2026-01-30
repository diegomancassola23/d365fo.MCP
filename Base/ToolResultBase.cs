using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace d365fo.MCP.Base
{
    public class ToolResultBase
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
    }
}

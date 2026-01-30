using d365fo.MCP.Tools;

using Microsoft.IdentityModel.Tokens;

using ModelContextProtocol.Server;

using System.ComponentModel;
using System.Configuration;
using System.Security.AccessControl;
using System.Xml;

namespace d365fo.MCP
{

    [McpServerToolType]
    public static class MCPGetOrCreateLabelTool
    {
        [McpServerTool, Description("Creates a label for a list of specific languages, using a provided label name and Text from a JSON input object. Returns a JSON with Success and ErrorMessage.")]
        public static GetOrCreateLabelToolResult[] GetOrCreateLabel(GetOrCreateLabelToolInput[] input)
        {
            return GetOrCreateLabelTool.Execute(input);
        }
    }
}


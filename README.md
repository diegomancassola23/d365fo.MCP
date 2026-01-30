# d365fo.MCP

🚀 MCP for X++ Development Automation
This repository contains an MCP (Model Context Protocol) project designed to simplify and automate common tasks in X++ development for Dynamics 365 Finance & Operations.

🧩 Current Features
* Automated creation of labels for X++ development using a consistent naming and structure

🔮 Roadmap / Future Vision
Planned extensions may include (but are not limited to):
Additional X++ development utilities
Code generation helpers
Best-practice enforcement
Other productivity tools for D365FO developers

⚙️ Installation & Configuration
1️⃣ Clone the repository
2️⃣ Add an .mcp.json file and register the MCP server, for example:

"d365fo.MCP": {
  "type": "stdio",
  "command": "dotnet",
  "args": [
    "run",
    "--project",
    "J:\\d365fo.MCP\\d365fo.MCP.csproj"
  ],
  "env": {}
}

Official Microsoft documentation on MCP servers (Visual Studio): 👉 https://learn.microsoft.com/en-us/visualstudio/ide/mcp-servers?view=visualstudio

3️⃣ Configure app.config
Set the required parameters according to your environment:
<add key="model" value="ALTC"/>
<add key="labelFileName" value="ALTCLBL"/>
<add key="labelsModel" value="ALTC"/>
<add key="aosServicePath" value="J:\AosService\PackagesLocalDirectory"/>
<add key="labelPrefix" value=""/>

Where you can define:
* The main model name
* A dedicated model for labels (if different)
* The PackagesLocalDirectory path
* An optional label prefix to follow project conventions
$$$ → external prefix placeholder Useful when the prefix must be provided dynamically (e.g. per feature, module, or context)
### → auto-increment placeholder Automatically resolves the next available numeric suffix by scanning existing labels
This allows prefixes like: TASK$$$_###

🎯 Purpose
This project aims to provide a modular and extensible MCP that can grow over time, adapting to real-world development needs and contributing to a more efficient Dynamics 365 Finance & Operations development experience.

🤝 Contributions
This solution originates as an extracted and simplified version of an internal tool, adapted and shared to contribute back to the community.
For any questions or additional information, feel free to reach out to me privately.
Ideas, feedback, and contributions are welcome.
The project is in its early stages, and community input will help shape its future direction.

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

```
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
```

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

  1. $$$ → external prefix placeholder Useful when the prefix must be provided dynamically (e.g. per feature, module, or context)
  2. \### → auto-increment placeholder Automatically resolves the next available numeric suffix by scanning existing labels
  
This allows prefixes like: TASK$$$_###

🤖 Copilot Instructions (Visual Studio 2022)

Using Copilot instructions is not mandatory, but strongly recommended.
Without explicit instructions, Copilot may still work, but its behavior can vary.
By providing instructions, label creation becomes predictable, repeatable, and aligned with project conventions.

* Copilot understands when and how to call the MCP tool
* Label creation follows project conventions automatically
* Common mistakes (missing translations, wrong primary language, duplicated labels) are avoided
* The workflow becomes repeatable and consistent across the team

How to install
Create a file named:
* copilot-instructions.md
* Add it to your Visual Studio 2022 solution as a Solution Item
(Right click on the solution → Add → New Item → Text File, or Add Existing Item).

🧪 Usage examples with Copilot

This section shows how the MCP behaves when used together with Copilot instructions or not.
The same MCP tool is used in both cases; what changes is how Copilot interprets the request based on the provided instructions.
Example 1 – Explicit whitout using instructions

```
User request:
Create label using MCP tool "this is my label" and translate it into these languages: IT, DE, ES
```

Example 2 – Simple request using instructions

```
User request:
Create label "this is my label"
```

🎯 Purpose

This project aims to provide a modular and extensible MCP that can grow over time, adapting to real-world development needs and contributing to a more efficient Dynamics 365 Finance & Operations development experience.

🤝 Contributions

This solution originates as an extracted and simplified version of an internal tool, adapted and shared to contribute back to the community.
For any questions or additional information, feel free to reach out to me privately.
Ideas, feedback, and contributions are welcome.
The project is in its early stages, and community input will help shape its future direction.

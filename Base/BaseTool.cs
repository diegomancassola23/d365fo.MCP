using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Security.AccessControl;
using System.Xml;

namespace d365fo.MCP.Base
{
    public abstract class BaseTool
    {
        protected static string GetConfigValue(string key)
        {
            return ConfigurationManager.AppSettings[key]?.Trim() ?? throw new Exception($"Configurazione mancante o vuota per la chiave: {key}");
        }
        public static readonly Dictionary<string, string> ObjectTypeToFolderMap = new Dictionary<string, string>
        {
            { "AxClass", "AxClass" },
            { "AxTable", "AxTable" },
            { "AxForm", "AxForm" },
            { "AxMenuItemAction", "AxMenuItemAction" },
            { "AxMenuItemDisplay", "AxMenuItemDisplay" },
            { "AxMenuItemOutput", "AxMenuItemOutput" },
            { "AxEnum", "AxEnum" },
            { "AxEnumExtension", "AxEnumExtension" },
            { "AxDataEntityView", "AxDataEntityView" },
            { "AxDataEntity", "AxDataEntity" },
            { "AxTableExtension", "AxTableExtension" },
            { "AxFormExtension", "AxFormExtension" },
            { "AxSecurityPrivilege", "AxSecurityPrivilege" },
            { "AxView", "AxView" },
            { "AxTile", "AxTile" }
        };

        public string GetObjectFilePath(string objectName, string objectType, string model)
        {
            if (string.IsNullOrWhiteSpace(objectName))
            {
                throw new ArgumentException("ObjectName is required.", nameof(objectName));
            }

            if (string.IsNullOrWhiteSpace(objectType) || !ObjectTypeToFolderMap.ContainsKey(objectType))
            {
                throw new ArgumentException("Invalid or missing ObjectType.", nameof(objectType));
            }

            if (string.IsNullOrWhiteSpace(model))
            {
                throw new ArgumentException("Invalid or missing Model.", nameof(model));
            }

            string aosServicePath = ConfigurationManager.AppSettings["aosServicePath"];
            if (string.IsNullOrWhiteSpace(aosServicePath) || string.IsNullOrWhiteSpace(model))
            {
                throw new InvalidOperationException("aosServicePath or model is not configured in app.config.");
            }

            string folderName = ObjectTypeToFolderMap[objectType];
            string destDir = Path.Combine(aosServicePath, model, model, folderName);
            return Path.Combine(destDir, objectName + ".xml");
        }

    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace HexBox.WinUI.Demo
{
    public static class AppSettings
    {
        private static JsonSettingsContainer? _localSettings;

        public static JsonSettingsContainer LocalSettings
        {
            get
            {
                if (_localSettings == null)
                {
                    var localFolder = Environment.CurrentDirectory;
                    if (Utils.IsPackaged())
                    {
                        localFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                    }
                    var appFolder = Path.Combine(localFolder, "Data");
                    Directory.CreateDirectory(appFolder);
                    _localSettings = new JsonSettingsContainer(Path.Combine(appFolder, "Settings.json"));
                }
                return _localSettings;
            }
        }
    }

    // 非打包应用的备用实现
    public class JsonSettingsContainer
    {
        private readonly string _filePath;

        private Dictionary<string, object>? _values;

        public Dictionary<string, object>? Values
        {
            get { return _values; }
        }

        public JsonSettingsContainer(string filePath)
        {
            _filePath = filePath;
            _values = File.Exists(filePath)
                ? JsonSerializer.Deserialize<Dictionary<string, object>>(File.ReadAllText(filePath))
                : new();
        }

        public bool TryGetUintValue(string key, out uint v)
        {
            if (Values != null)
            {
                try
                {
                    Values.TryGetValue(key, out var theme);

                    if (theme is JsonElement b)
                    {
                        b.TryGetUInt32(out v);
                        return true;
                    }
                    else
                    {
                        if (theme is uint c)
                        {
                            v = c;
                            return true;
                        }
                    }
                }
                catch { }
            }

            v = 0;
            return false;
        }

        public bool TryGetStringValue(string key, out string v)
        {
            if (Values != null)
            {
                try
                {
                    Values.TryGetValue(key, out var theme);

                    if (theme is JsonElement b)
                    {
                        if (b.GetString() is string c)
                        {
                            v = c;
                            return true;
                        }
                    }
                    else
                    {
                        if (theme is string c)
                        {
                            v = c;
                            return true;
                        }
                    }
                }
                catch { }
            }

            v = "";
            return false;
        }

        public void Save() =>
            File.WriteAllText(_filePath, JsonSerializer.Serialize(_values));
    }
}
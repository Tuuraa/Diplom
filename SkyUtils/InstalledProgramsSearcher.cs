using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SkyUtils
{
    public enum PlatformTypes
    {
        x86,
        amd64
    }
    public class InstalledProgramsSearcher
    {
        [DllImport("advapi32.dll")]
        extern public static int RegQueryInfoKey(
            Microsoft.Win32.SafeHandles.SafeRegistryHandle hkey,
            StringBuilder lpClass,
            ref uint lpcbClass,
            IntPtr lpReserved,
            IntPtr lpcSubKeys,
            IntPtr lpcbMaxSubKeyLen,
            IntPtr lpcbMaxClassLen,
            IntPtr lpcValues,
            IntPtr lpcbMaxValueNameLen,
            IntPtr lpcbMaxValueLen,
            IntPtr lpcbSecurityDescriptor,
            out long lpftLastWriteTime
        );

        public string DisplayName { get; private set; }
        public string UninstallString { get; private set; }
        public string KBNumber { get; private set; }
        public string DisplayIcon { get; private set; }
        public string Version { get; private set; }
        public DateTime InstallDate { get; private set; }
        public PlatformTypes Platform { get; private set; }
        public bool IsSystemComponent { get; private set; }
        public bool IsKB { get { return !string.IsNullOrWhiteSpace(KBNumber); } }

        public static List<InstalledProgramsSearcher> GetAllInstalledPrograms()
        {
            var result = new List<InstalledProgramsSearcher>();

            Action<PlatformTypes, RegistryKey, string> getRegKeysForRegPath = (platform, regBase, path) =>
            {
                using (var baseKey = regBase.OpenSubKey(path))
                {
                    if (baseKey != null)
                    {
                        string[] subKeyNames = baseKey.GetSubKeyNames();
                        foreach (string subkeyName in subKeyNames)
                        {
                            using (var subKey = baseKey.OpenSubKey(subkeyName))
                            {
                                object o;

                                o = subKey.GetValue("DisplayName");
                                string displayName = o != null ? o.ToString() : "";
                                o = subKey.GetValue("UninstallString");
                                string uninstallString = o != null ? o.ToString() : "";
                                o = subKey.GetValue("KBNumber");
                                string kbNumber = o != null ? o.ToString() : "";
                                o = subKey.GetValue("DisplayIcon");
                                string displayIcon = o != null ? o.ToString() : "";
                                o = subKey.GetValue("DisplayVersion");
                                string version = o != null ? o.ToString() : "";
                                o = subKey.GetValue("InstallDate");
                                DateTime installDate = o != null ? parseInstallDate(o.ToString()) : default(DateTime);
                                o = subKey.GetValue("SystemComponent");
                                bool isSystemComponent = o != null ? o.ToString() == "1" : false;

                                // Sometimes, you need to get the KB number another way.
                                if (kbNumber == "")
                                {
                                    var match = Regex.Match(displayName, @".*?\((KB\d+?)\).*");
                                    if (match.Success) kbNumber = match.Groups[1].ToString();
                                }

                                // Sometimes, the only way you can get install date is from the last write
                                // time on the registry key.
                                if (installDate == default(DateTime))
                                {
                                    string keyFull = baseKey + "\\" + subkeyName + "\\DisplayVersion";
                                    var sb = new StringBuilder(64);
                                    uint sbLen = 65;

                                    RegQueryInfoKey(
                                            subKey.Handle
                                            , sb
                                            , ref sbLen
                                            , IntPtr.Zero
                                            , IntPtr.Zero
                                            , IntPtr.Zero
                                            , IntPtr.Zero
                                            , IntPtr.Zero
                                            , IntPtr.Zero
                                            , IntPtr.Zero
                                            , IntPtr.Zero
                                            , out long lastWriteTime);

                                    installDate = DateTime.FromFileTime(lastWriteTime);
                                }

                                if (displayName != "" && uninstallString != "")
                                {
                                    result.Add(new InstalledProgramsSearcher
                                    {
                                        DisplayName = displayName,
                                        UninstallString = uninstallString,
                                        KBNumber = kbNumber,
                                        DisplayIcon = displayIcon,
                                        Version = version,
                                        InstallDate = installDate,
                                        Platform = platform,
                                        IsSystemComponent = isSystemComponent
                                    });
                                }
                            }
                        }
                    }
                }
            };

            getRegKeysForRegPath(PlatformTypes.amd64, Registry.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");
            getRegKeysForRegPath(PlatformTypes.amd64, Registry.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");
            if (Environment.Is64BitOperatingSystem)
            {
                getRegKeysForRegPath(PlatformTypes.x86, Registry.LocalMachine, @"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall");
                getRegKeysForRegPath(PlatformTypes.x86, Registry.CurrentUser, @"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall");
            }

            return result;
        }

        public static void SaveToJsonFile(string filePath, List<InstalledProgramsSearcher> programsList)
        {
            // Сериализация списка программ в JSON формат
            var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(programsList, jsonOptions);

            // Запись JSON строки в файл
            File.WriteAllText(filePath, jsonString);
        }
        // MsiExec.exe
        public static List<InstalledProgramsSearcher> GetFilteredPrograms(List<InstalledProgramsSearcher> list, List<string> filter)
        {
            List<InstalledProgramsSearcher> filtered = new List<InstalledProgramsSearcher>();

            foreach (InstalledProgramsSearcher item in list)
            {
                if (item.UninstallString.ToLower().Contains("MsiExec.exe".ToLower())) continue;
                filtered.Add(item);
            }

            List<InstalledProgramsSearcher> filtered2 = new List<InstalledProgramsSearcher>();

            foreach (InstalledProgramsSearcher item in filtered)
            {
                bool isNeeded = true;
                for (int i = 0; i < filter.Count; i++)
                {
                    isNeeded = true;
                    if (item.DisplayName.ToLower().Contains(filter[i].ToLower()))
                    {
                        isNeeded = false;
                        break;
                    }
                }
                if (isNeeded) filtered2.Add(item);
            }

            return filtered2;
        }

        public string Dump()
        {
            return Platform + "\t" + DisplayName + "\t" + InstallDate + "\t" + DisplayIcon + "\t" + Version + "\t" + KBNumber + "\t" + UninstallString;
        }

        private static DateTime parseInstallDate(string installDateStr)
        {
            DateTime.TryParseExact(
                    installDateStr
                    , format: "yyyyMMdd"
                    , provider: new System.Globalization.CultureInfo("en-US")
                    , style: System.Globalization.DateTimeStyles.None
                    , result: out DateTime result);

            return result;
        }

        public override string ToString()
        {
            return DisplayName;
        }

        public static void SaveInstalledProgramsToJson(List<InstalledProgram> programs, string filePath)
        {
            try
            {
                var json = JsonSerializer.Serialize(programs, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(filePath, json);
                Console.WriteLine("Данные успешно сохранены в файл.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при сохранении в JSON: {ex.Message}");
            }
        }

        public static List<InstalledProgram> LoadInstalledProgramsFromJson(string filePath)
        {
            try
            {
                var json = File.ReadAllText(filePath);
                var programs = JsonSerializer.Deserialize<List<InstalledProgram>>(json);
                return programs ?? new List<InstalledProgram>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при загрузке из JSON: {ex.Message}");
                return new List<InstalledProgram>();
            }
        }
    }

}


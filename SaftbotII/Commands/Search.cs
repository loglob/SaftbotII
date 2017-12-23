using SaftbotII.DatabaseSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml;

namespace SaftbotII.Commands
{
    internal static class SearchCommand
    {
        [Command("Searches for the given string on a given provider", "[<Search provider>] <Search string>")]
        public static async void Search(CommandInformation cmdinfo)
        {
            if (!SearchProvider.Initialized)
                await SearchProvider.Initialize();

            bool ignoreFirst = false;

            SearchProvider toUse = SearchProvider.All[(cmdinfo.ServerEntry[ServerSettings.UseGoogle])?"g":"ddg"];

            if(cmdinfo.arguments.Length > 0 && cmdinfo.arguments[0][0] == '-')
            {
                string toFind = cmdinfo.arguments[0].Substring(1).ToLower();
                ignoreFirst = true;

                if (toFind == "list")
                {
                    string msg = "Available providers are:\n";

                    foreach (var sp in SearchProvider.All)
                        msg += $"\t-{sp.Key}: {sp.Value.Description}\n";

                    await cmdinfo.messages.Send(msg);
                    return;
                }

                if (SearchProvider.All.ContainsKey(toFind))
                    toUse = SearchProvider.All[toFind];
                else
                    ignoreFirst = false;
            }

            string toSearch = String.Join(toUse.SpaceEscape, Util.SubArray(cmdinfo.arguments, ignoreFirst ? 1 : 0));

            await cmdinfo.messages.Send($"{toUse.BaseURL}{toSearch}");
        }
    }

    struct SearchProvider
    {
        public static Dictionary<string, SearchProvider> All = new Dictionary<string, SearchProvider>();
        const string Path = "./SearchProviders.xml";
        const string LoggingPrefix = "[SearchProviderReader]";
        public static bool Initialized = false;

        public static async Task Initialize()
        {
            if (!File.Exists(Path))
                File.Create(Path);

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(Path);

            var providerList = xmlDoc.GetElementsByTagName("Provider");
            await Log.Enter($"{LoggingPrefix} Found {providerList.Count} Search providers to read"); 

            for (int i = 0; i < providerList.Count; i++)
            {
                try
                {
                    var prov = providerList.Item(i);
                    SearchProvider sp = new SearchProvider();

                    sp.Shorthand = prov.Attributes["Shorthand"].InnerText;
                    sp.Description = prov.Attributes["Description"].InnerText;

                    var searchURL = prov["SearchURL"];
                    sp.BaseURL = searchURL.Attributes["BaseURL"].InnerText;

                    if (searchURL.Attributes["SpaceEscape"] != null)
                        sp.SpaceEscape = searchURL.Attributes["SpaceEscape"].InnerText;
                    else
                        sp.SpaceEscape = "+";

                    All.Add(sp.Shorthand.ToLower(), sp);
                }
                catch
                {
                    await Log.Enter($"{LoggingPrefix} Failed to parse {i}th search provider");
                }
            }

            Initialized = true;
            await Log.Enter($"{LoggingPrefix} Loaded {All.Count} search providers");
        }

        public string SpaceEscape;
        public string BaseURL;
        public string Description;
        public string Shorthand;
    }
}

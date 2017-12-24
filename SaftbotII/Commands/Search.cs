using SaftbotII.DatabaseSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml;

namespace SaftbotII.Commands
{
    /// <summary>
    /// Implements search command
    /// </summary>
    internal static class SearchCommand
    {
        [Command("Searches for the given string on a given provider", "[<Search provider>] <Search string>")]
        public static async void Search(CommandInformation cmdinfo)
        {
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

    /// <summary>
    /// Contains the information required to search with a servive
    /// </summary>
    struct SearchProvider
    {
        /// <summary>
        /// All registeres SearchProviders
        /// </summary>
        public static Dictionary<string, SearchProvider> All = new Dictionary<string, SearchProvider>();

        /// <summary>
        /// The file all known providers are stored in
        /// See the default SearchProviders.xml for the expected formatting
        /// </summary>
        const string Path = "./SearchProviders.xml";

        /// <summary>
        /// The Prefix prepended to all debugging messages from this module
        /// </summary>
        const string LoggingPrefix = "[SearchProviderReader]";

        /// <summary>
        /// Loads and reads the known search provider XML file
        /// Needs to be called before the search comamnd becomes usable
        /// </summary>
        public static void Initialize()
        {
            if (!File.Exists(Path))
                File.Create(Path);

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(Path);

            var providerList = xmlDoc.GetElementsByTagName("Provider");
            Log.Enter($"{LoggingPrefix} Found {providerList.Count} Search providers to read"); 

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
                    Log.Enter($"{LoggingPrefix} Failed to parse {i}th search provider");
                }
            }
            
            Log.Enter($"{LoggingPrefix} Loaded {All.Count} search providers");
        }

        /// <summary>
        /// What string is used to escape whitespace
        /// </summary>
        public string SpaceEscape;

        /// <summary>
        /// The URL after which the escaped search string is added
        /// </summary>
        public string BaseURL;

        /// <summary>
        /// Which service is searched with this provider
        /// </summary>
        public string Description;

        /// <summary>
        /// The shorthand used with search to select this provider
        /// </summary>
        public string Shorthand;
    }
}

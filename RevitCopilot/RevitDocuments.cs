using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Reflection;

namespace RevitCopilot
{
    public static partial class RevitDocuments
    {
		public static Document Doc { get; private set; }

		public static Application App { get; private set; }

		public static UIDocument UiDoc { get; private set; }

		public static UIApplication UiApp { get; private set; }

        public static bool SetRevitDocuments( Document doc )
		{
			if ( doc == null ) return false;
			Doc = doc;
			App = Doc.Application;
			UiDoc = new UIDocument( Doc );
			UiApp = new UIApplication( App );
			return true;
		}

        private static string GetApikey()
        {
            var dllPath = Assembly.GetExecutingAssembly().Location;
            var dllFolder = Path.GetDirectoryName(dllPath);
            var resourcesFolder = Path.Combine(dllFolder, "Resources");
            var apikeyPath = Path.Combine(resourcesFolder, "Apikey.json");
            string json = File.ReadAllText(apikeyPath);
            JObject jsonObj = JObject.Parse(json);
            return jsonObj["Apikey"].ToString();
        }
    }
}

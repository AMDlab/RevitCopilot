using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

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
    }
}

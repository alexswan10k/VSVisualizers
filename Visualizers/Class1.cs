using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using Microsoft.VisualStudio.DebuggerVisualizers;
using ServiceStack.Text;
using Visualizers;

[assembly: System.Diagnostics.DebuggerVisualizer(typeof(SomeDebugVisualizer), typeof(VisualizerObjectSource), Target = typeof(System.String), Description = "Pointless Visualizer")]
[assembly: System.Diagnostics.DebuggerVisualizer(typeof(SomeDebugVisualizer), typeof(VisualizerObjectSource), Target = typeof(IEnumerable<>), Description = "Pointless Visualizer")]
[assembly: System.Diagnostics.DebuggerVisualizer(typeof(NameValueCollectionVisualizer), typeof(VisualizerObjectSource), Target = typeof(NameValueCollection), Description = "NVC viz")]
//[assembly: System.Diagnostics.DebuggerVisualizer(typeof(ObjectDump), typeof(VisualizerObjectSource), Target = , Description = "JSON dump viz")]

[assembly: System.Diagnostics.DebuggerVisualizer(typeof(XDocVisualizer), typeof(XDocVisualizerObjectSource), Target = typeof(XDocument), Description = "XDocViz")]
[assembly: System.Diagnostics.DebuggerVisualizer(typeof(XDocVisualizer), typeof(XDocVisualizerObjectSource), Target = typeof(XElement), Description = "XDocViz")]

[assembly: System.Diagnostics.DebuggerVisualizer(typeof(XDocVisualizer), typeof(XDocVisualizerObjectSource), Target = typeof(XContainer), Description = "XDocViz")]

[assembly: System.Diagnostics.DebuggerVisualizer(typeof(XmlDocVisualizer), typeof(XmlDocVisualizerObjectSource), Target = typeof(XmlDocument), Description = "XmlDocViz")]
[assembly: System.Diagnostics.DebuggerVisualizer(typeof(StringXDocVisualizer), typeof(VisualizerObjectSource), Target = typeof(string), Description = "StringXDocViz")]


namespace Visualizers
{
	public class SomeDebugVisualizer :  DialogDebuggerVisualizer
    {
		protected override void Show(IDialogVisualizerService windowService, IVisualizerObjectProvider objectProvider)
		{
			MessageBox.Show(objectProvider.GetObject().ToString());
		}
    }

	public class NameValueCollectionVisualizer: DialogDebuggerVisualizer
	{
		protected override void Show(IDialogVisualizerService windowService, IVisualizerObjectProvider objectProvider)
		{
			var theObj = (NameValueCollection)objectProvider.GetObject();

			var dict = new Dictionary<string, string>();

			foreach(var key in theObj.AllKeys)
			{
				var value = theObj[key];
				dict.Add(key, value);
			}

			MessageBox.Show(dict.Dump() + theObj.Count);
		}
	}

	public class ObjectDump : DialogDebuggerVisualizer
	{
		protected override void Show(IDialogVisualizerService windowService, IVisualizerObjectProvider objectProvider)
		{
			var theObj = objectProvider.GetObject();
			try
			{
				MessageBox.Show(theObj.Dump());
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}
	}
}



using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Visualizers.ViewModels;
using Visualizers.Views;

namespace Visualizers.Harness
{
	class Program
	{
		[STAThread]
// ReSharper disable InconsistentNaming
		static void Main(string[] args)
// ReSharper restore InconsistentNaming
		{
			//XdocTest();
			//XdocCompareTest();
			XDocFormatter();
		}

		private static void XDocFormatter()
		{
			var win = new XDocPasteView();
			win.ShowDialog();
		}

		private static void XdocTest()
		{
			var win = new XDocVisualizerView();
			var xDocument = new XDocument();
			xDocument.Add(new XElement("Root"));
			xDocument.Root.Add(new XElement("someElement"), new XElement("someOtherElement"));

			win.DataContext = new XDocViewModel() { Document = xDocument };

			win.ShowDialog();
		}

		private static void XdocCompareTest()
		{
			var win = new XDocCompareView();
			//var xDocument = new XDocument();
			//xDocument.Add(new XElement("Root"));
			//xDocument.Root.Add(new XElement("someElement"), new XElement("someOtherElement"));
						
			var xml = File.ReadAllText(@"..\..\Test.xml");
			var theObjXdoc = XDocument.Parse(xml);
			win.DataContext = new XDocCompareViewModel(new XDocViewModel(theObjXdoc), new XDocViewModel(theObjXdoc));

			win.ShowDialog();
		}

		private static void XdocFromTextTest()
		{
			var xml = File.ReadAllText(@"..\..\Test.xml");

			var theObjXdoc = XDocument.Parse(xml);

			var win = new XDocVisualizerView();
			win.DataContext = new XDocViewModel() { Document = theObjXdoc };

			win.ShowDialog();
		}
	}
}

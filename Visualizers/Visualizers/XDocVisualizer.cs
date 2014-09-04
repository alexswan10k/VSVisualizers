using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Xml;
using System.Xml.Linq;
using Microsoft.VisualStudio.DebuggerVisualizers;
using Visualizers.Helpers;
using Visualizers.ViewModels;
using Visualizers.Views;

namespace Visualizers
{
	public static class Statics
	{
		public static bool HasLoadedWpfAssembly;

		public static void Initialize()
		{
			if (!HasLoadedWpfAssembly)
			{
				Assembly.Load("System.Windows.Interactivity");
				HasLoadedWpfAssembly = true;
			}
		}
	}

	public class XDocVisualizer : DialogDebuggerVisualizer
	{
		protected override void Show(IDialogVisualizerService windowService, IVisualizerObjectProvider objectProvider)
		{
			Statics.Initialize();

			var xml = new StreamReader(objectProvider.GetData()).ReadToEnd();

			var theObj = XDocument.Parse(xml);

			{
				var win = new XDocVisualizerView();
				win.DataContext = new XDocViewModel() { Document = theObj };

				win.ShowDialog();
			}
		}
	}

	public class XDocVisualizerObjectSource : VisualizerObjectSource
	{
		public override void GetData(object target, Stream outgoingData)
		{
			var xml = (target as XContainer).ToString();
			var writer = new StreamWriter(outgoingData);
			writer.Write(xml);
			writer.Flush();
		}
	}

	public class XmlDocVisualizer : DialogDebuggerVisualizer
	{
		protected override void Show(IDialogVisualizerService windowService, IVisualizerObjectProvider objectProvider)
		{
			Statics.Initialize();

			var xml = new StreamReader(objectProvider.GetData()).ReadToEnd();
			try
			{
				var theObj = XDocument.Parse(xml);
				{
					var theObjXdoc = theObj;

					var win = new XDocVisualizerView();
					win.DataContext = new XDocViewModel() { Document = theObjXdoc };

					win.ShowDialog();
				}
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message + " xml - " + xml);
			}
		}
	}

	public class XmlDocVisualizerObjectSource : VisualizerObjectSource
	{
		public override void GetData(object target, Stream outgoingData)
		{
			var xmlDoc = (target as XmlDocument);
			using (var stringWriter = new StringWriter())
			using (var xmlTextWriter = XmlWriter.Create(stringWriter))
			{
				xmlDoc.WriteTo(xmlTextWriter);
				xmlTextWriter.Flush();
				var xml = stringWriter.GetStringBuilder().ToString();
				var writer = new StreamWriter(outgoingData);
				writer.Write(xml);
				writer.Flush();
			}
		}
	}

	public class StringXDocVisualizer : DialogDebuggerVisualizer
	{
		protected override void Show(IDialogVisualizerService windowService, IVisualizerObjectProvider objectProvider)
		{
			Statics.Initialize();

			var xml = objectProvider.GetObject() as string;
			if (xml != null)
			{
				XDocument theObjXdoc;
				try
				{
					theObjXdoc= XDocument.Parse(xml);
				}
				catch(Exception ex)
				{
					xml = "<Root>" + xml + "</Root>";
					theObjXdoc = XDocument.Parse(xml);
				}

				var win = new XDocVisualizerView();
				win.DataContext = new XDocViewModel() { Document = theObjXdoc };

				win.ShowDialog();
			}
		}
	}
}
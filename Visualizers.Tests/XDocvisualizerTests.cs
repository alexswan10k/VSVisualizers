using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Xml.Linq;
using NUnit.Framework;
using Visualizers.ViewModels;
using Visualizers.Views;

namespace Visualizers.Tests
{
	[TestFixture]
	class XDocvisualizerTests : IDisposable
	{
		private Dispatcher _dispatcher;

		public XDocvisualizerTests()
		{
			var thread = new Thread(() =>{});
			thread.SetApartmentState(ApartmentState.STA);
			thread.Start();
			_dispatcher = Dispatcher.FromThread(thread);
		}


		[Test]
		public void Test1()
		{
			_dispatcher.Invoke(() =>
			{
				var win = new XDocVisualizerView();
				var xDocument = new XDocument();
				xDocument.Root.Add(new XElement("someElement"), new XElement("someOtherElement"));

				win.DataContext = new XDocViewModel() { Document = xDocument };

				win.ShowDialog();
			});
		}

		public void Dispose()
		{
			_dispatcher.Thread.Abort();
		}
	}
}

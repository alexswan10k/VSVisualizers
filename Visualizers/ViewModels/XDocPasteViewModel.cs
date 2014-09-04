using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Input;
using System.Xml.Linq;
using Visualizers.Annotations;
using Visualizers.Helpers;
using Visualizers.Views;

namespace Visualizers.ViewModels
{
	public class XDocPasteViewModel : INotifyPropertyChanged
	{
		private string _doc1;
		private string _doc2;
		private bool _htmlDecode;

		public string Doc1
		{
			get { return _doc1; }
			set
			{
				if (value == _doc1)
					return;
				_doc1 = value;
				OnPropertyChanged();
			}
		}

		public string Doc2
		{
			get { return _doc2; }
			set
			{
				if (value == _doc2)
					return;
				_doc2 = value;
				OnPropertyChanged();
			}
		}

		public ICommand Compare{get
		{
			return new DelegateCommand(o =>
			{
				var win = new XDocCompareView();

				if (!string.IsNullOrEmpty(Doc1) && !string.IsNullOrEmpty(Doc2))
				{
					try
					{
						var objXDoc1 = XDocument.Parse(_Preprocess(Doc1));
						var objXDoc2 = XDocument.Parse(_Preprocess(Doc2));
						win.DataContext = new XDocCompareViewModel(new XDocViewModel(objXDoc1), new XDocViewModel(objXDoc2));

						win.ShowDialog();
					}catch(Exception ex)
					{
						MessageBox.Show(ex.Message);
					}
				}
			});
		}}

		public bool HtmlDecode
		{
			get { return _htmlDecode; }
			set
			{
				if (value == _htmlDecode)
					return;
				_htmlDecode = value;
				OnPropertyChanged();
			}
		}

		public bool DestroyNamespaces
		{
			get { return _destroyNamespaces; }
			set
			{
				if (value.Equals(_destroyNamespaces))
					return;
				_destroyNamespaces = value;
				OnPropertyChanged();
			}
		}

		private bool _destroyNamespaces;


		private string _Preprocess(string doc1)
		{
			var sb = new StringBuilder(doc1);

			if (HtmlDecode)
			{
				//hack
				doc1 = HttpUtility.HtmlDecode(doc1).Replace("&", "");
			}

			if (DestroyNamespaces)
				doc1 = _DestroyNamespaces(doc1);

			//this horrible expression actually skips self closing input tags and matches the first close tag as group
			var regex = new Regex(@"<[Ii]nput[=\""\w\s]+(>).?(</\w+>)?");
			var matches = regex.Matches(doc1);

			int added = 0;

			foreach(Match match in matches)
			{
				if (match.Groups.Count > 1)
				{
					if (match.Groups.Count == 3)
					{
						var group = match.Groups.Cast<Group>().LastOrDefault();

						if (Regex.IsMatch(group.Value, "[Ii]nput"))
							continue;
					}

					//insert self closing elem

					var endTag = match.Groups[1];
					var stringToAdd = " /";
					doc1 = doc1.Insert(endTag.Index + added, stringToAdd);
					added += stringToAdd.Length;
				}
			}

			return doc1;
		}

		private string _DestroyNamespaces(string doc1)
		{
			var str = Regex.Replace(doc1,@"<\w+:" , "<");
			str = Regex.Replace(str, @"</\w+:", "</");	//remove nodes with xml namespace prefix
			str = Regex.Replace(str, @"xmlns=""[\w://\.]+""", "");	//remove xml namespaces

			return str;
		}

		public ICommand Use
		{
			get
			{
				return new DelegateCommand(o =>
					{
						try
						{
							var xdocVizView = new XDocVisualizerView();
							xdocVizView.DataContext = new XDocViewModel(XDocument.Parse(_Preprocess(Doc1)));
							xdocVizView.Show();
						}
						catch(Exception ex)
						{
							MessageBox.Show(ex.Message);
						}
					});
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			var handler = PropertyChanged;
			if (handler != null)
				handler(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}

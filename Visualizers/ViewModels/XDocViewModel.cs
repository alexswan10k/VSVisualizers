using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Visualizers.Annotations;
using Visualizers.Helpers;

namespace Visualizers.ViewModels
{
	public class XDocViewModel : INotifyPropertyChanged, IViewModelWithNodes
	{
		private XDocument _document;
		private XElementViewModel _selectedNode;
		private Lazy<IEnumerable<XElementViewModel>> _xElementViewModels;
		private string _xPathFilter;
		private readonly XmlNamespaceManager _xmlNamespaceManager = new XmlNamespaceManager(new NameTable());

		public XDocViewModel()
		{
			_xElementViewModels = new Lazy<IEnumerable<XElementViewModel>>(() => new XElementViewModel[]{ new XElementViewModel(Document.Root) });
		}

		public XDocViewModel(XDocument document)
			:this()
		{
			_document = document;
			var namespaces = _document.CreateNavigator().GetNamespacesInScope(XmlNamespaceScope.All);

			foreach (var ns in namespaces)
			{
				_xmlNamespaceManager.AddNamespace(ns.Key, ns.Value);
			}
		}

		public XDocument Document
		{
			get { return _document; }
			set
			{
				if (Equals(value, _document))
					return;
				_document = value;
				_OnPropertyChanged();
				_OnPropertyChanged("Nodes");
			}
		}

		private IEnumerable<XElementViewModel> _filteredNodes = null;

		public IEnumerable<XElementViewModel> Nodes
		{
			get
			{
				if (_filteredNodes != null)
					return _filteredNodes;

				if (Document == null)
					return null;

				return _xElementViewModels.Value;
			}
		}

		public XElementViewModel SelectedNode
		{
			get { return _selectedNode; }
			set
			{
				if (Equals(value, _selectedNode))
					return;
				_selectedNode = value;
				_OnPropertyChanged();
			}
		}

		public string XPathFilter
		{
			get { return _xPathFilter; }
			set
			{
				if (value == _xPathFilter)
					return;
				_xPathFilter = value;
				_OnPropertyChanged();
			}
		}

		public ICommand ExecuteXPath
		{
			get
			{
				return new DelegateCommand(o =>
				{
					try
					{
						if (!string.IsNullOrEmpty(XPathFilter))
						{
							_filteredNodes = Document.XPathSelectElements(XPathFilter, _xmlNamespaceManager).Select(s => new XElementViewModel(s)).ToList();
						}
						else
							_filteredNodes = null;
					}
					catch(Exception ex)
					{
						MessageBox.Show(ex.Message);
					}

					_OnPropertyChanged("Nodes");
					_OnPropertyChanged("SelectedNode");
				});
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void _OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			var handler = PropertyChanged;
			if (handler != null)
				handler(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}

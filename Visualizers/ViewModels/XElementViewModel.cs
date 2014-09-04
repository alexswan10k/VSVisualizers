using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using Visualizers.Annotations;
using Visualizers.Helpers;

namespace Visualizers.ViewModels
{
	public class XElementViewModel : INotifyPropertyChanged, IViewModelWithNodes
	{
		private readonly XElement _xElement;
		private readonly Lazy<IEnumerable<XElementViewModel>> _xElementViewModels;
		private bool _isExpanded;
		private XElementViewModel _pairedNode;
		private bool _pairedIsEqual;

		public XElementViewModel(XElement xElement)
		{
			_xElement = xElement;
			_xElementViewModels = new Lazy<IEnumerable<XElementViewModel>>(() => _xElement.Elements().Select(s => new XElementViewModel(s)).ToArray());
		}

		public IEnumerable<XElementViewModel> Nodes
		{
			get
			{
				return _xElementViewModels.Value;
			}
		}

		public string NodeCount
		{
			get
			{
				var count = _xElementViewModels.Value.Count();
				if (count > 0)
					return "("+ count +")";
				return string.Empty;
			}
		}


		public string FormattedXml { get { return _xElement.ToString(); } }
		public XElement Element { get { return _xElement; } }
		public IEnumerable<XAttribute> Attributes { get { return _xElement.Attributes(); } }

		public bool IsExpanded
		{
			get { return _isExpanded; }
			set
			{
				if (value.Equals(_isExpanded))
					return;
				_isExpanded = value;
				_OnPropertyChanged();
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

		public string XPath
		{
			get
			{
				return this.Element.GetAbsoluteXPath();
			}
		}

		public XElementViewModel PairedNode
		{
			get { return _pairedNode; }
			set
			{
				if (Equals(value, _pairedNode))
					return;
				if (_pairedNode != null)
					_pairedNode.PropertyChanged -= _PairedNodePropertyChanged;
				_pairedNode = value;
				_pairedNode.PropertyChanged += _PairedNodePropertyChanged;

				_OnPropertyChanged();
				_OnPropertyChanged("IsPaired");
			}
		}

		public bool IsPaired
		{
			get
			{
				return PairedNode != null;
			}
		}

		public bool PairedIsEqual
		{
			get { return _pairedIsEqual; }
			set
			{
				if (value.Equals(_pairedIsEqual))
					return;
				_pairedIsEqual = value;
				_OnPropertyChanged();
			}
		}

		void _PairedNodePropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if(e.PropertyName == "IsExpanded")
			{
				var xElementViewModel = (sender as XElementViewModel);
				if (xElementViewModel != null)
					this.IsExpanded = xElementViewModel.IsExpanded;
			}
		}
	}

	public interface IViewModelWithNodes
	{
		IEnumerable<XElementViewModel> Nodes { get; }
	}
}
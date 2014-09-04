using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Linq;
using Visualizers.Annotations;
using Visualizers.Helpers;

namespace Visualizers.ViewModels
{
	public class XDocCompareViewModel : INotifyPropertyChanged
	{
		public XDocViewModel DocVmA { get; set; }
		public XDocViewModel DocVmB { get;set; }

		readonly Dictionary<XElementViewModel, XElementViewModel> _elementPairs = new Dictionary<XElementViewModel, XElementViewModel>(); 

		public XDocCompareViewModel()
		{
			
		}

		public XDocCompareViewModel(XDocViewModel docVmA, XDocViewModel docVmB)
		{
			DocVmA = docVmA;
			DocVmB = docVmB;

			Derive(docVmA, docVmB);

			var recursePreventer = new RecursePreventer();

			docVmA.PropertyChanged += (sender, args) =>
			{
				if (recursePreventer.TryUpdate())
				{
					var docA = docVmA;
					var docB = docVmB;

					var propertyName = args.PropertyName;

					UpdatePairSelectedNode(propertyName, docA, docB);
					recursePreventer.EndUpdate();
				}
			};

			docVmB.PropertyChanged += (sender, args) =>
			{
				if (recursePreventer.TryUpdate())
				{
					var docA = docVmB;
					var docB = docVmA;

					var propertyName = args.PropertyName;

					UpdatePairSelectedNode(propertyName, docA, docB);
					recursePreventer.EndUpdate();
				}
			};	
		}

		private static void UpdatePairSelectedNode(string propertyName, XDocViewModel docA, XDocViewModel docB)
		{
			if (propertyName == "SelectedNode")
			{
				if (docA.SelectedNode != null && docA.SelectedNode.PairedNode != null)
					docB.SelectedNode = docA.SelectedNode.PairedNode;
			}
		}

		public void Derive(IViewModelWithNodes vmA, IViewModelWithNodes vmB)
		{
			foreach (var nodeA in vmA.Nodes)
			{
				foreach (var nodeB in vmB.Nodes)
				{
					if(_TryPair(nodeA, nodeB))
						Derive(nodeA, nodeB);
				}
			}
		}

		private static bool _TryPair(XElementViewModel nodeA, XElementViewModel nodeB)
		{
			if(nodeA.PairedNode == null && nodeB.PairedNode == null)
				if (nodeA.Element.Name == nodeB.Element.Name)
				{
					nodeA.PairedNode = nodeB;
					nodeB.PairedNode = nodeA;

					bool areEqual = XNode.DeepEquals(nodeA.Element, nodeB.Element);
					nodeA.PairedIsEqual = areEqual;
					nodeB.PairedIsEqual = areEqual;

					return true;
				}
			return false;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void _OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			var handler = PropertyChanged;
			if (handler != null)
				handler(this, new PropertyChangedEventArgs(propertyName));
		}

		public ICommand QueryXPathA{get{return new DelegateCommand(o =>
		{
				
		});}}
	}

	public class RecursePreventer
	{
		bool isUpdating = false;

		public RecursePreventer()
		{
			
		}

		public bool TryUpdate()
		{
			if(!isUpdating)
			{
				isUpdating = true;
				return true;
			}
			return false;
		}

		public void EndUpdate()
		{
			isUpdating=false;
		}
	}


}

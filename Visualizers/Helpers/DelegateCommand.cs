using System;
using System.Windows.Input;

namespace Visualizers.Helpers
{
	public class DelegateCommand : ICommand
	{
		Action<object> _execute;

		public DelegateCommand(Action<object> execute)
		{
			_execute = execute;
		}

		public bool CanExecute(object parameter)
		{
			return true;
		}

		public void Execute(object parameter)
		{
			_execute(parameter);
		}

		public event EventHandler CanExecuteChanged;
	}
}
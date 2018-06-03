using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepeatableTimer
{
    class DelegateCommand
    {
        public Action<object> ExecuteHandler { get; set; }
        public Func<object, bool> CanExecuteHandler { get; set; }

        public bool CanExecute(object parameter)
        {
            var d = CanExecuteHandler;
            return d == null ? true : d(parameter);
        }

        public void Execute(object parameter)
        {
            ExecuteHandler?.Invoke(parameter);
        }

        public event EventHandler CanExecuteChanged;

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, null);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppMeS.ViewModels
{
    public class ForgetPassViewModel : BindableBase, IDialogAware
    {
        public DialogCloseListener RequestClose { get; }

        public bool CanCloseDialog()=>true;
     

        public void OnDialogClosed()
        {
         
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
          
        }

        private DelegateCommand _confirmCommand;
        public DelegateCommand ConfirmCommand =>
            _confirmCommand ?? (_confirmCommand = new DelegateCommand(ExecuteConfirmCommand));

        void ExecuteConfirmCommand()
        {
            RequestClose.Invoke(ButtonResult.Cancel);
        }
    }
}

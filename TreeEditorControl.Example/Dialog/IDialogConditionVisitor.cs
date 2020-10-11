using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeEditorControl.Example.Dialog
{
    public interface IDialogConditionVisitor<out T>
    {
        T VisitPlayerVariableCondition(PlayerVariableCondition node);
    }
}

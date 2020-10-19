using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TreeEditorControl.Example.Dialog.Actions;
using TreeEditorControl.Example.Dialog.Conditions;

namespace TreeEditorControl.Example.Dialog
{
    public interface IDialogConditionVisitor<out T>
    {
        T VisitPlayerVariableCondition(PlayerVariableCondition node);
    }
}

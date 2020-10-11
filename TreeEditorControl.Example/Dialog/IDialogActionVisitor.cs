using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeEditorControl.Example.Dialog
{
    public interface IDialogActionVisitor<out T>
    {
        T VisitShowText(ShowTextAction node);

        T VisitShowChoice(ChoiceGroup node);

        T VisitConditionalInteraction(ConditionalAction node);

        T VisitModifyPlayerVariable(ModifyPlayerVariableAction node);
    }
}

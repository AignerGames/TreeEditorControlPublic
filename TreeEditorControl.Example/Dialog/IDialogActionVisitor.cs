﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TreeEditorControl.Example.Dialog.Actions;

namespace TreeEditorControl.Example.Dialog
{
    public interface IDialogActionVisitor<out T>
    {
        T VisitShowText(ShowTextAction node);

        T VisitShowChoice(ChoiceGroup node);

        T VisitConditionalInteraction(ConditionalAction node);

        T VisitDiceRollAction(DiceRollAction diceRollAction);

        T VisitModifyPlayerVariable(ModifyPlayerVariableAction node);
    }
}

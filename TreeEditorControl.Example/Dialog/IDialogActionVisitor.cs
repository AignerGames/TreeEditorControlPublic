using System;
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

        T VisitParallelAction(ParallelAction node);

        T VisitSequenceAction(SequenceAction node);

        T VisitAddSceneActor(AddSceneObjectAction node);

        T VisitRemoveSceneActor(RemoveSceneObjectAction node);

        T VisitLookAt(LookAtAction node);

        T VisitTriggerAnimation(TriggerAnimationAction node);

        T VisitWait(WaitAction node);
    }
}

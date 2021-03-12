using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TreeEditorControl.Catalog;
using TreeEditorControl.Nodes;
using TreeEditorControl.Nodes.Implementation;
using TreeEditorControl.Environment;
using TreeEditorControl.Utility;
using TreeEditorControl.UndoRedo.Implementation;
using System.Collections.ObjectModel;
using TreeEditorControl.Example.Combat;
using TreeEditorControl.Commands;

namespace TreeEditorControl.Example.Dialog.Actions
{
    [NodeCatalogInfo("BattlefieldAction", "Actions", "Battlefield")]
    public class BattlefieldAction : DialogAction, ICopyableNode<BattlefieldAction>
    {
        public BattlefieldAction(IEditorEnvironment editorEnvironment) : base(editorEnvironment)
        {
            WinActions = AddGroup<DialogAction>(nameof(WinActions));
            LoseActions = AddGroup<DialogAction>(nameof(LoseActions));

            AddEnemyCommand = new ActionCommand(AddEnemyClicked, () => NewEnemey != null);
            RemoveEnemyCommand = new ActionCommand(RemoveEnemyClicked, () => SelectedEnemy != null);
        }

        public ObservableCollection<string> Enemies { get; } = new ObservableCollection<string>();

        public string SelectedEnemy { get; set; }

        public string NewEnemey { get; set; }

        public ActionCommand AddEnemyCommand { get; }

        public ActionCommand RemoveEnemyCommand { get; }

        public TreeNodeContainer<DialogAction> WinActions { get; }

        public TreeNodeContainer<DialogAction> LoseActions { get; }

        public override T Accept<T>(IDialogActionVisitor<T> visitor) => visitor.VisitBattlefieldAction(this);

        public BattlefieldAction CreateCopy()
        {
            var copy = new BattlefieldAction(EditorEnvironment);

            copy.Enemies.AddItems(Enemies);
            copy.WinActions.AddNodes(WinActions.GetCopyableNodeCopies());
            copy.LoseActions.AddNodes(LoseActions.GetCopyableNodeCopies());

            return copy;
        }

        private void AddEnemyClicked()
        {
            if(NewEnemey == null)
            {
                return;
            }

            Enemies.Add(NewEnemey);
        }

        private void RemoveEnemyClicked()
        {
            if(SelectedEnemy == null)
            {
                return;
            }

            Enemies.Remove(SelectedEnemy);
        }
    }
}

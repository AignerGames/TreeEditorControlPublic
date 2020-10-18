using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StoryCreator.Common.Json;
using StoryCreator.Common.Data;
using StoryCreator.Common.Data.Interaction;


using TreeEditorControl.Example.Dialog;
using StoryCreator.Common.Data.Interaction.Commands;
using StoryCreator.Common.Data.Interaction.Conditions;
using TreeEditorControl.Environment;
using TreeEditorControl.Nodes;

namespace TreeEditorControl.Example.Data
{
    internal class FileSaveHandler
    {
        private readonly Visitor _visitor = new Visitor();

        public void Save(string editorDataPath, string gameExportPath, DialogTabViewModel viewModel)
        {
            var editorData = new EditorData();

            editorData.Actors.AddRange(viewModel.Actors.Select(item => item.Value));
            editorData.Variables.AddRange(viewModel.Variables.Select(item => item.Value));

            editorData.GameData = _visitor.CreateGameData(viewModel.EditorViewModel.RootNodes.OfType<DialogRootNode>());

            SerializationHelper.Save(editorDataPath, editorData, GameData.AbstractTypes);
            SerializationHelper.Save(gameExportPath, editorData.GameData, GameData.AbstractTypes);
        }

        private class Visitor :
            IDialogActionVisitor<InteractionCommandData>,
            IDialogConditionVisitor<InteractionConditionData>
        {

            public GameData CreateGameData(IEnumerable<DialogRootNode> dialogRootNodes)
            {
                var gameData = new GameData();

                foreach(var dialogNode in dialogRootNodes)
                {
                    var interactionData = new InteractionData
                    {
                        Name = dialogNode.Name,
                    };

                    gameData.Interactions.Add(interactionData);

                    interactionData.Condition = CreateConditionData(dialogNode.Conditions);
                    interactionData.Command = CreateCommandData(dialogNode.Actions.Nodes);
                }

                return gameData;
            }

            private InteractionCommandData CreateCommandData(IReadOnlyList<DialogAction> dialogActions)
            {
                var multiCommand = new InteractionMultiCommandData();

                foreach (var dialogAction in dialogActions)
                {
                    var command = dialogAction.Accept(this);

                    multiCommand.Commands.Add(command);
                }

                if(multiCommand.Commands.Count == 0)
                {
                    return null;
                }

                return multiCommand.Commands.Count > 1 ? multiCommand : multiCommand.Commands[0];
            }

            private InteractionConditionData CreateConditionData(ConditionNodeContainer dialogConditions)
            {
                var multiCondition = new InteractionMultiConditionData { Kind = dialogConditions.ConditionKind };

                foreach (var dialogCondition in dialogConditions.Nodes)
                {
                    var condition = dialogCondition.Accept(this);

                    multiCondition.Conditions.Add(condition);
                }

                if (multiCondition.Conditions.Count == 0)
                {
                    return null;
                }

                return multiCondition.Conditions.Count > 1 ? multiCondition : multiCondition.Conditions[0];
            }

            public InteractionCommandData VisitModifyPlayerVariable(ModifyPlayerVariableAction node)
            {
                return new ModifyPlayerVariableInteractionCommandData
                {
                    Variable = node.Variable,
                    ModifyKind = node.ModifyKind,
                    Value = node.Value
                };
            }

            public InteractionCommandData VisitShowText(ShowTextAction node)
            {
                return new ShowInteractionTextCommandData
                {
                    Actor = node.Actor,
                    Sprite = $"{node.Actor}.png",
                    Text = node.Text
                };
            }

            public InteractionCommandData VisitShowChoice(ChoiceGroup node)
            {
                var choiceData = new ShowInteractionChoiceCommandData
                {
                    Actor = node.Actor,
                    Sprite = $"{node.Actor}.png",
                    Text = node.Text
                };

                foreach(var itemNode in node.Choices.Nodes)
                {
                    var itemData = new InteractionChoiceItemData
                    {
                        Text = itemNode.Text,
                        Condition = CreateConditionData(itemNode.Conditions),
                        Command = CreateCommandData(itemNode.Actions.Nodes)
                    };

                    choiceData.Choices.Add(itemData);
                }

                return choiceData;
            }

            public InteractionCommandData VisitConditionalInteraction(ConditionalAction node)
            {
                return new ConditionalInteractionCommandData
                {
                    Condition = CreateConditionData(node.Conditions),
                    TrueCommand = CreateCommandData(node.TrueActions.Nodes),
                    FalseCommand = CreateCommandData(node.FalseActions.Nodes)
                };
            }

            public InteractionCommandData VisitDiceRollAction(DiceRollAction node)
            {
                return new RollDiceInteractionData
                {
                    TargetValue = node.TargetValue,
                    MaxValue = node.MaxValue,
                    SuccessCommand = CreateCommandData(node.SuccessActions.Nodes),
                    FailCommand = CreateCommandData(node.FailActions.Nodes)
                };
            }


            public InteractionConditionData VisitPlayerVariableCondition(PlayerVariableCondition node)
            {
                return new InteractionPlayerVariableConditionData
                {
                    Variable = node.Variable,
                    CompareKind = node.CompareKind,
                    CompareValue = node.CompareValue,
                };
            }
        }
    }
}

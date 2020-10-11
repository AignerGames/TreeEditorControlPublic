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
                        Name = dialogNode.Name
                    };

                    gameData.Interactions.Add(interactionData);

                    // TODO: Property mit einem command statt liste?
                    var commandData = CreateCommandData(dialogNode.Nodes);
                    if(commandData == null)
                    {
                        continue;
                    }

                    if(commandData is InteractionMultiCommandData multiCommandData)
                    {
                        interactionData.Commands.AddRange(multiCommandData.Commands);
                    }
                    else
                    {
                        interactionData.Commands.Add(commandData);
                    }
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

            private InteractionConditionData CreateConditionData(IReadOnlyList<DialogCondition> dialogConditions)
            {
                var multiCondition = new InteractionMultiConditionData();

                foreach (var dialogCondition in dialogConditions)
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
                        Condition = CreateConditionData(itemNode.Conditions.Nodes),
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
                    Condition = CreateConditionData(node.Conditions.Nodes),
                    TrueCommand = CreateCommandData(node.TrueActions.Nodes),
                    FalseCommand = CreateCommandData(node.FalseActions.Nodes)
                };
            }


            public InteractionConditionData VisitPlayerVariableCondition(PlayerVariableCondition node)
            {
                return new InteractionPlayerVariableConditionData
                {
                    Variable = node.Variable,
                    CompareValue = node.Value,
                };
            }
        }
    }
}

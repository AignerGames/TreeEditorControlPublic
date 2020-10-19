﻿using System;
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
using TreeEditorControl.Example.Dialog.Actions;
using TreeEditorControl.Example.Dialog.Conditions;

namespace TreeEditorControl.Example.Data
{
    internal class FileLoadHandler
    {
        private readonly IEditorEnvironment _editorEnvironment;
        private readonly Visitor _visitor;

        public FileLoadHandler(IEditorEnvironment editorEnvironment)
        {
            _editorEnvironment = editorEnvironment;

            _visitor = new Visitor(_editorEnvironment);
        }

        public void Load(string editorDataPath, DialogTabViewModel viewModel)
        {
            var editorData = SerializationHelper.Load<EditorData>(editorDataPath, GameData.AbstractTypes);

            editorData.Actors.ForEach(item => viewModel.Actors.Add(new StringViewModel { Value = item }));
            editorData.Variables.ForEach(item => viewModel.Variables.Add(new StringViewModel { Value = item }));

            var nodes = _visitor.CreateRootNodes(editorData.GameData);

            viewModel.EditorViewModel.AddRootNodes(nodes);
        }

        private class Visitor :
            IInteractionCommandDataVisitor<DialogAction>,
            IInteractionConditionDataVisitor<DialogCondition>
        {
            private readonly IEditorEnvironment _editorEnvironment;

            public Visitor(IEditorEnvironment editorEnvironment)
            {
                _editorEnvironment = editorEnvironment;
            }


            public List<DialogRootNode> CreateRootNodes(GameData gameData)
            {
                var rootNodes = new List<DialogRootNode>();

                if (gameData == null)
                {
                    return rootNodes;
                }

                foreach (var interactionData in gameData.Interactions)
                {
                    var rootNode = new DialogRootNode(_editorEnvironment, interactionData.Name);

                    CreateAndAddConditionNodes(rootNode.Conditions, interactionData.Condition);

                    var rootActions = CreateCommandNodes(interactionData.Command);
                    rootNode.Actions.AddNodes(rootActions);

                    rootNodes.Add(rootNode);
                }

                return rootNodes;
            }

            private List<DialogAction> CreateActionNodes(List<InteractionCommandData> interactionCommandDatas)
            {
                var dialogActions = new List<DialogAction>();

                foreach(var commandData in interactionCommandDatas)
                {
                    var commandNodes = CreateCommandNodes(commandData);

                    dialogActions.AddRange(commandNodes);
                }

                return dialogActions;
            }

            private List<DialogAction> CreateCommandNodes(InteractionCommandData commandData)
            {
                // Special handling, because all commands are handled as lists in the editor

                var dialogActions = new List<DialogAction>();

                if(commandData == null)
                {
                    return dialogActions;
                }

                if(commandData is InteractionMultiCommandData multiCommandData)
                {
                    var actionNodes = CreateActionNodes(multiCommandData.Commands);

                    dialogActions.AddRange(actionNodes);
                }
                else
                {
                    var actionNode = commandData.Accept(this);

                    dialogActions.Add(actionNode);
                }

                return dialogActions;
            }

            private void CreateAndAddConditionNodes(ConditionNodeContainer container, InteractionConditionData conditionData)
            {
                if(conditionData == null)
                {
                    return;
                }

                if(conditionData is InteractionMultiConditionData multiConditionData)
                {
                    container.ConditionKind = multiConditionData.Kind;

                    foreach(var conditionDataItem in multiConditionData.Conditions)
                    {
                        var conditionNode = conditionDataItem.Accept(this);

                        container.Add(conditionNode);
                    }
                }
                else
                {
                    var conditionNode = conditionData.Accept(this);

                    container.Add(conditionNode);
                }
            }

            public DialogAction VisitMultiCommand(InteractionMultiCommandData data)
            {
                throw new InvalidOperationException("Should never be called, because all commands are handled as lists in the editor");
            }

            public DialogCondition VisitMultiCondition(InteractionMultiConditionData interactionMultiConditionData)
            {
                throw new InvalidOperationException("Should never be called, because all conditions are handled as lists in the editor");
            }

            public DialogAction VisitModifyPlayerVariable(ModifyPlayerVariableInteractionCommandData data)
            {
                var node = new ModifyPlayerVariableAction(_editorEnvironment, data.Variable, data.ModifyKind, data.Value);

                return node;
            }

            public DialogAction VisitShowText(ShowInteractionTextCommandData data)
            {
                var node = new ShowTextAction(_editorEnvironment, data.Actor, data.Text);

                return node;
            }

            public DialogAction VisitShowChoice(ShowInteractionChoiceCommandData data)
            {
                var choiceGroupNode = new ChoiceGroup(_editorEnvironment, data.Actor, data.Text);

                foreach(var choiceData in data.Choices)
                {
                    var itemNode = CreateChoiceItem(choiceData);

                    choiceGroupNode.Choices.Add(itemNode);
                }

                return choiceGroupNode;

                ChoiceItem CreateChoiceItem(InteractionChoiceItemData itemData)
                {
                    var item = new ChoiceItem(_editorEnvironment, itemData.Text);

                    var actions = CreateCommandNodes(itemData.Command);
                    item.Actions.AddNodes(actions);

                    CreateAndAddConditionNodes(item.Conditions, itemData.Condition);

                    return item;
                }
            }

            public DialogAction VisitConditionalInteraction(ConditionalInteractionCommandData data)
            {
                var node = new ConditionalAction(_editorEnvironment);

                CreateAndAddConditionNodes(node.Conditions, data.Condition);

                var trueActions = CreateCommandNodes(data.TrueCommand);
                node.TrueActions.AddNodes(trueActions);

                var falseActions = CreateCommandNodes(data.FalseCommand);
                node.FalseActions.AddNodes(falseActions);

                return node;
            }

            public DialogAction VisitRollDiceInteraction(RollDiceInteractionData data)
            {
                var node = new DiceRollAction(_editorEnvironment, data.TargetValue, data.MaxValue);

                var successActions = CreateCommandNodes(data.SuccessCommand);
                node.SuccessActions.AddNodes(successActions);

                var failActions = CreateCommandNodes(data.FailCommand);
                node.FailActions.AddNodes(failActions);

                return node;
            }

            public DialogCondition VisitPlayerVariableCondition(InteractionPlayerVariableConditionData data)
            {
                var node = new PlayerVariableCondition(_editorEnvironment, data.Variable, data.CompareKind, data.CompareValue);

                return node;
            }
        }
    }
}

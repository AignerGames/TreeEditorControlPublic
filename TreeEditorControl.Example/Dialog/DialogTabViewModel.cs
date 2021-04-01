﻿using System;
using System.Reflection;
using System.Collections.ObjectModel;
using System.Windows;
using System.ComponentModel;
using System.IO;
using System.Windows.Input;
using System.Collections.Generic;
using System.Linq;

using TreeEditorControl.Catalog;
using TreeEditorControl.Utility;
using TreeEditorControl.ViewModel;
using TreeEditorControl.Environment.Implementation;
using TreeEditorControl.Commands;
using TreeEditorControl.Example.Data;
using TreeEditorControl.Nodes;
using TreeEditorControl.Example.DataNodes;

namespace TreeEditorControl.Example.Dialog
{
    public class DialogTabViewModel : TabViewModel
    {
        const string GameDataDirectoryName = "GameData";
        const string GameExportDirectoryName = "GameDataExport";
        const string UnityExportDirectoryPath = @"D:\UnityProjectsVR\AignerGamesStoryCreator\Assets\Resources\GameDataExport";


        private string _newFileName;
        private string _selectedFile;

        private string _currentGameName;

        private bool _gameDataLoaded;

        public DialogTabViewModel(EditorEnvironment editorEnvironment) : base("Dialog", editorEnvironment)
        {
            var nodeFactory = new DataNodeFactory(EditorEnvironment);

            nodeFactory.ComboBoxValueProviders["Test"] = new ComboBoxValueProvider(() => new List<string> { "A", "B", "C" });

            EditorEnvironment.NodeFactory = nodeFactory;

            EditorViewModel = new TreeEditorViewModel(EditorEnvironment);

            var catalogItems = NodeCatalogItem.CreateItemsForTypes(new Type[] { typeof(DataNodeFactory.SubNodeData) });
            EditorViewModel.CatalogItems.AddItems(catalogItems);

            EditorViewModel.AddDefaultCommands();
            EditorViewModel.AddDefaultContextMenuCommands();

            // TODO: Toolbar menu
            EditorViewModel.ContextMenuCommands.Add(ContextMenuCommand.Seperator);
            EditorViewModel.ContextMenuCommands.Add(new ContextMenuCommand("Expand node", ExpandNodeFull, () => EditorViewModel.SelectedNode != null));
            EditorViewModel.ContextMenuCommands.Add(ContextMenuCommand.Seperator);
            //EditorViewModel.ContextMenuCommands.Add(new ContextMenuCommand("Insert dialog root", AddDialogRoot, () => _gameDataLoaded));
            //EditorViewModel.ContextMenuCommands.Add(new ContextMenuCommand("Delete dialog root", DeleteDialogRoot, () => EditorViewModel.SelectedNode is DialogRootNode));
            EditorViewModel.ContextMenuCommands.Add(ContextMenuCommand.Seperator);
            EditorViewModel.ContextMenuCommands.Add(new ContextMenuCommand("Save", SaveFile, () => _gameDataLoaded));
            EditorViewModel.ContextMenuCommands.Add(ContextMenuCommand.Seperator);
            EditorViewModel.ContextMenuCommands.Add(new ContextMenuCommand("Reset", ResetFile, () => _gameDataLoaded));

            NewFileCommand = new ActionCommand(AddNewFile, () => !string.IsNullOrWhiteSpace(NewFileName));

            Directory.CreateDirectory(GameDataDirectoryName);
            Directory.CreateDirectory(GameExportDirectoryName);

            RefreshFiles();



            // TODO: Prototype
            var rootNode = nodeFactory.CreatePrototypeNode();
            EditorViewModel.AddRootNode(rootNode);


            var sourceNode = new DataNodeFactory.TestData();
            sourceNode.Name = "ABC";

            var sourceSubNode = new DataNodeFactory.SubNodeData();
            sourceSubNode.SubNodeText = "SUB";

            sourceNode.EpicSubNodes.Add(sourceSubNode);

            rootNode.SetInstanceValues(sourceNode);
        }

        public ObservableCollection<string> FileNames { get; } = new ObservableCollection<string>();

        public string NewFileName
        {
            get => _newFileName;
            set
            {
                SetAndNotify(ref _newFileName, value);
            }
        }

        public ICommand NewFileCommand { get; }

        public string SelectedFile
        {
            get => _selectedFile;
            set
            {
                if (_gameDataLoaded)
                {
                    var boxResult = MessageBox.Show($"Changing file to {value}, save current file {SelectedFile}?", "Question", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
                    if(boxResult == MessageBoxResult.Cancel)
                    {
                        return;
                    }

                    if(boxResult == MessageBoxResult.Yes)
                    {
                        ForceSaveFile();
                    }
                }

                SetAndNotify(ref _selectedFile, value);

                LoadFile();
            }
        }

        public string CurrentGameName
        {
            get => _currentGameName;
            set
            {
                SetAndNotify(ref _currentGameName, value);
            }
        }

        public ObservableCollection<StringViewModel> Actors { get; } = new ObservableCollection<StringViewModel>();

        public ObservableCollection<StringViewModel> Variables { get; } = new ObservableCollection<StringViewModel>();

        public ObservableCollection<StringViewModel> SceneObjects { get; } = new ObservableCollection<StringViewModel>
        {
            // Actors
            //new StringViewModel("Fake"),
            //new StringViewModel("Marc"),
            //new StringViewModel("Anika"),
            //new StringViewModel("Tom"),
            //new StringViewModel("Drake"),
            //new StringViewModel("VikingWarrior"),
            //new StringViewModel("EnglandWarrior"),
            new StringViewModel("HaloBackground"),
            new StringViewModel("MC_Drake"),
            new StringViewModel("MC_Tom"),
            new StringViewModel("MC_Blade"),
            new StringViewModel("MC_Fake"),
            new StringViewModel("MC_Happy"),

            // Effects
            new StringViewModel("Poop"),
            new StringViewModel("Blood"),
            new StringViewModel("Cry"),

            // Environment
            new StringViewModel("VikingCity"),
            new StringViewModel("EnglandCity"),
        };

        public ObservableCollection<StringViewModel> SceneReferenceNames { get; } = new ObservableCollection<StringViewModel>();

        public ObservableCollection<StringViewModel> AnimationTriggers { get; } = new ObservableCollection<StringViewModel>
        {
            new StringViewModel("Idle"),
            new StringViewModel("Move"),
            new StringViewModel("Attack"),
            new StringViewModel("Damage"),
            new StringViewModel("Die"),
        };

        public ObservableCollection<NamedVector> LocationVectors { get; } = new ObservableCollection<NamedVector>
        {
            new NamedVector("Left Outer", -4, 0, 0),
            new NamedVector("Left Inner", -2, 0, 0),
            new NamedVector("Center", 0, 0, 0),
            new NamedVector("Right Inner", 2, 0, 0),
            new NamedVector("Right Outer", 4, 0, 0),
        };

        public override void HandleClosing(CancelEventArgs args)
        {
            SaveFile();
        }

        private void RefreshFiles()
        {
            FileNames.Clear();

            var directoryInfo = new DirectoryInfo(GameDataDirectoryName);
            foreach(var file in directoryInfo.EnumerateFiles())
            {
                FileNames.Add(file.Name);
            }
        }

        private void LoadFile()
        {
            EditorEnvironment.UndoRedoStack.IsEnabled = false;
            EditorEnvironment.UndoRedoStack.Reset();

            CurrentGameName = string.Empty;
            Actors.Clear();
            Variables.Clear();
            SceneReferenceNames.Clear();
            EditorViewModel.ClearRootNodes();

            //_fileLoadHandler.Load(Path.Combine(GameDataDirectoryName, SelectedFile), this);

            //if(EditorViewModel.RootNodes.Count == 0)
            //{
            //    var rootNode = new DialogRootNode(EditorEnvironment, "NewInteraction");
            //    EditorViewModel.AddRootNode(rootNode);
            //}

            var firstNode = EditorViewModel.RootNodes.First();
            firstNode.IsSelected = true;
            firstNode.ExpandRecursive();

            EditorEnvironment.UndoRedoStack.IsEnabled = true;

            _gameDataLoaded = true;
        }

        private void SaveFile()
        {
            if(!_gameDataLoaded || MessageBox.Show("Save file?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
            {
                return;
            }

            ForceSaveFile();
        }

        private void ForceSaveFile()
        {
            EditorEnvironment.UndoRedoStack.IsEnabled = false;

            var unityExportPath = Directory.Exists(UnityExportDirectoryPath) ? Path.Combine(UnityExportDirectoryPath, SelectedFile) : null;

            //_fileSaveHandler.Save(Path.Combine(GameDataDirectoryName, SelectedFile), Path.Combine(GameExportDirectoryName, SelectedFile), unityExportPath, this);

            EditorEnvironment.UndoRedoStack.IsEnabled = true;
        }

        private void AddNewFile()
        {
            var fileName = NewFileName;

            if(!fileName.EndsWith(".json"))
            {
                fileName += ".json";
            }

            if(FileNames.Contains(fileName))
            {
                MessageBox.Show($"File with the same name already exists: {fileName}", "Error");

                return;
            }

            NewFileName = string.Empty;

            FileNames.Add(fileName);

            SelectedFile = fileName;
        }

        private void AddDialogRoot()
        {
            //var insertIndex = -1;

            //if(EditorViewModel.SelectedNode is DialogRootNode selectedRootNode)
            //{
            //    insertIndex = EditorViewModel.RootNodes.IndexOf(selectedRootNode) + 1;
            //}

            //var dialogRootNode = new DialogRootNode(EditorEnvironment, "NewDialog");

            //EditorViewModel.AddRootNode(dialogRootNode, insertIndex);
        }

        private void DeleteDialogRoot()
        {
            //if (!(EditorViewModel.SelectedNode is DialogRootNode rootNode))
            //{
            //    return;
            //}

            //if (MessageBox.Show($"Delete {rootNode.Header}?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
            //{
            //    return;
            //}

            //rootNode.IsSelected = false;
            //EditorViewModel.RemoveRootNode(rootNode);
        }

        private void ExpandNodeFull()
        {
            EditorViewModel.SelectedNode?.ExpandRecursive();
        }

        private void ResetFile()
        {
            if(MessageBox.Show("Reset file?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
            {
                return;
            }

            EditorEnvironment.UndoRedoStack.IsEnabled = false;
            EditorEnvironment.UndoRedoStack.Reset();

            Actors.Clear();
            Variables.Clear();
            EditorViewModel.ClearRootNodes();

            AddDialogRoot();

            EditorEnvironment.UndoRedoStack.IsEnabled = true;
        }
    }
}

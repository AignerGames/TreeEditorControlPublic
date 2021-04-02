using System;
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
using System.Windows.Threading;

namespace TreeEditorControl.Example.Dialog
{
    public class DialogTabViewModel : TabViewModel
    {
        const string GameDataDirectoryName = "GameData";
        const string GameExportDirectoryName = "GameDataExport";
        const string UnityExportDirectoryPath = @"D:\UnityProjectsVR\AignerGamesStoryCreator\Assets\Resources\GameDataExport";


        private string _newFileName;
        private string _selectedFile;

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

            rootNode.ExpandRecursive();



            var testDeserialize = rootNode.GetInstanceValues();
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
                if(_selectedFile == value)
                {
                    return;
                }

                if (_gameDataLoaded)
                {
                    // https://stackoverflow.com/questions/2585183/wpf-combobox-selecteditem-change-to-previous-value
                    var previousValue = _selectedFile;

                    _selectedFile = value;

                    var boxResult = MessageBox.Show($"Changing file to {value}, save current file {previousValue}?", "Question", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
                    if(boxResult == MessageBoxResult.Cancel)
                    {
                        Action dispatcherAction = () =>
                        {
                            SetAndNotify(ref _selectedFile, previousValue);
                        };

                        Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, dispatcherAction);
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

            EditorViewModel.ClearRootNodes();

            var loadPath = Path.Combine(GameDataDirectoryName, SelectedFile);

            var rootData = Shared.Json.SerializationHelper.Load<DataNodeFactory.TestData>(loadPath);
            if(rootData == null)
            {
                rootData = new DataNodeFactory.TestData();
            }

            var rootNode = (DataNode)EditorEnvironment.NodeFactory.CreateNode(rootData.GetType());
            rootNode.SetInstanceValues(rootData);

            EditorViewModel.AddRootNode(rootNode);

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

            var savePath = Path.Combine(GameDataDirectoryName, SelectedFile);

            var rootNode = EditorViewModel.RootNodes.OfType<DataNode>().First();
            var rootData = rootNode.GetInstanceValues() as DataNodeFactory.TestData;

            Shared.Json.SerializationHelper.Save(savePath, rootData);

            //var unityExportPath = Directory.Exists(UnityExportDirectoryPath) ? Path.Combine(UnityExportDirectoryPath, SelectedFile) : null;

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

            EditorViewModel.ClearRootNodes();

            AddDialogRoot();

            EditorEnvironment.UndoRedoStack.IsEnabled = true;
        }
    }
}

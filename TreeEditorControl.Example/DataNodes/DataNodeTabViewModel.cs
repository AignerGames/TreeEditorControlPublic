using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Xml.Serialization;

using TreeEditorControl.Catalog;
using TreeEditorControl.Utility;
using TreeEditorControl.ViewModel;
using TreeEditorControl.Environment.Implementation;
using TreeEditorControl.DataNodes;
using TreeEditorControl.DataNodeAttributes;

namespace TreeEditorControl.Example.DataNodes
{
    /// <summary>
    /// DataNodes are used to automatically map tree nodes to serializable data class properties.
    /// Currently supports properties of: simple data types (strings, numbers, enums) and list of other serializable data classes
    /// </summary>
    public class DataNodeTabViewModel : TabViewModel
    {
        private const string DataFileName = "DataNodesTest.xml";

        public DataNodeTabViewModel(EditorEnvironment editorEnvironment) : base("DataNodes", editorEnvironment)
        {
            var nodeFactory = new DataNodeFactory(editorEnvironment);

            var comboBoxValues = new List<string> { "Test", "A", "42" };
            nodeFactory.ComboBoxValueProviders["TestSelection"] = new ComboBoxValueProvider(() => comboBoxValues);

            editorEnvironment.NodeFactory = nodeFactory;

            EditorViewModel = new TreeEditorViewModel(editorEnvironment);

            EditorViewModel.AddDefaultCommands();
            EditorViewModel.AddDefaultContextMenuCommands();

            var catalogTypes = new List<Type> { typeof(SubData) };
            EditorViewModel.CatalogItems.AddItems(NodeCatalogItem.CreateItemsForTypes(catalogTypes));

            EditorViewModel.ContextMenuCommands.Add(Commands.ContextMenuCommand.Seperator);
            EditorViewModel.ContextMenuCommands.Add(new Commands.ContextMenuCommand("TestDataNodeWriteRead", TestDataNodeWriteRead));
            EditorViewModel.ContextMenuCommands.Add(new Commands.ContextMenuCommand("WriteToFile", WriteToFile));
            EditorViewModel.ContextMenuCommands.Add(new Commands.ContextMenuCommand("ReadFromFile", ReadFromFile));
            EditorViewModel.ContextMenuCommands.Add(Commands.ContextMenuCommand.Seperator);

            var dialogRootNode = nodeFactory.CreateDataNode(typeof(RootData));

            EditorViewModel.AddRootNode(dialogRootNode);
        }

        private void TestDataNodeWriteRead()
        {
            EditorEnvironment.UndoRedoStack.IsEnabled = false;

            var rootNode = (DataNode)EditorViewModel.RootNodes.First();
            rootNode.IsSelected = true;

            var rootData = rootNode.GetInstanceValues();

            rootNode.SetInstanceValues(rootData);

            EditorEnvironment.UndoRedoStack.IsEnabled = true;
        }

        private void WriteToFile()
        {
            try
            {
                EditorEnvironment.UndoRedoStack.IsEnabled = false;

                var rootNode = (DataNode)EditorViewModel.RootNodes.First();
                var rootData = rootNode.GetInstanceValues();

                var serializer = new XmlSerializer(typeof(RootData));

                var path = System.IO.Path.GetFullPath(DataFileName);

                using (var stream = System.IO.File.OpenWrite(path))
                {
                    serializer.Serialize(stream, rootData);
                }

                System.Windows.MessageBox.Show($"Saved file to {path}.", "WriteToFile");
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString(), "WriteToFile error");
            }
            finally
            {
                EditorEnvironment.UndoRedoStack.IsEnabled = true;
            }
        }

        private void ReadFromFile()
        {
            try
            {
                EditorEnvironment.UndoRedoStack.IsEnabled = false;

                var rootNode = (DataNode)EditorViewModel.RootNodes.First();
                rootNode.IsSelected = true;

                var serializer = new XmlSerializer(typeof(RootData));
                using(var stream = System.IO.File.OpenRead(DataFileName))
                {
                    var fileData =  serializer.Deserialize(stream);

                    rootNode.SetInstanceValues(fileData);
                }
            }
            catch(Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString(), "ReadFromFile error");
            }
            finally
            {
                EditorEnvironment.UndoRedoStack.IsEnabled = true;
            }
        }
    }

    public enum MyEnum
    {
        Test,
        Other,
        Last
    }

    [Serializable]
    [NodeHeaderFormat("{0} with {1}")]
    public class RootData
    {
        [TextBoxProperty]
        [NodeHeaderFormatIndex(0)]
        public string Name { get; set; }

        [TextBoxProperty(true, "Other Text")]
        public string OtherText { get; set; }

        [ComboBoxProperty("TestSelection")]
        [NodeHeaderFormatIndex(1)]
        public string Selection { get; set; }

        [TextBoxProperty]
        public int Number { get; set; } = 42;

        [ComboBoxProperty]
        public MyEnum EnumValue { get; set; }

        [CheckBoxProperty]
        public bool CheckBoxValue { get; set; } = true;

        [CheckBoxProperty]
        public string CheckBoxString { get; set; }

        [ObjectProperty]
        public List<SubData> SubNodes { get; } = new List<SubData>();

        [ObjectProperty("Other SubNodes")]
        public List<SubData> OtherSubNodes { get; } = new List<SubData>();

        [ObjectProperty]
        public SubData ObjectData { get; set; } = new SubData();

        [ObjectProperty(singleObjectList: true)]
        public SubData ObjectDataAsList { get; set; } = new SubData();
    }

    [Serializable]
    [NodeCatalogInfo]
    [NodeHeaderFormat("{0}")]
    public class SubData
    {
        [TextBoxProperty]
        [NodeHeaderFormatIndex(0)]
        public string SubNodeText { get; set; }

        [ObjectProperty]
        public NestingData NestingData { get; set; } = new NestingData();
    }

    [Serializable]
    [NodeHeaderFormat("{0}")]
    public class NestingData
    {
        [TextBoxProperty]
        [NodeHeaderFormatIndex(0)]
        public string NestingText { get; set; }
    }
}

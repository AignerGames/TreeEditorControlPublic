using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Collections.ObjectModel;

using TreeEditorControl.Catalog;
using TreeEditorControl.Nodes;
using TreeEditorControl.Environment;
using TreeEditorControl.DataNodeAttributes;

namespace TreeEditorControl.DataNodes
{
    public class DataNodeFactory : ITreeNodeFactory
    {
        private readonly IEditorEnvironment _editorEnvironment;

        private readonly Dictionary<Type, IComboBoxValueProvider> _enumValueProviderCache = new Dictionary<Type, IComboBoxValueProvider>();

        public DataNodeFactory(IEditorEnvironment editorEnvironment)
        {
            _editorEnvironment = editorEnvironment;
        }

        public Dictionary<string, IComboBoxValueProvider> ComboBoxValueProviders { get; } = new Dictionary<string, IComboBoxValueProvider>();

        public Dictionary<Type, Func<Type, List<NodeProperty>, DataNode>> CustomDataNodeFactories { get; } = new Dictionary<Type, Func<Type, List<NodeProperty>, DataNode>>();

        public ITreeNode CreateNode(NodeCatalogItem catalogItem)
        {
            return CreateNode(catalogItem.NodeType);
        }

        public ITreeNode CreateNode(Type nodeType)
        {
            return CreateDataNode(nodeType);
        }

        public DataNode CreatePrototypeNode()
        {
            return CreateDataNode(typeof(TestData));
        }

        public DataNode CreateDataNode(Type dataType)
        {
            var nodeProperties = GetNodeProperties(dataType);

            if(CustomDataNodeFactories.TryGetValue(dataType, out var factory))
            {
                return factory.Invoke(dataType, nodeProperties);
            }

            var dataNode = new DataNode(_editorEnvironment, dataType, nodeProperties);

            return dataNode;
        }

        private List<NodeProperty> GetNodeProperties(Type dataType)
        {
            var nodeProperties = new List<NodeProperty>();

            var typeProperties = dataType.GetProperties();
            foreach(var propertyInfo in typeProperties)
            {
                var propertyAttribute = propertyInfo.GetCustomAttribute<NodePropertyAttribute>();

                NodeProperty nodeProperty = null;

                switch(propertyAttribute)
                {
                    case TextBoxPropertyAttribute textBoxPropertyAttribute:
                        nodeProperty = CreateTextBoxProperty(textBoxPropertyAttribute, propertyInfo);
                        break;
                    case ComboBoxPropertyAttribute comboBoxPropertyAttribute:
                        nodeProperty = CreateComboBoxProperty(comboBoxPropertyAttribute, propertyInfo);
                        break;
                    case CheckBoxPropertyAttribute checkBoxPropertyAttribute:
                        nodeProperty = CreateCheckBoxProperty(checkBoxPropertyAttribute, propertyInfo);
                        break;
                    case ObjectPropertyAttribute objectPropertyAttribute:
                        nodeProperty = CreateObjectProperty(objectPropertyAttribute, propertyInfo);
                        break;
                }

                if(nodeProperty != null)
                {
                    nodeProperties.Add(nodeProperty);
                }
            }

            return nodeProperties;
        }

        private TextBoxProperty CreateTextBoxProperty(TextBoxPropertyAttribute textBoxPropertyAttribute, PropertyInfo propertyInfo)
        {
            return new TextBoxProperty(_editorEnvironment, propertyInfo, textBoxPropertyAttribute.Multiline, textBoxPropertyAttribute.PropertyName);
        }

        private ComboBoxProperty CreateComboBoxProperty(ComboBoxPropertyAttribute comboBoxPropertyAttribute, PropertyInfo propertyInfo)
        {
            IComboBoxValueProvider valueProvoder;

            if (comboBoxPropertyAttribute.ValueProviderId == null && propertyInfo.PropertyType.IsEnum)
            {
                if(!_enumValueProviderCache.TryGetValue(propertyInfo.PropertyType, out valueProvoder))
                {
                    var enumValues = new ObservableCollection<object>(Enum.GetValues(propertyInfo.PropertyType).OfType<object>());
                    valueProvoder = new ComboBoxValueProvider(() => enumValues);

                    _enumValueProviderCache[propertyInfo.PropertyType] = valueProvoder;
                }
            }
            else
            {
                valueProvoder = ComboBoxValueProviders[comboBoxPropertyAttribute.ValueProviderId];
            }

            return new ComboBoxProperty(_editorEnvironment, valueProvoder, propertyInfo, comboBoxPropertyAttribute.PropertyName);
        }

        private CheckBoxProperty CreateCheckBoxProperty(CheckBoxPropertyAttribute checkBoxPropertyAttribute, PropertyInfo propertyInfo)
        {
            return new CheckBoxProperty(_editorEnvironment, propertyInfo, checkBoxPropertyAttribute.PropertyName);
        }

        private ObjectProperty CreateObjectProperty(ObjectPropertyAttribute objectPropertyAttribute, PropertyInfo propertyInfo)
        {
            return new ObjectProperty(_editorEnvironment, propertyInfo, objectPropertyAttribute.PropertyName, objectPropertyAttribute.SingleObjectList);
        }

        public enum MyEnum
        {
            Test,
            Other,
            Last
        }

        [Serializable]
        [NodeHeaderFormat("{0} with {1}")]
        public class TestData
        {
            [TextBoxProperty]
            [NodeHeaderFormatIndex(0)]
            public string Name { get; set; }

            [TextBoxProperty(true, "Epic Text")]
            public string EpicText { get; set; }

            [ComboBoxProperty("Test")]
            [NodeHeaderFormatIndex(1)]
            public string Selection { get; set; }

            [TextBoxProperty]
            public int Number { get; set; }

            [ComboBoxProperty]
            public MyEnum EnumValue { get; set; }

            [CheckBoxProperty]
            public bool CheckBoxValue { get; set; }

            [CheckBoxProperty]
            public string CheckBoxString { get; set; }

            [ObjectProperty]
            public List<SubNodeData> SubNodes { get; } = new List<SubNodeData>();

            [ObjectProperty("Epic SubNodes")]
            public List<SubNodeData> EpicSubNodes { get; } = new List<SubNodeData>();

            [ObjectProperty]
            public SubNodeData ObjectData { get; set; } = new SubNodeData(); // Support null values and auto create them?

            [ObjectProperty(singleObjectList: true)]
            public SubNodeData ObjectDataAsList { get; set; } = new SubNodeData();
        }

        [Serializable]
        [NodeCatalogInfo]
        public class SubNodeData
        {
            [TextBoxProperty]
            public string SubNodeText { get; set; }

            [ObjectProperty]
            public NestingData NestingData { get; set; } = new NestingData();
        }

        public class NestingData
        {
            [TextBoxProperty]
            public string NestingText { get; set; }
        }
    }
}

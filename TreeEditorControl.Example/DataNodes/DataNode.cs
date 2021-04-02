using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Linq;

using TreeEditorControl.Nodes.Implementation;
using TreeEditorControl.Environment;
using TreeEditorControl.Nodes;

namespace TreeEditorControl.Example.DataNodes
{
    public class DataNode : ReadableGroupContainerNode, ICopyableNode<DataNode>
    {
        private readonly ObservableCollection<NodeProperty> _properties = new ObservableCollection<NodeProperty>();

        private readonly string _headerFormat;
        private readonly SortedList<int, NodeProperty> _headerValueProviders;

        public DataNode(IEditorEnvironment editorEnvironment, Type dataType, IReadOnlyList<NodeProperty> properties) : base(editorEnvironment)
        {
            DataType = dataType;

            Header = DataType.Name;

            _headerFormat = DataType.GetCustomAttribute<NodeHeaderFormatAttribute>()?.HeaderFormat;
            if (_headerFormat != null)
            {
                _headerValueProviders = new SortedList<int, NodeProperty>();
            }

            AddProperties(properties);
        }

        public Type DataType { get; }

        public IReadOnlyList<NodeProperty> Properties => _properties;

        public override Type GetNodeType()
        {
            return DataType;
        }

        public DataNode CreateCopy()
        {
            // TODO: Copy values
            var instanceCopy = GetInstanceValues();

            var copyNode = (DataNode)EditorEnvironment.NodeFactory.CreateNode(DataType);

            copyNode.SetInstanceValues(instanceCopy);

            return copyNode;
        }

        public void SetInstanceValues(object instance)
        {
            foreach(var property in Properties)
            {
                property.ReadInstanceValue(instance);
            }

            foreach(var containerNode in Nodes.OfType<PropertyNodeContainer>())
            {
                containerNode.ClearNodes();

                // TODO: Null checks?
                var subNodeValues = containerNode.PropertyInfo.GetValue(instance) as System.Collections.IList;

                foreach(var value in subNodeValues)
                {
                    var subNode = (DataNode)EditorEnvironment.NodeFactory.CreateNode(value.GetType());

                    subNode.SetInstanceValues(value);

                    containerNode.Add(subNode);
                }
            }
        }

        public object GetInstanceValues()
        {
            var instance = Activator.CreateInstance(DataType);

            foreach(var property in Properties)
            {
                property.WriteInstanceValue(instance);
            }

            foreach (var containerNode in Nodes.OfType<PropertyNodeContainer>())
            {
                // TODO: Null checks?
                var dataInstances = containerNode.PropertyInfo.GetValue(instance) as System.Collections.IList;

                foreach (var containerDataNode in containerNode.Nodes.OfType<DataNode>())
                {
                    var dataInstance = containerDataNode.GetInstanceValues();

                    dataInstances.Add(dataInstance);
                }
            }

            return instance;
        }

        private void AddProperties(IReadOnlyList<NodeProperty> properties)
        {
            foreach (var property in properties)
            {
                if (property is ObjectProperty objectProperty)
                {
                    // Special handling for sub node container
                    var listItemType = GetListItemType(objectProperty.PropertyInfo.PropertyType);
                    if(listItemType != null)
                    {
                        AddContainerNode(objectProperty.PropertyInfo, listItemType, property.Name);
                        continue;
                    }
                }

                AddProperty(property);
            }

            static Type GetListItemType(Type type)
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
                {
                    return type.GetGenericArguments()[0];
                }

                return null;
            }
        }

        private void AddProperty(NodeProperty nodeProperty)
        {
            _properties.Add(nodeProperty);

            if (_headerValueProviders == null)
            {
                return;
            }

            var headerIndexAttribute = nodeProperty.PropertyInfo.GetCustomAttribute<NodeHeaderFormatIndexAttribute>();
            if (headerIndexAttribute == null)
            {
                return;
            }

            _headerValueProviders.Add(headerIndexAttribute.Index, nodeProperty);

            nodeProperty.PropertyChanged += NodeProperty_PropertyChanged;
        }

        private TreeNodeContainer<DataNode> AddContainerNode(PropertyInfo propertyInfo, Type listItemType, string propertyName = null)
        {
            var containerName = propertyName ?? propertyInfo.Name;

            var groupContianer = new PropertyNodeContainer(EditorEnvironment, containerName, propertyInfo, listItemType);

            InsertChild(groupContianer);

            return groupContianer;
        }

        private void NodeProperty_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            UpdateHeader();
        }

        private void UpdateHeader()
        {
            try
            {
                var headerInfoValues = _headerValueProviders.Values.Select(provider => provider.Value ?? string.Empty).ToArray();
                var headerInfo = string.Format(_headerFormat, headerInfoValues);

                Header = GetHeaderString(DataType.Name, headerInfo);
            }
            catch
            {
                Header = DataType.Name;
            }
        }

        private static string GetHeaderString(string header, string headerInfo, int maxChar = 50)
        {
            if (string.IsNullOrWhiteSpace(headerInfo))
            {
                return header;
            }

            var displayInfo = headerInfo.Length < maxChar ? headerInfo : headerInfo.Substring(0, maxChar);

            displayInfo = displayInfo.Replace('\r', ' ').Replace('\n', ' ');

            return $"{header} ({displayInfo})";
        }

        private class PropertyNodeContainer : TreeNodeContainer<DataNode>
        {
            public PropertyNodeContainer(IEditorEnvironment editorEnvironment, string header, PropertyInfo propertyInfo, Type listeItemType) : base(editorEnvironment, header)
            {
                PropertyInfo = propertyInfo;
                ListItemType = listeItemType;
            }

            public PropertyInfo PropertyInfo { get; }

            public Type ListItemType { get; }

            public override bool CanInsertNode(Type nodeType)
            {
                 return ListItemType.IsAssignableFrom(nodeType);
            }
        }
    }
}

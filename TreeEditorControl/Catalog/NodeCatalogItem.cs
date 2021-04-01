using System;
using System.Collections.Generic;
using System.Reflection;
using TreeEditorControl.Utility;

namespace TreeEditorControl.Catalog
{
    public class NodeCatalogItem : ObservableObject
    {
        public NodeCatalogItem(string name, string category, string description, Type nodeType)
        {
            Name = name;
            Category = category;
            Description = description;
            NodeType = nodeType;
        }

        public string Name { get; }

        public string Category { get; }

        public string Description { get; }

        public Type NodeType { get; }

        /// <summary>
        /// This method will create <see cref="NodeCatalogItem"/>s for all assignable types in the given assembly.
        /// The node must have an empty constructor or an constructor with a <see cref="Environment.IEditorEnvironment"/> parameter, 
        /// otherwise the factory can't create an instance of the node type and 
        /// the type will be skipped.
        /// Use the <see cref="NodeCatalogInfoAttribute"/> on the node classes / interfaces to specify additional information.
        /// Otherwise the node type name will be used as default name / description for the item.
        /// </summary>
        /// <param name="assignableNodeType">The node type which should be used for the assignable check/></param>
        /// <param name="assembly">The assembly which contains the node types. (For example Assembly.GetExecutingAssembly())</param>
        public static List<NodeCatalogItem> CreateItemsForAssignableTypes(Type assignableNodeType, Assembly assembly)
        {
            var assignableTypes = TypeUtility.GetAssignableTypes(assignableNodeType, assembly);

            return CreateItemsForTypes(assignableTypes);
        }

        public static List<NodeCatalogItem> CreateItemsForTypes(IEnumerable<Type> types)
        {
            var catalogItems = new List<NodeCatalogItem>();

            foreach (var nodeType in types)
            {
                // The node needs an valid constructor, otherwise the factory can't create an instance
                if (!TypeUtility.CanCreateTreeNodeInstance(nodeType))
                {
                    continue;
                }

                var infoAttribute = TypeUtility.GetAttributeFromTypeOrInterface<NodeCatalogInfoAttribute>(nodeType);

                var itemName = infoAttribute?.Name ?? TypeUtility.GetTypeDisplayName(nodeType);
                var itemCategory = infoAttribute?.Category ?? itemName;
                var itemDescription = infoAttribute?.Description ?? itemName;

                catalogItems.Add(new NodeCatalogItem(itemName, itemCategory, itemDescription, nodeType));

            }

            return catalogItems;
        }

        public override string ToString() => $"{GetType().Name} {Name} {Category} {NodeType.Name}";
    }
}

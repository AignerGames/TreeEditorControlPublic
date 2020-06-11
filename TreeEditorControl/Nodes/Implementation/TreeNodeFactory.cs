using System;
using System.Collections.Generic;

using TreeEditorControl.Catalog;
using TreeEditorControl.Environment;
using TreeEditorControl.Utility;

namespace TreeEditorControl.Nodes.Implementation
{
    /// <summary>
    /// The default implementation will try to create a instance of the node with the System.Activator (reflection).
    /// The node type must have an empty constructor or an constructor which takes a <see cref="IEditorEnvironment"/> parameter.
    /// The constructor can have other optional parameter, but the <see cref="IEditorEnvironment"/> must be the first and only required parameter.
    /// A custom factory function can be set for specific node types if the default System.Activator call doesn't work for some reason,
    /// for example if the node type doesn't have the required constructor.
    /// </summary>
    public class TreeNodeFactory : ITreeNodeFactory
    {
        private readonly IEditorEnvironment _editorEnvironment;

        private readonly Dictionary<Type, Func<IEditorEnvironment, ITreeNode>> _nodeFactories = new Dictionary<Type, Func<IEditorEnvironment, ITreeNode>>();

        public TreeNodeFactory(IEditorEnvironment editorEnvironment)
        {
            _editorEnvironment = editorEnvironment;
        }

        public ITreeNode CreateNode(Type nodeType)
        {
            return GetFactoryFun(nodeType)?.Invoke(_editorEnvironment);
        }

        public ITreeNode CreateNode(NodeCatalogItem catalogItem)
        {
            return CreateNode(catalogItem.NodeType);
        }

        public void SetFactoryFunction(Type type, Func<IEditorEnvironment, ITreeNode> factoryFunction)
        {
            _nodeFactories[type] = factoryFunction;
        }

        private Func<IEditorEnvironment, ITreeNode> GetFactoryFun(Type nodeType)
        {
            if(!_nodeFactories.TryGetValue(nodeType, out var factoryFunc))
            {
                if(TypeUtility.CanCreateTreeNodeInstance(nodeType))
                {
                    factoryFunc = TypeUtility.GetNodeFactoryFunction(nodeType);
                }

                // Sore the function even if the value is null, otherwise this method would retry to find the factory function
                _nodeFactories[nodeType] = factoryFunc;
            }

            return factoryFunc;
        }
    }
}

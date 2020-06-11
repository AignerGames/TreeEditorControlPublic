using System;
using System.IO;
using System.Collections.ObjectModel;
using System.Collections.Generic;

using TreeEditorControl.Nodes;
using TreeEditorControl.Utility;

namespace TreeEditorControl.Example.Directory
{
    public class FileSystemNode : ObservableObject, ITreeNode, IReadableNodeContainer
    {
        private readonly FileSystemInfo _info;
        private readonly ObservableCollection<FileSystemNode> _nodes = new ObservableCollection<FileSystemNode>();

        private bool _isSelected;
        private bool _isExpanded;

        public event EventHandler<NodeChangedArgs> NodeChanged;

        public FileSystemNode(FileSystemInfo info)
        {
            _info = info;

            Header = info.Name;
            IconName = _info is DirectoryInfo ? "Directory" : "File";

            CreateAndAddDummyNode();
        }

        private FileSystemNode(string header = "")
        {
            // Private constructor for the dummy nodes
            Header = header;
        }

        public string Header { get; }

        public string FullPath => _info.FullName;

        public string IconName { get; }

        public bool IsSelected { get => _isSelected; set => SetAndNotify(ref _isSelected, value); }

        public ITreeNode Parent { get; private set; }

        public bool IsExpanded { get => _isExpanded; set => SetAndNotify(ref _isExpanded, value); }

        public IReadOnlyList<ITreeNode> Nodes => _nodes;

        protected override void NotifyPropertyChange(string propertyName)
        {
            if (propertyName == nameof(IsExpanded))
            {
                foreach (var child in _nodes)
                {
                    child.NodeChanged -= Child_NodeChanged;
                }

                _nodes.Clear();

                if (IsExpanded)
                {
                    CreateAndAddChildNodes();
                }
                else
                {
                    CreateAndAddDummyNode();
                }

                foreach(var child in _nodes)
                {
                    child.NodeChanged += Child_NodeChanged;
                }
            }

            base.NotifyPropertyChange(propertyName);

            NodeChanged?.Invoke(this, new NodeChangedArgs(this, propertyName));
        }

        private void CreateAndAddDummyNode()
        {
            if (_info is DirectoryInfo)
            {
                // Add a dummy node for the "load on expand" logic
                _nodes.Add(new FileSystemNode());
            }
        }

        private void CreateAndAddChildNodes()
        {
            if (!(_info is DirectoryInfo directoryInfo))
            {
                return;
            }

            try
            {
                foreach (var info in directoryInfo.GetFileSystemInfos())
                {
                    _nodes.Add(new FileSystemNode(info));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Directory expand error: {ex}");
                _nodes.Add(new FileSystemNode("<No access>"));
            }
        }

        private void Child_NodeChanged(object sender, NodeChangedArgs e)
        {
            NodeChanged?.Invoke(this, e);
        }
    }
}

using System.IO;
using System.Collections.Generic;
using TreeEditorControl.Nodes.Implementation;
using TreeEditorControl.Utility;
using TreeEditorControl.ViewModel;
using TreeEditorControl.Environment.Implementation;

namespace TreeEditorControl.Example.Directory
{
    public class DirectoryTabViewModel : TabViewModel
    {
        public DirectoryTabViewModel(EditorEnvironment editorEnvironment) : base("Directory", editorEnvironment)
        {
            var nodeFactory = new TreeNodeFactory(editorEnvironment);

            EditorViewModel = new TreeEditorViewModel(editorEnvironment, nodeFactory);

            EditorViewModel.AddRootNodes(GetDriveNodes());

            EditorViewModel.Commands.Add(new Commands.EditorCommand("Refresh", "Reloads the directory info", () =>
            {
                EditorViewModel.ClearRootNodes();
                EditorViewModel.AddRootNodes(GetDriveNodes());
            }));
        }

        private static List<FileSystemNode> GetDriveNodes()
        {
            var nodes = new List<FileSystemNode>();

            try
            {
                var drives = DriveInfo.GetDrives();
                foreach (var drive in drives)
                {
                    nodes.Add(new FileSystemNode(drive.RootDirectory));
                }
            }
            catch
            {

            }

            return nodes;
        }
    }
}

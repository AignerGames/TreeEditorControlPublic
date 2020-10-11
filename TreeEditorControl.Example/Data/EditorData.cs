using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StoryCreator.Common.Json;
using StoryCreator.Common.Data;
using StoryCreator.Common.Data.Interaction;

namespace TreeEditorControl.Example.Data
{
    [Serializable]
    public class EditorData
    {
        public List<string> Actors { get; set; } = new List<string>();

        public List<string> Variables { get; set; } = new List<string>();

        public GameData GameData { get; set; } = new GameData();
    }
}

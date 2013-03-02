using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace OcarinaData {
    public class CommandList : ContentObject
    {
        public List<string> Commands { get; set; }
    }

    public class CommandListReader : ContentTypeReader<CommandList> {
        protected override CommandList Read(ContentReader input, CommandList existingInstance) {
            CommandList cmds = existingInstance;
            if (cmds == null)
                cmds = new CommandList();

            cmds.AssetName = input.AssetName;
            cmds.Commands = input.ReadObject<List<string>>();

            return cmds;
        }
    }
}

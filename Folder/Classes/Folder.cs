using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Folder.Logic
{
    class Folder
    {
        public string Name { get; set; }
        public string DataCreated { get; set; }
        public string Size { get; set; }

        public List<File> Files { get; set; }
        public List<Folder> Children { get; set; }
    }
}

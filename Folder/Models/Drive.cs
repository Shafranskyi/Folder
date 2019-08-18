using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Folder.Models
{
    public class Drive
    {
        public Drive(string name, bool isReady)
        {
            this.Name = name;
            this.IsReady = isReady;
            this.Children = new ObservableCollection<object>();
        }
        public string Name
        {
            get;
            set;
        }
        public bool IsReady
        {
            get;
            set;
        }
        public ObservableCollection<object> Children
        {
            get;
            private set;
        }
    }

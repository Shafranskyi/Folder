using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Folder.Models
{
    public class ItemStyleSelector : StyleSelector
    {
        public override System.Windows.Style SelectStyle(object item, System.Windows.DependencyObject container)
        {
            if (item is Drive)
                return this.DriveStyle;
            else if (item is Directory)
                return this.DirectoryStyle;
            else if (item is File)
                return this.FileStyle;
            return base.SelectStyle(item, container);
        }

        public Style DirectoryStyle
        {
            get;
            set;
        }
        public Style FileStyle
        {
            get;
            set;
        }
        public Style DriveStyle
        {
            get;
            set;
        }
    }
}

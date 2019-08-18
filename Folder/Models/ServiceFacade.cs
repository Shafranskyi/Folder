using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Folder.Models
{
    public sealed class ServiceFacade
    {
        private static ServiceFacade instance;
        public static ServiceFacade Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ServiceFacade();
                    instance.Initialize();
                }
                return instance;
            }
        }
        public ObservableCollection<Drive> Drives
        {
            get;
            private set;
        }
        private void Initialize()
        {
            this.Drives = new ObservableCollection<Drive>();
            foreach (DriveInfo driveInfo in System.IO.DriveInfo.GetDrives())
            {
                this.Drives.Add(new Drive(driveInfo.Name, driveInfo.IsReady));
            }
        }
    }
}

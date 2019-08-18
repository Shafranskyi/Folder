using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Folder.Logic;
using Folder.ShellClasses;

namespace Folder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string GetPath { get; set; }

        GetFileWithData getFileWithData = null;

        public MainWindow()
        {
            InitializeComponent();
            InitializeFileSystemObjects();

            getFileWithData = new GetFileWithData();
        }

        private void InitializeFileSystemObjects()
        {
            var drives = DriveInfo.GetDrives();
            DriveInfo.GetDrives().ToList().ForEach(drive =>
            {
                var fileSystemObject = new FileSystemObjectInfo(drive);
                fileSystemObject.BeforeExplore += FileSystemObject_BeforeExplore;
                fileSystemObject.AfterExplore += FileSystemObject_AfterExplore;

                treeView.Items.Add(fileSystemObject);
            });
        }

        private void FileSystemObject_AfterExplore(object sender, System.EventArgs e)
        {
            Cursor = System.Windows.Input.Cursors.Arrow;
        }

        private void FileSystemObject_BeforeExplore(object sender, System.EventArgs e)
        {
            Cursor = System.Windows.Input.Cursors.Wait;
        }

        public async void GET_XML_Click(object sender, RoutedEventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    getFileWithData.SavePath = fbd.SelectedPath;
                    getFileWithData.Path = GetPath;
                }
            }
            await getFileWithData.GetFoldersXML();
        }

        public async void GET_JSON_Click(object sender, RoutedEventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    getFileWithData.SavePath = fbd.SelectedPath;
                    getFileWithData.Path = GetPath;
                }
            }
            await getFileWithData.GetFoldersJSON();
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            string FName = ((FileSystemObjectInfo)e.NewValue).FileSystemInfo.FullName;
            if (!String.IsNullOrEmpty(FName))
            {
                FileAttributes attr = System.IO.File.GetAttributes(FName);
                if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    GetPath = FName;
                    path.Text = GetPath;
                    xml.IsEnabled = json.IsEnabled = true;
                }
                else
                {
                    System.Windows.MessageBox.Show("Select folder, please!");
                }
            }
            else
            {
                System.Windows.MessageBox.Show("Select folder, please!");
            }
        }
    }
}

using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Diagnostics;
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
        CancellationTokenSource cts;
        string GetPath { get; set; }

        bool ok = false;

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
            DriveInfo.GetDrives().Where(drive => drive.IsReady == true).ToList().ForEach(drive =>
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

        private async void GET_XML_Click(object sender, RoutedEventArgs e)
        {
            ok = false;
            try
            {
                cts = new CancellationTokenSource();
                using (var fbd = new FolderBrowserDialog() { Description = "Select the folder to save the file, please", RootFolder = Environment.SpecialFolder.Desktop })
                {
                    if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                    {
                        if (System.Windows.MessageBox.Show("Save here:" + fbd.SelectedPath + "?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                        {
                            ok = true;
                            getFileWithData.SavePath = fbd.SelectedPath;
                            getFileWithData.Path = GetPath;
                        }
                    }
                }
                if (ok == true)
                {
                    cts = new CancellationTokenSource();
                    spinner.Visibility = Visibility.Visible;
                    mission.Visibility = Visibility.Hidden;
                    cancel.Visibility = Visibility.Visible;
                    select.Text = "XML layout you selected for your file! Expect, please.";
                    json.IsEnabled = xml.IsEnabled = false;
                    await getFileWithData.GetFoldersXML(cts.Token);
                    spinner.Visibility = Visibility.Hidden;
                    mission.Visibility = Visibility.Visible;
                    cancel.Visibility = Visibility.Hidden;
                    select.Text = "";
                }
            }
            catch (TaskCanceledException ex)
            {
                spinner.Visibility = Visibility.Hidden;
                select.Text = ex.Message;
                cancel.Visibility = Visibility.Hidden;
                path.Text = "";
            }
        }

        private async void GET_JSON_Click(object sender, RoutedEventArgs e)
        {
            ok = false;
            try
            {
                cts = new CancellationTokenSource();
                using (var fbd = new FolderBrowserDialog() { Description = "Select the folder to save the file, please", RootFolder = Environment.SpecialFolder.Desktop })
                {
                    if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                    {
                        if (System.Windows.MessageBox.Show("Save here:" + fbd.SelectedPath + "?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                        {
                            ok = true;
                            getFileWithData.SavePath = fbd.SelectedPath;
                            getFileWithData.Path = GetPath;
                        }
                    }
                }
                if (ok == true)
                {
                    cts = new CancellationTokenSource();
                    spinner.Visibility = Visibility.Visible;
                    mission.Visibility = Visibility.Hidden;
                    cancel.Visibility = Visibility.Visible;
                    select.Text = "JSON layout you selected for your file! Expect, please.";
                    json.IsEnabled = xml.IsEnabled = false;
                    var d = await getFileWithData.GetFoldersJSON(cts.Token);
                    spinner.Visibility = Visibility.Hidden;
                    mission.Visibility = Visibility.Visible;
                    cancel.Visibility = Visibility.Hidden;
                    select.Text = "";
                }
            }
            catch (TaskCanceledException ex)
            {
                spinner.Visibility = Visibility.Hidden;
                select.Text = ex.Message;
                cancel.Visibility = Visibility.Hidden;
                path.Text = "";
            }
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (!spinner.IsVisible)
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

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            if (cts != null)
            {
                cts.Cancel();
            }
        }
    }
}

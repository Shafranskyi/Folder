using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Folder.Logic
{
    class GetFileWithData
    {
        XmlDocument doc = new XmlDocument();
        public string Path { get; set; }
        public string SavePath { get; set; }
        public GetFileWithData() { }
        private ulong Get_Folders_JSON(ref Folder folder, string path_2, CancellationToken token)
        {
            if (token.IsCancellationRequested)
                return 0;
            string[] folders;
            string[] files;
            try
            {
                folders = Directory.GetDirectories(path_2);
                files = Directory.GetFiles(path_2);
            }
            catch (Exception exc)
            {
                return 0;
            }
            folder.Children = new List<Folder>();
            folder.Files = new List<File>();
            ulong size = 0;
            for (int j = 0; j < files.Length; ++j)
            {
                if (files[j].Length > 247)
                    return 0;
                FileInfo file_all = new FileInfo(files[j]);
                size += (ulong)file_all.Length;
                folder.Files.Add(new File() { Name = file_all.Name, Path = file_all.FullName, Size = file_all.Length.ToString() });
            }
            for (int j = 0; j < folders.Length; ++j)
            {
                if (folders[j].Length > 247)
                    return 0;
                Folder myfolder = new Folder() { };
                ulong upperSize = Get_Folders_JSON(ref myfolder, folders[j], token);
                myfolder.Name = folders[j].Substring(folders[j].LastIndexOf('\\') + 1);
                myfolder.DataCreated = Directory.GetCreationTime(folders[j]).ToString();
                myfolder.Size = upperSize.ToString();
                size += upperSize;
                folder.Children.Add(myfolder);
            }
            return size;
        }
        private ulong Get_Folders_XML(XmlElement a, string path_2, CancellationToken token)
        {
            if (token.IsCancellationRequested)
                return 0;
            string[] folders;
            string[] files;
            try
            {
                folders = Directory.GetDirectories(path_2);
                files = Directory.GetFiles(path_2);
            }
            catch (Exception exc)
            {
                return 0;
            }
            ulong size = 0;
            for (int j = 0; j < files.Length; ++j)
            {
                if (files[j].Length > 247)
                    return 0;
                FileInfo file_all;
                XmlElement file;
                XmlText file_name;
                file_all = new FileInfo(files[j]);
                size += (ulong)file_all.Length;
                file = doc.CreateElement("File");
                file_name = doc.CreateTextNode(file_all.Name);
                file.AppendChild(file_name);
                a.AppendChild(file);
            }
            for (int j = 0; j < folders.Length; ++j)
            {
                if (folders[j].Length > 247)
                    return 0;
                XmlElement folder;
                XmlAttribute size_folder, name_folder;
                folder = doc.CreateElement("Folder");
                ulong upperSize = Get_Folders_XML(folder, folders[j], token);
                size_folder = doc.CreateAttribute("size");
                name_folder = doc.CreateAttribute("name");
                name_folder.Value = folders[j].Substring(folders[j].LastIndexOf('\\') + 1);
                size_folder.Value = upperSize.ToString();
                folder.Attributes.Append(name_folder);
                folder.Attributes.Append(size_folder);
                size += upperSize;
                a.AppendChild(folder);
            }
            return size;
        }
        public Task<bool> GetFoldersXML(CancellationToken token)
        {
            Task<bool> task = null;

            task = Task.Run(() =>
            {
                if (token.IsCancellationRequested)
                    throw new TaskCanceledException(task);
                string sub_path = "";
                XmlElement xRoot;
                XmlDeclaration dec = doc.CreateXmlDeclaration("1.0", "utf-8", "yes");
                doc.AppendChild(dec);
                xRoot = doc.CreateElement("Computer_drives");
                doc.AppendChild(xRoot);
                DirectoryInfo directory = new DirectoryInfo(Path);
                sub_path = directory.Name;
                foreach (var drive in DriveInfo.GetDrives())
                {
                    if (directory.Name == drive.Name)
                    {
                        sub_path = drive.Name.Substring(0, directory.Name.LastIndexOf('\\') - 1);
                    }
                }
                XmlElement folder_x = doc.CreateElement(sub_path);
                Get_Folders_XML(folder_x, directory.FullName, token);
                if (token.IsCancellationRequested)
                {
                    throw new TaskCanceledException(task);
                }
                xRoot.AppendChild(folder_x);
                doc.Save(SavePath + "folder.txt");
                return true;
            });
            return task;
        }
        public Task<bool> GetFoldersJSON(CancellationToken token)
        {
            Task<bool> task = null;

            task = Task.Run(() =>
            {                
                Debug.WriteLine("Run");
                DirectoryInfo directory = new DirectoryInfo(Path);
                Folder Folder = new Folder() { Name = directory.Name, DataCreated = Directory.GetCreationTime(Path).ToString() };
                Folder.Size = Get_Folders_JSON(ref Folder, directory.FullName, token).ToString();
                if (token.IsCancellationRequested)
                {
                    throw new TaskCanceledException(task);
                }
                string json = JsonConvert.SerializeObject(Folder);
                System.IO.File.WriteAllText(SavePath + "folder.txt", json);
                return true;
            });
            return task;
        }
    }
}

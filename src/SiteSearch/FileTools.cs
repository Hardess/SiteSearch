using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using Word = Microsoft.Office.Interop.Word;
using System.Web;

namespace SiteSearch
{
    public class FileTools
    {
        /// <summary>
        /// 根据文件类型获取实体集合
        /// </summary>
        /// <param name="FilePath">附件路径</param>
        /// <param name="FileType">附件后缀名</param>
        /// <param name="Id">关联主键编号</param>
        /// <returns>创建索引时所需要的listmodel</returns>
        public static List<Model> GetModelListByFileType(string FilePath, string FileType,int Id)
        {
            List<Model> list = new List<Model>();
            switch (FileType)
            {
                case ".doc":
                case ".docx":
                    list.AddRange(GetWordFile(FilePath,Id));
                    break;
                case ".xls":
                case ".xlsx":
                    list.AddRange(GetExcelFile(FilePath,Id));
                    break;
                case ".rar":
                case ".zip":
                    list.AddRange(GetCompressFile(FilePath,Id));
                    break;
                default:
                    break;
            }
            return list;
        }

        public static List<Model> GetCompressFile(string FilePath, int Id)
        {
            //TODO:解压压缩文件并为其中的文件内容生成实体集合
           return  LoadData(FilePath,Id);
        }
        public static List<Model> LoadData(string FilePath, int Id)
        {
            List<Model> list = new List<Model>();
            //压缩文件解压后的路径
            string ToPath = @"D:\SiteSearchData";
            FileInfo F = new FileInfo(FilePath);
            if (!System.IO.Directory.Exists(ToPath))
            {
                System.IO.Directory.CreateDirectory(ToPath);//不存在就创建目录 
            }
            if (F.Extension == ".zip")
            {
                using (ZipArchive Archive = ZipFile.Open(FilePath, ZipArchiveMode.Update))
                {
                    //解压文件
                    Archive.ExtractToDirectory(ToPath);
                }
            }
            else
            {
                DeCompressRar(FilePath, ToPath);
            }
            if (Directory.GetFileSystemEntries(ToPath).Length > 0)
            {
                //遍历文件夹中所有文件  
                foreach (string file in Directory.GetFiles(ToPath))
                {
                    //文件是否存在 
                    if (File.Exists(file))
                    {
                        FileInfo fi = new FileInfo(file);
                        //根据压缩包内文件的类型生成实体集合
                        switch(fi.Extension)
                        {
                            case ".doc":
                            case ".docx":
                                list.AddRange(GetWordFile(FilePath,Id));
                                break;
                            case ".xls":
                            case ".xlsx":
                                list.AddRange(GetExcelFile(FilePath,Id));
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            return list;
        }
        /// <summary>
        /// 将格式为rar的压缩文件解压到指定的目录
        /// </summary>
        /// <param name="rarFileName">要解压rar文件的路径</param>
        /// <param name="saveDir">解压后要保存到的目录</param>
        public static void DeCompressRar(string rarFilePath, string ToPath)
        {
            string regKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\WinRAR.exe";
            RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(regKey);
            string winrarPath = registryKey.GetValue("").ToString();
            registryKey.Close();
            string winrarDir = System.IO.Path.GetDirectoryName(winrarPath);
            String commandOptions = string.Format("x {0} {1} -y", rarFilePath, ToPath);

            ProcessStartInfo processStartInfo = new ProcessStartInfo();
            processStartInfo.FileName = System.IO.Path.Combine(winrarDir, "rar.exe");
            processStartInfo.Arguments = commandOptions;
            processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;

            Process process = new Process();
            process.StartInfo = processStartInfo;
            process.Start();
            process.WaitForExit();
            process.Close();
        }
        ///// <summary>
        ///// 本机是否安装winrar程序
        ///// </summary>
        ///// <returns></returns>
        //public static string ExistsWinRar()
        //{
        //    string result = string.Empty;

        //    string key = @"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\WinRAR.exe";
        //    RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(key);
        //    if (registryKey != null)
        //    {
        //        result = registryKey.GetValue("").ToString();
        //    }
        //    registryKey.Close();

        //    return result;
        //}
        public static List<Model> GetExcelFile(string FilePath, int Id)
        {
            //TODO:为Excel文件内容生成实体集合
            throw new NotImplementedException();
        }

        public static List<Model> GetWordFile(string FilePath, int Id)
        {
            //TODO:为Word文件内容生成实体集合
            List<Model> list = new List<Model>();
            try
            {
                Word.Application app = new Microsoft.Office.Interop.Word.Application();
                Word.Document doc = null;
                object unknow = Type.Missing;
                app.Visible = true;
                string str = HttpContext.Current.Server.MapPath(FilePath);
                object file = str;
                doc = app.Documents.OpenNoRepairDialog(ref file,
                    ref unknow, ref unknow, ref unknow, ref unknow,
                    ref unknow, ref unknow, ref unknow, ref unknow,
                    ref unknow, ref unknow, ref unknow, ref unknow,
                    ref unknow, ref unknow, ref unknow);
                string temp = doc.Content.ToString();
                Model dto = new Model();
                dto.Id = Id;
                dto.Content = temp;
                dto.Title = new FileInfo(FilePath).Name;
                list.Add(dto);
            }
            catch (Exception ex)
            { 
            }
            return list;
        }
    }
}

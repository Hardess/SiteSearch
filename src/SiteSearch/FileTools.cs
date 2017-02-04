using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPOI;

namespace SiteSearch
{
    public class FileTools
    {
        /// <summary>
        /// 根据文件类型获取实体集合
        /// </summary>
        /// <param name="FilePath">附件路径</param>
        /// <param name="FileType">附件后缀名</param>
        /// <returns>创建索引时所需要的listmodel</returns>
        private static List<Model> GetModelListByFileType(string FilePath, string FileType)
        {
            List<Model> list = new List<Model>();
            switch (FileType)
            {
                case "doc":
                case "docx":
                    list.AddRange(GetWordFile(FilePath));
                    break;
                case "xls":
                case "xlsx":
                    list.AddRange(GetExcelFile(FilePath));
                    break;
                case "rar":
                case "zip":
                    list.AddRange(GetCompressFile(FilePath));
                    break;
                default:
                    break;
            }
            return list;
        }

        private static List<Model> GetCompressFile(string FilePath)
        {
            //TODO:解压压缩文件并为其中的文件内容生成实体集合
           return  LoadData(FilePath);
        }
        private static List<Model> LoadData(string FilePath)
        {
            List<Model> list = new List<Model>();
            //压缩文件解压后的路径
            string ToPath = @"D:\SiteSearchData";
            if (!System.IO.Directory.Exists(ToPath))
            {
                System.IO.Directory.CreateDirectory(ToPath);//不存在就创建目录 
            }
            using (ZipArchive Archive = ZipFile.Open(FilePath, ZipArchiveMode.Update))
            {
                //解压文件
                Archive.ExtractToDirectory(ToPath);
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
                            case "doc":
                            case "docx":
                                list.AddRange(GetWordFile(FilePath));
                                break;
                            case "xls":
                            case "xlsx":
                                list.AddRange(GetExcelFile(FilePath));
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            return list;
        }

        private static List<Model> GetExcelFile(string FilePath)
        {
            //TODO:为Excel文件内容生成实体集合
            throw new NotImplementedException();
        }

        private static List<Model> GetWordFile(string FilePath)
        {
            //TODO:为Word文件内容生成实体集合
            throw new NotImplementedException();
        }
    }
}

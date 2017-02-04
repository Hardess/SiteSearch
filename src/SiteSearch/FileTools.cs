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
        /// 根据文件类型获取实体
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
            throw new NotImplementedException();
        }
        private void LoadData(string FilePath)
        {
            string ToPath = @"D:\SiteSearchData";
            if (!System.IO.Directory.Exists(ToPath))
            {
                System.IO.Directory.CreateDirectory(ToPath);//不存在就创建目录 
            }
            using (ZipArchive Archive = ZipFile.Open(FilePath, ZipArchiveMode.Update))
            {

                Archive.ExtractToDirectory(ToPath);
            }
            if (Directory.GetFileSystemEntries(ToPath).Length > 0)
            {
                //遍历文件夹中所有文件  
                foreach (string file in Directory.GetFiles(ToPath))
                {

                }
            }
        }

        private static List<Model> GetExcelFile(string FilePath)
        {
            throw new NotImplementedException();
        }

        private static List<Model> GetWordFile(string FilePath)
        {
            throw new NotImplementedException();
        }
    }
}

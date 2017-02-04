using System;
using System.Collections.Generic;
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
            switch (FileType)
            {
                case "doc":
                case "docx":
                    return GetWordFile(FilePath);
                case "xls":
                case "xlsx":
                    return GetExcelFile(FilePath);
                case "rar":
                case "zip":
                    return GetCompressFile(FilePath);

                default: return new List<Model>();
            }
        }

        private static List<Model> GetCompressFile(string FilePath)
        {
            throw new NotImplementedException();
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

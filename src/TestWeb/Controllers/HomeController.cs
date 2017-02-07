using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using SiteSearch;

namespace TestWeb.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/
        static string IndexPath = @"D:\Index";
        public ActionResult AddData()
        {
            return View();
        }
        public ActionResult Index()
        {
            try
            {
                if (!System.IO.Directory.Exists(IndexPath))
                {
                    System.IO.Directory.CreateDirectory(IndexPath);//不存在就创建目录 
                }
                string FilePath = Request.QueryString["Path"];
                FileInfo F = new FileInfo(FilePath);
                List<Model> list = new List<Model>();
                list.AddRange(FileTools.GetModelListByFileType(FilePath, F.Extension, 1));
                CreateIndex.CreateIndexByData(IndexPath, list);
                ViewBag.msg = "创建索引库成功";
            }
            catch (Exception)
            {
                ViewBag.msg = "创建索引库失败";
            }
            return View();
        }
        public ActionResult SearchIndex()
        {
            List<Model> result = new List<Model>(SiteSearch.SearchIndex.QueryIndex(IndexPath, Request.QueryString["SearchKey"]));
            ViewData.Add("key", result);
            return View();
        }

    }
}

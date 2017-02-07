using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Lucene.Net.Analysis.PanGu;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Store;

namespace SiteSearch
{
    public class IndexManager
    {
        public static readonly IndexManager Index = new IndexManager();
        public static readonly string indexPath =@"D:\Index"; 
        private IndexManager()
        {
        }
        //请求队列 解决索引目录同时操作的并发问题
        private Queue<ViewMode> Queue = new Queue<ViewMode>();
        /// <summary>
        /// 新增数据表信息时 添加邢增索引请求至队列
        /// </summary>
        /// <param name="dto"></param>
        public void Add(Model dto)
        {
            ViewMode vm = new ViewMode();
            vm.Id = dto.Id;
            vm.Title = dto.Title;
            vm.IT = IndexType.Insert;
            vm.Content = dto.Content;
            Queue.Enqueue(vm);
        }
        /// <summary>
        /// 删除数据表信息时 添加删除索引请求至队列
        /// </summary>
        /// <param name="id"></param>
        public void Del(int id)
        {
            ViewMode vm = new ViewMode();
            vm.Id = id;
            vm.IT = IndexType.Delete;
            Queue.Enqueue(vm);
        }
        /// <summary>
        /// 修改数据表信息时 添加修改索引(实质上是先删除原有索引 再新增修改后索引)请求至队列
        /// </summary>
        /// <param name="dto"></param>
        public void Mod(Model dto)
        {
            ViewMode vm = new ViewMode();
            vm.Id = dto.Id;
            vm.Title = dto.Title;
            vm.IT = IndexType.Modify;
            vm.Content = dto.Content;
            Queue.Enqueue(vm);
        }

        public void StartNewThread()
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(QueueToIndex));
        }

        //定义一个线程 将队列中的数据取出来 插入索引库中
        private void QueueToIndex(object para)
        {
            while (true)
            {
                if (Queue.Count > 0)
                {
                    CRUDIndex();
                }
                else
                {
                    Thread.Sleep(5000);
                }
            }
        }
        /// <summary>
        /// 更新索引库操作
        /// </summary>
        private void CRUDIndex()
        {
            FSDirectory directory = FSDirectory.Open(new DirectoryInfo(indexPath), new NativeFSLockFactory());
            bool isExist = IndexReader.IndexExists(directory);
            if (isExist)
            {
                if (IndexWriter.IsLocked(directory))
                {
                    IndexWriter.Unlock(directory);
                }
            }
            IndexWriter writer = new IndexWriter(directory, new PanGuAnalyzer(), !isExist, IndexWriter.MaxFieldLength.UNLIMITED);
            while (Queue.Count > 0)
            {
                Document document = new Document();
                ViewMode dto = Queue.Dequeue();
                if (dto.IT == IndexType.Insert)
                {
                    document.Add(new Field("id", dto.Id.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
                    document.Add(new Field("title", dto.Title, Field.Store.YES, Field.Index.ANALYZED,
                                           Field.TermVector.WITH_POSITIONS_OFFSETS));
                    document.Add(new Field("content", dto.Content, Field.Store.YES, Field.Index.ANALYZED,
                                           Field.TermVector.WITH_POSITIONS_OFFSETS));
                    writer.AddDocument(document);
                }
                else if (dto.IT == IndexType.Delete)
                {
                    writer.DeleteDocuments(new Term("id", dto.Id.ToString()));
                }
                else if (dto.IT == IndexType.Modify)
                {
                    //先删除 再新增
                    writer.DeleteDocuments(new Term("id", dto.Id.ToString()));
                    document.Add(new Field("id", dto.Id.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
                    document.Add(new Field("title", dto.Title, Field.Store.YES, Field.Index.ANALYZED,
                                           Field.TermVector.WITH_POSITIONS_OFFSETS));
                    document.Add(new Field("content", dto.Content, Field.Store.YES, Field.Index.ANALYZED,
                                           Field.TermVector.WITH_POSITIONS_OFFSETS));
                    writer.AddDocument(document);
                }
            }
            writer.Close();
            directory.Close();
        }
    }
    public class ViewMode
    {
        public int Id
        {
            get;
            set;
        }
        public string Title
        {
            get;
            set;
        }
        public string Content
        {
            get;
            set;
        }
        public IndexType IT
        {
            get;
            set;
        }
    }
    //操作类型枚举
    public enum IndexType
    {
        Insert,
        Modify,
        Delete
    }
}

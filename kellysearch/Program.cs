using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Ivony.Html.Parser;
using Ivony.Html;
using System.Data;
using System.Threading;

namespace kellysearch
{
    class Program
    {
        static void Main(string[] args)
        {
            Program p = new Program();
            Console.WriteLine("请输入网址");
            string url = Console.ReadLine();
            Console.Clear();
            Console.WriteLine("输入完毕,您输入的网址为"+url);
            Console.WriteLine("请输入操作参数。");
            Console.WriteLine("1、获取所有公司链接地址");
            Console.WriteLine("2、获取公司传真");
            string text = Console.ReadLine();
            int i = int.Parse(text);
            switch (i)
            {
                case 1:
                    p.GetPage(url);
                    break;
                case 2:
                    p.GetFax();
                    break;
            }
            Console.ReadLine();
        }
        /// <summary>
        /// 获取公司地址
        /// </summary>
        /// <param name="url">需要查询的地址</param>
        private void GetPage(string url)
        {
            WebClient client = new WebClient();
            string html = client.DownloadString(url);
            JumonyParser jp = new JumonyParser();
            IHtmlDocument document = jp.Parse(html);
            IEnumerable<IHtmlElement> rows = document.Find(".pagediv input");
            int page = 1;
            foreach (IHtmlElement abc in rows)
            {
                string name = abc.Attribute("name").Value();
                if (name == "maxPage")
                {
                    string value = abc.Attribute("value").Value();
                    page = int.Parse(value);

                }
            }
            GetUrl(url, page);
        }
        private void GetUrl(string url, int maxPage)
        {
            for (int i = 1; i <= maxPage; i++)
            {
                
                    string urls = url + "&page=" + i;
                    WebClient client = new WebClient();
                    string html = client.DownloadString(urls);
                    JumonyParser jp = new JumonyParser();
                    IHtmlDocument document = jp.Parse(html);
                    IEnumerable<IHtmlElement> rows = document.Find(".searchresult_zonee .heading_address a");
                    foreach (IHtmlElement abc in rows)
                    {
                        try
                        {
                            string businessUrl = "http://www.kellysearch.com/" + abc.Attribute("href").Value();
                            string name = abc.InnerText();
                            faxDataSet.kellysearch_faxDataTable dt = new faxDataSet.kellysearch_faxDataTable();
                            DataRow row = dt.NewRow();
                            row["name"] = name;
                            row["status"] = 0;
                            row["url"] = businessUrl;
                            dt.Rows.Add(row);
                            faxDataSetTableAdapters.kellysearch_faxTableAdapter apt = new faxDataSetTableAdapters.kellysearch_faxTableAdapter();
                            apt.Update(dt);
                            Console.WriteLine(name + businessUrl);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
            }
        }
        /// <summary>
        /// 获取公司传真
        /// </summary>
        private void GetFax()
        {
            faxDataSetTableAdapters.kellysearch_faxTableAdapter apt = new faxDataSetTableAdapters.kellysearch_faxTableAdapter();
            DataTable dt = apt.GetDataBy();
            DataRow[] rows = dt.Select();
            foreach (DataRow row in rows)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(CallBack), row);

            }
        }
        private void CallBack(object obj)
        {
            DataRow row = (DataRow)obj;
            WebClient client = new WebClient();
            string html = client.DownloadString(row["url"].ToString());
            JumonyParser jp = new JumonyParser();
            IHtmlDocument document = jp.Parse(html);
            IEnumerable<IHtmlElement> htmlRows = document.Find(".tel");
            foreach (IHtmlElement abc in htmlRows)
            {
                string fax = abc.InnerText();
                int i = fax.IndexOf("fax");
                int length = fax.Length;
                string faxnum = "无";
                if (i > -1)
                {
                    i = i + 3;
                    string sub = fax.Substring(i, length - i);
                    sub = sub.Replace("+1", "");
                    sub = sub.Replace("+", "");
                    sub = sub.Replace("(", "");
                    sub = sub.Replace(")", "");
                    sub = sub.Replace(" ", "");
                    sub = sub.Replace(".", "");
                    sub = sub.Replace("-", "");
                    row["fax"] = sub;
                    faxnum = sub;
                    
                }
                row["status"] = 1;
                new faxDataSetTableAdapters.kellysearch_faxTableAdapter().Update(row);
                Console.WriteLine(faxnum);
            }
        }
    }
}

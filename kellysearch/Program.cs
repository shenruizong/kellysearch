using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
                    p.GetUrl(url);
                    break;
                case 2:
                    p.GetFax();
                    break;
            }
        }
        private void GetUrl(string url)
        {

        }
        private void GetFax()
        {
 
        }
    }
}

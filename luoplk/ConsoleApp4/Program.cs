using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ConsoleApp4
{
    class Run//该class功能为获取用户输入的指令，并根据指令调用其他class的函数
    {
        string s;
        public Run()
        {
            s = Console.ReadLine();//获取用户输入的指令
            get();
        }
        private void get()
        {
            string s1, s2;//储存文件地址
            s1 = @"F:\acm\ConsoleApp4\in.txt";//初始输入地址
            s2 = @"F:\acm\ConsoleApp4\out.txt";//初始打印地址
            //Console.WriteLine(s1); Console.WriteLine(s2);
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == '-' && s[i + 1] == 'i' && i + 3 < s.Length) //获取输入txt文件地址
                {
                    char[] ssa = new char[100];
                    i += 3; int j = 0;
                    while (s[i] != ' ' && i < s.Length)
                    {
                        ssa[j++] = s[i++];
                        if (i == s.Length)
                            break;
                    }
                    string ss1 = new string(ssa);
                    s1 = string.Format(@"{0}", ss1);
                   // Console.WriteLine(s1);
                }
                if (s[i] == '-' && s[i + 1] == 'o' && i + 3 < s.Length)//获取输出txt文件地址
                {
                    char[] ssb = new char[100];
                    i += 3; int j = 0;
                    while (s[i] != ' ' && i < s.Length)
                    {
                        ssb[j++] = s[i++];
                        if (i == s.Length)
                            break;
                    }
                    string ss2 = new string(ssb);
                    s2 = string.Format(@"{0}", ss2);
                   // Console.WriteLine(s2);
                }
            }
            Process qq = new Process(s1, s2);//通过用户输入更改输入和打印地址
            string key = qq.gets();
            Init p = new Init(key);//对文档内容进行处理
            Process q = new Process(p.return_diction(), s1, s2, p.return_charcount(), p.return_row());//传入地址和dictionary用于查询询问
            for (int i = 0; i < s.Length; i++)//获取各个询问
                if (s[i] == '-')
                {
                    if (s[i + 1] == 'n')//表示设定输出的单词数量
                    {
                        char[] ssc = new char[100];
                        i += 3; int j = 0;
                        while (s[i] != ' ' && i < s.Length)
                        {
                            ssc[j++] = s[i++];
                            if (i == s.Length)
                                break;
                        }
                        string sss1 = new string(ssc);
                        //Console.WriteLine(sss1);
                        int n = Convert.ToInt32(sss1);
                        q.getn(n);
                    }
                    if (s[i + 1] == 'm')//表示设定统计的词组长度
                    {
                        char[] ssd = new char[100];
                        i += 3; int j = 0;
                        while (s[i] != ' ' && i < s.Length)
                        {
                            ssd[j++] = s[i++];
                            if (i == s.Length)
                                break;
                        }
                        string sss2 = new string(ssd);
                        //Console.WriteLine(sss2);
                        int m = Convert.ToInt32(sss2);
                        q.getm(m);
                    }
                }
        }
    }

    class Process//用于响应Run的调用完成用户输入的指令
    {
        string sout, sin, charcount, row;
        Dictionary<string, int> diction = new Dictionary<string, int>();
        public Process(Dictionary<string, int> diction, string s1, string s2, int nn, int mm)//获取处理好的dictionary和输入打印地址
        {
            this.diction = diction;
            sin = s1; sout = s2;
            charcount = Convert.ToString(nn); row = Convert.ToString(mm);
            string sss = "字符个数:" + charcount + "  有效行数:" + row;
            print(sss);
        }
        public Process(string s1, string s2)//获取输入打印地址
        {
            sin = s1; sout = s2;
        }
        public void inputlink(string s1)//获取读入的txt文件地址
        {
            sin = s1;
        }
        public void outputlink(string s1)//获取打印的txt文件地址
        {
            sout = s1;
        }
        public void getm(int m)//输出单词长度为m的单词及其频数
        {
            Dictionary<string, int>.Enumerator da = diction.GetEnumerator();
            for (int i = 0; i < diction.Count; i++)//枚举所有单词
                if (da.MoveNext())
                {
                    if (da.Current.Key.Length == m) //单词长度满足要求
                    {
                        string sssk = string.Format("单词：" + da.Current.Key + "    频数：" + da.Current.Value);
                        print(sssk);
                    }
                }
        }
        public void getn(int n)//输出单词频数前n的单词及其频数
        {
            Dictionary<string, int>.Enumerator en = diction.GetEnumerator();
            for (int i = 0; i < diction.Count && i < n; i++) //取得前n个，若dictionary中单词总数小于n则输出所有单词
                if (en.MoveNext())
                {
                    string ssst = string.Format("单词：" + en.Current.Key + "    频数：" + en.Current.Value);
                    print(ssst);
                }
        }
        public string gets()//读入txt文件中的所有字符
        {
            string s;
            s = File.ReadAllText(sin);
            return s;
        }
        private void print(string sss)
        {
            try//打印到txt文档
            {
                string path = sout;
                FileStream fs = new FileStream(path, FileMode.Append);//文本加入不覆盖
                StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.Default);//转码
                sw.WriteLine(sss);
                sw.Flush();//清空缓冲区
                sw.Close();//关闭流
                fs.Close();
            }
            catch (Exception)
            {
                Console.WriteLine("<script>('写入失败！')</script>");
            }
        }
    }


    class Init//初始化，预处理，把我们需要统计的查询的地方都处理出来
    {
        Dictionary<string, int> diction = new Dictionary<string, int>();//用于统计单词及其出现的频数，string表示单词,int表示单词出现频数
        private string s;//文档转化为string
        private int charcount, row;//charcount表示需要统计的字符数量，row表示不为空的行数
        public Init(string sk)
        {
            s = sk;
            charcount = 0; row = 0;
            get_s();
        }
        private void join(string s1)//把传入的单词进行处理
        {
            if (diction.ContainsKey(s1))//若该单词出现过则把该单词的数量+1
            {
                int k = diction[s1] + 1;
                diction[s1] = k;
            }
            else//若该单次没有出现过，则加入dictionary
            {
                diction.Add(s1, 1);
            }
        }
        private int getword(int i)//把单词从string中提取出来
        {
            int count = 0;//用于记录单词长度
            bool flag = true;//用于表示该单词是否符合要求
            char[] ss = new char[] {' ',' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ' };
            for (; i < s.Length; i++)
            {
                if ((int)s[i] > 127)
                    break;
                if ((s[i] >= '0' && s[i] <= '9') || (s[i] >= 'a' && s[i] <= 'z') || (s[i] >= 'A' && s[i] <= 'Z'))
                {
                    charcount++;//之前在进入judge时就让charcount++,但是出现了问题，当字符为汉字或者不满足条件的符号charcount也会+1；
                    if (s[i] >= 'A' && s[i] <= 'Z')
                        ss[count++] = (char)(s[i] + 'a' - 'A');
                    else
                        ss[count++] = s[i];
                    if (s[i] >= '0' && s[i] <= '9' && count < 4)//单词前四个字符的字母数量小于4，不满足条件
                        flag = false;
                }
                else
                    break;
            }
            if (flag && count >= 4)//满足题目对单词要求
            {
                string s1 = new string(ss);
                s1 = s1.Trim();
                join(s1);//把单词加入dictionary
            }
            return i;//返回现在对string处理到的位置
        }
        private void sort()//对dictionary按照单词出现频数排序
        {
            var dicSort = from objDic in diction orderby objDic.Value descending select objDic;
            foreach (KeyValuePair<string, int> kvp in dicSort) ;
        }
        private void get_s()
        {
            for (int i = 0; i < s.Length; i++)//处理string
            {
                if (s[i] < '0' && s[i] > '9' && s[i] < 'a' && s[i] > 'z' && s[i] < 'A' && s[i] > 'Z' && (int)s[i] <= 127)
                {
                    if (s[i] == ' ' || s[i] == '\n' || s[i] == '\t')//判断字符是否是题目要求的特定字符
                    {
                        charcount++;
                        if ((int)s[i] == 13 || (int)s[i] == 10)//遇见换行符判定该行是否为空行
                        {
                            //Console.WriteLine("nnnnnnnnnnnnn");
                            for (int j = i - 1; j >= 0; j--)
                            {
                                if ((int)s[j] == (char)13 || (int)s[i] == 10)//为空行，直接返回
                                    break;
                                else if (s[j] != '\t' && s[j] != ' ')//不为空行，有其他非空白字符行数row+1；
                                {
                                    //Console.WriteLine("sssssssssssss");
                                    row++;
                                    break;
                                }
                            }
                        }
                    }
                }
                else if (i < s.Length)
                {
                    i = getword(i);
                }
            }
            sort();//处理完成后对dictionary排序
        }
        public Dictionary<string, int> return_diction()//返回dictionary
        {
            return diction;
        }
        public int return_row()//返回有效行数
        {
            return row;
        }
        public int return_charcount()//返回有效字符
        {
            return charcount;
        }
    }
    class Program//主函数程序的起点
    {
        static void Main(string[] args)
        {
            Run a = new Run();//执行程序
        }
    }
}




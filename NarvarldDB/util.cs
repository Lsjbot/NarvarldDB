using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Excel = Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;       //Microsoft Excel 14 object in references-> COM tab


namespace NarvarldDB
{
    class util
    {
        public static int tryconvert(string word)
        {
            int i = -1;

            if (String.IsNullOrEmpty(word))
                return -1;

            try
            {
                i = Convert.ToInt32(word);
            }
            catch (OverflowException)
            {
                Console.WriteLine("i Outside the range of the Int32 type: " + word);
            }
            catch (FormatException)
            {
                if (!String.IsNullOrEmpty(word))
                    Console.WriteLine("i Not in a recognizable format: " + word);
                if (word.Contains(" "))
                    return tryconvert(word.Replace(" ", ""));
            }

            return i;

        }

        public static int? tryconvertnull(string word)
        {
            int? i = null;

            if (String.IsNullOrEmpty(word))
                return null;

            try
            {
                i = Convert.ToInt32(word);
            }
            catch (OverflowException)
            {
                Console.WriteLine("i Outside the range of the Int32 type: " + word);
            }
            catch (FormatException)
            {
                if (!String.IsNullOrEmpty(word))
                    Console.WriteLine("i Not in a recognizable format: " + word);
                if (word.Contains(" "))
                    return tryconvertnull(word.Replace(" ", ""));
            }

            return i;

        }

        public static int tryconvertbf(string word)
        {
            int i = -1;

            if (String.IsNullOrEmpty(word))
                return -1;
            else if (word == "-")
                return 0;
            else if (word == "..C")
                return 2;

            try
            {
                i = Convert.ToInt32(word);
            }
            catch (OverflowException)
            {
                //Console.WriteLine("i Outside the range of the Int32 type: " + word);
            }
            catch (FormatException)
            {
                //if (!String.IsNullOrEmpty(word))
                //    Console.WriteLine("i Not in a recognizable format: " + word);
                if (word.Contains(" "))
                    return tryconvert(word.Replace(" ", ""));
            }

            return i;

        }

        public static double tryconvertdouble(string word)
        {
            double i = -1;

            try
            {
                i = Convert.ToDouble(word);
            }
            catch (OverflowException)
            {
                Console.WriteLine("i Outside the range of the Double type: " + word);
            }
            catch (FormatException)
            {
                try
                {
                    i = Convert.ToDouble(word.Replace(".", ","));
                }
                catch (FormatException)
                {
                    Console.WriteLine("i Not in a recognizable double format: " + word.Replace(".", ","));
                }
                //Console.WriteLine("i Not in a recognizable double format: " + word);
            }

            return i;

        }

        public static List<string> get_filelist(string dir)
        {
            List<string> fl = new List<string>();

            string[] fs = Directory.GetFiles(dir);
            foreach (string f in fs)
                fl.Add(f);

            string[] ds = Directory.GetDirectories(dir);
            foreach (string subdir in ds)
                foreach (string f in get_filelist(subdir))
                    fl.Add(f);

            return fl;
        }

        public static string uniquefilename(string fn)
        {
            string fn1 = fn;
            int i = 0;
            while (File.Exists(fn1))
            {
                i++;
                fn1 = fn.Replace(".", i.ToString() + ".");
            }
            return fn1;
        }


        public static double getdouble(dynamic cell)
        {
            //Type unknown = ((ObjectHandle)cell.Value).Unwrap().GetType();
            //return unknown.ToString();

            double i = 0;

            if (cell == null)
                return 0;

            if (cell.Value == null)
                return 0;



            if (cell.Value.GetType() != i.GetType())
                return 0;

            return cell.Value;

            //try
            //{
            //    i = cell.Value;
            //}
            //catch (Exception e)
            //{
            //    return 0;
            //}

            //return i;
        }

        public static string getstring(Excel.Worksheet xll, int row, int col)
        {

            try
            {
                return getstring(xll.Cells[row, col]);
            }
            catch (Exception e)
            {
                return "";
            }

        }

        public static string getstring(dynamic cell)
        {

            if (cell == null)
                return "";

            if (cell.Value == null)
                return "";

            if (cell.Value.GetType() != "".GetType())
                return "";

            return cell.Value.Trim();
            //Type unknown = cell.Value.GetType();
            //return unknown.ToString();

            //try
            //{
            //    if (cell == null)
            //        return "";
            //    if (cell.Value == null)
            //        return "";
            //    return cell.Value.Trim();
            //}
            //catch (Exception e)
            //{
            //    return "";
            //}


        }

        public static DateTime getdate(dynamic cell, DateTime defaulttime)
        {

            if (cell == null)
                return defaulttime;

            try
            {
                if (cell == null)
                    return defaulttime;
                if (cell.Value == null)
                    return defaulttime;
                return cell.Value;
            }
            catch (Exception e)
            {
                return defaulttime;
            }


        }

        public static string cleanfilename(string filename)
        {
            char[] nono ="<>:\"/\\|?*".ToCharArray();

            string fn = filename;
            foreach (char c in nono)
                if (fn.Contains(c))
                    fn = fn.Replace(c, '-');
            return fn;
        }

        public static Dictionary<string, List<string>> schoolsubjdict =
            new Dictionary<string, List<string>>()
            {
{"äbild",new List<string>(){"bild","bd","bildpedagogik"}},
{"äbio",new List<string>(){"biologi","bi","biologididaktik"}},
{"äeng",new List<string>(){"engelska","en","eng"}},
{"äfr",new List<string>(){"franska","fr"}},
{"äfy",new List<string>(){"fysik","fy","fysikdidaktik"}},
{"ägeo",new List<string>(){"geografi","ge"}},
{"ähi",new List<string>(){"historia","hi"}},
{"äidrott",new List<string>(){"idrott","id"}},
{"äkemi",new List<string>(){"kemi","ke","kemididaktik"}},
{"äma",new List<string>(){"matematik","ma","matematikdidaktik","matematisk"}},
{"ämoder",new List<string>(){"modersmål","moder"}},
{"ämu",new List<string>(){"musik","mu"}},
{"änk",new List<string>(){"naturkunskap","nk","naturkunskapsdidaktik"}},
{"ärel",new List<string>(){"religion","re","religionskunskap","religionsvetenskap"}},
{"äsam",new List<string>(){"samhällskunskap","sk","sh"}},
{"äsp",new List<string>(){"spanska","sp"}},
{"äsvas",new List<string>(){"svenska som andraspråk","svas","andraspråk"}},
{"äsv",new List<string>(){"svenska","sv"}},
{"ätk",new List<string>(){"teknik","t"}},
{"äty",new List<string>(){"tyska","ty"}},
{"äx",new List<string>(){"övrigt","xx"}}

            };
            
        public static SortedDictionary<int,string> parse_amneslarare(string namepar)
        {
            string name = namepar.ToLower();
            char[] trimchars = new char[] { ',', '(', ')', '.',':' };
            char[] splitchars = new char[] { ' ', ',','/','-'};
            SortedDictionary<int,string> dict = new SortedDictionary<int,string>();

            if (name.Contains('/'))
            {
                Console.WriteLine("/");
            }
            string[] words = name.ToLower().Split(splitchars);
            List<string> list = new List<string>();
            foreach (string word in words)
            {
                list.Add(word.Trim(trimchars));
            }

            string svas = "svenska som a"; //ofta trunkerat
            string sv = "svenska";

            foreach (string s1 in schoolsubjdict.Keys)
            {
                if (s1 == "äsv")
                {
                    if (!name.Contains(sv))
                        continue;
                    else if (name.Contains(svas))
                    {
                        int ksvas = name.IndexOf(svas);
                        int k1 = name.IndexOf(sv);
                        int k2 = name.LastIndexOf(sv);
                        if (k1 != ksvas)
                            dict.Add(k1, s1);
                        else if (k2 != ksvas)
                            dict.Add(k2, s1);
                    }
                    else
                    {
                        int k = name.IndexOf("svenska");
                        dict.Add(k, s1);
                    }
                }
                else if (s1 == "äsvas")
                {
                    if (name.Contains(svas))
                    {
                        int k = name.IndexOf(svas);
                        dict.Add(k, s1);
                    }
                }
                else if (s1 == "äsam")
                {
                    if (name.Contains("samhällsk")) //ofta trunkerat
                    {
                        int k = name.IndexOf("samhällsk");
                        dict.Add(k, s1);
                    }
                    else
                    {
                        foreach (string s2 in schoolsubjdict[s1])
                        {
                            if (list.Contains(s2))
                            {
                                int k = name.IndexOf(s2);
                                if (!dict.ContainsKey(k))
                                    dict.Add(k, s1);
                                break;
                            }
                        }
                    }
                }
                else
                {
                    foreach (string s2 in schoolsubjdict[s1])
                    {
                        if (list.Contains(s2))
                        {
                            int k = name.IndexOf(s2);
                            if (!dict.ContainsKey(k))
                                dict.Add(k, s1);
                            break;
                        }
                    }
                }
            }
            if (dict.Count == 0)
                dict.Add(0, "äx");
            return dict;
        }

    }
}

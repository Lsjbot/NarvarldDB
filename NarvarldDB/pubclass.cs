using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json.Linq;

namespace NarvarldDB
{
    public class pubclass
    {
        public string title = "";
        public string language = "";
        public int year = 0;
        public List<authorclass> authors = new List<authorclass>();
        public bool hassweuni = false;
        public int nsweuni = 0;
        public bool hasotheruni = false;
        public bool hasother = false;
        int nnoaff = 0;
        public int naff = 0;
        //bool hasdualaff = false;
        public List<int> sweuni = new List<int>();
        bool affsfixed = false;
        public string svep = "";
        public string genre = "";
        public List<string> subjectlist = new List<string>();
        public static List<affclass> afflist = new List<affclass>();
        public static Dictionary<int, string> unidict = new Dictionary<int, string>();
        public static Dictionary<char, string> subjnamedict = new Dictionary<char, string>() { { ' ', ""},{'0',"Forskningsämnesområde saknas" }, { '1', "Naturvetenskap" }, { '2', "Teknik" }, { '3', "Medicin och hälsovetenskap" }, { '4', "Lantbruksvetenskap och veterinärmedicin" }, { '5', "Samhällsvetenskap" }, { '6', "Humaniora och konst" } };

        public static int startyear = 2000;
        public static int endyear = 2020;
        public static int badyear = 2030;

        public static List<string> pubtypelist = new List<string>(){
            "",
            "conference",
            "conference/other",
            "conference/paper",
            "conference/poster",
            "conference/proceeding",
            "publication",
            "publication/book:swe",
            "publication/book:other",
            "publication/book-chapter",
            "publication/book-review",
            "publication/critical-edition",
            "publication/doctoral-thesis",
            "publication/edited-book",
            "publication/editorial-letter",
            "publication/encyclopedia-entry",
            "publication/foreword-afterword",
            "publication/journal-article:ref",
            "publication/journal-article:nonref",
            "publication/journal-issue",
            "publication/licentiate-thesis",
            "publication/magazine-article",
            "publication/newspaper-article",
            "publication/other",
            "publication/preprint",
            "publication/report",
            "publication/report-chapter",
            "publication/review-article",
            "publication/working-paper"
            };

        public string get_genre()
        {
            if (this.genre == "publication/book")
            {
                if (this.language == "swe")
                    return this.genre + ":swe";
                else
                    return this.genre + ":other";
            }
            if (this.genre == "publication/journal-article")
            {
                if (this.svep.Contains("ref"))
                {
                    return this.genre + ":ref";
                }
                else
                    return this.genre + ":nonref";
            }
            if (pubtypelist.Contains(this.genre))
                return this.genre;

            return null;
        }

        public static void readuni(string fn)
        {
            Console.WriteLine("Reading universities from " + fn);
            int n = 0;
            using (StreamReader sr = new StreamReader(fn))
            {
                //sr.ReadLine();
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    string[] words = line.Split('\t');
                    if (words.Length < 3)
                        continue;
                    unidict.Add(util.tryconvert(words[1]), words[0]);
                    //unitypedict.Add(util.tryconvert(words[1]), words[2]);

                    n++;
                }
            }
            Console.WriteLine(n + " universities read.");

        }


        static affclass getaff(string namepar)
        {
            affclass aff = (from c in afflist where c.name == namepar select c).FirstOrDefault();
            if (aff != null)
            {
                if (aff.partof > 0)
                {
                    aff = (from c in afflist where c.id == aff.partof select c).FirstOrDefault();
                }
                else if (aff.aliasof > 0)
                {
                    aff = (from c in afflist where c.id == aff.aliasof select c).FirstOrDefault();
                }
                return aff;
            }

            return null;

        }

        static string topaffiliation(JToken jaff)
        {
            string s = "";
            if (jaff["hasAffiliation"] != null)
            {
                foreach (JToken jaff2 in jaff["hasAffiliation"])
                {
                    if (jaff2["hasAffiliation"] != null)
                    {
                        return topaffiliation(jaff2);
                    }
                    else if (jaff2["name"] != null)
                        s = jaff2["name"].ToString();

                }
            }
            else if (jaff["name"] != null)
                s = jaff["name"].ToString();
            return s;
        }



        public static void readaffs(string fn)
        {
            Console.WriteLine("Reading affiliations from " + fn);
            int n = 0;
            using (StreamReader sr = new StreamReader(fn))
            {
                sr.ReadLine();
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    string[] words = line.Split('\t');
                    affclass aff = new affclass();
                    aff.id = util.tryconvert(words[0]);
                    if (aff.id < 0)
                        continue;
                    aff.name = words[1];
                    aff.orgtype = util.tryconvert(words[3]);
                    if (words.Length > 4)
                        if (!String.IsNullOrEmpty(words[4]))
                            aff.partof = util.tryconvert(words[4]);
                    if (words.Length > 5)
                        if (!String.IsNullOrEmpty(words[5]))
                            aff.aliasof = util.tryconvert(words[5]);
                    afflist.Add(aff);
                    n++;
                }
            }
            Console.WriteLine(n + " affiliations read.");
        }



        public int getnnoaff()
        {
            return nnoaff;
        }

        public bool hasfocus(List<int> focuslist)
        {
            if (focuslist.Count == 0)
                return true;
            foreach (int f in focuslist)
                if (hasfocus(f))
                    return true;
            return false;
        }

        public bool hasfocus(int focusuni)
        {
            if (focusuni > 0)
            {
                return sweuni.Contains(focusuni);
            }
            else
                return true;
        }

        public void fillhists(hbookclass hist)
        {
            if (this.authors.Count == 1)
            {
                hist.Add("Ensamförfattare");
            }
            else if (this.nsweuni > 1)
            {
                hist.Add("Flera svenska lärosäten");
                if (this.hasotheruni)
                {
                    if (this.hasother)
                        hist.Add("Extern, utländskt lärosäte och annat svenskt lärosäte");
                    else
                        hist.Add("Både utländskt och annat svenskt lärosäte");
                }
            }
            else if (this.hasotheruni)
            {
                if (this.hasother)
                    hist.Add("Både extern och utländskt lärosäte");
                else
                    hist.Add("Utländskt lärosäte");
            }
            else if (this.hasother)
                hist.Add("Extern, ej lärosäte");
            else
                hist.Add("Enbart intern samverkan");


        }

        public void fixaffs()
        {
            if (affsfixed) //call once only
                return;
            affsfixed = true;

            foreach (authorclass au in authors)
            {
                if (au.aff.Count == 0)
                    this.nnoaff++;
                else
                {
                    this.naff++;
                    foreach (affclass affc in au.aff)
                    {
                        switch (affc.orgtype)
                        {
                            case affclass.sweuni:
                                hassweuni = true;
                                if (!sweuni.Contains(affc.id))
                                    sweuni.Add(affc.id);
                                break;
                            case affclass.otheruni:
                                hasotheruni = true;
                                break;
                            case affclass.other:
                                hasother = true;
                                break;
                            case affclass.reslab:
                                hasother = true;
                                break;
                        }
                    }
                }
            }
            if (naff + nnoaff != authors.Count)
                Console.WriteLine("au,naff,nnoaff: " + authors.Count + ", " + naff + ", " + nnoaff);
            nsweuni = sweuni.Count;
        }

        static int getyear(JToken jp)
        {
            int year = badyear;
            if (jp != null)
            {
                //JToken jp = jj["master"]["provisionActivity"];
                foreach (JToken jpa in jp)
                {
                    if (jpa["date"] == null)
                    {
                        //string fnex = @"d:\Downloads\Swepub\nodate.json";
                        //using (StreamWriter sw = new StreamWriter(freefilename(fnex)))
                        //    sw.WriteLine(line);
                    }
                    else
                    {
                        if (jpa["date"].ToString().Length > 4)
                            try
                            {
                                year = DateTime.Parse(jpa["date"].ToString()).Year;
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message + " " + jpa["date"].ToString());
                            }
                        else
                            year = util.tryconvert(jpa["date"].ToString());
                        //Console.WriteLine(jpa["@type"] + "\t" + jpa["date"] + "\t" + year);
                    }
                }
                //yearhist.Add(year);
            }
            return year;
        }



        public static pubclass parseJSON(string jline)
        {
            pubclass pub = new pubclass();
            JObject jj = JObject.Parse(jline);
            JToken jp = jj["master"]["publication"];
            if (jp == null)
                jp = jj["master"]["provisionActivity"];

            int year = getyear(jp);

            pub.year = year;

            if (year < startyear)
                return null;
            if (year > endyear)
                return null;
            if (year == badyear)
                return null;

            JToken jm = jj["master"]["instanceOf"];

            foreach (JToken jgen in jm["hasTitle"].Children())
            {
                if (jgen["mainTitle"] != null)
                {
                    string tit = jgen["mainTitle"].ToString();
                    pub.title = tit;
                }
            }

            int nsvep = 0;
            int npub = 0;
            foreach (JToken jgen in jm["genreForm"].Children())
            {
                if (jgen["@id"] == null)
                    continue;
                //Console.WriteLine(jgen["@id"].ToString());
                string gf = jgen["@id"].ToString().Replace("https://id.kb.se/term/swepub/", "");
                if (gf.StartsWith("svep"))
                {
                    pub.svep = gf.Replace("svep/", "");
                    nsvep++;
                }
                if (gf.StartsWith("publication") || gf.StartsWith("conference"))
                {
                    pub.genre = gf;
                    npub++;
                }
                //genformhist.Add(gf);
            }
            //nsvephist.Add(nsvep);
            //pubconfhist.Add(npub);

            List<char> subjlist = new List<char>();
            foreach (JToken jsub in jm["subject"].Children())
            {
                if (jsub["@id"] == null)
                    continue;
                if (!jsub["@id"].ToString().Contains("id.kb.se"))
                    continue;
                string preflabel = "(no label)";
                if (jsub["prefLabel"] != null)
                    preflabel = jsub["prefLabel"].ToString();
                //string code = "(no code)";
                if (jsub["code"] != null)
                {
                    char csubj = jsub["code"].ToString()[0];
                    if (!subjlist.Contains(csubj))
                        subjlist.Add(csubj);
                }
            }

            if (subjlist.Count == 0)
                subjlist.Add('0');
            foreach (char c in subjlist)
                pub.subjectlist.Add(subjnamedict[c]);


            foreach (JToken jaut in jm["contribution"].Children())
            {
                Dictionary<string, JToken> autdict = jaut.ToObject<Dictionary<string, JToken>>();
                //bool hasaff = false;
                authorclass au = new authorclass();
                foreach (string s in autdict.Keys)
                {
                    if (s == "agent")
                    {
                        if (jaut["agent"]["@type"].ToString() == "Person")
                        {
                            //Console.WriteLine(jaut["agent"]["familyName"].ToString() + ", " + jaut["agent"]["givenName"].ToString());
                            if (jaut["agent"]["familyName"] != null)
                                au.familyname = jaut["agent"]["familyName"].ToString();
                            if (jaut["agent"]["givenName"] != null)
                                au.firstname = jaut["agent"]["givenName"].ToString();
                            //naut++;
                        }
                    }
                    else if (s == "hasAffiliation")
                    {
                        foreach (JToken jaff in jaut["hasAffiliation"])
                        {
                            string ss = topaffiliation(jaff);
                            affclass affc = getaff(ss);
                            if (affc != null)
                            {
                                au.aff.Add(affc);
                                //hasaff = true;
                            }
                        }
                    }
                }
                pub.authors.Add(au);
            }


            pub.fixaffs();

            foreach (JToken jlang in jm["language"].Children())
            {
                string lang = jlang["code"].ToString();
                //langhist.Add(lang);
                pub.language = lang;
            }

            return pub;
        }


    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using aviationLib;

namespace awyLoader
{
    class Fixs
    {
        public String id;
        public String fixId;

        public Fixs(String id, String fixId)
        {
            this.id = id;
            this.fixId = fixId;
        }
    }

    class Navaids
    {
        public String id;
        public String navId;

        public Navaids(String id, String navId)
        {
            this.id = id;
            this.navId = navId;
        }
    }

    class Program
    {
        static Char[] recordType_001_04 = new Char[04];

        static String magCourse_57_03;
        static String magCourseOpposite_63_03;

        static String MEA_75_05;
        static String MAX_97_05;
        static String MOCA_102_05;

        static StreamWriter ofileAWY1 = new StreamWriter("airway.txt");

        static StreamReader awyFixs = new StreamReader("awyFixs.txt");

        static StreamReader awyNavaids = new StreamReader("awyNavaids.txt");

        static List<Fixs> fixs = new List<Fixs>();

        static List<Navaids> navaids = new List<Navaids>();

        static void Main(String[] args)
        {
            String rec = awyFixs.ReadLine();

            while (!awyFixs.EndOfStream)
            {
                String[] f = rec.Split('~');

                Fixs o = new Fixs(f[0], f[1]);

                fixs.Add(o);

                rec = awyFixs.ReadLine();
            }

            String[] ff = rec.Split('~');

            Fixs oo = new Fixs(ff[0], ff[1]);

            fixs.Add(oo);

            awyFixs.Close();

            rec = awyNavaids.ReadLine();

            while (!awyNavaids.EndOfStream)
            {
                String[] f = rec.Split('~');

                Navaids o = new Navaids(f[0], f[1]);

                navaids.Add(o);

                rec = awyNavaids.ReadLine();
            }

            String[] f1 = rec.Split('~');

            Navaids o1 = new Navaids(f1[0], f1[1]);

            navaids.Add(o1);

            awyNavaids.Close();

            String userprofileFolder = Environment.GetEnvironmentVariable("USERPROFILE");
            String[] fileEntries = Directory.GetFiles(userprofileFolder + "\\Downloads\\", "28DaySubscription*.zip");

            ZipArchive archive = ZipFile.OpenRead(fileEntries[0]);
            ZipArchiveEntry entry = archive.GetEntry("AWY.txt");
            entry.ExtractToFile("AWY.txt", true);

            StreamReader file = new StreamReader("AWY.txt");

            
            rec = file.ReadLine();

            while (!file.EndOfStream)
            {
                ProcessRecord(rec);
                
                rec = file.ReadLine();
            }

            ProcessRecord(rec);

            
            file.Close();

            ofileAWY1.Close();
        }

        static void ProcessRecord(String record)
        {
            recordType_001_04 = record.ToCharArray(0, 4);

            String rt = new String(recordType_001_04);

            Int32 r = String.Compare(rt, "AWY1");
            
            if (r == 0)
            {
                magCourse_57_03 = new String(record.ToCharArray(56, 3)).Trim();
                magCourseOpposite_63_03 = new String(record.ToCharArray(62, 3)).Trim();

                MEA_75_05 = new String(record.ToCharArray(74, 5)).Trim();
                MAX_97_05 = new String(record.ToCharArray(96, 5)).Trim();
                MOCA_102_05 = new String(record.ToCharArray(101, 5)).Trim();
            }

            r = String.Compare(rt, "AWY2");
            
            if (r == 0)
            {
                Char[] cr = record.ToCharArray(45, 1);
                
                if ((cr[0] == 'V') || (cr[0] == 'R') || (cr[0] == 'A') || (cr[0] == 'N') || (cr[0] == 'W'))
                {
                    ofileAWY1.Write(new String(record.ToCharArray(4, 4)).Trim());
                    
                    if(String.Compare(new String(record.ToCharArray(9, 1)), " ") != 0)
                    {
                        ofileAWY1.Write(new String(record.ToCharArray(9, 1)).Trim());
                    }
                    
                    ofileAWY1.Write('~');

                    ofileAWY1.Write(new String(record.ToCharArray(4, 1)).Trim());
                    ofileAWY1.Write('~');

                    Int32 i = Convert.ToInt32((new String(record.ToCharArray(10, 5)).Trim()));
                    
                    ofileAWY1.Write(i.ToString("D05"));
                    ofileAWY1.Write('~');

                    String c = new String(record.ToCharArray(125, 33)).Trim();
                    
                    String[] s = c.Split('*');

                    if(s[0] == "A")
                    {
                        if(s[1].Length < 5)
                        {
                            ofileAWY1.Write(s[1]);
                            ofileAWY1.Write('~');
                            ofileAWY1.Write(new String(record.ToCharArray(45, 19)).Trim());

                            Navaids nav = LookupNavaid(s[1]);

                            if (nav == null)
                            {
                                ofileAWY1.Write('~');
                            }
                            else
                            {
                                ofileAWY1.Write('~');
                                ofileAWY1.Write(nav.id);
                            }
                        }
                        else
                        {
                            ofileAWY1.Write(s[1]);
                            ofileAWY1.Write('~');
                            ofileAWY1.Write(new String(record.ToCharArray(81, 2)).Trim());

                            Fixs fix = LookupFix(s[1]);

                            if (fix == null)
                            {
                                ofileAWY1.Write('~');
                            }
                            else
                            {
                                ofileAWY1.Write('~');
                                ofileAWY1.Write(fix.id);
                            }
                        }
                    }
                    else if (cr[0] == 'N')
                    {
                        ofileAWY1.Write(s[1]);
                        ofileAWY1.Write('~');
                        ofileAWY1.Write(new String(record.ToCharArray(45, 19)).Trim());

                        Navaids nav = LookupNavaid(s[1]);

                        if (nav == null)
                        {
                            ofileAWY1.Write('~');
                        }
                        else
                        {
                            ofileAWY1.Write('~');
                            ofileAWY1.Write(nav.id);
                        }
                    }
                    else if (cr[0] == 'R')
                    {
                        ofileAWY1.Write(s[1]);
                        ofileAWY1.Write('~');
                        ofileAWY1.Write(new String(record.ToCharArray(81, 2)).Trim());

                        Fixs fix = LookupFix(s[1]);

                        if (fix == null)
                        {
                            ofileAWY1.Write('~');
                        }
                        else
                        {
                            ofileAWY1.Write('~');
                            ofileAWY1.Write(fix.id);
                        }
                    }
                    else
                    {
                        ofileAWY1.Write(s[1]);
                        ofileAWY1.Write('~');
                        ofileAWY1.Write(new String(record.ToCharArray(45, 19)).Trim());

                        Navaids nav = LookupNavaid(s[1]);

                        if (nav == null)
                        {
                            ofileAWY1.Write('~');
                        }
                        else
                        {
                            ofileAWY1.Write('~');
                            ofileAWY1.Write(nav.id);
                        }
                    }

                    ofileAWY1.Write('~');

                    ofileAWY1.Write(magCourse_57_03);
                    ofileAWY1.Write('~');

                    ofileAWY1.Write(magCourseOpposite_63_03);
                    ofileAWY1.Write('~');

                    ofileAWY1.Write(MEA_75_05);
                    ofileAWY1.Write('~');

                    ofileAWY1.Write(MAX_97_05);
                    ofileAWY1.Write('~');

                    ofileAWY1.Write(MOCA_102_05);
                    ofileAWY1.Write('~');

                    LatLon ll = new LatLon(new String(record.ToCharArray(83, 14)).Trim(), new String(record.ToCharArray(97, 14)).Trim());
                    ofileAWY1.Write(ll.formattedLat);
                    ofileAWY1.Write('~');

                    ofileAWY1.Write(ll.formattedLon);
                    ofileAWY1.Write(ofileAWY1.NewLine);
                }

            }

        }

        public static Fixs LookupFix(String ident)
        {
            foreach (Fixs f in fixs)
            {
                if (f.fixId == ident)
                {
                    return f;
                }
            }

            return null;
        }

        public static Navaids LookupNavaid(String ident)
        {
            foreach (Navaids n in navaids)
            {
                if (n.navId == ident)
                {
                    return n;
                }
            }

            return null;
        }

    }
}

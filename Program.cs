using System;
using System.Collections.Generic;
using System.IO;

namespace Archive
{
    class Program
    {
        static string AchDir = "";

        static void Main(string[] args)
        {
            AchDir = GetArchiveDir();
            if (!Directory.Exists(AchDir))
                Environment.Exit(1);

            string[] fileArray = Directory.GetFiles(AchDir, "*.dat");

            if (fileArray.Length < 1)
                Environment.Exit(1);

            MoveArchiveToOld(fileArray);
        }

        internal static void MoveArchiveToOld(string[] fileArray)
        {
            foreach (string file in fileArray)
            {
                var list = ReadFileToList(file);
                foreach(var line in list)
                {
                    var split = line.Split('=');
                    if (split[0] == "ModifiedTime")
                    {
                        string modTime = split[1];
                        if (OrderOlderThan1Year(modTime))
                        {
                            var splitPath = file.Split('\\');
                            var datName = splitPath[splitPath.Length-1];
                            if(Directory.Exists(AchDir + "\\Old\\"))
                                new FileInfo(AchDir + "\\Old\\").Directory.Create();

                            File.Move(file, AchDir + "\\Old\\" + datName);
                        }
                    }
                }
            }
        }

        private static bool OrderOlderThan1Year(string modTime)
        {
            string fDate = modTime.Substring(0, 4) + "-" + modTime.Substring(4, 2) + "-" + modTime.Substring(6, 2);
            DateTime fileDate = DateTime.ParseExact(fDate, "yyyy-MM-dd", null);
            DateTime curDate = DateTime.Now;
            var diff = (curDate - fileDate).TotalDays;
            if (diff > 365)
                return true;

            return false;
        }

        public static List<string> ReadFileToList(string filePath)
        {
            var aReadFile = File.ReadAllLines(filePath);
            var lineList = new List<string>(aReadFile);
            return lineList;
        }

        internal static string GetArchiveDir()
        {
            string scriptpath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            if (!File.Exists(scriptpath))
                scriptpath = "C:\\DonsScripts\\H O T S";

            string iniPath = scriptpath + "\\Hite Order Tracking Config.ini";

            var lineList = ReadFileToList(iniPath);

            foreach (string line in lineList)
            {
                var split = line.Split('=');
                if (split[0] == "PathArchivedData")
                    return split[1];
               
            }
            return "";
        }
    }
}


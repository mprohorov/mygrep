using System;
using System.IO;
using System.Text;

namespace mygrep
{
    class mygrep
    {
        static bool byname;
        static bool rflag;
        static string search;
        static string dir;
        
        static void printUsage() 
        {
            //prints a usage statement in case of bad input
            Console.WriteLine("usage: grep [-name] [-r] SearchString StartingDirectory");
        }
        static void parseArgs(string[] args) 
        {
            //processes the argument set, and verifies it is a legal command
            if (args.Length > 4)
            {
                printUsage();
            }
            else if (args.Length == 2)
            {
                search = args[0];
                dir = args[1];
            }
            else if(args.Length == 3) 
            {
                if (args[0].Equals("-name"))
                {
                    if(args[1].Equals("-r") || args[2].Equals("-r")) 
                    {
                        printUsage();
                    }
                    else 
                    {
                        byname = true;
                        search = args[1];
                        dir = args[2];
                    }
                }
                else if (args[0].Equals("-r")) 
                {
                    if (args[1].Equals("-name") || args[2].Equals("-name"))
                    {
                        printUsage();
                    }
                    else
                    {
                        rflag = true;
                        search = args[1];
                        dir = args[2];
                    }
                }
            }
            else if(args.Length == 4 && args[0].Equals("-name") && args[1].Equals("-r")) 
            {
                byname = true;
                rflag = true;
                search = args[2];
                dir = args[3];
            }
            else 
            {
                printUsage();
            }
        }

        static void findFilenamesAll(string search, string startdir)
        {
            //grab all subdirectories in the current directory
            string[] subdirs = Directory.GetDirectories(startdir);

            //Gets all matches in the current directory
            findFilenamesCurrDir(search, startdir);

            //Loops through all subdirectories and use recursive call to match all subdirectory files
            foreach (string s in subdirs)
            {
                findFilenamesAll(search, s);
            }
        }
        static void findFilenamesCurrDir(string search, string startdir)
        {
            //Checks what files in the current directory matches the search parameter.
            string[] files = Directory.GetFiles(startdir);
            foreach (string path in files)
            {
                FileInfo fi = new FileInfo(path);
                if (fi.Name.Contains(search)) 
                {
                    Console.Write(fi.FullName);
                    Console.Write(": " + fi.Length + ", " + fi.CreationTime + "\n");
                }
            }
        }
        static void searchAll (string search, string startdir)
        {
            string[] subdirs = Directory.GetDirectories(startdir);

            searchFilesinDir(search, startdir);
            foreach (string s in subdirs) 
            {
                searchAll(search, s);
            }

        }
        static void searchFilesinDir(string search, string startdir)
        {
            //Reads all files in the current directory line by line and outputs all the ones that contain the search parameter.
            string [] files = Directory.GetFiles(startdir);

            foreach(string path in files)
            {
                FileInfo fi = new FileInfo(path);
                int lineno = 1;

                using var fs = new FileStream(path, FileMode.Open, FileAccess.Read);
                using (StreamReader sr = new StreamReader(fs, Encoding.UTF8))
                {
                    string line = String.Empty;

                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.Contains(search)) 
                        {
                            Console.Write(fi.FullName + "(" + lineno + ")");
                            Console.Write(": " + fi.Length + ", " + fi.CreationTime + "\n");
                        }
                        lineno++;
                    }
                }
            }


        }

        static void Main(string[] args)
        {
            Console.WriteLine("Starting mygrep...");
     
            parseArgs(args);

            if (byname && rflag)
            {
                findFilenamesAll(search, dir);
            }
            else if (byname && !rflag)
            {
                findFilenamesCurrDir(search, dir);
            }
            else if (rflag && !byname)
            {
                searchAll(search, dir);
            }
            else
            {
                searchFilesinDir(search, dir);
            }



            
            
        }
    }
}

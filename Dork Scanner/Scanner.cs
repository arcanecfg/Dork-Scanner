/*
 *        d8888                                                .d888         
      d88888                                               d88P"          
     d88P888                                               888            
    d88P 888888d888 .d8888b 8888b. 88888b.  .d88b.  .d8888b888888 .d88b.  
   d88P  888888P"  d88P"       "88b888 "88bd8P  Y8bd88P"   888   d88P"88b 
  d88P   888888    888     .d888888888  88888888888888     888   888  888 
 d8888888888888    Y88b.   888  888888  888Y8b.    Y88b.   888   Y88b 888 
d88P     888888     "Y8888P"Y888888888  888 "Y8888  "Y8888P888    "Y88888 
                                                                      888 
                                                                 Y8b d88P 
                                                                  "Y88P"  
Dork Scanner v1.0
 * www.WastedWolf.com
 * www.YouTube.com/Arcanecfg
 * 03/05/2016
 * */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Net;


namespace Dork_Scanner
{

    class Scanner
    {
        static string targetURL;
        static void Main(string[] args)
        {
            string dorkPath = string.Empty;
            string[] dorkList = { string.Empty };
            targetURL = string.Empty;
               
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Welcome to Dork Scanner v1.0 - Arcanecfg \n");
            Console.ForegroundColor = ConsoleColor.White;
            ServicePointManager.Expect100Continue = false;
            //Put the dorks in a file called "dorks.txt" in the program's directory.     
            dorkPath = "dorks.txt";
            try
            {
                if (File.Exists(dorkPath))
                {
                    dorkList = File.ReadAllLines(dorkPath);
                    Console.WriteLine("Successfully loaded " + dorkList.Count() + " dorks.");
                }
                else
                {
                    Console.Write("Dork list could not be located, enter path to list: ");
                    dorkPath = Console.ReadLine();
                    if (File.Exists(dorkPath))
                    {
                        dorkList = File.ReadAllLines(dorkPath);
                        Console.WriteLine("Succesfully loaded " + dorkList.Count() + " dorks.");
                        //Remove '/' from the beginning of dorks.
                        for (int i = 0; i < dorkList.Count(); i++)
                        {
                            if (dorkList[i][0] == '/')
                            {
                                dorkList[i] = dorkList[i].Substring(1);
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("The path you entered was invalid.");
                    }
                }
                Console.Write("Target URL (terminated by /): ");
                targetURL = Console.ReadLine();
                Directory.CreateDirectory(targetURL.ToString().Replace("http://","").Replace("/",""));
                //Console.Write("Threads: ");
                //string threadVal = Console.ReadLine();
                //startScan(dorkList,targetURL);
                List<Uri> finalList = new List<Uri>();

                for (int i = 0; i < dorkList.Count(); i++)
                {
                    finalList.Add(new Uri(targetURL + dorkList[i]));
                }
                startScan(finalList);
            }

            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("An error occured~ " + ex.Message);
                Console.ForegroundColor = ConsoleColor.White;
                Console.ReadKey();
            }   
        }
        static void startScan(List<Uri> finalList)
        {
            List<string> goodList = new List<string>();
            List<string> badList = new List<string>();

                Parallel.ForEach(finalList, curURL =>
                {
                    try
                    {
                        WebRequest newReq = HttpWebRequest.Create(curURL);
                        HttpWebResponse newRes = newReq.GetResponse() as HttpWebResponse;
                        string myResp;
                        StreamReader sReader = new StreamReader(newRes.GetResponseStream());
                        myResp = sReader.ReadToEnd();
                        if (myResp.Contains("404") == false)
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("Valid URL @~ " + curURL);
                            Console.ForegroundColor = ConsoleColor.White;
                            goodList.Add(curURL.ToString());
                        }
                        else
                        {
                            Console.WriteLine("404 occured.");
                            badList.Add(curURL.ToString());
                        }
                    }
                    catch 
                    {
                        Console.WriteLine("Invalid @~ " + curURL);
                        badList.Add(curURL.ToString());
                    }

                });
                Console.ForegroundColor = ConsoleColor.Yellow;
                File.WriteAllLines(targetURL.ToString().Replace("http://","").Replace("/","") + @"\Good.txt", goodList);
                File.WriteAllLines(targetURL.ToString().Replace("http://", "").Replace("/", "") + @"\Bad.txt", badList);
                Console.WriteLine("Done scanning the URLs - Results saved in folder.");
                Console.ReadKey();
            }
            
        }
    }



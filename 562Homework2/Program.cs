//Created by Edward Brunton
//2/19/18
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace _562Homework2
{
    public class BoolValues
    {
        Results resultsPart;
        bool cacheTested;
        bool setTested;
        bool lineTested;

        public BoolValues(Results resultsPart)
        {
            this.ResultsPart = resultsPart;
            CacheTested = false;
            SetTested = false;
            LineTested = false;
        }

        public bool CacheTested { get => cacheTested; set => cacheTested = value; }
        public bool SetTested { get => setTested; set => setTested = value; }
        public bool LineTested { get => lineTested; set => lineTested = value; }
        public Results ResultsPart { get => resultsPart; set => resultsPart = value; }
    }
    public class Results
    {
        private int testNumber;
        string benchMark;
        string name;
        double IPC;
        double dCacheMissRate;
        int iCache;
        int dCache;
        int setAssociative;
        int lineSize;
        double IPCdifference;
        double dCachedifference;
        public Results(int testNumber) => this.testNumber = testNumber;
        public int TestNumber { get => testNumber; set => testNumber = value; }
        public double DCacheMissRate { get => dCacheMissRate; set => dCacheMissRate = value; }
        public string Name { get => name; set => name = value; }
        public int ICache { get => iCache; set => iCache = value; }
        public int DCache { get => dCache; set => dCache = value; }
        public double IPC1 { get => IPC; set => IPC = value; }
        public string BenchMark { get => benchMark; set => benchMark = value; }
        public int SetAssociative { get => setAssociative; set => setAssociative = value; }
        public int LineSize { get => lineSize; set => lineSize = value; }
        public double IPCdifference1 { get => IPCdifference; set => IPCdifference = value; }
        public double DCachedifference { get => dCachedifference; set => dCachedifference = value; }
    }
    class ResultsComputer
    {
        List<Results> listOfResults;
        Results bestIPCR;
        Results bestdCache;
        Results baseR;
        string BenchMark()
        {
            return baseR.BenchMark;
        }

        double BaseIPC()
        {
            if (baseR is null)
            {
                Console.WriteLine("Error, base does not exist when attempting to access baseIPC");
                return -1.0;
            }
            return baseR.IPC1;
        }
        double BestIPC() => bestIPCR.IPC1;
        double IPCImpovement()
        {
            return 100 * bestIPCR.IPCdifference1;
        }
        double DCachImprovement()
        {
            return 100 * bestdCache.DCachedifference;
        }
        string BestIPCConfig()
        {
            //   return bestIPCR.DCache + "-" + bestIPCR.SetAssociative + "-" + bestIPCR.LineSize + "-" + bestIPCR.ICache;
            return bestIPCR.DCache + "-" + bestIPCR.SetAssociative + "-" + bestIPCR.LineSize;

        }
        string BestdCacheConfig()
        {
            //  return bestdCache.DCache + "-" + bestdCache.SetAssociative + "-" + bestdCache.LineSize + "-" + bestdCache.ICache;
            return bestdCache.DCache + "-" + bestdCache.SetAssociative + "-" + bestdCache.LineSize;
        }
        double BasedCacheMissRate() => baseR.DCacheMissRate;
        double BestdCacheMissRate() => bestdCache.DCacheMissRate;
        public ResultsComputer()
        {
            listOfResults = new List<Results>();
        }
        public Results ResultExists(string fileName)
        {
            if (fileName is null)
            {
                Console.WriteLine("Error, gave null string for file name");
                return null;
            }
            int testNumber = int.Parse(Regex.Match(fileName, @"\d+").Value, NumberFormatInfo.InvariantInfo);
            for (int i = 0; i < listOfResults.Count; i++)
            {
                if (listOfResults[i].TestNumber == testNumber)
                {
                    return listOfResults[i];
                }
            }
            Results temporaryResult = new Results(testNumber)
            {
                Name = fileName
            };
            listOfResults.Add(temporaryResult);
            return temporaryResult;
        }
        //this function sets the case for the base case (could be replaced in future by file read
        public Results SetBase()
        {

            int iCache = 32;//modify if different instruction cache base case
            int dCache = 32;//modify if different data cache base case
            int assoc = 4;//modify if different associativity base case
            int linesize = 64;//modify if different line size base case
            for (int i = 0; i < listOfResults.Count; i++)
            {
                if (listOfResults[i].DCache == dCache && listOfResults[i].SetAssociative == assoc
                    && listOfResults[i].LineSize == linesize && listOfResults[i].ICache == iCache)//see if all the parameters are the ones that we are looking for
                {
                    this.baseR = listOfResults[i];
                    break;//only one base case test will exist, so quit now
                }
            }
            if (this.baseR == null)
            {
                Console.WriteLine("Error: could not find basline");//evidence of serious issue
                return null;//will crash the program later on.
            }
            return baseR;
        }
        //finds the bast case relative to the base case
        public void FindBest()
        {
            if (baseR is null)
            {
                Console.WriteLine("Error, null Base. Try setting the base first.");
                return;
            }
            bestIPCR = baseR;
            bestdCache = baseR;
            baseR.DCachedifference = 0.0;
            baseR.IPCdifference1 = 0.0;
            for (int i = 0; i < listOfResults.Count; i++)
            {
                listOfResults[i].DCachedifference = (baseR.DCacheMissRate - listOfResults[i].DCacheMissRate) / baseR.DCacheMissRate;
                if (listOfResults[i].DCachedifference > bestdCache.DCachedifference)
                {
                    bestdCache = listOfResults[i];
                }
                listOfResults[i].IPCdifference1 = (listOfResults[i].IPC1 - baseR.IPC1) / baseR.IPC1;
                if (listOfResults[i].IPCdifference1 > bestdCache.IPCdifference1)
                {
                    bestIPCR = listOfResults[i];
                }
            }

        }

        //takes standard distribution of each different set of test and averages to find overal highest impact
        public string FindHighestImpactBest()
        {
            List<BoolValues> usableStructure = new List<BoolValues>();
            List<double> lineSizeSTDs = new List<double>();
            List<double> cacheSizeSTDs = new List<double>();
            List<double> setSizeSTDs = new List<double>();

            for (int i = 0; i < this.listOfResults.Count; i++)
            {
                usableStructure.Add(new BoolValues(listOfResults[i]));
            }
            for (int i = 0; i < usableStructure.Count; i++)
            {
                List<double> stdDivDcache = new List<double>();
                List<double> stdDivIPC = new List<double>();
                if (!usableStructure[i].CacheTested)
                {
                    stdDivDcache.Clear();
                    stdDivIPC.Clear();
                    for (int j = 0; j < usableStructure.Count; j++)
                    {
                        if (j == i)
                        {
                            continue;
                        }
                        if (usableStructure[i].ResultsPart.LineSize == usableStructure[j].ResultsPart.LineSize &&
                            usableStructure[i].ResultsPart.SetAssociative == usableStructure[j].ResultsPart.SetAssociative &&
                            !usableStructure[j].CacheTested)
                        {
                            usableStructure[j].CacheTested = true;
                            stdDivDcache.Add(usableStructure[j].ResultsPart.DCachedifference);
                            stdDivIPC.Add(usableStructure[j].ResultsPart.IPCdifference1);
                        }
                    }
                    if (stdDivDcache.Count > 0)
                    {
                        usableStructure[i].CacheTested = true;
                        stdDivDcache.Add(usableStructure[i].ResultsPart.DCachedifference);
                        stdDivIPC.Add(usableStructure[i].ResultsPart.IPCdifference1);
                        cacheSizeSTDs.Add(CalculateStdDev(stdDivDcache));
                        cacheSizeSTDs.Add(CalculateStdDev(stdDivIPC));
                    }
                }
                if (!usableStructure[i].LineTested)
                {
                    stdDivDcache.Clear();
                    stdDivIPC.Clear();
                    for (int j = 0; j < usableStructure.Count; j++)
                    {
                        if (j == i)
                        {
                            continue;//do not compare agaisnt self
                        }
                        if (usableStructure[i].ResultsPart.DCache == usableStructure[j].ResultsPart.DCache &&
                            usableStructure[i].ResultsPart.SetAssociative == usableStructure[j].ResultsPart.SetAssociative &&
                            !usableStructure[j].LineTested)
                        {
                            usableStructure[j].LineTested = true;
                            stdDivDcache.Add(usableStructure[j].ResultsPart.DCachedifference);
                            stdDivIPC.Add(usableStructure[j].ResultsPart.IPCdifference1);
                        }
                    }
                    if (stdDivDcache.Count > 0)
                    {
                        usableStructure[i].LineTested = true;
                        stdDivDcache.Add(usableStructure[i].ResultsPart.DCachedifference);
                        stdDivIPC.Add(usableStructure[i].ResultsPart.IPCdifference1);
                        setSizeSTDs.Add(CalculateStdDev(stdDivDcache));
                        setSizeSTDs.Add(CalculateStdDev(stdDivIPC));
                    }
                }
                if (!usableStructure[i].SetTested)
                {
                    stdDivDcache.Clear();
                    stdDivIPC.Clear();
                    for (int j = 0; j < usableStructure.Count; j++)
                    {
                        if (j == i)
                        {
                            continue;
                        }
                        if (usableStructure[i].ResultsPart.DCache == usableStructure[j].ResultsPart.DCache &&
                            usableStructure[i].ResultsPart.LineSize == usableStructure[j].ResultsPart.LineSize &&
                            !usableStructure[j].SetTested)
                        {
                            usableStructure[j].SetTested = true;
                            stdDivDcache.Add(usableStructure[j].ResultsPart.DCachedifference);
                            stdDivIPC.Add(usableStructure[j].ResultsPart.IPCdifference1);
                        }
                    }
                    if (stdDivDcache.Count > 0)
                    {
                        usableStructure[i].SetTested = true;
                        stdDivDcache.Add(usableStructure[i].ResultsPart.DCachedifference);
                        stdDivIPC.Add(usableStructure[i].ResultsPart.IPCdifference1);
                        lineSizeSTDs.Add(CalculateStdDev(stdDivDcache));
                        lineSizeSTDs.Add(CalculateStdDev(stdDivIPC));
                    }
                }
            }
            double lineAverage = lineSizeSTDs.Average();
            double cacheAverage = cacheSizeSTDs.Average();
            double setAverage = setSizeSTDs.Average();
            Console.WriteLine(lineAverage);
            Console.WriteLine(cacheAverage);
            Console.WriteLine(setAverage);
            if (cacheAverage > setAverage && cacheAverage > lineAverage)
            {
                return "Cache Size";
            }
            else if (setAverage > lineAverage)
            {
                return "Set Associativity";
            }
            else
            {
                return "Line Size";

            }
        }
        private double CalculateStdDev(List<double> values)
        {
            double ret = 0;
            if (values.Count() > 0)
            {
                //Compute the Average      
                double avg = values.Average();
                //Perform the Sum of (value-avg)_2_2      
                double sum = values.Sum(d => Math.Pow(d - avg, 2));
                //Put it all together      
                ret = Math.Sqrt((sum) / (values.Count() - 1));
            }
            return ret;
        }
        public void Print()
        {
            string path = @"results";
            if (!Directory.Exists(path))  // if it doesn't exist, create
            {
                Directory.CreateDirectory(path);
            }
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(BenchMark() + ".txt"))
            {
                file.WriteLine("Group member: Edward Brunton");
                file.WriteLine("Benchmark: " + BenchMark());
                file.WriteLine("Base IPC: " + BaseIPC());
                file.WriteLine("Best IPC: " + BestIPC());
                file.WriteLine("IPC improvement(%): " + IPCImpovement());
                file.WriteLine("Best IPC configuration: " + BestIPCConfig());
                file.WriteLine("Base dCache miss rate: " + BasedCacheMissRate());
                file.WriteLine("Best dCache miss rate: " + BestdCacheMissRate());
                file.WriteLine("dCache miss rate improvement(%): " + DCachImprovement());
                file.WriteLine("Best dCache configuration: " + BestdCacheConfig());
                file.WriteLine("Highest impact parameter: " + FindHighestImpactBest());//findHighestImpact());
            }
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(BenchMark() + "rawdata" + ".txt"))
            {
                // file.WriteLine("Base info: " + baseR.DCache + "-" + baseR.SetAssociative + "-" + baseR.LineSize + "-" + baseR.ICache);
                file.WriteLine("Base info: " + baseR.DCache + "-" + baseR.SetAssociative + "-" + baseR.LineSize);

                file.WriteLine("");
                for (int i = 0; i < listOfResults.Count; i++)
                {
                    file.WriteLine("Test Number " + listOfResults[i].TestNumber);
                    file.WriteLine("DCache " + listOfResults[i].DCache);
                    file.WriteLine("Associativity " + listOfResults[i].SetAssociative);
                    file.WriteLine("Line Size " + listOfResults[i].LineSize);
                    file.WriteLine("Name " + listOfResults[i].Name);
                    file.WriteLine("Associativity " + listOfResults[i].SetAssociative);
                    file.WriteLine("IPC " + listOfResults[i].IPC1);
                    file.WriteLine("ICache Size" + listOfResults[i].ICache);
                    file.WriteLine("IPC Improvement " + listOfResults[i].IPCdifference1);
                    file.WriteLine("DCache Miss Rate " + listOfResults[i].DCacheMissRate);
                    file.WriteLine("DCache Improvement" + listOfResults[i].DCachedifference);
                    file.WriteLine("");
                }
            }
        }
    }
    class Program
    {
        static double ParseDouble(string something)
        {
            int startIndex = -1;
            string useful = "";
            string validNumbers = "1234567890-.";
            for (int i = 0; i < validNumbers.Length - 1; i++)
            {
                for (int j = 0; j < something.Length; j++)
                {
                    if (validNumbers[i] == something[j])
                    {
                        if (j < startIndex | startIndex == -1)
                        {
                            startIndex = j;
                            break;
                        }
                    }
                }
            }
            if (startIndex == -1)
            {
                return -1.0;
            }
            else
            {
                int currentIndex = startIndex;

                for (int i = 0; i < validNumbers.Length; i++)
                {
                    if (validNumbers[i] == something[currentIndex])
                    {
                        useful += something[currentIndex];
                        currentIndex++;
                        i = -1;
                    }
                }

            }
            return Convert.ToDouble(useful);
        }
        static void Main(string[] args)
        {
            string[] fileList = new string[300];//max number of files that can be handled. Should have been a list, but whatever
            List<List<string>> fileContents = FileImporter(ref fileList);
            List<string> fileListFance = fileList.ToList();
            fileListFance.RemoveAll(item => item == null);
            ResultsComputer masterList = new ResultsComputer();
            if (fileListFance is null || fileContents is null)
            {
                Console.WriteLine("Error: lists not initialized; missing data!");
            }
            else if (fileListFance.Count != fileContents.Count)
            {
                Console.WriteLine("Error: missing at least one file");
            }
            for (int i = 0; i < fileListFance.Count; i++)
            {

                Results results = masterList.ResultExists(fileListFance[i]);
                if (fileListFance[i].Contains("test") == true)
                {
                    bool missrate = false;
                    bool ipc = false;

                    for (int j = 0; j < fileContents[i].Count; j++)
                    {
                        if (fileContents[i][j].Contains("system.cpu.dcache.overall_miss_rate::total"))
                        {
                            if (missrate)
                            {
                                Console.WriteLine("Error: double set the missrate");
                            }
                            else
                            {
                                missrate = true;
                            }
                            results.DCacheMissRate = ParseDouble(fileContents[i][j]);
                        }
                        else if (fileContents[i][j].Contains("system.cpu.ip"))
                        {
                            if (ipc && (results.IPC1 != ParseDouble(fileContents[i][j])))
                            {

                                Console.WriteLine("Error: double set the IPC");
                                Console.WriteLine("Previous IPC: " + results.IPC1);
                                Console.WriteLine("New IPC: " + ParseDouble(fileContents[i][j]));
                            }
                            else
                            {
                                ipc = true;
                            }
                            results.IPC1 = ParseDouble(fileContents[i][j]);
                        }

                    }
                    if (missrate == false)
                    {
                        Console.WriteLine("Error: Could not find miss rate");
                    }
                    if (ipc == false)
                    {
                        Console.WriteLine("Error: Could not find ipc");
                    }
                }
                else if (fileListFance[i].Contains("config") == true)
                {
                    bool lineSize = false;
                    bool assoc = false;
                    bool lookForDCacheSize = false;
                    bool lookForICacheSize = false;
                    for (int j = 0; j < fileContents[i].Count; j++)
                    {
                        if (fileContents[i][j].Contains("assoc"))
                        {
                            if (assoc)//only first associativity listed is desired
                            {
                                if (results.SetAssociative != int.Parse(Regex.Match(fileContents[i][j], @"\d+").Value, NumberFormatInfo.InvariantInfo))
                                {
                                    // Console.WriteLine("Blocking reseting of associativity");
                                }
                            }
                            else
                            {
                                assoc = true;
                                results.SetAssociative = int.Parse(Regex.Match(fileContents[i][j], @"\d+").Value, NumberFormatInfo.InvariantInfo);
                            }

                        }
                        else if (fileContents[i][j].Contains("cache_line_size"))
                        {
                            if (lineSize)
                            {
                                Console.WriteLine("Error: double set the IPC");
                            }
                            else
                            {
                                lineSize = true;
                            }
                            results.LineSize = int.Parse(Regex.Match(fileContents[i][j], @"\d+").Value, NumberFormatInfo.InvariantInfo);
                        }
                        else if (fileContents[i][j].Contains("a2time01"))
                        {
                            results.BenchMark = "a2time01";
                        }
                        else if (fileContents[i][j].Contains("mcf"))
                        {
                            results.BenchMark = "mcf";
                        }
                        else if (fileContents[i][j].Contains("cacheb01"))
                        {
                            results.BenchMark = "cacheb01";
                        }
                        else if (fileContents[i][j].Contains("bitmnp01"))
                        {
                            results.BenchMark = "bitmnp01";
                        }
                        else if (fileContents[i][j].Contains("bzip2"))
                        {
                            results.BenchMark = "bzip2";
                        }
                        else if (fileContents[i][j].Contains("libquantum"))
                        {
                            results.BenchMark = "libquantum";
                        }//add an additional benchmarks here. In future, make general function
                        else if (fileContents[i][j].Contains("[system.cpu.dcache]"))
                        {
                            lookForDCacheSize = true;
                        }
                        else if (lookForDCacheSize && fileContents[i][j].Contains("size"))
                        {
                            lookForDCacheSize = false;
                            results.DCache = int.Parse(Regex.Match(fileContents[i][j], @"\d+").Value, NumberFormatInfo.InvariantInfo) / 1024;
                        }
                        else if (fileContents[i][j].Contains("[system.cpu.icache]"))
                        {
                            lookForICacheSize = true;
                        }
                        else if (lookForICacheSize && fileContents[i][j].Contains("size"))
                        {
                            lookForICacheSize = false;
                            results.ICache = int.Parse(Regex.Match(fileContents[i][j], @"\d+").Value, NumberFormatInfo.InvariantInfo) / 1024;
                        }
                    }
                    if (lineSize == false)
                    {
                        Console.WriteLine("Error: Could not find line size");
                    }
                    if (assoc == false)
                    {
                        Console.WriteLine("Error: Could not find associtivity");
                    }
                }
                else
                {
                    Console.WriteLine("Warning: ");
                    Console.WriteLine("File name: " + fileListFance[i] + " could not be understood.");
                }
            }
            masterList.SetBase();
            masterList.FindBest();
            masterList.Print();
            //system.cpu.dcache.overall_miss_rate::total for data cache miss rates
            //system.cpu.ip for IPC
        }
        private static List<List<string>> FileImporter(ref string[] fileList)
        {
            List<List<string>> fileData = new List<List<string>>();
            string path = @"temp";
            if (!Directory.Exists(path))  // if it doesn't exist, create
            {
                Console.WriteLine("Could not find the path.");
                Directory.CreateDirectory(path);
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(path + "locationFile" + ".txt"))
                {
                    file.WriteLine("This is where you should put your files");
                    return null;
                }
            }
            else
            {
                fileList = Directory.GetFiles(path);
            }
            // Open the file to read from.
            for (int i = 0; i < fileList.Length; i++)
            {
                List<string> row = new List<string>();
                using (StreamReader sr = File.OpenText(fileList[i]))
                {
                    string s = "";
                    while ((s = sr.ReadLine()) != null)
                    {
                        row.Add(s.Trim());
                    }
                }
                fileData.Add(row);
            }
            return fileData;
        }
    }
}

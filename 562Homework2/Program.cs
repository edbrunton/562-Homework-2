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
            if (object.ReferenceEquals(null, baseR))
            {
                Console.WriteLine("Error, base does not exist when attempting to access baseIPC");
                return -1.0;
            }
            return baseR.IPC1;
        }
        double bestIPC() => bestIPCR.IPC1;
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
        string bestdCacheConfig()
        {
            //  return bestdCache.DCache + "-" + bestdCache.SetAssociative + "-" + bestdCache.LineSize + "-" + bestdCache.ICache;
            return bestdCache.DCache + "-" + bestdCache.SetAssociative + "-" + bestdCache.LineSize;
        }
        double basedCacheMissRate() => baseR.DCacheMissRate;
        double bestdCacheMissRate() => bestdCache.DCacheMissRate;
        public ResultsComputer()
        {
            listOfResults = new List<Results>();
        }
        public Results ResultExists(string fileName)
        {
            if (object.ReferenceEquals(null, fileName))
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
            Results temporaryResult = new Results(testNumber);
            temporaryResult.Name = fileName;
            listOfResults.Add(temporaryResult);
            return temporaryResult;
        }
        public Results setBase()
        {
            int iCache = 32;
            int dCache = 32;
            int assoc = 4;
            int linesize = 64;
            for (int i = 0; i < listOfResults.Count; i++)
            {
                if (listOfResults[i].DCache == dCache && listOfResults[i].SetAssociative == assoc
                    && listOfResults[i].LineSize == linesize && listOfResults[i].ICache == iCache)
                {

                    this.baseR = listOfResults[i];

                    break;
                }
            }
            if (this.baseR == null)
            {
                Console.WriteLine("Error: could not find basline");
                return null;
            }
            return baseR;
        }
        public void findBest()
        {
            if (object.ReferenceEquals(null, baseR))
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
        
  
        private void findSizes(out List<int> cacheSizes, out List<int> setAssociative, out List<int> lineSizes)
        {
            cacheSizes = new List<int>();
            setAssociative = new List<int>();
            lineSizes = new List<int>();
            for (int i = 0; i < this.listOfResults.Count; i++)
            {
                bool cacheSizeAlreadyExists = false;
                bool setAssociativeAlreadyExists = false;
                bool lineSizesAlreadyExists = false;

                for (int j = 0; j < cacheSizes.Count; j++)
                {
                    if (listOfResults[i].DCache == cacheSizes[j])
                    {
                        cacheSizeAlreadyExists = true;
                        break;
                    }
                }
                if (!cacheSizeAlreadyExists)
                {
                    cacheSizes.Add(listOfResults[i].DCache);
                }
                for (int j = 0; j < setAssociative.Count; j++)
                {

                    if (listOfResults[i].SetAssociative == setAssociative[j])
                    {
                        setAssociativeAlreadyExists = true;
                        break;
                    }
                }
                if (!setAssociativeAlreadyExists)
                {
                    setAssociative.Add(listOfResults[i].SetAssociative);
                }
                for (int j = 0; j < lineSizes.Count; j++)
                {
                    if (listOfResults[i].LineSize == lineSizes[j])
                    {
                        lineSizesAlreadyExists = true;
                        break;
                    }
                }
                if (!lineSizesAlreadyExists)
                {
                    lineSizes.Add(listOfResults[i].LineSize);
                }
            }
        }
        public string findHighestImpactBest()
        {
            List <BoolValues> usableStructure = new List<BoolValues>();
            List<double> lineSizeSTDs = new List<double>();
            List<double> cacheSizeSTDs = new List<double>();
            List<double> setSizeSTDs = new List<double>();

            for (int i = 0; i <this.listOfResults.Count; i++)
            {
                usableStructure.Add(new BoolValues(listOfResults[i]));
            }
            for(int i =0; i < usableStructure.Count; i++)
            {
                List<double> stdDiv = new List<double>();
                List<double> stdDiv2 = new List<double>();
                if (!usableStructure[i].CacheTested)
                {
                    stdDiv.Clear();
                    stdDiv2.Clear();
                    for(int j = 0; j < usableStructure.Count; j ++)
                    {
                        if(j==i)
                        {
                            continue;
                        }
                        if(usableStructure[i].ResultsPart.LineSize == usableStructure[j].ResultsPart.LineSize &&
                            usableStructure[i].ResultsPart.SetAssociative == usableStructure[j].ResultsPart.SetAssociative &&
                            !usableStructure[j].CacheTested)
                        {
                            usableStructure[j].CacheTested = true;
                            stdDiv.Add(usableStructure[j].ResultsPart.DCachedifference);
                            stdDiv2.Add(usableStructure[j].ResultsPart.IPCdifference1);
                        }
                    }
                    if(stdDiv.Count >0)
                    {
                        usableStructure[i].CacheTested = true;
                        stdDiv.Add(usableStructure[i].ResultsPart.DCachedifference);
                        stdDiv2.Add(usableStructure[i].ResultsPart.IPCdifference1);
                        cacheSizeSTDs.Add(CalculateStdDev(stdDiv));
                        cacheSizeSTDs.Add(CalculateStdDev(stdDiv2));
                    }
                }
                if(!usableStructure[i].LineTested)
                {
                    stdDiv.Clear();
                    stdDiv2.Clear();
                    for (int j = 0; j < usableStructure.Count; j++)
                    {
                        if (j == i)
                        {
                            continue;
                        }
                        if (usableStructure[i].ResultsPart.DCache == usableStructure[j].ResultsPart.DCache && 
                            usableStructure[i].ResultsPart.SetAssociative == usableStructure[j].ResultsPart.SetAssociative &&
                            !usableStructure[j].LineTested)
                        {
                            usableStructure[j].LineTested = true;
                            stdDiv.Add(usableStructure[j].ResultsPart.DCachedifference);
                            stdDiv2.Add(usableStructure[j].ResultsPart.IPCdifference1);
                        }
                    }
                    if (stdDiv.Count > 0)
                    {
                        usableStructure[i].LineTested = true;
                        stdDiv.Add(usableStructure[i].ResultsPart.DCachedifference);
                        stdDiv2.Add(usableStructure[i].ResultsPart.IPCdifference1);
                        setSizeSTDs.Add(CalculateStdDev(stdDiv));
                        setSizeSTDs.Add(CalculateStdDev(stdDiv2));
                    }
                }
                if(!usableStructure[i].SetTested)
                {
                    stdDiv.Clear();
                    stdDiv2.Clear();
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
                            stdDiv.Add(usableStructure[j].ResultsPart.DCachedifference);
                            stdDiv2.Add(usableStructure[j].ResultsPart.IPCdifference1);
                        }
                    }
                    if (stdDiv.Count > 0)
                    {
                        usableStructure[i].SetTested = true;
                        stdDiv.Add(usableStructure[i].ResultsPart.DCachedifference);
                        stdDiv2.Add(usableStructure[i].ResultsPart.IPCdifference1);
                        lineSizeSTDs.Add(CalculateStdDev(stdDiv));
                        lineSizeSTDs.Add(CalculateStdDev(stdDiv2));
                    }
                }
            }
            double lineAverage = lineSizeSTDs.Average();
            double cacheAverage = cacheSizeSTDs.Average();
            double setAverage = setSizeSTDs.Average();
            Console.WriteLine(lineAverage);
            Console.WriteLine(cacheAverage);
            Console.WriteLine(setAverage);
            if(cacheAverage > setAverage && cacheAverage > lineAverage)
            {
                return "Cache Size";
            }
            else if(setAverage > lineAverage)
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
             ret = Math.Sqrt((sum) / (values.Count()-1));   
          }   
          return ret;
        }

        private static double calculateDifferences(List<List<Results>> cacheSorted)
        {
            List<double> tempSum = new List<double>(cacheSorted.Count);
            for (int i = 0; i < cacheSorted.Count; i++)
            {
                List<Results> values = cacheSorted[i];
                tempSum.Add(0);
                for (int j = 0; j < values.Count; j++)
                {
                    Results value = values[j];
                    if (value.DCachedifference > 0)
                    {
                        tempSum[i] += value.DCachedifference;
                    }
                    else
                    {
                        tempSum[i] -= value.DCachedifference;
                    }
                    if (value.IPCdifference1 > 0)
                    {
                        tempSum[i] += value.IPCdifference1;
                    }
                    else
                    {
                        tempSum[i] -= value.IPCdifference1;
                    }
                }
            }
            double product = 1.0;
            for(int i = 0; i <tempSum.Count; i++)
            {
                product *= tempSum[i];
            }
            return product;
        }

        public List<Results> getAllofCacheSize(int size)
        {
            List<Results> temp = new List<Results>();
            for (int i = 0; i < this.listOfResults.Count; i++)
            {
                if(listOfResults[i].DCache == size)
                {
                    temp.Add(listOfResults[i]);
                }
            }
            if(temp.Count == 0)
            {
                Console.WriteLine("Warning: Invalid cache size");
            }
            return temp;
        }
        public List<Results> getsetAssociative(int size)
        {
            List<Results> temp = new List<Results>();
            for (int i = 0; i < this.listOfResults.Count; i++)
            {
                if (listOfResults[i].SetAssociative == size)
                {
                    temp.Add(listOfResults[i]);
                }
            }
            if (temp.Count == 0)
            {
                Console.WriteLine("Warning: Invalid set associative size");
            }
            return temp;
        }
        public List<Results> getlineSizes(int size)
        {
            List<Results> temp = new List<Results>();
            for (int i = 0; i < this.listOfResults.Count; i++)
            {
                if (listOfResults[i].LineSize == size)
                {
                    temp.Add(listOfResults[i]);
                }
            }
            if (temp.Count == 0)
            {
                Console.WriteLine("Warning: Invalid line size");
            }
            return temp;
        }
        public void print()
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
                file.WriteLine("Best IPC: " + bestIPC());
                file.WriteLine("IPC improvement(%): " + IPCImpovement());
                file.WriteLine("Best IPC configuration: " + BestIPCConfig());
                file.WriteLine("Base dCache miss rate: " + basedCacheMissRate());
                file.WriteLine("Best dCache miss rate: " + bestdCacheMissRate());
                file.WriteLine("dCache miss rate improvement(%): " + DCachImprovement());
                file.WriteLine("Best dCache configuration: " + bestdCacheConfig());
                file.WriteLine("Highest impact parameter: " + findHighestImpactBest());//findHighestImpact());
            }
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(BenchMark() + "rawdata" + ".txt"))
            {
                // file.WriteLine("Base info: " + baseR.DCache + "-" + baseR.SetAssociative + "-" + baseR.LineSize + "-" + baseR.ICache);
                file.WriteLine("Base info: " + baseR.DCache + "-" + baseR.SetAssociative + "-" + baseR.LineSize);

                file.WriteLine("");
               for(int i = 0; i < listOfResults.Count; i++)
                {
                    file.WriteLine("Test Number " + listOfResults[i].TestNumber);
                    file.WriteLine("DCache " + listOfResults[i].DCache);
                    file.WriteLine("Associativity "  + listOfResults[i].SetAssociative);
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
        static double parseDouble(string something)
        {
            int startIndex = -1;
            string useful = "";
            string validNumbers = "1234567890-.";
            for(int i = 0; i < validNumbers.Length-1; i++)
            {
                for(int j =0; j < something.Length; j++)
                {
                    if(validNumbers[i] == something[j])
                    {
                        if(j < startIndex | startIndex == -1)
                        {
                            startIndex = j;
                            break;
                        }
                    }
                }
            }
            if(startIndex == -1)
            {
                return -1.0;
            }
            else
            {
                int currentIndex = startIndex;
                
                    for(int i = 0; i < validNumbers.Length; i++)
                    {
                        if(validNumbers[i] == something[currentIndex])
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
            string[] fileList = new string[300];
            List<List<string>> fileContents = FileImporter(ref fileList);
            List<string> fileListFance = fileList.ToList();
            fileListFance.RemoveAll(item => item == null);
            ResultsComputer masterList = new ResultsComputer();
            if(object.ReferenceEquals(null, fileListFance) || object.ReferenceEquals(null, fileContents))
             {
                Console.WriteLine("Error: lists not initialized; missing data!");
            }
            else if(fileListFance.Count != fileContents.Count)
            {
                Console.WriteLine("Error: missing at least one file");
            }
            for(int i = 0; i < fileListFance.Count; i++)
            {
                
                Results results = masterList.ResultExists(fileListFance[i]);         
                if (fileListFance[i].Contains("test") == true)
                {
                    bool missrate = false;
                    bool ipc = false;
                   
                    for (int j = 0; j < fileContents[i].Count; j++)
                    {
                        if (fileContents[i][j].Contains("system.cpu.dcache.overall_miss_rate::total"))
                        {   if(missrate)
                            {
                                Console.WriteLine("Error: double set the missrate");
                            }
                            else
                            {
                                missrate = true;
                            }
                            results.DCacheMissRate = parseDouble(fileContents[i][j]);
                        }
                        else if (fileContents[i][j].Contains("system.cpu.ip"))
                        {
                            if (ipc && (results.IPC1 != parseDouble(fileContents[i][j])))
                            {
                              
                                Console.WriteLine("Error: double set the IPC");
                                Console.WriteLine("Previous IPC: " + results.IPC1);
                                Console.WriteLine("New IPC: " + parseDouble(fileContents[i][j]));
                            }
                            else
                            {
                                ipc = true;
                            }
                            results.IPC1 = parseDouble(fileContents[i][j]); 
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
                        else if(fileContents[i][j].Contains("a2time01"))
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
                        }
                        else if (fileContents[i][j].Contains("[system.cpu.dcache]"))
                        {
                            lookForDCacheSize = true;
                        }
                        else if(lookForDCacheSize && fileContents[i][j].Contains("size"))
                        {
                            lookForDCacheSize = false;
                            results.DCache = int.Parse(Regex.Match(fileContents[i][j], @"\d+").Value, NumberFormatInfo.InvariantInfo)/ 1024;
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
            masterList.setBase();
            masterList.findBest();
            masterList.print();
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

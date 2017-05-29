using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PageRank
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = @"..\..\data\web-Stanford.txt";
            int countScippedLines = 4;
            int numberOfNodes = 281903; // web-Stanford
            double epsilon = 1e-6;
            double d = 0.85;

            PageRank pageRank = new PageRank(epsilon, d, numberOfNodes);
            pageRank.readData(path, countScippedLines);
            pageRank.simplePowerIteration();
            pageRank.writeToFile("PageRankSimplePI.csv");
            pageRank.modifiedPowerIteration();
            pageRank.writeToFile("PageRankModifiedPI.csv"); 

            Console.ReadKey();
        }
    }
}

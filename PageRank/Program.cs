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
        static int maxNumberOfLines = 100000;
        static List<double> errors = new List<double>();
        static double epsilon = 1e-4;
        static double d = 0.85;
        static double teleportP = 1 - d;
        static string path = @"E:\Projects\Visual Studio\PageRank\web-Stanford.txt";

        static double distance(Dictionary<Int32, Double> vector1, Dictionary<Int32, Double> vector2)
        {
            double result = 0d;

            foreach (var key in vector1.Keys)
            {
                result += Math.Pow(vector1[key] - vector2[key], 2);
            }

            result = Math.Sqrt(result);
            return result; 
        }
        static void simplePowerIteration()
        {
            Dictionary<Int32, List<Int32>> connectionsDict = new Dictionary<Int32, List<Int32>>();
            HashSet<Int32> uniqueNods = new HashSet<int>();
            
            Int32 numberOfNodes = 0;
            int countLines = 0;
            

            using (StreamReader sr = new StreamReader(path, System.Text.Encoding.Default))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    String[] values = line.Split(new char[] { '\t' });
                    int from = Int32.Parse(values[0]);
                    int to = Int32.Parse(values[1]);

                    if (connectionsDict.ContainsKey(from))
                    {
                        connectionsDict[from].Add(to);
                    }
                    else
                    {
                        List<Int32> refs = new List<int>();
                        refs.Add(to);
                        connectionsDict.Add(from, refs);
                    }

                    uniqueNods.Add(from);
                    uniqueNods.Add(to);
                    countLines += 1;

                    if (countLines == maxNumberOfLines)
                    {
                        break;
                    }
                }
            }

            numberOfNodes = uniqueNods.Count;
            double equalProbability = (double)1 / numberOfNodes;

            Dictionary<Int32, Dictionary<Int32, Double>> P = new Dictionary<int, Dictionary<int, double>>(connectionsDict.Count); // матрица переходов
            Dictionary<Int32, Double> r0 = new Dictionary<int, double>(numberOfNodes);
            Dictionary<Int32, Double> r1 = new Dictionary<int, double>(numberOfNodes);

            // формирую матрицу переходов

            foreach (var from in connectionsDict.Keys)
            {
                P.Add(from, new Dictionary<int, double>());
            }

            foreach (var from in connectionsDict.Keys)
            {
                List<Int32> references = connectionsDict[from];
                Int32 countReferences = references.Count;
                double probability = (double)1 / countReferences;

                foreach (var to in references)
                {
                    P[from].Add(to, probability);

                }

            }

            // задаю начальный вектор рангов
            foreach (var node in uniqueNods)
            {
                r0[node] = equalProbability;
            }

            int countIterations = 0;

            // считаю ранги по итерациям
            do
            {
                
                foreach (var node in uniqueNods)
                {
                    double sum = 0d;

                    foreach (var nodeFrom in uniqueNods)
                    {
                        if (P.ContainsKey(nodeFrom))
                        {
                            if (P[nodeFrom].ContainsKey(node))
                            {
                                sum += d * P[nodeFrom][node] * r0[nodeFrom];
                            }
                        }
                        else // висячий узел, переходим во все вершины равновероятно
                        {
                            sum += d * (equalProbability) * r0[nodeFrom];
                        }
                    }

                    r1[node] = sum + (1 - d) * equalProbability; // телепорт   
                    int t = 3;                
                }

                countIterations += 1;
                double error = distance(r0, r1);
                errors.Add(error);
                Console.WriteLine("Error " + error);

                if (error < epsilon)
                {
                    break;
                }
                else
                {
                    foreach (var key in uniqueNods)
                    {
                        r0[key] = r1[key];
                    }

                }

            } while (true);

            double sum2 = 0d;

            foreach (var key in r1.Keys)
            {
                sum2 += r1[key];
            }

            Console.WriteLine("Сумма " + sum2);
            Console.WriteLine("Число итераций " + countIterations);

            if (path.Contains("file"))
            {
                foreach (var key in r1.Keys)
                {
                    Console.WriteLine(r1[key]);
                }
            }

        }

        static void modifiedPowerIteration()
        {
            Dictionary<Int32, List<Int32>> connectionsDict = new Dictionary<Int32, List<Int32>>();
            HashSet<Int32> uniqueNods = new HashSet<int>();

            Int32 numberOfNodes = 0;
            Int32 allReferencesCount = 0;
            int countLines = 0;
            int countIterations = 0;

            using (StreamReader sr = new StreamReader(path, System.Text.Encoding.Default))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    String[] values = line.Split(new char[] { '\t' });
                    int from = Int32.Parse(values[0]);
                    int to = Int32.Parse(values[1]);
                    allReferencesCount += 1;

                    if (connectionsDict.ContainsKey(from))
                    {
                        connectionsDict[from].Add(to);
                    }
                    else
                    {
                        List<Int32> refs = new List<int>();
                        refs.Add(to);
                        connectionsDict.Add(from, refs);
                    }

                    uniqueNods.Add(from);
                    uniqueNods.Add(to);
                    countLines += 1;

                    if (countLines == maxNumberOfLines)
                    {
                        break;
                    }
                }
            }

            numberOfNodes = uniqueNods.Count;
            double equalProbability = (double)1 / numberOfNodes;
            Dictionary<Int32, Dictionary<Int32, Double>> P = new Dictionary<int, Dictionary<int, double>>(connectionsDict.Count); // матрица переходов

            Dictionary<Int32, Double> r0 = new Dictionary<int, double>(numberOfNodes);
            Dictionary<Int32, Double> r1 = new Dictionary<int, double>(numberOfNodes);

            // формирую матрицу переходов

            foreach (var from in connectionsDict.Keys)
            {
                P.Add(from, new Dictionary<int, double>());
            }

            foreach (var from in connectionsDict.Keys)
            {
                List<Int32> references = connectionsDict[from];
                int countReferences = references.Count;
                double probability = (double)1 / countReferences;

                foreach (var to in references)
                {
                    P[from].Add(to, probability);

                }

            }

            // авторитетная телепортация

            Dictionary<int, double> authoritarianTeleport = new Dictionary<int, double>(uniqueNods.Count);

            foreach (var nodeTo in uniqueNods)
            {
                int countReferencesOnNode = 0;
                foreach (var nodeFrom in uniqueNods)
                {
                    if (P.ContainsKey(nodeFrom))
                    {
                        if (P[nodeFrom].ContainsKey(nodeTo))
                        {
                            countReferencesOnNode += 1;
                        }

                    }
                    else
                    {
                        authoritarianTeleport[nodeTo] = 0;
                    }


                }

                authoritarianTeleport[nodeTo] = (double)countReferencesOnNode / allReferencesCount;
            }


            foreach (var node in uniqueNods)
            {
                r0[node] = /*authoritarianTeleport[node]*/equalProbability;
            }

            Dictionary<Int32, Double> r0Copy = new Dictionary<Int32, Double>(r0.Count);

            // считаю ранги по итерациям
            do
            {

                foreach (var node in uniqueNods)
                {
                    r0Copy[node] = r0[node];
                    double sum = 0d;

                    foreach (var nodeFrom in uniqueNods)
                    {
                        if (P.ContainsKey(nodeFrom))
                        {
                            if (P[nodeFrom].ContainsKey(node))
                            {
                                sum += d * P[nodeFrom][node] * r0[nodeFrom];
                            }
                        }
                        //висячий узел, переходим во все вершины равновероятно
                        else
                        {
                            sum += d * equalProbability * r0[nodeFrom];
                        }

                    }

                    r1[node] = sum + (1 - d) * authoritarianTeleport[node]/*equalProbability*/; // телепорт
                    r0[node] = r1[node];
                
                }

                double error = distance(r0Copy, r1);
                errors.Add(error);
                Console.WriteLine("Error " + error);
                countIterations += 1;

                if (error < epsilon)
                {
                    break;
                }
                else
                {
                    foreach (var key in uniqueNods)
                    {
                        r0[key] = r1[key];
                    }
                }


            } while (true);

            double sum2 = 0d;

            foreach (var key in r1.Keys)
            {
                sum2 += r1[key];
            }

            Console.WriteLine("Сумма " + sum2);
            Console.WriteLine("Число итераций " + countIterations);

            if (path.Contains("file"))
            {
                foreach (var key in r1.Keys)
                {
                    Console.WriteLine(r1[key]);
                }
            }

        }

        static void writeToFile(string fileName)
        {
            if (path.Contains("file"))
            {
                return;
            }
            using(StreamWriter sw = new StreamWriter(fileName, false, Encoding.Unicode))
            {
                sw.Write("Iteration" + "\t");
                sw.Write("Error" + "\t");
                sw.Write("\r\n");

                for (int i = 0; i < errors.Count; i++)
                {
                    sw.Write(i+1 + "\t");
                    sw.Write(errors[i] + "\t");
                    sw.Write("\r\n");
                }

                errors = new List<double>();
            }
            
        }
        static void Main(string[] args)
        {
            simplePowerIteration();
            writeToFile("simplePI" + maxNumberOfLines + ".csv");
            modifiedPowerIteration();
            writeToFile("modifiedPIv3_" + maxNumberOfLines + ".csv");
            Console.ReadKey();
        }
    }
}

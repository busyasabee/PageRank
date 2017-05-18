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
            String path = @"E:\Projects\Visual Studio\PageRank\web-Stanford.txt";
            Dictionary<Int32, List<Int32>> connectionsDict = new Dictionary<Int32, List<Int32>>();
            HashSet<Int32> uniqueNods = new HashSet<int>();
            
            Int32 numberOfNodes = 0;
            double d = 0.85;
            double epsilon = 0.0001;
            int countLines = 0;
            

            using (StreamReader sr = new StreamReader(path, System.Text.Encoding.Default))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    String[] values = line.Split(new char[] { '\t' });
                    Int32 from = Int32.Parse(values[0]);
                    Int32 to = Int32.Parse(values[1]);

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
                    //Console.WriteLine(line);
                }
                //Int32 t = 0;
            }

            numberOfNodes = uniqueNods.Count;
            //uniqueNods = null;
            Dictionary<Int32, Dictionary<Int32, Double>> P = new Dictionary<int, Dictionary<int, double>>(); // матрица переходов
            //double[] r0 = new double[numberOfNodes]; // начальный вектор рангов
            //double[] r1 = new double[numberOfNodes];
            Dictionary<Int32, Double> r0 = new Dictionary<int, double>();
            Dictionary<Int32, Double> r1 = new Dictionary<int, double>();
            // формирую матрицу переходов
            // иду по вершинам в словаре, делю 1 на число ссылок из вершины
            foreach (var from in connectionsDict.Keys)
            {
                P.Add(from, new Dictionary<int, double>());
            }

            foreach (var from in connectionsDict.Keys)
            {
                List<Int32> references = connectionsDict[from];
                Int32 countReferences = references.Count;
                //double value = (double)1 / countReferences;
                // у меня получается при формировании матрицы не обрабатываются висячии узлы
                // можно потом при умножении это учесть
                foreach (var to in references)
                {
                    P[from].Add(to, (double)1 / countReferences);
   
                }

            }

            // задаю начальный вектор рангов
            foreach (var node in uniqueNods)
            {
                r0[node] = (double) 1 / numberOfNodes;
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
                            sum += d*((double) 1 / numberOfNodes)* r0[nodeFrom];
                        }
                        

                    }

                    r1[node] = sum + (1 - d) * (double)1 / numberOfNodes; // равновероятно делаем телепорт

                }

                double error = distance(r0, r1);
                errors.Add(error);
                Console.WriteLine("Error" + error);
                if (error < epsilon)
                {
                    errors = new List<double>();
                    break;
                }
                else
                {
                    r0 = r1;
                }

                countIterations += 1;
                r1 = new Dictionary<int, double>();
                int t2 = 3;

            } while (true);

            double sum2 = 0d;

            foreach (var key in r1.Keys)
            {
                sum2 += r1[key];
            }

            Console.WriteLine("Сумма " + sum2);
            Console.WriteLine("Число итераций " + countIterations);
            
            //foreach (var key in r1.Keys)
            //{
            //    Console.WriteLine(r1[key]);
            //}
            int t = 3;

            //foreach (Int32 key in connectionsDict.Keys)
            //{
            //    Console.Write(key + " ");
            //    for (int i = 0; i < connectionsDict[key].Count; i++)
            //    {
            //        Console.Write(connectionsDict[key][i] + " ");
            //    }
            //    Console.WriteLine();
            //}
            //Console.ReadKey();
        }

        static void modifiedPowerIteration()
        {
            String path = @"E:\Projects\Visual Studio\PageRank\web-Stanford.txt";
            Dictionary<Int32, List<Int32>> connectionsDict = new Dictionary<Int32, List<Int32>>();
            HashSet<Int32> uniqueNods = new HashSet<int>();

            Int32 numberOfNodes = 0;
            double d = 0.85;
            double epsilon = 0.001;
            Int32 allReferencesCount = 0;
            int countLines = 0;

            using (StreamReader sr = new StreamReader(path, System.Text.Encoding.Default))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    String[] values = line.Split(new char[] { '\t' });
                    Int32 from = Int32.Parse(values[0]);
                    Int32 to = Int32.Parse(values[1]);
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
                    //Console.WriteLine(line);
                }
                //Int32 t = 0;
            }

            numberOfNodes = uniqueNods.Count;
            //uniqueNods = null;
            Dictionary<Int32, Dictionary<Int32, Double>> P = new Dictionary<int, Dictionary<int, double>>(); // матрица переходов
            //double[] r0 = new double[numberOfNodes]; // начальный вектор рангов
            //double[] r1 = new double[numberOfNodes];
            Dictionary<Int32, Double> r0 = new Dictionary<int, double>();
            Dictionary<Int32, Double> r1 = new Dictionary<int, double>();
            // формирую матрицу переходов
            // иду по вершинам в словаре, делю 1 на число ссылок из вершины
            foreach (var from in connectionsDict.Keys)
            {
                P.Add(from, new Dictionary<int, double>());
            }

            foreach (var from in connectionsDict.Keys)
            {
                List<Int32> references = connectionsDict[from];
                Int32 countReferences = references.Count;
                // у меня получается при формировании матрицы не обрабатываются висячии узлы
                // можно потом при умножении это учесть
                foreach (var to in references)
                {
                    P[from].Add(to, (double)1 / countReferences);

                }

            }

            // задаю начальный вектор рангов
            // ранг зависит от числа ссылок

            foreach (var nodeTo in uniqueNods)
            {
                int countReferencesOnNode = 0;
                foreach(var nodeFrom in uniqueNods)
                {
                    if (P.ContainsKey(nodeFrom))
                    {
                        if (P[nodeFrom].ContainsKey(nodeTo))
                        {
                            countReferencesOnNode += 1;
                        }
                        // хз если нет ссылок на вершину, можно ли делать нулём. Можно просто сделать маленьким
                    }
                    else
                    {
                        r0[nodeTo] = 0;
                    }


                    
                }

                r0[nodeTo] = (double)countReferencesOnNode / allReferencesCount;
            }

            int countIterations = 0;

            Dictionary<Int32, Double> r0Copy = new Dictionary<Int32, Double>(r0.Count);
            //foreach (KeyValuePair<Int32, Double> entry in r0)
            //{
            //    r0Copy.Add(entry.Key, (Double)entry.Value);
            //}

            // считаю ранги по итерациям
            do
            {

                foreach (var node in uniqueNods)
                {
                    r0Copy[node] = r0[node];
                    double sum = 0d;

                    // если компоненты r0 и r1 не отличаются, то не вычисляем их
                    if (r1.ContainsKey(node))
                    {
                        double diff = Math.Abs(r0Copy[node] - r1[node]);
                        if (diff < epsilon)
                        {
                            r1[node] = r0Copy[node];
                            continue;
                        }
                    }

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
                            sum += d * ((double)1 / numberOfNodes) * r0[nodeFrom];
                        }

                    }

                    r1[node] = sum + (1 - d) * (double)1 / numberOfNodes; // равновероятно делаем телепорт
                    r0[node] = r1[node];
                  

                }

                double error = distance(r0Copy, r1);
                errors.Add(error);
                Console.WriteLine("Error " + error);
                countIterations += 1;

                if (error < epsilon)
                {
                    errors = new List<double>();
                    break;
                }
                else
                {
                    r0 = r1;
                }

                r1 = new Dictionary<int, double>();
                int t2 = 3;

            } while (true);

            double sum2 = 0d;

            foreach (var key in r1.Keys)
            {
                sum2 += r1[key];
            }

            Console.WriteLine("Сумма " + sum2);
            Console.WriteLine("Число итераций " + countIterations);
            //foreach (var key in r1.Keys)
            //{
            //    Console.WriteLine(r1[key]);
            //}
            //int t = 3;
        }

        static void writeToFile(string fileName)
        {
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
            }
            
        }
        static void Main(string[] args)
        {
            simplePowerIteration();
            writeToFile("simplePowerIteration.csv");
            //modifiedPowerIteration();
            Console.ReadKey();
        }
    }
}

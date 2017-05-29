using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PageRank
{
    class PageRank
    {
        double epsilon;
        double d;
        double teleportP;
        int numberOfNodes;
        Dictionary<Int32, List<Int32>> connectionsDict = new Dictionary<Int32, List<Int32>>(); // store nods which refers on the key node
        double equalProbability;
        double[] P;
        double[] r0;
        double[] r1;
        int[] countOutLinks;
        List<double> errors;
        bool wasPrepared = false;


        public PageRank(double epsilon, double d, int numberOfNodes)
        {
            this.epsilon = epsilon;
            this.d = d;
            this.numberOfNodes = numberOfNodes;
            teleportP = 1 - d;
            equalProbability = (double)1 / numberOfNodes;
            P = new double[numberOfNodes];
            countOutLinks = new int[numberOfNodes];
            r0 = new double[numberOfNodes];
            r1 = new double[numberOfNodes];
            errors = new List<double>();
        }


        double distance(double[] vector1, double[] vector2)
        {
            double result = 0d;

            for (int i = 0; i < numberOfNodes; i++)
            {
                result += Math.Pow(vector1[i] - vector2[i], 2);
            }

            result = Math.Sqrt(result);
            return result;
        }

        public void readData(string path, int countScippedLines)
        {
            int countLines = 0;

            using (StreamReader sr = new StreamReader(path, System.Text.Encoding.Default))
            {
                string line;
                String[] values;
                int from;
                int to;

                for (int i = 0; i < countScippedLines; i++)
                {
                    sr.ReadLine();
                }
   

                while ((line = sr.ReadLine()) != null)
                {
                    values = line.Split(new char[] { '\t' });
                    from = Int32.Parse(values[0]);
                    to = Int32.Parse(values[1]);

                    if (connectionsDict.ContainsKey(to))
                    {
                        connectionsDict[to].Add(from);
                    }
                    else
                    {
                        List<Int32> refs = new List<int>();
                        refs.Add(from);
                        connectionsDict.Add(to, refs);
                    }

                    countOutLinks[from - 1] += 1;                  
                    countLines += 1;
                    Console.WriteLine("Readed lines " + countLines);

                }
            }

        }

        void prepare()
        {
            if (wasPrepared)
            {
                r1 = new double[numberOfNodes];

                for (int i = 0; i < numberOfNodes; i++)
                {
                    r0[i] = equalProbability;
                }

                return;
            }

            for (int i = 0; i < numberOfNodes; i++)
            {
                if (countOutLinks[i] != 0)
                {
                    P[i] = (double)1 / countOutLinks[i];
                }
                else
                {
                    // hanging nods

                    P[i] = equalProbability;

                    foreach (var node in connectionsDict.Keys)
                    {
                        connectionsDict[node].Add(i + 1);
                    }
                }
                
                r0[i] = equalProbability;
            }

        }
        public void simplePowerIteration()
        {
            prepare();
            int countIterations = 0;
            double sum;

            while (true)
            {
                for (int i = 0; i < numberOfNodes; i++)
                {
                    sum = 0d;

                    if (connectionsDict.ContainsKey(i + 1))
                    {
                        foreach (var inLink in connectionsDict[i + 1])
                        {
                            sum += d * P[inLink - 1] * r0[inLink - 1];
                        }
                    }

                    r1[i] = sum + teleportP * equalProbability;

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
                    for (int i = 0; i < numberOfNodes; i++)
                    {
                        r0[i] = r1[i];
                    }

                }
            }

            double sum2 = 0d;

            for (int i = 0; i < numberOfNodes; i++)
            {
                sum2 += r1[i];

            }

            Console.WriteLine("Sum " + sum2);
            Console.WriteLine("Number of iterations " + countIterations);

        }

        public void modifiedPowerIteration()
        {
            prepare();
            int countIterations = 0;
            double sum;

            while (true)
            {
                for (int i = 0; i < numberOfNodes; i++)
                {
                    sum = 0d;

                    if (connectionsDict.ContainsKey(i + 1))
                    {
                        foreach (var inLink in connectionsDict[i + 1])
                        {
                            if (inLink - 1 < i)
                            {
                                sum += d * P[inLink - 1] * r1[inLink - 1];
                            }
                            else
                            {
                                sum += d * P[inLink - 1] * r0[inLink - 1];
                            }
                            
                        }
                    }

                    r1[i] = sum + teleportP * equalProbability;

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
                    for (int i = 0; i < numberOfNodes; i++)
                    {
                        r0[i] = r1[i];
                    }

                }
            }

            double sum2 = 0d;

            for (int i = 0; i < numberOfNodes; i++)
            {
                sum2 += r1[i];

            }

            Console.WriteLine("Sum " + sum2);
            Console.WriteLine("Number of iterations " + countIterations);

        }
        public void writeToFile(string fileName)
        {
            using (StreamWriter sw = new StreamWriter(fileName, false, Encoding.Unicode))
            {
                sw.Write("Iteration" + "\t");
                sw.Write("Error" + "\t");
                sw.Write("\r\n");

                for (int i = 0; i < errors.Count; i++)
                {
                    sw.Write(i + 1 + "\t");
                    sw.Write(errors[i] + "\t");
                    sw.Write("\r\n");
                }

                errors = new List<double>();
            }

        }
    }
}

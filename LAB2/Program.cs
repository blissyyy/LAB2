using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LAB2
{
    internal static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());

        }
        public class Generation
        {
            public int result;
            public double[] CreateArray()
            {

                double[] Array = new double[15000];
                Random rnd = new Random();
                for (int k = 0; k < 15000; k++)
                {
                    Array[k] = rnd.NextDouble() * 100;
                }
                return Array;
            }
            public void Sort(double[] arr)
            {
                const double factor = 1.247;
                int length = arr.Length;
                int step = (int)(length / factor);
                while (step > 0)
                {
                    for (int k = 0, i = step; i < length; k++, i++)
                    {
                        if (arr[k] > arr[i])
                        {
                            double temp = arr[i];
                            arr[i] = arr[k];
                            arr[k] = temp;
                            temp = 0;
                        }

                    }
                    step = (int)(step / factor);

                }

            }
            public TimeSpan Count()
            {
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
                double[] Array = CreateArray();
                Sort(Array);
                stopWatch.Stop();
                TimeSpan ts = stopWatch.Elapsed;
                return ts;
            }
            public (double, int) Results()
            {
                double executionTime = Count().TotalMilliseconds;
                int sysNum = Thread.CurrentThread.ManagedThreadId;

                return (executionTime, sysNum);
            }
        }
    }
}

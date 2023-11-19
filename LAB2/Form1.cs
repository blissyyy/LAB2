using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static LAB2.Program;

namespace LAB2
{
    public partial class Form1 : Form
    {
        Generation generation = new Generation();
        private static int endTaskCounter = 0;
        private static int endThreadCounter = 0;
        readonly static int totalThreads = 100;
        private void StartThreads()
        {
            for (int i = 1; i <= totalThreads; i++)
            {
                Thread thread = new Thread(RunGeneration);
                thread.Start(i);
            }
        }
        private void RunGeneration(object threadNumber)
        {
            var result = generation.Results();
            UpdateDataGridView(result.Item1, result.Item2, (int)threadNumber, Interlocked.Increment(ref endThreadCounter));
        }
        private void UpdateDataGridView(double executionTime, int sysNum, int startNum, int endNum)
        {
            if (dataGridView1.InvokeRequired)
            {
                dataGridView1.Invoke(new MethodInvoker(delegate {
                    dataGridView1.Rows.Add(
                        startNum,
                        endNum,
                        executionTime,
                        sysNum
                    );
                }));
            }
            else
            {
                dataGridView1.Rows.Add(
                    startNum,
                    endNum,
                    executionTime,
                    sysNum
                );
            }
        }
        private async Task StartTasksAsync()
        {
            List<Task<(double, int, int, int)>> tasks = new List<Task<(double, int, int, int)>>();

            for (int i = 1; i <= totalThreads; i++)
                tasks.Add(RunGenerationAsync(i));

            var results = await Task.WhenAll(tasks);

            foreach (var result in results)
                UpdateDataGridView(result.Item1, result.Item2, result.Item3, result.Item4);
        }
        private async Task<(double, int, int, int)> RunGenerationAsync(int startNum)
        {
            return await Task.Run(() =>
            {
                var result = generation.Results();
                return (result.Item1, result.Item2, startNum, Interlocked.Increment(ref endTaskCounter));
            });
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text = "Общее время: ";
            endThreadCounter = 0;
            endTaskCounter = 0;
            dataGridView1.Rows.Clear();
            dataGridView2.Rows.Clear();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Stopwatch totalStopwatch = new Stopwatch();
            totalStopwatch.Start();

            StartThreads();

            totalStopwatch.Stop();

            double totalTime = totalStopwatch.Elapsed.TotalMilliseconds;
            textBox1.Text = $"Общее время:\n{totalTime} мс";
        }
        private async void button3_Click(object sender, EventArgs e)
        {
            Stopwatch totalStopwatch = new Stopwatch();
            totalStopwatch.Start();

            await StartTasksAsync();

            totalStopwatch.Stop();

            double totalTime = totalStopwatch.Elapsed.TotalMilliseconds;
            textBox1.Text = $"Общее время:\n{totalTime} мс";
        }
        private void button4_Click(object sender, EventArgs e)
        {
            int rowCount = dataGridView1.Rows.Count;
            double minExecutionTime = double.MaxValue;
            double maxExecutionTime = double.MinValue;
            for (int i = 0; i < rowCount; i++)
            {
                DataGridViewRow row = dataGridView1.Rows[i];
                double executionTime = Convert.ToDouble(row.Cells[2].Value);
                minExecutionTime = Math.Min(minExecutionTime, executionTime);
                maxExecutionTime = Math.Max(maxExecutionTime, executionTime);
            }

            double intervalStep = (maxExecutionTime - minExecutionTime) / 10;
            double currentIntervalStart = minExecutionTime;
            double currentIntervalEnd = minExecutionTime + intervalStep;

            for (int i = 0; i < 10; i++)
            {
                int amount = 0;

                for (int j = 0; j < dataGridView1.Rows.Count; j++)
                {
                    DataGridViewRow row = dataGridView1.Rows[j];
                    double executionTime = Convert.ToDouble(row.Cells[2].Value);

                    if (executionTime >= currentIntervalStart && executionTime <= currentIntervalEnd)
                    {
                        amount++;
                    }
                }

                string piece = $"{currentIntervalStart:F0}-{currentIntervalEnd:F0}";
                dataGridView2.Rows.Add(piece, amount);

                currentIntervalStart = currentIntervalEnd;
                currentIntervalEnd += intervalStep;
            }
            double sumExecutionTime = 0;

            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                DataGridViewRow row = dataGridView1.Rows[i];
                double executionTime = Convert.ToDouble(row.Cells[2].Value);
                sumExecutionTime += executionTime;
            }

            double averageExecutionTime = sumExecutionTime / dataGridView1.Rows.Count;
            dataGridView2.Rows.Add("Среднее", 0);
            dataGridView2.Rows[10].Cells[1].Value = averageExecutionTime;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

    }
}

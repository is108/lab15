using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace laba_15
{
    public partial class Form1 : Form
    {
        MathStatistic stat;
        int state; 
        Random rnd = new Random();
        double a, time;
        double[] Q;

        public Form1()
        {
            InitializeComponent();
            stat = new MathStatistic();
            Q = new double[3] { -0.4, -0.8, -0.5 };
        }


        private void button1_Click(object sender, EventArgs e)
        {
            time = 0;
            state = 0;
            chart1.Series[0].Points.Clear();
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            chart1.Series[0].Points.AddXY((int)time, state + 1);
            a = rnd.NextDouble();
            time += Math.Log(a) / Q[state];
            state = stat.Modeling(a);
        }


        private void btnStop_Click(object sender, EventArgs e)
        {
            if (timer1.Enabled)
            {
                timer1.Stop();
            }
            else
            {
                timer1.Start();
            }
        }

        // сравнение эмпирического распределения вероятностей с теоретическим финальным
        private void btnCompare_Click(object sender, EventArgs e)
        {
            chart2.Series[0].Points.Clear();
            chart2.Series[1].Points.Clear();
            stat.Statistic();
            for(int i = 0; i < 3; i++)
            {
                chart2.Series[0].Points.AddXY(i + 1, stat.Frequency[i]);
                chart2.Series[1].Points.AddXY(i + 1, stat.Final[i]);
            }
        }

        class MathStatistic
        {
            // матрица интенсивностей переходов
            double[,] Q = new double[3, 3] { { -0.4, 0.3, 0.1 },
                                             { 0.4, -0.8, 0.4 },
                                             { 0.1, 0.4, -0.5 } };
            int N = 1000;
            int T = 1000;
            int state = 0, k; 
            Random rnd = new Random();
            double a, p;
            public double[] Frequency = new double[3]; 
            public double[] Final = new double[3]; 

            public int Modeling(double a) 
            {
                for (int i = 0; i < 3; i++)
                {
                    // новое состояние - дискретная случайная величина с рядом распределения
                    if (state != i)
                    {
                        a -= Q[state, i] / Math.Abs(Q[state, state]);
                    }
                    else a -= 0;

                    if (a < 0)
                    {
                        state = i;
                        return state;
                    }
                }
                return state;
            }
            
            public void Statistic()
            {
                state = 0;
                for(int i = 0; i < 3; i++)
                {
                    Frequency[i] = 0;
                    Final[i] = 0;
                }

                TheoreticalProbability();
                StatisticalProcessing();
            }

            // статистическая обработка
            public void StatisticalProcessing()
            {
                k = 0;
                if (k < N)
                {
                    for (int i = 0; i < N; i++)
                    {
                        for (double j = 0; j < T;)
                        {
                            a = rnd.NextDouble();
                            NewStateGenerator();
                            j += Math.Log(a) / Q[state, state];
                        }
                        Frequency[state] = Frequency[state] + 1;
                        k++;
                    }
                }
                // эмпирические распределения
                for (int i = 0; i < 3; i++) Frequency[i] /= N;
            }

            public int NewStateGenerator()
            {
                for(int i  = 0; i < 3; i++)
                {
                    // новое состояние - дискретная случайная величина с рядом распределения
                    if (state != i)
                    {
                        a -= Q[state, i] / Math.Abs(Q[state, state]);
                    }
                    else a -= 0;

                    if(a < 0)
                    {
                        state = i;
                        return state;
                    }
                }
                return state;
            }

            // теоретическое финальное распределение
            public void TheoreticalProbability()
            {
                for(int i = 0; i < 3; i++)
                {
                    /* Получаем эти коэффициенты решая систему уравнений
                       Pi1(-0.4) + Pi2(0.3) + Pi3(0.1) = 0
                       Pi1(0.3) + Pi2(-0.8) + Pi3(0.4) = 0
                       Pi1 + Pi2 + Pi3 = 1
                    */
                    Final[0] = 0.37; 
                    Final[1] = 0.31;
                    Final[2] = 0.32;
                }
            }
        }
    }
}

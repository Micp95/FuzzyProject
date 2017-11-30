using FuzzyProject.Fuzzy;
using FuzzyProject.Fuzzy.JConfig;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace FuzzyProject
{
    public partial class Form1 : Form
    {
        private FuzzyModel _fuzzyModel;

        public Form1()
        {
            InitializeComponent();

            //FuzzyConfig config = new FuzzyConfig();
            //config.Regules = new List<string>()
            //{
            //    "xd",
            //    "more"
            //};
            //config.Variables = new List<VariableConfig>()
            //{
            //    new VariableConfig()
            //    {
            //        Name="name",
            //        Range = new Range()
            //        {
            //            Min = 10,
            //            Max = 100

            //        },
            //        Type = VariableType.Input,
            //        sets = new List<SetConfig>()
            //        {
            //            new SetConfig()
            //            {
            //                Name="zmienna",
            //                Type=SetType.TrapezoidalFunctionCenter,
            //                Values=new List<float>()
            //                {
            //                    1,2,3
            //                }
            //            }
            //        }
            //    }

            //};
            //File.WriteAllText("config.json", JsonConvert.SerializeObject(config));


            InitializeModel();
        }


        private void InitializeModel()
        {
            using (StreamReader file = File.OpenText(@"config.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                FuzzyConfig config = (FuzzyConfig)serializer.Deserialize(file, typeof(FuzzyConfig));

                List<Chart> charts = new List<Chart>();
                charts.Add(chart1);
                charts.Add(chart2);
                charts.Add(chart3);
                charts.Add(chart4);

                _fuzzyModel = new FuzzyModel(config, charts);
                _fuzzyModel.DrawCharts();
            }


        }

        private void ChartClickEvent(EventArgs e, string chartName, Chart chart, Label label)
        {
            var click = (MouseEventArgs)e;
            var chartPosition = chart.HitTest(click.X, click.Y);
            if (chartPosition.ChartElementType == ChartElementType.DataPoint)
            {
                var point = (DataPoint)chartPosition.Object;
                float newValue = (float)point.XValue;
                _fuzzyModel.SetInput(chartName, newValue);
                label.Text = newValue.ToString("0.00");
            }
            else if (chartPosition.ChartElementType == ChartElementType.Annotation)
            {
                var ann = (Annotation)chartPosition.Object;
                float newValue = (float)ann.X;
                _fuzzyModel.SetInput(chartName, newValue);
                label.Text = newValue.ToString("0.00");
            }

            labelOutput.Text = string.Format("{0} (Per: {1} Value: {2})", _fuzzyModel.SystenOutputName, _fuzzyModel.SystemOutputPercentage.ToString("0.00"), _fuzzyModel.SystemOutputValue.ToString("0.00"));
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            //chart1.AccessibilityObject;
        }

        private void chart1_Click(object sender, EventArgs e)
        {
            ChartClickEvent(e, "InputLeft", chart1, labelLeft);
        }

        private void chart2_Click(object sender, EventArgs e)
        {
            ChartClickEvent(e, "InputCenter", chart2,labelCenter);
        }

        private void chart3_Click(object sender, EventArgs e)
        {
            ChartClickEvent(e, "InputRight", chart3,labelRight);
        }
    }
}

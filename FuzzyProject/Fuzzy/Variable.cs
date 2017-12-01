using Accord.Fuzzy;
using FuzzyProject.Fuzzy.JConfig;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;

namespace FuzzyProject.Fuzzy
{
    public class Variable
    {
        private Chart _chart;
        private VerticalLineAnnotation _chartLine;
        private ChartArea _chartArea;
        private Range _range;
        private List<FuzzySet> _sets;
        private LinguisticVariable _fVariable;
        static private int _sampling = 100;

        public String Name { get; set; }
        public VariableType Type { get; set; }

        public float? ResultValue { get; set; }

        public LinguisticVariable FVariable
        {
            get
            {
                return _fVariable;
            }
        }

        public Variable(VariableConfig config, Chart chart)
        {
            _chart = chart;
            _range = config.Range;
            Name = config.Name;
            Type = config.Type;
            _sets = new List<FuzzySet>();
            ResultValue = null;

            foreach (SetConfig setConfig in config.sets)
            {
                IMembershipFunction membershipFunction = null;
                switch (setConfig.Type)
                {
                    case SetType.TrapezoidalFunctionCenter:
                        membershipFunction = new TrapezoidalFunction(setConfig.Values[0], setConfig.Values[1], setConfig.Values[2], setConfig.Values[3]);
                        break;
                    case SetType.TrapezoidalFunctionLeft:
                        membershipFunction = new TrapezoidalFunction(setConfig.Values[0], setConfig.Values[1], TrapezoidalFunction.EdgeType.Right);
                        break;
                    case SetType.TrapezoidalFunctionRight:
                        membershipFunction = new TrapezoidalFunction(setConfig.Values[0], setConfig.Values[1], TrapezoidalFunction.EdgeType.Left);
                        break;
                    default:
                        break;
                }

                FuzzySet newSet = new FuzzySet(setConfig.Name, membershipFunction);
                _sets.Add(newSet);
            }

            Initialize();
        }

        private void Initialize()
        {
            _fVariable = new LinguisticVariable(Name, _range.Min, _range.Max);
            foreach(FuzzySet label in _sets)
            {
                _fVariable.AddLabel(label);
            }

            _chartArea = _chart.ChartAreas[0];
            _chartArea.AxisY.Maximum = 1;
            _chartArea.AxisY.Minimum = 0;

            _chartArea.AxisX.LabelStyle.Format = "{0}";
            _chartArea.AxisX.LabelStyle.Interval = 2;
            _chartArea.AxisY.LabelStyle.Format = "{0.0}";
            _chartArea.AxisY.LabelStyle.Interval = 0.5;

            //_chart = System.Windows.Forms.DockStyle.Bottom;
            _chart.Legends.ToList().ForEach(x => x.Docking = Docking.Bottom);

            if (Type == VariableType.Input)
                InitialSelectLine();
        }

        private void InitialSelectLine()
        {
            _chartLine = new VerticalLineAnnotation();
            _chartLine.AxisX = _chartArea.AxisX;
            _chartLine.AllowMoving = true;
            _chartLine.IsInfinitive = true;
            _chartLine.ClipToChartArea = _chartArea.Name;
            //_chartLine.Name = "Select";
            _chartLine.LineColor = Color.Red;
            _chartLine.LineWidth = 3;
            _chartLine.X = _range.Min;

            ResultValue = _range.Min;

            _chart.Annotations.Add(_chartLine);
        }

        private void ClearChart()
        {
            _chart.Series.Clear();
            foreach (FuzzySet label in _sets)
            {
                _chart.Series.Add(new Series(label.Name));
            }
        }




        public void DrawChartAndSelectSection()
        {
            if( Type == VariableType.Input)
            {
                ClearChart();
                float delta = (_range.Max - _range.Min) / _sampling;

                foreach (var label in _sets)
                {
                    for (float x = _range.Min; x <= _range.Max; x += delta)
                    {
                        _chart.Series.Where(xx => xx.Name == label.Name).FirstOrDefault().Points.AddXY(x, _fVariable.GetLabelMembership(label.Name, x));
                    }
                }

                _chartLine.X = (double)ResultValue;
                _chart.Update();
            }
        }
        public void DrawChartAndActiveSection(Dictionary<string, float> values)
        {
            if (Type == VariableType.Ouput)
            {
                ClearChart();
                float delta = (_range.Max - _range.Min) / _sampling;

                foreach (var label in _sets)
                {
                    for (float x = _range.Min; x <= _range.Max; x += delta)
                    {
                        float y = _fVariable.GetLabelMembership(label.Name, x);

                        if (!values.ContainsKey(label.Name))
                            y = 0;
                        else if (y > values[label.Name])
                            y = values[label.Name];

                        _chart.Series.Where(xx => xx.Name == label.Name).FirstOrDefault().Points.AddXY(x, y);
                    }
                }
                _chart.Update();
                _chart.Show();
            }
       }


    }
}

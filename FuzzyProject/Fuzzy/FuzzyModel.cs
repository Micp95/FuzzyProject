using Accord.Fuzzy;
using FuzzyProject.Fuzzy.JConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;

namespace FuzzyProject.Fuzzy
{
    public class FuzzyModel
    {
        private InferenceSystem _system;
        private Database _fuzzyDB;
        private string _outputName;
        private List<Variable> _allVariable;
        private CentroidDefuzzifier _centroidDefuzzifier;

        private string _systenOutputName;
        private float _systemOutputPercentage;
        private float _systemOutputValue;

        public string SystenOutputName
        {
            get
            {
                return _systenOutputName;
            }
        }
        public float SystemOutputPercentage
        {
            get
            {
                return _systemOutputPercentage;
            }
        }
        public float SystemOutputValue
        {
            get
            {
                return _systemOutputValue;
            }
        }

        public FuzzyModel(FuzzyConfig config, List<Chart> charts)
        {
            List<Variable> veriables = LoadVariables(config.Variables, charts);

            Initialize(veriables, config.Regules);
        }

        private List<Variable> LoadVariables(List<VariableConfig> configs, List<Chart> charts)
        {
            _allVariable = new List<Variable>();
            int chartCounter = 0;

            foreach(VariableConfig varConf in configs)
            {
                _allVariable.Add(new Variable(varConf, charts[chartCounter++]));
            }

            return _allVariable;
        }

        private void Initialize(List<Variable> variables, List<String> roles)
        {
            _fuzzyDB = new Database();

            foreach (var variable in variables)
            {
                _fuzzyDB.AddVariable(variable.FVariable);

                if (variable.Type == JConfig.VariableType.Ouput)
                    _outputName = variable.FVariable.Name;
            }

            _centroidDefuzzifier = new CentroidDefuzzifier(1000);
            _system = new InferenceSystem(_fuzzyDB, _centroidDefuzzifier);

            foreach (var variable in variables)
            {
                if (variable.Type == JConfig.VariableType.Input)
                    _system.SetInput(variable.Name, (float)variable.ResultValue);
            }

            int roleCounter = 0;
            foreach (var role in roles)
            {
                _system.NewRule("Role " + roleCounter++, role);
            }

        }

        private Dictionary<string,float> GetOutput ()
        {
            Dictionary<string, float> output = new Dictionary<string, float>();

            FuzzyOutput fuzzyOutput = _system.ExecuteInference(_outputName);

            foreach (var oc in fuzzyOutput.OutputList)
            {
                if (!output.ContainsKey(oc.Label))
                    output.Add(oc.Label, oc.FiringStrength);
                else if (output[oc.Label] < oc.FiringStrength)
                    output[oc.Label] = oc.FiringStrength;
            }
            SetSystemOutput(fuzzyOutput);

            return output;
        }

        private void SetSystemOutput(FuzzyOutput fuzzyOutput)
        {
            if(fuzzyOutput.OutputList.Count != 0)
            {
                _systemOutputValue = _centroidDefuzzifier.Defuzzify(fuzzyOutput, new MinimumNorm());
                _systemOutputPercentage = fuzzyOutput.OutputList.Max(x=>x.FiringStrength);
                _systenOutputName = fuzzyOutput.OutputList.First(x => x.FiringStrength == _systemOutputPercentage).Label;

            }else
            {
                _systemOutputValue = float.NaN;
                _systenOutputName = "STOP";
                _systemOutputPercentage = 1;
            }
        }

        public void DrawCharts()
        {
            _allVariable.Where(x => x.Type == VariableType.Input).ToList().ForEach(x => x.DrawChartAndSelectSection());
            _allVariable.Where(x => x.Type == VariableType.Ouput).FirstOrDefault().DrawChartAndActiveSection(GetOutput());
        }

        public void SetInput (string inputName, float value)
        {
            _system.SetInput(inputName, value);
            _allVariable.Where(x => x.Name.Equals(inputName)).FirstOrDefault().ResultValue = value;

            DrawCharts();
        }


    }
}

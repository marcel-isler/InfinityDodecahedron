using ColorMine.ColorSpaces;
using DodecahedronHandler.Interfaces;
using System;
using System.Collections.Generic;

namespace DodecahedronHandler.Effects
{
    public class Sparkler : ILedEffect
    {
        private int _currentStep;
        private readonly int _maxSteps;
        private readonly bool _randomColor;
        private readonly Rgb _sparkleColor;
        private readonly double _sparklerPercent;

        public Sparkler(IDictionary<string, string> config)
        {
            _currentStep = 0;
            _sparkleColor = new Rgb();

            if (config.TryGetValue("MaxSteps", out string value))
            {
                if (!int.TryParse(value, out _maxSteps))
                {
                    _maxSteps = int.MaxValue;
                }
            }
            else
            {
                _maxSteps = int.MaxValue;
            }

            if (config.TryGetValue("SparklerPercent", out string value1))
            {
                if (!double.TryParse(value1, out _sparklerPercent))
                {
                    _sparklerPercent = 1.0;
                }
            }
            else
            {
                _sparklerPercent = 1.0;
            }

            if (config.TryGetValue("ColorValue", out string value2))
            {
                string[] sparkleComponent = value2.Split(',');
                if (sparkleComponent.Length == 3)
                {
                    _sparkleColor.R = int.Parse(sparkleComponent[0]);
                    _sparkleColor.G = int.Parse(sparkleComponent[1]);
                    _sparkleColor.B = int.Parse(sparkleComponent[2]);
                }
                else
                {
                    _randomColor = true;
                }
            }
            else
            {
                _randomColor = true;
            }
        }

        public bool NextStep(LedBuffer ledBuffer)
        {
            Random r = new Random();

            for (int i = 0; i < ledBuffer.NumberOfLeds; i++)
            {
                double chance = r.NextDouble() * 100.0;
                if (chance < _sparklerPercent)
                {
                    if (_randomColor)
                    {
                        _sparkleColor.R = r.Next(0, 255);
                        _sparkleColor.G = r.Next(0, 255);
                        _sparkleColor.B = r.Next(0, 255);
                    }
                    ledBuffer.SetPixel(i, _sparkleColor);
                }
            }

            _currentStep++;

            bool keepGoing = _currentStep > _maxSteps;
            return keepGoing;
        }
    }
}

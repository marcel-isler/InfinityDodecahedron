using ColorMine.ColorSpaces;
using DodecahedronHandler.Interfaces;
using System;
using System.Collections.Generic;

namespace DodecahedronHandler.Effects
{
    public class Solid : ILedEffect
    {
        private int _currentStep;
        private readonly int _maxSteps;
        private readonly Rgb _chaserColor;

        public Solid(IDictionary<string, string> config)
        {
            _currentStep = 0;
            _chaserColor = new Rgb();

            if (config.TryGetValue("MaxSteps", out string value1))
            {
                if (!int.TryParse(value1, out _maxSteps))
                {
                    _maxSteps = int.MaxValue;
                }
            }
            else
            {
                _maxSteps = int.MaxValue;
            }

            if (config.TryGetValue("ColorValue", out string value))
            {
                string[] sparkleComponent = value.Split(',');
                if (sparkleComponent.Length == 3)
                {
                    _chaserColor.R = int.Parse(sparkleComponent[0]);
                    _chaserColor.G = int.Parse(sparkleComponent[1]);
                    _chaserColor.B = int.Parse(sparkleComponent[2]);
                }
                else
                {
                    _chaserColor.R = 255;
                    _chaserColor.G = 255;
                    _chaserColor.B = 255;
                }
            }
            else
            {
                _chaserColor.R = 255;
                _chaserColor.G = 255;
                _chaserColor.B = 255;
            }
        }

        public bool NextStep(LedBuffer ledBuffer)
        {
            for (int i = 0; i < ledBuffer.NumberOfLeds; i++)
            {
                ledBuffer.ForcePixel(i, _chaserColor);
            }
            _currentStep++;

            bool keepGoing = _currentStep > _maxSteps;
            _currentStep = 0;
            return keepGoing;
        }
    }
}

using ColorMine.ColorSpaces;
using DodecahedronHandler.Interfaces;
using System;
using System.Collections.Generic;

namespace DodecahedronHandler.Effects
{
    public class Pulse : ILedEffect
    {
        private readonly int _cycleSteps;
        private int _cycles;
        private double _stepSize;
        private double _currentStep;
        private Rgb _pixelValue = new Rgb();
        private int _cyclesSetting;

        public Pulse(IDictionary<string, string> config)
        {
            _currentStep = 0.0;

            if (config.TryGetValue("CycleSteps", out string valueCycleSteps))
            {
                if (!int.TryParse(valueCycleSteps, out _cycleSteps))
                {
                    _cycleSteps = int.MaxValue;
                }
            }
            else
            {
                _cycleSteps = int.MaxValue;
            }

            if (config.TryGetValue("Cycles", out string valueCycles))
            {
                if (!int.TryParse(valueCycles, out _cycles))
                {
                    _cycles = int.MaxValue;
                }
            }
            else
            {
                _cycles = int.MaxValue;
            }

            _cyclesSetting = _cycles;

            _stepSize = 1.0 / ((double)_cycleSteps / 2.0);
        }
        public string GetName()
        {
            return $"Pulse in {_cycleSteps} steps";
        }

        public bool NextStep(LedBuffer ledBuffer)
        {
            _currentStep += _stepSize;
            if (_currentStep > 1.0)
            {
                _currentStep = 1.0;
                _stepSize *= -1.0;
            }
            else if (_currentStep < 0)
            {
                _currentStep = 0.0;
                _stepSize *= -1.0;
                _cycles--;
            }

            for (int i = 0; i < ledBuffer.NumberOfLeds; i++)
            {
                ledBuffer.GetPixel(i, ref _pixelValue);
                _pixelValue.R *= _currentStep;
                _pixelValue.G *= _currentStep;
                _pixelValue.B *= _currentStep;
                _pixelValue.R = Math.Max(0.0,  Math.Min(255.0, _pixelValue.R));
                _pixelValue.G = Math.Max(0.0,  Math.Min(255.0, _pixelValue.G));
                _pixelValue.B = Math.Max(0.0,  Math.Min(255.0, _pixelValue.B));
                ledBuffer.ForcePixel(i, _pixelValue);
            }

            bool keepGoing = _cycles < 0.0;
            _cycles = _cyclesSetting;
            return keepGoing;
        }
    }
}

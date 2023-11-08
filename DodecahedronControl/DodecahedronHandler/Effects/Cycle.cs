using ColorMine.ColorSpaces;
using DodecahedronHandler.Interfaces;
using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;

namespace DodecahedronHandler.Effects
{
    public class Cycle : ILedEffect
    {
        private int _currentStep;
        private readonly int _maxSteps;
        private readonly int _cycleSteps;
        private readonly Rgb _chaserColor;

        private readonly List<Rgb> _colors = new List<Rgb>();
        private int _currentColor;
        private readonly int _colorSteps;
        private int _currentColorStep;

        public Cycle(IDictionary<string, string> config)
        {
            _currentStep = 0;
            _chaserColor = new Rgb();

            _colors.Add(new Rgb() { R = 71.8, G = 19.2, B = 27.8 }); // Red
            _colors.Add(new Rgb() { R = 91.0, G = 25.1, B = 12.9 }); // Orange-Red
            _colors.Add(new Rgb() { R = 95.3, G = 38.0, B = 18.8 }); // Red-Orange
            _colors.Add(new Rgb() { R = 90.6, G = 43.5, B = 3.9 }); // Orange
            _colors.Add(new Rgb() { R = 91.0, G = 52.2, B = 1.2 }); // Yellow-Orange
            _colors.Add(new Rgb() { R = 89.8, G = 68.6, B = 0.0 }); // Orange-Yellow
            _colors.Add(new Rgb() { R = 91.4, G = 97.3, B = 2.0 }); // Yellow
            _colors.Add(new Rgb() { R = 53.7, G = 83.5, B = 3.1 }); // Green-Yellow
            _colors.Add(new Rgb() { R = 30.6, G = 72.9, B = 17.3 }); // Yellow-Green
            _colors.Add(new Rgb() { R = 6.7, G = 57.3, B = 19.6 }); // Green
            _colors.Add(new Rgb() { R = 9.8, G = 52.9, B = 55.3 }); // Blue-Green
            _colors.Add(new Rgb() { R = 16.9, G = 47.8, B = 66.7 }); // Green-Blue
            _colors.Add(new Rgb() { R = 24.3, G = 46.7, B = 79.6 }); // Blue
            _colors.Add(new Rgb() { R = 23.1, G = 35.3, B = 65.5 }); // Violet-Blue
            _colors.Add(new Rgb() { R = 26.7, G = 25.9, B = 52.2 }); // Blue-Violet
            _colors.Add(new Rgb() { R = 30.6, G = 21.6, B = 44.3 }); // Violet
            _colors.Add(new Rgb() { R = 58.0, G = 23.1, B = 36.9 }); // Red-Violet
            _colors.Add(new Rgb() { R = 70.2, G = 23.5, B = 30.2 }); // Violet-Red

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

            _colorSteps = Math.Max(1, _cycleSteps / _colors.Count);
        }
        public string GetName()
        {
            return $"Cycle in {_cycleSteps} steps";
        }

        public bool NextStep(LedBuffer ledBuffer)
        {
            int fromColor = _currentColor; 
            int toColor = _currentColor + 1;
            if (toColor >= _colors.Count)
            {
                toColor = 0;
            }

            double redIncrement = (_colors[toColor].R - _colors[fromColor].R) / _colorSteps;
            double greenIncrement = (_colors[toColor].G - _colors[fromColor].G) / _colorSteps;
            double blueIncrement = (_colors[toColor].B - _colors[fromColor].B) / _colorSteps;

            _chaserColor.R = _colors[fromColor].R + (redIncrement * _currentColorStep);
            _chaserColor.G = _colors[fromColor].G + (greenIncrement * _currentColorStep);
            _chaserColor.B = _colors[fromColor].B + (blueIncrement * _currentColorStep);
            //if (_currentColorStep == 0)
            //{
            //    Console.WriteLine($"First cycle color at {_chaserColor.R},{_chaserColor.G},{_chaserColor.B}");
            //}
            //if (_currentColorStep == _colorSteps)
            //{
            //    Console.WriteLine($"Last cycle color at {_chaserColor.R},{_chaserColor.G},{_chaserColor.B} going from{_colors[fromColor].R},{_colors[fromColor].G},{_colors[fromColor].B} to {_colors[toColor].R},{_colors[toColor].G},{_colors[toColor].B} with increment {redIncrement},{greenIncrement},{blueIncrement} in {_colorSteps} steps");
            //}
            Console.WriteLine($"cycle color at {_chaserColor.R},{_chaserColor.G},{_chaserColor.B}");

            for (int i = 0; i < ledBuffer.NumberOfLeds; i++)
            {
                ledBuffer.ForcePixel(i, _chaserColor);
            }
            _currentStep++;

            bool keepGoing = _currentStep > _maxSteps;
            _currentStep = 0;

            _currentColorStep++;
            if (_currentColorStep > _colorSteps)
            {
                _currentColorStep = 0;
                _currentColor++;
                if (_currentColor >= _colors.Count)
                {
                    _currentColor = 0;
                }
            }

            return keepGoing;
        }
    }
}

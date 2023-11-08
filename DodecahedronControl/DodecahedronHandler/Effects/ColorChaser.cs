using ColorMine.ColorSpaces;
using DodecahedronHandler.Interfaces;
using System.Collections.Generic;

namespace DodecahedronHandler.Effects
{
    public class ColorChaser : ILedEffect
    {
        private int _currentStep;
        private readonly int _maxSteps;
        private readonly int _chaserLength;
        private readonly Rgb _chaserColor;
        private int _startPixel;

        public ColorChaser(IDictionary<string, string> config)
        {
            _currentStep = 0;
            _startPixel = 0;
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

            if (config.TryGetValue("ChaserLength", out string value2))
            {
                if (!int.TryParse(value2, out _chaserLength))
                {
                    _chaserLength = 32;
                }
            }
            else
            {
                _chaserLength = 32;
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

        public string GetName()
        {
            return $"ColorChaser R={_chaserColor.R} G={_chaserColor.G} B={_chaserColor.B}";
        }
        public bool NextStep(LedBuffer ledBuffer)
        {
            int step = _startPixel;

            for (int i = 0; i < _chaserLength; i++)
            {
                ledBuffer.SetPixel(step, _chaserColor);
                step = ledBuffer.GetNextPixel(step);
            }

            _startPixel = ledBuffer.GetNextPixel(_startPixel);
            _currentStep++;

            bool keepGoing = _currentStep > _maxSteps;
            return keepGoing;
        }
    }
}

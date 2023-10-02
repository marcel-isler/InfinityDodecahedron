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

            if (config.ContainsKey("MaxSteps"))
            {
                if (!int.TryParse(config["MaxSteps"], out _maxSteps))
                {
                    _maxSteps = int.MaxValue;
                }
            }
            else
            {
                _maxSteps = int.MaxValue;
            }

            if (config.ContainsKey("ChaserLength"))
            {
                if (!int.TryParse(config["ChaserLength"], out _chaserLength))
                {
                    _chaserLength = 32;
                }
            }
            else
            {
                _chaserLength = 32;
            }

            if (config.ContainsKey("ColorValue"))
            {
                string[] sparkleComponent = config["ColorValue"].Split(',');
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

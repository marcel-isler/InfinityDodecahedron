using ColorMine.ColorSpaces;
using DodecahedronHandler.Interfaces;
using System.Collections.Generic;

namespace DodecahedronHandler.Effects
{
    public class RainbowChaser : ILedEffect
    {
        private int _currentStep;
        private readonly int _maxSteps;
        private readonly int _rainbowLength;
        private int _startPixel;

        public RainbowChaser(IDictionary<string, string> config)
        {
            _currentStep = 0;
            _startPixel = 0;
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

            if (config.ContainsKey("RainbowLength"))
            {
                if (!int.TryParse(config["RainbowLength"], out _rainbowLength))
                {
                    _rainbowLength = 32;
                }
            }
            else
            {
                _rainbowLength = 32;
            }
        }

        public bool NextStep(LedBuffer ledBuffer)
        {
            int step = _startPixel;

            for (int i = 0; i < _rainbowLength; i++)
            {
                double angle = (step % _rainbowLength) * (360.0 / _rainbowLength);
                Hsv hsv = new Hsv { H = angle, S = 1.0, V = 0.3 };
                Rgb rgb = hsv.To<Rgb>();
                ledBuffer.SetPixel(step, rgb);
                step = ledBuffer.GetNextPixel(step);
            }

            _startPixel = ledBuffer.GetNextPixel(_startPixel);
            _currentStep++;

            bool keepGoing = _currentStep > _maxSteps;
            return keepGoing;
        }
    }
}

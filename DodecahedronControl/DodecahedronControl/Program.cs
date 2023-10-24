using OpenPixelControl;
using System;
using System.Configuration;
using System.Net;
using System.Threading;
using DodecahedronHandler;
using DodecahedronHandler.Effects;
using DodecahedronHandler.Interfaces;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.ComponentModel.Design;

namespace DodecahedronControl
{
    class Program
    {
        private static readonly ConcurrentDictionary<int, ILedEffect> Effects = new ConcurrentDictionary<int, ILedEffect>();
        private static readonly List<int> EffectsToRemove = new List<int>();
        private static int _effectId;
        private static LedBuffer  _ledBuffer = new LedBuffer(32);
        private static OpenPixelControlClient _client;
        private static bool _keepGoing = true;
        private static void Main(string[] args)
        {
            Console.WriteLine("Starting Dodecahedron");
            string fadeCandyAddress = ConfigurationManager.AppSettings["FadeCandyAddress"];

            IPAddress[] addresses = Dns.GetHostAddresses(fadeCandyAddress);
            if (addresses.Length == 0)
            {
                throw new ArgumentException(
                    "Unable to retrieve address from specified host name.",
                    "hostName"
                );
            }
            else if (addresses.Length > 1)
            {
                throw new ArgumentException("There is more that one IP address to the specified host.", "hostName");
            }

            // Configure the FadeCandy endpoint
            IPEndPoint endPoint = new IPEndPoint(addresses[0], 7890);
            _client = new OpenPixelControlClient(endPoint, true, false);

            // this is the LED buffer used to store the current frame
            Dictionary<ConsoleKey, ILedEffect> effectSet = new Dictionary<ConsoleKey, ILedEffect>();
            // create an instance of the Rainbow Chaser
            ILedEffect rainbowChaser = new RainbowChaser(new Dictionary<string, string> {{ "MaxSteps", "1316" }, { "RainbowLength", "16" }});
            effectSet.Add(ConsoleKey.R, rainbowChaser);
            ILedEffect sparkler1 = new Sparkler(new Dictionary<string, string> { { "MaxSteps", "400" }, { "SparklerPercent", "5" }, { "ColorValue", "255,0,0" } });
            effectSet.Add(ConsoleKey.D1, sparkler1);
            ILedEffect sparkler2 = new Sparkler(new Dictionary<string, string> { { "MaxSteps", "400" }, { "SparklerPercent", "5" }, { "ColorValue", "0,255,0" } });
            effectSet.Add(ConsoleKey.D2, sparkler2);
            ILedEffect sparkler3 = new Sparkler(new Dictionary<string, string> { { "MaxSteps", "400" }, { "SparklerPercent", "5" }, { "ColorValue", "0,0,255" } });
            effectSet.Add(ConsoleKey.D3, sparkler3);
            ILedEffect colorChaser = new ColorChaser(new Dictionary<string, string> { { "MaxSteps", "3200" }, { "ChaserLength", "2" }, { "ColorValue", "255,255,255" } });
            effectSet.Add(ConsoleKey.C, colorChaser);
            ILedEffect solidRed = new Solid(new Dictionary<string, string> { { "MaxSteps", "3200" }, { "ColorValue", "255,0,0" } });
            effectSet.Add(ConsoleKey.D8, solidRed);
            ILedEffect solidGreen = new Solid(new Dictionary<string, string> { { "MaxSteps", "3200" }, { "ColorValue", "0,255,0" } });
            effectSet.Add(ConsoleKey.D9, solidGreen);
            ILedEffect solidBlue = new Solid(new Dictionary<string, string> { { "MaxSteps", "3200" }, { "ColorValue", "0,0,255" } });
            effectSet.Add(ConsoleKey.D0, solidBlue);
            ILedEffect pulse = new Pulse(new Dictionary<string, string> { { "CycleSteps", (2000 / 50).ToString()}, { "Cycles", "2000" } });
            effectSet.Add(ConsoleKey.P, pulse); 
            //            ILedEffect sparkler = new Sparkler(new Dictionary<string, string> { { "MaxSteps", "400" }, { "SparklerPercent", "5" } });

            // add the effect to the list of effects that run
            Effects[_effectId++] = colorChaser;

            Thread driverThread = new Thread(DrivePixels);
            driverThread.Start();
            Help(effectSet);
            while (_keepGoing)
            {
                ConsoleKeyInfo struckKey = Console.ReadKey();
                switch (struckKey.Key)
                {
                    case ConsoleKey.Spacebar:
                        Console.WriteLine();
                        Console.WriteLine("Clearing");
                        Reset();
                        break;
                    case ConsoleKey.X:
                        _keepGoing = false;
                        Console.WriteLine();
                        Console.WriteLine("Bye!");
                        Thread.Sleep(100);
                        Reset();

                        break;
                    default:
                        if (effectSet.TryGetValue(struckKey.Key, out ILedEffect effect))
                        {
                            Console.WriteLine();
                            Console.WriteLine($"Adding {effect.GetType().Name}");
                            Effects[_effectId++] = effect;
                        }
                        else
                        {
                            Console.WriteLine("Key does not map to an effect.");
                            Help(effectSet);
                        }
                        break;
                }
            }
        }

        private static void Help(Dictionary<ConsoleKey, ILedEffect> effectSet)
        {
            Console.WriteLine("Press a key to add an effect:");
            foreach (KeyValuePair<ConsoleKey, ILedEffect> entry in effectSet)
            {
                Console.WriteLine($"   {entry.Key}: {entry.Value.GetType().Name}");
            }

            Console.WriteLine("<space>: clears all effects");
            Console.WriteLine("X: exits the program");
        }

        private static void DrivePixels()
        {

            try
            {
                while (_keepGoing)
                {
                    _ledBuffer.ClearBuffer();

                    foreach (KeyValuePair<int, ILedEffect> effectEntry in Effects)
                    {
                        bool effectIsDone = effectEntry.Value.NextStep(_ledBuffer);
                        if (effectIsDone)
                        {
                            EffectsToRemove.Add(effectEntry.Key);
                            Console.WriteLine($"Finished {effectEntry.Value.GetType().Name}");
                        }
                    }
                    // send the current frame to the FadeCandy controller
                    _client.SendMessage(0, _ledBuffer.NumberOfLeds, _ledBuffer.LedValues);

                    // remove all the effects that are done
                    foreach (int id in EffectsToRemove)
                    {
                        Effects.Remove(id, out ILedEffect _);
                    }
                    Thread.Sleep(50);
                }
            }
            catch (Exception ex)
            {

            }
        }

        private static void Reset()
        {
            // clear the display
            Effects.Clear();
            _ledBuffer.ClearBuffer();
            _client.SendMessage(0, _ledBuffer.NumberOfLeds, _ledBuffer.LedValues);
        }
    }
}

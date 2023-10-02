using OpenPixelControl;
using System;
using System.Configuration;
using System.Net;
using System.Threading;
using DodecahedronHandler;
using DodecahedronHandler.Effects;
using DodecahedronHandler.Interfaces;
using System.Collections.Generic;

namespace DodecahedronControl
{
    class Program
    {
        private static readonly Dictionary<int, ILedEffect> Effects = new Dictionary<int, ILedEffect>();
        private static readonly List<int> EffectsToRemove = new List<int>();
        private static int _effectId;

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
            OpenPixelControlClient client = new OpenPixelControlClient(endPoint, true, false);

            // this is the LED buffer used to store the current frame
            LedBuffer ledBuffer = new LedBuffer(32);

            // create an instance of the Rainbow Chaser
            ILedEffect rainbowChaser = new RainbowChaser(new Dictionary<string, string> {{ "MaxSteps", "1316" }, { "RainbowLength", "16" }});
            ILedEffect sparkler1 = new Sparkler(new Dictionary<string, string> { { "MaxSteps", "400" }, { "SparklerPercent", "5" }, { "ColorValue", "255,0,0" } });
            ILedEffect sparkler2 = new Sparkler(new Dictionary<string, string> { { "MaxSteps", "400" }, { "SparklerPercent", "5" }, { "ColorValue", "0,255,0" } });
            ILedEffect sparkler3 = new Sparkler(new Dictionary<string, string> { { "MaxSteps", "400" }, { "SparklerPercent", "5" }, { "ColorValue", "0,0,255" } });
            ILedEffect colorChaser = new ColorChaser(new Dictionary<string, string> { { "MaxSteps", "3200" }, { "ChaserLength", "1" }, { "ColorValue", "255,255,255" } });
            //            ILedEffect sparkler = new Sparkler(new Dictionary<string, string> { { "MaxSteps", "400" }, { "SparklerPercent", "5" } });

            // add the effect to the list of effects that run
            //_effects.Add(_effectId++, rainbowChaser);
            //_effects.Add(_effectId++, sparkler1);
            //_effects.Add(_effectId++, sparkler2);
            //_effects.Add(_effectId++, sparkler3);
            Effects.Add(_effectId++, colorChaser);

            try
            {
                while (Effects.Count > 0)
                {
                    ledBuffer.ClearBuffer();

                    foreach (KeyValuePair<int, ILedEffect> effectEntry in Effects)
                    {
                        bool effectIsDone = effectEntry.Value.NextStep(ledBuffer);
                        if (effectIsDone)
                        {
                            EffectsToRemove.Add(effectEntry.Key);
                        }
                    }
                    // send the current frame to the FadeCandy controller
                    client.SendMessage(0, ledBuffer.NumberOfLeds, ledBuffer.LedValues);

                    // remove all the effects that are done
                    foreach (int id in EffectsToRemove)
                    {
                        Effects.Remove(id);
                    }
                    Thread.Sleep(50);
                }
            }
            catch (Exception ex)
            {

            }

            // clear the display
            ledBuffer.ClearBuffer();
            client.SendMessage(0, ledBuffer.NumberOfLeds, ledBuffer.LedValues);
        }
    }
}

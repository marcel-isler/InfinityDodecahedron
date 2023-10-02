using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace OpenPixelControlClientCore
{
    public class OpenPixelControlClient
    {
        private readonly bool _verbose;
        private readonly bool _longConnection;
        private readonly IPEndPoint _server;
        private Socket _socket;

        public OpenPixelControlClient(IPEndPoint server, bool longConnection = true, bool verbose = false)
        {
            _verbose = verbose;
            _longConnection = longConnection;
            _server = server;
            _socket = null;
        }

        private void Debug(String m)
        {
            if (_verbose)
            {
                Console.WriteLine("    {0}", m);
            }
        }

        /**
         * Setup a connection if one doesn't already exist
         * @return bool True on success, false on failure.
         */
        private bool EnsureConnection()
        {
            if (_socket != null)
            {
                Debug("EnsureConnected: Already connected, doing nothing.");
                return true;
            }

            Debug("EnsureConnected: Trying to connect...");
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socket.Connect(_server);

            if (_socket.Connected)
            {
                Debug("EnsureConnected: Connection success.");
                return true;
            }
            else
            {
                Debug("EnsureConnected: Connection failure.");
                _socket = null;
                return false;
            }
        }

        /**
         * 
         */
        public void Disconnect()
        {
            Debug("Disconnecting.");
            _socket?.Close();
            _socket = null;
        }

        public bool CanConnect()
        {
            bool success = EnsureConnection();
            if (!_longConnection)
            {
                Disconnect();
            }
            return success;
        }

        public bool PutPixels(List<Color> pixels, int channel = 0)
        {
            Debug("PutPixels: Connecting.");
            bool isConnected = EnsureConnection();
            if (!isConnected)
            {
                Debug("PutPixels: Not connected. Ignoring these pixels.");
                return false;
            }

            // Build OPC message
            int lenHiByte = pixels.Count * 3 / 256;
            int lenLoByte = pixels.Count * 3 % 256;
            byte[] header = { (byte)channel, 0, (byte)lenHiByte, (byte)lenLoByte };
            List<byte[]> pieces = new List<byte[]> { header };

            foreach (Color pixel in pixels)
            {
                byte[] segment = { pixel.R, pixel.G, pixel.B };
                pieces.Add(segment);
            }

            byte[] message = pieces.SelectMany(a => a).ToArray();

            // Send the OPC message
            Debug("PutPixels: Sending pixels to server.");
            try
            {
                _socket.Send(message);
            }
            catch (SocketException e)
            {
                Debug($"PutPixels: Could not send message, {e.Message}.");
                _socket = null;
                return false;
            }
            catch (ObjectDisposedException)
            {
                Debug("PutPixels: Could not send message, socket closed.");
                _socket = null;
                return false;
            }

            if (!_longConnection)
            {
                Debug("PutPixels: Disconnecting.");
                Disconnect();
            }

            return true;
        }
    }
}
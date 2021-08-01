﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using OscCore;
using TotalMixVC.Communicator;

namespace TotalMixVC.CLI
{
    internal static class Program
    {
        internal static void Main()
        {
            var volumeManager = new VolumeManager(
                incomingEP: new IPEndPoint(IPAddress.Loopback, 7001),
                outgoingEP: new IPEndPoint(IPAddress.Loopback, 9001))
            {
                VolumeIncrement = 0.01f
            };

            var listenerTask = Task.Run(async () =>
            {
                Console.WriteLine("Listening for received volume updates from the device.");

                while (true)
                {
                    if (await volumeManager.ReceiveVolume().ConfigureAwait(false))
                    {
                        Console.WriteLine($"--- Volume updated to {volumeManager.Volume}");
                    }
                }
            });

            var senderTask = Task.Run(async () =>
            {
                Console.WriteLine("Starting keyboard shortcut monitor for adjusting volume.");

                await volumeManager.GetDeviceVolume().ConfigureAwait(false);

                while (true)
                {
                    ConsoleKeyInfo keyInfo = Console.ReadKey();

                    if (keyInfo.Key == ConsoleKey.UpArrow)
                    {
                        if (await volumeManager.IncreaseVolume().ConfigureAwait(false))
                        {
                            Console.WriteLine($"↑↑↑ Increased volume to {volumeManager.Volume}");
                        }
                    }
                    else
                    {
                        if (await volumeManager.DecreaseVolume().ConfigureAwait(false))
                        {
                            Console.WriteLine($"↓↓↓ Decreased volume to {volumeManager.Volume}");
                        }
                    }
                }
            });

            Task.WaitAll(listenerTask, senderTask);
        }
    }
}

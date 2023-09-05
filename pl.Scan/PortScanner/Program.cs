using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;

namespace PortScanner
{
    class Program
    {
        private static string host = "N/A";
        private static int openPorts = 0;
        private static int closedPorts = 0;
        private static List<string> open = new List<string>();

        static void Main(string[] args)
        {
            Console.SetWindowSize(75, 23);
            Console.Title = "pl.Scan | Multi Threaded Port Scanner";
            main();
            Console.ReadKey();
        }

        static void main()
        {

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("  ");
            Console.WriteLine("        ___                 ");
            Console.WriteLine("   _ __/ __| __ __ _ _ _    ");
            Console.WriteLine("  | '_ \\__ \\/ _/ _` | ' \\");
            Console.WriteLine("  | .__/___/\\__\\__,_|_||_|");
            Console.WriteLine("  |_|                       ");
            Console.WriteLine("          Open [ {0} ]", String.Join(", ", open));

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("");
            Console.WriteLine("");
            Console.Write("Enter a target: ");
            host = Console.ReadLine();
            Console.Write("Starting Port: ");
            string startPort = Console.ReadLine();
            Console.Write("Ending Port: ");
            string endPort = Console.ReadLine();
            Console.Write("Wait time [in MS]: ");
            string waitTime = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(host))
            {
                error(2, "Invalid Target!");
                Thread.Sleep(700);
                Console.Clear();
                main();
            }
            else if (string.IsNullOrWhiteSpace(startPort))
            {
                error(2, "Invalid Starting Port!");
                Thread.Sleep(700);
                Console.Clear();
                main();
            }
            else if (string.IsNullOrWhiteSpace(endPort))
            {
                error(2, "Invalid Ending Port!");
                Thread.Sleep(700);
                Console.Clear();
                main();
            }
            else if (string.IsNullOrWhiteSpace(waitTime))
            {
                error(2, "Invalid Delay!");
                Thread.Sleep(700);
                Console.Clear();
                main();
            }
            if (Convert.ToInt32(waitTime) < 40)
            {
                error(1, $"Delay under 50ms can skew results. Delay: {waitTime}ms! Continue?");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[Y]");
                Console.WriteLine("[N]");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("Choose: ");
                string option = Console.ReadLine();

                if (option == "2")
                {
                    Console.Clear();
                    main();
                }
            }


            openPorts = 0;
            closedPorts = 0;
            open.Clear();


            ping(IPAddress.Parse(host), Convert.ToInt32(startPort), Convert.ToInt32(endPort), Convert.ToInt32(waitTime));
        }

        static void error(int type, string message)
        {
            switch (type)
            {
                case 0:
                    // Information
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine($"NOTICE: {message}");
                    break;
                case 1:
                    // Warning
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine($"WARNING: {message}");
                    break;
                case 2:
                    // Error
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"ERROR: {message}");
                    break;
                case 3:
                    // Success
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"SUCCESS: {message}");
                    break;
            }
            Console.ForegroundColor = ConsoleColor.White;
            Thread.Sleep(500);
        }

        static void ping(IPAddress ip, int startPort, int endPort, int waitTime)
        {
            Console.WriteLine($"Scan Started! [ {DateTime.Now.ToString("h:mm:ss")} ]");

            for (int i = startPort; i < endPort + 1; i++)
            {
                using (TcpClient client = new TcpClient())
                {
                    if (client.ConnectAsync(ip, i).Wait(waitTime))
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("\r{0} is open  ", i);
                        openPorts += 1;
                        open.Add(i.ToString());
                        client.Dispose();
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("\r{0} is closed", i);
                        closedPorts += 1;
                        client.Dispose();
                    }
                }
            }


            Console.WriteLine();
            error(0, "Scan Complete! Refreshing Console");
            Thread.Sleep(1500);
            Console.Clear();
            main();
        }
    }
}

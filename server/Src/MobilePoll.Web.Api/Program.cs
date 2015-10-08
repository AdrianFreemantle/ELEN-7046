using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading;
using Microsoft.Owin.Hosting;
using MobilePoll.Infrastructure.Persistence.Mongo;
using MobilePoll.Web.Api.Wireup;

namespace MobilePoll.Web.Api
{
    class Program
    {
        private const string AddressFormat = "http://{0}:9000";
        private static readonly string BaseAddress;

        static Program()
        {
            MongoUnitOfWork.DropDatabaseOnStartup = false;
            BaseAddress = String.Format(AddressFormat, System.Environment.MachineName);
        }

        static void Main(string[] args)
        {
            var options = new StartOptions();

            if (!ConfigureAddressBindings(options))
                return;

            Console.WriteLine("Starting OWIN Server with {0} configuration...", Startup.DefaultConfiguration.GetType().Name);
            Console.WriteLine();

            using (WebApp.Start<Startup>(options))
            {
                PrintServerStartupMessage(options);

                Console.WriteLine("\nPress the X key to stop the server.");

                do
                {
                    ConsoleKeyInfo keyPress = Console.ReadKey();

                    if (keyPress.KeyChar == 'x' || keyPress.KeyChar == 'X')
                    {
                        Thread.SpinWait(10);
                        break;
                    }
                } while (true);
            }
        }

        private static bool ConfigureAddressBindings(StartOptions options)
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                Console.WriteLine("No available networks could be found.");
                return false;
            }

            options.Urls.Add("http://localhost:9000");
            options.Urls.Add("http://127.0.0.1:9000");
            options.Urls.Add(BaseAddress);

            foreach (NetworkInterface ni in GetNetworkInterfaces())
            {
                foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                {
                    if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork && ip.IsDnsEligible)
                    {
                        var address = String.Format(AddressFormat, ip.Address);
                        options.Urls.Add(address);
                    }
                }
            }

            return true;
        }

        private static IEnumerable<NetworkInterface> GetNetworkInterfaces()
        {
            return NetworkInterface.GetAllNetworkInterfaces().Where(n => n.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 || n.NetworkInterfaceType == NetworkInterfaceType.Ethernet);
        }

        private static void PrintServerStartupMessage(StartOptions options)
        {
            Console.WriteLine("\nServer Started. To test the connection go to {0}api/Test", BaseAddress);

            Console.WriteLine("OWIN Self Host bound to:");

            foreach (var url in options.Urls)
            {
                Console.WriteLine(url);
            }

            Console.WriteLine("\nAvailable API Calls:");
            Console.WriteLine("GET {0}/api/PollResult", BaseAddress);
            Console.WriteLine("GET {0}/api/PollResult/{{id}}", BaseAddress);
            Console.WriteLine("POST {0}/api/PollResult", BaseAddress);
            Console.WriteLine();
            Console.WriteLine("GET {0}/api/PollResult", BaseAddress);
            Console.WriteLine("GET {0}/api/PollResult/{{id}}", BaseAddress);
            Console.WriteLine("POST {0}/api/PollResult", BaseAddress);
            Console.WriteLine();
            Console.WriteLine("GET {0}/api/Report", BaseAddress);
        }
    }
}

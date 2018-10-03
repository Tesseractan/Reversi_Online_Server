using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Diagnostics;

namespace Reversi_Online_Server_1._1
{
    class Program
    {
        static void Main(string[] args)
        {
            DataServer dataServer = new DataServer(2018);
            dataServer.Start();

            GameServer gameServer = new GameServer(2019);
            gameServer.Start();

            Thread quering = new Thread(() =>
            {
                while (true)
                {
                    string request = Console.ReadLine();
                    if (request.StartsWith("PROF "))
                    {
                        string name = request.Split(' ')[1];
                        try
                        {
                            Process.Start($@"{Environment.GetFolderPath(Environment.SpecialFolder.CommonPictures)}\{name}.png");
                        }
                        catch
                        {
                            Console.WriteLine("The username doesn't exist :(");
                        }
                    }
                    else if (request.StartsWith("PROFS"))
                    {
                        Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.CommonPictures));
                    }
                    else
                    {
                        Console.WriteLine(">>> Unknown command");
                    }
                }
            });
            quering.Start();
        }
    }
}

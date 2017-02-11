using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace ConsoleApplication1
{
    class Program
    {
        
        static void Main(string[] args)
        {
            using (ServiceHost hote = new ServiceHost(typeof(serveur.Serveur)))
            {
                hote.Open();
                Console.WriteLine("Services démarrés");
                Console.ReadLine();
            }
        }
    }
}

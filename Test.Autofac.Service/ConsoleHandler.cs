using System;

namespace Test.Autofac.Service
{
    public class ConsoleHandler : Handler
    {
        public override void Handle(string message) {
            Console.WriteLine(message);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Test.Autofac.Service
{
    public class TraceHandler : Handler
    {
        public TraceHandler()
        {

        }

        public TraceHandler(string name)
        {

        }

        public TraceHandler(TextWriter writer)
        {

        }

        public TextWriter TextWriter { get; set; }

        public override void Handle(string message)
        {
            new StringWriter();
            Trace.WriteLine(message);
        }
    }
}

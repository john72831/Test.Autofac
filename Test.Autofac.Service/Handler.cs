namespace Test.Autofac.Service
{
    public abstract class Handler : IHandler
    {
        public abstract void Handle(string message);
    }
}
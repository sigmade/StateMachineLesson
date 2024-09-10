namespace ConsoleApp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var someClass = new SomeClass();

            var task = someClass.SomeAsync();
            var result = await task;

            Console.ReadLine();
        }
    }
}

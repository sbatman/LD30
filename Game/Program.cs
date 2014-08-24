namespace LD30
{
    public static class Program
    {
        static void Main()
        {
            using (var game = new Game())
                game.Run();
        }
    }
}

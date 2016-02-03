namespace SoftwareRenderer
{
    internal static class Program
    {
        private static void Main(string[] args) {
            using (var app = new AppDelegate()) {
                app.Run();
            }
        }
    }
}
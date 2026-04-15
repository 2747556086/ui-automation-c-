namespace CSharpAppiumPOM.Core;

public sealed class SimpleLogger
{
    private readonly string _name;

    public SimpleLogger(string name)
    {
        _name = name;
    }

    public void Info(string message) => Write("INFO", message);
    public void Debug(string message) => Write("DEBUG", message);
    public void Warn(string message) => Write("WARN", message);
    public void Error(string message) => Write("ERROR", message);

    private void Write(string level, string message)
    {
        var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        Console.WriteLine($"{timestamp} - {_name} | {level} - {message}");
    }
}

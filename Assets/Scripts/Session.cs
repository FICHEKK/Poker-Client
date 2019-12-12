using System.IO;
using System.Net.Sockets;

/// <summary>
/// Holds the important data between Unity scene changes.
/// </summary>
public static class Session {
    public static string Username { get; set; }
    public static int ChipCount { get; set; }
    public static int WinCount { get; set; }

    public static TcpClient Client { get; set; }
    public static StreamReader Reader { get; set; }
    public static StreamWriter Writer { get; set; }

    public static void Finish() {
        Reader.Close();
        Writer.Close();
        Client.Close();
        
        Username = null;
    }
}
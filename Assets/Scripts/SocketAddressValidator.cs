using System.Net;

/// <summary>
/// A simple socket address validator that offers methods for checking
/// the validity of the given IP address or port.
/// </summary>
public static class SocketAddressValidator {
    
    /// <summary>
    /// Checks if the given string is a valid IP address.
    /// </summary>
    /// <param name="address">The IP address.</param>
    /// <returns>True if the given string is a valid IP address.</returns>
    public static bool ValidateAddress(string address) {
        try {
            IPAddress.Parse(address);
            return true;
        }
        catch {
            return false;
        }
    }

    /// <summary>
    /// Checks if the given string is a valid port number.
    /// </summary>
    /// <param name="port">The port.</param>
    /// <returns>True if the given string is a valid port.</returns>
    public static bool ValidatePort(string port) {
        try {
            short.Parse(port);
            return true;
        }
        catch {
            return false;
        }
    }
}
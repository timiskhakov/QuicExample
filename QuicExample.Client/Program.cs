using System;
using System.Collections.Generic;
using System.Net.Quic;
using System.Net.Security;
using System.Net;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using System.Text;

namespace QuicExample.Client;

[SupportedOSPlatform("windows")]
internal static class Program
{
    private static async Task Main()
    {
        await using var connection = await QuicConnection.ConnectAsync(new QuicClientConnectionOptions
        {
            RemoteEndPoint = IPEndPoint.Parse("127.0.0.1:4242"),
            DefaultStreamErrorCode = 1,
            DefaultCloseErrorCode = 2,
            ClientAuthenticationOptions = new SslClientAuthenticationOptions
            {
                ApplicationProtocols = new List<SslApplicationProtocol>
                {
                    new SslApplicationProtocol("quic-example")
                },
                RemoteCertificateValidationCallback = (sender, certificate, chain, errors) => true
            }
        });

        await using var stream = await connection.OpenOutboundStreamAsync(QuicStreamType.Bidirectional);
        
        // Write
        await stream.WriteAsync(Encoding.UTF8.GetBytes("Ping"));

        // Read
        var buffer = new byte[4096];
        await stream.ReadAsync(buffer);
        Console.WriteLine(Encoding.UTF8.GetString(buffer, 0, buffer.Length));
    }
}
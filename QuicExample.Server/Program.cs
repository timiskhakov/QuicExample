using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Quic;
using System.Net.Security;
using System.Runtime.Versioning;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace QuicExample.Server;

[SupportedOSPlatform("windows")]
internal static class Program
{
    private static async Task Main()
    {
        await using var listener = await QuicListener.ListenAsync(new QuicListenerOptions
        {
            ListenEndPoint = IPEndPoint.Parse("127.0.0.1:4242"),
            ApplicationProtocols = new List<SslApplicationProtocol>
            {
                new SslApplicationProtocol("quic-example")
            },
            ConnectionOptionsCallback = (_, _, _) => ValueTask.FromResult(new QuicServerConnectionOptions
            {
                DefaultCloseErrorCode = 1,
                DefaultStreamErrorCode = 2,
                IdleTimeout = TimeSpan.FromSeconds(30),
                ServerAuthenticationOptions = new SslServerAuthenticationOptions
                {
                    ApplicationProtocols = new List<SslApplicationProtocol>
                    {
                        new SslApplicationProtocol("quic-example")
                    },
                    ServerCertificate = CreateCertificate(),
                    ClientCertificateRequired = false,
                    RemoteCertificateValidationCallback = (sender, certificate, chain, errors) => true,
                }
            }),
        });

        await using var connection = await listener.AcceptConnectionAsync();
        await using var stream = await connection.AcceptInboundStreamAsync();

        // Read
        var buffer = new byte[4096];
        await stream.ReadAsync(buffer);
        Console.WriteLine(Encoding.UTF8.GetString(buffer, 0, buffer.Length));

        // Write
        await stream.WriteAsync(Encoding.UTF8.GetBytes("Pong"));
    }

    private static X509Certificate CreateCertificate()
    {
        var filename = Path.Combine(Directory.GetCurrentDirectory(), "cert_key.p12");
        return X509Certificate.CreateFromCertFile(filename);
    }
}
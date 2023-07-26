# QuicExample

A minimal client-server setup demonstrating the usage of `MsQuic`.

## Requirements

- Windows 11 22H2 (tested on 22621.1992)
- OpenSSL (tested on 3.1.0 14)
- .NET 7 (tested on 7.0.9)

## Usage

1. Generate a certificate for the server:

```cmd
cd .\QuicExample.Server\
openssl genrsa 2048 > private.key
openssl req -new -x509 -nodes -sha1 -days 1000 -key private.key > public.cer
openssl pkcs12 -export -in public.cer -inkey private.key -out cert_key.p12
```

2. Run the server first:

```cmd
cd .\QuicExample.Server\
dotnet run
```

3. Run the server next:

```cmd
cd .\QuicExample.Client\
dotnet run
```

4. Expected results:
- The server prints `Ping`
- The client prints `Pong`
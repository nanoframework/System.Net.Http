[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=nanoframework_lib-nanoFramework.System.Net.Http&metric=alert_status)](https://sonarcloud.io/dashboard?id=nanoframework_lib-nanoFramework.System.Net.Http) [![Reliability Rating](https://sonarcloud.io/api/project_badges/measure?project=nanoframework_lib-nanoFramework.System.Net.Http&metric=reliability_rating)](https://sonarcloud.io/dashboard?id=nanoframework_lib-nanoFramework.System.Net.Http) [![License](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE) [![NuGet](https://img.shields.io/nuget/dt/nanoFramework.System.Net.Http.svg?label=NuGet&style=flat&logo=nuget)](https://www.nuget.org/packages/nanoFramework.System.Net.Http/) [![#yourfirstpr](https://img.shields.io/badge/first--timers--only-friendly-blue.svg)](https://github.com/nanoframework/Home/blob/main/CONTRIBUTING.md) [![Discord](https://img.shields.io/discord/478725473862549535.svg?logo=discord&logoColor=white&label=Discord&color=7289DA)](https://discord.gg/gCyBu8T)

![nanoFramework logo](https://raw.githubusercontent.com/nanoframework/Home/main/resources/logo/nanoFramework-repo-logo.png)

-----

### Welcome to the .NET **nanoFramework** System.Net.Http Library repository

## Build status

| Component | Build Status | NuGet Package |
|:-|---|---|
| System.Net.Http | [![Build Status](https://dev.azure.com/nanoframework/System.Net.Http/_apis/build/status/System.Net.Http?repoName=nanoframework%2FSystem.Net.Http&branchName=main)](https://dev.azure.com/nanoframework/System.Net.Http/_build/latest?definitionId=12&repoName=nanoframework%2FSystem.Net.Http&branchName=main) | [![NuGet](https://img.shields.io/nuget/v/nanoFramework.System.Net.Http.svg?label=NuGet&style=flat&logo=nuget)](https://www.nuget.org/packages/nanoFramework.System.Net.Http/) |
| System.Net.Http.Client | [![Build Status](https://dev.azure.com/nanoframework/System.Net.Http/_apis/build/status/System.Net.Http?repoName=nanoframework%2FSystem.Net.Http&branchName=main)](https://dev.azure.com/nanoframework/System.Net.Http/_build/latest?definitionId=12&repoName=nanoframework%2FSystem.Net.Http&branchName=main) | [![NuGet](https://img.shields.io/nuget/v/nanoFramework.System.Net.Http.Client.svg?label=NuGet&style=flat&logo=nuget)](https://www.nuget.org/packages/nanoFramework.System.Net.Http.Client/) |
| System.Net.Http.Server | [![Build Status](https://dev.azure.com/nanoframework/System.Net.Http/_apis/build/status/System.Net.Http?repoName=nanoframework%2FSystem.Net.Http&branchName=main)](https://dev.azure.com/nanoframework/System.Net.Http/_build/latest?definitionId=12&repoName=nanoframework%2FSystem.Net.Http&branchName=main) | [![NuGet](https://img.shields.io/nuget/v/nanoFramework.System.Net.Http.Server.svg?label=NuGet&style=flat&logo=nuget)](https://www.nuget.org/packages/nanoFramework.System.Net.Http.Server/) |

## Usage examples

The API, classes and namespaces in this library follow, as close as possible, the .NET ones.
Exceptions are the lack of async calls, generics and the Task, async/await model. The names reflect that by dropping the _Async_ suffix and not returning `Task` and the lack of the overloaded methods with `CancelationToken` parameters.

Also worth mentioning is the need to pass the CA root certificate to `HttpClient` in order to be able to validate the server certificate.

## HttpClient calling REST services

`HttpClient` makes it very easy to connect and consume REST services.
In order to use it, one has to create the object and them perform the calls. Note that `HttpClient` is meant to be reused throughout the application lifecycle. There is no need to create a new instance every time a call has to be performed. Like this:

```csharp
static readonly HttpClient _httpClient = new HttpClient();
```

To pass the CA root certificate allowing to validate the secure server certificate.
The CA root cert can also come from a binary file or text file from a resource.

```csharp
_httpClient.HttpsAuthentCert = new X509Certificate(
@"-----BEGIN CERTIFICATE-----
MIIEDzCCAvegAwIBAgIBADANBgkqhkiG9w0BAQUFADBoMQswCQYDVQQGEwJVUzEl
MCMGA1UEChMcU3RhcmZpZWxkIFRlY2hub2xvZ2llcywgSW5jLjEyMDAGA1UECxMp
U3RhcmZpZWxkIENsYXNzIDIgQ2VydGlmaWNhdGlvbiBBdXRob3JpdHkwHhcNMDQw
NjI5MTczOTE2WhcNMzQwNjI5MTczOTE2WjBoMQswCQYDVQQGEwJVUzElMCMGA1UE
ChMcU3RhcmZpZWxkIFRlY2hub2xvZ2llcywgSW5jLjEyMDAGA1UECxMpU3RhcmZp
ZWxkIENsYXNzIDIgQ2VydGlmaWNhdGlvbiBBdXRob3JpdHkwggEgMA0GCSqGSIb3
DQEBAQUAA4IBDQAwggEIAoIBAQC3Msj+6XGmBIWtDBFk385N78gDGIc/oav7PKaf
8MOh2tTYbitTkPskpD6E8J7oX+zlJ0T1KKY/e97gKvDIr1MvnsoFAZMej2YcOadN
+lq2cwQlZut3f+dZxkqZJRRU6ybH838Z1TBwj6+wRir/resp7defqgSHo9T5iaU0
X9tDkYI22WY8sbi5gv2cOj4QyDvvBmVmepsZGD3/cVE8MC5fvj13c7JdBmzDI1aa
K4UmkhynArPkPw2vCHmCuDY96pzTNbO8acr1zJ3o/WSNF4Azbl5KXZnJHoe0nRrA
1W4TNSNe35tfPe/W93bC6j67eA0cQmdrBNj41tpvi/JEoAGrAgEDo4HFMIHCMB0G
A1UdDgQWBBS/X7fRzt0fhvRbVazc1xDCDqmI5zCBkgYDVR0jBIGKMIGHgBS/X7fR
zt0fhvRbVazc1xDCDqmI56FspGowaDELMAkGA1UEBhMCVVMxJTAjBgNVBAoTHFN0
YXJmaWVsZCBUZWNobm9sb2dpZXMsIEluYy4xMjAwBgNVBAsTKVN0YXJmaWVsZCBD
bGFzcyAyIENlcnRpZmljYXRpb24gQXV0aG9yaXR5ggEAMAwGA1UdEwQFMAMBAf8w
DQYJKoZIhvcNAQEFBQADggEBAAWdP4id0ckaVaGsafPzWdqbAYcaT1epoXkJKtv3
L7IezMdeatiDh6GX70k1PncGQVhiv45YuApnP+yz3SFmH8lU+nLMPUxA2IGvd56D
eruix/U0F47ZEUD0/CwqTRV/p2JdLiXTAAsgGh1o+Re49L2L7ShZ3U0WixeDyLJl
xy16paq8U4Zt3VekyvggQQto8PT7dL5WXXp59fkdheMtlb71cZBDzI0fmgAKhynp
VSJYACPq4xJDKVtHCN2MQWplBqjlIapBtJUhlbl90TSrE9atvNziPTnNvT51cKEY
WQPJIrSPnNVeKtelttQKbfi3QBFGmh95DmK/D5fs4C8fF5Q=
-----END CERTIFICATE-----");
```

It's possible to add HTTP headers that will be sent along with each request.

```csharp
_httpClient.DefaultRequestHeaders.Add("x-ms-blob-type", "BlockBlob");
```

### Perform a HTTP GET request

Here's a example of a HTTP request to read some content as a string:

```csharp
HttpResponseMessage response = _httpClient.Get("https://httpbin.org/get");
response.EnsureSuccessStatusCode();
var responseBody = response.Content.ReadAsString();
```

The above call would return something similar to the following, which can be output in Visual Studio by calling `Debug.WriteLine(responseBody)`:

```console
{
  "args": {}, 
  "headers": {
    "Host": "httpbin.org", 
    "X-Amzn-Trace-Id": "Root=1-6214aad3-38e5f8357bdf90530300eb5f", 
    "X-Ms-Blob-Type": "BlockBlob"
  }, 
  "origin": "5.249.47.208", 
  "url": "https://httpbin.org/get"
}
```

Note the call to `response.EnsureSuccessStatusCode();`. This will throw an `HttpRequestException` in case the status code from the HTTP request is not a successful one.

### Perform a HTTP POST request

Follows an example of a HTTP request performing POST request to send some json content to an endpoint.

```csharp
var content = new StringContent("{\"someProperty\":\"someValue\"}", Encoding.UTF8, "application/json");
var result = _httpClient.Post("https://httpbin.org/anything", content);
result.EnsureSuccessStatusCode();
```

Worth noting that the json content above it's presented as a simple string to simplify the code. There is a [json library](https://github.com/nanoframework/nanoFramework.Json) available to help with serializing and deserializing from/to C# classes, even the most complex ones.

Again, note the call to `response.EnsureSuccessStatusCode();` to make sure the HTTP request was successfully performed.

### Download binary content to a file

Using `HttpClient` makes it easy to deal with binary content too. Here's an example of how to download a file from a webserver.

```csharp
HttpResponseMessage response = _httpClient.Get("https://storage-on-the-cloud.net/file-with-binary-content");
response.EnsureSuccessStatusCode();

using FileStream fs = new FileStream($"I:\\i-am-a-binary-file.bin", FileMode.Create, FileAccess.Write);
response.Content.ReadAsStream().CopyTo(fs);
```

### Debugging through a reverse proxy

When code is deployed to a MCU it might be desirable to let the device connect to your development machine running IIS Express.
This can be achieved with a proxy such as [this one](https://www.npmjs.com/package/iisexpress-proxy).
Be aware that this leads to SocketExceptions with the current version of **nanoFramework** System.Net.Http when sending consecutive
requests to your development machine. A simple retry mechanism in Debug mode will get around this.

## Feedback and documentation

For documentation, providing feedback, issues and finding out how to contribute please refer to the [Home repo](https://github.com/nanoframework/Home).

Join our Discord community [here](https://discord.gg/gCyBu8T).

## Credits

The list of contributors to this project can be found at [CONTRIBUTORS](https://github.com/nanoframework/Home/blob/main/CONTRIBUTORS.md).

## License

The **nanoFramework** Class Libraries are licensed under the [MIT license](LICENSE.md).

## Code of Conduct

This project has adopted the code of conduct defined by the Contributor Covenant to clarify expected behaviour in our community.
For more information see the [.NET Foundation Code of Conduct](https://dotnetfoundation.org/code-of-conduct).

### .NET Foundation

This project is supported by the [.NET Foundation](https://dotnetfoundation.org).

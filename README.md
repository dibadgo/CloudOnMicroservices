# InCloud Sample Hypervisor

![](https://img.shields.io/badge/version-0.8a-yellow)
![](https://img.shields.io/badge/envoy-1.14.1-blue)
![](https://img.shields.io/badge/ASP.NET%20Core-3.0-blue)
![](https://img.shields.io/badge/Docker-3-3AA0EE)
![](https://img.shields.io/badge/MSSQLServer-2019-yellow)
![](https://img.shields.io/badge/redis-6.0-red)

Sample .NET Core project, based on simplified microservices architecture.


## Description

This is a simulator created exclusively for demonstration purposes, created by me in my free time.
The idea of this application is to **demonstrate skills in microservice architecture**. That is why there are so many technologies.
This is simple cloud simulator application.
It is very similar to AWS, Azure, GCP.

## Technologies

* [Microservices architecture](https://en.wikipedia.org/wiki/Microservices)
* [.NET Core 3](https://docs.microsoft.com/en-us/dotnet/core/whats-new/dotnet-core-3-0)
* [REST API](https://en.wikipedia.org/wiki/Representational_state_transfer)
* [gRPC](https://grpc.io/) services
* [MS SQL Server](https://www.microsoft.com/en-us/sql-server/)
* [Redis](https://redis.io/)
* [Docker](https://www.docker.com/)
* [Docker-compose](https://docs.docker.com/compose/)
* [Envoy reverse proxy](https://www.envoyproxy.io/)
* [Swagger](https://swagger.io/) REST API documentation
* [IdentityServer4](https://identityserver4.readthedocs.io/en/latest/)

## Infrastructure

![InCloud infrastructure](https://drive.google.com/uc?id=1dyV0n6VFVvYECAT9xplPeVDyWAZ6Ormf)

## Dependencies
You'll need any linux machine with the docker daemon and docker-compose tools


## API Gateway

The goal is to separate the backend's client from the application architecture.
The client makes one request, while the gateway can execute several nested requests for microservices in one session.
So, the gateway is the main entry point to the application.
There can be many gateways. Usually create a gateway for each type of client, such as Android/iOS mobile devices or Web.

The cool explanations of ApiGateway can be found [here](https://microservices.io/patterns/apigateway.html)

### Std Gateway

At the moment, the REST API includes a couple of namespaces:

**/volumes** to manage of volumes (disks) from instance 

**/instances** to manage of VMs instance

The user should use the HTTP(s) protocol to make a request to the API Gateway.

I do not want to focus on the documentation. All documentation is generated automatically by Swagger.
After deploy you can try to follow by link https://std-gateway/swagger and enjoy the documentation.

![Swagger](https://drive.google.com/uc?id=1RxRDH1jWsqJLlYfKMXANn1l9wfKr5cdi)

## Authentication

There is too many ways to protect the API against unauthorized actions.

We can implement:

* oAuth 2.0 [RFC 6749](https://tools.ietf.org/html/rfc6749)
* Basic auth [RFC 7617](https://tools.ietf.org/html/rfc7617) 
* OpenId connect 2.0 [RFC 6750](https://www.ietf.org/rfc/rfc6750.txt)
* Sessions [RFC 6265](https://tools.ietf.org/html/rfc6265)

We can do it yourself, or you can use the popular framework. 

Since the purpose of this application is to create the infrastructure, authentication methods do not currently matter.
Well, in this case we'll use the **IdentityServer4** framework and separate it in the microservice named **Identity.API**.

The simplest and most rational way is to use the authentication method using **oAuth 2.0** (Bearer tokens).

Before calling an any ApiGateway, the user must obtain a Bearer token from the Identity.API.

**Example:**

    curl --location --request POST 'https://identity-api:443/connect/token' \
    --header 'Accept: application/json' \
    --header 'Content-Type: application/x-www-form-urlencoded' \
    --header 'Content-Type: application/x-www-form-urlencoded' \
    --data-urlencode 'grant_type=password' \
    --data-urlencode 'username=user@example.com' \
    --data-urlencode 'password=P@$$w0rd3z' \
    --data-urlencode 'scope=gateway volumes instances' \
    --data-urlencode 'client_id=client' \
    --data-urlencode 'client_secret=secret'

In the response we can find out the `access_token` and pin it to the all request to ApiGateway like a `Authorization` header.

## gRPC
  
gRPC is a modern open source high performance RPC framework that can run in any environment.
So, gRPC is easy to use, great for creating distributed systems (microservices) and APIs
This framework uses a protocol [Protobuf](https://developers.google.com/protocol-buffers/)

Protobuf is the default serialization format for transferring data between client and server.

Follow is an example of a volumes.proto file:

    service Volume {

      rpc Create (CreateVolumeRequest) returns (VolumeReply);
      rpc List (Empty) returns (stream VolumeReply);
      rpc Get (GetVolume) returns (VolumeReply);
     }

    message CreateVolumeRequest {
      string name = 1;
      int32 sizeGb = 2;
      string osType = 3;
      repeated string mountPints = 4;
    }

## Deploy

All infrastructure in the project is based on Docker.

To quickly expand all containers. you need to use the docker-compose framework.
Just run `docker-compose up -d` to start the deployment process.

However, this is sometimes not enough. In some cases we could observe the strange bechavour depended on OS version.

Known issues are:
* Volume folders wasn't created
* Volumes folders haven't required permissions
* iptables/firewall issues
* Nat rules issues

Well, I've created the shell script for this purpuses. The script `deploy_linux.sh` will create all required folders and will apply all required permissions.

Just run `sudo deploy_linux.sh` and't that's will do all proccess automatically.

## Problems

Unfortunately, the problems with the firewall and nat rules are not systematic, and we will need to resolve the problems face to face.

## Envoy

Proxies are invisible workers that help keep networks safe, optimize bandwidth, offload processing from backend servers, and ensure the smooth flow of requests. 
An outbound proxy provides both security isolation and performance optimization for network traffic.
Internal clients protected by a firewall do not have direct access out to the Internet. 
Instead, clients send all requests to a proxy that does have external access that then makes the requests on behalf of the original client

I made this design choice because of Envoy's built-in support for the WebSocket protocol, required by the new gRPC inter-service communications implemented in eShopOnContainers. 

There is a good [resource](https://dzone.com/articles/why-proxies-are-important-for-microservices) describes all benefit of a reverse proxy

## UnitTests coverage

    | Module                 | Coverage | isDone? |  
    |------------------------|----------|---------|
    | Identity.Api           |     0%   |         |
    | Disks.gRPC.Service     |     75%   |         |
    | Instances.gRPC.Service |     0%   |         |
    | StdGateway             |     0%   |         |


**Created by Artyom Arabadzhiyan**

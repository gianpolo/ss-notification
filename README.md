# Microservices Solution

This repository now contains three .NET 9 Web API microservices. `Notification.Api` uses Dapr and SignalR to broadcast messages to connected clients. `Orders.Api` and the new `Basket.Api` expose simple Dapr endpoints without SignalR. All projects are included in the `microservices.sln` solution file.

Open `microservices.sln` with Visual Studio 2022 or later to work with the services.

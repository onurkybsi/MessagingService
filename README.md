# MessagingService

MessagingService is a web application that provides real-time messaging using [SignalR](https://docs.microsoft.com/en-us/aspnet/core/signalr/introduction?view=aspnetcore-5.0), as well as it keeps the logs of the system with [ELK](https://www.elastic.co/what-is/elk-stack) and the data in [MongoDB](https://docs.mongodb.com/manual/).

## Installation

Use the [docker-compose](https://docs.docker.com/compose/) to install MessagingService.

```bash
docker-compose up
```

## Usage

While the client can use MessageHub for real-time messaging, it can use REST API to get various information such as the current status of the MessageHub and do authorization jobs required to access MessageHub.

### REST API

The REST API usage is described below.

#### Sign Up in the MessagingService

#### Request

`POST api/auth/signup`

    curl -v -d '{"Username":"testuser", "Password":"1234"}' -H "Content-Type: application/json" -X POST http://localhost:8080/api/auth/signup

#### Response

    HTTP/1.1 200 OK
    Date: Thu, 14 Jan 2021 18:09:14 GMT
    Content-Length: 0

## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

Please make sure to update tests as appropriate.

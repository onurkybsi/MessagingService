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

#### Sign up in the MessagingService

#### Request

`POST api/auth/signup`

    curl -v -d '{"Username":"testuser", "Password":"1234"}' -H "Content-Type: application/json" -X POST http://localhost:8080/api/auth/signup

#### Response

    HTTP/1.1 200 OK
    Date: Thu, 14 Jan 2021 18:09:14 GMT
    Content-Length: 0
    
#### Log in to the MessagingService

#### Request

`POST api/auth/login`

    curl -v -d '{"Username":"onurkayabasi", "Password":"1234"}' -H "Content-Type: application/json" -X POST http://localhost:8080/api/auth/login

#### Response

    HTTP/1.1 200 OK
    Date: Thu, 14 Jan 2021 18:17:26 GMT
    Content-Type: application/json; charset=utf-8
    Content-Length: 369

    {"isAuthenticated":true,"message":null,"token":"eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJJZCI6IjYwMDA4ODdhYzMyMzZkNDI2NjYyMzQwYiIsIm5hbWVpZCI6Im9udXJrYXlhYmFzaSIsInJvbGUiOiJBZG1pbiIsIm5iZiI6MTYxMDY0ODI0NiwiZXhwIjoxNjEwNjUwMDQ2LCJpYXQiOjE2MTA2NDgyNDYsImlzcyI6Imh0dHA6Ly9sb2NhbGhvc3Q6NTAwMCIsImF1ZCI6Ind3dy5iaWxtZW1uZS5jb20ifQ.hJQ_CfXB8hKMxz3gDOsXAy70djVft22Q-kvBwPZLkUo"}
   
 #### Log in to the MessagingService

 #### Request

`POST api/auth/login`

    curl -v -d '{"Username":"adminuser", "Password":"1234"}' -H "Content-Type: application/json" -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJJZCI6IjYwMDA4ODdhYzMyMzZkNDI2NjYyMzQwYiIsIm5hbWVpZCI6Im9udXJrYXlhYmFzaSIsInJvbGUiOiJBZG1pbiIsIm5iZiI6MTYxMDY0ODI0NiwiZXhwIjoxNjEwNjUwMDQ2LCJpYXQiOjE2MTA2NDgyNDYsImlzcyI6Imh0dHA6Ly9sb2NhbGhvc3Q6NTAwMCIsImF1ZCI6Ind3dy5iaWxtZW1uZS5jb20ifQ.hJQ_CfXB8hKMxz3gDOsXAy70djVft22Q-kvBwPZLkUo" -X POST http://localhost:8080/api/auth/signupasadmin
    
#### Response

    HTTP/1.1 200 OK
    Date: Thu, 14 Jan 2021 18:25:26 GMT
    Content-Length: 0
    

## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

Please make sure to update tests as appropriate.

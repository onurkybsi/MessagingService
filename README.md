# MessagingService

MessagingService is a web application that provides real-time messaging using [SignalR](https://docs.microsoft.com/en-us/aspnet/core/signalr/introduction?view=aspnetcore-5.0), as well as it keeps the logs of the system with [ELK](https://www.elastic.co/what-is/elk-stack) and the data in [MongoDB](https://docs.mongodb.com/manual/).

## Installation

Use the [docker-compose](https://docs.docker.com/compose/) to install MessagingService.

```bash
docker-compose up
```

## Usage

While the client can use MessageHub for real-time messaging, it can use REST API to get various information such as the current status of the MessageHub and do authorization jobs required to access MessageHub.

### MessageHub

[Hubs](https://docs.microsoft.com/en-us/aspnet/core/signalr/hubs?view=aspnetcore-5.0) in SingalR are structures that enable remote calls. MessageHub has two remote calling methods; SendPrivateMessage, SendMessageToAllUser (for admin users only). This method calls the ReceiveMessage method of subscribers connected to the hub at the end of the calls.

#### Example connection to MessageHub

```javascript
<script src="https://cdnjs.cloudflare.com/ajax/libs/microsoft-signalr/3.1.7/signalr.min.js"></script>

<script>
      const connection = new signalR.HubConnectionBuilder()
        .withUrl("http://messagingservicehost:8080/messagehub", {
          accessTokenFactory: () =>
            localStorage.getItem("access_token") != null
              ? localStorage.getItem("access_token")
              : "",
        })
        .build();

      connection.on("ReceiveMessage", (message) => {
        console.log("Message received !: ", message.content);
      });

      async function start() {
        try {
          await connection.start();
          console.log("SignalR Connected.");
        } catch (err) {
          console.log(err);
          setTimeout(start, 5000);
        }
      }
      connection.onclose(start);

      start();
    </script>
    
    <script>
      connection
        .invoke("SendPrivateMessage", {
          Message: "Hi !",
          ReceiverUser: "onurkayabasi",
        })
        .then((res) => {
          console.log(res);
        })
        .catch((err) => {
          console.log(err);
        });
    </script>
```

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
   
 #### Sign up as a admin in the MessagingService (for admin role)

 #### Request

`POST api/auth/signupasadmin`

    curl -v -d '{"Username":"adminuser", "Password":"1234"}' -H "Content-Type: application/json" -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJJZCI6IjYwMDA4ODdhYzMyMzZkNDI2NjYyMzQwYiIsIm5hbWVpZCI6Im9udXJrYXlhYmFzaSIsInJvbGUiOiJBZG1pbiIsIm5iZiI6MTYxMDY0ODI0NiwiZXhwIjoxNjEwNjUwMDQ2LCJpYXQiOjE2MTA2NDgyNDYsImlzcyI6Imh0dHA6Ly9sb2NhbGhvc3Q6NTAwMCIsImF1ZCI6Ind3dy5iaWxtZW1uZS5jb20ifQ.hJQ_CfXB8hKMxz3gDOsXAy70djVft22Q-kvBwPZLkUo" -X POST http://localhost:8080/api/auth/signupasadmin
    
#### Response

    HTTP/1.1 200 OK
    Date: Thu, 14 Jan 2021 18:25:26 GMT
    Content-Length: 0
    
 #### Get message history between users from the MessagingService 

 #### Request

`POST api/messagehubinfo/getmessagehistory` 

    curl -v -H "Content-Type: application/json" -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJJZCI6IjYwMDBiNWRlMDYzMGRjMjQxNmYxMjJiYSIsIm5hbWVpZCI6Im9udXJrYXlhYmFzaSIsInJvbGUiOiJBZG1pbiIsIm5iZiI6MTYxMDY1OTQxMywiZXhwIjoxNjEwNjYxMjEzLCJpYXQiOjE2MTA2NTk0MTMsImlzcyI6Imh0dHA6Ly9sb2NhbGhvc3Q6NTAwMCIsImF1ZCI6Ind3dy5iaWxtZW1uZS5jb20ifQ._9Ew1M5X0QPN0G4QYMkKWJZkEo-rOPpHRfhSRJ2CaR8" -X GET http://localhost:8080/api/messagehubinfo/GetMessageHistory?userName=testuser
    
#### Response

    HTTP/1.1 200 OK
    Date: Thu, 14 Jan 2021 21:30:55 GMT
    Content-Type: application/json; charset=utf-8
    Content-Length: 570
    
    [{"content":"Selam Onur, nasılsın ?\n","senderUsername":"testuser","receiverUsername":"onurkayabasi","timeToSend":"2021-01-14T21:30:55.719866+00:00","id":"6000b6410630dc2416f122bc"},{"content":"Selam, iyiyim teşekkürler sen nasılsın ?\n","senderUsername":"onurkayabasi","receiverUsername":"testuser","timeToSend":"2021-01-14T21:30:55.7199475+00:00","id":"6000b6670630dc2416f122bd"},{"content":"Selam nasıl gidiyor ?\n","senderUsername":"testuser","receiverUsername":"onurkayabasi","timeToSend":"2021-01-14T21:30:55.7200241+00:00","id":"6000b7560630dc2416f122be"}]
    
 #### Get connected usernames (for admin role)

 #### Request

`POST api/messagehubinfo/getconnectedusernames`

    curl -v -H "Content-Type: application/json" -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJJZCI6IjYwMDBiNWRlMDYzMGRjMjQxNmYxMjJiYSIsIm5hbWVpZCI6Im9udXJrYXlhYmFzaSIsInJvbGUiOiJBZG1pbiIsIm5iZiI6MTYxMDY1OTQxMywiZXhwIjoxNjEwNjYxMjEzLCJpYXQiOjE2MTA2NTk0MTMsImlzcyI6Imh0dHA6Ly9sb2NhbGhvc3Q6NTAwMCIsImF1ZCI6Ind3dy5iaWxtZW1uZS5jb20ifQ._9Ew1M5X0QPN0G4QYMkKWJZkEo-rOPpHRfhSRJ2CaR8" -X GET http://localhost:8080/api/messagehubinfo/GetConnectedUsernames
    
#### Response

    HTTP/1.1 200 OK
    Date: Thu, 14 Jan 2021 21:34:03 GMT
    Content-Type: application/json; charset=utf-8
    Content-Length: 27

    ["testuser","onurkayabasi"]
    
 #### Block another user

 #### Request

`POST api/messagehubinfo/blockuser`

    curl -v -d '{"BlockedUsername":"testuser"}' -H "Content-Type: application/json" -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJJZCI6IjYwMDBiNWRlMDYzMGRjMjQxNmYxMjJiYSIsIm5hbWVpZCI6Im9udXJrYXlhYmFzaSIsInJvbGUiOiJBZG1pbiIsIm5iZiI6MTYxMDY1OTQxMywiZXhwIjoxNjEwNjYxMjEzLCJpYXQiOjE2MTA2NTk0MTMsImlzcyI6Imh0dHA6Ly9sb2NhbGhvc3Q6NTAwMCIsImF1ZCI6Ind3dy5iaWxtZW1uZS5jb20ifQ._9Ew1M5X0QPN0G4QYMkKWJZkEo-rOPpHRfhSRJ2CaR8" -X POST http://localhost:8080/api/messagehubinfo/GetConnectedUsernames
    
#### Response

    HTTP/1.1 200 OK
    Date: Thu, 14 Jan 2021 21:40:25 GMT
    Content-Length: 0


## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

Please make sure to update tests as appropriate.

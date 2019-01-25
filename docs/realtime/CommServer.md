# CommServer
The comm server is the chat/lobby server which is implemented under 
[UberStrok.Realtime.Server.Comm](../src/UberStrok.Realtime.Server.Comm) in the
UberStrok project.

## Events
---
Events sent by the server to the client.

### HeartbeatChallenge
Used to request the client to send back a 
[SendHeartbeatResponse](#SendHeartbeatResponse). This was used as an anti-cheat
measure to check if any foreign assembly was loaded at runtime.

### LoadData
TODO: Document

### LobbyEntered
Notify the client that it has joined the lobby.

### DisconnectAndDisablePhoton
Disconnects the client and displays a message. This was used to kick the client
if it was detected to be cheating.

## Operations
---
Operations sent by the client to the server.

### AuthenticationRequest
Requests the comm server to identify the client using its AuthToken.

### SendHeartbeatResponse
Reponse when the server challenges the client with 
[HeartbeatChallenge](#HeartbeatChallenge).

## Sequence
---
The sequence of operations and events between the client and the server when
the client connects is as follows.

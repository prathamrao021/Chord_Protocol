# Chord_Protocol
This project employs Akka.NET and F# to create a robust, fault-tolerant distributed hash table (DHT) system. It supports dynamic node joining, finger table maintenance, and efficient key lookup, ensuring seamless decentralized data storage and retrieval in a scalable network environment.
### How to Run code:
Move to the project directory and run the following command:
```bash
dotnet run <numNodes> <numRequests>
```
### What is working?
*Both scalable and simple key lookups follow logarithmic trends and linear trends, respectively. 
*The starting node is produced statically. 
*Based on consistent hashing, the remaining nodes are dynamically generated and inserted into the chord ring. 
*Finger tables and the scalable lookup technique described in the article are used to find the successor of both the node and the key. 
*The average hop count is determined once each node has fulfilled a predetermined number of requests. 

### Table for Average Hop Count and Number of Nodes:
Attempt | #5 | #10 | #20 | #30 | #40 | #50 | #100 | #200 | #300 | #400 | #500 | #5 | #10 | #20 | #30 | #40 | #50 | #100 | #200 | #300 | #400 | #500
--- | --- | --- | --- |--- |--- |--- |--- |--- |--- |--- |--- |--- |--- | --- | --- | --- |--- |--- |--- |--- |--- |--- |--- |--- |--- |---
Seconds | 301 | 283 | 290 | 286 | 289 | 285 | 287 | 287 | 272 | 276 | 269 | 301 | 283 | 290 | 286 | 289 | 285 | 287 | 287 | 272 | 276 | 269

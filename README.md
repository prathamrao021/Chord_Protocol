# Chord_Protocol
This project employs Akka.NET and F# to create a robust, fault-tolerant distributed hash table (DHT) system. It supports dynamic node joining, finger table maintenance, and efficient key lookup, ensuring seamless decentralized data storage and retrieval in a scalable network environment.
### How to Run code:
Move to the project directory and run the following command:
```bash
dotnet run <numNodes> <numRequests>
```
### What is working?
* Both scalable and simple key lookups follow logarithmic trends and linear trends, respectively. 
*The starting node is produced statically. 
*Based on consistent hashing, the remaining nodes are dynamically generated and inserted into the chord ring. 
*Finger tables and the scalable lookup technique described in the article are used to find the successor of both the node and the key. 
*The average hop count is determined once each node has fulfilled a predetermined number of requests. 

### Table for Average Hop Count and Number of Nodes:
Number of Nodes | #5 | #10 | #20 | #30 | #40 | #50 | #100 | #200 | #300 | #400 | #500 | #1000 | #1500 | #2000 | #2500 | #3000 | #3500 | #4000 | #4500 | #5000 | #10000
--- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | ---
Average Hop Count | 1.36 | 2.04 | 2.64 | 3.22 | 3.33 | 4.24 | 4.88 | 6.25 | 7.00 | 7.95 | 8.54 | 9.21 | 10.76 | 10.82 | 11.98 | 12.65 | 13.68 | 13.47 | 13.65 | 14.05 | 15.43 

### Assumptions Made:
*We are initializing each finger table entry with a unique value for the first node. 
*We are initializing the whole finger table for every other node using the successor discovered in the Join() stage. 
*Every time a node joins, schedulers for FixFinger() and Stabilize() are generated. In our experiment, both schedulers are linked and operating at the same time period of 500 ms. 
*The document does not discuss the situation where successor and n are equal in the FindSuccessor() function. When the second node tries to join the chord ring, this occurs. If yes, we are giving back the successor(Or n) value in its entirety. As a result, the finger tables are updated in accordance with the second node's succession of the first node. 
*We are sending the node identifier as its successor when the key and node identification are the same. 

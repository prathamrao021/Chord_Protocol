open System
open Akka.FSharp
open System.Threading

type ConvergenceMessage =
    | NodeConverged of int * int * int

let convergenceCounter numNodes (mailbox: Actor<_>) = 
    let mutable totalHops = 0
    let mutable totalRequests = 0
    let mutable convergedNodeCount = 0

    let rec processMessages () = actor {
        let! message = mailbox.Receive ()

        match message with
        | NodeConverged (nodeID, hops, requests) ->
            printfn "NodeID: %d converged with hops: %d, requests: %d" nodeID hops requests
            totalHops <- totalHops + hops
            totalRequests <- totalRequests + requests
            convergedNodeCount <- convergedNodeCount + 1

            if(convergedNodeCount = numNodes) then
                printfn "Total number of hops: %d" totalHops
                printfn "Total number of requests: %d" totalRequests
                printfn "Average number of hops: %f" ((float totalHops) / (float totalRequests))
                mailbox.Context.System.Terminate() |> ignore

        return! processMessages ()
    }

    processMessages ()

type NetworkMessage =
    | Initialize
    | JoinNetwork of int
    | FindSuccessor of int
    | SuccessorReceived of int
    | Stabilize
    | FindPredecessor
    | PredecessorReceived of int
    | NotifyPredecessor of int
    | FixFingers
    | FindFingerSuccessor of int * int * int
    | UpdateFinger of int * int   
    | StartQuerying
    | QueryMessage
    | FindKeySuccessor of int * int * int
    | KeyFound of int

let constructActorPath actorID =
    let path = sprintf @"akka://my-system/user/%d" actorID
    path

let nodeBehavior (nodeID: int) m maxRequests convergenceCounterRef (mailbox: Actor<_>) =
    printfn "[INFO] Creating node %d" nodeID
    let hashSpace = int (Math.Pow(2.0, float m))
    let mutable predecessorID = -1
    let mutable fingerTable = Array.create m -1
    let mutable next = 0
    let mutable totalHops = 0
    let mutable numRequests = 0

    let rec nodeLoop () = actor {
        let! message = mailbox.Receive ()
        let sender = mailbox.Sender ()
        match message with
        | Initialize ->
            predecessorID <- -1
            for i = 0 to m - 1 do
                fingerTable.[i] <- nodeID
            mailbox.Context.System.Scheduler.ScheduleTellRepeatedly (
                TimeSpan.FromMilliseconds(0.0),
                TimeSpan.FromMilliseconds(500.0),
                mailbox.Self,
                Stabilize
            )
            mailbox.Context.System.Scheduler.ScheduleTellRepeatedly (
                    TimeSpan.FromMilliseconds(0.0),
                    TimeSpan.FromMilliseconds(500.0),
                    mailbox.Self,
                    FixFingers
                )

        | JoinNetwork (nDash) ->
            predecessorID <- -1
            let nDashPath = constructActorPath nDash
            let nDashRef = mailbox.Context.ActorSelection nDashPath
            nDashRef <! FindSuccessor (nodeID)

        // ... (rest of the code)

        return! nodeLoop ()
    }
    nodeLoop ()

[<EntryPoint>]
let main argv =
    let numNodes = int argv.[0]
    let numRequests = int argv.[1]
    let m = 20

    let system = System.create "my-system" (Configuration.load())

    let convergenceCounterRef = spawn system "convergenceCounter" (convergenceCounter numNodes)

    let nodeRefs =
        Array.init numNodes (fun _ ->
            let nodeID = (new Random()).Next(int (Math.Pow(2.0, float m)))
            let node = spawn system (constructActorPath nodeID) (nodeBehavior nodeID m numRequests convergenceCounterRef)
            if nodeID = 0 then node <! Initialize else node <! JoinNetwork 0
            Thread.Sleep(500)
            node)

    Thread.Sleep(30000) // Wait for stabilization

    Array.iter (fun nodeRef -> nodeRef <! StartQuerying; Thread.Sleep(500)) nodeRefs

    system.WhenTerminated.Wait()

    0
open System.IO

open System.Collections.Concurrent
open System.Net

open Suave
open Suave.Operators

open Shared
open Suave.RequestErrors

let path = Path.Combine("..","Client") |> Path.GetFullPath 
let port = 8085us

let config =
  { defaultConfig with 
      homeFolder = Some path
      bindings = [ HttpBinding.create HTTP (IPAddress.Parse "0.0.0.0") port ] }

let votes = new ConcurrentBag<_>()

let countResults () =
  let vs = votes |> Seq.toArray

  let overalls = 
    Array.countBy (fun v -> v.Overall) vs 
    |> Map.ofArray

  let favs = 
    vs
    |> Array.collect (fun v -> Array.ofSeq v.Favs)
    |> Array.countBy id
    |> Map.ofArray

  let comments =
    vs
    |> Array.choose (fun v -> v.Comment)

  { Overalls  = overalls
    FavsCount = favs
    Comments  = comments }

let sendVote (vote : Vote) : Async<VotingResults> = 
  async {
    do votes.Add vote
    do! Async.Sleep 1500
    return countResults ()
  }

let init : WebPart = 
  let api = 
    { sendVote = sendVote }
  let routeBuilder typeName methodName = 
    sprintf "/api/%s/%s" typeName methodName
  Fable.Remoting.Suave.FableSuaveAdapter.webPartWithBuilderFor api routeBuilder

let webPart =
  choose [
    init
    Files.browseHome
    NOT_FOUND "woops"
  ]

startWebServer config webPart
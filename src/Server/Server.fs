﻿open System.IO
open System.Net
open System.Collections.Concurrent

open Suave
open Suave.Operators

open Fable.Remoting.Suave

open Shared
open System.Collections.Concurrent

let clientPath = Path.Combine("..","Client") |> Path.GetFullPath 
let port = 8085us

let config =
  { defaultConfig with 
      homeFolder = Some clientPath
      bindings = [ HttpBinding.create HTTP (IPAddress.Parse "0.0.0.0") port ] }

let votes = ConcurrentBag<Vote>()

let countVotes() : VotingResults =
  let votes = Seq.toArray votes

  let scores =
    votes
    |> Array.map (fun vote -> vote.Score)
    |> Array.countBy id
    |> Map.ofArray

  let comments =
    votes
    |> Array.map (fun vote -> vote.Comment)
    |> Array.filter ((<>) "")

  { Comments = comments 
    Scores   = scores }

let vote (v : Vote) : Async<VotingResults> = 
  async {
    do votes.Add v
    do! Async.Sleep 1000
    return countVotes()
  }

let voting : WebPart =
  let protocol =
    { vote = vote }
  FableSuaveAdapter.webPartWithBuilderFor protocol Route.builder

let webPart =
  choose [
    voting
    Filters.path "/" >=> Files.browseFileHome "index.html"
    Files.browseHome
    RequestErrors.NOT_FOUND "Not found!"
  ]

startWebServer config webPart
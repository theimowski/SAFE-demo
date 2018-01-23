open System.IO
open System.Net

open Suave
open Suave.Operators

open Fable.Remoting.Suave

open Shared

let clientPath = Path.Combine("..","Client") |> Path.GetFullPath 
let port = 8085us

let config =
  { defaultConfig with 
      homeFolder = Some clientPath
      bindings = [ HttpBinding.create HTTP (IPAddress.Parse "0.0.0.0") port ] }

let getInitCounter () : Async<Counter> = async { return 42 }

let init : WebPart = 
  let counterProcotol = 
    { getInitCounter = getInitCounter }
  // creates a WebPart for the given implementation
  FableSuaveAdapter.webPartWithBuilderFor counterProcotol Route.builder

let webPart =
  choose [
    init
    Filters.path "/" >=> Files.browseFileHome "index.html"
    Files.browseHome
    RequestErrors.NOT_FOUND "Not found!"
  ]

startWebServer config webPart
module App

open Elmish
open Elmish.React

open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fable.PowerPack.Fetch

open Shared

open Fulma
open Fulma.Extra.FontAwesome
open Fulma.Layouts
open Fulma.Elements
open Fulma.Components
open Fulma.BulmaClasses
open Fulma.BulmaClasses.Bulma.Properties
open Fulma.Elements.Form

type Model = Counter option

type Msg =
| Increment
| Decrement
| Init of Result<Counter, exn>


module Server = 

  open Shared
  open Fable.Remoting.Client
  
  /// A proxy you can use to talk to server directly
  let api : ICounterProtocol = 
    Proxy.createWithBuilder<ICounterProtocol> Route.builder
    

let init () = 
  let model = None
  let cmd =
    Cmd.ofAsync 
      Server.api.getInitCounter
      () 
      (Ok >> Init)
      (Error >> Init)
  model, cmd

let update msg (model : Model) =
  let model' =
    match model,  msg with
    | Some x, Increment -> Some (x + 1)
    | Some x, Decrement -> Some (x - 1)
    | None, Init (Ok x) -> Some x
    | _ -> None
  model', Cmd.none

let field input =
  Field.field_div [ ]
    [ Field.body [ ]
        [ input ] ]

module Fields =
  let icon = function
  | Good -> Fa.I.SmileO, Button.isSuccess
  | SoSo -> Fa.I.MehO  , Button.isWarning
  | Poor -> Fa.I.FrownO, Button.isDanger


  let column overall =
    let i, option = icon overall
    Level.item [ ]
      [ Button.button_a 
          [ yield option ]
          [ Icon.faIcon [ ]
              [ Fa.icon i; Fa.fa2x ] ] ]

  let overall =
    Level.level [ ]
      [ column Good
        column SoSo
        column Poor ]


  let comment =
    Textarea.textarea [ Textarea.placeholder "Comment" ] []

  let name =
    Input.input [ Input.typeIsText; Input.placeholder "Name" ]

  let submit =
    Button.button_a [ Button.isInfo ] [ str "Submit" ]


let viewForm model dispatch =
  form [ ]
    [ field Fields.overall
      field Fields.comment
      field Fields.name
      field Fields.submit
      ]

let viewBody model dispatch =
  Container.container [ Container.customClass Alignment.HasTextCentered ]
    [ Column.column [ Column.Width.is6; Column.Offset.is3 ]
        [ Heading.h3 [ Heading.isSubtitle ]
            [ str "Score my talk" ]
          Box.box' []
            [ viewForm model dispatch ] ] ]

let view model dispatch =
  Hero.hero [ Hero.isInfo; Hero.isFullHeight ]
    [ Hero.body []
        [ viewBody model dispatch ] ]

#if DEBUG
open Elmish.Debug
open Elmish.HMR
#endif

Program.mkProgram init update view
#if DEBUG
|> Program.withConsoleTrace
|> Program.withHMR
#endif
|> Program.withReact "elmish-app"
#if DEBUG
|> Program.withDebugger
#endif
|> Program.run

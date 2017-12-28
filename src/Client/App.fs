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
open Fulma.BulmaClasses.Bulma
open System.ComponentModel

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
  let overallFA = function
  | Good -> Fa.I.SmileO
  | SoSo -> Fa.I.MehO
  | Poor -> Fa.I.FrownO

  let overallColor = function
  | Good -> Button.isSuccess
  | SoSo -> Button.isWarning
  | Poor -> Button.isDanger

  let column overall =
    Level.item [ ]
      [ Button.button_a 
          [ overallColor overall
            Button.isOutlined ]
          [ Icon.faIcon [ ]
              [ Fa.icon (overallFA overall); Fa.fa2x ] ] ]

  let overall =
    Level.level [ Level.Level.isMobile ]
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

let imgSrc = "http://fsharp.org/img/logo/fsharp256.png"

let viewBody model dispatch =
  Container.container [ Container.customClass Alignment.HasTextCentered ]
    [ Column.column [ Column.Width.is6; Column.Offset.is3 ]
        [ Level.level [ ]
            [ Level.item [ ]
                [ Image.image [Image.is64x64 ]
                    [ img [ Src imgSrc ] ] ] ]
          Heading.h2 [ ]
            [ str "SAFE Demo" ]
          Heading.h3 [ ]
            [ str "Score the talk" ]
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

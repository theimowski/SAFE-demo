module App

open Elmish
open Elmish.React

open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fable.PowerPack.Fetch

open Shared

open Fulma
open Fulma.Layouts
open Fulma.Elements
open Fulma.Components
open Fulma.BulmaClasses
open Fulma.Elements.Form

type Model = Counter option

type Msg =
| Increment
| Decrement
| Init of Result<Counter, exn>

let init () = 
  let model = None
  let cmd =
    let routeBuilder typeName methodName = 
      sprintf "/api/%s/%s" typeName methodName
    let api = Fable.Remoting.Client.Proxy.createWithBuilder<Init> routeBuilder
    Cmd.ofAsync 
      api.getCounter
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

let show = function
| Some x -> string x
| None -> "Loading..."

let button txt onClick = 
  Button.button_btn
    [ Button.isFullWidth
      Button.isPrimary
      Button.onClick onClick ] 
    [ str txt ]

let field lbl input =
  Field.field_div [ ]
    [ Label.label [ ]
        [ str lbl ]
      input ]

let imgSrc = "https://crossweb.pl/upload/gallery/cycles/11255/300x300/lambda_days.png"

let view model dispatch =
  div []
    [ Hero.hero [ Hero.isFullHeight ]
        [ Hero.body [ ]
            [ Container.container [ ]
                [ Columns.columns [ Columns.isCentered ] 
                    [ Image.image [ Image.is128x128 ]
                        [ img [ Src imgSrc ] ] ]
                  
                  Heading.h2 [ ] [ str "How did you like my talk?" ]
                  
                  form []
                    [ field "Name" (Input.input [ Input.typeIsText ])
                      field "Comment (optional)" (Textarea.textarea [ ] [ ])
                    ]
                ]
            ]
        ]
    ]
  
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

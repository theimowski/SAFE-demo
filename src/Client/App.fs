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
open Fulma.Extra.FontAwesome

type Overall =
| Good
| SoSo
| Poor

type Model =
  { Anonymous : bool
    Overall   : Overall option }

type Msg =
| MakeAnonymous of bool
| ChooseOverall of Overall

let init () = 
  let model =
    { Anonymous = false
      Overall   = None }
  let cmd =
    let routeBuilder typeName methodName = 
      sprintf "/api/%s/%s" typeName methodName
    let api = Fable.Remoting.Client.Proxy.createWithBuilder<Init> routeBuilder
    Cmd.none
  model, cmd

let update msg (model : Model) =
  let model' =
    match msg with
    | MakeAnonymous flag -> { model with Anonymous = flag }
    | ChooseOverall overall -> { model with Overall = Some overall }
  model', Cmd.none

let field lbl input =
  Field.field_div [ ]
    [ Label.label [ ]
        [ str lbl ]
      input ]

let imgSrc = "http://fsharp.org/img/logo/fsharp256.png"

let icon = function
| Good -> Fa.I.SmileO, Button.isSuccess
| SoSo -> Fa.I.MehO  , Button.isWarning
| Poor -> Fa.I.FrownO, Button.isDanger

let overall model dispatch =
  let column overall =
    let i, option = icon overall
    Column.column [ ]
      [ Button.button_a 
          [ yield option
            yield Button.onClick (fun _ -> dispatch (ChooseOverall overall))
            if model.Overall <> Some overall then 
              yield Button.isOutlined ]
          [ Icon.faIcon [ ]
              [ Fa.icon i; Fa.fa3x ] ] ]

  Columns.columns [ ]
    [ Column.column [ Column.Width.is4 ]
        [ Columns.columns []
            [ column Good
              column SoSo
              column Poor ] ] ]

let anon model dispatch  =
  let option anonymous =
    Radio.radio [ ]
      [ Radio.input 
          [ Radio.Input.name "anon"
            Radio.Input.props 
              [ Checked (model.Anonymous = anonymous)
                OnClick (fun _ -> dispatch (MakeAnonymous anonymous)) ] ]
        str (if anonymous then "No" else "Yes") ]

  Control.control_div [ ]
    [ option false
      option true ]

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
                    [ yield field "Overall impression" (overall model dispatch)
                      yield field "Comment (optional)" (Textarea.textarea [ ] [ ])
                      yield field "Want to give your name?" (anon model dispatch)
                      if not model.Anonymous then
                        yield field "Name" (Input.input [ Input.typeIsText ])
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

module App

open Elmish
open Elmish.React

open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fable.PowerPack.Fetch

open Fable.Core.JsInterop

open Shared

open Fulma
open Fulma.Layouts
open Fulma.Elements
open Fulma.Components
open Fulma.BulmaClasses
open Fulma.Elements.Form
open Fulma.Extra.FontAwesome

let favs =
  [ 
    "Hot Module Replacement"
    "Time Travel Debugger"
    "Shared Code"
    "Server Refresh"
  ]
  |> Set.ofList

type Model =
  { Anonymous  : bool
    Overall    : Overall option
    Favs       : Set<string>
    Comment    : string
    Submitting : bool
    Results    : VotingResults option }

type Msg =
| MakeAnonymous of bool
| ChooseOverall of Overall
| FavsChanged   of string list
| SetComment    of string
| SubmitForm
| VoteSubmitted of Result<VotingResults, exn>

let routeBuilder typeName methodName = 
  sprintf "/api/%s/%s" typeName methodName
let api = Fable.Remoting.Client.Proxy.createWithBuilder<Votes> routeBuilder

let init () = 
  let model =
    { Anonymous  = false
      Overall    = None
      Favs       = Set.empty
      Comment    = ""
      Submitting = false
      Results    = None }
  let cmd =
    Cmd.none
  model, cmd

let vote model =
  { Name     = if model.Anonymous then None else Some ""
    Overall  = defaultArg model.Overall Good
    Favs     = model.Favs
    Comment  = if model.Comment <> "" then Some model.Comment else None }

let update msg (model : Model) =
  let model' =
    match msg with
    | MakeAnonymous flag    -> { model with Anonymous = flag }
    | ChooseOverall overall -> { model with Overall = Some overall }
    | FavsChanged favs      -> { model with Favs = Set.ofList favs }
    | SetComment comment    -> { model with Comment = comment }
    | SubmitForm            -> { model with Submitting = true } 
    | VoteSubmitted result  ->
      match result with
      | Ok results ->
        { model with Submitting = false; Results = Some results }
      | Error e ->
        Fable.Import.Browser.console.error e
        { model with Submitting = false }
  let cmd =
    match msg with
    | SubmitForm -> 
      Cmd.ofAsync 
        api.sendVote 
        (vote model)
        (Ok >> VoteSubmitted)
        (Error >> VoteSubmitted)
    | _ ->
      Cmd.none
  model', cmd

let field lbl input =
  Field.field_div [ Field.isHorizontal ]
    [ Field.label [ ]
        [ Label.label [ ]
            [ str lbl ] ]
      
      Field.body [ ]
        [ input ] ]

let imgSrc = "http://fsharp.org/img/logo/fsharp256.png"

let icon = function
| Good -> Fa.I.SmileO, Button.isSuccess
| SoSo -> Fa.I.MehO  , Button.isWarning
| Poor -> Fa.I.FrownO, Button.isDanger


let onInput action = OnInput (fun e -> action !!e.target?value) 

let overall model dispatch =
  let column overall =
    let i, option = icon overall
    Column.column [ ]
      [ Button.button_a 
          [ yield option
            yield Button.props [ Disabled model.Submitting ]
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
                Disabled model.Submitting
                OnChange (fun _ -> dispatch (MakeAnonymous anonymous)) ] ]
        str (if anonymous then "No" else "Yes") ]

  Control.control_div [ ]
    [ option false
      option true ]

let getOptions (f : Fable.Import.React.FormEvent) : string list =
  !!f.target?options
  |> List.filter (fun o -> !!o?selected)
  |> List.map (fun o -> !!o?value)

let fav model dispatch =
  Control.control_div [ ]
    [ Select.select [ Select.customClass "is-multiple" ]
        [ select [ Multiple true
                   Size 3.
                   Disabled model.Submitting
                   OnChange (getOptions >> FavsChanged >> dispatch) ]
            [ for f in favs ->
                option 
                  [ Value f ] 
                  [ str f ] ] ] ]

let comment model dispatch =
  Textarea.textarea 
    [ Textarea.props 
        [ onInput (SetComment >> dispatch)
          Disabled model.Submitting ]
      Textarea.value model.Comment ]
    [ ]

let submit model dispatch =
  Control.control_div [ ]
    [ Button.button_a 
        [ yield Button.isPrimary
          yield Button.props [ Disabled model.Submitting ]
          yield Button.onClick (fun _ -> dispatch SubmitForm)
          if model.Submitting then
            yield Button.isLoading ]
        [ str "Submit" ] ]

let viewResults (results : VotingResults) dispatch =
  Hero.body [ ] 
    [ Container.container [ ]
        [ Columns.columns [ Columns.isCentered ]
            [ Column.column [ ]
                [ yield Heading.h4 [ ] [ str "Overall" ]
                  for (overall, cnt) in Map.toList results.Overalls do
                    let i, _ = icon overall
                    yield Icon.faIcon [ ]
                            [ Fa.icon i; Fa.fa3x ]
                    yield str (sprintf "---> %d" cnt) ]
              Column.column [ ]
                [ yield Heading.h4 [ ] [ str "Best parts" ]
                  for (part, cnt) in Map.toList results.FavsCount |> List.sortByDescending snd do
                    yield str (sprintf "%s : %d" part cnt)
                    yield br [ ] ]
              Column.column [ ]
                [ yield Heading.h4 [ ] [ str "Comments" ]
                  for comment in results.Comments |> Array.sortBy (fun x -> x.Length) do
                    yield str comment ] ] ] ]

let viewForm model dispatch =
  Hero.body [ ]
    [ Container.container [ ]
        [ Columns.columns [ Columns.isCentered ] 
            [ Image.image [ Image.is128x128 ]
                [ img [ Src imgSrc ] ] ]
          
          Heading.h3 [ ] [ str "How did you like my talk?" ]
          
          form []
            [ yield field "Overall" (overall model dispatch)
              yield field "Best parts" (fav model dispatch)
              yield field "Comment" (comment model dispatch)
              yield field "Leave name?" (anon model dispatch)
              if not model.Anonymous then
                yield field "Name" (Input.input [ Input.typeIsText ])
              yield field "" (submit model dispatch)
            ]
        ]
    ]

let view model dispatch =
  div []
    [ Hero.hero [ Hero.isFullHeight ]
        [ Hero.head [ ]
            [ Container.container [ ]
                [ Heading.h2 [ ] [ str "SAFE apps with F# web stack" ]
                  Heading.h4 [ ] [ str "by Tomasz Heimowski" ] ] ]
          
          (match model.Results with
          | Some results -> viewResults results dispatch
          | None         -> viewForm model dispatch)
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

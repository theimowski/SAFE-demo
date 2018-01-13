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
open Fulma.Elements.Form
open Fulma.Components
open Fulma.BulmaClasses

open Fulma.BulmaClasses.Bulma
open Fulma.BulmaClasses.Bulma.Properties
open Fulma.Extra.FontAwesome

type Score =
| Poor
| SoSo
| Good

type Model =
  { Score   : Score option
    Name    : string 
    Comment : string
    Loading : bool }

type Msg =
| SetScore   of Score
| SetName    of string
| SetComment of string
| Submit

module Server = 

  open Shared
  open Fable.Remoting.Client
  
  /// A proxy you can use to talk to server directly
  let api : ICounterProtocol = 
    Proxy.createWithBuilder<ICounterProtocol> Route.builder
    

let init () = 
  let model = 
    { Score   = None
      Name    = ""
      Comment = ""
      Loading = false }
  let cmd = Cmd.none
  model, cmd

let update msg (model : Model) =
  let model' =
    match msg with
    | SetScore   score   -> { model with Score   = Some score }
    | SetName    name    -> { model with Name    = name  }
    | SetComment comment -> { model with Comment = comment }
    | Submit             -> { model with Loading = true }
  model', Cmd.none

let navBrand =
  Navbar.brand_div [ ] 
    [ Navbar.item_a 
        [ Navbar.Item.props [ Href "https://safe-stack.github.io/" ]
          Navbar.Item.isActive ] 
        [ img [ Src "https://safe-stack.github.io/images/safe_top.png"
                Alt "Logo" ] ] 
      Navbar.burger [ ] 
        [ span [ ] [ ]
          span [ ] [ ]
          span [ ] [ ] ] ]

let navMenu =
  Navbar.menu [ ]
    [ Navbar.end_div [ ] 
        [ Navbar.item_a [ ] 
            [ str "Home" ] 
          Navbar.item_a [ ]
            [ str "Examples" ]
          Navbar.item_a [ ]
            [ str "Documentation" ]
          Navbar.item_div [ ]
            [ Button.button_a 
                [ Button.isWhite
                  Button.isOutlined
                  Button.isSmall
                  Button.props [ Href "https://github.com/SAFE-Stack/SAFE-template" ] ] 
                [ Icon.faIcon [ ] 
                    [ Fa.icon Fa.I.Github; Fa.fw ]
                  span [ ] [ str "View Source" ] ] ] ] ]

let field input =
  Field.field_div [ ]
    [ Field.body [ ] 
        [ input ] ]

let scoreIcon = function
| Poor -> Fa.I.FrownO
| SoSo -> Fa.I.MehO
| Good -> Fa.I.SmileO

let scoreColor = function
| Poor -> Button.isDanger
| SoSo -> Button.isWarning
| Good -> Button.isSuccess

let onInput action = OnInput (fun e -> action !!e.target?value) 

let score model dispatch =
  let column score =
    let icon  = scoreIcon score
    let color = scoreColor score

    Level.item [ ]
      [ Button.button_a 
          [ yield color
            yield Button.onClick (fun _ -> 
              if not model.Loading then dispatch (SetScore score))
            if model.Loading then
              yield Button.isDisabled
            if model.Score <> Some score then
              yield Button.isOutlined ] 
          [ Icon.faIcon [ ]
              [ Fa.icon icon
                Fa.fa2x ] ] ]

  Level.level [ Level.Level.isMobile ]
    [ column Good 
      column SoSo
      column Poor ]

let comment model dispatch =
  Textarea.textarea 
    [ yield Textarea.placeholder "Comment"
      yield Textarea.value model.Comment
      if model.Loading then
        yield Textarea.isDisabled
      yield Textarea.props [ onInput (SetComment >> dispatch) ] ]
    [ ]

let name model dispatch =
  Input.input 
    [ Input.typeIsText
      Input.placeholder "Name"
      Input.value model.Name
      Input.disabled model.Loading
      Input.props [ onInput (SetName >> dispatch) ] ]

let submit model dispatch =
  Button.button_a 
    [ yield Button.isPrimary
      yield Button.isFullWidth
      yield Button.onClick (fun _ -> dispatch Submit)
      if model.Loading then
        yield Button.isLoading ]
    [ str "Submit" ]

let containerBox model dispatch =
  Box.box' [ ]
    [ field (score model dispatch)
      field (comment model dispatch)
      field (name model dispatch)
      field (submit model dispatch) ]

let imgSrc = "http://fsharp.org/img/logo/fsharp256.png"

let view model dispatch =
  Hero.hero [ Hero.isPrimary; Hero.isFullHeight ] 
    [ Hero.head [ ] 
        [ Navbar.navbar [ ]
            [ Container.container [ ]
                [ navBrand
                  navMenu ] ] ]
      
      Hero.body [ ] 
        [ Container.container [ Container.customClass Alignment.HasTextCentered ]
            [ Column.column 
                [ Column.Width.is6
                  Column.Offset.is3 ]
                [ Level.level [ ]
                    [ Level.item [ ] 
                        [ Image.image [ Image.is64x64 ]
                            [ img [ Src imgSrc ] ] ] ]
                  h1 [ ClassName "title" ] 
                    [ str "SAFE Demo" ]
                  div [ ClassName "subtitle" ]
                    [ str "Score my talk" ]
                  containerBox model dispatch ] ] ] ]

  
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

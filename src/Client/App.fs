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
  { Score   : Score 
    Name    : string 
    Comment : string }

type Msg =
| SetScore   of Score
| SetName    of string
| SetComment of string

module Server = 

  open Shared
  open Fable.Remoting.Client
  
  /// A proxy you can use to talk to server directly
  let api : ICounterProtocol = 
    Proxy.createWithBuilder<ICounterProtocol> Route.builder
    

let init () = 
  let model = 
    { Score   = Good
      Name    = ""
      Comment = "" }
  let cmd = Cmd.none
  model, cmd

let update msg (model : Model) =
  let model' =
    match msg with
    | SetScore   score   -> { model with Score   = score }
    | SetName    name    -> { model with Name    = name  }
    | SetComment comment -> { model with Comment = comment }
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

let onInput action = OnInput (fun e -> action !!e.target?value) 

let comment model dispatch =
  Textarea.textarea 
    [ Textarea.placeholder "Comment"
      Textarea.value model.Comment
      Textarea.props [ onInput (SetComment >> dispatch) ] ]
    [ ]

let name =
  Input.input [ Input.typeIsText; Input.placeholder "Name" ]

let submit =
  Button.button_a [ Button.isPrimary; Button.isFullWidth ] [ str "Submit" ]

let containerBox model dispatch =
  Box.box' [ ]
    [ field (comment model dispatch)
      field name
      field submit ]

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

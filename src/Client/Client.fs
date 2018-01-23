module Client

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

open Fulma.BulmaClasses.Bulma
open Fulma.BulmaClasses.Bulma.Properties
open Fulma.Extra.FontAwesome

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

let safeComponents =
  let intersperse sep ls =
    List.foldBack (fun x -> function
      | [] -> [x]
      | xs -> x::sep::xs) ls []

  let components =
    [
      "Suave", "http://suave.io"
      "Fable", "http://fable.io"
      "Elmish", "https://fable-elmish.github.io/"
      "Fulma", "https://mangelmaxime.github.io/Fulma" 
      "Bulma\u00A0Templates", "https://dansup.github.io/bulma-templates/"
      "Fable.Remoting", "https://github.com/Zaid-Ajaj/Fable.Remoting"
    ]
    |> List.map (fun (desc,link) -> a [ Href link ] [ str desc ] )
    |> intersperse (str ", ")
    |> span [ ]

  p [ ]
    [ strong [] [ str "SAFE Template" ]
      str " powered by: "
      components ]

let show = function
| Some x -> string x
| None -> "Loading..."

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

let containerBox model dispatch =
  Box.box' [ ]
    [ Form.Field.field_div [ Form.Field.isGrouped ] 
        [ Form.Control.control_p [ Form.Control.customClass "is-expanded"] 
            [ Form.Input.input
                [ Form.Input.typeIsNumber
                  Form.Input.disabled true
                  Form.Input.value (show model) ] ]
          Form.Control.control_p [ ]
            [ Button.button_a 
                [ Button.isPrimary
                  Button.onClick (fun _ -> dispatch Increment) ]
                [ str "+" ] ]
          Form.Control.control_p [ ]
            [ Button.button_a 
                [ Button.isPrimary
                  Button.onClick (fun _ -> dispatch Decrement) ]
                [ str "-" ] ] ] ]

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
                [ h1 [ ClassName "title" ] 
                    [ str "SAFE Template" ]
                  div [ ClassName "subtitle" ]
                    [ safeComponents ]
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

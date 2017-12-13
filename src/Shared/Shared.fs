namespace Shared

type Overall =
| Good
| SoSo
| Poor

type Vote =
  { Name      : string option
    Overall   : Overall
    Favs      : Set<string>
    Comment   : string option }

type VotingResults =
  { Overalls  : Map<Overall, int>
    FavsCount : Map<string, int>
    Comments  : string [] }

type Votes =
  { sendVote : Vote -> Async<VotingResults> }

namespace Shared

type Score =
| Poor
| SoSo
| Good

type Vote =
  { Score   : Score 
    Name    : string 
    Comment : string }

type VotingResults =
  { Scores   : Map<Score,int>
    Comments : string [] }

module Route =
  /// Defines how routes are generated on server and mapped from client
  let builder typeName methodName = 
    sprintf "/api/%s/%s" typeName methodName

type IVotingProtocol =
  { vote : Vote -> Async<VotingResults> }

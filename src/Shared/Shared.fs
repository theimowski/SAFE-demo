namespace Shared

type Score =
| Poor
| SoSo
| Good

type Vote =
  { Comment : string 
    Name    : string 
    Score   : Score }

type VotingResults =
  { Comments : string []
    Scores : Map<Score, int> }

module Route =
  /// Defines how routes are generated on server and mapped from client
  let builder typeName methodName = 
    sprintf "/api/%s/%s" typeName methodName

type IVotingProtocol =
  { vote : Vote -> Async<VotingResults> }
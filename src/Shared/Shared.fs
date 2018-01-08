namespace Shared

type Counter = int

module Route =
  /// Defines how routes are generated on server and mapped from client
  let builder typeName methodName = 
    sprintf "/api/%s/%s" typeName methodName

/// A type that specifies the communication protocol for client and server
/// Every record field must have the type : 'a -> Async<'b> where 'a can also be `unit`
/// Add more such fields, implement them on the server and they be directly available on client
type ICounterProtocol =
  { getInitCounter : unit -> Async<Counter> }

module App

open AWSLambda.Types
open AWSLambda.Types.APIGateway.Request
open AWSLambda.Types.APIGateway.Response

// Our simple Lambda - its bound to an API Gateway proxy trigger and recieves the proxy request
// as its event. The second, unused here, parameter is the Lambda context (of type LambdaContext). 
let echo (event: ProxyRequest, _) = promise {
  lambdaLogger "Entered lambda"
  return
    match (getQueryParameter event "message") with
    | Some value -> {| echo = value |} |> Json |> ok 
    | None -> "No message" |> PlainText |> ok
}

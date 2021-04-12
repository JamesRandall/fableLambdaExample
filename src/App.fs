module App

open AWSLambda.Types
open AWSLambda.Types.APIGateway

// Our simple Lambda - its bound to an API Gateway proxy trigger and recieves the proxy request
// as its event. The second, unused here, parameter is the Lambda context (of type LambdaContext). 
let echo (event: ProxyRequest, _) = promise {
  lambdaLogger "Entered lambda"
  return
    match (getQueryParameter event "message") with
    | Some value -> Response.ok (OfJson {| echo = value |})
    | None -> Response.ok (OfText "No message")
}


module AWSLambda.Types

open System
open Fable.Core.JsInterop
open Fable.Core.JS

let lambdaLogger msg =
  console.log msg
  
let private safeGetFromObj objDictionary (propertyName:string) =
  if objDictionary = null then
    None
  else
    let possibleValue = objDictionary?(propertyName)
    if possibleValue = null then None else possibleValue.ToString() |> Some

type ClientApplication =
  { appPackageName: string
    appTitle: string
    appVersionCode: string
    appVersionName: string
    installationId: string
  }
  
type ClientEnvironment =
  {
    platformVersion: string
    platform: string
    make: string
    model: string
    locale: string
  }

type ClientContext =
  { environment: ClientEnvironment
    client: ClientApplication
    custom: obj
  }
  member context.getCustom = safeGetFromObj context.custom
  
type CognitoIdentity =
  { cognitoIdentityId: string
    cognitoIdentityPoolId: string
  }

type LambdaContext =
  { awsRequestId: string
    clientContext: ClientContext
    functionName: string
    functionVersion: string
    identity: CognitoIdentity
    invokedFunctionArn: string
    logGroupName: string
    logStreamName: string
    memoryLimitInMB: int
    remainingTime: TimeSpan
    getRemainingTimeInMillis : unit -> float
  }
  
module APIGateway =
  type ProxyRequestContext =
    {
      path: string
      accountId: string
      resourceId: string
      stage: string
      requestId: string
      resourcePath: string
      httpMethod: string
      apiId: string
      extendedRequestId: string
      connectionId: string
      connectionAt: string
      domainName: string
      domainPrefix: string
      eventType: string
      messageId: string
      routeKey: string      
      operationName: string
      error: string
      integrationLatency: string
      messageDirection: string
      requestTime: string
      requestTimeEpoch: float
      status: string
    }
  
  type ProxyRequest =
    { resource: string
      path: string
      httpMethod: string
      headers: obj
      multiValueHeaders: obj
      queryStringParameters: obj
      multiValueQueryStringParameters: obj
      pathParameters: obj
      stageVariables: obj
      requestContext: ProxyRequestContext
      body: string
      isBase64Encoded: bool      
    }
    
  type ProxyResponse =
    { statusCode: int
      headers: obj
      multiValueHeaders: obj
      body: string
      isBase64Encoded: bool
    }
    static member Empty =
      { statusCode = 500
        headers = createObj [ ]
        multiValueHeaders = createObj []
        body = ""
        isBase64Encoded = false
      }    
      
  let getQueryParameter request = safeGetFromObj request.queryStringParameters
  let getHeader request = safeGetFromObj request.headers
  let getPathParameter request = safeGetFromObj request.pathParameters
  let getStageVariable request = safeGetFromObj request.stageVariables
  
  type ResponseBodyType =
    | OfNone
    | OfJson of obj
    | OfText of string
  
  module Response =
    let ok responseBodyType =
      let contentType,body =
        match responseBodyType with
        | OfNone -> "text/plain",""
        | OfJson value -> "application/json", JSON.stringify value
        | OfText value -> "text/plain",value 
     
      { ProxyResponse.Empty with body = body
                                 headers = createObj [ "Content-Type" ==> contentType ]
                                 statusCode = 200 }
    let internalError =
      { ProxyResponse.Empty with statusCode = 500 }
namespace Infrastructure

open Amazon.CDK
open Amazon.CDK.AWS.APIGatewayv2
open Amazon.CDK.AWS.APIGatewayv2.Integrations
open Amazon.CDK.AWS.IAM
open Amazon.CDK.AWS.Lambda

type LambdaStack(scope, id, props) as stack =
    inherit Stack(scope, id, props)
    
    let standardTags = [
      "application","fablefsharplambdasample"
      "environment", "dev"
    ]

    let applyStandardTags construct =
      standardTags |> List.iter(fun (n,v) -> Tags.Of(construct).Add(n,v))  
      construct
      
    let lambdaRole =
      Role(
        stack,
        "fablelambdarole",
        RoleProps(
          AssumedBy = ServicePrincipal("lambda.amazonaws.com"),
          ManagedPolicies = [|
            ManagedPolicy.FromAwsManagedPolicyName("service-role/AWSLambdaBasicExecutionRole")
          |]
        )
      ) |> applyStandardTags    
    
    let echoFunction =
      Function(
        stack,
        "fableecholambda",
        FunctionProps(
          Runtime = Runtime.NODEJS_12_X,
          Role = lambdaRole,
          Code = Code.FromAsset("./build/dist"),
          Handler = "App.echo",
          MemorySize = 256.          
        )
      ) |> applyStandardTags
    
    let fableLambdaApi =
      HttpApi(
        stack,
        "fablelambda-api",
        HttpApiProps(
          ApiName = "Fable Lambda API",
          Description = "API for Fable lambda functions",
          CreateDefaultStage = true            
        )
      ) |> applyStandardTags
      
    let echoFunctionProxyIntegration =
      LambdaProxyIntegration(
        LambdaProxyIntegrationProps(
          Handler = echoFunction,
          PayloadFormatVersion = PayloadFormatVersion.VERSION_2_0
        )
      )
    do 
      fableLambdaApi.AddRoutes(
        AddRoutesOptions(
          Path="/echo",
          Integration = echoFunctionProxyIntegration,
          Methods = [| HttpMethod.POST |]
        )
      ) |> ignore
    
    do CfnOutput(stack, "apiGatewayUrl", CfnOutputProps(ExportName=("apiGatewayUrl"), Value=fableLambdaApi.Url)) |> ignore

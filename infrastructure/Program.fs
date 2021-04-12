open Amazon.CDK
open Infrastructure

[<EntryPoint>]
let main _ =
    let app = App(null)
    
    LambdaStack(app, "FableLambdaStack", StackProps()) |> ignore

    app.Synth() |> ignore
    0
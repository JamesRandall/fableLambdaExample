dotnet fable .\src\AWSLambda.fsproj --outDir "./build/fable"
parcel build ./build/fable/App.js --target=node --bundle-node-modules --no-minify --out-dir ./build/dist


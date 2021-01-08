// Learn more about F# at http://docs.microsoft.com/dotnet/fsharp

open System
open FSharpDev.HelloWorld
open FSharpDev.HttpTests

[<EntryPoint>]
let main argv =

    //HelloWorldExample.run()
    //CustomSettingsExample.run()
    SimpleHttpTest.run()

    0 // return an integer exit code

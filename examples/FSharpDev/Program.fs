open FSharpDev.HelloWorld
open FSharpDev.Plugin

[<EntryPoint>]
let main argv =

    HelloWorldExample.run()
    //SimplePluginReportExample.run()
    //AdvancedPluginReportExample.run()

    0 // return an integer exit code

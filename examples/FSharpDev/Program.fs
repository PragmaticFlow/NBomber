open FSharpDev.HelloWorld
open FSharpDev.Plugin

[<EntryPoint>]
let main argv =

    HelloWorldExample.run()
    //PluginHtmlReportExample.run()

    0 // return an integer exit code

namespace System

open System.Reflection

[<assembly: AssemblyVersionAttribute("0.2.6")>]
do ()

module internal AssemblyVersionInformation =
    let [<Literal>] Version = "0.2.6"
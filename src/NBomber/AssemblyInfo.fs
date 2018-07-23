namespace System

open System.Reflection

[<assembly: AssemblyVersionAttribute("0.2.4")>]
do ()

module internal AssemblyVersionInformation =
    let [<Literal>] Version = "0.2.4"
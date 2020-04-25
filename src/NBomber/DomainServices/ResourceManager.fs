module internal NBomber.DomainServices.ResourceManager

open System.IO

open NBomber.Contracts

let tryReadResource (name) =
    let assembly = typedefof<TestInfo>.Assembly
    assembly.GetManifestResourceNames()
    |> Array.tryFind(fun x -> x.Contains(name))
    |> Option.map(fun resourceName ->
        use stream = assembly.GetManifestResourceStream(resourceName)
        use reader = new StreamReader(stream)
        reader.ReadToEnd()
    )

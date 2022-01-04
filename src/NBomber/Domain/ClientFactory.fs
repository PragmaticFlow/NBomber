module internal NBomber.Domain.ClientFactory

open System.Threading.Tasks

open FSharp.Control.Tasks.NonAffine

open NBomber.Contracts
open NBomber.Extensions.InternalExtensions
open NBomber.Errors

let createFullName (factoryName: string) (scenarioName: string) =
    $"{factoryName}@{scenarioName}"

let getOriginalName (fullName: string) =
    fullName |> String.split [|"@"|] |> Array.head

let checkName (factoryName: string) =
    if factoryName.Contains("@") then Error(InvalidClientFactoryName factoryName)
    else Ok factoryName

type ClientFactory<'TClient>(name: string,
                             clientCount: int,
                             initClient: int * IBaseContext -> Task<'TClient>, // number * context
                             disposeClient: 'TClient * IBaseContext -> Task) =

    // we use lazy to prevent multiply initialization in one scenario
    // also, we do check on duplicates (that has the same name but different implementation) within one scenario
    let untypedFactory = lazy (
        ClientFactory<obj>(name, clientCount,
            initClient = (fun (number,token) -> task {
                let! client = initClient(number, token)
                return client :> obj
            }),
            disposeClient = (fun (client,context) -> disposeClient(client :?> 'TClient, context))
        )
    )

    member _.FactoryName = name
    member _.ClientCount = clientCount
    member _.GetUntyped() = untypedFactory.Value
    member _.Clone(newName: string) = ClientFactory<'TClient>(newName, clientCount, initClient, disposeClient)
    member _.Clone(newClientCount: int) = ClientFactory<'TClient>(name, newClientCount, initClient, disposeClient)

    interface IClientFactory<'TClient> with
        member _.FactoryName = name
        member _.ClientCount = clientCount
        member _.InitClient(number, context) = initClient(number, context)
        member _.DisposeClient(client, context) = disposeClient(client, context)

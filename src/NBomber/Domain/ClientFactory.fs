module internal NBomber.Domain.ClientFactory

open System.Threading.Tasks

open NBomber
open NBomber.Contracts
open NBomber.Extensions.Internal
open NBomber.Errors

let createFullName (factoryName: string) (scenarioName: string) =
    $"{factoryName}@{scenarioName}"

let getOriginalName (fullName: string) =
    fullName |> String.split [|"@"|] |> Array.head

let checkName (factoryName: string) =
    if factoryName.Contains("@") then Error(InvalidClientFactoryName factoryName)
    else Ok factoryName

let safeInitClient (initClient: int * IBaseContext -> Task<unit>)
                   (clientNumber: int)
                   (context: IBaseContext) =

    Operation.retry(Constants.MaxClientInitFailCount, fun () -> backgroundTask {
        try
            do! initClient(clientNumber, context)
            return Ok()
        with
        | ex -> return Error ex
    })

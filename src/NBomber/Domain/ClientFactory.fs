module internal NBomber.Domain.ClientFactory

open NBomber.Extensions.Internal
open NBomber.Errors

let createFullName (factoryName: string) (scenarioName: string) =
    $"{factoryName}@{scenarioName}"

let getOriginalName (fullName: string) =
    fullName |> String.split [|"@"|] |> Array.head

let checkName (factoryName: string) =
    if factoryName.Contains("@") then Error(InvalidClientFactoryName factoryName)
    else Ok factoryName

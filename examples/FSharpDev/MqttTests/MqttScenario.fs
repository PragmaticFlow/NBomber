module FSharpDev.MqttTests.MqttScenario

open NBomber.FSharp

let run () =

    NBomberRunner.registerScenarios [
        PublisherScenario.create()
        SubscriberScenario.create()
    ]
    |> NBomberRunner.run
    |> ignore

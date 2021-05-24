module FSharpProd.HttpTests.AdvancedHttpWithConfig
open FSharp.Control.Tasks.NonAffine
open Newtonsoft.Json
open NBomber
open NBomber.Contracts
open NBomber.FSharp
open NBomber.Plugins.Http.FSharp
open NBomber.Plugins.Network.Ping

open FSharpProd.HttpTests.AdvancedHttpTest

// in this example we use:
// - NBomber.Http (https://nbomber.com/docs/plugins-http)

[<CLIMutable>]
type User = {
    Id: int
    Name: string
}

let run () =

    let userFeed = FeedData.fromJson<User> "./HttpTests/Configs/user-feed.json"
                   |> Feed.createRandom "userFeed"

    let getUser = Step.create("get_user", feed = userFeed, execute = fun context ->

        let userId = context.FeedItem
        let url = "https://jsonplaceholder.typicode.com/users?id=" + userId.Id.ToString()

        Http.createRequest "GET" url
        |> Http.withCheck(fun response -> task {
            let! json = response.Content.ReadAsStringAsync()

            // parse JSON
            let users = json
                        |> JsonConvert.DeserializeObject<UserResponse[]>
                        |> ValueOption.ofObj

            match users with
            | ValueSome usr when usr.Length = 1 ->
                return Response.ok(usr.[0]) // we pass user object response to the next step

            | _ -> return Response.fail("not found user: " + userId.Id.ToString())
        })
        |> Http.send context
    )

    // this 'getPosts' will be executed only if 'getUser' finished OK.
    let getPosts = Step.create("get_posts", execute = fun context ->

        let user = context.GetPreviousStepResponse<UserResponse>()
        let url = "https://jsonplaceholder.typicode.com/posts?userId=" + user.Id

        Http.createRequest "GET" url
        |> Http.withCheck(fun response -> task {
            let! json = response.Content.ReadAsStringAsync()

            // parse JSON
            let posts = json
                        |> JsonConvert.DeserializeObject<PostResponse[]>
                        |> ValueOption.ofObj

            match posts with
            | ValueSome ps when ps.Length > 0 ->
                return Response.ok()

            | _ -> return Response.fail()
        })
        |> Http.send context
    )

    Scenario.create "rest_api" [getUser; getPosts]
    |> NBomberRunner.registerScenario
    |> NBomberRunner.withWorkerPlugins [new PingPlugin()]
    |> NBomberRunner.loadConfig "./HttpTests/Configs/config.json"
    |> NBomberRunner.loadInfraConfig "./HttpTests/Configs/infra-config.json"
    |> NBomberRunner.run
    |> ignore

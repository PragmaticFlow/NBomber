module internal NBomber.Domain.Feed

open System
open System.Threading.Tasks
open NBomber.Contracts

let toUntypedFeed (feed: IFeed<'TFeedItem>) = {
    new obj() with
        override _.GetHashCode() = feed.GetHashCode()
        override _.Equals(instance) = instance.GetHashCode() = feed.GetHashCode()
    interface IFeed<obj> with
        member _.FeedName = feed.FeedName
        member _.Init(context) = feed.Init(context)
        member _.GetNextItem(scenarioId, stepData) = feed.GetNextItem(scenarioId, stepData) :> obj
}

let constant (name, data: 'T seq) =
    let mutable _allItems = Array.empty

    { new IFeed<'T> with
        member _.FeedName = name

        member _.Init(context) =
            _allItems <- data |> Seq.toArray
            Task.CompletedTask

        member _.GetNextItem(scenarioId, stepData) =
            let index = scenarioId.Number % _allItems.Length
            _allItems.[index] }

let circular (name, data: 'T seq) =
    let mutable _enumerator = Unchecked.defaultof<_>

    let createInfiniteStream (items: 'T seq) = seq {
        while true do
            yield! items
    }

    { new IFeed<'T> with
        member _.FeedName = name

        member _.Init(context) =
            let infiniteItems = data |> createInfiniteStream
            _enumerator <- infiniteItems.GetEnumerator()
            Task.CompletedTask

        member _.GetNextItem(scenarioId, stepData) =
            _enumerator.MoveNext() |> ignore
            _enumerator.Current }

let random (name, data: 'T seq) =
    let _random = Random()
    let mutable _allItems = Array.empty

    let getRandomItem () =
        let index = _random.Next(_allItems.Length)
        _allItems.[index]

    { new IFeed<'T> with
        member _.FeedName = name

        member _.Init(context) =
            _allItems <- data |> Seq.toArray
            Task.CompletedTask

        member _.GetNextItem(scenarioId, stepData) = getRandomItem() }

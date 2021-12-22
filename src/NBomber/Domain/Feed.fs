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
        member _.GetNextItem(scenarioInfo, stepData) = feed.GetNextItem(scenarioInfo, stepData) :> obj
}

let constant (name, getData: IBaseContext -> 'T seq) =
    let mutable _allItems = Array.empty

    { new IFeed<'T> with
        member _.FeedName = name

        member _.Init(context) =
            _allItems <- getData(context) |> Seq.toArray
            Task.CompletedTask

        member _.GetNextItem(scenarioInfo, stepData) =
            let index = scenarioInfo.ThreadNumber % _allItems.Length
            _allItems[index] }

let circular (name, getData: IBaseContext -> 'T seq) =
    let mutable _enumerator = Unchecked.defaultof<_>
    let _lockObj = obj()

    let createInfiniteStream (items: 'T seq) = seq {
        while true do
            yield! items
    }

    { new IFeed<'T> with
        member _.FeedName = name

        member _.Init(context) =
            let infiniteItems = getData(context) |> createInfiniteStream
            _enumerator <- infiniteItems.GetEnumerator()
            Task.CompletedTask

        member _.GetNextItem(scenarioInfo, stepData) =
            lock _lockObj (fun _ ->
                _enumerator.MoveNext() |> ignore
                _enumerator.Current
            ) }

let random (name, getData: IBaseContext -> 'T seq) =
    let _random = Random()
    let mutable _allItems = Array.empty

    let getRandomItem () =
        let index = _random.Next(_allItems.Length)
        _allItems[index]

    { new IFeed<'T> with
        member _.FeedName = name

        member _.Init(context) =
            _allItems <- getData(context) |> Seq.toArray
            Task.CompletedTask

        member _.GetNextItem(scenarioInfo, stepData) = getRandomItem() }

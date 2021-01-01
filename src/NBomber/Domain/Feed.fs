module internal NBomber.Domain.Feed

open System
open System.Threading.Tasks
open NBomber
open NBomber.Contracts

let toUntypedFeed (feed: IFeed<'TFeedItem>) = {
    new obj() with
        override _.GetHashCode() = feed.GetHashCode()
        override _.Equals(instance) = instance.GetHashCode() = feed.GetHashCode()
    interface IFeed<obj> with
        member _.FeedName = feed.FeedName
        member _.Init(context) = feed.Init(context)
        member _.GetNextItem(correlationId, stepData) = feed.GetNextItem(correlationId, stepData) :> obj
}

let rec createInfiniteStream (items: 'T seq) = seq {
    yield! items
    yield! createInfiniteStream items
}

let constant (name, provider: IFeedProvider<'T>) =
    let mutable _allItems = Array.empty

    { new IFeed<'T> with
        member _.FeedName = name

        member _.Init(context) =
            _allItems <- provider.GetAllItems() |> Seq.toArray
            Task.CompletedTask

        member _.GetNextItem(correlationId, stepData) =
            let index = correlationId.CopyNumber % _allItems.Length
            _allItems.[index] }

let circular (name, provider: IFeedProvider<'T>) =
    let mutable _enumerator = Unchecked.defaultof<_>

    { new IFeed<'T> with
        member _.FeedName = name

        member _.Init(context) =
            let infiniteItems = provider.GetAllItems() |> createInfiniteStream
            _enumerator <- infiniteItems.GetEnumerator()
            Task.CompletedTask

        member _.GetNextItem(correlationId, stepData) =
         _enumerator.MoveNext() |> ignore
         _enumerator.Current }

let random (name, provider: IFeedProvider<'T>) =
    let _random = Random()
    let mutable _allItems = Array.empty

    let getRandomItem () =
        let index = _random.Next(_allItems.Length)
        _allItems.[index]

    { new IFeed<'T> with
        member _.FeedName = name

        member _.Init(context) =
            _allItems <- provider.GetAllItems() |> Seq.toArray
            Task.CompletedTask

        member _.GetNextItem(correlationId, stepData) = getRandomItem() }

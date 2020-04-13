module internal NBomber.Domain.Feed

open System
open NBomber
open NBomber.Contracts

let toUntypedFeed (feed: IFeed<'TFeedItem>) =
    { new IFeed<obj> with
        member x.FeedName = feed.FeedName
        member x.GetNextItem(correlationId, stepData) = feed.GetNextItem(correlationId, stepData) :> obj }

let rec createInfiniteStream (items: 'T seq) =
    seq {
        yield! items
        yield! createInfiniteStream items
    }

let empty<'T> =
    { new IFeed<'T> with
        member x.FeedName = Constants.EmptyFeedName
        member x.GetNextItem(correlationId, stepData) = Unchecked.defaultof<'T> }

let constant (name, provider: IFeedProvider<'T>) =
    let allItems = provider.GetAllItems()

    { new IFeed<'T> with
        member x.FeedName = name
        member x.GetNextItem(correlationId, stepData) =
            let index = correlationId.CopyNumber % allItems.Length
            allItems.[index] }

let circular (name, provider: IFeedProvider<'T>) =

    let infiniteItems = provider.GetAllItems() |> createInfiniteStream
    let enumerator = infiniteItems.GetEnumerator()

    { new IFeed<'T> with
        member x.FeedName = name
        member x.GetNextItem(correlationId, stepData) =
         enumerator.MoveNext() |> ignore
         enumerator.Current }

let random (name, provider: IFeedProvider<'T>) =

    let random = Random()
    let allItems = provider.GetAllItems()

    let getRandomItem () =
        let index = random.Next(allItems.Length)
        allItems.[index]

    { new IFeed<'T> with
        member x.FeedName = name
        member x.GetNextItem(correlationId, stepData) = getRandomItem() }

module NBomber.Domain.BatchFeed

open System
open System.Globalization
open System.IO
open System.Threading.Tasks
open CsvHelper
open NBomber.Contracts
open NBomber.Extensions.InternalExtensions

type LoadingMode =
    | Eager
    | Batch of int

type FeedStrategy =
    | Random
    | Circular

type BatchableFeedData = {
    filePath: string
    loadingMode: LoadingMode
    feedStrategy: FeedStrategy
}

let fromCsv (filePath: string) =
    { filePath = filePath
      loadingMode = LoadingMode.Eager
      feedStrategy = FeedStrategy.Circular }

let batch (batchSize: int) (feedData: BatchableFeedData) =
    { feedData with loadingMode = LoadingMode.Batch batchSize}

let eager (feedData: BatchableFeedData) =
    { feedData with loadingMode = LoadingMode.Eager}

let random(feedData: BatchableFeedData) =
    { feedData with feedStrategy = FeedStrategy.Random }

let circular(feedData: BatchableFeedData) =
    { feedData with feedStrategy = FeedStrategy.Circular }

let private fillBatch (filePath: string, batch: 'T[]) =
    let stream = File.OpenText(filePath)
    let csvReader = new CsvReader(stream, CultureInfo.InvariantCulture, true)
    let mutable enumerator = csvReader.GetRecords<'T>().GetEnumerator()

    let filler () =
        let mutable i = 0
        while i < batch.Length do

            if enumerator.MoveNext() then
                batch[i] <- enumerator.Current
                i <- i + 1
            else
                stream.BaseStream.Position <- 0
                stream.DiscardBufferedData()
                stream.ReadLine() |> ignore
                enumerator <- csvReader.GetRecords<'T>().GetEnumerator()
    filler

let private CircularBatchedSeparatedValuesFeeder<'T> (name, filePath: string, batchSize: int) =
    let _lockObj = obj()
    let batch = Array.zeroCreate batchSize
    let fillBatch = fillBatch(filePath, batch)
    let mutable index = Int32.MaxValue

    { new IFeed<'T> with

        member _.FeedName = name

        member _.Init(context) =
            fillBatch ()
            index <- 0
            Task.CompletedTask

        member _.GetNextItem(scenarioInfo, stepData) =
            lock _lockObj (fun _ ->
                if index < batch.Length then
                    let record = batch[index]
                    index <- index + 1
                    record
                else
                    fillBatch ()
                    index <- 1
                    batch[0]
            ) }

let private RandomBatchedSeparatedValuesFeeder<'T> (name, filePath: string, batchSize: int) =
    let _lockObj = obj()
    let batch = Array.zeroCreate batchSize
    let fillBatch = fillBatch(filePath, batch)
    let mutable index = Int32.MaxValue

    { new IFeed<'T> with

        member _.FeedName = name

        member _.Init(context) =
            fillBatch ()
            index <- 0
            Task.CompletedTask

        member _.GetNextItem(scenarioInfo, stepData) =
            lock _lockObj (fun _ ->
                if index < batch.Length then
                    let record = batch[index]
                    index <- index + 1
                    record
                else
                    fillBatch ()
                    Array.shuffleInPlace batch
                    index <- 1
                    batch[0]
            ) }

let private BatchedSeparatedValuesFeeder<'T>(name, filePath: string, feedStrategy: FeedStrategy, batchSize: int) =
    match feedStrategy with
    | Circular -> CircularBatchedSeparatedValuesFeeder<'T>(name, filePath, batchSize)
    | Random -> RandomBatchedSeparatedValuesFeeder<'T>(name, filePath, batchSize)

let private CircularEagerSeparatedValuesFeeder<'T> (name, filePath: string) =
    let _lockObj = obj()
    let csvReader = new CsvReader(File.OpenText(filePath), CultureInfo.InvariantCulture, true)

    let mutable _enumerator = Unchecked.defaultof<_>

    let createInfiniteStream (items: 'T seq) = seq {
        while true do
            yield! items
    }

    { new IFeed<'T> with

        member _.FeedName = name

        member _.Init(context) =
            let infiniteItems = csvReader.GetRecords<'T>() |> Seq.toArray |> createInfiniteStream
            _enumerator <- infiniteItems.GetEnumerator()
            Task.CompletedTask

        member _.GetNextItem(scenarioInfo, stepData) =
            lock _lockObj (fun _ ->
                _enumerator.MoveNext() |> ignore
                _enumerator.Current
            ) }

let private RandomEagerSeparatedValuesFeeder<'T> (name, filePath: string) =
    let _random = System.Random()
    let csvReader = new CsvReader(File.OpenText(filePath), CultureInfo.InvariantCulture, true)
    let mutable _allItems = Array.empty

    let getRandomItem () =
        let index = _random.Next(_allItems.Length)
        _allItems[index]

    { new IFeed<'T> with

        member _.FeedName = name

        member _.Init(context) =
            _allItems <- csvReader.GetRecords<'T>() |> Seq.toArray
            Task.CompletedTask

        member _.GetNextItem(scenarioInfo, stepData) = getRandomItem() }

let create<'T> (name, feedData: BatchableFeedData) =
    match feedData.feedStrategy, feedData.loadingMode with
    | Circular, Batch batchSize -> CircularBatchedSeparatedValuesFeeder<'T>(name, feedData.filePath, batchSize)
    | Random, Batch batchSize -> RandomBatchedSeparatedValuesFeeder<'T>(name, feedData.filePath, batchSize)
    | Circular, Eager -> CircularEagerSeparatedValuesFeeder<'T>(name, feedData.filePath)
    | Random, Eager -> RandomEagerSeparatedValuesFeeder<'T>(name, feedData.filePath)

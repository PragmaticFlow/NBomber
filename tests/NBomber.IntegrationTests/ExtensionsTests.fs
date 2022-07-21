module Tests.Extensions

open System
open System.Diagnostics
open System.Threading

open Xunit
open Swensen.Unquote
open FsCheck
open FsCheck.Xunit
open FsToolkit.ErrorHandling

open NBomber
open NBomber.Extensions.Internal

[<Fact>]
let ``String concatWithComma should concat strings with comma`` () =
    Assert.Equal("foo, bar, baz", ["foo"; "bar"; "baz"] |> String.concatWithComma)
    Assert.Equal("foo", ["foo"] |> String.concatWithComma)
    Assert.Equal("", [] |> String.concatWithComma)

[<Fact>]
let ``Operation retry should work correctly`` () =

    let retryCount = 5
    let mutable counter1 = 0
    let mutable counter2 = 0

    // simulate Ok case
    let result1 = Operation.retry(retryCount, fun () -> backgroundTask {
        if counter1 < 3 then
            counter1 <- counter1 + 1
            return Error ""
        else
            return Ok()
    })
    result1.Wait()

    // simulate Error case
    let result2 = Operation.retry(retryCount, fun () -> backgroundTask {
        counter2 <- counter2 + 1
        return Error ""
    })
    result2.Wait()

    test <@ Result.isOk result1.Result @>
    test <@ Result.isError result2.Result @>
    test <@ counter1 = 3 @>
    test <@ counter2 = 5 @>

[<Fact>]
let ``Operation retryDuring should work correctly`` () =

    let duration = seconds 5
    let retryDelay = milliseconds 100
    let mutable counter1 = 0
    let stopwatch = Stopwatch()

    // simulate Ok case
    stopwatch.Start()
    let result1 = Operation.retryDuring(duration, retryDelay, fun () -> backgroundTask {
        if counter1 < 3 then
            counter1 <- counter1 + 1
            return Error ""
        else
            return Ok()
    })
    result1.Wait()
    stopwatch.Stop()
    let stw1Time = stopwatch.Elapsed // should be < 2sec

    // simulate Error case
    stopwatch.Start()
    let result2 = Operation.retryDuring(duration, retryDelay, fun () -> backgroundTask {
        return Error ""
    })
    result2.Wait()
    stopwatch.Stop()
    let stw2Time = stopwatch.Elapsed // should be ~= 5sec

    test <@ Result.isOk result1.Result @>
    test <@ Result.isError result2.Result @>
    test <@ stw1Time < seconds 2 @>
    test <@ stw2Time >= seconds 5 @>

module NBomber.IntegrationTests.Reporting.TextReportTests

open System
open Xunit
open Swensen.Unquote
open NBomber.Domain
open NBomber.Contracts
open NBomber.DomainServices.Reporting

let goldTxt =
    """scenario: 'test scenario 1', duration: '00:00:01', RPS: '2046', concurrent Copies: '50'
+----------------------------+------------------------------------------------+
| step                       | details                                        |
+----------------------------+------------------------------------------------+
| - name                     | ok step no data                                |
+----------------------------+------------------------------------------------+
| - request count            | all = 2046 | OK = 2046 | failed = 0            |
+----------------------------+------------------------------------------------+
| - response time            | RPS = 2046 | min = 9 | mean = 21 | max = 64    |
+----------------------------+------------------------------------------------+
| - response time percentile | 50% = 21 | 75% = 26 | 95% = 36 | StdDev = 9    |
+----------------------------+------------------------------------------------+
|                            |                                                |
+----------------------------+------------------------------------------------+
| - name                     | ok step with data                              |
+----------------------------+------------------------------------------------+
| - request count            | all = 2046 | OK = 2046 | failed = 0            |
+----------------------------+------------------------------------------------+
| - response time            | RPS = 2046 | min = 9 | mean = 21 | max = 64    |
+----------------------------+------------------------------------------------+
| - response time percentile | 50% = 21 | 75% = 26 | 95% = 36 | StdDev = 9    |
+----------------------------+------------------------------------------------+
| - data transfer            | min = 1Kb | mean = 2Kb | max = 3Kb | all = 4MB |
+----------------------------+------------------------------------------------+

scenario: 'test scenario 2', duration: '00:00:01', RPS: '2046', concurrent Copies: '50'
+----------------------------+------------------------------------------------+
| step                       | details                                        |
+----------------------------+------------------------------------------------+
| - name                     | ok step no data                                |
+----------------------------+------------------------------------------------+
| - request count            | all = 2046 | OK = 2046 | failed = 0            |
+----------------------------+------------------------------------------------+
| - response time            | RPS = 2046 | min = 9 | mean = 21 | max = 64    |
+----------------------------+------------------------------------------------+
| - response time percentile | 50% = 21 | 75% = 26 | 95% = 36 | StdDev = 9    |
+----------------------------+------------------------------------------------+
|                            |                                                |
+----------------------------+------------------------------------------------+
| - name                     | ok step with data                              |
+----------------------------+------------------------------------------------+
| - request count            | all = 2046 | OK = 2046 | failed = 0            |
+----------------------------+------------------------------------------------+
| - response time            | RPS = 2046 | min = 9 | mean = 21 | max = 64    |
+----------------------------+------------------------------------------------+
| - response time percentile | 50% = 21 | 75% = 26 | 95% = 36 | StdDev = 9    |
+----------------------------+------------------------------------------------+
| - data transfer            | min = 1Kb | mean = 2Kb | max = 3Kb | all = 4MB |
+----------------------------+------------------------------------------------+
"""

let goldMD =
    """# Scenario: `test scenario 1`

- Duration: `00:00:01`
- RPS: `2046`
- Concurrent Copies: `50`

| __step__                 | __details__                                                             |
|--------------------------|-------------------------------------------------------------------------|
| name                     | `ok step no data`                                                       |
| request count            | all = `2046`, OK = `2046`, failed = `0`                                 |
| response time            | RPS = `2046`, min = `9`, mean = `21`, max = `64`                        |
| response time percentile | 50% = `21`, 75% = `26`, 95% = `36`, StdDev = `9`                        |
| name                     | `ok step with data`                                                     |
| request count            | all = `2046`, OK = `2046`, failed = `0`                                 |
| response time            | RPS = `2046`, min = `9`, mean = `21`, max = `64`                        |
| response time percentile | 50% = `21`, 75% = `26`, 95% = `36`, StdDev = `9`                        |
| data transfer            | min = `1.000 Kb`, mean = `2.000 Kb`, max = `3.000 Kb`, all = `4.000 MB` |

# Scenario: `test scenario 2`

- Duration: `00:00:01`
- RPS: `2046`
- Concurrent Copies: `50`

| __step__                 | __details__                                                             |
|--------------------------|-------------------------------------------------------------------------|
| name                     | `ok step no data`                                                       |
| request count            | all = `2046`, OK = `2046`, failed = `0`                                 |
| response time            | RPS = `2046`, min = `9`, mean = `21`, max = `64`                        |
| response time percentile | 50% = `21`, 75% = `26`, 95% = `36`, StdDev = `9`                        |
| name                     | `ok step with data`                                                     |
| request count            | all = `2046`, OK = `2046`, failed = `0`                                 |
| response time            | RPS = `2046`, min = `9`, mean = `21`, max = `64`                        |
| response time percentile | 50% = `21`, 75% = `26`, 95% = `36`, StdDev = `9`                        |
| data transfer            | min = `1.000 Kb`, mean = `2.000 Kb`, max = `3.000 Kb`, all = `4.000 MB` |
"""

let private noData =
    { MinKb  = 0.0
      MeanKb = 0.0
      MaxKb  = 0.0
      AllMB  = 0.0 }
let private someData =
    { MinKb  = 1.0
      MeanKb = 2.0
      MaxKb  = 3.0
      AllMB  = 4.0 }
let private stepStats =
    { StepName = "ok step no data"
      OkLatencies = [| 37; 10; 21; 26; 16; 16; 17; 27; 11; 15; 12; 17; 11; 17; 47; 33; 21 |]
      ReqeustCount = 2046
      OkCount = 2046
      FailCount = 0
      RPS = 2046
      Min = 9
      Mean = 21
      Max = 64
      Percent50 = 21
      Percent75 = 26
      Percent95 = 36
      StdDev = 9
      DataTransfer = noData }

let private scenario1stats =
    { ScenarioName = "test scenario 1"
      StepsStats =
           [| stepStats
              { stepStats with
                    StepName = "ok step with data"
                    DataTransfer = someData }
           |]
      RPS = 2046
      ConcurrentCopies = 50
      OkCount = 2046
      FailCount = 0
      LatencyCount =
         { Less800 = 2046
           More800Less1200 = 0
           More1200 = 0 }
      Duration = TimeSpan.FromSeconds 1.0 }

let private stats =
    { AllScenariosStats =
          [| scenario1stats
             { scenario1stats with ScenarioName = "test scenario 2" }
          |]
      OkCount = 2046
      FailCount = 0
      LatencyCount = { Less800 = 2046
                       More800Less1200 = 0
                       More1200 = 0 }
      NodeStatsInfo = { MachineName = "unit test machine"
                        Sender = SingleNode
                        CurrentOperation = NodeOperationType.Complete }
    }

[<Fact>]
let ``Markdown report test``() =

    let actualMd = MdReport.print stats
    test <@ actualMd = goldMD @>

[<Fact>]
let ``Text report test``() =

    let actualTxt = TxtReport.print stats
    test <@ actualTxt = goldTxt @>


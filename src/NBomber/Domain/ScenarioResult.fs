namespace NBomber.Domain

open NBomber.Contracts
open NBomber.Domain.DomainTypes

module internal ScenarioResult =
 
    let getAssertionResult(vr: ValidationResult) = 
        let assertionStatus = match vr.Result with
                              | Ok _ -> AssertionStatus.Success
                              | Error(AssertionError _) -> AssertionStatus.Fail
                              | Error(_) -> AssertionStatus.Fatal
                              
        { AssertionId = vr.Number; AssertionStatus = assertionStatus }
    
    let getStepResult(svr: StepValidationResult) =
        { StepName = svr.StepName; Stats = svr.Stats; AssertionsResults = svr.ValidationResults |> Array.map(getAssertionResult)}
    
    let create(validationResult: ScenarioValidationResult) = 
        { 
            ScenarioName = validationResult.ScenarioName
            StepStats = validationResult.StepResults |> Array.map(getStepResult) 
        }
module internal NBomber.DomainServices.Validation

open NBomber.Contracts

let validateRunnerContext(context: NBomberRunnerContext) = 
    // check on same scenario name
    // check on same step name within scenario
    Ok(context)
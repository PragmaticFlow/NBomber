[<AutoOpen>]
module internal rec Extensions

module String =

    let replace (oldValue: string, newValue: string) (str: string) =
        str.Replace(oldValue, newValue)
        


namespace ContactManagement

open System

module SearchService =
    let searchContacts (query: string) (contacts: Map<string, Contact>) : ContactSearchResult =
        // Exact match by name
        let exactMatch = 
            contacts
            |> Map.tryFind query // Directly find by name

        match exactMatch with
        | Some contact -> Exact contact
        | None -> 
            // Partial matches
            let partialMatches = 
                contacts 
                |> Map.filter (fun _ contact -> 
                    contact.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                    contact.Email.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                    contact.PhoneNumber.Contains(query)
                )
                |> Map.values
                |> Seq.toList

            match partialMatches with
            | [] -> NoMatch  // No matches
            | matches -> Partial matches


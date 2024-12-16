namespace ContactManagement

open System
open System.IO
open Newtonsoft.Json
open DataTypes

// FileManager.fs
module FileManager =

    // Define the file path where the contacts data will be stored
    let private contactsFilePath = "contacts.json"
    
    // Function to load contacts from a JSON file and return a Map<string, Contact>
    let loadContacts () : Map<string, Contact> =
        try
            // Check if the file exists
            if File.Exists(contactsFilePath) then
                // Read the file content
                let json = File.ReadAllText(contactsFilePath)
                
                // Deserialize the JSON content to a Map<string, Contact>
                JsonConvert.DeserializeObject<Map<string, Contact>>(json)
            else
                // Return an empty Map if the file doesn't exist
                Map.empty
        with
        | ex -> 
            // Handle exceptions and return an empty Map in case of an error
            printfn "Error loading contacts: %s" ex.Message
            Map.empty
    
    // Function to save contacts to a JSON file
    let saveContacts (contacts: Map<string, Contact>) =
        try
            // Serialize the contacts Map to JSON
            let json = JsonConvert.SerializeObject(contacts, Formatting.Indented)
            
            // Write the JSON to the file
            File.WriteAllText(contactsFilePath, json)
        with
        | ex -> 
            // Handle exceptions during saving
            printfn "Error saving contacts: %s" ex.Message

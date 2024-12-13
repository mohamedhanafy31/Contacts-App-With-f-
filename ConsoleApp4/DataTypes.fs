namespace ContactManagement

open System.Drawing

module DataTypes =

    // Immutable domain model for Contact
    type Contact = {
        Name: string
        PhoneNumber: string
        Email: string
        Color: string
    }

    // Discriminated union for search results
    type ContactSearchResult = 
        | Exact of Contact
        | Partial of Contact list
        | NoMatch

    type AppState = {
        Contacts: Map<string, Contact>
        SearchQuery: string option
    }

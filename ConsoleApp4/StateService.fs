namespace ContactManagement

open DataTypes

module StateService =
    let renameContact (oldName: string) (newName: string) (updatedContact: Contact) (state: AppState) : Result<AppState, string> =
        if not (state.Contacts.ContainsKey oldName) then
            Error "Original contact not found"
        elif state.Contacts.ContainsKey newName then
            Error "A contact with the new name already exists"
        else
            let updatedContacts = 
                state.Contacts
                |> Map.remove oldName
                |> Map.add newName updatedContact
            Ok { state with Contacts = updatedContacts }

    let updateContact (name: string) (updatedContact: Contact) (state: AppState) : Result<AppState, string> =
        if not (state.Contacts.ContainsKey name) then
            Error "Contact not found"
        else
            let existingContact = state.Contacts.[name]
            if existingContact.Name <> updatedContact.Name then
                // Call renameContact if the name has changed
                renameContact name updatedContact.Name updatedContact state
            else
                // Update the contact directly if the name hasn't changed
                Ok { state with Contacts = state.Contacts.Add(name, updatedContact) }



    let addContact (contact: Contact) (state: AppState) : Result<AppState, string> =
        if Validation.isValidContact contact then
            if state.Contacts.ContainsKey contact.Name then
                Error "A contact with this name already exists"
            else
                Ok { state with Contacts = state.Contacts.Add(contact.Name, contact) }
        else
            Error "Invalid contact details"


    let deleteContact (name: string) (state: AppState) : AppState =
        { state with Contacts = state.Contacts.Remove(name) }




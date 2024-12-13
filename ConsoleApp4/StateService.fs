namespace ContactManagement

module StateService =
    let updateContact (name: string) (updatedContact: Contact) (state: AppState) : Result<AppState, string> =
        if state.Contacts.ContainsKey name then
            Ok { state with Contacts = state.Contacts.Add(name, updatedContact) }
        else
            Error "Contact not found"


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


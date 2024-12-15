namespace ContactManagement

// Import necessary modules and classes
open System
open System.Windows.Forms
open System.Drawing
open System.IO

// Existing code for Domain Model, Validation, FileManager, etc.

[<RequireQualifiedAccess>]
module ContactManager =

    // Helper function to create a labeled text box
    let createLabeledTextBox label top =
        let itemLabel = new Label(Text = label, Top = top, Left = 10, Width = 100)
        let itemTextBox = new TextBox(Top = top + 20, Left = 10, Width = 200)
        (itemLabel :> Control, itemTextBox :> Control)

    // Function to show contact details
    let showContactCard (stateRef: ref<AppState>) (contact: Contact) (refreshPanel: unit -> unit) =
        let cardForm = 
            new Form(
                Text = contact.Name, 
                Width = 300, 
                Height = 350, 
                BackColor = ColorUtilities.hexToColor(contact.Color)
            )
        cardForm.StartPosition <- FormStartPosition.CenterParent
        
        let textColor = 
            Validation.getTextColor (ColorUtilities.hexToColor(contact.Color))
        
        let createLabel text top = 
            new Label(
                Text = text, 
                Top = top, 
                Left = 10, 
                Width = 280, 
                ForeColor = textColor
            )
        
        let nameLabel = createLabel $"Name: {contact.Name}" 10
        let phoneLabel = createLabel $"Phone: {contact.PhoneNumber}" 40
        let emailLabel = createLabel $"Email: {contact.Email}" 70
        
        [nameLabel; phoneLabel; emailLabel] 
        |> List.iter cardForm.Controls.Add
        
        // Edit and delete buttons
        let editButton = 
            new Button(
                Text = "Edit", 
                Top = 250, 
                Left = 110, 
                Width = 80
            )
        
        // Handle editing a contact
        editButton.Click.Add (fun _ ->
            let editForm = 
                new Form(
                    Text = "Edit Contact", 
                    Width = 400, 
                    Height = 350, 
                    StartPosition = FormStartPosition.CenterParent
                )

            let (nameLabel, nameTextBox) = 
                createLabeledTextBox "Name" 10
            let (phoneLabel, phoneTextBox) = 
                createLabeledTextBox "Phone (Digits Only)" 70
            let (emailLabel, emailTextBox) = 
                createLabeledTextBox "Email" 130

            nameTextBox.Text <- contact.Name
            phoneTextBox.Text <- contact.PhoneNumber
            emailTextBox.Text <- contact.Email
            
            let colorPicker = 
                new Button(
                    Text = "Choose Color", 
                    Top = 190, 
                    Left = 10, 
                    Width = 200
                )
            
            let mutable selectedColor = ColorUtilities.hexToColor(contact.Color)
            
            colorPicker.Click.Add (fun _ ->
                let colorDialog = new ColorDialog()
                if colorDialog.ShowDialog() = DialogResult.OK then
                    selectedColor <- colorDialog.Color
                    colorPicker.BackColor <- selectedColor
            )
            
            let saveButton = 
                new Button(
                    Text = "Save", 
                    Top = 230, 
                    Left = 10, 
                    Width = 200
                )
            
            // Add controls to edit form
            editForm.Controls.AddRange([|
                nameLabel :> Control; 
                nameTextBox :> Control; 
                phoneLabel :> Control; 
                phoneTextBox :> Control; 
                emailLabel :> Control; 
                emailTextBox :> Control; 
                colorPicker :> Control; 
                saveButton :> Control
            |])
            
            editForm.ShowDialog() |> ignore
        )
        
        let deleteButton = 
            new Button(
                Text = "Delete", 
                Top = 250, 
                Left = 200, 
                Width = 80, 
                BackColor = Color.LightCoral
            )
        
        // Handle deleting a contact
        deleteButton.Click.Add (fun _ ->
            let result = 
                MessageBox.Show(
                    $"Are you sure you want to delete {contact.Name}?", 
                    "Confirm Delete", 
                    MessageBoxButtons.YesNo, 
                    MessageBoxIcon.Warning
                )
            
            if result = DialogResult.Yes then
                let updatedState = StateService.deleteContact contact.Name (!stateRef)
                // Not implemented yet
        )
        
        [editButton; deleteButton] 
        |> List.iter cardForm.Controls.Add
        
        cardForm.ShowDialog() |> ignore

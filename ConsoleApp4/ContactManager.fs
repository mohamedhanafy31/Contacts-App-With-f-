namespace ContactManagement

// Import necessary modules and classes
open System
open System.Text.RegularExpressions
open System.Windows.Forms
open System.Drawing
open System.IO
open System.Text.Json


[<RequireQualifiedAccess>]
module ContactManager =
    
    // Helper function to create a labeled text box
    let createLabeledTextBox label top =
        let boldFont = new Font("Arial", 12.0f, FontStyle.Bold)
        let itemLabel = new Label(Text = label, Top = top, Left = 10, Width = 100,Font = boldFont)
        itemLabel.Font <- new Font("Segoe UI", 14.0f, FontStyle.Regular)

        let itemTextBox = new TextBox(Top = top + 20, Left = 10, Width = 300)
        itemTextBox.Font <- new Font("Open Sans", 12.0f, FontStyle.Regular)
        
        (itemLabel :> Control, itemTextBox :> Control)

    // Function to show contact details
    let showContactCard (stateRef: ref<AppState>) (contact: Contact) (refreshPanel: unit -> unit) =
        let cardForm = 
            new Form(
                Text = contact.Name, 
                Width = 500, 
                Height = 350, 
                BackColor = ColorUtilities.hexToColor(contact.Color)
            )
        cardForm.StartPosition <- FormStartPosition.CenterParent
        
        let textColor = 
            ColorUtilities.getTextColor (ColorUtilities.hexToColor(contact.Color))
        
        let createLabel text top = 
            let label = new Label(Text = text, Top = top, Left = 10, Width = 350, ForeColor = textColor)
            label.Font <- new Font("Segoe UI", 12.0f, FontStyle.Regular)
            label
        
        let nameLabel = createLabel $"Name: {contact.Name}" 10
        let phoneLabel = createLabel $"Phone: {contact.PhoneNumber}" 40
        let emailLabel = createLabel $"Email: {contact.Email}" 70
        
        [nameLabel; phoneLabel; emailLabel] 
        |> List.iter cardForm.Controls.Add
        
        // Edit button
        let editButton = 
            new Button(Text = "Edit", Top = 250, Left = 130, Width = 80, Height = 40, ForeColor = textColor)
        editButton.Font <- new Font("Segoe UI", 14.0f, FontStyle.Bold)

        editButton.Click.Add (fun _ ->
            let editForm = 
                new Form(
                    Text = "Edit Contact", 
                    Width = 400, 
                    Height = 350, 
                    StartPosition = FormStartPosition.CenterParent
                )

            let (nameLabel, nameTextBox) = createLabeledTextBox "Name" 10
            let (phoneLabel, phoneTextBox) = createLabeledTextBox "Phone (Digits Only)" 70
            let (emailLabel, emailTextBox) = createLabeledTextBox "Email" 130

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
            
            let colorPicker = 
                new Button(
                    Text = "Choose Color", 
                    Top = 210, 
                    Left = 10, 
                    Width = 200,
                    Height = 40
                )
                        
            colorPicker.Click.Add (fun _ ->
                let colorDialog = new ColorDialog()
                if colorDialog.ShowDialog() = DialogResult.OK then
                    selectedColor <- colorDialog.Color
                    colorPicker.BackColor <- selectedColor
            )
            
            let saveButton = 
                new Button(
                    Text = "\U0000270F", 
                    Top = 260, 
                    Left = 10, 
                    Width = 200,
                    Height = 40,
                    BackColor = Color.LightGreen
                )

            saveButton.Click.Add (fun _ ->
                let newContact = {
                    Name = nameTextBox.Text
                    PhoneNumber = phoneTextBox.Text
                    Email = emailTextBox.Text
                    Color = ColorUtilities.colorToHex(selectedColor)
                }
                
                match StateService.updateContact contact.Name newContact (!stateRef) with
                | Ok newState ->
                    stateRef := newState
                    FileManager.saveContacts newState.Contacts
                    refreshPanel()
                    editForm.Close()
                    cardForm.Close()
                | Error msg ->
                    MessageBox.Show(msg) |> ignore
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

        // Delete button
        let deleteButton = 
            new Button(Text = "Delete", Top = 250, Left = 250,  Width = 80, Height = 40 , BackColor = Color.Red)
        deleteButton.Font <- new Font("Segoe UI", 14.0f, FontStyle.Bold)

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
                stateRef := updatedState
                FileManager.saveContacts updatedState.Contacts
                refreshPanel()
                cardForm.Close()
        )

        [editButton; deleteButton] |> List.iter cardForm.Controls.Add
        cardForm.ShowDialog() |> ignore
    let createMainForm () =
        let form = 
            new Form(
                Text = "Functional Contact Management", 
                Width = 600, 
                Height = 600,
                Font = new Font("Segoe UI", 10.0f)
            )
        
        let layout = 
            new TableLayoutPanel(
                Dock = DockStyle.Top, 
                RowCount = 2, 
                ColumnCount = 3
            )
        
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20.0f))
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50.0f))
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30.0f))
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 50.0f))
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 50.0f))
        
        let addButton = 
            new Button(
                Text = "+", 
                Width = 50, 
                Height = 10, 
                Dock = DockStyle.Right, 
                Font = new Font("Segoe UI", 16.0f, FontStyle.Bold),
                BackColor = Color.LightBlue
            )
        
        let searchTextBox = 
            new TextBox(
                Width = 200, 
                Height = 30,
                Anchor = AnchorStyles.None,
                Font = new Font("Segoe UI", 10.0f)
            )
        
        let searchButton = 
            new Button(
                Text = "\U0001F50D", 
                Width = 30,
                Height = 30,
                Font = new Font("Segoe UI", 10.0f)
            )
        
        let searchPanel = 
            new FlowLayoutPanel(
                Dock = DockStyle.Fill, 
                AutoSize = true, 
                FlowDirection = FlowDirection.LeftToRight, 
                Margin = Padding(0, 20, 0, 0)
            )
        
        searchPanel.Controls.Add(searchTextBox)
        searchPanel.Controls.Add(searchButton)
        
        layout.Controls.Add(new Label(), 0, 0)
        layout.Controls.Add(searchPanel, 1, 0)
        layout.Controls.Add(addButton, 2, 0)
        
        let contactPanel = 
            new FlowLayoutPanel(
                Dock = DockStyle.Fill, 
                AutoScroll = true, 
                Padding = Padding(10)
            )
        
        let initialState = {
            Contacts = FileManager.loadContacts()
            SearchQuery = None
        }
        
        let state = ref initialState

        // Function to refresh contact panel
        let rec refreshContactPanel (searchQuery: string option) =
            contactPanel.Controls.Clear()
            
            let displayContacts = 
                match searchQuery with
                | Some query -> 
                    match SearchService.searchContacts query state.Value.Contacts with
                    | Exact contact -> [contact.Name, contact]
                    | Partial contacts -> 
                        contacts 
                        |> List.map (fun contact -> contact.Name, contact)
                    | NoMatch -> []
                | None -> 
                    state.Value.Contacts 
                    |> Map.toList
            
            displayContacts 
            |> List.iter (fun (_, contact) ->
                let textColor = 
                    ColorUtilities.getTextColor (ColorUtilities.hexToColor(contact.Color))
                
                let button = 
                    new Button(
                        Text = contact.Name, 
                        Width = 200, 
                        Height = 30, 
                        BackColor = ColorUtilities.hexToColor(contact.Color), 
                        ForeColor = textColor,
                        Font = new Font("Segoe UI", 10.0f, FontStyle.Bold)
                    )
                
                button.Click.Add(fun _ -> 
                    showContactCard state contact (fun () -> refreshContactPanel None)
                )
                
                contactPanel.Controls.Add(button)
            )
        
        // Search button functionality
        searchButton.Click.Add (fun _ ->
            let query = searchTextBox.Text
            refreshContactPanel (if String.IsNullOrWhiteSpace(query) then None else Some query)
        )
        
        // Add button functionality
        addButton.Click.Add (fun _ ->
            let popupForm = 
                new Form(
                    Text = "Add Contact", 
                    Width = 350, 
                    Height = 350, 
                    StartPosition = FormStartPosition.CenterParent
                )
            
            let (nameLabel, nameTextBox) = createLabeledTextBox "Name" 10
            let (phoneLabel, phoneTextBox) = createLabeledTextBox "Phone" 70
            let (emailLabel, emailTextBox) = createLabeledTextBox "Email" 130

            let colorPicker = 
                new Button(
                    Text = "Choose Color", 
                    Top = 210, 
                    Left = 10, 
                    Width = 200,
                    Height = 40
                )
            
            let mutable selectedColor = Color.LightGray
            
            colorPicker.Click.Add (fun _ ->
                let colorDialog = new ColorDialog()
                if colorDialog.ShowDialog() = DialogResult.OK then
                    selectedColor <- colorDialog.Color
                    colorPicker.BackColor <- selectedColor
            )
            
            let saveButton = 
                new Button(
                    Text = "\U0000270F", 
                    Top = 260, 
                    Left = 10, 
                    Width = 200,
                    Height = 40,
                    BackColor = Color.LightGreen
                )

            colorPicker.Font <- new Font("Segoe UI", 14.0f, FontStyle.Bold)
            saveButton.Font <- new Font("Segoe UI", 14.0f, FontStyle.Bold)
            
            saveButton.Click.Add (fun _ ->
                let newContact = {
                    Name = nameTextBox.Text
                    PhoneNumber = phoneTextBox.Text
                    Email = emailTextBox.Text
                    Color = ColorUtilities.colorToHex(selectedColor)
                }
                
                match StateService.addContact newContact !state with
                | Ok newState ->
                    state := newState
                    FileManager.saveContacts newState.Contacts
                    refreshContactPanel None
                    popupForm.Close()
                | Error msg ->
                    MessageBox.Show(msg) |> ignore
            )
            
            popupForm.Controls.AddRange([|
                nameLabel :> Control; 
                nameTextBox :> Control; 
                phoneLabel :> Control; 
                phoneTextBox :> Control; 
                emailLabel :> Control; 
                emailTextBox :> Control; 
                colorPicker :> Control; 
                saveButton :> Control
            |])
            
            popupForm.ShowDialog() |> ignore
        )

        // Initial load of contacts
        refreshContactPanel None
        
        form.Controls.Add(contactPanel)
        form.Controls.Add(layout)
        
        form

namespace ContactManagement

// Import necessary modules and classes
open System
open System.Text.RegularExpressions
open System.Windows.Forms
open System.Drawing
open System.IO
open System.Text.Json

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

    // Main UI form creation and layout
    let createMainForm () =
        let form = 
            new Form(
                Text = "Functional Contact Management", 
                Width = 800, 
                Height = 600
            )
        
        // Add contact button and search panel
        let layout = 
            new TableLayoutPanel(
                Dock = DockStyle.Top, 
                RowCount = 2, 
                ColumnCount = 3
            )
        
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20.0f))
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40.0f))
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40.0f))
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 50.0f))
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 50.0f))
        
        let addButton = 
            new Button(
                Text = "+", 
                Width = 50, 
                Height = 10, 
                Dock = DockStyle.Right, 
                Font = new Font(FontFamily.GenericSansSerif, 16.0f, FontStyle.Bold),
                BackColor = Color.LightBlue
            )
        
        let searchTextBox = 
            new TextBox(
                Width = 200, 
                Anchor = AnchorStyles.None
            )
        
        let searchButton = 
            new Button(
                Text = "Search", 
                Width = 80
            )
        
        let searchPanel = 
            new FlowLayoutPanel(
                Dock = DockStyle.Fill, 
                AutoSize = true, 
                FlowDirection = FlowDirection.LeftToRight, 
                Margin = Padding(0, 20, 0, 0)
            )
        
        searchPanel.Controls.Add(searchButton)
        searchPanel.Controls.Add(searchTextBox)
        
        layout.Controls.Add(new Label(), 0, 0)
        layout.Controls.Add(searchPanel, 1, 0)
        layout.Controls.Add(addButton, 2, 0)
        
        let contactPanel = 
            new FlowLayoutPanel(
                Dock = DockStyle.Fill, 
                AutoScroll = true, 
                Padding = Padding(10)
            )
        
        // State initialization
        let initialState = {
            Contacts = FileManager.loadContacts()
            SearchQuery = None
        }
        
        let state = ref initialState
        
        // Refresh contact panel based on search query
        let rec refreshContactPanel (searchQuery: string option) =
            contactPanel.Controls.Clear()
            
            let displayContacts = 
                state.Value.Contacts 
                |> Map.toList
            
            displayContacts 
            |> List.iter (fun (_, contact) ->
                let textColor = 
                    Validation.getTextColor (ColorUtilities.hexToColor(contact.Color))
                
                let button = 
                    new Button(
                        Text = contact.Name, 
                        Width = 200, 
                        Height = 30, 
                        BackColor = ColorUtilities.hexToColor(contact.Color), 
                        ForeColor = textColor
                    )
                
                button.Click.Add(fun _ -> 
                    showContactCard state contact (fun () -> refreshContactPanel None)
                )
                
                contactPanel.Controls.Add(button)
            )
        
        // Search button click
        searchButton.Click.Add (fun _ ->
            // Not implemented yet
        )
        
        // Add button click
        addButton.Click.Add (fun _ ->
            let popupForm = 
                new Form(
                    Text = "Add Contact", 
                    Width = 400, 
                    Height = 350, 
                    StartPosition = FormStartPosition.CenterParent
                )

            let nameControls = createLabeledTextBox "Name" 10 |> fun (l, t) -> [l; t]
            let phoneControls = createLabeledTextBox "Phone (Digits Only)" 70 |> fun (l, t) -> [l; t]
            let emailControls = createLabeledTextBox "Email" 130 |> fun (l, t) -> [l; t]


            let colorPicker = 
                new Button(
                    Text = "Choose Color", 
                    Top = 190, 
                    Left = 10, 
                    Width = 200
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
                    Text = "Save", 
                    Top = 230, 
                    Left = 10, 
                    Width = 200
                )

            // Get textboxes from controls
            let getTextBox (controls: Control list) = 
                controls 
                |> List.pick (function 
                    | :? TextBox as tb -> Some tb 
                    | _ -> None)

            saveButton.Click.Add (fun _ ->
                // Not implemented yet
            )

            popupForm.ShowDialog() |> ignore
        )

        // Initial panel refresh
        refreshContactPanel None
        
        // Add components to the form
        form.Controls.Add(contactPanel)
        form.Controls.Add(layout)
        
        form

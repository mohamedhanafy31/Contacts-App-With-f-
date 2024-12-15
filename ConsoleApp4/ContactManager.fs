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

    
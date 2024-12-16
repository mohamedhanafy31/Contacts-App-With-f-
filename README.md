Functional Contact Management Application
Project Description
The Functional Contact Management Application is a desktop application built using F# and Windows Forms. It provides an intuitive interface for managing a personal or professional contact list. The application emphasizes modular design and functional programming principles, offering features like adding, editing, deleting, and searching for contacts. Contact data is stored in a JSON file, ensuring portability and simplicity.

Features
Add Contact: Create new contacts with details like name, phone number, email, and preferred color for UI representation.
Edit Contact: Update existing contact details dynamically.
Delete Contact: Remove a contact with confirmation.
Search Contacts: Quickly find contacts using full or partial name matches.
Color Coding: Assign custom colors to contacts for better organization.
Project Flow
Start the Application:

Launch the application, which loads all existing contacts from a JSON file (contacts.json).
The main UI displays a list of contacts along with a search bar and an "Add Contact" button.
Add a Contact:

Clicking the "+" button opens a form where the user can input contact details (name, phone, email, and color).
On submission, the contact is validated and saved to the JSON file.
View Contact Details:

Clicking on a contact's name in the list opens a detailed view showing their information.
Edit or Delete a Contact:

From the contact detail view, the user can choose to edit or delete the contact.
Edits are validated and saved back to the JSON file.
Deletions remove the contact from both the list and the file.
Search Contacts:

Enter a search query in the search bar to filter contacts by name.
The list dynamically updates to show only matching results.
Save and Exit:

All changes are automatically saved to the contacts.json file, ensuring data persistence.
Technical Information
Project Structure
bash
Copy code
ContactManagement/
├── src/
│   ├── DataTypes.fs          # Defines the Contact data model
│   ├── Validation.fs         # Handles input validation logic
│   ├── StateService.fs       # Manages application state (add, edit, delete operations)
│   ├── SearchService.fs      # Implements contact search functionality
│   ├── ColorUtilities.fs     # Provides utilities for color conversion and contrast
│   ├── ContactService.fs     # Core business logic for contact operations
│   ├── ContactRepository.fs  # Manages file persistence using JSON
│   ├── FileManage.fs         # Abstracts file read/write operations
│   ├── ContactManager.fs     # User interface implementation (Windows Forms)
│   ├── Program.fs            # Entry point for the application
├── contacts.json             # JSON file storing all contact data
├── ContactManagement.fsproj  # F# project file
Technologies Used
Language:

F# (Functional-first programming language)
Framework:

.NET 8.0 with Windows Forms for the graphical user interface.
Data Storage:

JSON file-based persistence using System.Text.Json and Newtonsoft.Json.
Dependencies:

Newtonsoft.Json: Advanced JSON serialization and deserialization.
System.Text.Json: Lightweight JSON handling for high performance.
Installation and Setup
Prerequisites
.NET SDK 8.0 or later
Windows Operating System (required for Windows Forms)
Steps to Set Up the Project
Clone the repository:

bash
Copy code
git clone https://github.com/your-repo/contact-management.git
cd contact-management
Build the project:

bash
Copy code
dotnet build
Run the application:

bash
Copy code
dotnet run --project src/ContactManagement.fsproj
Ensure the contacts.json file is present in the root directory for storing contact data. If not, it will be created automatically on the first run.

Design Philosophy
Separation of Concerns:

Each module focuses on a single responsibility (e.g., UI, data storage, business logic).
Functional Principles:

Emphasis on immutability and composability to ensure reliability and predictability.
Persistence:

Lightweight, file-based storage for simplicity and ease of deployment.
Extensibility:

Modular design allows for adding new features or replacing storage mechanisms with minimal effort.
Future Enhancements
Cloud Storage: Integrate with cloud-based solutions for data synchronization across devices.
Advanced Search: Add filters for phone numbers, emails, or other attributes.
Export/Import: Support exporting contacts to CSV or importing from other formats.
Multi-Platform Support: Expand to Linux and macOS.
Acknowledgments
This project leverages the power of functional programming in F# to build a robust and maintainable desktop application.
Inspired by the simplicity of tools like phonebook managers and the need for a customizable solution.
For issues, suggestions, or contributions, feel free to open a pull request or issue on the repository.

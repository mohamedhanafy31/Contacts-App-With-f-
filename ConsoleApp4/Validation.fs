namespace ContactManagement

open System
open System.Text.RegularExpressions
open System.Drawing

module Validation =

    let isUniqueName (name: string) (contacts: Map<string, Contact>) =
        not (Map.containsKey name contacts)

    // Pure validation functions
    let isValidPhoneNumber (phone: string) =
        Regex.IsMatch(phone, @"^\d{7,15}$") // Restrict to 7-15 digits

    let isValidEmail (email: string) =
        Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$")

    let isValidContact (contact: Contact) =
        not (String.IsNullOrWhiteSpace(contact.Name)) &&
        isValidPhoneNumber contact.PhoneNumber &&
        isValidEmail contact.Email


module Validation

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

    // Shared luminance calculation
    let calculateLuminance (color: Color) =
        (0.299 * float color.R + 0.587 * float color.G + 0.114 * float color.B) / 255.0

    let getTextColor (bgColor: Color) =
        if calculateLuminance bgColor > 0.5 then Color.Black else Color.White

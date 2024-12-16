namespace ContactManagement

open System.Drawing
open System
module ColorUtilities =
    
    // Shared luminance calculation
    let calculateLuminance (color: Color) =
        (0.299 * float color.R + 0.587 * float color.G + 0.114 * float color.B) / 255.0

    let getTextColor (bgColor: Color) =
        if calculateLuminance bgColor > 0.5 then Color.Black else Color.White

    let getContrastColor (bgColor: Color) =
        let luminance = calculateLuminance bgColor
        if luminance > 0.5 then Color.Black else Color.White

    let colorToHex (color: Color) =
        System.Drawing.ColorTranslator.ToHtml(color)

    let hexToColor (hex: string) =
        try
            System.Drawing.ColorTranslator.FromHtml(hex)
        with
        | :? ArgumentException -> failwith "Invalid color hex string"



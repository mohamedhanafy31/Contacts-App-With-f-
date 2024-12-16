namespace ContactManagement

open System.Drawing
open System
open DataTypes

module ColorUtilities =
    let colorToHex (color: Color) =
        System.Drawing.ColorTranslator.ToHtml(color)

    let hexToColor (hex: string) =
        try
            System.Drawing.ColorTranslator.FromHtml(hex)
        with
        | :? ArgumentException -> failwith "Invalid color hex string"

    let getContrastColor (bgColor: Color) =
        let luminance = Validation.calculateLuminance bgColor
        if luminance > 0.5 then Color.Black else Color.White

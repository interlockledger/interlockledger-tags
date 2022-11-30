// ******************************************************************************************************************************
//
// Copyright (c) 2018-2021 InterlockLedger Network
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met
//
// * Redistributions of source code must retain the above copyright notice, this
//   list of conditions and the following disclaimer.
//
// * Redistributions in binary form must reproduce the above copyright notice,
//   this list of conditions and the following disclaimer in the documentation
//   and/or other materials provided with the distribution.
//
// * Neither the name of the copyright holder nor the names of its
//   contributors may be used to endorse or promote products derived from
//   this software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES, LOSS OF USE, DATA, OR PROFITS, OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
// ******************************************************************************************************************************

#nullable enable

namespace InterlockLedger.Tags;

[TypeConverter(typeof(TypeCustomConverter<InterlockColor>))]
[JsonConverter(typeof(JsonCustomConverter<InterlockColor>))]
#pragma warning disable CA2231 // Overload operator equals on overriding value type Equals
public partial struct InterlockColor : ITextual<InterlockColor>
{
#pragma warning restore CA2231 // Overload operator equals on overriding value type Equals
    public static readonly InterlockColor AliceBlue = new(240, 248, 255, "AliceBlue");
    public static readonly InterlockColor AntiqueWhite = new(250, 235, 215, "AntiqueWhite");
    public static readonly InterlockColor Aqua = new(0, 255, 255, "Aqua");
    public static readonly InterlockColor Aquamarine = new(127, 255, 212, "Aquamarine");
    public static readonly InterlockColor Azure = new(240, 255, 255, "Azure");
    public static readonly InterlockColor Beige = new(245, 245, 220, "Beige");
    public static readonly InterlockColor Bisque = new(255, 228, 196, "Bisque");
    public static readonly InterlockColor Black = new(0, 0, 0, "Black");
    public static readonly InterlockColor BlanchedAlmond = new(255, 235, 205, "BlanchedAlmond");
    public static readonly InterlockColor Blue = new(0, 0, 255, "Blue");
    public static readonly InterlockColor BlueViolet = new(138, 43, 226, "BlueViolet");
    public static readonly InterlockColor Brown = new(165, 42, 42, "Brown");
    public static readonly InterlockColor BurlyWood = new(222, 184, 135, "BurlyWood");
    public static readonly InterlockColor CadetBlue = new(95, 158, 160, "CadetBlue");
    public static readonly InterlockColor Chartreuse = new(127, 255, 0, "Chartreuse");
    public static readonly InterlockColor Chocolate = new(210, 105, 30, "Chocolate");
    public static readonly InterlockColor Coral = new(255, 127, 80, "Coral");
    public static readonly InterlockColor CornflowerBlue = new(100, 149, 237, "CornflowerBlue");
    public static readonly InterlockColor Cornsilk = new(255, 248, 220, "Cornsilk");
    public static readonly InterlockColor Crimson = new(220, 20, 60, "Crimson");
    public static readonly InterlockColor DarkBlue = new(0, 0, 139, "DarkBlue");
    public static readonly InterlockColor DarkCyan = new(0, 139, 139, "DarkCyan");
    public static readonly InterlockColor DarkGoldenrod = new(184, 134, 11, "DarkGoldenrod");
    public static readonly InterlockColor DarkGray = new(169, 169, 169, "DarkGray");
    public static readonly InterlockColor DarkGreen = new(0, 100, 0, "DarkGreen");
    public static readonly InterlockColor DarkKhaki = new(189, 183, 107, "DarkKhaki");
    public static readonly InterlockColor DarkMagenta = new(139, 0, 139, "DarkMagenta");
    public static readonly InterlockColor DarkOliveGreen = new(85, 107, 47, "DarkOliveGreen");
    public static readonly InterlockColor DarkOrange = new(255, 140, 0, "DarkOrange");
    public static readonly InterlockColor DarkOrchid = new(153, 50, 204, "DarkOrchid");
    public static readonly InterlockColor DarkRed = new(139, 0, 0, "DarkRed");
    public static readonly InterlockColor DarkSalmon = new(233, 150, 122, "DarkSalmon");
    public static readonly InterlockColor DarkSeaGreen = new(143, 188, 139, "DarkSeaGreen");
    public static readonly InterlockColor DarkSlateBlue = new(72, 61, 139, "DarkSlateBlue");
    public static readonly InterlockColor DarkSlateGray = new(47, 79, 79, "DarkSlateGray");
    public static readonly InterlockColor DarkTurquoise = new(0, 206, 209, "DarkTurquoise");
    public static readonly InterlockColor DarkViolet = new(148, 0, 211, "DarkViolet");
    public static readonly InterlockColor DeepPink = new(255, 20, 147, "DeepPink");
    public static readonly InterlockColor DeepSkyBlue = new(0, 191, 255, "DeepSkyBlue");
    public static readonly InterlockColor DimGray = new(105, 105, 105, "DimGray");
    public static readonly InterlockColor DodgerBlue = new(30, 144, 255, "DodgerBlue");
    public static readonly InterlockColor Firebrick = new(178, 34, 34, "Firebrick");
    public static readonly InterlockColor FloralWhite = new(255, 250, 240, "FloralWhite");
    public static readonly InterlockColor ForestGreen = new(34, 139, 34, "ForestGreen");
    public static readonly InterlockColor Fuchsia = new(255, 0, 255, "Fuchsia");
    public static readonly InterlockColor Gainsboro = new(220, 220, 220, "Gainsboro");
    public static readonly InterlockColor GhostWhite = new(248, 248, 255, "GhostWhite");
    public static readonly InterlockColor Gold = new(255, 215, 0, "Gold");
    public static readonly InterlockColor Goldenrod = new(218, 165, 32, "Goldenrod");
    public static readonly InterlockColor Gray = new(128, 128, 128, "Gray");
    public static readonly InterlockColor Green = new(0, 128, 0, "Green");
    public static readonly InterlockColor GreenYellow = new(173, 255, 47, "GreenYellow");
    public static readonly InterlockColor Honeydew = new(240, 255, 240, "Honeydew");
    public static readonly InterlockColor HotPink = new(255, 105, 180, "HotPink");
    public static readonly InterlockColor IndianRed = new(205, 92, 92, "IndianRed");
    public static readonly InterlockColor Indigo = new(75, 0, 130, "Indigo");
    public static readonly InterlockColor Ivory = new(255, 255, 240, "Ivory");
    public static readonly InterlockColor Khaki = new(240, 230, 140, "Khaki");
    public static readonly InterlockColor Lavender = new(230, 230, 250, "Lavender");
    public static readonly InterlockColor LavenderBlush = new(255, 240, 245, "LavenderBlush");
    public static readonly InterlockColor LawnGreen = new(124, 252, 0, "LawnGreen");
    public static readonly InterlockColor LemonChiffon = new(255, 250, 205, "LemonChiffon");
    public static readonly InterlockColor LightBlue = new(173, 216, 230, "LightBlue");
    public static readonly InterlockColor LightCoral = new(240, 128, 128, "LightCoral");
    public static readonly InterlockColor LightCyan = new(224, 255, 255, "LightCyan");
    public static readonly InterlockColor LightGoldenrodYellow = new(250, 250, 210, "LightGoldenrodYellow");
    public static readonly InterlockColor LightGray = new(211, 211, 211, "LightGray");
    public static readonly InterlockColor LightGreen = new(144, 238, 144, "LightGreen");
    public static readonly InterlockColor LightPink = new(255, 182, 193, "LightPink");
    public static readonly InterlockColor LightSalmon = new(255, 160, 122, "LightSalmon");
    public static readonly InterlockColor LightSeaGreen = new(32, 178, 170, "LightSeaGreen");
    public static readonly InterlockColor LightSkyBlue = new(135, 206, 250, "LightSkyBlue");
    public static readonly InterlockColor LightSlateGray = new(119, 136, 153, "LightSlateGray");
    public static readonly InterlockColor LightSteelBlue = new(176, 196, 222, "LightSteelBlue");
    public static readonly InterlockColor LightYellow = new(255, 255, 224, "LightYellow");
    public static readonly InterlockColor Lime = new(0, 255, 0, "Lime");
    public static readonly InterlockColor LimeGreen = new(50, 205, 50, "LimeGreen");
    public static readonly InterlockColor Linen = new(250, 240, 230, "Linen");
    public static readonly InterlockColor Maroon = new(128, 0, 0, "Maroon");
    public static readonly InterlockColor MediumAquamarine = new(102, 205, 170, "MediumAquamarine");
    public static readonly InterlockColor MediumBlue = new(0, 0, 205, "MediumBlue");
    public static readonly InterlockColor MediumOrchid = new(186, 85, 211, "MediumOrchid");
    public static readonly InterlockColor MediumPurple = new(147, 112, 219, "MediumPurple");
    public static readonly InterlockColor MediumSeaGreen = new(60, 179, 113, "MediumSeaGreen");
    public static readonly InterlockColor MediumSlateBlue = new(123, 104, 238, "MediumSlateBlue");
    public static readonly InterlockColor MediumSpringGreen = new(0, 250, 154, "MediumSpringGreen");
    public static readonly InterlockColor MediumTurquoise = new(72, 209, 204, "MediumTurquoise");
    public static readonly InterlockColor MediumVioletRed = new(199, 21, 133, "MediumVioletRed");
    public static readonly InterlockColor MidnightBlue = new(25, 25, 112, "MidnightBlue");
    public static readonly InterlockColor MintCream = new(245, 255, 250, "MintCream");
    public static readonly InterlockColor MistyRose = new(255, 228, 225, "MistyRose");
    public static readonly InterlockColor Moccasin = new(255, 228, 181, "Moccasin");
    public static readonly InterlockColor NavajoWhite = new(255, 222, 173, "NavajoWhite");
    public static readonly InterlockColor Navy = new(0, 0, 128, "Navy");
    public static readonly InterlockColor OldLace = new(253, 245, 230, "OldLace");
    public static readonly InterlockColor Olive = new(128, 128, 0, "Olive");
    public static readonly InterlockColor OliveDrab = new(107, 142, 35, "OliveDrab");
    public static readonly InterlockColor Orange = new(255, 165, 0, "Orange");
    public static readonly InterlockColor OrangeRed = new(255, 69, 0, "OrangeRed");
    public static readonly InterlockColor Orchid = new(218, 112, 214, "Orchid");
    public static readonly InterlockColor PaleGoldenrod = new(238, 232, 170, "PaleGoldenrod");
    public static readonly InterlockColor PaleGreen = new(152, 251, 152, "PaleGreen");
    public static readonly InterlockColor PaleTurquoise = new(175, 238, 238, "PaleTurquoise");
    public static readonly InterlockColor PaleVioletRed = new(219, 112, 147, "PaleVioletRed");
    public static readonly InterlockColor PapayaWhip = new(255, 239, 213, "PapayaWhip");
    public static readonly InterlockColor PeachPuff = new(255, 218, 185, "PeachPuff");
    public static readonly InterlockColor Peru = new(205, 133, 63, "Peru");
    public static readonly InterlockColor Pink = new(255, 192, 203, "Pink");
    public static readonly InterlockColor Plum = new(221, 160, 221, "Plum");
    public static readonly InterlockColor PowderBlue = new(176, 224, 230, "PowderBlue");
    public static readonly InterlockColor Purple = new(128, 0, 128, "Purple");
    public static readonly InterlockColor Red = new(255, 0, 0, "Red");
    public static readonly InterlockColor RosyBrown = new(188, 143, 143, "RosyBrown");
    public static readonly InterlockColor RoyalBlue = new(65, 105, 225, "RoyalBlue");
    public static readonly InterlockColor SaddleBrown = new(139, 69, 19, "SaddleBrown");
    public static readonly InterlockColor Salmon = new(250, 128, 114, "Salmon");
    public static readonly InterlockColor SandyBrown = new(244, 164, 96, "SandyBrown");
    public static readonly InterlockColor SeaGreen = new(46, 139, 87, "SeaGreen");
    public static readonly InterlockColor SeaShell = new(255, 245, 238, "SeaShell");
    public static readonly InterlockColor Sienna = new(160, 82, 45, "Sienna");
    public static readonly InterlockColor Silver = new(192, 192, 192, "Silver");
    public static readonly InterlockColor SkyBlue = new(135, 206, 235, "SkyBlue");
    public static readonly InterlockColor SlateBlue = new(106, 90, 205, "SlateBlue");
    public static readonly InterlockColor SlateGray = new(112, 128, 144, "SlateGray");
    public static readonly InterlockColor Snow = new(255, 250, 250, "Snow");
    public static readonly InterlockColor SpringGreen = new(0, 255, 127, "SpringGreen");
    public static readonly InterlockColor SteelBlue = new(70, 130, 180, "SteelBlue");
    public static readonly InterlockColor Tan = new(210, 180, 140, "Tan");
    public static readonly InterlockColor Teal = new(0, 128, 128, "Teal");
    public static readonly InterlockColor Thistle = new(216, 191, 216, "Thistle");
    public static readonly InterlockColor Tomato = new(255, 99, 71, "Tomato");
    public static readonly InterlockColor Transparent = new(255, 255, 255, "Transparent", 0);
    public static readonly InterlockColor Turquoise = new(64, 224, 208, "Turquoise");
    public static readonly InterlockColor Violet = new(238, 130, 238, "Violet");
    public static readonly InterlockColor Wheat = new(245, 222, 179, "Wheat");
    public static readonly InterlockColor White = new(255, 255, 255, "White");
    public static readonly InterlockColor WhiteSmoke = new(245, 245, 245, "WhiteSmoke");
    public static readonly InterlockColor Yellow = new(255, 255, 0, "Yellow");
    public static readonly InterlockColor YellowGreen = new(154, 205, 50, "YellowGreen");

    public readonly byte A;

    public readonly byte B;

    public readonly byte G;

    public readonly string Name;

    public readonly byte R;

    public InterlockColor(byte r, byte g, byte b, string? name = null, byte a = 255) {
        R = r;
        G = g;
        B = b;
        A = a;
        TextualRepresentation = Name = name ?? ToColorCode(r, g, b, a);
    }

    public InterlockColor(string textualRepresentation) {
        var color = FromString(textualRepresentation);
        R = color.R;
        G = color.G;
        B = color.B;
        A = color.A;
        TextualRepresentation = Name = color.Name;
    }

    public static InterlockColor Random => From((uint)(DateTimeOffset.Now.Ticks | 255u));
    public string AsCSS => Name is null || Name.StartsWith("#", StringComparison.Ordinal) && Name.Length > 7 ? $"rgba({R},{G},{B},{InvariantPercent(A)})" : Name;
    public InterlockColor Opposite => From(new InterlockColor(Invert(R), Invert(G), Invert(B)).RGBA);
    public uint RGBA => (uint)((R << 24) + (G << 16) + (B << 8) + A);
    public bool IsEmpty => false;
    public bool EqualsForValidInstances(InterlockColor other) => RGBA == other.RGBA;
    public static InterlockColor Empty { get; } = Black;
    public static Regex Mask { get; } = AnythingRegex();
    public string? InvalidityCause {
        get => _invalidityCause;
        private init {
            _invalidityCause = value;
            uint ic = Transparent.RGBA;
            R = (byte)(ic >> 24 & 255);
            G = (byte)(ic >> 16 & 255);
            B = (byte)(ic >> 8 & 255);
            A = (byte)(ic & 255);
            TextualRepresentation = Name = ToColorCode(R, G, B, A);
        }
    }

    public static InterlockColor From(uint value) {
        LazyInitKnownColors();
        return _knownColors!.TryGetValue(value, out var color) ? color : new InterlockColor(value);
    }

    public static InterlockColor FromString(string value) {
        value = value.Safe().Trim();
        LazyInitKnownColors();
        var colorCode = FromColorCode(value);
        return _knownColorsByName!.TryGetValue(value, out var color) ? color : From(colorCode);
    }

    public override bool Equals(object? obj) => obj is InterlockColor other && Equals(other);
    public bool Equals(InterlockColor other) => Textual.Equals(other);
    public ITextual<InterlockColor> Textual => this;

    public string TextualRepresentation { get; private init; }
    public static string InvalidTextualRepresentation { get; } = string.Empty;

    public override int GetHashCode() => (int)RGBA;
    public override string ToString() => Textual.FullRepresentation;

    public InterlockColor WithA(byte newA) => newA == A ? this : new InterlockColor(R, G, B, a: newA);

    private static Dictionary<uint, InterlockColor>? _knownColors;

    private static Dictionary<string, InterlockColor>? _knownColorsByName;
    private string? _invalidityCause;
    static InterlockColor ITextual<InterlockColor>.New(string? invalidityCause, string textualRepresentation) =>
        new() { InvalidityCause = invalidityCause, TextualRepresentation = textualRepresentation };

    private InterlockColor(uint value) {
        R = (byte)(value >> 24 & 255);
        G = (byte)(value >> 16 & 255);
        B = (byte)(value >> 8 & 255);
        A = (byte)(value & 255);
        TextualRepresentation = Name = ToColorCode(R, G, B, A);
    }

    private static uint FromColorCode(string colorCode) {
        if (colorCode.IsBlank())
            return Black.RGBA;
        if (colorCode.Length != 9 && colorCode.Length != 7 || colorCode[0] != '#' ||
            !uint.TryParse(colorCode[1..], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var partial))
            return Transparent.RGBA;
        if (colorCode.Length == 7)
            partial = (partial << 8) + 255;
        return partial;
    }

    private static string InvariantPercent(byte a) => (a / 255.0).ToString(CultureInfo.InvariantCulture);

    private static byte Invert(byte component) => (byte)(255 - component);

    private static void LazyInitKnownColors() {
        _knownColors ??= new Dictionary<uint, InterlockColor> {
            [Transparent.RGBA] = Transparent,
            [AliceBlue.RGBA] = AliceBlue,
            [AntiqueWhite.RGBA] = AntiqueWhite,
            [Aqua.RGBA] = Aqua,
            [Aquamarine.RGBA] = Aquamarine,
            [Azure.RGBA] = Azure,
            [Beige.RGBA] = Beige,
            [Bisque.RGBA] = Bisque,
            [Black.RGBA] = Black,
            [BlanchedAlmond.RGBA] = BlanchedAlmond,
            [Blue.RGBA] = Blue,
            [BlueViolet.RGBA] = BlueViolet,
            [Brown.RGBA] = Brown,
            [BurlyWood.RGBA] = BurlyWood,
            [CadetBlue.RGBA] = CadetBlue,
            [Chartreuse.RGBA] = Chartreuse,
            [Chocolate.RGBA] = Chocolate,
            [Coral.RGBA] = Coral,
            [CornflowerBlue.RGBA] = CornflowerBlue,
            [Cornsilk.RGBA] = Cornsilk,
            [Crimson.RGBA] = Crimson,
            [DarkBlue.RGBA] = DarkBlue,
            [DarkCyan.RGBA] = DarkCyan,
            [DarkGoldenrod.RGBA] = DarkGoldenrod,
            [DarkGray.RGBA] = DarkGray,
            [DarkGreen.RGBA] = DarkGreen,
            [DarkKhaki.RGBA] = DarkKhaki,
            [DarkMagenta.RGBA] = DarkMagenta,
            [DarkOliveGreen.RGBA] = DarkOliveGreen,
            [DarkOrange.RGBA] = DarkOrange,
            [DarkOrchid.RGBA] = DarkOrchid,
            [DarkRed.RGBA] = DarkRed,
            [DarkSalmon.RGBA] = DarkSalmon,
            [DarkSeaGreen.RGBA] = DarkSeaGreen,
            [DarkSlateBlue.RGBA] = DarkSlateBlue,
            [DarkSlateGray.RGBA] = DarkSlateGray,
            [DarkTurquoise.RGBA] = DarkTurquoise,
            [DarkViolet.RGBA] = DarkViolet,
            [DeepPink.RGBA] = DeepPink,
            [DeepSkyBlue.RGBA] = DeepSkyBlue,
            [DimGray.RGBA] = DimGray,
            [DodgerBlue.RGBA] = DodgerBlue,
            [Firebrick.RGBA] = Firebrick,
            [FloralWhite.RGBA] = FloralWhite,
            [ForestGreen.RGBA] = ForestGreen,
            [Fuchsia.RGBA] = Fuchsia,
            [Gainsboro.RGBA] = Gainsboro,
            [GhostWhite.RGBA] = GhostWhite,
            [Gold.RGBA] = Gold,
            [Goldenrod.RGBA] = Goldenrod,
            [Gray.RGBA] = Gray,
            [Green.RGBA] = Green,
            [GreenYellow.RGBA] = GreenYellow,
            [Honeydew.RGBA] = Honeydew,
            [HotPink.RGBA] = HotPink,
            [IndianRed.RGBA] = IndianRed,
            [Indigo.RGBA] = Indigo,
            [Ivory.RGBA] = Ivory,
            [Khaki.RGBA] = Khaki,
            [Lavender.RGBA] = Lavender,
            [LavenderBlush.RGBA] = LavenderBlush,
            [LawnGreen.RGBA] = LawnGreen,
            [LemonChiffon.RGBA] = LemonChiffon,
            [LightBlue.RGBA] = LightBlue,
            [LightCoral.RGBA] = LightCoral,
            [LightCyan.RGBA] = LightCyan,
            [LightGoldenrodYellow.RGBA] = LightGoldenrodYellow,
            [LightGreen.RGBA] = LightGreen,
            [LightGray.RGBA] = LightGray,
            [LightPink.RGBA] = LightPink,
            [LightSalmon.RGBA] = LightSalmon,
            [LightSeaGreen.RGBA] = LightSeaGreen,
            [LightSkyBlue.RGBA] = LightSkyBlue,
            [LightSlateGray.RGBA] = LightSlateGray,
            [LightSteelBlue.RGBA] = LightSteelBlue,
            [LightYellow.RGBA] = LightYellow,
            [Lime.RGBA] = Lime,
            [LimeGreen.RGBA] = LimeGreen,
            [Linen.RGBA] = Linen,
            [Maroon.RGBA] = Maroon,
            [MediumAquamarine.RGBA] = MediumAquamarine,
            [MediumBlue.RGBA] = MediumBlue,
            [MediumOrchid.RGBA] = MediumOrchid,
            [MediumPurple.RGBA] = MediumPurple,
            [MediumSeaGreen.RGBA] = MediumSeaGreen,
            [MediumSlateBlue.RGBA] = MediumSlateBlue,
            [MediumSpringGreen.RGBA] = MediumSpringGreen,
            [MediumTurquoise.RGBA] = MediumTurquoise,
            [MediumVioletRed.RGBA] = MediumVioletRed,
            [MidnightBlue.RGBA] = MidnightBlue,
            [MintCream.RGBA] = MintCream,
            [MistyRose.RGBA] = MistyRose,
            [Moccasin.RGBA] = Moccasin,
            [NavajoWhite.RGBA] = NavajoWhite,
            [Navy.RGBA] = Navy,
            [OldLace.RGBA] = OldLace,
            [Olive.RGBA] = Olive,
            [OliveDrab.RGBA] = OliveDrab,
            [Orange.RGBA] = Orange,
            [OrangeRed.RGBA] = OrangeRed,
            [Orchid.RGBA] = Orchid,
            [PaleGoldenrod.RGBA] = PaleGoldenrod,
            [PaleGreen.RGBA] = PaleGreen,
            [PaleTurquoise.RGBA] = PaleTurquoise,
            [PaleVioletRed.RGBA] = PaleVioletRed,
            [PapayaWhip.RGBA] = PapayaWhip,
            [PeachPuff.RGBA] = PeachPuff,
            [Peru.RGBA] = Peru,
            [Pink.RGBA] = Pink,
            [Plum.RGBA] = Plum,
            [PowderBlue.RGBA] = PowderBlue,
            [Purple.RGBA] = Purple,
            [Red.RGBA] = Red,
            [RosyBrown.RGBA] = RosyBrown,
            [RoyalBlue.RGBA] = RoyalBlue,
            [SaddleBrown.RGBA] = SaddleBrown,
            [Salmon.RGBA] = Salmon,
            [SandyBrown.RGBA] = SandyBrown,
            [SeaGreen.RGBA] = SeaGreen,
            [SeaShell.RGBA] = SeaShell,
            [Sienna.RGBA] = Sienna,
            [Silver.RGBA] = Silver,
            [SkyBlue.RGBA] = SkyBlue,
            [SlateBlue.RGBA] = SlateBlue,
            [SlateGray.RGBA] = SlateGray,
            [Snow.RGBA] = Snow,
            [SpringGreen.RGBA] = SpringGreen,
            [SteelBlue.RGBA] = SteelBlue,
            [Tan.RGBA] = Tan,
            [Teal.RGBA] = Teal,
            [Thistle.RGBA] = Thistle,
            [Tomato.RGBA] = Tomato,
            [Turquoise.RGBA] = Turquoise,
            [Violet.RGBA] = Violet,
            [Wheat.RGBA] = Wheat,
            [White.RGBA] = White,
            [WhiteSmoke.RGBA] = WhiteSmoke,
            [Yellow.RGBA] = Yellow,
            [YellowGreen.RGBA] = YellowGreen,
        };
        _knownColorsByName ??= new Dictionary<string, InterlockColor>(StringComparer.InvariantCultureIgnoreCase) {
            [Transparent.Name] = Transparent,
            [AliceBlue.Name] = AliceBlue,
            [AntiqueWhite.Name] = AntiqueWhite,
            [Aqua.Name] = Aqua,
            [Aquamarine.Name] = Aquamarine,
            [Azure.Name] = Azure,
            [Beige.Name] = Beige,
            [Bisque.Name] = Bisque,
            [Black.Name] = Black,
            [BlanchedAlmond.Name] = BlanchedAlmond,
            [Blue.Name] = Blue,
            [BlueViolet.Name] = BlueViolet,
            [Brown.Name] = Brown,
            [BurlyWood.Name] = BurlyWood,
            [CadetBlue.Name] = CadetBlue,
            [Chartreuse.Name] = Chartreuse,
            [Chocolate.Name] = Chocolate,
            [Coral.Name] = Coral,
            [CornflowerBlue.Name] = CornflowerBlue,
            [Cornsilk.Name] = Cornsilk,
            [Crimson.Name] = Crimson,
            [DarkBlue.Name] = DarkBlue,
            [DarkCyan.Name] = DarkCyan,
            [DarkGoldenrod.Name] = DarkGoldenrod,
            [DarkGray.Name] = DarkGray,
            [DarkGreen.Name] = DarkGreen,
            [DarkKhaki.Name] = DarkKhaki,
            [DarkMagenta.Name] = DarkMagenta,
            [DarkOliveGreen.Name] = DarkOliveGreen,
            [DarkOrange.Name] = DarkOrange,
            [DarkOrchid.Name] = DarkOrchid,
            [DarkRed.Name] = DarkRed,
            [DarkSalmon.Name] = DarkSalmon,
            [DarkSeaGreen.Name] = DarkSeaGreen,
            [DarkSlateBlue.Name] = DarkSlateBlue,
            [DarkSlateGray.Name] = DarkSlateGray,
            [DarkTurquoise.Name] = DarkTurquoise,
            [DarkViolet.Name] = DarkViolet,
            [DeepPink.Name] = DeepPink,
            [DeepSkyBlue.Name] = DeepSkyBlue,
            [DimGray.Name] = DimGray,
            [DodgerBlue.Name] = DodgerBlue,
            [Firebrick.Name] = Firebrick,
            [FloralWhite.Name] = FloralWhite,
            [ForestGreen.Name] = ForestGreen,
            [Fuchsia.Name] = Fuchsia,
            [Gainsboro.Name] = Gainsboro,
            [GhostWhite.Name] = GhostWhite,
            [Gold.Name] = Gold,
            [Goldenrod.Name] = Goldenrod,
            [Gray.Name] = Gray,
            [Green.Name] = Green,
            [GreenYellow.Name] = GreenYellow,
            [Honeydew.Name] = Honeydew,
            [HotPink.Name] = HotPink,
            [IndianRed.Name] = IndianRed,
            [Indigo.Name] = Indigo,
            [Ivory.Name] = Ivory,
            [Khaki.Name] = Khaki,
            [Lavender.Name] = Lavender,
            [LavenderBlush.Name] = LavenderBlush,
            [LawnGreen.Name] = LawnGreen,
            [LemonChiffon.Name] = LemonChiffon,
            [LightBlue.Name] = LightBlue,
            [LightCoral.Name] = LightCoral,
            [LightCyan.Name] = LightCyan,
            [LightGoldenrodYellow.Name] = LightGoldenrodYellow,
            [LightGreen.Name] = LightGreen,
            [LightGray.Name] = LightGray,
            [LightPink.Name] = LightPink,
            [LightSalmon.Name] = LightSalmon,
            [LightSeaGreen.Name] = LightSeaGreen,
            [LightSkyBlue.Name] = LightSkyBlue,
            [LightSlateGray.Name] = LightSlateGray,
            [LightSteelBlue.Name] = LightSteelBlue,
            [LightYellow.Name] = LightYellow,
            [Lime.Name] = Lime,
            [LimeGreen.Name] = LimeGreen,
            [Linen.Name] = Linen,
            [Maroon.Name] = Maroon,
            [MediumAquamarine.Name] = MediumAquamarine,
            [MediumBlue.Name] = MediumBlue,
            [MediumOrchid.Name] = MediumOrchid,
            [MediumPurple.Name] = MediumPurple,
            [MediumSeaGreen.Name] = MediumSeaGreen,
            [MediumSlateBlue.Name] = MediumSlateBlue,
            [MediumSpringGreen.Name] = MediumSpringGreen,
            [MediumTurquoise.Name] = MediumTurquoise,
            [MediumVioletRed.Name] = MediumVioletRed,
            [MidnightBlue.Name] = MidnightBlue,
            [MintCream.Name] = MintCream,
            [MistyRose.Name] = MistyRose,
            [Moccasin.Name] = Moccasin,
            [NavajoWhite.Name] = NavajoWhite,
            [Navy.Name] = Navy,
            [OldLace.Name] = OldLace,
            [Olive.Name] = Olive,
            [OliveDrab.Name] = OliveDrab,
            [Orange.Name] = Orange,
            [OrangeRed.Name] = OrangeRed,
            [Orchid.Name] = Orchid,
            [PaleGoldenrod.Name] = PaleGoldenrod,
            [PaleGreen.Name] = PaleGreen,
            [PaleTurquoise.Name] = PaleTurquoise,
            [PaleVioletRed.Name] = PaleVioletRed,
            [PapayaWhip.Name] = PapayaWhip,
            [PeachPuff.Name] = PeachPuff,
            [Peru.Name] = Peru,
            [Pink.Name] = Pink,
            [Plum.Name] = Plum,
            [PowderBlue.Name] = PowderBlue,
            [Purple.Name] = Purple,
            [Red.Name] = Red,
            [RosyBrown.Name] = RosyBrown,
            [RoyalBlue.Name] = RoyalBlue,
            [SaddleBrown.Name] = SaddleBrown,
            [Salmon.Name] = Salmon,
            [SandyBrown.Name] = SandyBrown,
            [SeaGreen.Name] = SeaGreen,
            [SeaShell.Name] = SeaShell,
            [Sienna.Name] = Sienna,
            [Silver.Name] = Silver,
            [SkyBlue.Name] = SkyBlue,
            [SlateBlue.Name] = SlateBlue,
            [SlateGray.Name] = SlateGray,
            [Snow.Name] = Snow,
            [SpringGreen.Name] = SpringGreen,
            [SteelBlue.Name] = SteelBlue,
            [Tan.Name] = Tan,
            [Teal.Name] = Teal,
            [Thistle.Name] = Thistle,
            [Tomato.Name] = Tomato,
            [Turquoise.Name] = Turquoise,
            [Violet.Name] = Violet,
            [Wheat.Name] = Wheat,
            [White.Name] = White,
            [WhiteSmoke.Name] = WhiteSmoke,
            [Yellow.Name] = Yellow,
            [YellowGreen.Name] = YellowGreen,
        };
    }

    private static string ToColorCode(byte r, byte g, byte b, byte a) {
        return "#" + ToHex(r) + ToHex(g) + ToHex(b) + (a < 255 ? ToHex(a) : "");

        static string ToHex(byte b) => b.ToString("X2", CultureInfo.InvariantCulture);
    }

    [GeneratedRegex(@"(\w+|#[0-9a-fA-F]{6})")]
    private static partial Regex AnythingRegex();
}

using ITGlobal.MarkDocs.Format.Impl;
using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Format
{
    /// <summary>
    ///  Built-in stylesheet name
    /// </summary>
    [PublicAPI]
    public enum HighlightJsStylesheet
    {
        /// <summary>
        ///     Disable style inlining
        /// </summary>
        [PublicAPI]
        None,

        // Run script
        // gci ./Resources/styles | %{ $name = $_.Name.Substring(0, $_.Name.Length - 4); $s = ""; $u = $true; for($i = 0; $i -lt $name.Length; $i++) { if([char]::IsLetterOrDigit($name[$i])) { if($u) { $s += [char]::ToUpper($name[$i]); $u = $false; } else { $s += [char]::ToLower($name[$i]) } } else { $u = $true; } } return "/// <summary>`n///     $s`n/// </summary>`n[Resource(`"$name.css`")]`n$s,`n"}
        // to generate code below

        /// <summary>
        ///     Agate
        /// </summary>
        [Resource("agate.css")]
        Agate,

        /// <summary>
        ///     Androidstudio
        /// </summary>
        [Resource("androidstudio.css")]
        Androidstudio,

        /// <summary>
        ///     ArduinoLight
        /// </summary>
        [Resource("arduino-light.css")]
        ArduinoLight,

        /// <summary>
        ///     Arta
        /// </summary>
        [Resource("arta.css")]
        Arta,

        /// <summary>
        ///     Ascetic
        /// </summary>
        [Resource("ascetic.css")]
        Ascetic,

        /// <summary>
        ///     AtelierCaveDark
        /// </summary>
        [Resource("atelier-cave-dark.css")]
        AtelierCaveDark,

        /// <summary>
        ///     AtelierCaveLight
        /// </summary>
        [Resource("atelier-cave-light.css")]
        AtelierCaveLight,

        /// <summary>
        ///     AtelierDuneDark
        /// </summary>
        [Resource("atelier-dune-dark.css")]
        AtelierDuneDark,

        /// <summary>
        ///     AtelierDuneLight
        /// </summary>
        [Resource("atelier-dune-light.css")]
        AtelierDuneLight,

        /// <summary>
        ///     AtelierEstuaryDark
        /// </summary>
        [Resource("atelier-estuary-dark.css")]
        AtelierEstuaryDark,

        /// <summary>
        ///     AtelierEstuaryLight
        /// </summary>
        [Resource("atelier-estuary-light.css")]
        AtelierEstuaryLight,

        /// <summary>
        ///     AtelierForestDark
        /// </summary>
        [Resource("atelier-forest-dark.css")]
        AtelierForestDark,

        /// <summary>
        ///     AtelierForestLight
        /// </summary>
        [Resource("atelier-forest-light.css")]
        AtelierForestLight,

        /// <summary>
        ///     AtelierHeathDark
        /// </summary>
        [Resource("atelier-heath-dark.css")]
        AtelierHeathDark,

        /// <summary>
        ///     AtelierHeathLight
        /// </summary>
        [Resource("atelier-heath-light.css")]
        AtelierHeathLight,

        /// <summary>
        ///     AtelierLakesideDark
        /// </summary>
        [Resource("atelier-lakeside-dark.css")]
        AtelierLakesideDark,

        /// <summary>
        ///     AtelierLakesideLight
        /// </summary>
        [Resource("atelier-lakeside-light.css")]
        AtelierLakesideLight,

        /// <summary>
        ///     AtelierPlateauDark
        /// </summary>
        [Resource("atelier-plateau-dark.css")]
        AtelierPlateauDark,

        /// <summary>
        ///     AtelierPlateauLight
        /// </summary>
        [Resource("atelier-plateau-light.css")]
        AtelierPlateauLight,

        /// <summary>
        ///     AtelierSavannaDark
        /// </summary>
        [Resource("atelier-savanna-dark.css")]
        AtelierSavannaDark,

        /// <summary>
        ///     AtelierSavannaLight
        /// </summary>
        [Resource("atelier-savanna-light.css")]
        AtelierSavannaLight,

        /// <summary>
        ///     AtelierSeasideDark
        /// </summary>
        [Resource("atelier-seaside-dark.css")]
        AtelierSeasideDark,

        /// <summary>
        ///     AtelierSeasideLight
        /// </summary>
        [Resource("atelier-seaside-light.css")]
        AtelierSeasideLight,

        /// <summary>
        ///     AtelierSulphurpoolDark
        /// </summary>
        [Resource("atelier-sulphurpool-dark.css")]
        AtelierSulphurpoolDark,

        /// <summary>
        ///     AtelierSulphurpoolLight
        /// </summary>
        [Resource("atelier-sulphurpool-light.css")]
        AtelierSulphurpoolLight,

        /// <summary>
        ///     AtomOneDark
        /// </summary>
        [Resource("atom-one-dark.css")]
        AtomOneDark,

        /// <summary>
        ///     AtomOneLight
        /// </summary>
        [Resource("atom-one-light.css")]
        AtomOneLight,

        /// <summary>
        ///     CodepenEmbed
        /// </summary>
        [Resource("codepen-embed.css")]
        CodepenEmbed,

        /// <summary>
        ///     ColorBrewer
        /// </summary>
        [Resource("color-brewer.css")]
        ColorBrewer,

        /// <summary>
        ///     Darcula
        /// </summary>
        [Resource("darcula.css")]
        Darcula,

        /// <summary>
        ///     Dark
        /// </summary>
        [Resource("dark.css")]
        Dark,

        /// <summary>
        ///     Darkula
        /// </summary>
        [Resource("darkula.css")]
        Darkula,

        /// <summary>
        ///     Default
        /// </summary>
        [Resource("default.css")]
        Default,

        /// <summary>
        ///     Docco
        /// </summary>
        [Resource("docco.css")]
        Docco,

        /// <summary>
        ///     Dracula
        /// </summary>
        [Resource("dracula.css")]
        Dracula,

        /// <summary>
        ///     Far
        /// </summary>
        [Resource("far.css")]
        Far,

        /// <summary>
        ///     Foundation
        /// </summary>
        [Resource("foundation.css")]
        Foundation,

        /// <summary>
        ///     GithubGist
        /// </summary>
        [Resource("github-gist.css")]
        GithubGist,

        /// <summary>
        ///     Github
        /// </summary>
        [Resource("github.css")]
        Github,

        /// <summary>
        ///     Googlecode
        /// </summary>
        [Resource("googlecode.css")]
        Googlecode,

        /// <summary>
        ///     Grayscale
        /// </summary>
        [Resource("grayscale.css")]
        Grayscale,

        /// <summary>
        ///     GruvboxDark
        /// </summary>
        [Resource("gruvbox-dark.css")]
        GruvboxDark,

        /// <summary>
        ///     GruvboxLight
        /// </summary>
        [Resource("gruvbox-light.css")]
        GruvboxLight,

        /// <summary>
        ///     Hopscotch
        /// </summary>
        [Resource("hopscotch.css")]
        Hopscotch,

        /// <summary>
        ///     Hybrid
        /// </summary>
        [Resource("hybrid.css")]
        Hybrid,

        /// <summary>
        ///     Idea
        /// </summary>
        [Resource("idea.css")]
        Idea,

        /// <summary>
        ///     IrBlack
        /// </summary>
        [Resource("ir-black.css")]
        IrBlack,

        /// <summary>
        ///     KimbieDark
        /// </summary>
        [Resource("kimbie.dark.css")]
        KimbieDark,

        /// <summary>
        ///     KimbieLight
        /// </summary>
        [Resource("kimbie.light.css")]
        KimbieLight,

        /// <summary>
        ///     Magula
        /// </summary>
        [Resource("magula.css")]
        Magula,

        /// <summary>
        ///     MonoBlue
        /// </summary>
        [Resource("mono-blue.css")]
        MonoBlue,

        /// <summary>
        ///     MonokaiSublime
        /// </summary>
        [Resource("monokai-sublime.css")]
        MonokaiSublime,

        /// <summary>
        ///     Monokai
        /// </summary>
        [Resource("monokai.css")]
        Monokai,

        /// <summary>
        ///     Obsidian
        /// </summary>
        [Resource("obsidian.css")]
        Obsidian,

        /// <summary>
        ///     Ocean
        /// </summary>
        [Resource("ocean.css")]
        Ocean,

        /// <summary>
        ///     ParaisoDark
        /// </summary>
        [Resource("paraiso-dark.css")]
        ParaisoDark,

        /// <summary>
        ///     ParaisoLight
        /// </summary>
        [Resource("paraiso-light.css")]
        ParaisoLight,

        /// <summary>
        ///     Purebasic
        /// </summary>
        [Resource("purebasic.css")]
        Purebasic,

        /// <summary>
        ///     QtcreatorDark
        /// </summary>
        [Resource("qtcreator_dark.css")]
        QtcreatorDark,

        /// <summary>
        ///     QtcreatorLight
        /// </summary>
        [Resource("qtcreator_light.css")]
        QtcreatorLight,

        /// <summary>
        ///     Railscasts
        /// </summary>
        [Resource("railscasts.css")]
        Railscasts,

        /// <summary>
        ///     Rainbow
        /// </summary>
        [Resource("rainbow.css")]
        Rainbow,

        /// <summary>
        ///     SolarizedDark
        /// </summary>
        [Resource("solarized-dark.css")]
        SolarizedDark,

        /// <summary>
        ///     SolarizedLight
        /// </summary>
        [Resource("solarized-light.css")]
        SolarizedLight,

        /// <summary>
        ///     Sunburst
        /// </summary>
        [Resource("sunburst.css")]
        Sunburst,

        /// <summary>
        ///     TomorrowNightBlue
        /// </summary>
        [Resource("tomorrow-night-blue.css")]
        TomorrowNightBlue,

        /// <summary>
        ///     TomorrowNightBright
        /// </summary>
        [Resource("tomorrow-night-bright.css")]
        TomorrowNightBright,

        /// <summary>
        ///     TomorrowNightEighties
        /// </summary>
        [Resource("tomorrow-night-eighties.css")]
        TomorrowNightEighties,

        /// <summary>
        ///     TomorrowNight
        /// </summary>
        [Resource("tomorrow-night.css")]
        TomorrowNight,

        /// <summary>
        ///     Tomorrow
        /// </summary>
        [Resource("tomorrow.css")]
        Tomorrow,

        /// <summary>
        ///     Vs
        /// </summary>
        [Resource("vs.css")]
        Vs,

        /// <summary>
        ///     Xcode
        /// </summary>
        [Resource("xcode.css")]
        Xcode,

        /// <summary>
        ///     Xt256
        /// </summary>
        [Resource("xt256.css")]
        Xt256,

        /// <summary>
        ///     Zenburn
        /// </summary>
        [Resource("zenburn.css")]
        Zenburn
    }
}
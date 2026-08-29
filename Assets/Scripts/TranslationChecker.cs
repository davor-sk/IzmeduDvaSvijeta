using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

// Provjera je li igrac u OVOM prolazu dosao do kanonskog prijevoda svih rijeci.
//
// VAZNO: namjerno se NE koristi $translationAccuracy.
// Taj brojac se samo povecava i nikada ne smanjuje, pa igrac koji prvo
// pogrijesi i kasnije ispravi prijevod (sto je namjerna mehanika igre)
// zavrsi s premalim brojem iako mu je konacno stanje tocno.
// Zato se gleda stvarno konacno stanje varijabli.
public static class TranslationChecker
{
    // Rijec -> prihvatljiva konacna stanja.
    // Vecina rijeci ima jedno tocno stanje; "uzima" ima dva ravnopravna.
    private static readonly Dictionary<string, string[]> Canonical =
        new Dictionary<string, string[]>
        {
            { "$velAhnTranslation",     new[] { "greeting" } },
            { "$toruTranslation",       new[] { "surface" } },
            { "$hodiTranslation",       new[] { "move" } },
            { "$dahTranslation",        new[] { "life" } },
            { "$moranTranslation",      new[] { "concept" } },
            { "$vornTranslation",       new[] { "mediator" } },
            { "$kaelTranslation",       new[] { "continues" } },
            { "$uzimaTranslation",      new[] { "take", "take_away" } },
            { "$kamenTranslation",      new[] { "resource" } },
            { "$padaTranslation",       new[] { "collapse" } },
            { "$rukaTranslation",       new[] { "cooperation" } },
            { "$utisaTranslation",      new[] { "silence_contact" } },

            // Pazi: ova varijabla se zove drugacije od ostalih
            { "$vatraInterpretation",   new[] { "surface_depth_activity" } }
        };

    public static int TotalWords => Canonical.Count;

    // Je li konacno stanje svih rijeci kanonsko
    public static bool AllCorrect(VariableStorageBehaviour storage)
    {
        return CorrectCount(storage) >= TotalWords;
    }

    public static int CorrectCount(VariableStorageBehaviour storage)
    {
        if (storage == null)
            return 0;

        int count = 0;

        foreach (var pair in Canonical)
        {
            if (IsCorrect(storage, pair.Key))
                count++;
        }

        return count;
    }

    public static bool IsCorrect(VariableStorageBehaviour storage, string variableName)
    {
        if (storage == null)
            return false;

        if (!Canonical.TryGetValue(variableName, out string[] accepted))
            return false;

        if (!storage.TryGetValue<string>(variableName, out string value))
            return false;

        if (string.IsNullOrEmpty(value))
            return false;

        foreach (var ok in accepted)
        {
            if (value == ok)
                return true;
        }

        return false;
    }

    // Za debug: koje rijeci jos nisu tocne
    public static List<string> GetIncorrectWords(VariableStorageBehaviour storage)
    {
        var result = new List<string>();

        foreach (var pair in Canonical)
        {
            if (!IsCorrect(storage, pair.Key))
                result.Add(pair.Key);
        }

        return result;
    }
}

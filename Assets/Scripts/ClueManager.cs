using System;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

[System.Serializable]
public class ClueDefinition
{
    public string id;
    public int session;
    public string description;

    public ClueDefinition(string id, int session, string description)
    {
        this.id = id;
        this.session = session;
        this.description = description;
    }
}

// Trajno pamti koja je otkrica igrac pronasao, kroz vise playthroughova.
// Isti clue moze se otkriti na vise nacina, ali se broji samo jednom.
public static class ClueManager
{
    public const int TotalClues = 33;

    public static event Action<string> OnClueUnlocked;

    // Kanonski popis svih otkrica, redom C01-C33.
    public static readonly ClueDefinition[] AllClues =
    {
        new ClueDefinition("C01_RESTRICTED_7G", 1, "Kanal 7-G ima ogranicen pristup i autorizaciju Razina V."),
        new ClueDefinition("C02_DEEP_SIGNAL", 1, "Signal dolazi oko 2,8 km ispod Povrsine i sadrzi strukturirane jezicne uzorke."),
        new ClueDefinition("C03_DHORN_PEOPLE", 1, "Dh'orn nije ime pojedinca nego naziv naroda/skupine."),
        new ClueDefinition("C04_KAEL_NOT_NAME", 1, "kael nije Kaelovo osobno ime nego Dhorn rijec."),
        new ClueDefinition("C05_PREVIOUS_CONTACT", 1, "Prije Kaela vec je postojao kontakt izmedu Dhorna i Povrsine / prethodni Vorn."),
        new ClueDefinition("C06_VORN_ROLE", 1, "Vorn nije ime nego uloga osobe koja slusa obje strane i prenosi govor."),
        new ClueDefinition("C07_DEPTH_THREAT", 2, "Moran/dubinski prostor je ugrozen; ono sto Dhorni pokusavaju sacuvati propada."),
        new ClueDefinition("C08_SURFACE_OPERATIONS", 2, "Povrsina provodi geoloske operacije gotovo na koordinatama Dhorn signala."),
        new ClueDefinition("C09_OLD_7G_ACTIVITY", 2, "Aktivnost na 7-G postojala je prije Kaelova dolaska."),
        new ClueDefinition("C10_27_YEAR_RECORD", 2, "Najstariji pronadeni zapis s 7-G star je 27 godina."),
        new ClueDefinition("C11_OLD_VORN_FRAGMENT", 2, "Stari ostecen zapis sadrzi vorn - toru - moran."),
        new ClueDefinition("C12_SILENCED_VORN", 3, "Dhorni tvrde da je Povrsina utisala prethodnog Vorna."),
        new ClueDefinition("C13_V27", 3, "U starom programu postojao je klasificirani lingvisticki posrednik V-27."),
        new ClueDefinition("C14_REMOVED_FROM_PROGRAM", 3, "Sluzbeni dokument kaze da je posrednik uklonjen iz programa."),
        new ClueDefinition("C15_V_PROGRAM", 3, "V-27 nije jedina V-oznaka; postoje V-11, V-19 itd."),
        new ClueDefinition("C16_VORN_DEPTH_LINK", 3, "Moguca veza izmedu starog Vorna i Surface aktivnosti u dubini."),
        new ClueDefinition("C17_VATRA7_PROJECT", 4, "Postoji sluzbeni Surface projekt pod nazivom Vatra-7."),
        new ClueDefinition("C18_VATRA7_OVERLAP", 4, "Koordinate Faze II preklapaju se s podrucjem Dhorn signala."),
        new ClueDefinition("C19_V27_DEPTH_TRACE", 4, "V-27 je imao pristup tehnickim zapisima iz istog dubinskog sektora."),
        new ClueDefinition("C20_VATRA7_COUNTDOWN", 4, "Faza II ide na vecu dubinu i puni kapacitet za 48 sati."),
        new ClueDefinition("C21_DHORN_COLLAPSE_NOW", 5, "Dhorni tvrde da se njihov prostor vec urusava."),
        new ClueDefinition("C22_VATRA7_CORRELATION", 5, "Vibracije koreliraju s radom Vatre-7, ali uzrocnost nije potvrdena."),
        new ClueDefinition("C23_PHASE2_SECURITY_CONTROL", 5, "Kontrolu Faze II preuzima Operativno zapovjednistvo."),
        new ClueDefinition("C24_MAREN_LOST_CONTROL", 5, "Maren vise nema punu kontrolu nad operativnom reakcijom."),
        new ClueDefinition("C25_V27_ARCHIVE_TRACE", 6, "V-27 pojavljuje se u starim tehnickim zapisima istog sektora."),
        new ClueDefinition("C26_RECOGNIZED_SYMBOL", 6, "Kael prepoznaje privatni simbol povezan s V-27."),
        new ClueDefinition("C27_UNIT14_ENTRY", 6, "Jedinica Cetrnaest dobiva odobrenje za ulazak u dubinski sektor."),
        new ClueDefinition("C28_MAREN_DID_NOT_AUTHORIZE", 6, "Maren tvrdi da ona nije odobrila eskalaciju Jedinice Cetrnaest."),
        new ClueDefinition("C29_V27_PRIVATE_TRANSMITTER", 6, "V-27 je koristio privatni/nestandardni odasiljac na 7-G."),
        new ClueDefinition("C30_GRANDFATHER_IS_V27", 7, "Fotografija potvrduje da je Kaelov djed bio V-27."),
        new ClueDefinition("C31_KAEL_MEANING", 7, "kael oznacava onoga koji dolazi poslije i nastavlja prekinutu ulogu."),
        new ClueDefinition("C32_V27_SAVED_ORIGINALS", 7, "V-27 je namjerno cuvao originalne transmisije izvan sluzbenog sustava."),
        new ClueDefinition("C33_V27_REFUSED_THREAT_LABEL", 7, "V-27 tvrdi da je uklonjen jer nije htio Dhorne oznaciti prijetnjom.")
    };

    private static Dictionary<string, ClueDefinition> lookup;

    private static Dictionary<string, ClueDefinition> Lookup
    {
        get
        {
            if (lookup == null)
            {
                lookup = new Dictionary<string, ClueDefinition>();

                foreach (var clue in AllClues)
                    lookup[clue.id] = clue;
            }

            return lookup;
        }
    }

    private static ClueProgressData cached;

    private static ClueProgressData Data
    {
        get
        {
            if (cached == null)
                cached = ClueSaveSystem.Load();

            return cached;
        }
    }

    // Yarn: <<unlock_clue "C05_PREVIOUS_CONTACT">>
    // Staticno, pa ne treba ciljni objekt u sceni.
    [YarnCommand("unlock_clue")]
    public static void UnlockClue(string clueId)
    {
        AddClue(clueId);
    }

    // Sigurno je pozvati vise puta - isti clue se broji samo jednom.
    public static void AddClue(string clueId)
    {
        if (string.IsNullOrEmpty(clueId))
            return;

        clueId = clueId.Trim();

        if (!Lookup.ContainsKey(clueId))
        {
            Debug.LogWarning("AddClue: nepoznat clue: " + clueId);
            return;
        }

        if (Data.unlockedClues.Contains(clueId))
            return;

        Data.unlockedClues.Add(clueId);
        ClueSaveSystem.Save(Data);

        Debug.Log("Novo otkrice: " + clueId + " (" + UnlockedCount + "/" + TotalClues + ")");

        OnClueUnlocked?.Invoke(clueId);

        if (AllCluesFound())
            AchievementManager.Unlock(AchievementManager.Investigator);
    }

    public static bool IsUnlocked(string clueId)
    {
        return !string.IsNullOrEmpty(clueId) && Data.unlockedClues.Contains(clueId.Trim());
    }

    public static int UnlockedCount => Data.unlockedClues.Count;

    public static bool AllCluesFound() => UnlockedCount >= TotalClues;

    public static string GetProgressText() => UnlockedCount + "/" + TotalClues;

    public static string GetDescription(string clueId)
    {
        return Lookup.TryGetValue(clueId ?? "", out var c) ? c.description : "";
    }

    public static void ResetAll()
    {
        cached = new ClueProgressData();
        ClueSaveSystem.ResetProgress();
    }
}

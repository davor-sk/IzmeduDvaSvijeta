using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class NotebookWordEntry
{
    public string dhornWord;
    public string translation;
}

[System.Serializable]
public class SaveData
{
    public string currentNode;

    
    public List<string> floatKeys = new List<string>();
    public List<float> floatValues = new List<float>();

    public List<string> stringKeys = new List<string>();
    public List<string> stringValues = new List<string>();

    public List<string> boolKeys = new List<string>();
    public List<bool> boolValues = new List<bool>();

   
    public List<NotebookWordEntry> notebookWords = new List<NotebookWordEntry>();

    public string savedAtDisplay;
}

public static class SaveSystem
{
   
    public const int SlotCount = 3;

    private static string FilePathFor(int slot)
    {
        return Path.Combine(Application.persistentDataPath, "savegame" + slot + ".json");
    }


    private static string LegacyFilePath => Path.Combine(Application.persistentDataPath, "savegame.json");

    public static bool IsValidSlot(int slot)
    {
        return slot >= 1 && slot <= SlotCount;
    }

    public static bool HasSave(int slot)
    {
        if (!IsValidSlot(slot))
            return false;

        MigrateLegacySaveIfNeeded();
        return File.Exists(FilePathFor(slot));
    }

    
    public static bool HasAnySave()
    {
        for (int slot = 1; slot <= SlotCount; slot++)
        {
            if (HasSave(slot))
                return true;
        }

        return false;
    }

    public static void Save(SaveData data, int slot)
    {
        if (!IsValidSlot(slot))
        {
            Debug.LogError("Save: nevaljan slot " + slot);
            return;
        }

        try
        {
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(FilePathFor(slot), json);
            Debug.Log("Igra spremljena u slot " + slot + ": " + FilePathFor(slot));
        }
        catch (System.Exception e)
        {
            Debug.LogError("Spremanje nije uspjelo: " + e.Message);
        }
    }

    public static SaveData Load(int slot)
    {
        if (!HasSave(slot))
            return null;

        try
        {
            string json = File.ReadAllText(FilePathFor(slot));
            return JsonUtility.FromJson<SaveData>(json);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Ucitavanje nije uspjelo: " + e.Message);
            return null;
        }
    }

    public static void DeleteSave(int slot)
    {
        if (HasSave(slot))
        {
            File.Delete(FilePathFor(slot));
            Debug.Log("Slot " + slot + " obrisan.");
        }
    }

    public static string GetSlotLabel(int slot)
    {
        if (!HasSave(slot))
            return slot + " - prazno";

        var data = Load(slot);

        if (data == null || string.IsNullOrEmpty(data.savedAtDisplay))
            return slot + " - spremljeno";

        return slot + " - " + data.savedAtDisplay;
    }

   
    private static bool legacyChecked = false;

    private static void MigrateLegacySaveIfNeeded()
    {
        if (legacyChecked)
            return;

        legacyChecked = true;

        try
        {
            if (!File.Exists(LegacyFilePath))
                return;

            string slot1 = FilePathFor(1);

            if (!File.Exists(slot1))
            {
                File.Move(LegacyFilePath, slot1);
                Debug.Log("Stari savegame.json prebacen u slot 1.");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("Migracija starog savea nije uspjela: " + e.Message);
        }
    }
}

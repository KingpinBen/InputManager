using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

public static class InputManager
{
    //  Hook this for objects like GUI elements to update them when
    //  a rebind occurs
    public static Action OnKeyBindChange;

    private static Dictionary<KeyUsage, KeyCode> _keyCodeChanges;

    private static void InitializeDictionary()
    {
        if (_keyCodeChanges != null)
            return;

        _keyCodeChanges = new Dictionary<KeyUsage, KeyCode>(new KeyUsageComparer());
    }

    public static bool GetKeyDown(KeyUsage use)
    {
        KeyCode code;
        if (_keyCodeChanges.TryGetValue(use, out code))
            return Input.GetKeyDown(code);
        return false;
    }

    public static bool GetKeyUp(KeyUsage use)
    {
        KeyCode code;
        if (_keyCodeChanges.TryGetValue(use, out code))
            return Input.GetKeyUp(code);
        return false;
    }

    public static bool GetKey(KeyUsage use)
    {
        KeyCode code;
        if (_keyCodeChanges.TryGetValue(use, out code))
            return Input.GetKey(code);
        return false;
    }

    public static void ChangeKeyBinding(KeyUsage use, KeyCode newKey)
    {
        //  If the chosen key is already a keybinding, switch them
        if (_keyCodeChanges.ContainsValue(newKey))
        {
            var e = _keyCodeChanges.Keys.GetEnumerator();

            while(e.MoveNext())
            {
                if (_keyCodeChanges[e.Current] == newKey)
                {
                    _keyCodeChanges[e.Current] = _keyCodeChanges[use];
                    break;
                }
            }
        }

        _keyCodeChanges[use] = newKey;

        if (OnKeyBindChange != null)
            OnKeyBindChange.Invoke();
    }

    public static void ResetKeyBinding()
    {
        if (_keyCodeChanges == null)
            InitializeDictionary();

        _keyCodeChanges[KeyUsage.Ability1] = KeyCode.Q;
        _keyCodeChanges[KeyUsage.Ability2] = KeyCode.W;
        _keyCodeChanges[KeyUsage.Ability3] = KeyCode.E;
        _keyCodeChanges[KeyUsage.Ability4] = KeyCode.R;

        _keyCodeChanges[KeyUsage.Recall] = KeyCode.B;
        _keyCodeChanges[KeyUsage.Stop] = KeyCode.S;

        _keyCodeChanges[KeyUsage.Inventory1] = KeyCode.Alpha1;
        _keyCodeChanges[KeyUsage.Inventory2] = KeyCode.Alpha2;
        _keyCodeChanges[KeyUsage.Inventory3] = KeyCode.Alpha3;
        _keyCodeChanges[KeyUsage.Inventory4] = KeyCode.Alpha4;
        _keyCodeChanges[KeyUsage.Inventory5] = KeyCode.Alpha5;
        _keyCodeChanges[KeyUsage.Inventory6] = KeyCode.Alpha6;
        _keyCodeChanges[KeyUsage.Inventory7] = KeyCode.Alpha7;

        if (OnKeyBindChange != null)
            OnKeyBindChange.Invoke();
    }

    public static bool CharacterHasCustomBindings(short characterID)
    {
        return File.Exists(string.Format("{0}/Character_{1}.binds", Application.dataPath, characterID));
    }

    public static void LoadKeyBindings(short characterID)
    {
        var path = string.Format("{0}/Character_{1}.binds", Application.dataPath, characterID);

        //  If there isn't a custom key layout for the characterID, we'll use 0 which is default
        if (!File.Exists(path))
            path = string.Format("{0}/Character_0.binds", Application.dataPath);

        //  If the default bindings path doesn't exist, make it with default settings
        if (!File.Exists(path))
        {
            ResetKeyBinding();
            SaveKeyBindings(0);
        }

        if (_keyCodeChanges == null)
            InitializeDictionary();

        using (var readStream = new BinaryReader(File.Open(path, FileMode.Open)))
        {
            //  I don't necessarily need to know the count as it's just the amount of enums in
            //  KeyUse, but it's just future safe
            int count = readStream.ReadInt32();

            for(int i = 0; i < count; i++)
            {
                var use = (KeyUsage)readStream.ReadInt32();
                _keyCodeChanges[use] = (KeyCode)readStream.ReadInt32();
            }
        }

        if (OnKeyBindChange != null)
            OnKeyBindChange.Invoke();
    }

    public static void SaveKeyBindings(short characterID)
    {
        var path = string.Format("{0}/Character_{1}.binds", Application.dataPath, characterID);
        var tempFilePath = string.Format("{0}.tmp", path);
        var tempFile = File.Create(tempFilePath);

        using (var wStream = new BinaryWriter(tempFile))
        {
            var e = _keyCodeChanges.GetEnumerator();

            wStream.Write(_keyCodeChanges.Count);
            
            while(e.MoveNext())
            {
                wStream.Write((int)e.Current.Key);
                wStream.Write((int)e.Current.Value);
            }

            wStream.Flush();
            wStream.Close();

            if (File.Exists(path))
                File.Replace(tempFilePath, path, string.Format("{0}.backup", path));
            else
                File.Move(tempFilePath, path);
        }
    }

    public static KeyCode GetKeyForUsage(KeyUsage usage)
    {
        if (_keyCodeChanges == null)
            LoadKeyBindings(0);

        return _keyCodeChanges[usage];
    }
}

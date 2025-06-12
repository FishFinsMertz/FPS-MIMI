using System;
using UnityEngine;
using System.Collections.Generic;

public static class InputManager
{
    public enum KeyType { ExampleKey }; 
    public static Dictionary<KeyType, KeyCode> keyMapping;

    static Dictionary<KeyType, KeyCode> defaultMapping = new Dictionary<KeyType, KeyCode>() 
    {
        {KeyType.ExampleKey, KeyCode.A}
    };

    static InputManager() 
    {
        keyMapping = defaultMapping;
    }

    public static void SetKeyMap(KeyType keyType, KeyCode button)
    {
        if (!keyMapping.ContainsKey(keyType)) throw new ArgumentException("Cannot Set KeyType " + keyType.ToString() + " Due to: KeyType Not in KeyMapping.");
        keyMapping[keyType] = button; // DOES NOT CHECK IF BUTTONS OVERLAP
    }
    public static void ResetDefaultKeyMap()
    {
        keyMapping = new Dictionary<KeyType, KeyCode>(defaultMapping);
    }
    public static bool GetKeyDown(KeyType keyType)
    {
        if (!keyMapping.ContainsKey(keyType)) throw new ArgumentException("Cannot Get KeyType " + keyType.ToString() + " Down Due to: KeyType Not in KeyMapping.");
        return Input.GetKeyDown(keyMapping[keyType]);
    }
    public static bool GetKey(KeyType keyType)
    {
        if (!keyMapping.ContainsKey(keyType)) throw new ArgumentException("Cannot Get KeyType " + keyType.ToString() + " Due to: KeyType Not in KeyMapping.");
        return Input.GetKey(keyMapping[keyType]);
    }
    public static bool GetKeyUp(KeyType keyType)
    {
        if (!keyMapping.ContainsKey(keyType)) throw new ArgumentException("Cannot Get KeyType " + keyType.ToString() + " Up Due to: KeyType Not in KeyMapping.");
        return Input.GetKeyUp(keyMapping[keyType]);
    }
}

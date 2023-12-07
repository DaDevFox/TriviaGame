using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public static class Extensions
{
    public static void DestroyChildren(this GameObject obj)
    {
        for (int i = 0; i < obj.transform.childCount; i++)
            GameObject.Destroy(obj.transform.GetChild(i));
    }

    public static void DestroyChildren(this Transform transform)
    {
        for (int i = 0; i < transform.childCount; i++)
            GameObject.Destroy(transform.GetChild(i).gameObject);
    }
}

[Serializable]
public class Player
{
    public int profileId;
    public int id;
    public string name;
    public Color color;

    public int points;
    public Dictionary<string, int> pointsByCategory = new Dictionary<string, int>();
    public Dictionary<string, int> possiblePointsByCategory = new Dictionary<string, int>();
}

public static class Game
{
    public enum MPType
    {
        TurnBased,
        Simultaneous
    }

    public static MPType currentMPType = MPType.TurnBased;

    public static Player[] players = new Player[4];

    /// <summary>
    /// If left null ownership of answers 
    /// </summary>
    public static Player currentLocal;
}


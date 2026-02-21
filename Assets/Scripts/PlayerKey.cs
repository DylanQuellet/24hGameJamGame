using UnityEngine;
using System.Collections.Generic;
public class PlayerKey : MonoBehaviour
{
    private HashSet<KeyColor> keys = new HashSet<KeyColor>();

    public void AddKey(KeyColor color)
    {
        keys.Add(color);
        Debug.Log("Clé obtenue : " + color);
    }

    public bool HasKey(KeyColor color)
    {
        return keys.Contains(color);
    }

    public void ResetKeys()
    {
        keys.Clear();
    }

    public List<KeyColor> GetOwnedKeys()
    {
        return new List<KeyColor>(keys);
    }
}

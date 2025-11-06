using ToolBox.Dev;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using ToolBox.Utils;

public class SecretSantaSelector : MonoBehaviour
{
    [SerializeField, Inline] PersonData[] persons;

    [System.Serializable]
    public struct PersonData
    {
        public string Name;
        public int Mask;
    }

    [Button]
    void ShuffleNames()
    {
        bool valid = false;
        Dictionary<PersonData, PersonData> result = new Dictionary<PersonData, PersonData>();

        while (!valid)
        {
            valid = true;
            result.Clear();

            List<PersonData> givers = persons.ToList();
            List<PersonData> receivers = persons.ToList();

            foreach (var giver in givers)
            {
                var choices = receivers.Where(p => p.Mask != giver.Mask && p.Name != giver.Name).ToList();

                if (choices.Count == 0)
                {
                    valid = false;
                    break; // on recommence tout
                }

                var receiver = choices.GetRandom();
                result[giver] = receiver;
                receivers.Remove(receiver);
            }
        }

        // Affiche le résultat final
        foreach (var pair in result)
        {
            Debug.Log($"<color=cyan>{pair.Key.Name}</color> offre à <color=cyan>{pair.Value.Name}</color>");
        }
    }
}
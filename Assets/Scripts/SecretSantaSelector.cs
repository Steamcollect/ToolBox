using MVsToolkit.Dev;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using MVsToolkit.Utils;

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
        if (persons.Length <= 0) return;

        List<PersonData> personsAvailable = persons.ToList();

        PersonData firstPerson = personsAvailable.GetRandom();
        PersonData currentPerson = firstPerson;

        int loop = 0;
        while(loop < 1000 && personsAvailable.Count > 0)
        {
            loop++;

            personsAvailable.Remove(currentPerson);

            PersonData selected;

            if (personsAvailable.Count == 0) selected = firstPerson;
            else
            {
                PersonData[] availableByMask = personsAvailable.FindAll(c => c.Mask != currentPerson.Mask).ToArray();

                if (availableByMask.Length == 0) availableByMask = personsAvailable.ToArray();

                selected = availableByMask.GetRandom();
            }

            Debug.Log($"{currentPerson.Name}, pour le Secret Santa familial, tu vas devoir offrir un cadeau à : {selected.Name}");

            currentPerson = selected;
        }
    }
}
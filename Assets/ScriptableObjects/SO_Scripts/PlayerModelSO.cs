using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "PlayerModelSO", menuName = "ScriptableObjects/PlayerModelSO", order = 1)]
public class PlayerModelSO : ScriptableObject
{
    [SerializeField] private List<Image> availableMeshAvatar;

    public List<Image> AvailableMeshAvatar
    {
        get => availableMeshAvatar;
    }

    public Image GetMeshByIndex(int index)
    {
        if (index >= 0 && index < availableMeshAvatar.Count)
        {
            return availableMeshAvatar[index];
        }

        return null;
    }
}
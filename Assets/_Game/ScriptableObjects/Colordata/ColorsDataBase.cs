using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newColorsDataBase", menuName = "ScriptableObjects/ColorsDatabase")]
public class ColorsDataBase : ScriptableObject
{
    [SerializeField] private List<Material> _colorsList;

    public Material GetColoredMaterialAtIndex(int index)
    {
        return _colorsList[index];
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretSelection : MonoBehaviour
{
    [SerializeField] private GameObject[] turretPrefab;

    public void Selection(int index)
    {
        if (index < 0 || index >= turretPrefab.Length) return;

        TurretPlacement.SelectedTurretPrefab = turretPrefab[index];
    }
}

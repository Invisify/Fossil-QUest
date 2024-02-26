using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FishData")]
public class FishData : ArtifactData
{
    [SerializeField]
    private string secondName;

    [SerializeField]
    private Fish fish3DModel;

    public Fish Fish3DModel { get => fish3DModel; }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameData")]
public class GameData : ScriptableObject
{
    [SerializeField]
    private int gameType;
    [SerializeField, Range(1,4)]
    private int noOfScreens;

    [SerializeField]
    private ArtifactData[] artifacts;
    [SerializeField]
    private FishData fishData;

    [SerializeField]
    private ArtifactPositionData fishPositionData;
    [SerializeField]
    private ArtifactPositionData[] artifactPositionData;

    public int GameType { get => gameType; }
    public int NoOfScreens { get => noOfScreens; }
    public ArtifactData[] Artifacts { get => artifacts; }
    public FishData FishData { get => fishData; }
    public ArtifactPositionData FishPositionData { get => fishPositionData; }
    public ArtifactPositionData[] ArtifactPositionData { get => artifactPositionData; }
}

[System.Serializable]
public class ArtifactPositionData
{
    [SerializeField]
    private ArtifactData artifactData;
    [SerializeField]
    private Vector3 positionInRect;

    public ArtifactData ArtifactData { get => artifactData; }
    public Vector3 PositionInRect { get => positionInRect; }
}
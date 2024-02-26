using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ArtifactData")]
public class ArtifactData : ScriptableObject
{
    [SerializeField]
    private string name;
    [SerializeField]
    private string description;

    [SerializeField]
    private Sprite artifactHiddenUI;
    [SerializeField]
    private Sprite artifactDiscoveredUI;
    [SerializeField]
    private Sprite artifactSprite;
    [SerializeField]
    private Artifact artifactGameObject;

    public string Name { get => name; }
    public string Description { get => description; }
    public Sprite ArtifactHiddenUI { get => artifactHiddenUI; }
    public Sprite ArtifactDiscoveredUI { get => artifactDiscoveredUI; }
    public Sprite ArtifactSprite { get => artifactSprite; }
    public Artifact ArtifactGameObject { get => artifactGameObject; }
}


using System.Collections.Generic;
using UnityEngine;

public class ScriptableObjectReferences : MonoBehaviour
{
    public static ScriptableObjectReferences Instance;

    [SerializeField] Item AlienArtifact;
    [SerializeField] Item ArtifactScanner;
    [SerializeField] Item Axe;
    [SerializeField] Item Biomass;
    [SerializeField] Item BioProcessor;
    [SerializeField] Item BioReactor;
    [SerializeField] Item GravityReactor;
    [SerializeField] Item Harverster;
    [SerializeField] Item Metal;
    [SerializeField] Item Miner;
    [SerializeField] Item Ore;
    [SerializeField] Item Pickaxe;
    [SerializeField] Item Plant;
    [SerializeField] Item ResearchStation;
    [SerializeField] Item Smelter;

    Dictionary<StorableItemEnum, Item> ResourceObject;
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }

        ResourceObject = new Dictionary<StorableItemEnum, Item>()
        {
            { StorableItemEnum.Alien_Artifact , AlienArtifact },
            { StorableItemEnum.Artifact_Scanner , ArtifactScanner },
            { StorableItemEnum.Axe , Axe },
            { StorableItemEnum.Biomass , Biomass },
            { StorableItemEnum.BioProcessor , BioProcessor },
            { StorableItemEnum.BioReactor , BioReactor },
            { StorableItemEnum.Gravity_Reactor , GravityReactor },
            { StorableItemEnum.Harvester , Harverster },
            { StorableItemEnum.Metal , Metal },
            { StorableItemEnum.Miner , Miner },
            { StorableItemEnum.Ore , Ore },
            { StorableItemEnum.Pickaxe , Pickaxe },
            { StorableItemEnum.Plant , Plant },
            { StorableItemEnum.Research_Station , ResearchStation },
            { StorableItemEnum.Smelter , Smelter },
        };
    }

    public Item GetResourceObject(StorableItemEnum item)
    {
        return ResourceObject[item];
    }
}

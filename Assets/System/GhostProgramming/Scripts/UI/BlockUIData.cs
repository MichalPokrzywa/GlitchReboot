using UnityEngine;
using static Block;

[CreateAssetMenu(fileName = "BlockUIData", menuName = "GhostProgramming/BlockUIData")]
public class BlockUIData : ScriptableObject
{
    [SerializeField] Color performerColor;
    [SerializeField] Color actionColor;
    [SerializeField] Color objectColor;

    public Color GetHeaderColor(BlockType blockType)
    {
        return blockType switch
        {
            BlockType.Performer => performerColor,
            BlockType.Action => actionColor,
            BlockType.Object => objectColor,
            _ => Color.white
        };
    }

    public Color GetInLabelColor(BlockType blockType)
    {
        return blockType switch
        {
            BlockType.Action => performerColor,
            BlockType.Object => actionColor,
            _ => Color.white
        };
    }

    public Color GetOutLabelColor(BlockType blockType)
    {
        return blockType switch
        {
            BlockType.Performer => actionColor,
            BlockType.Action => objectColor,
            BlockType.Object => actionColor,
            _ => Color.white
        };
    }

    public string GetBlockName(BlockType blockType)
    {
        return blockType switch
        {
            BlockType.Performer => "Performer",
            BlockType.Action => "Action",
            BlockType.Object => "Object",
            _ => "Unknown"
        };
    }

}

using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class EntityBase : MonoBehaviour
{
    [Tooltip("If the variable is not set here, it will dynamically take the next valid value when registering entities")]
    public int entityId;
    public string entityName = string.Empty;
    [HideInInspector] public string entityNameSuffix = string.Empty;
    [SerializeField] EntityTooltipInteraction entityTooltipInteraction;

    public virtual void UpdateEntityDisplayName()
    {
        entityTooltipInteraction?.UpdateTooltip(GetEntityName());
    }

    public string GetEntityName()
    {
        return $"{entityName}{entityId} {entityNameSuffix}";
    }
}

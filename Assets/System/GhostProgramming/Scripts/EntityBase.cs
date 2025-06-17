using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class EntityBase : MonoBehaviour
{
    [Tooltip("If the variable is not set here, it will dynamically take the next valid value when registering entities")]
    public int entityId;
    [HideInInspector] public string entityName = string.Empty;
    [HideInInspector] public string entityNameSuffix = string.Empty;
    [SerializeField] Tooltip tooltip;

    public virtual void Start()
    {
        Canvas canvas = tooltip.GetComponentInParent<Canvas>();
        if (canvas.renderMode == RenderMode.WorldSpace)
            canvas.worldCamera = Camera.main;
    }

    public abstract void UpdateEntityDisplayName();

    public string GetEntityName()
    {
        return $"{entityName}{entityId} {entityNameSuffix}";
    }

    public void OnMouseEnter()
    {
        if (tooltip != null )
            tooltip.Activate(GetEntityName());
    }

    public void OnMouseExit()
    {
        if (tooltip != null)
            tooltip.Deactivate();
    }
}

using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Reflection;


public class ResetObject : MonoBehaviour
{
    public enum ResetActionType
    {
        TransformReset,
        MaterialReset,
        ScriptMethodCall,
    }

    [System.Serializable]
    public class ResettableItem
    {
        public ResetActionType actionType;

        // Transform Reset
        [HideInInspector] public Vector3 initialPosition;
        [HideInInspector] public Quaternion initialRotation;
        [HideInInspector] public bool activeOnReset;

        // Material Reset
        public Material originalMaterial;

        // Script Method Call
        public Component targetComponent;
        public string methodName;
        public UnityEvent onResetEvent;
    }

    public List<ResettableItem> itemsToReset = new();

    void Start()
    {
        ResettableItem baseValues = new ResettableItem
        {
            initialPosition = transform.position,
            initialRotation = transform.rotation,
            activeOnReset = gameObject.activeSelf,
            actionType = ResetActionType.TransformReset
        };
        itemsToReset.Clear();
        itemsToReset.Add(baseValues);
    }
    // Called when the script is first added or reset
    void Reset()
    {
        ResettableItem baseValues = new ResettableItem
        {
            initialPosition = transform.position,
            initialRotation = transform.rotation,
            activeOnReset = gameObject.activeSelf,
            actionType = ResetActionType.TransformReset
        };

        itemsToReset.Clear();
        itemsToReset.Add(baseValues);
    }

    public void ResetToInitialState()
    {
        foreach (var item in itemsToReset)
        {

            switch (item.actionType)
            {
                case ResetActionType.TransformReset:

                    transform.position = item.initialPosition;
                    transform.rotation = item.initialRotation;
                    gameObject.SetActive(item.activeOnReset);
                    break;

                case ResetActionType.MaterialReset:
                    if (item.originalMaterial != null)
                    {
                        GetComponent<Renderer>().material = item.originalMaterial;
                    }
                    break;

                case ResetActionType.ScriptMethodCall:
                    if (item.targetComponent != null && !string.IsNullOrEmpty(item.methodName))
                    {
                        MethodInfo method = item.targetComponent.GetType().GetMethod(item.methodName);
                        if (method != null)
                        {
                            method.Invoke(item.targetComponent, null);
                        }
                        item.onResetEvent?.Invoke();
                    }
                    break;
            }
        }
    }
}

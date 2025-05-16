using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class VariableAddedEvent : UnityEvent<string, object> { }

[Serializable]
public class VariableRemovedEvent : UnityEvent<string> { }

public class VariablePlatform : MonoBehaviour
{
    [Header("Identity")]
    [Tooltip("Variable used for terminal")]
    public string variableName = "x_value";

    [Tooltip("Choose the variable type here")]
    public VariableType type;

    private INamedVariableHandler namedHandler;

    [Header("Initial Value")]
    [Tooltip("Starting value if Number")]
    [SerializeField] private int baseNumberValue = 0;
    [Tooltip("Starting value if Boolean")]
    [SerializeField] private bool baseBooleanValue = false;
    [Tooltip("Starting value if GameObject")]
    [SerializeField] private GameObject baseGameObjectValue = null;

    [Header("UI")]
    [SerializeField] private List<TMP_Text> textList = new();

    [Header("Tween Target")]
    [SerializeField] private Transform dicePosition;

    [HideInInspector] public VariableAddedEvent variableAdded = new();
    [HideInInspector] public VariableRemovedEvent variableRemoved = new();
    

    private void Awake()
    {
        // 1) create the core value-handler
        IVariableValueHandler core = type switch
        {
            VariableType.Number => new NumberHandler(baseNumberValue),
            VariableType.Boolean => new BooleanHandler(baseBooleanValue),
            VariableType.GameObject => new GameObjectHandler(baseGameObjectValue),
            _ => throw new Exception($"Unknown type {type}")
        };
        // 2) wrap it in a NamedVariableHandler
        namedHandler = new NamedVariableHandler(
            variableName,
            type,
            core
        );
	EntityManager.instance.Register(gameObject);
    }

    private void Start()
    {
        // update UI to show displayName
        foreach (var t in textList)
        {
            t.text = namedHandler.VariableName;
        }
        var rend = GetComponentInChildren<Renderer>();
        if (rend != null)
            rend.material.color = VariableTypeColor.GetColor(type);
    }

    public void ReceiveValue(object v)
    {
        if (!namedHandler.Accepts(v))
            return;
        namedHandler.UpdateValue(v);
        variableAdded.Invoke(
            namedHandler.VariableName,
            namedHandler.GetValue()
        );
        namedHandler.UpdateHighlight(textList, true);
    }

    public void ClearValue()
    {
        namedHandler.ResetToBase();
        variableRemoved.Invoke(namedHandler.VariableName);
        namedHandler.UpdateHighlight(textList, false);
    }

    public INamedVariableHandler GetHandler()
    {
        return namedHandler;
    }

    public void MoveObjectToPosition(GameObject go)
    {
        var rb = go.GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        go.transform.DOMove(dicePosition.position, 0.3f)
          .OnComplete(() =>
          {
              rb.linearVelocity = Vector3.zero;
              rb.angularVelocity = Vector3.zero;
              rb.useGravity = true;
          });
    }
}

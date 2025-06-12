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

public class VariablePlatform : VariablePlatformBase
{
    [Header("Identity")]
    [Tooltip("Variable used for terminal")]
    public string variableName = "x_value";

    private INamedVariableHandler namedHandler;

    [Header("Initial Value")]
    [Tooltip("Starting value if Number")]
    [SerializeField] private int baseNumberValue = 0;
    [Tooltip("Starting value if Boolean")]
    [SerializeField] private bool baseBooleanValue = false;
    [Tooltip("Starting value if String")]
    [SerializeField] private string baseStringValue = "";
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
            VariableType.String => new StringHandler(baseStringValue),
            VariableType.GameObject => new GameObjectHandler(baseGameObjectValue),
            _ => throw new Exception($"Unknown type {type}")
        };
        // 2) wrap it in a NamedVariableHandler
        namedHandler = new NamedVariableHandler(
            variableName,
            type,
            core
        );
    }

    public override void Start()
    {
        base.Start();
        // update UI to show displayName
        foreach (var t in textList)
        {
            t.text = namedHandler.VariableName;
        }

        UpdateEntityNameSuffix();

        var rend = GetComponentInChildren<Renderer>();
        if (rend != null)
            rend.material.color = VariableTypeColor.GetColor(type);
    }

    public override void ReceiveValue(object v)
    {
        if (!namedHandler.Accepts(v))
            return;
        if(assignedObject != null)
            return;

        namedHandler.UpdateValue(v);
        variableAdded.Invoke(
            namedHandler.VariableName,
            namedHandler.GetValue()
        );
        namedHandler.UpdateHighlight(textList, true);
    }

    public override void ClearValue()
    {
        namedHandler.ResetToBase();
        variableRemoved.Invoke(namedHandler.VariableName);
        assignedObject = null;
        namedHandler.UpdateHighlight(textList, false);
    }

    public INamedVariableHandler GetHandler()
    {
        return namedHandler;
    }

    public override void MoveObjectToPosition(GameObject go)
    {
        if (!ContainsObject())
            return;

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

    public override void AssignObjectToPlatform(GameObject go)
    {
        if (assignedObject == null)
        {
            assignedObject = go;
        }
    }

    private bool ContainsObject()
    {
        return assignedObject != null;
    }
    public override void UpdateEntityNameSuffix()
    {
        entityNameSuffix = variableName;
    }
}

using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine;

public enum VariableType
{
    Number = 0,
    Boolean = 1,
    GameObject = 2      // example of adding a 3rd type
}

public interface IVariableValueHandler
{
    void UpdateValue(object v);
    object GetValue();
    void ResetToBase();
    bool Accepts(object v);
    void UpdateTextValue(TMP_Text text, bool highlight);
    void UpdateAllText(IEnumerable<TMP_Text> texts, bool highlight);
}
public interface INamedVariableHandler : IVariableValueHandler
{
    string VariableName { get; }

    VariableType Type { get; }

    public void UpdateHighlight(IEnumerable<TMP_Text> texts, bool highlight);
}
public class NamedVariableHandler : INamedVariableHandler
{
    private readonly IVariableValueHandler inner;
    public string VariableName { get; }
    public VariableType Type { get; }

    public NamedVariableHandler(
        string variableName,
        VariableType type,
        IVariableValueHandler handler)
    {
        VariableName = variableName;
        Type = type;
        inner = handler;
    }

    public void UpdateValue(object v) => inner.UpdateValue(v);
    public object GetValue() => inner.GetValue();
    public void ResetToBase() => inner.ResetToBase();
    public bool Accepts(object v) => inner.Accepts(v);
    public void UpdateTextValue(TMP_Text t, bool h) =>
        inner.UpdateTextValue(t, h);
    public void UpdateAllText(IEnumerable<TMP_Text> ts, bool h) =>
        inner.UpdateAllText(ts, h);

    public void UpdateHighlight(IEnumerable<TMP_Text> texts, bool highlight)
    {
        foreach (var t in texts)
            t.color = highlight ? Color.green : Color.white;
    }

}

public abstract class VariableTypeHandlerBase<T> : IVariableValueHandler
{
    protected T currentValue;
    protected T baseValue;

    protected VariableTypeHandlerBase(T baseValue)
    {
        this.baseValue = baseValue;
        this.currentValue = baseValue;
    }

    public virtual void UpdateValue(object v) =>
        currentValue = (T)Convert.ChangeType(v, typeof(T));

    public object GetValue() => currentValue;

    public virtual void ResetToBase() =>
        currentValue = baseValue;

    public virtual bool Accepts(object v) =>
        v is T;

    public virtual void UpdateTextValue(TMP_Text text, bool highlight)
    {
        text.text = currentValue?.ToString() ?? "null";
        text.color = highlight ? Color.green : Color.white;
    }

    public void UpdateAllText(IEnumerable<TMP_Text> texts, bool highlight)
    {
        foreach (var t in texts)
            UpdateTextValue(t, highlight);
    }
}

public class NumberHandler : VariableTypeHandlerBase<int>
{
    public NumberHandler(int baseValue) : base(baseValue) { }
}

public class BooleanHandler : VariableTypeHandlerBase<bool>
{
    public BooleanHandler(bool baseValue) : base(baseValue) { }

    public override void UpdateTextValue(TMP_Text text, bool highlight)
    {
        text.text = currentValue ? "True" : "False";
        base.UpdateTextValue(text, highlight);
    }
}

public class GameObjectHandler : VariableTypeHandlerBase<GameObject>
{
    public GameObjectHandler(GameObject baseValue) : base(baseValue) { }

    public override void UpdateTextValue(TMP_Text text, bool highlight)
    {
        text.text = currentValue != null ? currentValue.name : "None";
        base.UpdateTextValue(text, highlight);
    }
}
public static class VariableTypeColor
{
    /// <summary>
    /// Returns the UnityEngine.Color associated with this VariableType.
    /// </summary>
    public static Color GetColor(VariableType type)
    {
        return type switch
        {
            VariableType.Number => new Color(46f/255f, 153f / 255f, 28f / 255f),
            VariableType.Boolean => Color.cyan,
            VariableType.GameObject => Color.cyan,
            _ => Color.white
        };
    }

    /// <summary>
    /// Returns the TMP‐friendly hex code (“#RRGGBB”) for this VariableType.
    /// </summary>
    public static string GetHex(VariableType type)
    {
        var c = GetColor(type);
        return "#" + ColorUtility.ToHtmlStringRGB(c);
    }
}
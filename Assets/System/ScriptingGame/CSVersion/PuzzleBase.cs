using MoonSharp.Interpreter;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;

public class PuzzleBase : MonoBehaviour
{
    [SerializeField] private TerminalAssignedObjects assignedObjects;
    [SerializeField] private TerminalCanvas canvas;
    [SerializeField] protected List<VariablePlatform> allLevelPlatforms = new();

    [Tooltip("If true, Puzzle Code will be called every frame.")]
    [SerializeField] private bool continuousExecution = false;

    [TextArea(5, 10)]
    [Tooltip("Use {var} for value, {var.name} for display name")]
    [SerializeField]
    public string naturalText = "";
    [TextArea(5, 10)]
    [Tooltip("Use {var} for value, {var.name} for display name")]
    [SerializeField]
    public string codeText = "";

    protected List<INamedVariableHandler> namedHandlers = new();
    public UnityEvent<string, string> onCodeUpdate;

    void Start()
    {
        foreach (VariablePlatform plat in allLevelPlatforms)
        {
            INamedVariableHandler h = plat.GetHandler();

            namedHandlers.Add(h);

            plat.variableAdded.AddListener((n, v) =>
            {
                canvas.SetCodeText(UpdateTemplate(codeText));
                canvas.SetNeutralText(UpdateTemplate(naturalText));
                onCodeUpdate.Invoke(UpdateTemplate(codeText), UpdateTemplate(naturalText));
                DoTerminalCode();
            });
            plat.variableRemoved.AddListener(n =>
            {
                canvas.SetCodeText(UpdateTemplate(codeText));
                canvas.SetNeutralText(UpdateTemplate(naturalText));
                onCodeUpdate.Invoke(UpdateTemplate(codeText), UpdateTemplate(naturalText));
                DoTerminalCode();
            });
        }
        canvas.SetCodeText(UpdateTemplate(codeText));
        canvas.SetNeutralText(UpdateTemplate(naturalText));
        //DoTerminalCode();
    }

    protected virtual void Update()
    {
        if (continuousExecution)
            DoTerminalCode();
    }

    public string UpdateTemplate(string template)
    {
        var pattern = new Regex(@"\{(\w+?)(?:\.(\w+))?\}");
        return pattern.Replace(template, match =>
        {
            var key = match.Groups[1].Value;                    // e.g. "platformX"
            var prop = match.Groups[2].Success ? match.Groups[2].Value : "value";                   // "name" or null;

            var handler = namedHandlers.FirstOrDefault(h => h.VariableName == key);
            if (handler == null)
                return match.Value;

            string hex;
            switch (prop)
            {
                case "name":
                    hex = VariableTypeColor.GetHex(handler.Type);
                    return $"<color={hex}>{handler.VariableName}</color>";

                case "value":
                    var raw = handler.GetValue()?.ToString() ?? "null";
                    hex = VariableTypeColor.GetHex(handler.Type);
                    return $"<color={hex}>{raw}</color>";

                default:
                    return match.Value;
            }
        });
    }

    public virtual void DoTerminalCode()
    {
        Debug.LogError("This is base implementation, you need create a DoTerminalCode function in file");
    }

    public void SendToTablet()
    {
        TabletTerminal.Instance.AssignTerminal(this);
        TabletTerminal.Instance.SendText(UpdateTemplate(codeText), UpdateTemplate(naturalText));
    }

    public void SwapLanguage()
    {
        canvas.ChangeTextType(UpdateTemplate(codeText), UpdateTemplate(naturalText));
    }

    #region Utility

    /// <summary>
    /// Returns the current value of the named variable, or null if it doesn't exist.
    /// </summary>
    protected object GetVariableValue(string variableName)
    {
        var h = namedHandlers
            .FirstOrDefault(x => x.VariableName == variableName);
        return h?.GetValue();
    }

    /// <summary>
    /// Generic version to save you casting.
    /// </summary>
    protected T GetVariableValue<T>(string variableName)
    {
        var obj = GetVariableValue(variableName);
        return obj is T t ? t : default;
    }
    /// <summary>
    /// Let derived classes (or UI buttons) turn continuous mode on/off at runtime.
    /// </summary>
    protected void SetContinuousExecution(bool on)
    {
        continuousExecution = on;
    }

    #endregion


}

using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GhostProgramming
{
    public interface IEntityNode
    {
        EntityBase GetEntity();
    }

    public abstract class EntityNode<T> : ArgumentNode<T>, IEntityNode where T : EntityBase
    {
        // NOTE: If we would want to store e.g. primitive types this class should be factored out and there should be a separate class named "DropdownArgumentNode"
        [SerializeField] TMP_Dropdown dropdown;

        List<Toggle> toggles = new List<Toggle>();

        void Start()
        {
            if (dropdown != null)
                InitDropdown();
        }

        public virtual void Update()
        {
            if (dropdown != null)
            {
                HandleDropdownState();

                if (dropdown.IsExpanded)
                    HandleDropdownContent();
            }
        }

        protected abstract List<T> GetEntityList();

        public override T GetValue()
        {
            return GetEntityList()[dropdown.value];
        }

        public EntityBase GetEntity()
        {
            return GetValue();
        }

        void InitDropdown()
        {
            dropdown.options.Clear();
            var list = GetEntityList();
            for (int i = 0; i < list.Count; i++)
            {
                var label = list[i].GetEntityName();
                dropdown.options.Add(new TMP_Dropdown.OptionData(label));
            }
            dropdown.RefreshShownValue();
        }

        void HandleDropdownContent()
        {
            // disable deactivated entities
            toggles = dropdown.transform.GetComponentsInChildren<Toggle>().ToList();
            var list = GetEntityList();
            for (int i = 0; i < list.Count; i++)
            {
                DropdownOptionActivationIndicator(i, list[i].gameObject.activeSelf);
            }
        }

        void DropdownOptionActivationIndicator(int index, bool active)
        {
            toggles[index].interactable = active;
            var label = toggles[index].GetComponentInChildren<TextMeshProUGUI>();
            if (label != null)
                label.color = active ? Color.white : Color.gray;
        }

        void HandleDropdownState()
        {
            var list = GetEntityList();

            if (list == null || list.Count == 0)
            {
                UpdateDropdownState(false);
                return;
            }

            RefreshDropdownIfChanged();

            // check if current value is valid
            if (dropdown.value < 0 || dropdown.value >= list.Count)
            {
                dropdown.value = 0;
                dropdown.RefreshShownValue();
            }

            // check if selected object is active in hierarchy and set interact ability accordingly
            if (list[dropdown.value].gameObject.activeSelf)
            {
                UpdateDropdownState(true);
                return;
            }

            // if it is deactivated, find and assign the first active object or mark dropdown invalid if none are active
            int firstActiveIndex = -1;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].gameObject.activeSelf)
                {
                    firstActiveIndex = i;
                    break;
                }
            }
            if (firstActiveIndex == -1)
            {
                UpdateDropdownState(false);
            }
            else
            {
                UpdateDropdownState(true);
                dropdown.value = firstActiveIndex;
                dropdown.RefreshShownValue();
            }
        }

        void UpdateDropdownState(bool isActive)
        {
            if (!isActive)
                dropdown.Hide();

            dropdown.interactable = isActive;
            this.isValid = isActive;
        }

        void RefreshDropdownIfChanged()
        {
            var list = GetEntityList();

            if (list == null)
                return;

            // if list changed, reinitialize the dropdown
            if (list.Count != dropdown.options.Count)
            {
                InitDropdown();
                return;
            }

            // if labels changed, reinitialize the dropdown
            for (int i = 0; i < list.Count; i++)
            {
                var currentLabel = list[i].GetEntityName();
                if (dropdown.options[i].text != currentLabel)
                {
                    InitDropdown();
                    return;
                }
            }
        }
    }
}
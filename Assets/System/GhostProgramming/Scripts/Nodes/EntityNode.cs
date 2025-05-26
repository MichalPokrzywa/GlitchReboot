using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GhostProgramming
{
    public abstract class EntityNode<T> : ArgumentNode where T : Object
    {
        // NOTE: This class should be factored out and there should be a separate class named "DropdownArgumentNode"
        // cause not all entity nodes need to have a dropdown
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
        protected abstract string GetEnityName();

        public override Object GetValue()
        {
            return GetEntityList()[dropdown.value];
        }

        void InitDropdown()
        {
            dropdown.options.Clear();
            var list = GetEntityList();
            for (int i = 0; i < list.Count; i++)
            {
                string objName = $"{GetEnityName()} {i + 1}";
                dropdown.options.Add(new TMP_Dropdown.OptionData(objName));
            }
        }

        void HandleDropdownContent()
        {
            // disable deactivated entities
            toggles = dropdown.transform.GetComponentsInChildren<Toggle>().ToList();
            var list = GetEntityList();
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] is MonoBehaviour mb)
                {
                    DropdownOptionActivationIndicator(i, mb.gameObject.activeSelf);
                }
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

            // check if selected object is active in hierarchy and set interactability accordingly
            if (list[dropdown.value] is MonoBehaviour monoB && monoB.gameObject.activeSelf)
            {
                dropdown.interactable = true;
                this.isValid = true;
                return;
            }

            // if it is deactivated, find the first active object or hide the dropdown
            int firstActiveIndex = -1;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] is MonoBehaviour mb && mb.gameObject.activeSelf)
                {
                    firstActiveIndex = i;
                    break;
                }
            }
            if (firstActiveIndex == -1)
            {
                dropdown.Hide();
                dropdown.interactable = false;
                this.isValid = false;
            }
            else
            {
                dropdown.interactable = true;
                this.isValid = true;
                dropdown.value = firstActiveIndex;
                dropdown.RefreshShownValue();
            }
        }
    }
}
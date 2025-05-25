using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace GhostProgramming
{
    public abstract class EntityNode<T> : ArgumentNode where T : Object
    {
        [SerializeField] TMP_Dropdown dropdown;

        void Start()
        {
            if (dropdown != null)
                InitDropdown();
        }

        public virtual void Update()
        {
            if (dropdown != null)
                HandleDropdown();
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

        void HandleDropdown()
        {
            // erase deactivated entities

            // add active entities

        }
    }
}
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
            dropdown.options.Clear();
            var list = GetEntityList();
            for (int i = 0; i < list.Count; i++)
            {
                string objName = $"{GetEnityName()} {i + 1}";
                dropdown.options.Add(new TMP_Dropdown.OptionData(objName));
            }
        }
        protected abstract List<T> GetEntityList();
        protected abstract string GetEnityName();

        public override Object GetValue()
        {
            return GetEntityList()[dropdown.value];
        }
    }
}
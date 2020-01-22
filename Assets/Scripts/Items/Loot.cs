using UnityEngine;

namespace Assets.Scripts
{
    public class Loot : MonoBehaviour
    {
        public int item;
        public int index;

        public void UpdateColor()
        {
            var spellManager = GameObject.Find("SpellManager").GetComponent<SpellManager>();
            var color = spellManager.getSpell(item).hotBarColor;
            GetComponent<Renderer>().material.SetColor("_EmissionColor", color);
        }
    }
}
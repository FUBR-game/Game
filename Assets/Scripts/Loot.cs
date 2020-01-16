using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class Loot: MonoBehaviour
    {
        Renderer renderer;
        float color;

        public void Awake()
        {
            color = UnityEngine.Random.value;
            renderer = GetComponent<Renderer>();
        }

        public void Update()
        {
            renderer.material.SetColor("_EmissionColor", Color.HSVToRGB(color, 0.8f, 0.8f));
            color += 0.01f;
            color %= 1.0f;
            transform.Rotate(new Vector3(0, 1f, 0));
        }
    }
}

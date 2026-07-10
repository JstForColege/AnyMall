
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Monobehavior_Script
{
    public class ProducerOutput : MonoBehaviour
    {
        public Producer producer;
        private void Awake()
        {
            producer = GetComponentInParent<Producer>();
        }
    }
}

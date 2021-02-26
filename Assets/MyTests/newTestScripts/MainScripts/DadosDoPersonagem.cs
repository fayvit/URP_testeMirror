using UnityEngine;
using System.Collections;

namespace MyTestMirror
{
    [System.Serializable]
    public class DadosDoPersonagem
    {
        [SerializeField] private int lifePoints=100;
        [SerializeField] private int maxLifePoints=100;
        [SerializeField] private StaminaManager stManager;

        public int LifePoints { get => lifePoints; }
        public int MaxLifePoints { get => maxLifePoints; }
        public StaminaManager StManager { get => stManager; }


        public void ApplyDamage(uint val)
        {
            lifePoints = Mathf.Max(lifePoints - (int)val, 0);
        }

        public void RestoreLifePoints(uint val)
        {
            lifePoints = Mathf.Min(lifePoints + (int)val, maxLifePoints);
        }
    }
}

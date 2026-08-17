using UnityEngine;

namespace AgeOfHistory.Simulation
{
    /// <summary>
    /// Avance la simulation par ticks fixes (ex. 20 ticks/seconde), indépendamment du framerate
    /// de rendu. C'est ce compteur de tick — pas Time.deltaTime — qui doit être utilisé par
    /// toute logique de simulation, pour garantir que tous les clients avancent au même rythme
    /// logique quelle que soit leur machine.
    /// </summary>
    public class SimulationClock : MonoBehaviour
    {
        [SerializeField] private int ticksPerSecond = 20;

        private float _accumulator;
        public int CurrentTick { get; private set; }

        /// <summary>Déclenché une fois par tick de simulation (pas une fois par frame de rendu).</summary>
        public event System.Action<int> OnSimulationTick;

        private void Update()
        {
            float fixedStep = 1f / ticksPerSecond;
            _accumulator += Time.deltaTime;

            while (_accumulator >= fixedStep)
            {
                _accumulator -= fixedStep;
                CurrentTick++;
                OnSimulationTick?.Invoke(CurrentTick);
            }
        }
    }
}

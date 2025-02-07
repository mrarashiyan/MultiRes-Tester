using UnityEngine;

namespace MultiResTester.Data
{
    [CreateAssetMenu(fileName = "MultiResTester Screen Config", menuName = "MultiResTester/New Screen Config", order = 0)]
    public class MultiResScreenConfig : ScriptableObject
    {
        public SingleScreenConfig[] configs;
        public SingleOverlayConfig[] overlays;
    }
}
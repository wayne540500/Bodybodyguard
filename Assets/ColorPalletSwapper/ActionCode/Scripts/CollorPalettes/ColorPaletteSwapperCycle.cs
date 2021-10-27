using UnityEngine;
using UnityEngine.InputSystem;

namespace ActionCode.ColorPalettes
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(ColorPaletteSwapper))]
    public sealed class ColorPaletteSwapperCycle : MonoBehaviour
    {
        public KeyCode swapKey = KeyCode.N;
        public Key swapKey1;
        public ColorPaletteSwapper swapper;
        public ColorPalette[] palettes;

        private int _palletIndex = -1;

        private void Reset()
        {
            swapper = GetComponent<ColorPaletteSwapper>();
        }

        private void Update()
        {
            //if (Input.GetKeyDown(swapKey)) SwapPalette();
            if(Keyboard.current[swapKey1].wasPressedThisFrame)
                SwapPalette();
        }

        private void SwapPalette()
        {
            if (palettes.Length == 0) return;

            _palletIndex = (_palletIndex + 1) % palettes.Length;
            swapper.SwitchPalette(palettes[_palletIndex]);
        }
    }
}
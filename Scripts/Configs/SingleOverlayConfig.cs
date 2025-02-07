using UnityEngine;

namespace MultiResTester.Data
{
    [System.Serializable]
    public class SingleOverlayConfig
    {
        public enum OverlayPosition
        {
            TopLeft,TopMiddle,TopRight,
            CenterLeft,CenterMiddle,CenterRight,
            BottomLeft,BottomMiddle,BottomRight,
        }
        
        public enum SizeUnit
        {
            Percent,Pixel
        }
        
        public bool isEnabled = true;
        public string overlayName = "Banner"; 
        public Color overlayColor = Color.red; 

        [Space]
        public OverlayPosition overlayPosition;
        
        [Space]
        public int overlayWidth = 320;
        public SizeUnit widthSizeUnit;
        
        [Space]
        public int overlayHeight = 50;
        public SizeUnit heightSizeUnit;
        
    }
}
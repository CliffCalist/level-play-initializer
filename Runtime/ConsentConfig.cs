namespace WhiteArrow
{
    public class ConsentConfig
    {
        public bool IsFamilyDirected;
        public bool IsChildeDirected;

        public IConsentConfirmer ConsentConfirmer;
        public IAgeOver18Confirmer AgeGroupConfirmer;
    }
}
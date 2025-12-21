using System.Threading.Tasks;

namespace WhiteArrow
{
    public interface IAgeOver18Confirmer
    {
        Task<bool> Confirm();
    }
}
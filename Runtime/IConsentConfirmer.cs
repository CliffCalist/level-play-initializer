using System.Threading.Tasks;

namespace WhiteArrow
{
    public interface IConsentConfirmer
    {
        Task<bool> Confirm();
    }
}
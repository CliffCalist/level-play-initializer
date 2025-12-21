using System.Threading.Tasks;
using UnityEngine;

namespace WhiteArrow
{
    public abstract class ConsentConfirmer : MonoBehaviour
    {
        internal protected abstract Task<bool> Confirm();
    }
}
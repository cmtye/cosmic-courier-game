using UnityEngine;
using UnityEngine.UI;

namespace Enemy_Scripts
{
    public class EnemyHealthContainer : MonoBehaviour
    {
        [SerializeField] private Image fillAmountImage;
        public Image FillAmountImage => fillAmountImage;
    }
}

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

// this namespace makes the magic, tho
 
namespace UX
{
    public class ObjectButtonBehavior : 
        MonoBehaviour, 
        IPointerClickHandler, 
        IPointerEnterHandler,
        IPointerExitHandler {

        [SerializeField] UnityEvent buttonEvent;
        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log("Here");
            //buttonEvent.Invoke();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Debug.Log("Over");
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            Debug.Log("Off");
        }
    }
}

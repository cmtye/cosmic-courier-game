using UnityEngine;
using System.Collections;

namespace Utility.Interaction
{
    public class DepotHandler : InteractionHandler
    {

        private PlayerController _playerController;
        private GameManager _manager;

        void Start()
        {
            _playerController =  GameObject.Find("Player").GetComponent<PlayerController>();
            _manager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        }

        public override GameObject Handle()
        {
            if(_playerController.currentlyHeld?.CompareTag("Item") == true)
            {
                // Take from the player 
                var item = _playerController.TakeHeldItem(gameObject);

                // Increment deposit count by item value
                var value = item.GetComponent<ItemController>().GetValue();
                _manager.Deposit(value);
                Destroy(item, 1);
            }
            return null;
        }



    }
}
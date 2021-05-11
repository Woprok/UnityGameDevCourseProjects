using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts
{
    public class PlayerController : MonoBehaviour
    {
        private PlayerInventory Inventory;
        public TextMeshProUGUI ScoreUI;
        public GameObject Endgame;

        private Rigidbody rigidbody;
        private float movementX;
        private float movementY;
        private float movementZ = 0;

        public float Speed = 0;

        // Start is called before the first frame update
        void Start()
        {
            Inventory = GetComponent<PlayerInventory>();
            rigidbody = GetComponent<Rigidbody>();
            Endgame.SetActive(false);
            Inventory.ObjectCount = GameObject.FindGameObjectsWithTag("Collectable").Length;
        }

        // Fixed Update is called before physics recalculation
        void FixedUpdate()
        {
            rigidbody.AddForce(new Vector3(movementX, movementZ, movementY) * Speed);
        }

        void OnMove(InputValue movementValue)
        {
            Vector2 movementVector2 = movementValue.Get<Vector2>();

            movementX = movementVector2.x;
            movementY = movementVector2.y;
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Collectable"))
            {
                other.gameObject.SetActive(false);

                if (other.name.Contains("Purple"))
                {
                    Inventory.PurpleCount++;
                }
                if (other.name.Contains("Gold"))
                {
                    Inventory.GoldCount++;
                }
                SetScoreUI();
            }
        }

        void SetScoreUI()
        {
            ScoreUI.text = $"Current Score: {Inventory.CalculateScore()} due to: Gold -> {Inventory.GoldCount}, Purple -> {Inventory.PurpleCount}";

            if (Inventory.ItemsLeft() == 0 && Inventory.CalculateScore() >= Inventory.WinScore)
            {
                Endgame.SetActive(true);
                Endgame.GetComponent<TextMeshProUGUI>().text = $"You reached minimum score for victory. Congrats!";
                GameObject.FindGameObjectWithTag("Player").SetActive(false);
            }
            else if (Inventory.ItemsLeft() == 0 && Inventory.CalculateScore() < Inventory.WinScore)
            {
                Endgame.SetActive(true);
                Endgame.GetComponent<TextMeshProUGUI>().text = $"You failed to reached minimum score. Congrats and try again!";
                GameObject.FindGameObjectWithTag("Player").SetActive(false);
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinPickup : MonoBehaviour {
	[SerializeField] AudioClip coinPickupSfX;
	[SerializeField] int pointsForCoinPickup = 100;

	bool wasCollected = false;

	private void OnTriggerEnter2D(Collider2D other) {
		if (other.tag == "Player" && !wasCollected) {
			wasCollected = true;
			FindObjectOfType<GameSession>().AddToScore(pointsForCoinPickup);
			AudioSource.PlayClipAtPoint(coinPickupSfX, Camera.main.transform.position);
			Destroy(gameObject);
		}

	}
}

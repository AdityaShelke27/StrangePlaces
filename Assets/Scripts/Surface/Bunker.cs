using UnityEngine;
using UnityEngine.SceneManagement;

public class Bunker : MonoBehaviour
{
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if(collision.CompareTag("Player"))
		{
			SceneManager.LoadScene("BunkerScene");
		}
	}
}

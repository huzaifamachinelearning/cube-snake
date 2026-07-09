using UnityEngine;
using TMPro;

public class HUDManager : MonoBehaviour
{
    [Header("Score Display")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI chainCountText;

    void Update()
    {
        if (GameManager.Instance != null && scoreText != null)
        {
            scoreText.text = "Score: " + GameManager.Instance.GetScore();
        }

        PlayerMovement player = FindObjectOfType<PlayerMovement>();
        if (player != null && chainCountText != null)
        {
            chainCountText.text = "Chain: " + player.chain.Count;
        }
    }
}

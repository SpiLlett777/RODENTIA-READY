using UnityEngine;

public class Loot : MonoBehaviour
{
    public GameObject coinPrefab;
    public int minCoins = 1;
    public int maxCoins = 5;

    public void DropCoins(Vector3 enemyPosition)
    {
        int coinsToDrop = Random.Range(minCoins, maxCoins + 1);

        for (int i = 0; i < coinsToDrop; i++)
        {
            Vector3 randomOffset = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f);
            Vector3 spawnPosition = enemyPosition + randomOffset;

            GameObject coin = Instantiate(coinPrefab, spawnPosition, Quaternion.identity);
            Rigidbody2D coinRigidBody = coin.GetComponent<Rigidbody2D>();
            if (coinRigidBody != null )
            {
                coinRigidBody.AddForce(new Vector2(Random.Range(-5f, 5f), Random.Range(2f, 5f)), ForceMode2D.Impulse);
            }
        }
    }
}


using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class HealthBehaviourTests
{
    [UnityTest]
    public IEnumerator ApplyDamage_UpdatesHealthInPlayMode()
    {
        // Arrange: Create a GameObject with the health component.
        GameObject gameObject = new GameObject("TestHealthObject");

        try
        {
            HealthBehaviour healthBehaviour =
                gameObject.AddComponent<HealthBehaviour>();

            // Wait one frame so Unity runs Awake().
            yield return null;

            // Assert: Health should initialize to its starting value.
            Assert.IsNotNull(healthBehaviour.Health);
            Assert.AreEqual(
                100,
                healthBehaviour.Health.CurrentHealth,
                "Health should initialize to 100."
            );

            // Act: Apply 25 damage.
            healthBehaviour.ApplyDamage(25);

            // Assert: Health should decrease to 75.
            Assert.AreEqual(
                75,
                healthBehaviour.Health.CurrentHealth,
                "Health should be 75 after taking 25 damage."
            );

            // Act: Apply lethal damage.
            healthBehaviour.ApplyDamage(100);

            // Assert: Health should reach zero.
            Assert.AreEqual(
                0,
                healthBehaviour.Health.CurrentHealth,
                "Health should not go below zero."
            );

            Assert.IsTrue(
                healthBehaviour.Health.IsDead,
                "The health system should report that the character is dead."
            );
        }
        finally
        {
            // Cleanup: Destroy the temporary GameObject.
            Object.Destroy(gameObject);
        }

        // Allow Unity to process destruction.
        yield return null;
    }
}

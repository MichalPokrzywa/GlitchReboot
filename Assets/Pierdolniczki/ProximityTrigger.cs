using UnityEngine;

public class ProximityTrigger : MonoBehaviour
{
    // Zasięg, w którym wykrywany jest obiekt
    public float detectionRadius = 5f;

    // Tag obiektu, który chcemy wykrywać
    public string targetTag = "Player";

    // Zdarzenie do uruchomienia
    public UnityEngine.Events.UnityEvent onObjectEnter;

    private void OnDrawGizmosSelected()
    {
        // Rysuje wizualne koło w edytorze pokazujące zasięg detekcji
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }

    void Update()
    {
        // Wyszukiwanie obiektów w zasięgu
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius);
        foreach (var collider in hitColliders)
        {
            // Sprawdzenie, czy obiekt ma odpowiedni tag
            if (collider.CompareTag(targetTag))
            {
                // Uruchomienie zdarzenia
                onObjectEnter.Invoke();

                // Opcjonalnie: przerwanie po pierwszym wykryciu
                return;
            }
        }
    }
}

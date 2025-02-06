using UnityEngine;
using UnityEngine.Events;

/*
    PRZYKŁAD UŻYCIA:

    1. Dodaj skrypt `CustomTrigger` do obiektu posiadającego Collider z włączonym `IsTrigger`.
    2. W innym skrypcie zadeklaruj zmienną referencyjną do `CustomTrigger`, np.:

       public CustomTrigger attackRangeTrigger;

    3. W metodzie Awake lub Start podłącz funkcję do eventu `EnteredTrigger`:

       void Awake()
       {
           attackRangeTrigger.EnteredTrigger.AddListener(OnAttackRangeTriggerEntered);
       }

    4. Zaimplementuj metodę obsługującą zdarzenie wejścia do triggera:

       void OnAttackRangeTriggerEntered(Collider collider)
       {
           Debug.Log("Hey, I entered the attack range! - says " + collider.name);
       }

    5. Kiedy obiekt wejdzie w trigger, wywoła się metoda `OnAttackRangeTriggerEntered`.

    Pamiętaj, że `CustomTrigger` może być użyty także do wyjścia z triggera
    poprzez event `ExitedTrigger`, podłączając do niego własną metodę analogicznie do powyższego przykładu.
*/


public class CustomTrigger : MonoBehaviour
{
    public UnityEvent<Collider> EnteredTrigger;
    public UnityEvent<Collider> ExitedTrigger;

    void OnTriggerEnter(Collider collider)
    {
        EnteredTrigger?.Invoke(collider);
    }
    void OnTriggerExit(Collider collider)
    {
        ExitedTrigger?.Invoke(collider);
    }
}


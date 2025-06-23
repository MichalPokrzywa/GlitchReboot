using UnityEngine;

public class CannonController : InteractionBase
{
    public GameObject player; // Referencja do obiektu gracza
    public Transform firePoint; // Punkt, z którego gracz ma zostać wystrzelony

    [Header("Ustawienia wystrzału gracza")]
    public float initialSpeed = 20f;     // Prędkość początkowa gracza
    public float launchAngleDegrees = 0.0f; // Kąt wystrzału w stopniach (w górę od firePoint.forward)

    // Zmienne do kontroli lotu parabolicznego
    private bool isPlayerFlying = false; // Flaga, czy gracz jest w trakcie lotu
    private Vector3 flightInitialPosition;
    private Vector3 flightInitialVelocity;
    private float flightStartTime;

    // Referencje do komponentów gracza, które trzeba będzie kontrolować
    private Rigidbody playerRigidbody;
    // Opcjonalnie: referencja do skryptu kontroli gracza (np. PlayerController), jeśli chcesz go wyłączać
    // private PlayerController playerMovementController;

    protected override void Start()
    {
        base.Start();
        TooltipText = "[E] " + "FIRE PLAYER"; // Zmieniamy tekst Tooltipa

        // Spróbuj pobrać Rigidbody gracza raz na początku
        if (player != null)
        {
            playerRigidbody = player.GetComponent<Rigidbody>();
            // Jeśli gracz ma też np. PlayerController, pobierz go tutaj:
            // playerMovementController = player.GetComponent<PlayerController>();
        }
        else
        {
            Debug.LogError("Player object is not assigned in CannonController!", this);
        }
    }

    public override void Update()
    {
        base.Update();

        // Wizualizacja kierunku strzału w edytorze Unity
        Debug.DrawRay(firePoint.position, GetAngledDirection() * 5, Color.red, 0.1f);

        // Jeśli gracz jest w trakcie lotu, aktualizuj jego pozycję
        if (isPlayerFlying)
        {
            UpdatePlayerFlight();
        }
    }

    public override void Interact()
    {
        Debug.Log("Interakcja: E naciśnięte. Próba wystrzelenia gracza.");

        // Sprawdź, czy gracz nie trzyma obiektu i nie jest już w locie
        if (!player.GetComponent<Interactor>().IsHoldingObject() && !isPlayerFlying)
        {
            LaunchPlayer();
        }
    }

    // Metoda oblicza kierunek wystrzału z uwzględnieniem kąta
    private Vector3 GetAngledDirection()
    {
        // Tworzymy kwaternion reprezentujący obrót o kąt 'launchAngleDegrees'
        // wokół lokalnej osi X (firePoint.right) firePointa.
        Quaternion rotation = Quaternion.AngleAxis(launchAngleDegrees, firePoint.right);
        
        // Mnożymy początkowy kierunek firePoint.forward przez ten kwaternion.
        return rotation * firePoint.forward;
    }

    // Rozpoczyna proces wystrzelenia gracza
    void LaunchPlayer()
    {
        if (player == null || playerRigidbody == null)
        {
            Debug.LogError("Cannot launch player: Player object or Rigidbody not assigned/found.", this);
            return;
        }

        // KROK 1: Przenieś gracza do pozycji lufy
        player.transform.position = firePoint.position;

        // KROK 2: Wyłącz standardowe sterowanie gracza (jeśli jest)
        // if (playerMovementController != null)
        // {
        //     playerMovementController.enabled = false; // Wyłącz skrypt ruchu gracza
        // }

        // KROK 3: Ustaw Rigidbody gracza na kinetyczne
        // Ważne: To wyłącza wpływ fizyki Unity na gracza na czas lotu
        playerRigidbody.isKinematic = true;

        // KROK 4: Zapisz początkowe parametry lotu
        flightInitialPosition = player.transform.position;
        flightInitialVelocity = GetAngledDirection().normalized * initialSpeed; // Upewnij się, że kierunek jest znormalizowany
        flightStartTime = Time.time;
        isPlayerFlying = true; // Rozpocznij lot
    }

    // Aktualizuje pozycję gracza podczas lotu parabolicznego
    void UpdatePlayerFlight()
    {
        float elapsedTime = Time.time - flightStartTime;

        // Obliczamy nową pozycję na podstawie równań ruchu parabolicznego
        // Pozycja = PozycjaPoczątkowa + PrędkośćPoczątkowa * Czas + 0.5 * Grawitacja * Czas^2
        Vector3 gravityEffect = Physics.gravity * 0.5f * (elapsedTime * elapsedTime);
        Vector3 newPosition = flightInitialPosition + (flightInitialVelocity * elapsedTime) + gravityEffect;

        // Ustawiamy pozycję gracza
        player.transform.position = newPosition;

        // Opcjonalnie: obracamy gracza, aby "patrzył" w kierunku ruchu
        Vector3 currentVelocity = flightInitialVelocity + Physics.gravity * elapsedTime;
        if (currentVelocity != Vector3.zero)
        {
            player.transform.forward = currentVelocity.normalized;
        }

        // KROK 5: Warunek zakończenia lotu
        // To jest kluczowe! Zastąp to realistyczną logiką lądowania.
        // Najlepszym sposobem byłoby sprawdzenie kolizji, kiedy gracz dotknie ziemi.
        // Na potrzeby tego przykładu użyjemy prostego warunku Y (np. gdy spadnie poniżej początkowego Y minus jakiś bufor)
        if (player.transform.position.y <= flightInitialPosition.y - 0.5f && elapsedTime > 0.5f) // Dodano minimalny czas lotu, żeby od razu nie lądował
        {
            // Możesz też sprawdzić kolizje za pomocą raycastingu w dół,
            // lub czekać na zdarzenie OnCollisionEnter na graczu.
            StopPlayerFlight();
        }
    }

    // Kończy lot gracza i przywraca jego kontrolę
    void StopPlayerFlight()
    {
        isPlayerFlying = false;

        if (playerRigidbody != null)
        {
            playerRigidbody.isKinematic = false; // Włącz z powrotem wpływ fizyki
            // Opcjonalnie: Zresetuj prędkość Rigidbody, jeśli nie chcesz, by gracz od razu się poruszał po włączeniu fizyki
            // playerRigidbody.velocity = Vector3.zero;
            // playerRigidbody.angularVelocity = Vector3.zero;
        }

        // KROK 6: Włącz standardowe sterowanie gracza (jeśli było wyłączone)
        // if (playerMovementController != null)
        // {
        //     playerMovementController.enabled = true; // Włącz skrypt ruchu gracza
        // }
        
        Debug.Log("Gracz zakończył lot paraboliczny.");
    }

    // WAŻNA UWAGA:
    // Aby lot zakończył się realistycznie (np. na ziemi),
    // zaleca się, aby na obiekcie gracza (lub jego podrzędnym colliderze)
    // dodać skrypt, który wywoła metodę StopPlayerFlight() z tego kontrolera
    // gdy gracz skoliduje z terenem.
    // Przykład:
    // W skrypcie na graczu:
    // void OnCollisionEnter(Collision collision)
    // {
    //     // Sprawdź, czy kolidujesz z ziemią/platformą
    //     if (collision.gameObject.CompareTag("Ground")) // Dodaj tag "Ground" do terenu
    //     {
    //         // Tutaj musisz znaleźć referencję do swojego CannonController
    //         // Najlepiej, jeśli CannonController sam by się "zarejestrował"
    //         // albo gracz miałby referencję do aktywnego CannonController.
    //         // Dla uproszczenia: możesz mieć publiczną statyczną instancję CannonController
    //         // albo znaleźć go przez FindObjectOfType
    //         CannonController activeCannon = FindObjectOfType<CannonController>(); // NIEZALECANE W PRODUKCJI, ZNAJDŹ LEPSZY SPOSÓB
    //         if (activeCannon != null && activeCannon.isPlayerFlying)
    //         {
    //             activeCannon.StopPlayerFlight();
    //         }
    //     }
    // }
}
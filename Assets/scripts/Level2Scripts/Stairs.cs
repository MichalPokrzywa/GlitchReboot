using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Stairs : MonoBehaviour
{
    public GameObject waypointParent; // Obiekt zawierający waypointy
    public VariablePlatform platformx;
    public VariablePlatform platformy;
    public VariablePlatform platformz;
    public VariablePlatform platformBaseHight;
    public TerminalCanvas penis;

    public string logika = "";
    [SerializeField] private List<GameObject> waypoints;
    [FormerlySerializedAs("basicTransform")] [SerializeField] private Vector3 basicPosition;
    
    private string logika_naturalna;
    private string logika_c_hash;
    private float czasDoZmiany;
    public float czasZmianyLogiki = 3f;
    // ----------------

    // ----------------
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        basicPosition = transform.position;
        
    }

    public string getLogika()
    {
        return logika;
    }
    void Start()
    {
        waypoints = new List<GameObject>(); // Inicjalizujemy listę, na wszelki wypadek
        if (waypointParent == null)
        {
            // Debug.LogError("Stairs: waypointParent nie został przypisany w Inspektorze!");
            return; // Przerywamy działanie, jeśli nie ma rodzica
        }


        foreach (Transform child in waypointParent.transform)
        {
            if (child == null)
            {
                continue; // Przechodzimy do następnej iteracji
            }
            waypoints.Add(child.gameObject);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        czasDoZmiany -= Time.deltaTime; // Odliczamy czas
        logika_naturalna = "Małpa kładzie " + platformx.variableValue + " na X, schody układają się w odległości " 
                           + platformx.variableValue + " na osi X, podobnie będzie z osią Z i Y. Małpa sprawdzi małpa zrozumie gdzie osie." +
                           "Platforma Base Height opisuje na jakiej wysokości schodzy zaczynają swoje układanie."+
                           "aktualne wartości X="+platformx.variableValue+" Y="+platformy.variableValue+" " +
                           "Z="+platformz.variableValue+" Base Height="+platformBaseHight.variableValue+".";
        
        logika_c_hash = "int iteration = 0;\n        " +
                        "foreach (GameObject schodek in schody)\n        " +
                        "{\n            Vector3 newPosition = transform.position + new Vector3(" +
                        "\n                " + platformx.variableValue + " * iteration,\n" +
                        "                " + platformy.variableValue + " * iteration,\n" +
                        "                " + platformz.variableValue + " * iteration\n" +
                        "            );\n            schodek.transform.position = newPosition;\n" +
                        "\n            iteration++;\n        }";
        
        if (czasDoZmiany <= 0f)
        {
            // Przełącz logikę
            if (logika == logika_naturalna)
            {
                logika = logika_c_hash;
            }
            else
            {
                logika = logika_naturalna;
            }
            penis.SetCodeText(logika); // Aktualizujemy tekst w obiekcie penis
            czasDoZmiany = czasZmianyLogiki; // Resetujemy licznik
        }
        
        
        penis.SetCodeText(logika);
        if (waypoints == null || waypoints.Count == 0)
        {
            return; // Przerywamy działanie, jeśli nie ma waypointów
        }

        if (!platformx || !platformy || !platformz)
        {
            return; // Przerywamy działanie, jeśli brakuje platform
        }
        transform.position = basicPosition+new Vector3(0,platformBaseHight.variableValue,0);
        Debug.Log("basicTransform.position:"+basicPosition);
        
        int iteration = 0;
        foreach (GameObject waypoint in waypoints)
        {
            Vector3 newPosition = transform.position + new Vector3(
                platformx.variableValue * iteration,
                platformy.variableValue * iteration,
                platformz.variableValue * iteration
            );
            waypoint.transform.position = newPosition;
            // Debug.Log("Stairs: Ustawiono pozycję waypointa " + waypoint.name + " na: " + newPosition);
            iteration++;
        }
    }
}
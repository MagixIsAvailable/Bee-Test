using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement; // Needed to reload level

public class GameOverManager : MonoBehaviour
{
    [Header("UI Components")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI factText;

    [Header("Data")]
    // Paste our fact list here!
    private string[] beeFacts = new string[]
    {
        "Honey bees fly about 15 mph—faster than most people can run!",
        "To make 1lb of honey, bees visit 2 million flowers.",
        "Bees use the sun as a compass, even when it's cloudy.",
        "A bee's wings beat 200 times per second.",
        "Bees can see Ultraviolet (UV) light to find nectar.",
        "A hive's queen can live for several years.",
        "Worker bees effectively work themselves to death in 6 weeks during summer.",
        "Bees perform a 'waggle dance' to share map coordinates.",
        "Honey never spoils; 3,000-year-old honey is still edible!",
        "Male bees (drones) have no stingers and do no work."
    };
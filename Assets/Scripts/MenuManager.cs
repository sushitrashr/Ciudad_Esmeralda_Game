using UnityEngine;
using UnityEngine.SceneManagement; // Necesario para cambiar de escena

public class MenuManager : MonoBehaviour
{
    // -------------------------
    // FUNCIONES PRINCIPALES
    // -------------------------

    // Botón "PLAY" -> Carga el VIDEO DE INTRODUCCIÓN
    public void Play()
    {
        // Asegúrate de que la escena del video se llame exactamente así en Unity
        SceneManager.LoadScene("IntroVideo");
    }

    // Botón "LEVEL MENU"
    public void CargarMenuNiveles()
    {
        SceneManager.LoadScene("LevelMenu");
    }

    // Botón "CIUDAD"
    public void CargarCiudadMenu()
    {
        SceneManager.LoadScene("CiudadMenu");
    }

    // Botón "CONTROLS"
    public void CargarMenuControles()
    {
        SceneManager.LoadScene("ControlsMenu");
    }

    // Botón "SKINS"
    public void CargarMenuSkins()
    {
        SceneManager.LoadScene("SkinsMenu");
    }

    // -------------------------
    // CAPÍTULOS DIRECTOS
    // -------------------------

    public void CargarNivel1()
    {
        SceneManager.LoadScene("Chapter1");
    }

    public void CargarNivel2()
    {
        SceneManager.LoadScene("Chapter2");
    }

    public void CargarNivel3()
    {
        SceneManager.LoadScene("Chapter3");
    }

    // -------------------------
    // SISTEMA
    // -------------------------

    public void VolverAlMenuPrincipal()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }
}
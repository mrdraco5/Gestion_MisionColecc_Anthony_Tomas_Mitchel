using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class MiniJuego : MonoBehaviour
{
    public UIManager uiManager;

    [Header("Elementos")]
    public GameObject botonAccion;
    public GameObject imagenMuñeco;
    public TMP_Text textoIndicacion;

    private int contador = 0;
    private string tipoMisionActual;

    public void IniciarMiniJuego(string tipo)
    {
        contador = 0;
        tipoMisionActual = tipo;

        botonAccion.SetActive(false);
        imagenMuñeco.SetActive(false);

        if (tipo == "click3")
        {
            botonAccion.SetActive(true);
            textoIndicacion.text = "Presiona el botón 3 veces para abrir la puerta";
        }
        else if (tipo == "muñeco")
        {
            imagenMuñeco.SetActive(true);
            textoIndicacion.text = "Golpea (da clic) al muñeco de práctica";
        }
        else if (tipo == "teclaE")
        {
            textoIndicacion.text = "Presiona la letra E para invocar tu energía ancestral";
        }

        gameObject.SetActive(true);
    }

    public void BotonPresionado()
    {
        contador++;

        if (contador >= 3)
        {
            Terminar();
        }
    }

    public void MuñecoPresionado()
    {
        Terminar();
    }

    void Update()
    {
        if (tipoMisionActual == "teclaE")
        {
            if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
            {
                Terminar();
            }
        }
    }

    void Terminar()
    {
        gameObject.SetActive(false);
        uiManager.FinalizarMision();
    }
}
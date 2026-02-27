using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    public DataManager dataManager;

    [Header("Paneles")]
    public GameObject panelBienvenida;
    public GameObject panelJuego;
    public GameObject panelFelicidades;
    public GameObject panelMiniJuego;
    public GameObject panelJuegoCompletado;
    public GameObject panelInventario;

    [Header("Scroll Misiones")]
    public Transform contentMisiones;
    public GameObject itemMisionPrefab;
    public MiniJuego miniJuego;

    [Header("Detalle Misión")]
    public TMP_Text tituloMisionTexto;
    public TMP_Text descripcionTexto;
    public GameObject botonIniciarMision;
    private Mision misionSeleccionada;

    [Header("Detalle Recompensa")]
    public TMP_Text nombreTexto;
    public TMP_Text rarezaTexto;
    public TMP_Text valorTexto;
    public Image iconoImagen;

    [Header("Inventario Scroll")]
    public Transform contentInventario;
    public GameObject coleccionableInventario;

    [Header("Felicidades")]
    public TMP_Text textoFelicidades;
    public Image imagenFelicidades;

    [Header("Inventario")]
    public TMP_Text nombreInvTexto;
    public TMP_Text rarezaInvTexto;
    public TMP_Text valorInvTexto;
    public Image iconoInvImagen;


    public void CargarJuego()
    {
        dataManager.CargarDatos();
        panelBienvenida.SetActive(false);
        panelJuegoCompletado.SetActive(false);
        panelInventario.SetActive(false);
        panelJuego.SetActive(true);

        MostrarListaMisiones();
        LimpiarPanelDetalle();
        MostrarInventario();

        botonIniciarMision.SetActive(false);
    }

    void MostrarListaMisiones()
    {
        foreach (Transform child in contentMisiones)
            Destroy(child.gameObject);

        Mision misionDisponible = dataManager.ObtenerMisionActual();

        foreach (Mision m in dataManager.misionesStack)
        {
            GameObject item = Instantiate(itemMisionPrefab, contentMisiones);
            TMP_Text texto = item.GetComponentInChildren<TMP_Text>();
            Button boton = item.GetComponent<Button>();

            texto.text = m.titulo;

            if (m == misionDisponible)
            {
                boton.interactable = true;
                boton.onClick.AddListener(() =>
                {
                    SeleccionarMision(m);
                });
            }
            else
            {
                boton.interactable = false;
                texto.text += " (Bloqueada)";
                texto.color = Color.gray;
            }
        }
    }

    void SeleccionarMision(Mision mision)
    {
        botonIniciarMision.SetActive(true);
        misionSeleccionada = mision;
        tituloMisionTexto.text = mision.titulo;
        descripcionTexto.text = mision.descripcion;

        Coleccionable coleccionable = dataManager.BuscarColeccionablePorNombre(mision.nombreColeccionable);

        if (coleccionable != null)
        {
            nombreTexto.text = coleccionable.nombre;
            rarezaTexto.text = coleccionable.rareza;
            valorTexto.text = "Valor: " + coleccionable.valor;

            Sprite sprite = Resources.Load<Sprite>(coleccionable.iconoId);
            iconoImagen.sprite = sprite;
            iconoImagen.gameObject.SetActive(true);

            if (coleccionable.rareza.Equals("Legendaria"))
                rarezaTexto.color = Color.yellow;
            else if (coleccionable.rareza.Equals("Epica"))
                rarezaTexto.color = Color.magenta;
            else if (coleccionable.rareza.Equals("Especial"))
                rarezaTexto.color = Color.cyan;
            else
                rarezaTexto.color = Color.white;
        }
    }

    void LimpiarPanelDetalle()
    {
        tituloMisionTexto.text = "";
        descripcionTexto.text = "";
        nombreTexto.text = "";
        rarezaTexto.text = "";
        valorTexto.text = "";
        iconoImagen.sprite = null;
        iconoImagen.gameObject.SetActive(false);
    }

    public void IniciarMision()
    {
        if (misionSeleccionada == null)
            return;

        if (misionSeleccionada != dataManager.ObtenerMisionActual())
            return;

        panelMiniJuego.SetActive(true);

        if (misionSeleccionada.id == 1)
            miniJuego.IniciarMiniJuego("click3");

        else if (misionSeleccionada.id == 2)
            miniJuego.IniciarMiniJuego("muñeco");

        else if (misionSeleccionada.id == 3)
            miniJuego.IniciarMiniJuego("teclaE");
    }

    public void FinalizarMision()
    {
        if (misionSeleccionada == null)
            return;

        Coleccionable recompensa = dataManager.CompletarMision();
        botonIniciarMision.SetActive(false);

        if (recompensa != null)
        {
            panelFelicidades.SetActive(true);
            textoFelicidades.text =
                "Felicidades!\nHas obtenido:\n" + recompensa.nombre;

            Sprite sprite = Resources.Load<Sprite>(recompensa.iconoId);
            imagenFelicidades.sprite = sprite;
        }

        misionSeleccionada = null;

        MostrarListaMisiones();
        MostrarInventario();
        LimpiarPanelDetalle();
    }

    public void CerrarPanelFelicidades()
    {
        panelFelicidades.SetActive(false);
        if (dataManager.misionesStack.Count == 0)
        {
            panelJuego.SetActive(false);
            panelJuegoCompletado.SetActive(true);
        }
    }

    void MostrarInventario()
    {
        foreach (Transform child in contentInventario)
            Destroy(child.gameObject);

        foreach (Coleccionable coleccionable in dataManager.inventarioJugador)
        {
            GameObject item = Instantiate(coleccionableInventario, contentInventario);
            item.GetComponentInChildren<TMP_Text>().text = coleccionable.nombre;

            Button btn = item.GetComponent<Button>();
            btn.onClick.AddListener(() =>
            {
                MostrarDetalleInventario(coleccionable);
            });
        }
    }

    void MostrarDetalleInventario(Coleccionable coleccionable)
    {
        nombreInvTexto.gameObject.SetActive(true);
        rarezaInvTexto.gameObject.SetActive(true);
        valorInvTexto.gameObject.SetActive(true);
        iconoInvImagen.gameObject.SetActive(true);

        nombreInvTexto.text = coleccionable.nombre;
        rarezaInvTexto.text = coleccionable.rareza;
        valorInvTexto.text = "Valor: " + coleccionable.valor;

        Sprite sprite = Resources.Load<Sprite>(coleccionable.iconoId);
        iconoInvImagen.sprite = sprite;

        if (coleccionable.rareza.Equals("Legendaria"))
            rarezaInvTexto.color = Color.yellow;
        else if (coleccionable.rareza.Equals("Epica"))
            rarezaInvTexto.color = Color.magenta;
        else if (coleccionable.rareza.Equals("Especial"))
            rarezaInvTexto.color = Color.cyan;
        else
            rarezaInvTexto.color = Color.white;
    }

    void OcultarDetalleInventario()
    {
        nombreInvTexto.gameObject.SetActive(false);
        rarezaInvTexto.gameObject.SetActive(false);
        valorInvTexto.gameObject.SetActive(false);
        iconoInvImagen.gameObject.SetActive(false);
    }

    public void AbrirInventario()
    {
        panelInventario.SetActive(true);
        MostrarInventario();
        OcultarDetalleInventario();
    }

    public void CerrarInventario()
    {
        panelInventario.SetActive(false);
    }

    public void BotonUndo()
    {
        dataManager.Revertir();

        MostrarListaMisiones();
        MostrarInventario();
        LimpiarPanelDetalle();
    }
}
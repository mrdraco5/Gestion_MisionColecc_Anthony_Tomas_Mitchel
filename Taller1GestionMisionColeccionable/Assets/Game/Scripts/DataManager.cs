using UnityEngine;
using System.Collections.Generic;

public class DataManager : MonoBehaviour
{
    public List<Coleccionable> listaColeccionables = new List<Coleccionable>();
    public List<Coleccionable> inventarioJugador = new List<Coleccionable>();
    public Stack<Mision> misionesStack = new Stack<Mision>();
    public Stack<Mision> historialStack = new Stack<Mision>();

    private GameData gameData;
    public void CargarDatos()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("GameData");

        if (jsonFile == null)
        {
            Debug.LogError("No se encontró GameData.json");
            return;
        }

        gameData = JsonUtility.FromJson<GameData>(jsonFile.text);

        listaColeccionables = gameData.coleccionables;

        misionesStack.Clear();
        historialStack.Clear();
        inventarioJugador.Clear();

        for (int i = gameData.misiones.Count - 1; i >= 0; i--)
        {
            misionesStack.Push(gameData.misiones[i]);
        }
    }

    public Coleccionable BuscarColeccionablePorNombre(string nombre)
    {
        foreach (Coleccionable col in listaColeccionables)
        {
            if (col.nombre.Equals(nombre))
                return col;
        }
        return null;
    }

    public Mision ObtenerMisionActual()
    {
        if (misionesStack.Count > 0)
            return misionesStack.Peek();

        return null;
    }

    public Coleccionable CompletarMision()
    {
        if (misionesStack.Count == 0)
            return null;

        Mision completada = misionesStack.Pop();
        historialStack.Push(completada);
        Coleccionable recompensa = BuscarColeccionablePorNombre(completada.nombreColeccionable);

        if (recompensa != null)
            inventarioJugador.Add(recompensa);

        return recompensa;
    }

    public void Revertir()
    {
        if (historialStack.Count > 0)
        {
            Mision ultima = historialStack.Pop();
            misionesStack.Push(ultima);

            Coleccionable recompensa = BuscarColeccionablePorNombre(ultima.nombreColeccionable);

            if (recompensa != null && inventarioJugador.Contains(recompensa))
                inventarioJugador.Remove(recompensa);
        }
    }
}
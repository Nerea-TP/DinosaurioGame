using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using Firebase.Firestore;
using UnityEngine.UI;
using System.Threading.Tasks;
using System;

public class RankingManager : MonoBehaviour
{
    private FirebaseAuth Auth;
    private FirebaseFirestore Firestore;

    // Asigna los elementos de texto en el Inspector
    [SerializeField] Text[] playerIdTexts; // Array para los textos de Player IDs
    [SerializeField] Text[] scoreTexts;
    async void Start()
    {
        // auth = FirebaseManager.Instance.Auth;
        Firestore = FirebaseManager.Instance.Firestore;
         if (Firestore == null)
        {
            Debug.LogError("Firestore no está inicializado. No se puede cargar el ranking.");
            return; // Salir si Firestore no está inicializado
        }
        await LoadRanking();
    }
    
    private async Task LoadRanking()
    {
        // Limpiar los textos antes de cargar los nuevos puntajes
        foreach (var text in playerIdTexts)
        {
            text.text = "";
        }
        foreach (var text in scoreTexts)
        {
            text.text = "";
        }
        try
        {
            QuerySnapshot rankingSnapshot = await Firestore.Collection("Ranking")
                .OrderByDescending("score") // Ordenar de mayor a menor
                .Limit(5) // Limitar a los 5 primeros
                .GetSnapshotAsync();

            int rank = 0; // Inicializar el rango

            foreach (DocumentSnapshot document in rankingSnapshot.Documents)
            {
                string playerId = document.GetValue<string>("playerId");
                int score = document.GetValue<int>("score");
                if (rank < playerIdTexts.Length)
                {
                    playerIdTexts[rank].text = playerId;
                    scoreTexts[rank].text = score.ToString();
                }
                rank++; // Incrementar el rango
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
        }
    }
}

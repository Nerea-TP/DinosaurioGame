using System.Threading.Tasks;
using UnityEngine;
using Firebase.Firestore;
using Firebase.Auth;
using System.Collections.Generic;
using System.Linq;

public class ScoreManager : MonoBehaviour
{
    private FirebaseAuth auth;
    private FirebaseFirestore firestore;
    public static ScoreManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Mantener en todas las escenas
        }
        else
        {
            Destroy(gameObject); // Elimina duplicados
        }
    }
    void Start()
    {
        // Inicializamos FirebaseAuth y FirebaseFirestore desde el FirebaseManager
        auth = FirebaseManager.Instance.Auth;
        firestore = FirebaseManager.Instance.Firestore;
    }

    // Método para guardar el puntaje en Firestore si supera el umbral
    public async Task SaveScoreIfHighEnough(int score)
    {
        const int requiredScore = 4000; // Cambia el umbral según sea necesario

        // Solo guardar el puntaje si es igual o superior al requerido
        if (score >= requiredScore)
        {
            var user = auth.CurrentUser;
            if (user != null)
            {
                string playerId = user.Email;

                // Crea una referencia al documento del jugador
                DocumentReference docRef = firestore.Collection("Ranking").Document(playerId);
                Dictionary<string, object> data = new Dictionary<string, object>
            {
                { "playerId", playerId },
                { "score", score }
            };

                try
                {
                    // Obtener todos los documentos del ranking
                    QuerySnapshot rankingSnapshot = await firestore.Collection("Ranking")
                        .OrderByDescending("score") // Ordena de mayor a menor
                        .Limit(5) // Obtiene solo los 5 mejores
                        .GetSnapshotAsync();

                    // Convertir el IEnumerable a una lista para acceder a los elementos
                    List<DocumentSnapshot> documents = rankingSnapshot.Documents.ToList();

                    // Verifica si el jugador ya tiene un documento
                    DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
                    if (snapshot.Exists)
                    {
                        // Si el documento ya existe, obtiene la puntuación anterior
                        int previousScore = snapshot.GetValue<int>("score");
                        // Si el documento ya existe, actualiza solo la puntuación
                        if (score > previousScore)
                        {
                            await docRef.UpdateAsync("score", score);
                            Debug.Log("Puntuación actualizada exitosamente en Firestore");
                        }
                    }
                    else
                    {
                        // Si el documento no existe, verifica si ya hay 5 puntuaciones
                        if (documents.Count >= 5)
                        {
                            // Elimina la puntuación más baja
                            DocumentSnapshot lowestScoreDoc = documents[documents.Count - 1];
                            await firestore.Collection("Ranking").Document(lowestScoreDoc.Id).DeleteAsync();
                            Debug.Log("Puntuación más baja eliminada");
                        }

                        // Guarda la nueva puntuación
                        await docRef.SetAsync(data);
                        Debug.Log("Puntuación guardada exitosamente en Firestore");
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError("Error al guardar la puntuación: " + e.Message);
                }
            }
            else
            {
                Debug.LogWarning("No hay usuario autenticado. No se puede guardar la puntuación.");
            }
        }
    }
}
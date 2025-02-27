namespace Rendu1;

public class Graphe
{
    private Dictionary<int, List<int>> listeAdjacence = new();
    private bool[,] matriceAdjacence;
    private int nbSommets;

    public Graphe(int nbSommets)
    {
        this.nbSommets = nbSommets;
        matriceAdjacence = new bool[nbSommets, nbSommets];

        for (int i = 0; i < nbSommets; i++)
        {
            listeAdjacence[i] = new List<int>(); //création d'une liste vide pour chaque sommet
        }
        
    }

    public void AjouterLien(int a, int b)
    {
        //Pour la liste d'adjancence 
        listeAdjacence[a].Add(b);
        listeAdjacence[b].Add(a);
        
        //Pour ne matrice d'adjacences 
        matriceAdjacence[a, b] = true;
        matriceAdjacence[b, a] = true;
    }

    public void AfficherListeAdjacence()
    {
        foreach (var VARIABLE in listeAdjacence)
        {
            Console.WriteLine($"{VARIABLE.Key} -> {string.Join(", ", VARIABLE.Value)}");
            //Console.Write(VARIABLE.Key + "->");//key correspond au sommet
            //Console.WriteLine(VARIABLE.Value);//value correspond aux sommets auquel il est lié
        }
    }

    public void AfficherMatriceAdjacence()
    {
        Console.Write("   "); // Espacement pour l'en-tête
        for (int i = 0; i < nbSommets; i++)
            Console.Write($"{i,2} "); // Afficher les indices des colonnes
        Console.WriteLine("\n  " + new string('-', nbSommets * 3));

        for (int i = 0; i < nbSommets; i++)
        {
            Console.Write($"{i,2} | "); // Afficher l'indice du sommet
            for (int j = 0; j < nbSommets; j++)
            {
                Console.Write(matriceAdjacence[i, j] ? "1 " : "0 "); 
            }
            Console.WriteLine();
        }
    }
}
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
            listeAdjacence[i] = new List<int>(); ///création d'une liste vide pour chaque sommet
        }
        
    }
    
    public Dictionary<int, List<int>> ListeAdjacence { get => listeAdjacence; }
    public int NbSommets { get => nbSommets; }
    public bool[,] MatriceAdjacence { get => matriceAdjacence; }

    public void AjouterLien(int a, int b)
    {
        ///Pour la liste d'adjancence 
        listeAdjacence[a].Add(b);
        listeAdjacence[b].Add(a);
        
        ///Pour ne matrice d'adjacences 
        matriceAdjacence[a, b] = true;
        matriceAdjacence[b, a] = true;
    }

    public void AfficherListeAdjacence()
    {
        foreach (var VARIABLE in listeAdjacence)
        {
            Console.WriteLine($"{VARIABLE.Key} -> {string.Join(", ", VARIABLE.Value)}");
        }
    }

    public void AfficherMatriceAdjacence()
    {
        Console.Write("   "); /// Espacement pour l'en-tête
        for (int i = 0; i < nbSommets; i++)
            Console.Write($"{i,2} "); /// Afficher les indices des colonnes
        Console.WriteLine("\n  " + new string('-', nbSommets * 3));

        for (int i = 0; i < nbSommets; i++)
        {
            Console.Write($"{i,2} | "); /// Afficher l'indice du sommet
            for (int j = 0; j < nbSommets; j++)
            {
                Console.Write(matriceAdjacence[i, j] ? "1 " : "0 "); 
            }
            Console.WriteLine();
        }
    }
    public void ParcoursEnProfondeurRec(int sommet, bool[] visite)
    {
        Console.Write(sommet + " ");
        visite[sommet] = true;
        Stack<int> pile = new Stack<int>();
        foreach(int voisin in listeAdjacence[sommet])
        {
            if (visite[voisin] == false)
            {
                ParcoursEnProfondeurRec(voisin, visite);
            }
        }
    }
    public void ParcoursEnProfondeur(int sommet)
    {
        int max = nbSommets;
        bool[] visite = new bool[max];
        Console.WriteLine("Parcours en Profondeur : ");
        Console.Write("[");
        ParcoursEnProfondeurRec(sommet, visite);
        Console.WriteLine("]");
    }
    
    public void ParcoursEnLargeur(int depart)
    {
        bool[] visited = new bool[nbSommets];
        Queue<int> file = new Queue<int>(); 

        file.Enqueue(depart);
        visited[depart] = true;

        Console.WriteLine("Parcours en Largeur : ");
        Console.Write("[");

        while (file.Count > 0)
        {
            int sommet = file.Dequeue();
            Console.Write(sommet + " ");

            foreach (int voisin in listeAdjacence[sommet])
            {
                if (visited[voisin]==false)
                {
                    file.Enqueue(voisin);
                    visited[voisin] = true;
                }
            }
        }
        Console.WriteLine("]");
    }
}